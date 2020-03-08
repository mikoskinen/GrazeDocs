using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GrazeDocs.LivePreview
{
    public class LivePreviewMiddleware
    {
        private const string HtmlContentType = "text/html";
        private const string XhtmlContentType = "application/xhtml+xml";
        private readonly RequestDelegate _next;

        public LivePreviewMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalStream = context.Response.Body;

            using (var newStream = new MemoryStream())
            {
                try
                {
                    context.Response.Body = newStream;

                    await _next(context);

                    context.Response.Body = originalStream;

                    newStream.Seek(0, SeekOrigin.Begin);

                    var inject = IsSupportedContentType(context.Response.ContentType) &&
                                 context.Response.StatusCode != 304;
                    if (inject)
                    {
                        var newContent = new StreamReader(newStream).ReadToEnd();

                        var modifiedHtml = InjectScriptTag(newContent);
                        context.Response.ContentLength = modifiedHtml.Length;
                        await context.Response.WriteAsync(modifiedHtml);
                    }
                    else
                    {
                        await newStream.CopyToAsync(originalStream);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
        }

        private static string InjectScriptTag(string originalHtml)
        {
            if (string.IsNullOrWhiteSpace(originalHtml))
            {
                return originalHtml;
            }

            var parser = new AngleSharp.Html.Parser.HtmlParser();
            var document = parser.ParseDocument(originalHtml);

            var signalRScript = document.CreateElement("script");
            signalRScript.SetAttribute("src",
                "https://cdn.jsdelivr.net/npm/@aspnet/signalr@1.1.2/dist/browser/signalr.min.js");
            document.Body.AppendChild(signalRScript);

            var scriptElement = document.CreateElement("script");
            scriptElement.TextContent = @"

""use strict"";

console.log(""Initializing live preview"");

var connection = new signalR.HubConnectionBuilder().withUrl(""/.livepreview"").build();

            connection.on(""Refresh"", function() {
                window.parent.document.location.reload(false);
            });

            connection.start()
                .catch (function (err) {
                return console.error(err.toString());
            });

        ";

            document.Body.AppendChild(scriptElement);

            var sw = new StringWriter();
            document.ToHtml(sw, new AngleSharp.Html.HtmlMarkupFormatter());

            return sw.ToString();
        }

        private static bool IsSupportedContentType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var parts = contentType.Split(';');

            return string.Equals(parts[0].Trim(), HtmlContentType, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(parts[0].Trim(), XhtmlContentType, StringComparison.OrdinalIgnoreCase);
        }
    }
}

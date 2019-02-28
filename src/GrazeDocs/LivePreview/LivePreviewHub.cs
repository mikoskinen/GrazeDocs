using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace GrazeDocs
{
    public class LivePreviewHub : Hub
    {
        public async Task Refresh()
        {
            await Clients.All.SendAsync("Refresh");
        }
    }
}

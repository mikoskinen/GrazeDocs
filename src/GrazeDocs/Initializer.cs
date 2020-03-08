using System;
using System.IO;
using System.Text;
using graze.common;

namespace GrazeDocs
{
    public class Initializer
    {
        public static bool Initialize(GrazeDocsOptions options)
        {
            if (File.Exists(options.ConfigurationFile))
            {
                throw new Exception(
                    $"Directory {Path.GetDirectoryName(options.ConfigurationFile)} can not be initialized. It already contains the GrazeDocs configuration file {options.ConfigurationFile}.");
            }

            var folder = options.InitializeFolder;
            if (string.Equals(folder, "."))
            {
                folder = Environment.CurrentDirectory;
            }

            Console.WriteLine($"Initializing GrazeDocs on {folder}");

            var projectName = "";
            while (string.IsNullOrWhiteSpace(projectName))
            {
                Console.Write("Project name: ");
                projectName = Console.ReadLine();
            }

            Console.Write("Add readme.md as placeholder [y/N]?: ");
            var addInitialReadme = string.Equals(Console.ReadLine(), "y", StringComparison.InvariantCultureIgnoreCase);

            var themeFolder = Path.Combine(folder, "_theme");
            var configurationFile = Path.Combine(folder, "configuration.xml");

            var configurationTemplate = $@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<data>
  <site>
    <Title>{projectName}</Title>
    <Description>{projectName}: Documentation</Description>
	<Logo>/assets/logo.png</Logo>
	<Footer>Copyright 2019</Footer>
	<Navigations>
		<Navigation Url=""/"" ExtraClass=""active"">Documentation</Navigation>
	</Navigations>
	<SecondaryLinks>
		<SecondaryLink Url=""https://grazedocs.io"">Project Home</SecondaryLink>
	</SecondaryLinks>	
  </site>
  <ChildPages Location="""" DefaultPageLayoutFile=""page.cshtml"" IndexLayoutFile=""pagesindex.cshtml"" IndexFileName=""all.html"" FolderPerPage=""false"" ReadmeName=""index""
              TagsIndexLayoutFile=""tagsindex.cshtml"" TagLayoutFile=""tag.cshtml"" RelativePathPrefix="""" IsRoot=""true""
			  RssGenerate=""false"" RssFeedName="""" RssUri="""" RssAuthor="""" RssDescription="""">
    <Groups>
    </Groups>
  </ChildPages>
</data>";
            Directory.CreateDirectory(themeFolder);

            var themeTemplateFolder = Path.Combine(options.GrazeBinFolder, "_theme");
            DirCopy.Copy(themeTemplateFolder, themeFolder);

            File.WriteAllText(configurationFile, configurationTemplate, Encoding.UTF8);

            if (addInitialReadme)
            {
                var readmePath = Path.Combine(folder, "readme.md");
                File.WriteAllText(readmePath, $@"---
title: Welcome
description: Every documentation page can optionally have a description.
tags: grazedocs
---

## Introduction

This is the placeholder for your documentation.

## More information

Please see https://grazedocs.io for more information.");
            }

            return addInitialReadme;
        }
    }
}

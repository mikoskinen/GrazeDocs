---
title: Config File
order: 0
description: Each documentation site has a configuration file which can be used to configure site's details.
---

## Basics

Each GrazeDocs documentation site must have a configuration.xml. This file is used to define the basic required information for the site, like its name and theme.

GrazeDocs uses the configuration format provided by graze: https://github.com/mikoskinen/graze

## Example

Example configuration for a multi-page documentation site:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<data>
  <site>
    <Title>Graze</Title>
    <Description>Graze: Static site generator using Razor</Description>
	<Logo>/assets/logo.png</Logo>
	<Footer>Copyright 2019 Mikael Koskinen</Footer>
	<Navigations>
		<Navigation Url="/" ExtraClass="active">Documentation</Navigation>
	</Navigations>
	<SecondaryLinks>
		<SecondaryLink Url="https://grazedocs.io">Project Home</SecondaryLink>
		<SecondaryLink Url="https://github.com/mikoskinen/graze">Graze</SecondaryLink>
	</SecondaryLinks>	
  </site>
  <ChildPages Location="" DefaultPageLayoutFile="page.cshtml" IndexLayoutFile="pagesindex.cshtml" IndexFileName="all.html" FolderPerPage="false" ReadmeName="index"
              TagsIndexLayoutFile="tagsindex.cshtml" TagLayoutFile="tag.cshtml" RelativePathPrefix=""
			  RssGenerate="false" RssFeedName="" RssUri="" RssAuthor="" RssDescription="">
    <Groups>
      <Group Key="">Home</Group>
      <Group Key="architecture">My Architecture</Group>
      <Group Key="about">About</Group>
    </Groups>
  </ChildPages>
</data>
```
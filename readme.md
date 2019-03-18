# GrazeDocs - Lightweight Static Documentation Generator

![GrazeDocs Logo](https://github.com/mikoskinen/GrazeDocs/raw/master/logo/grazedocs_logo_blue_front_small.png "Logo")

GrazeDocs is a static site generator for creating documentation for your projects. The content is created using Markdown which GrazeDocs converts into a beautiful web site. GrazeDocs provides a live preview feature which allows you to focus on writing your content.  

The default theme is optimized for technical documentation sites.

# Getting Started

GrazeDocs is available as a global tool for .NET Core. To install:

```
dotnet tool install -g GrazeDocs
```

![GrazeDocs Installation](https://github.com/mikoskinen/GrazeDocs/raw/master/docs/installation.gif "Installation")

To start creating your documentation, use GrazeDocs -i . to initialize documentation into the current folder:

```
GrazeDocs -i .
```

![GrazeDocs Init](https://github.com/mikoskinen/GrazeDocs/raw/master/docs/init.gif "Init")

With GrazeDocs -i, you can initialize the folder with readme.md which can work as the starting point for your project's documentation.

After your happy with the documentation, use GrazeDocs -p to publish your complete site:

```
GrazeDocs -p
```

![GrazeDocs Publish](https://github.com/mikoskinen/GrazeDocs/raw/master/docs/publish.gif "Publish")

For more thorough documentation, visit the GrazeDocs' home page https://grazedocs.io

# Features

* Clean and light default theme
* Markdown to documentation
* Automatically generated table of contents
* Single or multi-page documentations
* Live preview 
* Razor based themes

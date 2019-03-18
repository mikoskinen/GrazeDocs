---
title: Quick Start
order: 1
description: To get started with GrazeDocs, you have to do three things: 1) Install the tool, 2) Initialize the documentation folder and 3) Create and publish the documentation.
---
## Install

GrazeDocs is available as a global tool for .NET Core. To install:

```csharp {.line-numbers}
dotnet tool install -g GrazeDocs
```

![GrazeDocs Installation](https://github.com/mikoskinen/GrazeDocs/raw/master/docs/installation.gif "Installation")

## Initialize 

To start creating your documentation, use GrazeDocs -i . to initialize documentation into the current folder:

```csharp {.line-numbers}
GrazeDocs -i .
```

![GrazeDocs Init](https://github.com/mikoskinen/GrazeDocs/raw/master/docs/init.gif "Init")

With GrazeDocs -i, you can initialize the folder with readme.md which can work as the starting point for your project's documentation.

## Publish 

After your happy with the documentation, use GrazeDocs -p to publish your complete site:

```csharp {.line-numbers}
GrazeDocs -p
```

![GrazeDocs Publish](https://github.com/mikoskinen/GrazeDocs/raw/master/docs/publish.gif "Publish")
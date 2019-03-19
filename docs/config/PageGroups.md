---
title: Page Groups
order: 3
description: Each page of the documentation is part of a group. You can use GrazeDocs to decide the group automatically based on the page's folder or to manually set the group. 
group: My custom group
---

## What is a Page Group?

Each page of the documentation is part of a group:

![](2019-03-19-11-42-36.png)

In the example above, the documentation has four groups:

* Home
* My Architecture
* About
* features

One group can have 1-n amount of pages.

## Automatic Groups

By default GrazeDocs automatically assigns a group for each page based on the directory of the page. For example if page is in features-directory, the page's group is "features".

## Manual Groups

You can manually assign a group for each page using the "group" metadata:

![](2019-03-19-11-47-13.png)

## Customizing Groups

You can define names and orders for the groups using configuration.xml:

```xml
    <Groups>
      <Group Key="">Home</Group>
      <Group Key="architecture">My Architecture</Group>
      <Group Key="about">About</Group>
    </Groups>
```

Key is the directory name so for the root documentation directory the key is empty.
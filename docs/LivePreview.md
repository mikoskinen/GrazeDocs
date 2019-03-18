## What is Live Preview?

One of the key features of GrazeDocs is Live Preview. 

To start Live Preview, use the following command:

```
GrazeDocs -w
```

To close Live Preview, use Ctrl+C.

Live Preview automatically opens
With Live Preview, each new or changed Markdown-file refreshes the preview view of your documentation automatically. Live preview takes care of copying the new assets also, so you can see how your images look like without having to manually publish the site.

## Configuring Live Preview port

By default, Live Preview is started in port 7552. You can configure that with the -t switch:

```
GrazeDocs -w -t 8660
```
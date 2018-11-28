# Hugo.io - Classic Theme

Classic is forked from the **XMin** theme, written by [Yihui Xie](https://yihui.name). I made a fresh remix of it for my [personal blog](https://goodroot.ca). It provides some simplifications, `highlight.js` for syntax highlighting, emoji support, and slick fonts.

### Instructions

1: Install Hugo.

```
brew install hugo
```

2: Create a new site.

```
hugo new site classic
```

3: Change to themes dir.

```
cd classic/themes
```

4: Clone the repo

```
git clone git@github.com:goodroot/hugo-classic.git
```

5: Copy the `content/`, `static/css`, and `config.toml` from the `exampleSite` directory into the classic directory. The existing `content/`, `static/css`, and `config.toml` files will be over-written. 

6: Run `hugo server` within `classic/` and enjoy and customize to your hearts content!

### New Posts

To make new posts, simply use the command line:

```
hugo new post/good-to-great.md
```

### Header Colour

To adjust the header colour, head to `static/css/style.css` and change...

```
header {
    background: #613DC1;
}
```

... `background:` to any colour value you'd like!

For header font:

```
header a {
    color: #fff;
}
```

Change `color:` to a nice matching colour.

#### Screenshot

![Screenshot of Hugo Classic](https://github.com/goodroot/hugo-classic/raw/master/images/screenshot.png)

## Blog Posts

hugo-classic has appeared in...

[15 Hugo Framework blog themes](https://terrty.net/2018/15-hugo-framework-blog-themes/) by [paskal](https://github.com/paskal)

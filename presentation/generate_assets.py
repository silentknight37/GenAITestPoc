# -*- coding: utf-8 -*-
"""Generate light, abstract background images (no icons) for the deck."""
from PIL import Image, ImageDraw, ImageFilter
import os, math

os.makedirs("assets", exist_ok=True)
W, H = 1920, 1080

def lerp(a, b, t): return tuple(int(a[i] + (b[i]-a[i])*t) for i in range(3))

def blob(draw_img, cx, cy, r, color, alpha):
    """soft translucent circle on its own layer, returned for compositing."""
    layer = Image.new("RGBA", (W, H), (0,0,0,0))
    d = ImageDraw.Draw(layer)
    d.ellipse([cx-r, cy-r, cx+r, cy+r], fill=color + (alpha,))
    return layer

def make(name, blobs, lines=None, base=(255,255,255), grain=False):
    img = Image.new("RGBA", (W, H), base + (255,))
    for (cx, cy, r, color, alpha) in blobs:
        img = Image.alpha_composite(img, blob(img, cx, cy, r, color, alpha))
    # heavy blur for soft mesh-gradient feel
    img = img.filter(ImageFilter.GaussianBlur(120))
    # thin abstract geometry on top (very subtle)
    if lines:
        ov = Image.new("RGBA", (W, H), (0,0,0,0))
        d = ImageDraw.Draw(ov)
        for (x1,y1,x2,y2,c,a,w) in lines:
            d.line([x1,y1,x2,y2], fill=c+(a,), width=w)
        img = Image.alpha_composite(img, ov)
    img.convert("RGB").save(f"assets/{name}", quality=95)
    print("wrote assets/" + name)

BLUE=(47,95,224); TEAL=(18,179,160); VIO=(138,108,240); AMBER=(242,164,49); ROSE=(240,96,138)

# Cover: airy, premium — blue+teal+violet wash with a soft ring of dots
make("bg_cover.png",
     blobs=[(120,140,520,BLUE,70),(1780,260,560,TEAL,70),(1640,1000,620,VIO,55),
            (300,1020,500,AMBER,32),(960,540,700,BLUE,18)],
     lines=[(0,300,1920,180,BLUE,16,2),(0,820,1920,940,TEAL,16,2)])

# Section divider: calmer, one strong corner wash
make("bg_section.png",
     blobs=[(180,160,600,BLUE,60),(1760,940,640,TEAL,60),(1500,160,420,VIO,30)],
     lines=[(0,540,1920,540,BLUE,10,1)])

# Content slides: faint, mostly white so text is crisp
make("bg_content.png",
     blobs=[(60,60,460,BLUE,34),(1880,1040,520,TEAL,34)])

# Closing
make("bg_closing.png",
     blobs=[(960,200,560,BLUE,55),(300,940,520,TEAL,50),(1640,920,520,VIO,45)],
     lines=[(0,360,1920,300,BLUE,14,2),(0,760,1920,820,TEAL,14,2)])
print("done")
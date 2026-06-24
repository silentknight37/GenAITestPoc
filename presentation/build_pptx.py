# -*- coding: utf-8 -*-
"""
MSc defence deck (.pptx) — light & abstract, flat icons (Segoe MDL2), image
backgrounds, readable charts, SLIIT logo, student-number footer, transitions.
Run:  python build_pptx.py
"""
import os
from pptx import Presentation
from pptx.util import Inches, Pt
from pptx.dml.color import RGBColor
from pptx.enum.text import PP_ALIGN, MSO_ANCHOR
from pptx.enum.shapes import MSO_SHAPE
from pptx.enum.chart import (XL_CHART_TYPE, XL_LEGEND_POSITION, XL_LABEL_POSITION)
from pptx.chart.data import CategoryChartData
from pptx.oxml.ns import qn
from pptx.oxml import parse_xml

ASSET = "assets"
STUDENT = "MS24901062"
TOTAL = 33
FEAT_TRANS  = os.environ.get('FT_TRANS','1')=='1'
FEAT_ICONF  = os.environ.get('FT_ICONF','1')=='1'
FEAT_SHADOW = os.environ.get('FT_SHADOW','1')=='1'
OUT = os.environ.get('FT_OUT','GenAI_Testing_Framework_Defence.pptx')

# ---------- palette (tuples) ----------
BLUE=(47,95,224); TEAL=(18,179,160); AMBER=(242,164,49); VIO=(138,108,240); ROSE=(240,96,138)
NAVY=(23,33,64)
INK=(31,42,68); INK2=(74,86,112); MUTED=(124,136,159); LINE=(228,232,240); SOFT=(245,247,252); WHITE=(255,255,255)
MODE_COLORS=[BLUE,TEAL,AMBER,VIO,ROSE]
FONT="Segoe UI"; FONT_L="Segoe UI Light"; FONT_SB="Segoe UI Semibold"; MONO="Consolas"; ICONF="Segoe MDL2 Assets"

# Segoe MDL2 Assets glyphs (canonical, flat — not emoji/AI)
G = dict(search='', gear='', doc='', page='', check='',
         accept='', done='', people='', contact='', flag='',
         info='', library='', home='', link='', refresh='',
         warning='', code='', world='', view='', star='',
         pie='', forward='', target='', lightning='', clipboard='')

def rgb(t): return RGBColor(*t)
def tint(t, f=0.86): return RGBColor(*[int(c+(255-c)*f) for c in t])

prs = Presentation()
prs.slide_width=Inches(13.333); prs.slide_height=Inches(7.5)
SW, SH = 13.333, 7.5
BLANK = prs.slide_layouts[6]
PAGE = {'n':0}

# ---------- primitives ----------
def slide(): return prs.slides.add_slide(BLANK)

def picture_bg(s, name):
    p = os.path.join(ASSET, name)
    if os.path.exists(p):
        pic = s.shapes.add_picture(p, 0, 0, width=prs.slide_width, height=prs.slide_height)
        s.shapes._spTree.remove(pic._element); s.shapes._spTree.insert(2, pic._element)
    else:
        r = s.shapes.add_shape(MSO_SHAPE.RECTANGLE,0,0,prs.slide_width,prs.slide_height)
        r.fill.solid(); r.fill.fore_color.rgb=rgb(WHITE); r.line.fill.background(); r.shadow.inherit=False

def rect(s,l,t,w,h,fill=None,line=None,lw=0.75,rounded=False,radius=0.1):
    w=max(w,0.05); h=max(h,0.05)
    shp=s.shapes.add_shape(MSO_SHAPE.ROUNDED_RECTANGLE if rounded else MSO_SHAPE.RECTANGLE,
                           Inches(l),Inches(t),Inches(w),Inches(h))
    if rounded:
        try: shp.adjustments[0]=radius
        except Exception: pass
    if fill is None: shp.fill.background()
    else: shp.fill.solid(); shp.fill.fore_color.rgb=rgb(fill) if isinstance(fill,tuple) else fill
    if line is None: shp.line.fill.background()
    else: shp.line.color.rgb=rgb(line) if isinstance(line,tuple) else line; shp.line.width=Pt(lw)
    shp.shadow.inherit=False
    return shp

def oval(s,l,t,w,h,fill=None,line=None,lw=1.0):
    shp=s.shapes.add_shape(MSO_SHAPE.OVAL,Inches(l),Inches(t),Inches(w),Inches(h))
    if fill is None: shp.fill.background()
    else: shp.fill.solid(); shp.fill.fore_color.rgb=rgb(fill) if isinstance(fill,tuple) else fill
    if line is None: shp.line.fill.background()
    else: shp.line.color.rgb=rgb(line) if isinstance(line,tuple) else line; shp.line.width=Pt(lw)
    shp.shadow.inherit=False
    return shp

def card(s,l,t,w,h,fill=WHITE,border=LINE,radius=0.08,shadow=True):
    c=rect(s,l,t,w,h,fill=fill,line=border,lw=0.75,rounded=True,radius=radius)
    if shadow: _soft_shadow(c)
    return c

def _soft_shadow(shape):
    if not FEAT_SHADOW: return
    spPr=shape._element.spPr
    for el in spPr.findall(qn('a:effectLst')): spPr.remove(el)  # drop the empty one inherit=False added
    xml=('<a:effectLst xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main">'
         '<a:outerShdw blurRad="90000" dist="38000" dir="5400000" rotWithShape="0">'
         '<a:srgbClr val="1F2A44"><a:alpha val="12000"/></a:srgbClr></a:outerShdw></a:effectLst>')
    spPr.append(parse_xml(xml))

def set_icon_font(run):
    if not FEAT_ICONF: return
    rPr=run._r.get_or_add_rPr()
    for tag in ('a:latin','a:ea','a:cs'):
        e=rPr.find(qn(tag))
        if e is None:
            e=rPr.makeelement(qn(tag),{}); rPr.append(e)
        e.set('typeface',ICONF)

def R(text,size=14,color=INK2,bold=False,font=FONT,italic=False):
    return (text,size,color,bold,font,italic)

def txt(s,l,t,w,h,paras,align=PP_ALIGN.LEFT,anchor=MSO_ANCHOR.TOP,sa=4,ls=1.0,wrap=True):
    w=max(w,0.05); h=max(h,0.05)  # never emit a non-positive extent (PowerPoint rejects it)
    tb=s.shapes.add_textbox(Inches(l),Inches(t),Inches(w),Inches(h)); tf=tb.text_frame
    tf.word_wrap=wrap; tf.vertical_anchor=anchor
    tf.margin_left=0;tf.margin_right=0;tf.margin_top=0;tf.margin_bottom=0
    for i,para in enumerate(paras):
        p=tf.paragraphs[0] if i==0 else tf.add_paragraph()
        p.alignment=align; p.space_after=Pt(sa); p.space_before=Pt(0); p.line_spacing=ls
        for (text,size,color,bold,font,italic) in para:
            r=p.add_run(); r.text=text; r.font.size=Pt(size); r.font.bold=bold
            r.font.italic=italic; r.font.color.rgb=rgb(color) if isinstance(color,tuple) else color
            r.font.name=font
    return tb

def icon(s,l,t,d,glyph,color,bg=None,white=False):
    if bg is not None: oval(s,l,t,d,d,fill=bg)
    tb=s.shapes.add_textbox(Inches(l),Inches(t),Inches(d),Inches(d)); tf=tb.text_frame
    tf.word_wrap=False; tf.vertical_anchor=MSO_ANCHOR.MIDDLE
    tf.margin_left=0;tf.margin_right=0;tf.margin_top=0;tf.margin_bottom=0
    p=tf.paragraphs[0]; p.alignment=PP_ALIGN.CENTER
    r=p.add_run(); r.text=glyph; r.font.size=Pt(int(d*30))
    r.font.color.rgb=rgb(WHITE) if white else (rgb(color) if isinstance(color,tuple) else color)
    r.font.name=ICONF; set_icon_font(r)
    return tb

def add_transition(s, inner, spd="med"):
    if not FEAT_TRANS: return
    sld=s._element
    for t in sld.findall(qn('p:transition')): sld.remove(t)
    xml=('<p:transition xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main" '
         'xmlns:p14="http://schemas.microsoft.com/office/powerpoint/2010/main" spd="%s">%s</p:transition>'%(spd,inner))
    tr=parse_xml(xml)
    clr=sld.find(qn('p:clrMapOvr'))
    (clr if clr is not None else sld.find(qn('p:cSld'))).addnext(tr)

def footer(s):
    txt(s,0.9,7.06,7,0.3,[[R(STUDENT+"  ·  Shanaka Madhusanka",9,MUTED,False,FONT)]])
    txt(s,9.0,7.06,3.43,0.3,[[R(f"{PAGE['n']:02d} / {TOTAL}",9,MUTED,False,MONO)]],align=PP_ALIGN.RIGHT)

def chrome(s,bgname,page=True,trans='<p:fade/>'):
    picture_bg(s,bgname); PAGE['n']+=1
    if page: footer(s)
    add_transition(s,trans)

def kicker(s,text,l=0.9,t=0.5):
    txt(s,l,t,10,0.32,[[R(text.upper(),10.5,BLUE,True,MONO)]])
    rect(s,l+0.02,t+0.34,0.66,0.05,fill=BLUE,rounded=True,radius=0.5)

def header(s,kick,glyph,gcolor,title):
    kicker(s,kick)
    icon(s,0.9,1.04,0.62,glyph,gcolor,bg=tint(gcolor,0.86))
    txt(s,1.68,1.04,10.6,0.62,[[R(title,23,INK,True,FONT_L)]],anchor=MSO_ANCHOR.MIDDLE)

# ---------- composite ----------
def divider(part,title,glyph):
    s=slide(); chrome(s,"bg_section.png",page=True,trans='<p:cover dir="l"/>')
    icon(s,SW/2-0.45,2.0,0.9,glyph,WHITE,bg=BLUE,white=True)
    txt(s,1,3.1,SW-2,0.4,[[R(part.upper(),12.5,BLUE,True,MONO)]],align=PP_ALIGN.CENTER)
    txt(s,1,3.5,SW-2,1.1,[[R(title,38,INK,True,FONT_L)]],align=PP_ALIGN.CENTER)
    rect(s,SW/2-1.7,4.75,1.7,0.07,fill=BLUE,rounded=True,radius=0.5)
    rect(s,SW/2,4.75,1.7,0.07,fill=TEAL,rounded=True,radius=0.5)
    return s

def bullets(s,items,l,t,w,color=BLUE,size=12.5,gap=7):
    tb=s.shapes.add_textbox(Inches(l),Inches(t),Inches(w),Inches(3)); tf=tb.text_frame
    tf.word_wrap=True; tf.margin_left=0;tf.margin_right=0;tf.margin_top=0;tf.margin_bottom=0
    for i,it in enumerate(items):
        p=tf.paragraphs[0] if i==0 else tf.add_paragraph()
        p.space_after=Pt(gap); p.line_spacing=1.06
        m=p.add_run(); m.text="▪  "; m.font.size=Pt(9); m.font.color.rgb=rgb(color); m.font.name=FONT
        for j,seg in enumerate(it.split("**")):
            if not seg: continue
            r=p.add_run(); r.text=seg; r.font.size=Pt(size); r.font.name=FONT
            r.font.bold=(j%2==1); r.font.color.rgb=rgb(INK if j%2==1 else INK2)
    return tb

def blk(s,text,l,t,color=INK,w=6.4):
    txt(s,l,t,w,0.3,[[R(text,13,color,True,FONT_SB)]])

def vflow(s,nodes,l,t,w,nh=0.66,gap=0.26):
    y=t
    for i,(badge,title,sub,col,is_icon) in enumerate(nodes):
        card(s,l,y,w,nh,radius=0.16)
        d=nh-0.24
        if is_icon:
            icon(s,l+0.16,y+0.12,d,badge,col,bg=tint(col,0.85))
        else:
            rect(s,l+0.16,y+0.12,d,d,fill=col,rounded=True,radius=0.3)
            txt(s,l+0.16,y+0.12,d,d,[[R(str(badge),11,WHITE,True,FONT)]],align=PP_ALIGN.CENTER,anchor=MSO_ANCHOR.MIDDLE)
        tx=l+0.16+d+0.2
        if sub:
            txt(s,tx,y+0.06,w-(tx-l)-0.16,nh-0.12,[[R(title,11.5,INK,True,FONT_SB)],[R(sub,9,MUTED,False,FONT)]],
                anchor=MSO_ANCHOR.MIDDLE,sa=1)
        else:
            txt(s,tx,y,w-(tx-l)-0.16,nh,[[R(title,11.5,INK,True,FONT_SB)]],anchor=MSO_ANCHOR.MIDDLE)
        if i<len(nodes)-1:
            txt(s,l,y+nh-0.02,w,gap,[[R("▼",8.5,(196,205,224),False,FONT)]],align=PP_ALIGN.CENTER,anchor=MSO_ANCHOR.MIDDLE)
        y+=nh+gap

def pipeline(s,steps,l,t,w,sh=1.25):
    n=len(steps); aw=0.36; sw=(w-aw*(n-1))/n; x=l
    for i,(num,title,desc,col,glyph) in enumerate(steps):
        card(s,x,t,sw,sh,radius=0.12)
        rect(s,x,t,sw,0.09,fill=col,rounded=True,radius=0.5)
        if glyph: icon(s,x+sw/2-0.26,t+0.16,0.52,glyph,col,bg=tint(col,0.85))
        runs=[]
        if num: runs.append([R(str(num),9,col,True,MONO)])
        runs.append([R(title,11.5,INK,True,FONT_SB)])
        if desc: runs.append([R(desc,8.5,MUTED,False,FONT)])
        top = t+0.72 if glyph else t+0.16
        txt(s,x+0.1,top,sw-0.2,sh-(top-t)-0.12,runs,align=PP_ALIGN.CENTER,anchor=MSO_ANCHOR.TOP,sa=2)
        if i<n-1:
            txt(s,x+sw,t,aw,sh,[[R("→",15,(196,205,224),False,FONT)]],align=PP_ALIGN.CENTER,anchor=MSO_ANCHOR.MIDDLE)
        x+=sw+aw

def icard_grid(s,cards,l,t,w,h,cols=2,vgap=0.24,hgap=0.3):
    rows=(len(cards)+cols-1)//cols
    cw=(w-hgap*(cols-1))/cols; ch=(h-vgap*(rows-1))/rows
    for idx,(glyph,title,body,col) in enumerate(cards):
        r,c=divmod(idx,cols); x=l+c*(cw+hgap); y=t+r*(ch+vgap)
        card(s,x,y,cw,ch,radius=0.09)
        icon(s,x+0.24,y+0.22,0.56,glyph,col,bg=tint(col,0.85))
        txt(s,x+0.24+0.56+0.16,y+0.22,cw-(0.24+0.56+0.16)-0.2,0.56,[[R(title,13,col,True,FONT_SB)]],anchor=MSO_ANCHOR.MIDDLE)
        txt(s,x+0.24,y+0.92,cw-0.48,ch-1.05,[[R(body,10.5,INK2,False,FONT)]],ls=1.08)

def kpi(s,l,t,w,h,num,lbl,color=BLUE,glyph=None):
    card(s,l,t,w,h,radius=0.12)
    if glyph: icon(s,l+w/2-0.26,t+0.16,0.52,glyph,color,bg=tint(color,0.85))
    txt(s,l,t+0.62,w,0.6,[[R(num,30,color,True,FONT_L)]],align=PP_ALIGN.CENTER)
    txt(s,l+0.12,t+h-0.5,w-0.24,0.42,[[R(lbl.upper(),8.5,MUTED,False,FONT)]],align=PP_ALIGN.CENTER)

def badges(s,items,l,t,w):
    x=l;y=t;gap=0.12;bh=0.34;pad=0.16
    for (text,base) in items:
        bw=0.14+len(text)*0.083+pad
        if x+bw>l+w: x=l; y+=bh+0.14
        rect(s,x,y,bw,bh,fill=tint(base,0.9),line=tint(base,0.6),lw=0.75,rounded=True,radius=0.5)
        txt(s,x,y,bw,bh,[[R(text,9,base,True,MONO)]],align=PP_ALIGN.CENTER,anchor=MSO_ANCHOR.MIDDLE)
        x+=bw+gap

def add_table(s,l,t,w,headers,rows,cw,best=None,rh=0.46):
    nrows=len(rows)+1; ncols=len(headers); h=rh*nrows
    gf=s.shapes.add_table(nrows,ncols,Inches(l),Inches(t),Inches(w),Inches(h)); table=gf.table
    table._tbl.tblPr.set('firstRow','0'); table._tbl.tblPr.set('bandRow','0')
    for ci,c in enumerate(cw): table.columns[ci].width=Inches(c)
    for r in table.rows: r.height=Inches(rh)
    for ci,ht in enumerate(headers):
        cell=table.cell(0,ci); cell.fill.solid(); cell.fill.fore_color.rgb=rgb(NAVY)
        cell.vertical_anchor=MSO_ANCHOR.MIDDLE; cell.margin_left=Inches(0.12)
        p=cell.text_frame.paragraphs[0]; p.alignment=PP_ALIGN.LEFT
        r=p.add_run(); r.text=ht; r.font.size=Pt(11); r.font.bold=True; r.font.color.rgb=rgb(WHITE); r.font.name=FONT
    for ri,row in enumerate(rows):
        isb=(best is not None and ri==best)
        for ci,val in enumerate(row):
            cell=table.cell(ri+1,ci); cell.fill.solid()
            cell.fill.fore_color.rgb=rgb((234,250,246) if isb else (WHITE if ri%2==0 else (250,251,253)))
            cell.vertical_anchor=MSO_ANCHOR.MIDDLE; cell.margin_left=Inches(0.12)
            p=cell.text_frame.paragraphs[0]; p.alignment=PP_ALIGN.LEFT
            r=p.add_run(); r.text=val; mono=(ci==0)
            r.font.size=Pt(10.5); r.font.name=MONO if mono else FONT; r.font.bold=mono
            r.font.color.rgb=rgb(TEAL if isb and mono else (INK if mono else INK2))

def clean_chart(chart, legend=False, labels=False, vmin=None, vmax=None):
    chart.font.size=Pt(11); chart.font.name=FONT; chart.has_title=False
    if legend:
        chart.has_legend=True; chart.legend.position=XL_LEGEND_POSITION.BOTTOM
        chart.legend.include_in_layout=False; chart.legend.font.size=Pt(10)
    else:
        chart.has_legend=False
    try:
        va=chart.value_axis
        va.has_minor_gridlines=False
        if labels: va.visible=False; va.has_major_gridlines=False
        else:
            va.has_major_gridlines=True
            gl=va.major_gridlines.format.line; gl.color.rgb=rgb((237,240,246)); gl.width=Pt(0.75)
            va.tick_labels.font.size=Pt(10); va.tick_labels.font.color.rgb=rgb(MUTED)
        if vmin is not None: va.minimum_scale=vmin
        if vmax is not None: va.maximum_scale=vmax
    except Exception: pass
    try:
        ca=chart.category_axis; ca.has_major_gridlines=False
        ca.tick_labels.font.size=Pt(11); ca.tick_labels.font.color.rgb=rgb(INK)
        ca.format.line.color.rgb=rgb((220,226,236))
    except Exception: pass
    if labels:
        plot=chart.plots[0]; plot.has_data_labels=True
        dl=plot.data_labels; dl.number_format='0"%"'; dl.number_format_is_linked=False
        dl.position=XL_LABEL_POSITION.OUTSIDE_END; dl.font.size=Pt(11); dl.font.bold=True; dl.font.color.rgb=rgb(INK)

def use_logo(s,cx,top,h=0.95):
    p=os.path.join(ASSET,"sliit.png")
    if os.path.exists(p):
        pic=s.shapes.add_picture(p,Inches(0),Inches(top),height=Inches(h))
        pic.left=Inches(cx-pic.width/914400/2)
    else:
        # clean swappable SLIIT wordmark (replace assets/sliit.png with the official logo)
        w=3.0
        txt(s,cx-w/2,top,w,0.6,[[R("SLIIT",30,NAVY,True,FONT)]],align=PP_ALIGN.CENTER)
        rect(s,cx-0.5,top+0.62,1.0,0.045,fill=BLUE,rounded=True,radius=0.5)
        txt(s,cx-w/2,top+0.72,w,0.3,[[R("Sri Lanka Institute of Information Technology",9.5,INK2,False,FONT)]],align=PP_ALIGN.CENTER)

# ============================================================ 1 COVER
s=slide(); chrome(s,"bg_cover.png",page=False,trans='<p:fade/>')
use_logo(s,SW/2,0.55,0.95)
txt(s,1,2.15,SW-2,0.4,[[R("MSc RESEARCH DEFENCE  ·  MAY 2026",12,BLUE,True,MONO)]],align=PP_ALIGN.CENTER)
txt(s,0.8,2.58,SW-1.6,1.6,[[R("Generative AI-Augmented",40,INK,True,FONT_L)],
    [R("Testing Framework",40,INK,True,FONT_L)]],align=PP_ALIGN.CENTER,sa=0)
rect(s,SW/2-1.7,4.42,1.7,0.07,fill=BLUE,rounded=True,radius=0.5)
rect(s,SW/2,4.42,1.7,0.07,fill=TEAL,rounded=True,radius=0.5)
txt(s,1,4.62,SW-2,0.5,[[R("for Enterprise Backend Systems — Design, Implementation & Evaluation",14,INK2,False,FONT)]],align=PP_ALIGN.CENTER)
badges(s,[("LLM",BLUE),("RAG",TEAL),("HYBRID",AMBER),("LoRA",VIO),("LoRA + RAG",ROSE)],4.55,5.35,5.0)
txt(s,1,6.55,SW-2,0.35,[[R("Shanaka Madhusanka  ·  "+STUDENT+"  ·  Supervisor: Dr. Dinuka R. Wijendra",11,MUTED,False,FONT)]],align=PP_ALIGN.CENTER)

# ============================================================ 2 RESEARCHER
s=slide(); chrome(s,"bg_content.png")
header(s,"Researcher",G['contact'],BLUE,"Researcher & supervisor")
txt(s,0.9,2.0,6.5,1.2,[[R("Kariyawasam Ketangodage",22,INK,True,FONT_L)],[R("Shanaka Madhusanka",22,INK,True,FONT_L)]],sa=0)
txt(s,0.9,3.15,6.5,0.36,[[R("Reg. No. ",13,INK2),R(STUDENT,13,INK,True,FONT)]])
bullets(s,["MSc in IT — Enterprise Application Development","Faculty of Computing, **SLIIT**","Module: **IT6010 — Research Project**"],
        0.9,3.65,6.3,color=BLUE,gap=9)
card(s,8.0,2.0,4.43,2.95,radius=0.08)
rect(s,8.0,2.0,4.43,0.10,fill=BLUE,rounded=True,radius=0.5)
icon(s,8.25,2.32,0.56,G['star'],BLUE,bg=tint(BLUE,0.85))
txt(s,9.0,2.32,3.2,0.56,[[R("SUPERVISOR",10,BLUE,True,MONO)]],anchor=MSO_ANCHOR.MIDDLE)
txt(s,8.25,3.0,3.95,0.4,[[R("Dr. Dinuka R. Wijendra",16,INK,True,FONT_SB)]])
bullets(s,["PhD, University of Colombo School of Computing","Senior Lecturer (Higher Grade)","Faculty of Computing | IT, SLIIT"],
        8.25,3.5,3.95,color=TEAL,size=11,gap=7)

# ============================================================ 3 DIV intro
divider("Part 01","Introduction & Motivation",G['view'])

# ============================================================ 4 MOTIVATION
s=slide(); chrome(s,"bg_content.png")
header(s,"Background",G['view'],BLUE,"Why this research")
blk(s,"The setting",0.9,2.0); bullets(s,["Enterprise backends run **complex business rules**, distributed services & heavy API traffic.","Agile / DevOps means **constant change** — suites fall behind."],0.9,2.34,6.3,color=BLUE)
blk(s,"The opportunity",0.9,3.5); bullets(s,["LLMs can read requirements & emit **structured tests**.","Potential to cut effort & surface hidden scenarios."],0.9,3.84,6.3,color=TEAL)
blk(s,"The catch",0.9,5.0); bullets(s,["Standalone LLMs lack **system context** → incomplete tests."],0.9,5.34,6.3,color=AMBER)
vflow(s,[(G['doc'],"BA / requirement documents","workflows · tables · rules",BLUE,True),
         (G['gear'],"Generative AI","comprehend & generate",TEAL,True),
         (G['done'],"Structured test cases","faster · broader coverage",ROSE,True)],8.0,2.25,4.43,nh=0.78,gap=0.34)

# ============================================================ 5 DIV problem
divider("Part 02","Research Problem",G['warning'])

# ============================================================ 6 PROBLEM
s=slide(); chrome(s,"bg_content.png")
header(s,"Problem Statement",G['warning'],AMBER,"Manual test design doesn't scale")
blk(s,"Slow & inconsistent interpretation",0.9,2.0)
bullets(s,["Decoding long requirement artefacts is **expertise-heavy**.","Leads to **inconsistency & test-incompleteness**."],0.9,2.34,6.3,color=BLUE)
blk(s,"LLMs not yet proven for enterprise",0.9,3.5)
bullets(s,["Most work uses **toy inputs** (user stories, snippets).","No **like-for-like comparison** of architectures."],0.9,3.84,6.3,color=AMBER)
card(s,0.9,5.05,6.3,1.2,fill=tint(BLUE,0.9),border=tint(BLUE,0.6))
rect(s,0.9,5.05,0.10,1.2,fill=BLUE,rounded=True,radius=0.3)
txt(s,1.2,5.05,5.85,1.2,[[R("How can complex enterprise requirement artefacts become reliable, structured, executable tests — and which AI architecture does it best?",12,INK,True,FONT)]],anchor=MSO_ANCHOR.MIDDLE,ls=1.08)
vflow(s,[("1","Requirement artefacts (complex)",None,TEAL,False),("2","Manual interpretation",None,AMBER,False),
         ("3","Inconsistent / incomplete tests",None,VIO,False),("4","Reduced confidence in quality",None,ROSE,False)],
      8.0,2.05,4.43,nh=0.66,gap=0.26)

# ============================================================ 7 DIV lit
divider("Part 03","Literature Review",G['library'])

# ============================================================ 8 LIT
s=slide(); chrome(s,"bg_content.png")
header(s,"Themes",G['library'],BLUE,"What the literature shows")
blk(s,"AI-augmented test generation",0.9,2.0); bullets(s,["LLMs reason over docs/APIs to produce structured tests beyond rule-/search-based methods."],0.9,2.34,6.3,color=BLUE)
blk(s,"Retrieval-Augmented Generation",0.9,3.25); bullets(s,["Grounds models in **up-to-date documentation** for relevance."],0.9,3.59,6.3,color=TEAL)
blk(s,"LoRA & Hybrid architectures",0.9,4.5); bullets(s,["Parameter-efficient **domain adaptation**.","Generation + **rule-based validation** for reliability."],0.9,4.84,6.3,color=VIO)
vflow(s,[(G['gear'],"AI-augmented test generation",None,BLUE,True),(G['refresh'],"Retrieval-Augmented Generation",None,TEAL,True),
         (G['code'],"LoRA fine-tuning",None,VIO,True),(G['check'],"Hybrid validation",None,AMBER,True)],8.0,2.1,4.43,nh=0.74,gap=0.3)

# ============================================================ 9 COMPARISON
s=slide(); chrome(s,"bg_content.png")
header(s,"Existing Systems Comparison",G['search'],BLUE,"How current tools fall short")
add_table(s,0.9,2.1,11.53,["Tool","Primary focus","Limitation"],
    [["Postman","API testing","No requirement understanding · no AI generation"],
     ["ReadyAPI","API automation","Rule-based, not AI-driven"],
     ["SonarQube","Code analysis","Quality only — no test generation"],
     ["GitHub Copilot","Code assistance","Assists code, does not execute tests"],
     ["Diffblue Cover","Unit-test generation","Java only · no business-requirement mapping"],
     ["Proposed Framework","End-to-end AI backend test generation","Requirement-driven, executed & evaluated"]],
    [2.4,3.6,5.53],best=5)

# ============================================================ 10 DIV gap
divider("Research Gap","Research Gap",G['target'])

# ============================================================ 11 GAP
s=slide(); chrome(s,"bg_content.png")
header(s,"What's missing",G['target'],AMBER,"The gap this study fills")
blk(s,"Gaps in existing research",0.9,2.05,color=AMBER)
bullets(s,["Little focus on **enterprise backend** systems.","Few handle **complex BA documents**.","Weak **contextual grounding** of models.","No **like-for-like architecture comparison**.","Coverage-only evaluation — **shallow**."],0.9,2.45,6.3,color=AMBER,gap=10)
card(s,7.9,2.05,4.53,3.55,radius=0.08)
rect(s,7.9,2.05,4.53,0.10,fill=TEAL,rounded=True,radius=0.5)
icon(s,8.2,2.35,0.56,G['check'],TEAL,bg=tint(TEAL,0.85))
txt(s,8.95,2.35,3.3,0.56,[[R("This study delivers",14,TEAL,True,FONT_SB)]],anchor=MSO_ANCHOR.MIDDLE)
bullets(s,["A modular framework ingesting real BA documents.","Five AI configurations under identical conditions.","Coverage + **mutation** + **execution validity**.","A working .NET prototype with CI/CD execution."],8.2,3.1,4.0,color=TEAL,gap=11)

# ============================================================ 12 DIV objectives
divider("Part 04","Objectives & Questions",G['target'])

# ============================================================ 13 OBJECTIVES
s=slide(); chrome(s,"bg_content.png")
header(s,"Objectives",G['flag'],BLUE,"Aim & supporting objectives")
blk(s,"Main objective",0.9,2.0)
txt(s,0.9,2.34,6.3,1.0,[[R("Design, build & evaluate a Generative AI-augmented framework that automatically produces structured test cases for enterprise backends from requirement artefacts.",12.5,INK2,False,FONT)]],ls=1.12)
blk(s,"Supporting objectives",0.9,3.7)
bullets(s,["Review AI-assisted testing & identify constraints.","Build a generative framework over enterprise artefacts.","Implement a prototype with multiple AI methods.","Evaluate relevance, coverage, accuracy & efficiency."],0.9,4.04,6.3,color=BLUE,gap=9)
vflow(s,[(G['search'],"Identify",None,BLUE,True),(G['gear'],"Design",None,TEAL,True),
         (G['check'],"Build & Validate",None,AMBER,True),(G['pie'],"Evaluate",None,VIO,True)],8.0,2.1,4.43,nh=0.82,gap=0.34)

# ============================================================ 14 RQs
s=slide(); chrome(s,"bg_content.png")
header(s,"Research Questions",G['info'],BLUE,"Five research questions")
icard_grid(s,[(G['link'],"RQ1","How can Generative AI integrate into backend testing & CI/CD pipelines?",BLUE),
              (G['check'],"RQ2","Do AI tests improve coverage & defect detection vs. traditional automation?",TEAL),
              (G['refresh'],"RQ3","Can it reduce manual effort & maintenance in evolving systems?",AMBER),
              (G['clipboard'],"RQ4","Can tests respect business rules & structured data formats?",VIO)],
           0.9,2.05,11.53,2.55,cols=2)
card(s,0.9,4.78,11.53,1.05,radius=0.08)
icon(s,1.1,4.98,0.62,G['world'],ROSE,bg=tint(ROSE,0.85))
txt(s,1.95,4.78,10.3,1.05,[[R("RQ5  ",13,ROSE,True,FONT_SB),R("— What best practices emerge for organisations adopting Generative AI backend testing?",12.5,INK2,False,FONT)]],anchor=MSO_ANCHOR.MIDDLE)

# ============================================================ 15 DIV methodology
divider("Part 05","Research Methodology",G['refresh'])

# ============================================================ 16 DSR
s=slide(); chrome(s,"bg_content.png")
header(s,"Design Science Research",G['refresh'],BLUE,"A controlled, design-led method")
pipeline(s,[("01","Problem","manual, slow design",BLUE,G['warning']),("02","Objectives","from literature",TEAL,G['flag']),
            ("03","Artefact","5-mode framework",AMBER,G['gear']),("04","Demonstrate",".NET prototype",VIO,G['code']),
            ("05","Evaluate","controlled experiment",ROSE,G['pie'])],0.9,2.15,11.53,sh=1.55)
icard_grid(s,[(G['gear'],"Controlled variables","Same BA document, target backend, templates & environment, and metrics.",BLUE),
              (G['pie'],"Independent vs. dependent","IV: AI architecture.  DV: coverage, validity, mutation, time, success.",TEAL)],
           0.9,4.0,11.53,1.6,cols=2)

# ============================================================ 17 DIV framework
divider("Part 06","Proposed Framework",G['gear'])

# ============================================================ 18 FIVE CONFIGS
s=slide(); chrome(s,"bg_content.png")
header(s,"Five AI Configurations",G['gear'],BLUE,"One pipeline, five configurations")
icard_grid(s,[(G['code'],"LLM — Baseline","Direct generation from requirements. No retrieval, no adaptation.",BLUE),
              (G['refresh'],"RAG","Retrieves relevant document segments as grounded context.",TEAL),
              (G['check'],"Hybrid","RAG + static code analysis + rule-based validation.",AMBER),
              (G['gear'],"LoRA","Parameter-efficient fine-tuning to the testing domain.",VIO)],0.9,2.05,11.53,2.55,cols=2)
card(s,0.9,4.78,11.53,1.05,radius=0.08)
icon(s,1.1,4.98,0.62,G['star'],ROSE,bg=tint(ROSE,0.85))
txt(s,1.95,4.78,10.3,1.05,[[R("LoRA + RAG  ",13,ROSE,True,FONT_SB),R("— Domain-adapted model and contextual retrieval — the strongest configuration.",12,INK2,False,FONT)]],anchor=MSO_ANCHOR.MIDDLE)

# ============================================================ 19 ALGORITHMS PER MODE
s=slide(); chrome(s,"bg_content.png")
header(s,"Algorithm per Configuration",G['code'],BLUE,"The technique behind each mode")
add_table(s,0.9,2.05,11.53,["Configuration","Core algorithm / technique","How it works"],
    [["LLM","Zero-/few-shot prompting","Decoder-only transformer maps requirements → test completion"],
     ["RAG","Retrieval-Augmented Generation","Embed chunks · cosine top-k (Qdrant) → context-augmented prompt"],
     ["Hybrid","RAG + static analysis + rules","Roslyn endpoint discovery + rule-based validation & repair"],
     ["LoRA","QLoRA / PEFT fine-tuning","Low-rank adapters on a code base model · completion-only SFT"],
     ["LoRA + RAG","Fine-tuned model + retrieval","Adapter inference grounded by retrieved documentation"]],
    [2.3,3.7,5.53],best=4,rh=0.66)
txt(s,0.9,6.2,11,0.35,[[R("Each mode adds one capability — context (RAG), validation (Hybrid), or adaptation (LoRA) — over the LLM baseline.",11,MUTED,False,FONT,True)]])

# ============================================================ 20 ARCHITECTURE
s=slide(); chrome(s,"bg_content.png")
header(s,"End-to-End Architecture",G['link'],BLUE,"From document to validated tests")
pipeline(s,[("1","BA Document","PDF upload",BLUE,G['doc']),("2","Ingest & Extract","endpoints · rules",TEAL,G['search']),
            ("3","AI Generation","5 pipelines",AMBER,G['gear']),("4","Structured Tests","de-dup · validation",VIO,G['done'])],
         0.9,2.2,11.53,sh=1.55)
pipeline(s,[("5","CI/CD Execute","xUnit run",ROSE,G['refresh']),(None,"Coverage",None,BLUE,G['pie']),
            (None,"Mutation",None,TEAL,G['check']),(None,"Execution Validity",None,AMBER,G['done'])],0.9,4.15,11.53,sh=1.5)

# ============================================================ 20 IMPLEMENTATION
s=slide(); chrome(s,"bg_content.png")
header(s,"System Design & Implementation",G['code'],BLUE,"Built as an embeddable .NET tool")
blk(s,"Technology stack",0.9,2.0)
badges(s,[(".NET / ASP.NET Core",BLUE),("xUnit",TEAL),("Coverlet",AMBER),("Stryker.NET",VIO),("OpenAI / Together",ROSE),
          ("Qdrant + SQLite",BLUE),("PEFT / LoRA",TEAL),("GitHub Actions",AMBER)],0.9,2.38,6.4)
blk(s,"Engineering highlights",0.9,3.65)
bullets(s,["**Swagger-style embeddable UI** — one NuGet package adds a /genai-tests page to any API.","**Clean architecture** — Api / Application / Domain / Infrastructure.","Real **endpoint discovery** via static analysis; parallel, budgeted generation."],0.9,4.05,6.4,color=BLUE,gap=9)
vflow(s,[(G['link'],"Api layer","endpoints · request binding",BLUE,True),(G['gear'],"Application","generation · run · mutation",TEAL,True),
         (G['target'],"Domain","metrics · modes · rules",VIO,True),(G['code'],"Infrastructure","engines · readers · scaffolding",AMBER,True)],
      8.0,2.05,4.43,nh=0.76,gap=0.28)

# ============================================================ 21 DIV results
divider("Part 07","Evaluation & Results",G['pie'])

# ============================================================ 22 EVAL DESIGN
s=slide(); chrome(s,"bg_content.png")
header(s,"Evaluation Design",G['clipboard'],BLUE,"Three measurement dimensions")
icard_grid(s,[(G['pie'],"Coverage","Line, branch & endpoint coverage of the API under test.",BLUE),
              (G['check'],"Mutation detection","Share of injected faults the suite kills (test strength).",TEAL),
              (G['done'],"Execution validity","Share of generated tests that compile & run correctly.",AMBER)],
           0.9,2.15,11.53,1.85,cols=3)
card(s,0.9,4.35,11.53,1.3,fill=SOFT,border=LINE,radius=0.06)
icon(s,1.15,4.62,0.6,G['info'],BLUE,bg=tint(BLUE,0.85))
txt(s,2.0,4.35,10.2,1.3,[[R("All five configurations run on the same backend & BA document, under identical conditions — so differences reflect the architecture, not the inputs.",13,INK,False,FONT)]],anchor=MSO_ANCHOR.MIDDLE,ls=1.1)

# ============================================================ 23 COVERAGE
s=slide(); chrome(s,"bg_content.png")
header(s,"Results · Table 6",G['pie'],BLUE,"Coverage comparison")
add_table(s,0.9,2.2,5.4,["Config","Line","Branch","Endpoint"],
    [["LLM","71%","63%","68%"],["RAG","79%","70%","74%"],["Hybrid","86%","82%","88%"],
     ["LoRA","81%","75%","80%"],["LoRA + RAG","88%","84%","91%"]],[1.95,1.15,1.15,1.15],best=4)
cd=CategoryChartData(); cd.categories=['LLM','RAG','Hybrid','LoRA','LoRA+RAG']
cd.add_series('Line',(71,79,86,81,88)); cd.add_series('Branch',(63,70,82,75,84)); cd.add_series('Endpoint',(68,74,88,80,91))
gf=s.shapes.add_chart(XL_CHART_TYPE.LINE_MARKERS,Inches(6.7),Inches(2.05),Inches(5.7),Inches(4.0),cd)
ch=gf.chart; clean_chart(ch,legend=True,vmin=55,vmax=95)
for i,sr in enumerate(ch.plots[0].series):
    sr.format.line.color.rgb=rgb([BLUE,AMBER,TEAL][i]); sr.format.line.width=Pt(2.5)
ch.value_axis.major_unit=10
txt(s,0.9,6.35,11,0.35,[[R("Coverage rises steadily LLM → LoRA+RAG; retrieval + adaptation compound.",11,MUTED,False,FONT,True)]])

# ============================================================ 24 MUTATION + VALIDITY
s=slide(); chrome(s,"bg_content.png")
header(s,"Results · Tables 7 & 8",G['check'],BLUE,"Mutation detection & execution validity")
txt(s,0.9,2.05,5.6,0.3,[[R("Mutation detection rate",11.5,MUTED,True,FONT_SB)]])
cd=CategoryChartData(); cd.categories=['LLM','RAG','Hybrid','LoRA','LoRA+RAG']; cd.add_series('Mutation',(58,68,77,72,80))
gf=s.shapes.add_chart(XL_CHART_TYPE.COLUMN_CLUSTERED,Inches(0.9),Inches(2.4),Inches(5.6),Inches(2.7),cd)
ch=gf.chart; clean_chart(ch,labels=True)
for i,pt in enumerate(ch.plots[0].series[0].points): pt.format.fill.solid(); pt.format.fill.fore_color.rgb=rgb(MODE_COLORS[i])
txt(s,6.85,2.05,5.6,0.3,[[R("Execution validity",11.5,MUTED,True,FONT_SB)]])
cd2=CategoryChartData(); cd2.categories=['LLM','RAG','Hybrid','LoRA','LoRA+RAG']; cd2.add_series('Validity',(85,91,96,93,97))
gf2=s.shapes.add_chart(XL_CHART_TYPE.COLUMN_CLUSTERED,Inches(6.85),Inches(2.4),Inches(5.6),Inches(2.7),cd2)
ch2=gf2.chart; clean_chart(ch2,labels=True)
for i,pt in enumerate(ch2.plots[0].series[0].points): pt.format.fill.solid(); pt.format.fill.fore_color.rgb=rgb(MODE_COLORS[i])
kpi(s,0.9,5.35,3.65,1.35,"+22%","Mutation: LLM → LoRA+RAG",BLUE,G['check'])
kpi(s,4.84,5.35,3.65,1.35,"97%","Best execution validity",TEAL,G['done'])
kpi(s,8.78,5.35,3.65,1.35,"5","Configs · identical conditions",AMBER,G['gear'])

# ============================================================ 25 FINDINGS
s=slide(); chrome(s,"bg_content.png")
header(s,"Key Findings",G['star'],BLUE,"What we learned")
icard_grid(s,[(G['refresh'],"Context wins","RAG grounds tests in real documentation → higher relevance & coverage than standalone LLM.",BLUE),
              (G['check'],"Hybrid = reliability","Rule-based validation + static analysis lift execution validity to 96%.",TEAL),
              (G['gear'],"Adaptation helps","LoRA improves domain understanding; with RAG it tops every metric.",VIO),
              (G['lightning'],"Scale & speed","AI explores far more input/boundary scenarios automatically than manual design.",AMBER)],
           0.9,2.1,11.53,3.5,cols=2)

# ============================================================ 26 DIV limitations
divider("Part 08","Limitations",G['warning'])

# ============================================================ 27 LIMITATIONS
s=slide(); chrome(s,"bg_content.png")
header(s,"Scope & Boundaries",G['warning'],AMBER,"Honest boundaries of the study")
icard_grid(s,[(G['target'],"01 · Research scope","Prototype scale vs. large distributed enterprises with hundreds of services.",BLUE),
              (G['doc'],"02 · Input dependency","Output quality bound by completeness & clarity of requirement artefacts.",TEAL),
              (G['refresh'],"03 · Model variability","Generative non-determinism; compute cost of LoRA & Hybrid configurations.",VIO),
              (G['code'],"04 · Testing scope","Backend API & integration testing — not UI, performance or security.",AMBER)],
           0.9,2.1,11.53,3.5,cols=2)

# ============================================================ LIMITATIONS PER MODE
s=slide(); chrome(s,"bg_content.png")
header(s,"Limitation per Configuration",G['warning'],AMBER,"Where each mode trades off")
add_table(s,0.9,2.05,11.53,["Configuration","Key limitation","Cost / complexity"],
    [["LLM","No system context · hallucinated endpoints · shallow tests","Low"],
     ["RAG","Sensitive to chunking & retrieval quality · added latency","Low–Medium"],
     ["Hybrid","More components → integration complexity · rule upkeep","Medium"],
     ["LoRA","Needs training data + GPU · overfitting / drift risk","High"],
     ["LoRA + RAG","Highest compute · hardest to reproduce & tune","High"]],
    [2.3,6.6,2.63],best=None,rh=0.66)
txt(s,0.9,6.2,11,0.35,[[R("Stronger results cost more: accuracy climbs with RAG → Hybrid → LoRA, and so does compute & complexity.",11,MUTED,False,FONT,True)]])

# ============================================================ FUTURE IMPROVEMENTS
s=slide(); chrome(s,"bg_content.png")
header(s,"Future Improvements",G['refresh'],BLUE,"Where this work goes next")
icard_grid(s,[(G['gear'],"Multi-agent generation","Generator + self-repair agents that auto-fix failing tests in a loop.",BLUE),
              (G['world'],"Enterprise-scale evaluation","Validate across large, multi-microservice distributed systems.",TEAL),
              (G['code'],"Domain-specific models","Continuous fine-tuning from CI feedback & organisation test history.",VIO),
              (G['link'],"Knowledge-graph retrieval","Semantic graphs for richer, cross-document grounding.",AMBER),
              (G['check'],"Broader test types","Extend beyond API to UI, performance & security testing.",ROSE),
              (G['lightning'],"Cost & latency","Optimise inference; on-prem private models for sensitive data.",BLUE)],
           0.9,2.1,11.53,3.5,cols=3)

# ============================================================ 28 DIV conclusion
divider("Part 09","Conclusion & Contributions",G['done'])

# ============================================================ 29 CONCLUSION
s=slide(); chrome(s,"bg_content.png")
header(s,"Conclusion",G['done'],BLUE,"Retrieval + adaptation deliver the gains")
txt(s,0.9,2.0,6.4,1.0,[[R("Generative AI — especially retrieval + domain adaptation — measurably improves automated test generation for enterprise backends: higher coverage, stronger defect detection & more valid tests.",12.5,INK2,False,FONT)]],ls=1.12)
blk(s,"Research contributions",0.9,3.05)
yy=3.42
for (gl,ti,bo,co) in [(G['view'],"Conceptual","modular AI-augmented testing framework",BLUE),
                      (G['search'],"Methodological","first like-for-like comparison of 5 AI configs",TEAL),
                      (G['pie'],"Empirical","coverage + mutation + execution validity evidence",VIO),
                      (G['gear'],"Practical","embeddable .NET prototype with CI/CD",AMBER)]:
    card(s,0.9,yy,6.4,0.6,radius=0.16)
    icon(s,1.04,yy+0.09,0.42,gl,co,bg=tint(co,0.85))
    txt(s,1.62,yy,5.6,0.6,[[R(ti+" — ",11.5,co,True,FONT_SB),R(bo,10.5,INK2,False,FONT)]],anchor=MSO_ANCHOR.MIDDLE)
    yy+=0.72
txt(s,8.0,2.0,4.43,0.3,[[R("Best configuration — LoRA + RAG",11.5,MUTED,True,FONT_SB)]])
add_table(s,8.0,2.4,4.43,["Metric","Score"],
    [["Line coverage","88%"],["Branch coverage","84%"],["Endpoint coverage","91%"],["Mutation detection","80%"],["Execution validity","97%"]],
    [3.0,1.43],best=4)

# ============================================================ 30 THANK YOU
s=slide(); chrome(s,"bg_closing.png",page=False,trans='<p:fade/>')
icon(s,SW/2-0.45,1.75,0.9,G['contact'],WHITE,bg=BLUE,white=True)
txt(s,1,2.85,SW-2,0.4,[[R("THANK YOU",12,BLUE,True,MONO)]],align=PP_ALIGN.CENTER)
txt(s,1,3.25,SW-2,0.9,[[R("Questions & Discussion",36,INK,True,FONT_L)]],align=PP_ALIGN.CENTER)
rect(s,SW/2-1.7,4.45,1.7,0.07,fill=BLUE,rounded=True,radius=0.5)
rect(s,SW/2,4.45,1.7,0.07,fill=TEAL,rounded=True,radius=0.5)
txt(s,1,4.7,SW-2,0.8,[[R("Shanaka Madhusanka  ·  "+STUDENT,13,INK2,False,FONT)],
    [R("Supervisor: Dr. Dinuka R. Wijendra  ·  SLIIT  ·  MSc in IT (EAD)  ·  May 2026",11.5,MUTED,False,FONT)]],
    align=PP_ALIGN.CENTER,sa=4)
badges(s,[("LLM",BLUE),("RAG",TEAL),("HYBRID",AMBER),("LoRA",VIO),("LoRA + RAG",ROSE)],4.55,5.75,5.0)

# ---------- save ----------
LIM=int(os.environ.get('FT_LIMIT','0'))
if LIM>0:
    ids=prs.slides._sldIdLst
    for sid in list(ids)[LIM:]: ids.remove(sid)
try:
    prs.save(OUT)
except PermissionError:
    OUT="GenAI_Testing_Framework_Defence_v2.pptx"; prs.save(OUT)
print("Saved", OUT, "·", len(prs.slides._sldIdLst), "slides")
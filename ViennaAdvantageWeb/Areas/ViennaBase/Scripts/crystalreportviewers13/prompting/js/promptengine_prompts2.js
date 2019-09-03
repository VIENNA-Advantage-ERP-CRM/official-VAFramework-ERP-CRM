/* Copyright (c) Business Objects 2006. All rights reserved. */
var PE_VALUE_DESC_SEPARATOR = ' - ';

if(typeof(_pe) == 'undefined') {

_pe = new function()
{
var o=this
o._ie=(document.all!=null)?true:false
o._dom=(document.getElementById!=null)?true:false
o._isQuirksMode = (document.compatMode != 'CSS1Compat');
o._moz=o._dom&&!o._ie
o._appVer=navigator.appVersion.toLowerCase();
o._mac=(o._appVer.indexOf('macintosh')>=0)||(o._appVer.indexOf('macos')>=0);
o._userAgent=navigator.userAgent?navigator.userAgent.toLowerCase():null
o._saf=o._moz&&(o._userAgent.indexOf("safari")>=0)
o._ie6=o._ie&&(o._appVer.indexOf("msie 6")>=0)

o._root = ''
o._images= o._root + '/images/'

// prompt section
o._prompts=new Array
o._lovBS=1000

o._st='s'
o._nm='n'
o._cy='c'
o._bo='b'
o._da='d'
o._tm='t'
o._dt='dt'

_BlockWaitWidgetID = "PEBlockWidgetID"

// dialog secion
o._theLYR=null
o._dlgResize=null
o._widgets=new Array
o.DlgBox_modals=new Array;
o.DlgBox_instances=new Array
o.DlgBox_current=null

o._show='visible'
o._hide='hidden'
o._hand=o._ie?"hand":"pointer"

// functions
o.init=PE_init
o.canSubmit=PE_canSubmit
o.beginBlocking=PE_beginBlocking
o.endBlocking=PE_endBlocking

// commands
o.setLOVMsg=PE_setLOVMsg
}
}

function PE_init(root,lovBS)
{
var o=this
if (root && root.length > 0)
{
if (root.charAt(root.length - 1)!='/') root += '/'
o._root = root
o._images = root + 'images/'
}
else
{
o._root = null
o._images = null
}
if(lovBS>0) o._lovBS=lovBS
}

function PE_canSubmit()
{
return (this.DlgBox_current) ? false : true
}

/*
Commands:
cmd=lov  (get values from the active prompt)
ap(active prompt id)
*/
function PE_setLOVMsg(form,vid,pid)
{
var fl=document.getElementById(vid);
var fv=fl.value;

if (fv.length > 0) fv += "&";
fv += "cmd=1&ap" + "=" + pid;

fl.value=fv
}

var DateTimeFormatSetting = {
"datePattern":"Y-M-D",
"isTwoDigitMonth":true,
"isTwoDigitDay":true,
"dateRegex":null,
"dateTimeRegex":null
};

///////////////////////////////
// functions for DateTimeFormatSetting
function promptengine_getDatePattern()
{
    return DateTimeFormatSetting.datePattern;
}

function promptengine_setDatePattern(pattern)
{
    DateTimeFormatSetting.datePattern = pattern;
}

function promptengine_getIsTwoDigitMonth()
{
    return DateTimeFormatSetting.isTwoDigitMonth;
}

function promptengine_setIsTwoDigitMonth(isTwoDigitMonth)
{
    DateTimeFormatSetting.isTwoDigitMonth = isTwoDigitMonth;
}

function promptengine_getIsTwoDigitDay()
{
    return DateTimeFormatSetting.isTwoDigitDay;
}

function promptengine_setIsTwoDigitDay(isTwoDigitDay)
{
    DateTimeFormatSetting.isTwoDigitDay = isTwoDigitDay;
}

function promptengine_getDateRegex()
{
    return DateTimeFormatSetting.dateRegex;
}

function promptengine_setDateRegex(dateRegex)
{
    DateTimeFormatSetting.dateRegex = dateRegex;
}

function promptengine_getDateTimeRegex()
{
    return DateTimeFormatSetting.dateTineRegex;
}

function promptengine_setDateTimeRegex(dateTineRegex)
{
    DateTimeFormatSetting.dateTineRegex = dateTineRegex;
}

////////////////////////////////////////////
// helper functions duplicate in dhtml lib
function _convStr(s,nbsp,br)
{
s=''+s
var ret=s.replace(/&/g,"&amp;").replace(/</g,"&lt;").replace(/>/g,"&gt;").replace(/"/g,"&quot;")
if (nbsp)
ret=ret.replace(/ /g,"&nbsp;")
if (br)
ret=ret.replace(/\n/g,"<br>")
return ret
}

function _opt(val,txt,sel)
{
return '<option value="'+_convStr(val)+'" '+(sel?'selected':'')+'>'+_convStr(txt)+'</option>'
}

function _canScanFrames(w)
{
var ex=true,d=null
if (_pe._moz)
{
_oldErrHandler=window.onerror
window.onerror=localErrHandler
}
try
{
d=w.document
ex=false
}
catch(expt)
{
}
if (_pe._moz)
window.onerror=_oldErrHandler
return (!ex&&(d!=null))
}

function _restoreAllDisabledInputs(win,level)
{
if (_pe._ie&&window._peInputStackLevel!=null)
{
win=win?win:window
if (_canScanFrames(win))
{
if (level==null)
level=--window._peInputStackLevel
var b=win.document.body,arr=b?b.getElementsByTagName("SELECT"):null,len=arr?arr.length:0
for (var i=0;i<len;i++)
{
var inpt=arr[i]
if (inpt._peDisableLevel==level)
{
inpt.disabled=false
inpt._peDisableLevel=null
}
}
var frames=win.frames,flen=frames.length
for (var k=0;k<flen;k++)
_restoreAllDisabledInputs(frames[k],level)
}
}
}
function _disableAllInputs(x,y,w,h,win,level)
{
if (_pe._ie)
{
win=win?win:window
if (_canScanFrames(win))
{
var b=win.document.body,arr=b?b.getElementsByTagName("SELECT"):null,len=arr?arr.length:0
if (level==null)
{
if (window._peInputStackLevel==null)
window._peInputStackLevel=0
level=window._peInputStackLevel++
}
for (var i=0;i<len;i++)
{
var inpt=arr[i]
var inter=(x==null)||_isLayerIntersectRect(inpt,x,y,w,h)
if (!inpt.disabled&&inter)
{
inpt._peDisableLevel=level
inpt.disabled=true
}
}
var frames=win.frames,flen=frames.length
for (var k=0;k<flen;k++)
_disableAllInputs(null,null,null,null,frames[k],level)
}
}
}

function _getBGIframe(id)
{
return '<iframe id="'+id+'" style="display:none;left:0px;position:absolute;top:0px" src="' + _pe._images + 'transp.gif' + '" frameBorder="0" scrolling="no"></iframe>'
}

function _eventCancelBubble(e,win)
{
win=win?win:window
_pe._ie?win.event.cancelBubble=true:e.cancelBubble=true
}

function _append(e,s)
{
if (_pe._ie)
e.insertAdjacentHTML("BeforeEnd",s)
else
{
var curDoc = document
var r=curDoc.createRange()
r.setStartBefore(e)
var frag=r.createContextualFragment(s)
e.appendChild(frag)
}
}

function _targetApp(s)
{
_append(document.body,s)
}

function _isLayerIntersectRect(l,x1,y1,w,h)
{
var xl1=_getPos(l).x,yl1=_getPos(l).y,xl2=xl1+l.offsetWidth,yl2=yl1+l.offsetHeight,x2=x1+w,y2=y1+h
return ((x1>xl1)||(x2>xl1))&&((x1<xl2)||(x2<xl2)) && ((y1>yl1)||(y2>yl1))&&((y1<yl2)||(y2<yl2))
}

function _getPos(el,relTo)
{
relTo = relTo?relTo:null
for (var lx=0,ly=0;(el!=null)&&(el!=relTo);
lx+=el.offsetLeft,ly+=el.offsetTop,el=el.offsetParent);
return {x:lx,y:ly}
}

function _getLayer(id)
{
return document.getElementById(id)
}

function _getWidget(layer)
{
if (layer==null)
return null
var w=layer._widget
if (w!=null)
return _pe._widgets[w]
else
return _getWidget(layer.parentNode)
}

function _isHidden(lyr)
{
if ((lyr==null)||(lyr.tagName=="BODY")) return false;var sty=lyr.style;if ((sty==null)||(sty.visibility==_pe._hide)||(sty.display=='none')) return true;return _isHidden(lyr.parentNode)
}

function _attr(key,val)
{
return (val!=null?' '+key+'="'+val+'" ':'')
}

function _img(src,w,h,align,att,alt)
{
att=(att?att:'')
if (alt==null) alt=''
return '<img'+_attr('width',w)+_attr('height',h)+_attr('src', src)+_attr(_pe._ie?'alt':'title',alt)+_attr("align", align)+' border="0" hspace="0" vspace="0" '+(att?att:'')+'>'
}

function _imgOffset(url,w,h,dx,dy,id,att,alt,st,align)
{
return _img(_pe._images+'transp.gif',w,h,align,
(att?att:'') +' '+_attr('id',id)+' style="'+_backImgOffset(url,dx,dy)+(st?st:'')+'"',
alt)
}

function _changeOffset(lyr,dx,dy,url,alt)
{
var st=lyr.style
if (st)
{
if ((dx!=null)&&(dy!=null))
st.backgroundPosition=''+(-dx)+'px '+(-dy)+'px'
if (url)
st.backgroundImage='url(\''+url+'\')'
}
if(alt) lyr.alt=alt
}

function _simpleImgOffset(url,w,h,dx,dy,id,att,alt,st,align)
{
if (_pe._ie)
{
if (dx==null) dx=0
if (dy==null) dy=0
return '<div '+(att?att:'')+' '+_attr("id",id)+' style="position:relative;padding:0px;width:'+w+'px;height:'+h+'px;overflow:hidden;'+(st?st:'')+'">'+_img(url,null,null,'top','style="margin:0px;position:relative;top:'+(-dy)+'px;left:'+(-dx)+'px"',alt)+'</div>'
}
else
return _imgOffset(url,w,h,dx,dy,id,att,alt,st,align)
}

function _changeSimpleOffset(lyr,dx,dy,url,alt)
{
if (_pe._ie)
{
lyr=lyr.childNodes[0]
var st=lyr.style
if ((url!=null)&&(url!=lyr.src))
lyr.src=url
if (dx!=null)
st.left=""+(-dx)+"px"
if (dy!=null)
st.top=""+(-dy)+"px"
if (alt!=null)
lyr.alt=alt
}
else
_changeOffset(lyr,dx,dy,url,alt)
}

function _backImgOffset(url,dx,dy)
{
return 'background-image:url(\''+url+'\');background-position:'+(-dx)+'px '+(-dy)+'px;'
}

function _sty(key,val)
{
return (val!=null?key+':'+val+';' :'')
}

function _getSpace(w,h)
{
return '<table height="'+h+'" border="0" cellspacing="0" cellpadding="0"><tr><td>'+_img(_pe._images+'transp.gif',w,h)+'</td></tr></table>'
}

function _isTextInput(ev)
{
var source = _pe._ie?ev.srcElement:ev.target;
var isText=false;
if(source.tagName=="TEXTAREA")
isText=true
if((source.tagName=="INPUT") && (source.type.toLowerCase()=="text"))
isText=true
return isText;
}
function _documentWidth(win)
// Gets the document(page) width
// return [int]
{
    var win=win?win:window;  
    var width = Math.max(document.body.clientWidth,document.documentElement.clientWidth);
    width = Math.max(width,document.body.scrollWidth);
    return width;
}

function _documentHeight(win)
// Gets the document(page) width
// return [int]
{
    
    var win=win?win:window;  
    var height = Math.max(document.body.clientHeight,document.documentElement.clientHeight);
    height = Math.max(height,document.body.scrollHeight);

    return height;
}

function _winWidth(win)
{
    var win=win?win:window
    var width;  
    if(_pe._ie) 
    {
        if(_pe._isQuirksMode) 
        {
            width = win.document.body.clientWidth;
        }
        else
        {
            width = win.document.documentElement.clientWidth;
        }
    }
    else
    {
        width = win.innerWidth;
    }
    return width;

}
function _winHeight(win)
{
    var win=win?win:window;  
    var height;
    if(_pe._ie) 
    {
        if(_pe._isQuirksMode) 
        {
            height = document.body.clientHeight;
        }
        else
        {
            height = document.documentElement.clientHeight;
        }
    }
    else
    {
        height = win.innerHeight;
    }
    return height;

}

function _getScrollX(win)
{
    var scrollLeft = 0;
    var win=win?win:window;  
     
    if(typeof(win.scrollX ) == 'number') {
        scrollLeft = win.scrollX;
    }
    else {
        scrollLeft = Math.max(win.document.body.scrollLeft,win.document.documentElement.scrollLeft);
    }
    
    return scrollLeft;

}
function _getScrollY(win)
{
    var scrollTop = 0;
    var win=win?win:window;  
     
    if(typeof(win.scrollY ) == 'number') {
        scrollTop = window.scrollY;
    }
    else {
        scrollTop = Math.max(win.document.body.scrollTop,win.document.documentElement.scrollTop);
    }

    return scrollTop;
}

function _eventGetX(e)
{
return _pe._ie?window.event.clientX: e.clientX ? e.clientX : e.pageX;
}
function _eventGetY(e)
{
return _pe._ie?window.event.clientY: e.clientY ? e.clientY : e.pageY;
}

function _eventGetKey(e,win)
{
win=win?win:window
return _pe._ie?win.event.keyCode:e.keyCode
}

function _isLayerDisplayed(lyr)
{
var css=lyr?lyr.style:null
if(css)
{
if(css.display == "none" || css.visibility=="hidden")
return false
else
{
var par=lyr.parentNode
if(par!=null)
return _isLayerDisplayed(par)
else
return true
}
}
else
return true;
}

function _safeSetFocus(lyr)
{
if (lyr && lyr.focus && _isLayerDisplayed(lyr) && !lyr.disabled)
lyr.focus()
}

/////////////////////////
// Select Object Helper
function PE_getLB(lyr)
{
var o=new Object
o.lyr=lyr
o.arr=new Array
o.size=0

o.add=LB_add
o.update=LB_update
return o
}

function LB_add(val,txt,sel)
{
var o=this
o.arr[++o.size]=_opt(val,txt,sel)
}

function LB_update()
{
var o=this
var a=o.arr

if (!o.lyr) return
var parent=o.lyr.parentNode

var oldHTML=parent.innerHTML
var bpos=oldHTML.indexOf('>')
if (bpos == -1) return
var epos=oldHTML.lastIndexOf('<')
if (epos <= bpos) return

a[0]=oldHTML.substring(0,bpos+1)
a[o.size+1]=oldHTML.substr(epos)
parent.innerHTML=a.join('')
}

//////////////////////////
// Prompting Unit
function newUnits(num,idh)
{
var o=new Object

o.parr=new Array

if(idh) o.idh=idh
else o.idh=''
o.num=num

o.init=Units_init
o.show=Units_show
o.toggle=Units_toggle
o.updateForm=Units_updateForm
o.activate=Units_activate

o.addP=Units_addP

return o
}

function Units_init(uid)
{
var o=this
o.toggle(uid)
}

function Units_show(uid,b)
{
var o=this
var l=document.getElementById(o.idh+uid+'_PU')
if (l)
{
var css=l.style
if(b)
css.display=''
else 
css.display='none'
}
}

function Units_activate(uid)
{
var o=this
var l=document.getElementById(o.idh+uid+'_PU')
if (l)
{
var scrY=_getScrollY(),scrX=_getScrollX()
var h=l.offsetHeight, winCY=_winHeight(),y=_getPos(l).y

if (y<scrY) window.scrollTo(scrX, y)
else if (y+h>scrY+winCY) window.scrollTo(scrX, Math.max(y,y+h-winCY))
}
}

function Units_toggle(uid)
{
var o=this,c=o.num
for(var i=0;i<c;i++) 
o.show(i,true)
o.activate(uid)
}
/*
function Units_toggle(uid)
{
var o=this,c=o.num
for(var i=0;i<c;i++) 
o.show(i,((i==uid)?true:false))
}
*/

function Units_addP(p)
{
var o=this
var parr=o.parr
parr[parr.length]=p
}

function Units_updateForm(form,vid,chk)
{
var o=this,parr=o.parr
for(var i in parr)
{
var p=parr[i]
if(p)
{
if (!p.updateForm(form,vid,chk))
{
o.toggle(p.uid)
return false
}
}
}
return true
}

///////////////////////////////////////////////////
// Prompt Object
function P_updateForm(form,vid,chk)
{
var o=this, b=false

if(o.readonly==true) return true

if(o.mul) b=promptengine_updateMultiValue(form, vid, o.id, o.vt, chk, o.valueRequired)
else if(o.rn) b=promptengine_updateRangeValue(form, vid, o.id, o.vt, chk, o.valueRequired)
else b=promptengine_updateDiscreteValue(form, vid, o.id, o.vt, chk, o.valueRequired)
return b
}

function P_addV(v,d)
{
var o=this

if(o.vl==null)
{
o.vl=new Array
if (d)
o.dl=new Array
}

var len=o.vl.length
o.vl[len]=v
if(o.dl)
o.dl[len]=d
}

function P_findBatch(wty,tv)
{
if(!tv) return(-1)

var o=this
var vl=o.vl
if(wty)
{
var lov=o.lov[wty]
if(lov && lov.vl) vl=lov.vl
}

if(vl)
{
for(var i in vl)
{
var v=vl[i]
if(v && v==tv) return(Math.floor(i/_pe._lovBS))
}
}
return(-1)
}

function P_updateLOVNB(wty,b)
{
// b - if need to refresh batch list
var o=this
var pid=o.id

var nav=document.getElementById(pid+wty+'Batch')
if(!nav) return

var lov=o.lov[wty]
if(b)
{
var opts=nav.options
opts.length=0

var vl=o.vl, i=0
if(lov.vl) vl=lov.vl
var len=Math.ceil(vl.length/_pe._lovBS)

while(i<len)
{
var d=i+1
if(lov.sidx==i) d +='*'
opts[opts.length]=new Option(d,i,false,false)
i++
}
}
if (lov.bidx>=0) nav.selectedIndex=lov.bidx
}

function P_updateLOV(wty,bi)
{
var o=this
var pid=o.id
var wid=pid+wty

var l=document.getElementById(wid)
if (!l) return

var lov=o.lov[wty]
var sel=null

var vl=o.vl
var dl=o.dl
var si=-1
if(lov)
{
sel=lov.sel
if (lov.vl)
{
vl=lov.vl
dl=lov.dl
}
}

var lbCtl=PE_getLB(l)

if(bi) lbCtl.add('','...')

var sidx=lov.sidx

if (lov.bidx<0) {
lov.sidx=o.findBatch(wty,sel)
if(lov.sidx>=0) lov.bidx=lov.sidx
else lov.bidx=0
sidx=-2
}

var bidx=lov.bidx

var i=bidx*_pe._lovBS, len=vl.length, j=0
while (i<len)
{
if(j >= _pe._lovBS) break;

var v=vl[i]
var d=null
if (dl)
{
d=dl[i]
if(d=='') d=v
else if(o.dop==0) d=v+PE_VALUE_DESC_SEPARATOR+d
} 
else d=v

if (sel && sel==v)
{
si=j
o.sidx=bidx
}

lbCtl.add(v,d)
i++
j++
}

lbCtl.update()

l = document.getElementById(wid)
if(si!=-1)
{
if(bi) si++
l.selectedIndex=si
}

o.updateLOVNB(wty, sidx!=lov.sidx)
}

function P_getDesc(v)
{
if(!v) return null

var o=this
var vl=o.vl
var dl=o.dl

if(!dl) return null

// stop searching description when the list is too long.
// if (vl.length>1000) return null

var j=-1
for(var i in vl) if(vl[i]==v) {j=i; break;}

if(j>=0) return dl[j]
return null
}

function P_updateSLOV()
{
// NOTE:
// This function can only be called in the beginning.
// Selection object will not updated with user actions.

var o=this
var pid=o.id

var lyr=document.getElementById(pid+'ListBox')
if(!lyr) return

var sel=o.sl

if (typeof(sel)!='object') return

var lbCtl=PE_getLB(lyr)

var vl=o.vl
var dl=o.dl

for (var i in sel)
{
var v=sel[i]
if (typeof(v)=='string')
{
var d=o.getDesc(v)
var txt
if(d && d!='')
{
if(o.dop) txt=d
else txt=v+PE_VALUE_DESC_SEPARATOR+d
}
else txt=v

lbCtl.add(v,txt,false)
}
else
{
var lo=v.l
var up=v.u
var lt=v.lt
var ut=v.ut

var val=null
var txt=null
if (lt==0 || lt==1)
{
val='('
txt='('
}
else
{
val='['
txt='['
}

if (lt)
{
val+=lo

var d=o.getDesc(lo)

if(d && d!='')
{
if(o.dop) txt+=d
else txt+=lo+PE_VALUE_DESC_SEPARATOR+d
}
else txt+=lo
}

val+='_crRANGE_'
txt+='  ..  '

if (ut)
{
val+=up

var d=o.getDesc(up)
if(d && d!='')
{
if(o.dop) txt+=d
else txt+=up+PE_VALUE_DESC_SEPARATOR+d
}
else txt+=up
}

if (ut==0 || ut==1)
{
val+=')'
txt+=')'
}
else
{
val+=']'
txt+=']'
}

lbCtl.add(val,txt,false)
}
}

lbCtl.update()
}

function P_update(wty)
{
var o=this

if (wty)
{
if(wty=='AvailableList') o.updateLOV(wty)
else if(wty=='ListBox') o.updateSLOV() 
else o.updateLOV(wty,true)
}
else
{
o.updateLOV('SelectValue',true)
o.updateLOV('AvailableList')
o.updateLOV('SelectLowerRangeValue',true)
o.updateLOV('SelectUpperRangeValue',true)
o.updateSLOV()
}
}

// vl - value list
// dl - description list
function P_setLOV(vl,dl)
{
var o=this
o.vl=vl

if (vl)
{
if(!dl || vl.length!=dl.length)
o.dl=null
else
o.dl=dl
}
else
o.dl=null
}

function P_setInitSel(sel)
{
var o=this
var lov=o.lov['SelectValue']
lov.sel=sel
}

function P_setInitSelList(sl)
{
this.sl=sl
}

function P_setInitBound(lo,up)
{
var o=this
var loB=o.lov['SelectLowerRangeValue']
loB.sel=lo
var upB=o.lov['SelectUpperRangeValue']
upB.sel=up
}

function P_back(wty)
{
var o=this
var lov=o.lov[wty]
if(!lov) return

var bidx=lov.bidx

if (bidx > 0)
{
bidx--
if (bidx<0) bidx=0
lov.bidx=bidx
o.update(wty)
}
}

function P_next(wty)
{
var o=this
var lov=o.lov[wty]
if(!lov) return

var len=((lov.vl) ? lov.vl.length : o.vl.length)
var bidx=lov.bidx

if ((bidx+1)*_pe._lovBS < len)
{
lov.bidx=bidx+1
o.update(wty)
}
}

function newLOV()
{
var o=new Object
o.bidx=-1
o.sidx=-1
o.sel=null
o.filter=null
o.vl=null
o.dl=null
return o
}

function newP(units,uid,id,vt,mul,di,rn,dop,readonly,valueRequired)
{
var o=new Object
o.id=id
o.vt=vt
o.mul=mul
o.di=di
o.rn=rn
o.dop=dop
o.readonly=readonly
o.valueRequired=valueRequired
o.units=units
o.uid=uid

o.lov=new Array
o.lov['SelectValue']=newLOV()
o.lov['AvailableList']=newLOV()
o.lov['SelectLowerRangeValue']=newLOV()
o.lov['SelectUpperRangeValue']=newLOV()

o.vl=null
o.dl=null
o.sl=null

o.addV=P_addV
o.update=P_update
o.setLOV=P_setLOV
o.updateLOV=P_updateLOV
o.updateSLOV=P_updateSLOV
o.updateLOVNB=P_updateLOVNB
o.getDesc=P_getDesc

o.findBatch=P_findBatch
o.back=P_back
o.next=P_next
o.showFilter=P_showFilter
o.applyFilter=P_applyFilter

o.setInitSel=P_setInitSel
o.setInitBound=P_setInitBound
o.setInitSelList=P_setInitSelList

o.updateForm=P_updateForm

_pe._prompts[id]=o
if (units) units.addP(o)
return o
}

function P_navigateCB(fid,pid,wty,cmd)
{
var o=_pe._prompts[pid]
if (!o) return
if(cmd=='p') o.back(wty)
else if(cmd=='n') o.next(wty)
}

function P_selectCB(form,pid,wty,dty)
{
var o=_pe._prompts[pid]
if (!o) return

var did=pid+dty
var wid=pid+wty

promptengine_selectValue(form, wid, did)

var lov=o.lov[wty]

lov.sel=document.getElementById(did).value
if(!lov.sel || lov.sel=='')
{
lov.sidx=-1
lov.sel=null
}
else if (lov.sidx!=lov.bidx) lov.sidx=lov.bidx

o.updateLOVNB(wty,true)
}

function P_batchCB(fid,pid,wty)
{
var o=_pe._prompts[pid]
var el=document.getElementById(pid+wty+'Batch')
if (!o || !el) return

var i=el.selectedIndex
if (i>=0) 
{
var lov=o.lov[wty]
if (!lov) return
lov.bidx=i
o.update(wty)
}
}

function P_applyFilter(wty,filter)
{
    if (filter==null) {return;}
    
    var o=this;
    
    var vl=o.vl;
    var dl=o.dl;
    if (!vl || vl.constructor != Array || vl.length==0) {return;}
    var dlExists = true;
    if (!dl || dl.constructor != Array) {
        dlExists = false;
    }
    
    var lov=o.lov[wty];
    if (!lov) {return;}
    
    var oldfilter=lov.filter;
    if (!oldfilter) oldfilter='';
    
    if(filter==oldfilter) {return;}
    
    var wvl=null;
    var wdl=null;
    if(filter=='') {
        filter=null;
    }
    else {
        wvl=[];
        if (dlExists) {
            wdl=[];
        }
        
        // Replace no-break spaces used by thousand separators with regular spaces
        filter = filter.replace(String.fromCharCode(0xA0), ' ');
        
        var j = 0;
        // we always loop through vl because this is always guranteed to exist unlike dl
        for (var i = 0, len = vl.length; i < len; i++) {
            var value = vl[i];
            var desc = (dlExists ? dl[i] : '');
            
            var stringToSearch = '';
            
            if (o.dop == 1) { //if description only prompt
                if (desc == '') {
                    stringToSearch = value;
                }
                else {
                    stringToSearch = desc;
                }
            }
            else {
                stringToSearch = value;
                if (desc != '') {
                    stringToSearch += PE_VALUE_DESC_SEPARATOR;
                    stringToSearch += desc;
                }
            }
            
            // Replace no-break spaces used by thousand separators with regular spaces
            stringToSearch = stringToSearch.replace(String.fromCharCode(0xA0), ' ');
            
            if (stringToSearch && stringToSearch.toLowerCase().indexOf(filter.toLowerCase()) != -1) {
                wvl[j] = value;
                if(dlExists) {
                    wdl[j] = dl[i];
                }
                j++;
            }
        }
    }
    
    // update LOV
    lov.filter=filter
    lov.vl=wvl
    lov.dl=wdl
    lov.bidx=-1
    lov.sidx=-1
    
    o.updateLOV(wty,true)
}

function P_promptFilter(pid,wty, e)
{
    var o=_pe._prompts[pid]
    if (!o) return
    
    var vl=o.vl
    var dl=o.dl
    if (!vl || vl.length==0) return
    
    var lov=o.lov[wty]
    if (!lov) return
    
    var filter=lov.filter
    if (!filter) filter=''
    
    var filterIcon = e.target ? e.target : e.srcElement;
    var pos = _findPos(filterIcon)
    // Placing filter dialog below the filter icon
    var x = pos.x + filterIcon.offsetWidth;
    var y = pos.y + filterIcon.offsetHeight;
    
    o.showFilter(wty,filter,x,y)
}

function P_promptClearFilter(pid,wty, e)
{
    var o=_pe._prompts[pid];
    if (!o) return;
    
    if(o.filterDlg)
    {
        o.filterDlg.setValue('');
        // if there's no filter dialog, we don't need to call o.applyFilter since no filter has been applied yet
        o.applyFilter(wty, '');
    }
}

function P_showFilter(wty,v,x,y)
{
var o=this
if (!o.filterDlg) o.filterDlg = newFilterDlg(o.id)

var dlg = o.filterDlg
dlg.wty = wty
dlg.setValue(v)
dlg.show(true)
dlg.initDlg(x,y)
}

function _findPos(el,relTo) {
    var relTo = relTo?relTo:null
    var posX = 0;
    var posY = 0;
    
    while(el.parentNode || el.offsetParent) {
        if(el.offsetParent) {
            posX +=el.offsetLeft;
            posY += el.offsetTop;
            el = el.offsetParent;
        }
        else if(el.parentNode) {
            if(el.style) {
                if(el.style.left) {
                    posX += el.style.left;
                }
                if(el.style.top) {
                    posY += el.style.top;
                }
            }
            el = el.parentNode;   
        }
        else {
            break;
        }
    }
    
    if(relTo) {
        relToCord = getPos2(relTo);
        posX -= relToCord.x;
        posY -= relToCord.y;
    }
    return {x:posX,y:posY};
}

// Filter Dialog
function FilterDlg_okCB(dlgID)
{
var dlg=this
if(dlgID) dlg=_getWidget(_getLayer(dlgID))

if(dlg)
{
var o=_pe._prompts[dlg.promptid]
var filter=dlg.getValue()
dlg.show(false)
o.applyFilter(dlg.wty,filter)
}
}

function FilterDlg_cancelCB(dlgID)
{
var dlg=this
if(dlgID) dlg=_getWidget(_getLayer(dlgID))
if(dlg) dlg.show(false)
}

function FilterDlg_enterCB()
{
}

function newFilterDlg(pid)
{
var buttonsWidth= 60;
var buttonsHeight= 52;
var dlgWidth= 300;
var dlgHeight= 100;
var textWidth= 0.9*dlgWidth;

var dlgID="filterDlg"+pid
var o=newDlgBox(dlgID,L_SetFilter,dlgWidth,dlgHeight,FilterDlg_okCB,FilterDlg_cancelCB,false);

o.promptid=pid
o.setValue=FilterDlg_setValue
o.getValue=FilterDlg_getValue
o.initDlg=FilterDlg_initDlg

var okButton = newBtn(dlgID+"_okBtn", L_OK, "FilterDlg_okCB('" + dlgID + "')", buttonsWidth, "OK", "OK", 0, 0)
var cancelButton = newBtn(dlgID+"_cancelBtn", L_Cancel, "FilterDlg_cancelCB('" +dlgID + "')", buttonsWidth, "Cancel", "Cancel", 0, 0)
var textField = newTextField(dlgID+"_textFld",null,null,null,FilterDlg_enterCB,true,null,textWidth)

_targetApp(
o.beginHTML() +
'<table cellspacing="0" cellpadding="5" border="0"><tbody>'+
'<tr>'+
'<td>'+
'<table cellspacing="0" cellpadding="0" border="0"><tbody>' +
'<tr>' +
'<td><div style="overflow:auto">'+
textField.getHTML() +
'</div></td>'+
'</tr>' +
'</tbody></table>'+
'</td>' +
'</tr>' +

'<tr>' +
'<td align="center" valign="right">' +
'</td>' +
'</tr>' + 

'<tr>' +
'<td align="right" valign="center">' +
   '<table cellspacing="0" cellpadding="0" border="0"><tbody><tr>' +
   '<td>' +   
   okButton.getHTML() +
   '</td>' +
   '<td>' + _getSpace(5,1)+ '</td>' +
   '<td>' +
   cancelButton.getHTML() +
   '</td>' +
   '</tr></tbody></table>'+   
'</td>' +
'</tr>' + 
'</table>' +
o.endHTML()
)

o.init()
okButton.init();
cancelButton.init();
textField.init();

o.textField=textField

return o
}

function FilterDlg_setValue(v)
{
var o=this
o.textField.setValue(v)
}

function FilterDlg_getValue()
{
var o=this
if (o.textField) return o.textField.getValue()
return null
}

function FilterDlg_initDlg(x,y)
{
var o=this
if(x + o.getWidth() > _winWidth() + _getScrollX()) {
	x = Math.max(0,_winWidth() + _getScrollX() - o.getWidth() - 10);
}
if(y + o.getHeight() > _winHeight() + _getScrollY()) {
	y = Math.max(0, _winHeight() + getScrollY() - o.getHeight() - 10);
}

o.move(x, y );
o.placeIframe(true,true)

var f=o.textField
f.select()
f.focus()
}

// Widget
function newCtl(id)
{
var o=new Object
o.id=id
o.layer=null
o.css=null
o.getHTML=Ctl_getHTML
o.beginHTML=Ctl_getHTML
o.endHTML=Ctl_getHTML
o.write=Ctl_write
o.begin=Ctl_begin
o.end=Ctl_end
o.init=Ctl_init
o.move=Ctl_move
o.resize=Ctl_resize
o.setBgColor=Ctl_setBgColor
o.show=Ctl_show
o.getWidth=Ctl_getWidth
o.getHeight=Ctl_getHeight
o.setHTML=Ctl_setHTML
o.setDisabled=Ctl_setDisabled
o.focus=Ctl_focus
o.setDisplay=Ctl_setDisplay
o.isDisplayed=Ctl_isDisplayed
o.setTooltip=Ctl_setTooltip
o.initialized=Ctl_initialized
o.widx=_pe._widgets.length
_pe._widgets[o.widx]=o
return o
}

function Ctl_getHTML()
{
return ''
}
function Ctl_write(i)
{
var txt = this.getHTML(i)

if (parent.writeSource)
    parent.writeSource(txt)
    
document.write(txt)
}
function Ctl_begin()
{
document.write(this.beginHTML())
}
function Ctl_end()
{
document.write(this.endHTML())
}
function Ctl_init()
{
var o=this
o.layer=_getLayer(o.id)
o.css=o.layer.style
o.layer._widget=o.widx
if (o.initialHTML)
o.setHTML(o.initialHTML)
}
function Ctl_move(x,y)
{
c=this.css;if (x!=null){if (_pe._moz) c.left=""+x+"px";else c.pixelLeft=x}if (y!=null){if (_pe._moz) c.top=""+y+"px";else c.pixelTop=y}
}
function Ctl_focus()
{
_safeSetFocus(this.layer)
}
function Ctl_setBgColor(c)
{
this.css.backgroundColor=c
}
function Ctl_show(show)
{
this.css.visibility=show?_pe._show:_pe._hide
}
function Ctl_getWidth()
{
return this.layer.offsetWidth
}
function Ctl_getHeight()
{
return this.layer.offsetHeight
}
function Ctl_setHTML(s)
{
var o=this
if (o.layer)
o.layer.innerHTML=s
else
o.initialHTML=s
}
function Ctl_setDisplay(d)
{
this.css.display=d?"":"none"
}
function Ctl_isDisplayed()
{
if(this.css.display == "none")
return false
else
return true
}
function Ctl_setDisabled(d)
{
if (this.layer)
this.layer.disabled=d
}
function Ctl_resize(w,h)
{
if (w!=null) this.css.width=''+(Math.max(0,w))+'px';if (h!=null) this.css.height=''+(Math.max(0,h))+'px';
}
function Ctl_setTooltip(tooltip)
{
this.layer.title=tooltip
}
function Ctl_initialized()
{
return this.layer!=null
}

// BlockWidget
function PE_beginBlocking()
{
var w=newBlockWidget()
w.show(true)
}

function PE_endBlocking()
{
var lyr=_getLayer(_BlockWaitWidgetID)
if (lyr)
lyr.style.display="none"
}

function newBlockWidget()
{
if (window._PEBlockWidget!=null)
return window._PEBlockWidget
var o=newCtl(_BlockWaitWidgetID)
o.getPrivateHTML=BlockWidget_getPrivateHTML
o.init=BlockWidget_init
o.show=BlockWidget_show
window._PEBlockWidget=o
return o
}
function BlockWidget_init()
{
}
function BlockWidget_getPrivateHTML()
{
return '<div id="'+ this.id+'" onselectstart="return false" ondragstart="return false" onmousedown="'+'_eventCancelBubble(event)" border="0" hspace="0" vspace="0"  style="background-image:url(\''+_pe._images+'transp.gif\')";z-index:6000;cursor:wait;position:absolute;top:0px;left:0px;width:100%;height:100%"></div>'
}
function BlockWidget_show(show)
{
var o=this
if (o.layer==null)
{
o.layer=_getLayer(o.id)
if (o.layer==null)
{
_targetApp(o.getPrivateHTML())
o.layer=_getLayer(o.id)
o.css=o.layer.style
}
else
{
o.css=o.layer.style
}
}
o.setDisplay(show)
}

// button
function newBtn(id,label,cb,width,hlp,tooltip,tabIndex,margin,url,w,h,dx,dy,imgRight,disDx,disDy)
{
var o=newCtl(id)
o.label=label
o.cb=cb
o.width=width
o.hlp=hlp
o.tooltip=tooltip
o.tabIndex=tabIndex
o.isGray=false
o.txt=null
o.icn=null
o.margin=margin?margin:0
o.extraStyle=""
if (url)
{
o.url=url
o.w=w
o.h=h
o.dx=dx
o.dy=dy
o.disDx=(disDx!=null)?disDx:dx
o.disDy=(disDy!=null)?disDy:dy
o.imgRight=imgRight?true:false
}
o.getHTML=Btn_getHTML
o.setDisabled=Btn_setDisabled
o.setText=Btn_setText
o.changeImg=Btn_changeImg
o.oldInit=o.init
o.init=Btn_init
o.isDisabled=Btn_isDisabled
o.instIndex=Btn_currInst
Btn_inst[Btn_currInst++]=o
return o;
}
Btn_inst=new Array
Btn_currInst=0
function Btn_getHTML()
{
with (this)
{
var clk='Btn_clickCB('+this.instIndex+');return false;"'
var clcbs= 'onclick="'+clk+'" '
if (_pe._ie)  clcbs+= 'ondblclick="'+clk+'" '
var url1=_pe._images+"button.gif",addPar=' style="'+extraStyle+'cursor:'+_pe._hand+';margin-left:'+margin+'px; margin-right:'+margin+'px; "'+clcbs+' ',tip=_attr('title', tooltip),idText="theBttn"+id,bg=_backImgOffset(url1,0,42),idIcon="theBttnIcon"+id
var lnkB='<a '+_attr('id',idText)+' '+tip+' '+_attr('tabindex',tabIndex)+' href="javascript:void(0)" class="wizbutton">'
var l=(label!=null)
var im=(this.url?('<td align="'+(l?(this.imgRight?'right':'left'):'center')+'" style="'+bg+'" width="'+(!l&&(width!=null)?width+6:w+6)+'">'+(l?'':lnkB)+_simpleImgOffset(url,w,h,this.isGray?disDs:dx,this.isGray?disDy:dy,idIcon,null,(l?'':tooltip),'cursor:'+_pe._hand)+(l?'':'</a>')+'</td>'):'')
return '<table '+_attr('id',id)+' '+addPar+' border="0" cellspacing="0" cellpadding="0"><tr valign="middle">'+
'<td width="5">'+_simpleImgOffset(url1,5,21,0,0)+'</td>'+
(this.imgRight?'':im)+
(l?('<td '+_attr("width",width)+' align="center" class="'+(this.isGray?'wizbuttongray':'wizbutton')+'" style="padding-left:3px;padding-right:3px;'+bg+'"><nobr>'+lnkB+label+'</a></nobr></td>'):'')+
(this.imgRight?im:'')+
'<td width="5">'+_simpleImgOffset(url1,5,21,0,21)+'</td></tr></table>';
}
}
function Btn_setDisabled(d)
{
var o=this,newCur=d?'default':_pe._hand
o.isGray=d
if (o.layer)
{
o.txt.className=d?'wizbuttongray':'wizbutton'
o.txt.style.cursor=newCur
o.css.cursor=newCur
if(o.icn)
{
_changeSimpleOffset(o.icn,o.isGray?o.disDx:o.dx,o.isGray?o.disDy:o.dy)
o.icn.style.cursor=newCur
}
}
}
function Btn_isDisabled()
{
return this.isGray
}
function Btn_setText(str)
{
this.txt.innerHTML=convStr(str)
}
function Btn_init()
{
var o=this
o.oldInit()
o.txt=_getLayer('theBttn'+this.id)
o.icn=_getLayer('theBttnIcon'+this.id)
var className=o.isGray?'wizbuttongray':'wizbutton'
if (o.txt.className!=className)
o.txt.className=className
}
function Btn_changeImg(dx,dy,disDx,disDy,url,tooltip)
{
var o=this
if (url) o.url=url
if (dx!=null) o.dx=dx
if (dy!=null) o.dy=dy
if (disDx!=null) o.disDx=disDx
if (disDy!=null) o.disDy=disDy
if (tooltip!=null) o.tooltip=tooltip
if (o.icn)
_changeSimpleOffset(o.icn,o.isGray?o.disDx:o.dx,o.isGray?o.disDy:o.dy, o.url, o.tooltip)
}
function Btn_clickCB(index)
{
var btn=Btn_inst[index]
if (btn && !btn.isGray)
setTimeout("Btn_delayClickCB("+index+")",1)
}
function Btn_delayClickCB(index)
{
var btn=Btn_inst[index]
if (btn.cb)
{
if (typeof btn.cb!="string")
btn.cb()
else
eval(btn.cb)
}
}

// text field
function newTextField(id,changeCB,maxChar,keyUpCB,enterCB,noMargin,tooltip,width,focusCB,blurCB)
{
var o=newCtl(id)
o.tooltip=tooltip
o.changeCB=changeCB
o.maxChar=maxChar
o.keyUpCB=keyUpCB
o.enterCB=enterCB
o.noMargin=noMargin
o.width=width==null?null:''+width+'px'
o.focusCB=focusCB
o.blurCB=blurCB
o.getHTML=TextField_getHTML
o.getValue=TextField_getValue
o.setValue=TextField_setValue
o.intValue=TextField_intValue
o.intPosValue=TextField_intPosValue
o.select=TextField_select
o.beforeChange=null
o.wInit=o.init
o.init=TextField_init
o.oldValue=""
return o
}
function TextField_init()
{
var o=this
o.wInit()
o.layer.value=""+o.oldValue
}
function TextField_getHTML()
{
return '<input oncontextmenu="event.cancelBubble=true;return true" style="'+_sty("width",this.width)+(_pe._moz?'padding-left:3px;padding-right:3px;':'')+'margin-left:'+(this.noMargin?0:10)+'px" onfocus="'+'TextField_focus(this)" onblur="'+'TextField_blur(this)" onchange="'+'TextField_changeCB(event,this)" onkeyup="'+'TextField_keyUpCB(event,this);return true" type="text" '+_attr('maxLength',this.maxChar)+' ondragstart="event.cancelBubble=true;return true" onselectstart="event.cancelBubble=true;return true" class="textinputs" id="'+this.id+'" name="'+this.id+'"'+_attr('title',this.tooltip)+' value="">'
}
function TextField_getValue()
{
return this.layer.value
}
function TextField_setValue(s)
{
if (this.layer)
this.layer.value=''+s
else
this.oldValue=s
}
function TextField_changeCB(e,l)
{
var o=_getWidget(l)
if(o.beforeChange)
o.beforeChange()
if(o.changeCB)
o.changeCB(e)
}
function TextField_keyUpCB(e,l)
{
var o=_getWidget(l)
if (_eventGetKey(e)==13)
{
if (o.beforeChange)
o.beforeChange()
if (o.enterCB)
o.enterCB(e)
return false
}
else if(o.keyUpCB)
{
o.keyUpCB(e)
return true
}
}
function TextField_focus(l)
{
var o=_getWidget(l)
if (o.focusCB)
o.focusCB()
}
function TextField_blur(l)
{
var o=_getWidget(l)
if(o.beforeChange)
o.beforeChange()
if (o.blurCB)
o.blurCB()
}
function TextField_intValue(nanValue)
{
var n=parseInt(this.getValue())
return isNaN(n)?nanValue:n
}
function TextField_intPosValue(nanValue)
{
var n=this.intValue(nanValue)
return (n<0)?nanValue:n
}
function TextField_select()
{
this.layer.select()
}


// dialog
function newDlgBox(id,title,width,height,defaultCB,cancelCB,noCloseButton)
{
var o=newCtl(id)
o.title=title
o.width=width
o.height=height
o.defaultCB=defaultCB
o.cancelCB=cancelCB
o.noCloseButton=noCloseButton?noCloseButton:false
o.resizeable=false
o.oldKeyPress=null
o.oldMouseDown=null
o.oldCurrent=null
o.modal=null
o.hiddenVis=new Array
o.lastLink=null
o.firstLink=null
o.titleLayer = null
o.oldInit=o.init
o.oldShow=o.show
o.init=DlgBox_init
o.setResize=DlgBox_setResize
o.beginHTML=DlgBox_beginHTML
o.endHTML=DlgBox_endHTML
o.show=DlgBox_Show
o.center=DlgBox_center
o.focus=DlgBox_focus
o.setTitle=DlgBox_setTitle
o.getContainerWidth=DlgBox_getContainerWidth
o.getContainerHeight=DlgBox_getContainerHeight
_pe.DlgBox_instances[id]=o
o.modal=newCtl('modal_'+id)
o.placeIframe=DlgBox_placeIframe
o.oldResize=o.resize
o.resize=DlgBox_resize
return o
}
function DlgBox_setResize(resizeCB,minWidth,minHeight,noResizeW,noResizeH)
{
var o=this;
o.resizeable=true
o.resizeCB=resizeCB
o.minWidth=minWidth?minWidth:50
o.minHeight=minHeight?minHeight:50
o.noResizeW=noResizeW
o.noResizeH=noResizeH
}
function DlgBox_setTitle(title)
{
var o=this
o.title=title
if (o.titleLayer == null)
o.titleLayer = _getLayer('titledialog_'+this.id);
o.titleLayer.innerHTML=_convStr(title)
}
function DlgBox_setCloseIcon(lyr,isActive)
{
_changeOffset(lyr,0,(isActive==1?0:18))
}

function DlgBox_beginHTML()
{
with (this)
{
var moveableCb=' onselectstart="return false" ondragstart="return false" onmousedown="'+'DlgBox_down(event,\''+id+'\',this,false);return false;" '
var mdl=_pe._ie?('<img onselectstart="return false" ondragstart="return false" onmousedown="'+'_eventCancelBubble(event)" border="0" hspace="0" vspace="0" src="'+_pe._images+'transp.gif" id="modal_'+id+'" style="display:none;position:absolute;top:0px;left:0px;width:1px;height:1px">'):('<div onselectstart="return false" ondragstart="return false" onmousedown="'+'_eventCancelBubble(event)" border="0" hspace="0" vspace="0" src="'+_pe._images+'transp.gif" id="modal_'+id+'" style="position:absolute;top:0px;left:0px;width:1px;height:1px"></div>')
var titleBG="background-image:url('"+_pe._images+"dialogtitle.gif')"
return mdl+
'<a style="position:absolute;left:-30px;top:-30px; visibility:hidden" id="firstLink_'+this.id+'" href="javascript:void(0)" onfocus="'+'DlgBox_keepFocus(\''+this.id+'\');return false;" ></a>'+
_getBGIframe('dlgIF_'+id)+
'<table  border="0" cellspacing="0" cellpadding="2" id="'+id+'" class="dialogbox" style="display:none;padding:0px;visibility:'+_pe._hide+';position:absolute;top:-2000px;left:-2000px;'+_sty("width",width?(""+width+"px"):null)+_sty("height",height?(""+height+"px"):null)+'"><tr><td id="dlgFirstTr_'+id+'" valign="top">'+
'<table width="100%" border="0" cellspacing="0" cellpadding="0"><tr valign="top">'+
'<td '+moveableCb+' style="cursor:move;'+titleBG+'" class="titlezone">'+_getSpace(5,18)+'</td>'+
'<td '+moveableCb+' style="cursor:move;'+titleBG+'" class="titlezone" width="100%" valign="middle" align="left"><nobr><span id="titledialog_'+id+'" tabIndex="0" class="titlezone">'+_convStr(title)+'</span></nobr></td>'+
'<td class="titlezone" style="'+titleBG+'">'+
(noCloseButton?'':'<a href="javascript:void(0)" onclick="'+'DlgBox_close(\''+id+'\');return false;" title="'+ L_closeDialog +'">'+_imgOffset(_pe._images+'dialogelements.gif',18,18,0,18,'dialogClose_'+this.id,'onmouseover="'+'DlgBox_setCloseIcon(this,1)" onmouseout="'+'DlgBox_setCloseIcon(this,0)" ',L_closeDialog,'cursor:'+_pe._hand)+'</a>')+
'</td>'+
'</tr></table></td></tr><tr valign="top" height="100%"><td id="dlgSecTr_'+id+'" >'
}
}

function DlgBox_endHTML()
{
var moveableCb=' onselectstart="return false" ondragstart="return false" onmousedown="'+'DlgBox_down(event,\''+this.id+'\',this,true);return false;" '
var resz=this.resizeable?('<tr  onselectstart="return false" height="18" valign="bottom" align="right"><td>'+_img(_pe._images+"resize.gif",14,14,null, moveableCb + ' style="cursor:NW-resize" ')+'</td></tr>'):''
return '</td></tr>'+resz+'</table><a style="position:absolute;left:-30px;top:-30px; visibility:hidden" id="lastLink_'+this.id+'" href="javascript:void(0)" onfocus="'+'DlgBox_keepFocus(\''+this.id+'\');return false;" ></a>' 
}
function DlgBox_getContainerWidth()
{
var o=this
return o.width-(2+2)
}
function DlgBox_getContainerHeight() 
{
var o=this
return o.height-(2+18+2+2+2)
}
function DlgBox_close(id) 
{
var o=_pe.DlgBox_instances[id]
if (o)
{
o.show(false)
if(o.cancelCB!=null) o.cancelCB()
}
}
function DlgBox_resizeIframeCB(id)
{
_pe.DlgBox_instances[id].placeIframe(true,false)
}
function DlgBox_placeIframe(bResize,bMove)
{
var o=this
if (o.iframe)
{
if (bResize)
o.iframe.resize(o.getWidth(),o.getHeight())
if (bMove)
o.iframe.move(o.layer.offsetLeft,o.layer.offsetTop)
}
}
function DlgBox_resize(w,h)
{
var o=this;
o.oldResize(w,h);
if (o.iframe)
{
o.iframe.resize(w,h);
if (o.firstTR)
{
if (w!=null)
o.firstTR.style.width=w-4
if (h!=null)
o.secondTR.style.height=h-44
}
}
}
function DlgBox_init()
{
if (this.layer!=null)
return
var o=this
o.oldInit();
o.modal.init();
o.lastLink=newCtl("lastLink_"+o.id)
o.firstLink=newCtl("firstLink_"+o.id)
o.lastLink.init()
o.firstLink.init()
if (!o.noCloseButton)
{
o.closeButton=_getLayer('dialogClose_'+o.id)
DlgBox_setCloseIcon(o.closeButton,false)
}
if (_pe._moz&&!_pe._saf)
{
o.firstTR=_getLayer("dlgFirstTr_"+o.id)
o.secondTR=_getLayer("dlgSecTr_"+o.id)
}
o.iframe=newCtl('dlgIF_'+o.id)
o.iframe.init()
}

function DlgBox_down(e,id,obj,isResize)
{
_pe._dlgResize=isResize
var o=_pe.DlgBox_instances[id],lyr=o.layer,mod=o.modal.layer
lyr.onmousemove=mod.onmousemove=eval('DlgBox_move')
lyr.onmouseup=mod.onmouseup=eval('DlgBox_up')
lyr.dlgStartPosx=mod.dlgStartPosx=parseInt(lyr.style.left)
lyr.dlgStartPosy=mod.dlgStartPosy=parseInt(lyr.style.top)
lyr.dlgStartx=mod.dlgStartx=_eventGetX(e)
lyr.dlgStarty=mod.dlgStarty=_eventGetY(e)
lyr.dlgStartw=mod.dlgStartw=o.getWidth()
lyr.dlgStarth=mod.dlgStarth=o.getHeight()
lyr._widget=mod._widget=o.widx
_pe._theLYR=lyr
_eventCancelBubble(e)
if (lyr.setCapture)
lyr.setCapture(true)
}
function DlgBox_move(e)
{
var o=_pe._theLYR,dlg=_getWidget(o)

if(dlg)
{
if (_pe._dlgResize)
{
var newW=Math.max(dlg.minWidth,o.dlgStartw+_eventGetX(e)-o.dlgStartx)
var newH=Math.max(dlg.minHeight,o.dlgStarth+_eventGetY(e)-o.dlgStarty)
dlg.resize(dlg.noResizeW?null:newW,dlg.noResizeH?null:newH)
if (dlg.firstTR)
{
if (!dlg.noResizeW)
dlg.firstTR.style.width=newW-4
if (!dlg.noResizeH)
dlg.secondTR.style.height=newH-44
}
if (dlg.resizeCB)
dlg.resizeCB(newW,newH)
}
else
{
var x=Math.max(0,o.dlgStartPosx-o.dlgStartx+_eventGetX(e))
var y=Math.max(0,o.dlgStartPosy-o.dlgStarty+_eventGetY(e))
x = Math.min( Math.max(10,_winWidth()-10), x)
y = Math.min( Math.max(10,_winHeight()-18), y)
dlg.iframe.move(x,y)
dlg.move(x,y)
}
}
_eventCancelBubble(e)
return false
}
function DlgBox_up(e)
{
var o=_getWidget(_pe._theLYR),lyr=o.layer,mod=o.modal.layer;
lyr.onmousemove=mod.onmousemove=null;
lyr.onmouseup=mod.onmouseup=null;
if (lyr.releaseCapture)
lyr.releaseCapture();
_pe._theLYR=null
}
function DlgBox_keypress(e)
{
var dlg=_pe.DlgBox_current
if (dlg!=null)
{
switch( _eventGetKey(e))
{
case 13:
var sourceId = _pe._ie?window.event.srcElement.id:e.target.id;
if ((sourceId == "insRepText" || sourceId == "renRepText") && (dlg.defaultCB!=null))
{
dlg.defaultCB();return false;
}
if(dlg.yes && !dlg.no){ dlg.defaultCB();return false; }
break;
case 27:
dlg.show(false)
//hideBlockWhileWaitWidget()
if (dlg.cancelCB!=null) dlg.cancelCB()
return false;
break;
case 8: 
return _isTextInput(_pe._ie?window.event:e);
break;
}
}
}
function DlgBoxResizeModals(e)
{
for (var i in _pe.DlgBox_modals)
{
m_sty=_pe.DlgBox_modals[i]
m_sty.width=_documentWidth();
m_sty.height=_documentHeight();
}
}
function DlgBox_center()
{
var o=this,scrY=_getScrollY(),scrX=_getScrollX()
o.height=o.layer.offsetHeight;
o.width=o.layer.offsetWidth;
o.move(Math.max(0,scrX+(_winWidth()-o.width)/2),Math.max(0,scrY+(_winHeight()-o.height)/2));
o.placeIframe(true,true)
}
function DlgBox_Show(sh)
{
with (this)
{
m_sty=modal.css
l_sty=css
if (sh)
{
oldCurrent=_pe.DlgBox_current
_pe.DlgBox_current=this
if (_pe._ie)
{
oldKeyPress=document.onkeydown
document.onkeydown=eval('window.'+'DlgBox_keypress')
}
else
{
document.addEventListener("keydown", eval('window.'+'DlgBox_keypress'), false)
}
oldMouseDown=document.onmousedown
document.onmousedown=null
//hideBlockWhileWaitWidget()
_disableAllInputs()
}
else
{
_pe.DlgBox_current=oldCurrent
oldCurrent=null
if (_pe._ie)
{
document.onkeydown=oldKeyPress
}
else
{
document.removeEventListener("keydown", eval('window.'+'DlgBox_keypress'), false)
}
document.onmousedown=oldMouseDown
_restoreAllDisabledInputs()
}
var sameState=(layer.isShown==sh)
if (sameState)
return
layer.isShown=sh
if (sh)
{
if (window.DialogBoxWidget_zindex==null)
window.DialogBoxWidget_zindex=1000
this.iframe.css.zIndex=window.DialogBoxWidget_zindex++;
m_sty.zIndex=window.DialogBoxWidget_zindex++;
l_sty.zIndex=window.DialogBoxWidget_zindex++;
_pe.DlgBox_modals[_pe.DlgBox_modals.length]=m_sty
m_sty.display=''
l_sty.display='block'
this.iframe.setDisplay(true)
DlgBoxResizeModals()
this.height=layer.offsetHeight;
this.width=layer.offsetWidth;
if (_isHidden(layer))
{
this.center()
}
if (this.firstTR)
{
this.firstTR.style.width=this.getWidth()-4
this.secondTR.style.height=this.getHeight()-44
}
if (this.resizeCB)
this.resizeCB(this.width,this.height)
}
else
{
var l=_pe.DlgBox_modals.length=Math.max(0,_pe.DlgBox_modals.length-1)
m_sty.width='1px'
m_sty.height='1px'
m_sty.display='none'
l_sty.display='none'
move(-2000,-2000);
this.iframe.setDisplay(false)
}
modal.show(sh);
firstLink.show(sh)
lastLink.show(sh)
oldShow(sh);
if (_pe.DlgBox_current!=null && sh==true)
_pe.DlgBox_current.focus();
}
}
function DlgBox_keepFocus(id)
{
var o=_pe.DlgBox_instances[id];
if (o) o.focus();
}
function DlgBox_focus()
{
with (this)
{
if (titleLayer == null)
titleLayer = _getLayer('titledialog_'+id);
if (titleLayer.focus)titleLayer.focus();
}
}

//////////////////////////////////////////////////////
// promptengine_prompts functions
//////////////////////////////
// GLOBAL VAR
var isJava = false;  // do encodeURIComponent for Java only

var isNetscape = navigator.appName.indexOf("Netscape") != -1;

var LEFT_ARROW_KEY = 37;
var RIGHT_ARROW_KEY = 39;
var ENTER_KEY = 13;

///////////////////////////////
// properly encode prompt values
function promptengine_encodePrompt (prompt)
{
    if (isJava)
    {
        return encodeURIComponent(prompt);
    }
    else
    {
        return promptengine_urlEncode(prompt);
    }
}

////////////////////////////////
// add number, currency, string from dropdown/textbox to list box
// where multiple prompt values are supported
function promptengine_addDiscreteValue (
    fid,
    type,
    pid)
{
    var form=document.getElementById(fid)
    
    var sLyr = document.getElementById(pid + "DiscreteValue")
    var src = sLyr
    var sLT=sLyr.type.toLowerCase()
    
    var fromLB=false
    if (sLT!="text" && sLT!="hidden" && sLT!="password")
    {
        //select box not a textbox
        src = sLyr.options[sLyr.selectedIndex];
        fromLB=true
    }
    
    var sval=src.value
    if (!promptengine_checkValue (sval, type) )
    {
        _safeSetFocus(sLyr)
        return false;
    }

    var dLyr = document.getElementById(pid + "ListBox");
    PE_clearSel(dLyr)

    var si=promptengine_findOptionInList(dLyr,sval)
    if (si < 0)
    {
        si=dLyr.length
        dLyr.options[si] = new Option(((src.text)?src.text:sval),sval,false,false);
    }
    
    dLyr.options[si].selected=true

    _safeSetFocus(sLyr)
    
    if (sLyr.select) sLyr.select();
    if (fromLB && sLyr.selectedIndex < sLyr.length - 1)
        sLyr.selectedIndex = sLyr.selectedIndex + 1;      //... or move to next selection in listbox
}

function PE_clearSel(lb)
{
var i=0, c=lb.length

if(lb.type=='select-one') {
i=lb.selectedIndex
if (i<0) return
c=i+1
}

while(i<c) lb.options[i++].selected=false
}

////////////////////////////////
// add number, currency, string from available list box to selected list box
// where multiple prompt values are supported
function promptengine_addValueFromPickList(
    form,
    type,
    pid)
{
return PE_addValues(form,type,pid,false)
}

////////////////////////////////
// add all number, currency, string from available list box to selected list box
// where multiple prompt values are supported
function promptengine_addAllValues(
    form,
    type,
    pid)
{
return PE_addValues(form,type,pid,true)
}

function PE_addValues(form,type,pid,all)
{
var alLyr = document.getElementById(pid + "AvailableList");
var slLyr = document.getElementById(pid + "ListBox");

var numOfAL=alLyr.length
if(numOfAL==0) return false

var numOfSL=slLyr.length
var alOpts=alLyr.options;
var slOpts=slLyr.options;

var copyAL=new Array(numOfAL)
var copySL=new Array(numOfSL)
var redraw=false
var lastSI=-1;
for(var i=0;i<numOfAL;i++)
{
if(all || alOpts[i].selected)
{
var v=alOpts[i].value
var si=promptengine_findOptionInList(slLyr, v, alOpts[i].text)
if(si<0) copyAL[i]=v
else copySL[si]=v
redraw=true
if(!all) lastSI=i
}
}

if(!redraw) return false

var slCtl=PE_getLB(slLyr)

for(var i=0;i<numOfSL;i++)
{
var opt=slOpts[i]
slCtl.add(opt.value, opt.text, copySL[i]!=null)
}
 
var changed = false;
for (var i=0; i <numOfAL; i++)
{
if(copyAL[i])
{
var opt=alOpts[i]
slCtl.add(opt.value, opt.text,true)
changed=true
}
}

slCtl.update()

// set focus to the next item in the available list
if(!all && lastSI >= 0 && lastSI+1<numOfAL)
{
PE_clearSel(alLyr)
alOpts[lastSI+1].selected=true
}

return changed;
}

////////////////////////////////////
// adds Range prompt to listbox where multiple values are supported
function promptengine_addRangeValue (
    form,
    type,
    promptID )
{
    var lowerBoundPickList = document.getElementById(promptID + "SelectLowerRangeValue");
    var upperBoundPickList = document.getElementById(promptID + "SelectUpperRangeValue");
    
    lowerBound = document.getElementById(promptID + "LowerBound");
    upperBound = document.getElementById(promptID + "UpperBound");
    //handle select box, not text box case
    if ( lowerBound.type.toLowerCase () != "text" &&
     lowerBound.type.toLowerCase () != "hidden" &&
     lowerBound.type.toLowerCase () != "password" )  //either upper or lower, doesn't matter
    {
        lowerBound = lowerBound.options[lowerBound.selectedIndex];
        upperBound = upperBound.options[upperBound.selectedIndex];
    }

    lowerUnBounded = document.getElementById(promptID + "NoLBoundCheck").checked;
    upperUnBounded = document.getElementById(promptID + "NoUBoundCheck").checked;
    lvalue = uvalue = "";
    
    if ( ! lowerUnBounded )
    {
        if ( ! promptengine_checkRangeBoundValue ( lowerBound.value, type ) ) {
            if ( lowerBound.focus && lowerBound.type.toLowerCase () != "hidden")
                lowerBound.focus ();
            return false;
        }
        lvalue = lowerBound.value;
    }
    if ( ! upperUnBounded )
    {
        if ( ! promptengine_checkRangeBoundValue ( upperBound.value, type ) ) {
            if ( upperBound.focus && upperBound.type.toLowerCase () != "hidden")
                upperBound.focus ();
            return false;
        }
        uvalue = upperBound.value;
    }
    
    var ldisplay = "";
    var udisplay = "";
    
    var found = false;
    if (lowerBoundPickList != null && lvalue != null && lvalue.length > 0)
    {
        var cItems = lowerBoundPickList.length;
        for (var item = 0; item < cItems; item++)
        {
            var value = lowerBoundPickList.options[item].value;
            if (value != null && value.length > 0 && value == lvalue)
            {
                ldisplay = lowerBoundPickList.options[item].text;
                found = true;
                break;
            }
        }
    }
    if (!found)
        ldisplay = (lowerBound.text && !lowerUnBounded) ? lowerBound.text : lvalue;
        
    found = false;
    if (upperBoundPickList != null && uvalue != null && uvalue.length > 0)
    {
        var cItems = upperBoundPickList.length;
        for (var item = 0; item < cItems; item++)
        {
            var value = upperBoundPickList.options[item].value;
            if (value != null && value == uvalue)
            {
                udisplay = upperBoundPickList.options[item].text;
                found = true;
                break;
            }
        }
    }
    if (!found)
        udisplay = (upperBound.text && !upperUnBounded) ? upperBound.text : uvalue;

    lowerChecked = document.getElementById(promptID + "LowerCheck").checked;
    upperChecked = document.getElementById(promptID + "UpperCheck").checked;

    // value for showing in the list box only, no need encode here
    value = ( lowerChecked && ! lowerUnBounded ) ? "[" : "(";
    if ( ! lowerUnBounded ) //unbounded is empty string not quoted empty string (e.g not "_crEMPTY_")
        value += (lvalue);
    value += "_crRANGE_"
    if ( ! upperUnBounded )
        value += (uvalue);
    value += (upperChecked && ! upperUnBounded ) ? "]" : ")";

    display = ( lowerChecked && ! lowerUnBounded ) ? "[" : "(";
    display += ldisplay;
    display += " .. "
    display += udisplay;
    display += (upperChecked && ! upperUnBounded ) ? "]" : ")";

    promptEntry = new Option(display,value,false,false);
    theList = document.getElementById(promptID + "ListBox");
    
    // search the list/select box for the new option, if the returned index is -1, i.e., no such option, add it
    // otherwise, highlight that option
    var idx = promptengine_findOptionInList(theList, value);
    if (idx  > -1)
        theList.selectedIndex = idx;
    else
        theList.options[theList.length] = promptEntry;
	
    return true;
}

////////////////////////////////////
// search the given select object for the given option value, and returns its index.
function promptengine_findOptionInList(selectObj,val)
{
if (selectObj==null || val==null )
    return -1;

var c=selectObj.length, opts=selectObj.options
for(var i=0;i<c;i++)
{
if(opts[i].value==val) return i
}
return -1
}

////////////////////////////////////
// disable check boxes / dropdowns / textboxes based on user selection on the range parameters
function promptengine_onNoBoundCheckClicked(
    form,
    promptID,
    lowOrUpBound)
{
    if (lowOrUpBound == 0) {
        if (document.getElementById(promptID + "NoLBoundCheck").checked) {
            document.getElementById(promptID + "NoUBoundCheck").disabled = true;
            document.getElementById(promptID + "LowerCheck").disabled = true;
            document.getElementById(promptID + "LowerBound").disabled = true;
            if (document.getElementById(promptID + "SelectLowerRangeValue") != null)
                document.getElementById(promptID + "SelectLowerRangeValue").disabled = true;
        }
        else {
            document.getElementById(promptID + "NoUBoundCheck").disabled = false;
            document.getElementById(promptID + "LowerCheck").disabled = false;
            document.getElementById(promptID + "LowerBound").disabled = false;
            if (document.getElementById(promptID + "SelectLowerRangeValue") != null)
                document.getElementById(promptID + "SelectLowerRangeValue").disabled = false;
        }
    } else if (lowOrUpBound == 1) {
        if (document.getElementById(promptID + "NoUBoundCheck").checked) {
            document.getElementById(promptID + "NoLBoundCheck").disabled = true;
            document.getElementById(promptID + "UpperCheck").disabled = true;
            document.getElementById(promptID + "UpperBound").disabled = true;
            if (document.getElementById(promptID + "SelectUpperRangeValue") != null)
                document.getElementById(promptID + "SelectUpperRangeValue").disabled = true;
        } else {
            document.getElementById(promptID + "NoLBoundCheck").disabled = false;
            document.getElementById(promptID + "UpperCheck").disabled = false;
            document.getElementById(promptID + "UpperBound").disabled = false;
            if (document.getElementById(promptID + "SelectUpperRangeValue") != null)
                document.getElementById(promptID + "SelectUpperRangeValue").disabled = false;
        }
    }
}

////////////////////////////////////
// disable text boxes / list boxes based on whether setNull is checked
function promptengine_onSetNullCheckClicked(
    form,
    promptID)
{
    if (document.getElementById(promptID + "NULL").checked)
    {
        if (document.getElementById(promptID + "DiscreteValue") != null)
            document.getElementById(promptID + "DiscreteValue").disabled = true;
        if (document.getElementById(promptID + "SelectValue") != null)
            document.getElementById(promptID + "SelectValue").disabled = true;
    }
    else
    {
        if (document.getElementById(promptID + "DiscreteValue") != null)
            document.getElementById(promptID + "DiscreteValue").disabled = false;
        if (document.getElementById(promptID + "SelectValue") != null)
            document.getElementById(promptID + "SelectValue").disabled = false;            
    }
}

////////////////////////////////////
// puts "select" value into text box for an editable prompt which also has defaults
function promptengine_selectValue(
    form,
    selectCtrl,
    textCtrl)
{
    // If no selection, return unchanged.
    if(document.getElementById(selectCtrl).selectedIndex < 0)
        return false;

    selectedOption = document.getElementById(selectCtrl).options[document.getElementById(selectCtrl).selectedIndex];
    if (selectedOption.value == null && document.getElementById(textCtrl).value == null)
        return false;

    var changed = true;
    if (selectedOption.value == document.getElementById(textCtrl).value)
        changed = false;

    document.getElementById(textCtrl).value = selectedOption.value;
    return changed;
}

function promptengine_hasValueInTextBox(
    form,
    textboxID)
{
    if (document.getElementById(textboxID).value == null)
        return false;
    return true;
}

/////////////////////////////////////////
// set cascading prompt id into value field.
function promptengine_setCascadingPID(
    form,
    valueID,
    promptID)
{
    valueField = document.getElementById(valueID);

    curVal = valueField.value;

    if (curVal.length > 0)
        curVal += "&";
    curVal +=  "cascadingPID" + "=" + promptID;

    valueField.value = curVal;
    return true;
}

/////////////////////////////////////////////////
// remove selected values from multi-value prompt
function PE_removeValue(
    form,
    pid,
    all)
{
    var lyr = document.getElementById(pid+"ListBox")
    var opts=lyr.options
    var len= lyr.length
    if (len==0) return false
    
    var changed = false
    var lastSelected = -1

    var lbCtl=PE_getLB(lyr)
    for(var i=0; i<len; i++)
    {
        if(!all)
        {
            var opt=opts[i]
            if (!opt.selected)
            {
                lbCtl.add(opt.value,opt.text)
                continue
            }
            lastSelected=i
        }
        changed=true
    }
    
    if(!changed) return false
    
    lbCtl.update()
    
    // resync and update selection
    if (lastSelected >= 0)
    {
        lyr = document.getElementById(pid+"ListBox")
        if (lastSelected < lyr.length)
            lyr.options[lastSelected].selected = true;  // highlight the next item
        else if (lastSelected == lyr.length && lastSelected > 0)
            lyr.options[lastSelected-1].selected = true; // highlight the last item
    }
    
    return true;
}

function promptengine_removeValue(form, pid)
{
return PE_removeValue(form,pid,false)
}

function promptengine_onRemoveValue(
    form,
    promptID)
{
    promptengine_removeValue(form, promptID);
}

/////////////////////////////////////////////////
// remove all values from multi-value prompt
function promptengine_removeAllValues(form, pid)
{
return PE_removeValue(form,pid,true)
}

function promptengine_onRemoveAllValues(
    form,
    promptID)
{
    promptengine_removeAllValues(form, promptID);
}

/////////////////////////////////////
// update hidden value field with encoded value
function promptengine_updateValueField (
    form,
    valueID,
    promptID,
    value)
{
    valueField = document.getElementById(valueID);

    curVal = valueField.value;

    if (curVal.length > 0)
        curVal += "&";
        
    var encoded = promptengine_encodeValueField(value);
        
    curVal += promptID + "=" + encoded;

    valueField.value = curVal;

    return true;
}

///////////////////////////////////////
// reset hidden value field
function promptengine_resetValueField (
    form,
    valueID)
{
    valueField = document.getElementById(valueID);
    valueField.value = "";
}

/////////////////////////////////////
// sets prompt value into the hidden form field in proper format so that it can be submitted
function promptengine_updateDiscreteValue (
    form,
    valueID,
    promptID,
    type,
    checkValue,
    valueRequired)
{
    var value = "";

    if (document.getElementById(promptID + "NULL") != null &&
        document.getElementById(promptID + "NULL").checked )
    {
        value = "_crNULL_"; //NULL is a literal for, uhmm.. a NULL
    }
    else
    {
        valueField = document.getElementById(promptID + "DiscreteValue");

        if (valueField.type.toLowerCase () != "text" &&
            valueField.type.toLowerCase () != "hidden" &&
            valueField.type.toLowerCase () != "password")
        {
            value = valueField.options[valueField.selectedIndex].value;
        }
        else
        {
            value = valueField.value;
        }
        
        if (!valueRequired && (value == null || value.length == 0)) {
            return promptengine_updateValueField(form, valueID, promptID, "");
        }

        if ( checkValue && !promptengine_checkValue ( value, type ) )
        {
            if (valueField.focus && valueField.type.toLowerCase () != "hidden")
                valueField.focus ();
	    else
	    {
		var focusField = document.getElementById(promptID + "SelectValue");
		if (focusField != null && focusField.focus)
			focusField.focus();
	    }

            return false;
        }
    }

    return promptengine_updateValueField(form, valueID, promptID, value);
}

/////////////////////////////////////
// sets prompt value for a range into the hidden form field in proper format so that it can be submitted
function promptengine_updateRangeValue (
    form,
    valueID,
    promptID,
    type,
    checkValue,
    valueRequired)
{
    if (document.getElementById(promptID + "NULL") != null &&
        document.getElementById(promptID + "NULL").checked )
    {
        value = "_crNULL_"; //NULL is a literal for, uhmm.. a NULL
    }
    else
    {
        lowerBound = document.getElementById(promptID + "LowerBound");
        upperBound = document.getElementById(promptID + "UpperBound");

        //handle select box, not text box case
        if (lowerBound.type.toLowerCase () != "text" &&
            lowerBound.type.toLowerCase () != "hidden" &&
            lowerBound.type.toLowerCase () != "password")  //either upper or lower, doesn't matter
        {
            lowerBound = lowerBound.options[lowerBound.selectedIndex];
            upperBound = upperBound.options[upperBound.selectedIndex];
        }
        lowerUnBounded = document.getElementById(promptID + "NoLBoundCheck").checked;
        upperUnBounded = document.getElementById(promptID + "NoUBoundCheck").checked;
        lowerChecked = document.getElementById(promptID + "LowerCheck").checked;
        upperChecked = document.getElementById(promptID + "UpperCheck").checked;
        uvalue = lvalue = "";

        if (!valueRequired && 
            (lowerBound.value == null || lowerBound.value.length == 0 || lowerUnBounded) && 
            (upperBound.value == null || upperBound.value.length == 0 || upperUnBounded)) {
            return promptengine_updateValueField(form, valueID, promptID, "");
        }
        
        if ( ! lowerUnBounded )
        {
            if ( checkValue && !promptengine_checkRangeBoundValue ( lowerBound.value, type ) ) {
                if ( lowerBound.focus && lowerBound.type.toLowerCase () != "hidden")
                    lowerBound.focus();
                else
                {
                    var focusField = document.getElementById(promptID + "SelectLowerRangeValue");
                    if (focusField != null && focusField.focus)
	                    focusField.focus();
                }
                return false;
            }
            lvalue = lowerBound.value;
        }
        if ( ! upperUnBounded )
        {
            if ( checkValue && !promptengine_checkRangeBoundValue ( upperBound.value, type ) ) {
                if ( upperBound.focus && upperBound.type.toLowerCase () != "hidden")
                    upperBound.focus ();
                else
                {
                    var focusField = document.getElementById(promptID + "SelectUpperRangeValue");
                    if (focusField != null && focusField.focus)
	                    focusField.focus();
                }
                return false;
            }
            uvalue = upperBound.value;
        }
        value = ( lowerChecked && ! lowerUnBounded ) ? "[" : "(";
        if ( ! lowerUnBounded )
            value += lvalue;
        value += "_crRANGE_"
        if ( ! upperUnBounded )
            value += uvalue;
        value += (upperChecked && ! upperUnBounded ) ? "]" : ")";
    }

    return promptengine_updateValueField(form, valueID, promptID, value);
}

/////////////////////////////////////
// sets prompt value into the hidden form field in proper format so that it can be submitted
function promptengine_updateMultiValue (
    form,
    valueID,
    promptID,
    type,
    checkValue,
    valueRequired)
{
    values = document.getElementById(promptID + "ListBox").options;
    value = "";

    if (document.getElementById(promptID + "NULL") != null &&
        document.getElementById(promptID + "NULL").checked)
    {
        value = "_crNULL_"; //NULL is a literal for, uhmm.. a NULL
    }
    else
    {
        if ( values.length == 0 )
        {
            if (checkValue && valueRequired)
            {
                var focusField = document.getElementById(promptID + "ListBox");
                if (focusField != null && focusField.focus)
	                focusField.focus();
                return false;
            }
            
            value = "_crEMPTY_";     //if value is empty, set to empty string
        }
        else
        {
            for ( i = 0; i < values.length; i++ )
            {
                // first value could be empty string, then chcking value.length != 0 could miss one empty string
                if ( i != 0 )
                    value += "_crMULT_"
                value += values[i].value;
            }
        }
    }

    //NOTE: we'll always return true as the validation is done before values are added to select box
    return promptengine_updateValueField(form, valueID, promptID, value);
}

///////////////////////////////////////
// check and alert user about any errors based on type of prompt
var regNumber    = /^(\+|-)?((\d+(\.|,|'| |\xA0)?\d*)+|(\d*(\.|,| |\xA0)?\d+)+)$/
var regCurrency  = regNumber;
var regDate  = /^(D|d)(A|a)(T|t)(E|e) *\( *\d{4} *, *(0?[1-9]|1[0-2]) *, *((0?[1-9]|[1-2]\d)|3(0|1)) *\)$/
var regDateTime  = /^(D|d)(A|a)(T|t)(E|e)(T|t)(I|i)(M|m)(E|e) *\( *\d{4} *, *(0?[1-9]|1[0-2]) *, *((0?[1-9]|[1-2]\d)|3(0|1)) *, *([0-1]?\d|2[0-3]) *, *[0-5]?\d *, *[0-5]?\d *\)$/
var regTime  = /^(T|t)(I|i)(M|m)(E|e) *\( *([0-1]?\d|2[0-3]) *, *[0-5]?\d *, *[0-5]?\d *\)$/
var regDateTimeHTML  = /^ *\d{4} *- *(0?[1-9]|1[0-2]) *- *((0?[1-9]|[1-2]\d)|3(0|1)) *  *([0-1]?\d|2[0-3]) *: *[0-5]?\d *: *[0-5]?\d *$/
var regDateHTML  = /^ *\d{4} *- *(0?[1-9]|1[0-2]) *- *((0?[1-9]|[1-2]\d)|3(0|1)) *$/
var regTimeHTML  = /^ *([0-1]?\d|2[0-3]) *: *[0-5]?\d *: *[0-5]?\d *$/

function promptengine_getDateSpec()
{
    var datePattern = promptengine_getDatePattern();
    datePattern = datePattern.replace("Y", L_YYYY);
    datePattern = datePattern.replace("M", L_MM);
    datePattern = datePattern.replace("D", L_DD);
    return datePattern;
}

function promptengine_checkValue (
    value,
    type)
{
    if (value == null)
        return false;
        
    if (value=="_crNULL_")
        return true;
        
    if (type==_pe._nm && !regNumber.test (value))
    {
        if (value.length > 0)
            alert ( L_BadNumber );
        else
            alert ( (typeof L_Empty) != 'undefined'? L_Empty : L_NoValue );
            
        return false;
    }
    else if (type==_pe._cy && !regCurrency.test ( value ))
    {
        if (value.length > 0)
            alert ( L_BadCurrency );
        else
            alert ( (typeof L_Empty) != 'undefined'? L_Empty : L_NoValue );
        return false;
    }
    else if (type==_pe._da)
    {
        var regex = promptengine_getDateRegex();
        if((regex == null || !regex.test(value)) && ! regDate.test ( value ) && !regDateHTML.test( value ))
        {
            if (value.length > 0)
            {
                var badDate = L_BadDate.replace("%1", promptengine_getDateSpec());
                alert ( badDate );
            }
	    else
	        alert ( (typeof L_Empty) != 'undefined'? L_Empty : L_NoValue );

            return false;
        }
    }
    else if (type==_pe._dt)
    {
        var regex = promptengine_getDateTimeRegex();
        if((regex == null || !regex.test(value)) && ! regDateTime.test ( value ) && !regDateTimeHTML.test( value ))
        {
            if (value.length > 0)
            {
                var badDateTime = L_BadDateTime.replace("%1", promptengine_getDateSpec());
                alert ( badDateTime );
            }
	    else
	        alert ( (typeof L_Empty) != 'undefined'? L_Empty : L_NoValue );

            return false;
        }
    }
    else if (type==_pe._tm && !regTime.test ( value ) && !regTimeHTML.test( value )  )
    {
        if (value.length > 0)
            alert ( L_BadTime );
	else
            alert ( (typeof L_Empty) != 'undefined'? L_Empty : L_NoValue );

        return false;
    }

    //by default let it go...
    return true;
}

function promptengine_checkRangeBoundValue (
    value,
    type)
{
    if (value == null || value.length == 0)
    {
        alert ( (typeof L_Empty) != 'undefined'? L_Empty : L_NoValue );
        return false;
    }
        
    return promptengine_checkValue(value, type);    
}

function promptengine_isSubmitEvent(evt)
{
var b=false
if (isNetscape)
{
if (evt.which == ENTER_KEY && (evt.target.type == "text" || evt.target.type == "password"))
    b=true;
}
else
{
if (window.event.keyCode == ENTER_KEY && (window.event.srcElement.type == "text" || window.event.srcElement.type == "password"))
    b=true;
}

if (b) _eventCancelBubble(evt)
return b;
}

function promptengine_isEnterKey(evt)
{
var b=false
if (isNetscape)
{
if (evt.which == ENTER_KEY && evt.target.tagName.toLowerCase() != "a")
    b=true;
}
else
{
if (window.event.keyCode == ENTER_KEY && window.event.srcElement.tagName.toLowerCase() != "a")
    b=true;
}

if (b) _eventCancelBubble(evt)
return b
}

//This function should only be called from the COM components.
//Use encodeURIComponent for Java
function promptengine_urlEncode(strToBeEncoded)
{
    var encodedString = new String("");
    for( var i = 0; i < strToBeEncoded.length; i++ )
    {
        var nextChar = strToBeEncoded.charAt(i);
        switch( nextChar )
        {
            //Unsafe characters
            case '%':
            {
                encodedString += "%25";
                break;
            }
            case '+':
            {
                encodedString += "%2B";
                break;
            }
            case ' ':
            {
                encodedString += "%20";
                break;
            }
            case '<':
            {
                encodedString += "%3C";
                break;
            }
            case '>':
            {
                encodedString += "%3E";
                break;
            }
            case '"':
            {
                encodedString += "%22";
                break;
            }
            case '\'':
            {
                encodedString += "%27";
                break;
            }
            case '#':
            {
                encodedString += "%23";
                break;
            }
            case '{':
            {
                encodedString += "%7B";
                break;
            }
            case '}':
            {
                encodedString += "%7D";
                break;
            }
            case '|':
            {
                encodedString += "%7C";
                break;
            }
            case '\\':
            {
                encodedString += "%5C";
                break;
            }
            case '^':
            {
                encodedString += "%5E";
                break;
            }
            case '~':
            {
                encodedString += "%7E";
                break;
            }
            case '`':
            {
                encodedString += "%60";
                break;
            }
            case '[':
            {
                encodedString += "%5B";
                break;
            }
            case ']':
            {
                encodedString += "%5D";
                break;
            }
            //Reserved characters
            case ';':
            {
                encodedString += "%3B";
                break;
            }
            case '/':
            {
                encodedString += "%2F";
                break;
            }
            case '?':
            {
                encodedString += "%3F";
                break;
            }
            case ':':
            {
                encodedString += "%3A";
                break;
            }
            case '@':
            {
                encodedString += "%40";
                break;
            }
            case '=':
            {
                encodedString += "%3D";
                break;
            }
            case '&':
            {
                encodedString += "%26";
                break;
            }
            default:
            {
                encodedString += nextChar;
                break;
            }
        }
    }

    return encodedString;
}

function promptengine_CancelRightClick(evt)
{
    if (isNetscape)
    {
        if (evt.target.type != "text" && evt.target.type != "textarea")
        {
                evt.preventDefault();
                evt.cancelBubble = true;
                return true;
        }
    }
    else
    {
        if (window.event.srcElement.type != "text" && window.event.srcElement.type != "textarea")
        {
                window.event.cancelBubble = true;
                window.event.returnValue = false;
        }
    }
}

function promptengine_showHidePromptByKey(fieldSetId, imgId, currentImgPath, changeImgPath, evt)
{
    var correctKey = false;
    var fieldSet = document.getElementById(fieldSetId);
    
    if (fieldSet == null)
        return;

    if (isNetscape)
    {
        if ( (evt.which == LEFT_ARROW_KEY && fieldSet.style.display == "") || 
            (evt.which == RIGHT_ARROW_KEY && fieldSet.style.display == "none") )
                correctKey = true;
    }
    else
    {
        if ( (window.event.keyCode == LEFT_ARROW_KEY && fieldSet.style.display == "") || 
            (window.event.keyCode == RIGHT_ARROW_KEY && fieldSet.style.display == "none") )
                correctKey = true;
    }

    if (correctKey == true)
        promptengine_showHidePrompt(fieldSetId, imgId, currentImgPath, changeImgPath, evt)
}

function promptengine_showHidePrompt(fieldSetId, imgId, currentImgPath, changeImgPath, evt)
{
    var imgElem;    
    imgElem = document.getElementById(imgId);

    if (imgElem!= null && fieldSetId != null)
    {
        if (!imgElem.origImage)
            imgElem.origImage = imgElem.src;    

        var fieldSet = document.getElementById(fieldSetId);
        if (fieldSet != null)
        {
            if (fieldSet.style.display == "")
                fieldSet.style.display = "none";
            else
                fieldSet.style.display = "";

            if (!imgElem.changed || imgElem.changed == false)
            {
                imgElem.src = changeImgPath;
                imgElem.changed = true;
            }
            else
            {
                imgElem.src = currentImgPath;
                imgElem.changed = false;
            }
        }
    }
}

function promptengine_scrollTo(elt)
{
    if (!elt) return; 

    var scrY=_getScrollY(),scrX=_getScrollX()

    // Ajax
    if (elt.form)
    {
        var h=elt.form.offsetHeight, winCY=elt.form.clientHeight, y=_getPos(elt,elt.form).y

        elt.form.scrollLeft=scrX;
        if (y<scrY) elt.form.scrollTop=y;
        else if (y+h>scrY+winCY) elt.form.scrollTop=Math.max(y,y+h-winCY);
    }
    // non-Ajax
    else
    {
        var h=elt.offsetHeight, winCY=_winHeight(), y=_getPos(elt).y

        if (y<scrY) window.scrollTo(scrX, y)
        else if (y+h>scrY+winCY) window.scrollTo(scrX, Math.max(y,y+h-winCY))
    }
}

function doNothing() {
};

function promptengine_anchorOnKeyPress(e) {
	var evt = e ? e : window.event;
	var target = evt.srcElement ? evt.srcElement : evt.target;

	if(evt.keyCode == 13 && target.onclick) {
		target.onclick.apply(target,[e]);
	}
	
	return true;	
}

function promptengine_encodeUTF8(string) {
    var arr = [];
    var strLen = string.length;
    for(var i = 0; i < strLen; i++) {
        var c = string.charCodeAt(i);
        if(c < 0x80) {
            arr.push(c);
        }
        else if(c < 0x0800) {
            arr.push((c >> 6) | 0xc0);
            arr.push(c & 0x3f | 0x80);
        }
        else if(c < 0xd800 || c >= 0xe000) {
            arr.push((c >> 12) | 0xe0);
            arr.push((c >> 6) & 0x3f | 0x80);
            arr.push(c & 0x3f | 0x80);
        }
        else if(c < 0xdc00) {
            var c2 = string.charCodeAt(i + 1);
            if(isNaN(c2) || c2 < 0xdc00 || c2 >= 0xe000) {
                arr.push(0xef, 0xbf, 0xbd);
                continue;
            }
            i++;
            val = ((c & 0x3ff) << 10) | (c2 & 0x3ff);
            val += 0x10000;
            arr.push((val >> 18) | 0xf0);
            arr.push((val >> 12) & 0x3f | 0x80);
            arr.push((val >> 6) & 0x3f | 0x80);
            arr.push(val & 0x3f | 0x80);
        }
        else {
            arr.push(0xef, 0xbf, 0xbd);
        }
    }
    return arr;
}

function promptengine_encodeBASE64(byteArray) {
    var keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    var arr = [];
    var c1, c2, c3, e1, e2, e3, e4;
    var i = 0, arrLen = byteArray.length;
    
    while(i < arrLen) {
        c1 = byteArray[i++];
        c2 = byteArray[i++];
        c3 = byteArray[i++];
        
        e1 = c1 >> 2;
        e2 = ((c1 & 3) << 4) | (c2 >> 4);
        e3 = ((c2 & 15) << 2) | (c3 >> 6);
        e4 = c3 & 63;
        
        if (isNaN(c2)) {
            e3 = e4 = 64;
        } else if(isNaN(c3)) {
            e4 = 64;
        }
        arr.push(keyStr.charAt(e1));
        arr.push(keyStr.charAt(e2));
        arr.push(keyStr.charAt(e3));
        arr.push(keyStr.charAt(e4));
    }
    return arr.join('');
}

function promptengine_encodeValueField(value)
{
    return promptengine_encodePrompt(promptengine_encodeBASE64(promptengine_encodeUTF8(value)));
}

 /* Crystal Decisions Confidential Proprietary Information */

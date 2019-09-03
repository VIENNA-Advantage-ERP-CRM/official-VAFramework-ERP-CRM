// <script>

_userAgent=navigator.userAgent?navigator.userAgent.toLowerCase():null
_ie=(document.all!=null)?true:false
_dom=(document.getElementById!=null)?true:false
_isQuirksMode = (document.compatMode != 'CSS1Compat');
_dtd4=!_ie||(document.compatMode!='BackCompat') // Browser is compatible HTML 4.0 Strict
_moz=_dom&&!_ie
_show='visible'
_hide='hidden'
_hand=_ie?"hand":"pointer"
_appVer=navigator.appVersion.toLowerCase();
_webKit=(_userAgent.indexOf("safari")>=0)||(_userAgent.indexOf("applewebkit")>=0)
_mac=(_appVer.indexOf('macintosh')>=0)||(_appVer.indexOf('macos')>=0);
_opera = (_userAgent.indexOf('opera') != -1);     // Browser is opera
_userAgent=navigator.userAgent?navigator.userAgent.toLowerCase():null
_saf=_moz&&(_userAgent.indexOf("safari")>=0)
_ctrl=0
_shift=1
_alt=2
//_def_shortcut=_mac?"&#8984;":

var docMode = document.documentMode;
var _ie6Up=(docMode>=6)
var _ie8Up=(docMode>=8)
var _ie10Up=(docMode>=10)
var _ie11Up=(docMode>=11)
_small=(screen.height<=600)
_curDoc=document
_curWin=self
_tooltipWin=self
_tooltipDx=0
_tooltipDy=0
_codeWinName="_CW"
_leftBtn=(_ie||_saf)?1:0
_preloadArr=new Array
_widgets=new Array
_resizeW=_ie6Up?"col-resize":"E-resize"
_resizeH=_ie6Up?"row-resize":"S-resize"
_ddData=new Array
_dontNeedEncoding=null;
_thex=null;

_defaultButtonWidth=60;

// ================================================================================
// ================================================================================
//
// MANDATORY FUNCTIONS
//
// Must be called for the library to work
//
// IMPORTANT : must be used in a script tag in the doc header (not in loadCB for example)
// ================================================================================
// ================================================================================

function initDom(skin,style,lang,curWin,codeUniqueName)
// MUST be called after including dom.js
// skin [String] skin path
// lang [String] language code
// curWin [Window - Optional] pointer on a window to redirect the interface
// tooltipWin [Window - Optional] pointer on a window that displays drag & drop tooltips
// codeUniqueName [String - Optional] must be set if curWin specfied - enter a unique keyword
{
	_skin=skin;
	_lang=lang;
	_style=style;
	if (curWin)
	{
		_curWin=curWin
		_curDoc=curWin.document
	}

	_tooltipWin=_curWin

	if (codeUniqueName)
		_codeWinName="_CW"+codeUniqueName

	_curWin[_codeWinName]=self
}

function styleSheet()
// MUST be called after initDom()
// must be between scripts tags to avois sync problems
// Include the default style sheet depending on the parameters of initDom()
{
	includeCSS('style')
	//we use only one css file for all languages
	//switch(_lang)
	//{
	//	case 'ja':
	//	case 'ko':
	//	case 'zh':
	//		includeCSS('style_fe')
	//		break;
	//	default:
	//		includeCSS('style')
	//		break;
	//}
}

// ================================================================================

function isLayerDisplayed(lyr)
//check if the layer is displayed 
//and check its parents
//if one parent is not displayed return false
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
				return isLayerDisplayed(par)
			else
				return true	
		}			
	}
	else
		return true;
}

// ================================================================================

function safeSetFocus(lyr)
{
if (lyr && lyr.focus && isLayerDisplayed(lyr))
	lyr.focus()
		
}

// ================================================================================
// ================================================================================
//
// OBJECT newWidget (Constructor)
//
// Base Class for widgets
//
// ================================================================================
// ================================================================================

function newWidget(id)
// CONSTRUCTOR
// id [String]   the id for DHTML processing
{
	var o=new Object
	o.id=id
	o.layer=null
	o.css=null
	o.getHTML=Widget_getHTML
	o.beginHTML=Widget_getHTML
	o.endHTML=Widget_getHTML
	o.write=Widget_write
	o.begin=Widget_begin
	o.end=Widget_end
	o.init=Widget_init
	o.move=Widget_move
	o.resize=Widget_resize
	o.setBgColor=Widget_setBgColor
	o.show=Widget_show
	o.getWidth=Widget_getWidth
	o.getHeight=Widget_getHeight
	o.setHTML=Widget_setHTML
	o.setDisabled=Widget_setDisabled
	o.focus=Widget_focus
	o.setDisplay=Widget_setDisplay
	o.isDisplayed=Widget_isDisplayed
	o.appendHTML=Widget_appendHTML
	o.setTooltip=Widget_setTooltip
	o.initialized=Widget_initialized

	o.widx=_widgets.length
	_widgets[o.widx]=o
	return o
}

function new_Widget(prm)
// CONSTRUCTOR - DEPRECATED (use new_Widget instead)
// id [String]   the id for DHTML processing
{
    return newWidget(prm.id)
}


function getEvent(e,w)
{
    if (_ie&&(e==null))
        e = w ? w.event : _curWin.event

    return e
}

function Widget_param(paramsObj, paramName, paramDefaultValue)
{
    var val = paramsObj ? paramsObj[paramName] : null;
	
    return val == null ? paramDefaultValue : val;
}

// ================================================================================

function Widget_appendHTML()
// Write the widget dynamically into the body
{
	append(_curDoc.body,this.getHTML())
}

// ================================================================================

function Widget_getHTML()
// VIRTUAL Get the widget HTML
// return [String] the HTML
{
	return ''
}

// ================================================================================

function Widget_write(i)
// write the widget
{
	_curDoc.write(this.getHTML(i))
}

// ================================================================================

function Widget_begin()
// for container widgets - write the HTML begin part
{
	_curDoc.write(this.beginHTML())
}

// ================================================================================

function Widget_end()
// for container widgets - write the HTML begin part
{
	_curDoc.write(this.endHTML())
}

// ================================================================================

function Widget_init()
// Inits the widget layer pointers
{
	var o=this
	o.layer=getLayer(o.id)
	o.css=o.layer.style
	o.layer._widget=o.widx

	if (o.initialHTML)
		o.setHTML(o.initialHTML)
}

function Widget_move(x,y)
// moves a widget
// x [int optional] new abscissa
// y [int optional] new ordinate
{
	var c=this.css;if (x!=null){if (_moz) c.left=""+x+"px";else c.pixelLeft=x}if (y!=null){if (_moz) c.top=""+y+"px";else c.pixelTop=y}
}

// ================================================================================

function Widget_focus()
// Set the focus on the current widget
{
	safeSetFocus(this.layer)
}

// ================================================================================

function Widget_setBgColor(c)
// Set the widget background color
// c [String] HTML color code
{
	this.css.backgroundColor=c
}

// ================================================================================

function Widget_show(show)
// Show/hide a widget (the widget layout always exists)
// show [boolean]
{
	this.css.visibility=show?_show:_hide
}

// ================================================================================

function Widget_getWidth()
// return [int] the widget width in pixels
{
	return this.layer.offsetWidth
}

// ================================================================================

function Widget_getHeight()
// return [int] the widget height in pixels
{
	return this.layer.offsetHeight
}

// ================================================================================

function Widget_setHTML(s)
// Set the widget HTML
// s [String] the new HTML
{
	var o=this
	if (o.layer)
		o.layer.innerHTML=s
	else
		o.initialHTML=s
}

// ================================================================================

function Widget_setDisplay(d)
// Show/hide a widget (the widget takes no space if d is false)
// d [boolean]
{
	if (this.css)
		this.css.display=d?"":"none"
}

// ================================================================================

function Widget_isDisplayed()
{
	if(this.css.display == "none")
		return false
	else	
		return true		
}


// ================================================================================

function Widget_setDisabled(d)
// Disable a widget (its becomes insensitive to key or mouse actions)
// d [boolean]
{
	if (this.layer)
		this.layer.disabled=d
}

// ================================================================================

function Widget_resize(w,h)
// Resize a widget
// w [int optional] new width
// h [int optional] new height
{
	if (w!=null) this.css.width=''+(Math.max(0,w))+'px';if (h!=null) this.css.height=''+(Math.max(0,h))+'px';
}

// ================================================================================

function Widget_setTooltip(tooltip)
// Set the widget tooltip
// tooltip [String]
{
	this.layer.title=tooltip
}

// ================================================================================

function Widget_initialized()
// Return true if the widget is initialized
{
	return this.layer!=null
}

// ================================================================================
// ================================================================================
//
// OBJECT newGrabberWidget (Constructor)
//
// Class to add grabber for resizing zones
//
// ================================================================================
// ================================================================================

function newGrabberWidget(id,resizeCB,x,y,w,h,isHori,buttonCB,tooltip)
// CONSTRUCTOR
// id       [String]   the id for DHTML processing
// resizeCB [Function] callback when grabber is moved
// x        [int]      initial abscissa
// y        [int]      initial ordinate
// w        [int]      initial width
// h        [int]      initial height
// isHori   [boolean]  if true, moves horizontally, else vertically
{
	o=newWidget(id)
	o.resizeCB=resizeCB
	o.x=x
	o.y=y
	o.w=w
	o.h=h
	o.dx=0
	o.dy=0
	o.min=null
	o.max=null
	o.isHori=isHori
	o.preloaded=new Image
	o.preloaded.src=_skin+"../resizepattern.gif"
	o.buttonCB=buttonCB
	o.allowGrab=true
	o.collapsed=false
	o.isFromButton=false
	o.showGrab=GrabberWidget_showGrab
	o.setCollapsed=GrabberWidget_setCollapsed
	o.tooltipButton=tooltip

	o.getHTML=GrabberWidget_getHTML
	o.enableGrab=GrabberWidget_enableGrab
	o.setMinMax=GrabberWidget_setMinMax

	if (window._allGrabbers==null)
		window._allGrabbers=new Array

	o.index=_allGrabbers.length
	_allGrabbers[o.index]=o
	o.buttonLyr=null
	o.setButtonImage=GrabberWidget_setButtonImage
	o.getImgOffset=GrabberWidget_getImgOffset

	return o
}

// ================================================================================

function GrabberWidget_setCollapsed(collapsed,tooltip)
{
	this.collapsed=collapsed
	this.setButtonImage(false,tooltip)
}

// ================================================================================

function GrabberWidget_getImgOffset(isRollover)
{
	// PRIVATE
	var o=this

	if (o.isHori)
	{
		o.dx=(o.collapsed?12:0)+(isRollover?6:0)
		o.dy=0
	}
	else
	{
		o.dy=(o.collapsed?12:0)+(isRollover?6:0)
		o.dx=0
	}
}

// ================================================================================

function GrabberWidget_setButtonImage(isRollover,tooltip)
{
	// PRIVATE
	var o=this
	
	o.getImgOffset(isRollover)
	o.tooltipButton=tooltip
	
	if (o.layer)
	{
		if (o.buttonLyr==null)
			o.buttonLyr=getLayer("grabImg_"+o.id)
		if (o.buttonLyr)
		{
			changeSimpleOffset(o.buttonLyr,o.dx,o.dy,null,tooltip)
		}
	}
}

// ================================================================================

function GrabberWidget_enableGrab(bEnable)
{
	var o=this
	o.allowGrab=bEnable
	
	if (o.css)
		o.css.cursor=o.allowGrab?(o.isHori?_resizeW:_resizeH):"default"
}

// ================================================================================

function GrabberWidget_getHTML()
// returns [String] widget HTML
{
    var o=this
    var cr=o.isHori?_resizeW:_resizeH
    var moveableCb='onselectstart="return false" ondragstart="return false" onmousedown="'+_codeWinName+'.GrabberWidget_down(event,\''+o.index+'\',this);return false;"'
    var imgG=_ie?('<img onselectstart="return false" ondragstart="return false" onmousedown="'+_codeWinName+'.eventCancelBubble(event)" border="0" hspace="0" vspace="0" src="'+_skin+'../transp.gif" id="modal_'+o.id+'" style="z-index:10000;display:none;position:absolute;top:0px;left:0px;width:1px;height:1px;cursor:'+cr+'">'):('<div onselectstart="return false" ondragstart="return false" onmousedown="'+_codeWinName+'.eventCancelBubble(event)" border="0" hspace="0" vspace="0" id="modal_'+o.id+'" style="z-index:10000;display:none;position:absolute;top:0px;left:0px;width:1px;height:1px;cursor:'+cr+'"></div>')

    return getBGIframe('grabIframe_'+o.id)+imgG+'<table cellpadding="0" cellspacing="0" border="0" '+moveableCb+' id="'+o.id+'" style="overflow:hidden;position:absolute;left:'+o.x+'px;top:'+o.y+'px;width:'+o.w+'px;height:'+o.h+'px;cursor:'+cr+'"><tr><td></td></tr></table>'
}


// ================================================================================

function GrabberWidget_setMinMax(min,max)
// Set the range in pixels
// min [int] minimum value
// min [int] maximum value
{
	this.min=min
	this.max=max
}

// ================================================================================

function GrabberWidget_button(e,index,lyr)
// PRIVATE
{
	var o=_allGrabbers[index]
	o.isFromButton=true
	lyr.onmouseup=eval('_curWin.'+_codeWinName+'.GrabberWidget_buttonup')
}

// ================================================================================

function GrabberWidget_buttonover(e,index,lyr)
// PRIVATE
{
	var o=_allGrabbers[index]
	o.setButtonImage(true)
}

// ================================================================================

function GrabberWidget_buttonout(e,index,lyr)
// PRIVATE
{
	var o=_allGrabbers[index]
	o.setButtonImage(false)
}

// ================================================================================

function GrabberWidget_buttonup(e)
// PRIVATE
{
	GrabberWidget_up(e)
}

// ================================================================================

function GrabberWidget_showGrab()
{
	var o=this,mod=o.mod,ifr=o.iframe,stl=o.layer.style,st=mod.style
	ifr.setDisplay(true)
}

// ================================================================================

function GrabberWidget_down(e,index,lyr)
// PRIVATE
{
	var o=_allGrabbers[index]
	window._theGrabber=o

	if (o.mod==null)
	{
		o.mod=getLayer('modal_'+o.id)
		o.iframe=newWidget('grabIframe_'+o.id)
		o.iframe.init()
	}
	o.mod.onmousemove=eval('_curWin.'+_codeWinName+'.GrabberWidget_move')
	o.mod.onmouseup=eval('_curWin.'+_codeWinName+'.GrabberWidget_up')

	o.grabStartPosx=parseInt(lyr.style.left)
	o.grabStartPosy=parseInt(lyr.style.top)
	o.grabStartx=eventGetX(e)
	o.grabStarty=eventGetY(e)
	
	var mod=o.mod,ifr=o.iframe,stl=o.layer.style,st=mod.style
	
	stl.backgroundImage='url(\''+_skin+'../resizepattern.gif\')'
	o.prevZ=stl.zIndex
	stl.zIndex=9999
	ifr.css.zIndex=9998

	st.width='100%'
	st.height='100%'
	mod.style.display="block"

	var p=getPos(o.layer)
	ifr.move(p.x,p.y)
	ifr.resize(o.getWidth(),o.getHeight())
	
	if (!o.isFromButton)
		o.showGrab()

	return false
}

// ================================================================================

function GrabberWidget_move(e)
// PRIVATE
{
	var o=_theGrabber,lyr=o.layer,mod=o.mod

	if (o.isFromButton)
	{
		if (o.isHori)
		{
			var x = eventGetX(e), ox=o.grabStartx
			if ((x < ox - 3) || (x > ox + 3))
				o.isFromButton = false
		}
		else
		{
			var Y = eventGetY(e), oy=o.grabStarty
			if ((y < oy - 3) || (y > oy + 3))
				o.isFromButton = false
		}
		if (!o.isFromButton)
			o.showGrab()
	}
	
	
	if (!o.isFromButton)
	{	
		if (o.allowGrab)
		{
			var x=o.isHori?Math.max(0,o.grabStartPosx-o.grabStartx+eventGetX(e)):null
			var y=o.isHori?null:Math.max(0,o.grabStartPosy-o.grabStarty+eventGetY(e))

			if (o.isHori)
			{
				if (o.min!=null) x=Math.max(x,o.min)
				if (o.max!=null) x=Math.min(x,o.max)
			}
			else
			{
				if (o.min!=null) y=Math.max(y,o.min)
				if (o.max!=null) y=Math.min(y,o.max)
			}

			eventCancelBubble(e)
			o.move(x,y)
			getPos(o.layer)

			if (o.buttonCB)
			{
				var bCss=o.buttonLyr.style
				if (bCss.display!="none")
					bCss.display="none"
			}
			
			o.iframe.move(x,y)
		}
	}
}

// ================================================================================

function GrabberWidget_up(e)
{
	var o=_theGrabber,lyr=o.layer,mod=o.mod,stl=lyr.style
	stl.backgroundImage=''
	stl.zIndex=o.prevZ

	var ifr=o.iframe
	ifr.move(-100,-100)
	ifr.resize(1,1)
	ifr.setDisplay(false)
	eventCancelBubble(e)

	var st=mod.style
	st.display="none"
	st.width='0px'
	st.height='0px'
	
	if (o.buttonCB)
		o.buttonLyr.style.display=""

	if (o&&(o.isFromButton))
	{
		if (o.buttonCB)
			o.buttonCB()
		o.isFromButton=false
	}

	if (o.allowGrab&&(!o.isFromButton))
	{
		if (o.resizeCB)
			o.resizeCB(parseInt(lyr.style.left),parseInt(lyr.style.top))
	}
}

// ================================================================================
// ================================================================================
//
// OBJECT ButtonWidget (Constructor)
//
// Class for building push buttons
//
// ================================================================================
// ================================================================================

function newButtonWidget(id,label,cb,width,hlp,tooltip,tabIndex,margin,url,w,h,dx,dy,imgRight,disDx,disDy)
// CONSTRUCTOR
// id       [String] button ID
// label    [String Optional] button label
// w        [int optional] : text zone minimal width
// hlp      OBSOLETE
// tooltip  [String Optional] button tooltip
// tabIndex [int Optional] for 508/IE : tab index for keyboard navigation
// url      [String Optional] : the combined image url
// w        [int Optional] the visible image part width
// dx       [int Optional] the horizontal offset in image
// dy       [int Optional] the vertical offset in image
// disDx    : [int - optional]      horizontal offset for disabled state
// disDy    : [int - optional]      vertical offset for disabled state
{
	var o=newWidget(id)
	o.label=label
	o.cb=cb
	o.width=width
	o.hlp=hlp
	o.tooltip=tooltip
	o.tabIndex=tabIndex
	o.isGray=false
	o.isDefault=false
	o.txt=null
	o.icn=null
	o.margin=margin?margin:0
	o.extraStyle=""
 	o.isDelayCallback = true;

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

	o.getHTML=ButtonWidget_getHTML
	o.setDisabled=ButtonWidget_setDisabled	
	o.setText=ButtonWidget_setText
	o.changeImg=ButtonWidget_changeImg
	o.oldInit=o.init
	o.init=ButtonWidget_init
	o.isDisabled=ButtonWidget_isDisabled
	o.setDefaultButton=ButtonWidget_setDefaultButton
	o.executeCB=ButtonWidget_executeCB
	o.setTooltip=ButtonWidget_setTooltip	
	o.setDelayCallback=ButtonWidget_setDelayCallback
	

	o.instIndex=ButtonWidget_currInst
	ButtonWidget_inst[ButtonWidget_currInst++]=o
	return o;
}

// ================================================================================

ButtonWidget_inst=new Array
ButtonWidget_currInst=0

function ButtonWidget_getHTML()
// returns [String] widget HTML
{
	with (this)
	{
		var clk=_codeWinName+'.ButtonWidget_clickCB('+this.instIndex+');return false;"'
		
		var clcbs= 'onclick="'+clk+'" '
		if (_ie)  clcbs+= 'ondblclick="'+clk+'" '
		
		//
		var isDefaultSty=(this.isDefault && !this.isGray);

		//add onkeydown for dialog default action (Enter)
		clcbs+= 'onkeydown=" return '+_codeWinName+'.ButtonWidget_keydownCB(event,'+this.instIndex+');" '

		var url1=_skin+"button.gif",addPar=' style="'+extraStyle+'cursor:'+_hand+';margin-left:'+margin+'px; margin-right:'+margin+'px; "'+clcbs+' ',tip=attr('title', tooltip),idText="theBttn"+id, idIcon="theBttnIcon"+id;
		var bg=backImgOffset(url1,0,isDefaultSty?105:42);
		
		var lnkB='<a '+attr('id',idText)+' '+tip+' '+attr('tabindex',tabIndex)+' href="javascript:void(0)" class="wizbutton" role="button">'
		var l=(label!=null)
		var im=(this.url?('<td align="'+(l?(this.imgRight?'right':'left'):'center')+'" style="'+bg+'" width="'+(!l&&(width!=null)?width+6:w+6)+'">'+(l?'':lnkB)+simpleImgOffset(url,w,h,this.isGray?disDs:dx,this.isGray?disDy:dy,idIcon,null,(l?'':tooltip),'cursor:'+_hand)+(l?'':'</a>')+'</td>'):'')

        // Justin... I've change the button left and right to use backImgOffset because IE would not hide the simpleImgOffset once it had been displayed in the cancel dialog.
		return '<table onmouseover="return true" '+attr('id',id)+' '+addPar+' border="0" cellspacing="0" cellpadding="0"><tr valign="middle">'+
			'<td height="21" width="5" style="' + backImgOffset(url1,0,isDefaultSty?63:0)+'"></td>'+
			(this.imgRight?'':im)+
			(l?('<td '+attr("width",width)+ attr('id',"theBttnCenterImg"+id)+' align="center" class="'+(this.isGray?'wizbuttongray':'wizbutton')+'" style="padding-left:3px;padding-right:3px;'+bg+'"><nobr>'+lnkB+label+'</a></nobr></td>'):'')+
			(this.imgRight?im:'')+
			'<td height="21" width="5" style="' + backImgOffset(url1,0,isDefaultSty?84:21)+'"></td></tr></table>';
	}
}

function ButtonWidget_setDelayCallback(value) {
	
	this.isDelayCallback = (value == true);
}
// ================================================================================

function ButtonWidget_setDisabled(d)
// Enable/Disable
// s [boolean] if true disable the button
// returns [void]
{
	var o=this,newCur=d?'default':_hand
	o.isGray=d
	if (o.layer)
	{
		var newClassName=d?'wizbuttongray':'wizbutton'
	
		// Ensure the button state hasn't changed to avoid unnecessary processing
		// The text className is a safe way to do the test
		if (o.txt.className!=newClassName)
		{
			o.txt.className=newClassName
			o.txt.style.cursor=newCur
			o.css.cursor=newCur
			
			if(o.icn)
			{
				changeSimpleOffset(o.icn,o.isGray?o.disDx:o.dx,o.isGray?o.disDy:o.dy)
				o.icn.style.cursor=newCur
			}
					
			if(o.isDefault) //default button style
			{
				var isDefaultSty=!d,url=_skin+"button.gif";	
				changeSimpleOffset(o.leftImg,0,isDefaultSty?63:0,url);
				changeOffset(o.centerImg,0,isDefaultSty?105:42,url);
				changeSimpleOffset(o.rightImg,0,isDefaultSty?84:21,url);
			}
		}		
	}
}


// ================================================================================
function ButtonWidget_setDefaultButton()
{
	var o=this;
	
	if (o.layer)
	{
		var isDefaultSty = !o.isGray,url=_skin+"button.gif";		
		changeSimpleOffset(o.leftImg,0,isDefaultSty?63:0,url);
		changeOffset(o.centerImg,0,isDefaultSty?105:42,url);
		changeSimpleOffset(o.rightImg,0,isDefaultSty?84:21,url);		
	}	
	o.isDefault=true;	
}
// ================================================================================

function ButtonWidget_isDisabled()
{
	return this.isGray
}

// ================================================================================

function ButtonWidget_setText(str)
// Changes the button text
// str [String] the new button text
// returns [void]
{
	this.txt.innerHTML=convStr(str)
}

function ButtonWidget_setTooltip(tooltip)
// Set the widget tooltip
// tooltip [String]
{
	var o=this
	o.tooltip=tooltip
	o.layer.title=tooltip
	if (o.txt)
		o.txt.title=tooltip
	if (o.icn)	
		o.icn.title=tooltip	
}

// ================================================================================

function ButtonWidget_init()
// Inits the button
// returns [void]
{
	var o=this
	o.oldInit()
	o.txt=getLayer('theBttn'+this.id)
	o.icn=getLayer('theBttnIcon'+this.id)
	
	o.leftImg=getLayer('theBttnLeftImg'+this.id)
	o.centerImg=getLayer('theBttnCenterImg'+this.id)
	o.rightImg=getLayer('theBttnRightImg'+this.id)
	
	// reset if already initialized
	var newClassName=o.isGray?'wizbuttongray':'wizbutton'
	if (o.txt.className!=newClassName)
	{
		o.setDisabled(o.isGray)
	}	
}

// ================================================================================
function ButtonWidget_changeImg(dx,dy,disDx,disDy,url,tooltip)
{
	var o=this
	if (url) o.url=url
	if (dx!=null) o.dx=dx
	if (dy!=null) o.dy=dy
	if (disDx!=null) o.disDx=disDx
	if (disDy!=null) o.disDy=disDy
	if (tooltip!=null) o.tooltip=tooltip
	
	if (o.icn)
		changeSimpleOffset(o.icn,o.isGray?o.disDx:o.dx,o.isGray?o.disDy:o.dy, o.url, o.tooltip)
}

// ================================================================================

function ButtonWidget_clickCB(index)
// PRIVATE internal click event handler
{
	var btn=ButtonWidget_inst[index]
	if (btn && !btn.isGray) {
		if(btn.isDelayCallback)
			setTimeout("ButtonWidget_delayClickCB("+index+")",1);
		else
			ButtonWidget_delayClickCB(index);
	}
		
}

// ================================================================================

function ButtonWidget_delayClickCB(index)
// PRIVATE internal click event handler
{
	var btn=ButtonWidget_inst[index]
	btn.executeCB();
	/*
	if (btn.cb)
	{
		if (typeof btn.cb!="string")
			btn.cb()
		else
			eval(btn.cb)
	}*/	
}
// ================================================================================

function ButtonWidget_executeCB()
// PRIVATE internal click event handler
{
	var o=this
	if (o.cb)
	{
		if (typeof o.cb!="string")
			o.cb()
		else
			eval(o.cb)
	}
}
// ================================================================================

function ButtonWidget_keydownCB(e,index)
{
	var k=eventGetKey(e);
	var btn=ButtonWidget_inst[index]
	if(k == 13 && btn.cb )//enter
	{
		eventCancelBubble(e);	
	}
	return true;
}

// ================================================================================
// ================================================================================
//
// OBJECT ScrolledZoneWidget (Constructor)
//
// Base Class for widgets
//
// ================================================================================
// ================================================================================

function newScrolledZoneWidget(id,borderW,padding,w,h,bgClass)
// id      [String] the id for DHTML processing
// borderW [int] widget border width
// padding [int] widget padding
// w       [int] widget width, including borders
// h       [int] widget height, including borders
// Return  [ScrolledZoneWidget] the instance
{
	var o=newWidget(id)
	o.borderW=borderW
	o.padding=padding
	o.w=w
	o.h=h
	o.oldResize=o.resize
	o.beginHTML=ScrolledZoneWidget_beginHTML
	o.endHTML=ScrolledZoneWidget_endHTML
	o.resize=ScrolledZoneWidget_resize
	o.bgClass = (bgClass)? bgClass : 'insetBorder'
	return o;
}

// ================================================================================

function ScrolledZoneWidget_beginHTML()
// Returns : [String] the HTML beginning block
{
	var w=this.w,h=this.h;
	var ofs=_moz?2*(this.borderW+this.padding):0
	
	if (typeof(w)=="number")
	{
		if (_moz)
			w=Math.max(0,w-ofs)
		w=""+w+"px"
	}
	
	if (typeof(h)=="number")
	{
		if (_moz)
			h=Math.max(0,h-ofs)
		h=""+h+"px"
	}
	
	
	return '<div tabindex=-1 align="left" class="' + this.bgClass + '" id="'+this.id+'" style="border-width:'+this.borderW+'px;padding:'+this.padding+'px;'+sty("width",w)+sty("height",h)+'overflow:auto">'
}

// ================================================================================

function ScrolledZoneWidget_endHTML()
// Returns : [String] the HTML ending block
{
	return '</div>'
}

// ================================================================================

function ScrolledZoneWidget_resize(w,h)
// w       [int]  widget width, including borders
// h       [int]  widget height, including borders
// Returns [void]
{
	if (_moz)
	{
		var ofs=2*(this.borderW+this.padding)
		if (w!=null) w=Math.max(0,w-ofs)
		if (h!=null) h=Math.max(0,h-ofs)
	}
	this.oldResize(w,h)
}

// ================================================================================
// ================================================================================
//
// OBJECT newTooltipWidget (Constructor)
//
// A tooltip widget. Only one by window/frame
//
// ================================================================================
// ================================================================================
/*
function newTooltipWidget(maxw,maxh)
// Return [TooltipWidget] the instance
{
	if (window._theGlobalTooltip!=null)
		return window._theGlobalTooltip

	var o=newWidget('theGobalTooltip')
	o.maxw=maxw
	o.maxh=maxh
	o.getPrivateHTML=TooltipWidget_getPrivateHTML
	o.init=TooltipWidget_init
	o.oldShow=o.show
	o.show=TooltipWidget_show
	o.hide=TooltipWidget_hide
	o.setPos=TooltipWidget_setPos
	o.inputs=null
	window._theGlobalTooltip=o
	o.eventWin=_curWin

	preloadImg(_skin+'../swap.gif')
	return o;
}

// ================================================================================

function TooltipWidget_init()
{
	//cancels the default init behaviour
}

// ================================================================================

function TooltipWidget_getPrivateHTML()
// Returns [String] the HTML
{
	var o=this
	return getBGIframe('tipIframe_'+o.id)+'<div class="dragTooltip" id="'+o.id+'" style="visibility:hidden;z-index:10000;position:absolute;top:0px;left:0px,visibility:'+_hide+'"'+attr("width",o.w)+attr("height",o.h)+'></div>'+
		'<img width="11" height="11" border="0" hspace="0" vspace="0" src="'+_skin+'../swap.gif" id="swap_'+o.id+'" style="position:absolute;top:0px;left:0px;display:none;z-index:10000;">'
}

// ================================================================================

function TooltipWidget_show(show,str,url,w,h,dx,dy,isHTML, e)
// show/hide the drag & drop tooltip
// show [boolean] true for showing the tooltip
// str  [String] the tooltip text
// url [String] combined image url
{
	var o=this

	oldWin=_curWin
	_curWin=_tooltipWin
	_curDoc=_tooltipWin.document

	// object not init yet, 2 cases
	if (o.layer==null)
	{
		o.layer=getLayer(o.id)
		// another instance hasn't written it's HTML yet
		if (o.layer==null)
		{
			targetApp(o.getPrivateHTML())
			o.layer=getLayer(o.id)
		}
		o.css=o.layer.style
		o.swapLayer=getLayer("swap_"+o.id)
		o.iframe=newWidget('tipIframe_'+o.id)
		o.iframe.init()
	}
	// hides combo boxes or restore
	//if (show) o.inputs=hideAllInputs()
	//else restoreAllInputs(o.inputs)

	dx=dx!=null?dx:0
	dy=dy!=null?dy:0

	if (show)
	{
		var s=null

		if (url)
			//s='<span style="margin-right:2px;width:'+w+'px;height:'+h+'px;overflow:hidden">'+img(url,null,null,'top','style="position:relative;top:-'+dy+'px;left:-'+dx+'px"')+'</span>'
			s=simpleImgOffset(url,w,h,dx,dy,null,null,null,"margin-right:4px;margin-left:0px;",'top')
			
		//o.resize(0,0)

		o.css.width=''
		o.css.height=''
		
		o.setHTML('<table border="0" cellspacing="0" cellpadding="0"><tr valign="middle">'+(s?'<td align="center">'+s+'</td>':'')+'<td class="dragTxt"><nobr>'+(isHTML?str:convStr(str))+'</nobr></td></tr></table>')
		o.setPos(null,e)
		
		if (o.getWidth()>o.maxw)
			o.resize(o.maxw)
			
		o.oldShow(show)
		o.iframe.setDisplay(true)
	}
	else
	{
		o.oldShow(false)
		o.setHTML('')
		o.move(0,0)
		o.swapLayer.style.display="none"
		o.iframe.setDisplay(false)
	}

	_curWin=oldWin
	_curDoc=_curWin.document
}

// ================================================================================

function TooltipWidget_hide()
{
	var o=this
	
	o.show(false)
}

// ================================================================================

function TooltipWidget_setPos(shift,e)
// Return [void]
{	
	var o=this
	if (o.layer==null)
		return

	if (e) {
		var x=absxpos(e), y=absypos(e)
	} else {
		var ew=o.eventWin,x=ew.event.x+_curDoc.body.scrollLeft,y=ew.event.y+_curDoc.body.scrollTop
	}

	x+=_tooltipDx
	y+=_tooltipDy

	o.move(x+27,y+10);
	
	o.iframe.move(x+27,y+10);
	o.iframe.resize(o.getWidth(),o.getHeight())

	var c=o.swapLayer.style;

	c.display=shift?"":"none"
	if (shift)
	{
		y+=18
		x+=14
		if (_moz)
		{
			c.left=""+x+"px"
			c.top=""+y+"px"
		}
		else
		{
			c.pixelLeft=x
			c.pixelTop=y
		}
	}
}


// ================================================================================

function initTooltipWin(tooltipWin)
{
	_tooltipWin=tooltipWin
}

// ================================================================================

function setTooltipOffset(dx,dy)
{
	_tooltipDx=dx
	_tooltipDy=dy
}
*/

// ================================================================================
// ================================================================================
//
// OBJECT newComboWidget (Constructor)
//
// Combo box widget - based on SELECT tags
//
// ================================================================================
// ================================================================================

function newComboWidget(id,changeCB,noMargin,width,tooltip)
// id       [String]   the id for DHTML processing
// changeCB [Function] calback when selection is changed by the user
// noMargin [boolean - optional] default is false - no margin (else 10px left margin )
// width    [int - optional] combo width (if not, auto width)
// tooltip  [String - optional] combo tooltip for 508
// Return   [ComboWidget] the widget instance
{
	var o=newWidget(id)
	o.tooltip=tooltip
	o.size=1
	o.getHTML=ComboWidget_getHTML
	o.beginHTML=ComboWidget_beginHTML
	o.endHTML=ComboWidget_endHTML
	o.changeCB=changeCB
	o.noMargin=noMargin
	o.width=width==null?null:''+width+'px'
	o.add=ComboWidget_add
	o.del=ComboWidget_del
	o.getSelection=ComboWidget_getSelection
	o.select=ComboWidget_select
	o.valueSelect=ComboWidget_valueSelect
	o.getCount=ComboWidget_getCount
	o.oldSetDisabled=o.setDisabled
	o.setDisabled=ComboWidget_setDisabled
	o.setUndefined=ComboWidget_setUndefined
	o.delByID=ComboWidget_delByID
//	o.move=ComboWidget_move
	o.findByValue=ComboWidget_findByValue
	o.findByText=ComboWidget_findByText
	o.getValue=ComboWidget_getValue
	o.isGrayed=ComboWidget_isGrayed
	o.clearSelection=ComboWidget_clearSelection;

	o.isDisabled=false
	o.multi=false
	o.undef=false
	o.isCombo=true

	o.undefId=o.id+"__undef"
	o.disabledId=o.id+"__disabled"
	return o;
}

// ================================================================================

_extrCmbS=(_moz?'font-size:12px;':'')

function ComboWidget_beginHTML()
{
	var o=this,_extrCmbS=((_moz&&!o.isCombo)?'font-size:12px;':'')

	return '<select '+(o.multi?'multiple':'')+' '+(o.noMargin?'style="'+sty("width",o.width)+_extrCmbS+'"':'style="'+sty("width",o.width)+'margin-left:10px;'+_extrCmbS+'"')+' class="listinputs" '+attr('onchange',_codeWinName+'.ComboWidget_changeCB(event,this)')+attr('onclick',_codeWinName+'.ComboWidget_clickCB(event,this)')+attr('ondblclick',_codeWinName+'.ComboWidget_dblClickCB(event,this)')+attr('onkeyup',_codeWinName+'.ComboWidget_keyUpCB(event,this)')+attr('onkeydown',_codeWinName+'.ComboWidget_keyDownCB(event,this)')+attr('id',o.id)+attr('name',o.id)+attr('title',o.tooltip)+'size="'+o.size+'">'
}

// ================================================================================

function ComboWidget_clearSelection()
{
    var o=this;
    if(o.layer)
        o.layer.selectedIndex=-1;
}

//================================================================================

function ComboWidget_endHTML()
{
	return '</select>'
}

// ================================================================================

function ComboWidget_getHTML(inner)
{
	return this.beginHTML()+(inner?inner:'')+this.endHTML()
}

// ================================================================================

function ComboWidget_add(s,val,sel,id,grayed)
{
	var e=this.layer,opt=_curDoc.createElement('option');

	// Add item
	if (_ie)
		e.options.add(opt);
	else
		e.appendChild(opt);

	// Set text content
	if (opt.innerText!=null)
		opt.innerText=s;
	else
		opt.innerHTML=convStr(s);
		
	// Set other attributes
	opt.value=val;
	if(id!=null)
		opt.id=id;
	if (sel)
		opt.selected=true;
	
	if (grayed) 
	{
		opt.style.color='gray'
	}
	
	return opt;
}

// ================================================================================

//function ComboWidget_move(delta)
//{
//    var e=this.layer,i=e.selectedIndex,len=e.options.length-1,newI=i+delta
//    if ((i==-1)||(newI<0)||(newI>len))
//        return false
//
//    var oldOpt = e.options[i],newOpt = e.options[newI]
//    var oldVal=oldOpt.value,oldHTML=oldOpt.innerHTML,oldID=oldOpt.id,newID=newOpt.id,oldColor=oldOpt.style.color,newColor=newOpt.style.color
//
//    oldOpt.value=newOpt.value
//    oldOpt.innerHTML=newOpt.innerHTML
//    newOpt.id=null
//    oldOpt.id=newOpt.id
//	oldOpt.style.color=newColor
//
//    newOpt.value=oldVal
//    newOpt.innerHTML=oldHTML
//    newOpt.id=oldID
//	newOpt.style.color=oldColor
//
//    e.selectedIndex=newI
//    return true
//}

// ================================================================================

function ComboWidget_getSelection()
{
	var e=this.layer,i=e.selectedIndex;if (i<0) return null;var ret=new Object;ret.index=i;ret.value=e.options[i].value;ret.text=e.options[i].text;return ret
}

// ================================================================================

function ComboWidget_select(i)
{
	var o=this,e=o.layer,len=e.options.length
	if (i==null) e.selectedIndex=-1
	if ((i<0)||(i>=len))
		i=len-1
	if (i>=0)
		e.selectedIndex=i

	o.setUndefined(false)
}

// ================================================================================

function ComboWidget_valueSelect(v)
{
	var o=this,e=o.layer,opts=e.options,len=opts.length
	for (var i=0;i<len;i++)
	{
		if (opts[i].value==v)
		{
			//e.selectedIndex=i
		    opts[i].selected=true
			o.setUndefined(false)
			break;
		}
	}
}

// ================================================================================

function ComboWidget_del(i)
{
	var e=this.layer
	if (i==null)
		e.options.length=0
	else
	{
		if (_ie) e.remove(i)
		else e.options[i]=null
		this.select(i)
	}
}

// ================================================================================

function ComboWidget_changeCB(e,l)
// PRIVATE
{
	var o=getWidget(l);if(o.changeCB) o.changeCB(e)
}

// ================================================================================
function ComboWidget_clickCB(e,l)
// PRIVATE
{
	var o=getWidget(l);if(o.clickCB) o.clickCB(e)
}
// ================================================================================
function ComboWidget_dblClickCB(e,l)
// PRIVATE
{
	var o=getWidget(l);if(o.dblClickCB) o.dblClickCB(e)
}
// ================================================================================
function ComboWidget_keyUpCB(e,l)
{
	var o=getWidget(l);if(o.keyUpCB) o.keyUpCB(e)
}
// ================================================================================
function ComboWidget_keyDownCB(e,l)
{
	var k=eventGetKey(e);
	var o=getWidget(l);
	//be careful ! usefull for dialog box close by Enter ou Escape keypressed
	if(o.isCombo && (k==27 || k==13))//Escape ou Enter
	{
		eventCancelBubble(e);
	}
	else if(k==13 && o.keyUpCB) //Enter can be attached to an action in listwidget
	{				
		eventCancelBubble(e);		
	}	
}
// ================================================================================

function ComboWidget_getCount()
{
	return this.layer.options.length
}

// ================================================================================

function ComboWidget_delByID(id)
// PRIVATE
{
	var opt=getLayer(id)
	if (opt!=null)
		this.del(opt.index)
	opt=null
}

// ================================================================================

function ComboWidget_setDisabled(d,addEmptyElt)
{
	var o=this
	o.oldSetDisabled(d);
	o.isDisabled=d;

	if (d==true)
	{
		var old=getLayer(o.disabledId)
		if (old==null)
			o.add('','',true,o.disabledId);
		else
			o.layer.selectedIndex=old.index
	}
	else
		o.delByID(o.disabledId)
}

// ================================================================================

function ComboWidget_setUndefined(u)
{
	var o=this
	o.undef=u;

	if (u==true)
	{
		var old=getLayer(o.undefId)
		if (old==null)
			o.add('','',true,o.undefId);
		else
			o.layer.selectedIndex=old.index
	}
	else
		o.delByID(o.undefId)
}

// ================================================================================
function ComboWidget_findByValue(val)
//return the data if already exist else null
{
	var o=this,e=o.layer,opts=e.options,len=opts.length
	for (var i=0;i<len;i++)
	{
		if (opts[i].value==val)
		{
			var ret=new Object;
			ret.index=i;
			ret.value=e.options[i].value;
			ret.text=e.options[i].text;
			return ret
		}
	}
	return null
}

// ================================================================================
function ComboWidget_findByText(txt)
//return the data if already exist else null
{
	var o=this,e=o.layer,opts=e.options,len=opts.length
	for (var i=0;i<len;i++)
	{
		if (opts[i].text==txt)
		{
			var ret=new Object;
			ret.index=i;
			ret.value=e.options[i].value;
			ret.text=e.options[i].text;
			return ret
		}
	}
	return null
}

// ================================================================================
function ComboWidget_getValue(i)
//return the value at index i
{
	var o=this,e=o.layer,opts=e.options,len=opts.length
	if(i==null || i<0 || i>len)  return null;
	
	var ret=new Object;
	ret.index=i;
	ret.value=e.options[i].value;
	return ret				
}

// ================================================================================
function ComboWidget_isGrayed(i)
//return true is the option is gray
{
	var o=this,e=o.layer,opts=e.options,len=opts.length
	if(i==null || i<0 || i>len)  return false;

	return (e.options[i].style.color=="gray")			
}

// ================================================================================
// ================================================================================
//
// OBJECT newListWidget (Constructor)
//
// List box widget - based on SELECT tags
//
// ================================================================================
// ================================================================================

function newListWidget(id,changeCB,multi,width,lines,tooltip,dblClickCB,keyUpCB,clickCB)
{
	var o=newComboWidget(id,changeCB,true,width,tooltip)
	o.clickCB=clickCB
	o.dblClickCB=dblClickCB	
	o.keyUpCB=keyUpCB
	o.size=lines
	o.multi=multi
	o.getMultiSelection=ListWidget_getMultiSelection
	o.setUndefined=ListWidget_setUndefined
	o.isUndefined=ListWidget_isUndefined
	o.change=ListWidget_change
	o.isCombo=false
	return o;
}

// ================================================================================

function ListWidget_setUndefined(u)
{
	var o=this
	o.undef=u;

	if (u==true)
	{
		o.layer.selectedIndex = -1
	}
}

// ================================================================================

function ListWidget_isUndefined()
{
	return (this.layer.selectedIndex == -1)
}

// ================================================================================

function ListWidget_getMultiSelection()
{
	var e=this.layer,rets=new Array,len=e.options.length
	for (var i=0;i<len;i++)
	{
		var opt=e.options[i]
		if (opt.selected)
		{
			var ret=new Object;
			ret.index=i;ret.value=opt.value;ret.text=opt.text;rets[rets.length]=ret
		}
	}
	return rets
}

function ListWidget_change(multi,lines)
{	
	var o=this
	if(multi!=null)
	{	
		o.multi=multi
		o.layer.multiple=multi
	}	
	if(lines!=null)
	{
		o.size=lines	
		o.layer.size=lines
	}
}
// ================================================================================
// ================================================================================
//
// OBJECT newInfoWidget (Constructor)
//
// List box widget - based on SELECT tags
//
// ================================================================================
// ================================================================================

function newInfoWidget(id,title,boldTitle,text,height)
{
	var o=newWidget(id)
	o.title=title?title:""
	o.boldTitle=boldTitle?boldTitle:""
	o.text=text?text:""
	o.height=(height!=null)?height:55

	o.getHTML=InfoWidget_getHTML
	o.setText=InfoWidget_setText	
	o.setTitle=InfoWidget_setTitle
	o.setTitleBold=InfoWidget_setTitleBold
	o.oldResize=o.resize
	o.resize=InfoWidget_resize

	o.textLayer=null
	return o
}

// ================================================================================

function InfoWidget_setText(text,isHTML)
{
	var o=this
	text=text?text:""
	o.text=text

	if (o.layer)
	{
		var l=o.textLayer
		if (l==null)
			l=o.textLayer=getLayer('infozone_'+o.id)

		if (l) l.innerHTML=isHTML?text:convStr(text,false,true)
	}
}

// ================================================================================

function InfoWidget_setTitle(text)
{
	var o=this
	text=text?text:""
	o.title=text
	
	if (o.layer)
	{
		var l=o.titleLayer
		if (l==null)
			l=o.titleLayer=getLayer('infotitle_'+o.id)
		if (l) l.innerHTML=convStr(text)
	}
}

// ================================================================================

function InfoWidget_setTitleBold(text)
{
	var o=this
	text=text?text:""
	o.boldTitle=text
	
	if (o.layer)
	{
		var l=o.titleLayerBold
		if (l==null)
			l=o.titleLayerBold=getLayer('infotitlebold_'+o.id)
		if (l) l.innerHTML=convStr(text)
	}
}

// ================================================================================

function InfoWidget_getHTML()
{
	var o=this
	return '<div class="dialogzone" align="left" style="overflow:hidden;'+sty("width",o.width)+sty("height",""+o.height+"px")+'" id="'+o.id+'">'+
		'<nobr>'+img(_skin+'../help.gif',16,16,'top',null,_helpLab)+
		'<span class="dialogzone" style="padding-left:5px" id="infotitle_'+o.id+'">'+convStr(o.title)+'</span><span style="padding-left:5px" class="dialogzonebold" id="infotitlebold_'+o.id+'">'+convStr(o.boldTitle)+'</span></nobr>'+
		'<br>'+getSpace(1,2)+		
		'<div class="infozone" align="left" id="infozone_'+o.id+'" style="height:'+(o.height-18-(_moz?10:0))+'px;overflow'+(_ie?'-y':'')+':auto">'+convStr(o.text,false,true)+'</div>'+
	'</div>'
}

// ================================================================================

function InfoWidget_resize(w,h)
{
	var o=this;
	if (w!=null) o.w=w
	if (h!=null) o.h=h
	o.oldResize(w,h)

	if (o.layer)
	{
		var l=o.textLayer
		if (l==null)
			l=o.textLayer=getLayer('infozone_'+o.id)
		if (l)
		{
			if (o.h!=null) l.style.height=""+Math.max(0, o.h-(_ie?18:28))+"px"
		}
	}
}


// ================================================================================
// ================================================================================
//
// OBJECT newCheckWidget (Constructor)
//
// Check box widget
//
// ================================================================================
// ================================================================================

function newCheckWidget(id,text,changeCB,bold,imgUrl,imgW,imgH,bconvtext)
{
	var o=newWidget(id)
	o.text=text
	o.convText=bconvtext
	o.changeCB=changeCB
	o.idCheckbox='check_'+id
	o.checkbox=null
	o.kind='checkbox'
	o.name=o.idCheckbox
	o.bold=bold
	o.imgUrl=imgUrl
	o.imgW=imgW
	o.imgH=imgH
	o.getHTML=CheckWidget_getHTML
	o.setText=CheckWidget_setText
	o.parentInit=Widget_init
	o.init=CheckWidget_init
	o.check=CheckWidget_check
	o.isChecked=CheckWidget_isChecked
	o.setDisabled=CheckWidget_setDisabled
	o.isDisabled=CheckWidget_isDisabled
	o.uncheckOthers=CheckWidget_uncheckOthers
	o.isIndeterminate=CheckWidget_isIndeterminate
	o.setIndeterminate=CheckWidget_setIndeterminate
	o.layerClass=('dialogzone'+(o.bold?'bold':''))
	o.nobr=true
	return o
}

// ================================================================================

function CheckWidget_getHTML()
{
	var o=this,cls=o.layerClass;
	return '<table border="0" onselectstart="return false" cellspacing="0" cellpadding="0" class="'+cls+'"'+attr('id',o.id)+'>' +
	'<tr valign="middle">'+
		'<td style="height:20px;width:21px"><input style="margin:'+(_moz?3:0)+'px" onclick="'+_codeWinName+'.CheckWidget_changeCB(event,this)" ' +
		'type="'+o.kind+'"'+attr('id',o.idCheckbox)+attr('name',o.name)+'>' +
		'</td>'+
		(o.imgUrl?'<td><label style="padding-left:2px" for="'+o.idCheckbox+'">'+img(o.imgUrl,o.imgW,o.imgH)+'</label></td>':'')+
		'<td>'+(o.nobr?'<nobr>':'')+'<label style="padding-left:'+ (o.imgUrl?4:2) +'px" id="label_'+o.id+'" for="'+o.idCheckbox+'">'+(o.convText?convStr(o.text):o.text)+'</label>'+(o.nobr?'</nobr>':'')+'</td>'+
	'</tr></table>'
}

// ================================================================================

function CheckWidget_setText(s)
// Set the check text
// s [String] the new text
{
	var o=this
	o.text=s
	
	if (o.layer)
	{
		if (o.labelLyr==null)
			o.labelLyr=getLayer('label_'+o.id)
		o.labelLyr.innerHTML=o.convText?convStr(s):s
	}
}

function CheckWidget_init()
{
	this.parentInit()
	this.checkbox=getLayer(this.idCheckbox)
}

// ================================================================================

function CheckWidget_check(c) {this.checkbox.checked=c;if(c)this.uncheckOthers()}
function CheckWidget_isChecked() {return this.checkbox.checked}
function CheckWidget_changeCB(e,l) {var o=getWidget(l);o.uncheckOthers();if(o.changeCB) o.changeCB(e)}
function CheckWidget_setDisabled(d) {this.checkbox.disabled=d;if(_moz) this.checkbox.className=(d?'dialogzone':'')}
function CheckWidget_isDisabled(){ return this.checkbox.disabled }
function CheckWidget_uncheckOthers() {}
function CheckWidget_isIndeterminate() {return this.checkbox.indeterminate}
function CheckWidget_setIndeterminate(b) {this.checkbox.indeterminate=b}

// ================================================================================
// ================================================================================
//
// OBJECT newRadioWidget (Constructor)
//
// Class for radio buttons
//
// ================================================================================
// ================================================================================

function newRadioWidget(id,group,text,changeCB,bold,imgUrl,imgW,imgH,bconvtext)
{
	var o=newCheckWidget(id,text,changeCB,bold,imgUrl,imgW,imgH,bconvtext)
	o.kind='radio'
	o.name=group
	if (_RadioWidget_groups[group]==null)
		_RadioWidget_groups[group]=new Array
	o.groupInstance=_RadioWidget_groups[group]
	var g=o.groupInstance
	o.groupIdx=g.length
	g[g.length]=o
	o.uncheckOthers=RadioWidget_uncheckOthers
	return o;
}

// ================================================================================

var _RadioWidget_groups=new Array
function RadioWidget_uncheckOthers()
{
	var g=this.groupInstance,idx=this.groupIdx,len=g.length
	for (var i=0;i<len;i++)
	{
		if (i!=idx)
		{
			var c=g[i].checkbox
			if(c)
				c.checked=false
		}
	}
}

// ================================================================================
// ================================================================================
//
// OBJECT newIconNoTextCheckWidget (Constructor)
//
// Class for icon only checkbox
//
// ================================================================================
// ================================================================================
/*
function newIconNoTextCheckWidget(id,changeCB,imgUrl,imgW,imgH,tooltip)
{

	var o=newWidget(id)
	o.changeCB=changeCB
	o.idCheckbox='check_'+id
	o.checkbox=null
	o.kind='checkbox'
	o.name=o.idCheckbox
	o.imgUrl=imgUrl
	o.imgW=imgW
	o.imgH=imgH
	o.tooltip=(tooltip?tooltip:"")
	o.getHTML=IconNoTextCheckWidget_getHTML
	o.parentInit=Widget_init
	o.init=CheckWidget_init
	o.check=CheckWidget_check
	o.isChecked=CheckWidget_isChecked
	o.setDisabled=CheckWidget_setDisabled
	o.uncheckOthers=CheckWidget_uncheckOthers
	return o
}

// ================================================================================

function IconNoTextCheckWidget_getHTML()
{
	var o=this
	return '<table border="0" onselectstart="return false" cellspacing="0" cellpadding="0" title= "'+ convStr(o.tooltip) + '" ' + attr('id',o.id) +'>' +
	'<tr>'+
		'<td align="center"><label style="padding-left:2px" for="'+o.idCheckbox+'">'+img(o.imgUrl,o.imgW,o.imgH, null, null, convStr(o.tooltip))+'</label></td>'+
	'</tr>'+
	'<tr>'+
		'<td align="center"><input style="margin:'+(_moz?3:0)+'px" onclick="'+_codeWinName+'.CheckWidget_changeCB(event,this)" ' +
		'type="'+o.kind+'"'+attr('id',o.idCheckbox)+attr('name',o.name)+'>' +
		'</td>'+
	'</tr></table>'
}

// ================================================================================
// ================================================================================
//
// OBJECT newIconNoTextCheckWidget (Constructor)
//
// Class for icon only radio button
//
// ================================================================================
// ================================================================================

function newIconNoTextRadioWidget(id,group,changeCB,imgUrl,imgW,imgH,tooltip)
{
	var tip = tooltip?tooltip:""
	var o=newIconNoTextCheckWidget(id,changeCB,imgUrl,imgW,imgH,tip)
	o.kind='radio'
	o.name=group
	if (_RadioWidget_groups[group]==null)
		_RadioWidget_groups[group]=new Array
	o.groupInstance=_RadioWidget_groups[group]
	var g=o.groupInstance	
	o.groupIdx=g.length
	g[g.length]=o
	o.uncheckOthers=RadioWidget_uncheckOthers

	return o;
}
*/
// ================================================================================
// ================================================================================
//
// OBJECT newTextFieldWidget (Constructor)
//
// Text field widget
//
// ================================================================================
// ================================================================================

function newTextFieldWidget(id,changeCB,maxChar,keyUpCB,enterCB,noMargin,tooltip,width,focusCB,blurCB)
{
	var o=newWidget(id)
	o.tooltip=tooltip
	o.changeCB=changeCB
	o.maxChar=maxChar
	o.keyUpCB=keyUpCB
	o.enterCB=enterCB
	o.noMargin=noMargin
	o.width=width==null?null:''+width+'px'
	o.focusCB=focusCB
	o.blurCB=blurCB
	o.disabled=false
	o.getHTML=TextFieldWidget_getHTML
	o.getValue=TextFieldWidget_getValue
	o.setValue=TextFieldWidget_setValue
	o.intValue=TextFieldWidget_intValue
	o.intPosValue=TextFieldWidget_intPosValue
	o.select=TextFieldWidget_select
	o.setDisabled=TextFieldWidget_setDisabled
	o.beforeChange=null
	o.wInit=o.init
	o.init=TextFieldWidget_init
	o.oldValue=""
	o.helpTxt=''
	o.isHelpTxt=false
	o.setHelpTxt=TextFieldWidget_setHelpTxt
	o.eraseHelpTxt=TextFieldWidget_eraseHelpTxt
	o.enterCancelBubble=true
	return o
}

// ================================================================================

function TextFieldWidget_setDisabled(d)
{
	var o=this
	o.disabled=d
	
	if (o.layer)
		o.layer.disabled=d
}

// ================================================================================

function TextFieldWidget_init()
{
	var o=this
	o.wInit()	
	o.layer.value=""+ (o.oldValue != "")?o.oldValue : "";
	
	if(o.helpTxt && !o.oldValue)	
		o.setHelpTxt(o.helpTxt);	
}

// ================================================================================

function TextFieldWidget_getHTML()
{
	var o=this	
	return '<input'+(o.disabled?' disabled':'')+' oncontextmenu="event.cancelBubble=true;return true" style="'+sty("width",this.width)+(_moz?'margin-top:1px;margin-bottom:1px;padding-left:5px;padding-right:2px;':'')+(_isQuirksMode? 'height:20px;' : 'height:16px;') + 'margin-left:'+(this.noMargin?0:10)+'px" onfocus="'+_codeWinName+'.TextFieldWidget_focus(this)" onblur="'+_codeWinName+'.TextFieldWidget_blur(this)" onchange="'+_codeWinName+'.TextFieldWidget_changeCB(event,this)" onkeydown=" return '+_codeWinName+'.TextFieldWidget_keyDownCB(event,this);" onkeyup=" return '+_codeWinName+'.TextFieldWidget_keyUpCB(event,this);" onkeypress=" return '+_codeWinName+'.TextFieldWidget_keyPressCB(event,this);" type="text" '+attr('maxLength',this.maxChar)+' ondragstart="event.cancelBubble=true;return true" onselectstart="event.cancelBubble=true;return true" class="textinputs" id="'+this.id+'" name="'+this.id+'"'+attr('title',this.tooltip)+' value="">'
		
}

// ================================================================================

function TextFieldWidget_getValue()
{
	var o=this
	if (o.isHelpTxt) { 
		return ''
	}
	else
	{
		return o.layer ? o.layer.value : o.oldValue
	}	
}

// ================================================================================

function TextFieldWidget_setValue(s)
{
	var o=this
	if (o.layer) {
		o.eraseHelpTxt()		
		o.layer.value=''+s
	} else {
		o.oldValue=s
	}	
}

// ================================================================================

function TextFieldWidget_changeCB(e,l)
// PRIVATE
{
	var o=getWidget(l)
	o.eraseHelpTxt()
	if(o.beforeChange)
		o.beforeChange()
	if(o.changeCB)
		o.changeCB(e)
}

// ================================================================================

function TextFieldWidget_keyPressCB(e,l)
// PRIVATE
{
	var o=getWidget(l)

	if (eventGetKey(e)==13)
	{
            // Only non-IME enter key press will come here
	    o.enterKeyPressed = true;
            return false;
	}
	else
	{
	    o.enterKeyPressed = false;
	}
	return true
}

// ================================================================================

function TextFieldWidget_keyUpCB(e,l)
// PRIVATE
{
	var o=getWidget(l)
	o.eraseHelpTxt()
	if (eventGetKey(e)==13 && o.enterKeyPressed) // Ignore the IME enter key up
	{
		if (o.beforeChange)
			o.beforeChange()

		if (o.enterCB)
		{
			if (o.enterCancelBubble)
				eventCancelBubble(e)

			o.enterCB(e)
		}
				
		return false
	}
	else if(o.keyUpCB)
	{
		o.keyUpCB(e)	
	}
	o.enterKeyPressed = false;
	return true
}

// ================================================================================

function TextFieldWidget_keyDownCB(e,l)
// PRIVATE
{
	var o=getWidget(l)
	o.eraseHelpTxt()
	o.enterKeyPressed = false;
	if (eventGetKey(e)==13)
	{
		return true
	}
	else if (eventGetKey(e)==8)//back space
	{
		eventCancelBubble(e);
	}
	return true;	
}

// ================================================================================

function TextFieldWidget_eraseHelpTxt()
// PRIVATE
{
	var o=this
	if (o.isHelpTxt) o.layer.value= ""
	o.isHelpTxt = false
	o.layer.style.color="black"	
}

// ================================================================================

function TextFieldWidget_focus(l)
// PRIVATE
{
	var o=getWidget(l)
	o.eraseHelpTxt()
	if (o.focusCB)
		o.focusCB()
}

// ================================================================================

function TextFieldWidget_blur(l)
// PRIVATE
{
	var o=getWidget(l)

	if(o.beforeChange)
		o.beforeChange()

	if (o.blurCB)
		o.blurCB()
}

// ================================================================================

function TextFieldWidget_intValue(nanValue)
{
	var n=parseInt(this.getValue())
	return isNaN(n)?nanValue:n
}

// ================================================================================

function TextFieldWidget_intPosValue(nanValue)
{
	var n=this.intValue(nanValue)
	return (n<0)?nanValue:n
}

// ================================================================================

function TextFieldWidget_select()
{
	this.layer.select()
}

// ================================================================================
function TextFieldWidget_setHelpTxt(h)
{
	var o=this
	o.helpTxt=h		
	if (o.layer && (o.layer.value == "")) 
	{
		o.isHelpTxt=true
		o.layer.value=h
		o.layer.style.color="#808080"		
	}
}

// ================================================================================
// ================================================================================
//
// OBJECT newIntFieldWidget (Constructor)
//
// Text field widget that handles integer numeric values
//
// ================================================================================
// ================================================================================

function newIntFieldWidget(id,changeCB,maxChar,keyUpCB,enterCB,noMargin,tooltip,width,customCheckCB)
{
	var o=newTextFieldWidget(id,changeCB,maxChar,keyUpCB,enterCB,noMargin,tooltip,width)
	o.min=-Number.MAX_VALUE
	o.max=Number.MAX_VALUE
	o.customCheckCB=customCheckCB // Returns a boolean
	o.setMin=IntFieldWidget_setMin
	o.setMax=IntFieldWidget_setMax
	o.setValue=IntFieldWidget_setValue
	o.beforeChange=IntFieldWidget_checkChangeCB
	o.value=''
	return o
}

// ================================================================================

function IntFieldWidget_setMin(min)
{
	if (!isNaN(min))
		this.min=min
}

// ================================================================================

function IntFieldWidget_setMax(max)
{
	if (!isNaN(max))
		this.max=max
}

// ================================================================================

function IntFieldWidget_setValue(s)
{
	var o=this,l=o.layer
	s = '' + s
	if (s =='')
	{
		if (l)
			l.value= ''
		o.oldValue = ''
		return
	}
	
	var n=parseInt(s)
	value = ''
	if (!isNaN(n) && (n >= o.min) && (n <= o.max) && ((o.customCheckCB==null) || o.customCheckCB(n))) {
		 value = n
		 o.oldValue = value			
	} else {
		if (o.oldValue)
			value = o.oldValue
	}
	if (l)
		l.value= '' + value
}

// ================================================================================

function IntFieldWidget_checkChangeCB()
{
	var o=this
	o.setValue(o.layer.value)
}

// ================================================================================
// ================================================================================
//
// OBJECT newFloatFieldWidget (Constructor)
//
// Text field widget that handles floating point numeric values
//
// ================================================================================
// ================================================================================
/*
function newFloatFieldWidget(id,changeCB,maxChar,keyUpCB,enterCB,noMargin,tooltip,width,customCheckCB)
{
	var o=newIntFieldWidget(id,changeCB,maxChar,keyUpCB,enterCB,noMargin,tooltip,width)
	o.setValue=FloatFieldWidget_setValue
	o.customCheckCB=customCheckCB // Returns a boolean
	o.setPrecision=FloatFieldWidget_setPrecision
	o.toPrecision=FloatFieldWidget_toPrecision
	o.setSeparator=FloatFieldWidget_setSeparator
	o.beforeChange=FloatFieldWidget_checkChangeCB
	o.precision=10 // default precision !!is it enough?
	o.sep='.' // default separator

	return o
}

// ================================================================================

function FloatFieldWidget_setValue(s)
{
	var o=this,l=o.layer
	s = '' + s
	if (s =='')
	{
		if (l)
			l.value= ''
		o.oldValue = ''
		return
	}

	var n=parseFloat(s)
	value = ''
	if (!isNaN(n) && (n >= o.min) && (n <= o.max) && ((o.customCheckCB==null) || o.customCheckCB(n))) {
		value = '' + o.toPrecision(n)
		o.oldValue = value		
	} else {
		if (o.oldValue)
			value = o.oldValue
	}
	if (l)
		l.value = value	
}

// ================================================================================

function FloatFieldWidget_toPrecision(n)
{
	var o=this
	n = '' + n
	var nAr = n.split(o.sep)
	if (nAr.length == 1) return nAr[0]
	var dec = (nAr[1].length >= o.precision)? nAr[1].substr(0, o.precision) : nAr[1]
	return nAr[0] + o.sep + dec
}

// ================================================================================

function FloatFieldWidget_checkChangeCB()
{
	var o=this
	o.setValue(o.layer.value)
}

// ================================================================================

function FloatFieldWidget_setPrecision(p)
{
	this.precision=p
}

// ================================================================================

function FloatFieldWidget_setSeparator(s)
{
	this.sep=s
}

// ================================================================================
// ================================================================================
//
// OBJECT newTextAreaWidget (Constructor)
//
// Text Area widget
//
// ================================================================================
// ================================================================================

function newTextAreaWidget(id,rows,cols,tooltip,changeCB,enterCB,cancelCB)
{
	var o=newWidget(id)	
	o.rows=rows
	o.cols=cols
	o.allowCR=true
	o.tooltip=tooltip	
	o.changeCB=changeCB
	o.enterCB=enterCB
	o.cancelCB=cancelCB
	o.getHTML=TextAreaWidget_getHTML
	o.getValue=TextAreaWidget_getValue
	o.setValue=TextAreaWidget_setValue
	o.resize=TextAreaWidget_resize
	o.wInit=o.init
	o.init=TextAreaWidget_init
	o.oldValue=""
	
	
	if ((o.rows!=null)&&!_ie&&!_saf)
		o.rows--

	return o
}

// ================================================================================

function TextAreaWidget_init()
{
	var o=this
	o.wInit()
	o.layer.value=""+o.oldValue
}

// ================================================================================

function TextAreaWidget_getHTML()
{
	return '<textarea oncontextmenu="event.cancelBubble=true;return true" id="'+this.id+'" '+attr('title',this.tooltip)+ 'rows="'+this.rows+'" cols="'+this.cols+'" class="textinputs" value="" onkeydown="return '+_codeWinName+'.TextAreaWidget_keyDownCB(event,this)" ondragstart="event.cancelBubble=true;return true" onselectstart="event.cancelBubble=true;return true" ></textarea>'	
}

// ================================================================================

function TextAreaWidget_getValue()
{
	return this.layer.value
}

// ================================================================================

function TextAreaWidget_setValue(s)
{
	if (this.layer)
		this.layer.value=''+s
	else
		this.oldValue=s
}

// ================================================================================

function TextAreaWidget_resize(lines,cols)
{
	var o=this
	
	if(lines && lines >0) o.layer.rows=lines
	if(cols && cols>0) o.layer.cols=cols
}

// ================================================================================

function TextAreaWidget_keyDownCB(e,l)
{
	var key = eventGetKey(e),o=getWidget(l)

	if (key==13)//enter
	{		
		if (o.enterCB)
		{
			eventCancelBubble(e)
			o.enterCB(e)
		}
		else if (o.allowCR)
		{
			eventCancelBubble(e);
			//change in textarea
			setTimeout("TextAreaWidget_delayedChangeCB("+key+","+o.widx+")",1)
		}

		return o.allowCR;
	}
	else if(key==27)//escape
	{
		if(o.cancelCB) 
			return o.cancelCB(e)
		else 
			return true;
	}
	else if(key == 8)// back space
	{
			eventCancelBubble(e);
			//change in textarea
			setTimeout("TextAreaWidget_delayedChangeCB("+key+","+o.widx+")",1)
			
			return true;
	}
	else 
	{
		//setTimeout to be sure that the key is writen in the textarea
		//we do not use the keyup event because of rapid keyboard pression
		setTimeout("TextAreaWidget_delayedChangeCB("+key+","+o.widx+")",1)

		return true;
	}
}

// ================================================================================

function TextAreaWidget_delayedChangeCB(key,widx)
{
	var o=_widgets[widx]
	if (o.changeCB)
		o.changeCB(key)
}
*/
// ================================================================================
// ================================================================================
//
// OBJECT newFrameZoneWidget (Constructor)
//
// Text field widget that handles floating point numeric values
//
// ================================================================================
// ================================================================================

function newFrameZoneWidget(id,w,h,reverse)
{
	var o=newWidget(id)
	o.w=(w!=null)?""+Math.max(0,w-10)+"px":null
	o.h=(h!=null)?""+Math.max(0,h-10)+"px":null
	o.reverse=(reverse!=null)?reverse:false
	o.cont=null
	o.beginHTML=FrameZoneWidget_beginHTML
	o.endHTML=FrameZoneWidget_endHTML
	o.oldResize=o.resize
	o.resize=FrameZoneWidget_resize
	return o
}

// ================================================================================

function FrameZoneWidget_resize(w,h)
{
	var o=this
	var d=o.layer.display!="none"

	if (d&_moz&&!_saf)
		o.setDisplay(false)
	o.oldResize(w,h)
	if (d&_moz&&!_saf)
		o.setDisplay(true)
}

// ================================================================================

function FrameZoneWidget_beginHTML()
{
    var o=this
    
    return '<table width="100%" style="'+sty("width",o.w)+sty("height",o.h)+'" id="'+o.id+'" cellspacing="0" cellpadding="4" border="0"><tbody>'+
        '<tr><td valign="top" class="dlgFrame" id="frame_cont_'+o.id+'" style="padding:5px">'
        
}

// ================================================================================

function FrameZoneWidget_endHTML()
{
    var o=this
    return '</td></tr></tbody></table>'
}

// ================================================================================
// ================================================================================
//
// OBJECT newBOColor (Constructor)
//
// Color object
//
// ================================================================================
// ================================================================================
/*
function newBOColor(r,g,b)
{
	var o=new Object;
	if (r && (g==null) && (b==null))
	{
		s = r.split(",")
		o.r=parseInt(s[0])
		o.g=parseInt(s[1])
		o.b=parseInt(s[2])
	}
	else
	{
		o.r=r
		o.g=g
		o.b=b
	}

	o.set=BOColor_set
	o.getCopy=BOColor_getCopy
	o.getStringDef=BOColor_getStringDef
	o.getStyleColor=BOColor_getStyleColor
	return o;
}

// ================================================================================

function BOColor_getStringDef()
{
	var o=this
	return ""+o.r+","+o.g+","+o.b
}

// ================================================================================

function BOColor_getCopy()
{
	var f=this
	var o=newBOColor(f.r,f.g,f.b)
	return o;
}

// ================================================================================

function BOColor_set(r,g,b)
{
	this.r=r
	this.g=g
	this.b=b
}

// ================================================================================

function BOColor_getStyleColor()
{
	var o=this
	// if undefined
	if (o.r == -1 || o.r == null) return null;
	if (o.g == -1 || o.g == null) return null;
	if (o.b == -1 || o.b == null) return null;
	return "rgb(" + o.r + "," + o.g + "," + o.b + ")"
}


// ================================================================================
// ================================================================================
//
// PRIVATE OBJECT newDragDropData (Constructor)
//
// Generic object fo Drag and drop support.
//
// ================================================================================
// ================================================================================


function newDragDropData(widget,dragStartCB,dragCB,dragEndCB,acceptDropCB,leaveDropCB,dropCB)
// void    dragStartCB  (source,layer)
// void    dragCB       (source,layer, shift)
// void    dragEndCB    (source,layer)
// boolean acceptDropCB (source, target, ctrl, shift, layer, enter)
// void    leaveDropCB  (source, target, ctrl, shift, layer)
// void    dropCB       (source, target, ctrl, shift, layer)
{
	var o=new Object
	o.widget=widget
	o.dragStartCB=dragStartCB
	o.dragCB=dragCB
	o.dragEndCB=dragEndCB
	o.acceptDropCB=acceptDropCB
	o.leaveDropCB=leaveDropCB
	o.dropCB=dropCB
	o.attachCallbacks=DragDropData_attachCallbacks

	o.id=_ddData.length
	_ddData[o.id]=o

	return o
}

// ================================================================================

function DragDropData_attachCallbacks(lyr,onlyDrop)
{
	if (_ie)
	{
		onlyDrop=(onlyDrop==null)?false:onlyDrop

		if (!onlyDrop)
		{
			lyr.ondragstart=DDD_dragStart
			lyr.ondragend=DDD_dragEnd
		}
		lyr.ondrop=DDD_drop
		lyr.ondragleave=DDD_dragLeave
		lyr.ondragover=DDD_dragOver
		lyr.ondrag=DDD_drag
		lyr._dragDropData=this.id
	}
}

// ================================================================================

function DDD_dragStart()
{
	var e=_curWin.event,dt=e.dataTransfer
	dt.effectAllowed='copyMove'
	dt.dropEffect=_curWin.event.ctrlKey?'copy':'move'

	var o=_ddData[this._dragDropData]
	o.dragStartCB(o.widget,this)

	window._globalDDD=o.widget
	e.cancelBubble=true
}

// ================================================================================

function DDD_drag()
{
	var e=_curWin.event,dt=e.dataTransfer
	dt.dropEffect=e.ctrlKey?'copy':'move'

	var o=_ddData[this._dragDropData]
	o.dragCB(o.widget,this,e.ctrlKey?false:e.shiftKey)

	e.cancelBubble=true
}

// ================================================================================

function DDD_dragEnd()
{
	var o=_ddData[this._dragDropData]
	o.dragEndCB(o.widget,this)

	window._globalDDD=null
}

// ================================================================================

function DDD_dragEnter()
{
	DDD_dragOverEnter(this,true)
}

// ================================================================================

function DDD_dragOver()
{
	DDD_dragOverEnter(this,false)
}

// ================================================================================

function DDD_dragOverEnter(layer,isEnter)
{
	var o=_ddData[layer._dragDropData],e=_curWin.event
	e.dataTransfer.dropEffect=e.ctrlKey?'copy':'move'
	var o=_ddData[layer._dragDropData];

	if (o.acceptDropCB(window._globalDDD,o.widget,e.ctrlKey,e.ctrlKey?false:e.shiftKey,layer,isEnter))
		e.returnValue=false

	e.cancelBubble=true
}

// ================================================================================

function DDD_dragLeave()
{
	var o=_ddData[this._dragDropData],e=_curWin.event

	o.leaveDropCB(window._globalDDD,o.widget,e.ctrlKey,e.ctrlKey?false:e.shiftKey,this)
}

// ================================================================================

function DDD_drop()
{
	var o=_ddData[this._dragDropData],e=_curWin.event
	o.dropCB(window._globalDDD,o.widget,e.ctrlKey,e.ctrlKey?false:e.shiftKey,this)
	window._globalDDD=null
	e.cancelBubble=true
}
*/
// ================================================================================
// ================================================================================
//
// GLOBAL FUNCTIONS : ARRAYS
//
// Dynamic array management
//
// ================================================================================
// ================================================================================

function arrayAdd(obj,fieldName,item,idx)
{
	var array=obj[fieldName],len=array.length
	if ((idx==null)||(typeof idx!="number")) idx=-1
	if ((idx<0)||(idx>len)) idx=len
	if  (idx!=len)
	{
		var end=array.slice(idx)
		array.length=idx+1
		array[idx]=item
		array=array.concat(end)
	}
	else array[idx]=item
	obj[fieldName]=array
	return idx
}

// ================================================================================

function arrayRemove(obj,fieldName,idx)
{
	var array=obj[fieldName],last=array.length-1
	if (idx==null)
	{
		array.length=0
		obj[fieldName]=array
		return -1
	}
	if ((idx<0)||(idx>last)) return -1
	if (idx==last) array.length=last
	else
	{
		var end=array.slice(idx+1)
		array.length=idx
		array=array.concat(end)
	}
	obj[fieldName]=array
	return idx
}

// ================================================================================

//function arrayMove(obj,fieldName,i,j)
//{
//	var array=obj[fieldName],len=array.length
//
//	if ((i<0)||(i>=len)||(j<0)||(j>=len)) return false
//	var old=array[i]
//	arrayRemove(obj,fieldName,i)
//	arrayAdd(obj,fieldName,old,j)
//	return true;
//}

// ================================================================================

//function arrayGetCopy(arr)
//{
//	var o=new Array,len=arr.length;
//
//	for (var i=0;i<len;i++)
//		o[i]=arr[i]
//
//	return o;
//}
// ================================================================================

//function arrayFind(obj,fieldName,v,subfield)
//return the index of value in array
//{
//	var array=obj[fieldName],len=array.length;
//
//	for (var i=0;i<len;i++)
//	{
//		if(subfield)
//		{
//			if (array[i][subfield] == v) return i;
//		}
//		else 
//			if(array[i]==v) return i;
//	}
//
//	return -1;
//}

// ================================================================================
// ================================================================================
//
// GLOBAL FUNCTIONS : FRAMES
//
// Frame functions
//
// ================================================================================
// ================================================================================

function getFrame(name,par)
{
	if (par==null) par=self
	var frames=par.frames,w=eval("frames."+name)

	if (w==null) return w

	var l=frames.length
	for (var i=0;i<l;i++)
	{
		w=frames[i]
		try {
			if (w.name==name)
						return w
		} catch (exc) {
			// keep on
		}	
	}

	return null
}

// ================================================================================

//function frameNav(name,url,fillHistory,par,noRefreshDrillBar)
//// par [window optional]
//{	
//	var fr=null
//	if (noRefreshDrillBar & name=="Report")
//	{		
//		var topfs=getTopFrameset();				
//		fr=topfs.getReportFrame()
//	} else {
//		fr=getFrame(name,par)
//	}		
//
//	if (fr) {				
//		var l=fr.location
//		
//		if (fillHistory)
//			l.href=url
//		else
//			l.replace(url)
//	} else {
//		var lay = document.getElementById(name)
//		if (lay)
//			lay.src=url;
//	}			
//}

// ================================================================================
/*
function genericIframeNav(url,fillHistory)
{	
				
	var l = getDynamicBGIFrameLayer()

	if (fillHistory)
	{
		l.href=url
	} else {
		l.replace(url)
	} 
}
*/
// ================================================================================

function frameGetUrl(win)
{
	return win.location.href
}

// ================================================================================

function frameReload(win)
{
	var loc=win.location
	loc.replace(loc.href)
}

// ================================================================================

function setTopFrameset()
// Marks the current window as a "top frameset"
// returns [void]
{
	_curWin._topfs="topfs"
}

// ================================================================================

function getTopFrameset(f)
// get int the parents window the one marked as "top frameset"
// returns [window] the top frameset, or null if not
{
	if(f == null)
		f = self

	if(f._topfs=="topfs")
	{
		return f;
	}
	else
	{
		if(f!= top)
			return getTopFrameset(f.parent)
		else
			return null;
	}
}

// ================================================================================
// ================================================================================
//
// GLOBAL FUNCTIONS : UTILITY FUNCTIONS
//
// Frame functions
//
// ================================================================================
// ================================================================================

function convStr(s,nbsp,br)
{
	s=""+s
	var ret=s.replace(/&/g,"&amp;").replace(/</g,"&lt;").replace(/>/g,"&gt;").replace(/"/g,"&quot;")

	if (nbsp)
		ret=ret.replace(/ /g,"&nbsp;")
	if (br)
		ret=ret.replace(/\n/g,"<br>")

	return ret
}

// ================================================================================

function escapeCR(s)
{
	s=""+s
	var ret=s.replace(/\r/g,"").replace(/\n/g,"\\n");

	return ret
}

// ================================================================================

function addDblClickCB(l,cb)
// Adds a function double click event handler. fixes a Netscape bug
// l [layer] the layer
// cb [Function] the double click function handler
{
	if (l.addEventListener && !_saf) {// ADAPT00521043 for Safari : Bug 7790: ondblclick doesn't fire when attached with addEventListener	  
	  l.addEventListener("dblclick",cb,false)	  
	} else {
	  l.ondblclick=cb
	} 
}

// ================================================================================

function img(src,w,h,align,att,alt)
// get the HTML of an image
// src    [String] the combined image url
// w      [int]    the width
// h      [int]    the height
// align  [String optional] image alignment
// att    [String optional] optional attributes (do no use style & alt & align attribute)
// alt    [String optional] image tooltip
{
	att=(att?att:'')
	if (alt==null) alt=''
	return '<img'+attr('width',w)+attr('height',h)+attr('src', src)+attr('alt',alt)+attr("align", align)+' border="0" hspace="0" vspace="0" '+(att?att:'')+'>'
}

// ================================================================================

function imgOffset(url,w,h,dx,dy,id,att,alt,st,align)
// get the HTML of an image that display only a part of a bigger one
// url    [String] the combined image url
// w      [int]    the VISIBLE width
// h      [int]    the VISIBLE height
// dx     [int]    the horizontal offset in combined image
// dy     [int]    the vertical offset in combined image
// id     [String optional] the DOM id
// att    [String optional] optional attributes (do no use style & alt & align attribute)
// alt    [String optional] image tooltip
// st     [String optional] optional style attributes
// align  [String optional] image alignment
{
	return img(_skin+'../transp.gif',w,h,align,
		(att?att:'') +' '+attr('id',id)+' style="float:left;'+backImgOffset(url,dx,dy)+(st?st:'')+'"',
		alt)
}

// ================================================================================

function simpleImgOffset(url,w,h,dx,dy,id,att,alt,st,align)
// get the HTML of an image that display only a part of a bigger one
// url    [String] the combined image url
// w      [int]    the VISIBLE width
// h      [int]    the VISIBLE height
// dx     [int]    the horizontal offset in combined image
// dy     [int]    the vertical offset in combined image
// id     [String optional] the DOM id
// att    [String optional] optional attributes (do no use style & alt & align attribute)
// alt    [String optional] image tooltip
// st     [String optional] optional style attributes
// align [String optional] optional style attributes
{	
	if (_ie)
	{
		if (dx==null) dx=0
		if (dy==null) dy=0
		return '<div '+(att?att:'')+' '+attr("id",id)+' style="position:relative;padding:0px;width:'+w+'px;height:'+h+'px;overflow:hidden;'+(st?st:'')+'">'+img(url,null,null,(align?align:'top'),'style="margin:0px;position:relative;top:'+(-dy)+'px;left:'+(-dx)+'px" tabIndex="-1"',alt)+'</div>'		
	}
	else
		return imgOffset(url,w,h,dx,dy,id,att,alt,st,align)
}

// ================================================================================

function changeSimpleOffset(lyr,dx,dy,url,alt)
// Generate a background image with a given offset
// ONLY APPLIES TO HTML GENRATED BY simpleImgOffset !!
// url [String] the combined image url
// dx  [int]    the horizontal offset in combined image
// dy  [int]    the vertical offset in combined image
{
	if (_ie)
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
		{
			lyr.title=alt
			lyr.alt = alt;
		}
	}
	else
		changeOffset(lyr,dx,dy,url,alt)
}

// ================================================================================

function backImgOffset(url,dx,dy)
// Generate a background image with a given offset
// url [String] the combined image url
// dx  [int]    the horizontal offset in combined image
// dy  [int]    the vertical offset in combined image
{
	return 'background-image:url(\''+url+'\');background-position:'+(-dx)+'px '+(-dy)+'px;'
}

// ================================================================================

function changeOffset(lyr,dx,dy,url,alt)
// Change the offset in backgroung image (and eventually the image)
// lyr [layer] the layer
// dx  [int] the horizontal offset in combined image
// dy  [int] the vertical offset in combined image
// url [String - Optional]  the combined image url
{
	var st=lyr.style
	if (st)
	{
		if ((dx!=null)&&(dy!=null))
			st.backgroundPosition=''+(-dx)+'px '+(-dy)+'px'
		if (url)
			st.backgroundImage='url(\''+url+'\')'
	}
	if(alt) lyr.title=alt
}

// ================================================================================

//function includeScript(url)
//// Include a script
//{
//	document.write('<scr'+'ipt language="javascript" charset="UTF-8" src="'+url+'"><\/scr'+'ipt>')	
//}

// ================================================================================

function includeCSS(css,noskin)
// Include the CSS related to the current skin
// css [String] file name with no extension
// noskin [boolean optional] if true, search in the parent folder
{
    if (typeof (_skin) == 'string' && _skin != "") {
        var url = ""

        if (noskin)
            url = _skin + '../' + css
        else
            url = _skin + css

        url += '.css'

        _curDoc.write ('<li' + 'nk rel="stylesheet" type="text/css" href="' + url + '">')
    }
}

// ================================================================================

function getLayer(id)
// Get a layer from its ID
// id [String] the id
{
	return _curDoc.getElementById(id)
}

// ================================================================================

function setLayerTransp(lyr,percent)
// lyr [Object] : the layer
// percent [number] : the opacity 0 .. 100
{
	if (_ie)	
		lyr.style.filter=(percent==null) ? "" :  "progid:DXImageTransform.Microsoft.Alpha( style=0,opacity="+percent+")"
	else
		lyr.style.MozOpacity=(percent==null) ? 1 : percent/100
}

// ================================================================================

function getPos(el,relTo)
// Get the layer position in pixels
// returns an object with (x,y) int fields
{
	relTo = relTo?relTo:null

	for (var lx=0,ly=0;(el!=null)&&(el!=relTo);
		lx+=el.offsetLeft,ly+=el.offsetTop,el=el.offsetParent);
	return {x:lx,y:ly}
}

// ================================================================================
// Gets the position of el relativeto relTo
// it fixes the cases where Safari with doctype fails
function getPos2(el,relTo) {
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

// ================================================================================
function getPosScrolled(el,relTo)
// Get the layer position in pixels
// returns an object with (x,y) int fields
{
	relTo = relTo?relTo:null

	if (_ie)
	{
		for (var lx=0,ly=0;(el!=null)&&(el!=relTo);
			lx+=el.offsetLeft-el.scrollLeft,ly+=el.offsetTop-el.scrollTop,el=el.offsetParent);
	}
	else
	{
		var oldEl=el

		for (var lx=0,ly=0;(el!=null)&&(el!=relTo);
			lx+=el.offsetLeft,ly+=el.offsetTop,el=el.offsetParent);

		for (el=oldEl;(el!=null)&&(el!=relTo);el=el.parentNode)
		{
			if (el.scrollLeft!=null)
			{
				lx-=el.scrollLeft
				ly-=el.scrollTop
			}
		}
	}
	
	lx+=getScrollX()
	ly+=getScrollY()

	return {x:lx,y:ly}
}



// ================================================================================

function getWidget(layer)
// Apply only with layers generated by widgets
// layer [layer] the layer
// return [Widget] the widget (or null)
{
	if (layer==null)
		return null

	var w=layer._widget
	if (w!=null)
		return _widgets[w]
	else
		return getWidget(layer.parentNode)
}

// ================================================================================

function getWidgetFromID(id)
// Retrieve a widget from its id
// id	[string]	the id of the widget
// return [Widget] the widget (or null)
{
	if (id==null)
		return null

	var l=getLayer(id)
	
	return getWidget(l)
}


// ================================================================================

function attr(key,val)
// writes an HTML attribute (if val null, do nothing)
// key [String] the attribute name
// value [String or null] the attribute value
// returs [String]
{
	return (val!=null?' '+key+'="'+val+'" ':'')
}

// ================================================================================

function sty(key,val)
// writes style value (if val null, do nothing)
// key [String] the style name
// value [String or null] the attribute value
// returs [String]
{
	return (val!=null?key+':'+val+';' :'')
}

// ================================================================================

function getSep(marg,solid)
// get the HTML for a 3D look horizontal separator
// marg [int optional] left & right margin
// solid [boolean optional] solid look
{
	if (marg==null)marg=0;var spc=marg>0?'<td width="'+marg+'">'+getSpace(marg,1)+'</td>':''; return '<table style="margin-top:5px;margin-bottom:5px;" width="100%" cellspacing="0" cellpadding="0"><tr>'+spc+'<td background="'+_skin+'sep'+(solid?'_solid':'')+'.gif" class="smalltxt"><img alt="" src="'+_skin+'../transp.gif" width="10" height="2"></td>'+spc+'</tr></table>'
}

// ================================================================================

function writeSep(marg,solid)
// writes a 3D look horizontal separator
// marg [int optional] left & right margin
// solid [boolean optional] solid look
{
	_curDoc.write(getSep(marg,solid))
}

// ================================================================================

function getSpace(w,h)
// Return the HTML for forcing a space
// Return [String]
{
	return '<table height="'+h+'" border="0" cellspacing="0" cellpadding="0"><tr><td>'+img(_skin+'../transp.gif',w,h)+'</td></tr></table>'
}

// ================================================================================

function writeSpace(w,h)
// write the HTML for forcing a space
// Return [void]
{
	_curDoc.write(getSpace(w,h))
}

// ================================================================================

function documentWidth(win)
// Gets the document(page) width
// return [int]
{
    
    var win=win?win:_curWin;  
    var width = Math.max(document.body.clientWidth,document.documentElement.clientWidth);
    width = Math.max(width,document.body.scrollWidth);
	return width;
}

// ================================================================================

function documentHeight(win)
// Gets the document(page) height
// return [int]
{
    
    var win=win?win:_curWin;  
    var height = Math.max(document.body.clientHeight,document.documentElement.clientHeight);
    height = Math.max(height,document.body.scrollHeight);
	return height;
}

// ================================================================================

function winWidth(win)
// Gets the window width
// return [int]
{
    var width;
    var win=win?win:_curWin;  
    if(_ie) 
    {
        if(_isQuirksMode) 
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

// ================================================================================

function winHeight(win)
// Gets the window height
// return [int]
{
    var win=win?win:_curWin;  
    var height;
    if(_ie) 
    {
        if(_isQuirksMode) 
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

// ================================================================================

function getScrollX(win)
// Return [int] the scrolling horizontal value in pixels
{
    var scrollLeft = 0;
    var win=win?win:_curWin;  
     
    if(typeof(win.scrollX ) == 'number') {
        scrollLeft = win.scrollX;
    }
    else {
        scrollLeft = Math.max(win.document.body.scrollLeft,win.document.documentElement.scrollLeft);
    }
    
    return scrollLeft;
}


// ================================================================================

function getScrollY(win)
// Get the X scroll position in pixels
// returns an int
{
    var scrollTop = 0;
    var win=win?win:_curWin;  
     
    if(typeof(win.scrollY ) == 'number') {
        scrollTop = window.scrollY;
    }
    else {
        scrollTop = Math.max(win.document.body.scrollTop,win.document.documentElement.scrollTop);
    }

    return scrollTop;
}

// ================================================================================

function winScrollTo(x, y, win)
// Scroll to the x, y position in pixels
// takes 2 int
{
	win=win?win:_curWin	
	win.scrollTo(x,y)
	/*/if (_ie) {
		win.document.body.scrollLeft = x
		win.document.body.scrollTop = y
	} else {
		win.scrollTo(x,y)
	}*/	
}

// ================================================================================

function eventGetKey(e,win)
// Get the event key code
// e [event]
{
	win=win?win:_curWin
	//return _ie?win.event.keyCode:e.which
	return _ie?win.event.keyCode:e.keyCode
}

// ================================================================================

function eventGetX(e)
// Get the event mouse abscissa
// e [event]
{
	return _ie?_curWin.event.clientX: e.clientX ? e.clientX : e.pageX;
}

// ================================================================================

function eventGetY(e)
// Get the event mouse ordinate
// e [event]
{
	return _ie?_curWin.event.clientY: e.clientY ? e.clientY : e.pageY;
}

// ================================================================================

function xpos(o,e,doc,zoom)
{
	if ((zoom==null)||(!_ie))
		zoom=1;
		
	return ((e.clientX/zoom)-getPos(o).x)+getScrollX();
}
		
// ================================================================================

function ypos(o,e,doc,zoom)
{
	if ((zoom==null)||(!_ie))
		zoom=1
	return ((e.clientY/zoom)-getPos(o).y)+(_ie?doc.body.scrollTop:0)
}

// ================================================================================

function absxpos(e,zoom)
{
	if ((zoom==null)||(!_ie)) {
		return e.clientX
	} else {	
		return e.clientX/zoom
	}	
}

// ================================================================================

function absypos(e,zoom)
{
	if ((zoom==null)||(!_ie)) {
		return e.clientY
	} else {	
		return e.clientY/zoom
	}	
}

// ================================================================================

function eventCancelBubble(e,win)
// Cancels event bubbling
// e [event]
{
    
	win=win?win:_curWin
	var ev =_ie? win.event : e;
	if(ev) {
	    ev.cancelBubble = true;
	    if(ev.stopPropagation) ev.stopPropagation();
	}
}

// ================================================================================

function isHidden(lyr)
// Test if a layer is hidden
// lyr [layer]
// return [boolean]
{
	if ((lyr==null)||(lyr.tagName=="BODY")) return false;var sty=lyr.style;if ((sty==null)||(sty.visibility==_hide)||(sty.display=='none')) return true;return isHidden(lyr.parentNode)
}

// ================================================================================

function opt(val,txt,sel)
// get an option tag
// val [String] the option value
// txt [String] the option text
// sel [boolean optional] set it selected
// return [String]
{
	return '<option value="'+val+'" '+(sel?'selected':'')+'>'+convStr(''+txt)+'</option>'
}

// ================================================================================

function lnk(inner,clickCB,cls,id,att,dblClickCB)
// writes a A link tag
// inner      [String] inner HTML
// clickCB    [String] click callback
// cls        [String] CSS class
// id         [String] tag ID
// dblClickCB [String] double click callback
{
	if (clickCB==null)
		clickCB="return false"

	att=att?att:'';return '<a'+attr('class',cls)+attr('id',id)+attr('href','javascript:void(0)')+attr('onclick',clickCB)+attr('ondblclick',dblClickCB)+att+'>'+inner +'</a>'
}

// ================================================================================

_oldErrHandler=null

function localErrHandler()
{
	return true
}

// ================================================================================

function canScanFrames(w)
// PRIVATE
{
	//_excludeFromFrameScan variable is set by a frame that does not want to
	// be scaned, when the frame is used to download document for instance.
	
	var ex=true,d=null
	
	if (_moz)
	{
		_oldErrHandler=window.onerror
		window.onerror=localErrHandler
	}
	
	try
	{
		d=w.document
		//if ((d!=null&&(typeof(d)).toLowerCase()!="unknown"))
			//ex=(w._excludeFromFrameScan==true)?w._excludeFromFrameScan:false
		ex=false
	}
	catch(expt)
	{
	}


	if (_moz)
		window.onerror=_oldErrHandler

	return (!ex&&(d!=null))
}

// ================================================================================

//function restoreAllInputs(win,level)
//// restore inputs
//{
//	if (_ie&&_curWin._inptStackLevel!=null)
//	{
//		win=win?win:_curWin
//
//		//_excludeFromFrameScan variable is set by a frame that does not want to
//		// be scaned, when the frame is used to download document for instance.
//		
//		if (canScanFrames(win))
//		{
//			if (level==null)
//				level=--_curWin._inptStackLevel
//
//			var b=win.document.body,arr=b?b.getElementsByTagName("SELECT"):null,len=arr?arr.length:0
//			for (var i=0;i<len;i++)
//			{
//				var inpt=arr[i]
//				if (inpt._boHideLevel==level)
//				{
//					inpt.style.visibility=inpt._boOldVis
//					inpt._boHideLevel=null
//				}
//			}
//			// Process sub frames
//			var frames=win.frames,flen=frames.length
//			for (var k=0;k<flen;k++)
//				restoreAllInputs(frames[k],level)
//		}
//	}
//}

// ================================================================================
//
//function hideAllInputs(x,y,w,h,win,level)
//// Hide all inputs. If parameters passed, only in the specified rectangle
//// x [int optional] the rectangle abscissa
//// y [int optional] the rectangle ordinate
//// w [int optional] the rectangle width
//// h [int optional] the rectangle height
//{
//	if (_ie)
//	{
//		win=win?win:_curWin
//
//		//_excludeFromFrameScan variable is set by a frame that does not want to
//		// be scaned, when the frame is used to download document for instance.
//		
//		if (canScanFrames(win))
//		{
//			var b=win.document.body,arr=b?b.getElementsByTagName("SELECT"):null,len=arr?arr.length:0
//
//			if (level==null)
//			{
//				if (_curWin._inptStackLevel==null)
//					_curWin._inptStackLevel=0
//
//				level=_curWin._inptStackLevel++
//			}
//
//			for (var i=0;i<len;i++)
//			{
//				var inpt=arr[i],css=inpt.style;
//				var inter=(x==null)||isLayerIntersectRect(inpt,x,y,w,h)
//
//				if (!isHidden(inpt)&&inter)
//				{
//					inpt._boHideLevel=level
//					inpt._boOldVis=css.visibility
//					css.visibility=_hide
//				}
//			}
//
//			// Process sub frames
//			var frames=win.frames,flen=frames.length
//
//			for (var k=0;k<flen;k++)
//				hideAllInputs(null,null,null,null,frames[k],level)
//		}
//	}
//}

// ================================================================================

function getBGIframe(id)
{
	return '<iframe id="'+id+'" name="'+id+'" style="display:none;left:0px;position:absolute;top:0px" src="' + _skin + '../../empty.html' + '" frameBorder="0" scrolling="no"></iframe>'
}


function getDynamicBGIFrameLayer()
{	
	var recycle=false
	if (_curWin.BGIFramePool) 
	{
		BGIFrames = _curWin.BGIFramePool.split(",")
		BGIFCount = BGIFrames.length
		for (var id = 0; id < BGIFCount; id++) {
			if (BGIFrames[id] != "1") {				
				recycle=true
				break
			}
		}
	} else {
		id = 0
		BGIFrames = new Array	
	}
	BGIFrames[id] = "1"
	_curWin.BGIFramePool = BGIFrames.join(",")
	if (!recycle) {				
		targetApp(getBGIframe("BGIFramePool_" + id))
	}
	return getLayer("BGIFramePool_" + id)
}

function holdBGIFrame(layerId) {
	var l = getLayer(layerId)
	if (l) {
		l.style.display=""
	}	
	id = parseInt(layerId.split('_')[1])
	BGIFrames = _curWin.BGIFramePool.split(",")
	BGIFrames[id]=1
	_curWin.BGIFramePool = BGIFrames.join(",")
}

function releaseBGIFrame(layerId) {
	var l = getLayer(layerId)
	if (l) {
		l.style.display="none"
	}	
	id = parseInt(layerId.split('_')[1])
	BGIFrames = _curWin.BGIFramePool.split(",")
	BGIFrames[id]=0
	_curWin.BGIFramePool = BGIFrames.join(",")
}
// ================================================================================

// ================================================================================

function append(e,s,c)
// inserts HTML BEFORE the end of a given layer
// e [layer] the layer
// s [String] the HTML
// c curDoc
{
		
	if (_ie)
		e.insertAdjacentHTML("BeforeEnd",s)
	else
	{
		var curDoc = c?c:_curDoc
		var r=curDoc.createRange()
		r.setStartBefore(e)
		var frag=r.createContextualFragment(s)
		e.appendChild(frag)
	}
}

// ================================================================================

function append2(e,s,c)
// fixes IE's problem of appending elements to an element that hasn't been completely loaded
// inserts HTML BEFORE the end of a given layer
// e [layer] the layer
// s [String] the HTML
// c curDoc
{
		
	if (_ie)
		e.insertAdjacentHTML("afterBegin",s)
	else
	{
		var curDoc = c?c:_curDoc
		var r=curDoc.createRange()
		r.setStartBefore(e)
		var frag=r.createContextualFragment(s)
		e.appendChild(frag)
	}
}

// ================================================================================

function insBefore(e,s,c)
// inserts HTML AFTER the end of a given layer
// e [layer] the layer
// s [String] the HTML
{
	if (_ie)
		e.insertAdjacentHTML("BeforeBegin",s)
	else
	{
		var curDoc = c?c:_curDoc
		var r=_curDoc.createRange()
		r.setEndBefore(e)
		var frag=r.createContextualFragment(s)
		e.parentNode.insertBefore(frag,e)
	}
}

// ================================================================================

function insBefore2(e,s,c)
// Fixes Safari's problem of inserting HTML string before an element
// inserts HTML AFTER the end of a given layer
// e [layer] the layer
// s [String] the HTML
{
	if (_ie)
		e.insertAdjacentHTML("BeforeBegin",s)
	else
	{
		var curDoc = c?c:_curDoc
		var r=_curDoc.createRange()
		r.setStartBefore(e)
		var frag=r.createContextualFragment(s)
		e.parentNode.insertBefore(frag,e)
	}
}

// ================================================================================

function targetApp(s)
// Add HTML code at the end of the body
// s [String] the HTML
{
	append(_curDoc.body,s)
}

// ================================================================================

//function getBasePath()
//// Get the url path of the document WITHOUT the page name
//// Return [String] the path
//{
//	var url=document.location.href,last1= url.lastIndexOf('?');if (last1>=0) url=url.slice(0,last1);var last = url.lastIndexOf('/');return (last>=0)?url.slice(0,last+1):url
//}

// ================================================================================

//function isLayerIntersectRect(l,x1,y1,w,h)
// Test if a layer intersection with a rectangle if non empty
// l  [layer]
// x1 [int optional] the rectangle abscissa
// y1 [int optional] the rectangle ordinate
// w  [int optional] the rectangle width
// h  [int optional] the rectangle height
//{
//	var xl1=getPos(l).x,yl1=getPos(l).y,xl2=xl1+l.offsetWidth,yl2=yl1+l.offsetHeight,x2=x1+w,y2=y1+h
//
//	return ((x1>xl1)||(x2>xl1))&&((x1<xl2)||(x2<xl2)) && ((y1>yl1)||(y2>yl1))&&((y1<yl2)||(y2<yl2))
//}

// ================================================================================

function preloadImg(url)
// preload an image
// url [String] the image URL
{
	var img=_preloadArr[_preloadArr.length]=new Image;img.src=url
}

// ================================================================================

//function convURL(str)
//{
//	if (_dontNeedEncoding == null)
//	{
//		//first time call for this page, generates "static" variables
//		_dontNeedEncoding = new Array(256);
//		for (var i = 0 ; i < 256 ; i++) _dontNeedEncoding[i] = false;
//		for (var i = (new String('a')).charCodeAt(0); i <= (new String('z')).charCodeAt(0); i++) _dontNeedEncoding[i] = true;
//		for (var i = (new String('A')).charCodeAt(0); i <= (new String('Z')).charCodeAt(0); i++) _dontNeedEncoding[i] = true;
//		for (var i = (new String('0')).charCodeAt(0); i <= (new String('9')).charCodeAt(0); i++) _dontNeedEncoding[i] = true;
//		_dontNeedEncoding[(new String(' ')).charCodeAt(0)] = true; /* encoding a space to a + is done in the encode() method */
//		_dontNeedEncoding[(new String('-')).charCodeAt(0)] = true;
//		_dontNeedEncoding[(new String('_')).charCodeAt(0)] = true;
//		_dontNeedEncoding[(new String('.')).charCodeAt(0)] = true;
//		_dontNeedEncoding[(new String('*')).charCodeAt(0)] = true;
//		_thex = new Array("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F");
//	}
//
//	var encstr = "";
//	for (var i = 0 ; i < str.length ; i++) encstr += URLEncodeUTF8Char(str.charAt(i));
//	return encstr;
//}

// ================================================================================

//function URLEncodeUTF8Char(c)
//{
//	var unicodeval = c.charCodeAt(0);
//	if (unicodeval < 128)
//	{
//		//first case should generate a simple byte with high bit at 0 (7 bit ASCII US)
//		if (_dontNeedEncoding[unicodeval]) return (c == ' ' ? '+' : c);
//		else return ("%" + _thex[unicodeval >> 4] + _thex[unicodeval & 15]);
//	}
//	else if (unicodeval < 2048)
//	{
//		//on 2 bytes 110xxxxx 10xxxxxx
//		return ("%" 	+ _thex[(unicodeval >> 10) | 12]
//		+ _thex[(unicodeval >> 6) & 15]
//		+ "%"	+ _thex[(unicodeval >> 4) & 3 | 8]
//		+ _thex[unicodeval & 15]);
//	}
//	else
//	{
//		//on 3 bytes 1110xxxx 10xxxxxx 10xxxxxx
//		return ("%"	+ _thex[14]
//		+ _thex[unicodeval >> 12]
//		+ "%" 	+ _thex[(unicodeval >> 10) & 3 | 8]
//		+ _thex[(unicodeval >> 6) & 15]
//		+ "%"	+ _thex[(unicodeval >> 4) & 3 | 8]
//		+ _thex[unicodeval & 15]);
//	}
//}

// ================================================================================

//function encString(s)
//// encString() allows to serialize collections of
//// string with beginnig by "[", terminated with "]"
//// and using "," as separator.
//// s [String] the string to encode
//// return [String] the encoded string
//{
//	var res=''
//	if (s!=null)
//	{
//		var len=s.length
//		for (var i=0;i<len;i++)
//		{
//			var c=s.charAt(i)
//			switch (c)
//			{
//				case '$': res+='$3'; break
//				case ',': res+='$4'; break
//				case '[': res+='$5'; break
//				case ']': res+='$6'; break
//				default: res+=c; break
//			}
//		}
//	}
//	return res
//}

// ================================================================================

//function enc()
//// Encode several values
//// Variable number of parameters
//{
//	var args=enc.arguments,len=args.length,s='['
//	if (len>0) s+=args[0]
//	for (var i=1;i<len;i++) s+=','+args[i]
//	return s+']'
//}

// ================================================================================

//remove space caracters around a string
//function remSpaceAround(s)
//{	
//	var len = s.length;
//	if(len<=0) return "";
//	var start=0,end=len
//	var c=s.substr(start,1);
//	while (c==' ' && start<len)
//	{
//		start++
//		c=s.substr(start,1);		
//	}
//	if(start<len)
//	{
//		c=s.substr(end-1,1);
//		while (c==' ')
//		{
//			end--
//			c=s.substr(end-1,1);		
//		}
//	}
//	var sub = s.substring(start,end);
////	alert(sub.length +"-"+sub)
//	return sub	
//}
// ================================================================================

//function getArrows(upCb,downCB,hori,newNode,newNodeCB)
//// OBSOLETE
//
//// Get the HTML String for move buttons if a list
//// upCb [String] : callback string when pushing the up (rep left if hori == true) button
//// downCb [String] : callback string when pushing the down (rep right if hori == true) button
//// hori [Boolean - optional, default is false] : if true, display left & right arrows. Else display top & bottom arrows.
//// Return [String] the HTML
//{
//	if (hori==null) hori=false;
//
//	var s=''
//	if (hori) s+='<nobr>'
//	s+=lnk(img(_skin+(hori?'left.gif':'up.gif'),12,12,'top',null,hori?"_LEFT ARROW":"_UP ARROW"),upCb,null,null,null,(_moz?null:upCb))
//	s+=(hori?'':img(_skin+'../transp.gif',1,5)+'<br>')
//	s+=lnk(img(_skin+(hori?'right.gif':'down.gif'),hori?11:12,hori?12:11,'top',null,hori?"_RIGHT ARROW":"_LEFT ARROW"),downCB,null,null,null,(_moz?null:downCB))
//
//	if (newNode)
//	{
//		s+=img(_skin+'../transp.gif',1,5)+'<br>'
//		s+=lnk(img(_skin+('node.gif'),12,12,'top',null,"_NEW NODE"),newNodeCB,null,null,null,(_moz?null:newNodeCB))
//	}
//
//	if (hori) s+='</nobr>'
//	return s
//}


// ================================================================================
// ================================================================================
//
// OBJECT newBlockWhileWait (Constructor)
//
// Class for dialog box
//
// ================================================================================
// ================================================================================

_staticUnicBlockWhileWaitWidgetID = "staticUnicBlockWhileWaitWidgetID"

function hideBlockWhileWaitWidget()
{
	var lyr=getLayer(_staticUnicBlockWhileWaitWidgetID)
	if (lyr)
		lyr.style.display="none"
}

function newBlockWhileWaitWidget(urlImg)
// Return [TooltipWidget] the instance
{
	if (window._BlockWhileWaitWidget!=null)
		return window._BlockWhileWaitWidget

	var o=newWidget(_staticUnicBlockWhileWaitWidgetID)
	o.getPrivateHTML=BlockWhileWaitWidget_getPrivateHTML
	o.init=BlockWhileWaitWidget_init

	o.show=BlockWhileWaitWidget_show
	window._BlockWhileWaitWidget=o

	return o
}

// ================================================================================

function BlockWhileWaitWidget_init()
{
	//cancels the default init behaviour
}

// ================================================================================

function BlockWhileWaitWidget_getPrivateHTML()
// Returns [String] the HTML
{
	return '<div id="'+ this.id+'" onselectstart="return false" ondragstart="return false" onmousedown="'+_codeWinName+'.eventCancelBubble(event)" border="0" hspace="0" vspace="0"  style="background-image:url('+_skin+'../transp.gif);z-index:5000;cursor:wait;position:absolute;top:0px;left:0px;width:100%;height:100%"></div>'
}

// ================================================================================

function BlockWhileWaitWidget_show(show)
// show [boolean] true for showing the tooltip
{
	var o=this

	// object not init yet, 2 cases
	if (o.layer==null)
	{
		o.layer=getLayer(o.id)

		// another instance hasn't written it's HTML yet
		if (o.layer==null)
		{
			targetApp(o.getPrivateHTML())
			o.layer=getLayer(o.id)
			o.css=o.layer.style
		}
		else
		{	// convenient use of css property
			o.css=o.layer.style
		}
	}
	o.setDisplay(show)
}

/*
var regLang= /^[a-zA-Z]{2}$|^[a-zA-Z]{2}_[a-zA-Z]{2}$/, regIntPos=/^\d+$/, regYes=/^yes$/, regPath=/^[\w|\/|:|.|-]+$/, regAlphanumDot=/^\w+\.+\w+$/, regAlphanumDotEx=/^\w+\.*\w+\.*\w+$/
var paramRegs = new Array()
paramRegs["ID"]=regAlphanumDot
paramRegs["allTableCells"]=regYes
paramRegs["gotoPivot"]=regYes
paramRegs["reportIndex"]=regIntPos
paramRegs["fromLPane"]=regYes
paramRegs["skin"]=regPath
paramRegs["lang"]=regLang
paramRegs["noGrabber"]=regYes
paramRegs["isFormulaEdit"]=regYes
paramRegs["fromQuery"]=regYes
paramRegs["isFromHyperlinkEditor"]=regYes
paramRegs["iAction"]=regIntPos
paramRegs["callback"]=regAlphanumDotEx
paramRegs["callbackin"]=regAlphanumDotEx
paramRegs["callbackout"]=regAlphanumDotEx

function requestQueryString(win, par){
	params = win.location.search.substr(1).split("&")
	for(i=0;i<params.length;i++){
		var param = params[i].split("="), key = param[0], val = param[1]
		if (key == par){
			var reg = new RegExp(paramRegs[key])
			if ((val == "") || (reg.test(val))) {
				return val
			} else {
				//alert("bad param error, val=" + val + ", key=" + key)
				// the following code is not library independant at all...
				var tpfs = getTopFrameset()
				if (tpfs != null) {
					tpfs._dontCloseDoc=true
					tpfs.document.location.replace(tpfs._root+"html/badparamserror.html");
				} else {
					tpfs=getTopFrameset(window.opener)					
					if (tpfs != null)
					{
						document.location.replace(tpfs._skin+"../../../html/badparamserror.html");
					}
				}	
			}	
		}
	}
}
*/
// ================================================================================
//	Manipulating String function
// ================================================================================
//function trim(strString) {
//    if (strString != null) {
//        var iLength = strString.length;
//        //Left side
//        var i;
//        for (i=0; i<iLength; i++) {
//            if (strString.charAt(i) != " ") {
//                break;
//            }
//        }
//        strString = strString.substring(i);
//        iLength = strString.length;
//        //Right Side
//        for (i=iLength; i>0; i--) {
//            if (strString.charAt(i-1) != " ") {
//                break;
//            }
//        }
//        strString = strString.substring(0,i);
//    }
//    return strString;
//}

//function startsWithIgnoreCase(strString, strToFind) {
//    var blnRet = false;
//    if (strToFind != null) {
//        var strVar = strString.substring(0, strToFind.length);
//        if (strVar.toLowerCase() == strToFind.toLowerCase()) {
//            blnRet = true;
//        }
//    }
//    return blnRet;
//}

//function endsWithIgnoreCase(strString, strToFind) {
//    var blnRet = false;
//    if (strToFind != null) {
//        var iRight = strString.length- strToFind.length;
//        if (iRight >= 0) {
//            var strVar = strString.substring(iRight);
//            if (strVar.toLowerCase() == strToFind.toLowerCase()) {
//                blnRet = true;
//            }
//        }
//    }
//    return blnRet;
//}

// ================================================================================
function isTextInput(ev) {
	    var source = _ie?ev.srcElement:ev.target;
	    var isText=false;
	    if(source.tagName=="TEXTAREA")
	        isText=true;
	    if((source.tagName=="INPUT") && ((source.type.toLowerCase()=="text") || (source.type.toLowerCase()=="password")))
	        isText=true;
	
	    return isText;
}

function isTextArea(ev) {
	    var source = _ie?ev.srcElement:ev.target;	
	    if(source.tagName=="TEXTAREA")
	        return true;
	    else 
	        return false;
}

// function shrink text, used for tooltip, because a too long tooltip flickers ADAPT00465457 
// max n chars ?
//function shrinkTooltip(t,n)
//{ 
//	var n = n?n:360
//	return (t.length < n)? t : (t.substring(0,n) + "...")
//}
//	

// ================================================================================
// ================================================================================
//
// Functions to format a date
//
// ================================================================================
// ================================================================================

//function setDateValue(strDateValue, strInputFormat)
//{	
//	var strRet = ",,";
//	var strYear = "";
//	var strMonth = "";
//	var strDay = "";
//	//Get separator
//	var length = strInputFormat.length;
//	var sep = "";
//	for (var i=0; i<length; i++) 
//	{
//		var c = strInputFormat.charAt(i);
//		switch(c) 
//		{
//			case "/":
//			case "-":
//			case ".":
//			case ",":
//			case "\"": sep = c; break;
//		}
//		if (sep != "") break;
//	}
//	if (sep != "") 
//	{
//		var arrInputFormat = strInputFormat.split(sep);
//		var arrDateValue = strDateValue.split(sep);
//
//		for (var i=0; i<arrDateValue.length; i++) 
//		{
//			if (arrInputFormat[i] != null && typeof(arrInputFormat[i]) != "undefined") 
//			{
//				//Year
//				if (arrInputFormat[i].indexOf('y')>=0) 
//				{
//					var iPosA = arrInputFormat[i].indexOf('y');
//					var iPosB = arrInputFormat[i].lastIndexOf('y');
//					if (iPosB>=0) 
//					{
//						strYear = arrInputFormat[i].substring(iPosA, iPosB + 1);
//						if (strYear.length >= arrDateValue[i].length) strYear = arrDateValue[i];
//						else 
//						{
//							iPosB = iPosA;
//							for (var j=iPosA; j<arrDateValue[i].length; j++) 
//							{
//								var c = arrDateValue[i].charAt(j);
//								if (c < '0' || c > '9') break;
//								else iPosB = j + 1;
//							}
//							strYear = arrDateValue[i].substring(iPosA, iPosB);
//							if (strYear.length <= 2) 
//							{
//								var iYear = parseInt(strYear);
//								if (iYear>=70) iYear += 1900;
//								else iYear += 2000;
//								strYear = iYear.toString();
//							}
//						}
//					}
//					else 
//					{
//						return strRet;
//					}
//				}
//				//Month
//				else if (arrInputFormat[i].indexOf('M')>=0) 
//				{
//					var iPosA = arrInputFormat[i].indexOf('M');
//					var iPosB = arrInputFormat[i].lastIndexOf('M');
//					if (iPosB>=0) 
//					{
//						strMonth = arrInputFormat[i].substring(iPosA, iPosB + 1);
//						if (strMonth.length >= arrDateValue[i].length) strMonth = arrDateValue[i];
//						else 
//						{
//							iPosB = iPosA;
//							for (var j=iPosA; j<arrDateValue[i].length; j++) 
//							{
//								var c = arrDateValue[i].charAt(j);
//								if (c < '0' || c > '9') break;
//								else iPosB = j + 1;
//							}
//							strMonth = arrDateValue[i].substring(iPosA, iPosB);
//						}
//					}
//					else 
//					{
//						return strRet;
//					}
//				}
//				//Day
//				else if (arrInputFormat[i].indexOf('d')>=0) 
//				{
//					var iPosA = arrInputFormat[i].indexOf('d');
//					var iPosB = arrInputFormat[i].lastIndexOf('d');
//					if (iPosB>=0) 
//					{
//						strDay = arrInputFormat[i].substring(iPosA, iPosB + 1);
//						if (strDay.length >= arrDateValue[i].length) strDay = arrDateValue[i];
//						else {
//							iPosB = iPosA;
//							for (var j=iPosA; j<arrDateValue[i].length; j++) 
//							{
//								var c = arrDateValue[i].charAt(j);
//								if (c < '0' || c > '9') break;
//								else iPosB = j + 1;
//							}
//							strDay = arrDateValue[i].substring(iPosA, iPosB);
//						}
//					}
//					else 
//					{
//						return strRet;
//					}
//				}
//			}
//		}
//
//		if (strMonth != "" && strDay != "" && strYear != "" && !(isNaN(strMonth) || isNaN(strDay) || isNaN(strYear))) 
//		{
//			//Set Date
//			strRet = strMonth + ',' + strDay + ',' + strYear;
//		}		
//	}
//	return strRet;
//}

// ================================================================================

function LZ(x) {
    return(x<0||x>9?"":"0")+x
}

// ================================================================================
//
//function formatDate(date,format)
//{
//	var format=format+"";	
//    var result="";
//    var i_format=0;
//    var c="";
//    var token="";
//    var y=date.getFullYear()+"";
//    var M=date.getMonth()+1;
//    var d=date.getDate();
//    var E=date.getDay();
//    var H=date.getHours();
//    var m=date.getMinutes();
//    var s=date.getSeconds();
//    var yyyy,yy,MMM,MM,dd,hh,h,mm,ss,ampm,HH,H,KK,K,kk,k;
//    // Convert real date parts into formatted versions
//    var value=new Object();
//    if (y.length==2) {
//	if (y-0>=70) y=""+(y-0+1900);
//	else y=""+(y-0+2000);
//    }
//    value["y"]=""+y;
//    value["yyyy"]=y;
//    value["yy"]=y.substring(2,4);
//    value["M"]=M;
//    value["MM"]=LZ(M);
//    value["MMM"]=_month[M-1];
//    value["NNN"]=_month[M+11];
//    value["d"]=d;
//    value["dd"]=LZ(d);
//    value["E"]=_day[E+7];
//    value["EE"]=_day[E];
//    value["H"]=H;
//    value["HH"]=LZ(H);
//    if (H==0){value["h"]=12;}
//    else if (H>12){value["h"]=H-12;}
//    else {value["h"]=H;}
//    value["hh"]=LZ(value["h"]);
//    if (H>11){value["K"]=H-12;} else {value["K"]=H;}
//    value["k"]=H+1;
//    value["KK"]=LZ(value["K"]);
//    value["kk"]=LZ(value["k"]);
//    if (H > 11) { value["a"]=_PM; value["aa"]=_PM;; }
//    else { value["a"]=_AM; value["aa"]=_AM; }
//    value["m"]=m;
//    value["mm"]=LZ(m);
//    value["s"]=s;
//    value["ss"]=LZ(s);
//    while (i_format < format.length) {
//        c=format.charAt(i_format); 
//        token="";
//        while ((format.charAt(i_format)==c) && (i_format < format.length)) {
//            token += format.charAt(i_format++);
//        }
//        if (value[token] != null) { result=result + value[token]; }
//        else { result=result + token; }
//     }
//   return result;
//}

//
//// ================================================================================
//// ================================================================================
////
////  Search Widget
////
//// ================================================================================
//// ================================================================================
//
//function newSearchWidget(id,w,searchCB,helpText)
//{
//	var o=newWidget(id)
//									
//	o.bMatchCase			= false;	
//	
//	o.searchField			= newTextFieldWidget(id+"_searchVal",null,50,SearchWidget_keyUpCB,SearchWidget_searchCB,true,_lovSearchFieldLab,w?(w-40):null);
//	o.searchField.par		= o;
//	o.searchField.setHelpTxt(helpText?helpText:_lovSearchFieldLab);
//	
//	o.searchIcn				= newIconMenuWidget(id+"_searchIcn",_skin+'../lov.gif',SearchWidget_searchCB,null,_lovSearchLab,16,16,0,0,0,0)
//	o.searchIcn.par			= o
//	o.searchMenu			= o.searchIcn.getMenu()
//	o.normal				= o.searchMenu.addCheck(id+"normal",_lovNormalLab,SearchWidget_normalClickCB)
//	o.matchCase				= o.searchMenu.addCheck(id+"matchCase",_lovMatchCase,SearchWidget_matchCaseClickCB)
//		
//	o.oldInit				= o.init
//	o.searchCB				= searchCB
//	o.init					= SearchWidget_init
//	o.getHTML				= SearchWidget_getHTML
//	o.setCaseSensitive		= SearchWidget_setCaseSensitive
//	o.isCaseSensitive		= SearchWidget_isCaseSensitive
//	o.updateMatchCase		= SearchWidget_updateMatchCase
//	o.getSearchValue		= SearchWidget_getSearchValue;
//	o.setSearchValue		= SearchWidget_setSearchValue;
//	o.resize				= SearchWidget_resize;		
//	
//	return o;
//}
//
//function SearchWidget_getHTML()
//{
//	var o=this, s =''
//	s=	'<table id="'+o.id+'" border="0" cellspacing="0" cellpadding="0"><tbody>' +
//			'<tr>' +
//				'<td>' + o.searchField.getHTML() + '</td>' +
//				'<td>' + o.searchIcn.getHTML() + '</td>' +
//			'</tr>' +
//		'</tbody></table>';	
//		
//	return s
//}
//
//function SearchWidget_resize(w,h)
//{
//	var o = this
//	
//	o.searchField.resize(w-40,h);	
//}
//
//function SearchWidget_init()
//{
//	var o=this
//	
//	o.oldInit();
//	o.searchField.init()	
//	o.searchIcn.init()
//	o.searchIcn.setDisabled((o.searchField.getValue()==''))	// Disabled if no value set on loading widget
//	o.updateMatchCase(o.bMatchCase)			
//}
//
//// function isCaseSensitive
//// indicates if the option is checked
//function SearchWidget_isCaseSensitive()
//// return	[boolean]	true if the option is checked
//{
//	return this.bMatchCase;
//}
//
//// function setCaseSensitive
//// check or uncheck the match case checkbox
//// update the icon
//function SearchWidget_setCaseSensitive(b)
//// b	[boolean]	used as is
//{
//	var o=this
//	
//	if(o.bMatchCase!=b)
//	{
//		o.updateMatchCase(b);
//		o.bMatchCase=b;
//	}
//}
//
//function SearchWidget_updateMatchCase(b)
//{
//	var o=this
//	
//	o.normal.check(!b)
//	o.matchCase.check(b)
//	
//	if (b)
//		o.searchIcn.icon.changeImg(55,0)	// match case icon
//	else
//		o.searchIcn.icon.changeImg(0,0)		// normal icon			
//}
//
//function SearchWidget_normalClickCB()
//{
//	var o=this.par.parIcon.par
//		
//	if(o.bMatchCase) 
//		o.bMatchCase=false;	
//	
//	o.updateMatchCase(o.bMatchCase);		
//}
//
//function SearchWidget_matchCaseClickCB()
//{
//	var o=this.par.parIcon.par
//		
//	if(!o.bMatchCase) 
//		o.bMatchCase=true;	
//	
//	o.updateMatchCase(o.bMatchCase);	
//}
//
//function SearchWidget_keyUpCB()
//{
//	var p=this.par
//		
//	p.searchIcn.setDisabled((this.getValue()==''))
//}
//
//function SearchWidget_searchCB()
//{
//	var p=this.par;
//	
//	if (p.searchCB != null)
//		p.searchCB();
//}
//
//// getSearchValue()
//// Return the value inserted in the search text field
//function SearchWidget_getSearchValue()
//// return		a string with the value entered in the search field
//{
//	var o=this;
//	return o.searchField.getValue();
//}
//
//// setSearchValue(s)
//// Set the value of the search text field
//function SearchWidget_setSearchValue(s)
//// s			[String]	string that will be displayed in the textfield
//{
//	var o=this;
//	o.searchField.setValue(s);
//}

// ================================================================================
// ================================================================================
//
// OBJECT ToggleButtonWidget (Constructor)
//
// Class for building push buttons
//
// ================================================================================
// ================================================================================
/*
function newToggleButtonWidget(id,label,cb,width,hlp,tooltip,tabIndex,margin,url,w,h,dx,dy,imgRight,disDx,disDy,togX,togY)
{
// CONSTRUCTOR
// id       [String] button ID
// label    [String Optional] button label
// w        [int optional] : text zone minimal width
// hlp      OBSOLETE
// tooltip  [String Optional] button tooltip
// tabIndex [int Optional] for 508/IE : tab index for keyboard navigation
// url      [String Optional] : the combined image url
// w        [int Optional] the visible image part width
// dx       [int Optional] the horizontal offset in image
// dy       [int Optional] the vertical offset in image
// disDx    : [int - optional]      horizontal offset for disabled state
// disDy    : [int - optional]      vertical offset for disabled state

	var o=newButtonWidget(id,label,cb,width,hlp,tooltip,tabIndex,margin,url,w,h,dx,dy,imgRight,disDx,disDy)
	o.togX = togX
	o.togY = togY
	o.checked=false
	o.executeCB=ToggleButtonWidget_executeCB
	o.check=ToggleButtonWidget_check
	o.isChecked=ToggleButtonWidget_isChecked	
	o.init=ButtonWidget_init

	return o
}

// ================================================================================

function ToggleButtonWidget_executeCB()
// PRIVATE internal click event handler
{
	var o=this
		
	o.check(!o.checked) // toggle
	
	if (o.cb)
	{
		if (typeof o.cb!="string")
			o.cb()
		else
			eval(o.cb)
	}
}

function ToggleButtonWidget_check(checked)
// check or uncheck an icon check widget
// check : [boolean] specified if the check icon must be checked or not
{
	var o=this
	
	if (o.checked != checked)
	{
		o.checked=checked

		if (o.checked)
		{			
			changeSimpleOffset(o.icn,o.togX,o.togY,o.url)			
		} else {	
			changeSimpleOffset(o.icn,o.dx,o.dy,o.url);			
		}
	}	
	
	if (o.checked&&o.beforeClickCB)
	{
		if (o.layer)
			o.beforeClickCB()
	}
}

// ================================================================================

function ToggleButtonWidget_isChecked()
// return true if checked
{
	return this.checked
}
*/

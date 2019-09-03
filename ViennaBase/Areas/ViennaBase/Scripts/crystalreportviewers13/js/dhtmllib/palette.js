
// <script>

/*
=============================================================
WebIntelligence(r) Report Panel
Copyright(c) 2001-2003 Business Objects S.A.
All rights reserved

Use and support of this software is governed by the terms
and conditions of the software license agreement and support
policy of Business Objects S.A. and/or its subsidiaries. 
The Business Objects products and technology are protected
by the US patent number 5,555,403 and 6,247,008

File: palette.js
=============================================================
*/

_allBOIcons=new Array
_allBOIconsMenus=new Array
//_lastUsedColors=new Array
//_maxUsedColors=8

_menuType_simple=0
_menuType_color	=1
_menuType_border=2

// ================================================================================
// ================================================================================
//
// OBJECT NewLabelWidget (Constructor)
//
// Display a simple text. It can be used in a palette for instance
//
// ================================================================================
// ================================================================================

function NewLabelWidget(id,text,convBlanks)
// CONSTRUCTOR
// id [String] the label id for DHTML processing
// text [String] label text
// convBlanks [boolean optional] if true, blank are converted into &nbsp;
{
	var o=newWidget(id)
	o.text=text
	o.convBlanks=convBlanks
	o.getHTML=LabelWidget_getHTML
	o.setDisabled=LabelWidget_setDisabled
	o.dis=false

	return o;
}

// ================================================================================

function LabelWidget_setDisabled(dis)
// set the widget disabled
// dis [boolean] if true, disables the widget
{
	var o=this
	if (o.dis!=dis)
	{
		o.dis=dis
		if (o.layer)
		{
			o.layer.className="iconText"+(dis?"Dis":"")
		}
	}
}

// ================================================================================

function LabelWidget_getHTML()
// returns [String] the HTML of a label widget
{
	var o=this
	return '<div id="'+o.id+'" class="iconText'+(o.dis?"Dis":"")+'" style="white-space:nowrap;margin-right:4px;margin-left:4px;cursor:default">'+convStr(o.text,o.convBlanks)+'</div>'
}

// ================================================================================
// ================================================================================
//
// OBJECT newIconWidget (Constructor)
//
// Display a clickable icon with optional text. It can be used in a palette fo
// instance. The image can be a sub-part of a bigger image 
// (use dx and dy to define offset)
//
// ================================================================================
// ================================================================================

function newIconWidget(id,src,clickCB,text,alt,w,h,dx,dy,disDx,disDy,isTabEnabled,ariaHasPopUp)
// CONTRUCTOR
// id       	: [String]              the icon id for DHTML processing
// src      	: [String]              the icon image url
// clickCB  	: [Function - optional] callback called when clicking
// text     	: [String - optional]   icon  text
// alt     	 	: [String]              icon tooltip
// w        	: [int]                 visible image width
// h        	: [int]                 visible image height
// dx       	: [int - optional]      horizontal offset in the image
// dy       	: [int - optional]      vertical offset in the image
// disDx    	: [int - optional]      horizontal offset for disabled state
// disDy    	: [int - optional]      vertical offset for disabled state
// isTabEnabled : [boolean - optional] 	boolean for allowing tab control
{
	var o=newWidget(id)
	o.src=src
	o.clickCB=clickCB
	o.text=text
	o.alt=alt
	o.isTabEnabled=isTabEnabled
	o.ariaHasPopUp=ariaHasPopUp
	o.width=null
	o.txtAlign="left"
	o.border=4
	o.txtNoPadding=false	
	
	o.allowDblClick=false
	
	if (src)
	{
		o.w=(w!=null)?w:16
		o.h=(h!=null)?h:16
		o.dx=(dx!=null)?dx:0
		o.dy=(dy!=null)?dy:0
		o.disDx=(disDx!=null)?disDx:0
		o.disDy=(disDy!=null)?disDy:0
	}
	else
	{
		o.w=1		
		o.h=16
	}

	o.dis=false
	o.disp=true
	o.margin=1
	o.extraHTML=''	
	
	o.imgLayer=null
	o.txtLayer=null
	
	o.overCB="IconWidget_overCB"
	o.outCB="IconWidget_outCB"

	o.isDisplayed=IconWidget_isDisplayed
	o.setDisplay=IconWidget_setDisplay
	o.getHTML=IconWidget_getHTML
	o.getTxtWidth=IconWidget_getTxtWidth
	o.index=_allBOIcons.length++
	o.nocheckClass="iconnocheck"
	o.hoverClass="iconhover"
	o.checkClass="iconcheck"
	o.checkhoverClass="iconcheckhover"
	o.currentClass=o.nocheckClass
	o.currentHoverClass=o.hoverClass
	o.setClasses=IconWidget_setClasses
	o.internalUpCB=null
	o.internalDownCB=IconWidget_internalDownCB
	o.internalUpCB=IconWidget_internalUpCB
	o.isHover=false
	o.changeTooltip=IconWidget_changeTooltip
	o.changeText=IconWidget_changeText
	o.changeImg=IconWidget_changeImg
	o.setDisabled=IconWidget_setDisabled
	o.isDisabled=IconWidget_isDisabled
	o.acceptClick=IconWidget_acceptClick
	_allBOIcons[o.index]=o
	o.outEnable=true
	o.setCrs=IconWidget_setCrs
	
	o.oldRes=o.resize
	o.resize=IconWidget_resize
	
	o.iconOldInit=o.init
	o.init=IconWidget_init
	
	return o
}

// ================================================================================
// ================================================================================
//
// OBJECT newIconMenuWidget (Constructor)
//
// An icon widget that add an arrow part at the right. when clicking on the
// right part, a menu pops up
// If no callback is attached, the also pops up when clicking on the left part
// menu.js must be included to use this component
//
// ================================================================================
// ================================================================================

function newIconMenuWidget(id,src,clickCB,text,alt,w,h,dx,dy,disDx,disDy,isColor,beforeShowCB,menuType)
// CONSTRUCTOR
// id       : [String]              the icon id for DHTML processing
// src      : [String]              the icon image url
// clickCB  : [Function - optional] callback called when clicking
// text     : [String - optional]   icon  text
// alt      : [String]              icon tooltip
// w        : [int]                 visible image width
// h        : [int]                 visible image height
// dx       : [int - optional]      horizontal offset in the image
// dy       : [int - optional]      vertical offset in the image
// disDx    : [int - optional]      horizontal offset for disabled state
// disDy    : [int - optional]      vertical offset for disabled state
{
	var o=newWidget(id)
	
	if ( typeof(menuType) == 'undefined' )
		menuType=isColor?_menuType_color:_menuType_simple
		
	o.menuItemType = isColor? _isColor:_isNotColor;
	var tooltip = _openMenu.replace("{0}", (text?text:(alt?alt:"")));
	o.icon=newIconWidget("iconMenu_icon_"+id,src,IconMenuWidget_iconClickCB,text,alt,w,h,dx,dy,disDx,disDy,false)
	o.arrow=newIconWidget("iconMenu_arrow_"+id,_skin+"menus.gif",IconMenuWidget_arrowClickCB,null,tooltip,7,16,0,81,0,97,true)
	switch (menuType)
	{
		case _menuType_color:
			o.menu=newMenuColorWidget("iconMenu_menu_"+id,IconMenuWidget_hideCB)
			break
		case _menuType_border:
			o.menu=newMenuBordersWidget("iconMenu_menu_"+id,IconMenuWidget_hideCB,beforeShowCB,IconBordersMenuWidget_internalClickCB)
			break
		default:
		case _menuType_simple:
			o.menu=newMenuWidget("iconMenu_menu_"+id,IconMenuWidget_hideCB,beforeShowCB)
			break
	}	

	o.icon.par=o
	o.arrow.par=o
	o.menu.parIcon=o

	o.icon.margin=0
	o.arrow.margin=0

	o.icon.overCB="IconWidget_none"
	o.icon.outCB="IconWidget_none"
	o.arrow.overCB="IconWidget_none"
	o.arrow.outCB="IconWidget_none"

	o.margin=1
	o.spc=0

	o.getHTML=IconMenuWidget_getHTML
	o.clickCB=clickCB
	o.getMenu=IconMenuWidget_getMenu

	o.menIcnOldInit=o.init
	o.init=IconMenuWidget_init
	o.removeAllMenuItems=IconMenuWidget_removeAllMenuItems;

	o.index=_allBOIconsMenus.length++
	_allBOIconsMenus[o.index]=o	
	
	o.setDisabled=IconMenuWidget_setDisabled
	o.isDisabled=IconMenuWidget_isDisabled
	o.disableMenu=IconMenuWidget_disableMenu
	o.changeText=IconMenuWidget_changeText
	
	o.imwpResize=o.resize
	o.resize=IconMenuWidget_resize
	o.focus=IconMenuWidget_focus
	
	o.changeArrowTooltip=IconMenuWidget_changeArrowTooltip
	
	o.disp=true
	o.isDisplayed=IconWidget_isDisplayed
	o.setDisplay=IconWidget_setDisplay

	
	return o
}

function IconMenuWidget_removeAllMenuItems()
{
    this.menu.removeAll();
    this.menu.resetItemCount();
}

// ================================================================================

function IconMenuWidget_changeText(s)
// Change the icon text
// s [String] : the new text
{
	this.icon.changeText(s)
}


// ================================================================================

function IconMenuWidget_changeArrowTooltip(tooltip)
{
	this.arrow.changeTooltip(tooltip,false);
}

// ================================================================================
function IconMenuWidget_resize(w,h)
{
	var o=this
	if (w!=null)
		w=Math.max(0,w-2*o.margin)

	var d=o.layer.display!="none"

	if (d&_moz&&!_saf)
		o.setDisplay(false)

	o.imwpResize(w,h)
	if (w!=null)
		o.icon.resize(Math.max(0,w-13-o.spc))

	if (d&_moz&&!_saf)
		o.setDisplay(true)
}

// ================================================================================

function IconMenuWidget_setDisabled(dis)
{
	var o=this
	if (dis) {
		if (o.menu.isShown()) {
			o.menu.show(false)	
		}
		IconMenuWidgetOutCB(o.index)			
	}	
	o.icon.setDisabled(dis)
	o.arrow.setDisabled(dis)
}

// ================================================================================

function IconMenuWidget_isDisabled()
{
	return (this.icon.dis == true)	
}
// ================================================================================

function IconMenuWidget_internalCB()
{
	var o=this,col=null

	if (o.id!=null)
	{
		col = (o.menuItemType != _isLastUsedColor)? o.id.slice(6) : o.color // "color".length=6
	}	

	var icon=o.par.parIcon

	icon.oldColor=icon.curColor
	icon.curColor=col
	
	if (icon.curColor!=null)
		icon.showSample()
	
	if (icon.clickColor)
		icon.clickColor()
}
// ================================================================================
function IconMenuWidget_focus()
{
	var o=this
	o.arrow.focus()	
}

// ================================================================================
function IconMenuWidget_disableMenu(b)
{
	var o=this
	o.arrow.setDisabled(b)
	o.menu.setDisabled(b)
}

// ================================================================================

function IconMenuWidget_getMenu()
{
	return this.menu
}

// ================================================================================

function IconWidget_none()
{

}

// ================================================================================

function IconMenuWidget_init()
{
	var o=this
	o.menIcnOldInit()
	o.icon.init()
	o.arrow.init()
	o.menu.init()
	var l=o.layer
	l.onmouseover=IconMenuWidget_OverCB
	l.onmouseout=IconMenuWidget_OutCB
}

// ================================================================================

function IconMenuWidget_getHTML()
{
	var o=this,d=o.disp?"":"display:none;"

	return '<table id="'+o.id+'" cellspacing="0" cellpadding="0" border="0" style="'+d+'margin:'+o.margin+'px"><tr><td>'+o.icon.getHTML()+'</td><td style="padding-left:'+o.spc+'px" width="'+(13+o.spc)+'">'+o.arrow.getHTML()+'</td></table>'
}

// ================================================================================

function IconMenuWidget_OverCB()
{
	IconMenuWidgetOverCB(getWidget(this).index)
	return true
}

// ================================================================================

function IconMenuWidget_OutCB()
{
	IconMenuWidgetOutCB(getWidget(this).index)
}

// ================================================================================

function IconMenuWidgetOverCB(i)
{
	o=_allBOIconsMenus[i]
	IconWidget_overCB(o.icon.index)
	IconWidget_overCB(o.arrow.index)
}

// ================================================================================

function IconMenuWidgetOutCB(i)
{
	o=_allBOIconsMenus[i]
	if (!o.menu.isShown())
	{
		IconWidget_outCB(o.icon.index)
		IconWidget_outCB(o.arrow.index)
	}
	else
	{
		IconWidget_overCB(o.icon.index)
		IconWidget_overCB(o.arrow.index)
	}	
}

// ================================================================================

function IconMenuWidget_iconClickCB()
{
	var o=this.par
	if (o.clickCB==null)
	{
		var l=o.layer
		var position = getPos2(l);
		o.menu.show(!o.menu.isShown(),position.x,position.y+o.getHeight()+1,null,null,o)
		IconMenuWidgetOverCB(o.index)
	}
	else
		o.clickCB()
}

// ================================================================================

function IconMenuWidget_arrowClickCB()
{
	var o=this.par,l=o.layer;
	var position = getPos2(l);
	o.menu.show(!o.menu.isShown(),position.x,position.y+o.getHeight()+1,null,null,o)
	IconMenuWidgetOverCB(o.index)
}

// ================================================================================

function IconMenuWidget_hideCB()
{
	var o=this.parIcon
	if(o.arrow) {
		o.arrow.focus();
	}
	IconMenuWidgetOutCB(o.index)
}

//================================================================================
//================================================================================
//
//OBJECT newSingleIconMenuWidget (Constructor)
//
//An icon widget that add an arrow part at the right. When clicking on either the
//right part or the left part, a menu pops up. Behaves like a single button.
//menu.js must be included to use this component
//
//================================================================================
//================================================================================

function newSingleIconMenuWidget(id,src,clickCB,text,alt,w,h,dx,dy,disDx,disDy,isColor,beforeShowCB)
//CONSTRUCTOR
//id       : [String]              the icon id for DHTML processing
//src      : [String]              the icon image url
//clickCB  : [Function - optional] callback called when clicking
//text     : [String - optional]   icon  text
//alt      : [String]              icon tooltip
//w        : [int]                 visible image width
//h        : [int]                 visible image height
//dx       : [int - optional]      horizontal offset in the image
//dy       : [int - optional]      vertical offset in the image
//disDx    : [int - optional]      horizontal offset for disabled state
//disDy    : [int - optional]      vertical offset for disabled state
{
    var tooltip = _openMenu.replace("{0}", (text?text:(alt?alt:"")));
    
    var o=newIconWidget(id,src,SingleIconMenuWidget_clickCB,null,tooltip,w,h,dx,dy,disDx,disDy,true);
 
    o.icon=newIconWidget("singleIconMenu_icon_"+id,src,null,text,null,w,h,dx,dy,disDx,disDy,false);
    o.arrow=newIconWidget("singleIcon_arrow_"+id,_skin+"menus.gif",SingleIconMenuWidget_iconClickCB,null,tooltip,7,16,0,81,0,97,false);
    o.menu=newMenuWidget("singleIconMenu_menu_"+id,SingleIconMenuWidget_hideCB,beforeShowCB);

    o.icon.par=o;
    o.arrow.par=o;
    o.menu.parIcon=o;

    o.icon.margin=0;
    o.arrow.margin=0;

    o.icon.overCB="IconWidget_none";
    o.icon.outCB="IconWidget_none";
    o.arrow.overCB="IconWidget_none";
    o.arrow.outCB="IconWidget_none";

    o.margin=1;
    o.spc=0;

    o.getHTML=SingleIconMenuWidget_getHTML;
    o.getMenu=IconMenuWidget_getMenu;

    o.menIcnOldInit=o.init;
    o.init=SingleIconMenuWidget_init;
    o.removeAllMenuItems=IconMenuWidget_removeAllMenuItems;

    o.index=_allBOIconsMenus.length++;
    _allBOIconsMenus[o.index]=o; 
 
    o.setDisabled=IconMenuWidget_setDisabled;
    o.isDisabled=IconMenuWidget_isDisabled;
    o.disableMenu=IconMenuWidget_disableMenu;
    o.changeText=IconMenuWidget_changeText;
 
    o.imwpResize=o.resize;
    o.resize=IconMenuWidget_resize;
 
    o.changeArrowTooltip=IconMenuWidget_changeArrowTooltip;
 
    o.disp=true;
    o.isDisplayed=IconWidget_isDisplayed;
    o.setDisplay=IconWidget_setDisplay;

    return o;
}

//================================================================================

function SingleIconMenuWidget_init()
{
    var o=this;
    o.menIcnOldInit();
    o.menu.init();
    
    var l=o.layer;
    l.onmouseover=SingleIconMenuWidget_OverCB;
    l.onmouseout=SingleIconMenuWidget_OutCB;
}

//================================================================================

function SingleIconMenuWidget_getHTML()
{
    var o=this,d=o.disp?"":"display:none;";

    return '<table id="'+o.id+'" cellspacing="0" cellpadding="0" border="0" style="'+d+'margin:'+o.margin+'px" role="button"><tr>'+
           '<td class="singleIconMenuL"></td>'+
           '<td class="singleIconMenuM">'+o.icon.getHTML()+'</td>'+
           '<td class="singleIconMenuM" style="padding-left:'+o.spc+'px" width="'+(13+o.spc)+'">'+o.arrow.getHTML()+'</td>'+
           '<td class="singleIconMenuR"></td>'+
           '</tr></table>';
}

//================================================================================

function SingleIconMenuWidget_OverCB()
{
    SingleIconMenuWidgetOverCB(getWidget(this).index)
    return true
}

//================================================================================

function SingleIconMenuWidget_OutCB()
{
    SingleIconMenuWidgetOutCB(getWidget(this).index)
}

//================================================================================

function SingleIconMenuWidgetOverCB(i)
{
    o=_allBOIconsMenus[i];
    IconWidget_overCB(o.index);
}

//================================================================================

function SingleIconMenuWidgetOutCB(i)
{
    o=_allBOIconsMenus[i];
    if (!o.menu.isShown())
    {
        IconWidget_outCB(o.index);
    }
    else
    {
        IconWidget_overCB(o.index);
    }
}

//================================================================================

function SingleIconMenuWidget_clickCB()
{
    var o=this,l=o.layer;
    var position = getPos2(l);
    o.menu.show(!o.menu.isShown(),position.x,position.y+o.getHeight()+1,null,null,o);
    SingleIconMenuWidgetOverCB(o.index);
}

//================================================================================

function SingleIconMenuWidget_iconClickCB()
{
    // do nothing
    
    // this is needed to show the hand cursor on the arrow icon widget
}

//================================================================================

function SingleIconMenuWidget_hideCB()
{
    var o=this.parIcon;
    o.focus();
    SingleIconMenuWidgetOutCB(o.index);
}

// ================================================================================
// ================================================================================
//
// OBJECT newRadioIconMenuWidget (Constructor)
//
// like an Icon Widget but behave like radio widget except the button icon only is checked or not
// menu.js must be included to use this component
//
// ================================================================================
// ================================================================================
/*
function newRadioIconMenuWidget(id,src,clickCB,text,alt,w,h,dx,dy,disDx,disDy,radioIdx,attachMenuCB)
// CONSTRUCTOR
// id       : [String]              the icon id for DHTML processing
// src      : [String]              the icon image url
// clickCB  : [Function - optional] callback called when clicking
// text     : [String - optional]   icon  text
// alt      : [String]              icon tooltip
// w        : [int]                 visible image width
// h        : [int]                 visible image height
// dx       : [int - optional]      horizontal offset in the image
// dy       : [int - optional]      vertical offset in the image
// disDx    : [int - optional]      horizontal offset for disabled state
// disDy    : [int - optional]      vertical offset for disabled state
// radioIdx : [int - optional]      the index of the selected menuItem
// attachMenuCB : [Function - optional] callback change dynamically the menu
{
	var o=newIconMenuWidget(id,src,clickCB,text,alt,w,h,dx,dy,disDx,disDy,false)
	o.defDx=dx
	o.defDy=dy
	o.attachMenuCB=null	
	if (attachMenuCB) {
		o.attachMenuCB=attachMenuCB
		o.arrow.clickCB=RadioIconMenuWidget_arrowClickCB
	}	
	o.attachMenu=RadioIconMenuWidget_attachMenu
	o.updateButton=RadioIconMenuWidget_updateButton
	o.radioIdx=radioIdx?radioIdx:-1
	o.checked=radioIdx?true:false
	return o
}


// ================================================================================

function RadioIconMenuWidget_arrowClickCB()
{
	var o=this.par,l=o.layer
	o.attachMenuCB()
	o.menu.show(!o.menu.isShown(),getPosScrolled(l).x,getPosScrolled(l).y+o.getHeight()+1,null,null,o)
	IconMenuWidgetOverCB(o.index)
}

function RadioIconMenuWidget_updateButton(idx)
{
	var o=this
	o.icon.dis=false
	if ((idx != null) &&  (idx > 0)) {		
		if (o.radioIdx == idx) return
		o.radioIdx=idx
		o.checked=true		
		dx=idx*16
		dy=0
		cn=o.icon.checkClass				
	} else {
		o.checked=false
		dx=16
		dy=0
		cn=o.icon.nocheckClass
		o.radioIdx=0	
	}
	
	if (o.icon)
	{
		var lyr=o.icon.layer
		if (lyr)
		{
			o.icon.changeImg(dx,dy)
			lyr.className=cn			
		}
	}
	if (idx==null) o.setDisabled(true)
	//o.icon.
}

// ================================================================================

function RadioIconMenuWidget_attachMenu(menu)
// menu [MenuWidget] the menu to be attached
// return [MenuWidget] the menu to be attached
{
	var o=this
	o.par=null
	o.menu=menu
	menu.par=null
	menu.container=o	
	//menu.par=o
	//menu.zIndex=o.par.zIndex+2

	return menu
}

// ================================================================================
// Icon Color Menu Widget
// ================================================================================

function newIconColorMenuWidget(id,src,clickCB,text,alt,w,h,dx,dy,disDx,disDy, lastUsedColorsAr)
{
	var o=newIconMenuWidget(id,src,clickCB,text,alt,w,h,dx,dy,disDx,disDy,true)
	
	o.setColor=IconColorMenuWidget_setColor
	o.getColor=IconColorMenuWidget_getColor
	
	o.icon.extraHTML='<div id="colSample_'+o.id+'" style="position:relative;top:-6px;left:2px;width:16px;height:4px;overflow:hidden;margin:0px"></div>'

	o.curColor=null
	o.sampleLayer=null
	
	o.clickColor=clickCB
	o.clickCB=null
	
	o.colOldSetDis=o.setDisabled
	o.setDisabled=IconColorMenuWidget_setDisabled
	o.showSample=IconColorMenuWidget_showSample
	o.changeTooltip=IconColorMenuWidget_changeTooltip

	var m=o.menu
	m.ac=m.addColor
	var cb=IconMenuWidget_internalCB
	
	var c1=m.addCheck("color_-1,-1,-1",_default,cb)
	c1.leftZoneClass="menuLeftPartColor"
	c1.leftZoneSelClass="menuLeftPartSel"

	_colorsArr = new Array()
	
	_colorsArr["0,0,0"]=_black
	
	_colorsArr["148,52,0"]=_brown
	_colorsArr["49,52,0"]=_oliveGreen
	_colorsArr["0,52,0"]=_darkGreen
	_colorsArr["0,52,99"]=_darkTeal
	_colorsArr["0,0,132"]=_navyBlue
	_colorsArr["49,52,148"]=_indigo
	_colorsArr["66,65,66"]=_darkGray

	_colorsArr["132,4,0"]=_darkRed
	_colorsArr["255,101,0"]=_orange
	_colorsArr["123,125,0"]=_darkYellow
	_colorsArr["0,125,0"]=_green
	_colorsArr["0,125,123"]=_teal
	_colorsArr["0,0,255"]=_blue
	_colorsArr["99,101,148"]=_blueGray
	_colorsArr["132,130,132"]=_mediumGray
	
	_colorsArr["255,0,0"]=_red
	_colorsArr["255,195,66"]=_lightOrange
	_colorsArr["148,199,0"]=_lime
	_colorsArr["49,150,99"]=_seaGreen
	_colorsArr["49,199,198"]=_aqua
	_colorsArr["49,101,255"]=_lightBlue
	_colorsArr["132,0,132"]=_violet
	_colorsArr["148,150,148"]=_gray

	_colorsArr["255,0,255"]=_magenta
	_colorsArr["255,199,0"]=_gold
	_colorsArr["255,255,0"]=_yellow
	_colorsArr["0,255,0"]=_brightGreen
	_colorsArr["0,255,255"]=_cyan
	_colorsArr["0,199,255"]=_skyBlue
	_colorsArr["148,52,99"]=_plum
	_colorsArr["198,195,198"]=_lightGray

	_colorsArr["255,178,181"]=_pink
	_colorsArr["255,199,148"]=_tan
	_colorsArr["255,255,206"]=_lightYellow
	_colorsArr["206,255,206"]=_lightGreen
	_colorsArr["206,255,255"]=_lightTurquoise
	_colorsArr["148,199,255"]=_paleBlue
	_colorsArr["198,150,255"]=_lavender
	_colorsArr["255,255,255"]=_white
			
	with (m)
	{
		for (var i in _colorsArr) {
			ac(_colorsArr[i],i,cb)
		}	
	}

	if (lastUsedColorsAr) 
	{
		m.addLastUsed(_lastUsed,lastUsedColorsAr, cb)
	}
	c1=m.add(null,_moreColors,cb)
	c1.leftZoneClass="menuLeftPartColor"
	c1.leftZoneSelClass="menuLeftPartSel"
	
	return o
}

// ================================================================================

function IconColorMenuWidget_setColor(color)
{
	var o=this,menu=o.menu
	menu.uncheckAll()
	o.curColor=color
	
	if (color!=null)
	{
		var id="color_"+color
		var items=menu.items
	
		for (var i in items)
		{
			var item=items[i]
			if ((item.menuItemType && (item.menuItemType == _isLastUsedColor) && (color == menu.lastUsedColorsAr[item.idx])) || (item.id==id)){
				item.check(true)
				break
			}
		}	
	}
	o.showSample()
}

// ================================================================================

function IconColorMenuWidget_showSample()
{
	var o=this
	if (o.layer)
	{
		if (o.sampleLayer==null)		
			o.sampleLayer=getLayer('colSample_'+o.id)
			
		o.sampleLayer.style.backgroundColor=((o.curColor!='-1,-1,-1')&&(o.curColor!=null)) ? 'rgb('+o.curColor+')' : ''
	}	
}

// ================================================================================

function IconColorMenuWidget_setDisabled(dis)
{
	this.colOldSetDis(dis)
	if (this.layer)
		this.showSample()
}

// ================================================================================

function IconColorMenuWidget_getColor()
{
	return this.curColor
}

// ================================================================================

function IconColorMenuWidget_changeTooltip(s)
{
	var o=this
	
	if (s==null) return;
		
	if (o.icon && o.arrow) {
		o.icon.alt=s;
		o.icon.changeTooltip(s);
		var tooltip = _openMenu.replace("{0}", s);
		o.arrow.changeTooltip(tooltip)
	}
}

*/
// ================================================================================
// ================================================================================
//
// OBJECT newIconCheckWidget (Constructor)
//
// Display an icon with check box behaviour. It can be used in a palette for
// instance. The image can be a sub-part of a bigger image 
// (use dx and dy to define offset)
//
// ================================================================================
// ================================================================================

function newIconCheckWidget(id,src,clickCB,text,alt,w,h,dx,dy,disDx,disDy)
// CONSTRUCTOR
// id       : [String]              the icon id for DHTML processing
// src      : [String]              the icon image url
// clickCB  : [Function - optional] callback called when clicking
// text     : [String - optional]   icon  text
// alt      : [String]              icon tooltip
// w        : [int]                 visible image width
// h        : [int]                 visible image height
// dx       : [int - optional]      horizontal offset in the image
// dy       : [int - optional]      vertical offset in the image
// disDx    : [int - optional]      horizontal offset for disabled state
// disDy    : [int - optional]      vertical offset for disabled state
{
	var o=newIconWidget(id,src,clickCB,text,alt,w,h,dx,dy,disDx,disDy)
	o.checked=false
	o.internalUpCB=IconCheckWidget_internalUpCB
	o.internalDownCB=IconCheckWidget_internalDownCB
	o.check=IconCheckWidget_check
	o.isChecked=IconCheckWidget_isChecked	
	//o.setIcon=IconCheckWidget_setIcon
	
	o.oldInit=o.init
	o.init=IconCheckWidget_init

	o.isRadio=false

	return o
}



// ================================================================================
// ================================================================================
//
// OBJECT newIconToggleWidget (Constructor)
//
// Display an icon with toggle behaviour, ie. if selected  a different images is shown. It can be used in a palette for
// instance. The image can be a sub-part of a bigger image 
// (use dx and dy to define offset)
//
// ================================================================================
// ================================================================================
/*
function newIconToggleWidget(id,src,clickCB,text,alt,w,h,dx,dy,togX,togY,disDx,disDy)
// CONSTRUCTOR
// id       : [String]              the icon id for DHTML processing
// src      : [String]              the icon image url
// clickCB  : [Function - optional] callback called when clicking
// text     : [String - optional]   icon  text
// alt      : [String]              icon tooltip
// w        : [int]                 visible image width
// h        : [int]                 visible image height
// dx       : [int]					 horizontal offset in the image
// dy       : [int]				     vertical offset in the image
// togX     : [int]					horizontal offset 2nd image
// togY     : [int]					vertical offset 2nd image
// disDx    : [int - optional]      horizontal offset for disabled state
// disDy    : [int - optional]      vertical offset for disabled state
{
	var o=newIconWidget(id,src,clickCB,text,alt,w,h,dx,dy,disDx,disDy)
	o.togX = togX
	o.togY = togY
	o.checked=false
	o.internalUpCB=IconToggleWidget_internalUpCB
	o.internalDownCB=IconToggleWidget_internalDownCB
	o.check=IconToggleWidget_check
	o.isChecked=IconCheckWidget_isChecked	
	//o.setIcon=IconCheckWidget_setIcon
	
	o.oldInit=o.init
	o.init=IconCheckWidget_init

	o.isRadio=false

	return o
}

// ================================================================================
// ================================================================================
//
// OBJECT newIconRadioWidget (Constructor)
//
// Display an icon with radio button behaviour. It can be used in a palette for
// instance. The image can be a sub-part of a bigger image 
// (use dx and dy to define offset)
//
// ================================================================================
// ================================================================================

function newIconRadioWidget(id,src,clickCB,text,alt,group,w,h,dx,dy,disDx,disDy)
// CONSTRUCTOR
// id       : [String]              the icon id for DHTML processing
// src      : [String]              the icon image url
// clickCB  : [Function - optional] callback called when clicking
// text     : [String - optional]   icon  text
// alt      : [String]              icon tooltip
// group    : [String]              icon group - select an icon in a group deselects
//                                  the other icons
// w        : [int]                 visible image width
// h        : [int]                 visible image height
// dx       : [int - optional]      horizontal offset in the image
// dy       : [int - optional]      vertical offset in the image
// disDx    : [int - optional]      horizontal offset for disabled state
// disDy    : [int - optional]      vertical offset for disabled state
{
	var o=newIconCheckWidget(id,src,clickCB,text,alt,w,h,dx,dy,disDx,disDy)
	o.group=group
	o.beforeClickCB=IconRadioWidget_uncheckOthers
	o.isRadio=true

	if (_RadioWidget_groups[group]==null)
		_RadioWidget_groups[group]=new Array
	o.groupInstance=_RadioWidget_groups[group]
	var g=o.groupInstance
	o.groupIdx=g.length
	g[g.length]=o

	return o
}
*/
// ================================================================================
// ================================================================================
//
// OBJECT newPaletteContainerWidget (Constructor)
//
// This object is a container for multiple palettes
//
// ================================================================================
// ================================================================================

function newPaletteContainerWidget(id,contextMenu,margin)
// CONSTRUCTOR
// id : [String] the icon id for DHTML processing
{
	var o=newWidget(id)
	o.beginHTML=PaletteContainerWidget_beginHTML
	o.endHTML=PaletteContainerWidget_endHTML
	o.add=PaletteContainerWidget_add
	o.palettes=new Array
	o.contextMenu=contextMenu
	o.margin=(margin!=null)?margin:4
	return o
}

// ================================================================================
// ================================================================================
//
// OBJECT newPaletteWidget (Constructor)
//
// This object is an horizontal palette, designed to be uses in a palette
// container (see newPaletteContainerWidget)
//
// ================================================================================
// ================================================================================

function newPaletteWidget(id,height)
// CONSTRUCTOR
// id : [String] the icon id for DHTML processing
// height [int - optional] : minimum palette height
{
	var o=newWidget(id)
	o.getHTML=PaletteWidget_getHTML
	o.add=PaletteWidget_add
	o.disableChildren=PaletteWidget_disableChildren
	o.items=new Array
	o.oldInit=o.init
	o.init=PaletteWidget_init
	o.beginRightZone=PaletteWidget_beginRightZone

	o.height=height

	o.rightZoneIndex=-1
	
	o.sepCount=0
	o.vertPadding=4
	o.isLeftTableFixed=false
	
	return o
}

// ================================================================================
// ================================================================================
//
// OBJECT newPaletteSepWidget (Constructor)
//
// This object is a vertical separator used in a palette
//
// ================================================================================
// ================================================================================

/*
function newPaletteSepWidget(id)
// CONSTRUCTOR
// id : [String] the icon id for DHTML processing
{
	var o=newWidget(id)
	o.getHTML=PaletteSepWidget_getHTML
	o.isSeparator=true
	return o
}
*/
// ================================================================================
function newPaletteVerticalSepWidget(id)
// CONSTRUCTOR
// id : [String] the icon id for DHTML processing
{
	var o=newWidget(id)
	o.getHTML=PaletteVerticalSepWidget_getHTML
	o.isSeparator=true
	return o
}


function PaletteVerticalSepWidget_getHTML()
{
	return img(_skin+"iconsep.gif",6,22,null,' id="'+this.id+'" ')
}
// ================================================================================

function getPaletteSep()
// returns [String] the HTML of a vertical palette separator
{
	return img(_skin+"iconsep.gif",6,22)
}

// ================================================================================

function IconRadioWidget_uncheckOthers()
// PRIVATE
{
	var g=this.groupInstance,idx=this.groupIdx,len=g.length;
	for (var i=0;i<len;i++)
	{
		if (i!=idx)
		{
			var c=g[i];
			if(c)
				c.check(false);
		}
	}
}

// ================================================================================

function PaletteWidget_beginRightZone()
// begin the right zone of a palette. All added element will be right-aligned
// after this call. This function must be called once in a giver palette
{
	this.rightZoneIndex=this.items.length
}

// ================================================================================

function PaletteSepWidget_getHTML()
// returns [String] the HTML of a palette separator
{
	return '<div style="background-image:url('+_skin+'sep.gif);height:2px;padding:0px;margin-top:0px;margin-bottom:0px;margin-left:4px;margin-right:4px">'+getSpace(1,2)+'</div>'
}

// ================================================================================

function PaletteContainerWidget_beginHTML()
// returns [String] the begining HTML of a palette container widget
{
	var o=this
	var cm=o.contextMenu?('oncontextmenu="'+_codeWinName+'.PaletteContainerWidget_contextMenu(this,event);return false"'):''

	return '<div '+cm+ 'class="palette" style="overflow:hidden;margin:'+o.margin+'px;" id="'+o.id+'">'
}

_delayedMenu=null

// ================================================================================

function PaletteContainerWidget_contextMenu(o,e)
// PRIVATE
{
	if (_ie)
		e=_curWin.event
	_delayedMenu=getWidget(o).contextMenu
	setTimeout('_delayedMenu.par=null;_delayedMenu.show(true,'+absxpos(e)+','+absypos(e)+')',1)
}

// ================================================================================

function PaletteContainerWidget_endHTML()
// returns [String] the end HTML of a palette container widget
{
	return '</div>'
}

// ================================================================================

function PaletteContainerWidget_add(palette)
// add a palette in a palette container widget. The palette is horizontal
// and will be displayed at the bottom of the container
// return [String] the HTML
{
	this.palettes[this.palettes.length]=palette
	return palette
}

// ================================================================================

function PaletteWidget_getHTML()
// return [String] the HTML
{
	var o=this,items=o.items,len=items.length,fields=new Array;j=0
	
	fields[j++]='<table style="position:relative;overflow:hidden" id="'+o.id+'" '+attr("height",o.height)+' cellspacing="0" cellpadding="0" width="100%"><tbody><tr valign="middle">'
	fields[j++]='<td width="100%" align="left" style="padding-left:'+o.vertPadding+'px;padding-right:4px"><table cellspacing="0" cellpadding="0"'+(o.isLeftTableFixed?'style="table-layout:fixed;width:100%"':"")+'><tbody><tr valign="middle">'
	
	var haveRightZone=false
	for (var i=0;i<len;i++)
	{
		if (i==o.rightZoneIndex)
		{
			fields[j++]='</tr></tbody></table></td><td align="right" style="padding-right:'+o.vertPadding+'px"><table cellspacing="0" cellpadding="0"><tbody><tr valign="middle">'
			haveRightZone=true
		}
	
		var it=items[i]
		//fields[j++]='<td>'+(it?it.getHTML():getPaletteSep())+'</td>'
		fields[j++]='<td>'+it.getHTML()+'</td>'
	}
	
	if (!haveRightZone)
		fields[j++]='</tr></tbody></table></td><td align="right" style="padding-right:4px"><table cellspacing="0" cellpadding="0"><tbody><tr valign="middle"><td></td>'
	
	fields[j++]='</tr></tbody></table></td></tr></tbody></table>'
	return fields.join("")
}

// ================================================================================

function PaletteWidget_add(item)
// add an item into a palette. this element is a standard widget, with a valid
// getHTML function. For layout reasons, use only widget that are nor too high
// palette [PaletteWidget}
{
	if(item==null)
	{
		item=newPaletteVerticalSepWidget(this.id+"_palettesep_"+(this.sepCount++))
	}
	this.items[this.items.length]=item
	return item
}

// ================================================================================


function PaletteWidget_disableChildren(dis)
{
	var items=this.items
	for (var i in items)
	{
		var item=items[i]
		if (item&&(item.isSep!=true))
			item.setDisabled(dis)
	}
}


// ================================================================================

function PaletteWidget_init()
// inits a palette widget (ie get all layers, style for dynamic management)
// return [String] the HTML
{
	this.oldInit()
	var items=this.items
	for (var i in items)
	{
		var item=items[i]
		if (item)
			item.init()
	}
}

// ================================================================================

function IconWidget_isDisplayed()
{
	return this.disp
}

// ================================================================================

function IconWidget_setDisplay(d)
{
	var o=this
	
	if (o.css)
	{
		var ds=d?"block":"none"
		if (o.css.display!=ds)
			o.css.display=ds
	}
	o.disp=d
}

// ================================================================================

function IconWidget_getTxtWidth()
{
	var o=this,w=o.width
	if (w!=null)
	{
		w=w-(o.margin*2) // remove table margin
		w=w-(o.src?o.w+o.border:1) // Image size
		w=w-(o.txtNoPadding?0:((o.src?4:2)+2))
		
		if (_ie)
			w-=2
		else
			w-=2
		return Math.max(0,w)
	}
	else
		return -1
}

// ================================================================================

function IconWidget_init()
{
	var o=this,dblClick=false
	o.iconOldInit()
	var l=o.layer
			
	//manage focus for 508	
	l.tabIndex=o.dis?-1:0
	
	//manage tooltip for 508
	l.title=(o.alt?o.alt:(o.text?o.text:""))
	
	// attach callbacks
	if (o.clickCB)
	{
		l.onclick=IconWidget_upCB
		l.onmousedown=IconWidget_downCB		
		
		if (o.allowDblClick&&(_ie||_saf))
		{
			dblClick=true
			addDblClickCB(l,IconWidget_upCB)
		}
		l.onkeydown=IconWidget_keydownCB
		l.onmouseover=IconWidget_realOverCB
		l.onmouseout=IconWidget_realOutCB
	}

	if (!dblClick)
		addDblClickCB(l,IconWidget_retFalse)

	l.onselectstart=IconWidget_retFalse
	
	var d=o.disp?"block":"none"
	if (o.css.display!=d)
		o.css.display=d
}

// ================================================================================

function IconWidget_getHTML()
// returns [String] the HTML of an icon widget
{
	var o=this,imgCode=o.src?'<div style="overflow:hidden;height:'+(o.h+o.border)+'px;width:'+(o.w+o.border)+'px;cursor:'+(o.clickCB ? (!o.acceptClick() ? 'default' : _hand):'default')+'">'+simpleImgOffset(o.src,o.w,o.h,o.dis?o.disDx:o.dx,o.dis?o.disDy:o.dy,'IconImg_'+o.id,null,o.alt,'margin:2px;')+o.extraHTML+'</div>':'<div class="iconText" style="width:1px;height:'+(o.h+o.border)+'px"></div>'


	var txtAtt='style="white-space:nowrap;',txtW=o.getTxtWidth()
	if (txtW>=0)
		txtAtt+='text-overflow:ellipsis;overflow:hidden;width:'+txtW+'px'

	txtAtt+='"'
	
	var d=o.disp?"":"display:none;"
	
	return '<table style="'+d+'height:'+(o.h+o.border+(_moz?2:0))+'px;'+(o.width!=null?"width:"+o.width+"px;":"")+'margin:'+o.margin+'px" id="'+o.id+'"  class="' + o.nocheckClass + '" cellspacing="0" cellpadding="0" border="0" role="button" '+(o.ariaHasPopUp?'aria-haspopup="true"':'')+'><tr valign="middle">'+
			'<td>'+ ((o.clickCB&&_ie)?lnk(imgCode,null,null,null, ' tabIndex="-1"' ):imgCode)+'</td>'+
			(o.text?'<td align="'+o.txtAlign+'" style="padding-left:'+(o.txtNoPadding?0:(o.src?4:2))+'px;padding-right:'+(o.txtNoPadding?0:2)+'px"><div id="IconImg_Txt_'+o.id+'" class="iconText'+(o.dis?"Dis":"")+'" '+txtAtt+'>'+convStr(o.text)+'</div></td>':'')+
			'</tr></table>'
}

// ================================================================================

function IconWidget_realOutCB()
{
	var o=getWidget(this)
	eval(o.outCB+'('+o.index+')')
}

// ================================================================================

function IconWidget_realOverCB()
{
	var o=getWidget(this)
	eval(o.overCB+'('+o.index+')')
	return true
}

// ================================================================================

function IconWidget_retFalse()
// PRIVATE
{
	return false
}

// ================================================================================

function IconWidget_resize(w,h)
{
	var o=this
	
	if (o.layer)
		o.oldRes(w,h)
	
	if (o.txtLayer==null)
		o.txtLayer=getLayer("IconImg_Txt_"+o.id)
	
	if (w!=null)
	{
		o.width=w		
		var txtW=o.getTxtWidth()
		
		if (o.txtLayer&&(txtW>=0))
		{
			o.txtLayer.style.width=''+txtW+'px'			
		}		
	}
	if (h!=null)
	{		
		o.h=h?(h-o.border):o.h
		
		if (o.txtLayer&&(o.h>=0))
		{
			o.txtLayer.style.height=''+o.h+'px'
		}
		
	}
}

// ================================================================================

function IconWidget_changeTooltip(s,isTemporary)
// changes the tooltip of an icon widget
// s : [String] the new tooltip string.
{
	var o=this
	
	if (s==null) return;
	
	if(!isTemporary)
		o.alt=s;
	if(o.layer)
		o.layer.title=s
	if (o.imgLayer==null)
		o.imgLayer = getLayer('IconImg_'+this.id);
	if (o.imgLayer)	
		changeSimpleOffset(o.imgLayer,null,null,null,s)	
}

// ================================================================================

function IconWidget_changeText(s)
// Change the icon text
// s [String] : the new text
{
	var o=this
	o.text=s
	if (o.layer)
	{
		if (o.txtLayer==null)
			o.txtLayer=getLayer("IconImg_Txt_"+o.id)
		o.txtLayer.innerHTML=convStr(s)
	}
}

function IconWidget_changeImg(dx,dy,src)
{
	var o=this
	if (src) o.src=src
	if (dx!=null) o.dx=dx
	if (dy!=null) o.dy=dy

	if (o.layer&&(o.imgLayer==null))
		o.imgLayer = getLayer('IconImg_'+this.id);

	if (o.imgLayer)
		changeSimpleOffset(o.imgLayer,dx,dy,o.src)
}


// ================================================================================

function IconWidget_internalDownCB()
// PRIVATE
{
	if (!this.dis)
		this.currentHoverClass=this.checkhoverClass
}

// ================================================================================

function IconWidget_internalUpCB()
// PRIVATE
{
	if (!this.dis)
		this.currentHoverClass=this.hoverClass
}
// ================================================================================

function IconWidget_setCrs()
// PRIVATE
{
	var o=this,crs=(o.clickCB ? (!o.acceptClick() ? 'default' : _hand) : 'default')
	o.css.cursor=crs

	if (o.src)
	{
		if (o.imgLayer==null)
			o.imgLayer = getLayer('IconImg_'+o.id);
		if (o.imgLayer) o.imgLayer.style.cursor=crs
	}
}

// ================================================================================

function IconWidget_downCB()
// PRIVATE
{
	var o=getWidget(this)
	if ((o.layer)&&(o.acceptClick()))

	{
		o.internalDownCB()
		o.layer.className=o.currentHoverClass
		
		// If user mouse down on the icon of the one and only one menu
		// being shown (i.e. _globMenuCaptured), then release _globMenuCaptured
		// which prevent MenuWidget_globalClick in menu.js from closing the menu
		// which cause the menu to be re-open. TextComboWidget_clickCB,
		// IconMenuWidget_iconClickCB, and IconMenuWidget_arrowClickCB will properly
		// close the menu (toggle between open and close to be exact)
		if ((o.par != null && o.par.menu == _globMenuCaptured) || (o != null && o.menu && o.menu == _globMenuCaptured))
			MenuWidget_releaseGlobMenuCaptured()
	}
	if (_ie||_saf)
		return false
}

// ================================================================================

function IconWidget_upCB()
// PRIVATE
{	
	var o=getWidget(this)
	if ((o.layer)&&(o.acceptClick()))
	{
		o.internalUpCB()
		o.layer.className=o.isHover?o.currentHoverClass:o.currentClass
		o.setCrs()
		delayedClickCB(o.index);
	}
}
// ================================================================================
function IconWidget_keydownCB(e)
{	
	if(eventGetKey(e)==13 || eventGetKey(e)==32) //enter or spacebar
	{
		var o=getWidget(this)
		if ((o.layer)&&(o.acceptClick()))
                {
		    o.internalUpCB()
		    o.layer.className=o.isHover?o.currentHoverClass:o.currentClass
		    o.setCrs()
		    setTimeout("delayedClickCB("+o.index+")",1)
		}
		eventCancelBubble(e);//be careful ! usefull for dialog box close by Enter keypressed
	}
}

// ================================================================================

function delayedClickCB(index)
// PRIVATE
{
	var o=_allBOIcons[index]

	if (o.beforeClickCB)
		o.beforeClickCB()


	if (o.clickCB)
		o.clickCB()
}

// ================================================================================

function IconWidget_overCB(index)
// PRIVATE
{
	var o=_allBOIcons[index]
	
	o.setCrs()
	
	if ((o.layer)&&(!o.dis)&&!(o.par && o.par.checked)) // if not button container and it is checked
	{
		o.isHover=true
		o.layer.className=o.currentHoverClass
	}
}

// ================================================================================

function IconWidget_outCB(index)
// PRIVATE
{
	var o=_allBOIcons[index]
	if ((o.layer)&&(o.outEnable)&&!(o.par && o.par.checked)) // if not button container and it is checked
	{
		o.isHover=false
		o.layer.className=o.currentClass
	}
}

// ================================================================================

function IconCheckWidget_init()
// inits an icon check widget (ie get all layers, style for dynamic management)
{
	var o=this
	o.oldInit()
	o.check(o.checked,true)
}

// ================================================================================

function IconCheckWidget_internalDownCB()
// PRIVATE
{
	var o=this
	if (o.acceptClick())
		o.currentHoverClass=o.checked?o.hoverClass:o.checkhoverClass
}

// ================================================================================

function IconCheckWidget_internalUpCB()
// PRIVATE
{
	var o=this
	if (o.acceptClick())
	{
		o.checked=o.isRadio?true:!o.checked
		o.currentClass=o.checked?this.checkClass:this.nocheckClass
		o.currentHoverClass=o.checked?this.checkhoverClass:this.hoverClass
	}
}

/*
function IconToggleWidget_internalDownCB()
// PRIVATE
{
	var o=this
	if (o.acceptClick()) {
		//o.currentHoverClass=o.checked?o.hoverClass:o.checkhoverClass
	}	
}

// ================================================================================

function IconToggleWidget_internalUpCB()
// PRIVATE
{
	var o=this
	if (o.acceptClick())
	{
		if (o.layer&&(o.imgLayer==null))
			o.imgLayer = getLayer('IconImg_'+this.id);

		if (!o.imgLayer) return
		
		o.checked=!o.checked
		if (o.checked)
		{			
			changeSimpleOffset(o.imgLayer,o.togX,o.togY,o.src)
		} else {		
			changeSimpleOffset(o.imgLayer,o.dx,o.dy,o.src)			
		}
	}
}
*/

// ================================================================================

function IconCheckWidget_check(checked,force)
// check or uncheck an icon check widget
// check : [boolean] specified if the check icon must be checked or not
// force : [boolean - optional] for internal purpose, do not use it
{
	var o=this
	if ((o.checked!=checked)||force)
	{
		o.checked=checked
		
		if (o.layer)
		{
			o.layer.className=o.currentClass=o.checked?this.checkClass:this.nocheckClass
			o.currentHoverClass=o.checked?this.checkhoverClass:this.hoverClass
		}
	}
	
	if (o.checked&&o.beforeClickCB)
	{
		if (o.layer)
			o.beforeClickCB()
	}
}

// ================================================================================
/*
function IconToggleWidget_check(checked,force)
// check or uncheck an icon check widget
// check : [boolean] specified if the check icon must be checked or not
// force : [boolean - optional] for internal purpose, do not use it
{
	var o=this
	if ((o.checked!=checked)||force)
	{
		o.checked=checked
		if (o.layer&&(o.imgLayer==null))
			o.imgLayer = getLayer('IconImg_'+this.id);
		
		if (!o.imgLayer) return
			
		if (o.checked)
		{			
			changeSimpleOffset(o.imgLayer,o.togX,o.togY,o.src)
		} else {		
			changeSimpleOffset(o.imgLayer,o.dx,o.dy,o.src)			
		}
	}	
	
	if (o.checked&&o.beforeClickCB)
	{
		if (o.layer)
			o.beforeClickCB()
	}
}

*/
// ================================================================================

function IconCheckWidget_isChecked()
// return true if checked
{
	return this.checked
}

// ================================================================================

function IconWidget_setClasses(nocheck, check, hover, checkhover)
// set the 4 CSS classes for an icon : check/no check, hover/no
// nocheck    : [String] CSS class name for the no check state
// check      : [String] CSS class name for the check state
// hover      : [String] CSS class name for mouse over on a non checked icon
// checkhover : [String] CSS class name for mouse over on a checked icon
{
	var o=this
	o.nocheckClass=nocheck
	o.checkClass=check
	o.hoverClass=hover
	o.checkhoverClass=checkhover
	o.currentClass=o.nocheckClass
	o.currentHoverClass=o.hoverClass
}

// ================================================================================

function IconWidget_setDisabled(dis)
// set the widget disabled (ie insensitive to mouse or keyboard events)
// dis : [String] if true, disables the widget
{
 	var o=this
	if (o.dis!=dis)
	{
		o.dis=dis

		if (o.layer)
		{
			o.setCrs()

			if (o.src)
			{
				if (o.imgLayer==null)
					o.imgLayer = getLayer('IconImg_'+this.id);
				changeSimpleOffset(o.imgLayer,dis?o.disDx:o.dx,dis?o.disDy:o.dy)
			}
			if (o.text)
			{
				if (o.txtLayer==null)
					o.txtLayer=getLayer("IconImg_Txt_"+o.id)
				o.txtLayer.className="iconText"+(dis?"Dis":"")
	
				if (dis)
					o.layer.className=o.currentClass				
			}
			//manage focus for 508

			if (o.isTabEnabled)
				o.layer.tabIndex=o.dis?-1:0;
		}
	}
}

// return treu if the Widget is disabled
// 
function IconWidget_isDisabled()
{
	return this.dis?this.dis:false
}

function IconWidget_acceptClick()
{
	var o=this
	if (o.isDisabled()) return false
	if (o.isRadio&&o.checked) return false
	return true
}

// ================================================================================
// ================================================================================
//
// OBJECT newCustomCombo (Constructor)
//
// This object is a container for multiple palettes
//
// ================================================================================
// ================================================================================

function newCustomCombo(id,changeCB,noMargin,width,tooltip,url,w,h,dx,dy,disDx,disDy)
{
	var o=newIconMenuWidget(id,url,null," ",tooltip,w,h,dx,dy,disDx,disDy)
	o.icon.width=width!=null?Math.max(0,width-13):50-(2*o.margin)
	
	o.icon.setClasses("combonocheck", "combocheck", "combohover", "combocheck")
	o.icon.clip
	o.arrow.setClasses("iconnocheck", "combobtnhover", "combobtnhover", "combobtnhover")
	
	o.spc=0
	o.margin=2	
	
	if (url==null)
	{
		o.icon.h=12
		o.arrow.h=12
		o.arrow.dy+=2
		o.arrow.disDy+=2
	}
	
	// Private attributes
	o.counter=0
	o.changeCB=changeCB
	o.selectedItem=null
	o.setOldDid=o.setDisabled
	o.disabled=false
	o.ccomboOldInit=o.init
	

	// Public methods
	o.init=CustomCombo_init
	o.add=CustomCombo_add
	o.addSeparator=CustomCombo_addSeparator
	o.addMenuItem=CustomCombo_addMenuItem
	o.select=CustomCombo_select
	o.getSelection=CustomCombo_getSelection
	o.valueShow=CustomCombo_valueShow
	
	o.valueSelect=CustomCombo_valueSelect
	o.setUndefined=CustomCombo_setUndefined
	o.setDisabled=CustomCombo_setDisabled
	
	o.getVisibleItemsCount=CustomCombo_getVisibleItemsCount
	
	// Private methods
	o.selectItem=CustomCombo_selectItem
	o.getItemByIndex=CustomCombo_getItemByIndex
	o.getItemIndex=CustomCombo_getItemIndex
	o.setItemDisabled=CustomCombo_setItemDisabled

	return o
}

// ================================================================================

function CustomCombo_init()
{
	var o=this	
	
	o.ccomboOldInit()
	
	if(o.disabled) o.icon.changeTooltip(o.icon.alt?o.icon.alt:"",true);
	var arrowToolTip = _openMenu.replace("{0}", (o.icon.alt?o.icon.alt:""));
	o.arrow.changeTooltip(arrowToolTip);
}

function CustomCombo_add(s,val,selected)
{
	var o=this
	var item=o.menu.addCheck(o.id+"_it_"+(o.counter++),s,CustomCombo_internalCB)
	
	item.val=""+val
	item.parCombo=o
	item.isComboVal=true
	
	if ((o.selectedItem==null)||selected)
		o.selectItem(item)
}

// ================================================================================

function CustomCombo_addSeparator()
{
	this.menu.addSeparator()
}

// ================================================================================

function CustomCombo_addMenuItem(id,text,cb,icon,dx,dy,disabled,disDx,disDy)
{
	this.menu.add(id,text,cb,icon,dx,dy,disabled,disDx,disDy)
}

// ================================================================================

function CustomCombo_internalCB()
// PRIVATE
{
	var o=this,c=o.parCombo
	c.selectItem(o)

	if (c.changeCB)
		c.changeCB()
}

// ================================================================================

function CustomCombo_getItemByIndex(idx)
// PRIVATE
{
	var items=this.menu.items
	return ((idx>=0)&&(idx<items.length))?items[idx]:null
}

// ================================================================================

function CustomCombo_getItemIndex(item)
// PRIVATE
{
	var items=this.menu.items,len=items.length,j=0
	for (var i=0;i<len;i++)
	{
		var it=items[i]
		if (it.isComboVal)
		{
			if (it.id==item.id)
				return j
		
			j++
		}
	}
	return -1
}

// ================================================================================

function CustomCombo_selectItem(item)
// PRIVATE
{
	var o=this
	
	if (o.selectedItem)
		o.selectedItem.check(false)
	
	if (item)
	{
		o.val=item.val
		o.icon.changeText(o.disabled?"":item.text)		
		o.selectedItem=item
		item.check(true)
		if(o.disabled)
			o.icon.changeTooltip(o.icon.alt?o.icon.alt:"",true)		
		else
			o.icon.changeTooltip(o.icon.alt?(o.icon.alt+' ('+item.text)+')':(item.text),true)		
	}
	else
	{
		o.val=null
		o.icon.changeText("")
		o.icon.changeTooltip(o.icon.alt?o.icon.alt:"",true)		
		o.selectedItem=null
	}
}

// ================================================================================

function CustomCombo_setDisabled(d)
{
	var o=this
	if (o.selectedItem)
		o.icon.changeText(d?"":o.selectedItem.text)
	o.disabled=d
	o.setOldDid(d)
	if(d) o.icon.changeTooltip(o.icon.alt?o.icon.alt:"",true)
}

// ================================================================================

function CustomCombo_select(idx)
// Select an item in the combo
// idx : item index (only items added by "add" function are taken in account)
//       to allow menus combining combo items and menu action items
{
	var o=this,item=o.getItemByIndex(idx)
	if (item)
		o.selectItem(item)
}

// ================================================================================

function CustomCombo_setItemDisabled(idx,disabled)
// Disable a specific item on the combo box
// idx [int] item index (only items added by "add" function are taken in account)
// disabled [boolean] disable if true
{
	var o=this,item=o.getItemByIndex(idx)
	if (item)
		item.setDisabled(disabled)
}


// ================================================================================

function CustomCombo_getSelection()
{
	var o=this,it=o.selectedItem
	if (it)
		return {index:o.getItemIndex(it),value:it.val}
	else
		return null
}

// ================================================================================
//return true if selection can be done
function CustomCombo_valueSelect(v)
{	
	v=""+v
	var o=this,items=o.menu.items,len=items.length
	for (var i=0;i<len;i++)
	{
		var it=items[i]
		if ((it.isComboVal)&&(it.val==v)&&(it.isShown) )
		{
			o.selectItem(it)
			return true
		}
	}
	
	return false
}

// ================================================================================

function CustomCombo_valueShow(v,show)
{
	v=""+v
	var o=this,items=o.menu.items,len=items.length
	for (var i=0;i<len;i++)
	{
		var it=items[i]
		if ((it.isComboVal)&&(it.val==v))
		{
			it.show(show)
			return
		}
	}

}

// ================================================================================

function CustomCombo_setUndefined(u)
{
	var o=this
	if (u)
		o.selectItem(null)
}

// ================================================================================
//by default, return the count of shown elements of type isComboVal
function CustomCombo_getVisibleItemsCount()
{	
	var o=this,items=o.menu.items,len=items.length,n=0
	for (var i=0;i<len;i++)
	{
		var it=items[i]
		if ((it.isComboVal)&&(it.isShown))
		{
			n++
		}
	}
	return n;
}

// ================================================================================
// ================================================================================
//
// OBJECT newCustomTextFieldWidget (Constructor)
//
// This object is a Textfield followed by an IconMenuWidget
//
// ================================================================================
// ================================================================================

function newComboTextFieldWidget(id,changeCB,maxChar,keyUpCB,enterCB,noMargin,tooltip,width,focusCB,blurCB)
{
	var o=newTextFieldWidget(id,changeCB,maxChar,keyUpCB,enterCB,noMargin,tooltip,width,focusCB,blurCB)
	
	o.par=null
		
	o.oldInit=o.init
	o.init=ComboTextFieldWidget_init
	o.setContentEditable=ComboTextFieldWidget_setContentEditable
	o.isContentEditable=ComboTextFieldWidget_isContentEditable
	o.getHTML=ComboTextFieldWidget_getHTML
	o.oldSetDisabled=o.setDisabled
	o.setDisabled=ComboTextFieldWidget_setDisabled
	
	return o
}

// ================================================================================

function ComboTextFieldWidget_init()
{
	var o=this
	
	o.oldInit()
	
	var l=o.layer
	if (l!=null)
	{
		o.setContentEditable(true)
		l.onclick=ComboTextFieldWidget_onClick
	}
}

// ================================================================================

function ComboTextFieldWidget_setContentEditable(d)
{
	var o=this, l=o.layer
	o.contentEditable=d

	if (l)
	{
		if (_moz) // Fix for ADAPT01168940 didn't work on Firefox 3 and its somehow related to this
			l.readOnly = !d
		else
			l.contentEditable=d
			
		l.style.cursor=d?'text':_hand
		l.className=d?'comboEditable':'combo'
	}
}

// ================================================================================

function ComboTextFieldWidget_isContentEditable()
{
	var o=this
	return o.contentEditable
}

// ================================================================================

function ComboTextFieldWidget_onClick()
{
	var o=getWidget(this)
	
	if (o.contentEditable)
		return
		
	if (o.par!=null)
		o.par.clickCB()
}

// ================================================================================

function ComboTextFieldWidget_getHTML()
{
	var o=this	
	return '<input'+(o.disabled?' disabled':'')+' oncontextmenu="event.cancelBubble=true;return true" style="'+sty("width",this.width)+(_moz?'margin-top:1px;margin-bottom:1px;padding-left:5px;padding-right:2px;':'')+ (_isQuirksMode ? 'height:20px;' : 'height:16px;') + 'margin-left:'+(this.noMargin?0:10)+'px" onfocus="'+_codeWinName+'.TextFieldWidget_focus(this)" onblur="'+_codeWinName+'.TextFieldWidget_blur(this)" onchange="'+_codeWinName+'.TextFieldWidget_changeCB(event,this)" onkeydown=" return '+_codeWinName+'.TextFieldWidget_keyDownCB(event,this);" onkeyup=" return '+_codeWinName+'.TextFieldWidget_keyUpCB(event,this);" onkeypress=" return '+_codeWinName+'.TextFieldWidget_keyPressCB(event,this);" type="text" '+attr('maxLength',this.maxChar)+' ondragstart="event.cancelBubble=true;return true" onselectstart="event.cancelBubble=true;return true" class="combo" id="'+this.id+'" name="'+this.id+'"'+attr('title',this.tooltip)+' value="">'
}

// ================================================================================

function ComboTextFieldWidget_setDisabled(d)
{
	var o=this
	
	o.oldSetDisabled(d)	
	//o.layer.className=d?'comboDisabled':'combo'
}

// ================================================================================
// ================================================================================
//
// OBJECT newTextComboWidget (Constructor)
//
// This object is a Textfield followed by an IconMenuWidget
//
// ================================================================================
// ================================================================================

function newTextComboWidget(id,maxChar,tooltip,w,changeCB,checkCB,beforeShowCB,formName)
// CONSTRUCTOR
// id			: [String]              the icon id for DHTML processing
// maxChar		: [int]                 text field maximum length
// tooltip		: [String]              arrow icon tooltip
// w			: [int]                 TextComboWidget width
// changeCB		: [Function - optional] callback called when selecting or typing a value
// checkCB		: [Function - optional] callback called when typing a value to check its validity
// beforeShowCB : [Function - optional] callback called before showing the menu
// formName     : [String - optional]   specify a specific ID/Name for using the text fielad inside a form

{
	var o=newWidget(id)

	// basic widgets
	o.text=newComboTextFieldWidget((formName?formName:"text_"+id),TextComboWidget_checkCB,maxChar,null/*keyUpCB*/,TextComboWidget_enterCB,true,tooltip,w-13)
	var tooltipText = _openMenu.replace("{0}", (tooltip?tooltip:""));
	o.arrow=newIconWidget("arrow_"+id,_skin+"menus.gif",TextComboWidget_arrowClickCB,null,tooltipText,7,16,0,81,0,97,true,true)
	o.menu=newMenuWidget("menu_"+id,TextComboWidget_hideCB,beforeShowCB)	
	
	o.arrow.setClasses("iconnocheck", "combobtnhover", "combobtnhover", "combobtnhover")
	
	// set the parent
	o.text.par=o
	o.arrow.par=o
	o.menu.parIcon=o

	// properties
	o.arrow.margin=0
	o.arrow.overCB="IconWidget_none"
	o.arrow.outCB="IconWidget_none"

	o.margin=0
	o.spc=0
	
	o.counter=0

	// to align the text field and arrow icon
	o.arrow.h=12
	o.arrow.dy+=2
	o.arrow.disDy+=2
	
	o.index=_allBOIconsMenus.length++
	_allBOIconsMenus[o.index]=o

	// public methods		
	o.menIcnOldInit=o.init
	o.init=TextComboWidget_init
	o.imwpResize=o.resize
	o.resize=TextComboWidget_resize
	o.getHTML=TextComboWidget_getHTML
	o.setDisabled=TextComboWidget_setDisabled
	o.isDisabled=TextComboWidget_isDisabled
	
	o.add=TextComboWidget_add
	o.addSeparator=TextComboWidget_addSeparator
	o.addMenuItem=TextComboWidget_addMenuItem
	o.select=TextComboWidget_select
	o.getSelection=TextComboWidget_getSelection
	o.valueShow=TextComboWidget_valueShow	
	o.valueSelect=TextComboWidget_valueSelect
	o.setUndefined=TextComboWidget_setUndefined
	o.setContentEditable=TextComboWidget_setContentEditable
	o.isContentEditable=TextComboWidget_isContentEditable
	
	// private methods
	o.changeCB=changeCB
	o.checkCB=checkCB
	o.clickCB=TextComboWidget_clickCB
	o.selectItem=TextComboWidget_selectItem
	o.getItemByIndex=TextComboWidget_getItemByIndex
	o.getItemIndex=TextComboWidget_getItemIndex
	o.setItemDisabled=TextComboWidget_setItemDisabled
	
	o.text.enterCancelBubble=false

	return o
}

// ================================================================================
function TextComboWidget_init()
// Initialize the TextIconMenuWidget
{
	var o=this
	o.menIcnOldInit()
	o.text.init()
	o.arrow.init()
	o.menu.init()
	
	var l=o.layer
	l.onmouseover=TextCombo_OverCB
	l.onmouseout=TextCombo_OutCB
}

// ================================================================================
function TextComboWidget_getHTML()
// Return the HTML of the TextIconMenuWidget
{	
	var o=this, s=''
	
	s+='<table id="'+o.id+'" cellspacing="0" cellpadding="0" border="0" style="cursor:default;margin:'+o.margin+'px"><tbody><tr>'
	s+='<td>'+o.text.getHTML()+'</td>'
	s+='<td style="padding-left:'+o.spc+'px" width="'+(13+o.spc)+'">'+o.arrow.getHTML()+'</td>'
	s+='</tr></tbody></table>'
	
	return s
}

// ================================================================================
function TextComboWidget_resize(w,h)
// Resize the TextIconMenuWidget
{
	var o=this
	if (w!=null)
		w=Math.max(0,w-2*o.margin)

	var d=o.layer.display!="none"

	if (d&_moz&&!_saf)
		o.setDisplay(false)

	o.imwpResize(w,h)

	if (d&_moz&&!_saf)
		o.setDisplay(true)
}

// ================================================================================
function TextComboWidget_add(s,val,selected)
// Add a new item in the menu
// s		[string]	displayed value
// val		[string]	internal value
// selected	[boolean]	item selected or not
{
	var o=this
	var item=o.menu.addCheck(o.id+"_it_"+(o.counter++),s,TextComboWidget_internalCB)
	
	item.val=""+val
	item.parCombo=o
	item.isComboVal=true
	
	if ((o.selectedItem==null)||selected)
		o.selectItem(item)
}

// ================================================================================
function TextComboWidget_addSeparator()
// Add a separator to the menu
{
	this.menu.addSeparator()
}

// ================================================================================
function TextComboWidget_addMenuItem(id,text,cb,icon,dx,dy,disabled,disDx,disDy)
// Add a new item to the menu
{
	this.menu.add(id,text,cb,icon,dx,dy,disabled,disDx,disDy)
}

// ================================================================================
function TextComboWidget_setDisabled(d)
// Enable/Disable the TextIconMenuWidget
{
	var o=this
	
/*  if (o.selectedItem)
		o.text.setValue(d?"":o.selectedItem.text)*/
	o.text.setDisabled(d)
	o.arrow.setDisabled(d)
	o.menu.setDisabled(d)

	o.disabled=d

/*	if(d)
		o.icon.changeTooltip(o.icon.alt?o.icon.alt:"",true)*/
}

// ================================================================================
function TextComboWidget_isDisabled()
{
	var o=this
	
	return o.disabled
}

// ================================================================================
function TextComboWidget_select(idx)
// Select an item in the TextIconMenuWidget
// idx : item index (only items added by "add" function are taken in account)
//       to allow menus combining combo items and menu action items
{
	var o=this,item=o.getItemByIndex(idx)
	if (item)
		o.selectItem(item)
}

// ================================================================================
function TextComboWidget_setItemDisabled(idx,disabled)
// Disable a specific item on the TextIconMenuWidget
// idx [int] item index (only items added by "add" function are taken in account)
// disabled [boolean] disable if true
{
	var o=this,item=o.getItemByIndex(idx)
	if (item)
		item.setDisabled(disabled)
}

// ================================================================================
function TextComboWidget_getSelection()
// Return the value selected in the TextIconMenuWidget
// It is either a value selected from the menu or the 
// value type in the textfield
{
	var o=this,it=o.selectedItem
	var txt=o.text.getValue()
	if (it)
		return {index:o.getItemIndex(it),value:it.val}
	else
		return {index:-1,value:txt}
}

// ================================================================================
function TextComboWidget_valueSelect(v)
{
	v=""+v
	var o=this,items=o.menu.items,len=items.length
	for (var i=0;i<len;i++)
	{
		var it=items[i]
		if ((it.isComboVal)&&(it.val==v))
		{
			o.selectItem(it)
			return
		}
	}
	o.text.setValue(v)
}

// ================================================================================
function TextComboWidget_valueShow(v,show)
{
	v=""+v
	var o=this,items=o.menu.items,len=items.length
	for (var i=0;i<len;i++)
	{
		var it=items[i]
		if ((it.isComboVal)&&(it.val==v))
		{
			it.show(show)
			return
		}
	}
	o.text.setValue(v)
	o.text.show(show)
}

// ================================================================================
function TextComboWidget_setUndefined(u)
{
	var o=this
	if (u)
		o.selectItem(null)
}

// ================================================================================
function TextComboWidget_setContentEditable(d)
{
	var o=this
	
	o.text.setContentEditable(d)
}

// ================================================================================
function TextComboWidget_isContentEditable()
{
	var o=this
	
	return o.text.isContentEditable()
}

// ================================================================================
function TextComboWidget_selectItem(item)
// PRIVATE
{
	var o=this
	
	if (o.selectedItem)
		o.selectedItem.check(false)
	
	if (item)
	{
		o.val=item.val
		o.text.setValue(/*o.disabled?"":*/item.text)		
		o.selectedItem=item
		item.check(true)
/*		if(o.disabled)
			o.text.changeTooltip(o.icon.alt?o.icon.alt:"",true)		
		else
			o.text.changeTooltip(o.icon.alt?(o.icon.alt+' ('+item.text)+')':(item.text),true)	*/	
	}
	else
	{
		o.val=null
		o.text.setValue("")
//		o.text.changeTooltip(o.icon.alt?o.icon.alt:"",true)		
		o.selectedItem=null
	}
}

// ================================================================================
function TextComboWidget_getItemByIndex(idx)
// PRIVATE
{
	var items=this.menu.items
	return ((idx>=0)&&(idx<items.length))?items[idx]:null
}

// ================================================================================
function TextComboWidget_getItemIndex(item)
// PRIVATE
{
	var items=this.menu.items,len=items.length,j=0
	for (var i=0;i<len;i++)
	{
		var it=items[i]
		if (it.isComboVal)
		{
			if (it.id==item.id)
				return j
		
			j++
		}
	}
	return -1
}

// ================================================================================
function TextComboWidget_changeCB()
// PRIVATE
{
	var p=this.par
	
	var b=true
	if (p.checkCB)
		b=p.checkCB()
		
	if (!b)
		return
		
	if (p.changeCB)
		p.changeCB()

}

// ================================================================================
function TextComboWidget_enterCB()
// PRIVATE
{
	var p=this.par
	
	// 1st : Unselect any selected element from the menu
	if (p.selectedItem)
	{
		p.selectedItem.check(false)
		p.selectedItem=null
	}
	
	//2nd : Call the callBack function
	var b=true
	if (p.checkCB)
		b=p.checkCB()
		
	if (!b)
		return
		
	if (p.changeCB) 
		p.changeCB()
}

// ================================================================================
function TextComboWidget_checkCB()
// PRIVATE
{
	var p=this.par
	
	if (p.checkCB) 
		p.checkCB()
}

// ================================================================================
function TextComboWidget_hideCB()
// PRIVATE
{
	var o=this.parIcon;
	if(o.arrow)
		o.arrow.focus();
	TextComboOutCB(o.index)
}

// ================================================================================
function TextComboWidget_arrowClickCB()
// PRIVATE
{
	this.par.clickCB()
}

// ================================================================================
function TextComboWidget_clickCB()
// PRIVATE
{
	var o=this,l=o.layer
	
	o.menu.show(!o.menu.isShown(),getPosScrolled(l).x,getPosScrolled(l).y+o.getHeight()+1,null,null,o)
	TextComboOverCB(o.index)
}

// ================================================================================

function TextCombo_OverCB()
// PRIVATE
{
	TextComboOverCB(getWidget(this).index)
	return true
}


// ================================================================================

function TextComboOverCB(i)
// PRIVATE
{
	var o=_allBOIconsMenus[i]
	IconWidget_overCB(o.arrow.index)	// Re-use method from IconWidget class
}

// ================================================================================

function TextCombo_OutCB(i)
// PRIVATE
{
	TextComboOutCB(getWidget(this).index)
}

// ================================================================================

function TextComboOutCB(i)
// PRIVATE
{
	var o=_allBOIconsMenus[i]
	if (!o.menu.isShown())
		IconWidget_outCB(o.arrow.index)	// Re-use method from IconWidget class
	else
		IconWidget_overCB(o.arrow.index)
}

// ================================================================================
function TextComboWidget_internalCB()
// PRIVATE
{
	var o=this,c=o.parCombo
	c.selectItem(o)

	if (c.changeCB)
		c.changeCB()
}

// ================================================================================
function TextComboWidget_keyUpCB()
// PRIVATE

{
}

// ================================================================================
// ================================================================================
//
// OBJECT newIconScrollMenuWidget (Constructor)
//
// An icon widget that add an arrow part at the right. when clicking on the
// right part, a menu pops up
// If no callback is attached, the also pops up when clicking on the left part
// menu.js must be included to use this component
//
// ================================================================================
// ================================================================================
/*
function newIconScrollMenuWidget(id,src,clickCB,text,alt,w,h,dx,dy,disDx,disDy,
	changeCB,multi,width,lines,tooltip,dblClickCB,keyUpCB,showLabel,label,convBlanks,beforeShowCB)
// CONSTRUCTOR
// id			[String]				the icon id for DHTML processing
// src			[String]				the icon image url
// clickCB		[Function - optional]	callback called when clicking
// text			[String - optional]		icon  text
// alt			[String]				icon tooltip
// w			[int]					visible image width
// h			[int]					visible image height
// dx			[int - optional]		horizontal offset in the image
// dy			[int - optional]		vertical offset in the image
// disDx		[int - optional]		horizontal offset for disabled state
// disDy		[int - optional]		vertical offset for disabled state
// changeCB		[Function - optional]	calback when selection is changed by the user
// multi		[boolean - optional]	if true, multiselection is enabled
// lines		[int]					number of visible lines
// tooltip		[String - optional]		tooltip for 508
// dblClickCB	[Function - optional]	calback when an item is double-clicked
// keyUpCB		[Function - optional]	calback when a key
// showLabel	[boolean - optional]	if true a label is displayed
// label		[boolean - optional]	text of the label
// convBlanks	[int - optional]	
// beforeShowCB [Function - optional]	callback called before showing the menu
{
	var o=newIconMenuWidget(id,src,clickCB,text,alt,w,h,dx,dy,disDx,disDy)

	o.menu=newScrollMenuWidget("iconMenu_menu_"+id,changeCB,multi,width,lines,tooltip,dblClickCB,keyUpCB,showLabel,label,convBlanks,beforeShowCB)
	o.add=IconScrollMenu_add
	
	return o
}

// ================================================================================

function IconScrollMenu_add(s,val,sel,id)
{
	this.menu.add(s,val,sel,id)
}

// ================================================================================
// ================================================================================
//
// OBJECT newPaneWidget (Constructor)
//
// creates a pane with multiple switchable zones
// 
//
// ================================================================================
// ================================================================================

function newPaneWidget(id,tooltip,w,h,x,y,changeCB)
// id      [String] the pane id for DHTML processing
// w       [Int - Optional] the pane width (100 px if ommited or null)
// h       [Int - Optional] the pane height (400 px if ommited or null)

// Returns [PaneWidget] the new instance
{
	var o=newWidget(id)

	// Private Fields
	o.items=new Array
	o.w=(w==null?100:w)
	o.h=(w==null?400:h)
	o.x=x
	o.y=y
	o.changeCB=changeCB
	o.pal = newPaletteWidget("palette_"+id);
	o.pal.vertPadding=0
	o.panes = new Array;
	o.panemenu = null
	o.selection=-1
	o.topH=_ie?18:17
	o.bottomH=24+(_ie?0:2)
	o.tooltip=tooltip
	o.iframe=newWidget("_iframe"+o.id)
	o.title=newWidget("paneTitleB_"+o.id)
	
	// Methods
	o.add = PaneWidget_add
	o.valueSelect = PaneWidget_valueSelect
	o.valueShow = PaneWidget_valueShow
	o.getSelection = PaneWidget_getSelection
			
	// Standard widget methods
	o.getHTML=PaneWidget_getHTML
	o.oldInit=o.init
	o.init=PaneWidget_init
	o.oldResize=o.resize
	o.resize=PaneWidget_resize
	o.getFrame=PaneWidget_getFrame
	o.resizePalette=PaneWidget_resizePalette

	return o
}

// ================================================================================

function PaneWidget_getSelection()
{
	var o=this,i=o.selection
	if (i==-1)
		return null
	else
		return {index:i,value:o.panes[i].value}
}

// ================================================================================

function PaneWidget_resize(w,h)
{
	var o=this
	o.w=w
	o.h=h
	o.oldResize(Math.max(0,w+(_ie?0:-2)),Math.max(0,h+(_ie?0:-2)))
	o.pal.resize(Math.max(0,w-2))
	o.title.resize(Math.max(0,w-2))
	o.iframe.resize(Math.max(0,o.w-2),Math.max(0,o.h-(o.topH+o.bottomH)-3))
	o.resizePalette()
}

// ================================================================================

function PaneWidget_resizePalette()
// PRIVATE
{
	var o=this,nbButton=0,panes=o.panes,len=panes.length;maxVisible=Math.floor((o.w-22)/24),j=0,hasMenu=false
	
	for (var i=0;i<len;i++)
	{
		var pane=panes[i],isVis=j<maxVisible,menu=o.menu
		pane.button.setDisplay(pane.shown&&(isVis))
		
		if (menu)
		{
			var menuItem=menu.getItem(i),show=pane.shown&&(!isVis)
			menuItem.show(show)
			if (show)
				hasMenu=true
		}
		
		if (pane.shown) j++
	}
	o.panemenu.setDisplay(hasMenu)
}

// ================================================================================

function PaneWidget_init()
{
	var o=this
	
	o.oldInit()
	o.pal.init()
	o.panemenu.init()
	o.iframe.init()
	o.title.init()
}

// ================================================================================

function PaneWidget_getFrame()
{
	return eval("_curWin.frames._iframe"+this.id)
}

// ================================================================================

function PaneWidget_valueShow(value,show)
{
	var o=this,panes=o.panes,index=-1,pane=null

	for (i=0;i<panes.length;i++)
	{
		pane=panes[i]
		if (pane.value==value)
		{
			index=i
			break
		}
	}
	
	if (pane)
	{
		//pane.button.setDisplay(show)
		o.resizePalette()
		pane.menuItem.show(show)
		pane.shown=show
	}
}

// ================================================================================

function PaneWidget_valueSelect(value)
{
	var o=this,panes=o.panes,index=-1,pane=null,oldSel=o.selection

	for (i=0;i<panes.length;i++)
	{
		pane=panes[i]
		if (pane.value==value)
		{
			index=i
			break
		}
	}
	
	// Deselect prev
	if (o.selection>=0)
	{
		var oldPane=panes[o.selection]
		if (oldPane)
		{
			oldPane.menuItem.check(false)
			oldPane.button.check(false)
		}
	}
	
	if (index>=0)
	{
		pane.menuItem.check(true)
		pane.button.check(true)
		o.title.setHTML(convStr(pane.name))

		var hasChanged=(o.selection!=index)
		o.selection=index
		
		if (hasChanged)
		{
			if ((oldSel!=-1)&&(o.changeCB))
				o.changeCB()
			frameNav("_iframe"+o.id,pane.url,false,_curWin)
		}
	}
	else
	{
		if (o.selection!=-1)
		{
			o.selection=-1
			frameNav("_iframe"+o.id,_skin+"../../empty.html",false,_curWin)
		}
	}
}

// ================================================================================

function PaneWidget_getHTML()
{
	var o=this,panes=o.panes,pal=o.pal
	
	if (o.panemenu == null)
	{
		pal.beginRightZone()
		var pm=o.panemenu = newIconWidget("__panemenu_"+o.id,_skin+'panemenu.gif',PaneWidget_buttonCB,null,null,9,16);
		pm.isPaneMenu=true
		pm.parPane=o
		pal.add(pm)
		
		var menu=o.menu = newMenuWidget("panemenu_"+o.id);
		for (var i=0; i<panes.length; i++)
		{
			var pane=panes[i]
			var m=menu.addCheck("m__"+pane.button.id,pane.name,PaneWidget_buttonCB,pane.icon,pane.button.dx,pane.button.dy)
			m.paneElement=pane
			pane.menuItem=m
		}
	}
	o.resizePalette()
	
	var absSty = ((o.x!=null)&&(o.y!=null)) ? "position:absolute;left:"+o.x+"px;top:"+o.y+"px;" : ""

	return '<div class="treeZone" id="'+o.id+'" style="'+absSty+'overflow:hidden;width:'+(o.w+(_ie?0:-2))+'px;height:'+(o.h+(_ie?0:-2))+'px">' +
		'<div class="titlepane" id="paneTitleB_'+o.id+'" style="overflow:hidden;background-image:url('+_skin+'panetitle.gif);height:'+(o.topH)+'px"></div>'+
		'<iframe class="treeZone" style="border-width:0px;border-bottom-width:1px;width:'+Math.max(0,o.w-2)+'px;height:'+(Math.max(0,o.h-(o.topH+o.bottomH)-3))+'px" id="_iframe'+o.id+'" name="iframe_'+o.id+'" title="'+convStr(o.tooltip)+'" frameborder="0" src="'+_skin+'../../empty.html"></iframe>'+
		'<div class="panelzone" id="paneTools_'+o.id+'" style="height:'+o.bottomH+'px">'+pal.getHTML()+'</div>' +
		'</div>'
}

// ================================================================================

function PaneWidget_add(value,name,icon,dx,dy,url)
{
	newPaneElement(this,this.id+"_item_"+value,value,name,icon,dx,dy,url)
}

// ================================================================================

function PaneWidget_buttonCB()
{
	var o=this
	
	if (o.isPaneMenu)
	{
		var m = o.parPane.panemenu,l = m.layer
		o.parPane.menu.show(true,getPosScrolled(l).x+m.getWidth()+1,getPosScrolled(l).y+16)
	}
	else
	{
		var paneE = o.paneElement
		paneE.par.valueSelect(paneE.value)
	}
}

// ================================================================================
// ================================================================================
//
// OBJECT newPaneElementWidget (Constructor)
//
// creates a pane with multiple switchable zones
// 
//
// ================================================================================
// ================================================================================

function newPaneElement(par,id,value,name,icon,dx,dy,url)
{
	var o=new Object
	
	o.par=par
	par.panes[par.panes.length]=o
	o.value=value
	o.name=name
	o.icon=icon
	o.button=newIconCheckWidget(id,icon,PaneWidget_buttonCB,null,name,16,16,dx,dy);
	o.button.paneElement=o
	o.url=url
	par.pal.add(o.button)
	o.shown=true
}

// ================================================================================
// ================================================================================
//
// OBJECT newButtonMenuWidget (Constructor)
//
//
// ================================================================================
// ================================================================================
 
function newButtonMenuWidget(id,label,width,tooltip,beforeShowCB,tabIndex)
{
    var o=newButtonWidget(id,label,ButtonMenuWidget_clickCB,width,null,tooltip,tabIndex,0,_skin+"menus.gif",7,16,0,81,true,0,97)
 
    o.menu = newMenuWidget("buttonMenu_menu_"+id,null,beforeShowCB);
    o.getMenu=IconMenuWidget_getMenu
 
    return o;
}
 
// ================================================================================

function ButtonMenuWidget_clickCB()
// PRIVATE
{
      var o=this,l=o.layer;
      o.menu.show(!o.menu.isShown(),getPosScrolled(l).x,getPosScrolled(l).y+o.getHeight(),null,null,o)
}

_scrollW=20
_scrollH=20
_margin=2
	
// ================================================================================
// ================================================================================
//
// OBJECT newIconTableWidgetElem (Constructor)
//
// Display a clickable icon with optional text. It can be used in a palette fo
// instance. The image can be a sub-part of a bigger image 
// (use dx and dy to define offset)
//
// ================================================================================
// ================================================================================

function newIconTableWidgetElem(parid,id,src,clickCB,text,alt,w,h,dx,dy,disDx,disDy,maxW,maxH)
{
	var o=newIconRadioWidget(id,src,clickCB,text,alt,"grp_"+parid,w,h,dx,dy,disDx,disDy)
	
	// Properties
	o.txtNoPadding=true
	o.txtAlign='center'
	o.margin=0
	
	o.maxW=maxW
	o.maxH=maxH
	
	o.imgW=w+o.border
	o.imgH=h+o.border
	
	o.txtW=maxW-2*_margin

	// Methods
	o.oldGetHTML=o.getHTML
	o.getHTML=IconTableWidgetElem_getHTML
	o.oldResize=o.resize
	o.resize=IconTableWidgetElem_resize
	
	return o
}

// ================================================================================

function IconTableWidgetElem_getHTML()
// returns [String] the HTML of an icon widget
{
	var o=this,imgCode=o.src?'<div style="overflow:hidden;width:'+o.imgW+'px;height:'+o.imgH+'">'+simpleImgOffset(o.src,o.w,o.h,o.dis?o.disDx:o.dx,o.dis?o.disDy:o.dy,'IconImg_'+o.id,null,o.alt,'margin:2px;cursor:'+(o.clickCB ? (!o.acceptClick() ? 'default' : _hand):'default'))+o.extraHTML+'</div>':'<div class="iconText" style="width:1px;"></div>'

	var txtAtt='style="white-space:normal;'
	txtAtt+='text-overflow:ellipsis;overflow:hidden;width:'+o.txtW+'px;'
	txtAtt+='"'
	
	var d=o.disp?"":"display:none;"
	
	return '<table style="'+d+'width:'+o.maxW+'px;height:'+o.maxH+'margin:0px" id="'+o.id+'" class="' + o.nocheckClass + '" cellspacing="0" cellpadding="0" border="0"><tr valign="middle">'+
			'<td align="center">'+ ((o.clickCB&&_ie)?lnk(imgCode,null,null,null, ' tabIndex="-1"' ):imgCode)+'</td></tr><tr>'+
			(o.text?'<td align="center"><div id="IconImg_Txt_'+o.id+'" class="iconTableText'+(o.dis?"Dis":"")+'" '+txtAtt+'>'+convStr(o.text)+'</div></td>':'')+
			'</tr></table>'
}

// ================================================================================

function IconTableWidgetElem_resize(w,h)
{
	var o=this
	
	if (w!=null)
		o.oldResize(w-2*_margin,null)
	if (h!=null)
		o.oldResize(null,h-2*_margin)
}

// ================================================================================
// ================================================================================
//
// OBJECT newIconTableWidget (Constructor)
//
// Display a table of clickable icons.
//
// ================================================================================
// ================================================================================

function newIconTableWidget(id,horiz,w,h)
// CONTRUCTOR
// id       : [String]              the icon id for DHTML processing
// horiz    : [boolean]             if true, table is horizontal (1 row and n columns)
//									if false, table is vertical  (n rows and 1 column)
// w		: [int]					width
// h		: [int]					height
{
	var o=newScrolledZoneWidget(id,0,0,w,h)
	
	// Properties
	o.horiz=horiz
	o.padding=2
	o.icnTblLayer=null
	o.contCSS=null
	
	o.elems=new Array
	
	// Methods
	o.oldInit=o.init
	o.init=IconTableWidget_init
	o.oldResize2=o.resize
	o.resize=IconTableWidget_resize
	o.getHTML=IconTableWidget_getHTML	
	o.add=IconTableWidget_add
	o.select=IconTableWidget_select
	
	return o
}

// ================================================================================

function IconTableWidget_init()
{
	var o=this, len=o.elems.length
	
	//o.oldInit()
	
	for (var i=0;i<len;i++)
		o.elems[i].init()
	
	o.icnTblLayer=getLayer(_codeWinName+'icnTbl'+o.id)
	o.contCSS=o.icnTblLayer.style
}

// ================================================================================

function IconTableWidget_resize(w,h)
{
	var o=this
	
	//o.oldResize2(w,h)
	if (w!=null)
	{
		if (o.contCSS!=null)
			o.contCSS.width=''+w+'px'
	}
	
	if (h!=null)
	{
		if (o.contCSS!=null)
			o.contCSS.height=''+h+'px'
	}
}

// ================================================================================

function IconTableWidget_getHTML()
{
	var o=this, s=''
	
//	s+= o.beginHTML()
	s+=	'<table id="'+_codeWinName+'icnTbl'+o.id+'" width="'+o.w+'" height="'+o.h+'px" class="iconTableZone" cellspacing="0" cellpadding="0"><tbody>'	
	s+= '<tr valign="top"><td>'
	
	s+=	'<table cellspacing="0" cellpadding="'+o.padding+'px"><tbody>'	
	if (o.horiz)
	{
		s+= '<tr>'
		
		for (var i=0;i<o.elems.length;i++)
		{
			s+= '<td align="center">'
			s+= o.elems[i].getHTML()
			s+= '</td>'
		}
				
		s+= '</tr>'
	}
	else
	{		
		for (var i=0;i<o.elems.length;i++)
		{
			s+= '<tr><td align="center">'
			s+= o.elems[i].getHTML()
			s+= '</td></tr>'
		}
	}
	s+=	'</tbody></table>'
	s+= '</td><tr>'
		
	s+=	'</tbody></table>'
//	s+= o.endHTML()
	
	return s
}

// ================================================================================

function IconTableWidget_add(src,clickCB,text,alt,w,h,dx,dy,disDx,disDy)
{
	var o=this, len=o.elems.length, icn=null
	
	var maxW=o.horiz?o.h:o.w
	maxW-=2*_margin
	var maxH=maxW
	
	icn=newIconTableWidgetElem(o.id,"iconTable_icon_"+len+o.id,src,clickCB,text,alt,
		w,h,dx,dy,disDx,disDy,maxW,maxH)
	icn.par=this
	
	o.elems[len]=icn
}

// ================================================================================

function IconTableWidget_select(index)
{
	var o=this
	
	var e=o.elems[index]
	if ( e==null )
		return
		
	e.check(true)
}

// ================================================================================
// Icon Border Menu Widget
// ================================================================================

function newIconBordersMenuWidget(id,clickCB,beforeShowCB)
{
	var o=newIconMenuWidget(id,_skin+'../borders.gif',IconBordersMenuWidget_iconInternalClickCB,null,null,16,16,0,0,0,16,false,beforeShowCB,_menuType_border)
		
	// properties
	o.selectedBorder=new Object
	o.selectedBorder.top=0
	o.selectedBorder.bot=0
	o.selectedBorder.left=0
	o.selectedBorder.right=0
	
	o.lastSelectedBorder=new Object
	o.lastSelectedBorder.top=0
	o.lastSelectedBorder.bot=0
	o.lastSelectedBorder.left=0
	o.lastSelectedBorder.right=0
	
	o.bordersClickCB=clickCB
		
	// methods
	o.getSelectedBorder=IconBordersMenuWidget_getSelectedBorder
	o.getLastSelectedBorder=IconBordersMenuWidget_getLastSelectedBorder
	
	return o
}

// ================================================================================

function IconBordersMenuWidget_getSelectedBorder()
{
	var o=this
	
	return o.selectedBorder
}

// ================================================================================

function IconBordersMenuWidget_getLastSelectedBorder()
{
	var o=this
	
	return o.lastSelectedBorder
}

// ================================================================================

function IconBordersMenuWidget_iconInternalClickCB()
{
	var o=this
	
	o.selectedBorder.top=o.lastSelectedBorder.top
	o.selectedBorder.bot=o.lastSelectedBorder.bot
	o.selectedBorder.left=o.lastSelectedBorder.left
	o.selectedBorder.right=o.lastSelectedBorder.right
	
	if (o.bordersClickCB)
		o.bordersClickCB()
}

// ================================================================================

function IconBordersMenuWidget_internalClickCB()
{
	var o=this
	
	var top=bot=left=right=-1
	
	switch (o.id)
	{
		case "border_0":
			top=bot=left=right=0
			break;
		case "border_1":
			bot=1
			break;
		case "border_2":
			left=1
			break;
		case "border_3":
			right=1
			break;		
		case "border_4":
			bot=2
			break;
		case "border_5":
			bot=3
			break;
		case "border_6":
			top=bot=1
			break;
		case "border_7":
			top=1;bot=2
			break;
		case "border_8":
			top=1;bot=3
			break;
		case "border_9":
			top=bot=left=right=1
			break;
		case "border_10":
			top=bot=left=right=2
			break;
		case "border_11":
			top=bot=left=right=3
			break;			
		case "border_12":
			top=bot=left=right=-2
			break;
	}

	// Set the selection
	var pp=o.par.parIcon
	
	pp.selectedBorder.top=top
	pp.selectedBorder.bot=bot
	pp.selectedBorder.left=left
	pp.selectedBorder.right=right
			
	if (o.id!="border_12")
	{
		// Update the menu icon
		pp.icon.changeImg(16*o.idx,0)
	
		// Save the previous border
		pp.lastSelectedBorder.top=top
		pp.lastSelectedBorder.bot=bot
		pp.lastSelectedBorder.left=left
		pp.lastSelectedBorder.right=right
	}

	// Call the menu callback
	if (pp.bordersClickCB)
		pp.bordersClickCB()
}

*/
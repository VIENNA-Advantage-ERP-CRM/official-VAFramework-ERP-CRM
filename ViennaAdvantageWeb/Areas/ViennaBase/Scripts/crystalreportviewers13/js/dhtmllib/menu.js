// <script>


_menusZIndex=2000
_menusItems=new Array
_globMenuCaptured=null
_isColor=0
_isLastUsedColor=1
_isNotColor=2

_currentFocus=null

_mitemH=22

// ================================================================================
// ================================================================================
//
// OBJECT newMenuWidget (Constructor)
//
// creates a simple menu widget
// This widget does not have the standard behaviour. Using
// init() and getHTML() and write() is not allowed
//
// ================================================================================
// ================================================================================

function newMenuWidget(id,hideCB,beforeShowCB)
// id            [String] the menu id for DHTML processing
// hideCB        [Function - Optional] callback called when the menu is closed
// beforeShowCB  [Function - Optional] callback called before menu is shown
// Returns       [MenuWidget] the new instance
{
	var o=newWidget(id)

	// Private Fields
	o.items=new Array
	o.par=null
	o.container=null
	o.currentSub=-1
	o.nextSub=-1
	o.zIndex=_menusZIndex
	o.hideCB=hideCB
	o.beforeShowCB=beforeShowCB
	o.accelEnabled=true

	// Public Methods
	o.init=MenuWidget_init
	o.justInTimeInit=MenuWidget_justInTimeInit
	o.getHTML=MenuWidget_getHTML
	//o.getShadowHTML=MenuWidget_getShadowHTML
	o.show=MenuWidget_show
	
	o.setAccelEnabled=MenuWidget_setAccelEnabled
	o.isAccelEnabled=MenuWidget_isAccelEnabled

	o.internalAdd=o.add=MenuWidget_add
	o.addCheck=MenuWidget_addCheck
	o.addSeparator=MenuWidget_addSeparator	
	
	o.insert=MenuWidget_insert
	o.insertCheck=MenuWidget_insertCheck
	o.insertSeparator=MenuWidget_insertSeparator

	o.getItem=MenuWidget_getItem
	o.getItemByID=MenuWidget_getItemByID
	o.isShown=MenuWidget_isShown

	o.remove=MenuWidget_remove
	o.removeAll=MenuWidget_removeAll;
	o.removeByID=MenuWidget_removeByID	
	
	o.resetItemCount=MenuWidget_resetItemCount
	o.resetTooltips=MenuWidget_resetTooltips
	
	// Private Methods
	o.showSub=MenuWidget_showSub
	o.captureClicks=MenuWidget_captureClicks
	o.releaseClicks=MenuWidget_releaseClicks
	o.focus=MenuWidget_focus
	o.restoreFocus=MenuWidget_restoreFocus
	o.hasVisibleItem=MenuWidget_hasVisibleItem	
	o.updateIndex=MenuWidget_updateIndex
	o.getTotalNumItems=MenuWidget_getTotalNumItems
	
	// Click capture
	o.clickCB=new Array
	o.clickCBDocs=new Array
	
	// Disable direct write
	o.write=MenuWidget_write
	
	o.alignLeft=false
	o.sepCount=0
	o.itemCount=0
	
	return o
}

// ================================================================================

function MenuWidget_captureClicks(w)
// capture click in current frame and sub-frame, so when the users clicks outside
// the menu, the menu is closed. Eventual click handlers are stored.
// Use releaseClicks() to restore previous click handlers
// Returns [void]
{
	var o=this
	if (o.par==null)
	{
		if (w==null)
		{
			_globMenuCaptured=o
			o.clickCB.length=0
			o.clickCBDocs.length=0
			w=_curWin
		}

		//_excludeFromFrameScan variable is set by a frame that does not want to
		// be scaned, when the frame is used to download document for instance.

		if (canScanFrames(w))
		{
			if (_moz)
			{
				_oldErrHandler=window.onerror
				window.onerror=localErrHandler
			}
	
			try
			{
				d=w.document
				o.clickCB[o.clickCB.length]=d.onmousedown
				o.clickCBDocs[o.clickCBDocs.length]=d
				d.onmousedown=MenuWidget_globalClick
			
				var fr=w.frames,len=fr.length
				for (var i=0;i<len;i++)
					o.captureClicks(fr[i])
			}
			catch(expt)
			{
			}

			if (_moz)
				window.onerror=_oldErrHandler
		}
	}
}

// ================================================================================

function MenuWidget_releaseClicks()
// Restore click handlers overrided by captureClicks()
// Returns [void]
{
	var o=this
	if (o.par==null)
	{
		var len=o.clickCB.length
		for (var i=0;i<len;i++)
		{
			try
			{
				o.clickCBDocs[i].onmousedown=o.clickCB[i]
			}
			catch(expt)
			{
			}
			o.clickCB[i]=null
			o.clickCBDocs[i]=null
		}
		o.clickCB.length=0
		o.clickCBDocs.length=0
	}
}

// ================================================================================

_menuItem=null;
function MenuWidget_focus()
//give focus to the first visible menuItem
{
	var o=this, items=o.items, len=items.length	
	for(var i=0; i<len;i++)
	{
		if(items[i].isShown && !items[i].isSeparator)
		{	
			_menuItem=items[i];		
			setTimeout("_menuItem.focus()",1);
			if(o.endLink) o.endLink.show(true)
			if(o.startLink) o.startLink.show(true)
			
			break;
		}
	}		
}

// ================================================================================

function MenuWidget_keepFocus(id)
{		
	var o=getWidget(getLayer(id))		
	if (o) o.focus();
}

// ================================================================================

function MenuWidget_restoreFocus()
{
	var o=this
	
	if(o.endLink) o.endLink.show(false)
	if(o.startLink) o.startLink.show(false)
	
	if(o.parIcon) o.parIcon.focus()
	else 
		if (o.par)	o.par.focus()
		else if(o.parCalendar) o.parCalendar.focus()
}

// ================================================================================

function MenuWidget_keyDown(id,e)
{	
	var o=getWidget(getLayer(id))
	var key=eventGetKey(e)	
	if(key==27 && o)//escape
	{
		o.restoreFocus()		
		o.show(false)	
		if (o.par && o.par.par)//case submenu
		{
			o.par.par.currentSub=-1	
		}
		o.currentSub=-1	
		eventCancelBubble(e);//be careful ! usefull for dialog box close by Escape keypressed
	}
	else if(o && (key==109 || key==37))//collapse (- ou <-)
	{
		if (o.par && o.par.par)  //only for submenu
		{
			o.restoreFocus()		
			o.show(false)			
			o.par.par.currentSub=-1			
			o.currentSub=-1	
		}
	}	
	else if(key==13)//enter
	{
		eventCancelBubble(e);//be careful ! usefull for dialog box close by Enter keypressed
	}
}

// ================================================================================

function MenuWidget_releaseGlobMenuCaptured()
{
	var o=_globMenuCaptured
	if (o!=null)
	{
		o.releaseClicks()
		_globMenuCaptured=null
	}
}

//================================================================================

function MenuWidget_globalClick()
// PRIVATE
// Click handler that close the menu if the user clicks outside the menu
// Also call releaseClicks() to restore previous click handlers
{
	var o=_globMenuCaptured
	if (o!=null)
	{
		MenuWidget_releaseGlobMenuCaptured()
		o.show(false)
	}
}

// ================================================================================

function MenuWidget_add(id,text,cb,icon,dx,dy,disabled,disDx,disDy,alt)
// id       [String] the item id
// text     [String] the item text
// icon     [String - optional]  the item icon url (16x16 pixels)
// dx       [int - optional] the item icon horizontal offset
// dy       [int - optional] the item icon vertical offset
// disabled [boolean - optional] initial disabled state (default is not disabled)
// disDx    [int - optional] the item icon horizontal offset for disabled
// disDy    [int - optional] the item icon vertical offset for disabled
// Return   [MenuItem] the new item
{
	var o=this,i=o.items.length,itemNo=null
	if (id.substr(0,9)!="_menusep_")
	{
		o.itemCount++
		itemNo=o.itemCount
	}
	
	var ret=o.items[i]=newMenuItem(o,id,text,cb,itemNo,icon,dx,dy,disabled,disDx,disDy,false,alt)
	ret.menuIndex=i
	ret.dynHTML()

	return ret
}

// ================================================================================

function MenuWidget_addCheck(id,text,cb,icon,dx,dy,disabled,disDx,disDy,alt)
// id       [String] the item id
// text     [String] the item text
// icon     [String - optional] the item icon url (16x16 pixels)
// dx       [int - optional] the item icon horizontal offset
// dy       [int - optional] the item icon vertical offset
// disabled [boolean - optional] initial disabled state (default is not disabled)
// disDx    [int - optional] the item icon horizontal offset for disabled
// disDy    [int - optional] the item icon vertical offset for disabled
// Return   [MenuItem] the new menu item
{
	var o=this,i=o.items.length,itemNo=null
	if (id.substr(0,9)!="_menusep_")
	{
		o.itemCount++
		itemNo=o.itemCount
	}
	
	var ret=o.items[i]=newMenuItem(o,id,text,cb,itemNo,icon,dx,dy,disabled,disDx,disDy,true,alt)
	ret.menuIndex=i
	ret.dynHTML()

	return ret
}

// ================================================================================

function MenuWidget_addSeparator()
// Return  [MenuItem] the new separator
{
	var s=this.internalAdd("_menusep_"+(this.sepCount++))
	s.isSeparator=true
	
	return s
}

// ================================================================================

function MenuWidget_insert(index,id,text,cb,icon,dx,dy,disabled,disDx,disDy,alt)
// Return  [MenuItem] the new separator
{	
	var o=this,i=o.items.length,itemNo=null
	if (id.substr(0,9)!="_menusep_")
	{
		o.itemCount++
		itemNo=o.itemCount
	}
	
	var item = newMenuItem(o,id,text,cb,itemNo,icon,dx,dy,disabled,disDx,disDy,false,alt);			
	arrayAdd(o,'items',item,index);
	o.updateIndex();
	
	item.dynHTML()
	
	return item
}

// ================================================================================

function MenuWidget_insertCheck(index,id,text,cb,icon,dx,dy,disabled,disDx,disDy,alt)
// Return  [MenuItem] the new separator
{	
	var o=this,i=o.items.length,itemNo=null
	if (id.substr(0,9)!="_menusep_")
	{
		o.itemCount++
		itemNo=o.itemCount
	}
	
	var item = newMenuItem(o,id,text,cb,itemNo,icon,dx,dy,disabled,disDx,disDy,true,alt);
	arrayAdd(o,'items',item,index);
	o.updateIndex();
	
	item.dynHTML()
	
	return item
}


// ================================================================================

function MenuWidget_insertSeparator(index)
// index [int] the item index
// Return  [MenuItem] the new separator
{
	var item = newMenuItem(this,"_menusep_"+(this.sepCount++));
	item.isSeparator=true;
						
	arrayAdd(this,'items',item,index);
	this.updateIndex();	
	
	item.dynHTML()		
	
	return item
}

// ================================================================================

function MenuWidget_init()
// Redefined for disable the default widget init function
// Return [void]
{
}

// ================================================================================

function MenuWidget_getItem(index)
// Get an item in the menu 
// index [int] the item index
// Return [MenuItem] the item (null if invalid index)
{
	var o=this,items=o.items

	if ((index>=0)&&(index<items.length))
		return items[index]

	return null
}

// ================================================================================

function MenuWidget_getItemByID(id)
// Get an item in the menu 
// index [int] the item index
// Return [MenuItem] the item (null if invalid index)
{
	var o=this,items=o.items

	for(var i in items)
	{
		if(items[i].id == id)		
			return items[i];		
	}		

	return null
}

// ================================================================================

function MenuWidget_removeByID(id)
{
	var o=this;
	var item = o.getItemByID(id);
	if(item)
	{	
		//remove in items
		arrayRemove(o,'items',item.menuIndex);	
		o.updateIndex();
		
		//remove in html		
		if (o.layer==null)
			return
				
		var tbody=o.layer.childNodes[0];
		tbody.deleteRow(item.menuIndex);
	}
}

// ================================================================================

function MenuWidget_removeAll()
{
	this.remove();
}

function MenuWidget_remove(index)
{
	var o=this;
	if(index != null)
	{
		arrayRemove(o,'items',index);	
		o.updateIndex();	
	}
	else //null = remove all
	{
		o.items.length = 0;		
	}
	//remove in html		
	if (o.layer==null)
		return
	
	var tbody=o.layer.childNodes[0];
	if (index != null) {		
		tbody.deleteRow(index);
	}
	else {
	    while(tbody.firstChild) {
	        tbody.removeChild(tbody.firstChild);
	    }
	}
}

// ================================================================================

function MenuWidget_updateIndex()
{
	var items = this.items,len = items.length
	for(var i=0; i<len;i++)
	{
		items[i].menuIndex=i;
	}
}

// ================================================================================

function MenuWidget_showSub()
// PRIVATE show the sub menu - for internal purpose
// Return [void]
{
	var o=this

	if (o.nextSub!=-1)
	{
		if (o.nextSub!=o.currentSub)
		{
			// hide current sub-menu
			var currentItem=o.items[o.currentSub]
			if (currentItem&&currentItem.sub)
			{
				currentItem.sub.show(false)
				o.currentSub=-1
			}

			// show sub-menu
			var nextItem=o.items[o.nextSub]
			if (nextItem&&nextItem.sub)
			{
				var lyr=nextItem.layer
				var x=parseInt(o.css.left)
				var y=parseInt(o.css.top)
				
				for (var i=0;i<o.nextSub;i++)
				{
					var item=o.items[i]
					
					if (item.isShown)
					{
						if ((item.icon!=null)||(item.text!=null))
							y+=_mitemH
						else
							y+=3
					}
				}
				
				var w=o.getWidth()
				
				x=x+w-4

				nextItem.attachSubMenu(nextItem.sub)
				nextItem.sub.show(true,x,y,false,w)
				o.currentSub=o.nextSub
			}
		}
	}
	else if (o.currentSub!=-1)
	{
		// hide current sub-menu
		var currentItem=o.items[o.currentSub]
		if (currentItem&&currentItem.sub)
		{
			currentItem.sub.show(false)
			o.currentSub=-1
		}
	}
}

// ================================================================================

function MenuWidget_write()
// Cancels default widget behaviour
// Return [void]
{
}

// ================================================================================

function MenuWidget_justInTimeInit()
// Initialization called just before showing menu for the first time
// Return [void]
{
	var o=this
	o.layer=getLayer(o.id)
	
	if (o.layer==null)
	{
		targetApp(o.getHTML())
		o.layer=getLayer(o.id)
	}

	o.layer._widget=o.widx
	o.css=o.layer.style	
	
	o.endLink=newWidget("endLink_"+o.id)
	o.endLink.init()	
	o.startLink=newWidget("startLink_"+o.id)
	o.startLink.init()

	var items=o.items
	
	for (var i in items)
		items[i].init()
}

// ================================================================================

function MenuWidget_getHTML()
// PRIVATE get the HTML
// Return [String]
{
	var o=this,items=o.items
	var keysCbs=' onkeydown="'+_codeWinName+'.MenuWidget_keyDown(\''+o.id+'\',event);return true" '

	// we'll add the iframe with the show function
	// table inside table to have 1px padding effect over the menu items (fixing IE)
	var s='<a style="position:absolute;left:-30px;top:-30px; visibility:hidden" id="startLink_'+o.id+'" href="javascript:void(0)" onfocus="'+_codeWinName+'.MenuWidget_keepFocus(\''+o.id+'\');return false;" ></a><table style="display:none;" class="menuFrame" id="'+o.id+'" cellspacing="0" cellpadding="0" border="0" '+keysCbs+' dir="ltr" role="menu"><tbody><tr><td><table cellspacing="0" cellpadding="0" border="0"><tbody>'
	for (var i=0,l=items.length; i< l; i++)
	{
		items[i].needsRightPart = o.accelEnabled
		s+=items[i].getHTML()
	}

	s+='</tbody></td></tr></tbody></table><a style="position:absolute;left:-30px;top:-30px; visibility:hidden" id="endLink_'+o.id+'" href="javascript:void(0)" onfocus="'+_codeWinName+'.MenuWidget_keepFocus(\''+o.id+'\');return false;" ></a>'

	return s
}

// ================================================================================

function MenuWidget_show(show,x,y,parentPropagate,parentMenuW,buttonFrom)
// show            [boolean] if true shows the menu, otherwise hide it
// x               [int] menu abscissa (may be changed if the menu is outside the window)
// y               [int] menu ordinate (may be changed if the menu is outside the window)
// parentPropagate [boolean - optional] only active if show=false. Close also all parent menus
// parentMenuW     [int - optional] only used internally
// Return [void]
{
	var o=this
	
	if (o.layer==null)
		o.justInTimeInit()

	var css=o.css 
	
	if (show)
	{
		o.iframeLyr=getDynamicBGIFrameLayer()
		o.iframeCss=o.iframeLyr.style
		
		if (o.beforeShowCB)
			o.beforeShowCB()
		
		//User Rights: don't show menu if it contains no visible menu
		if(!o.hasVisibleItem()) return;
		
		o.captureClicks()
	
		// Show and place menu	
		css.display='block'
		css.zIndex=(o.zIndex+1)
		css.visibility="hidden"
		css.left="-1000px"
		css.top="-1000px"
		
		var w=o.getWidth()
		var h=o.getHeight()
		
		if (o.alignLeft)
			x-=w

		// buttonFrom means that we a are in a menu triggered by a button
		// (example : IconMenuWidget
		
		if (buttonFrom)
		{
			var buttonW=buttonFrom.getWidth()
			
			// Align the menu at the button right
			// if the button is larger than the menu
			if (buttonW>w)
				x=x+buttonW-w
		}
	
		// Change coordinates if the menu is out of the window
		var x2=x+w+4,y2=y+h+4

		if (x2-getScrollX()>winWidth())
		{
			if (buttonFrom)
			{
				// If the menu go out of the container windows
				// align to the the right of the window
				x=Math.max(0,winWidth()-w)
			}
			else
				x=Math.max(0,x-4-(w+((parentMenuW!=null)?parentMenuW-12:0)))
		}

		if (y2-getScrollY()>winHeight())
			y=Math.max(0,y-4-h+(parentMenuW!=null?30:0))
	
		css.left=""+x+"px"
		css.top=""+y+"px"
		
		css.visibility="visible"

		// Show and place menu shadow
		iCss=o.iframeCss
		iCss.left=""+x+"px"
		iCss.top=""+y+"px"
		iCss.width=""+w+"px"
		iCss.height=""+h+"px"
		iCss.zIndex=o.zIndex-1
		iCss.display='block'
		
		if (_ie)
		{
			y-=2
			x-=2
		}
			
		
		o.nextSub=-1
		o.showSub()
				
		o.focus()
	}
	else
	{											
		if (parentPropagate && o.par && o.par.par)
		{			
			o.par.par.show(show,x,y,parentPropagate)
		} 
		if (o.iframeLyr) {
			releaseBGIFrame(o.iframeLyr.id)
		}	
		css.display='none'
		if (o.iframeCss) {
			o.iframeCss.display='none'
		}
		o.nextSub=-1
		o.showSub()
		if (o.hideCB)
			o.hideCB()				
		
		o.releaseClicks()
	}
}

// ================================================================================

function MenuWidget_setAccelEnabled(newValue)
{
	var o=this
	o.accelEnabled=newValue
}

function MenuWidget_isAccelEnabled(newValue)
{
	var o=this
	return o.accelEnabled
}
// ================================================================================

function MenuWidget_isShown()
// Test if the menu is shown
// Returns [boolean]
{
	var o=this
	if (o.layer==null)
		return false
	else
		return (o.css.display=='block')
}

// ================================================================================

function MenuWidget_hasVisibleItem()
// Test if the menu contains shown items
// Returns [boolean]
{	
	var o=this
	if(o.isMenuColor || o.isCalendar) return true;	
	var items=o.items	
	for (var i in items)
	{
		var item=items[i]
		if (item && !(item.isSeparator==true) && item.isShown)						
			return true;		
	}
	return false
}

//================================================================================

function MenuWidget_getTotalNumItems()
// get the total number of items in the list
{
	var o=this,items=o.items
	return items.length-o.sepCount;
}

//================================================================================

function MenuWidget_resetItemCount()
// resets the item count/numbering to 0
//
// should be called prior to calling updateTooltip if that call to updateTooltip
// will be called more than once
{
	var o=this
	o.itemCount=0
}

//================================================================================

function MenuWidget_resetTooltips()
{
	var o=this
	o.resetItemCount();

	len=o.items.length;
	for (i=0;i<len;i++)
		o.items[i].updateTooltip();
}

// ================================================================================
// ================================================================================
//
// OBJECT newMenuWidgetItem (Constructor)
//
// creates a menu widget item - do not use this constructor directly. use instead
// MenuWidget::add
//
// ================================================================================
// ================================================================================

function newMenuItem(par,id,text,cb,itemNo,icon,dx,dy,disabled,disDx,disDy,isCheck,alt)
// par      [MenuWidget] the parent menu
// id       [String] the item id
// text     [String] the item text
// itemNo	[int] the numbering of the item in the list (starts at 1)
// icon     [String - optional] the item icon url (16x16 pixels)
// dx       [int - optional] the item icon horizontal offset
// dy       [int - optional] the item icon vertical offset
// disabled [boolean - optional] initial disabled state (default is not disabled)
// disDx    [int - optional] the item icon horizontal offset for disabled
// disDy    [int - optional] the item icon vertical offset for disabled
// isCheck  [boolean - optional] default is false - specified if the item is a check box
// Returns  [MenuItem] the new instance
{
	// No parent class
	var o=new Object

	// Private fields
	o.par=par
	o.id=id
	o.text=text
	o.cb=cb
	o.itemNo=itemNo
	o.icon=icon
	o.dx=(dx==null)?0:dx
	o.dy=(dy==null)?0:dy
	o.disDx=(disDx==null)?o.dx:disDx
	o.disDy=(disDy==null)?o.dy:disDy
	o.sub=null
	o.layer=null
	o.iconTDLayer=null
	o.iconLayer=null
	o.textLayer=null
	o.textOnlyLayer=null
	o.accel=null
	o.accelLayer=null
	o.hasNoLayer=false
	o.isSeparator=false
	o.disabled=(disabled!=null)?disabled:false
	o.isShown=true
	o.alt=alt //icon alt att
	o.needsRightPart=true
	
	o.index=_menusItems.length
	_menusItems[o.index]=o
	o.menuIndex=-1
	o.isCheck=isCheck
	o.checked=false
	o.menuItemType=_isNotColor
	o.init=MenuItem_init

	o.leftZoneClass="menuLeftPart"
	o.leftZoneSelClass="menuLeftPartSel"
		
	o.totalNumItems=null

	// Public methods
	o.attachSubMenu=MenuItem_attachSubMenu
	o.getHTML=MenuItem_getHTML
	o.getHTMLPart=MenuItem_getHTMLPart
	o.dynHTML=MenuItem_dynHTML
	o.setDisabled=MenuItem_setDisabled
	o.check=MenuItem_check
	o.isChecked=MenuItem_isChecked
	o.show=MenuItem_show
	o.setText=MenuItem_setText
	o.setIcon=MenuItem_setIcon
	o.setAccelerator=MenuItem_setAccelerator
	o.focus=MenuItem_focus
	o.setTextClass=MenuItem_setTextClass
	o.updateTooltip=MenuItem_updateTooltip
	
	return o
}

function MenuItem_setTextClass(cls)
//change text of menu item
{
	var o=this;
		
	if (o.textOnlyLayer)
	{
		o.textOnlyLayer.className=cls;
	}
}

// ================================================================================

function MenuItem_init()
// Init widget layers
// Return [void]
{
	if (!this.hasNoLayer)
	{
		var o=this,id=o.par.id
		o.layer=getLayer(id+'_item_'+o.id)
		o.layer._boIndex=o.index

		if (!o.isSeparator)
		{
			if ((o.icon!=null)||(o.isCheck))
			{
				o.iconLayer=getLayer(id+'_item_icon_'+o.id)
				o.iconTDLayer=getLayer(id+'_item_td_'+o.id)
			}
			o.textLayer=getLayer(id+'_text_'+o.id)
			o.textOnlyLayer=getLayer(id+'_span_text_'+o.id)
			o.hiddenLabelLayer=getLayer(id+'_hiddenLabel_'+o.id)
			o.accelLayer=getLayer(id+'_accel_'+o.id)
			
			// no need to create tooltip for checked menus here,
			// it will be created in o.check
			if (!o.isCheck)  
				o.updateTooltip()
		}
		
		if (o.isCheck)
		{
			o.check(o.checked,true)
		}
	}
}

// ================================================================================

function MenuItem_attachSubMenu(menu)
// menu [MenuWidget] the menu to be attached
// return [MenuWidget] the menu to be attached
{
	var o=this
	o.sub=menu
	menu.par=o
	menu.zIndex=o.par.zIndex+2
	
	if (o.layer)
	{
		if (o.arrowLayer==null)
			o.arrowLayer=getLayer(o.par.id+'_item_arrow_'+o.id)
		var dis=o.disabled
		changeSimpleOffset(o.arrowLayer,dis?7:0,dis?81:64)
	}
	
	return menu
}

// ================================================================================

function MenuItem_check(check,force)
// check/unchecks an item - only for items that a declared as check boxes
// check  [boolean] if true checks the item
// Return [void]
{
	var o=this
	
	if ((o.checked!=check)||force)
	{
		o.checked=check
	
		if (o.par.layer)
		{
			// Dynamic check
			var lyr=o.layer
			if (lyr)
			{
				if (o.icon==null)
					changeSimpleOffset(o.iconLayer,0,(o.checked?48:0),null,(o.checked?_menuCheckLab:""))
				changeOffset(o.iconTDLayer,0,(o.checked?0:0))
				
				if (o.checkFrame==null)
					o.checkFrame=getLayer(o.par.id+'_item_check_'+o.id)
				o.checkFrame.className='menuIcon'+(o.checked?"Check":"")
				
				o.updateTooltip()
			}
		}
	}
}

// ================================================================================

function MenuItem_setDisabled(dis)
// disables a menu item
// dis    [boolean] if true disables the item
// Return [void]
{
	var o=this
	
	if (o.disabled!=dis)
	{
		o.disabled=dis
	
		if (o.par.layer)
		{
			// Dynamic set disabled
	
			var lyr=o.layer
			if (lyr)
			{
				lyr.style.cursor=dis?'default':_hand
				
				if (o.icon)
					changeSimpleOffset(o.iconLayer,dis?o.disDx:o.dx,dis?o.disDy:o.dy)
					
				var cn='menuTextPart'+(o.disabled?'Disabled':'')
				
				if (cn!=o.textLayer.className)
					o.textLayer.className=cn
				
				if (o.accel && (cn!=o.accelLayer.className))
					o.accelLayer.className=cn
									
				if (o.sub)
				{
					if (o.arrowLayer==null)
						o.arrowLayer=getLayer(o.par.id+'_item_arrow_'+o.id)
					changeSimpleOffset(o.arrowLayer,dis?7:0,dis?81:64)					
				}
				
				o.updateTooltip()
			}
		}
	}
}

// ================================================================================

function _mii(lyr,inv)
// PRIVATE invert a menu item
// lyr [dom element] the <TR> element to invert
// inv [1 or 0] if 1 the menu must have a selected look
// Return [void]
{
	var c=lyr.childNodes,y=0,len=c.length,idx=lyr._boIndex

	var o=_menusItems[idx]
	
	if (o.disabled)
		inv=0
	else
	{
		if (inv)
		{
			o.par.nextSub=o.menuIndex
			MenuItem_callShowSub(idx,true)
			if (o.par.par)
			{
				if (o.par.par.par)
				{
					o.par.par.par.nextSub=o.par.par.menuIndex
				}
			}
		}
	}
 	
 	var realPart=0
 	
 	for (var i=0;i<len;i++)
 	{
 		var ce=c[i]
 		if (ce.tagName!=null)
 		{

 			if (realPart==0)
 				ce.className=inv?o.leftZoneSelClass:o.leftZoneClass
 			else if (realPart==1)
 				ce.className="menuTextPart"+(inv?"Sel":"")+(o.disabled?"Disabled":"")
 			else if (o.accel && (realPart==2)) 
 			{
 				ce.className="menuTextPart"+(inv?"Sel":"")+(o.disabled?"Disabled":"")
 				break
 			}	
 			else	
 				ce.className="menuRightPart"+(inv?"Sel":"")

 			 realPart++
 		}
 	}
}

// ================================================================================

function MenuItem_getHTMLPart(part)
{
	var o=this

	switch(part)
	{
		case 0: // the icon on the left
			var im=null,className=' class="menuIcon' + (o.checked?"Check":"")+'"'
			if (o.isCheck&&(o.icon==null))
				im=simpleImgOffset(_skin+"menus.gif",16,16,0,o.checked?48:0,(o.par.id+'_item_icon_'+o.id),null,(o.checked?_menuCheckLab:""))
			else
				im=o.icon?simpleImgOffset(o.icon,16,16,o.disabled?o.disDx:o.dx,o.disabled?o.disDy:o.dy,(o.par.id+'_item_icon_'+o.id),null,o.alt?o.alt:''):(getSpace(16,16))
			
			if (o.isCheck)
			{
				im='<div id="'+o.par.id+'_item_check_'+o.id+'" class="menuIcon'+(o.checked?"Check":"")+'" style="width:16px;height:16px;padding:2px">'+im+'</div>'
			}
			
			return im

		case 1: // the text
			var spanID = (o.par.id+'_span_text_'+o.id);
			var keysCbs=' onkeydown="'+_codeWinName+'._mikd(this, event);return true" ';
			var hiddenLabel = '<label for="' + spanID + '" id="'+(o.par.id+'_hiddenLabel_'+o.id)+'" style="display:none;" ></label>';
			return '<span id="'+ spanID +'" ' + keysCbs + ' tabIndex="0" role="menuitem">'+convStr(o.text)+ '</span>' + hiddenLabel;
		case 2:
			return simpleImgOffset(_skin+"menus.gif",16,16,o.sub?(o.disabled?7:0):0,o.sub?(o.disabled?81:64):0,o.par.id+'_item_arrow_'+o.id, null, null, null,"right")			
		case 3:
			return '<table width="100%" height="3" cellpadding="0" cellspacing="0" border="0" style="'+backImgOffset(_skin+"menus.gif",0,80)+';"><tbody><tr><td></td></tr></tbody></table>'
		case 4:	// the accelerator
			return convStr(o.accel)
	}
}

// ================================================================================

function MenuItem_getHTML()
// Return [String] the HTML
{
	var o=this
	
	if ((o.icon!=null)||(o.text!=null))
	{		
		var invertCbs=' onclick="'+_codeWinName+'._micl(this,event);return true" oncontextmenu="'+_codeWinName+'._micl(this,event);return false" onmouseover="'+_codeWinName+'._mii(this,1)" onmouseout="'+_codeWinName+'._mii(this,0);" '
		var keysCbs=' onkeydown="'+_codeWinName+'._mikd(this,event);return true" '
		var ar=new Array(), i=0
		ar[i++] = '<tr onmousedown="'+_codeWinName+'._minb(event)" onmouseup="'+_codeWinName+'._minb(event)" id="'+(o.par.id+'_item_'+o.id)+'" style="'+(!o.isShown?'display:none;':'')+'height:'+_mitemH+'px;width:24px;cursor:'+(o.disabled?'default':_hand)+'" '+invertCbs+keysCbs+' valign="middle">'
		ar[i++] =	'<td id="'+(o.par.id+'_item_td_'+o.id)+'" style="width:23px;height:'+_mitemH+'px;" align="center" class="'+o.leftZoneClass+'">'
		ar[i++] =		o.getHTMLPart(0)
		ar[i++] =	'</td>'	
		ar[i++] =	'<td ' +(o.centered?' align="center" ':'')+' style="height:'+_mitemH+'px" id="'+(o.par.id+'_text_'+o.id)+'" class="menuTextPart'+(o.disabled?'Disabled':'')+'">'
		ar[i++] =		o.getHTMLPart(1)
		ar[i++] =	'</td>'
		if (o.needsRightPart) 
		{
			if (o.accel!=null)
			{
				ar[i++] = '<td class="menuTextPart'+(o.disabled?'Disabled':'') + '" id="'+(o.par.id+'_accel_'+o.id)+'" align="right"' +' style="height:'+_mitemH+'px"  tabIndex="-1">'
				ar[i++] =	o.getHTMLPart(4)
				ar[i++] = '</td>'
			} else {			
				ar[i++] = '<td class="menuRightPart" align="right" style="width:40px;height:'+_mitemH+'px;" >'
				ar[i++] =	o.getHTMLPart(2)
				ar[i++] = '</td>'
			}
		}
		else // 2px width space to close the highlight border
		{
			ar[i++] = '<td class="menuRightPart" align="right" style="width:2px;height:'+_mitemH+'px;" >'
			ar[i++] =	img(_skin+'../transp.gif',1,1,null,null,null)
			ar[i++] = '</td>'
		}
		ar[i++] = '</tr>'
		return ar.join('')
	}
	else
	{
		return '<tr onmousedown="'+_codeWinName+'._minb(event)" onclick="'+_codeWinName+'._minb(event)" id="'+(o.par.id+'_item_'+o.id)+'" onmouseup="'+_codeWinName+'._minb(event)" style="height:3px">'+
			'<td class="'+o.leftZoneClass+'" style="width:24px;height:3px;border:0px"></td>'+
			'<td colspan="2" style="padding-left:5px;padding-right:5px;border:0px">'+
				o.getHTMLPart(3)+
			'</td></tr>'
	}
}

// ================================================================================

function MenuItem_dynHTML()
{
	var o=this
	if (o.par.layer==null)
		return

	var tbody=o.par.layer.childNodes[0],tr=tbody.insertRow(o.menuIndex),st=tr.style
	
	tr.onmousedown=_minb
	tr.onmouseup=_minb
	tr.id=(o.par.id+'_item_'+o.id)
	
	if ((o.icon!=null)||(o.text!=null))
	{
		var td1=tr.insertCell(0),td2=tr.insertCell(1),td3=tr.insertCell(2),st1=td1.style,st2=td2.style,st3=td3.style

		tr.onclick=MenuItem_clickCallTrue
		tr.oncontextmenu=MenuItem_clickCallFalse
		tr.onmouseover=MenuItem_invertCall1
		tr.onmouseout=MenuItem_invertCall0

		st.height=""+_mitemH+"px"
		st.width="24px"
		st.cursor=(o.disabled?'default':_hand)
		
		td1.id=(o.par.id+'_item_td_'+o.id)
		st1.width="23px"
		st1.height=""+_mitemH+"px"
		td1.innerHTML=o.getHTMLPart(0)
		td1.align="center"
		td1.className=o.leftZoneClass

		if (o.centered)
			td2.align="center"
		st2.height=""+_mitemH+"px"
		td2.id=(o.par.id+'_text_'+o.id)
		td2.className="menuTextPart"+(o.disabled?'Disabled':'')
		td2.innerHTML=o.getHTMLPart(1)

		if (o.accel) 
		{
			td3.className="menuTextPart"+(o.disabled?'Disabled':'')
			td3.align="right"
			//st3.width="24px"						
			st3.height=""+_mitemH+"px"
			//st3.paddingLeft="4px"
			//st3.paddingRight="4px"			
			td3.innerHTML=o.getHTMLPart(4)
		} else {		
			td3.className="menuRightPart"
			td3.align="right"
			st3.width="40px"
			st3.height=""+_mitemH+"px"		
			td3.innerHTML=o.getHTMLPart(2)
		}

		
		o.init()
	}
	else
	{
		tr.onclick=_minb
		tr.style.height="3px"
		
		var td1=tr.insertCell(0),td2=tr.insertCell(1),st1=td1.style,st2=td2.style

		td1.className=o.leftZoneClass
		st1.width="24px"
		st1.height="3px"
		st1.border="0px"
		
		td2.colSpan="2"
		st2.paddingLeft="5px"
		st2.paddingRight="5px"
		td2.innerHTML=o.getHTMLPart(3)
	}		
}

// ================================================================================

function MenuItem_isChecked()
// test if a check box item is checked
// Return [boolean]
{
	return this.checked
}


// ================================================================================

function MenuItem_setText(s)
//change text of menu item
{
	var o=this,id=o.par.id
	o.text=s	
	if (o.textLayer)
	{
		o.textLayer.innerHTML=o.getHTMLPart(1)
		o.textOnlyLayer=getLayer(id+'_span_text_'+o.id)
	}
}

// ================================================================================

function MenuItem_setAccelerator(keystroke, modifier)
// keystroke char corresponding to a keyboard key
// modifier int _ctrl=0,_shift=1,_alt=2
//add an accelerator(key combination) to the text of menu item
{
	var o=this,id=o.par.id	
	o.accel= ((modifier != null)?_modifiers[modifier]:"") + keystroke	
	if (o.accelLayer)
	{
		o.accelLayer.innerHTML=o.getHTMLPart(4)		
	}
}

// ================================================================================

function MenuItem_setIcon(dx,dy,disDx,disDy,url)
// Change icon
{
	var o=this
	o.url = url ? url : o.url
	o.dx = (dx != null) ? dx : o.dx
	o.dy = (dy != null) ? dy : o.dy
	o.disDx = (disDx != null) ? disDx : o.disDx
	o.disDy = (disDy != null) ? disDy : o.disDy
		
	if (o.icon && o.iconLayer)
		changeSimpleOffset(o.iconLayer,o.disabled?o.disDx:o.dx, o.disabled?o.disDy:o.dy,o.url)
}


// ================================================================================

function MenuItem_show(sh)
// show/hide a menu item
// sh     [boolean] if true shows the item
// Return [void]
{
	var o=this
	o.isShown=sh
	
	if (o.layer!=null)
		o.layer.style.display=sh?'':'none'
}

// ================================================================================

function _micl(lyr,e)
// PRIVATE - DHTML Callback
// lyr [dom element]   the <TR> element to invert
// e   [event pointer] the event
// Return [void]
{
	eventCancelBubble(e)
	var idx=lyr._boIndex,o=_menusItems[idx]
	o.layer=lyr
	
	if (!o.disabled)
	{
		if (o.sub)
		{
			o.par.nextSub=o.menuIndex
			MenuItem_callShowSub(idx)
		}
		else
		{
			o.par.show(false,0,0,true)

			if (o.isCheck)
			{
				if (o.par.uncheckAll)
					o.par.uncheckAll()
				o.check(!o.checked)
			}
			if (o.par.container && o.par.container.updateButton)
				o.par.container.updateButton(idx)

			_mii(lyr,0,idx)
			o.par.nextSub=-1
			if (o.cb)
				setTimeout("MenuItem_delayedClick("+idx+")",1)
		}
	}
}

// ================================================================================

function _mikd(lyr,e)
{	
	while(lyr && !lyr._boIndex)
		lyr = lyr.parentNode;
	
	if(!lyr || !lyr._boIndex)
		return;
	
	var idx=lyr._boIndex,o=_menusItems[idx]
	o.layer=lyr		
	var k=eventGetKey(e)	
	switch(k)
	{
		case 32:
		case 13://enter
			_micl(lyr,e)
		break;	
		
		case 107://expanded	(+ ou ->)			
		case 39:	
			if (!o.disabled && o.sub )
			{
				_micl(lyr,e)
			}
		break;
		case 109://collapse (- ou <-) 
		case 37:
			//let menu does action
		break;
		case 40://next 
			var items=o.par.items, len = items.length
			for(var i=o.menuIndex+1;i<len;i++)
			{
				if(items[i].isShown && !items[i].isSeparator)
				{			
					items[i].focus()
					break;
				}
			}
		break;
		case 38://previous
			var items=o.par.items, len = items.length
			for(var i=o.menuIndex-1;i>=0;i--)
			{
				if(items[i].isShown && !items[i].isSeparator)
				{			
					items[i].focus()
					break;
				}
			}
		break;		
	}	
}						

// ================================================================================

function MenuItem_callShowSub(idx,delayed)
// PRIVATE - DHTML Callback, show a sub menu
// idx     [int]  the menu item indes in the _menusItems array
// delayed [boolean - optional] if true show the menu with a delay (used for rollovers)
// Return [void]
{
	var o=_menusItems[idx]

	if (delayed)
		setTimeout('MenuItem_delayedShowSub('+idx+')',500)
	else
		MenuItem_delayedShowSub(idx)
}

// ================================================================================

function MenuItem_delayedShowSub(idx)
// PRIVATE - DHTML Callback, show a sub menu
// idx     [int]  the menu item indes in the _menusItems array
// Return [void]
{
	var o=_menusItems[idx]
	o.par.showSub()
}

// ================================================================================

function _minb(e)
// PRIVATE Cancel event bubble
// Return [void]
{
	eventCancelBubble(e)
}

// ================================================================================

function MenuItem_delayedClick(idx)
// idx [int]  the menu item index in the _menusItems array
// Return [void]
{
	var item=_menusItems[idx]
	if (item.cb)
		item.cb()
}

// ================================================================================

function MenuItem_clickCallTrue(event)
// PRIVATE
{
	_micl(this,event)
	return true
}

// ================================================================================

function MenuItem_clickCallFalse(event)
// PRIVATE
{
	_micl(this,event)
	return false
}

// ================================================================================

function MenuItem_invertCall0(event)
// PRIVATE
{
	_mii(this,0)
}

// ================================================================================

function MenuItem_invertCall1(event)
// PRIVATE
{
	_mii(this,1)
}

// ================================================================================

function MenuItem_focus()
//give focus to menu item
{	
	var o=this
		
	if(isLayerDisplayed(o.layer) && o.textOnlyLayer && o.textOnlyLayer.focus)
	{			
		o.textOnlyLayer.focus();	
	}
		
}

//================================================================================

function MenuItem_updateTooltip()
// update tooltip for menu item
{
	var o=this
	
	if (o.textOnlyLayer && !o.isSeparator)
	{
		if(o.textOnlyLayer.innerHTML)
			o.textOnlyLayer.title=o.textOnlyLayer.innerHTML
		o.textOnlyLayer.title+=o.checked?" "+_menuCheckLab:""			
		if (o.disabled) 
		    o.textOnlyLayer.title = (o.textOnlyLayer.title != null ? o.textOnlyLayer.title : "") + " " + _menuDisableLab
		
		if(o.hiddenLabelLayer) {
			var tooltip = ((_moz && o.textOnlyLayer.title) ? o.textOnlyLayer.title : "") +  o.itemNo + _of + o.par.getTotalNumItems();
			o.hiddenLabelLayer.innerHTML = tooltip;
		}
	}
}

// ================================================================================
// ================================================================================
//
// OBJECT newMenuColorWidget (Constructor)
//
// creates a color selector menu widget
//
// ================================================================================
// ================================================================================

//function newMenuColorWidget(id,hideCB)
//// id      [String] the menu id for DHTML processing
//// hideCB  [Function - Optional] callback called when the menu is closed
//// Returns [MenuColorWidget] the new instance
//{
//	var o=newMenuWidget(id,hideCB)
//
//	o.addSeparator=null
//	o.lastUsedTxt=""
//	o.lastUsedColorsAr=null
//	o.addColor=MenuColorWidget_addColor
//	o.addLastUsed=MenuColorWidget_addLastUsed
//	o.getHTML=MenuColorWidget_getHTML
//	o.uncheckAll=MenuColorWidget_uncheckAll
//	
//	o.isMenuColor=true
//		
//	return o
//}
//
//// ================================================================================
//
//function MenuColorWidget_addColor(tooltip,color,cb)
//// tooltip [String] the color name
//// color   [String] the color code r,g,b ( 0 <= rgb <= 255)
//// cb      [Function - Optional] the click Callback
//// Return  [MenuItem] the new menu item
//{
//	var o=this,i=o.items.length
//	var ret=o.items[i]=newColorMenuItem(o,color,tooltip,cb)
//	ret.menuIndex=i
//	return ret
//}
//
//// ================================================================================
//
//function MenuColorWidget_addLastUsed(text,lastUsedColorsAr,cb, beforeShowCB)
//// text [String] the menu label
//// lastUsedColorsAr   [Array] a reference to the stack of last used colors
//{
//	var o=this
//	o.lastUsedTxt = text
//	o.lastUsedColorsAr = lastUsedColorsAr
//	o.beforeShowCB = MenuColorWidget_beforeShowCB
//	colorsMax = 8
//	len = o.items.length
//	var it = null
//	
//	for (var c = 0; c < colorsMax; c++)
//	{					
//		it = newLastUsedColorMenuItem(o,c,lastUsedColorsAr[c],"",cb)
//		it.isLast = (c == colorsMax-1) ? true : false // is it the last 'last used' color item in the item list
//		o.items[len + c] = it
//	}	
//}
//
//// ================================================================================
//
//function MenuColorWidget_getHTML()
//// PRIVATE get the HTML
//// Return [String]
//{
//	var o=this,items=o.items
//	var j = 0
//	var keysCbs=' onkeydown="'+_codeWinName+'.MenuWidget_keyDown(\''+o.id+'\',event);return true" '
//	var s = new Array
//
//	s[j++] = '<a style="position:absolute;left:-30px;top:-30px; visibility:hidden" id="startLink_'+o.id+'" href="javascript:void(0)" onfocus="'+_codeWinName+'.MenuWidget_keepFocus(\''+o.id+'\');return false;" ></a>'
//	s[j++] = '<table style="display:none;" class="menuFrame" id="'+o.id+'" cellspacing="0" cellpadding="0" border="0"'+keysCbs+'><tbody>'
//	var sep = '<tr style="height:3px"><td colspan="8" style="padding-left:5px;padding-right:5px;"><table width="100%" height="3" cellpadding="0" cellspacing="0" border="0" style="'+backImgOffset(_skin+"menus.gif",0,80)+';"><tbody><tr><td></td></tr></tbody></table></td></tr>'
//	var len = items.length
//
//	lastUsedCol=""
//	lastUsedColIconsNb = 0
//	lastUsedColIconsMaxLine = 3
//	for (var i in items)
//	{
//		var item=items[i]
//		switch (item.menuItemType)
//		{
//			case _isColor:
//				s[j++] = item.getHTML()
//				break;
//			
//			case _isLastUsedColor:
//				lastUsedCol += item.getHTML()
//				lastUsedCol += (lastUsedColIconsNb++ == lastUsedColIconsMaxLine)? "</tr><tr>":""
//				
//				if (item.isLast)
//				{
//					s[j++] = sep
//					s[j++] = '<tr><td colspan="8">'
//					s[j++] =		'<table border="0" cellspacing="0" cellpadding="0" width="100%"><tbody><tr>'					
//					s[j++] =			'<td width="50%" class="menuTextPart">' + convStr(o.lastUsedTxt) + '</td>'
//					s[j++] =			'<td><table border="0" cellspacing="0" cellpadding="0"><tbody><tr>'
//					s[j++] =				lastUsedCol
//					s[j++] =			'</tr></tbody></table></td>'	
//					s[j++] =		'</tr></tbody></table>'
//					s[j++] = '</td></tr>'
//					s[j++] = sep
//				}
//				break;
//			
//			case _isNotColor:
//	
//				item.leftZoneClass="menuLeftPartColor"
//				item.leftZoneSelClass="menuLeftPartSel"
//				item.centered=true
//				s[j++] ='<tr><td colspan="8"><table border="0" cellspacing="0" cellpadding="0" width="100%"><tbody><tr>'+item.getHTML()+'</tr></tbody></table></td></tr>'
//				s[j++] = (i == 0 )? sep:""
//		}	
//	}
//
//	s[j++] ='</tbody></table><a style="position:absolute;left:-30px;top:-30px; visibility:hidden" id="endLink_'+o.id+'" href="javascript:void(0)" onfocus="'+_codeWinName+'.MenuWidget_keepFocus(\''+o.id+'\');return false;" ></a>'
//	
//	return s.join("")
//}
//
//// ================================================================================
//
//function MenuColorWidget_beforeShowCB()
//{
//
//	var o=this, j=0
//	lenLastUsed = o.lastUsedColorsAr.length
//
//	if ((lenLastUsed == 1) && ((o.lastUsedColorsAr[0].length == 0) || (o.lastUsedColorsAr[0] == "null"))) {
//		lenLastUsed = 0
//		o.lastUsedColorsAr.length = 0
//	}
//
//	for (var i in o.items)
//	{
//		var item=o.items[i]
//		if (item.menuItemType == _isLastUsedColor) 
//		{						
//			if (j < lenLastUsed)
//			{			
//				item.init()
//				var c = o.lastUsedColorsAr[j++]
//				item.color = c				
//				item.layer.childNodes[0].childNodes[0].style.backgroundColor = 'rgb(' + c + ')'
//										
//				var t = _colorsArr[""+c+""]				
//				item.text = (t)? t:(_RGBTxtBegin + c + _RGBTxtEnd)
//				item.layer.childNodes[0].childNodes[0].childNodes[0].title = item.text
//				
//				item.show(true)
//			} else {
//				item.show(false)
//			}
//		} 
//	}	
//}
//
//// ================================================================================
//
//function MenuColor_invert(lyr,inv)
//// PRIVATE - DHTML Callback
//// lyr    [DOM Element] the element
//// inv    [0..1] 1 to invert
//// Return [void]
//{
//	var o=_menusItems[lyr._boIndex]
//	if (o && o.checked)
//		inv=1
//
//	lyr.childNodes[0].className="menuColor"+(inv?"sel":"")
//}
//
//// ================================================================================
//
//function MenuColor_out()
//// PRIVATE
//{
//	MenuColor_invert(this,0);
//}
//
//// ================================================================================
//
//function _Mcov(l)
//// PRIVATE
//{
//	l.onmouseout=MenuColor_out
//	MenuColor_invert(l,1);
//}
//
//// ================================================================================
//
//function MenuColorWidget_uncheckAll()
//// Uncheck all items
//// Return [void]
//{
//	var o=this,items=o.items
//
//	for (var i in items)
//	{
//		var item=items[i]
//		if (item.checked)
//			item.check(false)
//	}	
//}
//
//// ================================================================================
//
//function _mcc(lyr,e)
//// PRIVATE - DHTML Callback
//// lyr    [DOM Element] the element
//// e      [Event] DHTML Event
//// Return [void]
//{
//	eventCancelBubble(e)
//	var idx=lyr._boIndex,o=_menusItems[idx]
//	o.par.uncheckAll()
//
//	MenuColor_invert(lyr,1,idx)
//	o.checked=true
//
//	o.par.show(false,0,0,true)
//
//	if (o.cb)
//		setTimeout("MenuItem_delayedClick("+idx+")",1)	
//}
//
//// ================================================================================
//// ================================================================================
////
//// OBJECT newColorMenuItem (Constructor)
////
//// creates a color widget item - a color square with the color name as tooltip
////
//// ================================================================================
//// ================================================================================
//
//function newColorMenuItem(par,color,text,cb)
//// par     [MenuWidget] the parent menu
//// color   [String] color (r,g,b)
//// text    [String] the item text
//// Returns [ColorMenuItem] the new instance
//{
//	var o=newMenuItem(par,"color_"+color,text,cb)
//	o.color=color
//
//	// Public methods
//	o.attachSubMenu=null
//	o.getHTML=ColorMenuItem_getHTML
//	o.check=ColorMenuItem_check
//	o.menuItemType=_isColor
//
//	return o
//}
//
//// ================================================================================
//
//function ColorMenuItem_check(check)
//// Check a color menu item
//// check  [boolean] checks the item if true
//// Return [void]
//{
//	var o=this
//	
//	if (o.checked!=check)
//	{
//		o.checked=check
//	
//		if (o.layer)
//			MenuColor_invert(o.layer,o.checked?1:0)
//	}
//}
//
//// ================================================================================
////
//// PUBLIC ColorMenuItem::getHTML
////
//// Write the color menu widget HTML
////
//// Returns
////
//// [String] the HTML
////
//// ================================================================================
//
//function ColorMenuItem_getHTML()
//{
//	var o=this,s="",d=_moz?10:12,lenTotal=o.par.items.length,index=o.menuIndex - 1;col=index%8
//	var len=0
//	
//	for (var i = 0; i <lenTotal; i++)
//	{
//		if (o.par.items[i].menuItemType == _isColor) len++
//	}
//	
//	var first=(col==0)
//	var last=(col==7)
//	var firstL=(index<8)
//	var lastL=(index>=(Math.floor((len-1)/8)*8))
//	var cbs=' onclick="'+_codeWinName+'._mcc(this,event);return true" oncontextmenu="'+_codeWinName+'._mcc(this,event);return false" onmousedown="'+_codeWinName+'._minb(event)" onmouseup="'+_codeWinName+'._minb(event)" onmouseover="'+_codeWinName+'._Mcov(this)" '
//
//	if (first)
//		s+='<tr valign="middle" align="center">'
//
//	s+='<td id="'+(o.par.id+'_item_'+o.id)+'" '+cbs+' style="padding-top:'+(firstL?2:0)+'px;padding-bottom:'+(lastL?2:0)+'px;padding-left:'+(first?3:1)+'px;padding-right:'+(last?3:1)+'px"><div class="menuColor'+(o.checked?'Sel':'')+'"><div style="cursor:'+_hand+';border:1px solid #4A657B;width:'+d+'px;height:'+d+'px;background-color:rgb('+o.color+');">'+img(_skin+'../transp.gif',10,10,null,null,o.text)+'</div></div></td>'
//	
//	/*if (o.menuIndex==len-1)
//	{
//		for (var i=col;i<8;i++)
//			s+='<td class="menuLeftPart"></td>'
//	}*/
//
//
//	if (last)
//		s+='</tr>'
//
//	return s
//}

// ================================================================================
// ================================================================================
//
// OBJECT newLastUsedColorMenuItem (Constructor)
//
// creates a color widget item - a color square with the color name as tooltip
//
// ================================================================================
// ================================================================================
//
//function newLastUsedColorMenuItem(par,idx,color,text,cb)
//// par     [MenuWidget] the parent menu
//// color   [String] color (r,g,b)
//// text    [String] the item text
//// Returns [ColorMenuItem] the new instance
//{
//	var o=newMenuItem(par,"color_"+idx,text,cb)
//	o.idx=idx
//	o.color=color
//	o.menuItemType = _isLastUsedColor
//
//	// Public methods
//	o.attachSubMenu=null
//	o.check=ColorMenuItem_check		
//	o.getHTML=LastUsedColorMenuItem_getHTML
//	o.init=LastUsedColorMenuItem_init
//	return o
//}
//
//// ================================================================================
////
//// PUBLIC lastUsedColorMenuItem_getHTML::getHTML
////
//// Write the color menu widget HTML
////
//// Returns
////
//// [String] the HTML
////
//// ================================================================================
//
//function LastUsedColorMenuItem_getHTML()
//{
//	var o=this,s="",d=_moz?10:12
//	var cbs=' onclick="'+_codeWinName+'._mcc(this,event);return true" oncontextmenu="'+_codeWinName+'._mcc(this,event);return false" onmousedown="'+_codeWinName+'._minb(event)" onmouseup="'+_codeWinName+'._minb(event)" onmouseover="'+_codeWinName+'._Mcov(this)" '
//
//	s+='<td id="'+(o.par.id+'_item_'+o.id)+'" width="18" '+cbs+' style="padding-top:0px;padding-bottom:0px;padding-left:1px;padding-right:1px"><div class="menuColor'+(o.checked?'sel':'')+'"><div style="cursor:'+_hand+';border:1px solid #4A657B;width:'+d+'px;height:'+d+'px;background-color:rgb('+o.color+');">'+img(_skin+'../transp.gif',10,10,null,null,o.text)+'</div></div></td>'
//
//	return s
//}
//
//// Init widget layers
//// Return [void]
//function LastUsedColorMenuItem_init()
//{
//	if (!this.hasNoLayer)
//	{
//		var o=this,id=o.par.id
//		o.layer=getLayer(id+'_item_'+o.id)
//		o.layer._boIndex=o.index
//
//		if (o.isCheck)
//		{
//			o.check(o.checked,true)
//		}
//	}
//}

// ================================================================================
// ================================================================================
//
// OBJECT newScrollMenuWidget (Constructor)
//
//
// ================================================================================
// ================================================================================

function newScrollMenuWidget(id,changeCB,multi,width,lines,tooltip,dblClickCB,keyUpCB,
	showLabel,label,convBlanks,beforeShowCB,menuClickCB)
// CONSTRUCTOR
// id			[String]			the menu id for DHTML processing
// changeCB		[Function]			calback when selection is changed by the user
// multi		[boolean]			if true, multiselection is enabled
// lines		[int]				number of visible lines
// tooltip		[String - optional] tooltip for 508
// dblClickCB	[Function]			calback when an item is double-clicked
// keyUpCB		[Function]			
// showLabel	[boolean]			if true a label is displayed
// label		[boolean - optional]text of the label
// convBlanks	[int - optional]	
// beforeShowCB  [Function - Optional] callback called before menu is shown
{
	var o=newWidget(id)
	
	// Properties
	o.list=newListWidget("list_"+id,ScrollMenuWidget_changeCB,multi,width,lines,tooltip,ScrollMenuWidget_dblClickCB,ScrollMenuWidget_keyUpCB,ScrollMenuWidget_clickCB)
	o.list.par=o
	o.label=NewLabelWidget("label_"+id,label,convBlanks)
	o.showLabel=showLabel
	o.changeCB=changeCB
	o.menuClickCB=menuClickCB
	o.dblClickCB=dblClickCB
	o.keyUpCB=keyUpCB	
	o.beforeShowCB=beforeShowCB
	o.zIndex=_menusZIndex
	
	// Methods
	o.init=ScrollMenuWidget_init
	o.justInTimeInit=ScrollMenuWidget_justInTimeInit
	o.setDisabled=ScrollMenuWidget_setDisabled
	o.write=ScrollMenuWidget_write

	o.getHTML=ScrollMenuWidget_getHTML
	o.show=ScrollMenuWidget_show
	o.add=ScrollMenuWidget_add
	
	o.del=ScrollMenuWidget_del
	o.getSelection=ScrollMenuWidget_getSelection
	o.select=ScrollMenuWidget_select
	o.clearSelection=ScrollMenuWidget_clearSelection 
	o.valueSelect=ScrollMenuWidget_valueSelect
	o.getCount=ScrollMenuWidget_getCount

	// Re-use of existing MenuWidget methods
	o.isShown=MenuWidget_isShown
	o.captureClicks=MenuWidget_captureClicks
	o.releaseClicks=MenuWidget_releaseClicks

	// Click capture
	o.clickCB=new Array
	o.clickCBDocs=new Array
	
	return o
}

// ================================================================================

function ScrollMenuWidget_init()
// Do nothing : should not be used
// Return	[void]
{
}

//================================================================================

function ScrollMenuWidget_clearSelection()
{
    var o=this;
    if(o.list) 
        o.list.clearSelection();
}

// ================================================================================

function ScrollMenuWidget_justInTimeInit()
// Initialization called just before showing scrollMenu for the first time
// Return [void]
{
	var o=this
	o.layer=getLayer(o.id)
	
	if (o.layer==null)
	{
		append2(_curDoc.body,o.getHTML());
		o.layer=getLayer(o.id)
	}

	o.layer._widget=o.widx
	o.css=o.layer.style
	o.css.visibility="hidden"
	
	//o.iframeLyr=getLayer("menuIframe_"+o.id)
	//o.iframeCss=o.iframeLyr.style
	
	o.list.init()
	o.label.init()
}

// ================================================================================

function ScrollMenuWidget_setDisabled()
{
	// To be implemented
}

// ================================================================================

function ScrollMenuWidget_write()
// Do nothing : should not be used
// Return	[void]
{
}

// ================================================================================

function ScrollMenuWidget_getHTML()
// Return [string]	the HTML sorce of this widget
{
	var o=this
	
	var s=''
	//s+=o.getShadowHTML()
	s+='<table dir="ltr" onmousedown="event.cancelBubble=true" id="'+o.id+'" style="display:none;" class="menuFrame" cellspacing="0" cellpadding="0" border="0"><tbody>'
	s+='<tr><td align="center">'+o.list.getHTML()+'</td></tr>'
	s+='<tr><td align="center">'+o.label.getHTML()+'</td></tr>'
	s+='</tbody></table>'
	
	return s
}

// ================================================================================

function ScrollMenuWidget_show(show,x,y)
// show		[boolean] if true shows the menu, otherwise hide it
// x        [int] menu abscissa (may be changed if the menu is outside the window)
// y        [int] menu ordinate (may be changed if the menu is outside the window)
// Return	[void]
{
	var o=this
	
	if (o.layer==null)
		o.justInTimeInit()

	var css=o.css
	
	if (show)
	{
		if (o.beforeShowCB)
			o.beforeShowCB()
		
		o.captureClicks()
	
		// Show and place menu	
		css.display='block'
		css.zIndex=(o.zIndex+1)
		css.visibility="hidden"
		css.left="-1000px"
		css.top="-1000px"
		
		var w=o.getWidth()
		var h=o.getHeight()
		
		if (o.alignLeft)
			x-=w
	
		// Change coordinates if the menu is out of the window
		var x2=x+w+4,y2=y+h+4

		if (x2>winWidth())
			x=Math.max(0,x-4-w)

		if (y2>winHeight())
			y=Math.max(0,y-4-h)
	
		css.left=""+x+"px"
		css.top=""+y+"px"
		
		//hideAllInputs(x,y,w+4,h+4)
		
		css.visibility="visible"

		// Show and place menu shadow
		o.iframeLyr=getDynamicBGIFrameLayer()
		o.iframeCss=o.iframeLyr.style
		iCss=o.iframeCss
		iCss.left=""+x+"px"
		iCss.top=""+y+"px"
		iCss.width=""+w+"px"
		iCss.height=""+h+"px"
		iCss.zIndex=o.zIndex-1
		iCss.display='block'
		
		if (_ie)
		{
			y-=2
			x-=2
		}
			
	}
	else
	{
		releaseBGIFrame(o.iframeLyr.id)
		
		css.display='none'
		iCss.display='none'
		o.releaseClicks()
	}
}

// ================================================================================

function ScrollMenuWidget_add(s,val,sel,id)
// Add an item to the menu
// s		[string]	text displayed
// val		[string]	value associated with the text
// sel      [boolean]	if true, the <s> is selected
// id       [int]		
// Return	[void]
{
	var o=this
	
	if (o.layer==null)
		o.justInTimeInit()
		
	o.list.add(s,val,sel,id)
}

// ================================================================================

function ScrollMenuWidget_del(i)
// Delete an item from the menu
// i		[int]	item index
// Return	[void]
{
	var o=this
	
	if (o.layer==null)
		o.justInTimeInit()
	
	o.list.del(i)			
}

// ================================================================================

function ScrollMenuWidget_getSelection()
// Return	[structure]	the item selected (index, value, text)
{
	var o=this
	
	if (o.layer==null)
		o.justInTimeInit()
	
	return o.list.getSelection()
}

// ================================================================================

function ScrollMenuWidget_select(i)
// Select the specified item from its index
// i		[int]	item index
// Return	[void]
{
	var o=this
	
	if (o.layer==null)
		o.justInTimeInit()
	
	o.list.select(i)
}

// ================================================================================

function ScrollMenuWidget_valueSelect(v)
// Select the specified item form its value
// i		[int]	item index
// Return	[void]
{
	var o=this
	
	if (o.layer==null)
		o.justInTimeInit()
	
	o.list.valueSelect(v)
}

// ================================================================================

function ScrollMenuWidget_getCount()
// Return	[int]	the number of item in the menu
{
	var o=this
	
	if (o.layer==null)
		o.justInTimeInit()
		
	return o.list.getCount()
}

// ================================================================================

function ScrollMenuWidget_changeCB()
// Callback called when an item is selected in the menu
// Return	[void]
{
	var o=this
	
	// Don't hide the menu whenever a different item is selected in the list.
	// We want to keep the menu list visible. We'll hide it only when there's a mouse click or Enter key.
	//o.par.show(false)
	
	if (o.par.changeCB)
		o.par.changeCB()
}

// ================================================================================
function ScrollMenuWidget_clickCB()
// Callback called when an item is clicked in the menu
// Return	[void]
{
	var o=this
	
	o.par.show(false)
	
	if (o.par.menuClickCB)
		o.par.menuClickCB()
}

// ================================================================================
function ScrollMenuWidget_dblClickCB()
// Callback called when an item is double-clicked in the menu
// Return	[void]
{
	var o=this
	
	o.par.show(false)
	
	if (o.par.dblClickCB)
		o.par.dblClickCB()
}

// ================================================================================

function ScrollMenuWidget_keyUpCB(e)
// Callback called when an keyup event occurs in the menu
// Return	[void]
{
	var ENTER=13, ESCAPE=27
	var o=this
	var k=eventGetKey(e)
	
	if (k==ENTER || k==ESCAPE)
		o.par.show(false)
	
	if (o.par.keyUpCB)
		o.par.keyUpCB()
}

// ================================================================================
// ================================================================================
//
// OBJECT newButtonScrollMenuWidget (Constructor)
//
//
// ================================================================================
// ================================================================================

//function newButtonScrollMenuWidget(id,label,buttonWidth,buttonTooltip,tabIndex,
//            changeCB,multi,menuWidth,lines,menuTooltip,dblClickCB,keyUpCB,showMenuLabel,menuLabel,convBlanks,beforeShowCB)
//
//{
//	var o=newButtonWidget(id,label,ButtonScrollMenuWidget_clickCB,buttonWidth,null,buttonTooltip,tabIndex,0,_skin+"menus.gif",7,16,0,81,true,0,97)
//
//	o.menu=newScrollMenuWidget("scrollMenu_menu_"+id,changeCB,multi,menuWidth,lines,menuTooltip,dblClickCB,keyUpCB,showMenuLabel,menuLabel,convBlanks,beforeShowCB)
//	o.getMenu=IconMenuWidget_getMenu
//	o.add = ButtonScrollMenuWidget_add
//	return o;
//}
// 
//
//// ================================================================================
//
//function ButtonScrollMenuWidget_clickCB()
//// PRIVATE
//{
//	var o=this,l=o.layer;
//	o.menu.show(!o.menu.isShown(),getPosScrolled(l).x,getPosScrolled(l).y+o.getHeight(),null,null,o)
//}
// 
//// ================================================================================
//
//function ButtonScrollMenuWidget_add(s,val,sel,id)
//{
//	this.menu.add(s,val,sel,id)
//}

// ================================================================================
// ================================================================================
//
// OBJECT newBorderMenuItem (Constructor)
//
// creates a border widget item
//
// ================================================================================
// ================================================================================
//
//function newBorderMenuItem(par,idx,cb,isLabel,label)
//// par     [MenuWidget] the parent menu
//// Returns [BorderMenuItem] the new instance
//{
//	var o=newMenuItem(par,"border_"+idx,null,cb)
//
//	o.idx=idx
//	o.isLabel=isLabel?isLabel:false
//	o.label=label?label:null
//	
//	// Public methods
//	o.attachSubMenu=null
//	o.getHTML=BorderMenuItem_getHTML
//	o.check=BorderMenuItem_check
//	o.menuItemType=_isNotColor
//
//	return o
//}
//
//// ================================================================================
//
//function BorderMenuItem_check(check)
//// Check a color menu item
//// check  [boolean] checks the item if true
//// Return [void]
//{
//	var o=this
//	
//	if (o.checked!=check)
//	{
//		o.checked=check
//	
//		if (o.layer)
//			BorderMenuItem_invert(o.layer,o.checked?1:0)
//	}
//}
//
//// ================================================================================
//
//function BorderMenuItem_getHTML()
//{
//	var o=this,s="",d=_moz?10:12,lenTotal=o.par.items.length,index=o.menuIndex - 1;col=index%8
//
//	var cbs=' onclick="'+_codeWinName+'.MenuBordersWidget_onclickCB(this,event);return true" oncontextmenu="'+_codeWinName+'.MenuBordersWidget_onclickCB(this,event);return false" onmousedown="'+_codeWinName+'._minb(event)" onmouseup="'+_codeWinName+'._minb(event)" onmouseover="'+_codeWinName+'.MenuBordersWidget_onmouseOverCB(this)" '
//
//	var cspan=(o.isLabel?' colspan="4"':'')
//	var cls="menuiconborders"+(o.checked?"Sel":"")
//	
//	s+='<td '+cspan+' id="'+(o.par.id+'_item_'+o.id)+'" '+cbs+' align="center"><div class="'+cls+'">'
//	s+=o.isLabel?convStr(o.label):simpleImgOffset(_skin+'../borders.gif',16,16,16*o.idx,0,'IconImg_'+o.id,null,_bordersTooltip[o.idx],'margin:2px;cursor:default')
//	s+='</div></td>'
//	
//	return s
//}
//
//// ================================================================================
//
//function BorderMenuItem_invert(lyr,inv)
//// PRIVATE - DHTML Callback
//// lyr    [DOM Element] the element
//// inv    [0..1] 1 to invert
//// Return [void]
//{
//	var o=_menusItems[lyr._boIndex]
//	if (o && o.checked)
//		inv=1
//
//	lyr.childNodes[0].className="menuiconborders"+(inv?"Sel":"")
//}

// ================================================================================
// ================================================================================
//
// OBJECT newMenuBordersWidget (Constructor)
//
// creates borders menu widget
//
// ================================================================================
// ================================================================================
//
//function newMenuBordersWidget(id,hideCB,beforeShowCB,clickCB)
//// id      [String] the menu id for DHTML processing
//// hideCB  [Function - Optional] callback called when the menu is closed
//// Returns [MenuBordersWidget] the new instance
//{
//	var o=newMenuWidget(id,hideCB,beforeShowCB)
//	
//	// properties
//	o.items=new Array
//	for (var i=0; i < 12; i++)
//		o.items[i]=newBorderMenuItem(o,i,clickCB)
//	
//	var len=o.items.length
//	o.items[len]=newBorderMenuItem(o,12,clickCB,true,_bordersMoreColorsLabel)
//	o.clickCB=clickCB
//	
//	// methods
//	o.getHTML=MenuBordersWidget_getHTML
//	o.hasVisibleItem=MenuBordersWidget_hasVisibleItem
//	o.uncheckAll=MenuBordersWidget_uncheckAll
//				
//	return o
//}
//
//// ================================================================================
//
//function MenuBordersWidget_getHTML()
//// PRIVATE get the HTML
//// Return [String]
//{
//	var o=this,items=o.items
//	var keysCbs=' onkeydown="'+_codeWinName+'.MenuWidget_keyDown(\''+o.id+'\',event);return true" '
//
//	// we'll add the iframe with the show function
//	var s='<a style="position:absolute;left:-30px;top:-30px; visibility:hidden" id="startLink_'+o.id+'" href="javascript:void(0)" onfocus="'+_codeWinName+'.MenuWidget_keepFocus(\''+o.id+'\');return false;" ></a>'
//	s+='<table style="display:none;" class="menuFrame" id="'+o.id+'" cellspacing="0" cellpadding="0" border="0" '+keysCbs+'><tbody>'
//	
//	s+='<tr>'
//	for (var i=0; i<=3; i++)
//		s+=items[i].getHTML()
//	s+='</tr>'
//	s+='<tr>'
//	for (var i=4; i<=7; i++)
//		s+=items[i].getHTML()
//	s+='</tr>'
//	s+='<tr>'
//	for (var i=8; i<=11; i++)
//		s+=items[i].getHTML()
//	s+='</tr>'
//	
//	s+='<tr>'+items[12].getHTML()+'</tr>'
//	
//	s+='</tbody></table><a style="position:absolute;left:-30px;top:-30px; visibility:hidden" id="endLink_'+o.id+'" href="javascript:void(0)" onfocus="'+_codeWinName+'.MenuWidget_keepFocus(\''+o.id+'\');return false;" ></a>'
//
//	return s
//}
//
//// ================================================================================
//
//function MenuBordersWidget_hasVisibleItem()
//{
//	return true
//}
//
//// ================================================================================
//
//function MenuBordersWidget_uncheckAll()
//// Uncheck all items
//// Return [void]
//{
//	var o=this,items=o.items
//
//	for (var i in items)
//	{
//		var item=items[i]
//		if (item.checked)
//			item.check(false)
//	}	
//}
//
//// ================================================================================
//
//function MenuBordersWidget_onclickCB(lyr,e)
//// PRIVATE - DHTML Callback
//// lyr    [DOM Element] the element
//// e      [Event] DHTML Event
//// Return [void]
//{
//	eventCancelBubble(e)
//	var idx=lyr._boIndex,o=_menusItems[idx]
//	o.par.uncheckAll()
//
//	BorderMenuItem_invert(lyr,1,idx)
//	o.checked=true
//
//	o.par.show(false,0,0,true)
//
//	if (o.cb)
//		setTimeout("MenuItem_delayedClick("+idx+")",1)	
//}
//
//// ================================================================================
//
//function MenuBordersWidget_out()
//// PRIVATE
//{
//	BorderMenuItem_invert(this,0);
//}
//
//// ================================================================================
//
//function MenuBordersWidget_onmouseOverCB(l)
//{
//	l.onmouseout=MenuBordersWidget_out
//	BorderMenuItem_invert(l,1)
//}


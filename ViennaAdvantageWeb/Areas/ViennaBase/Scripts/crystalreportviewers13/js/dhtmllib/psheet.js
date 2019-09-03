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

File: tabs.js
=============================================================
*/

if (window._DHTML_LIB_PSHEET_JS_LOADED==null)
{
    _DHTML_LIB_PSHEET_JS_LOADED=true
    _boAllTabs=new Array
    
    //
    _vertTabImgW=3;
    
    _vertTabLBorderToTxt=20-_vertTabImgW-1;
    _vertTabLBorderToIcon=12-_vertTabImgW-1;
    _vertTabIconToTxt=8;
    _vertTabIconSize=16;    //icon:16px x 16px
    
    //used in _horizTabCSS & _horizBottomTabCSS
    _tabImgLeft=0;
    _tabImgMid=1;
    _tabImgRight=2;
    _tabTxt=3;
    _tabScrollBar=4;
    _tabList=5;
    
    _VertTab=2;                 //vertical navigation panel without icon
    _VertTabWithIcon=3;         //vertical navigation panel with icon
    _HorizTabTop=4;             //horizontal navigation bar on top 
    _HorizTabBottom=5;          //horizontal navigation bar at bottom
    _HorizTabTopWithClose=6;
    _menuBarTab=7;              //menu bar 
    
    //menu.png: 4 menu states for vertical navigation panel
    _vertTabHover=0;
    _vertTabSelected=1;
    _vertTabPressed=2;
    _vertTabNormal=3;
    
    _vertTabCSS=[   ['menuLeftMostSel', 'naviVTabLSelected', 'naviVTabLPressed', 'naviVTabNormal'], //left
                    ['menuLeftPartSel', 'naviVTabMSelected', 'naviVTabMPressed', 'naviVTabNormal'], //middle
                    ['menuRightMostSel', 'naviVTabRSelected', 'naviVTabRPressed', 'naviVTabNormal'] //right
                ]
    
    _vertNaviPanelMinW=120
    _vertNaviPanelMaxW=180
    _vertNaviPanelWithIconW=150
    
    _vertNaviPanelH=23*8    //8 links x 23px
    
    _naviHorzTabH=24;
    
    //used in _horizTabCSS & _horizBottomTabCSS
    _horizTabSelected=0;
    _horizTabNormal=1;
    _horizTabHover=2;   //hover on no selected
    _horizTabSelHover=3;//hover on selected
    _horizTabPressed=4;
    

    
    //tab list icon state in horizontal bar used in _horizTabCSS & _horizBottomTabCSS
    _tabListNormal=0;
    _tabListHover=1;
    _tabListPressed=2;
    
    //horizontal bar at top
    _horizTabCSS=[  ['naviHTabLSelected', 'naviHTabLNormal', 'naviHTabLHover', 'naviHTabLSelHover', 'naviHTabLNormal'], //left
                    ['naviHTabMSelected', 'naviHTabMNormal', 'naviHTabMHover', 'naviHTabMSelHover', 'naviHTabMNormal'], //middle
                    ['naviHTabRSelected', 'naviHTabRNormal', 'naviHTabRHover', 'naviHTabRSelHover', 'naviHTabRNormal'], //right
                    ['naviHTabTextSel',   'naviHTabText',    'naviHTabTextHover','naviHTabTextSelHover', 'naviHTabText'],//text
                    //scroll bar
                    ['naviHScrollBarL', 'naviHScrollBarM', 'naviHScrollBarR', 
                    'naviHScrlBarFirstArrow naviHScrlBarArrowPos', 'naviHScrlBarPrevArrow naviHScrlBarArrowPos',
                    'naviHScrlBarNextArrow naviHScrlBarArrowPos', 'naviHScrlBarLastArrow naviHScrlBarArrowPos',
                    'naviHScrlBarHover', 'naviHScrlBarPressed',
                    //scroll bar disabled
                    'naviHScrlBarDisabled',
                    'naviHScrlBarFirstDis naviHScrlBarArrowPos', 'naviHScrlBarPrevDis naviHScrlBarArrowPos',
                    'naviHScrlBarNextDis naviHScrlBarArrowPos', 'naviHScrlBarLastDis naviHScrlBarArrowPos'
                    ],
                    //tab list
                    ['tabListTop','tabListTopHover','tabListTopPressed']
                 ]
                 
    //horizontal bar at top with close icon
    _horizTabWithCloseCSS=[ 
                    ['naviHTabLSelected', 'naviHTabLNormal', 'naviHTabLHover', 'naviHTabLSelHover', 'naviHTabLNormal'], //left
                    ['naviHTabMSelected', 'naviHTabMNormal', 'naviHTabMHover', 'naviHTabMSelHover', 'naviHTabMNormal'], //middle
                    ['naviHTabWithCloseRSel', 'naviHTabRWithCloseNormal', 'naviHTabWithCloseRHover', 'naviHTabWithCloseRSelHover', 'naviHTabRWithCloseNormal'], //right
                    ['naviHTabTextSel',   'naviHTabText',    'naviHTabTextHover','naviHTabTextSelHover', 'naviHTabText'],//text
                    //scroll bar
                    ['naviHScrollBarL', 'naviHScrollBarM', 'naviHScrollBarR', 
                    'naviHScrlBarFirstArrow naviHScrlBarArrowPos', 'naviHScrlBarPrevArrow naviHScrlBarArrowPos',
                    'naviHScrlBarNextArrow naviHScrlBarArrowPos', 'naviHScrlBarLastArrow naviHScrlBarArrowPos',
                    'naviHScrlBarHover', 'naviHScrlBarPressed',
                    //scroll bar disabled
                    'naviHScrlBarDisabled',
                    'naviHScrlBarFirstDis naviHScrlBarArrowPos', 'naviHScrlBarPrevDis naviHScrlBarArrowPos',
                    'naviHScrlBarNextDis naviHScrlBarArrowPos', 'naviHScrlBarLastDis naviHScrlBarArrowPos'
                    ]
                 ]
    
    _horizTabImgL=4;
    _horizTabImgR=23;
    
    _horizTabTxtPaddingL=15;
    _horizTabTxtPaddingR=3;
    _horizTabTxtPaddingB=5;
    
    _horizTabImgPadL=8;
    _horizTabImgToTxt=6;
    
    _horizBottomTabTxtPadB=7;
    
    //for close icon
    _horizTabTxtToClose=5;
    _horizTabClosePadR=1;
    
    //scroll bar in horizontal navigation bar
    _noScrollBar=0;
    _ScrollBarAtBegin=1;
    _ScrollBarAtEnd=2;
                       
    
    _horizBarWidth=100  //not include scroll bar
    
    //menu bar's height is the same as that of the tool bar
    _defaultMenuBarWidth=200;
    _menuBarTabHeight=22;
    

    //tab list icon for horizontal bar
    _tabListIconWidth=19;
}
//
//// ================================================================================
//// ================================================================================
////
//// OBJECT newTabWidget (Constructor)
////
//// Display a sing tab. It has a selected and an unselected
//// State.
////
//// ================================================================================
//// ================================================================================
//
//function newTabWidget(id,isTop,name,cb,value,icon,iconW,iconH,iconOffX,iconOffY,dblclick,alt)
//// id       : [String]   the tab id for DHTML processing
//// isTop    : [boolean]  change the tab look. true for top tabs
//// name     : [String]   tab label
//// cb       : [Function] callback pointer, called when clicking on the tab
//// value    : [String - optional] a value that is used to find it again
//// icon     : [String - optional] an image URL
//// iconW    : [int - optional] displayed image width
//// iconH    : [int - optional] displayed image height
//// iconOffX : [int - optional] x offset in the icon (for combined images)
//// iconOffY : [int - optional] y offset in the icon (for combined images)
//// Return   : The new object
//{
//    // Parent class
//    var o=newWidget(id)
//
//    // Members
//    o.isTop=isTop
//    o.isSelected=false
//    
//    // Other layers
//    o.lnkLayer=null
//    o.leftImgLayer=null
//    o.rightImgLayer=null
//    o.midImgLayer=null
//    o.imgImgLayer=null
//    o.iconLayer=null
//    o.tabBar=null
//
//    // Methods
//    o.getHTML=TabWidget_getHTML
//    o.select=TabWidget_select
//    o.change=TabWidget_change
//    o.changeContent=TabWidget_changeContent
//    o.zoneId = 'tzone_tab_'+ Math.round(Math.random() * 12345) + new Date().getTime(); /* returns a unique number */
//    
//    // Set content
//    o.change(name,cb,value,icon,iconW,iconH,iconOffX,iconOffY,dblclick,alt)
//    
//    // For further retrieval
//    _boAllTabs[id]=o
//    return o
//}
//
//// ================================================================================
//
//function TabWidget_getHTML()
//// Write the tab widget HTML
//// return [String] the HTML
//{
//    var o=this
//    var y=o.isSelected?0:72
//    if (!o.isTop)
//        y+=144
//    
//    var cls="thumbtxt"+(o.isSelected?"sel":"")
//    var cb=_codeWinName+".TabWidget_clickCB('"+o.id+"');return false"
//    var dblcb=_codeWinName+".TabWidget_dblclickCB('"+o.id+"');return false"
//    var keycb=_codeWinName+".TabWidget_keyDownCB('"+o.id+"',event);"
//    var menu=_codeWinName+".TabWidget_contextMenuCB('"+o.id+"',event);return false"
//    var icon=o.icon?o.icon:_skin+"../transp.gif"
//    var iconTDWidth=o.icon?3:0
//    
//    return '<table onmouseover="return true" onclick="'+cb+'" id="'+this.id+'" ondblclick="'+dblcb+'" onkeydown="'+keycb+'" oncontextmenu="'+menu+'" id="'+this.id+'" style="cursor:'+_hand+'" cellspacing="0" cellpadding="0" border="0"><tbody><tr valign="middle" height="24">'+
//        '<td width="15">'+imgOffset(_skin+'tabs.gif',15,24,0,y,"tabWidgetLeft_"+o.id)+'</td>'+
//        '<td id="tabWidgetImg_'+o.id+'" style="' +(o.isTop?'padding-top:2px;':'padding-bottom:2px;')+ ' padding-right:'+iconTDWidth+'px; '+backImgOffset(_skin+"tabs.gif",0,y+24)+'" width="'+(o.iconW+iconTDWidth)+'" align="left">'+imgOffset(icon,o.iconW,o.iconH,o.iconOffX,o.iconOffY,"tabWidgetIcon_"+o.id,null,o.iconAlt)+'</td>'+
//        '<td  width="50" id="tabWidgetMid_'+o.id+'" style="' +(o.isTop?'padding-top:3px;':'padding-bottom:3px;') +backImgOffset(_skin+"tabs.gif",0,y+24)+'"><span style="white-space:nowrap">'+lnk(convStr(o.name,true),null,cls,"tabWidgetLnk_"+o.id)+'</span></td>'+
//        '<td width="15">'+imgOffset(_skin+'tabs.gif',15,24,0,y+48,"tabWidgetRight_"+o.id)+'</td>'+
//    '</tr></tbody></table>'
//}
//
//// ================================================================================
//
//function TabWidget_clickCB(id)
//// Global function, internal click event handler for the Tab widget. It calls a 
//// id [String] the tab id
//// delayed function for fixing some browers bugs
//// return void
//{
//    setTimeout("TabWidget_delayedClickCB('"+id+"')",1)
//}
//
//// ================================================================================
//
//function TabWidget_dblclickCB(id)
//// id [String] the tab id
//// return void
//{
//    setTimeout("TabWidget_delayedDblClickCB('"+id+"')",1)
//}
//
//// ================================================================================
//
//
//function TabWidget_keyDownCB(id,e)
//// Global function, internal onkeydown event handler for the Tab widget. It calls a 
//// id [String] the tab id and event
//{
//    var k=eventGetKey(e);   
//    //be careful ! usefull for dialog box close by Enter ou Escape keypressed
//    if(eventGetKey(e) == 13)//Enter
//    {
//        eventCancelBubble(e);
//    }
//}
//// ================================================================================
//
//
//function TabWidget_contextMenuCB(id,e)
//// id [String] the tab id
//// e  [event] the event
//// return void
//{       
//    if (_ie)
//        e=_curWin.event
//                
//    var tab=_boAllTabs[id], tabbar = tab.tabBar
//    
//    if ((tab)&&(tab.cb))
//        tab.cb()
//        
//    if ((tabbar)&& (tabbar.showMenu))   
//        tabbar.showMenu(e)  
//}
//
//// ================================================================================
//
//function TabWidget_delayedClickCB(id)
//// Global function, internal click event handler for the Tab widget. Calls the
//// user defined callback of the TabWidget, if any
//// id [String] the tab id
//// return void
//{
//    var tab=_boAllTabs[id]
//    if ((tab)&&(tab.cb))
//        tab.cb()
//}
//
//// ================================================================================
//
//function TabWidget_delayedDblClickCB(id)
//// Global function, Delayed internal click event handler for the Tab widget. Calls the
//// user defined callback of the TabWidget, if any
//// id [String] the tab id
//// return void
//{
//    var tab=_boAllTabs[id]
//    
//    if ((tab)&& (tab.dblclick))
//        tab.dblclick()
//}
//
//// ================================================================================
//
//function TabWidget_changeContent(changeOnlySelection)
//// Select or unselects the tab widget
//// sel [boolean] : select if true
//// return void
//{
//    var o=this
//
//    // Get layers
//    if (o.lnkLayer==null)
//    {
//        o.lnkLayer=getLayer("tabWidgetLnk_"+o.id)
//        o.leftImgLayer=getLayer("tabWidgetLeft_"+o.id)
//        o.rightImgLayer=getLayer("tabWidgetRight_"+o.id)
//        o.midImgLayer=getLayer("tabWidgetMid_"+o.id)
//        o.imgImgLayer=getLayer("tabWidgetImg_"+o.id)
//        o.iconLayer=getLayer("tabWidgetIcon_"+o.id)
//    }
//
//    // Change icon and text
//    if (!changeOnlySelection)
//    {
//        o.lnkLayer.innerHTML=convStr(o.name,true)
//        changeOffset(o.iconLayer,o.iconOffX,o.iconOffY,o.icon?o.icon:_skin+"../transp.gif")
//        o.iconLayer.alt=o.iconAlt
//
//        //var parSty=o.iconLayer.parentNode.style
//        o.iconLayer.style.width=""+o.iconW+"px"
//        o.iconLayer.style.height=""+o.iconH+"px"
//        
//        var iconTDWidth=o.icon?3:0,imgL=o.imgImgLayer
//        imgL.style.paddingRight=""+iconTDWidth+"px"
//
//        imgL.style.width=""+(iconTDWidth+(((o.icon!=null)&&(o.iconW!=null))?o.iconW:0))+"px"
//        if (_moz&&!_saf)
//            imgL.width=(iconTDWidth+(((o.icon!=null)&&(o.iconW!=null))?o.iconW:0))
//    }
//
//    // Calculate positions
//    var y=o.isSelected?0:72
//    if (!o.isTop)
//        y+=144
//
//    changeOffset(o.leftImgLayer,0,y)
//    changeOffset(o.midImgLayer,0,y+24)
//    changeOffset(o.imgImgLayer,0,y+24)
//    changeOffset(o.rightImgLayer,0,y+48)
//
//    o.lnkLayer.className="thumbtxt"+(o.isSelected?"sel":"")
//}
//
//// ================================================================================
//
//function TabWidget_select(sel)
//// Select or unselects the tab widget
//// sel [boolean] : select if true
//// return : void
//{
//    var o=this
//    o.isSelected=sel
//    if (o.layer!=null)
//        o.changeContent(true)
//}
//
//// ================================================================================
//
//function TabWidget_change(name,cb,value,icon,iconW,iconH,iconOffX,iconOffY,dblclick,alt)
//// Change the tabs parameters (name, callbacks, icon)
//// name     : [String]   tab label
//// cb       : [Function] callback pointer, called when clicking on the tab
//// value    : [String - optional] a value that is used to find it again
//// icon     : [String - optional] an image URL
//// iconW    : [int - optional] displayed image width
//// iconH    : [int - optional] displayed image height
//// iconOffX : [int - optional] x offset in the icon (for combined images)
//// iconOffY : [int - optional] y offset in the icon (for combined images)
//// return   : void
//{
//    var o=this
//
//    // Set new values
//    if (name!=null)
//        o.name=name
//    if (cb!=null)
//        o.cb=cb
//    if (dblclick!=null)
//        o.dblclick=dblclick
//    if (value!=null)    
//        o.value=value
//    if (icon!=null)
//        o.icon=icon
//    o.iconW=iconW?iconW:0
//    o.iconH=iconH?iconH:0
//    o.iconOffX=iconOffX?iconOffX:0
//    o.iconOffY=iconOffY?iconOffY:0
//    if (alt!=null)
//        o.iconAlt=alt
//    
//    // If the widget is initialized, update it
//    if (o.layer!=null)
//        o.changeContent(false)
//}
//
//// ================================================================================
//// ================================================================================
////
//// OBJECT newTabBarWidget (Constructor)
////
//// Display a tabs bar. When using icons, these must be all in the same combined
//// image
////
//// ================================================================================
//// ================================================================================
//
//function newTabBarWidget(id,isTop,cb,st,dblclick,beforeShowMenu,showIcn)
//// id       : [String]   the tab id for DHTML processing
//// isTop    : [boolean]  change the tab look. true for top tabs
//// cb       : [Function - optional] callback pointer, called when clicking on a tab
//// st       : [String - optional] additional style in the tag
//// showIcn  : [boolean]  show the navigation icon (default is false)
//// Return   : The new object
//{
//    // Parent class
//    var o=newWidget(id)
//    var t
//
//    // Members
//    o.isTop=isTop
//    o.cb=cb
//    o.dblclick=dblclick
//    o.menu=newMenuWidget("menu_"+id,null,beforeShowMenu)    
//    o.st=st
//    o.counter=0
//    o.items=new Array
//    o.selIndex=-1
//    o.leftLimit=0
//    o.trLayer=null
//    o.showIcn=showIcn==null?false:showIcn
//    t=o.firstIcn=newIconWidget("firstIcn_"+id,_skin+"scroll_icon.gif",TabBarWidget_firstCB,null,_scroll_first_tab,5,8,0,0,0,8)
//    t.par=o
//    t.margin=0
//    t.allowDblClick=true
//    t=o.previousIcn=newIconWidget("previousIcn_"+id,_skin+"scroll_icon.gif",TabBarWidget_prevCB,null,_scroll_previous_tab,5,8,7,0,7,8)
//    t.par=o
//    t.margin=0
//    t.allowDblClick=true
//    t=o.nextIcn=newIconWidget("nextIcn_"+id,_skin+"scroll_icon.gif",TabBarWidget_nextCB,null,_scroll_next_tab,5,8,13,0,13,8)
//    t.par=o
//    t.margin=0
//    t.allowDblClick=true
//    t=o.lastIcn=newIconWidget("lastIcn_"+id,_skin+"scroll_icon.gif",TabBarWidget_lastCB,null,_scroll_last_tab,5,8,21,0,21,8)
//    t.par=o
//    t.margin=0
//    t.allowDblClick=true
//    o.showContextMenuAllowed=true
//    
//    // Methods
//    o.oldInit=o.init
//    o.init=TabBarWidget_init
//    o.getHTML=TabBarWidget_getHTML
//    o.add=TabBarWidget_add
//    o.remove=TabBarWidget_remove
//    o.removeAll=TabBarWidget_removeAll
//    o.select=TabBarWidget_select    
//    o.getSelection=TabBarWidget_getSelection    
//    o.getMenu=TabBarWidget_getMenu
//    o.showMenu=TabBarWidget_showMenu
//    o.showTab=TabBarWidget_showTab
//    o.getCount=TabBarWidget_getCount
//    o.oldResize=o.resize
//    o.resize=TabBarWidget_resize
//    o.getItemXPos=TabBarWidget_getItemXPos
//    o.scroll=TabBarWidget_scroll
//    o.setIconState=TabBarWidget_setIconState
//    o.setShowContextMenuAllowed=TabBarWidget_setShowContextMenuAllowed
//    
//    return o
//}
//
//// ================================================================================
//
//function TabBarWidget_init()
//// init the widget
//// return void
//{
//    var o=this,items=o.items
//    o.oldInit()
//    
//    if (o.showIcn)
//    {
//        o.firstIcn.init()
//        o.previousIcn.init()
//        o.nextIcn.init()
//        o.lastIcn.init()
//    }
//    
//    o.trLayer=getLayer("tr_"+o.id)  
//    o.tabsLayer=getLayer("tabs_"+o.id)
//    
//    var len = items.length
//    for (var i=0;i<len;i++)
//    {
//        var it=items[i]
//        it.init()
//        it.select(i==o.selIndex)
//    }   
//}
//
//// ================================================================================
//
//function TabBarWidget_getSelection()
//// get the selection
//// return an object (or null if no selection). Object fields
////    - index [int] : the selection index
////    - value [String] : value of the select object
//{
//    var o=this,index=o.selIndex
//    if (index>=0)
//    {
//        var obj=new Object
//        obj.index=index
//        obj.valueOf=o.items[index].value
//        obj.name=o.items[index].name
//        return obj
//    }
//    else
//        return null
//}
//
//// ================================================================================
//
//function TabBarWidget_getHTML()
//// Write the tab bar widget HTML
//// return [String] the HTML
//{
//    var o=this,items=o.items,len=items.length
//    var s= '<div id="'+this.id+'" style="height:24px;overflow:hidden;'+(o.st?o.st:'')+'">'
//    s+='<table cellspacing="0" cellpadding="0" border="0"><tbody><tr valign="top" height="24">'
//    
//    if (o.showIcn)
//    {
//        s+='<td><table class="palette" cellspacing="0" cellpadding="0" border="0"><tbody><tr>'
//        s+='<td>'+o.firstIcn.getHTML()+'</td>'  
//        s+='<td>'+o.previousIcn.getHTML()+'</td>'
//        s+='<td>'+o.nextIcn.getHTML()+'</td>'
//        s+='<td>'+o.lastIcn.getHTML()+'</td>'
//        s+='</tr></tbody></table></td>'
//    }
//    
//    s+='<td><div style="overflow:'+(true?'hidden':'scroll')+'" id="tabs_'+this.id +'"><table cellspacing="0" cellpadding="0" border="0"><tbody><tr id="tr_'+this.id +'">'   
//    for (var i=0;i<len;i++) 
//        s+='<td>'+items[i].getHTML()+'</td>'
//    s+='</tr></tbody></table></div></td>'
//    
//    s+='</tr></tbody></table></div>'
//    
//    return s    
//}
//
//// ================================================================================
//
//function TabBarWidget_select(index)
//// Select an elemnt by index
//// index : [int] index to select (-1 = last)
//// return : void
//{
//    var o=this,items=o.items,len=items.length
//    if (index==-1) index=len-1
//    
//    if ((index>=0)&&(index<len))
//    {
//        if ((o.selIndex>=0)&&(o.selIndex!=index)&&(o.selIndex<len))
//            items[o.selIndex].select(false)
//    
//        o.selIndex=index
//        items[index].select(true)
//        
//        o.scroll(null,o.selIndex)
//    }
//}
//
//// ================================================================================
//
//function TabBarWidget_resize(w,h)
//// Resizes the TabBarWidget
//// w    [int]   The new tabBarWidget width
//// h    [int]   The new tabBarWidget height
//{
//    var o=this
//    var d=isHidden(o.layer)
//
//    if (d&_moz&&!_saf)
//        o.setDisplay(false)
//        
//    o.oldResize(w,h)
//    if (w!=null)
//        o.tabsLayer.style.width=""+Math.max(0,w-54)
//
//    if (d&_moz&&!_saf)
//        o.setDisplay(true)
//        
//    o.setIconState()
//}
//
//// ================================================================================
//
//function TabBarWidget_showTab(index,show)
//{
//    var o=this,items=o.items,len=items.length
//    if ((index>=0)||(index<len))
//        items[index].setDisplay(show)
//}
//
//// ================================================================================
//
//function TabBarWidget_add(name, value, idx, icon, iconW,iconH,iconOffX,iconOffY,alt)
//// Add an element in the tab bar
//// name     : [String]   tab label
//// value    : [String - optional] a value that is used to find it again
//// icon     : [String - optional] an image URL
//// iconW    : [int - optional] displayed image width
//// iconH    : [int - optional] displayed image height
//// iconOffX : [int - optional] x offset in the icon (for combined images)
//// iconOffY : [int - optional] y offset in the icon (for combined images)
//// Returns  : the new TabWidget object
//{
//    var o=this,counter=o.counter++
//    var obj=newTabWidget(o.id+"_tab"+counter,o.isTop,name,TabBarWidget_itemClick,value,icon,iconW,iconH,iconOffX,iconOffY,TabBarWidget_itemDblClick,alt)
//    
//    obj.tabBar=o
//    obj.idx=counter
//    arrayAdd(o,"items",obj,idx)     
//    
//    var l=o.trLayer 
//    if(l!=null)
//    {       
//        var node=document.createElement("td")
//        node.innerHTML=obj.getHTML()
//        l.appendChild(node)
//        
//        obj.init()
//    }
//
//    return obj
//}
//
//// ================================================================================
//
//function TabBarWidget_remove(idx)
//{
//    var o=this,items=o.items,len=items.length
//
//    if ((idx>=0)&&(idx<len))
//    {
//        var elem=items[idx]
//        var l=elem.layer
//
//        arrayRemove(o,"items",idx)
//        items=o.items
//        len=items.length
//
//        if (l!=null)
//        {
//            l=l.parentNode
//            l.parentNode.removeChild(l)
//        }
//
//        if (o.selIndex>idx) o.select(o.selIndex-1)
//        else if ((o.selIndex==idx) && (len>0))
//            o.select(Math.min(idx,len-1))
//    }
//}
//
//// ================================================================================
//function TabBarWidget_removeAll()
//{
//    var o=this,items=o.items, len= items.length 
//    for (var i=len-1;i>=0;i--)
//        o.remove(i)         
//}
//
//// ================================================================================
//
//function TabBarWidget_itemClick()
//// Internal callback when an item is selected
//// Returns  void
//{
//    var o=this.tabBar,items=o.items,len=items.length,index=-1
//    for (var i=0;i<len;i++)
//    {
//        if (items[i].idx==this.idx)
//        {
//            o.select(i)
//            index=i
//            break
//        }
//    }
//    
//    if (o.cb)
//        o.cb(index)
//}
//
//// ================================================================================
//
//function TabBarWidget_itemDblClick()
//// Internal callback when a double click is done
//// Returns  void
//{
//    
//    var o=this.tabBar,items=o.items,len=items.length,index=-1
//    for (var i=0;i<len;i++)
//    {
//        if (items[i].idx==this.idx)
//        {                   
//            index=i
//            break
//        }
//    }
//    
//    if (o.dblclick)
//        o.dblclick(index)
//}
//
//// ================================================================================
//
//function TabBarWidget_getMenu()
//// get the menu
//// return [MenuWidget] the menu
//{
//    return this.menu
//}
//
//// ================================================================================
//
//function TabBarWidget_setShowContextMenuAllowed(b)
//{
//    this.showContextMenuAllowed=b
//}
//
//// ================================================================================
//
//function TabBarWidget_showMenu(e)
//// Show the menu
//// e    : event
//// return void
//{
//    if (this.showContextMenuAllowed==false)
//        return
//        
//    if (_ie)
//        e=event
//
//    this.menu.show(true,eventGetX(e),eventGetY(e))  
//}
//
//// ================================================================================
//
//function TabBarWidget_getCount()
//// Get the number of tabs
//// return [int] the number of tabs
//{
//    return this.items.length
//}
//
//// ================================================================================
//
//function TabBarWidget_scroll(step,destItem)
//// Scroll the TabBarWidget in order for the <destIndex> tab to be visible
//{
//    var o=this
//
//    if (o.tabsLayer==null)
//        return
//            
//    var tabsl=o.tabsLayer
//    var tabsSL=tabsl.scrollLeft,tabsOW=tabsl.offsetWidth,tabsSW=tabsl.scrollWidth,SLMax=tabsSW-tabsOW
//
//    //alert("AVANT scroll\nstep="+step+"\no.leftLimit="+o.leftLimit+"\ntabsSL="+tabsSL+"\nSLMax="+SLMax)
//    
//    // Scroll
//    if (step=='first')      // go to first tab
//    {
//        tabsl.scrollLeft=tabsSL=0
//        o.leftLimit=0
//    }
//    else
//    if (step=='previous')   // go to previous tab
//    {   
//        o.leftLimit=o.leftLimit-1   
//        var x=o.getItemXPos(o.leftLimit)
//        tabsl.scrollLeft=tabsSL=x       
//    }
//    else
//    if (step=='next')       // go to next tab
//    {
//        if (o.leftLimit>o.getCount()-1)
//            return
//        o.leftLimit+=1
//        var x=o.getItemXPos(o.leftLimit)
//        if (x<SLMax)
//            tabsl.scrollLeft=tabsSL=x
//        else
//            tabsl.scrollLeft=tabsSL=SLMax
//    }
//    else
//    if (step=='last')       // go to last tab
//    {       
//        for (var i=0;i<o.getCount();i++)
//        {
//            var x=o.getItemXPos(i);
//            if (x>SLMax)
//                break;
//        }
//        tabsl.scrollLeft=tabsSL=Math.max(0,SLMax)
//        o.leftLimit=i
//    }
//    else
//    if (step==null)         // go to the specified <destItem>
//    {
//        var x=o.getItemXPos(destItem);
//        if (x<SLMax)
//            tabsl.scrollLeft=tabsSL=x
//        else
//            tabsl.scrollLeft=tabsSL=SLMax
//        for (var i=0;i<o.getCount();i++)
//        {
//            var x=o.getItemXPos(i)
//            if (x>SLMax)
//                break;
//        }
//        o.leftLimit=i
//    }
//    
//    o.setIconState()
//
//    //alert("AVANT scroll\nstep="+step+"\no.leftLimit="+o.leftLimit+"\ntabsSL="+tabsSL+"\nSLMax="+SLMax)
//}
//
//// ================================================================================
//
//function TabBarWidget_getItemXPos(index)
//// Retrieve the x coordinates of a tab
//// index    [int]   The tab index
//// Returns  [int]   the x coordinates of a the tab
//{
//    var o=this
//    var x=0
//    for (var i=0;i<index;i++)
//        x+=parseInt(o.items[i].getWidth())
//
//    return x
//}
//
//// ================================================================================
//
//function TabBarWidget_setIconState()
//// Enable or disable the navigation icons first, previous, next, last
//{
//    var o=this
//
//    if (o.tabsLayer==null)
//        return
//            
//    var tabsl=o.tabsLayer
//    var tabsSL=tabsl.scrollLeft,tabsOW=tabsl.offsetWidth,tabsSW=tabsl.scrollWidth,SLMax=tabsSW-tabsOW
//
//    // Enable or disable the navigation icons if no scroll is possible on the
//    // right
//    if (tabsSL==SLMax)
//    {
//        o.nextIcn.setDisabled(true)
//        o.lastIcn.setDisabled(true)
//    }
//    else
//    {
//        o.nextIcn.setDisabled(false)
//        o.lastIcn.setDisabled(false)
//    }
//
//    // Enable or disable the navigation icons if no scroll is possible on the
//    // left
//    if (tabsSL == 0)
//    {
//        o.firstIcn.setDisabled(true)
//        o.previousIcn.setDisabled(true)
//    }
//    else
//    {
//        o.firstIcn.setDisabled(false)
//        o.previousIcn.setDisabled(false)
//    }
//
//}
//
//// ================================================================================
//
//function TabBarWidget_firstCB()
//// Callback function called when click on the "first" icon
//{   
//    var p=this.par
//    p.scroll('first')
//}
//
//// ================================================================================
//
//function TabBarWidget_prevCB()
//// Callback function called when click on the "previous" icon
//{
//    var p=this.par
//    p.scroll('previous')
//}
//
//// ================================================================================
//
//function TabBarWidget_nextCB()
//// Callback function called when click on the "next" icon
//{
//    var p=this.par
//    p.scroll('next')    
//}
//
//// ================================================================================
//
//function TabBarWidget_lastCB()
//// Callback function called when click on the "last" icon
//{           
//    var p=this.par
//    p.scroll('last')
//}
//
//// ================================================================================
//// ================================================================================
////
//// OBJECT newTabbedZone (Constructor)
////
//// Display a tabs bar. and a framed zone that display one content by tab
//// image
////
//// ================================================================================
//// ================================================================================

function newTabbedZone(id,tabs,cb,w,h)
// id [String]   the tab id for DHTML processing
// cb [Function - optional] callback pointer, called when clicking on a tab
// w  [int] width
// h  [int] height
// Return   : The new object
{
    var o=newFrameZoneWidget(id,w,h)
    o.w=w
    o.h=h
    o.cb=cb
    
    o.oldIndex=-1
    o.tzOldInit=o.init
    o.add=TabbedZoneWidget_add
    o.select=TabbedZoneWidget_select
    o.getTabCSS=TabbedZoneWidget_getTabCSS

    o.init=TabbedZoneWidget_init
    o.beginHTML=TabbedZoneWidget_beginHTML
    
    o.oldFrameZoneEndHTML=o.endHTML
    o.endHTML=TabbedZoneWidget_endHTML
    
    if(!tabs) {
        o.tabs=newTabBarWidget("tzone_tabs_"+id,true,TabbedZone_itemClick)
        o.tabs.parentTabbedZone=o
    }
    else {
        o.tabs = tabs;
    }
    o.beginTabHTML=TabbedZoneWidget_beginTabHTML
    o.endTabHTML=TabbedZoneWidget_endTabHTML
    o.beginTab=TabbedZoneWidget_beginTab
    o.endTab=TabbedZoneWidget_endTab
    
    o.showTab=TabbedZoneWidget_showTab
    o.tzOldResize=o.resize
    o.resize=TabbedZoneWidget_resize;

    return o
}

// ================================================================================

function TabbedZone_itemClick()
// Internal callback when an item is selected
// Return void
{
    var o=this.parentTabbedZone,i=this.getSelection().index
    o.select(i)
    if (o.cb)
        o.cb(i)
}

// ================================================================================

function TabbedZoneWidget_add(name, value, icon, iconW,iconH,iconOffX,iconOffY)
// Add an element in the tabbed zone
// name     : [String]   tab label
// value    : [String - optional] a value that is used to find it again
// icon     : [String - optional] an image URL
// iconW    : [int - optional] displayed image width
// iconH    : [int - optional] displayed image height
// iconOffX : [int - optional] x offset in the icon (for combined images)
// iconOffY : [int - optional] y offset in the icon (for combined images)
// return [TabWidget] : the new TabWidget object
{
    var o=this
    o.tabs.add(name, value, -1, icon, iconW,iconH,iconOffX,iconOffY)
}


// ================================================================================

function TabbedZoneWidget_init()
// init the widget
// return void
{
    var o=this
    o.tzOldInit()
    o.tabs.init()
    o.select(0)
}

// ================================================================================

function TabbedZoneWidget_getTabCSS(tab)
// get the tab style
// return [DOM style]
{
    if (tab != null) {
        if(!tab.zoneLayer) {
            tab.zoneLayer = getLayer(tab.zoneId);   
        }
        if(tab.zoneLayer)
            return tab.zoneLayer.style;         
    }
    
    return null
}


// ================================================================================

function TabbedZoneWidget_showTab(index,show)
// Show/hide a tab
// index [int] the tab index
// show boolean : show/hide
// return void
{
    var tab=this.tabs.items[index]
    if (tab)
        tab.setDisplay(show)
}
//================================================================================

function TabbedZoneWidget_resize(w, h)
//Resize the tab zone
//w [Int - optional] width
//h [Int - optional] height
//return void
{
    var o = this; 
    
    if (w != null) 
    {
        o.w = w;    
    }
    if (h != null)
    {
        o.h = h;
    }
    
    o.tzOldResize(w, h);
    
    var container = getLayer(o.id+'_container');
    if (container)
    {
        if(o.w)
            container.style.width = o.w + 'px';
        if(o.h)
            container.style.height = o.h + 'px';
    }
    var oldTab = o.tabs.items[o.oldIndex];
    if (oldTab)
    {
        var tab = getLayer(oldTab.zoneId);
        if (tab)
        {
            if(o.w)
                tab.style.width = o.w + 'px';
            if(o.h)
                tab.style.height = o.h + 'px';
        }
    }
}

// ================================================================================

function TabbedZoneWidget_select(index)
//selects a tab
//index [int] the tab index
//return void
{
    var o=this,tabs=o.tabs,sel=tabs.getSelection(),oldIndex=o.oldIndex,c
    
    var oldTab = tabs.items[o.oldIndex];
    o.tabs.select(index)
    if (oldTab)
    {
        c=o.getTabCSS(oldTab)
        if (c) c.display="none"
    }
    else
    {
        var len=tabs.items.length
        for (var i=0;i<len;i++)
        {
            c=o.getTabCSS(tabs.items[i])
            if (c) c.display="none"
        }
    }
    
    o.oldIndex=index
    
    c=o.getTabCSS(tabs.items[index])
    if (c)
    {   
        c.display=""
        o.resize(o.w, o.h);
    }
}





// ================================================================================

function TabbedZoneWidget_beginHTML()
// Write the beginng of the widget HTML
// return [String] the HTML
{
    var o=this
    return '<table id="'+this.id+'" cellspacing="0" cellpadding="0" border="0" style="position:absolute;"><tbody><tr class="hideableFrame" valign="bottom" height="28">'+
    '<td>'+imgOffset(_skin+'dialogframe.gif',5,5,0,0)+'</td>'+
    '<td valign="top" align="left" style="'+backImgOffset(_skin+"tabs.gif",0,288)+'">'+o.tabs.getHTML()+'</td>'+
    '<td>'+imgOffset(_skin+'dialogframe.gif',5,5,5,0)+'</td></tr>'+
    '<tr><td class="hideableFrame" style="'+backImgOffset(_skin+"dialogframeleftright.gif",0,0)+'"></td><td class="dialogzone"><div id="' + o.id + '_container' + '" style="'+sty("width",o.w)+sty("height",o.h)+'">'
}

function TabbedZoneWidget_endHTML()
{
    return '</div><td class="hideableFrame" style="'+backImgOffset(_skin+"dialogframeleftright.gif",5,0)+'"></td></tr>' +
                '<tr class="hideableFrame"><td>'+imgOffset(_skin+'dialogframe.gif',5,5,0,5)+'</td>' +
                '<td style="'+backImgOffset(_skin+"tabs.gif",0,187)+'"></td>' +
                '<td>'+imgOffset(_skin+'dialogframe.gif',5,5,5,5)+'</td></tr>'+
                '</tr></tbody></table>';
}

// ================================================================================

function TabbedZoneWidget_beginTabHTML(index)
// get the beginning of a tab zone
// index [int] the tab index
// return [String] the HTML
{
    var o=this
    return '<div id="' + o.zoneId  +'" style="display:none;'+sty("width",o.w)+sty("height",o.h)+'">'
}

// ================================================================================

function TabbedZoneWidget_endTabHTML()
// get the end of a tab zone
// return [String] the HTML
{
    return '</div>'
}

// ================================================================================

function TabbedZoneWidget_beginTab(index)
// write in the document the beginning of a tab zone
// return void
{
    _curDoc.write(this.beginTabHTML(index))
}

// ================================================================================

function TabbedZoneWidget_endTab()
// write in the document the end of a tab zone
// return void
{
    _curDoc.write(this.endTabHTML())
}

// ================================================================================
//
// vertical navigation panel & horizontal navigation bar for signature
//
// ================================================================================
function newNaviTabWidget(id, name, value, tabType, cb,dblClick, tooltip, icon,iconW,iconH,iconOffX,iconOffY, closeTabCB)
{
    return new_NaviTabWidget({  id:id,
                                name:name,
                                value:value,
                                tabType:tabType,
                                clickCB:cb,
                                dblclickCB:dblClick,
                                tooltip:tooltip,
                                icon:icon,
                                iconW:iconW,
                                iconH:iconH,
                                iconOffX:iconOffX,
                                iconOffY:iconOffY,
                                closeTabCB:closeTabCB           
                            });
}

function new_NaviTabWidget(prms)
//prms's content:
//
// id       : [String]   the tab id for DHTML processing
// name     : [String]   tab label
// value    : [String - optional] a value that is used to find it again
// tabType  : [enum] _VertTab, _VertTabWithIcon, _HorizTabTop, _HorizTabBottom
//
// cb       : [Function  - optional] callback pointer, called when clicking on the tab
// dblClick : [Function - optional] 
//
// icon     : [String - optional] an image URL
// iconW    : [int - optional] displayed image width
// iconH    : [int - optional] displayed image height
// iconOffX : [int - optional] x offset in the icon (for combined images)
// iconOffY : [int - optional] y offset in the icon (for combined images)
// closeTabCB:[function - optional]: only useful for _HorizTabTopWithClose
// Return   : The new object
{
    var o = new_Widget(prms);
    
    o.superInit=o.init;
    
    //public API
    o.init=NaviTabWidget_init;
    o.getHTML=NaviTabWidget_getHTML;
    o.hasCloseButton=NaviTabWidget_hasCloseButton;
    o.select=NaviTabWidget_select;
    
    o.setUserData=NaviTabWidget_setUserData;
    o.getUserData=NaviTabWidget_getUserData;
    
    o.setMenu=NaviTabWidget_setMenu;
    o.getMenu=NaviTabWidget_getMenu;
    
    o.setHtml=NaviTabWidget_setHtml;
    o.getHtml=NaviTabWidget_getHtml;
    
    o.zoneId = 'tzone_tab_'+ Math.round(Math.random() * 12345) + new Date().getTime(); /* returns a unique number */
    
    //internal usage
    o.tabType=Widget_param(prms, "tabType", _HorizTabTop);  
        
    o.name=Widget_param(prms, "name", "Tab");
    o.value=Widget_param(prms, "value", 0); 
    
    o.cb=Widget_param(prms, "clickCB", null);  
    o.dblClick=Widget_param(prms, "dblclickCB", null); 

    o.icon=Widget_param(prms, "icon", null);  
    o.iconW=Widget_param(prms, "iconW", (o.icon?_vertTabIconSize:1));  
    o.iconH=Widget_param(prms, "iconH", (o.icon?_vertTabIconSize:1));   
    o.iconOffX=Widget_param(prms, "iconOffX", 0);    
    o.iconOffY=Widget_param(prms, "iconOffY", 0); 
    
    o.tooltip=Widget_param(prms, "tooltip", null);  
     
    o.closeTabCB=Widget_param(prms, "closeTabCB", null);  
    
    o.isSelected=false;
    
    o.leftimgid="naviTabL_"+o.id
    o.midimgid="naviTabM_"+o.id
    o.rightimgid="naviTabR_"+o.id
    o.txtid="naviTabTxt_"+o.id
    o.sepid="naviTabSep_"+o.id
    o.closeid="naviTabClose_"+o.id
    o.iconid="naviTabIcon_"+o.id
    
    switch (o.tabType)
    {
        case _VertTab:
        case _VertTabWithIcon:
            o.tabCSSTable=_vertTabCSS;
            break;
        
        case _HorizTabBottom:
            o.tabCSSTable=_horizBottomTabCSS;
            break;
        
        case _HorizTabTopWithClose:
            o.tabCSSTable=_horizTabWithCloseCSS;
            break;
            
        case _HorizTabTop:
        default:
            o.tabCSSTable=_horizTabCSS;
            break;
            
        case _menuBarTab:
            o.tabCSSTable=_menuBarTabCSS;
            break;
    }
    
    o.mover=NaviTabWidget_mover;
    o.mdown=NaviTabWidget_mdown;
    o.keydownCB=NaviTabWidget_keydownCB;
    o.contextMenuCB=NaviTabWidget_contextMenuCB;
    o.changeState=NaviTabWidget_changeState;
    
    o.getVertHTML=NaviTabWidget_getVertHTML;
    o.getHorizHTML=NaviTabWidget_getHorizHTML;
    
    o.displaySep=NaviTabWidget_displaySep;
    
    o.isVert=NaviTabWidget_isVert;
    
    o.updateCloseIcon=NaviTabWidget_updateCloseIcon;
    o.mdown_closeIcon=NaviTabWidget_mdown_closeIcon;
    o.kdown_closeIcon=NaviTabWidget_kdown_closeIcon;
    
    o.clickCB=NaviTabWidget_clickCB;
    o.dblClickCB=NaviTabWidget_dblClickCB;
    
    o.leftimgLyr=null;
    o.midimgLyr=null;
    o.rightimgLyr=null;
    
    o.txtLyr=null;  
    o.iconLyr=null;
    
    //for horiz bar
    o.sepLyr=null;  
    o.closeLyr=null;
    
    o.data=new Object;
    return o;
}

// ================================================================================
function NaviTabWidget_init()
{
    var o=this;
    
    o.superInit();
    
    if (o.layer)
    {
        o.layer.onmouseover=o.mover;
        o.layer.onmouseout=o.mover;
        
        o.layer.onmousedown=o.mdown
        o.layer.onmouseup=o.mdown
        
        o.layer.onclick=o.clickCB;
        
        if (_ie)
            o.layer.ondblclick=o.dblClickCB;
        
        o.layer.onkeydown=o.keydownCB;
        
        o.layer.onselectstart=function() {return false;}
        o.layer.ondragstart=function() {return false;}
        
        o.layer.oncontextmenu=o.contextMenuCB;
    }
    
    o.leftimgLyr=getLayer(o.leftimgid);
    o.midimgLyr=getLayer(o.midimgid);
    o.rightimgLyr=getLayer(o.rightimgid);
    
    o.txtLyr=getLayer(o.txtid); 
    o.iconLyr=getLayer(o.iconid);   
    
    //for horiz navigation tab
    o.sepLyr=getLayer(o.sepid); 
    
    if (o.hasCloseButton())
    {
        o.closeLyr=getLayer(o.closeid); 
        
        o.closeLyr.onmousedown=o.mdown_closeIcon;
        o.closeLyr.onkeypress=o.kdown_closeIcon;
        o.closeLyr.onmouseup=o.mdown_closeIcon;
        
        o.updateCloseIcon("naviHTabCloseSel", o.isSelected);
    }
}

// ================================================================================

function NaviTabWidget_getVertHTML()
{
    var o=this;
    var s='';
    
    s='<table id="'+o.id+'" style="cursor:'+_hand+'" cellspacing="0" cellpadding="0" border="0">';
    s+='<tbody><tr height="'+ _mitemH+'">';
    
    //left
    var state=o.isSelected?_vertTabSelected:_vertTabNormal;
    s+='<td id="'+o.leftimgid+'" class="'+ o.tabCSSTable[_tabImgLeft][state]+'"><div style="width:'+_vertTabImgW+'px;"></div></td>';
    
    //middle
    var w=o.par.w-2*_vertTabImgW;
    
    s+='<td id="'+o.midimgid+'" class="'+ o.tabCSSTable[_tabImgMid][state]+'" width="'+w+'">'
    
    if (o.tabType==_VertTabWithIcon)
    {
        s+='<table cellspacing="0" cellpadding="0" border="0" width="100%">';
        s+='<tbody><tr height="'+ _mitemH+'">';
        
        //icon
        s+='<td style="padding-left:'+ _vertTabLBorderToIcon+'px;">'+
            imgOffset((o.icon?o.icon:(_skin+"../transp.gif")),_vertTabIconSize,o.iconH,o.iconOffX,o.iconOffY, o.iconid)+'</td>'
        
        //text
        var ww=Math.max(40,w-_vertTabLBorderToIcon-_vertTabIconSize);
        
        s+='<td  style="padding-left:'+ _vertTabIconToTxt +'px;width:'+ww+'px;" >'
        s+='<div id="'+o.txtid+'" class="naviVTabText" style="width:'+(ww-_vertTabIconToTxt-2)+'px;">'+convStr(o.name)+'</div></td>'
        
        s+='</tr></tbody></table>';
    }
    else
    {
        //_VertTab 
        s+='<div id="'+o.txtid+'" class="naviVTabText" style="padding-left:'+ _vertTabLBorderToTxt+'px;width:'+(w-_vertTabLBorderToTxt-2)+'px;">'
            +convStr(o.name)+'</div>';
    }
        
    s+='</td>';
    
    //right
    s+='<td id="'+o.rightimgid+'" class="'+o.tabCSSTable[_tabImgRight][state]+'"><div style="width:'+_vertTabImgW+'px"></div></td>';
    
    s+='</tr></tbody></table>';
    return s;
}

function NaviTabWidget_hasCloseButton () 
{
    var o = this;
    return _HorizTabTopWithClose == o.tabType && (!o.par.getTabIndexByName(o.name) == 0  || o.isFirstTabClosable);
}

// ================================================================================

function NaviTabWidget_getHorizHTML()
{
    var o=this;
    
    var s='<table id="'+o.id+'" style="cursor:'+_hand+'" cellspacing="0" cellpadding="0" border="0">';
    s+='<tbody><tr height="'+ _naviHorzTabH+'">';
    
    //left
    var state=o.isSelected?_horizTabSelected:_horizTabNormal;
    s+='<td id="'+o.leftimgid+'" class="'+ o.tabCSSTable[_tabImgLeft][state]+'" valign="top"><div style="width:'+_horizTabImgL+'px;"></div></td>';
    
    //middle
    s+='<td id="'+o.midimgid+'" valign="bottom" class="'+ o.tabCSSTable[_tabImgMid][state]+'">';
    s+='<table cellspacing="0" cellpadding="0" border="0" width="100%">';
    s+='<tbody><tr height="'+ _naviHorzTabH+'">';

    if (_menuBarTab!=o.tabType)
    {
        //icon
        var sty=(_HorizTabTop==o.tabType || _HorizTabTopWithClose==o.tabType)?"margin-bottom:3px;":"margin-bottom:6px;";
        sty=sty+"margin-left:"+((o.iconW>1)?_horizTabImgPadL:0)+"px;";
        
        s+='<td  valign="bottom" >'+
            imgOffset((o.icon?o.icon:(_skin+"../transp.gif")),o.iconW,o.iconH,o.iconOffX,o.iconOffY, o.iconid, null, null, sty)+'</td>';
                
    }
    
    //text
    var txt_sty=' style="padding-left:'+ ((o.iconW>1)?_horizTabImgToTxt:_horizTabTxtPaddingL)+
                'px;padding-right:'+((_HorizTabTopWithClose==o.tabType)?_horizTabTxtToClose:_horizTabTxtPaddingR)+
                'px;padding-bottom:'+((_HorizTabTop==o.tabType|| _HorizTabTopWithClose==o.tabType)?_horizTabTxtPaddingB:_horizBottomTabTxtPadB)+'px;" ';
                
    var txt='<div tabindex="0" role="tab" id="'+o.txtid+'" '+txt_sty+' class="'+o.tabCSSTable[_tabTxt][state]+'" >'+convStr(o.name)+'</div>';
    
    s+='<td valign="bottom">'+txt+'</td>';
        
    //close icon on top-right
    if (o.hasCloseButton())
    {
        s+='<td valign="top" style="padding-right:'+_horizTabClosePadR+'px;">';
        s+='<div tabindex="0" class="naviHTabCloseSel" id="'+o.closeid+'" role="button" title="'+_closeTab+' '+convStr(o.name)+'"></div></td>';
    }
    
    s+='</tr></tbody></table>';
    s+='</td>';
    
    //right
    s+='<td id="'+o.rightimgid+'" class="'+o.tabCSSTable[_tabImgRight][state]+'" valign="top">';
    s+='<div style="width:'+((_HorizTabTopWithClose==o.tabType)?_horizTabImgL:_horizTabImgR)+'px"></div></td>';
    
    //separator
    s+='<td  class="'+((o.tabType==_HorizTabBottom)?'naviHBottomTabMNormal':'naviHTabMNormal')+'">'
    s+='<div id="'+o.sepid+'" class="naviHTabSeparator"></div></td>'

    s+='</tr></tbody></table>';
    return s;
}
// ================================================================================
function NaviTabWidget_isVert()
{
    var o=this;
    return (o.tabType==_VertTabWithIcon || o.tabType==_VertTab);
}

// ================================================================================
function NaviTabWidget_getHTML()
{
    var o=this;
    
    return (o.isVert())?o.getVertHTML():o.getHorizHTML();
}

// ================================================================================
function NaviTabWidget_select(sel)
{
    var o=this;
    
    o.isSelected=sel;
    o.changeState(o.isVert()?(sel?_vertTabSelected:_vertTabNormal):(sel?_horizTabSelected:_horizTabNormal));
    
    o.updateCloseIcon("naviHTabCloseSel", o.isSelected);
}

// ================================================================================
function NaviTabWidget_updateCloseIcon(cls, show)
{
    var o=this;
    
    if (o.tabType==_HorizTabTopWithClose && o.closeLyr)
    {
        o.closeLyr.className=cls;
        o.closeLyr.style.visibility=show?_show:_hide;
    }
}

// ===============================================================================
function NaviTabWidget_changeState(state)
{
    var o=this;
    
    if (o.layer)
    {
        o.leftimgLyr.className=o.tabCSSTable[_tabImgLeft][state];
        o.midimgLyr.className=o.tabCSSTable[_tabImgMid][state];
        o.rightimgLyr.className=o.tabCSSTable[_tabImgRight][state];
    }
    if (o.txtLyr && !o.isVert())
    {
        o.txtLyr.className=o.tabCSSTable[_tabTxt][state];
    }
}

// ================================================================================
function NaviTabWidget_mover(evt)
{
    var o=getWidget(this);
    var evt=getEvent(evt);
    var over=(evt && evt.type=="mouseover")?true:false;
    
    if (o.isVert())
    {
        o.changeState(over?_vertTabHover:(o.isSelected?_vertTabSelected:_vertTabNormal));
    }
    else
    {
        //horizontal tabs 
        o.changeState(o.isSelected?(over?_horizTabSelHover:_horizTabSelected):(over?_horizTabHover:_horizTabNormal));
        
        //update separator display state
        var tabs=o.par.getPrevNextTabs(o.idx);
        
        if (!tabs) return;
        
        var prevTab=tabs.prevTab;
        var nextTab=tabs.nextTab;
        if (!o.isSelected)
        {
            var d=false;
            
            if (!over)
            {
                d=true;
                
                if (nextTab)
                {
                    if (nextTab.isSelected)
                        d=false;    
                }
                else
                    d=false;
            }   
            o.displaySep(d, true);
            
            if (prevTab)
                prevTab.displaySep(over?false:((prevTab.isSelected)?false:true), true); 
        }
        else
        {
            o.displaySep(false);
            if (prevTab)
                prevTab.displaySep(false);  
        }
        
        o.updateCloseIcon((over?"naviHTabCloseHover":"naviHTabCloseSel"), (o.isSelected ||over) );
    }
}

// ================================================================================
function NaviTabWidget_mdown(evt)
{
    var o=getWidget(this);
    var evt=getEvent(evt);
    var down=(evt && evt.type=="mousedown")?true:false;
    
    if (o.isVert())
    {
        o.changeState(down?_vertTabPressed:(o.isSelected?_vertTabSelected:_vertTabNormal));
    }
}

function NaviTabWidget_mdown_closeIcon(evt)
{
    var o=getWidget(this);
    var evt=getEvent(evt);
    var down=(evt && evt.type=="mousedown")?true:false;
    
    //o is NaviTabWidget
    o.updateCloseIcon((down?"naviHTabClosePressed":"naviHTabCloseSel"), (down || o.isSelected));
    
    if (!down)
    {   
        if (o.par && o.par.closeTab)    //NaviBarWidget
            o.par.closeTab(o.par.findTabIndex(o), o.closeTabCB);
    }
}

function NaviTabWidget_kdown_closeIcon(e)
{
    var k=eventGetKey(e);
    if(k==32 || k==13)//enter or spacebar
    {
        eventCancelBubble(e);
        var o=getWidget(this);
        if (o.par && o.par.closeTab)    //NaviBarWidget
            o.par.closeTab(o.par.findTabIndex(o), o.closeTabCB);
    }
}
// ================================================================================

function NaviTabWidget_clickCB(evt)
{
    var evt=getEvent(evt);
    var o=getWidget(this);
    
    if (o && o.cb)
        o.cb();
    
    eventCancelBubble(evt);
    return false;
}
// ================================================================================

function NaviTabWidget_dblClickCB(evt)
{
    var evt=getEvent(evt);
    var o=getWidget(this);
    
    if (o && o.dblClick)
        o.dblClick();
    
    eventCancelBubble(evt);
    return false;
}

// ================================================================================

function NaviTabWidget_keydownCB(e)
{
    var k=eventGetKey(e);
    
    if(k == 13)//enter
    {
        eventCancelBubble(e); 
        var o=getWidget(this);
        if (o && o.cb)
            o.cb();
    }
}

function NaviTabWidget_displaySep(d, effect)
{
    var o=this;
    
    if (!o.isVert() && o.sepLyr)
    {       
       /*if (d && effect) {
            new Effect.Opacity(o.id, {
                from: 0,
                to: 1,
                duration: 0.5
            })
       } else {*/
           o.sepLyr.style.visibility = d ? _show : _hide
       //}
    }
}
//================================================================================

function NaviTabWidget_setUserData(s)
{
    this.data.userdata=s;
}

function NaviTabWidget_getUserData()
{
    return this.data.userdata;
}

//================================================================================
function NaviTabWidget_contextMenuCB(evt)
// evt  [event] the event
// return void
{       
    evt=getEvent(evt);
    var tab=getWidget(this);        
    var tabbar = tab.par;
    
    if (tab && tab.cb)
        tab.cb();       //NaviBarWidget_itemClick
        
    if (tabbar && tabbar.showTabMenu)   
    {
        tabbar.showTabMenu(evt, tab.idx);   
    }
    eventCancelBubble(evt);
    return false;
}

//================================================================================
function NaviTabWidget_setMenu(m)
{
    this.data.menu=m;
}

function NaviTabWidget_getMenu()
{
    return this.data.menu;
}
    
function NaviTabWidget_setHtml(html)
{
    this.data.html=html;
}

function NaviTabWidget_getHtml()
{
    return this.data.html;
}
////================================================================================
//function NaviTabWidget_change(prms)
//// Change the tabs parameters (name, callbacks, icon)
//// prms's content:
////
//// name     : [String]   tab label
//// value    : [String - optional] a value that is used to find it again
////
//// cb       : [Function] callback pointer, called when clicking on the tab
//// dblClick
////
//// icon     : [String - optional] an image URL
//// iconW    : [int - optional] displayed image width
//// iconH    : [int - optional] displayed image height
//// iconOffX : [int - optional] x offset in the icon (for combined images)
//// iconOffY : [int - optional] y offset in the icon (for combined images)
//{
//	var o=this;
//		
//	if (prms.value)
//		o.value=prms.value;
//		
//	if (prms.cb)
//		o.cb=prms.cb;
//	
//	if (prms.dblClick)
//		o.dblClick=prms.dblClick; 
//
//	if (prms.tooltip)
//		o.tooltip=prms.tooltip;   
//		
//	if (typeof(prms.icon)!="undefined")
//		o.icon=prms.icon;  
//	
//	if (prms.iconW!=null)
//	{
//		o.iconW=prms.iconW;  
//	}	
//	else
//	if (typeof(prms.icon)!="undefined")
//		o.iconW=(o.icon)?_vertTabIconSize:1;
//		
//	if (prms.iconH!=null)
//		o.iconH=prms.iconH;   
//	else
//	if (typeof(prms.icon)!="undefined")
//		o.iconH=o.icon?_vertTabIconSize:1;
//	
//	if (prms.iconOffX!=null)
//		o.iconOffX=prms.iconOffX; 
//	
//	if (prms.iconOffY!=null)   
//		o.iconOffY=prms.iconOffY; 
//	
//	if ((typeof(prms.icon)!="undefined") || (prms.iconW!=null) || (prms.iconH!=null) || (prms.iconOffX!=null) || (prms.iconOffY!=null))
//	{
//		if (o.iconLyr)
//		{
//			changeOffset(o.iconLyr,o.iconOffX,o.iconOffY,(o.icon?o.icon:(_skin+"../transp.gif")));
//			
//			var w=(_VertTabWithIcon==o.tabType)?_vertTabIconSize:o.iconW;
//			
//			o.iconLyr.style.width=""+w+"px"
//			o.iconLyr.style.height=""+o.iconH+"px"
//		}
//	}
//	if (prms.name)
//	{
//		o.name=prms.name;
//		
//		if (o.txtLyr)
//		{
//			o.txtLyr.innerHTML=convStr(o.name,true);
//		}
//	}
//	
//	if (!o.isVert())
//	{
//		if (o.txtLyr)
//			o.txtLyr.style.paddingLeft=""+((o.iconW>1)?_horizTabImgToTxt:_horizTabTxtPaddingL)+"px";
//			
//		if (o.iconLyr)
//			o.iconLyr.style.marginLeft=""+((o.iconW>1)?_horizTabImgPadL:0)+"px";	
//	}
//	if (o.par && o.par.updateScrollIconState)
//		o.par.updateScrollIconState();	
//	
//}
// ================================================================================
//
// newNaviBarWidget: horizontal navigation bar or vertical navigation panel
//
// ================================================================================
function new_NaviBarWidget(prms){
//prms's content:
//
// naviBarType [enum]: _VertTab, _VertTabWithIcon, _HorizTabTop, _HorizTabBottom, _HorizTabTopWithClose
//
// vertical navigation panel:
// w [int]:optional, min:_vertNaviPanelMinW=120px, max:_vertNaviPanelMaxW=180px
// h [int]:optional, min:23px * 8 link=184px
//
// horizontal bar:
// w : required, if omitted, will take _horizBarWidth. The width includes the scroll bar if there is.
// h [not needed] is ignored.
//
// showScrollBar [boolean]:optinal
// showTabList[boolean - optinal]: if true, when resize, the tabs will be added in a menu if there is no enough space to show all of them.
//  
    var o = new_Widget(prms);
    
    o.superInit=o.init;
    o.oldResize=o.resize;
    
    // public API
    o.init=NaviBarWidget_init;
    o.getHTML=NaviBarWidget_getHTML;
    
    o.add=NaviBarWidget_add;
    o.addByPrms=NaviBarWidget_addByPrms;
    
    o.remove=NaviBarWidget_remove;
    o.removeAll=NaviBarWidget_removeAll;
    
    o.getCount=NaviBarWidget_getCount;
    o.select=NaviBarWidget_select;
    o.getSelection=NaviBarWidget_getSelection;
    
    o.getBarType=NaviBarWidget_getBarType;  
//    o.scroll=NaviBarWidget_scroll;
    
    o.getMenu=NaviBarWidget_getMenu;
    o.showMenu=NaviBarWidget_showMenu;
    
    o.getTabMenu=NaviBarWidget_getTabMenu;
    o.showTabMenu=NaviBarWidget_showTabMenu;
    
    o.setShowContextMenuAllowed=NaviBarWidget_setShowContextMenuAllowed;
    
    o.getTab=NaviBarWidget_getTab;
    o.findTabIndex=NaviBarWidget_findTabIndex
    o.getSelectedTab=NaviBarWidget_getSelectedTab;
    
    o.showTab=NaviBarWidget_showTab;
    
    o.resize=NaviBarWidget_resize;
    
    o.setTabHTML=NaviBarWidget_setTabHTML;
    o.getTabHTML=NaviBarWidget_getTabHTML;
    
    o.getTabIndexByName=NaviBarWidget_getTabIndexByName;
    o.getTabIndexByValue=NaviBarWidget_getTabIndexByValue;
    
    o.getTabID=NaviBarWidget_getTabID;  //from tab index to get the unique number NaviTabWidget.idx

    //internal usage
    var w = Widget_param(prms, "w", null)
    var h = Widget_param(prms, "h", null)
    o.cb = Widget_param(prms, "cb", null)
    o.isFirstTabClosable = Widget_param(prms, "isFirstTabClosable", true);
    o.dblclick = Widget_param(prms, "dblclick", null)   
    o.beforeShowTabMenu= Widget_param(prms, "beforeShowTabMenu", null);        
        
    o.type = Widget_param(prms, "naviBarType", _VertTab);
    o.counter=0;
    
    o.items=new Array;
    o.selIndex=-1;
    
    o.leftLimit=0;
    
    o.showContextMenuAllowed=true;
    o.menu=newMenuWidget("naviBarMenu_"+o.id,null,Widget_param(prms, "beforeShowMenu", null));  
    
    o.tabList=null;
    o.showScrollBar=_noScrollBar;
    
    o.isVert=((o.type== _VertTab) || (o.type==_VertTabWithIcon))?true:false;
    if (o.isVert)
    {
        if (w)
        {
            w=Math.max(_vertNaviPanelMinW,w);
            w=Math.min(w, _vertNaviPanelMaxW);
        }
        
        o.w=(w?w:((o.type== _VertTab)?_vertNaviPanelMinW:_vertNaviPanelWithIconW))+2    //border;
        
        if (h)
        {
            var n=Math.ceil(h/_mitemH);
            h=(Math.max(1, n))*_mitemH;
        }   
        o.h=(h?h:_vertNaviPanelH)+2;    //border
    }
    else
    {
        //horizontal bar
        //tab list
        var showTabList= Widget_param(prms, "showTabList", false);
        
        if (showTabList)
        {
            o.tabList=newMenuWidget("naviBarTabListMenu_"+o.id, null, NaviBarWidget_beforeShowTabListCB,
                                        NaviBarWidget_TabListonPositionCB);
                                        
            o.tabList.navibar=o;
        }
        else
        {
            var showScrollBar = Widget_param(prms, "showScrollBar", false);
            o.showScrollBar=showScrollBar?((_HorizTabBottom==o.type)?_ScrollBarAtBegin:_ScrollBarAtEnd):_noScrollBar;
        }
        
        o.w=w?w:(_horizBarWidth+_scrollBarWidth);
        o.h=_naviHorzTabH;
    }
    
    switch (o.type)
    {
        case _VertTab:
        case _VertTabWithIcon:
            o.tabCSSTable=_vertTabCSS;
            break;
        
        case _HorizTabBottom:
            o.tabCSSTable=_horizBottomTabCSS;
            break;
        
        case _HorizTabTop:
        default:
            o.tabCSSTable=_horizTabCSS;
            break;
    }
    
    //for horizontal navigation bar
    o.trid="naviBarTR_"+o.id;
    o.trLyr=null;
    
    o.divid="naviBarDIV_"+o.id;
    o.divLyr=null;
    
    //for scroll bar in horizontal bar at top or at bottom
    o.scrollbarid="scrlbar_"+o.id
    o.firstid="f_"+o.id
    o.previd="p_"+o.id
    o.nextid="n_"+o.id
    o.lastid="l_"+o.id
    
    o.scrollbarLyr=null;
    o.firstLyr=null;
    o.prevLyr=null;
    o.nexttLyr=null;
    o.lastLyr=null;
    
    o.updateSepDisplay=NaviBarWidget_updateSepDisplay;
    o.getPrevNextTabs=NaviBarWidget_getPrevNextTabs;
    
    o.closeTab=NaviBarWidget_closeTab;
    
    o.getBarIndex=NaviBarWidget_getBarIndex;
    
    o.mover_scrollbar=NaviBarWidget_mover_scrollbar;
    o.mdown_scrollbar=NaviBarWidget_mdown_scrollbar;
    o.contextMenuCB=NaviBarWidget_contextMenuCB;
    o.dblclickCB=NaviBarWidget_dblclickCB;
    
    o.getItemXPos=NaviBarWidget_getItemXPos;
//    o.updateScrollIconState=NaviBarWidget_updateScrollIconState;
    
//    o.firstCB=NaviBarWidget_firstCB;
//    o.prevCB=NaviBarWidget_prevCB;
//    o.nextCB=NaviBarWidget_nextCB;
//    o.lastCB=NaviBarWidget_lastCB;
    
    o.par=null;
    
    //scroll menu
    o.tablistid="bartablist_"+o.id;
    o.tablistLyr=null;
    
    o.mover_tablist=NaviBarWidget_mover_tablist;
    o.mdown_tablist=NaviBarWidget_mdown_tablist;
    o.kdown_tablist=NaviBarWidget_kdown_tablist;
    o.onfocus_tablist=NaviBarWidget_onfocus_tablist;
    o.onblur_tablist=NaviBarWidget_onblur_tablist;
    o.click_tablist=NaviBarWidget_click_tablist;
    o.onChangeTabList=NaviBarWidget_onChangeTabList;
    o.showTabListIcon=NaviBarWidget_showTabListIcon;
    o.buildTabList=NaviBarWidget_buildTabList;
    return o;
    
}

function newNaviBarWidget(id, naviBarType, cb, dblclick, w, h, beforeShowTabMenu, beforeShowMenu, showScrollBar, showTabList, isFirstTabClosable)
{
    return new_NaviBarWidget({
        id:id,
        naviBarType:naviBarType,
        cb:cb,
        dblclick:dblclick,
        w:w,
        h:h,
        beforeShowTabMenu:beforeShowTabMenu,
        beforeShowMenu:beforeShowMenu,
        showScrollBar:showScrollBar,
        showTabList:showTabList,
        isFirstTabClosable : isFirstTabClosable
    })
}

// ================================================================================
function NaviBarWidget_getBarType()
{
    return this.type;
}

// ================================================================================
function NaviBarWidget_init()
{
    var o=this,items=o.items
    o.superInit();
    
    o.trLyr=getLayer(o.trid);
    o.divLyr=getLayer(o.divid);
    
    var len = items.length
    for (var i=0;i<len;i++)
    {
        var it=items[i]
        it.init()
        it.select(i==o.selIndex)
    }   
    
    o.updateSepDisplay()
    
    if (o.tabList)
    {
        o.tablistLyr=getLayer(o.tablistid);
        
        o.tablistLyr.onmouseover=o.mover_tablist;
        o.tablistLyr.onmouseout=o.mover_tablist;
            
        o.tablistLyr.onmousedown=o.mdown_tablist;
        o.tablistLyr.onkeypress=o.kdown_tablist;
        o.tablistLyr.onmouseup=o.mdown_tablist;
        o.tablistLyr.onfocus=o.onfocus_tablist;
        o.tablistLyr.onblur=o.onblur_tablist;
        o.tablistLyr.onclick=o.click_tablist;
        
        o.tablistLyr.oncontextmenu=function() {return false;};
        
        o.showTabListIcon();
    }
    else
    if (o.showScrollBar!=_noScrollBar)
    {
        o.scrollbarLyr=getLayer(o.scrollbarid);
        
        o.firstLyr=getLayer(o.firstid);
        o.prevLyr=getLayer(o.previd);
        o.nexttLyr=getLayer(o.nextid);
        o.lastLyr=getLayer(o.lastid);
        
        var arr=[o.firstLyr, o.prevLyr, o.nexttLyr, o.lastLyr];
        
        for (var i=0; i<4; i++)
        {
            arr[i].onmouseover=o.mover_scrollbar;
            arr[i].onmouseout=o.mover_scrollbar;
            
            arr[i].onmousedown=o.mdown_scrollbar
            arr[i].onmouseup=o.mdown_scrollbar
        }
        o.firstLyr.onclick=o.firstCB
        o.prevLyr.onclick=o.prevCB
        o.nexttLyr.onclick=o.nextCB
        o.lastLyr.onclick=o.lastCB
        
        o.scrollbarLyr.oncontextmenu=function(){return false;}
    }
    
    if (o.isVert)
        o.layer.oncontextmenu=o.contextMenuCB;
    else
    if (o.divLyr)
        o.divLyr.oncontextmenu=o.contextMenuCB;
        
    if (o.cb)
    {
        if (o.isVert)
            o.layer.onclick=function(){o.cb();return false;}
        else
        if (o.divLyr)
            o.divLyr.onclick=function(){o.cb();return false;}
    }
    if (o.dblclick)
    {
        if (o.isVert)
            o.layer.ondblclick=o.dblclickCB;
        else
        if (o.divLyr)
            o.divLyr.ondblclick=o.dblclickCB;
    }
    o.layer.onselectstart=function() {return false;}
    o.layer.ondragstart=function() {return false;}
    
    o.resize(o.w,o.h);
}

// ================================================================================
function NaviBarWidget_dblclickCB(evt)
{
    var o=getWidget(this);
    var evt=getEvent(evt);
    
    if (o.dblclick)
        o.dblclick();
        
    eventCancelBubble(evt);
    return false;
}

function NaviBarWidget_mover_scrollbar(evt)
{
    if (this.disabled) return;
    
    var o=getWidget(this);
    var evt=getEvent(evt);
    var over=(evt && evt.type=="mouseover")?true:false;
    
    //o is NaviBarWidget
    this.className=over?o.tabCSSTable[_tabScrollBar][_scrollbarHover]:o.tabCSSTable[_tabScrollBar][_scrollbarM];
}

function NaviBarWidget_mdown_scrollbar(evt)
{
    if (this.disabled) return;
    
    var o=getWidget(this);
    var evt=getEvent(evt);
    var down=(evt && evt.type=="mousedown")?true:false;
    
    //o is NaviBarWidget
    this.className=down?o.tabCSSTable[_tabScrollBar][_scrollbarPressed]:o.tabCSSTable[_tabScrollBar][_scrollbarM];
}

function NaviBarWidget_getHTML()
{
    var o=this,items=o.items,len=items.length
    var s= '<div id="'+this.id+'" class="'+(o.isVert?"dlgFrame":"")+'" align="left" style="overflow:hidden;width:'+o.w+'px;height:'+o.h+'px">'
    
    s+='<table cellspacing="0" cellpadding="0" border="0" '+(o.isVert?' class="naviVTabBackgnd"':'')+'><tbody>'
    
    if (o.isVert)
    {
        for (var i=0;i<len;i++) 
            s+='<tr><td>'+items[i].getHTML()+'</td></tr>'
    }
    else
    {
        //horizontal navigation bar
        s+='<tr>'
        
        var scroll=''
        if (o.showScrollBar!=_noScrollBar)
        {
            scroll='<td>'
        
            scroll+='<table id="'+o.scrollbarid+'" cellspacing="0" cellpadding="0" border="0" width="'+_scrollBarWidth+'"><tbody><tr style="width:'+_scrollBarWidth+'px;">'
            
            scroll+='<td class="'+o.tabCSSTable[_tabScrollBar][_scrollbarL]+'"><div></div></td>'
            scroll+='<td id="'+o.firstid+'" class="'+o.tabCSSTable[_tabScrollBar][_scrollbarM]+'" align="center" valign="bottom"><div class="'+o.tabCSSTable[_tabScrollBar][_scrollbarFirst]+'"></div></td>'
            scroll+='<td id="'+o.previd+'" class="'+o.tabCSSTable[_tabScrollBar][_scrollbarM]+'" align="center" valign="bottom"><div class="'+o.tabCSSTable[_tabScrollBar][_scrollbarPrev]+'"></div></td>'
            scroll+='<td id="'+o.nextid+'" class="'+o.tabCSSTable[_tabScrollBar][_scrollbarM]+'" align="center" valign="bottom"><div class="'+o.tabCSSTable[_tabScrollBar][_scrollbarNext]+'"></div></td>'
            scroll+='<td id="'+o.lastid+'" class="'+o.tabCSSTable[_tabScrollBar][_scrollbarM]+'" align="center" valign="bottom"><div class="'+o.tabCSSTable[_tabScrollBar][_scrollbarLast]+'"></div></td>'
            scroll+='<td class="'+o.tabCSSTable[_tabScrollBar][_scrollbarR]+'"><div></div></td>'
            
            scroll+='</tr></tbody></table>'
            scroll+='</td>'
        }
        
        if (o.showScrollBar==_ScrollBarAtBegin)
        {
            s+=scroll;
        }
        
        var w=o.w;
        if (o.showScrollBar!=_noScrollBar)
        {
            w=Math.max(w-_scrollBarWidth, _horizBarWidth);
        }
        
        s+='<td><div style="overflow:hidden;width:'+w+'px;"  id="'+o.divid+'" class="'+o.tabCSSTable[_tabImgMid][_horizTabNormal]+'">'
        s+='<table cellspacing="0" cellpadding="0" border="0"><tbody>'
        s+='<tr id="'+o.trid+'">'
            
        for (var i=0;i<len;i++) 
        {
            s+='<td>'+items[i].getHTML()+'</td>'
        }
            
        s+='</tr></tbody></table></div></td>'
        
        if (o.tabList)
        {
            s+='<td class="'+o.tabCSSTable[_tabImgMid][_horizTabNormal]+'"><table cellspacing="0" cellpadding="0" border="0"><tbody><tr>'
            s+='<td tabindex=0 id="'+o.tablistid+'" class="'+o.tabCSSTable[_tabList][_tabListNormal]+'" role="button" title="'+L_bobj_crv_TabList+'">'
            s+='<div class="tabListIcon"></div></td>';
            s+='</tr></tbody></table></td>';
        }
        
        if (o.showScrollBar==_ScrollBarAtEnd)
        {
            s+=scroll;
        }
        
        s+='</tr>'
    }
    
    s+='</tbody></table></div>'
    
    return s    
}

// ================================================================================
function NaviBarWidget_add(name, value, idx, icon, iconW,iconH,iconOffX,iconOffY, tooltip, closeTabCB)
{
    return this.addByPrms({name:name, 
                    value:value, 
                    tooltip:tooltip, 
                    icon:icon, 
                    iconW:iconW,
                    iconH:iconH,
                    iconOffX:iconOffX,
                    iconOffY:iconOffY,
                    closeTabCB:closeTabCB
                    }, idx);
}

function NaviBarWidget_addByPrms(prms, idx)
// Add an element in the vertical navigation panel or the horizontal bar
//
// prms's content:
//
// name     : [String]   tab label
//
// value    : [String - optional] a value that is used to find it again
// icon     : [String - optional] an image URL
// iconW    : [int - optional] displayed image width
// iconH    : [int - optional] displayed image height
// iconOffX : [int - optional] x offset in the icon (for combined images)
// iconOffY : [int - optional] y offset in the icon (for combined images)
// closeTabCB:[function-optional]:only useful for _HorizTabTopWithClose
//
// idx: [int - optional] the index, if omitted or -1, added in the end 
{
    var o=this,counter=o.counter++  
    
    prms.id="naviTab_"+counter+"_"+o.id;
    prms.tabType= o.type;
    prms.clickCB=NaviBarWidget_itemClick;
    prms.dblclickCB=NaviBarWidget_itemDblClick;
    
    var obj=new_NaviTabWidget(prms);

    obj.par=o;
    obj.idx=counter;
    arrayAdd(o,"items",obj,idx);        
    
    var len=o.items.length;
        
    //for vertical navigation panel
    if (o.isVert && o.layer!=null)
    {   
        var tdElt=document.createElement("td");
        tdElt.innerHTML=obj.getHTML()
        
        var trElt=document.createElement("tr");
        
        trElt.appendChild(tdElt);
        
        var node=o.layer.childNodes[0].childNodes[0];
        
        if ((typeof(idx)=="undefined") || (len==1) ||(idx==null) || (idx==-1) || (idx >=len))
        {
            node.appendChild(trElt);
            idx=len-1;
        }
        else
        if (node.childNodes[parseInt(idx)])
            node.insertBefore(trElt,node.childNodes[parseInt(idx)]);
        
        obj.init();
    }
    else
    if (o.trLyr)
    {
        //horizontal navigation bar
        var tdElt=document.createElement("td");
        tdElt.innerHTML=obj.getHTML()
        
        if ((typeof(idx)=="undefined") || (len==1) || (idx==null) || (idx==-1) || (idx >=len))
        {
            o.trLyr.appendChild(tdElt);
            idx=len-1;
        }
        else
            o.trLyr.insertBefore(tdElt, o.trLyr.childNodes[parseInt(idx)]);
        
        obj.init();
        
        o.showTabListIcon();
    }
    //update the selection index
    if ((o.selIndex!=null) && (o.selIndex>=0))
    {
        if (idx<=o.selIndex)
            o.selIndex++;   
    }
    o.updateSepDisplay();
//    o.updateScrollIconState();
    return obj;
}

//=================================================================================
function NaviBarWidget_getBarIndex(itemIdx)
//itemIdx: get menu index from MenuBarWidget.idx
{
    var o=this,items=o.items,len=items.length
    
    for (var i=0; i<len;i++)
    {
        if (items[i].idx==itemIdx) return i;
    }
    return null;
}

function NaviBarWidget_itemClick()
//called by tab
{
    var o=this.par,items=o.items,len=items.length,index=-1
    for (var i=0;i<len;i++)
    {
        if (items[i].idx==this.idx)
        {
            o.select(i)
            index=i
            break
        }
    }
    
    if (o.cb)
        o.cb(index)     //NaviFrameWidget_tabClick
}

// ================================================================================

function NaviBarWidget_itemDblClick()
//called by tab
{
    var o=this.par,items=o.items,len=items.length,index=-1
    for (var i=0;i<len;i++)
    {
        if (items[i].idx==this.idx)
        {                   
            index=i
            break
        }
    }
    
    if (o.dblclick)
        o.dblclick(index)
}

// ================================================================================

function NaviBarWidget_select(index)
{
    if (index==null || typeof(index)=="undefined") return;
    
    var o=this,items=o.items,len=items.length
    if (index==-1) index=len-1
    
    if ((index>=0)&&(index<len))
    {
        if ((o.selIndex!=null) && (o.selIndex>=0)&&(o.selIndex!=index)&&(o.selIndex<len))
            items[o.selIndex].select(false)
    
        o.selIndex=index
        items[index].select(true)
        
        //update separator display state
        o.updateSepDisplay();
    }
}

// ================================================================================

function NaviBarWidget_updateSepDisplay()
{
    var o=this;
    
    if (o.isVert || (o.layer==null)) return;
    
    var items=o.items,len=items.length
    
    var d=true;
    for (var i=0;i<len;i++)
    {
        d=true;
        if (items[i].isSelected)
        {
            d=false;
        }
        else
        if ((i+1 < len) && items[i+1].isSelected)   //next one is selected
        {
            d=false;
        }
        else
        if (i==len-1)   //last one
        {
            d=false;
        }
        items[i].displaySep(d);
    }   
}

//================================================================================
function NaviBarWidget_closeTab(itemIndex, closeTabCB)
{
    var o=this;
    var i=o.getBarIndex(itemIndex);
    
    if (closeTabCB)
        closeTabCB(i);
    
    if (o.par && o.par.closeTab)
        o.par.closeTab(i);  //NaviFrameWidget in case
        
    o.remove(i);    //remove the tab from the bar
}


function NaviBarWidget_getPrevNextTabs(index)
{
    var o=this;
    var i=o.getBarIndex(index);
    var items=o.items,len=items.length;
    
    if (i!=null)
    {
        var ret=new Object;
            
        ret.prevTab=(i==0)?null:items[i-1];
        ret.nextTab=(i==len-1)?null:items[i+1];
        return ret;
    }
    
    return null;
}

//================================================================================
function NaviBarWidget_getItemXPos(index)
// Retrieve the x coordinates of a tab
// index    [int]   The tab index
// Returns  [int]   the x coordinates of a the tab
{
    var o=this
    var x=0
    for (var i=0;i<index;i++)
        x+=parseInt(o.items[i].getWidth())

    return x
}

//================================================================================
function NaviBarWidget_getCount()
// Get the number of tabs
// return [int] the number of tabs
{
    return this.items.length
}
//================================================================================
//function NaviBarWidget_updateScrollIconState()
//// Enable or disable the navigation icons first, previous, next, last
//{
//    var o=this
//
//    if (o.divLyr==null || (o.showScrollBar==_noScrollBar)) return;
//            
//    var tabsl=o.divLyr
//    var tabsSL=tabsl.scrollLeft,tabsOW=tabsl.offsetWidth,contentLen=o.trLyr.offsetWidth;
//    
//    // Enable or disable the navigation icons if no scroll is possible on the right
//    o.nexttLyr.disabled=(contentLen-tabsSL>tabsOW)?false:true;
//    o.lastLyr.disabled=(contentLen-tabsSL>tabsOW)?false:true;
//
//    // Enable or disable the navigation icons if no scroll is possible on the left
//    o.firstLyr.disabled=(tabsSL == 0)?true:false;
//    o.prevLyr.disabled=(tabsSL == 0)?true:false;
//    
//    //update scroll bar disabled state
//    var arr=[o.firstLyr, o.prevLyr, o.nexttLyr, o.lastLyr];
//    var dis;
//    for (var i=0; i<4; i++)
//    {
//        dis=arr[i].disabled;
//        arr[i].className=o.tabCSSTable[_tabScrollBar][(dis?_scrollbarDisabled:_scrollbarM)]
//        arr[i].childNodes[0].className=o.tabCSSTable[_tabScrollBar][(dis?(_scrollbarFirstDis+i):(_scrollbarFirst+i))]
//    }
//}
//================================================================================
//function NaviBarWidget_firstCB()
//// Callback function called when click on the "first" icon
//{
//    var o=getWidget(this);
//    o.scroll('first')   
//}
//function NaviBarWidget_prevCB()
//// Callback function called when click on the "previous" icon
//{
//    var o=getWidget(this);
//    o.scroll('previous')    
//}
//function NaviBarWidget_nextCB()
//// Callback function called when click on the "next" icon
//{
//    var o=getWidget(this);
//    o.scroll('next')    
//}
//function NaviBarWidget_lastCB()
//// Callback function called when click on the "last" icon
//{
//    var o=getWidget(this);
//    o.scroll('last')    
//}

//================================================================================
//function NaviBarWidget_scroll(step,destItem)
//// Scroll the NaviBarWidget in order for the <destIndex> tab to be visible
//{
//    var o=this
//
//    if ((o.divLyr==null) || (o.showScrollBar==_noScrollBar)) return;
//            
//    var tabsl=o.divLyr
//    var tabsSL=tabsl.scrollLeft,tabsOW=tabsl.offsetWidth,tabsSW=tabsl.scrollWidth,SLMax=tabsSW-tabsOW
//
//    //alert("AVANT scroll\nstep="+step+"\no.leftLimit="+o.leftLimit+"\ntabsSL="+tabsSL+"\nSLMax="+SLMax)
//    
//    // Scroll
//    if (step=='first')      // go to first tab
//    {
//        tabsl.scrollLeft=tabsSL=0
//        o.leftLimit=0
//    }
//    else
//    if (step=='previous')   // go to previous tab
//    {   
//        if (o.leftLimit > 0)
//        {
//            o.leftLimit=o.leftLimit-1   
//            var x=o.getItemXPos(o.leftLimit)
//            tabsl.scrollLeft=tabsSL=x
//        }
//        else return;        
//    }
//    else
//    if (step=='next')       // go to next tab
//    {
//        if (o.leftLimit>o.getCount()-1)
//            return;
//            
//        if (tabsSL < SLMax)
//        {
//            o.leftLimit+=1
//            var x=o.getItemXPos(o.leftLimit)
//            if (x<SLMax)
//                tabsl.scrollLeft=tabsSL=x
//            else
//                tabsl.scrollLeft=tabsSL=SLMax
//        }
//        else return;
//    }
//    else
//    if (step=='last')       // go to last tab
//    {       
//        for (var i=0;i<o.getCount();i++)
//        {
//            var x=o.getItemXPos(i);
//            if (x>SLMax)
//                break;
//        }
//        tabsl.scrollLeft=tabsSL=Math.max(0,SLMax)
//        o.leftLimit=i
//    }
//    else
//    if (step==null)         // go to the specified <destItem>
//    {
//        var x=o.getItemXPos(destItem);
//        if (x<SLMax)
//            tabsl.scrollLeft=tabsSL=x
//        else
//            tabsl.scrollLeft=tabsSL=SLMax
//        for (var i=0;i<o.getCount();i++)
//        {
//            var x=o.getItemXPos(i)
//            if (x>SLMax)
//                break;
//        }
//        o.leftLimit=i
//    }
//    else return;
//    
//    o.updateScrollIconState()
//
//    //alert("AVANT scroll\nstep="+step+"\no.leftLimit="+o.leftLimit+"\ntabsSL="+tabsSL+"\nSLMax="+SLMax)
//}

//================================================================================
function NaviBarWidget_mover_tablist(evt)
{   
    var o=getWidget(this);
    var evt=getEvent(evt);
    var over=(evt && evt.type=="mouseover")?true:false;
    
    //o is NaviBarWidget
    this.className=over?o.tabCSSTable[_tabList][_tabListHover]:o.tabCSSTable[_tabList][_tabListNormal];
}
function NaviBarWidget_mdown_tablist(evt)
{
    var o=getWidget(this);
    var evt=getEvent(evt);
    var down=(evt && evt.type=="mousedown")?true:false;
    
    //o is NaviBarWidget
    this.className=down?o.tabCSSTable[_tabList][_tabListPressed]:o.tabCSSTable[_tabList][_tabListNormal];
}

function NaviBarWidget_onfocus_tablist(evt)
{
    var o=getWidget(this);
    this.className=o.tabCSSTable[_tabList][_tabListHover];
}

function NaviBarWidget_onblur_tablist(evt)
{
    var o=getWidget(this);
    this.className=o.tabCSSTable[_tabList][_tabListNormal];
}

function NaviBarWidget_kdown_tablist(evt)
{
    if(eventGetKey(evt) == 13)//enter
    {
        NaviBarWidget_click_tablist.apply(this);
    }
}
//================================================================================
function NaviBarWidget_click_tablist()
{
    //"this" is the tablist icon, o is NaviBarWidget
    var o=getWidget(this);
    
    o.buildTabList();   
    
    //x, y will be set via NaviBarWidget_TabListonPositionCB()
    var pos =  NaviBarWidget_TabListonPositionCB.apply(o);
    o.tabList.show(true,pos.x, pos.y);
    o.tabList.resetTooltips();
}

function NaviBarWidget_TabListonPositionCB()
{
    var o=this;
    
    if (!o) return;
    
    if(!o.tabList.layer)
        o.tabList.justInTimeInit();
    
    var tablist_pos=getPosScrolled(o.tablistLyr);
    var pos=new Object;
        
    pos.x=Math.max(0, (tablist_pos.x+ 23 -o.tabList.getWidth()));
    pos.y=(tablist_pos.y+ o.tablistLyr.offsetHeight);

    return pos;
}

function NaviBarWidget_beforeShowTabListCB()
{
    //this is NaviBarWidget.tabList menu widget
    var o=this.navibar;
    
    if (!o) return;
    
    var tab=o.getSelectedTab();
    
    if (tab)
    {
        var menuitem=this.getItemByID(tab.idx);
        
        if (menuitem)
            menuitem.setTextClass("tabListMenuItem");
    }
}
//================================================================================
function NaviBarWidget_remove(idx, autoSelectNext)
{
    var o=this,items=o.items,len=items.length

    if ((idx>=0)&&(idx<len))
    {
        var elem=items[idx]
    
        arrayRemove(o,"items",idx)
        items=o.items
        len=items.length

        var l=elem.layer
        if (l!=null)
        {
            if (o.isVert && o.layer)
            {
                var row=l.parentNode.parentNode;
                var node=o.layer.childNodes[0].childNodes[0];
                node.removeChild(row);
            }
            else
            if (o.trLyr)
            {
                o.trLyr.removeChild(l.parentNode);
            }
            
            if(elem.zoneId)
            {
                var zoneLayer = getLayer(elem.zoneId);
                if(zoneLayer)
                {
                    zoneLayer.parentNode.removeChild(zoneLayer);   
                }
            }
        }
        
        for(var i = 0; i < len; i++) {
            if(items[i].isSelected) {
                o.selIndex = i;
                break;
            }
        }

        if(autoSelectNext) {
            if (o.selIndex>idx) 
                o.cb(o.selIndex-1)
            else if ((o.selIndex==idx) && (len>0))
                o.cb(Math.min(idx,len-1))
        }
        
        if (len==0) //empty bar
            o.selIndex=null;
        
//        o.updateScrollIconState();
        
        o.showTabListIcon();
    }
    
}

//================================================================================
function NaviBarWidget_removeAll()
{
    var o=this,items=o.items, len= items.length 
    for (var i=len-1;i>=0;i--)
        o.remove(i)         
}

//================================================================================
function NaviBarWidget_setTabHTML(index, s)
//index [int]: start from 0 indicating tab 1,...
{
    var o=this
    
    if (typeof(index)=="undefined" || index==null) index=o.items.length-1;
    
    if (index>=0 && index <=(o.items.length-1))
    {
        o.items[index].setHtml(s);
    }       
}
function NaviBarWidget_getTabHTML(index)
//index [int]: start from 0 indicating tab 1,...
{
    var o=this
    
    if (index>=0 && index <=(o.items.length-1))
    {   
        return o.items[index].getHtml();
    }   
    return "";  
}
//================================================================================
function NaviBarWidget_getSelection()
// get the selection
// return an object (or null if no selection). Object fields
//    - index [int] : the selection index
//    - value [String] : value of the select object
{
    var o=this;
    
    if (o.getCount()==0)
    {
        o.selIndex=-1;
        return null;
    }
    
    var index=o.selIndex;
    if ((index!=null) && (index>=0) && o.items[index])
    {
        var obj=new Object;
        obj.index=index;
        obj.valueOf=o.items[index].value;
        obj.name=o.items[index].name;
        return obj;
    }
    else
        return null;
}
//================================================================================
function NaviBarWidget_getMenu()
{
    return this.menu
}
function NaviBarWidget_getTabMenu(index)
{
    var menu=null;
    var o=this,items=o.items,len=items.length;
    
    if ((index>=0) && (index<len))
    {
        menu=items[index].getMenu();
        
        if (!menu)
        {
            menu=newMenuWidget(("naviTabMenu_"+o.id+"_"+o.getTabID(index)),null, o.beforeShowTabMenu);  
            items[index].setMenu(menu);
        }       
    }
    return menu;
}

function NaviBarWidget_setShowContextMenuAllowed(b)
{
    this.showContextMenuAllowed=b
}

function NaviBarWidget_showMenu(evt)
// Show the menu
// e    : event
// return void
{
    if (this.showContextMenuAllowed==false)
        return
        
    evt=getEvent(evt);

    this.menu.show(true,(eventGetX(evt)+winScrollX()),(eventGetY(evt)+winScrollY()))    
}

function NaviBarWidget_showTabMenu(evt, itemIndex)
// Show the menu
// e    : event
// return void
{
    if (this.showContextMenuAllowed==false)
        return
        
    evt=getEvent(evt);

    var index=this.getBarIndex(itemIndex);
    var menu=this.items[index].getMenu();
    
    if (menu)
        menu.show(true,(eventGetX(evt)+winScrollX()),(eventGetY(evt)+winScrollY()));    
}
//================================================================================
function NaviBarWidget_showTab(index,show)
{
    var o=this,items=o.items,len=items.length
    if ((index>=0) && (index<len))
        items[index].show(show)
}

//================================================================================
function NaviBarWidget_showTabListIcon()
{
    var o=this;
    
    if (o.tablistLyr)
        o.tablistLyr.style.visibility=(o.items.length>1)?_show:_hide
}

//================================================================================
function NaviBarWidget_tabListMenuItemsCB()
{
    //this is newMenuItem, newMenuItem.par is MenuWidget, MenuWidget.navibar is NaviBarWidget
    var o=this.par.navibar;
    
    if (!o) return;
    
    //get tab index from tab unique idx
    var tab_index=o.getBarIndex(parseInt(this.id));
    
    if ((tab_index>=0) && (tab_index<o.items.length) && o.divLyr && o.tabList)
    {
        o.onChangeTabList(tab_index);
        
        o.select(tab_index);
        
        if (o.cb)
            o.cb(tab_index);
    }
}

function NaviBarWidget_onChangeTabList(visibleIdx)
{
    if (visibleIdx<0) visibleIdx=0;
    
    var o=this;
    var items=o.items,len=items.length;
    
    if (!o.divLyr || !o.trLyr || !o.tabList || !len) return;
    
    //at least one visible item
    var toRight=visibleIdx;
    var toLeft=visibleIdx;
    var nLeftMost=visibleIdx;   
    var visibleLen=o.divLyr.offsetWidth;
    var nItemsLen=items[visibleIdx].getWidth();
    if (o.trLyr.offsetWidth>o.divLyr.offsetWidth)
    {
        if (nItemsLen<visibleLen)
        {
            while (true)
            {
                toRight++;
                
                if (toRight < len)
                {
                    if ((items[toRight].getWidth()+nItemsLen) < visibleLen)
                    {
                        nItemsLen+=items[toRight].getWidth();
                    }
                    else break;
                }
                
                toLeft--;   
                if (toLeft>=0)
                {
                    if ((items[toLeft].getWidth()+nItemsLen) < visibleLen)
                    {
                        nItemsLen+=items[toLeft].getWidth();
                        
                        nLeftMost=toLeft;
                    }
                    else break;
                }
            }
        }
    }
    else nLeftMost=0;   
    
    if (nLeftMost>=0)
    {
        o.divLyr.scrollLeft=o.getItemXPos(nLeftMost);
    }
}

function NaviBarWidget_buildTabList()
{
    var o=this;
    var items=o.items,len=items.length;
    
    //build tab list menu items
    o.tabList.removeAll();
    for (var i=0; i<len; i++)
    {
        o.tabList.add((""+items[i].idx), items[i].name, NaviBarWidget_tabListMenuItemsCB);
    }
}

//================================================================================
function NaviBarWidget_resize(w,h)
// Resizes the TabBarWidget
// w    [int]   The new tabBarWidget width
// h    [int]   The new tabBarWidget height
{
    var o=this
    
    if (o.isVert) return;
    
    o.oldResize(w)
    if (w!=null)
    {
        o.w=w;
        //horizontal bar
        if (o.divLyr)
        {
            if (o.tabList)
                w=Math.max((w-_tabListIconWidth), _horizBarWidth);
            else
            if (o.showScrollBar!=_noScrollBar)
                w=Math.max(o.w-_scrollBarWidth, _horizBarWidth);
            
            o.divLyr.style.width=''+w+'px';
        }
        
        o.onChangeTabList(o.selIndex);
    }
        
//    o.updateScrollIconState()
}

//================================================================================
function NaviBarWidget_contextMenuCB(evt)
// evt  [event] the event
// return void
{       
    evt=getEvent(evt);
    var tabbar=getWidget(this);     
    
    if (tabbar.cb)
        tabbar.cb();
        
    if (tabbar.showMenu)    
        tabbar.showMenu(evt);   
    
    return false;
}
// ================================================================================
function NaviBarWidget_getTabIndexByName(name)
{
    var o=this,items=o.items,len=items.length
    
    for (var i=0;i<len;i++)
    {
        if (items[i].name==name) return i;
    }
    return -1;
}
function NaviBarWidget_getTabIndexByValue(value)
{
    var o=this,items=o.items,len=items.length
    
    for (var i=0;i<len;i++)
    {
        if (items[i].value==value) return i;
    }
    return -1;
}
//=================================================================================
function NaviBarWidget_getTabID(tabIndex)
{
    var o=this,items=o.items;
    
    if ((tabIndex!=null) && (tabIndex>=0) && (tabIndex<items.length))
        return items[tabIndex].idx;
        
    return null;
}
//=================================================================================
function NaviBarWidget_getTab(index)
{
    var o=this,items=o.items;
    
    if ((index!=null) && (index>=0) && (index<items.length))
        return items[index];
        
    return null;
}

function NaviBarWidget_findTabIndex(tab)
{
    var o=this,items=o.items,l=items.length;
    
    for(var i = 0; i < l; i++)
        if(tab == items[i])
            return i;
        
    return -1;
}

function NaviBarWidget_getSelectedTab()
{
    var o=this;
    
    var sel=o.getSelection();
    
    if (sel)
        return o.getTab(sel.index);
        
    return null;
}
//
////=================================================================================
////=================================================================================
////
//// OBJECT new_MenuBarTabWidget (Constructor)
////
////
////=================================================================================
////=================================================================================
//function new_MenuBarTabWidget(prms)
////id:id
////name:name
////value:value
//{
//    prms.tabType=_menuBarTab;
//    
//    var o=new_NaviTabWidget(prms);
//    
//    //public API
//    o.getHTML=MenuBarTabWidget_getHTML;
//    
//    o.mover=MenuBarTabWidget_mover;
//    o.mdown=MenuBarTabWidget_mdown;
//    o.contextMenuCB=MenuBarTabWidget_contextMenuCB;
//    
//    o.keydownCB=MenuBarTabWidget_none;
//    o.clickCB=MenuBarTabWidget_none;
//    o.dblClickCB=MenuBarTabWidget_none;
//    
//    return o;
//}
//
////================================================================================
//function MenuBarTabWidget_getHTML()
//{
//    var o=this;
//    
//    var s='<table id="'+o.id+'"  style="cursor:'+_hand+'" cellspacing="0" cellpadding="0" border="0">';
//    
//    s+='<tbody><tr height="'+ _menuBarTabHeight+'" valign="middle">';
//    
//    //left
//    s+='<td id="'+o.leftimgid+'" ><div style="width:3px;"></div></td>';
//    
//    //middle
//    s+='<td id="'+o.midimgid+'" >';
//    s+='<div style="padding-left:5px;padding-right:5px;" class="naviHTabText">'+convStr(o.name)+'</div>';
//    s+='</td>';
//    
//    //right
//    s+='<td id="'+o.rightimgid+'" ><div style="width:3px;"></div></td>';
//    
//    //space between menus
//    s+='<td style="cursor:default;"><div style="width:'+_spaceBetweenInMenuBar+'px;" ></div></td>';
//        
//    s+='</tr></tbody></table>';
//    return s;
//}
//
////================================================================================
//function MenuBarTabWidget_mover(evt)
//{
//    var o=getWidget(this);
//    
//    var cxtmenu=o.par.getContextMenu();
//    
//    if (!cxtmenu || !cxtmenu.isShown()) 
//    {
//        var evt=getEvent(evt);
//        var over=(evt && evt.type=="mouseover")?true:false;
//        
//        var menu=o.getMenu();
//        var state=(over || (menu && menu.isShown()))?_menuBarHover:_menuBarNormal;
//        
//        o.changeState(state);
//        
//        //debuggingLogger("tab mover  state="+(state),300,300,200,400);
//
//        if (over)
//            o.par.mover(o.idx);
//    }
//    eventCancelBubble(evt);
//    return false;
//}
//
//function MenuBarTabWidget_mdown(evt)
//{
//    var o=getWidget(this);
//    var evt=getEvent(evt);
//    var down=(evt && evt.type=="mousedown")?true:false;
//    
//    var cxtmenu=o.par.getContextMenu();
//    if (cxtmenu.isShown())
//        cxtmenu.show(false);
//
//    var menu=o.getMenu();       
//    var state=down?_menuBarPressed:((menu && menu.isShown())?_menuBarHover:_menuBarNormal); 
//    o.changeState(state);
//    
//    //debuggingLogger("tab mdown  state="+(state),300,300,200,400);
//
//    //is the left button down?
//    if (down)
//    {
//        if (_ie)
//        {
//            if (evt.button!=1) return false;
//        }
//        else
//        {
//            if (evt.which!=1) return false;
//        }
//    }
//    
//    if (down && o.par)
//        o.par.mdown(o.idx);
//        
//    eventCancelBubble(evt);
//    return false;
//}
//function MenuBarTabWidget_contextMenuCB(evt)
//{
//    var o=getWidget(this);
//    var evt=getEvent(evt);
//    
//    if (o.par && o.par.showMenu)    
//        o.par.showMenu(evt);    
//        
//    eventCancelBubble(evt);
//    return false;
//}
//function MenuBarTabWidget_none(evt)
//{
//    eventCancelBubble(evt);
//    return false;
//}
//
////=================================================================================
//function MenuBarTabWidget_hideMenuCB()
//{
//    var o=this.bartab;  //"this" is the tab menu
//    
//    if (!o) return;
//    
//    o.changeState(_menuBarNormal);  
//    
//    o.par.shownMenuTabIdx=-1;
//}
//
//function MenuBarTabWidget_beforeShowCB()
//{
//    var o=this.bartab;  //"this" is the tab menu
//    
//    if (!o) return;
//    
//    o.par.shownMenuTabIdx=o.idx;
//}
////=================================================================================
////=================================================================================
////
//// OBJECT new_MenuBarWidget (Constructor)
////
////
////=================================================================================
////=================================================================================
//function new_MenuBarWidget(prms)
////id:id
////width:width
////beforeShowMenu [optional]
//{
//    // Base class
//    var o = new_Widget(prms);
//    
//    // Parameters parsing
//    o.cssClassName = Widget_param(prms, "cssClassName", "toolbarBackgnd");
//    o.marginTop = Widget_param(prms, "marginTop", 2);
//
//    
//    o.superInit=o.init;
//    
//    //public API
//    o.init=MenuBarWidget_init;
//    o.getHTML=MenuBarWidget_getHTML;
//     
//    o.add=MenuBarWidget_add;
//    o.remove=MenuBarWidget_remove;
//    o.removeByValue=MenuBarWidget_removeByValue;
//    o.removeAll=MenuBarWidget_removeAll;
//        
//    o.getMenu=MenuBarWidget_getMenu;    //to add menu items in a menu 
//    
//    o.getContextMenu=MenuBarWidget_getContextMenu;
//    o.setShowContextMenuAllowed=MenuBarWidget_setShowContextMenuAllowed;
//
//    o.getCount=MenuBarWidget_getCount;
//    
//    //internal usage
//    o.width = Widget_param(prms, "width", _defaultMenuBarWidth);
//    o.menu=newMenuWidget("menuBarContextMenu_"+o.id,null,Widget_param(prms, "beforeShowMenu", null));   
//        
//    o.counter=0;   
//    o.items=new Array;
//    o.showContextMenuAllowed=true;
//    
//    o.showMenu=MenuBarWidget_showMenu;
//    o.mover=MenuBarWidget_mover;
//    o.mdown=MenuBarWidget_mdown;
//    o.getShownMenuIndex=MenuBarWidget_getShownMenuIndex;
//    o.contextMenuCB=MenuBarWidget_contextMenuCB;
//    o.getBarIndex=MenuBarWidget_getBarIndex;
//    
//    o.shownMenuTabIdx=-1;
//    return o;
//}
//
////=================================================================================
//function MenuBarWidget_init()
//{
//    var o=this;
//    
//    o.superInit();
//    
//    o.layer.oncontextmenu=o.contextMenuCB;
//    o.layer.onselectstart=function() {return false;}
//    o.layer.ondragstart=function() {return false;}
//    
//    var items=o.items,len = items.length
//    for (var i=0;i<len;i++)
//    {
//        var it=items[i]
//        it.init()
//    }   
//}
////=================================================================================
//function MenuBarWidget_getHTML()
//{
//    var o=this,items=o.items,len=items.length;
//    var s='<div class="'+o.cssClassName+'" id="'+o.id+'" align="left" style="width:'+o.width+'px;overflow:hidden;">'
//    
//    s+='<table cellspacing="0"  cellpadding="0" border="0" style="margin-top:'+o.marginTop+'px;" height="'+_menuBarTabHeight+'"><tbody><tr valign="middle">'
//    for (var i=0; i<len; i++)
//    {
//        s+='<td >'+items[i].getHTML()+'</td>';
//    }
//    
//    s+='</tr></tbody></table></div>'
//    
//    return s;
//}
//
////=================================================================================
//function MenuBarWidget_add(name, value, idx)
//// name : [String]   menu label
//// value: [String - optional] a value that is used to find it again
//// idx  : [int  - optional] the position in the menu bar, starting from 0; if omitted, add in the end 
//{
//    var o=this,counter=o.counter++  
//    var obj=new_MenuBarTabWidget({id:"menuBarTab_"+counter+"_"+o.id, 
//                                  name:name, 
//                                  value:value
//                                }); 
//
//    obj.par=o;
//    obj.idx=counter;
//    arrayAdd(o,"items",obj,idx);
//            
//    var len=o.items.length;
//    if (o.layer)
//    {
//        var trLyr=o.layer.childNodes[0].childNodes[0].childNodes[0];
//        var tdElt=document.createElement("td");
//        tdElt.innerHTML=obj.getHTML();
//        
//        if ((typeof(idx)=="undefined") || (len==1) || (idx==null) || (idx==-1) || (idx >=len))
//        {
//            trLyr.appendChild(tdElt);
//        }
//        else
//            trLyr.insertBefore(tdElt, trLyr.childNodes[parseInt(idx)]);
//        
//        obj.init();
//    }
//    return obj;
//}
//
//function MenuBarWidget_remove(idx)
////idx: index from menu bar starting from 0
//{
//    var o=this,items=o.items,len=items.length
//
//    if ((idx>=0)&&(idx<len))
//    {
//        var elem=items[idx];
//    
//        arrayRemove(o,"items",idx);
//
//        var l=elem.layer;
//        if (l!=null)
//        {
//            var trLyr=o.layer.childNodes[0].childNodes[0].childNodes[0];
//            if (trLyr)
//            {
//                trLyr.removeChild(l.parentNode);
//            }
//        }
//    }
//}
//
//function MenuBarWidget_removeByValue(value)
//{
//    var o=this,items=o.items,len=items.length
//    var index=null;
//    for (var i=0; i<len;i++)
//    {
//        if (value==items[i].value) 
//        {
//            index=i;
//            break;
//        }
//    }
//    if (index==null) return;
//    
//    o.remove(index);
//}
//
////================================================================================
//function MenuBarWidget_removeAll()
//{
//    var o=this,items=o.items, len= items.length 
//    for (var i=len-1;i>=0;i--)
//        o.remove(i)         
//}
////================================================================================
//function MenuBarWidget_getCount()
//{
//    return this.items.length;
//}
////=================================================================================
//function MenuBarWidget_setMenu(idx, menu)
////idx: index for menu starting from 0
//{
//    var o=this,items=o.items,len=items.length
//
//    if ((idx>=0)&&(idx<len))
//    {
//        items[idx].setMenu(menu);
//    }
//}
//
//function MenuBarWidget_getMenu(idx)
////idx: index for menu starting from 0
//{
//    var o=this,items=o.items;
//
//    if ((idx>=0)&&(idx<items.length))
//    {
//        var menu=items[idx].getMenu();
//        
//        if (!menu)
//        {
//            menu=newMenuWidget({id:("menuBarTabMenu_"+items[idx].idx+"_"+o.id),
//                                hideCB:MenuBarTabWidget_hideMenuCB,
//                                beforeShowCB:MenuBarTabWidget_beforeShowCB});
//            items[idx].setMenu(menu);
//            menu.bartab=items[idx];
//            return menu;
//        }   
//    }
//    return null;
//}
//
////=================================================================================
//function MenuBarWidget_getShownMenuIndex()
////return: index where menu is shown, otherwise null
//{
//    return this.getBarIndex(this.shownMenuTabIdx);
//}
////=================================================================================
//function MenuBarWidget_getBarIndex(itemIdx)
////itemIdx: get menu index from MenuBarWidget.idx
//{
//    if (itemIdx>=0)
//    {
//        var o=this,items=o.items,len=items.length
//        
//        for (var i=0; i<len;i++)
//        {
//            if (items[i].idx==itemIdx) return i;
//        }
//    }
//    return null;
//}
//
////================================================================================
//function MenuBarWidget_mover(itemIdx)
////called by MenuBarTabWidget
////idx: menu bar tab index
//{
//    var o=this,items=o.items
//    
//    var index=o.getBarIndex(itemIdx);
//    if (index==null) return;
//    
//    var oldIndex=o.getShownMenuIndex();
//    
//    /*
//    debuggingLogger("----------------------------------------",300,300,200,400);
//    debuggingLogger("bar mover      oldIndex="+(oldIndex) +" index="+(index),300,300,200,400);
//    debuggingLogger("----------------------------------------",300,300,200,400);
//    */  
//    if ((oldIndex!=null) && (oldIndex>=0) && (index!=oldIndex))
//    {
//        var oldMenu=items[oldIndex].getMenu();
//        
//        if (oldMenu)
//        {
//            oldMenu.show(false);
//            
//            //debuggingLogger("bar mover     close menu "+(oldIndex),300,300,200,400);
//            //debuggingLogger(oldMenu.id+" isShown: "+(oldMenu.css.display) +" bool="+(oldMenu.css.display!='none'),300,300,200,400);
//        }
//        var item=items[index], l=item.layer, menu=item.getMenu();
//        if (menu)
//        {
//            menu.show(true,getScrolledPos(l).x,getScrolledPos(l).y+item.getHeight()+1,null,null,item);
//        }
//    }
//}
//function MenuBarWidget_mdown(itemIdx)
////called by menu bar tab
//{
//    var o=this,items=o.items;
//    var index=o.getBarIndex(itemIdx);
//    if (index==null) return;
//    
//    var oldIndex=o.getShownMenuIndex();
//    var item=items[index];
//    var menu=item.getMenu();
//    /*
//    debuggingLogger("----------------------------------------",300,300,200,400);
//    debuggingLogger("bar mdown   oldIndex="+(oldIndex) +" index="+(index),300,300,200,400);
//    debuggingLogger("----------------------------------------",300,300,200,400);
//    */
//    
//    //todo
//    //MenuWidget_globalClick();
//    
//    if (menu)   
//    {
//        if (oldIndex!=index)
//        {
//            var l=item.layer;
//            
//            menu.show(true, getScrolledPos(l).x,getScrolledPos(l).y+item.getHeight()+1,null,null,item);
//            
//            //debuggingLogger(menu.id+" isShown: "+(menu.css.display) +" bool="+(menu.css.display!='none'),300,300,200,400);
//    
//        }
//        else
//        {
//            menu.show(false);
//        }
//    }
//}
////=================================================================================
//function MenuBarWidget_contextMenuCB(evt)
//{
//    evt=getEvent(evt);
//    var bar=getWidget(this);        
//    
//    if (bar.cb)
//        bar.cb();
//        
//    if (bar.showMenu)   
//        bar.showMenu(evt);  
//    
//    eventCancelBubble(evt);
//    return false;
//}
//
//function MenuBarWidget_getContextMenu()
//{
//    return this.menu;
//}
//function MenuBarWidget_showMenu(evt)
//// Show the menu
//// e    : event
//// return void
//{
//    var o=this;
//    
//    if (o.showContextMenuAllowed==false)
//        return
//    
//    var oldIndex=o.getShownMenuIndex();
//    if (oldIndex!=null)
//    {
//        o.items[oldIndex].changeState(_menuBarNormal);
//    }   
//    
//    evt=getEvent(evt);
//
//    o.menu.show(true,(eventGetX(evt)+winScrollX()),(eventGetY(evt)+winScrollY()));  
//}
//
//function MenuBarWidget_setShowContextMenuAllowed(b)
//{
//    this.showContextMenuAllowed=b
//}
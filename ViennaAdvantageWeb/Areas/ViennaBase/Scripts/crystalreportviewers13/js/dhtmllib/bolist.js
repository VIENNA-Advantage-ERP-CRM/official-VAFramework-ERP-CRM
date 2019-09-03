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

File: list.js
Custom BusObj List control
=============================================================
*/

_horiBOList=0
_vertBOList=1
_wrapBOList=2
_andOrBOList=3


_lstNobord="3px solid #FFFFFF"
_lstBord="3px solid #C00000"
_lstNullbord="0px solid #FFFFFF"

_dropFeebackWidget=null
_dropParentWidget=null
_dropPosition=-1

// ================================================================================
// ================================================================================
//
// OBJECT newBOListContainerWidget (Constructor)
//
// Base class for a nested filter list (with OR or AND branches)
//
// ================================================================================
// ================================================================================

function newBOListContainerWidget(id,w,h,image,layout,changeCB,dblClickCB,moveCB,deleteCB,noText,focusCB,addQuickFilterCB,help)
// CONSTRUCTOR
// id         [String] the id for DHTML processing
// w          [int] widget width, including borders
// h          [int] widget height, including borders
// changeCB   [function] callback when selecting items
// dblClickCB [function] callback when double clicking on items
// moveCB     [function] callback a node is moved from the container buttons up & down
//                       parameters elem,node,idx. return false if all move performed by function
// noText     [boolean] display buttons with no text if true
// deleteCB   [function] callback when delete key pressed
// focusCB   [function] callback when key pressed on the widget
// Return     [AndOrBOListWidget] the instance
{
	var o=newWidget(id),v=layout==_vertBOList,l=null
	
	o.w=w
	o.h=h
	o.andOrList=newBOListWidget("andOrList_"+id,Math.max(0,w-18),h,image,layout,changeCB,dblClickCB,moveCB,deleteCB,focusCB,help)
	
	o.noText=noText?noText:false
	
	l=(v?_lstMoveUpLab:_lstMoveLeftLab)
	o.up=newButtonWidget("andOrList_up_"+id,noText?null:l,AndOrContainerWidget_upDownCb,null,null,noText?l:null,null,null,_skin+'buttonIcons.gif',16,16,0,v?64:32,null,16,v?64:32)
	l=(v?_lstMoveDownLab:_lstMoveRightLab)
	o.down=newButtonWidget("andOrList_down_"+id,noText?null:l,AndOrContainerWidget_upDownCb,null,null,noText?l:null,null,null,_skin+'buttonIcons.gif',16,16,0,v?80:48,null,16,v?80:48)
	o.up.lst=o
	o.up.isUp=true
	o.down.lst=o
	o.down.isUp=false
	o.down.extraStyle=o.up.extraStyle="margin-top:2px;"
		
	if(addQuickFilterCB)
	{
		l=_lstQuickFilterLab
		o.quickFilter=newButtonWidget("andOrList_quickFilter_"+id,noText?null:l,addQuickFilterCB,null,null,noText?l:null,null,null,_skin+'buttonIcons.gif',16,16,0,160,null,16,160)
		o.quickFilter.lst=o
		o.quickFilter.extraStyle="margin-top:2px;"
	}	
	o.getList=AndOrContainerWidget_getList
	o.getHTML=AndOrContainerWidget_getHTML
	o.oldResize=o.resize
	o.resize=AndOrContainerWidget_resize
	o.moveElem=AndOrContainerWidget_moveElem
	o.prvInit=o.init
	o.init=AndOrContainerWidget_init
	o.chgLayout=BOListContainerWidget_chgLayout
	return o	
}

//
// we can change the layout of a container even after the HTML is written
//
function BOListContainerWidget_chgLayout(layout)
{
	var o=this, v=(layout==_vertBOList),noTxt=o.noText
	if (o.layer==null) return
	l=(v?_lstMoveUpLab:_lstMoveLeftLab)
	if (!noTxt)	o.up.setText(l)	
	o.up.changeImg(0,v?64:32,16,v?64:32,null,l)
	
	l=(v?_lstMoveDownLab:_lstMoveRightLab)
	if (!noTxt) o.down.setText(l)	
	o.down.changeImg(0,v?80:48,16,v?80:48,null,l)
}

// ================================================================================
// ================================================================================
//
// OBJECT newBOListWidget (Constructor)
//
// Base class for a nested filter list (with OR or AND branches)
//
// ================================================================================
// ================================================================================

function newBOListWidget(id,w,h,image,layout,changeCB,dblClickCB,moveCB,deleteCB,focusCB,help)
// CONSTRUCTOR
// id         [String] the id for DHTML processing
// w          [int] widget width, including borders
// h          [int] widget height, including borders
// image [String] the combined image URL
// layout [_horiBOList | _vertBOList | _wrapBOList] the list layout
// changeCB   [function] callback when selecting items
// dblClickCB [function] callback when double clicking on items
// moveCB     [function] callback a node is moved from the container buttons up & down
// deleteCB   [function] callback when delete key pressed
// focusCB   [function] callback when key pressed on the widget
// Return     [BOListWidget] the instance
{
	var o=newScrolledZoneWidget(id,2,4,w,h)
	
	o.image=image
	o.layout=layout
	o.autofit=true
	o.items=new Array
	o.counter=0
	o.list=o
	o.help=help
	o.helpZone=newWidget('boListCont_help_'+o.id)

	o.changeCB=changeCB
	o.dblClickCB=dblClickCB
	o.moveCB=moveCB
	o.deleteCB=deleteCB
	o.focusCB=focusCB
	
	o.getHTML=BOListWidget_getHTML
	o.add=BOListWidget_add
	o.genericAdd=BOListWidget_genericAdd

	o.setDragDrop=AndOrBOListWidget_setDragDrop
	
	o.unselect=AndOrBOListWidget_unselect
	o.getSelection=BOListWidget_getSelection
	o.getChildIndex=AndOrNodeWidget_getChildIndex
	
	o.select=BOListWidget_select
	o.selectByVal=BOListWidget_selectByVal
	o.remove=BOListWidget_remove
	o.getElement=BOListWidget_getElement
	o.getElementByValue=BOListWidget_getElementByValue
	o.oldAONodeInit=o.init
	o.init=BOListWidget_init
	o.removeChild=BOListWidget_removeChild
	o.getLastItem=AndOrNodeWidget_getLastItem
	o.getLength=AndOrNodeWidget_getLength
	o.getItem=AndOrNodeWidget_getItem

	o.selection=null
	o.htmlWritten=false
	
	o.move=BOListWidget_move
	o.getContentTag=BOListWidget_getContentTag
	
	o.showOrhideHelp=BOListWidget_showOrhideHelp
	o.oldresize=o.resize
	o.resize=BOListWidget_resize

	switch(layout)
	{
		case _vertBOList:
			o.lBegin='<tr><td>'
			o.lEnd='</td></tr>'
			o.iBegin=''
			o.iEnd=''
			o.createCell=false
			break
		default:
			o.lBegin='<tr>'
			o.lEnd='</tr>'
			o.iBegin='<td>'
			o.iEnd='</td>'
			o.createCell=true
			break
	}
	
	return o
}

// ================================================================================

function BOListWidget_focusCB()
{
	var o=getWidget(this)
	if (o.focusCB)
		o.focusCB()
}

// ================================================================================

function BOListWidget_getHTML(noWriteParent)
// Returns : [String] the HTML
{
	var o=this,items=o.items,len=items.length,a=new Array(len+3),j=0
	
	o.htmlWritten=true

	a[j++]=(noWriteParent?'':o.beginHTML())+'<table id="boListCont_'+o.id+'" cellpadding="0" cellspacing="0" border="0"><tbody>'+o.lBegin
		
	//a[j++]=o.iBegin+'<div id="boListCont_help_'+o.id+'" class="bgzone" style="overflow:visible" ><i>'+(o.help?o.help:'')+'</i></div>'+o.iEnd
	
	for (var i in items) a[j++]=o.iBegin+items[i].getHTML()+o.iEnd
			
	a[j++]=o.lEnd+'</tbody></table>'//+(noWriteParent?'':o.endHTML())
	//regression: do not put the help in the table because we use insertBefore(idx)
	a[j++]='<div id="boListCont_help_'+o.id+'" class="bgzone" style="overflow:visible" ><i>'+(o.help?o.help:'')+'</i></div>'
	a[j++]=(noWriteParent?'':o.endHTML())
	return a.join("")
}

// ================================================================================

function BOListWidget_init()
// Init the widget layers
// Return [void]
{
	var o=this,items=o.items
	o.oldAONodeInit()
	if (!o.htmlWritten)
	{
		o.layer.innerHTML=o.getHTML(true)
		o.oldAONodeInit()
	}
	
	if (o.layer._counter !=null)
		o.counter=o.layer._counter
	else
		o.layer._counter=o.counter
		
	o.layer.onmousedown=BOListWidget_focusCB
	
	for (var i in items)
		items[i].init()
	
	var dd=o.dragDrop
	if (_ie&&dd)
	{
		dd.attachCallbacks(o.layer)
		o.layer.BODDType="list"
	}

	o.helpZone.init();
	o.showOrhideHelp();
}

function BOListWidget_showOrhideHelp()
{
	var o=this
	if(o.help)
	{
		var len = o.items.length
		if(len == 0)
			o.helpZone.setDisplay(true)
		else
			o.helpZone.setDisplay(false)
			
		//resize if necessary
		BOListWidget_resizeHelp(o,o.getWidth())
	}
}

function BOListWidget_resizeHelp(list,w)
{
	if(list.help && list.helpZone.isDisplayed())
	{
		list.helpZone.resize(Math.max(0,w-20),null)
	}
	
}
// ================================================================================

function BOListWidget_getSelection()
// Get the current delected node or item
// Return and object with index & value, or null if no selection
{
	var o=this,sel=o.selection
	if (sel)
	{
		var id=sel.id,index=-1,items=o.items;len=items.length
		for (var i=0;i<len;i++)
		{
			if (items[i].id==id)
			{
				index=i
				break
			}
		}
	
		ret=new Object;
		ret.index=i
		ret.value=sel.value
		
		return ret
	}
	else
		return null
}

function BOListWidget_getContentTag()
{
	var o=this
	var parNode=o.layer.childNodes[0].childNodes[0].childNodes[0]
	if (!o.createCell)
		parNode=parNode.childNodes[0]
	return parNode
}

// ================================================================================

function BOListWidget_genericAdd(nodeOrItem,idx)
// PRIVATE Adds a node or an item
// nodeOrItem   [AndOrNodeWidget or AndOrItemWidget]
// Return [AndOrNodeWidget or AndOrItemWidget]
{
	nodeOrItem.fullW=true

	var o=this, list=o.list
	idx=arrayAdd(o,"items",nodeOrItem,idx)
	var items=o.items

	nodeOrItem.par=o
	nodeOrItem.list=o.list
	
	if (o.layer)
	{
		var parNode=o.getContentTag()
		var s=nodeOrItem.getHTML()

		if (o.createCell)
		{
			var theCell=_curDoc.createElement('td')
			theCell.innerHTML=s
			
			if (idx==(items.length-1))
				parNode.appendChild(theCell)
			else
				parNode.insertBefore(theCell,parNode.childNodes[idx])
		}
		else
		{
			if ((idx==-1)||(idx==(items.length-1)))
				append(parNode,s)
			else
				insBefore(parNode.childNodes[idx],s)
		}

		nodeOrItem.init()
	}	
	o.showOrhideHelp();
	
	return nodeOrItem
}

// ================================================================================

function BOListWidget_add(text,imgIndex,value,tooltip,idx)
// Adds an item
// text     [String] item text
// imgIndex [int] image index in the combined image
// value    [String] item value - must be unique in the list
// tooltip  [String] tooltip
// Returns : [void]
{
	var o=this,last=o.getLastItem()
	var ret=o.genericAdd(newAndOrNodeItem(o.id+'_item-'+(o.counter++),text,imgIndex,value,tooltip),idx)
	var last2=o.getLastItem()
	
	if (o.layer)
	{
		if (last&&(last2.id!=last.id))
		{
			if (o.layout==_vertBOList)
				last.css.borderBottom=_lstNullbord
			else
				last.css.borderRight=_lstNullbord
		}

		o.layer._counter=o.counter
	}
	
	return ret
}

// ================================================================================

function BOListWidget_select(index)
// Selects and item from its index
// index [int] : the index
// Returns : [void]
{
	var items=this.items,len=items.length
	index=index==-1?len-1:index
	if ((index>=0)&&(index<len))
		items[index].select(true)
}

// ================================================================================

function BOListWidget_getElementByValue(value)
// Get an item from its value
// value [String] : the value
// Return [AndOrNodeItem] the item (null if not found)
{
	var o=this,items=o.items,len=items.length
	for (var i=0;i<len;i++)
	{
		if (items[i].value==value)
			return items[i]
	}
	return null
}

// ================================================================================

function BOListWidget_getElement(idx)
// Get an item from its index
// idx [int] : the index
// Return [AndOrNodeItem] the item (null if not found)
{
	var it=this.items,len=it.length
	return ((idx>=0)&&(idx<len))?it[idx]:null
}

// ================================================================================

function BOListWidget_remove(idx)
// Remove an element from its index
// idx [int] element index. if null, remove ALL elements
// Returns : [void]
{
	var o=this
	if (idx==null)
	{
		while(o.items.length>0)
			o.remove(0)
	}
	else
	{
		var it=o.getElement(idx)
		if (it)
			it.remove()
	}
}

// ================================================================================

function BOListWidget_selectByVal(value)
// Selects an item from its value
// value [String] : the value
// Return [void]
{
	var item=this.getElementByValue(value)
	if (item)
		item.select(true)
}

// ================================================================================

function BOListWidget_removeChild(nodeOrItem)
// Removes a child
// nodeOrItem [AndOrNodeItem] : the item to remove
// Return [void]
{
	var o=this,items=o.items,len=items.length,list=o.list
	
	for (var i=0;i<len;i++)
	{
		var item=items[i]
		if (item.id==nodeOrItem.id)
		{
			var l=item.layer

			if (item.selected)
				item.select(false)
			
			if (l)
			{
				var p=l.parentNode
				
				if (o.createCell)
				{
					l=p
					p=p.parentNode
				}

				p.removeChild(l)
			}
			
			arrayRemove(o,"items",i)
			items=o.items
			
			var newSel=Math.min(i,items.length-1)
			if (newSel>=0)
			{
				items[newSel].select(true)
				var last=o.getLastItem()
				if (o.layer)
				{
					if (last&&!last.isNode())
					{
						var ly=o.list.layout
						if (ly==_vertBOList)
						{
							last.css.borderBottom=_lstNobord
						}
						else
							last.css.borderRight=_lstNobord
					}
				}
			}
			else o.select(true)
			
			break
		}
	}	
	o.showOrhideHelp();
}

// ================================================================================

function BOListWidget_move(e,node,destIdx)
{

	var o=this
	e.select(false)
	var n=o.add(e.text,e.imgIndex,e.value,e.tooltip,destIdx)
	e.remove()
	n.select(true)
}

// ================================================================================
function BOListWidget_resize(w,h)
{
	var o=this	
	BOListWidget_resizeHelp(o,w)	
	o.oldresize(w,h)	
}

// ================================================================================
// ================================================================================
//
// OBJECT newAndOrContainerWidget (Constructor)
//
// Base class for a nested filter list (with OR or AND branches)
//
// ================================================================================
// ================================================================================

function newAndOrContainerWidget(id,w,h,image,changeCB,dblClickCB,andOrCB,moveCB,newNodeCB,deleteCB,noText,help)
// CONSTRUCTOR
// id         [String] the id for DHTML processing
// w          [int] widget width, including borders
// h          [int] widget height, including borders
// changeCB   [function] callback when selecting items
// dblClickCB [function] callback when double clicking on items
// andOrCB    [function] callback a node type is changed
// moveCB     [function] callback a node is moved from the container buttons up & down
// newNodeCB  [function] callback a new node is created from the container button
// deleteCB   [function] callback when delete key pressed
// noText     [boolean] display buttons with no text if true
// Return     [AndOrBOListWidget] the instance
{
	var o=newWidget(id),l
	
	o.w=w
	o.h=h
	o.andOrList=newAndOrBOListWidget("andOrList_"+id,10,h,image,changeCB,dblClickCB,andOrCB,moveCB,newNodeCB,deleteCB,help)
	l=_lstMoveUpLab
	o.up=newButtonWidget("andOrList_up_"+id,noText?null:l,AndOrContainerWidget_upDownCb,null,null,noText?l:null,null,null,_skin+'buttonIcons.gif',16,16,0,64,null,16,64)
	l=_lstMoveDownLab
	o.down=newButtonWidget("andOrList_down_"+id,noText?null:l,AndOrContainerWidget_upDownCb,null,null,noText?l:null,null,null,_skin+'buttonIcons.gif',16,16,0,80,null,16,80)
	l=_lstNewNodeLab
	o.newNode=newButtonWidget("andOrList_newNode_"+id,noText?null:l,AndOrContainerWidget_nodeCb,null,null,noText?l:null,null,null,_skin+'buttonIcons.gif',16,16,0,96,null,16,96)
	
	o.down.extraStyle="margin-top:2px;"
	o.newNode.extraStyle="margin-top:2px;"
	
	o.up.lst=o
	o.up.isUp=true
	o.down.lst=o
	o.down.isUp=false

	o.newNode.lst=o
	
	o.getList=AndOrContainerWidget_getList
	o.getHTML=AndOrContainerWidget_getHTML
	
	o.oldResize=o.resize
	o.resize=AndOrContainerWidget_resize
	o.moveElem=AndOrContainerWidget_moveElem
	o.canBeMoved=AndOrContainerWidget_canBeMoved
	o.prvInit=o.init
	o.init=AndOrContainerWidget_init
	
	return o	
}

// ================================================================================

function AndOrContainerWidget_init()
{
	var o=this
	o.rightZone=getLayer('rightZone_'+o.id)
	o.prvInit()
	o.up.init()
	o.down.init()
	if(o.quickFilter)
		o.quickFilter.init()
	if (o.newNode)
		o.newNode.init()
	o.andOrList.init()
	o.resize(o.w,o.h)
}

// ================================================================================

function AndOrContainerWidget_getHTML()
// Returns the widget HTML
{
	var o=this,list=o.andOrList
	
	return '<table id="'+o.id+'" style="overflow:hidden;width:'+o.w+'px;height='+o.h+'px" cellpadding="0" cellspacing="0" border="0"><tbody><tr>'+
		'<td  style="padding-right:5px" width="100%">'+list.getHTML()+'</td>'+
		'<td id="rightZone_'+o.id+'" align="center" valign="top">'+o.up.getHTML()+o.down.getHTML()+(o.newNode?o.newNode.getHTML():'')+(o.quickFilter?o.quickFilter.getHTML():'')+'</td>'+
		'</tr></tbody></table>'
}

// ================================================================================

function AndOrContainerWidget_getList()
// get the included AndOrBOListWidget
// return [AndOrBOListWidget]
{
	return this.andOrList
}

// ================================================================================

function AndOrContainerWidget_resize(w,h)
// resize the container widget
// w [int] : width
// h [int] : height
// return [void]
{
	var o=this
	o.oldResize(w,h)
	var butnW=o.rightZone.offsetWidth
	o.andOrList.resize(w!=null?Math.max(0,w-butnW-5):null,h)		
	o.up.resize(butnW)
	o.down.resize(butnW)
	
	if(o.quickFilter)
		o.quickFilter.resize(butnW)
	if (o.newNode)
		o.newNode.resize(butnW)
	
	var d=o.layer.display!="none"
	
	if (d&_moz&&!_saf)
		o.setDisplay(false)
	o.oldResize(w,h)
	if (d&_moz&&!_saf)
		o.setDisplay(true)

}

// ================================================================================

function AndOrContainerWidget_upDownCb()
// DHTML Callback. Called when up or down arrow is pressed
// elem [DHTML Element] : the element
// up [boolean] : if true move up; else move down
// return [void]
{
	var o=this.lst,list=o.andOrList,sel=list.selection

	if (sel)
			o.moveElem(sel,this.isUp)
}

// ================================================================================

function AndOrContainerWidget_nodeCb()
// DHTML Callback. Called when node button is pressed
// elem [DHTML Element] : the element
// return [void]
{
	var o=this.lst.andOrList,sel=o.getSelection()
	
	if (sel)
	{
		var n=null
		if (sel.isNode())
			n=sel.addNode(!sel.isAnd,"")
		else
			n=sel.par.addNode(!sel.par.isAnd,"",sel.getIndexInParent()+1)
		
		n.select(true)
		if (o.newNodeCB)
			o.newNodeCB()
	}
}

// ================================================================================

function AndOrContainerWidget_canBeMoved(elem,up)
{
	
	var o=this,node=elem.par
	if (node)
	{
		var idx=node.getChildIndex(elem),itemIdx=idx
		if (idx!=-1)
		{
			if (up&&(idx>0))							
				return true;			
			else if (!up&&(idx<(node.items.length-1)))		
				return true;		
			else
			{
				idx+=(up?-1:2)
				if ((idx<0)||(idx>node.items.length))
				{						
					if (node.par)					
						return true;							
				}
			}			
		}
	}
	return false;
}
// ================================================================================
function AndOrContainerWidget_moveElem(elem,up)
// Move down or up an item, in depth before
// elem [AndOrNodeWidget] or [AndOrItemWidget] : the item to move
// up [boolean] : if true move up; else move down
{
	var o=this,node=elem.par
	if (node)
	{
		var idx=node.getChildIndex(elem),itemIdx=idx
		if (idx!=-1)
		{
			if (up&&(idx>0)&&(node.items[idx-1].isNode()))
			{
				node=node.items[idx-1]
				idx=node.items.length
			}
			else if (!up&&(idx<(node.items.length-1))&&(node.items[idx+1].isNode()))
			{
				node=node.items[idx+1]
				idx=0
			}
			else
			{
				idx+=(up?-1:2)
				if ((idx<0)||(idx>node.items.length))
				{
					var oldNode=node
					node=node.par
					if (node)
						idx=node.getChildIndex(oldNode)+(up?0:1)
				}
			}

			if (node)
			{
				var list=o.getList(),doMove=true

				if (list.moveCB)
					doMove=list.moveCB(elem,node,idx)

				if ((doMove==null)||doMove)
					list.move(elem,node,idx)
			}
		}
	}
}

// ================================================================================
// ================================================================================
//
// OBJECT newAndOrBOListWidget (Constructor)
//
// Base class for a nested filter list (with OR or AND branches)
//
// ================================================================================
// ================================================================================

function newAndOrBOListWidget(id,w,h,image,changeCB,dblClickCB,andOrCB,moveCB,newNodeCB,deleteCB,help)
// CONSTRUCTOR
// id         [String] the id for DHTML processing
// w          [int] widget width, including borders
// h          [int] widget height, including borders
// changeCB   [function] callback when selecting items
// dblClickCB [function] callback when double clicking on items
// andOrCB    [function] callback a node type is changed
// moveCB     [function] callback a node is moved from the container buttons up & down
// newNodeCB  [function] callback a new node is created from the container button
// deleteCB   [function] callback when delete key pressed
// Return     [AndOrBOListWidget] the instance
{
	var o=newScrolledZoneWidget(id,2,4,w,h)
	
	o.image=image
	o.changeCB=changeCB
	o.dblClickCB=dblClickCB
	o.andOrCB=andOrCB
	o.moveCB=moveCB
	o.newNodeCB=newNodeCB
	o.deleteCB=deleteCB
	o.layout=_andOrBOList
	
	o.root=newAndOrNodeWidget(id+"_rootOperator",true,"")	
	//o.root.isDisplayed=true
	o.root.list=o
	o.selection=o.root
	o.root.selected=true
	o.help=help
	o.helpZone=newWidget("andOrCont_help_"+id)

	o.getHTML=AndOrBOListWidget_getHTML
	o.getRoot=AndOrBOListWidget_getRoot
	
	o.setDragDrop=AndOrBOListWidget_setDragDrop
	o.unselect=AndOrBOListWidget_unselect
	o.getSelection=AndOrBOListWidget_getSelection
	o.select=AndOrBOListWidget_select
	o.getElementByValue=AndOrBOListWidget_getElementByValue
	o.showOrhideRoot=AndOrBOListWidget_showOrhideRoot	
	o.oldAOListInit=o.init
	o.init=AndOrBOListWidget_init
	o.move=AndOrBOListWidget_move
	o.htmlWritten=false
	
	o.oldresize=o.resize
	o.resize=AndOrBOListWidget_resize
	
	return o
}

// ================================================================================

function AndOrBOListWidget_move(e,destNode,destIdx)
// move an item
// e [AndOrNodeWidget] or [AndOrItemWidget] : the item to move
// destNode [AndOrNodeWidget] the destination node
// destIdx [int] the index into the destination node
// Return [void]
{
	if (destNode)
	{
		var n=null
		e.select(false)
		if (e.isNode())
			n=destNode.addCopyNode(e,destIdx)
		else
		{
			if(e.isAdvFilter)
				n=destNode.addAdvFilterItem(e.text,e.imgIndex,e.value,e.tooltip,destIdx)
			else
				n=destNode.addItem(e.text,e.imgIndex,e.value,e.tooltip,destIdx)
		}	
		e.remove()
		n.select(true)
	}
}

// ================================================================================

function AndOrBOListWidget_getSelection()
// Get the current selected node or item
// Return [AndOrNodeWidget] or [AndOrItemWidget] or null if no selection
{
	return this.selection
}

// ================================================================================

function AndOrBOListWidget_select(itemOrNode)
// Select an item or a node
// itemOrNode [AndOrNodeWidget] or [AndOrItemWidget]
{
	itemOrNode.select(true)
}

// ================================================================================

function AndOrBOListWidget_getElementByValue(value)
// Get an element from its value
// Return [AndOrNodeWidget or  [AndOrItemWidget] or null if not found
{
	return this.root.getElementByValue(value)
}

// ================================================================================

function AndOrBOListWidget_init()
// Init the widget
{
	var o=this
	o.oldAOListInit()
	
	if (!o.htmlWritten)
	{
		o.layer.innerHTML=o.getHTML(true)
		o.oldAOListInit()
	}

	
	o.root.init()
	o.helpZone.init();
	o.showOrhideRoot()
	
	var dd=o.dragDrop
	if (_ie&&dd)
	{
		dd.attachCallbacks(o.layer)
		o.layer.BODDType="andorlist"
	}
}

// ================================================================================

function AndOrBOListWidget_unselect()
// Unselect
{
	var o=this
	if (o.selection)
		o.selection.select(false)
}

// ================================================================================

function AndOrBOListWidget_getHTML(noWriteParent)
// Returns : [String] the HTML
{
	var o=this
	o.htmlWritten=true
	
	var a=new Array(3), j=0, len = o.root.items.length
	a[j++]=(noWriteParent?'':o.beginHTML())
	a[j++]='<div id="andOrCont_'+o.id+'">'+'<div id="andOrCont_help_'+o.id+'" class="bgzone" style="overflow:visible"><i>'+(o.help?o.help:'')+'</i></div>'+o.root.getHTML()+'</div>'
	a[j++]=(noWriteParent?'':o.endHTML())	
	
	return a.join("")
	//return (noWriteParent?'':o.beginHTML())+'<div id="andOrCont_'+o.id+'">'+o.root.getHTML()+'</div>'+(noWriteParent?'':o.endHTML())
}

// ================================================================================

function AndOrBOListWidget_getRoot()
// Return [AndOrBOOperatorWidget] the root widget
{
	return this.root
}

// ================================================================================
function AndOrBOListWidget_showOrhideHelp(list)
{
	if(list.help)
	{
		var len = list.root.items.length
		if(len == 0)
			list.helpZone.setDisplay(true)
		else
			list.helpZone.setDisplay(false)
	
		//resize 
		AndOrBOListWidget_resizeHelp(list,list.getWidth())
	}
}

function AndOrBOListWidget_resizeHelp(list,w)
{
	if(list.help && list.helpZone.isDisplayed())
	{		
		list.helpZone.resize(Math.max(0,w-20),null)
	}
}

// ================================================================================
function AndOrBOListWidget_showOrhideRoot()
// hide/show the root widget
{	
	var o=this, root= o.root, len = root.items.length, show = true
	
	if (o.layer)
	{
		if((len == 0) || ((len==1) && (!root.items[0].isNode()) )) show=false
		
		root.operLayer.parentNode.style.display=show?"":"none"		
		
		//show or hide the help zone
		AndOrBOListWidget_showOrhideHelp(o)	
	}
}

// ================================================================================

function AndOrBOListWidget_resize(w,h)
{
	var o=this
	AndOrBOListWidget_resizeHelp(o,w)
	o.oldresize(w,h);	
}

// ================================================================================

function AndOrBOListWidget_setDragDrop(dragCB,acceptDropCB,dropCB)
// Add drag & drop handlers to the widget
// dragCB       (source)                       : triggered when beginning D&D
// acceptDropCB (source, target, ctrl, shift)  : return boolean if D&D is accepted
// dropCB       (source, target, ctrl, shift)  : triggered when ending D&D
//
// when acceptDropCB & dropCB is called, target has some additional fields
//
//  * target.dropWidget : the container (list or node) where inserting
//  * target.dropIndex  : the index to insert (-1 is at the end)
{
	var o=this
	o.dragCB=dragCB
	o.acceptDropCB=acceptDropCB
	o.dropCB=dropCB

	o.dragDrop=newDragDropData(o,EBLWidget_dragStartCB,EBLWidget_dragCB,EBLWidget_dragEndCB,EBLWidget_acceptDropCB,EBLWidget_leaveDropCB,EBLWidget_dropCB)
}
// ================================================================================

function EBLWidget_getDropInfo(layer)
// Get the widget that must display drop feedback
// Fill the global _dropFeebackWidget
// and _dropPosition
{
	var o=getWidget(layer)
	switch (layer.BODDType)
	{
		case "root":
		case "nodebg":
		case "list":
		case "andorlist":
			_dropParentWidget=o.items?o:o.root
			_dropFeebackWidget=_dropParentWidget.getLastItem()
			
			if ((layer.BODDType=="nodebg")&&(_dropFeebackWidget==null))
				_dropFeebackWidget=_dropParentWidget
			
			_dropPosition=-1
			break
		
		case "node":
		case "item":
			_dropParentWidget=o.par
			_dropFeebackWidget=o
			_dropPosition=o.getIndexInParent()
			break
	}
	return _dropFeebackWidget
}

// ================================================================================

function EBLWidget_dragStartCB(source,layer)
// Triggered when starting drag
{
	var o=getWidget(layer)
	_curWin.event.cancelBubble=true

	if (o)
	{
		var list=o.list
		source.dragCB(source)
		newTooltipWidget().show(true,o.text?o.text:(o.isAnd?_lstAndLabel:_lstOrLabel),o.text?list.image:null,16,16,0,o.imgIndex*16)
	}
}

// ================================================================================

function EBLWidget_dragCB(source,layer,shift)
// Triggered while dragging
{
	newTooltipWidget().setPos(shift);
}

// ================================================================================

function EBLWidget_dragEndCB(source)
// Triggered when ending drag & drop
{
	newTooltipWidget().show(false)
}

// ================================================================================

function EBLWidget_acceptDropCB(source,target,ctrl,shift,layer,enter)
{
	var o=EBLWidget_getDropInfo(layer)

	target.dropWidget=_dropParentWidget
	target.dropIndex=_dropPosition
	

	var accepted=target.acceptDropCB(source,target,ctrl,shift)
	
	if (accepted&&o)
	{
		if (source&&(source.id==target.id))
		{
			var s=source.selection
			if ((s&&s.isNode()&&(o.isChildOf(s)))||
			   (shift&&s&&o.isNode()&&(s.isChildOf(o)))||
			   (!ctrl&&s&&(s.id==o.id)))
			{
				o.setInsertFeedback(false,shift)
				return false
			}
			
			if (!shift&&!ctrl&&o.par&&(o.par==s.par))
			{
				var srcIdx=s.getIndexInParent()
				if (srcIdx==(_dropPosition-1))
				{
					o.setInsertFeedback(false,shift)
					return false
				}
			}
			
		}
		o.setInsertFeedback(true,shift)
	}
	else if(o)
		o.setInsertFeedback(false,shift)
	return accepted;
}

// ================================================================================

function EBLWidget_leaveDropCB(source,target,ctrl,shift,layer)
{
	var o=EBLWidget_getDropInfo(layer)
	if (o)
		o.setInsertFeedback(false)
}

// ================================================================================

function EBLWidget_dropCB(source,target,ctrl,shift,layer)
// Drop function
{
	newTooltipWidget().show(false);
	var o=EBLWidget_getDropInfo(layer)
	if (o)
		o.setInsertFeedback(false)

	EBLWidget_leaveDropCB(source,target,ctrl,shift,layer)

	target.dropWidget=_dropParentWidget
	target.dropIndex=_dropPosition
	target.dropCB(source,target,ctrl,shift)
}

// ================================================================================
// ================================================================================
//
// OBJECT newAndOrNodeWidget (Constructor)
//
// Base class for a filter list OR or AND node
//
// ================================================================================
// ================================================================================

function newAndOrNodeWidget(id,isAnd,value)
// CONSTRUCTOR
// id     [String] the id for DHTML processing
// isAnd  [boolean] if true, is a AND node; else is a OR node
// Return [AndOrNodeWidget] the instance
{
	var o=newWidget(id)
	o.isAnd=isAnd
	o.items=new Array
	o.value=value
	o.par=null
	o.list=null
	
	o.counter=0
	o.operLayer=null
	
	o.getHTML=AndOrNodeWidget_getHTML
	o.getInnerHTML=AndOrNodeWidget_getInnerHTML
	o.getDynamicHTML=AndOrNodeWidget_getDynamicHTML
	o.addNode=AndOrNodeWidget_addNode
	o.addCopyNode=AndOrNodeWidget_addCopyNode
	o.addItem=AndOrNodeWidget_addItem
	o.addAdvFilterItem=AndOrNodeWidget_AdvFilterItem
	o.select=AndOrNodeWidget_select
	o.removeChild=AndOrNodeWidget_removeChild
	o.remove=AndOrNodeWidget_remove
	o.isNode=AndOrNodeWidget_isNode
	o.getElementByValue=AndOrNodeWidget_getElementByValue
	o.setIsAnd=AndOrNodeWidget_setIsAnd
	o.setInsertFeedback=AndOrNodeItem_setInsertFeedback
	o.getLastItem=AndOrNodeWidget_getLastItem
	o.getLength=AndOrNodeWidget_getLength

	o.getParentNode=AndOrNodeWidget_getParentNode	
	o.getChildIndex=AndOrNodeWidget_getChildIndex
	o.getIndexInParent=AndOrNodeWidget_getIndexInParent
	o.getItem=AndOrNodeWidget_getItem

	o.genericAdd=AndOrNodeWidget_genericAdd
	o.oldAONodeInit=o.init
	o.init=AndOrNodeWidget_init
	o.isChildOf=AndOrNodeItem_isChildOf

	o.andLabel=_lstAndLabel
	o.orLabel=_lstOrLabel

	return o
}

// ================================================================================

function AndOrNodeWidget_getParentNode()
// Return [AndOrNodeWidget] the parent node (null if root)
{
	return this.par
}

// ================================================================================

function AndOrNodeWidget_getLastItem()
// Return [AndOrNodeWidget] the last node (null if not)
{
	var it=this.items,len=it.length
	return len>0?it[len-1]:null
}

// ================================================================================

function AndOrNodeWidget_getChildIndex(child)
// Return [int] the child index (position between 0 and length-1).
// if child is not in the node, return -1
{
	var items=this.items,len=items.length
	for (var i=0;i<len;i++)
		if (items[i].id==child.id)
			return i
	return -1
}

// ================================================================================

function AndOrNodeWidget_getIndexInParent()
// return [int] index in parent node
{
	var o=this,par=o.par
	return par?par.getChildIndex(o):-1
}

// ================================================================================

function AndOrNodeWidget_getElementByValue(value)
{
	var o=this,items=o.items
	if (o.value==value)
		return o
		
	for (var i in items)
	{
		var item=items[i]
		if (item.isNode())
		{
			var elem=item.getElementByValue(value)
			if (elem!=null)
				return elem
		}
		else if (item.value==value)
			return item
	}
	return null
}

// ================================================================================

function AndOrNodeWidget_isNode()
// Test if it's a node
{
	return true
}

// ================================================================================

function AndOrNodeWidget_getItem(idx)
// Test if it's a node
{
	var it=this.items,len=it.length
	if (idx==-1) idx=len-1
	return ((idx>=0)&&(idx<len))?it[idx]:null
}

// ================================================================================

function AndOrNodeWidget_getLength()
{
	return this.items.length
}

// ================================================================================

function AndOrNodeWidget_getHTML()
// Get the widget HTML
// Returns : [String] the HTML
{
	var o=this
	
	return  '<table id="'+o.id+'" style="border-top:'+_lstNobord+';" cellpadding="0" cellspacing="0" border="0">' + 
		o.getInnerHTML() +
		'</table>'
}

// ================================================================================

function AndOrNodeWidget_getInnerHTML()
// Get the widget HTML
// Returns : [String] the HTML
{
	var o=this,items=o.items,len=items.length,a=new Array(len+2),j=0
	var lab=o.isAnd?o.andLabel:o.orLabel
		
	//lab=elasticZone(lab,_skin+'bolist.gif',3,26,o.selected?234:156,'oper_'+o.id,'treeNormal',null,0,0,'width="30" align="center"','operTxt_'+o.id)+getSpace(40,1)
	lab=elasticZone(lab,_skin+'bolist.gif',3,28,o.selected?252:168,'oper_'+o.id,'treeNormal',null,0,0,'width="30" align="center"','operTxt_'+o.id)+getSpace(40,1)

	a[j++]='<tbody><tr>'+
		'<td class="treeNormal" style="border-right:1px solid #A0A0A0;" align="center" valign="middle">'+lab+'</td><td style="width:70px;padding-bottom:2px;padding-left:4px">'
	for (var i in items) a[j++]=items[i].getHTML()
	a[j++]='</td></tr></tbody>'
	
	return a.join("")
}

// ================================================================================

function AndOrNodeWidget_getDynamicHTML()
// Get the widget HTML
// Returns : [String] the HTML
{
	var o=this

	var theTable=_curDoc.createElement('table')
	
	theTable.id = o.id
	theTable.style.borderTop = _lstNobord
	theTable.cellPadding = 0
	theTable.cellSpacing = 0
	theTable.border = 0
	theTable.innerHTML= o.getInnerHTML()
	
	return theTable
}

// ================================================================================

function AndOrNodeWidget_init()
// Init the widget layers
// Return [void]
{
	var o=this,items=o.items
	o.oldAONodeInit()
	
	if (o.layer._counter !=null)
		o.counter=o.layer._counter
	else
		o.layer._counter=o.counter
	
	var l=o.operLayer=getLayer('oper_'+o.id)
		
	o.operLayerTxt=getLayer('operTxt_'+o.id)
	
	l.onmousedown=AndOrNodeWidget_clickCB
	l.onmouseup=AndOrNodeWidget_mouseUp
	
	if (_ie)
		l.onmousemove=AndOrNodeWidget_triggerDD
	
	l.onkeydown=AndOrNodeWidget_keyDownCB
	addDblClickCB(l,AndOrNodeWidget_dblClickCB)
	
	if (o.isDisplayed!=null)
		o.setDisplay(o.isDisplayed)
	
	var dd=o.list.dragDrop
	if (_ie&&dd)
	{
		var rl=o.layer.childNodes[0].childNodes[0].childNodes[1]
		
		dd.attachCallbacks(rl)
		dd.attachCallbacks(o.layer)
	
		o.layer.BODDType=o.par?"node":"root"
		rl.BODDType="nodebg"
	}
	
	for (var i in items)
		items[i].init()
}

// ================================================================================

function AndOrNodeWidget_clickCB(e)
{
	var o=getWidget(this),list=o.list
	o.clicked=true
	
	o.initialX=eventGetX(e)
	o.initialY=eventGetY(e)
	
	o.select(true)

	if (list.changeCB)
		list.changeCB()

	return false
}

// ================================================================================

function AndOrNodeWidget_mouseUp()
{
	var o=getWidget(this)
	o.clicked=false
}

// ================================================================================

function AndOrNodeWidget_triggerDD(e)
{
	var o=getWidget(this)

	if ((o.clicked)&&(_curWin.event.button==_leftBtn))
	{
		if (o.initialX!=null)
		{
			var x=eventGetX(e),y=eventGetY(e),threshold=3
			
			if ((x<(o.initialX-threshold))||(x>(o.initialX+threshold))||(y<(o.initialY-threshold))||(y>(o.initialY+threshold)))
			{
				this.dragDrop()
				o.clicked=false
			}
		}
	}
}

// ================================================================================

function AndOrNodeWidget_keyDownCB(e)
{
	var o=getWidget(this),list=o.list
	var key=eventGetKey(e);	
	
	if (key==46)
	{
		if (list.deleteCB)
			list.deleteCB()
	}
	if(key==13)	
	{	
		eventCancelBubble(e);
			
		if(o.selected)//already selected
		{		
			// Change operator	
			o.setIsAnd(!o.isAnd)	
			if (o.list.andOrCB)
				o.list.andOrCB()
		}
		else
		{
			o.select(true)
		}

		if (list.changeCB)
			list.changeCB()
	}
}
// ================================================================================

function AndOrNodeWidget_dblClickCB()
{
	var o=getWidget(this),list=o.list
	// Change operator	
	o.setIsAnd(!o.isAnd)
	
	if (o.list.andOrCB)
		o.list.andOrCB()
	
	return false
}

// ================================================================================

function AndOrNodeWidget_setIsAnd(isAndNode)
{
	var o=this
	// Change operator
	o.isAnd=isAndNode
	o.operLayerTxt.innerHTML=convStr(o.isAnd?o.andLabel:o.orLabel)
}

// ================================================================================

function AndOrNodeWidget_select(sel)
// Select the current item
// sel [boolean]
// Return [void]
{
	var o=this,list=o.list
	if (sel)
		list.unselect()
	list.selection=sel?o:null
	o.selected=sel
	
	if (o.layer)
	{
		var dy=6+(sel?3:0)
		elasticZoneSetImg(o.operLayer,28,dy,dy+1,dy+2)
		
		
		if(sel&&o.operLayer.focus&&(!isHidden(o.operLayer)))
			o.operLayer.focus()
	}
}

// ================================================================================

function AndOrNodeWidget_genericAdd(nodeOrItem,idx)
// PRIVATE Adds a node or an item
// nodeOrItem   [AndOrNodeWidget or AndOrItemWidget]
// Return [AndOrNodeWidget or AndOrItemWidget]
{
	var o=this, list=o.list
	idx=arrayAdd(o,"items",nodeOrItem,idx)
	var items=o.items

	nodeOrItem.par=o
	nodeOrItem.list=o.list
	
	if (o.layer)
	{
		var parNode=o.layer.childNodes[0].childNodes[0].childNodes[1]
		
		if ((idx==-1)||(idx==(items.length-1)))
			append(parNode,nodeOrItem.getHTML())
		else
		{
			if (_saf)	
				parNode.insertBefore(nodeOrItem.getDynamicHTML(),parNode.childNodes[idx])
			else
				insBefore(parNode.childNodes[idx],nodeOrItem.getHTML())
		}

		nodeOrItem.init()
	}
	
	list.showOrhideRoot()
	
	return nodeOrItem
}

// ================================================================================

function AndOrNodeWidget_addNode(isAnd,value,idx)
// Adds a node 
// Returns : [AndOrNodeWidget] the new node
{
	var o=this
	var ret= o.genericAdd(newAndOrNodeWidget(o.id+'_item-'+(o.counter++), isAnd, value),idx)
	
	if (o.layer)
		o.layer._counter=o.counter
	
	return ret
}

// ================================================================================

function AndOrNodeWidget_addCopyNode(src,idx)
{
	var o=this,n=o.addNode(src.isAnd,src.value,idx),srcItems=src.items
	
	for (var i in srcItems)
	{
		var item=srcItems[i]
		if (item.isNode())
			n.addCopyNode(item,-1)
		else
		{
			if(item.isAdvFilter)
				n.addAdvFilterItem(item.text,item.imgIndex,item.value,item.tooltip,-1)
			else
				n.addItem(item.text,item.imgIndex,item.value,item.tooltip,-1)
		}
	}
	return n
}

// ================================================================================

function AndOrNodeWidget_addItem(text,imgIndex,value,tooltip,idx)
// Adds a node 
// Returns : [void]
{
	var o=this,last=o.getLastItem()
	var ret=o.genericAdd(newAndOrNodeItem(o.id+'_item-'+(o.counter++),text,imgIndex,value,tooltip),idx)	
	var last2=o.getLastItem()
	
	if (o.layer)
	{
		if (last&&(last2.id!=last.id))
			last.css.borderBottom=_lstNullbord

		o.layer._counter=o.counter
	}
	
	return ret
}

function AndOrNodeWidget_AdvFilterItem(text,imgIndex,value,tooltip,idx)
// Adds a node 
// Returns : [void]
{
	var o=this,last=o.getLastItem()
	var ret=o.genericAdd(newAndOrFilter(o.id+'_item-'+(o.counter++),text,imgIndex,value,tooltip),idx)
	var last2=o.getLastItem()
	
	if (o.layer)
	{
		if (last&&(last2.id!=last.id))
			last.css.borderBottom=_lstNullbord

		o.layer._counter=o.counter
	}
	
	return ret
}

// ================================================================================

function AndOrNodeWidget_removeChild(nodeOrItem)
// Removes a direct child
{
	var o=this,items=o.items,len=items.length,list=o.list
	
	for (var i=0;i<len;i++)
	{
		var item=items[i]
		if (item.id==nodeOrItem.id)
		{
			var l=item.layer

			if (item.selected)
				item.select(false)
			
			if (l)
				l.parentNode.removeChild(l)
			
			arrayRemove(o,"items",i)
			items=o.items
			
			var newSel=Math.min(i,items.length-1)
			if (newSel>=0)
			{
				items[newSel].select(true)
				var last=o.getLastItem()
				if (o.layer&&last)
					last.css.borderBottom=_lstNobord
			}
			else o.select(true)
			
			list.showOrhideRoot()
			
			return true;
		}
		if (item.isNode())
		{
			if (item.removeChild(nodeOrItem))
				return true
		}
	}
	return false
}

// ================================================================================

function AndOrNodeWidget_remove()
// Remove itself or all the tree except the root
{
	var o=this
	if (o.par)
		o.par.removeChild(o)
	else 
	{ 	
		//delete all child of root	
		var items=o.items,len=items.length		
		for (var i=len-1;i>=0;i--)
		{				
			o.removeChild(items[i])
		}
	}
	
}

// ================================================================================
// ================================================================================
//
// OBJECT newAndOrNodeItem (Constructor)
//
// Base class for a filter list OR or AND item (leaf)
//
// ================================================================================
// ================================================================================

function newAndOrNodeItem(id,text,imgIndex,value,tooltip)
// CONSTRUCTOR
// id       [String] the id for DHTML processing
// text     [String] item text
// imgIndex [int] image index in the combined image
// value    [String] item value - must be unique in the list
// tooltip  [String] tooltip
// Return [AndOrNodeWidget] the instance
{
	var o=newWidget(id)
	o.text=text
	o.value=value
	o.tooltip=tooltip?tooltip:''
	o.imgIndex=imgIndex?imgIndex:0
	o.par=null
	o.txtLayer=null

	o.getHTML=AndOrNodeItem_getHTML
	o.getDynamicHTML=AndOrNodeItem_getDynamicHTML
	o.oldAOItemInit=o.init
	o.init=AndOrNodeItem_init
	o.select=AndOrNodeItem_select
	o.isNode=AndOrNodeItem_isNode
	o.getParentNode=AndOrNodeItem_getParentNode
	o.getIndexInParent=AndOrNodeItem_getIndexInParent
	o.remove=AndOrNodeItem_remove
	o.setText=AndOrNodeItem_setText
	o.setInsertFeedback=AndOrNodeItem_setInsertFeedback
	o.isChildOf=AndOrNodeItem_isChildOf
	
	o.fullW=false

	return o
}

// ================================================================================

function AndOrNodeItem_setInsertFeedback(show,isAll)
{
	var o=this,isLeft=(_dropPosition!=-1),st=o.css,b=show?"#C00000":"#FFFFFF"
	var node=o.isNode()

	if (isAll)
	{
		st.borderTopColor=st.borderLeftColor=st.borderBottomColor=st.borderRightColor="#FFFFFF"
		
		
		if (node)
		{
			var dy=show?15:(6+(o.selected?3:0))
			elasticZoneSetImg(o.operLayer,28,dy,dy+1,dy+2)
		}
		else
		{
			var dy=show?12:(o.selected?3:0)
			elasticZoneSetImg(o.layer,28,dy,dy+1,dy+1,dy+2)
		}
	}	
	else 
	{
		var isVert=((o.list.layout==_vertBOList)||(o.list.layout==_andOrBOList))

		// Case of insersion into an empty node
		if (_dropParentWidget&&(_dropFeebackWidget.id==_dropParentWidget.id))
		{
			var feedLayer=o.layer.childNodes[0].childNodes[0].childNodes[1],feedCSS=feedLayer.style
			feedCSS.borderLeft=show?_lstBord:_lstNullbord
			feedLayer.innerHTML=show?'&nbsp;':''
		}
		else
		{
			if (isLeft)
			{
				if (isVert)
					st.borderTopColor=b
				else
					st.borderLeftColor=b
			}
			else
			{
				if (isVert)
					st.borderBottomColor=b
				else
					st.borderRightColor=b
			}
		
			if (node)
			{
				var dy=6+(o.selected?3:0)
				elasticZoneSetImg(o.operLayer,28,dy,dy+1,dy+2)
			}
			else
			{
				var dy=(o.selected?3:0)
				elasticZoneSetImg(o.layer,28,dy,dy+1,dy+1,dy+2)
			}
		}

	}
}


// ================================================================================

function AndOrNodeItem_getParentNode()
// Return [AndOrNodeWidget] the parent node
{
	return this.par
}

// ================================================================================

function AndOrNodeItem_isNode()
// Test if it's a node
{
	return false
}

// ================================================================================

function AndOrNodeItem_init()
// Initialization
// Returns : [void]
{
	var o=this
	o.oldAOItemInit()
	o.layer.onmousedown=AndOrNodeItem_clickCB
	o.layer.onmouseup=AndOrNodeItem_mouseupCB
	if (_ie)
		o.layer.onmousemove=AndOrNodeWidget_triggerDD

	o.layer.onkeydown=AndOrNodeItem_keyDownCB	
	addDblClickCB(o.layer,AndOrNodeItem_dblClickCB)
	
	o.layer.title=o.tooltip

	var dd=o.list.dragDrop
	if (_ie&&dd)
	{
		dd.attachCallbacks(o.layer)
		o.layer.BODDType="item"
	}
}

// ================================================================================

function AndOrNodeItem_getHTML()
// Returns : [String] the HTML
{
	var o=this, list=o.list;
	var topBorder=((list.layout==_vertBOList)||(list.layout==_andOrBOList))
	var bottomBorder=(o.par.getLastItem().id==o.id)?topBorder:null
	
	//only display 10 first caracters of the name
	var text=o.text
	if(list.layout!=_andOrBOList && !list.autofit)
	{		
		if(text.length>10)	text=text.slice(0,10)+"..."					
	}	
	return elasticZone(text,_skin+'bolist.gif',3,28,o.selected?84:0,o.id,'treeNormal',o.list.image,0,o.imgIndex*16,null,'AndOrItemTxt_'+o.id,o.fullW,topBorder,bottomBorder)
}

// ================================================================================

function AndOrNodeItem_getDynamicHTML()
// Returns : [String] the HTML
{
	var o=this
	var topBorder=((this.list.layout==_vertBOList)||(this.list.layout==_andOrBOList))
	var bottomBorder=(o.par.getLastItem().id==o.id)?topBorder:null
	
	//only display 10 first caracters of the name
	var text=o.text
	if(this.list.layout!=_andOrBOList)
	{		
		if(text.length>10)	text=text.slice(0,10)+"..."					
	}	
	return dynamicElasticZone(text,_skin+'bolist.gif',3,28,o.selected?84:0,o.id,'treeNormal',o.list.image,0,o.imgIndex*16,null,'AndOrItemTxt_'+o.id,o.fullW,topBorder,bottomBorder)
}

// ================================================================================

function AndOrNodeItem_clickCB(e)
// HTML Callback
{
	var o=getWidget(this),list=o.list
	o.select(true)
	o.clicked=true

	o.initialX=eventGetX(e)
	o.initialY=eventGetY(e)

	if (list.changeCB)
		list.changeCB()
	
	return false
}

// ================================================================================

function AndOrNodeItem_mouseupCB()
{
	var o=getWidget(this)
	o.clicked=false
}

// ================================================================================
function AndOrNodeItem_keyDownCB(e)
{
	var o=getWidget(this),list=o.list
	var key=eventGetKey(e);	
	
	if (key==46)
	{		
		if (list.deleteCB)
			list.deleteCB()
	}
	if(key==13)	
	{
		eventCancelBubble(e);
		
		o.select(true)

		if (list.changeCB)
			list.changeCB()
	}	
}
// ================================================================================

function AndOrNodeItem_dblClickCB()
// HTML Callback
{
	var o=getWidget(this),list=o.list

	if (list.dblClickCB)
		list.dblClickCB()
	
	return false
}

// ================================================================================

function AndOrNodeItem_select(sel)
// Select the current item
{
	var o=this,list=o.list
	if (sel)
		list.unselect()
	list.selection=sel?o:null
	o.selected=sel
	
	if (o.layer)
	{
		var dy=sel?3:0
		elasticZoneSetImg(o.layer,28,dy,dy+1,dy+1,dy+2)
		
		if(sel)
			o.layer.title=_lstSelectedLabel+" "+o.tooltip
		else
			o.layer.title=o.tooltip
		
		if(sel&&o.layer.focus&&(!isHidden(o.layer)))
			o.layer.focus()
	}
}

// ================================================================================

function AndOrNodeItem_remove()
// Remove itself
{
	var o=this
	if (o.par)
		o.par.removeChild(o)
}

// ================================================================================

function AndOrNodeItem_getIndexInParent()
// return [int] index in parent node (-1 if no parent)
{
	var o=this,par=o.par
	return par?par.getChildIndex(o):-1
}

// ================================================================================

function AndOrNodeItem_setText(text)
// Remove itself
{
	var o=this
	if (o.txtLayer==null)	
		o.txtLayer=getLayer("AndOrItemTxt_"+o.id)		
	
	o.text=text
	o.txtLayer.innerHTML=convStr(o.text,true)
	
	o.tooltip=text
	o.layer.title=o.tooltip
}

// ================================================================================

function AndOrNodeItem_isChildOf(node)
// Tests if node is a parent
// return [boolean]
{
	var p=this.par

	if (p==null)
		return false
	if (p.id==node.id)
		return true
	else
		return p.isChildOf(node)
}

// ================================================================================

function elasticZoneInner(text,url,w,h,dy,id,cls,img,imgDx,imgDy,att,txtId,fullW,topBorder,bottomBorder,crs,extraHTML)
// Create a zone with a left image, a 
{	
	var bodyHTML ='';
	if(extraHTML)
		//bodyHTML = '<div '+attr("id",txtId)+' class="'+cls+'" style="width:0px">'+extraHTML+'</div>'
		bodyHTML = '<table '+attr("id",txtId)+' class="'+cls+'" cellpadding="0" cellspacing="0" border="0"><tr valign="middle"><td>'+extraHTML+'</td></tr></table>'
	else
		bodyHTML = '<nobr '+attr("id",txtId)+' class="'+cls+'">'+convStr(text,true)+'</nobr>'
		
	return '<tbody><tr valign="middle"><td width="'+w+'">'+imgOffset(url,w,h,0,dy)+'</td>'+
				(img?('<td style="padding-left:2px;'+backImgOffset(url,0,dy+h)+'">'+imgOffset(img,16,16,imgDx,imgDy)+'</td>'):'')+
				'<td '+(att?att:'')+'  '+(fullW? 'width="100%"':'')+' style="padding-left:2px;padding-right:'+(img?8:4)+'px;'+backImgOffset(url,0,dy+h)+'">'+bodyHTML+'</td><td width="'+w+'">'+imgOffset(url,w,h,0,dy+(2*h))+'</td></tr></tbody>'
}


// ================================================================================

function elasticZone(text,url,w,h,dy,id,cls,img,imgDx,imgDy,att,txtId,fullW,topBorder,bottomBorder,crs,extraHTML)
// Create a zone with a left image, a 
{
	var s=""
	if (topBorder!=null)
		s+=(topBorder?"border-top:":"border-left:")+_lstNobord+";"
		
	if (bottomBorder!=null)
		s+=(bottomBorder?"border-bottom:":"border-right:")+_lstNobord+";"

	crs=crs?crs:_hand
	
	return '<table '+attr("id",id)+' boelastic="1" tabIndex="0" '+(fullW?'width="100%"':'')+' cellpadding="0" cellspacing="0" border="0"  style="'+s+'cursor:'+crs+'" height="'+h+'">' +
				elasticZoneInner(text,url,w,h,dy,id,cls,img,imgDx,imgDy,att,txtId,fullW,topBorder,bottomBorder,crs,extraHTML) +
			'</table>'
}

// ================================================================================

function dynamicElasticZone(text,url,w,h,dy,id,cls,img,imgDx,imgDy,att,txtId,fullW,topBorder,bottomBorder,crs,extraHTML)
// Create a zone with a left image, a 
{
	crs=crs?crs:_hand
	
	var theTable=_curDoc.createElement('table')
	
	if (topBorder!=null)
	{
		if (topBorder)
			theTable.style.borderTop = _lstNobord
		else
			theTable.style.borderLeft = _lstNobord
	}

	if (bottomBorder!=null)
	{
		if (bottomBorder)
			theTable.style.borderBottom = _lstNobord
		else
			theTable.style.borderRight = _lstNobord
	}
		
	
	theTable.id = id
	theTable.setAttribute("boelastic","1")
	theTable.tabIndex = 0

	if (fullW)
		theTable.width="100%"
	
	theTable.cellPadding = 0
	theTable.cellSpacing = 0
	theTable.border = 0
	
	theTable.style.cursor = crs
	theTable.height = h
	theTable.innerHTML= elasticZoneInner(text,url,w,h,dy,id,cls,img,imgDx,imgDy,att,txtId,fullW,topBorder,bottomBorder,crs,extraHTML)

	return theTable
}

// ================================================================================

function elasticZoneSetImg(lyr,h,dy1,dy2,dy3,dy4)
{
	if (lyr&&(lyr.getAttribute("boelastic")!=null))
	{
		var c=lyr.childNodes[0].childNodes[0].childNodes,j=1,len=c.length
		
 		for (var i=0;i<len;i++)
 		{
 			var ce=c[i]
 			if (ce.tagName!=null)
 				changeOffset(((j==1||j==(len))?ce.childNodes[0]:ce),0,h*eval('dy'+(j++)))
 		}
 	}
}

// ================================================================================


// ================================================================================
// ================================================================================
//
// OBJECT newAndOrFilter (Constructor)
//
// Base class for a filter list OR or AND item (leaf)
//
// ================================================================================
// ================================================================================

function newAndOrFilter(id,text,imgIndex,value,tooltip)
// CONSTRUCTOR
// id       [String] the id for DHTML processing
// filter   [Object] filter structure
// imgIndex [int] image index in the combined image
// value    [String] item value - must be unique in the list
// tooltip  [String] tooltip
// Return [AndOrNodeWidget] the instance
{
	var o=newWidget(id)
	o.text=text
	o.isAdvFilter=true;
	
	o.value=value
	o.tooltip=tooltip?tooltip:''
	o.imgIndex=imgIndex?imgIndex:0
	o.par=null
	o.txtLayer=null
	
	o.operatorCombo=newCustomCombo('AndOrFilterOperator_'+id,operator_changeCB,true,null);
	o.operatorCombo.par=o
	AndOrFilter_initOpCombo(o.operatorCombo);
	
	o.defZone=newAdvFilterDef('AndOrFilterDefinition_'+id);
	o.defZone2=newAdvFilterDef('AndOrFilterDefinition2_'+id);
	o.defZone.par=o
	o.defZone2.par=o
	
	o.getHTML=AndOrFilter_getHTML
	o.getExtraHTML=AndOrFilter_getExtraHTML
	o.getDynamicHTML=AndOrFilter_getDynamicHTML
	o.oldAOItemInit=o.init
	o.init=AndOrFilter_init
	o.select=AndOrFilter_select
	o.isNode=AndOrFilter_isNode
	o.getParentNode=AndOrFilter_getParentNode
	o.getIndexInParent=AndOrFilter_getIndexInParent
	o.remove=AndOrFilter_remove
	o.setText=AndOrFilter_setText
	o.setInsertFeedback=AndOrFilter_setInsertFeedback
	o.isChildOf=AndOrFilter_isChildOf
	
	o.getDefinition=AndOrFilter_getDefinition
	
	o.fullW=false

	return o
}

// ================================================================================

function AndOrFilter_setInsertFeedback(show,isAll)
{
	var o=this,isLeft=(_dropPosition!=-1),st=o.css,b=show?"#C00000":"#FFFFFF"
	var node=o.isNode()

	if (isAll)
	{
		st.borderTopColor=st.borderLeftColor=st.borderBottomColor=st.borderRightColor="#FFFFFF"
		
		
		if (node)
		{
			var dy=show?15:(6+(o.selected?3:0))
			elasticZoneSetImg(o.operLayer,28,dy,dy+1,dy+2)
		}
		else
		{
			var dy=show?12:(o.selected?3:0)
			elasticZoneSetImg(o.layer,28,dy,dy+1,dy+1,dy+2)
		}
	}	
	else 
	{
		var isVert=((o.list.layout==_vertBOList)||(o.list.layout==_andOrBOList))

		// Case of insersion into an empty node
		if (_dropParentWidget&&(_dropFeebackWidget.id==_dropParentWidget.id))
		{
			var feedLayer=o.layer.childNodes[0].childNodes[0].childNodes[1],feedCSS=feedLayer.style
			feedCSS.borderLeft=show?_lstBord:_lstNullbord
			feedLayer.innerHTML=show?'&nbsp;':''
		}
		else
		{
			if (isLeft)
			{
				if (isVert)
					st.borderTopColor=b
				else
					st.borderLeftColor=b
			}
			else
			{
				if (isVert)
					st.borderBottomColor=b
				else
					st.borderRightColor=b
			}
		
			if (node)
			{
				var dy=6+(o.selected?3:0)
				elasticZoneSetImg(o.operLayer,28,dy,dy+1,dy+2)
			}
			else
			{
				var dy=(o.selected?3:0)
				elasticZoneSetImg(o.layer,28,dy,dy+1,dy+1,dy+2)
			}
		}

	}
}


// ================================================================================

function AndOrFilter_getParentNode()
// Return [AndOrNodeWidget] the parent node
{
	return this.par
}

// ================================================================================

function AndOrFilter_isNode()
// Test if it's a node
{
	return false
}

// ================================================================================

function AndOrFilter_init()
// Initialization
// Returns : [void]
{
	var o=this
	o.oldAOItemInit()
	
	o.operatorCombo.init();
	o.defZone.init();
	o.defZone2.init();
	o.defZone2.setDisplay(false);
	
	o.layer.onmousedown=AndOrFilter_clickCB
	o.layer.onmouseup=AndOrFilter_mouseupCB
	if (_ie)
		o.layer.onmousemove=AndOrNodeWidget_triggerDD

	o.layer.onkeydown=AndOrFilter_keyDownCB	
	addDblClickCB(o.layer,AndOrFilter_dblClickCB)
	
	o.layer.title=o.tooltip

	var dd=o.list.dragDrop
	if (_ie&&dd)
	{
		dd.attachCallbacks(o.layer)
		o.layer.BODDType="item"
	}
}
// ================================================================================

function AndOrFilter_getExtraHTML()
{	
	var o=this,s="";
	s+='<table class="treeNormal" border="0" cellspacing="0" cellpadding="0"><tr valign="middle"><td>';	
	s+=o.text+'</td><td>';	
	s+=o.operatorCombo.getHTML();	
	s+='</td><td>';		
	s+=o.defZone.getHTML();
	s+='</td><td>';		
	s+=o.defZone2.getHTML();
	s+='</td></tr></table>';
	return s;
}

// ================================================================================

function AndOrFilter_getHTML()
// Returns : [String] the HTML
{
	var o=this, list=o.list;
	var topBorder=((list.layout==_vertBOList)||(list.layout==_andOrBOList))
	var bottomBorder=(o.par.getLastItem().id==o.id)?topBorder:null
	
	//only display 10 first caracters of the name
	var text=o.text
	if(list.layout!=_andOrBOList && !list.autofit)
	{
		if(text.length>10)	text=text.slice(0,10)+"..."					
	}	
	return elasticZone(text,_skin+'bolist.gif',3,28,o.selected?84:0,o.id,'treeNormal',o.list.image,0,o.imgIndex*16,null,'AndOrFilterTxt_'+o.id,o.fullW,topBorder,bottomBorder,null,o.getExtraHTML())
}

// ================================================================================

function AndOrFilter_getDynamicHTML()
// Returns : [String] the HTML
{
	var o=this
	var topBorder=((this.list.layout==_vertBOList)||(this.list.layout==_andOrBOList))
	var bottomBorder=(o.par.getLastItem().id==o.id)?topBorder:null
	
	//only display 10 first caracters of the name
	var text=o.text
	if(this.list.layout!=_andOrBOList)
	{		
		if(text.length>10)	text=text.slice(0,10)+"..."					
	}	
	return dynamicElasticZone(text,_skin+'bolist.gif',3,28,o.selected?84:0,o.id,'treeNormal',o.list.image,0,o.imgIndex*16,null,'AndOrFilterTxt_'+o.id,o.fullW,topBorder,bottomBorder,null,o.getExtraHTML())
}

// ================================================================================

function AndOrFilter_clickCB(e)
// HTML Callback
{
	var o=getWidget(this),list=o.list
	o.select(true)
	o.clicked=true

	o.initialX=eventGetX(e)
	o.initialY=eventGetY(e)

	if (list.changeCB)
		list.changeCB()
	
	return false
}

// ================================================================================

function AndOrFilter_mouseupCB()
{
	var o=getWidget(this)
	o.clicked=false
}

// ================================================================================
function AndOrFilter_keyDownCB(e)
{
	var o=getWidget(this),list=o.list
	var key=eventGetKey(e);	
	
	if (key==46)
	{		
		if (list.deleteCB)
			list.deleteCB()
	}
	if(key==13)	
	{
		eventCancelBubble(e);
		
		o.select(true)

		if (list.changeCB)
			list.changeCB()
	}	
}
// ================================================================================

function AndOrFilter_dblClickCB()
// HTML Callback
{
	var o=getWidget(this),list=o.list

	if (list.dblClickCB)
		list.dblClickCB()
	
	return false
}

// ================================================================================

function AndOrFilter_select(sel)
// Select the current item
{
	var o=this,list=o.list
	if (sel)
		list.unselect()
	list.selection=sel?o:null
	o.selected=sel
	
	if (o.layer)
	{
		var dy=sel?3:0
		elasticZoneSetImg(o.layer,28,dy,dy+1,dy+1,dy+2)
		
		if(sel)
			o.layer.title=_lstSelectedLabel+" "+o.tooltip
		else
			o.layer.title=o.tooltip
		
		if(sel&&o.layer.focus&&(!isHidden(o.layer)))
			o.layer.focus()
	}
}

// ================================================================================

function AndOrFilter_remove()
// Remove itself
{
	var o=this
	if (o.par)
		o.par.removeChild(o)
}

// ================================================================================

function AndOrFilter_getIndexInParent()
// return [int] index in parent node (-1 if no parent)
{
	var o=this,par=o.par
	return par?par.getChildIndex(o):-1
}

// ================================================================================

function AndOrFilter_setText(text)
// Remove itself
{
	var o=this
	if (o.txtLayer==null)	
		o.txtLayer=getLayer("AndOrFilterTxt_"+o.id)		
	
	o.text=text
	o.txtLayer.innerHTML=convStr(o.text,true)
	
	o.tooltip=text
	o.layer.title=o.tooltip
}

// ================================================================================

function AndOrFilter_isChildOf(node)
// Tests if node is a parent
// return [boolean]
{
	var p=this.par

	if (p==null)
		return false
	if (p.id==node.id)
		return true
	else
		return p.isChildOf(node)
}

// ================================================================================
function AndOrFilter_getDefinition()
// get the included AndOrBOListWidget
// return [AndOrBOListWidget]
{
	return this.defZone
}
// ================================================================================
function AndOrFilter_initOpCombo(fope)
{
	fope.icon.setClasses("iconnochecknobg", "iconchecknobg", "iconhovernobg", "iconcheckhovernobg")	
	fope.arrow.setClasses("iconnochecknobg", "iconchecknobg", "iconhovernobg", "iconcheckhovernobg")
	fope.arrow.overCB="IconWidget_overCB"
	fope.arrow.outCB="IconWidget_outCB"	
	fope.icon.overCB="IconWidget_overCB"
	fope.icon.outCB="IconWidget_outCB"
	fope.icon.width=null;
/*
	for (var i=_topfs._firstFilter;i<=_topfs._lastFilter;i++)
	{									
		fope.add(_topfs._labFil[i],""+i);	
	}
*/
	
	fope.add("EQUAL","EQUAL");
	fope.add("NOT_EQUAL","NOT_EQUAL");
	fope.add("GREATER","GREATER");
	fope.add("GREATER_OR_EQUAL","GREATER_OR_EQUAL");
	fope.add("LESS","LESS");
	fope.add("LESS_OR_EQUAL","LESS_OR_EQUAL");
	fope.add("BETWEEN","BETWEEN");
	fope.add("NOT_BETWEEN","NOT_BETWEEN");		
	fope.add("IN_LIST","IN_LIST",true);
	fope.add("NOT_IN_LIST","NOT_IN_LIST");
	fope.add("IS_NULL","IS_NULL");
	fope.add("NOT_IS_NULL","NOT_IS_NULL");	
	fope.add("LIKE","LIKE");
	fope.add("NOT_LIKE","NOT_LIKE");
	fope.add("BOTH","BOTH");
	fope.add("EXCEPT","EXCEPT");
}

// ================================================================================
function operator_changeCB()
{	
	var o=this, sel = o.getSelection()
	var advflt= o.par;
	if(sel && advflt)
	{
		switch(sel.value)
		{
			case "BETWEEN" :
			case "NOT_BETWEEN" :
			case "BOTH" :
				advflt.defZone2.setDisplay(true);
			break;
			case "IS_NULL" :
			case "NOT_IS_NULL" :
				advflt.defZone.setDisplay(false);
				advflt.defZone2.setDisplay(false);
			break;
			default :
				advflt.defZone.setDisplay(true);
				advflt.defZone2.setDisplay(false);
			break;			
		}	
		
	}
}

// ================================================================================
// AdvFilterDef: definition zone of the filter
// ================================================================================

_filterDefConst=1
_filterDefFromList=2
_filterDefPrompt=3
_filterDefObj=4

function newAdvFilterDef(id)
{
	var o=newWidget(id);
	
	o.type=_filterDefPrompt;
	o.obj=null;
	o.input="prompt by default ?";
	
	o.objLabel= newWidget(id+"_objlabel");
	o.inputTxt= newTextFieldWidget(id+"_input",null,null,null,null);
	
	o.propertyIcn=newIconWidget(id+"_property",_skin+"promptProp.gif",clickPropCB,null,null,16,16,0,0);	
	o.typeIcn=newIconWidget(id+"_type",_skin+"filterType.gif",clickTypeCB,null,null,16,16,0,0);		
	o.propertyIcn.setClasses("iconnochecknobg", "iconchecknobg", "iconhovernobg", "iconcheckhovernobg");
	o.typeIcn.setClasses("iconnochecknobg", "iconchecknobg", "iconhovernobg", "iconcheckhovernobg");
	
	var m=o.typeMenu=newMenuWidget(id+"_type_menu");	

	//build filter type menu
	m.addCheck("advf_constant","Constant",changeFTypeCB);
	m.addCheck("advf_fromList","From List",changeFTypeCB);
	m.addCheck("advf_prompt","Prompt",changeFTypeCB);
	m.addCheck("advf_object","Object",changeFTypeCB);

	o.inputTxt.afPar=o;
	o.propertyIcn.afPar=o;
	o.typeIcn.afPar=o;
	o.typeMenu.afPar=o;

	
	o.oldInit=o.init
	o.init=AdvFilterDef_init
	o.getHTML=AdvFilterDef_getHTML
	o.updateUI=AdvFilterDef_updateUI
	
	return o;
}

function AdvFilterDef_getHTML()
{
	var o=this;
	
	return(
	'<table id="'+o.id+'" border="0" cellspacing="0" cellpadding="0"><tbody>'+
		'<tr>'+
			'<td id="'+o.id+'_objlabel">'+				
				'<table class="treeNormal" border="0" cellspacing="0" cellpadding="0"><tr><td>'+imgOffset(_skin+"qualification.gif",16,16,0,0)+'</td><td><nobr>my object</nobr></td></tr></table>'+					
			'</td><td>'+
				o.inputTxt.getHTML()+
			'</td><td>'+
				o.propertyIcn.getHTML()+
			'</td><td>'+
				o.typeIcn.getHTML()+
			'</td>'+
		'</tr>'+
	'</tbody></table>');
}

function AdvFilterDef_init()
{
	var o=this;
	
	//init widget
	o.oldInit();
	o.objLabel.init();
	o.inputTxt.init();
	o.propertyIcn.init();
	o.typeIcn.init();
		
	//update UI depending on the type of filter
	o.updateUI();
}

function changeFTypeCB()
{
	var id = this.id;
	var menu=this.par,items=menu.items
	var o=menu.afPar;//AdvFilterDef objet
	
	for (var i in items)
	{
		var item=items[i]
		item.check(item.id==id)
	}
	
	switch(id)
	{
		case "advf_constant" :
			o.type	= _filterDefConst;	
		break;
		case "advf_fromList" :
			o.type	= _filterDefFromList;	
			alert("lov dialog");
		break;
		case "advf_prompt" :
			o.type	= _filterDefPrompt;	
		break;
		case "advf_object" :
			o.type	= _filterDefObj;	
			alert("universe dimensions dialog");			
		break;		
	}	
	o.updateUI();
}

function AdvFilterDef_updateUI()
{
	var o=this;
	
	switch(o.type)
	{		
		case _filterDefConst :
			o.objLabel.setDisplay(false);
			o.inputTxt.setDisplay(true);
			o.inputTxt.setValue("");
			o.propertyIcn.setDisplay(false);
			o.typeMenu.getItemByID("advf_constant").check(true);
		break;
		case _filterDefFromList :		
			o.objLabel.setDisplay(false);
			o.inputTxt.setDisplay(true);
			o.inputTxt.setValue("");
			o.propertyIcn.setDisplay(false);
			o.typeMenu.getItemByID("advf_fromList").check(true);				
		break;
		case _filterDefPrompt :
			o.objLabel.setDisplay(false);
			o.inputTxt.setDisplay(true);
			o.inputTxt.setValue(o.input);			
			o.propertyIcn.setDisplay(true);	
			o.typeMenu.getItemByID("advf_prompt").check(true);
		break;
		case _filterDefObj :
			o.objLabel.setDisplay(true);
			o.inputTxt.setDisplay(false);
			o.propertyIcn.setDisplay(false);
			o.typeMenu.getItemByID("advf_object").check(true);
		break;	
	}
}

function clickPropCB()
{
	alert("prompt property dialog");
}

function clickTypeCB()
{
	var o=this.afPar, icn = this, l=icn.layer,menu=o.typeMenu;
	menu.show(true,getPosScrolled(l).x  + (menu.alignLeft?icn.getWidth():0) ,getPosScrolled(l).y+icn.getHeight()+1);
	IconWidget_overCB(icn.index)

}




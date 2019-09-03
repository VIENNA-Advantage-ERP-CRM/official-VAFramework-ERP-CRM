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

File: treeview.js
Custom treeview control
=============================================================
*/

// ================================================================================
// ================================================================================
//
// OBJECT newTreePromptWidget (Constructor)
//
// Tree view class
//
// ================================================================================
// ================================================================================

function newTreePromptWidget(id,w,h,clickCB,doubleClickCB,bgClass,expandCB,collapseCB)
{
	var o=newTreeWidget(id,w,h,_skin+'../prompt.gif',clickCB,doubleClickCB,bgClass,expandCB,collapseCB)

	o.padding=0
	o.iconW=15
	o.iconH=17
	o.dispIcnFuncName="dispIcnPrompt"
	return o
}

// ================================================================================
// ================================================================================
//
// OBJECT newTreePromptElem (Constructor)
//
// Tree view element class
//
// ================================================================================
// ================================================================================

function newTreePromptElem(name,values,userData)
// name [String] the prompt name
// values [String] the prompt values. use null if no values (used to chnage the check mark)
// userData [Varies - Optional] user data
{
	var o=newTreeWidgetElem(3,name,userData,null,2,null)
	o.isPrompt=true
	o.values=values

	o.getHTML=TreePromptElem_getHTML
	o.isChecked=TreePromptElem_isChecked
	o.change=TreePromptElem_change

	o.selectedClass='promptSelected'
	o.nonselectedClass='promptNormal'

	o.select=TreePromptElem_select
	o.selected=false
	o.checkIcn=null
	o.init=TreePromptElem_init

	return o
}

// ================================================================================

function TreePromptElem_init()
{
	var o=this
	o.domElem=getLayer(_codeWinName+'trLstElt' + o.id)
	o.icnLyr=getLayer(_codeWinName+'icn' + o.id)
	o.toggleLyr=getLayer(_codeWinName+'trTog' + o.id)
}

// ================================================================================

function TreePromptElem_change(name,values)
{
	var o=this

	o.name=name
	o.values=values

	if (o.domElem==null)
		o.domElem=getLayer(_codeWinName+"trLstElt"+o.id);

	if (o.domElem!=null)
	{
		o.domElem.innerHTML=convStr(o.name)+(o.values!=null?('<b><span style="color:red">&nbsp;:&nbsp;</span>'+convStr(o.values)+'</b>'):'')		
					
		if (o.par==null)
		{

			if (o.checkIcn==null)
				o.checkIcn=getLayer(_codeWinName+o.id+"_checkIcn")

			changeSimpleOffset(o.checkIcn,0,o.isChecked()?0:17,null,o.isChecked()?_checkedPromptLab:_nocheckedPromptLab)					
		}
		//cf updatetooltip in treeview.js
		if(o.values)
			o.advTooltip=_selectionPromptLab +' '+ o.values
		else
			o.advTooltip=_noselectionPromptLab		
	}
}

// ================================================================================

function TreePromptElem_select(setFocus,ev)
{
	with (this)
	{
		if (treeView.selId!=id)
		{
			if (treeView.selId>=0)
			{
				var prev=_TreeWidgetElemInstances[treeView.selId]
				prev.init()
				var e=prev.domElem
				e.className=prev.nonselectedClass
				e.parentNode.parentNode.className=prev.nonselectedClass

				var prevIcn=prev.icnLyr
				if (prevIcn==null)
					prevIcn=prev.icnLyr=getLayer(_codeWinName+"icn"+prev.id)
				prevIcn.parentNode.className=prev.nonselectedClass

				prev.selected=false

				var arrowL=prevIcn.childNodes[0]
				changeSimpleOffset(arrowL,prev.selected?15:0,prev.sub.length==0?17*4:(prev.expanded?17*2:17*3))

			}

			treeView.selId=id;
			init()
			treeView.layer._BOselId=id

			var de=domElem
			de.className=selectedClass
			de.parentNode.parentNode.className=selectedClass

			if (this.icnLyr==null)
				this.icnLyr=getLayer(_codeWinName+"icn"+id)
			this.icnLyr.parentNode.className=selectedClass

			this.selected=true

			var arrowL=this.icnLyr.childNodes[0]
			changeSimpleOffset(arrowL,this.selected?15:0,this.sub.length==0?17*4:(this.expanded?17*2:17*3))



			if (setFocus)
				de.focus()

			if (treeView.clickCB) treeView.clickCB(userData)
		}
	}
	// stop propagating event to the other link
	if (ev)
		eventCancelBubble(ev) //DOESN'T WORK WITH NETSCAPE
}

// ================================================================================

function TreePromptElem_isChecked()
{
	return this.values!=null
}

// ================================================================================

function TreePromptElem_getHTML(indent,isFirst)
// Get the prompt tree element HTML
// indent [int] : element indentation in pixels
// returns [String] the HTML
{
	var o=this,a=new Array,i=0,len=o.sub.length,exp=(len>0)||o.isIncomplete,icns=o.treeView.icns
	var bord="border-top:0px;"

	if (isFirst)
	{
		o.treeView.isFirst=false
		bord=""
	}
	
	var isLast=false
	if (o.par)
	{
		var psub=o.par.sub,plen=psub.length
		if (plen>0)
			isLast = (psub[plen-1].id==o.id)
	}
	
	if ((o.par!=null)&&isLast)
		bord+="border-bottom:0px;"

	var cliEvt=' onclick="'+_codeWinName+'.TreeWidget_clickCB('+o.id+',false,event);if(_saf||_ie) return false" onmousedown="'+_codeWinName+'.TreeWidget_clickCB('+o.id+',false,event);if(_saf||_ie) return false" ondblclick="'+_codeWinName+'.treeDblClickCB('+o.id+',event);return false" '

	a[i++]='<table style="border:1px solid #E2E2E2;border-right:0px;'+bord+'" width="100%" border="0" cellspacing="0" cellpadding="0">'

	// first row
	a[i++]='<tr valign="top">'

		// Icon cheched/unchecked for first level elements
		if (o.par==null)
			a[i++]='<td width="15" style="border-right:1px solid #E2E2E2;">'+simpleImgOffset(icns,15,17,0,o.isChecked()?0:17,_codeWinName+o.id+"_checkIcn",null,o.isChecked()?_checkedPromptLab:_nocheckedPromptLab)+'</td>'

		// Icon
		a[i++]='<td '+cliEvt+' class="promptNormal" width="15"><span id="'+_codeWinName+'icn'+o.id+'" '+(_moz?'onclick':'onmousedown')+'="'+_codeWinName+'.TreeWidget_clickCB('+o.id+',true,event); if (_ie) return false" ondblclick="'+_codeWinName+'.treeDblClickCB('+o.id+',event);return false">'+simpleImgOffset(icns,15,17,0,exp?(o.expanded?34:51):68)+'</span></td>'

		// name & values
		a[i++]='<td  '+cliEvt+' class="promptNormal" width="100%" style="padding-top:2px;padding-right:0px"><nobr>'
		a[i++]='&nbsp;<a class="promptNormal" id="'+_codeWinName+'trLstElt'+o.id+'" href="javascript:void(0)" onfocus="'+_codeWinName+'.treeFCCB(this,'+o.id+',true)" onblur="'+_codeWinName+'.treeFCCB(this,'+o.id+',false)">'
		a[i++]=''+convStr(o.name)+(o.values!=null?('<b><span style="color:red">&nbsp;:&nbsp;</span>'+convStr(o.values)+'</b>'):'')+'</a>'
		a[i++]='</nobr>'


		// child nodes
		if (exp)
			a[i++]='<table width="100%" style="display:'+(o.expanded?'block':'none')+' " border="0" cellspacing="0" cellpadding="0"><tr><td id="'+_codeWinName+'trTog'+o.id+'" style="padding:0px;padding-left:20px;padding-top:3px;">'

			// Generate child HTML if needed
			if (o.expanded)
			{
				o.generated=true
				var idt=indent+_trIndent
				for (var j=0;j<len;j++) a[i++]=o.sub[j].getHTML(idt,j==0);
				o.treeView.isFirst=false
			}

		// End child nodes
		if (exp)
		{
			nodeIndent=indent
			a[i++]="</td></tr></table>"
		}

		a[i++]='</td></tr></table>'

	// End first row

	//use in updatetooltip in treeview.js
	if(o.values)
		o.advTooltip=_selectionPromptLab +' '+ o.values
	else
		o.advTooltip=_noselectionPromptLab	
			
	return a.join("");
}

// ================================================================================

function dispIcnPrompt(eId)
{
	var e=_TreeWidgetElemInstances[eId]
	with (e)
	{
		select()

		if (expanded&&!generated)
		{
			e.generated=true;
			var s='',len=sub.length,newInd=nodeIndent+_trIndent
			for (var j=0;j<len;j++) s+=sub[j].getHTML(newInd,j==0);
			toggleLyr.innerHTML=s;
		}

		var icn=treeView.icns[iconId]
		toggleLyr.parentNode.parentNode.parentNode.style.display=expanded?'block':'none'

		var arrowL=icnLyr.childNodes[0]
		changeSimpleOffset(arrowL,selected?15:0,expanded?17*2:17*3)
	}
}


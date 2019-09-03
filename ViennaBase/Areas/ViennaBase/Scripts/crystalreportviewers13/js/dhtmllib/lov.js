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

File: lov.js

dom.js palette.js treeview.js bolist.js menu.js calendar.js  
must be included before

=============================================================
*/

//---------------------------------------------------------------------------------------
//newLovWidget definition
//---------------------------------------------------------------------------------------
function newLovWidget(id,label,w,lines,showText,showRefresh,moveCB,refreshCB,searchCB,
	dblClickCB,multi,enterCB,showDate,showTextOnly)
// CONSTRUCTOR
// id			[String]	the id for DHTML processing
// label		[String]	the label of the chunk
// w			[int]		widget width, including borders
// lines		[int]		number of visible lines in the LOV list 
// showText		[boolean]	display a textFieldWidget if true
// showRefresh	[boolean]	display a refresh icon if true
// moveCB		[function]	callback when click on the left arrow, combobox, right arrow
// refreshCB	[function]	callback when clicking on the refresh icon
// searchCB		[function]	callback when clicking on ENTER or on search icon
// dlbClickCB	[function]	callback when double clicking on the lov list
// multi		[boolean]	list is multi "selectionable" if true
// showDate		[boolean]	display a calendar widget if true
// showTextOnly [boolean]   show only the text field, not the LOV part
// Return		[LovWidget] the instance
{		
	_arrowSize		= 20
	_refreshIcnSize	= 28
	_searchIcnSize	= 40
	_margin			= 2
	// We suppose that a text field height is 20 pix
	_textFieldH	= 20;
	_searchMinW = 20;
	// We suppose that the height of an item line in a combo is 9 pix
	// The width of the LovWidget is w
	// The height of the LovWidget is h
	// With all this data, we can calcultate the widgets width and height	
	var comboW	= showRefresh ? (w - (2*_arrowSize+_refreshIcnSize)) : (w - (2*_arrowSize))	
	
	var o=newWidget(id)
	o.width=w
	o.lines=lines
		
	// widgets
	o.textField				= newTextFieldWidget(id+"_textField",null,250,null,LovWidget_enterCB,true,_lovTextFieldLab,w);
	o.textField.par			= o;
	
	o.dateField				= newCalendarTextFieldButton(id+"_calendarText",null,null,null,null,LovWidget_enterCB,true,_lovCalendarLab,w);
	o.dateField.par			= o;

	o.chunkLabel			= newWidget(id+"_label");	
	o.noChunkLabel			= newWidget(id+"_noChunkLabel");
		
	o.prevChunkIcn			= newIconWidget(id+"_prevChunk",_skin+'../lov.gif',LovWidget_prevCB,null,_lovPrevChunkLab,7,16,2*16,0,2*16,16);
	o.prevChunkIcn.par		= o;
		
	o.chunkCombo			= newComboWidget(id+"_chunk",LovWidget_comboCB,true,comboW,_lovComboChunkLab);
	o.chunkCombo.par		= o;
	
	o.nextChunkIcn			= newIconWidget(id+"_nextChunk",_skin+'../lov.gif',LovWidget_nextCB,null,_lovNextChunkLab,7,16,2*16+1*7,0,2*16+1*7,16);
	o.nextChunkIcn.par		= o;
	
	o.refreshIcn			= newIconWidget(id+"_refresh",_skin+'../lov.gif',LovWidget_refreshCB,null,_lovRefreshLab,16,16,16,0);
	o.refreshIcn.par		= o;

	// special property for Grouping prompts feature
	o.hideChunk_n_Refresh	= false;
	
	var multiSelection		= (multi != null) ? multi : false;	
	o.lovList				= newListWidget(id+"_lov",null,multiSelection,w,lines,null,LovWidget_dblClickCB,LovWidget_ListKeyUpCB);
	o.lovList.par			= o;
	
	o.lovSearch				= newSearchWidget(id+"_searchVal",null,searchCB);
	/*
	o.searchField			= newTextFieldWidget(id+"_searchVal",null,50,LovWidget_keyUpCB,LovWidget_searchCB,true,_lovSearchFieldLab);
	o.searchField.par		= o;
	
	o.searchIcn				= newIconMenuWidget(id+"_searchIcn",_skin+'../lov.gif',LovWidget_searchCB,null,_lovSearchLab,16,16,0,0,0,0)
	o.searchIcn.par			= o
	o.searchMenu			= o.searchIcn.getMenu()
	o.normal				= o.searchMenu.addCheck("normal",_lovNormalLab,LovWidget_normalClickCB)
	o.matchCase				= o.searchMenu.addCheck("matchCase",_lovMatchCase,LovWidget_matchCaseClickCB)
	
	*/
	
	// private properties
	o.showTextField			= showText;
	o.showRefreshIcn		= showRefresh;
	o.showTextOnly			= showTextOnly==null?false:showTextOnly;
	o.chunkText				= label;
	o.showDate				= showDate==null?false:showDate;
	
	// private methods
	o.oldInit				= o.init;
	o.goToChunk				= LovWidget_goToChunk;
	o.setPrevNextIconState	= LovWidget_setPrevNextIconState;
	o.moveCB				= moveCB;
	o.refreshCB				= refreshCB;
	o.searchCB				= searchCB;
	o.dblClickCB			= dblClickCB;
	o.enterCB				= enterCB;
	
	// public methods
	o.init					= LovWidget_init;
	o.getHTML				= LovWidget_getHTML;	
	o.resize				= LovWidget_resize;		
	o.addChunkElem			= LovWidget_addChunkElem;
	o.addLOVElem			= LovWidget_addLOVElem;	
	o.fillChunk				= LovWidget_fillChunk;
	o.fillLOV				= LovWidget_fillLOV;
	o.getSearchValue		= LovWidget_getSearchValue;
	o.setSearchValue		= LovWidget_setSearchValue;
	o.getTextValue			= LovWidget_getTextValue;
	o.setTextValue			= LovWidget_setTextValue;
	o.getDateValue			= LovWidget_getDateValue;
	o.setDateValue			= LovWidget_setDateValue;
	o.getChunkSelection		= LovWidget_getChunkSelection;
	o.getLOVSelection		= LovWidget_getLOVSelection;
	o.setTooltips			= LovWidget_setTooltips;	
	o.change				= LovWidget_change;
	o.showTextFieldOnly		= LovWidget_showTextFieldOnly;
	o.setFormatDate			= LovWidget_setFormatDate;
	o.isCaseSensitive		= LovWidget_isCaseSensitive;
	o.setCaseSensitive		= LovWidget_setCaseSensitive;
	o.updateWidget			= LovWidget_updateWidget
	
	return o;
}

//---------------------------------------------------------------------------------------
// widget functions
//---------------------------------------------------------------------------------------
// getHTML
// Returns the HTML generated for the LOVWidget
function LovWidget_getHTML()
{
	var o=this, s =''
	s=	'<table id="'+o.id+'" class="dialogzone" border="0" cellspacing="0" cellpadding="0"><tbody>'
	
	s+= '<tr>' + 
			'<td colspan="4">' + o.textField.getHTML() + '</td>' + 
		'</tr>'		
	
	s+= '<tr>' + 
			'<td colspan="4">' + o.dateField.getHTML() + '</td>' + 
		'</tr>'		
	
	s+=	'<tr>' +
			'<td colspan="4"><span id="' + o.chunkLabel.id + '"></span></td>' +
		'</tr>'
		
	s+=	'<tr' + (o.hideChunk_n_Refresh?' style="display:none">':'>') +			
			'<td align="center">' + o.prevChunkIcn.getHTML() + '</td>' +
			'<td align="center">' + o.chunkCombo.getHTML() + '</td>' +
			'<td align="center">' + o.nextChunkIcn.getHTML() + '</td>' +
			'<td align="right">' +  
				'<table class="dialogzone" border="0" cellspacing="0" cellpadding="0"><tbody>' +
				'<tr>' + 
					'<td><span id="' + o.noChunkLabel.id + '"></span></td>' +
					'<td>' + o.refreshIcn.getHTML() + '</td>' +	
				'</tr>' +
				'</tbody></table>' +
			'</td>' +
		'</tr>'
				
	s+= '<tr>' +
			'<td colspan="4">' + o.lovList.getHTML() + '</td>' +		
		'</tr>'
		
	s+=	'<tr>' +
			'<td colspan="4">' +
				o.lovSearch.getHTML()+
/*			
				'<table border="0" width="100%" cellspacing="0" cellpadding="0"><tbody>' +
				'<tr>' +
					'<td>' + o.searchField.getHTML() + '</td>' +
					'<td>' + o.searchIcn.getHTML() + '</td>' +
				'</tr>' +
				'</tbody></table>' +
*/			'</td>' +		
		'</tr>' +		
	'</tbody></table>'
		
	return s
}

// init
// Inits all widgets
function LovWidget_init()
{	
	var o=this
	o.oldInit()
	
	o.textField.init()
	o.textField.setDisplay(o.showTextField)	
	
	o.dateField.init()
	o.dateField.setDisplay(o.showDate)
	
	// Do not show textfield if date is visible
	if(o.showDate) 
		o.textField.setDisplay(false)
	
	o.chunkLabel.init()	
	o.chunkLabel.setHTML(o.chunkText)
	o.noChunkLabel.init()
	o.noChunkLabel.setHTML(_lovRefreshValuesLab)
	o.noChunkLabel.setDisplay(false)	// Not displayed by default
	o.prevChunkIcn.init()	
	o.chunkCombo.init()	
	o.nextChunkIcn.init()
		
	o.refreshIcn.init()
	o.refreshIcn.setDisplay(o.showRefreshIcn)	
		
	o.lovList.init()	
	o.lovSearch.init()
/*	o.searchField.init()	
	o.searchIcn.init()
	o.searchIcn.setDisabled((o.searchField.getValue()==''))	// Disabled if no value set on loading widget
	o.setCaseSensitive(false)
*/			
	o.showTextFieldOnly(o.showTextOnly)
	
	o.resize(o.width,o.lines)	
}

// resize(w,lines)
// Resize the LOVWidget
function LovWidget_resize(w,lines)
// w			[int]		widget width, including borders
// lines		[int]		number of visible lines in the LOV list
{	
	var o=this
	
	// The search field must be at least <_searchMinW>px long
	var tmp		= w - _searchIcnSize
	var searchW	= (tmp < _searchMinW)?_searchMinW:tmp
	
	// If not possible to have a search field of <_searchMinW>px long with the <w> specified, increase <w>	
	var newWidth= (tmp < _searchMinW)?((w+_searchMinW)-_margin):(w- _margin)
	
	var textW	= newWidth
		
	var comboW	= o.showRefreshIcn ? (newWidth - (2*_arrowSize+_refreshIcnSize)) : (newWidth - (2*_arrowSize))

	if ( o.showTextField )		
		o.textField.resize(textW)
		
	if ( o.showDate )
		o.dateField.resize(newWidth)
		
	o.chunkCombo.resize(comboW)
	
	o.lovList.resize(newWidth)
	o.lovList.layer.size = lines
	
	//o.searchField.resize(searchW)		
	o.lovSearch.resize(_searchIcnSize+searchW);
}

// addChunkElem(s,val,sel,id)
// Add one element to the chunkCombo combobox
function LovWidget_addChunkElem(s,val,sel,id)
// s			[String]	string that will be displayed
// val			[String]	stored value
// sel			[boolean]	if true the item is selected
// id			[String]	the id for DHTML processing
{
	var o=this;
	o.chunkCombo.add(s,val,sel,id);
}

// addLOVElem(s,val,sel,id)
// Add one element to the lovList ListWidget
function LovWidget_addLOVElem(s,val,sel,id)
// s			[String]	string that will be displayed
// val			[String]	stored value
// sel			[boolean]	if true the item is selected
// id			[String]	the id for DHTML processing
{
	var o=this;
	o.lovList.add(s,val,sel,id);
}

// fillChunk(arrTxt,arrVal)
// Fill the chunkCombo combobox
function LovWidget_fillChunk(arrTxt,arrVal)
// This function can take one or two parameters
// 1st parameter is mandatory
// 2nd parameter is optional
// arrTxt		[array]	array with text to insert the combo
// arrTxt		[array]	array with values to insert the combo
{
			
	var o=this
	o.chunkCombo.del()
	
	//disable the buttons and the combo box when it is empty
	o.setPrevNextIconState()
	
	if ( arrTxt == null )
	{	
		o.updateWidget()	
		return
	}
		
	if ( arrVal == null )
	{
		for (var i=0;i<arrTxt.length;i++)
			o.chunkCombo.add(arrTxt[i],arrTxt[i],false,i)
	}
	else
	{
		for (var i=0;i<arrTxt.length;i++)
			o.chunkCombo.add(arrTxt[i],arrVal[i],false,i)
	}
	
	// select the 1st item
	o.chunkCombo.select(0)
	
	// set the previous and next icon state
	o.setPrevNextIconState()
	
	o.updateWidget()
}

// _fillLOV(arrTxt,arrVal)
// Fill the lovList ListWidget
function LovWidget_fillLOV(arrTxt,arrVal)
// This function can take one or two parameters
// 1st parameter is mandatory
// 2nd parameter is optional
// arrTxt		[array]	array with text to insert the list
// arrTxt		[array]	array with values to insert the list
{
	var o=this;
	o.lovList.del();
				
	if ( arrTxt == null )
		return;
	
	if ( arrVal == null )
	{
		for (var i=0;i<arrTxt.length;i++)
			o.lovList.add(arrTxt[i],arrTxt[i],false,i);
	}
	else
	{
		for (var i=0;i<arrTxt.length;i++)
			o.lovList.add(arrTxt[i],arrVal[i],false,i);
	}
}

// getSearchValue()
// Return the value inserted in the search text field
function LovWidget_getSearchValue()
// return		a string with the value entered in the search field
{
	var o=this;
	return o.lovSearch.getSearchValue();
}

// setSearchValue(s)
// Set the value of the search text field
function LovWidget_setSearchValue(s)
// s			[String]	string that will be displayed in the textfield
{
	var o=this;
	o.lovSearch.setValue(s);
}

// getTextValue()
// Return the value inserted in the text field
function LovWidget_getTextValue()
// return		a string with the value entered in the text field
{
	var o=this;
	return o.textField.getValue();
}

// setTextValue(s)
// Set the value of the search text field
function LovWidget_setTextValue(s)
// s			[String]	string that will be displayed in the textfield
{
	var o=this;
	o.textField.setValue(s);
}

// getChunkSelection()
// Return the selected chunk
function LovWidget_getChunkSelection()
// return		a structure(index, value) of the selected chunk
{
	var o=this;
	return o.chunkCombo.getSelection();
}

// getLOVSelection()
// Return the selected lov(s)
function LovWidget_getLOVSelection()
// return		an array of structure(index, value) of the selected lov(s)
{
	var o=this;
	return o.lovList.getMultiSelection();
}

// getDateValue()
// Return the value inserted in the date text field
function LovWidget_getDateValue()
// return		a string with the value entered in the text field
{
	var o=this;
	return o.dateField.getValue();
}

// setDateValue(s)
// Set the value of the date text field
function LovWidget_setDateValue(s)
// s			[String]	string that will be displayed in the textfield
{
	var o=this;
	o.dateField.setValue(s);
}

// setTooltips(text,prev,next,refresh,search,chunk,lov)
// Set the tooltips of the differents widgets
function LovWidget_setTooltips(text,prev,next,refresh,search,chunk,lov)
{
	var o=this;
	if ( o.showTextField )
		o.textField.setTooltip(text);
	o.prevChunkIcn.setTooltip(prev);
	o.nextChunkIcn.setTooltip(next);
	if (o.showRefreshIcn)
		o.refreshIcn.setTooltip(refresh);
	//o.searchIcn.setTooltip(search);	
	o.lovSearch.searchIcn.setTooltip(search);
	o.chunkCombo.setTooltip(chunk);
	o.lovList.setTooltip(lov);
}

// LovWidget_goToChunk(step)
// Move to a specified chunk
function LovWidget_goToChunk(step)
{
	var o=this;
	if ( o.chunkCombo.getSelection() == null )
		return;
		
	var curChunk = o.chunkCombo.getSelection().index;
	
	// If click on next or previous icon,
	// select the corresponding item in the combobox
	if (step != null)
		o.chunkCombo.select(curChunk+step);
	
	// set the previous and next icon state
	o.setPrevNextIconState();
	
	// Call the user callBack
	if (o.moveCB != null)
		o.moveCB();			
}

// function LovWidget_setPrevNextIconState
// Enables or disables the next and previous icon
// depending on the item number selected in the LOV list
function LovWidget_setPrevNextIconState()
{
	var o=this;
	
	//if combo is empty the disable combo and buttons
	if(!o.chunkCombo.getSelection())
	{
		o.chunkCombo.setDisabled(true);
		o.prevChunkIcn.setDisabled(true);
		o.nextChunkIcn.setDisabled(true);	
	}
	else
	{
		o.chunkCombo.setDisabled(false);
	}
		
	var curChunk = o.chunkCombo.getSelection().index;
	
	if ( o.chunkCombo.getCount() == 1 )
	{
		o.prevChunkIcn.setDisabled(true);
		o.nextChunkIcn.setDisabled(true);
		return;
	}
	
	if ( curChunk == 0 )
	{
		o.prevChunkIcn.setDisabled(true);
		o.nextChunkIcn.setDisabled(false);
		return;
	}
	
	if ( (curChunk+1) == o.chunkCombo.getCount() )
	{
		o.prevChunkIcn.setDisabled(false);
		o.nextChunkIcn.setDisabled(true);
		return;
	}
	
	o.prevChunkIcn.setDisabled(false);
	o.nextChunkIcn.setDisabled(false);	
}

// function LovWidget_change
// modify the UI of the widget
function LovWidget_change(label,w,lines,showText,showRefresh,moveCB,refreshCB,searchCB,
	dblClickCB,multi,enterCB,showDate,showTextOnly)
// label		[String]	the label of the chunk
// w			[int]		widget width, including borders
// lines		[int]		number of visible lines in the LOV list 
//							IF SET TO 0 ONLY THE TEXTFIELD IS VISIBLE
//							(LOVWIDGET equals a TEXTFIELDWIDGET IN THIS CASE)
// showText		[boolean]	display a textFieldWidget if true
// showRefresh	[boolean]	display a refresh icon if true
// moveCB		[function]	callback when click on the left arrow, combobox, right arrow
// refreshCB	[function]	callback when clicking on the refresh icon
// searchCB		[function]	callback when clicking on ENTER or on search icon
// dlbClickCB	[function]	callback when double clicking on the lov list
// multi		[boolean]	list is multi "selectionable" if true
// showDate		[boolean]	display a calendar widget if true
{
	var  o=this
	
	// properties
	if(showText!=null)
	{
		o.showTextField			= showText;
		o.textField.setDisplay(o.showTextField);
	}
	if(showRefresh!=null)
	{
		o.showRefreshIcn		= showRefresh;
		o.refreshIcn.setDisplay(o.showRefreshIcn);
		o.updateWidget();//to initialize o.noChunkLabel		
	}
	if(label!=null)
	{
		o.chunkText				= label;
		o.chunkLabel.setHTML(o.chunkText);
	}
	if(showDate!=null)
	{
		o.showDate				= showDate;
		o.dateField.setDisplay(o.showDate);
		if(o.showDate)
			o.textField.setDisplay(false);
	}
	// methods		
	if(moveCB!=null)
		o.moveCB				= moveCB;
	
	if(refreshCB!=null)
		o.refreshCB				= refreshCB;
	
	if(searchCB!=null)
		o.searchCB				= searchCB;
	
	if(dblClickCB!=null)
		o.dblClickCB			= dblClickCB;
	
	if(enterCB!=null)
		o.enterCB				= enterCB;

	//change list multi selection and lines
	o.lovList.change(multi,lines)				
	
	o.showTextOnly				= showTextOnly==null?false:showTextOnly;
	o.showTextFieldOnly(o.showTextOnly)
	if (o.showTextOnly) o.lovList.del() //FR 109490 
	// change w ? if(w!=null)
	
}

// function isCaseSensitive
// indicates if the option is checked
function LovWidget_isCaseSensitive()
// return	[boolean]	true if the option is checked
{
	return this.lovSearch.isCaseSensitive()
}

// function setCaseSensitive
// check or uncheck the match case checkbox
// update the icon
function LovWidget_setCaseSensitive(b)
// b	[boolean]	used as is
{
	var o=this
	
	o.lovSearch.setCaseSensitive(b);
}

//---------------------------------------------------------------------------------------
//calback functions
//---------------------------------------------------------------------------------------
function LovWidget_prevCB()
{
	this.par.goToChunk(-1);
}

function LovWidget_nextCB()
{
	this.par.goToChunk(+1);
}

function LovWidget_comboCB()
{
	this.par.goToChunk(null);
}

function LovWidget_refreshCB()
{
	var p=this.par;
	
	if (p.refreshCB != null) 
		p.refreshCB();
}

function LovWidget_searchCB()
{
	var p=this.par;
	
	if (p.searchCB != null)
		p.searchCB();
}

function LovWidget_enterCB(e)
{
	var p=this.par;
	
	if (p.enterCB != null)
		p.enterCB(e);
}

function LovWidget_ListKeyUpCB(e)
{
	var p=this.par;
	var k=eventGetKey(e);
	if ((eventGetKey(e) == 13)&& (p.enterCB != null))
		p.enterCB(e);
}

function LovWidget_dblClickCB()
{
	var p=this.par;
	
	if (p.dblClickCB != null)
		p.dblClickCB();
}

function LovWidget_clickCB()
{
}
/*
function LovWidget_normalClickCB()
{
	var p=this.par.parIcon.par
	
	var b=this.isChecked()
	if (b)
		p.searchIcn.icon.changeImg(0,0)		// normal icon
	else
		p.searchIcn.icon.changeImg(55,0)	// match case icon
	p.matchCase.check(!b)		
}

function LovWidget_matchCaseClickCB()
{
	var p=this.par.parIcon.par
	
	var b=this.isChecked()
	
	if (b)
		p.searchIcn.icon.changeImg(55,0)	// match case icon
	else
		p.searchIcn.icon.changeImg(0,0)		// normal icon
		
	p.normal.check(!b)		
}

function LovWidget_keyUpCB()
{
	var p=this.par
		
	p.searchIcn.setDisabled((this.getValue()==''))
}
*/
function LovWidget_showTextFieldOnly(bVal)
{
	var o=this;

	o.showTextOnly=bVal

	if(o.showTextOnly == true)
	{
		o.textField.setDisplay(true);
		o.dateField.setDisplay(false);
		o.chunkLabel.setDisplay(false);
		o.prevChunkIcn.setDisplay(false);
		o.chunkCombo.setDisplay(false);
		o.nextChunkIcn.setDisplay(false);		
		o.refreshIcn.setDisplay(false);
		o.noChunkLabel.setDisplay(false);
		o.lovList.setDisplay(false);
		o.lovSearch.setDisplay(false);
		//o.searchField.setDisplay(false);
		//o.searchIcn.setDisplay(false);
	}
	else
	{
		o.textField.setDisplay(o.showTextField);
		o.dateField.setDisplay(o.showDate);
		if(o.showDate) o.textField.setDisplay(false);
		
		o.chunkLabel.setDisplay(true);
		o.prevChunkIcn.setDisplay(true);
		o.chunkCombo.setDisplay(true);
		o.nextChunkIcn.setDisplay(true);
		o.refreshIcn.setDisplay(o.showRefreshIcn);
		//be carefull o.noChunkLabel depends on the combo size ===>function updateWidget()
		o.lovList.setDisplay(true);
		o.lovSearch.setDisplay(true);
		//o.searchField.setDisplay(true);
		//o.searchIcn.setDisplay(true);
		
		// Update the widget to take the combo size into account
		o.updateWidget()
	}
}

//set the input format for calendar widget
function LovWidget_setFormatDate(format,arrDays,arrMonth,AM,PM)
{
	var o=this
	if(o.showDate && o.dateField)	
		o.dateField.setFormatInfo(format,arrDays,arrMonth,AM,PM);
}

function LovWidget_updateWidget()
{
	var o=this
	var b=(o.chunkCombo.getCount()>1)
	
	o.chunkLabel.setDisplay(b)	
	o.prevChunkIcn.setDisplay(b)	
	o.chunkCombo.setDisplay(b)
	o.nextChunkIcn.setDisplay(b)
	
	//o.noChunkLabel depends on the combo size and the display of the refresh button
	o.noChunkLabel.setDisplay(!b && o.showRefreshIcn)
}
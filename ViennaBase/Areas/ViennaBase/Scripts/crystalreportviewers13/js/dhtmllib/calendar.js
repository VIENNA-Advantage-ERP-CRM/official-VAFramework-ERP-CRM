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

File: calendar.js

palette.js, menu.js and dom.js must also be included before

=============================================================
*/



_firstWeekDay=0
_dateObj=new Date

// ================================================================================
// ================================================================================
//
// OBJECT newCalendarTextField (Constructor)
//
// creates a calendar text field and button widget
//
// ================================================================================
// ================================================================================

function newCalendarTextFieldButton(id,textId,changeCB,maxChar,keyUpCB,enterCB,noMargin,tooltip,width,focusCB,blurCB,format,arrDays,arrMonth,AM,PM)
{
	var o=newWidget(id)

	o.changeCB=changeCB
	o.keyUpCB=keyUpCB
	o.enterCB=enterCB
	o.focusCB=focusCB
	o.blurCB=blurCB
	o.getValue=CTFB_getValue
	o.setValue=CTFB_setValue
	o.width=width?width:150

	o.setFormatInfo=CTFB_setFormatInfo
	o.setFormatInfo(format,arrDays,arrMonth,AM,PM)

	o.widResize=o.resize
	o.resize=CTFB_resize
	o.text=newTextFieldWidget(textId?textId:"text_"+id,CTFB_changeCB,maxChar,CTFB_keyUpCB,CTFB_enterCB,noMargin,tooltip,width-22,CTFB_focusCB,CTFB_blurCB)
	o.text.ctfb=o

	o.calendar=newCalendarButton("calendar_"+id,CTFB_CalendarChangeCB)
	o.calendar.ctfb=o
	o.calendar.menu.alignLeft=true

	o.getHTML=CTFB_getHTML
	o.oldInit=o.init
	o.init=CTFB_init
	o.setDateValueFromString=CTFB_setDateValueFromString
	o.setStringFromDateValue=CTFB_setStringFromDateValue



	return o
}

// ================================================================================

function CTFB_setFormatInfo(format,arrDays,arrMonth,AM,PM)
{
	var o=this

	o.arrMonthNames=arrMonth?arrMonth:_month
	o.arrDayNames=arrDays?arrDays:_day
	o.format=format?format:"MM/dd/yyyy hh:mm:ss a"
	o.AM=AM?AM:_AM
	o.PM=PM?PM:_PM
}

// ================================================================================

function CTFB_resize(w,h)
{
	var o=this
	
	o.text.resize(w!=null?(Math.max(0,w-22)):null,h)
	o.widResize(w,h)
}

// ================================================================================

function CTFB_getHTML()
{
	var o=this
	return '<table cellpadding="0" border="0" cellspacing="0" id="'+o.id+'" style="width:'+o.width+'px"><tr valign="middle"><td width="100%">'+o.text.getHTML()+'</td><td>'+o.calendar.getHTML()+'</td></tr></table>'
}

// ================================================================================

function CTFB_init()
{
	var o=this
	o.oldInit()
	o.text.init()
	o.calendar.init()
}

// ================================================================================

function CTFB_changeCB()
// PRIVATE
{
	if (this.ctfb.changeCB) this.ctfb.changeCB()
}

// ================================================================================

function CTFB_keyUpCB()
// PRIVATE
{
	if (this.ctfb.keyUpCB) this.ctfb.keyUpCB()
}

// ================================================================================

function CTFB_enterCB()
// PRIVATE
{
	if (this.ctfb.enterCB) this.ctfb.enterCB()
}

// ================================================================================

function CTFB_focusCB()
// PRIVATE
{
	if (this.ctfb.focusCB) this.ctfb.focusCB()
}

// ================================================================================

function CTFB_blurCB()
// PRIVATE
{
	if (this.ctfb.blurCB) this.ctfb.blurCB()
}

// ================================================================================

function CTFB_getValue()
{
	return this.text.getValue()
}

// ================================================================================

function CTFB_setValue(v)
{
	this.text.setValue(v)
}

// ================================================================================

function CTFB_CalendarChangeCB()
// PRIVATE
{
	var c=this,o=c.ctfb

	o.setStringFromDateValue()

	var v=c.get()

	if (o.changeCB)
		o.changeCB()
}

// ================================================================================

function CTFB_setDateValueFromString()
{
	var o=this
	var strDateValue=o.getValue()
	var strInputFormat=o.format
	
	var strRet = setDateValue(strDateValue, strInputFormat);
	
	if(strRet == ",,")
		o.calendar.menu.setToday(true);
	else
	{
		var arr = strRet.split(",");//strRet = strMonth + ',' + strDay + ',' + strYear;	
		var strDay=arr[1],strMonth=arr[0],strYear=arr[2];
		
		o.calendar.set(parseInt(strDay), parseInt(strMonth), parseInt(strYear))
		o.calendar.menu.update()
	}	
}

// ================================================================================

function CTFB_setStringFromDateValue() {
	var o=this
	var format=""+o.format
		
	var date=_dateObj
	var menu=o.calendar.menu
	//date.setDate(menu.day+1)

	date.setYear(menu.year)
	date.setMonth(menu.month)
	date.setDate(menu.day+1)

	date.setHours(0)
	date.setMinutes(0)
	date.setSeconds(0)
	
	var result=formatDate(date,format);
	o.setValue(result)
}



// ================================================================================
// ================================================================================
//
// OBJECT newCalendarButton (Constructor)
//
// creates a calendar button widget
//
// ================================================================================
// ================================================================================

function newCalendarButton(id,changeCB)
// CONSTRUCTOR
{
	var o=newIconWidget(id,_skin+"menus.gif",IconCalendarMenuWidget_ClickCB,null,_openCalendarLab,16,16,0,114,0,114)	

	o.setClasses("iconhover", "iconcheck", "iconhover", "iconcheckhover")

	o.changeCB=changeCB
	o.menu=newCalendarMenuWidget("iconMenu_menu_"+id,IconCalendarMenuWidget_hideCB,IconCalendarMenuWidget_closeCB)
	o.menu.parCalendar=o

	o.set=CalendarButton_set
	o.get=CalendarButton_get

	o.getMenu=IconMenuWidget_getMenu

	return o
}

// ================================================================================
// ================================================================================
//
// OBJECT newCalendarIconButton (Constructor)
//
// creates a calendar icon button widget
// 
// ================================================================================
// ================================================================================

function newCalendarIconButton(id,src,changeCB,text,alt,w,h,dx,dy,disDx,disDy)
// CONSTRUCTOR
{
	var o=newIconMenuWidget(id,src,IconCalendarMenuWidget_ClickCB,text,_openCalendarLab,w,h,dx,dy,disDx,disDy,false)	
	
	o.changeCB=changeCB
	o.menu=newCalendarMenuWidget("iconMenu_menu_"+id,IconCalendarMenuWidget_hideCB,IconMenuCalendarMenuWidget_closeCB)
	o.menu.parCalendar=o

	o.set=CalendarButton_set
	o.get=CalendarButton_get

	o.getMenu=IconMenuWidget_getMenu

	return o
}

function IconMenuCalendarMenuWidget_closeCB()
{
	var menu=this,o=menu.parCalendar
	o.outEnable=true
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

function IconCalendarMenuWidget_ClickCB()
{
	var o=this,l=o.layer
	o.outEnable=false

	if (o.ctfb)
		o.ctfb.setDateValueFromString()

	o.menu.show(true,getPosScrolled(l).x  + (o.menu.alignLeft?o.getWidth():0) ,getPosScrolled(l).y+o.getHeight()+1)
	IconWidget_overCB(o.index)
}

// ================================================================================

function IconCalendarMenuWidget_hideCB()
{
	var o=this.parCalendar

	if (o.changeCB)
		o.changeCB()
}

// ================================================================================

function IconCalendarMenuWidget_closeCB()
{
	var menu=this,o=menu.parCalendar
	o.outEnable=true
	if (!o.menu.isShown())
		IconWidget_outCB(o.index)
	else
		IconWidget_overCB(o.index)

}

// ================================================================================

function CalendarButton_set(day,month,year)
// day  [int] 1..31
// month [int] 1..12
// year [int] year
{
	var o=this.menu
	o.day=day?day-1:0
	o.month=month?month-1:0
	o.year=year?year:2000

	o.day=Math.min(Math.max(o.day,0),30)
	o.month=Math.min(Math.max(o.month,0),11)
}

// ================================================================================

function CalendarButton_get(day,month,year)
{
	var o=this.menu
	return {day:o.day+1,month:o.month+1,year:o.year}
}

// ================================================================================
// ================================================================================
//
// OBJECT newCalendarMenuWidget (Constructor)
//
// creates a calendar menu widget
//
// ================================================================================
// ================================================================================

function newCalendarWidget(id,changeCB)
// id            [String] the calendar id for DHTML processing
// Returns       [CalendarWidget] the new instance
{
	var o=newWidget(id)

	// Private Fields
	o.day=0
	o.month=0
	o.year=2000
	o.daysL=new Array
	o.changeCB=changeCB

	var p1=o.p1=newIconWidget(id+"_p1",_skin+'../lov.gif',CalendarWidget_clickCB,"",_calendarPrevMonthLab,8,4,46,12)
	var p2=o.p2=newIconWidget(id+"_p2",_skin+'../lov.gif',CalendarWidget_clickCB,"",_calendarPrevYearLab,8,4,46,12)
	var n1=o.n1=newIconWidget(id+"_n1",_skin+'../lov.gif',CalendarWidget_clickCB,"",_calendarNextMonthLab,8,4,46,0)
	var n2=o.n2=newIconWidget(id+"_n2",_skin+'../lov.gif',CalendarWidget_clickCB,"",_calendarNextYearLab,8,4,46,0)
	
	p1.allowDblClick=true
	p2.allowDblClick=true
	n1.allowDblClick=true
	n2.allowDblClick=true

	p1.margin=p2.margin=n1.margin=n2.margin=0
	var t=o.today=newButtonWidget(id+"_t",_today,CalendarWidget_clickCB)

	t.par=p1.par=p2.par=n1.par=n2.par=o
	t.today=p1.p1=p2.p2=n1.n1=n2.n2=true

	// Public Methods
	o.init=CalendarWidget_init
	o.getHTML=CalendarWidget_getHTML

	// Private methods
	o.update=CalendarWidget_update
	o.setToday=CalendarWidget_setToday
	o.set=CalendarWidget_set
	o.get=CalendarWidget_get

	o.focus=CalendarWidget_focus
	o.focusOnDay=CalendarWidget_focusOnDay
	
	o.isCalendar=true
	
	return o
}

// ================================================================================

function CalendarWidget_set(day,month,year)
// day  [int] 1..31
// month [int] 1..12
// year [int] year
{
	var o=this
	o.day=day?day-1:0
	o.month=month?month-1:0
	o.year=year?year:2000

	o.day=Math.min(Math.max(o.day,0),30)
	o.month=Math.min(Math.max(o.month,0),11)
}

// ================================================================================

function CalendarWidget_get(day,month,year)
{
	var o=this
	return {day:o.day+1,month:o.month+1,year:o.year}
}

// ================================================================================

function CalendarWidget_init()
// Redefined for disable the default widget init function
// Return [void]
{
	var o=this
	o.p1.init()
	o.p2.init()
	o.n1.init()
	o.n2.init()
	o.today.init()

	var l=o.layer=getLayer(o.id)
	o.layer._widget=o.widx
	o.css=o.layer.style	
	for (var i=0;i<42;i++)
		o.daysL[i]=getElemBySub(l,""+i)

	o.update()
}

// ================================================================================

function CalendarWidget_getHTML()
{
	var o=this,d=_moz?10:12
	
	var keysCbs=' onkeydown="'+_codeWinName+'.MenuWidget_keyDown(\''+o.id+'\',event);return true" '
	
	var s='<table onmousedown="'+_codeWinName+'._minb(event)" onmouseup="'+_codeWinName+'._minb(event)" '+keysCbs+' style="cursor:default" class="calendarFrame" id="'+o.id+'" border="0" cellpadding="0" cellspacing="0">'+

		// Month year zone
		'<tr>'+
			'<td align="center" style="padding:1px" >'+
				'<table class="dialogzone" width="100%" cellpadding="0" border="0" cellspacing="0">'+
					'<tr>'+
						'<td style="padding-top:1px">'+o.n1.getHTML()+'</td>'+
						'<td rowspan="2" width="100%" align="center" class="dialogzone"><span subid="month" tabIndex=0>'+convStr(_month[o.month])+'</span>&nbsp;&nbsp;<span subid="year" tabIndex=0>'+convStr(o.year)+'</span></td>'+
						'<td style="padding-top:1px">'+o.n2.getHTML()+'</td>'+
					'</tr>'+
					'<tr valign="top">'+
						'<td style="padding-bottom:1px">'+o.p1.getHTML()+'</td><td style="padding-bottom:1px">'+o.p2.getHTML()+'</td>'+
					'</tr>'+
				'</table>'+
			'</td>'+
		'</tr>'

		// Days header
		s+='<tr><td align="center"><table style="margin:2px;margin-top:6px" cellpadding="0" border="0" cellspacing="0">'
		for (var i=0;i<7;i++)
		{
			var j=(i+_firstWeekDay)%7
			if ((j%7)==0) s+='<tr align="center">'
				s+='<td class="calendarTextPart">'+_day[j]+'</td>'

			if ((j%7)==6) s+='</tr>'
		}
		s+='<tr><td colspan="7" style="padding:2px;"><table width="100%" height="3" cellpadding="0" cellspacing="0" border="0" style="'+backImgOffset(_skin+"menus.gif",0,80)+';"><tbody><tr><td></td></tr></tbody></table></td></tr>'

		// Days
		for (var i=0;i<6;i++)
		{
			s+='<tr align="right">'
			for (var j=0;j<7;j++)
			{				
				s+='<td class="calendarTextPart" onmousedown="'+_codeWinName+'.CalendarWidget_mouseDown(this)" onmouseover="'+_codeWinName+'.CalendarWidget_mouseOver(this)" onmouseout="'+_codeWinName+'.CalendarWidget_mouseOut(this)" onkeydown="'+_codeWinName+'.CalendarWidget_keyDown(this,event)">'+
					'<div subid="'+(j+(i*7))+'" class="menuCalendar"></div></td>'					
			}

			s+='</tr>'
		}

		s+='<tr><td colspan="7" style="padding:2px;"><table width="100%" height="3" cellpadding="0" cellspacing="0" border="0" style="'+backImgOffset(_skin+"menus.gif",0,80)+';"><tbody><tr><td></td></tr></tbody></table></td></tr>'
		s+='<tr><td colspan="7" align="center" style="padding-bottom=3px;padding-top:3px;">'+o.today.getHTML()+'</td></tr>'

		// End menu
		s+='</table></td></tr>'
		s+='</table>'

	return s
}

// ================================================================================

function CalendarWidget_mouseDown(lay)
{
	lay=lay.childNodes[0]
	var o=getWidget(lay),index=parseInt(lay.getAttribute("subid"))

	var day=index-o.dateOffset

	if ((day<0)||(day>o.lastDate))
		return

	o.day=day
	o.update()
	if (o.changeCB)
		o.changeCB()
	
}

// ================================================================================

function CalendarWidget_clickCB()
// PRIVATE
{
	var o=m=this

	// Modify current month
	if (o.p1||o.n1)
	{
		m=o.par
		m.month=(m.month+(o.p1?-1:1))%12
		if (m.month==-1)
			m.month=11
		m.update()
	}

	// Modify current year
	if (o.p2||o.n2)
	{
		m=o.par
		m.year=m.year+(o.p2?-1:1)
		m.update()
	}
	if (o.today)	
	{
		m=o.par
		m.setToday(true)			
		if (m.changeCB)
			m.changeCB()
	}
}


// ================================================================================
// ================================================================================
//
// OBJECT newCalendarMenuWidget (Constructor)
//
// creates a calendar menu widget
//
// ================================================================================
// ================================================================================

function newCalendarMenuWidget(id,changeCB,closeCB)
// id            [String] the calendar id for DHTML processing
// Returns       [CalendarMenuWidget] the new instance
{
	var o=newMenuWidget(id,closeCB)

	// Private Fields
	o.day=0
	o.month=0
	o.year=2000
	o.daysL=new Array
	o.changeCB=changeCB

	var p1=o.p1=newIconWidget(id+"_p1",_skin+'../lov.gif',CalendarMenuWidget_clickCB,"",_calendarPrevMonthLab,8,4,46,12)
	var p2=o.p2=newIconWidget(id+"_p2",_skin+'../lov.gif',CalendarMenuWidget_clickCB,"",_calendarPrevYearLab,8,4,46,12)
	var n1=o.n1=newIconWidget(id+"_n1",_skin+'../lov.gif',CalendarMenuWidget_clickCB,"",_calendarNextMonthLab,8,4,46,0)
	var n2=o.n2=newIconWidget(id+"_n2",_skin+'../lov.gif',CalendarMenuWidget_clickCB,"",_calendarNextYearLab,8,4,46,0)
	
	p1.allowDblClick=true
	p2.allowDblClick=true
	n1.allowDblClick=true
	n2.allowDblClick=true

	p1.margin=p2.margin=n1.margin=n2.margin=0
	var t=o.today=newButtonWidget(id+"_t",_today,CalendarMenuWidget_clickCB)

	t.par=p1.par=p2.par=n1.par=n2.par=o
	t.today=p1.p1=p2.p2=n1.n1=n2.n2=true

	// Public Methods
	o.getHTML=CalendarMenuWidget_getHTML
	o.oldMenuInit=o.justInTimeInit
	o.justInTimeInit=CalendarMenuWidget_justInTimeInit

	// Private methods
	o.update=CalendarWidget_update
	o.setToday=CalendarWidget_setToday

	o.focus=CalendarWidget_focus
	o.focusOnDay=CalendarWidget_focusOnDay
	
	o.isCalendar=true
	
	return o
}

// ================================================================================

function CalendarMenuWidget_justInTimeInit()
{
	var o=this
	o.oldMenuInit()
	o.p1.init()
	o.p2.init()
	o.n1.init()
	o.n2.init()
	o.today.init()

	var l=o.layer
	for (var i=0;i<42;i++)
		o.daysL[i]=getElemBySub(l,""+i)

	o.update()
}

// ================================================================================

function CalendarMenuWidget_getHTML()
{
	var o=this,d=_moz?10:12

	var keysCbs=' onkeydown="'+_codeWinName+'.MenuWidget_keyDown(\''+o.id+'\',event);return true" '
	
	//var s=o.getShadowHTML()
	var s=''
	s+='<a style="position:absolute;left:-30px;top:-30px; visibility:hidden" id="startLink_'+o.id+'" href="javascript:void(0)" onfocus="'+_codeWinName+'.MenuWidget_keepFocus(\''+o.id+'\');return false;" ></a>'
	s+='<table onmousedown="'+_codeWinName+'._minb(event)" onmouseup="'+_codeWinName+'._minb(event)" '+keysCbs+' style="cursor:default" class="menuFrame" id="'+o.id+'" border="0" cellpadding="0" cellspacing="0">'+

		// Month year zone
		'<tr>'+
			'<td align="center" style="padding:1px" >'+
				'<table class="dialogzone" width="100%" cellpadding="0" border="0" cellspacing="0">'+
					'<tr>'+
						'<td style="padding-top:1px">'+o.n1.getHTML()+'</td>'+
						'<td rowspan="2" width="100%" align="center" class="dialogzone"><span subid="month" tabIndex=0>'+convStr(_month[o.month])+'</span>&nbsp;&nbsp;<span subid="year" tabIndex=0>'+convStr(o.year)+'</span></td>'+
						'<td style="padding-top:1px">'+o.n2.getHTML()+'</td>'+
					'</tr>'+
					'<tr valign="top">'+
						'<td style="padding-bottom:1px">'+o.p1.getHTML()+'</td><td style="padding-bottom:1px">'+o.p2.getHTML()+'</td>'+
					'</tr>'+
				'</table>'+
			'</td>'+
		'</tr>'

		// Days header
		s+='<tr><td align="center"><table style="margin:2px;margin-top:6px" cellpadding="0" border="0" cellspacing="0">'
		for (var i=0;i<7;i++)
		{
			var j=(i+_firstWeekDay)%7
			if ((j%7)==0) s+='<tr align="center">'
				s+='<td class="calendarTextPart">'+_day[j]+'</td>'

			if ((j%7)==6) s+='</tr>'
		}
		s+='<tr><td colspan="7" style="padding:2px;"><table width="100%" height="3" cellpadding="0" cellspacing="0" border="0" style="'+backImgOffset(_skin+"menus.gif",0,80)+';"><tbody><tr><td></td></tr></tbody></table></td></tr>'

		// Days
		for (var i=0;i<6;i++)
		{
			s+='<tr align="right">'
			for (var j=0;j<7;j++)
			{				
				s+='<td class="calendarTextPart" onmousedown="'+_codeWinName+'.CalendarMenuWidget_mouseDown(this)" onmouseover="'+_codeWinName+'.CalendarWidget_mouseOver(this)" onmouseout="'+_codeWinName+'.CalendarWidget_mouseOut(this)" onkeydown="'+_codeWinName+'.CalendarMenuWidget_keyDown(this,event)">'+
					'<div subid="'+(j+(i*7))+'" class="menuCalendar"></div></td>'					
			}

			s+='</tr>'
		}

		s+='<tr><td colspan="7" style="padding:2px;"><table width="100%" height="3" cellpadding="0" cellspacing="0" border="0" style="'+backImgOffset(_skin+"menus.gif",0,80)+';"><tbody><tr><td></td></tr></tbody></table></td></tr>'
		s+='<tr><td colspan="7" align="center" style="padding-bottom=3px;padding-top:3px;">'+o.today.getHTML()+'</td></tr>'



		// End menu
		s+='</table></td></tr>'
		s+='</table>'
		s+='<a style="position:absolute;left:-30px;top:-30px; visibility:hidden" id="endLink_'+o.id+'" href="javascript:void(0)" onfocus="'+_codeWinName+'.MenuWidget_keepFocus(\''+o.id+'\');return false;" ></a>'

	return s
}

// ================================================================================

function CalendarMenuWidget_clickCB()
// PRIVATE
{
	var o=m=this

	// Modify current month
	if (o.p1||o.n1)
	{
		m=o.par
		m.month=(m.month+(o.p1?-1:1))%12
		if (m.month==-1)
			m.month=11
		m.update()
	}

	// Modify current year
	if (o.p2||o.n2)
	{
		m=o.par
		m.year=m.year+(o.p2?-1:1)
		m.update()
	}
	if (o.today)	
	{
		m=o.par
		m.setToday(true)			
		m.show(false)
		if (m.changeCB)
			m.changeCB()
	
		//give focus to parent textfield when close menu
		var cal = m.parCalendar
		if( cal && cal.ctfb && cal.ctfb.text)
			cal.ctfb.text.focus()
		else
			cal.focus()		
	}
}

// ================================================================================

function CalendarWidget_update()
// PRIVATE
{
	var o=this
	if (o.layer==null)
		return
	var month=getElemBySub(o.layer,"month"),year=getElemBySub(o.layer,"year"),l=o.layer

	month.innerHTML=convStr(_month[o.month])
	year.innerHTML=convStr(""+o.year)


	_dateObj.setYear(o.year)
	_dateObj.setMonth(o.month)
	_dateObj.setDate(1)

	var offset=_dateObj.getDay()

	var last=26
	for (var i=26;i<33;i++)
	{
		_dateObj.setDate(i)
		if (_dateObj.getMonth()==o.month)
			last=i-1
		else
			break
	}

	o.lastDate=last
	o.dateOffset=offset

	o.day=Math.min(last,o.day)

	for (var i=0;i<42;i++)
	{
		var j=i-offset,lay=o.daysL[i],cName="menuCalendar"

		if ((j<0)||(j>last))
		{
			lay.innerHTML=""
			lay.tabIndex=-1 //cannot receive focus
		}
		else
		{
			lay.innerHTML=""+(j+1)
			lay.tabIndex=0 //can receive focus
			if (j==o.day)
			{
				cName="menuCalendarSel"
				lay.title=_calendarSelectionLab+(j+1)
			}
		}
		lay.className=cName
	}
}
// ================================================================================

function CalendarWidget_setToday(update)
{
	_dateObj=new Date
	var o=this
	o.day=_dateObj.getDate()-1
	o.month=_dateObj.getMonth()
	o.year=_dateObj.getFullYear()

	if (update)
		o.update()
}

// ================================================================================

function CalendarMenuWidget_mouseDown(lay)
{
	lay=lay.childNodes[0]
	var o=getWidget(lay),index=parseInt(lay.getAttribute("subid"))

	var day=index-o.dateOffset

	if ((day<0)||(day>o.lastDate))
		return

	o.day=day
	o.update()
	o.show(false)
	if (o.changeCB)
		o.changeCB()
	
	//give focus to parent textfield when close menu
	var cal = o.parCalendar
	if( cal && cal.ctfb && cal.ctfb.text)
		cal.ctfb.text.focus()
	else
		cal.focus()		
}

// ================================================================================

function CalendarWidget_mouseOver(lay)
{
	lay=lay.childNodes[0]
	var o=getWidget(lay),index=parseInt(lay.getAttribute("subid"))

	var day=index-o.dateOffset,cName=""

	if ((day<0)||(day>o.lastDate))
		cName="menuCalendar"
	else
		cName="menuCalendarSel"

	lay.className=cName
}

// ================================================================================

function CalendarWidget_mouseOut(lay)
{
	lay=lay.childNodes[0]
	var o=getWidget(lay),index=parseInt(lay.getAttribute("subid"))
	var day=index-o.dateOffset,cName=""

	if (o.day!=day)
		lay.className="menuCalendar"

}

// ================================================================================

function getElemBySub(l,subId)
{
	if (l.getAttribute&&(l.getAttribute("subid")==subId))
		return l

	var sub=l.childNodes,len=sub.length
	for (var i=0;i<len;i++)
	{
		var r=getElemBySub(sub[i],subId)
		if (r) return r
	}
	return null
}

// ================================================================================
function CalendarWidget_focus()
{
	var o=this	
	o.n1.focus()	
	if(o.endLink) o.endLink.show(true)
	if(o.startLink) o.startLink.show(true)
}

function CalendarWidget_focusOnDay()
{
	var o=this	
	o.day.focus()
}

function CalendarWidget_keyDown(lay,e)
{
	var k=eventGetKey(e)
	if(k==13)//enter
	{
		CalendarWidget_mouseDown(lay)
	}
}

function CalendarMenuWidget_keyDown(lay,e)
{
	var k=eventGetKey(e)
	if(k==13)//enter
	{
		CalendarMenuWidget_mouseDown(lay)
	}
}

/* Copyright (c) Business Objects 2006. All rights reserved. */

if (typeof bobj == 'undefined') {
    bobj = {};
}
if (typeof bobj.crv == 'undefined') {
    bobj.crv = {};
}
if (typeof bobj.crv.Calendar == 'undefined') {
    bobj.crv.Calendar = {};
}

/*
================================================================================
Calendar Widget
================================================================================
*/

/**
 * Get a shared calendar instance 
 */
bobj.crv.Calendar.getInstance = function() {
    if (!bobj.crv.Calendar.__instance) {
        bobj.crv.Calendar.__instance = bobj.crv.newCalendar();    
    }
    return bobj.crv.Calendar.__instance;
};

bobj.crv.Calendar.Signals = {
    OK_CLICK: 'okClick',
    CANCEL_CLICK: 'cancelClick',
    ON_HIDE: 'onHide' 
};

bobj.crv.newCalendar = function(kwArgs) {
    var UPDATE = MochiKit.Base.update;
    kwArgs = UPDATE({
        id: bobj.uniqueId() + "_calendar",
        showTime: false,
        date: new Date(),
        // List of formats to match in order of preference. Once a format is 
        // matched, the time field will be displayed in that format.
        timeFormats: ["HH:mm:ss", "H:mm:ss", "H:m:s", "HH:mm", "H:mm", "H:m", 
            "h:mm:ss a", "h:m:s a", "h:mm:ssa", "h:m:sa", "h:mm a", "h:m a", 
            "h:mma", "h:ma"]
    }, kwArgs);
    
    var o = newMenuWidget( );
        
    o.widgetType = 'Calendar';
    
    // Update instance with constructor arguments
    bobj.fillIn(o, kwArgs);
    
    // Update instance with member functions
    o._menuJustInTimeInit = o.justInTimeInit;
    UPDATE(o, bobj.crv.Calendar);
    
    o._curTimeFormat = o.timeFormats[0];
    o._cells = [];
    o._firstDay = 0;
    o._numDays = 0;
    
    return o;
};

bobj.crv.Calendar._createHeaderButtons = function() {
    var w = 8;
    var h = 4;
    var dx = 46;
    var dyUp = 0;
    var dyDown = 12;
    
    var bind = MochiKit.Base.bind;
    
    this._prevMonthBtn = newIconWidget(this.id+"_pm",_skin+'../lov.gif',bind(this._onPrevMonthClick, this),"",_calendarPrevMonthLab,w,h,dx,dyDown);
    this._prevYearBtn = newIconWidget(this.id+"_py",_skin+'../lov.gif',bind(this._onPrevYearClick, this),"",_calendarPrevYearLab,w,h,dx,dyDown);
    this._nextMonthBtn = newIconWidget(this.id+"_nm",_skin+'../lov.gif',bind(this._onNextMonthClick, this),"",_calendarNextMonthLab,w,h,dx,dyUp);
    this._nextYearBtn = newIconWidget(this.id+"_ny",_skin+'../lov.gif',bind(this._onNextYearClick, this),"",_calendarNextYearLab,w,h,dx,dyUp);
    
    this._prevMonthBtn.allowDblClick = true;
    this._prevYearBtn.allowDblClick = true;
    this._nextMonthBtn.allowDblClick = true;
    this._nextYearBtn.allowDblClick = true;
};

bobj.crv.Calendar._createTimeTextField = function() {
    var bind = MochiKit.Base.bind;
    
    this._timeField = newTextFieldWidget(
        this.id + '_time',
        bind(this._onTimeChange, this),  //changeCB
        null,  //maxChar
        null,  //keyUpCB
        null,  //enterCB
        true,  //noMargin
        null,  //tooltip
        null,  //width
        null,  //focusCB
        null); //blurCB    
};

bobj.crv.Calendar._createOKCancelButtons = function() {
    var bind = MochiKit.Base.bind;
    this._okBtn = newButtonWidget(this.id + "_ok", L_bobj_crv_OK, bind(this._onOKClick, this));
    this._cancelBtn = newButtonWidget(this.id + "_cancel", L_bobj_crv_Cancel, bind(this._onCancelClick, this));
};

/**
 * Widget will auto-initialize the first time its show method is called.
 * Client code shoud not call this method.
 */
bobj.crv.Calendar.justInTimeInit = function() {
    this._menuJustInTimeInit();
    
    this._prevMonthBtn.init();
    this._prevYearBtn.init();
    this._nextMonthBtn.init();
    this._nextYearBtn.init();

    this._okBtn.init();
    this._cancelBtn.init();
    
    this._timeField.init();
    this._timeField.layer.style.width = '100%';
    this._timeField.setValue(bobj.external.date.formatDate(this.date, this._curTimeFormat));
    
    this._timeRow = getLayer(this.id + '_timeRow');
    this._timeSep = getLayer(this.id + '_timeSep');
    
    this._month = getLayer(this.id + "_month");
    this._year = getLayer(this.id + "_year");
    
    var numCells = 6 * 7; // six rows in the calendar with 7 days each
    for (var i = 0; i < numCells; i++) {
        this._cells[i] = getLayer(this.id + '_c' + i);
    }
    
    this._update(); 
};

/**
 * Widget will be written into the document the first time its show method is called.
 * Client code shoud not call this method.
 */
bobj.crv.Calendar.getHTML = function() {
    var h = bobj.html;
    var TABLE = h.TABLE;
    var TBODY = h.TBODY;
    var TR = h.TR;
    var TD = h.TD;
    var DIV = h.DIV;
    var SPAN = h.SPAN;
    var A = h.A;
    
    this._createHeaderButtons();
    this._createTimeTextField();
    this._createOKCancelButtons();

    var onkeydown = "MenuWidget_keyDown('" + this.id + "', event); return true";
    var onmousedown = "eventCancelBubble(event)";
    var onmouseup = "eventCancelBubble(event)";
    var onkeypress = "eventCancelBubble(event)";
    
    var dayHeaderAtt = {'class':"calendarTextPart"};
   
    var html = TABLE({dir: 'ltr', id: this.id, border:"0", cellpadding:"0", cellspacing:"0",
        onkeydown: onkeydown, onmousedown: onmousedown, onmouseup: onmouseup, onkeypress: onkeypress,
        'class':"menuFrame", style:{cursor:"default", visibility:"hidden",'z-index': 10000}},
        TBODY(null, 
            TR(null, TD(null, this._getMonthYearHTML())),
            TR(null, TD({align:"center"},
                TABLE({border:"0", cellspacing:"0", cellpadding:"0", style:{margin:"2px", 'margin-top': "6px"}},
                    TR({align:"center"},
                        TD(dayHeaderAtt, L_bobj_crv_SundayShort),
                        TD(dayHeaderAtt, L_bobj_crv_MondayShort),
                        TD(dayHeaderAtt, L_bobj_crv_TuesdayShort),
                        TD(dayHeaderAtt, L_bobj_crv_WednesdayShort),
                        TD(dayHeaderAtt, L_bobj_crv_ThursdayShort),
                        TD(dayHeaderAtt, L_bobj_crv_FridayShort),
                        TD(dayHeaderAtt, L_bobj_crv_SaturdayShort)),
                    TR(null, TD({colspan:"7", style:{padding:"2px"}}, this._getSeparatorHTML())),
                    this._getDaysHTML(),
                    TR(null, TD({colspan:"7", style:{padding:"2px"}}, this._getSeparatorHTML())),
                    TR({id:this.id + '_timeRow', style:{display:this.showTime ? '' : 'none'}},
                        TD({colspan:"7", style:{'padding-top':"3px", 'padding-bottom':"3px", 'padding-right':"10px", 'padding-left':"10px"}},
                            this._timeField.getHTML())),
                    TR({id:this.id + '_timeSep',style:{display:this.showTime ? '' : 'none'}}, 
                        TD({colspan:"7", style:{padding:"2px"}}, this._getSeparatorHTML())),  
                    TR(null, TD({colspan:"7", align:"right", style:{'padding-bottom':"3px", 'padding-top':"3px"}},
                        TABLE(null, TBODY(null, TR(null, 
                            TD(null, this._okBtn.getHTML()),
                            TD(null, this._cancelBtn.getHTML())))))))))));
                            
    return this._getLinkHTML('startLink_' + this.id) + html + this._getLinkHTML('endLink_' + this.id);                         
};

bobj.crv.Calendar._getMonthYearHTML = function() {
    var h = bobj.html;
    var TABLE = h.TABLE;
    var TBODY = h.TBODY;
    var TR = h.TR;
    var TD = h.TD;
    var DIV = h.DIV;
    var SPAN = h.SPAN;
    
    return TABLE({'class':"dialogzone", width:"100%", cellpadding:"0", cellspacing:"0"},
        TBODY(null,
            TR(null,
                TD({style:{'padding-top':"1px"}}, this._nextMonthBtn.getHTML()),
                TD({rowspan:"2", width:"100%", align:"center", 'class':"dialogzone"},
                    SPAN({id:this.id + "_month", tabIndex:"0"}, _month[this.date.getMonth()]),
                    "&nbsp;&nbsp;",
                    SPAN({id:this.id + "_year", tabIndex:"0"}, this.date.getFullYear())),
                TD({style:{'pading-top':"1px"}}, this._nextYearBtn.getHTML())),
            TR({valign:"top"},
                TD({style:{'padding-bottom':"1px"}}, this._prevMonthBtn.getHTML()),
                TD({style:{'padding-bottom':"1px"}}, this._prevYearBtn.getHTML()))));
};

bobj.crv.Calendar._getSeparatorHTML = function() {
    var h = bobj.html;
    var TABLE = h.TABLE;
    var TBODY = h.TBODY;
    var TR = h.TR;
    var TD = h.TD;
    
    return TABLE({width:"100%", 
                  height:"3", 
                  cellpadding:"0", 
                  cellspacing:"0", 
                  border:"0", 
                  style:backImgOffset(_skin+"menus.gif",0,80)},
        TBODY(null, TR(null, TD())));
};

bobj.crv.Calendar._getLinkHTML = function(id) {
    return bobj.html.A({
        id: id, 
        href: "javascript:void(0)", 
        onfocus: "MenuWidget_keepFocus('"+this.id+"')", 
        style:{
            visibility:"hidden", 
            position:"absolute"
    }});
};

bobj.crv.Calendar._getDaysHTML = function() {
    var TD = bobj.html.TD;
    var DIV = bobj.html.DIV;
    var html = '';
    
    for (i = 0; i < 6; ++i) {
        html += '<tr align="right">';
        
        for (j = 0; j < 7; ++j) {
            var cellNum = j + (i * 7);
            var eventArgs =  "(this," + cellNum + "," + "event);";
            
            html += TD({id: this.id + '_c' + (i * 7 + j), 
                        'class':"calendarTextPart", 
                        onmousedown: "bobj.crv.Calendar._onDayMouseDown" + eventArgs,
                        onmouseover: "bobj.crv.Calendar._onDayMouseOver" + eventArgs,
                        onmouseout:  "bobj.crv.Calendar._onDayMouseOut"  + eventArgs,
                        ondblclick:  "bobj.crv.Calendar._onDayDoubleClick" + eventArgs,
                        onkeydown:   "bobj.crv.Calendar._onDayKeyDown"   + eventArgs},
                DIV({'class':"menuCalendar"}));
        }

        html += '</tr>';
    }
    
    return html;
};

/**
 * Update the calendar's display using the current date value
 */
bobj.crv.Calendar._update = function() {
    var numCells = 6 * 7; // six rows in the calendar with 7 days each
    var curDate = this.date.getDate();
    
    var info = this._getMonthInfo(this.date.getMonth(), this.date.getFullYear());
    
    var firstCellInMonth = info.firstDay;
    this._firstDay = info.firstDay;
    this._numDays = info.numDays;
    
    var year = "" + this.date.getFullYear();
    while (year.length < 4) {
        year = "0" + year;    
    }
    this._year.innerHTML = year;
    this._month.innerHTML = _month[this.date.getMonth()];
    
    this._selectedDate = null;
    
    for (var cellNum = 0; cellNum < numCells; cellNum++) {
        var cell = this._cells[cellNum].firstChild;
        var cssClass = "menuCalendar";
        var cellDate = this._getDateFromCellNum(cellNum);
        
        if (cellDate < 1 || cellDate > info.numDays) {
            cell.innerHTML = "";
            cell.tabIndex = "-1"; 
        }
        else {
            cell.innerHTML = "" + cellDate;
            cell.tabIndex = "0";
            if (cellDate == curDate) {
                cssClass = "menuCalendarSel";
                this._selectedDay = cell;
            }
        }
        
        cell.className = cssClass;
    }
};

bobj.crv.Calendar._getMonthInfo = function(month, year) {
    var date = new Date();
    date.setDate(1);
    date.setFullYear(year);
    date.setMonth(month);
    
    var firstDay = date.getDay(); // First day of the week in this month
    
    date.setDate(28);
    var lastDate = 28; // Last date in this month
    
    for (var i = 29; i < 32; i++) {
        date.setDate(i);
        if (date.getMonth() != month) {
            break;
        }
        lastDate = i;
    }
    
    return {firstDay: firstDay, numDays: lastDate};
};

bobj.crv.Calendar._setDayOfMonth = function(date) {
    if (date > 0 && date <= this._numDays) {
        var prevDate = this.date.getDate(); 
        
        if (date != prevDate) {
            var prevCell = this._getCellFromDate(prevDate); 
        
            if (prevCell) {
                prevCell.firstChild.className = "menuCalendar";    
            }
            
            this._getCellFromDate(date).firstChild.className = "menuCalendarSel";
            this.date.setDate(date);
        }
    }
};

bobj.crv.Calendar._getCellFromDate = function(date) {
    var cellNum = date + this._firstDay - 1;
    return this._cells[cellNum];
};

bobj.crv.Calendar._getDateFromCellNum = function(cellNum) {
    return cellNum - this._firstDay + 1;
};

bobj.crv.Calendar._onDayMouseOver = function(node, cellNum, event) {
    var o = getWidget(node);
    var div = node.firstChild;
    
    var date = cellNum - o._firstDay + 1;
    if (date < 1 || date > o._numDays) {
        div.className = "menuCalendar";    
    }
    else {
        div.className = "menuCalendarSel";    
    }
};

bobj.crv.Calendar._onDayMouseOut = function(node, cellNum, event) {
    var o = getWidget(node);
    var div = node.firstChild;
    
    var date = cellNum - o._firstDay + 1; 
    
    if (date != o.date.getDate()) {
        div.className = "menuCalendar";  
    }
};

bobj.crv.Calendar._onDayMouseDown = function(node, cellNum, event) {
    var o = getWidget(node);
    var date = cellNum - o._firstDay + 1;
    o._setDayOfMonth(date);
};

bobj.crv.Calendar._onDayDoubleClick = function(node, cellNum, event) {
    var o = getWidget(node);
    o._onOKClick();
};

bobj.crv.Calendar._onDayKeyDown = function(node, cellNum, event) {
    event = new MochiKit.Signal.Event(node, event);
    var key = event.key().string; 
    if (key === "KEY_ENTER") {
        var o = getWidget(node);
        var date = cellNum - o._firstDay + 1;
        o._setDayOfMonth(date);
    }
};

bobj.crv.Calendar._onPrevMonthClick = function() {
    var d = this.date;
    var oldMonth = d.getMonth();
    if(d.getMonth() === 0) {
        d.setYear(d.getFullYear() -1);
        d.setMonth(11);
    }
    else {
        d.setMonth(d.getMonth() - 1);
        if (oldMonth === d.getMonth()) {
            // that means we have decremented 0 month instead of 1. This happens if the current date is Oct 31, for eg. Since there's no Sept 31, it jumps to October 1.
            d.setMonth(oldMonth-1);
        }
    }
    this._update();
};

bobj.crv.Calendar._onPrevYearClick = function() {
    this.date.setFullYear(this.date.getFullYear() - 1);
    this._update();
};

bobj.crv.Calendar._onNextMonthClick = function() {
    var d = this.date;
    var oldMonth = d.getMonth();
    d.setMonth(d.getMonth() + 1);
    if ((oldMonth+1) < d.getMonth()) {
        // that means we have incremented 2 months instead of 1. This happens if the current date is Oct 31, for eg. Since there's no Nov 31, it jumps to Dec 1.
        // For Dec 31, we don't need to worry because there's Jan 31.
        d.setMonth(oldMonth+1);
    }
    this._update();
};

bobj.crv.Calendar._onNextYearClick = function() {
    this.date.setFullYear(this.date.getFullYear() + 1);
    this._update();
};

bobj.crv.Calendar._onOKClick = function() {
    this.restoreFocus();
    MochiKit.Signal.signal(this, this.Signals.OK_CLICK, this._copyDate(this.date));  
    this.show(false);  
};

bobj.crv.Calendar._copyDate = function(date) {
    if (date) {
        return new Date(date.getFullYear(),
                            date.getMonth(),
                            date.getDate(),
                            date.getHours(),
                            date.getMinutes(),
                            date.getSeconds(),
                            date.getMilliseconds());
    }
    return new Date();
};

bobj.crv.Calendar._onCancelClick = function() {
    this.restoreFocus();
    this.show(false);
    MochiKit.Signal.signal(this, this.Signals.CANCEL_CLICK);
};

bobj.crv.Calendar._onTimeChange = function() {
    var text = this._timeField.getValue();
    var date = null;
    var format = null;
    
    for (var i = 0; i < this.timeFormats.length && date === null; ++i) {
        format = this.timeFormats[i];
        date = bobj.external.date.getDateFromFormat(text, format);
    }
    
    if (date) { 
        this._curTimeFormat = format;
        this.date.setHours(date.getHours());
        this.date.setMinutes(date.getMinutes());
        this.date.setSeconds(date.getSeconds());
        this.date.setMilliseconds(date.getMilliseconds());
    }
    else {
        this._timeField.setValue(bobj.external.date.formatDate(this.date, this._curTimeFormat));
    }
};

bobj.crv.Calendar.setShowTime = function(isShow) {
    var disp = isShow ? '' : 'none';
    this.showTime = isShow;
    if (this.layer) {
        this._timeRow.style.display = disp;
        this._timeSep.style.display = disp;
    }
};

bobj.crv.Calendar.setDate = function(date) {
    this.date = date;
    if (this.layer) {
        this._timeField.setValue(bobj.external.date.formatDate(this.date, this._curTimeFormat));
        this._update();
    }
};

/**
 * Show the calendar. Will write out the HTML and init the widget also.
 *
 * @param isShow [bool] Show calendar if true. Hide it if false.
 * @param x [int] x coordinate for left of calendar
 * @param y [int] y coordinate for top of calendar
 * @param isAlignRight [bool, optional] When true, the x coordinate applies to 
 *                                      the right edge of the calendar
 * @param isAlignBottom [bool, optional] When true, the y coordinate applies to 
 *                                      the bottom edge of the calendar
 */
bobj.crv.Calendar.show = function(isShow, x, y, isAlignRight, isAlignBottom) {
    ScrollMenuWidget_show.call(this, isShow, x, y);
    if(isShow) {
        this.focus();
    }
    else {
        MochiKit.Signal.signal(this, this.Signals.ON_HIDE);
    }
};

/**
 * Set focus on the Calendar. The currently selected day will receive focus.
 */
bobj.crv.Calendar.focus = function() {
    if (this._selectedDay) {
        this._selectedDay.focus();
    }
};
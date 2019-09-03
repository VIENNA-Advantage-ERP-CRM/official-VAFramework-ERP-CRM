/* Copyright (c) Business Objects 2006. All rights reserved. */

//////////////////////////////
// FOR DEBUGGING ONLY
var debug = false;
function dumpFormFields(formName)
{
    theForm = document.forms[formName];
    for ( idx = 0; idx < theForm.elements.length; ++idx )
        alert ( theForm.elements[idx].name + " - " + theForm.elements[idx].value );
}

//////////////////////////////
// GLOBAL VAR
var isJava = false;  // do encodeURIComponent for Java only

var isNetscape = navigator.appName.indexOf("Netscape") != -1;

var LEFT_ARROW_KEY = 37;
var RIGHT_ARROW_KEY = 39;
var ENTER_KEY = 13;

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
    form,
    type ,
    promptID)
{
    var widget, obj;
    widget = obj = form[promptID + "DiscreteValue"];
    if ( obj.type && obj.type.toLowerCase() != "text" &&
         obj.type.toLowerCase() != "hidden" && obj.type.toLowerCase() != "password")
    {
        //select box not a textbox
        obj = obj.options[obj.selectedIndex];
    }
    if ( ! promptengine_checkValue ( obj.value, type ) )
    {
        if (widget.focus && widget.type.toLowerCase() != "hidden")
            widget.focus();
        return false;
    }
    promptValue =  obj.value;
    displayString = ( obj.text ) ? obj.text : obj.value;
    promptEntry = new Option(displayString,promptValue,false,false);
    theList = form[promptID + "ListBox"];
    theList.options[theList.length] = promptEntry;
    if (widget.focus && widget.type.toLowerCase() != "hidden" && !widget.disabled)
        widget.focus ();
    if ( widget.select )
        widget.select ();
    if ( widget.type.toLowerCase != "text" &&
     widget.type.toLowerCase != "hidden" &&
     widget.type.toLowerCase != "password")
        if ( widget.selectedIndex < widget.length - 1 )
            widget.selectedIndex = widget.selectedIndex + 1;      //... or move to next selection in listbox
}

function promptengine_deselectAllItems(listbox)
{
    for (var i = 0; i < listbox.length; i++)
        listbox.options[i].selected = false;
}

function promptengine_addAvailableItem(
    availableList,
    index,
    selectedList)
{
    for (var i = 0; i < selectedList.length; i++)
    {
        if (selectedList.options[i].value == availableList.options[index].value &&
            selectedList.options[i].text == availableList.options[index].text)
        {
            selectedList.options[i].selected = true;
            return false;
        }
    }

    var promptEntry = new Option(availableList.options[index].text, availableList.options[index].value, false, true);
    selectedList.options[selectedList.length] = promptEntry;
    return true;
}

////////////////////////////////
// add number, currency, string from available list box to selected list box
// where multiple prompt values are supported
function promptengine_addValueFromPickList(
    form,
    type,
    promptID)
{
    var AvailableList, SelectedList;
    AvailableList = form[promptID + "AvailableList"];
    SelectedList = form[promptID + "ListBox"];

    promptengine_deselectAllItems(SelectedList);

    var changed = false;

    var lastSelected = -1;
    for (var i = 0; i < AvailableList.length; i++)
    {
        if (AvailableList.options[i].selected)
        {
            var added = promptengine_addAvailableItem(AvailableList, i, SelectedList);
            if (added == true)
                changed = true;
            lastSelected = i;
        }
    }

    // set focus to the next item on the available list
    if (lastSelected++ >= 0 && lastSelected < AvailableList.length)
    {
        promptengine_deselectAllItems(AvailableList);
        AvailableList.options[lastSelected].selected = true;
    }

    return changed;
}

////////////////////////////////
// add all number, currency, string from available list box to selected list box
// where multiple prompt values are supported
function promptengine_addAllValues(
    form,
    type,
    promptID)
{
    var AvailableList, SelectedList;
    AvailableList = form[promptID + "AvailableList"];
    SelectedList = form[promptID + "ListBox"];

    promptengine_deselectAllItems(SelectedList);

    var changed = false;
    for (var i = 0; i < AvailableList.length; i++)
    {
        var added = promptengine_addAvailableItem(AvailableList, i, SelectedList);
        if (added == true)
            changed = true;
    }

    return changed;
}

////////////////////////////////////
// adds Range prompt to listbox where multiple values are supported
function promptengine_addRangeValue (
    form,
    type ,
    promptID )
{
    var lowerBoundPickList = form[promptID + "SelectLowerRangeValue"];
    var upperBoundPickList = form[promptID + "SelectUpperRangeValue"];
    
    lowerBound = form[promptID + "LowerBound"];
    upperBound = form[promptID + "UpperBound"];
    //handle select box, not text box case
    if ( lowerBound.type.toLowerCase () != "text" &&
     lowerBound.type.toLowerCase () != "hidden" &&
     lowerBound.type.toLowerCase () != "password" )  //either upper or lower, doesn't matter
    {
        lowerBound = lowerBound.options[lowerBound.selectedIndex];
        upperBound = upperBound.options[upperBound.selectedIndex];
    }

    lowerUnBounded = form[promptID + "NoLBoundCheck"].checked;
    upperUnBounded = form[promptID + "NoUBoundCheck"].checked;
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

    lowerChecked = form[promptID + "LowerCheck"].checked;
    upperChecked = form[promptID + "UpperCheck"].checked;

    // value for showing in the list box only, no need encode here
    value = ( lowerChecked && ! lowerUnBounded ) ? "[" : "(";
    if ( ! lowerUnBounded ) //unbounded is empty string not quoted empty string (e.g not "_crEMPTY_")
        value += (lvalue);
    value += "_crRANGE_"
    if ( ! upperUnBounded )
        value += (uvalue);
    value += (upperChecked && ! upperUnBounded ) ? "]" : ")";
    if ( debug ) alert (value);

    display = ( lowerChecked && ! lowerUnBounded ) ? "[" : "(";
    display += ldisplay;
    display += " .. "
    display += udisplay;
    display += (upperChecked && ! upperUnBounded ) ? "]" : ")";

    promptEntry = new Option(display,value,false,false);
    theList = form[promptID + "ListBox"];
    
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
function promptengine_findOptionInList(selectObj, optionValue)
{	
    if (selectObj == null || optionValue == null || optionValue == "")
        return -1;
	
    for (var i = 0; i < selectObj.options.length; i++)
    {
        if (selectObj.options[i].value == optionValue)
            return i;
    }

    return -1;	
}

////////////////////////////////////
// disable check boxes / dropdowns / textboxes based on user selection on the range parameters
function promptengine_onNoBoundCheckClicked(
    form,
    promptID,
    lowOrUpBound)
{
    if (lowOrUpBound == 0) {
        if (form[promptID + "NoLBoundCheck"].checked) {
            form[promptID + "NoUBoundCheck"].disabled = true;
            form[promptID + "LowerCheck"].disabled = true;
            form[promptID + "LowerBound"].disabled = true;
            if (form[promptID + "SelectLowerRangeValue"] != null)
                form[promptID + "SelectLowerRangeValue"].disabled = true;
        }
        else {
            form[promptID + "NoUBoundCheck"].disabled = false;
            form[promptID + "LowerCheck"].disabled = false;
            form[promptID + "LowerBound"].disabled = false;
            if (form[promptID + "SelectLowerRangeValue"] != null)
                form[promptID + "SelectLowerRangeValue"].disabled = false;
        }
    } else if (lowOrUpBound == 1) {
        if (form[promptID + "NoUBoundCheck"].checked) {
            form[promptID + "NoLBoundCheck"].disabled = true;
            form[promptID + "UpperCheck"].disabled = true;
            form[promptID + "UpperBound"].disabled = true;
            if (form[promptID + "SelectUpperRangeValue"] != null)
                form[promptID + "SelectUpperRangeValue"].disabled = true;
        } else {
            form[promptID + "NoLBoundCheck"].disabled = false;
            form[promptID + "UpperCheck"].disabled = false;
            form[promptID + "UpperBound"].disabled = false;
            if (form[promptID + "SelectUpperRangeValue"] != null)
                form[promptID + "SelectUpperRangeValue"].disabled = false;
        }
    }
}

////////////////////////////////////
// disable text boxes / list boxes based on whether setNull is checked
function promptengine_onSetNullCheckClicked(
    form,
    promptID)
{
    if (form[promptID + "NULL"].checked)
    {
        if (form[promptID + "DiscreteValue"] != null)
            form[promptID + "DiscreteValue"].disabled = true;
        if (form[promptID + "SelectValue"] != null)
            form[promptID + "SelectValue"].disabled = true;
    }
    else
    {
        if (form[promptID + "DiscreteValue"] != null)
            form[promptID + "DiscreteValue"].disabled = false;
        if (form[promptID + "SelectValue"] != null)
            form[promptID + "SelectValue"].disabled = false;            
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
    if(form[selectCtrl].selectedIndex < 0)
        return false;

    selectedOption = form[selectCtrl].options[form[selectCtrl].selectedIndex];
    if (selectedOption.value == null && form[textCtrl].value == null)
        return false;

    var changed = true;
    if (selectedOption.value == form[textCtrl].value)
        changed = false;

    form[textCtrl].value = selectedOption.value;
    return changed;
}

function promptengine_hasValueInTextBox(
    form,
    textboxID)
{
    if (form[textboxID].value == null)
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
    valueField = form[valueID];

    curVal = valueField.value;

    if (curVal.length > 0)
        curVal += "&";
    curVal +=  "cascadingPID" + "=" + promptID;

    if ( debug )
        alert (curVal);

    valueField.value = curVal;
    return true;
}

/////////////////////////////////////////////////
// remove selected values from multi-value prompt
function promptengine_removeValue(
    form,
    promptID)
{
    var lbox = form[promptID + "ListBox"];
    var changed = false;

    var lastSelected = -1;
    for ( var idx = 0; idx < lbox.options.length; )
    {
        if ( lbox.options[idx].selected )
        {
            lbox.options[idx] = null;
            changed = true;
            lastSelected = idx;
        }
        else
            idx++;
    }

    if (lastSelected >= 0 && lastSelected < lbox.length)
    {
        // highlight the next item
        promptengine_deselectAllItems(lbox);
        lbox.options[lastSelected].selected = true;
    }
    else if (lastSelected == lbox.length && lastSelected > 0)
    {
        // highlight the last item
        promptengine_deselectAllItems(lbox);
        lbox.options[lastSelected - 1].selected = true;
    }
    
    return changed;
}

function promptengine_onRemoveValue(
    form,
    promptID)
{
    promptengine_removeValue(form, promptID);
}

/////////////////////////////////////////////////
// remove all values from multi-value prompt
function promptengine_removeAllValues(
    form,
    promptID)
{
    var lbox = form[promptID + "ListBox"];

    var changed = false;

    if (lbox.options.length > 0)
        changed = true;

    for ( var idx = 0; idx < lbox.options.length; )
    {
        lbox.options[idx] = null;
    }

    return changed;
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
    valueField = form[valueID];

    curVal = valueField.value;

    if (curVal.length > 0)
        curVal += "&";
    curVal += promptID + "=" + value;

    if ( debug )
        alert (curVal);

    valueField.value = curVal;

    return true;
}

///////////////////////////////////////
// reset hidden value field
function promptengine_resetValueField (
    form,
    valueID)
{
    valueField = form[valueID];
    valueField.value = "";
}

/////////////////////////////////////
// sets prompt value into the hidden form field in proper format so that it can be submitted
function promptengine_updateDiscreteValue (
    form,
    valueID,
    promptID,
    type,
    checkValue)
{
    var value = "";

    if (form[promptID + "NULL"] != null &&
        form[promptID + "NULL"].checked )
    {
        value = "_crNULL_"; //NULL is a literal for, uhmm.. a NULL
    }
    else
    {
        valueField = form[promptID + "DiscreteValue"];

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

        if ( checkValue && !promptengine_checkValue ( value, type ) )
        {
            if (valueField.focus)
            {
                if(valueField.type.toLowerCase () != "hidden")
                    valueField.focus ();
                else
                {
                    var valueSelectField = form[promptID + "SelectValue"];
                    if(valueSelectField && valueSelectField.focus)
                        valueSelectField.focus ();
                }
            }

            return false;
        }
        else
        {
            value = promptengine_encodePrompt(value);
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
    checkValue)
{
    if (form[promptID + "NULL"] != null &&
        form[promptID + "NULL"].checked )
    {
        value = "_crNULL_"; //NULL is a literal for, uhmm.. a NULL
    }
    else
    {
        lowerBound = form[promptID + "LowerBound"];
        upperBound = form[promptID + "UpperBound"];

        //handle select box, not text box case
        if (lowerBound.type.toLowerCase () != "text" &&
            lowerBound.type.toLowerCase () != "hidden" &&
            lowerBound.type.toLowerCase () != "password")  //either upper or lower, doesn't matter
        {
            lowerBound = lowerBound.options[lowerBound.selectedIndex];
            upperBound = upperBound.options[upperBound.selectedIndex];
        }
        lowerUnBounded = form[promptID + "NoLBoundCheck"].checked;
        upperUnBounded = form[promptID + "NoUBoundCheck"].checked;
        lowerChecked = form[promptID + "LowerCheck"].checked;
        upperChecked = form[promptID + "UpperCheck"].checked;
        uvalue = lvalue = "";

        if ( ! lowerUnBounded )
        {
            if ( checkValue && !promptengine_checkRangeBoundValue ( lowerBound.value, type ) ) {
                if ( lowerBound.focus && lowerBound.type.toLowerCase () != "hidden")
                    lowerBound.focus();
                return false;
            }
            lvalue = lowerBound.value;
        }
        if ( ! upperUnBounded )
        {
            if ( checkValue && !promptengine_checkRangeBoundValue ( upperBound.value, type ) ) {
                if ( upperBound.focus && upperBound.type.toLowerCase () != "hidden")
                    upperBound.focus ();
                return false;
            }
            uvalue = upperBound.value;
        }
        value = ( lowerChecked && ! lowerUnBounded ) ? "[" : "(";
        if ( ! lowerUnBounded )
            value += promptengine_encodePrompt(lvalue);
        value += "_crRANGE_"
        if ( ! upperUnBounded )
            value += promptengine_encodePrompt(uvalue);
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
    checkValue)
{
    values = form[promptID + "ListBox"].options;
    value = "";

    if (form[promptID + "NULL"] != null &&
        form[promptID + "NULL"].checked)
    {
        value = "_crNULL_"; //NULL is a literal for, uhmm.. a NULL
    }
    else
    {
        if ( values.length == 0 )
        {
            value = "_crEMPTY_";     //if value is empty, set to empty string
        }
        else
        {
            for ( i = 0; i < values.length; i++ )
            {
                // first value could be empty string, then chcking value.length != 0 could miss one empty string
                if ( i != 0 )
                    value += "_crMULT_"
                value += promptengine_encodePrompt(values[i].value);
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
    type )
{
    if (value == null)
        return false;
        
    if (value == "_crNULL_")
        return true;
        
    if ( type == 'n' && ! regNumber.test ( value ) )
    {
        if (value.length > 0)
            alert ( L_BadNumber );
        else
            alert ( (typeof L_Empty) != 'undefined'? L_Empty : L_NoValue );

        return false;
    }
    else if ( type == 'c' && ! regCurrency.test ( value ) )
    {
        if (value.length > 0)
            alert ( L_BadCurrency );
        else
            alert ( (typeof L_Empty) != 'undefined'? L_Empty : L_NoValue );

        return false;
    }
    else if ( type == 'd' )
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
    else if ( type == "dt" )
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
    else if ( type == 't' && ! regTime.test ( value ) && !regTimeHTML.test( value )  )
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
    type )
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
  if (isNetscape)
  {
    if (evt.which == ENTER_KEY && (evt.target.type == "text" || evt.target.type == "password"))
        return true;
  }
  else
  {
    if (window.event.keyCode == ENTER_KEY && (window.event.srcElement.type == "text" || window.event.srcElement.type == "password"))
        return true;
  }

  return false;
}

function promptengine_isEnterKey(evt)
{
  if (isNetscape)
  {
    if (evt.which == ENTER_KEY && evt.target.tagName.toLowerCase() != "a")
        return true;
  }
  else
  {
    if (window.event.keyCode == ENTER_KEY && window.event.srcElement.tagName.toLowerCase() != "a")
        return true;
  }
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
	
	var offsetTop = elt.offsetTop; 
	if (!offsetTop) return;
	
	var myOffsetParent = elt.offsetParent; 
	while( myOffsetParent ) 	
	{ 
		offsetTop += myOffsetParent.offsetTop; 
		myOffsetParent = myOffsetParent.offsetParent; 
	}  
	
	window.scrollTo(0, offsetTop);
}

 /* Crystal Decisions Confidential Proprietary Information */

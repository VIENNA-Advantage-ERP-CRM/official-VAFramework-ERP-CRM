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
var needURLEncode = false;	// only need to do url encode in java
var promptPrefix = "promptex-";

///////////////////////////////
// properly encode prompt values
function encodePrompt (prompt)
{
    if (needURLEncode)
    {
    	return encodeURIComponent(prompt);
    }
    else
    {
    	return prompt;
    }
}

////////////////////////////////
// add number, currency, string from dropdown/textbox to list box
// where multiple prompt values are supported
function addPromptDiscreteValue ( inForm, type , paramName)
{
	var widget, obj;
    widget = obj = inForm[paramName + "DiscreteValue"];
	if ( obj.type && obj.type.toLowerCase() != "text" &&
	     obj.type.toLowerCase() != "hidden" && obj.type.toLowerCase() != "password")
	{
		//select box not a textbox
		obj = obj.options[obj.selectedIndex];
	}
	if ( ! checkSingleValue ( obj.value, type ) )
    {
        if (widget.focus && widget.type.toLowerCase() != "hidden")
			widget.focus();
		return false;
    }
	promptValue =  encodePrompt(obj.value);
	displayString = ( obj.text ) ? obj.text : obj.value;
	promptEntry = new Option(displayString,promptValue,false,false);
	theList = inForm[paramName + "ListBox"];
	theList.options[theList.length] = promptEntry;
    if (widget.focus && widget.type.toLowerCase() != "hidden")
        widget.focus ();
    if ( widget.select )
        widget.select ();
    if ( widget.type.toLowerCase != "text" &&
	 widget.type.toLowerCase != "hidden" &&
	 widget.type.toLowerCase != "password")
        if ( widget.selectedIndex < widget.length - 1 )
            widget.selectedIndex = widget.selectedIndex + 1;      //... or move to next selection in listbox
}

////////////////////////////////////
// adds Range prompt to listbox where multiple values are supported
function addPromptRangeValue ( inForm, type , paramName )
{
    lowerBound = inForm[paramName + "LowerBound"];
    upperBound = inForm[paramName + "UpperBound"];
    //handle select box, not text box case
    if ( lowerBound.type.toLowerCase () != "text" &&
	 lowerBound.type.toLowerCase () != "hidden" &&
	 lowerBound.type.toLowerCase () != "password" )  //either upper or lower, doesn't matter
    {
        lowerBound = lowerBound.options[lowerBound.selectedIndex];
        upperBound = upperBound.options[upperBound.selectedIndex];
    }

    lowerUnBounded = inForm[paramName + "NoLowerBoundCheck"].checked;
    upperUnBounded = inForm[paramName + "NoUpperBoundCheck"].checked;
    lvalue = uvalue = "";

    if ( ! lowerUnBounded )
    {
        if ( ! checkSingleValue ( lowerBound.value, type ) ) {
            if ( lowerBound.focus && lowerBound.type.toLowerCase () != "hidden")
                lowerBound.focus ();
            return false;
        }
        lvalue = lowerBound.value;
    }
    if ( ! upperUnBounded )
    {
        if ( ! checkSingleValue ( upperBound.value, type ) ) {
            if ( upperBound.focus && upperBound.type.toLowerCase () != "hidden")
                upperBound.focus ();
            return false;
        }
        uvalue = upperBound.value;
    }
    ldisplay = (lowerBound.text && !lowerUnBounded) ? lowerBound.text : lvalue;
    udisplay = (upperBound.text && !upperUnBounded) ? upperBound.text : uvalue;

    lowerChecked = inForm[paramName + "LowerCheck"].checked;
    upperChecked = inForm[paramName + "UpperCheck"].checked;

    value = ( lowerChecked && ! lowerUnBounded ) ? "[" : "(";
    if ( ! lowerUnBounded ) //unbounded is empty string not quoted empty string (e.g not "_crEMPTY_")
        value += encodePrompt(lvalue);
    value += "_crRANGE_"
    if ( ! upperUnBounded )
        value += encodePrompt(uvalue);
    value += (upperChecked && ! upperUnBounded ) ? "]" : ")";
    if ( debug ) alert (value);

    display = ( lowerChecked && ! lowerUnBounded ) ? "[" : "(";
    display += ldisplay;
    display += "  ..  "
    display += udisplay;
    display += (upperChecked && ! upperUnBounded ) ? "]" : ")";

	promptEntry = new Option(display,value,false,false);
	theList = inForm[paramName + "ListBox"];
	theList.options[theList.length] = promptEntry;

}

////////////////////////////////////
// disable check boxes based on user selection on the range parameters
function disableBoundCheck(lowOrUpBound, inform, paramName) {
	if (lowOrUpBound == 0) {
		if (inform[paramName + "NoLowerBoundCheck"].checked) {
			inform[paramName + "NoUpperBoundCheck"].disabled = true;
			inform[paramName + "LowerCheck"].disabled = true;
			inform[paramName + "LowerBound"].disabled = true;
		}
		else {
			inform[paramName + "NoUpperBoundCheck"].disabled = false;
			inform[paramName + "LowerCheck"].disabled = false;
			inform[paramName + "LowerBound"].disabled = false;
		}
	} else if (lowOrUpBound == 1) {
		if (inform[paramName + "NoUpperBoundCheck"].checked) {
			inform[paramName + "NoLowerBoundCheck"].disabled = true;
			inform[paramName + "UpperCheck"].disabled = true;
			inform[paramName + "UpperBound"].disabled = true;
		} else {
			inform[paramName + "NoLowerBoundCheck"].disabled = false;
			inform[paramName + "UpperCheck"].disabled = false;
			inform[paramName + "UpperBound"].disabled = false;
		}
	}
}

////////////////////////////////////
// puts "select" value into text box for an editable prompt which also has defaults
function setSelectedValue (inForm, selectCtrl, textCtrl)
{
    selectedOption = inForm[selectCtrl].options[inForm[selectCtrl].selectedIndex];
    inForm[textCtrl].value = selectedOption.value;
}

///////////////////////////////////
// remove value from listbox where multiple value prompts are supported
function removeFromListBox ( inForm, paramName )
{
	lbox = inForm[paramName + "ListBox"];
	for ( var idx = 0; idx < lbox.options.length; )
	{
		if ( lbox.options[idx].selected )
			lbox.options[idx] = null;
		else
			idx++;
	}
}

/////////////////////////////////////
// sets prompt value into the hidden form field in proper format so that it can be submitted
function setPromptSingleValue (inform, type, paramName)
{
    hiddenField = inform[promptPrefix + paramName];
    value = "";
    if ( inform[paramName + "NULL"] != null && inform[paramName + "NULL"].checked )
        value = "_crNULL_"; //NULL is a literal for, uhmm.. a NULL
    else
    {
        discreteVal = inform[paramName + "DiscreteValue"];
        if ( discreteVal.type.toLowerCase () != "text" &&
 	     discreteVal.type.toLowerCase () != "hidden" &&
	     discreteVal.type.toLowerCase () != "password")
            value = discreteVal.options[discreteVal.selectedIndex].value;
        else
            value = discreteVal.value;
        if ( ! checkSingleValue ( value, type ) ) {
            if (discreteVal.focus && discreteVal.type.toLowerCase ())
 	    		discreteVal.focus ();
            return false;
        }
        else
            value = encodePrompt(value);
    }
    hiddenField.value = value;
	return true;
}

/////////////////////////////////////
// sets prompt value for a range into the hidden form field in proper format so that it can be submitted
function setPromptRangeValue (inform, type, paramName)
{
    hiddenField = inform[promptPrefix + paramName];
    
    if ( inform[paramName + "NULL"] != null && inform[paramName + "NULL"].checked )
    {
        value = "_crNULL_"; //NULL is a literal for, uhmm.. a NULL
        hiddenField.value = value;
	return true;
    }

    lowerBound = inform[paramName + "LowerBound"];
    upperBound = inform[paramName + "UpperBound"];
    //handle select box, not text box case
    if ( lowerBound.type.toLowerCase () != "text" &&
	 lowerBound.type.toLowerCase () != "hidden" &&
	 lowerBound.type.toLowerCase () != "password")  //either upper or lower, doesn't matter
    {
        lowerBound = lowerBound.options[lowerBound.selectedIndex];
        upperBound = upperBound.options[upperBound.selectedIndex];
    }
    lowerUnBounded = inform[paramName + "NoLowerBoundCheck"].checked;
    upperUnBounded = inform[paramName + "NoUpperBoundCheck"].checked;
    lowerChecked = inform[paramName + "LowerCheck"].checked;
    upperChecked = inform[paramName + "UpperCheck"].checked;
    uvalue = lvalue = "";

    if ( ! lowerUnBounded )
    {
        if ( ! checkSingleValue ( lowerBound.value, type ) ) {
            if ( lowerBound.focus && lowerBound.type.toLowerCase () != "hidden")
                lowerBound.focus();
            return false;
        }
        lvalue = lowerBound.value;
    }
    if ( ! upperUnBounded )
    {
        if ( ! checkSingleValue ( upperBound.value, type ) ) {
            if ( upperBound.focus && upperBound.type.toLowerCase () != "hidden")
                upperBound.focus ();
            return false;
        }
        uvalue = upperBound.value;
    }
    value = ( lowerChecked && ! lowerUnBounded ) ? "[" : "(";
    if ( ! lowerUnBounded )
        value += encodePrompt(lvalue);
    value += "_crRANGE_"
    if ( ! upperUnBounded )
        value += encodePrompt(uvalue);
    value += (upperChecked && ! upperUnBounded ) ? "]" : ")";
    if ( debug )
        alert (value);
    hiddenField.value = value;
	return true;
}

/////////////////////////////////////
// sets prompt value into the hidden form field in proper format so that it can be submitted
function setPromptMultipleValue (inform, type, paramName)
{
    hiddenField = inform[promptPrefix + paramName];
    values = inform[paramName + "ListBox"].options;
    
    if ( inform[paramName + "NULL"] != null && inform[paramName + "NULL"].checked )
    {
        value = "_crNULL_"; //NULL is a literal for, uhmm.. a NULL
        hiddenField.value = value;
        return true;
    }
    
    if ( values.length == 0 )
    {
        value = "_crEMPTY_";     //if value is empty, set to empty string
    }
    else
    {
        value = "";
        for ( idx = 0; idx < values.length; ++idx )
        {
            // first value could be empty string, then chcking value.length != 0 could miss one empty string
            if ( idx != 0 )
                value += "_crMULT_"
            value += values[idx].value;
        }
    }

    if ( debug )
        alert (value);
    hiddenField.value = value;
    //NOTE: we'll always return true as the validation is done before values are added to select box
	return true;
}

///////////////////////////////////////
// check and alert user about any errors based on type of prompt
var regNumber    = /^(\+|-)?((\d+(\.|,| |\u00A0)?\d*)+|(\d*(\.|,| |\u00A0)?\d+)+)$/
var regCurrency  = regNumber;
var regDate	 = /^(D|d)(A|a)(T|t)(E|e) *\( *\d{4} *, *(0?[1-9]|1[0-2]) *, *((0?[1-9]|[1-2]\d)|3(0|1)) *\)$/
var regDateTime  = /^(D|d)(A|a)(T|t)(E|e)(T|t)(I|i)(M|m)(E|e) *\( *\d{4} *, *(0?[1-9]|1[0-2]) *, *((0?[1-9]|[1-2]\d)|3(0|1)) *, *([0-1]?\d|2[0-3]) *, *[0-5]?\d *, *[0-5]?\d *\)$/
var regTime	 = /^(T|t)(I|i)(M|m)(E|e) *\( *([0-1]?\d|2[0-3]) *, *[0-5]?\d *, *[0-5]?\d *\)$/

function checkSingleValue ( value, type )
{
	if ( type == 'n' && ! regNumber.test ( value ) )
	{
		alert ( L_BadNumber );
		return false;
	}
	else if ( type == 'c' && ! regCurrency.test ( value ) )
	{
		alert ( L_BadCurrency );
		return false;
	}
	else if ( type == 'd' && ! regDate.test ( value ) )
	{
		alert ( L_BadDate );
		return false;
	}
	else if ( type == "dt" && ! regDateTime.test ( value ) )
	{
		alert ( L_BadDateTime );
		return false;
	}
	else if ( type == 't' && ! regTime.test ( value ) )
	{
		alert ( L_BadTime );
		return false;
	}

	//by default let it go...
	return true;
}

function checkValue(evt) {
  
  if (navigator.appName == "Netscape") 
  {
	if (evt.which == 13 && (evt.target.type == "text" || evt.target.type == "password")) 
	{
		checkSetAndSubmitValues ();
	}
  } 
  else 
  {
	if (window.event.keyCode == 13 && (window.event.srcElement.type == "text" || window.event.srcElement.type == "password")) 
	{
		checkSetAndSubmitValues ();
	}
  }
}
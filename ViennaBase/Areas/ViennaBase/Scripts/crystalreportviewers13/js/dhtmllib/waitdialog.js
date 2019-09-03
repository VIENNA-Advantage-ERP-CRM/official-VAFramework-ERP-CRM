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

File: waitdialog.js
Waiting Dialog boxes

dom.js dialog.js palette.js must be included before

=============================================================
*/


// ================================================================================
// ================================================================================
//

// ================================================================================
// ================================================================================
//
// OBJECT newWaitDialogBoxWidget (Constructor)
//
// Class for waiting dialog box
//
// ================================================================================
// ================================================================================

function newWaitDialogBoxWidget(id,w,h,title,bShowCancel,cancelCB,bShowLabel,text, noCloseButton)
// CONSTRUCTOR
// id				[String]            the id for DHTML processing
// title			[String]            the caption dialog text
// width			[int optional]      dialog box width
// height			[int optional]      dialog box height
// showCancel		[boolean optional]	if true, a CANCEL button is displayed
// cancelCB			[Function optional] callback when esc key is hit or CANCEL button pushed
// showLabel		[boolean optional]	if true, a label is displayed
// text				[String optional]	label
{
	// Min values for width and height of this dialog box
	var minW=250
	var minH=150
	if (w<minW) 
		w=minW
	if (h<minH) 
		h=minH
		
    // Justin: I've added the close button to the top right corner (last param=false) so that the user can hide 
    // the progress dialog.  The cancel button option of the waitdialog adds a full button to the dialog 
    // and gives the user the expectation that the request has been cancelled vs. just hiding this dialog.
	var o=newDialogBoxWidget(id,title,w,null,null,WaitDialogBoxWidget_cancelCB, noCloseButton,true)
	
	// Properties
	o.pad=5	
	o.frZone=newFrameZoneWidget(id+"_frZone",null,null)
	o.showLabel=(bShowLabel!=null)?bShowLabel:false	
	o.showCancel=(bShowCancel!=null)?bShowCancel:false
	o.label=newWidget(id+"_label")
	o.label.text=text
	if (o.showCancel) {
		o.cancelButton=newButtonWidget(id+"_cancelButton",_cancelButtonLab,CancelButton_cancelCB)
		o.cancelButton.par=o
	}
	else {
		// otherwise getHTML asks for button.gif which costs an extra http request
		o.cancelButton={};
		o.cancelButton.init=function(){};
		o.cancelButton.setDisplay=function(x) {};
		o.cancelButton.setDisabled=function(x) {};
		o.cancelButton.getHTML=function(){ return '' };		
	}
	o.cancelCB=cancelCB
	
	// Methods
	o.oldDialogBoxInit=o.init
	o.init=WaitDialogBoxWidget_init
	o.getHTML=WaitDialogBoxWidget_getHTML
	o.setShowCancel=WaitDialogBoxWidget_setShowCancel	
	o.setShowLabel=WaitDialogBoxWidget_setShowLabel

	return o
}

// ================================================================================

function WaitDialogBoxWidget_init()
// Init the widget layers
// Return [void]
{
	var o=this
	o.oldDialogBoxInit()

	o.frZone.init()

	o.label.init()
	o.label.setDisplay(o.showLabel)
	
	o.cancelButton.init()
	o.cancelButton.setDisplay(o.showCancel)
}

// ================================================================================

function WaitDialogBoxWidget_getHTML()
// getHTML
// Returns the HTML generated for the WaitDialogBoxWidget
{
    var o=this, s=''

        s+= o.beginHTML()

        s+= '<table border="0" cellspacing="0" cellpadding="0" width="100%"><tbody>'
        s+= '<tr>' +
                '<td align="center" valign="top">' +
                    o.frZone.beginHTML()
        s+=         '<table border="0" cellspacing="0" cellpadding="0" width="100%"><tbody>'    +
                    '<tr>' +
                        '<td align="center" style="padding-top:5px;">' + img(_skin+'wait01.gif',200,40) + '</td>' +
                    '</tr>' +
                    '<tr>' +
                        '<td align="left" style="padding-left:2px;padding-right:2px;padding-top:5px;">' +
                            '<div id="'+o.label.id+'" class="iconText" style="wordWrap:break_word;text-align:center;">'+convStr(o.label.text,false,true)+'</div>'+
                        '</td>' +
                    '</tr>' +
                    '</tbody></table>'
        s+=         o.frZone.endHTML() +
                '</td>' +
            '</tr>'
        s+= '<tr>' +
                '<td align="right" valign="middle" style="padding-top:5px;padding-right:9px">' +
                    o.cancelButton.getHTML() +
                '</td>' +
            '</tr>'
        s+= '</tbody></table>'

        s+= o.endHTML()

        return s
}

// ================================================================================

function WaitDialog_FrameZoneWidget_beginHTML()
{
	var o=this
	return '<table class="waitdialogzone" style="'+sty("width",o.w)+sty("height",o.h)+'" id="'+o.id+'" cellspacing="0" cellpadding="0" border="0"><tbody>'+
		'<tr><td valign="top" class="dialogzone" id="frame_cont_'+o.id+'">'
}

// ================================================================================

function WaitDialog_FrameZoneWidget_endHTML()
{
	var o=this
	return '</td></tr></tbody></table>'
}

// ================================================================================

function WaitDialogBoxWidget_setShowCancel(show,cancelCB)
// Show or hide the cancel button
// show	[boolean]	if true, cancel button is displayed
{
	var o=this

	o.showCancel=show
	o.cancelButton.setDisabled(false)
	o.cancelButton.setDisplay(show)	
	o.cancelCB=cancelCB
}

// ================================================================================

function WaitDialogBoxWidget_setShowLabel(show,text)
// Show or hide the label
// show	[boolean]	if true, label is displayed
// text	[string]	text label
{
	var o=this

	o.showLabel=show
	o.label.text=text
	o.label.setHTML(text)
	o.label.setDisplay(show)
}

// ================================================================================

function WaitDialogBoxWidget_cancelCB()
// Callback called when the cancel button is hit
{
	var o=this
	if (o.cancelCB != null)
	{
		o.cancelCB()		
		o.cancelButton.setDisabled(true);
	}
}

// ================================================================================

function CancelButton_cancelCB()
// Callback called when the cancel button is hit
{
	var o=this
	if (o.par.cancelCB != null)
	{
		o.par.cancelCB()		
		o.par.cancelButton.setDisabled(true);
	}
}

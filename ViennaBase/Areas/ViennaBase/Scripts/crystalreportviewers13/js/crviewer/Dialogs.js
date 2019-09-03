/* Copyright (c) Business Objects 2006. All rights reserved. */

if (typeof bobj.crv.PrintUI == 'undefined') {
    bobj.crv.PrintUI = {};
}

if (typeof bobj.crv.ExportUI == 'undefined') {
    bobj.crv.ExportUI = {};
}

if (typeof bobj.crv.ErrorDialog == 'undefined') {
    bobj.crv.ErrorDialog = {};
}

if (typeof bobj.crv.ReportProcessingUI == 'undefined') {
    bobj.crv.ReportProcessingUI = {};
}

/*
================================================================================
PrintUI
================================================================================
*/

bobj.crv.newPrintUI = function(kwArgs) {
    if (!kwArgs.id) {
        kwArgs = MochiKit.Base.update({id: bobj.uniqueId()}, kwArgs);
    }
    
    var lbl = kwArgs.submitBtnLabel;
    if (!lbl) {
        lbl = L_bobj_crv_submitBtnLbl;
    }
    
    var infoTitle = kwArgs.infoTitle;
    if (!infoTitle) {
        infoTitle = L_bobj_crv_PrintInfoTitle;
    }
    
    var dialogTitle = kwArgs.dialogTitle;
    if (!dialogTitle) {
        if (kwArgs.isActxPrinting) {
            dialogTitle = L_bobj_crv_ActiveXPrintDialogTitle;
        }
        else {
            dialogTitle = L_bobj_crv_PDFPrintDialogTitle;
        }
    }
    
    var infoMsg = kwArgs.infoMsg;
    if (!infoMsg) {
        infoMsg = L_bobj_crv_PrintInfo1;
        infoMsg += '\n';
        infoMsg += L_bobj_crv_PrintInfo2;
    }
    
    var o = newDialogBoxWidget(kwArgs.id + '_dialog', 
                                dialogTitle, 
                                300, 
                                100,
                                null,
                                bobj.crv.PrintUI._cancel,
                                false);
    o.infoMsg = infoMsg;
    o.infoTitle = infoTitle;
    o.actxId = o.id + '_actx';
    o.actxContainerId = o.id + '_actxdiv';
    o._processingPrinting = false;
    o._initOld = o.init;
    o._showOld = o.show;
    
    if (!kwArgs.isActxPrinting) {
        o._fromBox = newIntFieldWidget(o.id + "_fromBox", 
                                        null, 
                                        null, 
                                        null, 
                                        null, 
                                        true,
                                        '', 
                                        50);
        o._fromBox.setDisabled =  bobj.crv.PrintUI.disabledTextFieldWidget;                                
        o._toBox = newIntFieldWidget(o.id + "_toBox", 
                                        null, 
                                        null, 
                                        null, 
                                        null, 
                                        true,
                                        '', 
                                        50);
        o._toBox.setDisabled = bobj.crv.PrintUI.disabledTextFieldWidget;
                                        
        o._submitBtn = newButtonWidget(o.id + "_submitBtn", 
                                        lbl, 
                                        MochiKit.Base.bind(bobj.crv.PrintUI._submitBtnCB, o));
             
        o._submitBtn.setDelayCallback(false);
        o._allRadio = newRadioWidget(o.id + "_allRadio", 
                                        o.id + "_grp", 
                                        L_bobj_crv_PrintAllLbl,
                                        MochiKit.Base.bind(bobj.crv.PrintUI.disabledPageRange ,o, true));
        
        o._allRadio.layerClass= "dlgContent";
                                        
        o._rangeRadio = newRadioWidget(o.id + "_rangeRadio", 
                                        o.id + "_grp", 
                                        L_bobj_crv_PrintPagesLbl,
                                        MochiKit.Base.bind(bobj.crv.PrintUI.disabledPageRange ,o, false));
        
        o._rangeRadio.layerClass= "dlgContent";
    }
    
    o.widgetType = 'PrintUI';
    
    // Update instance with constructor arguments
    bobj.fillIn(o, kwArgs);
    
    // Update instance with member functions
    MochiKit.Base.update(o, bobj.crv.PrintUI);
    
    return o;
};
bobj.crv.PrintUI.disabledTextFieldWidget = function(disabled)
{
    TextFieldWidget_setDisabled.call(this,disabled);
    
    if(disabled)
    {
        MochiKit.DOM.addElementClass(this.layer, "textDisabled");
    }
    else {
        MochiKit.DOM.removeElementClass(this.layer, "textDisabled");
    }
}

bobj.crv.PrintUI.disabledPageRange = function(bool)
{
    if(this._fromBox && this._toBox)
    {
        this._fromBox.setDisabled(bool);
        this._toBox.setDisabled(bool);    
    }
}

bobj.crv.PrintUI._submitBtnCB = function() {
    var start = null;
    var end = null;
    if (this._rangeRadio.isChecked()) {
        start = parseInt(this._fromBox.getValue(), 10);
        end = parseInt(this._toBox.getValue(), 10);
        
        if (!start || !end || (start < 0) || (start > end)) {
            alert(L_bobj_crv_PrintPageRangeError);
            return;
        }
    }
    
    if (this.widgetType == 'PrintUI') {
        MochiKit.Signal.signal(this, 'printSubmitted', start, end);
    }
    else {
        MochiKit.Signal.signal(this, 'exportSubmitted', start, end, this._comboBox.getSelection().value);
    }
    
    this.show(false);
};

bobj.crv.PrintUI._getRPSafeURL = function(url) {
    if (!url) {
        return;
    }
    
    if (url.indexOf('/') === 0) {
        return url;
    }
    
    var winLoc = window.location.href;
    
    var qPos = winLoc.lastIndexOf('?');
    if (qPos > 0) {
        winLoc = winLoc.substring(0, qPos)
    }

    var lPos = winLoc.lastIndexOf('/');
    
    if (lPos < 0) {
        return url;
    }
    
    winLoc = winLoc.substring(0, lPos);
    return winLoc + '/' + url;
};

bobj.crv.PrintUI._getObjectTag = function(postData) {
    var oa = [];
    
    oa.push('<OBJECT width="0" height="0" ID="');
    oa.push(this.actxId);
    oa.push('" CLASSID="CLSID:');
    oa.push(bobj.crv.ActxPrintControl_CLSID);
    oa.push('" CODEBASE="');
    oa.push(this._getRPSafeURL(this.codeBase));
    oa.push('#Version=');
    oa.push(bobj.crv.ActxPrintControl_Version);
    oa.push('" VIEWASTEXT>');
    
    oa.push('<PARAM NAME="PostBackData" VALUE="');
    oa.push(postData);
    oa.push('">');
    
    oa.push('<PARAM NAME="ServerResourceVersion" VALUE="');
    oa.push(bobj.crv.ActxPrintControl_Version);
    oa.push('">');
    
    if (this.lcid) {
        oa.push('<PARAM NAME="LocaleID" VALUE="');
        oa.push(this.lcid);
        oa.push('">');
    }
    
    if (this.url) {
        oa.push('<PARAM NAME="URL" VALUE="');
        oa.push(this._getRPSafeURL(this.url));
        oa.push('">');
    }
    
    if (this.title) {
        oa.push('<PARAM NAME="Title" VALUE="');
        oa.push(this.title);
        oa.push('">');
    }
    
    if (this.maxPage) {
        oa.push('<PARAM NAME="MaxPageNumber" VALUE="');
        oa.push(this.maxPage);
        oa.push('">');
    }
    
    if (this.paperOrientation) {
        oa.push('<PARAM NAME="PageOrientation" VALUE="');
        oa.push(this.paperOrientation);
        oa.push('">');
    }
    
    if (this.paperSize) {
        oa.push('<PARAM NAME="PaperSize" VALUE="');
        oa.push(this.paperSize);
        oa.push('">');
    }
    
    if (this.paperWidth) {
        oa.push('<PARAM NAME="PaperWidth" VALUE="');
        oa.push(this.paperWidth);
        oa.push('">');
    }
    
    if (this.paperLength) {
        oa.push('<PARAM NAME="PaperLength" VALUE="');
        oa.push(this.paperLength);
        oa.push('">');
    }
    
    if (this.driverName) {
        oa.push('<PARAM NAME="PrinterDriverName" VALUE="');
        oa.push(this.driverName);
        oa.push('">');
    }
    
    if (this.useDefPrinter) {
        oa.push('<PARAM NAME="UseDefaultPrinter" VALUE="');
        oa.push(this.useDefPrinter);
        oa.push('">');
    }
    
    if (this.useDefPrinterSettings) {
        oa.push('<PARAM NAME="UseDefaultPrinterSettings" VALUE="');
        oa.push(this.useDefPrinterSettings);
        oa.push('">');
    }
    
    if (this.sendPostDataOnce) {
        oa.push('<PARAM NAME="SendPostDataOnce" VALUE="');
        oa.push(this.sendPostDataOnce);
        oa.push('">');
    }
    oa.push('</OBJECT>');
	
   // Add waiting UI while the control is loading
    oa.push('<table id="')
    oa.push(this.actxId);
    oa.push('_wait" border="0" cellspacing="0" cellpadding="0" width="100%" ><tbody>');
    oa.push('<tr><td align="center" valign="top">');
	
    // Frame Zone
    var o = this;
    var zoneW=o.getContainerWidth()-10;
    var zoneH=o.getContainerHeight()-(2*o.pad+21+10);
    oa.push('<table style="');
    oa.push(sty("width",zoneW));
    oa.push(sty("height",zoneH));
    oa.push('" id="frame_table_');
    oa.push(o.id);
    oa.push('" cellspacing="0" cellpadding="0" border="0"><tbody><tr><td valign="top" class="dlgFrame" style="padding:5px" id="frame_cont_');
    oa.push(o.id);
    oa.push('">');
    
    oa.push('<table border="0" cellspacing="0" cellpadding="0" width="100%"><tbody>');
    oa.push('<tr><td align="center" style="padding-top:5px;">');
    oa.push(img(_skin+'wait01.gif',200,40));
    oa.push('</td></tr>');
    oa.push('<tr><td align="left" style="padding-left:2px;padding-right:2px;padding-top:5px;">');
    oa.push('<div class="icontext" style="wordWrap:break_word;">');
    oa.push(convStr(L_bobj_crv_PrintControlProcessingMessage,false,true));
    oa.push('</div></td></tr></tbody></table>');
    
    oa.push('</td></tr></tbody></table>');
    
    oa.push('</td></tr></tbody></table>');
    
    return oa.join('');
};

bobj.crv.PrintUI._cancel = function () {
    if (this.isActxPrinting) {
        document.getElementById(this.actxContainerId).innerHTML = '';
        this._processingPrinting = false;
    }
};

bobj.crv.PrintUI._processPrinting = function(){
    if (!this._processingPrinting){
        var o = document.getElementById(this.actxId);
        var w = document.getElementById(this.actxId + '_wait');
        if (o && w){
           o.width = "100%";
           o.height = "100%";
           w.style.display="none";
        }
        this._processingPrinting = true;
    }
    
};

bobj.crv.PrintUI.show = function(visible, postBackData) {
    this._processingPrinting = false;
    if (visible) {
        if (!this.layer) {
            targetApp(this.getHTML());
            this.init();
        }
        if (this.isActxPrinting) {
            document.getElementById(this.actxContainerId).innerHTML = this._getObjectTag(postBackData);
        }
        this._showOld(true);
    } 
    else if (this.layer) {
        this._showOld(false);
    }
};

bobj.crv.PrintUI.init = function() {
    this._initOld();
    
    if (!this.isActxPrinting) {
        this._fromBox.init();
        this._toBox.init();
        this._submitBtn.init();
            
        this._allRadio.init();
        this._rangeRadio.init();
        
        this._allRadio.check(true);
        this._toBox.setDisabled(true);
        this._fromBox.setDisabled(true);        
        
        if (this.widgetType == 'ExportUI') {
            this._updateExportList();
        }
    }
};

bobj.crv.PrintUI.getHTML = function(){
    var h = bobj.html;
    var o = this;
    
    var html =  o.beginHTML();

    if (!this.isActxPrinting) {
        html += "<table cellspacing=0 cellpadding=0 border=0>" +
                    "<tr>" +
                        "<td>" +
                            "<div class='dlgFrame'>" +
                                "<table cellspacing=0 cellpadding=0 border=0 style='height:" + (this.height * 0.9) +"px;width:" + this.width + "px;'>" +
                                    "<tr>" +
                                        "<td valign='top' class='naviBarFrame naviFrame'>" +
                                            (this.isExporting ? this._getExportList() : "") +
                                            "<fieldset style='border:0px;padding:0px'>" +
                                                "<legend style='position:relative;"+(_ie?"margin:0px -7px":"")+"'>" +
                                                    "<table datatable='0' style='width:100%;line-height:10px;'>" +
                                                        "<tr>" +
                                                            (_ie?"<td class='dialogTitleLevel2'><label>":"<td class='dialogTitleLevel2'><label>") + 
                                                            L_bobj_crv_PrintRangeLbl + "</label></td>" +
                                                            "<td class='dialogTitleLevel2Underline' style='width:100%'>&nbsp;</td>" +
                                                            "</tr>" +
                                                    "</table>" +
                                                "</legend>" +
                                                "<div style='margin:10px 25px;'>" +
                                                    o._allRadio.getHTML() +
                                                    o._rangeRadio.getHTML() +
                                                    "<div style='padding-left:25px'>" +
                                                        "<table class=dlgContent datatable='0'>" +
                                                            "<tr>" +
                                                                "<td align=right>" +
                                                                    "<label for='" + o._fromBox.id + "'> " + L_bobj_crv_PrintFromLbl + "</label>" +
                                                                "</td>" +
                                                                "<td align=left> " +
                                                                    o._fromBox.getHTML() +
                                                                "</td>" +
                                                            "</tr>" +
                                                            "<tr>" +
                                                                "<td align=right>" +
                                                                    "<label for='" + o._toBox.id + "'> " + L_bobj_crv_PrintToLbl + "</label>" +
                                                                "</td>" +
                                                                "<td align=left>" +
                                                                	o._toBox.getHTML() +
                                                                "</td>" +
                                                            "</tr>" +
                                                        "</table>" +
                                                    "</div>" +
                                                "</div>" +
                                            "</fieldset>" +
                                            (!this.isExporting ? 
                                                "<table style='width:100%;line-height:10px;'>" +
                                                    "<tr>" +
                                                        "<td class='dialogTitleLevel2' tabIndex=0><label>" + this.infoTitle + "</label></td>" +
                                                        "<td class='dialogTitleLevel2Underline' style='width:100%'>&nbsp;</td>" +
                                                    "</tr>" +
                                                "</table>" +
                                                "<div style='margin:10px 0px 10px 25px;' class='dlgHelpText'>" +
                                                    this.infoMsg +
                                                "</div>" 
                                            : '') +
                                        "</td>" +
                                    "</tr>" +
                                "</table>" +
                            "</div>" +
                        "</td>" + 
                    "</tr>" +
                    "<tr>" +
                        "<td align='right' valign='top'>" +
                            "<table style='margin:6px 9px 0px 0px' cellspacing=0 cellpadding=0 border=0><tbody><tr>" +
                                "<td>" +                        
                                    this._submitBtn.getHTML() +
                                "</td></tbody></tr>" +
                            "</table>" +
                        "</td>" +
                    "</tr>" +
                "</table>";


    }
    else {
        html += "<div id='" + this.actxContainerId + "'></div>" +
                '<script for="' + this.actxId + '" EVENT="Finished(status, statusText)" language="javascript">' +
                    'getWidgetFromID("' + this.id + '").show(false);' +
                '</script>' +
                '<script for="' + this.actxId + '" EVENT="PrintingProgress(pageNumber)" language="javascript">' +
                    'getWidgetFromID("' + this.id + '")._processPrinting();' +
                '</script>' ;
    }
    
    html += o.endHTML();
    html +=  bobj.crv.getInitHTML(this.widx);
    
    return html;
};

/*
================================================================================
ExportUI
================================================================================
*/
bobj.crv.newExportUI = function(kwArgs) {
    kwArgs = MochiKit.Base.update({ submitBtnLabel:L_bobj_crv_ExportBtnLbl,
                                    dialogTitle:L_bobj_crv_ExportDialogTitle,
                                    infoTitle:L_bobj_crv_ExportInfoTitle,
                                    infoMsg:L_bobj_crv_PrintInfo1,
                                    isExporting:true}, kwArgs);
    
    var o = bobj.crv.newPrintUI(kwArgs);
    
    o._comboBox = newCustomCombo(
        o.id + "_combo",
        MochiKit.Base.bind(bobj.crv.ExportUI._onSelectFormat, o),
        false,
        270,
        L_bobj_crv_ExportFormatLbl,
        _skin + "../transp.gif", // Screen reader can't read its text without transp.gif
        0,
        14);

    //Adjustment to combo box after adding transp.gif as icon
    if(o._comboBox) {
        o._comboBox.icon.border=0;
        o._comboBox.icon.h=14;
        o._comboBox.arrow.h=12;
        o._comboBox.arrow.dy+=2
        o._comboBox.arrow.disDy+=2
    }
    
    o.widgetType = 'ExportUI';
    MochiKit.Base.update(o, bobj.crv.ExportUI);
    
    return o;
};

bobj.crv.ExportUI._onSelectFormat = function() {
    var format = this._comboBox.getSelection().value;
    if (format == 'CrystalReports' || format == 'RPTR' || format == 'RecordToMSExcel' || format == 'RecordToMSExcel2007' || format == 'CharacterSeparatedValues' || format == 'XML') {
        this._fromBox.setDisabled(true);
        this._toBox.setDisabled(true);
        
        this._rangeRadio.check(false);
        this._rangeRadio.setDisabled(true);
        
        this._allRadio.check(true);
    }
    else {
        this._rangeRadio.setDisabled(false);
    }
};

bobj.crv.ExportUI.update = function (update) {
    if (!update || update.cons !== "bobj.crv.newExportUI") {
        return;
    }
    
    this.availableFormats = update.args.availableFormats;
    
    if(this._comboBox.initialized()) {
        /*
         * Uninitialized combobox is not required to be updated as it will
         * be updated during initialization
         */
        this._updateExportList();
    }
};

bobj.crv.ExportUI._updateExportList = function() {
    if(!this._comboBox.initialized()) {
        this._comboBox.init();
    }
    
    this._updateComboItems();
    
    var item0 = this._comboBox.getItemByIndex(0);
    if(item0 != null)
        this._comboBox.selectItem(item0);

    this._onSelectFormat();
};

bobj.crv.ExportUI._updateComboItems = function () {
    this._comboBox.removeAllMenuItems();
    var itemsCount = (bobj.isArray(this.availableFormats) ? this.availableFormats.length : 0);
    
    for (var i = 0; i < itemsCount; i++) {
        var item = this.availableFormats[i];
        this._comboBox.add(item.name, item.value, item.isSelected);
    }
};

bobj.crv.ExportUI._getExportList = function() {	
    return "<table datatable='0' style='width:100%;line-height:10px;'>" +
                "<tr>" +
                (_ie?"<td class='dialogTitleLevel2'><label>":"<td class='dialogTitleLevel2'><label>") + 
                		L_bobj_crv_ExportFormatLbl + "</label></td>" +
                    "<td class='dialogTitleLevel2Underline' style='width:100%'>&nbsp;</td>" +
                "</tr>" +
            "</table>" + 
            "<div style='margin:10px 25px;'>" +
                this._comboBox.getHTML() + 
            "</div>";
};

/*
================================================================================
ErrorDialog

TODO Dave: If time permits, make dialog resizable with mouse
================================================================================
*/

/**
 * Static function.
 * @returns [ErrorDialog] Returns a shared Error Dialog
 */
bobj.crv.ErrorDialog.getInstance = function() {
    if (!bobj.crv.ErrorDialog.__instance) {
        bobj.crv.ErrorDialog.__instance = bobj.crv.newErrorDialog();
    }
    return bobj.crv.ErrorDialog.__instance;
};

bobj.crv.newErrorDialog = function(kwArgs) {
    kwArgs = MochiKit.Base.update({ 
        id: bobj.uniqueId(),
        title: L_bobj_crv_Error,
        text: null,
        detailText: null,
        okLabel: L_bobj_crv_OK,
        promptType: _promptDlgCritical
    }, kwArgs);
    
    var o = newPromptDialog(
        kwArgs.id,
        kwArgs.title,
        kwArgs.text,
        kwArgs.okLabel,
        null,   // cancelLabel
        kwArgs.promptType,
        null,   // yesCB
        null,   // noCB,
        true,   // noCloseButton
        true);  // isAlert
    
    o.widgetType = 'ErrorDialog';
    
    // Update instance with constructor arguments
    bobj.fillIn(o, kwArgs);

    // Update instance with member functions
    o._promptDlgInit = o.init;
    o._promptDialogSetText = o.setText;
    o._promptDialogShow = o.show;
    o._promptDialogSetTitle = o.setTitle;
    o._promptDialogSetPromptType = o.setPromptType;
    MochiKit.Base.update(o, bobj.crv.ErrorDialog);
    
    o.noCB = MochiKit.Base.bind(o._onClose, o);
    o.yesCB = o.noCB;
    
    o._detailBtn = newIconWidget(
        o.id + "_detailBtn", 
        bobj.skinUri('../help.gif'), 
        MochiKit.Base.bind(bobj.crv.ErrorDialog._onDetailBtnClick, o),  
        L_bobj_crv_showDetails, // Text
        L_bobj_crv_showDetails, // Tooltip 
        16,16,0,0,22,0,
        true); // Tabbing is enabled
        
    return o;
};

bobj.crv.ErrorDialog.init = function() {
    this._promptDlgInit();
    this._detailBtn.init();
    this._detailRow = document.getElementById(this.id + '_detRow');
    this._detailArea = document.getElementById(this.id + '_detArea'); 
    
    if (!this.detailText) {
        this._detailBtn.show(false);    
    }
};

bobj.crv.ErrorDialog.getHTML = function() {
    var TABLE = bobj.html.TABLE;
    var TBODY = bobj.html.TBODY; 
    var TR = bobj.html.TR;
    var TD = bobj.html.TD;
    var PRE = bobj.html.PRE;
    var DIV = bobj.html.DIV;
    
    var imgPath = PromptDialog_getimgPath(this.promptType);
    var imgAlt = PromptDialog_getimgAlt(this.promptType);		
    
    var width = "320";	
    var detailWidth = "300px"; 
    var detailHeight = "100px";
    
    var contentHTML = 
        TABLE({'class':"dlgBody", width: width, cellpadding:"0", cellspacing:"5", border:"0"},
            TBODY(null,
                TR(null, TD(null,
                    TABLE({'class':"dlgBody", cellpadding:"5", cellspacing:"0", border:"0"},                                       
                        TBODY(null,
                            TR(null, 
                                TD({align:"right", width:"32"}, 
                                    img(imgPath, 32, 32, null, 'id="dlg_img_' + this.id + '"', imgAlt)),
                                TD(),
                                TD({id:"dlg_txt_" + this.id, align:"left"},
                                    DIV({tabindex:"0"}, convStr(this.text, false, true)))))))),
                TR({id: this.id + '_detRow', style: {display: "none"}}, 
                    TD(null, DIV({'class': "infozone", style: {width: detailWidth, 'height': detailHeight, overflow: "auto"}},
                        PRE({id: this.id + '_detArea'}, this.detailText)))),                                    
                TR(null, TD(null, getSep())),
                TR(null, TD(null,
                    TABLE({cellpadding:"5", cellspacing:"0", border:"0", width:"100%"},
                        TBODY(null, 
                            TR(null,
                                TD({align:"left"}, this._detailBtn.getHTML()),
                                TD({align:"right"}, this.yes.getHTML())))))))); 
                    
            
    return this.beginHTML() + contentHTML + this.endHTML();
};

/**
 * Set the error message and detail text.
 *
 * @param text       [String] Error message
 * @param detailText [String] Detailed error message or technical info
 */
bobj.crv.ErrorDialog.setText = function (text, detailText) {
    this.text = text;
    this.detailText = detailText;
    
    if (this.layer) {
        this._promptDialogSetText(text || '');
        
        if (this._detailArea) {
            this._detailArea.innerHTML = detailText || '';
        }
        
        var showDetails = detailText ? true : false;
        this._detailBtn.show(showDetails);
        if (!showDetails) {
            this.showDetails(false);    
        }
    }
};

bobj.crv.ErrorDialog.setTitle = function (title) {
    this.title = title;
    if (this.layer) {
        this._promptDialogSetTitle(title || '');
    }
};

bobj.crv.ErrorDialog.setPromptType = function (promptType) {
    this.promptType = promptType;
    if (this.layer) {
        this._promptDialogSetPromptType(promptType);
    }
};

/**
 * Show/Hide the dialog
 *
 * @param isShow  [bool(=true)]  True value displays the dialog, false hides it.
 * @param closeCB [function]     Callback to call after the next close event
 */
bobj.crv.ErrorDialog.show = function(isShow, closeCB) {
    if (typeof isShow == 'undefined') {
        isShow = true;
    }
    
    if (isShow) {
        this._closeCB = closeCB;
        if (!this.layer) {
            targetApp(this.getHTML());
            this.init();
        }
        this.layer.onkeyup = DialogBoxWidget_keypress;
        DialogBoxWidget_keypress = MochiKit.Base.noop;
        
        this._promptDialogShow(true);
    } 
    else if (this.layer){
        this._closeCB = null;
        this._promptDialogShow(false);
    }
};

/**
 * Show/Hide the detailed error message
 *
 * @param isShow [bool(=true)]  True value displays the details, false hides them.
 */
bobj.crv.ErrorDialog.showDetails = function(isShow) {
    if (typeof isShow == 'undefined') {
        isShow = true;
    }
    
    if (this._detailRow && this._detailBtn) {
        if (isShow) {
            this._detailRow.style.display = '';
            this._detailBtn.changeText(L_bobj_crv_hideDetails);
        }
        else {
            this._detailRow.style.display = 'none';
            this._detailBtn.changeText(L_bobj_crv_showDetails);
        }
    }
};

/**
 * Private. Handles detail button clicks.
 */
bobj.crv.ErrorDialog._onDetailBtnClick = function() { 
    if (this._detailRow) {
        this.showDetails(this._detailRow.style.display == 'none');
    }
};

/**
 * Private. Notifies listener that dialog has closed;
 */
bobj.crv.ErrorDialog._onClose = function() {
    if (this._closeCB) {
        this._closeCB();
        this._closeCB = null;
    }
    DialogBoxWidget_keypress = this.layer.onkeyup;
    this.layer.onkeyup = null;
};

/*
================================================================================
Report Processing Dialog
================================================================================
kwArgs
delay   - the wait time prior to showing the dialog.
message - a customized message to display in the dialog.
*/
bobj.crv.newReportProcessingUI = function(kwArgs) {
    kwArgs = MochiKit.Base.update({
        id: bobj.uniqueId(),
        delay: 250,
        message: L_bobj_crv_ReportProcessingMessage
    }, kwArgs);
    
    /* Since JSON escapes the '\' in unicode character references (\uXXXX) in Viewer
     * process indicator text is converted to html numeric referece (&#ddddd) which Javascript
     * don't display as expected. Little hack here to it as HTML string */
    var d = document.createElement('div');
    d.style.visibility = 'hidden';
    d.innerHTML = kwArgs.message;
    var newMsg = d.innerHTML;
    d = null;
    
    var o = newWaitDialogBoxWidget(
        kwArgs.id,          // id
        0,                  // width
        0,                  // height
        '',                 // title
        false,               // show cancel
        bobj.crv.ReportProcessingUI.cancelCB,               // cancel callback
        true,               // show label
        newMsg,	            // label text 
        true                // no close button
        );				
    
    
    o.widgetType = 'ReportProcessingUI';
    o.delay = kwArgs.delay;
    
    // Update instance with member functions
    MochiKit.Base.update(o, bobj.crv.ReportProcessingUI);
    
    return o;
};

bobj.crv.reportProcessingDialog = null;
bobj.crv.timerID = null;

bobj.crv.ReportProcessingUI.cancelCB = function ()
{
    bobj.crv.reportProcessingDialog.cancelled = true;

    if (bobj.crv.reportProcessingDialog.deferred !== null) {
        bobj.crv.reportProcessingDialog.deferred.cancel ();
    }

    bobj.crv.reportProcessingDialog.cancelShow ();
};

bobj.crv.ReportProcessingUI.wasCancelled = function ()
{
    return bobj.crv.reportProcessingDialog.cancelled;
};

bobj.crv.ReportProcessingUI._prepareToShow = function () {
    // cleanup any existing dialog?
    if (bobj.crv.reportProcessingDialog !== null) {
        bobj.crv.reportProcessingDialog.cancelShow ();
    }
    
    if (!this.layer) {
        append2(document.body, this.getHTML());
        this.init();
    }

    this.deferred = null;
    bobj.crv.reportProcessingDialog = this;
};

bobj.crv.ReportProcessingUI.Show = function () {
	this._prepareToShow();
	bobj.crv.reportProcessingDialog.show(true);
};

bobj.crv.ReportProcessingUI.delayedShow = function () {
	this._prepareToShow();
	bobj.crv.timerID = setTimeout("bobj.crv._showReportProcessingDialog ()", bobj.crv.reportProcessingDialog.delay);
};
    
bobj.crv.ReportProcessingUI.cancelShow = function () {
    if (bobj.crv.timerID) {
        clearTimeout (bobj.crv.timerID);
    }
            
    if (bobj.crv.reportProcessingDialog){
        bobj.crv.reportProcessingDialog.show (false);
    }
    
    bobj.crv.reportProcessingDialog = null;
    bobj.crv.timerID = null;
};

bobj.crv.ReportProcessingUI.setDeferred = function (deferred) {
    bobj.crv.reportProcessingDialog.deferred = deferred;
    
    if (bobj.crv.reportProcessingDialog.wasCancelled () === true) {
        deferred.cancel ();
    }
};

bobj.crv._showReportProcessingDialog = function () {
    if (bobj.crv.reportProcessingDialog && bobj.crv.reportProcessingDialog.delay !== 0) {
        bobj.crv.logger.info('ShowReportProcessingDialog');
        bobj.crv.reportProcessingDialog.show (true);
    }
};

/********************************************************
 * Module Name    :     Application
 * Purpose        :     open dialog to add accounting and process it
 * Author         :     Raghu
 * Date           :     21-july-2015
  ******************************************************/
; VIS = window.VIS || {};

; (function (VIS, $) {

    /**
    *	Parameter Dialog.
    *	- called from ProcessCtl
    *	- checks, if parameters exist and inquires and saves them
    *  @class ParameterDialog
    */

    function ParameterDialog(windowNop, parent) {
        var _parent = parent;
        //Variabls
        this.windowNo = windowNop;
        this.processId = null;
        this.name = null;
        this.entityType = null;
        this.pInstanceId = 0;
        this.className = null;
        this.fields = null;
        this.paraList = null;
        this.asyncProcess = true;

        this.gridFields = [];
        this.depOnFieldColumn = [];
        this.depOnField = [];

        this.vEditors = [];
        this.vEditors2 = [];
        this.mFields = [];
        this.mFields2 = [];
        this.isOkClicked = false;
        this.onClosed;
        var $root, $table, $tr, $td1, $td2, $td3, $btnOK, $btnClose;
        var $busyDiv = $("<div class='vis-apanel-busy' style='width:95%;height:98%;position:absolute'>");
        //Privilized functions
        var self = this;
        this.onCloseMain = null;
        this.isOkProcess = false;

        //call instance genration
        function setBusy(isBusy) {
            $busyDiv.css("display", isBusy ? 'block' : 'none');
        };

        // updated for frpt COA by vinay bhatt
        this.process = function (id, name, windowNo) {
            //var data = {
            //    Process_ID: id, 
            //    Name: name,
            //    AD_Table_ID: 0,
            //    Record_ID: 0,
            //    WindowNo: windowNo,
            //    csv: false,
            //    pdf: false
            //}

            //var processUrl = VIS.Application.contextUrl + "JsonData/Process";
            //$.ajax({
            //    url: processUrl,
            //    type: "GET",
            //    datatype: "json",
            //    async: this.asyncProcess,
            //    data: data
            //}).done(function (json) {
            //    if (json) {
            //        loadfieldspara(json);
            //    }
            //})

            var pInfo = new VIS.ProcessInfo(name, id, 0, 0);
            pInfo.setAD_PInstance_ID(self.pInstanceId);
            pInfo.setWindowNo(windowNo);

            var data = { processInfo: pInfo.toJson() };
            if (this.asyncProcess) {
                VIS.dataContext.process(data, function (jsonStr) {
                    loadfieldspara(jsonStr);
                });
            }
            else {
                var jsonStr = VIS.dataContext.process(data);
                loadfieldspara(jsonStr);
            }
        };

        function loadfieldspara(jsonStr) {
            if (jsonStr.error != null) {
                return;
            }
            var json = JSON.parse(jsonStr.result);
            if (json.IsError) {
                return;
            }

            if (json.ShowParameter) { //Open Paramter Dialog
                self.pInstanceId = json.AD_PInstance_ID;
                self.fields = json.ProcessFields;
            }
            else {
                if (json.ReportProcessInfo != null) {
                    VIS.ADialog.info(json.ReportProcessInfo.Summary, true, "", "");

                    if (VIS.Msg.getMsg("ImportedSuccessfully") == json.ReportProcessInfo.Summary) {
                        _parent.parentcall(true);
                        if (self)
                            self.isOkProcess = true;
                        return true;
                    }
                }
                if (self)
                    self.isOkProcess = false;
                _parent.parentcall(false);
                return false;
            }
        }

        function initlizedComponent() {

            $root = $("<div title='ParameterDialog'>");
            $table = $("<table class='vis-processpara-table'>");
            $root.append($table);

            $btnOK = $('<input type="button" style="margin-right:5px" >').val(VIS.Msg.getMsg("OK"));
            $btnOK.addClass("VIS_Pref_btn-2");
            $btnClose = $("<input type='button'>").val(VIS.Msg.getMsg("Close"));
            $btnClose.addClass("VIS_Pref_btn-2");

            $btnClose.css({ "margin-bottom": "0px", "margin-top": "7px" });
            $btnOK.css({ "margin-bottom": "0px", "margin-top": "7px" });
        };
        //call
        initlizedComponent();

        this.addLine = function () {
            $tr = $td1 = $td2 = $td3 = null;

            $td1 = $("<td class=''>");
            $td2 = $("<td class=''>");
            $td3 = $("<td class='vis-processpara-table-td3'>");
            $table.append($("<tr>").append($td1).append($td2).append($td3));
        };

        this.addFields = function (c1, c2) {
            if (c1)
                $td1.append(c1.getControl());
            if (c2) {
                c2.getControl().width("200px");
                c2.getControl().height("30px");
                $td3.append(c2.getControl());
                if (c2.getBtnCount() > 0) {
                    var btn = c2.getBtn(0);
                    $td3.append(btn);

                    if (c2.getDisplayType() == VIS.DisplayType.MultiKey) {
                        $td3.append(c2.getBtn(1));
                    }

                }
            }
        };

        this.addButtons = function () {
            this.addLine();
            $td3.append($btnClose).append($btnOK);
        };

        this.showDialog = function () {
            $root.append($busyDiv);
            $root.dialog({
                modal: true,
                width: "auto",
                close: function () {
                    onProcessDialogClosed(self.isOkClicked, self.parameterList);
                    if (self.onCloseMain)
                        self.onCloseMain(self.isOkClicked);
                    self.dispose();
                    self = null;
                    setBusy(false);
                }
            });
            window.setTimeout(function () {
                $btnClose.focus();
            }, 200);
            setBusy(false);
        };

        // updated for frpt COA by vinay bhatt
        function onProcessDialogClosed(isOK, paraList) {
            if (isOK) { //Ok clicked
                self.paraList = paraList;
                //var data = {
                //    Process_ID: self.processId,
                //    AD_PInstance_ID: self.pInstanceId,
                //    Name: self.name,
                //    AD_Table_ID: 0,
                //    Record_ID: 0,
                //    ParameterList: paraList,
                //    csv: false,
                //    pdf: false
                //}

                //var executeProcessUrl = VIS.Application.contextUrl + "JsonData/ExecuteProcess";
                //$.ajax({
                //    url: executeProcessUrl,
                //    type: "POST",
                //    datatype: "json",
                //    contentType: "application/json; charset=utf-8",
                //    //async: self.asyncProcess,
                //    async: true,
                //    data: JSON.stringify(data)
                //}).done(function (json) {
                //    if (json) {
                //        loadfieldspara(json);
                //    }
                //})

                var pInfo = new VIS.ProcessInfo(self.name, self.processId, 0, 0);
                pInfo.setAD_PInstance_ID(self.pInstanceId);

                VIS.dataContext.executeProcess({ processInfo: pInfo.toJson(), parameterList: paraList }, function (jsonStr) {
                    loadfieldspara(jsonStr);
                });
            }
            else {
                //close form
            }
        };


        this.onClose = function (isOkClicked) {
            self.isOkClicked = isOkClicked
            $root.dialog('close');
        };

        /* Events */
        $btnClose.on(VIS.Events.onTouchStartOrClick, function () { self.onClose(false); });

        $btnOK.on(VIS.Events.onTouchStartOrClick, function () {
            setBusy(true);
            var list = self.saveParameters();
            if (!list) {
                setBusy(false);
                return;
            }
            self.parameterList = list;
            self.onClose(true, list);
        });

        this.disposeComponent = function () {
            $btnClose.off(VIS.Events.onTouchStartOrClick);
            $btnOK.off(VIS.Events.onTouchStartOrClick);

            $root.dialog('destroy');
            $root.remove();
            self = null;

            this.gridFields.length = 0;
            this.gridFields = null;

            this.depOnFieldColumn.length = 0;
            this.depOnFieldColumn - null;

            this.depOnField.length = 0;
            this.depOnField = null;

            this.vEditors.length = 0;
            this.vEditors2.length = 0;
            this.mFields.length = 0;
            this.mFields2.length = 0;

            //this.pi = null;
            this.parameterList = null;

            this.addLine = null;
            this.addFields = null;
            this.addButtons = null;
            this.showDialog = null;
            this.onClose = null;

            this.isOkClicked = null;

            $root = $table = $tr = $td1 = $td2 = $td3 = $btnOK = $btnClose = null;;

        };
    };


    /**
	 *	Read and Create Fields to display
	 *	- creates Fields and adds it mFields list
	 *  - creates Editor and adds it to vEditors list
	 *  Handeles Ranges by adding additional mField/vEditor.
	 *  <p>
	 *  mFields are used for default value and mandatory checking;
	 *  vEditors are used to retrieve the value (no data binding)
	 *  @param {array object}  fields
	 *  @return true if loaded OK
	 */
    ParameterDialog.prototype.initDialog = function (processIDp, namep, typep, classNamep, asyn) {
        this.processId = processIDp;
        this.name = namep;
        this.entityType = typep;
        this.className = classNamep;
        if (typeof asyn != 'undefined') {
            this.asyncProcess = asyn;
        }

        this.process(processIDp, namep, this.windowNo);
        var fields = this.fields;
        var mField = null;
        for (var i = 0, len = fields.length; i < len; i++) {
            mField = new VIS.GridField(fields[i]);
            this.addLine(); // add new line
            this.mFields.push(mField);
            var list = mField.getDependentOn(false); //dependents file

            for (var j = 0; j < list.length; j++) {
                this.depOnField.push(mField); // ColumnName, Field
                this.depOnFieldColumn.push(list[j]);
            }

            var label = VIS.VControlFactory.getLabel(mField); //get label
            //	The Editor
            var vEditor = VIS.VControlFactory.getControl(null, mField, false); //get control
            if (vEditor) {
                var defaultValue = mField.getDefault(VIS.context, this.windowNo);
                vEditor.setValue(defaultValue);
                vEditor.addVetoableChangeListener(this);
            }
            //  GridField => VEditor - New Field value to be updated to editor
            mField.setPropertyChangeListener(vEditor);
            //

            this.vEditors.push(vEditor);                   //  add to Editors
            this.addFields(label, vEditor);

            if (mField.getIsRange()) {
                this.addLine(); // add new line
                var vof2 = {};
                $.extend(true, vof2, fields[i]);

                var mField2 = new VIS.GridField(vof2);
                this.mFields2.push(mField2);
                //	The Editor
                var vEditor2 = VIS.VControlFactory.getControl(null, mField2, false);
                //  New Field value to be updated to editor
                if (vEditor2) {

                    var defaultValue = mField2.getDefault(VIS.context, this.windowNo);
                    vEditor2.setValue(defaultValue);
                    vEditor2.addVetoableChangeListener(this);
                }
                //
                this.vEditors2.push(vEditor2);
                this.addFields(null, vEditor2);
            }
            else {
                this.mFields2.push(null);
                this.vEditors2.push(null);
            }
        }
        this.addButtons();
        return true;
    };

    /**
	 *	Editor Listener
	 *	@param {object} evt Event
	 */
    ParameterDialog.prototype.vetoablechange = function (evt) {
        console.log(evt);
        var value = evt.newValue == null ? "" : evt.newValue.toString();
        var columnName = evt.propertyName;
        VIS.Env.getCtx().setWindowContext(this.windowNo, columnName, value);

        if (this.depOnFieldColumn.indexOf(columnName) !== -1) {

            var dependentFields = this.getDependantFields(columnName);
            for (var i = 0, len = dependentFields.length; i < len; i++) {
                //var dep = field;
                //field is not defined
                var dep = dependentFields[i];
                if (dep == null)
                    continue;
                dep.refreshLookup();
                dep.setValue(dep.getDefault(VIS.Env.getCtx(), this.windowNo), true);
            }
        }
    };

    /**
      get dependent fields against column
      @method getDependantFields
      @param {string} columnName
      @return array of GridFeild objects 
    */
    ParameterDialog.prototype.getDependantFields = function (columnName) {
        var list = [];
        if (this.depOnFieldColumn.indexOf(columnName) != -1) {
            var size = this.depOnFieldColumn.length;
            for (var i = 0; i < size; i++) {
                if (this.depOnFieldColumn[i].equals(columnName))
                    if (list.indexOf(this.depOnField[i]) < 0)
                        list.push(this.depOnField[i]);
            }
        }
        return list;
    };

    /**
	 * Save Parameter values
	 * @return true if parameters saved
	 */
    ParameterDialog.prototype.saveParameters = function () {
        //Mandatory Fields
        var sb = new StringBuilder();
        var size = this.mFields.length;
        var i = 0;
        for (i = 0; i < size; i++) {
            var field = this.mFields[i];
            if (field.getIsMandatory(true)) {        //  check context
                var vEditor = this.vEditors[i];
                var data = vEditor.getValue();
                if ((data == null) || (data.toString().length == 0)) {
                    field.setInserting(true);  //  set editable (i.e. updateable) otherwise deadlock
                    field.setError(true);
                    if (sb.length() > 0)
                        sb.append(", ");
                    sb.append(field.getHeader());
                }
                else
                    field.setError(false);
                //  Check for Range
                var vEditor2 = this.vEditors2[i];
                if (vEditor2 != null) {
                    var data2 = vEditor.getValue();
                    var field2 = this.mFields2[i];
                    if ((data2 == null) || (data2.toString().length == 0)) {
                        field.setInserting(true);  //  set editable (i.e. updateable) otherwise deadlock
                        field2.setError(true);
                        if (sb.length() > 0)
                            sb.append(", ");
                        sb.append(field.getHeader());
                    }
                    else
                        field2.setError(false);
                }   //  range field
            }   //  mandatory
        }   //  field loop
        if (sb.length() != 0) {
            VIS.ADialog.error("FillMandatory", true, sb.toString());
            //alert("FillMandatory", sb.toString());
            return false;
        }


        /**********************************************************************
		 *	Save Now
		 */

        var parameterList = [];
        var para = {};

        for (i = 0; i < size; i++) {
            //	Get Values
            var editor = this.vEditors[i];
            var editor2 = this.vEditors2[i];
            var result = editor.getValue();
            var result2 = null;
            if (editor2 != null)
                result2 = editor2.getValue();

            //	Don't save NULL values
            if ((result == null) && (result2 == null))
                continue;

            //	Create Parameter
            para = {};


            //MPInstancePara para = new MPInstancePara (Env.getCtx(), m_processInfo.getAD_PInstance_ID(), i);
            var mField = this.mFields[i];
            para.Name = mField.getColumnName();
            para.DisplayType = mField.getDisplayType();

            para.Result = result;
            para.Result2 = result2;

            ////	Date
            //if ((result instanceof Timestamp) || (result2 instanceof Timestamp))
            //{
            //    para.setP_Date((Timestamp)result);
            //    if ((editor2 != null) && (result2 != null))
            //        para.setP_Date_To((Timestamp)result2);
            //}
            //    //	Integer
            //else if ((result instanceof Integer) || (result2 instanceof Integer))
            //{
            //    if (result != null)
            //    {
            //        Integer ii = (Integer)result;
            //        para.setP_Number(ii.intValue());
            //    }
            //    if ((editor2 != null) && (result2 != null))
            //    {
            //        Integer ii = (Integer)result2;
            //        para.setP_Number_To(ii.intValue());
            //    }
            //}
            //    //	BigDecimal
            //else if ((result instanceof BigDecimal) || (result2 instanceof BigDecimal))
            //{
            //    para.setP_Number ((BigDecimal)result);
            //    if ((editor2 != null) && (result2 != null))
            //        para.setP_Number_To ((BigDecimal)result2);
            //}
            //    //	Boolean
            //else if (result instanceof Boolean)
            //{
            //    Boolean bb = (Boolean)result;
            //    String value = bb.booleanValue() ? "Y" : "N";
            //    para.setP_String (value);
            //    //	to does not make sense
            //}
            //	String
            //else
            //{
            //    if (result != null)
            //        para.setP_String (result.toString());
            //    if ((editor2 != null) && (result2 != null))
            //        para.setP_String_To (result2.toString());
            //}

            //  Info
            para.Info = editor.getDisplay();

            if (editor2 != null)
                para.Info_To = editor2.getDisplay();
            parameterList.push(para);

        }

        return parameterList;

    };

    /**
         clean up
         @method dispose
    */
    ParameterDialog.prototype.dispose = function () {

        this.disposeComponent();
    };

    // Global Assignment
    VIS.ParameterDialog = ParameterDialog;
})(VIS, jQuery);
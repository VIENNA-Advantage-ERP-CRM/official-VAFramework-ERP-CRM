; (function (VIS, $) {


    /**
    *	Parameter Dialog.
    *	- called from ProcessCtl
    *	- checks, if parameters exist and inquires and saves them
    *  @class ProcessParameter
    */

    function ProcessParameter(pi, parent, windowNo) {
        //Variabls
        this.pi = pi;
        this.parent = parent;
        this.windowNo = windowNo;

        this.gridFields = [];
        this.depOnFieldColumn = [];
        this.depOnField = [];

        this.vEditors = [];
        this.vEditors2 = [];
        this.mFields = [];
        this.mFields2 = [];
        this.isOkClicked = false;

        this.onClosed;


        var $root, $table, $tr, $td1, $td2, $td3, $btnOK, $btnClose, $divButtons, $divTable;

        function initlizedComponent() {

            $root = $("<div title='" + VIS.Msg.getMsg('Process') + "'>");
            $table = $("<table class='vis-processpara-table'>");
            // $root.append($table);
            $divButtons = $('<div style="overflow:auto">');
            $divTable = $('<div style="overflow-y:auto;overflow-x:hidden">');
            $divTable.append($table);
            $root.append($divTable).append($divButtons);
            $btnOK = $('<input type="button" style="margin-right:5px" >').val(VIS.Msg.getMsg("OK"));
            $btnOK.addClass("VIS_Pref_btn-2");
            $btnClose = $("<input type='button'>").val(VIS.Msg.getMsg("Close"));
            $btnClose.addClass("VIS_Pref_btn-2");
            $btnClose.css({ "margin-bottom": "0px", "margin-top": "7px" });
            $btnOK.css({ "margin-bottom": "0px", "margin-top": "7px" });
            $divTable.css('max-height', (window.innerHeight - 200) + 'px');

        };

        initlizedComponent();


        //Privilized functions

        var self = this;

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
            $divButtons.append($btnClose).append($btnOK);
        };

        this.showDialog = function () {
           // if (IsPosReport != "IsPosReport") {
            $root.dialog({
                modal: true,
                width: "auto",
                closeText: VIS.Msg.getMsg("close"),
                close: function () {
                    if (self.parent)
                        self.parent.onProcessDialogClosed(self.isOkClicked, self.parameterList);
                    self.dispose();
                    self = null;
                }
            });
           // }
            //else {
            //    this.IsPosReportForDialog = "IsPosReport";
            //    var x = $root.dialog({
            //        modal: true,
            //        width: "auto",
            //        closeText: VIS.Msg.getMsg("close"),
            //        close: function () {
            //            if (self.parent)
            //                self.parent.onProcessDialogClosed(self.isOkClicked, self.parameterList);
            //            self.dispose();
            //            self = null;
            //        }
            //    });

            //    VAPOS.DashBoardDesign.append(x);
            //    VAPOS.DashBoardDesign.parents().find(".ui-widget-overlay").remove();
            //    VAPOS.DashBoardDesign.parents().find(".ui-dialog").remove();

            //    var getTr = x.find("tr");
            //    for (var i = 0; i < getTr.length ; i++) {
            //        for (var j = 0; j < $(getTr[0]).find("td").length; j++) {
            //            var chiild = $($(getTr[i]).find("td")[j]).children();
            //            if (chiild.length == 0)
            //            { continue; }
            //            if (chiild.length > 2) {
            //                VAPOS.DashBoardDesign.append($('<div class="VAPOS-FormDataRowRap VAPOS-FormDataIcons2">').append(chiild));
            //            }
            //            else if (chiild.length == 2) {
            //                VAPOS.DashBoardDesign.append($('<div class="VAPOS-FormDataRowRap VAPOS-FormDataIcons1">').append(chiild));
            //            }
            //            else {
            //                VAPOS.DashBoardDesign.append($('<div class="VAPOS-FormDataRowRap VAPOS-FormDataIcons0">').append(chiild));
            //            }
            //        }
            //    }

            //    VAPOS.DashBoardDesign.append($('<div>').append($($(x.find("div")[1]).find("input")[1]).css("margin-right", "0px !important")));
            //    VAPOS.DashBoardDesign.find(".ui-dialog-content").remove();
            //    VAPOS.DashBoardDesign.find("input[type='Checkbox']").css({ "height": "auto", "width": "auto !important" });
            //    VAPOS.DashBoardDesign.find(".vis-gc-vpanel-table-label-checkbox").css({ "height": "auto !important", "margin-bottom": "0px" })
            //};
            window.setTimeout(function () {
                $btnClose.focus();
            }, 200);
        };

        this.onClose = function (isOkClicked) {
            self.isOkClicked = isOkClicked
            $root.dialog('close');
        };




        /* Events */
        $btnClose.on(VIS.Events.onTouchStartAndClick, function (e) {
            e.stopPropagation();
            e.preventDefault();
            self.onClose(false);
        });

        $btnOK.on(VIS.Events.onTouchStartAndClick, function (e) {
            e.stopPropagation();
            e.preventDefault();
            var list = self.saveParameters();
            if (!list) {
                return;
            }
            self.parameterList = list;
            //self.onClose(true, list);
            //if (self.IsPosReportForDialog != "IsPosReport") {
                self.onClose(true, list);
           // }
           // else {
            //    self.parent.onProcessDialogClosed(true, self.parameterList, self.IsPosReportForDialog);
            //}
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
            this.parent = null;
            this.parameterList = null;

            this.pi = null;

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
    ProcessParameter.prototype.initDialog = function (fields) {
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

                if (mField.getDisplayType() == VIS.DisplayType.AmtDimension) {
                    vEditor.hideButton(false);
                    vEditor.setReadOnlyTextbox(false);
                    vEditor.getControl().css("width", "100%");
                }

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

                    var defaultValue = mField2.getDefault2(VIS.context, this.windowNo);
                    vEditor2.setValue(defaultValue);
                    vEditor2.addVetoableChangeListener(this);

                    if (mField.getDisplayType() == VIS.DisplayType.AmtDimension) {
                        vEditor2.hideButton(false);
                        vEditor2.setReadOnlyTextbox(false);
                        vEditor2.getControl().css("width", "100%");
                    }


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
    ProcessParameter.prototype.vetoablechange = function (evt) {
        console.log(evt);
        var value = evt.newValue == null ? "" : evt.newValue.toString();
        var columnName = evt.propertyName;
        for (var i = 0; i < this.mFields.length; i++) {
            if (this.mFields[i].getColumnName().toLower() == columnName.toLower()) {
                if (this.mFields[i].getDisplayType() == VIS.DisplayType.YesNo) {
                    if (value.toString() == "Y" || value.toString() == "N") {
                        VIS.Env.getCtx().setWindowContext(this.windowNo, columnName, value.toString());
                    } else {
                        VIS.Env.getCtx().setWindowContext(this.windowNo, columnName, value.toString().toLowerCase() == "true" ? "Y" : "N");
                    }
                    break;
                }
                else {
                    VIS.Env.getCtx().setWindowContext(this.windowNo, columnName, value);
                    break;
                }
            }
        }



        if (this.depOnFieldColumn.indexOf(columnName) !== -1) {

            var dependentFields = this.getDependantFields(columnName);
            for (var i = 0, len = dependentFields.length; i < len; i++) {
                var dep = dependentFields[i];
                if (dep == null)
                    continue;

                // If M_Product_ID and M_Locator_ID is used as parameter in same report, 
                //then on change M_product_ID system clears value of M_Locator_ID, bcoz M_Locator is depednet on M_Product.
                // then in this case we will not refresh lookup of Locator.
                if (columnName == "M_Product_ID" && (dep.getColumnName() == "M_Locator_ID" || dep.getColumnName() == "M_LocatorTo_ID")) {
                    continue;
                }
                else {
                    dep.refreshLookup();
                    dep.setValue(dep.getDefault(VIS.Env.getCtx(), this.windowNo), true);
                }
            }
        }
    };

    /**
      get dependent fields against column
      @method getDependantFields
      @param {string} columnName
      @return array of GridFeild objects 
    */
    ProcessParameter.prototype.getDependantFields = function (columnName) {
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
    ProcessParameter.prototype.saveParameters = function () {
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
                    var data2 = vEditor2.getValue();
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

            if (editor.getDisplayType() == VIS.DisplayType.AmtDimension) {
                result = editor.getText();
            }


            var result2 = null;
            if (editor2 != null) {
                result2 = editor2.getValue();

                if (editor2.getDisplayType() == VIS.DisplayType.AmtDimension) {
                    result = editor2.getText();
                }

            }

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
            para.LoadRecursiveData = mField.getLoadRecursiveData();
            para.ShowChildOfSelected = mField.getShowChildOfSelected();
            para.AD_Column_ID = mField.getAD_Column_ID();
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


            if (editor.getDisplayType() == VIS.DisplayType.MultiKey) {
                para.Info = editor.lastDisplay;
            }

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
    ProcessParameter.prototype.dispose = function () {

        this.disposeComponent();
    };

    // Global Assignment
    VIS.ProcessParameter = ProcessParameter;
})(VIS, jQuery);
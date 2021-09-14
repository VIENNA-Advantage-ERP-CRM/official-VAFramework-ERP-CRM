; (function (VIS, $) {

    function VDocAction(windowNo, tabObj, recordId) {

        var ctx = VIS.Env.getCtx();

        // Change By Lokesh Chauhan 2-Sep
        // To Handle Doc Action's at child Tab
        //var AD_Table_ID = ctx.getContextAsInt(windowNo, "BaseTable_ID");
        var AD_Table_ID = tabObj.getAD_Table_ID();

        var _values = [];
        var _names = [];
        var _descriptions = [];
        var docStatus = null;
        var index = 0;
        var defaultV = "";
        this.log = VIS.Logging.VLogger.getVLogger("VDocAction");
        var options;
        var $cmbAction = $('<select class="vis-select-docAction"></select>');
        var $message = $('<p style="font-size:12px;margin-bottom:-5px"></p>');
        // var $btnbackground = $('<button>');
        //  var $btnok = $('<button>');
        //  var $btncancel = $('<button>');
        var $maindiv = $('<div style="margin-bottom:-5px"></div>');
        var self = this;
        this.batch = false;
        this.isOKPressed = false;
        var $table = $('<table style="width:360px;margin-bottom:9px">');
        var ch = null;
        this.onClose = null;
        var tabmenubusy = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
        var loadLabel = $('<label>' + VIS.Msg.getMsg("Loading") + '</label>');

        if (VIS.Application.isRTL) {
            //$cmbAction.css({ "margin-left": "0px", "margin-right": "10px" });
            //$message.css({ "margin-right": "3px" });
        }

        function init() {
            createDesign();
            events();
            readReference();
            dynInit(recordId);
        };

        init();

        function readReference() {
            var sql;
            if (VIS.Env.isBaseLanguage(ctx, "AD_Ref_List"))//    GlobalVariable.IsBaseLanguage())//   Env.isBaseLanguage(ctx, "AD_Ref_List"))
                sql = "SELECT Value, Name, Description FROM AD_Ref_List "
                    + "WHERE AD_Reference_ID=135 ORDER BY Name";
            else
                sql = "SELECT l.Value, t.Name, t.Description "
                    + "FROM AD_Ref_List l, AD_Ref_List_Trl t "
                    + "WHERE l.AD_Ref_List_ID=t.AD_Ref_List_ID"
                    + " AND t.AD_Language='" + VIS.Env.getAD_Language(ctx) + "'"
                    + " AND l.AD_Reference_ID=135 ORDER BY t.Name";

            var valueLst = [];
            var nameLst = [];
            var descriptionLst = [];

            //  IDataReader dr = null;
            try {
                //var dr = VIS.DB.executeReader(sql, null);
                //while (dr.read()) {
                //    var value = dr.get(0);
                //    var name = dr.get(1);
                //    var description = dr.get(2);
                //    if (description == null || description == 'undefined') {
                //        description = "";
                //    }
                //    //
                //    valueLst.push(value);
                //    nameLst.push(name);
                //    descriptionLst.push(description);
                //}
                //dr.Close();
                //dr.dispose();
                var _sql = VIS.secureEngine.encrypt(sql);
                var dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "DocAction/GetReference", { "RefQry": _sql }, null);
                if (dr != null) {
                    for (var i in dr) {
                        var value = dr[i]["Value"];
                        var name = dr[i]["Name"];
                        var description = dr[i]["Description"];
                        if (description == null || description == 'undefined') {
                            description = "";
                        }
                        //
                        valueLst.push(value);
                        nameLst.push(name);
                        descriptionLst.push(description);
                    }
                }
            }
            catch (e) {
                //if (dr != null) {
                //    dr.close();
                //}
                //this.log.log(VIS.Logging.Level.SEVERE, sql, e);
                console.log(e);
            }
            finally {
                //if (dr != null) {
                //    dr.close();
                //}
            }

            //	convert to arrays
            var size = valueLst.length;
            _values = [];
            _names = [];
            _descriptions = [];
            for (var i = 0; i < size; i++) {
                _values.push(valueLst[i]);
                _names.push(nameLst[i]);
                _descriptions.push(descriptionLst[i]);
            }
        };

        function dynInit(Record_ID) {

            docStatus = tabObj.getValue("DocStatus");
            var docAction = tabObj.getValue("DocAction");
            var processing = tabObj.getValue("Processing");

            //Rakesh(VA228):Check processing return value Y/N in case when reference is button and true/false when reference is Checkbox
            if (processing == "N" || !processing) {
                processing = false;
            }
            else {
                processing = true;
            }
            var orderType = ctx.getWindowContext(windowNo, "OrderType");
            var isSOTrx = ctx.isSOTrx(windowNo);


            //   log.Fine("DocStatus=" + docStatus
            //       + ", DocAction=" + docAction + ", OrderType=" + orderType
            //       + ", IsSOTrx=" + isSOTrx + ", Processing=" + processing
            //       + ", AD_Table_ID=" + AD_Table_ID + ", Record_ID=" + Record_ID);
            //
            //options = new Array(_values.Length);
            //  VIS.dataContext.getDocActions(AD_Table_ID, Record_ID, docStatus, processing, orderType, isSOTrx, docAction, tabObj.getTableName(), _values, _names, generateActions);


            $.ajax({
                url: VIS.Application.contextUrl + 'DocAction/GetDocActions',
                type: 'POST',
                dataType: 'Json',
                data: {

                    AD_Table_ID: AD_Table_ID, Record_ID: Record_ID, docStatus: docStatus, processing: processing, orderType: orderType, isSOTrx: isSOTrx, docAction: docAction,

                    tableName: tabObj.getTableName(), values: JSON.stringify(_values), names: JSON.stringify(_names), C_DocType_ID: tabObj.getValue("C_DocType_ID"), C_DocTypeTarget_ID: tabObj.getValue("C_DocTypeTarget_ID")
                },
                success: function (data) {
                    var result = JSON.parse(data);
                    generateActions(result);
                    fillCombo(result);
                }
            });
        };

        function generateActions(result) {


            setVisibility(false);
            docStatus = result.DocStatus;
            if (result.Error != null && result.Error != "" && result.Error != 'undefined') {
                VIS.ADialog.error(result.Error);
                return;
            }

            if (result.DocStatus == null) {
                $message.text("*** ERROR ***");
                //SetBusy(false);
                return;
            }
            //createDesign();

        };

        function fillCombo(result) {

            /**
             *	Fill actionCombo
             */

            for (var i = 0; i < result.Index; i++) {
                //	Serach for option and add it
                var added = false;
                for (var j = 0; j < _values.length && !added; j++)
                    if (result.Options[i].equals(_values[j])) {
                        //actionCombo.addItem(_names[j]);
                        $cmbAction.append('<option >' + _names[j] + '</option>');
                        added = true;
                    }
            }

            if (result.DefaultV != null && result.DefaultV != "" && result.DefaultV != 'undefined') {
                $cmbAction.val(result.DefaultV).change();
            }

            $maindiv.prop("disable", false);

            if (self.getNumberOfOptions() == 0) {
                VIS.ADialog.info("InfoDocActionNoOptions", "");
                self.log.info("DocAction - No Options");
                ch.close();

                return;
            }
        };


        /// <summary>
        /// Number of options available (to decide to display it)
        /// </summary>
        /// <returns>item count</returns>
        this.getNumberOfOptions = function () {
            return $cmbAction.children('option').length;
        };


        /// <summary>
        /// Should the process be started?
        /// </summary>
        /// <returns>OK pressed</returns>
        this.isStartProcess = function () {
            return this.isOKPressed;
        }

        /// <summary>
        /// Should the process be started in batch?
        /// </summary>
        /// <returns>batch</returns>
        this.isBatch = function () {
            return this.batch;
        }

        function createDesign() {

            var $tr1 = $('<tr>');
            var $tr2 = $('<tr>');
            var $tr3 = $('<tr>');

            var $td11 = $('<td>');
            var $td12 = $('<td>');
            var $DivInputWrap1 = $("<div class='input-group vis-input-wrap'></div>");
            var $DivInputCtrlWrap1 = $("<div class='vis-control-wrap'></div>");
            //if (VIS.Application.isRTL) {
            //    $td11.append('<span style="margin-right:3px">' + VIS.Msg.getMsg('DocAction') + '</span>');
            //}
            //else {
                $td11.append($DivInputWrap1);
                $DivInputWrap1.append($DivInputCtrlWrap1);
            //}
            $DivInputCtrlWrap1.append($cmbAction);
            $DivInputCtrlWrap1.append('<label >' + VIS.Msg.getMsg('DocAction') + '</label>');
            //$tr1.append($td11).append($td12);
            $tr1.append($td11);

            var $td22 = $('<td>');
            $td22.append($message);
            $tr2.append($td22);

            $td31 = $('<td>');
            $td32 = $('<td style="text-align:right">');

            ////Background button
            //$btnbackground.addClass("VIS_Pref_btn-2");
            //$btnbackground.text(VIS.Msg.getMsg("BackgroundProcess"));
            //$btnbackground.css({ "margin-top": "5px", "margin-bottom": "0px" });
            //$btnbackground.hide();
            //$td31.append($btnbackground);


            ////Cancel Button
            //$btncancel.addClass("VIS_Pref_btn-2");
            //$btncancel.text(VIS.Msg.getMsg("Cancel"));
            //$btncancel.css({ "margin-top": "5px", "margin-bottom": "0px", "margin-left": "5px" });
            //$td32.append($btncancel);



            ////OK Button
            //$btnok.addClass("VIS_Pref_btn-2");
            //$btnok.text(VIS.Msg.getMsg("OK"));
            //$btnok.css({ "margin-top": "5px", "margin-bottom": "0px" });
            //$td32.append($btnok);




            $tr3.append($td31).append($td32);

            $table.append($tr1).append($tr2).append($tr3);

            $maindiv.append($table);
            setVisibility(true);
            $maindiv.append(tabmenubusy).append(loadLabel);
            //  $maindiv.prop("disable", true);
            ch = new VIS.ChildDialog();


        };


        function events() {

            //Seleced Item Change
            $cmbAction.on("change", function () {

                var index = getSelectedIndex();
                //	Display descriprion
                if (index != -1 && _descriptions.length > 0) {
                    $message.text(_descriptions[index]);
                }
            });

            //Background button clcik 
            //$btnbackground.on("click", function () {
            //    if ($btnbackground.prop("disabled") == true) {
            //        return;
            //    }
            //    if (save()) {
            //        self.batch = true;
            //        self.isOKPressed = true;
            //        ch.close();
            //    }
            //});

            //Ok button click
            ch.onOkClick = function () {
                //if ($btnok.prop("disabled") == true) {
                //    return;
                //}
                if (save()) {
                    self.isOKPressed = true;

                    if (self.onClose) {
                        self.onClose();
                    }
                    //ch.close();

                }
            };

            //Cancel button click
            ch.onCancelClick = function () {
                //if ($btncancel.prop("disabled") == true) {
                //    return;
                //}
                //ch.close();
                if (self.onClose) {
                    self.onClose();
                }
            };
        };

        this.show = function () {
            ch.setContent($maindiv);
            ch.setTitle(VIS.Msg.getMsg("DocAction"));
            ch.setModal(true);
            ch.show();
            //  ch.hidebuttons();
        };

        ch.onClose = function () { self.dispose(); };

        function setVisibility(tvisible) {
            if (tvisible) {
                tabmenubusy.show(); loadLabel.show();
                $maindiv.find("button").prop("disabled", true);
            }
            else {
                tabmenubusy.hide();
                loadLabel.hide();
                $maindiv.find("button").prop("disabled", false);
            }
        }


        /// <summary>
        /// Get index of selected choice
        /// </summary>
        /// <returns>index or -1</returns>
        function getSelectedIndex() {
            var index = -1;

            if ($cmbAction.val() == null)
                return index;
            //	get Selection
            var sel = $cmbAction.val();

            //	find it in vector
            for (var i = 0; i < _names.length && index == -1; i++)
                if (sel.equals(_names[i])) {
                    index = i;
                    break;
                }
            //
            return index;
        }

        function save() {
            setVisibility(true);
            var selectedindex = getSelectedIndex();
            if (selectedindex == -1) {
                setVisibility(false);
                return false;
            }

            //	Save Selection
            // thi.log.Config("DocAction=" + _values[selectedindex]);
            tabObj.setValue("DocAction", _values[selectedindex]);
            return true;
        };

        this.dispose = function () {
            ch.close();
            ch = null;
            AD_Table_ID = null;
            _values = null;
            _names = null;
            _descriptions = null;
            docStatus = null;
            index = 0;
            defaultV = null;
            options = null;
            // $cmbAction.empty();
            $cmbAction = null;
            $message = null;
            // // $btnbackground = null;
            // $btnok = null;
            // $btncancel = null;
            // $maindiv.empty();
            $maindiv = null;
            self = null;
            if ($table)
                $table.empty();

            $table = null;
        };

    };


    function documentEngine() {
        //Complete = CO
        this.ACTION_COMPLETE = "CO";
        //Wait Complete = WC 
        this.ACTION_WAITCOMPLETE = "WC";
        // Approve = AP 
        this.ACTION_APPROVE = "AP";
        // Reject = RJ 
        this.ACTION_REJECT = "RJ";
        // Post = PO 
        this.ACTION_POST = "PO";
        // Void = VO 
        this.ACTION_VOID = "VO";
        // Close = CL
        this.ACTION_CLOSE = "CL";
        // Reverse - Correct = RC 
        this.ACTION_REVERSE_CORRECT = "RC";
        // Reverse - Accrual = RA 
        this.ACTION_REVERSE_ACCRUAL = "RA";
        // ReActivate = RE 
        this.ACTION_REACTIVATE = "RE";
        // <None> = -- 
        this.ACTION_NONE = "--";
        // Prepare = PR
        this.ACTION_PREPARE = "PR";
        // Unlock = XL 
        this.ACTION_UNLOCK = "XL";
        // Invalidate = IN 
        this.ACTION_INVALIDATE = "IN";
        // ReOpen = OP 
        this.ACTION_REOPEN = "OP";

        // Drafted = DR
        this.STATUS_DRAFTED = "DR";
        // Completed = CO 
        this.STATUS_COMPLETED = "CO";
        // Approved = AP 
        this.STATUS_APPROVED = "AP";
        // Invalid = IN 
        this.STATUS_INVALID = "IN";
        //Not Approved = NA 
        this.STATUS_NOTAPPROVED = "NA";
        //Voided = VO 
        this.STATUS_VOIDED = "VO";
        // Reversed = RE 
        this.STATUS_REVERSED = "RE";
        // Closed = CL 
        this.STATUS_CLOSED = "CL";
        // Unknown = ??
        this.STATUS_UNKNOWN = "??";
        //In Progress = IP 
        this.STATUS_INPROGRESS = "IP";
        // Waiting Payment = WP 
        this.STATUS_WAITINGPAYMENT = "WP";
        //Waiting Confirmation = WC 
        this.STATUS_WAITINGCONFIRMATION = "WC";
    };

    VIS.VDocAction = VDocAction;
    VIS.DocumentEngine = documentEngine;

})(VIS, jQuery);
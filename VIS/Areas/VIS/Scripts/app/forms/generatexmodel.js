
; (function (VIS, $) {
    VIS.AForms = VIS.AForms || {};

    function GenerateXModel() {
        this.frame;
        this.windowNo;
        this.log = VIS.Logging.VLogger.getVLogger("GenerateXModel");

        var $self = this;
        var $root = $("<div style='width: 100%; height: 100%; background-color: white;'>");
        //var $busyDiv = $("<div class='vis-apanel-busy' style='width:98%;height:98%;position:absolute'>");
        var $busyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
        var okBtn = null;
        var btnOkA = null;
        var cancelBtn = null;
        var cmbAllTable = null;
        varchkAllTable = null;
        vartxtpath = null;
        varchkClassType = null;

         
        function initializeComponent() {

            var div = '<div style="width: 400px;margin: 20px auto;vertical-align: middle;">' +

                        '<div style="width: 400px;float: left;">' +
                            '<div style="width: 100px;float: left;   margin-top: 8px;">' +
                            '<span>' + VIS.Msg.getMsg("VISPRO_SelectTable") + '</span>' +
                            '</div>' +
                            '<div style="width: 300px;float: left;">' +
                            '<select style="width: 100%;float: left;" class="VIS_Pref_pass" id="cmbAllTable_@windowNo"></select>' +
                            '</div>' +
                        '</div>' +

                        '<div style="float: left;margin-left: 5px;margin-top: 7px; display: none;">' +
                            '<input id="chkAllTable_@windowNo" type="checkbox" />' +
                            '<span>' + VIS.Msg.getMsg("VISPRO_AllTable") + '</span>' +
                        '</div>' +


                        '<div style="width: 400px;float: left; display: none;">' +
                            '<div style="width: 100px;float: left;">' +
                            '<span>' + VIS.Msg.getMsg("VISPRO_Path") + '</span>' +
                            '</div>' +
                            '<div style="width: 300px;float: left;">' +
                            '<input style="width: 81%;float: left;" class="VIS_Pref_pass" type="text" id="txtpath_@windowNo" />' +
                            '<input type="file" multiple="" webkitdirectory="" style="height: -1px; width:30px ">' +
                            '</div>' +
                        '</div>' +


                        '<div style="width: 400px;float: left; display: none;">' +
                            '<div style="width: 100px;float: left;">' +
                            '<span>' + VIS.Msg.getMsg("VISPRO_EntityType") + '</span>' +
                            '</div>' +
                            '<div style="width: 300px;float: left;">' +
                            '<input id="chkClassType_@windowNo" type="checkbox" />' +
                            '<span>' + VIS.Msg.getMsg("VISPRO_ClassType") + '</span>' +
                            '</div>' +
                        '</div>' +
                        '<div class="VIS-locator-DivStyle" style="text-align: right">' +
                            '<input type="button" id="btnCancel_@windowNo" style="margin-bottom:0px;margin-top:0px; float:right " value="' + VIS.Msg.getMsg("Cancel") + '" class="VIS_Pref_btn-2" />' +
                            '<input type="button" id="btnOk_@windowNo" style="margin-bottom:0px;margin-top:0px; float:right; margin-right:10px" class="VIS_Pref_btn-2" value="' + VIS.Msg.getMsg("VIS_Genrate") + '" />' +
                            '<a href="javascript:void(0)" class="VIS_Pref_pass-btn" style="display: none; margin-bottom: 0px; margin-top: 0px;float: right; margin-right: 10px;" id="btnOkA_@windowNo" >' +
                            VIS.Msg.getMsg("VIS_Download") + '</a>' +
                        '</div>' +
                    '</div>';

            div = div.replaceAll("@windowNo", $self.windowNo);
            $root.html(div);
            $root.append($busyDiv);

            okBtn = $root.find("#btnOk_" + $self.windowNo);
            cancelBtn = $root.find("#btnCancel_" + $self.windowNo);
            btnOkA = $root.find("#btnOkA_" + $self.windowNo);

            cmbAllTable = $root.find("#cmbAllTable_" + $self.windowNo);
            chkAllTable = $root.find("#chkAllTable_" + $self.windowNo);
            txtpath = $root.find("#txtpath_" + $self.windowNo);
            chkClassType = $root.find("#chkClassType_" + $self.windowNo);
        }

        function jbInit() {
            // $self.lblStatusInfo.getControl().text(VIS.Msg.getMsg("InvGenerateSel", false, false));
            getTable();
        }

        function events() {
            //Events
            if (okBtn != null)
                okBtn.on(VIS.Events.onTouchStartOrClick, function () {
                    setBusy(true);
                    generateXclasses();
                    setBusy(false);
                });

            if (btnOkA != null)
                btnOkA.on(VIS.Events.onTouchStartOrClick, function () {
                    btnOkA.css("display", "none");
                    okBtn.css("display", "block");
                });


            if (cancelBtn != null)
                cancelBtn.on(VIS.Events.onTouchStartOrClick, function () {
                    $self.dispose();
                });
        }

        function generateXclasses() {
            var obj = $self;
            var directory = "";
            var chkStatus = false;
            var tableId = cmbAllTable.find('option:selected').val();
            var classType = "";

            $.ajax({
                url: VIS.Application.contextUrl + "Form/GenerateXClasses",
                dataType: "json",
                data: {
                    directory: directory,
                    chkStatus: chkStatus,
                    tableId: tableId,
                    classType: classType
                },
                success: function (data) {
                    if (data.contant != null) {
                        //var fileData = "data:text/csv;base64," + window.btoa(data.contant);
                        var fileData = "data:text/csv;charset=utf-8," + encodeURIComponent(data.contant);
                        //btnOkA.attr("href", fileData).attr("download", data.fileName)
                        btnOkA.attr('download', data.fileName).attr('href', fileData).attr('target', '_blank');
                        //btnOkA.trigger("click");
                        // btnOkA.removeAttr("download");

                        //window.open(URL, name, specs, replace)
                        //window.open("data:text/csv;charset=utf-8," + encodeURIComponent(data.contant), data.fileName);
                        VIS.ADialog.info(data.msg);
                        btnOkA.css("display", "block");
                        okBtn.css("display", "none");
                    }
                    setBusy(false);
                }
            });
        }

        //function getTable() {
        //    try {
        //        var strQuery = "select Name, AD_TABLE_ID,TableName from AD_TABLE order by name";
        //        var dr = VIS.DB.executeReader(strQuery, null, null);
        //        while (dr.read()) {
        //            cmbAllTable.append("<option value='" + dr.getString(1) + "'>" + dr.getString(0) + '</option>');
        //        }
        //        dr.close();
        //    }
        //    catch (e) {
        //    }
        //}


        // Get Table Name
        function getTable() {
            $.ajax({
                url: VIS.Application.contextUrl + "GenerateXModel/GenXModelGetTable",
                type: 'POST',
                async: false,
                success: function (data) {
                    var res = data.result;
                    if (res) {
                        for (var i = 0; i < res.length; i++) {
                            cmbAllTable.append("<option value='" + res[i].AD_Table_ID + "'>" + VIS.Utility.encodeText(res[i].Name) + '</option>');
                        }
                    }
                },
                error: function (e) {
                    $self.log.info(e);
                },
            });
        }


        function getEntity() {
            try {
                var strQuery = "VIS_130";
                var dr = executeReader(strQuery, null, null);
                while (dr.read()) {

                }
                dr.close();
            }
            catch (e) {
            }
        }


        var baseUrl = VIS.Application.contextUrl;
        var dataSetUrl = baseUrl + "JsonData/JDataSet";

        var executeReader = function (sql, param, callback) {
            var async = callback ? true : false;

            var dataIn = { sql: sql, page: 1, pageSize: 0 };
            if (param) {
                dataIn.param = param;
            }
            var dr = null;
            getDataSetJString(dataIn, async, function (jString) {
                dr = new VIS.DB.DataReader().toJson(jString);
                if (callback) {
                    callback(dr);
                }
            });
            return dr;
        };

        function getDataSetJString(data, async, callback) {
            var result = null;
          //  data.sql = VIS.secureEngine.encrypt(data.sql);
            $.ajax({
                url: dataSetUrl,
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: async,
                data: JSON.stringify(data)
            }).done(function (json) {
                result = json;
                if (callback) {
                    callback(json);
                }
                //return result;
            });
            return result;
        };







        function setBusy(isBusy) {
            $busyDiv.css("display", isBusy ? 'block' : 'none');
        };

        this.Initialize = function () {
            initializeComponent();
            jbInit();
            events();
            setBusy(false);
        }

        //Privilized function
        this.getRoot = function () {
            return $root;
        };

        this.disposeComponent = function () {

            if (okBtn)
                okBtn.off(VIS.Events.onTouchStartOrClick);
            if (cancelBtn)
                cancelBtn.off(VIS.Events.onTouchStartOrClick);

            $self = null;
            this.windowNo = null;

            $root = null;
            $busyDiv = null;

            this.getRoot = null;
            this.disposeComponent = null;
        };
    };

    //Must Implement with same parameter
    GenerateXModel.prototype.init = function (windowNo, frame) {
        this.frame = frame;
        this.windowNo = windowNo;

        try {
            this.Initialize();
        }
        catch (ex) {
            //log.Log(Level.SEVERE, "init", ex);
        }

        this.frame.getContentGrid().append(this.getRoot());
    };

    //Must implement dispose
    GenerateXModel.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };

    VIS.AForms.GenerateXModel = GenerateXModel;


})(VIS, jQuery);
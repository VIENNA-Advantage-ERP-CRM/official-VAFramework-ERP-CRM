/********************************************************
 * Module Name    :     Application
 * Purpose        :     Generate Dialog to mark records with module .
 * Author         :     Lakhwinder
 * Date           :     29-Jun-2015
  ******************************************************/
; (function (VIS, $) {
    function MarkModule() {

        this.onClose = null;
        var selfi = this;

        var root = null;
        var bsyDiv = null;
        var contentDiv = null;
        var divMlist = null;
        var btnOk = null;
        var btnCancel = null;
        var divButtons = null;
        var lstModules = null;

        var _recordID = [];
        var _strRecordID = null;
        var _tableID = null;
        var _tableName = null;

        var lstExistingRec = [];
        var lstCtrl = [];
        var table = null;


        var initializeComponent = function () {

            root = $("<div>");
            bsyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
            bsyDiv[0].style.visibility = "visible";
            root.append(bsyDiv);

            contentDiv = $("<div>");
            divMlist = $("<div class='vis-markmodulemainwrap'>");
            contentDiv.append(divMlist);
            divButtons = $('<div>');
            contentDiv.append(divButtons);
            var canceltxt = VIS.Msg.getMsg("Cancel");
            var Oktxt = VIS.Msg.getMsg("Ok");
            btnCancel = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;'>").append(canceltxt);
            btnOk = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;margin-right:5px'>").append(Oktxt);

            divButtons.append(btnCancel);
            divButtons.append(btnOk);

            root.append(contentDiv);
            table = $('<table>');


        };

        initializeComponent();

        var events = function () {
            btnCancel.on("click", function () {
                disposeComponent();
            });
            btnOk.on("click", function () {
                saveData();
            });
        };

        events();
        var loadModules = function () {

            lstModules = [];
            var module = null;
            //var dr = VIS.DB.executeReader("SELECT AD_ModuleInfo_ID, Name FROM AD_Moduleinfo WHERE isActive='Y' ORDER BY Upper(Name) ");
            var dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "MarkModule/LoadModules", null, null);
            //while (dr.read()) {
            //    module = {};
            //    module.AD_ModuleInfo_ID = dr.getInt(0);
            //    module.Name = dr.getString(1);
            //    lstModules.push(module);
            //}
            //dr = null;

            if (dr != null) {
                for (var i in dr) {
                    module = {};
                    module.AD_ModuleInfo_ID = dr[i]["AD_ModuleInfo_ID"];
                    module.Name = dr[i]["Name"];
                    lstModules.push(module);
                }
            }
            if (_recordID.length == 1) {
                //dr = VIS.DB.executeReader("select AD_moduleinfo_id  from ad_exportdata e  where e.record_id=" + _recordID[0] + " and e.ad_table_id=" + _tableID);
                //while (dr.read()) {
                //    lstExistingRec.push(dr.getInt(0));
                //}
                var data = {                    
                    RecordID: _recordID[0],
                    TableID: _tableID,                   
                };
                var dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "MarkModule/GetExportData", data, null);
                if (dr != null) {
                    for (var i in dr) {
                        lstExistingRec.push(dr[i]);
                    }
                }
            }
            //dr = null;
            //var htmlstr = "<Table>";
            var tr = null;
            var td = null;
            var lbl = null;
            var chkbox = null;
            if (lstExistingRec.length == 0) {

                for (var i = 0; i < lstModules.length; i++) {

                    tr = $('<tr>');
                    td = $('<td>');
                    lbl = $('<label class="vis-gc-vpanel-table-label-checkbox" style="display: inline-block; opacity: 1;">');
                    chkbox = $("<input type='checkbox'>");
                    lbl.append(chkbox);
                    lbl.append(lstModules[i].Name);
                    td.append(lbl);
                    tr.append(td);
                    table.append(tr);
                    var ctrlItem = {};
                    ctrlItem.Ctrl = chkbox;
                    ctrlItem.AD_ModuleInfo_ID = lstModules[i].AD_ModuleInfo_ID;
                    lstCtrl.push(ctrlItem);
                    //htmlstr += "<tr><td><label class='vis-gc-vpanel-table-label-checkbox' style='display: inline-block; opacity: 1;'><input type='checkbox'>" + lstModules[i].Name + "</label></td></tr>";
                }

            }
            else {

                //for (var i = 0; i < lstExistingRec.length; i++) {
                for (var j = 0; j < lstModules.length; j++) {

                    tr = $('<tr>');
                    td = $('<td>');
                    lbl = $('<label class="vis-gc-vpanel-table-label-checkbox" style="display: inline-block; opacity: 1;">');
                    if (lstExistingRec.indexOf(lstModules[j].AD_ModuleInfo_ID)>-1) {
                        chkbox = $("<input type='checkbox' checked>");
                        // htmlstr += "<tr><td><label class='vis-gc-vpanel-table-label-checkbox' style='display: inline-block; opacity: 1;'><input type='checkbox' checked>" + lstModules[j].Name + "</label></td></tr>";
                    }
                    else {
                        chkbox = $("<input type='checkbox'>");
                        // htmlstr += "<tr><td><label class='vis-gc-vpanel-table-label-checkbox' style='display: inline-block; opacity: 1;'><input type='checkbox'>" + lstModules[j].Name + "</label></td></tr>";
                    }
                    lbl.append(chkbox);
                    lbl.append(lstModules[j].Name);
                    td.append(lbl);
                    tr.append(td);
                    table.append(tr);
                    var ctrlItem = {};
                    ctrlItem.Ctrl = chkbox;
                    ctrlItem.AD_ModuleInfo_ID = lstModules[j].AD_ModuleInfo_ID;
                    lstCtrl.push(ctrlItem);
                    //}
                }

            }
            //htmlstr += "</Table>";
            //divMlist.append(htmlstr);
            divMlist.append(table);
            bsyDiv[0].style.visibility = "hidden";
        }


        var saveData = function () {
            bsyDiv[0].style.visibility = "visible";
            var lstselectedID = [];
            // var selectedID = '';
            for (var i = 0; i < lstCtrl.length; i++) {
                var chkbox = lstCtrl[i].Ctrl;
                if (chkbox.prop('checked')) {
                    lstselectedID.push(lstCtrl[i].AD_ModuleInfo_ID);
                    // selectedID += lstCtrl[i].AD_ModuleInfo_ID + ',';
                }
            }
           
            var data = {
                moduleId: lstselectedID,
                _recordID: _recordID,
                _tableID: _tableID,
                _strRecordID: _strRecordID
            };
            var res = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "MarkModule/SaveExportData", data);
            if (res.result != "OK") {
                alert(res.result);
            }
            bsyDiv[0].style.visibility = "hidden";
           
            disposeComponent();
          


        };

        this.show = function () {

            root.dialog({
                title: VIS.Msg.getMsg('VIS_MarkModule'),
                width: 400,
                height: 500,
                resizable: false,
                modal: true,
                close: onClosing
            });
            loadModules();
        };

        this.init = function (recordID, TableID, tableName) {
            _recordID = recordID.toString().split(',');
            _strRecordID = recordID;
            _tableID = TableID;
            _tableName = tableName;
        }

        var onClosing = function () {
            disposeComponent();
        };

        var disposeComponent = function () {

            if (selfi.onClose) {
                selfi.onClose();
            }
            contentDiv = null;
            divMlist = null;
            btnCancel.off("click");
            btnOk.off("click");
            btnOk = null;
            btnCancel = null;
            divButtons = null;
            loadModules = null;
            saveData = null;
            initializeComponent = null;
            events = null;
            onClosing = null;
            bsyDiv = null;
            lstModules = null;

            _recordID = null;
            _strRecordID = null;
            _tableID = null;
            _tableName = null;

            lstExistingRec = null;
            lstCtrl = null;
            table = null;
            if (root != null) {
                root.dialog('destroy');
                root.remove();
            }
            root = null;

        };
    }
    VIS.MarkModule = MarkModule;
})(VIS, jQuery);

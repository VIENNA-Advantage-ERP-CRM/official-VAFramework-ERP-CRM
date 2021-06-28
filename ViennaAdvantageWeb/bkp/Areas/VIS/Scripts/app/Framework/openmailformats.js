; (function (VIS, $) {

    VIS.Openmailformat = function (window_ID, isEmail) {
        var sql;
        var ds = null;
        var selectedRow = {};
        var self = this;
        var attchmentName = [];

        var $maingrid;
        var ch;


        this.onClose = null;

        loadData();

        function loadData() {

            //sql = " SELECT NAME        ,"
            //sql += " nvl(SUBJECT,' ')  as SUBJECT          ,"
            //sql += " nvl(MAILTEXT,' ')     as MAILTEXT    ,"
            //sql += " nvl(CREATED,sysdate)         as CREATED  , "
            //sql += " AD_CLIENT_ID      ,"
            //sql += "  AD_ORG_ID         ,"
            //sql += " CREATEDBY         ,"
            //sql += "  nvl(ISACTIVE,'Y')         as ISACTIVE ,"
            //sql += " nvl(ISHTML,'Y')          as ISHTML  ,"
            //sql += "  AD_TEXTTEMPLATE_ID,"
            //sql += "  nvl( UPDATED,sysdate)         as UPDATED  ,"
            //sql += "  UPDATEDBY         ,"
            //sql += "  AD_Window_ID         ,"
            //sql += "  nvl(ISDYNAMICCONTENT,'N') as ISDYNAMICCONTENT"
            //sql += "   FROM AD_TextTemplate"
            //sql += "   WHERE IsActive='Y' and (AD_Window_ID IS NULL ";

            //if (window_ID > 0) {
            //    sql += " OR AD_Window_ID=" + window_ID;
            //}

            //sql += ")";

            //sql = MRole.GetDefault(Envs.GetCtx(), false).AddAccessSQL(sqlexec, "ad_texttemplate", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO) + " order by ad_texttemplate_id";

            //sql = VIS.MRole.addAccessSQL(sql,"AD_TextTemplate", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO);
            //ds = VIS.DB.executeDataSet(sql);

            // Added by Bharat on 12 Dec 2017
            ds = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Email/GetMailFormat", { "Window_ID": window_ID }, null);
        };

        function ok() {
            var index = w2ui['openformatgrid'].getSelection();
            selectedRow = w2ui['openformatgrid'].get(index);

            //var sql = "select name from MailAttachment1 where ad_table_id=(Select AD_Table_ID from AD_Table where tablename='AD_TextTemplate') and record_id=" + selectedRow.ad_texttemplate_ID;
            //ds = VIS.DB.executeDataSet(sql);


            //$.ajax({
            //    type: 'json',
            //    async: false,
            //    data: { textTemplate_ID: selectedRow.ad_texttemplate_ID },
            //    url: VIS.Application.contextUrl + "VACOM/Email/SavedAttachmentForFormat",
            //    success: function (json) {
            //        var result = JSON.parse(json);
            //        for (var i = 0; i < result.length; i++) {
            //            attchmentName.push(result[i]);
            //        }
            //    },
            //});



            self.onClose();
            return true;

        };

        function dispose() {

            ds = null;
            sql = null;
            ch = null;
            if (w2ui['openformatgrid'] != undefined) {
                w2ui['openformatgrid'].destroy();
            }
            $maingrid.remove();

        };

        this.show = function () {
            $maingrid = $('<div class="vis-forms-container" style="height:304px"></div>');

            ch = new VIS.ChildDialog();
            if (VIS.Application.isRTL) {
                ch.setHeight(435);
            }
            else {
                ch.setHeight(430);
            }
            ch.setWidth(850);
            if (isEmail) {
                ch.setTitle(VIS.Msg.getMsg("EmailFormats"));
            }
            else {
                ch.setTitle(VIS.Msg.getMsg("LetterFormat"));
            }
            ch.setModal(true);
            ch.setContent($maingrid);
            ch.show();
            ch.onOkClick = ok;
            ch.onClose = dispose;

            //Columns size of grid is set to 20% each so that no empty space there in grid
            //for (var c = 0; c < ds.tables[0].columns.length; c++) {


            //    if (ds.tables[0].columns[c].name == "name") {
            //        columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.getMsg("Name"), size: '20%', hidden: false });
            //    }
            //    else if (ds.tables[0].columns[c].name == "subject") {
            //        columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.getMsg("Subject"), size: '20%', hidden: false });
            //    }
            //    else if (ds.tables[0].columns[c].name == "mailtext") {
            //        columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.getMsg("Mailtext"), size: '20%', hidden: false });
            //    }
            //    else if (ds.tables[0].columns[c].name == "created") {
            //        columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.getMsg("Created"), size: '20%', hidden: false });
            //    }
            //    else if (ds.tables[0].columns[c].name == "isdynamiccontent") {
            //        columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.getMsg("IsDynamicContent"), size: '20%', hidden: false });
            //    }
            //    else {
            //        columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.getMsg(ds.tables[0].columns[c].name), size: '100px', hidden: true });
            //    }

            //}


            //var rows = [];

            //var colkeys = [];

            //if (ds.tables[0].rows.length > 0) {
            //    colkeys = Object.keys(ds.tables[0].rows[0].cells);
            //}

            //for (var r = 0; r < ds.tables[0].rows.length; r++) {
            //    var singleRow = {};
            //    singleRow['recid'] = r;
            //    for (var c = 0; c < colkeys.length; c++) {
            //        var colna = colkeys[c];
            //        if (colna == "mailtext") {
            //            singleRow[colna] = (ds.tables[0].rows[r].cells[colna]);
            //        }
            //        else {
            //            singleRow[colna] = VIS.Utility.encodeText(ds.tables[0].rows[r].cells[colna]);
            //        }
            //    }
            //    rows.push(singleRow);
            //}

            var columns = [];
            var rows = [];

            if (ds != null) {
                for (var c in ds) {

                    if (ds[c].name == "NAME") {
                        columns.push({ field: ds[c].name, caption: VIS.Msg.getMsg("Name"), size: '20%', hidden: false });
                    }
                    else if (ds[c].name == "SUBJECT") {
                        columns.push({ field: ds[c].name, caption: VIS.Msg.getMsg("Subject"), size: '20%', hidden: false });
                    }
                    else if (ds[c].name == "MAILTEXT") {
                        columns.push({ field: ds[c].name, caption: VIS.Msg.getMsg("Mailtext"), size: '20%', hidden: false });
                    }
                    else if (ds[c].name == "CREATED") {
                        columns.push({ field: ds[c].name, caption: VIS.Msg.getMsg("Created"), size: '20%', hidden: false });
                    }
                    else if (ds[c].name == "ISDYNAMICCONTENT") {
                        columns.push({ field: ds[c].name, caption: VIS.Msg.getMsg("IsDynamicContent"), size: '20%', hidden: false });
                    }
                    else {
                        columns.push({ field: ds[c].name, caption: VIS.Msg.getMsg(ds[c].name), size: '100px', hidden: true });
                    }
                }

                for (var j = 0; j < ds[0].RowCount; j++) {
                    var singleRow = {};
                    singleRow['recid'] = j;
                    for (var r in ds) {                        
                        
                        if (ds[r].name == "MAILTEXT") {
                            singleRow[ds[r].name] = (ds[r].value[j]);
                        }
                        else {
                            singleRow[ds[r].name] = VIS.Utility.encodeText(ds[r].value[j]);
                        }
                    }
                    rows.push(singleRow);
                }
            }

            $maingrid.w2grid({
                name: 'openformatgrid',
                recordHeight: 30
                ,
                show: { selectColumn: true },
                multiSelect: false,
                columns: columns,
                records: rows
            });
            ///  $maingrid.append($maingrid);
        };

        this.getSelectedRow = function () {
            return selectedRow;
        };

        this.getAttachments = function () {
            return attchmentName;
        };


    };
})(VIS, jQuery);
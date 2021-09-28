; (function (VIS, $) {
    function RecordAccessDialog() {
        var _AD_Table_ID = null;
        var _Record_ID = null;
        var root = null;
        var subroot = null;
        var drow1 = null;
        var drow2 = null;
        var divRecord = null;
        var divNav = null;
        var divRole = null;
        var divRoleInputWrp = null;
        var divRoleInputCtrlWrp = null;
        var lblRole = null;
        var cmbRole = null;
        var divAccOp = null;
        var divActive = null;
        var chkActive = null;
        var lblActive = null;
        var divExclude = null;
        var chkExclude = null;
        var lblExclude = null;
        var divReadOnly = null;
        var chkReadOnly = null;
        var lblReadOnly = null;
        var divDepEntry = null;
        var chkDepEntry = null;
        var lblDepEntry = null;
        var divAction = null;
        var btnNew = null;
        var btnDelete = null;
        var spanRec = null;
        var divWrap = null;
        var btnOk = null;
        var btnCancel = null;
        var btnDown = null;
        var btnUp = null;

        var drow3 = null;
        var drow4 = null;

        var divIncNull = null;
        var chkIncNull = null;
        var lblIncNull = null;



        var lblRecNo = null;
        var recordAccessData = null;
        var curIndex = -1;
        var cmdnew = false;
        this.Load = function (AD_Table_ID, Record_ID) {
            _AD_Table_ID = AD_Table_ID;
            _Record_ID = Record_ID;

            //if (VIS.Application.isRTL) {
            //    divRecord = $("<div class='vis-rad-recordData' style='float:right'>");
            //    btnDown = $("<button  class='vis-rad-navBtn' disabled='disabled' style='float:left'>").append($("<i class='fa fa-caret-down'></i>"));
            //    btnUp = $("<button  class='vis-rad-navBtn' disabled='disabled' style='float:left'>").append($("<i class='fa fa-caret-up' ></i>"));
            //    divAction = $("<div class='vis-rad-actionIcons' style='float:right'>");
            //    btnNew = $("<button  class='vis-rad-actionBtn' style='margin-right:0px'>").append($("<i class='vis vis-plus'></i>"));
            //    divWrap = $("<div class='vis-rad-btnWrap' style='float:left'>");
            //    btnCancel = $("<a href = 'javascript:void(0)'  class='vis-rad-buttons VIS_Pref_btn-2' style='margin-left:0px'>").append(VIS.Msg.getMsg('Cancel'));
            //    spanRec = $("<span class='vis-rad-paging' style='float:none' >").append(lblRecNo);
            //    chkActive = $("<input type='checkbox' checked style='float:right' >");
            //    lblActive = $("<label style='float:right'>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'IsActive'));

            //    chkExclude = $("<input type='checkbox' checked style='float:right'>");
            //    lblExclude = $("<label style='float:right'>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'IsExclude'));
            //    chkReadOnly = $("<input type='checkbox' style='float:right'>");
            //    lblReadOnly = $("<label style='float:right'>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'IsReadOnly'));
            //    chkDepEntry = $("<input type='checkbox' style='float:right'>");
            //    lblDepEntry = $("<label style='float:right'>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'IsDependentEntities'));

            //}
            //else {
            divRecord = $("<div class='vis-rad-recordData'>");
            btnDown = $("<button  class='vis-rad-navBtn' disabled='disabled'>").append($("<i class='fa fa-caret-down'></i>"));
            btnUp = $("<button  class='vis-rad-navBtn' disabled='disabled'>").append($("<i class='fa fa-caret-up'></i>"));
            divAction = $("<div class='vis-rad-actionIcons' >");
            btnNew = $("<button  class='vis-rad-actionBtn'>").append($("<i class='vis vis-plus'></i>"));
            divWrap = $("<div class='vis-rad-btnWrap' >");
            btnCancel = $("<a href = 'javascript:void(0)'  class='vis-rad-buttons VIS_Pref_btn-2'>").append(VIS.Msg.getMsg('Cancel'));
            spanRec = $("<span class='vis-rad-paging'>").append(lblRecNo);
            chkActive = $("<input type='checkbox' checked >");
            lblActive = $("<label>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'IsActive'));

            chkExclude = $("<input type='checkbox' checked >");
            lblExclude = $("<label>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'IsExclude'));
            chkReadOnly = $("<input type='checkbox'>");
            lblReadOnly = $("<label>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'IsReadOnly'));
            chkDepEntry = $("<input type='checkbox'>");
            lblDepEntry = $("<label>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'IsDependentEntities'));

            chkIncNull = $("<input type='checkbox'>");
            lblIncNull = $("<label>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'IncludeNull'));


            //}
            root = $("<div class='vis-forms-container'>");
            subroot = $("<div class='vis-forms-container vis-rad-contantWrap'>");
            drow1 = $("<div class='vis-rad-contantTop'>");
            //divRecord = $("<div class='vis-rad-recordData' style='float:" + VIS.Application.isRTL?'right':'left' + "'>");
            divRole = $("<div class='vis-rad-roleCombo'>");
            divRoleInputWrp = $("<div class='input-group vis-input-wrap'>");
            divRoleInputCtrlWrp = $("<div class='vis-control-wrap'>");

            lblRole = $("<label>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'AD_Role_ID'));
            cmbRole = $("<select class='vis-custom-select'>");
            divRole.append(divRoleInputWrp);
            divRoleInputWrp.append(divRoleInputCtrlWrp);
            divRoleInputCtrlWrp.append(cmbRole);
            divRoleInputCtrlWrp.append(lblRole);
            divAccOp = $("<div class='vis-rad-recordOp'>");

            divActive = $("<div class='vis-rad-check'>");
            //chkActive = $("<input type='checkbox' checked >");
            //lblActive = $("<label>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'IsActive'));
            divActive.append(chkActive);
            divActive.append(lblActive);
            divAccOp.append(divActive);

            divExclude = $("<div class='vis-rad-check'>");
            //chkExclude = $("<input type='checkbox' checked >");
            //lblExclude = $("<label>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'IsExclude'));
            divExclude.append(chkExclude);
            divExclude.append(lblExclude);
            divAccOp.append(divExclude);

            divReadOnly = $("<div class='vis-rad-check'>");
            //chkReadOnly = $("<input type='checkbox'>");
            //lblReadOnly = $("<label>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'IsReadOnly'));
            divReadOnly.append(chkReadOnly);
            divReadOnly.append(lblReadOnly);
            divAccOp.append(divReadOnly);

            divDepEntry = $("<div class='vis-rad-check'>");
            //chkDepEntry = $("<input type='checkbox'>");
            //lblDepEntry = $("<label>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'IsDependentEntities'));
            divDepEntry.append(chkDepEntry);
            divDepEntry.append(lblDepEntry);
            divAccOp.append(divDepEntry);

            divIncNull = $("<div class='vis-rad-check'>");
            //chkReadOnly = $("<input type='checkbox'>");
            //lblReadOnly = $("<label>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'IsReadOnly'));
            divIncNull.append(chkIncNull);
            divIncNull.append(lblIncNull);
            divAccOp.append(divIncNull);

            divRecord.append(divRole);
            divRecord.append(divAccOp);
            drow1.append(divRecord);

            divNav = $("<div class='vis-rad-navIcons'>");
            //btnDown = $("<button  class='vis-rad-navBtn' disabled='disabled' style='float:" + VIS.Application.isRTL ? 'left' : 'right' + "'>").append($("<img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/down-arrow.png'>"));
            //btnUp = $("<button  class='vis-rad-navBtn' disabled='disabled' style='float:" + VIS.Application.isRTL ? 'left' : 'right' + "'>").append($("<img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/up-arrow.png'>"));;
            divNav.append(btnDown);
            divNav.append(btnUp);

            drow1.append(divNav);

            drow2 = $("<div class='vis-rad-contantBottom'>");
            //divAction = $("<div class='vis-rad-actionIcons' style='float:" + VIS.Application.isRTL ? 'right' : 'left' + "'>");
            //btnNew = $("<button  class='vis-rad-actionBtn' style='margin-right:" + VIS.Application.isRTL ? '0px' : '5px' + "'>").append($("<img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/add-new.png'>"));
            btnDelete = $("<button  class='vis-rad-actionBtn'>").append($("<i class='vis vis-delete'></i>"));
            divAction.append(btnNew);
            divAction.append(btnDelete);
            drow2.append(divAction);

            lblRecNo = $("<lable>");
            //spanRec = $("<span class='vis-rad-paging'>").append(lblRecNo);
            drow2.append(spanRec);

            //divWrap = $("<div class='vis-rad-btnWrap' style='float:" + VIS.Application.isRTL ? 'left' : 'right' + "'>");
            //btnCancel = $("<a href = 'javascript:void(0)'  class='vis-rad-buttons' style='margin-left:" + VIS.Application.isRTL ? '0px' : '5px' + "'>").append(VIS.Msg.getMsg('Cancel'));
            btnOk = $("<a href = 'javascript:void(0)'  class='vis-rad-buttons VIS_Pref_btn-2'>").append(VIS.Msg.getMsg('Ok'));
            divWrap.append(btnCancel);
            divWrap.append(btnOk);
            drow2.append(divWrap);

            subroot.append(drow1);
            subroot.append(drow2);
            root.append(subroot);


            //  ///////////////////
            //  drow3 = $("<div>");
            //  drow4 = $("<div>");
            //  root.append(drow1);
            //  root.append(drow2);
            //  root.append(drow3);
            //  root.append(drow4);
            //  btnOk = $("<button  class='VIS_Pref_pass-btn VIS_Pref_btn-pass-click'>OK</button>");
            //  btnCancel = $("<button  class='VIS_Pref_pass-btn VIS_Pref_btn-pass-click'>Cancel</button>");
            //  lblRole = $("<lable>").append(VIS.Msg.translate(VIS.Env.getCtx(), 'AD_Role_ID'));
            //  cmbRole = $("<select>");

            // // chkExclude = $("<input type='checkbox' checked >" + VIS.Msg.translate(VIS.Env.getCtx(), 'IsExclude') + "</input>");
            // // chkReadOnly = $("<input type='checkbox' >" + VIS.Msg.translate(VIS.Env.getCtx(), 'IsReadOnly') + "</input>");
            ////  chkDepEntry = $("<input type='checkbox' >" + VIS.Msg.translate(VIS.Env.getCtx(), 'IsDependentEntities') + "</input>");

            //  drow1.append(btnDown);
            //  drow1.append(btnNew);
            //  drow2.append(lblRole);
            //  drow2.append(cmbRole);
            //  drow2.append(chkActive);
            //  drow2.append(chkExclude);
            //  drow2.append(chkReadOnly);
            //  drow2.append(chkDepEntry);
            //  drow2.append(btnDelete);
            //  drow3.append(btnUp);
            //  drow3.append(lblRecNo);
            //  drow4.append(btnCancel);
            //  drow4.append(btnOk);
            bindEvents();
            loadRoles();
            loadRecords();
            root.dialog({
                width: 600,
                height: 232,
                resizable: false,
                title: 'Record',
                modal: true
            });

        };

        var loadRoles = function () {

            //var sqlRole = VIS.MRole.getDefault().addAccessSQL("SELECT AD_Role_ID, Name FROM AD_Role ORDER BY 2", "AD_Role", VIS.MRole.SQL_NOTQUALIFIED,VIS.MRole.SQL_RW);
            //var dr = VIS.DB.executeReader(sqlRole, null, null);
            var dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "RecordAccess/GetRoles", null, null);
            var options = '<option Value="-1"></option>';
            //while (dr.read()) {
            //    options += ('<option value="' + dr.getInt(0) + '">' + dr.getString(1) + '</option>');
            //}
            if (dr != null) {
                for (i in dr)
                    options += ('<option value="' + dr[i].AD_Role_ID + '">' + dr[i].Name + '</option>');
            }
            cmbRole.append(options);
            options = null;
            //sqlRole = null;
            dr = null;

        };
        var loadRecords = function () {
            //var sql = "SELECT AD_ROLE_ID,ISACTIVE,ISDEPENDENTENTITIES,ISEXCLUDE,ISREADONLY FROM AD_Record_Access WHERE AD_Table_ID=" + _AD_Table_ID + " AND Record_ID=" + _Record_ID + " AND AD_Client_ID=" + VIS.Env.getCtx().getAD_Client_ID();
            if (recordAccessData == null) {
                recordAccessData = [];
            }
            //var dr = VIS.DB.executeReader(sql, null, null);

            var dr = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "RecordAccess/GetRecordAccess", { "Table_ID": _AD_Table_ID, "Record_ID": _Record_ID }, null);
            var item = null;
            //while (dr.read()) {
            //    item = {};
            //    item.AD_ROLE_ID = dr.getInt(0);
            //    item.ISACTIVE = dr.getString(1);
            //    item.ISDEPENDENTENTITIES = dr.getString(2);
            //    item.ISEXCLUDE = dr.getString(3);
            //    item.ISREADONLY = dr.getString(4);
            //    recordAccessData.push(item);

            //}

            if (dr != null) {
                for (var i in dr) {
                    item = {};
                    item.AD_ROLE_ID = dr[i].AD_ROLE_ID;
                    item.ISACTIVE = dr[i].ISACTIVE;
                    item.ISDEPENDENTENTITIES = dr[i].ISDEPENDENTENTITIES;
                    item.ISEXCLUDE = dr[i].ISEXCLUDE;
                    item.ISREADONLY = dr[i].ISREADONLY;
                    item.ISINCLUDENULL = dr[i].ISINCLUDENULL;
                    recordAccessData.push(item);
                }
            }
            item = null;
            dr = null;
            //sql = null;
            if (recordAccessData.length > 0) {

                curIndex = 0;
                setLine();
            }
            else {
                cmdnew = true;
            }

        };
        var save = function () {

            var roleID = cmbRole.val();
            if (roleID == -1) {
                return;
            }
            var update = false;
            if (!cmdnew) {
                update = true;
            }
            $.ajax({
                url: VIS.Application.contextUrl + "RecordAccess/SaveAccess/",
                dataType: "json",
                data: {
                    AD_Role_ID: roleID,
                    AD_Table_ID: _AD_Table_ID,
                    Record_ID: _Record_ID,
                    isActive: chkActive.prop('checked'),
                    isExclude: chkExclude.prop('checked'),
                    isReadOnly: chkReadOnly.prop('checked'),
                    isDependentEntities: chkDepEntry.prop('checked'),
                    isIncludeNull: chkIncNull.prop('checked'),
                    isUpdate: update
                },
                success: function (data) {

                    var res = data.result;
                    if (res == true) {
                        onclose();
                    }
                }
            });


        };
        var bindEvents = function () {
            btnDown.on('click', function () {
                curIndex += 1;
                setLine();
            });
            btnUp.on('click', function () {
                if (cmdnew) {
                    cmdnew = false;
                }
                else {
                    curIndex -= 1;
                }
                setLine();
            });
            btnNew.on('click', function () { cmdNew(); });
            btnDelete.on('click', function () {
                cmdDelete();
            });
            btnOk.on('click', function () { save(); });
            btnCancel.on('click', function () { onclose(); });
        };
        var cmdNew = function () {

            cmdnew = true;
            curIndex = (recordAccessData.length - 1);
            setLine();
            cmbRole.val(-1);
            chkActive.attr('checked', true);
            chkExclude.attr('checked', true);
            chkReadOnly.attr('checked', false);
            chkIncNull.attr('checked', false);
            chkDepEntry.attr('checked', false);
            lblRecNo.empty();
            lblRecNo.append("+" + (recordAccessData.length + 1) + "/" + (recordAccessData.length + 1));
            if (recordAccessData.length > 0) {
                btnUp.attr('disabled', false);
                btnUp.css('opacity', '1');
            }
        };

        var setLine = function () {
            cmbRole.val(recordAccessData[curIndex].AD_ROLE_ID);
            chkActive.attr('checked', recordAccessData[curIndex].ISACTIVE == 'Y' ? true : false);
            chkExclude.attr('checked', recordAccessData[curIndex].ISEXCLUDE == 'Y' ? true : false);
            chkReadOnly.attr('checked', recordAccessData[curIndex].ISREADONLY == 'Y' ? true : false);
            chkIncNull.attr('checked', recordAccessData[curIndex].ISINCLUDENULL == 'Y' ? true : false);
            chkDepEntry.attr('checked', recordAccessData[curIndex].ISDEPENDENTENTITIES == 'Y' ? true : false);
            lblRecNo.empty();
            lblRecNo.append((curIndex + 1) + "/" + recordAccessData.length);
            if (recordAccessData.length > (curIndex + 1)) {
                btnDown.attr('disabled', false);
                btnDown.css('opacity', '1');
            }
            else {
                btnDown.attr('disabled', true);
                btnDown.css('opacity', '.5');
            }
            if (curIndex == 0) {
                btnUp.attr('disabled', true);
                btnUp.css('opacity', '.5');
            }
            else {
                btnUp.attr('disabled', false);
                btnUp.css('opacity', '1');
            }
        };
        var cmdDelete = function () {
            var roleID = cmbRole.val();
            if (roleID == -1 || cmdnew) {
                return;
            }
            $.ajax({
                url: VIS.Application.contextUrl + "RecordAccess/DeleteRecord/",
                dataType: "json",
                data: {
                    AD_Role_ID: roleID,
                    AD_Table_ID: _AD_Table_ID,
                    Record_ID: _Record_ID,
                    isActive: chkActive.prop('checked'),
                    isExclude: chkExclude.prop('checked'),
                    isReadOnly: chkReadOnly.prop('checked'),
                    isIncludeNull: chkIncNull.prop('checked'),
                    isDependentEntities: chkDepEntry.prop('checked')
                },
                success: function (data) {

                    var res = data.result;
                    if (res == true) {
                        onclose();
                    }
                }
            });
        };
        var onclose = function () {

            btnDown.off("click");
            btnUp.off("click");
            btnNew.off("click");
            btnDelete.off("click");
            btnOk.off("click");
            btnCancel.off("click");
            loadRoles = null;
            loadRecords = null;
            save = null;
            bindEvents = null;
            cmdNew = null;
            setLine = null;
            cmdDelete = null;

            _AD_Table_ID = null;
            _Record_ID = null;
            drow1 = null;
            drow2 = null;
            drow3 = null;
            drow4 = null;
            btnDown = null;
            btnUp = null;
            btnNew = null;
            btnDelete = null;
            btnOk = null;
            btnCancel = null;
            lblRole = null;
            cmbRole = null;
            chkActive = null;
            chkExclude = null;
            chkReadOnly = null;
            chkIncNull = null;
            chkDepEntry = null;
            lblRecNo = null;
            recordAccessData = null;
            loadRoles = null;
            loadRecords = null;
            save = null;
            root.dialog('destroy');
            root.remove();
            root = null;
        };

    };
    VIS.RecordAccessDialog = RecordAccessDialog;

})(VIS, jQuery);
/********************************************************
 * Module Name    :     Application
 * Purpose        :     Dialog to maintain versions in system
 * Author         :     Lokesh Chauhan
 * Date           :     14-Oct-2019
  ******************************************************/

; (function (VIS, $) {

    function MasterDataVersion(baseTblName, gridFields, Record_ID, whereClause, mtnTblVer, newRecord, callback) {
        this._tblName = baseTblName;
        this.rec_ID = Record_ID;
        this.sqlWhere = whereClause;
        this.gFields = gridFields;
        this._callbackClose = callback;
        this._newRec = newRecord;
        this.gridCols = new Array();
        this.htmlUI = new Array();

        this.deletable = false;
        this.maintTblVer = mtnTblVer;

        this.defaultColsIniPos = [
            Version = VIS.Msg.getMsg("VIS_RecordVersion"),
            ValidFrom = VIS.Msg.translate(VIS.Env.getCtx(), "VersionValidFrom"),
            Approved = VIS.Msg.getMsg("VIS_IsVersionApproved"),
        ];

        this.defaultColEleIniPos = [
            Version = "RECORDVERSION",
            ValidFrom = "VERSIONVALIDFROM",
            Approved = "ISVERSIONAPPROVED"
        ];

        this.defaultCols = [
            VersionLog = VIS.Msg.translate(VIS.Env.getCtx(), "VersionLog"),
            Delete = VIS.Msg.getMsg("Action")
        ];

        this.defaultColElements = [
            VersionLog = "VERSIONLOG",
            Delete = "ACTION"
        ];
        this.exclCols = [
            Tenant = "AD_Client_ID",
            Created = "Created",
            CreatedBy = "CreatedBy",
            Updated = "Updated",
            UpdatedBy = "UpdatedBy",
            Export_ID = "Export_ID"
        ];
    };

    MasterDataVersion.prototype.show = function () {

        var ismvRunning = false;
        var chkImmSave = null;
        var secVerValFrom = null;
        var dtValFrom = null;

        var self = this;

        var paramString = this._tblName + "," + true + "," + this.sqlWhere;

        // get data for current table for existing versions saved in Version table
        var dr = VIS.dataContext.getJSONRecord("Common/GetIDTextData", paramString);

        // check if records can be deleted from Version table
        var del = VIS.dataContext.getJSONRecord("Common/CheckTableDeletable", this._tblName + "_Ver");
        var diaWidth = "40%";
        // if Records are deleteable from Version Table then set width and Deleteable property to true
        // based on which Delete Icon will be displayed in the popup
        if (del == "Y") {
            this.deletable = true;
            // diaWidth = "40%";
        }

        var gpDia = new VIS.ChildDialog();
        var masVerUI = this.getMasterVersionUI(dr);
        gpDia.setContent(masVerUI);

        if (this.gridCols.length > 8)
            diaWidth = "70%";

        gpDia.setWidth(diaWidth);
        // Set title of Master Version Dialog
        gpDia.setTitle(VIS.Msg.getMsg("MasterDataVersioning"));
        gpDia.setModal(true);

        //Disposing Everything on Close
        gpDia.onClose = function () {
            // turn off events and set controls to null on close dialog
            if (chkImmSave) {
                chkImmSave.off("click");
                chkImmSave = null;
            }
            if (dtValFrom)
                dtValFrom = null;
            if (secVerValFrom)
                secVerValFrom = null;
            if (this.deletable)
                masVerUI.find(".vis-mas-ver-btnDel").off("click");

            gpDia = null;
        };

        gpDia.onOkClick = function () {
            // check if any button pressed then return 
            if (ismvRunning)
                return;

            // callback function
            if (self._callbackClose) {
                ismvRunning = true;

                var dtValidFromDate = dtValFrom.val();
                if (!dtValidFromDate || dtValidFromDate == "")
                    dtValidFromDate = new Date();
                // Cases if save immediate is false
                if (!chkImmSave.prop("checked") && (new Date(dtValFrom.val()) > new Date())) {
                    // get rows from Dialog that are already saved
                    var rows = masVerUI.find(".vis-mas-ver-recRow");
                    var recordExist = false;
                    var verRecID = 0;
                    // if any records found then check 
                    // whether there are any records saved already for the
                    // date which user selected in Valid From Field
                    if (rows.length > 0) {
                        for (var i = 0; i < rows.length; i++) {
                            var rowData = $(rows[i]);
                            var valFromDt = rowData.find('.vis-mas-ver-valFromDate');
                            var dateSaved = new Date($(valFromDt).attr('valdt')).getTime();
                            var dateSelected = new Date(new Date(dtValFrom.val()).setHours(0, 0, 0, 0)).getTime();
                            // if date matched then break
                            if (dateSaved == dateSelected) {
                                recordExist = true;
                                verRecID = rowData.attr("recid");
                                break;
                            }
                        }
                    }
                    // if record already exist for the date that user selected 
                    // then return with message, either overwrite or cancel
                    if (recordExist) {
                        VIS.ADialog.confirm("VersionExistOverwrite", true, "", "Confirm", function (result) {
                            // if user want to overwrite then versiont record will be overwritten for that date in version table
                            if (result) {
                                // call callback function
                                self._callbackClose(chkImmSave.prop("checked"), dtValidFromDate, verRecID);
                                gpDia.close();
                                ismvRunning = false;
                            }
                            else
                                ismvRunning = false;
                        });
                        return false;
                    }
                    else {
                        self._callbackClose(chkImmSave.prop("checked"), dtValidFromDate, 0);
                        ismvRunning = false;
                    }
                }
                else {
                    // in case of save immediate call the callback function
                    self._callbackClose(chkImmSave.prop("checked"), dtValidFromDate, 0);
                    ismvRunning = false;
                }
            }
        };

        gpDia.onCancelClick = function () {
            gpDia.close();
        };

        gpDia.show();

        // if records are deleteable, then bind event
        if (this.deletable) {
            masVerUI.find(".vis-mas-ver-btnDel").on("click", function (e) {
                // on delete button click, find row and delete
                var recRow = $(e.target).closest('.vis-mas-ver-recRow');
                if (recRow.length > 0) {
                    var verRecID = recRow.attr('recid');
                    if (verRecID > 0) {
                        var deleting = false;
                        if (deleting)
                            return;
                        // ask for confirmation of record deletion
                        VIS.ADialog.confirm("DeleteConfirm", true, "", "Confirm", function (result) {
                            deleting = true;
                            // if user confirms then delete
                            if (result) {
                                var parameters = self._tblName + "_Ver" + "," + verRecID;
                                var retRes = VIS.dataContext.getJSONRecord("Common/DeleteRecord", parameters);
                                if (retRes && retRes.Success && retRes.Success == 'Y') {
                                    // if record is successfully deleted then remove record from dialog
                                    recRow.remove();
                                    recRow = masVerUI.find('.vis-mas-ver-recRow');
                                    if (recRow.length <= 0) {
                                        masVerUI.find('.vis-mas-ver-gridData').append(self.getNoRecDiv());
                                    }
                                }
                                else {
                                    if (retRes.Success && retRes.Msg != "") {
                                        VIS.ADialog.error(retRes.Msg);
                                    }
                                    else {
                                        VIS.ADialog.error(VIS.Msg.getMsg("ErrorDeletingRecord"));
                                    }
                                }
                                deleting = false;
                            }
                            else
                                deleting = false;
                        });
                    }
                }
            });
        }

        chkImmSave = masVerUI.find(".vis-mas-ver-chkImmSave");
        secVerValFrom = masVerUI.find(".vis-mas-ver-secValFrom");
        dtValFrom = masVerUI.find(".vis-mas-ver-dtVerVal");
        // click event for Save Immediate checkbox
        chkImmSave.on("click", function () {
            secVerValFrom.toggle();
            // if Save Immediate checkbox is false then display Valid From Field else hide in case of true
            if (!chkImmSave.prop("checked")) {
                if (self._newRec) {
                    dtValFrom.val(self.getDateString(true));
                }
                else
                    dtValFrom.val(self.getDateString());
            }
        });

        // change event for Valid From field
        dtValFrom.on("change", function () {
            // selected date from control
            var valFromDate = new Date(dtValFrom.val()).setHours(0, 0, 0, 0);
            // current date from system
            var curDate = new Date().setHours(0, 0, 0, 0);
            // check if selected date is less than or equal to system date then set tomorrow's date in 
            // valid from date control
            if (valFromDate == curDate) {
                // add message here for info to not allow to set previous date
                dtValFrom.val(self.getDateString());
            }

            // check added in case for version in case of new record
            if (self._newRec && valFromDate >= curDate) {
                dtValFrom.val(self.getDateString(true));
            }
        });
    };

    MasterDataVersion.prototype.getMasterVersionUI = function (dr) {
        var mvUI = $('<div class= "vis-mas-ver-outerwrap">'
            + '<div class="vis-mas-ver-text">' + VIS.Msg.getMsg("VersioningColsFoundValidFrom") + '</div>'
            + '<div style="display: flex;">'
            + '<div class="vis-mas-ver-checkwrap">'
            + '<input type="checkbox" class="vis-mas-ver-ImmSav vis-mas-ver-chkImmSave" checked="checked">'
            + '<label>' + VIS.Msg.getMsg("SaveImmediate") + '</label>'
            + '</div>'
            + '<div class="vis-mas-ver-fieldwrap vis-mas-ver-checkwrap vis-mas-ver-secValFrom" style="display: none;">'
            + '<label>' + VIS.Msg.getMsg("VersionValidFrom") + '</label>'
            + '<input type="date" class="vis-mas-ver-ImmSav vis-mas-ver-dtVerVal">'
            + '</div>'
            + '</div>'
            + this.getGrid(dr)
            + '</div>');
        return mvUI;
    };

    MasterDataVersion.prototype.getGrid = function (dr) {
        // create UI for Master Data Version Popup
        this.htmlUI.push('<div class= "vis-mas-ver-gridSection" style="min-height: 300px;">');
        this.htmlUI.push('<table>');
        var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/delete10.png";
        this.htmlUI.push('<thead class="vis-mas-ver-gridData">');
        this.htmlUI.push(this.createHeaders());
        this.htmlUI.push('</thead>');
        if (dr && dr.Table) {
            if (dr.Table.length > 0) {
                this.htmlUI.push('<tbody>');
                for (var r = 0; r < dr.Table.length; r++) {

                    var recRowOld = dr.Table[r];
                    //recRow = Object.keys(recRow).reduce((c, k) => (c[k.toUpperCase()] = recRow[k], c), {});

                    var keys = Object.keys(recRowOld);
                    var recRow = {};
                    for (var j = 0; j < keys.length; j++) {
                        recRow[keys[j].toUpperCase()] = recRowOld[keys[j]];
                    }




                    var bckColor = "";
                    // for alternate row colors
                    if ((r % 2) == 0)
                        bckColor = "style='background-color: #ebf1f3;'";
                    //if (recRow[this._tblName.toUpper() + "_ID"] != this.rec_ID)
                    //    continue;

                    this.htmlUI.push('<tr recid="' + recRow[this._tblName.toUpper() + "_VER_ID"] + '"  ' + bckColor + ' class="vis-mas-ver-recRow">');
                    for (var i = 0; i < this.gridCols.length; i++) {
                        var colName = this.gridCols[i];
                        var gf = $.grep(this.gFields, function (e) { return e.vo.ColumnName.toUpper() == colName; })[0];
                        // by default display text for columns
                        var displayText = "---";
                        // check if there is some text in columnname_TXT column then display that column value
                        if (recRow[colName + "_TXT"]) {
                            this.htmlUI.push('<td>' + VIS.Utility.encodeText(recRow[colName + "_TXT"]) + '</td>');
                        }
                        // display delete button icon for row if RecordsDeleteable is set on Version table in AD_Table
                        else if (colName == "ACTION") {
                            if (this.deletable) {
                                this.htmlUI.push("<td style='text-align: center;'><button class='vis-mas-ver-btnDel'><img src='" + src + "' /></button></td>");
                            }
                        }
                        // check if there is some text in columnname_LOC or columnname_LTR or columnname_ASI or columnname_ACT or columnname_CTR column then display that column value
                        else if (recRow[colName + "_LOC"] || recRow[colName + "_LTR"] || recRow[colName + "_ASI"] || recRow[colName + "_ACT"] || recRow[colName + "_CTR"]) {
                            if (gf) {
                                displayText = gf.getLookup().getDisplay(gf.value);
                            }
                            this.htmlUI.push('<td>' + VIS.Utility.encodeText(displayText) + '</td>');
                        }
                        else if (recRow[colName]) {
                            // if (colName == 'ISVERSIONAPPROVED') {
                            // if columnname is "IsVersionApproved" then display control in center, handled separately
                            if ((colName == 'ISVERSIONAPPROVED') || (gf && gf.vo && gf.vo.displayType && gf.vo.displayType == 20)) {
                                if (recRow[colName] == 'Y')
                                    this.htmlUI.push('<td style="text-align: center;"><input type="checkbox" checked="checked" disabled></td>');
                                else
                                    this.htmlUI.push('<td style="text-align: center;"><input type="checkbox" disabled></td>');
                            }
                            // for date type to display "Valid From"
                            else if (colName == 'VERSIONVALIDFROM') {
                                var verValFrom = Globalize.format(new Date(recRow[colName]), 'd'); // new Date(recRow[colName]).toDateString();
                                this.htmlUI.push('<td class="vis-mas-ver-valFromDate" valdt="' + new Date(recRow[colName]) + '">' + verValFrom + '</td>');
                            }
                            // for date type controls
                            else if (gf && gf.vo && gf.vo.displayType && gf.vo.displayType == 15) {
                                var dateCol = Globalize.format(new Date(recRow[colName]), 'd');
                                this.htmlUI.push('<td dtDate="' + new Date(recRow[colName]) + '">' + dateCol + '</td>');
                            }
                            // for date + time type controls
                            else if (gf && gf.vo && gf.vo.displayType && gf.vo.displayType == 16) {
                                var dateCol = Globalize.format(new Date(recRow[colName] + "Z"), 'f');
                                this.htmlUI.push('<td dtDate="' + new Date(recRow[colName]) + '">' + dateCol + '</td>');
                            }
                            // for time type controls
                            else if (gf && gf.vo && gf.vo.displayType && gf.vo.displayType == 24) {
                                var dateCol = Globalize.format(new Date(recRow[colName] + "Z"), 't');
                                this.htmlUI.push('<td dtDate="' + new Date(recRow[colName]) + '">' + dateCol + '</td>');
                            }
                            else if (colName == 'RECORDVERSION') {
                                this.htmlUI.push('<td style="text-align: center">' + VIS.Utility.encodeText(recRow[colName]) + '</td>');
                            }
                            else
                                this.htmlUI.push('<td>' + VIS.Utility.encodeText(recRow[colName]) + '</td>');
                        }
                        else {
                            this.htmlUI.push('<td> --- </td>');
                        }
                    }
                    this.htmlUI.push('</tr>');
                }
                this.htmlUI.push('</tbody>');
            }
            else
                this.htmlUI.push(this.getNoRecDiv());
        }
        else {
            this.htmlUI.push(this.getNoRecDiv());
        }

        this.htmlUI.push('</table>');
        this.htmlUI.push('</div>');
        return this.htmlUI.join(' ');
    };

    MasterDataVersion.prototype.createHeaders = function () {
        // create headers here for the popup for saved versions
        var hdrUI = new Array();
        hdrUI.push('<tr style="border-bottom: 2px #b5b3b3 solid;">');
        if (this.gFields && this.gFields.length > 0) {

            this.createVerColsHdrs(hdrUI, this.defaultColsIniPos, this.defaultColEleIniPos);

            for (var i = 0; i < this.gFields.length; i++) {
                var field = this.gFields[i];
                if ((field.vo.IsMaintainVersions || this.maintTblVer) && (this.exclCols.indexOf(field.vo.ColumnName) < 0)) {
                    this.gridCols.push(field.vo.ColumnName.toUpper());
                    if (field.vo.displayType == 20)
                        hdrUI.push('<th style="text-align: center;">' + VIS.Utility.encodeText(field.vo.Header) + '</th>');
                    else
                        hdrUI.push('<th>' + VIS.Utility.encodeText(field.vo.Header) + '</th>');
                }
            }

            this.createVerColsHdrs(hdrUI, this.defaultCols, this.defaultColElements);

            //for (var v = 0; v < this.defaultCols.length; v++) {
            //    this.gridCols.push(this.defaultColElements[v]);

            //    // column header for Action (delete), based on setting on AD_Table for Version table
            //    if (this.defaultColElements[v] == "ACTION") {
            //        if (this.deletable) {
            //            hdrUI.push('<th style="text-align: center;">' + VIS.Utility.encodeText(this.defaultCols[v]) + '</th>');
            //        }
            //    }
            //    else if (this.defaultColElements[v] == "ISVERSIONAPPROVED") {
            //        hdrUI.push('<th style="text-align: center;">' + VIS.Utility.encodeText(this.defaultCols[v]) + '</th>');
            //    }
            //    else
            //        hdrUI.push('<th>' + VIS.Utility.encodeText(this.defaultCols[v]) + '</th>');
            //}
        }
        hdrUI.push('</tr>');
        return hdrUI.join(' ');
    };

    MasterDataVersion.prototype.createVerColsHdrs = function (hUI, defCols, defColEle) {
        for (var v = 0; v < defCols.length; v++) {
            this.gridCols.push(defColEle[v]);

            // column header for Action (delete), based on setting on AD_Table for Version table
            if (defColEle[v] == "ACTION") {
                if (this.deletable) {
                    hUI.push('<th style="text-align: center;">' + VIS.Utility.encodeText(defCols[v]) + '</th>');
                }
            }
            else if ((defColEle[v] == "ISVERSIONAPPROVED") || (defColEle[v] == "RECORDVERSION")) {
                hUI.push('<th style="text-align: center;">' + VIS.Utility.encodeText(defCols[v]) + '</th>');
            }
            else
                hUI.push('<th>' + VIS.Utility.encodeText(defCols[v]) + '</th>');
        }
    };

    MasterDataVersion.prototype.getDateString = function (previous) {
        var dateVal = new Date();
        if (previous)
            dateVal.setDate(dateVal.getDate() - 1);
        else
            dateVal.setDate(dateVal.getDate() + 1);
        var year = dateVal.getFullYear();
        var month = dateVal.getMonth() + 1;
        if (month < 10)
            month = "0" + month;
        var date = dateVal.getDate();
        if (date < 10)
            date = "0" + date;
        return year + "-" + (month) + "-" + (date);
    };

    MasterDataVersion.prototype.getNoRecDiv = function () {
        return '<div class="vis-mas-ver-norecord">' + VIS.Utility.encodeText(VIS.Msg.getMsg("NoRecords")) + '</div>';
    };

    VIS.MasterDataVersion = MasterDataVersion;

})(VIS, jQuery);
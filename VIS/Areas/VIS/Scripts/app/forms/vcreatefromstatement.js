
; (function (VIS, $) {

    //form declaretion
    function VCreateFromStatement(tab) {

        var baseObj = this.$super;
        baseObj.constructor(tab);
        var localObj = this;
        var windowNo = tab.getWindowNo();
        dynInit();
        baseObj.jbInit();
        baseObj.initOK = true;

        baseObj.middelDiv.css("height", "61%");


        function dynInit() {
            if (baseObj.mTab.getValue("C_BankStatement_ID") == null) {
                VIS.ADialog.error(VIS.Msg.getMsg("SaveErrorRowNotFound", true));
                return false;
            }

            baseObj.title = VIS.Msg.getElement(VIS.Env.getCtx(), "C_BankStatement_ID", false) + " .. " + VIS.Msg.getMsg("CreateFrom");
            baseObj.relatedToOrg = null;
            if (baseObj.lblOrder != null)
                baseObj.lblOrder.getControl().css('display', 'none');
            if (baseObj.cmbOrder != null)
                baseObj.cmbOrder.getControl().css('display', 'none');
            if (baseObj.lblShipment != null)
                baseObj.lblShipment.getControl().css('display', 'none');
            if (baseObj.cmbShipment != null)
                baseObj.cmbShipment.getControl().css('display', 'none');
            if (baseObj.lblInvoice != null)
                baseObj.lblInvoice.getControl().css('display', 'none');
            if (baseObj.cmbInvoice != null)
                baseObj.cmbInvoice.getControl().css('display', 'none');
            var AD_Column_ID = 4917;
            var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, AD_Column_ID, VIS.DisplayType.TableDir);

            // JID_0331: System is allowing to change bank in create lines from in bank account window.
            baseObj.cmbBankAccount = new VIS.Controls.VComboBox("C_BankAccount_ID", true, true, true, lookup, 150, VIS.DisplayType.TableDir, 0);
            baseObj.Date = new VIS.Controls.VDate("Date", false, false, true, VIS.DisplayType.Date, "Date");
            baseObj.DocumentNo = new VIS.Controls.VTextBox("DocumentNo", false, false, true, 50,    100, null, null, false);
            baseObj.Amount = new VIS.Controls.VAmountTextBox("Amount", false, false, true, 50, 100, VIS.DisplayType.Amount, VIS.Msg.getMsg("Amount"));
            //if (window.VA034) {
            baseObj.DepositSlip = new VIS.Controls.VTextBox("DepositSlip", false, false, true, 50, 100, null, null, false);
            //}
            baseObj.AuthCode = new VIS.Controls.VTextBox("AuthCode", false, false, true, 50, 100, null, null, false);
            baseObj.CheckNo = new VIS.Controls.VTextBox("CheckNo", false, false, true, 50, 100, null, null, false);
            baseObj.cmbBankAccount.getControl().prop('selectedIndex', 0);

            //  Set Default
            var C_BankAccount_ID = VIS.Env.getCtx().getContextAsInt(windowNo, "C_BankAccount_ID");
            baseObj.cmbBankAccount.setValue(C_BankAccount_ID);
            localObj.initBPartner(C_BankAccount_ID);
            localObj.loadBankAccounts(C_BankAccount_ID, 0, "", "", "", "", "", "0", 1);
            if (baseObj.btnRefresh)
                baseObj.btnRefresh.css("display", "black");
            return true;
        }

        this.disposeComponent = function () {

            this.disposeComponent = null;
        };
    };

    VIS.Utility.inheritPrototype(VCreateFromStatement, VIS.VCreateFrom);//Inherit from VCreateFrom

    //Added by Bharat on 05/May/2017
    VCreateFromStatement.prototype.initBPartner = function (C_BankAccount_ID) {
        //  load BPartner
        var baseObj = this.$super;
        var AD_Column_ID = 5398;        //  C_Payment.C_BPartner_ID
        var where = " C_BPartner.C_BPartner_ID IN (SELECT  p.C_BPartner_ID as C_BPartner_ID"
            + " FROM C_Payment p WHERE p.Processed='Y' AND p.IsReconciled='N'"
            + " AND p.DocStatus IN ('CO','CL','RE','VO') AND p.PayAmt<>0"
            + " AND p.C_BankAccount_ID = " + C_BankAccount_ID                           	//  #2
            + " AND NOT EXISTS (SELECT * FROM C_BankStatementLine l "
                + "WHERE p.C_Payment_ID=l.C_Payment_ID AND l.StmtAmt <> 0) )";
        var lookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), this.windowNo, AD_Column_ID, VIS.DisplayType.Search, "C_BPartner_ID", 0, false, where);
        baseObj.vBPartner = new VIS.Controls.VTextBoxButton("C_BPartner_ID", true, false, true, VIS.DisplayType.Search, lookup);
    }

    VCreateFromStatement.prototype.getTableFieldVOs = function () {
        var baseObj = this.$super;
        baseObj.arrListColumns = [];
        baseObj.arrListColumns.push({
            field: "Date", caption: VIS.Msg.getMsg("DateAcct"), sortable: true, size: '100px', hidden: false, editable: { type: 'date' },
            render: function (record, index, col_index) {
                var val = record["Date"];
                return new Date(val).toLocaleDateString();
            }
        });
        baseObj.arrListColumns.push({ field: "C_Payment_ID", caption: VIS.Msg.getMsg("Payment"), sortable: true, size: '100px', hidden: false });
        baseObj.arrListColumns.push({ field: "C_Currency_ID", caption: VIS.Msg.getMsg("Currency"), sortable: true, size: '80px', hidden: false });

        ////Sukhwinder  on 25-08-2016 for Module(VA034) specific
        var countVA034 = VIS.Utility.Util.getValueOfInt(executeScalar("VIS_145"));
        if (countVA034 > 0) {
            baseObj.arrListColumns.push({ field: "VA034_DepositSlipNo", caption: VIS.Msg.getMsg("DepositSlipNo"), sortable: true, size: '100px', hidden: false });
        }


        baseObj.arrListColumns.push({ field: "AuthCode", caption: VIS.Msg.getMsg("AuthCode"), sortable: true, size: '100px', hidden: false });
        baseObj.arrListColumns.push({ field: "CheckNo", caption: VIS.Msg.getMsg("CheckNo"), sortable: true, size: '80px', hidden: false });
        //
        baseObj.arrListColumns.push({
            field: "Amount", caption: VIS.Msg.getMsg("Amount"), sortable: true, size: '80px', hidden: false,
            render: function (record, index, col_index) {
                var val = VIS.Utility.Util.getValueOfDecimal(record["Amount"]);
                return (val).toLocaleString();
            }
        });
        baseObj.arrListColumns.push({
            field: "ConvertedAmount", caption: VIS.Msg.getMsg("ConvertedAmount"), sortable: true, size: '80px', hidden: false,
            render: function (record, index, col_index) {
                var val = VIS.Utility.Util.getValueOfDecimal(record["ConvertedAmount"]);
                return (val).toLocaleString();
            }
        });
        baseObj.arrListColumns.push({ field: "Description", caption: VIS.Msg.getMsg("Description"), sortable: true, size: '250px', hidden: false });
        baseObj.arrListColumns.push({ field: "C_BPartner_ID", caption: VIS.Msg.getMsg("BusinessPartner"), sortable: true, size: '150px', hidden: false });
        baseObj.arrListColumns.push({ field: "Type", caption: VIS.Msg.getMsg("Type"), sortable: true, size: '80px', hidden: true });
        baseObj.arrListColumns.push({ field: "C_Payment_ID_K", caption: VIS.Msg.getMsg("Payment"), sortable: true, size: '150px', hidden: true });
        baseObj.arrListColumns.push({ field: "C_Currency_ID_K", caption: VIS.Msg.getMsg("Currency"), sortable: true, size: '150px', hidden: true });
    }

    // Added by Bharat for New search Filters and Apply Paging

    VCreateFromStatement.prototype.loadBankAccounts = function (C_BankAccount_ID, C_BPartner_ID, trxDate, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, pNo) {
        /**
         *  Selected        - 0
         *  Date            - 1
         *  C_Payment_ID    - 2
         *  C_Currenncy     - 3
         *  Amt             - 4
         */
        var baseObj = this.$super;
        //  Get StatementDate
        var ts = baseObj.mTab.getValue("StatementDate");
        var data = this.getBankAccountsData(VIS.Env.getCtx(), C_BankAccount_ID, C_BPartner_ID, trxDate, ts, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, pNo);
        //  Header Info
        this.getTableFieldVOs();
        //setTimeout(function () {
        //    baseObj.loadGrid(data);
        //}, 10);

    }



    // get bank account details
    VCreateFromStatement.prototype.getBankAccountsData = function (ctx, C_BankAccount_ID, C_BPartner_ID, trxDate, ts, DocumentNo, DepositSlip, AuthCode, CheckNo, Amount, pNo) {
        var data = [];
        var self = this;

        if (self.$super.dGrid != null) {
            var selection = self.$super.dGrid.getSelection();
            for (item in selection) {
                var obj = $.grep(self.$super.multiValues, function (n, i) {
                    return n.C_Payment_ID_K == self.$super.dGrid.get(selection[item])["C_Payment_ID_K"]
                });
                if (obj.length > 0) {

                }
                else {
                    self.$super.multiValues.push(self.$super.dGrid.get(selection[item]));
                }
            }
        }

        if (ts == null) {
            ts = DateTime.Today.Date;
        }
        var date = VIS.DB.to_date(ts, true);

        var trxDates = "";
        var trxDatesUnion = "";
        var cBPartnerID = "0";
        var DocumentNum = "";
        var DocumentNoUnion = "";

        if (C_BPartner_ID > 0) {
            cBPartnerID = " AND p.C_BPartner_ID = " + C_BPartner_ID;
        }
        if (DocumentNo != "") {
            DocumentNum = " AND p.DocumentNo LIKE '" + DocumentNo + "'";
        }
        if (trxDate != "" && trxDate != null) {
            var dateTrx = VIS.DB.to_date(trxDate, true);
            trxDates = " AND p.DateAcct = " + dateTrx;
        }
        if (DocumentNo != "") {
            DocumentNoUnion = " AND cs.DocumentNo LIKE '" + DocumentNo + "'";
        }
        if (trxDate != "" && trxDate != null) {
            var dateAcct = VIS.DB.to_date(trxDate, true);
            trxDatesUnion = " AND cs.DateAcct = " + dateAcct;
        }

        if (!pNo) {
            pNo = 1;
        }

        $.ajax({
            url: VIS.Application.contextUrl + "VCreateFrom/GetBankAccountsData",
            dataType: "json",
            type: "POST",
            data: {
                pageNo: pNo,
                dates: date,
                trxDatess: trxDates,
                trxDatesUnions: trxDatesUnion,
                cBPartnerIDs: cBPartnerID,
                DocumentNos: DocumentNum,
                DocumentNoUnions: DocumentNoUnion,
                DepositSlips: DepositSlip,
                AuthCodes: AuthCode,
                CheckNos: CheckNo,
                Amounts: Amount,
                cBankAccountId: C_BankAccount_ID
            },
            error: function () {
                alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                self.$super.setBusy(false);
                return;
            },
            success: function (dyndata) {

                var res = JSON.parse(dyndata);
                if (res.Error) {
                    VIS.ADialog.error(res.Error);
                    self.$super.setBusy(false);
                    return;
                }
                self.$super.resetPageCtrls(res.pSetting);
                self.$super.loadGrid(res.data);
                self.$super.setBusy(false);
            }
        });
        return data;
    }







    //VCreateFromStatement.prototype.getBankAccountsData = function (ctx, C_BankAccount_ID, C_BPartner_ID, trxDate, ts, DocumentNo, DepositSlip, AuthCode, pNo) {
    //    var data = [];
    //    var self = this;

    //    if (self.$super.dGrid != null) {
    //        var selection = self.$super.dGrid.getSelection();
    //        for (item in selection) {
    //            var obj = $.grep(self.$super.multiValues, function (n, i) {
    //                return n.C_Payment_ID_K == self.$super.dGrid.get(selection[item])["C_Payment_ID_K"]
    //            });
    //            if (obj.length > 0) {

    //            }
    //            else {
    //                self.$super.multiValues.push(self.$super.dGrid.get(selection[item]));
    //            }
    //        }
    //    }

    //    if (ts == null) {
    //        ts = DateTime.Today.Date;
    //    }
    //    var countVA034 = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar("SELECT Count(AD_ModuleInfo_ID) FROM AD_ModuleInfo WHERE PREFIX='VA034_' AND IsActive = 'Y'"));
    //    var date = VIS.DB.to_date(ts, true);
    //    var sql = "SELECT  p.DateTrx,p.C_Payment_ID, p.DocumentNo, ba.C_Currency_ID,c.ISO_Code,p.PayAmt,"
    //        + "currencyConvert(p.PayAmt,p.C_Currency_ID,ba.C_Currency_ID," + date + ",null,p.AD_Client_ID,p.AD_Org_ID),"   //  #1
    //        + " bp.Name,'P' AS Type ";
    //    if (countVA034 > 0)
    //        sql += ", p.va034_depositslipno ";

    //    sql += " , p.TrxNo, p.CheckNo  FROM C_BankAccount ba"
    //        + " INNER JOIN C_Payment_v p ON (p.C_BankAccount_ID=ba.C_BankAccount_ID)"
    //        + " INNER JOIN C_Currency c ON (p.C_Currency_ID=c.C_Currency_ID)"
    //        + " LEFT OUTER JOIN C_BPartner bp ON (p.C_BPartner_ID=bp.C_BPartner_ID) "
    //        + "WHERE p.Processed='Y' AND p.IsReconciled='N'"
    //        + " AND p.DocStatus IN ('CO','CL','RE','VO') AND p.PayAmt<>0"
    //        + " AND p.C_BankAccount_ID = " + C_BankAccount_ID;                           	//  #2
    //    if (C_BPartner_ID > 0) {
    //        sql += " AND p.C_BPartner_ID = " + C_BPartner_ID;
    //    }
    //    if (DocumentNo != "") {
    //        sql += " AND p.DocumentNo LIKE '" + DocumentNo + "'";
    //    }
    //    if (DepositSlip != "") {
    //        sql += " AND p.va034_depositslipno LIKE '" + DepositSlip + "'";
    //    }
    //    if (AuthCode != "") {
    //        sql += " AND p.TrxNo LIKE '" + AuthCode + "'";
    //    }
    //    if (trxDate != "" && trxDate != null) {
    //        var dateTrx = VIS.DB.to_date(trxDate, true);
    //        sql += " AND p.DateTrx = " + dateTrx;
    //    }
    //    sql += " AND NOT EXISTS (SELECT * FROM C_BankStatementLine l "        //	Voided Bank Statements have 0 StmtAmt
    //        + "WHERE p.C_Payment_ID=l.C_Payment_ID AND l.StmtAmt <> 0)";
    //    var countVA012 = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar("SELECT Count(AD_ModuleInfo_ID) FROM AD_ModuleInfo WHERE PREFIX='VA012_' AND IsActive = 'Y'"));
    //    if (countVA012 > 0) {

    //        sql += " UNION SELECT cs.DateAcct AS DateTrx,cl.C_CashLine_ID AS C_Payment_ID,  cs.DocumentNo, ba.C_Currency_ID,c.ISO_Code,cl.Amount AS PayAmt,"
    //        + "currencyConvert(cl.Amount,cl.C_Currency_ID,ba.C_Currency_ID," + date + ",null,cs.AD_Client_ID,cs.AD_Org_ID),"   //  #1
    //        + " Null AS Name,'C' AS Type"

    //        if (countVA034 > 0)
    //            sql += ", NULL as va034_depositslipno ";

    //        sql += " , null as TrxNo, null AS CheckNo FROM C_BankAccount ba"
    //                + " INNER JOIN C_CashLine cl ON (cl.C_BankAccount_ID=ba.C_BankAccount_ID)"
    //                + " INNER JOIN C_Cash cs ON (cl.C_Cash_ID=cs.C_Cash_ID)"
    //                + " INNER JOIN C_Charge chrg ON chrg.C_Charge_ID=cl.C_Charge_ID"
    //                + " INNER JOIN C_Currency c ON (cl.C_Currency_ID=c.C_Currency_ID)"
    //                + " WHERE cs.Processed='Y' AND cl.VA012_IsReconciled='N'"
    //                + " AND cl.CashType ='C' AND chrg.DTD001_ChargeType ='CON'"
    //                + " AND cs.DocStatus IN ('CO','CL','RE','VO') AND cl.Amount<>0"
    //                + " AND cl.C_BankAccount_ID = " + C_BankAccount_ID;                             	//  #2            
    //        if (DocumentNo != "") {
    //            sql += " AND cs.DocumentNo LIKE '" + DocumentNo + "'";
    //        }
    //        if (trxDate != "" && trxDate != null) {
    //            var dateAcct = VIS.DB.to_date(trxDate, true);
    //            sql += " AND cs.DateAcct = " + dateAcct;
    //        }
    //        sql += " AND NOT EXISTS (SELECT * FROM C_BankStatementLine l "            //	Voided Bank Statements have 0 StmtAmt
    //                + "WHERE cl.C_CashLine_ID=l.C_CashLine_ID AND l.StmtAmt <> 0)";
    //    }

    //    if (!pNo) {
    //        pNo = 1;
    //    }

    //    $.ajax({
    //        url: VIS.Application.contextUrl + "Common/GetAccountData",
    //        dataType: "json",
    //        type: "POST",
    //        data: {
    //            sql: sql,
    //            pageNo: pNo
    //        },
    //        error: function () {
    //            alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
    //            self.$super.setBusy(false);
    //            return;
    //        },
    //        success: function (dyndata) {

    //            var res = JSON.parse(dyndata);
    //            if (res.Error) {
    //                VIS.ADialog.error(res.Error);
    //                self.$super.setBusy(false);
    //                return;
    //            }
    //            self.$super.resetPageCtrls(res.pSetting);
    //            self.$super.loadGrid(res.data);
    //            self.$super.setBusy(false);
    //        }
    //    });
    //    //try {
    //    //    if (ts == null) {
    //    //        ts = DateTime.Today.Date;
    //    //    }

    //    //    var dr = null;
    //    //    var param = [];

    //    //    param[0] = new VIS.DB.SqlParam("@t", ts);
    //    //    param[0].setIsDate(true);
    //    //    param[1] = new VIS.DB.SqlParam("@C_BankAccount_ID", C_BankAccount_ID);

    //    //    dr = VIS.DB.executeReader(sql, param, null);
    //    //    var count = 1;
    //    //    while (dr.read()) {
    //    //        var line = {};
    //    //        line['Date'] = dr.getString(0);             //  1-DateTrx
    //    //        line['C_Payment_ID'] = dr.getString(2);     //  2-C_Payment_ID
    //    //        line['C_Currency_ID'] = dr.getString(4);    //  3-Currency
    //    //        line['Amount'] = dr.getDecimal(5);          //  4-PayAmt
    //    //        line['ConvertedAmount'] = dr.getDecimal(6); //  5-Conv Amt
    //    //        line['C_BPartner_ID'] = dr.getString(7);    //  6-BParner
    //    //        line['Type'] = dr.getString(8);
    //    //        line['C_Payment_ID_K'] = dr.getString(1);   //  2-C_Payment_ID -Key
    //    //        line['C_Currency_ID_K'] = dr.getInt(3);     //  3-Currency -Key

    //    //        line['recid'] = count;
    //    //        count++;
    //    //        data.push(line);
    //    //    }
    //    //}
    //    //catch (e) {

    //    //}
    //    return data;
    //}

    // End done by Bharat

    VCreateFromStatement.prototype.loadBankAccount = function (C_BankAccount_ID) {
        /**
         *  Selected        - 0
         *  Date            - 1
         *  C_Payment_ID    - 2
         *  C_Currenncy     - 3
         *  Amt             - 4
         */
        var baseObj = this.$super;
        //  Get StatementDate
        var ts = baseObj.mTab.getValue("StatementDate");
        var data = this.getBankAccountData(VIS.Env.getCtx(), C_BankAccount_ID, ts);
        //  Header Info
        this.getTableFieldVOs();
        setTimeout(function () {
            baseObj.loadGrid(data);
        }, 10);

    }

    VCreateFromStatement.prototype.getBankAccountData = function (ctx, C_BankAccount_ID, ts) {
        var data = [];

        //var sql = "SELECT p.DateTrx,p.C_Payment_ID,p.DocumentNo, ba.C_Currency_ID,c.ISO_Code,p.PayAmt,"
        //    + "currencyConvert(p.PayAmt,p.C_Currency_ID,ba.C_Currency_ID,@t,null,p.AD_Client_ID,p.AD_Org_ID),"   //  #1
        //    + " bp.Name,'P' AS Type "
        //    + "FROM C_BankAccount ba"
        //    + " INNER JOIN C_Payment_v p ON (p.C_BankAccount_ID=ba.C_BankAccount_ID)"
        //    + " INNER JOIN C_Currency c ON (p.C_Currency_ID=c.C_Currency_ID)"
        //    + " LEFT OUTER JOIN C_BPartner bp ON (p.C_BPartner_ID=bp.C_BPartner_ID) "
        //    + "WHERE p.Processed='Y' AND p.IsReconciled='N'"
        //    + " AND p.DocStatus IN ('CO','CL','RE','VO') AND p.PayAmt<>0"
        //    + " AND p.C_BankAccount_ID=@C_BankAccount_ID"                              	//  #2
        //    + " AND NOT EXISTS (SELECT * FROM C_BankStatementLine l "
        //    //	Voided Bank Statements have 0 StmtAmt
        //        + "WHERE p.C_Payment_ID=l.C_Payment_ID AND l.StmtAmt <> 0)";
        //var countVA012 = VIS.Utility.Util.getValueOfInt(executeScalar("SELECT Count(AD_ModuleInfo_ID) FROM AD_ModuleInfo WHERE PREFIX='VA012_' AND IsActive = 'Y'"));
        //if (countVA012 > 0) {

        //    sql += " UNION SELECT cs.DateAcct AS DateTrx,cl.C_CashLine_ID AS C_Payment_ID,cs.DocumentNo, ba.C_Currency_ID,c.ISO_Code,cl.Amount AS PayAmt,"
        //    + "currencyConvert(cl.Amount,cl.C_Currency_ID,ba.C_Currency_ID,@t,null,cs.AD_Client_ID,cs.AD_Org_ID),"   //  #1
        //    + " Null AS Name,'C' AS Type FROM C_BankAccount ba"
        //    + " INNER JOIN C_CashLine cl ON (cl.C_BankAccount_ID=ba.C_BankAccount_ID)"
        //    + " INNER JOIN C_Cash cs ON (cl.C_Cash_ID=cs.C_Cash_ID)"
        //    + " INNER JOIN C_Charge chrg ON chrg.C_Charge_ID=cl.C_Charge_ID"
        //    + " INNER JOIN C_Currency c ON (cl.C_Currency_ID=c.C_Currency_ID)"
        //    + " WHERE cs.Processed='Y' AND cl.VA012_IsReconciled='N'"
        //    + " AND cl.CashType ='C' AND chrg.DTD001_ChargeType ='CON'"
        //    + " AND cs.DocStatus IN ('CO','CL','RE','VO') AND cl.Amount<>0"
        //    + " AND cl.C_BankAccount_ID=@C_BankAccount_ID"                              	//  #2
        //    + " AND NOT EXISTS (SELECT * FROM C_BankStatementLine l "
        //    //	Voided Bank Statements have 0 StmtAmt
        //        + "WHERE cl.C_CashLine_ID=l.C_CashLine_ID AND l.StmtAmt <> 0)";
        //}

        try {
            if (ts == null) {
                ts = DateTime.Today.Date;
            }

            var dr = null;
            //var param = [];

            //param[0] = new VIS.DB.SqlParam("@t", ts);
            //param[0].setIsDate(true);
            //param[1] = new VIS.DB.SqlParam("@C_BankAccount_ID", C_BankAccount_ID);

            //dr = executeReader(sql, param, null);

            $.ajax({
                url: VIS.Application.contextUrl + "Form/GetBankAccontData",
                async: false,
                data: {
                    C_BankAccount_ID: C_BankAccount_ID,
                    ts: ts
                }
            }).done(function (json) {
                dr = new VIS.DB.DataReader().toJson(json);
            });

            var count = 1;
            while (dr.read()) {
                var line = {};
                line['Date'] = dr.getString(0);             //  1-DateTrx
                line['C_Payment_ID'] = dr.getString(2);     //  2-C_Payment_ID
                line['C_Currency_ID'] = dr.getString(4);    //  3-Currency
                line['Amount'] = dr.getDecimal(5);          //  4-PayAmt
                line['ConvertedAmount'] = dr.getDecimal(6); //  5-Conv Amt
                line['C_BPartner_ID'] = dr.getString(7);    //  6-BParner
                line['Type'] = dr.getString(8);
                line['C_Payment_ID_K'] = dr.getString(1);   //  2-C_Payment_ID -Key
                line['C_Currency_ID_K'] = dr.getInt(3);     //  3-Currency -Key

                line['recid'] = count;
                count++;
                data.push(line);
            }
        }
        catch (e) {

        }
        return data;
    }

    var baseUrl = VIS.Application.contextUrl;
    var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";
    var nonQueryUrl = baseUrl + "JsonData/ExecuteNonQuer";

    function getDataSetJString(data, async, callback) {
        var result = null;
        // data.sql = VIS.secureEngine.encrypt(data.sql);
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
    var executeScalar = function (sql, params, callback) {
        var async = callback ? true : false;
        var dataIn = { sql: sql, page: 1, pageSize: 0 }

        var value = null;


        getDataSetJString(dataIn, async, function (jString) {
            dataSet = new VIS.DB.DataSet().toJson(jString);
            var dataSet = new VIS.DB.DataSet().toJson(jString);
            if (dataSet.getTable(0).getRows().length > 0) {
                value = dataSet.getTable(0).getRow(0).getCell(0);

            }
            else { value = null; }
            dataSet.dispose();
            dataSet = null;
            if (async) {
                callback(value);
            }
        });

        return value;
    };

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


    VCreateFromStatement.prototype.saveStatment = function () {
        if (this.$super.dGrid == null) {
            return false;
        }

        //	Get Shipment
        var C_BankStatement_ID = this.$super.mTab.getValue("C_BankStatement_ID");

        var model = {};//this.$super.dGrid.records;
        var selectedItems = this.$super.multiValues;

        if (selectedItems == null) {
            return false;
        }
        if (selectedItems.length <= 0) {
            return false;
        }
        //var splitValue = selectedItems.toString().split(',');
        for (var i = 0; i < selectedItems.length; i++) {
            model[i] = (selectedItems[i]);
        }

        return this.saveData(model, "", C_BankStatement_ID);
    }

    VCreateFromStatement.prototype.saveData = function (model, selectedItems, C_BankStatement_ID) {
        var obj = this;
        $.ajax({
            type: "POST",
            url: VIS.Application.contextUrl + "Common/SaveStatment",
            dataType: "json",
            data: {
                model: model,
                selectedItems: selectedItems,
                C_BankStatement_ID: C_BankStatement_ID
            },
            success: function (data) {
                returnValue = data.result;
                if (returnValue) {
                    obj.dispose();
                    obj.$super.$root.dialog('close');
                    obj.$super.setBusy(false);
                    return;
                }
                obj.$super.setBusy(false);
                alert(returnValue);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                debugger;
                console.log(textStatus);
                obj.$super.setBusy(false);
                alert(errorThrown);
            }
        });

    }

    //dispose call
    VCreateFromStatement.prototype.dispose = function () {
        if (this.disposeComponent != null)
            this.disposeComponent();
    };

    //Load form into VIS
    VIS.VCreateFromStatement = VCreateFromStatement;

})(VIS, jQuery);


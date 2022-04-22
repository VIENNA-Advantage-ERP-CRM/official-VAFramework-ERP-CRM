; (function (VIS, $) {

    function AcctViewerData(windowNu, ad_Client_ID, ad_Table_ID) {
        this.windowNo = windowNu;
        this.AD_Client_ID = ad_Client_ID;
        this.AD_Org_ID = 0;
        this.DateFrom = null;
        this.DateTo = null;
        var $selfObj = this;
        this.ASchemas = [];
        this.ASchema = [];
        // Create Log
        this.log = VIS.Logging.VLogger.getVLogger("AccountViewer");
        //paging variables
        var divPaging, ulPaging, liFirstPage, liPrevPage, liCurrPage, liNextPage, liLastPage, cmbPage;

        this.dataByData = null;
        this._elements = [];
        this.documentQuery = false;
        this.C_AcctSchema_ID = 0;
        this.PostingType = "";

        this.AD_Table_ID = ad_Table_ID;
        this.Record_ID = 0;

        this.whereInfo = [];
        this.tableInfo = [];

        this.displayQty = false;
        this.displaySourceAmt = false;
        this.displayDocumentInfo = false;

        this.sortBy1 = "";
        this.sortBy2 = "";
        this.sortBy3 = "";
        this.sortBy4 = "";
        this.group1 = false;
        this.group2 = false;
        this.group3 = false;
        this.group4 = false;
        this._leadingColumns = 0;
        this._ref1 = null;
        this._ref2 = null;
        this.TABLE_ALIAS = "zz";
        this.FUNCTION_COUNT = "Count";
        this.FUNCTION_SUM = "Sum";

        this.onLoaded = null;
        this.HasOrgUnit = null;


        if (this.AD_Client_ID == 0) {
            this.AD_Client_ID = VIS.Env.getCtx().getContextAsInt(this.windowNo, "AD_Client_ID");
        }
        if (this.AD_Client_ID == 0) {
            this.AD_Client_ID = VIS.Env.getCtx().ctx["AD_Client_ID"];
        }

        if (this.AD_Org_ID == 0) {
            this.AD_Org_ID = Number(VIS.context.getWindowTabContext(this.windowNo, 0, "AD_Org_ID"));
        }


        var elements = [
            "AmtAcctCr",
            "AmtAcctDr",
            "AmtSourceCr",
            "AmtSourceDr",
        ];

        VIS.translatedTexts = VIS.Msg.translate(VIS.Env.getCtx(), elements, true);


        this.ELEMENTTYPE_AD_Reference_ID = 181;
        this.ELEMENTTYPE_Account = "AC";
        this.ELEMENTTYPE_Activity = "AY";
        this.ELEMENTTYPE_BPartner = "BP";
        this.ELEMENTTYPE_LocationFrom = "LF";
        this.ELEMENTTYPE_LocationTo = "LT";
        this.ELEMENTTYPE_Campaign = "MC";
        this.ELEMENTTYPE_Organization = "OO";
        this.ELEMENTTYPE_OrgTrx = "OT";
        this.ELEMENTTYPE_Project = "PJ";
        this.ELEMENTTYPE_Product = "PR";
        this.ELEMENTTYPE_SubAccount = "SA";
        this.ELEMENTTYPE_SalesRegion = "SR";
        this.ELEMENTTYPE_UserList1 = "U1";
        this.ELEMENTTYPE_UserList2 = "U2";
        this.ELEMENTTYPE_UserElement1 = "X1";
        this.ELEMENTTYPE_UserElement2 = "X2";

        //show posted button or not ("Y" or ""), when "N" - not to show button
        var notShowPosted;

        // get Accounting Schema
        this.getClientAcctSchema = function (AD_Client_ID, OrgID) {
            var obj = [];
            var that = this;
            $.ajax({
                url: VIS.Application.contextUrl + "AcctViewerData/GetClientAcctSchema",
                type: 'POST',
                async: true,
                data: { ad_client_id: AD_Client_ID, ad_org_id: OrgID },
                success: function (data) {
                    if (data.result && data.result.AcctSchemas) {
                        var actSch = data.result.AcctSchemas;
                        for (var i = 0; i < actSch.length; i++) {
                            var line = {};
                            c_acctschema_id = actSch[i].Key;
                            line['Key'] = c_acctschema_id;
                            line['Name'] = VIS.Utility.encodeText(actSch[i].Name);
                            obj.push(line);
                        }
                    }
                    if (data.result && data.result.OtherAcctSchemas) {
                        var otheActSch = data.result.OtherAcctSchemas;
                        for (var i = 0; i < otheActSch.length; i++) {
                            var lineObj = {};
                            lineObj['Key'] = otheActSch[i].Key;
                            lineObj['Name'] = VIS.Utility.encodeText(otheActSch[i].Name);
                            obj.push(lineObj);
                        }
                    }
                    that.ASchemas = obj;
                    that.ASchema = obj[0];
                    if (that.onLoaded)
                        that.onLoaded();
                },
                error: function (e) {
                    $selfObj.log.info(e);
                    if (that.onLoaded)
                        that.onLoaded();
                },
            });
            return obj;
        };


        this.getClientAcctSchema(this.AD_Client_ID, this.AD_Org_ID);
        //this.ASchemas = this.getClientAcctSchema(this.AD_Client_ID, this.AD_Org_ID);
        // this.ASchema = this.ASchemas[0];
        //function getClientAcctSchema(AD_Client_ID, OrgID) {
        //    var obj = [];

        //    //var sql = "SELECT C_ACCTSCHEMA_ID,NAME FROM C_ACCTSCHEMA WHERE C_ACCTSCHEMA_ID=(SELECT  C_ACCTSCHEMA1_ID   FROM AD_CLIENTINFO WHERE AD_CLIENT_ID=" + AD_Client_ID + ")";

        //    sql = "SELECT C_ACCTSCHEMA_ID,NAME FROM C_AcctSchema WHERE ISACTIVE='Y' AND C_ACCTSCHEMA_ID IN( " +
        //"SELECT C_ACCTSCHEMA_ID FROM FRPT_AssignedOrg WHERE ISACTIVE='Y' AND AD_CLIENT_ID=" + AD_Client_ID + " AND AD_ORG_ID=" + OrgID + ")" +
        ////Get default Accounting schema selected on tenant
        //" OR C_ACCTSCHEMA_ID IN (SELECT C_ACCTSCHEMA1_ID  FROM AD_ClientInfo where  AD_Client_ID=" + AD_Client_ID + ")";


        //    var c_acctschema_id = null;
        //    var dr = VIS.DB.executeReader(sql.toString(), null, null);

        //    while (dr.read()) {
        //        var line = {};
        //        c_acctschema_id = dr.getInt(0);
        //        line['Key'] = c_acctschema_id;
        //        line['Name'] = dr.getString(1);
        //        obj.push(line);
        //    }
        //    dr.close();

        //    //	Other
        //    sql = "SELECT c_acctschema_id,name FROM C_AcctSchema acs "
        //        + "WHERE IsActive='Y'"
        //        + " AND EXISTS (SELECT * FROM C_AcctSchema_GL gl WHERE acs.C_AcctSchema_ID=gl.C_AcctSchema_ID)"
        //        + " AND EXISTS (SELECT * FROM C_AcctSchema_Default d WHERE acs.C_AcctSchema_ID=d.C_AcctSchema_ID)";
        //    if (AD_Client_ID != 0) {
        //        sql += " AND AD_Client_ID=" + AD_Client_ID;
        //    }

        //    sql += " ORDER BY C_AcctSchema_ID";
        //    dr = VIS.DB.executeReader(sql.toString(), null, null);
        //    while (dr.read()) {
        //        var line = {};
        //        var id = dr.getInt(0);
        //        line['Key'] = id;
        //        line['Name'] = dr.getString(1);

        //        if (id != c_acctschema_id)	//	already in _elements
        //        {
        //            sql = "SELECT c_acctschema_id,name from C_AcctSchema WHERE C_AcctSchema_ID=" + id;
        //            var drSch = VIS.DB.executeReader(sql.toString(), null, null);
        //            if (drSch.read()) {
        //                var lineObj = {};
        //                lineObj['Key'] = drSch.getInt(0);
        //                lineObj['Name'] = drSch.getString(1);
        //                obj.push(lineObj);
        //            }
        //            drSch.close();
        //        }
        //    }
        //    dr.close();
        //    return obj;
        //}
    }

    AcctViewerData.prototype.getAcctSchema = function () {
        //for (var i = 0; i < this.ASchemas.length; i++) {
        //    ctrol.append(" <option value='" + this.ASchemas[i].c_acctschema_id + "'>" + this.ASchemas[i].name + "</option>");
        //}
        return this.ASchemas;
    }

    // Get table dataName
    AcctViewerData.prototype.getTable = function () {
        var options = [];
        var defaultKey = null;
        /** cached the object of account viewer data to set tables info.  **/
        var $that = this;
        $.ajax({
            url: VIS.Application.contextUrl + "AcctViewerData/AcctViewerGetTabelData",
            type: 'POST',
            async: false,
            success: function (data) {

                if (this.tableInfo == undefined || this.tableInfo == null) {
                    this.tableInfo = [];
                }
                if (data.result) {
                    options.push({ "Key": "", "Name": "" });
                    var res = data.result;
                    for (var i = 0; i < res.length; i++) {
                        var id = res[i].AD_Table_ID;
                        var tableName = VIS.Utility.encodeText(res[i].TableName);
                        var name = "";
                        // Change done to show order instead of purchase order in selection 
                        if (tableName == "C_Order") {
                            name = VIS.Msg.getMsg("Order");
                        }
                        else {
                            name = VIS.Msg.translate(VIS.Env.getCtx(), tableName + "_ID");
                        }

                        options.push({ "Key": tableName, "Name": name });

                        $that.tableInfo.push({
                            "Key": id, "Name": tableName

                        });

                        if (id == this.AD_Table_ID) {
                            defaultKey = tableName;
                        }
                    }
                }
            },
            error: function (e) {
                $selfObj.log.info(e);
            },
        });
        return options;
    }



    //AcctViewerData.prototype.getTable = function () {
    //    var options = [];
    //    var defaultKey = null;

    //    var sql = "SELECT AD_Table_ID, TableName FROM AD_Table t "
    //        + "WHERE EXISTS (SELECT * FROM AD_Column c"
    //        + " WHERE t.AD_Table_ID=c.AD_Table_ID AND c.ColumnName='Posted')"
    //        + " AND IsView='N'";

    //    try {

    //        var dr = VIS.DB.executeReader(sql.toString(), null, null);
    //        while (dr.read()) {
    //            var id = dr.getInt(0);
    //            var tableName = dr.getString(1);
    //            var name = VIS.Msg.translate(VIS.Env.getCtx(), tableName + "_ID");

    //            options.push({ "Key": tableName, "Name": name });

    //            this.tableInfo.push({ "Key": id, "Name": tableName });

    //            if (id == this.AD_Table_ID) {
    //                defaultKey = tableName;
    //            }
    //        }
    //        dr.close();
    //    }
    //    catch (e) {

    //    }
    //    return options;
    //}


    //Get Organization
    AcctViewerData.prototype.getOrg = function () {
        var obj = [];
        var cID = this.AD_Client_ID;
        $.ajax({
            url: VIS.Application.contextUrl + "AcctViewerData/AcctViewerGetOrgData",
            type: 'POST',
            async: false,
            data: { client_id: cID },
            success: function (data) {
                var res = data.result;
                if (res) {
                    obj.push({ 'Key': 0, 'Name': "" });
                    for (var i = 0; i < res.length; i++) {
                        obj.push({
                            'Key': res[i].AD_Org_ID, 'Name': VIS.Utility.encodeText(res[i].OrgName)
                        });
                    }
                }
            },
            error: function (e) {
                $selfObj.log.info(e);
            },
        });
        return obj;
    }

    //AcctViewerData.prototype.getOrg = function () {
    //    var obj = [];
    //    var sql = "SELECT AD_Org_ID, Name FROM AD_Org WHERE AD_Client_ID=" + this.AD_Client_ID + " ORDER BY Value";
    //    var dr = VIS.DB.executeReader(sql.toString(), null, null);
    //    obj.push({ 'Key': 0, 'Name': "" });
    //    while (dr.read()) {
    //        obj.push({ 'Key': dr.getInt(0), 'Name': dr.getString(1) });
    //    }
    //    dr.close();
    //    return obj;
    //}



    // Get posting Data  
    AcctViewerData.prototype.getPostingType = function () {
        var AD_Reference_ID = 125;
        var obj = [];

        $.ajax({
            url: VIS.Application.contextUrl + "AcctViewerData/AcctViewerGetPostingType",
            type: 'POST',
            async: false,
            data: { reference_id: AD_Reference_ID },
            success: function (data) {
                var res = data.result;
                if (res) {
                    obj.push({ 'Key': 0, 'Name': "" });
                    for (var i = 0; i < res.length; i++) {
                        obj.push({
                            'Key': res[i].PostingValue, 'Name': VIS.Utility.encodeText(res[i].PostingName)
                        });
                    }
                }
            },
            error: function (e) {
                $selfObj.log.info(e);
            },
        });
        return obj;
    }


    //AcctViewerData.prototype.getPostingType = function () {
    //    var AD_Reference_ID = 125;
    //    var obj = [];

    //    var sql = " SELECT Value, Name FROM AD_Ref_List "
    //        + "WHERE AD_Reference_ID=" + AD_Reference_ID + " AND IsActive='Y' ORDER BY 1";

    //    var dr = VIS.DB.executeReader(sql.toString(), null, null);
    //    obj.push({ 'Key': 0, 'Name': "" });
    //    while (dr.read()) {
    //        obj.push({ 'Key': dr.getString(0), 'Name': dr.getString(1) });
    //    }
    //    dr.close();
    //    return obj;
    //}



    // Get Accounting Schema Elements
    AcctViewerData.prototype.getAcctSchemaElements = function (C_AcctSchema_ID) {
        this._elements = [];
        var currentThis = this._elements;
        $.ajax({
            url: VIS.Application.contextUrl + "AcctViewerData/AcctViewerGetAcctSchElements",
            type: 'POST',
            async: true,
            data: { keys: C_AcctSchema_ID },
            success: function (data) {
                var res = data.result;
                if (res) {
                    for (var i = 0; i < res.length; i++) {
                        var ase = [];
                        ase.push({
                            'c_acctschema_element_id': res[i].C_AcctSchema_Element_ID, 'name': VIS.Utility.encodeText(res[i].ElementName),
                            'elementtype': res[i].ElementType, 'c_elementvalue_id': res[i].C_ElementValue_ID,
                            'seqno': res[i].SeqNo, 'detail': res[i].Detail, 'c_element_id': res[i].C_Element_ID
                        });
                        currentThis.push(ase);
                    }
                }
            },
            error: function (e) {
                $selfObj.log.info(e);
            },
        });
        return currentThis;
    }


    //AcctViewerData.prototype.getAcctSchemaElements = function (C_AcctSchema_ID) {

    //    //working on this function
    //    var key = C_AcctSchema_ID;
    //    this._elements = [];

    //    var sql = "SELECT c_acctschema_element_id,name,elementtype,c_elementvalue_id,seqno," +
    //    "'AcctSchemaElement['||c_acctschema_element_id||'-'||name||'('||elementtype||')='||c_elementvalue_id||',Pos='||seqno||']' as detail,c_element_id FROM C_AcctSchema_Element "
    //        + "WHERE C_AcctSchema_ID=" + key + " AND IsActive='Y' ORDER BY SeqNo";

    //    try {
    //        var dr = VIS.DB.executeReader(sql.toString(), null, null);
    //        while (dr.read()) {
    //            var ase = [];
    //            ase.push({ 'c_acctschema_element_id': dr.getInt(0), 'name': dr.getString(1), 'elementtype': dr.getString(2), 'c_elementvalue_id': dr.getInt(3), 'seqno': dr.getString(4), 'detail': dr.getString(5), 'c_element_id': dr.getInt(6) });

    //            this._elements.push(ase);
    //        }
    //        dr.close();
    //    }
    //    catch (e) {
    //    }

    //    return this._elements;
    //}

    AcctViewerData.prototype.getAcctSchemaElement = function (elementType) {
        if (this._elements == null) {
            this.getAcctSchemaElements();
        }
        for (var i = 0; i < this._elements.length; i++) {
            var ase = this._elements[i][0];
            if (ase.elementtype.equals(elementType)) {
                return ase;
            }
        }
        return null;
    };

    AcctViewerData.prototype.getColumnName = function (elementType) {
        if (elementType.equals(this.ELEMENTTYPE_Organization))
            return "AD_Org_ID";
        else if (elementType.equals(this.ELEMENTTYPE_Account))
            return "Account_ID";
        else if (elementType.equals(this.ELEMENTTYPE_BPartner))
            return "C_BPartner_ID";
        else if (elementType.equals(this.ELEMENTTYPE_Product))
            return "M_Product_ID";
        else if (elementType.equals(this.ELEMENTTYPE_Activity))
            return "C_Activity_ID";
        else if (elementType.equals(this.ELEMENTTYPE_LocationFrom))
            return "C_LocFrom_ID";
        else if (elementType.equals(this.ELEMENTTYPE_LocationTo))
            return "C_LocTo_ID";
        else if (elementType.equals(this.ELEMENTTYPE_Campaign))
            return "C_Campaign_ID";
        else if (elementType.equals(this.ELEMENTTYPE_OrgTrx))
            return "AD_OrgTrx_ID";
        else if (elementType.equals(this.ELEMENTTYPE_Project))
            return "C_Project_ID";
        else if (elementType.equals(this.ELEMENTTYPE_SalesRegion))
            return "C_SalesRegion_ID";
        else if (elementType.equals(this.ELEMENTTYPE_UserList1))
            return "User1_ID";
        else if (elementType.equals(this.ELEMENTTYPE_UserList2))
            return "User2_ID";
        else if (elementType.equals(this.ELEMENTTYPE_UserElement1))
            return "UserElement1_ID";
        else if (elementType.equals(this.ELEMENTTYPE_UserElement2))
            return "UserElement2_ID";
        //
        return "";
    }


    // Get the value from the info window.
    AcctViewerData.prototype.getButtonText = function (tableName, columnName, selectSQL) {

        var language = VIS.Env.getAD_Language(VIS.Env.getCtx());
        var lookupDirEmbed = VIS.MLookupFactory.getLookup_TableDirEmbed(language, columnName, "avd", columnName);
        var retValue = "<" + selectSQL + ">";
        $.ajax({
            url: VIS.Application.contextUrl + "AcctViewerData/AcctViewerGetButtonText",
            type: 'POST',
            async: false,
            data: {
                lookupDirEmbeded: lookupDirEmbed,
                tName: tableName,
                wheres: " avd WHERE avd.",
                selectSQLs: selectSQL
            },
            success: function (data) {
                var res = data.result;
                if (res != "") {
                    retValue = res;
                }
            },
            error: function (e) {
                $selfObj.log.info(e);
            },
        });
        return retValue;
    }


    //AcctViewerData.prototype.getButtonText = function (tableName, columnName, selectSQL) {
    //    var sql = "SELECT (";
    //    var language = VIS.Env.getAD_Language(VIS.Env.getCtx());

    //    sql = sql.concat(VIS.MLookupFactory.getLookup_TableDirEmbed(language, columnName, "avd", columnName))
    //        .concat(") FROM ").concat(tableName).concat(" avd WHERE avd.").concat(selectSQL);

    //    var retValue = "<" + selectSQL + ">";
    //    try {
    //        var dr = VIS.DB.executeReader(sql.toString(), null, null);
    //        if (dr.read()) {
    //            retValue = dr.getString(0);
    //        }
    //        dr.close();
    //    }
    //    catch (e) {
    //    }
    //    return retValue;
    //}

    AcctViewerData.prototype.Query = function (AD_Client_ID, callbackGetDataModel, resetPageCtrls, pNo) {
        //  Set Where Clause
        var whereClause = "";
        //  Add Organization
        if (this.C_AcctSchema_ID != 0) {
            whereClause = whereClause.concat(this.TABLE_ALIAS).concat(".C_AcctSchema_ID=").concat(this.C_AcctSchema_ID);
        }

        //	Posting Type Selected
        if (this.PostingType != null && this.PostingType.length > 0) {
            if (whereClause.length > 0) {
                whereClause = whereClause.concat(" AND ");
            }
            whereClause = whereClause.concat(this.TABLE_ALIAS).concat(".PostingType='").concat(this.PostingType).concat("'");
        }

        if (this.documentQuery) {
            if (whereClause.length > 0) {
                whereClause = whereClause.concat(" AND ");
            }
            whereClause = whereClause.concat(this.TABLE_ALIAS).concat(".AD_Table_ID=").concat(this.AD_Table_ID).concat(" AND ").concat(this.TABLE_ALIAS).concat(".Record_ID=").concat(this.Record_ID);
        }
        else {
            //  get values (Queries)
            var it = this.whereInfo;//.Values.GetEnumerator();
            if (it != null) {
                for (var i = 0; i < it.length; i++) {
                    var where = it[i].Value;
                    if (where != null && where.length > 0)    //  add only if not empty
                    {
                        if (whereClause.length > 0) {
                            whereClause = whereClause.concat(" AND ");
                        }
                        whereClause = whereClause.concat(this.TABLE_ALIAS).concat(".").concat(where);
                    }
                }
            }

            if (!this.DateFrom != null || this.DateTo != null) {
                if (whereClause.length > 0 && (this.DateFrom != "" || this.DateTo != "")) {
                    whereClause = whereClause.concat(" AND ");
                }
                if (this.DateFrom != "" && this.DateTo != "") {
                    whereClause = whereClause.concat("TRUNC(").concat(this.TABLE_ALIAS).concat(".DateAcct,'DD') BETWEEN ")
                        .concat(VIS.DB.to_date(this.DateFrom)).concat(" AND ").concat(VIS.DB.to_date(this.DateTo));
                }
                else if (this.DateFrom != "") {
                    whereClause = whereClause.concat("TRUNC(").concat(this.TABLE_ALIAS).concat(".DateAcct,'DD') >= ")
                        .concat(VIS.DB.to_date(this.DateFrom));
                }
                else if (this.DateTo != "") {
                    whereClause = whereClause.concat("TRUNC(").concat(this.TABLE_ALIAS).concat(".DateAcct,'DD') <= ")
                        .concat(VIS.DB.to_date(this.DateTo));
                }
            }

            //  Add Organization
            if (this.AD_Org_ID != 0) {
                if (whereClause.length > 0) {
                    whereClause = whereClause.concat(" AND ");
                }
                whereClause = whereClause.concat(this.TABLE_ALIAS).concat(".AD_Org_ID=").concat(this.AD_Org_ID);
            }
        }

        //  Set Order By Clause
        var orderClause = "";
        if (this.sortBy1.length > 0) {
            orderClause = orderClause.concat(this.TABLE_ALIAS).concat(".").concat(this.sortBy1);
        }
        if (this.sortBy2.length > 0) {
            if (orderClause.length > 0) {
                orderClause = orderClause.concat(",");
            }
            orderClause = orderClause.concat(this.TABLE_ALIAS).concat(".").concat(this.sortBy2);
        }
        if (this.sortBy3.length > 0) {
            if (orderClause.length > 0) {
                orderClause = orderClause.concat(",");
            }
            orderClause = orderClause.concat(this.TABLE_ALIAS).concat(".").concat(this.sortBy3);
        }
        if (this.sortBy4.length > 0) {
            if (orderClause.length > 0) {
                orderClause = orderClause.concat(",");
            }
            orderClause = orderClause.concat(this.TABLE_ALIAS).concat(".").concat(this.sortBy4);
        }
        if (orderClause.length == 0) {
            orderClause = orderClause.concat(this.TABLE_ALIAS).concat(".Fact_Acct_ID");
        }
        if (!pNo) {
            pNo = 1;
        }

        this.getDataModel(AD_Client_ID, whereClause, orderClause, this.group1, this.group2, this.group3, this.group4, this.sortBy1, this.sortBy2, this.sortBy3, this.sortBy4, this.displayDocumentInfo, this.displaySourceAmt, this.displayQty, callbackGetDataModel, resetPageCtrls, pNo);
        //var val = this.dataByData;
        //return val;
    }

    AcctViewerData.prototype.getDataModel = function (AD_Client_ID, whereClause, orderClause, gr1, gr2, gr3, gr4, sort1, sort2, sort3, sort4, displayDocInfo, displaySrcAmt, displayqty, callbackGetDataModel, resetPageCtrls, pNo) {
        var obj = this;
        $.ajax({
            url: VIS.Application.contextUrl + "Common/GetDataQuery",
            async: true,
            dataType: "json",
            type: "POST",
            data: {
                AD_Client_ID: AD_Client_ID,
                whereClause: whereClause,
                orderClause: orderClause,
                gr1: gr1,
                gr2: gr2,
                gr3: gr3,
                gr4: gr4,
                sort1: sort1,
                sort2: sort2,
                sort3: sort3,
                sort4: sort4,
                displayDocInfo: displayDocInfo,
                displaySrcAmt: displaySrcAmt,
                displayqty: displayqty,
                pageNo: pNo
            },
            success: function (data) {
                obj.dataByData = data.result;
                resetPageCtrls(data.result.pSetting);
                callbackGetDataModel(data.result);
                //return data.result;
            }
        });
    }

    //form declaretion
    function AcctViewer(AD_Client_ID, AD_Table_ID, Record_ID, windowNum, AD_Window_ID) {

        var $root = $("<div style='position:relative;'>");
        var $busyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');

        var $self = this;
        var _AD_Client_ID = AD_Client_ID;
        var _AD_Table_ID = AD_Table_ID;
        var _Record_ID = Record_ID;
        var windowNo = windowNum;
        var _AD_Window_ID = AD_Window_ID;

        this.arrListColumns = [];
        this.dGrid = null;
        this.menuForm = false;

        var Load = false;
        var ACCT_SCHEMA = "C_AcctSchema_ID";
        var DOC_TYPE = "DocumentType";
        var POSTING_TYPE = "PostingType";
        var ORG = "AD_Org_ID";
        var TRXORG = "AD_OrgTrx_ID";
        var ACCT = "Account_ID";
        var SELECT_DOCUMENT = "SelectDocument";
        var ACCT_DATE = "AcctDateFrom";//DateAcct

        var PROD = "M_Product_ID";
        var BPARTNER = "C_BPartner_ID";
        var PROJECT = "C_Project_ID";
        var CAMPAIGN = "C_Campaign_ID";


        var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/Find24.gif";
        var srcz = VIS.Application.contextUrl + "Areas/VIS/Images/cancel-18.png";
        var srcRefresh = VIS.Application.contextUrl + "Areas/VIS/Images/base/Refresh24.png";
        var srcPrint = VIS.Application.contextUrl + "Areas/VIS/Images/base/Print24.PNG";

        var btnRefresh = $("<button id='" + "btnRefresh_" + windowNo + "'style='margin-top: 0;' class='VIS_Pref_btn-2'><i class='vis vis-refresh'></i></button>");
        var btnPrint = $("<button class='VIS_Pref_btn-2' id='" + "btnPrint_" + windowNo + "' style='margin-top: 0px; margin-left: 10px;'><i class='vis vis-print'></button>");
        var btnRePost = $("<button class='VIS_Pref_btn-2' id='" + "btnRePost_" + windowNo + "' style='margin-top: 10px;'><img src='" + src + "'/></button>");


        var btnSelctDoc = $("<button class='input-group-text' Name='btnSelctDoc' id='" + "btnSelctDoc_" + windowNo + "'><i class='vis vis-find'></i></button>");
        var btnAccount = $("<button class='input-group-text' Name='btnAccount' id='" + "btnAccount_" + windowNo + "'><i class='vis vis-find'></i></button>");
        var btnProduct = $("<button class='input-group-text' Name='btnProduct' id='" + "btnProduct_" + windowNo + "'><i class='vis vis-find'></i></button>");
        var btnBPartner = $("<button class='input-group-text' Name='btnBPartner' id='" + "btnBPartner_" + windowNo + "'><i class='vis vis-find'></i></button>");
        var btnProject = $("<button class='input-group-text'Name='btnProject' id='" + "btnProject_" + windowNo + "'><i class='vis vis-find'></i></button>");
        var btnCampaning = $("<button class='input-group-text' Name='btnCampaning' id='" + "btnCampaning_" + windowNo + "'><i class='vis vis-find'></i></button>");
        var btnOrgUnit = $("<button class='input-group-text' Name='btnOrgUnit' id='" + "btnOrgUnit_" + windowNo + "'><i class='vis vis-find'></i></button>");

        var btnSelctDocClear = $("<button Name='btnSelctDocClear' id='" + "btnSelctDocClear_" + windowNo + "' class='input-group-text'><i class='vis vis-mark'></i></button>");
        var btnAccountClear = $("<button  Name='btnAccountClear' id='" + "btnAccountClear_" + windowNo + "'   class='input-group-text'><i class='vis vis-mark'></i></button>");
        var btnProductClear = $("<button  Name='btnProductClear' id='" + "btnProductClear_" + windowNo + "'   class='input-group-text'><i class='vis vis-mark'></i></button>");
        var btnBPartnerClear = $("<button Name='btnBPartnerClear' id='" + "btnBPartnerClear_" + windowNo + "' class='input-group-text'><i class='vis vis-mark'></i></button>");
        var btnProjectClear = $("<button  Name='btnProjectClear' id='" + "btnProjectClear_" + windowNo + "'   class='input-group-text'><i class='vis vis-mark'></i></button>");
        var btnCampaningClear = $("<button Name='btnCampaningClear' id='" + "btnCampaningClear_" + windowNo + "' class='input-group-text'><i class='vis vis-mark'></i></button>");
        var btnOrgUnitClear = $("<button Name='btnOrgUnitClear' id='" + "btnOrgUnitClear_" + windowNo + "' class='input-group-text'><i class='vis vis-mark'></i></button>");




        var cmbAccSchema = new VIS.Controls.VComboBox('', false, false, true);
        var cmbAccSchemaFilter = new VIS.Controls.VComboBox('', false, false, true);
        var cmbPostType = new VIS.Controls.VComboBox('', false, false, true);
        var cmbOrg = new VIS.Controls.VComboBox('', false, false, true);
        var cmbSelectDoc = new VIS.Controls.VComboBox('', false, false, true);

        var cmbSort1 = new VIS.Controls.VComboBox('', false, false, true);
        var cmbSort2 = new VIS.Controls.VComboBox('', false, false, true);
        var cmbSort3 = new VIS.Controls.VComboBox('', false, false, true);
        var cmbSort4 = new VIS.Controls.VComboBox('', false, false, true);

        var lblAccSchema = new VIS.Controls.VLabel();
        var lblAccSchemaFilter = new VIS.Controls.VLabel();
        var lblPostType = new VIS.Controls.VLabel();
        var lblAccDate = new VIS.Controls.VLabel();
        var lblAccDateTo = new VIS.Controls.VLabel();
        var lblOrg = new VIS.Controls.VLabel();
        var lblAcc = new VIS.Controls.VLabel();
        var lblProduct = new VIS.Controls.VLabel();
        var lblBP = new VIS.Controls.VLabel();
        var lblProject = new VIS.Controls.VLabel();
        var lblCompaning = new VIS.Controls.VLabel();
        var lblSort = new VIS.Controls.VLabel();
        var lblSumrise = new VIS.Controls.VLabel();



        var lblSel5 = new VIS.Controls.VLabel();
        var lblSel6 = new VIS.Controls.VLabel();
        var lblSel7 = new VIS.Controls.VLabel();
        var lblSel8 = new VIS.Controls.VLabel();

        var lblOrgUnit = new VIS.Controls.VLabel();

        var btnSel5 = $("<button id='" + "btnSel5_" + windowNo + "' style='border: 0px;background-color: transparent; padding: 0px;'><img src='" + src + "' /><span></span></button>");
        var btnSel6 = $("<button id='" + "btnSel6_" + windowNo + "' style='border: 0px;background-color: transparent; padding: 0px;'><img src='" + src + "' /><span></span></button>");
        var btnSel7 = $("<button id='" + "btnSel7_" + windowNo + "' style='border: 0px;background-color: transparent; padding: 0px;'><img src='" + src + "' /><span></span></button>");
        var btnSel8 = $("<button id='" + "btnSel8_" + windowNo + "' style='border: 0px;background-color: transparent; padding: 0px;'><img src='" + src + "' /><span></span></button>");

        var lblstatusLine = new VIS.Controls.VLabel();

        var groupBox1 = new VIS.Controls.VLabel();
        var groupBox2 = new VIS.Controls.VLabel();
        var tabT1 = null;
        var tabT2 = null;

        var chkSelectDoc = $("<label id='" + "lblSelectDoc_" + windowNo + "' class='VIS_Pref_Label_Font vis-ec-col-lblchkbox'><input id='" + "chkSelectDoc_" + windowNo + "' type='checkbox' checked class='VIS_Pref_automatic'>" +
            "SelectDoc</label>");

        var chkQty = $("<input id='" + "chkQty_" + windowNo + "' type='checkbox' class='VIS_Pref_automatic'>" +
            "<span><label id='" + "lblQty_" + windowNo + "' class='VIS_Pref_Label_Font'>chkQty</label></span>");

        var chkforcePost = $("<input id='" + "chkforcePost_" + windowNo + "' type='checkbox' class='VIS_Pref_automatic' style='display: inline-block;margin-left: 20px; margin-top: 0;'>" +
            "<span><label id='" + "lblforcePost_" + windowNo + "' class='VIS_Pref_Label_Font'>chkforcePost</label></span>");

        var DrAndCr = $("<span style='margin-left: 20px;'><label id='" + "lblDrAndCR_" + windowNo + "' class='VIS_Pref_Label_Font'></label></span>");

        var chkDisDocinfo = $("<input id='" + "chkDisDocinfo_" + windowNo + "' type='checkbox' class='VIS_Pref_automatic'>" +
            "<span><label id='" + "lblDisDocinfo_" + windowNo + "' class='VIS_Pref_Label_Font'>chkDisDocinfo</label></span>");

        var chkDisSouce = $("<input id='" + "chkDisSouce_" + windowNo + "' type='checkbox' class='VIS_Pref_automatic'>" +
            "<span><label id='" + "lblDisSouce_" + windowNo + "' class='VIS_Pref_Label_Font'>chkDisSouce</label></span>");

        var chkSumriz1 = $("<input id='" + "chkSumriz1_" + windowNo + "' type='checkbox' class='VIS_Pref_automatic'>");
        //"<span><label id='" + "lblSumriz1_" + windowNo + "' class='VIS_Pref_Label_Font'>chkSumriz1</label></span>");

        var chkSumriz2 = $("<input id='" + "chkSumriz2_" + windowNo + "' type='checkbox' class='VIS_Pref_automatic'>");
        //"<span><label id='" + "lblSumriz2_" + windowNo + "' class='VIS_Pref_Label_Font'>chkSumriz2</label></span>");

        var chkSumriz3 = $("<input id='" + "chkSumriz3_" + windowNo + "' type='checkbox' class='VIS_Pref_automatic'>");
        //"<span><label id='" + "lblSumriz3_" + windowNo + "' class='VIS_Pref_Label_Font'>chkSumriz3</label></span>");

        var chkSumriz4 = $("<input id='" + "chkSumriz4_" + windowNo + "' type='checkbox' class='VIS_Pref_automatic'>");
        //"<span><label id='" + "lblSumriz4_" + windowNo + "' class='VIS_Pref_Label_Font'>chkSumriz4</label></span>");

        var vdtpkAccDateFrom = $("<input id='" + "vdtpkAccDateFrom_" + windowNo + "' type='date' name='DateOrdered'>");
        var vdtpkAccDateTo = $("<input id='" + "vdtpkAccDateTo_" + windowNo + "' type='date' name='DateOrdered'>");

        var topDiv = null;
        var leftSideDiv = null;
        var rightSideDiv = null;
        var div = null;
        var bottumDiv = null;
        var resultDiv = null;

        var leftSideDivWidth = $(window).width() / 2;
        var rightSideDivWidth = $(window).width() / 2;
        var selectDivHeight = $(window).height() - 200;
        var _data = null;
        //var _data = _data = new AcctViewerData(windowNo, AD_Client_ID, AD_Table_ID);;


        function jbInit() {
            //Selection
            groupBox1.getControl().text(VIS.Msg.getMsg("Selection"));
            lblAccSchema.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), ACCT_SCHEMA));
            lblAccSchemaFilter.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), ACCT_SCHEMA));
            lblPostType.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), POSTING_TYPE));
            chkSelectDoc.find("label").text(VIS.Msg.getMsg(SELECT_DOCUMENT));
            lblOrg.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), ORG));
            lblAcc.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), ACCT));
            lblAccDate.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), ACCT_DATE));
            lblAccDateTo.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "AcctDateTo"));
            // Mohit- added labels
            lblProduct.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), PROD));
            lblBP.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), BPARTNER));
            lblProject.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "Project"));
            lblCompaning.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), CAMPAIGN));
            lblOrgUnit.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "OrganizationUnit"));


            //Display
            groupBox2.getControl().text(VIS.Msg.getMsg("Display"));
            chkQty.find("label").text(VIS.Msg.getMsg("DisplayQty"));
            chkDisSouce.find("label").text(VIS.Msg.getMsg("DisplaySourceInfo"));
            chkDisDocinfo.find("label").text(VIS.Msg.getMsg("DisplayDocumentInfo"));
            lblSort.getControl().text(VIS.Msg.getMsg("SortBy"));
            lblSumrise.getControl().text(VIS.Msg.getMsg("GroupBy"));
            // 
            // South
            lblstatusLine.getControl().css("color", "rgba(var(--v-c-primary), 1)");//css("font-size", "28px").
            btnRePost.text(VIS.Msg.getMsg("RePost"));
            if (notShowPosted) {
                btnRePost.hide();
                chkforcePost.find("label").text(VIS.Msg.getMsg("Force"));
                chkforcePost.hide();
            }
            else {
                btnRePost.show();
                chkforcePost.find("label").text(VIS.Msg.getMsg("Force"));
                chkforcePost.show();
            }
        }

        function tab1Select() {
            tabT2.css("color", "rgba(var(--v-c-on-secondary), 1)");
            tabT1.css("font-size", "1rem").css("color", "rgba(var(--v-c-primary), 1)");
            rightSideDiv.show();
            leftSideDiv.show();
            ulPaging.css("display", "none");
            divPaging.css("display", "none");
            resultDiv.css("display", "none");
            btnRePost.hide();
            chkforcePost.hide();
            btnRefresh.show();
            lblstatusLine.getControl().show();
            DrAndCr.hide();
            lblAccSchemaFilter.getControl().hide();
            cmbAccSchemaFilter.getControl().hide();

            leftSideDiv.find("#btnSelctDoc_" + windowNo).attr("name", (leftSideDiv.find("#btnSelctDoc_" + windowNo).parent().find("select").val() + "_ID"));
            leftSideDiv.find("#btnSelctDocClear_" + windowNo).attr("name", (leftSideDiv.find("#btnSelctDocClear_" + windowNo).parent().find("select").val() + "_ID"));

        }

        function tab2Select() {
            tabT1.css("color", "rgba(var(--v-c-on-secondary), 1)");
            tabT2.css("font-size", "1rem").css("color", "rgba(var(--v-c-primary), 1)");
            rightSideDiv.hide();
            leftSideDiv.hide();
            ulPaging.css("display", "block");
            divPaging.css("display", "block");
            DrAndCr.show();
            btnRefresh.hide();
            lblstatusLine.getControl().hide();
            divPaging.append(ulPaging);
            bottumDiv.append(divPaging);
            resultDiv.css("display", "block");
            if (notShowPosted) {
                btnRePost.hide();
                chkforcePost.hide();
            }
            else {
                btnRePost.show();
                chkforcePost.show();
            }
            lblAccSchemaFilter.getControl().show();
            cmbAccSchemaFilter.getControl().show();
        }
        createPageSettings();
        //bottumDiv.append(ulPaging);
        //Paging UI
        function createPageSettings() {
            divPaging = $('<div class="vis-info-pagingwrp" style="text-align: right; flex: 1;">');
            ulPaging = $('<ul class="vis-statusbar-ul">');

            liFirstPage = $('<li style="opacity: 1;"><div><i class="vis vis-shiftleft" title="' + VIS.Msg.getMsg("FirstPage") + '" style="opacity: 0.6;"></i></div></li>');

            liPrevPage = $('<li style="opacity: 1;"><div><i class="vis vis-pageup" title="' + VIS.Msg.getMsg("PageUp") + '" style="opacity: 0.6;"></i></div></li>');

            cmbPage = $("<select>");

            liCurrPage = $('<li>').append(cmbPage);

            liNextPage = $('<li style="opacity: 1;"><div><i class="vis vis-pagedown" title="' + VIS.Msg.getMsg("PageDown") + '" style="opacity: 0.6;"></i></div></li>');

            liLastPage = $('<li style="opacity: 1;"><div><i class="vis vis-shiftright" title="' + VIS.Msg.getMsg("LastPage") + '" style="opacity: 0.6;"></i></div></li>');


            ulPaging.append(liFirstPage).append(liPrevPage).append(liCurrPage).append(liNextPage).append(liLastPage);
            pageEvents();
        }
        //Paging events
        function pageEvents() {
            liFirstPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    setBusy(true);
                    _data.Query(AD_Client_ID, callbackGetDataModel, resetPageCtrls, 1);
                }
            });
            liPrevPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    setBusy(true);
                    _data.Query(AD_Client_ID, callbackGetDataModel, resetPageCtrls, parseInt(cmbPage.val()) - 1);
                }
            });
            liNextPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    setBusy(true);
                    _data.Query(AD_Client_ID, callbackGetDataModel, resetPageCtrls, parseInt(cmbPage.val()) + 1);
                }
            });
            liLastPage.on("click", function () {
                if ($(this).css("opacity") == "1") {
                    setBusy(true);
                    _data.Query(AD_Client_ID, callbackGetDataModel, resetPageCtrls, parseInt(cmbPage.find("Option:last").val()));
                }
            });
            cmbPage.on("change", function () {
                setBusy(true);
                _data.Query(AD_Client_ID, callbackGetDataModel, resetPageCtrls, cmbPage.val());
            });

        }

        function setBusy(isBusy) {
            $busyDiv.css("display", isBusy ? 'block' : 'none');
        };

        function setModel(dataObj) {

            if ($self.dGrid != null) {
                $self.dGrid.destroy();
                $self.dGrid = null;
            }
            $self.arrListColumns = [];
            //Create Data Array
            var data = [];
            var count = 1;
            for (var i = 0; i < dataObj.Data.length; i++) {
                var row = dataObj.Data[i];
                DrAndCr.text(dataObj.DebitandCredit);
                var line = {};
                for (var j = 0; j < dataObj.Columns.length; j++) {
                    if ($self.arrListColumns.length != dataObj.Columns.length) {
                        // alignment of Credit and Debit field
                        if (row[j] != null && typeof (row[j]) == "number" &&
                            (VIS.translatedTexts.AmtAcctCr == dataObj.Columns[j] ||
                                VIS.translatedTexts.AmtAcctDr == dataObj.Columns[j] ||
                                VIS.translatedTexts.AmtSourceCr == dataObj.Columns[j] ||
                                VIS.translatedTexts.AmtSourceDr == dataObj.Columns[j])) {
                            $self.arrListColumns.push({
                                field: dataObj.Columns[j], caption: VIS.Msg.translate(VIS.Env.getCtx(),
                                    dataObj.Columns[j]), sortable: true, size: '16%', hidden: false, style: 'text-align: right'
                            });
                        }
                        else {
                            $self.arrListColumns.push({
                                field: dataObj.Columns[j], caption: VIS.Msg.translate(VIS.Env.getCtx(),
                                    dataObj.Columns[j]), sortable: true, size: '16%', hidden: false
                            });
                        }
                    }
                    if (row[j] != null && typeof (row[j]) == "object") {
                        line[dataObj.Columns[j]] = row[j].Name;
                    }
                    else {
                        if (row[j] != null && dataObj.Columns[j].indexOf("Date") > 0) {
                            if (row[j] != "") {
                                var date = new Date(parseInt(row[j].substr(6)));
                                if (data != null)
                                    line[dataObj.Columns[j]] = date.toLocaleDateString();
                            }
                        }
                        else if (row[j] != null && typeof (row[j]) == "number" &&
                            (VIS.translatedTexts.AmtAcctCr == dataObj.Columns[j] ||
                                VIS.translatedTexts.AmtAcctDr == dataObj.Columns[j] ||
                                VIS.translatedTexts.AmtSourceCr == dataObj.Columns[j] ||
                                VIS.translatedTexts.AmtSourceDr == dataObj.Columns[j])) {
                            line[dataObj.Columns[j]] = parseFloat(row[j]).toLocaleString();
                        }
                        else {
                            line[dataObj.Columns[j]] = row[j];
                        }
                    }
                }
                line['recid'] = count;
                count++;
                data.push(line);
            }
            setTimeout(10);

            w2utils.encodeTags(data);

            $self.dGrid = $(resultDiv).w2grid({
                name: "gridAccViewer" + windowNo,
                recordHeight: 37,
                //show: {
                //    lineNumbers: true  // indicates if line numbers column is visible
                //},
                columns: $self.arrListColumns,
                records: data
            });
        }

        function cleardata(button) {
            var keyColumn = button.attr('name');
            button.find('span').text(" ");
            if (_data.whereInfo.length > 0) {
                var index = _data.whereInfo.map(function (item) { return item.Key == keyColumn; }).indexOf(true);
                _data.whereInfo.splice(index, 1);
            }
            button.width = 44;
            return 0;
        }

        function events() {
            //  $root.find("lblquery_1

            if (tabT1 != null)
                tabT1.on(VIS.Events.onTouchStartOrClick, function () {
                    tab1Select();
                });

            if (tabT2 != null)
                tabT2.on(VIS.Events.onTouchStartOrClick, function () {
                    $self.dGrid.refresh();
                    tab2Select();
                });

            if (cmbAccSchema != null) {
                cmbAccSchema.getControl().change(function () {
                    actionAcctSchema();
                });
            }

            if (cmbAccSchemaFilter != null) {
                cmbAccSchemaFilter.getControl().change(function () {
                    setBusy(true);
                    _data.C_AcctSchema_ID = cmbAccSchemaFilter.getControl().find('option:selected').val();
                    setTimeout(function () {
                        //var dataValue = _data.Query(AD_Client_ID , callbackGetDataModel);
                        var pNo = 1;
                        _data.Query(AD_Client_ID, callbackGetDataModel, resetPageCtrls, pNo);
                        //if (dataValues != null) {
                        //    setModel(dataValues);
                        //}
                    }, 2);
                });
            }


            if (btnRefresh != null) {
                btnRefresh.on("click", function () {
                    setBusy(true);
                    cmbAccSchemaFilter.setValue(cmbAccSchema.getControl().find('option:selected').val());
                    actionQuery();
                    setBusy(false);
                });
            }

            if (chkSelectDoc != null) {
                chkSelectDoc.change(function () {
                    actionDocument();
                });
            }

            if (cmbSelectDoc != null) {
                cmbSelectDoc.getControl().change(function () {
                    actionTable();
                    _data.Record_ID = Record_ID;
                    _data.AD_Table_ID
                });
            }


            if (btnRePost != null) {


                //btnRePost.on("click", function () {
                //    setBusy(true);                
                //    var invoiceID = "(SELECT ca.c_invoice_id FROM c_allocationline ca" +
                //         " inner join c_invoice ci on ci.c_invoice_id= ca.c_invoice_id" +
                //         " WHERE ci.issotrx='Y' and ca.c_allocationhdr_id=" + _data.Record_ID;

                //    var postValue = "SELECT (SELECT SUM(al.amount) FROM c_allocationline al INNER JOIN" +
                //        " c_allocationhdr alh ON al.c_allocationhdr_id=alh.c_allocationhdr_id  WHERE " +
                //        " alh.posted   ='Y' and c_invoice_id=" + invoiceID + ")) as aloc  ," +
                //        "(SELECT SUM(cl.linenetamt)  FROM c_invoiceline cl WHERE " +
                //        " c_invoice_id     =" + invoiceID + ")) as adj  from dual";


                //    setTimeout(function () {
                //        var dr = VIS.DB.executeReader(postValue.toString(), null, null);
                //        if (dr.read()) {
                //            if (dr.getInt(0) - dr.getInt(1) == 0) {
                //                //reposting
                //                var sql = "update c_allocationhdr alh set alh.posted ='N' where alh.c_allocationhdr_id in (select c_allocationhdr_id from c_allocationline where c_invoice_id=" + invoiceID + "))";
                //                VIS.DB.executeQuery(sql);
                //            }
                //        }
                //        dr.close();
                //        dr = null;
                //    }, 5);
                //    actionRePost();
                //});


                btnRePost.on("click", function () {
                    setBusy(true);
                    var postedDemands = actionRePost();
                    setTimeout(function () {
                        if (postedDemands) {
                            rePostClick(_data.Record_ID);
                        }
                        else {
                            setBusy(false);
                        }
                    }, 5);

                });
            }

            if (btnSelctDoc != null) {
                btnSelctDoc.on("click", function () {
                    actionButton(btnSelctDoc);
                });
            }

            if (btnAccount != null) {
                btnAccount.on("click", function () {
                    actionButton(btnAccount);
                });
            }

            if (btnProduct != null) {
                btnProduct.on("click", function () {
                    actionButton(btnProduct);
                });
            }

            if (btnBPartner != null) {
                btnBPartner.on("click", function () {
                    actionButton(btnBPartner);
                });
            }

            if (btnProject != null) {
                btnProject.on("click", function () {
                    actionButton(btnProject);
                });
            }

            if (btnCampaning != null) {
                btnCampaning.on("click", function () {
                    actionButton(btnCampaning);
                });
            }

            if (btnOrgUnit != null) {
                btnOrgUnit.on("click", function () {
                    /** Open the info window according to the button clicked. **/
                    actionButton(btnOrgUnit);
                });
            }

            if (btnSelctDocClear != null) {
                btnSelctDocClear.on("click", function () {
                    cleardata(btnSelctDoc);
                });
            }

            if (btnAccountClear != null) {
                btnAccountClear.on("click", function () {
                    cleardata(btnAccount);
                });
            }

            if (btnProductClear != null) {
                btnProductClear.on("click", function () {
                    cleardata(btnProduct);
                });
            }

            if (btnBPartnerClear != null) {
                btnBPartnerClear.on("click", function () {
                    cleardata(btnBPartner);
                });
            }

            if (btnProjectClear != null) {
                btnProjectClear.on("click", function () {
                    cleardata(btnProject);
                });
            }

            if (btnCampaningClear != null) {
                btnCampaningClear.on("click", function () {
                    cleardata(btnCampaning);
                });
            }
            if (btnOrgUnitClear != null) {
                btnOrgUnitClear.on("click", function () {
                    /** Clear data from info button and the from the array maintained at backend. **/
                    cleardata(btnOrgUnit);
                });
            }
        }

        function rePostClick(_dataRecord_IDs) {
            $.ajax({
                url: VIS.Application.contextUrl + "AcctViewerData/AcctViewerRePost",
                type: 'POST',
                async: true,
                data: {
                    dataRecID: _dataRecord_IDs
                },
                success: function (data) {
                    //  var res = data.result;
                    //setBusy(false);
                },
                error: function (e) {
                    $selfObj.log.info(e);
                    setBusy(false);
                },
            });
        }

        function initializeComponent() {

            notShowPosted = VIS.Env.getCtx().getContext('#SHOW_REPOST').equals("N");
            topDiv = $("<div id='" + "topDiv_" + windowNo + "' style='float: left; width: 100%;'>" +
                "<div id='" + "queryDiv_" + windowNo + "'style='display: inline-block; margin-right: 15px; margin-top: 5px' >" +

                "<label id='" + "lblquery_" + windowNo + "' class='VIS_Pref_Label_Font' style='cursor: pointer;font-size: 1rem;color: rgba(var(--v-c-primary), 1);'>"
                + VIS.Msg.getMsg("ViewerQuery") + "</label></div>" +
                "<div id='" + "resulttopDiv_" + windowNo + "' style='display: inline-block; width: 160px;'>" +
                "<label id='" + "lblresult_" + windowNo + "' class='VIS_Pref_Label_Font' style='vertical-align: middle; cursor: pointer;'>"
                + VIS.Msg.getMsg("ViewerResult") + "</label></div></div>" +
                "</div>");
            var localdiv = $("<div id='" + "resultTopFilterDiv_" + windowNo + "' style='float: right;width: 50%;'>");
            var $DivInputWrap = $('<div class="input-group vis-input-wrap">');
            var $DivControlWrap = $('<div class="vis-control-wrap">');
            localdiv.append($DivInputWrap);
            $DivInputWrap.append($DivControlWrap);
            $DivControlWrap.append(cmbAccSchemaFilter.getControl()).append(lblAccSchemaFilter.getControl());
            topDiv.append(localdiv);

            //Left side Div designing
            leftSideDiv = $("<div class='vis-leftsidebarouterwrap' id='" + "leftSideDiv_" + windowNo + "' style='float: left;background-color: rgba(var(--v-c-secondary), 1);overflow: auto;padding: 10px;'>");

            if (VIS.Application.isRTL) {
                leftSideDiv.css("float", "right");
                //btnRefresh.css("float", "left");
                //btnRePost.css("float", "right");
            }

            leftSideDiv.css("width", "50%");// leftSideDivWidth);
            leftSideDiv.css("height", selectDivHeight);
            div = $("<div style='float: left;width: 100%;height: 100%;' id='" + "parameterLeftDiv_" + windowNo + "'>");
            leftSideDiv.append(div);

            var tble = $("<table style='width: 100%;height: 100%;'>");

            //line1
            var tr = $("<tr>");
            var td = $("<td>");
            var $FrmInputWrap = $('<div class="input-group vis-input-wrap">');
            var $FrmControlWrap = $('<div class="vis-control-wrap">');
            div.append(tble);
            tble.append(tr);
            //tr.append(td);
            //td.append(lblAccSchema.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            ////line2
            //tr = $("<tr>");
            //td = $("<td>");//width: 236px;
            //tble.append(tr);
            tr.append(td);
            td.append($FrmInputWrap);
            $FrmInputWrap.append($FrmControlWrap);
            $FrmControlWrap.append(cmbAccSchema.getControl()).append(lblAccSchema.getControl());

            //line3
            tr = $("<tr>");
            //td = $("<td style='padding: 4px 2px 2px;'>");
            var $FrmInputWrap = $('<div class="input-group vis-input-wrap">');
            var $FrmControlWrap = $('<div class="vis-control-wrap">');
            tble.append(tr);
            //tr.append(td);
            //td.append(lblPostType.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line4
            //tr = $("<tr>");
            td = $("<td>");
            // tble.append(tr);
            tr.append(td);
            td.append($FrmInputWrap);
            $FrmInputWrap.append($FrmControlWrap);
            $FrmControlWrap.append(cmbPostType.getControl()).append(lblPostType.getControl());

            //line5
            tr = $("<tr>");
            td = $("<td>");
            var $FrmInputWrap = $('<div class="input-group vis-input-wrap">');
            var $FrmControlWrap = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append($FrmInputWrap);
            $FrmInputWrap.append($FrmControlWrap);
            $FrmControlWrap.append(chkSelectDoc);

            tr = $("<tr>");

            td = $("<td>");
            var $FrmInputWrap = $('<div class="input-group vis-input-wrap">');
            var $FrmControlWrap = $('<div class="vis-control-wrap">');
            var $FrmCtrlBtnWrap = $('<div class="input-group-append">');

            tble.append(tr);

            tr.append(td);
            td.append($FrmInputWrap);
            $FrmInputWrap.append($FrmControlWrap);
            $FrmInputWrap.append($FrmCtrlBtnWrap);
            $FrmControlWrap.append(cmbSelectDoc.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' '));

            //// tr = $("<tr>");
            //td = $("<td>");//style='padding: 0px 15px 0px;'
            //// tble.append(tr);
            //tr.append(td);
            $FrmCtrlBtnWrap.append(btnSelctDoc).append(btnSelctDocClear);

            //line6
            tr = $("<tr>");
            //td = $("<td style='padding: 4px 2px 2px;'>");
            var $FrmInputWrap = $('<div class="input-group vis-input-wrap">');
            var $FrmControlWrap = $('<div class="vis-control-wrap">');
            tble.append(tr);
            //tr.append(td);
            //td.append(lblAccDate.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line7
            // tr = $("<tr>");
            td = $("<td>");
            // tble.append(tr);
            tr.append(td);
            td.append($FrmInputWrap);
            $FrmInputWrap.append($FrmControlWrap);
            $FrmControlWrap.append(vdtpkAccDateFrom).append(lblAccDate.getControl());

            //line8
            tr = $("<tr>");
            //td = $("<td style='padding: 4px 2px 2px;'>");
            var $FrmInputWrap = $('<div class="input-group vis-input-wrap">');
            var $FrmControlWrap = $('<div class="vis-control-wrap">');
            tble.append(tr);
            //tr.append(td);
            //td.append(lblAccDateTo.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line9
            // tr = $("<tr>");
            td = $("<td>");
            //  tble.append(tr);
            tr.append(td);
            td.append($FrmInputWrap);
            $FrmInputWrap.append($FrmControlWrap);
            $FrmControlWrap.append(vdtpkAccDateTo).append(lblAccDateTo.getControl());

            //line10
            tr = $("<tr>");
            //td = $("<td style='padding: 4px 2px 2px;'>");
            var $FrmInputWrap = $('<div class="input-group vis-input-wrap">');
            var $FrmControlWrap = $('<div class="vis-control-wrap">');
            tble.append(tr);
            //tr.append(td);
            //td.append(lblOrg.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line11
            // tr = $("<tr>");
            td = $("<td>");
            // tble.append(tr);
            tr.append(td);
            td.append($FrmInputWrap);
            $FrmInputWrap.append($FrmControlWrap);
            $FrmControlWrap.append(cmbOrg.getControl()).append(lblOrg.getControl());

            //line12
            tr = $("<tr>");
            //td = $("<td style='padding: 4px 2px 2px;'>");
            var $FrmInputWrap = $('<div class="input-group vis-input-wrap">');
            var $FrmControlWrap = $('<div class="vis-control-wrap">');
            var $FrmCtrlBtnWrap = $('<div class="input-group-append">');
            var $FrmInputRO = $('<input type="text" readonly data-hasbtn=" ">');
            tble.append(tr);
            //tr.append(td);
            //td.append(lblAcc.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line13
            //  tr = $("<tr>");
            td = $("<td>");
            // tble.append(tr);
            tr.append(td);
            td.append($FrmInputWrap);
            $FrmInputWrap.append($FrmControlWrap);
            $FrmInputWrap.append($FrmCtrlBtnWrap);
            $FrmControlWrap.append($FrmInputRO).append(lblAcc.getControl());
            $FrmCtrlBtnWrap.append(btnAccount).append(btnAccountClear);

            //line20
            tr = $("<tr>");
            //td = $("<td style='padding: 4px 2px 2px;'>");
            var $FrmInputWrap = $('<div class="input-group vis-input-wrap">');
            var $FrmControlWrap = $('<div class="vis-control-wrap">');
            var $FrmCtrlBtnWrap = $('<div class="input-group-append">');
            var $FrmInputRO = $('<input type="text" readonly data-hasbtn=" ">');
            tble.append(tr);
            //tr.append(td);
            //td.append(lblOrgUnit.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line21
            // tr = $("<tr>");
            td = $("<td>");
            //  tble.append(tr);
            tr.append(td);
            td.append($FrmInputWrap);
            $FrmInputWrap.append($FrmControlWrap);
            $FrmInputWrap.append($FrmCtrlBtnWrap);
            $FrmControlWrap.append($FrmInputRO).append(lblOrgUnit.getControl());
            $FrmCtrlBtnWrap.append(btnOrgUnit).append(btnOrgUnitClear);

            //line14
            tr = $("<tr>");
            //td = $("<td style='padding: 4px 2px 2px;'>");
            var $FrmInputWrap = $('<div class="input-group vis-input-wrap">');
            var $FrmControlWrap = $('<div class="vis-control-wrap">');
            var $FrmCtrlBtnWrap = $('<div class="input-group-append">');
            var $FrmInputRO = $('<input type="text" readonly data-hasbtn=" ">');
            tble.append(tr);
            //tr.append(td);
            //td.append(lblProduct.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));


            //line15
            // tr = $("<tr>");
            td = $("<td>");
            // tble.append(tr);
            tr.append(td);
            td.append($FrmInputWrap);
            $FrmInputWrap.append($FrmControlWrap);
            $FrmInputWrap.append($FrmCtrlBtnWrap);
            $FrmControlWrap.append($FrmInputRO).append(lblProduct.getControl());
            $FrmCtrlBtnWrap.append(btnProduct).append(btnProductClear);

            //line16
            tr = $("<tr>");
            //td = $("<td style='padding: 4px 2px 2px;'>");
            var $FrmInputWrap = $('<div class="input-group vis-input-wrap">');
            var $FrmControlWrap = $('<div class="vis-control-wrap">');
            var $FrmCtrlBtnWrap = $('<div class="input-group-append">');
            var $FrmInputRO = $('<input type="text" readonly data-hasbtn=" ">');
            tble.append(tr);
            //tr.append(td);
            //td.append(lblBP.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line17
            // tr = $("<tr>");
            td = $("<td>");
            // tble.append(tr);
            tr.append(td);
            td.append($FrmInputWrap);
            $FrmInputWrap.append($FrmControlWrap);
            $FrmInputWrap.append($FrmCtrlBtnWrap);
            $FrmControlWrap.append($FrmInputRO).append(lblBP.getControl());
            $FrmCtrlBtnWrap.append(btnBPartner).append(btnBPartnerClear);

            //line18
            tr = $("<tr>");
            //td = $("<td style='padding: 4px 2px 2px;'>");
            var $FrmInputWrap = $('<div class="input-group vis-input-wrap">');
            var $FrmControlWrap = $('<div class="vis-control-wrap">');
            var $FrmCtrlBtnWrap = $('<div class="input-group-append">');
            var $FrmInputRO = $('<input type="text" readonly data-hasbtn=" ">');
            tble.append(tr);
            //tr.append(td);
            //td.append(lblProject.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line19
            // tr = $("<tr>");
            td = $("<td>");
            // tble.append(tr);
            tr.append(td);
            td.append($FrmInputWrap);
            $FrmInputWrap.append($FrmControlWrap);
            $FrmInputWrap.append($FrmCtrlBtnWrap);
            $FrmControlWrap.append($FrmInputRO).append(lblProject.getControl());
            $FrmCtrlBtnWrap.append(btnProject).append(btnProjectClear);

            //Right side Div designing
            rightSideDiv = $("<div id='" + "rightSideDiv_" + windowNo + "' style='float: right;margin-bottom: 3px; margin-right: 0px; '>");
            rightSideDiv.css("width", "50%");// rightSideDivWidth);
            rightSideDiv.css("height", selectDivHeight);
            div = $("<div style='float: left;width: 100%;height: 100%;' id='" + "parameterRightDiv_" + windowNo + "'>");
            rightSideDiv.append(div);

            tble = $("<table style='width: 100%;height: 100%;'>");

            //line1
            tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            div.append(tble);
            tble.append(tr);
            tr.append(td);
            td.append(chkDisDocinfo);

            //line2
            tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(chkDisSouce);

            //line3
            tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(chkQty);

            //line4
            tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(lblSort.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));
            td = $("<td style='padding: 0px 15px 0px;'>");
            tr.append(td);
            td.append(lblSumrise.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line5
            tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(cmbSort1.getControl().css("display", "inline-block").css("width", "100%").css("height", "32px"));
            td = $("<td style='padding: 0px 15px 0px;'>");
            tr.append(td);
            td.append(chkSumriz1);

            //line6
            tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(cmbSort2.getControl().css("display", "inline-block").css("width", "100%").css("height", "32px"));
            td = $("<td style='padding: 0px 15px 0px;'>");
            tr.append(td);
            td.append(chkSumriz2);

            //line7
            tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(cmbSort3.getControl().css("display", "inline-block").css("width", "100%").css("height", "32px"));
            td = $("<td style='padding: 0px 15px 0px;'>");
            tr.append(td);
            td.append(chkSumriz3);

            //line8
            tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(cmbSort4.getControl().css("display", "inline-block").css("width", "100%").css("height", "32px"));
            td = $("<td style='padding: 0px 15px 0px;'>");
            tr.append(td);
            td.append(chkSumriz4);

            //Bottum Div

            bottumDiv = $("<div class='vis-acctviewerbottdiv'>");
            //createPageSettings();
            //bottumDiv.append(ulPaging);
            //bottumDiv.css("height", 50);
            /** dont show repost butoon if form is opened from menu. **/
            if (!$self.getIsMenu()) {
                bottumDiv.append(btnRePost);
                bottumDiv.append(chkforcePost).append(DrAndCr);
            }

            bottumDiv.append(lblstatusLine.getControl().addClass("VIS_Pref_Label_Font"));
            bottumDiv.append(btnRefresh);
            bottumDiv.append(btnPrint);

            resultDiv = $("<div id='" + "resultDiv_" + windowNo + "' style='float: left; width: 100%; display: none;'>");
            //resultDiv.css("width", $(window).width() - 30);
            resultDiv.css("height", selectDivHeight);

            //Add to root

            $root.append(topDiv).append(leftSideDiv).append(rightSideDiv).append(resultDiv).append(bottumDiv);
            $root.append($busyDiv);
        }

        function dynInit(AD_Table_ID, Record_ID) {
            //if (!_data) {
            _data = new AcctViewerData(windowNo, AD_Client_ID, AD_Table_ID);
            _data.onLoaded = function () {
                // }
                //setTimeout(10);
                // setTimeout(function () {
                fillComboBox(cmbAccSchema.getControl(), _data.getAcctSchema());

                cmbAccSchemaFilter.getControl().append("<option value='-1' ></option>");
                fillComboBox(cmbAccSchemaFilter.getControl(), _data.getAcctSchema());
                actionAcctSchema();
                fillComboBox(cmbSelectDoc.getControl(), _data.getTable());
                fillComboBox(cmbPostType.getControl(), _data.getPostingType());
                fillComboBox(cmbOrg.getControl(), _data.getOrg());

                // }, 2);

                btnSelctDoc.find('span').text('');
                btnAccount.attr("Name", ACCT);
                btnAccountClear.attr("Name", ACCT);
                btnAccount.find('span').text('');

                // Change done to resolve the issue of dimentions control not opening.
                btnProduct.attr("Name", PROD);
                btnProductClear.attr("Name", PROD);
                btnProduct.find('span').text('');

                btnBPartner.attr("Name", BPARTNER);
                btnBPartnerClear.attr("Name", BPARTNER);
                btnBPartner.find('span').text('');

                btnProject.attr("Name", PROJECT);
                btnProjectClear.attr("Name", PROJECT);
                btnProject.find('span').text('');

                btnCampaning.attr("Name", CAMPAIGN);
                btnCampaningClear.attr("Name", CAMPAIGN);
                btnCampaning.find('span').text('');

                btnOrgUnit.attr("Name", TRXORG);
                btnOrgUnitClear.attr("Name", TRXORG);
                btnOrgUnit.find('span').text('');

                //  Document Select
                var haveDoc = AD_Table_ID != 0 && Record_ID != 0;

                if (haveDoc) {
                    chkSelectDoc.find('input').prop("checked", true);
                }

                actionDocument();
                actionTable();
                lblstatusLine.getControl().text(VIS.Msg.getMsg("VIS_EnterSelctionAndDisplayToQueryFind"));
                lblstatusLine.getControl().css("color", "rgba(var(--v-c-primary), 1)");//css("font-size", "28px").
                ////  Initial Query
                if (haveDoc) {
                    _data.AD_Table_ID = AD_Table_ID;
                    _data.Record_ID = Record_ID;

                }
                actionQuery();
            };
        }

        function actionAcctSchema() {
            var kp = cmbAccSchema.getControl().find('option:selected').val();
            if (kp == null) {
                return;
            }
            _data.C_AcctSchema_ID = kp;
            // _data.ASchema = MAcctSchema.Get(Env.GetCtx(), _data.C_AcctSchema_ID);//--->
            var elements = _data.getAcctSchemaElements(_data.C_AcctSchema_ID);//--->

            //  Sort Options
            cmbSort1.getControl().empty();
            cmbSort2.getControl().empty();
            cmbSort3.getControl().empty();
            cmbSort4.getControl().empty();

            sortAddItem({ 'Key': "", 'Name': "" });
            sortAddItem({ 'Key': "DateAcct", 'Name': VIS.Msg.translate(VIS.Env.getCtx(), "DateAcct") });
            sortAddItem({ 'Key': "DateTrx", 'Name': VIS.Msg.translate(VIS.Env.getCtx(), "DateTrx") });
            sortAddItem({ 'Key': "C_Period_ID", 'Name': VIS.Msg.translate(VIS.Env.getCtx(), "C_Period_ID") });

            var labels = [];

            labels.push(lblSel5);
            labels.push(lblSel6);
            labels.push(lblSel7);
            labels.push(lblSel8);

            var buttons = [];
            buttons.push(btnProduct);
            buttons.push(btnBPartner);
            buttons.push(btnProject);
            buttons.push(btnCampaning);
            buttons.push(btnProductClear);
            buttons.push(btnBPartnerClear);
            buttons.push(btnProjectClear);
            buttons.push(btnCampaningClear);
            buttons.push(btnSel5);
            buttons.push(btnSel6);
            buttons.push(btnSel7);
            buttons.push(btnSel8);
            buttons.push(btnOrgUnit);
            buttons.push(btnOrgUnitClear);

            var selectionIndex = 0;

            for (var i = 0; i < elements.length && selectionIndex < labels.length; i++) {
                var ase = elements[i][0];
                var columnName = _data.getColumnName(ase.elementtype);//GetColumnName();
                var displayColumnName = _data.getColumnName(ase.elementtype); //ase.GetDisplayColumnName();
                //  Add Sort Option
                sortAddItem({ 'Key': columnName, 'Name': VIS.Msg.translate(VIS.Env.getCtx(), displayColumnName) });
                //  Additional Elements
                if (!(ase.elementtype == "OO") && !(ase.elementtype == "AC")) {
                    labels[selectionIndex].getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), displayColumnName));
                    labels[selectionIndex].getControl().show();

                    buttons[selectionIndex].attr("Name", columnName);

                    buttons[selectionIndex].find('span').text('');
                    buttons[selectionIndex].show();
                    selectionIndex++;
                }
            }

            //	don't show remaining
            while (selectionIndex < labels.length) {
                labels[selectionIndex].getControl().hide();
                // buttons[selectionIndex++].hide();
                selectionIndex++;
            }

            cmbSort1.getControl().prop('selectedIndex', 0);
            cmbSort2.getControl().prop('selectedIndex', 0);
            cmbSort3.getControl().prop('selectedIndex', 0);
            cmbSort4.getControl().prop('selectedIndex', 0);
        }

        function sortAddItem(vo) {
            cmbSort1.getControl().append(" <option selected value='" + vo.Key + "' >" + vo.Name + "</option>");
            cmbSort2.getControl().append(" <option selected value='" + vo.Key + "' >" + vo.Name + "</option>");
            cmbSort3.getControl().append(" <option selected value='" + vo.Key + "' >" + vo.Name + "</option>");
            cmbSort4.getControl().append(" <option selected value='" + vo.Key + "' >" + vo.Name + "</option>");
        }

        function actionQuery() {

            //  Parameter Info
            var para = "";
            //  Reset Selection Data
            _data.C_AcctSchema_ID = 0;
            _data.AD_Org_ID = 0;

            //  Save Selection Choices
            var kp = cmbAccSchema.getControl().find('option:selected').val();
            if (kp != null) {
                _data.C_AcctSchema_ID = kp;
            }

            para = para.concat("C_AcctSchema_ID=").concat(_data.C_AcctSchema_ID);
            //
            kp = cmbPostType.getControl().find('option:selected').val();
            if (kp != null && kp != "0") {
                _data.PostingType = kp;
            }
            para = para.concat(", PostingType=").concat(_data.PostingType);

            //  Document
            _data.documentQuery = chkSelectDoc.find('input').prop("checked");
            para = para.concat(", DocumentQuery=").concat(_data.documentQuery);
            if (chkSelectDoc.find('input').prop("checked")) {
                if (_data.AD_Table_ID == 0 || _data.Record_ID == 0) {
                    return;
                }
                para = para.concat(", AD_Table_ID=").concat(_data.AD_Table_ID)
                    .concat(", Record_ID=").concat(_data.Record_ID);
            }
            else {
                _data.DateFrom = vdtpkAccDateFrom.val();

                if (_data.DateFrom != "")
                    para = para.concat(", DateFrom=").concat(_data.DateFrom);

                _data.DateTo = vdtpkAccDateTo.val();
                if (_data.DateTo != "")
                    para = para.concat(", DateTo=").concat(_data.DateTo);

                kp = cmbOrg.getControl().find('option:selected').val();
                if (kp != null) {
                    _data.AD_Org_ID = kp;
                }
                para = para.concat(", AD_Org_ID=").concat(_data.AD_Org_ID);

                var it = _data.whereInfo;//.Values.GetEnumerator();
                if (it != null) {
                    for (var i = 0; i < it.length; i++) {
                        para = para.concat(", ").concat(it[i].Value);
                    }
                }
            }

            //  Save Display Choices
            _data.displayQty = chkQty.prop("checked");
            para = para.concat(", - Display Qty=").concat(_data.displayQty);

            _data.displaySourceAmt = chkDisSouce.prop("checked");
            para = para.concat(", Source=").concat(_data.displaySourceAmt);
            _data.displayDocumentInfo = chkDisDocinfo.prop("checked");
            para = para.concat(", Doc=").concat(_data.displayDocumentInfo);


            if (cmbSort1.getControl().find('option:selected').val() != null) {
                _data.sortBy1 = cmbSort1.getControl().find('option:selected').val();
            }
            _data.group1 = chkSumriz1.prop("checked");
            para = para.concat(" - Sorting: ").concat(_data.sortBy1).concat("/").concat(_data.group1);

            if (cmbSort2.getControl().find('option:selected').val() != null) {
                _data.sortBy2 = cmbSort2.getControl().find('option:selected').val();
            }
            _data.group2 = chkSumriz2.prop("checked");

            para = para.concat(", ").concat(_data.sortBy2).concat("/").concat(_data.group2);

            if (cmbSort3.getControl().find('option:selected').val() != null) {
                _data.sortBy3 = cmbSort3.getControl().find('option:selected').val();
            }

            _data.group3 = chkSumriz3.prop("checked");
            para = para.concat(", ").concat(_data.sortBy3).concat("/").concat(_data.group3);

            if (cmbSort4.getControl().find('option:selected').val() != null) {
                _data.sortBy4 = cmbSort4.getControl().find('option:selected').val();
            }
            _data.group4 = chkSumriz4.prop("checked");

            para = para.concat(", ").concat(_data.sortBy4).concat("/").concat(_data.group4);

            //pfreshT2.Enabled = false;
            btnRefresh.Enabled = false;

            //***********************************Set line text
            lblstatusLine.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "Processing"));
            /** show first tab in casde the form is opened from menu.  **/
            if (!$self.getIsMenu()) {
                tab2Select();
            }
            else {
                if (!Load) {
                    tab1Select();
                }
                else {
                    $self.dGrid.refresh();
                    tab2Select();
                }
            }
            Load = true;
            //log.Config(para.ToString());
            //Causes the currently executing thread object to temporarily pause 
            //and allow other threads to execute. 
            //setModel(_data.Query(VIS.Env.getCtx()));

            //setTimeout(function () {
            // var dataValue = _data.Query(AD_Client_ID);
            var pNo = 1;
            _data.Query(AD_Client_ID, callbackGetDataModel, resetPageCtrls, pNo);
            //if (dataValues != null) {
            if (_data.C_AcctSchema_ID > 0) {
                cmbAccSchemaFilter.setValue(_data.C_AcctSchema_ID);
            }
            //setModel(dataValues);
            //    setBusy(false);
            //};
            //}, 2);
            btnRefresh.Enabled = true;
            //2nd tab status line text
            lblstatusLine.getControl().text(VIS.Msg.getMsg("VIS_EnterSelctionAndDisplayToQueryFind"));
        }

        function callbackGetDataModel(dataValues) {
            if (dataValues != null) {
                setModel(dataValues);
            };
            setBusy(false);
        };
        //reset Page controls
        function resetPageCtrls(psetting) {

            cmbPage.empty();
            if (psetting.TotalPage > 0) {
                for (var i = 0; i < psetting.TotalPage; i++) {
                    cmbPage.append($("<option value=" + (i + 1) + ">" + (i + 1) + "</option>"))
                }
                cmbPage.val(psetting.CurrentPage);


                if (psetting.TotalPage > psetting.CurrentPage) {
                    liNextPage.css("opacity", "1");
                    liLastPage.css("opacity", "1");
                }
                else {
                    liNextPage.css("opacity", "0.6");
                    liLastPage.css("opacity", "0.6");
                }

                if (psetting.CurrentPage > 1) {
                    liFirstPage.css("opacity", "1");
                    liPrevPage.css("opacity", "1");
                }
                else {
                    liFirstPage.css("opacity", "0.6");
                    liPrevPage.css("opacity", "0.6");
                }

                if (psetting.TotalPage == 1) {
                    liFirstPage.css("opacity", "0.6");
                    liPrevPage.css("opacity", "0.6");
                    liNextPage.css("opacity", "0.6");
                    liLastPage.css("opacity", "0.6");
                }

            }
            else {
                liFirstPage.css("opacity", "0.6");
                liPrevPage.css("opacity", "0.6");
                liNextPage.css("opacity", "0.6");
                liLastPage.css("opacity", "0.6");
            }
        }

        function actionDocument() {
            var doc = chkSelectDoc.find('input').prop("checked");
            if (doc) {
                cmbSelectDoc.getControl().removeAttr("disabled");
                btnSelctDoc.removeAttr("disabled");
                btnSelctDocClear.removeAttr("disabled");

                vdtpkAccDateFrom.attr('disabled', 'disabled');
                vdtpkAccDateTo.attr('disabled', 'disabled');
                cmbOrg.getControl().attr('disabled', 'disabled');

                btnAccount.attr('disabled', 'disabled');
                btnProduct.attr('disabled', 'disabled');
                btnBPartner.attr('disabled', 'disabled');
                btnProject.attr('disabled', 'disabled');
                btnCampaning.attr('disabled', 'disabled');
                btnOrgUnit.attr('disabled', 'disabled');

                btnAccountClear.attr('disabled', 'disabled');
                btnProductClear.attr('disabled', 'disabled');
                btnBPartnerClear.attr('disabled', 'disabled');
                btnProjectClear.attr('disabled', 'disabled');
                btnCampaningClear.attr('disabled', 'disabled');
                btnOrgUnitClear.attr('disabled', 'disabled');

                btnSel5.attr('disabled', 'disabled');
                btnSel6.attr('disabled', 'disabled');
                btnSel7.attr('disabled', 'disabled');
                btnSel8.attr('disabled', 'disabled');
            }
            else {
                cmbSelectDoc.getControl().attr('disabled', 'disabled');
                btnSelctDoc.attr('disabled', 'disabled');
                btnSelctDocClear.attr('disabled', 'disabled');

                vdtpkAccDateFrom.removeAttr("disabled");
                vdtpkAccDateTo.removeAttr("disabled");
                cmbOrg.getControl().removeAttr("disabled");

                btnAccount.removeAttr("disabled");
                btnProduct.removeAttr("disabled");
                btnBPartner.removeAttr("disabled");
                btnProject.removeAttr("disabled");
                btnCampaning.removeAttr("disabled");
                btnOrgUnit.removeAttr("disabled");

                btnAccountClear.removeAttr("disabled");
                btnProductClear.removeAttr("disabled");
                btnBPartnerClear.removeAttr("disabled");
                btnProjectClear.removeAttr("disabled");
                btnCampaningClear.removeAttr("disabled");
                btnOrgUnitClear.removeAttr("disabled");

                btnSel5.removeAttr("disabled");
                btnSel6.removeAttr("disabled");
                btnSel7.removeAttr("disabled");
                btnSel8.removeAttr("disabled");
            }
        }

        function actionTable() {
            var vp = cmbSelectDoc.getControl().find('option:selected').val();
            if (vp != null) {
                //_data.AD_Table_ID = _data.tableInfo[vp];
                if (jQuery.grep(_data.tableInfo, function (person) { return person.Name == vp })[0] != null) {
                    _data.AD_Table_ID = jQuery.grep(_data.tableInfo, function (person) { return person.Name == vp })[0].Key;
                }
            }

            //log.Config(vp + " = " + _data.AD_Table_ID);
            //  Reset Record
            // _data.Record_ID = 0;

            btnSelctDoc.find('span').text('');
            //btnSelctDoc.Name = vp + "_ID";
            btnSelctDoc.attr("Name", vp + "_ID");
            btnSelctDocClear.attr("Name", vp + "_ID");
        }

        function actionButton(button) {
            /** Check if organization unit window is available or not on target DB. **/
            if (_data.HasOrgUnit == null) {
                checkHasOrgUnit();
            }

            var keyColumn = button.attr('name');//.name;
            //log.Info(keyColumn);
            var whereClause = "IsSummary='N'";
            var lookupColumn = keyColumn;
            if (keyColumn.equals("Account_ID")) {
                lookupColumn = "C_ElementValue_ID";
                var ase = _data.getAcctSchemaElement("AC");
                if (ase != null) {
                    whereClause += " AND C_Element_ID=" + ase.c_element_id;
                }
            }
            else if (keyColumn.equals("User1_ID")) {
                lookupColumn = "C_ElementValue_ID";
                var ase = _data.getAcctSchemaElement("U1");
                if (ase != null) {
                    whereClause += " AND C_Element_ID=" + ase.c_element_id;
                }
            }
            else if (keyColumn.equals("User2_ID")) {
                lookupColumn = "C_ElementValue_ID";
                var ase = _data.getAcctSchemaElement("U2");
                if (ase != null) {
                    whereClause += " AND C_Element_ID=" + ase.c_element_id;
                }
            }
            else if (chkSelectDoc.find('input').prop("checked")) {
                whereClause = "";
            }

            var tableName = lookupColumn.substring(0, lookupColumn.length - 3);
            var info;
            if (keyColumn == "M_Product_ID") {
                info = new VIS.InfoWindow(101, "", _data.windowNo, "", false);
            }
            else if (keyColumn == "C_BPartner_ID") {
                info = new VIS.InfoWindow(100, "", _data.windowNo, "", false);
            }
            else {

                /** Check applied if to show the records from organization unit  **/
                if (keyColumn == TRXORG) {
                    lookupColumn = ORG;
                    tableName = lookupColumn.substring(0, lookupColumn.length - 3);
                    if (_data.HasOrgUnit) {
                        whereClause += " AND (IsCostCenter='Y' OR IsProfitCenter='Y')";
                    }
                    info = new VIS.infoGeneral(true, _data.windowNo, "", tableName, lookupColumn, false, whereClause);
                }
                else {
                    info = new VIS.infoGeneral(true, _data.windowNo, "", tableName, lookupColumn, false, whereClause);
                }
            }
            info.onClose = function () {

                //button.Text = "";
                //_data.whereInfo.Add(keyColumn, "");
                //return 0;

                var selectSQL = null;

                var key = info.getSelectedValues();
                if (key != null && key.length > 0) {
                    selectSQL = lookupColumn + "=" + key.toString();
                }

                info = null;
                if (selectSQL == null || selectSQL.length == 0 || key.length == 0) {
                    /** clear previous selection in case no selection is made from info window.  **/
                    cleardata(button);
                    return 0;
                }

                //  Save for query
                if (button == btnSelctDoc)                            //  Record_ID
                {
                    _data.Record_ID = key;
                }
                else {
                    /** clear previous selection in case a new selection is made from info window.  **/
                    cleardata(button);
                    _data.whereInfo.push({ 'Key': keyColumn, 'Value': keyColumn + "=" + key });

                }

                //  Display Selection and resize
                button.find('span').text(_data.getButtonText(tableName, lookupColumn, selectSQL));

                if (button.find('span').text() != "") {
                    button.AutoSize = true;
                }
                else {
                    button.Width = 44;
                }
                //pack();
                return key;

            };
            info.show();
        }

        function actionRePost() {
            VIS.ADialog.confirm("PostImmediate?", true, "", "Confirm", function (result) {
                if (result) {
                    setBusy(true);
                    if (_data.documentQuery && _data.AD_Table_ID != 0 && _data.Record_ID != 0) {
                        var force = chkforcePost.prop("checked");

                        //check for old and new posting logic
                        checkPostingByNewLogic(function (result) {
                            var postingByNewLogic = false;
                            if (result == "Yes") {
                                postingByNewLogic = true;
                            }

                            if (window.FRPT && postingByNewLogic) {
                                var orgID = Number(VIS.context.getWindowTabContext(windowNo, 0, "AD_Org_ID"));
                                var docTypeID = Number(VIS.context.getWindowTabContext(windowNo, 0, "C_DocType_ID"));

                                $.ajax({
                                    url: VIS.Application.contextUrl + "FRPT/PostingLogicFRPT/PostImediateFRPT",
                                    dataType: "json",
                                    async: true,
                                    data: {
                                        'AD_Client_ID': _data.AD_Client_ID,
                                        'AD_Table_ID': _data.AD_Table_ID,
                                        'Record_ID': _data.Record_ID,
                                        'force': force,
                                        'OrgID': orgID,
                                        'AD_Window_ID': _AD_Window_ID,
                                        'DocTypeID': docTypeID
                                    },
                                    error: function (e) {
                                        setBusy(false);
                                        VIS.ADialog.info(VIS.Msg.getMsg('ERRORGettingPostingServer'));
                                    },
                                    success: function (data) {
                                        if (data.result) {
                                            /*VIS_0045: Display Message on UI during Re-Post Document*/
                                            VIS.ADialog.info("", true, data.result, null);
                                            actionQuery();
                                        }
                                        setBusy(false);
                                    }
                                });
                            }
                            else {
                                $.ajax({
                                    url: VIS.Application.contextUrl + "Posting/PostImmediate",
                                    dataType: "json",
                                    data: {
                                        AD_Client_ID: _data.AD_Client_ID,
                                        AD_Table_ID: _data.AD_Table_ID,
                                        Record_ID: _data.Record_ID,
                                        force: force
                                    },
                                    error: function (e) {
                                        setBusy(false);
                                        VIS.ADialog.info(VIS.Msg.getMsg('ERRORGettingPostingServer'));
                                    },
                                    success: function (data) {
                                        if (data.result == "OK") {
                                            actionQuery();
                                        }
                                        setBusy(false);
                                    }
                                });
                            }
                        });
                    }
                    return true;
                }
                else {
                    setBusy(false);
                    return false;
                }
            });
        }

        function checkPostingByNewLogic(callback) {
            $.ajax({
                url: VIS.Application.contextUrl + "Posting/PostByNewLogic",
                dataType: "json",
                async: true,
                data: {
                    AD_Client_ID: VIS.context.getAD_Client_ID()
                },
                error: function (e) {
                    alert(VIS.Msg.getMsg('ERRORGettingPostingServer'));
                },
                success: function (data) {
                    if (callback) {
                        callback(data.result);
                    }
                }
            });
        }
        /** check if organization unit window available or not on the target DB. **/
        function checkHasOrgUnit() {
            $.ajax({
                url: VIS.Application.contextUrl + "AcctViewerData/HasOrganizationUnit",
                dataType: "json",
                async: false,
                error: function (e) {
                    alert(VIS.Msg.getMsg('ERROR'));
                },
                success: function (data) {
                    if (data != null) {
                        _data.HasOrgUnit = data.result;
                    }
                }
            });
        }

        function fillComboBox(cntrl, vo) {
            for (var i = 0; i < vo.length; i++) {
                cntrl.append(" <option selected value='" + vo[i].Key + "' >" + VIS.Utility.decodeText(vo[i].Name) + "</option>");
            }
            cntrl.prop('selectedIndex', 0);

            //if (vo.GetDefaultKey() != null)
            //{
            //    cb.SelectedItem = vo.GetDefaultKey();
            //}
        }

        this.showDialog = function () {
            initializeComponent();
            setBusy(true);
            tabT1 = $root.find("#lblquery_" + windowNo);
            tabT2 = $root.find("#lblresult_" + windowNo);
            /** load first tab in case form is opened from menu. **/
            if (!$self.getIsMenu()) {
                tab2Select();
            }
            else {

                tab1Select();
            }



            btnRePost.hide();
            chkforcePost.hide();
            btnPrint.hide();

            /// setTimeout(function () {
            jbInit();
            events();
            ///}, 2);

            dynInit(_AD_Table_ID, _Record_ID);

            $root.dialog({
                modal: true,
                resizable: false,
                title: VIS.Msg.getMsg("AcctViewer"),
                width: ($(window).width() / 2) + ($(window).width() / 2) / 2,
                //height: $(window).height() - 200,
                position: { at: "center top", of: window },
                close: function () {
                    if ($self.dGrid != null) {
                        $self.dGrid.destroy();
                        $self.dGrid = null;
                    }
                    $self.dispose();
                    $self = null;
                    $root.dialog("destroy");
                    $root = null;
                }
            });
            // actionQuery();
            //setBusy(false);
        };

        this.disposeComponent = function () {

            if (btnRefresh)
                btnRefresh.off("click");
            if (btnPrint)
                btnPrint.off("click");
            if (btnRePost)
                btnRePost.off("click");
            if (btnSelctDoc)
                btnSelctDoc.off("click");
            if (btnAccount)
                btnAccount.off("click");
            if (btnProduct)
                btnProduct.off("click");
            if (btnBPartner)
                btnBPartner.off("click");
            if (btnProject)
                btnProject.off("click");
            if (btnCampaning)
                btnCampaning.off("click");
            if (btnSelctDocClear)
                btnSelctDocClear.off("click");
            if (btnAccountClear)
                btnAccountClear.off("click");
            if (btnProductClear)
                btnProductClear.off("click");
            if (btnBPartnerClear)
                btnBPartnerClear.off("click");
            if (btnProjectClear)
                btnProjectClear.off("click");
            if (btnCampaningClear)
                btnCampaningClear.off("click");

            if (btnOrgUnitClear)
                btnOrgUnitClear.off("click");
            if (btnOrgUnit)
                btnOrgUnit.off("click");



            _AD_Client_ID = null;
            _AD_Table_ID = null;
            _Record_ID = null;;
            windowNo = null;;
            this.arrListColumns = null;
            this.dGrid = null;
            ACCT_SCHEMA = null;
            DOC_TYPE = null;
            POSTING_TYPE = null;
            ORG = null;
            ACCT = null;
            SELECT_DOCUMENT = null;
            ACCT_DATE = null;
            src = null;
            srcRefresh = null;
            srcPrint = null;
            btnRefresh = null;
            btnPrint = null;
            btnRePost = null;
            btnSelctDoc = null;
            btnAccount = null;
            btnProduct = null;
            btnBPartner = null;
            btnProject = null;
            btnCampaning = null;
            btnSelctDocClear = null;
            btnAccountClear = null;
            btnProductClear = null;
            btnBPartnerClear = null;
            btnProjectClear = null;
            btnCampaningClear = null;
            cmbAccSchema = null;
            cmbAccSchemaFilter = null;
            cmbPostType = null;
            cmbOrg = null;
            cmbSelectDoc = null;
            cmbSort1 = null;
            cmbSort2 = null;
            cmbSort3 = null;
            cmbSort4 = null;
            lblAccSchema = null;
            lblAccSchemaFilter = null;
            lblPostType = null;
            lblAccDate = null;;
            lblAccDateTo = null;
            lblOrg = null;
            lblAcc = null;
            lblProduct = null;
            lblBP = null;
            lblProject = null;
            lblCompaning = null;
            lblSort = null;
            lblSumrise = null;
            lblSel5 = null;
            lblSel6 = null;
            lblSel7 = null;
            lblSel8 = null;
            btnSel5 = null;
            btnSel6 = null;
            btnSel7 = null;
            btnSel8 = null;
            lblstatusLine = null;
            groupBox1 = null;
            groupBox2 = null;
            tabT1 = null;
            tabT2 = null;
            chkSelectDoc = null;
            chkQty = null;
            chkforcePost = null;
            chkDisDocinfo = null;
            chkDisSouce = null;
            chkSumriz1 = null;
            chkSumriz2 = null;
            chkSumriz3 = null;
            chkSumriz4 = null;
            vdtpkAccDateFrom = null;
            vdtpkAccDateTo = null;
            topDiv = null;
            leftSideDiv = null;
            rightSideDiv = null;
            div = null;
            bottumDiv = null;
            resultDiv = null;
            leftSideDivWidth = null;
            rightSideDivWidth = null;
            selectDivHeight = null;
            _data = null;

            $busyDiv = null;
            this.disposeComponent = null;

        };

    };

    AcctViewer.prototype.dispose = function () {
        this.disposeComponent();
    };
    /** Set proprty if form opened from menu **/
    AcctViewer.prototype.setIsMenu = function (isMenuForm) {
        if (isMenuForm != null && isMenuForm != undefined) {
            this.menuForm = isMenuForm;
        }
    };
    AcctViewer.prototype.getIsMenu = function () {
        return this.menuForm;
    };

    //Load form into VIS
    VIS.AcctViewer = AcctViewer;

})(VIS, jQuery);
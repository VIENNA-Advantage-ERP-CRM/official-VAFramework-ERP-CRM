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


        if (this.AD_Client_ID == 0) {
            this.AD_Client_ID = VIS.Env.getCtx().getContextAsInt(this.windowNo, "AD_Client_ID");
        }
        if (this.AD_Client_ID == 0) {
            this.AD_Client_ID = VIS.Env.getCtx().ctx["AD_Client_ID"];
        }

        if (this.AD_Org_ID == 0) {
            this.AD_Org_ID = Number(VIS.context.getWindowTabContext(this.windowNo, 0, "AD_Org_ID"));
        }



        this.ASchemas = getClientAcctSchema(this.AD_Client_ID, this.AD_Org_ID);
        this.ASchema = this.ASchemas[0];

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



        // get Accounting Schema
        function getClientAcctSchema(AD_Client_ID, OrgID) {
            var obj = [];
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

                },
                error: function (e) {
                    $selfObj.log.info(e);
                },
            });
            return obj;
        };


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

        $.ajax({
            url: VIS.Application.contextUrl + "AcctViewerData/AcctViewerGetTabelData",
            type: 'POST',
            async: true,
            success: function (data) {

                if (this.tableInfo == undefined || this.tableInfo == null) {
                    this.tableInfo = [];
                }
                if (data.result) {
                    var res = data.result;
                    for (var i = 0; i < res.length; i++) {
                        var id = res[i].AD_Table_ID;
                        var tableName = VIS.Utility.encodeText(res[i].TableName);
                        var name = VIS.Msg.translate(VIS.Env.getCtx(), tableName + "_ID");

                        options.push({ "Key": tableName, "Name": name });

                        this.tableInfo.push({
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
            async: true,
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
            async: true,
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
                            'c_acctschema_element_id': res[i].C_AcctSchema_Element_ID, 'name': VIS.Utility.encodeText(res[i].ElementName),                            'elementtype': res[i].ElementType, 'c_elementvalue_id': res[i].C_ElementValue_ID,
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
            async: true,
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

    AcctViewerData.prototype.Query = function (AD_Client_ID, callbackGetDataModel) {
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

        this.getDataModel(AD_Client_ID, whereClause, orderClause, this.group1, this.group2, this.group3, this.group4, this.sortBy1, this.sortBy2, this.sortBy3, this.sortBy4, this.displayDocumentInfo, this.displaySourceAmt, this.displayQty, callbackGetDataModel);
        //var val = this.dataByData;
        //return val;
    }

    AcctViewerData.prototype.getDataModel = function (AD_Client_ID, whereClause, orderClause, gr1, gr2, gr3, gr4, sort1, sort2, sort3, sort4, displayDocInfo, displaySrcAmt, displayqty, callbackGetDataModel) {
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
                displayqty: displayqty
            },
            success: function (data) {
                obj.dataByData = data.result;
                callbackGetDataModel(data.result);
                //return data.result;
            }
        });
    }

    //form declaretion
    function AcctViewer(AD_Client_ID, AD_Table_ID, Record_ID, windowNum, AD_Window_ID) {

        var $root = $("<div style='position:relative;'>");
        var $busyDiv = $("<div class='vis-apanel-busy' style='width:98%;height:98%;position:absolute'>");

        var $self = this;
        var _AD_Client_ID = AD_Client_ID;
        var _AD_Table_ID = AD_Table_ID;
        var _Record_ID = Record_ID;
        var windowNo = windowNum;
        var _AD_Window_ID = AD_Window_ID;

        this.arrListColumns = [];
        this.dGrid = null;

        var ACCT_SCHEMA = "C_AcctSchema_ID";
        var DOC_TYPE = "DocumentType";
        var POSTING_TYPE = "PostingType";
        var ORG = "AD_Org_ID";
        var ACCT = "Account_ID";
        var SELECT_DOCUMENT = "SelectDocument";
        var ACCT_DATE = "AcctDateFrom";//DateAcct

        var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/Find24.gif";
        var srcz = VIS.Application.contextUrl + "Areas/VIS/Images/cancel-18.png";
        var srcRefresh = VIS.Application.contextUrl + "Areas/VIS/Images/base/Refresh24.png";
        var srcPrint = VIS.Application.contextUrl + "Areas/VIS/Images/base/Print24.PNG";

        var btnRefresh = $("<button id='" + "btnRefresh_" + windowNo + "'style='margin-bottom: 1px; margin-top: 6px; float: right; margin-left: 15px; height: 38px;' class='VIS_Pref_btn-2'><img src='" + srcRefresh + "'/></button>");
        var btnPrint = $("<button id='" + "btnPrint_" + windowNo + "' style='border: 0px;background-color: transparent; padding: 0px;float: right;margin-right: 10px;margin-top: 5px;'><img src='"
            + srcPrint + "'/></button>");
        var btnRePost = $("<button id='" + "btnRePost_" + windowNo + "' style='float: left; display: inline-block;height: 30px;margin-top: 10px;margin-left: 5px;' ><img src='" + src + "'/></button>");


        var btnSelctDoc = $("<button Name='btnSelctDoc' id='" + "btnSelctDoc_" + windowNo + "' style='height:30px; margin-top: 1px;'><img src='" + src + "'/><span style='padding-left: 3px;'></span></button>");
        var btnAccount = $("<button  Name='btnAccount' id='" + "btnAccount_" + windowNo + "'   style='height:30px; padding: 0px;'><img src='" + src + "'/><span style='padding-left: 3px;'></span></button>");
        var btnProduct = $("<button  Name='btnProduct' id='" + "btnProduct_" + windowNo + "'   style='height:30px; padding: 0px;'><img src='" + src + "'/><span style='padding-left: 3px;'></span></button>");
        var btnBPartner = $("<button Name='btnBPartner' id='" + "btnBPartner_" + windowNo + "' style='height:30px; padding: 0px;'><img src='" + src + "'/><span style='padding-left: 3px;'></span></button>");
        var btnProject = $("<button  Name='btnProject' id='" + "btnProject_" + windowNo + "'   style='height:30px; padding: 0px;'><img src='" + src + "'/><span style='padding-left: 3px;'></span></button>");
        var btnCampaning = $("<button Name='btnCampaning' id='" + "btnCampaning_" + windowNo + "' style=' height:30px; padding: 0px;'><img src='" + src + "'/><span style='padding-left: 3px;'></span></button>");


        var btnSelctDocClear = $("<button Name='btnSelctDocClear' id='" + "btnSelctDocClear_" + windowNo + "' class='VIS-pref-button'><img src='" + srcz + "'/><span style='padding-left: 3px;'></span></button>");
        var btnAccountClear = $("<button  Name='btnAccountClear' id='" + "btnAccountClear_" + windowNo + "'   class='VIS-pref-button'><img src='" + srcz + "'/><span style='padding-left: 3px;'></span></button>");
        var btnProductClear = $("<button  Name='btnProductClear' id='" + "btnProductClear_" + windowNo + "'   class='VIS-pref-button'><img src='" + srcz + "'/><span style='padding-left: 3px;'></span></button>");
        var btnBPartnerClear = $("<button Name='btnBPartnerClear' id='" + "btnBPartnerClear_" + windowNo + "' class='VIS-pref-button'><img src='" + srcz + "'/><span style='padding-left: 3px;'></span></button>");
        var btnProjectClear = $("<button  Name='btnProjectClear' id='" + "btnProjectClear_" + windowNo + "'   class='VIS-pref-button'><img src='" + srcz + "'/><span style='padding-left: 3px;'></span></button>");
        var btnCampaningClear = $("<button Name='btnCampaningClear' id='" + "btnCampaningClear_" + windowNo + "' class='VIS-pref-button'><img src='" + srcz + "'/><span style='padding-left: 3px;'></span></button>");




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

        var btnSel5 = $("<button id='" + "btnSel5_" + windowNo + "' style='border: 0px;background-color: transparent; padding: 0px;'><img src='" + src + "' /><span></span></button>");
        var btnSel6 = $("<button id='" + "btnSel6_" + windowNo + "' style='border: 0px;background-color: transparent; padding: 0px;'><img src='" + src + "' /><span></span></button>");
        var btnSel7 = $("<button id='" + "btnSel7_" + windowNo + "' style='border: 0px;background-color: transparent; padding: 0px;'><img src='" + src + "' /><span></span></button>");
        var btnSel8 = $("<button id='" + "btnSel8_" + windowNo + "' style='border: 0px;background-color: transparent; padding: 0px;'><img src='" + src + "' /><span></span></button>");

        var lblstatusLine = new VIS.Controls.VLabel();

        var groupBox1 = new VIS.Controls.VLabel();
        var groupBox2 = new VIS.Controls.VLabel();
        var tabT1 = null;
        var tabT2 = null;

        var chkSelectDoc = $("<input id='" + "chkSelectDoc_" + windowNo + "' type='checkbox' checked class='VIS_Pref_automatic'>" +
           "<span><label id='" + "lblSelectDoc_" + windowNo + "' class='VIS_Pref_Label_Font'>SelectDoc</label></span>");

        var chkQty = $("<input id='" + "chkQty_" + windowNo + "' type='checkbox' class='VIS_Pref_automatic'>" +
           "<span><label id='" + "lblQty_" + windowNo + "' class='VIS_Pref_Label_Font'>chkQty</label></span>");

        var chkforcePost = $("<input id='" + "chkforcePost_" + windowNo + "' type='checkbox' class='VIS_Pref_automatic' style='display: inline-block;margin-left: 20px;'>" +
           "<span><label id='" + "lblforcePost_" + windowNo + "' class='VIS_Pref_Label_Font'>chkforcePost</label></span>");


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

        var vdtpkAccDateFrom = $("<input id='" + "vdtpkAccDateFrom_" + windowNo + "' type='date' name='DateOrdered' style='display: inline-block;line-height: 23px;width:100%'>");
        var vdtpkAccDateTo = $("<input id='" + "vdtpkAccDateTo_" + windowNo + "' type='date' name='DateOrdered' style='display: inline-block;line-height: 23px;width:100%'>");

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

            //Display
            groupBox2.getControl().text(VIS.Msg.getMsg("Display"));
            chkQty.find("label").text(VIS.Msg.getMsg("DisplayQty"));
            chkDisSouce.find("label").text(VIS.Msg.getMsg("DisplaySourceInfo"));
            chkDisDocinfo.find("label").text(VIS.Msg.getMsg("DisplayDocumentInfo"));
            lblSort.getControl().text(VIS.Msg.getMsg("SortBy"));
            lblSumrise.getControl().text(VIS.Msg.getMsg("GroupBy"));
            // 
            // South
            lblstatusLine.getControl().css("color", "#19A0ED");//css("font-size", "28px").
            btnRePost.text(VIS.Msg.getMsg("RePost"));
            btnRePost.show();
            chkforcePost.find("label").text(VIS.Msg.getMsg("Force"));
            chkforcePost.show();
        }

        function tab1Select() {
            tabT2.css("font-size", "14px").css("color", "#333333");
            tabT1.css("font-size", "28px").css("color", "#19A0ED");
            rightSideDiv.show();
            leftSideDiv.show();
            resultDiv.css("display", "none");
            btnRePost.hide();
            chkforcePost.hide();
            lblAccSchemaFilter.getControl().hide();
            cmbAccSchemaFilter.getControl().hide();

            leftSideDiv.find("#btnSelctDoc_" + windowNo).attr("name", (leftSideDiv.find("#btnSelctDoc_" + windowNo).parent().find("select").val() + "_ID"));
            leftSideDiv.find("#btnSelctDocClear_" + windowNo).attr("name", (leftSideDiv.find("#btnSelctDocClear_" + windowNo).parent().find("select").val() + "_ID"));

        }

        function tab2Select() {
            tabT1.css("font-size", "14px").css("color", "#333333");
            tabT2.css("font-size", "28px").css("color", "#19A0ED");
            rightSideDiv.hide();
            leftSideDiv.hide();
            resultDiv.css("display", "block");
            btnRePost.show();
            chkforcePost.show();
            lblAccSchemaFilter.getControl().show();
            cmbAccSchemaFilter.getControl().show();
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
            for (var i = 0; i < dataObj.Data.length ; i++) {
                var row = dataObj.Data[i];
                var line = {};
                for (var j = 0; j < dataObj.Columns.length; j++) {
                    if ($self.arrListColumns.length != dataObj.Columns.length) {
                        $self.arrListColumns.push({
                            field: dataObj.Columns[j], caption: VIS.Msg.translate(VIS.Env.getCtx(),
                                dataObj.Columns[j]), sortable: true, size: '16%', hidden: false
                        });
                    }
                    if (row[j] != null && typeof (row[j]) == "object") {
                        line[dataObj.Columns[j]] = row[j].Name;
                    }
                    else {
                        if (row[j] != null && dataObj.Columns[j].indexOf("Date") > 0) {
                            var date = new Date(parseInt(row[j].substr(6)));
                            if (data != null)
                                line[dataObj.Columns[j]] = date.toDateString();
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
                recordHeight: 40,
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
                        _data.Query(AD_Client_ID, callbackGetDataModel);
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

            topDiv = $("<div id='" + "topDiv_" + windowNo + "' style='float: left; width: 100%; height: 45px; margin-bottom: 2px;margin-left: 0px;'>" +
                      "<div id='" + "queryDiv_" + windowNo + "'style='display: inline-block;  padding-left: 15px;margin-right: 15px;' >" +
                       "<label id='" + "lblquery_" + windowNo + "' class='VIS_Pref_Label_Font' style='vertical-align: middle; cursor: pointer;font-size: 28px;color: #19A0ED;'>"
                      + VIS.Msg.getMsg("ViewerQuery") + "</label></div>" +
                      "<div id='" + "resulttopDiv_" + windowNo + "' style='display: inline-block; width: 160px;'>" +
                      "<label id='" + "lblresult_" + windowNo + "' class='VIS_Pref_Label_Font' style='vertical-align: middle; cursor: pointer;'>"
                      + VIS.Msg.getMsg("ViewerResult") + "</label></div>" +
                      "</div>");
            var localdiv = $("<div id='" + "resultTopFilterDiv_" + windowNo + "' style='float: right;width: 50%;'>");
            localdiv.append(lblAccSchemaFilter.getControl().css("display", "inline-block").css("width", "49%")
                .css("text-align", "right")
                .css("margin-top", "5px")
                .addClass("VIS_Pref_Label_Font"))
                .append(cmbAccSchemaFilter.getControl().css("display", "inline-block").css("width", "50%").css("float", "right").css("height", "32px"));
            topDiv.append(localdiv);

            //Left side Div designing
            leftSideDiv = $("<div id='" + "leftSideDiv_" + windowNo + "' style='float: left; margin-bottom: 3px; margin-left: 0px;background-color: #F1F1F1; '>");

            if (VIS.Application.isRTL) {
                leftSideDiv.css("float", "right");
                btnRefresh.css("float", "left");
                btnRePost.css("float", "right");
            }

            leftSideDiv.css("width", "50%");// leftSideDivWidth);
            leftSideDiv.css("height", selectDivHeight);
            div = $("<div style='float: left;width: 100%;height: 100%;' id='" + "parameterLeftDiv_" + windowNo + "'>");
            leftSideDiv.append(div);

            var tble = $("<table style='width: 100%;height: 100%;'>");

            //line1
            var tr = $("<tr>");
            var td = $("<td style='padding: 4px 2px 2px;'>");
            div.append(tble);
            tble.append(tr);
            tr.append(td);
            td.append(lblAccSchema.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            ////line2
            //tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px; '>");//width: 236px;
            //tble.append(tr);
            tr.append(td);
            td.append(cmbAccSchema.getControl().css("display", "inline-block").css("width", "100%").css("height", "32px"));

            //line3
            tr = $("<tr>");
            td = $("<td style='padding: 4px 2px 2px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(lblPostType.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line4
            //tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            // tble.append(tr);
            tr.append(td);
            td.append(cmbPostType.getControl().css("display", "inline-block").css("width", "100%").css("height", "32px"));

            //line5
            tr = $("<tr>");
            td = $("<td style='padding: 4px 2px 2px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(chkSelectDoc);

            // tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            // tble.append(tr);
            tr.append(td);
            td.append(cmbSelectDoc.getControl().css("display", "inline-block").css("width", "100%").css("height", "32px"));

            //// tr = $("<tr>");
            //td = $("<td>");//style='padding: 0px 15px 0px;'
            //// tble.append(tr);
            //tr.append(td);
            td.append(btnSelctDoc).append(btnSelctDocClear);

            //line6
            tr = $("<tr>");
            td = $("<td style='padding: 4px 2px 2px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(lblAccDate.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line7
            // tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            // tble.append(tr);
            tr.append(td);
            td.append(vdtpkAccDateFrom);

            //line8
            tr = $("<tr>");
            td = $("<td style='padding: 4px 2px 2px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(lblAccDateTo.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line9
            // tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            //  tble.append(tr);
            tr.append(td);
            td.append(vdtpkAccDateTo);

            //line10
            tr = $("<tr>");
            td = $("<td style='padding: 4px 2px 2px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(lblOrg.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line11
            // tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            // tble.append(tr);
            tr.append(td);
            td.append(cmbOrg.getControl().css("display", "inline-block").css("width", "100%").css("height", "32px"));

            //line12
            tr = $("<tr>");
            td = $("<td style='padding: 4px 2px 2px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(lblAcc.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line13
            //  tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            // tble.append(tr);
            tr.append(td);
            td.append(btnAccount).append(btnAccountClear);

            //line14
            tr = $("<tr>");
            td = $("<td style='padding: 4px 2px 2px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(lblProduct.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line15
            // tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            // tble.append(tr);
            tr.append(td);
            td.append(btnProduct).append(btnProductClear);

            //line16
            tr = $("<tr>");
            td = $("<td style='padding: 4px 2px 2px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(lblBP.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line17
            // tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            // tble.append(tr);
            tr.append(td);
            td.append(btnBPartner).append(btnBPartnerClear);

            //line18
            tr = $("<tr>");
            td = $("<td style='padding: 4px 2px 2px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(lblProject.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line19
            // tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            // tble.append(tr);
            tr.append(td);
            td.append(btnProject).append(btnProjectClear);

            //line20
            tr = $("<tr>");
            td = $("<td style='padding: 4px 2px 2px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(lblCompaning.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line21
            // tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            //  tble.append(tr);
            tr.append(td);
            td.append(btnCampaning).append(btnCampaningClear);

            //Right side Div designing
            rightSideDiv = $("<div id='" + "rightSideDiv_" + windowNo + "' style='float: right;margin-bottom: 3px; margin-right: 0px; '>");
            rightSideDiv.css("width", "50%");// rightSideDivWidth);
            rightSideDiv.css("height", selectDivHeight);
            div = $("<div style='float: left;width: 100%;height: 100%;' id='" + "parameterRightDiv_" + windowNo + "'>");
            rightSideDiv.append(div);

            tble = $("<table style='width: 100%;height: 100%;'>");

            //line1
            tr = $("<tr>");
            td = $("<td style='padding: 4px 15px 2px;'>");
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
            td = $("<td style='padding: 4px 15px 2px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(chkQty);

            //line4
            tr = $("<tr>");
            td = $("<td style='padding: 4px 15px 2px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(lblSort.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));
            td = $("<td style='padding: 4px 15px 2px;'>");
            tr.append(td);
            td.append(lblSumrise.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //line5
            tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(cmbSort1.getControl().css("display", "inline-block").css("width", "100%").css("height", "32px"));
            td = $("<td style='padding: 4px 15px 2px;'>");
            tr.append(td);
            td.append(chkSumriz1);

            //line6
            tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(cmbSort2.getControl().css("display", "inline-block").css("width", "100%").css("height", "32px"));
            td = $("<td style='padding: 4px 15px 2px;'>");
            tr.append(td);
            td.append(chkSumriz2);

            //line7
            tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(cmbSort3.getControl().css("display", "inline-block").css("width", "100%").css("height", "32px"));
            td = $("<td style='padding: 4px 15px 2px;'>");
            tr.append(td);
            td.append(chkSumriz3);

            //line8
            tr = $("<tr>");
            td = $("<td style='padding: 0px 15px 0px;'>");
            tble.append(tr);
            tr.append(td);
            td.append(cmbSort4.getControl().css("display", "inline-block").css("width", "100%").css("height", "32px"));
            td = $("<td style='padding: 4px 15px 2px;'>");
            tr.append(td);
            td.append(chkSumriz4);

            //Bottum Div

            bottumDiv = $("<div style='float: left;background-color: #F1F1F1; width:100%'>");
            bottumDiv.css("height", 50);

            bottumDiv.append(btnRePost);
            bottumDiv.append(chkforcePost);
            bottumDiv.append(lblstatusLine.getControl().css("margin-left", "10px").css("margin-right", "10px").css("margin-top", "15px").addClass("VIS_Pref_Label_Font"));
            bottumDiv.append(btnRefresh);
            bottumDiv.append(btnPrint);

            resultDiv = $("<div id='" + "resultDiv_" + windowNo + "' style='float: left; width: 100%; display: none; margin-bottom: 2px;margin-left: 0px;background-color: #F1F1F1;'>");
            //resultDiv.css("width", $(window).width() - 30);
            resultDiv.css("height", selectDivHeight);

            //Add to root

            $root.append(topDiv).append(leftSideDiv).append(rightSideDiv).append(resultDiv).append(bottumDiv);
            $root.append($busyDiv);
        }

        function dynInit(AD_Table_ID, Record_ID) {
            if (!_data) {
                _data = new AcctViewerData(windowNo, AD_Client_ID, AD_Table_ID);
            }

            setTimeout(function () {
                fillComboBox(cmbAccSchema.getControl(), _data.getAcctSchema());

                cmbAccSchemaFilter.getControl().append("<option value='-1' ></option>");
                fillComboBox(cmbAccSchemaFilter.getControl(), _data.getAcctSchema());
                actionAcctSchema();
                fillComboBox(cmbSelectDoc.getControl(), _data.getTable());
                fillComboBox(cmbPostType.getControl(), _data.getPostingType());
                fillComboBox(cmbOrg.getControl(), _data.getOrg());
            }, 2);

            btnSelctDoc.find('span').text('');
            btnAccount.attr("Name", ACCT);
            btnAccountClear.attr("Name", ACCT);
            btnAccount.find('span').text('')

            //  Document Select
            var haveDoc = AD_Table_ID != 0 && Record_ID != 0;

            if (haveDoc) {
                chkSelectDoc.prop("checked", true);
            }

            actionDocument();
            actionTable();
            lblstatusLine.getControl().text(VIS.Msg.getMsg("VIS_EnterSelctionAndDisplayToQueryFind"));
            lblstatusLine.getControl().css("color", "#19A0ED");//css("font-size", "28px").
            ////  Initial Query
            if (haveDoc) {
                _data.AD_Table_ID = AD_Table_ID;
                _data.Record_ID = Record_ID;
                //actionQuery();
            }
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
            labels.push(lblProduct);
            labels.push(lblBP);
            labels.push(lblProject);
            labels.push(lblCompaning);
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
            _data.documentQuery = chkSelectDoc.prop("checked");
            para = para.concat(", DocumentQuery=").concat(_data.documentQuery);
            if (chkSelectDoc.prop("checked")) {
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

            tab2Select();

            //log.Config(para.ToString());
            //Causes the currently executing thread object to temporarily pause 
            //and allow other threads to execute. 
            //setModel(_data.Query(VIS.Env.getCtx()));

            setTimeout(function () {
                // var dataValue = _data.Query(AD_Client_ID);
                _data.Query(AD_Client_ID, callbackGetDataModel);
                //if (dataValues != null) {
                if (_data.C_AcctSchema_ID > 0) {
                    cmbAccSchemaFilter.setValue(_data.C_AcctSchema_ID);
                }
                //setModel(dataValues);
                //    setBusy(false);
                //};
            }, 2);
            btnRefresh.Enabled = true;
            //2nd tab status line text
            lblstatusLine.getControl().text(VIS.Msg.getMsg("VIS_EnterSelctionAndDisplayToQueryFind"));
        }

        function callbackGetDataModel(dataValues) {
            if (dataValues != null) {
                setModel(dataValues);
                setBusy(false);
            };
        };

        function actionDocument() {
            var doc = chkSelectDoc.prop("checked");
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

                btnAccountClear.attr('disabled', 'disabled');
                btnProductClear.attr('disabled', 'disabled');
                btnBPartnerClear.attr('disabled', 'disabled');
                btnProjectClear.attr('disabled', 'disabled');
                btnCampaningClear.attr('disabled', 'disabled');


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

                btnAccountClear.removeAttr("disabled");
                btnProductClear.removeAttr("disabled");
                btnBPartnerClear.removeAttr("disabled");
                btnProjectClear.removeAttr("disabled");
                btnCampaningClear.removeAttr("disabled");


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
            else if (chkSelectDoc.prop("checked")) {
                whereClause = "";
            }

            var tableName = lookupColumn.substring(0, lookupColumn.length - 3);

            var info = new VIS.infoGeneral(true, _data.windowNo, "", tableName, lookupColumn, false, whereClause);
            info.onClose = function () {

                //button.Text = "";
                //_data.whereInfo.Add(keyColumn, "");
                //return 0;

                var selectSQL = null;

                var key = info.getSelectedValues();
                if (key != null) {
                    selectSQL = lookupColumn + "=" + key.toString();
                }

                info = null;
                if (selectSQL == null || selectSQL.length == 0 || key == null) {
                    button.find('span').text('')
                    _data.whereInfo.Remove(keyColumn);//remove select text from button by removeing key     //  no query
                    button.width = 44;
                    return 0;
                }

                //  Save for query
                if (button == btnSelctDoc)                            //  Record_ID
                {
                    _data.Record_ID = key;
                }
                else {
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
            if (VIS.ADialog.ask("PostImmediate?")) {
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
                                        actionQuery();
                                    }
                                    //  setBusy(false);
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
                                    // setBusy(false);
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
            tab2Select();
            btnRePost.hide();
            chkforcePost.hide();
            btnPrint.hide();

            setTimeout(function () {
                jbInit();
                events();
            }, 2);

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
            actionQuery();
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

    //Load form into VIS
    VIS.AcctViewer = AcctViewer;

})(VIS, jQuery);
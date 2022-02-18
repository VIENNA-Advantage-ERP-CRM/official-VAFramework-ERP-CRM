// MRole

; VIS.MRole.getDefault = function () {
    return VIS.MRole;
};

VIS.MRole.log = VIS.Logging.VLogger.getVLogger("MRole"); //log

VIS.MRole.canUpdate = function (AD_Client_ID, AD_Org_ID, AD_Table_ID, Record_ID, createError) {

    var userLevel = this.vo.UserLevel.trim(); // Format 'SCO'

    var retValue = true;
    var whatMissing = "";

    // System == Client=0 & Org=0
    if (AD_Client_ID == 0 && AD_Org_ID == 0 && userLevel.indexOf('S') == -1) {
        retValue = false;
        whatMissing += "S";
    }

    // Client == Client!=0 & Org=0
    else if (AD_Client_ID != 0 && AD_Org_ID == 0
        && userLevel.indexOf('C') == -1) {
        if (userLevel.indexOf('O') == -1 && this.getIsOrgAccess(AD_Org_ID, true))
            ; // Client+Org with access to *
        else {
            retValue = false;
            whatMissing += "C";
        }
    }

    // Organization == Client!=0 & Org!=0
    else if (AD_Client_ID != 0 && AD_Org_ID != 0
        && userLevel.indexOf('O') == -1) {
        retValue = false;
        whatMissing += "O";
    }

    // Data Access
    if (retValue)
        retValue = this.getIsTableAccess(AD_Table_ID, false);

    if (retValue && Record_ID != 0)
        retValue = this.getIsRecordAccess(AD_Table_ID, Record_ID, false);

    if (!retValue && createError) {
        VIS.MRole.log.warning("AccessTableNoUpdate => AD_Client_ID="
            + AD_Client_ID + ", AD_Org_ID=" + AD_Org_ID
            + ", UserLevel=" + userLevel + " => missing="
            + whatMissing);
        //log.warning(toString());
    }
    return retValue;
}; //can update

VIS.MRole.getIsColumnAccess = function (AD_Table_ID, AD_Column_ID, ro) {
    if (!this.getIsTableAccess(AD_Table_ID, ro)) // No Access to Table
        return false;

    var retValue = true; // assuming exclusive
    var m_columnAccess = this.vo.columnAccess;
    for (var i = 0; i < m_columnAccess.length; i++) {
        if (m_columnAccess[i].IsExclude) // Exclude
        // If you Exclude Access to a column and select Read Only,
        // you can only read data (otherwise no access).
        {
            if (m_columnAccess[i].AD_Table_ID == AD_Table_ID
                && m_columnAccess[i].AD_Column_ID == AD_Column_ID) {
                if (!ro) // just R/O Access requested
                    retValue = !m_columnAccess[i].IsReadOnly;
                else
                    retValue = false;
                if (!retValue)
                    //log.fine("Exclude AD_Table_ID=" + AD_Table_ID
                    //        + ", AD_Column_ID=" + AD_Column_ID + " (ro="
                    //        + ro + ",ColumnAccessRO="
                    //        + m_columnAccess[i].isReadOnly() + ") = "
                    //        + retValue);
                    return retValue;
            }
        } else // Include
        // If you Include Access to a column and select Read Only,
        // you can only read data (otherwise full access).
        {
            if (m_columnAccess[i].AD_Table_ID == AD_Table_ID) {
                // retValue = false;
                if (m_columnAccess[i].AD_Column_ID == AD_Column_ID) {
                    if (!ro) // rw only if not r/o
                        retValue = !m_columnAccess[i].IsReadOnly;
                    else
                        retValue = true;
                    if (!retValue)
                        //log.fine("Include AD_Table_ID=" + AD_Table_ID
                        //        + ", AD_Column_ID=" + AD_Column_ID
                        //        + " (ro=" + ro + ",ColumnAccessRO="
                        //        + m_columnAccess[i].isReadOnly() + ") = "
                        //        + retValue);
                        return retValue;
                }
            } // same table
        } // include
    } // for all Table Access
    if (!retValue) {
        //log.fine("AD_Table_ID=" + AD_Table_ID + ", AD_Column_ID="
        //        + AD_Column_ID + " (ro=" + ro + ") = " + retValue);
    }
    return retValue;
}; // isColumnAccess

VIS.MRole.getIsTableAccessLevel = function (AD_Table_ID, isAccess) {
    if (isAccess)				//	role can always read
        return true;

    var strRoleAccessLevel = this.vo.tableAccessLevel[AD_Table_ID];
    if (strRoleAccessLevel == null) {
        // log no access tableid
        //log.Fine("NO - No AccessLevel - AD_Table_ID=" + AD_Table_ID);
        return false;
    }
    //7
    if (strRoleAccessLevel.equals(VIS.Consts.ACCESSLEVEL_All)) {
        return true;
    }
    //	User Level = SCO
    var userLevel = this.vo.UserLevel.trim();
    //S,4,6
    if (userLevel.indexOf('S') != -1 && (strRoleAccessLevel.equals(VIS.Consts.ACCESSLEVEL_SystemOnly)
        || strRoleAccessLevel.equals(VIS.Consts.ACCESSLEVEL_SystemPlusClient))) {
        return true;
    }//8**********************************************************8//
    if (userLevel.indexOf('C') != -1 && (strRoleAccessLevel.equals(VIS.Consts.ACCESSLEVEL_ClientOnly)
        || strRoleAccessLevel.equals(VIS.Consts.ACCESSLEVEL_SystemPlusClient))) {
        return true;
    }
    if (userLevel.indexOf('O') != -1 && (strRoleAccessLevel.equals(VIS.Consts.ACCESSLEVEL_Organization)
        || strRoleAccessLevel.equals(VIS.Consts.ACCESSLEVEL_ClientPlusOrganization))) {
        return true;
    }
    //    log.Fine("NO - AD_Table_ID=" + AD_Table_ID
    //+ ", UserLevel=" + userLevel + ", AccessLevel=" + strRoleAccessLevel);
    return false;

}; //table access

VIS.MRole.canView = function (strAccesLevel) {
    var userLevel = this.vo.UserLevel.trim(); //Format 'SCO'

    var retValue = true;
    if (VIS.Consts.ACCESSLEVEL_All.equals(strAccesLevel)) { retValue = true; }

    //	4 - System data 
    else if (VIS.Consts.ACCESSLEVEL_SystemOnly.equals(strAccesLevel)
        && userLevel.indexOf('S') == -1) { retValue = false; }

    //	2 - Client data requires C
    else if (VIS.Consts.ACCESSLEVEL_ClientOnly.equals(strAccesLevel)
        && userLevel.indexOf('C') == -1) { retValue = false; }

    //	1 - Organization data requires O
    else if (VIS.Consts.ACCESSLEVEL_Organization.equals(strAccesLevel)
        && userLevel.indexOf('O') == -1) { retValue = false; }

    //	3 - Client Shared requires C or O
    else if (VIS.Consts.ACCESSLEVEL_ClientPlusOrganization.equals(strAccesLevel)
        && (!(userLevel.indexOf('C') != -1 || userLevel.indexOf('O') != -1))) { retValue = false; }

    //	6 - System/Client requires S or C
    else if (VIS.Consts.ACCESSLEVEL_SystemPlusClient.equals(strAccesLevel)
        && (!(userLevel.indexOf('S') != -1 || userLevel.indexOf('C') != -1))) { retValue = false; }

    if (retValue) {
        return retValue;
    }
    //  Notification
    VIS.MRole.log.warning("AccessTableNoView",
        "Required=" + strAccesLevel + "("

        + ") != UserLevel=" + userLevel);
    //log.Info(ToString());
    return retValue;
};//can view

VIS.MRole.getIsTableAccess = function (AD_Table_ID, ro) {
    if (!this.getIsTableAccessLevel(AD_Table_ID, ro)) // Role Based Access
        return false;
    var hasAccess = true; // assuming exclusive rule
    var m_tableAccess = this.vo.tableAccess;
    for (var i = 0; i < m_tableAccess.length; i++) {
        if (!VIS.Consts.ACCESSTYPERULE_Accessing
            .equals(m_tableAccess[i].AccessTypeRule))
            continue;
        if (m_tableAccess[i].IsExclude) // Exclude
        // If you Exclude Access to a table and select Read Only,
        // you can only read data (otherwise no access).
        {
            if (m_tableAccess[i].AD_Table_ID === AD_Table_ID) {
                if (ro)
                    hasAccess = m_tableAccess[i].IsReadOnly;
                else
                    hasAccess = false;
                //log.fine("Exclude AD_Table_ID=" + AD_Table_ID + " (ro="
                //        + ro + ",TableAccessRO="
                //        + m_tableAccess[i].isReadOnly() + ") = "
                //        + hasAccess);
                return hasAccess;
            }
        } else // Include
        // If you Include Access to a table and select Read Only,
        // you can only read data (otherwise full access).
        {
            hasAccess = false;
            if (m_tableAccess[i].AD_Table_ID == AD_Table_ID) {
                if (!ro) // rw only if not r/o
                    hasAccess = !m_tableAccess[i].IsReadOnly;
                else
                    hasAccess = true;
                //log.fine("Include AD_Table_ID=" + AD_Table_ID + " (ro="
                //        + ro + ",TableAccessRO="
                //        + m_tableAccess[i].isReadOnly() + ") = "
                //        + hasAccess);
                return hasAccess;
            }
        }
    } // for all Table Access
    if (!hasAccess) {
        //log.fine("AD_Table_ID=" + AD_Table_ID + "(ro=" + ro + ") = "
        //        + hasAccess);
    }
    m_tableAccess = null;
    return hasAccess;
};// isTableAccess

VIS.MRole.getIsCanReport = function (AD_Table_ID) {
    if (!this.vo.IsCanReport) // Role Level block
    {
        //log.warning("Role denied");
        return false;
    }
    if (!this.getIsTableAccess(AD_Table_ID, true)) // No R/O Access to Table
        return false;
    //
    var canReport = true;
    var m_tableAccess = this.vo.tableAccess;
    for (var i = 0; i < m_tableAccess.length; i++) {
        if (!VIS.Consts.ACCESSTYPERULE_Reporting
            .equals(m_tableAccess[i].AccessTypeRule))
            continue;
        if (m_tableAccess[i].IsExclude) // Exclude
        {
            if (m_tableAccess[i].AD_Table_ID === AD_Table_ID) {
                canReport = m_tableAccess[i].IsCanReport;
                //log.fine("Exclude " + AD_Table_ID + " - " + canReport);
                return canReport;
            }
        } else // Include
        {
            canReport = false;
            if (m_tableAccess[i].AD_Table_ID === AD_Table_ID) {
                canReport = m_tableAccess[i].IsCanReport;
                //log.fine("Include " + AD_Table_ID + " - " + canReport);
                return canReport;
            }
        }
    } // for all Table Access
    //log.fine(AD_Table_ID + " - " + canReport);
    m_tableAccess = null;
    return canReport;
}; // isCanReport

VIS.MRole.getIsCanExport = function (AD_Table_ID) {
    if (!this.vo.IsCanExport) // Role Level block
    {
        //log.warning("Role denied");
        return false;
    }
    if (!this.getIsTableAccess(AD_Table_ID, true)) // No R/O Access to Table
        return false;
    if (!this.getIsCanReport(AD_Table_ID)) // We cannot Export if we cannot report
        return false;
    //
    var canExport = true;
    var m_tableAccess = this.vo.tableAccess;
    for (var i = 0; i < m_tableAccess.length; i++) {
        if (!VIS.Consts.ACCESSTYPERULE_Exporting
            .equals(m_tableAccess[i].AccessTypeRule))
            continue;
        if (m_tableAccess[i].IsExclude) // Exclude
        {
            //  added if condition (similar to the if condition
            // in isCanReport)
            if (m_tableAccess[i].AD_Table_ID === AD_Table_ID) {
                canExport = m_tableAccess[i].IsCanExport;
                //log.fine("Exclude " + AD_Table_ID + " - " + canExport);
                return canExport;
            } // end bug 10018373
        } else // Include
        {
            canExport = false;
            //  added if condition (similar to the if condition
            // in isCanReport)
            if (m_tableAccess[i].AD_Table_ID === AD_Table_ID) {
                canExport = m_tableAccess[i].IsCanExport;
                //log.fine("Include " + AD_Table_ID + " - " + canExport);
                return canExport;
            }
        }
    } // for all Table Access
    //log.fine(AD_Table_ID + " - " + canExport);
    m_tableAccess = null;
    return canExport;
}; // isCanExport

VIS.MRole.getIsOrgAccess = function (AD_Org_ID, rw) {
    if (this.vo.IsAccessAllOrgs)
        return true;
    if (AD_Org_ID == 0 && !rw) // can always read common org
        return true;
    // Positive List
    var m_orgAccess = this.vo.orgAccess;
    for (var i = 0; i < m_orgAccess.length; i++) {
        if (m_orgAccess[i].AD_Org_ID === AD_Org_ID) {
            if (!rw)
                return true;
            if (!m_orgAccess[i].readOnly) // rw
                return true;
            return false;
        }
    }
    m_orgAccess = null;
    return false;
}; // isOrgAccess

VIS.MRole.getIsRecordAccess = function (AD_Table_ID, Record_ID, ro) {
    // if (!isTableAccess(AD_Table_ID, ro)) // No Access to Table
    // return false;

    var negativeList = true;
    var m_recordAccess = this.vo.recordAccess;
    var ra = null;
    for (var i = 0; i < m_recordAccess.length; i++) {
        ra = m_recordAccess[i];

        if (ra.AD_Table_ID !== AD_Table_ID)
            continue;

        if (ra.IsExclude) // Exclude
        // If you Exclude Access to a column and select Read Only,
        // you can only read data (otherwise no access).
        {
            if (ra.Record_ID === Record_ID) {
                m_recordAccess = null;
                if (ro)
                    return ra.IsReadOnly;
                else
                    return false;
            }
        } else // Include
        // If you Include Access to a column and select Read Only,
        // you can only read data (otherwise full access).
        {
            negativeList = false; // has to be defined
            if (ra.Record_ID === Record_ID) {
                m_recordAccess = null;
                if (!ro)
                    return !ra.IsReadOnly;
                else
                    // ro
                    return true;
            }
        }
    } // for all Table Access
    return negativeList;
};// isRecordAccess

VIS.MRole.getClientWhere = function (rw) {
    // All Orgs - use Client of Role
    if (this.vo.IsAccessAllOrgs) {
        if (rw || this.vo.AD_Client_ID == 0)
            return "AD_Client_ID=" + this.vo.AD_Client_ID;
        return "AD_Client_ID IN (0," + this.vo.AD_Client_ID + ")";
    }

    // Unique Strings
    var set = [];
    if (!rw)
        set.push("0");
    // Positive List
    for (var ele = 0; ele < this.vo.orgAccess.length; ele++)// OrgAccess element : m_orgAccess)
    {
        var c = this.vo.orgAccess[ele].AD_Client_ID.toString();
        if (set.indexOf(c) < 0) {
            set.push(c);
        }
        c = null;
    }
    //
    var sb = new StringBuilder();
    var oneOnly = true;

    //while (it.hasNext()) {
    for (var i = 0; i < set.length; i++) {
        if (sb.length() > 0) {
            sb.append(",");
            oneOnly = false;
        }
        sb.append(set[i]);
    }
    set = null;
    if (oneOnly) {
        if (sb.length() > 0)
            return "AD_Client_ID=" + sb.toString();
        else {
            this.log.log(VIS.Logging.Level.SEVERE, "No Access Org records");
            return "AD_Client_ID=-1"; // No Access Record
        }
    }
    return "AD_Client_ID IN(" + sb.toString() + ")";
}; //get client where

VIS.MRole.getOrgWhere = function (tableName, rw) {
    if (this.vo.IsAccessAllOrgs)
        return null;
    // Unique Strings
    var set = [];
    if (!rw)
        set.push("0");
    // Positive List
    var orgAccess = this.vo.orgAccess;
    for (var i = 0; i < orgAccess.length; i++) {
        var o = orgAccess[i].AD_Org_ID.toString();
        if (!rw) {
            if (set.indexOf(o) < 0)
                set.push(o);
        }
        else if (!o.readOnly) {
            if (set.indexOf(o) < 0)
                set.push(o);
        }
    }
    orgAccess = null;
    //
    if (set.length == 1) {
        return "COALESCE(" + (tableName == null ? "" : tableName + ".") + "AD_Org_ID,0)=" + set[0];
    } else if (set.length == 0) {
        this.log.log(VIS.Logging.Level.SEVERE, "No Access Org records");
        return (tableName == null ? "" : tableName + ".") + "AD_Org_ID=-1"; // No Access Record
    }

    var sql = new StringBuilder();
    var sb = new StringBuilder();
    var count = 0;
    //while (it.hasNext()) {
    for (var r = 0; r < set.length; r++) {

        if (sb.length() > 0) {
            sb.append(",");
        }
        sb.append(set[r]);
        count++;
        //if there are 999 orgs already, or it reaches the end , reset
        //we do this 'cuz IN() cannot contain more than 1000 values
        if (count % 999 == 0 || count == set.length) {
            if (sql.length() > 0)
                sql.append(" OR ");

            sql.append("COALESCE(" + (tableName == null ? "" : tableName + ".") + "AD_Org_ID,0) IN(").append(sb.toString()).append(")");
            sb = null;
            sb = new StringBuilder();
        }
    }
    return "(" + sql.toString() + ")";
};// getOrgWhereValue

VIS.MRole.getAD_Table_ID = function (tableName) {
    var ii = this.vo.tableName[tableName];
    if (ii != null)
        return ii;
    return 0;
}; //table id

VIS.MRole.getSynonym = function (keyColumn) {
    var o = this.columnSynonym[keyColumn];
    if (o)
        return o;
    return null;
};

VIS.MRole.getKeyColumnName = function (tableInfo, keyColumnName) {
    var columnSyn = this.getSynonym(keyColumnName);
    if (columnSyn == null)
        return keyColumnName;

    //	We have a synonym - ignore it if base table inquired
    for (var i = 0; i < tableInfo.length; i++) {
        var element = tableInfo[i];
        if (keyColumnName.equals("AD_User_ID")) {

            //	List of tables where not to use SalesRep_ID
            if (element.getTableName().equals("AD_User"))
                return keyColumnName;
        }
        else if (keyColumnName.equals("AD_ElementValue_ID")) {
            //	List of tables where not to use Account_ID
            if (element.getTableName().equals("AD_ElementValue"))
                return keyColumnName;
        }
    }	//	tables to be ignored
    return columnSyn;
}; //key columnname 

VIS.MRole.getDependentAccess = function (whereColumnName, includes, excludes, isIncludeNull) {
    if (includes.length == 0 && excludes.length == 0)
        return "";
    if (includes.length != 0 && excludes.length != 0)
        this.log.warning("Mixing Include and Excluse rules - Will not return values");

    var where = new StringBuilder(" AND ");
    if (isIncludeNull)
        where.append(" ( ");
    if (includes.length == 1) {

        where.append(whereColumnName).append("=").append(includes[0]);
    }
    else if (includes.length > 1) {
        where.append(whereColumnName).append(" IN (");
        for (var ii = 0; ii < includes.length; ii++) {
            if (ii > 0)
                where.append(",");
            where.append(includes[ii]);
        }
        where.append(")");
    } else if (excludes.length == 1)
        where.append(whereColumnName).append("<>").append(excludes[0]);
    else if (excludes.length > 1) {
        where.append(whereColumnName).append(" NOT IN (");
        for (var ii = 0; ii < excludes.length; ii++) {
            if (ii > 0)
                where.append(",");
            where.append(excludes[ii]);
        }
        where.append(")");
    }

    if (isIncludeNull)
        where.append(" OR ").append(whereColumnName).append(" IS NULL").append(" ) ");
    return where.toString();
}; // getDependentAccess

VIS.MRole.getDependentRecordWhereColumn = function (mainSql, columnName) {
    var retValue = columnName; // if nothing else found
    var index = mainSql.indexOf(columnName);
    // see if there are table synonym
    var offset = index - 1;
    var c = mainSql.charAt(offset);
    if (c == '.') {
        var sb = new StringBuilder();
        while (c != ' ' && c != ',' && c != '(') // delimeter
        {
            sb.insert(0, c);
            c = mainSql.charAt(--offset);
        }
        sb.append(columnName);
        return sb.toString();
    }
    return retValue;
}; // ge

VIS.MRole.getRecordWhere = function (AD_Table_ID, keyColumnName, rw) {
    //
    var sbInclude = new StringBuilder();
    var sbExclude = new StringBuilder();
    var isIncludeNull = false;
    // Role Access
    var m_recordAccess = this.vo.recordAccess;
    for (var i = 0; i < m_recordAccess.length; i++) {
        if (m_recordAccess[i].AD_Table_ID == AD_Table_ID) {
            // NOT IN (x)
            if (m_recordAccess[i].IsExclude) {
                if (sbExclude.length() == 0)
                    sbExclude.append(keyColumnName).append(" NOT IN (");
                else
                    sbExclude.append(",");
                sbExclude.append(m_recordAccess[i].Record_ID);
            }
            // IN (x)
            else if (!rw || !m_recordAccess[i].IsReadOnly) // include
            {
                if (sbInclude.length() == 0)
                    sbInclude.append(keyColumnName).append(" IN (");
                else
                    sbInclude.append(",");
                sbInclude.append(m_recordAccess[i].Record_ID);
            }
            isIncludeNull = isIncludeNull || m_recordAccess[i].IsIncludeNull;
        }
    } // for all Table Access

    var sb = new StringBuilder();
    if (sbExclude.length() > 0) {
        if (isIncludeNull) {
            sb.append("(");
        }
        sb.append(sbExclude).append(")");
        if (isIncludeNull) {
            sb.append(" OR ").append(keyColumnName).append(" IS NULL )");
        }
        isIncludeNull = false;
    }
    if (sbInclude.length() > 0) {
        if (sb.length() > 0)
            sb.append(" AND ");
        if (isIncludeNull) {
            sb.append("(");
        }
        sb.append(sbInclude).append(")");
        if (isIncludeNull) {
            sb.append(" OR ").append(keyColumnName).append(" IS NULL )");
        }
    }

    // Don't ignore Privacy Access
    if (!this.vo.IsPersonalAccess) {

        if (!this.vo.tableData[AD_Table_ID].IsView && this.vo.tableData[AD_Table_ID].HasKey) {
            var lockedIDs = " NOT IN ( SELECT Record_ID FROM AD_Private_Access WHERE AD_Table_ID = "
                + AD_Table_ID + " AND AD_User_ID <> " + this.vo.AD_User_ID + " AND IsActive = 'Y' )";
            //if (lockedIDs.length > 0) {
            if (sb.length() > 0)
                sb.append(" AND ");
            sb.append(keyColumnName).append(lockedIDs);
            //}
        }
    }
    //
    m_recordAccess = null;
    return sb.toString();
};// getRecordWhere

/**
     * Get Doc Where Clause Value
     * 
     * @param tableName
     *            TableName
     * @return where clause or null (if access all doc)
     */
VIS.MRole.getDocWhere = function (TableName) {

    var baseUrl = VIS.Application.contextUrl;
    var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";


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

    //DataSet String
    function getDataSetJString(data, async, callback) {
        var result = null;
        //data.sql = VIS.secureEngine.encrypt(data.sql);
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



    if (!this.vo.IsUseBPRestrictions)
        return "";

    var hasBPColumn = false;
    var sql = "VIS_101";
    var param = [];
    param[0] = new VIS.DB.SqlParam("@TableName", TableName);

    var ret = executeScalar(sql, param);

    hasBPColumn = (ret ? parseInt(ret) : 0) != 0;

    if (!hasBPColumn)
        return "";

    var AD_User_ID = VIS.context.getAD_User_ID();

    //var docAccess = "(EXISTS (SELECT 1 FROM C_BPartner bp INNER JOIN AD_User u "
    //    + "ON (u.C_BPartner_ID=bp.C_BPartner_ID) "
    //    + " WHERE u.AD_User_ID="
    //    + AD_User_ID
    //    + " AND bp.C_BPartner_ID="
    //    + TableName
    //    + ".C_BPartner_ID)"
    //    + " OR EXISTS (SELECT 1 FROM C_BP_Relation bpr INNER JOIN AD_User u "
    //    + "ON (u.C_BPartner_ID=bpr.C_BPartnerRelation_ID) "
    //    + " WHERE u.AD_User_ID="
    //    + AD_User_ID
    //    + " AND bpr.C_BPartner_ID=" + TableName + ".C_BPartner_ID)";

    //var hasUserColumn = false;
    //var sql1 = "SELECT count(*) FROM AD_Table t "
    //    + "INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID) "
    //    + "WHERE t.tableName='" + TableName
    //    + "' AND c.ColumnName='AD_User_ID' ";

    //ret = executeScalar(sql);
    //hasUserColumn = (ret ? parseInt(ret) : 0) != 0;

    //if (hasUserColumn)
    //    docAccess += " OR " + TableName + ".AD_User_ID =" + AD_User_ID;
    //docAccess += ")";


    //GetDocWhere

    $.ajax({
        type: 'Get',
        async: false,
        url: VIS.Application.contextUrl + "Form/GetDocWhere",
        data: {
            AD_User_ID: AD_User_ID,
            TableName: TableName
        },
        success: function (data) {
            docAccess = data;
        },
        error: function (err) {
            console.log(err);
        }
    });



    return docAccess;
};// getDocWhere

VIS.MRole.addAccessSQL = function (SQL, TableNameIn, fullyQualified, rw, addOrgAccessForAll) {

    if (fullyQualified && VIS.Utility.Util.isEmpty(TableNameIn))
        fullyQualified = false;

    var retSQL = new StringBuilder();

    // Cut off last ORDER BY clause
    var orderBy = "";
    var posOrder = SQL.lastIndexOf(" ORDER BY ");
    if (posOrder != -1) {
        orderBy = SQL.substring(posOrder);
        retSQL.append(SQL.substring(0, posOrder));
    } else
        retSQL.append(SQL);

    // Parse SQL
    var asp = new VIS.AccessSqlParser(retSQL.toString());
    var ti = asp.getTableInfo(asp.getMainSqlIndex());

    // Do we have to add WHERE or AND
    if (asp.getMainSql().indexOf(" WHERE ") == -1)
        retSQL.append(" WHERE ");
    else
        retSQL.append(" AND ");

    // Use First Table
    var tableName = "";
    if (ti.length > 0
        && (ti[0].getTableName().equals(TableNameIn) || ti[0]
            .getSynonym().equals(TableNameIn))) {
        tableName = ti[0].getSynonym();
        if (VIS.Utility.Util.isEmpty(tableName))
            tableName = ti[0].getTableName();
    }
    // Check for error condition
    if (!VIS.Utility.Util.isEmpty(TableNameIn) && VIS.Utility.Util.isEmpty(tableName)) {
        var msg = "TableName not correctly parsed - TableNameIn="
            + TableNameIn + " - " + asp;
        if (ti.length > 0)
            msg += " - #1 " + ti[0];
        msg += "\n = " + SQL;
        this.log.log(VIS.Logging.Level.SEVERE, msg);
        tableName = TableNameIn;
    }

    // Client Access
    if (fullyQualified && !VIS.Utility.Util.isEmpty(tableName))
        retSQL.append(tableName).append(".");
    retSQL.append(this.getClientWhere(rw));

    // Org Access
    if (!this.vo.IsAccessAllOrgs && !addOrgAccessForAll) {
        retSQL.append(" AND ");
        if (fullyQualified && !VIS.Utility.Util.isEmpty(tableName))
            retSQL.append(this.getOrgWhere(tableName, rw));
        else
            retSQL.append(this.getOrgWhere(null, rw));
    }

    if (this.vo.IsUseBPRestrictions) {
        var documentWhere = this.getDocWhere(tableName);
        if (documentWhere.length() > 0) {
            retSQL.append(" AND ");
            retSQL.append(documentWhere);
        }
    }

    // ** Data Access **
    var AD_Table_ID = 0;
    var element;
    for (i = 0; i < ti.length; i++) {
        element = ti[i];
        var TableName = element.getTableName();
        AD_Table_ID = this.getAD_Table_ID(TableName);

        // Org Access
        if (AD_Table_ID != 0 && !this.vo.IsAccessAllOrgs && addOrgAccessForAll) {
            var TableSynonym = element.getSynonym();
            if (TableSynonym == null || TableSynonym.isEmpty())
                TableSynonym = TableName;

            retSQL.append(" AND ");
            retSQL.append(this.getOrgWhere(TableSynonym, rw));
        }

        // Data Table Access
        if (AD_Table_ID != 0 && !this.getIsTableAccess(AD_Table_ID, !rw)) {
            retSQL.append(" AND 1=3"); // prevent access at all
            this.log.fine("No access to AD_Table_ID=" + AD_Table_ID + " - "
                + TableName + " - " + retSQL);
            break; // no need to check further
        }

        // Data Column Access

        // Data Record Access
        var keyColumnName = "";
        if (fullyQualified) {
            keyColumnName = element.getSynonym(); // table synonym
            if (keyColumnName.length == 0)
                keyColumnName = TableName;
            keyColumnName += ".";
        }
        keyColumnName += TableName + "_ID"; // derived from table

        // log.fine("addAccessSQL - " + TableName + "(" + AD_Table_ID + ") "
        // + keyColumnName);
        var recordWhere = this.getRecordWhere(AD_Table_ID, keyColumnName, rw);
        if (recordWhere.length > 0) {
            retSQL.append(" AND ").append(recordWhere);
            this.log.finest("Record access - " + recordWhere);
        }
    } // for all table info
    element = null;

    // Dependent Records (only for main SQL)
    var mainSql = asp.getMainSql();
    AD_Table_ID = 0;
    var whereColumnName = null;
    var includes = [];
    var excludes = [];
    var isIncludeNull = false; // include null value with include exclude list
    //MTable table = MTable.get(getCtx(), TableNameIn);
    var m_recordDependentAccess = this.vo.recordDependentAccess;
    for (i = 0; i < m_recordDependentAccess.length; i++) {
        var columnName = this.getKeyColumnName(asp.getTableInfo(asp.getMainSqlIndex()), m_recordDependentAccess[i].KeyColumnName);
        if (columnName == null)
            continue; // no key column
        var posColumn = mainSql.indexOf(columnName);
        if (posColumn == -1)
            continue;
        // we found the column name - make sure it's a clumn name
        var charCheck = mainSql.charAt(posColumn - 1); // before
        if (!(charCheck == ',' || charCheck == '.' || charCheck == ' ' || charCheck == '('))
            continue;
        charCheck = mainSql.charAt(posColumn + columnName.length); // after
        if (!(charCheck == ',' || charCheck == ' ' || charCheck == ')'))
            continue;

        if (AD_Table_ID != 0
            && AD_Table_ID != m_recordDependentAccess[i].AD_Table_ID)
            retSQL.append(this.getDependentAccess(whereColumnName, includes,
                excludes, isIncludeNull));

        AD_Table_ID = m_recordDependentAccess[i].AD_Table_ID;
        // *** we found the column in the main query
        if (m_recordDependentAccess[i].IsExclude) {
            excludes.push(m_recordDependentAccess[i].Record_ID);
            this.log.fine("Exclude " + columnName + " - "
                + m_recordDependentAccess[i]);
        } else if (!rw || !m_recordDependentAccess[i].IsReadOnly) {
            includes.push(m_recordDependentAccess[i].Record_ID);
            //log.fine("Include " + columnName + " - "
            //        + m_recordDependentAccess[i]);
        }
        isIncludeNull = isIncludeNull || m_recordDependentAccess[i].IsIncludeNull;

        var column = null;// table.getColumn(columnName);
        if (column != null) {
            var columnSQL = "";//column.getColumnSQL();
            if (columnSQL == null || columnSQL.length() == 0)
                whereColumnName = this.getDependentRecordWhereColumn(mainSql, columnName);
            else
                whereColumnName = columnSQL;
        } else
            whereColumnName = this.getDependentRecordWhereColumn(mainSql,
                columnName);
    } // for all dependent records

    retSQL.append(this.getDependentAccess(whereColumnName, includes, excludes, isIncludeNull));
    //
    retSQL.append(orderBy);
    // console.log(retSQL.toString());
    //log.finest(retSQL.toString());
    return retSQL.toString();
};

VIS.MRole.getWindowAccess = function (AD_Window_ID) {
    if (this.vo.windowAccess[AD_Window_ID])
        return true;
    else return false;
}; //get window access

VIS.MRole.getFormAccess = function (AD_Form_ID) {
    if (this.vo.formAccess[AD_Form_ID])
        return true;
    else return false;
}; //get form access

VIS.MRole.getProcessAccess = function (AD_Process_ID) {
    if (this.vo.processAccess[AD_Process_ID])
        return true;
    else return false;
}; //get process access

VIS.MRole.getIsClientAccess = function (AD_Client_ID, rw) {
    if (AD_Client_ID == 0 && !rw) // can always read System
        return true;
    // Positive List
    var m_orgAccess = this.vo.orgAccess;
    for (var i = 0; i < m_orgAccess.length; i++) {
        if (m_orgAccess[i].AD_Client_ID == AD_Client_ID) {
            if (!rw)
                return true;
            if (!m_orgAccess[i].readOnly) // rw
                return true;
        }
    }
    return false;
};// isClientAccess

VIS.MRole.getIsShowPreference = function () {
    return this.vo.IsShowPreference;
};

VIS.MRole.getName = function () {
    return this.vo.Name;
};

VIS.MRole.getPreferenceType = function () {
    return this.vo.PreferenceType;
};

VIS.MRole.getIsDisplayClient = function () {
    return this.vo.IsDisplayClient;
};

VIS.MRole.getIsDisplayOrg = function () {
    return this.vo.IsDisplayOrg;
};

VIS.MRole.getIsPersonalLock = function () {
    return this.vo.IsPersonalLock;
};

VIS.MRole.getMaxQueryRecords = function () {
    return this.vo.MaxQueryRecords;//TODO
};

VIS.MRole.getIsQueryMax = function (noRecords) {
    var max = this.getMaxQueryRecords();
    return max > 0 && noRecords > max;
};

VIS.MRole.getIsShowAcct = function () {
    return this.vo.IsShowAcct;
};

VIS.MRole.getIsOverwritePriceLimit = function () {
    return this.vo.IsOverwritePriceLimit;
};

VIS.MRole.getIsOverrideReturnPolicy = function () {
    return this.vo.IsOverrideReturnPolicy;
};

VIS.MRole.getIsDisableMenu = function () {
    return this.vo.IsDisableMenu;
};
VIS.MRole.getHomePage = function () {
    return this.vo.HomePage;
};


VIS.MRole.toStringX = function (ctx) {
    var sb = new StringBuilder();
    sb.append(VIS.Msg.translate(ctx, "AD_Role_ID")).append("=").append(
        this.getName()).append(" - ").append(
            VIS.Msg.translate(ctx, "IsCanExport")).append("=").append(
                this.getIsCanExport()).append(" - ").append(
                    VIS.Msg.translate(ctx, "IsCanReport")).append("=").append(
                        this.getIsCanReport()).append(VIS.Env.NL).append(VIS.Env.NL);
    return sb.toString();
};
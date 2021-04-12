; (function (VIS, $) {

    // Updated un initialized user and client id, table name by vinay bhatt
    function ProcessInfo(strTitle, AD_Process_ID, Table_ID, Record_ID) {
        this.title = strTitle;
        this.AD_Process_ID = AD_Process_ID;
        this.table_ID = Table_ID;
        this.tableName = "";
        this.record_ID = Record_ID;
        this.AD_User_ID = 0;
        this.AD_Client_ID = 0;
        this.className = null;

        this.AD_PInstance_ID = 0;
        this.summary = "";
        this.error = false;
        this.batch = false;
        //Process timed out		
        this.timeout = false;
        //private List<ProcessInfoLog> m_logs = null;
        this.parameter = null;
        this.AD_PrintFormat_Table_ID = 0;
        this.AD_PrintFormat_ID = 0;
        this.isReportFormat = false;
        this.isCrystal = false;
        this.totalrecords = 0;
        this.isTelerikReport = false;
        this.supportPaging = false;
        this.dynamicAction = "";
        this.pageSize = VIS.context.ctx["#REPORT_PAGE_SIZE"];
        this.totalPage = 1;
        this.pageNo = 1;
        this.fileType = VIS.ProcessCtl.prototype.REPORT_TYPE_PDF;
        this.windowNo = 0;
        this.tableName = "";
        this.AD_Window_ID = 0;
        this.AD_ReportView_ID = 0;
        this.printFormatTableName = "";
        this.useCrystalReportViewer = false;
        this.isReport = false;
        this.isBackground = false;
        this.ActionOrigin = "";
        this.OriginName = "";
        this.isBiHtml = false;
    };

    ProcessInfo.prototype.toJson = function () {

        var o = {
            "Title": this.title,
            "Process_ID": this.AD_Process_ID,
            "AD_PInstance_ID": this.AD_PInstance_ID,
            "Record_ID": this.record_ID,
            "Error": this.error,
            "Summary": this.getSummary(),
            "ClassName": this.className,

            "AD_Table_ID": this.table_ID,
            "AD_TableName": this.tableName,
            "AD_User_ID": this.AD_User_ID,
            "AD_Client_ID": this.AD_Client_ID,
            "Batch": this.batch,
            "TimeOut": this.timeout,
            "AD_PrintFormat_Table_ID": this.AD_PrintFormat_Table_ID,
            "PrintFormatTableName": this.printFormatTableName,
            "AD_PrintFormat_ID": this.AD_PrintFormat_ID,
            "SupportPaging": this.supportPaging,
            "DynamicAction": this.dynamicAction,
            "PageSize": this.pageSize,
            "TotalPage": this.totalPage,
            "PageNo": this.pageNo,
            "FileType": this.fileType,
            "AD_Window_ID": this.AD_Window_ID,
            "WindowNo": this.windowNo,
            "AD_ReportView_ID": this.AD_ReportView_ID,
            "UseCrystalReportViewer": this.useCrystalReportViewer,
            "IsReport": this.isReport,
            "IsBackground": this.isBackground,
            "ActionOrigin": this.ActionOrigin,
            "OriginName":this.OriginName
        }
        return o;

    };

    ProcessInfo.prototype.fromJson = function (o) {
        var info = null;
        if (this instanceof ProcessInfo) {
            info = this;
        }
        else {
            info = new ProcessInfo(o.Title, o.Process_ID);
        }
        if (o && o.length > 0) {
            createPIFromJson(info, o);
        }
        return info;
    };

    function createPIFromJson(info, o) {
        info.className = o.ClassName;
        info.record_ID = o.Record_ID;
        info.error = o.Error;
        info.AD_PInstance_ID = o.AD_PInstance_ID;
        info.summary = o.Summary;
        info.table_ID = o.AD_Table_ID;
        info.tableName = o.AD_TableName;
        info.AD_User_ID = o.AD_User_ID;
        info.AD_Client_ID = o.AD_Client_ID;;
        info.batch = o.Batch;
        info.timeout = o.TimeOut;
        info.AD_PrintFormat_Table_ID = o.AD_PrintFormat_Table_ID;
        info.printFormatTableName = o.PrintFormatTableName;
        info.AD_PrintFormat_ID = o.AD_PrintFormat_ID;
        info.isCrystal = o.IsCrystal;
        info.isReportFormat = o.IsReportFormat;
        info.totalrecords = parseInt(o.TotalRecords);
        info.isTelerikReport = o.IsTelerikReport;
        info.supportPaging = o.SupportPaging;
        info.dynamicAction = o.DynamicAction;
        info.pageSize = o.PageSize;
        info.title = o.Title;
        info.AD_Process_ID = o.Process_ID;
        info.totalPage = o.TotalPage;
        info.pageNo = o.PageNo;
        info.windowNo = o.WindowNo;
        info.AD_ReportView_ID = o.AD_ReportView_ID;
        info.isJasperReport = o.IsJasperReport;
        info.useCrystalReportViewer = o.UseCrystalReportViewer;
        info.isReport = o.IsReport;
        info.isBackground = o.IsBackground;
        info.ActionOrigin = o.ActionOrigin;
        info.OriginName = o.OriginName;
    };

    ProcessInfo.prototype.setPrintFormatTableName = function (tableName) {
        this.printFormatTableName = tableName;
    };

    ProcessInfo.prototype.getPrintFormatTableName = function () {
        return this.printFormatTableName;
    };


    ProcessInfo.prototype.setSupportPaging = function (sPaging) {
        this.supportPaging = sPaging;
    };

    ProcessInfo.prototype.getSupportPaging = function () {
        return this.supportPaging;
    };

    ProcessInfo.prototype.setTotalPages = function (tpages) {
        this.totalPage = tpages;
    };

    ProcessInfo.prototype.getTotalPages = function () {
        return this.totalPage;
    };

    ProcessInfo.prototype.setPageNo = function (pNo) {
        this.pageNo = pNo;
    };

    ProcessInfo.prototype.getPageNo = function () {
        return this.pageNo;
    };

    ProcessInfo.prototype.setPageSize = function (pageSize) {
        this.pageSize = pageSize;
    };

    ProcessInfo.prototype.getPageSize = function () {
        return this.pageSize;
    };

    ProcessInfo.prototype.setError = function (error) {
        this.error = error;
    };

    ProcessInfo.prototype.getIsError = function () {
        return this.error;
    };

    ProcessInfo.prototype.getIsBatch = function () {
        return this.batch;
    };

    ProcessInfo.prototype.setIsBatch = function (batch) {
        this.batch = batch;
    };

    ProcessInfo.prototype.getAD_PInstance_ID = function () {
        return this.AD_PInstance_ID;
    };

    ProcessInfo.prototype.setAD_PInstance_ID = function (AD_PInstance_ID) {
        this.AD_PInstance_ID = AD_PInstance_ID;
    };

    ProcessInfo.prototype.getAD_Process_ID = function () {
        return this.AD_Process_ID;
    };

    ProcessInfo.prototype.setAD_Process_ID = function (AD_Process_ID) {
        this.AD_Process_ID = AD_Process_ID;
    };

    ProcessInfo.prototype.getClassName = function () {
        return this.className;
    };

    ProcessInfo.prototype.setClassName = function (className) {
        this.className = className;
        if (className != null && className.length == 0)
            this.className = null;
    };

    ProcessInfo.prototype.getTable_ID = function () {
        return this.table_ID;
    };

    ProcessInfo.prototype.setTable_ID = function (AD_Table_ID) {
        this.table_ID = AD_Table_ID;
    };

    ProcessInfo.prototype.setTable_Name = function (tableName) {
        this.tableName = tableName;
    };

    ProcessInfo.prototype.getTable_Name = function () {
        return this.tableName;
    };

    ProcessInfo.prototype.setPrintFormatTableName = function (tableName) {
        this.printFormatTableName = tableName;
    };

    ProcessInfo.prototype.getPrintFormatTableName = function () {
        return this.printFormatTableName;
    };


    ProcessInfo.prototype.getRecord_ID = function () {
        return this.record_ID;
    };

    ProcessInfo.prototype.setRecord_ID = function (Record_ID) {
        this.record_ID = Record_ID;
    };

    ProcessInfo.prototype.getTitle = function () {
        return this.title;
    };

    ProcessInfo.prototype.setTitle = function (title) {
        this.title = title;
    };

    ProcessInfo.prototype.setAD_Client_ID = function (AD_Client_ID) {
        this.AD_Client_ID = AD_Client_ID;
    };

    ProcessInfo.prototype.getAD_Client_ID = function () {
        return this.AD_Client_ID;
    };

    ProcessInfo.prototype.setAD_User_ID = function (AD_User_ID) {
        this.AD_User_ID = AD_User_ID;
    };

    ProcessInfo.prototype.getAD_User_ID = function () {
        return this.AD_User_ID;
    };

    ProcessInfo.prototype.getParameter = function () {
        return this.parameter;
    };

    ProcessInfo.prototype.setParameter = function (parameter) {
        this.parameter = parameter;
    };

    ProcessInfo.prototype.setIsTimeout = function (timeout) {
        this.timeout = timeout;
    };

    ProcessInfo.prototype.getIsTimeout = function () {
        return this.timeout;
    };

    ProcessInfo.prototype.set_AD_PrintFormat_Table_ID = function (AD_PrintFormat_Table_ID) {
        this.AD_PrintFormat_Table_ID = AD_PrintFormat_Table_ID;
    };

    ProcessInfo.prototype.get_AD_PrintFormat_Table_ID = function () {
        return this.AD_PrintFormat_Table_ID;
    };

    ProcessInfo.prototype.set_AD_PrintFormat_ID = function (AD_PrintFormat_ID) {
        this.AD_PrintFormat_ID = AD_PrintFormat_ID;
    };

    ProcessInfo.prototype.Get_AD_PrintFormat_ID = function () {
        return this.AD_PrintFormat_ID;
    };

    ProcessInfo.prototype.setIsCrystal = function (isCrystal) {
        this.isCrystal = isCrystal;
    };

    ProcessInfo.prototype.getIsCrystal = function () {
        return this.isCrystal;
    };

    ProcessInfo.prototype.setIsTelerikReport = function (isTelerikReport) {
        this.isTelerikReport = isTelerikReport;
    };

    ProcessInfo.prototype.getIsTelerikReport = function () {
        return this.isTelerikReport;
    };

    ProcessInfo.prototype.setIsReportFormat = function (isRF) {
        this.isReportFormat = isRF;
    };

    ProcessInfo.prototype.getIsReportFormat = function () {
        return this.isReportFormat;
    };

    ProcessInfo.prototype.getTotalRecord = function () {
        return this.totalrecords;
    };

    ProcessInfo.prototype.getFileType = function () {
        return this.fileType;
    };

    ProcessInfo.prototype.setFileType = function (fileType) {
        this.fileType = fileType;
    };

    ProcessInfo.prototype.setWindowNo = function (windowNo) {
        this.windowNo = windowNo;
    };


    ProcessInfo.prototype.getWindowNo = function () {
        return this.windowNo;
    };


    ProcessInfo.prototype.setUseCrystalReportViewer = function (ucrv) {
        this.useCrystalReportViewer = ucrv;
    };


    ProcessInfo.prototype.getUseCrystalReportViewer = function () {
        return this.useCrystalReportViewer;
    };


    ProcessInfo.prototype.setIsReport = function (isRep) {
        this.isReport = isRep;
    };

    ProcessInfo.prototype.getIsReport = function () {
        return this.isReport;
    };




    ProcessInfo.prototype.GetAD_ReportView_ID = function () {
        return this.AD_ReportView_ID;
    };

    ProcessInfo.prototype.SetAD_ReportView_ID = function (AD_ReportView_ID) {
        this.AD_ReportView_ID = AD_ReportView_ID;
    };

    ProcessInfo.prototype.setIsJasperReport = function (isJasperReport) {
        this.isJasperReport = isJasperReport;
    }
    ProcessInfo.prototype.getIsJasperReport = function () {
        return this.isJasperReport;
    }


    ProcessInfo.prototype.setAD_Window_ID = function (Window_ID) {
        this.AD_Window_ID = Window_ID;
    }
    ProcessInfo.prototype.getAD_Window_ID = function () {
        return this.AD_Window_ID;
    }

    ProcessInfo.prototype.setIsBackground = function (Background) {
        this.isBackground = Background;
    }

    ProcessInfo.prototype.getIsBackground = function () {
        return this.isBackground;
    }

    ProcessInfo.prototype.getActionOrigin = function () {
        return this.ActionOrigin;
    }

    ProcessInfo.prototype.setActionOrigin = function (ActionOrigin) {
        this.ActionOrigin = ActionOrigin;
    }

    ProcessInfo.prototype.getOriginName = function () {
        return this.OriginName;
    }

    ProcessInfo.prototype.setOriginName = function (OriginName) {
        this.OriginName = OriginName;
    }

    ProcessInfo.prototype.setIsBiHtml = function (isRF) {
        this.isBiHtml = isRF;
    };

    ProcessInfo.prototype.getIsBiHtml = function () {
        return this.isBiHtml;
    };

    ProcessInfo.prototype.setSummary = function (summary, error) {

        if (arguments.length == 2) {
            this.setError(error)
        }

        this.summary = summary;
    };

    ProcessInfo.prototype.getSummary = function () {
        return this.summary;
        //	return Util.cleanMnemonic(m_Summary);
    };

    // Change Lokesh Chauhan
    ProcessInfo.prototype.setCustomHTML = function (HTML) {
        this.customHTML = HTML;
    }
    ProcessInfo.prototype.getCustomHTML = function () {
        return this.customHTML;
    }

    /**************************************************************************
	 * 	Add to Log
	 *	@param Log_ID Log ID
	 *	@param P_ID Process ID
	 *	@param P_Date Process Date
	 *	@param P_Number Process Number
	 *	@param P_Msg Process Message
	 */
    ProcessInfo.prototype.addLog = function (Log_ID, P_ID, P_Date, P_Number, P_Msg) {
        return this.addLogEntry(new ProcessInfoLog(Log_ID, P_ID, P_Date, P_Number, P_Msg));
    };	//	

    /**
	 * 	Add to Log.
	 * 	Checks for duplicates;
	 *	@param logEntry log entry
	 */
    ProcessInfo.prototype.addLogEntry = function (logEntry) {
        if (logEntry == null)
            return null;
        if (this.logs == null)
            this.logs = [];
        //
        var newID = logEntry.getLog_ID();
        for (var i = 0; i < this.logs.length; i++) {
            var thisEntry = this.logs[i];
            var thisID = thisEntry.getLog_ID();
            if (newID == thisID)
                return thisEntry;		//	already exists
        }
        this.logs.push(logEntry);
        return logEntry;
    };	//	addLog


    /**
	 *	Set Log of Process.
	 *  <pre>
	 *  - Translated Process Message
	 *  - List of log entries
	 *      Date - Number - Msg
	 *  </pre>
	 *	@param html if true with HTML markup
	 *	@return Log Info
	 */
    ProcessInfo.prototype.getLogInfo = function (html) {
        if (this.logs == null)
            return "";
        //
        var sb = new StringBuilder();
        //SimpleDateFormat dateTimeFormat = DisplayType.getDateFormat(DisplayTypeConstants.DateTime);
        //SimpleDateFormat dateFormat = DisplayType.getDateFormat(DisplayTypeConstants.Date);
        if (html)
            sb.append("<div class='vis-process-result-table'><table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"2\">");
        //
        //	boolean hasIDCol = false;
        var hasDateCol = false;
        var hasNoCol = false;
        var hasMsgCol = false;
        for (var i = 0; i < this.logs.length; i++) {
            var log = this.logs[i];
            //	if (log.getP_ID() != 0)
            //		hasIDCol = true;
            if (log.getP_Date() != null)
                hasDateCol = true;
            if (log.getP_Number() != null)
                hasNoCol = true;
            if (log.getP_Msg() != null)
                hasMsgCol = true;
        }

        for (i = 0; i < this.logs.length; i++) {
            if (html)
                sb.append("<tr>");
            else if (i > 0)
                sb.append("\n");
            //
            var log = this.logs[i];
            /**
            if (log.getP_ID() != 0)
                sb.append(html ? "<td>" : "")
                    .append(log.getP_ID())
                    .append(html ? "</td>" : " \t");	**/
            //
            if (log.getP_Date() != null) {
                sb.append(html ? "<td>" : "");
                var ts = log.getP_Date();
                //if (TimeUtil.isDay(ts))
                //  sb.append(dateFormat.format(ts));
                // else
                //  sb.append(dateTimeFormat.format(ts));
                sb.append(html ? "</td>" : " \t");
            }
            else if (hasDateCol)
                sb.append(html ? "<td>&nbsp;</td>" : " \t");
            //
            if (log.getP_Number() != null) {
                sb.append(html ? "<td>" : "")
                    .append(log.getP_Number())
                    .append(html ? "</td>" : " \t");
            }
            else if (hasNoCol)
                sb.append(html ? "<td>&nbsp;</td>" : " \t");
            //
            if (log.getP_Msg() != null) {
                sb.append(html ? "<td>" : "")
                    .append(VIS.Msg.parseTranslation(VIS.Env.getCtx(), log.getP_Msg()))
                    .append(html ? "</td>" : "");
            }
            else if (hasMsgCol)
                sb.append(html ? "<td>&nbsp;</td>" : "");
            //
            if (html)
                sb.append("</tr>");
        }
        if (html)
            sb.append("</table></div>");
        return sb.toString();
    }	//	getLogInfo

    ProcessInfo.prototype.dispose = function () {
        this.title = null;
        this.AD_Process_ID = null;
        this.table_ID = null;
        this.tableName = null;
        this.record_ID = null;
        this.AD_User_ID = null;
        this.AD_Client_ID == null;
        this.className = null;

        this.AD_PInstance_ID = null;
        this.summary = null;
        this.error = null;
        this.batch = null;
        //Process timed out		
        this.timeout = null;
        //private List<ProcessInfoLog> m_logs = null;
        this.parameter = null;
        this.AD_PrintFormat_Table_ID = null;
        this.AD_PrintFormat_ID = null;
        this.isReportFormat = null;
        this.isCrystal = null;
        this.totalrecords = null;
        this.isJasperReport = null;

        // Change Lokesh Chauhan
        this.customHTML = null;
    };

    var baseUrl = VIS.Application.contextUrl;
    var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";
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

    /*****************************************************/
    /********       Process Info Util            ********/
    /***************************************************/

    /**
 * 	Process Info with Utilities
 *
 */
    VIS.ProcessInfoUtil = {

        /**
	 *	Set Log of Process.
	 * 	@param pi process info
	 */
        setLogFromDB: function (pi) {
            //	s_log.fine("setLogFromDB - AD_PInstance_ID=" + pi.getAD_PInstance_ID());
            var sql = "VIS_144";
            var param = [];
            param[0] = new VIS.DB.SqlParam("@AD_PInstance_ID", pi.getAD_PInstance_ID());

            var dr = null;
            try {

                dr = executeReader(sql, param);

                while (dr.read())
                    //	int Log_ID, int P_ID, Timestamp P_Date, BigDecimal P_Number, String P_Msg
                    pi.addLog(dr.getInt(0), dr.getInt(1), dr.getDateTime(2), dr.getDecimal(3), dr.getString(4));

                //dr.dispose();
            }
            catch (e) {
                console.log(e);
            }
            finally {
                if (dr)
                    dr.dispose();
            }
        }	//	getLogFromDB

    };


    /*****************************************************/
    /********               End                  ********/
    /***************************************************/



    /**
     * 	Process Info Log (VO)
     */
    function ProcessInfoLog(Log_ID, P_ID, P_Date, P_Number, P_Msg) {
        this.P_ID;
        this.P_Date;
        this.P_Number;
        this.P_Msg;

        if (Log_ID) {
            this.setLog_ID(Log_ID);
        }
        else
            this.setLog_ID(this.Log_ID++);

        this.setP_ID(P_ID);
        this.setP_Date(P_Date);
        this.setP_Number(P_Number);
        this.setP_Msg(P_Msg);
    };
    ProcessInfoLog.prototype.Log_ID = 0;

    /* Get Log_ID
    * @return id
    */
    ProcessInfoLog.prototype.getLog_ID = function () {
        return this.Log_ID;
    };

    /**
     * 	Set Log_ID
     *	@param Log_ID id
     */
    ProcessInfoLog.prototype.setLog_ID = function (Log_ID) {
        this.Log_ID = Log_ID;
    };

    /**
     * Method getP_ID
     * @return int
     */
    ProcessInfoLog.prototype.getP_ID = function () {
        return this.P_ID;
    };
    /**
     * Method setP_ID
     * @param P_ID int
     */
    ProcessInfoLog.prototype.setP_ID = function (P_ID) {
        this.P_ID = P_ID;
    };

    /**
     * Method getP_Date
     * @return Timestamp
     */
    ProcessInfoLog.prototype.getP_Date = function () {
        return this.P_Date;
    };
    /**
     * Method setP_Date
     * @param P_Date Timestamp
     */
    ProcessInfoLog.prototype.setP_Date = function (P_Date) {
        this.P_Date = P_Date;
    };

    /**
     * Method getP_Number
     * @return BigDecimal
     */
    ProcessInfoLog.prototype.getP_Number = function () {
        return this.P_Number;
    };
    /**
     * Method setP_Number
     * @param P_Number BigDecimal
     */
    ProcessInfoLog.prototype.setP_Number = function (P_Number) {
        this.P_Number = P_Number;
    };

    /**
     * Method getP_Msg
     * @return String
     */
    ProcessInfoLog.prototype.getP_Msg = function () {
        return this.P_Msg;
    };
    /**
     * Method setP_Msg
     * @param P_Msg String
     */
    ProcessInfoLog.prototype.setP_Msg = function (P_Msg) {
        this.P_Msg = P_Msg;
    };


    //	ProcessInfoLog

    VIS.ProcessInfo = ProcessInfo;
})(VIS, jQuery);
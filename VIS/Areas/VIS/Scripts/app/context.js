/**
 *	Typed Context
 *  - has logined data and app setting
 *
 */

VIS.context.m_map = {}; //window's context


; VIS.context.getContext = function (context) {
    if (context == null)
        throw new ArgumentException("Require Context");
    if (arguments.length > 2) {
        if (typeof (arguments[1]) == "number") {
            return this.getWindowTabContext(arguments[0], arguments[1], arguments[2]);
        }
        return this.getWindowContext(arguments[0], arguments[1], arguments[2]);
    }
    else if (arguments.length == 2) {
        return this.getWindowContext(arguments[0], arguments[1]);
    }


    var value = VIS.context.ctx[context];
    //m_map[context];
    if (!value) {
        if (context == "#AD_User_ID")
            return VIS.context.getContext("#" + context);
        if (context == "#AD_User_Name")
            return VIS.context.getContext("#" + context);
        return "";
    }
    return value;
};

VIS.context.getWindowContext = function (windowNo, context, onlyWindow,val2) {
    if (context == null)
        throw new ArgumentException("Require Context");
    if (typeof (arguments[1]) == "number" && arguments.length > 2) {
        return VIS.context.getTabRecordContext(arguments[0], arguments[1], arguments[2],val2);
    }

    var tabNo = "";
    if (typeof (context) == "number") {
        tabNo = "-" + context;
        context = onlyWindow;
        onlyWindow = val2;
    }

    var key = windowNo + tabNo + "|" + context;

    var tabNo = "";
    if (typeof (context) == "number") {
        tabNo = "-" + context;
        context = onlyWindow;
        onlyWindow = val2;
    }

    var key = windowNo + tabNo + "|" + context;

    var value = "";
    if (this.m_map[windowNo]) {
        value = this.m_map[windowNo][key];
    }

    if (!value || value == "") {
        //	Explicit Base Values
        if (context.startsWith("#") || context.startsWith("$"))
            return VIS.context.getContext(context);
        if (onlyWindow)			//	no Default values
            return "";
        return VIS.context.getContext("#" + context);
    }
    return value;
};

VIS.context.getWindowTabContext = function (windowNo, tabNo, context) {
    if (context == null)
        throw new ArgumentException("Require Context");
    //check windowNo exist in map or not
    var value = "";
    if (this.m_map[windowNo]) {
        value = this.m_map[windowNo][windowNo + "|" + tabNo + "|" + context];
    }
    //m_map.TryGetValue(windowNo + "|" + tabNo + "|" + context,out value);
    if (!value || value == "")
        return VIS.context.getWindowContext(windowNo, context, false);
    return value;
};

VIS.context.getTabRecordContext = function (windowNo, tabNo, context,onlyWindow) {
    if (context == null)
        throw new ArgumentException("Require Context");
    //check windowNo exist in map or not
    var value = "";
    if (this.m_map[windowNo]) {
        value = this.m_map[windowNo][windowNo + "-" + tabNo + "|" + context];
    }
    //m_map.TryGetValue(windowNo + "|" + tabNo + "|" + context,out value);
    if (!value || value == "")
        return VIS.context.getWindowContext(windowNo, context, onlyWindow);
    return value;
};

VIS.context.setTabRecordContext = function (windowNo, tabNo,context, value) {
    if (context == null) {
        return;
    }

    if (!this.m_map[windowNo])
        this.m_map[windowNo] = {};

    if (value == null || value === "")
        this.m_map[windowNo][windowNo +'-'+ tabNo + "|" + context] = null;
    else
        this.m_map[windowNo][windowNo +'-'+ tabNo + "|" + context] = value;
};

VIS.context.setContext = function (key, value) {
    if (arguments.length == 4) {
        this.setWindowTabContext(arguments[0], arguments[1], arguments[2], arguments[3]);
    }
    else if (arguments.length == 3) {
        this.setWindowContext(arguments[0], arguments[1], arguments[2]);
    }
    else {
        VIS.context.ctx[key] = value;
    }
    return key;
};

VIS.context.setWindowContext = function (windowNo, context, value,val2) {
    if (context == null) {
        return;
    }

    if (!this.m_map[windowNo])
        this.m_map[windowNo] = {};
    var tabNo = "";
    if (typeof (context) == "number") {
        tabNo = "-" + context;
        context = value;
        value = val2;
    }

    if (value == null || value === "")
        this.m_map[windowNo][windowNo +tabNo+ "|" + context] = null;
    else
        this.m_map[windowNo][windowNo + tabNo+"|" + context] = value;
};

VIS.context.setWindowTabContext = function (windowNo, tabNo, context, value) {
    if (context == null)
        return;
    if (!this.m_map[windowNo])
        this.m_map[windowNo] = {};
    if (value == null || String(value).equals(""))
        this.m_map[windowNo][windowNo + "|" + tabNo + "|" + context] = null;
        //	m_map.remove(WindowNo+"|"+TabNo+"|"+context);
    else
        this.m_map[windowNo][windowNo + "|" + tabNo + "|" + context] = value;
};


/**
	 * Clean up context for Window Tab (i.e. delete it).
	 * Please note that this method is not clearing the tab info context (i.e. _TabInfo).
	 * @param ctx context
	 * @param WindowNo window
	 * @param TabNo tab
	 */
VIS.context.clearTabContext = function (windowNo, tabNo) {

    var wCtx = this.m_map[windowNo];

    for (var prop in wCtx) {
        if (prop.startsWith(windowNo + "-" + tabNo + "|"))
            delete wCtx[prop];
    }
};


VIS.context.getAD_User_ID = function () {
    return VIS.context.getContext("##AD_User_ID");
};

VIS.context.getAD_User_Name = function () {
    return this.getContext("#AD_User_Name");
};

VIS.context.getAD_Role_ID = function () {
    return this.getContextAsInt("#AD_Role_ID");
}

VIS.context.getAD_Role_Name = function () {
    return this.getContext("#AD_Role_Name");
};

VIS.context.getAD_Client_ID = function () {
    return VIS.context.getContext("#AD_Client_ID");
};

VIS.context.getAD_Client_Name = function () {
    return this.getContext("#AD_Client_Name");
};

VIS.context.getAD_Org_ID = function () {
    return VIS.context.getContext("#AD_Org_ID");
};

VIS.context.getAD_Language = function () {
    return VIS.context.getContext('#AD_Language');
};

VIS.context.setAutoCommit = function (windowNo, autoCommit) {
    VIS.context.setWindowContext(windowNo, "AutoCommit", autoCommit ? "Y" : "N");
};

VIS.context.setAutoNew = function (windowNo, autoNew) {
    VIS.context.setWindowContext(windowNo, "AutoNew", autoNew ? "Y" : "N");
};

VIS.context.isAutoNew = function () {
    var s = VIS.context.getContext("AutoNew");
    if (s != null && s === "Y")
        return true;
    return false;
};

VIS.context.getStdPrecision = function () {
    return VIS.context.getContext('#StdPrecision');
};

VIS.context.setStdPrecision = function () {
    return VIS.context.getContext('#StdPrecision');
};

/**
	 *	Is Window Auto New Record (if not set use default)
	 *  @param WindowNo window no
	 *  @return true if auto new record
	 */
VIS.context.getIsAutoNew = function (WindowNo) {
    var s = this.getWindowContext(WindowNo, "AutoNew", false);
    if (s != null && s != "") {
        if (s.equals("Y"))
            return true;
        else
            return false;
    }
    return this.isAutoNew();
};	//	i

VIS.context.isAutoCommit = function () {
    var s = VIS.context.getContext("AutoCommit");
    if (s != null && s === "Y")
        return true;
    return false;
};

VIS.context.setContextOfWindow = function (ctxArray, winodowNo) {
    if (ctxArray && winodowNo) {
        for (prop in ctxArray) {
            VIS.context.setWindowContext(winodowNo, prop, ctxArray[prop]);
        }
        ctxArray = null;
    }
};

/**
 * 
 * @param {any} windowNo
 * @param {any} context
 * @param {any} onlyWindow
 */
VIS.context.getContextAsInt = function (windowNo, context, onlyWindow) {

    var s = "";
    if (arguments.length > 1 && typeof (arguments[1] )== "number")
        s = this.getWindowTabContext(arguments[0], arguments[1], arguments[2]);
    else if (arguments.length > 1 && typeof (arguments[0]) == "number")
        s = this.getWindowContext(windowNo, context, onlyWindow);
    else {
        s = this.getContext(arguments[0]);
    }

    if (s.length == 0)
        return 0;
    //
    try {
        return parseInt(s);
    }
    catch (e) {
    }
    return 0;
};	//

	/**
	 *	Get Context and convert it to an integer (0 if error)
	 *  @param windowNo window no
	 *  @param tabNo tab no
	 * 	@param context context key
	 *  @return value or 0
//	 */
//VIS.context.getContextAsInt = function (windowNo, TabNo, context) {
//    var s = this.getContext(windowNo, TabNo, context);
//    if (!s || s == "" || isNaN(s))
//        return 0;
//    return parseInt(s);
//};

VIS.context.getWindowContextAsInt = function (windowNo, context, onlyWindow) {

    var s = this.getWindowContext(windowNo, context, onlyWindow);

    if (s.length == 0)
        return 0;
    try {
        return parseInt(s);
    }
    catch (e) {
    }
    return 0;
};

VIS.context.getContextAsTime = function (windowNo, context) {
    var s = this.getContext(windowNo, context, false);
    if (s == null || s.length == 0) {

        return Date.now().getTime();
    }
    //try
    //{

    return (new Date(s)).getTime();
    //}
    //catch(e)
    //{
    //    //log.Warning("(" + context + ") = " + s + " - " + e.Message);
    //}
    //// return Convert.ToInt64(DateTime.Now); 
    //return CommonFunctions.CurrentTimeMillis();// Convert.ToInt64(DateTime.Now);// System.currentTimeMillis();
};

VIS.context.getShowClientOrg = function () {
    return this.getContextAsInt("#ClientOrgLevel");
};

/**
 *	Remove context for Window (i.e. delete it)
 *  @param WindowNo window
 */
VIS.context.removeWindow = function (windowNo) {
    var keys = [];
    //var ctx = this.m_map[windowNo];
    this.m_map[windowNo] = null;
    //for (var prop in ctx) {
    //    if (prop.startsWith(windowNo + "|")) {
    //        keys.push(prop);
    //    }
    //}
    //var key;
    //while (keys.length > 0) {
    //    key = keys.pop();
    //    ctx[key] = null;
    delete this.m_map[windowNo];
    key = null;
    //}
};

VIS.context.getWindowCtx = function (windowNo) {
    var ctx = {};
    $.extend(ctx, this.ctx);
    if (windowNo)
        $.extend(ctx, this.m_map[windowNo]);
    return ctx;
};

VIS.context.getEntireCtx = function () {
    var ctx = {};
    $.extend(ctx, this.ctx);
    for (var property in this.m_map) {
        if (this.m_map.hasOwnProperty(property)) {
            $.extend(ctx, this.m_map[property]);
        }
    }
    return ctx;
};

/**
	 *	Is Sales Order Trx 
	 *  @param WindowNo window no
	 *  @return true if SO (default)
	 */
VIS.context.isSOTrx = function (WindowNo) {
    var s = false;
    if (WindowNo) {
        s = this.getWindowContext(WindowNo, "IsSOTrx", true);
    }
    else {
        s = this.getContext("IsSOTrx");
    }
    if ((s != null) && s.equals("Y"))
        return true;
    return false;
};	//

VIS.context.setIsSOTrx = function (windowNo, isSOTrx) {
    if (arguments.length == 2)
        this.setContext(arguments[0], "IsSOTrx", arguments[1] ? "Y" : "N");
    else
        this.setContext("IsSOTrx", arguments[0] ? "Y" : "N");
};

VIS.context.getIsUseCrystalReportViewer = function ()
{
    return VIS.context.getContext("#USE_CRYSTAL_REPORT_VIEWER")=="Y";
};

// VIS0008 added for bulk location name reload
VIS.context.getLocationBulkReload = function () {
    return VIS.context.getContext('#LOCATION_NAME_BULK_REQUEST') == "Y";
};



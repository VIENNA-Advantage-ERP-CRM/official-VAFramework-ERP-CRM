
; (function ($, VIS) {

    VIS.IniConstants =
   {
       log: VIS.Logging.VLogger.getVLogger(" VIS.Ini"),
       ENV_PREFIX: "env.", VIENNA_HOME: "VIENNA_HOME", VIENNA_PROPERTY_FILE: "vienna.properties",
       VIENNA_ENV_FILE: "viennaEnv.properties", P_UID: "ApplicationUserID", DEFAULT_UID: "UserPwd",
       P_PWD: "ApplicationPassword", DEFAULT_PWD: "UserPwd", P_STORE_PWD: "StorePassword", DEFAULT_STORE_PWD: true,
       P_TRACELEVEL: "TraceLevel", DEFAULT_TRACELEVEL: "WARNING", P_TRACEFILE: "TraceFile", DEFAULT_TRACEFILE: false,
       //P_LANGUAGE: "Language", DEFAULT_LANGUAGE: VIS.Login.Language.GetName("en-US"),
       P_INI: "FileNameINI", DEFAULT_INI: "", P_CONNECTION: "Connection", DEFAULT_CONNECTION: "",
       P_A_LOGIN: "AutoLogin", DEFAULT_A_LOGIN: false, P_A_NEW: "AutoNew", DEFAULT_A_NEW: true,
       P_VIENNASYS: "ViennaSys", DEFAULT_VIENNASYS: false, P_CACHE_WINDOW: "CacheWindow", DEFAULT_CACHE_WINDOW: false,
       P_TEMP_DIR: "TempDir", DEFAULT_TEMP_DIR: "", P_ROLE: "Role", DEFAULT_ROLE: "", P_CLIENT: "Client",
       DEFAULT_CLIENT: "", P_ORG: "Organization", DEFAULT_ORG: "", P_WAREHOUSE: "Warehouse", DEFAULT_WAREHOUSE: "",
       P_TODAY: "CDate264", DEFAULT_TODAY: Date.now,
       P_PRINTPREVIEW: "PrintPreview", DEFAULT_PRINTPREVIEW: true, P_WARNING: "Warning",
       DEFAULT_WARNING: "Do_not_change_any_of_the_data_as_they_will_have_undocumented_side_effects.",
       P_PRINTER: "Printer", DEFAULT_PRINTER: "", P_DBUSED: "dbused", DEFAULT_DBUSED: "",
       P_DB_NAME: "dbname", DEFAULT_DB_NAME: "", P_DBUSER_ID: "dbuserid", DEFAULT_DBUSER_ID: "",
       P_DBPWD: "dbpwd", DEFAULT_DB_PWD: "", P_DB_PORT: "dbport", DEFAULT_DB_PORT: "", P_DB_HOST: "dbhost",
       DEFAULT_DB_HOST: "",
       //Show Client-Org level  0 : both  1 : Client    2 : Org    3 : None
       CO_SHOWLEVEL: "ClientOrgLevel", DEFAULT_CO_SHOWLEVEL: "2", W_PAGESIZE: "WorkSpacePageSize", DEFAULT_W_PAGESIZE: "5",
       WIN_PAGESIZE: "WindowPageSize", DEFAULT_WIN_PAGESIZE: "50", P_Show_Mini_Grid: "ShowMiniGrid", DEFAULT_MiniGrid_Status: false,
       P_Print_Native_Digits: "PrintNativeDigits", DEFAULT_Native_Digits_Status: true, P_WELCOME_SCREEN: "HideWelcomeScreen",
       DEFAULT_SCREEN_STATUS: false, P_APP_TYPE: "AppType", DEFAULT_APP_TYPE: "VServer",
       P_APP_HOST: "AppHost", DEFAULT_APP_HOST: "localhost", P_APP_PORT: "AppPort", DEFAULT_APP_PORT: "2090",
       _VIENNASYS: "VFramworkSys", _XML_DOC_PATH: "appconn.xml", _XML_ROOT: "//connectionvar"
   };

    VIS.Ini = function () {

        function setValueToLocalStorage(key, value) {
            localStorage.setItem(key, value);
        };
        function getLocalStorage(key) {
            return localStorage.getItem(key);
        };
        function clearLocalStorage() {
            localStorage.clear();
        };
        function removeLocalStorageItem(key) {
            localStorage.removeItem(key);
        };

        function getIsCacheWindow() {
            return localStorage.getItem(VIS.IniConstants.P_CACHE_WINDOW) == "Y";
        };

        function getTraceLevel() {
            if (VIS.Ini.getLocalStorage(VIS.IniConstants.P_TRACELEVEL) != null) {
                return VIS.Logging.Level.prototype.getLevel(VIS.Ini.getLocalStorage(VIS.IniConstants.P_TRACELEVEL));
            }
            return VIS.Logging.Level.INFO;
        };


        function updateLocalContextFromIni() {
            var iniarray = [VIS.IniConstants.P_A_NEW, VIS.IniConstants.P_TRACEFILE, VIS.IniConstants.P_TRACELEVEL,
                VIS.IniConstants.CO_SHOWLEVEL, VIS.IniConstants.W_PAGESIZE, VIS.IniConstants.WIN_PAGESIZE,
                VIS.IniConstants.P_Show_Mini_Grid, VIS.IniConstants.P_Print_Native_Digits, VIS.IniConstants.P_A_LOGIN,
                VIS.IniConstants.P_STORE_PWD, VIS.IniConstants.P_CACHE_WINDOW, VIS.IniConstants.P_PRINTPREVIEW, VIS.IniConstants.P_TRACEFILE];

            var obj = {};
            for (var i = 0; i < iniarray.length; i++) {
                if (iniarray[i] == "AutoNew") {
                    VIS.context.setContext(iniarray[i], VIS.Ini.getLocalStorage(iniarray[i]) == null ? "Y" : VIS.Ini.getLocalStorage(iniarray[i]));
                }
                else {
                    VIS.context.setContext('#' + iniarray[i], VIS.Ini.getLocalStorage(iniarray[i]));
                }
                obj[iniarray[i]] = VIS.Ini.getLocalStorage(iniarray[i]);
            }

            obj['#TimezoneOffset'] = new Date().getTimezoneOffset().toString();

            VIS.dataContext.updateClientCtx(obj);
        };

        return {
            setValueToLocalStorage: setValueToLocalStorage,
            getLocalStorage: getLocalStorage,
            clearLocalStorage: clearLocalStorage,
            removeLocalStorageItem: removeLocalStorageItem,
            updateLocalContextFromIni: updateLocalContextFromIni,
            getIsCacheWindow: getIsCacheWindow,
            getTraceLevel: getTraceLevel
        }
    }();

}(jQuery, VIS));
; VISUtil = window.VISUtil || {};

; (function (VIS, $) {

    VIS.Events = function () {

        var onTouchStartOrClick = "click";

        var onTouchStartAndClick = "touchstart";

        var onTouchEndOrClick = "click";

        if ('onmouseover' in document && 'ontouchstart' in document) {
            onTouchStartOrClick = "click";
            onTouchStartAndClick = "click";
        }
        else if ('ontouchstart' in document) {
            onTouchStartOrClick = 'touchstart';
            onTouchStartAndClick = 'touchstart';
        }
        else {
            onTouchStartOrClick = 'click';
            onTouchStartAndClick = "click";
        }

        if ('ontouchend' in document) {
            onTouchEndOrClick = 'touchend';
        } else {
            onTouchEndOrClick = 'click';
        }

        var onClick = "click";

        return {
            onTouchStartOrClick: onTouchStartOrClick,
            onTouchEndOrClick: onTouchEndOrClick,
            onClick: onClick,
            onTouchStartAndClick: onTouchStartAndClick
        }

    }();

    VIS.Actions = {
        zoom: "zoom",
        refresh: "refresh",
        preference: "preference",
        add: "add",
        update: "update",
        remove: "delete",
        contact: "contact",
        addnewrec: "AddNewRecord"
    };

    VIS.EnvConstants =
    {
        /** WindowNo for Find           */
        WINDOW_FIND: 1110,
        /** WinowNo for MLookup         */
        WINDOW_MLOOKUP: 1111,
        /** WindowNo for PrintCustomize */
        WINDOW_CUSTOMIZE: 1112,

        /** WindowNo for PrintCustomize */
        WINDOW_INFO: 1113,
        /** Tab for Info                */
        TAB_INFO: 1113,
        /** WindowNo for AccountEditor */
        WINDOW_ACCOUNT: 1114,
        /** Temp WindowNo for GridField */
        WINDOW_TEMP: 11100000,
        /** Maximum int value --code by raghu*/
        INT32MAXVALUE: 2147483647
    }




    //**********************  NumberFormating and Min,Max and fraction Length Setting **********************//    
    function Format(maxIntDigit, maxFractionDigit, minFractionDigit) {



        var SetIntDigit = function (val) {

            if (isNaN(val) || val === null) {
                return 0;
            }
            var orgStr = val.toString();

            var deciPos = orgStr.indexOf('.');
            //if sending object is decimal type
            if (deciPos != -1) {
                var beforeDeciStr = orgStr.substring(0, deciPos);
                if (beforeDeciStr.length > maxIntDigit) {
                    beforeDeciStr = beforeDeciStr.substring(0, maxIntDigit);
                    var finalStr = beforeDeciStr + orgStr.substring(deciPos, orgStr.Length - deciPos);
                    return finalStr;
                }
            }
            else //if sending object is integer type    
            {
                //if (orgStr.length > maxIntDigit) {
                //    var finalStr = orgStr.substring(0, maxIntDigit);
                //    if (finalStr > VIS.EnvConstants.INT32MAXVALUE) {
                //        return VIS.EnvConstants.INT32MAXVALUE;
                //    }
                //    //parseint work fine only when no fraction otherwise it convert 1.8 to 1
                //    return parseInt(orgStr.substring(0, maxIntDigit));
                //}
                if (maxFractionDigit === 0 && minFractionDigit === 0) {
                    if (orgStr > VIS.EnvConstants.INT32MAXVALUE) {
                        //return max integer value
                        return VIS.EnvConstants.INT32MAXVALUE;
                    }
                    else if (orgStr < -1 * (VIS.EnvConstants.INT32MAXVALUE + 1)) {
                        //return minimum integer value
                        return -1 * (VIS.EnvConstants.INT32MAXVALUE + 1);
                    }
                }
            }
            return val;
        };

        /// <summary>
        /// return formatted string 
        /// if value is greate than system default integer max value
        /// then slice value to set integer max
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        this.GetFormatedValue = function (val) {
            var o = SetIntDigit(val).toString();
            if (minFractionDigit != 0) {
                if (o.indexOf(".") <= -1) {
                    o = parseFloat(o).toFixed(minFractionDigit);
                }
                else if (o.split(".")[1].length > minFractionDigit) {
                    o = parseFloat(o).toFixed(maxFractionDigit);
                    o = parseFloat(o);
                }
                else if (minFractionDigit === maxFractionDigit) {
                    o = parseFloat(o).toFixed(minFractionDigit);
                }
                else if (o.split(".")[1].length < minFractionDigit) {
                    o = parseFloat(o).toFixed(minFractionDigit);
                }
            }
            //Also remove extra zero before return
            return o;
        };

        this.GetConvertedNumber = function (val, dotFormatter) {
            val = this.GetConvertedString(val, dotFormatter);
            if (dotFormatter) {

                var inputSplit = val.split(".");
                // Check value without exponetial
                if (!Number.isSafeInteger(Number(inputSplit[0]))) {
                    return Number.MAX_SAFE_INTEGER; // Set maximumn value
                }
                return Number(String(val).replace(/[^0-9.-]+/g, ""));
            } else {
                var inputSplit = val.split(",");
                // Check value without exponetial
                if (!Number.isSafeInteger(Number(inputSplit[0]))) {
                    return Number.MAX_SAFE_INTEGER; // Set maximumn value
                }
                return Number(String(val).replace(/[^0-9,-]+/g, "").replace(/[,]+/g, "."));
            }


        }

        // Function to convert String To Number in 1000 Format
        this.GetConvertedString = function (num, dotFormatter) {
            var _tStr = "";
            if (dotFormatter) {
                //return String(String(num).replace(/[^0-9.-]+/g, ""));

                _tStr = num.match(/[.]+/g);
                if (_tStr != null && _tStr.length > 1) {
                    return "";
                } else {
                    return String(String(num).replace(/[^0-9.-]+/g, ""));
                }
            } else {
                //return String(String(num).replace(/[^0-9,-]+/g, "").replace(/[,]+/g, ","));

                _tStr = num.match(/[,]+/g);
                if (_tStr != null && _tStr.length > 1) {
                    return "";
                } else {
                    return String(String(num).replace(/[^0-9,-]+/g, "").replace(/[,]+/g, ","));
                }
            }
        }

        //For to Format value as 1000 separator [, or .]
        this.GetFormatAmount = function (_value, _typeFormat, dotFormatter) {
            var language = window.navigator.language;

            // For testing purpose
            //language = "en-US";

            var input = String(_value);
            var inputSplit = "";

            if (_typeFormat == "init") {
                inputSplit = input.split(".");
            } else if (_typeFormat == "blur" || _typeFormat == "formatOnly") {
                if (dotFormatter) {
                    inputSplit = input.split(".");
                } else {
                    inputSplit = input.split(",");
                }
            }

            if (inputSplit[1] != undefined) {
                var inputFrac = inputSplit[1].replace(/[\D\s\,_\-]+/g, "");
            }
            // Format the number string on thousand separator using the regex
            var inputPrime = inputSplit[0].replace(/[\s\.\,_]+/g, "");
            inputPrime = inputPrime ? parseInt(inputPrime, 10) : 0;
            if (inputFrac != undefined) {
                if (_typeFormat == "init" || _typeFormat == "formatOnly") {
                    if (dotFormatter) {
                        // en-US
                        //return (inputPrime === 0) ? "" : (inputPrime.toLocaleString(language) + "." + inputFrac);
                        return inputPrime.toLocaleString(language) + "." + inputFrac;
                    } else {
                        // de-DE
                        //return (inputPrime === 0) ? "" : (inputPrime.toLocaleString(language) + "," + inputFrac);
                        return inputPrime.toLocaleString(language) + "," + inputFrac;
                    }
                } else {
                    if (dotFormatter) {
                        return inputPrime + "." + inputFrac;
                    } else {
                        return inputPrime + "," + inputFrac;
                    }
                }
            } else {
                if (_typeFormat == "init" || _typeFormat == "formatOnly") {
                    if (dotFormatter) {
                        // en-US
                        return inputPrime.toLocaleString(language);
                    } else {
                        // de-DE
                        return inputPrime.toLocaleString(language);
                    }
                } else {
                    if (dotFormatter) {
                        return inputPrime;
                    } else {
                        return inputPrime;
                    }
                }
            }
        }

        this.getMinFractionDigit = function () {
            return minFractionDigit;
        }

        this.getMaxFractionDigit = function () {
            return maxFractionDigit;
        }

        this.getLocaleAmount = function (amount) {
            //maximumFractionDigits set to 6, so that same decimal value can be generated. We assume that in our system there will be no amount more than 6 decimal places
            var formattedAmount = this.GetFormatedValue(amount).toLocaleString(undefined, { maximumFractionDigits: 6 });//.toFixed(2);
            // 2nd parameter changed from init to formatonly because init was checking . only.
            return this.GetFormatAmount(formattedAmount, "formatOnly", VIS.Env.isDecimalPoint());
        };

        /* privilized function */
        this.dispose = function () {
            this.GetFormatedValue = null;
            this.GetConvertedNumber = null;
            this.GetConvertedString = null;
            this.GetFormatAmount = null;
            SetIntDigit = null;
        };


    };
    VIS.Format = Format;

    /*************  Utility  ***********************



    ************************************************/


    VIS.Utility = {

        divCoding: $('<div>'),

        inheritPrototype: function (subType, superType) {
            //Perform inheritence in javascript way
            var prototype = Object.create(superType.prototype); //create object
            prototype.constructor = subType; //augment object
            subType.prototype = prototype; //assign object
            subType.prototype.$super = superType.prototype;//base proto
        },

        getBusyPanel: function () {
            //Return Busy Indicator panel
            var busy = $("<div>").html($("<strong>").html("Loading...."));
            return busy;
        },

        getFunctionByName: function (functionName, context /*, args */) {

            var args = [].slice.call(arguments).splice(2);
            var namespaces = functionName.split(".");
            var func = namespaces.pop();
            for (var i = 0; i < namespaces.length; i++) {
                context = context[namespaces[i]];
            }
            return context[func];
        },

        utf8_to_b64: function (str) {
            return window.btoa(unescape(encodeURIComponent(str)));
        },

        b64_to_utf8: function (str) {
            return decodeURIComponent(escape(window.atob(str)));
        },

        decodeText: function (str) {
            return this.divCoding.html(str).text();
        },

        encodeText: function (str) {
            //create a in-memory div, set it's inner text(which jQuery automatically encodes)
            //then grab the encoded contents back out.  The div never exists on the page.
            return this.divCoding.text(str).html();
        }
    };

    VIS.Utility.encode = VIS.Utility.utf8_to_b64;
    VIS.Utility.decode = VIS.Utility.b64_to_utf8;
    //VIS.Utility.decodeText = VIS.Utility.decodeText;
    //VIS.Utility.encodeText = VIS.Utility.encodeText;



    /*************  Util  ***********************
    Common Functions
    ************************************************/
    VIS.Utility.Util = {
        getValueOfInt: function (value) {
            if (value == null || value.toString().trim() == "") {
                return 0;
            }
            if (isNaN(value)) {
                return 0;
            }
            return parseInt(value.toString());
        },
        getValueOfDecimal: function (value) {
            if (value == null || value.toString().trim() == "") {
                return 0;
            }
            if (isNaN(value)) {
                return 0;
            }
            return parseFloat(value.toString());
        },
        getValueOfDouble: function (value) {
            if (value == null || value.toString().trim() == "") {
                return 0;
            }
            if (isNaN(value)) {
                return 0;
            }
            return parseFloat(value.toString());
        },

        getValueOfString: function (value) {
            if (value == null || value.toString().trim() == "") {
                return "";
            }
            if (value == null) {
                return "";
            }
            return value.toString();
        },
        getValueOfDate: function (value) {
            if (value == null || value.toString().trim() == "") {
                return 0;
            }
            if (value) {
                return new Date(value);
            }
            else
                return null;
        },

        /** @param {datetime in string }
        *to get formatted datetime*/
        getValueOfFormattedDateTime: function (value) {
            if (value == null || value.toString().trim() == "") {
                return 0;
            }
            if (value) {
                var now = new Date(value)
                    , year
                    , month
                    , date
                    , hours
                    , minutes
                    , seconds
                    , formattedDateTime;

                year = now.getFullYear();
                month = now.getMonth().toString().length === 1 ? '0' + (now.getMonth() + 1).toString() : now.getMonth() + 1;
                date = now.getDate().toString().length === 1 ? '0' + (now.getDate()).toString() : now.getDate();
                hours = now.getHours().toString().length === 1 ? '0' + now.getHours().toString() : now.getHours();
                minutes = now.getMinutes().toString().length === 1 ? '0' + now.getMinutes().toString() : now.getMinutes();
                seconds = now.getSeconds().toString().length === 1 ? '0' + now.getSeconds().toString() : now.getSeconds();
                formattedDateTime = year + '-' + month + '-' + date + 'T' + hours + ':' + minutes + ':' + seconds;
                return formattedDateTime;
            }
            else
                return null;
        },

        //get formatted datetime

        getValueOfBoolean: function (value) {

            if (value == null || value.toString().trim() == "") {
                return null;
            }
            if (value.toString().toLowerCase() == "true" || value.toString() == "Y") {
                return true;
            }
            else
                return false;
        },

        contains: function (arr, val) {
            return arr.indexOf(val) > -1;
        },
        isEmpty: function (value) {
            if (value == null || value.length === 0)
                return true;
            return false;
        },
        cleanMnemonic: function (inStr) {
            if (inStr == null || inStr.length == 0)
                return inStr;
            var pos = inStr.indexOf('&');
            if (pos == -1)
                return inStr;
            //	Single & - '&&' or '& ' -> &
            if (pos + 1 < inStr.length && inStr.charAt(pos + 1) != ' ')
                inStr = inStr.substring(0, pos) + inStr.substring(pos + 1);
            return inStr;
        },
        scale: function (value) {
            if (isNaN(value)) {
                return 0;
            }
            //function whatDecimalSeparator() {
            var n = 1.1;
            n = n.toLocaleString().substring(1, 2);
            //  return n;
            //}
            var pieces = value.toString().split(n);
            // var pieces = numStr.split(n);  //Commented By Sarab numstr is Undefined
            if (pieces.length > 2 || pieces.length == 1) {
                return 0;
            }
            return pieces[1].length;
        },

        /**
         * @param {any} Name Removes image from identifier and returns text
         */
        getIdentifierDisplayVal: function (Name) {
            var val = "";
            if (Name && Name.indexOf("Images/") > -1) {
                val = Name.replace("^^" + Name.substring(Name.indexOf("Images/"), Name.lastIndexOf("^^") + 3), "_")
                if (val.indexOf("Images/") > -1) {
                    val = val.replace(val.substring(val.indexOf("Images/"), val.lastIndexOf("^^") + 3), "_")
                }
                if (val.endsWith("_")) {
                    val = val.substring(0, val.length - 1);
                }
                if (val.startsWith("_")) {
                    val = val.substring(1);
                }
            }
            else
                val = Name;
            return val;
        }
    };

    //**********************  ENV  **********************//    
    VIS.Env = function () {
        var windowNo = 1;
        var WINDOW_PAGE_SIZE = 50;
        var window_height = 400;
        var NULLString = "NULLValue";
        var obscureTypes = { DigitButLast4: "904", DigitButFirstLast4: "944", AlphanumButLast4: "A04", AlphaNumButFirstLast4: "A44" };

        function getWindowNo() {
            return windowNo++;
        };

        function getCtx() {
            return VIS.context;
        };

        function parseContext(ctx, windowNo, tabNo, value, onlyWindow, ignoreUnparsable) {

            if (typeof (tabNo) != "number") {
                ignoreUnparsable = onlyWindow;
                onlyWindow = value;
                value = tabNo;
                tabNo = 0;
            }

            if (value == null || value.length == 0)
                return "";

            var token = "";;
            var outStr = new String("");

            var i = value.indexOf('@');
            // Check whether the @ is not the last in line (i.e. in EMailAdress or with wrong entries)
            while (i != -1 && i != value.lastIndexOf("@")) {
                var getValue = value.substring(0, i);
                outStr += value.substring(0, i);			// up to @
                value = value.substring(i + 1, value.length);	// from first @

                var j = value.indexOf('@');						// next @
                if (j < 0) {
                    //_log.log(Level.SEVERE, "No second tag: " + inStr);
                    return "";						//	no second tag
                }

                var ctxInfo = "";
                var ctxInfo1 = "";

                token = value.substring(0, j);

                if (token.contains(".")) {
                    token = token.substring(0, token.indexOf("."));
                    //txInfo = ctx.getWindowContext(WindowNo, tabNo, token.substring(0, token.indexOf(".")), onlyWindow);	// get context
                }

                ctxInfo = ctx.getWindowContext(windowNo, tabNo, token, onlyWindow);	// get context

                if (ctxInfo.length == 0 && (token.startsWith("#") || token.startsWith("$")))
                    ctxInfo = ctx.getContext(token);	// get global context
                if (ctxInfo.length == 0) {
                    //_log.config("No Context Win=" + WindowNo + " for: " + token);
                    if (!ignoreUnparsable)
                        return "";

                    outStr += ' NULL ';
                    //						//	token not found
                }
                else {
                    outStr += ctxInfo;				// replace context with Context
                }

                value = value.substring(j + 1, value.length);	// from second @
                i = value.indexOf('@');
            }
            outStr += value;						// add the rest of the string
            return outStr;
        };

        function getObscureValue(type, value) {
            if (value) {
                if (type == obscureTypes.DigitButLast4) {
                    return value.replace(/[^a-zA-Z0-9\.\-\@]/gi, '').replace(/[0-9](?=[\w\.\-\@]{4})/g, "*");
                }
                else if (type == obscureTypes.DigitButFirstLast4) {
                    value = value.replace(/[^a-zA-Z0-9\.\-\@\s]/gi, '');
                    var len = value.length;
                    var retVal = '';
                    for (var i = 0; i < len; i++) {
                        var cur = value[i];
                        if ((i > 3 && i < len - 4) && !isNaN(cur))  // skip first four and last four 
                            cur = '*'
                        retVal += cur;
                    }
                    return retVal;
                }
                else if (type == obscureTypes.AlphanumButLast4) {
                    return value.replace(/[^a-zA-Z0-9\.\s\@\-]/gi, '').replace(/[a-zA-Z0-9\s\.\@\-](?=[a-zA-Z0-9\s\.\@\-]{4})/g, "*");
                }
                else if (type == obscureTypes.AlphaNumButFirstLast4) {
                    value = value.replace(/[^a-zA-Z0-9\@\.\s\-]/gi, '');
                    var len = value.length;
                    var retVal = '';
                    for (var i = 0; i < len; i++) {
                        var cur = value[i];
                        if (i > 3 && i < len - 4)  // skip first four and last four 
                            cur = '*'
                        retVal += cur;
                    }
                    return retVal;
                }
            }
        };

        function getWINDOW_PAGE_SIZE() {
            return WINDOW_PAGE_SIZE;
        };

        function setWINDOW_PAGE_SIZE(pSize) {
            if (!pSize || pSize.toString().length == 0)
                pSize = 50;
            WINDOW_PAGE_SIZE = pSize;
        };

        function getScreenHeight() {
            return window_height;
        };

        function setScreenHeight(height) {
            window_height = height
        };

        function getPreference(ctx, AD_Window_ID, context, system) {
            /**************************************************************************
         *	Get Preference.
         *  <pre>
         *		0)	Current Setting
         *		1) 	Window Preference
         *		2) 	Global Preference
         *		3)	Login settings
         *		4)	Accounting settings
         *  </pre>
         *  @param  ctx context
         *	@param	AD_Window_ID window no
         *	@param	context		Entity to search
         *	@param	system		System level preferences (vs. user defined)
         *  @return preference value
         */

            if (ctx == null || context == null)
                throw new IllegalArgumentException("Require Context");
            var retValue = null;
            //
            if (!system)	//	User Preferences
            {
                retValue = ctx.getContext("P" + AD_Window_ID + "|" + context);//	Window Pref
                if (retValue.length == 0)
                    retValue = ctx.getContext("P|" + context);  			//	Global Pref
            }
            else			//	System Preferences
            {
                retValue = ctx.getContext("#" + context);   				//	Login setting
                if (retValue.length == 0)
                    retValue = ctx.getContext("$" + context);   			//	Accounting setting
            }
            //
            return retValue == null ? "" : retValue;
        };	//	getPreference

        function getHeader(ctx, windowNo) {
            var sb = "";
            if (windowNo > 0)
                sb += ctx.getWindowContext(windowNo, "WindowName", false) + "  ";

            //sb += ctx.getContext("##AD_User_Name") + "@" +
            //    ctx.getContext("#AD_Org_Name") + "." +
            //    ctx.getContext("#AD_Client_Name");

            return sb;
        };

        function clearWinContext(ctx, WindowNo) {
            if (ctx == null)
                throw new Error("Require Context");
            var keys = [];
            for (var prop in ctx) {
                if (prop.startsWith(windowNo + "|")) {
                    keys.push(prop);
                }
            }
            var key;
            while (keys.length > 0) {
                key = keys.pop();
                ctx[key] = null;
                delete ctx[key];
                key = null;
            }
            //removeWindow(WindowNo);
        }	//	clearWinContext

        function removeCultCommaDot(value, deciSep) {
            if (isNaN(value) || value === null) {
                return "0";
            }
            var newVal = value.toString();
            if (newVal.contains(deciSep)) {
                //newVal = newVal.replace(/\./g, '');
                //newVal = newVal.replaceAll(deciSep, "");
                newVal = newVal.split(deciSep).join('');
            }

            //if (type === VIS.DisplayType.Integer || type === VIS.DisplayType.Quantity) {
            //    if (newVal.indexOf(",") > -1) {
            //        newVal = newVal.replaceAll(",", "");
            //    }
            //    //dot not alloewd here if come then it is from culture then remove before procceds
            //    if (newVal.indexOf(".") > -1) {
            //        newVal = newVal.replaceAll(".", "");
            //    }
            //}
            //else if (type === VIS.DisplayType.Amount || type === VIS.DisplayType.Number) {
            //    if ((newVal.split(",").length > newVal.split(".").length) && Globalize.cultureSelector === "en-US") {
            //        if (newVal.indexOf(",") > -1) {
            //            newVal = newVal.replaceAll(",", "");
            //        }
            //    }
            //    else if ((newVal.split(".").length > newVal.split(",").length) && Globalize.cultureSelector != "en-US") {
            //        if (newVal.indexOf(".") > -1) {
            //            newVal = newVal.replaceAll(".", "");
            //        }
            //    }
            //}
            return newVal;
        };

        function numberCultureValueFormat(calVal, sep) {
            var splitValue = [];
            splitValue = calVal.split(".");
            var val = Globalize.format(Number(splitValue[0]), "N", Globalize.cultureSelector);
            var valAft = splitValue[1] === "" ? ".0" : splitValue[1];
            calVal = val.substring(0, val.length - 3) + sep + valAft;
            //if (calVal.split('.')[1].length < 4) {
            //    calVal = this.value.replace(".000", ".0");
            //}
            return calVal;
        };

        function numberDisplay(self, val, displayType) {
            val = VIS.Env.removeCultCommaDot(val, self.thousendSeprator);
            val = val.split(self.decimalSeprator).join('.');
            val = self.format.GetFormatedValue(val);

            if (displayType === VIS.DisplayType.Amount) {
                val = Globalize.format(Number(val), "N", Globalize.cultureSelector);
            }
            else if (displayType === VIS.DisplayType.Number) {
                val = numberCultureValueFormat(val.toString(), self.decimalSeprator);
            }
            return val;
        };

        function currentTimeMillis() {
            return Date.now;
        };


        /**
     *  Check Base Language
     *  @param ctx context
     * 	@param tableName table to be translated
     * 	@return true if base language and table not translated
     */
        function isBaseLanguage(ctx, tableName) {

            var lang = "";
            if (typeof (ctx) != "string") {
                lang = getAD_Language(ctx);
            }
            else {
                lang = ctx; //string 
            }
            return getBaseAD_Language() == lang;
        };	//	isBaseLanguage

        /**
             *  Get System AD_Language
             *  @param ctx context
             *	@return AD_Language eg. en_US
             */
        function getAD_Language(ctx) {
            if (ctx != null) {
                var lang = ctx.getContext("#AD_Language");
                if (lang != null && lang.length > 0)
                    return lang;
            }
            return getBaseAD_Language();
        };	//	getAD

        function getBaseAD_Language() {
            return "en_US";
        };

        function signum(value) {
            if (value == null || value.toString().trim() == "") {
                return 0;
            }
            if (isNaN(value)) {
                return 0;
            }
            return value > 0 ? 1 : value < 0 ? -1 : 0;
        };

        function startBrowser(url) {
            window.open(url);
        };

        function getDecimalSeparator(locale) {
            var numberWithDecimalSeparator = 1.1;
            //if (window.Intl) {
            //    return Intl.NumberFormat(locale)
            //        .formatToParts(numberWithDecimalSeparator)
            //        .find(part => part.type === 'decimal')
            //        .value;
            //}
            return numberWithDecimalSeparator
                .toLocaleString(locale)
                .substring(1, 2);
        };

        function isDecimalPointss(lang) {
            var language = window.navigator.language;
            if (lang && lang != '')
                language = lang;

            if (getDecimalSeparator(language) != ',')
                return true;
            return false;
        };

        return {
            getWindowNo: getWindowNo,
            getCtx: getCtx,
            parseContext: parseContext,
            getWINDOW_PAGE_SIZE: getWINDOW_PAGE_SIZE,
            setWINDOW_PAGE_SIZE: setWINDOW_PAGE_SIZE,
            setScreenHeight: setScreenHeight,
            getScreenHeight: getScreenHeight,
            getPreference: getPreference,
            getHeader: getHeader,
            clearWinContext: clearWinContext,
            removeCultCommaDot: removeCultCommaDot,
            numberCultureValueFormat: numberCultureValueFormat,
            numberDisplay: numberDisplay,
            isBaseLanguage: isBaseLanguage,
            getAD_Language: getAD_Language,
            getBaseAD_Language: getBaseAD_Language,
            currentTimeMillis: currentTimeMillis,
            signum: signum,
            startBrowser: startBrowser,
            isDecimalPoint: isDecimalPointss,
            getDecimalSeparator: getDecimalSeparator,
            ZERO: 0,
            ONE: 1,
            ONEHUNDRED: 100.0,
            NL: '\r\n',
            SHOW_CLIENT_ORG: 0,
            SHOW_CLIENT_ONLY: 1,
            SHOW_ORG_ONLY: 2,
            HIDE_CLIENT_ORG: 3,
            NULLString: NULLString,
            approveCol: "IsApproved",
            getObscureValue: getObscureValue
        }
    }();
    // ******************** END ENV *********************//


    //********************* AENV ************************//
    VIS.AEnv = function () {

        var voCache = {};
        var s_workflow = null;
        var s_workflow_Window_ID = 0;

        var baseUrl = VIS.Application.contextUrl;
        var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";
        var nonQueryUrl = baseUrl + "JsonData/ExecuteNonQuer";

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


        function getGridWindow(windowNo, AD_Window_ID, callback) {
            VIS.dataContext.getWindowJString({ windowNo: windowNo, AD_Window_ID: AD_Window_ID }, callback);
            //return getGridWindowFromServer(windowNo, AD_Window_ID);
        };

        function getGridWindowFromServer(curWindowNo, ID) {


        };

        function getZoomButton(disabled) {

            return $('<button class="vis-controls-txtbtn-table-td2" ' + ((disabled) ? "disabled" : "") + ' ><i class="vis vis-find" /></button>');
        }

        function getContextPopup(options, fieldName) {

            var ulPopup = $("<ul class='vis-apanel-rb-ul'>");
            if (typeof options[VIS.Actions.zoom] !== "undefined") {
                ulPopup.append($("<li data-action='" + VIS.Actions.zoom + "' style='opacity:" + (options[VIS.Actions.zoom] ? .7 : 1) +
                    "'><i data-action='" + VIS.Actions.zoom + "' class='fa fa-search-plus'></i><span data-action='" + VIS.Actions.zoom + "'>" + VIS.Msg.getMsg("Zoom") + "</span></li>"));
                //New option for combo and search control.
                if (options[VIS.Actions.addnewrec])
                    ulPopup.append($("<li data-action='" + VIS.Actions.addnewrec + "' style='opacity:" + (options[VIS.Actions.zoom] ? .7 : 1) +
                        "'><i data-action='" + VIS.Actions.addnewrec + "' class='fa fa-plus'></i><span data-action='" + VIS.Actions.addnewrec + "'>" + VIS.Msg.getMsg("AddNew") + "</span></li>"));
            }
            if (options[VIS.Actions.preference])
                ulPopup.append($("<li data-action='" + VIS.Actions.preference + "'><i data-action='" + VIS.Actions.preference + "' class='fa fa-cog'></i><span data-action='" + VIS.Actions.preference + "'>" + VIS.Msg.getMsg("Preference") + "</span></li>"));
            if (options[VIS.Actions.refresh])
                ulPopup.append($("<li data-action='" + VIS.Actions.refresh + "'><i data-action='" + VIS.Actions.refresh + "' class='fa fa-refresh'></i><span data-action='" + VIS.Actions.refresh + "'>" + VIS.Msg.getMsg("Requery") + "</span></li>"));
            if (options[VIS.Actions.add])
                ulPopup.append($("<li data-action='" + VIS.Actions.add + "'><i data-action='" + VIS.Actions.add + "' class='vis vis-addbp'></i><span data-action='" + VIS.Actions.add + "'>" + VIS.Msg.getMsg("Add") + "</span></li>"));
            if (options[VIS.Actions.update])
                ulPopup.append($("<li data-action='" + VIS.Actions.update + "'><i data-action='" + VIS.Actions.update + "' class='vis vis-updatebp'></i><span data-action='" + VIS.Actions.update + "'>" + VIS.Msg.getMsg("Update") + "</span></li>"));
            if (options[VIS.Actions.remove])
                ulPopup.append($("<li data-action='" + VIS.Actions.remove + "'><i data-action='" + VIS.Actions.remove + "' class='fa fa-arrow-left'></i><span data-action='" + VIS.Actions.remove + "'>" + VIS.Msg.getMsg("Clear") + "</span></li>"));
            if (options[VIS.Actions.contact])
                ulPopup.append($("<li data-action='" + VIS.Actions.contact + "'><i data-action='" + VIS.Actions.contact + "' class='fa fa-user'></i><span data-action='" + VIS.Actions.contact + "'>" + VIS.Msg.getMsg("Contact") + "</span></li>"));

            //
            return ulPopup;
        };

        /**
          * 	Is Workflow Process view enabled.
          *	@return true if enabled
          */
        function getIsWorkflowProcess() {
            if (s_workflow == null) {
                s_workflow = false;
                var AD_Table_ID = 645;	//	AD_WF_Process	
                if (VIS.MRole.getIsTableAccess(AD_Table_ID, true))	//	RO
                    s_workflow = true;
                else {
                    AD_Table_ID = 644;	//	AD_WF_Activity	
                    if (VIS.MRole.getIsTableAccess(AD_Table_ID, true))	//	RO
                        s_workflow = true;
                }
                //	Get Window
                if (s_workflow) {
                    // VIS.DB.executeScalar("SELECT AD_Window_ID FROM AD_Table WHERE AD_Table_ID=" + AD_Table_ID, null, function (val) {
                    //var dr=null;
                    $.ajax({
                        type: 'Get',
                        async: true,
                        url: VIS.Application.contextUrl + "Form/GetWorkflowWindowID",
                        data: { AD_Table_ID: AD_Table_ID },
                        success: function (data) {
                            var val = JSON.parse(data);
                            if (val && !isNaN(val))
                                s_workflow_Window_ID = parseInt(val);
                            if (s_workflow_Window_ID == 0)
                                s_workflow_Window_ID = 297;	//	fallback HARDCODED
                        },
                    });


                    //    if (val && !isNaN(val))
                    //        s_workflow_Window_ID = parseInt(val);
                    //    if (s_workflow_Window_ID == 0)
                    //        s_workflow_Window_ID = 297;	//	fallback HARDCODED
                    //});
                }
            }
            return s_workflow;
        };

        function startWorkflowProcess(AD_Table_ID, Record_ID) {
            //if (Envs.workflowWindowID == 0)
            //{
            //    return;
            //}
            //
            var query = null;
            if (AD_Table_ID != 0 && Record_ID != 0) {
                query = new VIS.Query("AD_WF_Process");
                query.addRestriction("AD_Table_ID", VIS.Query.prototype.EQUAL, AD_Table_ID);
                query.addRestriction("Record_ID", VIS.Query.prototype.EQUAL, Record_ID);
                VIS.viewManager.startWindow(s_workflow_Window_ID, query);
            }
        };

        function zoom(AD_Table_ID, Record_ID) {
            var tableName = null;
            var AD_Window_ID = 0;
            var PO_Window_ID = 0;

            // var sql = "SELECT TableName, AD_Window_ID, PO_Window_ID FROM AD_Table WHERE AD_Table_ID=" + AD_Table_ID;
            var dr = null;
            $.ajax({
                type: 'Get',
                async: false,
                url: VIS.Application.contextUrl + "Form/GetZoomWindowID",
                data: { AD_Table_ID: AD_Table_ID },
                success: function (data) {
                    dr = new VIS.DB.DataReader().toJson(data)
                },
            });

            //dr = VIS.DB.executeReader(sql);
            if (dr.read()) {
                tableName = dr.get(0).toString();
                AD_Window_ID = VIS.Utility.Util.getValueOfInt(dr.getInt(1));
                PO_Window_ID = VIS.Utility.Util.getValueOfInt(dr.getInt(2));
            }
            dr.dispose();

            //  Nothing to Zoom to
            if (tableName == null || AD_Window_ID == 0) {
                //log.Info("No window/Form --> open table window and bind a window to that table");
                return;
            }

            //	PO Zoom ?
            if (PO_Window_ID != 0) {
                var whereClause = tableName + "_ID=" + Record_ID;

                AD_Window_ID = VIS.ZoomTarget.getZoomAD_Window_ID(tableName, 0, whereClause, true);

                if (AD_Window_ID == 0)
                    return;
            }
            VIS.viewManager.startWindow(AD_Window_ID, VIS.Query.prototype.getEqualQuery(tableName + "_ID", Record_ID));
        };

        // vinay bhatt workflow zoom with window id
        function wfzoom(AD_Table_ID, Record_ID, AD_WF_Activity_ID) {
            var tableName = null;
            var AD_Window_ID = 0;
            var PO_Window_ID = 0;
            var ActivityWindow = 0;

            var sql = "VIS_146";

            var param = [];
            param[0] = new VIS.DB.SqlParam("@AD_Table_ID", AD_Table_ID);
            param[1] = new VIS.DB.SqlParam("@AD_WF_Activity_ID", AD_WF_Activity_ID);
            var dr = null;
            dr = executeReader(sql, param);
            if (dr.read()) {
                tableName = dr.get(0).toString();
                AD_Window_ID = VIS.Utility.Util.getValueOfInt(dr.getInt(1));
                PO_Window_ID = VIS.Utility.Util.getValueOfInt(dr.getInt(2));
                ActivityWindow = VIS.Utility.Util.getValueOfInt(dr.getInt(3));//"1000157"
            }
            else {
                zoom(AD_Table_ID, Record_ID);
                return;
            }
            dr.dispose();

            //  Nothing to Zoom to
            if (tableName == null && AD_Window_ID == 0 && (ActivityWindow != null && ActivityWindow != 0)) {
                //log.Info("No window/Form --> open table window and bind a window to that table");
                return;
            }

            //	WFActivity Zoom ?
            if (ActivityWindow != null && ActivityWindow != 0) {
                var zoomQuery = new VIS.Query();
                //zoomQuery.addRestriction(tableName + "_ID", VIS.Query.prototype.EQUAL, Record_ID);
                //VIS.viewManager.startWindow(ActivityWindow, zoomQuery);
                VIS.viewManager.startWindow(ActivityWindow, VIS.Query.prototype.getEqualQuery(tableName + "_ID", Record_ID));
                return;
            }

            //	PO Zoom ?
            if (PO_Window_ID != 0) {
                var whereClause = tableName + "_ID=" + Record_ID;

                AD_Window_ID = VIS.ZoomTarget.getZoomAD_Window_ID(tableName, 0, whereClause, true);

                if (AD_Window_ID == 0)
                    return;
            }
            VIS.viewManager.startWindow(AD_Window_ID, VIS.Query.prototype.getEqualQuery(tableName + "_ID", Record_ID));
        };
        //

        return {
            getGridWindow: getGridWindow,
            getContextPopup: getContextPopup,
            getZoomButton: getZoomButton,
            getIsWorkflowProcess: getIsWorkflowProcess,
            startWorkflowProcess: startWorkflowProcess,
            zoom: zoom,
            wfzoom: wfzoom // vinay bhatt zoom with window id
        }

    }();
    //****************** END AENV ***********************//



    //**************** MessageFormat *******************//

    function MessageFormat(pattren) {
        this.pattern;
        //"(C) Currency: . . . . . . . . {0:C}\n" +
        //"(D) Decimal:. . . . . . . . . {0:D}\n" +
        //"(E) Scientific: . . . . . . . {1:E}\n" +
        //"(F) Fixed point:. . . . . . . {1:F}\n" +
        //"(G) General:. . . . . . . . . {0:G}\n" +
        //"    (default):. . . . . . . . {0} (default = 'G')\n" +
        //"(N) Number: . . . . . . . . . {0:N}\n" +
        //"(P) Percent:. . . . . . . . . {1:P}\n" +
        //"(R) Round-trip: . . . . . . . {1:R}\n" +
        //"(X) Hexadecimal:. . . . . . . {0:X}\n",

        this.applyPattern(pattren);
    };

    MessageFormat.prototype.typeList = ["", "", "number", "", "date", "", "time", "", "choice"];
    MessageFormat.prototype.modifierList = ["", "", "currency", "", "percent", "", "integer"];
    MessageFormat.prototype.dateModifierList = ["", "", "short", "", "medium", "", "long", "", "full"];
    MessageFormat.prototype.allFormats = {
        "currency": "C",
        "decimal": "D",
        "general": "G",
        "number": "N",
        "percent": "P",
        "shortdate": "d",
        "longdate": "D",
        "shorttime": "t",
        "longtime": "T",
        "fulltime": "T",
        "fulldate": "D",
        "mediumdate": "d",
        "mediumtime": "t",
        "choice": ""
    };

    MessageFormat.prototype.applyPattern = function (pattern) {
        if (!this.cmpreFormat(pattern)) {
            this.pattern = pattern;
            return;
        }

        var segmentsList = [];

        var segment = new StringBuilder("");
        for (var i = 0; i < pattern.length; ++i) {
            var ch = pattern[i];
            if (ch == '{') {
                if (segment.length() == 0) {
                    segment.append(ch);
                }
                else {
                    segmentsList.push(segment.toString());
                    segment.clear();
                    segment.append(ch);
                }
            }
            else if (ch == '}') {
                segment.append(ch);
                segmentsList.push(segment.toString());
                segment.clear();
            }
            else if (i == pattern.length) {
                segmentsList.add(segment.toString());
                segment.clear();
            }
            else {
                segment.append(ch);
            }
        }
        this.pattern = this.parsePattren(segmentsList);
    };

    MessageFormat.prototype.parsePattren = function (segments) {
        var result = new StringBuilder(" ");
        for (var i = 0; i < segments.length; i++) {
            var part = segments[i];
            if (part.startsWith("{") && this.cmpreFormat(part)) {
                part = this.getFormat(part.split(','));
                result.append(part);
                continue;
            }
            result.append(part);
        }
        return result.toString();
    };

    MessageFormat.prototype.getFormat = function (segments) {
        var format = new StringBuilder(segments[0]);//   .append(":");
        var newFormat = "";
        switch (this.findKeyword(segments[1], this.typeList)) {
            case 0:
                break;
            case 1:
            case 2:// number
                switch (this.findKeyword(segments[2].toString(), this.modifierList)) {
                    case 0: // default;
                        newFormat = this.allFormats["number"];
                        break;
                    case 1:
                    case 2:// currency
                        newFormat = this.allFormats["currency"];
                        break;
                    case 3:
                    case 4:// percent
                        newFormat = this.allFormats["percent"];
                        break;
                    case 5:
                    case 6:// integer
                        newFormat = this.allFormats["decimal"];
                        break;
                    default: // pattern
                        newFormat = this.allFormats["number"];
                        break;
                }
                break;
            case 3:
            case 4: // date
                switch (this.findKeyword(segments[2], this.dateModifierList)) {
                    case 0: // default
                        newFormat = this.allFormats["shortdate"];
                        break;
                    case 1:
                    case 2: // short
                        newFormat = this.allFormats["shortdate"];
                        break;
                    case 3:
                    case 4: // medium
                        newFormat = this.allFormats["mediumdate"];
                        break;
                    case 5:
                    case 6: // long
                        newFormat = this.allFormats["longdate"];
                        break;
                    case 7:
                    case 8: // full
                        newFormat = this.allFormats["fulldate"];
                        break;
                    default:
                        newFormat = segments[3].ToString();
                        break;
                }
                break;
            case 5:
            case 6:// time
                switch (this.FindKeyword(segments[2].ToString(), this.dateModifierList)) {
                    case 0: // default
                        newFormat = this.allFormats["shorttime"];
                        break;
                    case 1:
                    case 2: // short
                        newFormat = this.allFormats["shorttime"];
                        break;
                    case 3:
                    case 4: // medium
                        newFormat = this.allFormats["mediumtime"];
                        break;
                    case 5:
                    case 6: // long
                        newFormat = this.allFormats["mediumtime"];
                        break;
                    case 7:
                    case 8: // full
                        newFormat = this.allFormats["mediumtime"];
                        break;
                    default:
                        newFormat = segments[2].ToString();
                        break;
                }
                break;
            case 7:
            case 8:// choice
                try {
                    newFormat = segments[3];
                    this.allFormats["choice"] = segments[3].ToString();
                }
                catch (e) {
                    throw new ArgumentException(
                        "Choice Pattern incorrect");
                }
                break;
            default:
                throw new ArgumentException("unknown format type at ");
        }
        segments = null;
        newFormat = "";
        format.append(newFormat).append("}");
        return format.toString();
    };

    MessageFormat.prototype.format = function (args) {

        // The string containing the format items (e.g. "{0}")
        // will and always has to be the first argument.
        var theString = this.pattern;

        this.pattern = null;
        // start with the second argument (i = 1)
        for (var i = 0; i < args.length; i++) {
            // "gm" = RegEx options for Global search (more than  instance)
            // and for Multiline search
            var regEx = new RegExp("\\{" + (i) + "\\}", "gm");
            theString = theString.replace(regEx, args[i]);
        }

        return theString;
    };

    MessageFormat.prototype.cmpreFormat = function (ptrn) {
        if (ptrn.contains("number,") || ptrn.contains("date,") || ptrn.contains("time,") || ptrn.contains("custom,")) {
            return true;
        }
        return false;
    };

    MessageFormat.prototype.findKeyword = function (text, str) {
        for (var i = 0; i < str.length; i++) {
            if (text.equals(str[i]))
                return i;
        }
        return -1;
    };

    //********************* END ***********************//


    function traceConsole(args) {
        var stack = "";
        try {
            throw new Error("ss")
        }
        catch (e) {
            stack = e.stack;//.split('\n');
            //console.log(stack);
        }
        var fc = false;
        //if (stack.substr(stack.lastIndexOf("utility.js") + 10).indexOf('.js') == -1)
        //{
        //    fc = true;
        //}

        var length = (stack.match(/VISjs?/g) || []).length;

        //if (length == 3) {
        //    fc = true;
        //    console.log(length);
        //}

        //for (var i = 0; i < stack.length; i++) {
        if (stack.indexOf("at Object.InjectedScript.") >= 0) {
            fc = true;   // Chrome console
        }
        if (stack.indexOf("Unknown script") == 0) {
            fc = true;   // Firefox console
        }

        if (stack.indexOf("@debugger eval code") == 0) {
            fc = true;   // Firefox console
        }
        if (stack.indexOf("_evaluateOn") == 0)
            fc = true;   // Safari console
        // }

        if (!fc) {

            if (!stack || stack.split('\n').length > 5)
                return;
        }
        //console.log("invalid function call");
        return true;

    };



    function validateAndExecute(arguments, dataIn, async, callback) {
        //if (traceConsole(arguments))
        //    return null;
        //dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
        VIS.dataContext.getDataSetJString(dataIn, async, function (jString) {
            callback(jString);
        });
    };

    function validateAndExecuteQuery(arguments, dataIn, async, callback) {
        //if (traceConsole(arguments))
        //    return null;
        // dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
        VIS.dataContext.executeQuery(dataIn, async, function (jString) {
            callback(jString);
        });
    };

    function validateAndExecuteQueries(arguments, dataIn, async, callback) {
        //if (traceConsole(arguments))
        //    return null;
        // dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
        VIS.dataContext.executeQueries(dataIn, async, function (jString) {
            callback(jString);
        });
    }



    VIS.DB = {

        QUOTE: '\'',


        getDBDateFormat: function (date, includeTime, convertToUniversal) {
            //yyyy-MM-dd
            var dString = "";




            //year 
            if (!convertToUniversal) {
                date.setMinutes(-date.getTimezoneOffset());
                //date = new Date(date.toUTCString());
            }

            dString = date.toISOString();
            var formats = dString.split('T');

            dString = formats[0];

            if (includeTime)
                dString += " " + formats[1].substring(0, formats[1].length - 5);//  .replace('Z', '');

            return dString;
        },

        to_date: function (date, dayOnly) {

            if (date && date.toString().length > 0) {
                if (date instanceof Date)
                    time = date;
                else
                    time = new Date(date);
            }
            else
                time = null;

            if (time.toString() == "Invaild Date") {
                time = null;
            }

            var dateString = "";
            var myDate = "";
            if (true) {
                if (time == null) {
                    if (dayOnly)
                        return "TRUNC(SysDate)";
                    return "SysDate";
                }

                dateString += "TO_DATE('";
                //  YYYY-MM-DD HH24:MI:SS.mmmm  JDBC Timestamp format
                //String myDate = time.ToString("yyyy-mm-dd");
                //myDate = time.ToString("yyyy-MM-dd HH:mm:ss");
                myDate = time;//"yyyy-MM-dd");
                if (dayOnly) {
                    //myDate = time.Value.ToString("yyyy-MM-dd");
                    myDate = this.getDBDateFormat(time, false, false);
                    dateString += myDate;
                    dateString += "','YYYY-MM-DD')";
                }
                else {
                    //myDate = time.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    myDate = this.getDBDateFormat(time, true, true);
                    dateString += myDate;	//	cut off miliseconds
                    dateString += "','YYYY-MM-DD HH24:MI:SS')";
                }
            }
            else if (DatabaseType.IsPostgre) {
                if (time == null) {
                    if (dayOnly)
                        return "TRUNC(SysDate)";
                    return "SysDate";
                }

                dateString = new StringBuilder("TO_DATE('");
                //  YYYY-MM-DD HH24:MI:SS.mmmm  JDBC Timestamp format
                if (dayOnly) {
                    myDate = time.Value.ToString("yyyy-MM-dd");
                    dateString.Append(myDate);
                    dateString.Append("','YYYY-MM-DD')");
                }
                else {
                    myDate = time.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    dateString.Append(myDate);	//	cut off miliseconds
                    dateString.Append("','YYYY-MM-DD HH24:MI:SS')");
                }
            }
            else if (DatabaseType.IsMSSql) {
                if (time == null) {
                    if (dayOnly)
                        return "CAST(STR(YEAR(Getdate()))+'-'+STR(Month(Getdate()))+'-'+STR(Day(Getdate())) AS DATETIME)";
                    return "getdate()";
                }

                dateString = new StringBuilder("CAST('");
                //  YYYY-MM-DD HH24:MI:SS.mmmm  JDBC Timestamp format
                myDate = time.ToString();
                if (dayOnly) {
                    dateString.Append(myDate.Substring(0, 10));
                    dateString.Append("' AS DATETIME)");
                }
                else {
                    dateString.Append(myDate.Substring(0, myDate.IndexOf(".")));	//	cut off miliseconds
                    dateString.Append("' AS DATETIME)");
                }
            }
            return dateString.toString();

            //return date;
        },

        to_char: function (columnName, displayType, AD_Language) {
            var retValue = "TRIM(TO_CHAR(";
            retValue = retValue.concat(columnName);
            //  Numbers
            if (VIS.DisplayType.IsNumeric(displayType)) {
                if (displayType == VIS.DisplayType.Amount)
                    retValue = retValue.concat(",'9G999G999G999G990D00'");
                else
                    retValue = retValue.concat(",'TM9'");
            }
            else if (VIS.DisplayType.IsDate(displayType)) {
                retValue = retValue.concat(",'").concat("yyyy-MM-dd").concat("'");
            }
            retValue = retValue.concat("))");
            return retValue.toString();
        },

        to_string: function (txt, maxLength) {
            if (txt == null && txt.length == 0)
                return "NULL";
            if (maxLength != null) {
                maxLength = 0;
            }

            //  Length
            var text = txt;
            if (maxLength != 0 && text.length > maxLength)
                text = txt.substring(0, maxLength);

            var out = new StringBuilder();
            out.append(this.QUOTE);		//	'
            for (var i = 0; i < text.length; i++) {
                var c = text.charAt(i);
                if (c == this.QUOTE)
                    out.append("''");
                else
                    out.append(c);
            }
            out.append(this.QUOTE);
            //
            text = out.toString();
            out.clear();
            return text;
        },

        //executeDataSet: function (sql, param, callback) {
        //    var async = callback ? true : false;

        //    if (traceConsole(arguments)) {
        //        return null;
        //    }

        //    var dataIn = { sql: sql, page: 1, pageSize: 0 };
        //    if (param) {
        //        dataIn.param = param;
        //    }

        //    var dataSet = null;

        //    VIS.dataContext.getDataSetJString(dataIn, async, function (jString) {
        //        dataSet = new VIS.DB.DataSet().toJson(jString);
        //        if (async) {
        //            callback(dataSet);
        //        }
        //    });
        //    return dataSet;
        //},

        executeDataSet: function (sql, param, callback) {
            var async = callback ? true : false;

            var dataIn = { sql: sql, page: 1, pageSize: 0 };
            if (param) {
                dataIn.param = param;
            }

            var dataSet = null;

            validateAndExecute(arguments, dataIn, async, function (jString) {
                dataSet = new VIS.DB.DataSet().toJson(jString);
                if (async) {
                    callback(dataSet);
                }
            });

            return dataSet;
        },

        executeDataReader: function (sql, param, callback) {
            var async = callback ? true : false;

            var dataIn = { sql: sql, page: 1, pageSize: 0 };
            if (param) {
                dataIn.param = param;
            }
            var dr = null;


            validateAndExecute(arguments, dataIn, async, function (jString) {
                dr = new VIS.DB.DataReader().toJson(jString);
                if (async) {
                    callback(dr);
                }
            });
            return dr;
        },

        executeReader: function (sql, param, callback) {
            var async = callback ? true : false;

            var dataIn = { sql: sql, page: 1, pageSize: 0 };
            if (param) {
                dataIn.param = param;
            }
            var dr = null;

            validateAndExecute(arguments, dataIn, async, function (jString) {
                dr = new VIS.DB.DataReader().toJson(jString);
                if (async) {
                    callback(dr);
                }
            });

            return dr;
        },

        executeDataReaderPaging: function (sql, page, pageSize, param, callback) {
            var async = callback ? true : false;

            var dataIn = { sql: sql, page: page, pageSize: pageSize };
            if (param) {
                dataIn.param = param;
            }
            var dr = null;

            validateAndExecute(arguments, dataIn, async, function (jString) {
                dr = new VIS.DB.DataReader().toJson(jString);
                if (async) {
                    callback(dr);
                }
            });

            return dr;
        },

        executeDataSetPaging: function (sql, page, pageSize, param, callback) {
            var async = callback ? true : false;

            var dataIn = { sql: sql, page: page, pageSize: pageSize };
            if (param) {
                dataIn.param = param;
            }

            var dataSet = null;

            validateAndExecute(arguments, dataIn, async, function (jString) {
                dataSet = new VIS.DB.DataSet().toJson(jString);
                if (async) {
                    callback(dataSet);
                }
            });

            return dataSet;
        },

        executeScalar: function (sql, params, callback) {
            var async = callback ? true : false;
            var dataIn = { sql: sql, page: 1, pageSize: 0 }

            var value = null;


            validateAndExecute(arguments, dataIn, async, function (jString) {
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
        },

        executeQuery: function (sql, param, callback) {

            var async = callback ? true : false;
            var ret = null;
            var dataIn = { sql: sql };
            if (param) {

                dataIn.param = param;
            }


            validateAndExecuteQuery(arguments, dataIn, async, function (jString) {
                ret = JSON.parse(jString);
                if (async) {
                    callback(ret);
                }
            });

            return ret;
        },

        /* Execute Multiple Non Queries 
          * @param sqls   - array of string sql
          * @param params - array of sqlParams array
          * @param callback - callback function execute on complete
          * @return array of result per query (1 0 or -1)
        */
        executeQueries: function (sqls, params, callback) {
            var async = callback ? true : false;
            var ret = null;
            var dataIn = { sql: sqls.join("/"), param: params };
            //if (param) {
            //    dataIn.param = params;
            //}

            validateAndExecuteQueries(arguments, dataIn, async, function (jString) {
                ret = JSON.parse(jString);
                if (async) {
                    callback(ret);
                }
            });

            return ret;
        },




    };

    //Sql Parameter Class
    function SqlParam(name, value) {
        this.name = name;
        this.value = value;
        this.isDate = false;
        this.isByteArray = false;
    };
    SqlParam.prototype.setIsDate = function (isDate) {
        this.isDate = isDate;
    };
    SqlParam.prototype.setIsByteArray = function (isByteArray) {
        this.isByteArray = isByteArray;
    };

    //****************************************************************//
    /*                          DATASET                               */
    //****************************************************************//

    function DataSet() {
        this.tables = [];
        this.total = 0;
    };

    DataSet.prototype.toJson = function (jsonString) {
        var tables = jsonString;
        if (typeof (jsonString) == "string")
            tables = JSON.parse(jsonString);

        tables = $.isArray(tables) ? tables : [tables];

        for (var i = 0; i < tables.length; i++) {
            var dt = new VIS.DB.DataTable();
            dt.toJson(tables[i]);
            this.tables.push(dt);
        }
        tables = null;
        return this;
    };

    DataSet.prototype.getTable = function (index) {

        if (index > -1 && index < this.tables.length) {
            return this.tables[index];
        }
        return null;
    };

    DataSet.prototype.getTables = function () {
        return this.tables;
    };

    DataSet.prototype.dispose = function () {
        for (var i = this.tables.length; i > 0; i--) {
            this.tables.pop().dispose();
        }
        this.tables.length = 0;
        this.tables = null;
    };

    function DataTable() {
        this.count = 0; //total rows in page
        this.totalPage = 0; //total number of pages
        this.page = 1; //current page index
        this.columns = []; // tables column
        this.rows = []; // rows of column
        this.totalRecord = 0; // total record 
        this.columnsName = [];

    };

    DataTable.prototype.toJson = function (js) {
        if (js) {
            this.totalRecord = js.records;
            this.totalPage = js.total;
            this.page = js.page;

            if (js.columns) {
                for (var col = 0; col < js.columns.length; col++) {

                    this.columnsName[col] = js.columns[col].name;
                }

                for (var row = 0; row < js.rows.length; row++) {
                    this.rows.push(new VIS.DB.DataRow(js.rows[row], this));
                }



                this.columns = js.columns;
            }

            this.count = this.rows.count;
        }

        js = null;
    };

    DataTable.prototype.getRowCount = function () {
        return this.rows.length;
    };

    DataTable.prototype.getColumnCount = function () {
        return this.columns.length;
    };

    DataTable.prototype.getRow = function (index) {
        if (index > -1 && index < this.rows.length) {
            return this.rows[index];
        }
        return null;
    };

    DataTable.prototype.getRows = function () {
        ///Get all row of table json raw string
        return this.rows;

    };

    DataTable.prototype.getColumnsName = function () {
        return this.columnsName;
    };

    DataTable.prototype.dispose = function () {

        this.count = 0; //total rows in page
        this.totalPage = 0; //total number of pages
        this.page = 1; //current page index
        this.columns.length = 0;
        this.columns = null;
        while (this.rows.length > 0) {
            this.rows.pop().dispose();
        }
        this.rows.length = 0;
        this.rows = null;
        this.totalRecord = 0; // total record
    };

    function DataRow(row, table) {
        this.cells = row.cells;
        this.id = row.id;
        this.parent = table;
    };

    DataRow.prototype.getCell = function (sel) {

        var index = -1;
        var cellName = "";
        if (isNaN(sel)) { //by name
            sel = sel.toLowerCase();
            index = this.parent.columnsName.indexOf(sel);
            cellName = sel;
        } else {
            index = sel;
            cellName = this.parent.columnsName[index];
        }

        var cellInfo = this.parent.columns[index];
        var dataType = cellInfo.type;
        var value = this.cells[cellName];
        if (dataType == "system.datetime") {
            //value = Date.parse(value);
        }
        return value;
    };

    DataRow.prototype.getJSCells = function () {
        //return jsin string cells [raw format]
        return this.cells;
    };

    DataRow.prototype.dispose = function () {
        this.cells.length = 0;
        this.cells = null;
        this.id = null;
        this.parent = null;
    };

    /*
      DataReader
    */

    function DataReader() {
        this.rowIndex = -1;
        this.tables = [];
        this.curTable;
        return this;
    };
    DataReader.prototype.toJson = function (jsonString) {
        var tables = JSON.parse(jsonString);
        tables = $.isArray(tables) ? tables : [tables];
        for (var i = 0; i < tables.length; i++) {
            var dt = new VIS.DB.DataTable();
            dt.toJson(tables[i]);
            this.tables.push(dt);
            if (i === 0) {
                this.curTable = this.tables[i];
            }
        }
        tables = null;
        return this;
    };
    DataReader.prototype.read = function () {
        if (!this.curTable)
            return false;
        if (++this.rowIndex >= this.curTable.getRowCount())
            return false;
        return true;
    };
    DataReader.prototype.getCell = function (col) {
        return this.curTable.getRow(this.rowIndex).getCell(col);
    };
    DataReader.prototype.get = function (col) {
        return this.curTable.getRow(this.rowIndex).getCell(col);
    };
    DataReader.prototype.getString = function (col) {
        var val = this.curTable.getRow(this.rowIndex).getCell(col);
        if (val == null)
            return "";
        return val.toString();
    };
    DataReader.prototype.getInt = function (col) {
        var val = this.curTable.getRow(this.rowIndex).getCell(col);
        if (val == null)
            return 0;
        else return parseInt(val);
    };
    DataReader.prototype.getDecimal = function (col) {
        var val = this.curTable.getRow(this.rowIndex).getCell(col);
        if (val == null)
            return 0;
        else return parseFloat(val);
    };
    DataReader.prototype.getDateTime = function (col) {
        return this.curTable.getRow(this.rowIndex).getCell(col);
    };
    DataReader.prototype.setTableIndex = function (index) {
        this.rowIndex = 0;
        this.curTable = this.tables[index]
    };
    DataReader.prototype.close = function () {
    };
    DataReader.prototype.dispose = function () {
        this.curTable = null;
        for (var i = this.tables.length; i > 0; i--) {
            this.tables.pop().dispose();
        }
        this.tables.length = 0;
        this.tables = null;
        this.rowIndex = null;
    };

    /******************** END ****************************/


    function TimeUtil() {

    };

    TimeUtil.prototype.max = function (d1, d2) {
        if (d1 == null)
            return d2;
        if (d2 == null)
            return d1;
        if (d1 > d2)
            return d1;
        return d2;
    };


    function CultureSeparator() {
        this.List = {
            'sq': { 'decimalSeparator': false },      // Albanian (Albania)
            'ar': { 'decimalSeparator': true },      // Arabic            
            'bg': { 'decimalSeparator': false },      // Bulgarian(Bulgaria)
            'be': { 'decimalSeparator': false },      // Byelorussian (Belarus)           
            'ca': { 'decimalSeparator': false },      // Catalan (Spain)
            'zh': { 'decimalSeparator': true },       // Chinese (China)
            'zh-HK': { 'decimalSeparator': true },    // Chinese (Hong Kong)
            'zh-CN': { 'decimalSeparator': true },    // chineese simple
            'zh-TW': { 'decimalSeparator': true },    // Chinese (Taiwan)
            'hr': { 'decimalSeparator': true },       // Croatian (Croatia)
            'cs': { 'decimalSeparator': false },      // Czech (Czech Republic)
            'da': { 'decimalSeparator': false },      // Danish (Denmark)
            'nl': { 'decimalSeparator': false },      // Dutch (Netherlands)            
            'en': { 'decimalSeparator': true },       // English
            'en-AU': { 'decimalSeparator': true },    // English (Australia)
            'en-CA': { 'decimalSeparator': true },    // English (Canada)
            'en-IN': { 'decimalSeparator': true },    // English (India)
            'en-NZ': { 'decimalSeparator': true },    // English (New Zealand)
            'en-ZA': { 'decimalSeparator': true },    // English (South Africa)
            'en-US': { 'decimalSeparator': true },    // English (USA)
            'en-GB': { 'decimalSeparator': true },    // English (United Kingdom)
            'et': { 'decimalSeparator': false },      // Estonian (Estonia)
            'fi': { 'decimalSeparator': false },      // Finnish (Finland)
            'fr': { 'decimalSeparator': false },      // French 
            'fr-CA': { 'decimalSeparator': false },   // French (Canada)
            'fr-FR': { 'decimalSeparator': false },   // French (France)
            'fr-CH': { 'decimalSeparator': false },   // French (Switzerland)
            'de': { 'decimalSeparator': false },      // German
            'de-AT': { 'decimalSeparator': false },   // German (Austria)
            'de-DE': { 'decimalSeparator': false },   // German (Germany)
            'de-LI': { 'decimalSeparator': false },   // German (Luxembourg)
            'de-CH': { 'decimalSeparator': false },   // German (Switzerland)
            'el': { 'decimalSeparator': false },      // Greek (Greece)
            'he': { 'decimalSeparator': true },       // Hebrew (Israel)
            'hi': { 'decimalSeparator': true },       // Hindi (India)
            'hu': { 'decimalSeparator': false },      // Hungarian (Hungary)
            'is': { 'decimalSeparator': false },      // Icelandic (Iceland)
            'it': { 'decimalSeparator': false },      // Italian
            'it-IT': { 'decimalSeparator': false },   // Italian (Italy)
            'it-CH': { 'decimalSeparator': false },   // Italian (Switzerland)
            'ja': { 'decimalSeparator': true },       // Japanese (Japan)
            'ko': { 'decimalSeparator': true },       // Korean (South Korea)
            'lv': { 'decimalSeparator': false },      // Latvian (Lettish) (Latvia)
            'lt': { 'decimalSeparator': false },      // Lithuanian (Lithuania)
            'mk': { 'decimalSeparator': false },      // Macedonian (Macedonia)
            'nb': { 'decimalSeparator': false },      // Norwegian (Norway)
            'nn': { 'decimalSeparator': false },      // Norwegian
            'pl': { 'decimalSeparator': false },      // Polish (Poland)
            'pt': { 'decimalSeparator': false },      // Portuguese
            'pt-BR': { 'decimalSeparator': false },   // Portuguese (Brazil)
            'pt-PT': { 'decimalSeparator': false },   // Portuguese (Portugal)
            'ro': { 'decimalSeparator': false },      // Romanian (Romania)
            'ru': { 'decimalSeparator': false },      // Russian (Russia)
            'sr': { 'decimalSeparator': false },      // Serbian (Yugoslavia)
            'sh': { 'decimalSeparator': false },      // Serbo-Croatian (Yugoslavia)
            'sk': { 'decimalSeparator': false },      // Slovak (Slovakia)
            'sl': { 'decimalSeparator': false },      // Slovenian (Slovenia)
            'es': { 'decimalSeparator': false },      // Spanish
            'es-AR': { 'decimalSeparator': false },   // Spanish (Argentina)
            'es-CL': { 'decimalSeparator': false },   // Spanish (Chile)
            'es-CO': { 'decimalSeparator': false },   // Spanish (Colombia)
            'es-CR': { 'decimalSeparator': false },   // Spanish (Costa Rica)
            'es-HN': { 'decimalSeparator': false },   // Spanish (Honduras)
            'es-MX': { 'decimalSeparator': false },   // Spanish (Mexico)
            'es-PE': { 'decimalSeparator': false },   // Spanish (Peru)
            'es-ES': { 'decimalSeparator': false },   // Spanish (Spain)
            'es-US': { 'decimalSeparator': false },   // Spanish (United States)
            'es-UY': { 'decimalSeparator': false },   // Spanish (Uruguay)
            'es-VE': { 'decimalSeparator': false },   // Spanish (Venezuela)
            'sv': { 'decimalSeparator': false },      // Swedish (Sweden)
            'th': { 'decimalSeparator': true },       // Thai (Thailand)
            'tr': { 'decimalSeparator': false },      // Turkish (Turkey)
            'uk': { 'decimalSeparator': false },      // Ukrainian (Ukraine)
            'vi': { 'decimalSeparator': false },      // Vietnamese


        };
    };

    CultureSeparator.prototype.isDecimalSeparatorDot = function (value) {
        if (value) {
            if (this.List[value] != undefined) {
                return this.List[value].decimalSeparator;
            }
        }
        return null;
    };







    VIS.DB.DataSet = DataSet;
    VIS.DB.DataTable = DataTable;
    VIS.DB.DataRow = DataRow;
    VIS.DB.SqlParam = SqlParam;
    VIS.DB.DataReader = DataReader;
    VIS.MessageFormat = MessageFormat;
    VIS.TimeUtil = TimeUtil;
    VIS.CultureSeparator = CultureSeparator;

}(VIS, jQuery));









; (function ($, VIS) {


    var isoDateRegx = /(\d{4})-(\d{2})-(\d{2})T(\d{2})\:(\d{2})\:(\d{2})/;
    var Level = VIS.Logging.Level;
    var baseUrl = VIS.Application.contextUrl;
    var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";
    var executeReader = function (sql, param, callback, isEncrypt) {
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
        }, isEncrypt);
        return dr;
    };

    //DataSet String
    function getDataSetJString(data, async, callback, isEncrypt) {
        var result = null;
        //if (isEncrypt) {
        //    data.sql = VIS.secureEngine.encrypt(data.sql);
        //}
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

    /**
     *	Expression Evaluator
     *
     */
    VIS.Evaluator = {

        /**
         *  Parse String and add variables with @ to the list.
         *  @param list list to be added to
         *  @param parseString string to parse for variables
         */
        parseDepends: function (list, parseString) {

            if (parseString == null || parseString.length == 0)
                return;
            //	//log.fine(parseString);
            var s = parseString;
            //  while we have variables 
            while (s.indexOf("@") != -1) {
                var pos = s.indexOf("@");
                s = s.substring(pos + 1);
                pos = s.indexOf("@");
                if (pos == -1)
                    continue;	//	error number of @@ not correct......................
                var variable = s.substring(0, pos);
                s = s.substring(pos + 1);
                //	//log.fine(variable);
                if (list.indexOf(variable) < 0)
                    list[list.length] = variable;
            }
        },

        /**
         *	Evaluate Logic.
         *  <code>
         *	format		:= <expression> [<logic> <expression>]
         *	expression	:= @<context>@<exLogic><value>
         *	logic		:= <|> | <&>
         *  exLogic		:= <=> | <!> | <^> | <<> | <>>
         *
         *	context		:= any global or window context
         *	value		:= strings can be with ' or "
         *	logic operators	:= AND or OR with the prevoius result from left to right
         *
         *	Example	'@AD_Table@=Test | @Language@=GERGER
         *  </code>
         *  @param source class[function] implementing get_ValueAsString(variable)
         *  @param logic logic string
         *  @return locic result
         */
        evaluateLogic: function (source, logic) {
            //	ConditionalStringTokenizer
            var st = new VIS.StringTokenizer(logic.trim(), "&|", true);
            // try {

            var it = st.countTokens();
            if (((it / 2) - ((it + 1) / 2)) == 0)		//	only uneven arguments
            {
                //log.severe("Logic does not comply with format "
                //    + "'<expression> [<logic> <expression>]' => " + logic);
                return false;
            }

            var retValue = this.evaluateLogicDouble(source, st.nextToken());
            while (st.hasMoreTokens()) {
                var logOp = st.nextToken().trim();
                var temp = this.evaluateLogicDouble(source, st.nextToken());
                if (logOp.equals("&"))
                    retValue = retValue && temp;
                else if (logOp.equals("|"))
                    retValue = retValue || temp;
                else {
                    //            //Common.ErrorLog.FillErrorLog("Evaluatot.EvaluateLogic()", "DynamicDisplay", "Logic operant '|' or '&' expected => " + logic, VAdvantage.Framework.Message.MessageType.ERROR);
                    //            //log.warning("Logic operant '|' or '&' expected => " + logic);
                    return false;
                }
            }
            return retValue;
            //}
            // catch (exception) {
            //    return false;
            // }
        },


        /**
         *	Evaluate	@context@=value or @context@!value or @context@^value.
         *  <pre>
         *	value: strips ' and " always (no escape or mid stream)
         *  value: can also be a context variable
         *  </pre>
         *  @param source class implementing get_ValueAsString(variable)
         *  @param logic logic tuple
         *	@return	true or false
         */
        evaluateLogicDouble: function (source, logic) {

            var st = new VIS.StringTokenizer(logic.trim(), "!=^><", true);

            if (st.countTokens() !== 3) {
                //log.warning("Logic tuple does not comply with format "
                //    + "'@context@=value' where operand could be one of '=!^><' => " + logic);

                return false;
            }
            //	First Part
            var first = st.nextToken().trim();					//	get '@tag@'
            var firstEval = first.trim();
            if (first.indexOf('@') != -1)		//	variable
            {
                first = first.replaceAll('@', ' ').trim(); 			//	strip 'tag'
                //firstEval = source.get_ValueAsString(first);		//	replace with it's value
                firstEval = source.getValueAsString(first);
                if (firstEval == null)
                    firstEval = "";
            }
            firstEval = firstEval.replaceAll('\'', ' ').replaceAll('"', ' ').trim();	//	strip ' and "
            //	Comperator
            var operand = st.nextToken();

            //	Second Part
            var second = st.nextToken();							//	get value
            var secondEval = second.trim();
            if (second.indexOf('@') != -1 && second[0] == '@' && second[second.length - 1] == '@') {
                second = second.replaceAll('@', ' ').trim();			// strip tag
                //secondEval = source.get_ValueAsString(second);		//	replace with it's value
                secondEval = source.getValueAsString(second);		//	replace with it's value
                if (secondEval == null)
                    secondEval = "";
            }
            secondEval = secondEval.replaceAll('\'', ' ').replaceAll('"', ' ').trim();	//	strip ' and "
            //	Handling of ID compare (null => 0)
            if (first.indexOf("_ID") != -1 && firstEval.length == 0)
                firstEval = "0";
            if (second.indexOf("_ID") != -1 && secondEval.length == 0)
                secondEval = "0";
            //handle null value 
            if (firstEval == "0" && (secondEval == "" || secondEval == "null"))
                secondEval = "0";
            if (firstEval.length == 0 && secondEval == "null")
                secondEval = "";


            //	Logical Comparison
            var result = this.evaluateLogicTuple(firstEval, operand, secondEval);

            //if (log.isLevelFinest())
            //    //log.finest(logic
            //        + " => \"" + firstEval + "\" " + operand + " \"" + secondEval + "\" => " + result);
            //
            return result;
        },

        /**
         *	Evaluate	@context@=value or @context@!value or @context@^value.
         *  <pre>
         *	value: strips ' and " always (no escape or mid stream)
         *  value: can also be a context variable
         *  </pre>
         *  @param source class implementing get_ValueAsString(variable)
         *  @param logic logic tuple
         *	@return	true or false
         */
        evaluateLogicTuple: function (value1, operand, value2) {

            if (value1 == null || operand == null || value2 == null)
                return false;



            var value1bd = 0;
            var value2bd = 0;
            try {
                if (!value1.startsWith("'") && !isNaN(value1))
                    value1bd = Number(value1);
                if (!value2.startsWith("'") && !isNaN(value2))
                    value2bd = Number(value2);
            }
            catch (exception) {
                value1bd = 0;
                value2bd = 0;
            }
            //
            if (operand.equals("=")) {
                if (value1bd != 0 && value2bd != 0)
                    return value1bd.compareTo(value2bd) == 0;
                return value1.compareTo(value2) == 0;
            }
            else if (operand.equals("<")) {
                if (value1bd != 0 && value2bd != 0)
                    return value1bd.compareTo(value2bd) < 0;
                return value1.compareTo(value2) < 0;
            }
            else if (operand.equals(">")) {
                if (value1bd != 0 && value2bd != 0)
                    return value1bd.compareTo(value2bd) > 0;
                return value1.compareTo(value2) > 0;
            }
            else //	interpreted as not
            {
                if (value1bd != 0 && value2bd != 0)
                    return value1bd.compareTo(value2bd) != 0;
                return value1.compareTo(value2) != 0;
            }
        },

        /**
         * 	Get Variables in string
         *	@param raw input
         *	@return variables
         */
        getVariables: function (raw) {
            var variables = [];
            var buf = new StringBuilder();
            var st = new VIS.StringTokenizer(raw, "@");
            while (st.hasMoreTokens()) {
                buf.append(st.nextToken());
                if (st.hasMoreTokens()) {
                    buf.append("?");
                    var temp = st.nextToken();

                    //now if the variable contains |, which means it is in the form of @var|alternative@, remove | the the part after
                    if (temp.indexOf("|") != -1)
                        temp = temp.substring(0, temp.indexOf("|"));

                    if (variables.indexOf(temp) === -1)
                        variables.push(temp);
                }
            }
            return variables;
        },	//	getVariable

        replaceVariables: function (raw, ctx, windowCtx) {
            var result = raw;
            var variables = [];
            this.parseDepends(variables, raw);
            //log.finest("The variables are:"+ variables);
            for (var i = 0; i < variables.length; i++) {
                var variable = variables[i];
                // Assume that variables will never contain newline
                if (variable.indexOf("\n") != -1)
                    break;
                var param = null;
                if (windowCtx != null)
                    param = windowCtx.get(variable);
                if (param == null)
                    param = ctx.getContext(variable);
                if ((param != null) && (param.length > 0)) {
                    var l = null;
                    try { l = new Number(param); } catch (e) { }
                    if (l != null)
                        result = result.replaceAll("@" + variable + "@", l.toString());
                    else {
                        var num = null;
                        try { num = new Double(param); } catch (e) { }
                        if (num != null)
                            result = result.replaceAll("@" + variable + "@", num.toString());
                        else
                            result = result.replaceAll("@" + variable + "@", param.replaceAll("'", "''"));
                    }
                }
                else {
                    result = result.replaceAll("@" + variable + "@", "NULL");
                }
            }
            return result;
        }
    };



    /**
     *	Query Descriptor.
     * 	Maintains QueryRestrictions (WHERE clause)
     *
     */
    VIS.Query = function (tableName, isORcondition) {
        if (tableName) {
            this.tableName = tableName;
        }
        else {
            this.tableName = "";
        }
        this.list = [];
        /**	Record Count				*/
        this.recordCount = 999999;
        /** New Record VIS.Query			*/
        this.newRecord = false;
        this.isORcondition = false;
        if (isORcondition && isORcondition == true) {
            this.isORcondition = true;
        }

        /** New Record String			*/
    };

    VIS.Query.prototype.addRestriction = function () {

        var r = null;
        if (arguments.length == 5) {
            r = new VIS.QueryRestriction(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4]);
            if (this.isORcondition)
                r.andCondition = false;
            if (!(arguments[3].toString().startsWith("-"))) {
                this.list.push(r);
            }
        }
        else if (arguments.length == 3) {
            r = new VIS.QueryRestriction(arguments[0], arguments[1], arguments[2], null, null);
            if (this.isORcondition)
                r.andCondition = false;
            this.list.push(r);
        }
        else if (arguments.length == 1) {

            var whereClause = arguments[0];
            if (typeof whereClause == "string") {
                if (whereClause == null || whereClause.trim().length == 0)
                    return;
                r = new VIS.QueryRestriction(whereClause);
                if (this.isORcondition)
                    r.andCondition = false;
                this.list.push(r);
                this.newRecord = whereClause.equals(VIS.Query.NEWRECORD);
            }
            else {
                r = whereClause;
                if (this.isORcondition)
                    r.andCondition = false;
                this.list.push(r);
            }
        }

        else {
            throw new Error("invalid arguments Addrestriction");
        }
        r = null;
    };

    VIS.Query.prototype.setTableName = function (tableName) {
        this.tableName = tableName;
    };

    VIS.Query.prototype.setRecordCount = function (count) {
        this.recordCount = count;
    };

    VIS.Query.prototype.setColumnName = function (index, ColumnName) {
        /// <summary>
        /// Set ColumnName of index
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="ColumnName">new column name</param>
        if (index < 0 || index >= this.list.length)
            return;
        this.list[index].columnName = ColumnName;
    };

    VIS.Query.prototype.getCode = function (index) {
        if (index < 0 || index >= this.list.length)
            return null;
        this.list[index].code;
    };

    VIS.Query.prototype.getInfo = function () {
        /// <summary>
        /// Get printable VIS.Query Info
        /// </summary>
        /// <returns></returns>

        var sb = "", r = null;
        if (this.tableName != null)
            sb += this.tableName + ": ";
        //
        for (var i = 0; i < this.list.length; i++) {
            r = this.list[i];
            if (i != 0)
                sb += r.andCondition ? " AND " : " OR ";
            //
            sb += r.getInfoName() + r.getInfoOperator() + r.getInfoDisplayAll();
        }
        return sb;
    };

    VIS.Query.prototype.getIsActive = function () {
        /// <summary>
        /// Restrictions are active
        /// </summary>
        /// <returns></returns>
        return this.list.length != 0;
    };

    VIS.Query.prototype.getIsNewRecordQuery = function () {
        /// <summary>
        ///	New Record VIS.Query
        /// </summary>
        /// <returns></returns>
        return this.newRecord;
    };

    VIS.Query.prototype.getRecordCount = function () {
        return this.recordCount;
    };

    VIS.Query.prototype.getWhereClause = function () {

        var index = (arguments.length == 1) ? arguments[0] : false;
        var sb = "", r = null;

        if (typeof (index) == "boolean" || isNaN(index)) {

            var qualified = index;
            if (qualified && (this.tableName == null || this.tableName.length == 0))
                qualified = false;
            //
            for (var i = 0; i < this.list.length; i++) {
                r = this.list[i];
                if (i != 0) {
                    sb += r.andCondition ? " AND " : " OR ";
                }
                if (qualified)
                    sb += r.getSQL(this.tableName);
                else
                    sb += r.getSQL(null);
            }
            if (sb.length > 0) {
                return " ( " + sb + " ) ";
            }
            return sb;
        }
        else {

            if (index >= 0 && index < this.list.Count) {
                r = this.list[index];
                sb += r.GetSQL(null);
            }
            return sb;

        }
    };

    VIS.Query.prototype.getRestrictionCount = function () {
        /// <summary>
        ///Get VIS.Query Restriction Count
        /// </summary>
        /// <returns></returns>
        return this.list.length;
    };

    VIS.Query.prototype.clear = function () {
        this.list = [];
    };

    VIS.Query.prototype.getRestrictions = function () {
        /// <summary>
        ///Get VIS.Query Restriction Count
        /// </summary>
        /// <returns></returns>
        return this.list;
    };

    VIS.Query.prototype.copyRestrictions = function (arrRes) {
        // for (var i = 0, j = arrRes; i < j; i++)
        this.list = this.list.concat(arrRes);
    };

    VIS.Query.prototype.addRangeRestriction = function () {

        var r = null;
        if (arguments.length == 3) {
            r = new VIS.QueryRestriction(arguments[0], arguments[1], arguments[2], null, null, null);
        }
        else {
            r = new VIS.QueryRestriction(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
        }
        this.list.push(r);

    };

    VIS.Query.prototype.getColumnName = function (index) {
        /// <summary>
        /// Get ColumnName of index
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>ColumnName</returns>
        if (index < 0 || index >= this.list.length)
            return null;
        return this.list[index].columnName;
    };

    VIS.Query.prototype.getTableName = function () {
        /// <summary>
        /// Get Table name
        /// </summary>
        /// <returns></returns>
        return this.tableName;
    };

    VIS.Query.prototype.getOperator = function (index) {
        if (index < 0 || index >= this.list.length)
            return null;
        return this.list[index].operator;
    };

    VIS.Query.prototype.getCodeTo = function (index) {
        /// <summary>
        /// Get Operator of index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        if (index < 0 || index >= this.list.Count)
            return null;
        return this.list[index].code_to;
    };

    VIS.Query.prototype.getInfoDisplay = function (index) {
        /// <summary>
        /// Get QueryRestriction Display of index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        if (index < 0 || index >= this.list.length)
            return null;
        return this.list[index].infoDisplay;
    };

    VIS.Query.prototype.getInfoDisplay_to = function (index) {
        /// <summary>
        ///Get TO QueryRestriction Display of index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        if (index < 0 || index >= this.list.length)
            return null;

        return this.list[index].infoDisplay_to;
    };

    VIS.Query.prototype.getInfoName = function (index) {
        /// <summary>
        ///Get Info Name
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        if (index < 0 || index >= this.list.length)
            return null;
        return this.list[index].infoName;
    };

    VIS.Query.prototype.getInfoOperator = function (index) {
        /// <summary>
        /// Get Info Operator
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        if (index < 0 || index >= this.list.length)
            return null;

        return this.list[index].getInfoOperator();
    };

    VIS.Query.prototype.getInfoDisplayAll = function (index) {
        /// <summary>
        ///Get Display with optional To
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        if (index < 0 || index >= _list.Count)
            return null;

        return this.list[index].getInfoDisplayAll();
    };

    VIS.Query.prototype.deepCopy = function () {
        var newQuery = new VIS.Query(this.tableName);
        for (var i = 0; i < this.list.length; i++) {
            newQuery.addRestriction(this.list[i]);
        }
        return newQuery;
    };

    VIS.Query.prototype.getNoRecordQuery = function (tableName, newRecord) {
        var query = new VIS.Query(tableName);
        if (newRecord) {
            query.AddRestriction(VIS.Query.NEWRECORD);
        }
        else {
            query.AddRestriction("1=2");
        }
        query.setRecordCount(0);
        return query;
    };

    VIS.Query.prototype.NEWRECORD = "2=3";

    VIS.Query.prototype.EQUAL = "=";
    /** Equal - 0		*/
    VIS.Query.prototype.EQUAL_INDEX = 0;
    /** Not Equal		*/
    VIS.Query.prototype.NOT_EQUAL = "!=";
    /** Like			*/
    VIS.Query.prototype.LIKE = " LIKE ";
    /** Not Like		*/
    VIS.Query.prototype.NOT_LIKE = " NOT LIKE ";
    /** Greater			*/
    VIS.Query.prototype.GREATER = ">";
    /** Greater Equal	*/
    VIS.Query.prototype.GREATER_EQUAL = ">=";
    /** Less			*/
    VIS.Query.prototype.LESS = "<";
    /** Less Equal		*/
    VIS.Query.prototype.LESS_EQUAL = "<=";
    /** Between			*/
    VIS.Query.prototype.BETWEEN = " BETWEEN ";
    /** Between - 8		*/
    VIS.Query.prototype.BETWEEN_INDEX = 8;
    /** IN			*/
    VIS.Query.prototype.IN = " IN ";
    /** NOT IN			*/
    VIS.Query.prototype.NOT_IN = " NOT IN ";

    /**	Operators for Strings				*/
    VIS.Query.prototype.OPERATORS = {
        "=": " = ",		//	0
        "!=": " != ",
        " LIKE ": " ~ ",
        " NOT LIKE ": " !~ ",
        ">": " > ",
        ">=": " >= ",	//	5
        "<": " < ",
        "<=": " <= ",
        " BETWEEN ": " >-< "	//	8
        //	,new ValueNamePair (IN,				" () "),
        //	new ValueNamePair (NOT_IN,			" !() ")			
    };
    VIS.Query.prototype.CVOPERATORS = {
        "=": " = ",		//	0
        "!=": " != ",
        ">": " > ",
        "<": " < ",
        //	,new ValueNamePair (IN,				" () "),
        //	new ValueNamePair (NOT_IN,			" !() ")			
    };


    /**	Operators for IDs					*/
    VIS.Query.prototype.OPERATORS_ID = {
        "=": " = ",		//	0
        "!=": " != "
        //	,new ValueNamePair (IN,				" IN "),			
        //	new ValueNamePair (NOT_IN,			" !() ")			
    };

    /**	Operators for IDs					*/
    VIS.Query.prototype.CVOPERATORS_ID = {
        "=": " = ",		//	0
        "!": " != "
        //	,new ValueNamePair (IN,				" IN "),			
        //	new ValueNamePair (NOT_IN,			" !() ")			
    };

    ///**	Operators for IDs					*/
    //VIS.Query.prototype.OPERATORS_DYNAMIC_ID = {
    //    "@#AD_User_ID@": VIS.Msg.getMsg("LoginUser")		//	0
    //    //new ValueNamePair (NOT_EQUAL,		" != ")
    //    //	,new ValueNamePair (IN,				" IN "),			
    //    //	new ValueNamePair (NOT_IN,			" !() ")			
    //};
    /**	Operators for Boolean					*/
    VIS.Query.prototype.OPERATORS_YN = {
        "=": " = "
    };

    VIS.Query.prototype.OPERATORS_DATE_DYNAMIC = {
        "0": "Today",
        "1": "lastxDays",		//	0
        "2": "lastxMonth",
        "3": "lastxYears"
        //,
        //"4": "xDaysAgo",
        //"5": "xMonthAgo",
        //"6": "xYearsAgo"
    };
    /**	Operators for IDs					*/
    VIS.Query.prototype.OPERATORS_DYNAMIC_ID = {

        "@#AD_User_ID@": "LoginUser"		//	0
        //new ValueNamePair (NOT_EQUAL,		" != ")
        //	,new ValueNamePair (IN,				" IN "),			
        //	new ValueNamePair (NOT_IN,			" !() ")			
    };



    VIS.Query.prototype.getEqualQuery = function (columnName, value) {
        var query = new VIS.Query();
        query.addRestriction(columnName, VIS.Query.prototype.EQUAL, value);
        query.setRecordCount(1);	//	guess
        return query;
    };


    /**
	 * 	Query Restriction
	 */
    VIS.QueryRestriction = function () {

        this.directWhereClause = null;
        /**	Column Name			*/
        this.columnName;
        /** Name				*/
        this.infoName;
        /** Operator			*/
        this.operator;
        /** sql Where Code		*/
        this.code;
        /** Info				*/
        this.infoDisplay;
        /** sql Where Code To	*/
        this.code_to;
        /** Info To				*/
        this.infoDisplay_to;
        /** And/Or Condition	*/
        this.andCondition = true;

        switch (arguments.length) {
            case 5:
                this.fnArgu5(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4]);
                break;
            case 6:
                this.fnArgu6(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
                break;
            case 9:
                this.fnArgu9(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8]);
                break;
            case 1:
                this.fnArgu1(arguments[0]);
                break;
            case 0:
                break;
            default:
                throw new Error("Invalid arguments");

        }
    };

    /**
	 * 	Restriction
	 * 	@param columnName ColumnName
	 * 	@param operator Operator, e.g. = != ..
	 * 	@param code Code, e.g 0, All%
	 *  @param infoName Display Name
	 * 	@param infoDisplay Display of Code (Lookup)
	 */
    VIS.QueryRestriction.prototype.fnArgu5 = function (columnName, oprator, code, infoName, infoDisplay) {
        var ColumnName = columnName.trim();
        var InfoName;
        if (infoName != null)
            InfoName = infoName;
        else
            InfoName = ColumnName;
        //
        Operator = oprator;
        var Code;
        //	Boolean
        if (code != null && typeof code == "Boolean") {
            Code = (new Boolean(code) == true) ? "Y" : "N";
        }
        //else if (code != null && code.GetType() == typeof(KeyNamePair))
        //  Code = ((KeyNamePair)code).GetKey();
        //else if (code != null && code.GetType() == typeof(ValueNamePair))
        //  Code = ((ValueNamePair)code).GetValue();
        else
            Code = code;
        ///	clean code
        if (Code != null && typeof Code == "string") {
            if (Code.startsWith("'"))
                Code = Code.substring(1);
            if (Code.endsWith("'"))
                Code = Code.substring(0, Code.length - 2);
        }

        var InfoDisplay;
        if (infoDisplay != null)
            InfoDisplay = infoDisplay.toString().trim();
        else if (code != null)
            InfoDisplay = code;

        this.columnName = ColumnName;
        /** Name				*/
        this.infoName = InfoName;
        /** Operator			*/
        this.operator = Operator
        /** sql Where Code		*/
        this.code = Code;
        /** Info				*/
        this.infoDisplay = InfoDisplay;
    };

    /**
	 * 	Range Restriction (BETWEEN)
	 * 	@param columnName ColumnName
	 * 	@param code Code, e.g 0, All%
	 * 	@param code_to Code, e.g 0, All%
	 *  @param infoName Display Name
	 * 	@param infoDisplay Display of Code (Lookup)
	 * 	@param infoDisplay_to Display of Code (Lookup)
	 */
    VIS.QueryRestriction.prototype.fnArgu6 = function (columnName, code, code_to, infoName, infoDisplay, infoDisplay_to) {
        this.fnArgu5(columnName, VIS.Query.prototype.BETWEEN, code, infoName, infoDisplay);

        var Code_to = code_to;
        if (typeof Code_to == "string") {
            if (Code_to.startsWith("'"))
                Code_to = Code_to.substring(1);
            if (Code_to.endsWith("'"))
                Code_to = Code_to.substring(0, Code_to.length - 2);
        }
        //	InfoDisplay_to
        var InfoDisplay_to;
        if (infoDisplay_to != null)
            InfoDisplay_to = infoDisplay_to.trim();
        else if (Code_to != null)
            InfoDisplay_to = Code_to.toString();

        this.code_to = Code_to;
        this.infoDisplay_to = InfoDisplay_to;
    };

    /**
	 * 	add restirction - Internal Use
	 *	@param columnName
	 *	@param code
	 *	@param code_to
	 *	@param infoName
	 *	@param infoDisplay
	 *	@param infoDisplay_to
	 *	@param operator
	 *	@param directWhereClause
	 *	@param andCondition
	 */
    VIS.QueryRestriction.prototype.fnArgu9 = function (columnName, code, code_to, infoName, infoDisplay, infoDisplay_to, ooperator, directWhereClause, andCondition) {
        this.columnName = columnName;
        this.infoName = infoName;
        this.code = code;
        this.code_to = code_to;
        this.infoName = infoName;
        this.infoDisplay = infoDisplay;
        this.infoDisplay_to = infoDisplay_to;
        this.operator = ooperator;
        this.directWhereClause = directWhereClause;
        this.andCondition = andCondition;
    }

    VIS.QueryRestriction.prototype.fnArgu1 = function (whereClause) {
        this.directWhereClause = whereClause;
    };

    VIS.QueryRestriction.prototype.getInfoName = function () {
        /// <summary>
        /// Get Info Name
        /// </summary>
        /// <returns></returns>
        return this.infoName;
    };

    VIS.QueryRestriction.prototype.getInfoOperator = function () {
        /// <summary>
        /// Get Info Operator
        /// </summary>
        /// <returns></returns>
        if (this.operator in VIS.Query.OPERATORS) {
            return VIS.Query.OPERATORS[this.operator];
        }
        return this.operator;
    };

    VIS.QueryRestriction.prototype.getInfoDisplayAll = function () {
        /// <summary>
        ///	Get Display with optional To
        /// </summary>
        /// <returns></returns>
        if (this.infoDisplay_to == null)
            return this.infoDisplay;
        var sb = this.infoDisplay;
        sb += " - " + InfoDisplay_to;
        return sb;
    };

    VIS.QueryRestriction.prototype.getSQL = function (tableName) {
        /// <summary>
        /// Return sql construct for this restriction
        /// </summary>
        /// <param name="tableName">optional table name</param>
        /// <returns>sql WHERE construct</returns>

        if (this.directWhereClause != null)
            return this.directWhereClause;
        //
        var sb = "";

        // opening parenthesis for case insensitive search
        //if (Code instanceof String)
        if (typeof this.code == "string" && isNaN(this.code)) {
                sb += " UPPER( ";
        }
        if (tableName != null && tableName.length > 0) {
            //	Assumes - REPLACE(INITCAP(variable),'s','X') or UPPER(variable)
            var pos = this.columnName.lastIndexOf('(') + 1;	//	including (
            var end = this.columnName.indexOf(')');
            //	We have a Function in the ColumnName
            if (pos != -1 && end != -1)
                sb += this.columnName.substring(0, pos)
                    + tableName + "." + this.columnName.substring(pos, (pos + (end - pos)))
                    + this.columnName.substring(end);
            else
                sb += tableName + "." + this.columnName;
        }
        else {
            sb += this.columnName;
        }

        // closing parenthesis for case insensitive search
        if (typeof this.code == "string" && isNaN(this.code)) {
            sb += " ) ";
        }


        //	NULL Operator
        if (this.code == null || "NULL".equals(this.code.toString().toUpper()) || ("NullValue").toUpper().equals(this.code.toString().toUpper())) {
            if (this.operator.equals(VIS.Query.prototype.EQUAL))
                sb += " IS NULL ";
            else
                sb += " IS NOT NULL ";
        }
        else {
            sb += this.operator + " ";
            if (VIS.Query.prototype.IN.equals(this.operator) || VIS.Query.prototype.NOT_IN.equals(this.operator)) {
                sb += "(";
            }

            if (this.code instanceof Date || (this.code && (isoDateRegx.test(this.code.toString())))) {//  endsWith('Z') && this.code.toString().contains('T')))) {
                sb += VIS.DB.to_date(this.code, false);
            }

            else if ("string" == typeof this.code) {
                if (isNaN(this.code)) {
                    sb += " UPPER( ";
                    sb += VIS.DB.to_string(this.code.toString());
                    sb += " ) ";
                }
                else {
                    sb += VIS.DB.to_string(this.code.toString());
                }
            }

            else
                sb += this.code;

            //	Between
            if (VIS.Query.prototype.BETWEEN.equals(this.operator)) {
                //	if (Code_to != null && InfoDisplay_to != null)
                sb += " AND ";

                if (this.code_to instanceof Date || (this.code_to && (isoDateRegx.test(this.code_to.toString())))) {//  endsWith('Z') ||  this.code.toString().contains('T')))) {
                    sb += VIS.DB.to_date(this.code_to, false);
                }

                else if (typeof (this.code_to) == "string") {
                    sb += VIS.DB.to_string(this.code_to.toString());
                   
                }

                else
                    sb += this.code_to;
            }
            else if (VIS.Query.prototype.IN.equals(this.operator) || VIS.Query.prototype.NOT_IN.equals(this.operator))
                sb += ")";
        }
        return sb;
    };


    /***************************************************************/
    //     AccessSqlParser
    /**************************************************************/

    /**
     *	Parse FROM in SQL WHERE clause
     *
     */
    function AccessSqlParser(sql) {
        this.log = VIS.Logging.VLogger.getVLogger("AccessSqlParser");
        /**	Original SQL			*/
        this.sqlOriginal;
        /**	SQL Selects			*/
        this.sql = [];
        /**	List of Arrays		*/
        this.tableInfo = [];
        this.setSql(sql);
    }
    AccessSqlParser.prototype.FROM = " FROM ";
    AccessSqlParser.prototype.FROM_LENGTH = AccessSqlParser.prototype.FROM.length;;
    AccessSqlParser.prototype.WHERE = " WHERE ";
    AccessSqlParser.prototype.ON = " ON ";

    AccessSqlParser.prototype.setSql = function (sql) {
        if (sql == null)
            throw new IllegalArgumentException("No SQL");
        this.sqlOriginal = sql;
        var index = this.sqlOriginal.indexOf("\nFROM ");
        if (index != -1)
            this.sqlOriginal = this.sqlOriginal.replace("\nFROM ", this.FROM);
        index = this.sqlOriginal.indexOf("\nWHERE ");
        if (index != -1)
            this.sqlOriginal = this.sqlOriginal.replace("\nWHERE ", this.WHERE);
        //
        this.parse();
    }	//
    AccessSqlParser.prototype.getSql = function () {
        return this.sqlOriginal;
    }
    AccessSqlParser.prototype.setSql = function (sql) {
        if (sql == null)
            throw new IllegalArgumentException("No SQL");
        this.sqlOriginal = sql;
        var index = this.sqlOriginal.indexOf("\nFROM ");
        if (index != -1)
            this.sqlOriginal = this.sqlOriginal.replaceAll("\nFROM ", this.FROM);
        index = this.sqlOriginal.indexOf("\nWHERE ");
        if (index != -1)
            this.sqlOriginal = this.sqlOriginal.replaceAll("\nWHERE ", this.WHERE);
        //
        this.parse();
    }
    AccessSqlParser.prototype.getSql = function () {
        return this.sqlOriginal;
    }
    AccessSqlParser.prototype.parse = function () {
        if ((this.sqlOriginal == null) || (this.sqlOriginal.length == 0))
            throw new IllegalArgumentException("No SQL");
        //
        //	if (CLogMgt.isLevelFinest())
        //		log.fine(m_sqlOriginal);
        this.getSelectStatements();
        //	analyze each select
        for (var i = 0; i < this.sql.length; i++) {
            var info = this.getTableInfo(this.sql[i].trim());
            this.tableInfo.push(info);
        }
        //
        //if (CLogMgt.isLevelFinest())
        //    log.fine(toString());
        return this.tableInfo.length > 0;
    }
    AccessSqlParser.prototype.getSelectStatements = function () {
        var sqlIn = [this.sqlOriginal];
        var sqlOut = null;
        try {
            sqlOut = this.getSubSQL(sqlIn);
        }
        catch (e) {
            this.log.log(VIS.Logging.Level.SEVERE, this.sqlOriginal, e);
            throw new IllegalArgumentException(this.sqlOriginal);
        }
        //	a sub-query was found
        while (sqlIn.length != sqlOut.length) {
            sqlIn = sqlOut;
            try {
                sqlOut = this.getSubSQL(sqlIn);
            }
            catch (e) {
                this.log.log(VIS.Logging.Level.SEVERE, this.sqlOriginal, e);
                throw new IllegalArgumentException(sqlOut.length + ": " + m_sqlOriginal);
            }
        }
        this.sql = sqlOut;
    }
    AccessSqlParser.prototype.getSubSQL = function (sqlIn) {
        var list = [];
        var sql = "";
        for (var i = 0; i < sqlIn.length; i++) {
            sql = sqlIn[i];

            var index = sql.indexOf("(SELECT ", 7);
            while (index != -1) {
                var endIndex = index + 1;
                var parenthesisLevel = 0;
                //	search for the end of the sql
                while (endIndex++ < sql.length) {
                    var c = sql.charAt(endIndex);
                    if (c == ')') {
                        if (parenthesisLevel == 0)
                            break;
                        else
                            parenthesisLevel--;
                    }
                    else if (c == '(')
                        parenthesisLevel++;
                }
                var subSQL = sql.substring(index, endIndex + 1);
                list.push(subSQL);
                //	remove inner SQL (##)
                sql = sql.substring(0, index + 1) + "##"
                    + sql.substring(endIndex);
                index = sql.indexOf("(SELECT ", 7);
            }
            list.push(sql);	//	last SQL
        }
        //String[] retValue = new String[list.size()];
        //list.toArray(retValue);
        return list;
    }
    AccessSqlParser.prototype.getTableInfo = function (sql) {
        if (isNaN(sql)) {
            var list = [];
            //	remove ()
            if (sql.startsWith("(") && sql.endsWith(")"))
                sql = sql.substring(1, sql.length - 1);

            var fromIndex = sql.indexOf(this.FROM);
            if (fromIndex != sql.lastIndexOf(this.FROM))
                this.log.log(VIS.Logging.Level.FINE, "More than one FROM clause - " + sql);
            while (fromIndex != -1) {
                var from = sql.substring(fromIndex + this.FROM_LENGTH);
                //Lakhwinder
                //var index = from.lastIndexOf(this.WHERE);	//	end at where
                var index = from.indexOf(this.WHERE);	//	end at where
                if (index != -1)
                    from = from.substring(0, index);
                from = from.replaceAll(" AS ", " ");
                from = from.replaceAll(" as ", " ");
                from = from.replaceAll(" INNER JOIN ", ", ");
                from = from.replaceAll(" LEFT OUTER JOIN ", ", ");
                from = from.replaceAll(" RIGHT OUTER JOIN ", ", ");
                from = from.replaceAll(" FULL JOIN ", ", ");
                //	Remove ON clause - assumes that there is no IN () in the clause
                index = from.indexOf(this.ON);
                while (index != -1) {
                    var indexClose = from.indexOf(')');		//	does not catch "IN (1,2)" in ON
                    var indexNextOn = from.indexOf(this.ON, index + 4);
                    if (indexNextOn != -1)
                        indexClose = from.lastIndexOf(')', indexNextOn);
                    if (indexClose != -1)
                        from = from.substring(0, index) + from.substring(indexClose + 1);
                    else {
                        this.log.log(VIS.Logging.Level.SEVERE, "Could not remove ON " + from);
                        break;
                    }
                    index = from.indexOf(this.ON);
                }
                //			log.fine("getTableInfo - " + from);
                var tableST = new VIS.StringTokenizer(from, ",");
                while (tableST.hasMoreTokens()) {
                    var tableString = tableST.nextToken().trim();
                    var synST = new VIS.StringTokenizer(tableString, " ");
                    var tableInfo = null;
                    if (synST.countTokens() > 1)
                        tableInfo = new TableInfo(synST.nextToken(), synST.nextToken());
                    else
                        tableInfo = new TableInfo(tableString);
                    //				log.fine("getTableInfo -- " + tableInfo);
                    list.push(tableInfo);
                }
                //
                sql = sql.substring(0, fromIndex);
                fromIndex = sql.lastIndexOf(this.FROM);
            }
            //nfo[] retValue = new TableInfo[list.size()];
            //list.toArray(retValue);
            return list;
        }
        else {
            var index = sql;
            if ((index < 0) || (index > this.tableInfo.length))
                return null;
            var retValue = this.tableInfo[index];
            return retValue;
        }
    }	//	getTableInfo

    AccessSqlParser.prototype.getSqlStatement = function (index) {
        if ((index < 0) || (index > this.sql.length))
            return null;
        return this.sql[index];
    }
    AccessSqlParser.prototype.getNoSqlStatments = function () {
        if (this.sql == null)
            return 0;
        return this.sql.length;
    }	//	getNoSqlStatments
    AccessSqlParser.prototype.getMainSqlIndex = function () {
        if (this.sql == null)
            return -1;
        else if (this.sql.length == 1)
            return 0;
        for (var i = this.sql.length - 1; i >= 0; i--) {
            if (this.sql[i].charAt(0) != '(')
                return i;
        }
        return -1;
    }
    AccessSqlParser.prototype.getMainSql = function () {
        if (this.sql == null)
            return this.sqlOriginal;

        if (this.sql.length == 1)
            return this.sql[0];
        for (var i = this.sql.length - 1; i >= 0; i--) {
            if (this.sql[i].charAt(0) != '(')
                return this.sql[i];
        }
        return "";
    }	//	getMainSql
    AccessSqlParser.prototype.size = function () {
        return this.tableInfo.length;
    }


    /****************** END  **************************************/


    /*************************************************************
                    TableInfo
    **************************************************************/
    /**
	 * 	Table Info VO
	 */
    function TableInfo(tableName, synonym) {
        this.tableName = tableName;
        this.synonym = synonym;
    };

    TableInfo.prototype.getSynonym = function () {
        if (this.synonym == null)
            return "";
        return this.synonym;
    };
    TableInfo.prototype.getTableName = function () {
        return this.tableName;
    };


    /******************* END *************************************/

    VIS.TableInfo = TableInfo;
    VIS.AccessSqlParser = AccessSqlParser;


    /**
 *	Zoom Target identifier.
 *  Used in Zoom across (Where used) and Zoom into
 *	
 *  @author Jorg Janke
 *  @version $Id: ZoomTarget.java 8753 2010-05-12 17:34:49Z nnayak $
 */
    VIS.ZoomTarget = {

        log: VIS.Logging.VLogger.getVLogger("VIS.ZoomTarget"),


        /**
	 * 	Get the Zoom Into Target for a table.
	 *  
	 *  @param targetTableName for Target Table for zoom
	 *  @param curWindow_ID Window from where zoom is invoked
	 * 	@param targetWhereClause Where Clause in the format "Record_ID=<value>"
	 *  @param isSOTrx Sales contex of window from where zoom is invoked
	 */
        getZoomAD_Window_ID: function (targetTableName, curWindow_ID, targetWhereClause, isSOTrx) {

            var zoomWindow_ID = 0;
            var PO_zoomWindow_ID = 0;

            // Hard code for zooming to Production Resource Window		
            if (targetTableName.equals("M_Product")) {
                //var ProductType = null;
                //var ResourceGroup = null;
                //if(targetWhereClause!=null && !targetWhereClause.equals(""))
                //{
                //    var sql1 = " SELECT ProductType, ResourceGroup FROM M_Product "
                //                +" WHERE " + targetWhereClause;

                //    var rs = null;

                //    try
                //    {
                //        rs = VIS.DB.executeDataReader(sql1);
                //        if (rs.read())
                //        {
                //            ProductType = rs.getString(0);
                //            ResourceGroup = rs.getString(1);
                //        }
                //    }
                //    catch (e) {
                //        //log.log(Level.SEVERE, sql1, e);
                //    }
                //    finally {
                //        if(rs != null)
                //            rs.dispose();
                //    }

                //if(ProductType != null && ResourceGroup != null &&
                //        ProductType.equals(X_M_Product.PRODUCTTYPE_Resource)&& 
                //        (ResourceGroup.equals(X_M_Product.RESOURCEGROUP_Person) ||
                //                ResourceGroup.equals(X_M_Product.RESOURCEGROUP_Equipment)))
                //{
                //    var windowID = 0;
                //    var sql11 = " SELECT AD_Window_ID FROM AD_Window WHERE NAME LIKE 'Production Resource'";
                //    try
                //    {
                //        pstmt = DB.prepareStatement(sql11, (Trx) null);
                //        rs = pstmt.executeQuery();
                //        while(rs.next())
                //        {
                //            windowID= rs.getInt(1);
                //        }Query
                //    }
                //    catch (SQLException e) {
                //        log.log(Level.SEVERE, sql11, e);
                //    }
                //    finally {
                //        DB.closeResultSet(rs);
                //    DB.closeStatement(pstmt);
                //}

                // if(windowID !=0)
                //  return windowID;
                // }
                //}
            }

            // END hard code for Zooming into Production resource.

            // Find windows where the first tab is based on the table
            var sql = "VIS_78";

            var param = [];
            param[0] = new VIS.DB.SqlParam("@targetTableName", targetTableName);

            var dr = null;
            try {
                var index = 1;

                dr = executeReader(sql, param);

                if (dr.read()) {
                    zoomWindow_ID = dr.getInt(0);
                    PO_zoomWindow_ID = dr.getInt(1);
                }
            }
            catch (e) {
                this.log.log(VIS.Logging.Level.SEVERE, sql, e);
            }
            finally {
                if (dr != null)
                    dr.dispose();
                dr = null;
            }

            if (PO_zoomWindow_ID == 0)
                return zoomWindow_ID;

            if (PO_zoomWindow_ID != 0 && zoomWindow_ID != 0 && targetWhereClause != null && targetWhereClause.length != 0) {
                var ParentTable = null;
                var ind = targetTableName.indexOf("Line");
                if (ind != -1)
                    ParentTable = targetTableName.substring(0, ind);
                if (ParentTable != null) {
                    var sql3 = "VIS_79";

                    var param = [];
                    param[0] = new VIS.DB.SqlParam("@ParentTable", ParentTable);
                    param[1] = new VIS.DB.SqlParam("@targetTableName", targetTableName);
                    param[2] = new VIS.DB.SqlParam("@targetWhereClause", targetWhereClause);
                    param[3] = new VIS.DB.SqlParam("@ParentTable1", ParentTable);
                    param[4] = new VIS.DB.SqlParam("@ParentTable2", ParentTable);


                    try {
                        dr = executeReader(sql3, param);

                        if (dr.read())
                            isSOTrx = dr.getString(0).equals("Y");
                    }
                    catch (e) {
                        this.log(VIS.Logging.Level.SEVERE, sql3.toString(), e);
                    }
                    finally {
                        if (dr != null)
                            dr.dispose();
                        dr = null;
                    }
                }
            }

            var AD_Window_ID = 0;

            if (targetWhereClause != null && targetWhereClause.length != 0) {
                var zoomList = [];
                zoomList = this.getZoomTargets(targetTableName, curWindow_ID, targetWhereClause);
                if (zoomList != null && zoomList.length > 0)
                    AD_Window_ID = zoomList[0].Key;
            }

            if (AD_Window_ID != 0)
                return AD_Window_ID;

            if (isSOTrx)
                return zoomWindow_ID;

            return PO_zoomWindow_ID;
        },

        /**
	 *  Parse String and add columnNames to the list.
	 *  String should be of the format ColumnName=<Value> AND ColumnName2=<Value2>
	 *  @param list list to be added to
	 *  @param parseString string to parse for variables
	 */
        parseColumns: function (list, parseString) {
            if (parseString == null || parseString.length == 0)
                return;

            //	log.fine(parseString);
            var s = parseString;

            // Currently parsing algorithm does not handle parenthesis, IN clause or EXISTS clause
            if (s.contains(" EXISTS ") || s.contains(" IN ") || s.contains("(") || s.contains(")"))
                return;

            //  while we have columns
            while (s.indexOf("=") != -1) {
                var endIndex = s.indexOf("=");
                var clause = s.substring(0, endIndex);
                clause = clause.trim();

                var beginIndex = clause.lastIndexOf(' ', clause.length);
                var variable = clause.substring(beginIndex + 1);

                if (variable.indexOf(".") != -1) {
                    beginIndex = variable.indexOf(".") + 1;
                    variable = variable.substring(beginIndex, variable.length);
                }

                if (list.indexOf(variable) < 0)
                    list.push(variable);

                s = s.substring(endIndex + 1);
            }
        },   //  parseDepends

        /**
         *  Evaluate where clause
         *  @param columnValues columns with the values
         *  @param whereClause where clause
         *  @return true if where clause evaluates to true
         */
        evaluateWhereClause: function (columnValues, whereClause) {

            if (whereClause == null || whereClause.length == 0)
                return true;


            var s = whereClause;
            var result = true;

            // Currently parsing algorithm does not handle parenthesis, IN clause or EXISTS clause
            if (s.contains(" EXISTS ") || s.contains(" IN ") || s.contains("(") || s.contains(")"))
                return false;

            //  while we have variables
            while (s.indexOf("=") != -1) {
                var endIndex = s.indexOf("=");
                var beginIndex = s.lastIndexOf(' ', endIndex);

                var variable = s.substring(beginIndex + 1, endIndex);
                var operand1 = "";
                var operand2 = "";
                var operator = "=";

                if (variable.indexOf(".") != -1) {
                    beginIndex = variable.indexOf(".") + 1;
                    variable = variable.substring(beginIndex, variable.length);
                }

                for (var i = 0; i < columnValues.length; i++) {
                    if (variable.equals(columnValues[i].Name)) {
                        operand1 = columnValues[i].Value;
                        break;
                    }

                }

                s = s.substring(endIndex + 1);
                beginIndex = 0;
                endIndex = s.indexOf(' ');
                if (endIndex == -1)
                    operand2 = s.substring(beginIndex);
                else
                    operand2 = s.substring(beginIndex, endIndex);

                /* log.fine("operand1:"+operand1+ 
                        " operator:"+ operator +
                        " operand2:"+operand2); */
                if (!VIS.Evaluator.evaluateLogicTuple(operand1, operator, operand2)) {
                    result = false;
                    break;
                }
            }
            return result;
        },


        /*  Get the Zoom Across Targets for a table.
         *  
         *  @param targetTableName for Target Table for zoom
         *  @param curWindow_ID Window from where zoom is invoked
         *  @param targetWhereClause Where Clause in the format "WHERE Record_ID=?"
         *  @param params[] parameter to whereClause. Should be the Record_ID
         *
         */
        getZoomTargets: function (targetTableName, curWindow_ID, targetWhereClause, params) {
            if (arguments.length == 4) {
                if (params.length != 1)
                    return null;
                var record_ID = params[0];
                var whereClause = targetWhereClause.replace("?", record_ID.toString());
                whereClause = whereClause.replace("WHERE ", " ");
                this.log.fine("WhereClause : " + whereClause);
                targetWhereClause = whereClause;
            }

            /**	The Option List					*/
            var zoomList = [];
            var windowList = [];
            var columns = [];
            var zoom_Window_ID = 0;
            var PO_Window_ID = 0;
            var zoom_WindowName = "";
            var whereClause = "";
            var windowFound = false;


            /** Start Hard code for adding Maintain cost windows in the zoom across for product and work center */
            var WorkCenterCostWindowName = null;
            var WorkCenterWindowID = 0;
            var WorkCenterCostWindowID = 0;
            var ProductionResourceWindowID = 0;

            var sql11 = "VIS_80";

            var dr = null;
            try {
                dr = executeReader(sql11);
                while (dr.read()) {
                    var windowID = dr.getInt(0);
                    var windowName = dr.getString(1);
                    if (windowName.equals("Work Center")) {
                        WorkCenterWindowID = windowID;
                    }
                    else if (windowName.equals("Work Center Costs")) {
                        WorkCenterCostWindowID = windowID;
                        WorkCenterCostWindowName = windowName;
                    }
                    else if (windowName.equals("Production Resource")) {
                        ProductionResourceWindowID = windowID;
                    }

                }
            }
            catch (e) {
                this.log.log(VIS.Logging.Level.SEVERE, sql11, e);
            }
            finally {
                if (dr != null)
                    dr.dispose();
                dr = null;
            }

            //var costWindow = MWindow.get(Env.getCtx(), 344);
            //var cost = new KeyNamePair(costWindow.getAD_Window_ID(),costWindow.getName());
            ///var WcCost = new KeyNamePair(WorkCenterCostWindowID,WorkCenterCostWindowName);

            /** End Hard Code for product and work center window */

            // Find windows where the first tab is based on the table
            //var sql = "SELECT DISTINCT w.AD_Window_ID, w.Name, tt.WhereClause, t.TableName, " +
            //        "wp.AD_Window_ID, wp.Name, ws.AD_Window_ID, ws.Name "
            //    + "FROM AD_Table t "
            //    + "INNER JOIN AD_Tab tt ON (tt.AD_Table_ID = t.AD_Table_ID) ";
            //var baseLanguage = VIS.Env.isBaseLanguage(VIS.Env.getCtx(), "AD_Window");
            //if (baseLanguage) {
            //    sql += "INNER JOIN AD_Window w ON (tt.AD_Window_ID=w.AD_Window_ID)";
            //    sql += " LEFT OUTER JOIN AD_Window ws ON (t.AD_Window_ID=ws.AD_Window_ID)"
            //        + " LEFT OUTER JOIN AD_Window wp ON (t.PO_Window_ID=wp.AD_Window_ID)";
            //}
            //else {
            //    sql += "INNER JOIN AD_Window_Trl w ON (tt.AD_Window_ID=w.AD_Window_ID AND w.AD_Language=@para1)";
            //    sql += " LEFT OUTER JOIN AD_Window_Trl ws ON (t.AD_Window_ID=ws.AD_Window_ID AND ws.AD_Language=@para2)"
            //        + " LEFT OUTER JOIN AD_Window_Trl wp ON (t.PO_Window_ID=wp.AD_Window_ID AND wp.AD_Language=@para3)";
            //}
            //sql += "WHERE t.TableName = @para4"
            //    + " AND w.AD_Window_ID <> @para5 AND w.isActive='Y'"
            //    + " AND tt.SeqNo=10"
            //    + " AND (wp.AD_Window_ID IS NOT NULL "
            //            + "OR EXISTS (SELECT 1 FROM AD_Tab tt2 WHERE tt2.AD_Window_ID = ws.AD_Window_ID AND tt2.AD_Table_ID=t.AD_Table_ID AND tt2.SeqNo=10))"
            //    + " ORDER BY 2";

            try {
                //var params = [];
                //index = 1;
                //if (!baseLanguage) {
                //    params.push(new VIS.SqlParam("@para1", VIS.Env.getAD_Language(VIS.Env.getCtx())));
                //    params.push(new VIS.SqlParam("@para2", VIS.Env.getAD_Language(VIS.Env.getCtx())));
                //    params.push(new VIS.SqlParam("@para3", VIS.Env.getAD_Language(VIS.Env.getCtx())));
                //}

                //params.push(new VIS.DB.SqlParam("@para4", targetTableName));
                //params.push(new VIS.DB.SqlParam("@para5", curWindow_ID));

                //dr = executeReader(sql, params);

                var dr = null;
                $.ajax({
                    type: 'Get',
                    async: false,
                    url: VIS.Application.contextUrl + "Form/GetZoomTargetClass",
                    data: { targetTableName: targetTableName, curWindow_ID: curWindow_ID },
                    success: function (data) {
                        dr = new VIS.DB.DataReader().toJson(data)
                    },
                });

                while (dr.read()) {
                    windowFound = true;
                    zoom_Window_ID = dr.getInt(6);
                    zoom_WindowName = dr.getString(7);
                    PO_Window_ID = dr.getInt(4);
                    whereClause = dr.getString(2);

                    // Multiple window support only for Order, Invoice, Shipment/Receipt which have PO windows
                    if (PO_Window_ID == 0)
                        break;
                    windowList.push({ "AD_Window_ID": dr.getInt(0), "windowName": dr.getString(1), "whereClause": whereClause });
                }
            }
            catch (e) {
                this.log.log(VIS.Logging.Level.SEVERE, sql, e);
            }
            finally {
                if (dr != null)
                    dr.dispose();
            }

            var sql1 = "";

            if (!windowFound || (windowList.length <= 1 && zoom_Window_ID == 0))
                return zoomList;

            //If there is a single window for the table, no parsing is necessary
            if (windowList.length <= 1) {

                //Check if record exists in target table
                sql1 = "SELECT count(*) FROM " + targetTableName + " WHERE "
                    + targetWhereClause;
                if (whereClause != null && whereClause.length != 0)
                    sql1 += " AND " + VIS.Evaluator.replaceVariables(whereClause, VIS.Env.getCtx(), null);

            }
            else if (windowList.length > 1) {
                // Get the columns used in the whereClause
                for (var i = 0; i < windowList.length; i++)
                    this.parseColumns(columns, windowList[i].whereClause);

                // Get the distinct values of the columns from the table if record exists
                sql1 = "SELECT DISTINCT ";
                for (i = 0; i < columns.length; i++) {
                    if (i != 0)
                        sql1 += ",";
                    sql1 += columns[i];
                }

                if (columns.length == 0)
                    sql1 += "count(*) ";
                sql1 += " FROM " + targetTableName + " WHERE "
                    + targetWhereClause;
            }


            this.log.fine(sql1);

            var columnValues = [];
            try {


                var dr = null;
                $.ajax({
                    type: 'Get',
                    async: false,
                    url: VIS.Application.contextUrl + "Form/GetZoomWhereClause",
                    data: { sql: VIS.secureEngine.encrypt(sql1) },
                    success: function (data) {
                        dr = new VIS.DB.DataReader().toJson(data)
                    },
                });


                //  dr = executeReader(sql1, null, null, true);



                while (dr.read()) {
                    if (columns.length > 0) {
                        columnValues.length = 0;
                        for (var j = 0; j < columns.length; j++) {
                            var columnName = columns[j];
                            var columnValue = "";

                            if (typeof (dr.get(columnName)) !== "string")
                                columnValue = dr.get(columnName).toString();
                            else
                                columnValue = "'" + dr.get(columnName).toString() + "'";

                            this.log.fine(columnName + " = " + columnValue);
                            columnValues.push({ "Value": columnValue, "Name": columnName });
                        }

                        // Find matching windows
                        for (j = 0; j < windowList.length; j++) {
                            //log.fine("Window : "+windowList.get(i).windowName + " WhereClause : " + windowList.get(i).whereClause);
                            if (this.evaluateWhereClause(columnValues, windowList[j].whereClause)) {
                                //log.fine("MatchFound : "+windowList.get(i).windowName );
                                var pp = { "Key": windowList[j].AD_Window_ID, "Value": windowList[j].windowName };
                                zoomList.push(pp);
                                // Use first window found. Ideally there should be just one matching
                                break;
                            }
                        }
                    }
                    else {
                        var rowCount = dr.getInt(0);
                        if (rowCount != 0) {
                            var pp = { "Key": zoom_Window_ID, "Value": zoom_WindowName };
                            zoomList.push(pp);
                        }
                    }
                }
            }
            catch (e) {
                this.log.log(VIS.Logging.Level.SEVERE, sql1, e);
            }
            finally {
                if (dr != null)
                    dr.dispose();
            }

            // Add the windows for Product Production resource and work center.
            //if(curWindow_ID == 140)  // Product window
            // zoomList.add(cost);
            //if(curWindow_ID == WorkCenterWindowID) // Work Center Window
            //  zoomList.add(WcCost);
            //if(curWindow_ID == ProductionResourceWindowID) // Production Resource
            //  zoomList.add(cost);		
            return zoomList;
        }
    };


    /** Cache */


    function CacheMgt() {

        /**	List of Instances				*/
        var m_instances = [];
        var m_tableNames = [];
        var log = VIS.Logging.VLogger.getVLogger("CacheMgt");

        function get() {
            if (s_cache == null) {
                //    s_cache = 
            }
        }

        function register(instance) {
            if (instance == null)
                return false;
            var tableName = instance.getTableName();
            m_tableNames.push(tableName);
            m_instances.push(instance);
            return true;
        }


        /**
	 * 	Un-Register Cache Instance
	 *	@param instance Cache
	 *	@return true if removed
	 */
        function unregister(instance) {
            if (instance == null)
                return false;
            var found = false;
            //	Could be included multiple times
            for (var i = m_instances.lenght - 1; i >= 0; i--) {
                var stored = m_instances[i];
                if (instance === stored) {
                    m_instances.splice(1, 1);// .remove(i);
                    found = true;
                }
            }
            return found;
        }	//	unregister


        function reset() {
            if (arguments.length == 2)
                return reset2(arguments[0], arguments[1]);
            else if (arguments.length == 1)
                return reset2(arguments[0]);

            var counter = 0;
            var total = 0;
            for (var i = 0; i < m_instances.length; i++) {
                var stored = m_instances.get(i);
                if (stored != null && stored.lenght > 0) {
                    log.info(stored.toString());
                    total += stored.reset();
                    counter++;
                }
            }
            //MRole.reset(0);
            if (counter > 0)
                log.info("#" + counter + " (" + total + ")");
            return total;
        }	//	reset



        function reset2(tableName, Record_ID) {

            if (tableName == null || tableName.length() == 0)
                return reset();
            if (!Record_ID)
                Record_ID = 0;

            //	if (tableName.endsWith("Set"))
            //		tableName = tableName.substring(0, tableName.length()-3);
            if (m_tableNames.indexOf(tableName) < 0)
                return 0;
            if (tableName.equals("AD_Role")) {
                //  MRole.reset(Record_ID);
            }
            //
            var counter = 0;
            var total = 0;
            for (var i = 0; i < m_instances.length; i++) {
                var stored = m_instances[i];
                if (stored == null)
                    continue;
                var ii = stored.reset(tableName, Record_ID);
                if (ii >= 0)
                    counter++;
                if (ii > 0)
                    total += ii;
            }
            if (counter > 0)
                log.fine(tableName + ": #" + counter + " (" + total + ")");
            //	Update Server
            // if (DB.isRemoteObjects())
            // {
            //Server server = CConnection.get().getServer();
            //try
            //{
            //    if (server != null)
            //    {	//	See ServerBean
            //        int serverTotal = server.cacheReset(tableName, Record_ID); 
            //        if (CLogMgt.isLevelFinest())
            //            log.fine("Server => " + serverTotal);
            //    }
            //}
            //catch (RemoteException ex)
            //{
            //    log.log(Level.SEVERE, "AppsServer error", ex);
            //}
            // }
            return total;
        }	//	reset


        var c = {
            register: register,
            reset: reset,
            unregister: unregister
        }
        return c;
    };

    function CCache(tableName, initialCapacity, expireMinutes,
        resetAll, type, tableName2) {

        /** The Cache		**/
        this.m_cacheK = [];
        this.m_cacheV = [];
        /**	Table Name					*/
        this.m_tableName = tableName;
        /** Reset All Records			*/
        this.m_resetAll = (resetAll) ? resetAll : false;
        /**	Alternative Table Name		*/
        this.m_tableName2 = tableName2;
        this.setExpireMinutes(expireMinutes);
        VIS.CacheMgt.register(this);
    };

    CCache.prototype.setExpireMinutes = function (expireMinutes) {

        if (expireMinutes > 0) {
            this.m_expire = expireMinutes;
            var addMS = 60000 * expireMinutes;
            this.m_timeExp = VIS.Env.currentTimeMillis() + addMS;
        }
        else {
            this.m_expire = 0;
            this.m_timeExp = 0;
        }
    };

    CCache.prototype.expire = function () {
        if (this.m_expire != 0 && this.m_timeExp < VIS.Env.currentTimeMillis()) {
            //	System.out.println ("------------ Expired: " + GetName() + " --------------------");
            this.reset();
        }
    };

    CCache.prototype.reset = function () {
        if (arguments.length == 2)
            return this.reset2(arguments[0], arguments[1]);
        else if (arguments.length == 1)
            return this.reset2(arguments[0]);

        var no = this.m_cacheK.length;
        //	Clear
        this.clear();
        if (this.m_expire != 0) {
            var addMS = 60000 * this.m_expire;
            this.m_timeExp = VIS.Env.currentTimeMillis() + addMS;
        }
        return no;
    };	//

    CCache.prototype.reset2 = function (tableName, record_ID) {
        if (this.tableName == null)
            return -1;
        var exact = tableName === this.m_tableName
            || tableName === this.m_tableName2;
        if (exact
            || this.m_tableName2.startsWith(tableName))	//	include Child tables
        {
            if ((record_ID == 0) || this.m_resetAll)
                return reset();
            return this.remove(record_ID);
        }
        return -1;
    };	//	reset

    CCache.prototype.remove = function (key) {
        //	 might be wrong key
        var index = this.m_cacheK.indexOf(key);

        if (index > -1) {
            this.m_cacheK.splice(index, 1);
            this.m_cacheV.splice(index, 1);
            return 1;
        }
        return 0;
    };

    CCache.prototype.get = function (key) {
        this.expire();

        var index = this.m_cacheK.indexOf(key);
        if (index > -1)
            return this.m_cacheV[index];
        return null;
    };	//	get


    CCache.prototype.contains = function (key) {
        var index = this.m_cacheK.indexOf(key);
        if (index > -1)
            return true;
        return false;
    }


    /**
     * 	Put value
     *	@param key key
     *	@param value value
     *	@return previous value
     */
    CCache.prototype.add = function (key, value) {
        this.expire();

        if (typeof value === "function" || $.isFunction(value)) {
            var ss = "";
        }


        var index = this.m_cacheK.indexOf(key);
        if (index < 0) {
            this.m_cacheK.push(key);
            this.m_cacheV.push(value);
        }
        else
            this.m_cacheV[index] = value;
    };	// put

    CCache.prototype.clear = function () {
        this.m_cacheK.length = 0;
        this.m_cacheV.length = 0;
    };

    CCache.prototype.size = function () {
        this.expire();
        return this.m_cacheV.length;
    };	//	s

    /**
	 * 	Get Table Name
	 *	@return name
	 */
    CCache.prototype.getTableName = function () {
        return this.m_tableName;
    };//

    VIS.CacheMgt = CacheMgt();
    VIS.CCache = CCache;



}(jQuery, VIS));


/**
 * Tokenizer/jQuery.Tokenizer
 * Copyright (c) 2007-2008 Ariel Flesler - aflesler(at)gmail(dot)com | http://flesler.blogspot.com
 * Dual licensed under MIT and GPL.
 * Date: 2/29/2008
 *
 * @projectDescription JS Class to generate tokens from strings.
 * http://flesler.blogspot.com/2008/03/string-tokenizer-for-javascript.html
 *
 * @author Ariel Flesler
 * @version 1.0.1
 */
; (function (VIS) {

    var Tokenizer = function (str, tokenizers, returnDelims, doBuild) {

        this.tokenizers = tokenizers.splice ? tokenizers : tokenizers.split("");
        if (doBuild) {
            this.doBuild = doBuild;
        }
        //this.parse(str);
        this.src = str;
        this.ended = false;
        this.tokens = [];
        this.returnDelim = returnDelims;
        this.curIndex = 0;
        this.parse();

    };

    Tokenizer.prototype = {
        parse: function () {
            do this.next(); while (!this.ended);
            return this.tokens;
        },
        build: function (src, real) {

            if (src) {
                if (!this.returnDelim && this.tokenizers.indexOf(src) > -1) {
                    return;
                }

                this.tokens.push(
                    !this.doBuild ? src :
                        this.doBuild(src, real, this.tkn)
                );
            }
        },
        next: function () {
            var self = this,
                plain;

            self.findMin();
            plain = self.src.slice(0, self.min);

            self.build(plain, false);

            self.src = self.src.slice(self.min).replace(self.tkn, function (all) {
                self.build(all, true);
                return '';
            });

            if (!self.src)
                self.ended = true;
        },
        findMin: function () {
            var self = this, i = 0, tkn, idx;
            self.min = -1;
            self.tkn = '';

            while ((tkn = self.tokenizers[i++]) !== undefined) {
                idx = self.src[tkn.test ? 'search' : 'indexOf'](tkn);
                if (idx != -1 && (self.min == -1 || idx < self.min)) {
                    self.tkn = tkn;
                    self.min = idx;
                }
            }
            if (self.min == -1)
                self.min = self.src.length;
        },
        countTokens: function () {

            return this.tokens.length;
        },
        nextToken: function () {
            if (this.hasMoreTokens()) {
                return this.tokens[this.curIndex++]
            }
            return null;
        },
        hasMoreTokens: function () {

            if (this.curIndex < this.tokens.length) {
                return true;
            }
            return false;
        },
        reset: function () {
            this.curIndex = -1;
        }
    };

    //if (window.jQuery) {
    //  console.log("Jquery");
    //   jQuery.tokenizer = Tokenizer;//export as jquery plugin
    //   Tokenizer.fn = Tokenizer.prototype;
    // } else {
    //    console.log("Window");
    //   window.Tokenizer = Tokenizer;//export as standalone class
    // }
    VIS.StringTokenizer = Tokenizer;

})(VIS);
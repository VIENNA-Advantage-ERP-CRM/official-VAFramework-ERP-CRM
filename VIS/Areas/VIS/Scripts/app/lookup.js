; (function (VIS, $) {

    var DB = VIS.DB;
    var Level = VIS.Logging.Level;

    var baseUrl = VIS.Application.contextUrl;
    var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";
    var dSetUrl = baseUrl + "Form/JDataSet";
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

    //executeDataSet
    var executeDataSet = function (sql, param, callback) {
        var async = callback ? true : false;

        var dataIn = { sql: sql, page: 1, pageSize: 0 };
        if (param) {
            dataIn.param = param;
        }

        var dataSet = null;

        getDataSetJString(dataIn, async, function (jString) {
            dataSet = new VIS.DB.DataSet().toJson(jString);
            if (callback) {
                callback(dataSet);
            }
        });

        return dataSet;
    };

    var executeDSet = function (sql, param, callback) {
        var async = callback ? true : false;

        var dataIn = { sql: sql, page: 1, pageSize: 0 };
        if (param) {
            dataIn.param = param;
        }
        //dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
        var dataSet = null;

        getDSetJString(dataIn, async, function (jString) {
            dataSet = new VIS.DB.DataSet().toJson(jString);
            if (callback) {
                callback(dataSet);
            }
        });

        return dataSet;
    };

    var executeDReader = function (sql, param, callback) {
        var async = callback ? true : false;

        var dataIn = { sql: sql, page: 1, pageSize: 0 };
        if (param) {
            dataIn.param = param;
        }
        //dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
        var dr = null;
        getDSetJString(dataIn, async, function (jString) {
            dr = new VIS.DB.DataReader().toJson(jString);
            if (callback) {
                callback(dr);
            }
        });
        return dr;
    };


    var executeDataReaderPaging = function (sql, page, pageSize, param, callback) {
        var async = callback ? true : false;

        var dataIn = { sql: sql, page: page, pageSize: pageSize };
        if (param) {
            dataIn.param = param;
        }
        //dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);

        var dr = null;

        getDSetJString(dataIn, async, function (jString) {
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

    //DataSet String
    function getDSetJString(data, async, callback) {
        var result = null;
        data.sql = VIS.secureEngine.encrypt(data.sql);
        $.ajax({
            url: dSetUrl,
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


    function Lookup(lookupInfo, lookup, ctx, windowNo, displayType) {
        this.info = lookupInfo;
        this.lookup = null;
        this.allLoaded = false;
        this.hasInactive = false;
        this.displayType = displayType
        this.selectedObject;
        this.windowNo = windowNo;
        this.validationDisabled = false;
        this.tabNo = 0;
        this.colName = "";

        this.data = [];
        this.ctx = ctx;

        if (lookup) {
            this.lookup = lookup._lookup;
            this.allLoaded = lookup._allLoaded;
            this.hasInactive = lookup._hasInactive;
            this.displayType = lookup._displayType;
            this.windowNo = lookup._WindowNo;
            this.tabNo = lookup._TabNo;
            this.colName = lookup._columnName;
            if (this.info) {
                this.info.windowNo = lookup._WindowNo;
            }
        }
    };

    Lookup.prototype.INACTIVE_S = '~';
    /** Inactive Marker End         */
    Lookup.prototype.INACTIVE_E = "~";
    /** Number of max rows to load */
    Lookup.prototype.MAX_ROWS = 10000;
    /** Indicator for Null */
    Lookup.prototype.MINUS_ONE = -1;

    Lookup.prototype.getValidation = function () {
        return "";
    };
    Lookup.prototype.getWindowNo = function () {
        return this.windowNo;
    };

    Lookup.prototype.getTabNo = function () {
        return this.tabNo;
    };

    Lookup.prototype.setTabNo = function (tNo) {
        this.tabNo = tNo;
    };

    Lookup.prototype.getFieldColName = function () {
        return this.colName; //lower case
    };

    Lookup.prototype.getDisplayType = function () {
        return this.displayType;
    };
    Lookup.prototype.setDisplayType = function (displayType) {
        this.displayType = displayType;
    };
    Lookup.prototype.disableValidation = function () {
        this.validationDisabled = true;
    };
    Lookup.prototype.getIsValidationDisabled = function () {
        return this.validationDisabled;
    };
    Lookup.prototype.getIsValidated = function () {
        return true;
    };
    Lookup.prototype.getHasInactive = function () {
        return this.hasInactive;
    };
    Lookup.prototype.getData = function (mandatory, onlyValidated, onlyActive, temporary) {
        return [];
    };
    Lookup.prototype.getDisplay = function (value) {
        return "";
    };
    Lookup.prototype.getSelectedItem = function () {
        return this.selectedObject;
    };
    Lookup.prototype.fillCombobox = function (mandatory, onlyValidated, onlyActive, temporary) {
        var obj = this.selectedObject;
        if (this.data != null)
            this.data.length = 0;
        // may cause delay *** The Actual Work ***
        this.data = this.getData(mandatory, onlyValidated, onlyActive, temporary);
        var size = 0;
        if (this.data != null)
            size = this.data.length;
        // Selected Object changed
        if (obj != this.selectedObject) {
            obj = this.selectedObject;
        }
        // if nothing selected & mandatory, select first
        if ((obj == null) && mandatory && (size > 0)) {
            obj = this.data[0];
            this.selectedObject = obj;
        }
    };
    Lookup.prototype.setSelectedItem = function (anObject) {
        if (((this.selectedObject != null) && this.selectedObject !== anObject)
            || ((this.selectedObject == null) && (anObject != null))) {
            this.setSelectedItemAlways(anObject);
        }
    };
    Lookup.prototype.setSelectedItemAlways = function (anObject) {
        if (this.data.indexOf(anObject) > -1 || (anObject == null)) {
            this.selectedObject = anObject;
        } else {
            this.selectedObject = null;
        }
    };
    Lookup.prototype.refresh = function () {
        return 0;
    };
    Lookup.prototype.getZoomQuery = function () {
        return null;
    };

    Lookup.prototype.getColumnName = function () {
        return this.info.keyColumn;
    };

    Lookup.prototype.dispose = function () {
        this.info = null;
        this.lookup = null;
        this.allLoaded = null;
        this._hasInactive = null;
        this.selectedObject = null;
        this.data = null;
        this.ctx = null;
    };
    Lookup.prototype.removeAllElements = function () {
        if (this.data.length > 0) {
            //int firstIndex = 0;
            //int lastIndex = p_data.size() - 1;
            this.data.length = 0;
            this.selectedObject = null;
            //fireIntervalRemoved(this, firstIndex, lastIndex);
        }
    };// removeAllElements
    Lookup.prototype.clear = function () {
        this.removeAllElements();
    };
    Lookup.prototype.gethasImageIdentifier = function () {
        return false;
    };

    //END




    //1.  MLookup wrapper class for lookup json Object 
    function MLookup(lookupInfo, lookup) {
        Lookup.call(this, lookupInfo, lookup);
        /** Save getDirect last return value */
        this.lookupDirect = {};
        /** Save last unsuccessful */
        this.directNullKey;

        this.loading = false;

        this.nextRead = 0;
        this.cLookup = false;
        this.lookupDirectAll = {};

        this.zomQuery = null;

        this.prepareZQuery();

        this.log = VIS.Logging.VLogger.getVLogger("MLookup");
    };
    VIS.Utility.inheritPrototype(MLookup, Lookup);//Inherit

    MLookup.prototype.prepareZQuery = function () {
        if (this.info.zoomQuery == null || $.isEmptyObject(this.info.zoomQuery)) {
            return;
        }
        var q = this.info.zoomQuery;
        this.zomQuery = new VIS.Query(q._tableName);
        var qr = null;
        for (var i = 0; i < q._list.length; i++) {
            var l = q._list[i];
            qr = new VIS.QueryRestriction(l.ColumnName, l.Code, l.Code_to,
                l.InfoName, q.InfoDisplay, l.InfoDisplay_to,
                l.Operator, l.DirectWhereClause, l.AndCondition)
            this.zomQuery.addRestriction(qr);
        }
    };


    MLookup.prototype.initialize = function (later) {

        if (later) {
            return this;
        }

        //  load into local lookup, if already cached

        //if (this.info.isCreadedUpdatedBy) {
        //    return this;
        //}

        if (this.info.isParent || this.info.isKey) {
            this.hasInactive = true; // creates focus listener for dynamic loading
            return this; // required when parent needs to be selected (e.g.
            // price from product)
        }


        //if (this.getDisplayType() == VIS.DisplayType.Search) {
        //    this.fillDirectList();
        //    return this;
        //}

        if ((this.getDisplayType() == VIS.DisplayType.Search)
            || this.info.isCreadedUpdatedBy) {
            return this;
        }
        //Don't load Parents/Keys


        //if (VIS.MLookupCache.loadFromCache(this.info, this)) {
        //    return this;
        //}

        if (!this.info.isValidated) {
            this.fillDirectList();
        }
        else {
            this.load(true, true);
        }
        //
        // m_loader.run(); // test sync call
        return this;
    };


    MLookup.prototype.getValidation = function () {
        if (this.getIsValidationDisabled())
            return "";
        return this.info.validationCode;
    }
    MLookup.prototype.getIsValidated = function () {
        if (this.getIsValidationDisabled())
            return true;
        if (this.info == null)
            return false;
        return this.info.isValidated;
    }
    MLookup.prototype.getData = function (mandatory, onlyValidated, onlyActive, temporary) {
        // create list
        var list = this.getData2(onlyValidated, temporary);

        // Remove inactive choices
        if (onlyActive && this.hasInactive) {
            // list from the back
            for (var i = list.length; i > 0; i--) {
                var o = list[i - 1];
                if (o != null) {
                    var s = o.Name;
                    if (s.startsWith(this.INACTIVE_S) && s.endsWith(this.INACTIVE_E)) {
                        console.log(s);
                        list.splice(i - 1, 1);
                    }
                }
            }
        }

        // Add Optional (empty) selection
        if (!mandatory) {
            var p = null;
            if ((this.info.keyColumn != null) && this.info.keyColumn.toUpperCase().endsWith("_ID")) {
                p = { Key: -1, Name: "" };
            } else {
                p = { Key: "", Name: "" };
            }
            list.unshift(p); //insert at 0 index
        }

        return list;
    }
    MLookup.prototype.getData2 = function (onlyValidated, loadParent) {

        // Never Loaded (correctly)
        if (!this.allLoaded || $.isEmptyObject(this.lookup)) {
            this.refresh(loadParent);
        }

        // already validation included
        if (this.info.isValidated)
            return this.getObjectArray(this.lookup);

        if (!this.info.isValidated && onlyValidated) {
            this.refresh(loadParent);
            //log.fine(m_info.KeyColumn + ": Validated - #" + m_lookupMap.size()
            //      + (loadParent ? " (loadParent)" : " (not loadParent)"));
        }

        return this.getObjectArray(this.lookup);
    }
    MLookup.prototype.getObjectArray = function (obj) {
        var list = [];
        var i = 0;
        for (var prop in obj) {
            list[i++] = obj[prop];
        }
        return list;
    };

    MLookup.prototype.getDisplay = function (value, checkLocalList, loadImage) {
        if (value == null) {
            return "";
        }
        var display = !checkLocalList ? this.get(value) : this.getFromList(value);
        if (display == null)
            return "<" + value.toString() + ">";
        if (loadImage)
            return display.Name;
        else
            return VIS.Utility.Util.getIdentifierDisplayVal(display.Name);

        return display.Name;
    };


    MLookup.prototype.getLovIconType = function (value, checLocalList) {
        var iconobj = this.lookup[" " + value];

        if (iconobj) {
            var iconpath = iconobj["icoType"];
            return iconpath;
        }
    };

    MLookup.prototype.getLOVIconElement = function (value, checkLocalList, getsource) {

        var iconobj = this.lookup[" " + value];

        if (iconobj) {
            var iconpath = iconobj["ico"];
            var iconstyle = iconobj["icohtml"];
            //Apply style on icon
            if (iconstyle) {
                iconstyle = " style='" + iconstyle + "'";
            }
            else {
                iconstyle = '';
            }

            if (iconpath) {
                if (iconpath.indexOf(" ") > -1) {
                    if (getsource)
                        return iconpath;
                    else
                        return "<i class='" + iconpath + "'" + iconstyle + "></i>";
                }
                else {
                    var img = iconpath.substring(iconpath.indexOf("Images/") + 7);
                    img = VIS.Application.contextUrl + "Images/Thumb32x32/" + img;
                    if (getsource)
                        return img;
                    else
                        return "<img src='" + img + "'" + iconstyle + "></img>";
                }
            }
        }

    };

    MLookup.prototype.getLOVIconStyle = function (value) {
        var iconobj = this.lookup[" " + value];

        if (iconobj) {
            var iconHTML = iconobj["icohtml"];
            return iconHTML;
        }
    }





    MLookup.prototype.get = function (key) {
        if ((key == null) || this.MINUS_ONE === key) // indicator for null
            return null;

        if (this.info.isParent && this.nextRead < Date.now()) {
            //this.lookup = null;
            //this.lookup = {};
            ////if (this.lookupDirect) {
            //    this.lookupDirect = null;
            //    this.lookupDirect = {};
            //}
            this.nextRead = Date.now() + 2000; // 1/2 sec
        }

        var retValue = this.lookup[" " + key];
        if (retValue)
            return retValue;


        // Always check for parents - not if we SQL was validated and completely
        // loaded
        if (!this.info.isParent && this.info.isValidated && this.allLoaded) {
            this.getDirect(key, false, true); // cache locally 
        }
        if (!this.allLoaded && ($.isEmptyObject(this.lookup))
            && !this.info.isCreadedUpdatedBy && !this.info.isParent
            && (this.getDisplayType() != VIS.DisplayType.Search)) {
            //m_result = s_exec.submit(m_loader);
            this.load();
            //loadComplete();
            retValue = this.lookup[" " + key];
            if (retValue != null)
                return retValue;
        }


        var cacheLocal = true;// this.info.isValidated;
        return this.getDirect(key, false, cacheLocal); // do NOT cache
    };
    MLookup.prototype.getFromList = function (key) {

        if ((key == null) || this.MINUS_ONE === key) // indicator for null
            return null;

        if (this.info.isParent && this.nextRead < Date.now()) {
            //this.lookup = null;
            //this.lookup = {};
            //if (this.lookupDirect) {
            //    this.lookupDirect = null;
            //    this.lookupDirect = {};
            //}
            this.nextRead = Date.now() + 2000; // 1/2/3 sec
            return this.get(key);
        }

        //	try cache
        var retValue;

        retValue = this.lookup[" " + key];
        if (retValue != null)
            return retValue;

        //var keyCol = this.info.keyColumn.substring(this.info.keyColumn.indexOf('.') + 1).toLowerCase();

        //var text = VIS.MLookupCache.getRecordLookup(this.getWindowNo(), this.getTabNo(), keyCol, key);
        //if (text) {
        //    //var keyValue = var isNumber = this.info.keyColumn.toUpperCase().endsWith("_ID");
        //    retValue = { Key: key, Name: VIS.Utility.encodeText(text) };
        //    this.lookup[" " + key] = retValue;
        //    return retValue;
        //}


        //  Always check for parents - not if we SQL was validated and completely loaded
        if (!this.info.isParent && this.info.isValidated && this.allLoaded) {
            this.getDirectFromList(key, false, true);		//	cache locally
        }


        if (!this.allLoaded
            && $.isEmptyObject(this.lookup)
            && !this.info.isCreadedUpdatedBy
            && !this.info.isParent
            && this.getDisplayType() != VIS.DisplayType.Search) {


            retValue = this.lookup[" " + key];
            if (retValue != null)
                return retValue;
        }
        //	Try to get it directly
        var cacheLocal = this.info.isValidated;
        return this.getDirectFromList(key, false, cacheLocal);	//	do NOT cache	
    };	//	get


    MLookup.prototype.getDirect = function (key, saveInCache, cacheLocal) {
        // Nothing to query
        if ((key == null) || (this.info.queryDirect == null)
            || (this.info.queryDirect.length == 0))
            return null;
        if (key === this.directNullKey)
            return null;
        var directValue = null;
        if (this.lookupDirect != null) // Lookup cache
        {
            directValue = this.lookupDirect[key];
            if (directValue)
                return directValue;
        }
        var isNumber = this.info.keyColumn.toUpperCase().endsWith("_ID");

        try {
            // SELECT Key, Value, Name FROM ...
            var query = this.info.queryDirect;
            var param = [];
            if (isNumber) {
                param.push(new DB.SqlParam("@key", parseInt(key)));
            }
            else {
                param.push(new DB.SqlParam("@key", key.toString()));
            }
            var dr = executeDReader(query, param);

            if (dr.read()) {
                var name = dr.getString(2);

                var isActive = dr.getString(3).equals("Y");
                if (!isActive) {
                    name = this.INACTIVE_S + name + this.INACTIVE_E;
                    this.hasInactive = true;
                }

                if (isNumber) {
                    var keyValue = dr.getInt(0);
                    var p = { Key: keyValue, Name: VIS.Utility.encodeText(name) };
                    if (saveInCache) // save if
                        this.lookup[" " + keyValue] = p;
                    directValue = p;
                } else {
                    var value = dr.getString(1);
                    var p1 = { Key: value, Name: VIS.Utility.encodeText(name) };
                    if (saveInCache) // save if
                        this.lookup[" " + value] = p1;
                    directValue = p1;
                }
                if (dr.read()) {
                    this.log.log(VIS.Logging.Level.SEVERE, this.info.keyColumn
                        + ": Not unique (first returned) for " + key
                        + " SQL=" + query);
                }

            } else {
                this.directNullKey = key;
                directValue = null;
            }

            //if (CLogMgt.isLevelFinest())
            //    log.finest(m_info.KeyColumn + ": " + directValue + " - "
            //			+ m_info);
        }
        catch (e) {
            this.log.log(VIS.Logging.Level.SEVERE, this.info.keyColumn + ": SQL="
                + this.info.queryDirect + "; Key=" + key, e);
            directValue = null;
        }
        finally {
            if (dr != null)
                dr.dispose();
            dr = null;
        }
        // Cache Local if not added to R/W cache
        if (cacheLocal && !saveInCache && (directValue != null)) {
            if (this.lookupDirect == null)
                this.lookupDirect = {};
            this.lookupDirect[key] = directValue;
        }
        this.hasInactive = true;
        return directValue;
    };

    MLookup.prototype.getDirectFromList = function (key, saveInCache, cacheLocal) {
        //	Nothing to query
        if (key == null || this.info.queryDirect == null || this.info.queryDirect.length == 0)
            return null;
        if (key === this.directNullKey)
            return null;
        //
        var directValue = null;
        if (this.lookupDirect != null)		//	Lookup cache
        {
            var o = null;
            o = this.lookupDirect[" " + key];
            if (o && o != null)
                return o;
        }

        var p;

        if (!this.lookupDirectAll[" " + key]) {
            var keyCol = this.getFieldColName();// this.info.keyColumn.substring(this.info.keyColumn.indexOf('.') + 1).toLowerCase();

            var text = VIS.MLookupCache.getRecordLookup(this.getWindowNo(), this.getTabNo(), keyCol, key);
            if (text || (!text && key == 0)) {
                //var keyValue = var isNumber = this.info.keyColumn.toUpperCase().endsWith("_ID");
                var retValue = { Key: key, Name: VIS.Utility.encodeText(text) };
                this.lookupDirectAll[" " + key] = retValue;
                // return retValue;
            }
        }

        if (this.lookupDirectAll[" " + key]) {
            p = this.lookupDirectAll[" " + key];
            if (saveInCache)		//	save if
                this.lookup[" " + key] = p;
            directValue = p;
        }
        else if (!this.loading) {
            this.addInList(key);
            directValue = null;
        }


        //	Cache Local if not added to R/W cache
        if (cacheLocal && !saveInCache && directValue != null) {
            if (this.lookupDirect == null)
                this.lookupDirect = {};
            this.lookupDirect[" " + key] = directValue;
        }
        this.hasInactive = true;
        return directValue;
    };

    MLookup.prototype.addInList = function (key) {
        if (this.info.queryDirect.length <= 0 || key == null) {
            return;
        }
        //string directAllQuery = _vInfo.queryDirect.Substring(0, _vInfo.queryDirect.LastIndexOf("WHERE"));

        var isNumber = this.info.keyColumn.toUpperCase().endsWith("_ID");

        // SELECT Key, Value, Name FROM ...
        var query = this.info.queryDirect;
        var param = [];
        if (isNumber) {
            param.push(new DB.SqlParam("@key", parseInt(key)));
        }
        else {
            param.push(new DB.SqlParam("@key", key.toString()));
        }

        var directAllQuery = this.info.queryDirect;//.replace("@key", key.toString());

        var self = this;
        executeDReader(directAllQuery, param, function (dr) {
            try {

                if (dr.read()) {
                    var name = dr.getString(2);
                    if (isNumber) {
                        var keyValue = VIS.Utility.Util.getValueOfInt(dr.getCell(0));
                        self.lookupDirectAll[" " + keyValue] = { Key: keyValue, Name: VIS.Utility.encodeText(name) };
                    }
                    else {
                        var value = dr.getString(1);
                        //ValueNamePair p = new ValueNamePair(value, name);
                        self.lookupDirectAll[" " + value] = { Key: value, Name: VIS.Utility.encodeText(name) };
                    }
                }
                else {
                    self.directNullKey = key;
                }
            }
            catch (e) {
                console.log(e);
            }
            self = null;
        });
    };

    MLookup.prototype.load = function (async, checkCache) {

        var sql = this.info.query;
        var params = [];

        // not validated
        if (!this.info.isValidated) {
            var validation = VIS.Env.parseContext(VIS.context, this.getWindowNo(), this.getTabNo(), this.info.validationCode, false, true);
            if (validation.length == 0 && this.info.validationCode.length > 0) {
                this.log.fine(this.info.keyColumn + ": Loader NOT Validated: " + this.info.validationCode);
                //console.log(this.info.keyColumn + ": Loader NOT Validated: " + this.info.validationCode);
                return;
            }
            else {
                var posFrom = sql.lastIndexOf(" FROM ");
                var hasWhere = sql.indexOf(" WHERE ", posFrom) != -1;
                //
                var posOrder = sql.lastIndexOf(" ORDER BY ");
                if (posOrder != -1)
                    sql = sql.substring(0, posOrder) + (hasWhere ? " AND " : " WHERE ") + validation + sql.substring(posOrder);
                else
                    sql += (hasWhere ? " AND " : " WHERE ") + validation;
                this.log.fine(this.info.keyColumn + ": Validation=" + validation);
            }
        }

        var isNumber = this.info.keyColumn.toUpperCase().endsWith("_ID");
        this.hasInactive = false;

        var dr = null;
        this.lookup = {};

        if (checkCache) {
            this.cLookup = false;
            if (VIS.MLookupCache.loadFromCache(this.info, this)) {
                return this;
            }
        }

        if (async) {

            var self = this;
            self.loading = true;
            executeDataReaderPaging(sql, 1, 1000, null, function (dr) {
                self.readData(dr, isNumber);
                if (checkCache) {

                    VIS.MLookupCache.loadEnd(self.info, self);
                }
                self.loading = false;;
                self = null;
            });
            return 0;
        }

        try {
            dr = executeDReader(sql, null);
            this.readData(dr, isNumber);
            if (checkCache) {

                VIS.MLookupCache.loadEnd(this.info, this);
            }
        }
        catch (e) {
            this.log.log(VIS.Logging.Level.SEVERE, this.info.keyColumn + ": Loader - " + sql, e);
        }
        finally {
            if (dr != null) {
                dr.dispose();
                dr = null;
            }
        }

        return 0;
    }; // run

    MLookup.prototype.fillDirectList = function () {

        if (this.info.queryAll == null || this.info.queryAll.length <= 0) {
            return;
        }
        //string directAllQuery =    _vInfo.queryDirect.Substring(0, _vInfo.queryDirect.LastIndexOf("WHERE"));

        var directAllQuery = this.info.queryAll;


        this.cLookup = true;

        if (VIS.MLookupCache.loadFromCache(this.info, this)) {
            return;
        }

        var self = this;
        executeDataReaderPaging(directAllQuery, 1, 100, null, function (dr) {
            try {
                var isNumber = self.info.keyColumn.endsWith("_ID");

                while (dr.read()) {

                    var name = dr.getString(2);
                    if (isNumber) {
                        var keyValue = dr.getInt(0);
                        self.lookupDirectAll[" " + keyValue] = { Key: keyValue, Name: VIS.Utility.encodeText(name) };
                    }
                    else {
                        var value = dr.getString(1);
                        self.lookupDirectAll[" " + value] = { Key: value, Name: VIS.Utility.encodeText(name) };
                    }
                }
            }
            catch (e) {
                console.log(e);
            }
            VIS.MLookupCache.loadEnd(self.info, self);
            self.loading = false;
            self = null;
        });
        this.loading = true;
    };

    MLookup.prototype.readData = function (dr, isNumber) {
        var lookup = {};
        var rows = 0;
        this.allLoaded = true;
        while (dr.read()) {
            if (rows++ > this.MAX_ROWS) {
                this.log.fine(this.info.keyColumn
                    + ": Loader - Too many records");
                this.allLoaded = false;
                break;
            }

            // load data
            var name = dr.getString(2);

            var isActive = dr.getString(3).equals("Y");
            var p = {};
            var key;
            if (!isActive) {
                name = this.INACTIVE_S + name + this.INACTIVE_E;
                this.hasInactive = true;
            }
            if (isNumber) {
                key = dr.getInt(0);
                p = { Key: key, Name: VIS.Utility.encodeText(name) };
            } else {
                key = dr.getString(1);
                p = { Key: key, Name: VIS.Utility.encodeText(name) };
            }

            if (dr.tables[0].columns.length > 4) {
               /// p["ico"] = dr.getString(4);
                var iconInfo = dr.getString(4);
                if (iconInfo && iconInfo.length > 0) {
                    var ico = iconInfo.split('|');
                    p["ico"] = ico[0];
                    p["icohtml"] = ico[1];

                }
                else {
                    p["ico"] = iconInfo;
                }

                p["icoType"] = dr.getString(5);
            }
            lookup[" " + key] = p;

        }
        this.lookup = null;
        this.lookup = lookup;

        //this.log.fine(this.info.keyColumn + ": " + name);
        this.loading = false;
    };

    MLookup.prototype.refresh = function () {
        // Don't load Search or CreatedBy/UpdatedBy
        if ((this.getDisplayType() == VIS.DisplayType.Search)
            || this.info.IsCreadedUpdatedBy) {
            return 0;
        }
        if (this.loading)
            return 0;
        var size = this.load();
        return size;
    };
    MLookup.prototype.clear = function () {
        this.$super.clear.call(this);
        this.lookup = {};
    };// clear
    MLookup.prototype.getCLookup = function () {
        if (this.cLookup)
            this.lookupDirectAll;
        return this.lookup;
    };
    MLookup.prototype.setCLookup = function (lookup) {
        if (this.cLookup)
            this.lookupDirectAll = lookup;
        this.lookup = lookup;
    };
    MLookup.prototype.getIsLoading = function () {
        return this.loading;
    };
    MLookup.prototype.setIsLoading = function (loading) {
        this.loading = loading;
    };
    /**
     * Get Zoom
     * 
     * @return Zoom AD_Window_ID
     */
    MLookup.prototype.getZoomWindow = function () {
        if (arguments.length == 0) {
            return this.info.zoomWindow;
        }
        var query = arguments[0];

        /*
    * Handle cases where you have multiple windows for a single table. So
    * far it is just the tables that have a PO Window defined. For eg.,
    * Order, Invoice and Shipments This will need to be expanded to add
    * more tables if they have multiple windows.
    */

        var AD_Window_ID = VIS.ZoomTarget.getZoomAD_Window_ID(this.info.tableName,
            this.windowNo, query.getWhereClause(), VIS.context.isSOTrx(this.windowNo));
        return AD_Window_ID;
    }; // getZoomWindow
    /**
     * Get Zoom Query String
     * 
     * @return Zoom SQL Where Clause
     */
    MLookup.prototype.getZoomQuery = function () {
        return this.zomQuery;
    }; // getZoom
    /// <summary>
    /// Get Reference Value
    /// </summary>
    /// <returns>Reference Value</returns>
    MLookup.prototype.getAD_Reference_Value_ID = function () {
        return this.info.AD_Reference_Value_ID;
    };

    MLookup.prototype.gethasImageIdentifier = function () {
        return this.info.hasImageIdentifier;
    };

    MLookup.prototype.disableValidation = function () {
        var validationCode = this.info.validationCode;

        this.$super.disableValidation.call(this);

        this.info.isValidated = true;
        this.info.validationCode = "";

        //	Switch to Search for Table/Dir Validation
        if (validationCode != null && validationCode.length > 0) {
            if (validationCode.indexOf("@") != -1
                && this.getDisplayType() == VIS.DisplayType.Table || this.getDisplayType() == VIS.DisplayType.TableDir) {
                this.setDisplayType(VIS.DisplayType.Search);
            }
        }
    }

    MLookup.prototype.dispose = function () {
        this.$super.dispose.call(this);
        this.lookupDirect = null;
        this.directNullKey = null;

        this.log = null;
    }
    //END MLookup 




    //2. MLocationLookup
    function MLocationLookup(ctx, windowNo) {
        Lookup.call(this, null, null, ctx, windowNo, VIS.DisplayType.TableDir);
        this.lookup = {};
        this.load();
    };
    VIS.Utility.inheritPrototype(MLocationLookup, Lookup);//Inherit


    MLocationLookup.prototype.cacheLookup = null;


    MLocationLookup.prototype.getDisplay = function (value) {

        if (value == null)
            return "";
        var loc = this.getLocation(value, null);
        if (loc == null)
            return "<" + value.toString() + ">";
        return loc.Name;
    };
    /**
     *	Get Object of Key Value
     *  @param value value
     *  @return Object or null
     */
    MLocationLookup.prototype.get = function (value) {
        if (value == null)
            return null;
        var loc = this.getLocation(value, null);
        if (loc == null)
            return null;
        return loc;
    };	//	get
    MLocationLookup.prototype.getColumnName = function () {
        return "C_Location_ID";
    };
    MLocationLookup.prototype.getLocation = function (key, isLatLong) {
        if (key == null)
            return null;
        if (!isLatLong) {
            if (key in this.lookup) {
                return this.lookup[key];
            }
        }
        else {
            if (key in this.lstLatLng) {
                return this.lstLatLng[key];
            }
        }
        // VIS0008 load location data based on setting in system config
        if (VIS.context.getLocationBulkReload()) {
            var data = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Location" + "/GetLocation", { id: key })
            this.lookup[key] = data.LocItem;
            this.lstLatLng[key] = data.LocLatLng;
            if (!isLatLong)
                return data.LocItem;
            return data.LocLatLng;
        }
        else {
            var self = this;
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Location" + "/GetLocation", { id: key }, function (data) {
                if (key in self.lookup) {
                }
                else {
                    self.lookup[key] = data.LocItem;
                    self.lstLatLng[key] = data.LocLatLng;
                }
            });
            return null;
        }
    };	//

    MLocationLookup.prototype.refreshLocation = function (key) {
        if (key == null)
            return null;
        var data = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Location" + "/GetLocation", { id: key })
        this.lookup[key] = data.LocItem;
        this.lstLatLng[key] = data.LocLatLng;
        return data;
    };

    MLocationLookup.prototype.load = function () {

        var self = this;
        self.lstLatLng = {};

        if (MLocationLookup.prototype.cacheLookup != null) {
            self.lookup = MLocationLookup.prototype.cacheLookup.LocLookup;
            self.lstLatLng = MLocationLookup.prototype.cacheLookup.LocLatLng;
            self = null;
            return;
        }

        VIS.dataContext.getJSONData(VIS.Application.contextUrl + "Location" + "/GetLocations", "", function (data) {

            if (!MLocationLookup.prototype.cacheLookup) {
                MLocationLookup.prototype.cacheLookup = data;
            }

            self.lookup = data.LocLookup;
            self.lstLatLng = data.LocLatLng;
            self = null;
        });
    };

    MLocationLookup.prototype.getLatLng = function (key) {
        if (key == null || key.toString().len < 1)
            return null;
        if (key in this.lstLatLng) {
            return this.lstLatLng[key];
        }
        var data = this.getLocation(key, true);
        return data;
    };


    MLocationLookup.prototype.dispose = function () {
        this.lstLatLng = null;
        this.$super.dispose.call(this);
        this.lookup = null;
    };
    //END




    //3. MLocatorLookup
    function MLocatorLookup(ctx, windowNo) {
        Lookup.call(this, null, null, ctx, windowNo, VIS.DisplayType.TableDir);

        this.C_Locator_ID = 0;
        //Only  Organization
        this.OnlyOrgID = 0;
        //	Only Warehouse	
        this.onlyWarehouseID = 0;
        //	Only Product				
        this.onlyProductID = 0;
        //	Only outgoing Trx			
        this.onlyOutgoing = false;

        this.lookup = {};
        // Max Locators per Lookup		
        this.maxRows = 10000;

        this.defaultID = "";

        this.log = VIS.Logging.VLogger.getVLogger("MLocatorLookup");

        this.load(false);

        this.disposeLocal = function () {
            this.lookup = null;
            this.log = null;
        };
    };
    //inherit lookup class
    VIS.Utility.inheritPrototype(MLocatorLookup, Lookup);
    MLocatorLookup.prototype.getDefault = function () {
        return this.defaultID;
    };
    MLocatorLookup.prototype.getOnlyOrgID = function () {
        return this.OnlyOrgID;
    };
    MLocatorLookup.prototype.getOnlyWarehouseID = function () {
        return this.onlyWarehouseID;
    };
    MLocatorLookup.prototype.getOnlyProductId = function () {
        return this.onlyProductID;
    };
    MLocatorLookup.prototype.getIsOnlyOutgoing = function () {
        return this.onlyOutgoing;
    };
    MLocatorLookup.prototype.setOnlyOrgID = function (OnlyOrgID) {
        this.OnlyOrgID = OnlyOrgID;
    };
    MLocatorLookup.prototype.setOnlyWarehouseID = function (onlyWarehouseID) {
        this.onlyWarehouseID = onlyWarehouseID;
    };
    MLocatorLookup.prototype.setOnlyProductID = function (onlyProductID) {
        this.onlyProductID = onlyProductID;
    };
    MLocatorLookup.prototype.setOnlyOutgoing = function (isOutgoing) {
        this.onlyOutgoing = isOutgoing;
    };
    /**
     * Get Data Direct from Table
     * 
     * @param keyValue
     *            integer key value
     * @param saveInCache
     *            save in cache
     * @param trx
     *            transaction
     * @return Object directly loaded
     */
    MLocatorLookup.prototype.getDirect = function (keyValue, saveInCache, trxName) {
        if (keyValue == null) {
            return null;;
        }

        var sqlQry = "VIS_92";
        var param = [];
        param[0] = new VIS.DB.SqlParam("@keyValue", keyValue);
        var dr = executeReader(sqlQry, param);
        var value = "";
        if (dr.read()) {
            value = dr.getString(0);
        }
        else return null;

        dr.dispose();
        var retValue = { Key: keyValue, Name: VIS.Utility.encodeText(value) };
        if (saveInCache) {
            this.lookup[" " + keyValue] = retValue;
        }
        return retValue;
    };
    /**
     * Is Locator with key valid (Warehouse)
     * 
     * @param key
     *            key
     * @return true if valid
     */
    MLocatorLookup.prototype.getIsValid = function (key) {
        if (key == null)
            return true;
        // try cache
        var pp = this.lookup[key];
        return pp != null;
    };// isValid
    //Override functions
    MLocatorLookup.prototype.load = function (sync) {
        var param = [];
        var sqlParaCount = 0;
        var rows = 0;

        var orgId = this.getOnlyOrgID();
        var warehouseId = this.getOnlyWarehouseID();
        var productId = this.getOnlyProductId();
        var onlyIsSOTrx = this.getIsOnlyOutgoing();

        // Commented This 
        //var sql = "SELECT M_Locator_ID,Value FROM M_Locator WHERE IsActive='Y'";


        // Change By Lokesh Chauhan 6 Aug 2015
        // Display Identifiers in Locator Reference instead of Value
        //var colName = "Value";
        var colName = "Value, LocatorCombination, M_Warehouse_ID, (SELECT Name FROM M_Warehouse WHERE M_Warehouse_ID = M_Locator.M_Warehouse_ID) AS Warehouse";
        try {
            // Commented by Bharat on 24 Jan 2019
            // JID_1024: "Show locator name in dropdown with Warehouse Name_LocatorSearchkey(locatorName)

            //var sql = "VIS_93";
            //var ds = executeDataSet(sql, sqlParaCount > 0 ? param : null);
            //var dt = ds.getTable(0);
            //var len = dt.getRows().length;

            //for (var i = 0; i < len; i++) {
            //    //	Max out
            //    //if (rows++ > this.maxRows) {
            //    //    this.log.warning("Over Max Rows - " + rows);
            //    //    break;
            //    //}

            //    if (i == 0) {
            //        colName = dt.getRow(i).getCell(0);
            //    }
            //    else {
            //        colName += " || '_' || " + dt.getRow(i).getCell(0);
            //    }
            //}
            //ds.dispose();
            //ds = null;
        }
        catch (ex) {
            this.log.log(VIS.Logging.Level.SEVERE, sql, ex);
            colName = "Value";
        }
        // Change By Lokesh Chauhan 6 Aug 2015

        //var sql = "SELECT * FROM M_Locator WHERE IsActive='Y'"; not using mclass to get override value
        //sql = "SELECT M_Locator_ID," + colName + " FROM M_Locator WHERE IsActive='Y'";
        //if (warehouseId != 0) {
        //    sql += " AND M_Warehouse_ID=@w";
        //}
        //if (productId != 0) {
        //    sql += " AND (IsDefault='Y' OR EXISTS (SELECT * FROM M_Storage s WHERE s.M_Locator_ID=M_Locator.M_Locator_ID AND s.M_Product_ID=@p)";
        //    if (onlyIsSOTrx == null || !onlyIsSOTrx.value) {
        //        //	Default Product
        //        sql += "OR EXISTS (SELECT * FROM M_Product p WHERE p.M_Locator_ID=M_Locator.M_Locator_ID AND p.M_Product_ID=@p)OR EXISTS (SELECT * FROM M_ProductLocator pl " +
        //            " WHERE pl.M_Locator_ID=M_Locator.M_Locator_ID AND pl.M_Product_ID=@p) OR 0 = (SELECT COUNT(*) FROM M_ProductLocator pl " +
        //            " INNER JOIN M_Locator l2 ON (pl.M_Locator_ID=l2.M_Locator_ID) WHERE pl.M_Product_ID=@p AND l2.M_Warehouse_ID=M_Locator.M_Warehouse_ID )";
        //    }
        //    sql += ")";
        //}

        //var finalSql = VIS.MRole.addAccessSQL(sql, "M_Locator", VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO);

        //	Reset
        this.lookup = null;
        this.lookup = {};
        if (onlyIsSOTrx == null || !onlyIsSOTrx.value) {
            onlyIsSOTrx = true;
        }
        else {
            onlyIsSOTrx = false;
        }

        try {
            //if (warehouseId != 0) {
            //    param.push(new DB.SqlParam("@w", parseInt(warehouseId)));
            //    sqlParaCount += 1;
            //}
            //if (productId != 0) {
            //    param.push(new DB.SqlParam("@p", productId));
            //    sqlParaCount += 1;
            //}
            var ds = null;


            if (sync) {

                $.ajax({
                    type: 'Get',
                    async: false,
                    url: VIS.Application.contextUrl + "Form/GetWareProWiseLocator",
                    data: {
                        colName: colName,
                        orgId: orgId,
                        warehouseId: warehouseId,
                        productId: productId,
                        onlyIsSOTrx: onlyIsSOTrx
                    },
                    success: function (data) {
                        ds = new VIS.DB.DataSet().toJson(data);
                    },
                });


                // var ds = executeDataSet(finalSql, sqlParaCount > 0 ? param : null);
                var dt = ds.getTable(0);
                var len = dt.getRows().length;

                for (var i = 0; i < len; i++) {
                    //	Max out
                    if (rows++ > this.maxRows) {
                        this.log.warning("Over Max Rows - " + rows);
                        break;
                    }
                    var name = VIS.Utility.Util.getValueOfString(dt.getRow(i).getCell(4)) + "_" + VIS.Utility.Util.getValueOfString(dt.getRow(i).getCell(1))
                        + "(" + VIS.Utility.Util.getValueOfString(dt.getRow(i).getCell(2)) + ")";
                    this.lookup[dt.getRow(i).getCell(0)] = { Key: dt.getRow(i).getCell(0), Name: name };
                }
                ds.dispose();
                ds = null;
                this.log.fine("Complete #" + rows);
            }
            else {
                var self = this;

                $.ajax({
                    type: 'Get',
                    async: true,
                    url: VIS.Application.contextUrl + "Form/GetWareProWiseLocator",
                    data: {
                        colName: colName,
                        orgId: orgId,
                        warehouseId: warehouseId,
                        productId: productId,
                        onlyIsSOTrx: onlyIsSOTrx
                    },
                    success: function (data) {
                        ds = new VIS.DB.DataSet().toJson(data);
                        try {
                            var dt = ds.getTable(0);
                            var len = dt.getRows().length;

                            for (var i = 0; i < len; i++) {
                                //	Max out
                                if (rows++ > self.maxRows) {
                                    self.log.warning("Over Max Rows - " + rows);
                                    break;
                                }

                                self.lookup[dt.getRow(i).getCell(0)] = { Key: dt.getRow(i).getCell(0), Name: VIS.Utility.Util.getValueOfString(dt.getRow(i).getCell(1)) };
                            }
                            ds.dispose();
                            ds = null;
                            self.log.fine("Complete #" + rows);
                        }
                        catch (ex) {
                            self.log(VIS.Logging.Level.SEVERE, "", ex);
                        }
                        self = null;


                    },
                });



                //executeDSet(finalSql, sqlParaCount > 0 ? param : null, function (ds) {
                //try {
                //    var dt = ds.getTable(0);
                //    var len = dt.getRows().length;

                //    for (var i = 0; i < len; i++) {
                //        //	Max out
                //        if (rows++ > self.maxRows) {
                //            self.log.warning("Over Max Rows - " + rows);
                //            break;
                //        }

                //        self.lookup[dt.getRow(i).getCell(0)] = { Key: dt.getRow(i).getCell(0), Name: VIS.Utility.Util.getValueOfString(dt.getRow(i).getCell(1)) };
                //    }
                //    ds.dispose();
                //    ds = null;
                //    self.log.fine("Complete #" + rows);
                //}
                //catch (ex) {
                //    self.log(VIS.Logging.Level.SEVERE, finalSql, ex);
                //}
                //self = null;
                //});
            }
        }
        catch (e) {
            this.log.log(VIS.Logging.Level.SEVERE, finalSql, e);
        }

        if (sync && $.isEmptyObject(this.lookup)) {
            this.log.Finer(finalSql);
        }
    };
    MLocatorLookup.prototype.get = function (key) {
        if (key == null) {
            return null;
        }
        var key1 = Number(key);
        //	try cache
        var pp = this.lookup[key1];
        if (pp != null) {
            return pp;
        }
        //	Try to get it directly
        return this.getDirect(key, true, null);
    };
    MLocatorLookup.prototype.getDisplay = function (value) {
        if (value == null) {
            return "";
        }
        var display = this.get(value);
        if (display == null)
            return "<" + value.toString() + ">";
        return display.Name;
    };
    MLocatorLookup.prototype.getData = function (mandatory, onlyValidated, onlyActive, temporary) {
        var collection = this.lookup; //valuestodo

        var arr = $.map(this.lookup, function (v) {
            return v;
        })
        return arr;
    };
    MLocatorLookup.prototype.refresh = function () {

        this.log.fine("start");
        try {
            this.load(true);
        }
        catch (e) {
            this.log.severe("#" + this.lookup.Count);
        }
        this.log.info("#" + this.lookup.Count);
        return this.lookup.length;
    };
    MLocatorLookup.prototype.getColumnName = function () {
        return "M_Locator.M_Locator_ID";
    };

    /**
    * Get Zoom
    * 
    * @return Zoom AD_Window_ID
    */
    MLocatorLookup.prototype.getZoomWindow = function () {
        if (arguments.length == 0) {
            return this.info.zoomWindow;
        }
        var query = arguments[0];

        /*
    * Handle cases where you have multiple windows for a single table. So
    * far it is just the tables that have a PO Window defined. For eg.,
    * Order, Invoice and Shipments This will need to be expanded to add
    * more tables if they have multiple windows.
    */

        var AD_Window_ID = VIS.ZoomTarget.getZoomAD_Window_ID("M_Locator",
            this.windowNo, query.getWhereClause(), VIS.context.isSOTrx(this.windowNo));
        return AD_Window_ID;
    }; // getZoomWindow


    MLocatorLookup.prototype.dispose = function () {
        this.disposeLocal();
        this.$super.dispose.call(this);
        this.lookupDirect = null;
        this.directNullKey = null;
        this.log = null;
        this.disposeLocal = null;
    };
    //END




    //4. MAccountLookup
    function MAccountLookup(ctx, windowNo) {
        Lookup.call(this, null, null, ctx, windowNo, VIS.DisplayType.TableDir);
        this.log = VIS.Logging.VLogger.getVLogger("MAccountLookup");
        /** Account_ID			*/
        this.C_ValidCombination_ID = 0;
        this.combination;
        this.description;
        this.lst = {};
    };
    VIS.Utility.inheritPrototype(MAccountLookup, Lookup);//Inherit
    /**
     *	Get Display for Value
     *  @param value value
     *  @return String
     */
    MAccountLookup.prototype.getDisplay = function (value) {
        if (!this.containsKey(value))
            return "<" + value.toString() + ">";
        return this.toStringX();
    };	//	getDisplay
    /**
     *	Return String representation
     *  @return Combination
     */
    MAccountLookup.prototype.toStringX = function () {
        if (this.C_ValidCombination_ID == 0)
            return "";
        return this.combination;
    };	//
    /**
     *  The Lookup contains the key
     *  @param key key
     *  @return true if exists
     */
    MAccountLookup.prototype.containsKey = function (key) {
        var intValue = 0;
        if (key != null)
            intValue = parseInt(key.toString());
        //
        return this.load(intValue);
    };   //  containsKey
    /**
     *	Load C_ValidCombination with ID
     *  @param ID C_ValidCombination_ID
     *  @return true if found
     */
    MAccountLookup.prototype.load = function (ID) {
        if (ID == 0)						//	new
        {
            this.C_ValidCombination_ID = 0;
            this.combination = "";
            this.description = "";
            return true;
        }
        if (ID == this.C_ValidCombination_ID)	//	already loaded
            return true;




        var lstObj = this.lst[ID];
        if (lstObj) {
            this.C_ValidCombination_ID = ID;
            this.combination = lstObj["C"];
            this.description = lstObj["D"];
            return true;
        }


        var text = VIS.MLookupCache.getRecordLookup(this.getWindowNo(), this.getTabNo(), "c_validcombination_id", ID);

        if (text) {
            lstObj = { "C": text, "D": "-" };
            this.C_ValidCombination_ID = ID;
            this.combination = lstObj["C"];
            this.description = lstObj["D"];
            this.lst[ID] = lstObj;
            return true;
        }

        var SQL = "VIS_94";

        var param = [];
        param[0] = new VIS.DB.SqlParam("@ID", ID);

        var self = this;
        executeReader(SQL, param, function (dr) {

            var dr = null;
            try {
                //	Prepare Statement
                dr = executeReader(SQL, param);
                if (!dr.read())
                    return false;

                self.lst[dr.getInt(0)] = { "C": dr.getString(1), "D": dr.getString(2) }
                // this.C_ValidCombination_ID = dr.getInt(0);
                //  this.combination = dr.getString(1);
                //  this.description = dr.getString(2);
            }
            catch (e) {
                return false;
            }
            finally {
            }
        });
        return false;
    };	//	load
    /**
     *	Get underlying fully qualified Table.Column Name
     *  @return ""
     */
    MAccountLookup.prototype.getColumnName = function () {
        return "";
    };
    /**
     *	Return data as sorted Array.
     *  Used in Web Interface
     *  @param mandatory mandatory
     *  @param onlyValidated only valid
     *  @param onlyActive only active
     * 	@param temporary force load for temporary display
     *  @return ArrayList with KeyNamePair
     */
    MAccountLookup.prototype.getData = function (mandatory, onlyValidated, onlyActive, temporary) {
        var list = [];
        if (!mandatory)
            list.push({ "Key": -1, "Name": "" });
        //
        //var sql = "SELECT C_ValidCombination_ID, Combination, Description "
        //    + "FROM C_ValidCombination WHERE AD_Client_ID=" + this.ctx.getAD_Client_ID();
        //if (onlyActive)
        //    sql += " AND IsActive='Y'";
        //sql.append(" ORDER BY 2");

        var dr = null;
        try {

            $.ajax({
                type: 'Get',
                async: false,
                url: VIS.Application.contextUrl + "Form/GetValidAccountCombination",
                data: {
                    AD_Client_ID: AD_Client_ID,
                    onlyActive: onlyActive
                },
                success: function (data) {
                    dr = new VIS.DB.DataReader().toJson(jString);
                },
            });

            //dr = executeReader(sql.toString(), null);
            while (dr.read())
                list.push({ "Key": dr.getInt(0), "Name": dr.getString(1) + " - " + dr.getString(2) });
        }
        catch (e) {
            this.log.log(Level.SEVERE, sql, e);
        }
        //  Sort & return
        return list;
    };   //  getData
    //END




    //5. MPAttributeLookup
    function MPAttributeLookup(ctx, windowNo) {
        Lookup.call(this, null, null, ctx, windowNo, VIS.DisplayType.TableDir);
        this.NO_INSTANCE = { "Key": 0, "Name": "" };
        this.log = VIS.Logging.VLogger.getVLogger("MPAttributeLookup");
    };
    VIS.Utility.inheritPrototype(MPAttributeLookup, Lookup);//Inherit
    /**
     *	Get Display for Value (not cached)
     *  @param value Location_ID
     *  @return String Value
     */
    MPAttributeLookup.prototype.getDisplay = function (value) {
        if (value == null)
            return "";
        var pp = this.get(value);
        if (pp == null)
            return "<" + value.toString() + ">";
        return pp.Name;
    };	//	getDisplay
    /**
     *	Get Object of Key Value
     *  @param value value
     *  @return Object or null
     */
    MPAttributeLookup.prototype.get = function (value) {
        if (value == null)
            return null;
        var M_AttributeSetInstance_ID = 0;
        if (value instanceof Number)
            M_AttributeSetInstance_ID = value;
        else {
            try {
                M_AttributeSetInstance_ID = parseInt(value.toString());
            }
            catch (e) {
                this.log.log(Level.SEVERE, "Value=" + value, e);
            }
        }

        if (M_AttributeSetInstance_ID == 0)
            return this.NO_INSTANCE;

        var description = null;
        try {

            var param = [];
            param[0] = new VIS.DB.SqlParam("@M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
            var dr = executeReader("VIS_95", param);
            if (dr.read()) {
                description = dr.getString(0);			//	Description
                if (description == null || description.length == 0) {
                    if (VIS.Logging.VLogMgt.getIsLevelFine())
                        description = "{" + M_AttributeSetInstance_ID + "}";
                    else
                        description = "";
                }
            }
            dr.dispose();
            dr = null;
        }
        catch (e) {
            this.log.log(Level.SEVERE, "get", e);
        }
        if (description == null)
            return null;
        return { "Key": M_AttributeSetInstance_ID, "Name": description };
    };	//	get

    MPAttributeLookup.prototype.getColumnName = function () {
        return "M_AttributeSetInstance_ID";
    };	//	getColumnName

    MPAttributeLookup.prototype.dispose = function () {
        this.$super.dispose.call(this);
        this.NO_INSTANCE = null;
        this.log = null;
    };
    //END


    //6. MGAttributeLookup
    function MGAttributeLookup(ctx, windowNo) {
        Lookup.call(this, null, null, ctx, windowNo, VIS.DisplayType.TableDir);
        this.NO_INSTANCE = { "Key": 0, "Name": "" };
        this.log = VIS.Logging.VLogger.getVLogger("MGAttributeLookup");
    };

    VIS.Utility.inheritPrototype(MGAttributeLookup, Lookup);//Inherit

    /**
     *	Get Display for Value (not cached)
     *  @param value Location_ID
     *  @return String Value
     */
    MGAttributeLookup.prototype.getDisplay = function (value) {
        if (value == null)
            return "";
        var pp = this.get(value);
        if (pp == null)
            return "<" + value.toString() + ">";
        return pp.Name;
    };

    /**
     *	Get Object of Key Value
     *  @param value value
     *  @return Object or null
     */
    MGAttributeLookup.prototype.get = function (value) {
        if (value == null)
            return null;
        var C_GenttributeSetInstance_ID = 0;
        if (value instanceof Number)
            C_GenttributeSetInstance_ID = value;
        else {
            try {
                C_GenttributeSetInstance_ID = parseInt(value.toString());
            }
            catch (e) {
                this.log.log(Level.SEVERE, "Value=" + value, e);
            }
        }

        if (C_GenttributeSetInstance_ID == 0)
            return this.NO_INSTANCE;

        var description = null;
        try {
            var param = [];
            param[0] = new VIS.DB.SqlParam("@C_GenttributeSetInstance_ID", C_GenttributeSetInstance_ID);
            var dr = executeReader("VIS_96", param);
            if (dr.read()) {
                description = dr.getString(0);			//	Description
                if (description == null || description.length == 0) {
                    if (VIS.Logging.VLogMgt.getIsLevelFine())
                        description = "{" + C_GenttributeSetInstance_ID + "}";
                    else
                        description = "";
                }
            }
            dr.dispose();
            dr = null;
        }
        catch (e) {
            this.log.log(Level.SEVERE, "get", e);
        }
        if (description == null)
            return null;
        return { "Key": C_GenttributeSetInstance_ID, "Name": description };
    };	//	get


    MGAttributeLookup.prototype.getColumnName = function () {
        return "C_GenAttributeSetInstance_ID";
    };	//	getColumnName

    MGAttributeLookup.prototype.dispose = function () {
        this.$super.dispose.call(this);
        this.NO_INSTANCE = null;
        this.log = null;
    };

    MGAttributeLookup.prototype.getData = function (mandatory, onlyValidated, onlyActive, temporary) {
        var list = [];
        if (!mandatory)
            list.push({ "Key": -1, "Name": "" });
        //var sql = "SELECT ASI.C_GenAttributeSetInstance_ID, ASI.Description " +
        //    " from C_GenAttributeSetInstance ASI, M_Product P WHERE ASI.C_GenAttributeSet_ID  = " + VIS.Env.getCtx().getContextAsInt(this.windowNo, "C_GenAttributeSet_ID");
        //if (onlyActive)
        //    sql += " AND IsActive='Y'";
        //sql.append(" ORDER BY 2");



        var dr = null;
        try {
            //dr = executeReader(sql.toString(), null);
            $.ajax({
                type: 'Get',
                async: false,
                url: VIS.Application.contextUrl + "Form/GenAttributeSetInstance",
                data: {
                    C_GenAttributeSet_ID: VIS.Env.getCtx().getContextAsInt(this.windowNo, "C_GenAttributeSet_ID"),
                    onlyActive: onlyActive
                },
                success: function (data) {
                    dr = new VIS.DB.DataReader().toJson(jString);
                },
            });

            while (dr.read())
                list.push({ "Key": dr.getInt(0), "Name": dr.getString(1) + " - " + dr.getString(2) });
        }
        catch (e) {
            this.log.log(Level.SEVERE, sql, e);
        }
        //  Sort & return
        return list;
    };

    //END


    /*******************************************************************
                   MLookupFactory
    *******************************************************************/


    VIS.MLookupFactory =
    {

        get: function (ctx, windowNo, column_ID, AD_Reference_ID, columnName, AD_Reference_Value_ID, isParent, validationCode) {

            //var lookup = this.getLookupInfo(ctx, windowNo, AD_Reference_ID, column_ID, columnName, AD_Reference_Value_ID, isParent, validationCode);
            //if (lookup == null)
            //    throw new IllegalArgumentException("MLookup.create - no LookupInfo");
            //return lookup;
            //  var ctxstr = JSON.stringify(ctx);

            var d = {
                'ctx': ctx.getWindowCtx(windowNo),
                'windowNo': windowNo,
                'column_ID': column_ID,
                'AD_Reference_ID': AD_Reference_ID,
                'columnName': columnName,
                'AD_Reference_Value_ID': AD_Reference_Value_ID,
                'isParent': isParent,
                'validationCode': validationCode
            };

            //var paramStr = "ctx="+ctxstr+"&windowNo=" + windowNo + "&column_ID=" + column_ID + "&AD_Reference_ID="
            //    + AD_Reference_ID + "&columnName=" + columnName + "&AD_Reference_Value_ID=" + AD_Reference_Value_ID
            //    + "&isParent=" + isParent + "&validationCode="+validationCode;

            var lookup = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "JsonData/GetLookup", d);
            var df = new VIS.MLookup(lookup._vInfo, lookup);
            return df;
        },
        getMLookUp: function (ctx, windowNo, Column_ID, AD_Reference_ID) {
            var columnName = "";
            var AD_Reference_Value_ID = 0;
            var isParent = false;
            var validationCode = "";
            //
            var sql = "VIS_97";
            var dr = null;
            try {
                var param = [];
                param[0] = new VIS.DB.SqlParam("@Column_ID", Column_ID);
                dr = executeReader(sql.toString(), param, null);
                if (dr.read()) {
                    columnName = dr.getString(0);
                    AD_Reference_Value_ID = dr.getInt(1);
                    isParent = "Y".equals(dr.getString(2));
                    validationCode = dr.getString(3);
                }
                else {
                    //this.log.Log(Level.SEVERE, "Column Not Found - AD_Column_ID=" + Column_ID);
                }
                dr.close();
                dr = null;
            }
            catch (ex) {
                if (dr != null) {
                    dr.close();
                    dr = null;
                }
                // s_log.Log(Level.SEVERE, sql, ex);
            }

            return VIS.MLookupFactory.get(ctx, windowNo, Column_ID, AD_Reference_ID, columnName, AD_Reference_Value_ID, isParent, validationCode);
            //var lookup = new VIS.MLookup(ctx, windowNo, AD_Reference_ID);
            //var info = GetLookUpInfo(VIS.Env.getLanguage(ctx), windowNo, lookup, Column_ID, columnName, AD_Reference_Value_ID, isParent, validationCode);
            //if (info == null) {
            //   return null;
            //}
            //return lookup.initialize(info);
        },
        GetLookUpInfo: function (ctx, windowNum, AD_Reference_ID, column_ID, columnName, AD_Reference_Value_ID, isParent, validationCode) {

            var data = {
                WindowNum: windowNum,
                AD_Reference_ID: AD_Reference_ID,
                Column_ID: column_ID,
                columnName: columnName,
                AD_Reference_Value_ID: AD_Reference_Value_ID,
                IsParent: isParent,
                ValidationCode: validationCode
            };

        },

        getLookup_TableDirEmbed: function (language, columnName, baseTable, baseColumn) {
            var tableName = columnName.substring(0, columnName.length - 3);
            //	get display column name (first identifier column)
            var sql = "VIS_98";
            var list = [];
            var param = [];
            param[0] = new VIS.DB.SqlParam("@tableName", tableName);

            try {
                var dr = executeReader(sql, param);
                while (dr.read()) {
                    list.push({ 'ColumnName': dr.getString(0), 'IsTranslated': dr.getString(1), 'DisplayType': dr.getInt(2), 'AD_Reference_Value_ID': dr.getInt(3) });
                }
                dr.close();
                dr = null;
            }
            catch (e) {
                return "";
            }
            //  Do we have columns ?
            if (list.length == 0) {
                return "";
            }

            var embedSQL = "SELECT ";

            for (var i = 0; i < list.length; i++) {
                if (i > 0) {
                    embedSQL = embedSQL.concat("||' - '||");
                }
                var ldc = list[i];

                //  date, number
                if (VIS.DisplayType.IsDate(ldc.DisplayType) || VIS.DisplayType.IsNumeric(ldc.DisplayType)) {
                    embedSQL = embedSQL.concat(VIS.DB.to_char(tableName + "." + ldc.ColumnName, ldc.DisplayType, VIS.Env.getAD_Language(VIS.Env.getCtx())));
                }
                //  TableDir
                else if ((ldc.DisplayType == VIS.DisplayType.TableDir || ldc.DisplayType == VIS.DisplayType.Search) && ldc.ColumnName.endsWith("_ID")) {
                    alert("TableDir");
                    //var embeddedSQL = GetLookup_TableDirEmbed(language, ldc.ColumnName, tableName);
                    //embedSQL = embedSQL.concat("(").concat(embeddedSQL).concat(")");
                }
                //  String
                else {
                    //jz EDB || problem
                    //if (DatabaseType.IsPostgre)
                    //    embedSQL.concat("COALESCE(TO_CHAR(").concat(tableName).concat(".").Append(ldc.ColumnName).concat("),'')");
                    //else
                    embedSQL = embedSQL.concat(tableName).concat(".").concat(ldc.ColumnName);
                }
            }

            embedSQL = embedSQL.concat(" FROM ").concat(tableName);
            embedSQL = embedSQL.concat(" WHERE ").concat(baseTable).concat(".").concat(baseColumn);
            embedSQL = embedSQL.concat("=").concat(tableName).concat(".").concat(columnName);
            //
            return embedSQL.toString();
        }
    }


    /*******************************************************************
                   END
    *******************************************************************/



    VIS.MLookupCache = {

        /** Static Logger */
        s_log: VIS.Logging.VLogger.getVLogger("MLookupCache"),
        /** Static Lookup data with MLookupInfo -> HashMap */
        s_loadedLookups: new VIS.CCache("MLookupCache"),
        s_sentLookups: new VIS.CCache("MLookupCacheSent"),
        s_windowLookup: {},
        s_windowRecordLookup: {},

        getKey: function (info) {
            if (info == null)
                return VIS.Env.currentTimeMillis();
            //
            var sb = info.windowNo + ":";
            //sb += info.column_ID;
            sb += ":" + info.keyColumn +
                info.AD_Reference_Value_ID + info.query +
                info.validationCode;
            // does not include ctx
            return sb;
        }, // getKey

        loadFromCache: function (info, cacheTarget) {
            var key = this.getKey(info);
            var cache = null;
            var retVal = true;

            if (this.s_loadedLookups.contains(key))
                cache = this.s_loadedLookups.get(key);
            if (cache == null)
                retVal = false;
            //  Nothing cached
            if (retVal && $.isEmptyObject(cache)) {
                this.s_loadedLookups.remove(key);
                retVal = false;
            }

            if (retVal) {
                //  we can use iterator, as the lookup loading is complete (i.e. no additional entries)
                //var lookupTarget = cacheTarget.getCLookup();
                // lookupTarget = jQuery.extend({}, cache);
                cacheTarget.setCLookup(jQuery.extend({}, cache));

                //while (iterator.MoveNext())
                //{
                //    Object cacheKey = iterator.Current;
                //    NamePair cacheData = cache[cacheKey];
                //    lookupTarget[cacheKey] = cacheData;
                //}
                cacheTarget.setIsLoading(false);
            }


            if (!retVal) {
                retVal = true;
                if (!this.s_sentLookups.contains(key)) {
                    this.s_sentLookups.add(key, []);
                    retVal = false;
                }
                else if (this.s_sentLookups.get(key).indexOf(cacheTarget) == -1) {
                    cacheTarget.setIsLoading(true);
                    this.s_sentLookups.get(key).push(cacheTarget);
                }
                else {
                    //
                }
            }
            return retVal;
        },

        loadEnd: function (info, cacheLookup) {
            var key = this.getKey(info);

            if (cacheLookup.getCLookup() && !$.isEmptyObject(cacheLookup.getCLookup())) {

                this.s_loadedLookups.add(key, cacheLookup.getCLookup());

                if (this.s_sentLookups.contains(key)) {
                    //  we can use iterator, as the lookup loading is complete (i.e. no additional entries)
                    var list = this.s_sentLookups.get(key);

                    if (list.length > 0) {
                        var s = "";
                    }

                    for (var i = 0; i < list.length; i++) {
                        var lookupTarget = list[i];


                        lookupTarget.setCLookup($.extend({}, cacheLookup.getCLookup()));



                        //}
                        lookupTarget.setIsLoading(false);
                    }
                    list.length = 0;
                    this.s_sentLookups.remove(key);
                }
            }
            else {
                if (this.s_sentLookups.contains(key)) {
                    var list = this.s_sentLookups.get(key);

                    for (var i = 0; i < list.length; i++) {
                        list[i].setIsLoading(false);
                    }
                    list.length = 0;
                    this.s_sentLookups.remove(key);
                }
            }
        },

        cacheReset: function (windowNo) {
            var keyStart = windowNo.toString() + ":";
            var startNo = this.s_loadedLookups.size();
            //  find keys of Lookups to delete
            var toDelete = [];
            for (var j = 0, k = this.s_loadedLookups.m_cacheK.length; j < k; j++) {
                var info = this.s_loadedLookups.m_cacheK[j];
                try {
                    if (info != null && info.startsWith(keyStart))
                        toDelete.push(info);
                }
                catch (e) {
                    var s = "";
                }
            }

            //  Do the actual delete
            for (var i = 0; i < toDelete.length; i++)
                this.s_loadedLookups.remove(toDelete[i]);
            var endNo = this.s_loadedLookups.size();

            // Remove window tab Record lookup
            for (var prop in this.s_windowRecordLookup) {
                if (prop.startsWith(keyStart))
                    delete this.s_windowRecordLookup[prop];
            }
        },

        addWindowLookup: function (windowNo, lookup) {

            if (this.s_windowLookup[windowNo]) {
                this.s_windowLookup[windowNo].push(lookup);
            }
            else {
                var lst = [];
                lst.push(lookup);
                this.s_windowLookup[windowNo] = lst;
            }
        },

        initWindowLookup: function (windowNo) {
            if (this.s_windowLookup[windowNo]) {
                var arr = this.s_windowLookup[windowNo];
                for (var i = 0, j = arr.length; i < j; i++) {
                    arr[i].initialize();
                }
                arr.length = 0;
                delete this.s_windowLookup[windowNo];
            }
        },

        /**
         * Add window tab lookup direct record
         * @param {any} windowNo
         * @param {any} tabNo
         * @param {any} lookupDirect
         */
        addRecordLookup: function (windowNo, tabNo, lookupDirect) {
            var key = windowNo + ':' + tabNo;
            this.s_windowRecordLookup[key] = lookupDirect;
        },

        /**
         * Get Window Record Lookup
         * @param {any} windowNo
         * @param {any} tabNo
         * @param {any} keyCol
         * @param {any} key
         */
        getRecordLookup: function (windowNo, tabNo, keyCol, key) {
            var lookup = this.s_windowRecordLookup[windowNo + ':' + tabNo];
            if (lookup && keyCol in lookup)
                return lookup[keyCol][key];
            return null;
        }
    };


    ////2. Amount Division Lookup
    function MAmtDivLookup(ctx, windowNo) {
        Lookup.call(this, null, null, ctx, windowNo, VIS.DisplayType.AmtDimension);
        this.lookup = {};
    };


    VIS.Utility.inheritPrototype(MAmtDivLookup, Lookup);//Inherit


    MAmtDivLookup.prototype.cacheLookup = null;


    MAmtDivLookup.prototype.getDisplay = function (value) {
        if (value == null)
            return "";
        var pp = this.get(value);
        if (pp == null)
            return "<" + value.toString() + ">";
        return pp;
    };
    /**
     *	Get Object of Key Value
     *  @param value value
     *  @return Object or null
     */
    MAmtDivLookup.prototype.get = function (value) {
        if (value == null)
            return null;
        var C_DimAmt_ID = 0;
        if (value instanceof Number)
            C_DimAmt_ID = value;
        else {
            try {
                C_DimAmt_ID = parseInt(value.toString());
            }
            catch (e) {
                this.log.log(Level.SEVERE, "Value=" + value, e);
            }
        }
        if (C_DimAmt_ID in this.lookup) {
            return this.lookup[C_DimAmt_ID];
        }

        if (C_DimAmt_ID == 0)
            return this.NO_INSTANCE;

        var description = null;
        try {
            var param = [];
            param[0] = new VIS.DB.SqlParam("@C_DimAmt_ID", C_DimAmt_ID);

            var dr = executeReader("VIS_99", param);
            if (dr.read()) {
                description = dr.getString(0);			//	Description
                if (description == null || description.length == 0) {
                    if (VIS.Logging.VLogMgt.getIsLevelFine())
                        description = "{" + C_DimAmt_ID + "}";
                    else
                        description = "";
                }
            }
            dr.dispose();
            dr = null;
            this.lookup[C_DimAmt_ID] = description;
        }
        catch (e) {
            this.log.log(Level.SEVERE, "get", e);
        }
        if (description == null)
            return null;
        //return { "Key": C_DimAmt_ID, "Name": description };
        return description;
    };	//	get
    MAmtDivLookup.prototype.getColumnName = function () {
        return "C_DimAmt_ID";
    };

    /**
     *	Get Object of Key Value
     *  @param value value
     *  @return Object or null
     */
    MAmtDivLookup.prototype.refreshAmountDivision = function (value) {
        if (value == null)
            return null;
        var C_DimAmt_ID = 0;
        if (value instanceof Number)
            C_DimAmt_ID = value;
        else {
            try {
                C_DimAmt_ID = parseInt(value.toString());
            }
            catch (e) {
                this.log.log(Level.SEVERE, "Value=" + value, e);
            }
        }

        if (C_DimAmt_ID == 0)
            return this.NO_INSTANCE;

        var description = null;
        try {
            var param = [];
            param[0] = new VIS.DB.SqlParam("@C_DimAmt_ID", C_DimAmt_ID);

            var dr = executeReader("VIS_99", param);
            if (dr.read()) {
                description = dr.getString(0);			//	Description
                if (description == null || description.length == 0) {
                    if (VIS.Logging.VLogMgt.getIsLevelFine())
                        description = "{" + C_DimAmt_ID + "}";
                    else
                        description = "";
                }
            }
            dr.dispose();
            dr = null;
            this.lookup[C_DimAmt_ID] = description;
        }
        catch (e) {
            this.log.log(Level.SEVERE, "get", e);
        }
        if (description == null)
            return null;
        //return { "Key": C_DimAmt_ID, "Name": description };
        return description;
    };

    //MAmtDivLookup.prototype.load = function () {

    //    var self = this;
    //    self.lstLatLng = {};

    //    if (MAmtDivLookup.prototype.cacheLookup != null) {
    //        self.lookup = MAmtDivLookup.prototype.cacheLookup.LocLookup;
    //        self.lstLatLng = MAmtDivLookup.prototype.cacheLookup.LocLatLng;
    //        self = null;
    //        return;
    //    }

    //    VIS.3.getJSONData(VIS.Application.contextUrl + "Location" + "/GetLocations", "", function (data) {

    //        if (!MAmtDivLookup.prototype.cacheLookup) {
    //            MAmtDivLookup.prototype.cacheLookup = data;
    //        }

    //        self.lookup = data.LocLookup;
    //        self.lstLatLng = data.LocLatLng;
    //        self = null;
    //    });
    //};

    //MAmtDivLookup.prototype.getLatLng = function (key) {
    //    if (key == null || key.toString().len < 1)
    //        return null;
    //    if (key in this.lstLatLng) {
    //        return this.lstLatLng[key];
    //    }
    //    var data = this.getLocation(key, true);
    //    return data;
    //};


    MAmtDivLookup.prototype.dispose = function () {
        this.lstLatLng = null;
        this.$super.dispose.call(this);
        this.lookup = null;
    };
    //   END



    ////3. Product Container Lookup
    function MProductContainerLookup(ctx, windowNo) {

        Lookup.call(this, null, null, ctx, windowNo, VIS.DisplayType.ProductContainer);

        this.log = VIS.Logging.VLogger.getVLogger("MProductContainerLookup");

        this.lookup = {};
    };


    VIS.Utility.inheritPrototype(MProductContainerLookup, Lookup);//Inherit


    MProductContainerLookup.prototype.cacheLookup = null;


    MProductContainerLookup.prototype.getDisplay = function (value) {
        if (value == null)
            return "";
        var pp = this.get(value);
        if (pp == null)
            return "<" + value.toString() + ">";
        return pp.Name;
    };
    /**
     *	Get Object of Key Value
     *  @param value value
     *  @return Object or null
     */
    MProductContainerLookup.prototype.get = function (value) {
        if (value == null)
            return null;
        var M_ProductContainer_ID = 0;
        if (value instanceof Number)
            M_ProductContainer_ID = value;
        else {
            try {
                M_ProductContainer_ID = parseInt(value.toString());
            }
            catch (e) {
                this.log.log(Level.SEVERE, "Value=" + value, e);
            }
        }

        if (M_ProductContainer_ID in this.lookup) {
            return this.lookup[M_ProductContainer_ID];
        }


        if (M_ProductContainer_ID == 0)
            return this.NO_INSTANCE;

        var description = null;
        //try {
        //    var param = [];
        //    param[0] = new VIS.DB.SqlParam("@M_ProductContainer_ID", M_ProductContainer_ID);

        //    var dr = executeReader("VIS_99", param);
        //    if (dr.read()) {
        //        description = dr.getString(0);			//	Description
        //        if (description == null || description.length == 0) {
        //            if (VIS.Logging.VLogMgt.getIsLevelFine())
        //                description = "{" + M_ProductContainer_ID + "}";
        //            else
        //                description = "";
        //        }
        //    }
        //    dr.dispose();
        //    dr = null;
        //}
        //catch (e) {
        //    this.log.log(Level.SEVERE, "get", e);
        //}


        $.ajax({
            url: baseUrl + "ProductContainer/GetProductContainerInfo",
            data: { ID: M_ProductContainer_ID },
            async: false,
            success: function (result) {
                if (!result) {
                    if (VIS.Logging.VLogMgt.getIsLevelFine())
                        description = "{" + M_ProductContainer_ID + "}";
                    else
                        description = "";
                }
                else {
                    description = result;
                }
            },
            error: function (err) {
                this.log.log(Level.SEVERE, "get", err);
            }
        });

        this.lookup[M_ProductContainer_ID] = description;
        if (description == null)
            return null;
        return { "Key": M_ProductContainer_ID, "Name": description };
    };	//	get
    MProductContainerLookup.prototype.getColumnName = function () {
        return "M_ProductContainer_ID";
    };


    MProductContainerLookup.prototype.dispose = function () {
        this.lstLatLng = null;
        this.$super.dispose.call(this);
        this.lookup = null;
    };
    //   END





    /* Namespace */
    VIS.MLookup = MLookup;
    VIS.MLocationLookup = MLocationLookup;
    VIS.MAccountLookup = MAccountLookup;
    VIS.MPAttributeLookup = MPAttributeLookup;
    VIS.MGAttributeLookup = MGAttributeLookup;
    VIS.MLocatorLookup = MLocatorLookup;
    VIS.MAmtDivLookup = MAmtDivLookup;
    VIS.MProductContainerLookup = MProductContainerLookup;

    /* END */
})(VIS, jQuery);
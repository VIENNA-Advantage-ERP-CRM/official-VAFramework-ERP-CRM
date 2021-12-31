; VIS.Controls = {};
; (function ($, VIS) {

    /**
     *	System Display Types.
     *  <pre>
     *	SELECT AD_Reference_ID, Name FROM AD_Reference WHERE ValidationType = 'D'
     *  </pre>
     */



    var baseUrl = VIS.Application.contextUrl;
    var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";
    var nonQueryUrl = baseUrl + "JsonData/ExecuteNonQuer";
    var dSetUrl = baseUrl + "Form/JDataSet";

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


    var executeScalar = function (sql, params, callback) {
        var async = callback ? true : false;
        var dataIn = { sql: sql, page: 1, pageSize: 0 }
        if (params) {
            dataIn.param = params;
        }
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

    var executeQueries = function (sqls, params, callback) {
        var async = callback ? true : false;
        var ret = null;
        var dataIn = { sql: sqls.join("/"), param: params };

        //   dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
        $.ajax({
            url: nonQueryUrl + 'iesWithCode',
            type: "POST",
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            async: async,
            data: JSON.stringify(dataIn)
        }).done(function (json) {
            ret = json;
            if (callback) {
                callback(json);
            }
        });

        return ret;
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

    var executeScalarEn = function (sql, params, callback) {
        var async = callback ? true : false;
        var dataIn = { sql: sql, page: 1, pageSize: 0 }
        dataIn.sql = VIS.secureEngine.encrypt(dataIn.sql);
        if (params) {
            dataIn.param = params;
        }
        var value = null;


        getDataSetJStringEn(dataIn, async, function (jString) {
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

    function getDataSetJStringEn(data, async, callback) {
        var result = null;
        //data.sql = VIS.secureEngine.encrypt(data.sql);
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


    VIS.DisplayType = {
        String: 10, Integer: 11, Amount: 12, ID: 13, Text: 14, Date: 15, DateTime: 16, List: 17, Table: 18, TableDir: 19,
        YesNo: 20, Location: 21, Number: 22, Binary: 23, Time: 24, Account: 25, RowID: 26, Color: 27, Button: 28, Quantity: 29,
        Search: 30, Locator: 31, Image: 32, Assignment: 33, Memo: 34, PAttribute: 35, TextLong: 36, CostPrice: 37, FilePath: 38,
        FileName: 39, URL: 40, PrinterName: 42, Label: 44, MultiKey: 45, GAttribute: 46, AmtDimension: 47, ProductContainer: 48, ProgressBar: 49,

        IsString: function (displayType) {
            return VIS.DisplayType.String == displayType;
        },
        IsText: function (displayType) {
            if (displayType == VIS.DisplayType.String || displayType == VIS.DisplayType.Text
                || displayType == VIS.DisplayType.TextLong || displayType == VIS.DisplayType.Memo
                || displayType == VIS.DisplayType.FilePath || displayType == VIS.DisplayType.FileName
                || displayType == VIS.DisplayType.URL || displayType == VIS.DisplayType.PrinterName)
                return true;
            return false;
        },
        IsLookup: function (displayType) {
            if (VIS.DisplayType.List == displayType || displayType == VIS.DisplayType.TableDir || displayType == VIS.DisplayType.Table
                || displayType == VIS.DisplayType.Search || displayType == VIS.DisplayType.MultiKey || displayType == VIS.DisplayType.ProductContainer) {
                return true;
            }
            return false;
        },
        IsInt: function (displayType) {
            if (displayType == VIS.DisplayType.Integer || displayType == VIS.DisplayType.ProgressBar) {
                return true;
            }
            return false;
        },
        IsSearch: function (displayType) {
            if (displayType == this.Search) {
                return true;
            }
            return false;
        },
        IsID: function (displayType) {
            if (displayType == VIS.DisplayType.ID || displayType == VIS.DisplayType.Table || displayType == VIS.DisplayType.TableDir
                || displayType == VIS.DisplayType.Search || displayType == VIS.DisplayType.Location || displayType == VIS.DisplayType.Locator
                || displayType == VIS.DisplayType.Account || displayType == VIS.DisplayType.Assignment || displayType == VIS.DisplayType.PAttribute
                || displayType == VIS.DisplayType.Image || displayType == VIS.DisplayType.Color || displayType == VIS.DisplayType.AmtDimension || displayType == VIS.DisplayType.ProductContainer)
                return true;
            return false;
        },
        IsNumeric: function (displayType) {
            if (displayType == VIS.DisplayType.Amount || displayType == VIS.DisplayType.Number || displayType == VIS.DisplayType.CostPrice
                || displayType == VIS.DisplayType.Integer || displayType == VIS.DisplayType.Quantity || displayType == VIS.DisplayType.ProgressBar)
                return true;
            return false;
        },	//	
        IsDate: function (displayType) {
            if (displayType == VIS.DisplayType.Date || displayType == VIS.DisplayType.DateTime || displayType == VIS.DisplayType.Time)
                return true;
            return false;
        },	//	isDate
        IsLOB: function (displayType) {
            if (displayType == VIS.DisplayType.Binary
                || displayType == VIS.DisplayType.TextLong)
                return true;
            return false;
        },
        MAX_DIGITS: 28, //  Oracle Standard Limitation 38 digits
        INTEGER_DIGITS: 10,
        MAX_FRACTION: 12,
        AMOUNT_FRACTION: 2,
        GetNumberFormat: function (displayType) {

            var format = null;
            if (displayType == this.Integer || displayType == this.ProgressBar) {
                format = new VIS.Format(this.INTEGER_DIGITS, 0, 0);
            }
            else if (displayType == this.Quantity) {
                format = new VIS.Format(this.MAX_DIGITS, this.MAX_FRACTION, 0);
            }
            else if (displayType == this.Amount ) {
                format = new VIS.Format(this.MAX_DIGITS, this.MAX_FRACTION, this.AMOUNT_FRACTION);
            }
            else if (displayType == this.CostPrice) {
                format = new VIS.Format(this.MAX_DIGITS, this.MAX_FRACTION, this.AMOUNT_FRACTION);
            }
            else //	if (displayType == Number)
            {
                format = new VIS.Format(this.MAX_DIGITS, this.MAX_FRACTION, 1);
            }
            return format;
        }
    };

    /*********** END DisplayType********************/

    /**
     *  Factory for Control and its Label for single Row display 
     *
     */
    VIS.VControlFactory = {
        /**
         *  Create control for MField.
         *  The Name is set to the column name for dynamic display management
         *  @param mTab MTab
         *  @param mField MField
         *  @param tableEditor true if table editor
         *  @param disableValidation show all values
         *  @return icontrol
         */
        getControl: function (mTab, mField, tableEditor, disableValidation, other) {
            if (!mField)
                return null;
            var columnName = mField.getColumnName();
            var isMandatory = mField.getIsMandatory(false);      //  no context check
            //  Not a Field
            if (mField.getIsHeading())
                return null;
            var isDisplayed = mField.getIsDisplayed();
            var minValue = mField.getMinValue();
            var maxValue = mField.getMaxValue();
            var ctrl = null;
            var displayType = mField.getDisplayType();

            var isReadOnly = mField.getIsReadOnly();
            var isUpdateable = mField.getIsEditable(false);
            var windowNo = mField.getWindowNo();
            var tabNo = 0;
            if (mTab)
                tabNo = mTab.getTabNo();

            if (displayType == VIS.DisplayType.Button) {

                var btn = new VButton(columnName, isMandatory, isReadOnly, isUpdateable, mField.getHeader(), mField.getDescription(), mField.getHelp(), mField.getAD_Process_ID(), mField.getIsLink(), mField.getIsRightPaneLink(), mField.getAD_Form_ID(), mField.getIsBackgroundProcess(), mField.getAskUserBGProcess())
                btn.setField(mField);
                btn.setReferenceKey(mField.getAD_Reference_Value_ID());
                ctrl = btn;
            }

            else if (displayType == VIS.DisplayType.String) {

                //if (mField.getIsEncryptedField()) {
                //    //VPassword vs = new VPassword (columnName, mandatory, readOnly, updateable,
                //    //    mField.getDisplayLength(), mField.getFieldLength(), mField.getVFormat());
                //    //vs.setName (columnName);
                //    //vs.setField (mField);
                //    //editor = vs;
                //}
                //else {
                var txt = new VTextBox(columnName, isMandatory, isReadOnly, isUpdateable, mField.getDisplayLength(), mField.getFieldLength(),
                    mField.getVFormat(), mField.getObscureType(), mField.getIsEncryptedField());
                txt.setField(mField);
                ctrl = txt;
                //}
            }
            else if (displayType == VIS.DisplayType.YesNo) {
                //columnName, mandatory, isReadOnly, isUpdateable, text, description, tableEditor
                var chk = new VCheckBox(columnName, isMandatory, isReadOnly, isUpdateable, mField.getHeader(), mField.getDescription(), mField.getIsSwitch());
                chk.setField(mField);
                ctrl = chk;
            }
            else if (VIS.DisplayType.IsDate(displayType)) {

                if (displayType == VIS.DisplayType.DateTime)
                    readOnly = true;
                var vd = new VDate(columnName, isMandatory, isReadOnly, isUpdateable,
                    displayType, mField.getHeader());
                vd.setName(columnName);
                vd.setField(mField);
                ctrl = vd;
            }
            else if (VIS.DisplayType.IsLookup(displayType) || VIS.DisplayType.ID == displayType) {
                var lookup = mField.getLookup();
                if (disableValidation && lookup != null)
                    lookup.disableValidation();

                if (!disableValidation) {

                    if (displayType != VIS.DisplayType.Search && displayType != VIS.DisplayType.MultiKey && displayType != VIS.DisplayType.ProductContainer) {

                        var cmb = new VComboBox(columnName, isMandatory, isReadOnly, isUpdateable, lookup, mField.getDisplayLength(), displayType, mField.getZoomWindow_ID());
                        cmb.setField(mField);
                        ctrl = cmb;
                        //ctrl = new VComboBox();
                    }
                    else if (displayType == VIS.DisplayType.ProductContainer) {
                        var txtAmtDiv = new VProductContainer(columnName, isMandatory, isReadOnly, isUpdateable, displayType, mField.getLookup(), windowNo);
                        txtAmtDiv.setField(mField);
                        ctrl = txtAmtDiv;
                    }
                    else {

                        var txtb = new VTextBoxButton(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup, mField.getZoomWindow_ID());
                        txtb.setField(mField);
                        ctrl = txtb;
                    }
                }

                else {
                    if (lookup == null || (displayType != VIS.DisplayType.Search && lookup.getDisplayType() != VIS.DisplayType.Search)) {
                        var cmb = new VComboBox(columnName, isMandatory, isReadOnly, isUpdateable, lookup, mField.getDisplayLength(), displayType, mField.getZoomWindow_ID());
                        cmb.setDisplayType(displayType);
                        cmb.setField(mField);
                        ctrl = cmb;
                        // ctrl = new VComboBox();
                    }
                    else {
                        displayType = VIS.DisplayType.Search;
                        var txtb = new VTextBoxButton(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup, mField.getZoomWindow_ID());
                        txtb.setField(mField);
                        ctrl = txtb;
                    }
                }
            }
            else if (displayType == VIS.DisplayType.Location) {

                var txtLoc = new VLocation(columnName, isMandatory, isReadOnly, isUpdateable, displayType, mField.getLookup());
                txtLoc.setField(mField);
                ctrl = txtLoc;
            }
            else if (displayType == VIS.DisplayType.Locator) {
                var txtLocator = new VLocator(columnName, isMandatory, isReadOnly, isUpdateable, displayType, mField.getLookup());
                txtLocator.setField(mField);
                ctrl = txtLocator;
            }
            else if (VIS.DisplayType.Text == displayType || VIS.DisplayType.TextLong == displayType || VIS.DisplayType.Memo == displayType) {
                var tl = new VTextArea(columnName, isMandatory, isReadOnly, isUpdateable, mField.getDisplayLength(), mField.getFieldLength(), displayType);
                tl.setField(mField);
                ctrl = tl;
            }
            else if (VIS.DisplayType.IsNumeric(displayType)) {

                if (VIS.DisplayType.Amount == displayType || VIS.DisplayType.Quantity == displayType || VIS.DisplayType.CostPrice == displayType) {
                    var amt = new VAmountTextBox(columnName, isMandatory, isReadOnly, isUpdateable, mField.getDisplayLength(), mField.getFieldLength(), displayType, "FixedHeader");// mField.getHeader());
                    amt.setMinValue(minValue);
                    amt.setMaxValue(maxValue);
                    amt.setField(mField);
                    ctrl = amt;
                }
                else if (VIS.DisplayType.Integer == displayType) {
                    var int = new VNumTextBox(columnName, isMandatory, isReadOnly, isUpdateable, mField.getDisplayLength(), mField.getFieldLength(), "FixedHeader");// mField.getHeader());
                    int.setField(mField);
                    ctrl = int;
                }
                else if (VIS.DisplayType.Number == displayType) {
                    var num = new VAmountTextBox(columnName, isMandatory, isReadOnly, isUpdateable, mField.getDisplayLength(), mField.getFieldLength(), displayType, "FixedHeader");// mField.getHeader());
                    num.setField(mField);
                    num.setMinValue(minValue);
                    num.setMaxValue(maxValue);
                    ctrl = num;
                }
                else if (displayType == VIS.DisplayType.ProgressBar) {
                    var bar = new VProgressBar(columnName, isMandatory, isReadOnly, isUpdateable, mField.getDisplayLength(), mField.getFieldLength(), displayType);
                    bar.setField(mField);
                    bar.setMinValue(minValue);
                    bar.setMaxValue(maxValue);
                    ctrl = bar;
                }
            }
            else if (displayType == VIS.DisplayType.PAttribute) {

                var txtP = new VPAttribute(columnName, isMandatory, isReadOnly, isUpdateable, displayType, mField.getLookup(), windowNo, false, false, false, true, tabNo);
                txtP.setField(mField);
                ctrl = txtP;
            }
            else if (displayType == VIS.DisplayType.GAttribute) {
                var txtP = new VPAttribute(columnName, isMandatory, isReadOnly, isUpdateable, displayType, mField.getLookup(), windowNo, false, false, false, false, tabNo);
                txtP.setField(mField);
                ctrl = txtP;
            }
            else if (displayType == VIS.DisplayType.Account) {
                var txtA = new VAccount(columnName, isMandatory, isReadOnly, isUpdateable, displayType, mField.getLookup(), windowNo, mField.getHeader());
                txtA.setField(mField);
                ctrl = txtA;
            }
            else if (displayType == VIS.DisplayType.Binary) {
                var bin = new VBinary(columnName, isMandatory, isReadOnly, isUpdateable, windowNo);
                bin.setField(mField);
                ctrl = bin;
            }
            else if (displayType == VIS.DisplayType.Image) {
                var image = new VImage(columnName, isMandatory, isReadOnly, windowNo);
                image.setField(mField);
                ctrl = image;
            }
            else if (displayType == VIS.DisplayType.URL) {
                var vs = new VURL(columnName, isMandatory, isReadOnly, isUpdateable, mField.getDisplayLength(), mField.getFieldLength());
                vs.setField(mField);
                ctrl = vs;
            }
            else if (displayType == VIS.DisplayType.FileName || displayType == VIS.DisplayType.FilePath) {
                var vs = new VFile(columnName, isMandatory, isReadOnly, isUpdateable, windowNo, displayType);
                vs.setField(mField);
                ctrl = vs;
            }

            else if (displayType == VIS.DisplayType.Label) {
                ///******14/2/2012 *******///
                ///implement Action pane 
                ///

                var txt = new VLabel(mField.getHelp(), columnName, false, true);
                //VAdvantage.Controls.VButton button = new VAdvantage.Controls.VButton();
                //button.SetAttribute(mField, columnName, mandatory, isReadOnly, isUpdateable,
                //                    mField.GetHeader(), mField.GetDescription(), mField.GetHelp(), mField.GetAD_Process_ID());
                txt.setField(mField);
                ctrl = txt;
            }
            else if (displayType == VIS.DisplayType.AmtDimension) {
                var txtAmtDiv = new VAmtDimension(columnName, isMandatory, isReadOnly, isUpdateable, displayType, mField.getLookup(), windowNo);
                txtAmtDiv.setField(mField);
                txtAmtDiv.setDefaultValue(mField.vo.DefaultValue);
                ctrl = txtAmtDiv;
            }
            else if (displayType == VIS.DisplayType.ProductContainer) {
                var txtAmtDiv = new VProductContainer(columnName, isMandatory, isReadOnly, isUpdateable, displayType, mField.getLookup(), windowNo);
                txtAmtDiv.setField(mField);
                ctrl = txtAmtDiv;
            }

            if (ctrl == null) {
                var txt = new VTextBox(columnName, isMandatory, isReadOnly, isUpdateable, mField.getDisplayLength(), mField.getFieldLength(),
                    mField.getVFormat(), mField.getObscureType(), mField.getIsEncryptedField());
                txt.setField(mField);
                ctrl = txt;
                //ctrl = new VTextBox(columnName, mandatory, isReadOnly, isUpdateable, 40, 20, null, "");
            }


            return ctrl;
        },

        /**
         *  Create Label for MField. (null for YesNo/Button)
         *  The Name is set to the column name for dynamic display management
         *  ignoreCheckbox is used to create label for checkbox from header panel.
         *  @param mField MField
         *  @param ignoreCheckbox bool
         *  @return Label
         */
        getLabel: function (mField) {
            if (mField == null)
                return null;

            var displayType = mField.getDisplayType();

            //	No Label for FieldOnly, CheckBox, Button
            if (mField.getIsFieldOnly()
                || (displayType == VIS.DisplayType.YesNo)
                || displayType == VIS.DisplayType.Button
                || displayType == VIS.DisplayType.Label)
                return null;
            return new VIS.Controls.VLabel(mField.getHeader(), mField.getColumnName(), mField.getIsMandatory());
        },


        getHeaderLabel: function (mField) {
            if (mField == null)
                return null;

            var displayType = mField.getDisplayType();

            //	No Label for FieldOnly, CheckBox, Button
            if ((mField.getHeaderIconOnly() && mField.getHeaderHeadingOnly()) || (mField.getHeaderIconOnly())
                || displayType == VIS.DisplayType.Button
                || displayType == VIS.DisplayType.Label)
                return null;
            return new VIS.Controls.VLabel(mField.getHeader(), mField.getColumnName(), mField.getIsMandatory());
        },

        ///Return label for almost all the references. Used in header panel.
        getReadOnlyControl: function (Tab, mField, tableEditor, disableValidation, other) {
            if (!mField)
                return null;
            var columnName = mField.getColumnName();
            var isMandatory = mField.getIsMandatory(false);
            var windowNo = mField.getWindowNo();//  no context check
            var displayType = mField.getOrginalDisplayType();
            var isReadOnly = mField.getIsReadOnly();
            var isUpdateable = mField.getIsEditable(false);

            var ctrl = null;



            if (displayType == VIS.DisplayType.Image) {
                var image = new VImage(columnName, isMandatory, true, windowNo);
                //image.setField(mField);
                image.setDimension(120, 140);
                image.hideText();
                image.hideEditIcon();
                ctrl = image;
            }
            else if (displayType == VIS.DisplayType.Button) {
                var btn = new VButton(columnName, isMandatory, isReadOnly, isUpdateable, mField.getHeader(), mField.getDescription(), mField.getHelp(), mField.getAD_Process_ID(), mField.getIsLink(), mField.getIsRightPaneLink(), mField.getAD_Form_ID(), mField.getIsBackgroundProcess(), mField.getAskUserBGProcess())
                btn.setField(mField, true);
                ctrl = btn;
            }
            else {
                var $ctrl = new VSpan(mField.getHelp(), columnName, false, true);
                if (VIS.DisplayType.IsNumeric(displayType)) {
                    $ctrl.format = VIS.DisplayType.GetNumberFormat(displayType);
                }

                ctrl = $ctrl;
            }


            return ctrl;

        },

        ///Checks if class is applied, then return i tag with className
        //Otherwise return image tag along with source.
        getIcon: function (mField) {
            if (mField.getFontClass()) {
                return "<i class='" + mField.getFontClass() + "'> </i>";
            }

            if (mField.getAD_Image_ID() > 0) {
                return "<img src='" + VIS.Application.contextUrl + "Images/Thumb16x16/" + mField.getAD_Image_ID() + ".jpg' > ";
            }
        }

    };




    /***************************************************************************
     *	base class  for single Row controls 
     *  <p>
     *  Controls fire VetoableChange to inform about new entered values
     *  and listen to propertyChange (MField.PROPERTY) to receive new values
     *  and performed action to inform action listner (etc click event);
     *  or  changes of Background or Editability
     *
     ***************************************************************************/

    function IControl(ctrl, displayType, isReadOnly, colName, isMandatory) {
        if (this instanceof IControl) {
            this.ctrl = ctrl;
            this.displayType = displayType;
            this.isReadOnly = isReadOnly;
            this.colName = colName;
            this.isMandatory = isMandatory;
            this.oldValue = "oldValue";
            this.visible;
            this.tag;
            this.setVisible(true);
            this.editingGrid = null;
        }
        else {
            return new VIS.controls.IControl(ctrl, displayType, isReadOnly, colName, isMandatory);
        }
    };

    /**
     *	Get control
     * 	@return control
     */
    IControl.prototype.getControl = function () {
        return this.ctrl
    };

    /**
    *	Get root
    * 	@return control
    */
    IControl.prototype.getRoot = function () {
        if (!this.root) {
            return this.ctrl;
        }
        return this.root;
    };

    /**
     *	Get model field
     * 	@return field
     */
    IControl.prototype.getField = function () { return this.mField };
    /**
     *	Get Column Name
     * 	@return column name
     */
    IControl.prototype.getName = function () { return this.colName };
    /**
     *	Get Column Name
     * 	@return column name
     */
    IControl.prototype.getColumnName = function () { return this.colName };
    /**
     *	Get is Mandatory
     * 	@return true control can not be empty
     */
    IControl.prototype.getIsMandatory = function () { return this.isMandatory; };
    /**
     *	Get is readonly
     * 	@return read only
     */
    IControl.prototype.getIsReadonly = function () { return this.readOnly };
    /**
     *	Get type of control
     * 	@return display type
     */
    IControl.prototype.getDisplayType = function () { return this.displayType };
    /**
     *	Get visibility
     * 	@return visible or not
     */
    IControl.prototype.getIsVisible = function () {
        return this.visible;
    };
    /**
     *	Get value of control
     * 	@return value
     */
    IControl.prototype.getValue = function () {
        var v = this.ctrl.val();
        //if (v == "") return null;
        return v;
    };	//

    IControl.prototype.getDisplay = function () {
        return this.ctrl.val();
    };

    /**
     * get additinal action button count with this control
     -  zoom 0r Info
    */
    IControl.prototype.getBtnCount = function () {
        return 0;
    };	//

    /**
     *	set name of control
     *
     * @param name
     */
    IControl.prototype.setName = function (name, prefix) { if (!prefix) { this.colName = name; } else { this.name = prefix + name; } };
    /**
     *	set model field
     *
     * @param model field
     */
    IControl.prototype.setField = function (mField) {
        this.mField = mField;
    };
    /**
     *	set value 
     *
     * @param value to set 
     */
    IControl.prototype.setValue = function (val) {
    };
    /**
     *	show hide control
     *
     * @param visible
     */
    IControl.prototype.setVisible = function (visible) {
        this.visible = visible;
        if (visible) {
            this.ctrl.show();
        } else {
            this.ctrl.hide();
        }
    };
    /**
     *	set readonly
     *
     * @param readOnly
     */
    IControl.prototype.setReadOnly = function (readOnly) {
        this.isReadOnly = readOnly;
        this.ctrl.prop('disabled', readOnly ? true : false);
        this.setBackground(false);
    };

    /**
     * set/reset style on control
     * @param {any} style inline style / class
     */
    IControl.prototype.setHtmlStyle = function (style) {
        if (style && this.dynStyle != style) {

            if (style.contains(':')) {
                if (!this.dynStyle) this.oldStyle = this.ctrl.attr('style');
                this.ctrl.removeAttr(style).attr('style', style);
            }
            else {
                this.ctrl.addClass(style);
            }
            this.dynStyle = style;
        }
        else if (!style && this.dynStyle) {
            if (this.dynStyle.contains(':')) {
                this.ctrl.removeAttr('style');
                if (this.oldStyle) this.ctrl.attr('style', this.oldStyle);
            }
            else {
                this.ctrl.removeClass(this.dynStyle);
            }
            this.oldStyle = null;
            this.dynStyle = null;
        }
    }

    /*
    Set Default Focus
    */
    IControl.prototype.setDefaultFocus = function (focus) {
        if (focus) {
            this.ctrl.focus();
        }
    };

    /**
     *	set mandatory
     *
     * @param ismandotry
     */
    IControl.prototype.setMandatory = function (isMandatory) {
        this.isMandatory = isMandatory;
        this.setBackground(false);
    };

    /**
     *	set backgoud color of control
     *
     * @param iserror
     */
    IControl.prototype.setBackground = function (e) {

        if (this.colName.startsWith("lbl") || this.displayType == VIS.DisplayType.Label ||
            this.displayType == VIS.DisplayType.YesNo || this.displayType == VIS.DisplayType.Button || this.displayType == VIS.DisplayType.ProgressBar)
            return;


        //console.log(typeof (e));
        if (typeof (e) == "boolean") {
            // console.log("1");
            if (e)
                this.setBackground("vis-ev-col-error");
            else if (this.isReadOnly)
                this.setBackground("vis-ev-col-readonly");
            else if (this.isMandatory) {
                var val = this.getValue();
                if (val && val.toString().length > 0) {
                    this.setBackground("");
                    return;
                }
                this.setBackground("vis-ev-col-mandatory");
            }
            else
                this.setBackground("");
        }
        else {
            // console.log("2");
            //var c = this.ctrl.css('background-color');
            //if (c == color)
            //    return;
            if (this.activeClass == e)
                return;

            this.ctrl.removeClass();
            if (e.length > 0)
                this.ctrl.addClass(e);
            this.activeClass = e;
            //this.ctrl.css('background-color', color);
            //console.log(this.ctrl.css('background-color'));
        }
    };

    IControl.prototype.setDisplayType = function (displayType) {
        this.displayType = displayType;
    };

    /**
     *	value Change Listener 
     *  @param listener
     */
    IControl.prototype.addVetoableChangeListener = function (listner) {
        this.vetoablechangeListner = listner;
    };

    /**
     *	Notify value changed
     *  @param event
     */
    IControl.prototype.fireValueChanged = function (evt, force) {

        if (this.isReadOnly) {
            //if (this.valSetting) {
            //    this.valSetting = false;
            //    return;
            //}
            var oldVal = this.oldValue;
            this.oldValue = "";
            // this.valSetting = true;
            this.setValue(oldVal);
            return;
        }

        if (this.editingGrid && (this.gridPos.dialog || force)) {
            window.setTimeout(function (self) {

                if (self.ctrl.parent().length != 0)
                    self.ctrl.detach();

                self.editingGrid.editChange.call(self.editingGrid, self.ctrl[0], self.gridPos.index, self.gridPos.col, evt, evt.newValue);
                self = null;
                evt = null;
            }, 10, this);
        }

        else if (this.vetoablechangeListner && !this.editingGrid) {
            window.setTimeout(function (self) {
                self.vetoablechangeListner.vetoablechange(evt);
                self = null;
                evt = null;
            }, 10, this);
        }
    };

    /**
     *	Refresh UI
     *  @param event
     */
    IControl.prototype.fireRefreshUI = function (evt) {
        if (this.vetoablechangeListner) {
            window.setTimeout(function (self) {
                self.vetoablechangeListner.refreshUI(evt);
                self = null;
                evt = null;
            }, 10, this);
        }

    };

    /**
    *	action listner
    *   @param event
    */
    IControl.prototype.addActionListner = function (listner) {
        this.actionListner = listner;
    };

    /**
     *	Notify action (eg click )
     *  @param event
     */
    IControl.prototype.invokeActionPerformed = function (evt) {
        if (this.actionListner) {
            this.actionListner.actionPerformed(evt);
        }
    };

    /**
     *	clean up
     */
    IControl.prototype.dispose = function () {
        console.log("dispose ");
        this.ctrl.remove();
        this.ctrl = null;
        this.mField = null;
        this.displayType = null;
        this.isReadOnly = null;
        this.colName = null;
        this.isMandatory = null;
        this.oldValue = null;
        this.vetoablechangeListner = null;
        this.actionListner = null;
        this.disposeComponent();
    };
    //END IControls 


    /**************************************************************************
     *	Data Binding:
     *		Icontrols call fireVetoableChange(m_columnName, null, getText());
     *		GridController (for Single-Row) 
     *      listen to Vetoable Change Listener (vetoableChange)
     *		then set the value for that column in the current row in the table
     *
     **************************************************************************/



    /**************************************************************************
     *	visual control for system displaytype text or string
     *	Detail Constructor
     *  @param columnName column name
     *  @param mandatory mandatory
     *  @param isReadOnly read only
     *  @param isUpdateable updateable
     *  @param displayLength display length
     *  @param fieldLength field length
     *  @param VFormat format
     *  @param ObscureType obscure type
     ***************************************************************************/

    function VTextBox(columnName, isMandatory, isReadOnly, isUpdateable, displayLength, fieldLength, vFormat, obscureType, isPwdField) {

        var displayType = VIS.DisplayType.String;
        this.obscureType = obscureType;
        var src = "fa fa-credit-card";
        //Init Control
        var $ctrl = $('<input>', { type: (isPwdField) ? 'password' : 'text', name: columnName, maxlength: fieldLength });
        var $btnSearch = $('<button class="input-group-text" style="display:none"><i class="' + src + '" /></button>');
        //if (obscureType && !isReadOnly)
        //    $ctrl.append($btnSearch);

        //Call base class
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);

        if (isReadOnly || !isUpdateable || obscureType) {
            this.setReadOnly(true);
        }
        else {
            this.setReadOnly(false);
        }

        this.getBtn = function (index) {
            if (index == 0 && obscureType) {
                this.setReadOnly(true);
                return $btnSearch;
            }
        };


        this.showObscureButton = function (show) {
            if (show && obscureType) {
                $btnSearch.css("display", "");
            }
            else {
                $btnSearch.css("display", "none");
                $btnSearch.off("click");
            }
        };

        this.getBtnCount = function () {

            if (obscureType)
                return 1;
            return 0;
        };

        var self = this; //self pointer

        /* Event */
        $ctrl.on("change", function (e) {
            e.stopPropagation();
            var newVal = $ctrl.val();
            this.value = newVal;
            if (newVal !== self.oldValue) {
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
            }
            if (obscureType) {
                self.setReadOnly(true);
            }
        });

        /* Event */
        $ctrl.on("blur", function (e) {
            // e.stopPropagation();
            var newValue = $ctrl.val();

            if (self.obscureType && newValue != "" && self.oldValue == newValue) {
                self.ctrl.val(VIS.Env.getObscureValue(self.obscureType, newValue));
                self.setReadOnly(true);
            }
        });

        $btnSearch.on("click", function () {
            if (self.mField.getIsEditable(true)) {
                self.setReadOnly(false, true, true);
                $ctrl.val(self.mField.getValue());
                $ctrl.focus();
            }
        });

        this.disposeComponent = function () {
            $ctrl.off("change"); //u bind event
            $ctrl = null;
            self = null;
        }




    };

    VIS.Utility.inheritPrototype(VTextBox, IControl);//Inherit from IControl


    VTextBox.prototype.setReadOnly = function (readOnly, forceWritable) {
        if (!readOnly && this.obscureType && !forceWritable && (!this.mField.getIsInserting() || this.ctrl.val() != "")) {
            readOnly = true;
        }

        this.isReadOnly = readOnly;
        this.ctrl.prop('disabled', readOnly ? true : false);
        this.setBackground(false);
    };


    /** 
     *  set value 
     *  @param new value to set
     */
    VTextBox.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {

            //console.log(newValue);

            if (this.obscureType && (!this.mField.getIsInserting() || this.ctrl.val() != "")) {
                this.ctrl.val(VIS.Env.getObscureValue(this.obscureType, newValue));
                this.setReadOnly(true);
            }
            else
                this.ctrl.val(newValue);

            this.oldValue = newValue;
            //this.setBackground("white");
        }
    };

    /** 
     *  get display text of control
     *  @return text of control
     */
    VTextBox.prototype.getDisplay = function () {
        return this.ctrl.val();
    };
    //END VTextbox


    //2.  VLabel

    /**
     *  Label with Mnemonics interpretation
     *  Label against model field control like (textbox, combobox etc)
     *  @param value  The text to be displayed by the label.
     *  @param name  name of control to bind label with
     */
    function VLabel(value, name, isMandatory, isADControl) {
        value = value != null ? value.replace("[&]", "") : "";
        var strFor = ' for="' + name + '"';
        if (isADControl)
            strFor = '';

        var $ctrl = $('<label ' + strFor + '></label>');

        IControl.call(this, $ctrl, VIS.DisplayType.Label, true, isADControl ? name : "lbl" + name);
        if (isMandatory) {
            $ctrl.text(value).append("<sup>*</sup>");
        }
        else {
            $ctrl.text(value);
        }

        this.disposeComponent = function () {
            $ctrl = null;
            self = null;
        }
    };


    VIS.Utility.inheritPrototype(VLabel, IControl); //Inherit


    // END VLabel 

    //2.  VSPAN

    /**
     *  VSpan with Mnemonics interpretation
     *  VSpan against model field control like (textbox, combobox etc)
     *  @param value  The text to be displayed by the VSpan.
     *  @param name  name of control to bind VSpan with
     */
    function VSpan(value, name, isMandatory, isADControl) {
        value = value != null ? value.replace("[&]", "") : "";

        var strFor = ' for="' + name + '"';
        if (isADControl)
            strFor = '';

        var $ctrl = $('<span ' + strFor + '></span>');

        IControl.call(this, $ctrl, VIS.DisplayType.Label, true, isADControl ? name : "lbl" + name);
        if (isMandatory) {
            $ctrl.text(value).append("<sup>*</sup>");
        }
        else {
            $ctrl.text(value);
        }

        this.disposeComponent = function () {
            $ctrl = null;
            self = null;
            if (this.format)
                this.format.dispose();
            this.format = null;
        }
    };


    VIS.Utility.inheritPrototype(VSpan, IControl); //Inherit

    VSpan.prototype.setValue = function (newValue, isHTML) {

        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            this.ctrl.text(newValue);
            if (isHTML) {
                this.ctrl.html(newValue);
            }
        }
    };

    VSpan.prototype.getValue = function () {
        if (this.value != null) {
            return this.ctrl.text().toString();
        }
        else {
            return null;
        }
    };


    // END VSpan 






    //3. VButton 
    /***************************************************************************
     *  General Button.
     *  <pre>
     *  Special Buttons:
     *      Payment,
     *      Processing,
     *      CreateFrom,
     *      Record_ID       - Zoom
     *  </pre>
     *  Maintains all values for display in m_values.
     *  implement action listner to notify click event
     *
     *  @param columnName column
     *  @param mandatory mandatory
     *  @param isReadOnly read only
     *  @param isUpdateable updateable
     *  @param text text
     *  @param description description
     *  @param help help
     *  @param AD_Process_ID process to start
    
     ***************************************************************************/
    function VButton(columnName, mandatory, isReadOnly, isUpdateable, text, description, help, AD_Process_ID, isLink, isRightLink
        , AD_Form_ID, isBGProcess, isAskUserBGProcess) {

        this.actionListner;
        this.AD_Process_ID = AD_Process_ID;
        this.AD_Form_ID = AD_Form_ID;
        this.description = description;
        this.help = help;
        this.text = text;
        this.isLink = isLink;
        this.isRightLink = isRightLink;
        this.actionLink = null;
        this.isCsv = false;
        this.isPdf = false;
        this.askUserBGProcess = isAskUserBGProcess;
        this.isBackgroundProcess = isBGProcess;

        this.values = null;
        this.isIconSet = true;

        var $img = $("<i style='color:inherit' title='" + text + "'>");

        var $txt = $("<span style='color:inherit'>").text(text);
        var rootPath = VIS.Application.contextUrl + "Areas/VIS/Images/base/";

        var $ctrl = null;
        //Init Control
        if (!isLink) {
            $ctrl = $('<button class="vis-ev-col-wrap-button">', { type: 'button', name: columnName });
            $img.css("margin-right", "8px");
            $ctrl.append($img).append($txt);
        }
        else {
            if (isRightLink) {
                $ctrl = $('<li>');
                $ctrl.append($txt).append($img);
            }
            else {
                $ctrl = $('<button type="button" class="vis-ev-col-wrap-button vis-ev-col-linkbutton"></button>');
                $ctrl.append($img).append($txt);
            }
        }

        this.setIcon = function (img, isSrc) {
            if (isSrc)
                $img.src = img;
            else
                $img.addClass(img);
        };
      
        //	Special Buttons

        if (columnName.equals("PaymentRule")) {
            this.readReference(195);
            this.setIcon("vis vis-payment");    //  29*14
        }
        else if (columnName.equals("DocAction")) {
            this.readReference(135);
            this.setIcon("vis vis-cog");    //  16*16
        }
        else if (columnName.equals("CreateFrom")) {
            this.setIcon("vis vis-copy");       //  16*16
        }
        else if (columnName.equals("Record_ID")) {
            this.setIcon("vis vis-find");       //  16*16
            $ctrl.text(VIS.Msg.getMsg("ZoomDocument"));
        }
        else if (columnName.equals("Posted")) {
            this.readReference(234);
            //$ctrl.css("color", "magenta"); //
            this.setIcon("fa fa-line-chart");    //  16*16
        }
        else if (isLink) {
            this.isIconSet = false;
            this.setIcon("vis vis-action");
        }


        IControl.call(this, $ctrl, VIS.DisplayType.Button, isReadOnly, columnName, mandatory);

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
            //this.Enabled = false;
        }
        else {
            this.setReadOnly(false);
        }

        var self = this; //self pointer
        var $ulPopup = null;

        $ulPopup = getPopupList();
        function getPopupList() {
            var ullst = $("<ul class='vis-apanel-rb-ul'>");
            //ullst.append($("<li data-action='D'>").text(VIS.Msg.getMsg("Default")));
            ullst.append($("<li data-action='C'>").text(VIS.Msg.getMsg("OpenCSV")));
            ullst.append($("<li data-action='P'>").text(VIS.Msg.getMsg("OpenPDF")));
            return ullst;
        };

        $ctrl.on(VIS.Events.onClick, function (evt) { //click handler
            evt.stopPropagation();

            var isReport = null;
            // self.invokeActionPerformed({ source: self });
            if (!self.isReadOnly) {
                var sqlQry = "VIS_81";
                var param = [];
                param[0] = new VIS.DB.SqlParam("@AD_Process_ID", AD_Process_ID);
                isReport = executeScalar(sqlQry, param);


                sqlQry = "VIS_149";
                param = [];
                param[0] = new VIS.DB.SqlParam("@AD_Process_ID", AD_Process_ID);
                var isCrystalReport = executeScalar(sqlQry, param);

                if (isCrystalReport == "Y" && VIS.context.getIsUseCrystalReportViewer()) {
                    self.invokeActionPerformed({ source: self });
                }
                else {
                    if (isReport == 'Y') {
                        $img.w2overlay($ulPopup.clone(true));
                    }
                    else {
                        self.invokeActionPerformed({ source: self });
                    }
                }
            }
        });

        if ($ulPopup) {
            $ulPopup.on("click", "LI", function (e) {
                var target = $(e.target);

                if (target.data("action") == "P") {
                    self.isPdf = true;
                    self.isCsv = false;
                }
                else if (target.data("action") == "C") {
                    self.isCsv = true;
                    self.isPdf = false;
                }
                self.invokeActionPerformed({ source: self });
            });
        }

        this.setText = function (text) {
            if (text == null) {
                $txt.text("");
                return;
            }
            var pos = text.indexOf('&');
            if (pos != -1)					//	We have a nemonic - creates ALT-_
            {
                var mnemonic = text.toUpperCase().charAt(pos + 1);
                if (mnemonic != ' ') {
                    //setMnemonic(mnemonic);
                    text = text.substring(0, pos) + text.substring(pos + 1);
                }
            }
            $txt.text(text);

        };

        this.setLayout = function (isHeaderPnl) {
            if (!this.mField)
                return;
            if (!isHeaderPnl) {
                if (this.mField.getIsFieldOnly() && this.mField.getShowIcon())
                    $txt.remove();
                else if (this.mField.getIsFieldOnly())
                    $img.remove();
            }
            else {
                if (this.mField.getHeaderHeadingOnly())
                    $img.remove();
                else if (this.mField.getHeaderIconOnly())
                    $txt.remove();
            }
        };

        this.disposeComponent = function () {
            $ctrl.off(VIS.Events.onClick);
            $ctrl = null;
            self = null;
            //this.actionListner = null;
            this.AD_Process_ID = null;
            this.description = null;
            this.help = null;
            this.setText = null;
        };
    };
    VIS.Utility.inheritPrototype(VButton, IControl);//Inherit

    VButton.prototype.referenceList = {};

    VButton.prototype.setField = function (mField, isHeaderPnl) {
        this.mField = mField;
        // if (!this.isIconSet) {
        if (mField.getShowIcon() && (mField.getFontClass() != '' || mField.getImageName() != '')) {
            if (mField.getFontClass() != '')
                this.setIcon(mField.getFontClass());
            else
                this.setIcon(VIS.Application.contextUrl + 'Images/Thumb16x16/' + mField.getImageName(), true);
        }
        // }
        this.setLayout(isHeaderPnl);
    };



    VButton.prototype.setReferenceKey = function (refid) {
        if (refid && refid > 0 && refid != 195 && refid != 135 && refid != 234) {
            this.readReference(refid);
        }
    }

    VButton.prototype.setValue = function (value) {
        this.value = value;
        var text = this.text;

        //	Nothing to show or Record_ID
        if (value == null || this.colName.equals("Record_ID"))
            ;
        else if (this.values != null)
            text = this.values[value];
        else if (this.lookup != null) {
            var pp = this.lookup.get(value);
            if (pp != null)
                text = pp.getName();
        }
        //	Display it
        if (!text) {
            text = VIS.Msg.getMsg("Invalid") + " { '" + value + "' }";
        }
        this.setText(text != null ? text : "");
    };

    VButton.prototype.setReadOnly = function (readOnly) {
        this.isReadOnly = readOnly;
        this.ctrl.css('opacity', readOnly ? .6 : 1);
        if (this.isLink) {
        }
        else {
            this.ctrl.prop('disabled', readOnly ? true : false);
        }
        this.setBackground(false);
    };

    /**
     *	Return Value
     *  @return value
     */

    VButton.prototype.getValue = function () {
        if (this.value != null) {
            return this.value.toString();
        }
        else {
            return null;
        }
    };	//	getValue

    /**
     *  Return Display Value
     *  @return String value
     */
    VButton.prototype.getDisplay = function () {
        return this.value;
    };  //  getDispl

    /**
     *	Fill m_Values with Ref_List values
     *  @param AD_Reference_ID reference
     */
    VButton.prototype.readReference = function (AD_Reference_ID) {

        this.values = null;
        if (this.referenceList) {
            this.values = this.referenceList[AD_Reference_ID];
        }

        if (this.values) {
            return this.values;
        }
        else {
            this.values = {};
        }
        
        var SQL;
        if (VIS.Env.isBaseLanguage(VIS.Env.getCtx(), "")) {
            SQL = "VIS_82";
        } else {
            SQL = "VIS_148";
        }
        var param = [];
        param[0] = new VIS.DB.SqlParam("@AD_Reference_ID", AD_Reference_ID);
        try {
            var dr = executeReader(SQL, param);
            while (dr.read()) {

                this.values[dr.getString(0)] = dr.getString(1);
            }
            this.referenceList[AD_Reference_ID] = this.values;
            dr.close();
        }
        catch (e) {
            VIS.Logging.VLogger.getVLogger().get().log(VIS.Logging.Level.SEVERE, SQL, e);
        }
    };	//	readReferenc
    /**
     *  Return process id
     *  @return ad_process_id
     */
    VButton.prototype.getProcess_ID = function () {
        return this.AD_Process_ID;
    };

    VButton.prototype.getAD_Form_ID = function () {
        return this.AD_Form_ID;
    };

    /**
     *  Return description
     *  @return String value
     */
    VButton.prototype.getDescription = function () {
        return this.description;
    };
    /**
     *  Return help[ text
     *  @return String value
     */
    VButton.prototype.getHelp = function () {
        return this.help;
    };

    /**
    * return if user want to open confirmation popup or not
    * @return Bool value
    */
    VButton.prototype.getAskUserBGProcess = function () {
        return this.askUserBGProcess;
    };

    /**
    * return if process is background or not
    * @return Bool value
    */
    VButton.prototype.getIsBackgroundProcess = function () {
        return this.isBackgroundProcess;
    };

    //End Button 





    //4. VCheckBox 
    /*********************************************************************
    *  Checkbox Control
    *
    *  @param columnName
    *  @param mandatory
    *  @param isReadOnly
    *  @param isUpdateable
    *  @param title
    *  @param description
    **********************************************************************/

    function VCheckBox(columnName, mandatory, isReadOnly, isUpdateable, text, description, isSwitch) {
        var $ctrl = $('<input>', { type: 'checkbox', name: columnName, value: text });
        var $lbl = $('<label class="vis-ec-col-lblchkbox" />').html(text);
        if (isSwitch) {
            $ctrl.addClass('vis-ctrl-switch');
            $lbl.prepend('<i for="switch" class="vis-ctrl-switchSlider">Toggle</i>');
        }
        $lbl.prepend($ctrl);
        //var $lbl = $('<label class="vis-ec-col-lblchkbox" />').html(text).prepend('<i for="switch" class="vis-switchSlider">Toggle</i>').prepend($ctrl);
        IControl.call(this, $lbl, VIS.DisplayType.YesNo, isReadOnly, columnName, mandatory);
        this.cBox = $ctrl;
        var self = this;

        this.setReadOnly = function (isReadOnly) {
            this.isReadOnly = isReadOnly;
            $ctrl.prop('disabled', isReadOnly);
            $lbl.css('opacity', isReadOnly ? .7 : 1);
        }

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
            //this.Enabled = false;
        }
        else {
            this.setReadOnly(false);
        }

        $ctrl.on("change", function (e) {
            e.stopPropagation();
            var newVal = $ctrl.prop('checked');
            if (newVal !== self.oldValue) {
                self.oldValue = newVal;
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
            }
        });
        this.disposeComponent = function () {
            $ctrl.off("change");
            $ctrl = null;
            self = null;
            this.cBox = null;
        }
    };

    VIS.Utility.inheritPrototype(VCheckBox, IControl);//Inherit
    /** 
     *  Set Value 
     *  @param new Value
     */
    VCheckBox.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            //this.ctrl.val(newValue);
            this.cBox.prop('checked', newValue);
        }
    };

    /** @override  
     *  get Value 
     *  @return value
     */
    VCheckBox.prototype.getValue = function () {
        return this.cBox.prop('checked');
    };

    /** 
     *  get display
     *  @return value
     */
    VCheckBox.prototype.getDisplay = function () {
        return this.cBox.prop('checked').toString();
    };

    //END 


    //5.VCombobox

    /******************************************************************
    *  select control for Lookup Visual Field.
    *  Special handling of BPartner and Product
    *
    *  @param columnName column
    *  @param mandatory mandatory
    *  @param isReadOnly read only
    *  @param isUpdateable updateable
    *  @param lookup lookup
    *******************************************************************/
    function VComboBox(columnName, mandatory, isReadOnly, isUpdateable, lookup, displayLength, displayType, zoomWindow_ID) {
        if (!displayType)
            displayType = VIS.DisplayType.Table;

        // var $ctrl = $('<input>', { name: columnName });
        var $ctrl = $('<select>', { name: columnName });
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, mandatory);


        this.lookup = lookup;
        this.lastDisplay = "";
        this.settingFocus = false;
        this.inserting = false;
        this.settingValue = false;
        this.loading = false;

        if (lookup != null && lookup.getIsValidated()) {
            // if (lookup.getIsLoading())
            this.loading = true;
            //else {
            //  lookup.fillCombobox(mandatory, false, false, false);
            //  this.refreshOptions(lookup.data);
            // }
        }
        $ctrl[0].selectedIndex = -1;

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
            //this.Enabled = false;
        }
        else {
            this.setReadOnly(false);
        }

        //Set Buttons and [pop up]
        var isZoom = false;
        var $btnPop = null;
        var btnCount = 0;
        var $ulPopup = null;
        var options = {};
        var disabled = false;
        if (lookup != null) {

            if ((lookup.getDisplayType() == VIS.DisplayType.List && VIS.context.getAD_Role_ID() == 0)
                || lookup.getDisplayType() != VIS.DisplayType.List)     //  only system admins can change lists, so no need to zoom for others
            {
                isZoom = true;

                if (lookup instanceof VIS.MLookup) {
                    if (lookup.getZoomWindow() == 0 && (zoomWindow_ID < 1)) {
                        disabled = true;
                    }
                }

                //$btnZoom = VIS.AEnv.getZoomButton(disabled);
                options[VIS.Actions.zoom] = disabled;
               
                // btnCount += 1;
            }
            options[VIS.Actions.addnewrec] = true;

            if ((this.lookup &&
                (this.lookup.info.keyColumn.toLowerCase() == "ad_user.ad_user_id"
                    || this.lookup.info.keyColumn.toLowerCase() == "ad_user_id"))
                || columnName === "AD_User_ID" || columnName === "SalesRep_ID") {
                options[VIS.Actions.contact] = true;
            }

            // $btnPop = $('<button tabindex="-1" class="input-group-text"><img tabindex="-1" src="' + VIS.Application.contextUrl + "Areas/VIS/Images/base/Info20.png" + '" /></button>');
            $btnPop = $('<button tabindex="-1" class="input-group-text"><i tabindex="-1" class="fa fa-ellipsis-v" /></button>');
            options[VIS.Actions.refresh] = true;
           
            if (VIS.MRole.getIsShowPreference())
                options[VIS.Actions.preference] = true;
            $ulPopup = VIS.AEnv.getContextPopup(options);
            btnCount += 1;
            options = null;
        }


        /* provilized function */

        /* @overridde
        */
        this.getBtnCount = function () {
            return btnCount;
        };

        //this.createAutoComplete = function () {
        //    $ctrl.autocomplete({
        //    });
        //};

        //this.createAutoComplete();

        /** 
            get contols button by index 
            -  zoom or info button 
            - index 0 for info button 
            - index 1 for zoom button
            - control must tell or implemnt getBtnCount default is zero [no button]
        *
        */
        this.getBtn = function (index) {

            return $btnPop;
        };

        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                if ($btnPop)
                    $btnPop.show();
                $ctrl.show();

            } else {
                $ctrl.hide();
                if ($btnPop)
                    $btnPop.hide();
            }
        };

        var self = this; //self pointer


        //$ctrl.on("mousedown", function (e) {
        //    e.preventDefault();
        //});

        //$ctrl.on("touchend", function (e) {
        //    e.preventDefault();
        //});

        $ctrl.on("focus", function (e) {


            var outp = [];
            if (self.lookup == null)
                return;

            var selVal = $ctrl.val();
            if (self.lookup.getIsValidated() && !self.lookup.getHasInactive()) {

                if (self.loading) {
                    self.lookup.fillCombobox(mandatory, false, false, false);
                    if (self.lookup.loading) return;
                    self.refreshOptions(self.lookup.data, selVal);

                    if (selVal != null && $ctrl[0].selectedIndex < 0) {
                        //fire change event
                        self.oldValue = "old";
                        // fire  on change to clear datasource value

                        $ctrl.trigger("change")
                    }
                    self.loading = false;
                }
                return;
            }
            if (self.getIsReadonly())
                return;

            self.settingFocus = true;

            //var obj = lookup.getSelectedItem();
            var selVal = $ctrl.val();

            self.lookup.fillCombobox(self.getIsMandatory(), true, true, true);     //  only validated & active & temporary
            //self.lookup.setSelectedItem(selVal);
            //obj = self.lookup.getSelectedItem();

            self.refreshOptions(self.lookup.data, selVal);

            if (selVal && $ctrl[0].selectedIndex < 0) {
                self.oldValue = "old";
                // fire  on change to clear datasource value

                $ctrl.trigger("change")
            }
            self.settingFocus = false;

        });

        $ctrl.on("change", function (e) {

            var newVal = $ctrl.val();
            if (newVal !== self.oldValue) {
                if (newVal == -1 || newVal == "") {
                    newVal = null;
                }
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
            }
            e.stopPropagation();
        });

        function zoomAction(value) {
            if (!self.lookup || disabled)
                return;
            //
            var zoomQuery = self.lookup.getZoomQuery();
           //var value = self.getValue();


            if (!value)
                value = self.getValue();

            if (value == "")
                value = null;

            if (value != null && !isNaN(value))
                value = parseInt(value);


            //	If not already exist or exact value
            if ((zoomQuery == null) || (value != null)) {
                zoomQuery = new VIS.Query();	//	ColumnName might be changed in MTab.validateQuery

                var keyColumnName = null;
                //	Check if it is a Table Reference
                if ((self.lookup != null) && (self.lookup instanceof VIS.MLookup)) {
                    var AD_Reference_ID = self.lookup.getAD_Reference_Value_ID();
                    if (AD_Reference_ID != 0) {
                        var query = "VIS_83";
                        var param = [];
                        param[0] = new VIS.DB.SqlParam("@AD_Reference_ID", AD_Reference_ID);
                        try {
                            var dr = executeReader(query, param);
                            if (dr.read()) {
                                keyColumnName = dr.getString(0);
                            }
                            dr.dispose();
                        }
                        catch (e) {
                            this.log.log(VIS.Logging.Level.SEVERE, query, e);
                        }
                    }	//	Table Reference
                }	//	MLookup

                if ((keyColumnName != null) && (keyColumnName.length != 0))
                    zoomQuery.addRestriction(keyColumnName, VIS.Query.prototype.EQUAL, value);
                else
                    zoomQuery.addRestriction(self.getColumnName(), VIS.Query.prototype.EQUAL, value);
                zoomQuery.setRecordCount(1);	//	guess
            }

            var AD_Window_ID = 0;
            // VIS0045 : Handle Zoom Issue on Combo when control used on Form
            if (self.mField != null && self.mField.getZoomWindow_ID() > 0) {
                AD_Window_ID = self.mField.getZoomWindow_ID();
            }
            else {
                AD_Window_ID = self.lookup.getZoomWindow(zoomQuery);
            }

            // No target record to be zoomed - then zoom querry set null to show all records.
            if (value == null || value == -1) {
                zoomQuery = null;
            }


            //
            //this.log.info(this.getColumnName() + " - AD_Window_ID=" + AD_Window_ID
            //    + " - Query=" + zoomQuery + " - Value=" + value);
            //
            //setCursor(Cursor.getPredefinedCursor(Cursor.WAIT_CURSOR));
            //
            VIS.viewManager.startWindow(AD_Window_ID, zoomQuery);

            //setCursor(Cursor.getDefaultCursor());
        };

        var option

        if ($btnPop) {
            $btnPop.on(VIS.Events.onClick, function (e) {
                $btnPop.w2overlay($ulPopup.clone(true));
                e.stopPropagation();
            });
        }

        if ($ulPopup) {
            $ulPopup.on("click", "LI", function (e) {

                var action = $(e.target).data("action");
                if (action == VIS.Actions.preference) {
                    var obj = new VIS.ValuePreference(self.mField, self.getValue(), self.getDisplay());
                    if (obj != null) {
                        obj.showDialog();
                    }
                    obj = null;
                }
                else if (action == VIS.Actions.refresh) {
                    if (lookup == null)
                        return;
                    //
                    //setCursor(Cursor.getPredefinedCursor(Cursor.WAIT_CURSOR));
                    //
                    self.settingFocus = true;

                    var selVal = $ctrl.val();

                    self.lookup.refresh();
                    if (self.lookup.getIsValidated())
                        self.lookup.fillCombobox(self.getIsMandatory(), false, false, false);
                    else
                        self.lookup.fillCombobox(self.getIsMandatory(), true, false, false);
                    //m_combo.setSelectedItem(obj);

                    //	m_combo.revalidate();
                    //
                    self.refreshOptions(self.lookup.data, selVal);
                    self.settingFocus = false;
                    //alert(selVal);
                    //log.info(m_columnName + " #" + m_lookup.getSize() + ", Selected=" + m_combo.getSelectedItem());
                }
                else if (action == VIS.Actions.contact) {
                    var val = self.getValue();
                    if (val && val.toString().length > 0) {
                        //var contactInfo = new VIS.ContactInfo(val, this.mField.getWindowNo());
                        var contactInfo = new VIS.ContactInfo(val, self.mField.getWindowNo());
                        contactInfo.show();

                    }
                }
                else if (action == VIS.Actions.zoom) {
                    if (!disabled)
                        zoomAction();
                }
                else if (action == VIS.Actions.addnewrec) {
                    if (!disabled)
                        zoomAction(-10);
                }
                
            });
        }

        this.disposeComponent = function () {
            $ctrl.off("focus");
            $ctrl.off("change");
            if ($btnPop) {
                $btnPop.off(VIS.Events.onClick);
                $btnPop.remove();
            }

            $btnPop = null;

            if ($ulPopup)
                $ulPopup.remove();
            $ulPopup = null;

            this.lookup = null;
            this.lastDisplay = null;
            this.settingFocus = null;
            this.inserting = null;
            this.settingValue = null;
            $ctrl = null;
            self = null;
        }
    };
    VIS.Utility.inheritPrototype(VComboBox, IControl);//inherit IControl

    /**
     *  Set control to value
     *  @param newValue new Value
     */
    VComboBox.prototype.setValue = function (newValue, inserting) {




        if (this.oldValue != newValue) {
            this.settingValue = true;
            this.oldValue = newValue;

            //	Set comboValue
            this.ctrl.val(newValue);

            if (newValue == null) {
                this.lastDisplay = "";
                this.settingValue = false;
                return;
            }
            if (this.lookup == null) {
                this.lastDisplay = newValue.toString();
                this.settingValue = false;
                return;
            }

            this.lastDisplay = this.lookup.getDisplay(newValue, true);
            if (this.lastDisplay.equals("<-1>")) {
                this.lastDisplay = "";
                this.oldValue = null;
            }

            this.inserting = inserting;	//	MField.setValue

            var notFound = this.lastDisplay.startsWith("<") && this.lastDisplay.endsWith(">");

            var selValue = this.ctrl.val();

            if ((selValue == null || (this.inserting && (this.lookup.getDisplayType() != VIS.DisplayType.Search)))) {
                //  lookup found nothing too

                if (notFound && this.lookup.loading) { //wait for fill lookup operation completation
                    window.setTimeout(function (that) {
                        that.setValue(newValue, inserting);
                        that = null;
                        return;
                    }, 500, this);
                    this.oldValue = "oldValue";
                    return;
                }

                if (notFound) {
                    //  we may have a new value
                    // this.lookup.refresh();
                    // this.lookup.fillCombobox(
                    // this.refreshOptions(this.lookup.data, newValue);

                    this.lastDisplay = this.lookup.getDisplay(newValue, false);
                    notFound = this.lastDisplay.startsWith("<") && this.lastDisplay.endsWith(">");
                }
                if (notFound)	//	<key>
                {
                    this.oldValue = "old";
                    this.ctrl.val(null);
                    // fire  on change to clear datasource value
                    if (this.inserting)
                        this.ctrl.trigger("change");
                }
                //  we have lookup
                else if (this.ctrl.val() == null) {
                    //  do-not add item in combobox if look up validated and loaded 
                    if (this.inserting && !this.lookup.info.isParent && this.lookup.getIsValidated() && this.lookup.allLoaded) {
                        //If Validated lookup ata source list not filled or not binded with combo 
                        this.lookup.fillCombobox(this.getIsMandatory(), false, false, false); // fill lookup list
                        if (!this.lookup.loading) { // if not from validated cache list 
                            this.refreshOptions(this.lookup.data, newValue); // bind datasource 
                        }
                    }
                    else {
                        var pp = null;
                        if (!this.lookup.loading) {
                            pp = this.lookup.getFromList(newValue);
                        }
                        if (pp == null) {
                            pp = this.lookup.get(newValue);
                        }
                        if (pp != null) {
                            var valName = VIS.Utility.Util.getIdentifierDisplayVal(pp.Name);
                            this.ctrl.append('<option value="' + pp.Key + '">' + valName + '</option>');
                            this.ctrl.val(newValue);
                        }
                    }
                }
                //  Not in Lookup - set to Null
                if ((this.ctrl.val() == null) && (newValue != null)) {
                    this.oldValue = null;
                }
            }
            this.settingValue = false;
            //this.setBackground("white");
        }
        this.inserting = false;
    };

    /**
     * if Identifer value contains image path, then remove it and return remaining Identifier
     * @param {any} Name
     */
    VComboBox.prototype.getDisplayValue = function (Name) {
        var val = "";
        if (Name.indexOf("Images/") > -1) {
            val = Name.replace("^^" + Name.substring(Name.indexOf("Images/"), Name.lastIndexOf("^^") + 3), "_")
            if (val.indexOf("Images/") > -1) {
                val = val.replace(val.substring(val.indexOf("Images/"), val.lastIndexOf("^^") + 3), "_")
            }
        }
        else
            val = Name;
        return val;
    }

    VComboBox.prototype.getValue = function () {
        var val = this.ctrl.val();
        if (val == "-1") {
            return null;
        }
        return val;
    };

    /**
     *  recrete options of control
     *  @param data  object array
     *  @param selVal value to select
     */
    VComboBox.prototype.refreshOptions = function (data, selVal) {
        var output = [];
        var selIndex = -1;
        //userQueries = [];
        for (var i = 0; i < data.length; i++) {
            if (selVal && selVal == data[i].Key) {
                selIndex = i;
            }
            var val = VIS.Utility.Util.getIdentifierDisplayVal(data[i].Name);

            output[i] = '<option value="' + data[i].Key + '">' + val + '</option>';

            //userQueries.push({
            //    'title': val, 'label': val, 'value': val, 'id': data[i].Key
            //});
        }
        this.ctrl.empty();
        this.ctrl.html(output.join(''));

        //this.ctrl.autocomplete('option', 'source', userQueries);

        //if (selVal) {
        this.ctrl[0].selectedIndex = selIndex;
        // }
    };

    /**
     *  Return control display
     *  @return display value
     */
    VComboBox.prototype.getDisplay = function () {
        var retValue = "";
        if (this.lookup == null)
            retValue = this.ctrl.val();
        else
            retValue = this.lookup.getDisplay(this.ctrl.val());
        return retValue;
    };


    //VComboBox.prototype.dispose = function () {
    //    this.ctrl.off("focus");
    //    this.ctrl.off("change");
    //    this.ctrl.remove();
    //    this.ctrl = null;
    //    this.mField = null;
    //}
    //End VCombobox


    //6. VDate

    /**
     *	Create Date field
     *  @param columnName column name
     *  @param mandatory mandatory
     *  @param isReadOnly read only
     *  @param isUpdateable updateable
     *  @param displayType display type
     *  @param title title
     */
    function VDate(columnName, isMandatory, isReadOnly, isUpdateable, displayType, title) {

        var type = 'date';
        var max = '9999-12-31';

        if (displayType == VIS.DisplayType.Time) {
            type = 'time';
            max = '';
        }
        if (displayType == VIS.DisplayType.DateTime) {
            type = 'datetime-local';
            max += 'T24:00:00';
        }

        var $ctrl = $('<input>', {
            'type': type, name: columnName,
        });

        if (max != '') {
            $ctrl.attr('max', max);
        }

        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);

        //	ReadWrite
        if (isReadOnly || !isUpdateable)
            this.setReadOnly(true);
        else
            this.setReadOnly(false);


        var self = this;

        $ctrl.on("focusout", function (e) {
            if (!this.editingGrid) {
                var newVal = self.getValue();
                //if (newVal !== new Date(self.oldValue).toISOString()) {
                self.oldValue = newVal;
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
            }
        });


        /* privilized function */
        this.disposeComponent = function () {
            $ctrl.off("change");
            self = null;
            $ctrl = null;
        };
    }

    VIS.Utility.inheritPrototype(VDate, IControl);//inherit IControl

    VDate.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            if (newValue == null || newValue == "") {
                this.ctrl.val("");
                return;
            }

            var date = new Date(newValue);
            if (this.displayType != VIS.DisplayType.Date)
                date.setMinutes(-date.getTimezoneOffset() + date.getMinutes());

            newValue = date.toISOString();
            var val = newValue.substring(0, newValue.length - 1);
            var indexTime = newValue.indexOf("T");
            if (this.displayType == VIS.DisplayType.DateTime) {
                this.ctrl.val(val);
            }
            else if (this.displayType == VIS.DisplayType.Date) {
                this.ctrl.val(val.substring(0, indexTime));
            }
            else {
                //var d = new Date(newValue);
                //var h = d.getHours();
                //var m = d.getMinutes();
                var h = date.getUTCHours();
                var m = date.getUTCMinutes();

                this.ctrl.val(((h < 10) ? ("0" + h) : h) + ":" + ((m < 10) ? ("0" + m) : m));//.substring(indexTime + 1, val.length));
            }

            //this.setBackground("white");
        }
    };

    VDate.prototype.getValue = function () {
        var val = this.ctrl.val();
        if (val == null || val == "")
            return null;
        var d = null;

        if (this.displayType == VIS.DisplayType.Time) {
            d = new Date(0);
            var parts = val.match(/(\d+)\:(\d+)/);
            var hours = parseInt(parts[1], 10),
                minutes = parseInt(parts[2], 10);
            d.setHours(hours);
            d.setMinutes(minutes);
        }
        else {
            d = new Date(val + "Z");  // Done by Bharat on 08 Sep 2017 to solve Date time issue
        }

        try {
            if (this.displayType == VIS.DisplayType.DateTime)
                d.setMinutes(d.getTimezoneOffset() + d.getMinutes());
            return d.toISOString().replace('.000Z', 'Z');
        }
        catch (e) {
            console.log(val);
            return new Date();
        }
    };

    VDate.prototype.getDisplay = function () {
        return this.getValue();
    };

    //EndDate

    //6. VSearch

    /**
     *	Create lookup search field
     *  @param columnName column name
     *  @param mandatory mandatory
     *  @param isReadOnly read only
     *  @param isUpdateable updateable
     *  @param displayType display type
     *  @param title title
     */

    function VTextBoxButton(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup, zoomWindow_ID) {
        if (!displayType)
            displayType = VIS.DisplayType.Search;

        this.lookup = lookup;
        this.isMultiKeyTextBox = displayType === VIS.DisplayType.MultiKey;
        this.value = null;
        // Variable for setting custom info window while creating search control.- added by Mohit.
        this.custInfoWin = null;
        var _TableName = null;
        var _KeyColumnName = null;
        this.infoMultipleIds = null;
        var _value = null;
        this.log = VIS.Logging.VLogger.getVLogger("VTextBoxButton");
        // WindowNo for PrintCustomize */
        var WINDOW_INFO = 1113;

        // Tab for Info                */
        var TAB_INFO = 1113;

        var src = "";// VIS.Application.contextUrl + "Areas/VIS/Images/base/";

        if (displayType == VIS.DisplayType.Location || displayType == VIS.DisplayType.MultiKey) {
            if (!this.isMultiKeyTextBox) {
                src += "vis vis-card";
            }
            else {
                src += "fa fa-caret-down";//delete10.png";
            }
            //txtText.IsReadOnly = true;
        }
        else if (displayType == VIS.DisplayType.Locator) {
            src += "vis vis-locator";
        }
        else if (displayType == VIS.DisplayType.Search) {
            if (columnName.equals("C_BPartner_ID")
                || (columnName.equals("C_BPartner_To_ID") && lookup.getColumnName().equals("C_BPartner.C_BPartner_ID"))) {
                src += "fa fa-handshake-o";
            }
            else if (columnName.equals("M_Product_ID")
                || (columnName.equals("M_Product_To_ID") && lookup.getColumnName().equals("M_Product.M_Product_ID"))) {
                src += "vis vis-product";
            }
            else {
                src += "fa fa-caret-down";
            }
        }

        var btnCount = 0;
        //create ui
        var $ctrl = $('<input>', { type: 'text', name: columnName });
        $ctrl.attr('autocomplete', 'off');

        var $btnSearch = $('<button  tabindex="-1" class="input-group-text"><i  tabindex="-1" class="' + src + '"></i></button>');
        btnCount += 1;

        //Set Buttons and [pop up]

        var $btnPop = null;
        var $ulPopup = null;
        var $btnDelete = null;
        var options = {};
        var disabled = false;
        var addBtn = null;
        var addItem = null;

        if (lookup != null && !this.isMultiKeyTextBox) {

            if (lookup instanceof VIS.MLookup) {
                if (lookup.getZoomWindow() == 0 && (zoomWindow_ID < 1)) {
                    disabled = true;
                }
            }

            //$btnZoom = VIS.AEnv.getZoomButton(disabled);
            // btnCount += 1;
            options[VIS.Actions.zoom] = disabled;
            options[VIS.Actions.addnewrec] = true;

            //$btnPop = $('<button  tabindex="-1" class="input-group-text"><img tabindex="-1" src="' + VIS.Application.contextUrl + "Areas/VIS/Images/base/Info20.png" + '" /></button>');
            $btnPop = $('<button  tabindex="-1" class="input-group-text"><i tabindex="-1" Class="fa fa-ellipsis-v" /></button>');
            //	VBPartner quick entry link
            var isBP = false;
            if (columnName === "C_BPartner_ID") {
                options[VIS.Actions.addnewrec] = false;
                options[VIS.Actions.add] = true;
                options[VIS.Actions.update] = true;
            }

            if ((this.lookup &&
                (this.lookup.info.keyColumn.toLowerCase() == "ad_user.ad_user_id"
                    || this.lookup.info.keyColumn.toLowerCase() == "ad_user_id"))
                || columnName === "AD_User_ID" || columnName === "SalesRep_ID") {
                options[VIS.Actions.contact] = true;
            }

            if (VIS.MRole.getIsShowPreference())
                options[VIS.Actions.preference] = true;
            options[VIS.Actions.refresh] = true;
            options[VIS.Actions.remove] = true;

            $ulPopup = VIS.AEnv.getContextPopup(options);
            btnCount += 1;
            options = null;
        }

        if (this.isMultiKeyTextBox) {
            $btnPop = $('<button  tabindex="-1" class="input-group-text"><i class="fa fa-arrow-left" aria-hidden="true"></i></button>');
            btnCount += 1;
        }

        var self = this;

        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory); //call base function


        this.setReadOnly = function (readOnly) {

            this.isReadOnly = readOnly;
            $ctrl.prop('disabled', readOnly ? true : false);
            // this.$super.setReadOnly(readonly);
            if (readOnly) {
                $btnSearch.css("opacity", .7);
            } else {
                $btnSearch.css("opacity", 1);
            }

            if (this.isMultiKeyTextBox) {
                if (readOnly) {
                    $btnPop.css("opacity", .7);
                } else {
                    $btnPop.css("opacity", 1);
                }
            }

        };
        // Autocomplete
        if (displayType == VIS.DisplayType.Search) {
             addBtn = $("<div class='vis-autocompleteList-item vis-auto-addItem' style='background-color: rgba(var(--v-c-secondary), 1)'>" + VIS.Msg.getMsg("AddNew") + "</div>");
             addItem = $("<div><center>" + VIS.Msg.getMsg("NoDataFoundSugg") + "</center></div>").append($("<center></center>").append(addBtn));
            $ctrl.vaautocomplete({
                source: function (term, response) {
                    var sql = self.lookup.info.query;
                    var keyColumn = self.lookup.info.keyColumn;
                    var displayColumn = self.lookup.info.displayColSubQ;
                    sql = sql.replace(displayColumn, '');

                    var posFrom = sql.indexOf(" FROM ");
                    var hasWhere = sql.indexOf(" WHERE ", posFrom) != -1;
                    var posOrder = sql.lastIndexOf(" ORDER BY ");
                    var validation = "";
                    if (!self.lookup.info.isValidated) {
                        validation = VIS.Env.parseContext(VIS.context, self.lookup.windowNo, self.lookup.tabNo, self.lookup.info.validationCode, false, true);
                        if (validation.length == 0 && self.lookup.info.validationCode.length > 0) {
                            return;
                        }
                        validation = " AND " + validation;
                    }

                    if (posOrder != -1) {
                        var orderByIdx = validation.toUpper().lastIndexOf(" ORDER BY ");
                        if (orderByIdx == -1) {
                            validation = validation + sql.substring(posOrder);
                        }
                        sql = sql.substring(0, posOrder) + (hasWhere ? " AND " : " WHERE ") + self.lookup.info.tableName + ".isActive='Y' " + validation;
                    }
                    else {
                        sql += (hasWhere ? " AND " : " WHERE ") + self.lookup.info.tableName + ".isActive='Y' " + validation;
                    }

                    var lastPart = sql.substr(sql.indexOf('FROM'), sql.length);
                    sql = "SELECT " + keyColumn + " AS ID,NULL," + displayColumn + " AS finalValue " + lastPart;

                    term = term.toUpper();
                    term = "%" + term + "%";
                    $.ajax({
                        type: 'Post',
                        url: VIS.Application.contextUrl + "Form/GetAccessSqlAutoComplete",
                        data: { sql: VIS.secureEngine.encrypt(sql), columnName: columnName, text: term },
                        success: function (data) {
                            var res = [];
                            if (JSON.parse(data) != null) {
                                result = JSON.parse(data).Table;
                                for (var i = 0; i < result.length; i++) {
                                    var parseObj = {};
                                    parseObj[Object.keys(result[i])[0].toLowerCase()] = result[i][Object.keys(result[i])[0]];
                                    parseObj[Object.keys(result[i])[1].toLowerCase()] = result[i][Object.keys(result[i])[1]];
                                    parseObj[Object.keys(result[i])[2].toLowerCase()] = result[i][Object.keys(result[i])[2]];
                                    res.push({
                                        id: parseObj.id,
                                        value: VIS.Utility.Util.getIdentifierDisplayVal(parseObj.finalvalue)
                                    });
                                }
                                response(res);
                            }
                            if (res.length == 0) {
                                res = [];
                                res.push({
                                    id: "vis-AddNew",
                                    value: VIS.Msg.getMsg("AddNew"),
                                    msg: VIS.Msg.getMsg("NoDataFound")//"No data found. Do you want to add?"                                    
                                });
                                response(res);
                            }
                        },
                    });

                },
                minLength: 2,
                html: addItem,
                onSelect: function (e, item) {
                    if (item.id == "vis-AddNew") {
                        zoomAction();
                        setTimeout(function () {
                            self.setValue(-1, true, true);
                        }, 500);
                    } else {
                        self.setValue(item.id, true, true);
                    }
                }
            });
            addBtn.on("click", function (event) {
                zoomAction();
            })
        }
        $ctrl.on("keydown", function (event) {

            //if (event.shiftKey && event.keyCode == 13) {
            //$('input, select, textarea, button')[$('input,select,textarea,button').index(this) + 1].focus();
            // $ctrl                   



            //        event.stopPropagation();
            //        event.preventDefault();
            //    }

            //else 
            if ((event.keyCode == 13 || (event.keyCode == 9 && $ctrl.val().trim() != '')) && !event.shiftKey && $ctrl.val().length == 0) {//will work on press of Tab key OR Enter Key
                if (self.actionText()) {
                    event.stopPropagation();
                    event.preventDefault();
                }
            }
            else if ((event.keyCode == 46 || event.keyCode == 8) && self.getValue() != null) {
                self.setValue(null, true, true);
            }

        });


        /// <summary>
        ///Check, if data returns unique entry, otherwise involve Info via Button
        /// </summary>
        this.actionText = function () {
            var text = $ctrl.val().trim();
            ;
            VIS.context.setContext(self.lookup.windowNo, "AttrCode", text);

            // Change GSI Barcode
            // To get Product UPC from GS1 bar code
            if (text != null) {
                // for GS1 bar code starting with (01)
                if (text.indexOf("(01)") >= 0) {
                    var prodUPC = text;
                    // check whether (10) exists in the UPC (For Lot No)
                    if (prodUPC.indexOf("(10)") >= 0) {
                        prodUPC = prodUPC.substring(0, prodUPC.indexOf("(10)"));
                    }
                    // check whether (17) exists in the UPC (For Expiry Date)
                    if (prodUPC.indexOf("(17)") >= 0) {
                        prodUPC = prodUPC.substring(0, prodUPC.indexOf("(17)"));
                    }
                    prodUPC = prodUPC.replace("(01)", "");
                    text = prodUPC;
                }
            }

            //	Nothing entered
            if (text == null || text.length == 0 || text.equals("%")) {
                self.openSearchForm();
                return true;
            }
            text = text.toUpper();

            var id = -3;
            var keyId = null;
            var dr = null;


            var finalSql = VIS.Msg.parseTranslation(VIS.context, self.getDirectAccessSQL(text));
            try {
                // dr = executeReader(finalSql);

                var dr = null;
                $.ajax({
                    type: 'Post',
                    async: false,
                    url: VIS.Application.contextUrl + "Form/GetTextButtonQueryResult",
                    data: { sql: VIS.secureEngine.encrypt(finalSql) },
                    success: function (data) {
                        dr = new VIS.DB.DataReader().toJson(data)
                    },
                });


                if (dr.read()) {
                    try {
                        keyId = parseInt(dr.get(0));	//	first
                    }
                    catch (ex) {
                        keyId = dr.get(0);
                    }
                    id = 1;
                    if (dr.read()) {
                        id = -1;			//	only if unique
                        keyId = null;
                    }
                }
                dr.close();
                dr = null;
            }
            catch (ee) {
                if (dr != null)
                    dr.close();
                dr = null;
                this.log.log(VIS.Logging.Level.SEVERE, e);

                id = -2;
            }

            //	Try like
            if (id == -3 && !text.endsWith("%")) {
                text += "%";
                finalSql = VIS.Msg.parseTranslation(VIS.context, self.getDirectAccessSQL(text));
                try {
                    // dr = executeReader(finalSql);


                    var dr = null;
                    $.ajax({
                        type: 'Get',
                        async: false,
                        url: VIS.Application.contextUrl + "Form/GetTextButtonQueryResult",
                        data: { sql: VIS.secureEngine.encrypt(finalSql) },
                        success: function (data) {
                            dr = new VIS.DB.DataReader().toJson(data)
                        },
                    });

                    if (dr.read()) {
                        //id = rs.getInt(1);		//	first
                        try {
                            keyId = parseInt(dr.get(0));	//	first
                        }
                        catch (es) {
                            keyId = dr.get(0);
                        }
                        id = 1;
                        if (dr.read()) {
                            keyId = null;
                            id = -1;			//	only if unique
                        }
                    }
                    dr.close();
                }
                catch (ewe) {
                    if (dr != null)
                        dr.close();
                    dr = null;
                    this.log.log(VIS.Logging.Level.SEVERE, e);

                    id = -2;
                }
            }


            //	No (unique) result
            if (id < 0 && keyId == null) {
                //if (id == -3)
                // this.log.log(VIS.Logging.Level.INFO, _columnName + " - Not Found - " + finalSql);
                //else
                //  this.log.log(VIS.Logging.Level.INFO, _columnName + " - Not Unique - " + finalSql);
                _value = {};	// force re-display
                self.openSearchForm();
                return true;
            }

            //if (this.oldValue == keyId) {
            //    return false;
            //}

            text = "";

            self.setValue(keyId, true, true); //bind value and text
            return false;


        };


        /// <summary>
        /// Generate Access SQL for Search.
        /// The SQL returns the ID of the value entered
        /// Also sets _tableName and _keyColumnName
        /// </summary>
        /// <param name="text">uppercase text for LIKE comparison</param>
        /// <returns>sql or ""</returns>
        this.getDirectAccessSQL = function (text) {
            var result = self.getDirectAccessSQL1(VIS.context, columnName, lookup, text);
            _TableName = result[1];
            _KeyColumnName = result[2];
            return result[0];
        };


        /// <summary>
        /// Generate Access SQL for Search.
        /// The SQL returns the ID of the value entered
        /// Also sets _tableName and _keyColumnName
        /// </summary>
        /// <param name="ctx">Current Context</param>
        /// <param name="_columnName">Column Name</param>
        /// <param name="_lookup">Lookup Object</param>
        /// <param name="text">uppercase text for LIKE comparison</param>
        /// <returns>An array of 3 Strings; 0=SQL, 1=_tableName, 2=_keyColumnName</returns>
        this.getDirectAccessSQL1 = function (ctx, _columnName, _lookup, text) {
            var sql = "";
            var retVal = [];
            var _tableName = _columnName.substring(0, _columnName.length - 3);
            var _keyColumnName = _columnName;
            //
            if (_columnName.equals("M_Product_ID")) {
                //	Reset
                ctx.setContext(WINDOW_INFO, TAB_INFO, "M_Product_ID", "0");
                ctx.setContext(WINDOW_INFO, TAB_INFO, "M_AttributeSetInstance_ID", "0");
                ctx.setContext(WINDOW_INFO, TAB_INFO, "M_Locator_ID", "0");
            }
            else if (_columnName.equals("SalesRep_ID")) {
                _tableName = "AD_User";
                _keyColumnName = "AD_User_ID";
            }

            $.ajax({
                url: VIS.Application.contextUrl + "Form/GetAccessSql",
                dataType: "json",
                async: false,
                data: {
                    columnName: _columnName,
                    text: text
                },
                success: function (data) {
                    sql = data;
                }
            });


            //	Predefined
            if (sql.length > 0) {
                var wc = self.getWhereClause(ctx, _columnName, _lookup);
                if (_columnName.equals("M_Product_ID")) {
                    if (wc != null && wc.length > 0)
                        sql += " AND " + wc.replace(/M_Product\./g, "p.") + " AND p.IsActive='Y'";
                }
                else {
                    if (wc != null && wc.length > 0)
                        sql += " AND " + wc + " AND IsActive='Y'";
                }
                //	***
                // //log.finest(_columnName + " (predefined) " + sql.toString());

                retVal.push(VIS.MRole.getDefault().addAccessSQL(sql, _tableName, VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO));
                retVal.push(_tableName);
                retVal.push(_keyColumnName);
                return retVal;

            }

            //	Check if it is a Table Reference
            var dr = null;
            if (_lookup != null && _lookup instanceof VIS.MLookup) {
                var AD_Reference_ID = _lookup.getAD_Reference_Value_ID();
                if (AD_Reference_ID != 0) {

                    // Commented 10 Aug 2015 For : not searching based on the identifiers
                    var tblQuery = "VIS_84";

                    try {
                        var param = [];
                        param[0] = new VIS.DB.SqlParam("@refid", AD_Reference_ID);
                        dr = executeReader(tblQuery, param);

                        while (dr.read()) {
                            _columnName = dr.get(0);
                            _keyColumnName = dr.get(0);
                            _tableName = dr.get(1);
                            break;
                        }

                        dr.close();
                        dr = null;
                    }
                    catch (e) {
                        if (dr != null) {
                            dr.close();
                            dr = null;
                        }
                        this.log.log(VIS.Logging.Level.SEVERE, squery, e);
                        //Logging.VLogger.Get().Log(Logging.Level.SEVERE, sql.ToString(), ex);
                    }


                    var query = "VIS_85";

                    var displayColumnName = null;

                    _keyColumnName = _columnName;

                    sql = "(";

                    try {
                        var param = [];
                        param[0] = new VIS.DB.SqlParam("@refid", AD_Reference_ID);
                        param[1] = new VIS.DB.SqlParam("@colname", _columnName);
                        dr = executeReader(query, param);

                        while (dr.read()) {
                            if (sql.length > 1)
                                sql += " OR ";
                            _tableName = dr.get(0);
                            sql += "UPPER(" + dr.get(1) + ") LIKE " + VIS.DB.to_string(text);
                        }
                        sql += ")";
                        dr.close();
                        dr = null;

                    }
                    catch (e) {
                        if (dr != null) {
                            dr.close();
                            dr = null;
                        }
                        this.log.log(VIS.Logging.Level.SEVERE, squery, e);
                        //Logging.VLogger.Get().Log(Logging.Level.SEVERE, sql.ToString(), ex);
                    }

                    if (sql.length == 0) {
                        this.log.log(VIS.Logging.Level.SEVERE, _columnName + " (TableDir) - no standard/identifier columns");
                        //Logging.VLogger.Get().Log(Logging.Level.SEVERE, _columnName + " (TableDir) - no standard/identifier columns");
                        retVal.push("");
                        retVal.push(_tableName);
                        retVal.push(_keyColumnName);
                        return retVal;
                        //return new String[] { "", _tableName, _keyColumnName };
                    }
                    //
                    var retValue = "SELECT " + _columnName + " FROM " + _tableName + " WHERE " + sql + " AND IsActive='Y'";
                    var _wc = self.getWhereClause(ctx, _columnName, _lookup);
                    if (_wc != null && _wc.length > 0)
                        retValue += " AND " + _wc;
                    //	***
                    ////log.finest(_columnName + " (TableDir) " + sql.toString());
                    retVal.push(VIS.MRole.getDefault().addAccessSQL(retValue, _tableName, VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO));
                    retVal.push(_tableName);
                    retVal.push(_keyColumnName);
                    return retVal;

                    // Commented 10 Aug 2015 For : not searching based on the identifiers
                    //var query = "SELECT kc.ColumnName, dc.ColumnName, t.TableName "
                    //    + "FROM AD_Ref_Table rt"
                    //    + " INNER JOIN AD_Column kc ON (rt.Column_Key_ID=kc.AD_Column_ID)"
                    //    + " INNER JOIN AD_Column dc ON (rt.Column_Display_ID=dc.AD_Column_ID)"
                    //    + " INNER JOIN AD_Table t ON (rt.AD_Table_ID=t.AD_Table_ID) "
                    //    + "WHERE rt.AD_Reference_ID=@refid";
                    //var displayColumnName = null;

                    //try {
                    //    var param = [];
                    //    param[0] = new VIS.DB.SqlParam("@refid", AD_Reference_ID);
                    //    dr = VIS.DB.executeReader(query, param);
                    //    while (dr.read()) {
                    //        _keyColumnName = dr.get(0);
                    //        displayColumnName = dr.get(1);
                    //        _tableName = dr.get(2);
                    //    }
                    //    dr.close();
                    //    dr = null;

                    //}
                    //catch (e) {
                    //    if (dr != null) {
                    //        dr.close();
                    //        dr = null;
                    //    }
                    //    this.log.log(VIS.Logging.Level.SEVERE, query, e);
                    //}

                    //if (displayColumnName != null) {
                    //    sql = "";
                    //    sql += "SELECT " + _keyColumnName + " FROM " + _tableName + " WHERE UPPER(" + displayColumnName + ") LIKE ";
                    //    sql += VIS.DB.to_string(text) + " AND IsActive='Y'";
                    //    var wc = self.getWhereClause(ctx, _columnName, _lookup);
                    //    if (wc != null && wc.length > 0)
                    //        sql += " AND " + wc;
                    //    //	***
                    //    //log.finest(_columnName + " (Table) " + sql.toString());

                    //    retVal.push(VIS.MRole.getDefault().addAccessSQL(sql, _tableName, VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO));
                    //    retVal.push(_tableName);
                    //    retVal.push(_keyColumnName);
                    //    return retVal;

                    //}
                }	//	Table Reference
            }	//	MLookup

            /** Check Well Known Columns of Table - assumes TableDir	**/
            var squery = "VIS_86";
            _keyColumnName = _columnName;
            sql = "(";

            //IDataReader dr = null;
            try {
                var param = [];
                param[0] = new VIS.DB.SqlParam("@colname", _keyColumnName);
                dr = executeReader(squery, param);

                while (dr.read()) {
                    if (sql.length > 1)
                        sql += " OR ";
                    _tableName = dr.get(0);
                    sql += "UPPER(" + dr.get(1) + ") LIKE " + VIS.DB.to_string(text);
                }
                sql += ")";
                dr.close();
                dr = null;

            }
            catch (e) {
                if (dr != null) {
                    dr.close();
                    dr = null;
                }
                this.log.log(VIS.Logging.Level.SEVERE, squery, e);
                //Logging.VLogger.Get().Log(Logging.Level.SEVERE, sql.ToString(), ex);
            }


            if (sql.length == 0) {
                this.log.log(VIS.Logging.Level.SEVERE, _columnName + " (TableDir) - no standard/identifier columns");
                //Logging.VLogger.Get().Log(Logging.Level.SEVERE, _columnName + " (TableDir) - no standard/identifier columns");
                retVal.push("");
                retVal.push(_tableName);
                retVal.push(_keyColumnName);
                return retVal;
                //return new String[] { "", _tableName, _keyColumnName };
            }
            //
            var retValue = "SELECT " + _columnName + " FROM " + _tableName + " WHERE " + sql + " AND IsActive='Y'";
            var _wc = self.getWhereClause(ctx, _columnName, _lookup);
            if (_wc != null && _wc.length > 0)
                retValue += " AND " + _wc;
            //	***
            ////log.finest(_columnName + " (TableDir) " + sql.toString());
            retVal.push(VIS.MRole.getDefault().addAccessSQL(retValue, _tableName, VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO));
            retVal.push(_tableName);
            retVal.push(_keyColumnName);
            return retVal;

        };	//	getDirectAccessSQL





        /// <summary>
        /// Get Where Clause
        /// </summary>
        /// <param name="ctx">Current Context</param>
        /// <param name="_ColumnName">Column Name</param>
        /// <param name="_Lookup">Lookup reference</param>
        /// <returns>where clause or ""</returns>
        this.getWhereClause = function (ctx, _ColumnName, _Lookup) {
            //_Lookup = (MLookup)_Lookup;
            var WhereClause = "";
            try {

                if ((_Lookup) == null)
                    return "";
                if (_Lookup.getZoomQuery() != null)
                    WhereClause = _Lookup.getZoomQuery().getWhereClause();
                var validation = _Lookup.getValidation();
                if (validation == null)
                    validation = "";
                if (WhereClause.length == 0)
                    WhereClause = validation;
                else if (validation.length > 0)
                    WhereClause += " AND " + validation;
                //	//log.finest("ZoomQuery=" + (_lookup.getZoomQuery()==null ? "" : _lookup.getZoomQuery().getWhereClause())
                //		+ ", Validation=" + _lookup.getValidation());
                if (WhereClause.indexOf('@') != -1) {
                    var validated = VIS.Env.parseContext(ctx, _Lookup.getWindowNo(), _Lookup.getTabNo(), WhereClause, false, true);
                    if (validated.length == 0) {
                        ////log.severe(_columnName + " - Cannot Parse=" + whereClause);
                    }
                    else {
                        ////log.fine(_columnName + " - Parsed: " + validated);
                        return validated;
                    }
                }
            }
            catch (eee) {
            }
            return WhereClause;
        };

        //	ReadWrite
        if (isReadOnly || !isUpdateable)
            this.setReadOnly(true);
        else
            this.setReadOnly(false);
        /* provilized function */

        /* @overridde
        */
        this.getBtnCount = function () {
            return btnCount;
        };

        /** 
            get contols button by index 
            -  zoom or info button 
            - index 0 for info button 
            - index 1 for zoom button
            - control must tell or implemnt getBtnCount default is zero [no button]
        *
        */
        this.getBtn = function (index) {
            if (index == 0) {
                return $btnSearch;
            }

            if (index == 1) { //zoom
                if ($btnPop)
                    return $btnPop;
            }
        };


        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                //$ctrl.show();
                $ctrl.css("visibility", "visible");
                if ($btnPop)
                    $btnPop.show();
                $btnSearch.show();

            } else {
                //$ctrl.hide();
                $ctrl.css("visibility", "hidden");
                if ($btnPop)
                    $btnPop.hide();
                $btnSearch.hide();
            }
        };

        this.openSearchForm = function () {
            if (self.isReadOnly) {
                return;
            }
            // e.stopPropagation();
            //alert("[pending] Info window for [" + self.value + "] => " + self.getName());
            var text = $ctrl.val().trim();
            if (typeof (text) == "object") {
                text = "";
            }
            if (self.isReadOnly)
                return;
            if (self.lookup == null)
                return;
            var infoWinID = 0;
            var wc = self.getWhereClause(VIS.Env.getCtx(), columnName, self.lookup);
            // Check if custom info window set on control.- done by mohit- asked by mukesh sir - 13 Feb 2019
            if (self.custInfoWin != null) {
                // Call controller to get info window id from search key.
                $.ajax({
                    url: VIS.Application.contextUrl + "InfoWindow/GetInfoWindowID",
                    dataType: "json",
                    async: false,
                    data: {
                        InfoSearchKey: self.custInfoWin

                    },
                    success: function (data) {
                        infoWinID = data.result;
                    }
                });

            }
            // Get info window from field if exist.
            else if (self.getField() != null & infoWinID == 0) {
                infoWinID = self.getField().getAD_InfoWindow_ID();
            }
            var InfoWindow = null;

            if (infoWinID != 0) {
                InfoWindow = new VIS.InfoWindow(infoWinID, text, self.lookup.windowNo, wc, self.isMultiKeyTextBox);

            }
            else {
                var tableName = tableName;
                var _keyColumnName = columnName;
                var query = null;
                var dr = null;

                // Added by Bharat 
                var M_Warehouse_ID = 0, M_PriceList_ID = 0, window_ID = 0;

                //
                var AD_Reference_ID = self.lookup.getAD_Reference_Value_ID();
                if (AD_Reference_ID > 0) {
                    query = "VIS_87";
                    var displayColumnName = null;

                    try {
                        var param = [];
                        param[0] = new VIS.DB.SqlParam("@AD_Reference_ID", AD_Reference_ID);
                        dr = executeReader(query, param);
                        while (dr.read()) {
                            _keyColumnName = dr.getString(0);//.Table.rows[0].cells['ColumnName']; [0].ToString();
                            displayColumnName = dr.getString(1); //dr[1].ToString();
                            tableName = dr.getString(2); //dr[2].ToString();
                        }
                        dr.close();
                        dr = null;

                    }
                    catch (e) {
                        if (dr != null) {
                            dr.close();
                            dr = null;
                        }
                        //Logging.VLogger.Get().Log(Logging.Level.SEVERE, query, e);
                    }
                }
                else {
                    tableName = String(columnName).substr(0, columnName.length - 3);
                    _keyColumnName = columnName;
                }

                // Added by Bharat    For Product Info
                //VIS.context.getContext(self.lookup.windowNo, "0|AD_Tab_ID", true)
                //query = "SELECT AD_Window_ID FROM AD_Window WHERE Name = '" + VIS.context.getContext(self.lookup.windowNo, "WindowName") + "'";
                query = "VIS_88";
                var param = [];
                param[0] = new VIS.DB.SqlParam("@AD_Tab_ID", VIS.context.getContext(self.lookup.windowNo, "0|AD_Tab_ID", true));

                window_ID = executeScalar(query, param);
                if (_keyColumnName.equals("M_Product_ID") && window_ID) {
                    if (window.DTD001 && window_ID == 170) {
                        M_Warehouse_ID = VIS.context.getContextAsInt(self.lookup.windowNo, "DTD001_MWarehouseSource_ID");
                    }
                    else {
                        M_Warehouse_ID = VIS.context.getContextAsInt(self.lookup.windowNo, "M_Warehouse_ID");
                    }
                    M_PriceList_ID = VIS.context.getContextAsInt(self.lookup.windowNo, "M_PriceList_ID");
                    var multipleSelection = false;
                    if (self.lookup.windowNo > 0) {
                        multipleSelection = (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "C_OrderLine_ID") ||
                            (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "C_InvoiceLine_ID") ||
                            (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "M_InOutLine_ID") ||
                            (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "M_PackageLine_ID") ||
                            (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "M_MovementLine_ID") ||
                            (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "M_InventoryLine_ID") ||
                            (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "M_ProductPrice_ID") ||
                            (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "C_ProjectLine_ID") ||
                            (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "M_RequisitionLine_ID") ||
                            (VIS.context.getWindowTabContext(self.lookup.windowNo, 0, "KeyColumnName") == "M_PriceList_ID") ||
                            (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "SAP001_StockTransferLine_ID") //;
                            ;
                    }
                    InfoWindow = new VIS.infoProduct(true, self.lookup.windowNo, M_Warehouse_ID, M_PriceList_ID,
                        text, tableName, _keyColumnName, multipleSelection, wc);
                }
                else {
                    //try get dynamic window
                    // Change by mohit - to change the logic of getting dynamic window. Date - 17 october 2017
                    query = "VIS_89";
                    var param = [];
                    param[0] = new VIS.DB.SqlParam("@tableName", tableName);

                    dr = executeReader(query, param);
                    while (dr.read()) {
                        infoWinID = dr.getString(0);
                        break;
                    }
                    dr.close();
                    dr = null;
                    if (infoWinID > 0) {
                        InfoWindow = new VIS.InfoWindow(infoWinID, text, self.lookup.windowNo, wc, self.isMultiKeyTextBox);
                    }
                    else {
                        InfoWindow = new VIS.infoGeneral(true, self.lookup.windowNo, text,
                            tableName, _keyColumnName, self.isMultiKeyTextBox, wc);
                    }
                }
            }


            InfoWindow.onClose = function () {
                //self.setValue(InfoWindow.getSelectedValues(), false, true);

                var objResult = InfoWindow.getSelectedValues();

                if (objResult && objResult.length == 0) {

                    if ($ctrl.closest('td').hasClass('vis-gc-vpanel-table-td3')) {
                        //var next = $ctrl.closest('tr').next('tr');
                        //if (next && next.length > 0) {
                        //    window.setTimeout(function () {
                        //        next.find('.vis-gc-vpanel-table-td1').find('input,select,textarea,button').focus();
                        //    }, 200);
                        //}

                        window.setTimeout(function () {
                            $ctrl.closest('tr').nextAll().children('input,select,textarea,button').focus();
                        }, 200);
                    }
                    else if ($ctrl.closest('td').hasClass('vis-gc-vpanel-table-td1')) {
                        var next = $ctrl.closest('td').siblings('.vis-gc-vpanel-table-td3');
                        if (next && next.length > 0) {
                            next = next.find('input,select,textarea,button');
                            if (next && next.length > 0) {
                                window.setTimeout(function () {
                                    next.focus();
                                }, 200);
                            }
                            else {
                                //var next = $ctrl.closest('tr').next('tr');
                                //if (next && next.length > 0) {
                                //    window.setTimeout(function () {
                                //        next.find('.vis-gc-vpanel-table-td1').find('input,select,textarea,button').focus();
                                //    }, 200);
                                //}
                                window.setTimeout(function () {
                                    $ctrl.closest('tr').nextAll().children('input,select,textarea,button').focus();
                                }, 200);
                            }
                        }
                    }
                    else {
                        window.setTimeout(function () {
                            $ctrl.closest('td').siblings('.vis-gc-vpanel-table-td3').find('input,select,textarea,button').focus();
                        }, 200);
                    }


                    $ctrl.closest('td').nextAll().children('input,select,textarea,button').focus();
                    // change by mohit to refresh the grid if resfresh ui status sent true from info window.
                    if (InfoWindow.constructor.name == "infoProduct" && InfoWindow.getRefreshStatus()) {
                        self.fireRefreshUI();
                    }
                    return;
                }


                if (self.isMultiKeyTextBox) {

                    var sb = "";
                    var i = 0;
                    var j = 0;
                    for (i = 0, j = objResult.length; i < j; i++) {
                        if (sb.length == 0) {
                            sb += objResult[i];
                            continue;
                        }
                        sb += "," + objResult[i];
                    }

                    self.setValue(sb, false, true);
                }
                else {

                    var newVal = null;
                    if (InfoWindow.getRefreshStatus && InfoWindow.getRefreshStatus()) {
                        self.fireRefreshUI();
                    }

                    else {

                        if (objResult != null && objResult.length == 1) {
                            newVal = objResult[0];
                        }

                        else {
                            //if ((_value.ToString() != objResult.ToString()))
                            InfoMultipleIds = null;

                            if (objResult.length > 1) {
                                newVal = objResult[0];
                            }
                        }
                        if (newVal != null) {
                            self.setValue(newVal, false, true);
                        }
                    }
                }
                InfoWindow = null;
            };
            InfoWindow.show();
        };

        $btnSearch.on(VIS.Events.onClick, self.openSearchForm);

        function zoomAction(value) {

            if (!self.lookup || disabled)
                return;
            //
            var zoomQuery = self.lookup.getZoomQuery();
            //var value = self.getValue();
            if (!value)
                value = self.getValue();

            if (value == "")
                value = null;

            //	If not already exist or exact value
            if ((zoomQuery == null || $.isEmptyObject(zoomQuery)) || (value != null)) {
                zoomQuery = new VIS.Query();	//	ColumnName might be changed in MTab.validateQuery

                var keyColumnName = null;
                //	Check if it is a Table Reference
                if ((self.lookup != null) && (self.lookup instanceof VIS.MLookup)) {
                    var AD_Reference_ID = self.lookup.getAD_Reference_Value_ID();
                    if (AD_Reference_ID != 0) {
                        var query = "VIS_90";
                        var param = [];
                        param[0] = new VIS.DB.SqlParam("@AD_Reference_ID", AD_Reference_ID);
                        try {
                            var dr = executeReader(query, param);
                            if (dr.read()) {
                                keyColumnName = dr.getString(0);
                            }
                            dr.dispose();
                        }
                        catch (e) {
                            this.log.log(VIS.Logging.Level.SEVERE, query, e);
                        }
                    }	//	Table Reference
                }	//	MLookup

                if ((keyColumnName != null) && (keyColumnName.length != 0))
                    zoomQuery.addRestriction(keyColumnName, VIS.Query.prototype.EQUAL, value);
                else
                    zoomQuery.addRestriction(self.getColumnName(), VIS.Query.prototype.EQUAL, value);
                zoomQuery.setRecordCount(1);	//	guess
            }

            var AD_Window_ID = 0;
            if (self.mField != null && self.mField.getZoomWindow_ID() > 0) {
                AD_Window_ID = self.mField.getZoomWindow_ID();
            }
            else if (zoomWindow_ID && zoomWindow_ID > 0) {
                AD_Window_ID = zoomWindow_ID;
            }
            else {
                AD_Window_ID = self.lookup.getZoomWindow(zoomQuery);
            }



            //
            //this.log.info(this.getColumnName() + " - AD_Window_ID=" + AD_Window_ID
            //    + " - Query=" + zoomQuery + " - Value=" + value);
            //
            //setCursor(Cursor.getPredefinedCursor(Cursor.WAIT_CURSOR));
            //
            VIS.viewManager.startWindow(AD_Window_ID, zoomQuery);

            //setCursor(Cursor.getDefaultCursor());

        };


        if ($btnPop) {
            $btnPop.on(VIS.Events.onClick, function (e) {



                if (!self.isMultiKeyTextBox)
                    $btnPop.w2overlay($ulPopup.clone(true));
                else {
                    if (self.isReadOnly) {
                        return;
                    }
                    var val = self.getValue();
                    if (val && val.toString().length > 0) {
                        self.setValue(null, false, true);
                    }
                }
                e.stopPropagation();
            });
        }

        if ($ulPopup) {
            $ulPopup.on("click", "LI", function (e) {

                var action = $(e.target).data("action");
                if (action == VIS.Actions.zoom) {
                    if (disabled)
                        return;
                    zoomAction();
                }
                else if (action == VIS.Actions.addnewrec) {
                    if (disabled)
                        return;
                    zoomAction(-10);
                }
                else if (action == VIS.Actions.preference) {
                    var obj = new VIS.ValuePreference(self.mField, self.getValue(), self.getDisplay());
                    if (obj != null) {
                        obj.showDialog();
                    }
                    obj = null;
                }
                else if (action == VIS.Actions.refresh) {

                    if (!self.lookup || self.isReadOnly)
                        return;
                    self.lookup.refresh();
                }
                else if (action == VIS.Actions.add) {
                    var val = self.getValue();
                    // if (val && val.toString().length > 0) {
                    VIS.AddUpdateBPartner(self.mField.getWindowNo(), 0, VIS.Msg.getMsg("Customer"), null, 0, 0);
                    //  var contactInfo = new VIS.ContactInfo(101, 1);
                    // contactInfo.show();
                    //}
                }
                else if (action == VIS.Actions.update) {
                    var val = self.getValue();
                    if (val && val.toString().length > 0) {
                        VIS.AddUpdateBPartner(self.mField.getWindowNo(), val, VIS.Msg.getMsg("Customer"), null, 0, 0);
                    }
                }
                else if (action == VIS.Actions.contact) {
                    var val = self.getValue();
                    if (val && val.toString().length > 0) {
                        //var contactInfo = new VIS.ContactInfo(val, this.mField.getWindowNo());
                        var contactInfo = new VIS.ContactInfo(val, self.mField.getWindowNo());
                        contactInfo.show();

                    }
                }
                else if (action == VIS.Actions.remove) {
                    if (self.isReadOnly)
                        return;
                    self.setValue(null, false, true);
                }
            });
        }

        /** 
        *  dispose 
        */
        this.disposeComponent = function () {
            $btnSearch.off(VIS.Events.onClick);

            if ($btnPop)
                $btnPop.off(VIS.Events.onClick);
            if ($ulPopup)
                $ulPopup.off(VIS.Events.onTouchStartOrClick);

            $ulPopup = null;
            self = null;
            $ctrl = null;
            $btnSearch = null;
            $btnPop = null;
            this.getBtn = null;
            this.setVisible = null;
            if (addBtn) {
                addBtn.off("click");
            }
            addBtn = null;
            addItem = null;
            
        };
    };

    VIS.Utility.inheritPrototype(VTextBoxButton, IControl);//inherit IControl

    VTextBoxButton.prototype.setValue = function (newValue, refrsh, fire) {
        if (this.oldValue != newValue || refrsh) {
            this.settingValue = true;
            this.oldValue = newValue;
            this.value = newValue
            //	Set comboValue
            if (newValue == null) {
                this.lastDisplay = "";
                this.ctrl.val("");
                this.settingValue = false;
                if (fire) {
                    var evt = { newValue: newValue, propertyName: this.getName() };
                    this.fireValueChanged(evt);
                    evt = null;
                }
                return;
            }
            if (this.lookup == null) {
                this.ctrl.val(newValue.toString());
                this.lastDisplay = newValue.toString();
                this.settingValue = false;
                return;
            }

            if (this.displayType !== VIS.DisplayType.MultiKey)
                this.lastDisplay = this.lookup.getDisplay(newValue);
            else {
                var arr = newValue.toString().split(',');
                var sb = "";

                for (var i = 0, j = arr.length; i < j; i++) {
                    var val = arr[i];
                    if (!isNaN(val)) {
                        val = Number(val);
                    }
                    if (sb.length == 0) {
                        sb += this.lookup.getDisplay(val);
                        continue;
                    }
                    sb += ", " + this.lookup.getDisplay(val);
                }
                this.lastDisplay = sb;
            }
            if (this.lastDisplay.equals("<-1>")) {
                this.lastDisplay = "";
                this.oldValue = null;
                this.value = null;
            }

            this.value = newValue;

            var ctrlval = VIS.Utility.Util.getIdentifierDisplayVal(this.lastDisplay);
            this.ctrl.val(VIS.Utility.decodeText(ctrlval));

            this.settingValue = false;
            //this.setBackground("white");
            if (fire) {
                if (newValue == -1 || newValue == "") {
                    newValue = null;
                }
                var evt = { newValue: newValue, propertyName: this.getName() };
                this.fireValueChanged(evt);
                evt = null;
            }
        }
        else if (newValue == null) {
            this.ctrl.val("");
        }
    };

    VTextBoxButton.prototype.getValue = function () {
        return this.value;
    };

    VTextBoxButton.prototype.getMultipleIds = function () {
        return this.infoMultipleIds;
    };

    VTextBoxButton.prototype.getDisplay = function () {
        var retValue = "";
        if (this.lookup == null)
            retValue = this.value;
        else
            retValue = this.lookup.getDisplay(this.value);
        return retValue;
    };

    // Added by mohit - To set the custom info window needed to be opened while creating search control.
    VTextBoxButton.prototype.setCustomInfo = function (InfoWindow) {
        if (InfoWindow != null) {
            this.custInfoWin = InfoWindow;
        }
    }


    //7. 
    function VTextArea(columnName, isMandatory, isReadOnly, isUpdateable, displayLength, fieldLength, displayType) {

        var rows = 6;
        var minHeight = 79;
        if (displayType != VIS.DisplayType.Memo) {
            if (displayType == VIS.DisplayType.TextLong) {
                rows = 6;
                fieldLength = 100000;
            }
            else {
                rows = fieldLength < 300 ? 2 : (fieldLength < 2000) ? 4 : 6;
                minHeight = fieldLength < 300 ? 79 : (fieldLength < 2000) ? 129 : 129;
            }
        }
        else {
            try {
                rows = fieldLength < 300 ? 2 : (fieldLength < 2000) ? 4 : (fieldLength / 500);
                minHeight = fieldLength < 300 ? 79 : (fieldLength < 2000) ? 129 : 129;
            }
            catch (e) {
                rows = fieldLength < 300 ? 2 : (fieldLength < 2000) ? 4 : 6;
                minHeight = fieldLength < 300 ? 79 : (fieldLength < 2000) ? 129 : 129;
            }
        }


        //Init Control
        var $ctrl = $('<textarea>', { name: columnName, maxlength: fieldLength, rows: rows, 'style': 'min-height:' + minHeight + 'px' });
        //Call base class
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
            //this.Enabled = false;
        }
        else {
            this.setReadOnly(false);
        }

        var self = this; //self pointer

        /* Event */
        $ctrl.on("change", function (e) {
            e.stopPropagation();
            var newVal = $ctrl.val();
            if (newVal !== self.oldValue) {
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
            }
        });

        this.disposeComponent = function () {
            $ctrl.off("change"); //u bind event
            $ctrl = null;
            self = null;
        }
    };

    VIS.Utility.inheritPrototype(VTextArea, IControl);//inherit IControl
    /** 
    *  set value 
    *  @param new value to set
    */
    VTextArea.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            //console.log(newValue);
            this.ctrl.val(newValue);
            //this.setBackground("white");
        }
    };
    /** 
     *  get display text of control
     *  @return text of control
     */
    VTextArea.prototype.getDisplay = function () {
        return this.ctrl.val();
    };

    /*************HTML 5 Amount,Integer,Number Txtbox Block Code By raghu 06-05-2014****************************************/

    //8.
    /**
    * Create VAmountTextBox text box Control
    *  @param columnName column name
    *  @param mandatory mandatory
    *  @param isReadOnly read only
    *  @param isUpdateable updateable
    *  @param displayLength textbox lenght
    *  @param fieldLength column lenght
    *  @param title title
    */
    function VAmountTextBox(columnName, isMandatory, isReadOnly, isUpdateable, displayLength, fieldLength, controlDisplayType, title) {

        var displayType = controlDisplayType;
        var length = fieldLength;

        //Init Control

        var $ctrl = $('<input>', { type: 'number', step: 'any', name: columnName, maxlength: 16, 'data-type': 'int' });
        $ctrl.attr('autocomplete', 'off');


        //Call base class
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);
        //Set Fration,min,max value for control according to there dispay type
        this.format = VIS.DisplayType.GetNumberFormat(displayType);

        // Define Formatter type
        //this.dotFormatter = VIS.Env.isDecimalPoint();

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
        }
        else {
            this.setReadOnly(false);
        }

        var self = this; //self pointer

        // Assign Value
        this.dotFormatter = VIS.Env.isDecimalPoint();

        // For testing purpose
        //this.dotFormatter = true;

        //On key down event
        $ctrl.on("keydown", function (event) {

            if (event.keyCode == 189 || event.keyCode == 109 || event.keyCode == 173) { // dash (-)
                if (event.keyCode == 189 && this.value.length == 0) {
                    return true;
                }



                var val = self.format.GetConvertedNumber(this.value, self.dotFormatter);

                this.value = Number(val) * -1;
                setTimeout(function () {
                    $ctrl.trigger("change");
                }, 100);
                return false;
            }

            //if (event.shiftKey) {
            //    return false;
            //}

            if ((event.keyCode >= 37 && event.keyCode <= 40) || // Left, Up, Right and Down        
                event.keyCode == 8 || // backspaceASKII
                event.keyCode == 9 || // tabASKII
                event.keyCode == 16 || // shift
                event.keyCode == 17 || // control
                event.keyCode == 35 || // End
                event.keyCode == 36 || // Home
                event.keyCode == 46) // deleteASKII
            {
                return true;
            }
            // 0-9 numbers (the numeric keys at the right of the keyboard)
            if ((event.keyCode >= 48 && event.keyCode <= 57 && event.shiftKey == false) || (event.keyCode >= 96 && event.keyCode <= 105 && event.shiftKey == false)) {
                if (this.value.length >= length) {
                    return false;
                }
                return true;
            }
            else if (event.keyCode == 189 || event.keyCode == 109 || event.keyCode == 173) { // dash (-)
                this.value = this.value * -1;
                return false;
            }

            // Get culture decimal separator
            // if (window.navigator.language != undefined) {
            //    var culture = new VIS.CultureSeparator();
            // var isDotSeparator = VIS.Env.isDecimalPoint();// culture.isDecimalSeparatorDot(window.navigator.language);

            // Not . decimal separator
            //console.log(event.keyCode, " >keyCode ", VIS.Env.isDecimalPoint());

            //if (!VIS.Env.isDecimalPoint()) {
            if (!self.dotFormatter) {

                if (event.keyCode == 190 || event.keyCode == 110) {
                    return false;
                }
            }
            // . separator
            else {
                if (event.keyCode == 188) {
                    // for ","
                    return false;
                }
            }

            // }

            if (event.keyCode == 190 || event.keyCode == 110 || event.keyCode == 188) {// decimal (.)
                if (this.value.indexOf('.') > -1) {
                    this.value = this.value.replace('.', '');
                }
                // For mulpile , separator
                else if (this.value.indexOf(',') > -1) {
                    this.value = this.value.replace(',', '');
                }


                if (this.value.length >= length) {
                    return false;
                }
                return true;
            }

            // Select All (CTRL + A)
            // console.log(event.ctrlKey, " >> ", event.keyCode)
            if (event.ctrlKey && event.keyCode == 65) {
                if (this.value.length > 0) {
                    event.stopPropagation();
                    $ctrl.select();
                    return true;
                }
                // Copy (CTRL +C)
            } else if (event.ctrlKey && event.keyCode == 67) {
                if (this.value.length > 0) {
                    event.stopPropagation();
                    return true;
                }
                // Paster (CTRL+V)
            } else if (event.ctrlKey && event.keyCode == 86) {
                setTimeout(function () {
                    event.stopPropagation();
                    var _value = event.target.value;
                    event.target.value = _value ? self.format.GetConvertedString(_value, self.dotFormatter) : '';
                }, 10);
                return true;
                // CUT (CTRL+X)
            } else if (event.ctrlKey && event.keyCode == 88) {
                event.stopPropagation();
                return true;
            }

            /* Check Only for . and , */
            //if (event.keyCode == 188) {
            //   return false;
            //}
            //else {
            return false;
            //}
        });


        //If user click in amount control, select all amount present in control
        $ctrl.on("focus", function (e) {
            e.stopPropagation();
            // Change Amount Display Type
            var _value = e.target.value;
            $ctrl.attr("type", "text");


            e.target.value = _value ? self.format.GetConvertedString(_value, self.dotFormatter) : '';

            if (VIS.DisplayType.Amount == displayType) {
                $ctrl.select();
            }
        });


        //text change Event
        $ctrl.on("change", function (e) {
            e.stopPropagation();
            // var newVal = $ctrl.val();

            var newVal = self.getValue();          
            this.value = newVal;

            if (newVal !== self.oldValue) {
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
            }
        });

        $ctrl.on("blur", function (e) {
            e.stopPropagation();
            $ctrl.attr("type", "text");
            var val = $ctrl.val()
            if (!self.dotFormatter) {
                val = val.replace(".", ",");
            }


            var _value = self.format.GetConvertedString(val, self.dotFormatter);

            var _val = self.format.GetFormatAmount(_value, "formatOnly", self.dotFormatter);
            $ctrl.val(_val);

        });

        this.disposeComponent = function () {
            $ctrl.off("keydown"); //u bind event
            $ctrl.off("change"); //u bind event

            $ctrl = null;
            self = null;
            this.format.dispose();
            this.format = null;
            length = null;
        }
    };

    VIS.Utility.inheritPrototype(VAmountTextBox, IControl);//Inherit from IControl

    // set value   @param new value to set
    VAmountTextBox.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            newValue = this.format.GetFormatedValue(newValue);

            // this.ctrl.val(newValue);
            // this.setBackground("white");

            this.ctrl.attr("type", "text");
            var _value = this.format.GetFormatAmount(newValue, "init", this.dotFormatter);

            this.ctrl.val(_value);
        }
    };

    VAmountTextBox.prototype.getValue = function () {
        var val = this.$super.getValue.call(this);

        val = this.format.GetConvertedNumber(val, this.dotFormatter);

        if (isNaN(val) || val === null) {
            return null;
        }
        return Number(val);
    };

    //get display text of control  @return text of control
    VAmountTextBox.prototype.getDisplay = function () {
        return this.ctrl.val();
    };

    VAmountTextBox.prototype.setMaxValue = function (maxValue) {
        if ($.isNumeric(maxValue)) {
            this.ctrl.attr("max", maxValue);
        }
    };

    VAmountTextBox.prototype.setMinValue = function (minValue) {
        if ($.isNumeric(minValue)) {
            this.ctrl.attr("min", minValue);
        }
    };

    /***END VAmountTextBox***/

    //9. 
    /**
    * Create VNumTextBox Allow only Integer values (-,+) decimal not allowed
    *  @param columnName column name
    *  @param mandatory mandatory
    *  @param isReadOnly read only
    *  @param isUpdateable updateable
    *  @param displayLength textbox lenght
    *  @param fieldLength column lenght
    *  @param title title
    */
    function VNumTextBox(columnName, isMandatory, isReadOnly, isUpdateable, displayLength, fieldLength, title) {

        var displayType = VIS.DisplayType.Integer;
        var length = fieldLength;
        //Init Control
        var $ctrl = $('<input>', { type: 'text', name: columnName, maxlength: length, 'data-type': 'int' });

        //Call base class
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);
        //Set Fration,min,max value for control according to there dispay type
        this.format = VIS.DisplayType.GetNumberFormat(displayType);

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
            //this.Enabled = false;
        }
        else {
            this.setReadOnly(false);
        }
        var self = this; //self pointer
        //$ctrl.addClass("vis-control-wrap-int-amount");

        //On key down event
        $ctrl.on("keydown", function (event) {
            if (event.keyCode == 189 || event.keyCode == 109 || event.keyCode == 173) { // dash (-)
                if (event.keyCode == 189 && this.value.length == 0) {
                    return true;
                }
                this.value = Number(this.value * -1);
                setTimeout(function () {
                    $ctrl.trigger("change");
                }, 100);
                return false;
            }

            //if (event.shiftKey) {
            //    return false;
            //}

            if ((event.keyCode >= 37 && event.keyCode <= 40) || // Left, Up, Right and Down        
                event.keyCode == 8 || // backspaceASKII
                event.keyCode == 9 || // tabASKII
                event.keyCode == 16 || // shift
                event.keyCode == 17 || // control
                event.keyCode == 35 || // End
                event.keyCode == 36 || // Home
                event.keyCode == 46) // deleteASKII
            {
                return true;
            }
            if (this.value < 0) {
                if (this.value.length > VIS.DisplayType.INTEGER_DIGITS && event.keyCode != 8 && event.keyCode != 9 && event.keyCode != 46) {
                    return false;
                }
            }
            else {
                if (this.value.length >= VIS.DisplayType.INTEGER_DIGITS && event.keyCode != 8 && event.keyCode != 9 && event.keyCode != 46) {
                    return false;
                }
            }
            // 0-9 numbers (the numeric keys at the right of the keyboard)
            if ((event.keyCode >= 48 && event.keyCode <= 57 && event.shiftKey == false) || (event.keyCode >= 96 && event.keyCode <= 105 && event.shiftKey == false)) {
                if (this.value.length >= length) {
                    return false;
                }
                return true;
            }
            else {
                return false;
            }

        });

        // text change Event 
        $ctrl.on("change", function (e) {
            e.stopPropagation();
            //var newVal = $ctrl.val();    
            var newVal = self.getValue();
            newVal = Globalize.parseInt(newVal.toString());
            //alert(newVal);
            var newFormatedVal = Number(self.format.GetFormatedValue(newVal));
            if (newVal != newFormatedVal) {
                self.mField.setValue(null);
                newVal = newFormatedVal;
            }

            if (newVal !== self.oldValue) {
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
            }
        });

        this.disposeComponent = function () {
            $ctrl.off("keydown"); //u bind event
            $ctrl.off("change"); //u bind event
            $ctrl = null;
            self = null;
            this.format.dispose();
            this.format = null;
            length = null;
        }
    };

    //Inherit from IControl
    VIS.Utility.inheritPrototype(VNumTextBox, IControl);

    // set value   @param new value to set
    VNumTextBox.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            //console.log(newValue);
            //newValue = Number(this.format.GetFormatedValue(newValue));
            // newValue = Globalize.format(newValue, "n0");
            this.ctrl.val(newValue);
            //this.setBackground("white");
        }
    };

    VNumTextBox.prototype.getValue = function () {
        var val = this.$super.getValue.call(this);
        if (val === null) {
            return null;
        }

        if (this.editingGrid) {
            val = this.format.GetFormatedValue(val);
        }
        //return Number(val);
        return val;
    };

    //get display text of control @return text of control
    VNumTextBox.prototype.getDisplay = function () {
        return this.ctrl.val();
    };

        /***END VNumTextBox***/



    //10. Location

    /**
     *	Create lookup for location field
     *  @param columnName column name
     *  @param mandatory mandatory
     *  @param isReadOnly read only
     *  @param isUpdateable updateable
     *  @param displayType display type
     *  @param title title
     */

    function VLocation(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup) {
        if (!displayType) {
            displayType = VIS.DisplayType.Location;
        }

        this.lookup = lookup;
        this.lastDisplay = "";
        this.settingFocus = false;
        this.inserting = false;
        this.settingValue = false;

        this.value = null;

        //create ui
        var $ctrl = $('<input readonly>', { type: 'text', name: columnName });
        var $btnMap = $('<button class="input-group-text"><i class="vis vis-location" aria-hidden="true"></i></button>');
        var $btnLocation = $('<button class="input-group-text"><i class="vis vis-card" aria-hidden="true"></i></button>');
        var btnCount = 2;
        //$ctrl.append($btnMap).append($btnLocation);
        var self = this;
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory); //call base function

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
        }
        else {

            this.setReadOnly(false);
        }

        this.getBtn = function (index) {
            if (index == 0) {
                return $btnMap;
            }
            if (index == 1) {
                return $btnLocation;
            }
        };

        this.getBtnCount = function () {
            return btnCount;
        };

        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                $ctrl.show();
                $btnMap.show();
                $btnLocation.show();

            } else {
                $ctrl.hide();
                $btnMap.hide();
                $btnLocation.hide();
            }
        };

        this.setReadOnly = function (readOnly) {

            this.isReadOnly = readOnly;
            $ctrl.prop('disabled', readOnly ? true : false);
            if (readOnly) {
                $btnLocation.css("opacity", .7);
            } else {
                $btnLocation.css("opacity", 1);
            }
            this.setBackground(false);
        };

        $btnMap.on(VIS.Events.onClick, function (e) {
            var url = "http://local.google.com/maps?q=" + self.getDisplay();
            window.open(url);
            e.stopPropagation();
        });

        $btnLocation.on(VIS.Events.onClick, function (e) {
            e.stopPropagation();
            if (self.isReadOnly) {
                return;
            }
            // passed new parameter for Maintain Versions in case of Location Control
            var maintainVers = null;
            if (self.mField && self.mField.vo)
                maintainVers = self.mField.vo.IsMaintainVersions;
            var obj = new VIS.LocationForm(self.value, maintainVers);
            obj.load();
            obj.showDialog();
            obj.onClose = function (location, change) {
                //if (self.oldValue != location)
                {
                    if (change) {
                        self.oldValue = 0;
                        self.lookup.refreshLocation(location);

                        self.setValue(location);
                        var evt = { newValue: location, propertyName: self.getColumnName() };
                        self.fireValueChanged(evt);
                        evt = null;
                    }
                }
            };
            obj = null;
            //alert("Map button [" + self.value + "] => " + self.getName());
        });

        //dispose 
        this.disposeComponent = function () {
            $btnMap.off(VIS.Events.onClick);
            $btnLocation.off(VIS.Events.onClick);
            self = null;
            $ctrl = null;
            $btnMap = null;
            $btnLocation = null;
            this.setVisible = null;
            //this.lookup = null;
            //this.lastDisplay = "";
            //this.settingFocus = false;
            //this.inserting = false;
            //this.settingValue = false;
        };
    };

    VIS.Utility.inheritPrototype(VLocation, IControl);//inherit IControl

    VLocation.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.settingValue = true;
            this.oldValue = newValue;
            this.value = newValue

            //	Set comboValue
            if (newValue == null) {
                this.lastDisplay = "";
                this.ctrl.val("");
                this.settingValue = false;
                return;
            }
            if (this.lookup == null) {
                this.ctrl.val(newValue.toString());
                this.lastDisplay = newValue.toString();
                this.settingValue = false;
                return;
            }

            this.lastDisplay = this.lookup.getDisplay(newValue);
            if (this.lastDisplay.equals("<-1>")) {
                this.lastDisplay = "";
                this.oldValue = null;
                this.value = null;
            }
            this.value = newValue;
            //this.ctrl.val(this.lastDisplay);
            this.ctrl.val(VIS.Utility.decodeText(this.lastDisplay));
            this.settingValue = true;

        }
    };

    VLocation.prototype.getValue = function () {
        return this.value;
    };

    VLocation.prototype.getDisplay = function () {
        var retValue = "";
        if (this.lookup == null)
            retValue = this.value;
        else
            retValue = this.lookup.getDisplay(this.value);
        return retValue;
    };

    //END

    //11. Locator

    /**
     *	Create lookup for locator field
     *  @param columnName column name
     *  @param mandatory mandatory
     *  @param isReadOnly read only
     *  @param isUpdateable updateable
     *  @param displayType display type
     *  @param title title
     */
    function VLocator(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup) {

        this.mandatory = isMandatory;
        this.onlyWarehouseId = 0;
        this.onlyProductId = 0;
        this.onlyOutgoing = null;
        this.windowNum = lookup.getWindowNo();
        this.columnName = columnName;
        this.lookup = lookup;

        var src = "vis vis-locator";//VIS.Application.contextUrl + "Areas/VIS/Images/base/Locator20.png";
        this.value = null;

        if (!displayType)
            displayType = VIS.DisplayType.Locator;


        //create ui
        var $ctrl = $('<input readonly>', { type: 'text', name: columnName });
        var $btn = $('<button class="input-group-text"><i class="' + src + '" aria-hidden="true"></i></button>');
        var $btnZoom = $('<button class="input-group-text"><i class="vis vis-find" aria-hidden="true"></i></button>');
        var btnCount = 2;

        var self = this;
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);
        // @overridde
        this.getBtnCount = function () {
            return btnCount;
        };

        // get contols button by index 
        this.getBtn = function (index) {
            if (index == 0) {
                return $btn;
            }
            if (index == 1) { //zoom
                return $btnZoom;
            }
        };
        //show visivility
        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                $ctrl.show();
                $btn.show();
                $btnZoom.show();

            } else {

                $btn.hide();
                $btnZoom.hide();
                $ctrl.hide();
            }
        };


        this.setReadOnly = function (readOnly) {

            this.isReadOnly = readOnly;
            $ctrl.prop('disabled', readOnly ? true : false);
            if (readOnly) {
                $btn.css("opacity", .7);
            } else {
                $btn.css("opacity", 1);
            }
            this.setBackground(false);
        };


        $btn.on(VIS.Events.onClick, function (e) {
            e.stopPropagation();
            if (self.isReadOnly) {
                return;
            }
            //	Warehouse/Product
            var warehouseId = self.getOnlyWarehouseID();
            var productId = self.getOnlyProductID();

            self.showLocatorForm(warehouseId, productId);

        });

        $btnZoom.on(VIS.Events.onClick, function (e) {
            e.stopPropagation();
            if (!self.lookup)
                return;
            //
            var zoomQuery = self.lookup.getZoomQuery();
            var value = self.getValue();
            if (value == null) {
                //   value = selectedItem;
            }
            if (value == "")
                value = null;
            //	If not already exist or exact value
            if ((zoomQuery == null) || (value != null)) {
                zoomQuery = new VIS.Query();	//	ColumnName might be changed in MTab.validateQuery

                var keyColumnName = null;
                //	Check if it is a Table Reference
                if ((self.lookup != null) && (self.lookup instanceof VIS.MLookup)) {
                    var AD_Reference_ID = self.lookup.getAD_Reference_Value_ID();
                    if (AD_Reference_ID != 0) {
                        var query = "VIS_91";
                        var param = [];
                        param[0] = new VIS.DB.SqlParam("@AD_Reference_ID", AD_Reference_ID);
                        try {
                            var dr = executeReader(query, param);
                            if (dr.read()) {
                                keyColumnName = dr.getString(0);
                            }
                            dr.dispose();
                        }
                        catch (e) {
                            this.log.log(VIS.Logging.Level.SEVERE, query, e);
                        }
                    }	//	Table Reference
                }	//	MLookup

                if ((keyColumnName != null) && (keyColumnName.length != 0))
                    zoomQuery.addRestriction(keyColumnName, VIS.Query.prototype.EQUAL, value);
                else
                    zoomQuery.addRestriction(self.getColumnName(), VIS.Query.prototype.EQUAL, value);
                zoomQuery.setRecordCount(1);	//	guess
            }

            var AD_Window_ID = 0;
            if (self.mField.getZoomWindow_ID() > 0) {
                AD_Window_ID = self.mField.getZoomWindow_ID();
            }
            else {
                AD_Window_ID = self.lookup.getZoomWindow(zoomQuery);
            }
            //
            //this.log.info(this.getColumnName() + " - AD_Window_ID=" + AD_Window_ID
            //    + " - Query=" + zoomQuery + " - Value=" + value);
            //
            //setCursor(Cursor.getPredefinedCursor(Cursor.WAIT_CURSOR));
            //
            VIS.viewManager.startWindow(AD_Window_ID, zoomQuery);
            //setCursor(Cursor.getDefaultCursor());

        });

        //dispose
        this.disposeComponent = function () {
            $btn.off(VIS.Events.onClick);
            $btnZoom.off(VIS.Events.onClick);
            self = null;
            $ctrl = null;
            $btn = null;
            this.getBtn = null;
            this.setVisible = null;
        };
    };

    VIS.Utility.inheritPrototype(VLocator, IControl);

    VLocator.prototype.getOnlyWarehouseID = function () {
        var ctx = VIS.Env.getCtx();
        // gwu: do not restrict locators by warehouse when in Import Inventory Transactions window 
        var AD_Table_ID = ctx.getContext(this.windowNum, "0|AD_Table_ID", true);
        // Import Inventory Transactions
        if ("572" == AD_Table_ID) {
            return 0;
        }
        var only_Warehouse = ctx.getContext(this.windowNum, "M_Warehouse_ID", true);
        var only_Warehouse_ID = 0;
        try {
            if (only_Warehouse != null && only_Warehouse.length > 0) {
                only_Warehouse_ID = Number(only_Warehouse);
            }
        }
        catch (ex) {
            // log.Log(Logging.Level.SEVERE, ex.Message);
        }
        return only_Warehouse_ID;
    };

    VLocator.prototype.getOnlyProductID = function () {

        var ctx = VIS.Env.getCtx();
        // gwu: do not restrict locators by product when in Import Inventory Transactions window 
        var AD_Table_ID = ctx.getContext(this.windowNum, "0|AD_Table_ID", true);
        // Import Inventory Transactions
        if ("572" == AD_Table_ID) {
            return 0;
        }

        var only_Product = ctx.getContext(this.windowNum, "M_Product_ID", true);
        var only_Product_ID = 0;
        try {
            if (only_Product != null && only_Product.length > 0) {
                only_Product_ID = Number(only_Product);
            }
        }
        catch (ex) {
            //log.Log(Logging.Level.SEVERE, ex.Message);
        }
        return only_Product_ID;
    };

    //Function which show form
    VLocator.prototype.showLocatorForm = function (warehouseId, productId) {
        var M_Locator_ID = 0;
        if (this.value != null) {
            M_Locator_ID = Number(this.value);
        }

        this.lookup.setOnlyWarehouseID(warehouseId);
        this.lookup.setOnlyProductID(productId);

        var isReturnTrx = "Y".equals(VIS.context.getWindowContext(this.windowNum, "IsReturnTrx"))
        var isSOTrx = VIS.Env.getCtx().isSOTrx(this.windowNum);

        var isOnlyOutgoing = ((isSOTrx && !isReturnTrx) || (!isSOTrx && isReturnTrx)) && this.columnName == "M_Locator_ID";
        this.lookup.setOnlyOutgoing(isOnlyOutgoing);
        this.lookup.refresh();

        //Open locator form
        self = this;
        var obj = new VIS.LocatorForm(this.columnName, this.lookup, M_Locator_ID, this.mandatory, warehouseId, this.windowNum);
        obj.load();
        obj.showDialog();
        obj.onClose = function (locator) {
            if (self.oldValue != locator) {
                self.setValue(locator);
                var evt = { newValue: locator, propertyName: self.columnName };
                self.fireValueChanged(evt);
                evt = null;
            }
        };
    };

    VLocator.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.settingValue = true;
            this.oldValue = newValue;
            this.value = newValue
            //	Set comboValue
            if (newValue == null) {
                this.lastDisplay = "";
                this.ctrl.val("");
                this.settingValue = false;
                return;
            }
            if (this.lookup == null) {
                this.ctrl.val(newValue.toString());
                this.lastDisplay = newValue.toString();
                this.settingValue = false;
                return;
            }

            this.lastDisplay = this.lookup.getDisplay(newValue);
            if (this.lastDisplay.equals("<-1>")) {
                this.lastDisplay = "";
                this.oldValue = null;
                this.value = null;
            }
            //console.log(newValue);

            this.value = newValue;
            this.ctrl.val(this.lastDisplay);

            this.settingValue = true;
            //this.setBackground("white");
        }
    };

    VLocator.prototype.getValue = function () {
        return this.value;
    };

    VLocator.prototype.getDisplay = function () {
        var retValue = "";
        if (this.lookup == null)
            retValue = this.value;
        else
            retValue = this.lookup.getDisplay(this.value);
        return retValue;
    };
    //END

    //pAttribute control
    function VPAttribute(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup, windowNop, isActivityForm, search, fromDMS, pAttribute, tabNo) {

        //Variable region
        /**	No Instance Key					*/
        this.NO_INSTANCE = 0;
        /**	Calling Window Info	*/
        this.AD_Column_ID = 0;
        var colName = "M_AttributeSetInstance_ID";
        var focus = false;

        //For genral attribute variable settings
        this.C_GenAttributeSet_ID = 0;
        this.C_GenAttributeSetInstance_ID = 0;
        this.M_Locator_ID = 0;
        colName = columnName;
        this.value = null;

        this.windowNo = windowNop;
        this.tabNo = tabNo;
        //set lookup into current object from pttribute/gattribute lookup
        this.lookup = lookup;

        this.C_BPartner_ID = VIS.Env.getCtx().getContextAsInt(this.windowNo, "C_BPartner_ID");


        /**	Logger			*/
        this.log = VIS.Logging.VLogger.getVLogger("VPAttribute");

        var src = "vis vis-pattribute";// VIS.Application.contextUrl + "Areas/VIS/Images/base/PAttribute20.png";
        if (!displayType) {
            displayType = VIS.DisplayType.PAttribute;
        }

        //Genral Attribute parameter

        this.isFromActivityForm = isActivityForm;
        this.isSearch = search;
        this.isFromDMS = fromDMS;
        this.isPAttribute = pAttribute;
        this.canSaveRecord = true;


        //create ui
        var $ctrl = $('<input>', { type: 'text', name: columnName });
        var $btn = $('<button class="input-group-text"><i class="' + src + '"></i></button>');
        var btnCount = 1;

        var self = this;

        this.setInstanceIDs = null;

        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);

        //read only control
        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
        }
        else {

            this.setReadOnly(false);
        }


        // @overridde
        this.getBtnCount = function () {
            return btnCount;
        };

        // get contols button by index 
        this.getBtn = function (index) {
            if (index == 0) {
                return $btn;
            }

        };
        //show visivility
        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                $ctrl.show();
                $btn.show();

            } else {

                $btn.hide();
                $ctrl.hide();
            }
        };


        this.setReadOnly = function (readOnly) {
            this.isReadOnly = readOnly;
            $ctrl.prop('disabled', readOnly ? true : false);
            if (readOnly) {
                $btn.css("opacity", .7);
            } else {
                $btn.css("opacity", 1);
            }
            this.setBackground(false);
        };

        //Open Genral attribute Dialog form
        function OpenGeneralAttributeDialog(VADMS_AttributeSet_ID, oldValue) {
            var valueChange = false;
            var C_GenAttributeSetInstance_IDWin = VIS.Env.getCtx().getContextAsInt(self.windowNo, "C_GenAttributeSetInstance_ID");
            if (self.isFromActivityForm) {
                C_GenAttributeSetInstance_IDWin = self.C_GenAttributeSetInstance_ID;
            }

            if (C_GenAttributeSetInstance_IDWin == 0) {
                //txtAttribute.Text = string.Empty;
            }

            var obj = new VIS.GenralAttributeForm(C_GenAttributeSetInstance_IDWin, VADMS_AttributeSet_ID, self.windowNo, self.isSearch, self.getCanSaveRecord(), self.isFromDMS);
            obj.showDialog();
            obj.onClose = function (mGenAttributeSetInstanceId, name, instanceIDs) {
                VIS.Env.getCtx().setContext(windowNop, "C_GenAttributeSetInstance_ID", mGenAttributeSetInstanceId);
                setValueInControl(mGenAttributeSetInstanceId, name);
                if (instanceIDs != null)
                    self.setInstanceIDs = instanceIDs;
                self.getControl().val(name);
            };
        }

        function setValueInControl(mAttributeSetInstanceId, name) {
            self.log.finest("Changed M_AttributeSetInstance_ID=" + mAttributeSetInstanceId);
            if (self.oldValue != mAttributeSetInstanceId) {
                if (mAttributeSetInstanceId == 0) {
                    self.setValue(null);
                }
                self.setValue(mAttributeSetInstanceId);
                var evt = { newValue: mAttributeSetInstanceId, propertyName: self.colName };
                self.fireValueChanged(evt);
                evt = null;
            }
        }

        //Open PAttribute form
        function OpenPAttributeDialog(oldValue) {
            var M_AttributeSetInstance_ID = (oldValue == null) ? 0 : oldValue;
            var M_Product_ID = VIS.Env.getCtx().getTabRecordContext(windowNop, tabNo, "M_Product_ID");
            var M_ProductBOM_ID = VIS.context.getTabRecordContext(windowNop, tabNo, "M_ProductBOM_ID");
            var M_Locator_ID = VIS.Env.getCtx().getContextAsInt(windowNop, "M_Locator_ID");
            self.log.config("M_Product_ID=" + M_Product_ID + "/" + M_ProductBOM_ID + ",M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + ", AD_Column_ID=" + self.AD_Column_ID);
            var productWindow = self.AD_Column_ID == 8418;		//	HARDCODED

            //	Exclude ability to enter ASI
            var exclude = true;
            var changed = false;

            if (M_ProductBOM_ID != 0)	//	Use BOM Component
            {
                M_Product_ID = M_ProductBOM_ID;
            }

            if (M_Product_ID != 0) {
                //call controller of pAttributeForm to get is value should include or exclude
                $.ajax({
                    url: VIS.Application.contextUrl + "PAttributes/ExcludeEntry",
                    dataType: "json",
                    async: false,
                    data: {
                        productId: M_Product_ID,
                        adColumn: self.AD_Column_ID,
                        windowNo: windowNop,
                    },
                    success: function (data) {
                        exclude = data.result;
                    }
                });
            }



            if (!productWindow && (M_Product_ID == 0 || exclude)) {
                changed = true;
                M_AttributeSetInstance_ID = 0;
                setValueInControl(M_AttributeSetInstance_ID);

                // JID_0898: Need to show the Info message when no attribute set is defined on product or having exclude entry for this table.
                VIS.ADialog.info("VIS_PAttributeNotFound", null, null, null);
            }
            else {
                var obj = new VIS.PAttributesForm(M_AttributeSetInstance_ID, M_Product_ID, M_Locator_ID, self.C_BPartner_ID, productWindow, self.AD_Column_ID, windowNop);
                if (obj.hasAttribute) {
                    obj.showDialog();
                }
                obj.onClose = function (mAttributeSetInstanceId, name, mLocatorId) {
                    this.M_Locator_ID = mLocatorId;
                    setValueInControl(mAttributeSetInstanceId, name);
                };
            }
        }

        $btn.on(VIS.Events.onClick, function (e) {
            e.stopPropagation();
            if (self.isReadOnly) {
                return;
            }
            var oldValue = self.getValue();
            //Genral Attribute Logic
            if (!self.isPAttribute) {
                var genAttributeSetId = 0;
                genAttributeSetId = VIS.Env.getCtx().getContextAsInt(self.windowNo, "C_GenAttributeSet_ID");

                if (genAttributeSetId == 0) {
                    genAttributeSetId = self.C_GenAttributeSet_ID;
                    if (genAttributeSetId == 0) {
                        VIS.ADialog.info("NoAttributeSet", null, null, null);
                        return;
                    }
                }
                OpenGeneralAttributeDialog(genAttributeSetId, oldValue);
                return;
            }
            //PAttribute logic for open form
            OpenPAttributeDialog(oldValue);
        });

        this.SetC_GenAttributeSet_ID = function (instanceID) {
            this.C_GenAttributeSet_ID = instanceID;
        }

        //dispose
        this.disposeComponent = function () {
            $btn.off(VIS.Events.onClick);
            self = null;
            $ctrl = null;
            $btn = null;
            this.getBtn = null;
            this.setVisible = null;
            this.value = null;
            this.lookup = null;
        }
    };

    VIS.Utility.inheritPrototype(VPAttribute, IControl);

    /**************************************************************************
      * Set/lookup Value
      * 
      * @param value
      *            value
      */
    VPAttribute.prototype.setValue = function (value) {
        if (value == null || 0 === value) {
            this.ctrl.text("");
            this.value = value;
        }
        // The same
        //if (value === this.value)
        //    return;
        this.value = value;
        this.ctrl.val(this.lookup.getDisplay(value)); // loads value
    }; // setValue

    VPAttribute.prototype.setField = function (mField) {
        if (mField != null) {
            this.windowNo = mField.getWindowNo();
            this.AD_Column_ID = mField.getAD_Column_ID();
        }
        this.mField = mField;
    }

    /**
     * Get Value
     * 
     * @return value
     */
    VPAttribute.prototype.getValue = function () {
        return this.value;
    }; // getValue

    VPAttribute.prototype.genSetInstanceIDs = function () {
        return this.setInstanceIDs;
    };


    /**
     * Get Display Value
     * 
     * @return info
     */
    VPAttribute.prototype.getDisplay = function () {
        return this.ctrl.val();
    }; // getDisplay

    VPAttribute.prototype.getCanSaveRecord = function () {
        return this.canSaveRecord;
    }; // getCanSaveRecord

    VPAttribute.prototype.setCanSaveRecord = function (value) {
        return this.canSaveRecord = value;
    };


    //End


    //Account
    function VAccount(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup, windowNo, title) {

        this.value = null;
        this.windowNo = windowNo;
        this.lookup = lookup;
        this.title = title;
        var colName = columnName;
        var src = "vis vis-account";// VIS.Application.contextUrl + "Areas/VIS/Images/base/Account20.png";
        if (!displayType)
            displayType = VIS.DisplayType.PAttribute;


        //create ui
        var $ctrl = $('<input readonly>', { type: 'text', name: columnName });
        var $btn = $('<button class="input-group-text"><i class="' + src + '" ></i></button>');
        var btnCount = 1;

        var self = this;
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);

        // @overridde
        this.getBtnCount = function () {
            return btnCount;
        };

        // get contols button by index 
        this.getBtn = function (index) {
            if (index == 0) {
                return $btn;
            }
        };
        //show visivility
        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                $ctrl.show();
                $btn.show();

            } else {

                $ctrl.hide();
                $btn.hide();
            }
        };


        this.setReadOnly = function (readOnly) {
            this.isReadOnly = readOnly;
            $ctrl.prop('disabled', readOnly ? true : false);
            if (readOnly) {
                $btn.css("opacity", .7);
            } else {
                $btn.css("opacity", 1);
            }
            this.setBackground(false);
        };


        $btn.on(VIS.Events.onClick, function (e) {
            e.stopPropagation();
            if (self.isReadOnly) {
                return;
            }
            var C_AcctSchema_ID = VIS.Env.getCtx().getContextAsInt(self.windowNo, "C_AcctSchema_ID");


            var getTbID_s = 0;

            if (self != null && self.getField() != null && self.getField().vo != null && self.getField().vo.AD_Table_ID != null) {
                getTbID_s = self.getField().vo.AD_Table_ID;
            }

            var obj = new VIS.AccountForm(self.title, self.lookup, C_AcctSchema_ID, getTbID_s);



            //var obj = new VIS.AccountForm(self.title, self.lookup, C_AcctSchema_ID);
            obj.load();
            obj.showDialog();
            obj.onClose = function (location) {
                if (self.oldValue != location) {
                    self.setValue(location);
                    var evt = { newValue: location, propertyName: colName };
                    self.fireValueChanged(evt);
                    evt = null;
                }
            };
            obj = null;
        });


        //dispose
        this.disposeComponent = function () {
            $btn.off(VIS.Events.onClick);
            self = null;
            $ctrl = null;
            $btn = null;
            this.getBtn = null;
            this.setVisible = null;
            this.value = null;
            this.lookup = null;
            this.value = null;
            this.windowNo = null;
            this.title = null;

        }
    };
    VIS.Utility.inheritPrototype(VAccount, IControl);
    /*
      * Set/lookup Value
      * 
      * @param value
      *            value
      */
    VAccount.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            this.value = newValue
            this.ctrl.val(this.lookup.getDisplay(newValue));	//	loads value
        }
    }; // setValue
    /**
     * Get Value
     * 
     * @return value
     */
    VAccount.prototype.getValue = function () {
        return this.value;
    }; // getValue
    /**
     * Get Display Value
     * 
     * @return info
     */
    VAccount.prototype.getDisplay = function () {
        return this.ctrl.val();
    }; // getDisplay
    //End




    //VImage
    function VImage(colName, mandatoryField, isReadOnly, winNo) {
        this.values = null;
        this.log = VIS.Logging.VLogger.getVLogger("VImage");
        var windowNo = winNo;
        var columnName = colName;// "AD_Image_ID";
        var $img = $("<img >");
        var $icon = $("<i>");
        var $txt = $("<span>").text("-");
        var $spanIcon = $('<span class="vis vis-edit vis-img-ctrl-icon">')
        var $ctrl = null;
        var dimension = "Thumb500x375";

        $ctrl = $('<button >', { type: 'button', name: columnName, class: 'vis-ev-col-img-ctrl', tabIndex: '-1' });
        $txt.css("color", "blue");




        $ctrl.append($spanIcon).append($img).append($icon).append($txt);

        IControl.call(this, $ctrl, VIS.DisplayType.Button, isReadOnly, columnName, mandatoryField);

        if (isReadOnly) {
            this.setReadOnly(true);
            //this.Enabled = false;
        }
        else {
            this.setReadOnly(false);
        }

        var self = this; //self pointer

        $spanIcon.on(VIS.Events.onClick, function (e) { //click handler
            e.stopPropagation();
            if (!self.isReadOnly) {
                //self.invokeActionPerformed({ source: self });
                var obj = new VIS.VImageForm(self.getValue(), $txt.text().trim().length);
                obj.showDialog();
                obj.onClose = function (ad_image_Id, change) {
                    //call set only when change call 
                    if (change) {
                        self.oldValue = -1;
                        if (self.oldValue != ad_image_Id) {
                            // 'null' in case of image delete 
                            ad_image_Id = (ad_image_Id == 'null' ? null : ad_image_Id);
                            self.setValue(ad_image_Id);
                            var evt = { newValue: ad_image_Id, propertyName: columnName };
                            self.fireValueChanged(evt, true);
                            evt = null;
                        }
                        else {
                            self.refreshImage(ad_image_Id);
                        }
                    }
                };
                obj = null;
            }
        });

        this.setText = function (text) {
            if (text == null) {
                $txt.text("");
                $img.image.src = '';
                return;
            }
            var pos = text.indexOf('&');
            if (pos != -1)					//	We have a nemonic - creates ALT-_
            {
                var mnemonic = text.toUpperCase().charAt(pos + 1);
                if (mnemonic != ' ') {
                    //setMnemonic(mnemonic);
                    text = text.substring(0, pos) + text.substring(pos + 1);
                }
            }
            $txt.text(text);
        };

        this.setDimension = function (height, width) {
            dimension = "Thumb" + width + "x" + height;
        };

        this.setIcon = function (resImg, imgPath) {
            //$img.attr('src', rootPath + img);

            if (imgPath) {
                $img.attr('src', VIS.Application.contextUrl + "Images/" + dimension + "/" + imgPath + "? timestamp =" + new Date().getTime());
                $img.show();
                $icon.hide();
                $txt.text("");
                this.ctrl.addClass('vis-input-wrap-button-image-add');
            }
            else if (resImg != null) {
                $img.attr('src', "data:image/jpg;base64," + resImg + "? timestamp =" + new Date().getTime());
                $img.show();
                $icon.hide();
                $txt.text("");
                this.ctrl.addClass('vis-input-wrap-button-image-add');
            }
            else {
                $img.attr('src', "data:image/jpg;base64," + resImg);
                $img.hide();
                $txt.text("-");
                this.ctrl.removeClass('vis-input-wrap-button-image-add');
            }
        };

        this.hideText = function () {
            $txt.hide();
        }
        /**
         * hide edit icon
         * */
        this.hideEditIcon = function () {
            $spanIcon.hide();
        };

        this.hideEditIcon = function () {
            $spanIcon.hide();
        };


        this.hideEditIcon = function () {
            $spanIcon.hide();
        };

        this.disposeComponent = function () {
            $ctrl.off(VIS.Events.onClick);
            $ctrl = null;
            this.log = null;
            windowNo = null;
            $img = null;
            $txt = null;
            this.setText = null;
            this.setIcon = null;
            self = null;
        };
    };

    VIS.Utility.inheritPrototype(VImage, IControl);//Inherit

    VImage.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {

            this.oldValue = newValue;
            this.value = newValue
            //	Set comboValue
            if (newValue == null) {
                this.setIcon(null);
                this.value = 0;
                if (this) {
                    this.ctrl.val(null);
                }
                return;
            }
            //var neValue = newValue;
            //  Get/Create Image byte array
            //var sql = "select * from AD_Image where AD_Image_ID=" + newValue;
            //var dr = VIS.DB.executeDataReader(sql.toString());
            //if (dr.read()) {
            //    var data = dr.getString("BINARYDATA");
            //    this.setIcon(data);
            //}

            //By Ajex request
            ////var localObj = this;
            ////$.ajax({
            ////    url: VIS.Application.contextUrl + "VImageForm/GetImageAsByte",
            ////    dataType: "json",
            ////    data: {
            ////        ad_image_id: neValue
            ////    },
            ////    success: function (data) {
            ////        var data = JSON.parse(data);
            ////        if (data) {
            ////            localObj.setIcon(data.Bytes, data.Path);
            ////        }
            ////        else {
            ////            localObj.setIcon(null, null);
            ////        }
            ////        localObj.ctrl.val(neValue);
            ////        localObj = null;
            ////    }
            ////});

            this.refreshImage(newValue);
        }
    };

    VImage.prototype.refreshImage = function (neValue) {
        var localObj = this;
        $.ajax({
            url: VIS.Application.contextUrl + "VImageForm/GetImageAsByte",
            dataType: "json",
            data: {
                ad_image_id: neValue
            },
            success: function (data) {
                var data = JSON.parse(data);
                if (data) {
                    localObj.setIcon(data.Bytes, data.Path);
                }
                else {
                    localObj.setIcon(null, null);
                }
                localObj.ctrl.val(neValue);
                localObj = null;
            }
        });
    };

    VImage.prototype.setReadOnly = function (readOnly) {
        this.isReadOnly = readOnly;
        if (this.isLink) {
            this.ctrl.css('opacity', readOnly ? .6 : 1);
        }
        else {
            this.ctrl.prop('disabled', readOnly ? true : false);
        }
        this.setBackground(false);
    };

    VImage.prototype.getValue = function () {
        return this.value;
    };

    VImage.prototype.getDisplay = function () {
        return this.value;
    };

    //End VImage 

    //VBinary
    function VBinary(colName, mandatoryField, isReadOnly, isUpdateable, winNo) {

        this.values = null;
        var $txt = $("<span>").text("");

        var windowNo = winNo;
        var columnName = colName;// "AD_Image_ID";

        var $ctrl = null;
        var $ulPopup = null;
        var SaveTolocalFile = 'SaveTolocalFile';
        var LoadIntoDatabase = 'LoadIntoDatabase';

        this.data = null;
        var inputCtrl = $("<input type='file' class='file' name='file'/>");

        $ctrl = $('<button>', { type: 'button', name: columnName });
        $ctrl.append($txt);

        IControl.call(this, $ctrl, VIS.DisplayType.Button, isReadOnly, columnName, mandatoryField);

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
        }
        else {
            this.setReadOnly(false);
        }

        var self = this; //self pointer

        function getPopupList() {
            var ullst = $("<ul class='vis-apanel-rb-ul'>");
            ullst.append($("<a id='link' style='border-bottom: 1px solid #CCDADE;color: #535353;font-size: 12px;display: block;margin-bottom: 8px;'  href='javascript:void(0)'>" + VIS.Msg.getMsg("SaveTolocalFile") + "</a>"));
            // ullst.append($("<li data-action='" + SaveTolocalFile + "'>").text(VIS.Msg.getMsg("SaveTolocalFile")));
            ullst.append($("<li data-action='" + LoadIntoDatabase + "'>").text(VIS.Msg.getMsg("Open/LoadIntoDatabase")));
            return ullst;
        };

        $ulPopup = getPopupList();

        if ($ulPopup) {
            $ulPopup.on("click", "LI", function (e) {

                var action = $(e.target).data("action");
                if (action == SaveTolocalFile) {
                    // onDownload(self.getValue());
                    //var d = new Date().toISOString().slice(0, 19).replace(/-/g, "");
                    //$("#link").attr("href", self.getValue()).attr("download", "file-" + d + ".log");
                }
                else if (action == LoadIntoDatabase) {
                    inputCtrl.trigger('click');
                }
            });

            $ulPopup.on("click", "a", function (e) {

                if (self.getValue() != null) {
                    var d = new Date().toISOString().slice(0, 19).replace(/-/g, "");
                    var fileData = "data:;base64," + self.getValue();
                    $(this).attr("href", fileData).attr("download", "file-" + d + ".log");
                }
            });
        }

        //Upload File on client side and get byte array from file on client side
        inputCtrl.change(function () {
            var file = inputCtrl[0].files[0];
            var reader = new FileReader();
            var ary = reader.readAsDataURL(file);

            reader.onloadend = function () {
                var base64data = (reader.result).split(',')[1];
                if (self.oldValue != base64data) {
                    self.setValue(base64data);
                    var evt = { newValue: base64data, propertyName: columnName };
                    self.fireValueChanged(evt, true);
                    evt = null;
                }
            }
            return;

            //get byte array from server
            //var xhr = new XMLHttpRequest();
            //var fd = new FormData();
            //fd.append("file", file);
            //xhr.open("POST", VIS.Application.contextUrl + "VImageForm/GetFileByteArray", true);
            //xhr.send(fd);
            //xhr.addEventListener("load", function (event) {
            //    alert("response");
            //    var dd = event.target.response;
            //    dd = JSON.parse(dd);
            //    var newByt = dd.result;

            //    if (self.oldValue != newByt) {
            //        self.setValue(newByt);
            //        var evt = { newValue: newByt, propertyName: columnName };
            //        self.fireValueChanged(evt);
            //        evt = null;
            //    }
            //}, false);
        });

        $ctrl.on(VIS.Events.onClick, function (evt) { //click handler
            if (!self.isReadOnly) {
                $ctrl.w2overlay($ulPopup.clone(true));
            }
            evt.stopPropagation();
        });

        this.setText = function (text) {
            if (text == null) {
                $txt.text("");
                return;
            }
            var pos = text.indexOf('&');
            if (pos != -1)					//	We have a nemonic - creates ALT-_
            {
                var mnemonic = text.toUpperCase().charAt(pos + 1);
                if (mnemonic != ' ') {
                    //setMnemonic(mnemonic);
                    text = text.substring(0, pos) + text.substring(pos + 1);
                }
            }
            $txt.text(text);

        };

        this.disposeComponent = function () {
            $ctrl.off(VIS.Events.onClick);
            $ctrl = null;
            this.setText = null;
            this.values = null;
            $txt = null;
            windowNo = null;
            columnName = null;
            $ulPopup = null;
            SaveTolocalFile = null;
            LoadIntoDatabase = null;
            this.data = null;
            inputCtrl = null;
            self = null;
        };
    };

    VIS.Utility.inheritPrototype(VBinary, IControl);//Inherit

    VBinary.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;

            if (newValue == null) {
                this.setText("-");
                this.value = newValue;
                this.ctrl.val(newValue);
            }
            else {
                var text = "?";
                if (newValue.length > 0) {
                    var bb = newValue;
                    text = "#" + bb.length;
                }
                else {
                    text = newValue.GetType().FullName;
                }
                //	Display it
                this.setText(text != null ? text : "");
                this.value = newValue;
                this.ctrl.val(newValue);
            }
        }
    };

    VBinary.prototype.setReadOnly = function (readOnly) {
        this.isReadOnly = readOnly;
        this.ctrl.prop('disabled', readOnly ? true : false);
        this.setBackground(false);
    };

    VBinary.prototype.getValue = function () {
        return this.value;
    };

    VBinary.prototype.getDisplay = function () {
        return this.value;
    };

    //End VBinary 

    //VURL
    function VURL(columnName, isMandatory, isReadOnly, isUpdateable, displayLength, fieldLength) {
        this.value = null;
        var src = "vis vis-url";// VIS.Application.contextUrl + "Areas/VIS/Images/base/Url20.png";
        var btnCount = 0;
        //create ui
        var $ctrl = $('<input>', { type: 'text', name: columnName, maxlength: fieldLength });
        var $btnSearch = $('<button class="input-group-text"><i class="' + src + '" ></i></button>');
        btnCount += 1;

        //Set Buttons and [pop up]
        var self = this;
        IControl.call(this, $ctrl, VIS.DisplayType.URL, isReadOnly, columnName, isMandatory);

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
        }
        else {
            this.setReadOnly(false);
        }

        //provilized function
        this.getBtnCount = function () {
            return btnCount;
        };

        //Get url Button
        this.getBtn = function (index) {
            if (index == 0) {
                return $btnSearch;
            }
        };

        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                $ctrl.show();
                $btnSearch.show();

            }
            else {
                $ctrl.hide();
                $btnSearch.hide();
            }
        };

        $btnSearch.on(VIS.Events.onClick, function (e) {
            e.stopPropagation();
            if (self.value == null) {
                return;
            } else if (self.isReadOnly) {
                return;
            }
            var urlString = self.value.trim();
            if (urlString.length > 0) {
                if (urlString.contains("http://") || urlString.contains("https://")) {
                    ;
                }
                else {
                    urlString = "http://" + urlString;
                }

                window.open(urlString);
                return;
            }
            VIS.ADialog.warn("URLnotValid", true, null);
        });

        /* Event */
        $ctrl.on("change", function (e) {
            e.stopPropagation();
            var newVal = $ctrl.val();
            if (newVal !== self.oldValue) {
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
            }
        });



        //  dispose 
        this.disposeComponent = function () {
            $btnSearch.off(VIS.Events.onClick);
            $ctrl.off("change");
            self = null;
            $ctrl = null;
            $btnSearch = null;
            this.getBtn = null;
            this.setVisible = null;
            this.value = null;
            src = null;
            btnCount = null;
            this.getBtnCount = null;
            this.getBtn = null;
        };
    };

    VIS.Utility.inheritPrototype(VURL, IControl);//inherit IControl

    VURL.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            this.value = newValue
            //	Set comboValue
            if (newValue == null) {
                this.lastDisplay = "";
                this.ctrl.val("");
                return;
            }

            this.value = newValue;
            this.ctrl.val(newValue);
            this.ctrl.val(newValue);

        }
    };

    VURL.prototype.getValue = function () {
        return this.value;
    };

    VURL.prototype.getDisplay = function () {
        return this.ctrl.val();
    };


    //File

    function VFile(colName, mandatoryField, isReadOnly, isUpdateable, winNo, displayType) {

        var files = false;
        if (displayType == displayType.FileName) {
            files = true;
        }

        DialogType = {
            OpenFile: 0,
            SaveFile: 1,
            Custom: 2
        };

        SelectionType = {
            FilesOnly: 0,
            DirectoryOnly: 1
        };
        //Selection Mode					
        var selectionMode = SelectionType.DirectoryOnly;
        //Save/Open						
        var dialogType = DialogType.Custom;
        //Logger
        this.log = VIS.Logging.VLogger.getVLogger("VFile");

        var displayType = VIS.DisplayType.FileName;

        var src = "fa fa-folder-open-o";//VIS.Application.contextUrl + "Areas/VIS/Images/base/Folder20.png";
        if (files) {
            selectionMode = SelectionType.FilesOnly;
            src = "fa fa-file-text-o";// VIS.Application.contextUrl + "Areas/VIS/Images/base/File20.png";
        }
        var col = colName.toUpperCase();

        if (col.indexOf("open") != -1 || col.indexOf("load") != -1) {
            dialogType = DialogType.OpenFile;
        }
        else if (col.indexOf("save") != -1) {
            dialogType = DialogType.SaveFile;
        }



        var windowNo = winNo;
        var columnName = colName;
        this.value = null;
        var btnCount = 0;

        var $ctrl = $('<input>', { type: 'text', name: columnName});
        var $btnSearch = $('<button class="input-group-text"><i class="' + src + '" /></button>');
        btnCount += 1;

        var inputCtrl = $("<input autocomplete='off' type='file' class='file' name='file'/>");
        $ctrl.append($btnSearch);

        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, mandatoryField);

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
        }
        else {
            this.setReadOnly(false);
        }

        //provilized function
        this.getBtnCount = function () {
            return btnCount;
        };

        //Get url Button
        this.getBtn = function (index) {
            if (index == 0) {
                return $btnSearch;
            }
        };

        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                $ctrl.show();
                $btnSearch.show();

            }
            else {
                $ctrl.hide();
                $btnSearch.hide();
            }
        };

        var self = this; //self pointer

        $btnSearch.on(VIS.Events.onClick, function (evt) {
            if (self.isReadOnly) {
                return;
            }
            evt.stopPropagation();
            if (selectionMode == SelectionType.DirectoryOnly) {
                //inputCtrl.trigger('click');
                var obj = new VIS.VisImportFiles(true);
                obj.onClose = function (names) {
                    self.setValue(names);
                }
                obj.show();
            }
            else {
                if (dialogType == DialogType.SaveFile) {
                    if (self.getValue() != null) {
                        var d = new Date().toISOString().slice(0, 19).replace(/-/g, "");
                        var fileData = "data:;base64," + self.getValue();
                        $(this).attr("href", fileData).attr("download", "file-" + d + ".log");
                    }
                }
                else {
                    //inputCtrl.trigger('click');
                    var obj = new VIS.VisImportFiles(false);
                    obj.onClose = function (names) {
                        self.setValue(name);
                    }
                    obj.show();
                }
            }
        });

        //Upload File on client side and get byte array from file on client side
        inputCtrl.on("change", function (e) {
            e.stopPropagation();
            var file = $(inputCtrl).val().split('\\').pop();
            if (self.oldValue != file) {
                self.setValue(file);
                var evt = { newValue: file, propertyName: columnName };
                self.fireValueChanged(evt);
                evt = null;
            }
        });

        /* Event */
        $ctrl.on("change", function (e) {
            e.stopPropagation();
            var newVal = $ctrl.val();
            this.value = newVal;
            if (newVal !== self.oldValue) {
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
                self.setValue(newVal);
            }
        });

        this.disposeComponent = function () {
            $ctrl.off(VIS.Events.onClick);
            $ctrl.off("change");
            inputCtrl.off("change");
            $ctrl = null;
            windowNo = null;
            columnName = null;
            inputCtrl = null;
            self = null;
        };
    };

    VIS.Utility.inheritPrototype(VFile, IControl);//Inherit

    VFile.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            this.value = newValue;
            this.ctrl.val(newValue);
        }
    };

    VFile.prototype.setReadOnly = function (readOnly) {
        this.isReadOnly = readOnly;
        this.ctrl.prop('disabled', readOnly ? true : false);
        this.setBackground(false);
    };

    VFile.prototype.getValue = function () {
        return this.value;
    };

    VFile.prototype.getDisplay = function () {
        return this.value;
    };

    //End File


    // Amount Division
    function VAmtDimension(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup, windowNo) {
        if (!displayType) {
            displayType = VIS.DisplayType.AmtDimension;
        }

        this.lookup = lookup;
        this.lastDisplay = "";
        this.settingFocus = false;
        this.inserting = false;
        this.settingValue = false;
        this.value = null;
        this.defaultValue = "";
        //create ui
        var $ctrl = $('<input readonly>', { type: 'text', name: columnName });
        var $btnAmtDiv = $('<button class="input-group-text"><i class="vis vis-amtdimension" /></button>');
        var btnCount = 1;
        var self = this;
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory); //call base function

        this.format = VIS.DisplayType.GetNumberFormat(VIS.DisplayType.Amount);
        this.dotFormatter = VIS.Env.isDecimalPoint();

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
        }
        else {

            this.setReadOnly(false);
        }

        this.getBtn = function (index) {
            if (index == 0) {
                return $btnAmtDiv;
            }
        };

        this.getBtnCount = function () {
            return btnCount;
        };

        this.hideButton = function (show) {
            if (show) {
                $btnAmtDiv.show();
            }
            else {
                $btnAmtDiv.hide();
            }
        };

        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                $ctrl.show();
                $btnAmtDiv.show();

            } else {
                $ctrl.hide();
                $btnAmtDiv.hide();
            }
        };

        this.setReadOnly = function (readOnly) {
            //if (!isMandatory) {
            //    //readOnly = true;
            //    $ctrl.css("background-color", "#f8f8f8");
            //}
            //else if (isMandatory && self.value) {
            //    //readOnly = true;
            //    $ctrl.css("background-color", "#f8f8f8");
            //}
            //this.isReadOnly = readOnly;
            //$ctrl.prop('disabled', readOnly ? true : false);
            //if (readOnly) {
            //    $btnAmtDiv.css("opacity", .7);
            //} else {
            //    $btnAmtDiv.css("opacity", 1);
            //}
            this.isReadOnly = readOnly;
            $ctrl.css("background-color", "#f8f8f8");
            this.setBackground(readOnly);
        };




        this.setReadOnlyTextbox = function (readOnly) {

            //this.isReadOnly = readOnly;
            //$ctrl.prop('disabled', readOnly ? true : false);
            if (readOnly) {
                $ctrl.attr('readonly', 'readonly');
                $ctrl.attr('disabled', 'disabled');
                //$btnAmtDiv.css("opacity", .7);
            } else {
                $ctrl.removeAttr('readonly');
                $ctrl.removeAttr('disabled');
                //$btnAmtDiv.css("opacity", 1);
            }
            //this.setBackground(false);
        };


        this.getText = function () {
            return $ctrl.val();
        };

        this.setDefaultValue = function (deValue) {
            this.defaultValue = deValue;
        };

        ///* Event */
        //$ctrl.on("change", function (e) {
        //    e.stopPropagation();
        //    var newVal = $ctrl.val();
        //    this.value = newVal;
        //    if (newVal !== self.oldValue) {
        //        var evt = { newValue: newVal, propertyName: self.getName() };
        //        self.fireValueChanged(evt);
        //        evt = null;
        //    }
        //});


        $btnAmtDiv.on(VIS.Events.onClick, function (e) {
            e.stopPropagation();
            //if (self.isReadOnly) {
            //    return;
            //}

            self.actionText();
            //var obj = new VIS.LocationForm(self.value);
            //obj.load();
            //obj.showDialog();


            //alert("Map button [" + self.value + "] => " + self.getName());
        });

        this.actionText = function () {
            if (!self.value) {
                self.value = 0;
            }

            var orgID = VIS.Env.getCtx().getContextAsInt(windowNo, "AD_Org_ID", false);
            var dValue;

            if (this.defaultValue) {
                self.defaultValue = self.defaultValue.replace(/@/g, "").trim().replace(".value", "");
                dValue = VIS.Env.getCtx().getWindowContext(windowNo, self.defaultValue, false);
            }

            self.oldValue = self.value;
            var obj = new VIS.AmountDivision(self.value, orgID, dValue, self.isReadOnly);
            obj.onClosing = function (rid) {
                if (rid > 0) {
                    self.oldValue = 0;
                    // Refresh lookup to get updated uplve
                    self.lookup.refreshAmountDivision(rid);
                    self.setValue(rid);
                    var evt = { newValue: rid, propertyName: self.getColumnName() };
                    self.fireValueChanged(evt);
                    evt = null;
                }
            };
            obj.show();
            obj = null;
        };

        //$ctrl.on("keydown", function (event) {
        //    if (event.keyCode == 13 || event.keyCode == 9) {//will work on press of Tab key OR Enter Key
        //        self.actionText();
        //    }
        //});

        //dispose 
        this.disposeComponent = function () {
            $btnAmtDiv.off(VIS.Events.onClick);
            self = null;
            $ctrl = null;
            $btnAmtDiv = null;
            this.setVisible = null;

        };
    };

    VIS.Utility.inheritPrototype(VAmtDimension, IControl);//inherit IControl

    VAmtDimension.prototype.setValue = function (newValue, isInserting) {
        // if (!isInserting && this.oldValue != newValue) {
        if (this.oldValue != newValue) {
            this.settingValue = true;
            this.oldValue = newValue;
            this.value = newValue

            //	Set comboValue
            if (newValue == null) {
                this.lastDisplay = "";
                this.ctrl.val("");
                this.settingValue = false;
                return;
            }
            if (this.lookup == null) {
                this.ctrl.val(newValue.toString());
                this.lastDisplay = newValue.toString();
                this.settingValue = false;
                return;
            }

            this.lastDisplay = this.lookup.getDisplay(newValue);

            if (this.lastDisplay && this.lastDisplay.equals("<-1>")) {
                this.lastDisplay = "";
                this.oldValue = null;
                this.value = null;
            }
            if (this.lastDisplay && this.lastDisplay.indexOf("<") > -1) {
                this.value = this.lastDisplay;
            }
            else {
                this.value = newValue;
            }
            //this.ctrl.val(this.lastDisplay);
            if (this.lastDisplay) {
                var _value = this.format.GetFormatedValue(this.lastDisplay);
                this.ctrl.val(this.format.GetFormatAmount(_value, "init", this.dotFormatter));
            }
            this.settingValue = true;

        }
        //else if (isInserting) {
        //    this.lastDisplay = "";
        //    this.ctrl.val("");
        //    this.settingValue = false;
        //    return;
        //}
    };

    VAmtDimension.prototype.getValue = function () {
        return this.value;
    };

    VAmtDimension.prototype.getDisplay = function () {
        var retValue = "";
        if (this.lookup == null)
            retValue = this.value;
        else
            retValue = this.lookup.getDisplay(this.value);
        return retValue;
    };


    // Product Container
    function VProductContainer(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup, windowNo) {
        if (!displayType) {
            displayType = VIS.DisplayType.ProductContainer;
        }
        // Options to show in context menu....
        var options = {};
        this.lookup = lookup;
        this.lastDisplay = "";
        this.settingFocus = false;
        this.inserting = false;
        this.settingValue = false;
        this.value = null;
        this.defaultValue = "";
        var disabled = false;

        //Show Zoom, Preferences and Clear Option
        options[VIS.Actions.zoom] = disabled;
        if (VIS.MRole.getIsShowPreference())
            options[VIS.Actions.preference] = true;
        options[VIS.Actions.remove] = true;

        var $ulPopup = VIS.AEnv.getContextPopup(options);



        //create ui
        var $ctrl = $('<input>', { type: 'text', name: columnName });
        $ctrl.attr('autocomplete', 'off');
        var $btnpContainer = $('<button class="input-group-text"><i class="vis vis-pcontainer" /></button>');
        //var $btnPop = $('<button  tabindex="-1" class="input-group-text"><img tabindex="-1" src="' + VIS.Application.contextUrl + "Areas/VIS/Images/base/Info20.png" + '" /></button>');
        var $btnPop = $('<button  tabindex="-1" class="input-group-text"><i tabindex="-1" Class="fa fa-ellipsis-v" /></button>');
        var btnCount = 1;
        btnCount += 1;
        var self = this;
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory); //call base function

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
        }
        else {

            this.setReadOnly(false);
        }

        this.getBtn = function (index) {
            if (index == 0) {
                return $btnpContainer;
            }
            if (index == 1) { //zoom
                if ($btnPop)
                    return $btnPop;
            }
        };

        this.getBtnCount = function () {
            return btnCount;
        };

        this.hideButton = function (show) {
            if (show) {
                $btnpContainer.show();
                if (this.getBtnCount() > 1) { //zoom
                    if ($btnPop)
                        $btnPop.show();
                }
            }
            else {
                $btnpContainer.hide();
                if (this.getBtnCount() > 1) { //zoom
                    if ($btnPop)
                        $btnPop.hide();
                }
            }
        };

        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                $ctrl.show();
                $btnpContainer.show();
                if (this.getBtnCount() > 1) { //zoom
                    if ($btnPop)
                        $btnPop.show();
                }

            } else {
                $ctrl.hide();
                $btnpContainer.hide();
                if (this.getBtnCount() > 1) { //zoom
                    if ($btnPop)
                        $btnPop.hide();
                }
            }
        };

        this.setReadOnly = function (readOnly) {
            this.isReadOnly = readOnly;
            $ctrl.prop('disabled', readOnly ? true : false);
            if (readOnly) {
                $btnpContainer.css("opacity", .7);
            } else {
                $btnpContainer.css("opacity", 1);
            }
            this.setBackground(false);
        };

        if ($btnPop) {
            $btnPop.on(VIS.Events.onClick, function (e) {
                $btnPop.w2overlay($ulPopup.clone(true));
                e.stopPropagation();
            });
        }


        // Handle Click when click on Context Menu
        if ($ulPopup) {
            $ulPopup.on("click", "LI", function (e) {

                var action = $(e.target).data("action");
                if (action == VIS.Actions.preference) { // Show Preferences
                    var obj = new VIS.ValuePreference(self.mField, self.getValue(), self.getDisplay());
                    if (obj != null) {
                        obj.showDialog();
                    }
                    obj = null;
                }
                else if (action == VIS.Actions.zoom) {// Zoom
                    zoomAction();
                }
                else if (action == VIS.Actions.remove) { // Clear
                    if (self.isReadOnly)
                        return;
                    self.setValue(null, false, true);
                }
            });
        }


        function zoomAction() {
            if (!self.lookup)
                return;
            //
            var zoomQuery = self.lookup.getZoomQuery();
            var value = self.getValue();
            if (value == null) {
                //   value = selectedItem;
            }

            if (value == "")
                value = null;

            if (value != null && !isNaN(value))
                value = parseInt(value);

            //Create Zoom Query to Zoom to product Container window
            zoomQuery = new VIS.Query();
            zoomQuery.addRestriction("M_ProductContainer_ID", VIS.Query.prototype.EQUAL, value);
            zoomQuery.setRecordCount(1);	//	guess
            //}

            var AD_Window_ID = 0;
            if (self.mField.getZoomWindow_ID() > 0) {
                AD_Window_ID = self.mField.getZoomWindow_ID();
            }
            else {
                AD_Window_ID = self.lookup.getZoomWindow(zoomQuery);
            }
            //Open Window
            VIS.viewManager.startWindow(AD_Window_ID, zoomQuery);
        };

        this.getText = function () {
            return $ctrl.val();
        };

        this.setDefaultValue = function (deValue) {
            this.defaultValue = deValue;
        };

        $btnpContainer.on(VIS.Events.onClick, function (e) {
            e.stopPropagation();
            if (self.isReadOnly) {
                return;
            }
            self.actionText(true);
        });

        /*
        * Decide if open container dialog or not.
        * If Must Open that means must open dialog.(when user click Icon)
        */

        this.actionText = function (mustOpen) {
            if (!self.value) {
                self.value = 0;
            }


            var text = $ctrl.val().trim();

            var validated = VIS.Env.parseContext(VIS.Env.getCtx(), windowNo, self.lookup.getTabNo(), self.lookup.info.validationCode, false, true);

            if (!text || mustOpen) {
                self.openForm(0, 0, text, validated);
            }
            else {
                $.ajax({
                    url: baseUrl + 'productContainer/GetProductContainer',
                    data: { text: text, validation: validated },
                    success: function (result) {
                        result = JSON.parse(result);
                        if (result == "null" || result == null || result == "" || result == 0) {
                            self.openForm(0, 0, text, validated);
                        }
                        else {
                            self.setVal(result);
                        }
                    },
                    error: function () {

                    }
                });
            }





        };

        $ctrl.on("keydown", function (event) {
            if (event.keyCode == 13 || event.keyCode == 9) {//will work on press of Tab key OR Enter Key
                self.actionText(false);
            }
        });

        this.setVal = function (rid) {
            self.oldValue = 0;
            self.setValue(rid);
            var evt = { newValue: rid, propertyName: self.getColumnName() };
            self.fireValueChanged(evt, true);
            evt = null;
        };

        this.openForm = function (warehouseID, mLocator, text, validation) {
            self.oldValue = self.value;
            var obj = new VIS.productContainerTree(warehouseID, mLocator, text, validation);
            if (obj != null) {
                obj.showDialog();
                obj.onClosing = function (containerId) {
                    self.setVal(containerId);
                };
            }
            obj = null;
        };


        //dispose 
        this.disposeComponent = function () {
            $btnpContainer.off(VIS.Events.onClick);
            self = null;
            $ctrl = null;
            $btnpContainer = null;
            this.setVisible = null;

        };
    };

    VIS.Utility.inheritPrototype(VProductContainer, IControl);//inherit IControl

    VProductContainer.prototype.setValue = function (newValue, isInserting, fire) {
        // if (!isInserting && this.oldValue != newValue) {
        if (this.oldValue != newValue) {
            this.settingValue = true;
            this.oldValue = newValue;
            this.value = newValue

            //	Set comboValue
            if (newValue == null) {
                this.lastDisplay = "";
                this.ctrl.val("");
                this.settingValue = false;
                if (fire) {
                    var evt = { newValue: newValue, propertyName: this.getName() };
                    this.fireValueChanged(evt);
                    evt = null;
                }
                return;
            }
            if (this.lookup == null) {
                this.ctrl.val(newValue.toString());
                this.lastDisplay = newValue.toString();
                this.settingValue = false;
                return;
            }

            this.lastDisplay = this.lookup.getDisplay(newValue);
            if (this.lastDisplay.equals("<-1>")) {
                this.lastDisplay = "";
                this.oldValue = null;
                this.value = null;
            }
            if (this.lastDisplay.indexOf("<") > -1) {
                this.value = this.lastDisplay;
            }
            else {
                this.value = newValue;
            }
            //this.ctrl.val(this.lastDisplay);
            this.ctrl.val(VIS.Utility.decodeText(this.lastDisplay));
            this.settingValue = true;

        }
        //else if (isInserting) {
        //    this.lastDisplay = "";
        //    this.ctrl.val("");
        //    this.settingValue = false;
        //    return;
        //}
    };

    VProductContainer.prototype.getValue = function () {
        return this.value;
    };

    VProductContainer.prototype.getDisplay = function () {
        var retValue = "";
        if (this.lookup == null)
            retValue = this.value;
        else
            retValue = this.lookup.getDisplay(this.value);
        return retValue;
    };


    /**
     *  VKeyText with Mnemonics interpretation
     *  VKeyText against header panel item key-value pair
     *  @param value  The text to be displayed by the VSpan.
     *  @param name  name of control to bind VSpan with
     */
    function VKeyText(colSql, windowNo, name) {
        this.colSql = colSql;
        this.windowNo = windowNo;
        this.cache = {};
        // this.col = '';
        this.needtoParse = false;

        if (colSql.contains('@')) {
            this.needtoParse = true;
        }



        var strFor = ' for="' + name + '"';

        var $ctrl = $('<span ' + strFor + '></span>');

        IControl.call(this, $ctrl, VIS.DisplayType.Label, true, "lbl" + name);

        this.disposeComponent = function () {
            $ctrl = null;
            self = null;
            if (this.format)
                this.format.dispose();
            this.format = null;
            this.cache = {};
            this.cache = null;
        }
    };


    VIS.Utility.inheritPrototype(VKeyText, IControl); //Inherit

    VKeyText.prototype.setValue = function (newValue, isHTML) {
        var validation = null;
        if (this.needtoParse) {
            validation = VIS.Env.parseContext(VIS.context, this.windowNo, 0, this.colSql, false, true);
        }
        else {
            validation = this.colSql;
        }

        if (!validation || validation.length == 0)
            //console.log(this.info.keyColumn + ": Loader NOT Validated: " + this.info.validationCode);
            return;


        var wIndex = validation.toUpperCase().lastIndexOf('WHERE');
        var where = '-1';
        if (wIndex > -1) {
            where = validation.substring(wIndex);
        }

        if (this.cache[where]) {
            // if (this.oldValue != newValue) {
            //  this.oldValue = newValue;
            this.ctrl.text(this.cache[where]);
            if (isHTML) {
                this.ctrl.html(this.cache[where]);
            }
        }
        else {
            if (validation.toLowerCase().indexOf("select") == -1) {
                this.cache[where] = validation;
                this.ctrl.text(validation);
                return;
            }

            var self = this;
            executeScalarEn(validation, null, function (val) {
                if (val) {
                    self.ctrl.text(val);
                }
                else
                    self.ctrl.text("");
                self.cache[where] = val;
            });
        }
    };

    VKeyText.prototype.getValue = function () {
        if (this.value != null) {
            return this.ctrl.text().toString();
        }
        else {
            return null;
        }
    };


    // VProgressBar

    function VProgressBar(columnName, isMandatory, isReadOnly, isUpdateable, displayLength, fieldLength, controlDisplayType) {
        var $ctrl = $('<div class="vis-progressCtrlWrap">');
        var $rangeCtrl = $('<input>', { type: 'range', step: '1', name: columnName, maxlength: fieldLength, 'data-type': 'int' });
        var $oputput = $('<output  class="vis-progress-output">');

        $ctrl.append($oputput).append($rangeCtrl);

        IControl.call(this, $ctrl, controlDisplayType, isReadOnly, columnName, isMandatory);
        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
        }
        else {
            this.setReadOnly(false);
        }
        this.rangeCtrl = $rangeCtrl;
        this.oputput = $oputput;
        $oputput.text(0);
        this.setText = function (val) {
            $oputput.text(val);
        };
        this.setRange = function (val) {
            if (val != null) {
                $rangeCtrl.val(val);
            } else {
                $rangeCtrl.val(0);
            }

        };

        this.getRange = function () {
            return $rangeCtrl.val();
        };

        var self = this; //self pointer




        /* Event */

        $oputput.keypress(function (event) {
            if (event.which < 46 || event.which > 59) {
                event.preventDefault();
            };
            if ((event.which == 46 && $(this).val().indexOf('.') != -1) || (event.which == 44 && $(this).val().indexOf(',') != -1)) {
                event.preventDefault();
            };

        }).on("blur", function () {
            if ($.isNumeric($rangeCtrl.attr("max")) && Number($rangeCtrl.attr("max")) < Number($(this).text())) {
                $(this).text($rangeCtrl.attr("max"));
            }
            $rangeCtrl.val($(this).text() || 0).change();
        })

        $rangeCtrl.on("input", function (e) {
            e.stopPropagation();
            var newVal = $rangeCtrl.val();
            //self.setOutputPosition();
            $oputput.show();
            $oputput.text(newVal);
            //$ctrl.val(newVal);
        });

        $rangeCtrl.on("change", function (e) {
            e.stopPropagation();
            var newVal = $rangeCtrl.val();
            //$ctrl.val(newVal);
            if (newVal !== self.oldValue) {
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
                //self.setOutputPosition();
            }
        });

        this.disposeComponent = function () {
            $ctrl = null;
            $rangeCtrl = null;
            this.rangeCtrl = this.$oputput = null;
            self = null;
        }
    };

    VIS.Utility.inheritPrototype(VProgressBar, IControl);
    VProgressBar.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            this.setText(newValue);
            this.setRange(newValue);
            //this.setOutputPosition();
        }
    };

    VProgressBar.prototype.getValue = function () {
        return this.getRange();
    };
    VProgressBar.prototype.setMaxValue = function (maxValue) {
        if ($.isNumeric(maxValue)) {
            this.rangeCtrl.attr("max", maxValue);
        } else {
            this.rangeCtrl.attr("max", 100);
        }
    };

    VProgressBar.prototype.setMinValue = function (minValue) {
        if ($.isNumeric(minValue)) {
            this.rangeCtrl.attr("min", minValue);
        }
    };
    VProgressBar.prototype.getDisplay = function () {
        return this.rangeCtrl.val();
    };
    VProgressBar.prototype.getControl = function (parent) {
        if (parent) {
            parent.addClass("vis-progressCtrlWrap");
            parent.append(this.oputput);
            return this.rangeCtrl;
        }
        return this.ctrl;
    };

    VProgressBar.prototype.setReadOnly = function (readOnly) {
        this.isReadOnly = readOnly;
        this.ctrl.find('input').prop('disabled', readOnly ? true : false);
        this.ctrl.find('output').attr('contenteditable', readOnly ? false : true);
        this.setBackground(false);
    };
    //VProgressBar.prototype.setOutputPosition = function () {
    //    var offset = 30;
    //    if (this.editingGrid) {
    //        offset = 0;
    //    }
    //    var width = this.ctrl.width();
    //    var val = this.getValue();
    //    var min = this.mField.getMinValue() ? this.mField.getMinValue() : 0;
    //    var max = this.mField.getMaxValue() ? this.mField.getMaxValue() : 100;
    //    var newPoint = (val - Number(min)) / (Number(max) - Number(min));
    //    if (newPoint < 0) {
    //        newPlace = 0;
    //    }
    //    else if (newPoint > 1) {
    //        newPlace = width;
    //    }
    //    else {
    //        newPlace = width * newPoint;
    //    }

    //    this.getProgressOutput().css({ left: (newPlace / 2) + offset }).text(val);

    //}

    //To implement culture change
    //1.Control type number to textbox:number text not comma in un english culture
    //2.implement formate in setValue Globalize.format(newValue, "n0");
    //3.Change event part formated value Globalize.parseInt(newVal.toString());

    /* NameSpace */
    VIS.Controls.IControl = IControl;
    VIS.Controls.VTextBox = VTextBox;
    VIS.Controls.VLabel = VLabel;
    VIS.Controls.VButton = VButton;
    VIS.Controls.VCheckBox = VCheckBox;
    VIS.Controls.VComboBox = VComboBox;
    VIS.Controls.VTextBoxButton = VTextBoxButton;
    VIS.Controls.VTextArea = VTextArea;
    VIS.Controls.VAmountTextBox = VAmountTextBox;
    VIS.Controls.VNumTextBox = VNumTextBox;
    VIS.Controls.VLocation = VLocation;
    VIS.Controls.VDate = VDate;
    VIS.Controls.VLocator = VLocator;
    VIS.Controls.VAccount = VAccount;
    VIS.Controls.VPAttribute = VPAttribute;
    VIS.Controls.VImage = VImage;
    VIS.Controls.VBinary = VBinary;
    VIS.Controls.VFile = VFile;
    VIS.Controls.VAmtDimension = VAmtDimension;
    VIS.Controls.VProductContainer = VProductContainer;
    VIS.Controls.VKeyText = VKeyText;
    VIS.Controls.VProgressBar = VProgressBar;
    /* END */
}(jQuery, VIS));
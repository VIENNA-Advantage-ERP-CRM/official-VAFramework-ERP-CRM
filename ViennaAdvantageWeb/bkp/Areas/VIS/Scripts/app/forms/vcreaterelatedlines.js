; VIS = window.VIS || {};
; (function (VIS, $) {
    VIS.CreateRelatedLines = function () {
        this.frame;
        this.windowNo;
        this.Record_ID;
        this.Table_ID;
        var $bsyDiv;
        var $self = this; //scoped self pointer
        var $OkBtn;
        var $ApplyBtn;
        var $CancelBtn;
        var $root = $('<div>');
        var SelectedRecords;
        var _ProductLookUp;
        var _ProductCtrl;
        var pattrLookup;
        var _CmbRelatedType;
        var lookupRelatedType = null;
        var ctrlRelatedType = null;
        var prdCtrl;
        var puomLookup = null;
        var pruomLookup = null;
        var PAttrCtrl;
        var PRAttrCtrl;
        var PUomCtrl = null;
        var PRUomCtrl = null;
        var ProductGrid;
        var $DivProductGrid;
        var RelatedGrid;
        var $DivRelatedGrid;
        var product_ID = 0;
        var relType = "";
        var BPartner_ID = 0;
        var isSOTrx;
        var PriceList_ID = 0;
        var precision = 2;
        VIS.translatedTexts = null;
        var format = VIS.DisplayType.GetNumberFormat(VIS.DisplayType.Amount);
        var dotFormatter = VIS.Env.isDecimalPoint();

        this.initalize = function () {

            var elements = [
                "RelatedProductType",
                "M_Product_ID",
                "SearchKeyValue",
                "ASI",
                "C_UOM_ID",
                "QtyEntered",
                "PriceActual",
                "PriceList",

            ];

            VIS.translatedTexts = VIS.Msg.translate(VIS.Env.getCtx(), elements, true);

            $root.append('<div class="VIS_Pref_show vis-formouterwrpdiv">'
                + '<div class= "VIS_Pref_dd">'
                + '<div class="input-group vis-input-wrap" id="VIS_Product_' + $self.windowNo + '" ></div>'
                + '</div>'
                + '<div class= "VIS_Pref_dd"><div class="input-group vis-input-wrap">'
                + '<div class="vis-control-wrap" id="VIS_RelatedType_' + $self.windowNo + '"><label class="VIS_Pref_Label_Font" > '
                + VIS.translatedTexts.RelatedProductType + '</label ></div></div>'
                + '</div></div>');
            $root.append('<div class="vis-crtfrm-datawrp" style="height: 15.5%;"><div id="VIS_ProductGrd_' + $self.windowNo + '" style="height:80px;" ></div></div>');
            $root.append('<div class="vis-crtfrm-datawrp" style="height: 48.5%;"><div id="VIS_RelatedGrd_' + $self.windowNo + '" style="height:250px;" ></div></div>');
            $root.append('<div class="vis-ctrfrm-btnwrp">'
                + '<input id="VIS_CancelBtn_' + $self.windowNo + '" class= "VIS_Pref_btn-2" type = "button" value = "' + VIS.Msg.getMsg("Cancel") + '">'
                + '<input id="VIS_ApplyBtn_' + $self.windowNo + '" class="VIS_Pref_btn-2" type="button" value="' + VIS.Msg.getMsg("Apply") + '">'
                + '<input id="VIS_OKBtn_' + $self.windowNo + '" class="VIS_Pref_btn-2" type="button" value="' + VIS.Msg.getMsg("OK") + '">'
                + '</div>');
            createBusyIndicator();
            $bsyDiv[0].style.visibility = "visible";
        };

        // Get controls from design
        this.intialLoad = function () {
            $OkBtn = $root.find("#VIS_OKBtn_" + $self.windowNo);
            $ApplyBtn = $root.find("#VIS_ApplyBtn_" + $self.windowNo);
            $CancelBtn = $root.find("#VIS_CancelBtn_" + $self.windowNo);
            _ProductCtrl = $root.find("#VIS_Product_" + $self.windowNo);
            _CmbRelatedType = $root.find("#VIS_RelatedType_" + $self.windowNo);
            $DivProductGrid = $root.find("#VIS_ProductGrd_" + $self.windowNo);
            $DivRelatedGrid = $root.find("#VIS_RelatedGrd_" + $self.windowNo);
            LoadControls();
            InitEvents();
            $bsyDiv[0].style.visibility = "hidden";
        };

        // load dynamic vienna controls
        function LoadControls() {
            // Related Type Control
            lookupRelatedType = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), $self.windowNo, 10853, VIS.DisplayType.List);
            ctrlRelatedType = new VIS.Controls.VComboBox("RelatedProductType", false, false, true, lookupRelatedType, 50);
            _CmbRelatedType.prepend(ctrlRelatedType.getControl());

            // Product Control
            _ProductLookUp = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), $self.windowNo, 2221, VIS.DisplayType.Search);
            prdCtrl = new VIS.Controls.VTextBoxButton("M_Product_ID", true, false, true, VIS.DisplayType.Search, _ProductLookUp);

            var _ProductCtrlWrap = $('<div class="vis-control-wrap">');
            var _ProductBtnWrap = $('<div class="input-group-append">');
            _ProductCtrl.append(_ProductCtrlWrap);
            _ProductCtrlWrap.append(prdCtrl.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ')).append('<label>' + VIS.translatedTexts.M_Product_ID + '</label>');
            _ProductCtrl.append(_ProductBtnWrap);
            _ProductBtnWrap.append(prdCtrl.getBtn(0));
            _ProductBtnWrap.append(prdCtrl.getBtn(1));

            VIS.Env.getCtx().setContext($self.windowNo, "M_Product_ID", null);

            pattrLookup = new VIS.MPAttributeLookup(VIS.context, $self.windowNo);
            PAttrCtrl = new VIS.Controls.VPAttribute("M_AttributeSetInstance_ID", false, false, true, VIS.DisplayType.PAttribute, pattrLookup, $self.windowNo, false, false, false, true);
            PRAttrCtrl = new VIS.Controls.VPAttribute("M_AttributeSetInstance_ID", false, false, true, VIS.DisplayType.PAttribute, pattrLookup, $self.windowNo, false, false, false, true);

            // UOM Control
            puomLookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), $self.windowNo, 2222, VIS.DisplayType.TableDir);
            pruomLookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), $self.windowNo, 2222, VIS.DisplayType.TableDir);
            PUomCtrl = new VIS.Controls.VComboBox("C_UOM_ID", true, false, true, puomLookup, 50);
            PRUomCtrl = new VIS.Controls.VComboBox("C_UOM_ID", true, false, true, pruomLookup, 50);

            BPartner_ID = VIS.Env.getCtx().getContextAsInt($self.windowNo, "C_BPartner_ID", false);
            isSOTrx = VIS.Env.getCtx().getWindowContext($self.windowNo, "IsSOTrx", true) == "Y";
            PriceList_ID = VIS.Env.getCtx().getContextAsInt($self.windowNo, "M_PriceList_ID", false);
        };

        // load Details of selected Product
        function GetProductDetails(product_ID, attribute_ID, uom_ID, pricelist_ID) {

            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "RelatedProduct/GetProductData", {
                "M_Product_ID": product_ID, "M_AttributeSetInstance_ID": attribute_ID, "C_UOM_ID": uom_ID,
                "M_PriceList_ID": pricelist_ID, "Table_ID": $self.Table_ID, "Record_ID": $self.Record_ID
            }, CallbackProductDetails);
        };

        // Callback Product Details
        function CallbackProductDetails(data) {
            if (data != null) {
                ProductGrid.add(data);
            }
            GetRelatedProducts(product_ID, 0, 0, PriceList_ID)
        };

        // load Related Products of selected product
        function GetRelatedProducts(product_ID, attribute_ID, uom_ID, pricelist_ID) {
            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "RelatedProduct/GetRelatedProduct", {
                "M_Product_ID": product_ID, "M_AttributeSetInstance_ID": attribute_ID, "C_UOM_ID": uom_ID, "M_PriceList_ID": pricelist_ID,
                "RelatedProductType": relType, "Table_ID": $self.Table_ID, "Record_ID": $self.Record_ID
            }, CallbackRelatedProduct);
        };

        // Callback Related Product
        function CallbackRelatedProduct(data) {
            if (data != null) {
                RelatedGrid.add(data);
            }

            if (RelatedGrid.records.length > 0) {
                for (var i = 0; i < RelatedGrid.records.length; i++) {
                    if (RelatedGrid.records[i].RelatedProductType == 'M') {
                        $($("#grid_" + RelatedGrid.name + "_rec_" + (i + 1)).find("input[type=checkbox]")).prop("checked", true).attr('disabled', true);
                        //$("#grid_" + RelatedGrid.name + "_rec_" + (i + 1) + ' ' + ' input[type=checkbox]').attr('disabled', true);
                    }
                }
            }
            $bsyDiv[0].style.visibility = "hidden";
        };

        // Events
        function InitEvents() {
            prdCtrl.fireValueChanged = function () {
                if (prdCtrl.value != null) {
                    $bsyDiv[0].style.visibility = "visible";
                    product_ID = prdCtrl.value;
                    ProductGrid.clear();
                    RelatedGrid.clear();
                    GetProductDetails(product_ID, 0, 0, PriceList_ID);
                }
                else {
                    product_ID = 0;
                    ProductGrid.clear();
                    RelatedGrid.clear();
                }
            };

            ctrlRelatedType.fireValueChanged = function (e) {
                $bsyDiv[0].style.visibility = "visible";
                relType = ctrlRelatedType.getValue();
                RelatedGrid.clear();
                GetRelatedProducts(product_ID, 0, 0, PriceList_ID);
            };

            $OkBtn.on("click touchstart", function (ev) {
                if (RelatedGrid.records.length > 0) {
                    for (var i = 0; i < RelatedGrid.records.length; i++) {
                        if (RelatedGrid.records[i].RelatedProductType == 'M') {
                            RelatedGrid.select(i + 1);
                        }
                    }
                }

                if (ProductGrid.records.length > 0) {
                    $bsyDiv[0].style.visibility = "visible";
                    GetSelectedRecords();
                    CreateLines(false);
                }
                else {
                    VIS.ADialog.error("VIS_PlzSelLines");
                }
            });

            $ApplyBtn.on("click touchstart", function (ev) {
                if (RelatedGrid.records.length > 0) {
                    for (var i = 0; i < RelatedGrid.records.length; i++) {
                        if (RelatedGrid.records[i].RelatedProductType == 'M') {
                            RelatedGrid.select(i + 1);
                        }
                    }
                }
                if (ProductGrid.records.length > 0) {
                    $bsyDiv[0].style.visibility = "visible";
                    GetSelectedRecords();
                    CreateLines(true);
                }
                else {
                    VIS.ADialog.error("VIS_PlzSelLines");
                }
            });

            $CancelBtn.on("click touchstart", function (ev) {
                $self.frame.close();
            });

        };

        // load Product grid
        this.LoadProductGrid = function () {
            ProductGrid = null;
            ProductGrid = $DivProductGrid.w2grid({
                name: "VIS_ProductGrid_" + $self.windowNo,
                recordHeight: 25,
                show: {
                    toolbar: false,  // indicates if toolbar is v isible
                    //columnHeaders: true,   // indicates if columns is visible
                    //lineNumbers: true,  // indicates if line numbers column is visible
                    selectColumn: false,  // indicates if select column is visible
                    toolbarReload: false,   // indicates if toolbar reload button is visible
                    toolbarColumns: true,   // indicates if toolbar columns button is visible
                    toolbarSearch: false,   // indicates if toolbar search controls are visible
                    toolbarAdd: false,   // indicates if toolbar add new button is visible
                    toolbarDelete: true,   // indicates if toolbar delete button is visible
                    toolbarSave: true,   // indicates if toolbar save button is visible
                },
                multiSelect: true,
                columns: [
                    {
                        field: 'Product_ID', caption: VIS.translatedTexts.M_Product_ID, size: '20%', display: false
                    },
                    {
                        field: 'Value', caption: VIS.translatedTexts.SearchKeyValue, size: '150px'
                    },
                    {
                        field: 'Product', caption: VIS.translatedTexts.M_Product_ID, size: '200px'
                    },
                    {
                        field: "M_AttributeSetInstance_ID", caption: VIS.translatedTexts.ASI, sortable: false, size: '150px', editable: { type: 'custom', ctrl: PAttrCtrl, showAll: true },
                        render: function (record, index, col_index) {
                            var l = pattrLookup;
                            var val = record["M_AttributeSetInstance_ID"];
                            if (record.changes && typeof record.changes["M_AttributeSetInstance_ID"] != 'undefined') {
                                val = record.changes["M_AttributeSetInstance_ID"];
                            }
                            var d;
                            if (l) {
                                d = l.getDisplay(val, true);
                            }
                            return d;
                        }
                    },
                    {
                        //field: 'UOM', caption: VIS.translatedTexts.C_UOM_ID, size: '120px'
                        field: "UOM_ID", caption: VIS.translatedTexts.C_UOM_ID, sortable: false, size: '120px', editable: { type: 'custom', ctrl: PUomCtrl, showAll: true },
                        render: function (record, index, col_index) {
                            var l = puomLookup;
                            var val = record["UOM_ID"];
                            if (record.changes && typeof record.changes["UOM_ID"] != 'undefined') {
                                val = record.changes["UOM_ID"];
                            }
                            var d;
                            if (l) {
                                d = l.getDisplay(val);
                            }
                            return d;
                        }
                    },
                    {
                        field: 'UOM', caption: VIS.translatedTexts.C_UOM_ID, size: '20%', display: false
                    },
                    {
                        field: "QtyEntered", caption: '<div ><span>' + VIS.translatedTexts.QtyEntered + '</span></div>', sortable: false, size: '120px', min: 80, hidden: false, editable: { type: 'number' },
                        render: function (record, index, col_index) {
                            var val = record["QtyEntered"];
                            val = checkcommaordot(event, val);
                            return parseFloat(val).toLocaleString();
                        }
                    },
                    {
                        field: "PriceStd", caption: '<div ><span>' + VIS.translatedTexts.PriceActual + '</span></div>', sortable: false, size: '120px', min: 80, hidden: false, editable: { type: 'number' },

                        render: function (record, index, col_index) {
                            var val = record["PriceStd"];
                            val = checkcommaordot(event, val);
                            return parseFloat(val).toLocaleString();
                        }
                    },
                    {
                        field: "PriceList", caption: '<div ><span>' + VIS.translatedTexts.PriceList + '</span></div>', sortable: false, size: '120px', min: 80, hidden: false, editable: { type: 'number' },

                        render: function (record, index, col_index) {
                            var val = record["PriceList"];
                            val = checkcommaordot(event, val);
                            return parseFloat(val).toLocaleString();
                        }
                    }
                ],
                onEditField: function (event) {
                    id = event.recid;
                    if (event.column == 6 || event.column == 7 || event.column == 8) {
                        ProductGrid.records[event.index][ProductGrid.columns[event.column].field] = checkcommaordot(event, ProductGrid.records[event.index][ProductGrid.columns[event.column].field]);
                        var _value = format.GetFormatAmount(ProductGrid.records[event.index][ProductGrid.columns[event.column].field], "init", dotFormatter);
                        ProductGrid.records[event.index][ProductGrid.columns[event.column].field] = format.GetConvertedString(_value, dotFormatter);
                        $("#grid_VIS_ProductGrid_" + $self.windowNo + "_rec_" + id).keydown(function (event) {
                            if (!dotFormatter && (event.keyCode == 190 || event.keyCode == 110)) {// , separator
                                return false;
                            }
                            else if (dotFormatter && event.keyCode == 188) { // . separator
                                return false;
                            }
                            if (event.target.value.contains(".") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                                if (event.target.value.indexOf('.') > -1) {
                                    event.target.value = event.target.value.replace('.', '');
                                }
                            }
                            else if (event.target.value.contains(",") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                                if (event.target.value.indexOf(',') > -1) {
                                    event.target.value = event.target.value.replace(',', '');
                                }
                            }
                            if (event.keyCode != 8 && event.keyCode != 9 && (event.keyCode < 37 || event.keyCode > 40) &&
                                (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)
                                && event.keyCode != 109 && event.keyCode != 189 && event.keyCode != 110
                                && event.keyCode != 144 && event.keyCode != 188 && event.keyCode != 190) {
                                return false;
                            }
                        });
                    }
                },

                onChange: function (event) {
                    if (event.column == 8) {
                        var _val = format.GetConvertedNumber(event.value_new, dotFormatter);
                        ProductGrid.records[event.index]["PriceList"] = _val.toFixed(precision);

                    }
                    else if (event.column == 7) {
                        var _val = format.GetConvertedNumber(event.value_new, dotFormatter);
                        ProductGrid.records[event.index]["PriceStd"] = _val.toFixed(precision);
                    }
                    else if (event.column == 6) {
                        var _val = format.GetConvertedNumber(event.value_new, dotFormatter);
                        ProductGrid.records[event.index]["QtyEntered"] = _val.toFixed(precision);
                    }
                    else if (event.column == 3) {
                        if (event.value_new != null) {
                            ProductGrid.records[event.index]["M_AttributeSetInstance_ID"] = event.value_new;

                            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "RelatedProduct/GetProductData", {
                                "M_Product_ID": product_ID, "M_AttributeSetInstance_ID": event.value_new, "C_UOM_ID": VIS.Utility.Util.getValueOfInt(ProductGrid.records[event.index]["UOM_ID"]),
                                "M_PriceList_ID": PriceList_ID, "Table_ID": $self.Table_ID, "Record_ID": $self.Record_ID
                            }, function (data) {
                                if (data != null) {
                                    ProductGrid.records[event.index]["PriceStd"] = data.PriceStd;
                                    ProductGrid.records[event.index]["PriceList"] = data.PriceList;
                                    ProductGrid.refreshCell(event.recid, "PriceStd");
                                    ProductGrid.refreshCell(event.recid, "PriceList");
                                }
                            });
                        }
                    }
                    else if (event.column == 4) {
                        if (event.value_new != null) {
                            ProductGrid.records[event.index]["UOM_ID"] = event.value_new;

                            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "RelatedProduct/GetProductData", {
                                "M_Product_ID": product_ID, "M_AttributeSetInstance_ID": VIS.Utility.Util.getValueOfInt(ProductGrid.records[event.index]["M_AttributeSetInstance_ID"]),
                                "C_UOM_ID": event.value_new, "M_PriceList_ID": PriceList_ID, "Table_ID": $self.Table_ID, "Record_ID": $self.Record_ID
                            }, function (data) {
                                if (data != null) {
                                    ProductGrid.records[event.index]["PriceStd"] = data.PriceStd;
                                    ProductGrid.records[event.index]["PriceList"] = data.PriceList;
                                    ProductGrid.refreshCell(event.recid, "PriceStd");
                                    ProductGrid.refreshCell(event.recid, "PriceList");
                                }
                            });
                        }
                    }
                },

                onClick: function (event) {
                    VIS.Env.getCtx().setContext($self.windowNo, "M_Product_ID", VIS.Utility.Util.getValueOfInt(ProductGrid.records[event.recid - 1]["Product_ID"]));
                }
            });
            ProductGrid.hideColumn('Product_ID');
            //ProductGrid.hideColumn('AttributeSetInstance_ID');
            ProductGrid.hideColumn('UOM');
        };

        // load related product grid
        this.LoadRelatedGrid = function () {
            RelatedGrid = null;
            RelatedGrid = $DivRelatedGrid.w2grid({
                name: "VIS_RelatedGrid_" + $self.windowNo,
                recordHeight: 25,
                show: {
                    toolbar: false,  // indicates if toolbar is v isible
                    //columnHeaders: true,   // indicates if columns is visible
                    //lineNumbers: true,  // indicates if line numbers column is visible
                    selectColumn: true,  // indicates if select column is visible
                    toolbarReload: false,   // indicates if toolbar reload button is visible
                    toolbarColumns: true,   // indicates if toolbar columns button is visible
                    toolbarSearch: false,   // indicates if toolbar search controls are visible
                    toolbarAdd: false,   // indicates if toolbar add new button is visible
                    toolbarDelete: true,   // indicates if toolbar delete button is visible
                    toolbarSave: true,   // indicates if toolbar save button is visible
                },
                multiSelect: true,
                columns: [
                    {
                        field: 'Product_ID', caption: VIS.translatedTexts.M_Product_ID, size: '20%', display: false
                    },
                    {
                        field: 'Value', caption: VIS.translatedTexts.SearchKeyValue, size: '150px'
                    },
                    {
                        field: 'Product', caption: VIS.translatedTexts.M_Product_ID, size: '200px'
                    },
                    {
                        field: 'RelatedProductType', caption: VIS.translatedTexts.RelatedProductType, size: '150px'
                    },
                    {
                        field: 'RelatedType', caption: VIS.translatedTexts.RelatedProductType, size: '150px'
                    },
                    {
                        field: "M_AttributeSetInstance_ID", caption: VIS.translatedTexts.ASI, sortable: false, size: '150px', editable: { type: 'custom', ctrl: PRAttrCtrl, showAll: true },
                        render: function (record, index, col_index) {
                            var l = pattrLookup;
                            var val = record["M_AttributeSetInstance_ID"];
                            if (record.changes && typeof record.changes["M_AttributeSetInstance_ID"] != 'undefined') {
                                val = record.changes["M_AttributeSetInstance_ID"];
                            }
                            var d;
                            if (l) {
                                d = l.getDisplay(val, true);
                            }
                            return d;
                        }
                    },
                    {
                        //field: 'UOM', caption: VIS.translatedTexts.C_UOM_ID, size: '100px'
                        field: "UOM_ID", caption: VIS.translatedTexts.C_UOM_ID, sortable: false, size: '100px', editable: { type: 'custom', ctrl: PRUomCtrl, showAll: true },
                        render: function (record, index, col_index) {
                            var l = pruomLookup;
                            var val = record["UOM_ID"];
                            if (record.changes && typeof record.changes["UOM_ID"] != 'undefined') {
                                val = record.changes["UOM_ID"];
                            }
                            var d;
                            if (l) {
                                d = l.getDisplay(val);
                            }
                            return d;
                        }
                    },
                    {
                        field: 'UOM', caption: VIS.translatedTexts.C_UOM_ID, size: '20%', display: false
                    },
                    {
                        field: "QtyEntered", caption: '<div ><span>' + VIS.translatedTexts.QtyEntered + '</span></div>', sortable: false, size: '100px', min: 80, hidden: false, editable: { type: 'number' },
                        render: function (record, index, col_index) {
                            var val = record["QtyEntered"];
                            val = checkcommaordot(event, val);
                            return parseFloat(val).toLocaleString();
                        }
                    },
                    {
                        field: "PriceStd", caption: '<div ><span>' + VIS.translatedTexts.PriceActual + '</span></div>', sortable: false, size: '100px', min: 80, hidden: false, editable: { type: 'number' },

                        render: function (record, index, col_index) {
                            var val = record["PriceStd"];
                            val = checkcommaordot(event, val);
                            return parseFloat(val).toLocaleString();
                        }
                    },
                    {
                        field: "PriceList", caption: '<div ><span>' + VIS.translatedTexts.PriceList + '</span></div>', sortable: false, size: '100px', min: 80, hidden: false, editable: { type: 'number' },

                        render: function (record, index, col_index) {
                            var val = record["PriceList"];
                            val = checkcommaordot(event, val);
                            return parseFloat(val).toLocaleString();
                        }
                    }
                ],
                onUnselect: function (event) {
                    if (event.all) {
                        event.onComplete = function () {
                            if (RelatedGrid.records.length > 0) {
                                for (var i = 0; i < RelatedGrid.records.length; i++) {
                                    if (RelatedGrid.records[i].RelatedProductType == 'M') {
                                        $($("#grid_" + RelatedGrid.name + "_rec_" + (i + 1)).find("input[type=checkbox]")).prop("checked", true).attr('disabled', true);
                                        //$("#grid_" + RelatedGrid.name + "_rec_" + (i + 1) + ' ' + ' input[type=checkbox]').attr('disabled', true);
                                    }
                                }
                            }
                        }
                    }
                    else {
                        event.onComplete = function () {
                            if (RelatedGrid.records[event.index].RelatedProductType == 'M') {
                                $($("#grid_" + RelatedGrid.name + "_rec_" + event.recid).find("input[type=checkbox]")).prop("checked", true).attr('disabled', true);
                                //$("#grid_" + RelatedGrid.name + "_rec_" + RelatedGrid 3.records[i].recid + ' ' + ' input[type=checkbox]').attr('disabled', true);
                            }
                        }
                    }
                },
                onEditField: function (event) {
                    id = event.recid;
                    if (event.column == 8 || event.column == 9 || event.column == 10) {
                        RelatedGrid.records[event.index][RelatedGrid.columns[event.column].field] = checkcommaordot(event, RelatedGrid.records[event.index][RelatedGrid.columns[event.column].field]);
                        var _value = format.GetFormatAmount(RelatedGrid.records[event.index][RelatedGrid.columns[event.column].field], "init", dotFormatter);
                        RelatedGrid.records[event.index][RelatedGrid.columns[event.column].field] = format.GetConvertedString(_value, dotFormatter);
                        $("#grid_VIS_RelatedGrid_" + $self.windowNo + "_rec_" + id).keydown(function (event) {
                            if (!dotFormatter && (event.keyCode == 190 || event.keyCode == 110)) {// , separator
                                return false;
                            }
                            else if (dotFormatter && event.keyCode == 188) { // . separator
                                return false;
                            }
                            if (event.target.value.contains(".") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                                if (event.target.value.indexOf('.') > -1) {
                                    event.target.value = event.target.value.replace('.', '');
                                }
                            }
                            else if (event.target.value.contains(",") && (event.which == 110 || event.which == 190 || event.which == 188)) {
                                if (event.target.value.indexOf(',') > -1) {
                                    event.target.value = event.target.value.replace(',', '');
                                }
                            }
                            if (event.keyCode != 8 && event.keyCode != 9 && (event.keyCode < 37 || event.keyCode > 40) &&
                                (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)
                                && event.keyCode != 109 && event.keyCode != 189 && event.keyCode != 110
                                && event.keyCode != 144 && event.keyCode != 188 && event.keyCode != 190) {
                                return false;
                            }
                        });
                    }
                    else if (event.column == 5) {
                        VIS.Env.getCtx().setContext($self.windowNo, "M_Product_ID", VIS.Utility.Util.getValueOfInt(RelatedGrid.records[event.recid - 1]["Product_ID"]));
                    }
                },

                onChange: function (event) {
                    if (event.column == 10) {
                        var _val = format.GetConvertedNumber(event.value_new, dotFormatter);
                        RelatedGrid.records[event.index]["PriceList"] = _val.toFixed(precision);
                    }
                    else if (event.column == 9) {
                        var _val = format.GetConvertedNumber(event.value_new, dotFormatter);
                        RelatedGrid.records[event.index]["PriceStd"] = _val.toFixed(precision);
                    }
                    else if (event.column == 8) {
                        var _val = format.GetConvertedNumber(event.value_new, dotFormatter);
                        RelatedGrid.records[event.index]["QtyEntered"] = _val.toFixed(precision);
                    }
                    else if (event.column == 5) {
                        if (event.value_new != null) {
                            RelatedGrid.records[event.index]["M_AttributeSetInstance_ID"] = event.value_new;

                            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "RelatedProduct/GetProductData", {
                                "M_Product_ID": RelatedGrid.records[event.index]["Product_ID"], "M_AttributeSetInstance_ID": event.value_new,
                                "C_UOM_ID": VIS.Utility.Util.getValueOfInt(RelatedGrid.records[event.index]["UOM_ID"]), "M_PriceList_ID": PriceList_ID, "Table_ID": $self.Table_ID, "Record_ID": $self.Record_ID
                            }, function (data) {
                                if (data != null) {
                                    RelatedGrid.records[event.index]["PriceStd"] = data.PriceStd;
                                    RelatedGrid.records[event.index]["PriceList"] = data.PriceList;
                                    RelatedGrid.refreshCell(event.recid, "PriceStd");
                                    RelatedGrid.refreshCell(event.recid, "PriceList");
                                }
                            });
                        }
                    }
                    else if (event.column == 6) {
                        if (event.value_new != null) {
                            RelatedGrid.records[event.index]["UOM_ID"] = event.value_new;

                            VIS.dataContext.getJSONData(VIS.Application.contextUrl + "RelatedProduct/GetProductData", {
                                "M_Product_ID": RelatedGrid.records[event.index]["Product_ID"], "M_AttributeSetInstance_ID": VIS.Utility.Util.getValueOfInt(RelatedGrid.records[event.index]["M_AttributeSetInstance_ID"]),
                                "C_UOM_ID": event.value_new, "M_PriceList_ID": PriceList_ID, "Table_ID": $self.Table_ID, "Record_ID": $self.Record_ID
                            }, function (data) {
                                if (data != null) {
                                    RelatedGrid.records[event.index]["PriceStd"] = data.PriceStd;
                                    RelatedGrid.records[event.index]["PriceList"] = data.PriceList;
                                    RelatedGrid.refreshCell(event.recid, "PriceStd");
                                    RelatedGrid.refreshCell(event.recid, "PriceList");
                                }
                            });
                        }
                    }
                },

                onClick: function (event) {
                    VIS.Env.getCtx().setContext($self.windowNo, "M_Product_ID", VIS.Utility.Util.getValueOfInt(RelatedGrid.records[event.recid - 1]["Product_ID"]));
                }
            });

            RelatedGrid.hideColumn('RelatedProductType');
            RelatedGrid.hideColumn('Product_ID');
            RelatedGrid.hideColumn('UOM');
        };

        // Get selected records from Grid
        function GetSelectedRecords() {
            SelectedRecords = [];

            SelectedRecords.push(
                {
                    Product_ID: ProductGrid.records[0].Product_ID,
                    Product: ProductGrid.records[0].Product,
                    AttributeSetInstance_ID: ProductGrid.records[0].M_AttributeSetInstance_ID,
                    UOM_ID: ProductGrid.records[0].UOM_ID,
                    UOM: ProductGrid.records[0].UOM_ID,
                    RelatedProductType: ProductGrid.records[0].RelatedProductType,
                    RelatedType: ProductGrid.records[0].RelatedType,
                    Value: ProductGrid.records[0].Value,
                    QtyEntered: ProductGrid.records[0].QtyEntered,
                    PriceStd: ProductGrid.records[0].PriceStd,
                    PriceList: ProductGrid.records[0].PriceList
                });


            var selection = RelatedGrid.getSelection();
            for (var i = 0; i < selection.length; i++) {
                SelectedRecords.push(
                    {
                        Product_ID: RelatedGrid.records[selection[i] - 1].Product_ID,
                        Product: RelatedGrid.records[selection[i] - 1].Product,
                        AttributeSetInstance_ID: RelatedGrid.records[selection[i] - 1].M_AttributeSetInstance_ID,
                        UOM_ID: RelatedGrid.records[selection[i] - 1].UOM_ID,
                        UOM: RelatedGrid.records[selection[i] - 1].UOM_ID,
                        RelatedProductType: RelatedGrid.records[selection[i] - 1].RelatedProductType,
                        RelatedType: RelatedGrid.records[selection[i] - 1].RelatedType,
                        Value: RelatedGrid.records[selection[i] - 1].Value,
                        QtyEntered: RelatedGrid.records[selection[i] - 1].QtyEntered,
                        PriceStd: RelatedGrid.records[selection[i] - 1].PriceStd,
                        PriceList: RelatedGrid.records[selection[i] - 1].PriceList
                    });
            }
        };

        // function to check comma or dot from given value and return new value
        function checkcommaordot(event, val) {
            var foundComma = false;
            event.value_new = VIS.Utility.Util.getValueOfString(val);
            if (event.value_new.contains(".")) {
                foundComma = true;
                var indices = [];
                for (var i = 0; i < event.value_new.length; i++) {
                    if (event.value_new[i] === ".")
                        indices.push(i);
                }
                if (indices.length > 1) {
                    event.value_new = removeAllButLast(event.value_new, '.');
                }
            }
            if (event.value_new.contains(",")) {
                if (foundComma) {
                    event.value_new = removeAllButLast(event.value_new, ',');
                }
                else {
                    var indices = [];
                    for (var i = 0; i < event.value_new.length; i++) {
                        if (event.value_new[i] === ",")
                            indices.push(i);
                    }
                    if (indices.length > 1) {
                        event.value_new = removeAllButLast(event.value_new, ',');
                    }
                    else {
                        event.value_new = event.value_new.replace(",", ".");
                    }
                }
            }
            if (event.value_new == "") {
                event.value_new = "0";
            }
            return event.value_new;
        };

        // Remove all seperator but only bring last seperator
        function removeAllButLast(amt, seprator) {
            var parts = amt.split(seprator);
            amt = parts.slice(0, -1).join('') + '.' + parts.slice(-1);
            if (amt.indexOf('.') == (amt.length - 1)) {
                amt = amt.replace(".", "");
            }
            return amt;
        };

        // Create busy indicator
        function createBusyIndicator() {
            $bsyDiv = $("<div class='vis-apanel-busy'>");
            $bsyDiv.css({
                "position": "absolute", "width": "98%", "height": "97%", 'text-align': 'center', 'z-index': '999'
            });
            $bsyDiv[0].style.visibility = "visible";
            $root.append($bsyDiv);
        };

        // Create Related Products lines
        CreateLines = function (fromApply) {
            $.ajax({
                url: VIS.Application.contextUrl + "RelatedProduct/SaveRelatedLines",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ Data: SelectedRecords, Table_ID: $self.Table_ID, Record_ID: $self.Record_ID }),

                success: function (data) {
                    if (JSON.parse(data) != "") {
                        prdCtrl.setValue(null, false, true);
                        ProductGrid.clear();
                        RelatedGrid.clear();
                        VIS.ADialog.info("", true, JSON.parse(data), "");
                    }
                    $bsyDiv[0].style.visibility = "hidden";
                    if (!fromApply) {
                        $self.frame.close();
                    }
                },
                error: function () {
                    $bsyDiv[0].style.visibility = "hidden";
                }
            })
        };

        this.getRoot = function () {
            return $root;
        };
    };

    VIS.CreateRelatedLines.prototype.init = function (windowNo, frame) {
        this.frame = frame;
        this.windowNo = windowNo;
        this.Record_ID = this.frame.getRecord_ID();
        this.Table_ID = this.frame.getAD_Table_ID();
        this.initalize();
        this.intialLoad();
        this.frame.getContentGrid().append(this.getRoot());
        var ssef = this;
        window.setTimeout(function () {
            ssef.LoadProductGrid();
            ssef.LoadRelatedGrid();
        }, 50);
        window.setTimeout(function () {

        }, 50);
    };
    VIS.CreateRelatedLines.prototype.setHeight = function () {
        return "575";
    };
    VIS.CreateRelatedLines.prototype.setWidth = function () {
        return "1050";
    };

    VIS.CreateRelatedLines.prototype.dispose = function () {
        w2ui['VIS_ProductGrid_' + this.windowNo].destroy();
        w2ui['VIS_RelatedGrid_' + this.windowNo].destroy();
        this.frame = null;
        this.windowNo = null;
        $bsyDiv = null;
        $self = null;
        $OkBtn = null;
        $ApplyBtn = null;
        $root = null;
        _ProductLookUp = null;
        pattrLookup = null;
        puomLookup = null;
        pruomLookup = null;
        PUomCtrl = null;
        PRUomCtrl = null;
        _ProductCtrl = null;
        _CmbRelatedType = null;
        lookupRelatedType = null;
        ctrlRelatedType = null;
        PAttrCtrl = null;
        PRAttrCtrl = null;
        SelectedRecords = null;
    };
})(VIS, jQuery);

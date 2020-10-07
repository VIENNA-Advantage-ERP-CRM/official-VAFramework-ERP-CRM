/********************************************************
 * Module Name    :     Application
 * Purpose        :     Generate Product Container.
 * Author         :     Amit Bansal
 * Date           :     13-July-2018
  ******************************************************/
; (function (VIS, $) {
    VIS.AForms = VIS.AForms || {};
    function productContainer() {

        this.frame;
        this.windowNo;
        var $self = this;
        var $root = $("<div style='height:100%;background-color:white;' >");
        //var $bsyDiv = $("<div class='vis-apanel-busy' style='height:100%;position:relative;z-index: 2'>");
        var $bsyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');

        /**Create Main View Variable **/
        var $mainpageContent = null;
        var $formDataRow = null;
        var $formLeftWrap = null;
        var $formRightWrap = null;
        var $formdata = null;
        var $actionDiv = null;

        // Left div Variables
        var $lblName = null;
        var $containerName = null;

        var $lblWarehouse = null;
        var $warehouseSearch = null;
        var _ReqLookUpWarehouse = null;
        this.$ReqControlWarehouse = null; // for Globaly access use this operator

        var $lblLocator = null;
        var $LocatorSearch = null;
        var _ReqLookUpLocator = null;
        this.$ReqControlLocator = null; // for Globaly access use this operator

        var $searchRecordBtn = null;
        var $okBtn = null;

        /** Create Grid Variable**/
        var $ContainerGrid = null;
        var CGrid = null;
        var grdname = null;

        this.$selectedContainerId = null;

        this.Initialize = function () {
            busyDiv('visible');
            createMainView();
            bindEvents();
            busyDiv('hidden');
        };

        function createMainView() {
            $mainpageContent = $('<div class="Page-content" style="height:100%;background-color:white;">');

            /**created 1st Div **/
            $formLeftWrap = $(' <div class="formLeft" style="background-color: rgb(241, 241, 241);display: inline-block; float: left;width:23%;height:87.8%;overflow:auto;padding-left: 7px;">');

            $formDataRow = $('<div class="VIS_formDataRow">');
            $formdata = $('<div id=PCont_Name_' + $self.windowNo + ' class="VIS_formData">');
            $lblName = $('<label>' + VIS.Msg.translate(VIS.Env.getCtx(), "Name") + '</label><br>');
            $containerName = $('<input type=text id=containerName_' + $self.windowNo + ' ></input>');
            $formdata.append($lblName).append($containerName);
            $formDataRow.append($formdata);
            $formLeftWrap.append($formDataRow);

            $formDataRow = $('<div class="VIS_formDataRow">');
            $formdata = $('<div id=PCont_WarehouseSearch_' + $self.windowNo + ' class="VIS_formData" style="margin-top:20px">');
            $lblWarehouse = $('<label>' + VIS.Msg.translate(VIS.Env.getCtx(), "M_Warehouse_ID") + '</label><br>');
            $formdata.append($lblWarehouse);
            $formDataRow.append($formdata);
            $formLeftWrap.append($formDataRow);

            $formDataRow = $('<div class="VIS_formDataRow">');
            $formdata = $('<div id=PCont_LocatorSearch_' + $self.windowNo + ' class="VIS_formData" style="margin-top:20px">');
            $lblLocator = $('<label>' + VIS.Msg.translate(VIS.Env.getCtx(), "M_Locator_ID") + '</label><br>');
            $formdata.append($lblLocator).append($LocatorSearch);
            $formDataRow.append($formdata);
            $formLeftWrap.append($formDataRow);

            $formDataRow = $('<div class="VIS_formDataRow">');
            $formdata = $('<div class="VIS_formData">');
            //$searchRecordBtn = $('<span class="gray-btn" style="Float:left;margin-top:20px">' + VIS.Msg.getMsg("VIS_Search") + '</span>');
            $searchRecordBtn = $('<input type=button id="PCont_btnSearch_' + $self.windowNo + ' class="#" style="margin-top:20px;width: 100px;" value = ' + VIS.Msg.getMsg("Search") + '></input>');
            $formdata.append($searchRecordBtn);
            $formDataRow.append($formdata);
            $formLeftWrap.append($formDataRow);
            /**End form Left wrapper**/

            /**created 2nd Div **/
            $formRightWrap = $(' <div class="formRight" style="display: inline-block; float: left; width: 75%; height: 87.8%; margin-left: 10px;">');
            $ContainerGrid = $('<div style="display: inline-block; float: left; width: 75%; height: 87.8%; margin-left: 10px;">');
            $actionDiv = $('<div>');
            $okBtn = $('<input type=button id="PCont_OK_' + $self.windowNo + ' style="margin-top:20px;width: 100px;" value = ' + VIS.Msg.getMsg("OK") + '></input>');
            $actionDiv.append($okBtn);
            $formRightWrap.append($ContainerGrid).append($actionDiv);

            $mainpageContent.append($formLeftWrap).append($formRightWrap);

            $root.append($mainpageContent).append($bsyDiv);

        };

        function bindEvents() {

            // load warehouse control 
            $warehouseSearch = $root.find("#PCont_WarehouseSearch_" + $self.windowNo);
            loadControlWarehouse();

            //load locator control
            $LocatorSearch = $root.find("#PCont_LocatorSearch_" + $self.windowNo);
            loadControlLocator();

            // when the form is intailize, on behalf of default filter -- load grid
            var name = $root.find("#containerName_" + $self.windowNo)[0].value;
            getProductContainer(name, $ReqControlWarehouse.value, $ReqControlLocator.value);

            $searchRecordBtn.on("click", function () {
                // get container name from filter
                var name = $root.find("#containerName_" + $self.windowNo)[0].value;
                // load container on behalf of filter parameter
                getProductContainer(name, $ReqControlWarehouse.value, $ReqControlLocator.value);
            });

            $okBtn.on("click", function () {
                // get key column for updating container ID for the same record
                var tableName = VIS.context.getWindowTabContext($self.windowNo, 1, "KeyColumnName");
                // get record id on behalf of key column
                var RecordId = 0;
                if (tableName.length > 0) {
                    RecordId = VIS.context.getWindowContext($self.windowNo, tableName, true);
                }
                else {
                    w2alert(VIS.Msg.getMsg("VIS_TableNotFound"));
                    return;
                }
                // call to ajax or updation 
                if (RecordId > 0 && $selectedContainerId > 0) {
                    updateProductContainer(tableName, RecordId, $selectedContainerId);
                }
                else {
                    w2alert(VIS.Msg.getMsg("VIS_RecordNotSelected"));
                }
            });
        };


        //Warehouse control Load
        function loadControlWarehouse() {
            _ReqLookUpWarehouse = VIS.MLookupFactory.get(VIS.Env.getCtx(), $self.windowNo, 3798, VIS.DisplayType.Search, "M_Warehouse_ID", 0, false, null); //ctx, windowNo, column_ID, AD_Reference_ID, columnName, AD_Reference_Value_ID, isParent, validationCode
            this.$ReqControlWarehouse = new VIS.Controls.VTextBoxButton("M_Warehouse_ID", true, true, true, VIS.DisplayType.Search, _ReqLookUpWarehouse); // columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup
            this.$ReqControlWarehouse.getControl().css("width", "55%");
            this.$ReqControlWarehouse.getBtn(0).css("height", "29px");
            this.$ReqControlWarehouse.getBtn(0).css("margin-top", "-1px");
            this.$ReqControlWarehouse.getBtn(0).css("width", "14%");
            this.$ReqControlWarehouse.getBtn(1).css("height", "29px");
            this.$ReqControlWarehouse.getBtn(1).css("margin-top", "-1px");
            this.$ReqControlWarehouse.getBtn(1).css("width", "14%");
            $warehouseSearch.append(this.$ReqControlWarehouse.getControl());
            $warehouseSearch.append(this.$ReqControlWarehouse.getBtn(0));
            $warehouseSearch.append(this.$ReqControlWarehouse.getBtn(1));

            //this.$ReqControlWarehouse.fireValueChanged = locationChanged;

            var WarehouseId = GetValuefromContext($self.windowNo, "M_Warehouse_ID");
            $ReqControlWarehouse.setValue(WarehouseId);
        };

        // Locator Control Load
        function loadControlLocator() {
            //_ReqLookUpLocator = VIS.MLookupFactory.get(VIS.Env.getCtx(), $self.windowNo, 3537, VIS.DisplayType.Locator, "M_Locator_ID", 0, false, null); //ctx, windowNo, column_ID, AD_Reference_ID, columnName, AD_Reference_Value_ID, isParent, validationCode
            _ReqLookUpLocator = new VIS.MLocatorLookup(VIS.Env.getCtx(), $self.windowNo);
            this.$ReqControlLocator = new VIS.Controls.VLocator("M_Locator_ID", true, true, true, VIS.DisplayType.Locator, _ReqLookUpLocator); // columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup
            this.$ReqControlLocator.getControl().css("width", "55%");
            this.$ReqControlLocator.getBtn(0).css("height", "29px");
            this.$ReqControlLocator.getBtn(0).css("margin-top", "-1px");
            this.$ReqControlLocator.getBtn(0).css("width", "14%");
            this.$ReqControlLocator.getBtn(1).css("height", "29px");
            this.$ReqControlLocator.getBtn(1).css("margin-top", "-1px");
            this.$ReqControlLocator.getBtn(1).css("width", "14%");
            $LocatorSearch.append(this.$ReqControlLocator.getControl());
            $LocatorSearch.append(this.$ReqControlLocator.getBtn(0));
            $LocatorSearch.append(this.$ReqControlLocator.getBtn(1));

            //this.$ReqControlLocator.fireValueChanged = locationChanged;

            var locatorId = GetValuefromContext($self.windowNo, "M_Locator_ID");
            $ReqControlLocator.setValue(locatorId);
        }; 

        // pick value from the context
        function GetValuefromContext(windowNo, token) {
            var value = VIS.context.getWindowContext(windowNo, token, false); // windowNo, context, onlyWindow
            return value;
        };

        /** Load Gid**/
        function getProductContainer(Name, WarehouseId, LocatorId) {
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "ProductContainer/ProductContainer",
                data: { Name: Name, WarehouseId: WarehouseId, LocatorId: LocatorId },
                dataType: "json",
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result) {
                        createContainerGrid(result);
                    }
                    else {
                        w2ui[grdname].clear();
                    }
                },
                error: function (er) {
                    console.log(er);

                }
            });
        };

        /** Create Grids**/
        function createContainerGrid(data) {
            var grdCols = [];
            var grdRows = [];

            grdCols.push({ field: "ContainerName", caption: VIS.Msg.getMsg("ContainerName"), size: "15%" });
            grdCols.push({ field: "Width", caption: VIS.Msg.getMsg("Width"), size: "15%" });
            grdCols.push({ field: "Height", caption: VIS.Msg.getMsg("Height"), size: "15%" });
            grdCols.push({ field: "ParentPath", caption: VIS.Msg.getMsg("ParentPath"), size: "22%" });
            grdCols.push({ field: "M_ProductContainer_ID", caption: VIS.Msg.getMsg("M_ProductContainer_ID"), hidden: true });
            grdCols.push({ field: "Ref_M_Container_ID", caption: VIS.Msg.getMsg("Ref_M_Container_ID"), hidden: true });

            for (var j = 0; j < data.length; j++) {
                var row = {};
                row['M_ProductContainer_ID'] = data[j].M_ProductContainer_ID;
                row['Ref_M_Container_ID'] = data[j].Ref_M_Container_ID;
                row['ContainerName'] = data[j].ContainerName;
                row['ParentPath'] = data[j].ParentPath;
                row['Width'] = data[j].Width;
                row['Height'] = data[j].Height;
                row['recid'] = j + 1;
                grdRows[j] = row;
            }

            grdname = 'PCont_gridContainer' + Math.random();
            grdname = grdname.replace('.', '');
            w2utils.encodeTags(grdRows);
            CGrid = $ContainerGrid.w2grid({
                name: grdname,
                recordHeight: 20,
                show: {
                    toolbar: false,  // indicates if toolbar is visible
                    columnHeaders: true,   // indicates if columns is visible
                    lineNumbers: true,  // indicates if line numbers column is visible
                    selectColumn: false,  // indicates if select column is visible
                    toolbarReload: false,   // indicates if toolbar reload button is visible
                    toolbarColumns: true,   // indicates if toolbar columns button is visible
                    toolbarSearch: false,   // indicates if toolbar search controls are visible
                    toolbarAdd: false,   // indicates if toolbar add new button is visible
                    toolbarDelete: false,   // indicates if toolbar delete button is visible
                    toolbarSave: false,   // indicates if toolbar save button is visible
                    selectionBorder: false,	 // display border arround selection (for selectType = 'cell')
                    recordTitles: false	 // indicates if to define titles for records

                },
                columns: grdCols,
                records: grdRows,
                onClick: function (event) {
                    //if (event.column < 8 && CGrid.records.length > 0) {
                    $selectedContainerId = CGrid.get(event.recid).M_ProductContainer_ID;
                    //}
                    //gridRecordId = [];
                    //gridRecordId.push(CGrid.get(event.recid).M_ProductContainer_ID);
                },
            });
        };

        /** OK Button - update containe ID on Record**/
        function updateProductContainer(TableName, RecordId, ContainerId) {
            $.ajax({
                type: "POST",
                url: VIS.Application.contextUrl + "ProductContainer/UpdateProductContainer",
                data: { TableName: TableName, RecordId: RecordId, ContainerId: ContainerId },
                dataType: "json",
                success: function (data) {
                    var result = JSON.parse(data);
                    if (result) {
                        w2alert(VIS.Msg.getMsg("VIS_UpdatedSuccess"));

                        // Re-Load container grid
                        var name = $root.find("#containerName_" + $self.windowNo)[0].value;
                        getProductContainer(name, $ReqControlWarehouse.value, $ReqControlLocator.value);
                        //$self.dispose();
                    }
                    else {
                        w2alert(VIS.Msg.getMsg("VIS_NotUpdated"));
                    }
                },
                error: function (er) {
                    console.log(er);
                }
            });
        };

        // Busy Indicator
        function busyDiv(Value) {
            $bsyDiv[0].style.visibility = Value;
        };

        this.getRoot = function () {
            return $root;
        };

        this.formSizeChanged = function (height, width) { };

        this.sizeChanged = function (height, width) { };

        disposeComponent = function () {
            //$root.empty();
            //$root.remove();
            /**Create Main View Variable **/
            var $mainpageContent = null;
            var $formDataRow = null;
            var $formLeftWrap = null;
            var $formRightWrap = null
            var $formdata = null;
            var $actionDiv = null;

            // Left div Variables
            var $lblName = null;
            var $containerName = null;

            var $lblWarehouse = null;
            var $warehouseSearch = null;
            var _ReqLookUpWarehouse = null;
            this.$ReqControlWarehouse = null;

            var $lblLocator = null;
            var $LocatorSearch = null;
            var _ReqLookUpLocator = null;
            this.$ReqControlLocator = null;

            var $searchRecordBtn = null;
            var $okBtn = null;

            /** Create Grid Variable**/
            var $ContainerGrid = null;
            var CGrid = null;
            var grdname = null;

            this.$selectedContainerId = null;
        };
    };

    productContainer.prototype.init = function (windowNo, frame) {

        this.frame = frame;
        this.windowNo = windowNo;
        var obj = this.Initialize();
        this.frame.getContentGrid().append(this.getRoot());
        this.formSizeChanged();

    };


    productContainer.prototype.sizeChanged = function (height, width) {
        this.formSizeChanged(height, width);
    };

    productContainer.prototype.refresh = function (height, width) {
        this.formSizeChanged();
    };

    //Must implement dispose
    productContainer.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        disposeComponent();

        //call frame dispose function
        this.frame = null;
    };


    VIS.AForms.productContainer = productContainer;
})(VIS, jQuery);
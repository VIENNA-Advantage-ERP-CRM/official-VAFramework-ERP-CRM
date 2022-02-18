
/********************************************************
 * Module Name    :     Application
 * Purpose        :     Create Forecast Line (On Team Master / master Forecast )
 * Author         :     Amit Bansal
 * Date           :     03-Oct-2021
  ******************************************************/
; (function (VIS, $) {
    VIS.AForms = VIS.AForms || {};
    function createforecast() {

        this.frame;
        this.windowNo;
        var $self = this;
        var $root = $("<div style='height:100%;background-color:white;' class='vis-forms-container' >");
        var $bsyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
        var ctx = VIS.Env.getCtx();
        var $mainpageContent = null;
        var $formWrap = null;
        var $formDataRow = null;
        var $formData = null;

        // controls of fields
        var divOrganizationCtrl;
        var _OrganizationLookUp;
        var _OrganizationCtrl;

        var divPeriodCtrl;
        var _PeriodLookUp;
        var _PeriodCtrl;

        var divIncludeSOCtrl;
        var _IncludeSOCtrl;

        var divDocumentTypeCtrl;
        var _DocumentTypeLookUp;
        var _DocumentTypeCtrl;

        var divIncludeOpenSalesOrderCtrl;
        var _IncludeOpenSalesOrderCtrl;

        var divOpenSalesOrderCtrl;
        var _OpenSalesOrderLookUp;
        var _OpenSalesOrderCtrl;

        var divIncludeOpportunityCtrl;
        var _IncludeOpportunityCtrl;

        var divOpportunityCtrl;
        var _OpportunityLookUp;
        var _OpportunityCtrl;

        var divTeamForecastCtrl;
        var _TeamForecastLookUp;
        var _TeamForecastCtrl;
        var _TeamForecast_ID = 0;

        var divBudgetForecastCtrl;
        var _BudgetForecastLookUp;
        var _BudgetForecastCtrl;
        var _budgetForecast_ID = 0;

        var divProductCategoryCtrl;
        var _ProductCategoryLookUp;
        var _ProductCategoryCtrl;

        var divSalesPriceListCtrl;
        var _SalesPriceListLookUp;
        var _SalesPriceListCtrl;

        var divBudgetQuantityCtrl;
        var _BudgetQuantityCtrl;

        var divGenerateLines;
        var _GenerateLines

        var $actionPanel = null;
        var $buttons = null;
        var $btnOk = null;
        var $btnCancel = null;
        var $btnApply = null;
        this.ad_table_ID;
        this.ad_window_ID;
        this.record_ID;
        var AD_Org_ID = VIS.context.getWindowContextAsInt($self.windowNo, "AD_Org_ID", false);
        var AD_Client_ID = VIS.context.getWindowContextAsInt($self.windowNo, "AD_Client_ID", false);

        var isMasterForecast = false;
        var isBudgetForecast = false;

        var elements = [
            "AD_Org_ID",
            "C_Period_ID",
            "C_DocType_ID",
            "M_Product_Category_ID",
            "TeamForecast",
            "MasterForecast",
            "M_PriceList_ID"
        ];

        VIS.translatedTexts = VIS.Msg.translate(VIS.Env.getCtx(), elements, true);

        this.Initialize = function () {
            busyDiv(true);
            loadWindowAndRecordInfo();
            createMainView();
            GetControls();
            loadControls();
            bindEvents();
            busyDiv(false);
        };

        /** Set Height of Form */
        this.setHeight = function () {
            if (isMasterForecast) {
               367;
            }
            else if (isBudgetForecast){
                402;
            }
            else {
               352;
            }
        };

        /** set width of form */
        this.setWidth = function () {
            return 635;
        };

        /**Load org value and check window (Either team forecast and master Forecast) and Team Forecast reference  */
        function loadWindowAndRecordInfo() {
            var data = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "ForecastForm/GetTableAndRecordInfo", {
                "AD_Table_ID": $self.ad_table_ID, "AD_Record_ID": $self.record_ID
            });
            if (data != null) {
                AD_Org_ID = VIS.Utility.Util.getValueOfInt(data["AD_Org_ID"]);
                isMasterForecast = VIS.Utility.Util.getValueOfString(data["Table_Name"]) == "C_MasterForecast" ? true : false;
                isBudgetForecast = VIS.Utility.Util.getValueOfString(data["Table_Name"]) == "VA073_SalesForecast" ? true : false;
                _TeamForecast_ID = VIS.Utility.Util.getValueOfInt(data["TeamColumn_ID"]);
                _budgetForecast_ID = VIS.Utility.Util.getValueOfInt(data["BudgetColumn_ID"]);
            }
        };

        /** Is used to design Forecast Form */
        function createMainView() {
            $mainpageContent = $('<div class="vis-budget-main-wrap vis-formouterwrpdiv">');

            // Row 1
            $formWrap = $('<div class="vis-budget-form-wrap">');
            $formDataRow = $('<div class="vis-budget-form-row">');
            $formData = $('<div class="input-group vis-input-wrap" id="VIS_Organization_' + $self.windowNo + '">');
            $formDataRow.append($formData);
            $formWrap.append($formDataRow);

            // Row 2
            $formDataRow = $('<div class="vis-budget-form-row">');
            $formData = $('<div class="input-group vis-input-wrap" id="VIS_Period_' + $self.windowNo + '">');
            $formDataRow.append($formData);
            $formWrap.append($formDataRow);

            // Row 3
            $formDataRow = $('<div class="vis-budget-form-row">');
            $formData = $('<div class="input-group vis-input-wrap" id="VIS_IncludeSO_' + $self.windowNo + '">');
            $formDataRow.append($formData);
            $formWrap.append($formDataRow);

            // Row 4
            $formDataRow = $('<div class="vis-budget-form-row">');
            $formData = $('<div class="input-group vis-input-wrap" id="VIS_DocumentType_' + $self.windowNo + '">');
            $formDataRow.append($formData);
            $formWrap.append($formDataRow);

            // Row 5
            $formDataRow = $('<div class="vis-budget-form-row">');
            $formData = $('<div class="input-group vis-input-wrap" id="VIS_IncludeOpenSalesOrder_' + $self.windowNo + '">');
            $formDataRow.append($formData);
            $formWrap.append($formDataRow);

            // Row 6
            $formDataRow = $('<div class="vis-budget-form-row">');
            $formData = $('<div class="input-group vis-input-wrap" id="VIS_OpenSalesOrder_' + $self.windowNo + '">');
            $formDataRow.append($formData);
            $formWrap.append($formDataRow);

            // Row 7
            $formDataRow = $('<div class="vis-budget-form-row">');
            $formData = $('<div class="input-group vis-input-wrap" id="VIS_IncludeOpportunity_' + $self.windowNo + '">');
            $formDataRow.append($formData);
            $formWrap.append($formDataRow);

            // Row 8
            $formDataRow = $('<div class="vis-budget-form-row">');
            $formData = $('<div class="input-group vis-input-wrap" id="VIS_Opportunity_' + $self.windowNo + '">');
            $formDataRow.append($formData);
            $formWrap.append($formDataRow);

            // Row 8.1
            if (isMasterForecast) {
                $formDataRow = $('<div class="vis-budget-form-row">');
                $formData = $('<div class="input-group vis-input-wrap" id="VIS_TeamForecast_' + $self.windowNo + '">');
                $formDataRow.append($formData);
                $formWrap.append($formDataRow);
            }
            // Row 8.2
            if (isBudgetForecast) {
                $formDataRow = $('<div class="vis-budget-form-row">');
                $formData = $('<div class="input-group vis-input-wrap" id="VIS_BudgetForecast_' + $self.windowNo + '">');
                $formDataRow.append($formData);
                $formWrap.append($formDataRow);
            }

            // Row 9
            $formDataRow = $('<div class="vis-budget-form-row">');
            $formData = $('<div class="input-group vis-input-wrap" id="VIS_ProductCategory_' + $self.windowNo + '">');
            $formDataRow.append($formData);
            $formWrap.append($formDataRow);

            // Row 9.2
            if (isBudgetForecast) {
                $formDataRow = $('<div class="vis-budget-form-row">');
                $formData = $('<div class="input-group vis-input-wrap" id="VIS_PriceList_' + $self.windowNo + '">');
                $formDataRow.append($formData);
                $formWrap.append($formDataRow);
            }

            // Row 10
            $formDataRow = $('<div class="vis-budget-form-row">');
            $formData = $('<div class="input-group vis-input-wrap" id="VIS_BudgetQuantity_' + $self.windowNo + '">');
            $formDataRow.append($formData);
            $formWrap.append($formDataRow);

            // Row 11
            $formDataRow = $('<div class="vis-budget-form-row">');
            $formData = $('<div class="input-group vis-input-wrap" id="VIS_GenerateLines_' + $self.windowNo + '">');
            $formDataRow.append($formData);
            $formWrap.append($formDataRow);

            //Action 
            $actionPanel = $('<div class="VIS_buttons-wrap .vis-budget-buttons-wrap">');

            $buttons = $('<div class="pull-right">');
            $btnOk = $('<span class="btn vis-budget-buttons-wrap-span">' + VIS.Msg.translate(ctx, "VIS_OK") + '</span>');
            $btnCancel = $('<span class="btn vis-budget-buttons-wrap-span">' + VIS.Msg.translate(ctx, "Cancel") + '</span>');
            $btnApply = $('<span class="btn vis-budget-buttons-wrap-span">' + VIS.Msg.translate(ctx, "Apply") + '</span>');
            $buttons.append($btnOk).append($btnApply).append($btnCancel);
            $actionPanel.append($buttons);
            $mainpageContent.append($formWrap).append($actionPanel);
            $root.append($mainpageContent).append($bsyDiv);
        };

        /**Get Controls  */
        function GetControls() {

            divOrganizationCtrl = $root.find("#VIS_Organization_" + $self.windowNo);
            divPeriodCtrl = $root.find("#VIS_Period_" + $self.windowNo);
            divIncludeSOCtrl = $root.find("#VIS_IncludeSO_" + $self.windowNo);
            divDocumentTypeCtrl = $root.find("#VIS_DocumentType_" + $self.windowNo);
            divIncludeOpenSalesOrderCtrl = $root.find("#VIS_IncludeOpenSalesOrder_" + $self.windowNo);
            divOpenSalesOrderCtrl = $root.find("#VIS_OpenSalesOrder_" + $self.windowNo);
            divIncludeOpportunityCtrl = $root.find("#VIS_IncludeOpportunity_" + $self.windowNo);
            divOpportunityCtrl = $root.find("#VIS_Opportunity_" + $self.windowNo);
            if (isMasterForecast) {
                divTeamForecastCtrl = $root.find("#VIS_TeamForecast_" + $self.windowNo);
            }
            if (isBudgetForecast) {
                divBudgetForecastCtrl = $root.find("#VIS_BudgetForecast_" + $self.windowNo);
                divSalesPriceListCtrl = $root.find("#VIS_PriceList_" + $self.windowNo);
            }
            divProductCategoryCtrl = $root.find("#VIS_ProductCategory_" + $self.windowNo);
            divBudgetQuantityCtrl = $root.find("#VIS_BudgetQuantity_" + $self.windowNo);
            divGenerateLines = $root.find("#VIS_GenerateLines_" + $self.windowNo);
        };

        /**Load Controls */
        function loadControls() {

            // Organization
            //ctx, windowNo, column_ID, AD_Reference_ID, columnName, AD_Reference_Value_ID, isParent, validationCode
            _OrganizationLookUp = VIS.MLookupFactory.get(ctx, $self.windowNo, AD_Org_ID, VIS.DisplayType.Search, "AD_Org_ID", 0, false, "AD_Org_ID.IsActive = 'Y'");
            // columnname, Mandatory, ReadOnly, Updateable, DisplayType, lookup
            _OrganizationCtrl = new VIS.Controls.VTextBoxButton("AD_Org_ID", true, false, false, VIS.DisplayType.Search, _OrganizationLookUp);
            var _OrgCtrlWrap = $('<div class="vis-control-wrap">');
            var _OrgBtnWrap = $('<div class="input-group-append">');
            divOrganizationCtrl.append(_OrgCtrlWrap);
            _OrgCtrlWrap.append(_OrganizationCtrl.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ')).append('<label>' + VIS.translatedTexts.AD_Org_ID + '</label>');
            divOrganizationCtrl.append(_OrgBtnWrap);
            _OrgBtnWrap.append(_OrganizationCtrl.getBtn(0));

            var validation = "C_Period.IsActive='Y' AND C_Period.C_Year_ID  IN " +
                " (Select C_Year_ID FROM C_Year WHERE C_Calendar_ID = " +
                " ((SELECT AD_OrgInfo.C_Calendar_ID FROM AD_OrgInfo WHERE AD_OrgInfo.AD_Org_ID = " + AD_Org_ID + ")) " +
                " OR (EXISTS(SELECT 1 FROM AD_OrgInfo WHERE AD_OrgInfo.AD_Org_ID = " + AD_Org_ID + " AND AD_OrgInfo.C_Calendar_ID IS NULL) AND " +
                " (C_Calendar_ID = (SELECT AD_ClientInfo.C_Calendar_ID FROM AD_ClientInfo WHERE AD_ClientInfo.AD_Client_ID = " + AD_Client_ID + "))) " +
                " AND C_year.IsActive = 'Y') and C_Period.Startdate < SYSDATE ORDER BY C_Period.C_Period_ID";
            _PeriodLookUp = VIS.MLookupFactory.get(ctx, $self.windowNo, 1004891, VIS.DisplayType.Search, "C_Period_ID", 0, false, validation);
            _PeriodCtrl = new VIS.Controls.VTextBoxButton("C_Period_ID", true, false, true, VIS.DisplayType.Search, _PeriodLookUp);
            var _PeriodCtrlWrap = $('<div class="vis-control-wrap">');
            var _PeriodBtnWrap = $('<div class="input-group-append">');
            divPeriodCtrl.append(_PeriodCtrlWrap);
            _PeriodCtrlWrap.append(_PeriodCtrl.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ')).append('<label>' + VIS.translatedTexts.C_Period_ID + '</label>');
            divPeriodCtrl.append(_PeriodBtnWrap);
            _PeriodBtnWrap.append(_PeriodCtrl.getBtn(0));
            _PeriodBtnWrap.append(_PeriodCtrl.getBtn(1));
            _PeriodCtrl.getControl().prop('disabled', true);
            _PeriodCtrl.getBtn(0).prop('disabled', true);
            _PeriodCtrl.getBtn(1).prop('disabled', true);

            ////columnName, mandatory, isReadOnly, isUpdateable, text, description, tableEditor
            _IncludeSOCtrl = new VIS.Controls.VCheckBox("IncludeSO", false, false, true, VIS.Msg.getMsg("IncludeSO"), null);
            var _IncludeSOCtrlWrap = $('<div class="vis-control-wrap">');
            divIncludeSOCtrl.append(_IncludeSOCtrlWrap);
            _IncludeSOCtrlWrap.append(_IncludeSOCtrl.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' '));


            validation = "C_DocType.IsActive = 'Y' AND C_DocType.DocBaseType IN ('SOO', 'POO', 'BOO') AND C_DocType.IsSOTrx = 'Y' AND " +
                " C_DocType.IsReturnTrx = 'N' AND  C_DocType.IsSalesQuotation = 'N' AND C_DocType.IsBlanketTrx = 'N' " +
                " AND C_DocType.AD_Org_ID IN (0, " + AD_Org_ID + ") ";
            _DocumentTypeLookUp = VIS.MLookupFactory.get(ctx, $self.windowNo, 2173, VIS.DisplayType.TableDir, "C_DocType_ID", 0, false, validation);
            _DocumentTypeCtrl = new VIS.Controls.VComboBox("C_DocType_ID", false, false, true, _DocumentTypeLookUp, 50);
            _DocumentTypeCtrl.getControl().prop('disabled', true);
            var _DocTypeCtrlWrap = $('<div class="vis-control-wrap">');
            divDocumentTypeCtrl.append(_DocTypeCtrlWrap);
            _DocTypeCtrlWrap.append(_DocumentTypeCtrl.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ')).append('<label>' + VIS.translatedTexts.C_DocType_ID + '</label>');

            _IncludeOpenSalesOrderCtrl = new VIS.Controls.VCheckBox("IncludeOpenSO", false, false, true, VIS.Msg.getMsg("IncludeOpenSO"), null);
            var _IncludeOpenSOCtrlWrap = $('<div class="vis-control-wrap">');
            divIncludeOpenSalesOrderCtrl.append(_IncludeOpenSOCtrlWrap);
            _IncludeOpenSOCtrlWrap.append(_IncludeOpenSalesOrderCtrl.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' '));

            validation = "C_Order.DocStatus IN ('CO') AND C_Order.IsSOTrx = 'Y' AND " +
                " C_Order.IsReturnTrx = 'N' AND  C_Order.IsSalesQuotation = 'N' AND C_Order.IsBlanketTrx = 'N' AND C_Order.AD_Org_ID IN (0, " + AD_Org_ID + ")";
            _OpenSalesOrderLookUp = VIS.MLookupFactory.get(ctx, $self.windowNo, 2213, VIS.DisplayType.MultiKey, "C_Order_ID", 0, false, validation);
            _OpenSalesOrderCtrl = new VIS.Controls.VTextBoxButton("C_Order_ID", true, false, true, VIS.DisplayType.MultiKey, _OpenSalesOrderLookUp);
            _OpenSalesOrderCtrl.getControl().prop('disabled', true);
            _OpenSalesOrderCtrl.getBtn(0).prop('disabled', true);
            _OpenSalesOrderCtrl.getBtn(1).prop('disabled', true);
            _OpenSalesOrderCtrl.setCustomInfo('Budget_OpenSalesOrder');
            var _OpenSOCtrlWrap = $('<div class="vis-control-wrap">');
            var _openSOBtnWrap = $('<div class="input-group-append">');
            divOpenSalesOrderCtrl.append(_OpenSOCtrlWrap);
            _OpenSOCtrlWrap.append(_OpenSalesOrderCtrl.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ')).append('<label>' + VIS.Msg.getMsg("OpenSO") + '</label>');
            divOpenSalesOrderCtrl.append(_openSOBtnWrap);
            _openSOBtnWrap.append(_OpenSalesOrderCtrl.getBtn(0));
            _openSOBtnWrap.append(_OpenSalesOrderCtrl.getBtn(1));

            _IncludeOpportunityCtrl = new VIS.Controls.VCheckBox("IncludeOpportunity", false, false, true, VIS.Msg.getMsg("IncludeOpportunity"), null);
            var _IncludeOpportunityCtrlWrap = $('<div class="vis-control-wrap">');
            divIncludeOpportunityCtrl.append(_IncludeOpportunityCtrlWrap);
            _IncludeOpportunityCtrlWrap.append(_IncludeOpportunityCtrl.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' '));


            validation = "C_Project.IsActive = 'Y' AND C_Project.IsOpportunity = 'Y' AND C_Project.AD_Org_ID IN (0, " + AD_Org_ID + ")";
            _OpportunityLookUp = VIS.MLookupFactory.get(ctx, $self.windowNo, 5766, VIS.DisplayType.MultiKey, "C_Project_ID", 0, false, validation);
            _OpportunityCtrl = new VIS.Controls.VTextBoxButton("C_Project_ID", true, false, true, VIS.DisplayType.MultiKey, _OpportunityLookUp);
            _OpportunityCtrl.getControl().prop('disabled', true);
            _OpportunityCtrl.getBtn(0).prop('disabled', true);
            _OpportunityCtrl.getBtn(1).prop('disabled', true);
            _OpportunityCtrl.setCustomInfo('Budget_Opportunity');
            var _OpportunityCtrlWrap = $('<div class="vis-control-wrap">');
            var _OpportunityBtnWrap = $('<div class="input-group-append">');
            divOpportunityCtrl.append(_OpportunityCtrlWrap);
            _OpportunityCtrlWrap.append(_OpportunityCtrl.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ')).append('<label>' + VIS.Msg.getMsg("Opportunity") + '</label>');
            divOpportunityCtrl.append(_OpportunityBtnWrap);
            _OpportunityBtnWrap.append(_OpportunityCtrl.getBtn(0));
            _OpportunityBtnWrap.append(_OpportunityCtrl.getBtn(1));

            if (isMasterForecast) {
                validation = " C_Forecast.AD_Org_ID IN (0, " + AD_Org_ID + ") AND C_Forecast.DocStatus IN ('CO', 'CL')";
                _TeamForecastLookUp = VIS.MLookupFactory.get(ctx, $self.windowNo, _TeamForecast_ID, VIS.DisplayType.MultiKey, "C_Forecast_ID", 0, false, validation);;
                _TeamForecastCtrl = new VIS.Controls.VTextBoxButton("C_Forecast_ID", true, false, true, VIS.DisplayType.MultiKey, _TeamForecastLookUp);
                _TeamForecastCtrl.setCustomInfo('Budget_TeamForecast');
                var _TeamForecastCtrlWrap = $('<div class="vis-control-wrap">');
                var _TeamForecastBtnWrap = $('<div class="input-group-append">');
                divTeamForecastCtrl.append(_TeamForecastCtrlWrap);
                _TeamForecastCtrlWrap.append(_TeamForecastCtrl.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ')).append('<label>' + VIS.translatedTexts.TeamForecast + '</label>');
                divTeamForecastCtrl.append(_TeamForecastBtnWrap);
                _TeamForecastBtnWrap.append(_TeamForecastCtrl.getBtn(0));
                _TeamForecastBtnWrap.append(_TeamForecastCtrl.getBtn(1));
            }

            if (isBudgetForecast) {
                validation = " C_MasterForecast.AD_Org_ID IN (0, " + AD_Org_ID + ") AND C_MasterForecast.DocStatus IN ('CO', 'CL')";
                _BudgetForecastLookUp = VIS.MLookupFactory.get(ctx, $self.windowNo, _budgetForecast_ID, VIS.DisplayType.MultiKey, "C_MasterForecast_ID", 0, false, validation);;
                _BudgetForecastCtrl = new VIS.Controls.VTextBoxButton("C_MasterForecast_ID", true, false, true, VIS.DisplayType.MultiKey, _BudgetForecastLookUp);
                var _BudgetForecastCtrlWrap = $('<div class="vis-control-wrap">');
                var _BudgetForecastBtnWrap = $('<div class="input-group-append">');
                divBudgetForecastCtrl.append(_BudgetForecastCtrlWrap);
                _BudgetForecastCtrlWrap.append(_BudgetForecastCtrl.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ')).append('<label>' + VIS.translatedTexts.MasterForecast + '</label>');
                divBudgetForecastCtrl.append(_BudgetForecastBtnWrap);
                _BudgetForecastBtnWrap.append(_BudgetForecastCtrl.getBtn(0));
                _BudgetForecastBtnWrap.append(_BudgetForecastCtrl.getBtn(1));
            }

            validation = "M_Product_Category.IsActive = 'Y' AND M_Product_Category.AD_Org_ID IN (0, " + AD_Org_ID + ")";
            _ProductCategoryLookUp = VIS.MLookupFactory.get(ctx, $self.windowNo, 2012, VIS.DisplayType.MultiKey, "M_Product_Category_ID", 0, false, validation);;
            _ProductCategoryCtrl = new VIS.Controls.VTextBoxButton("M_Product_Category_ID", true, false, true, VIS.DisplayType.MultiKey, _ProductCategoryLookUp);
            var _ProductCategoryCtrlWrap = $('<div class="vis-control-wrap">');
            var _ProductCategoryBtnWrap = $('<div class="input-group-append">');
            divProductCategoryCtrl.append(_ProductCategoryCtrlWrap);
            _ProductCategoryCtrlWrap.append(_ProductCategoryCtrl.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ')).append('<label>' + VIS.translatedTexts.M_Product_Category_ID + '</label>');
            divProductCategoryCtrl.append(_ProductCategoryBtnWrap);
            _ProductCategoryBtnWrap.append(_ProductCategoryCtrl.getBtn(0));
            _ProductCategoryBtnWrap.append(_ProductCategoryCtrl.getBtn(1));
            _ProductCategoryCtrl.fireValueChanged = function () {
                _ProductCategoryCtrl.getControl().trigger("change");
            };

            if (isBudgetForecast) {
                //Price list  Control
                validation = "M_PriceList.IsActive = 'Y' AND M_PriceList.IsSOPriceList='Y' AND M_PriceList.AD_Org_ID IN (0, " + AD_Org_ID + ")";
                _SalesPriceListLookUp = VIS.MLookupFactory.get(ctx, $self.windowNo, 2100, VIS.DisplayType.Search, "M_PriceList_ID", 0, false, validation);;
                _SalesPriceListCtrl = new VIS.Controls.VTextBoxButton("M_PriceList_ID", true, false, true, VIS.DisplayType.Search, _SalesPriceListLookUp);
                var SalesPriceListCtrlWrap = $('<div class="vis-control-wrap">');
                var _SalesPriceListBtnWrap = $('<div class="input-group-append">');
                divSalesPriceListCtrl.append(SalesPriceListCtrlWrap);
                SalesPriceListCtrlWrap.append(_SalesPriceListCtrl.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ')).append('<label>' + VIS.translatedTexts.M_PriceList_ID + '</label>');
                divSalesPriceListCtrl.append(_SalesPriceListBtnWrap);
                _SalesPriceListBtnWrap.append(_SalesPriceListCtrl.getBtn(0));
                _SalesPriceListBtnWrap.append(_SalesPriceListCtrl.getBtn(1));
                _SalesPriceListCtrl.getControl().prop('disabled', true);

            }

            _BudgetQuantityCtrl = new VIS.Controls.VAmountTextBox("BudgetQuantity", false, false, true, 50, 100, VIS.DisplayType.Quantity);
            _BudgetQuantityCtrl.getControl().prop('disabled', true);
            var _BudgetQuantityCtrlWrap = $('<div class="vis-control-wrap">');
            divBudgetQuantityCtrl.append(_BudgetQuantityCtrlWrap);
            _BudgetQuantityCtrlWrap.append(_BudgetQuantityCtrl.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ')).append('<label>' + VIS.Msg.getMsg("BudgetQuantity") + '</label>');


            //$formDataR = $('<div class="vis-budget-form-col2">');
            _GenerateLines = new VIS.Controls.VCheckBox("GenerateLines", false, false, true, VIS.Msg.getMsg("GenerateLines"), null);
            var _GenerateLinesCtrlWrap = $('<div class="vis-control-wrap">');
            divGenerateLines.append(_GenerateLinesCtrlWrap);
            _GenerateLinesCtrlWrap.append(_GenerateLines.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' '));


        };

        /** Events handling*/
        function bindEvents() {

            // set value on Organization control, if value is defined there
            _OrganizationCtrl.setValue(AD_Org_ID);

            _IncludeSOCtrl.getControl().on("click", function (ev) {
                // when Include Sales Order True, then make the document type as selectable
                _DocumentTypeCtrl.getControl().prop('disabled', _IncludeSOCtrl.getValue() ? false : true);
                _PeriodCtrl.getControl().prop('disabled', _IncludeSOCtrl.getValue() ? false : true);
                _PeriodCtrl.getBtn(0).prop('disabled', _IncludeSOCtrl.getValue() ? false : true);
                _PeriodCtrl.getBtn(1).prop('disabled', _IncludeSOCtrl.getValue() ? false : true);
                // when document type contain values, but user mark Include SO false after selection, then make it as null
                if (!_IncludeSOCtrl.getValue()) {
                    _DocumentTypeCtrl.setValue();
                    _PeriodCtrl.setValue();
                }

                // Disable Product Category, if include open SO selected as true and vice versa
                _ProductCategoryCtrl.getControl().trigger("change");

            });

            _IncludeOpenSalesOrderCtrl.getControl().on("click", function (ev) {
                // when Include Open Sales Order True, then make the Open Sales Order as selectable
                _OpenSalesOrderCtrl.getControl().prop('disabled', _IncludeOpenSalesOrderCtrl.getValue() ? false : true);
                _OpenSalesOrderCtrl.getBtn(0).prop('disabled', _IncludeOpenSalesOrderCtrl.getValue() ? false : true);
                _OpenSalesOrderCtrl.getBtn(1).prop('disabled', _IncludeOpenSalesOrderCtrl.getValue() ? false : true);

                // when Open Sales Order contain values, but user mark Include Open SO false after selection, then make it as null
                if (!_IncludeOpenSalesOrderCtrl.getValue()) {
                    _OpenSalesOrderCtrl.setValue();
                }

                // Disable Product Category, if include open SO selected as true and vice versa
                _ProductCategoryCtrl.getControl().trigger("change");

            });

            _IncludeOpportunityCtrl.getControl().on("click", function (ev) {
                // when Include Opportunity True, then make the Opportunity as selectable
                _OpportunityCtrl.getControl().prop('disabled', _IncludeOpportunityCtrl.getValue() ? false : true);
                _OpportunityCtrl.getBtn(0).prop('disabled', _IncludeOpportunityCtrl.getValue() ? false : true);
                _OpportunityCtrl.getBtn(1).prop('disabled', _IncludeOpportunityCtrl.getValue() ? false : true);

                // when Opportunity contain values, but user mark Include Opportunity false after selection, then make it as null
                if (!_IncludeOpportunityCtrl.getValue()) {
                    _OpportunityCtrl.setValue();
                }

                // Disable Product Category, if include open SO selected as true and vice versa
                _ProductCategoryCtrl.getControl().trigger("change");

            });

            _ProductCategoryCtrl.getControl().on("change", function (ev) {
                // Disable Product Category, if any checbox as true and vice versa
                _ProductCategoryCtrl.getControl().prop('disabled', (_IncludeOpenSalesOrderCtrl.getValue()
                    || _IncludeOpportunityCtrl.getValue() || _IncludeSOCtrl.getValue()) ? true : false);
                _ProductCategoryCtrl.getBtn(0).prop('disabled', (_IncludeOpenSalesOrderCtrl.getValue()
                    || _IncludeOpportunityCtrl.getValue() || _IncludeSOCtrl.getValue()) ? true : false);
                _ProductCategoryCtrl.getBtn(1).prop('disabled', (_IncludeOpenSalesOrderCtrl.getValue()
                    || _IncludeOpportunityCtrl.getValue() || _IncludeSOCtrl.getValue()) ? true : false);

                // clear Product Category Control if satisfied
                (_IncludeOpenSalesOrderCtrl.getValue() || _IncludeOpportunityCtrl.getValue() || _IncludeSOCtrl.getValue()) ? _ProductCategoryCtrl.setValue() : "";

                // when product category contains value then make the budget field as editabled and vice versa
                if (_ProductCategoryCtrl.getValue() != undefined || VIS.Utility.Util.getValueOfString(_ProductCategoryCtrl.getValue()) != "") {
                    _BudgetQuantityCtrl.getControl().prop('disabled', false);
                    _SalesPriceListCtrl.getControl().prop('disabled', false);

                }
                else {
                    _BudgetQuantityCtrl.getControl().prop('disabled', true);
                    _BudgetQuantityCtrl.getControl().val(0);
                    _SalesPriceListCtrl.getControl().prop('disabled', true);

                }
            });

            // Create Lines and close the form 
            $btnOk.on("click", function () {
                busyDiv(true);
                var TeamForecast_ID = 0;
                var MasterForecast_ID = 0;
                var SalesPriceList_ID = 0;
                if (isMasterForecast) {
                    TeamForecast_ID = _TeamForecastCtrl.getValue();
                }

                if (isBudgetForecast) {
                    MasterForecast_ID = _BudgetForecastCtrl.getValue();
                    SalesPriceList_ID = _SalesPriceListCtrl.getValue();
                }
                // Incase of Product category BudgetQuantity should be 
                if (_ProductCategoryCtrl.getValue() != undefined || VIS.Utility.Util.getValueOfString(_ProductCategoryCtrl.getValue()) != "") {
                    if (VIS.Utility.Util.getValueOfDecimal(_BudgetQuantityCtrl.getValue()) == 0) {
                        VIS.ADialog.info("SelectBudgetQunatity");
                        busyDiv(false);
                        return false;
                    }
                    if (isBudgetForecast && VIS.Utility.Util.getValueOfDecimal(_SalesPriceListCtrl.getValue()) == 0) {
                        VIS.ADialog.info("SelectPriceList");
                        busyDiv(false);
                        return false;
                    }
                }

                GenerateForecastLines(TeamForecast_ID, MasterForecast_ID, SalesPriceList_ID, function (result) {
                    VIS.ADialog.info("", "", JSON.parse(result));
                    busyDiv(false);
                    // close Form
                    $self.frame.close();
                });             
            });

            //Create Lines but donot close form 
            $btnApply.on("click", function () {
                busyDiv(true);
                var TeamForecast_ID = 0;
                var MasterForecast_ID = 0;
                var SalesPriceList_ID = 0;

                if (isMasterForecast) {
                    TeamForecast_ID = _TeamForecastCtrl.getValue();
                }
                if (isBudgetForecast) {
                    MasterForecast_ID = _BudgetForecastCtrl.getValue();
                    SalesPriceList_ID = _SalesPriceListCtrl.getValue();
                }
                // Incase of Product category BudgetQuantity should be 
                if (_ProductCategoryCtrl.getValue() != undefined || VIS.Utility.Util.getValueOfString(_ProductCategoryCtrl.getValue()) != "") {
                    if (VIS.Utility.Util.getValueOfDecimal(_BudgetQuantityCtrl.getValue()) == 0) {
                        VIS.ADialog.info("SelectBudgetQunatity");
                        busyDiv(false);
                        return false;
                    }
                    if (isBudgetForecast && VIS.Utility.Util.getValueOfDecimal(_SalesPriceListCtrl.getValue()) == 0) {
                        VIS.ADialog.info("SelectPriceList");
                        busyDiv(false);
                        return false;
                    }
                }

                GenerateForecastLines(TeamForecast_ID, MasterForecast_ID, SalesPriceList_ID, function(result) {
                    VIS.ADialog.info("", "", JSON.parse(result));
                    busyDiv(false);
                });

            })
            // Close Form
            $btnCancel.on("click", function () {
                $self.frame.close();
            });
        };

      
        /** Generate Team forecast lines */
        function GenerateForecastLines(TeamForecast_ID, MasterForecast_ID, SalesPriceList_ID,callback) {      
            $.ajax({
                url: VIS.Application.contextUrl + "ForecastForm/CreateForecastLine",
                type: "POST",
                dataType: "json",
                async: true,
                //  contentType: "application/json; charset=utf-8",
                data: {
                    Org_ID: _OrganizationCtrl.getValue(),
                    Period_ID: _PeriodCtrl.getValue(),
                    IncludeSO: _IncludeSOCtrl.getValue(),
                    DocType: _DocumentTypeCtrl.getValue(),
                    IncludeOpenSO: _IncludeOpenSalesOrderCtrl.getValue(),
                    OpenOrders: _OpenSalesOrderCtrl.getValue(),
                    IncludeOpportunity: _IncludeOpportunityCtrl.getValue(),
                    Opportunity: _OpportunityCtrl.getValue(),
                    ProductCategory: _ProductCategoryCtrl.getValue(),
                    BudgetQunatity: _BudgetQuantityCtrl.getValue(),
                    DeleteAndGenerateLines: _GenerateLines.getValue(),
                    Forecast_ID: $self.record_ID,
                    TeamForecast_IDS: TeamForecast_ID,
                    Table_ID: $self.ad_table_ID,
                    IsMasterForecast: isMasterForecast,
                    IsBudgetForecast: isBudgetForecast,
                    MasterForecast_IDs: MasterForecast_ID,
                    SalesPriceList_ID: SalesPriceList_ID
                },
                success: function (result) {
                    callback(result);           
                },
                error: function (ex) {
                    console.log(ex);
                    busyDiv(false);
                    VIS.ADialog.error("Error");
                }
            });
        }      
            
       
        /** Busy Indicator */
        function busyDiv(Value) {
            if (Value) {
                $bsyDiv[0].style.visibility = 'visible';
            }
            else {
                $bsyDiv[0].style.visibility = 'hidden';
            }
        };

        this.getRoot = function () {
            return $root;
        };
      
        /** Dispose Components/Variables */
        this.disposeComponent = function () {
            $self = null;
            $root = null;

            $bsyDiv = null;
            $mainpageContent = null;
            $formWrap = null;
            $formDataRow = null;
            $formData = null;
            $actionPanel = null;
            $buttons = null;
            $btnOk = null;
            $btnCancel = null;

            divOrganizationCtrl = null;
            _OrganizationLookUp = null;
            _OrganizationCtrl = null;

            divPeriodCtrl = null;
            _PeriodLookUp = null;
            _PeriodCtrl = null;

            divIncludeSOCtrl = null;
            _IncludeSOCtrl = null;

            divDocumentTypeCtrl = null;
            _DocumentTypeLookUp = null;
            _DocumentTypeCtrl = null;

            divIncludeOpenSalesOrderCtrl = null;
            _IncludeOpenSalesOrderCtrl = null;

            divOpenSalesOrderCtrl = null;
            _OpenSalesOrderLookUp = null;
            _OpenSalesOrderCtrl = null;

            divIncludeOpportunityCtrl = null;
            _IncludeOpportunityCtrl = null;

            divOpportunityCtrl = null;
            _OpportunityLookUp = null;
            _OpportunityCtrl = null;

            divTeamForecastCtrl = null;
            _TeamForecastLookUp = null;
            _TeamForecastCtrl = null;

            divProductCategoryCtrl = null;
            _ProductCategoryLookUp = null;
            _ProductCategoryCtrl = null;

            divBudgetQuantityCtrl = null;
            _BudgetQuantityCtrl = null;

            divGenerateLines = null;
            _GenerateLines = null;
        };


        /** Intialize Form
         * @param {any} windowNo
         * @param {any} frame
         */
        createforecast.prototype.init = function (windowNo, frame) {

            this.frame = frame;
            this.windowNo = windowNo;
            this.ad_table_ID = this.frame.getAD_Table_ID();
            this.ad_window_ID = this.frame.getAD_Window_ID();
            this.record_ID = this.frame.getRecord_ID();
            this.Initialize();
            this.frame.getContentGrid().append(this.getRoot());
        };

        /** Must implement dispose */
        createforecast.prototype.dispose = function () {
            /*CleanUp Code */
            //dispose this component
            this.disposeComponent();

            //call frame dispose function
            this.frame = null;
        };

    };
    VIS.AForms.createforecast = createforecast;
})(VIS, jQuery);
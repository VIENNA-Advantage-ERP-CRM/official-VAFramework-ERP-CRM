
/********************************************************
 * Module Name    :     Application
 * Purpose        :     GL Dimension Value.
 * Author         :     Amit Bansal
 * Date           :     03-Oct-2019
  ******************************************************/
; (function (VIS, $) {
    VIS.AForms = VIS.AForms || {};
    function glDimensionValue() {

        this.frame;
        this.windowNo;
        var $self = this;
        var $root = $("<div style='height:100%;background-color:white;' class='vis-forms-container' >");
        //var $bsyDiv = $("<div class='vis-apanel-busy' style='height:100%;position:relative;z-index: 2'>");
        var $bsyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
        var ctx = VIS.Env.getCtx();
        var $mainpageContent = null;
        var $formWrap = null;
        var $formDataRow = null;
        var $formData = null;

        // label value of user element column
        var $lblUserElement = null;
        // dimension value of user element
        var $infoUserElement = null;

        var $actionPanel = null;
        var $buttons = null;
        var $btnOk = null;
        var $btnCancel = null;

        this.Initialize = function () {
            busyDiv(true);
            createMainView();
            bindEvents();
            busyDiv(false);
        };

        /** Set Height of Form */
        this.setHeight = function () {
            return 235;
        };

        /** set width of form */
        this.setWidth = function () {
            return 368;
        };

        /** Is used to design GL Dimension Value Form */
        function createMainView() {
            $mainpageContent = $('<div class="vis-gl-main-wrap">');

            // get User Element Column ID
            var paramStr = VIS.context.getWindowContext($self.windowNo, "AD_Column_ID", true);
            if (paramStr == "") {
                VIS.ADialog.error("VIS_DimensionColNotSelected", true, "", "");
                return false;
            }

            // get User Element Column Name
            var columnName = VIS.dataContext.getJSONRecord("GlJournall/ColumnName", paramStr);

            // Row 1
            $formWrap = $('<div class="vis-gl-form-wrap">');
            $formDataRow = $('<div class="vis-gl-form-row">');

            $formData = $('<div class="vis-gl-form-col">');
            $lblUserElement = $('<label>' + VIS.Msg.translate(ctx, columnName) + '</label>');
            $formData.append($lblUserElement);

            $formDataR = $('<div class="vis-gl-form-col2">');
            var valueInfo = VIS.MLookupFactory.getMLookUp(ctx, $self.windowNo, VIS.Utility.Util.getValueOfString(paramStr), VIS.DisplayType.Search);
            $infoUserElement = new VIS.Controls.VTextBoxButton(columnName, false, false, true, VIS.DisplayType.Search, valueInfo);
            $formDataR.append($('<div>').append($infoUserElement).append($infoUserElement.getControl()).append($infoUserElement.getBtn(0)));

            $formDataRow.append($formData).append($formDataR);
            $formWrap.append($formDataRow);

            //Action 
            $actionPanel = $('<div class="VIS_buttons-wrap .vis-gl-buttons-wrap">');

            $buttons = $('<div class="pull-right">');
            $btnOk = $('<span class="btn vis-gl-buttons-wrap-span">' + VIS.Msg.translate(ctx, "VIS_OK") + '</span>');
            $btnCancel = $('<span class="btn vis-gl-buttons-wrap-span">' + VIS.Msg.translate(ctx, "Cancel") + '</span>');
            $buttons.append($btnOk).append($btnCancel);
            $actionPanel.append($buttons);
            $mainpageContent.append($formWrap).append($actionPanel);
            $root.append($mainpageContent).append($bsyDiv);
        };

        /** Events handling*/
        function bindEvents() {

            // set value on Dimenssion control, if value is defined there
            var DimensionId = VIS.context.getWindowContextAsInt($self.windowNo, "DimensionValue", true);
            if (DimensionId > 0) {
                $infoUserElement.setValue(DimensionId);
            }

            // Set Dimenssion value
            $btnOk.on("click", function () {

                busyDiv(true);

                if ($infoUserElement.getValue() == null) {
                    busyDiv(false);
                    VIS.ADialog.error("VIS_DimensionNotSelected", true, "", "");
                    return false;
                };

                // Get Record Id
                var recordId = VIS.context.getWindowContext($self.windowNo, "GL_LineDimension_ID", true);

                // Get Selecetd Dimenssion value
                var paramStr = recordId.toString().concat(",", $infoUserElement.getValue().toString());

                // Update record with Dimenssion Value
                var result = VIS.dataContext.getJSONRecord("GlJournall/SaveDimensionReference", paramStr);
                if (result <= 0) {
                    busyDiv(false);
                    VIS.ADialog.error("VIS_NotSaved", true, "", "");
                    return false;
                }
                else {
                    VIS.ADialog.info("VIS_Saved", true, "", "");
                }

                busyDiv(false);

                // close Form
                $self.frame.close();

            });

            // Close Form
            $btnCancel.on("click", function () {
                $self.frame.close();
            });
        };

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
            $lblUserElement = null;
            $infoUserElement = null;
            $actionPanel = null;
            $buttons = null;
            $btnOk = null;
            $btnCancel = null;
        };
    };

    /** Intialize Form
     * @param {any} windowNo
     * @param {any} frame
     */
    glDimensionValue.prototype.init = function (windowNo, frame) {

        this.frame = frame;
        this.windowNo = windowNo;
        this.Initialize();
        this.frame.getContentGrid().append(this.getRoot());
    };

    /** Must implement dispose */
    glDimensionValue.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        this.frame = null;
    };


    VIS.AForms.glDimensionValue = glDimensionValue;
})(VIS, jQuery);
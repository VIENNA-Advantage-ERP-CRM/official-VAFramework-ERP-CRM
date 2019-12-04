
; (function (VIS, $) {
    VIS.Apps = VIS.Apps || {};

    /*	Form Interface.
    	for communicating between AFrame and user Form
      
      Every form must Implement these two function 
    
         1.  'form'.prototype.init = function (WindowNo, frame);
         2.  'form'.prototype.dispose = function();
      *   'form' = User Form 
      see TestForm Exemple below
   */

    //Form Class function fullnamespace
    VIS.Apps.TestForm = function () {
        this.frame;
        this.windowNo;

        var $root, $text, $okBtn, $cancelBtn;
        var parameterdiv, reportDiv, toolBar, busyIndicator;
        var cmbReports;
        var wind;
        var cPanel = null;
        var self = this;

        function initializeComponent() {
            $root = $("<div class='vis-height-full'>");
            $text = $("<input type='text'>");
            $okBtn = $("<input type='button'>").val(VIS.Msg.getMsg("OK"));
            $cancelBtn = $("<input type='button'>").val(VIS.Msg.getMsg("Close"));

            //var $h2 = $("<h2>").text("Test Form");
            //var $h1 = $("<h3>").text("enter text in box and click ok button");
            //Add to root
            //append($h2).append($h1) .
            //$root.append($('<div class="vis-awindow-header vis-menuTitle"><a href="javascript:void(0)" class="vis-mainMenuIcons vis-icon-menuclose"></a><p>Workspace  SuperUser@HQ.IdeasInc.</p><div class="vis-awindow-toolbar"></div></div>')).append( $text).append($okBtn).append($cancelBtn);
            cmbReports = $('<select style="width:300px;height:30px">');
            parameterdiv = $('<div style="width:300px;height:100%;float:left;background-color:gray">');
            reportDiv = $('<div style="width:calc(100% - 400px);height:calc(100% - 35px);float:right;background-color:yellow">');
            toolBar = $('<div style="width:calc(100% - 300px);height:35px;float:right;background-color:brown">');
            busyIndicator = $('<div style="width:calc(100% - 400px);height:calc(100%);position:absolute">');



            $root.append(cmbReports).append(toolBar).append(parameterdiv).append(reportDiv);



            $.ajax({
                url: VIS.Application.contextUrl + "home/GetReports",
                success: function (data) {
                    data = JSON.parse(data);
                    if (data) {
                        for (var i = 0; i < data.length; i++) {
                            cmbReports.append('<option value="' + data[i].Key + '">' + data[i].Name + '</option>');
                        }
                    }
                }
            });

            cmbReports.on("change", function () {

                if (!wind) {
                    wind = new VIS.AWindow();
                }
                cPanel = wind.refreshProcess(cmbReports.val(), null, "P", true, self);
                cPanel.showCloseIcon(false);
                cPanel.setSaveCsvIcons('vis-savecsv-ico-pos');
                cPanel.setReportFormatIcons('vis-repformat-ico-pos');
                cPanel.setSavePdfIcons('vis-savepdf-ico-pos');
                cPanel.setArchiveIcons('vis-archive-ico-pos');
                cPanel.setRequeryIcons('vis-requery-ico-pos');
                cPanel.setCustomizeIcons('vis-customize-ico-pos');
                cPanel.setPrintIcons('vis-print-ico-pos');
                cPanel.setToolbarColor('white');
                cPanel.showParameterCloseIcon(false);
            });



        }

        initializeComponent();

        var self = this; //scoped self pointer

        //Event

        $okBtn.on(VIS.Events.onTouchStartOrClick, function () {
            alert("Test From has value :==> " + $text.val());
        });

        $cancelBtn.on(VIS.Events.onTouchStartOrClick, function () {
            if (confirm("wanna close this form???"))
                self.dispose(); // 
        });

        //Privilized function
        this.getRoot = function () {
            return $root;
        };

        this.getParameterContainer = function () {
            return parameterdiv;
        }

        this.getContentContainer = function () {
            return reportDiv;
        }

        this.getToolbarContainer = function () {
            return toolBar;
        };


        this.getBusyIndicatorContainer = function ()
        {
            return busyIndicator;
        };

        this.disposeComponent = function () {

            self = null;
            if ($root)
                $root.remove();
            $root = null;

            if ($okBtn)
                $okBtn.off(VIS.Events.onTouchStartOrClick);
            if ($cancelBtn)
                $cancelBtn.off(VIS.Events.onTouchStartOrClick);

            $text = $okBtn = $cancelBtn = null;

            this.getRoot = null;
            this.disposeComponent = null;
        };


    };

    //Must Implement with same parameter
    VIS.Apps.TestForm.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        // frame.hideHeader(true);

        this.frame.getContentGrid().append(this.getRoot());
    };

    VIS.Apps.TestForm.prototype.sizeChanged = function (height, width) {

    };

    //Must implement dispose
    VIS.Apps.TestForm.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };


    VIS.Apps.TestForm.prototype.show = function () {
        var c = new VIS.CFrame();
        c.setName(VIS.Msg.getMsg("Tect"));
        c.setTitle(VIS.Msg.getMsg("Test"));
        c.hideHeader(true);
        c.setContent(this);
        c.show();
    }


})(VIS, jQuery);
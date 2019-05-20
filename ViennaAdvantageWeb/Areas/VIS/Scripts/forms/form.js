
; (function (VIS, $) {
    VIS.Apps = VIS.Apps ||{};

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

        function initializeComponent() {
            $root = $("<div class='vis-height-full'>");
            $text = $("<input type='text'>");
            $okBtn = $("<input type='button'>").val(VIS.Msg.getMsg("OK"));
            $cancelBtn = $("<input type='button'>").val(VIS.Msg.getMsg("Close"));

            //var $h2 = $("<h2>").text("Test Form");
            //var $h1 = $("<h3>").text("enter text in box and click ok button");
            //Add to root
            //append($h2).append($h1) .
            $root.append($('<div class="vis-awindow-header vis-menuTitle"><a href="javascript:void(0)" class="vis-mainMenuIcons vis-icon-menuclose"></a><p>Workspace  SuperUser@HQ.IdeasInc.</p><div class="vis-awindow-toolbar"></div></div>')).append( $text).append($okBtn).append($cancelBtn);
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
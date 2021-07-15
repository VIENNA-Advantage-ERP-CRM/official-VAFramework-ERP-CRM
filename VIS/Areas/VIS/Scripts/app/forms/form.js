
; (function (VIS, $) {

    /*	Form Interface.
    	for communicating between AFrame and user Form
      
      Every form must Implement these two function 
    
         1.  'form'.prototype.init = function (WindowNo, frame);
         2.  'form'.prototype.dispose = function();
      *   'form' = User Form 

      Optional functions
       1. 'form'.prototype.sizeChanged = function(height,width)  fire when screen size changed
       2. 'form'.prototype.refresh = function()  fire when form got focus
      



      see TestForm Exemple below
   */

    //Form Class function fullnamespace
    VIS.TestForm = function () {
        this.frame;
        this.windowNo;

        var $root, $text, $okBtn, $cancelBtn;

        function initializeComponent() {
            $root = $("<div>");
            $text = $("<input type='text'>");
            $okBtn = $("<input type='button'>").val("Show");
            $cancelBtn = $("<input type='button'>").val("Close");

            //Add to root
            $root.append($text).append($okBtn).append($cancelBtn);
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
    VIS.TestForm.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        this.frame.getContentGrid().append(this.getRoot());
    };

    //Must implement dispose
    VIS.TestForm.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };



   /* Optional functions
    3. server side message handling 
*/
    //Form Class function fullnamespace
    VIS.TestMsgForm = function () {
        this.frame;
        this.windowNo;

        var $root, $text, $okBtn, $cancelBtn;

        function initializeComponent() {
            $root = $("<div>");
            $text = $("<input type='text'>");
            $okBtn = $("<input type='button'>").val("Start");
            $cancelBtn = $("<input type='button' disable>").val("Stop");

            //Add to root
            $root.append($text).append($okBtn).append($cancelBtn);
        }
        initializeComponent();


        var self = this; //scoped self pointer

        //Event

        $okBtn.on(VIS.Events.onTouchStartOrClick, function () {
            self.sendRequest(false);
            changeUI(false, "started");
        });

        $cancelBtn.on(VIS.Events.onTouchStartOrClick, function () {
            self.sendRequest(true);
            changeUI(true,"Stopped");
        });

        //Privilized function
        this.getRoot = function () {
            return $root;
        };

        function changeUI(stop, text) {
            if (stop) {
                $cancelBtn.hide();
                $okBtn.show();
                $text.css('color','red');
            }
            else {
                $cancelBtn.show();
                $okBtn.hide();
                $text.css('color','green');
            }
            $text.val(text);
        }

        this.onmessage = function (data) {
        /* Event should be unique */
            if (data.Event == "DEMOS") {
                changeUI(false, data.Message);
            }
            else if (data.Event == "DEMOE") {
                changeUI(true, data.Message);
            }

            //elseskip
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
    VIS.TestMsgForm.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        this.frame.getContentGrid().append(this.getRoot());
        //register
        VIS.sseManager.register(this);
    };


    VIS.TestMsgForm.prototype.sendRequest = function (isStop) {
        VIS.dataContext.postJSONData(VIS.Application.contextUrl + 'Message/Demo', { 'IsStop': isStop }, function (e) {
        });
    };

    //Must implement dispose
    VIS.TestMsgForm.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
        //unregister
        VIS.sseManager.unregister(this);
    };

    /* another way */
    //VIS.TestMsgForm.prototype.onmessage = function (data) {
        

    //};

})(VIS, jQuery);
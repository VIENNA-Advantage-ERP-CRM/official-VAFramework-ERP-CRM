; (function (VIS, $) {

    function SSEMgr() {

        var _observers = [];



        function register(source) {
            if (_observers.indexOf(source) < 0) {
                _observers.push(source);
            }
        }

        function unregister(source) {
            var index = _observers.indexOf(source);
            if (index > -1) {
                _observers.splice(index, 1);
            }
        }

        function notify(evtData) {
            for (var i = 0, j = _observers.length; i < j; i++) {
                if (_observers[i].onmessage)
                    _observers[i].onmessage(evtData);
            }
        };


        function start() {
            //var source = new EventSource('Channel/MsgForToastr?varificationToken=' + $("#vis_antiForgeryToken").val());
            var source = new EventSource('Message/Get');
            source.onmessage = function (e) {

                var returnedItem = JSON.parse(e.data);
                if (returnedItem && returnedItem.length > 0) {
                    for (var i = 0, j = returnedItem.length; i < j; i++) {
                        if (returnedItem[i].Event == "MSG") {
                            toastr.success(returnedItem[i].Message, '', { timeOut: 4000, "positionClass": "toast-top-center", "closeButton": true, });
                        }
                        else if (returnedItem[i].Event == "LOGIN") {
                            //toastr.success(returnedItem[i].Message, '', { timeOut: 4000, "positionClass": "toast-top-center", "closeButton": true, });
                        }
                        else if (returnedItem[i].Event == "LOGOFF") {
                            //toastr.success(returnedItem[i].Message, '', { timeOut: 4000, "positionClass": "toast-top-center", "closeButton": true, });
                        }
                        else
                            notify(returnedItem[i]);
                    }
                }
            };
        }

        return {
            start: start,
            register: register,
            unregister: unregister
        };
    };

    VIS.sseManager = SSEMgr();




    function UserMgr() {


    }

})(VIS, jQuery);
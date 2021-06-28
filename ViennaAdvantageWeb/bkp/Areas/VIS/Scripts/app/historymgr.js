// Should be executed BEFORE any hash change has occurred.
;(function (namespace, VIS) { // Closure to protect local variable "var hash"
    var historyMgr = namespace.historyMgr || {};


    /**
     *  Add replaceState function to historyMgrObject
     *
     */
    if ('replaceState' in history) { // Yay, supported!
        historyMgr.replaceHash = function (newhash) {
            if (('' + newhash).charAt(0) !== '#') newhash = '#' + newhash;
            history.replaceState('', '', newhash);
        }
    } else {
        var hash = location.hash;
        historyMgr.replaceHash = function (newhash) {
            if (location.hash !== hash) history.back();
            location.hash = newhash;
        };
    }

    /* update url string after hash */
    historyMgr.updateHistory = function (newEntry) {
        historyMgr.replaceHash(newEntry);
    };

    //historyMgr.removeHistory = function (entry) {
    //    var loc = historyMgr.geHashtLocation();
    //    loc = loc.replace(entry, "");
    //    historyMgr.replaceHash(loc);
    //};

    historyMgr.restoreHistory = function () {
        var loc = historyMgr.geHashtLocation();
        VIS.viewManager.restoreActions(loc);
    };

    /**
     *  Get Hash Location
     *
     */
    historyMgr.geHashtLocation = function () {
        var h = location.hash;
        if (h.length > 1)
            return h.substring(1);
        return "";
    }

    namespace.historyMgr = historyMgr; //global Assignment

})(window, VIS);

// This function can be namespaced. In this example, we define it on window:

//var hash = window.geHashtLocation();

//window.replaceHash(hash + ',Newhashvariable');
//hash = window.geHashtLocation();

//window.replaceHash(hash+',variable');
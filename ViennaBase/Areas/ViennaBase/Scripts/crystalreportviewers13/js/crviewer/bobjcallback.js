function bobj_WebForm_Callback (viewerID, callbackEventArgument, formID) {
    if (!viewerID || !formID) {
        return;
    }
    
    var frm = document.getElementById(formID); // get the form by using viewerID
    
    if (!frm) {
        return;
    }
    
    var strArr = [];
    for (var i = 0, itemCount = frm.elements.length; i < itemCount; i++) {
        var elem = frm.elements[i];
        if (elem.name && elem.value) {
            strArr.push(elem.name);
            strArr.push('=');
            strArr.push(encodeURIComponent(elem.value));
            strArr.push('&');
        }
    }
    
    strArr.push('__BOBJ_CALLBACK_EVENTTARGET=');
    strArr.push(encodeURIComponent(viewerID));
    strArr.push('&__BOBJ_CALLBACK_EVENTARGUMENT=');
    strArr.push(encodeURIComponent(callbackEventArgument));
    var qryString = strArr.join('');
    
    var req = MochiKit.Async.getXMLHttpRequest();
    req.open("POST", frm.action, true);
    req.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    req.setRequestHeader('Accept','application/json');
    return MochiKit.Async.sendXMLHttpRequest(req, qryString);
}
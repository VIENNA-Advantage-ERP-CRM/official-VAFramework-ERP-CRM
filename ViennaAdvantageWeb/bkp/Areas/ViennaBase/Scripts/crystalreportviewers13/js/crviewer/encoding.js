/* Copyright (c) Business Objects 2006. All rights reserved. */

if (typeof bobj == 'undefined') {
    bobj = {};    
}

bobj.encodeUTF8 = function(string) {
    var arr = [];
    var strLen = string.length;
    for(var i = 0; i < strLen; i++) {
        var c = string.charCodeAt(i);
        if(c < 0x80) {
            arr.push(c);
        }
        else if(c < 0x0800) {
            arr.push((c >> 6) | 0xc0);
            arr.push(c & 0x3f | 0x80);
        }
        else if(c < 0xd800 || c >= 0xe000) {
            arr.push((c >> 12) | 0xe0);
            arr.push((c >> 6) & 0x3f | 0x80);
            arr.push(c & 0x3f | 0x80);
        }
        else if(c < 0xdc00) {
            var c2 = string.charCodeAt(i + 1);
            if(isNaN(c2) || c2 < 0xdc00 || c2 >= 0xe000) {
                arr.push(0xef, 0xbf, 0xbd);
                continue;
            }
            i++;
            val = ((c & 0x3ff) << 10) | (c2 & 0x3ff);
            val += 0x10000;
            arr.push((val >> 18) | 0xf0);
            arr.push((val >> 12) & 0x3f | 0x80);
            arr.push((val >> 6) & 0x3f | 0x80);
            arr.push(val & 0x3f | 0x80);
        }
        else {
            arr.push(0xef, 0xbf, 0xbd);
        }
    }
    return arr;
};

bobj.encodeBASE64 = function(byteArray) {
    var keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    var arr = [];
    var c1, c2, c3, e1, e2, e3, e4;
    var i = 0, arrLen = byteArray.length;
    
    while(i < arrLen) {
        c1 = byteArray[i++];
        c2 = byteArray[i++];
        c3 = byteArray[i++];
        
        e1 = c1 >> 2;
        e2 = ((c1 & 3) << 4) | (c2 >> 4);
        e3 = ((c2 & 15) << 2) | (c3 >> 6);
        e4 = c3 & 63;
        
        if (isNaN(c2)) {
            e3 = e4 = 64;
        } else if(isNaN(c3)) {
            e4 = 64;
        }
        arr.push(keyStr.charAt(e1));
        arr.push(keyStr.charAt(e2));
        arr.push(keyStr.charAt(e3));
        arr.push(keyStr.charAt(e4));
    }
    return arr.join('');
};
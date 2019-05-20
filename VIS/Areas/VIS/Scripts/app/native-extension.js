/// <reference path="native-extension.js" />

/*******************************************************
//                  STRING                             //
//*****************************************************/
;String.prototype.equals = function (value, ignoreCase) {

    if (ignoreCase) {
       return  this.toString().toLowerCase() === value.toLowerCase();
    }
    return this.toString() === value;
};

String.prototype.equalsIgnoreCase = function (value) {
        return this.toString().toLowerCase() === value.toLowerCase();
};

String.prototype.startsWith = function (starts) {
    if (starts === '') return true;
    if (this == null || starts == null) return false;
    starts = String(starts);
    return this.length >= starts.length && this.slice(0, starts.length) === starts;
};

String.prototype.endsWith = function (ends) {
    if (ends === '') return true;
    if (this == null || ends == null) return false;
    ends = String(ends);
    return this.length >= ends.length && this.slice(this.length - ends.length) === ends;
};

String.prototype.contains = function (val) {
    return this.indexOf(val) > -1;
};

String.prototype.toUpper = function () {
    return this.toUpperCase();
};

String.prototype.toLower = function () {
    return this.toLowerCase();
};

String.prototype.compareTo = function (s) {

    var len1 = this.length;
    var len2 = s.length;
    var n = (len1 < len2 ? len1 : len2);

    for (i = 0 ; i < n ; i++) {
        var a = this.charCodeAt(i);
        var b = s.charCodeAt(i)
        if (a != b) {
            if (a > b) {
                return 1;
            } else if (a < b) {
                return -1;
            }
            else { return 0; }
        }
    }
    var r = len1 - len2;
    if (r > 0) { return 1; }
    else if (r < 0) { return -1 }
    else return 0;
};

String.prototype.replaceAll = function (find, replace) {
    var str = this;
    return str.replace(new RegExp(find, 'g'), replace);
};



/*******************************************************
//                 NUMBER                             //11
//*****************************************************/


Number.prototype.compareTo = function (n) {
    var r = this - n;
    if (r > 0) { return 1; }
    else if (r < 0) { return -1 }
    else return 0;
};




/*******************************************************
//                  Array                             //
//*****************************************************/

if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (needle) {
        for (var i = 0; i < this.length; i++) {
            if (this[i] === needle) {
                return i;
            }
        }
        return -1;
    };
}

//Array.prototype.contains = function (obj) {
//    return this.indexOf(obj) > -1;
//}

//Array.prototype.clear = function () {
//    this.length = 0;
//};







/*******************************************************
//              STRINGBUILDER                         //
//*****************************************************/
StringBuilder = function (init) {
    if (init)
        this.str = init;
    else
        this.str = "";
};

StringBuilder.prototype.append = function (value) {
    this.str += value;
    return this;
};

StringBuilder.prototype.toString = function () {
    //var value = this.str;
    //this.str.length = 0;
    //this.str = "";
    return this.str;
};

StringBuilder.prototype.endsWith = function (end) {
    return this.str.endsWith(end);
};

StringBuilder.prototype.replace = function (find, replace) {
    var str = this.str;
    str = str.replace(new RegExp(find, 'g'), replace);
    return str;
};

StringBuilder.prototype.indexOf = function (val) {
    return this.str.indexOf(val);
}

StringBuilder.prototype.length = function () {
    return this.str.length;
};

StringBuilder.prototype.insert = function (index, string) {
    if (index > 0)
        this.str =  this.str.substring(0, index) + string + this.str.substring(index, this.str.length);
    else
        this.str = string + this.str;
    return str;
};

StringBuilder.prototype.clear = function () {
    this.str.length = 0;
    this.str = "";
    return this;
};







/*******************************************************
//                    Date                         //
//*****************************************************/
Date.prototype.toUniversalTime = function () {
    return new Date(this.getTime());
};
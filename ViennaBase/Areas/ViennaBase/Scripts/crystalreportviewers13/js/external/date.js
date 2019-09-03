// ===================================================================
// Original Author: Matt Kruse <matt@mattkruse.com>
// WWW: http://www.mattkruse.com/
//
// NOTICE: You may use this code for any purpose, commercial or
// private, without any further permission from the author. You may
// remove this notice from your final code if you wish, however it is
// appreciated by the author if at least my web site address is kept.
//
// You may *NOT* re-distribute this code in any way except through its
// use. That means, you can include it in your product, or your web
// site, or any other form where the code is actually being used. You
// may not put the plain javascript up on your site for download or
// include it in your javascript libraries for download. 
// If you wish to share this code with others, please just point them
// to the URL instead.
// Please DO NOT link directly to my .js files from your site. Copy
// the files to your server and use them there. Thank you.
// ===================================================================

// HISTORY
// ------------------------------------------------------------------
// Jul 23, 2007: Add capital Y and D for Year and Date formats
// Dec 19, 2006: Replaced calls to getYear() with getFullYear()
// Dec 19, 2006: Removed parseDate() function
// Dec 19, 2006: Modified getDateFromFormat() to return Date or null. Also made 
//               pass null to Date constructor for values it doesn't know. 
// Dec 18, 2006: Packaged for use in crystal reports viewer
// May 17, 2003: Fixed bug in parseDate() for dates <1970
// March 11, 2003: Added parseDate() function
// March 11, 2003: Added "NNN" formatting option. Doesn't match up
//                 perfectly with SimpleDateFormat formats, but 
//                 backwards-compatability was required.

// ------------------------------------------------------------------
// These functions use the same 'format' strings as the 
// java.text.SimpleDateFormat class, with minor exceptions.
// The format string consists of the following abbreviations:
// 
// Field        | Full Form          | Short Form
// -------------+--------------------+-----------------------
// Year         | yyyy (4 digits)    | yy (2 digits), y (2 or 4 digits)
//              | YYYY (4 digits)    | YY (2 digits), Y (2 or 4 digits)
// Month        | MMM (name or abbr.)| MM (2 digits), M (1 or 2 digits)
//              | NNN (abbr.)        |
// Day of Month | dd (2 digits)      | d (1 or 2 digits)
//              | DD (2 digits)      | D (1 or 2 digits)
// Day of Week  | EE (name)          | E (abbr)
// Hour (1-12)  | hh (2 digits)      | h (1 or 2 digits)
// Hour (0-23)  | HH (2 digits)      | H (1 or 2 digits)
// Hour (0-11)  | KK (2 digits)      | K (1 or 2 digits)
// Hour (1-24)  | kk (2 digits)      | k (1 or 2 digits)
// Minute       | mm (2 digits)      | m (1 or 2 digits)
// Second       | ss (2 digits)      | s (1 or 2 digits)
// AM/PM        | a                  |
//
// NOTE THE DIFFERENCE BETWEEN MM and mm! Month=MM, not mm!
// Examples:
//  "MMM d, y" matches: January 01, 2000
//                      Dec 1, 1900
//                      Nov 20, 00
//  "M/d/yy"   matches: 01/20/00
//                      9/2/00
//  "MMM dd, yyyy hh:mm:ssa" matches: "January 01, 2000 12:30:45AM"
// ------------------------------------------------------------------

if (typeof bobj == 'undefined') {
    bobj = {};
}

if (typeof bobj.external == 'undefined') {
    bobj.external = {};
}

if (typeof bobj.external.date == 'undefined') {
    bobj.external.date = {};
}

bobj.external.date.MONTH_NAMES=new Array('January','February','March','April','May','June','July','August','September','October','November','December','Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec');
bobj.external.date.DAY_NAMES=new Array('Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday','Sun','Mon','Tue','Wed','Thu','Fri','Sat');
bobj.external.date.LZ = function(x) {return(x<0||x>9?"":"0")+x}

// ------------------------------------------------------------------
// isDate ( date_string, format_string )
// Returns true if date string matches format of format string and
// is a valid date. Else returns false.
// It is recommended that you trim whitespace around the value before
// passing it to this function, as whitespace is NOT ignored!
// ------------------------------------------------------------------
bobj.external.date.isDate = function(val,format) {
    var date=bobj.external.date.getDateFromFormat(val,format);
    if (!date) { return false; }
    return true;
}

// -------------------------------------------------------------------
// compareDates(date1,date1format,date2,date2format)
//   Compare two date strings to see which is greater.
//   Returns:
//   1 if date1 is greater than date2
//   0 if date2 is greater than date1 of if they are the same
//  -1 if either of the dates is in an invalid format
// -------------------------------------------------------------------
bobj.external.date.compareDates = function(date1,dateformat1,date2,dateformat2) {
    var d1=bobj.external.date.getDateFromFormat(date1,dateformat1);
    var d2=bobj.external.date.getDateFromFormat(date2,dateformat2);
    if (!d1 || !d2) {
        return -1;
        }
    else if (d1.getTime() > d2.getTime()) {
        return 1;
        }
    return 0;
}

// ------------------------------------------------------------------
// formatDate (date_object, format)
// Returns a date in the output format specified.
// The format string uses the same abbreviations as in getDateFromFormat()
// ------------------------------------------------------------------
bobj.external.date.formatDate = function(date,format) {
    format=format+"";
    var result="";
    var i_format=0;
    var c="";
    var token="";
    var y=date.getFullYear()+"";
    var M=date.getMonth()+1;
    var d=date.getDate();
    var E=date.getDay();
    var H=date.getHours();
    var m=date.getMinutes();
    var s=date.getSeconds();
    var yyyy,yy,MMM,MM,dd,hh,h,mm,ss,ampm,HH,H,KK,K,kk,k;
    // Convert real date parts into formatted versions
    var value=new Object();
    while (y.length < 4) {y="0" + y}
    value["y"]=""+y;
    value["yyyy"]=y;
    value["yy"]=y.substring(2,4);
    value["Y"]=value["y"];
    value["YY"]=value["yy"];
    value["YYYY"]=value["yyyy"];
    value["M"]=M;
    value["MM"]=bobj.external.date.LZ(M);
    value["MMM"]=bobj.external.date.MONTH_NAMES[M-1];
    value["NNN"]=bobj.external.date.MONTH_NAMES[M+11];
    value["d"]=d;
    value["dd"]=bobj.external.date.LZ(d);
    value["D"]=value["d"];
    value["DD"]=value["dd"];
    value["E"]=bobj.external.date.DAY_NAMES[E+7];
    value["EE"]=bobj.external.date.DAY_NAMES[E];
    value["H"]=H;
    value["HH"]=bobj.external.date.LZ(H);
    if (H==0){value["h"]=12;}
    else if (H>12){value["h"]=H-12;}
    else {value["h"]=H;}
    value["hh"]=bobj.external.date.LZ(value["h"]);
    if (H>11){value["K"]=H-12;} else {value["K"]=H;}
    value["k"]=H+1;
    value["KK"]=bobj.external.date.LZ(value["K"]);
    value["kk"]=bobj.external.date.LZ(value["k"]);
    if (H > 11) { value["a"]="PM"; }
    else { value["a"]="AM"; }
    value["m"]=m;
    value["mm"]=bobj.external.date.LZ(m);
    value["s"]=s;
    value["ss"]=bobj.external.date.LZ(s);
    while (i_format < format.length) {
        c=format.charAt(i_format);
        token="";
        while ((format.charAt(i_format)==c) && (i_format < format.length)) {
            token += format.charAt(i_format++);
            }
        if (value[token] != null) { result=result + value[token]; }
        else { result=result + token; }
        }
    return result;
}
    
// ------------------------------------------------------------------
// Utility functions for parsing in getDateFromFormat()
// ------------------------------------------------------------------
bobj.external.date._isInteger = function(val) {
    var digits="1234567890";
    for (var i=0; i < val.length; i++) {
        if (digits.indexOf(val.charAt(i))==-1) { return false; }
        }
    return true;
}

bobj.external.date._getInt = function(str,i,minlength,maxlength) {
    for (var x=maxlength; x>=minlength; x--) {
        var token=str.substring(i,i+x);
        if (token.length < minlength) { return null; }
        if (bobj.external.date._isInteger(token)) { return token; }
        }
    return null;
}
    
// ------------------------------------------------------------------
// getDateFromFormat( date_string , format_string )
//
// This function takes a date string and a format string. It matches
// If the date string matches the format string, it returns the date.
// If it does not match, it returns null.
// ------------------------------------------------------------------
bobj.external.date.getDateFromFormat = function(val,format) {
    val=val+"";
    format=format+"";
    var i_val=0;
    var i_format=0;
    var c="";
    var token="";
    var token2="";
    var x,y;
    var year=null;
    var month=null;
    var date=null;
    var hh=null;
    var mm=null;
    var ss=null;
    var ampm="";
    
    while (i_format < format.length) {
        // Get next token from format string
        c=format.charAt(i_format);
        token="";
        while ((format.charAt(i_format)==c) && (i_format < format.length)) {
            token += format.charAt(i_format++);
            }
        // Extract contents of value based on format token
        if (token=="yyyy" || token=="YYYY" || token=="yy" || token=="YY" || token=="y" || token=="Y") {
            if (token=="yyyy" || token=="YYYY") { x=4;y=4; }
            if (token=="yy" || token=="YY")   { x=2;y=2; }
            if (token=="y" || token=="Y")    { x=2;y=4; }
            year=bobj.external.date._getInt(val,i_val,x,y); 
            if (year==null) { return null; }
            i_val += year.length;
            if (year.length==2) { 
                if (year > 70) { year=1900+(year-0); }
                else { year=2000+(year-0); }
                }
            }
        else if (token=="MMM"||token=="NNN"){
            month=0;
            for (var i=0; i<bobj.external.date.MONTH_NAMES.length; i++) {
                var month_name=bobj.external.date.MONTH_NAMES[i];
                if (val.substring(i_val,i_val+month_name.length).toLowerCase()==month_name.toLowerCase()) {
                    if (token=="MMM"||(token=="NNN"&&i>11)) {
                        month=i+1;
                        if (month>12) { month -= 12; }
                        i_val += month_name.length;
                        break;
                        }
                    }
                }
            if ((month < 1)||(month>12)){return null;}
            }
        else if (token=="EE"||token=="E"){
            for (var i=0; i<bobj.external.date.DAY_NAMES.length; i++) {
                var day_name=bobj.external.date.DAY_NAMES[i];
                if (val.substring(i_val,i_val+day_name.length).toLowerCase()==day_name.toLowerCase()) {
                    i_val += day_name.length;
                    break;
                    }
                }
            }
        else if (token=="MM"||token=="M") {
            month=bobj.external.date._getInt(val,i_val,token.length,2);
            if(month==null||(month<1)||(month>12)){return null;}
            i_val+=month.length;}
        else if (token=="dd"||token=="DD"||token=="d"||token=="D") {
            date=bobj.external.date._getInt(val,i_val,token.length,2);
            if(date==null||(date<1)||(date>31)){return null;}
            i_val+=date.length;}
        else if (token=="hh"||token=="h") {
            hh=bobj.external.date._getInt(val,i_val,token.length,2);
            if(hh==null||(hh<1)||(hh>12)){return null;}
            i_val+=hh.length;}
        else if (token=="HH"||token=="H") {
            hh=bobj.external.date._getInt(val,i_val,token.length,2);
            if(hh==null||(hh<0)||(hh>23)){return null;}
            i_val+=hh.length;}
        else if (token=="KK"||token=="K") {
            hh=bobj.external.date._getInt(val,i_val,token.length,2);
            if(hh==null||(hh<0)||(hh>11)){return null;}
            i_val+=hh.length;}
        else if (token=="kk"||token=="k") {
            hh=bobj.external.date._getInt(val,i_val,token.length,2);
            if(hh==null||(hh<1)||(hh>24)){return null;}
            i_val+=hh.length;hh--;}
        else if (token=="mm"||token=="m") {
            mm=bobj.external.date._getInt(val,i_val,token.length,2);
            if(mm==null||(mm<0)||(mm>59)){return null;}
            i_val+=mm.length;}
        else if (token=="ss"||token=="s") {
            ss=bobj.external.date._getInt(val,i_val,token.length,2);
            if(ss==null||(ss<0)||(ss>59)){return null;}
            i_val+=ss.length;}
        else if (token=="a") {
            if (val.substring(i_val,i_val+2).toLowerCase()=="am") {ampm="AM";}
            else if (val.substring(i_val,i_val+2).toLowerCase()=="pm") {ampm="PM";}
            else {return null;}
            i_val+=2;}
        else {
            if (val.substring(i_val,i_val+token.length)!=token) {return null;}
            else {i_val+=token.length;}
            }
        }
    // If there are any trailing characters left in the value, it doesn't match
    if (i_val != val.length) { return null; }
    // Is date valid for month?
    if (month==2) {
        // Check for leap year
        if ( ( (year%4==0)&&(year%100 != 0) ) || (year%400==0) ) { // leap year
            if (date > 29){ return null; }
            }
        else { if (date > 28) { return null; } }
        }
    if ((month==4)||(month==6)||(month==9)||(month==11)) {
        if (date > 30) { return null; }
        }
    // Correct hours value
    if (hh !== null) {
        if (hh<12 && ampm=="PM") { hh=hh-0+12; }
        else if (hh>11 && ampm=="AM") { hh-=12; }
    }
    
    var newDate = new Date(year,month-1,date,hh,mm,ss);
    newDate.setFullYear(year); 
    return newDate; 
}


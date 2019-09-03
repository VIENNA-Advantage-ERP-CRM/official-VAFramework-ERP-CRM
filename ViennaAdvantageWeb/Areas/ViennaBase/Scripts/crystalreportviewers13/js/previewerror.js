function writeError(header, message)
{
    document.write(
        '<link rel="stylesheet" type="text/css" href="css/exception.css">' +
        '<table class="crExceptionBorder" width="100%" cellspacing=1 cellpadding=0 border=0>' +
          '<tr><td class="crExceptionHeader">' + header + '</td></tr>' +
          '<tr><td>' +
            '<table width="100%" border=0 cellpadding=5 cellspacing=0>' +
              '<tr><td class="crExceptionElement">' + 
                '<table border=0 cellpadding=5 cellspacing=0>' +
                  '<tr><td><span class="crExceptionText">' + message + '</span></td></tr>' +
                '</table>' +
              '</td></tr>' +
            '</table>' +
          '</td></tr>' +
        '</table>\n' );
}

function loadResources(locale) 
{
    var lang = locale.split('-');
    var locales = [];
    
    locales.push('en');
    
    if(lang.length >= 1) {
        locales.push(lang[0]);
    }
    
    if(lang.length >= 2) {
        locales.push(lang[0] + '_' + lang[1]);
    }
    
    for(var i =0 ; i < locales.length; i++) {
        _loadStrings(locales[i]);
    }
}

function _loadStrings(locale) 
{
    document.write('<script language="javascript" src="js/strings_' + locale + '.js"></script>');
}
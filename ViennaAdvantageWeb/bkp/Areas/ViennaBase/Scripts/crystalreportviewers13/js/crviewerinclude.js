// crviewerinclude.js
// Includes functions that should be written out for all javascript enabled clients

function crv_createCookie(name,value,days)
{
	if (days)
	{
		var date = new Date();
		date.setTime(date.getTime()+(days*24*60*60*1000));
		var expires = "; expires="+date.toGMTString();
	}
	else var expires = "";
	document.cookie = name+"="+value+expires+"; path=/";
}

function crv_readCookie(name)
{
	var nameEQ = name + "=";
	var ca = document.cookie.split(';');
	for(var i=0;i < ca.length;i++)
	{
		var c = ca[i];
		while (c.charAt(0)==' ') c = c.substring(1,c.length);
		if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
	}
	return null;
}

function crv_eraseCookie(name)
{
	crv_createCookie(name,"",-1);
}

function crv_isCookieEnabled()
{
    var TESTNAME = '__BOBJ_VIEWER_TEST_COOKIE_NAME';
    var TESTVALUE = '__BOBJ_VIEWER_TEST_COOKIE_VALUE';
    
    crv_createCookie(TESTNAME, TESTVALUE);
    var cookieValue = crv_readCookie(TESTNAME);
    if (cookieValue == TESTVALUE)
    {
        crv_eraseCookie(TESTNAME);
        return true;
    }
    
    return false;
}

function crv_showDialog(showFunc, cookieName)
{
    var cookieVal = crv_readCookie(cookieName);
    if (cookieVal == "true" || !crv_isCookieEnabled()) 
    {
        showFunc();
    }
    crv_eraseCookie(cookieName);
}

function getComposite(prefixArray, suffixStr, encodeF)
{
    if (!prefixArray) return "";
    suffixStr = suffixStr ? suffixStr : "";
    var composite="";
    for (var i in prefixArray) 
    {
        var eltID = prefixArray[i] + suffixStr;
        var elt = document.getElementById(eltID);
        if (elt) 
        {
            var value = elt.value;
            if (value) 
            {
                if (composite != "")
                    composite += ";";
                composite += prefixArray[i] + "=" + value;
            }
        }
    }
    return encodeF ? encodeF(composite) : composite;
}

function scrollToElement(id)
{
	var obj = document.getElementById(id); 
    if(obj != null)
    { 
		var offsetTop = obj.offsetTop; 
		var offsetLeft = obj.offsetLeft; 
		var myOffsetParent = obj.offsetParent; 
	
		while( myOffsetParent ) 
		{ 
			offsetTop += myOffsetParent.offsetTop; 
			offsetLeft += myOffsetParent.offsetLeft; 
			myOffsetParent = myOffsetParent.offsetParent; 
		}  
		
		window.scrollTo(offsetLeft, offsetTop);
	}
}

function isFunction(f)
{
    return (typeof f == 'function');
}

function addOnloadFunction(func)
{
    var oldOnload = window.onload;
    window.onload = function ()
    {
        if (isFunction(oldOnload))
            oldOnload();
        if (isFunction(func))
            func();
    }
}

//This function should only be called from the COM viewer.  Use encodeURIComponent from the
//Java viewer to get the appropriate encoding.
function COMUrlEncode( strToBeEncoded )
{
	var encodedString = new String("");
	for( var i = 0; i < strToBeEncoded.length; i++ )
	{
		var nextChar = strToBeEncoded.charAt(i);
		switch( nextChar )
		{
			//Unsafe characters
			case '%':
			{
				encodedString += "%25";
				break;
			}
			case '+':
			{
				encodedString += "%2B";
				break;
			}
			case ' ':
			{
				encodedString += "%20";
				break;
			}
			case '<':
			{
				encodedString += "%3C";
				break;
			}
			case '>':
			{
				encodedString += "%3E";
				break;
			}
			case '"':
			{
				encodedString += "%22";
				break;
			}
			case '\'':
			{
				encodedString += "%27";
				break;
			}
			case '#':
			{
				encodedString += "%23";
				break;
			}
			case '{':
			{
				encodedString += "%7B";
				break;
			}
			case '}':
			{
				encodedString += "%7D";
				break;
			}
			case '|':
			{
				encodedString += "%7C";
				break;
			}
			case '\\':
			{
				encodedString += "%5C";
				break;
			}
			case '^':
			{
				encodedString += "%5E";
				break;
			}
			case '~':
			{
				encodedString += "%7E";
				break;
			}
			case '`':
			{
				encodedString += "%60";
				break;
			}
			case '[':
			{
				encodedString += "%5B";
				break;
			}
			case ']':
			{
				encodedString += "%5D";
				break;
			}
			//Reserved characters
			case ';':
			{
				encodedString += "%3B";
				break;
			}
			case '/':
			{
				encodedString += "%2F";
				break;
			}
			case '?':
			{
				encodedString += "%3F";
				break;
			}
			case ':':
			{
				encodedString += "%3A";
				break;
			}
			case '@':
			{
				encodedString += "%40";
				break;
			}
			case '=':
			{
				encodedString += "%3D";
				break;
			}
			case '&':
			{
				encodedString += "%26";
				break;
			}
			default:
			{
				encodedString += nextChar;
				break;
			}
		}
	}

	return encodedString;
}
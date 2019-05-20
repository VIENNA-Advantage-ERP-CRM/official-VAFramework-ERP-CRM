using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Utility
{
    public class Umlout
    {
        public static String ConvertUmlouts2UML(String umlout)
        {
            if (umlout != null)
            {
                umlout = umlout.Replace("ä", "&auml;");
                umlout = umlout.Replace("Ä", "&Auml;");
                umlout = umlout.Replace("ö", "&ouml;");
                umlout = umlout.Replace("Ö", "&Ouml;");
                umlout = umlout.Replace("ü", "&uuml;");
                umlout = umlout.Replace("Ü", "&Uuml;");
                umlout = umlout.Replace("ß", "&beta;");
                umlout = umlout.Replace("°", "&deg;");
                umlout = umlout.Replace(" ", " "); // Non Printable characters
                // Replace with Space
                umlout = umlout.Replace("€", "&euro;");
                umlout = umlout.Replace("£", "&pound");
                umlout = umlout.Replace("«", "&laquo;");
                umlout = umlout.Replace("»", "&raquo;");
                umlout = umlout.Replace("•", "&bull;");
                umlout = umlout.Replace("†", "&dagger;");
                umlout = umlout.Replace("©", "&copy;");
                umlout = umlout.Replace("®", "&reg;");
                umlout = umlout.Replace("µ", "&micro;");
                umlout = umlout.Replace("•", "&middot;");
                umlout = umlout.Replace("–", "&ndash;");
                umlout = umlout.Replace("—", "&mdash");
                umlout = umlout.Replace("№", "&#8470;");

                umlout = umlout.Replace("á", "&aacute;");
                umlout = umlout.Replace("à", "&agrave;");
                umlout = umlout.Replace("è", "&egrave;");
                umlout = umlout.Replace("é", "&eacute;");
                umlout = umlout.Replace("ì", "&igrave;");
                umlout = umlout.Replace("í", "&iacute");
                umlout = umlout.Replace("ò", "&ograve;");
                umlout = umlout.Replace("ó", "&oacute;");
                umlout = umlout.Replace("ù", "&ugrave;");
                umlout = umlout.Replace("ú", "&uacute;");
                umlout = umlout.Replace("ÿ", "&yuml;");
                umlout = umlout.Replace("Ø", "&Oslash;");
                umlout = umlout.Replace("ï", "&iuml;");
                umlout = umlout.Replace("Ï", "&Iuml;");
                umlout = umlout.Replace("Â", "&Acirc;");
                umlout = umlout.Replace("Ê", "&Ecirc;");
                umlout = umlout.Replace("Î", "&Icirc;");
                umlout = umlout.Replace("Ô", "&Ocirc;");
                umlout = umlout.Replace("Û", "&Ucirc;");
                umlout = umlout.Replace("â", "&acirc;");
                umlout = umlout.Replace("ê", "&ecirc;");
                umlout = umlout.Replace("î", "&icirc;");
                umlout = umlout.Replace("ô", "&ocirc;");
                umlout = umlout.Replace("û", "&ucirc;");
                umlout = umlout.Replace("²", "&sup2;");
                umlout = umlout.Replace("…", "&hellip;");
                umlout = umlout.Replace("–", "&ndash;");
                umlout = umlout.Replace("’", "&rsquo;");
                umlout = umlout.Replace("‘", "&lsquo;");
                umlout = umlout.Replace("„", "&ldquor;");
                umlout = umlout.Replace("“", "&ldquo;");
                umlout = umlout.Replace("•", "&bull;");

            }
            return umlout;
        }

        public static String ConvertUML2Umlouts(String umlout)
        {

            if (umlout != null)
            {
                umlout = umlout.Replace("&auml;", "");
                umlout = umlout.Replace("&Auml;", "");
                umlout = umlout.Replace("&ouml;", "");
                umlout = umlout.Replace("&Ouml;", "");
                umlout = umlout.Replace("&uuml;", "");
                umlout = umlout.Replace("&Uuml;", "");
                umlout = umlout.Replace("&szlig;", "");
                umlout = umlout.Replace("&beta;", "");
                umlout = umlout.Replace("&deg;", "");
                umlout = umlout.Replace("&euro;", "");
                umlout = umlout.Replace("&pound", "");
                umlout = umlout.Replace("&laquo;", "");
                umlout = umlout.Replace("&raquo;", "");
                umlout = umlout.Replace("&bull;", "");
                umlout = umlout.Replace("&dagger;", "");
                umlout = umlout.Replace("&copy;", "");
                umlout = umlout.Replace("&reg;", "");
                umlout = umlout.Replace("&micro;", "");
                umlout = umlout.Replace("&middot;", "");
                umlout = umlout.Replace("&ndash;", "");
                umlout = umlout.Replace("&mdash", "");
                umlout = umlout.Replace("&amp;", "&");
                umlout = umlout.Replace("&#8470;", "?");
                umlout = umlout.Replace("&#039;", " ");
                umlout = umlout.Replace("&#216;", "");
                umlout = umlout.Replace("&#937;", "O");
                umlout = umlout.Replace("&#8486;", "O");
                umlout = umlout.Replace("&#8776;", "");
                umlout = umlout.Replace("&#61533;", "");
                umlout = umlout.Replace("&#246;", "");
                umlout = umlout.Replace("&#45;", "-");
                umlout = umlout.Replace("&#43;", "+");
                umlout = umlout.Replace("&#252;", "");
                umlout = umlout.Replace("&#228;", "");
                umlout = umlout.Replace("&#223;", "");
                umlout = umlout.Replace("&#37;", "%");
                umlout = umlout.Replace("&#178;", "");
                umlout = umlout.Replace("&#196;", "");
                umlout = umlout.Replace("&#181;", "");
                umlout = umlout.Replace("&#220;", "");
                umlout = umlout.Replace("&#176;", "");
                umlout = umlout.Replace("ï¿½", "ø");
            }
            return umlout;
        }

        public static String ConvertUmlouts2E(String umlout)
        {
            if (umlout != null)
            {
                umlout = umlout.Replace("ä", "ae");
                umlout = umlout.Replace("Ä", "Ae");
                umlout = umlout.Replace("ö", "oe");
                umlout = umlout.Replace("Ö", "Oe");
                umlout = umlout.Replace("ü", "ue");
                umlout = umlout.Replace("Ü", "Ue");
                umlout = umlout.Replace("ß", "ss");
                umlout = umlout.Replace("µ", "mu");
                umlout = umlout.Replace("°", "deg ");
                umlout = umlout.Replace("á", "a");
                umlout = umlout.Replace("à", "a");
                umlout = umlout.Replace("è", "e");
                umlout = umlout.Replace("é", "e");
                umlout = umlout.Replace("ì", "i");
                umlout = umlout.Replace("í", "i");
                umlout = umlout.Replace("ò", "o");
                umlout = umlout.Replace("ó", "o");
                umlout = umlout.Replace("ù", "u");
                umlout = umlout.Replace("ú", "u");
                umlout = umlout.Replace("ÿ", "ye");
                umlout = umlout.Replace("Ø", "O");
                umlout = umlout.Replace("ï", "ie");
                umlout = umlout.Replace("Ï", "Ie");
                umlout = umlout.Replace("Â", "A");
                umlout = umlout.Replace("Ê", "E");
                umlout = umlout.Replace("Î", "I");
                umlout = umlout.Replace("Ô", "O");
                umlout = umlout.Replace("Û", "U");
                umlout = umlout.Replace("â", "a");
                umlout = umlout.Replace("ê", "e");
                umlout = umlout.Replace("î", "i");
                umlout = umlout.Replace("ô", "o");
                umlout = umlout.Replace("û", "u");

                // umlout = umlout.Replace("á", "&aacute;");
                // umlout = umlout.Replace("à", "&agrave;");
                // umlout = umlout.Replace("è", "&egrave;");
                // umlout = umlout.Replace("é", "&eacute;");
                // umlout = umlout.Replace("ì", "&igrave;");
                // umlout = umlout.Replace("í", "&iacute");
                // umlout = umlout.Replace("ò", "&ograve;");
                // umlout = umlout.Replace("ó", "&oacute;");
                // umlout = umlout.Replace("ù", "&ugrave;");
                // umlout = umlout.Replace("ú", "&uacute;");
                // umlout = umlout.Replace("ÿ", "&yuml;");
                // umlout = umlout.Replace("Ø", "&Oslash;");
                // umlout = umlout.Replace("ï", "&iuml;");
                // umlout = umlout.Replace("Ï", "&Iuml;");
                umlout = umlout.Replace("²", "2");
                umlout = umlout.Replace("…", "...");
                umlout = umlout.Replace("–", "-");
                umlout = umlout.Replace("’", "`");
                umlout = umlout.Replace("‘", "`");
                umlout = umlout.Replace("„", " ");
                umlout = umlout.Replace("“", " ");


            }
            return umlout;
        }
    }

}

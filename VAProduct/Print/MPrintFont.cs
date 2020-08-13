/********************************************************
 * Module Name    : Report
 * Purpose        : Launch Report
 * Author         : Jagmohan Bhatt
 * Date           : 02-June-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Classes;
using System.Data;

using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Design;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Print
{
    public class MPrintFont : X_AD_PrintFont
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_PrintFont_ID">AD_PrintFont_ID</param>
        /// <param name="trxName">transaction</param>
        public MPrintFont(Context ctx, int AD_PrintFont_ID, Trx trxName)
            : base(ctx, AD_PrintFont_ID, trxName)
        {

            if (AD_PrintFont_ID == 0)
                SetIsDefault(false);
        }	//	MPrintFont

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MPrintFont(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }	//	MPrintFont

        /** Font cached					*/
        private Font _cacheFont = null;
        private const float JAVA_COMPATIBLE = 0.0f;

        public Font GetFont()
        {
            try
            {
                if (_cacheFont != null)
                    return _cacheFont;
                String code = (String)Get_Value("Code");
                int pos = code.LastIndexOf("-");
                int size = Convert.ToInt32(code.Substring(pos + 1));
                if (code == null || code.Equals("."))
                    _cacheFont = new Font("sansserif", size - JAVA_COMPATIBLE, FontStyle.Regular, GraphicsUnit.World);
                try
                {
                    if (code != null && !code.Equals("."))
                    //	fontfamilyname-style-pointsize
                    {
                        _cacheFont = Decode(code);
                    }
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, "Code=" + code, e);
                }
                if (code == null)
                    _cacheFont = null;
            }
            catch
            {
                _cacheFont = null;
            }
            //	family=dialog,name=Dialog,style=plain,size=12
            //	log.fine(code + " - " + m_cacheFont);
            return _cacheFont;
        }	//	getFont


        /// <summary>
        /// Decode the font string saved in database
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public Font Decode(string str)
        {
            String fontName = str;
            String styleName = "";
            //int pos = str.LastIndexOf("-");
            //int fontSize = Convert.ToInt32(str.Substring(pos + 1)) - 2;
            int fontSize = 12;
            FontStyle fontStyle = FontStyle.Regular;

            if (str == null)
            {
                return new Font("Dialog", fontSize, fontStyle, GraphicsUnit.World);
            }

            int lastHyphen = str.LastIndexOf('-');
            int lastSpace = str.LastIndexOf(' ');
            char sepChar = (lastHyphen > lastSpace) ? '-' : ' ';
            int sizeIndex = str.LastIndexOf(sepChar);
            int styleIndex = str.LastIndexOf(sepChar, sizeIndex - 1);
            int strlen = str.Length;

            if (sizeIndex > 0 && sizeIndex + 1 < strlen)
            {
                try
                {
                    fontSize = int.Parse(str.Substring(sizeIndex + 1));
                    if (fontSize <= 0)
                    {
                        fontSize = 12;
                    }
                }
                catch 
                {
                    /* It wasn't a valid size, if we didn't also find the
                     * start of the style string perhaps this is the style */
                    styleIndex = sizeIndex;
                    sizeIndex = strlen;
                    if (str[sizeIndex - 1] == sepChar)
                    {
                        sizeIndex--;
                    }
                }
            }

            if (styleIndex >= 0 && styleIndex + 1 < strlen)
            {
                styleName = str.Substring(styleIndex + 1, sizeIndex - (styleIndex + 1));
                styleName = styleName.ToLower();
                if (styleName.Equals("bolditalic"))
                {
                    fontStyle = FontStyle.Bold | FontStyle.Italic;
                }
                else if (styleName.Equals("italic"))
                {
                    fontStyle = FontStyle.Italic;
                }
                else if (styleName.Equals("bold"))
                {
                    fontStyle = FontStyle.Bold;
                }
                else if (styleName.Equals("plain"))
                {
                    fontStyle = FontStyle.Regular;
                }
                else
                {
                    /* this string isn't any of the expected styles, so
                     * assume its part of the font name
                     */
                    styleIndex = sizeIndex;
                    if (str[styleIndex - 1] == sepChar)
                    {
                        styleIndex--;
                    }
                }
                fontName = str.Substring(0, styleIndex);

            }
            else
            {
                int fontEnd = strlen;
                if (styleIndex > 0)
                {
                    fontEnd = styleIndex;
                }
                else if (sizeIndex > 0)
                {
                    fontEnd = sizeIndex;
                }
                if (fontEnd > 0 && str[fontEnd - 1] == sepChar)
                {
                    fontEnd--;
                }
                fontName = str.Substring(0, fontEnd);
            }

            if (fontName == "serif")
            {
                return new Font(FontFamily.GenericSerif, fontSize - JAVA_COMPATIBLE, fontStyle, GraphicsUnit.World);
            }
            {
                return new Font(fontName, fontSize - JAVA_COMPATIBLE, fontStyle, GraphicsUnit.World);
            }
        }

        public void SetFont(Font font)
        {
            //	fontfamilyname-style-pointsize
            StringBuilder sb = new StringBuilder();
            sb.Append(font.FontFamily.ToString()).Append("-");
            FontStyle style = font.Style;
            if (style == FontStyle.Regular)
                sb.Append("PLAIN");
            else if (style == FontStyle.Bold)
                sb.Append("BOLD");
            else if (style == FontStyle.Italic)
                sb.Append("ITALIC");
            else if (style == (FontStyle.Bold | FontStyle.Italic))
                sb.Append("BOLDITALIC");
            sb.Append("-").Append(font.Size.ToString());
            SetCode(sb.ToString());
        }	//	setFont


        public static MPrintFont Create(Font font)
        {
            MPrintFont pf = new MPrintFont(VAdvantage.Utility.Env.GetContext(), 0, null);
            StringBuilder name = new StringBuilder(font.Name);
            if (font.Style == FontStyle.Bold)
                name.Append(" bold");
            if (font.Style == FontStyle.Italic)
                name.Append(" italic");
            name.Append(" ").Append(font.Size);
            pf.SetName(name.ToString());
            pf.SetFont(font);
            pf.Save();
            return pf;
        }	//	create


        /** Cached Fonts						*/
        static private CCache<int, MPrintFont> _fonts = new CCache<int, MPrintFont>("AD_PrintFont", 20);

        static public MPrintFont Get(int AD_PrintFont_ID)
        {
            int key = AD_PrintFont_ID;
            MPrintFont pf = (MPrintFont)_fonts[key];
            if (pf == null)
            {
                pf = new MPrintFont(VAdvantage.Utility.Env.GetContext(), AD_PrintFont_ID, null);
                _fonts.Add(key, pf);
            }
            return pf;
        }	//	get
    }
}

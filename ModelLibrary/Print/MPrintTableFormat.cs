/********************************************************
 * Module Name    : Report
 * Purpose        : Launch Report
 * Author         : Jagmohan Bhatt
 * Date           : 04-June-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Common;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Drawing.Printing;

namespace VAdvantage.Print
{
    public class MPrintTableFormat : X_AD_PrintTableFormat
    {
        /// <summary>
        /// Standard Constructor    
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_PrintTableFormat_ID">AD_PrintTableFormat_ID</param>
        /// <param name="trxName">transaction</param>
        public MPrintTableFormat(Ctx ctx, int AD_PrintTableFormat_ID, Trx trxName)
            : base(ctx, AD_PrintTableFormat_ID, trxName)
        {

            if (AD_PrintTableFormat_ID == 0)
            {
                //	setName (null);
                SetIsDefault(false);
                SetIsPaintHeaderLines(true);	// Y
                SetIsPaintBoundaryLines(false);
                SetIsPaintHLines(false);
                SetIsPaintVLines(false);
                SetIsPrintFunctionSymbols(true);
            }
        }	//	MPrintTableFormat


        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MPrintTableFormat(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }	//	MPrintTableFormat



        private Font standard_Font = null;

        private Font pageHeader_Font = null;
        private Font pageFooter_Font = null;
        private Color? pageHeaderFG_Color = null;
        private Color? pageHeaderBG_Color = null;
        private Color? pageFooterFG_Color = null;
        private Color? pageFooterBG_Color = null;

        private Font parameter_Font = null;
        private Color? parameter_Color = null;

        private Font header_Font = null;
        private Color? headerFG_Color = null;
        private Color? headerBG_Color = null;
        private Color? hdrLine_Color = null;
        private DashStyle header_Stroke;		//	-
        private PdfSharp.Drawing.XDashStyle header_StrokePdf;		//	-

        private Font funct_Font;
        private Color? functFG_Color = null;
        private Color? functBG_Color = null;

        private DashStyle lineH_Stroke;	//	-
        private DashStyle lineV_Stroke;	//	|

        private PdfSharp.Drawing.XDashStyle lineH_StrokePdf;	//	-
        private PdfSharp.Drawing.XDashStyle lineV_StrokePdf;	//	|

        private Color? lineH_Color = null;
        private Color? lineV_Color = null;
        /// <summary>
        /// Get header font
        /// </summary>
        /// <returns>font</returns>
        public Font GetHeader_Font()
        {
            if (header_Font != null)
                return header_Font;
            int i = GetHdr_PrintFont_ID();
            if (i != 0)
                header_Font = MPrintFont.Get(i).GetFont();
            if (header_Font == null)
                header_Font = new Font(standard_Font.Name, standard_Font.Size, FontStyle.Bold, GraphicsUnit.World);
            return header_Font;
        }	//	getHeader_Font

        /// <summary>
        /// Get header bg color
        /// </summary>
        /// <returns>color</returns>
        public Color GetHeaderBG_Color()
        {
            if (headerBG_Color != null)
                return (Color)headerBG_Color;
            int i = GetHdrTextBG_PrintColor_ID();
            if (i != 0)
                headerBG_Color = MPrintColor.Get(GetCtx(), i).GetColor();
            if (headerBG_Color == null)
                headerBG_Color = Color.Cyan;
            return (Color)headerBG_Color;
        }	//	getHeaderBG_Color

        /// <summary>
        /// Get header line color
        /// </summary>
        /// <returns>color</returns>
        public Color GetHeaderLine_Color()
        {
            if (hdrLine_Color != null)
                return (Color)hdrLine_Color;
            int i = GetHdrLine_PrintColor_ID();
            if (i != 0)
                hdrLine_Color = MPrintColor.Get(GetCtx(), i).GetColor();
            if (hdrLine_Color == null)
                hdrLine_Color = MPrintColor.blackBlue;
            return (Color)hdrLine_Color;
        }	//	getHeaderLine_Color

        /// <summary>
        /// Get standard font
        /// </summary>
        /// <returns>font</returns>
        public Font GetStandard_Font()
        {
            return standard_Font;
        }	//	getStandard_Font

        /// <summary>
        /// Get header FG color
        /// </summary>
        /// <returns>color</returns>
        public Color GetHeaderFG_Color()
        {
            if (headerFG_Color != null)
                return (Color)headerFG_Color;
            int i = GetHdrTextFG_PrintColor_ID();
            if (i != 0)
                headerFG_Color = MPrintColor.Get(GetCtx(), i).GetColor();
            if (headerFG_Color == null)
                headerFG_Color = MPrintColor.blackBlue;
            return (Color)headerFG_Color;
        }	//	getHeaderFG_Color

        /// <summary>
        /// Set Standard font
        /// </summary>
        /// <param name="standardFont">font</param>
        public void SetStandard_Font(Font standardFont)
        {
            if (standardFont != null)
                standard_Font = standardFont;
        }	//	setStandard_Font


        /// <summary>
        /// Get header stroke
        /// </summary>
        /// <returns>Stroke in decimal</returns>
        public new Decimal GetHdrStroke()
        {
            Decimal retValue = base.GetHdrStroke();
            if ( Env.ZERO.CompareTo(retValue) <= 0)
                retValue = new Decimal(2.0);
            return retValue;
        }	//	getHdrStroke

        /// <summary>
        /// Get Page header font
        /// </summary>
        /// <returns>font</returns>
        public Font GetPageHeader_Font()
        {
            if (pageHeader_Font == null)
                pageHeader_Font = new Font(standard_Font.Name, standard_Font.Size, FontStyle.Bold, GraphicsUnit.World);
            return pageHeader_Font;
        }	//	getPageHeader_Font



        /// <summary>
        /// Get funtion font
        /// </summary>
        /// <returns>font</returns>
        public Font GetFunct_Font()
        {
            if (funct_Font != null)
                return funct_Font;
            int i = GetFunct_PrintFont_ID();
            if (i != 0)
                funct_Font = MPrintFont.Get(i).GetFont();
            if (funct_Font == null)
                funct_Font = new Font(standard_Font.Name, standard_Font.Size, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.World);
            return funct_Font;
        }	//	getFunct_Font

        /// <summary>
        /// Get function bg color
        /// </summary>
        /// <returns>color</returns>
        public Color GetFunctBG_Color()
        {
            if (functBG_Color != null)
                return (Color)functBG_Color;
            int i = GetFunctBG_PrintColor_ID();
            if (i != 0)
                functBG_Color = MPrintColor.Get(GetCtx(), i).GetColor();
            if (functBG_Color == null)
                functBG_Color = Color.White;
            return (Color)functBG_Color;
        }	//	getFunctBG_Color

        /// <summary>
        /// Get function FG color
        /// </summary>
        /// <returns>color</returns>
        public Color GetFunctFG_Color()
        {
            if (functFG_Color != null)
                return (Color)functFG_Color;
            int i = GetFunctFG_PrintColor_ID();
            if (i != 0)
                functFG_Color = MPrintColor.Get(GetCtx(), i).GetColor();
            if (functFG_Color == null)
                functFG_Color = MPrintColor.darkGreen;
            return (Color)functFG_Color;
        }	//	getFunctFG_Color

        /// <summary>
        /// Get param font
        /// </summary>
        /// <returns>font</returns>
        public Font GetParameter_Font()
        {
            if (parameter_Font == null)
                parameter_Font = new Font(standard_Font.Name, standard_Font.Size, FontStyle.Italic, GraphicsUnit.World);
            return parameter_Font;
        }	//	getParameter_Font

        /// <summary>
        /// Get param color
        /// </summary>
        /// <returns>color</returns>
        public Color GetParameter_Color()
        {
            if (parameter_Color == null)
                parameter_Color = Color.DarkGray;
            return (Color)parameter_Color;
        }	//	getParameter_Color


        /// <summary>
        /// Get page header fG color
        /// </summary>
        /// <returns>color</returns>
        public Color GetPageHeaderFG_Color()
        {
            if (pageHeaderFG_Color == null)
                pageHeaderFG_Color = MPrintColor.blackBlue;
            return (Color)pageHeaderFG_Color;
        }	//	getPageHeaderFG_Color


        /// <summary>
        /// Get page footer font
        /// </summary>
        /// <returns>font</returns>
        public Font GetPageFooter_Font()
        {
            if (pageFooter_Font == null)
                pageFooter_Font = new Font(standard_Font.Name, standard_Font.Size - 2, FontStyle.Regular, GraphicsUnit.World);
            return pageFooter_Font;
        }	//	getPageFooter_Font

        /// <summary>
        /// Get page footer FG color
        /// </summary>
        /// <returns></returns>
        public Color GetPageFooterFG_Color()
        {
            if (pageFooterFG_Color == null)
                pageFooterFG_Color = MPrintColor.blackBlue;
            return (Color)pageFooterFG_Color;
        }	//	getPageFooterFG_Color

        /// <summary>
        /// Get page footer bg color
        /// </summary>
        /// <returns>color</returns>
        public Color GetPageFooterBG_Color()
        {
            if (pageFooterBG_Color == null)
                pageFooterBG_Color = Color.White;
            return (Color)pageFooterBG_Color;
        }	//	getPageFooterBG_Color

        /// <summary>
        /// Get page header BG color
        /// </summary>
        /// <returns>color</returns>
        public Color GetPageHeaderBG_Color()
        {
            if (pageHeaderBG_Color == null)
                pageHeaderBG_Color = Color.White;
            return (Color)pageHeaderBG_Color;
        }	//	getPageHeaderBG_Color


        private StringBuilder header_Stroke1 = new StringBuilder("");

        /// <summary>
        /// Get header stroke
        /// </summary>
        /// <returns>sroke value in string</returns>
        public DashStyle GetHeader_Stroke()
        {
            float width = (float)GetHdrStroke();
            if (GetHdrStrokeType() == null || HDRSTROKETYPE_SolidLine.Equals(GetHdrStrokeType()))
                header_Stroke = DashStyle.Solid;
            else if (HDRSTROKETYPE_DashedLine.Equals(GetHdrStrokeType()))
                header_Stroke = DashStyle.Dash;
            else if (HDRSTROKETYPE_DottedLine.Equals(GetHdrStrokeType()))
                header_Stroke = DashStyle.Dot;
            else if (HDRSTROKETYPE_Dash_DottedLine.Equals(GetHdrStrokeType()))
                header_Stroke = DashStyle.DashDot;

            if (string.IsNullOrEmpty(header_Stroke.ToString()))
                header_Stroke = DashStyle.Solid;


            return header_Stroke;

        }


        public PdfSharp.Drawing.XDashStyle GetHeader_StrokePdf()
        {
            float width = (float)GetHdrStroke();
            if (GetHdrStrokeType() == null || HDRSTROKETYPE_SolidLine.Equals(GetHdrStrokeType()))
                header_StrokePdf = PdfSharp.Drawing.XDashStyle.Solid;
            else if (HDRSTROKETYPE_DashedLine.Equals(GetHdrStrokeType()))
                header_StrokePdf = PdfSharp.Drawing.XDashStyle.Dash;
            else if (HDRSTROKETYPE_DottedLine.Equals(GetHdrStrokeType()))
                header_StrokePdf = PdfSharp.Drawing.XDashStyle.Dot;
            else if (HDRSTROKETYPE_Dash_DottedLine.Equals(GetHdrStrokeType()))
                header_StrokePdf = PdfSharp.Drawing.XDashStyle.DashDot;

            if (string.IsNullOrEmpty(header_Stroke.ToString()))
                header_StrokePdf = PdfSharp.Drawing.XDashStyle.Solid;


            return header_StrokePdf;

        }

        public Excel.XlLineStyle GetHeader_StrokeForExcel()
        {
            if (string.IsNullOrEmpty(header_Stroke1.ToString()))
            {
                float width = (float)GetHdrStroke();
                if (GetHdrStrokeType() == null || HDRSTROKETYPE_SolidLine.Equals(GetHdrStrokeType()))
                    return Excel.XlLineStyle.xlContinuous;
                else if (HDRSTROKETYPE_DashedLine.Equals(GetHdrStrokeType()))
                    return Excel.XlLineStyle.xlDash;
                else if (HDRSTROKETYPE_DottedLine.Equals(GetHdrStrokeType()))
                    return Excel.XlLineStyle.xlDot;
                else if (HDRSTROKETYPE_Dash_DottedLine.Equals(GetHdrStrokeType()))
                    return Excel.XlLineStyle.xlDashDot;

                if (string.IsNullOrEmpty(header_Stroke1.ToString()))
                    return Excel.XlLineStyle.xlContinuous;
            }

            return Excel.XlLineStyle.xlContinuous;

        }

        Excel.XlLineStyle lineH_Stroke_Excel;
        //Excel.XlLineStyle lineV_Stroke_Excel;

        /// <summary>
        /// GEt Horizontal Line stroke for excel
        /// </summary>
        /// <returns>Line style</returns>
        public DashStyle GetHLine_Stroke()
        {
            decimal width = GetLineStroke() / 2;
            if (GetHdrStrokeType() == null || LINESTROKETYPE_DottedLine.Equals(GetLineStrokeType()))
                lineH_Stroke = DashStyle.Dot;
            else if (LINESTROKETYPE_SolidLine.Equals(GetLineStrokeType()))
                lineH_Stroke = DashStyle.Solid;			//	-
            else if (LINESTROKETYPE_DashedLine.Equals(GetLineStrokeType()))
                lineH_Stroke = DashStyle.Dash;		//	- -
            else if (LINESTROKETYPE_Dash_DottedLine.Equals(GetLineStrokeType()))
                lineH_Stroke = DashStyle.DashDot;                //	default / fallback
            return lineH_Stroke;
        }	//	getHLine_Stroke

        public PdfSharp.Drawing.XDashStyle GetHLine_StrokePdf()
        {
            decimal width = GetLineStroke() / 2;
            if (GetHdrStrokeType() == null || LINESTROKETYPE_DottedLine.Equals(GetLineStrokeType()))
                lineH_StrokePdf = PdfSharp.Drawing.XDashStyle.Dot;
            else if (LINESTROKETYPE_SolidLine.Equals(GetLineStrokeType()))
                lineH_StrokePdf = PdfSharp.Drawing.XDashStyle.Solid;			//	-
            else if (LINESTROKETYPE_DashedLine.Equals(GetLineStrokeType()))
                lineH_StrokePdf = PdfSharp.Drawing.XDashStyle.Dash;		//	- -
            else if (LINESTROKETYPE_Dash_DottedLine.Equals(GetLineStrokeType()))
                lineH_StrokePdf = PdfSharp.Drawing.XDashStyle.DashDot;                //	default / fallback
            return lineH_StrokePdf;
        }	//	getHLine_Stroke

        /// <summary>
        /// GEt Horizontal Line stroke for excel
        /// </summary>
        /// <returns>Line style</returns>
        public Excel.XlLineStyle GetHLine_Stroke_Excel()
        {
            decimal width = GetLineStroke() / 2;
            if (GetHdrStrokeType() == null || LINESTROKETYPE_DottedLine.Equals(GetLineStrokeType()))
                lineH_Stroke_Excel = Excel.XlLineStyle.xlDot;
            else if (LINESTROKETYPE_SolidLine.Equals(GetLineStrokeType()))
                lineH_Stroke_Excel = Excel.XlLineStyle.xlContinuous;			//	-
            else if (LINESTROKETYPE_DashedLine.Equals(GetLineStrokeType()))
                lineH_Stroke_Excel = Excel.XlLineStyle.xlDash;		//	- -
            else if (LINESTROKETYPE_Dash_DottedLine.Equals(GetLineStrokeType()))
                lineH_Stroke_Excel = Excel.XlLineStyle.xlDashDot;                //	default / fallback
            return lineH_Stroke_Excel;
        }	//	getHLine_Stroke

        public DashStyle GetVLine_Stroke()
        {
            decimal width = GetLineStroke() / 2;
            if (GetHdrStrokeType() == null || LINESTROKETYPE_DottedLine.Equals(GetLineStrokeType()))
                lineV_Stroke = DashStyle.Dot;
            else if (LINESTROKETYPE_SolidLine.Equals(GetLineStrokeType()))
                lineV_Stroke = DashStyle.Solid;			//	-
            else if (LINESTROKETYPE_DashedLine.Equals(GetLineStrokeType()))
                lineV_Stroke = DashStyle.Dash;		//	- -
            else if (LINESTROKETYPE_Dash_DottedLine.Equals(GetLineStrokeType()))
                lineV_Stroke = DashStyle.DashDot; ;                //	default / fallback

            return lineV_Stroke;
        }	//	getHLine_Stroke


        public  PdfSharp.Drawing.XDashStyle GetVLine_StrokeForPdf()
        {
            decimal width = GetLineStroke() / 2;
            if (GetHdrStrokeType() == null || LINESTROKETYPE_DottedLine.Equals(GetLineStrokeType()))
                lineV_StrokePdf = PdfSharp.Drawing.XDashStyle.Dot;
            else if (LINESTROKETYPE_SolidLine.Equals(GetLineStrokeType()))
                lineV_StrokePdf = PdfSharp.Drawing.XDashStyle.Solid;			//	-
            else if (LINESTROKETYPE_DashedLine.Equals(GetLineStrokeType()))
                lineV_StrokePdf = PdfSharp.Drawing.XDashStyle.Dash;		//	- -
            else if (LINESTROKETYPE_Dash_DottedLine.Equals(GetLineStrokeType()))
                lineV_StrokePdf = PdfSharp.Drawing.XDashStyle.DashDot; ;                //	default / fallback

            return lineV_StrokePdf;
        }	//	getHLine_Stroke

        private static CCache<int, MPrintTableFormat> _cache = new CCache<int, MPrintTableFormat>("AD_PrintTableFormat", 3);

        static public MPrintTableFormat Get(Ctx ctx, int AD_PrintTableFormat_ID, Font standard_font)
        {
            int ii = AD_PrintTableFormat_ID;
            //MPrintTableFormat tf = (MPrintTableFormat)_cache[ii];
            MPrintTableFormat tf = null;
            if (tf == null)
            {
                if (AD_PrintTableFormat_ID == 0)
                    tf = GetDefault(ctx);
                else
                    tf = new MPrintTableFormat(ctx, AD_PrintTableFormat_ID, null);
                _cache.Add(ii, tf);
            }
            tf.SetStandard_Font(standard_font);
            return tf;
        }	//	get


        static public MPrintTableFormat Get(Ctx ctx, int AD_PrintTableFormat_ID, int AD_PrintFont_ID)
        {
            return Get(ctx, AD_PrintTableFormat_ID, MPrintFont.Get(AD_PrintFont_ID).GetFont());
        }	//	get

        /// <summary>
        /// Get Default Table Format.
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>Default Table Format (need to set standard font)</returns>
        static public MPrintTableFormat GetDefault(Ctx ctx)
        {
            int AD_Client_ID = ctx.GetAD_Client_ID();

            MPrintTableFormat tf = null;
            String sql = "SELECT * FROM AD_PrintTableFormat "
                + "WHERE AD_Client_ID IN (0," + AD_Client_ID + ") AND IsActive='Y' "
                + "ORDER BY IsDefault DESC, AD_Client_ID DESC";
            try
            {
                DataSet ds = SqlExec.ExecuteQuery.ExecuteDataset(sql);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    tf = new MPrintTableFormat(ctx, dr, null);
                    break;  //in case there are multiple records, just select the first one
                }

            }
            catch 
            {
                //log, if any
            }

            return tf;
        }	//	GetDefault


        public new Decimal GetLineStroke()
        {
            Decimal? retValue = base.GetLineStroke();
            if (retValue == null || Env.ZERO.CompareTo(retValue) <= 0)
                retValue = new Decimal(1.0);
            return (Decimal)retValue;
        }	//	getLineStroke

        public Decimal GetVLineStroke()
        {
            Decimal? retValue = base.GetLineStroke();
            if (retValue == null || Env.ZERO.CompareTo(retValue) <= 0)
                retValue = new Decimal(1.0);
            return (Decimal)retValue;
        }	//	getVLineStroke

        private float[] GetPatternDotted(float width)
        {
            return new float[] { 2 * width, 2 * width };
        }	//	getPatternDotted


        private float[] GetPatternDashed(float width)
        {
            return new float[] { 10 * width, 4 * width };
        }	//	getPatternDashed

        private float[] GetPatternDash_Dotted(float width)
        {
            return new float[] { 10 * width, 2 * width, 2 * width, 2 * width };
        }	//	getPatternDash_Dotted

        public Color GetVLine_Color()
        {
            if (lineV_Color != null)
                return (Color)lineV_Color;
            int i = GetLine_PrintColor_ID();
            if (i != 0)
                lineV_Color = MPrintColor.Get(Env.GetCtx(), i).GetColor();
            if (lineV_Color == null)
                lineV_Color = Color.LightGray;
            return (Color)lineV_Color;
        }	//	getVLine_Color

        public Color GetHLine_Color()
        {
            if (lineH_Color != null)
                return (Color)lineH_Color;
            int i = GetLine_PrintColor_ID();
            if (i != 0)
                lineH_Color = MPrintColor.Get(Env.GetCtx(), i).GetColor();
            if (lineH_Color == null)
                lineH_Color = Color.LightGray;
            return (Color)lineH_Color;
        }	//	getHLine_Color
    }
}

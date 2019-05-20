namespace VAdvantage.Model
{

/** Generated Model - DO NOT CHANGE */
using System;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
/** Generated Model for AD_PrintTableFormat
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_PrintTableFormat : PO
{
public X_AD_PrintTableFormat (Context ctx, int AD_PrintTableFormat_ID, Trx trxName) : base (ctx, AD_PrintTableFormat_ID, trxName)
{
/** if (AD_PrintTableFormat_ID == 0)
{
SetAD_PrintTableFormat_ID (0);
SetIsDefault (false);
SetIsPaintBoundaryLines (false);
SetIsPaintHLines (false);
SetIsPaintHeaderLines (true);	// Y
SetIsPaintVLines (false);
SetIsPrintFunctionSymbols (false);
SetName (null);
}
 */
}
public X_AD_PrintTableFormat (Ctx ctx, int AD_PrintTableFormat_ID, Trx trxName) : base (ctx, AD_PrintTableFormat_ID, trxName)
{
/** if (AD_PrintTableFormat_ID == 0)
{
SetAD_PrintTableFormat_ID (0);
SetIsDefault (false);
SetIsPaintBoundaryLines (false);
SetIsPaintHLines (false);
SetIsPaintHeaderLines (true);	// Y
SetIsPaintVLines (false);
SetIsPrintFunctionSymbols (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintTableFormat (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintTableFormat (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintTableFormat (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_PrintTableFormat()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363097L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046308L;
/** AD_Table_ID=523 */
public static int Table_ID;
 // =523;

/** TableName=AD_PrintTableFormat */
public static String Table_Name="AD_PrintTableFormat";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO(Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_AD_PrintTableFormat[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Print Table Format.
@param AD_PrintTableFormat_ID Table Format in Reports */
public void SetAD_PrintTableFormat_ID (int AD_PrintTableFormat_ID)
{
if (AD_PrintTableFormat_ID < 1) throw new ArgumentException ("AD_PrintTableFormat_ID is mandatory.");
Set_ValueNoCheck ("AD_PrintTableFormat_ID", AD_PrintTableFormat_ID);
}
/** Get Print Table Format.
@return Table Format in Reports */
public int GetAD_PrintTableFormat_ID() 
{
Object ii = Get_Value("AD_PrintTableFormat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Footer Center.
@param FooterCenter Content of the center portion of the footer. */
public void SetFooterCenter (String FooterCenter)
{
if (FooterCenter != null && FooterCenter.Length > 255)
{
log.Warning("Length > 255 - truncated");
FooterCenter = FooterCenter.Substring(0,255);
}
Set_Value ("FooterCenter", FooterCenter);
}
/** Get Footer Center.
@return Content of the center portion of the footer. */
public String GetFooterCenter() 
{
return (String)Get_Value("FooterCenter");
}
/** Set Footer Left.
@param FooterLeft Content of the left portion of the footer. */
public void SetFooterLeft (String FooterLeft)
{
if (FooterLeft != null && FooterLeft.Length > 255)
{
log.Warning("Length > 255 - truncated");
FooterLeft = FooterLeft.Substring(0,255);
}
Set_Value ("FooterLeft", FooterLeft);
}
/** Get Footer Left.
@return Content of the left portion of the footer. */
public String GetFooterLeft() 
{
return (String)Get_Value("FooterLeft");
}
/** Set Footer Right.
@param FooterRight Content of the right portion of the footer. */
public void SetFooterRight (String FooterRight)
{
if (FooterRight != null && FooterRight.Length > 255)
{
log.Warning("Length > 255 - truncated");
FooterRight = FooterRight.Substring(0,255);
}
Set_Value ("FooterRight", FooterRight);
}
/** Get Footer Right.
@return Content of the right portion of the footer. */
public String GetFooterRight() 
{
return (String)Get_Value("FooterRight");
}

/** FunctBG_PrintColor_ID AD_Reference_ID=266 */
public static int FUNCTBG_PRINTCOLOR_ID_AD_Reference_ID=266;
/** Set Function BG Color.
@param FunctBG_PrintColor_ID Function Background Color */
public void SetFunctBG_PrintColor_ID (int FunctBG_PrintColor_ID)
{
if (FunctBG_PrintColor_ID <= 0) Set_Value ("FunctBG_PrintColor_ID", null);
else
Set_Value ("FunctBG_PrintColor_ID", FunctBG_PrintColor_ID);
}
/** Get Function BG Color.
@return Function Background Color */
public int GetFunctBG_PrintColor_ID() 
{
Object ii = Get_Value("FunctBG_PrintColor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** FunctFG_PrintColor_ID AD_Reference_ID=266 */
public static int FUNCTFG_PRINTCOLOR_ID_AD_Reference_ID=266;
/** Set Function Color.
@param FunctFG_PrintColor_ID Function Foreground Color */
public void SetFunctFG_PrintColor_ID (int FunctFG_PrintColor_ID)
{
if (FunctFG_PrintColor_ID <= 0) Set_Value ("FunctFG_PrintColor_ID", null);
else
Set_Value ("FunctFG_PrintColor_ID", FunctFG_PrintColor_ID);
}
/** Get Function Color.
@return Function Foreground Color */
public int GetFunctFG_PrintColor_ID() 
{
Object ii = Get_Value("FunctFG_PrintColor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Funct_PrintFont_ID AD_Reference_ID=267 */
public static int FUNCT_PRINTFONT_ID_AD_Reference_ID=267;
/** Set Function Font.
@param Funct_PrintFont_ID Function row Font */
public void SetFunct_PrintFont_ID (int Funct_PrintFont_ID)
{
if (Funct_PrintFont_ID <= 0) Set_Value ("Funct_PrintFont_ID", null);
else
Set_Value ("Funct_PrintFont_ID", Funct_PrintFont_ID);
}
/** Get Function Font.
@return Function row Font */
public int GetFunct_PrintFont_ID() 
{
Object ii = Get_Value("Funct_PrintFont_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** HdrLine_PrintColor_ID AD_Reference_ID=266 */
public static int HDRLINE_PRINTCOLOR_ID_AD_Reference_ID=266;
/** Set Header Line Color.
@param HdrLine_PrintColor_ID Table header row line color */
public void SetHdrLine_PrintColor_ID (int HdrLine_PrintColor_ID)
{
if (HdrLine_PrintColor_ID <= 0) Set_Value ("HdrLine_PrintColor_ID", null);
else
Set_Value ("HdrLine_PrintColor_ID", HdrLine_PrintColor_ID);
}
/** Get Header Line Color.
@return Table header row line color */
public int GetHdrLine_PrintColor_ID() 
{
Object ii = Get_Value("HdrLine_PrintColor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Header Stroke.
@param HdrStroke Width of the Header Line Stroke */
public void SetHdrStroke (Decimal? HdrStroke)
{
Set_Value ("HdrStroke", (Decimal?)HdrStroke);
}
/** Get Header Stroke.
@return Width of the Header Line Stroke */
public Decimal GetHdrStroke() 
{
Object bd =Get_Value("HdrStroke");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** HdrStrokeType AD_Reference_ID=312 */
public static int HDRSTROKETYPE_AD_Reference_ID=312;
/** Dash-Dotted Line = 2 */
public static String HDRSTROKETYPE_Dash_DottedLine = "2";
/** Dashed Line = D */
public static String HDRSTROKETYPE_DashedLine = "D";
/** Solid Line = S */
public static String HDRSTROKETYPE_SolidLine = "S";
/** Dotted Line = d */
public static String HDRSTROKETYPE_DottedLine = "d";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsHdrStrokeTypeValid (String test)
{
return test == null || test.Equals("2") || test.Equals("D") || test.Equals("S") || test.Equals("d");
}
/** Set Header Stroke Type.
@param HdrStrokeType Type of the Header Line Stroke */
public void SetHdrStrokeType (String HdrStrokeType)
{
if (!IsHdrStrokeTypeValid(HdrStrokeType))
throw new ArgumentException ("HdrStrokeType Invalid value - " + HdrStrokeType + " - Reference_ID=312 - 2 - D - S - d");
if (HdrStrokeType != null && HdrStrokeType.Length > 1)
{
log.Warning("Length > 1 - truncated");
HdrStrokeType = HdrStrokeType.Substring(0,1);
}
Set_Value ("HdrStrokeType", HdrStrokeType);
}
/** Get Header Stroke Type.
@return Type of the Header Line Stroke */
public String GetHdrStrokeType() 
{
return (String)Get_Value("HdrStrokeType");
}

/** HdrTextBG_PrintColor_ID AD_Reference_ID=266 */
public static int HDRTEXTBG_PRINTCOLOR_ID_AD_Reference_ID=266;
/** Set Header Row BG Color.
@param HdrTextBG_PrintColor_ID Background color of header row */
public void SetHdrTextBG_PrintColor_ID (int HdrTextBG_PrintColor_ID)
{
if (HdrTextBG_PrintColor_ID <= 0) Set_Value ("HdrTextBG_PrintColor_ID", null);
else
Set_Value ("HdrTextBG_PrintColor_ID", HdrTextBG_PrintColor_ID);
}
/** Get Header Row BG Color.
@return Background color of header row */
public int GetHdrTextBG_PrintColor_ID() 
{
Object ii = Get_Value("HdrTextBG_PrintColor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** HdrTextFG_PrintColor_ID AD_Reference_ID=266 */
public static int HDRTEXTFG_PRINTCOLOR_ID_AD_Reference_ID=266;
/** Set Header Row Color.
@param HdrTextFG_PrintColor_ID Foreground color of the table header row */
public void SetHdrTextFG_PrintColor_ID (int HdrTextFG_PrintColor_ID)
{
if (HdrTextFG_PrintColor_ID <= 0) Set_Value ("HdrTextFG_PrintColor_ID", null);
else
Set_Value ("HdrTextFG_PrintColor_ID", HdrTextFG_PrintColor_ID);
}
/** Get Header Row Color.
@return Foreground color of the table header row */
public int GetHdrTextFG_PrintColor_ID() 
{
Object ii = Get_Value("HdrTextFG_PrintColor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Hdr_PrintFont_ID AD_Reference_ID=267 */
public static int HDR_PRINTFONT_ID_AD_Reference_ID=267;
/** Set Header Row Font.
@param Hdr_PrintFont_ID Header row Font */
public void SetHdr_PrintFont_ID (int Hdr_PrintFont_ID)
{
if (Hdr_PrintFont_ID <= 0) Set_Value ("Hdr_PrintFont_ID", null);
else
Set_Value ("Hdr_PrintFont_ID", Hdr_PrintFont_ID);
}
/** Get Header Row Font.
@return Header row Font */
public int GetHdr_PrintFont_ID() 
{
Object ii = Get_Value("Hdr_PrintFont_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Header Center.
@param HeaderCenter Content of the center portion of the header. */
public void SetHeaderCenter (String HeaderCenter)
{
if (HeaderCenter != null && HeaderCenter.Length > 255)
{
log.Warning("Length > 255 - truncated");
HeaderCenter = HeaderCenter.Substring(0,255);
}
Set_Value ("HeaderCenter", HeaderCenter);
}
/** Get Header Center.
@return Content of the center portion of the header. */
public String GetHeaderCenter() 
{
return (String)Get_Value("HeaderCenter");
}
/** Set Header Left.
@param HeaderLeft Content of the left portion of the header. */
public void SetHeaderLeft (String HeaderLeft)
{
if (HeaderLeft != null && HeaderLeft.Length > 255)
{
log.Warning("Length > 255 - truncated");
HeaderLeft = HeaderLeft.Substring(0,255);
}
Set_Value ("HeaderLeft", HeaderLeft);
}
/** Get Header Left.
@return Content of the left portion of the header. */
public String GetHeaderLeft() 
{
return (String)Get_Value("HeaderLeft");
}
/** Set Header Right.
@param HeaderRight Content of the right portion of the header. */
public void SetHeaderRight (String HeaderRight)
{
if (HeaderRight != null && HeaderRight.Length > 255)
{
log.Warning("Length > 255 - truncated");
HeaderRight = HeaderRight.Substring(0,255);
}
Set_Value ("HeaderRight", HeaderRight);
}
/** Get Header Right.
@return Content of the right portion of the header. */
public String GetHeaderRight() 
{
return (String)Get_Value("HeaderRight");
}
/** Set Image attached.
@param ImageIsAttached The image to be printed is attached to the record */
public void SetImageIsAttached (Boolean ImageIsAttached)
{
Set_Value ("ImageIsAttached", ImageIsAttached);
}
/** Get Image attached.
@return The image to be printed is attached to the record */
public Boolean IsImageIsAttached() 
{
Object oo = Get_Value("ImageIsAttached");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Image URL.
@param ImageURL URL of  image */
public void SetImageURL (String ImageURL)
{
if (ImageURL != null && ImageURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
ImageURL = ImageURL.Substring(0,120);
}
Set_Value ("ImageURL", ImageURL);
}
/** Get Image URL.
@return URL of  image */
public String GetImageURL() 
{
return (String)Get_Value("ImageURL");
}
/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault)
{
Set_Value ("IsDefault", IsDefault);
}
/** Get Default.
@return Default value */
public Boolean IsDefault() 
{
Object oo = Get_Value("IsDefault");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Paint Boundary Lines.
@param IsPaintBoundaryLines Paint table boundary lines */
public void SetIsPaintBoundaryLines (Boolean IsPaintBoundaryLines)
{
Set_Value ("IsPaintBoundaryLines", IsPaintBoundaryLines);
}
/** Get Paint Boundary Lines.
@return Paint table boundary lines */
public Boolean IsPaintBoundaryLines() 
{
Object oo = Get_Value("IsPaintBoundaryLines");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Paint Horizontal Lines.
@param IsPaintHLines Paint horizontal lines */
public void SetIsPaintHLines (Boolean IsPaintHLines)
{
Set_Value ("IsPaintHLines", IsPaintHLines);
}
/** Get Paint Horizontal Lines.
@return Paint horizontal lines */
public Boolean IsPaintHLines() 
{
Object oo = Get_Value("IsPaintHLines");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Paint Header Lines.
@param IsPaintHeaderLines Paint Lines over/under the Header Line */
public void SetIsPaintHeaderLines (Boolean IsPaintHeaderLines)
{
Set_Value ("IsPaintHeaderLines", IsPaintHeaderLines);
}
/** Get Paint Header Lines.
@return Paint Lines over/under the Header Line */
public Boolean IsPaintHeaderLines() 
{
Object oo = Get_Value("IsPaintHeaderLines");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Paint Vertical Lines.
@param IsPaintVLines Paint vertical lines */
public void SetIsPaintVLines (Boolean IsPaintVLines)
{
Set_Value ("IsPaintVLines", IsPaintVLines);
}
/** Get Paint Vertical Lines.
@return Paint vertical lines */
public Boolean IsPaintVLines() 
{
Object oo = Get_Value("IsPaintVLines");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Print Function Symbols.
@param IsPrintFunctionSymbols Print Symbols for Functions (Sum, Average, Count) */
public void SetIsPrintFunctionSymbols (Boolean IsPrintFunctionSymbols)
{
Set_Value ("IsPrintFunctionSymbols", IsPrintFunctionSymbols);
}
/** Get Print Function Symbols.
@return Print Symbols for Functions (Sum, Average, Count) */
public Boolean IsPrintFunctionSymbols() 
{
Object oo = Get_Value("IsPrintFunctionSymbols");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Line Stroke.
@param LineStroke Width of the Line Stroke */
public void SetLineStroke (Decimal? LineStroke)
{
Set_Value ("LineStroke", (Decimal?)LineStroke);
}
/** Get Line Stroke.
@return Width of the Line Stroke */
public Decimal GetLineStroke() 
{
Object bd =Get_Value("LineStroke");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** LineStrokeType AD_Reference_ID=312 */
public static int LINESTROKETYPE_AD_Reference_ID=312;
/** Dash-Dotted Line = 2 */
public static String LINESTROKETYPE_Dash_DottedLine = "2";
/** Dashed Line = D */
public static String LINESTROKETYPE_DashedLine = "D";
/** Solid Line = S */
public static String LINESTROKETYPE_SolidLine = "S";
/** Dotted Line = d */
public static String LINESTROKETYPE_DottedLine = "d";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsLineStrokeTypeValid (String test)
{
return test == null || test.Equals("2") || test.Equals("D") || test.Equals("S") || test.Equals("d");
}
/** Set Line Stroke Type.
@param LineStrokeType Type of the Line Stroke */
public void SetLineStrokeType (String LineStrokeType)
{
if (!IsLineStrokeTypeValid(LineStrokeType))
throw new ArgumentException ("LineStrokeType Invalid value - " + LineStrokeType + " - Reference_ID=312 - 2 - D - S - d");
if (LineStrokeType != null && LineStrokeType.Length > 1)
{
log.Warning("Length > 1 - truncated");
LineStrokeType = LineStrokeType.Substring(0,1);
}
Set_Value ("LineStrokeType", LineStrokeType);
}
/** Get Line Stroke Type.
@return Type of the Line Stroke */
public String GetLineStrokeType() 
{
return (String)Get_Value("LineStrokeType");
}

/** Line_PrintColor_ID AD_Reference_ID=266 */
public static int LINE_PRINTCOLOR_ID_AD_Reference_ID=266;
/** Set Line Color.
@param Line_PrintColor_ID Table line color */
public void SetLine_PrintColor_ID (int Line_PrintColor_ID)
{
if (Line_PrintColor_ID <= 0) Set_Value ("Line_PrintColor_ID", null);
else
Set_Value ("Line_PrintColor_ID", Line_PrintColor_ID);
}
/** Get Line Color.
@return Table line color */
public int GetLine_PrintColor_ID() 
{
Object ii = Get_Value("Line_PrintColor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
}

}

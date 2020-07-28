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
/** Generated Model for AD_Color
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Color : PO
{
public X_AD_Color (Context ctx, int AD_Color_ID, Trx trxName) : base (ctx, AD_Color_ID, trxName)
{
/** if (AD_Color_ID == 0)
{
SetAD_Color_ID (0);
SetAlpha (0);
SetBlue (0);
SetColorType (null);
SetGreen (0);
SetImageAlpha (0.0);
SetIsDefault (false);
SetName (null);
SetRed (0);
}
 */
}
public X_AD_Color (Ctx ctx, int AD_Color_ID, Trx trxName) : base (ctx, AD_Color_ID, trxName)
{
/** if (AD_Color_ID == 0)
{
SetAD_Color_ID (0);
SetAlpha (0);
SetBlue (0);
SetColorType (null);
SetGreen (0);
SetImageAlpha (0.0);
SetIsDefault (false);
SetName (null);
SetRed (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Color (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Color (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Color (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Color()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360950L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044161L;
/** AD_Table_ID=457 */
public static int Table_ID;
 // =457;

/** TableName=AD_Color */
public static String Table_Name="AD_Color";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_AD_Color[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set System Color.
@param AD_Color_ID Color for backgrounds or indicators */
public void SetAD_Color_ID (int AD_Color_ID)
{
if (AD_Color_ID < 1) throw new ArgumentException ("AD_Color_ID is mandatory.");
Set_ValueNoCheck ("AD_Color_ID", AD_Color_ID);
}
/** Get System Color.
@return Color for backgrounds or indicators */
public int GetAD_Color_ID() 
{
Object ii = Get_Value("AD_Color_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Image.
@param AD_Image_ID Image or Icon */
public void SetAD_Image_ID (int AD_Image_ID)
{
if (AD_Image_ID <= 0) Set_Value ("AD_Image_ID", null);
else
Set_Value ("AD_Image_ID", AD_Image_ID);
}
/** Get Image.
@return Image or Icon */
public int GetAD_Image_ID() 
{
Object ii = Get_Value("AD_Image_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Alpha.
@param Alpha Color Alpha value 0-255 */
public void SetAlpha (int Alpha)
{
Set_Value ("Alpha", Alpha);
}
/** Get Alpha.
@return Color Alpha value 0-255 */
public int GetAlpha() 
{
Object ii = Get_Value("Alpha");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set 2nd Alpha.
@param Alpha_1 Alpha value for second color */
public void SetAlpha_1 (int Alpha_1)
{
Set_Value ("Alpha_1", Alpha_1);
}
/** Get 2nd Alpha.
@return Alpha value for second color */
public int GetAlpha_1() 
{
Object ii = Get_Value("Alpha_1");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Blue.
@param Blue Color RGB blue value */
public void SetBlue (int Blue)
{
Set_Value ("Blue", Blue);
}
/** Get Blue.
@return Color RGB blue value */
public int GetBlue() 
{
Object ii = Get_Value("Blue");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set 2nd Blue.
@param Blue_1 RGB value for second color */
public void SetBlue_1 (int Blue_1)
{
Set_Value ("Blue_1", Blue_1);
}
/** Get 2nd Blue.
@return RGB value for second color */
public int GetBlue_1() 
{
Object ii = Get_Value("Blue_1");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** ColorType AD_Reference_ID=243 */
public static int COLORTYPE_AD_Reference_ID=243;
/** Normal (Flat) = F */
public static String COLORTYPE_NormalFlat = "F";
/** Gradient = G */
public static String COLORTYPE_Gradient = "G";
/** Line = L */
public static String COLORTYPE_Line = "L";
/** Texture (Picture) = T */
public static String COLORTYPE_TexturePicture = "T";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsColorTypeValid (String test)
{
return test.Equals("F") || test.Equals("G") || test.Equals("L") || test.Equals("T");
}
/** Set Color Type.
@param ColorType Color presentation for this color */
public void SetColorType (String ColorType)
{
if (ColorType == null) throw new ArgumentException ("ColorType is mandatory");
if (!IsColorTypeValid(ColorType))
throw new ArgumentException ("ColorType Invalid value - " + ColorType + " - Reference_ID=243 - F - G - L - T");
if (ColorType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ColorType = ColorType.Substring(0,1);
}
Set_Value ("ColorType", ColorType);
}
/** Get Color Type.
@return Color presentation for this color */
public String GetColorType() 
{
return (String)Get_Value("ColorType");
}
/** Set Green.
@param Green RGB value */
public void SetGreen (int Green)
{
Set_Value ("Green", Green);
}
/** Get Green.
@return RGB value */
public int GetGreen() 
{
Object ii = Get_Value("Green");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set 2nd Green.
@param Green_1 RGB value for second color */
public void SetGreen_1 (int Green_1)
{
Set_Value ("Green_1", Green_1);
}
/** Get 2nd Green.
@return RGB value for second color */
public int GetGreen_1() 
{
Object ii = Get_Value("Green_1");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Image Alpha.
@param ImageAlpha Image Texture Composite Alpha */
public void SetImageAlpha (Decimal? ImageAlpha)
{
if (ImageAlpha == null) throw new ArgumentException ("ImageAlpha is mandatory.");
Set_Value ("ImageAlpha", (Decimal?)ImageAlpha);
}
/** Get Image Alpha.
@return Image Texture Composite Alpha */
public Decimal GetImageAlpha() 
{
Object bd =Get_Value("ImageAlpha");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Line Distance.
@param LineDistance Distance between lines */
public void SetLineDistance (int LineDistance)
{
Set_Value ("LineDistance", LineDistance);
}
/** Get Line Distance.
@return Distance between lines */
public int GetLineDistance() 
{
Object ii = Get_Value("LineDistance");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Line Width.
@param LineWidth Width of the lines */
public void SetLineWidth (int LineWidth)
{
Set_Value ("LineWidth", LineWidth);
}
/** Get Line Width.
@return Width of the lines */
public int GetLineWidth() 
{
Object ii = Get_Value("LineWidth");
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
/** Set Red.
@param Red RGB value */
public void SetRed (int Red)
{
Set_Value ("Red", Red);
}
/** Get Red.
@return RGB value */
public int GetRed() 
{
Object ii = Get_Value("Red");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set 2nd Red.
@param Red_1 RGB value for second color */
public void SetRed_1 (int Red_1)
{
Set_Value ("Red_1", Red_1);
}
/** Get 2nd Red.
@return RGB value for second color */
public int GetRed_1() 
{
Object ii = Get_Value("Red_1");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Repeat Distance.
@param RepeatDistance Distance in points to repeat gradient color - or zero */
public void SetRepeatDistance (int RepeatDistance)
{
Set_Value ("RepeatDistance", RepeatDistance);
}
/** Get Repeat Distance.
@return Distance in points to repeat gradient color - or zero */
public int GetRepeatDistance() 
{
Object ii = Get_Value("RepeatDistance");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** StartPoint AD_Reference_ID=248 */
public static int STARTPOINT_AD_Reference_ID=248;
/** North = 1 */
public static String STARTPOINT_North = "1";
/** North East = 2 */
public static String STARTPOINT_NorthEast = "2";
/** East = 3 */
public static String STARTPOINT_East = "3";
/** South East = 4 */
public static String STARTPOINT_SouthEast = "4";
/** South = 5 */
public static String STARTPOINT_South = "5";
/** South West = 6 */
public static String STARTPOINT_SouthWest = "6";
/** West = 7 */
public static String STARTPOINT_West = "7";
/** North West = 8 */
public static String STARTPOINT_NorthWest = "8";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsStartPointValid (String test)
{
return test == null || test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("4") || test.Equals("5") || test.Equals("6") || test.Equals("7") || test.Equals("8");
}
/** Set Start Point.
@param StartPoint Start point of the gradient colors */
public void SetStartPoint (String StartPoint)
{
if (!IsStartPointValid(StartPoint))
throw new ArgumentException ("StartPoint Invalid value - " + StartPoint + " - Reference_ID=248 - 1 - 2 - 3 - 4 - 5 - 6 - 7 - 8");
if (StartPoint != null && StartPoint.Length > 1)
{
log.Warning("Length > 1 - truncated");
StartPoint = StartPoint.Substring(0,1);
}
Set_Value ("StartPoint", StartPoint);
}
/** Get Start Point.
@return Start point of the gradient colors */
public String GetStartPoint() 
{
return (String)Get_Value("StartPoint");
}
}

}

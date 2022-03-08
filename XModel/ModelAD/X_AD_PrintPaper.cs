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
/** Generated Model for AD_PrintPaper
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_PrintPaper : PO
{
public X_AD_PrintPaper (Context ctx, int AD_PrintPaper_ID, Trx trxName) : base (ctx, AD_PrintPaper_ID, trxName)
{
/** if (AD_PrintPaper_ID == 0)
{
SetAD_PrintPaper_ID (0);
SetCode (null);	// iso-a4
SetIsDefault (false);
SetIsLandscape (true);	// Y
SetMarginBottom (0);	// 36
SetMarginLeft (0);	// 36
SetMarginRight (0);	// 36
SetMarginTop (0);	// 36
SetName (null);
}
 */
}
public X_AD_PrintPaper (Ctx ctx, int AD_PrintPaper_ID, Trx trxName) : base (ctx, AD_PrintPaper_ID, trxName)
{
/** if (AD_PrintPaper_ID == 0)
{
SetAD_PrintPaper_ID (0);
SetCode (null);	// iso-a4
SetIsDefault (false);
SetIsLandscape (true);	// Y
SetMarginBottom (0);	// 36
SetMarginLeft (0);	// 36
SetMarginRight (0);	// 36
SetMarginTop (0);	// 36
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintPaper (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintPaper (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintPaper (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_PrintPaper()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362987L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046198L;
/** AD_Table_ID=492 */
public static int Table_ID;
 // =492;

/** TableName=AD_PrintPaper */
public static String Table_Name="AD_PrintPaper";

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
StringBuilder sb = new StringBuilder ("X_AD_PrintPaper[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Print Paper.
@param AD_PrintPaper_ID Printer paper definition */
public void SetAD_PrintPaper_ID (int AD_PrintPaper_ID)
{
if (AD_PrintPaper_ID < 1) throw new ArgumentException ("AD_PrintPaper_ID is mandatory.");
Set_ValueNoCheck ("AD_PrintPaper_ID", AD_PrintPaper_ID);
}
/** Get Print Paper.
@return Printer paper definition */
public int GetAD_PrintPaper_ID() 
{
Object ii = Get_Value("AD_PrintPaper_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Code.
@param Code Code to execute or to validate */
public void SetCode (String Code)
{
if (Code == null) throw new ArgumentException ("Code is mandatory.");
if (Code.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Code = Code.Substring(0,2000);
}
Set_Value ("Code", Code);
}
/** Get Code.
@return Code to execute or to validate */
public String GetCode() 
{
return (String)Get_Value("Code");
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

/** DimensionUnits AD_Reference_ID=375 */
public static int DIMENSIONUNITS_AD_Reference_ID=375;
/** Inch = I */
public static String DIMENSIONUNITS_Inch = "I";
/** MM = M */
public static String DIMENSIONUNITS_MM = "M";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDimensionUnitsValid (String test)
{
return test == null || test.Equals("I") || test.Equals("M");
}
/** Set Dimension Units.
@param DimensionUnits Units of Dimension */
public void SetDimensionUnits (String DimensionUnits)
{
if (!IsDimensionUnitsValid(DimensionUnits))
throw new ArgumentException ("DimensionUnits Invalid value - " + DimensionUnits + " - Reference_ID=375 - I - M");
if (DimensionUnits != null && DimensionUnits.Length > 1)
{
log.Warning("Length > 1 - truncated");
DimensionUnits = DimensionUnits.Substring(0,1);
}
Set_Value ("DimensionUnits", DimensionUnits);
}
/** Get Dimension Units.
@return Units of Dimension */
public String GetDimensionUnits() 
{
return (String)Get_Value("DimensionUnits");
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
/** Set Landscape.
@param IsLandscape Landscape orientation */
public void SetIsLandscape (Boolean IsLandscape)
{
Set_Value ("IsLandscape", IsLandscape);
}
/** Get Landscape.
@return Landscape orientation */
public Boolean IsLandscape() 
{
Object oo = Get_Value("IsLandscape");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Bottom Margin.
@param MarginBottom Bottom Space in 1/72 inch */
public void SetMarginBottom (int MarginBottom)
{
Set_Value ("MarginBottom", MarginBottom);
}
/** Get Bottom Margin.
@return Bottom Space in 1/72 inch */
public int GetMarginBottom() 
{
Object ii = Get_Value("MarginBottom");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Left Margin.
@param MarginLeft Left Space in 1/72 inch */
public void SetMarginLeft (int MarginLeft)
{
Set_Value ("MarginLeft", MarginLeft);
}
/** Get Left Margin.
@return Left Space in 1/72 inch */
public int GetMarginLeft() 
{
Object ii = Get_Value("MarginLeft");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Right Margin.
@param MarginRight Right Space in 1/72 inch */
public void SetMarginRight (int MarginRight)
{
Set_Value ("MarginRight", MarginRight);
}
/** Get Right Margin.
@return Right Space in 1/72 inch */
public int GetMarginRight() 
{
Object ii = Get_Value("MarginRight");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Top Margin.
@param MarginTop Top Space in 1/72 inch */
public void SetMarginTop (int MarginTop)
{
Set_Value ("MarginTop", MarginTop);
}
/** Get Top Margin.
@return Top Space in 1/72 inch */
public int GetMarginTop() 
{
Object ii = Get_Value("MarginTop");
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
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Size X.
@param SizeX X (horizontal) dimension size */
public void SetSizeX (Decimal? SizeX)
{
Set_Value ("SizeX", (Decimal?)SizeX);
}
/** Get Size X.
@return X (horizontal) dimension size */
public Decimal GetSizeX() 
{
Object bd =Get_Value("SizeX");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Size Y.
@param SizeY Y (vertical) dimension size */
public void SetSizeY (Decimal? SizeY)
{
Set_Value ("SizeY", (Decimal?)SizeY);
}
/** Get Size Y.
@return Y (vertical) dimension size */
public Decimal GetSizeY() 
{
Object bd =Get_Value("SizeY");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}

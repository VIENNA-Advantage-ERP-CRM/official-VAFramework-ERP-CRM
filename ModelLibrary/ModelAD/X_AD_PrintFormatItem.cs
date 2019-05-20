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
/** Generated Model for AD_PrintFormatItem
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_PrintFormatItem : PO
{
public X_AD_PrintFormatItem (Context ctx, int AD_PrintFormatItem_ID, Trx trxName) : base (ctx, AD_PrintFormatItem_ID, trxName)
{
/** if (AD_PrintFormatItem_ID == 0)
{
SetAD_PrintFormatItem_ID (0);
SetAD_PrintFormat_ID (0);
SetFieldAlignmentType (null);	// D
SetImageIsAttached (false);
SetIsAveraged (false);
SetIsCentrallyMaintained (false);
SetIsCounted (false);
SetIsDeviationCalc (false);
SetIsFilledRectangle (false);	// N
SetIsFixedWidth (false);
SetIsGroupBy (false);
SetIsHeightOneLine (true);	// Y
SetIsImageField (false);
SetIsMaxCalc (false);
SetIsMinCalc (false);
SetIsNextLine (true);	// Y
SetIsNextPage (false);
SetIsOrderBy (false);
SetIsPageBreak (false);
SetIsPrinted (true);	// Y
SetIsRelativePosition (true);	// Y
SetIsRunningTotal (false);
SetIsSetNLPosition (false);
SetIsSummarized (false);
SetIsSuppressNull (false);
SetIsVarianceCalc (false);
SetLineAlignmentType (null);	// X
SetMaxHeight (0);
SetMaxWidth (0);
SetName (null);
SetPrintAreaType (null);	// C
SetPrintFormatType (null);	// F
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM AD_PrintFormatItem WHERE AD_PrintFormat_ID=@AD_PrintFormat_ID@
SetSortNo (0);
SetXPosition (0);
SetXSpace (0);
SetYPosition (0);
SetYSpace (0);
}
 */
}
public X_AD_PrintFormatItem (Ctx ctx, int AD_PrintFormatItem_ID, Trx trxName) : base (ctx, AD_PrintFormatItem_ID, trxName)
{
/** if (AD_PrintFormatItem_ID == 0)
{
SetAD_PrintFormatItem_ID (0);
SetAD_PrintFormat_ID (0);
SetFieldAlignmentType (null);	// D
SetImageIsAttached (false);
SetIsAveraged (false);
SetIsCentrallyMaintained (false);
SetIsCounted (false);
SetIsDeviationCalc (false);
SetIsFilledRectangle (false);	// N
SetIsFixedWidth (false);
SetIsGroupBy (false);
SetIsHeightOneLine (true);	// Y
SetIsImageField (false);
SetIsMaxCalc (false);
SetIsMinCalc (false);
SetIsNextLine (true);	// Y
SetIsNextPage (false);
SetIsOrderBy (false);
SetIsPageBreak (false);
SetIsPrinted (true);	// Y
SetIsRelativePosition (true);	// Y
SetIsRunningTotal (false);
SetIsSetNLPosition (false);
SetIsSummarized (false);
SetIsSuppressNull (false);
SetIsVarianceCalc (false);
SetLineAlignmentType (null);	// X
SetMaxHeight (0);
SetMaxWidth (0);
SetName (null);
SetPrintAreaType (null);	// C
SetPrintFormatType (null);	// F
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM AD_PrintFormatItem WHERE AD_PrintFormat_ID=@AD_PrintFormat_ID@
SetSortNo (0);
SetXPosition (0);
SetXSpace (0);
SetYPosition (0);
SetYSpace (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintFormatItem (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintFormatItem (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintFormatItem (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_PrintFormatItem()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362799L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046010L;
/** AD_Table_ID=489 */
public static int Table_ID;
 // =489;

/** TableName=AD_PrintFormatItem */
public static String Table_Name="AD_PrintFormatItem";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
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
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_AD_PrintFormatItem[").Append(Get_ID()).Append("-").Append(GetName()).Append("]");
return sb.ToString();
}
/** Set Column.
@param AD_Column_ID Column in the table */
public void SetAD_Column_ID (int AD_Column_ID)
{
if (AD_Column_ID <= 0) Set_Value ("AD_Column_ID", null);
else
Set_Value ("AD_Column_ID", AD_Column_ID);
}
/** Get Column.
@return Column in the table */
public int GetAD_Column_ID() 
{
Object ii = Get_Value("AD_Column_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Print Color.
@param AD_PrintColor_ID Color used for printing and display */
public void SetAD_PrintColor_ID (int AD_PrintColor_ID)
{
if (AD_PrintColor_ID <= 0) Set_Value ("AD_PrintColor_ID", null);
else
Set_Value ("AD_PrintColor_ID", AD_PrintColor_ID);
}
/** Get Print Color.
@return Color used for printing and display */
public int GetAD_PrintColor_ID() 
{
Object ii = Get_Value("AD_PrintColor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Print Font.
@param AD_PrintFont_ID Maintain Print Font */
public void SetAD_PrintFont_ID (int AD_PrintFont_ID)
{
if (AD_PrintFont_ID <= 0) Set_Value ("AD_PrintFont_ID", null);
else
Set_Value ("AD_PrintFont_ID", AD_PrintFont_ID);
}
/** Get Print Font.
@return Maintain Print Font */
public int GetAD_PrintFont_ID() 
{
Object ii = Get_Value("AD_PrintFont_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AD_PrintFormatChild_ID AD_Reference_ID=259 */
public static int AD_PRINTFORMATCHILD_ID_AD_Reference_ID=259;
/** Set Included Print Format.
@param AD_PrintFormatChild_ID Print format that is included here. */
public void SetAD_PrintFormatChild_ID (int AD_PrintFormatChild_ID)
{
if (AD_PrintFormatChild_ID <= 0) Set_Value ("AD_PrintFormatChild_ID", null);
else
Set_Value ("AD_PrintFormatChild_ID", AD_PrintFormatChild_ID);
}
/** Get Included Print Format.
@return Print format that is included here. */
public int GetAD_PrintFormatChild_ID() 
{
Object ii = Get_Value("AD_PrintFormatChild_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Print Format Item.
@param AD_PrintFormatItem_ID Item/Column in the Print format */
public void SetAD_PrintFormatItem_ID (int AD_PrintFormatItem_ID)
{
if (AD_PrintFormatItem_ID < 1) throw new ArgumentException ("AD_PrintFormatItem_ID is mandatory.");
Set_ValueNoCheck ("AD_PrintFormatItem_ID", AD_PrintFormatItem_ID);
}
/** Get Print Format Item.
@return Item/Column in the Print format */
public int GetAD_PrintFormatItem_ID() 
{
Object ii = Get_Value("AD_PrintFormatItem_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Print Format.
@param AD_PrintFormat_ID Data Print Format */
public void SetAD_PrintFormat_ID (int AD_PrintFormat_ID)
{
if (AD_PrintFormat_ID < 1) throw new ArgumentException ("AD_PrintFormat_ID is mandatory.");
Set_ValueNoCheck ("AD_PrintFormat_ID", AD_PrintFormat_ID);
}
/** Get Print Format.
@return Data Print Format */
public int GetAD_PrintFormat_ID() 
{
Object ii = Get_Value("AD_PrintFormat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Graph.
@param AD_PrintGraph_ID Graph included in Reports */
public void SetAD_PrintGraph_ID (int AD_PrintGraph_ID)
{
if (AD_PrintGraph_ID <= 0) Set_Value ("AD_PrintGraph_ID", null);
else
Set_Value ("AD_PrintGraph_ID", AD_PrintGraph_ID);
}
/** Get Graph.
@return Graph included in Reports */
public int GetAD_PrintGraph_ID() 
{
Object ii = Get_Value("AD_PrintGraph_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Arc Diameter.
@param ArcDiameter Arc Diameter for rounded Rectangles */
public void SetArcDiameter (int ArcDiameter)
{
Set_Value ("ArcDiameter", ArcDiameter);
}
/** Get Arc Diameter.
@return Arc Diameter for rounded Rectangles */
public int GetArcDiameter() 
{
Object ii = Get_Value("ArcDiameter");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** BarcodeType AD_Reference_ID=377 */
public static int BARCODETYPE_AD_Reference_ID=377;
/** Code 128 A character set = 28A */
public static String BARCODETYPE_Code128ACharacterSet = "28A";
/** Code 128 B character set = 28B */
public static String BARCODETYPE_Code128BCharacterSet = "28B";
/** Code 128 C character set = 28C */
public static String BARCODETYPE_Code128CCharacterSet = "28C";
/** Codabar 2 of 7 linear = 2o9 */
public static String BARCODETYPE_Codabar2Of7Linear = "2o9";
/** Code 39  3 of 9 linear with Checksum = 3O9 */
public static String BARCODETYPE_Code393Of9LinearWithChecksum = "3O9";
/** Code 39  3 of 9 linear w/o Checksum = 3o9 */
public static String BARCODETYPE_Code393Of9LinearWOChecksum = "3o9";
/** PDF417 two dimensional = 417 */
public static String BARCODETYPE_PDF417TwoDimensional = "417";
/** SCC-14 shipping code UCC/EAN 128 = C14 */
public static String BARCODETYPE_SCC_14ShippingCodeUCCEAN128 = "C14";
/** SSCC-18 number UCC/EAN 128 = C18 */
public static String BARCODETYPE_SSCC_18NumberUCCEAN128 = "C18";
/** Code 128 dynamically switching = C28 */
public static String BARCODETYPE_Code128DynamicallySwitching = "C28";
/** Code 39 linear with Checksum = C39 */
public static String BARCODETYPE_Code39LinearWithChecksum = "C39";
/** Codeabar linear = COD */
public static String BARCODETYPE_CodeabarLinear = "COD";
/** EAN 128 = E28 */
public static String BARCODETYPE_EAN128 = "E28";
/** Global Trade Item No GTIN UCC/EAN 128 = GTN */
public static String BARCODETYPE_GlobalTradeItemNoGTINUCCEAN128 = "GTN";
/** Codabar Monarch linear = MON */
public static String BARCODETYPE_CodabarMonarchLinear = "MON";
/** Codabar NW-7 linear = NW7 */
public static String BARCODETYPE_CodabarNW_7Linear = "NW7";
/** Shipment ID number UCC/EAN 128 = SID */
public static String BARCODETYPE_ShipmentIDNumberUCCEAN128 = "SID";
/** UCC 128 = U28 */
public static String BARCODETYPE_UCC128 = "U28";
/** Code 39 USD3 with Checksum = US3 */
public static String BARCODETYPE_Code39USD3WithChecksum = "US3";
/** Codabar USD-4 linear = US4 */
public static String BARCODETYPE_CodabarUSD_4Linear = "US4";
/** US Postal Service UCC/EAN 128 = USP */
public static String BARCODETYPE_USPostalServiceUCCEAN128 = "USP";
/** Code 39 linear w/o Checksum = c39 */
public static String BARCODETYPE_Code39LinearWOChecksum = "c39";
/** Code 39 USD3 w/o Checksum = us3 */
public static String BARCODETYPE_Code39USD3WOChecksum = "us3";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsBarcodeTypeValid (String test)
{
return test == null || test.Equals("28A") || test.Equals("28B") || test.Equals("28C") || test.Equals("2o9") || test.Equals("3O9") || test.Equals("3o9") || test.Equals("417") || test.Equals("C14") || test.Equals("C18") || test.Equals("C28") || test.Equals("C39") || test.Equals("COD") || test.Equals("E28") || test.Equals("GTN") || test.Equals("MON") || test.Equals("NW7") || test.Equals("SID") || test.Equals("U28") || test.Equals("US3") || test.Equals("US4") || test.Equals("USP") || test.Equals("c39") || test.Equals("us3");
}
/** Set Barcode Type.
@param BarcodeType Type of barcode */
public void SetBarcodeType (String BarcodeType)
{
if (!IsBarcodeTypeValid(BarcodeType))
throw new ArgumentException ("BarcodeType Invalid value - " + BarcodeType + " - Reference_ID=377 - 28A - 28B - 28C - 2o9 - 3O9 - 3o9 - 417 - C14 - C18 - C28 - C39 - COD - E28 - GTN - MON - NW7 - SID - U28 - US3 - US4 - USP - c39 - us3");
if (BarcodeType != null && BarcodeType.Length > 3)
{
log.Warning("Length > 3 - truncated");
BarcodeType = BarcodeType.Substring(0,3);
}
Set_Value ("BarcodeType", BarcodeType);
}
/** Get Barcode Type.
@return Type of barcode */
public String GetBarcodeType() 
{
return (String)Get_Value("BarcodeType");
}
/** Set Below Column.
@param BelowColumn Print this column below the column index entered */
public void SetBelowColumn (int BelowColumn)
{
Set_Value ("BelowColumn", BelowColumn);
}
/** Get Below Column.
@return Print this column below the column index entered */
public int GetBelowColumn() 
{
Object ii = Get_Value("BelowColumn");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** FieldAlignmentType AD_Reference_ID=253 */
public static int FIELDALIGNMENTTYPE_AD_Reference_ID=253;
/** Block = B */
public static String FIELDALIGNMENTTYPE_Block = "B";
/** Center = C */
public static String FIELDALIGNMENTTYPE_Center = "C";
/** Default = D */
public static String FIELDALIGNMENTTYPE_Default = "D";
/** Leading (left) = L */
public static String FIELDALIGNMENTTYPE_LeadingLeft = "L";
/** Trailing (right) = T */
public static String FIELDALIGNMENTTYPE_TrailingRight = "T";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsFieldAlignmentTypeValid (String test)
{
return test.Equals("B") || test.Equals("C") || test.Equals("D") || test.Equals("L") || test.Equals("T");
}
/** Set Field Alignment.
@param FieldAlignmentType Field Text Alignment */
public void SetFieldAlignmentType (String FieldAlignmentType)
{
if (FieldAlignmentType == null) throw new ArgumentException ("FieldAlignmentType is mandatory");
if (!IsFieldAlignmentTypeValid(FieldAlignmentType))
throw new ArgumentException ("FieldAlignmentType Invalid value - " + FieldAlignmentType + " - Reference_ID=253 - B - C - D - L - T");
if (FieldAlignmentType.Length > 1)
{
log.Warning("Length > 1 - truncated");
FieldAlignmentType = FieldAlignmentType.Substring(0,1);
}
Set_Value ("FieldAlignmentType", FieldAlignmentType);
}
/** Get Field Alignment.
@return Field Text Alignment */
public String GetFieldAlignmentType() 
{
return (String)Get_Value("FieldAlignmentType");
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
/** Set Calculate Mean (µ).
@param IsAveraged Calculate Average of numeric content or length */
public void SetIsAveraged (Boolean IsAveraged)
{
Set_Value ("IsAveraged", IsAveraged);
}
/** Get Calculate Mean (µ).
@return Calculate Average of numeric content or length */
public Boolean IsAveraged() 
{
Object oo = Get_Value("IsAveraged");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Centrally maintained.
@param IsCentrallyMaintained Information maintained in System Element table */
public void SetIsCentrallyMaintained (Boolean IsCentrallyMaintained)
{
Set_Value ("IsCentrallyMaintained", IsCentrallyMaintained);
}
/** Get Centrally maintained.
@return Information maintained in System Element table */
public Boolean IsCentrallyMaintained() 
{
Object oo = Get_Value("IsCentrallyMaintained");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Calculate Count (¿).
@param IsCounted Count number of not empty elements */
public void SetIsCounted (Boolean IsCounted)
{
Set_Value ("IsCounted", IsCounted);
}
/** Get Calculate Count (¿).
@return Count number of not empty elements */
public Boolean IsCounted() 
{
Object oo = Get_Value("IsCounted");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Calculate Deviation (s).
@param IsDeviationCalc Calculate Standard Deviation */
public void SetIsDeviationCalc (Boolean IsDeviationCalc)
{
Set_Value ("IsDeviationCalc", IsDeviationCalc);
}
/** Get Calculate Deviation (s).
@return Calculate Standard Deviation */
public Boolean IsDeviationCalc() 
{
Object oo = Get_Value("IsDeviationCalc");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Fill Shape.
@param IsFilledRectangle Fill the shape with the color selected */
public void SetIsFilledRectangle (Boolean IsFilledRectangle)
{
Set_Value ("IsFilledRectangle", IsFilledRectangle);
}
/** Get Fill Shape.
@return Fill the shape with the color selected */
public Boolean IsFilledRectangle() 
{
Object oo = Get_Value("IsFilledRectangle");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Fixed Width.
@param IsFixedWidth Column has a fixed width */
public void SetIsFixedWidth (Boolean IsFixedWidth)
{
Set_Value ("IsFixedWidth", IsFixedWidth);
}
/** Get Fixed Width.
@return Column has a fixed width */
public Boolean IsFixedWidth() 
{
Object oo = Get_Value("IsFixedWidth");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Group by.
@param IsGroupBy After a group change, totals, etc. are printed */
public void SetIsGroupBy (Boolean IsGroupBy)
{
Set_Value ("IsGroupBy", IsGroupBy);
}
/** Get Group by.
@return After a group change, totals, etc. are printed */
public Boolean IsGroupBy() 
{
Object oo = Get_Value("IsGroupBy");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set One Line Only.
@param IsHeightOneLine If selected, only one line is printed */
public void SetIsHeightOneLine (Boolean IsHeightOneLine)
{
Set_Value ("IsHeightOneLine", IsHeightOneLine);
}
/** Get One Line Only.
@return If selected, only one line is printed */
public Boolean IsHeightOneLine() 
{
Object oo = Get_Value("IsHeightOneLine");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Image Field.
@param IsImageField The image is retrieved from the data column */
public void SetIsImageField (Boolean IsImageField)
{
Set_Value ("IsImageField", IsImageField);
}
/** Get Image Field.
@return The image is retrieved from the data column */
public Boolean IsImageField() 
{
Object oo = Get_Value("IsImageField");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Calculate Maximum (¿).
@param IsMaxCalc Calculate the maximum amount */
public void SetIsMaxCalc (Boolean IsMaxCalc)
{
Set_Value ("IsMaxCalc", IsMaxCalc);
}
/** Get Calculate Maximum (¿).
@return Calculate the maximum amount */
public Boolean IsMaxCalc() 
{
Object oo = Get_Value("IsMaxCalc");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Calculate Minimum (¿).
@param IsMinCalc Calculate the minimum amount */
public void SetIsMinCalc (Boolean IsMinCalc)
{
Set_Value ("IsMinCalc", IsMinCalc);
}
/** Get Calculate Minimum (¿).
@return Calculate the minimum amount */
public Boolean IsMinCalc() 
{
Object oo = Get_Value("IsMinCalc");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Next Line.
@param IsNextLine Print item on next line */
public void SetIsNextLine (Boolean IsNextLine)
{
Set_Value ("IsNextLine", IsNextLine);
}
/** Get Next Line.
@return Print item on next line */
public Boolean IsNextLine() 
{
Object oo = Get_Value("IsNextLine");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Next Page.
@param IsNextPage The column is printed on the next page */
public void SetIsNextPage (Boolean IsNextPage)
{
Set_Value ("IsNextPage", IsNextPage);
}
/** Get Next Page.
@return The column is printed on the next page */
public Boolean IsNextPage() 
{
Object oo = Get_Value("IsNextPage");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Order by.
@param IsOrderBy Include in sort order */
public void SetIsOrderBy (Boolean IsOrderBy)
{
Set_Value ("IsOrderBy", IsOrderBy);
}
/** Get Order by.
@return Include in sort order */
public Boolean IsOrderBy() 
{
Object oo = Get_Value("IsOrderBy");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Page break.
@param IsPageBreak Start with new page */
public void SetIsPageBreak (Boolean IsPageBreak)
{
Set_Value ("IsPageBreak", IsPageBreak);
}
/** Get Page break.
@return Start with new page */
public Boolean IsPageBreak() 
{
Object oo = Get_Value("IsPageBreak");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Printed.
@param IsPrinted Indicates if this document / line is printed */
public void SetIsPrinted (Boolean IsPrinted)
{
Set_Value ("IsPrinted", IsPrinted);
}
/** Get Printed.
@return Indicates if this document / line is printed */
public Boolean IsPrinted() 
{
Object oo = Get_Value("IsPrinted");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Relative Position.
@param IsRelativePosition The item is relative positioned (not absolute) */
public void SetIsRelativePosition (Boolean IsRelativePosition)
{
Set_Value ("IsRelativePosition", IsRelativePosition);
}
/** Get Relative Position.
@return The item is relative positioned (not absolute) */
public Boolean IsRelativePosition() 
{
Object oo = Get_Value("IsRelativePosition");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Running Total.
@param IsRunningTotal Create a running total (sum) */
public void SetIsRunningTotal (Boolean IsRunningTotal)
{
Set_Value ("IsRunningTotal", IsRunningTotal);
}
/** Get Running Total.
@return Create a running total (sum) */
public Boolean IsRunningTotal() 
{
Object oo = Get_Value("IsRunningTotal");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Set NL Position.
@param IsSetNLPosition Set New Line Position */
public void SetIsSetNLPosition (Boolean IsSetNLPosition)
{
Set_Value ("IsSetNLPosition", IsSetNLPosition);
}
/** Get Set NL Position.
@return Set New Line Position */
public Boolean IsSetNLPosition() 
{
Object oo = Get_Value("IsSetNLPosition");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Calculate Sum (S).
@param IsSummarized Calculate the Sum of numeric content or length */
public void SetIsSummarized (Boolean IsSummarized)
{
Set_Value ("IsSummarized", IsSummarized);
}
/** Get Calculate Sum (S).
@return Calculate the Sum of numeric content or length */
public Boolean IsSummarized() 
{
Object oo = Get_Value("IsSummarized");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Suppress Null.
@param IsSuppressNull Suppress columns or elements with NULL value */
public void SetIsSuppressNull (Boolean IsSuppressNull)
{
Set_Value ("IsSuppressNull", IsSuppressNull);
}
/** Get Suppress Null.
@return Suppress columns or elements with NULL value */
public Boolean IsSuppressNull() 
{
Object oo = Get_Value("IsSuppressNull");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Calculate Variance (s²).
@param IsVarianceCalc Calculate Variance */
public void SetIsVarianceCalc (Boolean IsVarianceCalc)
{
Set_Value ("IsVarianceCalc", IsVarianceCalc);
}
/** Get Calculate Variance (s²).
@return Calculate Variance */
public Boolean IsVarianceCalc() 
{
Object oo = Get_Value("IsVarianceCalc");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}

/** LineAlignmentType AD_Reference_ID=254 */
public static int LINEALIGNMENTTYPE_AD_Reference_ID=254;
/** Center = C */
public static String LINEALIGNMENTTYPE_Center = "C";
/** Leading (left) = L */
public static String LINEALIGNMENTTYPE_LeadingLeft = "L";
/** Trailing (right) = T */
public static String LINEALIGNMENTTYPE_TrailingRight = "T";
/** None = X */
public static String LINEALIGNMENTTYPE_None = "X";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsLineAlignmentTypeValid (String test)
{
return test.Equals("C") || test.Equals("L") || test.Equals("T") || test.Equals("X");
}
/** Set Line Alignment.
@param LineAlignmentType Line Alignment */
public void SetLineAlignmentType (String LineAlignmentType)
{
if (LineAlignmentType == null) throw new ArgumentException ("LineAlignmentType is mandatory");
if (!IsLineAlignmentTypeValid(LineAlignmentType))
throw new ArgumentException ("LineAlignmentType Invalid value - " + LineAlignmentType + " - Reference_ID=254 - C - L - T - X");
if (LineAlignmentType.Length > 1)
{
log.Warning("Length > 1 - truncated");
LineAlignmentType = LineAlignmentType.Substring(0,1);
}
Set_Value ("LineAlignmentType", LineAlignmentType);
}
/** Get Line Alignment.
@return Line Alignment */
public String GetLineAlignmentType() 
{
return (String)Get_Value("LineAlignmentType");
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
/** Set Max Height.
@param MaxHeight Maximum Height in 1/72 if an inch - 0 = no restriction */
public void SetMaxHeight (int MaxHeight)
{
Set_Value ("MaxHeight", MaxHeight);
}
/** Get Max Height.
@return Maximum Height in 1/72 if an inch - 0 = no restriction */
public int GetMaxHeight() 
{
Object ii = Get_Value("MaxHeight");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Max Width.
@param MaxWidth Maximum Width in 1/72 if an inch - 0 = no restriction */
public void SetMaxWidth (int MaxWidth)
{
Set_Value ("MaxWidth", MaxWidth);
}
/** Get Max Width.
@return Maximum Width in 1/72 if an inch - 0 = no restriction */
public int GetMaxWidth() 
{
Object ii = Get_Value("MaxWidth");
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

/** PrintAreaType AD_Reference_ID=256 */
public static int PRINTAREATYPE_AD_Reference_ID=256;
/** Content = C */
public static String PRINTAREATYPE_Content = "C";
/** Footer = F */
public static String PRINTAREATYPE_Footer = "F";
/** Header = H */
public static String PRINTAREATYPE_Header = "H";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPrintAreaTypeValid (String test)
{
return test.Equals("C") || test.Equals("F") || test.Equals("H");
}
/** Set Area.
@param PrintAreaType Print Area */
public void SetPrintAreaType (String PrintAreaType)
{
if (PrintAreaType == null) throw new ArgumentException ("PrintAreaType is mandatory");
if (!IsPrintAreaTypeValid(PrintAreaType))
throw new ArgumentException ("PrintAreaType Invalid value - " + PrintAreaType + " - Reference_ID=256 - C - F - H");
if (PrintAreaType.Length > 1)
{
log.Warning("Length > 1 - truncated");
PrintAreaType = PrintAreaType.Substring(0,1);
}
Set_Value ("PrintAreaType", PrintAreaType);
}
/** Get Area.
@return Print Area */
public String GetPrintAreaType() 
{
return (String)Get_Value("PrintAreaType");
}

/** PrintFormatType AD_Reference_ID=255 */
public static int PRINTFORMATTYPE_AD_Reference_ID=255;
/** Field = F */
public static String PRINTFORMATTYPE_Field = "F";
/** Image = I */
public static String PRINTFORMATTYPE_Image = "I";
/** Line = L */
public static String PRINTFORMATTYPE_Line = "L";
/** Print Format = P */
public static String PRINTFORMATTYPE_PrintFormat = "P";
/** Rectangle = R */
public static String PRINTFORMATTYPE_Rectangle = "R";
/** Text = T */
public static String PRINTFORMATTYPE_Text = "T";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPrintFormatTypeValid (String test)
{
return test.Equals("F") || test.Equals("I") || test.Equals("L") || test.Equals("P") || test.Equals("R") || test.Equals("T");
}
/** Set Format Type.
@param PrintFormatType Print Format Type */
public void SetPrintFormatType (String PrintFormatType)
{
if (PrintFormatType == null) throw new ArgumentException ("PrintFormatType is mandatory");
if (!IsPrintFormatTypeValid(PrintFormatType))
throw new ArgumentException ("PrintFormatType Invalid value - " + PrintFormatType + " - Reference_ID=255 - F - I - L - P - R - T");
if (PrintFormatType.Length > 1)
{
log.Warning("Length > 1 - truncated");
PrintFormatType = PrintFormatType.Substring(0,1);
}
Set_Value ("PrintFormatType", PrintFormatType);
}
/** Get Format Type.
@return Print Format Type */
public String GetPrintFormatType() 
{
return (String)Get_Value("PrintFormatType");
}
/** Set Print Text.
@param PrintName The label text to be printed on a document or correspondence. */
public void SetPrintName (String PrintName)
{
if (PrintName != null && PrintName.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
PrintName = PrintName.Substring(0,2000);
}
Set_Value ("PrintName", PrintName);
}
/** Get Print Text.
@return The label text to be printed on a document or correspondence. */
public String GetPrintName() 
{
return (String)Get_Value("PrintName");
}
/** Set Print Label Suffix.
@param PrintNameSuffix The label text to be printed on a document or correspondence after the field */
public void SetPrintNameSuffix (String PrintNameSuffix)
{
if (PrintNameSuffix != null && PrintNameSuffix.Length > 60)
{
log.Warning("Length > 60 - truncated");
PrintNameSuffix = PrintNameSuffix.Substring(0,60);
}
Set_Value ("PrintNameSuffix", PrintNameSuffix);
}
/** Get Print Label Suffix.
@return The label text to be printed on a document or correspondence after the field */
public String GetPrintNameSuffix() 
{
return (String)Get_Value("PrintNameSuffix");
}
/** Set Running Total Lines.
@param RunningTotalLines Create Running Total Lines (page break) every x lines */
public void SetRunningTotalLines (int RunningTotalLines)
{
Set_Value ("RunningTotalLines", RunningTotalLines);
}
/** Get Running Total Lines.
@return Create Running Total Lines (page break) every x lines */
public int GetRunningTotalLines() 
{
Object ii = Get_Value("RunningTotalLines");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** ShapeType AD_Reference_ID=333 */
public static int SHAPETYPE_AD_Reference_ID=333;
/** 3D Rectangle = 3 */
public static String SHAPETYPE_3DRectangle = "3";
/** Normal Rectangle = N */
public static String SHAPETYPE_NormalRectangle = "N";
/** Oval = O */
public static String SHAPETYPE_Oval = "O";
/** Round Rectangle = R */
public static String SHAPETYPE_RoundRectangle = "R";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsShapeTypeValid (String test)
{
return test == null || test.Equals("3") || test.Equals("N") || test.Equals("O") || test.Equals("R");
}
/** Set Shape Type.
@param ShapeType Type of the shape to be painted */
public void SetShapeType (String ShapeType)
{
if (!IsShapeTypeValid(ShapeType))
throw new ArgumentException ("ShapeType Invalid value - " + ShapeType + " - Reference_ID=333 - 3 - N - O - R");
if (ShapeType != null && ShapeType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ShapeType = ShapeType.Substring(0,1);
}
Set_Value ("ShapeType", ShapeType);
}
/** Get Shape Type.
@return Type of the shape to be painted */
public String GetShapeType() 
{
return (String)Get_Value("ShapeType");
}
/** Set Record Sort No.
@param SortNo Determines in what order the records are displayed */
public void SetSortNo (int SortNo)
{
Set_Value ("SortNo", SortNo);
}
/** Get Record Sort No.
@return Determines in what order the records are displayed */
public int GetSortNo() 
{
Object ii = Get_Value("SortNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set X Position.
@param XPosition Absolute X (horizontal) position in 1/72 of an inch */
public void SetXPosition (int XPosition)
{
Set_Value ("XPosition", XPosition);
}
/** Get X Position.
@return Absolute X (horizontal) position in 1/72 of an inch */
public int GetXPosition() 
{
Object ii = Get_Value("XPosition");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set X Space.
@param XSpace Relative X (horizontal) space in 1/72 of an inch */
public void SetXSpace (int XSpace)
{
Set_Value ("XSpace", XSpace);
}
/** Get X Space.
@return Relative X (horizontal) space in 1/72 of an inch */
public int GetXSpace() 
{
Object ii = Get_Value("XSpace");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Y Position.
@param YPosition Absolute Y (vertical) position in 1/72 of an inch */
public void SetYPosition (int YPosition)
{
Set_Value ("YPosition", YPosition);
}
/** Get Y Position.
@return Absolute Y (vertical) position in 1/72 of an inch */
public int GetYPosition() 
{
Object ii = Get_Value("YPosition");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Y Space.
@param YSpace Relative Y (vertical) space in 1/72 of an inch */
public void SetYSpace (int YSpace)
{
Set_Value ("YSpace", YSpace);
}
/** Get Y Space.
@return Relative Y (vertical) space in 1/72 of an inch */
public int GetYSpace() 
{
Object ii = Get_Value("YSpace");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}


/** Set Org Logo.
@param IsOrgLogo Org Logo */
public void SetIsOrgLogo(Boolean IsOrgLogo)
{
    Set_Value("IsOrgLogo", IsOrgLogo);
}
/** Get Org Logo.
@return Org Logo */
public Boolean IsOrgLogo()
{
    Object oo = Get_Value("IsOrgLogo");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
}

}

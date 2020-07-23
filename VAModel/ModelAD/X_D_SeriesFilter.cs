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
/** Generated Model for D_SeriesFilter
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_D_SeriesFilter : PO
{
public X_D_SeriesFilter (Context ctx, int D_SeriesFilter_ID, Trx trxName) : base (ctx, D_SeriesFilter_ID, trxName)
{
/** if (D_SeriesFilter_ID == 0)
{
SetD_SeriesFilter_ID (0);
}
 */
}
public X_D_SeriesFilter (Ctx ctx, int D_SeriesFilter_ID, Trx trxName) : base (ctx, D_SeriesFilter_ID, trxName)
{
/** if (D_SeriesFilter_ID == 0)
{
SetD_SeriesFilter_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_D_SeriesFilter (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_D_SeriesFilter (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_D_SeriesFilter (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_D_SeriesFilter()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376105L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059316L;
/** AD_Table_ID=1000008 */
public static int Table_ID;
 // =1000008;

/** TableName=D_SeriesFilter */
public static String Table_Name="D_SeriesFilter";

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
StringBuilder sb = new StringBuilder ("X_D_SeriesFilter[").Append(Get_ID()).Append("]");
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

/** AD_Table_ID AD_Reference_ID=415 */
public static int AD_TABLE_ID_AD_Reference_ID=415;
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID <= 0) Set_Value ("AD_Table_ID", null);
else
Set_Value ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set D_SeriesFilter_ID.
@param D_SeriesFilter_ID D_SeriesFilter_ID */
public void SetD_SeriesFilter_ID (int D_SeriesFilter_ID)
{
if (D_SeriesFilter_ID < 1) throw new ArgumentException ("D_SeriesFilter_ID is mandatory.");
Set_ValueNoCheck ("D_SeriesFilter_ID", D_SeriesFilter_ID);
}
/** Get D_SeriesFilter_ID.
@return D_SeriesFilter_ID */
public int GetD_SeriesFilter_ID() 
{
Object ii = Get_Value("D_SeriesFilter_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Series Name.
@param D_Series_ID Series Name */
public void SetD_Series_ID (int D_Series_ID)
{
if (D_Series_ID <= 0) Set_Value ("D_Series_ID", null);
else
Set_Value ("D_Series_ID", D_Series_ID);
}
/** Get Series Name.
@return Series Name */
public int GetD_Series_ID() 
{
Object ii = Get_Value("D_Series_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 50)
{
log.Warning("Length > 50 - truncated");
Name = Name.Substring(0,50);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set ValueTo.
@param ValueTo ValueTo */
public void SetValueTo (String ValueTo)
{
if (ValueTo != null && ValueTo.Length > 50)
{
log.Warning("Length > 50 - truncated");
ValueTo = ValueTo.Substring(0,50);
}
Set_Value ("ValueTo", ValueTo);
}
/** Get ValueTo.
@return ValueTo */
public String GetValueTo() 
{
return (String)Get_Value("ValueTo");
}

/** WhereCondition AD_Reference_ID=1000004 */
public static int WHERECONDITION_AD_Reference_ID=1000004;
/** = = 1 */
public static String WHERECONDITION_Eq = "1";
/** != = 2 */
public static String WHERECONDITION_NotEq = "2";
/** >= = 3 */
public static String WHERECONDITION_GtEq = "3";
/** <= = 4 */
public static String WHERECONDITION_LeEq = "4";
/** >< = 5 */
public static String WHERECONDITION_ = "5";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsWhereConditionValid (String test)
{
return test == null || test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("4") || test.Equals("5");
}
/** Set Where Condition.
@param WhereCondition Where Condition */
public void SetWhereCondition (String WhereCondition)
{
if (!IsWhereConditionValid(WhereCondition))
throw new ArgumentException ("WhereCondition Invalid value - " + WhereCondition + " - Reference_ID=1000004 - 1 - 2 - 3 - 4 - 5");
if (WhereCondition != null && WhereCondition.Length > 1)
{
log.Warning("Length > 1 - truncated");
WhereCondition = WhereCondition.Substring(0,1);
}
Set_Value ("WhereCondition", WhereCondition);
}
/** Get Where Condition.
@return Where Condition */
public String GetWhereCondition() 
{
return (String)Get_Value("WhereCondition");
}
/** Set WhereValue.
@param WhereValue WhereValue */
public void SetWhereValue (String WhereValue)
{
if (WhereValue != null && WhereValue.Length > 50)
{
log.Warning("Length > 50 - truncated");
WhereValue = WhereValue.Substring(0,50);
}
Set_Value ("WhereValue", WhereValue);
}
/** Get WhereValue.
@return WhereValue */
public String GetWhereValue() 
{
return (String)Get_Value("WhereValue");
}
}

}

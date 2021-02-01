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
/** Generated Model for VAF_Record_Rights
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_Record_Rights : PO
{
public X_VAF_Record_Rights (Context ctx, int VAF_Record_Rights_ID, Trx trxName) : base (ctx, VAF_Record_Rights_ID, trxName)
{
/** if (VAF_Record_Rights_ID == 0)
{
SetVAF_Role_ID (0);
SetVAF_TableView_ID (0);
SetIsDependentEntities (false);	// N
SetIsExclude (true);	// Y
SetIsReadOnly (false);
SetRecord_ID (0);
}
 */
}
public X_VAF_Record_Rights (Ctx ctx, int VAF_Record_Rights_ID, Trx trxName) : base (ctx, VAF_Record_Rights_ID, trxName)
{
/** if (VAF_Record_Rights_ID == 0)
{
SetVAF_Role_ID (0);
SetVAF_TableView_ID (0);
SetIsDependentEntities (false);	// N
SetIsExclude (true);	// Y
SetIsReadOnly (false);
SetRecord_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Record_Rights (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Record_Rights (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Record_Rights (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_Record_Rights()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363301L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046512L;
/** VAF_TableView_ID=567 */
public static int Table_ID;
 // =567;

/** TableName=VAF_Record_Rights */
public static String Table_Name="VAF_Record_Rights";

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
StringBuilder sb = new StringBuilder ("X_VAF_Record_Rights[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Role.
@param VAF_Role_ID Responsibility Role */
public void SetVAF_Role_ID (int VAF_Role_ID)
{
if (VAF_Role_ID < 0) throw new ArgumentException ("VAF_Role_ID is mandatory.");
Set_ValueNoCheck ("VAF_Role_ID", VAF_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetVAF_Role_ID() 
{
Object ii = Get_Value("VAF_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_Role_ID().ToString());
}
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID)
{
if (VAF_TableView_ID < 1) throw new ArgumentException ("VAF_TableView_ID is mandatory.");
Set_ValueNoCheck ("VAF_TableView_ID", VAF_TableView_ID);
}
/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() 
{
Object ii = Get_Value("VAF_TableView_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Dependent Entities.
@param IsDependentEntities Also check access in dependent entities */
public void SetIsDependentEntities (Boolean IsDependentEntities)
{
Set_Value ("IsDependentEntities", IsDependentEntities);
}
/** Get Dependent Entities.
@return Also check access in dependent entities */
public Boolean IsDependentEntities() 
{
Object oo = Get_Value("IsDependentEntities");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Exclude.
@param IsExclude Exclude access to the data - if not selected Include access to the data */
public void SetIsExclude (Boolean IsExclude)
{
Set_Value ("IsExclude", IsExclude);
}
/** Get Exclude.
@return Exclude access to the data - if not selected Include access to the data */
public Boolean IsExclude() 
{
Object oo = Get_Value("IsExclude");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Read Only.
@param IsReadOnly Field is read only */
public void SetIsReadOnly (Boolean IsReadOnly)
{
Set_Value ("IsReadOnly", IsReadOnly);
}
/** Get Read Only.
@return Field is read only */
public Boolean IsReadOnly() 
{
Object oo = Get_Value("IsReadOnly");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID < 0) throw new ArgumentException ("Record_ID is mandatory.");
Set_ValueNoCheck ("Record_ID", Record_ID);
}
/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() 
{
Object ii = Get_Value("Record_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}

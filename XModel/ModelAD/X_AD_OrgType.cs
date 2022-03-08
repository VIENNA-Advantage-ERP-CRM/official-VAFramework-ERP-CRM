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
/** Generated Model for AD_OrgType
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_OrgType : PO
{
public X_AD_OrgType (Context ctx, int AD_OrgType_ID, Trx trxName) : base (ctx, AD_OrgType_ID, trxName)
{
/** if (AD_OrgType_ID == 0)
{
SetAD_OrgType_ID (0);
SetIsBalancing (false);	// N
SetIsLegalEntity (false);	// N
SetName (null);
}
 */
}
public X_AD_OrgType (Ctx ctx, int AD_OrgType_ID, Trx trxName) : base (ctx, AD_OrgType_ID, trxName)
{
/** if (AD_OrgType_ID == 0)
{
SetAD_OrgType_ID (0);
SetIsBalancing (false);	// N
SetIsLegalEntity (false);	// N
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_OrgType (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_OrgType (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_OrgType (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_OrgType()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362376L;
/** Last Updated Timestamp 7/29/2010 1:07:25 PM */
public static long updatedMS = 1280389045587L;
/** AD_Table_ID=689 */
public static int Table_ID;
 // =689;

/** TableName=AD_OrgType */
public static String Table_Name="AD_OrgType";

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
StringBuilder sb = new StringBuilder ("X_AD_OrgType[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Organization Type.
@param AD_OrgType_ID Organization Type allows you to categorize your organizations */
public void SetAD_OrgType_ID (int AD_OrgType_ID)
{
if (AD_OrgType_ID < 1) throw new ArgumentException ("AD_OrgType_ID is mandatory.");
Set_ValueNoCheck ("AD_OrgType_ID", AD_OrgType_ID);
}
/** Get Organization Type.
@return Organization Type allows you to categorize your organizations */
public int GetAD_OrgType_ID() 
{
Object ii = Get_Value("AD_OrgType_ID");
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
/** Set Balancing.
@param IsBalancing All transactions within an element value must balance (e.g. legal entities) */
public void SetIsBalancing (Boolean IsBalancing)
{
Set_Value ("IsBalancing", IsBalancing);
}
/** Get Balancing.
@return All transactions within an element value must balance (e.g. legal entities) */
public Boolean IsBalancing() 
{
Object oo = Get_Value("IsBalancing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Legal Entity.
@param IsLegalEntity The organizations are legal entities */
public void SetIsLegalEntity (Boolean IsLegalEntity)
{
Set_Value ("IsLegalEntity", IsLegalEntity);
}
/** Get Legal Entity.
@return The organizations are legal entities */
public Boolean IsLegalEntity() 
{
Object oo = Get_Value("IsLegalEntity");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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

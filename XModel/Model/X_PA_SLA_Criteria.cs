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
/** Generated Model for VAPA_SLA_Creteria
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAPA_SLA_Creteria : PO
{
public X_VAPA_SLA_Creteria (Context ctx, int VAPA_SLA_Creteria_ID, Trx trxName) : base (ctx, VAPA_SLA_Creteria_ID, trxName)
{
/** if (VAPA_SLA_Creteria_ID == 0)
{
SetIsManual (true);	// Y
SetName (null);
SetVAPA_SLA_Creteria_ID (0);
}
 */
}
public X_VAPA_SLA_Creteria (Ctx ctx, int VAPA_SLA_Creteria_ID, Trx trxName) : base (ctx, VAPA_SLA_Creteria_ID, trxName)
{
/** if (VAPA_SLA_Creteria_ID == 0)
{
SetIsManual (true);	// Y
SetName (null);
SetVAPA_SLA_Creteria_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_SLA_Creteria (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_SLA_Creteria (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_SLA_Creteria (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAPA_SLA_Creteria()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382296L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065507L;
/** VAF_TableView_ID=744 */
public static int Table_ID;
 // =744;

/** TableName=VAPA_SLA_Creteria */
public static String Table_Name="VAPA_SLA_Creteria";

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
StringBuilder sb = new StringBuilder ("X_VAPA_SLA_Creteria[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Classname.
@param Classname Java Classname */
public void SetClassname (String Classname)
{
if (Classname != null && Classname.Length > 60)
{
log.Warning("Length > 60 - truncated");
Classname = Classname.Substring(0,60);
}
Set_Value ("Classname", Classname);
}
/** Get Classname.
@return Java Classname */
public String GetClassname() 
{
return (String)Get_Value("Classname");
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
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}
/** Set Manual.
@param IsManual This is a manual process */
public void SetIsManual (Boolean IsManual)
{
Set_Value ("IsManual", IsManual);
}
/** Get Manual.
@return This is a manual process */
public Boolean IsManual() 
{
Object oo = Get_Value("IsManual");
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
/** Set SLA Criteria.
@param VAPA_SLA_Creteria_ID Service Level Agreement Criteria */
public void SetVAPA_SLA_Creteria_ID (int VAPA_SLA_Creteria_ID)
{
if (VAPA_SLA_Creteria_ID < 1) throw new ArgumentException ("VAPA_SLA_Creteria_ID is mandatory.");
Set_ValueNoCheck ("VAPA_SLA_Creteria_ID", VAPA_SLA_Creteria_ID);
}
/** Get SLA Criteria.
@return Service Level Agreement Criteria */
public int GetVAPA_SLA_Creteria_ID() 
{
Object ii = Get_Value("VAPA_SLA_Creteria_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}

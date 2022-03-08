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
/** Generated Model for C_JobRemuneration
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_JobRemuneration : PO
{
public X_C_JobRemuneration (Context ctx, int C_JobRemuneration_ID, Trx trxName) : base (ctx, C_JobRemuneration_ID, trxName)
{
/** if (C_JobRemuneration_ID == 0)
{
SetC_JobRemuneration_ID (0);
SetC_Job_ID (0);
SetC_Remuneration_ID (0);
SetValidFrom (DateTime.Now);
}
 */
}
public X_C_JobRemuneration (Ctx ctx, int C_JobRemuneration_ID, Trx trxName) : base (ctx, C_JobRemuneration_ID, trxName)
{
/** if (C_JobRemuneration_ID == 0)
{
SetC_JobRemuneration_ID (0);
SetC_Job_ID (0);
SetC_Remuneration_ID (0);
SetValidFrom (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_JobRemuneration (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_JobRemuneration (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_JobRemuneration (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_JobRemuneration()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372767L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055978L;
/** AD_Table_ID=793 */
public static int Table_ID;
 // =793;

/** TableName=C_JobRemuneration */
public static String Table_Name="C_JobRemuneration";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_C_JobRemuneration[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Position Remuneration.
@param C_JobRemuneration_ID Remuneration for the Position */
public void SetC_JobRemuneration_ID (int C_JobRemuneration_ID)
{
if (C_JobRemuneration_ID < 1) throw new ArgumentException ("C_JobRemuneration_ID is mandatory.");
Set_ValueNoCheck ("C_JobRemuneration_ID", C_JobRemuneration_ID);
}
/** Get Position Remuneration.
@return Remuneration for the Position */
public int GetC_JobRemuneration_ID() 
{
Object ii = Get_Value("C_JobRemuneration_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Position.
@param C_Job_ID Job Position */
public void SetC_Job_ID (int C_Job_ID)
{
if (C_Job_ID < 1) throw new ArgumentException ("C_Job_ID is mandatory.");
Set_ValueNoCheck ("C_Job_ID", C_Job_ID);
}
/** Get Position.
@return Job Position */
public int GetC_Job_ID() 
{
Object ii = Get_Value("C_Job_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_Job_ID().ToString());
}
/** Set Remuneration.
@param C_Remuneration_ID Wage or Salary */
public void SetC_Remuneration_ID (int C_Remuneration_ID)
{
if (C_Remuneration_ID < 1) throw new ArgumentException ("C_Remuneration_ID is mandatory.");
Set_ValueNoCheck ("C_Remuneration_ID", C_Remuneration_ID);
}
/** Get Remuneration.
@return Wage or Salary */
public int GetC_Remuneration_ID() 
{
Object ii = Get_Value("C_Remuneration_ID");
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
/** Set Valid from.
@param ValidFrom Valid from including this date (first day) */
public void SetValidFrom (DateTime? ValidFrom)
{
if (ValidFrom == null) throw new ArgumentException ("ValidFrom is mandatory.");
Set_Value ("ValidFrom", (DateTime?)ValidFrom);
}
/** Get Valid from.
@return Valid from including this date (first day) */
public DateTime? GetValidFrom() 
{
return (DateTime?)Get_Value("ValidFrom");
}
/** Set Valid to.
@param ValidTo Valid to including this date (last day) */
public void SetValidTo (DateTime? ValidTo)
{
Set_Value ("ValidTo", (DateTime?)ValidTo);
}
/** Get Valid to.
@return Valid to including this date (last day) */
public DateTime? GetValidTo() 
{
return (DateTime?)Get_Value("ValidTo");
}
}

}

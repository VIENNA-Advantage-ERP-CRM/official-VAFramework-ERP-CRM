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
/** Generated Model for C_DunningRun
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_DunningRun : PO
{
public X_C_DunningRun (Context ctx, int C_DunningRun_ID, Trx trxName) : base (ctx, C_DunningRun_ID, trxName)
{
/** if (C_DunningRun_ID == 0)
{
SetC_DunningLevel_ID (0);
SetC_DunningRun_ID (0);
SetDunningDate (DateTime.Now);	// @#Date@
SetProcessed (false);	// N
}
 */
}
public X_C_DunningRun (Ctx ctx, int C_DunningRun_ID, Trx trxName) : base (ctx, C_DunningRun_ID, trxName)
{
/** if (C_DunningRun_ID == 0)
{
SetC_DunningLevel_ID (0);
SetC_DunningRun_ID (0);
SetDunningDate (DateTime.Now);	// @#Date@
SetProcessed (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_DunningRun (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_DunningRun (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_DunningRun (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_DunningRun()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372015L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055226L;
/** AD_Table_ID=526 */
public static int Table_ID;
 // =526;

/** TableName=C_DunningRun */
public static String Table_Name="C_DunningRun";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_C_DunningRun[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Dunning Level.
@param C_DunningLevel_ID Dunning Level */
public void SetC_DunningLevel_ID (int C_DunningLevel_ID)
{
if (C_DunningLevel_ID < 1) throw new ArgumentException ("C_DunningLevel_ID is mandatory.");
Set_ValueNoCheck ("C_DunningLevel_ID", C_DunningLevel_ID);
}
/** Get Dunning Level.
@return Dunning Level */
public int GetC_DunningLevel_ID() 
{
Object ii = Get_Value("C_DunningLevel_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Dunning Run.
@param C_DunningRun_ID Dunning Run */
public void SetC_DunningRun_ID (int C_DunningRun_ID)
{
if (C_DunningRun_ID < 1) throw new ArgumentException ("C_DunningRun_ID is mandatory.");
Set_ValueNoCheck ("C_DunningRun_ID", C_DunningRun_ID);
}
/** Get Dunning Run.
@return Dunning Run */
public int GetC_DunningRun_ID() 
{
Object ii = Get_Value("C_DunningRun_ID");
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
/** Set Dunning Date.
@param DunningDate Date of Dunning */
public void SetDunningDate (DateTime? DunningDate)
{
if (DunningDate == null) throw new ArgumentException ("DunningDate is mandatory.");
Set_Value ("DunningDate", (DateTime?)DunningDate);
}
/** Get Dunning Date.
@return Date of Dunning */
public DateTime? GetDunningDate() 
{
return (DateTime?)Get_Value("DunningDate");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetDunningDate().ToString());
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
}
/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() 
{
Object oo = Get_Value("Processed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Send.
@param SendIt Send */
public void SetSendIt (String SendIt)
{
if (SendIt != null && SendIt.Length > 1)
{
log.Warning("Length > 1 - truncated");
SendIt = SendIt.Substring(0,1);
}
Set_Value ("SendIt", SendIt);
}
/** Get Send.
@return Send */
public String GetSendIt() 
{
return (String)Get_Value("SendIt");
}
}

}

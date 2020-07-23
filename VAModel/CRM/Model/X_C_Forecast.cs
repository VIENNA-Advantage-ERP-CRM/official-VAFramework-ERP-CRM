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
/** Generated Model for C_Forecast
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Forecast : PO
{
public X_C_Forecast (Context ctx, int C_Forecast_ID, Trx trxName) : base (ctx, C_Forecast_ID, trxName)
{
/** if (C_Forecast_ID == 0)
{
SetC_Forecast_ID (0);
}
 */
}
public X_C_Forecast (Ctx ctx, int C_Forecast_ID, Trx trxName) : base (ctx, C_Forecast_ID, trxName)
{
/** if (C_Forecast_ID == 0)
{
SetC_Forecast_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Forecast (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Forecast (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Forecast (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Forecast()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27609451555927L;
/** Last Updated Timestamp 1/23/2012 7:13:59 PM */
public static long updatedMS = 1327326239138L;
/** AD_Table_ID=1000244 */
public static int Table_ID;
 // =1000244;

/** TableName=C_Forecast */
public static String Table_Name="C_Forecast";

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
StringBuilder sb = new StringBuilder ("X_C_Forecast[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Forecast.
@param C_Forecast_ID Forecast */
public void SetC_Forecast_ID (int C_Forecast_ID)
{
if (C_Forecast_ID < 1) throw new ArgumentException ("C_Forecast_ID is mandatory.");
Set_ValueNoCheck ("C_Forecast_ID", C_Forecast_ID);
}
/** Get Forecast.
@return Forecast */
public int GetC_Forecast_ID() 
{
Object ii = Get_Value("C_Forecast_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Period.
@param C_Period_ID Period of the Calendar */
public void SetC_Period_ID (int C_Period_ID)
{
if (C_Period_ID <= 0) Set_Value ("C_Period_ID", null);
else
Set_Value ("C_Period_ID", C_Period_ID);
}
/** Get Period.
@return Period of the Calendar */
public int GetC_Period_ID() 
{
Object ii = Get_Value("C_Period_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Comments.
@param Comments Comments or additional information */
public void SetComments (String Comments)
{
if (Comments != null && Comments.Length > 50)
{
log.Warning("Length > 50 - truncated");
Comments = Comments.Substring(0,50);
}
Set_Value ("Comments", Comments);
}
/** Get Comments.
@return Comments or additional information */
public String GetComments() 
{
return (String)Get_Value("Comments");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
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
}

}

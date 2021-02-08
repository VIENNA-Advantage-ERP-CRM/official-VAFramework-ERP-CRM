namespace VAdvantage.Model
{

/** Generated Model - DO NOT CHANGE */
using System;
using System.Text;
using VAdvantage.DataBase;
//using VAdvantage.Common;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
/** Generated Model for VAB_MasterForecast
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_MasterForecast : PO
{
public X_VAB_MasterForecast (Context ctx, int VAB_MasterForecast_ID, Trx trxName) : base (ctx, VAB_MasterForecast_ID, trxName)
{
/** if (VAB_MasterForecast_ID == 0)
{
SetVAB_MasterForecast_ID (0);
}
 */
}
public X_VAB_MasterForecast (Ctx ctx, int VAB_MasterForecast_ID, Trx trxName) : base (ctx, VAB_MasterForecast_ID, trxName)
{
/** if (VAB_MasterForecast_ID == 0)
{
SetVAB_MasterForecast_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_MasterForecast (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_MasterForecast (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_MasterForecast (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_MasterForecast()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27609451521051L;
/** Last Updated Timestamp 1/23/2012 7:13:24 PM */
public static long updatedMS = 1327326204262L;
/** VAF_TableView_ID=1000246 */
public static int Table_ID;
 // =1000246;

/** TableName=VAB_MasterForecast */
public static String Table_Name="VAB_MasterForecast";

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
StringBuilder sb = new StringBuilder ("X_VAB_MasterForecast[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Forecast.
@param VAB_MasterForecast_ID Forecast */
public void SetVAB_MasterForecast_ID (int VAB_MasterForecast_ID)
{
if (VAB_MasterForecast_ID < 1) throw new ArgumentException ("VAB_MasterForecast_ID is mandatory.");
Set_ValueNoCheck ("VAB_MasterForecast_ID", VAB_MasterForecast_ID);
}
/** Get Forecast.
@return Forecast */
public int GetVAB_MasterForecast_ID() 
{
Object ii = Get_Value("VAB_MasterForecast_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Period.
@param VAB_YearPeriod_ID Period of the Calendar */
public void SetVAB_YearPeriod_ID (int VAB_YearPeriod_ID)
{
if (VAB_YearPeriod_ID <= 0) Set_Value ("VAB_YearPeriod_ID", null);
else
Set_Value ("VAB_YearPeriod_ID", VAB_YearPeriod_ID);
}
/** Get Period.
@return Period of the Calendar */
public int GetVAB_YearPeriod_ID() 
{
Object ii = Get_Value("VAB_YearPeriod_ID");
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
/** Set Include Oppurtunity.
@param IsIncludeOpp Include Oppurtunity */
public void SetIsIncludeOpp (Boolean IsIncludeOpp)
{
Set_Value ("IsIncludeOpp", IsIncludeOpp);
}
/** Get Include Oppurtunity.
@return Include Oppurtunity */
public Boolean IsIncludeOpp() 
{
Object oo = Get_Value("IsIncludeOpp");
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
/** Set Create Version.
@param createversion Create Version */
public void Setcreateversion (String createversion)
{
if (createversion != null && createversion.Length > 1)
{
log.Warning("Length > 1 - truncated");
createversion = createversion.Substring(0,1);
}
Set_Value ("createversion", createversion);
}
/** Get Create Version.
@return Create Version */
public String Getcreateversion() 
{
return (String)Get_Value("createversion");
}
/** Set Current Version.
@param currentversion Current Version */
public void SetCurrentVersion(Boolean CurrentVersion)
{
    Set_Value("CurrentVersion", CurrentVersion);
}
/** Get Current Version.
@return Current Version */
public Boolean IsCurrentVersion() 
{
    Object oo = Get_Value("CurrentVersion");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}

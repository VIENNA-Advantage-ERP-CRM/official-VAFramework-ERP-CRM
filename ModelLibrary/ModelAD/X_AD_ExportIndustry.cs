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
/** Generated Model for AD_ExportIndustry
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_ExportIndustry : PO
{
public X_AD_ExportIndustry (Context ctx, int AD_ExportIndustry_ID, Trx trxName) : base (ctx, AD_ExportIndustry_ID, trxName)
{
/** if (AD_ExportIndustry_ID == 0)
{
SetAD_ExportIndustry_ID (0);
}
 */
}
public X_AD_ExportIndustry (Ctx ctx, int AD_ExportIndustry_ID, Trx trxName) : base (ctx, AD_ExportIndustry_ID, trxName)
{
/** if (AD_ExportIndustry_ID == 0)
{
SetAD_ExportIndustry_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ExportIndustry (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ExportIndustry (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ExportIndustry (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ExportIndustry()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27564341066445L;
/** Last Updated Timestamp 8/19/2010 4:32:30 PM */
public static long updatedMS = 1282215749656L;
/** AD_Table_ID=1000166 */
public static int Table_ID;
 // =1000166;

/** TableName=AD_ExportIndustry */
public static String Table_Name="AD_ExportIndustry";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_AD_ExportIndustry[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set AD_ExportIndustry_ID.
@param AD_ExportIndustry_ID AD_ExportIndustry_ID */
public void SetAD_ExportIndustry_ID (int AD_ExportIndustry_ID)
{
if (AD_ExportIndustry_ID < 1) throw new ArgumentException ("AD_ExportIndustry_ID is mandatory.");
Set_ValueNoCheck ("AD_ExportIndustry_ID", AD_ExportIndustry_ID);
}
/** Get AD_ExportIndustry_ID.
@return AD_ExportIndustry_ID */
public int GetAD_ExportIndustry_ID() 
{
Object ii = Get_Value("AD_ExportIndustry_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Language ID.
@param AD_Language_ID Language ID */
public void SetAD_Language_ID (int AD_Language_ID)
{
if (AD_Language_ID <= 0) Set_Value ("AD_Language_ID", null);
else
Set_Value ("AD_Language_ID", AD_Language_ID);
}
/** Get Language ID.
@return Language ID */
public int GetAD_Language_ID() 
{
Object ii = Get_Value("AD_Language_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set DBName.
@param DBName DBName */
public void SetDBName (String DBName)
{
if (DBName != null && DBName.Length > 50)
{
log.Warning("Length > 50 - truncated");
DBName = DBName.Substring(0,50);
}
Set_Value ("DBName", DBName);
}
/** Get DBName.
@return DBName */
public String GetDBName() 
{
return (String)Get_Value("DBName");
}

/** DatabaseType AD_Reference_ID=1000073 */
public static int DATABASETYPE_AD_Reference_ID=1000073;
/** Oracle = 1 */
public static String DATABASETYPE_Oracle = "1";
/** Postgre SQL = 2 */
public static String DATABASETYPE_PostgreSQL = "2";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDatabaseTypeValid (String test)
{
return test == null || test.Equals("1") || test.Equals("2");
}
/** Set DatabaseType.
@param DatabaseType DatabaseType */
public void SetDatabaseType (String DatabaseType)
{
if (!IsDatabaseTypeValid(DatabaseType))
throw new ArgumentException ("DatabaseType Invalid value - " + DatabaseType + " - Reference_ID=1000073 - 1 - 2");
if (DatabaseType != null && DatabaseType.Length > 1)
{
log.Warning("Length > 1 - truncated");
DatabaseType = DatabaseType.Substring(0,1);
}
Set_Value ("DatabaseType", DatabaseType);
}
/** Get DatabaseType.
@return DatabaseType */
public String GetDatabaseType() 
{
return (String)Get_Value("DatabaseType");
}
/** Set Host Address.
@param HostAddress Host Address URL or DNS */
public void SetHostAddress (String HostAddress)
{
if (HostAddress != null && HostAddress.Length > 50)
{
log.Warning("Length > 50 - truncated");
HostAddress = HostAddress.Substring(0,50);
}
Set_Value ("HostAddress", HostAddress);
}
/** Get Host Address.
@return Host Address URL or DNS */
public String GetHostAddress() 
{
return (String)Get_Value("HostAddress");
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
/** Set Password.
@param Password Password of any length (case sensitive) */
public void SetPassword (String Password)
{
if (Password != null && Password.Length > 50)
{
log.Warning("Length > 50 - truncated");
Password = Password.Substring(0,50);
}
Set_Value ("Password", Password);
}
/** Get Password.
@return Password of any length (case sensitive) */
public String GetPassword() 
{
return (String)Get_Value("Password");
}
/** Set User ID.
@param UserID User ID or account number */
public void SetUserID (String UserID)
{
if (UserID != null && UserID.Length > 50)
{
log.Warning("Length > 50 - truncated");
UserID = UserID.Substring(0,50);
}
Set_Value ("UserID", UserID);
}
/** Get User ID.
@return User ID or account number */
public String GetUserID() 
{
return (String)Get_Value("UserID");
}
}

}

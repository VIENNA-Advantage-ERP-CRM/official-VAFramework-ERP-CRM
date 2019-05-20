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
/** Generated Model for CM_WebProject_Domain
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_WebProject_Domain : PO
{
public X_CM_WebProject_Domain (Context ctx, int CM_WebProject_Domain_ID, Trx trxName) : base (ctx, CM_WebProject_Domain_ID, trxName)
{
/** if (CM_WebProject_Domain_ID == 0)
{
SetCM_WebProject_Domain_ID (0);
SetCM_WebProject_ID (0);
SetFQDN (null);
SetName (null);
}
 */
}
public X_CM_WebProject_Domain (Ctx ctx, int CM_WebProject_Domain_ID, Trx trxName) : base (ctx, CM_WebProject_Domain_ID, trxName)
{
/** if (CM_WebProject_Domain_ID == 0)
{
SetCM_WebProject_Domain_ID (0);
SetCM_WebProject_ID (0);
SetFQDN (null);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_WebProject_Domain (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_WebProject_Domain (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_WebProject_Domain (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_WebProject_Domain()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369303L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052514L;
/** AD_Table_ID=873 */
public static int Table_ID;
 // =873;

/** TableName=CM_WebProject_Domain */
public static String Table_Name="CM_WebProject_Domain";

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
StringBuilder sb = new StringBuilder ("X_CM_WebProject_Domain[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Web Container.
@param CM_Container_ID Web Container contains content like images, text etc. */
public void SetCM_Container_ID (int CM_Container_ID)
{
if (CM_Container_ID <= 0) Set_Value ("CM_Container_ID", null);
else
Set_Value ("CM_Container_ID", CM_Container_ID);
}
/** Get Web Container.
@return Web Container contains content like images, text etc. */
public int GetCM_Container_ID() 
{
Object ii = Get_Value("CM_Container_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set WebProject Domain.
@param CM_WebProject_Domain_ID Definition of Domainhandling */
public void SetCM_WebProject_Domain_ID (int CM_WebProject_Domain_ID)
{
if (CM_WebProject_Domain_ID < 1) throw new ArgumentException ("CM_WebProject_Domain_ID is mandatory.");
Set_ValueNoCheck ("CM_WebProject_Domain_ID", CM_WebProject_Domain_ID);
}
/** Get WebProject Domain.
@return Definition of Domainhandling */
public int GetCM_WebProject_Domain_ID() 
{
Object ii = Get_Value("CM_WebProject_Domain_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Web Project.
@param CM_WebProject_ID A web project is the main data container for Containers, URLs, Ads, Media etc. */
public void SetCM_WebProject_ID (int CM_WebProject_ID)
{
if (CM_WebProject_ID < 1) throw new ArgumentException ("CM_WebProject_ID is mandatory.");
Set_ValueNoCheck ("CM_WebProject_ID", CM_WebProject_ID);
}
/** Get Web Project.
@return A web project is the main data container for Containers, URLs, Ads, Media etc. */
public int GetCM_WebProject_ID() 
{
Object ii = Get_Value("CM_WebProject_ID");
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
/** Set Fully Qualified Domain Name.
@param FQDN Fully Qualified Domain Name e.g.  www.ViennaAdvantage.com */
public void SetFQDN (String FQDN)
{
if (FQDN == null) throw new ArgumentException ("FQDN is mandatory.");
if (FQDN.Length > 120)
{
log.Warning("Length > 120 - truncated");
FQDN = FQDN.Substring(0,120);
}
Set_Value ("FQDN", FQDN);
}
/** Get Fully Qualified Domain Name.
@return Fully Qualified Domain Name e.g.  www.ViennaAdvantage.com */
public String GetFQDN() 
{
return (String)Get_Value("FQDN");
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

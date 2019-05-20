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
/** Generated Model for K_Source
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_K_Source : PO
{
public X_K_Source (Context ctx, int K_Source_ID, Trx trxName) : base (ctx, K_Source_ID, trxName)
{
/** if (K_Source_ID == 0)
{
SetK_Source_ID (0);
SetName (null);
}
 */
}
public X_K_Source (Ctx ctx, int K_Source_ID, Trx trxName) : base (ctx, K_Source_ID, trxName)
{
/** if (K_Source_ID == 0)
{
SetK_Source_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_Source (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_Source (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_K_Source (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_K_Source()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514378096L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061307L;
/** AD_Table_ID=609 */
public static int Table_ID;
 // =609;

/** TableName=K_Source */
public static String Table_Name="K_Source";

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
StringBuilder sb = new StringBuilder ("X_K_Source[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Description URL.
@param DescriptionURL URL for the description */
public void SetDescriptionURL (String DescriptionURL)
{
if (DescriptionURL != null && DescriptionURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
DescriptionURL = DescriptionURL.Substring(0,120);
}
Set_Value ("DescriptionURL", DescriptionURL);
}
/** Get Description URL.
@return URL for the description */
public String GetDescriptionURL() 
{
return (String)Get_Value("DescriptionURL");
}
/** Set Knowledge Source.
@param K_Source_ID Source of a Knowledge Entry */
public void SetK_Source_ID (int K_Source_ID)
{
if (K_Source_ID < 1) throw new ArgumentException ("K_Source_ID is mandatory.");
Set_ValueNoCheck ("K_Source_ID", K_Source_ID);
}
/** Get Knowledge Source.
@return Source of a Knowledge Entry */
public int GetK_Source_ID() 
{
Object ii = Get_Value("K_Source_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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

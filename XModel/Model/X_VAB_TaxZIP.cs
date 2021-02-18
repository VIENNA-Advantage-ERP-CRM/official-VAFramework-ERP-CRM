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
/** Generated Model for VAB_TaxZIP
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_TaxZIP : PO
{
public X_VAB_TaxZIP (Context ctx, int VAB_TaxZIP_ID, Trx trxName) : base (ctx, VAB_TaxZIP_ID, trxName)
{
/** if (VAB_TaxZIP_ID == 0)
{
SetVAB_TaxZIP_ID (0);
SetVAB_TaxRate_ID (0);
SetPostal (null);
}
 */
}
public X_VAB_TaxZIP (Ctx ctx, int VAB_TaxZIP_ID, Trx trxName) : base (ctx, VAB_TaxZIP_ID, trxName)
{
/** if (VAB_TaxZIP_ID == 0)
{
SetVAB_TaxZIP_ID (0);
SetVAB_TaxRate_ID (0);
SetPostal (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_TaxZIP (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_TaxZIP (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_TaxZIP (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_TaxZIP()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375525L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058736L;
/** VAF_TableView_ID=701 */
public static int Table_ID;
 // =701;

/** TableName=VAB_TaxZIP */
public static String Table_Name="VAB_TaxZIP";

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
StringBuilder sb = new StringBuilder ("X_VAB_TaxZIP[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Tax ZIP.
@param VAB_TaxZIP_ID Tax Postal/ZIP */
public void SetVAB_TaxZIP_ID (int VAB_TaxZIP_ID)
{
if (VAB_TaxZIP_ID < 1) throw new ArgumentException ("VAB_TaxZIP_ID is mandatory.");
Set_ValueNoCheck ("VAB_TaxZIP_ID", VAB_TaxZIP_ID);
}
/** Get Tax ZIP.
@return Tax Postal/ZIP */
public int GetVAB_TaxZIP_ID() 
{
Object ii = Get_Value("VAB_TaxZIP_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax.
@param VAB_TaxRate_ID Tax identifier */
public void SetVAB_TaxRate_ID (int VAB_TaxRate_ID)
{
if (VAB_TaxRate_ID < 1) throw new ArgumentException ("VAB_TaxRate_ID is mandatory.");
Set_ValueNoCheck ("VAB_TaxRate_ID", VAB_TaxRate_ID);
}
/** Get Tax.
@return Tax identifier */
public int GetVAB_TaxRate_ID() 
{
Object ii = Get_Value("VAB_TaxRate_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set ZIP.
@param Postal Postal code */
public void SetPostal (String Postal)
{
if (Postal == null) throw new ArgumentException ("Postal is mandatory.");
if (Postal.Length > 10)
{
log.Warning("Length > 10 - truncated");
Postal = Postal.Substring(0,10);
}
Set_Value ("Postal", Postal);
}
/** Get ZIP.
@return Postal code */
public String GetPostal() 
{
return (String)Get_Value("Postal");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetPostal());
}
/** Set ZIP To.
@param Postal_To Postal code to */
public void SetPostal_To (String Postal_To)
{
if (Postal_To != null && Postal_To.Length > 10)
{
log.Warning("Length > 10 - truncated");
Postal_To = Postal_To.Substring(0,10);
}
Set_Value ("Postal_To", Postal_To);
}
/** Get ZIP To.
@return Postal code to */
public String GetPostal_To() 
{
return (String)Get_Value("Postal_To");
}
}

}

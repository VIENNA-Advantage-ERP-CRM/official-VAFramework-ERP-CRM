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
/** Generated Model for C_TaxPostal
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_TaxPostal : PO
{
public X_C_TaxPostal (Context ctx, int C_TaxPostal_ID, Trx trxName) : base (ctx, C_TaxPostal_ID, trxName)
{
/** if (C_TaxPostal_ID == 0)
{
SetC_TaxPostal_ID (0);
SetC_Tax_ID (0);
SetPostal (null);
}
 */
}
public X_C_TaxPostal (Ctx ctx, int C_TaxPostal_ID, Trx trxName) : base (ctx, C_TaxPostal_ID, trxName)
{
/** if (C_TaxPostal_ID == 0)
{
SetC_TaxPostal_ID (0);
SetC_Tax_ID (0);
SetPostal (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TaxPostal (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TaxPostal (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TaxPostal (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_TaxPostal()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375525L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058736L;
/** AD_Table_ID=701 */
public static int Table_ID;
 // =701;

/** TableName=C_TaxPostal */
public static String Table_Name="C_TaxPostal";

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
StringBuilder sb = new StringBuilder ("X_C_TaxPostal[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Tax ZIP.
@param C_TaxPostal_ID Tax Postal/ZIP */
public void SetC_TaxPostal_ID (int C_TaxPostal_ID)
{
if (C_TaxPostal_ID < 1) throw new ArgumentException ("C_TaxPostal_ID is mandatory.");
Set_ValueNoCheck ("C_TaxPostal_ID", C_TaxPostal_ID);
}
/** Get Tax ZIP.
@return Tax Postal/ZIP */
public int GetC_TaxPostal_ID() 
{
Object ii = Get_Value("C_TaxPostal_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax.
@param C_Tax_ID Tax identifier */
public void SetC_Tax_ID (int C_Tax_ID)
{
if (C_Tax_ID < 1) throw new ArgumentException ("C_Tax_ID is mandatory.");
Set_ValueNoCheck ("C_Tax_ID", C_Tax_ID);
}
/** Get Tax.
@return Tax identifier */
public int GetC_Tax_ID() 
{
Object ii = Get_Value("C_Tax_ID");
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

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
/** Generated Model for C_BP_Withholding
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_BP_Withholding : PO
{
public X_C_BP_Withholding (Context ctx, int C_BP_Withholding_ID, Trx trxName) : base (ctx, C_BP_Withholding_ID, trxName)
{
/** if (C_BP_Withholding_ID == 0)
{
SetC_BPartner_ID (0);
SetC_Withholding_ID (0);
SetIsMandatoryWithholding (false);
SetIsTemporaryExempt (false);
}
 */
}
public X_C_BP_Withholding (Ctx ctx, int C_BP_Withholding_ID, Trx trxName) : base (ctx, C_BP_Withholding_ID, trxName)
{
/** if (C_BP_Withholding_ID == 0)
{
SetC_BPartner_ID (0);
SetC_Withholding_ID (0);
SetIsMandatoryWithholding (false);
SetIsTemporaryExempt (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_Withholding (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_Withholding (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_Withholding (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_BP_Withholding()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514370401L;
/** Last Updated Timestamp 7/29/2010 1:07:33 PM */
public static long updatedMS = 1280389053612L;
/** AD_Table_ID=299 */
public static int Table_ID;
 // =299;

/** TableName=C_BP_Withholding */
public static String Table_Name="C_BP_Withholding";

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
StringBuilder sb = new StringBuilder ("X_C_BP_Withholding[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID < 1) throw new ArgumentException ("C_BPartner_ID is mandatory.");
Set_ValueNoCheck ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_BPartner_ID().ToString());
}
/** Set Withholding.
@param C_Withholding_ID Withholding type defined */
public void SetC_Withholding_ID (int C_Withholding_ID)
{
if (C_Withholding_ID < 1) throw new ArgumentException ("C_Withholding_ID is mandatory.");
Set_ValueNoCheck ("C_Withholding_ID", C_Withholding_ID);
}
/** Get Withholding.
@return Withholding type defined */
public int GetC_Withholding_ID() 
{
Object ii = Get_Value("C_Withholding_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Exempt reason.
@param ExemptReason Reason for not withholding */
public void SetExemptReason (String ExemptReason)
{
if (ExemptReason != null && ExemptReason.Length > 20)
{
log.Warning("Length > 20 - truncated");
ExemptReason = ExemptReason.Substring(0,20);
}
Set_Value ("ExemptReason", ExemptReason);
}
/** Get Exempt reason.
@return Reason for not withholding */
public String GetExemptReason() 
{
return (String)Get_Value("ExemptReason");
}
/** Set Mandatory Withholding.
@param IsMandatoryWithholding Monies must be withheld */
public void SetIsMandatoryWithholding (Boolean IsMandatoryWithholding)
{
Set_Value ("IsMandatoryWithholding", IsMandatoryWithholding);
}
/** Get Mandatory Withholding.
@return Monies must be withheld */
public Boolean IsMandatoryWithholding() 
{
Object oo = Get_Value("IsMandatoryWithholding");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Temporary exempt.
@param IsTemporaryExempt Temporarily do not withhold taxes */
public void SetIsTemporaryExempt (Boolean IsTemporaryExempt)
{
Set_Value ("IsTemporaryExempt", IsTemporaryExempt);
}
/** Get Temporary exempt.
@return Temporarily do not withhold taxes */
public Boolean IsTemporaryExempt() 
{
Object oo = Get_Value("IsTemporaryExempt");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}

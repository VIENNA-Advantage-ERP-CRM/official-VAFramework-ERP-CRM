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
/** Generated Model for C_GenAttributeSet
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_GenAttributeSet : PO
{
public X_C_GenAttributeSet (Context ctx, int C_GenAttributeSet_ID, Trx trxName) : base (ctx, C_GenAttributeSet_ID, trxName)
{
/** if (C_GenAttributeSet_ID == 0)
{
SetC_GenAttributeSet_ID (0);
SetIsGuaranteeDate (false);
SetIsGuaranteeDateMandatory (false);
SetIsInstanceAttribute (false);
SetIsLot (false);
SetIsLotMandatory (false);
SetIsSerNo (false);
SetIsSerNoMandatory (false);
SetMandatoryType (null);
SetName (null);
}
 */
}
public X_C_GenAttributeSet (Ctx ctx, int C_GenAttributeSet_ID, Trx trxName) : base (ctx, C_GenAttributeSet_ID, trxName)
{
/** if (C_GenAttributeSet_ID == 0)
{
SetC_GenAttributeSet_ID (0);
SetIsGuaranteeDate (false);
SetIsGuaranteeDateMandatory (false);
SetIsInstanceAttribute (false);
SetIsLot (false);
SetIsLotMandatory (false);
SetIsSerNo (false);
SetIsSerNoMandatory (false);
SetMandatoryType (null);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeSet (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeSet (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeSet (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_GenAttributeSet()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27667169099943L;
/** Last Updated Timestamp 11/21/2013 7:53:03 PM */
public static long updatedMS = 1385043783154L;
/** AD_Table_ID=1000421 */
public static int Table_ID;
 // =1000421;

/** TableName=C_GenAttributeSet */
public static String Table_Name="C_GenAttributeSet";

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
StringBuilder sb = new StringBuilder ("X_C_GenAttributeSet[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set C_GenAttributeSet_ID.
@param C_GenAttributeSet_ID C_GenAttributeSet_ID */
public void SetC_GenAttributeSet_ID (int C_GenAttributeSet_ID)
{
if (C_GenAttributeSet_ID < 1) throw new ArgumentException ("C_GenAttributeSet_ID is mandatory.");
Set_ValueNoCheck ("C_GenAttributeSet_ID", C_GenAttributeSet_ID);
}
/** Get C_GenAttributeSet_ID.
@return C_GenAttributeSet_ID */
public int GetC_GenAttributeSet_ID() 
{
Object ii = Get_Value("C_GenAttributeSet_ID");
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
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_Value ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
/** Set Guarantee Days.
@param GuaranteeDays Number of days the product is guaranteed or available */
public void SetGuaranteeDays (int GuaranteeDays)
{
Set_Value ("GuaranteeDays", GuaranteeDays);
}
/** Get Guarantee Days.
@return Number of days the product is guaranteed or available */
public int GetGuaranteeDays() 
{
Object ii = Get_Value("GuaranteeDays");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Guarantee Date.
@param IsGuaranteeDate Product has Guarantee or Expiry Date */
public void SetIsGuaranteeDate (Boolean IsGuaranteeDate)
{
Set_Value ("IsGuaranteeDate", IsGuaranteeDate);
}
/** Get Guarantee Date.
@return Product has Guarantee or Expiry Date */
public Boolean IsGuaranteeDate() 
{
Object oo = Get_Value("IsGuaranteeDate");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Mandatory Guarantee Date.
@param IsGuaranteeDateMandatory The entry of a Guarantee Date is mandatory when creating a Product Instance */
public void SetIsGuaranteeDateMandatory (Boolean IsGuaranteeDateMandatory)
{
Set_Value ("IsGuaranteeDateMandatory", IsGuaranteeDateMandatory);
}
/** Get Mandatory Guarantee Date.
@return The entry of a Guarantee Date is mandatory when creating a Product Instance */
public Boolean IsGuaranteeDateMandatory() 
{
Object oo = Get_Value("IsGuaranteeDateMandatory");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Instance Attribute.
@param IsInstanceAttribute The product attribute is specific to the instance (like Serial No, Lot or Guarantee Date) */
public void SetIsInstanceAttribute (Boolean IsInstanceAttribute)
{
Set_Value ("IsInstanceAttribute", IsInstanceAttribute);
}
/** Get Instance Attribute.
@return The product attribute is specific to the instance (like Serial No, Lot or Guarantee Date) */
public Boolean IsInstanceAttribute() 
{
Object oo = Get_Value("IsInstanceAttribute");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Lot.
@param IsLot The product instances have a Lot Number */
public void SetIsLot (Boolean IsLot)
{
Set_Value ("IsLot", IsLot);
}
/** Get Lot.
@return The product instances have a Lot Number */
public Boolean IsLot() 
{
Object oo = Get_Value("IsLot");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Mandatory Lot.
@param IsLotMandatory The entry of Lot info is mandatory when creating a Product Instance */
public void SetIsLotMandatory (Boolean IsLotMandatory)
{
Set_Value ("IsLotMandatory", IsLotMandatory);
}
/** Get Mandatory Lot.
@return The entry of Lot info is mandatory when creating a Product Instance */
public Boolean IsLotMandatory() 
{
Object oo = Get_Value("IsLotMandatory");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Serial No.
@param IsSerNo The product instances have Serial Numbers */
public void SetIsSerNo (Boolean IsSerNo)
{
Set_Value ("IsSerNo", IsSerNo);
}
/** Get Serial No.
@return The product instances have Serial Numbers */
public Boolean IsSerNo() 
{
Object oo = Get_Value("IsSerNo");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Mandatory Serial No.
@param IsSerNoMandatory The entry of a Serial No is mandatory when creating a Product Instance */
public void SetIsSerNoMandatory (Boolean IsSerNoMandatory)
{
Set_Value ("IsSerNoMandatory", IsSerNoMandatory);
}
/** Get Mandatory Serial No.
@return The entry of a Serial No is mandatory when creating a Product Instance */
public Boolean IsSerNoMandatory() 
{
Object oo = Get_Value("IsSerNoMandatory");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Lot Char End Overwrite.
@param LotCharEOverwrite Lot/Batch End Indicator overwrite - default » */
public void SetLotCharEOverwrite (String LotCharEOverwrite)
{
if (LotCharEOverwrite != null && LotCharEOverwrite.Length > 1)
{
log.Warning("Length > 1 - truncated");
LotCharEOverwrite = LotCharEOverwrite.Substring(0,1);
}
Set_Value ("LotCharEOverwrite", LotCharEOverwrite);
}
/** Get Lot Char End Overwrite.
@return Lot/Batch End Indicator overwrite - default » */
public String GetLotCharEOverwrite() 
{
return (String)Get_Value("LotCharEOverwrite");
}
/** Set Lot Char Start Overwrite.
@param LotCharSOverwrite Lot/Batch Start Indicator overwrite - default « */
public void SetLotCharSOverwrite (String LotCharSOverwrite)
{
if (LotCharSOverwrite != null && LotCharSOverwrite.Length > 1)
{
log.Warning("Length > 1 - truncated");
LotCharSOverwrite = LotCharSOverwrite.Substring(0,1);
}
Set_Value ("LotCharSOverwrite", LotCharSOverwrite);
}
/** Get Lot Char Start Overwrite.
@return Lot/Batch Start Indicator overwrite - default « */
public String GetLotCharSOverwrite() 
{
return (String)Get_Value("LotCharSOverwrite");
}
/** Set Lot Control.
@param M_LotCtl_ID Product Lot Control */
public void SetM_LotCtl_ID (int M_LotCtl_ID)
{
if (M_LotCtl_ID <= 0) Set_Value ("M_LotCtl_ID", null);
else
Set_Value ("M_LotCtl_ID", M_LotCtl_ID);
}
/** Get Lot Control.
@return Product Lot Control */
public int GetM_LotCtl_ID() 
{
Object ii = Get_Value("M_LotCtl_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Serial No Control.
@param M_SerNoCtl_ID Product Serial Number Control */
public void SetM_SerNoCtl_ID (int M_SerNoCtl_ID)
{
if (M_SerNoCtl_ID <= 0) Set_Value ("M_SerNoCtl_ID", null);
else
Set_Value ("M_SerNoCtl_ID", M_SerNoCtl_ID);
}
/** Get Serial No Control.
@return Product Serial Number Control */
public int GetM_SerNoCtl_ID() 
{
Object ii = Get_Value("M_SerNoCtl_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** MandatoryType AD_Reference_ID=324 */
public static int MANDATORYTYPE_AD_Reference_ID=324;
/** Not Mandatary = N */
public static String MANDATORYTYPE_NotMandatary = "N";
/** When Shipping = S */
public static String MANDATORYTYPE_WhenShipping = "S";
/** Always Mandatory = Y */
public static String MANDATORYTYPE_AlwaysMandatory = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsMandatoryTypeValid (String test)
{
return test.Equals("N") || test.Equals("S") || test.Equals("Y");
}
/** Set Mandatory Type.
@param MandatoryType The specification of a Attribute is mandatory */
public void SetMandatoryType (String MandatoryType)
{
if (MandatoryType == null) throw new ArgumentException ("MandatoryType is mandatory");
if (!IsMandatoryTypeValid(MandatoryType))
throw new ArgumentException ("MandatoryType Invalid value - " + MandatoryType + " - Reference_ID=324 - N - S - Y");
if (MandatoryType.Length > 1)
{
log.Warning("Length > 1 - truncated");
MandatoryType = MandatoryType.Substring(0,1);
}
Set_Value ("MandatoryType", MandatoryType);
}
/** Get Mandatory Type.
@return The specification of a Attribute is mandatory */
public String GetMandatoryType() 
{
return (String)Get_Value("MandatoryType");
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
/** Set SerNo Char End Overwrite.
@param SerNoCharEOverwrite Serial Number End Indicator overwrite - default empty */
public void SetSerNoCharEOverwrite (String SerNoCharEOverwrite)
{
if (SerNoCharEOverwrite != null && SerNoCharEOverwrite.Length > 1)
{
log.Warning("Length > 1 - truncated");
SerNoCharEOverwrite = SerNoCharEOverwrite.Substring(0,1);
}
Set_Value ("SerNoCharEOverwrite", SerNoCharEOverwrite);
}
/** Get SerNo Char End Overwrite.
@return Serial Number End Indicator overwrite - default empty */
public String GetSerNoCharEOverwrite() 
{
return (String)Get_Value("SerNoCharEOverwrite");
}
/** Set SerNo Char Start Overwrite.
@param SerNoCharSOverwrite Serial Number Start Indicator overwrite - default # */
public void SetSerNoCharSOverwrite (String SerNoCharSOverwrite)
{
if (SerNoCharSOverwrite != null && SerNoCharSOverwrite.Length > 1)
{
log.Warning("Length > 1 - truncated");
SerNoCharSOverwrite = SerNoCharSOverwrite.Substring(0,1);
}
Set_Value ("SerNoCharSOverwrite", SerNoCharSOverwrite);
}
/** Get SerNo Char Start Overwrite.
@return Serial Number Start Indicator overwrite - default # */
public String GetSerNoCharSOverwrite() 
{
return (String)Get_Value("SerNoCharSOverwrite");
}
}

}

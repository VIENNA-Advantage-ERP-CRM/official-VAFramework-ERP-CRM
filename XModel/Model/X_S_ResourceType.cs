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
/** Generated Model for S_ResourceType
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_S_ResourceType : PO
{
public X_S_ResourceType (Context ctx, int S_ResourceType_ID, Trx trxName) : base (ctx, S_ResourceType_ID, trxName)
{
/** if (S_ResourceType_ID == 0)
{
SetAllowUoMFractions (false);	// N
SetC_TaxCategory_ID (0);
SetC_UOM_ID (0);
SetIsDateSlot (false);
SetIsSingleAssignment (false);
SetIsTimeSlot (false);
SetM_Product_Category_ID (0);
SetName (null);
SetOnFriday (true);	// Y
SetOnMonday (true);	// Y
SetOnSaturday (false);
SetOnSunday (false);
SetOnThursday (true);	// Y
SetOnTuesday (true);	// Y
SetOnWednesday (true);	// Y
SetS_ResourceType_ID (0);
SetValue (null);
}
 */
}
public X_S_ResourceType (Ctx ctx, int S_ResourceType_ID, Trx trxName) : base (ctx, S_ResourceType_ID, trxName)
{
/** if (S_ResourceType_ID == 0)
{
SetAllowUoMFractions (false);	// N
SetC_TaxCategory_ID (0);
SetC_UOM_ID (0);
SetIsDateSlot (false);
SetIsSingleAssignment (false);
SetIsTimeSlot (false);
SetM_Product_Category_ID (0);
SetName (null);
SetOnFriday (true);	// Y
SetOnMonday (true);	// Y
SetOnSaturday (false);
SetOnSunday (false);
SetOnThursday (true);	// Y
SetOnTuesday (true);	// Y
SetOnWednesday (true);	// Y
SetS_ResourceType_ID (0);
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_S_ResourceType (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_S_ResourceType (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_S_ResourceType (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_S_ResourceType()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383769L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066980L;
/** AD_Table_ID=480 */
public static int Table_ID;
 // =480;

/** TableName=S_ResourceType */
public static String Table_Name="S_ResourceType";

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
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_S_ResourceType[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Allow UoM Fractions.
@param AllowUoMFractions Allow Unit of Measure Fractions */
public void SetAllowUoMFractions (Boolean AllowUoMFractions)
{
Set_Value ("AllowUoMFractions", AllowUoMFractions);
}
/** Get Allow UoM Fractions.
@return Allow Unit of Measure Fractions */
public Boolean IsAllowUoMFractions() 
{
Object oo = Get_Value("AllowUoMFractions");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Tax Category.
@param C_TaxCategory_ID Tax Category */
public void SetC_TaxCategory_ID (int C_TaxCategory_ID)
{
if (C_TaxCategory_ID < 1) throw new ArgumentException ("C_TaxCategory_ID is mandatory.");
Set_Value ("C_TaxCategory_ID", C_TaxCategory_ID);
}
/** Get Tax Category.
@return Tax Category */
public int GetC_TaxCategory_ID() 
{
Object ii = Get_Value("C_TaxCategory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set UOM.
@param C_UOM_ID Unit of Measure */
public void SetC_UOM_ID (int C_UOM_ID)
{
if (C_UOM_ID < 1) throw new ArgumentException ("C_UOM_ID is mandatory.");
Set_Value ("C_UOM_ID", C_UOM_ID);
}
/** Get UOM.
@return Unit of Measure */
public int GetC_UOM_ID() 
{
Object ii = Get_Value("C_UOM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Chargeable Quantity.
@param ChargeableQty Chargeable Quantity */
public void SetChargeableQty (int ChargeableQty)
{
Set_Value ("ChargeableQty", ChargeableQty);
}
/** Get Chargeable Quantity.
@return Chargeable Quantity */
public int GetChargeableQty() 
{
Object ii = Get_Value("ChargeableQty");
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
/** Set Day Slot.
@param IsDateSlot Resource has day slot availability */
public void SetIsDateSlot (Boolean IsDateSlot)
{
Set_Value ("IsDateSlot", IsDateSlot);
}
/** Get Day Slot.
@return Resource has day slot availability */
public Boolean IsDateSlot() 
{
Object oo = Get_Value("IsDateSlot");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Single Assignment only.
@param IsSingleAssignment Only one assignment at a time (no double-booking or overlapping) */
public void SetIsSingleAssignment (Boolean IsSingleAssignment)
{
Set_Value ("IsSingleAssignment", IsSingleAssignment);
}
/** Get Single Assignment only.
@return Only one assignment at a time (no double-booking or overlapping) */
public Boolean IsSingleAssignment() 
{
Object oo = Get_Value("IsSingleAssignment");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Time Slot.
@param IsTimeSlot Resource has time slot availability */
public void SetIsTimeSlot (Boolean IsTimeSlot)
{
Set_Value ("IsTimeSlot", IsTimeSlot);
}
/** Get Time Slot.
@return Resource has time slot availability */
public Boolean IsTimeSlot() 
{
Object oo = Get_Value("IsTimeSlot");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Product Category.
@param M_Product_Category_ID Category of a Product */
public void SetM_Product_Category_ID (int M_Product_Category_ID)
{
if (M_Product_Category_ID < 1) throw new ArgumentException ("M_Product_Category_ID is mandatory.");
Set_Value ("M_Product_Category_ID", M_Product_Category_ID);
}
/** Get Product Category.
@return Category of a Product */
public int GetM_Product_Category_ID() 
{
Object ii = Get_Value("M_Product_Category_ID");
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
/** Set Friday.
@param OnFriday Available on Fridays */
public void SetOnFriday (Boolean OnFriday)
{
Set_Value ("OnFriday", OnFriday);
}
/** Get Friday.
@return Available on Fridays */
public Boolean IsOnFriday() 
{
Object oo = Get_Value("OnFriday");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Monday.
@param OnMonday Available on Mondays */
public void SetOnMonday (Boolean OnMonday)
{
Set_Value ("OnMonday", OnMonday);
}
/** Get Monday.
@return Available on Mondays */
public Boolean IsOnMonday() 
{
Object oo = Get_Value("OnMonday");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Saturday.
@param OnSaturday Available on Saturday */
public void SetOnSaturday (Boolean OnSaturday)
{
Set_Value ("OnSaturday", OnSaturday);
}
/** Get Saturday.
@return Available on Saturday */
public Boolean IsOnSaturday() 
{
Object oo = Get_Value("OnSaturday");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Sunday.
@param OnSunday Available on Sundays */
public void SetOnSunday (Boolean OnSunday)
{
Set_Value ("OnSunday", OnSunday);
}
/** Get Sunday.
@return Available on Sundays */
public Boolean IsOnSunday() 
{
Object oo = Get_Value("OnSunday");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Thursday.
@param OnThursday Available on Thursdays */
public void SetOnThursday (Boolean OnThursday)
{
Set_Value ("OnThursday", OnThursday);
}
/** Get Thursday.
@return Available on Thursdays */
public Boolean IsOnThursday() 
{
Object oo = Get_Value("OnThursday");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Tuesday.
@param OnTuesday Available on Tuesdays */
public void SetOnTuesday (Boolean OnTuesday)
{
Set_Value ("OnTuesday", OnTuesday);
}
/** Get Tuesday.
@return Available on Tuesdays */
public Boolean IsOnTuesday() 
{
Object oo = Get_Value("OnTuesday");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Wednesday.
@param OnWednesday Available on Wednesdays */
public void SetOnWednesday (Boolean OnWednesday)
{
Set_Value ("OnWednesday", OnWednesday);
}
/** Get Wednesday.
@return Available on Wednesdays */
public Boolean IsOnWednesday() 
{
Object oo = Get_Value("OnWednesday");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Resource Type.
@param S_ResourceType_ID Resource Type */
public void SetS_ResourceType_ID (int S_ResourceType_ID)
{
if (S_ResourceType_ID < 1) throw new ArgumentException ("S_ResourceType_ID is mandatory.");
Set_ValueNoCheck ("S_ResourceType_ID", S_ResourceType_ID);
}
/** Get Resource Type.
@return Resource Type */
public int GetS_ResourceType_ID() 
{
Object ii = Get_Value("S_ResourceType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Slot End.
@param TimeSlotEnd Time when timeslot ends */
public void SetTimeSlotEnd (DateTime? TimeSlotEnd)
{
Set_Value ("TimeSlotEnd", (DateTime?)TimeSlotEnd);
}
/** Get Slot End.
@return Time when timeslot ends */
public DateTime? GetTimeSlotEnd() 
{
return (DateTime?)Get_Value("TimeSlotEnd");
}
/** Set Slot Start.
@param TimeSlotStart Time when timeslot starts */
public void SetTimeSlotStart (DateTime? TimeSlotStart)
{
Set_Value ("TimeSlotStart", (DateTime?)TimeSlotStart);
}
/** Get Slot Start.
@return Time when timeslot starts */
public DateTime? GetTimeSlotStart() 
{
return (DateTime?)Get_Value("TimeSlotStart");
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value == null) throw new ArgumentException ("Value is mandatory.");
if (Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value = Value.Substring(0,40);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
}

}

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
/** Generated Model for C_GenAttributeInstance
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_GenAttributeInstance : PO
{
public X_C_GenAttributeInstance (Context ctx, int C_GenAttributeInstance_ID, Trx trxName) : base (ctx, C_GenAttributeInstance_ID, trxName)
{
/** if (C_GenAttributeInstance_ID == 0)
{
SetC_GenAttributeInstance_ID (0);
SetC_GenAttribute_ID (0);
}
 */
}
public X_C_GenAttributeInstance (Ctx ctx, int C_GenAttributeInstance_ID, Trx trxName) : base (ctx, C_GenAttributeInstance_ID, trxName)
{
/** if (C_GenAttributeInstance_ID == 0)
{
SetC_GenAttributeInstance_ID (0);
SetC_GenAttribute_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeInstance (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeInstance (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeInstance (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_GenAttributeInstance()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27667169129038L;
/** Last Updated Timestamp 11/21/2013 7:53:32 PM */
public static long updatedMS = 1385043812249L;
/** AD_Table_ID=1000419 */
public static int Table_ID;
 // =1000419;

/** TableName=C_GenAttributeInstance */
public static String Table_Name="C_GenAttributeInstance";

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
StringBuilder sb = new StringBuilder ("X_C_GenAttributeInstance[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set C_GenAttributeInstance_ID.
@param C_GenAttributeInstance_ID C_GenAttributeInstance_ID */
public void SetC_GenAttributeSetInstance_ID (int C_GenAttributeSetInstance_ID)
{
    if (C_GenAttributeSetInstance_ID < 1) throw new ArgumentException("C_GenAttributeSetInstance_ID is mandatory.");
    Set_ValueNoCheck("C_GenAttributeSetInstance_ID", C_GenAttributeSetInstance_ID);
}
/** Get C_GenAttributeInstance_ID.
@return C_GenAttributeInstance_ID */
public int GetC_GenAttributeSetInstance_ID() 
{
Object ii = Get_Value("C_GenAttributeSetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set C_GenAttributeValue_ID.
@param C_GenAttributeValue_ID C_GenAttributeValue_ID */
public void SetC_GenAttributeValue_ID (int C_GenAttributeValue_ID)
{
if (C_GenAttributeValue_ID <= 0) Set_Value ("C_GenAttributeValue_ID", null);
else
Set_Value ("C_GenAttributeValue_ID", C_GenAttributeValue_ID);
}
/** Get C_GenAttributeValue_ID.
@return C_GenAttributeValue_ID */
public int GetC_GenAttributeValue_ID() 
{
Object ii = Get_Value("C_GenAttributeValue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set C_GenAttribute_ID.
@param C_GenAttribute_ID C_GenAttribute_ID */
public void SetC_GenAttribute_ID (int C_GenAttribute_ID)
{
if (C_GenAttribute_ID < 1) throw new ArgumentException ("C_GenAttribute_ID is mandatory.");
Set_ValueNoCheck ("C_GenAttribute_ID", C_GenAttribute_ID);
}
/** Get C_GenAttribute_ID.
@return C_GenAttribute_ID */
public int GetC_GenAttribute_ID() 
{
Object ii = Get_Value("C_GenAttribute_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value != null && Value.Length > 40)
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
/** Set Value.
@param ValueNumber Numeric Value */
public void SetValueNumber (Decimal? ValueNumber)
{
Set_Value ("ValueNumber", (Decimal?)ValueNumber);
}
/** Get Value.
@return Numeric Value */
public Decimal GetValueNumber() 
{
Object bd =Get_Value("ValueNumber");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}

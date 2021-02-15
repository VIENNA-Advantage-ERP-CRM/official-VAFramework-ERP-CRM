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
/** Generated Model for VAB_GenFeatureInstance
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_GenFeatureInstance : PO
{
public X_VAB_GenFeatureInstance (Context ctx, int VAB_GenFeatureInstance_ID, Trx trxName) : base (ctx, VAB_GenFeatureInstance_ID, trxName)
{
/** if (VAB_GenFeatureInstance_ID == 0)
{
SetVAB_GenFeatureInstance_ID (0);
SetVAB_GenFeature_ID (0);
}
 */
}
public X_VAB_GenFeatureInstance (Ctx ctx, int VAB_GenFeatureInstance_ID, Trx trxName) : base (ctx, VAB_GenFeatureInstance_ID, trxName)
{
/** if (VAB_GenFeatureInstance_ID == 0)
{
SetVAB_GenFeatureInstance_ID (0);
SetVAB_GenFeature_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_GenFeatureInstance (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_GenFeatureInstance (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_GenFeatureInstance (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_GenFeatureInstance()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27667169129038L;
/** Last Updated Timestamp 11/21/2013 7:53:32 PM */
public static long updatedMS = 1385043812249L;
/** VAF_TableView_ID=1000419 */
public static int Table_ID;
 // =1000419;

/** TableName=VAB_GenFeatureInstance */
public static String Table_Name="VAB_GenFeatureInstance";

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
StringBuilder sb = new StringBuilder ("X_VAB_GenFeatureInstance[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set VAB_GenFeatureInstance_ID.
@param VAB_GenFeatureInstance_ID VAB_GenFeatureInstance_ID */
public void SetVAB_GenFeatureSetInstance_ID (int VAB_GenFeatureSetInstance_ID)
{
    if (VAB_GenFeatureSetInstance_ID < 1) throw new ArgumentException("VAB_GenFeatureSetInstance_ID is mandatory.");
    Set_ValueNoCheck("VAB_GenFeatureSetInstance_ID", VAB_GenFeatureSetInstance_ID);
}
/** Get VAB_GenFeatureInstance_ID.
@return VAB_GenFeatureInstance_ID */
public int GetVAB_GenFeatureSetInstance_ID() 
{
Object ii = Get_Value("VAB_GenFeatureSetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAB_GenFeatureValue_ID.
@param VAB_GenFeatureValue_ID VAB_GenFeatureValue_ID */
public void SetVAB_GenFeatureValue_ID (int VAB_GenFeatureValue_ID)
{
if (VAB_GenFeatureValue_ID <= 0) Set_Value ("VAB_GenFeatureValue_ID", null);
else
Set_Value ("VAB_GenFeatureValue_ID", VAB_GenFeatureValue_ID);
}
/** Get VAB_GenFeatureValue_ID.
@return VAB_GenFeatureValue_ID */
public int GetVAB_GenFeatureValue_ID() 
{
Object ii = Get_Value("VAB_GenFeatureValue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAB_GenFeature_ID.
@param VAB_GenFeature_ID VAB_GenFeature_ID */
public void SetVAB_GenFeature_ID (int VAB_GenFeature_ID)
{
if (VAB_GenFeature_ID < 1) throw new ArgumentException ("VAB_GenFeature_ID is mandatory.");
Set_ValueNoCheck ("VAB_GenFeature_ID", VAB_GenFeature_ID);
}
/** Get VAB_GenFeature_ID.
@return VAB_GenFeature_ID */
public int GetVAB_GenFeature_ID() 
{
Object ii = Get_Value("VAB_GenFeature_ID");
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

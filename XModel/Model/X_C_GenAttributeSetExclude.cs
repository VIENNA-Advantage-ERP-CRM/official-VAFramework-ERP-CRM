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
/** Generated Model for VAB_GenFeatureSetExclude
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_GenFeatureSetExclude : PO
{
public X_VAB_GenFeatureSetExclude (Context ctx, int VAB_GenFeatureSetExclude_ID, Trx trxName) : base (ctx, VAB_GenFeatureSetExclude_ID, trxName)
{
/** if (VAB_GenFeatureSetExclude_ID == 0)
{
SetVAF_TableView_ID (0);
SetVAB_GenFeatureSetExclude_ID (0);
SetVAB_GenFeatureSet_ID (0);
SetIsSOTrx (false);
}
 */
}
public X_VAB_GenFeatureSetExclude (Ctx ctx, int VAB_GenFeatureSetExclude_ID, Trx trxName) : base (ctx, VAB_GenFeatureSetExclude_ID, trxName)
{
/** if (VAB_GenFeatureSetExclude_ID == 0)
{
SetVAF_TableView_ID (0);
SetVAB_GenFeatureSetExclude_ID (0);
SetVAB_GenFeatureSet_ID (0);
SetIsSOTrx (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_GenFeatureSetExclude (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_GenFeatureSetExclude (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_GenFeatureSetExclude (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_GenFeatureSetExclude()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27667169105344L;
/** Last Updated Timestamp 11/21/2013 7:53:08 PM */
public static long updatedMS = 1385043788555L;
/** VAF_TableView_ID=1000422 */
public static int Table_ID;
 // =1000422;

/** TableName=VAB_GenFeatureSetExclude */
public static String Table_Name="VAB_GenFeatureSetExclude";

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
StringBuilder sb = new StringBuilder ("X_VAB_GenFeatureSetExclude[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID)
{
if (VAF_TableView_ID < 1) throw new ArgumentException ("VAF_TableView_ID is mandatory.");
Set_Value ("VAF_TableView_ID", VAF_TableView_ID);
}
/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() 
{
Object ii = Get_Value("VAF_TableView_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAB_GenFeatureSetExclude_ID.
@param VAB_GenFeatureSetExclude_ID VAB_GenFeatureSetExclude_ID */
public void SetVAB_GenFeatureSetExclude_ID (int VAB_GenFeatureSetExclude_ID)
{
if (VAB_GenFeatureSetExclude_ID < 1) throw new ArgumentException ("VAB_GenFeatureSetExclude_ID is mandatory.");
Set_ValueNoCheck ("VAB_GenFeatureSetExclude_ID", VAB_GenFeatureSetExclude_ID);
}
/** Get VAB_GenFeatureSetExclude_ID.
@return VAB_GenFeatureSetExclude_ID */
public int GetVAB_GenFeatureSetExclude_ID() 
{
Object ii = Get_Value("VAB_GenFeatureSetExclude_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAB_GenFeatureSet_ID.
@param VAB_GenFeatureSet_ID VAB_GenFeatureSet_ID */
public void SetVAB_GenFeatureSet_ID (int VAB_GenFeatureSet_ID)
{
if (VAB_GenFeatureSet_ID < 1) throw new ArgumentException ("VAB_GenFeatureSet_ID is mandatory.");
Set_ValueNoCheck ("VAB_GenFeatureSet_ID", VAB_GenFeatureSet_ID);
}
/** Get VAB_GenFeatureSet_ID.
@return VAB_GenFeatureSet_ID */
public int GetVAB_GenFeatureSet_ID() 
{
Object ii = Get_Value("VAB_GenFeatureSet_ID");
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
/** Set Sales Transaction.
@param IsSOTrx This is a Sales Transaction */
public void SetIsSOTrx (Boolean IsSOTrx)
{
Set_Value ("IsSOTrx", IsSOTrx);
}
/** Get Sales Transaction.
@return This is a Sales Transaction */
public Boolean IsSOTrx() 
{
Object oo = Get_Value("IsSOTrx");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}

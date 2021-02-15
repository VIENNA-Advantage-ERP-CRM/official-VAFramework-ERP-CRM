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
/** Generated Model for VAB_GenFeatureUse
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_GenFeatureUse : PO
{
public X_VAB_GenFeatureUse (Context ctx, int VAB_GenFeatureUse_ID, Trx trxName) : base (ctx, VAB_GenFeatureUse_ID, trxName)
{
/** if (VAB_GenFeatureUse_ID == 0)
{
SetVAB_GenFeatureSet_ID (0);
SetVAB_GenFeature_ID (0);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM VAM_PFeature_Use WHERE VAM_PFeature_Set_ID=@VAM_PFeature_Set_ID@
}
 */
}
public X_VAB_GenFeatureUse (Ctx ctx, int VAB_GenFeatureUse_ID, Trx trxName) : base (ctx, VAB_GenFeatureUse_ID, trxName)
{
/** if (VAB_GenFeatureUse_ID == 0)
{
SetVAB_GenFeatureSet_ID (0);
SetVAB_GenFeature_ID (0);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM VAM_PFeature_Use WHERE VAM_PFeature_Set_ID=@VAM_PFeature_Set_ID@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_GenFeatureUse (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_GenFeatureUse (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_GenFeatureUse (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_GenFeatureUse()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27667169133549L;
/** Last Updated Timestamp 11/21/2013 7:53:36 PM */
public static long updatedMS = 1385043816760L;
/** VAF_TableView_ID=1000424 */
public static int Table_ID;
 // =1000424;

/** TableName=VAB_GenFeatureUse */
public static String Table_Name="VAB_GenFeatureUse";

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
StringBuilder sb = new StringBuilder ("X_VAB_GenFeatureUse[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}

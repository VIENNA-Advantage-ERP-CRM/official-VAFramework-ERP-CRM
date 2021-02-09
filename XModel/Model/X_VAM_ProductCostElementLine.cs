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
/** Generated Model for VAM_ProductCostElementLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_ProductCostElementLine : PO
{
public X_VAM_ProductCostElementLine (Context ctx, int VAM_ProductCostElementLine_ID, Trx trxName) : base (ctx, VAM_ProductCostElementLine_ID, trxName)
{
/** if (VAM_ProductCostElementLine_ID == 0)
{
SetVAM_ProductCostElementLine_ID (0);
SetVAM_ProductCostElement_ID (0);
}
 */
}
public X_VAM_ProductCostElementLine(Ctx ctx, int VAM_ProductCostElementLine_ID, Trx trxName)
    : base(ctx, VAM_ProductCostElementLine_ID, trxName)
{
/** if (VAM_ProductCostElementLine_ID == 0)
{
SetVAM_ProductCostElementLine_ID (0);
SetVAM_ProductCostElement_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ProductCostElementLine(Context ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ProductCostElementLine(Ctx ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_ProductCostElementLine(Ctx ctx, IDataReader dr, Trx trxName)
    : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_ProductCostElementLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27742135977027L;
/** Last Updated Timestamp 4/7/2016 12:01:00 PM */
public static long updatedMS = 1460010660238L;
/** VAF_TableView_ID=1000479 */
public static int Table_ID;
 // =1000479;

/** TableName=VAM_ProductCostElementLine */
public static String Table_Name="VAM_ProductCostElementLine";

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
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
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
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_VAM_ProductCostElementLine[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Line.
@param LineNo Line No */
public void SetLineNo (int LineNo)
{
Set_Value ("LineNo", LineNo);
}
/** Get Line.
@return Line No */
public int GetLineNo() 
{
Object ii = Get_Value("LineNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAM_ProductCostElementLine_ID.
@param VAM_ProductCostElementLine_ID VAM_ProductCostElementLine_ID */
public void SetVAM_ProductCostElementLine_ID (int VAM_ProductCostElementLine_ID)
{
if (VAM_ProductCostElementLine_ID < 1) throw new ArgumentException ("VAM_ProductCostElementLine_ID is mandatory.");
Set_ValueNoCheck ("VAM_ProductCostElementLine_ID", VAM_ProductCostElementLine_ID);
}
/** Get VAM_ProductCostElementLine_ID.
@return VAM_ProductCostElementLine_ID */
public int GetVAM_ProductCostElementLine_ID() 
{
Object ii = Get_Value("VAM_ProductCostElementLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Cost Element.
@param VAM_ProductCostElement_ID Product Cost Element */
public void SetVAM_ProductCostElement_ID (int VAM_ProductCostElement_ID)
{
if (VAM_ProductCostElement_ID < 1) throw new ArgumentException ("VAM_ProductCostElement_ID is mandatory.");
Set_ValueNoCheck ("VAM_ProductCostElement_ID", VAM_ProductCostElement_ID);
}
/** Get Cost Element.
@return Product Cost Element */
public int GetVAM_ProductCostElement_ID() 
{
Object ii = Get_Value("VAM_ProductCostElement_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** M_Ref_CostElement VAF_Control_Ref_ID=1000170 */
public static int M_REF_COSTELEMENT_VAF_Control_Ref_ID=1000170;
/** Set Cost Element.
@param M_Ref_CostElement Cost Element */
public void SetM_Ref_CostElement (int M_Ref_CostElement)
{
Set_Value ("M_Ref_CostElement", M_Ref_CostElement);
}
/** Get Cost Element.
@return Cost Element */
public int GetM_Ref_CostElement() 
{
Object ii = Get_Value("M_Ref_CostElement");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}

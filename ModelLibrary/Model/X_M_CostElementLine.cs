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
/** Generated Model for M_CostElementLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_CostElementLine : PO
{
public X_M_CostElementLine (Context ctx, int M_CostElementLine_ID, Trx trxName) : base (ctx, M_CostElementLine_ID, trxName)
{
/** if (M_CostElementLine_ID == 0)
{
SetM_CostElementLine_ID (0);
SetM_CostElement_ID (0);
}
 */
}
public X_M_CostElementLine(Ctx ctx, int M_CostElementLine_ID, Trx trxName)
    : base(ctx, M_CostElementLine_ID, trxName)
{
/** if (M_CostElementLine_ID == 0)
{
SetM_CostElementLine_ID (0);
SetM_CostElement_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_CostElementLine(Context ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_CostElementLine(Ctx ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_CostElementLine(Ctx ctx, IDataReader dr, Trx trxName)
    : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_CostElementLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27742135977027L;
/** Last Updated Timestamp 4/7/2016 12:01:00 PM */
public static long updatedMS = 1460010660238L;
/** AD_Table_ID=1000479 */
public static int Table_ID;
 // =1000479;

/** TableName=M_CostElementLine */
public static String Table_Name="M_CostElementLine";

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
StringBuilder sb = new StringBuilder ("X_M_CostElementLine[").Append(Get_ID()).Append("]");
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
/** Set M_CostElementLine_ID.
@param M_CostElementLine_ID M_CostElementLine_ID */
public void SetM_CostElementLine_ID (int M_CostElementLine_ID)
{
if (M_CostElementLine_ID < 1) throw new ArgumentException ("M_CostElementLine_ID is mandatory.");
Set_ValueNoCheck ("M_CostElementLine_ID", M_CostElementLine_ID);
}
/** Get M_CostElementLine_ID.
@return M_CostElementLine_ID */
public int GetM_CostElementLine_ID() 
{
Object ii = Get_Value("M_CostElementLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Cost Element.
@param M_CostElement_ID Product Cost Element */
public void SetM_CostElement_ID (int M_CostElement_ID)
{
if (M_CostElement_ID < 1) throw new ArgumentException ("M_CostElement_ID is mandatory.");
Set_ValueNoCheck ("M_CostElement_ID", M_CostElement_ID);
}
/** Get Cost Element.
@return Product Cost Element */
public int GetM_CostElement_ID() 
{
Object ii = Get_Value("M_CostElement_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** M_Ref_CostElement AD_Reference_ID=1000170 */
public static int M_REF_COSTELEMENT_AD_Reference_ID=1000170;
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

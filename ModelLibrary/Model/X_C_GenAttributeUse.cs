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
/** Generated Model for C_GenAttributeUse
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_GenAttributeUse : PO
{
public X_C_GenAttributeUse (Context ctx, int C_GenAttributeUse_ID, Trx trxName) : base (ctx, C_GenAttributeUse_ID, trxName)
{
/** if (C_GenAttributeUse_ID == 0)
{
SetC_GenAttributeSet_ID (0);
SetC_GenAttribute_ID (0);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM M_AttributeUse WHERE M_AttributeSet_ID=@M_AttributeSet_ID@
}
 */
}
public X_C_GenAttributeUse (Ctx ctx, int C_GenAttributeUse_ID, Trx trxName) : base (ctx, C_GenAttributeUse_ID, trxName)
{
/** if (C_GenAttributeUse_ID == 0)
{
SetC_GenAttributeSet_ID (0);
SetC_GenAttribute_ID (0);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM M_AttributeUse WHERE M_AttributeSet_ID=@M_AttributeSet_ID@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeUse (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeUse (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_GenAttributeUse (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_GenAttributeUse()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27667169133549L;
/** Last Updated Timestamp 11/21/2013 7:53:36 PM */
public static long updatedMS = 1385043816760L;
/** AD_Table_ID=1000424 */
public static int Table_ID;
 // =1000424;

/** TableName=C_GenAttributeUse */
public static String Table_Name="C_GenAttributeUse";

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
StringBuilder sb = new StringBuilder ("X_C_GenAttributeUse[").Append(Get_ID()).Append("]");
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

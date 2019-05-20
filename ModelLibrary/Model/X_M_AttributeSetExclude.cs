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
/** Generated Model for M_AttributeSetExclude
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_AttributeSetExclude : PO
{
public X_M_AttributeSetExclude (Context ctx, int M_AttributeSetExclude_ID, Trx trxName) : base (ctx, M_AttributeSetExclude_ID, trxName)
{
/** if (M_AttributeSetExclude_ID == 0)
{
SetAD_Table_ID (0);
SetIsSOTrx (false);
SetM_AttributeSetExclude_ID (0);
SetM_AttributeSet_ID (0);
}
 */
}
public X_M_AttributeSetExclude (Ctx ctx, int M_AttributeSetExclude_ID, Trx trxName) : base (ctx, M_AttributeSetExclude_ID, trxName)
{
/** if (M_AttributeSetExclude_ID == 0)
{
SetAD_Table_ID (0);
SetIsSOTrx (false);
SetM_AttributeSetExclude_ID (0);
SetM_AttributeSet_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_AttributeSetExclude (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_AttributeSetExclude (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_AttributeSetExclude (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_AttributeSetExclude()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514378331L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061542L;
/** AD_Table_ID=809 */
public static int Table_ID;
 // =809;

/** TableName=M_AttributeSetExclude */
public static String Table_Name="M_AttributeSetExclude";

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
StringBuilder sb = new StringBuilder ("X_M_AttributeSetExclude[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID < 1) throw new ArgumentException ("AD_Table_ID is mandatory.");
Set_Value ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Exclude Attribute Set.
@param M_AttributeSetExclude_ID Exclude the ability to enter Attribute Sets */
public void SetM_AttributeSetExclude_ID (int M_AttributeSetExclude_ID)
{
if (M_AttributeSetExclude_ID < 1) throw new ArgumentException ("M_AttributeSetExclude_ID is mandatory.");
Set_ValueNoCheck ("M_AttributeSetExclude_ID", M_AttributeSetExclude_ID);
}
/** Get Exclude Attribute Set.
@return Exclude the ability to enter Attribute Sets */
public int GetM_AttributeSetExclude_ID() 
{
Object ii = Get_Value("M_AttributeSetExclude_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Attribute Set.
@param M_AttributeSet_ID Product Attribute Set */
public void SetM_AttributeSet_ID (int M_AttributeSet_ID)
{
if (M_AttributeSet_ID < 0) throw new ArgumentException ("M_AttributeSet_ID is mandatory.");
Set_ValueNoCheck ("M_AttributeSet_ID", M_AttributeSet_ID);
}
/** Get Attribute Set.
@return Product Attribute Set */
public int GetM_AttributeSet_ID() 
{
Object ii = Get_Value("M_AttributeSet_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}

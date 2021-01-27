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
/** Generated Model for VAB_ProjectSupplyMA
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_ProjectSupplyMA : PO
{
public X_VAB_ProjectSupplyMA (Context ctx, int VAB_ProjectSupplyMA_ID, Trx trxName) : base (ctx, VAB_ProjectSupplyMA_ID, trxName)
{
/** if (VAB_ProjectSupplyMA_ID == 0)
{
SetVAB_ProjectSupply_ID (0);
SetM_AttributeSetInstance_ID (0);
SetMovementQty (0.0);
}
 */
}
public X_VAB_ProjectSupplyMA (Ctx ctx, int VAB_ProjectSupplyMA_ID, Trx trxName) : base (ctx, VAB_ProjectSupplyMA_ID, trxName)
{
/** if (VAB_ProjectSupplyMA_ID == 0)
{
SetVAB_ProjectSupply_ID (0);
SetM_AttributeSetInstance_ID (0);
SetMovementQty (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectSupplyMA (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectSupplyMA (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_ProjectSupplyMA (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_ProjectSupplyMA()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374272L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057483L;
/** VAF_TableView_ID=761 */
public static int Table_ID;
 // =761;

/** TableName=VAB_ProjectSupplyMA */
public static String Table_Name="VAB_ProjectSupplyMA";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(1);
/** AccessLevel
@return 1 - Org 
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
StringBuilder sb = new StringBuilder ("X_VAB_ProjectSupplyMA[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Project Issue.
@param VAB_ProjectSupply_ID Project Issues (Material, Labor) */
public void SetVAB_ProjectSupply_ID (int VAB_ProjectSupply_ID)
{
if (VAB_ProjectSupply_ID < 1) throw new ArgumentException ("VAB_ProjectSupply_ID is mandatory.");
Set_ValueNoCheck ("VAB_ProjectSupply_ID", VAB_ProjectSupply_ID);
}
/** Get Project Issue.
@return Project Issues (Material, Labor) */
public int GetVAB_ProjectSupply_ID() 
{
Object ii = Get_Value("VAB_ProjectSupply_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAB_ProjectSupply_ID().ToString());
}
/** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
public void SetM_AttributeSetInstance_ID (int M_AttributeSetInstance_ID)
{
if (M_AttributeSetInstance_ID < 0) throw new ArgumentException ("M_AttributeSetInstance_ID is mandatory.");
Set_ValueNoCheck ("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
}
/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetM_AttributeSetInstance_ID() 
{
Object ii = Get_Value("M_AttributeSetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Movement Quantity.
@param MovementQty Quantity of a product moved. */
public void SetMovementQty (Decimal? MovementQty)
{
if (MovementQty == null) throw new ArgumentException ("MovementQty is mandatory.");
Set_Value ("MovementQty", (Decimal?)MovementQty);
}
/** Get Movement Quantity.
@return Quantity of a product moved. */
public Decimal GetMovementQty() 
{
Object bd =Get_Value("MovementQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}

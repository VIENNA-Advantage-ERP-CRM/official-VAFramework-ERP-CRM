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
/** Generated Model for M_ProductOperation
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_ProductOperation : PO
{
public X_M_ProductOperation (Context ctx, int M_ProductOperation_ID, Trx trxName) : base (ctx, M_ProductOperation_ID, trxName)
{
/** if (M_ProductOperation_ID == 0)
{
SetM_ProductOperation_ID (0);
SetM_Product_ID (0);
SetName (null);
}
 */
}
public X_M_ProductOperation (Ctx ctx, int M_ProductOperation_ID, Trx trxName) : base (ctx, M_ProductOperation_ID, trxName)
{
/** if (M_ProductOperation_ID == 0)
{
SetM_ProductOperation_ID (0);
SetM_Product_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ProductOperation (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ProductOperation (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ProductOperation (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_ProductOperation()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380713L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063924L;
/** AD_Table_ID=796 */
public static int Table_ID;
 // =796;

/** TableName=M_ProductOperation */
public static String Table_Name="M_ProductOperation";

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
StringBuilder sb = new StringBuilder ("X_M_ProductOperation[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}
/** Set Product Operation.
@param M_ProductOperation_ID Product Manufacturing Operation */
public void SetM_ProductOperation_ID (int M_ProductOperation_ID)
{
if (M_ProductOperation_ID < 1) throw new ArgumentException ("M_ProductOperation_ID is mandatory.");
Set_ValueNoCheck ("M_ProductOperation_ID", M_ProductOperation_ID);
}
/** Get Product Operation.
@return Product Manufacturing Operation */
public int GetM_ProductOperation_ID() 
{
Object ii = Get_Value("M_ProductOperation_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID < 1) throw new ArgumentException ("M_Product_ID is mandatory.");
Set_ValueNoCheck ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
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
/** Set Setup Time.
@param SetupTime Setup time before starting Production */
public void SetSetupTime (Decimal? SetupTime)
{
Set_Value ("SetupTime", (Decimal?)SetupTime);
}
/** Get Setup Time.
@return Setup time before starting Production */
public Decimal GetSetupTime() 
{
Object bd =Get_Value("SetupTime");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Teardown Time.
@param TeardownTime Time at the end of the operation */
public void SetTeardownTime (Decimal? TeardownTime)
{
Set_Value ("TeardownTime", (Decimal?)TeardownTime);
}
/** Get Teardown Time.
@return Time at the end of the operation */
public Decimal GetTeardownTime() 
{
Object bd =Get_Value("TeardownTime");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Runtime per Unit.
@param UnitRuntime Time to produce one unit */
public void SetUnitRuntime (Decimal? UnitRuntime)
{
Set_Value ("UnitRuntime", (Decimal?)UnitRuntime);
}
/** Get Runtime per Unit.
@return Time to produce one unit */
public Decimal GetUnitRuntime() 
{
Object bd =Get_Value("UnitRuntime");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}

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
/** Generated Model for C_UOM_Conversion
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_UOM_Conversion : PO
{
public X_C_UOM_Conversion (Context ctx, int C_UOM_Conversion_ID, Trx trxName) : base (ctx, C_UOM_Conversion_ID, trxName)
{
/** if (C_UOM_Conversion_ID == 0)
{
SetC_UOM_Conversion_ID (0);
SetC_UOM_ID (0);
SetC_UOM_To_ID (0);
SetDivideRate (0.0);
SetMultiplyRate (0.0);
}
 */
}
public X_C_UOM_Conversion (Ctx ctx, int C_UOM_Conversion_ID, Trx trxName) : base (ctx, C_UOM_Conversion_ID, trxName)
{
/** if (C_UOM_Conversion_ID == 0)
{
SetC_UOM_Conversion_ID (0);
SetC_UOM_ID (0);
SetC_UOM_To_ID (0);
SetDivideRate (0.0);
SetMultiplyRate (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_UOM_Conversion (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_UOM_Conversion (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_UOM_Conversion (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_UOM_Conversion()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375808L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059019L;
/** AD_Table_ID=175 */
public static int Table_ID;
 // =175;

/** TableName=C_UOM_Conversion */
public static String Table_Name="C_UOM_Conversion";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
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
StringBuilder sb = new StringBuilder ("X_C_UOM_Conversion[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set UOM Conversion.
@param C_UOM_Conversion_ID Unit of Measure Conversion */
public void SetC_UOM_Conversion_ID (int C_UOM_Conversion_ID)
{
if (C_UOM_Conversion_ID < 1) throw new ArgumentException ("C_UOM_Conversion_ID is mandatory.");
Set_ValueNoCheck ("C_UOM_Conversion_ID", C_UOM_Conversion_ID);
}
/** Get UOM Conversion.
@return Unit of Measure Conversion */
public int GetC_UOM_Conversion_ID() 
{
Object ii = Get_Value("C_UOM_Conversion_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_UOM_Conversion_ID().ToString());
}

/** C_UOM_ID AD_Reference_ID=114 */
public static int C_UOM_ID_AD_Reference_ID=114;
/** Set UOM.
@param C_UOM_ID Unit of Measure */
public void SetC_UOM_ID (int C_UOM_ID)
{
if (C_UOM_ID < 1) throw new ArgumentException ("C_UOM_ID is mandatory.");
Set_Value ("C_UOM_ID", C_UOM_ID);
}
/** Get UOM.
@return Unit of Measure */
public int GetC_UOM_ID() 
{
Object ii = Get_Value("C_UOM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_UOM_To_ID AD_Reference_ID=114 */
public static int C_UOM_TO_ID_AD_Reference_ID=114;
/** Set UoM To.
@param C_UOM_To_ID Target or destination Unit of Measure */
public void SetC_UOM_To_ID (int C_UOM_To_ID)
{
if (C_UOM_To_ID < 1) throw new ArgumentException ("C_UOM_To_ID is mandatory.");
Set_Value ("C_UOM_To_ID", C_UOM_To_ID);
}
/** Get UoM To.
@return Target or destination Unit of Measure */
public int GetC_UOM_To_ID() 
{
Object ii = Get_Value("C_UOM_To_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Divide Rate.
@param DivideRate To convert Source number to Target number, the Source is divided */
public void SetDivideRate (Decimal? DivideRate)
{
if (DivideRate == null) throw new ArgumentException ("DivideRate is mandatory.");
Set_Value ("DivideRate", (Decimal?)DivideRate);
}
/** Get Divide Rate.
@return To convert Source number to Target number, the Source is divided */
public Decimal GetDivideRate() 
{
Object bd =Get_Value("DivideRate");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);
else
Set_Value ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Multiply Rate.
@param MultiplyRate Rate to multiple the source by to calculate the target. */
public void SetMultiplyRate (Decimal? MultiplyRate)
{
if (MultiplyRate == null) throw new ArgumentException ("MultiplyRate is mandatory.");
Set_Value ("MultiplyRate", (Decimal?)MultiplyRate);
}
/** Get Multiply Rate.
@return Rate to multiple the source by to calculate the target. */
public Decimal GetMultiplyRate() 
{
Object bd =Get_Value("MultiplyRate");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set UPC/EAN.
@param UPC Bar Code (Universal Product Code or its superset European Article Number) */
public void SetUPC(String UPC)
{
    if (UPC != null && UPC.Length > 30)
    {
        log.Warning("Length > 30 - truncated");
        UPC = UPC.Substring(0, 30);
    }
    Set_Value("UPC", UPC);
}
/** Get UPC/EAN.
@return Bar Code (Universal Product Code or its superset European Article Number) */
public String GetUPC()
{
    return (String)Get_Value("UPC");
}
}

}

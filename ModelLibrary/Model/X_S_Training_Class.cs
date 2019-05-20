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
/** Generated Model for S_Training_Class
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_S_Training_Class : PO
{
public X_S_Training_Class (Context ctx, int S_Training_Class_ID, Trx trxName) : base (ctx, S_Training_Class_ID, trxName)
{
/** if (S_Training_Class_ID == 0)
{
SetEndDate (DateTime.Now);
SetM_Product_ID (0);
SetS_Training_Class_ID (0);
SetS_Training_ID (0);
SetStartDate (DateTime.Now);
}
 */
}
public X_S_Training_Class (Ctx ctx, int S_Training_Class_ID, Trx trxName) : base (ctx, S_Training_Class_ID, trxName)
{
/** if (S_Training_Class_ID == 0)
{
SetEndDate (DateTime.Now);
SetM_Product_ID (0);
SetS_Training_Class_ID (0);
SetS_Training_ID (0);
SetStartDate (DateTime.Now);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_S_Training_Class (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_S_Training_Class (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_S_Training_Class (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_S_Training_Class()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383942L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067153L;
/** AD_Table_ID=537 */
public static int Table_ID;
 // =537;

/** TableName=S_Training_Class */
public static String Table_Name="S_Training_Class";

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
StringBuilder sb = new StringBuilder ("X_S_Training_Class[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set End Date.
@param EndDate Last effective date (inclusive) */
public void SetEndDate (DateTime? EndDate)
{
if (EndDate == null) throw new ArgumentException ("EndDate is mandatory.");
Set_Value ("EndDate", (DateTime?)EndDate);
}
/** Get End Date.
@return Last effective date (inclusive) */
public DateTime? GetEndDate() 
{
return (DateTime?)Get_Value("EndDate");
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
/** Set Training Class.
@param S_Training_Class_ID The actual training class instance */
public void SetS_Training_Class_ID (int S_Training_Class_ID)
{
if (S_Training_Class_ID < 1) throw new ArgumentException ("S_Training_Class_ID is mandatory.");
Set_ValueNoCheck ("S_Training_Class_ID", S_Training_Class_ID);
}
/** Get Training Class.
@return The actual training class instance */
public int GetS_Training_Class_ID() 
{
Object ii = Get_Value("S_Training_Class_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Training.
@param S_Training_ID Repeated Training */
public void SetS_Training_ID (int S_Training_ID)
{
if (S_Training_ID < 1) throw new ArgumentException ("S_Training_ID is mandatory.");
Set_ValueNoCheck ("S_Training_ID", S_Training_ID);
}
/** Get Training.
@return Repeated Training */
public int GetS_Training_ID() 
{
Object ii = Get_Value("S_Training_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Start Date.
@param StartDate First effective day (inclusive) */
public void SetStartDate (DateTime? StartDate)
{
if (StartDate == null) throw new ArgumentException ("StartDate is mandatory.");
Set_Value ("StartDate", (DateTime?)StartDate);
}
/** Get Start Date.
@return First effective day (inclusive) */
public DateTime? GetStartDate() 
{
return (DateTime?)Get_Value("StartDate");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetStartDate().ToString());
}
}

}

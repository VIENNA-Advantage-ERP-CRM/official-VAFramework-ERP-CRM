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
/** Generated Model for VAM_PFeature_SetExclude
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAM_PFeature_SetExclude : PO
{
public X_VAM_PFeature_SetExclude (Context ctx, int VAM_PFeature_SetExclude_ID, Trx trxName) : base (ctx, VAM_PFeature_SetExclude_ID, trxName)
{
/** if (VAM_PFeature_SetExclude_ID == 0)
{
SetVAF_TableView_ID (0);
SetIsSOTrx (false);
SetVAM_PFeature_SetExclude_ID (0);
SetVAM_PFeature_Set_ID (0);
}
 */
}
public X_VAM_PFeature_SetExclude (Ctx ctx, int VAM_PFeature_SetExclude_ID, Trx trxName) : base (ctx, VAM_PFeature_SetExclude_ID, trxName)
{
/** if (VAM_PFeature_SetExclude_ID == 0)
{
SetVAF_TableView_ID (0);
SetIsSOTrx (false);
SetVAM_PFeature_SetExclude_ID (0);
SetVAM_PFeature_Set_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_PFeature_SetExclude (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_PFeature_SetExclude (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAM_PFeature_SetExclude (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAM_PFeature_SetExclude()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514378331L;
/** Last Updated Timestamp 7/29/2010 1:07:41 PM */
public static long updatedMS = 1280389061542L;
/** VAF_TableView_ID=809 */
public static int Table_ID;
 // =809;

/** TableName=VAM_PFeature_SetExclude */
public static String Table_Name="VAM_PFeature_SetExclude";

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
StringBuilder sb = new StringBuilder ("X_VAM_PFeature_SetExclude[").Append(Get_ID()).Append("]");
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
@param VAM_PFeature_SetExclude_ID Exclude the ability to enter Attribute Sets */
public void SetVAM_PFeature_SetExclude_ID (int VAM_PFeature_SetExclude_ID)
{
if (VAM_PFeature_SetExclude_ID < 1) throw new ArgumentException ("VAM_PFeature_SetExclude_ID is mandatory.");
Set_ValueNoCheck ("VAM_PFeature_SetExclude_ID", VAM_PFeature_SetExclude_ID);
}
/** Get Exclude Attribute Set.
@return Exclude the ability to enter Attribute Sets */
public int GetVAM_PFeature_SetExclude_ID() 
{
Object ii = Get_Value("VAM_PFeature_SetExclude_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Attribute Set.
@param VAM_PFeature_Set_ID Product Attribute Set */
public void SetVAM_PFeature_Set_ID (int VAM_PFeature_Set_ID)
{
if (VAM_PFeature_Set_ID < 0) throw new ArgumentException ("VAM_PFeature_Set_ID is mandatory.");
Set_ValueNoCheck ("VAM_PFeature_Set_ID", VAM_PFeature_Set_ID);
}
/** Get Attribute Set.
@return Product Attribute Set */
public int GetVAM_PFeature_Set_ID() 
{
Object ii = Get_Value("VAM_PFeature_Set_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}

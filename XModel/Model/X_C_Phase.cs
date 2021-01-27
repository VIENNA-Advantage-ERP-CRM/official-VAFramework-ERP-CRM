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
/** Generated Model for VAB_Std_Stage
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_Std_Stage : PO
{
public X_VAB_Std_Stage (Context ctx, int VAB_Std_Stage_ID, Trx trxName) : base (ctx, VAB_Std_Stage_ID, trxName)
{
/** if (VAB_Std_Stage_ID == 0)
{
SetVAB_Std_Stage_ID (0);
SetVAB_ProjectType_ID (0);
SetName (null);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM VAB_Std_Stage WHERE VAB_ProjectType_ID=@VAB_ProjectType_ID@
SetStandardQty (0.0);	// 1
}
 */
}
public X_VAB_Std_Stage (Ctx ctx, int VAB_Std_Stage_ID, Trx trxName) : base (ctx, VAB_Std_Stage_ID, trxName)
{
/** if (VAB_Std_Stage_ID == 0)
{
SetVAB_Std_Stage_ID (0);
SetVAB_ProjectType_ID (0);
SetName (null);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM VAB_Std_Stage WHERE VAB_ProjectType_ID=@VAB_ProjectType_ID@
SetStandardQty (0.0);	// 1
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Std_Stage (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Std_Stage (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_Std_Stage (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_Std_Stage()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374131L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057342L;
/** VAF_TableView_ID=577 */
public static int Table_ID;
 // =577;

/** TableName=VAB_Std_Stage */
public static String Table_Name="VAB_Std_Stage";

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
StringBuilder sb = new StringBuilder ("X_VAB_Std_Stage[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Standard Phase.
@param VAB_Std_Stage_ID Standard Phase of the Project Type */
public void SetVAB_Std_Stage_ID (int VAB_Std_Stage_ID)
{
if (VAB_Std_Stage_ID < 1) throw new ArgumentException ("VAB_Std_Stage_ID is mandatory.");
Set_ValueNoCheck ("VAB_Std_Stage_ID", VAB_Std_Stage_ID);
}
/** Get Standard Phase.
@return Standard Phase of the Project Type */
public int GetVAB_Std_Stage_ID() 
{
Object ii = Get_Value("VAB_Std_Stage_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project Type.
@param VAB_ProjectType_ID Type of the project */
public void SetVAB_ProjectType_ID (int VAB_ProjectType_ID)
{
if (VAB_ProjectType_ID < 1) throw new ArgumentException ("VAB_ProjectType_ID is mandatory.");
Set_ValueNoCheck ("VAB_ProjectType_ID", VAB_ProjectType_ID);
}
/** Get Project Type.
@return Type of the project */
public int GetVAB_ProjectType_ID() 
{
Object ii = Get_Value("VAB_ProjectType_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Standard Quantity.
@param StandardQty Standard Quantity */
public void SetStandardQty (Decimal? StandardQty)
{
if (StandardQty == null) throw new ArgumentException ("StandardQty is mandatory.");
Set_Value ("StandardQty", (Decimal?)StandardQty);
}
/** Get Standard Quantity.
@return Standard Quantity */
public Decimal GetStandardQty() 
{
Object bd =Get_Value("StandardQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}

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
/** Generated Model for AD_Module
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Module : PO
{
public X_AD_Module (Context ctx, int AD_Module_ID, Trx trxName) : base (ctx, AD_Module_ID, trxName)
{
/** if (AD_Module_ID == 0)
{
SetAD_Module_ID (0);
SetName (null);
}
 */
}
public X_AD_Module (Ctx ctx, int AD_Module_ID, Trx trxName) : base (ctx, AD_Module_ID, trxName)
{
/** if (AD_Module_ID == 0)
{
SetAD_Module_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Module (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Module (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Module (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Module()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27634389867916L;
/** Last Updated Timestamp 11/7/2012 10:32:31 AM */
public static long updatedMS = 1352264551127L;
/** AD_Table_ID=1000012 */
public static int Table_ID;
 // =1000012;

/** TableName=AD_Module */
public static String Table_Name="AD_Module";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_AD_Module[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Image.
@param AD_Image_ID Image or Icon */
public void SetAD_Image_ID (int AD_Image_ID)
{
if (AD_Image_ID <= 0) Set_Value ("AD_Image_ID", null);
else
Set_Value ("AD_Image_ID", AD_Image_ID);
}
/** Get Image.
@return Image or Icon */
public int GetAD_Image_ID() 
{
Object ii = Get_Value("AD_Image_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set AD_Module_ID.
@param AD_Module_ID AD_Module_ID */
public void SetAD_Module_ID (int AD_Module_ID)
{
if (AD_Module_ID < 1) throw new ArgumentException ("AD_Module_ID is mandatory.");
Set_ValueNoCheck ("AD_Module_ID", AD_Module_ID);
}
/** Get AD_Module_ID.
@return AD_Module_ID */
public int GetAD_Module_ID() 
{
Object ii = Get_Value("AD_Module_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 50)
{
log.Warning("Length > 50 - truncated");
Description = Description.Substring(0,50);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
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
Set_ValueNoCheck ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 50)
{
log.Warning("Length > 50 - truncated");
Name = Name.Substring(0,50);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
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

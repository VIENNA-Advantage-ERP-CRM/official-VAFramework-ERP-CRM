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
/** Generated Model for C_PaymentTerm_Trl
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_PaymentTerm_Trl : PO
{
public X_C_PaymentTerm_Trl (Context ctx, int C_PaymentTerm_Trl_ID, Trx trxName) : base (ctx, C_PaymentTerm_Trl_ID, trxName)
{
/** if (C_PaymentTerm_Trl_ID == 0)
{
SetAD_Language (null);
SetC_PaymentTerm_ID (0);
SetIsTranslated (false);
SetName (null);
}
 */
}
public X_C_PaymentTerm_Trl (Ctx ctx, int C_PaymentTerm_Trl_ID, Trx trxName) : base (ctx, C_PaymentTerm_Trl_ID, trxName)
{
/** if (C_PaymentTerm_Trl_ID == 0)
{
SetAD_Language (null);
SetC_PaymentTerm_ID (0);
SetIsTranslated (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PaymentTerm_Trl (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PaymentTerm_Trl (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PaymentTerm_Trl (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_PaymentTerm_Trl()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27716755601852L;
/** Last Updated Timestamp 6/18/2015 5:54:45 PM */
public static long updatedMS = 1434630285063L;
/** AD_Table_ID=303 */
public static int Table_ID;
 // =303;

/** TableName=C_PaymentTerm_Trl */
public static String Table_Name="C_PaymentTerm_Trl";

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
StringBuilder sb = new StringBuilder ("X_C_PaymentTerm_Trl[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_Language AD_Reference_ID=106 */
public static int AD_LANGUAGE_AD_Reference_ID=106;
/** Set Language.
@param AD_Language Language for this entity */
public void SetAD_Language (String AD_Language)
{
if (AD_Language.Length > 5)
{
log.Warning("Length > 5 - truncated");
AD_Language = AD_Language.Substring(0,5);
}
Set_ValueNoCheck ("AD_Language", AD_Language);
}
/** Get Language.
@return Language for this entity */
public String GetAD_Language() 
{
return (String)Get_Value("AD_Language");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_Language().ToString());
}
/** Set Payment Term.
@param C_PaymentTerm_ID The terms of Payment (timing, discount) */
public void SetC_PaymentTerm_ID (int C_PaymentTerm_ID)
{
if (C_PaymentTerm_ID < 1) throw new ArgumentException ("C_PaymentTerm_ID is mandatory.");
Set_ValueNoCheck ("C_PaymentTerm_ID", C_PaymentTerm_ID);
}
/** Get Payment Term.
@return The terms of Payment (timing, discount) */
public int GetC_PaymentTerm_ID() 
{
Object ii = Get_Value("C_PaymentTerm_ID");
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
/** Set Document Note.
@param DocumentNote Additional information for a Document */
public void SetDocumentNote (String DocumentNote)
{
if (DocumentNote != null && DocumentNote.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
DocumentNote = DocumentNote.Substring(0,2000);
}
Set_Value ("DocumentNote", DocumentNote);
}
/** Get Document Note.
@return Additional information for a Document */
public String GetDocumentNote() 
{
return (String)Get_Value("DocumentNote");
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
/** Set Translated.
@param IsTranslated This column is translated */
public void SetIsTranslated (Boolean IsTranslated)
{
Set_Value ("IsTranslated", IsTranslated);
}
/** Get Translated.
@return This column is translated */
public Boolean IsTranslated() 
{
Object oo = Get_Value("IsTranslated");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
}

}

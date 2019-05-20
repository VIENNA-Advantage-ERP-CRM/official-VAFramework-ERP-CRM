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
/** Generated Model for S_Training
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_S_Training : PO
{
public X_S_Training (Context ctx, int S_Training_ID, Trx trxName) : base (ctx, S_Training_ID, trxName)
{
/** if (S_Training_ID == 0)
{
SetC_TaxCategory_ID (0);
SetC_UOM_ID (0);
SetM_Product_Category_ID (0);
SetName (null);
SetS_Training_ID (0);
}
 */
}
public X_S_Training (Ctx ctx, int S_Training_ID, Trx trxName) : base (ctx, S_Training_ID, trxName)
{
/** if (S_Training_ID == 0)
{
SetC_TaxCategory_ID (0);
SetC_UOM_ID (0);
SetM_Product_Category_ID (0);
SetName (null);
SetS_Training_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_S_Training (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_S_Training (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_S_Training (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_S_Training()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383926L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067137L;
/** AD_Table_ID=538 */
public static int Table_ID;
 // =538;

/** TableName=S_Training */
public static String Table_Name="S_Training";

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
StringBuilder sb = new StringBuilder ("X_S_Training[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Tax Category.
@param C_TaxCategory_ID Tax Category */
public void SetC_TaxCategory_ID (int C_TaxCategory_ID)
{
if (C_TaxCategory_ID < 1) throw new ArgumentException ("C_TaxCategory_ID is mandatory.");
Set_Value ("C_TaxCategory_ID", C_TaxCategory_ID);
}
/** Get Tax Category.
@return Tax Category */
public int GetC_TaxCategory_ID() 
{
Object ii = Get_Value("C_TaxCategory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
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
/** Set Description URL.
@param DescriptionURL URL for the description */
public void SetDescriptionURL (String DescriptionURL)
{
if (DescriptionURL != null && DescriptionURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
DescriptionURL = DescriptionURL.Substring(0,120);
}
Set_Value ("DescriptionURL", DescriptionURL);
}
/** Get Description URL.
@return URL for the description */
public String GetDescriptionURL() 
{
return (String)Get_Value("DescriptionURL");
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
/** Set Image URL.
@param ImageURL URL of  image */
public void SetImageURL (String ImageURL)
{
if (ImageURL != null && ImageURL.Length > 120)
{
log.Warning("Length > 120 - truncated");
ImageURL = ImageURL.Substring(0,120);
}
Set_Value ("ImageURL", ImageURL);
}
/** Get Image URL.
@return URL of  image */
public String GetImageURL() 
{
return (String)Get_Value("ImageURL");
}
/** Set Product Category.
@param M_Product_Category_ID Category of a Product */
public void SetM_Product_Category_ID (int M_Product_Category_ID)
{
if (M_Product_Category_ID < 1) throw new ArgumentException ("M_Product_Category_ID is mandatory.");
Set_Value ("M_Product_Category_ID", M_Product_Category_ID);
}
/** Get Product Category.
@return Category of a Product */
public int GetM_Product_Category_ID() 
{
Object ii = Get_Value("M_Product_Category_ID");
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
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
}

}

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
/** Generated Model for AD_Modification
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Modification : PO
{
public X_AD_Modification (Context ctx, int AD_Modification_ID, Trx trxName) : base (ctx, AD_Modification_ID, trxName)
{
/** if (AD_Modification_ID == 0)
{
SetAD_Modification_ID (0);
SetAD_Version_ID (0);
SetModificationType (null);
SetName (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 FROM AD_Modification WHERE AD_Version_ID='@AD_Version_ID@'
}
 */
}
public X_AD_Modification (Ctx ctx, int AD_Modification_ID, Trx trxName) : base (ctx, AD_Modification_ID, trxName)
{
/** if (AD_Modification_ID == 0)
{
SetAD_Modification_ID (0);
SetAD_Version_ID (0);
SetModificationType (null);
SetName (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(MAX(SeqNo),0)+10 FROM AD_Modification WHERE AD_Version_ID='@AD_Version_ID@'
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Modification (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Modification (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Modification (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Modification()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362266L;
/** Last Updated Timestamp 7/29/2010 1:07:25 PM */
public static long updatedMS = 1280389045477L;
/** AD_Table_ID=883 */
public static int Table_ID;
 // =883;

/** TableName=AD_Modification */
public static String Table_Name="AD_Modification";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_AD_Modification[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Modification.
@param AD_Modification_ID System Modification or Extension */
public void SetAD_Modification_ID (int AD_Modification_ID)
{
if (AD_Modification_ID < 1) throw new ArgumentException ("AD_Modification_ID is mandatory.");
Set_ValueNoCheck ("AD_Modification_ID", AD_Modification_ID);
}
/** Get Modification.
@return System Modification or Extension */
public int GetAD_Modification_ID() 
{
Object ii = Get_Value("AD_Modification_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Entity Version.
@param AD_Version_ID Entity Version */
public void SetAD_Version_ID (int AD_Version_ID)
{
if (AD_Version_ID < 1) throw new ArgumentException ("AD_Version_ID is mandatory.");
Set_ValueNoCheck ("AD_Version_ID", AD_Version_ID);
}
/** Get Entity Version.
@return Entity Version */
public int GetAD_Version_ID() 
{
Object ii = Get_Value("AD_Version_ID");
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

/** ModificationType AD_Reference_ID=429 */
public static int MODIFICATIONTYPE_AD_Reference_ID=429;
/** Functionality Improvement = 1 */
public static String MODIFICATIONTYPE_FunctionalityImprovement = "1";
/** Technology Improvement = 2 */
public static String MODIFICATIONTYPE_TechnologyImprovement = "2";
/** Business Process Improvement = 3 */
public static String MODIFICATIONTYPE_BusinessProcessImprovement = "3";
/** Bug Fix = 9 */
public static String MODIFICATIONTYPE_BugFix = "9";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsModificationTypeValid (String test)
{
return test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("9");
}
/** Set Modification Type.
@param ModificationType Type of Modification */
public void SetModificationType (String ModificationType)
{
if (ModificationType == null) throw new ArgumentException ("ModificationType is mandatory");
if (!IsModificationTypeValid(ModificationType))
throw new ArgumentException ("ModificationType Invalid value - " + ModificationType + " - Reference_ID=429 - 1 - 2 - 3 - 9");
if (ModificationType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ModificationType = ModificationType.Substring(0,1);
}
Set_Value ("ModificationType", ModificationType);
}
/** Get Modification Type.
@return Type of Modification */
public String GetModificationType() 
{
return (String)Get_Value("ModificationType");
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
}

}

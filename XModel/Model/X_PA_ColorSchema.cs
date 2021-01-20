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
/** Generated Model for PA_ColorSchema
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_PA_ColorSchema : PO
{
public X_PA_ColorSchema (Context ctx, int PA_ColorSchema_ID, Trx trxName) : base (ctx, PA_ColorSchema_ID, trxName)
{
/** if (PA_ColorSchema_ID == 0)
{
SetVAF_Print_Rpt_Colour1_ID (0);
SetVAF_Print_Rpt_Colour2_ID (0);
SetEntityType (null);	// U
SetMark1Percent (0);
SetMark2Percent (0);
SetName (null);
SetPA_ColorSchema_ID (0);
}
 */
}
public X_PA_ColorSchema (Ctx ctx, int PA_ColorSchema_ID, Trx trxName) : base (ctx, PA_ColorSchema_ID, trxName)
{
/** if (PA_ColorSchema_ID == 0)
{
SetVAF_Print_Rpt_Colour1_ID (0);
SetVAF_Print_Rpt_Colour2_ID (0);
SetEntityType (null);	// U
SetMark1Percent (0);
SetMark2Percent (0);
SetName (null);
SetPA_ColorSchema_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_ColorSchema (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_ColorSchema (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_ColorSchema (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_PA_ColorSchema()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381653L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064864L;
/** VAF_TableView_ID=831 */
public static int Table_ID;
 // =831;

/** TableName=PA_ColorSchema */
public static String Table_Name="PA_ColorSchema";

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
StringBuilder sb = new StringBuilder ("X_PA_ColorSchema[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** VAF_Print_Rpt_Colour1_ID VAF_Control_Ref_ID=266 */
public static int VAF_PRINT_RPT_COLOUR1_ID_VAF_Control_Ref_ID=266;
/** Set Color 1.
@param VAF_Print_Rpt_Colour1_ID First color used */
public void SetVAF_Print_Rpt_Colour1_ID (int VAF_Print_Rpt_Colour1_ID)
{
if (VAF_Print_Rpt_Colour1_ID < 1) throw new ArgumentException ("VAF_Print_Rpt_Colour1_ID is mandatory.");
Set_Value ("VAF_Print_Rpt_Colour1_ID", VAF_Print_Rpt_Colour1_ID);
}
/** Get Color 1.
@return First color used */
public int GetVAF_Print_Rpt_Colour1_ID() 
{
Object ii = Get_Value("VAF_Print_Rpt_Colour1_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAF_Print_Rpt_Colour2_ID VAF_Control_Ref_ID=266 */
public static int VAF_PRINT_RPT_COLOUR2_ID_VAF_Control_Ref_ID=266;
/** Set Color 2.
@param VAF_Print_Rpt_Colour2_ID Second color used */
public void SetVAF_Print_Rpt_Colour2_ID (int VAF_Print_Rpt_Colour2_ID)
{
if (VAF_Print_Rpt_Colour2_ID < 1) throw new ArgumentException ("VAF_Print_Rpt_Colour2_ID is mandatory.");
Set_Value ("VAF_Print_Rpt_Colour2_ID", VAF_Print_Rpt_Colour2_ID);
}
/** Get Color 2.
@return Second color used */
public int GetVAF_Print_Rpt_Colour2_ID() 
{
Object ii = Get_Value("VAF_Print_Rpt_Colour2_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAF_Print_Rpt_Colour3_ID VAF_Control_Ref_ID=266 */
public static int VAF_PRINT_RPT_COLOUR3_ID_VAF_Control_Ref_ID=266;
/** Set Color 3.
@param VAF_Print_Rpt_Colour3_ID Third color used */
public void SetVAF_Print_Rpt_Colour3_ID (int VAF_Print_Rpt_Colour3_ID)
{
if (VAF_Print_Rpt_Colour3_ID <= 0) Set_Value ("VAF_Print_Rpt_Colour3_ID", null);
else
Set_Value ("VAF_Print_Rpt_Colour3_ID", VAF_Print_Rpt_Colour3_ID);
}
/** Get Color 3.
@return Third color used */
public int GetVAF_Print_Rpt_Colour3_ID() 
{
Object ii = Get_Value("VAF_Print_Rpt_Colour3_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAF_Print_Rpt_Colour4_ID VAF_Control_Ref_ID=266 */
public static int VAF_PRINT_RPT_COLOUR4_ID_VAF_Control_Ref_ID=266;
/** Set Color 4.
@param VAF_Print_Rpt_Colour4_ID Forth color used */
public void SetVAF_Print_Rpt_Colour4_ID (int VAF_Print_Rpt_Colour4_ID)
{
if (VAF_Print_Rpt_Colour4_ID <= 0) Set_Value ("VAF_Print_Rpt_Colour4_ID", null);
else
Set_Value ("VAF_Print_Rpt_Colour4_ID", VAF_Print_Rpt_Colour4_ID);
}
/** Get Color 4.
@return Forth color used */
public int GetVAF_Print_Rpt_Colour4_ID() 
{
Object ii = Get_Value("VAF_Print_Rpt_Colour4_ID");
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

/** EntityType VAF_Control_Ref_ID=389 */
public static int ENTITYTYPE_VAF_Control_Ref_ID=389;
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_Value ("EntityType", EntityType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetEntityType() 
{
return (String)Get_Value("EntityType");
}
/** Set Mark 1 Percent.
@param Mark1Percent Percentage up to this color is used */
public void SetMark1Percent (int Mark1Percent)
{
Set_Value ("Mark1Percent", Mark1Percent);
}
/** Get Mark 1 Percent.
@return Percentage up to this color is used */
public int GetMark1Percent() 
{
Object ii = Get_Value("Mark1Percent");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Mark 2 Percent.
@param Mark2Percent Percentage up to this color is used */
public void SetMark2Percent (int Mark2Percent)
{
Set_Value ("Mark2Percent", Mark2Percent);
}
/** Get Mark 2 Percent.
@return Percentage up to this color is used */
public int GetMark2Percent() 
{
Object ii = Get_Value("Mark2Percent");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Mark 3 Percent.
@param Mark3Percent Percentage up to this color is used */
public void SetMark3Percent (int Mark3Percent)
{
Set_Value ("Mark3Percent", Mark3Percent);
}
/** Get Mark 3 Percent.
@return Percentage up to this color is used */
public int GetMark3Percent() 
{
Object ii = Get_Value("Mark3Percent");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Mark 4 Percent.
@param Mark4Percent Percentage up to this color is used */
public void SetMark4Percent (int Mark4Percent)
{
Set_Value ("Mark4Percent", Mark4Percent);
}
/** Get Mark 4 Percent.
@return Percentage up to this color is used */
public int GetMark4Percent() 
{
Object ii = Get_Value("Mark4Percent");
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
/** Set Color Schema.
@param PA_ColorSchema_ID Performance Color Schema */
public void SetPA_ColorSchema_ID (int PA_ColorSchema_ID)
{
if (PA_ColorSchema_ID < 1) throw new ArgumentException ("PA_ColorSchema_ID is mandatory.");
Set_ValueNoCheck ("PA_ColorSchema_ID", PA_ColorSchema_ID);
}
/** Get Color Schema.
@return Performance Color Schema */
public int GetPA_ColorSchema_ID() 
{
Object ii = Get_Value("PA_ColorSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}

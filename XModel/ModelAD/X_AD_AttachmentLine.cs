namespace VAdvantage.Model
{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for VAF_AttachmentLine
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_AttachmentLine : PO{public X_VAF_AttachmentLine (Context ctx, int VAF_AttachmentLine_ID, Trx trxName) : base (ctx, VAF_AttachmentLine_ID, trxName){/** if (VAF_AttachmentLine_ID == 0){SetVAF_AttachmentLine_ID (0);} */
}public X_VAF_AttachmentLine (Ctx ctx, int VAF_AttachmentLine_ID, Trx trxName) : base (ctx, VAF_AttachmentLine_ID, trxName){/** if (VAF_AttachmentLine_ID == 0){SetVAF_AttachmentLine_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AttachmentLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AttachmentLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AttachmentLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_AttachmentLine(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27878748375598L;/** Last Updated Timestamp 8/5/2020 10:24:19 AM */
public static long updatedMS = 1596623058809L;/** VAF_TableView_ID=1000444 */
public static int Table_ID; // =1000444;
/** TableName=VAF_AttachmentLine */
public static String Table_Name="VAF_AttachmentLine";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(6);/** AccessLevel
@return 6 - System - Client 
*/
protected override int Get_AccessLevel(){return Convert.ToInt32(accessLevel.ToString());}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Info
@return info
*/
public override String ToString(){StringBuilder sb = new StringBuilder ("X_VAF_AttachmentLine[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set VAF_AttachmentLine_ID.
@param VAF_AttachmentLine_ID VAF_AttachmentLine_ID */
public void SetVAF_AttachmentLine_ID (int VAF_AttachmentLine_ID){if (VAF_AttachmentLine_ID < 1) throw new ArgumentException ("VAF_AttachmentLine_ID is mandatory.");Set_ValueNoCheck ("VAF_AttachmentLine_ID", VAF_AttachmentLine_ID);}/** Get VAF_AttachmentLine_ID.
@return VAF_AttachmentLine_ID */
public int GetVAF_AttachmentLine_ID() {Object ii = Get_Value("VAF_AttachmentLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Attachment.
@param VAF_Attachment_ID Attachment for the document */
public void SetVAF_Attachment_ID (int VAF_Attachment_ID){if (VAF_Attachment_ID <= 0) Set_Value ("VAF_Attachment_ID", null);else
Set_Value ("VAF_Attachment_ID", VAF_Attachment_ID);}/** Get Attachment.
@return Attachment for the document */
public int GetVAF_Attachment_ID() {Object ii = Get_Value("VAF_Attachment_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set BinaryData.
@param BinaryData Binary Data */
public void SetBinaryData (Byte[] BinaryData){Set_Value ("BinaryData", BinaryData);}/** Get BinaryData.
@return Binary Data */
public Byte[] GetBinaryData() {return (Byte[])Get_Value("BinaryData");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set File Name.
@param FileName Name of the local file or URL */
public void SetFileName (String FileName){if (FileName != null && FileName.Length > 250){log.Warning("Length > 250 - truncated");FileName = FileName.Substring(0,250);}Set_Value ("FileName", FileName);}/** Get File Name.
@return Name of the local file or URL */
public String GetFileName() {return (String)Get_Value("FileName");}/** Set File Size.
@param FileSize Size of the File in bytes */
public void SetFileSize (Decimal? FileSize){Set_Value ("FileSize", (Decimal?)FileSize);}/** Get File Size.
@return Size of the File in bytes */
public Decimal GetFileSize() {Object bd =Get_Value("FileSize");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set FileType.
@param FileType FileType */
public void SetFileType (String FileType){if (FileType != null && FileType.Length > 10){log.Warning("Length > 10 - truncated");FileType = FileType.Substring(0,10);}Set_Value ("FileType", FileType);}/** Get FileType.
@return FileType */
public String GetFileType() {return (String)Get_Value("FileType");}}
}
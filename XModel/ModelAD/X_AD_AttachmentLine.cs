namespace VAdvantage.Model
{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for AD_AttachmentLine
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_AttachmentLine : PO{public X_AD_AttachmentLine (Context ctx, int AD_AttachmentLine_ID, Trx trxName) : base (ctx, AD_AttachmentLine_ID, trxName){/** if (AD_AttachmentLine_ID == 0){SetAD_AttachmentLine_ID (0);} */
}public X_AD_AttachmentLine (Ctx ctx, int AD_AttachmentLine_ID, Trx trxName) : base (ctx, AD_AttachmentLine_ID, trxName){/** if (AD_AttachmentLine_ID == 0){SetAD_AttachmentLine_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AttachmentLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AttachmentLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AttachmentLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_AttachmentLine(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27878748375598L;/** Last Updated Timestamp 8/5/2020 10:24:19 AM */
public static long updatedMS = 1596623058809L;/** AD_Table_ID=1000444 */
public static int Table_ID; // =1000444;
/** TableName=AD_AttachmentLine */
public static String Table_Name="AD_AttachmentLine";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_AD_AttachmentLine[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set AD_AttachmentLine_ID.
@param AD_AttachmentLine_ID AD_AttachmentLine_ID */
public void SetAD_AttachmentLine_ID (int AD_AttachmentLine_ID){if (AD_AttachmentLine_ID < 1) throw new ArgumentException ("AD_AttachmentLine_ID is mandatory.");Set_ValueNoCheck ("AD_AttachmentLine_ID", AD_AttachmentLine_ID);}/** Get AD_AttachmentLine_ID.
@return AD_AttachmentLine_ID */
public int GetAD_AttachmentLine_ID() {Object ii = Get_Value("AD_AttachmentLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Attachment.
@param AD_Attachment_ID Attachment for the document */
public void SetAD_Attachment_ID (int AD_Attachment_ID){if (AD_Attachment_ID <= 0) Set_Value ("AD_Attachment_ID", null);else
Set_Value ("AD_Attachment_ID", AD_Attachment_ID);}/** Get Attachment.
@return Attachment for the document */
public int GetAD_Attachment_ID() {Object ii = Get_Value("AD_Attachment_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set BinaryData.
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
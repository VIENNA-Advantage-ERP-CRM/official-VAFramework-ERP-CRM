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
    using System.Data;/** Generated Model for VAF_AttachmentReference
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_AttachmentReference : PO
    {
        public X_VAF_AttachmentReference(Context ctx, int VAF_AttachmentReference_ID, Trx trxName) : base(ctx, VAF_AttachmentReference_ID, trxName)
        {/** if (VAF_AttachmentReference_ID == 0){SetVAF_AttachmentLine_ID (0);SetVAF_AttachmentRef (null);SetVAF_AttachmentReference_ID (0);} */
        }
        public X_VAF_AttachmentReference(Ctx ctx, int VAF_AttachmentReference_ID, Trx trxName) : base(ctx, VAF_AttachmentReference_ID, trxName)
        {/** if (VAF_AttachmentReference_ID == 0){SetVAF_AttachmentLine_ID (0);SetVAF_AttachmentRef (null);SetVAF_AttachmentReference_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_AttachmentReference(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_AttachmentReference(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_AttachmentReference(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAF_AttachmentReference() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27864742157215L;/** Last Updated Timestamp 2/25/2020 1:17:20 PM */
        public static long updatedMS = 1582616840426L;/** VAF_TableView_ID=1001039 */
        public static int Table_ID; // =1001039;
                                    /** TableName=VAF_AttachmentReference */
        public static String Table_Name = "VAF_AttachmentReference";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
        protected override int Get_AccessLevel() { return Convert.ToInt32(accessLevel.ToString()); }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Context ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Ctx ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Info
@return info
*/
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VAF_AttachmentReference[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Attachment Line.
@param VAF_AttachmentLine_ID Linked Attachment */
        public void SetVAF_AttachmentLine_ID(int VAF_AttachmentLine_ID) { if (VAF_AttachmentLine_ID < 1) throw new ArgumentException("VAF_AttachmentLine_ID is mandatory."); Set_Value("VAF_AttachmentLine_ID", VAF_AttachmentLine_ID); }/** Get Attachment Line.
@return Linked Attachment */
        public int GetVAF_AttachmentLine_ID() { Object ii = Get_Value("VAF_AttachmentLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Attachment Refeference.
@param VAF_AttachmentRef Reference of the attachment */
        public void SetVAF_AttachmentRef(String VAF_AttachmentRef) { if (VAF_AttachmentRef == null) throw new ArgumentException("VAF_AttachmentRef is mandatory."); if (VAF_AttachmentRef.Length > 100) { log.Warning("Length > 100 - truncated"); VAF_AttachmentRef = VAF_AttachmentRef.Substring(0, 100); } Set_Value("VAF_AttachmentRef", VAF_AttachmentRef); }/** Get Attachment Refeference.
@return Reference of the attachment */
        public String GetVAF_AttachmentRef() { return (String)Get_Value("VAF_AttachmentRef"); }/** Set VAF_AttachmentReference_ID.
@param VAF_AttachmentReference_ID VAF_AttachmentReference_ID */
        public void SetVAF_AttachmentReference_ID(int VAF_AttachmentReference_ID) { if (VAF_AttachmentReference_ID < 1) throw new ArgumentException("VAF_AttachmentReference_ID is mandatory."); Set_ValueNoCheck("VAF_AttachmentReference_ID", VAF_AttachmentReference_ID); }/** Get VAF_AttachmentReference_ID.
@return VAF_AttachmentReference_ID */
        public int GetVAF_AttachmentReference_ID() { Object ii = Get_Value("VAF_AttachmentReference_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }
    }
}
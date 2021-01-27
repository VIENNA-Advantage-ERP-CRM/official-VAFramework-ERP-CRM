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
    using System.Data;/** Generated Model for VAB_RFQ_Subject
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAB_RFQ_Subject : PO
    {
        public X_VAB_RFQ_Subject(Context ctx, int VAB_RFQ_Subject_ID, Trx trxName)
            : base(ctx, VAB_RFQ_Subject_ID, trxName)
        {/** if (VAB_RFQ_Subject_ID == 0){SetVAB_RFQ_Subject_ID (0);SetIsSelfService (false);SetName (null);} */
        }
        public X_VAB_RFQ_Subject(Ctx ctx, int VAB_RFQ_Subject_ID, Trx trxName)
            : base(ctx, VAB_RFQ_Subject_ID, trxName)
        {/** if (VAB_RFQ_Subject_ID == 0){SetVAB_RFQ_Subject_ID (0);SetIsSelfService (false);SetName (null);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_RFQ_Subject(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_RFQ_Subject(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_RFQ_Subject(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAB_RFQ_Subject() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27867952784146L;/** Last Updated Timestamp 4/2/2020 5:07:47 PM */
        public static long updatedMS = 1585827467357L;/** VAF_TableView_ID=671 */
        public static int Table_ID; // =671;
        /** TableName=VAB_RFQ_Subject */
        public static String Table_Name = "VAB_RFQ_Subject";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VAB_RFQ_Subject[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Print Format.
@param VAF_Print_Rpt_Layout_ID Data Print Format */
        public void SetVAF_Print_Rpt_Layout_ID(int VAF_Print_Rpt_Layout_ID)
        {
            if (VAF_Print_Rpt_Layout_ID <= 0) Set_Value("VAF_Print_Rpt_Layout_ID", null);
            else
                Set_Value("VAF_Print_Rpt_Layout_ID", VAF_Print_Rpt_Layout_ID);
        }/** Get Print Format.
@return Data Print Format */
        public int GetVAF_Print_Rpt_Layout_ID() { Object ii = Get_Value("VAF_Print_Rpt_Layout_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set RfQ Topic.
@param VAB_RFQ_Subject_ID Topic for Request for Quotations */
        public void SetVAB_RFQ_Subject_ID(int VAB_RFQ_Subject_ID) { if (VAB_RFQ_Subject_ID < 1) throw new ArgumentException("VAB_RFQ_Subject_ID is mandatory."); Set_ValueNoCheck("VAB_RFQ_Subject_ID", VAB_RFQ_Subject_ID); }/** Get RfQ Topic.
@return Topic for Request for Quotations */
        public int GetVAB_RFQ_Subject_ID() { Object ii = Get_Value("VAB_RFQ_Subject_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description) { if (Description != null && Description.Length > 255) { log.Warning("Length > 255 - truncated"); Description = Description.Substring(0, 255); } Set_Value("Description", Description); }/** Get Description.
@return Optional short description of the record */
        public String GetDescription() { return (String)Get_Value("Description"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_ValueNoCheck("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Self-Service.
@param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
        public void SetIsSelfService(Boolean IsSelfService) { Set_Value("IsSelfService", IsSelfService); }/** Get Self-Service.
@return This is a Self-Service entry or this entry can be changed via Self-Service */
        public Boolean IsSelfService() { Object oo = Get_Value("IsSelfService"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Name.
@param Name Alphanumeric identifier of the entity */
        public void SetName(String Name) { if (Name == null) throw new ArgumentException("Name is mandatory."); if (Name.Length > 60) { log.Warning("Length > 60 - truncated"); Name = Name.Substring(0, 60); } Set_Value("Name", Name); }/** Get Name.
@return Alphanumeric identifier of the entity */
        public String GetName() { return (String)Get_Value("Name"); }/** Get Record ID/ColumnName
@return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair() { return new KeyNamePair(Get_ID(), GetName()); }/** Set Mail Template.
@param R_MailText_ID Text templates for mailings */
        public void SetR_MailText_ID(int R_MailText_ID)
        {
            if (R_MailText_ID <= 0) Set_Value("R_MailText_ID", null);
            else
                Set_Value("R_MailText_ID", R_MailText_ID);
        }/** Get Mail Template.
@return Text templates for mailings */
        public int GetR_MailText_ID() { Object ii = Get_Value("R_MailText_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }

}

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
    using System.Data;/** Generated Model for VAB_RecurringRun
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAB_RecurringRun : PO
    {
        public X_VAB_RecurringRun(Context ctx, int VAB_RecurringRun_ID, Trx trxName)
            : base(ctx, VAB_RecurringRun_ID, trxName)
        {/** if (VAB_RecurringRun_ID == 0){SetVAB_Recurring_ID (0);SetVAB_RecurringRun_ID (0);} */
        }
        public X_VAB_RecurringRun(Ctx ctx, int VAB_RecurringRun_ID, Trx trxName)
            : base(ctx, VAB_RecurringRun_ID, trxName)
        {/** if (VAB_RecurringRun_ID == 0){SetVAB_Recurring_ID (0);SetVAB_RecurringRun_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_RecurringRun(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_RecurringRun(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_RecurringRun(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAB_RecurringRun() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27763907230203L;/** Last Updated Timestamp 12/15/2016 11:35:13 AM */
        public static long updatedMS = 1481781913414L;/** VAF_TableView_ID=573 */
        public static int Table_ID; // =573;
        /** TableName=VAB_RecurringRun */
        public static String Table_Name = "VAB_RecurringRun";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VAB_RecurringRun[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Invoice.
@param VAB_Invoice_ID Invoice Identifier */
        public void SetVAB_Invoice_ID(int VAB_Invoice_ID)
        {
            if (VAB_Invoice_ID <= 0) Set_ValueNoCheck("VAB_Invoice_ID", null);
            else
                Set_ValueNoCheck("VAB_Invoice_ID", VAB_Invoice_ID);
        }/** Get Invoice.
@return Invoice Identifier */
        public int GetVAB_Invoice_ID() { Object ii = Get_Value("VAB_Invoice_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order.
@param VAB_Order_ID Sales Order */
        public void SetVAB_Order_ID(int VAB_Order_ID)
        {
            if (VAB_Order_ID <= 0) Set_ValueNoCheck("VAB_Order_ID", null);
            else
                Set_ValueNoCheck("VAB_Order_ID", VAB_Order_ID);
        }/** Get Order.
@return Sales Order */
        public int GetVAB_Order_ID() { Object ii = Get_Value("VAB_Order_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Payment.
@param VAB_Payment_ID Payment identifier */
        public void SetVAB_Payment_ID(int VAB_Payment_ID)
        {
            if (VAB_Payment_ID <= 0) Set_ValueNoCheck("VAB_Payment_ID", null);
            else
                Set_ValueNoCheck("VAB_Payment_ID", VAB_Payment_ID);
        }/** Get Payment.
@return Payment identifier */
        public int GetVAB_Payment_ID() { Object ii = Get_Value("VAB_Payment_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Opportunity.
@param VAB_Project_ID Business Opportunity */
        public void SetVAB_Project_ID(int VAB_Project_ID)
        {
            if (VAB_Project_ID <= 0) Set_ValueNoCheck("VAB_Project_ID", null);
            else
                Set_ValueNoCheck("VAB_Project_ID", VAB_Project_ID);
        }/** Get Opportunity.
@return Business Opportunity */
        public int GetVAB_Project_ID() { Object ii = Get_Value("VAB_Project_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Recurring.
@param VAB_Recurring_ID Recurring Document */
        public void SetVAB_Recurring_ID(int VAB_Recurring_ID) { if (VAB_Recurring_ID < 1) throw new ArgumentException("VAB_Recurring_ID is mandatory."); Set_ValueNoCheck("VAB_Recurring_ID", VAB_Recurring_ID); }/** Get Recurring.
@return Recurring Document */
        public int GetVAB_Recurring_ID() { Object ii = Get_Value("VAB_Recurring_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Recurring Run.
@param VAB_RecurringRun_ID Recurring Document Run */
        public void SetVAB_RecurringRun_ID(int VAB_RecurringRun_ID) { if (VAB_RecurringRun_ID < 1) throw new ArgumentException("VAB_RecurringRun_ID is mandatory."); Set_ValueNoCheck("VAB_RecurringRun_ID", VAB_RecurringRun_ID); }/** Get Recurring Run.
@return Recurring Document Run */
        public int GetVAB_RecurringRun_ID() { Object ii = Get_Value("VAB_RecurringRun_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Document Date.
@param DateDoc Date of the Document */
        public void SetDateDoc(DateTime? DateDoc) { Set_Value("DateDoc", (DateTime?)DateDoc); }/** Get Document Date.
@return Date of the Document */
        public DateTime? GetDateDoc() { return (DateTime?)Get_Value("DateDoc"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_ValueNoCheck("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Journal Batch.
@param VAGL_BatchJRNL_ID General Ledger Journal Batch */
        public void SetVAGL_BatchJRNL_ID(int VAGL_BatchJRNL_ID)
        {
            if (VAGL_BatchJRNL_ID <= 0) Set_ValueNoCheck("VAGL_BatchJRNL_ID", null);
            else
                Set_ValueNoCheck("VAGL_BatchJRNL_ID", VAGL_BatchJRNL_ID);
        }/** Get Journal Batch.
@return General Ledger Journal Batch */
        public int GetVAGL_BatchJRNL_ID() { Object ii = Get_Value("VAGL_BatchJRNL_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Journal.
@param VAGL_JRNL_ID General Ledger Journal */
        public void SetVAGL_JRNL_ID(int VAGL_JRNL_ID)
        {
            if (VAGL_JRNL_ID <= 0) Set_Value("VAGL_JRNL_ID", null);
            else
                Set_Value("VAGL_JRNL_ID", VAGL_JRNL_ID);
        }/** Get Journal.
@return General Ledger Journal */
        public int GetVAGL_JRNL_ID() { Object ii = Get_Value("VAGL_JRNL_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }
}
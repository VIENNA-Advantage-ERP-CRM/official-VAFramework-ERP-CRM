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
    using System.Data;/** Generated Model for C_Recurring_Run
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_Recurring_Run : PO
    {
        public X_C_Recurring_Run(Context ctx, int C_Recurring_Run_ID, Trx trxName)
            : base(ctx, C_Recurring_Run_ID, trxName)
        {/** if (C_Recurring_Run_ID == 0){SetC_Recurring_ID (0);SetC_Recurring_Run_ID (0);} */
        }
        public X_C_Recurring_Run(Ctx ctx, int C_Recurring_Run_ID, Trx trxName)
            : base(ctx, C_Recurring_Run_ID, trxName)
        {/** if (C_Recurring_Run_ID == 0){SetC_Recurring_ID (0);SetC_Recurring_Run_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_Recurring_Run(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_Recurring_Run(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_Recurring_Run(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_Recurring_Run() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27763907230203L;/** Last Updated Timestamp 12/15/2016 11:35:13 AM */
        public static long updatedMS = 1481781913414L;/** AD_Table_ID=573 */
        public static int Table_ID; // =573;
        /** TableName=C_Recurring_Run */
        public static String Table_Name = "C_Recurring_Run";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_Recurring_Run[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
        public void SetC_Invoice_ID(int C_Invoice_ID)
        {
            if (C_Invoice_ID <= 0) Set_ValueNoCheck("C_Invoice_ID", null);
            else
                Set_ValueNoCheck("C_Invoice_ID", C_Invoice_ID);
        }/** Get Invoice.
@return Invoice Identifier */
        public int GetC_Invoice_ID() { Object ii = Get_Value("C_Invoice_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order.
@param C_Order_ID Sales Order */
        public void SetC_Order_ID(int C_Order_ID)
        {
            if (C_Order_ID <= 0) Set_ValueNoCheck("C_Order_ID", null);
            else
                Set_ValueNoCheck("C_Order_ID", C_Order_ID);
        }/** Get Order.
@return Sales Order */
        public int GetC_Order_ID() { Object ii = Get_Value("C_Order_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Payment.
@param C_Payment_ID Payment identifier */
        public void SetC_Payment_ID(int C_Payment_ID)
        {
            if (C_Payment_ID <= 0) Set_ValueNoCheck("C_Payment_ID", null);
            else
                Set_ValueNoCheck("C_Payment_ID", C_Payment_ID);
        }/** Get Payment.
@return Payment identifier */
        public int GetC_Payment_ID() { Object ii = Get_Value("C_Payment_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Opportunity.
@param C_Project_ID Business Opportunity */
        public void SetC_Project_ID(int C_Project_ID)
        {
            if (C_Project_ID <= 0) Set_ValueNoCheck("C_Project_ID", null);
            else
                Set_ValueNoCheck("C_Project_ID", C_Project_ID);
        }/** Get Opportunity.
@return Business Opportunity */
        public int GetC_Project_ID() { Object ii = Get_Value("C_Project_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Recurring.
@param C_Recurring_ID Recurring Document */
        public void SetC_Recurring_ID(int C_Recurring_ID) { if (C_Recurring_ID < 1) throw new ArgumentException("C_Recurring_ID is mandatory."); Set_ValueNoCheck("C_Recurring_ID", C_Recurring_ID); }/** Get Recurring.
@return Recurring Document */
        public int GetC_Recurring_ID() { Object ii = Get_Value("C_Recurring_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Recurring Run.
@param C_Recurring_Run_ID Recurring Document Run */
        public void SetC_Recurring_Run_ID(int C_Recurring_Run_ID) { if (C_Recurring_Run_ID < 1) throw new ArgumentException("C_Recurring_Run_ID is mandatory."); Set_ValueNoCheck("C_Recurring_Run_ID", C_Recurring_Run_ID); }/** Get Recurring Run.
@return Recurring Document Run */
        public int GetC_Recurring_Run_ID() { Object ii = Get_Value("C_Recurring_Run_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Document Date.
@param DateDoc Date of the Document */
        public void SetDateDoc(DateTime? DateDoc) { Set_Value("DateDoc", (DateTime?)DateDoc); }/** Get Document Date.
@return Date of the Document */
        public DateTime? GetDateDoc() { return (DateTime?)Get_Value("DateDoc"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_ValueNoCheck("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Journal Batch.
@param GL_JournalBatch_ID General Ledger Journal Batch */
        public void SetGL_JournalBatch_ID(int GL_JournalBatch_ID)
        {
            if (GL_JournalBatch_ID <= 0) Set_ValueNoCheck("GL_JournalBatch_ID", null);
            else
                Set_ValueNoCheck("GL_JournalBatch_ID", GL_JournalBatch_ID);
        }/** Get Journal Batch.
@return General Ledger Journal Batch */
        public int GetGL_JournalBatch_ID() { Object ii = Get_Value("GL_JournalBatch_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Journal.
@param GL_Journal_ID General Ledger Journal */
        public void SetGL_Journal_ID(int GL_Journal_ID)
        {
            if (GL_Journal_ID <= 0) Set_Value("GL_Journal_ID", null);
            else
                Set_Value("GL_Journal_ID", GL_Journal_ID);
        }/** Get Journal.
@return General Ledger Journal */
        public int GetGL_Journal_ID() { Object ii = Get_Value("GL_Journal_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }
}
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
    using System.Data;/** Generated Model for VAF_DefaultCardView
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_DefaultCardView : PO
    {
        public X_VAF_DefaultCardView(Context ctx, int VAF_DefaultCardView_ID, Trx trxName)
            : base(ctx, VAF_DefaultCardView_ID, trxName)
        {/** if (VAF_DefaultCardView_ID == 0){SetVAF_DefaultCardView_ID (0);} */
        }
        public X_VAF_DefaultCardView(Ctx ctx, int VAF_DefaultCardView_ID, Trx trxName)
            : base(ctx, VAF_DefaultCardView_ID, trxName)
        {/** if (VAF_DefaultCardView_ID == 0){SetVAF_DefaultCardView_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_DefaultCardView(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_DefaultCardView(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_DefaultCardView(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAF_DefaultCardView() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27748096393125L;/** Last Updated Timestamp 6/15/2016 11:41:16 AM */
        public static long updatedMS = 1465971076336L;/** VAF_TableView_ID=1000597 */
        public static int Table_ID; // =1000597;
        /** TableName=VAF_DefaultCardView */
        public static String Table_Name = "VAF_DefaultCardView";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(7);/** AccessLevel
@return 7 - System - Client - Org 
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VAF_DefaultCardView[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set VAF_CardView_ID.
@param VAF_CardView_ID VAF_CardView_ID */
        public void SetVAF_CardView_ID(int VAF_CardView_ID)
        {
            if (VAF_CardView_ID <= 0) Set_Value("VAF_CardView_ID", null);
            else
                Set_Value("VAF_CardView_ID", VAF_CardView_ID);
        }/** Get VAF_CardView_ID.
@return VAF_CardView_ID */
        public int GetVAF_CardView_ID() { Object ii = Get_Value("VAF_CardView_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set VAF_DefaultCardView_ID.
@param VAF_DefaultCardView_ID VAF_DefaultCardView_ID */
        public void SetVAF_DefaultCardView_ID(int VAF_DefaultCardView_ID) { if (VAF_DefaultCardView_ID < 1) throw new ArgumentException("VAF_DefaultCardView_ID is mandatory."); Set_ValueNoCheck("VAF_DefaultCardView_ID", VAF_DefaultCardView_ID); }/** Get VAF_DefaultCardView_ID.
@return VAF_DefaultCardView_ID */
        public int GetVAF_DefaultCardView_ID() { Object ii = Get_Value("VAF_DefaultCardView_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Customer/Prospect Contact. */
        public void SetVAF_UserContact_ID(int VAF_UserContact_ID)
        {
            if (VAF_UserContact_ID <= 0) Set_Value("VAF_UserContact_ID", null);
            else
                Set_Value("VAF_UserContact_ID", VAF_UserContact_ID);
        }/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
        public int GetVAF_UserContact_ID() { Object ii = Get_Value("VAF_UserContact_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }

        /** Set Tab.
@param VAF_Tab_ID Tab within a Window */
        public void SetVAF_Tab_ID(int VAF_Tab_ID)
        {
            if (VAF_Tab_ID <= 0) Set_Value("VAF_Tab_ID", null);
            else
                Set_Value("VAF_Tab_ID", VAF_Tab_ID);
        }/** Get Tab.
@return Tab within a Window */
        public int GetVAF_Tab_ID() { Object ii = Get_Value("VAF_Tab_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

    }
}
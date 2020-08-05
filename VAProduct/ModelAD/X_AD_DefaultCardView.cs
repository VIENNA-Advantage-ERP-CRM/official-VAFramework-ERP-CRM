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
    using System.Data;/** Generated Model for AD_DefaultCardView
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_DefaultCardView : PO
    {
        public X_AD_DefaultCardView(Context ctx, int AD_DefaultCardView_ID, Trx trxName)
            : base(ctx, AD_DefaultCardView_ID, trxName)
        {/** if (AD_DefaultCardView_ID == 0){SetAD_DefaultCardView_ID (0);} */
        }
        public X_AD_DefaultCardView(Ctx ctx, int AD_DefaultCardView_ID, Trx trxName)
            : base(ctx, AD_DefaultCardView_ID, trxName)
        {/** if (AD_DefaultCardView_ID == 0){SetAD_DefaultCardView_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_DefaultCardView(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_DefaultCardView(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_DefaultCardView(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_AD_DefaultCardView() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27748096393125L;/** Last Updated Timestamp 6/15/2016 11:41:16 AM */
        public static long updatedMS = 1465971076336L;/** AD_Table_ID=1000597 */
        public static int Table_ID; // =1000597;
        /** TableName=AD_DefaultCardView */
        public static String Table_Name = "AD_DefaultCardView";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_AD_DefaultCardView[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set AD_CardView_ID.
@param AD_CardView_ID AD_CardView_ID */
        public void SetAD_CardView_ID(int AD_CardView_ID)
        {
            if (AD_CardView_ID <= 0) Set_Value("AD_CardView_ID", null);
            else
                Set_Value("AD_CardView_ID", AD_CardView_ID);
        }/** Get AD_CardView_ID.
@return AD_CardView_ID */
        public int GetAD_CardView_ID() { Object ii = Get_Value("AD_CardView_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set AD_DefaultCardView_ID.
@param AD_DefaultCardView_ID AD_DefaultCardView_ID */
        public void SetAD_DefaultCardView_ID(int AD_DefaultCardView_ID) { if (AD_DefaultCardView_ID < 1) throw new ArgumentException("AD_DefaultCardView_ID is mandatory."); Set_ValueNoCheck("AD_DefaultCardView_ID", AD_DefaultCardView_ID); }/** Get AD_DefaultCardView_ID.
@return AD_DefaultCardView_ID */
        public int GetAD_DefaultCardView_ID() { Object ii = Get_Value("AD_DefaultCardView_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID <= 0) Set_Value("AD_User_ID", null);
            else
                Set_Value("AD_User_ID", AD_User_ID);
        }/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
        public int GetAD_User_ID() { Object ii = Get_Value("AD_User_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }

        /** Set Tab.
@param AD_Tab_ID Tab within a Window */
        public void SetAD_Tab_ID(int AD_Tab_ID)
        {
            if (AD_Tab_ID <= 0) Set_Value("AD_Tab_ID", null);
            else
                Set_Value("AD_Tab_ID", AD_Tab_ID);
        }/** Get Tab.
@return Tab within a Window */
        public int GetAD_Tab_ID() { Object ii = Get_Value("AD_Tab_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

    }
}
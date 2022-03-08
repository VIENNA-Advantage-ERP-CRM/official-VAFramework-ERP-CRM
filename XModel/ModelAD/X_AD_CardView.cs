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
    using System.Data;/** Generated Model for AD_CardView
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_CardView : PO
    {
        public X_AD_CardView(Context ctx, int AD_CardView_ID, Trx trxName)
            : base(ctx, AD_CardView_ID, trxName)
        {/** if (AD_CardView_ID == 0){SetAD_CardView_ID (0);} */
        }
        public X_AD_CardView(Ctx ctx, int AD_CardView_ID, Trx trxName)
            : base(ctx, AD_CardView_ID, trxName)
        {/** if (AD_CardView_ID == 0){SetAD_CardView_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_CardView(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_CardView(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_CardView(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_AD_CardView() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27719840933480L;/** Last Updated Timestamp 7/24/2015 10:56:56 AM */
        public static long updatedMS = 1437715616691L;/** AD_Table_ID=1000563 */
        public static int Table_ID; // =1000563;
        /** TableName=AD_CardView */
        public static String Table_Name = "AD_CardView";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(6);/** AccessLevel
@return 6 - System - Client 
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_AD_CardView[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set AD_CardView_ID.
@param AD_CardView_ID AD_CardView_ID */
        public void SetAD_CardView_ID(int AD_CardView_ID) { if (AD_CardView_ID < 1) throw new ArgumentException("AD_CardView_ID is mandatory."); Set_ValueNoCheck("AD_CardView_ID", AD_CardView_ID); }/** Get AD_CardView_ID.
@return AD_CardView_ID */
        public int GetAD_CardView_ID() { Object ii = Get_Value("AD_CardView_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Field.
@param AD_Field_ID Field on a tab in a window */
        public void SetAD_Field_ID(int AD_Field_ID)
        {
            if (AD_Field_ID <= 0) Set_Value("AD_Field_ID", null);
            else
                Set_Value("AD_Field_ID", AD_Field_ID);
        }/** Get Field.
@return Field on a tab in a window */
        public int GetAD_Field_ID() { Object ii = Get_Value("AD_Field_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Tab.
@param AD_Tab_ID Tab within a Window */
        public void SetAD_Tab_ID(int AD_Tab_ID)
        {
            if (AD_Tab_ID <= 0) Set_Value("AD_Tab_ID", null);
            else
                Set_Value("AD_Tab_ID", AD_Tab_ID);
        }/** Get Tab.
@return Tab within a Window */
        public int GetAD_Tab_ID() { Object ii = Get_Value("AD_Tab_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID <= 0) Set_Value("AD_User_ID", null);
            else
                Set_Value("AD_User_ID", AD_User_ID);
        }/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
        public int GetAD_User_ID() { Object ii = Get_Value("AD_User_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Window.
@param AD_Window_ID Data entry or display window */
        public void SetAD_Window_ID(int AD_Window_ID)
        {
            if (AD_Window_ID <= 0) Set_Value("AD_Window_ID", null);
            else
                Set_Value("AD_Window_ID", AD_Window_ID);
        }/** Get Window.
@return Data entry or display window */
        public int GetAD_Window_ID() { Object ii = Get_Value("AD_Window_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Name.
@param Name Alphanumeric identifier of the entity */
        public void SetName(String Name) { if (Name != null && Name.Length > 50) { log.Warning("Length > 50 - truncated"); Name = Name.Substring(0, 50); } Set_Value("Name", Name); }/** Get Name.
@return Alphanumeric identifier of the entity */
        public String GetName() { return (String)Get_Value("Name"); }
    }
}
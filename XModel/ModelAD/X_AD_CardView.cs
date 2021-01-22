namespace ViennaAdvantage.Model
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
    using System.Data;/** Generated Model for VAF_CardView
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_CardView : PO
    {
        public X_VAF_CardView(Context ctx, int VAF_CardView_ID, Trx trxName)
            : base(ctx, VAF_CardView_ID, trxName)
        {/** if (VAF_CardView_ID == 0){SetVAF_CardView_ID (0);} */
        }
        public X_VAF_CardView(Ctx ctx, int VAF_CardView_ID, Trx trxName)
            : base(ctx, VAF_CardView_ID, trxName)
        {/** if (VAF_CardView_ID == 0){SetVAF_CardView_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_CardView(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_CardView(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_CardView(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAF_CardView() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27719840933480L;/** Last Updated Timestamp 7/24/2015 10:56:56 AM */
        public static long updatedMS = 1437715616691L;/** VAF_TableView_ID=1000563 */
        public static int Table_ID; // =1000563;
        /** TableName=VAF_CardView */
        public static String Table_Name = "VAF_CardView";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VAF_CardView[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set VAF_CardView_ID.
@param VAF_CardView_ID VAF_CardView_ID */
        public void SetVAF_CardView_ID(int VAF_CardView_ID) { if (VAF_CardView_ID < 1) throw new ArgumentException("VAF_CardView_ID is mandatory."); Set_ValueNoCheck("VAF_CardView_ID", VAF_CardView_ID); }/** Get VAF_CardView_ID.
@return VAF_CardView_ID */
        public int GetVAF_CardView_ID() { Object ii = Get_Value("VAF_CardView_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Field.
@param VAF_Field_ID Field on a tab in a window */
        public void SetVAF_Field_ID(int VAF_Field_ID)
        {
            if (VAF_Field_ID <= 0) Set_Value("VAF_Field_ID", null);
            else
                Set_Value("VAF_Field_ID", VAF_Field_ID);
        }/** Get Field.
@return Field on a tab in a window */
        public int GetVAF_Field_ID() { Object ii = Get_Value("VAF_Field_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Tab.
@param VAF_Tab_ID Tab within a Window */
        public void SetVAF_Tab_ID(int VAF_Tab_ID)
        {
            if (VAF_Tab_ID <= 0) Set_Value("VAF_Tab_ID", null);
            else
                Set_Value("VAF_Tab_ID", VAF_Tab_ID);
        }/** Get Tab.
@return Tab within a Window */
        public int GetVAF_Tab_ID() { Object ii = Get_Value("VAF_Tab_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Customer/Prospect Contact. */
        public void SetVAF_UserContact_ID(int VAF_UserContact_ID)
        {
            if (VAF_UserContact_ID <= 0) Set_Value("VAF_UserContact_ID", null);
            else
                Set_Value("VAF_UserContact_ID", VAF_UserContact_ID);
        }/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
        public int GetVAF_UserContact_ID() { Object ii = Get_Value("VAF_UserContact_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Window.
@param VAF_Screen_ID Data entry or display window */
        public void SetVAF_Screen_ID(int VAF_Screen_ID)
        {
            if (VAF_Screen_ID <= 0) Set_Value("VAF_Screen_ID", null);
            else
                Set_Value("VAF_Screen_ID", VAF_Screen_ID);
        }/** Get Window.
@return Data entry or display window */
        public int GetVAF_Screen_ID() { Object ii = Get_Value("VAF_Screen_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Export.
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
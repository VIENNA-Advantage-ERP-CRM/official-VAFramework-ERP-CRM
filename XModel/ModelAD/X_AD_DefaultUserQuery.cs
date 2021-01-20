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
    using System.Data;/** Generated Model for VAF_DefaultUserQuery
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_DefaultUserQuery : PO
    {
        public X_VAF_DefaultUserQuery(Context ctx, int VAF_DefaultUserQuery_ID, Trx trxName)
            : base(ctx, VAF_DefaultUserQuery_ID, trxName)
        {/** if (VAF_DefaultUserQuery_ID == 0){SetVAF_DefaultUserQuery_ID (0);} */
        }
        public X_VAF_DefaultUserQuery(Ctx ctx, int VAF_DefaultUserQuery_ID, Trx trxName)
            : base(ctx, VAF_DefaultUserQuery_ID, trxName)
        {/** if (VAF_DefaultUserQuery_ID == 0){SetVAF_DefaultUserQuery_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_DefaultUserQuery(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_DefaultUserQuery(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_DefaultUserQuery(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAF_DefaultUserQuery() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27746910526019L;/** Last Updated Timestamp 6/1/2016 6:16:49 PM */
        public static long updatedMS = 1464785209230L;/** VAF_TableView_ID=1000595 */
        public static int Table_ID; // =1000595;
        /** TableName=VAF_DefaultUserQuery */
        public static String Table_Name = "VAF_DefaultUserQuery";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VAF_DefaultUserQuery[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set VAF_DefaultUserQuery_ID.
@param VAF_DefaultUserQuery_ID VAF_DefaultUserQuery_ID */
        public void SetVAF_DefaultUserQuery_ID(int VAF_DefaultUserQuery_ID) { if (VAF_DefaultUserQuery_ID < 1) throw new ArgumentException("VAF_DefaultUserQuery_ID is mandatory."); Set_ValueNoCheck("VAF_DefaultUserQuery_ID", VAF_DefaultUserQuery_ID); }/** Get VAF_DefaultUserQuery_ID.
@return VAF_DefaultUserQuery_ID */
        public int GetVAF_DefaultUserQuery_ID() { Object ii = Get_Value("VAF_DefaultUserQuery_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Tab.
@param VAF_Tab_ID Tab within a Window */
        public void SetVAF_Tab_ID(int VAF_Tab_ID)
        {
            if (VAF_Tab_ID <= 0) Set_Value("VAF_Tab_ID", null);
            else
                Set_Value("VAF_Tab_ID", VAF_Tab_ID);
        }/** Get Tab.
@return Tab within a Window */
        public int GetVAF_Tab_ID() { Object ii = Get_Value("VAF_Tab_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Table.
@param VAF_TableView_ID Database Table information */
        public void SetVAF_TableView_ID(int VAF_TableView_ID)
        {
            if (VAF_TableView_ID <= 0) Set_Value("VAF_TableView_ID", null);
            else
                Set_Value("VAF_TableView_ID", VAF_TableView_ID);
        }/** Get Table.
@return Database Table information */
        public int GetVAF_TableView_ID() { Object ii = Get_Value("VAF_TableView_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set User Query.
@param AD_UserQuery_ID Saved User Query */
        public void SetAD_UserQuery_ID(int AD_UserQuery_ID)
        {
            if (AD_UserQuery_ID <= 0) Set_Value("AD_UserQuery_ID", null);
            else
                Set_Value("AD_UserQuery_ID", AD_UserQuery_ID);
        }/** Get User Query.
@return Saved User Query */
        public int GetAD_UserQuery_ID() { Object ii = Get_Value("AD_UserQuery_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set User/Contact.
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
    }
}
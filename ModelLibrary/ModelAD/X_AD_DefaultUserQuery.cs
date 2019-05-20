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
    using System.Data;/** Generated Model for AD_DefaultUserQuery
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_DefaultUserQuery : PO
    {
        public X_AD_DefaultUserQuery(Context ctx, int AD_DefaultUserQuery_ID, Trx trxName)
            : base(ctx, AD_DefaultUserQuery_ID, trxName)
        {/** if (AD_DefaultUserQuery_ID == 0){SetAD_DefaultUserQuery_ID (0);} */
        }
        public X_AD_DefaultUserQuery(Ctx ctx, int AD_DefaultUserQuery_ID, Trx trxName)
            : base(ctx, AD_DefaultUserQuery_ID, trxName)
        {/** if (AD_DefaultUserQuery_ID == 0){SetAD_DefaultUserQuery_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_DefaultUserQuery(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_DefaultUserQuery(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_DefaultUserQuery(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_AD_DefaultUserQuery() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27746910526019L;/** Last Updated Timestamp 6/1/2016 6:16:49 PM */
        public static long updatedMS = 1464785209230L;/** AD_Table_ID=1000595 */
        public static int Table_ID; // =1000595;
        /** TableName=AD_DefaultUserQuery */
        public static String Table_Name = "AD_DefaultUserQuery";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_AD_DefaultUserQuery[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set AD_DefaultUserQuery_ID.
@param AD_DefaultUserQuery_ID AD_DefaultUserQuery_ID */
        public void SetAD_DefaultUserQuery_ID(int AD_DefaultUserQuery_ID) { if (AD_DefaultUserQuery_ID < 1) throw new ArgumentException("AD_DefaultUserQuery_ID is mandatory."); Set_ValueNoCheck("AD_DefaultUserQuery_ID", AD_DefaultUserQuery_ID); }/** Get AD_DefaultUserQuery_ID.
@return AD_DefaultUserQuery_ID */
        public int GetAD_DefaultUserQuery_ID() { Object ii = Get_Value("AD_DefaultUserQuery_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Tab.
@param AD_Tab_ID Tab within a Window */
        public void SetAD_Tab_ID(int AD_Tab_ID)
        {
            if (AD_Tab_ID <= 0) Set_Value("AD_Tab_ID", null);
            else
                Set_Value("AD_Tab_ID", AD_Tab_ID);
        }/** Get Tab.
@return Tab within a Window */
        public int GetAD_Tab_ID() { Object ii = Get_Value("AD_Tab_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Table.
@param AD_Table_ID Database Table information */
        public void SetAD_Table_ID(int AD_Table_ID)
        {
            if (AD_Table_ID <= 0) Set_Value("AD_Table_ID", null);
            else
                Set_Value("AD_Table_ID", AD_Table_ID);
        }/** Get Table.
@return Database Table information */
        public int GetAD_Table_ID() { Object ii = Get_Value("AD_Table_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set User Query.
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
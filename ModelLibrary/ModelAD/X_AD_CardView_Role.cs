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
    using System.Data;/** Generated Model for AD_CardView_Role
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_CardView_Role : PO
    {
        public X_AD_CardView_Role(Context ctx, int AD_CardView_Role_ID, Trx trxName)
            : base(ctx, AD_CardView_Role_ID, trxName)
        {/** if (AD_CardView_Role_ID == 0){SetAD_CardView_Role_ID (0);} */
        }
        public X_AD_CardView_Role(Ctx ctx, int AD_CardView_Role_ID, Trx trxName)
            : base(ctx, AD_CardView_Role_ID, trxName)
        {/** if (AD_CardView_Role_ID == 0){SetAD_CardView_Role_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_CardView_Role(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_CardView_Role(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_CardView_Role(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_AD_CardView_Role() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27720368784123L;/** Last Updated Timestamp 7/30/2015 1:34:27 PM */
        public static long updatedMS = 1438243467334L;/** AD_Table_ID=1000575 */
        public static int Table_ID; // =1000575;
        /** TableName=AD_CardView_Role */
        public static String Table_Name = "AD_CardView_Role";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_AD_CardView_Role[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set AD_CardView_ID.
@param AD_CardView_ID AD_CardView_ID */
        public void SetAD_CardView_ID(int AD_CardView_ID)
        {
            if (AD_CardView_ID <= 0) Set_Value("AD_CardView_ID", null);
            else
                Set_Value("AD_CardView_ID", AD_CardView_ID);
        }/** Get AD_CardView_ID.
@return AD_CardView_ID */
        public int GetAD_CardView_ID() { Object ii = Get_Value("AD_CardView_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set AD_CardView_Role_ID.
@param AD_CardView_Role_ID AD_CardView_Role_ID */
        public void SetAD_CardView_Role_ID(int AD_CardView_Role_ID) { if (AD_CardView_Role_ID < 1) throw new ArgumentException("AD_CardView_Role_ID is mandatory."); Set_ValueNoCheck("AD_CardView_Role_ID", AD_CardView_Role_ID); }/** Get AD_CardView_Role_ID.
@return AD_CardView_Role_ID */
        public int GetAD_CardView_Role_ID() { Object ii = Get_Value("AD_CardView_Role_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Role.
@param AD_Role_ID Responsibility Role */
        public void SetAD_Role_ID(int AD_Role_ID)
        {
            if (AD_Role_ID <= 0) Set_Value("AD_Role_ID", null);
            else
                Set_Value("AD_Role_ID", AD_Role_ID);
        }/** Get Role.
@return Responsibility Role */
        public int GetAD_Role_ID() { Object ii = Get_Value("AD_Role_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }
    }
}
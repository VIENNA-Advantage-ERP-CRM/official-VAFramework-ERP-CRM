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
    using System.Data;/** Generated Model for VAF_CardView_Role
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_CardView_Role : PO
    {
        public X_VAF_CardView_Role(Context ctx, int VAF_CardView_Role_ID, Trx trxName)
            : base(ctx, VAF_CardView_Role_ID, trxName)
        {/** if (VAF_CardView_Role_ID == 0){SetVAF_CardView_Role_ID (0);} */
        }
        public X_VAF_CardView_Role(Ctx ctx, int VAF_CardView_Role_ID, Trx trxName)
            : base(ctx, VAF_CardView_Role_ID, trxName)
        {/** if (VAF_CardView_Role_ID == 0){SetVAF_CardView_Role_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_CardView_Role(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_CardView_Role(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAF_CardView_Role(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAF_CardView_Role() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27720368784123L;/** Last Updated Timestamp 7/30/2015 1:34:27 PM */
        public static long updatedMS = 1438243467334L;/** VAF_TableView_ID=1000575 */
        public static int Table_ID; // =1000575;
        /** TableName=VAF_CardView_Role */
        public static String Table_Name = "VAF_CardView_Role";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VAF_CardView_Role[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set VAF_CardView_ID.
@param VAF_CardView_ID VAF_CardView_ID */
        public void SetVAF_CardView_ID(int VAF_CardView_ID)
        {
            if (VAF_CardView_ID <= 0) Set_Value("VAF_CardView_ID", null);
            else
                Set_Value("VAF_CardView_ID", VAF_CardView_ID);
        }/** Get VAF_CardView_ID.
@return VAF_CardView_ID */
        public int GetVAF_CardView_ID() { Object ii = Get_Value("VAF_CardView_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set VAF_CardView_Role_ID.
@param VAF_CardView_Role_ID VAF_CardView_Role_ID */
        public void SetVAF_CardView_Role_ID(int VAF_CardView_Role_ID) { if (VAF_CardView_Role_ID < 1) throw new ArgumentException("VAF_CardView_Role_ID is mandatory."); Set_ValueNoCheck("VAF_CardView_Role_ID", VAF_CardView_Role_ID); }/** Get VAF_CardView_Role_ID.
@return VAF_CardView_Role_ID */
        public int GetVAF_CardView_Role_ID() { Object ii = Get_Value("VAF_CardView_Role_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Role.
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
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
    using System.Data;/** Generated Model for AD_CardView_Condition
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_CardView_Condition : PO
    {
        public X_AD_CardView_Condition(Context ctx, int AD_CardView_Condition_ID, Trx trxName)
            : base(ctx, AD_CardView_Condition_ID, trxName)
        {/** if (AD_CardView_Condition_ID == 0){SetAD_CardView_Condition_ID (0);} */
        }
        public X_AD_CardView_Condition(Ctx ctx, int AD_CardView_Condition_ID, Trx trxName)
            : base(ctx, AD_CardView_Condition_ID, trxName)
        {/** if (AD_CardView_Condition_ID == 0){SetAD_CardView_Condition_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_CardView_Condition(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_CardView_Condition(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_AD_CardView_Condition(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_AD_CardView_Condition() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27721072404984L;/** Last Updated Timestamp 8/7/2015 5:01:28 PM */
        public static long updatedMS = 1438947088195L;/** AD_Table_ID=1000576 */
        public static int Table_ID; // =1000576;
        /** TableName=AD_CardView_Condition */
        public static String Table_Name = "AD_CardView_Condition";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_AD_CardView_Condition[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set AD_CardView_Condition_ID.
@param AD_CardView_Condition_ID AD_CardView_Condition_ID */
        public void SetAD_CardView_Condition_ID(int AD_CardView_Condition_ID) { if (AD_CardView_Condition_ID < 1) throw new ArgumentException("AD_CardView_Condition_ID is mandatory."); Set_ValueNoCheck("AD_CardView_Condition_ID", AD_CardView_Condition_ID); }/** Get AD_CardView_Condition_ID.
@return AD_CardView_Condition_ID */
        public int GetAD_CardView_Condition_ID() { Object ii = Get_Value("AD_CardView_Condition_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set AD_CardView_ID.
@param AD_CardView_ID AD_CardView_ID */
        public void SetAD_CardView_ID(int AD_CardView_ID)
        {
            if (AD_CardView_ID <= 0) Set_Value("AD_CardView_ID", null);
            else
                Set_Value("AD_CardView_ID", AD_CardView_ID);
        }/** Get AD_CardView_ID.
@return AD_CardView_ID */
        public int GetAD_CardView_ID() { Object ii = Get_Value("AD_CardView_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Color.
@param Color Color */
        public void SetColor(String Color) { if (Color != null && Color.Length > 100) { log.Warning("Length > 100 - truncated"); Color = Color.Substring(0, 100); } Set_Value("Color", Color); }/** Get Color.
@return Color */
        public String GetColor() { return (String)Get_Value("Color"); }/** Set CondiitonValue.
@param CondiitonValue CondiitonValue */
        public void SetConditionValue(String ConditionValue) { if (ConditionValue != null && ConditionValue.Length > 200) { log.Warning("Length > 200 - truncated"); ConditionValue = ConditionValue.Substring(0, 200); } Set_Value("ConditionValue", ConditionValue); }/** Get CondiitonValue.
@return CondiitonValue */
        public String GetConditionValue() { return (String)Get_Value("ConditionValue"); }/** Set ConditionText.
@param ConditionText ConditionText */
        public void SetConditionText(String ConditionText) { if (ConditionText != null && ConditionText.Length > 200) { log.Warning("Length > 200 - truncated"); ConditionText = ConditionText.Substring(0, 200); } Set_Value("ConditionText", ConditionText); }/** Get ConditionText.
@return ConditionText */
        public String GetConditionText() { return (String)Get_Value("ConditionText"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }
    }
}
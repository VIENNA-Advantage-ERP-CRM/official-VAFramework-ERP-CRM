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
    using System.Data;/** Generated Model for M_Brand
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_Brand : PO
    {
        public X_M_Brand(Context ctx, int M_Brand_ID, Trx trxName)
            : base(ctx, M_Brand_ID, trxName)
        {/** if (M_Brand_ID == 0){SetM_Brand_ID (0);} */
        }
        public X_M_Brand(Ctx ctx, int M_Brand_ID, Trx trxName)
            : base(ctx, M_Brand_ID, trxName)
        {/** if (M_Brand_ID == 0){SetM_Brand_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_Brand(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_Brand(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_Brand(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_M_Brand() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27797191401445L;/** Last Updated Timestamp 1/4/2018 5:11:24 PM */
        public static long updatedMS = 1515066084656L;/** AD_Table_ID=1000514 */
        public static int Table_ID; // =1000514;
        /** TableName=M_Brand */
        public static String Table_Name = "M_Brand";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_M_Brand[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Image Column.
@param ImageField Image Column */
        public void SetImageField(int ImageField) { Set_Value("ImageField", ImageField); }/** Get Image Column.
@return Image Column */
        public int GetImageField() { Object ii = Get_Value("ImageField"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set M_Brand_ID.
@param M_Brand_ID M_Brand_ID */
        public void SetM_Brand_ID(int M_Brand_ID) { if (M_Brand_ID < 1) throw new ArgumentException("M_Brand_ID is mandatory."); Set_ValueNoCheck("M_Brand_ID", M_Brand_ID); }/** Get M_Brand_ID.
@return M_Brand_ID */
        public int GetM_Brand_ID() { Object ii = Get_Value("M_Brand_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Name.
@param Name Alphanumeric identifier of the entity */
        public void SetName(String Name) { if (Name != null && Name.Length > 50) { log.Warning("Length > 50 - truncated"); Name = Name.Substring(0, 50); } Set_Value("Name", Name); }/** Get Name.
@return Alphanumeric identifier of the entity */
        public String GetName() { return (String)Get_Value("Name"); }/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
        public void SetValue(String Value) { if (Value != null && Value.Length > 50) { log.Warning("Length > 50 - truncated"); Value = Value.Substring(0, 50); } Set_Value("Value", Value); }/** Get Search Key.
@return Search key for the record in the format required - must be unique */
        public String GetValue() { return (String)Get_Value("Value"); }
    }
}
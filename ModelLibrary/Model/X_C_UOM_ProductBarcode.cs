namespace VAdvantage.Model
{
    /** Generated Model - DO NOT CHANGE */
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using VAdvantage.Classes;
    using VAdvantage.Common;
    using VAdvantage.Process;
    using VAdvantage.ProcessEngine;
    using VAdvantage.Model;
    using VAdvantage.DataBase;
    using VAdvantage.SqlExec;
    using VAdvantage.Utility;
    using System.Windows.Forms;
    //using VAdvantage.Controls;
    using System.Data;
    using System.Data.SqlClient;
    using VAdvantage.Logging;
    /** Generated Model for C_UOM_ProductBarcode
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_UOM_ProductBarcode : PO
    {
        public X_C_UOM_ProductBarcode(Context ctx, int C_UOM_ProductBarcode_ID, Trx trxName)
            : base(ctx, C_UOM_ProductBarcode_ID, trxName)
        {/** if (C_UOM_ProductBarcode_ID == 0){SetC_UOM_Conversion_ID (0);SetC_UOM_ProductBarcode_ID (0);} */
        }
        public X_C_UOM_ProductBarcode(Ctx ctx, int C_UOM_ProductBarcode_ID, Trx trxName)
            : base(ctx, C_UOM_ProductBarcode_ID, trxName)
        {/** if (C_UOM_ProductBarcode_ID == 0){SetC_UOM_Conversion_ID (0);SetC_UOM_ProductBarcode_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_UOM_ProductBarcode(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_UOM_ProductBarcode(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_UOM_ProductBarcode(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_UOM_ProductBarcode() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27798405399783L;/** Last Updated Timestamp 1/18/2018 6:24:43 PM */
        public static long updatedMS = 1516280082994L;/** AD_Table_ID=1000518 */
        public static int Table_ID; // =1000518;
        /** TableName=C_UOM_ProductBarcode */
        public static String Table_Name = "C_UOM_ProductBarcode";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_UOM_ProductBarcode[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set UOM Conversion.
@param C_UOM_Conversion_ID Unit of Measure Conversion */
        public void SetC_UOM_Conversion_ID(int C_UOM_Conversion_ID) { if (C_UOM_Conversion_ID < 1) throw new ArgumentException("C_UOM_Conversion_ID is mandatory."); Set_ValueNoCheck("C_UOM_Conversion_ID", C_UOM_Conversion_ID); }/** Get UOM Conversion.
@return Unit of Measure Conversion */
        public int GetC_UOM_Conversion_ID() { Object ii = Get_Value("C_UOM_Conversion_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set UOM.
@param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID <= 0) Set_ValueNoCheck("C_UOM_ID", null);
            else
                Set_ValueNoCheck("C_UOM_ID", C_UOM_ID);
        }/** Get UOM.
@return Unit of Measure */
        public int GetC_UOM_ID() { Object ii = Get_Value("C_UOM_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set C_UOM_ProductBarcode_ID.
@param C_UOM_ProductBarcode_ID C_UOM_ProductBarcode_ID */
        public void SetC_UOM_ProductBarcode_ID(int C_UOM_ProductBarcode_ID) { if (C_UOM_ProductBarcode_ID < 1) throw new ArgumentException("C_UOM_ProductBarcode_ID is mandatory."); Set_ValueNoCheck("C_UOM_ProductBarcode_ID", C_UOM_ProductBarcode_ID); }/** Get C_UOM_ProductBarcode_ID.
@return C_UOM_ProductBarcode_ID */
        public int GetC_UOM_ProductBarcode_ID() { Object ii = Get_Value("C_UOM_ProductBarcode_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** C_UOM_To_ID AD_Reference_ID=114 */
        public static int C_UOM_TO_ID_AD_Reference_ID = 114;/** Set UoM To.
@param C_UOM_To_ID Target or destination Unit of Measure */
        public void SetC_UOM_To_ID(int C_UOM_To_ID)
        {
            if (C_UOM_To_ID <= 0) Set_ValueNoCheck("C_UOM_To_ID", null);
            else
                Set_ValueNoCheck("C_UOM_To_ID", C_UOM_To_ID);
        }/** Get UoM To.
@return Target or destination Unit of Measure */
        public int GetC_UOM_To_ID() { Object ii = Get_Value("C_UOM_To_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Product.
@param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_ValueNoCheck("M_Product_ID", null);
            else
                Set_ValueNoCheck("M_Product_ID", M_Product_ID);
        }/** Get Product.
@return Product, Service, Item */
        public int GetM_Product_ID() { Object ii = Get_Value("M_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set UPC/EAN.
@param UPC Bar Code (Universal Product Code or its superset European Article Number) */
        public void SetUPC(String UPC) { if (UPC != null && UPC.Length > 30) { log.Warning("Length > 30 - truncated"); UPC = UPC.Substring(0, 30); } Set_Value("UPC", UPC); }/** Get UPC/EAN.
@return Bar Code (Universal Product Code or its superset European Article Number) */
        public String GetUPC() { return (String)Get_Value("UPC"); }
    }
}
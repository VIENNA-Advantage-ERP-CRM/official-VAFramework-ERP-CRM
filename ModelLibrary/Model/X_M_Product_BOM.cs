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
    using System.Data;/** Generated Model for M_Product_BOM
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_Product_BOM : PO
    {
        public X_M_Product_BOM(Context ctx, int M_Product_BOM_ID, Trx trxName)
            : base(ctx, M_Product_BOM_ID, trxName)
        {/** if (M_Product_BOM_ID == 0){SetBOMQty (0.0);SetLine (0);// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM M_Product_BOM WHERE M_Product_ID=@M_Product_ID@
SetM_ProductBOM_ID (0);SetM_Product_BOM_ID (0);SetM_Product_ID (0);} */
        }
        public X_M_Product_BOM(Ctx ctx, int M_Product_BOM_ID, Trx trxName)
            : base(ctx, M_Product_BOM_ID, trxName)
        {/** if (M_Product_BOM_ID == 0){SetBOMQty (0.0);SetLine (0);// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM M_Product_BOM WHERE M_Product_ID=@M_Product_ID@
SetM_ProductBOM_ID (0);SetM_Product_BOM_ID (0);SetM_Product_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_Product_BOM(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_Product_BOM(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_Product_BOM(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_M_Product_BOM() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27763409763836L;/** Last Updated Timestamp 12/9/2016 5:24:07 PM */
        public static long updatedMS = 1481284447047L;/** AD_Table_ID=383 */
        public static int Table_ID; // =383;
        /** TableName=M_Product_BOM */
        public static String Table_Name = "M_Product_BOM";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_M_Product_BOM[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Quantity.
@param BOMQty Bill of Materials Quantity */
        public void SetBOMQty(Decimal? BOMQty) { if (BOMQty == null) throw new ArgumentException("BOMQty is mandatory."); Set_Value("BOMQty", (Decimal?)BOMQty); }/** Get Quantity.
@return Bill of Materials Quantity */
        public Decimal GetBOMQty() { Object bd = Get_Value("BOMQty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
        /** BOMType AD_Reference_ID=279 */
        public static int BOMTYPE_AD_Reference_ID = 279;/** In alternative Group 1 = 1 */
        public static String BOMTYPE_InAlternativeGroup1 = "1";/** In alternative Group 2 = 2 */
        public static String BOMTYPE_InAlternativeGroup2 = "2";/** In alternaltve Group 3 = 3 */
        public static String BOMTYPE_InAlternaltveGroup3 = "3";/** In alternative Group 4 = 4 */
        public static String BOMTYPE_InAlternativeGroup4 = "4";/** In alternative Group 5 = 5 */
        public static String BOMTYPE_InAlternativeGroup5 = "5";/** In alternative Group 6 = 6 */
        public static String BOMTYPE_InAlternativeGroup6 = "6";/** In alternative Group 7 = 7 */
        public static String BOMTYPE_InAlternativeGroup7 = "7";/** In alternative Group 8 = 8 */
        public static String BOMTYPE_InAlternativeGroup8 = "8";/** In alternative Group 9 = 9 */
        public static String BOMTYPE_InAlternativeGroup9 = "9";/** Optional Part = O */
        public static String BOMTYPE_OptionalPart = "O";/** Standard Part = P */
        public static String BOMTYPE_StandardPart = "P";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsBOMTypeValid(String test) { return test == null || test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("4") || test.Equals("5") || test.Equals("6") || test.Equals("7") || test.Equals("8") || test.Equals("9") || test.Equals("O") || test.Equals("P"); }/** Set BOM Type.
@param BOMType Type of BOM */
        public void SetBOMType(String BOMType)
        {
            if (!IsBOMTypeValid(BOMType))
                throw new ArgumentException("BOMType Invalid value - " + BOMType + " - Reference_ID=279 - 1 - 2 - 3 - 4 - 5 - 6 - 7 - 8 - 9 - O - P"); if (BOMType != null && BOMType.Length > 1) { log.Warning("Length > 1 - truncated"); BOMType = BOMType.Substring(0, 1); } Set_Value("BOMType", BOMType);
        }/** Get BOM Type.
@return Type of BOM */
        public String GetBOMType() { return (String)Get_Value("BOMType"); }/** Set UOM.
@param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID <= 0) Set_Value("C_UOM_ID", null);
            else
                Set_Value("C_UOM_ID", C_UOM_ID);
        }/** Get UOM.
@return Unit of Measure */
        public int GetC_UOM_ID() { Object ii = Get_Value("C_UOM_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description) { if (Description != null && Description.Length > 255) { log.Warning("Length > 255 - truncated"); Description = Description.Substring(0, 255); } Set_Value("Description", Description); }/** Get Description.
@return Optional short description of the record */
        public String GetDescription() { return (String)Get_Value("Description"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_ValueNoCheck("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Line No.
@param Line Unique line for this document */
        public void SetLine(int Line) { Set_Value("Line", Line); }/** Get Line No.
@return Unique line for this document */
        public int GetLine() { Object ii = Get_Value("Line"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** M_ProductBOM_ID AD_Reference_ID=162 */
        public static int M_PRODUCTBOM_ID_AD_Reference_ID = 162;/** Set Component.
@param M_ProductBOM_ID Bill of Material Component Product */
        public void SetM_ProductBOM_ID(int M_ProductBOM_ID) { if (M_ProductBOM_ID < 1) throw new ArgumentException("M_ProductBOM_ID is mandatory."); Set_Value("M_ProductBOM_ID", M_ProductBOM_ID); }/** Get Component.
@return Bill of Material Component Product */
        public int GetM_ProductBOM_ID() { Object ii = Get_Value("M_ProductBOM_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Get Record ID/ColumnName
@return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair() { return new KeyNamePair(Get_ID(), GetM_ProductBOM_ID().ToString()); }/** Set BOM Line.
@param M_Product_BOM_ID BOM Line */
        public void SetM_Product_BOM_ID(int M_Product_BOM_ID) { if (M_Product_BOM_ID < 1) throw new ArgumentException("M_Product_BOM_ID is mandatory."); Set_ValueNoCheck("M_Product_BOM_ID", M_Product_BOM_ID); }/** Get BOM Line.
@return BOM Line */
        public int GetM_Product_BOM_ID() { Object ii = Get_Value("M_Product_BOM_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Product.
@param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID) { if (M_Product_ID < 1) throw new ArgumentException("M_Product_ID is mandatory."); Set_ValueNoCheck("M_Product_ID", M_Product_ID); }/** Get Product.
@return Product, Service, Item */
        public int GetM_Product_ID() { Object ii = Get_Value("M_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Cost.
@param VA019_Cost Cost */
        public void SetVA019_Cost(Decimal? VA019_Cost) { Set_Value("VA019_Cost", (Decimal?)VA019_Cost); }/** Get Cost.
@return Cost */
        public Decimal GetVA019_Cost() { Object bd = Get_Value("VA019_Cost"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Ingredient Yield.
@param VA019_Yield Ingredient Yield */
        public void SetVA019_Yield(Decimal? VA019_Yield) { Set_Value("VA019_Yield", (Decimal?)VA019_Yield); }/** Get Ingredient Yield.
@return Ingredient Yield */
        public Decimal GetVA019_Yield() { Object bd = Get_Value("VA019_Yield"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set Waste Yield Cost.
@param VA019_WasteYieldCost Waste Yield Cost */
        public void SetVA019_WasteYieldCost(Decimal? VA019_WasteYieldCost) { Set_Value("VA019_WasteYieldCost", (Decimal?)VA019_WasteYieldCost); }/** Get Waste Yield Cost.
@return Waste Yield Cost */
        public Decimal GetVA019_WasteYieldCost() { Object bd = Get_Value("VA019_WasteYieldCost"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

    }
}
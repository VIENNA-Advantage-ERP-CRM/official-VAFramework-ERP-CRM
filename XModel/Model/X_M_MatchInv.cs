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
    using System.Data;
    /** Generated Model for M_MatchInv
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_MatchInv : PO
    {
        public X_M_MatchInv(Context ctx, int M_MatchInv_ID, Trx trxName)
            : base(ctx, M_MatchInv_ID, trxName)
        {
            /** if (M_MatchInv_ID == 0)
            {
            SetVAB_InvoiceLine_ID (0);
            SetDateAcct (DateTime.Now);
            SetDateTrx (DateTime.Now);
            SetM_InOutLine_ID (0);
            SetM_MatchInv_ID (0);
            SetM_Product_ID (0);
            SetPosted (false);
            SetProcessed (false);	// N
            SetProcessing (false);	// N
            SetQty (0.0);
            }
             */
        }
        public X_M_MatchInv(Ctx ctx, int M_MatchInv_ID, Trx trxName)
            : base(ctx, M_MatchInv_ID, trxName)
        {
            /** if (M_MatchInv_ID == 0)
            {
            SetVAB_InvoiceLine_ID (0);
            SetDateAcct (DateTime.Now);
            SetDateTrx (DateTime.Now);
            SetM_InOutLine_ID (0);
            SetM_MatchInv_ID (0);
            SetM_Product_ID (0);
            SetPosted (false);
            SetProcessed (false);	// N
            SetProcessing (false);	// N
            SetQty (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_MatchInv(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_MatchInv(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_MatchInv(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_MatchInv()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514379976L;
        /** Last Updated Timestamp 7/29/2010 1:07:43 PM */
        public static long updatedMS = 1280389063187L;
        /** VAF_TableView_ID=472 */
        public static int Table_ID;
        // =472;

        /** TableName=M_MatchInv */
        public static String Table_Name = "M_MatchInv";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(3);
        /** AccessLevel
        @return 3 - Client - Org 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_M_MatchInv[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Invoice Line.
        @param VAB_InvoiceLine_ID Invoice Detail Line */
        public void SetVAB_InvoiceLine_ID(int VAB_InvoiceLine_ID)
        {
            if (VAB_InvoiceLine_ID < 1) throw new ArgumentException("VAB_InvoiceLine_ID is mandatory.");
            Set_ValueNoCheck("VAB_InvoiceLine_ID", VAB_InvoiceLine_ID);
        }
        /** Get Invoice Line.
        @return Invoice Detail Line */
        public int GetVAB_InvoiceLine_ID()
        {
            Object ii = Get_Value("VAB_InvoiceLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Account Date.
        @param DateAcct General Ledger Date */
        public void SetDateAcct(DateTime? DateAcct)
        {
            if (DateAcct == null) throw new ArgumentException("DateAcct is mandatory.");
            Set_Value("DateAcct", (DateTime?)DateAcct);
        }
        /** Get Account Date.
        @return General Ledger Date */
        public DateTime? GetDateAcct()
        {
            return (DateTime?)Get_Value("DateAcct");
        }
        /** Set Transaction Date.
        @param DateTrx Transaction Date */
        public void SetDateTrx(DateTime? DateTrx)
        {
            if (DateTrx == null) throw new ArgumentException("DateTrx is mandatory.");
            Set_ValueNoCheck("DateTrx", (DateTime?)DateTrx);
        }
        /** Get Transaction Date.
        @return Transaction Date */
        public DateTime? GetDateTrx()
        {
            return (DateTime?)Get_Value("DateTrx");
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                Description = Description.Substring(0, 255);
            }
            Set_Value("Description", Description);
        }
        /** Get Description.
        @return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
        }
        /** Set Document No.
        @param DocumentNo Document sequence number of the document */
        public void SetDocumentNo(String DocumentNo)
        {
            if (DocumentNo != null && DocumentNo.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                DocumentNo = DocumentNo.Substring(0, 30);
            }
            Set_Value("DocumentNo", DocumentNo);
        }
        /** Get Document No.
        @return Document sequence number of the document */
        public String GetDocumentNo()
        {
            return (String)Get_Value("DocumentNo");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetDocumentNo());
        }
        /** Set Attribute Set Instance.
        @param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID <= 0) Set_ValueNoCheck("M_AttributeSetInstance_ID", null);
            else
                Set_ValueNoCheck("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }
        /** Get Attribute Set Instance.
        @return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID()
        {
            Object ii = Get_Value("M_AttributeSetInstance_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shipment/Receipt Line.
        @param M_InOutLine_ID Line on Shipment or Receipt document */
        public void SetM_InOutLine_ID(int M_InOutLine_ID)
        {
            if (M_InOutLine_ID < 1) throw new ArgumentException("M_InOutLine_ID is mandatory.");
            Set_ValueNoCheck("M_InOutLine_ID", M_InOutLine_ID);
        }
        /** Get Shipment/Receipt Line.
        @return Line on Shipment or Receipt document */
        public int GetM_InOutLine_ID()
        {
            Object ii = Get_Value("M_InOutLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Match Invoice.
        @param M_MatchInv_ID Match Shipment/Receipt to Invoice */
        public void SetM_MatchInv_ID(int M_MatchInv_ID)
        {
            if (M_MatchInv_ID < 1) throw new ArgumentException("M_MatchInv_ID is mandatory.");
            Set_ValueNoCheck("M_MatchInv_ID", M_MatchInv_ID);
        }
        /** Get Match Invoice.
        @return Match Shipment/Receipt to Invoice */
        public int GetM_MatchInv_ID()
        {
            Object ii = Get_Value("M_MatchInv_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID < 1) throw new ArgumentException("M_Product_ID is mandatory.");
            Set_ValueNoCheck("M_Product_ID", M_Product_ID);
        }
        /** Get Product.
        @return Product, Service, Item */
        public int GetM_Product_ID()
        {
            Object ii = Get_Value("M_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Posted.
        @param Posted Posting status */
        public void SetPosted(Boolean Posted)
        {
            Set_ValueNoCheck("Posted", Posted);
        }
        /** Get Posted.
        @return Posting status */
        public Boolean IsPosted()
        {
            Object oo = Get_Value("Posted");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Processed.
        @param Processed The document has been processed */
        public void SetProcessed(Boolean Processed)
        {
            Set_ValueNoCheck("Processed", Processed);
        }
        /** Get Processed.
        @return The document has been processed */
        public Boolean IsProcessed()
        {
            Object oo = Get_Value("Processed");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Process Now.
        @param Processing Process Now */
        public void SetProcessing(Boolean Processing)
        {
            Set_Value("Processing", Processing);
        }
        /** Get Process Now.
        @return Process Now */
        public Boolean IsProcessing()
        {
            Object oo = Get_Value("Processing");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Quantity.
        @param Qty Quantity */
        public void SetQty(Decimal? Qty)
        {
            if (Qty == null) throw new ArgumentException("Qty is mandatory.");
            Set_ValueNoCheck("Qty", (Decimal?)Qty);
        }
        /** Get Quantity.
        @return Quantity */
        public Decimal GetQty()
        {
            Object bd = Get_Value("Qty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Cost Calculated.@param IsCostCalculated Cost Calculated */
        public void SetIsCostCalculated(Boolean IsCostCalculated) { Set_Value("IsCostCalculated", IsCostCalculated); }
        /** Get Cost Calculated.@return Cost Calculated */
        public Boolean IsCostCalculated() { Object oo = Get_Value("IsCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** Set Cost Immediately.@param IsCostImmediate Update Costs immediately for testing */
        public void SetIsCostImmediate(Boolean IsCostImmediate) { Set_Value("IsCostImmediate", IsCostImmediate); }
        /** Get Cost Immediately.@return Update Costs immediately for testing */
        public Boolean IsCostImmediate() { Object oo = Get_Value("IsCostImmediate"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** Set Post Current Cost Price.@param PostCurrentCostPrice It indicate the cost after cost calculation of current record */
        public void SetPostCurrentCostPrice(Decimal? PostCurrentCostPrice) { Set_Value("PostCurrentCostPrice", (Decimal?)PostCurrentCostPrice); }
        /** Get Post Current Cost Price. @return It indicate the cost after cost calculation of current record */
        public Decimal GetPostCurrentCostPrice() { Object bd = Get_Value("PostCurrentCostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
        /** Set Current Cost.@param CurrentCostPrice The currently used cost price */
        public void SetCurrentCostPrice(Decimal? CurrentCostPrice) { Set_Value("CurrentCostPrice", (Decimal?)CurrentCostPrice); }
        /** Get Current Cost.@return The currently used cost price */
        public Decimal GetCurrentCostPrice() { Object bd = Get_Value("CurrentCostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

    }

}

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
    /** Generated Model for C_OrderLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_OrderLine : PO
    {
        public X_C_OrderLine(Context ctx, int C_OrderLine_ID, Trx trxName)
            : base(ctx, C_OrderLine_ID, trxName)
        {
            /** if (C_OrderLine_ID == 0)
            { 
            SetC_Currency_ID (0);	// @C_Currency_ID@
            SetC_OrderLine_ID (0);
            SetC_Order_ID (0);
            SetC_Tax_ID (0);
            SetC_UOM_ID (0);	// @#C_UOM_ID@
            SetDateOrdered (DateTime.Now);	// @DateOrdered@
            SetFreightAmt (0.0);
            SetIsDescription (false);	// N
            SetLine (0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM C_OrderLine WHERE C_Order_ID=@C_Order_ID@
            SetLineNetAmt (0.0);
            SetM_Warehouse_ID (0);	// @M_Warehouse_ID@
            SetPriceActual (0.0);
            SetPriceEntered (0.0);
            SetPriceLimit (0.0);
            SetPriceList (0.0);
            SetProcessed (false);	// N
            SetQtyDelivered (0.0);
            SetQtyEntered (0.0);	// 1
            SetQtyInvoiced (0.0);
            SetQtyLostSales (0.0);
            SetQtyOrdered (0.0);	// 1
            SetQtyReserved (0.0);
            }
             */
        }
        public X_C_OrderLine(Ctx ctx, int C_OrderLine_ID, Trx trxName)
            : base(ctx, C_OrderLine_ID, trxName)
        {
            /** if (C_OrderLine_ID == 0)
            {
            SetC_Currency_ID (0);	// @C_Currency_ID@
            SetC_OrderLine_ID (0);
            SetC_Order_ID (0);
            SetC_Tax_ID (0);
            SetC_UOM_ID (0);	// @#C_UOM_ID@
            SetDateOrdered (DateTime.Now);	// @DateOrdered@
            SetFreightAmt (0.0);
            SetIsDescription (false);	// N
            SetLine (0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM C_OrderLine WHERE C_Order_ID=@C_Order_ID@
            SetLineNetAmt (0.0);
            SetM_Warehouse_ID (0);	// @M_Warehouse_ID@
            SetPriceActual (0.0);
            SetPriceEntered (0.0);
            SetPriceLimit (0.0);
            SetPriceList (0.0);
            SetProcessed (false);	// N
            SetQtyDelivered (0.0);
            SetQtyEntered (0.0);	// 1
            SetQtyInvoiced (0.0);
            SetQtyLostSales (0.0);
            SetQtyOrdered (0.0);	// 1
            SetQtyReserved (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_OrderLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_OrderLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_OrderLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_OrderLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27562514373331L;
        /** Last Updated Timestamp 7/29/2010 1:07:36 PM */
        public static long updatedMS = 1280389056542L;
        /** AD_Table_ID=260 */
        public static int Table_ID;
        // =260;

        /** TableName=C_OrderLine */
        public static String Table_Name = "C_OrderLine";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(1);
        /** AccessLevel
        @return 1 - Org 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
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
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_C_OrderLine[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** AD_OrgTrx_ID AD_Reference_ID=130 */
        public static int AD_ORGTRX_ID_AD_Reference_ID = 130;
        /** Set Trx Organization.
        @param AD_OrgTrx_ID Performing or initiating organization */
        public void SetAD_OrgTrx_ID(int AD_OrgTrx_ID)
        {
            if (AD_OrgTrx_ID <= 0) Set_Value("AD_OrgTrx_ID", null);
            else
                Set_Value("AD_OrgTrx_ID", AD_OrgTrx_ID);
        }
        /** Get Trx Organization.
        @return Performing or initiating organization */
        public int GetAD_OrgTrx_ID()
        {
            Object ii = Get_Value("AD_OrgTrx_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Activity.
        @param C_Activity_ID Business Activity */
        public void SetC_Activity_ID(int C_Activity_ID)
        {
            if (C_Activity_ID <= 0) Set_Value("C_Activity_ID", null);
            else
                Set_Value("C_Activity_ID", C_Activity_ID);
        }
        /** Get Activity.
        @return Business Activity */
        public int GetC_Activity_ID()
        {
            Object ii = Get_Value("C_Activity_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Business Partner.
        @param C_BPartner_ID Identifies a Business Partner */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID <= 0) Set_ValueNoCheck("C_BPartner_ID", null);
            else
                Set_ValueNoCheck("C_BPartner_ID", C_BPartner_ID);
        }
        /** Get Business Partner.
        @return Identifies a Business Partner */
        public int GetC_BPartner_ID()
        {
            Object ii = Get_Value("C_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Partner Location.
        @param C_BPartner_Location_ID Identifies the (ship to) address for this Business Partner */
        public void SetC_BPartner_Location_ID(int C_BPartner_Location_ID)
        {
            if (C_BPartner_Location_ID <= 0) Set_Value("C_BPartner_Location_ID", null);
            else
                Set_Value("C_BPartner_Location_ID", C_BPartner_Location_ID);
        }
        /** Get Partner Location.
        @return Identifies the (ship to) address for this Business Partner */
        public int GetC_BPartner_Location_ID()
        {
            Object ii = Get_Value("C_BPartner_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Campaign.
        @param C_Campaign_ID Marketing Campaign */
        public void SetC_Campaign_ID(int C_Campaign_ID)
        {
            if (C_Campaign_ID <= 0) Set_Value("C_Campaign_ID", null);
            else
                Set_Value("C_Campaign_ID", C_Campaign_ID);
        }
        /** Get Campaign.
        @return Marketing Campaign */
        public int GetC_Campaign_ID()
        {
            Object ii = Get_Value("C_Campaign_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Charge.
        @param C_Charge_ID Additional document charges */
        public void SetC_Charge_ID(int C_Charge_ID)
        {
            if (C_Charge_ID <= 0) Set_Value("C_Charge_ID", null);
            else
                Set_Value("C_Charge_ID", C_Charge_ID);
        }
        /** Get Charge.
        @return Additional document charges */
        public int GetC_Charge_ID()
        {
            Object ii = Get_Value("C_Charge_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency.
        @param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            if (C_Currency_ID < 1) throw new ArgumentException("C_Currency_ID is mandatory.");
            Set_ValueNoCheck("C_Currency_ID", C_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetC_Currency_ID()
        {
            Object ii = Get_Value("C_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Order Line.
        @param C_OrderLine_ID Order Line */
        public void SetC_OrderLine_ID(int C_OrderLine_ID)
        {
            if (C_OrderLine_ID < 1) throw new ArgumentException("C_OrderLine_ID is mandatory.");
            Set_ValueNoCheck("C_OrderLine_ID", C_OrderLine_ID);
        }
        /** Get Order Line.
        @return Order Line */
        public int GetC_OrderLine_ID()
        {
            Object ii = Get_Value("C_OrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Order.
        @param C_Order_ID Order */
        public void SetC_Order_ID(int C_Order_ID)
        {
            if (C_Order_ID < 1) throw new ArgumentException("C_Order_ID is mandatory.");
            Set_ValueNoCheck("C_Order_ID", C_Order_ID);
        }
        /** Get Order.
        @return Order */
        public int GetC_Order_ID()
        {
            Object ii = Get_Value("C_Order_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetC_Order_ID().ToString());
        }
        /** Set Project Phase.
        @param C_ProjectPhase_ID Phase of a Project */
        public void SetC_ProjectPhase_ID(int C_ProjectPhase_ID)
        {
            if (C_ProjectPhase_ID <= 0) Set_ValueNoCheck("C_ProjectPhase_ID", null);
            else
                Set_ValueNoCheck("C_ProjectPhase_ID", C_ProjectPhase_ID);
        }
        /** Get Project Phase.
        @return Phase of a Project */
        public int GetC_ProjectPhase_ID()
        {
            Object ii = Get_Value("C_ProjectPhase_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Project Task.
        @param C_ProjectTask_ID Actual Project Task in a Phase */
        public void SetC_ProjectTask_ID(int C_ProjectTask_ID)
        {
            if (C_ProjectTask_ID <= 0) Set_ValueNoCheck("C_ProjectTask_ID", null);
            else
                Set_ValueNoCheck("C_ProjectTask_ID", C_ProjectTask_ID);
        }
        /** Get Project Task.
        @return Actual Project Task in a Phase */
        public int GetC_ProjectTask_ID()
        {
            Object ii = Get_Value("C_ProjectTask_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Project.
        @param C_Project_ID Financial Project */
        public void SetC_Project_ID(int C_Project_ID)
        {
            if (C_Project_ID <= 0) Set_Value("C_Project_ID", null);
            else
                Set_Value("C_Project_ID", C_Project_ID);
        }
        /** Get Project.
        @return Financial Project */
        public int GetC_Project_ID()
        {
            Object ii = Get_Value("C_Project_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax.
        @param C_Tax_ID Tax identifier */
        public void SetC_Tax_ID(int C_Tax_ID)
        {
            if (C_Tax_ID < 1) throw new ArgumentException("C_Tax_ID is mandatory.");
            Set_Value("C_Tax_ID", C_Tax_ID);
        }
        /** Get Tax.
        @return Tax identifier */
        public int GetC_Tax_ID()
        {
            Object ii = Get_Value("C_Tax_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set UOM.
        @param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID < 1) throw new ArgumentException("C_UOM_ID is mandatory.");
            Set_ValueNoCheck("C_UOM_ID", C_UOM_ID);
        }
        /** Get UOM.
        @return Unit of Measure */
        public int GetC_UOM_ID()
        {
            Object ii = Get_Value("C_UOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Date Delivered.
        @param DateDelivered Date when the product was delivered */
        public void SetDateDelivered(DateTime? DateDelivered)
        {
            Set_ValueNoCheck("DateDelivered", (DateTime?)DateDelivered);
        }
        /** Get Date Delivered.
        @return Date when the product was delivered */
        public DateTime? GetDateDelivered()
        {
            return (DateTime?)Get_Value("DateDelivered");
        }
        /** Set Date Invoiced.
        @param DateInvoiced Date printed on Invoice */
        public void SetDateInvoiced(DateTime? DateInvoiced)
        {
            Set_ValueNoCheck("DateInvoiced", (DateTime?)DateInvoiced);
        }
        /** Get Date Invoiced.
        @return Date printed on Invoice */
        public DateTime? GetDateInvoiced()
        {
            return (DateTime?)Get_Value("DateInvoiced");
        }
        /** Set Date Ordered.
        @param DateOrdered Date of Order */
        public void SetDateOrdered(DateTime? DateOrdered)
        {
            if (DateOrdered == null) throw new ArgumentException("DateOrdered is mandatory.");
            Set_Value("DateOrdered", (DateTime?)DateOrdered);
        }
        /** Get Date Ordered.
        @return Date of Order */
        public DateTime? GetDateOrdered()
        {
            return (DateTime?)Get_Value("DateOrdered");
        }
        /** Set Date Promised.
        @param DatePromised Date Order was promised */
        public void SetDatePromised(DateTime? DatePromised)
        {
            Set_Value("DatePromised", (DateTime?)DatePromised);
        }
        /** Get Date Promised.
        @return Date Order was promised */
        public DateTime? GetDatePromised()
        {
            return (DateTime?)Get_Value("DatePromised");
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
        /** Set Discount %.
        @param Discount Discount in percent */
        public void SetDiscount(Decimal? Discount)
        {
            Set_Value("Discount", (Decimal?)Discount);
        }
        /** Get Discount %.
        @return Discount in percent */
        public Decimal GetDiscount()
        {
            Object bd = Get_Value("Discount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Freight Amount.
        @param FreightAmt Freight Amount */
        public void SetFreightAmt(Decimal? FreightAmt)
        {
            if (FreightAmt == null) throw new ArgumentException("FreightAmt is mandatory.");
            Set_Value("FreightAmt", (Decimal?)FreightAmt);
        }
        /** Get Freight Amount.
        @return Freight Amount */
        public Decimal GetFreightAmt()
        {
            Object bd = Get_Value("FreightAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Description Only.
        @param IsDescription if true, the line is just description and no transaction */
        public void SetIsDescription(Boolean IsDescription)
        {
            Set_Value("IsDescription", IsDescription);
        }
        /** Get Description Only.
        @return if true, the line is just description and no transaction */
        public Boolean IsDescription()
        {
            Object oo = Get_Value("IsDescription");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Line No.
        @param Line Unique line for this document */
        public void SetLine(int Line)
        {
            Set_Value("Line", Line);
        }
        /** Get Line No.
        @return Unique line for this document */
        public int GetLine()
        {
            Object ii = Get_Value("Line");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Line Amount.
        @param LineNetAmt Line Extended Amount (Quantity * Actual Price) without Freight and Charges */
        public void SetLineNetAmt(Decimal? LineNetAmt)
        {
            if (LineNetAmt == null) throw new ArgumentException("LineNetAmt is mandatory.");
            Set_ValueNoCheck("LineNetAmt", (Decimal?)LineNetAmt);
        }
        /** Get Line Amount.
        @return Line Extended Amount (Quantity * Actual Price) without Freight and Charges */
        public Decimal GetLineNetAmt()
        {
            Object bd = Get_Value("LineNetAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Line Total.
        @param LineTotalAmt Total line amount incl. Tax */
        public void SetLineTotalAmt(Decimal? LineTotalAmt)
        {
            Set_Value("LineTotalAmt", (Decimal?)LineTotalAmt);
        }
        /** Get Line Total.
        @return Total line amount incl. Tax */
        public Decimal GetLineTotalAmt()
        {
            Object bd = Get_Value("LineTotalAmt");
            if (bd == null)
                return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Attribute Set Instance.
        @param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID <= 0) Set_Value("M_AttributeSetInstance_ID", null);
            else
                Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }
        /** Get Attribute Set Instance.
        @return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID()
        {
            Object ii = Get_Value("M_AttributeSetInstance_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_Value("M_Product_ID", null);
            else
                Set_Value("M_Product_ID", M_Product_ID);
        }
        /** Get Product.
        @return Product, Service, Item */
        public int GetM_Product_ID()
        {
            Object ii = Get_Value("M_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Freight Carrier.
        @param M_Shipper_ID Method or manner of product delivery */
        public void SetM_Shipper_ID(int M_Shipper_ID)
        {
            if (M_Shipper_ID <= 0) Set_Value("M_Shipper_ID", null);
            else
                Set_Value("M_Shipper_ID", M_Shipper_ID);
        }
        /** Get Freight Carrier.
        @return Method or manner of product delivery */
        public int GetM_Shipper_ID()
        {
            Object ii = Get_Value("M_Shipper_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** M_Warehouse_ID AD_Reference_ID=197 */
        public static int M_WAREHOUSE_ID_AD_Reference_ID = 197;
        /** Set Warehouse.
        @param M_Warehouse_ID Storage Warehouse and Service Point */
        public void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (M_Warehouse_ID < 1) throw new ArgumentException("M_Warehouse_ID is mandatory.");
            Set_Value("M_Warehouse_ID", M_Warehouse_ID);
        }
        /** Get Warehouse.
        @return Storage Warehouse and Service Point */
        public int GetM_Warehouse_ID()
        {
            Object ii = Get_Value("M_Warehouse_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Orig_InOutLine_ID AD_Reference_ID=295 */
        public static int ORIG_INOUTLINE_ID_AD_Reference_ID = 295;
        /** Set Orig Shipment Line.
        @param Orig_InOutLine_ID Original shipment line of the RMA */
        public void SetOrig_InOutLine_ID(int Orig_InOutLine_ID)
        {
            if (Orig_InOutLine_ID <= 0) Set_Value("Orig_InOutLine_ID", null);
            else
                Set_Value("Orig_InOutLine_ID", Orig_InOutLine_ID);
        }
        /** Get Orig Shipment Line.
        @return Original shipment line of the RMA */
        public int GetOrig_InOutLine_ID()
        {
            Object ii = Get_Value("Orig_InOutLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Orig_OrderLine_ID AD_Reference_ID=271 */
        public static int ORIG_ORDERLINE_ID_AD_Reference_ID = 271;
        /** Set Orig Sales Order Line.
        @param Orig_OrderLine_ID Original Sales Order Line for Return Material Authorization */
        public void SetOrig_OrderLine_ID(int Orig_OrderLine_ID)
        {
            if (Orig_OrderLine_ID <= 0) Set_Value("Orig_OrderLine_ID", null);
            else
                Set_Value("Orig_OrderLine_ID", Orig_OrderLine_ID);
        }
        /** Get Orig Sales Order Line.
        @return Original Sales Order Line for Return Material Authorization */
        public int GetOrig_OrderLine_ID()
        {
            Object ii = Get_Value("Orig_OrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Unit Price.
        @param PriceActual Actual Price */
        public void SetPriceActual(Decimal? PriceActual)
        {
            if (PriceActual == null) throw new ArgumentException("PriceActual is mandatory.");
            Set_ValueNoCheck("PriceActual", (Decimal?)PriceActual);
        }
        /** Get Unit Price.
        @return Actual Price */
        public Decimal GetPriceActual()
        {
            Object bd = Get_Value("PriceActual");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Cost Price.
        @param PriceCost Price per Unit of Measure including all indirect costs (Freight, etc.) */
        public void SetPriceCost(Decimal? PriceCost)
        {
            Set_Value("PriceCost", (Decimal?)PriceCost);
        }
        /** Get Cost Price.
        @return Price per Unit of Measure including all indirect costs (Freight, etc.) */
        public Decimal GetPriceCost()
        {
            Object bd = Get_Value("PriceCost");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Price.
        @param PriceEntered Price Entered - the price based on the selected/base UoM */
        public void SetPriceEntered(Decimal? PriceEntered)
        {
            if (PriceEntered == null) throw new ArgumentException("PriceEntered is mandatory.");
            Set_Value("PriceEntered", (Decimal?)PriceEntered);
        }
        /** Get Price.
        @return Price Entered - the price based on the selected/base UoM */
        public Decimal GetPriceEntered()
        {
            Object bd = Get_Value("PriceEntered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Limit Price.
        @param PriceLimit Lowest price for a product */
        public void SetPriceLimit(Decimal? PriceLimit)
        {
            if (PriceLimit == null) throw new ArgumentException("PriceLimit is mandatory.");
            Set_Value("PriceLimit", (Decimal?)PriceLimit);
        }
        /** Get Limit Price.
        @return Lowest price for a product */
        public Decimal GetPriceLimit()
        {
            Object bd = Get_Value("PriceLimit");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set List Price.
        @param PriceList List Price */
        public void SetPriceList(Decimal? PriceList)
        {
            if (PriceList == null) throw new ArgumentException("PriceList is mandatory.");
            Set_Value("PriceList", (Decimal?)PriceList);
        }
        /** Get List Price.
        @return List Price */
        public Decimal GetPriceList()
        {
            Object bd = Get_Value("PriceList");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Processed.
        @param Processed The document has been processed */
        public void SetProcessed(Boolean Processed)
        {
            Set_Value("Processed", Processed);
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
        /** Set Quantity Delivered.
        @param QtyDelivered Quantity Delivered */
        public void SetQtyDelivered(Decimal? QtyDelivered)
        {
            if (QtyDelivered == null) throw new ArgumentException("QtyDelivered is mandatory.");
            Set_ValueNoCheck("QtyDelivered", (Decimal?)QtyDelivered);
        }
        /** Get Quantity Delivered.
        @return Quantity Delivered */
        public Decimal GetQtyDelivered()
        {
            Object bd = Get_Value("QtyDelivered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Quantity.
        @param QtyEntered The Quantity Entered is based on the selected UoM */
        public void SetQtyEntered(Decimal? QtyEntered)
        {
            if (QtyEntered == null) throw new ArgumentException("QtyEntered is mandatory.");
            Set_Value("QtyEntered", (Decimal?)QtyEntered);
        }
        /** Get Quantity.
        @return The Quantity Entered is based on the selected UoM */
        public Decimal GetQtyEntered()
        {
            Object bd = Get_Value("QtyEntered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Quantity Invoiced.
        @param QtyInvoiced Invoiced Quantity */
        public void SetQtyInvoiced(Decimal? QtyInvoiced)
        {
            if (QtyInvoiced == null) throw new ArgumentException("QtyInvoiced is mandatory.");
            Set_ValueNoCheck("QtyInvoiced", (Decimal?)QtyInvoiced);
        }
        /** Get Quantity Invoiced.
        @return Invoiced Quantity */
        public Decimal GetQtyInvoiced()
        {
            Object bd = Get_Value("QtyInvoiced");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Lost Sales Quantity.
        @param QtyLostSales Quantity of potential sales */
        public void SetQtyLostSales(Decimal? QtyLostSales)
        {
            if (QtyLostSales == null) throw new ArgumentException("QtyLostSales is mandatory.");
            Set_Value("QtyLostSales", (Decimal?)QtyLostSales);
        }
        /** Get Lost Sales Quantity.
        @return Quantity of potential sales */
        public Decimal GetQtyLostSales()
        {
            Object bd = Get_Value("QtyLostSales");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Ordered Quantity.
        @param QtyOrdered Ordered Quantity */
        public void SetQtyOrdered(Decimal? QtyOrdered)
        {
            if (QtyOrdered == null) throw new ArgumentException("QtyOrdered is mandatory.");
            Set_Value("QtyOrdered", (Decimal?)QtyOrdered);
        }
        /** Get Ordered Quantity.
        @return Ordered Quantity */
        public Decimal GetQtyOrdered()
        {
            Object bd = Get_Value("QtyOrdered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        // GetQtyEstimation

        /** Set Estimation Quantity.
       @param QtyEstimation Estimation Quantity */
        public void SetQtyEstimation(Decimal? QtyEstimation)
        {
            if (QtyEstimation == null) throw new ArgumentException("QtyEstimation is mandatory.");
            Set_Value("QtyEstimation", (Decimal?)QtyEstimation);
        }
        /** Get Estimation Quantity.
        @return Estimation Quantity */
        public Decimal GetQtyEstimation()
        {
            Object bd = Get_Value("QtyEstimation");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }



        // GetQtyReleased

        /** Set Released Quantity.
       @param QtyReleased Released Quantity */
        public void SetQtyReleased(Decimal? QtyReleased)
        {
            if (QtyReleased == null) throw new ArgumentException("QtyReleased is mandatory.");
            Set_Value("QtyReleased", (Decimal?)QtyReleased);
        }
        /** Get Released Quantity.
        @return Released Quantity */
        public Decimal GetQtyReleased()
        {
            Object bd = Get_Value("QtyReleased");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }





        /** Set Quantity Reserved.
        @param QtyReserved Quantity Reserved */
        public void SetQtyReserved(Decimal? QtyReserved)
        {
            if (QtyReserved == null) throw new ArgumentException("QtyReserved is mandatory.");
            Set_ValueNoCheck("QtyReserved", (Decimal?)QtyReserved);
        }
        /** Get Quantity Reserved.
        @return Quantity Reserved */
        public Decimal GetQtyReserved()
        {
            Object bd = Get_Value("QtyReserved");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Quantity Returned.
        @param QtyReturned Quantity Returned */
        public void SetQtyReturned(Decimal? QtyReturned)
        {
            Set_Value("QtyReturned", (Decimal?)QtyReturned);
        }
        /** Get Quantity Returned.
        @return Quantity Returned */
        public Decimal GetQtyReturned()
        {
            Object bd = Get_Value("QtyReturned");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Revenue Recognition Amt.
        @param RRAmt Revenue Recognition Amount */
        public void SetRRAmt(Decimal? RRAmt)
        {
            Set_Value("RRAmt", (Decimal?)RRAmt);
        }
        /** Get Revenue Recognition Amt.
        @return Revenue Recognition Amount */
        public Decimal GetRRAmt()
        {
            Object bd = Get_Value("RRAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Revenue Recognition Start.
        @param RRStartDate Revenue Recognition Start Date */
        public void SetRRStartDate(DateTime? RRStartDate)
        {
            Set_Value("RRStartDate", (DateTime?)RRStartDate);
        }
        /** Get Revenue Recognition Start.
        @return Revenue Recognition Start Date */
        public DateTime? GetRRStartDate()
        {
            return (DateTime?)Get_Value("RRStartDate");
        }

        /** Ref_OrderLine_ID AD_Reference_ID=271 */
        public static int REF_ORDERLINE_ID_AD_Reference_ID = 271;
        /** Set Referenced Order Line.
        @param Ref_OrderLine_ID Reference to corresponding Sales/Purchase Order */
        public void SetRef_OrderLine_ID(int Ref_OrderLine_ID)
        {
            if (Ref_OrderLine_ID <= 0) Set_Value("Ref_OrderLine_ID", null);
            else
                Set_Value("Ref_OrderLine_ID", Ref_OrderLine_ID);
        }
        /** Get Referenced Order Line.
        @return Reference to corresponding Sales/Purchase Order */
        public int GetRef_OrderLine_ID()
        {
            Object ii = Get_Value("Ref_OrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Assigned Resource.
        @param S_ResourceAssignment_ID Assigned Resource */
        public void SetS_ResourceAssignment_ID(int S_ResourceAssignment_ID)
        {
            if (S_ResourceAssignment_ID <= 0) Set_Value("S_ResourceAssignment_ID", null);
            else
                Set_Value("S_ResourceAssignment_ID", S_ResourceAssignment_ID);
        }
        /** Get Assigned Resource.
        @return Assigned Resource */
        public int GetS_ResourceAssignment_ID()
        {
            Object ii = Get_Value("S_ResourceAssignment_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Taxable Amount.
        @param TaxAbleAmt Taxable amount in Transaction currency */
        public void SetTaxAbleAmt(Decimal? TaxAbleAmt)
        {
            Set_Value("TaxAbleAmt", (Decimal?)TaxAbleAmt);
        }
        /** Get Taxable Amount.
        @return Taxable amount in Transaction currency */
        public Decimal GetTaxAbleAmt()
        {
            Object bd = Get_Value("TaxAbleAmt");
            if (bd == null)
                return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Tax Amount.
        @param TaxAmt Tax Amount for a document */
        public void SetTaxAmt(Decimal? TaxAmt)
        {
            Set_Value("TaxAmt", (Decimal?)TaxAmt);
        }
        /** Get Tax Amount.
        @return Tax Amount for a document */
        public Decimal GetTaxAmt()
        {
            Object bd = Get_Value("TaxAmt");
            if (bd == null)
                return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Tax Base Amount.
        @param TaxBaseAmt Tax in Base Currecy */
        public void SetTaxBaseAmt(Decimal? TaxBaseAmt)
        {
            Set_Value("TaxBaseAmt", (Decimal?)TaxBaseAmt);
        }
        /** Get Tax Base Amount.
        @return Tax in Base Currecy */
        public Decimal GetTaxBaseAmt()
        {
            Object bd = Get_Value("TaxBaseAmt");
            if (bd == null)
                return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** User1_ID AD_Reference_ID=134 */
        public static int USER1_ID_AD_Reference_ID = 134;
        /** Set User List 1.
        @param User1_ID User defined list element #1 */
        public void SetUser1_ID(int User1_ID)
        {
            if (User1_ID <= 0) Set_Value("User1_ID", null);
            else
                Set_Value("User1_ID", User1_ID);
        }
        /** Get User List 1.
        @return User defined list element #1 */
        public int GetUser1_ID()
        {
            Object ii = Get_Value("User1_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** User2_ID AD_Reference_ID=137 */
        public static int USER2_ID_AD_Reference_ID = 137;
        /** Set User List 2.
        @param User2_ID User defined list element #2 */
        public void SetUser2_ID(int User2_ID)
        {
            if (User2_ID <= 0) Set_Value("User2_ID", null);
            else
                Set_Value("User2_ID", User2_ID);
        }
        /** Get User List 2.
        @return User defined list element #2 */
        public int GetUser2_ID()
        {
            Object ii = Get_Value("User2_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Discount per Unit.
        @param Discount per Unit */
        public void SetED007_DiscountPerUnit(Decimal? ED007_DiscountPerUnit)
        {
            Set_Value("ED007_DiscountPerUnit", (Decimal?)ED007_DiscountPerUnit);
        }
        /** Get Discount per Unit.
        @return Discount per Unit */
        public Decimal GetED007_DiscountPerUnit()
        {
            Object bd = Get_Value("ED007_DiscountPerUnit");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set ED007_DiscountPercent.
        @param ED007_DiscountPercent */
        public void SetED007_DiscountPercent(Decimal? ED007_DiscountPercent)
        {
            Set_Value("ED007_DiscountPercent", (Decimal?)ED007_DiscountPercent);
        }
        /** Get Discount Percentage 5.
        @return Discount Percentage 5 */
        public Decimal GetED007_DiscountPercent()
        {
            Object bd = Get_Value("ED007_DiscountPercent");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Discount Percentage 1.
        @param ED007_DiscountPercentage1 */
        public void SetED007_DiscountPercentage1(Decimal? ED007_DiscountPercentage1)
        {
            Set_Value("ED007_DiscountPercentage1", (Decimal?)ED007_DiscountPercentage1);
        }
        /** Get Discount Percentage 1.
        @return Discount Percentage 1 */
        public Decimal GetED007_DiscountPercentage1()
        {
            Object bd = Get_Value("ED007_DiscountPercentage1");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Discount Percentage 2.
        @param ED007_DiscountPercentage2*/
        public void SetED007_DiscountPercentage2(Decimal? ED007_DiscountPercentage2)
        {
            Set_Value("ED007_DiscountPercentage2", (Decimal?)ED007_DiscountPercentage2);
        }
        /** Get Discount Percentage 2.
        @return Discount Percentage 2 */
        public Decimal GetED007_DiscountPercentage2()
        {
            Object bd = Get_Value("ED007_DiscountPercentage2");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Discount Percentage 3.
        @param ED007_DiscountPercentage3 */
        public void SetED007_DiscountPercentage3(Decimal? ED007_DiscountPercentage3)
        {
            Set_Value("ED007_DiscountPercentage3", (Decimal?)ED007_DiscountPercentage3);
        }
        /** Get Discount Percentage 3.
        @return Discount Percentage 3 */
        public Decimal GetED007_DiscountPercentage3()
        {
            Object bd = Get_Value("ED007_DiscountPercentage3");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Discount Percentage 4.
        @param ED007_DiscountPercentage4 */
        public void SetED007_DiscountPercentage4(Decimal? ED007_DiscountPercentage4)
        {
            Set_Value("ED007_DiscountPercentage4", (Decimal?)ED007_DiscountPercentage4);
        }
        /** Get Discount Percentage 4.
        @return Discount Percentage 4 */
        public Decimal GetED007_DiscountPercentage4()
        {
            Object bd = Get_Value("ED007_DiscountPercentage4");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Discount Percentage 5.
        @param ED007_DiscountPercentage5 */
        public void SetED007_DiscountPercentage5(Decimal? ED007_DiscountPercentage5)
        {
            Set_Value("ED007_DiscountPercentage5", (Decimal?)ED007_DiscountPercentage5);
        }
        /** Get Discount Percentage 5.
        @return Discount Percentage 5 */
        public Decimal GetED007_DiscountPercentage5()
        {
            Object bd = Get_Value("ED007_DiscountPercentage5");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set ED007_DiscountAmount.
        @param ED007_DiscountAmount */
        public void SetED007_DiscountAmount(Decimal? ED007_DiscountAmount)
        {
            Set_Value("ED007_DiscountAmount", (Decimal?)ED007_DiscountAmount);
        }
        /** Get ED007_DiscountAmount.
        @return ED007_DiscountAmount */
        public Decimal GetED007_DiscountAmount()
        {
            Object bd = Get_Value("ED007_DiscountAmount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set ED007_DscuntlineAmt.
        @param ED007_DscuntlineAmt */
        public void SetED007_DscuntlineAmt(Decimal? ED007_DscuntlineAmt)
        {
            Set_Value("ED007_DscuntlineAmt", (Decimal?)ED007_DscuntlineAmt);
        }
        /** Get ED007_DscuntlineAmt.
        @return ED007_DscuntlineAmt */
        public Decimal GetED007_DscuntlineAmt()
        {
            Object bd = Get_Value("ED007_DscuntlineAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Value Base Discount.
        @param Value Base Discount */
        public void SetED007_ValueBaseDiscount(Decimal? ED007_ValueBaseDiscount)
        {
            Set_Value("ED007_ValueBaseDiscount", (Decimal?)ED007_ValueBaseDiscount);
        }
        /** Get Value Base Discount.
        @return Value Base Discount */
        public Decimal GetED007_ValueBaseDiscount()
        {
            Object bd = Get_Value("ED007_ValueBaseDiscount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Contract.
        @param IsContract Contract */
        public void SetIsContract(Boolean IsContract)
        {
            Set_Value("IsContract", IsContract);
        }
        /** Get Contract.
        @return Contract */
        public Boolean IsContract()
        {
            Object oo = Get_Value("IsContract");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set C_Contract_ID.
        @param C_Contract_ID C_Contract_ID */
        public void SetC_Contract_ID(int C_Contract_ID)
        {
            if (C_Contract_ID <= 0) Set_Value("C_Contract_ID", null);
            else
                Set_Value("C_Contract_ID", C_Contract_ID);
        }
        /** Get C_Contract_ID.
        @return C_Contract_ID */
        public int GetC_Contract_ID()
        {
            Object ii = Get_Value("C_Contract_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Start Date.
        @param StartDate First effective day (inclusive) */
        public void SetStartDate(DateTime? StartDate)
        {
            Set_Value("StartDate", (DateTime?)StartDate);
        }
        /** Get Start Date.
        @return First effective day (inclusive) */
        public DateTime? GetStartDate()
        {
            return (DateTime?)Get_Value("StartDate");
        }

        /** Set End Date.
        @param EndDate Last effective date (inclusive) */
        public void SetEndDate(DateTime? EndDate)
        {
            Set_Value("EndDate", (DateTime?)EndDate);
        }
        /** Get End Date.
        @return Last effective date (inclusive) */
        public DateTime? GetEndDate()
        {
            return (DateTime?)Get_Value("EndDate");
        }

        /** Set Billing Frequency.
        @param C_Frequency_ID Billing Frequency */
        public void SetC_Frequency_ID(int C_Frequency_ID)
        {
            if (C_Frequency_ID <= 0) Set_Value("C_Frequency_ID", null);
            else
                Set_Value("C_Frequency_ID", C_Frequency_ID);
        }
        /** Get Billing Frequency.
        @return Billing Frequency */
        public int GetC_Frequency_ID()
        {
            Object ii = Get_Value("C_Frequency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set QuantityPerCycle.
        @param QuantityPerCycle The QuantityPerCycle Entered is based on the selected UoM */
        public void SetQtyPerCycle(Decimal? QtyPerCycle)
        {
            if (QtyPerCycle == null) throw new ArgumentException("QtyPerCycle is mandatory.");
            Set_Value("QtyPerCycle", (Decimal?)QtyPerCycle);
        }
        /** Get QuantityPerCycle.
        @return The QuantityPerCycle is based on the selected UoM */
        public Decimal GetQtyPerCycle()
        {
            Object bd = Get_Value("QtyPerCycle");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set No of Cycle.
        @param NoofCycle No of Cycle */
        public void SetNoofCycle(int NoofCycle)
        {
            Set_Value("NoofCycle", NoofCycle);
        }
        /** Get No of Cycle.
        @return No of Cycle */
        public int GetNoofCycle()
        {
            Object ii = Get_Value("NoofCycle");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Checked.
        @param IsChecked Checked */
        public void SetIsChecked(Boolean IsChecked)
        {
            Set_Value("IsChecked", IsChecked);
        }
        /** Get Checked.
        @return Checked */
        public Boolean IsChecked()
        {
            Object oo = Get_Value("IsChecked");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set Create Service Contract.
        @param CreateServiceContract Create Service Contract */
        public void SetCreateServiceContract(String CreateServiceContract)
        {
            if (CreateServiceContract != null && CreateServiceContract.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                CreateServiceContract = CreateServiceContract.Substring(0, 50);
            }
            Set_Value("CreateServiceContract", CreateServiceContract);
        }
        /** Get Create Service Contract.
        @return Create Service Contract */
        public String GetCreateServiceContract()
        {
            return (String)Get_Value("CreateServiceContract");
        }

        /** Set Budget Violation Amount.
        @param BudgetViolationAmount Budget Violation Amount */
        public void SetBudgetViolationAmount(Decimal? BudgetViolationAmount)
        {
            Set_Value("BudgetViolationAmount", (Decimal?)BudgetViolationAmount);
        }
        /** Get Budget Violation Amount.
        @return Budget Violation Amount */
        public Decimal GetBudgetViolationAmount()
        {
            Object bd = Get_Value("BudgetViolationAmount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** DTD001_Org_ID AD_Reference_ID=417 */
        public static int DTD001_ORG_ID_AD_Reference_ID = 417;
        /** Set Reference Organization.
        @param DTD001_Org_ID Reference Organization */
        public void SetDTD001_Org_ID(int DTD001_Org_ID)
        {
            if (DTD001_Org_ID <= 0) Set_Value("DTD001_Org_ID", null);
            else
                Set_Value("DTD001_Org_ID", DTD001_Org_ID);
        }
        /** Get Reference Organization.
        @return Reference Organization */
        public int GetDTD001_Org_ID()
        {
            Object ii = Get_Value("DTD001_Org_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        //Added by Mohit VAWMS 20-8-2015

        /** Set Quantity Dedicated.
        @param QtyDedicated Quantity for which there is a pending Warehouse Task */
        public void SetQtyDedicated(Decimal? QtyDedicated)
        {
            if (QtyDedicated == null) throw new ArgumentException("QtyDedicated is mandatory.");
            Set_ValueNoCheck("QtyDedicated", (Decimal?)QtyDedicated);
        }
        /** Get Quantity Dedicated.
        @return Quantity for which there is a pending Warehouse Task */
        public Decimal GetQtyDedicated()
        {
            Object bd = Get_Value("QtyDedicated");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Quantity Allocated.
        @param QtyAllocated Quantity that has been picked and is awaiting shipment */
        public void SetQtyAllocated(Decimal? QtyAllocated)
        {
            if (QtyAllocated == null) throw new ArgumentException("QtyAllocated is mandatory.");
            Set_ValueNoCheck("QtyAllocated", (Decimal?)QtyAllocated);
        }
        /** Get Quantity Allocated.
        @return Quantity that has been picked and is awaiting shipment */
        public Decimal GetQtyAllocated()
        {
            Object bd = Get_Value("QtyAllocated");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        //End

        /** VA008_RefOrderLineExt_ID AD_Reference_ID=1000179 */
        public static int VA008_REFORDERLINEEXT_ID_AD_Reference_ID = 1000179;
        /** Set Reference Order Line Extension.
        @param VA008_RefOrderLineExt_ID Reference Order Line Extension */
        public void SetVA008_RefOrderLineExt_ID(int VA008_RefOrderLineExt_ID)
        {
            if (VA008_RefOrderLineExt_ID <= 0) Set_Value("VA008_RefOrderLineExt_ID", null);
            else
                Set_Value("VA008_RefOrderLineExt_ID", VA008_RefOrderLineExt_ID);
        }
        /** Get Reference Order Line Extension.
        @return Reference Order Line Extension */
        public int GetVA008_RefOrderLineExt_ID()
        {
            Object ii = Get_Value("VA008_RefOrderLineExt_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VA008_RefOrderLine_ID AD_Reference_ID=1000179 */
        public static int VA008_REFORDERLINE_ID_AD_Reference_ID = 1000179;
        /** Set Reference Order Line.
        @param VA008_RefOrderLine_ID Reference Order Line */
        public void SetVA008_RefOrderLine_ID(int VA008_RefOrderLine_ID)
        {
            if (VA008_RefOrderLine_ID <= 0) Set_Value("VA008_RefOrderLine_ID", null);
            else
                Set_Value("VA008_RefOrderLine_ID", VA008_RefOrderLine_ID);
        }
        /** Get Reference Order Line.
        @return Reference Order Line */
        public int GetVA008_RefOrderLine_ID()
        {
            Object ii = Get_Value("VA008_RefOrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Discount Amount.
        @param VAPOS_DiscountAmount Discount Amount */
        public void SetVAPOS_DiscountAmount(Decimal? VAPOS_DiscountAmount)
        {
            Set_Value("VAPOS_DiscountAmount", (Decimal?)VAPOS_DiscountAmount);
        }
        /** Get Discount Amount.
        @return Discount Amount */
        public Decimal GetVAPOS_DiscountAmount()
        {
            Object bd = Get_Value("VAPOS_DiscountAmount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Attributes.
        @param Attributes Optional short Attributes of the record */
        public void SetVAPOS_Attributes(String VAPOS_Attributes)
        {
            if (VAPOS_Attributes != null && VAPOS_Attributes.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                VAPOS_Attributes = VAPOS_Attributes.Substring(0, 255);
            }
            Set_Value("VAPOS_Attributes", VAPOS_Attributes);
        }
        /** Get Attributes.
        @return Optional short Attributes of the record */
        public String GetVAPOS_Attributes()
        {
            return (String)Get_Value("VAPOS_Attributes");
        }

        /** Set Line No.
        @param VAPOS_LineNo Line No */
        public void SetVAPOS_LineNo(int VAPOS_LineNo)
        {
            Set_Value("VAPOS_LineNo", VAPOS_LineNo);
        }
        /** Get Line No.
        @return Line No */
        public int GetVAPOS_LineNo()
        {
            Object ii = Get_Value("VAPOS_LineNo");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Voucher No Detail.
        @param VA018_VouchrNoDtail_ID Voucher No Detail */
        public void SetVA018_VouchrNoDtail_ID(int VA018_VouchrNoDtail_ID)
        {
            if (VA018_VouchrNoDtail_ID <= 0) Set_Value("VA018_VouchrNoDtail_ID", null);
            else
                Set_Value("VA018_VouchrNoDtail_ID", VA018_VouchrNoDtail_ID);
        }
        /** Get Voucher No Detail.
        @return Voucher No Detail */
        public int GetVA018_VouchrNoDtail_ID()
        {
            Object ii = Get_Value("VA018_VouchrNoDtail_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set VA023_Reference.
@param VA023_Reference VA023_Reference */
        public void SetVA023_Reference(String VA023_Reference)
        {
            if (VA023_Reference != null && VA023_Reference.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                VA023_Reference = VA023_Reference.Substring(0, 100);
            }
            Set_Value("VA023_Reference", VA023_Reference);
        }
        /** Get VA023_Reference.
        @return VA023_Reference */
        public String GetVA023_Reference()
        {
            return (String)Get_Value("VA023_Reference");
        }
        /** VAPOS_RtnOrderLine_ID AD_Reference_ID=271 */
        public static int VAPOS_RTNORDERLINE_ID_AD_Reference_ID = 271;
        /** Set Return Order Line.
        @param VAPOS_RtnOrderLine_ID Displays the reference of the order line returned from POS */
        public void SetVAPOS_RtnOrderLine_ID(int VAPOS_RtnOrderLine_ID)
        {
            if (VAPOS_RtnOrderLine_ID <= 0) Set_Value("VAPOS_RtnOrderLine_ID", null);
            else
                Set_Value("VAPOS_RtnOrderLine_ID", VAPOS_RtnOrderLine_ID);
        }
        /** Get Return Order Line.
        @return Displays the reference of the order line returned from POS */
        public int GetVAPOS_RtnOrderLine_ID()
        {
            Object ii = Get_Value("VAPOS_RtnOrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Return Order Line.
       @param VAPOS_RtnOrderLine_ID Displays the reference of the order line returned from POS */
        public void SetVA019_SeatNo(String VA019_SeatNo)
        {
            if (VA019_SeatNo != null && VA019_SeatNo.Length > 20)
            {
                log.Warning("Length > 100 - truncated");
                VA019_SeatNo = VA019_SeatNo.Substring(0, 100);
            }
            Set_Value("VA019_SeatNo", VA019_SeatNo);
        }
        /** Get VA023_Reference.
        @return VA023_Reference */
        public String GetVA019_SeatNo()
        {
            return (String)Get_Value("VA019_SeatNo");
        }

        /** VA019_Locator_ID AD_Reference_ID=1000264 */
        public static int VA019_LOCATOR_ID_AD_Reference_ID = 1000264;
        /** Set Locator.
        @param VA019_Locator_ID Locator */
        public void SetVA019_Locator_ID(int VA019_Locator_ID)
        {
            if (VA019_Locator_ID <= 0) Set_Value("VA019_Locator_ID", null);
            else
                Set_Value("VA019_Locator_ID", VA019_Locator_ID);
        }
        /** Get Locator.
        @return Locator */
        public int GetVA019_Locator_ID()
        {
            Object ii = Get_Value("VA019_Locator_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }


        /** VA019_Warehouse_ID AD_Reference_ID=1000265 */
        public static int VA019_WAREHOUSE_ID_AD_Reference_ID = 1000265;
        /** Set Warehouse.
        @param VA019_Warehouse_ID Warehouse */
        public void SetVA019_Warehouse_ID(int VA019_Warehouse_ID)
        {
            if (VA019_Warehouse_ID <= 0) Set_Value("VA019_Warehouse_ID", null);
            else
                Set_Value("VA019_Warehouse_ID", VA019_Warehouse_ID);
        }
        /** Get Warehouse.
        @return Warehouse */
        public int GetVA019_Warehouse_ID()
        {
            Object ii = Get_Value("VA019_Warehouse_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Waste.
        @param VA019_IsWaste Waste */
        public void SetVA019_IsWaste(Boolean VA019_IsWaste)
        {
            Set_Value("VA019_IsWaste", VA019_IsWaste);
        }
        /** Get Waste.
        @return Waste */
        public Boolean IsVA019_IsWaste()
        {
            Object oo = Get_Value("VA019_IsWaste");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set UPC/EAN.
        @param UPC Bar Code (Universal Product Code or its superset European Article Number) */
        public void SetUPC(String UPC)
        {
            if (UPC != null && UPC.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                UPC = UPC.Substring(0, 100);
            }
            Set_Value("UPC", UPC);
        }
        /** Get UPC/EAN.
        @return Bar Code (Universal Product Code or its superset European Article Number) */
        public String GetUPC()
        {
            return (String)Get_Value("UPC");
        }



        /** Set Reason.
        @param VAPOS_Reason_ID Reason */
        public void SetVAPOS_Reason_ID(int VAPOS_Reason_ID)
        {
            if (VAPOS_Reason_ID <= 0) Set_Value("VAPOS_Reason_ID", null);
            else
                Set_Value("VAPOS_Reason_ID", VAPOS_Reason_ID);
        }
        /** Get Reason.
        @return Reason */
        public int GetVAPOS_Reason_ID() { Object ii = Get_Value("VAPOS_Reason_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** VAPOS_LineStatus AD_Reference_ID=1000259 */
        public static int VAPOS_LINESTATUS_AD_Reference_ID = 1000259;/** Void = VO */
        public static String VAPOS_LINESTATUS_Void = "VO";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsVAPOS_LineStatusValid(String test) { return test == null || test.Equals("VO"); }/** Set Line Status.
@param VAPOS_LineStatus Line Status */
        public void SetVAPOS_LineStatus(String VAPOS_LineStatus)
        {
            if (!IsVAPOS_LineStatusValid(VAPOS_LineStatus))
                throw new ArgumentException("VAPOS_LineStatus Invalid value - " + VAPOS_LineStatus + " - Reference_ID=1000259 - VO"); if (VAPOS_LineStatus != null && VAPOS_LineStatus.Length > 2) { log.Warning("Length > 2 - truncated"); VAPOS_LineStatus = VAPOS_LineStatus.Substring(0, 2); } Set_Value("VAPOS_LineStatus", VAPOS_LineStatus);
        }/** Get Line Status.
@return Line Status */
        public String GetVAPOS_LineStatus() { return (String)Get_Value("VAPOS_LineStatus"); }

        //---------------------------------------
        /** Set Discount Per Unit.
@param VA025_DiscountPerUnit Discount Per Unit */
        public void SetVA025_DiscountPerUnit(Decimal? VA025_DiscountPerUnit) { Set_Value("VA025_DiscountPerUnit", (Decimal?)VA025_DiscountPerUnit); }/** Get Discount Per Unit.
@return Discount Per Unit */
        public Decimal GetVA025_DiscountPerUnit() { Object bd = Get_Value("VA025_DiscountPerUnit"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Discount Percentage 1.
@param VA025_DiscountPercentage1 Discount Percentage 1 */
        public void SetVA025_DiscountPercentage1(Decimal? VA025_DiscountPercentage1) { Set_Value("VA025_DiscountPercentage1", (Decimal?)VA025_DiscountPercentage1); }/** Get Discount Percentage 1.
@return Discount Percentage 1 */
        public Decimal GetVA025_DiscountPercentage1() { Object bd = Get_Value("VA025_DiscountPercentage1"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Discount Percentage 2.
@param VA025_DiscountPercentage2 Discount Percentage 2 */
        public void SetVA025_DiscountPercentage2(Decimal? VA025_DiscountPercentage2) { Set_Value("VA025_DiscountPercentage2", (Decimal?)VA025_DiscountPercentage2); }/** Get Discount Percentage 2.
@return Discount Percentage 2 */
        public Decimal GetVA025_DiscountPercentage2() { Object bd = Get_Value("VA025_DiscountPercentage2"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Discount Percentage 3.
@param VA025_DiscountPercentage3 Discount Percentage 3 */
        public void SetVA025_DiscountPercentage3(Decimal? VA025_DiscountPercentage3) { Set_Value("VA025_DiscountPercentage3", (Decimal?)VA025_DiscountPercentage3); }/** Get Discount Percentage 3.
@return Discount Percentage 3 */
        public Decimal GetVA025_DiscountPercentage3() { Object bd = Get_Value("VA025_DiscountPercentage3"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Discount Percentage 4.
@param VA025_DiscountPercentage4 Discount Percentage 4 */
        public void SetVA025_DiscountPercentage4(Decimal? VA025_DiscountPercentage4) { Set_Value("VA025_DiscountPercentage4", (Decimal?)VA025_DiscountPercentage4); }/** Get Discount Percentage 4.
@return Discount Percentage 4 */
        public Decimal GetVA025_DiscountPercentage4() { Object bd = Get_Value("VA025_DiscountPercentage4"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Discount Percentage 5.
@param VA025_DiscountPercentage5 Discount Percentage 5 */
        public void SetVA025_DiscountPercentage5(Decimal? VA025_DiscountPercentage5) { Set_Value("VA025_DiscountPercentage5", (Decimal?)VA025_DiscountPercentage5); }/** Get Discount Percentage 5.
@return Discount Percentage 5 */
        public Decimal GetVA025_DiscountPercentage5() { Object bd = Get_Value("VA025_DiscountPercentage5"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Discount Line Amount.
@param VA025_DscuntLineAmt Discount Line Amount */
        public void SetVA025_DscuntLineAmt(Decimal? VA025_DscuntLineAmt) { Set_Value("VA025_DscuntLineAmt", (Decimal?)VA025_DscuntLineAmt); }/** Get Discount Line Amount.
@return Discount Line Amount */
        public Decimal GetVA025_DscuntLineAmt() { Object bd = Get_Value("VA025_DscuntLineAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Promotional Amount.
@param VA025_PromotionalAmount Promotional Amount */
        public void SetVA025_PromotionalAmount(Decimal? VA025_PromotionalAmount) { Set_Value("VA025_PromotionalAmount", (Decimal?)VA025_PromotionalAmount); }/** Get Promotional Amount.
@return Promotional Amount */
        public Decimal GetVA025_PromotionalAmount() { Object bd = Get_Value("VA025_PromotionalAmount"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Value Base Discount.
@param VA025_ValueBaseDiscount Value Base Discount */
        public void SetVA025_ValueBaseDiscount(Decimal? VA025_ValueBaseDiscount) { Set_Value("VA025_ValueBaseDiscount", (Decimal?)VA025_ValueBaseDiscount); }/** Get Value Base Discount.
@return Value Base Discount */
        public Decimal GetVA025_ValueBaseDiscount() { Object bd = Get_Value("VA025_ValueBaseDiscount"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set IsCommissionCalculated.
@param IsCommissionCalculated IsCommissionCalculated */
        public void SetIsCommissionCalculated(Boolean IsCommissionCalculated)
        {
            Set_Value("IsCommissionCalculated", IsCommissionCalculated);
        }
        /** Get IsCommissionCalculated.
        @return IsCommissionCalculated */
        public Boolean IsCommissionCalculated()
        {
            Object oo = Get_Value("IsCommissionCalculated");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }



        // GetQtyEstimation

        /** Set Blanket Quantity.
       @param QtyBlanket Blanket Quantity */
        public void SetQtyBlanket(Decimal? QtyBlanket)
        {
            if (QtyBlanket == null) throw new ArgumentException("QtyBlanket is mandatory.");
            Set_Value("QtyBlanket", (Decimal?)QtyBlanket);
        }
        /** Get QtyBlanket .
        @return QtyBlanket  */
        public Decimal GetQtyBlanket()
        {
            Object bd = Get_Value("QtyBlanket");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }


        public int GetC_OrderLine_Blanket_ID()
        {
            Object ii = Get_Value("C_OrderLine_Blanket_ID");
            if (ii == null)
                return 0;
            return Convert.ToInt32(ii);
        }
        public void SetC_OrderLine_Blanket_ID(int C_OrderLine_Blanket_ID)
        {
            if (C_OrderLine_Blanket_ID <= 0) Set_Value("C_OrderLine_Blanket_ID", null);
            else
                Set_Value("C_OrderLine_Blanket_ID", C_OrderLine_Blanket_ID);
        }

        /** Set Current Cost.@param CurrentCostPrice The currently used cost price */
        public void SetCurrentCostPrice(Decimal? CurrentCostPrice) { Set_Value("CurrentCostPrice", (Decimal?)CurrentCostPrice); }
        /** Get Current Cost.@return The currently used cost price */
        public Decimal GetCurrentCostPrice() { Object bd = Get_Value("CurrentCostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set Drop Shipment.
        @param IsDropShip Drop Shipments are sent from the Vendor directly to the Customer */
        public void SetIsDropShip(Boolean IsDropShip) { Set_Value("IsDropShip", IsDropShip); }/** Get Drop Shipment.
        @return Drop Shipments are sent from the Vendor directly to the Customer */
        public Boolean IsDropShip() { Object oo = Get_Value("IsDropShip"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** Set BasePrice.@param BasePrice This column contains the original price of product from pricelist . */
        public void SetBasePrice(Decimal? BasePrice)
        {
            Set_Value("BasePrice", (Decimal?)BasePrice);
        }
        /** Get BasePrice.@return This column contains the original price of product from pricelist . */
        public Decimal GetBasePrice()
        {
            Object bd = Get_Value("BasePrice");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Discount Amount after Total Discount.@param AmountAfterApplyDiscount It is the Total amount deducted with respect to Line amount based on the calculation of Overall Discount on the Grand Total. */
        public void SetAmountAfterApplyDiscount(Decimal? AmountAfterApplyDiscount) { Set_Value("AmountAfterApplyDiscount", (Decimal?)AmountAfterApplyDiscount); }
        /** Get Discount Amount after Total Discount.@return It is the Total amount deducted with respect to Line amount based on the calculation of Overall Discount on the Grand Total. */
        public Decimal GetAmountAfterApplyDiscount() { Object bd = Get_Value("AmountAfterApplyDiscount"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /// <summary>
        /// Set Surcharge Amount.
        /// </summary>
        /// <param name="SurchargeAmt">Surcharge Amount for a document</param>
        public void SetSurchargeAmt(Decimal? SurchargeAmt)
        {
            Set_Value("SurchargeAmt", (Decimal?)SurchargeAmt);
        }
        /// <summary>
        /// Get Surcharge Amount.
        /// </summary>
        /// <returns>Surcharge Amount for a document</returns>
        public Decimal GetSurchargeAmt()
        {
            Object bd = Get_Value("SurchargeAmt");
            if (bd == null)
                return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Tax exempt. 
@param IsTaxExempt Business partner is exempt from tax */
        public void SetIsTaxExempt(Boolean IsTaxExempt)
        { Set_Value("IsTaxExempt", IsTaxExempt); }/** Get Tax exempt.
@return Business partner is exempt from tax */
        public Boolean IsTaxExempt() { Object oo = Get_Value("IsTaxExempt"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** Set Tax Exemption Reason.
@param C_TaxExemptReason_ID Tax Exemption reason indicates the reason for the exemption for the items those are exempted from tax. */
        public void SetC_TaxExemptReason_ID(int C_TaxExemptReason_ID)
        {
            if (C_TaxExemptReason_ID <= 0) Set_Value("C_TaxExemptReason_ID", null);
            else
                Set_Value("C_TaxExemptReason_ID", C_TaxExemptReason_ID);
        }/** Get Tax Exemption Reason.
@return Tax Exemption reason indicates the reason for the exemption for the items those are exempted from tax. */
        public int GetC_TaxExemptReason_ID() { Object ii = Get_Value("C_TaxExemptReason_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }

}

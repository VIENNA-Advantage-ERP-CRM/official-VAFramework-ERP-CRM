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
/** Generated Model for VAM_Inv_InOutLineConfirm
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_Inv_InOutLineConfirm : PO
    {
        public X_VAM_Inv_InOutLineConfirm(Context ctx, int VAM_Inv_InOutLineConfirm_ID, Trx trxName)
            : base(ctx, VAM_Inv_InOutLineConfirm_ID, trxName)
        {
            /** if (VAM_Inv_InOutLineConfirm_ID == 0)
            {
            SetConfirmedQty (0.0);
            SetVAM_Inv_InOutConfirm_ID (0);
            SetVAM_Inv_InOutLineConfirm_ID (0);
            SetVAM_Inv_InOutLine_ID (0);
            SetProcessed (false);	// N
            SetTargetQty (0.0);
            }
             */
        }
        public X_VAM_Inv_InOutLineConfirm(Ctx ctx, int VAM_Inv_InOutLineConfirm_ID, Trx trxName)
            : base(ctx, VAM_Inv_InOutLineConfirm_ID, trxName)
        {
            /** if (VAM_Inv_InOutLineConfirm_ID == 0)
            {
            SetConfirmedQty (0.0);
            SetVAM_Inv_InOutConfirm_ID (0);
            SetVAM_Inv_InOutLineConfirm_ID (0);
            SetVAM_Inv_InOutLine_ID (0);
            SetProcessed (false);	// N
            SetTargetQty (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_Inv_InOutLineConfirm(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_Inv_InOutLineConfirm(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_Inv_InOutLineConfirm(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_Inv_InOutLineConfirm()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514379679L;
        /** Last Updated Timestamp 7/29/2010 1:07:42 PM */
        public static long updatedMS = 1280389062890L;
        /** VAF_TableView_ID=728 */
        public static int Table_ID;
        // =728;

        /** TableName=VAM_Inv_InOutLineConfirm */
        public static String Table_Name = "VAM_Inv_InOutLineConfirm";

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
            StringBuilder sb = new StringBuilder("X_VAM_Inv_InOutLineConfirm[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Invoice Line.
        @param VAB_InvoiceLine_ID Invoice Detail Line */
        public void SetVAB_InvoiceLine_ID(int VAB_InvoiceLine_ID)
        {
            if (VAB_InvoiceLine_ID <= 0) Set_Value("VAB_InvoiceLine_ID", null);
            else
                Set_Value("VAB_InvoiceLine_ID", VAB_InvoiceLine_ID);
        }
        /** Get Invoice Line.
        @return Invoice Detail Line */
        public int GetVAB_InvoiceLine_ID()
        {
            Object ii = Get_Value("VAB_InvoiceLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Confirmation No.
        @param ConfirmationNo Confirmation Number */
        public void SetConfirmationNo(String ConfirmationNo)
        {
            if (ConfirmationNo != null && ConfirmationNo.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                ConfirmationNo = ConfirmationNo.Substring(0, 20);
            }
            Set_Value("ConfirmationNo", ConfirmationNo);
        }
        /** Get Confirmation No.
        @return Confirmation Number */
        public String GetConfirmationNo()
        {
            return (String)Get_Value("ConfirmationNo");
        }
        /** Set Confirmed Quantity.
        @param ConfirmedQty Confirmation of a received quantity */
        public void SetConfirmedQty(Decimal? ConfirmedQty)
        {
            if (ConfirmedQty == null) throw new ArgumentException("ConfirmedQty is mandatory.");
            Set_Value("ConfirmedQty", (Decimal?)ConfirmedQty);
        }
        /** Get Confirmed Quantity.
        @return Confirmation of a received quantity */
        public Decimal GetConfirmedQty()
        {
            Object bd = Get_Value("ConfirmedQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
        /** Set Difference.
        @param DifferenceQty Difference Quantity */
        public void SetDifferenceQty(Decimal? DifferenceQty)
        {
            Set_Value("DifferenceQty", (Decimal?)DifferenceQty);
        }
        /** Get Difference.
        @return Difference Quantity */
        public Decimal GetDifferenceQty()
        {
            Object bd = Get_Value("DifferenceQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Ship/Receipt Confirmation.
        @param VAM_Inv_InOutConfirm_ID Material Shipment or Receipt Confirmation */
        public void SetVAM_Inv_InOutConfirm_ID(int VAM_Inv_InOutConfirm_ID)
        {
            if (VAM_Inv_InOutConfirm_ID < 1) throw new ArgumentException("VAM_Inv_InOutConfirm_ID is mandatory.");
            Set_ValueNoCheck("VAM_Inv_InOutConfirm_ID", VAM_Inv_InOutConfirm_ID);
        }
        /** Get Ship/Receipt Confirmation.
        @return Material Shipment or Receipt Confirmation */
        public int GetVAM_Inv_InOutConfirm_ID()
        {
            Object ii = Get_Value("VAM_Inv_InOutConfirm_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Ship/Receipt Confirmation Line.
        @param VAM_Inv_InOutLineConfirm_ID Material Shipment or Receipt Confirmation Line */
        public void SetVAM_Inv_InOutLineConfirm_ID(int VAM_Inv_InOutLineConfirm_ID)
        {
            if (VAM_Inv_InOutLineConfirm_ID < 1) throw new ArgumentException("VAM_Inv_InOutLineConfirm_ID is mandatory.");
            Set_ValueNoCheck("VAM_Inv_InOutLineConfirm_ID", VAM_Inv_InOutLineConfirm_ID);
        }
        /** Get Ship/Receipt Confirmation Line.
        @return Material Shipment or Receipt Confirmation Line */
        public int GetVAM_Inv_InOutLineConfirm_ID()
        {
            Object ii = Get_Value("VAM_Inv_InOutLineConfirm_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shipment/Receipt Line.
        @param VAM_Inv_InOutLine_ID Line on Shipment or Receipt document */
        public void SetVAM_Inv_InOutLine_ID(int VAM_Inv_InOutLine_ID)
        {
            if (VAM_Inv_InOutLine_ID < 1) throw new ArgumentException("VAM_Inv_InOutLine_ID is mandatory.");
            Set_ValueNoCheck("VAM_Inv_InOutLine_ID", VAM_Inv_InOutLine_ID);
        }
        /** Get Shipment/Receipt Line.
        @return Line on Shipment or Receipt document */
        public int GetVAM_Inv_InOutLine_ID()
        {
            Object ii = Get_Value("VAM_Inv_InOutLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetVAM_Inv_InOutLine_ID().ToString());
        }
        /** Set Phys Inventory Line.
        @param VAM_InventoryLine_ID Unique line in an Inventory document */
        public void SetVAM_InventoryLine_ID(int VAM_InventoryLine_ID)
        {
            if (VAM_InventoryLine_ID <= 0) Set_Value("VAM_InventoryLine_ID", null);
            else
                Set_Value("VAM_InventoryLine_ID", VAM_InventoryLine_ID);
        }
        /** Get Phys Inventory Line.
        @return Unique line in an Inventory document */
        public int GetVAM_InventoryLine_ID()
        {
            Object ii = Get_Value("VAM_InventoryLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Scrapped Quantity.
        @param ScrappedQty The Quantity scrapped due to QA issues */
        public void SetScrappedQty(Decimal? ScrappedQty)
        {
            Set_Value("ScrappedQty", (Decimal?)ScrappedQty);
        }
        /** Get Scrapped Quantity.
        @return The Quantity scrapped due to QA issues */
        public Decimal GetScrappedQty()
        {
            Object bd = Get_Value("ScrappedQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Target Quantity.
        @param TargetQty Target Movement Quantity */
        public void SetTargetQty(Decimal? TargetQty)
        {
            if (TargetQty == null) throw new ArgumentException("TargetQty is mandatory.");
            Set_ValueNoCheck("TargetQty", (Decimal?)TargetQty);
        }
        /** Get Target Quantity.
        @return Target Movement Quantity */
        public Decimal GetTargetQty()
        {
            Object bd = Get_Value("TargetQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Locator.
        @param VAM_Locator_ID Warehouse Locator */
        public void SetVAM_Locator_ID(int VAM_Locator_ID)
        {
            if (VAM_Locator_ID <= 0) Set_Value("VAM_Locator_ID", null);
            else
                Set_Value("VAM_Locator_ID", VAM_Locator_ID);
        }
        /** Get Locator.
        @return Warehouse Locator */
        public int GetVAM_Locator_ID()
        {
            Object ii = Get_Value("VAM_Locator_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        //Arpit To Set UOM on Ship/Receipt Confirm Line (Case of UOM Conversion)
        /** Set UOM.
        @param VAB_UOM_ID Unit of Measure */
        public void SetVAB_UOM_ID(int VAB_UOM_ID)
        {
            if (VAB_UOM_ID <= 0) Set_Value("VAB_UOM_ID", null);
            else
                Set_Value("VAB_UOM_ID", VAB_UOM_ID);
        }/** Get UOM.
        @return Unit of Measure */
        public int GetVAB_UOM_ID() { Object ii = Get_Value("VAB_UOM_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }

}

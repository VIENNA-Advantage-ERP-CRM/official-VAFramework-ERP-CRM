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
    /** Generated Model for VAM_ProductionLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_ProductionLine : PO
    {
        public X_VAM_ProductionLine(Context ctx, int VAM_ProductionLine_ID, Trx trxName)
            : base(ctx, VAM_ProductionLine_ID, trxName)
        {
            /** if (VAM_ProductionLine_ID == 0)
            {
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAM_ProductionLine WHERE VAM_ProductionPlan_ID=@VAM_ProductionPlan_ID@
            SetVAM_Locator_ID (0);	// @VAM_Locator_ID@
            SetVAM_Product_ID (0);
            SetVAM_ProductionLine_ID (0);
            SetVAM_ProductionPlan_ID (0);
            SetMovementQty (0.0);
            SetProcessed (false);	// N
            }
             */
        }
        public X_VAM_ProductionLine(Ctx ctx, int VAM_ProductionLine_ID, Trx trxName)
            : base(ctx, VAM_ProductionLine_ID, trxName)
        {
            /** if (VAM_ProductionLine_ID == 0)
            {
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAM_ProductionLine WHERE VAM_ProductionPlan_ID=@VAM_ProductionPlan_ID@
            SetVAM_Locator_ID (0);	// @VAM_Locator_ID@
            SetVAM_Product_ID (0);
            SetVAM_ProductionLine_ID (0);
            SetVAM_ProductionPlan_ID (0);
            SetMovementQty (0.0);
            SetProcessed (false);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductionLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductionLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductionLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_ProductionLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514380980L;
        /** Last Updated Timestamp 7/29/2010 1:07:44 PM */
        public static long updatedMS = 1280389064191L;
        /** VAF_TableView_ID=326 */
        public static int Table_ID;
        // =326;

        /** TableName=VAM_ProductionLine */
        public static String Table_Name = "VAM_ProductionLine";

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
            StringBuilder sb = new StringBuilder("X_VAM_ProductionLine[").Append(Get_ID()).Append("]");
            return sb.ToString();
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
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetLine().ToString());
        }
        /** Set Attribute Set Instance.
        @param VAM_PFeature_SetInstance_ID Product Attribute Set Instance */
        public void SetVAM_PFeature_SetInstance_ID(int VAM_PFeature_SetInstance_ID)
        {
            if (VAM_PFeature_SetInstance_ID <= 0) Set_Value("VAM_PFeature_SetInstance_ID", null);
            else
                Set_Value("VAM_PFeature_SetInstance_ID", VAM_PFeature_SetInstance_ID);
        }
        /** Get Attribute Set Instance.
        @return Product Attribute Set Instance */
        public int GetVAM_PFeature_SetInstance_ID()
        {
            Object ii = Get_Value("VAM_PFeature_SetInstance_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Locator.
        @param VAM_Locator_ID Warehouse Locator */
        public void SetVAM_Locator_ID(int VAM_Locator_ID)
        {
            if (VAM_Locator_ID < 1) throw new ArgumentException("VAM_Locator_ID is mandatory.");
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
        /** Set Product.
        @param VAM_Product_ID Product, Service, Item */
        public void SetVAM_Product_ID(int VAM_Product_ID)
        {
            if (VAM_Product_ID < 1) throw new ArgumentException("VAM_Product_ID is mandatory.");
            Set_Value("VAM_Product_ID", VAM_Product_ID);
        }
        /** Get Product.
        @return Product, Service, Item */
        public int GetVAM_Product_ID()
        {
            Object ii = Get_Value("VAM_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Production Line.
        @param VAM_ProductionLine_ID Document Line representing a production */
        public void SetVAM_ProductionLine_ID(int VAM_ProductionLine_ID)
        {
            if (VAM_ProductionLine_ID < 1) throw new ArgumentException("VAM_ProductionLine_ID is mandatory.");
            Set_ValueNoCheck("VAM_ProductionLine_ID", VAM_ProductionLine_ID);
        }
        /** Get Production Line.
        @return Document Line representing a production */
        public int GetVAM_ProductionLine_ID()
        {
            Object ii = Get_Value("VAM_ProductionLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Production Plan.
        @param VAM_ProductionPlan_ID Plan for how a product is produced */
        public void SetVAM_ProductionPlan_ID(int VAM_ProductionPlan_ID)
        {
            if (VAM_ProductionPlan_ID < 1) throw new ArgumentException("VAM_ProductionPlan_ID is mandatory.");
            Set_ValueNoCheck("VAM_ProductionPlan_ID", VAM_ProductionPlan_ID);
        }
        /** Get Production Plan.
        @return Plan for how a product is produced */
        public int GetVAM_ProductionPlan_ID()
        {
            Object ii = Get_Value("VAM_ProductionPlan_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Movement Quantity.
        @param MovementQty Quantity of a product moved. */
        public void SetMovementQty(Decimal? MovementQty)
        {
            if (MovementQty == null) throw new ArgumentException("MovementQty is mandatory.");
            Set_Value("MovementQty", (Decimal?)MovementQty);
        }
        /** Get Movement Quantity.
        @return Quantity of a product moved. */
        public Decimal GetMovementQty()
        {
            Object bd = Get_Value("MovementQty");
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

        /** Set Planned Quantity.
        @param PlannedQty Planned quantity for this project */
        public void SetPlannedQty(Decimal? PlannedQty) { Set_Value("PlannedQty", (Decimal?)PlannedQty); }/** Get Planned Quantity.
@return Planned quantity for this project */
        public Decimal GetPlannedQty() { Object bd = Get_Value("PlannedQty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set Production.
        @param VAM_Production_ID Plan for producing a product */
        public void SetVAM_Production_ID(int VAM_Production_ID)
        {
            if (VAM_Production_ID <= 0) Set_Value("VAM_Production_ID", null); else Set_Value("VAM_Production_ID", VAM_Production_ID);
        }/** Get Production.
@return Plan for producing a product */
        public int GetVAM_Production_ID() { Object ii = Get_Value("VAM_Production_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** Set Warehouse.
        @param VAM_Warehouse_ID Storage Warehouse and Service Point */
        public void SetVAM_Warehouse_ID(int VAM_Warehouse_ID)
        {
            if (VAM_Warehouse_ID <= 0) Set_Value("VAM_Warehouse_ID", null);
            else
                Set_Value("VAM_Warehouse_ID", VAM_Warehouse_ID);
        }/** Get Warehouse.
@return Storage Warehouse and Service Point */
        public int GetVAM_Warehouse_ID() { Object ii = Get_Value("VAM_Warehouse_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }


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

        /** ReversalDoc_ID VAF_Control_Ref_ID=297 */
        public static int REVERSALDOC_ID_VAF_Control_Ref_ID = 297;
        /** Set Reversal Document.@param ReversalDoc_ID Reversal Document */
        public void SetReversalDoc_ID(int ReversalDoc_ID)
        {
            if (ReversalDoc_ID <= 0) Set_Value("ReversalDoc_ID", null);
            else
                Set_Value("ReversalDoc_ID", ReversalDoc_ID);
        }
        /** Get Reversal Document.@return Reversal Document */
        public int GetReversalDoc_ID() { Object ii = Get_Value("ReversalDoc_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** Set Product Container.@param VAM_ProductContainer_ID Pallet No. / Container Name */
        public void SetVAM_ProductContainer_ID(int VAM_ProductContainer_ID)
        {
            if (VAM_ProductContainer_ID <= 0) Set_Value("VAM_ProductContainer_ID", null);
            else
                Set_Value("VAM_ProductContainer_ID", VAM_ProductContainer_ID);
        }
        /** Get Product Container.@return Pallet No. / Container Name */
        public int GetVAM_ProductContainer_ID() { Object ii = Get_Value("VAM_ProductContainer_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }

}

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
    /** Generated Model for M_ProductionLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_ProductionLine : PO
    {
        public X_M_ProductionLine(Context ctx, int M_ProductionLine_ID, Trx trxName)
            : base(ctx, M_ProductionLine_ID, trxName)
        {
            /** if (M_ProductionLine_ID == 0)
            {
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM M_ProductionLine WHERE M_ProductionPlan_ID=@M_ProductionPlan_ID@
            SetM_Locator_ID (0);	// @M_Locator_ID@
            SetM_Product_ID (0);
            SetM_ProductionLine_ID (0);
            SetM_ProductionPlan_ID (0);
            SetMovementQty (0.0);
            SetProcessed (false);	// N
            }
             */
        }
        public X_M_ProductionLine(Ctx ctx, int M_ProductionLine_ID, Trx trxName)
            : base(ctx, M_ProductionLine_ID, trxName)
        {
            /** if (M_ProductionLine_ID == 0)
            {
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM M_ProductionLine WHERE M_ProductionPlan_ID=@M_ProductionPlan_ID@
            SetM_Locator_ID (0);	// @M_Locator_ID@
            SetM_Product_ID (0);
            SetM_ProductionLine_ID (0);
            SetM_ProductionPlan_ID (0);
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
        public X_M_ProductionLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_ProductionLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_ProductionLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_ProductionLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514380980L;
        /** Last Updated Timestamp 7/29/2010 1:07:44 PM */
        public static long updatedMS = 1280389064191L;
        /** AD_Table_ID=326 */
        public static int Table_ID;
        // =326;

        /** TableName=M_ProductionLine */
        public static String Table_Name = "M_ProductionLine";

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
            StringBuilder sb = new StringBuilder("X_M_ProductionLine[").Append(Get_ID()).Append("]");
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
        /** Set Locator.
        @param M_Locator_ID Warehouse Locator */
        public void SetM_Locator_ID(int M_Locator_ID)
        {
            if (M_Locator_ID < 1) throw new ArgumentException("M_Locator_ID is mandatory.");
            Set_Value("M_Locator_ID", M_Locator_ID);
        }
        /** Get Locator.
        @return Warehouse Locator */
        public int GetM_Locator_ID()
        {
            Object ii = Get_Value("M_Locator_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID < 1) throw new ArgumentException("M_Product_ID is mandatory.");
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
        /** Set Production Line.
        @param M_ProductionLine_ID Document Line representing a production */
        public void SetM_ProductionLine_ID(int M_ProductionLine_ID)
        {
            if (M_ProductionLine_ID < 1) throw new ArgumentException("M_ProductionLine_ID is mandatory.");
            Set_ValueNoCheck("M_ProductionLine_ID", M_ProductionLine_ID);
        }
        /** Get Production Line.
        @return Document Line representing a production */
        public int GetM_ProductionLine_ID()
        {
            Object ii = Get_Value("M_ProductionLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Production Plan.
        @param M_ProductionPlan_ID Plan for how a product is produced */
        public void SetM_ProductionPlan_ID(int M_ProductionPlan_ID)
        {
            if (M_ProductionPlan_ID < 1) throw new ArgumentException("M_ProductionPlan_ID is mandatory.");
            Set_ValueNoCheck("M_ProductionPlan_ID", M_ProductionPlan_ID);
        }
        /** Get Production Plan.
        @return Plan for how a product is produced */
        public int GetM_ProductionPlan_ID()
        {
            Object ii = Get_Value("M_ProductionPlan_ID");
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
        @param M_Production_ID Plan for producing a product */
        public void SetM_Production_ID(int M_Production_ID)
        {
            if (M_Production_ID <= 0) Set_Value("M_Production_ID", null); else Set_Value("M_Production_ID", M_Production_ID);
        }/** Get Production.
@return Plan for producing a product */
        public int GetM_Production_ID() { Object ii = Get_Value("M_Production_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** Set Warehouse.
        @param M_Warehouse_ID Storage Warehouse and Service Point */
        public void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (M_Warehouse_ID <= 0) Set_Value("M_Warehouse_ID", null);
            else
                Set_Value("M_Warehouse_ID", M_Warehouse_ID);
        }/** Get Warehouse.
@return Storage Warehouse and Service Point */
        public int GetM_Warehouse_ID() { Object ii = Get_Value("M_Warehouse_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }


        /** Set UOM.
        @param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID <= 0) Set_Value("C_UOM_ID", null);
            else
                Set_Value("C_UOM_ID", C_UOM_ID);
        }/** Get UOM.
@return Unit of Measure */
        public int GetC_UOM_ID() { Object ii = Get_Value("C_UOM_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** ReversalDoc_ID AD_Reference_ID=297 */
        public static int REVERSALDOC_ID_AD_Reference_ID = 297;
        /** Set Reversal Document.@param ReversalDoc_ID Reversal Document */
        public void SetReversalDoc_ID(int ReversalDoc_ID)
        {
            if (ReversalDoc_ID <= 0) Set_Value("ReversalDoc_ID", null);
            else
                Set_Value("ReversalDoc_ID", ReversalDoc_ID);
        }
        /** Get Reversal Document.@return Reversal Document */
        public int GetReversalDoc_ID() { Object ii = Get_Value("ReversalDoc_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** Set Product Container.@param M_ProductContainer_ID Pallet No. / Container Name */
        public void SetM_ProductContainer_ID(int M_ProductContainer_ID)
        {
            if (M_ProductContainer_ID <= 0) Set_Value("M_ProductContainer_ID", null);
            else
                Set_Value("M_ProductContainer_ID", M_ProductContainer_ID);
        }
        /** Get Product Container.@return Pallet No. / Container Name */
        public int GetM_ProductContainer_ID() { Object ii = Get_Value("M_ProductContainer_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }

}

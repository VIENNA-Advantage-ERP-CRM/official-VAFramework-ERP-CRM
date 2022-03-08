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
    /** Generated Model for M_Warehouse
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_Warehouse : PO
    {
        public X_M_Warehouse(Context ctx, int M_Warehouse_ID, Trx trxName)
            : base(ctx, M_Warehouse_ID, trxName)
        {
            /** if (M_Warehouse_ID == 0)
            {
            SetC_Location_ID (0);
            SetIsDisallowNegativeInv (false);	// N
            SetIsWMSEnabled (false);	// N
            SetM_Warehouse_ID (0);
            SetName (null);
            SetSeparator (null);	// *
            SetValue (null);
            }
             */
        }
        public X_M_Warehouse(Ctx ctx, int M_Warehouse_ID, Trx trxName)
            : base(ctx, M_Warehouse_ID, trxName)
        {
            /** if (M_Warehouse_ID == 0)
            {
            SetC_Location_ID (0);
            SetIsDisallowNegativeInv (false);	// N
            SetIsWMSEnabled (false);	// N
            SetM_Warehouse_ID (0);
            SetName (null);
            SetSeparator (null);	// *
            SetValue (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Warehouse(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Warehouse(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Warehouse(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_Warehouse()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27721596375150L;
        /** Last Updated Timestamp 8/13/2015 6:34:18 PM */
        public static long updatedMS = 1439471058361L;
        /** AD_Table_ID=190 */
        public static int Table_ID;
        // =190;

        /** TableName=M_Warehouse */
        public static String Table_Name = "M_Warehouse";

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
            StringBuilder sb = new StringBuilder("X_M_Warehouse[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Address.
        @param C_Location_ID Location or Address */
        public void SetC_Location_ID(int C_Location_ID)
        {
            if (C_Location_ID < 1) throw new ArgumentException("C_Location_ID is mandatory.");
            Set_Value("C_Location_ID", C_Location_ID);
        }
        /** Get Address.
        @return Location or Address */
        public int GetC_Location_ID()
        {
            Object ii = Get_Value("C_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** DTD001_ReqFulRule AD_Reference_ID=1000310 */
        public static int DTD001_REQFULRULE_AD_Reference_ID = 1000310;
        /** Full = F */
        public static String DTD001_REQFULRULE_Full = "F";
        /** Partial = P */
        public static String DTD001_REQFULRULE_Partial = "P";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDTD001_ReqFulRuleValid(String test)
        {
            return test == null || test.Equals("F") || test.Equals("P");
        }
        /** Set Requisition fulfillmentRule.
        @param DTD001_ReqFulRule Requisition fulfillmentRule */
        public void SetDTD001_ReqFulRule(String DTD001_ReqFulRule)
        {
            if (!IsDTD001_ReqFulRuleValid(DTD001_ReqFulRule))
                throw new ArgumentException("DTD001_ReqFulRule Invalid value - " + DTD001_ReqFulRule + " - Reference_ID=1000310 - F - P");
            if (DTD001_ReqFulRule != null && DTD001_ReqFulRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                DTD001_ReqFulRule = DTD001_ReqFulRule.Substring(0, 1);
            }
            Set_Value("DTD001_ReqFulRule", DTD001_ReqFulRule);
        }
        /** Get Requisition fulfillmentRule.
        @return Requisition fulfillmentRule */
        public String GetDTD001_ReqFulRule()
        {
            return (String)Get_Value("DTD001_ReqFulRule");
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
        /** Set Export.
        @param Export_ID Export */
        public void SetExport_ID(String Export_ID)
        {
            if (Export_ID != null && Export_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Export_ID = Export_ID.Substring(0, 50);
            }
            Set_ValueNoCheck("Export_ID", Export_ID);
        }
        /** Get Export.
        @return Export */
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }
        /** Set Backward Flow.
        @param IsBackwardFlow Backward Flow */
        public void SetIsBackwardFlow(Boolean IsBackwardFlow)
        {
            Set_Value("IsBackwardFlow", IsBackwardFlow);
        }
        /** Get Backward Flow.
        @return Backward Flow */
        public Boolean IsBackwardFlow()
        {
            Object oo = Get_Value("IsBackwardFlow");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Disallow Negative Inventory.
        @param IsDisallowNegativeInv Disallow Negative Inventory */
        public void SetIsDisallowNegativeInv(Boolean IsDisallowNegativeInv)
        {
            Set_Value("IsDisallowNegativeInv", IsDisallowNegativeInv);
        }
        /** Get Disallow Negative Inventory.
        @return Disallow Negative Inventory */
        public Boolean IsDisallowNegativeInv()
        {
            Object oo = Get_Value("IsDisallowNegativeInv");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set WMS Enabled.
        @param IsWMSEnabled WMS Enabled */
        public void SetIsWMSEnabled(Boolean IsWMSEnabled)
        {
            Set_Value("IsWMSEnabled", IsWMSEnabled);
        }
        /** Get WMS Enabled.
        @return WMS Enabled */
        public Boolean IsWMSEnabled()
        {
            Object oo = Get_Value("IsWMSEnabled");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** M_RcvLocator_ID AD_Reference_ID=1000044 */
        public static int M_RCVLOCATOR_ID_AD_Reference_ID = 1000044;
        /** Set Receiving Locator.
        @param M_RcvLocator_ID Receiving Locator */
        public void SetM_RcvLocator_ID(int M_RcvLocator_ID)
        {
            if (M_RcvLocator_ID <= 0) Set_Value("M_RcvLocator_ID", null);
            else
                Set_Value("M_RcvLocator_ID", M_RcvLocator_ID);
        }
        /** Get Receiving Locator.
        @return Receiving Locator */
        public int GetM_RcvLocator_ID()
        {
            Object ii = Get_Value("M_RcvLocator_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** M_StgLocator_ID AD_Reference_ID=1000044 */
        public static int M_STGLOCATOR_ID_AD_Reference_ID = 1000044;
        /** Set Staging Locator.
        @param M_StgLocator_ID Staging Locator */
        public void SetM_StgLocator_ID(int M_StgLocator_ID)
        {
            if (M_StgLocator_ID <= 0) Set_Value("M_StgLocator_ID", null);
            else
                Set_Value("M_StgLocator_ID", M_StgLocator_ID);
        }
        /** Get Staging Locator.
        @return Staging Locator */
        public int GetM_StgLocator_ID()
        {
            Object ii = Get_Value("M_StgLocator_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** M_WarehouseSource_ID AD_Reference_ID=197 */
        public static int M_WAREHOUSESOURCE_ID_AD_Reference_ID = 197;
        /** Set Source Warehouse.
        @param M_WarehouseSource_ID Optional Warehouse to replenish from */
        public void SetM_WarehouseSource_ID(int M_WarehouseSource_ID)
        {
            if (M_WarehouseSource_ID <= 0) Set_Value("M_WarehouseSource_ID", null);
            else
                Set_Value("M_WarehouseSource_ID", M_WarehouseSource_ID);
        }
        /** Get Source Warehouse.
        @return Optional Warehouse to replenish from */
        public int GetM_WarehouseSource_ID()
        {
            Object ii = Get_Value("M_WarehouseSource_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Warehouse.
        @param M_Warehouse_ID Storage Warehouse and Service Point */
        public void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (M_Warehouse_ID < 1) throw new ArgumentException("M_Warehouse_ID is mandatory.");
            Set_ValueNoCheck("M_Warehouse_ID", M_Warehouse_ID);
        }
        /** Get Warehouse.
        @return Storage Warehouse and Service Point */
        public int GetM_Warehouse_ID()
        {
            Object ii = Get_Value("M_Warehouse_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name == null) throw new ArgumentException("Name is mandatory.");
            if (Name.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Name = Name.Substring(0, 60);
            }
            Set_Value("Name", Name);
        }
        /** Get Name.
        @return Alphanumeric identifier of the entity */
        public String GetName()
        {
            return (String)Get_Value("Name");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetName());
        }
        /** Set Replenishment Class.
        @param ReplenishmentClass Custom class to calculate Quantity to Order */
        public void SetReplenishmentClass(String ReplenishmentClass)
        {
            if (ReplenishmentClass != null && ReplenishmentClass.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                ReplenishmentClass = ReplenishmentClass.Substring(0, 60);
            }
            Set_Value("ReplenishmentClass", ReplenishmentClass);
        }
        /** Get Replenishment Class.
        @return Custom class to calculate Quantity to Order */
        public String GetReplenishmentClass()
        {
            return (String)Get_Value("ReplenishmentClass");
        }
        /** Set Element Separator.
        @param Separator Element Separator */
        public void SetSeparator(String Separator)
        {
            if (Separator == null) throw new ArgumentException("Separator is mandatory.");
            if (Separator.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                Separator = Separator.Substring(0, 1);
            }
            Set_Value("Separator", Separator);
        }
        /** Get Element Separator.
        @return Element Separator */
        public String GetSeparator()
        {
            return (String)Get_Value("Separator");
        }
        /** Set Search Key.
        @param Value Search key for the record in the format required - must be unique */
        public void SetValue(String Value)
        {
            if (Value == null) throw new ArgumentException("Value is mandatory.");
            if (Value.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                Value = Value.Substring(0, 40);
            }
            Set_Value("Value", Value);
        }
        /** Get Search Key.
        @return Search key for the record in the format required - must be unique */
        public String GetValue()
        {
            return (String)Get_Value("Value");
        }
        /** Set Drop Shipment.
        @param IsDropShip Drop Shipments are sent from the Vendor directly to the Customer */
        public void SetIsDropShip(Boolean IsDropShip)
        {
            Set_Value("IsDropShip", IsDropShip);
        }
        /** Get Drop Shipment.
        @return Drop Shipments are sent from the Vendor directly to the Customer */
        public Boolean IsDropShip()
        {
            Object oo = Get_Value("IsDropShip");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);
            }
            return false;
        }
    }

}

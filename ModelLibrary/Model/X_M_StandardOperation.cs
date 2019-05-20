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
    /** Generated Model for M_StandardOperation
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_StandardOperation : PO
    {
        public X_M_StandardOperation(Context ctx, int M_StandardOperation_ID, Trx trxName)
            : base(ctx, M_StandardOperation_ID, trxName)
        {
            /** if (M_StandardOperation_ID == 0)
            {
            SetC_UOM_ID (0);	// @SQL=SELECT C_UOM_ID FROM C_UOM WHERE Name='Day'
            SetIsHazmat (false);	// N
            SetIsPermitRequired (false);	// N
            SetM_Operation_ID (0);
            SetM_StandardOperation_ID (0);
            SetM_Warehouse_ID (0);
            SetM_WorkCenter_ID (0);
            SetSetupTime (0.0);	// 0
            SetUnitRuntime (0.0);	// 0
            }
             */
        }
        public X_M_StandardOperation(Ctx ctx, int M_StandardOperation_ID, Trx trxName)
            : base(ctx, M_StandardOperation_ID, trxName)
        {
            /** if (M_StandardOperation_ID == 0)
            {
            SetC_UOM_ID (0);	// @SQL=SELECT C_UOM_ID FROM C_UOM WHERE Name='Day'
            SetIsHazmat (false);	// N
            SetIsPermitRequired (false);	// N
            SetM_Operation_ID (0);
            SetM_StandardOperation_ID (0);
            SetM_Warehouse_ID (0);
            SetM_WorkCenter_ID (0);
            SetSetupTime (0.0);	// 0
            SetUnitRuntime (0.0);	// 0
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_StandardOperation(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_StandardOperation(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_StandardOperation(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_StandardOperation()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27581088221842L;
        /** Last Updated Timestamp 3/1/2011 12:31:45 PM */
        public static long updatedMS = 1298962905053L;
        /** AD_Table_ID=2089 */
        public static int Table_ID;
        // =2089;

        /** TableName=M_StandardOperation */
        public static String Table_Name = "M_StandardOperation";

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
            StringBuilder sb = new StringBuilder("X_M_StandardOperation[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set UOM.
        @param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID < 1) throw new ArgumentException("C_UOM_ID is mandatory.");
            Set_Value("C_UOM_ID", C_UOM_ID);
        }
        /** Get UOM.
        @return Unit of Measure */
        public int GetC_UOM_ID()
        {
            Object ii = Get_Value("C_UOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Comment.
        @param Help Comment, Help or Hint */
        public void SetHelp(String Help)
        {
            if (Help != null && Help.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                Help = Help.Substring(0, 2000);
            }
            Set_Value("Help", Help);
        }
        /** Get Comment.
        @return Comment, Help or Hint */
        public String GetHelp()
        {
            return (String)Get_Value("Help");
        }
        /** Set Hazmat.
        @param IsHazmat Involves hazardous materials */
        public void SetIsHazmat(Boolean IsHazmat)
        {
            Set_Value("IsHazmat", IsHazmat);
        }
        /** Get Hazmat.
        @return Involves hazardous materials */
        public Boolean IsHazmat()
        {
            Object oo = Get_Value("IsHazmat");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Permit Required.
        @param IsPermitRequired Indicates if a permit or similar authorization is required for use or execution of a product, resource or work order operation. */
        public void SetIsPermitRequired(Boolean IsPermitRequired)
        {
            Set_Value("IsPermitRequired", IsPermitRequired);
        }
        /** Get Permit Required.
        @return Indicates if a permit or similar authorization is required for use or execution of a product, resource or work order operation. */
        public Boolean IsPermitRequired()
        {
            Object oo = Get_Value("IsPermitRequired");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Operation.
        @param M_Operation_ID Manufacturing Operation */
        public void SetM_Operation_ID(int M_Operation_ID)
        {
            if (M_Operation_ID < 1) throw new ArgumentException("M_Operation_ID is mandatory.");
            Set_ValueNoCheck("M_Operation_ID", M_Operation_ID);
        }
        /** Get Operation.
        @return Manufacturing Operation */
        public int GetM_Operation_ID()
        {
            Object ii = Get_Value("M_Operation_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetM_Operation_ID().ToString());
        }
        /** Set Standard Operation.
        @param M_StandardOperation_ID Identifies a standard operation template */
        public void SetM_StandardOperation_ID(int M_StandardOperation_ID)
        {
            if (M_StandardOperation_ID < 1) throw new ArgumentException("M_StandardOperation_ID is mandatory.");
            Set_ValueNoCheck("M_StandardOperation_ID", M_StandardOperation_ID);
        }
        /** Get Standard Operation.
        @return Identifies a standard operation template */
        public int GetM_StandardOperation_ID()
        {
            Object ii = Get_Value("M_StandardOperation_ID");
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
        /** Set Work Center.
        @param M_WorkCenter_ID Identifies a production area within a warehouse consisting of people and equipment */
        public void SetM_WorkCenter_ID(int M_WorkCenter_ID)
        {
            if (M_WorkCenter_ID < 1) throw new ArgumentException("M_WorkCenter_ID is mandatory.");
            Set_ValueNoCheck("M_WorkCenter_ID", M_WorkCenter_ID);
        }
        /** Get Work Center.
        @return Identifies a production area within a warehouse consisting of people and equipment */
        public int GetM_WorkCenter_ID()
        {
            Object ii = Get_Value("M_WorkCenter_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Setup Time.
        @param SetupTime Setup time before starting Production */
        public void SetSetupTime(Decimal? SetupTime)
        {
            if (SetupTime == null) throw new ArgumentException("SetupTime is mandatory.");
            Set_Value("SetupTime", (Decimal?)SetupTime);
        }
        /** Get Setup Time.
        @return Setup time before starting Production */
        public Decimal GetSetupTime()
        {
            Object bd = Get_Value("SetupTime");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Runtime per Unit.
        @param UnitRuntime Time to produce one unit */
        public void SetUnitRuntime(Decimal? UnitRuntime)
        {
            if (UnitRuntime == null) throw new ArgumentException("UnitRuntime is mandatory.");
            Set_Value("UnitRuntime", (Decimal?)UnitRuntime);
        }
        /** Get Runtime per Unit.
        @return Time to produce one unit */
        public Decimal GetUnitRuntime()
        {
            Object bd = Get_Value("UnitRuntime");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
    }

}

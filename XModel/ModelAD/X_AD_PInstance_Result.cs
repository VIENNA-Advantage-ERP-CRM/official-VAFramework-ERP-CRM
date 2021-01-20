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
    using System.Data;/** Generated Model for VAF_JInstance_Result
 *  @author Vienna Solutions 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_JInstance_Result : PO
    {
        public X_VAF_JInstance_Result(Context ctx, int VAF_JInstance_Result_ID, Trx trxName)
            : base(ctx, VAF_JInstance_Result_ID, trxName)
        {
            /** if (VAF_JInstance_Result_ID == 0){SetVAF_JInstance_ID (0);SetVAF_JInstance_Result_ID (0);} */
        }
        public X_VAF_JInstance_Result(Ctx ctx, int VAF_JInstance_Result_ID, Trx trxName)
            : base(ctx, VAF_JInstance_Result_ID, trxName)
        {
            /** if (VAF_JInstance_Result_ID == 0){SetVAF_JInstance_ID (0);SetVAF_JInstance_Result_ID (0);} */
        }

        ///<summary>
        /// Load Constructor
        ///</summary>
        ///<param name="ctx"> Context </param>
        ///<param name="rs"> Result Set </param>
        ///<param name="trxName"> Transaction </param>
        public X_VAF_JInstance_Result(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        ///<summary>
        /// Load Constructor
        ///</summary>
        ///<param name="ctx"> Context </param>
        ///<param name="rs"> Result Set </param>
        ///<param name="trxName"> Transaction </param>
        public X_VAF_JInstance_Result(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        ///<summary>
        /// Load Constructor
        ///</summary>
        ///<param name="ctx"> Context </param>
        ///<param name="rs"> Result Set </param>
        ///<param name="trxName"> Transaction </param>
        public X_VAF_JInstance_Result(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        ///<summary>
        /// Static Constructor 
        /// Set Table ID By Table Name
        ///</summary>
        static X_VAF_JInstance_Result()
        {
            Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name);
        }

        ///<summary>
        /// Serial Version No.
        ///</summary>
        static long serialVersionUID = 27841689092615L;

        ///<summary>
        /// Last Updated Timestamp 6/3/2019 5:39:35 PM
        ///</summary>
        public static long updatedMS = 1559563775826L;

        ///<summary>
        /// VAF_TableView_ID=1000845
        ///</summary>
        public static int Table_ID; // =1000845;

        ///<summary>
        /// TableName=VAF_JInstance_Result
        ///</summary>
        public static String Table_Name = "VAF_JInstance_Result";
        protected static KeyNamePair model;

        protected Decimal accessLevel = new Decimal(6);
        ///<summary>
        /// AccessLevel 
        ///</summary>
        ///<returns>6 - System - Client </returns>
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }

        ///<summary>
        /// Load Meta Data
        ///</summary>
        ///<param name="ctx"> Context </param>
        ///<returns> PO Info </returns>
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi;
        }

        ///<summary>
        /// Load Meta Data
        ///</summary>
        ///<param name="ctx"> Context </param>
        ///<returns> PO Info </returns>
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi;
        }

        ///<returns>Info </returns>
        ///<summary>
        /// Info 
        ///</summary>
        ///<returns> Info </returns>

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_VAF_JInstance_Result[").Append(Get_ID()).Append("]"); return sb.ToString();
        }

        ///<summary>
        /// SetProcess Instance
        ///</summary>
        ///<param name="VAF_JInstance_ID">Instance of the process</param>
        public void SetVAF_JInstance_ID(int VAF_JInstance_ID)
        {
            if (VAF_JInstance_ID < 1) throw new ArgumentException("VAF_JInstance_ID is mandatory.");
            Set_ValueNoCheck("VAF_JInstance_ID", VAF_JInstance_ID);
        }

        ///<summary>
        /// GetProcess Instance
        ///</summary>
        ///<returns> Instance of the process</returns>
        public int GetVAF_JInstance_ID()
        {
            Object ii = Get_Value("VAF_JInstance_ID");
            if (ii == null) return 0; return Convert.ToInt32(ii);
        }

        ///<summary>
        /// SetVAF_JInstance_Result_ID
        ///</summary>
        ///<param name="VAF_JInstance_Result_ID">VAF_JInstance_Result_ID</param>
        public void SetVAF_JInstance_Result_ID(int VAF_JInstance_Result_ID)
        {
            if (VAF_JInstance_Result_ID < 1) throw new ArgumentException("VAF_JInstance_Result_ID is mandatory.");
            Set_ValueNoCheck("VAF_JInstance_Result_ID", VAF_JInstance_Result_ID);
        }

        ///<summary>
        /// GetVAF_JInstance_Result_ID
        ///</summary>
        ///<returns> VAF_JInstance_Result_ID</returns>
        public int GetVAF_JInstance_Result_ID()
        {
            Object ii = Get_Value("VAF_JInstance_Result_ID");
            if (ii == null) return 0; return Convert.ToInt32(ii);
        }

        ///<summary>
        /// SetExport
        ///</summary>
        ///<param name="Export_ID">Export</param>
        public void SetExport_ID(String Export_ID)
        {
            if (Export_ID != null && Export_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Export_ID = Export_ID.Substring(0, 50);
            }
            Set_Value("Export_ID", Export_ID);
        }

        ///<summary>
        /// GetExport
        ///</summary>
        ///<returns> Export</returns>
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }

        ///<summary>
        /// SetResult
        ///</summary>
        ///<param name="Result">Result of the action taken</param>
        public void SetResult(String Result)
        {
            Set_Value("Result", Result);
        }

        ///<summary>
        /// GetResult
        ///</summary>
        ///<returns> Result of the action taken</returns>
        public String GetResult()
        {
            return (String)Get_Value("Result");
        }
    }
}
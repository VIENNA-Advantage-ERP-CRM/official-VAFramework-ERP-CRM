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
    using System.Data;/** Generated Model for AD_PInstance_Result
 *  @author Vienna Solutions 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_PInstance_Result : PO
    {
        public X_AD_PInstance_Result(Context ctx, int AD_PInstance_Result_ID, Trx trxName)
            : base(ctx, AD_PInstance_Result_ID, trxName)
        {
            /** if (AD_PInstance_Result_ID == 0){SetAD_PInstance_ID (0);SetAD_PInstance_Result_ID (0);} */
        }
        public X_AD_PInstance_Result(Ctx ctx, int AD_PInstance_Result_ID, Trx trxName)
            : base(ctx, AD_PInstance_Result_ID, trxName)
        {
            /** if (AD_PInstance_Result_ID == 0){SetAD_PInstance_ID (0);SetAD_PInstance_Result_ID (0);} */
        }

        ///<summary>
        /// Load Constructor
        ///</summary>
        ///<param name="ctx"> Context </param>
        ///<param name="rs"> Result Set </param>
        ///<param name="trxName"> Transaction </param>
        public X_AD_PInstance_Result(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        ///<summary>
        /// Load Constructor
        ///</summary>
        ///<param name="ctx"> Context </param>
        ///<param name="rs"> Result Set </param>
        ///<param name="trxName"> Transaction </param>
        public X_AD_PInstance_Result(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        ///<summary>
        /// Load Constructor
        ///</summary>
        ///<param name="ctx"> Context </param>
        ///<param name="rs"> Result Set </param>
        ///<param name="trxName"> Transaction </param>
        public X_AD_PInstance_Result(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        ///<summary>
        /// Static Constructor 
        /// Set Table ID By Table Name
        ///</summary>
        static X_AD_PInstance_Result()
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
        /// AD_Table_ID=1000845
        ///</summary>
        public static int Table_ID; // =1000845;

        ///<summary>
        /// TableName=AD_PInstance_Result
        ///</summary>
        public static String Table_Name = "AD_PInstance_Result";
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
            StringBuilder sb = new StringBuilder("X_AD_PInstance_Result[").Append(Get_ID()).Append("]"); return sb.ToString();
        }

        ///<summary>
        /// SetProcess Instance
        ///</summary>
        ///<param name="AD_PInstance_ID">Instance of the process</param>
        public void SetAD_PInstance_ID(int AD_PInstance_ID)
        {
            if (AD_PInstance_ID < 1) throw new ArgumentException("AD_PInstance_ID is mandatory.");
            Set_ValueNoCheck("AD_PInstance_ID", AD_PInstance_ID);
        }

        ///<summary>
        /// GetProcess Instance
        ///</summary>
        ///<returns> Instance of the process</returns>
        public int GetAD_PInstance_ID()
        {
            Object ii = Get_Value("AD_PInstance_ID");
            if (ii == null) return 0; return Convert.ToInt32(ii);
        }

        ///<summary>
        /// SetAD_PInstance_Result_ID
        ///</summary>
        ///<param name="AD_PInstance_Result_ID">AD_PInstance_Result_ID</param>
        public void SetAD_PInstance_Result_ID(int AD_PInstance_Result_ID)
        {
            if (AD_PInstance_Result_ID < 1) throw new ArgumentException("AD_PInstance_Result_ID is mandatory.");
            Set_ValueNoCheck("AD_PInstance_Result_ID", AD_PInstance_Result_ID);
        }

        ///<summary>
        /// GetAD_PInstance_Result_ID
        ///</summary>
        ///<returns> AD_PInstance_Result_ID</returns>
        public int GetAD_PInstance_Result_ID()
        {
            Object ii = Get_Value("AD_PInstance_Result_ID");
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
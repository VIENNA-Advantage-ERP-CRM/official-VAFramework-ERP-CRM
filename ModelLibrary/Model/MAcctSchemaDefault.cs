/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAcctSchemaDefault
 * Purpose        : Default Accounts for MAcctSchema
 * Class Used     : X_C_AcctSchema_Default
 * Chronological    Development
 * Deepak           23-Nov-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Data.SqlClient;

namespace VAdvantage.Model
{
    public class MAcctSchemaDefault : X_C_AcctSchema_Default
    {
        /**	Logger							*/
        protected static VLogger _log = VLogger.GetVLogger(typeof(MAcctSchemaDefault).FullName);

        /// <summary>
        /// 	Get Accounting Schema Default Info
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_AcctSchema_ID">id</param>
        /// <returns>defaults</returns>
        public static MAcctSchemaDefault Get(Ctx ctx, int C_AcctSchema_ID)
        {
            MAcctSchemaDefault retValue = null;
            String sql = "SELECT * FROM C_AcctSchema_Default WHERE C_AcctSchema_ID=@Param1";
            SqlParameter[] Param = new SqlParameter[1];
            IDataReader idr = null;
            DataTable dt = null;
            //PreparedStatement pstmt = null;
            try
            {
                Param[0] = new SqlParameter("@Param1", C_AcctSchema_ID);
                //pstmt = DataBase.prepareStatement(sql, null);
                //pstmt.setInt(1, C_AcctSchema_ID);
                //ResultSet rs = pstmt.executeQuery();
                idr = DataBase.DB.ExecuteReader(sql, Param, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MAcctSchemaDefault(ctx, dr, null);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            return retValue;
        }	//	get

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_AcctSchema_ID">parent</param>
        /// <param name="trxName">transaction</param>
        public MAcctSchemaDefault(Ctx ctx, int C_AcctSchema_ID, Trx trxName)
            : base(ctx, C_AcctSchema_ID, trxName)
        {
            //super(ctx, C_AcctSchema_ID, trxName);
        }	

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MAcctSchemaDefault(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	

        /// <summary>
        /// Get Realized Gain Acct for currency
        /// </summary>
        /// <param name="C_Currency_ID">currency</param>
        /// <returns>gain acct</returns>
        public int GetRealizedGain_Acct(int C_Currency_ID)
        {
            MCurrencyAcct acct = MCurrencyAcct.Get(this, C_Currency_ID);
            if (acct != null)
            {
                return acct.GetRealizedGain_Acct();
            }
            return base.GetRealizedGain_Acct();
        }	

        /// <summary>
        /// Get Realized Loss Acct for currency 
        /// </summary>
        /// <param name="C_Currency_ID">currency</param>
        /// <returns>loss acct</returns>
        public int GetRealizedLoss_Acct(int C_Currency_ID)
        {
            MCurrencyAcct acct = MCurrencyAcct.Get(this, C_Currency_ID);
            if (acct != null)
            {
                return acct.GetRealizedLoss_Acct();
            }
            return base.GetRealizedLoss_Acct();
        }	

        /// <summary>
        /// 	Get Acct Info list 
        /// </summary>
        /// <returns>list</returns>
        public List<KeyNamePair> GetAcctInfo()
        {
            //ArrayList<KeyNamePair> list = new ArrayList<KeyNamePair>();
            List<KeyNamePair> list = new List<KeyNamePair>();
            for (int i = 0; i < Get_ColumnCount(); i++)
            {
                String columnName = Get_ColumnName(i);
                if (columnName.EndsWith("Acct"))
                {
                    int id = Utility.Util.GetValueOfInt(Get_Value(i));
                    list.Add(new KeyNamePair(id, columnName));
                }
            }
            return list;
        }	

        /// <summary>
        /// Set Value (don't use)
        /// </summary>
        /// <param name="columnName">column name</param>
        /// <param name="value">value</param>
        /// <returns>true if value set</returns>
        public Boolean SetValue(String columnName, int value)
        {
            return base.Set_Value(columnName, value);
        }	

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>true</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            if (GetAD_Org_ID() != 0)
            {
                SetAD_Org_ID(0);
            }
            return true;
        }	

    }

}

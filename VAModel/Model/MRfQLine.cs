/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRfQLine
 * Purpose        : RfQ Line
 * Class Used     : X_C_RfQLine
 * Chronological    Development
 * Raghunandan     10-Aug.-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MRfQLine : X_C_RfQLine
    {
        //Cache		
        private static CCache<int, MRfQLine> s_cache = new CCache<int, MRfQLine>("C_RfQLine", 20);
        //Qyantities	
        private MRfQLineQty[] _qtys = null;

        /// <summary>
        /// Get MRfQLine from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_RfQLine_ID">ID</param>
        /// <param name="trxName">Transaction</param>
        /// <returns>MRFQLINE</returns>
        public static MRfQLine Get(Ctx ctx, int C_RfQLine_ID, Trx trxName)
        {
            int key = C_RfQLine_ID;
            MRfQLine retValue = (MRfQLine)s_cache[key];
            if (retValue != null)
            {
                return retValue;
            }
            retValue = new MRfQLine(ctx, C_RfQLine_ID, trxName);
            if (retValue.Get_ID() != 0)
            {
                s_cache.Add(key, retValue);
            }
            return retValue;
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_RfQLine_ID">id</param>
        /// <param name="trxName">transction</param>
        public MRfQLine(Ctx ctx, int C_RfQLine_ID, Trx trxName)
            : base(ctx, C_RfQLine_ID, trxName)
        {
            if (C_RfQLine_ID == 0)
            {
                SetLine(0);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transction</param>
        public MRfQLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            if (Get_ID() > 0)
            {
                s_cache.Add(Get_ID(), this);
            }
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="rfq">RFQ</param>
        public MRfQLine(MRfQ rfq)
            : this(rfq.GetCtx(), 0, rfq.Get_TrxName())
        {

            SetClientOrg(rfq);
            SetC_RfQ_ID(rfq.GetC_RfQ_ID());
        }

        /// <summary>
        /// Get Quantities
        /// </summary>
        /// <returns>array of quantities</returns>
        public MRfQLineQty[] GetQtys()
        {
            return GetQtys(false);
        }

        /// <summary>
        /// Get Quantities
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>array of quantities</returns>
        public MRfQLineQty[] GetQtys(bool requery)
        {
            if (_qtys != null && !requery)
            {
                return _qtys;
            }
            List<MRfQLineQty> list = new List<MRfQLineQty>();
            String sql = "SELECT * FROM C_RfQLineQty "
                + "WHERE C_RfQLine_ID=@param1 AND IsActive='Y' "
                + "ORDER BY Qty";
            DataTable dt = null;
            IDataReader idr = null;
            SqlParameter[] param = null;
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@param1", GetC_RfQLine_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)// while (dr.next())
                {
                    list.Add(new MRfQLineQty(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }

            //	Create Default (1)
            if (list.Count == 0)
            {
                MRfQLineQty qty = new MRfQLineQty(this);
                qty.Save();
                list.Add(qty);
            }

            _qtys = new MRfQLineQty[list.Count];
            _qtys = list.ToArray();
            return _qtys;
        }

        /// <summary>
        /// Get Product Details
        /// </summary>
        /// <returns>Product Name, etc.</returns>
        public String GetProductDetailHTML()
        {
            if (GetM_Product_ID() == 0)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            MProduct product = MProduct.Get(GetCtx(), GetM_Product_ID());
            sb.Append(product.GetName());
            if (product.GetDescription() != null && product.GetDescription().Length > 0)
            {
                sb.Append("<br><i>").Append(product.GetDescription()).Append("</i>");
            }
            return sb.ToString();
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRfQLine[");
            sb.Append(Get_ID()).Append(",").Append(GetLine())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Calculate Complete Date (also used to verify)
            if (GetDateWorkStart() != null && GetDeliveryDays() != 0)
            {
                SetDateWorkComplete(TimeUtil.AddDays(GetDateWorkStart(), GetDeliveryDays()));
            }
            //	Calculate Delivery Days
            else if (GetDateWorkStart() != null && GetDeliveryDays() == 0 && GetDateWorkComplete() != null)
            {
                SetDeliveryDays(TimeUtil.GetDaysBetween((DateTime?)GetDateWorkStart(), (DateTime?)GetDateWorkComplete()));
            }
            //	Calculate Start Date
            else if (GetDateWorkStart() == null && GetDeliveryDays() != 0 && GetDateWorkComplete() != null)
            {
                SetDateWorkStart(TimeUtil.AddDays(GetDateWorkComplete(), GetDeliveryDays() * -1));
            }

            return true;
        }
    }
}

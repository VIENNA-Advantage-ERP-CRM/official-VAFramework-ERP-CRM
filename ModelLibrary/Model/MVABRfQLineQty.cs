/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRfQLineQty
 * Purpose        : RfQ Line Qty Model
 * Class Used     : X_VAB_RFQLine_Qty
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
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVABRfQLineQty : X_VAB_RFQLine_Qty
    {
        //	Cache	
        private static CCache<int, MVABRfQLineQty> s_cache = new CCache<int, MVABRfQLineQty>("VAB_RFQLine_Qty", 20);
        //Unit of Measure		
        private MUOM _uom = null;

        /// <summary>
        /// Get MRfQLineQty from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_RFQLine_Qty_ID">ID</param>
        /// <param name="trxName">Transaction</param>
        /// <returns>MRfQLineQty</returns>
        public static MVABRfQLineQty Get(Ctx ctx, int VAB_RFQLine_Qty_ID, Trx trxName)
        {
            int key = VAB_RFQLine_Qty_ID;
            MVABRfQLineQty retValue = (MVABRfQLineQty)s_cache[key];
            if (retValue != null)
            {
                return retValue;
            }
            retValue = new MVABRfQLineQty(ctx, VAB_RFQLine_Qty_ID, trxName);
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
        /// <param name="VAB_RFQLine_Qty_ID">ID</param>
        /// <param name="trxName">transction</param>
        public MVABRfQLineQty(Ctx ctx, int VAB_RFQLine_Qty_ID, Trx trxName)
            : base(ctx, VAB_RFQLine_Qty_ID, trxName)
        {
            if (VAB_RFQLine_Qty_ID == 0)
            {
                //	setVAB_RFQLine_ID (0);
                //	setVAB_UOM_ID (0);
                SetIsOfferQty(false);
                SetIsPurchaseQty(false);
                SetQty(Env.ONE);	// 1
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName"></param>
        public MVABRfQLineQty(Ctx ctx, DataRow dr, Trx trxName)
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
        /// <param name="line">RfQ line</param>
        public MVABRfQLineQty(MVABRfQLine line)
            : this(line.GetCtx(), 0, line.Get_TrxName())
        {
            
            SetClientOrg(line);
            SetVAB_RFQLine_ID(line.GetVAB_RFQLine_ID());
        }
       
         /// <summary>
         /// Get Uom Name
         /// </summary>
         /// <returns>UOM</returns>
        public String GetUomName()
        {
            if (_uom == null)
            {
                _uom = MUOM.Get(GetCtx(), GetVAB_UOM_ID());
            }
            return _uom.GetName();
        }

       /// <summary>
       /// Get active Response Qtys of this RfQ Qty
       /// </summary>
       /// <param name="onlyValidAmounts">only valid amounts</param>
       /// <returns>array of response line qtys</returns>
        public MVABRFQReplyLineQty[] GetResponseQtys(bool onlyValidAmounts)
        {
            List<MVABRFQReplyLineQty> list = new List<MVABRFQReplyLineQty>();
            DataTable dt = null;
            String sql = "SELECT * FROM VAB_RFQReplyLineQty WHERE VAB_RFQLine_Qty_ID=" + GetVAB_RFQLine_Qty_ID() + " AND IsActive='Y'";
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    MVABRFQReplyLineQty qty = new MVABRFQReplyLineQty(GetCtx(), dr, Get_TrxName());
                    if (onlyValidAmounts && !qty.IsValidAmt())
                    {
                        ;
                    }
                    else
                    {
                        list.Add(qty);
                    }
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
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            MVABRFQReplyLineQty[] retValue = new MVABRFQReplyLineQty[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRfQLineQty[");
            sb.Append(Get_ID()).Append(",Qty=").Append(GetQty())
                .Append(",Offer=").Append(IsOfferQty())
                .Append(",Purchase=").Append(IsPurchaseQty())
                .Append("]");
            return sb.ToString();
        }
    }
}

/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRfQLineQty
 * Purpose        : RfQ Line Qty Model
 * Class Used     : X_C_RfQLineQty
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
    public class MRfQLineQty : X_C_RfQLineQty
    {
        //	Cache	
        private static CCache<int, MRfQLineQty> s_cache = new CCache<int, MRfQLineQty>("C_RfQLineQty", 20);
        //Unit of Measure		
        private MUOM _uom = null;

        /// <summary>
        /// Get MRfQLineQty from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_RfQLineQty_ID">ID</param>
        /// <param name="trxName">Transaction</param>
        /// <returns>MRfQLineQty</returns>
        public static MRfQLineQty Get(Ctx ctx, int C_RfQLineQty_ID, Trx trxName)
        {
            int key = C_RfQLineQty_ID;
            MRfQLineQty retValue = (MRfQLineQty)s_cache[key];
            if (retValue != null)
            {
                return retValue;
            }
            retValue = new MRfQLineQty(ctx, C_RfQLineQty_ID, trxName);
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
        /// <param name="C_RfQLineQty_ID">ID</param>
        /// <param name="trxName">transction</param>
        public MRfQLineQty(Ctx ctx, int C_RfQLineQty_ID, Trx trxName)
            : base(ctx, C_RfQLineQty_ID, trxName)
        {
            if (C_RfQLineQty_ID == 0)
            {
                //	setC_RfQLine_ID (0);
                //	setC_UOM_ID (0);
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
        public MRfQLineQty(Ctx ctx, DataRow dr, Trx trxName)
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
        public MRfQLineQty(MRfQLine line)
            : this(line.GetCtx(), 0, line.Get_TrxName())
        {
            
            SetClientOrg(line);
            SetC_RfQLine_ID(line.GetC_RfQLine_ID());
        }
       
         /// <summary>
         /// Get Uom Name
         /// </summary>
         /// <returns>UOM</returns>
        public String GetUomName()
        {
            if (_uom == null)
            {
                _uom = MUOM.Get(GetCtx(), GetC_UOM_ID());
            }
            return _uom.GetName();
        }

       /// <summary>
       /// Get active Response Qtys of this RfQ Qty
       /// </summary>
       /// <param name="onlyValidAmounts">only valid amounts</param>
       /// <returns>array of response line qtys</returns>
        public MRfQResponseLineQty[] GetResponseQtys(bool onlyValidAmounts)
        {
            List<MRfQResponseLineQty> list = new List<MRfQResponseLineQty>();
            DataTable dt = null;
            String sql = "SELECT * FROM C_RfQResponseLineQty WHERE C_RfQLineQty_ID=" + GetC_RfQLineQty_ID() + " AND IsActive='Y'";
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    MRfQResponseLineQty qty = new MRfQResponseLineQty(GetCtx(), dr, Get_TrxName());
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

            MRfQResponseLineQty[] retValue = new MRfQResponseLineQty[list.Count];
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

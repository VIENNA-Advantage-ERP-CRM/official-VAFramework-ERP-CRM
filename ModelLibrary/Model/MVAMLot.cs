/********************************************************
 * Module Name    : Workflow
 * Purpose        : 
 * Class Used     : X_VAM_Lot
 * Chronological Development
 * Veena Pandey     16-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MVAMLot : X_VAM_Lot
    {
        //	Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMLot).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Lot_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMLot(Ctx ctx, int VAM_Lot_ID, Trx trxName)
            : base(ctx, VAM_Lot_ID, trxName)
        {
            /** if (VAM_Lot_ID == 0)
            {
                setVAM_Lot_ID (0);
                setVAM_Product_ID (0);
                setName (null);
            }
            **/
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAMLot(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="ctl">lot control</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="Name">name</param>
        public MVAMLot(MVAMLotControl ctl, int VAM_Product_ID, String Name)
            : this(ctl.GetCtx(), 0, ctl.Get_TrxName())
        {
            SetClientOrg(ctl);
            SetVAM_LotControl_ID(ctl.GetVAM_LotControl_ID());
            SetVAM_Product_ID(VAM_Product_ID);
            SetName(Name);
        }

        /// <summary>
        /// Get Lots for Product
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Product_ID">id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>Array of Lots for Product</returns>
        public static MVAMLot[] GetProductLots(Ctx ctx, int VAM_Product_ID, Trx trxName)
        {
            String sql = "SELECT * FROM VAM_Lot WHERE VAM_Product_ID=" + VAM_Product_ID;
            List<MVAMLot> list = new List<MVAMLot>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow rs in ds.Tables[0].Rows)
                    {
                        list.Add(new MVAMLot(ctx, rs, trxName));
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, sql, ex);
            }

            //
            MVAMLot[] retValue = new MVAMLot[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Lot for Product
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="lot">lot</param>
        /// <param name="trxName">transaction</param>
        /// <returns>Array of Lots for Product</returns>
        public static MVAMLot GetProductLot(Ctx ctx, int VAM_Product_ID, String lot, Trx trxName)
        {
            String sql = "SELECT * FROM VAM_Lot WHERE VAM_Product_ID=" + VAM_Product_ID + " AND Name='" + lot + "'";
            MVAMLot retValue = null;
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow rs in ds.Tables[0].Rows)
                    {
                        retValue = new MVAMLot(ctx, rs, trxName);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, sql, ex);
            }
            return retValue;
        }

        /// <summary>
        /// Get Lot Key Name Pairs for Product
        /// </summary>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="trxName">transaction</param>
        /// <returns>Array of Lot Key Name Pairs for Product</returns>
        public static KeyNamePair[] GetProductLotPairs(int VAM_Product_ID, Trx trxName)
        {
            String sql = "SELECT VAM_Lot_ID, Name FROM VAM_Lot WHERE VAM_Product_ID=" + VAM_Product_ID;
            List<KeyNamePair> list = new List<KeyNamePair>();
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null, trxName);
                while (dr.Read())
                {
                    list.Add(new KeyNamePair(Utility.Util.GetValueOfInt(dr[0].ToString()), dr[1].ToString()));
                }
                dr.Close();

            }
            catch (Exception ex)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                _log.Log(Level.SEVERE, sql, ex);
            }
            

            //
            KeyNamePair[] retValue = new KeyNamePair[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            return GetName();
        }

    }
}

/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : MProductBOM
 * Chronological Development
 * Raghunandan      18-june-2009 
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
using System.IO;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MProductBOM : X_M_Product_BOM
    {

        //	Included Product		
        private MProduct _product = null;
        // Static Logger
        private static VLogger _log = VLogger.GetVLogger(typeof(MProductBOM).FullName);

        /**
         * 	Get BOM Lines for Product
         *	@param product product
         *	@return array of BOMs
         */
        public static MProductBOM[] GetBOMLines(MProduct product)
        {
            return GetBOMLines(product.GetCtx(), product.GetM_Product_ID(), product.Get_TrxName());
        }

        /**
         * 	Get BOM Lines for Product
         * 	@param ctx context
         *	@param M_Product_ID product
         *	@param trxName transaction
         *	@return array of BOMs
         */
        public static MProductBOM[] GetBOMLines(Ctx ctx, int M_Product_ID, Trx trxName)
        {
            String sql = "SELECT * FROM M_Product_BOM WHERE IsActive = 'Y' AND M_Product_ID=" + M_Product_ID + " ORDER BY Line";
            List<MProductBOM> list = new List<MProductBOM>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MProductBOM(ctx, dr, trxName));
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
            //	s_log.fine("getBOMLines - #" + list.size() + " - M_Product_ID=" + M_Product_ID);
            MProductBOM[] retValue = new MProductBOM[list.Count];
            retValue = list.ToArray();
            return retValue;
        }



        /****
         * 	Standard Constructor
         *	@param ctx context
         *	@param M_Product_BOM_ID id
         *	@param trxName transaction
         */
        public MProductBOM(Ctx ctx, int M_Product_BOM_ID, Trx trxName)
            : base(ctx, M_Product_BOM_ID, trxName)
        {
            if (M_Product_BOM_ID == 0)
            {
                //	setM_Product_ID (0);	//	parent
                //	setLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM M_Product_BOM WHERE M_Product_ID=@M_Product_ID@
                //	setM_ProductBOM_ID(0);
                SetBOMQty(Env.ZERO);	// 1
            }
        }

        /**
         * 	Load Construvtor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MProductBOM(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Get BOM Product
         *	@return product
         */
        public MProduct GetProduct()
        {
            if (_product == null && GetM_ProductBOM_ID() != 0)
                _product = MProduct.Get(GetCtx(), GetM_ProductBOM_ID());
            return _product;
        }

        /**
         * 	Set included Product
         *	@param M_ProductBOM_ID product ID
         */
        public new void SetM_ProductBOM_ID(int M_ProductBOM_ID)
        {
            base.SetM_ProductBOM_ID(M_ProductBOM_ID);
            _product = null;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MProductBOM[");
            sb.Append(Get_ID()).Append(",Line=").Append(GetLine())
                .Append(",Type=").Append(GetBOMType()).Append(",Qty=").Append(GetBOMQty());
            if (_product == null)
                sb.Append(",M_Product_ID=").Append(GetM_ProductBOM_ID());
            else
                sb.Append(",").Append(_product);
            sb.Append("]");
            return sb.ToString();
        }


        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //	Product Line was changed
            if (newRecord || Is_ValueChanged("M_ProductBOM_ID"))
            {
                //	Invalidate BOM
                MProduct product = new MProduct(GetCtx(), GetM_Product_ID(), Get_TrxName());
                if (Get_TrxName() != null)
                    product.Load(Get_TrxName());
                if (product.IsVerified())
                {
                    product.SetIsVerified(false);
                    product.Save(Get_TrxName());
                }
                //	Invalidate Products where BOM is used

            }
            return success;
        }

    }
}

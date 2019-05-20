/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : BOMValidate
 * Purpose        : Validate BOM
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     16-Oct-2009
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
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;

namespace VAdvantage.Process
{
    public class BOMValidate : SvrProcess
    {
        //	The Product			
        private int _M_Product_ID = 0;
        // Product Category	
        private int _M_Product_Category_ID = 0;
        // Re-Validate			
        private bool _IsReValidate = false;

        //	Product				
        private MProduct _product = null;
        //	List of Products	
        private List<MProduct> _products = null;

        // Manufacturing Module exist or not
        int count = 0;

        /// <summary>
        /// Prepare
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("M_Product_Category_ID"))
                {
                    _M_Product_Category_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("IsReValidate"))
                {
                    _IsReValidate = "Y".Equals(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _M_Product_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>Info</returns>
        protected override String DoIt()
        {
            if (_M_Product_ID != 0)
            {
                log.Info("M_Product_ID=" + _M_Product_ID);
                return ValidateProduct(new MProduct(GetCtx(), _M_Product_ID, Get_Trx()));
            }
            log.Info("M_Product_Category_ID=" + _M_Product_Category_ID
                + ", IsReValidate=" + _IsReValidate);
            //
            int counter = 0;
            DataTable dt = null;
            IDataReader idr = null;
            int AD_Client_ID = GetCtx().GetAD_Client_ID();
            String sql = "SELECT * FROM M_Product "
                + "WHERE IsBOM='Y' AND ";
            if (_M_Product_Category_ID == 0)
            {
                sql += "AD_Client_ID=" + AD_Client_ID;
            }
            else
            {
                sql += "M_Product_Category_ID=" + _M_Product_Category_ID;
            }
            if (!_IsReValidate)
            {
                sql += "AND IsVerified<>'Y' ";
            }
            sql += "ORDER BY Name";

            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    String info = ValidateProduct(new MProduct(GetCtx(), dr, Get_Trx()));
                    AddLog(0, null, null, info);
                    counter++;
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

            return "#" + counter;
        }



        /// <summary>
        /// Validate Product
        /// </summary>
        /// <param name="product">product</param>
        /// <returns>Info</returns>
        private String ValidateProduct(MProduct product)
        {
            count = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM AD_MODULEINFO WHERE IsActive = 'Y' AND PREFIX ='VAMFG_'"));

            if (!product.IsBOM())
            {
                return product.GetName() + " @NotValid@ @M_BOM_ID@";
            }
            _product = product;
            //	Check Old Product BOM Structure
            log.Config(_product.GetName());
            _products = new List<MProduct>();
            if (!ValidateOldProduct(_product))
            {
                _product.SetIsVerified(false);
                _product.Save();
                return _product.GetName() + " @NotValid@";
            }

            //	New Structure
            MBOM[] boms = MBOM.GetOfProduct(GetCtx(), _M_Product_ID, Get_Trx(), null);
            // if manufacturing module  exist and  BOM not contain any record against this product then not to verify Product
            if (count > 0 && boms.Length == 0)
            {
                _product.SetIsVerified(false);
                _product.Save();
                return _product.GetName() + " @NotValid@";
            }
            for (int i = 0; i < boms.Length; i++)
            {
                _products = new List<MProduct>();
                if (!ValidateBOM(boms[i]))
                {
                    _product.SetIsVerified(false);
                    _product.Save();
                    return _product.GetName() + " " + boms[i].GetName() + " @NotValid@";
                }
            }

            //	OK
            _product.SetIsVerified(true);
            _product.Save();
            return _product.GetName() + " @IsValid@";
        }


        /// <summary>
        /// Validate Old BOM Product structure
        /// </summary>
        /// <param name="product">product</param>
        /// <returns>true if valid</returns>
        private bool ValidateOldProduct(MProduct product)
        {
            count = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM AD_MODULEINFO WHERE IsActive = 'Y' AND PREFIX ='VAMFG_'"));
            if (!product.IsBOM())
            {
                return true;
            }
            //
            if (_products.Contains(product))
            {
                log.Warning(_product.GetName() + " recursively includes " + product.GetName());
                return false;
            }
            _products.Add(product);
            log.Fine(product.GetName());
            //
            MProductBOM[] productsBOMs = MProductBOM.GetBOMLines(product);
            // if manufacturing module not exist and Product BOM not containany record against this product thennot to verify BOM
            if (count <= 0 && productsBOMs.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < productsBOMs.Length; i++)
            {
                MProductBOM productsBOM = productsBOMs[i];
                MProduct pp = new MProduct(GetCtx(), productsBOM.GetM_ProductBOM_ID(), Get_Trx());
                if (!pp.IsBOM())
                {
                    log.Finer(pp.GetName());
                }
                else if (!ValidateOldProduct(pp))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Validate BOM
        /// </summary>
        /// <param name="bom">bom</param>
        /// <returns>true if valid</returns>
        private bool ValidateBOM(MBOM bom)
        {
            count = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM AD_MODULEINFO WHERE IsActive = 'Y' AND PREFIX ='VAMFG_'"));
            MBOMProduct[] BOMproducts = MBOMProduct.GetOfBOM(bom);
            // if manufacturing module  exist and  BOM Componet not contain any record against this BOM then not to verify Product
            if (count > 0 && BOMproducts.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < BOMproducts.Length; i++)
            {
                MBOMProduct BOMproduct = BOMproducts[i];
                MProduct pp = new MProduct(GetCtx(), BOMproduct.GetM_BOMProduct_ID(), Get_Trx());
                if (pp.IsBOM())
                {
                    return ValidateProduct(pp, bom.GetBOMType(), bom.GetBOMUse());
                }
            }
            return true;
        }

        /// <summary>
        /// Validate Product
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="BOMType"></param>
        /// <param name="BOMUse"></param>
        /// <returns>true if valid</returns>
        private bool ValidateProduct(MProduct product, String BOMType, String BOMUse)
        {
            if (!product.IsBOM())
            {
                return true;
            }
            //
            String restriction = "BOMType='" + BOMType + "' AND BOMUse='" + BOMUse + "'";
            MBOM[] boms = MBOM.GetOfProduct(GetCtx(), _M_Product_ID, Get_Trx(),
                restriction);
            if (boms.Length != 1)
            {
                log.Warning(restriction + " - Length=" + boms.Length);
                return false;
            }
            if (_products.Contains(product))
            {
                log.Warning(_product.GetName() + " recursively includes " + product.GetName());
                return false;
            }
            _products.Add(product);
            log.Fine(product.GetName());
            //
            MBOM bom = boms[0];
            MBOMProduct[] BOMproducts = MBOMProduct.GetOfBOM(bom);
            for (int i = 0; i < BOMproducts.Length; i++)
            {
                MBOMProduct BOMproduct = BOMproducts[i];
                MProduct pp = new MProduct(GetCtx(), BOMproduct.GetM_BOMProduct_ID(), Get_Trx());
                if (pp.IsBOM())
                {
                    return ValidateProduct(pp, bom.GetBOMType(), bom.GetBOMUse());
                }
            }
            return true;
        }
    }
}

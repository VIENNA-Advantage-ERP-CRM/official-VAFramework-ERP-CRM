using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;

namespace VAdvantage.Process
{
    class MMVAMBOMValidate: ProcessEngine.SvrProcess
    
        
    {
        //	The Product			
        private int _VAM_Product_ID = 0;
        // Product Category	
        private int _VAM_ProductCategory_ID = 0;
        // Re-Validate			
        private bool _IsReValidate = false;

        //	Product				
        private MVAMProduct _product = null;
        //	List of Products	
        private List<MVAMProduct> _products = null;

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
                else if (name.Equals("VAM_ProductCategory_ID"))
                {
                    _VAM_ProductCategory_ID = para[i].GetParameterAsInt();
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
            _VAM_Product_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>Info</returns>
        protected override String DoIt()
        {
            if (_VAM_Product_ID != 0)
            {
                log.Info("VAM_Product_ID=" + _VAM_Product_ID);
                return ValidateProduct(new MVAMProduct(GetCtx(), _VAM_Product_ID, Get_Trx()));
            }
            log.Info("VAM_ProductCategory_ID=" + _VAM_ProductCategory_ID
                + ", IsReValidate=" + _IsReValidate);
            //
            int counter = 0;
            DataTable dt = null;
            IDataReader idr = null;
            int VAF_Client_ID = GetCtx().GetVAF_Client_ID();
            String sql = "SELECT * FROM VAM_Product "
                + "WHERE IsBOM='Y' AND ";
            if (_VAM_ProductCategory_ID == 0)
            {
                sql += "VAF_Client_ID=" + VAF_Client_ID;
            }
            else
            {
                sql += "VAM_ProductCategory_ID=" + _VAM_ProductCategory_ID;
            }
            if (!_IsReValidate)
            {
                sql += "AND IsVerified<>'Y' ";
            }
            sql += "ORDER BY Name";

            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    String info = ValidateProduct(new MVAMProduct(GetCtx(), dr, Get_Trx()));
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
        private String ValidateProduct(MVAMProduct product)
        {
            if (!product.IsBOM())
            {
                return product.GetName() + " @NotValid@ @VAM_BOM_ID@";
            }
            _product = product;
            //	Check Old Product BOM Structure
            log.Config(_product.GetName());
            _products = new List<MVAMProduct>();
            if (!ValidateOldProduct(_product))
            {
                _product.SetIsVerified(false);
                _product.Save();
                return _product.GetName() + " @NotValid@";
            }

            //	New Structure
            MVAMBOM[] boms = MVAMBOM.GetOfProduct(GetCtx(), _VAM_Product_ID, Get_Trx(), null);
            for (int i = 0; i < boms.Length; i++)
            {
                _products = new List<MVAMProduct>();
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
        private bool ValidateOldProduct(MVAMProduct product)
        {
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
            MVAMProductBOM[] productsBOMs = MVAMProductBOM.GetBOMLines(product);
            for (int i = 0; i < productsBOMs.Length; i++)
            {
                MVAMProductBOM productsBOM = productsBOMs[i];
                MVAMProduct pp = new MVAMProduct(GetCtx(), productsBOM.GetVAM_ProductBOM_ID(), Get_Trx());
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
        private bool ValidateBOM(MVAMBOM bom)
        {
            MVAMBOMVAMProduct[] BOMVAMProducts = MVAMBOMVAMProduct.GetOfBOM(bom);
            for (int i = 0; i < BOMVAMProducts.Length; i++)
            {
                MVAMBOMVAMProduct BOMVAMProduct = BOMVAMProducts[i];
                MVAMProduct pp = new MVAMProduct(GetCtx(), BOMVAMProduct.GetVAM_BOMVAMProduct_ID(), Get_Trx());
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
        private bool ValidateProduct(MVAMProduct product, String BOMType, String BOMUse)
        {
            if (!product.IsBOM())
            {
                return true;
            }
            //
            String restriction = "BOMType='" + BOMType + "' AND BOMUse='" + BOMUse + "'";
            MVAMBOM[] boms = MVAMBOM.GetOfProduct(GetCtx(), _VAM_Product_ID, Get_Trx(),
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
            MVAMBOM bom = boms[0];
            MVAMBOMVAMProduct[] BOMVAMProducts = MVAMBOMVAMProduct.GetOfBOM(bom);
            for (int i = 0; i < BOMVAMProducts.Length; i++)
            {
                MVAMBOMVAMProduct BOMVAMProduct = BOMVAMProducts[i];
                MVAMProduct pp = new MVAMProduct(GetCtx(), BOMVAMProduct.GetVAM_BOMVAMProduct_ID(), Get_Trx());
                if (pp.IsBOM())
                {
                    return ValidateProduct(pp, bom.GetBOMType(), bom.GetBOMUse());
                }
            }
            return true;
        }
    }
}

/********************************************************
 * Module Name    : VFramwork (Model class)
 * Purpose        : BOM Model
 * Class Used     : X_M_BOM
 * Chronological Development
 * Raghunandan    24-June-2009
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Common;
//using VAdvantage.Grid;
using VAdvantage.Process;
using VAdvantage.Utility;
namespace VAdvantage.Model
{
    public class MBOM : X_M_BOM
    {
        //	Cache						
        private static CCache<int, MBOM> _cache = new CCache<int, MBOM>("M_BOM", 20);
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MBOM).FullName);


        /**
        * 	Get BOM from Cache
        *	@param ctx context
        *	@param M_BOM_ID id
        *	@return MBOM
        */
        public static MBOM Get(Ctx ctx, int M_BOM_ID)
        {
            int key = M_BOM_ID;
            MBOM retValue = (MBOM)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MBOM(ctx, M_BOM_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }	//	Get

        /**
         * 	Get BOMs Of Product
         *	@param ctx context
         *	@param M_Product_ID product
         *	@param trxName trx
         *	@param whereClause optional WHERE clause w/o AND
         *	@return array of BOMs
         */
        public static MBOM[] GetOfProduct(Ctx ctx, int M_Product_ID,
            Trx trxName, String whereClause)
        {
            List<MBOM> list = new List<MBOM>();
            String sql = "SELECT * FROM M_BOM WHERE IsActive = 'Y' AND M_Product_ID=" + M_Product_ID;
            if (whereClause != null && whereClause.Length > 0)
                sql += " AND " + whereClause;
            //PreparedStatement pstmt = null;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement (sql, trxName);
                //pstmt.SetInt (1, M_Product_ID);
                //ResultSet rs = pstmt.executeQuery ();
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                foreach (DataRow dr in dt.Rows)
                    list.Add(new MBOM(ctx, dr, trxName));
                //rs.close ();
                //pstmt.close ();
                //pstmt = null;
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
                dt = null;
            }
            //try
            //{
            //    if (pstmt != null)
            //        pstmt.close ();
            //    pstmt = null;
            //}
            //catch (Exception e)
            //{
            //    pstmt = null;
            //}

            MBOM[] retValue = new MBOM[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	//	GetOfProduct



        /**************************************************************************
         * 	Standard Constructor
         *	@param ctx context
         *	@param M_BOM_ID id
         *	@param trxName trx
         */
        public MBOM(Ctx ctx, int M_BOM_ID, Trx trxName) :
            base(ctx, M_BOM_ID, trxName)
        {

            if (M_BOM_ID == 0)
            {
                //	SetM_Product_ID (0);
                //	SetName (null);
                SetBOMType(BOMTYPE_CurrentActive);	// A
                SetBOMUse(BOMUSE_Master);	// A
            }
        }	//	MBOM

        /**
         * 	Load Constructor
         *	@param ctx ctx
         *	@param rs result Set
         *	@param trxName trx
         */
        public MBOM(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super (ctx, rs, trxName);
        }	//	MBOM

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true/false
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            //	BOM Type
            //if (newRecord || Is_ValueChanged("BOMType"))
            if (Is_ValueChanged("BOMType") || Is_ValueChanged("BOMUse"))
            {
                //	Only one Current Active
                if (GetBOMType().Equals(BOMTYPE_CurrentActive))
                {
                    MBOM[] boms = GetOfProduct(GetCtx(), GetM_Product_ID(), Get_Trx(),
                        "BOMType='A' AND BOMUse='" + GetBOMUse() + "' AND IsActive='Y' AND M_AttributeSetInstance_ID=" + GetM_AttributeSetInstance_ID());
                    if (boms.Length == 0	//	only one = this 
                        || (boms.Length == 1 && boms[0].GetM_BOM_ID() == GetM_BOM_ID()))
                    { ;}
                    else
                    {
                        //log.SaveError("Error", Msg.ParseTranslation(GetCtx(),
                        //"Can only have one Current Active BOM for Product BOM Use (" + GetBOMType() + ")"));
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "ProductBOMExist"));
                        return false;
                    }
                }
                //	Only one MTO
                else if (GetBOMType().Equals(BOMTYPE_Make_To_Order))
                {
                    MBOM[] boms = GetOfProduct(GetCtx(), GetM_Product_ID(), Get_Trx(),
                        "IsActive='Y'");
                    if (boms.Length == 0	//	only one = this 
                        || (boms.Length == 1 && boms[0].GetM_BOM_ID() == GetM_BOM_ID()))
                    { ;}
                    else
                    {
                        log.SaveError("Error", Msg.ParseTranslation(GetCtx(),
                           "Can only have single Make-to-Order BOM for Product"));
                        return false;
                    }
                }
                //	BOM Type               
            }

            return true;
        }	//	beforeSave

        protected override bool AfterSave(bool newRecord, bool success)
        {
            //set verified on Product as False when we change BOMType AND BOMUse
            if (newRecord || Is_ValueChanged("BOMType") || Is_ValueChanged("BOMUse") || Is_ValueChanged("IsActive") || Is_ValueChanged("M_AttributeSetInstance_ID"))
            {
                MProduct product = new MProduct(GetCtx(), GetM_Product_ID(), Get_Trx());
                product.SetIsVerified(false);
                if (!product.Save())
                {
                    log.SaveError("Error", "Verified not updated on Product : " + product.GetValue());
                }
            }
            return true;
        }

        protected override bool AfterDelete(bool success)
        {
            // when we delete BOM, then set IsVerified False on Product
            MProduct product = new MProduct(GetCtx(), GetM_Product_ID(), Get_Trx());
            product.SetIsVerified(false);
            if (!product.Save())
            {
                log.SaveError("Error", "Verified not updated on Product : " + product.GetValue());
            }
            return true;
        }
    }
}

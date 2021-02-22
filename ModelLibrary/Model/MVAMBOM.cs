/********************************************************
 * Module Name    : VFramwork (Model class)
 * Purpose        : BOM Model
 * Class Used     : X_VAM_BOM
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
    public class MVAMBOM : X_VAM_BOM
    {
        //	Cache						
        private static CCache<int, MVAMBOM> _cache = new CCache<int, MVAMBOM>("VAM_BOM", 20);
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMBOM).FullName);


        /**
        * 	Get BOM from Cache
        *	@param ctx context
        *	@param VAM_BOM_ID id
        *	@return MVAMBOM
        */
        public static MVAMBOM Get(Ctx ctx, int VAM_BOM_ID)
        {
            int key = VAM_BOM_ID;
            MVAMBOM retValue = (MVAMBOM)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVAMBOM(ctx, VAM_BOM_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }	//	Get

        /**
         * 	Get BOMs Of Product
         *	@param ctx context
         *	@param VAM_Product_ID product
         *	@param trxName trx
         *	@param whereClause optional WHERE clause w/o AND
         *	@return array of BOMs
         */
        public static MVAMBOM[] GetOfProduct(Ctx ctx, int VAM_Product_ID,
            Trx trxName, String whereClause)
        {
            List<MVAMBOM> list = new List<MVAMBOM>();
            String sql = "SELECT * FROM VAM_BOM WHERE IsActive = 'Y' AND VAM_Product_ID=" + VAM_Product_ID;
            if (whereClause != null && whereClause.Length > 0)
                sql += " AND " + whereClause;
            //PreparedStatement pstmt = null;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement (sql, trxName);
                //pstmt.SetInt (1, VAM_Product_ID);
                //ResultSet rs = pstmt.executeQuery ();
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                foreach (DataRow dr in dt.Rows)
                    list.Add(new MVAMBOM(ctx, dr, trxName));
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

            MVAMBOM[] retValue = new MVAMBOM[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	//	GetOfProduct



        /**************************************************************************
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAM_BOM_ID id
         *	@param trxName trx
         */
        public MVAMBOM(Ctx ctx, int VAM_BOM_ID, Trx trxName) :
            base(ctx, VAM_BOM_ID, trxName)
        {

            if (VAM_BOM_ID == 0)
            {
                //	SetVAM_Product_ID (0);
                //	SetName (null);
                SetBOMType(BOMTYPE_CurrentActive);	// A
                SetBOMUse(BOMUSE_Master);	// A
            }
        }	//	MVAMBOM

        /**
         * 	Load Constructor
         *	@param ctx ctx
         *	@param rs result Set
         *	@param trxName trx
         */
        public MVAMBOM(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super (ctx, rs, trxName);
        }	//	MVAMBOM

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
                    MVAMBOM[] boms = GetOfProduct(GetCtx(), GetVAM_Product_ID(), Get_Trx(),
                        "BOMType='A' AND BOMUse='" + GetBOMUse() + "' AND IsActive='Y' AND VAM_PFeature_SetInstance_ID=" + GetVAM_PFeature_SetInstance_ID());
                    if (boms.Length == 0	//	only one = this 
                        || (boms.Length == 1 && boms[0].GetVAM_BOM_ID() == GetVAM_BOM_ID()))
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
                    MVAMBOM[] boms = GetOfProduct(GetCtx(), GetVAM_Product_ID(), Get_Trx(),
                        "IsActive='Y'");
                    if (boms.Length == 0	//	only one = this 
                        || (boms.Length == 1 && boms[0].GetVAM_BOM_ID() == GetVAM_BOM_ID()))
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
            if (newRecord || Is_ValueChanged("BOMType") || Is_ValueChanged("BOMUse") || Is_ValueChanged("IsActive") || Is_ValueChanged("VAM_PFeature_SetInstance_ID"))
            {
                MVAMProduct product = new MVAMProduct(GetCtx(), GetVAM_Product_ID(), Get_Trx());
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
            MVAMProduct product = new MVAMProduct(GetCtx(), GetVAM_Product_ID(), Get_Trx());
            product.SetIsVerified(false);
            if (!product.Save())
            {
                log.SaveError("Error", "Verified not updated on Product : " + product.GetValue());
            }
            return true;
        }
    }
}

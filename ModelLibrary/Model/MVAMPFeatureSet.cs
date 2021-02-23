/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMPFeatureSet
 * Purpose        : Gat the value using VAM_PFeature_Use table 
 * Class Used     : X_VAM_PFeature_Set
 * Chronological    Development
 * Raghunandan     05-Jun-2009
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
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MVAMPFeatureSet : X_VAM_PFeature_Set
    {
        #region Private Variables
        //	Instance Attributes					
        private MVAMProductFeature[] _instanceAttributes = null;
        //	Instance Attributes					
        private MVAMProductFeature[] _productAttributes = null;
        // Entry Exclude						
        private X_VAM_PFeature_SetExclude[] _excludes = null;
        // Lot create Exclude					
        private X_VAM_LotControlExclude[] _excludeLots = null;
        //Serial No create Exclude				
        private X_VAM_ExcludeCtlSerialNo[] _excludeSerNos = null;
        //	Cache						
        private static CCache<int, MVAMPFeatureSet> s_cache = new CCache<int, MVAMPFeatureSet>("VAM_PFeature_Set", 20);
        #endregion

        /* Get MVAMPFeatureSet from Cache
        *	@param ctx context
        *	@param VAM_PFeature_Set_ID id
        *	@return MVAMPFeatureSet
        */
        public static MVAMPFeatureSet Get(Ctx ctx, int VAM_PFeature_Set_ID)
        {
            int key = VAM_PFeature_Set_ID;
            MVAMPFeatureSet retValue = (MVAMPFeatureSet)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVAMPFeatureSet(ctx, VAM_PFeature_Set_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /**
         * 	Standard constructor
         *	@param ctx context
         *	@param VAM_PFeature_Set_ID id
         *	@param trxName transaction
         */
        public MVAMPFeatureSet(Ctx ctx, int VAM_PFeature_Set_ID, Trx trxName)
            : base(ctx, VAM_PFeature_Set_ID, trxName)
        {

            if (VAM_PFeature_Set_ID == 0)
            {
                //	setName (null);
                SetIsGuaranteeDate(false);
                SetIsGuaranteeDateMandatory(false);
                SetIsLot(false);
                SetIsLotMandatory(false);
                SetIsSerNo(false);
                SetIsSerNoMandatory(false);
                SetIsInstanceAttribute(false);
                SetMandatoryType(MANDATORYTYPE_NotMandatary);
            }
        }

        /**
         * 	Load constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MVAMPFeatureSet(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Get Attribute Array
         * 	@param instanceAttributes true if for instance
         *	@return instance or product attribute array
         */
        public MVAMProductFeature[] GetMAttributes(bool instanceAttributes)
        {
            if ((_instanceAttributes == null && instanceAttributes)
                || _productAttributes == null && !instanceAttributes)
            {
                String sql = "SELECT mau.VAM_ProductFeature_ID "
                    + "FROM VAM_PFeature_Use mau"
                    + " INNER JOIN VAM_ProductFeature ma ON (mau.VAM_ProductFeature_ID=ma.VAM_ProductFeature_ID) "
                    + "WHERE mau.IsActive='Y' AND ma.IsActive='Y'"
                    + " AND mau.VAM_PFeature_Set_ID=" + GetVAM_PFeature_Set_ID() + " AND ma.IsInstanceAttribute= " +
                    ((instanceAttributes) ? "'Y'" : "'N'").ToString()
                    + " ORDER BY mau.VAM_ProductFeature_ID";
                List<MVAMProductFeature> list = new List<MVAMProductFeature>();
                DataTable dt = null;
                IDataReader idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                try
                {
                    //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    foreach (DataRow dr in dt.Rows)
                    {
                        //DataRow dr = ds.Tables[0].Rows[i];
                        MVAMProductFeature ma = new MVAMProductFeature(GetCtx(), Convert.ToInt32(dr[0]), Get_TrxName());
                        list.Add(ma);
                    }
                }
                catch (Exception ex)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    log.Log(Level.SEVERE, sql, ex);
                }
                finally {
                    dt = null;
                }

                //	Differentiate attributes
                if (instanceAttributes)
                {
                    _instanceAttributes = new MVAMProductFeature[list.Count];
                    _instanceAttributes = list.ToArray();
                }
                else
                {
                    _productAttributes = new MVAMProductFeature[list.Count];
                    _productAttributes = list.ToArray();
                }
            }
            //
            if (instanceAttributes)
            {
                if (IsInstanceAttribute() != _instanceAttributes.Length > 0)
                    SetIsInstanceAttribute(_instanceAttributes.Length > 0);
            }

            //	Return
            if (instanceAttributes)
                return _instanceAttributes;
            return _productAttributes;
        }

        /**
         * 	Something is Mandatory
         *	@return true if something is mandatory
         */
        public bool IsMandatory()
        {
            return !MANDATORYTYPE_NotMandatary.Equals(GetMandatoryType())
                || IsLotMandatory()
                || IsSerNoMandatory()
                || IsGuaranteeDateMandatory();
        }

        /**
         * 	Is always mandatory
         *	@return mandatory 
         */
        public bool IsMandatoryAlways()
        {
            return MANDATORYTYPE_AlwaysMandatory.Equals(GetMandatoryType());
        }

        /**
         * 	Is Mandatory when Shipping
         *	@return true if required for shipping
         */
        public bool IsMandatoryShipping()
        {
            return MANDATORYTYPE_WhenShipping.Equals(GetMandatoryType());
        }

        /**
         * 	Exclude entry
         *	@param VAF_Column_ID column
         *	@param isSOTrx sales order
         *	@return true if excluded
         */
        public bool ExcludeEntry(int VAF_Column_ID, bool isSOTrx)
        {
            if (_excludes == null)
            {
                List<X_VAM_PFeature_SetExclude> list = new List<X_VAM_PFeature_SetExclude>();
                String sql = "SELECT * FROM VAM_PFeature_SetExclude WHERE IsActive='Y' AND VAM_PFeature_Set_ID=" + GetVAM_PFeature_Set_ID();
                DataSet ds = new DataSet();
                try
                {
                    ds = ExecuteQuery.ExecuteDataset(sql, null);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        list.Add(new X_VAM_PFeature_SetExclude(GetCtx(), dr, null));
                    }
                    ds = null;
                }
                catch (Exception e)
                {
                    log.Log (Level.SEVERE, sql, e);
                }
                _excludes = new X_VAM_PFeature_SetExclude[list.Count];
                _excludes = list.ToArray();
            }
            //	Find it
            if (_excludes != null && _excludes.Length > 0)
            {
                MVAFColumn column = MVAFColumn.Get(GetCtx(), VAF_Column_ID);
                for (int i = 0; i < _excludes.Length; i++)
                {
                    if (_excludes[i].GetVAF_TableView_ID() == column.GetVAF_TableView_ID()
                        && _excludes[i].IsSOTrx() == isSOTrx)
                        return true;
                }
            }
            return false;
        }

        /**
         * 	Exclude Lot creation
         *	@param VAF_Column_ID column
         *	@param isSOTrx SO
         *	@return true if excluded
         */
        public bool IsExcludeLot(int VAF_Column_ID, bool isSOTrx)
        {
            if (GetVAM_LotControl_ID() == 0)
                return true;
            if (_excludeLots == null)
            {
                List<X_VAM_LotControlExclude> list = new List<X_VAM_LotControlExclude>();
                String sql = "SELECT * FROM VAM_LotControlExclude WHERE IsActive='Y' AND VAM_LotControl_ID=" + GetVAM_LotControl_ID();
                DataSet ds = null;
                try
                {
                    ds = ExecuteQuery.ExecuteDataset(sql, null);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        list.Add(new X_VAM_LotControlExclude(GetCtx(), dr, null));
                    }
                    ds = null;
                }
                catch (Exception e)
                {
                    log.Log (Level.SEVERE, sql, e);
                }

                _excludeLots = new X_VAM_LotControlExclude[list.Count];
                _excludeLots = list.ToArray();
            }
            //	Find it
            if (_excludeLots != null && _excludeLots.Length > 0)
            {
                MVAFColumn column = MVAFColumn.Get(GetCtx(), VAF_Column_ID);
                for (int i = 0; i < _excludeLots.Length; i++)
                {
                    if (_excludeLots[i].GetVAF_TableView_ID() == column.GetVAF_TableView_ID()
                        && _excludeLots[i].IsSOTrx() == isSOTrx)
                        return true;
                }
            }
            return false;
        }

        /**
         *	Exclude SerNo creation
         *	@param VAF_Column_ID column
         *	@param isSOTrx SO
         *	@return true if excluded
         */
        public bool IsExcludeSerNo(int VAF_Column_ID, bool isSOTrx)
        {
            if (GetVAM_CtlSerialNo_ID() == 0)
                return true;
            if (_excludeSerNos == null)
            {
                List<X_VAM_ExcludeCtlSerialNo> list = new List<X_VAM_ExcludeCtlSerialNo>();
                String sql = "SELECT * FROM VAM_ExcludeCtlSerialNo WHERE IsActive='Y' AND VAM_CtlSerialNo_ID=" + GetVAM_CtlSerialNo_ID();
                DataSet ds = null;
                try
                {
                    ds = ExecuteQuery.ExecuteDataset(sql, null);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        list.Add(new X_VAM_ExcludeCtlSerialNo(GetCtx(), dr, null));
                    }
                    ds = null;
                }
                catch (Exception e)
                {
                    log.Log (Level.SEVERE, sql, e);
                }

                _excludeSerNos = new X_VAM_ExcludeCtlSerialNo[list.Count];
                _excludeSerNos = list.ToArray();
            }
            //	Find it
            if (_excludeSerNos != null && _excludeSerNos.Length > 0)
            {
                MVAFColumn column = MVAFColumn.Get(GetCtx(), VAF_Column_ID);
                for (int i = 0; i < _excludeSerNos.Length; i++)
                {
                    if (_excludeSerNos[i].GetVAF_TableView_ID() == column.GetVAF_TableView_ID()
                        && _excludeSerNos[i].IsSOTrx() == isSOTrx)
                        return true;
                }
            }
            return false;
        }

        /**
         * 	Get Lot Char Start
         *	@return defined or \u00ab 
         */
        public String GetLotCharStart()
        {
            String s = base.GetLotCharSOverwrite();
            if (s != null && s.Length == 1 && !s.Equals(" "))
                return s;
            return "\u00ab";
        }

        /**
         * 	Get Lot Char End
         *	@return defined or \u00bb 
         */
        public String GetLotCharEnd()
        {
            String s = base.GetLotCharEOverwrite();
            if (s != null && s.Length == 1 && !s.Equals(" "))
                return s;
            return "\u00bb";
        }

        /**
         * 	Get SerNo Char Start
         *	@return defined or #
         */
        public String GetSerNoCharStart()
        {
            String s = base.GetSerNoCharSOverwrite();
            if (s != null && s.Length == 1 && !s.Equals(" "))
                return s;
            return "#";
        }

        /**
         * 	Get SerNo Char End
         *	@return defined or none
         */
        public String GetSerNoCharEnd()
        {
            String s = base.GetSerNoCharEOverwrite();
            if (s != null && s.Length == 1 && !s.Equals(" "))
                return s;
            return "";
        }

        /**
         * 	Before Save.
         * 	- set instance attribute flag
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            if (!IsInstanceAttribute()
                && (IsSerNo() || IsLot() || IsGuaranteeDate()))
                SetIsInstanceAttribute(true);
            return true;
        }

        /**
         * 	After Save.
         * 	- Verify Instance Attribute
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //	Set Instance Attribute
            if (!IsInstanceAttribute())
            {
                String sql = "UPDATE VAM_PFeature_Set mas"
                    + " SET IsInstanceAttribute='Y' "
                    + "WHERE VAM_PFeature_Set_ID=" + GetVAM_PFeature_Set_ID()
                    + " AND IsInstanceAttribute='N'"
                    + " AND (IsSerNo='Y' OR IsLot='Y' OR IsGuaranteeDate='Y'"
                        + " OR EXISTS (SELECT * FROM VAM_PFeature_Use mau"
                            + " INNER JOIN VAM_ProductFeature ma ON (mau.VAM_ProductFeature_ID=ma.VAM_ProductFeature_ID) "
                            + "WHERE mau.VAM_PFeature_Set_ID=mas.VAM_PFeature_Set_ID"
                            + " AND mau.IsActive='Y' AND ma.IsActive='Y'"
                            + " AND ma.IsInstanceAttribute='Y')"
                            + ")";
                int no = DataBase.DB.ExecuteQuery(sql,null,null);
                if (no != 0)
                {
                    log.Warning("Set Instance Attribute");
                    SetIsInstanceAttribute(true);
                }
            }
            //	Reset Instance Attribute
            if (IsInstanceAttribute() && !IsSerNo() && !IsLot() && !IsGuaranteeDate())
            {
                String sql = "UPDATE VAM_PFeature_Set mas"
                    + " SET IsInstanceAttribute='N' "
                    + "WHERE VAM_PFeature_Set_ID=" + GetVAM_PFeature_Set_ID()
                    + " AND IsInstanceAttribute='Y'"
                    + "	AND IsSerNo='N' AND IsLot='N' AND IsGuaranteeDate='N'"
                    + " AND NOT EXISTS (SELECT * FROM VAM_PFeature_Use mau"
                        + " INNER JOIN VAM_ProductFeature ma ON (mau.VAM_ProductFeature_ID=ma.VAM_ProductFeature_ID) "
                        + "WHERE mau.VAM_PFeature_Set_ID=mas.VAM_PFeature_Set_ID"
                        + " AND mau.IsActive='Y' AND ma.IsActive='Y'"
                        + " AND ma.IsInstanceAttribute='Y')";
                int no = DataBase.DB.ExecuteQuery(sql, null, null);
                if (no != 0)
                {
                    log.Warning("Reset Instance Attribute");
                    SetIsInstanceAttribute(false);
                }
            }
            return success;
        }

    }
}

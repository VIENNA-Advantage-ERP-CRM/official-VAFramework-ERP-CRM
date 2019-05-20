/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAttributeSet
 * Purpose        : Gat the value using M_AttributeUse table 
 * Class Used     : X_M_AttributeSet
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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MAttributeSet : X_M_AttributeSet
    {
        #region Private Variables
        //	Instance Attributes					
        private MAttribute[] _instanceAttributes = null;
        //	Instance Attributes					
        private MAttribute[] _productAttributes = null;
        // Entry Exclude						
        private X_M_AttributeSetExclude[] _excludes = null;
        // Lot create Exclude					
        private X_M_LotCtlExclude[] _excludeLots = null;
        //Serial No create Exclude				
        private X_M_SerNoCtlExclude[] _excludeSerNos = null;
        //	Cache						
        private static CCache<int, MAttributeSet> s_cache = new CCache<int, MAttributeSet>("M_AttributeSet", 20);
        #endregion

        /* Get MAttributeSet from Cache
        *	@param ctx context
        *	@param M_AttributeSet_ID id
        *	@return MAttributeSet
        */
        public static MAttributeSet Get(Ctx ctx, int M_AttributeSet_ID)
        {
            int key = M_AttributeSet_ID;
            MAttributeSet retValue = (MAttributeSet)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MAttributeSet(ctx, M_AttributeSet_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /**
         * 	Standard constructor
         *	@param ctx context
         *	@param M_AttributeSet_ID id
         *	@param trxName transaction
         */
        public MAttributeSet(Ctx ctx, int M_AttributeSet_ID, Trx trxName)
            : base(ctx, M_AttributeSet_ID, trxName)
        {

            if (M_AttributeSet_ID == 0)
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
        public MAttributeSet(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Get Attribute Array
         * 	@param instanceAttributes true if for instance
         *	@return instance or product attribute array
         */
        public MAttribute[] GetMAttributes(bool instanceAttributes)
        {
            if ((_instanceAttributes == null && instanceAttributes)
                || _productAttributes == null && !instanceAttributes)
            {
                String sql = "SELECT mau.M_Attribute_ID "
                    + "FROM M_AttributeUse mau"
                    + " INNER JOIN M_Attribute ma ON (mau.M_Attribute_ID=ma.M_Attribute_ID) "
                    + "WHERE mau.IsActive='Y' AND ma.IsActive='Y'"
                    + " AND mau.M_AttributeSet_ID=" + GetM_AttributeSet_ID() + " AND ma.IsInstanceAttribute= " +
                    ((instanceAttributes) ? "'Y'" : "'N'").ToString()
                    + " ORDER BY mau.M_Attribute_ID";
                List<MAttribute> list = new List<MAttribute>();
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
                        MAttribute ma = new MAttribute(GetCtx(), Convert.ToInt32(dr[0]), Get_TrxName());
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
                    _instanceAttributes = new MAttribute[list.Count];
                    _instanceAttributes = list.ToArray();
                }
                else
                {
                    _productAttributes = new MAttribute[list.Count];
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
         *	@param AD_Column_ID column
         *	@param isSOTrx sales order
         *	@return true if excluded
         */
        public bool ExcludeEntry(int AD_Column_ID, bool isSOTrx)
        {
            if (_excludes == null)
            {
                List<X_M_AttributeSetExclude> list = new List<X_M_AttributeSetExclude>();
                String sql = "SELECT * FROM M_AttributeSetExclude WHERE IsActive='Y' AND M_AttributeSet_ID=" + GetM_AttributeSet_ID();
                DataSet ds = new DataSet();
                try
                {
                    ds = ExecuteQuery.ExecuteDataset(sql, null);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        list.Add(new X_M_AttributeSetExclude(GetCtx(), dr, null));
                    }
                    ds = null;
                }
                catch (Exception e)
                {
                    log.Log (Level.SEVERE, sql, e);
                }
                _excludes = new X_M_AttributeSetExclude[list.Count];
                _excludes = list.ToArray();
            }
            //	Find it
            if (_excludes != null && _excludes.Length > 0)
            {
                MColumn column = MColumn.Get(GetCtx(), AD_Column_ID);
                for (int i = 0; i < _excludes.Length; i++)
                {
                    if (_excludes[i].GetAD_Table_ID() == column.GetAD_Table_ID()
                        && _excludes[i].IsSOTrx() == isSOTrx)
                        return true;
                }
            }
            return false;
        }

        /**
         * 	Exclude Lot creation
         *	@param AD_Column_ID column
         *	@param isSOTrx SO
         *	@return true if excluded
         */
        public bool IsExcludeLot(int AD_Column_ID, bool isSOTrx)
        {
            if (GetM_LotCtl_ID() == 0)
                return true;
            if (_excludeLots == null)
            {
                List<X_M_LotCtlExclude> list = new List<X_M_LotCtlExclude>();
                String sql = "SELECT * FROM M_LotCtlExclude WHERE IsActive='Y' AND M_LotCtl_ID=" + GetM_LotCtl_ID();
                DataSet ds = null;
                try
                {
                    ds = ExecuteQuery.ExecuteDataset(sql, null);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        list.Add(new X_M_LotCtlExclude(GetCtx(), dr, null));
                    }
                    ds = null;
                }
                catch (Exception e)
                {
                    log.Log (Level.SEVERE, sql, e);
                }

                _excludeLots = new X_M_LotCtlExclude[list.Count];
                _excludeLots = list.ToArray();
            }
            //	Find it
            if (_excludeLots != null && _excludeLots.Length > 0)
            {
                MColumn column = MColumn.Get(GetCtx(), AD_Column_ID);
                for (int i = 0; i < _excludeLots.Length; i++)
                {
                    if (_excludeLots[i].GetAD_Table_ID() == column.GetAD_Table_ID()
                        && _excludeLots[i].IsSOTrx() == isSOTrx)
                        return true;
                }
            }
            return false;
        }

        /**
         *	Exclude SerNo creation
         *	@param AD_Column_ID column
         *	@param isSOTrx SO
         *	@return true if excluded
         */
        public bool IsExcludeSerNo(int AD_Column_ID, bool isSOTrx)
        {
            if (GetM_SerNoCtl_ID() == 0)
                return true;
            if (_excludeSerNos == null)
            {
                List<X_M_SerNoCtlExclude> list = new List<X_M_SerNoCtlExclude>();
                String sql = "SELECT * FROM M_SerNoCtlExclude WHERE IsActive='Y' AND M_SerNoCtl_ID=" + GetM_SerNoCtl_ID();
                DataSet ds = null;
                try
                {
                    ds = ExecuteQuery.ExecuteDataset(sql, null);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        list.Add(new X_M_SerNoCtlExclude(GetCtx(), dr, null));
                    }
                    ds = null;
                }
                catch (Exception e)
                {
                    log.Log (Level.SEVERE, sql, e);
                }

                _excludeSerNos = new X_M_SerNoCtlExclude[list.Count];
                _excludeSerNos = list.ToArray();
            }
            //	Find it
            if (_excludeSerNos != null && _excludeSerNos.Length > 0)
            {
                MColumn column = MColumn.Get(GetCtx(), AD_Column_ID);
                for (int i = 0; i < _excludeSerNos.Length; i++)
                {
                    if (_excludeSerNos[i].GetAD_Table_ID() == column.GetAD_Table_ID()
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
                String sql = "UPDATE M_AttributeSet mas"
                    + " SET IsInstanceAttribute='Y' "
                    + "WHERE M_AttributeSet_ID=" + GetM_AttributeSet_ID()
                    + " AND IsInstanceAttribute='N'"
                    + " AND (IsSerNo='Y' OR IsLot='Y' OR IsGuaranteeDate='Y'"
                        + " OR EXISTS (SELECT * FROM M_AttributeUse mau"
                            + " INNER JOIN M_Attribute ma ON (mau.M_Attribute_ID=ma.M_Attribute_ID) "
                            + "WHERE mau.M_AttributeSet_ID=mas.M_AttributeSet_ID"
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
                String sql = "UPDATE M_AttributeSet mas"
                    + " SET IsInstanceAttribute='N' "
                    + "WHERE M_AttributeSet_ID=" + GetM_AttributeSet_ID()
                    + " AND IsInstanceAttribute='Y'"
                    + "	AND IsSerNo='N' AND IsLot='N' AND IsGuaranteeDate='N'"
                    + " AND NOT EXISTS (SELECT * FROM M_AttributeUse mau"
                        + " INNER JOIN M_Attribute ma ON (mau.M_Attribute_ID=ma.M_Attribute_ID) "
                        + "WHERE mau.M_AttributeSet_ID=mas.M_AttributeSet_ID"
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

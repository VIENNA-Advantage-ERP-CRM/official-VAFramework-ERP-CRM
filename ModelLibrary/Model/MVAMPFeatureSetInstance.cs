/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMPFeatureSetInstance
 * Purpose        : Get attribute instance from VAM_Product tab;e
 * Class Used     : X_VAM_PFeature_SetInstance
 * Chronological    Development
 * Raghunandan     08-Jun-2009
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
    public class MVAMPFeatureSetInstance : X_VAM_PFeature_SetInstance
    {
        #region Private variable
        /**	Attribute Set				*/
        private MVAMPFeatureSet _mas = null;
        /**	Date Format					*/
        private SimpleDateFormat _dateFormat = DisplayType.GetDateFormat(DisplayType.Date);
        //private DateTimePickerFormat _dateFormat = DisplayType.getDateFormat(DisplayType.Date);
        //private DateTime _dateFormat = new DateTime();
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMPFeatureSetInstance).FullName);
        #endregion

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_PFeature_SetInstance_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMPFeatureSetInstance(Ctx ctx, int VAM_PFeature_SetInstance_ID, Trx trxName)
            : base(ctx, VAM_PFeature_SetInstance_ID, trxName)
        {
            if (VAM_PFeature_SetInstance_ID == 0)
            {
            }
        }

        /**
 * 	Get Attribute Set Instance from ID or Product
 *	@param ctx context
 * 	@param VAM_PFeature_SetInstance_ID id or 0
 * 	@param VAM_Product_ID required if id is 0
 * 	@return Attribute Set Instance or null
 */
        public static MVAMPFeatureSetInstance Get(Ctx ctx,
            int VAM_PFeature_SetInstance_ID, int VAM_Product_ID)
        {
            MVAMPFeatureSetInstance retValue = null;
            //	Load Instance if not 0
            if (VAM_PFeature_SetInstance_ID != 0)
            {
               _log.Fine("From VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID);
                return new MVAMPFeatureSetInstance(ctx, VAM_PFeature_SetInstance_ID, null);
            }
            //	Get new from Product
           _log.Fine("From VAM_Product_ID=" + VAM_Product_ID);
            if (VAM_Product_ID == 0)
                return null;
            String sql = "SELECT VAM_PFeature_Set_ID, VAM_PFeature_SetInstance_ID "
                + "FROM VAM_Product "
                + "WHERE VAM_Product_ID=" + VAM_Product_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    int VAM_PFeature_Set_ID = Utility.Util.GetValueOfInt(dr[0].ToString()); //Convert.ToInt32(dr[0]);//.getInt(1);
                    //	VAM_PFeature_SetInstance_ID = dr.getInt(2);	//	needed ?
                    retValue = new MVAMPFeatureSetInstance(ctx, 0, VAM_PFeature_Set_ID, null);
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
               _log.Log(Level.SEVERE, sql, ex);
            }
            finally { dt = null; }
            return retValue;
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MVAMPFeatureSetInstance(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAM_PFeature_SetInstance_ID id
         * 	@param VAM_PFeature_Set_ID attribute set
         *	@param trxName transaction
         */
        public MVAMPFeatureSetInstance(Ctx ctx, int VAM_PFeature_SetInstance_ID,
            int VAM_PFeature_Set_ID, Trx trxName)
            : this(ctx, VAM_PFeature_SetInstance_ID, trxName)
        {
            SetVAM_PFeature_Set_ID(VAM_PFeature_Set_ID);
        }

        /**
         * 	Set Attribute Set
         * 	@param mas attribute set
         */
        public void SetMVAMPFeatureSet(MVAMPFeatureSet mas)
        {
            _mas = mas;
            SetVAM_PFeature_Set_ID(mas.GetVAM_PFeature_Set_ID());
        }

        /**
         * 	Get Attribute Set
         *	@return Attrbute Set or null
         */
        public MVAMPFeatureSet GetMVAMPFeatureSet()
        {
            if (_mas == null && GetVAM_PFeature_Set_ID() != 0)
                _mas = new MVAMPFeatureSet(GetCtx(), GetVAM_PFeature_Set_ID(), Get_TrxName());
            return _mas;
        }

        /**
         * 	Set Description.
         * 	- Product Values
         * 	- Instance Values
         * 	- SerNo	= #123
         *  - Lot 	= \u00ab123\u00bb
         *  - GuaranteeDate	= 10/25/2003
         */
        public void SetDescription()
        {
            //	Make sure we have a Attribute Set
            GetMVAMPFeatureSet();
            if (_mas == null)
            {
                SetDescription("");
                return;
            }
            StringBuilder sb = new StringBuilder();
            //	Instance Attribute Values
            MVAMProductFeature[] attributes = _mas.GetMAttributes(true);
            for (int i = 0; i < attributes.Length; i++)
            {
                MVAMPFeatueInstance mai = attributes[i].GetMVAMPFeatueInstance(GetVAM_PFeature_SetInstance_ID());
                if (mai != null && mai.GetValue() != null)
                {
                    if (sb.Length > 0)
                        sb.Append("_");
                    sb.Append(mai.GetValue());
                }
            }
            //	SerNo
            if (_mas.IsSerNo() && GetSerNo() != null)
            {
                if (sb.Length > 0)
                    sb.Append("_");
                sb.Append(_mas.GetSerNoCharStart()).Append(GetSerNo()).Append(_mas.GetSerNoCharEnd());
            }
            //	Lot
            if (_mas.IsLot() && GetLot() != null)
            {
                if (sb.Length > 0)
                    sb.Append("_");
                sb.Append(_mas.GetLotCharStart()).Append(GetLot()).Append(_mas.GetLotCharEnd());
            }
            //	GuaranteeDate
            if (_mas.IsGuaranteeDate() && GetGuaranteeDate() != null)
            {
                if (sb.Length > 0)
                    sb.Append("_");
                sb.Append(_dateFormat.Format(GetGuaranteeDate()));
                //MessageBox.Show("set date time formate cheak line");
                //sb.Append(GetGuaranteeDate());
            }

            //	Product Attribute Values
            attributes = _mas.GetMAttributes(false);
            for (int i = 0; i < attributes.Length; i++)
            {
                MVAMPFeatueInstance mai = attributes[i].GetMVAMPFeatueInstance(GetVAM_PFeature_SetInstance_ID());
                if (mai != null && mai.GetValue() != null)
                {
                    if (sb.Length > 0)
                        sb.Append("_");
                    sb.Append(mai.GetValue());
                }
            }
            SetDescription(sb.ToString());
        }


        /**
         * 	Get Guarantee Date
         * 	@param getNew if true calculates/sets guarantee date
         *	@return guarantee date or null if days = 0
         */
        public DateTime? GetGuaranteeDate(bool getNew)
        {
            if (getNew)
            {
                int days = GetMVAMPFeatureSet().GetGuaranteeDays();
                if (days > 0)
                {
                    DateTime ts = TimeUtil.AddDays( DateTime.Now, days);
                    SetGuaranteeDate(ts);
                }
            }
            return GetGuaranteeDate();
        }

        /**
         * 	Get Lot No
         * 	@param getNew if true create/set new lot
         * 	@param VAM_Product_ID product used if new
         *	@return lot
         */
        public String GetLot(bool getNew, int VAM_Product_ID)
        {
            if (getNew)
                CreateLot(VAM_Product_ID);
            return GetLot();
        }

        /**
         * 	Create Lot
         * 	@param VAM_Product_ID product used if new
         *	@return lot info
         */
        public KeyNamePair CreateLot(int VAM_Product_ID)
        {
            KeyNamePair retValue = null;
            int VAM_LotControl_ID = GetMVAMPFeatureSet().GetVAM_LotControl_ID();
            if (VAM_LotControl_ID != 0)
            {
                MVAMLotControl ctl = new MVAMLotControl(GetCtx(), VAM_LotControl_ID, null);
                MVAMLot lot = ctl.CreateLot(VAM_Product_ID);
                SetVAM_Lot_ID(lot.GetVAM_Lot_ID());
                SetLot(lot.GetName());
                retValue = new KeyNamePair(lot.GetVAM_Lot_ID(), lot.GetName());
            }
            return retValue;
        }

        /**
         * 	To to find lot and set Lot/ID
         *	@param Lot lot
         *	@param VAM_Product_ID product
         */
        public void SetLot(String Lot, int VAM_Product_ID)
        {
            //	Try to find it
            MVAMLot MVAMLot = MVAMLot.GetProductLot(GetCtx(), VAM_Product_ID, Lot, Get_TrxName());
            if (MVAMLot != null)
                SetVAM_Lot_ID(MVAMLot.GetVAM_Lot_ID());
            SetLot(Lot);
        }

        /**
         * 	Exclude Lot creation
         *	@param VAF_Column_ID column
         *	@param isSOTrx SO
         *	@return true if excluded
         */
        public bool IsExcludeLot(int VAF_Column_ID, bool isSOTrx)
        {
            GetMVAMPFeatureSet();
            if (_mas != null)
                return _mas.IsExcludeLot(VAF_Column_ID, isSOTrx);
            return false;
        }

        /**
         *	Get Serial No
         * 	@param getNew if true create/set new Ser No
         *	@return Serial Number
         */
        public String GetSerNo(bool getNew)
        {
            if (getNew)
            {
                int VAM_CtlSerialNo_ID = GetMVAMPFeatureSet().GetVAM_CtlSerialNo_ID();
                if (VAM_CtlSerialNo_ID != 0)
                {
                    MSerNoCtl ctl = new MSerNoCtl(GetCtx(), VAM_CtlSerialNo_ID, Get_TrxName());
                    SetSerNo(ctl.CreateSerNo());
                }
            }
            return GetSerNo();
        }

        /**
         *	Exclude SerNo creation
         *	@param VAF_Column_ID column
         *	@param isSOTrx SO
         *	@return true if excluded
         */
        public bool IsExcludeSerNo(int VAF_Column_ID, bool isSOTrx)
        {
            GetMVAMPFeatureSet();
            if (_mas != null)
                return _mas.IsExcludeSerNo(VAF_Column_ID, isSOTrx);
            return false;
        }


    }
}

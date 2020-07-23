/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAttributeSetInstance
 * Purpose        : Get attribute instance from M_Product tab;e
 * Class Used     : X_M_AttributeSetInstance
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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
namespace VAdvantage.Model
{
    public class MAttributeSetInstance : X_M_AttributeSetInstance
    {
        #region Private variable
        /**	Attribute Set				*/
        private MAttributeSet _mas = null;
        /**	Date Format					*/
        private SimpleDateFormat _dateFormat = DisplayType.GetDateFormat(DisplayType.Date);
        //private DateTimePickerFormat _dateFormat = DisplayType.getDateFormat(DisplayType.Date);
        //private DateTime _dateFormat = new DateTime();
        private static VLogger _log = VLogger.GetVLogger(typeof(MAttributeSetInstance).FullName);
        #endregion

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_AttributeSetInstance_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MAttributeSetInstance(Ctx ctx, int M_AttributeSetInstance_ID, Trx trxName)
            : base(ctx, M_AttributeSetInstance_ID, trxName)
        {
            if (M_AttributeSetInstance_ID == 0)
            {
            }
        }

        /**
 * 	Get Attribute Set Instance from ID or Product
 *	@param ctx context
 * 	@param M_AttributeSetInstance_ID id or 0
 * 	@param M_Product_ID required if id is 0
 * 	@return Attribute Set Instance or null
 */
        public static MAttributeSetInstance Get(Ctx ctx,
            int M_AttributeSetInstance_ID, int M_Product_ID)
        {
            MAttributeSetInstance retValue = null;
            //	Load Instance if not 0
            if (M_AttributeSetInstance_ID != 0)
            {
               _log.Fine("From M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID);
                return new MAttributeSetInstance(ctx, M_AttributeSetInstance_ID, null);
            }
            //	Get new from Product
           _log.Fine("From M_Product_ID=" + M_Product_ID);
            if (M_Product_ID == 0)
                return null;
            String sql = "SELECT M_AttributeSet_ID, M_AttributeSetInstance_ID "
                + "FROM M_Product "
                + "WHERE M_Product_ID=" + M_Product_ID;
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
                    int M_AttributeSet_ID = Utility.Util.GetValueOfInt(dr[0].ToString()); //Convert.ToInt32(dr[0]);//.getInt(1);
                    //	M_AttributeSetInstance_ID = dr.getInt(2);	//	needed ?
                    retValue = new MAttributeSetInstance(ctx, 0, M_AttributeSet_ID, null);
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
        public MAttributeSetInstance(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param M_AttributeSetInstance_ID id
         * 	@param M_AttributeSet_ID attribute set
         *	@param trxName transaction
         */
        public MAttributeSetInstance(Ctx ctx, int M_AttributeSetInstance_ID,
            int M_AttributeSet_ID, Trx trxName)
            : this(ctx, M_AttributeSetInstance_ID, trxName)
        {
            SetM_AttributeSet_ID(M_AttributeSet_ID);
        }

        /**
         * 	Set Attribute Set
         * 	@param mas attribute set
         */
        public void SetMAttributeSet(MAttributeSet mas)
        {
            _mas = mas;
            SetM_AttributeSet_ID(mas.GetM_AttributeSet_ID());
        }

        /**
         * 	Get Attribute Set
         *	@return Attrbute Set or null
         */
        public MAttributeSet GetMAttributeSet()
        {
            if (_mas == null && GetM_AttributeSet_ID() != 0)
                _mas = new MAttributeSet(GetCtx(), GetM_AttributeSet_ID(), Get_TrxName());
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
            GetMAttributeSet();
            if (_mas == null)
            {
                SetDescription("");
                return;
            }
            StringBuilder sb = new StringBuilder();
            //	Instance Attribute Values
            MAttribute[] attributes = _mas.GetMAttributes(true);
            for (int i = 0; i < attributes.Length; i++)
            {
                MAttributeInstance mai = attributes[i].GetMAttributeInstance(GetM_AttributeSetInstance_ID());
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
                MAttributeInstance mai = attributes[i].GetMAttributeInstance(GetM_AttributeSetInstance_ID());
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
                int days = GetMAttributeSet().GetGuaranteeDays();
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
         * 	@param M_Product_ID product used if new
         *	@return lot
         */
        public String GetLot(bool getNew, int M_Product_ID)
        {
            if (getNew)
                CreateLot(M_Product_ID);
            return GetLot();
        }

        /**
         * 	Create Lot
         * 	@param M_Product_ID product used if new
         *	@return lot info
         */
        public KeyNamePair CreateLot(int M_Product_ID)
        {
            KeyNamePair retValue = null;
            int M_LotCtl_ID = GetMAttributeSet().GetM_LotCtl_ID();
            if (M_LotCtl_ID != 0)
            {
                MLotCtl ctl = new MLotCtl(GetCtx(), M_LotCtl_ID, null);
                MLot lot = ctl.CreateLot(M_Product_ID);
                SetM_Lot_ID(lot.GetM_Lot_ID());
                SetLot(lot.GetName());
                retValue = new KeyNamePair(lot.GetM_Lot_ID(), lot.GetName());
            }
            return retValue;
        }

        /**
         * 	To to find lot and set Lot/ID
         *	@param Lot lot
         *	@param M_Product_ID product
         */
        public void SetLot(String Lot, int M_Product_ID)
        {
            //	Try to find it
            MLot mLot = MLot.GetProductLot(GetCtx(), M_Product_ID, Lot, Get_TrxName());
            if (mLot != null)
                SetM_Lot_ID(mLot.GetM_Lot_ID());
            SetLot(Lot);
        }

        /**
         * 	Exclude Lot creation
         *	@param AD_Column_ID column
         *	@param isSOTrx SO
         *	@return true if excluded
         */
        public bool IsExcludeLot(int AD_Column_ID, bool isSOTrx)
        {
            GetMAttributeSet();
            if (_mas != null)
                return _mas.IsExcludeLot(AD_Column_ID, isSOTrx);
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
                int M_SerNoCtl_ID = GetMAttributeSet().GetM_SerNoCtl_ID();
                if (M_SerNoCtl_ID != 0)
                {
                    MSerNoCtl ctl = new MSerNoCtl(GetCtx(), M_SerNoCtl_ID, Get_TrxName());
                    SetSerNo(ctl.CreateSerNo());
                }
            }
            return GetSerNo();
        }

        /**
         *	Exclude SerNo creation
         *	@param AD_Column_ID column
         *	@param isSOTrx SO
         *	@return true if excluded
         */
        public bool IsExcludeSerNo(int AD_Column_ID, bool isSOTrx)
        {
            GetMAttributeSet();
            if (_mas != null)
                return _mas.IsExcludeSerNo(AD_Column_ID, isSOTrx);
            return false;
        }


    }
}

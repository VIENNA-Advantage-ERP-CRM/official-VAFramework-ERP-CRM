using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVABGenFeatureSetInstance: X_VAB_GenFeatureSetInstance
    {

        private MVABGenFeatureSet _mas = null;
        /**	Date Format					*/
        private SimpleDateFormat _dateFormat = DisplayType.GetDateFormat(DisplayType.Date);
        public MVABGenFeatureSetInstance(Ctx ctx, int VAB_GenFeatureSetInstance_ID, Trx trxName)
            : base(ctx, VAB_GenFeatureSetInstance_ID, trxName)
        {


        }

         /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MVABGenFeatureSetInstance(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }
        /**
         * 	Get Attribute Set
         *	@return Attrbute Set or null
         */
        public MVABGenFeatureSet GetMVABGenFeatureSet()
        {
            if (_mas == null && GetVAB_GenFeatureSet_ID() != 0)
                _mas = new MVABGenFeatureSet(GetCtx(), GetVAB_GenFeatureSet_ID(), Get_TrxName());
            return _mas;
        }
        public void SetDescription()
        {
            //	Make sure we have a Attribute Set
            GetCGenAttributeSet();
            if (_mas == null)
            {
                SetDescription("");
                return;
            }
            StringBuilder sb = new StringBuilder();
            //	Instance Attribute Values
            MVABGenFeature[] attributes = _mas.GetCGenAttributes(true);
            for (int i = 0; i < attributes.Length; i++)
            {
                MVABGenFeatureInstance mai = attributes[i].GetCGenAttributeInstance(GetVAB_GenFeatureSetInstance_ID());
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
            attributes = _mas.GetCGenAttributes(false);
            for (int i = 0; i < attributes.Length; i++)
            {
                MVABGenFeatureInstance mai = attributes[i].GetCGenAttributeInstance(GetVAB_GenFeatureSetInstance_ID());

                if (attributes[i].GetAttributeValueType().Equals("N"))
                {
                    if (mai != null && mai.GetValueNumber() != 0)
                    {
                        if (sb.Length > 0)
                            sb.Append("_");
                        sb.Append(mai.GetValueNumber());
                    }
                }
                else
                {
                    if (mai != null && mai.GetValue() != null)
                    {
                        if (sb.Length > 0)
                            sb.Append("_");
                        sb.Append(mai.GetValue());
                    }
                }
            }
            SetDescription(sb.ToString());
        }
        /**
       * 	Get Attribute Set
       *	@return Attrbute Set or null
       */
        public MVABGenFeatureSet GetCGenAttributeSet()
        {
            if (_mas == null && GetVAB_GenFeatureSet_ID() != 0)
                _mas = new MVABGenFeatureSet(GetCtx(), GetVAB_GenFeatureSet_ID(), Get_TrxName());
            return _mas;
        }
    }
}

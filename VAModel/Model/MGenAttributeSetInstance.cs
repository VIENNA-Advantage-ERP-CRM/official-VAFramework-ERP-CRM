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
    public class MGenAttributeSetInstance: X_C_GenAttributeSetInstance
    {

        private MGenAttributeSet _mas = null;
        /**	Date Format					*/
        private SimpleDateFormat _dateFormat = DisplayType.GetDateFormat(DisplayType.Date);
        public MGenAttributeSetInstance(Ctx ctx, int C_GenAttributeSetInstance_ID, Trx trxName)
            : base(ctx, C_GenAttributeSetInstance_ID, trxName)
        {


        }

         /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MGenAttributeSetInstance(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }
        /**
         * 	Get Attribute Set
         *	@return Attrbute Set or null
         */
        public MGenAttributeSet GetMGenAttributeSet()
        {
            if (_mas == null && GetC_GenAttributeSet_ID() != 0)
                _mas = new MGenAttributeSet(GetCtx(), GetC_GenAttributeSet_ID(), Get_TrxName());
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
            MGenAttribute[] attributes = _mas.GetCGenAttributes(true);
            for (int i = 0; i < attributes.Length; i++)
            {
                MGenAttributeInstance mai = attributes[i].GetCGenAttributeInstance(GetC_GenAttributeSetInstance_ID());
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
                MGenAttributeInstance mai = attributes[i].GetCGenAttributeInstance(GetC_GenAttributeSetInstance_ID());

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
        public MGenAttributeSet GetCGenAttributeSet()
        {
            if (_mas == null && GetC_GenAttributeSet_ID() != 0)
                _mas = new MGenAttributeSet(GetCtx(), GetC_GenAttributeSet_ID(), Get_TrxName());
            return _mas;
        }
    }
}

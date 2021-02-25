using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;
using System.Collections.Generic;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MGenAttributeSet: X_C_GenAttributeSet
    {

        //	Instance Attributes					
        private MGenAttribute[] _instanceAttributes = null;
        //	Instance Attributes					
        private MGenAttribute[] _productAttributes = null;

        public MGenAttributeSet(Ctx ctx, int C_GenAttributeSet_ID, Trx trxName)
            : base(ctx, C_GenAttributeSet_ID, trxName)
        {


        }

         /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MGenAttributeSet(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }


        /**
     * 	Get Attribute Array
     * 	@param instanceAttributes true if for instance
     *	@return instance or product attribute array
     */
        public MGenAttribute[] GetCGenAttributes(bool instanceAttributes)
        {
            if ((_instanceAttributes == null && instanceAttributes)
                || _productAttributes == null && !instanceAttributes)
            {
                String sql = "SELECT mau.C_GenAttribute_ID "
                    + "FROM C_GenAttributeUse mau"
                    + " INNER JOIN C_GenAttribute ma ON (mau.C_GenAttribute_ID=ma.C_GenAttribute_ID) "
                    + "WHERE mau.IsActive='Y' AND ma.IsActive='Y'"
                    + " AND mau.C_GenAttributeSet_ID=" + GetC_GenAttributeSet_ID() + " AND ma.IsInstanceAttribute= " +
                    ((instanceAttributes) ? "'Y'" : "'N'").ToString()
                    + " ORDER BY mau.SeqNo, mau.C_GenAttribute_ID";
                List<MGenAttribute> list = new List<MGenAttribute>();
                DataTable dt = null;
                IDataReader idr = DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                try
                {
                    //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    foreach (DataRow dr in dt.Rows)
                    {
                        //DataRow dr = ds.Tables[0].Rows[i];
                        MGenAttribute ma = new MGenAttribute(GetCtx(), Convert.ToInt32(dr[0]), Get_TrxName());
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
                finally
                {
                    dt = null;
                }

                //	Differentiate attributes
                if (instanceAttributes)
                {
                    _instanceAttributes = new MGenAttribute[list.Count];
                    _instanceAttributes = list.ToArray();
                }
                else
                {
                    _productAttributes = new MGenAttribute[list.Count];
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
    }
}

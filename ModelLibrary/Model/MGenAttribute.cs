using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using System.Collections.Generic;

namespace VAdvantage.Model
{
    public class MGenAttribute : X_C_GenAttribute
    {
        public MGenAttribute(Ctx ctx, int C_GenAttribute_ID, Trx trxName)
            : base(ctx, C_GenAttribute_ID, trxName)
        {


        }

         /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MGenAttribute(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }
        /// <summary>
        /// Get Attribute Instance
        /// </summary>
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <returns>Attribute Instance or null</returns>
        public MGenAttributeInstance GetCGenAttributeInstance(int C_GenAttributeSetInstance_ID)
        {
            MGenAttributeInstance retValue = null;
            String sql = "SELECT * "
                + "FROM C_GenAttributeInstance "
                + "WHERE C_GenAttribute_ID=" + GetC_GenAttribute_ID() + " AND C_GenAttributeSetInstance_ID=" + C_GenAttributeSetInstance_ID ;
            DataSet ds = null;
            try
            {
                ds = DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MGenAttributeInstance(GetCtx(), dr, Get_TrxName());
                }
                ds = null;
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, sql, ex);
            }
            ds = null;

            return retValue;
        }

        private MGenAttributeValue[] m_values = null;
        /// <summary>
        /// Get Values if List
        /// </summary>
        /// <returns>Values or null if not list</returns>
        public MGenAttributeValue[] GetMAttributeValues()
        {
            if (m_values == null && ATTRIBUTEVALUETYPE_List.Equals(GetAttributeValueType()))
            {
                List<MGenAttributeValue> list = new List<MGenAttributeValue>();
                MGenAttributeValue val = null;
                if (!IsMandatory())
                    list.Add(val);
                //
                String sql = "SELECT * FROM C_GenAttributeValue "
                    + "WHERE C_GenAttribute_ID=" + GetC_GenAttribute_ID()
                    + " ORDER BY Value";
                sql = MRole.GetDefault(GetCtx()).AddAccessSQL(sql, "C_GenAttributeValue", true, false);

                DataSet ds = null;
                try
                {
                    ds = DB.ExecuteDataset(sql, null);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        list.Add(new MGenAttributeValue(GetCtx(), dr, null));
                    }
                    ds = null;
                }
                catch (Exception ex)
                {
                    log.Log(Level.SEVERE, sql, ex);
                }

                ds = null;
                m_values = new MGenAttributeValue[list.Count];
                m_values = list.ToArray();
            }
            return m_values;
        }


        /// <summary>
        /// Set Attribute Instance
        /// </summary>
        /// <param name="M_AttributeSetInstance_ID">value</param>
        /// <param name="value">id</param>
        public void SetCGenAttributeInstance(int M_AttributeSetInstance_ID, MGenAttributeValue value)
        {
            MGenAttributeInstance instance = GetCGenAttributeInstance(M_AttributeSetInstance_ID);
            if (instance == null)
            {
                if (value != null)
                {
                    instance = new MGenAttributeInstance(GetCtx(), GetC_GenAttribute_ID(),
                      M_AttributeSetInstance_ID, value.GetC_GenAttributeValue_ID(),
                    value.GetName(), Get_TrxName()); 					//	Cached !!
                }
                else
                {
                    instance = new MGenAttributeInstance(GetCtx(), GetC_GenAttribute_ID(),
                        M_AttributeSetInstance_ID, 0, null, Get_TrxName());
                }
            }
            else
            {
                if (value != null)
                {
                    instance.SetC_GenAttributeValue_ID(value.GetC_GenAttributeValue_ID());
                    instance.SetValue(value.GetName()); 	//	Cached !!
                }
                else
                {
                    instance.SetC_GenAttributeValue_ID(0);
                    instance.SetValue(null);
                }
            }
            instance.Save();
        }

        /// <summary>
        /// Set Attribute Instance
        /// </summary>
        /// <param name="M_AttributeSetInstance_ID">number value</param>
        /// <param name="value">id</param>
        public void SetCGenAttributeInstance(int M_AttributeSetInstance_ID, Decimal? value)
        {
            MGenAttributeInstance instance = GetCGenAttributeInstance(M_AttributeSetInstance_ID);
            if (instance == null)
            {
                instance = new MGenAttributeInstance(GetCtx(), GetC_GenAttribute_ID(),
                    M_AttributeSetInstance_ID, value, Get_TrxName());
            }
            else
            {
                instance.SetValueNumber(value);
            }
            instance.Save();

        }
        /// <summary>
        /// Set Attribute Instance
        /// </summary>
        /// <param name="M_AttributeSetInstance_ID">id</param>
        /// <param name="value">string value</param>
        public void SetCGenAttributeInstance(int C_GenAttributeSetInstance_ID, String value)
        {
            MGenAttributeInstance instance = GetCGenAttributeInstance(C_GenAttributeSetInstance_ID);
            if (instance == null)
            {
                instance = new MGenAttributeInstance(GetCtx(), GetC_GenAttribute_ID(),
                    C_GenAttributeSetInstance_ID, value, Get_TrxName());
            }
            else
            {
                instance.SetValue(value);
            }
            instance.Save();
        }

    }
}

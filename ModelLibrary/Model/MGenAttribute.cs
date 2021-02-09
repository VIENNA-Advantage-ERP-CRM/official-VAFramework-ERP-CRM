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
    public class MGenAttribute : X_VAB_GenFeature
    {
        public MGenAttribute(Ctx ctx, int VAB_GenFeature_ID, Trx trxName)
            : base(ctx, VAB_GenFeature_ID, trxName)
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
        /// <param name="VAM_PFeature_SetInstance_ID">attribute set instance</param>
        /// <returns>Attribute Instance or null</returns>
        public MGenAttributeInstance GetCGenAttributeInstance(int VAB_GenFeatureSetInstance_ID)
        {
            MGenAttributeInstance retValue = null;
            String sql = "SELECT * "
                + "FROM VAB_GenFeatureInstance "
                + "WHERE VAB_GenFeature_ID=" + GetVAB_GenFeature_ID() + " AND VAB_GenFeatureSetInstance_ID=" + VAB_GenFeatureSetInstance_ID;
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
        public MGenAttributeValue[] GetMVAMPFeatureValues()
        {
            if (m_values == null && ATTRIBUTEVALUETYPE_List.Equals(GetAttributeValueType()))
            {
                List<MGenAttributeValue> list = new List<MGenAttributeValue>();
                MGenAttributeValue val = null;
                if (!IsMandatory())
                    list.Add(val);
                //
                String sql = "SELECT * FROM VAB_GenFeatureValue "
                    + "WHERE VAB_GenFeature_ID=" + GetVAB_GenFeature_ID()
                    + " ORDER BY Value";
                sql = MVAFRole.GetDefault(GetCtx()).AddAccessSQL(sql, "VAB_GenFeatureValue", true, false);

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
        /// <param name="VAM_PFeature_SetInstance_ID">value</param>
        /// <param name="value">id</param>
        public void SetCGenAttributeInstance(int VAM_PFeature_SetInstance_ID, MGenAttributeValue value)
        {
            MGenAttributeInstance instance = GetCGenAttributeInstance(VAM_PFeature_SetInstance_ID);
            if (instance == null)
            {
                if (value != null)
                {
                    instance = new MGenAttributeInstance(GetCtx(), GetVAB_GenFeature_ID(),
                      VAM_PFeature_SetInstance_ID, value.GetVAB_GenFeatureValue_ID(),
                    value.GetName(), Get_TrxName()); 					//	Cached !!
                }
                else
                {
                    instance = new MGenAttributeInstance(GetCtx(), GetVAB_GenFeature_ID(),
                        VAM_PFeature_SetInstance_ID, 0, null, Get_TrxName());
                }
            }
            else
            {
                if (value != null)
                {
                    instance.SetVAB_GenFeatureValue_ID(value.GetVAB_GenFeatureValue_ID());
                    instance.SetValue(value.GetName()); 	//	Cached !!
                }
                else
                {
                    instance.SetVAB_GenFeatureValue_ID(0);
                    instance.SetValue(null);
                }
            }
            instance.Save();
        }

        /// <summary>
        /// Set Attribute Instance
        /// </summary>
        /// <param name="VAM_PFeature_SetInstance_ID">number value</param>
        /// <param name="value">id</param>
        public void SetCGenAttributeInstance(int VAM_PFeature_SetInstance_ID, Decimal? value)
        {
            MGenAttributeInstance instance = GetCGenAttributeInstance(VAM_PFeature_SetInstance_ID);
            if (instance == null)
            {
                instance = new MGenAttributeInstance(GetCtx(), GetVAB_GenFeature_ID(),
                    VAM_PFeature_SetInstance_ID, value, Get_TrxName());
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
        /// <param name="VAM_PFeature_SetInstance_ID">id</param>
        /// <param name="value">string value</param>
        public void SetCGenAttributeInstance(int VAB_GenFeatureSetInstance_ID, String value)
        {
            MGenAttributeInstance instance = GetCGenAttributeInstance(VAB_GenFeatureSetInstance_ID);
            if (instance == null)
            {
                instance = new MGenAttributeInstance(GetCtx(), GetVAB_GenFeature_ID(),
                    VAB_GenFeatureSetInstance_ID, value, Get_TrxName());
            }
            else
            {
                instance.SetValue(value);
            }
            instance.Save();
        }

    }
}

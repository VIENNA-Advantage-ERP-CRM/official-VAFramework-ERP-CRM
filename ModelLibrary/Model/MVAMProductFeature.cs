/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMProductFeature
 * Purpose        : Used for VAM_ProductFeature table
 * Class Used     : X_VAM_ProductFeature
 * Chronological    Development
 * Raghunandan     04-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
//////using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAMProductFeature : X_VAM_ProductFeature
    {
        /// <summary>
        /// Get Attributes Of Client
        /// </summary>
        /// <param name="ctx">Properties</param>
        /// <param name="onlyProductAttributes">only Product Attributes</param>
        /// <param name="onlyListAttributes">st Attributes</param>
        /// <returns>array of attributes</returns>
        public static MVAMProductFeature[] GetOfClient(Ctx ctx, bool onlyProductAttributes, bool onlyListAttributes)
        {
            List<MVAMProductFeature> list = new List<MVAMProductFeature>();
            int VAF_Client_ID = ctx.GetVAF_Client_ID();
            String sql = "SELECT * FROM VAM_ProductFeature "
                + "WHERE VAF_Client_ID=" + VAF_Client_ID + " AND IsActive='Y'";
            if (onlyProductAttributes)
                sql += " AND IsInstanceAttribute='N'";
            if (onlyListAttributes)
                sql += " AND AttributeValueType='L'";
            sql += " ORDER BY Name";
            DataSet ds = null;
            try
            {
                ds = ExecuteQuery.ExecuteDataset(sql, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MVAMProductFeature(ctx, dr, null));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MVAMProductFeature[] retValue = new MVAMProductFeature[list.Count];
            retValue = list.ToArray();
            _log.Fine("VAF_Client_ID=" + VAF_Client_ID + " - #" + retValue.Length);
            return retValue;
        }

        //Logger
        // private static CLogger s_log = CLogger.GetCLogger(typeof(MVAMProductFeature));
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMProductFeature).FullName);

        /// <summary>
        /// 	Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_ProductFeature_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMProductFeature(Ctx ctx, int VAM_ProductFeature_ID, Trx trxName)
            : base(ctx, VAM_ProductFeature_ID, trxName)
        {
            if (VAM_ProductFeature_ID == 0)
            {
                SetAttributeValueType(ATTRIBUTEVALUETYPE_StringMax40);
                SetIsInstanceAttribute(false);
                SetIsMandatory(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">set</param>
        /// <param name="trxName">transaction</param>
        public MVAMProductFeature(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        //Values						
        private MAttributeValue[] m_values = null;

        /// <summary>
        /// Get Values if List
        /// </summary>
        /// <returns>Values or null if not list</returns>
        public MAttributeValue[] GetMAttributeValues()
        {
            if (m_values == null && ATTRIBUTEVALUETYPE_List.Equals(GetAttributeValueType()))
            {
                List<MAttributeValue> list = new List<MAttributeValue>();
                MAttributeValue val = null;
                if (!IsMandatory())
                    list.Add(val);
                //
                String sql = "SELECT * FROM VAM_PFeature_Value "
                    + "WHERE VAM_ProductFeature_ID=" + GetVAM_ProductFeature_ID()
                    + "ORDER BY Value";
                DataSet ds = null;
                try
                {
                    ds = ExecuteQuery.ExecuteDataset(sql, null);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        list.Add(new MAttributeValue(GetCtx(), dr, null));
                    }
                    ds = null;
                }
                catch (Exception ex)
                {
                    log.Log(Level.SEVERE, sql, ex);
                }

                ds = null;
                m_values = new MAttributeValue[list.Count];
                m_values = list.ToArray();
            }
            return m_values;
        }

        /// <summary>
        /// Get Attribute Instance
        /// </summary>
        /// <param name="VAM_PFeature_SetInstance_ID">attribute set instance</param>
        /// <returns>Attribute Instance or null</returns>
        public MVAMPFeatueInstance GetMVAMPFeatueInstance(int VAM_PFeature_SetInstance_ID)
        {
            MVAMPFeatueInstance retValue = null;
            String sql = "SELECT * "
                + "FROM VAM_PFeatue_Instance "
                + "WHERE VAM_ProductFeature_ID=" + GetVAM_ProductFeature_ID() + " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MVAMPFeatueInstance(GetCtx(), dr, Get_TrxName());
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

        /// <summary>
        /// Set Attribute Instance
        /// </summary>
        /// <param name="VAM_PFeature_SetInstance_ID">value</param>
        /// <param name="value">id</param>
        public void SetMVAMPFeatueInstance(int VAM_PFeature_SetInstance_ID, MAttributeValue value)
        {
            MVAMPFeatueInstance instance = GetMVAMPFeatueInstance(VAM_PFeature_SetInstance_ID);
            if (instance == null)
            {
                if (value != null)
                {
                    instance = new MVAMPFeatueInstance(GetCtx(), GetVAM_ProductFeature_ID(),
                      VAM_PFeature_SetInstance_ID, value.GetVAM_PFeature_Value_ID(),
                    value.GetName(), Get_TrxName()); 					//	Cached !!
                }
                else
                {
                    instance = new MVAMPFeatueInstance(GetCtx(), GetVAM_ProductFeature_ID(),
                        VAM_PFeature_SetInstance_ID, 0, null, Get_TrxName());
                }
                // Create new Attribute Instances in * Organization
                instance.SetVAF_Org_ID(0);
            }
            else
            {
                if (value != null)
                {
                    instance.SetVAM_PFeature_Value_ID(value.GetVAM_PFeature_Value_ID());
                    instance.SetValue(value.GetName()); 	//	Cached !!
                }
                else
                {
                    instance.SetVAM_PFeature_Value_ID(0);
                    instance.SetValue(null);
                }
            }
            instance.Save();
        }

        /// <summary>
        /// Set Attribute Instance
        /// </summary>
        /// <param name="VAM_PFeature_SetInstance_ID">id</param>
        /// <param name="value">string value</param>
        public void SetMVAMPFeatueInstance(int VAM_PFeature_SetInstance_ID, String value)
        {
            MVAMPFeatueInstance instance = GetMVAMPFeatueInstance(VAM_PFeature_SetInstance_ID);
            if (instance == null)
            {
                instance = new MVAMPFeatueInstance(GetCtx(), GetVAM_ProductFeature_ID(),
                    VAM_PFeature_SetInstance_ID, value, Get_TrxName());
            }
            else
            {
                instance.SetValue(value);
            }
            instance.Save();
        }

        /// <summary>
        /// Set Attribute Instance
        /// </summary>
        /// <param name="VAM_PFeature_SetInstance_ID">number value</param>
        /// <param name="value">id</param>
        public void SetMVAMPFeatueInstance(int VAM_PFeature_SetInstance_ID, Decimal? value)
        {
            MVAMPFeatueInstance instance = GetMVAMPFeatueInstance(VAM_PFeature_SetInstance_ID);
            if (instance == null)
            {
                instance = new MVAMPFeatueInstance(GetCtx(), GetVAM_ProductFeature_ID(),
                    VAM_PFeature_SetInstance_ID, value, Get_TrxName());
            }
            else
            {
                instance.SetValueNumber(value);

            }
            instance.Save();
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMProductFeature[");
            sb.Append(Get_ID()).Append("-").Append(GetName())
                .Append(",Type=").Append(GetAttributeValueType())
                .Append(",Instance=").Append(IsInstanceAttribute())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// AfterSave
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //	Changed to Instance Attribute
            if (!newRecord && Is_ValueChanged("IsInstanceAttribute") && IsInstanceAttribute())
            {
                String sql = "UPDATE VAM_PFeature_Set mas "
                    + "SET IsInstanceAttribute='Y' "
                    + "WHERE IsInstanceAttribute='N'"
                    + " AND EXISTS (SELECT * FROM VAM_PFeature_Use mau "
                        + "WHERE mas.VAM_PFeature_Set_ID=mau.VAM_PFeature_Set_ID"
                        + " AND mau.VAM_ProductFeature_ID=" + GetVAM_ProductFeature_ID() + ")";
                int no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
                log.Fine("AttributeSet Instance set #" + no);
            }
            return success;
        }
    }
}

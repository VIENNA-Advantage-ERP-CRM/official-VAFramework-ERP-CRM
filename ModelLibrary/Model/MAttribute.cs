/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAttribute
 * Purpose        : Used for M_Attribute table
 * Class Used     : X_M_Attribute
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
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MAttribute : X_M_Attribute
    {
        /// <summary>
        /// Get Attributes Of Client
        /// </summary>
        /// <param name="ctx">Properties</param>
        /// <param name="onlyProductAttributes">only Product Attributes</param>
        /// <param name="onlyListAttributes">st Attributes</param>
        /// <returns>array of attributes</returns>
        public static MAttribute[] GetOfClient(Ctx ctx, bool onlyProductAttributes, bool onlyListAttributes)
        {
            List<MAttribute> list = new List<MAttribute>();
            int AD_Client_ID = ctx.GetAD_Client_ID();
            String sql = "SELECT * FROM M_Attribute "
                + "WHERE AD_Client_ID=" + AD_Client_ID + " AND IsActive='Y'";
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
                    list.Add(new MAttribute(ctx, dr, null));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MAttribute[] retValue = new MAttribute[list.Count];
            retValue = list.ToArray();
            _log.Fine("AD_Client_ID=" + AD_Client_ID + " - #" + retValue.Length);
            return retValue;
        }

        //Logger
        // private static CLogger s_log = CLogger.GetCLogger(typeof(MAttribute));
        private static VLogger _log = VLogger.GetVLogger(typeof(MAttribute).FullName);

        /// <summary>
        /// 	Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Attribute_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MAttribute(Ctx ctx, int M_Attribute_ID, Trx trxName)
            : base(ctx, M_Attribute_ID, trxName)
        {
            if (M_Attribute_ID == 0)
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
        public MAttribute(Ctx ctx, DataRow dr, Trx trxName)
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
                String sql = "SELECT * FROM M_AttributeValue "
                    + "WHERE M_Attribute_ID=" + GetM_Attribute_ID()
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
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <returns>Attribute Instance or null</returns>
        public MAttributeInstance GetMAttributeInstance(int M_AttributeSetInstance_ID)
        {
            MAttributeInstance retValue = null;
            String sql = "SELECT * "
                + "FROM M_AttributeInstance "
                + "WHERE M_Attribute_ID=" + GetM_Attribute_ID() + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MAttributeInstance(GetCtx(), dr, Get_TrxName());
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
        /// <param name="M_AttributeSetInstance_ID">value</param>
        /// <param name="value">id</param>
        public void SetMAttributeInstance(int M_AttributeSetInstance_ID, MAttributeValue value)
        {
            MAttributeInstance instance = GetMAttributeInstance(M_AttributeSetInstance_ID);
            if (instance == null)
            {
                if (value != null)
                {
                    instance = new MAttributeInstance(GetCtx(), GetM_Attribute_ID(),
                      M_AttributeSetInstance_ID, value.GetM_AttributeValue_ID(),
                    value.GetName(), Get_TrxName()); 					//	Cached !!
                }
                else
                {
                    instance = new MAttributeInstance(GetCtx(), GetM_Attribute_ID(),
                        M_AttributeSetInstance_ID, 0, null, Get_TrxName());
                }
            }
            else
            {
                if (value != null)
                {
                    instance.SetM_AttributeValue_ID(value.GetM_AttributeValue_ID());
                    instance.SetValue(value.GetName()); 	//	Cached !!
                }
                else
                {
                    instance.SetM_AttributeValue_ID(0);
                    instance.SetValue(null);
                }
            }
            instance.Save();
        }

        /// <summary>
        /// Set Attribute Instance
        /// </summary>
        /// <param name="M_AttributeSetInstance_ID">id</param>
        /// <param name="value">string value</param>
        public void SetMAttributeInstance(int M_AttributeSetInstance_ID, String value)
        {
            MAttributeInstance instance = GetMAttributeInstance(M_AttributeSetInstance_ID);
            if (instance == null)
            {
                instance = new MAttributeInstance(GetCtx(), GetM_Attribute_ID(),
                    M_AttributeSetInstance_ID, value, Get_TrxName());
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
        /// <param name="M_AttributeSetInstance_ID">number value</param>
        /// <param name="value">id</param>
        public void SetMAttributeInstance(int M_AttributeSetInstance_ID, Decimal? value)
        {
            MAttributeInstance instance = GetMAttributeInstance(M_AttributeSetInstance_ID);
            if (instance == null)
            {
                instance = new MAttributeInstance(GetCtx(), GetM_Attribute_ID(),
                    M_AttributeSetInstance_ID, value, Get_TrxName());
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
        public override  String ToString()
        {
            StringBuilder sb = new StringBuilder("MAttribute[");
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
                String sql = "UPDATE M_AttributeSet mas "
                    + "SET IsInstanceAttribute='Y' "
                    + "WHERE IsInstanceAttribute='N'"
                    + " AND EXISTS (SELECT * FROM M_AttributeUse mau "
                        + "WHERE mas.M_AttributeSet_ID=mau.M_AttributeSet_ID"
                        + " AND mau.M_Attribute_ID=" + GetM_Attribute_ID() + ")";
                int no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
                log.Fine("AttributeSet Instance set #" + no);
            }
            return success;
        }
    }
}

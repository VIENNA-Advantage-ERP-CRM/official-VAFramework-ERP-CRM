/********************************************************
 * Module Name    : Workflow
 * Purpose        : Workflow Node Process Parameter Model
 * Chronological Development
 * Mukesh Arora     28-April-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.Process;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.WF
{
    /// <summary>
    /// Workflow Node Process Parameter Model
    /// </summary>
    public class MVAFWFlowNodePara : X_VAF_WFlow_Node_Para
    {
        // Linked Process Parameter      
        private MVAFJobPara _processPara = null;
        // Static Log
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAFWFlowNodePara).FullName);

        //private static final long serialVersionUID = 1L;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_WFlow_Node_Para_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowNodePara(Ctx ctx, int VAF_WFlow_Node_Para_ID, Trx trxName)
            : base(ctx, VAF_WFlow_Node_Para_ID, trxName)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowNodePara(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
       
        /// <summary>
        /// Get Parameters for a node
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_WFlow_Node_ID">node</param>
        /// <returns>array of parameters</returns>
        public static MVAFWFlowNodePara[] GetParameters(Ctx ctx, int VAF_WFlow_Node_ID)
        {
            List<MVAFWFlowNodePara> list = new List<MVAFWFlowNodePara>();
            String sql = "SELECT * FROM VAF_WFlow_Node_Para "
                + "WHERE VAF_WFlow_Node_ID=" + VAF_WFlow_Node_ID;

            //String sql = "SELECT * FROM VAF_Org WHERE VAF_Client_ID=" + VAF_WFlow_Node_ID;
            DataSet ds = null;

            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    DataRow dr = null;
                    int totCount = ds.Tables[0].Rows.Count;
                    for (int i = 0; i < totCount; i++)
                    {
                        dr = ds.Tables[0].Rows[i];
                        list.Add(new MVAFWFlowNodePara(ctx, dr, null));
                    }
                    dr = null;
                }
            }

            catch (Exception e)
            {
                _log.Log(Level.SEVERE, "GetParameters", e);
            }
            finally
            {
                ds = null;

            }

            //Make a array of MWFNodePara object and return
            MVAFWFlowNodePara[] retValue = new MVAFWFlowNodePara[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Process Parameter
        /// </summary>
        /// <returns>process parameter</returns>
        public MVAFJobPara GetProcessPara()
        {
            if (_processPara == null)
                _processPara = new MVAFJobPara(GetCtx(), GetVAF_Job_Para_ID(), Get_TrxName());
            return _processPara;
        }

        /// <summary>
        /// Get Attribute Name.
	    ///	If not set - retrieve it
        /// </summary>
        /// <returns>attribute name</returns>
        public new String GetAttributeName ()
	    {
		    String an = base.GetAttributeName();
		    if (an == null || an.Length == 0 && GetVAF_Job_Para_ID() != 0)
		    {
                an = GetProcessPara().GetColumnName();
			    SetAttributeName(an);
			    Save();
		    }
		    return an;
	    }

        /// <summary>
        /// Get Display Type
        /// </summary>
        /// <returns>display type</returns>
        public int GetDisplayType()
        {
            return GetProcessPara().GetVAF_Control_Ref_ID();
        }

        /// <summary>
        /// Is Mandatory
        /// </summary>
        /// <returns>true if mandatory</returns>
        public bool IsMandatory()
        {
            return GetProcessPara().IsMandatory();
        }

        /// <summary>
        /// Set VAF_Job_Para_ID
        /// </summary>
        /// <param name="VAF_Job_Para_ID">id</param>
	    public new void SetVAF_Job_Para_ID (int VAF_Job_Para_ID)
	    {
		    base.SetVAF_Job_Para_ID(VAF_Job_Para_ID);
		    SetAttributeName(null);
	    }

    }
}

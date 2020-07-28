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
    public class MWFNodePara : X_AD_WF_Node_Para
    {
        // Linked Process Parameter      
        private MProcessPara _processPara = null;
        // Static Log
        private static VLogger _log = VLogger.GetVLogger(typeof(MWFNodePara).FullName);

        //private static final long serialVersionUID = 1L;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_WF_Node_Para_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MWFNodePara(Ctx ctx, int AD_WF_Node_Para_ID, Trx trxName)
            : base(ctx, AD_WF_Node_Para_ID, trxName)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MWFNodePara(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
       
        /// <summary>
        /// Get Parameters for a node
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_WF_Node_ID">node</param>
        /// <returns>array of parameters</returns>
        public static MWFNodePara[] GetParameters(Ctx ctx, int AD_WF_Node_ID)
        {
            List<MWFNodePara> list = new List<MWFNodePara>();
            String sql = "SELECT * FROM AD_WF_Node_Para "
                + "WHERE AD_WF_Node_ID=" + AD_WF_Node_ID;

            //String sql = "SELECT * FROM AD_Org WHERE AD_Client_ID=" + AD_WF_Node_ID;
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
                        list.Add(new MWFNodePara(ctx, dr, null));
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
            MWFNodePara[] retValue = new MWFNodePara[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Process Parameter
        /// </summary>
        /// <returns>process parameter</returns>
        public MProcessPara GetProcessPara()
        {
            if (_processPara == null)
                _processPara = new MProcessPara(GetCtx(), GetAD_Process_Para_ID(), Get_TrxName());
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
		    if (an == null || an.Length == 0 && GetAD_Process_Para_ID() != 0)
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
            return GetProcessPara().GetAD_Reference_ID();
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
        /// Set AD_Process_Para_ID
        /// </summary>
        /// <param name="AD_Process_Para_ID">id</param>
	    public new void SetAD_Process_Para_ID (int AD_Process_Para_ID)
	    {
		    base.SetAD_Process_Para_ID(AD_Process_Para_ID);
		    SetAttributeName(null);
	    }

    }
}

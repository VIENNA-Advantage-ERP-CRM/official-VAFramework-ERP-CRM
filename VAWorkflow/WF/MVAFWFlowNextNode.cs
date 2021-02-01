/********************************************************
 * Module Name    : Workflow
 * Purpose        : 
 * Class Used     : MWFNodeNext inherits X_VAF_WFlow_NextNode
 * Chronological    Development
 * Raghunandan      01-May-2009
 * Veena Pandey     04-May-2009
  ******************************************************/
using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.WF
{
   public class MVAFWFlowNextNode : X_VAF_WFlow_NextNode
    {
        /** Transition Conditions			*/
        private MVAFWFlowNextCondition[] _conditions = null;
        /**	From (Split Eleemnt) is AND		*/
        public Boolean? _fromSplitAnd = null;
        /**	To (Join Element) is AND		*/
        public Boolean? _toJoinAnd = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_WFlow_NextNode_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowNextNode(Ctx ctx, int VAF_WFlow_NextNode_ID, Trx trxName)
            : base(ctx, VAF_WFlow_NextNode_ID, trxName) 
        {
            if (VAF_WFlow_NextNode_ID == 0)
            {
                //	setAD_WF_Next_ID (0);
                //	setVAF_WFlow_Node_ID (0);
                SetEntityType(ENTITYTYPE_UserMaintained);	// U
                SetIsStdUserWorkflow(false);
                SetSeqNo(10);	// 10
            }
        }


        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row to load info from</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowNextNode(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        { 
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="AD_WF_Next_ID">next id</param>
        public MVAFWFlowNextNode(MVAFWFlowNode parent, int AD_WF_Next_ID)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAF_WFlow_Node_ID(parent.GetVAF_WFlow_Node_ID());
            SetAD_WF_Next_ID(AD_WF_Next_ID);
        }

        /// <summary>
        /// Set Client Org
        /// </summary>
        /// <param name="VAF_Client_ID">client</param>
        /// <param name="VAF_Org_ID">org</param>
        //public void SetClientOrg(int VAF_Client_ID, int VAF_Org_ID)
        //{
        //    base.SetClientOrg(VAF_Client_ID, VAF_Org_ID);
        //}

        /// <summary>
        /// Get Conditions
        /// </summary>
        /// <param name="requery">true if requery</param>
        /// <returns>Array of Conditions</returns>
        public MVAFWFlowNextCondition[] GetConditions(bool requery)
        {
            if (!requery && _conditions != null)
                return _conditions;
            //
            List<MVAFWFlowNextCondition> list = new List<MVAFWFlowNextCondition>();
            String sql = "SELECT * FROM VAF_WFlow_NextCondition WHERE VAF_WFlow_NextNode_ID=" + GetVAF_WFlow_NextNode_ID() + " AND IsActive='Y' ORDER BY SeqNo";

            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    DataRow rs = null;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        rs = ds.Tables[0].Rows[i];
                        list.Add(new MVAFWFlowNextCondition(GetCtx(), rs, Get_TrxName()));
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            
            _conditions = new MVAFWFlowNextCondition[list.Count];
            _conditions = list.ToArray();
            return _conditions;
        }

        /// <summary>
        /// Unconditional
        /// </summary>
        /// <returns>true if no conditions</returns>
        public bool IsUnconditional()
        {
            return !IsStdUserWorkflow() && GetConditions(false).Length == 0;
        }

        /// <summary>
        /// Is this a Valid Transition For ..
        /// </summary>
        /// <param name="activity">activity</param>
        /// <returns>true if valid</returns>
        public bool IsValidFor (MVAFWFlowTask activity)
	    {
		    if (IsStdUserWorkflow())
		    {
			    PO po = activity.GetPO();
                if (po.GetType() == typeof(DocAction) || po.GetType().GetInterface("DocAction") == typeof(DocAction))
			    {
				    DocAction da = (DocAction)po;
				    String docStatus = da.GetDocStatus();
				    String docAction = da.GetDocAction();
                    if (!DocActionVariables.ACTION_COMPLETE.Equals(docAction)
					    || DocActionVariables.STATUS_COMPLETED.Equals(docStatus)
					    || DocActionVariables.STATUS_WAITINGCONFIRMATION.Equals(docStatus)
					    || DocActionVariables.STATUS_WAITINGPAYMENT.Equals(docStatus)
                        || DocActionVariables.STATUS_VOIDED.Equals(docStatus)
                        || DocActionVariables.STATUS_CLOSED.Equals(docStatus)
                        || DocActionVariables.STATUS_REVERSED.Equals(docStatus))
					    /*
					    || DocAction.ACTION_Complete.equals(docAction)	
					    || DocAction.ACTION_ReActivate.equals(docAction)	
					    || DocAction.ACTION_None.equals(docAction)
					    || DocAction.ACTION_Post.equals(docAction)
					    || DocAction.ACTION_Unlock.equals(docAction)
					    || DocAction.ACTION_Invalidate.equals(docAction)	) */
				    {
					    log.Fine("isValidFor =NO= StdUserWF - Status=" + docStatus + " - Action=" + docAction);
					    return false;
				    }
			    }
		    }
		    //	No Conditions
		    if (GetConditions(false).Length == 0)
		    {
			    log.Fine("#0 " + ToString());
			    return true;
		    }
		    //	First condition always AND
		    bool ok = _conditions[0].Evaluate(activity);
		    for (int i = 1; i < _conditions.Length; i++)
		    {
			    if (_conditions[i].IsOr())
				    ok = ok || _conditions[i].Evaluate(activity);
			    else
				    ok = ok && _conditions[i].Evaluate(activity);
		    }	//	for all conditions
		    log.Fine("isValidFor (" + ok + ") " + ToString());
		    return ok;
	    }

        /// <summary>
        /// Is Split Element is AND
        /// </summary>
        /// <returns>Returns the from Split And</returns>
        public bool IsFromSplitAnd()
        {
            if (_fromSplitAnd != null)
                return (bool)_fromSplitAnd;
            return false;
        }

        /// <summary>
        /// Split Element is AND.
        /// Set by MWFNode.loadNodes
        /// </summary>
        /// <param name="fromSplitAnd">The from Split And</param>
        public void SetFromSplitAnd(bool fromSplitAnd)
        {
            _fromSplitAnd = fromSplitAnd;
        }

        /// <summary>
        /// Join Element is AND
        /// </summary>
        /// <returns>Returns the to Join And.</returns>
        public bool IsToJoinAnd()
        {
            if (_toJoinAnd == null && GetAD_WF_Next_ID() != 0)
            {
                MVAFWFlowNode next = MVAFWFlowNode.Get(GetCtx(), GetAD_WF_Next_ID());
                SetToJoinAnd(MVAFWFlowNode.JOINELEMENT_AND.Equals(next.GetJoinElement()));
            }
            if (_toJoinAnd != null)
                return (bool)_toJoinAnd;
            return false;
        }

        /// <summary>
        /// Join Element is AND.
        /// </summary>
        /// <param name="toJoinAnd">The to Join And to set.</param>
        private void SetToJoinAnd(bool toJoinAnd)
        {
            _toJoinAnd = toJoinAnd;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MWFNodeNext[");
            sb.Append(GetSeqNo())
                .Append(":Node=").Append(GetVAF_WFlow_Node_ID()).Append("->Next=").Append(GetAD_WF_Next_ID());
            if (_conditions != null)
                sb.Append(",#").Append(_conditions.Length);
            if (GetDescription() != null && GetDescription().Length > 0)
                sb.Append(",").Append(GetDescription());
            sb.Append("]");
            return sb.ToString();
        }
    }
}

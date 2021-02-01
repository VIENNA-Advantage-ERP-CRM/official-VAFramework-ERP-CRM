/********************************************************
 * Module Name    : Workflow
 * Purpose        : 
 * Class Used     : X_VAF_WFlow_Handler
 * Chronological Development
 * Veena Pandey     04-May-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;
namespace VAdvantage.WF
{
    public class MVAFWFlowHandler : X_VAF_WFlow_Handler
    {
        /**	State Machine				*/
        private StateEngine _state = null;
        /**	Activities					*/
        private MVAFWFlowTask[] _activities = null;
        /**	Workflow					*/
        private MVAFWorkflow _wf = null;
        /**	Process Info				*/
        private ProcessInfo _pi = null;
        /**	Persistent Object			*/
        private PO _po = null;
        /** Message from Activity		*/
        private String _processMsg = null;

        //Thread thred;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_WFlow_Handler_ID">process id</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowHandler(Ctx ctx, int VAF_WFlow_Handler_ID, Trx trxName)
            : base(ctx, VAF_WFlow_Handler_ID, trxName)
        {
            if (VAF_WFlow_Handler_ID == 0)
                throw new ArgumentException("Cannot create new WF Process directly");
            _state = new StateEngine(GetWFState());
            _state.SetCtx(GetCtx());
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAFWFlowHandler(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            _state = new StateEngine(GetWFState());
            _state.SetCtx(GetCtx());
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="wf">workflow</param>
        /// <param name="pi">Process Info (Record_ID)</param>
        public MVAFWFlowHandler (MVAFWorkflow wf, ProcessInfo pi)
            : base(wf.GetCtx(), 0, wf.Get_TrxName())
	    {

            if (!Utility.TimeUtil.IsValid(wf.GetValidFrom(), wf.GetValidTo())) // make this class or this function
            {
			    //throw new IllegalStateException("Workflow not valid");
                throw new Exception("Workflow not valid");
            }
		    _wf = wf;
		    _pi = pi;
            SetVAF_Client_ID(wf.GetVAF_Client_ID());
		    SetVAF_Workflow_ID (wf.GetVAF_Workflow_ID());
		    SetPriority(wf.GetPriority());
		    base.SetWFState (WFSTATE_NotStarted);
    		
            // vinay bhatt for window id
            SetVAF_Screen_ID(pi.GetVAF_Screen_ID());
            //

		    //	Document
		    SetVAF_TableView_ID(wf.GetVAF_TableView_ID());
		    SetRecord_ID(pi.GetRecord_ID());
		    if (GetPO() == null)
		    {
			    SetTextMsg("No PO with ID=" + pi.GetRecord_ID());
			    base.SetWFState (WFSTATE_Terminated);
		    }
            else
                SetTextMsg(GetPO());
		    //	Responsible/User
            if (wf.GetVAF_WFlow_Incharge_ID() == 0)
                SetVAF_WFlow_Incharge_ID();
            else
                SetVAF_WFlow_Incharge_ID(wf.GetVAF_WFlow_Incharge_ID());
            SetUser_ID((int)pi.GetVAF_UserContact_ID());		//	user starting
		    //
		    _state = new StateEngine (GetWFState());
            _state.SetCtx(GetCtx());
		    SetProcessed (false);
		    //	Lock Entity
		    GetPO();
            if (_po != null)
            {
                // Set transaction organization on workflow process
                SetVAF_Org_ID(_po.GetVAF_Org_ID());
                _po.Lock();
            }
	    }

        /// <summary>
        /// Get active Activities of Process
        /// </summary>
        /// <param name="requery">if true requery</param>
        /// <param name="onlyActive">only active activities</param>
        /// <returns>array of activities</returns>
        public MVAFWFlowTask[] GetActivities(bool requery, bool onlyActive)
        {
            if (!requery && _activities != null)
                return _activities;
            //
            List<MVAFWFlowTask> list = new List<MVAFWFlowTask>();
            String sql = "SELECT * FROM VAF_WFlow_Task WHERE VAF_WFlow_Handler_ID=" + GetVAF_WFlow_Handler_ID() + "";
            if (onlyActive)
                sql += " AND Processed='N'";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    DataRow dr = null;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dr = ds.Tables[0].Rows[i];
                        list.Add(new MVAFWFlowTask(GetCtx(), dr, Get_TrxName()));
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            _activities = new MVAFWFlowTask[list.Count];
            _activities = list.ToArray();
            return _activities;
        }

        /// <summary>
        /// Get State
        /// </summary>
        /// <returns>state</returns>
        public StateEngine GetState()
        {
            return _state;
        }

        /// <summary>
        /// Get Action Options
        /// </summary>
        /// <returns>array of valid actions</returns>
        public String[] GetActionOptions()
        {
            return _state.GetActionOptions();
        }

        /// <summary>
        /// Set Process State and update Actions
        /// </summary>
        /// <param name="WFState">WFState</param>
        public new void   SetWFState(String WFState)
        {
            if (_state == null)
            {
                _state = new StateEngine(GetWFState());
                _state.SetCtx(GetCtx());
            }
            if (_state.IsClosed())
                return;
            if (GetWFState().Equals(WFState))
                return;
            //
            if (_state.IsValidNewState(WFState))
            {
                log.Fine(ToString() + " => " + WFState);
                base.SetWFState(WFState);
                _state = new StateEngine(GetWFState());
                _state.SetCtx(GetCtx());
                if (_state.IsClosed() || _state.IsBackground())
                    SetProcessed(true);
                Save();
                //	Force close to all Activities
                if (_state.IsClosed() || _state.IsBackground())
                {
                    MVAFWFlowTask[] activities = GetActivities(true, true);	//	requery only active
                    for (int i = 0; i < activities.Length; i++)
                    {
                        if (!activities[i].IsClosed())
                        {
                            activities[i].SetTextMsg("Process:" + WFState);
                            activities[i].SetWFState(WFState);
                        }
                        if (!activities[i].IsProcessed())
                            activities[i].SetProcessed(true);
                        activities[i].Save();
                    }
                }	//	closed
            }
            else
            {
                log.Log(Level.SEVERE, "Ignored Invalid Transformation " + ToString() + " => " + WFState);
            }
        }

        /// <summary>
        /// Check Status of Activities.
        /// - update Process if required
        /// - start new activity
        /// </summary>
        public void CheckActivities()
        {
            log.Info(ToString());
            if (_state.IsClosed())
                return;
            //
            MVAFWFlowTask[] activities = GetActivities(true, true);	//	requery active
            String closedState = null;
            bool suspended = false;
            bool running = false;
            for (int i = 0; i < activities.Length; i++)
            {
                MVAFWFlowTask activity = activities[i];
                activity.SetVAF_Screen_ID(GetVAF_Screen_ID());
                StateEngine activityState = activity.GetState();

                //	Completed - Start Next
                if (activityState.IsCompleted() || activityState.IsBackground())
                {
                    if (StartNext(activity, activities))
                        continue;
                }
                //
                String activityWFState = activity.GetWFState();
                if (activityState.IsClosed() || activityState.IsBackground())
                {
                    //	eliminate from active processed
                    //activity.SetProcessed(true);
                    activity.Set_ValueNoCheck("Processed", true);
                    //activities
                    activity.Save();
                    //
                    if (closedState == null)
                        closedState = activityWFState;
                    else if (!closedState.Equals(activityState))
                    {
                        //	Overwrite if terminated
                        if (WFSTATE_Terminated.Equals(activityState))
                            closedState = activityWFState;
                        //	Overwrite if activity aborted and no other terminated
                        else if (WFSTATE_Aborted.Equals(activityState) && !WFSTATE_Terminated.Equals(closedState))
                            closedState = activityWFState;
                    }
                }
                else	//	not closed
                {
                    closedState = null;		//	all need to be closed
                    if (activityState.IsSuspended())
                        suspended = true;
                    if (activityState.IsRunning())
                        running = true;
                }
            }	//	for all activities
            if (activities.Length == 0)
            {
                SetTextMsg("No Active Processed found");
                closedState = WFSTATE_Terminated;
            }
            if (closedState != null)
            {
                if (closedState == StateEngine.STATE_BACKGROUND)
                    _state.SetState(StateEngine.STATE_BACKGROUND);
                SetWFState(closedState);
                GetPO();
                if (_po != null)
                {
                    _po.Unlock(Get_TrxName());
                }
            }
            else if (suspended)
                SetWFState(WFSTATE_Suspended);
            else if (running)
                SetWFState(WFSTATE_Running);
        }

        /// <summary>
        /// Start Next Activity
        /// </summary>
        /// <param name="last">last activity</param>
        /// <param name="activities">all activities</param>
        /// <returns>true if there is a next activity</returns>
        private bool StartNext(MVAFWFlowTask last, MVAFWFlowTask[] activities)
        {
            log.Config("Last=" + last);
            //	transitions from the last processed node
            MVAFWFlowNextNode[] transitions = GetWorkflow().GetNodeNexts(last.GetVAF_WFlow_Node_ID(), last.GetVAF_Client_ID());
            if (transitions == null || transitions.Length == 0)
            {
                log.Config("none");
                return false;	//	done
            }

            //	We need to wait for last activity
            if (MVAFWFlowNode.JOINELEMENT_AND.Equals(last.GetNode().GetJoinElement()))
            {
                //	get previous nodes
                //	check if all have closed activities
                //	return false for all but the last
            }
            //	eliminate from active processed
            //last.SetProcessed(true);
            last.Set_ValueNoCheck("Processed", true);
            last.Save();

            //	Start next activity
            String split = last.GetNode().GetSplitElement();
            for (int i = 0; i < transitions.Length; i++)
            {
                //	Is this a valid transition?
                if (!transitions[i].IsValidFor(last))
                    continue;

                //	Start new Activity
                MVAFWFlowTask activity = new MVAFWFlowTask(this, transitions[i].GetAD_WF_Next_ID());
                // set Last Activity ID property in current WF Activity
                activity.SetLastActivity(last.GetVAF_WFlow_Task_ID());
                // new Thread(activity).Start();
                //thred = new Thread(new ThreadStart(activity.Run));
                //thred.CurrentCulture = Utility.Env.GetLanguage(Utility.Env.GetContext()).GetCulture(Utility.Env.GetLoginLanguage(Utility.Env.GetContext()).GetVAF_Language());
                //thred.CurrentUICulture = Utility.Env.GetLanguage(Utility.Env.GetContext()).GetCulture(Utility.Env.GetLoginLanguage(Utility.Env.GetContext()).GetVAF_Language());
                activity.Run();
               // thred.Start();

                //	only the first valid if XOR
                if (MVAFWFlowNode.SPLITELEMENT_XOR.Equals(split))
                    return true;
            }	//	for all transitions
            return true;
        }

        /// <summary>
        /// Set Workflow Responsible. Searches for a Invoker.
        /// </summary>
        public void SetVAF_WFlow_Incharge_ID()
        {
            int VAF_WFlow_Incharge_ID = DataBase.DB.GetSQLValue(null,
                    MVAFRole.GetDefault(GetCtx(), false).AddAccessSQL(
                    "SELECT VAF_WFlow_Incharge_ID FROM VAF_WFlow_Incharge "
                    + "WHERE ResponsibleType='H' AND COALESCE(VAF_UserContact_ID,0)=0 "
                    + "ORDER BY VAF_Client_ID DESC",
                    "VAF_WFlow_Incharge", MVAFRole.SQL_NOTQUALIFIED, MVAFRole.SQL_RO));
            SetVAF_WFlow_Incharge_ID(VAF_WFlow_Incharge_ID);
        }

        /// <summary>
        /// Set User from 
        /// - (1) Responsible
        /// - (2) Document Sales Rep
        /// - (3) Document UpdatedBy
        /// - (4) Process invoker
        /// </summary>
        /// <param name="User_ID">process invoker</param>
        private void SetUser_ID (int User_ID)
	    {
		    //	Responsible
		    MVAFWFlowIncharge resp = MVAFWFlowIncharge.Get(GetCtx(), GetVAF_WFlow_Incharge_ID());
		    //	(1) User - Directly responsible
		    int VAF_UserContact_ID = resp.GetVAF_UserContact_ID();
    		
		    //	Invoker - get Sales Rep or last updater of Document
		    if (VAF_UserContact_ID == 0 && resp.IsInvoker())
		    {
			    GetPO();
			    //	(2) Doc Owner
			    //if (_po != null && _po instanceof DocAction)
                if (_po != null && (_po.GetType() == typeof(DocAction) || _po.GetType().GetInterface("DocAction") == typeof(DocAction)))
			    {
				    DocAction da = (DocAction)_po;
				    VAF_UserContact_ID = da.GetDoc_User_ID();
			    }
			    //	(2) Sales Rep
			    if (VAF_UserContact_ID == 0 && _po != null && _po.Get_ColumnIndex("SalesRep_ID") != -1)
			    {
				    Object sr = _po.Get_Value("SalesRep_ID");
				    if (sr != null && sr.GetType() == typeof(int))
					    VAF_UserContact_ID = int.Parse(sr.ToString());
			    }
			    //	(3) UpdatedBy
			    if (VAF_UserContact_ID == 0 && _po != null)
				    VAF_UserContact_ID = _po.GetUpdatedBy();
		    }
    		
		    //	(4) Process Owner
		    if (VAF_UserContact_ID == 0 )
			    VAF_UserContact_ID = User_ID;
		    //	Fallback 
		    if (VAF_UserContact_ID == 0)
			    VAF_UserContact_ID = GetCtx().GetVAF_UserContact_ID();
		    //
		    SetVAF_UserContact_ID(VAF_UserContact_ID);
	    }

        /// <summary>
        /// Get Workflow
        /// </summary>
        /// <returns>workflow</returns>
        private MVAFWorkflow GetWorkflow()
        {
            if (_wf == null)
                _wf = MVAFWorkflow.Get(GetCtx(), GetVAF_Workflow_ID());
            if (_wf.Get_ID() == 0)
            {
                //throw new IllegalStateException("Not found - VAF_Workflow_ID=" + getVAF_Workflow_ID());
                throw new Exception("Not found - VAF_Workflow_ID=" + GetVAF_Workflow_ID());
            }
            return _wf;
        }

        /// <summary>
        /// Perform Action
        /// </summary>
        /// <param name="action">StateEngine.ACTION_*</param>
        /// <returns>true if valid</returns>
        public bool Perform(String action)
        {
            if (!_state.IsValidAction(action))
            {
                log.Log(Level.SEVERE, "Ignored Invalid Transformation - Action=" + action + " - " + ToString());
                return false;
            }
            log.Fine(action);
            //	Action is Valid
            if (StateEngine.ACTION_START.Equals(action))
                return StartWork();
            //	Set new State
            SetWFState(_state.GetNewStateIfAction(action));
            return true;
        }

        /// <summary>
        /// Start WF Execution async
        /// </summary>
        /// <returns>true if success</returns>
        public bool StartWork()
        {
            if (!_state.IsValidAction(StateEngine.ACTION_START))
            {
                log.Warning("State=" + GetWFState() + " - cannot start");
                return false;
            }
            int VAF_WFlow_Node_ID = GetWorkflow().GetVAF_WFlow_Node_ID();
            log.Fine("VAF_WFlow_Node_ID=" + VAF_WFlow_Node_ID);
            SetWFState(WFSTATE_Running);
            try
            {
                ////	Start first Activity with first Node
                //MVAFWFlowTask activity = new MVAFWFlowTask(this, VAF_WFlow_Node_ID);
                ////new Thread(activity).Start();
                //thred = new Thread(new ThreadStart(activity.Run));
                ////System.Threading.Thread.CurrentThread.CurrentCulture = Env.GetLanguage(p_ctx).GetCulture(Env.GetBaseVAF_Language());
                ////System.Threading.Thread.CurrentThread.CurrentUICulture = Env.GetLanguage(p_ctx).GetCulture(Env.GetBaseVAF_Language());

                //thred.CurrentCulture = Utility.Env.GetLanguage(p_ctx).GetCulture(Utility.Env.GetBaseVAF_Language());
                //thred.CurrentUICulture = Utility.Env.GetLanguage(p_ctx).GetCulture(Utility.Env.GetBaseVAF_Language());
                //thred.Start();

                //Update By --Raghu
                //Date-14-Feb-2012
                //remove thread logic for workflow becouse to show updated workflow record on window
                MVAFWFlowTask activity = new MVAFWFlowTask(this, VAF_WFlow_Node_ID);
                activity.Run();

            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "VAF_WFlow_Node_ID=" + VAF_WFlow_Node_ID, e);
                SetTextMsg(e.Message);
                SetWFState(StateEngine.STATE_TERMINATED);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get Persistent Object
        /// </summary>
        /// <returns>po</returns>
        public PO GetPO()
        {
            if (_po != null)
                return _po;
            if (GetRecord_ID() == 0)
                return null;

            MVAFTableView table = MVAFTableView.Get(GetCtx(), GetVAF_TableView_ID());
            _po = table.GetPO(GetCtx(), GetRecord_ID(), Get_TrxName());
            return _po;
        }

        /// <summary>
        /// Set Text Msg (add to existing)
        /// </summary>
        /// <param name="po">po base object</param>
        public void SetTextMsg (PO po)
	    {
            if (po != null && (po.GetType() == typeof(DocAction) || po.GetType().GetInterface("DocAction") == typeof(DocAction)))
			    SetTextMsg(((DocAction)po).GetSummary());
	    }

        /// <summary>
        /// Set Text Msg (add to existing)
        /// </summary>
        /// <param name="textMsg">msg</param>
        public new void SetTextMsg(String textMsg)
        {
            String oldText = GetTextMsg();
            if (oldText == null || oldText.Length == 0)
                base.SetTextMsg(textMsg);
            else if (textMsg != null && textMsg.Length > 0)
                base.SetTextMsg(oldText + "\n - " + textMsg);
        }

        /// <summary>
        /// Set Runtime (Error) Message
        /// </summary>
        /// <param name="msg">message</param>
        public void SetProcessMsg(String msg)
        {
            _processMsg = msg;
            if (msg != null && msg.Length > 0)
                SetTextMsg(msg);
        }

        /// <summary>
        /// Get Runtime (Error) Message
        /// </summary>
        /// <returns>msg</returns>
        public String GetProcessMsg()
        {
            return _processMsg;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAFWFlowHandler[")
                .Append(Get_ID())
                .Append(", VAF_Workflow_ID=").Append(GetVAF_Workflow_ID())
                .Append(", WFState=").Append(GetWFState())
                .Append("]");
            return sb.ToString();
        }
    }
}
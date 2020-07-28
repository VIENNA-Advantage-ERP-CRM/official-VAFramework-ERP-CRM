/********************************************************
 * Module Name    : Workflow
 * Purpose        : 
 * Class Used     : X_AD_WF_Process
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
    public class MWFProcess : X_AD_WF_Process
    {
        /**	State Machine				*/
        private StateEngine _state = null;
        /**	Activities					*/
        private MWFActivity[] _activities = null;
        /**	Workflow					*/
        private MWorkflow _wf = null;
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
        /// <param name="AD_WF_Process_ID">process id</param>
        /// <param name="trxName">transaction</param>
        public MWFProcess(Ctx ctx, int AD_WF_Process_ID, Trx trxName)
            : base(ctx, AD_WF_Process_ID, trxName)
        {
            if (AD_WF_Process_ID == 0)
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
        public MWFProcess(Ctx ctx, DataRow rs, Trx trxName)
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
        public MWFProcess (MWorkflow wf, ProcessInfo pi)
            : base(wf.GetCtx(), 0, wf.Get_TrxName())
	    {

            if (!Utility.TimeUtil.IsValid(wf.GetValidFrom(), wf.GetValidTo())) // make this class or this function
            {
			    //throw new IllegalStateException("Workflow not valid");
                throw new Exception("Workflow not valid");
            }
		    _wf = wf;
		    _pi = pi;
            SetAD_Client_ID(wf.GetAD_Client_ID());
		    SetAD_Workflow_ID (wf.GetAD_Workflow_ID());
		    SetPriority(wf.GetPriority());
		    base.SetWFState (WFSTATE_NotStarted);
    		
            // vinay bhatt for window id
            SetAD_Window_ID(pi.GetAD_Window_ID());
            //

		    //	Document
		    SetAD_Table_ID(wf.GetAD_Table_ID());
		    SetRecord_ID(pi.GetRecord_ID());
		    if (GetPO() == null)
		    {
			    SetTextMsg("No PO with ID=" + pi.GetRecord_ID());
			    base.SetWFState (WFSTATE_Terminated);
		    }
            else
                SetTextMsg(GetPO());
		    //	Responsible/User
            if (wf.GetAD_WF_Responsible_ID() == 0)
                SetAD_WF_Responsible_ID();
            else
                SetAD_WF_Responsible_ID(wf.GetAD_WF_Responsible_ID());
            SetUser_ID((int)pi.GetAD_User_ID());		//	user starting
		    //
		    _state = new StateEngine (GetWFState());
            _state.SetCtx(GetCtx());
		    SetProcessed (false);
		    //	Lock Entity
		    GetPO();
            if (_po != null)
            {
                // Set transaction organization on workflow process
                SetAD_Org_ID(_po.GetAD_Org_ID());
                _po.Lock();
            }
	    }

        /// <summary>
        /// Get active Activities of Process
        /// </summary>
        /// <param name="requery">if true requery</param>
        /// <param name="onlyActive">only active activities</param>
        /// <returns>array of activities</returns>
        public MWFActivity[] GetActivities(bool requery, bool onlyActive)
        {
            if (!requery && _activities != null)
                return _activities;
            //
            List<MWFActivity> list = new List<MWFActivity>();
            String sql = "SELECT * FROM AD_WF_Activity WHERE AD_WF_Process_ID=" + GetAD_WF_Process_ID() + "";
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
                        list.Add(new MWFActivity(GetCtx(), dr, Get_TrxName()));
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            _activities = new MWFActivity[list.Count];
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
                    MWFActivity[] activities = GetActivities(true, true);	//	requery only active
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
            MWFActivity[] activities = GetActivities(true, true);	//	requery active
            String closedState = null;
            bool suspended = false;
            bool running = false;
            for (int i = 0; i < activities.Length; i++)
            {
                MWFActivity activity = activities[i];
                activity.SetAD_Window_ID(GetAD_Window_ID());
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
        private bool StartNext(MWFActivity last, MWFActivity[] activities)
        {
            log.Config("Last=" + last);
            //	transitions from the last processed node
            MWFNodeNext[] transitions = GetWorkflow().GetNodeNexts(last.GetAD_WF_Node_ID(), last.GetAD_Client_ID());
            if (transitions == null || transitions.Length == 0)
            {
                log.Config("none");
                return false;	//	done
            }

            //	We need to wait for last activity
            if (MWFNode.JOINELEMENT_AND.Equals(last.GetNode().GetJoinElement()))
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
                MWFActivity activity = new MWFActivity(this, transitions[i].GetAD_WF_Next_ID());
                // set Last Activity ID property in current WF Activity
                activity.SetLastActivity(last.GetAD_WF_Activity_ID());
                // new Thread(activity).Start();
                //thred = new Thread(new ThreadStart(activity.Run));
                //thred.CurrentCulture = Utility.Env.GetLanguage(Utility.Env.GetContext()).GetCulture(Utility.Env.GetLoginLanguage(Utility.Env.GetContext()).GetAD_Language());
                //thred.CurrentUICulture = Utility.Env.GetLanguage(Utility.Env.GetContext()).GetCulture(Utility.Env.GetLoginLanguage(Utility.Env.GetContext()).GetAD_Language());
                activity.Run();
               // thred.Start();

                //	only the first valid if XOR
                if (MWFNode.SPLITELEMENT_XOR.Equals(split))
                    return true;
            }	//	for all transitions
            return true;
        }

        /// <summary>
        /// Set Workflow Responsible. Searches for a Invoker.
        /// </summary>
        public void SetAD_WF_Responsible_ID()
        {
            int AD_WF_Responsible_ID = DataBase.DB.GetSQLValue(null,
                    MRole.GetDefault(GetCtx(), false).AddAccessSQL(
                    "SELECT AD_WF_Responsible_ID FROM AD_WF_Responsible "
                    + "WHERE ResponsibleType='H' AND COALESCE(AD_User_ID,0)=0 "
                    + "ORDER BY AD_Client_ID DESC",
                    "AD_WF_Responsible", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO));
            SetAD_WF_Responsible_ID(AD_WF_Responsible_ID);
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
		    MWFResponsible resp = MWFResponsible.Get(GetCtx(), GetAD_WF_Responsible_ID());
		    //	(1) User - Directly responsible
		    int AD_User_ID = resp.GetAD_User_ID();
    		
		    //	Invoker - get Sales Rep or last updater of Document
		    if (AD_User_ID == 0 && resp.IsInvoker())
		    {
			    GetPO();
			    //	(2) Doc Owner
			    //if (_po != null && _po instanceof DocAction)
                if (_po != null && (_po.GetType() == typeof(DocAction) || _po.GetType().GetInterface("DocAction") == typeof(DocAction)))
			    {
				    DocAction da = (DocAction)_po;
				    AD_User_ID = da.GetDoc_User_ID();
			    }
			    //	(2) Sales Rep
			    if (AD_User_ID == 0 && _po != null && _po.Get_ColumnIndex("SalesRep_ID") != -1)
			    {
				    Object sr = _po.Get_Value("SalesRep_ID");
				    if (sr != null && sr.GetType() == typeof(int))
					    AD_User_ID = int.Parse(sr.ToString());
			    }
			    //	(3) UpdatedBy
			    if (AD_User_ID == 0 && _po != null)
				    AD_User_ID = _po.GetUpdatedBy();
		    }
    		
		    //	(4) Process Owner
		    if (AD_User_ID == 0 )
			    AD_User_ID = User_ID;
		    //	Fallback 
		    if (AD_User_ID == 0)
			    AD_User_ID = GetCtx().GetAD_User_ID();
		    //
		    SetAD_User_ID(AD_User_ID);
	    }

        /// <summary>
        /// Get Workflow
        /// </summary>
        /// <returns>workflow</returns>
        private MWorkflow GetWorkflow()
        {
            if (_wf == null)
                _wf = MWorkflow.Get(GetCtx(), GetAD_Workflow_ID());
            if (_wf.Get_ID() == 0)
            {
                //throw new IllegalStateException("Not found - AD_Workflow_ID=" + getAD_Workflow_ID());
                throw new Exception("Not found - AD_Workflow_ID=" + GetAD_Workflow_ID());
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
            int AD_WF_Node_ID = GetWorkflow().GetAD_WF_Node_ID();
            log.Fine("AD_WF_Node_ID=" + AD_WF_Node_ID);
            SetWFState(WFSTATE_Running);
            try
            {
                ////	Start first Activity with first Node
                //MWFActivity activity = new MWFActivity(this, AD_WF_Node_ID);
                ////new Thread(activity).Start();
                //thred = new Thread(new ThreadStart(activity.Run));
                ////System.Threading.Thread.CurrentThread.CurrentCulture = Env.GetLanguage(p_ctx).GetCulture(Env.GetBaseAD_Language());
                ////System.Threading.Thread.CurrentThread.CurrentUICulture = Env.GetLanguage(p_ctx).GetCulture(Env.GetBaseAD_Language());

                //thred.CurrentCulture = Utility.Env.GetLanguage(p_ctx).GetCulture(Utility.Env.GetBaseAD_Language());
                //thred.CurrentUICulture = Utility.Env.GetLanguage(p_ctx).GetCulture(Utility.Env.GetBaseAD_Language());
                //thred.Start();

                //Update By --Raghu
                //Date-14-Feb-2012
                //remove thread logic for workflow becouse to show updated workflow record on window
                MWFActivity activity = new MWFActivity(this, AD_WF_Node_ID);
                activity.Run();

            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "AD_WF_Node_ID=" + AD_WF_Node_ID, e);
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

            MTable table = MTable.Get(GetCtx(), GetAD_Table_ID());
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
            StringBuilder sb = new StringBuilder("MWFProcess[")
                .Append(Get_ID())
                .Append(", AD_Workflow_ID=").Append(GetAD_Workflow_ID())
                .Append(", WFState=").Append(GetWFState())
                .Append("]");
            return sb.ToString();
        }
    }
}
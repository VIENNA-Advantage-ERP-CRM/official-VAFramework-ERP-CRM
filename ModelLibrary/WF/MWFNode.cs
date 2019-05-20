
/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MWFNode
 * Purpose        : 
 * Class Used     : MWFNode inherits X_AD_WF_Node
 * Chronological    Development
 * Raghunandan      01-May-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.SqlExec;
using System.Windows.Forms;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.WF
{
    public class MWFNode : X_AD_WF_Node
    {
        #region Private variable
        private const long SERIALVERSIONUID = 1L;
        //Next Modes
        private List<MWFNodeNext> _next = new List<MWFNodeNext>();
        //Translated Name
        private string _name_trl = null;
        //Translated Description	
        private string _description_trl = null;
        //Translated Help
        private string _help_trl = null;
        //Translation Flag
        private bool _translated = false;
        //Column
        private MColumn _column = null;
        //Process Parameters
        private MWFNodePara[] _paras = null;
        //Duration Base MS	
        private long _durationBaseMS = -1;
        private static CCache<int, MWFNode> _cache = new CCache<int, MWFNode>("AD_WF_Node", 50);
        #endregion

        /// <summary>
        ///Standard Constructor - save to cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_WF_Node_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MWFNode(Ctx ctx, int AD_WF_Node_ID, Trx trxName)
            : base(ctx, AD_WF_Node_ID, trxName)
        {
            if (AD_WF_Node_ID == 0)
            {
                //	setAD_WF_Node_ID (0);
                //	setAD_Workflow_ID (0);
                //	setValue (null);
                //	setName (null);
                SetAction(ACTION_WaitSleep);
                SetCost(0);
                SetDuration(0);
                SetEntityType(ENTITYTYPE_UserMaintained);	// U
                SetIsCentrallyMaintained(true);	// Y
                SetJoinElement(JOINELEMENT_XOR);	// X
                SetDurationLimit(0);
                SetSplitElement(SPLITELEMENT_XOR);	// X
                SetWaitingTime(0);
                SetXPosition(0);
                SetYPosition(0);
            }
            //	Save to Cache
            if (Get_ID() != 0)
            {
                _cache.Add(GetAD_WF_Node_ID(), this);
            }
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="wf">workflow (parent)</param>
        /// <param name="Value">value</param>
        /// <param name="Name">name</param>
        public MWFNode(MWorkflow wf, String value, String name)
            : base(wf.GetCtx(), 0, wf.Get_Trx())
        {
            SetClientOrg(wf);
            SetAD_Workflow_ID(wf.GetAD_Workflow_ID());
            SetValue(value);
            SetName(name);
            _durationBaseMS = wf.GetDurationBaseSec() * 1000;
        }

        /// <summary>
        /// Load Constructor - save to cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">set to load info from</param>
        /// <param name="trxName">transaction</param>
        public MWFNode(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            LoadNext();
            LoadTrl();
            //	Save to Cache            
            _cache.Add((int)GetAD_WF_Node_ID(), this);
        }

        /// <summary>
        /// Get WF Node from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_WF_Node_ID">id</param>
        /// <returns>MWFNode</returns>
        public static MWFNode Get(Ctx ctx, int AD_WF_Node_ID)
        {
            int key = AD_WF_Node_ID;
            MWFNode retValue = _cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MWFNode(ctx, AD_WF_Node_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        ///Set Client Org
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">org</param>
        //public  void SetClientOrg(int AD_Client_ID, int AD_Org_ID)
        //{
        //    base.SetClientOrg(AD_Client_ID, AD_Org_ID);
        //}

        /// <summary>
        ///	Load Next
        /// </summary>
        private void LoadNext()
        {
            String sql = "SELECT * FROM AD_WF_NodeNext WHERE AD_WF_Node_ID=" + Get_ID() + " AND IsActive='Y' ORDER BY SeqNo";
            bool splitAnd = SPLITELEMENT_AND.Equals(GetSplitElement());
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_Trx());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    MWFNodeNext next = new MWFNodeNext(GetCtx(), dr, Get_Trx());
                    next.SetFromSplitAnd(splitAnd);
                    _next.Add(next);
                }
                ds = null;
            }
            catch (SqlException e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            log.Fine("#" + _next.Count);
        }

        /// <summary>
        ///Load Translation
        /// </summary>
        private void LoadTrl()
        {
            if (Utility.Env.IsBaseLanguage(GetCtx(),"AD_Workflow") || Get_ID() == 0)
                return;
            String sql = "SELECT Name, Description, Help FROM AD_WF_Node_Trl WHERE AD_WF_Node_ID=" + Get_ID() + " AND AD_Language='" + Utility.Env.GetAD_Language(GetCtx()) + "'";
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_Trx());
                while (idr.Read())
                {
                    _name_trl = idr[0].ToString();
                    _description_trl = idr[1].ToString();
                    _help_trl = idr[2].ToString();
                    _translated = true;
                }
                idr.Close();
            }
            catch (SqlException e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            log.Fine("Trl=" + _translated);
        }

        /// <summary>
        ///Get Number of Next Nodes
        /// </summary>
        /// <returns>number of next nodes</returns>
        public int GetNextNodeCount()
        {
            return _next.Count;
        }

        /// <summary>
        ///Get Name
        /// </summary>
        /// <param name="translated">translated</param>
        /// <returns>Name</returns>
        public String GetName(bool translated)
        {
            if (translated && _translated)
                return _name_trl;
            return GetName();
        }

        /// <summary>
        /// Get Description
        /// </summary>
        /// <param name="translated">translated</param>
        /// <returns>Description</returns>
        public String GetDescription(bool translated)
        {
            if (translated && _translated)
                return _description_trl;
            return GetDescription();
        }

        /// <summary>
        ///Get Help
        /// </summary>
        /// <param name="translated">translated</param>
        /// <returns>Name</returns>
        public String GetHelp(bool translated)
        {
            if (translated && _translated)
                return _help_trl;
            return GetHelp();
        }
         
        /// <summary>
        ///Set Position
        /// </summary>
        /// <param name="position">position point</param>
        public void SetPosition(Point position)
        {
            SetPosition(position.X, position.Y);
        }

        /// <summary>
        /// Set Position
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        public void SetPosition(int x, int y)
        {
            SetXPosition(x);
            SetYPosition(y);
        }

        /// <summary>
        ///Get Position
        /// </summary>
        /// <returns>position point</returns>
        public Point GetPosition()
        {
            return new Point(GetXPosition(), GetYPosition());
        }

        /// <summary>
        ///Get Action Info
        /// </summary>
        /// <returns>info</returns>
        public String GetActionInfo()
        {
            String action = GetAction();
            if (ACTION_AppsProcess.Equals(action))
                return "Process:AD_Process_ID=" + GetAD_Process_ID();
            else if (ACTION_DocumentAction.Equals(action))
                return "DocumentAction=" + GetDocAction();
            else if (ACTION_AppsReport.Equals(action))
                return "Report:AD_Process_ID=" + GetAD_Process_ID();
            else if (ACTION_AppsTask.Equals(action))
                return "Task:AD_Task_ID=" + GetAD_Task_ID();
            else if (ACTION_SetVariable.Equals(action))
                return "SetVariable:AD_Column_ID=" + GetAD_Column_ID();
            else if (ACTION_SubWorkflow.Equals(action))
                return "Workflow:AD_Workflow_ID=" + GetAD_Workflow_ID();
            else if (ACTION_UserChoice.Equals(action))
                return "UserChoice:AD_Column_ID=" + GetAD_Column_ID();
            else if (ACTION_UserWorkbench.Equals(action))
                return "Workbench:?";
            else if (ACTION_UserForm.Equals(action))
                return "Form:AD_Form_ID=" + GetAD_Form_ID();
            else if (ACTION_UserWindow.Equals(action))
                return "Window:AD_Window_ID=" + GetAD_Window_ID();
            else if (ACTION_WaitSleep.Equals(action))
                return "Sleep:WaitTime=" + GetWaitTime();
            return "??";
        }

        /// <summary>
        /// Get Attribute Name
        /// @see model.X_AD_WF_Node#getAttributeName()
        /// </summary>
        /// <returns>Attribute Name</returns>
        public new String GetAttributeName()
        {
            if (GetAD_Column_ID() == 0)
                return base.GetAttributeName();
            //	We have a column
            String attribute = base.GetAttributeName();
            if (attribute != null && attribute.Length > 0)
                return attribute;
            SetAttributeName(GetColumn().GetColumnName());
            return base.GetAttributeName();
        }

        /// <summary>
        ///Get Column
        /// </summary>
        /// <returns>column if valid</returns>
        public MColumn GetColumn()
        {
            if (GetAD_Column_ID() == 0)
                return null;
            if (_column == null)
                _column = MColumn.Get(GetCtx(), GetAD_Column_ID());
            return _column;
        }

        /// <summary>
        ///Is this an Approval setp?
        /// </summary>
        /// <returns>true if User Approval</returns>
        public bool IsUserApproval()
        {
            if (!ACTION_UserChoice.Equals(GetAction()))
                return false;
            return (GetColumn() != null && "IsApproved".Equals(GetColumn().GetColumnName()));
        }

        /// <summary>
        ///Is this a User Choice step?
        /// </summary>
        /// <returns>true if User Choice</returns>
        public bool IsUserChoice()
        {
            return ACTION_UserChoice.Equals(GetAction());
        }

        /// <summary>
        ///Is this a Manual user step?
        /// </summary>
        /// <returns>true if Window/Form/Workbench</returns>
        public bool IsUserManual()
        {
            if (ACTION_UserForm.Equals(GetAction())
                || ACTION_UserWindow.Equals(GetAction())
                || ACTION_UserWorkbench.Equals(GetAction()))
                return true;
            return false;
        }

        /// <summary>
        ///Get Duration in ms
        /// </summary>
        /// <returns>duration in ms</returns>
        public long GetDurationMS()
        {
            long duration = base.GetDuration();
            if (duration == 0)
                return 0;
            if (_durationBaseMS == -1)
                _durationBaseMS = GetWorkflow().GetDurationBaseSec() * 1000;
            return duration * _durationBaseMS;
        }

        /// <summary>
        ///Get Duration Limit in ms
        /// </summary>
        /// <returns>duration limit in ms</returns>
        public long GetDurationLimitMS()
        {
            long limit = base.GetDurationLimit();
            if (limit == 0)
                return 0;
            if (_durationBaseMS == -1)
                _durationBaseMS = GetWorkflow().GetDurationBaseSec() * 1000;
            return limit * _durationBaseMS;
        }

        /// <summary>
        ///Get Duration CalendarField
        /// </summary>
        /// <returns>Calendar.MINUTE, etc.</returns>
        public int GetDurationCalendarField()
        {
            return GetWorkflow().GetDurationCalendarField();
        }

        /// <summary>
        ///Calculate Dynamic Priority
        /// </summary>
        /// <param name="seconds">second after created</param>
        /// <returns>dyn prio</returns>
        public int CalculateDynamicPriority(int seconds)
        {
            if (seconds == 0 || GetDynPriorityUnit() == null
                //|| GetDynPriorityChange() == null
                || GetDynPriorityChange() == 0
                || Utility.Env.ZERO.CompareTo(GetDynPriorityChange()) == 0)
                return 0;
            //
            Decimal divide = Utility.Env.ZERO;
            if (DYNPRIORITYUNIT_Minute.Equals(GetDynPriorityUnit()))
                divide = new Decimal(60);
            else if (DYNPRIORITYUNIT_Hour.Equals(GetDynPriorityUnit()))
                divide = new Decimal(3600);
            else if (DYNPRIORITYUNIT_Day.Equals(GetDynPriorityUnit()))
                divide = new Decimal(86400);
            else
                return 0;
            //
            //Decimal change = new Decimal(seconds).divide(divide, BigDecimal.ROUND_DOWN).multiply(getDynPriorityChange());
            Decimal change = Decimal.Multiply(Decimal.Round(Decimal.Divide(new Decimal(seconds), divide), MidpointRounding.AwayFromZero), GetDynPriorityChange());

            //Decimal change = seconds / Math.Round(divide) * (GetDynPriorityChange());
            return (int)change;
        }

        /// <summary>
        ///Get Workflow
        /// </summary>
        /// <returns>workflow</returns>
        public MWorkflow GetWorkflow()
        {
            return MWorkflow.Get(GetCtx(), GetAD_Workflow_ID());
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MWFNode[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName())
                .Append(",Action=").Append(GetActionInfo())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// User String Representation
        /// </summary>
        /// <returns>info</returns>
        public String ToStringX()
        {
            StringBuilder sb = new StringBuilder("MWFNode[");
            sb.Append(GetName())
                .Append("-").Append(GetActionInfo())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        ///	Before Save
        /// </summary>
        /// <param name="newRecord">newRecord</param>
        /// <returns>true if can be saved</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            String action = GetAction();
            if (action.Equals(ACTION_WaitSleep))
            {
                ;
            }
            else if (action.Equals(ACTION_AppsProcess) || action.Equals(ACTION_AppsReport))
            {
                if (GetAD_Process_ID() == 0)
                {
                    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "AD_Process_ID"));
                    return false;
                }
            }
            else if (action.Equals(ACTION_AppsTask))
            {
                if (GetAD_Task_ID() == 0)
                {
                    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "AD_Task_ID"));
                    return false;
                }
            }
            else if (action.Equals(ACTION_DocumentAction))
            {
                if (GetDocAction() == null || GetDocAction().Length == 0)
                {
                    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "DocAction"));
                    return false;
                }
            }
            else if (action.Equals(ACTION_EMail))
            {
                if (GetR_MailText_ID() == 0)
                {
                    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "R_MailText_ID"));
                    return false;
                }
            }
            else if (action.Equals(ACTION_SetVariable))
            {
                if (GetAttributeValue() == null)
                {
                    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "AttributeValue"));
                    return false;
                }
            }
            else if (action.Equals(ACTION_SubWorkflow))
            {
                if (GetAD_Workflow_ID() == 0)
                {
                    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "AD_Workflow_ID"));
                    return false;
                }
            }
            else if (action.Equals(ACTION_UserChoice))
            {
                if (GetAD_Column_ID() == 0)
                {
                    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "AD_Column_ID"));
                    return false;
                }
            }
            else if (action.Equals(ACTION_UserForm))
            {
                if (GetAD_Form_ID() == 0)
                {
                    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "AD_Form_ID"));
                    return false;
                }
            }
            else if (action.Equals(ACTION_UserWindow))
            {
                if (GetAD_Window_ID() == 0)
                {
                    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "AD_Window_ID"));
                    return false;
                }
            }
            //else if (action.equals(ACTION_UserWorkbench)) 
            //{
            //&& getAD_Workbench_ID() == 0)
            //    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "AD_Workbench_ID"));
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>saved</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;
            TranslationTable.Save(this, newRecord);
            return true;
        }

        /// <summary>
        /// After Delete
        /// </summary>
        /// <param name="success">success</param>
        /// <returns>deleted</returns>
        protected override bool AfterDelete(bool success)
        {
            if (TranslationTable.IsActiveLanguages(false))
                TranslationTable.Delete(this);
            return success;
        }

        /// <summary>
        /// Get the transitions
        /// </summary>
        /// <param name="AD_Client_ID">for client</param>
        /// <returns>array of next nodes</returns>
        public MWFNodeNext[] GetTransitions(int AD_Client_ID)
        {
            List<MWFNodeNext> list = new List<MWFNodeNext>();
            for (int i = 0; i < _next.Count; i++)
            {
                MWFNodeNext next = _next[i];
                if (next.GetAD_Client_ID() == 0 || next.GetAD_Client_ID() == AD_Client_ID)
                    list.Add(next);
            }
            MWFNodeNext[] retValue = new MWFNodeNext[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        ///Get Parameters
        /// </summary>
        /// <returns>array of parameters</returns>
        public MWFNodePara[] GetParameters()
        {
            if (_paras == null)
                _paras = MWFNodePara.GetParameters(GetCtx(), GetAD_WF_Node_ID());
            return _paras;
        }
    }
}
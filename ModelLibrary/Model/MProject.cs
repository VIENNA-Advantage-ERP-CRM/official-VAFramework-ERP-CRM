/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MProject
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     17-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MProject : X_C_Project
    {
        /**	Cached PL			*/
        private int _M_PriceList_ID = 0;

        /**
     * 	Create new Project by copying
     * 	@param ctx context
     *	@param C_Project_ID project
     * 	@param dateDoc date of the document date
     *	@param trxName transaction
     *	@return Project
     */
        public static MProject CopyFrom(Ctx ctx, int C_Project_ID, DateTime? dateDoc, Trx trxName)
        {
            MProject from = new MProject(ctx, C_Project_ID, trxName);
            if (from.GetC_Project_ID() == 0)
                throw new ArgumentException("From Project not found C_Project_ID=" + C_Project_ID);
            //
            MProject to = new MProject(ctx, 0, trxName);
            PO.CopyValues(from, to, from.GetAD_Client_ID(), from.GetAD_Org_ID());
            to.Set_ValueNoCheck("C_Project_ID", I_ZERO);
            //	Set Value with Time
            String Value = to.GetValue() + " ";
            String Time = dateDoc.ToString();
            int length = Value.Length + Time.Length;
            if (length <= 40)
                Value += Time;
            else
                Value += Time.Substring(length - 40 - 1);
            to.SetValue(Value);
            to.SetInvoicedAmt(Env.ZERO);
            to.SetProjectBalanceAmt(Env.ZERO);
            to.SetProcessed(false);
            //
            if (!to.Save())
                throw new Exception("Could not create Project");

            if (to.CopyDetailsFrom(from) == 0)
                throw new Exception("Could not create Project Details");

            return to;
        }


        /*****
         * 	Standard Constructor
         *	@param ctx context
         *	@param C_Project_ID id
         *	@param trxName transaction
         */
        public MProject(Ctx ctx, int C_Project_ID, Trx trxName)
            : base(ctx, C_Project_ID, trxName)
        {

            if (C_Project_ID == 0)
            {
                //	setC_Project_ID(0);
                //	setValue (null);
                //	setC_Currency_ID (0);
                SetCommittedAmt(Env.ZERO);
                SetCommittedQty(Env.ZERO);
                SetInvoicedAmt(Env.ZERO);
                SetInvoicedQty(Env.ZERO);
                SetPlannedAmt(Env.ZERO);
                SetPlannedMarginAmt(Env.ZERO);
                SetPlannedQty(Env.ZERO);
                SetProjectBalanceAmt(Env.ZERO);
                //	setProjectCategory(PROJECTCATEGORY_General);
                SetProjInvoiceRule(PROJINVOICERULE_None);
                SetProjectLineLevel(PROJECTLINELEVEL_Project);
                SetIsCommitCeiling(false);
                SetIsCommitment(false);
                SetIsSummary(false);
                SetProcessed(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MProject(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }



        /**
         * 	Get Project Type as Int (is Button).
         *	@return C_ProjectType_ID id
         */
        public int GetC_ProjectType_ID_Int()
        {
            String pj = base.GetC_ProjectType_ID();
            if (pj == null)
                return 0;
            int C_ProjectType_ID = 0;
            try
            {
                C_ProjectType_ID = int.Parse(pj);
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, pj, ex);
            }
            return C_ProjectType_ID;
        }

        /**
         * 	Set Project Type (overwrite r/o)
         *	@param C_ProjectType_ID id
         */
        public void SetC_ProjectType_ID(int C_ProjectType_ID)
        {
            if (C_ProjectType_ID == 0)
                base.Set_Value("C_ProjectType_ID", null);
            else
                base.Set_Value("C_ProjectType_ID", (int)C_ProjectType_ID);
        }

        /**
         *	String Representation
         * 	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MProject[").Append(Get_ID())
                .Append("-").Append(GetValue()).Append(",ProjectCategory=").Append(GetProjectCategory())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Get Price List from Price List Version
         *	@return price list or 0
         */
        public new int GetM_PriceList_ID()
        {
            if (GetM_PriceList_Version_ID() == 0)
                return 0;
            if (_M_PriceList_ID > 0)
                return _M_PriceList_ID;
            //
            String sql = "SELECT M_PriceList_ID FROM M_PriceList_Version WHERE M_PriceList_Version_ID=" + GetM_PriceList_Version_ID();
            _M_PriceList_ID = DataBase.DB.GetSQLValue(null, sql);
            return _M_PriceList_ID;
        }

        /**
         * 	Set PL Version
         *	@param M_PriceList_Version_ID id
         */
        public new void SetM_PriceList_Version_ID(int M_PriceList_Version_ID)
        {
            base.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
            _M_PriceList_ID = 0;	//	reset
        }


        /**************************************************************************
         * 	Get Project Lines
         *	@return Array of lines
         */
        public MProjectLine[] GetLines()
        {
            List<MProjectLine> list = new List<MProjectLine>();
            String sql = "SELECT * FROM C_ProjectLine WHERE C_Project_ID=" + GetC_Project_ID() + " ORDER BY Line";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MProjectLine(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            MProjectLine[] retValue = new MProjectLine[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Get Project Issues
         *	@return Array of issues
         */
        public MProjectIssue[] GetIssues()
        {
            List<MProjectIssue> list = new List<MProjectIssue>();
            String sql = "SELECT * FROM C_ProjectIssue WHERE C_Project_ID=" + GetC_Project_ID() + " ORDER BY Line";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MProjectIssue(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            MProjectIssue[] retValue = new MProjectIssue[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Get Project Phases
         *	@return Array of phases
         */
        public MProjectPhase[] GetPhases()
        {
            List<MProjectPhase> list = new List<MProjectPhase>();
            String sql = "SELECT * FROM C_ProjectPhase WHERE C_Project_ID=" + GetC_Project_ID() + " ORDER BY SeqNo";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MProjectPhase(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            MProjectPhase[] retValue = new MProjectPhase[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        /**
         * 	Copy Lines/Phase/Task from other Project
         *	@param project project
         *	@return number of total lines copied
         */
        public int CopyDetailsFrom(MProject project)
        {
            if (IsProcessed() || project == null)
                return 0;
            int count = CopyLinesFrom(project)
                + CopyPhasesFrom(project);
            return count;
        }

        /// <summary>
        /// To copy details from one project to another 
        /// </summary>
        /// <param name="fromProject">From Project</param>
        /// <param name="toProject">To Project</param>
        /// <returns>No Of Lines copied</returns>
        public int CopyDetailsFrom(MProject fromProject, MProject toProject)
        {
            if (IsProcessed() || fromProject == null || toProject == null)
                return 0;
            int count = 0; ValueNamePair pp = null; StringBuilder msg = new StringBuilder();

            #region create lines for project
            if (toProject.GetProjectLineLevel().Equals(PROJECTLINELEVEL_Project))
            {
                MProjectLine[] fromLines = fromProject.GetLines();
                MProjectLine line = null;
                for (int i = 0; i < fromLines.Length; i++)
                {
                    line = new MProjectLine(GetCtx(), 0, fromProject.Get_TrxName());
                    PO.CopyValues(fromLines[i], line, GetAD_Client_ID(), GetAD_Org_ID());
                    line.SetC_Project_ID(toProject.GetC_Project_ID());
                    line.SetInvoicedAmt(Env.ZERO);
                    line.SetInvoicedQty(Env.ZERO);
                    line.SetC_OrderPO_ID(0);
                    line.SetC_Order_ID(0);
                    line.SetProcessed(false);
                    if (line.Save())
                        count++;
                    else
                    {
                        pp = VLogger.RetrieveError();
                        if (pp != null)
                        {
                            msg.Append(pp.GetName());
                            //if GetName is Empty then it will check GetValue
                            if (string.IsNullOrEmpty(msg.ToString()))
                                msg.Append(Msg.GetMsg("", pp.GetValue()));
                        }
                        if (string.IsNullOrEmpty(msg.ToString()))
                            msg.Append(Msg.GetMsg(GetCtx(), "VIS_LineNotSaved"));
                        else
                            msg.Append(Msg.GetMsg(GetCtx(), "VIS_LineNotSaved") + "," + msg.ToString());
                    }
                }
            }
            #endregion

            if (toProject.GetProjectLineLevel().Equals(PROJECTLINELEVEL_Phase) || toProject.GetProjectLineLevel().Equals(PROJECTLINELEVEL_Task)
                || toProject.GetProjectLineLevel().Equals(PROJECTLINELEVEL_TaskLine))
            {
                #region if project line level is Project then create lines for phase only
                MProjectPhase[] myPhases = GetPhases();
                MProjectPhase[] fromPhases = fromProject.GetPhases();
                int C_Phase_ID = 0; bool exists = false;
                MProjectPhase toPhase = null;
                MProjectLine[] fromLines = null;
                List<MProjectLine> list = null;
                //	Copy Phases
                for (int i = 0; i < fromPhases.Length; i++)
                {
                    //	Check if Phase already exists
                    C_Phase_ID = fromPhases[i].GetC_Phase_ID();
                    exists = false;
                    if (C_Phase_ID == 0)
                        exists = false;
                    else
                    {
                        for (int ii = 0; ii < myPhases.Length; ii++)
                        {
                            if (myPhases[ii].GetC_Phase_ID() == C_Phase_ID)
                            {
                                exists = true;
                                break;
                            }
                        }
                    }
                    //	Phase exist
                    if (exists)
                    {
                        log.Info("Phase already exists here, ignored - " + fromPhases[i]);
                    }
                    else
                    {
                        toPhase = new MProjectPhase(GetCtx(), 0, Get_TrxName());
                        PO.CopyValues(fromPhases[i], toPhase, GetAD_Client_ID(), GetAD_Org_ID());
                        toPhase.SetC_Project_ID(toProject.GetC_Project_ID());
                        toPhase.SetC_Order_ID(0);
                        toPhase.SetIsComplete(false);
                        if (toPhase.Save())
                        {
                            count++;
                            if (toProject.GetProjectLineLevel().Equals(PROJECTLINELEVEL_Task) || toProject.GetProjectLineLevel().Equals(PROJECTLINELEVEL_TaskLine))
                            {
                                toPhase.CopyTasksFrom(fromPhases[i], toPhase);
                            }
                            else
                            {


                                DataSet projDs = DB.ExecuteDataset(" SELECT C_ProjectLine_ID FROM C_ProjectLine WHERE " +
                                    " C_ProjectPhase_ID=" + fromPhases[i].GetC_ProjectPhase_ID() + " AND " +
                                    " C_Project_ID = " + fromPhases[i].GetC_Project_ID() + " ORDER BY Line ");

                                if (projDs != null && projDs.Tables[0].Rows.Count > 0)
                                {
                                    list = new List<MProjectLine>();
                                    for (int k = 0; k < projDs.Tables[0].Rows.Count; k++)
                                    {
                                        list.Add(new MProjectLine(GetCtx(), Util.GetValueOfInt(projDs.Tables[0].Rows[k]["C_ProjectLine_ID"]), Get_TrxName()));
                                    }
                                    fromLines = new MProjectLine[list.Count];
                                    fromLines = list.ToArray();

                                }

                                if (fromLines != null && fromLines.Length > 0)
                                {
                                    MProjectLine line = null;
                                    for (int j = 0; j < fromLines.Length; j++)
                                    {
                                        line = new MProjectLine(GetCtx(), 0, fromProject.Get_TrxName());
                                        PO.CopyValues(fromLines[j], line, GetAD_Client_ID(), GetAD_Org_ID());
                                        line.SetC_Project_ID(toProject.GetC_Project_ID());
                                        line.SetC_ProjectPhase_ID(toPhase.GetC_ProjectPhase_ID());
                                        line.SetInvoicedAmt(Env.ZERO);
                                        line.SetInvoicedQty(Env.ZERO);
                                        line.SetC_OrderPO_ID(0);
                                        line.SetC_Order_ID(0);
                                        line.SetProcessed(false);
                                        if (line.Save())
                                            count++;
                                        else
                                        {
                                            pp = VLogger.RetrieveError();
                                            if (pp != null)
                                            {
                                                msg.Append(pp.GetName());
                                                //if GetName is Empty then it will check GetValue
                                                if (string.IsNullOrEmpty(msg.ToString()))
                                                    msg.Append(Msg.GetMsg("", pp.GetValue()));
                                            }
                                            if (string.IsNullOrEmpty(msg.ToString()))
                                                msg.Append(Msg.GetMsg(GetCtx(), "VIS_LineNotSaved"));
                                            else
                                                msg.Append(Msg.GetMsg(GetCtx(), "VIS_LineNotSaved") + "," + msg.ToString());
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                msg.Append(pp.GetName());
                                //if GetName is Empty then it will check GetValue
                                if (string.IsNullOrEmpty(msg.ToString()))
                                    msg.Append(Msg.GetMsg("", pp.GetValue()));
                            }
                            if (string.IsNullOrEmpty(msg.ToString()))
                                msg.Append(Msg.GetMsg(GetCtx(), "VIS_LineNotSaved"));
                            else
                                msg.Append(Msg.GetMsg(GetCtx(), "VIS_LineNotSaved") + "," + msg.ToString());
                        }
                    }
                }
                #endregion
            }



            //    count = CopyLinesFrom(fromProject)
            //+ CopyPhasesFrom(fromProject);
            return count;
        }

        /**
         * 	Copy Lines From other Project
         *	@param project project
         *	@return number of lines copied
         */
        public int CopyLinesFrom(MProject project)
        {
            if (IsProcessed() || project == null)
                return 0;
            int count = 0;
            MProjectLine[] fromLines = project.GetLines();
            for (int i = 0; i < fromLines.Length; i++)
            {
                MProjectLine line = new MProjectLine(GetCtx(), 0, project.Get_TrxName());
                PO.CopyValues(fromLines[i], line, GetAD_Client_ID(), GetAD_Org_ID());
                line.SetC_Project_ID(GetC_Project_ID());
                line.SetInvoicedAmt(Env.ZERO);
                line.SetInvoicedQty(Env.ZERO);
                line.SetC_OrderPO_ID(0);
                line.SetC_Order_ID(0);
                line.SetProcessed(false);
                if (line.Save())
                    count++;
            }
            if (fromLines.Length != count)
            {
                log.Log(Level.SEVERE, "Lines difference - Project=" + fromLines.Length + " <> Saved=" + count);
            }

            return count;
        }

        /**
         * 	Copy Phases/Tasks from other Project
         *	@param fromProject project
         *	@return number of items copied
         */
        public int CopyPhasesFrom(MProject fromProject)
        {
            if (IsProcessed() || fromProject == null)
                return 0;
            int count = 0;
            int taskCount = 0;
            //	Get Phases
            MProjectPhase[] myPhases = GetPhases();
            MProjectPhase[] fromPhases = fromProject.GetPhases();
            //	Copy Phases
            for (int i = 0; i < fromPhases.Length; i++)
            {
                //	Check if Phase already exists
                int C_Phase_ID = fromPhases[i].GetC_Phase_ID();
                bool exists = false;
                if (C_Phase_ID == 0)
                    exists = false;
                else
                {
                    for (int ii = 0; ii < myPhases.Length; ii++)
                    {
                        if (myPhases[ii].GetC_Phase_ID() == C_Phase_ID)
                        {
                            exists = true;
                            break;
                        }
                    }
                }
                //	Phase exist
                if (exists)
                {
                    log.Info("Phase already exists here, ignored - " + fromPhases[i]);
                }
                else
                {
                    MProjectPhase toPhase = new MProjectPhase(GetCtx(), 0, Get_TrxName());
                    PO.CopyValues(fromPhases[i], toPhase, GetAD_Client_ID(), GetAD_Org_ID());
                    toPhase.SetC_Project_ID(GetC_Project_ID());
                    toPhase.SetC_Order_ID(0);
                    toPhase.SetIsComplete(false);
                    if (toPhase.Save())
                    {
                        count++;
                        taskCount += toPhase.CopyTasksFrom(fromPhases[i]);
                    }
                }
            }
            if (fromPhases.Length != count)
            {
                log.Warning("Count difference - Project=" + fromPhases.Length + " <> Saved=" + count);
            }

            return count + taskCount;
        }

        /**
         *	Set Project Type and Category.
         * 	If Service Project copy Projet Type Phase/Tasks
         *	@param type project type
         */
        public void SetProjectType(MProjectType type)
        {
            if (type == null)
                return;
            SetC_ProjectType_ID(type.GetC_ProjectType_ID());
            SetProjectCategory(type.GetProjectCategory());
            //vikas  Mantis Issue 0000529 5 dec 2014
            //    if (PROJECTCATEGORY_ServiceChargeProject.Equals(GetProjectCategory()))
            CopyPhasesFrom(type);
        }

        /**
         *	Copy Phases from Type
         *	@param type Project Type
         *	@return count
         */
        public int CopyPhasesFrom(MProjectType type)
        {
            //	create phases
            int count = 0;
            int taskCount = 0;
            MProjectTypePhase[] typePhases = type.GetPhases();
            for (int i = 0; i < typePhases.Length; i++)
            {
                MProjectPhase toPhase = new MProjectPhase(this, typePhases[i]);
                if (toPhase.Save())
                {
                    count++;
                    taskCount += toPhase.CopyTasksFrom(typePhases[i]);
                }
            }
            log.Fine("#" + count + "/" + taskCount
                + " - " + type);
            if (typePhases.Length != count)
            {
                log.Log(Level.SEVERE, "Count difference - Type=" + typePhases.Length + " <> Saved=" + count);
            }
            return count;
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetAD_User_ID() == -1)  //	Summary Project in Dimensions
                SetAD_User_ID(0);

            //	Set Currency
            if (Is_ValueChanged("M_PriceList_Version_ID") && GetM_PriceList_Version_ID() != 0)
            {
                MPriceList pl = MPriceList.Get(GetCtx(), GetM_PriceList_ID(), null);
                if (pl != null && pl.Get_ID() != 0)
                    SetC_Currency_ID(pl.GetC_Currency_ID());
            }
            return true;
        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            PO project = null;
            int _client_ID = 0;
            StringBuilder _sql = new StringBuilder();
            //_sql.Append("Select count(*) from  ad_table where tablename like 'FRPT_Project_Acct'");
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_Project_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_Project_Acct"));
            int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (count > 0)
            {
                _sql.Clear();
                _sql.Append("Select L.Value From Ad_Ref_List L inner join AD_Reference r on R.AD_REFERENCE_ID=L.AD_REFERENCE_ID where r.name='FRPT_RelatedTo' and l.name='Project'");
                var relatedtoProject = Convert.ToString(DB.ExecuteScalar(_sql.ToString()));
                _client_ID = GetAD_Client_ID();
                _sql.Clear();
                _sql.Append("select C_AcctSchema_ID from C_AcctSchema where AD_CLIENT_ID=" + _client_ID);
                DataSet ds3 = new DataSet();
                ds3 = DB.ExecuteDataset(_sql.ToString(), null);
                if (ds3 != null && ds3.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < ds3.Tables[0].Rows.Count; k++)
                    {
                        int _AcctSchema_ID = Util.GetValueOfInt(ds3.Tables[0].Rows[k]["C_AcctSchema_ID"]);
                        _sql.Clear();
                        _sql.Append("Select Frpt_Acctdefault_Id,C_Validcombination_Id,Frpt_Relatedto From Frpt_Acctschema_Default Where ISACTIVE='Y' AND AD_CLIENT_ID=" + _client_ID + "AND C_Acctschema_Id=" + _AcctSchema_ID);
                        DataSet ds = DB.ExecuteDataset(_sql.ToString(), null);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                string _relatedTo = ds.Tables[0].Rows[i]["Frpt_Relatedto"].ToString();
                                if (_relatedTo != "" && (_relatedTo == relatedtoProject))
                                {
                                    _sql.Clear();
                                    _sql.Append("Select COUNT(*) From C_Project Bp Left Join FRPT_Project_Acct ca On Bp.C_Project_ID=ca.C_Project_ID And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"] + " WHERE Bp.IsActive='Y' AND Bp.AD_Client_ID=" + _client_ID + " AND ca.C_Validcombination_Id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Validcombination_Id"]) + " AND Bp.C_Project_ID = " + GetC_Project_ID());
                                    int recordFound = Convert.ToInt32(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
                                    if (recordFound == 0)
                                    {
                                        project = MTable.GetPO(GetCtx(), "FRPT_Project_Acct", 0, null);
                                        project.Set_ValueNoCheck("AD_Org_ID", 0);
                                        project.Set_ValueNoCheck("C_Project_ID", Util.GetValueOfInt(GetC_Project_ID()));
                                        project.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                        project.Set_ValueNoCheck("C_ValidCombination_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Validcombination_Id"]));
                                        project.Set_ValueNoCheck("C_AcctSchema_ID", _AcctSchema_ID);
                                        if (!project.Save())
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (newRecord & success && (String.IsNullOrEmpty(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) || Util.GetValueOfString(GetCtx().GetContext("#DEFAULT_ACCOUNTING_APPLICABLE")) == "Y"))
            {
                bool sucs = Insert_Accounting("C_Project_Acct", "C_AcctSchema_Default", null);
                //Karan. work done to show message if data not saved in accounting tab. but will save data in current tab.
                // Before this, data was being saved but giving message "record not saved".
                if (!sucs)
                {
                    log.SaveWarning("AcctNotSaved", "");
                }
            }

            //	Value/Name change
            MProject prjph = null;
            if (success && !newRecord
                && (Is_ValueChanged("Value") || Is_ValueChanged("Name")))
                MAccount.UpdateValueDescription(GetCtx(), "C_Project_ID=" + GetC_Project_ID(), Get_TrxName());
            if (GetC_Campaign_ID() != 0)
            {
                //Used transaction because total was not updating on header
                MCampaign cam = new MCampaign(GetCtx(), GetC_Campaign_ID(), Get_TrxName());
                decimal plnAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.PlannedAmt),0)  FROM C_Project pl WHERE pl.IsActive = 'Y' AND pl.C_Campaign_ID = " + GetC_Campaign_ID(), null, Get_TrxName()));
                cam.SetCosts(plnAmt);
                cam.Save();
            }
            else
            {
                //Used transaction because total was not updating on header
                prjph = new MProject(GetCtx(), GetC_Project_ID(), Get_TrxName());
                if (!prjph.IsOpportunity())
                {
                    //Used transaction because total was not updating on header
                    decimal plnAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(PlannedAmt),0) FROM C_ProjectPhase WHERE IsActive= 'Y' AND C_Project_ID= " + GetC_Project_ID(), null, Get_TrxName()));
                    DB.ExecuteQuery("UPDATE C_Project SET PlannedAmt=" + plnAmt + " WHERE C_Project_ID=" + GetC_Project_ID(), null, Get_TrxName());
                }
            }
            return success;
        }

        /**
         * 	Before Delete
         *	@return true
         */
        protected override bool BeforeDelete()
        {
            return Delete_Accounting("C_Project_Acct");
        }
    }
}

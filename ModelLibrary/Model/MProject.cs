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
//////using System.Windows.Forms;
using VAdvantage.Model; 
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MProject : X_VAB_Project
    {
        /**	Cached PL			*/
        private int _M_PriceList_ID = 0;

        /**
     * 	Create new Project by copying
     * 	@param ctx context
     *	@param VAB_Project_ID project
     * 	@param dateDoc date of the document date
     *	@param trxName transaction
     *	@return Project
     */
        public static MProject CopyFrom(Ctx ctx, int VAB_Project_ID, DateTime? dateDoc, Trx trxName)
        {
            MProject from = new MProject(ctx, VAB_Project_ID, trxName);
            if (from.GetVAB_Project_ID() == 0)
                throw new ArgumentException("From Project not found VAB_Project_ID=" + VAB_Project_ID);
            //
            MProject to = new MProject(ctx, 0, trxName);
            PO.CopyValues(from, to, from.GetVAF_Client_ID(), from.GetVAF_Org_ID());
            to.Set_ValueNoCheck("VAB_Project_ID", I_ZERO);
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
         *	@param VAB_Project_ID id
         *	@param trxName transaction
         */
        public MProject(Ctx ctx, int VAB_Project_ID, Trx trxName)
            : base(ctx, VAB_Project_ID, trxName)
        {

            if (VAB_Project_ID == 0)
            {
                //	setVAB_Project_ID(0);
                //	setValue (null);
                //	setVAB_Currency_ID (0);
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
         *	@return VAB_ProjectType_ID id
         */
        public int GetVAB_ProjectType_ID_Int()
        {
            String pj = base.GetVAB_ProjectType_ID();
            if (pj == null)
                return 0;
            int VAB_ProjectType_ID = 0;
            try
            {
                VAB_ProjectType_ID = int.Parse(pj);
            }
            catch (Exception ex)
            {
               log.Log(Level.SEVERE, pj, ex);
            }
            return VAB_ProjectType_ID;
        }

        /**
         * 	Set Project Type (overwrite r/o)
         *	@param VAB_ProjectType_ID id
         */
        public void SetVAB_ProjectType_ID(int VAB_ProjectType_ID)
        {
            if (VAB_ProjectType_ID == 0)
                base.Set_Value("VAB_ProjectType_ID", null);
            else
                base.Set_Value("VAB_ProjectType_ID", (int)VAB_ProjectType_ID);
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
            String sql = "SELECT * FROM VAB_ProjectLine WHERE VAB_Project_ID=" + GetVAB_Project_ID() + " ORDER BY Line";
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
            finally {
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
            String sql = "SELECT * FROM VAB_ProjectSupply WHERE VAB_Project_ID=" + GetVAB_Project_ID() + " ORDER BY Line";
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
            finally {
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
            String sql = "SELECT * FROM VAB_ProjectStage WHERE VAB_Project_ID=" + GetVAB_Project_ID() + " ORDER BY SeqNo";
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
                PO.CopyValues(fromLines[i], line, GetVAF_Client_ID(), GetVAF_Org_ID());
                line.SetVAB_Project_ID(GetVAB_Project_ID());
                line.SetInvoicedAmt(Env.ZERO);
                line.SetInvoicedQty(Env.ZERO);
                line.SetVAB_OrderPO_ID(0);
                line.SetVAB_Order_ID(0);
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
                int VAB_Std_Stage_ID = fromPhases[i].GetVAB_Std_Stage_ID();
                bool exists = false;
                if (VAB_Std_Stage_ID == 0)
                    exists = false;
                else
                {
                    for (int ii = 0; ii < myPhases.Length; ii++)
                    {
                        if (myPhases[ii].GetVAB_Std_Stage_ID() == VAB_Std_Stage_ID)
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
                    PO.CopyValues(fromPhases[i], toPhase, GetVAF_Client_ID(), GetVAF_Org_ID());
                    toPhase.SetVAB_Project_ID(GetVAB_Project_ID());
                    toPhase.SetVAB_Order_ID(0);
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
            SetVAB_ProjectType_ID(type.GetVAB_ProjectType_ID());
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
            if (GetVAF_UserContact_ID() == -1)	//	Summary Project in Dimensions
                SetVAF_UserContact_ID(0);

            //	Set Currency
            if (Is_ValueChanged("M_PriceList_Version_ID") && GetM_PriceList_Version_ID() != 0)
            {
                MPriceList pl = MPriceList.Get(GetCtx(), GetM_PriceList_ID(), null);
                if (pl != null && pl.Get_ID() != 0)
                    SetVAB_Currency_ID(pl.GetVAB_Currency_ID());
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
            //_sql.Append("Select count(*) from  vaf_tableview where tablename like 'FRPT_Project_Acct'");
            //_sql.Append("SELECT count(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('FRPT_Project_Acct')  AND OWNER LIKE '" + DB.GetSchema() + "'");
            _sql.Append(DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_Project_Acct"));
            int count = Util.GetValueOfInt(DB.ExecuteScalar(_sql.ToString()));
            if (count > 0)
            {
                _sql.Clear();
                _sql.Append("Select L.Value From VAF_CtrlRef_List L inner join VAF_Control_Ref r on R.VAF_CONTROL_REF_ID=L.VAF_CONTROL_REF_ID where r.name='FRPT_RelatedTo' and l.name='Project'");
                var relatedtoProject = Convert.ToString(DB.ExecuteScalar(_sql.ToString()));
                _client_ID = GetVAF_Client_ID();
                _sql.Clear();
                _sql.Append("select VAB_AccountBook_ID from VAB_AccountBook where VAF_CLIENT_ID=" + _client_ID);
                DataSet ds3 = new DataSet();
                ds3 = DB.ExecuteDataset(_sql.ToString(), null);
                if (ds3 != null && ds3.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < ds3.Tables[0].Rows.Count; k++)
                    {
                        int _AcctSchema_ID = Util.GetValueOfInt(ds3.Tables[0].Rows[k]["VAB_AccountBook_ID"]);
                        _sql.Clear();
                        _sql.Append("Select Frpt_Acctdefault_Id,VAB_Acct_ValidParameter_Id,Frpt_Relatedto From Frpt_Acctschema_Default Where ISACTIVE='Y' AND VAF_CLIENT_ID=" + _client_ID + "AND VAB_AccountBook_Id=" + _AcctSchema_ID);
                        DataSet ds = DB.ExecuteDataset(_sql.ToString(), null);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                string _relatedTo = ds.Tables[0].Rows[i]["Frpt_Relatedto"].ToString();
                                if (_relatedTo != "" && (_relatedTo == relatedtoProject))
                                {
                                    _sql.Clear();
                                    _sql.Append("Select COUNT(*) From VAB_Project Bp Left Join FRPT_Project_Acct ca On Bp.VAB_Project_ID=ca.VAB_Project_ID And ca.Frpt_Acctdefault_Id=" + ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"] + " WHERE Bp.IsActive='Y' AND Bp.VAF_Client_ID=" + _client_ID + " AND ca.VAB_Acct_ValidParameter_Id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]) + " AND Bp.VAB_Project_ID = " + GetVAB_Project_ID());
                                    int recordFound = Convert.ToInt32(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
                                    if (recordFound == 0)
                                    {
                                        project = MTable.GetPO(GetCtx(), "FRPT_Project_Acct", 0, null);
                                        project.Set_ValueNoCheck("VAF_Org_ID", 0);
                                        project.Set_ValueNoCheck("VAB_Project_ID", Util.GetValueOfInt(GetVAB_Project_ID()));
                                        project.Set_ValueNoCheck("FRPT_AcctDefault_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["FRPT_AcctDefault_ID"]));
                                        project.Set_ValueNoCheck("VAB_Acct_ValidParameter_ID", Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Acct_ValidParameter_Id"]));
                                        project.Set_ValueNoCheck("VAB_AccountBook_ID", _AcctSchema_ID);
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
              bool sucs=  Insert_Accounting("VAB_Project_Acct", "VAB_AccountBook_Default", null);
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
                MAccount.UpdateValueDescription(GetCtx(), "VAB_Project_ID=" + GetVAB_Project_ID(), Get_TrxName());
            if (GetVAB_Promotion_ID() != 0)
            {
                MCampaign cam = new MCampaign(GetCtx(), GetVAB_Promotion_ID(), null);
                decimal plnAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.PlannedAmt),0)  FROM VAB_Project pl WHERE pl.IsActive = 'Y' AND pl.VAB_Promotion_ID = " + GetVAB_Promotion_ID()));
                cam.SetCosts(plnAmt);
                cam.Save();
            }
            else
            {                
                prjph = new MProject(GetCtx(), GetVAB_Project_ID(), Get_Trx());
                if (!prjph.IsOpportunity())
                {
                    decimal plnAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(PlannedAmt),0) FROM VAB_ProjectStage WHERE IsActive= 'Y' AND VAB_Project_ID= " + GetVAB_Project_ID()));
                    DB.ExecuteQuery("UPDATE VAB_Project SET PlannedAmt=" + plnAmt + " WHERE VAB_Project_ID=" + GetVAB_Project_ID(), null, Get_Trx());
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
            return Delete_Accounting("VAB_Project_Acct");
        }
    }
}

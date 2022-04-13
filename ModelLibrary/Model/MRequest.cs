using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.IO;
using System.Windows.Forms;

using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using System.Threading;
using VAdvantage.PushNotif;

namespace VAdvantage.Model
{
    public class MRequest : X_R_Request
    {
        /**
	     * 	Get Request ID from mail text
	     *	@param mailText mail text
	     *	@return ID if it contains request tag otherwise 0
	     */
        public static int GetR_Request_ID(String mailText)
        {
            if (mailText == null)
                return 0;
            int indexStart = mailText.IndexOf(TAG_START);
            if (indexStart == -1)
                return 0;
            int indexEnd = mailText.IndexOf(TAG_END, indexStart);
            if (indexEnd == -1)
                return 0;
            //
            indexStart += 5;
            String idString = mailText.Substring(indexStart, indexEnd);
            int R_Request_ID = 0;
            try
            {
                R_Request_ID = int.Parse(idString);
            }
            catch (Exception e)
            {
                _log.Severe("Cannot parse " + idString + " Err" + e.Message);
            }
            return R_Request_ID;
        }

        //	Static Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MRequest).FullName);
        /** Request Tag Start				*/
        private const String TAG_START = "[Req#";
        /** Request Tag End					*/
        private const String TAG_END = "#ID]";

        private StringBuilder message = null;
        private String subject = "";
        private int mailText_ID = 0;

        /**************************************************************************
         * 	Constructor
         * 	@param ctx context
         * 	@param R_Request_ID request or 0 for new
         *	@param trxName transaction
         */
        public MRequest(Ctx ctx, int R_Request_ID, Trx trxName) :
            base(ctx, R_Request_ID, trxName)
        {

            if (R_Request_ID == 0)
            {
                SetDueType(DUETYPE_Due);
                //  SetSalesRep_ID (0);
                //	SetDocumentNo (null);
                SetConfidentialType(CONFIDENTIALTYPE_PublicInformation);	// A
                SetConfidentialTypeEntry(CONFIDENTIALTYPEENTRY_PublicInformation);	// A
                SetProcessed(false);
                SetRequestAmt(Env.ZERO);
                SetPriorityUser(PRIORITY_Low);
                //  SetR_RequestType_ID (0);
                //  SetSummary (null);
                SetIsEscalated(false);
                SetIsSelfService(false);
                SetIsInvoiced(false);
            }
        }

        /**
         * 	SelfService Constructor
         * 	@param ctx context
         * 	@param SalesRep_ID SalesRep
         * 	@param R_RequestType_ID request type
         * 	@param Summary summary
         * 	@param isSelfService self service
         *	@param trxName transaction
         */
        public MRequest(Ctx ctx, int SalesRep_ID,
            int R_RequestType_ID, String Summary, Boolean isSelfService, Trx trxName)
            : this(ctx, 0, trxName)
        {
            Set_Value("SalesRep_ID", (int)SalesRep_ID);	//	could be 0
            Set_Value("R_RequestType_ID", (int)R_RequestType_ID);
            SetSummary(Summary);
            SetIsSelfService(isSelfService);
            GetRequestType();
            if (_requestType != null)
            {
                String ct = _requestType.GetConfidentialType();
                if (ct != null)
                {
                    SetConfidentialType(ct);
                    SetConfidentialTypeEntry(ct);
                }
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result Set
         *	@param trxName transaction
         */
        public MRequest(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
        }

        public MRequest(Ctx ctx, IDataReader dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
        }

        /**
         * 	Import Constructor
         *	@param imp import
         */
        public MRequest(X_I_Request imp)
            : this(imp.GetCtx(), 0, imp.Get_TrxName())
        {

            PO.CopyValues(imp, this, imp.GetAD_Client_ID(), imp.GetAD_Org_ID());
        }

        /** Request Type				*/
        private MRequestType _requestType = null;
        /**	Changed						*/
        private bool _changed = false;
        /**	BPartner					*/
        private MBPartner _partner = null;
        /** User/Contact				*/
        private MUser _user = null;
        /** List of EMail Notices		*/
        private StringBuilder _emailTo = new StringBuilder();

        /** Separator line				*/
        public const String SEPARATOR =
            "\n---------.----------.----------.----------.----------.----------\n";

        private int _success = 0;
        private int _failure = 0;
        private int _notices = 0;


        /**************************************************************************
         * 	Set Default Request Type.
         */
        public void SetR_RequestType_ID()
        {
            _requestType = MRequestType.GetDefault(GetCtx());
            if (_requestType == null)
            {
                log.Warning("No default found");
            }
            else
            {
                base.SetR_RequestType_ID(_requestType.GetR_RequestType_ID());
            }
        }

        /**
         * 	Set Default Request Status.
         */
        public void SetR_Status_ID()
        {
            MStatus status = MStatus.GetDefault(GetCtx(), GetR_RequestType_ID());
            if (status == null)
            {
                log.Warning("No default found");
                if (GetR_Status_ID() != 0)
                    SetR_Status_ID(0);
            }
            else
                SetR_Status_ID(status.GetR_Status_ID());
        }

        /**
         * 	Add To Result
         * 	@param Result
         */
        public void AddToResult(String Result)
        {
            String oldResult = GetResult();
            if (Result == null || Result.Length == 0)
            {
                ;
            }
            else if (oldResult == null || oldResult.Length == 0)
                SetResult(Result);
            else
                SetResult(oldResult + "\n-\n" + Result);
        }

        /**
         * 	Set DueType based on Date Next Action
         */
        public void SetDueType()
        {
            DateTime? due = GetDateNextAction();
            if (due == null)
                return;
            //
            int dueDateTolerance = GetRequestType().GetDueDateTolerance();
            DateTime overdue = TimeUtil.AddDays(due, dueDateTolerance);
            DateTime now = System.DateTime.Now;
            //
            String DueType = DUETYPE_Due;
            if (now < due)
            {
                DueType = DUETYPE_Scheduled;
            }
            else if (now > overdue)
            {
                DueType = DUETYPE_Overdue;
            }
            base.SetDueType(DueType);
        }

        /*
         * 	Get Action History
         *	@return array of actions
         */
        public MRequestAction[] GetActions()
        {
            String sql = "SELECT * FROM R_RequestAction "
                + "WHERE R_Request_ID= " + GetR_Request_ID()
                + " ORDER BY Created DESC";
            List<MRequestAction> list = new List<MRequestAction>();
            DataTable dt;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MRequestAction(GetCtx(), dr, Get_TrxName()));
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            //
            MRequestAction[] retValue = new MRequestAction[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Get Updates
         * 	@param confidentialType maximum confidential type - null = all
         *	@return updates
         */
        public MRequestUpdate[] GetUpdates(String confidentialType)
        {
            String sql = "SELECT * FROM R_RequestUpdate "
                + "WHERE R_Request_ID= " + GetR_Request_ID()
                + " ORDER BY Created DESC";
            List<MRequestUpdate> list = new List<MRequestUpdate>();
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
                    MRequestUpdate ru = new MRequestUpdate(GetCtx(), dr, Get_TrxName());
                    if (confidentialType != null)
                    {
                        //	Private only if private
                        if (ru.GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPEENTRY_PrivateInformation)
                            && !confidentialType.Equals(CONFIDENTIALTYPEENTRY_PrivateInformation))
                            continue;
                        //	Internal not if Customer/Public
                        if (ru.GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPEENTRY_Internal)
                            && (confidentialType.Equals(CONFIDENTIALTYPEENTRY_PartnerConfidential)
                                || confidentialType.Equals(CONFIDENTIALTYPEENTRY_PublicInformation)))
                            continue;
                        //	No Customer if public
                        if (ru.GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPEENTRY_PartnerConfidential)
                            && confidentialType.Equals(CONFIDENTIALTYPEENTRY_PublicInformation))
                            continue;
                    }
                    list.Add(ru);
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            //
            MRequestUpdate[] retValue = new MRequestUpdate[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        //Added By Manjot 09 July 2015
        public MRequestUpdate[] GetUpdatedRecord(String confidentialType)
        {
            //String sql = "SELECT * FROM R_RequestUpdate "
            //    + "WHERE R_Request_ID= " + GetR_Request_ID()
            //    + " ORDER BY Created DESC";

            String sql = @"SELECT * FROM R_RequestUpdate
                    WHERE R_RequestUpdate_ID = (SELECT MAX(R_RequestUpdate_ID) FROM R_RequestUpdate
                    WHERE R_Request_ID= " + GetR_Request_ID()
                    + " ) ORDER BY Created DESC";

            List<MRequestUpdate> list = new List<MRequestUpdate>();
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
                    MRequestUpdate ru = new MRequestUpdate(GetCtx(), dr, Get_TrxName());
                    if (confidentialType != null)
                    {
                        //	Private only if private
                        if (ru.GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPEENTRY_PrivateInformation)
                            && !confidentialType.Equals(CONFIDENTIALTYPEENTRY_PrivateInformation))
                            continue;
                        //	Internal not if Customer/Public
                        if (ru.GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPEENTRY_Internal)
                            && (confidentialType.Equals(CONFIDENTIALTYPEENTRY_PartnerConfidential)
                                || confidentialType.Equals(CONFIDENTIALTYPEENTRY_PublicInformation)))
                            continue;
                        //	No Customer if public
                        if (ru.GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPEENTRY_PartnerConfidential)
                            && confidentialType.Equals(CONFIDENTIALTYPEENTRY_PublicInformation))
                            continue;
                    }
                    list.Add(ru);
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            //
            MRequestUpdate[] retValue = new MRequestUpdate[list.Count];
            retValue = list.ToArray();
            return retValue;
        }
        // Manjot
        /**
         * 	Get Public Updates
         *	@return public updates
         */
        public MRequestUpdate[] GetUpdatesPublic()
        {
            return GetUpdates(CONFIDENTIALTYPE_PublicInformation);
        }

        /**
         * 	Get Customer Updates
         *	@return customer updates
         */
        public MRequestUpdate[] GetUpdatesCustomer()
        {
            return GetUpdates(CONFIDENTIALTYPE_PartnerConfidential);
        }

        /**
         * 	Get Internal Updates
         *	@return internal updates
         */
        public MRequestUpdate[] GetUpdatesInternal()
        {
            return GetUpdates(CONFIDENTIALTYPE_Internal);
        }

        /**
         *	Get Request Type
         *	@return Request Type 	
         */
        public MRequestType GetRequestType()
        {
            if (_requestType == null)
            {
                int R_RequestType_ID = GetR_RequestType_ID();
                if (R_RequestType_ID == 0)
                {
                    SetR_RequestType_ID();
                    R_RequestType_ID = GetR_RequestType_ID();
                }
                _requestType = MRequestType.Get(GetCtx(), R_RequestType_ID);
            }
            return _requestType;
        }

        /**
         *	Get Request Type Text (for jsp)
         *	@return Request Type Text	
         */
        public String GetRequestTypeName()
        {
            if (_requestType == null)
                GetRequestType();
            if (_requestType == null)
                return "??";
            return _requestType.GetName();
        }

        /**
         * 	Get Request Category
         *	@return category
         */
        public MRequestCategory GetCategory()
        {
            if (GetR_Category_ID() == 0)
                return null;
            return MRequestCategory.Get(GetCtx(), GetR_Category_ID());
        }

        /**
         * 	Get Request Category Name
         *	@return name
         */
        public String GetCategoryName()
        {
            MRequestCategory cat = GetCategory();
            if (cat == null)
                return "";
            return cat.GetName();
        }

        /**
         * 	Get Request Group
         *	@return group
         */
        public MGroup GetGroup()
        {
            if (GetR_Group_ID() == 0)
                return null;
            return MGroup.Get(GetCtx(), GetR_Group_ID());
        }

        /**
         * 	Get Request Group Name
         *	@return name
         */
        public String GetGroupName()
        {
            MGroup grp = GetGroup();
            if (grp == null)
                return "";
            return grp.GetName();
        }

        /**
         * 	Get Status
         *	@return status
         */
        public MStatus GetStatus()
        {
            if (GetR_Status_ID() == 0)
                return null;
            return MStatus.Get(GetCtx(), GetR_Status_ID());
        }

        /**
         * 	Get Request Status Name
         *	@return name
         */
        public String GetStatusName()
        {
            MStatus sta = GetStatus();
            if (sta == null)
                return "?";
            return sta.GetName();
        }

        /**
         * 	Get Request Resolution
         *	@return resolution
         */
        public MResolution GetResolution()
        {
            if (GetR_Resolution_ID() == 0)
                return null;
            return MResolution.Get(GetCtx(), GetR_Resolution_ID());
        }

        /**
         * 	Get Request Resolution Name
         *	@return name
         */
        public String GetResolutionName()
        {
            MResolution res = GetResolution();
            if (res == null)
                return "";
            return res.GetName();
        }

        /**
         * 	Is Overdue
         *	@return true if overdue
         */
        public Boolean IsOverdue()
        {
            return DUETYPE_Overdue.Equals(GetDueType());
        }

        /**
         * 	Is due
         *	@return true if due
         */
        public Boolean IsDue()
        {
            return DUETYPE_Due.Equals(GetDueType());
        }

        /**
         * 	Get DueType Text (for jsp)
         *	@return text
         */
        public String GetDueTypeText()
        {
            return MRefList.GetListName(GetCtx(), DUETYPE_AD_Reference_ID, GetDueType());
        }

        /**
         * 	Get Priority Text (for jsp)
         *	@return text
         */
        public String GetPriorityText()
        {
            return MRefList.GetListName(GetCtx(), PRIORITY_AD_Reference_ID, GetPriority());
        }

        /**
         * 	Get Importance Text (for jsp)
         *	@return text
         */
        public String GetPriorityUserText()
        {
            return MRefList.GetListName(GetCtx(), PRIORITYUSER_AD_Reference_ID, GetPriorityUser());
        }

        /**
         * 	Get Confidential Text (for jsp)
         *	@return text
         */
        public String GetConfidentialText()
        {
            return MRefList.GetListName(GetCtx(), CONFIDENTIALTYPE_AD_Reference_ID, GetConfidentialType());
        }

        /**
         * 	Get Confidential Entry Text (for jsp)
         *	@return text
         */
        public String GetConfidentialEntryText()
        {
            return MRefList.GetListName(GetCtx(), CONFIDENTIALTYPEENTRY_AD_Reference_ID, GetConfidentialTypeEntry());
        }

        /**
         * 	Set Date Last Alert to today
         */
        public void SetDateLastAlert()
        {
            //    base.SetDateLastAlert(new DateTime((CommonFunctions.CurrentTimeMillis()) * TimeSpan.TicksPerMillisecond));
            base.SetDateLastAlert(DateTime.Now);
        }

        /**
         * 	Get Sales Rep
         *	@return Sales Rep User
         */
        public MUser GetSalesRep()
        {
            if (GetSalesRep_ID() == 0)
                return null;
            return MUser.Get(GetCtx(), GetSalesRep_ID());
        }

        /**
         * 	Get Sales Rep Name
         *	@return Sales Rep User
         */
        public String GetSalesRepName()
        {
            MUser sr = GetSalesRep();
            if (sr == null)
                return "n/a";
            return sr.GetName();
        }

        /**
         * 	Get Name of creator
         *	@return name
         */
        public String GetCreatedByName()
        {
            MUser user = MUser.Get(GetCtx(), GetCreatedBy());
            return user.GetName();
        }

        /**
         * 	Get Contact (may be not defined)
         *	@return Sales Rep User
         */
        public MUser GetUser()
        {
            if (GetAD_User_ID() == 0)
                return null;
            if (_user != null && _user.GetAD_User_ID() != GetAD_User_ID())
                _user = null;
            if (_user == null)
                _user = new MUser(GetCtx(), GetAD_User_ID(), Get_TrxName());
            return _user;
        }

        /**
         * 	Set Business Partner - Callout
         *	@param oldAD_User_ID old value
         *	@param newAD_User_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetAD_User_ID(String oldAD_User_ID, String newAD_User_ID, int windowNo)
        {
            if (newAD_User_ID == null || newAD_User_ID.Length == 0)
                return;
            int AD_User_ID = int.Parse(newAD_User_ID);
            base.SetAD_User_ID(AD_User_ID);
            if (AD_User_ID == 0)
                return;

            if (GetC_BPartner_ID() == 0)
            {
                MUser user = new MUser(GetCtx(), AD_User_ID, null);
                SetC_BPartner_ID(user.GetC_BPartner_ID());
            }
        }

        /**
         * 	Get BPartner (may be not defined)
         *	@return Sales Rep User
         */
        public MBPartner GetBPartner()
        {
            if (GetC_BPartner_ID() == 0)
                return null;
            if (_partner != null && _partner.GetC_BPartner_ID() != GetC_BPartner_ID())
                _partner = null;
            if (_partner == null)
                _partner = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_TrxName());
            return _partner;
        }

        /**
         * 	Web Can Update Request
         *	@return true if Web can update
         */
        public Boolean IsWebCanUpdate()
        {
            if (IsProcessed())
                return false;
            if (GetR_Status_ID() == 0)
                SetR_Status_ID();
            if (GetR_Status_ID() == 0)
                return false;
            MStatus status = MStatus.Get(GetCtx(), GetR_Status_ID());
            if (status == null)
                return false;
            return status.IsWebCanUpdate();
        }

        /// <summary>
        /// Set Priority
        /// </summary>
        private void SetPriority()
        {
            if (GetPriorityUser() == null)
                SetPriorityUser(PRIORITYUSER_Low);
            //
            if (GetBPartner() != null)
            {
                MBPGroup bpg = MBPGroup.Get(GetCtx(), GetBPartner().GetC_BP_Group_ID());
                String prioBase = bpg.GetPriorityBase();
                if (prioBase != null && !prioBase.Equals(X_C_BP_Group.PRIORITYBASE_Same))
                {
                    char tarGetPrio = Convert.ToChar(GetPriorityUser().Substring(0, 1));
                    if (prioBase.Equals(X_C_BP_Group.PRIORITYBASE_Lower))
                    {
                        tarGetPrio = Convert.ToChar((char.GetNumericValue(tarGetPrio) + 2).ToString());
                    }
                    else
                    {
                        tarGetPrio = Convert.ToChar((char.GetNumericValue(tarGetPrio) - 2).ToString());
                    }
                    if (tarGetPrio < Convert.ToChar(PRIORITY_High.Substring(0, 1)))	//	1
                    {
                        tarGetPrio = Convert.ToChar(PRIORITY_High.Substring(0, 1));
                    }
                    if (tarGetPrio > Convert.ToChar(PRIORITY_Low.Substring(0, 1)))	//	9
                    {
                        tarGetPrio = Convert.ToChar(PRIORITY_Low.Substring(0, 1));
                    }
                    if (GetPriority() == null)
                        SetPriority(tarGetPrio.ToString());
                    else	//	previous priority
                    {
                        if (tarGetPrio < Convert.ToChar(GetPriority().Substring(0, 1)))
                        {
                            SetPriority(tarGetPrio.ToString());
                        }
                    }
                }
            }
            //	Same if nothing else
            if (GetPriority() == null)
                SetPriority(GetPriorityUser());
        }

        /**
         * 	Set Confidential Type Entry
         *	@param ConfidentialTypeEntry confidentiality
         */
        public new void SetConfidentialTypeEntry(String ConfidentialTypeEntry)
        {
            if (ConfidentialTypeEntry == null)
                ConfidentialTypeEntry = GetConfidentialType();
            //
            if (CONFIDENTIALTYPE_Internal.Equals(GetConfidentialType()))
                base.SetConfidentialTypeEntry(CONFIDENTIALTYPE_Internal);
            else if (CONFIDENTIALTYPE_PrivateInformation.Equals(GetConfidentialType()))
            {
                if (CONFIDENTIALTYPE_Internal.Equals(ConfidentialTypeEntry)
                    || CONFIDENTIALTYPE_PrivateInformation.Equals(ConfidentialTypeEntry))
                    base.SetConfidentialTypeEntry(ConfidentialTypeEntry);
                else
                    base.SetConfidentialTypeEntry(CONFIDENTIALTYPE_PrivateInformation);
            }
            else if (CONFIDENTIALTYPE_PartnerConfidential.Equals(GetConfidentialType()))
            {
                if (CONFIDENTIALTYPE_Internal.Equals(ConfidentialTypeEntry)
                    || CONFIDENTIALTYPE_PrivateInformation.Equals(ConfidentialTypeEntry)
                    || CONFIDENTIALTYPE_PartnerConfidential.Equals(ConfidentialTypeEntry))
                    base.SetConfidentialTypeEntry(ConfidentialTypeEntry);
                else
                    base.SetConfidentialTypeEntry(CONFIDENTIALTYPE_PartnerConfidential);
            }
            else if (CONFIDENTIALTYPE_PublicInformation.Equals(GetConfidentialType()))
                base.SetConfidentialTypeEntry(ConfidentialTypeEntry);
        }

        /**
         * 	Web Update
         *	@param result result
         *	@return true if updated
         */
        public Boolean WebUpdate(String result)
        {
            MStatus status = MStatus.Get(GetCtx(), GetR_Status_ID());
            if (!status.IsWebCanUpdate())
                return false;
            if (status.GetUpdate_Status_ID() > 0)
                SetR_Status_ID(status.GetUpdate_Status_ID());
            SetResult(result);
            return true;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRequest[");
            sb.Append(Get_ID()).Append("-").Append(GetDocumentNo()).Append("]");
            return sb.ToString();
        }

        /**
         * 	Create PDF
         *	@return pdf or null
         */
        public FileInfo CreatePDF()
        {
            try
            {
                string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                                    + ".txt"; //.pdf
                string filePath = Path.GetTempPath() + fileName;

                FileInfo temp = new FileInfo(filePath);
                if (!temp.Exists)
                {
                    return CreatePDF(temp);
                }
            }
            catch (Exception e)
            {
                log.Severe("Could not create PDF - " + e.Message);
            }
            return null;
        }

        /**
         * 	Create PDF file
         *	@param file output file
         *	@return file if success
         */
        public FileInfo CreatePDF(FileInfo file)
        {
            //	ReportEngine re = ReportEngine.Get (GetCtx(), ReportEngine.INVOICE, GetC_Invoice_ID());
            //	if (re == null)
            return null;
            //	return re.GetPDF(file);
        }

        /**************************************************************************
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            //	Request Type
            GetRequestType();
            if (newRecord || Is_ValueChanged("R_RequestType_ID"))
            {
                if (_requestType != null)
                {
                    if (IsInvoiced() != _requestType.IsInvoiced())
                        SetIsInvoiced(_requestType.IsInvoiced());
                    if (GetDateNextAction() == null && _requestType.GetAutoDueDateDays() > 0)
                        SetDateNextAction(TimeUtil.AddDays(DateTime.Now,
                            _requestType.GetAutoDueDateDays()));
                }
                //	Is Status Valid
                if (GetR_Status_ID() != 0)
                {
                    MStatus sta = MStatus.Get(GetCtx(), GetR_Status_ID());

                    if (_requestType != null && sta.GetR_StatusCategory_ID() != _requestType.GetR_StatusCategory_ID())
                        SetR_Status_ID();	//	Set to default
                }
            }
            // Start Plan Date And End Plan Date 
            if (GetCloseDate() < GetStartDate())
            {
                log.SaveError("Error", Msg.GetMsg(GetCtx(), "EnddategrtrthnStartdate"));
                return false;
            }
            if (GetDateCompletePlan() < GetDateStartPlan())
            {
                log.SaveError("Error", Msg.GetMsg(GetCtx(), "EndPlandategrtrthnStartdate"));
                return false;
            }
            //	Request Status
            if (GetR_Status_ID() == 0)
                SetR_Status_ID();
            //	Validate/Update Due Type
            SetDueType();
            MStatus status = MStatus.Get(GetCtx(), GetR_Status_ID());
            //	Close/Open
            if (status != null)
            {
                if (status.IsOpen())
                {
                    if (GetStartDate() == null)
                        SetStartDate(DateTime.Now);
                    if (GetCloseDate() != null)
                        SetCloseDate(null);
                }
                if (status.IsClosed()
                    && GetCloseDate() == null)
                    SetCloseDate(DateTime.Now);
                if (status.IsFinalClose())
                    SetProcessed(true);
            }

            //	Confidential Info
            if (GetConfidentialType() == null)
            {
                GetRequestType();
                if (_requestType != null)
                {
                    String ct = _requestType.GetConfidentialType();
                    if (ct != null)
                        SetConfidentialType(ct);
                }
                if (GetConfidentialType() == null)
                    SetConfidentialType(CONFIDENTIALTYPEENTRY_PublicInformation);
            }
            if (GetConfidentialTypeEntry() == null)
                SetConfidentialTypeEntry(GetConfidentialType());
            else
                SetConfidentialTypeEntry(GetConfidentialTypeEntry());

            //	Importance / Priority
            SetPriority();

            //	New
            if (newRecord)
                return true;

            //	Change Log
            _changed = false;
            List<String> sendInfo = new List<String>();
            MRequestAction ra = new MRequestAction(this, false);
            //
            if (CheckChange(ra, "R_RequestType_ID"))
                sendInfo.Add("R_RequestType_ID");
            if (CheckChange(ra, "R_Group_ID"))
                sendInfo.Add("R_Group_ID");
            if (CheckChange(ra, "R_Category_ID"))
                sendInfo.Add("R_Category_ID");
            if (CheckChange(ra, "R_Status_ID"))
                sendInfo.Add("R_Status_ID");
            if (CheckChange(ra, "R_Resolution_ID"))
                sendInfo.Add("R_Resolution_ID");
            //
            if (CheckChange(ra, "SalesRep_ID"))
            {
                //	Sender
                int AD_User_ID = p_ctx.GetAD_User_ID();
                if (AD_User_ID == 0)
                    AD_User_ID = GetUpdatedBy();
                //	Old
                Object oo = Get_ValueOld("SalesRep_ID");
                int oldSalesRep_ID = 0;
                if (oo is int)
                {
                    oldSalesRep_ID = ((int)oo);
                }
                if (oldSalesRep_ID != 0)
                {
                    //  RequestActionTransfer - Request {0} was transfered by {1} from {2} to {3}
                    Object[] args = new Object[] {GetDocumentNo(),
                        MUser.GetNameOfUser(AD_User_ID),
                        MUser.GetNameOfUser(oldSalesRep_ID),
                        MUser.GetNameOfUser(GetSalesRep_ID())
                        };
                    String msg = Msg.GetMsg(GetCtx(), "RequestActionTransfer");
                    AddToResult(msg);
                    sendInfo.Add("SalesRep_ID");
                }
            }
            CheckChange(ra, "AD_Role_ID");
            //
            if (CheckChange(ra, "Priority"))
                sendInfo.Add("Priority");
            if (CheckChange(ra, "PriorityUser"))
                sendInfo.Add("PriorityUser");
            if (CheckChange(ra, "IsEscalated"))
                sendInfo.Add("IsEscalated");
            //
            CheckChange(ra, "ConfidentialType");
            if (CheckChange(ra, "Summary"))
                sendInfo.Add("Summary");
            CheckChange(ra, "IsSelfService");
            CheckChange(ra, "C_BPartner_ID");
            CheckChange(ra, "AD_User_ID");
            CheckChange(ra, "C_Project_ID");
            CheckChange(ra, "A_AsSet_ID");
            CheckChange(ra, "C_Order_ID");
            CheckChange(ra, "C_Invoice_ID");
            CheckChange(ra, "M_Product_ID");
            CheckChange(ra, "C_Payment_ID");
            CheckChange(ra, "M_InOut_ID");
            //	checkChange(ra, "C_Campaign_ID");
            //	checkChange(ra, "RequestAmt");
            CheckChange(ra, "IsInvoiced");
            CheckChange(ra, "C_Activity_ID");
            CheckChange(ra, "DateNextAction");
            CheckChange(ra, "M_ProductSpent_ID");
            CheckChange(ra, "QtySpent");
            CheckChange(ra, "QtyInvoiced");
            CheckChange(ra, "StartDate");
            CheckChange(ra, "CloseDate");
            CheckChange(ra, "TaskStatus");
            CheckChange(ra, "DateStartPlan");
            CheckChange(ra, "DateCompletePlan");
            //new filed result added in list if anyone change/add anything in result email will send to user
            CheckChange(ra, "Result");

            if (_changed)
            {
                if (sendInfo.Count > 0)
                {
                    // get the columns which were changed.
                    string colsChanged = getChangedString(sendInfo);
                    ra.SetChangedValues(colsChanged);
                }
                ra.Save();
            }

            //	Current Info
            MRequestUpdate update = new MRequestUpdate(this);
            if (update.IsNewInfo())
                update.Save();
            else
                update = null;
            //
            // check mail templates from request or request type.
            if (GetR_MailText_ID() > 0)
            {
                mailText_ID = GetR_MailText_ID();
            }
            if (mailText_ID == 0)
            {
                if (_requestType != null && _requestType.GetR_MailText_ID() > 0)
                {
                    mailText_ID = _requestType.GetR_MailText_ID();
                }
            }

            if (mailText_ID == 0)
            {
                _emailTo = new StringBuilder();
                if (_requestType.IsR_AllowSaveNotify() && (update != null || sendInfo.Count > 0))
                {
                    prepareNotificMsg(sendInfo);
                    //	Update
                    if (ra != null)
                        SetDateLastAction(ra.GetCreated());
                    SetLastResult(GetResult());
                    SetDueType();
                    //	ReSet
                    SetConfidentialTypeEntry(GetConfidentialType());
                    SetStartDate(null);
                    SetEndTime(null);
                    SetR_StandardResponse_ID(0);
                    SetR_MailText_ID(0);
                    SetResult(null);
                    //	SetQtySpent(null);
                    //	SetQtyInvoiced(null);
                }
            }
            else
            {
                // get message if mail template is found.
                prepareNotificMsg(sendInfo);
            }

            return true;
        }

        /// <summary>
        /// get string of the changed columns
        /// </summary>
        /// <param name="sendInfo">list of columns changed.</param>
        /// <returns>return the comma separated string.</returns>
        private string getChangedString(List<string> sendInfo)
        {
            StringBuilder colString = null;
            if (sendInfo.Count > 0)
            {
                colString = new StringBuilder();
                for (int i = 0; i < sendInfo.Count; i++)
                {
                    if (i == 0)
                    {
                        colString.Append(sendInfo[i]);
                    }
                    else
                    {
                        colString.Append(',').Append(sendInfo[i]);
                    }

                }
            }
            return colString.ToString();
        }


        /// <summary>
        /// Prepare the notification message before going to after save.
        /// </summary>
        /// <param name="list"></param>
        private void prepareNotificMsg(List<String> list)
        {
            if (mailText_ID == 0)
            {
                message = new StringBuilder();
                //		UpdatedBy: Joe
                int UpdatedBy = GetCtx().GetAD_User_ID();
                MUser from = MUser.Get(GetCtx(), UpdatedBy);
                if (from != null)
                    message.Append(Msg.Translate(GetCtx(), "UpdatedBy")).Append(": ")
                        .Append(from.GetName());
                //		LastAction/Created: ...	
                if (GetDateLastAction() != null)
                    message.Append("\n").Append(Msg.Translate(GetCtx(), "DateLastAction"))
                        .Append(": ").Append(GetDateLastAction());
                else
                    message.Append("\n").Append(Msg.Translate(GetCtx(), "Created"))
                        .Append(": ").Append(GetCreated());

                if (list != null)
                {
                    //	Changes
                    for (int i = 0; i < list.Count; i++)
                    {
                        String columnName = (String)list[i];
                        message.Append("\n").Append(Msg.GetElement(GetCtx(), columnName))
                            .Append(": ").Append(Get_DisplayValue(columnName, false))
                            .Append(" -> ").Append(Get_DisplayValue(columnName, true));
                    }
                }
                //	NextAction
                if (GetDateNextAction() != null)
                    message.Append("\n").Append(Msg.Translate(GetCtx(), "DateNextAction"))
                        .Append(": ").Append(GetDateNextAction());
                message.Append(SEPARATOR)
                    .Append(GetSummary());
                if (GetResult() != null)
                    message.Append("\n----------\n").Append(GetResult());
                message.Append(GetMailTrailer(null));
            }
            else
            {
                message = new StringBuilder();
                subject = string.Empty; // in case of Mail Template is selected subject was coming 2 times that's why we make it empty.
                MRequest _req = new MRequest(GetCtx(), GetR_Request_ID(), null);
                MMailText text = new MMailText(GetCtx(), mailText_ID, null);
                text.SetPO(_req, true); //Set _Po Current value
                subject += GetDocumentNo() + ": " + text.GetMailHeader();

                message.Append(text.GetMailText(true));
                if (GetDateNextAction() != null)
                    message.Append("\n").Append(Msg.Translate(GetCtx(), "DateNextAction"))
                        .Append(": ").Append(GetDateNextAction());

                // message.Append(GetMailTrailer(null));
            }
        }



        /// <summary>
        /// Send notice to role
        /// </summary>
        /// <param name="list">change information</param>
        private List<int> SendRoleNotice()
        {
            List<int> _users = new List<int>();
            string sql = @"SELECT AD_User.ad_user_ID,
                         AD_User_Roles.AD_Role_ID
                        FROM AD_User_Roles
                        INNER JOIN ad_user
                        ON (AD_User_Roles.AD_User_ID    =AD_User.AD_User_ID)
                        WHERE AD_User_Roles.AD_Role_ID IN
                          (SELECT AD_Role_ID
                          FROM R_RequestTypeUpdates
                          WHERE AD_Role_ID   IS NOT NULL
                          AND R_RequestType_ID=" + GetR_RequestType_ID() + @"
                          AND IsActive        ='Y'
                          )
                        AND AD_User_Roles.AD_User_ID NOT IN
                          (SELECT u.AD_User_ID
                          FROM RV_RequestUpdates_Only ru
                          INNER JOIN AD_User u
                          ON (ru.AD_User_ID=u.AD_User_ID)
                          LEFT OUTER JOIN AD_User_Roles r
                          ON (u.AD_User_ID     =r.AD_User_ID)
                          WHERE ru.R_Request_ID=" + GetR_Request_ID() + @"
                          )
                        AND ad_user.email IS NOT NULL";

            DataSet _ds = DB.ExecuteDataset(sql, null, null);
            if (_ds != null && _ds.Tables[0].Rows.Count > 0)
            {
                _users = validateUsers(_ds);
            }
            return _users;

        }


        /// <summary>
        /// Validate the organization access of users according to the role.
        /// </summary>
        /// <param name="_ds"></param>
        /// <returns></returns>
        private List<int> validateUsers(DataSet _ds)
        {
            List<int> users = new List<int>();
            MRole role = new MRole(GetCtx(), Util.GetValueOfInt(_ds.Tables[0].Rows[0]["AD_Role_ID"]), null);
            bool isAllUser = false;
            // if access all organization
            if (role.IsAccessAllOrgs())
            {
                isAllUser = true;
            }
            // if not access user organization access.
            if (!isAllUser && !role.IsUseUserOrgAccess())
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_Org_ID) FROm AD_Role_OrgAccess WHERE IsActive='Y' AND  AD_Role_ID=" + role.GetAD_Role_ID() + " AND AD_Org_ID IN (" + GetAD_Org_ID() + ",0)")) > 0)
                {
                    isAllUser = true;
                }
                else
                {
                    return users;
                }
            }
            for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
            {
                if (isAllUser)
                {
                    users.Add(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["AD_User_ID"]));
                }
                else
                {
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_Org_ID) FROm AD_User_OrgAccess WHERE AD_User_ID=" + Util.GetValueOfInt(_ds.Tables[0].Rows[i]["AD_User_ID"]) + " AND  IsActive='Y' AND  AD_Org_ID IN (" + GetAD_Org_ID() + ",0)")) > 0)
                    {
                        users.Add(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["AD_User_ID"]));
                    }
                }
            }
            return users;
        }

        /// <summary>
        /// Send notifications to users
        /// </summary>
        /// <param name="newRec">if new record or not</param>
        private void SendNotifications(bool newRec)
        {
            if (newRec)
            {
                prepareNotificMsg(new List<String>());
                Thread thread = new Thread(new ThreadStart(() => SendNotices(new List<String>())));
                thread.Start();
            }
            else
            {
                _changed = false;
                List<String> sendInfo = new List<String>();
                MRequestAction ra = new MRequestAction(this, false);
                //
                if (CheckChange(ra, "R_RequestType_ID"))
                    sendInfo.Add("R_RequestType_ID");
                if (CheckChange(ra, "R_Group_ID"))
                    sendInfo.Add("R_Group_ID");
                if (CheckChange(ra, "R_Category_ID"))
                    sendInfo.Add("R_Category_ID");
                if (CheckChange(ra, "R_Status_ID"))
                    sendInfo.Add("R_Status_ID");
                if (CheckChange(ra, "R_Resolution_ID"))
                    sendInfo.Add("R_Resolution_ID");
                //
                if (CheckChange(ra, "SalesRep_ID"))
                {
                    //	Sender
                    int AD_User_ID = p_ctx.GetAD_User_ID();
                    if (AD_User_ID == 0)
                        AD_User_ID = GetUpdatedBy();
                    //	Old
                    Object oo = Get_ValueOld("SalesRep_ID");
                    int oldSalesRep_ID = 0;
                    if (oo is int)
                    {
                        oldSalesRep_ID = ((int)oo);
                    }
                    if (oldSalesRep_ID != 0)
                    {
                        //  RequestActionTransfer - Request {0} was transfered by {1} from {2} to {3}
                        Object[] args = new Object[] {GetDocumentNo(),
                        MUser.GetNameOfUser(AD_User_ID),
                        MUser.GetNameOfUser(oldSalesRep_ID),
                        MUser.GetNameOfUser(GetSalesRep_ID())
                        };
                        String msg = Msg.GetMsg(GetCtx(), "RequestActionTransfer");
                        AddToResult(msg);
                        sendInfo.Add("SalesRep_ID");
                    }
                }
                CheckChange(ra, "AD_Role_ID");
                //
                if (CheckChange(ra, "Priority"))
                    sendInfo.Add("Priority");
                if (CheckChange(ra, "PriorityUser"))
                    sendInfo.Add("PriorityUser");
                if (CheckChange(ra, "IsEscalated"))
                    sendInfo.Add("IsEscalated");
                //
                CheckChange(ra, "ConfidentialType");
                if (CheckChange(ra, "Summary"))
                    sendInfo.Add("Summary");
                CheckChange(ra, "IsSelfService");
                CheckChange(ra, "C_BPartner_ID");
                CheckChange(ra, "AD_User_ID");
                CheckChange(ra, "C_Project_ID");
                CheckChange(ra, "A_AsSet_ID");
                CheckChange(ra, "C_Order_ID");
                CheckChange(ra, "C_Invoice_ID");
                CheckChange(ra, "M_Product_ID");
                CheckChange(ra, "C_Payment_ID");
                CheckChange(ra, "M_InOut_ID");
                //	checkChange(ra, "C_Campaign_ID");
                //	checkChange(ra, "RequestAmt");
                CheckChange(ra, "IsInvoiced");
                CheckChange(ra, "C_Activity_ID");
                CheckChange(ra, "DateNextAction");
                CheckChange(ra, "M_ProductSpent_ID");
                CheckChange(ra, "QtySpent");
                CheckChange(ra, "QtyInvoiced");
                CheckChange(ra, "StartDate");
                CheckChange(ra, "CloseDate");
                CheckChange(ra, "TaskStatus");
                CheckChange(ra, "DateStartPlan");
                CheckChange(ra, "DateCompletePlan");
                //new filed result added in list if anyone change/add anything in result email will send to user
                if (CheckChange(ra, "Result"))
                    sendInfo.Add("Result");
                //
                //
                //if (_changed)
                //    ra.Save();

                //	Current Info
                MRequestUpdate update = new MRequestUpdate(this);
                if (update.IsNewInfo())
                    update.Save();
                else
                    update = null;
                //
                if (mailText_ID == 0)
                {
                    _emailTo = new StringBuilder();
                    if (update != null || sendInfo.Count > 0)
                    {

                        // For Role Changes
                        Thread thread = new Thread(new ThreadStart(() => SendNotices(sendInfo)));
                        thread.Start();
                    }
                }
                else
                {
                    // For Role Changes
                    Thread thread = new Thread(new ThreadStart(() => SendNotices(sendInfo)));
                    thread.Start();
                }
            }
        }



        /**
         * 	Check for changes
         *	@param ra request action
         *	@param columnName column
         *	@return true if changes
         */
        private Boolean CheckChange(MRequestAction ra, String columnName)
        {
            if (Is_ValueChanged(columnName))
            {
                Object value = Get_ValueOld(columnName);
                if (value == null)
                    ra.AddNullColumn(columnName);
                else
                    ra.Set_ValueNoCheck(columnName, value);
                _changed = true;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Get Column Value
        /// </summary>
        /// <param name="columnName"> Column name</param>
        /// <returns>Returns value of the column.</returns>
        public string getColumnValue(string columnName)
        {
            return Get_DisplayValue(columnName, true);
        }
        /**
         *  Check the ability to send email.
         *  @return AD_Message or null if no error
         */
        private String CheckEMail()
        {
            //  Mail Host
            MClient client = MClient.Get(GetCtx());
            if (client == null
                || client.GetSmtpHost() == null
                || client.GetSmtpHost().Length == 0)
                return "RequestActionEMailNoSMTP";

            //  Mail To
            MUser to = new MUser(GetCtx(), GetAD_User_ID(), Get_TrxName());
            if (to == null
                || to.GetEMail() == null
                || to.GetEMail().Length == 0)
                return "RequestActionEMailNoTo";

            //  Mail From real user
            MUser from = MUser.Get(GetCtx(), GetCtx().GetAD_User_ID());
            if (from == null
                || from.GetEMail() == null
                || from.GetEMail().Length == 0)
                return "RequestActionEMailNoFrom";

            //  Check that UI user is Request User
            //int realSalesRep_ID = Env.GetAD_User_ID(GetCtx());
            //if (realSalesRep_ID != GetSalesRep_ID())
            //    SetSalesRep_ID(realSalesRep_ID);

            //  RequestActionEMailInfo - EMail from {0} to {1}
            //Object[] args = new Object[] { emailFrom, emailTo };
            //String msg = Msg.GetMsg(GetCtx(),"RequestActionEMailInfo");
            //SetLastResult(msg);
            return null;
        }

        /**
         * 	Set SalesRep_ID
         *	@param SalesRep_ID id
         */
        public new void SetSalesRep_ID(int SalesRep_ID)
        {
            if (SalesRep_ID != 0)
                base.SetSalesRep_ID(SalesRep_ID);
            else if (GetSalesRep_ID() != 0)
            { }
            log.Warning("Ignored - Tried to Set SalesRep_ID to 0 from " + GetSalesRep_ID());
        }

        /**
         * 	Set MailText - Callout
         *	@param oldR_MailText_ID old value
         *	@param newR_MailText_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetR_MailText_ID(String oldR_MailText_ID, String newR_MailText_ID, int windowNo)
        {
            if (newR_MailText_ID == null || newR_MailText_ID.Length == 0)
                return;
            int R_MailText_ID = int.Parse(newR_MailText_ID);
            base.SetR_MailText_ID(R_MailText_ID);
            if (R_MailText_ID == 0)
                return;

            MMailText mt = new MMailText(GetCtx(), R_MailText_ID, null);
            if (mt.Get_ID() == R_MailText_ID)
            {
                String txt = mt.GetMailText();
                txt = Env.ParseContext(GetCtx(), windowNo, txt, false, true);
                SetResult(txt);
            }
        }

        /**
         * 	Set Standard Response - Callout
         *	@param oldR_StandardResponse_ID old value
         *	@param newR_StandardResponse_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetR_StandardResponse_ID(String oldR_StandardResponse_ID,
                String newR_StandardResponse_ID, int windowNo)
        {
            if (newR_StandardResponse_ID == null || newR_StandardResponse_ID.Length == 0)
                return;
            int R_StandardResponse_ID = int.Parse(newR_StandardResponse_ID);
            base.SetR_StandardResponse_ID(R_StandardResponse_ID);
            if (R_StandardResponse_ID == 0)
                return;

            MStandardResponse sr = new MStandardResponse(GetCtx(), R_StandardResponse_ID, null);
            if (sr.Get_ID() == R_StandardResponse_ID)
            {
                String txt = sr.GetResponseText();
                txt = Env.ParseContext(GetCtx(), windowNo, txt, false, true);
                SetResult(txt);
            }
        }

        /**
         * 	Set Request Type - Callout
         *	@param oldR_RequestType_ID old value
         *	@param newR_RequestType_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetR_RequestType_ID(String oldR_RequestType_ID,
                String newR_RequestType_ID, int windowNo)
        {
            if (newR_RequestType_ID == null || newR_RequestType_ID.Length == 0)
                return;
            int R_RequestType_ID = int.Parse(newR_RequestType_ID);
            base.SetR_RequestType_ID(R_RequestType_ID);
            if (R_RequestType_ID == 0)
                return;

            MRequestType rt = MRequestType.Get(GetCtx(), R_RequestType_ID);
            int R_Status_ID = rt.GetDefaultR_Status_ID();
            SetR_Status_ID(R_Status_ID);
        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (!success)
                return success;

            //	Create Update
            if (newRecord && GetResult() != null)
            {
                MRequestUpdate update = new MRequestUpdate(this);
                update.Save();
            }
            else if (!newRecord && GetResult() != null)
            {
                // get message if someone is updating in Result field of Request Record.
                //We need to send updated msg in email.
                prepareNotificMsg(null);
            }

            //	Initial Mail
            if (_requestType != null && _requestType.IsR_AllowSaveNotify())
            {
                SendNotifications(newRecord);
            }
            //SendNotices(new List<String>());

            //	ChangeRequest - created in Request Processor
            if (GetM_ChangeRequest_ID() != 0
                && Is_ValueChanged("R_Group_ID"))	//	different ECN assignment?
            {
                int oldID = Convert.ToInt32(Get_ValueOld("R_Group_ID"));
                if (GetR_Group_ID() == 0)
                    SetM_ChangeRequest_ID(0);	//	not effective as in afterSave
                else
                {
                    MGroup oldG = MGroup.Get(GetCtx(), oldID);
                    MGroup newG = MGroup.Get(GetCtx(), GetR_Group_ID());
                    if (oldG.GetM_BOM_ID() != newG.GetM_BOM_ID()
                        || oldG.GetM_ChangeNotice_ID() != newG.GetM_ChangeNotice_ID())
                    {
                        MChangeRequest ecr = new MChangeRequest(GetCtx(), GetM_ChangeRequest_ID(), Get_TrxName());
                        if (!ecr.IsProcessed()
                            || ecr.GetM_FixChangeNotice_ID() == 0)
                        {
                            ecr.SetM_BOM_ID(newG.GetM_BOM_ID());
                            ecr.SetM_ChangeNotice_ID(newG.GetM_ChangeNotice_ID());
                            ecr.Save();
                        }
                    }
                }
            }

            //if (_emailTo.Length > 0)
            // {
            //log.SaveInfo("RequestActionEMailOK", _emailTo.ToString());

            //}

            if (_requestType != null && _requestType.IsR_AllowSaveNotify())
            {
                log.SaveInfo("R_EmailSentBackgrnd", "");
            }

            // VIS264 - Send push notification
            #region Push Notification

            if (!IsProcessed() && (newRecord || Is_ValueChanged("R_Status_ID")))
            {
                string IsClosedValue = $"SELECT IsClosed FROM R_Status S JOIN R_Request R ON S.R_Status_ID = R.R_Status_ID WHERE S.IsActive = 'Y' AND R.R_Request_ID = {GetR_Request_ID()}";

                string IsClosed = Util.GetValueOfString(DB.ExecuteScalar(IsClosedValue));

                string msgTitle = Msg.GetMsg(GetCtx(), "VIS_Request") + " (" + GetDocumentNo() + ")";
                string msgBody = Msg.GetMsg(GetCtx(), "Received") + " " + GetStatusName() + " " + Msg.GetMsg(GetCtx(), "Notification");

                if (IsClosed == "N")
                {
                    PushNotification.SendNotificationToUser(GetSalesRep_ID(), GetAD_Window_ID(), GetRecord_ID(), msgTitle, msgBody, "R");
                }
            }

            #endregion

            return success;
        }

        /// <summary>
        /// 	Send transfer Message
        /// </summary>
        private void SendTransferMessage()
        {
            //	Sender
            int AD_User_ID = p_ctx.GetAD_User_ID();
            if (AD_User_ID == 0)
                AD_User_ID = GetUpdatedBy();
            //	Old
            Object oo = Get_ValueOld("SalesRep_ID");
            int oldSalesRep_ID = 0;
            //if (oo.GetType() == typeof(int))
            if (oo is int)
            {
                oldSalesRep_ID = int.Parse(oo.ToString());
            }

            //  RequestActionTransfer - Request {0} was transfered by {1} from {2} to {3}
            Object[] args = new Object[] {GetDocumentNo(),
                    MUser.GetNameOfUser(AD_User_ID),
                    MUser.GetNameOfUser(oldSalesRep_ID),
                    MUser.GetNameOfUser(GetSalesRep_ID())
                    };
            String subject = Msg.GetMsg(GetCtx(), "RequestActionTransfer");
            String message = subject + "\n" + GetSummary();
            MClient client = MClient.Get(GetCtx(), GetAD_Client_ID());
            MUser from = MUser.Get(GetCtx(), AD_User_ID);
            MUser to = MUser.Get(GetCtx(), GetSalesRep_ID());
            //
            client.SendEMail(from, to, subject, message, CreatePDF());
        }

        /**
         * 	Send Update EMail/Notices
         * 	@param list list of changes
         */
        protected void SendNotices(List<String> list)
        {
            bool isEmailSent = false;
            StringBuilder finalMsg = new StringBuilder();
            finalMsg.Append(Msg.Translate(GetCtx(), "R_Request_ID") + ": " + GetDocumentNo()).Append("\n").Append(Msg.Translate(GetCtx(), "R_NotificSent"));
            //	Subject
            if (mailText_ID == 0)
            {
                subject = Msg.Translate(GetCtx(), "R_Request_ID")
                   + " " + Msg.GetMsg(GetCtx(), "Updated", true) + ": " + GetDocumentNo() + " (●" + MTable.Get_Table_ID(Table_Name) + "-" + GetR_Request_ID() + "●) " + Msg.GetMsg(GetCtx(), "DoNotChange");
            }
            //	Message

            //		UpdatedBy: Joe
            int UpdatedBy = GetCtx().GetAD_User_ID();
            MUser from = MUser.Get(GetCtx(), UpdatedBy);

            FileInfo pdf = CreatePDF();
            log.Finer(message.ToString());

            //	Prepare sending Notice/Mail
            MClient client = MClient.Get(GetCtx(), GetAD_Client_ID());
            //	ReSet from if external
            if (from.GetEMailUser() == null || from.GetEMailUserPW() == null)
                from = null;
            _success = 0;
            _failure = 0;
            _notices = 0;

            /** List of users - aviod duplicates	*/
            List<int> userList = new List<int>();
            String sql = "SELECT u.AD_User_ID, u.NotificationType, u.EMail, u.Name, MAX(r.AD_Role_ID) "
                + "FROM RV_RequestUpdates_Only ru"
                + " INNER JOIN AD_User u ON (ru.AD_User_ID=u.AD_User_ID)"
                + " LEFT OUTER JOIN AD_User_Roles r ON (u.AD_User_ID=r.AD_User_ID) "
                + "WHERE ru.R_Request_ID= " + GetR_Request_ID()
                + " GROUP BY u.AD_User_ID, u.NotificationType, u.EMail, u.Name";

            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    int AD_User_ID = Utility.Util.GetValueOfInt(idr[0]);
                    String NotificationType = Util.GetValueOfString(idr[1]); //idr.GetString(1);
                    if (NotificationType == null)
                        NotificationType = X_AD_User.NOTIFICATIONTYPE_EMail;
                    String email = Util.GetValueOfString(idr[2]);// idr.GetString(2);

                    if (String.IsNullOrEmpty(email))
                    {
                        continue;
                    }

                    String Name = Util.GetValueOfString(idr[3]);//idr.GetString(3);
                    //	Role
                    int AD_Role_ID = Utility.Util.GetValueOfInt(idr[4]);
                    if (idr == null)
                    {
                        AD_Role_ID = -1;
                    }

                    //	Don't send mail to oneself
                    //		if (AD_User_ID == UpdatedBy)
                    //			continue;

                    //	No confidential to externals
                    if (AD_Role_ID == -1
                        && (GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPE_Internal)
                            || GetConfidentialTypeEntry().Equals(CONFIDENTIALTYPE_PrivateInformation)))
                        continue;

                    if (X_AD_User.NOTIFICATIONTYPE_None.Equals(NotificationType))
                    {
                        log.Config("Opt out: " + Name);
                        continue;
                    }
                    if ((X_AD_User.NOTIFICATIONTYPE_EMail.Equals(NotificationType)
                        || X_AD_User.NOTIFICATIONTYPE_EMailPlusNotice.Equals(NotificationType))
                        && (email == null || email.Length == 0))
                    {
                        if (AD_Role_ID >= 0)
                            NotificationType = X_AD_User.NOTIFICATIONTYPE_Notice;
                        else
                        {
                            log.Config("No EMail: " + Name);
                            continue;
                        }
                    }
                    if (X_AD_User.NOTIFICATIONTYPE_Notice.Equals(NotificationType)
                        && AD_Role_ID >= 0)
                    {
                        log.Config("No internal User: " + Name);
                        continue;
                    }

                    //	Check duplicate receivers
                    int ii = AD_User_ID;
                    if (userList.Contains(ii))
                        continue;
                    userList.Add(ii);

                    // check the user roles for organization access.                    
                    MUser user = new MUser(GetCtx(), AD_User_ID, null);

                    // VIS0060: Commented after discussion with Mukesh and Mohit, as there will be no role of requested User.
                    //MRole[] role = user.GetRoles(GetAD_Org_ID());
                    //if (role.Length == 0)
                    //    continue;


                    //
                    SendNoticeNow(AD_User_ID, NotificationType,
                        client, from, subject, message.ToString(), pdf);
                    finalMsg.Append("\n").Append(user.GetName()).Append(".");
                    isEmailSent = true;
                }

                idr.Close();
                // Notification For Role
                List<int> _users = SendRoleNotice();
                for (int i = 0; i < _users.Count; i++)
                {
                    MUser user = new MUser(GetCtx(), _users[i], null);
                    int AD_User_ID = user.GetAD_User_ID();
                    String NotificationType = user.GetNotificationType(); //idr.GetString(1);
                    if (NotificationType == null)
                        NotificationType = X_AD_User.NOTIFICATIONTYPE_EMail;
                    String email = user.GetEMail();// idr.GetString(2);

                    if (String.IsNullOrEmpty(email))
                    {
                        continue;
                    }

                    String Name = user.GetName();//idr.GetString(3);
                                                 //	Role                  

                    if (X_AD_User.NOTIFICATIONTYPE_None.Equals(NotificationType))
                    {
                        log.Config("Opt out: " + Name);
                        continue;
                    }

                    //
                    SendNoticeNow(_users[i], NotificationType,
                        client, from, subject, message.ToString(), pdf);
                    finalMsg.Append("\n").Append(user.GetName()).Append(".");
                    isEmailSent = true;
                }

                if (!isEmailSent)
                {
                    finalMsg.Clear();
                    finalMsg.Append(Msg.Translate(GetCtx(), "R_Request_ID") + ": " + GetDocumentNo()).Append("\n").Append(Msg.Translate(GetCtx(), "R_NoNotificationSent"));
                }

                int AD_Message_ID = 834;
                MNote note = new MNote(GetCtx(), AD_Message_ID, GetCtx().GetAD_User_ID(),
                    X_R_Request.Table_ID, GetR_Request_ID(),
                    subject, finalMsg.ToString(), Get_TrxName());
                if (note.Save())
                    log.Log(Level.INFO, "ProcessFinished", "");
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }


            //	New Sales Rep (may happen if sent from beforeSave
            if (!userList.Contains(GetSalesRep_ID()))
                SendNoticeNow(GetSalesRep_ID(), null,
                    client, from, subject, message.ToString(), pdf);

            log.Info("EMail Success=" + _success + ", Failure=" + _failure
                + " - Notices=" + _notices);
        }

        /**
         * 	Send Notice Now
         *	@param AD_User_ID	recipient
         *	@param NotificationType	optional notification type
         *	@param client client
         *	@param from sender
         *	@param subject subject
         *	@param message message
         *	@param pdf optional attachment
         */
        private void SendNoticeNow(int AD_User_ID, String NotificationType,
            MClient client, MUser from, String subject, String message, FileInfo pdf)
        {
            MUser to = MUser.Get(GetCtx(), AD_User_ID);
            if (NotificationType == null)
                NotificationType = to.GetNotificationType();
            //	Send Mail
            if (X_AD_User.NOTIFICATIONTYPE_EMail.Equals(NotificationType)
                || X_AD_User.NOTIFICATIONTYPE_EMailPlusNotice.Equals(NotificationType))
            {
                VAdvantage.Model.MMailAttachment1 _mAttachment = new VAdvantage.Model.MMailAttachment1(GetCtx(), 0, null);
                _mAttachment.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                _mAttachment.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
                _mAttachment.SetAD_Table_ID(MTable.Get_Table_ID(Table_Name));
                _mAttachment.IsActive();
                _mAttachment.SetMailAddress("");
                _mAttachment.SetAttachmentType("M");
                _mAttachment.SetRecord_ID(GetR_Request_ID());
                _mAttachment.SetTextMsg(message);
                _mAttachment.SetTitle(subject);
                _mAttachment.SetMailAddress(to.GetEMail());

                if (from != null && !string.IsNullOrEmpty(from.GetEMail()))
                {
                    _mAttachment.SetMailAddressFrom(from.GetEMail());
                }
                else
                {
                    _mAttachment.SetMailAddressFrom(client.GetRequestEMail());
                }

                _mAttachment.NewRecord();

                if (client.SendEMail(from, to, subject, message.ToString(), pdf))
                {
                    _success++;
                    if (_emailTo.Length > 0)
                        _emailTo.Append(", ");
                    _emailTo.Append(to.GetEMail());
                    _mAttachment.SetIsMailSent(true);
                }
                else
                {
                    log.Warning("Failed: " + to);
                    _failure++;
                    NotificationType = X_AD_User.NOTIFICATIONTYPE_Notice;
                    _mAttachment.SetIsMailSent(false);
                }

                _mAttachment.Save();
            }

            //	Send Note
            if (X_AD_User.NOTIFICATIONTYPE_Notice.Equals(NotificationType)
                || X_AD_User.NOTIFICATIONTYPE_EMailPlusNotice.Equals(NotificationType))
            {
                int AD_Message_ID = 834;
                MNote note = new MNote(GetCtx(), AD_Message_ID, AD_User_ID,
                    X_R_Request.Table_ID, GetR_Request_ID(),
                    subject, message.ToString(), Get_TrxName());
                if (note.Save())
                    _notices++;
            }

        }

        /*****
         * 	Get MailID
         * 	@param serverAddress server address
         *	@return Mail Trailer
         */
        public String GetMailTrailer(String serverAddress)
        {
            StringBuilder sb = new StringBuilder("\n").Append(SEPARATOR)
                .Append(Msg.Translate(GetCtx(), "R_Request_ID"))
                .Append(": ").Append(GetDocumentNo())
                .Append("  ").Append(GetMailTag())
                .Append("\nSent by ViennaMail");
            if (serverAddress != null)
                sb.Append(" from ").Append(serverAddress);
            return sb.ToString();
        }


        /**
         * 	Get Mail Tag
         *	@return [Req@{id}@]
         */
        public String GetMailTag()
        {
            return TAG_START + Get_ID() + TAG_END;
        }

        /**
         * 	(Soft) Close request.
         * 	Must be called after webUpdate
         */
        public void DoClose()
        {
            MStatus status = MStatus.Get(GetCtx(), GetR_Status_ID());
            if (!status.IsClosed())
            {
                MStatus[] closed = MStatus.GetClosed(GetCtx());
                MStatus newStatus = null;
                for (int i = 0; i < closed.Length; i++)
                {
                    if (!closed[i].IsFinalClose())
                    {
                        newStatus = closed[i];
                        break;
                    }
                }
                if (newStatus == null && closed.Length > 0)
                    newStatus = closed[0];
                if (newStatus != null)
                    SetR_Status_ID(newStatus.GetR_Status_ID());
            }
        }

        /**
         * 	Escalate request
         * 	@param user true if user escalated - otherwise system
         */
        public void DoEscalate(Boolean user)
        {
            if (user)
            {
                String Importance = GetPriorityUser();
                if (PRIORITYUSER_Urgent.Equals(Importance))
                {; }	//	high as it goes
                else if (PRIORITYUSER_High.Equals(Importance))
                    SetPriorityUser(PRIORITYUSER_Urgent);
                else if (PRIORITYUSER_Medium.Equals(Importance))
                    SetPriorityUser(PRIORITYUSER_High);
                else if (PRIORITYUSER_Low.Equals(Importance))
                    SetPriorityUser(PRIORITYUSER_Medium);
                else if (PRIORITYUSER_Minor.Equals(Importance))
                    SetPriorityUser(PRIORITYUSER_Low);
            }
            else
            {
                String Importance = GetPriority();
                if (PRIORITY_Urgent.Equals(Importance))
                {; }	//	high as it goes
                else if (PRIORITY_High.Equals(Importance))
                    SetPriority(PRIORITY_Urgent);
                else if (PRIORITY_Medium.Equals(Importance))
                    SetPriority(PRIORITY_High);
                else if (PRIORITY_Low.Equals(Importance))
                    SetPriority(PRIORITY_Medium);
                else if (PRIORITY_Minor.Equals(Importance))
                    SetPriority(PRIORITY_Low);
            }
        }
    }
}

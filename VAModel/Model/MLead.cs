/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MLead
 * Purpose        : for workflow
 * Chronological    Development
 * Raghunandan     05-Jun-2009
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
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;


namespace VAdvantage.Model
{
    public class MLead : X_C_Lead
    {
        #region private variables
        /** BPartner			*/
        private MBPartner _bp = null;
        /** User				*/
        private MUser _user = null;
        /** Request				*/
        private MRequest _request = null;
        /** Project				*/
        private MProject _project = null;
        /** Request Status		*/
        private MStatus _Status = null;
        #endregion

        /**
        * 	Standard Constructor
        *	@param ctx context
        *	@param C_Lead_ID id
        *	@param trxName trx
        */
        public MLead(Ctx ctx, int C_Lead_ID, Trx trxName) :
            base(ctx, C_Lead_ID, trxName)
        {
            if (C_Lead_ID == 0)
            {
                SetProcessed(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result Set
         *	@param trxName trx
         */
        public MLead(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

        /**
         * 	Lead map constructor
         *	@param ctx context
         *	@param map map
         *	@param trxName trx
         */
        public MLead(Ctx ctx, Dictionary<String, String> map, Trx trxName)
            : this(ctx, 0, trxName)
        {

            Load(map);
            //	Overwrite
            //	Set_ValueNoCheck ("C_Lead_ID", null);
            SetIsActive(true);
            SetProcessed(false);
        }

        /**
         * 	Set AD_User_ID from email
         */
        public void SetAD_User_ID()
        {
            if (GetAD_User_ID() != 0)
                return;
            String email = GetEMail();
            if (email != null && email.Length > 0)
            {
                _user = MUser.Get(GetCtx(), email, Get_TrxName());
                if (_user != null)
                {
                    base.SetAD_User_ID(_user.GetAD_User_ID());
                    if (GetC_BPartner_ID() == 0)
                        SetC_BPartner_ID(_user.GetC_BPartner_ID());
                    else if (_user.GetC_BPartner_ID() != GetC_BPartner_ID())
                    {
                        log.Warning("@C_BPartner_ID@ (ID=" + GetC_BPartner_ID()
                            + ") <> @AD_User_ID@ @C_BPartner_ID@ (ID=" + _user.GetC_BPartner_ID() + ")");
                    }
                }
            }
        }

        /**
         * 	Set AD_User_ID
         *	@param AD_User_ID user
         */
        public new void SetAD_User_ID(int AD_User_ID)
        {
            base.SetAD_User_ID(AD_User_ID);
            GetUser();
        }

        /**
         * 	Get User
         *	@return user
         */
        public MUser GetUser()
        {
            if (GetAD_User_ID() == 0)
                _user = null;
            else if (_user == null
                || _user.GetAD_User_ID() != GetAD_User_ID())
                _user = new MUser(GetCtx(), GetAD_User_ID(), Get_TrxName());
            return _user;
        }

        /**
         * 	Set C_BPartner_ID
         *	@param C_BPartner_ID bp
         */
        public new void SetC_BPartner_ID(int C_BPartner_ID)
        {
            base.SetC_BPartner_ID(C_BPartner_ID);
            GetBPartner();
        }

        /**
         * 	Get BPartner
         *	@return bp or null
         */
        public MBPartner GetBPartner()
        {
            if (GetC_BPartner_ID() == 0)
                _bp = null;
            else if (_bp == null
                || _bp.GetC_BPartner_ID() != GetC_BPartner_ID())
                _bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_TrxName());
            return _bp;
        }

        /**
         * 	Set R_Request_ID
         *	@param R_Request_ID
         */
        public new void SetR_Request_ID(int R_Request_ID)
        {
            base.SetR_Request_ID(R_Request_ID);
            GetRequest();
        }

        /**
         * 	Get Request
         *	@return request
         */
        public MRequest GetRequest()
        {
            if (GetR_Request_ID() == 0)
                _request = null;
            else if (_request == null
                || _request.GetR_Request_ID() != GetR_Request_ID())
                _request = new MRequest(GetCtx(), GetR_Request_ID(), Get_TrxName());
            return _request;
        }

        /**
         * 	Set R_Status_ID
         *	@see Model.X_C_Lead#SetR_Status_ID(int)
         *	@param R_Status_ID
         */
        public new void SetR_Status_ID(int R_Status_ID)
        {
            if (IsR_Status_IDValid(R_Status_ID))
                base.SetR_Status_ID(R_Status_ID);
            else
                base.SetR_Status_ID(0);
            GetStatus();
        }

        /**
         * 	Is R_Status_ID Valid
         *	@param R_Status_ID id
         *	@return true if valid
         */
        public bool IsR_Status_IDValid(int R_Status_ID)
        {
            if (R_Status_ID == 0)
                return true;

            _Status = MStatus.Get(GetCtx(), R_Status_ID);
            int R_StatusCategory_ID = _Status.GetR_StatusCategory_ID();
            //
            int R_RequestType_ID = GetR_RequestType_ID();
            if (R_RequestType_ID == 0)
            {
                log.Warning("No Client Request Type");
                return false;
            }
            MRequestType rt = MRequestType.Get(GetCtx(), R_RequestType_ID);
            if (rt.GetR_StatusCategory_ID() != R_StatusCategory_ID)
            {
                log.Warning("Status Category different - Status("
                    + R_StatusCategory_ID + ") <> RequestType("
                    + rt.GetR_StatusCategory_ID() + ")");
                return false;
            }
            return true;
        }

        /**
         * 	Get R_RequestType_ID
         *	@return Request Type
         */
        private int GetR_RequestType_ID()
        {
            MClientInfo ci = MClientInfo.Get(GetCtx(), GetAD_Client_ID());
            int R_RequestType_ID = ci.GetR_RequestType_ID();
            if (R_RequestType_ID != 0)
                return R_RequestType_ID;
            log.Warning("Set Request Type in Window Client Info");

            //	Default
            MRequestType rt = MRequestType.GetDefault(GetCtx());
            if (rt != null)
            {
                R_RequestType_ID = rt.GetR_RequestType_ID();
                ci.SetR_RequestType_ID(R_RequestType_ID);
                ci.Save();
                return R_RequestType_ID;
            }
            //
            return 0;
        }

        /**
         * 	Get Status
         *	@return status or null
         */
        public new MStatus GetStatus()
        {
            if (GetR_Status_ID() == 0)
                _Status = null;
            else if (_Status == null
                || _Status.GetR_Status_ID() != GetR_Status_ID())
                _Status = MStatus.Get(GetCtx(), GetR_Status_ID());
            return _Status;
        }

        /**
         * 	Set C_Project_ID
         *	@param C_Project_ID project
         */
        public new void SetC_Project_ID(int C_Project_ID)
        {
            base.SetC_Project_ID(C_Project_ID);
            GetProject();
        }

        /**
         * 	Get Project
         *	@return project or null
         */
        public MProject GetProject()
        {
            if (GetC_Project_ID() == 0)
                _project = null;
            else if (_project == null
                || _project.GetC_Project_ID() != GetC_Project_ID())
                _project = new MProject(GetCtx(), GetC_Project_ID(), Get_TrxName());
            return _project;
        }

        /**
         * 	Get Name
         *	@return not null value
         */
        public new String GetName()
        {
            String name = base.GetName();			//	Subject
            if (name == null)
            {
                name = GetBPName();					//	BPartner
                if (name == null)
                {
                    name = GetContactName();		//	Contact
                    if (name == null)
                    {
                        name = GetDocumentNo();		//	DocumentNo
                        if (name == null)
                            name = "Lead";
                    }
                }
            }
            return name;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MLead[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }

        /**
         * 	Create BP, Contact, Location from Lead
         *	@return error message
         */
        public String CreateBP()
        {
            if (GetC_BPartner_ID() != 0 && GetAD_User_ID() != 0 && GetC_BPartner_Location_ID() == 0)
                return "@AlreadyExists@: @C_BPartner_ID@ (ID=" + GetC_BPartner_ID() + ")";

            //	BPartner
            if (GetC_BPartner_ID() == 0
                && (GetBPName() != null && GetBPName().Length > 0))
            {
                //	Existing User
                _user = GetUser();
                if (_user != null)
                {
                    if (_user.GetC_BPartner_ID() != 0)
                    {
                        SetRef_BPartner_ID(_user.GetC_BPartner_ID());
                        log.Info("Set to BPartner of User - " + _user);
                        return CreateBPLocation();
                    }
                }
                //
                _bp = new MBPartner(GetCtx(), Get_TrxName());	//	Template
                _bp.SetAD_Org_ID(GetAD_Org_ID());
                //_bp.SetValue(GetBPName());
                _bp.SetName(GetBPName());
                _bp.SetIsCustomer(false);
                _bp.SetIsProspect(true);
                _bp.SetSOCreditStatus("O");
                // Check Removed as per Surya Sir's Decision BY Lokesh

                //_bp.SetC_Campaign_ID(GetC_Campaign_ID());
                //
                if (GetC_BP_Group_ID() == 0)
                {
                    // ShowMessage.Info("First Select Customer Group Then process again", true, null, null);
                    return null;

                }
                else
                {
                    _bp.SetC_BP_Group_ID(GetC_BP_Group_ID());
                }
                MBPGroup gp = new MBPGroup(GetCtx(), GetC_BP_Group_ID(), Get_TrxName());
                _bp.SetM_PriceList_ID(gp.GetM_PriceList_ID());
                if (GetC_BP_Size_ID() != 0)
                    _bp.SetC_BP_Size_ID(GetC_BP_Size_ID());
                if (GetURL() != null)
                    _bp.SetURL(GetURL());
                if (GetC_BP_Status_ID() != 0)
                    _bp.SetC_BP_Status_ID(GetC_BP_Status_ID());
                if (GetC_IndustryCode_ID() != 0)
                    _bp.SetC_IndustryCode_ID(GetC_IndustryCode_ID());
                if (GetNAICS() != null)
                    _bp.SetNAICS(GetNAICS());
                if (GetDUNS() != null)
                    _bp.SetDUNS(GetDUNS());
                if (GetNumberEmployees() != 0)
                    _bp.SetNumberEmployees(GetNumberEmployees());
                if (GetSalesVolume() != 0)
                    _bp.SetSalesVolume(GetSalesVolume());
                if (GetSalesRep_ID() != 0)
                    _bp.SetSalesRep_ID(GetSalesRep_ID());
                if (GetC_Campaign_ID() != 0)
                    _bp.SetC_Campaign_ID(GetC_Campaign_ID());
                if (!_bp.Save())
                {
                    return "@SaveError@";
                }
                //	Update User
                if (_user != null && _user.GetC_BPartner_ID() == 0)
                {
                    _user.SetC_BPartner_ID(_bp.GetC_BPartner_ID());
                    _user.Save();
                }
                //	Save BP
                SetRef_BPartner_ID(_bp.GetC_BPartner_ID());
            }

            String error = CreateBPContact();
            if (error != null && error.Length > 0)
                return error;
            CreateBPLocation();

            try
            {
                int id = _bp.GetC_BPartner_ID();
                string qry = "Update c_bpartner set Description='' where c_bpartner_id=" + id;
                int check = DB.ExecuteQuery(qry, null, Get_TrxName());
                string val = _bp.GetValue();
                qry = "Update c_bpartner set Value='" + val + GetBPName() + "' where c_bpartner_id=" + id;
                check = DB.ExecuteQuery(qry, null, Get_TrxName());

                if (GetR_InterestArea_ID() != 0)
                {
                    string sql = "Select R_InterestArea_ID from vss_lead_interestarea where C_Lead_ID=" + GetC_Lead_ID();
                    IDataReader dr = DB.ExecuteReader(sql, null, Get_TrxName());
                    while (dr.Read())
                    {
                        X_R_ContactInterest Prospect = new X_R_ContactInterest(GetCtx(), 0, Get_TrxName());
                        Prospect.SetR_InterestArea_ID(Util.GetValueOfInt(dr[0]));
                        Prospect.SetC_BPartner_ID(GetRef_BPartner_ID());
                        String query = "Select ad_user_id from ad_user where c_bpartner_id= " + GetRef_BPartner_ID();
                        int UserId = Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_TrxName()));
                        Prospect.SetAD_User_ID(UserId);
                        query = "Select C_BPartner_Location_id from C_BPartner_Location where c_bpartner_id= " + GetRef_BPartner_ID();

                        int Id = Util.GetValueOfInt(DB.ExecuteScalar(query, null, Get_TrxName()));
                        X_C_BPartner_Location loc = new X_C_BPartner_Location(GetCtx(), Id, Get_TrxName());
                        Prospect.SetC_BPartner_Location_ID(Id);
                        Prospect.SetPhone(loc.GetPhone());
                        Prospect.SetFax(loc.GetFax());

                        X_AD_User us = new X_AD_User(GetCtx(), UserId, Get_TrxName());
                        Prospect.SetC_Job_ID(us.GetC_Job_ID());
                        Prospect.SetSubscribeDate(DateTime.Today);
                        query = "Select Email from ad_user where ad_user_id= " + UserId;
                        String mail = Util.GetValueOfString(DB.ExecuteScalar(query, null, Get_TrxName()));
                        Prospect.SetEMail(mail);
                        if (Prospect.Save())
                        {

                        }
                    } dr.Close();
                }
            }
            catch
            {

            }

            return null;
        }

        /**
         * 	Create BP Contact from Lead
         *	@return error
         */
        private String CreateBPContact()
        {
            //	Contact exists
            if (GetAD_User_ID() != 0)
                return null;

            //	Something to save
            if ((GetContactName() != null && GetContactName().Length > 0))
            {
                ;
            }
            else
            {
                log.Fine("No BP Contact Info to save");
                return null;
            }

            if (_user == null)
            {
                if (_bp == null)
                    _user = new MUser(GetCtx(), 0, Get_TrxName());
                else
                    _user = new MUser(_bp);
            }
            _user.SetName(GetContactName());
            //
            if (GetC_Job_ID() != 0)
                _user.SetC_Job_ID(GetC_Job_ID());
            if (GetEMail() != null)
                _user.SetEMail(GetEMail());
            if (GetC_Greeting_ID() != 0)
                _user.SetC_Greeting_ID(GetC_Greeting_ID());
            if (GetPhone() != null)
                _user.SetPhone(GetPhone());
            if (GetPhone2() != null)
                _user.SetPhone2(GetPhone2());
            if (GetFax() != null)
                _user.SetFax(GetFax());
            if (GetTitle() != null)
                _user.SetTitle(GetTitle());
            //
            if (!_user.Save())
            {
                log.Warning("Contact not saved");
            }
            else
                SetAD_User_ID(_user.GetAD_User_ID());
            return null;
        }

        /**
         * 	Create BP Location from Lead
         *	@return error message
         */
        private String CreateBPLocation()
        {
            if (GetC_BPartner_Location_ID() != 0
                || GetC_Country_ID() == 0)	//	mandatory
                return null;

            //	Something to save
            if ((GetAddress1() != null && GetAddress1().Length > 0)
                || (GetPostal() != null && GetPostal().Length > 0)
                || (GetCity() != null && GetCity().Length > 0)
                || (GetRegionName() != null && GetRegionName().Length > 0)
            )
            {
                ;
            }
            else
            {
                log.Fine("No BP Location Info to save");
                return null;
            }

            //	Address
            MLocation location = new MLocation(GetCtx(), GetC_Country_ID(),
                GetC_Region_ID(), GetCity(), Get_TrxName());
            location.SetAddress1(GetAddress1());
            location.SetAddress2(GetAddress2());
            location.SetPostal(GetPostal());
            location.SetPostal_Add(GetPostal_Add());
            location.SetRegionName(GetRegionName());
            if (location.Save())
            {
                MBPartnerLocation bpl = new MBPartnerLocation(_bp);
                bpl.SetC_Location_ID(location.GetC_Location_ID());
                bpl.SetPhone(GetPhone());
                bpl.SetPhone2(GetPhone2());
                bpl.SetFax(GetFax());
                bpl.SetC_SalesRegion_ID(GetC_SalesRegion_ID());
                if (bpl.Save())
                    SetC_BPartner_Location_ID(bpl.GetC_BPartner_Location_ID());
            }
            return null;
        }

        /**
         * 	Create Project from Lead
         *	@return error message
         */
        public String CreateProject(int C_ProjectType_ID)
        {
            if (GetC_Project_ID() != 0)
                return "@AlreadyExists@: @C_Project_ID@ (ID=" + GetC_Project_ID() + ")";
            if (GetC_BPartner_ID() == 0)
            {
                String retValue = CreateBP();
                if (retValue != null)
                    return retValue;
            }
            _project = new MProject(GetCtx(), 0, Get_TrxName());
            _project.SetAD_Org_ID(GetAD_Org_ID());
            _project.SetProjectLineLevel(MProject.PROJECTLINELEVEL_Project);
            _project.SetName(GetName());
            _project.SetDescription(GetDescription());
            _project.SetNote(GetHelp());
            //
            _project.SetC_BPartner_ID(GetC_BPartner_ID());
            _project.SetC_BPartner_Location_ID(GetC_BPartner_Location_ID());
            _project.SetAD_User_ID(GetAD_User_ID());
            _project.SetC_BPartnerSR_ID(GetC_BPartnerSR_ID());
            _project.SetC_Campaign_ID(GetC_Campaign_ID());

            _project.SetC_ProjectType_ID(C_ProjectType_ID);
            _project.SetSalesRep_ID(GetSalesRep_ID());
            _project.SetC_SalesRegion_ID(GetC_SalesRegion_ID());
            if (!_project.Save())
                return "@SaveError@";
            //
            if (GetRequest() != null)
            {
                _request.SetC_Project_ID(_project.GetC_Project_ID());
                _request.Save();
            }
            //
            SetC_Project_ID(_project.GetC_Project_ID());
            return null;
        }

        /**
         * 	Create Request from Lead
         * 	@param R_RequestType_ID request type
         *	@return error message
         */
        public String CreateRequest()
        {
            int R_RequestType_ID = GetR_RequestType_ID();
            if (R_RequestType_ID == 0)
                return "@NotFound@: @R_RequestType_ID@ (@AD_Client_ID@)";
            return CreateRequest(R_RequestType_ID);
        }

        /**
         * 	Create Request from Lead
         * 	@param R_RequestType_ID request type
         *	@return error message
         */
        private String CreateRequest(int R_RequestType_ID)
        {
            if (GetR_Request_ID() != 0)
                return "@AlreadyExists@: @R_Request_ID@ (ID=" + GetR_Request_ID() + ")";
            if (GetC_BPartner_ID() == 0)
            {
                String retValue = CreateBP();
                if (retValue != null)
                    return retValue;
            }
            _request = new MRequest(GetCtx(), 0, Get_TrxName());
            _request.SetAD_Org_ID(GetAD_Org_ID());
            String summary = GetName();
            if (summary == null)
                summary = GetHelp();
            if (summary == null)
                summary = GetSummary();
            if (summary == null)
                summary = GetDescription();
            _request.SetSummary(summary);
            //
            _request.SetR_RequestType_ID(R_RequestType_ID);
            if (IsR_Status_IDValid(GetR_Status_ID()))
                _request.SetR_Status_ID(GetR_Status_ID());
            else
                _request.SetR_Status_ID();
            //
            _request.SetC_Lead_ID(GetC_Lead_ID());
            //
            _request.SetC_BPartner_ID(GetC_BPartner_ID());
            _request.SetAD_User_ID(GetAD_User_ID());
            _request.SetC_Project_ID(GetC_Project_ID());
            _request.SetC_Campaign_ID(GetC_Campaign_ID());
            _request.SetR_Source_ID(GetR_Source_ID());
            _request.SetC_BPartnerSR_ID(GetC_BPartnerSR_ID());
            _request.SetC_SalesRegion_ID(GetC_SalesRegion_ID());

            _request.SetSalesRep_ID(GetSalesRep_ID());
            if (!_request.Save())
                return "@SaveError@";
            //
            SetR_Request_ID(_request.GetR_Request_ID());
            return null;
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            //	EMail Address specified
            if (GetEMail() != null && GetAD_User_ID() == 0)
                SetAD_User_ID();

            if (newRecord || Is_ValueChanged("R_Status_ID"))
            {
                if (!IsR_Status_IDValid(GetR_Status_ID()))
                    SetR_Status_ID(0);
                else if (_Status != null)
                    SetProcessed(_Status.IsClosed());
            }

            return true;
        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@returnsuccess
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;

            //	Create Contact Interest
            if (GetAD_User_ID() != 0 && GetR_InterestArea_ID() != 0
                && (Is_ValueChanged("AD_User_ID") || Is_ValueChanged("R_InterestArea_ID")))
            {
                MContactInterest ci = MContactInterest.Get(GetCtx(),
                    GetR_InterestArea_ID(), GetAD_User_ID(),
                    true, Get_TrxName());
                ci.Save();		//	don't subscribe or re-activate
            }
            return true;
        }



    }
}

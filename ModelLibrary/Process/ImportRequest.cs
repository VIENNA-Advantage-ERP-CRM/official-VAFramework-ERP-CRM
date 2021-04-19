/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ImportRequest
 * Purpose        : Import Request from I_Request
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           12-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class ImportRequest : ProcessEngine.SvrProcess
    {

        /**	Client to be imported to		*/
        private int _VAF_Client_ID = 0;
        /**	Organization to be imported to		*/
        private int _VAF_Org_ID = 0;
        /**	Delete old Imported				*/
        private bool _deleteOldImported = false;


        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {

            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (name.Equals("VAF_Client_ID"))
                    _VAF_Client_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("VAF_Org_ID"))
                    _VAF_Org_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("DeleteOldImported"))
                    _deleteOldImported = "Y".Equals(para[i].GetParameter());
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
        }
        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {

            StringBuilder sql = null;
            int no = 0;
            String clientCheck = " AND VAF_Client_ID=" + _VAF_Client_ID;

            //	****	Prepare	****

            //	Delete Old Imported
            if (_deleteOldImported)
            {
                sql = new StringBuilder("DELETE FROM I_Request "
                      + "WHERE I_IsImported='Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Delete Old Impored =" + no);
            }

            //Set Client from Key
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAF_Client_ID = (SELECT VAF_Client_ID FROM VAF_Client c WHERE c.Value = r.ClientValue), "
                      + " ClientName = (SELECT Name FROM VAF_Client c WHERE c.Value = r.ClientValue), "
                      + " Updated = COALESCE (Updated, SysDate),"
                      + " UpdatedBy = COALESCE (UpdatedBy, 0)"
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND VAF_Client_ID is NULL"
                      + " AND ClientValue is NOT NULL");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Set client from key =" + no);

            //	Set Client from Name
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAF_Client_ID = (SELECT VAF_Client_ID FROM VAF_Client c WHERE c.Name = r.ClientName), "
                      + " ClientValue = (SELECT Value FROM VAF_Client c WHERE c.Name = r.ClientName),"
                      + " Updated = COALESCE (Updated, SysDate),"
                      + " UpdatedBy = COALESCE (UpdatedBy, 0)"
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND VAF_Client_ID is NULL"
                      + " AND ClientName is NOT NULL");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Set client from name =" + no);

            //Set Org from Key
            sql = new StringBuilder("UPDATE I_Request r"
                          + " SET VAF_Org_ID = (SELECT VAF_Org_ID FROM VAF_Org o WHERE o.Value = r.OrgValue), "
                          + " OrgName = (SELECT Name FROM VAF_Org o WHERE o.Value = r.OrgValue), "
                          + " Updated = COALESCE (Updated, SysDate),"
                          + " UpdatedBy = COALESCE (UpdatedBy, 0)"
                          + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                          + " AND (VAF_Org_ID is NULL OR VAF_Org_ID =0)"
                          + " AND OrgValue is NOT NULL");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Set org from key =" + no);

            //	Set Org from Name
            sql = new StringBuilder("UPDATE I_Request r"
                          + " SET VAF_Org_ID = (SELECT VAF_Org_ID FROM VAF_Org o WHERE o.Name = r.OrgName), "
                          + " OrgValue = (SELECT Value FROM VAF_Org o WHERE o.Name = r.OrgName), "
                          + " Updated = COALESCE (Updated, SysDate),"
                          + " UpdatedBy = COALESCE (UpdatedBy, 0)"
                          + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                          + " AND (VAF_Org_ID is NULL OR VAF_Org_ID =0)"
                          + " AND OrgName is NOT NULL");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Set Org from name =" + no);


            sql = new StringBuilder("UPDATE I_Request r"
                  + " SET VAF_Client_ID = COALESCE (VAF_Client_ID,").Append(_VAF_Client_ID).Append("),"
                  + " VAF_Org_ID = COALESCE (VAF_Org_ID,").Append(_VAF_Org_ID).Append("),"
                  + " IsActive = COALESCE (IsActive, 'Y'),"
                  + " Created = COALESCE (Created, SysDate),"
                  + " CreatedBy = COALESCE (CreatedBy, 0),"
                  + " Updated = COALESCE (Updated, SysDate),"
                  + " UpdatedBy = COALESCE (UpdatedBy, 0),"
                  + " I_ErrorMsg = NULL,"
                  + " I_IsImported = 'N' "
                  + "WHERE I_IsImported<>'Y' OR I_IsImported IS NULL");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Reset=" + no);

            String ts = DataBase.DB.IsPostgreSQL() ?
                    "COALESCE(I_ErrorMsg,'')"
                    : "I_ErrorMsg";  //java bug, it could not be used directly
            sql = new StringBuilder("UPDATE I_Request r "
                    + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Org, '"
                    + "WHERE (VAF_Org_ID IS NULL "
                    + " OR NOT EXISTS (SELECT * FROM VAF_Org oo WHERE r.VAF_Org_ID=oo.VAF_Org_ID AND oo.IsSummary='N' AND oo.IsActive='Y'))"
                    + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Org=" + no);

            // Set Request Type
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAR_Req_Type_ID = "
                      + " (SELECT VAR_Req_Type_ID FROM VAR_Req_Type rt WHERE rt.Name = r.ReqTypeName)"
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAR_Req_Type_ID is NULL"
                      + " AND r.ReqTypeName is NOT NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set RequestType=" + no);

            // Error - Request Type not specified
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid RequestTypeName, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAR_Req_Type_ID is NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid ReqType=" + no);

            // Set Group
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAR_Group_ID = "
                      + " (SELECT VAR_Group_ID FROM VAR_Group g WHERE g.Name = r.GroupName)"
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAR_Group_ID is NULL"
                      + " AND r.GroupName is NOT NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Group=" + no);

            // Error - Invalid Group
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid GroupName, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAR_Group_ID is NULL"
                      + " AND r.GroupName is NOT NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid GroupName=" + no);

            // Set Category
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAR_Category_ID = "
                      + " (SELECT VAR_Category_ID FROM VAR_Category c WHERE c.Name = r.CategoryName)"
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAR_Category_ID is NULL"
                      + " AND r.CategoryName is NOT NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Category=" + no);

            // Error - Invalid Category
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid CategoryName, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAR_Category_ID is NULL"
                      + " AND r.CategoryName is NOT NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid CategoryName=" + no);


            // Set Status
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAR_Req_Status_ID = "
                      + " (SELECT VAR_Req_Status_ID FROM VAR_Req_Status s, VAR_Req_StatusCategory sc, VAR_Req_Type t "
                      + " WHERE s.Name = r.StatusName and t.VAR_Req_Type_ID = r.VAR_Req_Type_ID and t.VAR_Req_StatusCategory_ID = sc.VAR_Req_StatusCategory_ID"
                      + " AND s.VAR_Req_StatusCategory_ID = sc.VAR_Req_StatusCategory_ID)"
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAR_Req_Status_ID is NULL"
                      + " AND r.StatusName is NOT NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Status=" + no);

            // Error - Invalid Status
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid StatusName, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAR_Req_Status_ID is NULL"
                      + " AND r.StatusName is NOT NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Status=" + no);


            // Set Resolution
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAR_Resolution_ID = "
                      + " (SELECT VAR_Resolution_ID FROM VAR_Resolution rr "
                      + " WHERE rr.Name = r.ResolutionName )"
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAR_Resolution_ID is NULL"
                      + " AND r.ResolutionName is NOT NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Resolution=" + no);

            // Error - Invalid Resolution
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid ResolutionName, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAR_Resolution_ID is NULL"
                      + " AND r.ResolutionName is NOT NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Resolution=" + no);

            // Error - Invalid Summary
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Summary, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.Summary is NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Summary=" + no);

            // Set BP from BPartnerKey
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAB_BusinessPartner_ID = (SELECT VAB_BusinessPartner_ID FROM VAB_BusinessPartner b"
                      + " WHERE b.Value=r.BPartnerValue AND b.VAF_Client_ID=r.VAF_Client_ID ), "
                      + " BPartnerName=(SELECT Name FROM VAB_BusinessPartner b"
                      + " WHERE b.Value=r.BPartnerValue AND b.VAF_Client_ID=r.VAF_Client_ID )"
                      + " WHERE VAB_BusinessPartner_ID IS NULL AND BPartnerValue IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set BPartner from BPartnerKey=" + no);

            //	Set BP from BPartnerName
            sql = new StringBuilder("UPDATE I_Request r"
                  + " SET VAB_BusinessPartner_ID=(SELECT VAB_BusinessPartner_ID FROM VAB_BusinessPartner b"
                  + " WHERE b.Name=r.BpartnerName AND b.VAF_Client_ID=r.VAF_Client_ID ), "
                  + " BPartnerValue=(SELECT Value FROM VAB_BusinessPartner b"
                  + " WHERE b.Name=r.BpartnerName AND b.VAF_Client_ID=r.VAF_Client_ID )"
                  + " WHERE VAB_BusinessPartner_ID IS NULL AND BPartnerName IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set BPartner from BPartnerName=" + no);

            // Error - Invalid BPartner
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Bpartner, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAB_BusinessPartner_ID is NULL"
                      + " AND (r.BPartnerName is NOT NULL OR r.BPartnerValue IS NOT NULL)").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid BPartner=" + no);

            //	Set User from UserKey
            sql = new StringBuilder("UPDATE I_Request r"
                  + " SET VAF_UserContact_ID =(SELECT VAF_UserContact_ID FROM VAF_UserContact u"
                  + " WHERE r.ContactValue=u.Value AND r.VAF_Client_ID=u.VAF_Client_ID AND u.VAB_BusinessPartner_ID = r.VAB_BusinessPartner_ID), "
                  + " ContactName =(SELECT Name FROM VAF_UserContact u"
                  + " WHERE r.ContactValue=u.Value AND r.VAF_Client_ID=u.VAF_Client_ID AND u.VAB_BusinessPartner_ID = r.VAB_BusinessPartner_ID) "
                  + " WHERE VAF_UserContact_ID IS NULL AND ContactValue IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set User from UserKey=" + no);

            //	Set User from UserName
            sql = new StringBuilder("UPDATE I_Request r"
                  + " SET VAF_UserContact_ID=(SELECT VAF_UserContact_ID FROM VAF_UserContact u"
                  + " WHERE r.ContactName=u.Name AND r.VAF_Client_ID=u.VAF_Client_ID AND u.VAB_BusinessPartner_ID = r.VAB_BusinessPartner_ID ),"
                  + " ContactValue =(SELECT Value FROM VAF_UserContact u"
                  + " WHERE r.ContactName=u.Name AND r.VAF_Client_ID=u.VAF_Client_ID AND u.VAB_BusinessPartner_ID = r.VAB_BusinessPartner_ID )"
                  + " WHERE VAF_UserContact_ID IS NULL AND ContactName IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set User from UserName=" + no);

            // Error - Invalid User
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid User, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND ("
                      + " (r.VAF_UserContact_ID is NULL AND (r.ContactName is NOT NULL OR r.ContactValue IS NOT NULL))"
                      + " OR (r.VAF_UserContact_ID is NOT NULL AND NOT EXISTS"
                      + "(SELECT 1 FROM VAF_USERCONTACT u WHERE u.VAF_UserContact_ID = r.VAF_UserContact_ID "
                      + " AND r.VAF_Client_ID=u.VAF_Client_ID AND u.VAB_BusinessPartner_ID = r.VAB_BusinessPartner_ID)))").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid User=" + no);

            //	Set SalesRep from SalesRepKey
            sql = new StringBuilder("UPDATE I_Request r"
                  + " SET SalesRep_ID = (SELECT VAF_UserContact_ID FROM VAF_UserContact u"
                  + " WHERE r.SalesRepValue=u.Value AND r.VAF_Client_ID=u.VAF_Client_ID ), "
                  + " SalesRepName = (SELECT name FROM VAF_UserContact u"
                  + " WHERE r.SalesRepValue=u.Value AND r.VAF_Client_ID=u.VAF_Client_ID ) "
                  + "WHERE SalesRep_ID IS NULL AND SalesRepValue IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set SalesRep from SalesRepValue=" + no);

            //	Set Representative from RepresentativeName
            sql = new StringBuilder("UPDATE I_Request r"
                  + " SET SalesRep_ID = (SELECT VAF_UserContact_ID FROM VAF_UserContact u"
                  + " WHERE r.SalesRepName=u.Name AND r.VAF_Client_ID=u.VAF_Client_ID ), "
                  + " SalesRepValue =(SELECT  value FROM VAF_UserContact u"
                  + " WHERE r.SalesRepName=u.Name AND r.VAF_Client_ID=u.VAF_Client_ID )"
                  + " WHERE SalesRep_ID IS NULL AND SalesRepName IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set SalesRep from SalesRepName=" + no);


            // Error - Invalid SalesRep
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Representative, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.SalesRep_ID is NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid SalesRep=" + no);

            // Set Table
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAF_TableView_ID = "
                      + " (SELECT VAF_TableView_ID FROM VAF_TableView t "
                      + " WHERE t.Name = r.TableName )"
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAF_TableView_ID is NULL"
                      + " AND r.TableName is NOT NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Table=" + no);

            // Error - Invalid Table
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid TableName, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAF_TableView_ID is NULL"
                      + " AND r.TableName is NOT NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Table=" + no);

            // Set Related Request
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAR_RequestRelated_ID = "
                      + " (SELECT VAR_Request_ID FROM VAR_Request rr "
                      + " WHERE rr.DocumentNo = r.RequestRelatedDocNo )"
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAR_RequestRelated_ID is NULL"
                      + " AND r.RequestRelatedDocNo is NOT NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Resolution=" + no);

            // Error - Invalid Related Request
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Rel, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAR_RequestRelated_ID is NULL"
                      + " AND r.RequestRelatedDocNo is NOT NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid RelatedRequest=" + no);

            //	Set Source from SourceKey
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAR_Source_ID = (SELECT VAR_Source_ID FROM VAR_Source s"
                      + " WHERE r.SourceValue=s.Value AND r.VAF_Client_ID=s.VAF_Client_ID ), "
                      + " SourceName = (SELECT name FROM VAR_Source s"
                      + " WHERE r.SourceValue=s.Value AND r.VAF_Client_ID=s.VAF_Client_ID )"
                      + " WHERE VAR_Source_ID IS NULL AND SourceValue IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Source from SourceValue =" + no);

            //	Set Source from SourceName
            sql = new StringBuilder("UPDATE I_Request r"
                  + " SET VAR_Source_ID=(SELECT VAR_Source_ID FROM VAR_Source s"
                  + " WHERE r.SourceName=s.Name AND r.VAF_Client_ID=s.VAF_Client_ID ), "
                  + " SourceValue = (SELECT value FROM VAR_Source s"
                  + " WHERE r.SourceName=s.Name AND r.VAF_Client_ID=s.VAF_Client_ID ) "
                  + " WHERE VAR_Source_ID IS NULL AND SourceName IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Source from SourceName=" + no);


            // Error - Invalid Source
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Source, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAR_Source_ID IS NULL"
                      + " AND (r.SourceName IS NOT NULL OR r.SourceValue is NOT NULL)").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Source=" + no);

            // Set Role
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAF_Role_ID = "
                      + " (SELECT VAF_Role_ID FROM VAF_Role ar "
                      + " WHERE ar.Name = r.RoleName )"
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAF_Role_ID is NULL"
                      + " AND r.RoleName is NOT NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Role=" + no);

            // Error - Invalid Role
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid RoleName, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAF_Role_ID is NULL"
                      + " AND r.RoleName is NOT NULL").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Role=" + no);

            //	Set ProductSpent from ProductSpentKey
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAM_ProductSpent_ID=(SELECT VAM_Product_ID FROM VAM_Product m"
                      + " WHERE r.ProductSpentValue=m.Value AND r.VAF_Client_ID=m.VAF_Client_ID ), "
                      + " ProductSpentName =(SELECT Name FROM VAM_Product m"
                      + " WHERE r.ProductSpentValue=m.Value AND r.VAF_Client_ID=m.VAF_Client_ID ) "
                      + " WHERE VAM_ProductSpent_ID IS NULL AND ProductSpentValue IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set ProductSpent from ProductSpentValue =" + no);

            //	Set ProductSpent from ProductSpentName
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAM_ProductSpent_ID = (SELECT VAM_Product_ID FROM VAM_Product m"
                      + " WHERE r.ProductSpentName=m.Name AND r.VAF_Client_ID=m.VAF_Client_ID ), "
                      + " ProductSpentValue =(SELECT Value FROM VAM_Product m"
                      + " WHERE r.ProductSpentName=m.Name AND r.VAF_Client_ID=m.VAF_Client_ID )"
                      + " WHERE VAM_ProductSpent_ID IS NULL AND ProductSpentName IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set ProductSpent from ProductSpentValue =" + no);


            // Error - Invalid ProductSpent
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid ProductSpent, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAM_ProductSpent_ID IS NULL"
                      + " AND (r.productSpentName IS NOT NULL OR r.ProductSpentValue is NOT NULL)").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid ProductSpent=" + no);

            //	Set Activity from ActivityKey
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAB_BillingCode_ID=(SELECT VAB_BillingCode_ID FROM VAB_BillingCode a"
                      + " WHERE r.ActivityValue=a.Value AND r.VAF_Client_ID=a.VAF_Client_ID ), "
                      + " ActivityName=(SELECT Name FROM VAB_BillingCode a"
                      + " WHERE r.ActivityValue=a.Value AND r.VAF_Client_ID=a.VAF_Client_ID )"
                      + " WHERE VAB_BillingCode_ID IS NULL AND ActivityValue IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Activity from ActivityValue =" + no);

            //	Set Activity from ActivityName
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAB_BillingCode_ID = (SELECT VAB_BillingCode_ID FROM VAB_BillingCode a"
                      + " WHERE r.ActivityName=a.Name AND r.VAF_Client_ID=a.VAF_Client_ID ), "
                      + " ActivityValue =(SELECT Value FROM VAB_BillingCode a"
                      + " WHERE r.ActivityName=a.Name AND r.VAF_Client_ID=a.VAF_Client_ID )"
                      + " WHERE VAB_BillingCode_ID IS NULL AND ActivityName IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Activity from ActivityName =" + no);

            // Error - Invalid Activity
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Activity, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAB_BillingCode_ID IS NULL"
                      + " AND (r.ActivityName IS NOT NULL OR r.ActivityValue is NOT NULL)").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Activity=" + no);

            // Set BP from BPartnerKey
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAB_BusinessPartnerSR_ID=(SELECT VAB_BusinessPartner_ID FROM VAB_BusinessPartner b"
                      + " WHERE b.Value=r.BPartnerSRValue AND b.VAF_Client_ID=r.VAF_Client_ID AND isSalesRep='Y'), "
                      + " BPartnerSRName =(SELECT Name FROM VAB_BusinessPartner b"
                      + " WHERE b.Value=r.BPartnerSRValue AND b.VAF_Client_ID=r.VAF_Client_ID AND isSalesRep='Y')"
                      + " WHERE VAB_BusinessPartnerSR_ID IS NULL AND BPartnerSRValue IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set BPartnerSR from BPartnerSRKey=" + no);

            //	Set BP (Agent) from BPartnerSRName
            sql = new StringBuilder("UPDATE I_Request r"
                  + " SET VAB_BusinessPartnerSR_ID=(SELECT VAB_BusinessPartner_ID FROM VAB_BusinessPartner b"
                  + " WHERE b.Name=r.BPartnerSRName AND b.VAF_Client_ID=r.VAF_Client_ID AND isSalesRep='Y'), "
                  + " BPartnerSRValue =(SELECT value FROM VAB_BusinessPartner b"
                  + " WHERE b.Name=r.BPartnerSRName AND b.VAF_Client_ID=r.VAF_Client_ID AND isSalesRep='Y')"
                  + " WHERE VAB_BusinessPartnerSR_ID IS NULL AND BPartnerSRName IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set BPartnerSR from BPartnerSRName=" + no);

            // Error - Invalid BPartnerSR
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Bpartner(Agent), ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND ((r.VAB_BusinessPartnerSR_ID is NULL AND (r.BPartnerSRName is NOT NULL OR r.BPartnerSRValue IS NOT NULL))"
                      + " OR (r.VAB_BusinessPartnerSR_ID is NOT NULL AND "
                      + " NOT EXISTS (SELECT 1 from VAB_BusinessPartner b WHERE b.VAB_BusinessPartner_ID = r.VAB_BusinessPartnerSR_ID"
                      + " AND b.VAF_Client_ID=r.VAF_Client_ID AND isSalesRep='Y')))").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid BPartner (Agent)=" + no);

            // Set Project from ProjectKey
            sql = new StringBuilder("UPDATE I_Request r "
                      + " SET VAB_Project_ID=(SELECT VAB_Project_ID FROM VAB_Project p"
                      + " WHERE p.Value=r.ProjectValue AND p.VAF_Client_ID=r.VAF_Client_ID ), "
                      + " ProjectName=(SELECT Name FROM VAB_Project p"
                      + " WHERE p.Value=r.ProjectValue AND p.VAF_Client_ID=r.VAF_Client_ID ) "
                      + " WHERE VAB_Project_ID IS NULL AND ProjectValue IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Project from ProjectKey=" + no);

            // Set Project from ProjectName
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAB_Project_ID=(SELECT VAB_Project_ID FROM VAB_Project p"
                      + " WHERE p.Name=r.ProjectName AND p.VAF_Client_ID=r.VAF_Client_ID ), "
                      + " ProjectValue=(SELECT Value FROM VAB_Project p"
                      + " WHERE p.Name=r.ProjectName AND p.VAF_Client_ID=r.VAF_Client_ID ) "
                      + " WHERE VAB_Project_ID IS NULL AND ProjectName IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Project from ProjectName=" + no);

            // Error - Invalid Project
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Project, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAB_Project_ID is NULL"
                      + " AND (r.ProjectName is NOT NULL OR r.ProjectValue IS NOT NULL)").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Project=" + no);

            // Set Asset from AssetKey
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAA_Asset_ID=(SELECT VAA_Asset_ID FROM VAA_Asset a"
                      + " WHERE a.Value=r.AssetValue AND a.VAF_Client_ID=r.VAF_Client_ID ), "
                      + " AssetName =(SELECT Name FROM VAA_Asset a"
                      + " WHERE a.Value=r.AssetValue AND a.VAF_Client_ID=r.VAF_Client_ID ) "
                      + " WHERE VAA_Asset_ID IS NULL AND AssetValue IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Asset from AssetKey=" + no);

            // Set Project from AssetName
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAA_Asset_ID=(SELECT VAA_Asset_ID FROM VAA_Asset a"
                      + " WHERE a.Name=r.AssetName AND a.VAF_Client_ID=r.VAF_Client_ID ), "
                      + " AssetValue=(SELECT Value FROM VAA_Asset a"
                      + " WHERE a.Name=r.AssetName AND a.VAF_Client_ID=r.VAF_Client_ID ) "
                      + " WHERE VAA_Asset_ID IS NULL AND AssetName IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Asset from AssetName=" + no);

            // Error - Invalid Asset
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Asset, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAA_Asset_ID is NULL"
                      + " AND (r.AssetName is NOT NULL OR r.AssetValue IS NOT NULL)").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Asset=" + no);

            //	Set Product from ProductKey
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAM_Product_ID=(SELECT VAM_Product_ID FROM VAM_Product m"
                      + " WHERE r.ProductValue=m.Value AND r.VAF_Client_ID=m.VAF_Client_ID ), "
                      + " ProductName=(SELECT Name FROM VAM_Product m"
                      + " WHERE r.ProductValue=m.Value AND r.VAF_Client_ID=m.VAF_Client_ID )"
                      + " WHERE VAM_Product_ID IS NULL AND ProductValue IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Product from ProductValue =" + no);

            //	Set Product from ProductName
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAM_Product_ID =(SELECT VAM_Product_ID FROM VAM_Product m"
                      + " WHERE r.ProductName=m.Name AND r.VAF_Client_ID=m.VAF_Client_ID ), "
                      + " ProductValue =(SELECT Value FROM VAM_Product m"
                      + " WHERE r.ProductName=m.Name AND r.VAF_Client_ID=m.VAF_Client_ID )"
                      + " WHERE VAM_Product_ID IS NULL AND ProductName IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Product from ProductName =" + no);


            // Error - Invalid Product
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid ProductSpent, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAM_Product_ID IS NULL"
                      + " AND (r.productName IS NOT NULL OR r.ProductValue is NOT NULL)").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Product=" + no);

            //	Set Campaign from CampaignKey
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAB_Promotion_ID=(SELECT VAB_Promotion_ID FROM VAB_Promotion c"
                      + " WHERE r.CampaignValue=c.Value AND r.VAF_Client_ID=c.VAF_Client_ID ), "
                      + " CampaignName =(SELECT Name FROM VAB_Promotion c"
                      + " WHERE r.CampaignValue=c.Value AND r.VAF_Client_ID=c.VAF_Client_ID )"
                      + " WHERE VAB_Promotion_ID IS NULL AND CampaignValue IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Campaign from CampaignValue =" + no);

            //	Set Campaign from CampaignName
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAB_Promotion_ID =(SELECT VAB_Promotion_ID FROM VAB_Promotion c"
                      + " WHERE r.CampaignName=c.Name AND r.VAF_Client_ID=c.VAF_Client_ID ), "
                      + " CampaignValue =(SELECT Value FROM VAB_Promotion c"
                      + " WHERE r.CampaignName=c.Name AND r.VAF_Client_ID=c.VAF_Client_ID )"
                      + " WHERE VAB_Promotion_ID IS NULL AND CampaignName IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Campaign from CampaignValue =" + no);


            // Error - Invalid Campaign
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Campaign, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAB_Promotion_ID IS NULL"
                      + " AND (r.CampaignName IS NOT NULL OR r.CampaignValue is NOT NULL)").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Campaign=" + no);

            //	Set SalesRegion from SalesRegionKey
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAB_SalesRegionState_ID=(SELECT VAB_SalesRegionState_ID FROM VAB_SalesRegionState s"
                      + " WHERE r.SalesRegionValue=s.Value AND r.VAF_Client_ID=s.VAF_Client_ID ), "
                      + " SalesRegionName=(SELECT Name FROM VAB_SalesRegionState s"
                      + " WHERE r.SalesRegionValue=s.Value AND r.VAF_Client_ID=s.VAF_Client_ID )"
                      + " WHERE VAB_SalesRegionState_ID IS NULL AND SalesRegionValue IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set SalesRegion from SalesRegionValue =" + no);

            //	Set SalesRegion from SalesRegionName
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAB_SalesRegionState_ID=(SELECT VAB_SalesRegionState_ID FROM VAB_SalesRegionState s"
                      + " WHERE r.SalesRegionName=s.Name AND r.VAF_Client_ID=s.VAF_Client_ID ), "
                      + " SalesRegionValue=(SELECT SalesRegionValue FROM VAB_SalesRegionState s"
                      + " WHERE r.SalesRegionName=s.Name AND r.VAF_Client_ID=s.VAF_Client_ID )"
                      + " WHERE VAB_SalesRegionState_ID IS NULL AND SalesRegionName IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set SalesRegion from SalesRegionValue =" + no);

            // Error - Invalid SalesRegion
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid SalesRegion, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAB_SalesRegionState_ID IS NULL"
                      + " AND (r.SalesRegionName IS NOT NULL OR r.SalesRegionValue is NOT NULL)").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid SalesRegion=" + no);

            //	Set Order from OrderDocumentNo
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAB_Order_ID =(SELECT VAB_Order_ID FROM VAB_Order o"
                      + " WHERE r.OrderDocumentNo = o.DocumentNo AND r.VAF_Client_ID=o.VAF_Client_ID )"
                      + " WHERE VAB_Order_ID IS NULL AND OrderDocumentNo IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Order from OrderDocumentNo =" + no);

            // Error - Invalid OrderDocumentNo
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid OrderDocumentNo, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAB_Order_ID IS NULL"
                      + " AND r.OrderDocumentNo IS NOT NULL ").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid OrderDocumentNo=" + no);

            //	Set Invoice from InvoiceDocumentNo
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAB_Invoice_ID =(SELECT VAB_Invoice_ID FROM VAB_Invoice i"
                      + " WHERE r.InvoiceDocumentNo = i.DocumentNo AND r.VAF_Client_ID=i.VAF_Client_ID )"
                      + " WHERE VAB_Invoice_ID IS NULL AND InvoiceDocumentNo IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Invoice from InvoiceDocumentNo =" + no);

            // Error - Invalid InvoiceDocumentNo
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid InvoiceDocumentNo, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAB_Invoice_ID IS NULL"
                      + " AND r.InvoiceDocumentNo IS NOT NULL ").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid InvoiceDocumentNo=" + no);

            //	Set Payment from PaymentDocumentNo
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAB_Payment_ID =(SELECT VAB_Payment_ID FROM VAB_Payment p"
                      + " WHERE r.PaymentDocumentNo = p.DocumentNo AND r.VAF_Client_ID=p.VAF_Client_ID )"
                      + " WHERE VAB_Payment_ID IS NULL AND PaymentDocumentNo IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Payment from PaymentDocumentNo =" + no);

            // Error - Invalid PaymentDocumentNo
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid PaymentDocumentNo, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAB_Payment_ID IS NULL"
                      + " AND r.PaymentDocumentNo IS NOT NULL ").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid PaymentDocumentNo=" + no);

            //	Set Order from InOutDocumentNo
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAM_Inv_InOut_ID =(SELECT VAM_Inv_InOut_ID FROM VAM_Inv_InOut i"
                      + " WHERE r.InOutDocumentNo = i.DocumentNo AND r.VAF_Client_ID=i.VAF_Client_ID )"
                      + " WHERE VAM_Inv_InOut_ID IS NULL AND InOutDocumentNo IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Shipment/Receipt from InOutDocumentNo =" + no);

            // Error - Invalid InOutDocumentNo
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid InOutDocumentNo, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAM_Inv_InOut_ID IS NULL"
                      + " AND r.InOutDocumentNo IS NOT NULL ").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid InOutDocumentNo=" + no);

            //	Set Order from RMADocumentNo
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET M_RMA_ID =(SELECT M_RMA_ID FROM M_RMA rm"
                      + " WHERE r.RMADocumentNo = rm.DocumentNo AND r.VAF_Client_ID=rm.VAF_Client_ID )"
                      + " WHERE M_RMA_ID IS NULL AND RMADocumentNo IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Order from RMADocumentNo =" + no);

            // Error - Invalid RMADocumentNo
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid RMADocumentNo, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.M_RMA_ID IS NULL"
                      + " AND r.RMADocumentNo IS NOT NULL ").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid RMADocumentNo=" + no);

            //	Set Lead from LeadDocumentNo
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAB_Lead_ID =(SELECT VAB_Lead_ID FROM VAB_Lead l"
                      + " WHERE r.LeadDocumentNo = l.DocumentNo AND r.VAF_Client_ID=l.VAF_Client_ID )"
                      + " WHERE VAB_Lead_ID IS NULL AND LeadDocumentNo IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Lead from LeadDocumentNo =" + no);

            // Error - Invalid LeadDocumentNo
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid LeadDocumentNo, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAB_Lead_ID IS NULL"
                      + " AND r.LeadDocumentNo IS NOT NULL ").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid LeadDocumentNo=" + no);

            //	Set ChangeRequest from ChangeRequestName
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET VAM_ChangeRequest_ID =(SELECT VAM_ChangeRequest_ID FROM M_ChangeRequest c"
                      + " WHERE r.ChangeRequestName = c.Name AND r.VAF_Client_ID=c.VAF_Client_ID )"
                      + " WHERE VAM_ChangeRequest_ID IS NULL AND ChangeRequestName IS NOT NULL"
                      + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set ChangeRequest from ChangeRequestName =" + no);

            // Error - Invalid ChangeRequestName
            sql = new StringBuilder("UPDATE I_Request r"
                      + " SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid ChangeRequestName, ' "
                      + " WHERE (I_IsImported<>'Y' OR I_IsImported IS NULL)"
                      + " AND r.VAM_ChangeRequest_ID IS NULL"
                      + " AND r.ChangeRequestName IS NOT NULL ").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid ChangeRequestName=" + no);

            Commit();

            //	-- New Requests -----------------------------------------------------

            int noInsert = 0;
            int noProcessed = 0;

            //	Go through Order Records w/o
            sql = new StringBuilder("SELECT * FROM I_Request "
                      + "WHERE I_IsImported='N'").Append(clientCheck)
                        .Append(" ORDER BY DocumentNo, I_Request_ID");
            IDataReader idr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement (sql.ToString(), Get_TrxName());
                //ResultSet rs = pstmt.executeQuery ();
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
               // String oldDocumentNo = "";
                MVARRequest request = null;

                while (idr.Read())
                {
                    X_VAI_Request imp = new X_VAI_Request(GetCtx(), idr, Get_TrxName());
                    request = new MVARRequest(imp);

                    // Save Request
                    if (!request.Save())
                    {
                        String msg = "Could not save Request";
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null)
                            msg += " - " + pp.ToString();
                        imp.SetI_ErrorMsg(msg);
                        imp.Save();
                        continue;
                    }

                    noProcessed++;
                    imp.SetVAR_Request_ID(request.GetVAR_Request_ID());
                    imp.SetI_IsImported(X_VAI_Order.I_ISIMPORTED_Yes);
                    imp.SetProcessed(true);
                    imp.Save();
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "Order - " + sql.ToString(), e);
            }

            //Set Error to indicator to not imported
            sql = new StringBuilder("UPDATE I_Request "
                + "SET I_IsImported='N', Updated=SysDate "
                + "WHERE I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, Utility.Util.GetValueOfDecimal(no), "@Errors@");
            //addLog (0, null, new BigDecimal (noInsert), "@VAR_Request_ID@: @Inserted@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noProcessed), " @Processed@");
            return "#" + noInsert + "/" + noProcessed;
        }



    }
}

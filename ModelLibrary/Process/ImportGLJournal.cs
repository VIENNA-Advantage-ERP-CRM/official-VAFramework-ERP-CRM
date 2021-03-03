/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ImportGLJournal
 * Purpose        : Import GL Journal Batch/JournalLine from I_Journal
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
    public class ImportGLJournal : ProcessEngine.SvrProcess
    {
        /**	Client to be imported to		*/
        private int _VAF_Client_ID = 0;
        /**	Organization to be imported to	*/
        private int _VAF_Org_ID = 0;
        /**	Acct Schema to be imported to	*/
        private int _VAB_AccountBook_ID = 0;
        /** Default Date					*/
        private DateTime? _DateAcct = null;
        /**	Delete old Imported				*/
        private bool _DeleteOldImported = false;
        /**	Don't import					*/
        private bool _IsValidateOnly = false;
        /** Import if no Errors				*/
        private bool _IsImportOnlyNoErrors = true;


        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAF_Client_ID"))
                    _VAF_Client_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("VAF_Org_ID"))
                    _VAF_Org_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("VAB_AccountBook_ID"))
                    _VAB_AccountBook_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("DateAcct"))
                    _DateAcct = (DateTime?)para[i].GetParameter();
                else if (name.Equals("IsValidateOnly"))
                    _IsValidateOnly = "Y".Equals(para[i].GetParameter());
                else if (name.Equals("IsImportOnlyNoErrors"))
                    _IsImportOnlyNoErrors = "Y".Equals(para[i].GetParameter());
                else if (name.Equals("DeleteOldImported"))
                    _DeleteOldImported = "Y".Equals(para[i].GetParameter());
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
        }	//	prepare


        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("IsValidateOnly=" + _IsValidateOnly + ", IsImportOnlyNoErrors=" + _IsImportOnlyNoErrors);
            StringBuilder sql = null;
            int no = 0;
            String clientCheck = " AND VAF_Client_ID=" + _VAF_Client_ID;

            //	****	Prepare	****

            //	Delete Old Imported
            if (_DeleteOldImported)
            {
                sql = new StringBuilder("DELETE FROM I_GLJournal "
                      + "WHERE I_IsImported='Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Delete Old Impored =" + no);
            }

            //	Set IsActive, Created/Updated
            sql = new StringBuilder("UPDATE I_GLJournal "
                + "SET IsActive = COALESCE (IsActive, 'Y'),"
                + " Created = COALESCE (Created, SysDate),"
                + " CreatedBy = COALESCE (CreatedBy, 0),"
                + " Updated = COALESCE (Updated, SysDate),"
                + " UpdatedBy = COALESCE (UpdatedBy, 0),"
                + " I_ErrorMsg = NULL,"
                + " I_IsImported = 'N' "
                + "WHERE I_IsImported<>'Y' OR I_IsImported IS NULL");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Reset=" + no);

            //	Set Client from Name
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET VAF_Client_ID=(SELECT c.VAF_Client_ID FROM VAF_Client c WHERE c.Value=i.ClientValue) "
                + "WHERE (VAF_Client_ID IS NULL OR VAF_Client_ID=0) AND ClientValue IS NOT NULL"
                + " AND I_IsImported<>'Y'");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Client from Value=" + no);

            //	Set Default Client, Doc Org, AcctSchema, DatAcct
            sql = new StringBuilder("UPDATE I_GLJournal "
                  + "SET VAF_Client_ID = COALESCE (VAF_Client_ID,").Append(_VAF_Client_ID).Append("),"
                  + " VAF_OrgDoc_ID = COALESCE (VAF_OrgDoc_ID,").Append(_VAF_Org_ID).Append("),");
            if (_VAB_AccountBook_ID != 0)
                sql.Append(" VAB_AccountBook_ID = COALESCE (VAB_AccountBook_ID,").Append(_VAB_AccountBook_ID).Append("),");
            if (_DateAcct != null)
                sql.Append(" DateAcct = COALESCE (DateAcct,").Append(DataBase.DB.TO_DATE(_DateAcct)).Append("),");
            sql.Append(" Updated = COALESCE (Updated, SysDate) "
                  + "WHERE I_IsImported<>'Y' OR I_IsImported IS NULL");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Client/DocOrg/Default=" + no);

            //	Error Doc Org
            String ts = DataBase.DB.IsPostgreSQL() ? "COALESCE(I_ErrorMsg,'')" : "I_ErrorMsg";  //java bug, it could not be used directly
            sql = new StringBuilder("UPDATE I_GLJournal o "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Doc Org, '"
                + "WHERE (VAF_OrgDoc_ID IS NULL OR VAF_OrgDoc_ID=0"
                + " OR EXISTS (SELECT * FROM VAF_Org oo WHERE o.VAF_Org_ID=oo.VAF_Org_ID AND (oo.IsSummary='Y' OR oo.IsActive='N')))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Doc Org=" + no);

            //	Set AcctSchema
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET VAB_AccountBook_ID=(SELECT a.VAB_AccountBook_ID FROM VAB_AccountBook a"
                + " WHERE i.AcctSchemaName=a.Name AND i.VAF_Client_ID=a.VAF_Client_ID) "
                + "WHERE VAB_AccountBook_ID IS NULL AND AcctSchemaName IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set AcctSchema from Name=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET VAB_AccountBook_ID=(SELECT c.VAB_AccountBook1_ID FROM VAF_ClientDetail c WHERE c.VAF_Client_ID=i.VAF_Client_ID) "
                + "WHERE VAB_AccountBook_ID IS NULL AND AcctSchemaName IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set AcctSchema from Client=" + no);
            //	Error AcctSchema
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid AcctSchema, '"
                + "WHERE (VAB_AccountBook_ID IS NULL OR VAB_AccountBook_ID=0"
                + " OR NOT EXISTS (SELECT * FROM VAB_AccountBook a WHERE i.VAF_Client_ID=a.VAF_Client_ID))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid AcctSchema=" + no);

            //	Set DateAcct (mandatory)
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET DateAcct=SysDate "
                + "WHERE DateAcct IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set DateAcct=" + no);

            //	Document Type
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET VAB_DocTypes_ID=(SELECT d.VAB_DocTypes_ID FROM VAB_DocTypes d"
                + " WHERE d.Name=i.DocTypeName AND d.DocBaseType='GLJ' AND i.VAF_Client_ID=d.VAF_Client_ID) "
                + "WHERE VAB_DocTypes_ID IS NULL AND DocTypeName IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set DocType=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid DocType, '"
                + "WHERE (VAB_DocTypes_ID IS NULL OR VAB_DocTypes_ID=0"
                + " OR NOT EXISTS (SELECT * FROM VAB_DocTypes d WHERE i.VAF_Client_ID=d.VAF_Client_ID AND d.DocBaseType='GLJ'))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid DocType=" + no);

            //	GL Category
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET VAGL_Group_ID=(SELECT c.VAGL_Group_ID FROM VAGL_Group c"
                + " WHERE c.Name=i.CategoryName AND i.VAF_Client_ID=c.VAF_Client_ID) "
                + "WHERE VAGL_Group_ID IS NULL AND CategoryName IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set DocType=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Category, '"
                + "WHERE (VAGL_Group_ID IS NULL OR VAGL_Group_ID=0)"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Category=" + no);

            //	Set Currency
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET VAB_Currency_ID=(SELECT c.VAB_Currency_ID FROM VAB_Currency c"
                + " WHERE c.ISO_Code=i.ISO_Code AND c.VAF_Client_ID IN (0,i.VAF_Client_ID)) "
                + "WHERE VAB_Currency_ID IS NULL AND ISO_Code IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Currency from ISO=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET VAB_Currency_ID=(SELECT a.VAB_Currency_ID FROM VAB_AccountBook a"
                + " WHERE a.VAB_AccountBook_ID=i.VAB_AccountBook_ID AND a.VAF_Client_ID=i.VAF_Client_ID)"
                + "WHERE VAB_Currency_ID IS NULL AND ISO_Code IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Default Currency=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Currency, '"
                + "WHERE (VAB_Currency_ID IS NULL OR VAB_Currency_ID=0)"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Currency=" + no);

            //	Set Conversion Type
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET ConversionTypeValue='S' "
                + "WHERE VAB_CurrencyType_ID IS NULL AND ConversionTypeValue IS NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set CurrencyType Value to Spot =" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET VAB_CurrencyType_ID=(SELECT c.VAB_CurrencyType_ID FROM VAB_CurrencyType c"
                + " WHERE c.Value=i.ConversionTypeValue AND c.VAF_Client_ID IN (0,i.VAF_Client_ID)) "
                + "WHERE VAB_CurrencyType_ID IS NULL AND ConversionTypeValue IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set CurrencyType from Value=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid CurrencyType, '"
                + "WHERE (VAB_CurrencyType_ID IS NULL OR VAB_CurrencyType_ID=0) AND ConversionTypeValue IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid CurrencyTypeValue=" + no);


            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No ConversionType, '"
                + "WHERE (VAB_CurrencyType_ID IS NULL OR VAB_CurrencyType_ID=0)"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No CourrencyType=" + no);

            //	Set/Overwrite Home Currency Rate
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET CurrencyRate=1"
                + "WHERE EXISTS (SELECT * FROM VAB_AccountBook a"
                + " WHERE a.VAB_AccountBook_ID=i.VAB_AccountBook_ID AND a.VAB_Currency_ID=i.VAB_Currency_ID)"
                + " AND VAB_Currency_ID IS NOT NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Home CurrencyRate=" + no);
            //	Set Currency Rate
            sql = new StringBuilder("UPDATE I_GLJournal i "
               + "SET CurrencyRate=(SELECT MAX(r.MultiplyRate) FROM VAB_ExchangeRate r, VAB_AccountBook s"
               + " WHERE s.VAB_AccountBook_ID=i.VAB_AccountBook_ID AND s.VAF_Client_ID=i.VAF_Client_ID"
               + " AND r.VAB_Currency_ID=i.VAB_Currency_ID AND r.VAB_Currency_ID_TO=s.VAB_Currency_ID"
               + " AND r.VAF_Client_ID=i.VAF_Client_ID AND r.VAF_Org_ID=i.VAF_OrgDoc_ID"
               + " AND r.VAB_CurrencyType_ID=i.VAB_CurrencyType_ID"
               + " AND i.DateAcct BETWEEN r.ValidFrom AND r.ValidTo "
                //	ORDER BY ValidFrom DESC
               + ") WHERE CurrencyRate IS NULL OR CurrencyRate=0 AND VAB_Currency_ID>0"
               + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Org Rate=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET CurrencyRate=(SELECT MAX(r.MultiplyRate) FROM VAB_ExchangeRate r, VAB_AccountBook s"
                + " WHERE s.VAB_AccountBook_ID=i.VAB_AccountBook_ID AND s.VAF_Client_ID=i.VAF_Client_ID"
                + " AND r.VAB_Currency_ID=i.VAB_Currency_ID AND r.VAB_Currency_ID_TO=s.VAB_Currency_ID"
                + " AND r.VAF_Client_ID=i.VAF_Client_ID"
                + " AND r.VAB_CurrencyType_ID=i.VAB_CurrencyType_ID"
                + " AND i.DateAcct BETWEEN r.ValidFrom AND r.ValidTo "
                //	ORDER BY ValidFrom DESC
                + ") WHERE CurrencyRate IS NULL OR CurrencyRate=0 AND VAB_Currency_ID>0"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Client Rate=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No Rate, '"
                + "WHERE CurrencyRate IS NULL OR CurrencyRate=0"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No Rate=" + no);

            //	Set Period
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET VAB_YearPeriod_ID=(SELECT MAX(p.VAB_YearPeriod_ID) FROM VAB_YearPeriod p"
                + " INNER JOIN VAB_Year y ON (y.VAB_Year_ID=p.VAB_Year_ID)"
                + " INNER JOIN VAF_ClientDetail c ON (c.VAB_Calender_ID=y.VAB_Calender_ID)"
                + " WHERE c.VAF_Client_ID=i.VAF_Client_ID"
                + " AND i.DateAcct BETWEEN p.StartDate AND p.EndDate AND p.PeriodType='S') "
                + "WHERE VAB_YearPeriod_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Period=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Period, '"
                + "WHERE VAB_YearPeriod_ID IS NULL OR VAB_YearPeriod_ID NOt IN "
                + "(SELECT VAB_YearPeriod_ID FROM VAB_YearPeriod p"
                + " INNER JOIN VAB_Year y ON (y.VAB_Year_ID=p.VAB_Year_ID)"
                + " INNER JOIN VAF_ClientDetail c ON (c.VAB_Calender_ID=y.VAB_Calender_ID) "
                + " WHERE c.VAF_Client_ID=i.VAF_Client_ID"
                + " AND i.DateAcct BETWEEN p.StartDate AND p.EndDate AND p.PeriodType='S')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Period=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_ErrorMsg=" + ts + "||'WARN=Period Closed, ' "
                + "WHERE VAB_YearPeriod_ID IS NOT NULL AND NOT EXISTS"
                + " (SELECT * FROM VAB_YearPeriodControl pc WHERE pc.VAB_YearPeriod_ID=i.VAB_YearPeriod_ID AND DocBaseType='GLJ' AND PeriodStatus='O') "
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Period Closed=" + no);

            //	Posting Type
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET PostingType='A' "
                + "WHERE PostingType IS NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Actual PostingType=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid PostingType, ' "
                + "WHERE PostingType IS NULL OR NOT EXISTS"
                + " (SELECT * FROM VAF_CtrlRef_List r WHERE r.VAF_Control_Ref_ID=125 AND i.PostingType=r.Value)"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid PostingTypee=" + no);


            //	** Account Elements (optional) **
            //	(VAB_Acct_ValidParameter_ID IS NULL OR VAB_Acct_ValidParameter_ID=0)

            //	Set Org from Name (* is overwritten and default)
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET VAF_Org_ID=(SELECT o.VAF_Org_ID FROM VAF_Org o"
                + " WHERE o.Value=i.OrgValue AND o.IsSummary='N' AND i.VAF_Client_ID=o.VAF_Client_ID) "
                + "WHERE (VAF_Org_ID IS NULL OR VAF_Org_ID=0) AND OrgValue IS NOT NULL"
                + " AND (VAB_Acct_ValidParameter_ID IS NULL OR VAB_Acct_ValidParameter_ID=0) AND I_IsImported<>'Y'");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Org from Value=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET VAF_Org_ID=VAF_OrgDoc_ID "
                + "WHERE (VAF_Org_ID IS NULL OR VAF_Org_ID=0) AND OrgValue IS NULL AND VAF_OrgDoc_ID IS NOT NULL AND VAF_OrgDoc_ID<>0"
                + " AND (VAB_Acct_ValidParameter_ID IS NULL OR VAB_Acct_ValidParameter_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Org from Doc Org=" + no);
            //	Error Org
            sql = new StringBuilder("UPDATE I_GLJournal o "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Org, '"
                + "WHERE (VAF_Org_ID IS NULL OR VAF_Org_ID=0"
                + " OR EXISTS (SELECT * FROM VAF_Org oo WHERE o.VAF_Org_ID=oo.VAF_Org_ID AND (oo.IsSummary='Y' OR oo.IsActive='N')))"
                + " AND (VAB_Acct_ValidParameter_ID IS NULL OR VAB_Acct_ValidParameter_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Org=" + no);

            //	Set Account
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET Account_ID=(SELECT MAX(ev.VAB_Acct_Element_ID) FROM VAB_Acct_Element ev"
                + " INNER JOIN VAB_Element e ON (e.VAB_Element_ID=ev.VAB_Element_ID)"
                + " INNER JOIN VAB_AccountBook_Element ase ON (e.VAB_Element_ID=ase.VAB_Element_ID AND ase.ElementType='AC')"
                + " WHERE ev.Value=i.AccountValue AND ev.IsSummary='N'"
                + " AND i.VAB_AccountBook_ID=ase.VAB_AccountBook_ID AND i.VAF_Client_ID=ev.VAF_Client_ID) "
                + "WHERE Account_ID IS NULL AND AccountValue IS NOT NULL"
                + " AND (VAB_Acct_ValidParameter_ID IS NULL OR VAB_Acct_ValidParameter_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Account from Value=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Account, '"
                + "WHERE (Account_ID IS NULL OR Account_ID=0)"
                + " AND (VAB_Acct_ValidParameter_ID IS NULL OR VAB_Acct_ValidParameter_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Account=" + no);

            //	Set BPartner
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET VAB_BusinessPartner_ID=(SELECT bp.VAB_BusinessPartner_ID FROM VAB_BusinessPartner bp"
                + " WHERE bp.Value=i.BPartnerValue AND bp.IsSummary='N' AND i.VAF_Client_ID=bp.VAF_Client_ID) "
                + "WHERE VAB_BusinessPartner_ID IS NULL AND BPartnerValue IS NOT NULL"
                + " AND (VAB_Acct_ValidParameter_ID IS NULL OR VAB_Acct_ValidParameter_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set BPartner from Value=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid BPartner, '"
                + "WHERE VAB_BusinessPartner_ID IS NULL AND BPartnerValue IS NOT NULL"
                + " AND (VAB_Acct_ValidParameter_ID IS NULL OR VAB_Acct_ValidParameter_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid BPartner=" + no);

            //	Set Product
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET VAM_Product_ID=(SELECT MAX(p.VAM_Product_ID) FROM VAM_Product p"
                + " WHERE (p.Value=i.ProductValue OR p.UPC=i.UPC OR p.SKU=i.SKU)"
                + " AND p.IsSummary='N' AND i.VAF_Client_ID=p.VAF_Client_ID) "
                + "WHERE VAM_Product_ID IS NULL AND (ProductValue IS NOT NULL OR UPC IS NOT NULL OR SKU IS NOT NULL)"
                + " AND (VAB_Acct_ValidParameter_ID IS NULL OR VAB_Acct_ValidParameter_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Product from Value=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Product, '"
                + "WHERE VAM_Product_ID IS NULL AND (ProductValue IS NOT NULL OR UPC IS NOT NULL OR SKU IS NOT NULL)"
                + " AND (VAB_Acct_ValidParameter_ID IS NULL OR VAB_Acct_ValidParameter_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Product=" + no);

            //	Set Project
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET VAB_Project_ID=(SELECT p.VAB_Project_ID FROM VAB_Project p"
                + " WHERE p.Value=i.ProjectValue AND p.IsSummary='N' AND i.VAF_Client_ID=p.VAF_Client_ID) "
                + "WHERE VAB_Project_ID IS NULL AND ProjectValue IS NOT NULL"
                + " AND (VAB_Acct_ValidParameter_ID IS NULL OR VAB_Acct_ValidParameter_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Project from Value=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Project, '"
                + "WHERE VAB_Project_ID IS NULL AND ProjectValue IS NOT NULL"
                + " AND (VAB_Acct_ValidParameter_ID IS NULL OR VAB_Acct_ValidParameter_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Project=" + no);

            //	Set TrxOrg
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET VAF_OrgTrx_ID=(SELECT o.VAF_Org_ID FROM VAF_Org o"
                + " WHERE o.Value=i.OrgValue AND o.IsSummary='N' AND i.VAF_Client_ID=o.VAF_Client_ID) "
                + "WHERE VAF_OrgTrx_ID IS NULL AND OrgTrxValue IS NOT NULL"
                + " AND (VAB_Acct_ValidParameter_ID IS NULL OR VAB_Acct_ValidParameter_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set OrgTrx from Value=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid OrgTrx, '"
                + "WHERE VAF_OrgTrx_ID IS NULL AND OrgTrxValue IS NOT NULL"
                + " AND (VAB_Acct_ValidParameter_ID IS NULL OR VAB_Acct_ValidParameter_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid OrgTrx=" + no);


            //	Source Amounts
            sql = new StringBuilder("UPDATE I_GLJournal "
                + "SET AmtSourceDr = 0 "
                + "WHERE AmtSourceDr IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set 0 Source Dr=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal "
                + "SET AmtSourceCr = 0 "
                + "WHERE AmtSourceCr IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set 0 Source Cr=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_ErrorMsg=" + ts + "||'WARN=Zero Source Balance, ' "
                + "WHERE (AmtSourceDr-AmtSourceCr)=0"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Zero Source Balance=" + no);

            //	Accounted Amounts (Only if No Error)
            sql = new StringBuilder("UPDATE I_GLJournal "
                + "SET AmtAcctDr = ROUND(AmtSourceDr * CurrencyRate, 2) "	//	HARDCODED rounding
                + "WHERE AmtAcctDr IS NULL OR AmtAcctDr=0"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Calculate Acct Dr=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal "
                + "SET AmtAcctCr = ROUND(AmtSourceCr * CurrencyRate, 2) "
                + "WHERE AmtAcctCr IS NULL OR AmtAcctCr=0"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Calculate Acct Cr=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_ErrorMsg=" + ts + "||'WARN=Zero Acct Balance, ' "
                + "WHERE (AmtSourceDr-AmtSourceCr)<>0 AND (AmtAcctDr-AmtAcctCr)=0"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Zero Acct Balance=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_ErrorMsg=" + ts + "||'WARN=Check Acct Balance, ' "
                + "WHERE ABS(AmtAcctDr-AmtAcctCr)>100000000"	//	100 mio
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Chack Acct Balance=" + no);


            /*********************************************************************/

            //	Get Balance
            sql = new StringBuilder("SELECT SUM(AmtSourceDr)-SUM(AmtSourceCr), SUM(AmtAcctDr)-SUM(AmtAcctCr) "
                + "FROM I_GLJournal "
                + "WHERE I_IsImported='N'").Append(clientCheck);
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement (sql.ToString(), get_TrxName());
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                if (idr.Read())
                {
                    Decimal? source = Utility.Util.GetValueOfDecimal(idr[0]);// rs.getBigDecimal(1);
                    Decimal? acct = Utility.Util.GetValueOfDecimal(idr[1]);// rs.getBigDecimal(2);
                    if ( Env.Signum(source.Value) == 0
                        && acct != null && Env.Signum(acct.Value) == 0)
                        log.Info("Import Balance = 0");
                    else
                        log.Warning("Balance Source=" + source.Value + ", Acct=" + acct.Value);
                    if (source != null)
                        AddLog(0, null, source, "@AmtSourceDr@ - @AmtSourceCr@");
                    if (acct != null)
                        AddLog(0, null, acct, "@AmtAcctDr@ - @AmtAcctCr@");
                }
                idr.Close();
            }
            catch (Exception ex)
            {
                if (idr != null)
                { idr.Close(); }
                log.Log(Level.SEVERE, sql.ToString(), ex);
            }

            //	Count Errors
            int errors = DataBase.DB.GetSQLValue(Get_TrxName(),
                "SELECT COUNT(*) FROM I_GLJournal WHERE I_IsImported NOT IN ('Y','N')" + clientCheck);

            if (errors != 0)
            {
                if (_IsValidateOnly || _IsImportOnlyNoErrors)
                    throw new Exception("@Errors@=" + errors);
            }
            else if (_IsValidateOnly)
                return "@Errors@=" + errors;

            log.Info("Validation Errors=" + errors);
            Commit();

            /*********************************************************************/

            int noInsert = 0;
            int noInsertJournal = 0;
            int noInsertLine = 0;

            MVAGLBatchJRNL batch = null;		//	Change Batch per Batch DocumentNo
            String BatchDocumentNo = "";
            MVAGLJRNL journal = null;
            String JournalDocumentNo = "";
            DateTime? DateAcct = null;

            //	Go through Journal Records
            sql = new StringBuilder("SELECT * FROM I_GLJournal "
                + "WHERE I_IsImported='N'").Append(clientCheck)
                .Append(" ORDER BY COALESCE(BatchDocumentNo, TO_NCHAR(I_GLJournal_ID)),	COALESCE(JournalDocumentNo, TO_NCHAR(I_GLJournal_ID)), VAB_AccountBook_ID, PostingType, VAB_DocTypes_ID, VAGL_Group_ID, VAB_Currency_ID, TRUNC(DateAcct,'DD'), Line, I_GLJournal_ID");
            try
            {
                //pstmt = DataBase.prepareStatement (sql.ToString (), get_TrxName());
                //ResultSet rs = pstmt.executeQuery ();
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                while (idr.Read())
                {
                    X_VAI_GLJRNL imp = new X_VAI_GLJRNL(GetCtx(), idr, Get_TrxName());

                    //	New Batch if Batch Document No changes
                    String impBatchDocumentNo = imp.GetBatchDocumentNo();
                    if (impBatchDocumentNo == null)
                        impBatchDocumentNo = "";
                    if (batch == null
                        || imp.IsCreateNewBatch()
                        || journal.GetVAB_AccountBook_ID() != imp.GetVAB_AccountBook_ID()
                        || !BatchDocumentNo.Equals(impBatchDocumentNo))
                    {
                        BatchDocumentNo = impBatchDocumentNo;	//	cannot compare real DocumentNo
                        batch = new MVAGLBatchJRNL(GetCtx(), 0, null);
                        batch.SetClientOrg(imp.GetVAF_Client_ID(), imp.GetVAF_OrgDoc_ID());
                        if (imp.GetBatchDocumentNo() != null
                            && imp.GetBatchDocumentNo().Length > 0)
                            batch.SetDocumentNo(imp.GetBatchDocumentNo());
                        batch.SetVAB_DocTypes_ID(imp.GetVAB_DocTypes_ID());
                        batch.SetPostingType(imp.GetPostingType());
                        String description = imp.GetBatchDescription();
                        if (description == null || description.Length == 0)
                            description = "*Import-";
                        else
                            description += " *Import-";
                        description += DateTime.Now; //new Timestamp(System.currentTimeMillis());
                        batch.SetDescription(description);
                        if (!batch.Save())
                        {
                            log.Log(Level.SEVERE, "Batch not saved");
                            Exception ex = VLogger.RetrieveException();
                            if (ex != null)
                            {
                                AddLog(0, null, null, ex.Message);
                                if (idr != null)
                                {
                                    idr.Close();
                                }
                                throw ex;
                            }
                            break;
                        }
                        noInsert++;
                        journal = null;
                    }
                    //	Journal
                    String impJournalDocumentNo = imp.GetJournalDocumentNo();
                    if (impJournalDocumentNo == null)
                        impJournalDocumentNo = "";
                    DateTime? impDateAcct = TimeUtil.GetDay(imp.GetDateAcct());
                    if (journal == null
                        || imp.IsCreateNewJournal()
                        || !JournalDocumentNo.Equals(impJournalDocumentNo)
                        || journal.GetVAB_DocTypes_ID() != imp.GetVAB_DocTypes_ID()
                        || journal.GetVAGL_Group_ID() != imp.GetVAGL_Group_ID()
                        || !journal.GetPostingType().Equals(imp.GetPostingType())
                        || journal.GetVAB_Currency_ID() != imp.GetVAB_Currency_ID()
                        || !impDateAcct.Equals(DateAcct)
                    )
                    {
                        JournalDocumentNo = impJournalDocumentNo;	//	cannot compare real DocumentNo
                        DateAcct = impDateAcct.Value;
                        journal = new MVAGLJRNL(GetCtx(), 0, Get_TrxName());
                        journal.SetVAGL_BatchJRNL_ID(batch.GetVAGL_BatchJRNL_ID());
                        journal.SetClientOrg(imp.GetVAF_Client_ID(), imp.GetVAF_OrgDoc_ID());
                        //
                        String description = imp.GetBatchDescription();
                        if (description == null || description.Length == 0)
                            description = "(Import)";
                        journal.SetDescription(description);
                        if (imp.GetJournalDocumentNo() != null && imp.GetJournalDocumentNo().Length > 0)
                            journal.SetDocumentNo(imp.GetJournalDocumentNo());
                        //
                        journal.SetVAB_AccountBook_ID(imp.GetVAB_AccountBook_ID());
                        journal.SetVAB_DocTypes_ID(imp.GetVAB_DocTypes_ID());
                        journal.SetVAGL_Group_ID(imp.GetVAGL_Group_ID());
                        journal.SetPostingType(imp.GetPostingType());
                        journal.SetVAGL_Budget_ID(imp.GetVAGL_Budget_ID());
                        //
                        journal.SetCurrency(imp.GetVAB_Currency_ID(), imp.GetVAB_CurrencyType_ID(), imp.GetCurrencyRate());
                        //
                        journal.SetVAB_YearPeriod_ID(imp.GetVAB_YearPeriod_ID());
                        journal.SetDateAcct(imp.GetDateAcct());		//	sets Period if not defined
                        journal.SetDateDoc(imp.GetDateAcct());
                        //
                        if (!journal.Save())
                        {
                            log.Log(Level.SEVERE, "Journal not saved");
                            Exception ex = VLogger.RetrieveException();
                            if (ex != null)
                            {
                                AddLog(0, null, null, ex.Message);
                                if (idr != null)
                                {
                                    idr.Close();
                                }
                                throw ex;
                            }
                            break;
                        }
                        noInsertJournal++;
                    }

                    //	Lines
                    MVAGLJRNLLine line = new MVAGLJRNLLine(journal);
                    //
                    line.SetDescription(imp.GetDescription());
                    line.SetCurrency(imp.GetVAB_Currency_ID(), imp.GetVAB_CurrencyType_ID(), imp.GetCurrencyRate());
                    //	Set/Get Account Combination
                    if (imp.GetVAB_Acct_ValidParameter_ID() == 0)
                    {
                        MVABAccount acct = MVABAccount.Get(GetCtx(), imp.GetVAF_Client_ID(), imp.GetVAF_Org_ID(),
                            imp.GetVAB_AccountBook_ID(), imp.GetAccount_ID(), 0,
                            imp.GetVAM_Product_ID(), imp.GetVAB_BusinessPartner_ID(), imp.GetVAF_OrgTrx_ID(),
                            imp.GetC_LocFrom_ID(), imp.GetC_LocTo_ID(), imp.GetVAB_SalesRegionState_ID(),
                            imp.GetVAB_Project_ID(), imp.GetVAB_Promotion_ID(), imp.GetVAB_BillingCode_ID(),
                            imp.GetUser1_ID(), imp.GetUser2_ID(), 0, 0);
                        if (acct != null && acct.Get_ID() == 0)
                            acct.Save();
                        if (acct == null || acct.Get_ID() == 0)
                        {
                            imp.SetI_ErrorMsg("ERROR creating Account");
                            imp.SetI_IsImported(X_VAI_GLJRNL.I_ISIMPORTED_No);
                            imp.Save();
                            continue;
                        }
                        else
                        {
                            line.SetVAB_Acct_ValidParameter_ID(acct.Get_ID());
                            imp.SetVAB_Acct_ValidParameter_ID(acct.Get_ID());
                        }
                    }
                    else
                        line.SetVAB_Acct_ValidParameter_ID(imp.GetVAB_Acct_ValidParameter_ID());
                    //
                    line.SetLine(imp.GetLine());
                    line.SetAmtSourceCr(imp.GetAmtSourceCr());
                    line.SetAmtSourceDr(imp.GetAmtSourceDr());
                    line.SetAmtAcct(imp.GetAmtAcctDr(), imp.GetAmtAcctCr());	//	only if not 0
                    line.SetDateAcct(imp.GetDateAcct());
                    //
                    line.SetVAB_UOM_ID(imp.GetVAB_UOM_ID());
                    line.SetQty(imp.GetQty());
                    //
                    if (line.Save())
                    {
                        imp.SetVAGL_BatchJRNL_ID(batch.GetVAGL_BatchJRNL_ID());
                        imp.SetVAGL_JRNL_ID(journal.GetVAGL_JRNL_ID());
                        imp.SetVAGL_JRNLLine_ID(line.GetVAGL_JRNLLine_ID());
                        imp.SetI_IsImported(X_VAI_GLJRNL.I_ISIMPORTED_Yes);
                        imp.SetProcessed(true);
                        if (imp.Save())
                            noInsertLine++;
                    }
                }	//	while records
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "", e);
            }
            //	clean up

            //	Set Error to indicator to not imported
            sql = new StringBuilder("UPDATE I_GLJournal "
                + "SET I_IsImported='N', Updated=SysDate "
                + "WHERE I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, Utility.Util.GetValueOfDecimal(no), "@Errors@");
            //
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsert), "@VAGL_BatchJRNL_ID@: @Inserted@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsertJournal), "@VAGL_JRNL_ID@: @Inserted@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsertLine), "@VAGL_JRNLLine_ID@: @Inserted@");
            return "";
        }	//	doIt

    }
}

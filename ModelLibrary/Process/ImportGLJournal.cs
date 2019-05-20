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
        private int _AD_Client_ID = 0;
        /**	Organization to be imported to	*/
        private int _AD_Org_ID = 0;
        /**	Acct Schema to be imported to	*/
        private int _C_AcctSchema_ID = 0;
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
                else if (name.Equals("AD_Client_ID"))
                    _AD_Client_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("AD_Org_ID"))
                    _AD_Org_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("C_AcctSchema_ID"))
                    _C_AcctSchema_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
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
            String clientCheck = " AND AD_Client_ID=" + _AD_Client_ID;

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
                + "SET AD_Client_ID=(SELECT c.AD_Client_ID FROM AD_Client c WHERE c.Value=i.ClientValue) "
                + "WHERE (AD_Client_ID IS NULL OR AD_Client_ID=0) AND ClientValue IS NOT NULL"
                + " AND I_IsImported<>'Y'");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Client from Value=" + no);

            //	Set Default Client, Doc Org, AcctSchema, DatAcct
            sql = new StringBuilder("UPDATE I_GLJournal "
                  + "SET AD_Client_ID = COALESCE (AD_Client_ID,").Append(_AD_Client_ID).Append("),"
                  + " AD_OrgDoc_ID = COALESCE (AD_OrgDoc_ID,").Append(_AD_Org_ID).Append("),");
            if (_C_AcctSchema_ID != 0)
                sql.Append(" C_AcctSchema_ID = COALESCE (C_AcctSchema_ID,").Append(_C_AcctSchema_ID).Append("),");
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
                + "WHERE (AD_OrgDoc_ID IS NULL OR AD_OrgDoc_ID=0"
                + " OR EXISTS (SELECT * FROM AD_Org oo WHERE o.AD_Org_ID=oo.AD_Org_ID AND (oo.IsSummary='Y' OR oo.IsActive='N')))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Doc Org=" + no);

            //	Set AcctSchema
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET C_AcctSchema_ID=(SELECT a.C_AcctSchema_ID FROM C_AcctSchema a"
                + " WHERE i.AcctSchemaName=a.Name AND i.AD_Client_ID=a.AD_Client_ID) "
                + "WHERE C_AcctSchema_ID IS NULL AND AcctSchemaName IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set AcctSchema from Name=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET C_AcctSchema_ID=(SELECT c.C_AcctSchema1_ID FROM AD_ClientInfo c WHERE c.AD_Client_ID=i.AD_Client_ID) "
                + "WHERE C_AcctSchema_ID IS NULL AND AcctSchemaName IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set AcctSchema from Client=" + no);
            //	Error AcctSchema
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid AcctSchema, '"
                + "WHERE (C_AcctSchema_ID IS NULL OR C_AcctSchema_ID=0"
                + " OR NOT EXISTS (SELECT * FROM C_AcctSchema a WHERE i.AD_Client_ID=a.AD_Client_ID))"
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
                + "SET C_DocType_ID=(SELECT d.C_DocType_ID FROM C_DocType d"
                + " WHERE d.Name=i.DocTypeName AND d.DocBaseType='GLJ' AND i.AD_Client_ID=d.AD_Client_ID) "
                + "WHERE C_DocType_ID IS NULL AND DocTypeName IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set DocType=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid DocType, '"
                + "WHERE (C_DocType_ID IS NULL OR C_DocType_ID=0"
                + " OR NOT EXISTS (SELECT * FROM C_DocType d WHERE i.AD_Client_ID=d.AD_Client_ID AND d.DocBaseType='GLJ'))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid DocType=" + no);

            //	GL Category
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET GL_Category_ID=(SELECT c.GL_Category_ID FROM GL_Category c"
                + " WHERE c.Name=i.CategoryName AND i.AD_Client_ID=c.AD_Client_ID) "
                + "WHERE GL_Category_ID IS NULL AND CategoryName IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set DocType=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Category, '"
                + "WHERE (GL_Category_ID IS NULL OR GL_Category_ID=0)"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Category=" + no);

            //	Set Currency
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET C_Currency_ID=(SELECT c.C_Currency_ID FROM C_Currency c"
                + " WHERE c.ISO_Code=i.ISO_Code AND c.AD_Client_ID IN (0,i.AD_Client_ID)) "
                + "WHERE C_Currency_ID IS NULL AND ISO_Code IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Currency from ISO=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET C_Currency_ID=(SELECT a.C_Currency_ID FROM C_AcctSchema a"
                + " WHERE a.C_AcctSchema_ID=i.C_AcctSchema_ID AND a.AD_Client_ID=i.AD_Client_ID)"
                + "WHERE C_Currency_ID IS NULL AND ISO_Code IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Default Currency=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Currency, '"
                + "WHERE (C_Currency_ID IS NULL OR C_Currency_ID=0)"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Currency=" + no);

            //	Set Conversion Type
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET ConversionTypeValue='S' "
                + "WHERE C_ConversionType_ID IS NULL AND ConversionTypeValue IS NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set CurrencyType Value to Spot =" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET C_ConversionType_ID=(SELECT c.C_ConversionType_ID FROM C_ConversionType c"
                + " WHERE c.Value=i.ConversionTypeValue AND c.AD_Client_ID IN (0,i.AD_Client_ID)) "
                + "WHERE C_ConversionType_ID IS NULL AND ConversionTypeValue IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set CurrencyType from Value=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid CurrencyType, '"
                + "WHERE (C_ConversionType_ID IS NULL OR C_ConversionType_ID=0) AND ConversionTypeValue IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid CurrencyTypeValue=" + no);


            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No ConversionType, '"
                + "WHERE (C_ConversionType_ID IS NULL OR C_ConversionType_ID=0)"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No CourrencyType=" + no);

            //	Set/Overwrite Home Currency Rate
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET CurrencyRate=1"
                + "WHERE EXISTS (SELECT * FROM C_AcctSchema a"
                + " WHERE a.C_AcctSchema_ID=i.C_AcctSchema_ID AND a.C_Currency_ID=i.C_Currency_ID)"
                + " AND C_Currency_ID IS NOT NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Home CurrencyRate=" + no);
            //	Set Currency Rate
            sql = new StringBuilder("UPDATE I_GLJournal i "
               + "SET CurrencyRate=(SELECT MAX(r.MultiplyRate) FROM C_Conversion_Rate r, C_AcctSchema s"
               + " WHERE s.C_AcctSchema_ID=i.C_AcctSchema_ID AND s.AD_Client_ID=i.AD_Client_ID"
               + " AND r.C_Currency_ID=i.C_Currency_ID AND r.C_Currency_ID_TO=s.C_Currency_ID"
               + " AND r.AD_Client_ID=i.AD_Client_ID AND r.AD_Org_ID=i.AD_OrgDoc_ID"
               + " AND r.C_ConversionType_ID=i.C_ConversionType_ID"
               + " AND i.DateAcct BETWEEN r.ValidFrom AND r.ValidTo "
                //	ORDER BY ValidFrom DESC
               + ") WHERE CurrencyRate IS NULL OR CurrencyRate=0 AND C_Currency_ID>0"
               + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Org Rate=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET CurrencyRate=(SELECT MAX(r.MultiplyRate) FROM C_Conversion_Rate r, C_AcctSchema s"
                + " WHERE s.C_AcctSchema_ID=i.C_AcctSchema_ID AND s.AD_Client_ID=i.AD_Client_ID"
                + " AND r.C_Currency_ID=i.C_Currency_ID AND r.C_Currency_ID_TO=s.C_Currency_ID"
                + " AND r.AD_Client_ID=i.AD_Client_ID"
                + " AND r.C_ConversionType_ID=i.C_ConversionType_ID"
                + " AND i.DateAcct BETWEEN r.ValidFrom AND r.ValidTo "
                //	ORDER BY ValidFrom DESC
                + ") WHERE CurrencyRate IS NULL OR CurrencyRate=0 AND C_Currency_ID>0"
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
                + "SET C_Period_ID=(SELECT MAX(p.C_Period_ID) FROM C_Period p"
                + " INNER JOIN C_Year y ON (y.C_Year_ID=p.C_Year_ID)"
                + " INNER JOIN AD_ClientInfo c ON (c.C_Calendar_ID=y.C_Calendar_ID)"
                + " WHERE c.AD_Client_ID=i.AD_Client_ID"
                + " AND i.DateAcct BETWEEN p.StartDate AND p.EndDate AND p.PeriodType='S') "
                + "WHERE C_Period_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Period=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Period, '"
                + "WHERE C_Period_ID IS NULL OR C_Period_ID NOt IN "
                + "(SELECT C_Period_ID FROM C_Period p"
                + " INNER JOIN C_Year y ON (y.C_Year_ID=p.C_Year_ID)"
                + " INNER JOIN AD_ClientInfo c ON (c.C_Calendar_ID=y.C_Calendar_ID) "
                + " WHERE c.AD_Client_ID=i.AD_Client_ID"
                + " AND i.DateAcct BETWEEN p.StartDate AND p.EndDate AND p.PeriodType='S')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Period=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_ErrorMsg=" + ts + "||'WARN=Period Closed, ' "
                + "WHERE C_Period_ID IS NOT NULL AND NOT EXISTS"
                + " (SELECT * FROM C_PeriodControl pc WHERE pc.C_Period_ID=i.C_Period_ID AND DocBaseType='GLJ' AND PeriodStatus='O') "
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
                + " (SELECT * FROM AD_Ref_List r WHERE r.AD_Reference_ID=125 AND i.PostingType=r.Value)"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid PostingTypee=" + no);


            //	** Account Elements (optional) **
            //	(C_ValidCombination_ID IS NULL OR C_ValidCombination_ID=0)

            //	Set Org from Name (* is overwritten and default)
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET AD_Org_ID=(SELECT o.AD_Org_ID FROM AD_Org o"
                + " WHERE o.Value=i.OrgValue AND o.IsSummary='N' AND i.AD_Client_ID=o.AD_Client_ID) "
                + "WHERE (AD_Org_ID IS NULL OR AD_Org_ID=0) AND OrgValue IS NOT NULL"
                + " AND (C_ValidCombination_ID IS NULL OR C_ValidCombination_ID=0) AND I_IsImported<>'Y'");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Org from Value=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET AD_Org_ID=AD_OrgDoc_ID "
                + "WHERE (AD_Org_ID IS NULL OR AD_Org_ID=0) AND OrgValue IS NULL AND AD_OrgDoc_ID IS NOT NULL AND AD_OrgDoc_ID<>0"
                + " AND (C_ValidCombination_ID IS NULL OR C_ValidCombination_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Org from Doc Org=" + no);
            //	Error Org
            sql = new StringBuilder("UPDATE I_GLJournal o "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Org, '"
                + "WHERE (AD_Org_ID IS NULL OR AD_Org_ID=0"
                + " OR EXISTS (SELECT * FROM AD_Org oo WHERE o.AD_Org_ID=oo.AD_Org_ID AND (oo.IsSummary='Y' OR oo.IsActive='N')))"
                + " AND (C_ValidCombination_ID IS NULL OR C_ValidCombination_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Org=" + no);

            //	Set Account
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET Account_ID=(SELECT MAX(ev.C_ElementValue_ID) FROM C_ElementValue ev"
                + " INNER JOIN C_Element e ON (e.C_Element_ID=ev.C_Element_ID)"
                + " INNER JOIN C_AcctSchema_Element ase ON (e.C_Element_ID=ase.C_Element_ID AND ase.ElementType='AC')"
                + " WHERE ev.Value=i.AccountValue AND ev.IsSummary='N'"
                + " AND i.C_AcctSchema_ID=ase.C_AcctSchema_ID AND i.AD_Client_ID=ev.AD_Client_ID) "
                + "WHERE Account_ID IS NULL AND AccountValue IS NOT NULL"
                + " AND (C_ValidCombination_ID IS NULL OR C_ValidCombination_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Account from Value=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Account, '"
                + "WHERE (Account_ID IS NULL OR Account_ID=0)"
                + " AND (C_ValidCombination_ID IS NULL OR C_ValidCombination_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Account=" + no);

            //	Set BPartner
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET C_BPartner_ID=(SELECT bp.C_BPartner_ID FROM C_BPartner bp"
                + " WHERE bp.Value=i.BPartnerValue AND bp.IsSummary='N' AND i.AD_Client_ID=bp.AD_Client_ID) "
                + "WHERE C_BPartner_ID IS NULL AND BPartnerValue IS NOT NULL"
                + " AND (C_ValidCombination_ID IS NULL OR C_ValidCombination_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set BPartner from Value=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid BPartner, '"
                + "WHERE C_BPartner_ID IS NULL AND BPartnerValue IS NOT NULL"
                + " AND (C_ValidCombination_ID IS NULL OR C_ValidCombination_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid BPartner=" + no);

            //	Set Product
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET M_Product_ID=(SELECT MAX(p.M_Product_ID) FROM M_Product p"
                + " WHERE (p.Value=i.ProductValue OR p.UPC=i.UPC OR p.SKU=i.SKU)"
                + " AND p.IsSummary='N' AND i.AD_Client_ID=p.AD_Client_ID) "
                + "WHERE M_Product_ID IS NULL AND (ProductValue IS NOT NULL OR UPC IS NOT NULL OR SKU IS NOT NULL)"
                + " AND (C_ValidCombination_ID IS NULL OR C_ValidCombination_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Product from Value=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Product, '"
                + "WHERE M_Product_ID IS NULL AND (ProductValue IS NOT NULL OR UPC IS NOT NULL OR SKU IS NOT NULL)"
                + " AND (C_ValidCombination_ID IS NULL OR C_ValidCombination_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Product=" + no);

            //	Set Project
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET C_Project_ID=(SELECT p.C_Project_ID FROM C_Project p"
                + " WHERE p.Value=i.ProjectValue AND p.IsSummary='N' AND i.AD_Client_ID=p.AD_Client_ID) "
                + "WHERE C_Project_ID IS NULL AND ProjectValue IS NOT NULL"
                + " AND (C_ValidCombination_ID IS NULL OR C_ValidCombination_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Project from Value=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Project, '"
                + "WHERE C_Project_ID IS NULL AND ProjectValue IS NOT NULL"
                + " AND (C_ValidCombination_ID IS NULL OR C_ValidCombination_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Project=" + no);

            //	Set TrxOrg
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET AD_OrgTrx_ID=(SELECT o.AD_Org_ID FROM AD_Org o"
                + " WHERE o.Value=i.OrgValue AND o.IsSummary='N' AND i.AD_Client_ID=o.AD_Client_ID) "
                + "WHERE AD_OrgTrx_ID IS NULL AND OrgTrxValue IS NOT NULL"
                + " AND (C_ValidCombination_ID IS NULL OR C_ValidCombination_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set OrgTrx from Value=" + no);
            sql = new StringBuilder("UPDATE I_GLJournal i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid OrgTrx, '"
                + "WHERE AD_OrgTrx_ID IS NULL AND OrgTrxValue IS NOT NULL"
                + " AND (C_ValidCombination_ID IS NULL OR C_ValidCombination_ID=0) AND I_IsImported<>'Y'").Append(clientCheck);
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

            MJournalBatch batch = null;		//	Change Batch per Batch DocumentNo
            String BatchDocumentNo = "";
            MJournal journal = null;
            String JournalDocumentNo = "";
            DateTime? DateAcct = null;

            //	Go through Journal Records
            sql = new StringBuilder("SELECT * FROM I_GLJournal "
                + "WHERE I_IsImported='N'").Append(clientCheck)
                .Append(" ORDER BY COALESCE(BatchDocumentNo, TO_NCHAR(I_GLJournal_ID)),	COALESCE(JournalDocumentNo, TO_NCHAR(I_GLJournal_ID)), C_AcctSchema_ID, PostingType, C_DocType_ID, GL_Category_ID, C_Currency_ID, TRUNC(DateAcct,'DD'), Line, I_GLJournal_ID");
            try
            {
                //pstmt = DataBase.prepareStatement (sql.ToString (), get_TrxName());
                //ResultSet rs = pstmt.executeQuery ();
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                while (idr.Read())
                {
                    X_I_GLJournal imp = new X_I_GLJournal(GetCtx(), idr, Get_TrxName());

                    //	New Batch if Batch Document No changes
                    String impBatchDocumentNo = imp.GetBatchDocumentNo();
                    if (impBatchDocumentNo == null)
                        impBatchDocumentNo = "";
                    if (batch == null
                        || imp.IsCreateNewBatch()
                        || journal.GetC_AcctSchema_ID() != imp.GetC_AcctSchema_ID()
                        || !BatchDocumentNo.Equals(impBatchDocumentNo))
                    {
                        BatchDocumentNo = impBatchDocumentNo;	//	cannot compare real DocumentNo
                        batch = new MJournalBatch(GetCtx(), 0, null);
                        batch.SetClientOrg(imp.GetAD_Client_ID(), imp.GetAD_OrgDoc_ID());
                        if (imp.GetBatchDocumentNo() != null
                            && imp.GetBatchDocumentNo().Length > 0)
                            batch.SetDocumentNo(imp.GetBatchDocumentNo());
                        batch.SetC_DocType_ID(imp.GetC_DocType_ID());
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
                        || journal.GetC_DocType_ID() != imp.GetC_DocType_ID()
                        || journal.GetGL_Category_ID() != imp.GetGL_Category_ID()
                        || !journal.GetPostingType().Equals(imp.GetPostingType())
                        || journal.GetC_Currency_ID() != imp.GetC_Currency_ID()
                        || !impDateAcct.Equals(DateAcct)
                    )
                    {
                        JournalDocumentNo = impJournalDocumentNo;	//	cannot compare real DocumentNo
                        DateAcct = impDateAcct.Value;
                        journal = new MJournal(GetCtx(), 0, Get_TrxName());
                        journal.SetGL_JournalBatch_ID(batch.GetGL_JournalBatch_ID());
                        journal.SetClientOrg(imp.GetAD_Client_ID(), imp.GetAD_OrgDoc_ID());
                        //
                        String description = imp.GetBatchDescription();
                        if (description == null || description.Length == 0)
                            description = "(Import)";
                        journal.SetDescription(description);
                        if (imp.GetJournalDocumentNo() != null && imp.GetJournalDocumentNo().Length > 0)
                            journal.SetDocumentNo(imp.GetJournalDocumentNo());
                        //
                        journal.SetC_AcctSchema_ID(imp.GetC_AcctSchema_ID());
                        journal.SetC_DocType_ID(imp.GetC_DocType_ID());
                        journal.SetGL_Category_ID(imp.GetGL_Category_ID());
                        journal.SetPostingType(imp.GetPostingType());
                        journal.SetGL_Budget_ID(imp.GetGL_Budget_ID());
                        //
                        journal.SetCurrency(imp.GetC_Currency_ID(), imp.GetC_ConversionType_ID(), imp.GetCurrencyRate());
                        //
                        journal.SetC_Period_ID(imp.GetC_Period_ID());
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
                    MJournalLine line = new MJournalLine(journal);
                    //
                    line.SetDescription(imp.GetDescription());
                    line.SetCurrency(imp.GetC_Currency_ID(), imp.GetC_ConversionType_ID(), imp.GetCurrencyRate());
                    //	Set/Get Account Combination
                    if (imp.GetC_ValidCombination_ID() == 0)
                    {
                        MAccount acct = MAccount.Get(GetCtx(), imp.GetAD_Client_ID(), imp.GetAD_Org_ID(),
                            imp.GetC_AcctSchema_ID(), imp.GetAccount_ID(), 0,
                            imp.GetM_Product_ID(), imp.GetC_BPartner_ID(), imp.GetAD_OrgTrx_ID(),
                            imp.GetC_LocFrom_ID(), imp.GetC_LocTo_ID(), imp.GetC_SalesRegion_ID(),
                            imp.GetC_Project_ID(), imp.GetC_Campaign_ID(), imp.GetC_Activity_ID(),
                            imp.GetUser1_ID(), imp.GetUser2_ID(), 0, 0);
                        if (acct != null && acct.Get_ID() == 0)
                            acct.Save();
                        if (acct == null || acct.Get_ID() == 0)
                        {
                            imp.SetI_ErrorMsg("ERROR creating Account");
                            imp.SetI_IsImported(X_I_GLJournal.I_ISIMPORTED_No);
                            imp.Save();
                            continue;
                        }
                        else
                        {
                            line.SetC_ValidCombination_ID(acct.Get_ID());
                            imp.SetC_ValidCombination_ID(acct.Get_ID());
                        }
                    }
                    else
                        line.SetC_ValidCombination_ID(imp.GetC_ValidCombination_ID());
                    //
                    line.SetLine(imp.GetLine());
                    line.SetAmtSourceCr(imp.GetAmtSourceCr());
                    line.SetAmtSourceDr(imp.GetAmtSourceDr());
                    line.SetAmtAcct(imp.GetAmtAcctDr(), imp.GetAmtAcctCr());	//	only if not 0
                    line.SetDateAcct(imp.GetDateAcct());
                    //
                    line.SetC_UOM_ID(imp.GetC_UOM_ID());
                    line.SetQty(imp.GetQty());
                    //
                    if (line.Save())
                    {
                        imp.SetGL_JournalBatch_ID(batch.GetGL_JournalBatch_ID());
                        imp.SetGL_Journal_ID(journal.GetGL_Journal_ID());
                        imp.SetGL_JournalLine_ID(line.GetGL_JournalLine_ID());
                        imp.SetI_IsImported(X_I_GLJournal.I_ISIMPORTED_Yes);
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
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsert), "@GL_JournalBatch_ID@: @Inserted@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsertJournal), "@GL_Journal_ID@: @Inserted@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsertLine), "@GL_JournalLine_ID@: @Inserted@");
            return "";
        }	//	doIt

    }
}

/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ImportOrder
 * Purpose        : Import Order from I_Order
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
    public class ImportOrder : ProcessEngine.SvrProcess
    {
        /**	Client to be imported to		*/
        private int _AD_Client_ID = 0;
        /**	Organization to be imported to		*/
        private int _AD_Org_ID = 0;
        /**	Delete old Imported				*/
        private bool _deleteOldImported = false;
        /**	Document Action					*/
        private String _docAction = MOrder.DOCACTION_Prepare;


        /** Effective						*/
        private DateTime? _DateValue = null;

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (name.Equals("AD_Client_ID"))
                    _AD_Client_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("AD_Org_ID"))
                    _AD_Org_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("DeleteOldImported"))
                    _deleteOldImported = "Y".Equals(para[i].GetParameter());
                else if (name.Equals("DocAction"))
                    _docAction = (String)para[i].GetParameter();
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            if (_DateValue == null)
                _DateValue = DateTime.Now;// new Timestamp (System.currentTimeMillis());
        }	//	prepare


        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            StringBuilder sql = null;
            int no = 0;
            String clientCheck = " AND AD_Client_ID=" + _AD_Client_ID;

            //	****	Prepare	****

            //	Delete Old Imported
            if (_deleteOldImported)
            {
                sql = new StringBuilder("DELETE FROM I_Order "
                      + "WHERE I_IsImported='Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Delete Old Impored =" + no);
            }

            //	Set Client, Org, IsActive, Created/Updated
            sql = new StringBuilder("UPDATE I_Order "
                  + "SET AD_Client_ID = COALESCE (AD_Client_ID,").Append(_AD_Client_ID).Append("),"
                  + " AD_Org_ID = COALESCE (AD_Org_ID,").Append(_AD_Org_ID).Append("),"
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
            sql = new StringBuilder("UPDATE I_Order o "
                //jz	+ "SET I_IsImported='E', I_ErrorMsg=I_ErrorMsg||'ERR=Invalid Org, '"
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Org, '"
                + "WHERE (AD_Org_ID IS NULL OR AD_Org_ID=0"
                + " OR EXISTS (SELECT * FROM AD_Org oo WHERE o.AD_Org_ID=oo.AD_Org_ID AND (oo.IsSummary='Y' OR oo.IsActive='N')))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Org=" + no);

            //	Document Type - PO - SO
            sql = new StringBuilder("UPDATE I_Order o "	//	PO Document Type Name
                  + "SET C_DocType_ID=(SELECT C_DocType_ID FROM C_DocType d WHERE d.Name=o.DocTypeName"
                  + " AND d.DocBaseType='POO' AND o.AD_Client_ID=d.AD_Client_ID) "
                  + "WHERE C_DocType_ID IS NULL AND IsSOTrx='N' AND DocTypeName IS NOT NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set PO DocType=" + no);
            sql = new StringBuilder("UPDATE I_Order o "	//	SO Document Type Name
                  + "SET C_DocType_ID=(SELECT C_DocType_ID FROM C_DocType d WHERE d.Name=o.DocTypeName"
                  + " AND d.DocBaseType='SOO' AND o.AD_Client_ID=d.AD_Client_ID) "
                  + "WHERE C_DocType_ID IS NULL AND IsSOTrx='Y' AND DocTypeName IS NOT NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set SO DocType=" + no);
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET C_DocType_ID=(SELECT C_DocType_ID FROM C_DocType d WHERE d.Name=o.DocTypeName"
                  + " AND d.DocBaseType IN ('SOO','POO') AND o.AD_Client_ID=d.AD_Client_ID) "
                //+ "WHERE C_DocType_ID IS NULL AND IsSOTrx IS NULL AND DocTypeName IS NOT NULL AND I_IsImported<>'Y'").Append (clientCheck);
                  + "WHERE C_DocType_ID IS NULL AND DocTypeName IS NOT NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set DocType=" + no);
            sql = new StringBuilder("UPDATE I_Order "	//	Error Invalid Doc Type Name
                  + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid DocTypeName, ' "
                  + "WHERE C_DocType_ID IS NULL AND DocTypeName IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid DocTypeName=" + no);
            //	DocType Default
            sql = new StringBuilder("UPDATE I_Order o "	//	Default PO
                  + "SET C_DocType_ID=(SELECT MAX(C_DocType_ID) FROM C_DocType d WHERE d.IsDefault='Y'"
                  + " AND d.DocBaseType='POO' AND o.AD_Client_ID=d.AD_Client_ID) "
                  + "WHERE C_DocType_ID IS NULL AND IsSOTrx='N' AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set PO Default DocType=" + no);
            sql = new StringBuilder("UPDATE I_Order o "	//	Default SO
                  + "SET C_DocType_ID=(SELECT MAX(C_DocType_ID) FROM C_DocType d WHERE d.IsDefault='Y'"
                  + " AND d.DocBaseType='SOO' AND o.AD_Client_ID=d.AD_Client_ID) "
                  + "WHERE C_DocType_ID IS NULL AND IsSOTrx='Y' AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set SO Default DocType=" + no);
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET C_DocType_ID=(SELECT MAX(C_DocType_ID) FROM C_DocType d WHERE d.IsDefault='Y'"
                  + " AND d.DocBaseType IN('SOO','POO') AND o.AD_Client_ID=d.AD_Client_ID) "
                  + "WHERE C_DocType_ID IS NULL AND IsSOTrx IS NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Default DocType=" + no);
            sql = new StringBuilder("UPDATE I_Order "	// No DocType
                  + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No DocType, ' "
                  + "WHERE C_DocType_ID IS NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No DocType=" + no);

            //	Set IsSOTrx
            sql = new StringBuilder("UPDATE I_Order o SET IsSOTrx='Y' "
                  + "WHERE EXISTS (SELECT * FROM C_DocType d WHERE o.C_DocType_ID=d.C_DocType_ID AND d.DocBaseType='SOO' AND o.AD_Client_ID=d.AD_Client_ID)"
                  + " AND C_DocType_ID IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set IsSOTrx=Y=" + no);
            sql = new StringBuilder("UPDATE I_Order o SET IsSOTrx='N' "
                  + "WHERE EXISTS (SELECT * FROM C_DocType d WHERE o.C_DocType_ID=d.C_DocType_ID AND d.DocBaseType='POO' AND o.AD_Client_ID=d.AD_Client_ID)"
                  + " AND C_DocType_ID IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set IsSOTrx=N=" + no);

            //	Price List
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET M_PriceList_ID=(SELECT MAX(M_PriceList_ID) FROM M_PriceList p WHERE p.IsDefault='Y'"
                  + " AND p.C_Currency_ID=o.C_Currency_ID AND p.IsSOPriceList=o.IsSOTrx AND o.AD_Client_ID=p.AD_Client_ID) "
                  + "WHERE M_PriceList_ID IS NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Default Currency PriceList=" + no);
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET M_PriceList_ID=(SELECT MAX(M_PriceList_ID) FROM M_PriceList p WHERE p.IsDefault='Y'"
                  + " AND p.IsSOPriceList=o.IsSOTrx AND o.AD_Client_ID=p.AD_Client_ID) "
                  + "WHERE M_PriceList_ID IS NULL AND C_Currency_ID IS NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Default PriceList=" + no);
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET M_PriceList_ID=(SELECT MAX(M_PriceList_ID) FROM M_PriceList p "
                  + " WHERE p.C_Currency_ID=o.C_Currency_ID AND p.IsSOPriceList=o.IsSOTrx AND o.AD_Client_ID=p.AD_Client_ID) "
                  + "WHERE M_PriceList_ID IS NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Currency PriceList=" + no);
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET M_PriceList_ID=(SELECT MAX(M_PriceList_ID) FROM M_PriceList p "
                  + " WHERE p.IsSOPriceList=o.IsSOTrx AND o.AD_Client_ID=p.AD_Client_ID) "
                  + "WHERE M_PriceList_ID IS NULL AND C_Currency_ID IS NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set PriceList=" + no);
            //
            sql = new StringBuilder("UPDATE I_Order "
                  + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No PriceList, ' "
                  + "WHERE M_PriceList_ID IS NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No PriceList=" + no);

            //	Payment Rule
            //  We support Payment Rule being input in the login language 
            //Language language = Language.getLoginLanguage();		//	Base Language
            VAdvantage.Login.Language language = VAdvantage.Login.Language.GetLoginLanguage();
            String AD_Language = language.GetAD_Language();
            sql = new StringBuilder("UPDATE I_Order O " +
                    "SET PaymentRule= " +
                    "(SELECT R.value " +
                    "  FROM AD_Ref_List R " +
                    "  left outer join AD_Ref_List_Trl RT " +
                    "  on RT.AD_Ref_List_ID = R.AD_Ref_List_ID and RT.AD_Language = @param " +
                    "  WHERE R.AD_Reference_ID = 195 and coalesce( RT.Name, R.Name ) = O.PaymentRuleName ) " +
                    "WHERE PaymentRule is null AND PaymentRuleName IS NOT NULL AND I_IsImported<>'Y'").Append(clientCheck);
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@param", AD_Language);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), param, Get_TrxName());
            log.Fine("Set PaymentRule=" + no);
            // do not set a default; if null, the import logic will derive from the business partner
            // do not error in absence of a default


            //	Payment Term
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET C_PaymentTerm_ID=(SELECT C_PaymentTerm_ID FROM C_PaymentTerm p"
                  + " WHERE o.PaymentTermValue=p.Value AND o.AD_Client_ID=p.AD_Client_ID) "
                  + "WHERE C_PaymentTerm_ID IS NULL AND PaymentTermValue IS NOT NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set PaymentTerm=" + no);
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET C_PaymentTerm_ID=(SELECT MAX(C_PaymentTerm_ID) FROM C_PaymentTerm p"
                  + " WHERE p.IsDefault='Y' AND o.AD_Client_ID=p.AD_Client_ID) "
                  + "WHERE C_PaymentTerm_ID IS NULL AND o.PaymentTermValue IS NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Default PaymentTerm=" + no);
            //
            sql = new StringBuilder("UPDATE I_Order "
                  + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No PaymentTerm, ' "
                  + "WHERE C_PaymentTerm_ID IS NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No PaymentTerm=" + no);

            //	Warehouse
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET M_Warehouse_ID=(SELECT MAX(M_Warehouse_ID) FROM M_Warehouse w"
                  + " WHERE o.AD_Client_ID=w.AD_Client_ID AND o.AD_Org_ID=w.AD_Org_ID) "
                  + "WHERE M_Warehouse_ID IS NULL AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());	//	Warehouse for Org
            if (no != 0)
                log.Fine("Set Warehouse=" + no);
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET M_Warehouse_ID=(SELECT M_Warehouse_ID FROM M_Warehouse w"
                  + " WHERE o.AD_Client_ID=w.AD_Client_ID) "
                  + "WHERE M_Warehouse_ID IS NULL"
                  + " AND EXISTS (SELECT AD_Client_ID FROM M_Warehouse w WHERE w.AD_Client_ID=o.AD_Client_ID GROUP BY AD_Client_ID HAVING COUNT(*)=1)"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Fine("Set Only Client Warehouse=" + no);
            //
            sql = new StringBuilder("UPDATE I_Order "
                  + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No Warehouse, ' "
                  + "WHERE M_Warehouse_ID IS NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No Warehouse=" + no);

            //	BP from EMail
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET (C_BPartner_ID,AD_User_ID)=(SELECT C_BPartner_ID,AD_User_ID FROM AD_User u"
                  + " WHERE o.EMail=u.EMail AND o.AD_Client_ID=u.AD_Client_ID AND u.C_BPartner_ID IS NOT NULL) "
                  + "WHERE C_BPartner_ID IS NULL AND EMail IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set BP from EMail=" + no);
            //	BP from ContactName
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET (C_BPartner_ID,AD_User_ID)=(SELECT C_BPartner_ID,AD_User_ID FROM AD_User u"
                  + " WHERE o.ContactName=u.Name AND o.AD_Client_ID=u.AD_Client_ID AND u.C_BPartner_ID IS NOT NULL) "
                  + "WHERE C_BPartner_ID IS NULL AND ContactName IS NOT NULL"
                  + " AND EXISTS (SELECT Name FROM AD_User u WHERE o.ContactName=u.Name AND o.AD_Client_ID=u.AD_Client_ID AND u.C_BPartner_ID IS NOT NULL GROUP BY Name HAVING COUNT(*)=1)"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set BP from ContactName=" + no);
            //	BP from Value
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET C_BPartner_ID=(SELECT MAX(C_BPartner_ID) FROM C_BPartner bp"
                  + " WHERE o.BPartnerValue=bp.Value AND o.AD_Client_ID=bp.AD_Client_ID) "
                  + "WHERE C_BPartner_ID IS NULL AND BPartnerValue IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set BP from Value=" + no);
            //	Default BP
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET C_BPartner_ID=(SELECT C_BPartnerCashTrx_ID FROM AD_ClientInfo c"
                  + " WHERE o.AD_Client_ID=c.AD_Client_ID) "
                  + "WHERE C_BPartner_ID IS NULL AND BPartnerValue IS NULL AND Name IS NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Default BP=" + no);

            //	Existing Location ? Exact Match
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET (BillTo_ID,C_BPartner_Location_ID)=(SELECT C_BPartner_Location_ID,C_BPartner_Location_ID"
                  + " FROM C_BPartner_Location bpl INNER JOIN C_Location l ON (bpl.C_Location_ID=l.C_Location_ID)"
                  + " WHERE o.C_BPartner_ID=bpl.C_BPartner_ID AND bpl.AD_Client_ID=o.AD_Client_ID"
                  + " AND DUMP(o.Address1)=DUMP(l.Address1) AND DUMP(o.Address2)=DUMP(l.Address2)"
                  + " AND DUMP(o.City)=DUMP(l.City) AND DUMP(o.Postal)=DUMP(l.Postal)"
                  + " AND DUMP(o.C_Region_ID)=DUMP(l.C_Region_ID) AND DUMP(o.C_Country_ID)=DUMP(l.C_Country_ID)) "
                  + "WHERE C_BPartner_ID IS NOT NULL AND C_BPartner_Location_ID IS NULL"
                  + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Found Location=" + no);
            //	Set Bill Location from BPartner
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET BillTo_ID=(SELECT MAX(C_BPartner_Location_ID) FROM C_BPartner_Location l"
                  + " WHERE l.C_BPartner_ID=o.C_BPartner_ID AND o.AD_Client_ID=l.AD_Client_ID"
                  + " AND ((l.IsBillTo='Y' AND o.IsSOTrx='Y') OR (l.IsPayFrom='Y' AND o.IsSOTrx='N'))"
                  + ") "
                  + "WHERE C_BPartner_ID IS NOT NULL AND BillTo_ID IS NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set BP BillTo from BP=" + no);
            //	Set Location from BPartner
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET C_BPartner_Location_ID=(SELECT MAX(C_BPartner_Location_ID) FROM C_BPartner_Location l"
                  + " WHERE l.C_BPartner_ID=o.C_BPartner_ID AND o.AD_Client_ID=l.AD_Client_ID"
                  + " AND ((l.IsShipTo='Y' AND o.IsSOTrx='Y') OR o.IsSOTrx='N')"
                  + ") "
                  + "WHERE C_BPartner_ID IS NOT NULL AND C_BPartner_Location_ID IS NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set BP Location from BP=" + no);
            //
            sql = new StringBuilder("UPDATE I_Order "
                  + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No BP Location, ' "
                  + "WHERE C_BPartner_ID IS NOT NULL AND (BillTo_ID IS NULL OR C_BPartner_Location_ID IS NULL)"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No BP Location=" + no);

            //	Set Country
            /**
            sql = new StringBuilder ("UPDATE I_Order o "
                  + "SET CountryCode=(SELECT CountryCode FROM C_Country c WHERE c.IsDefault='Y'"
                  + " AND c.AD_Client_ID IN (0, o.AD_Client_ID) AND ROWNUM=1) "
                  + "WHERE C_BPartner_ID IS NULL AND CountryCode IS NULL AND C_Country_ID IS NULL"
                  + " AND I_IsImported<>'Y'").Append (clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(),null, Get_TrxName());
            log.Fine("Set Country Default=" + no);
            **/
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET C_Country_ID=(SELECT C_Country_ID FROM C_Country c"
                  + " WHERE o.CountryCode=c.CountryCode AND c.IsSummary='N' AND c.AD_Client_ID IN (0, o.AD_Client_ID)) "
                  + "WHERE C_BPartner_ID IS NULL AND C_Country_ID IS NULL AND CountryCode IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Country=" + no);
            //
            sql = new StringBuilder("UPDATE I_Order "
                  + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Country, ' "
                  + "WHERE C_BPartner_ID IS NULL AND C_Country_ID IS NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Country=" + no);

            //	Set Region
            sql = new StringBuilder("UPDATE I_Order o "
                  + "Set RegionName=(SELECT MAX(Name) FROM C_Region r"
                  + " WHERE r.IsDefault='Y' AND r.C_Country_ID=o.C_Country_ID"
                  + " AND r.AD_Client_ID IN (0, o.AD_Client_ID)) "
                  + "WHERE C_BPartner_ID IS NULL AND C_Region_ID IS NULL AND RegionName IS NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Region Default=" + no);
            //
            sql = new StringBuilder("UPDATE I_Order o "
                  + "Set C_Region_ID=(SELECT C_Region_ID FROM C_Region r"
                  + " WHERE r.Name=o.RegionName AND r.C_Country_ID=o.C_Country_ID"
                  + " AND r.AD_Client_ID IN (0, o.AD_Client_ID)) "
                  + "WHERE C_BPartner_ID IS NULL AND C_Region_ID IS NULL AND RegionName IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Region=" + no);
            //
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Region, ' "
                  + "WHERE C_BPartner_ID IS NULL AND C_Region_ID IS NULL "
                  + " AND EXISTS (SELECT * FROM C_Country c"
                  + " WHERE c.C_Country_ID=o.C_Country_ID AND c.HasRegion='Y')"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Region=" + no);

            //	Product
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET M_Product_ID=(SELECT MAX(M_Product_ID) FROM M_Product p"
                  + " WHERE o.ProductValue=p.Value AND o.AD_Client_ID=p.AD_Client_ID) "
                  + "WHERE M_Product_ID IS NULL AND ProductValue IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Product from Value=" + no);
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET M_Product_ID=(SELECT MAX(M_Product_ID) FROM M_Product p"
                  + " WHERE o.UPC=p.UPC AND o.AD_Client_ID=p.AD_Client_ID) "
                  + "WHERE M_Product_ID IS NULL AND UPC IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Product from UPC=" + no);
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET M_Product_ID=(SELECT MAX(M_Product_ID) FROM M_Product p"
                  + " WHERE o.SKU=p.SKU AND o.AD_Client_ID=p.AD_Client_ID) "
                  + "WHERE M_Product_ID IS NULL AND SKU IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Product fom SKU=" + no);
            sql = new StringBuilder("UPDATE I_Order "
                  + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Product, ' "
                  + "WHERE M_Product_ID IS NULL AND (ProductValue IS NOT NULL OR UPC IS NOT NULL OR SKU IS NOT NULL)"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Product=" + no);

            //	Tax
            sql = new StringBuilder("UPDATE I_Order o "
                  + "SET C_Tax_ID=(SELECT MAX(C_Tax_ID) FROM C_Tax t"
                  + " WHERE o.TaxIndicator=t.TaxIndicator AND o.AD_Client_ID=t.AD_Client_ID) "
                  + "WHERE C_Tax_ID IS NULL AND TaxIndicator IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Tax=" + no);
            sql = new StringBuilder("UPDATE I_Order "
                  + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Tax, ' "
                  + "WHERE C_Tax_ID IS NULL AND TaxIndicator IS NOT NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Tax=" + no);

            Commit();

            //	-- New BPartner ---------------------------------------------------

            //	Go through Order Records w/o C_BPartner_ID
            sql = new StringBuilder("SELECT * FROM I_Order "
                  + "WHERE I_IsImported='N' AND C_BPartner_ID IS NULL").Append(clientCheck);
            IDataReader idr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement (sql.ToString(), Get_TrxName());
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                while (idr.Read())
                {
                    X_I_Order imp = new X_I_Order(GetCtx(), idr, Get_TrxName());
                    if (imp.GetBPartnerValue() == null)
                    {
                        if (imp.GetEMail() != null)
                            imp.SetBPartnerValue(imp.GetEMail());
                        else if (imp.GetName() != null)
                            imp.SetBPartnerValue(imp.GetName());
                        else
                            continue;
                    }
                    if (imp.GetName() == null)
                    {
                        if (imp.GetContactName() != null)
                            imp.SetName(imp.GetContactName());
                        else
                            imp.SetName(imp.GetBPartnerValue());
                    }
                    //	BPartner
                    MBPartner bp = MBPartner.Get(GetCtx(), imp.GetBPartnerValue());
                    if (bp == null)
                    {
                        bp = new MBPartner(GetCtx(), -1, Get_TrxName());
                        bp.SetClientOrg(imp.GetAD_Client_ID(), imp.GetAD_Org_ID());
                        bp.SetValue(imp.GetBPartnerValue());
                        bp.SetName(imp.GetName());
                        if (!bp.Save())
                            continue;
                    }
                    imp.SetC_BPartner_ID(bp.GetC_BPartner_ID());

                    //	BP Location
                    MBPartnerLocation bpl = null;
                    MBPartnerLocation[] bpls = bp.GetLocations(true);
                    for (int i = 0; bpl == null && i < bpls.Length; i++)
                    {
                        if (imp.GetC_BPartner_Location_ID() == bpls[i].GetC_BPartner_Location_ID())
                            bpl = bpls[i];
                        //	Same Location ID
                        else if (imp.GetC_Location_ID() == bpls[i].GetC_Location_ID())
                            bpl = bpls[i];
                        //	Same Location Info
                        else if (imp.GetC_Location_ID() == 0)
                        {
                            MLocation loc = bpl.GetLocation(false);
                            if (loc.Equals(imp.GetC_Country_ID(), imp.GetC_Region_ID(),
                                    imp.GetPostal(), "", imp.GetCity(),
                                    imp.GetAddress1(), imp.GetAddress2()))
                                bpl = bpls[i];
                        }
                    }
                    if (bpl == null)
                    {
                        //	New Location
                        MLocation loc = new MLocation(GetCtx(), 0, Get_TrxName());
                        loc.SetAddress1(imp.GetAddress1());
                        loc.SetAddress2(imp.GetAddress2());
                        loc.SetCity(imp.GetCity());
                        loc.SetPostal(imp.GetPostal());
                        if (imp.GetC_Region_ID() != 0)
                            loc.SetC_Region_ID(imp.GetC_Region_ID());
                        loc.SetC_Country_ID(imp.GetC_Country_ID());
                        if (!loc.Save())
                            continue;
                        //
                        bpl = new MBPartnerLocation(bp);
                        bpl.SetC_Location_ID(imp.GetC_Location_ID());
                        if (!bpl.Save())
                            continue;
                    }
                    imp.SetC_Location_ID(bpl.GetC_Location_ID());
                    imp.SetBillTo_ID(bpl.GetC_BPartner_Location_ID());
                    imp.SetC_BPartner_Location_ID(bpl.GetC_BPartner_Location_ID());

                    //	User/Contact
                    if (imp.GetContactName() != null
                        || imp.GetEMail() != null
                        || imp.GetPhone() != null)
                    {
                        MUser[] users = bp.GetContacts(true);
                        MUser user = null;
                        for (int i = 0; user == null && i < users.Length; i++)
                        {
                            String name = users[i].GetName();
                            if (name.Equals(imp.GetContactName())
                                || name.Equals(imp.GetName()))
                            {
                                user = users[i];
                                imp.SetAD_User_ID(user.GetAD_User_ID());
                            }
                        }
                        if (user == null)
                        {
                            user = new MUser(bp);
                            if (imp.GetContactName() == null)
                                user.SetName(imp.GetName());
                            else
                                user.SetName(imp.GetContactName());
                            user.SetEMail(imp.GetEMail());
                            user.SetPhone(imp.GetPhone());
                            if (user.Save())
                                imp.SetAD_User_ID(user.GetAD_User_ID());
                        }
                    }
                    imp.Save();
                }	//	for all new BPartners
                idr.Close();
                //
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "BP - " + sql.ToString(), e);
            }
            sql = new StringBuilder("UPDATE I_Order "
                  + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No BPartner, ' "
                  + "WHERE C_BPartner_ID IS NULL"
                  + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("No BPartner=" + no);

            Commit();

            //	-- New Orders -----------------------------------------------------

            int noInsert = 0;
            int noInsertLine = 0;

            //	Go through Order Records w/o
            sql = new StringBuilder("SELECT * FROM I_Order "
                  + "WHERE I_IsImported='N'").Append(clientCheck)
                .Append(" ORDER BY C_BPartner_ID, BillTo_ID, C_BPartner_Location_ID, I_Order_ID");
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement (sql.ToString(), Get_TrxName());
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                //
                int oldC_BPartner_ID = 0;
                int oldBillTo_ID = 0;
                int oldC_BPartner_Location_ID = 0;
                String oldDocumentNo = "";
                //
                MOrder order = null;
                int lineNo = 0;
                while (idr.Read())
                {
                    X_I_Order imp = new X_I_Order(GetCtx(), idr, Get_TrxName());
                    String cmpDocumentNo = imp.GetDocumentNo();
                    if (cmpDocumentNo == null)
                        cmpDocumentNo = "";
                    //	New Order
                    if (oldC_BPartner_ID != imp.GetC_BPartner_ID()
                        || oldC_BPartner_Location_ID != imp.GetC_BPartner_Location_ID()
                        || oldBillTo_ID != imp.GetBillTo_ID()
                        || !oldDocumentNo.Equals(cmpDocumentNo))
                    {
                        if (order != null)
                        {
                            if (_docAction != null && _docAction.Length > 0)
                            {
                                order.SetDocAction(_docAction);
                                order.ProcessIt(_docAction);
                            }
                            order.Save();	//	fails to process, import OK
                        }
                        oldC_BPartner_ID = imp.GetC_BPartner_ID();
                        oldC_BPartner_Location_ID = imp.GetC_BPartner_Location_ID();
                        oldBillTo_ID = imp.GetBillTo_ID();
                        oldDocumentNo = imp.GetDocumentNo();
                        if (oldDocumentNo == null)
                            oldDocumentNo = "";
                        //
                        order = new MOrder(GetCtx(), 0, Get_TrxName());
                        order.SetClientOrg(imp.GetAD_Client_ID(), imp.GetAD_Org_ID());
                        order.SetC_DocTypeTarget_ID(imp.GetC_DocType_ID());
                        order.SetIsSOTrx(imp.IsSOTrx());
                        if (imp.GetDocumentNo() != null)
                            order.SetDocumentNo(imp.GetDocumentNo());
                        //	Ship Partner
                        order.SetC_BPartner_ID(imp.GetC_BPartner_ID());
                        order.SetC_BPartner_Location_ID(imp.GetC_BPartner_Location_ID());
                        if (imp.GetAD_User_ID() != 0)
                            order.SetAD_User_ID(imp.GetAD_User_ID());
                        //	Bill Partner
                        order.SetBill_BPartner_ID(imp.GetC_BPartner_ID());
                        order.SetBill_Location_ID(imp.GetBillTo_ID());
                        //
                        if (imp.GetDescription() != null)
                            order.SetDescription(imp.GetDescription());
                        if (imp.GetPaymentRule() != null)
                            order.SetPaymentRule(imp.GetPaymentRule());
                        order.SetC_PaymentTerm_ID(imp.GetC_PaymentTerm_ID());
                        order.SetM_PriceList_ID(imp.GetM_PriceList_ID());
                        order.SetM_Warehouse_ID(imp.GetM_Warehouse_ID());
                        if (imp.GetM_Shipper_ID() != 0)
                            order.SetM_Shipper_ID(imp.GetM_Shipper_ID());
                        //	SalesRep from Import or the person running the import
                        if (imp.GetSalesRep_ID() != 0)
                            order.SetSalesRep_ID(imp.GetSalesRep_ID());
                        if (order.GetSalesRep_ID() == 0)
                            order.SetSalesRep_ID(GetAD_User_ID());
                        //
                        if (imp.GetAD_OrgTrx_ID() != 0)
                            order.SetAD_OrgTrx_ID(imp.GetAD_OrgTrx_ID());
                        if (imp.GetC_Activity_ID() != 0)
                            order.SetC_Activity_ID(imp.GetC_Activity_ID());
                        if (imp.GetC_Campaign_ID() != 0)
                            order.SetC_Campaign_ID(imp.GetC_Campaign_ID());
                        if (imp.GetC_Project_ID() != 0)
                            order.SetC_Project_ID(imp.GetC_Project_ID());
                        //
                        if (imp.GetDateOrdered() != null)
                            order.SetDateOrdered(imp.GetDateOrdered());
                        if (imp.GetDateAcct() != null)
                            order.SetDateAcct(imp.GetDateAcct());
                        //
                        if (!order.Save())
                        {
                            String msg = "Could not save Order";
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null)
                                msg += " - " + pp.ToString();

                            imp.SetI_ErrorMsg(msg);
                            imp.Save();
                            continue;
                        }
                        noInsert++;
                        lineNo = 10;
                    }
                    imp.SetC_Order_ID(order.GetC_Order_ID());
                    //	New OrderLine
                    MOrderLine line = new MOrderLine(order);
                    line.SetLine(lineNo);
                    lineNo += 10;

                    // gwu: 1712639, added support for UOM conversions
                    bool convertUOM = false;
                    if (imp.GetM_Product_ID() != 0 && imp.GetC_UOM_ID() != 0)
                    {
                        line.SetM_Product_ID(imp.GetM_Product_ID(), imp.GetC_UOM_ID());
                        convertUOM = (line.GetProduct().GetC_UOM_ID() != imp.GetC_UOM_ID());
                    }
                    else if (imp.GetM_Product_ID() != 0)
                    {
                        line.SetM_Product_ID(imp.GetM_Product_ID(), true);
                        convertUOM = false;
                    }

                    if (convertUOM)
                    {
                        Decimal? rateQty = MUOMConversion.GetProductRateFrom(GetCtx(), line.GetM_Product_ID(), imp.GetC_UOM_ID());
                        if (rateQty == null)
                        {
                            String msg = Msg.Translate(GetCtx(), "NoProductUOMConversion");
                            imp.SetI_ErrorMsg(msg);
                            imp.Save();
                            continue;
                        }
                        line.SetQtyEntered(imp.GetQtyOrdered());
                        //line.SetQtyOrdered( imp.GetQtyOrdered().multiply( rateQty ) );
                        line.SetQtyOrdered(Decimal.Multiply(imp.GetQtyOrdered(), rateQty.Value));
                        line.SetPrice();
                        if (imp.GetPriceActual().CompareTo(Env.ZERO) != 0)
                        {
                            line.SetPriceEntered(imp.GetPriceActual());
                            //line.SetPriceActual( imp.GetPriceActual().divide( rateQty, 12, BigDecimal.ROUND_HALF_UP ) );
                            line.SetPriceActual(Decimal.Round(Decimal.Divide(imp.GetPriceActual(), rateQty.Value), 12, MidpointRounding.AwayFromZero));
                        }
                    }
                    else // no UOM conversion
                    {
                        line.SetQty(imp.GetQtyOrdered());
                        line.SetPrice();
                        if (imp.GetPriceActual().CompareTo(Env.ZERO) != 0)
                            line.SetPrice(imp.GetPriceActual());
                    }

                    if (imp.GetC_Tax_ID() != 0)
                        line.SetC_Tax_ID(imp.GetC_Tax_ID());
                    else
                    {
                        line.SetTax();
                        imp.SetC_Tax_ID(line.GetC_Tax_ID());
                    }
                    //if (imp.GetFreightAmt() != null)
                        line.SetFreightAmt(imp.GetFreightAmt());
                    if (imp.GetLineDescription() != null)
                        line.SetDescription(imp.GetLineDescription());

                    if (!line.Save())
                    {
                        String msg = "Could not save OrderLine";
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null)
                            msg += " - " + pp.ToString();
                        imp.SetI_ErrorMsg(msg);
                        imp.Save();
                        continue;
                    }

                    //	Update Import
                    imp.SetC_OrderLine_ID(line.GetC_OrderLine_ID());
                    imp.SetI_IsImported(X_I_Order.I_ISIMPORTED_Yes);
                    imp.SetProcessed(true);
                    if (imp.Save())
                        noInsertLine++;
                }
                if (order != null)
                {
                    if (_docAction != null && _docAction.Length > 0)
                    {
                        order.SetDocAction(_docAction);
                        order.ProcessIt(_docAction);
                    }
                    order.Save();
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

            //	Set Error to indicator to not imported
            sql = new StringBuilder("UPDATE I_Order "
                + "SET I_IsImported='N', Updated=SysDate "
                + "WHERE I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, Utility.Util.GetValueOfDecimal(no), "@Errors@");
            //
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsert), "@C_Order_ID@: @Inserted@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsertLine), "@C_OrderLine_ID@: @Inserted@");
            return "#" + noInsert + "/" + noInsertLine;
        }	//	doIt

    }
}

/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ImportBPartner
 * Purpose        : Import BPartners from I_BPartner
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
    public class ImportBPartner : ProcessEngine.SvrProcess
    {
        /**	Client to be imported to		*/
        private int _VAF_Client_ID = 0;
        /**	Delete old Imported				*/
        private bool _deleteOldImported = false;

        /** Effective						*/
        private DateTime? _DateValue = null;

        /// <summary>
        ///  Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (name.Equals("VAF_Client_ID"))
                    _VAF_Client_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("DeleteOldImported"))
                    _deleteOldImported = "Y".Equals(para[i].GetParameter());
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
            String clientCheck = " AND VAF_Client_ID=" + _VAF_Client_ID;

            //	****	Prepare	****

            //	Delete Old Imported
            if (_deleteOldImported)
            {
                sql = new StringBuilder("DELETE FROM I_BPartner "
                    + "WHERE I_IsImported='Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Delete Old Impored =" + no);
            }

            //	Set Client, Org, IsActive, Created/Updated
            sql = new StringBuilder("UPDATE I_BPartner "
                + "SET VAF_Client_ID = COALESCE (VAF_Client_ID, ").Append(_VAF_Client_ID).Append("),"
                + " VAF_Org_ID = COALESCE (VAF_Org_ID, 0),"
                + " IsActive = COALESCE (IsActive, 'Y'),"
                + " Created = COALESCE (Created, SysDate),"
                + " CreatedBy = COALESCE (CreatedBy, 0),"
                + " Updated = COALESCE (Updated, SysDate),"
                + " UpdatedBy = COALESCE (UpdatedBy, 0),"
                + " I_ErrorMsg = NULL,"
                + " I_IsImported = 'N' "
                + "WHERE I_IsImported<>'Y' OR I_IsImported IS NULL");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Reset=" + no);

            //	Set BP_Group
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET GroupValue=(SELECT MAX(Value) FROM VAB_BPart_Category g WHERE g.IsDefault='Y'"
                + " AND g.VAF_Client_ID=i.VAF_Client_ID) ");
            sql.Append("WHERE GroupValue IS NULL AND VAB_BPart_Category_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Group Default=" + no);
            //
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET VAB_BPart_Category_ID=(SELECT VAB_BPart_Category_ID FROM VAB_BPart_Category g"
                + " WHERE i.GroupValue=g.Value AND g.VAF_Client_ID=i.VAF_Client_ID) "
                + "WHERE VAB_BPart_Category_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Group=" + no);
            //
            String ts = DataBase.DB.IsPostgreSQL() ? "COALESCE(I_ErrorMsg,'')" : "I_ErrorMsg";  //java bug, it could not be used directly
            sql = new StringBuilder("UPDATE I_BPartner "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Group, ' "
                + "WHERE VAB_BPart_Category_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid Group=" + no);

            //	Set Country
            /**
            sql = new StringBuilder ("UPDATE I_BPartner i "
                + "SET CountryCode=(SELECT CountryCode FROM VAB_Country c WHERE c.IsDefault='Y'"
                + " AND c.VAF_Client_ID IN (0, i.VAF_Client_ID) AND ROWNUM=1) "
                + "WHERE CountryCode IS NULL AND VAB_Country_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(),null, Get_TrxName());
            log.Fine("Set Country Default=" + no);
            **/
            //
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET VAB_Country_ID=(SELECT VAB_Country_ID FROM VAB_Country c"
                + " WHERE i.CountryCode=c.CountryCode AND c.IsSummary='N' AND c.VAF_Client_ID IN (0, i.VAF_Client_ID)) "
                + "WHERE VAB_Country_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Country=" + no);
            //
            sql = new StringBuilder("UPDATE I_BPartner "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Country, ' "
                + "WHERE VAB_Country_ID IS NULL AND (City IS NOT NULL OR Address1 IS NOT NULL)"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid Country=" + no);

            //	Set Region
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "Set RegionName=(SELECT Name FROM VAB_RegionState r"
                + " WHERE r.IsDefault='Y' AND r.VAB_Country_ID=i.VAB_Country_ID"
                + " AND r.VAF_Client_ID IN (0, i.VAF_Client_ID)) ");
            /*
            if (DataBase.isOracle()) //jz
            {
                sql.Append(" AND ROWNUM=1) ");
            }
            else 
                sql.Append(" AND r.UPDATED IN (SELECT MAX(UPDATED) FROM VAB_RegionState r1"
                + " WHERE r1.IsDefault='Y' AND r1.VAB_Country_ID=i.VAB_Country_ID"
                + " AND r1.VAF_Client_ID IN (0, i.VAF_Client_ID) ");
                */
            sql.Append("WHERE RegionName IS NULL AND VAB_RegionState_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Region Default=" + no);
            //
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "Set VAB_RegionState_ID=(SELECT VAB_RegionState_ID FROM VAB_RegionState r"
                + " WHERE r.Name=i.RegionName AND r.VAB_Country_ID=i.VAB_Country_ID"
                + " AND r.VAF_Client_ID IN (0, i.VAF_Client_ID)) "
                + "WHERE VAB_RegionState_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Region=" + no);
            //
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Region, ' "
                + "WHERE VAB_RegionState_ID IS NULL "
                + " AND EXISTS (SELECT * FROM VAB_Country c"
                + " WHERE c.VAB_Country_ID=i.VAB_Country_ID AND c.HasRegion='Y')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid Region=" + no);

            //	Set Greeting
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET VAB_Greeting_ID=(SELECT VAB_Greeting_ID FROM VAB_Greeting g"
                + " WHERE i.BPContactGreeting=g.Name AND g.VAF_Client_ID IN (0, i.VAF_Client_ID)) "
                + "WHERE VAB_Greeting_ID IS NULL AND BPContactGreeting IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Greeting=" + no);
            //
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Greeting, ' "
                + "WHERE VAB_Greeting_ID IS NULL AND BPContactGreeting IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid Greeting=" + no);

            //	Existing User ?
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET (VAB_BusinessPartner_ID,VAF_UserContact_ID)="
                    + "(SELECT VAB_BusinessPartner_ID,VAF_UserContact_ID FROM VAF_UserContact u "
                    + "WHERE i.EMail=u.EMail AND u.VAF_Client_ID=i.VAF_Client_ID) "
                + "WHERE i.EMail IS NOT NULL AND I_IsImported='N'").Append(clientCheck);

            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Found EMail User=" + no);

            //	Existing BPartner ? Match Value
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET VAB_BusinessPartner_ID=(SELECT VAB_BusinessPartner_ID FROM VAB_BusinessPartner p"
                + " WHERE i.Value=p.Value AND p.VAF_Client_ID=i.VAF_Client_ID) "
                + "WHERE VAB_BusinessPartner_ID IS NULL AND Value IS NOT NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Found BPartner=" + no);

            //	Existing Contact ? Match Name
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET VAF_UserContact_ID=(SELECT VAF_UserContact_ID FROM VAF_UserContact c"
                + " WHERE i.ContactName=c.Name AND i.VAB_BusinessPartner_ID=c.VAB_BusinessPartner_ID AND c.VAF_Client_ID=i.VAF_Client_ID) "
                + "WHERE VAB_BusinessPartner_ID IS NOT NULL AND VAF_UserContact_ID IS NULL AND ContactName IS NOT NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Found Contact=" + no);

            //	Existing Location ? Exact Match
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET VAB_BPart_Location_ID=(SELECT VAB_BPart_Location_ID"
                + " FROM VAB_BPart_Location bpl INNER JOIN VAB_Address l ON (bpl.VAB_Address_ID=l.VAB_Address_ID)"
                + " WHERE i.VAB_BusinessPartner_ID=bpl.VAB_BusinessPartner_ID AND bpl.VAF_Client_ID=i.VAF_Client_ID"
                + " AND DUMP(i.Address1)=DUMP(l.Address1) AND DUMP(i.Address2)=DUMP(l.Address2)"
                + " AND DUMP(i.City)=DUMP(l.City) AND DUMP(i.Postal)=DUMP(l.Postal) AND DUMP(i.Postal_Add)=DUMP(l.Postal_Add)"
                + " AND DUMP(i.VAB_RegionState_ID)=DUMP(l.VAB_RegionState_ID) AND DUMP(i.VAB_Country_ID)=DUMP(l.VAB_Country_ID)) "
                + "WHERE VAB_BusinessPartner_ID IS NOT NULL AND VAB_BPart_Location_ID IS NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Found Location=" + no);

            //	Interest Area
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET VAR_InterestArea_ID=(SELECT VAR_InterestArea_ID FROM VAR_InterestArea ia "
                    + "WHERE i.InterestAreaName=ia.Name AND ia.VAF_Client_ID=i.VAF_Client_ID) "
                + "WHERE VAR_InterestArea_ID IS NULL AND InterestAreaName IS NOT NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Interest Area=" + no);


            Commit();
            //	-------------------------------------------------------------------
            int noInsert = 0;
            int noUpdate = 0;
            IDataReader idr = null;
            //	Go through Records
            sql = new StringBuilder("SELECT * FROM I_BPartner "
                + "WHERE I_IsImported='N'").Append(clientCheck);
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql.ToString(), Get_TrxName());
                //ResultSet rs = pstmt.executeQuery();
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                while (idr.Read())
                {
                    X_I_BPartner impBP = new X_I_BPartner(GetCtx(), idr, Get_TrxName());
                    log.Fine("I_BPartner_ID=" + impBP.GetI_BPartner_ID()
                        + ", VAB_BusinessPartner_ID=" + impBP.GetVAB_BusinessPartner_ID()
                        + ", VAB_BPart_Location_ID=" + impBP.GetVAB_BPart_Location_ID()
                        + ", VAF_UserContact_ID=" + impBP.GetVAF_UserContact_ID());


                    //	****	Create/Update BPartner	****
                    MVABBusinessPartner bp = null;
                    if (impBP.GetVAB_BusinessPartner_ID() == 0)	//	Insert new BPartner
                    {
                        bp = new MVABBusinessPartner(impBP);
                        if (bp.Save())
                        {
                            impBP.SetVAB_BusinessPartner_ID(bp.GetVAB_BusinessPartner_ID());
                            log.Finest("Insert BPartner - " + bp.GetVAB_BusinessPartner_ID());
                            noInsert++;
                        }
                        else
                        {
                            sql = new StringBuilder("UPDATE I_BPartner i "
                                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||")
                                .Append("Cannot Insert BPartner")
                                .Append("WHERE I_BPartner_ID=").Append(impBP.GetI_BPartner_ID());
                            DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                            continue;
                        }
                    }
                    else				//	Update existing BPartner
                    {
                        bp = new MVABBusinessPartner(GetCtx(), impBP.GetVAB_BusinessPartner_ID(), Get_TrxName());
                        //	if (impBP.getValue() != null)			//	not to overwite
                        //		bp.setValue(impBP.getValue());
                        if (impBP.GetName() != null)
                        {
                            bp.SetName(impBP.GetName());
                            bp.SetName2(impBP.GetName2());
                        }
                        if (impBP.GetDUNS() != null)
                            bp.SetDUNS(impBP.GetDUNS());
                        if (impBP.GetTaxID() != null)
                            bp.SetTaxID(impBP.GetTaxID());
                        if (impBP.GetNAICS() != null)
                            bp.SetNAICS(impBP.GetNAICS());
                        if (impBP.GetVAB_BPart_Category_ID() != 0)
                            bp.SetVAB_BPart_Category_ID(impBP.GetVAB_BPart_Category_ID());
                        if (impBP.GetDescription() != null)
                            bp.SetDescription(impBP.GetDescription());
                        //
                        if (bp.Save())
                        {
                            log.Finest("Update BPartner - " + bp.GetVAB_BusinessPartner_ID());
                            noUpdate++;
                        }
                        else
                        {
                            sql = new StringBuilder("UPDATE I_BPartner i "
                                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||")
                                .Append("' Cannot Update BPartner' ") //jz
                                .Append("WHERE I_BPartner_ID=").Append(impBP.GetI_BPartner_ID());
                            DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                            continue;
                        }
                    }

                    //	****	Create/Update BPartner Location	****
                    MVABBPartLocation bpl = null;
                    if (impBP.GetVAB_BPart_Location_ID() != 0)		//	Update Location
                    {
                        bpl = new MVABBPartLocation(GetCtx(), impBP.GetVAB_BPart_Location_ID(), Get_TrxName());
                        MVABAddress location = new MVABAddress(GetCtx(), bpl.GetVAB_Address_ID(), Get_TrxName());
                        location.SetVAB_Country_ID(impBP.GetVAB_Country_ID());
                        location.SetVAB_RegionState_ID(impBP.GetVAB_RegionState_ID());
                        location.SetCity(impBP.GetCity());
                        location.SetAddress1(impBP.GetAddress1());
                        location.SetAddress2(impBP.GetAddress2());
                        location.SetPostal(impBP.GetPostal());
                        location.SetPostal_Add(impBP.GetPostal_Add());
                        location.SetRegionName(impBP.GetRegionName());
                        if (!location.Save())
                            log.Warning("Location not updated");
                        else
                            bpl.SetVAB_Address_ID(location.GetVAB_Address_ID());
                        if (impBP.GetPhone() != null)
                            bpl.SetPhone(impBP.GetPhone());
                        if (impBP.GetPhone2() != null)
                            bpl.SetPhone2(impBP.GetPhone2());
                        if (impBP.GetFax() != null)
                            bpl.SetFax(impBP.GetFax());
                        bpl.Save();
                    }
                    else 	//	New Location
                        if (impBP.GetVAB_Country_ID() != 0
                            && impBP.GetAddress1() != null
                            && impBP.GetCity() != null)
                        {
                            MVABAddress location = new MVABAddress(GetCtx(), impBP.GetVAB_Country_ID(),
                                impBP.GetVAB_RegionState_ID(), impBP.GetCity(), Get_TrxName());
                            location.SetAddress1(impBP.GetAddress1());
                            location.SetAddress2(impBP.GetAddress2());
                            location.SetPostal(impBP.GetPostal());
                            location.SetPostal_Add(impBP.GetPostal_Add());
                            location.SetRegionName(impBP.GetRegionName());
                            if (location.Save())
                                log.Finest("Insert Location - " + location.GetVAB_Address_ID());
                            else
                            {
                                Rollback();
                                noInsert--;
                                sql = new StringBuilder("UPDATE I_BPartner i "
                                    + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||")
                                    .Append("Cannot Insert Location")
                                    .Append("WHERE I_BPartner_ID=").Append(impBP.GetI_BPartner_ID());
                                DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                                continue;
                            }
                            //
                            bpl = new MVABBPartLocation(bp);
                            bpl.SetVAB_Address_ID(location.GetVAB_Address_ID());
                            bpl.SetPhone(impBP.GetPhone());
                            bpl.SetPhone2(impBP.GetPhone2());
                            bpl.SetFax(impBP.GetFax());
                            if (bpl.Save())
                            {
                                log.Finest("Insert BP Location - " + bpl.GetVAB_BPart_Location_ID());
                                impBP.SetVAB_BPart_Location_ID(bpl.GetVAB_BPart_Location_ID());
                            }
                            else
                            {
                                Rollback();
                                noInsert--;
                                sql = new StringBuilder("UPDATE I_BPartner i "
                                    + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||")
                                    .Append("Cannot Insert BPLocation")
                                    .Append("WHERE I_BPartner_ID=").Append(impBP.GetI_BPartner_ID());
                                DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                                continue;
                            }
                        }

                    //	****	Create/Update Contact	****
                    MVAFUserContact user = null;
                    if (impBP.GetVAF_UserContact_ID() != 0)
                    {
                        user = new MVAFUserContact(GetCtx(), impBP.GetVAF_UserContact_ID(), Get_TrxName());
                        if (user.GetVAB_BusinessPartner_ID() == 0)
                            user.SetVAB_BusinessPartner_ID(bp.GetVAB_BusinessPartner_ID());
                        else if (user.GetVAB_BusinessPartner_ID() != bp.GetVAB_BusinessPartner_ID())
                        {
                            Rollback();
                            noInsert--;
                            sql = new StringBuilder("UPDATE I_BPartner i "
                                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||")
                                .Append("BP of User <> BP")
                                .Append("WHERE I_BPartner_ID=").Append(impBP.GetI_BPartner_ID());
                            DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                            continue;
                        }
                        if (impBP.GetVAB_Greeting_ID() != 0)
                            user.SetVAB_Greeting_ID(impBP.GetVAB_Greeting_ID());
                        String name = impBP.GetContactName();
                        if (name == null || name.Length == 0)
                            name = impBP.GetEMail();
                        user.SetName(name);
                        if (impBP.GetTitle() != null)
                            user.SetTitle(impBP.GetTitle());
                        if (impBP.GetContactDescription() != null)
                            user.SetDescription(impBP.GetContactDescription());
                        if (impBP.GetComments() != null)
                            user.SetComments(impBP.GetComments());
                        if (impBP.GetPhone() != null)
                            user.SetPhone(impBP.GetPhone());
                        if (impBP.GetPhone2() != null)
                            user.SetPhone2(impBP.GetPhone2());
                        if (impBP.GetFax() != null)
                            user.SetFax(impBP.GetFax());
                        if (impBP.GetEMail() != null)
                            user.SetEMail(impBP.GetEMail());
                        if (impBP.GetBirthday() != null)
                            user.SetBirthday(impBP.GetBirthday());
                        if (bpl != null)
                            user.SetVAB_BPart_Location_ID(bpl.GetVAB_BPart_Location_ID());
                        if (user.Save())
                        {
                            log.Finest("Update BP Contact - " + user.GetVAF_UserContact_ID());
                        }
                        else
                        {
                            Rollback();
                            noInsert--;
                            sql = new StringBuilder("UPDATE I_BPartner i "
                                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||")
                                .Append("Cannot Update BP Contact")
                                .Append("WHERE I_BPartner_ID=").Append(impBP.GetI_BPartner_ID());
                            DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                            continue;
                        }
                    }
                    else 	//	New Contact
                        if (impBP.GetContactName() != null || impBP.GetEMail() != null)
                        {
                            user = new MVAFUserContact(bp);
                            if (impBP.GetVAB_Greeting_ID() != 0)
                                user.SetVAB_Greeting_ID(impBP.GetVAB_Greeting_ID());
                            String name = impBP.GetContactName();
                            if (name == null || name.Length == 0)
                                name = impBP.GetEMail();
                            user.SetName(name);
                            user.SetTitle(impBP.GetTitle());
                            user.SetDescription(impBP.GetContactDescription());
                            user.SetComments(impBP.GetComments());
                            user.SetPhone(impBP.GetPhone());
                            user.SetPhone2(impBP.GetPhone2());
                            user.SetFax(impBP.GetFax());
                            user.SetEMail(impBP.GetEMail());
                            user.SetBirthday(impBP.GetBirthday());
                            if (bpl != null)
                                user.SetVAB_BPart_Location_ID(bpl.GetVAB_BPart_Location_ID());
                            if (user.Save())
                            {
                                log.Finest("Insert BP Contact - " + user.GetVAF_UserContact_ID());
                                impBP.SetVAF_UserContact_ID(user.GetVAF_UserContact_ID());
                            }
                            else
                            {
                                Rollback();
                                noInsert--;
                                sql = new StringBuilder("UPDATE I_BPartner i "
                                    + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||")
                                    .Append("Cannot Insert BPContact")
                                    .Append("WHERE I_BPartner_ID=").Append(impBP.GetI_BPartner_ID());
                                DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                                continue;
                            }
                        }

                    //	Interest Area
                    if (impBP.GetR_InterestArea_ID() != 0 && user != null)
                    {
                        MVARInterestedUser ci = MVARInterestedUser.Get(GetCtx(),
                            impBP.GetR_InterestArea_ID(), user.GetVAF_UserContact_ID(),
                            true, Get_TrxName());
                        ci.Save();		//	don't subscribe or re-activate
                    }
                    //
                    impBP.SetI_IsImported(X_I_BPartner.I_ISIMPORTED_Yes);
                    impBP.SetProcessed(true);
                    impBP.SetProcessing(false);
                    impBP.Save();
                    Commit();
                }	//	for all I_Product
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "", e);
                Rollback();
            }

            //	Set Error to indicator to not imported
            sql = new StringBuilder("UPDATE I_BPartner "
                + "SET I_IsImported='N', Updated=SysDate "
                + "WHERE I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, Utility.Util.GetValueOfDecimal(no), "@Errors@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsert), "@VAB_BusinessPartner_ID@: @Inserted@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noUpdate), "@VAB_BusinessPartner_ID@: @Updated@");
            return "";
        }	//	doIt
    }

}

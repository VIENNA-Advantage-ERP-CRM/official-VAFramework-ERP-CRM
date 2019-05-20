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
        private int _AD_Client_ID = 0;
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
                if (name.Equals("AD_Client_ID"))
                    _AD_Client_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
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
            String clientCheck = " AND AD_Client_ID=" + _AD_Client_ID;

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
                + "SET AD_Client_ID = COALESCE (AD_Client_ID, ").Append(_AD_Client_ID).Append("),"
                + " AD_Org_ID = COALESCE (AD_Org_ID, 0),"
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
                + "SET GroupValue=(SELECT MAX(Value) FROM C_BP_Group g WHERE g.IsDefault='Y'"
                + " AND g.AD_Client_ID=i.AD_Client_ID) ");
            sql.Append("WHERE GroupValue IS NULL AND C_BP_Group_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Group Default=" + no);
            //
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET C_BP_Group_ID=(SELECT C_BP_Group_ID FROM C_BP_Group g"
                + " WHERE i.GroupValue=g.Value AND g.AD_Client_ID=i.AD_Client_ID) "
                + "WHERE C_BP_Group_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Group=" + no);
            //
            String ts = DataBase.DB.IsPostgreSQL() ? "COALESCE(I_ErrorMsg,'')" : "I_ErrorMsg";  //java bug, it could not be used directly
            sql = new StringBuilder("UPDATE I_BPartner "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Group, ' "
                + "WHERE C_BP_Group_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid Group=" + no);

            //	Set Country
            /**
            sql = new StringBuilder ("UPDATE I_BPartner i "
                + "SET CountryCode=(SELECT CountryCode FROM C_Country c WHERE c.IsDefault='Y'"
                + " AND c.AD_Client_ID IN (0, i.AD_Client_ID) AND ROWNUM=1) "
                + "WHERE CountryCode IS NULL AND C_Country_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(),null, Get_TrxName());
            log.Fine("Set Country Default=" + no);
            **/
            //
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET C_Country_ID=(SELECT C_Country_ID FROM C_Country c"
                + " WHERE i.CountryCode=c.CountryCode AND c.IsSummary='N' AND c.AD_Client_ID IN (0, i.AD_Client_ID)) "
                + "WHERE C_Country_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Country=" + no);
            //
            sql = new StringBuilder("UPDATE I_BPartner "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Country, ' "
                + "WHERE C_Country_ID IS NULL AND (City IS NOT NULL OR Address1 IS NOT NULL)"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid Country=" + no);

            //	Set Region
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "Set RegionName=(SELECT Name FROM C_Region r"
                + " WHERE r.IsDefault='Y' AND r.C_Country_ID=i.C_Country_ID"
                + " AND r.AD_Client_ID IN (0, i.AD_Client_ID)) ");
            /*
            if (DataBase.isOracle()) //jz
            {
                sql.Append(" AND ROWNUM=1) ");
            }
            else 
                sql.Append(" AND r.UPDATED IN (SELECT MAX(UPDATED) FROM C_Region r1"
                + " WHERE r1.IsDefault='Y' AND r1.C_Country_ID=i.C_Country_ID"
                + " AND r1.AD_Client_ID IN (0, i.AD_Client_ID) ");
                */
            sql.Append("WHERE RegionName IS NULL AND C_Region_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Region Default=" + no);
            //
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "Set C_Region_ID=(SELECT C_Region_ID FROM C_Region r"
                + " WHERE r.Name=i.RegionName AND r.C_Country_ID=i.C_Country_ID"
                + " AND r.AD_Client_ID IN (0, i.AD_Client_ID)) "
                + "WHERE C_Region_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Region=" + no);
            //
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Region, ' "
                + "WHERE C_Region_ID IS NULL "
                + " AND EXISTS (SELECT * FROM C_Country c"
                + " WHERE c.C_Country_ID=i.C_Country_ID AND c.HasRegion='Y')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid Region=" + no);

            //	Set Greeting
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET C_Greeting_ID=(SELECT C_Greeting_ID FROM C_Greeting g"
                + " WHERE i.BPContactGreeting=g.Name AND g.AD_Client_ID IN (0, i.AD_Client_ID)) "
                + "WHERE C_Greeting_ID IS NULL AND BPContactGreeting IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Greeting=" + no);
            //
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Greeting, ' "
                + "WHERE C_Greeting_ID IS NULL AND BPContactGreeting IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid Greeting=" + no);

            //	Existing User ?
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET (C_BPartner_ID,AD_User_ID)="
                    + "(SELECT C_BPartner_ID,AD_User_ID FROM AD_User u "
                    + "WHERE i.EMail=u.EMail AND u.AD_Client_ID=i.AD_Client_ID) "
                + "WHERE i.EMail IS NOT NULL AND I_IsImported='N'").Append(clientCheck);

            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Found EMail User=" + no);

            //	Existing BPartner ? Match Value
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET C_BPartner_ID=(SELECT C_BPartner_ID FROM C_BPartner p"
                + " WHERE i.Value=p.Value AND p.AD_Client_ID=i.AD_Client_ID) "
                + "WHERE C_BPartner_ID IS NULL AND Value IS NOT NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Found BPartner=" + no);

            //	Existing Contact ? Match Name
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET AD_User_ID=(SELECT AD_User_ID FROM AD_User c"
                + " WHERE i.ContactName=c.Name AND i.C_BPartner_ID=c.C_BPartner_ID AND c.AD_Client_ID=i.AD_Client_ID) "
                + "WHERE C_BPartner_ID IS NOT NULL AND AD_User_ID IS NULL AND ContactName IS NOT NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Found Contact=" + no);

            //	Existing Location ? Exact Match
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET C_BPartner_Location_ID=(SELECT C_BPartner_Location_ID"
                + " FROM C_BPartner_Location bpl INNER JOIN C_Location l ON (bpl.C_Location_ID=l.C_Location_ID)"
                + " WHERE i.C_BPartner_ID=bpl.C_BPartner_ID AND bpl.AD_Client_ID=i.AD_Client_ID"
                + " AND DUMP(i.Address1)=DUMP(l.Address1) AND DUMP(i.Address2)=DUMP(l.Address2)"
                + " AND DUMP(i.City)=DUMP(l.City) AND DUMP(i.Postal)=DUMP(l.Postal) AND DUMP(i.Postal_Add)=DUMP(l.Postal_Add)"
                + " AND DUMP(i.C_Region_ID)=DUMP(l.C_Region_ID) AND DUMP(i.C_Country_ID)=DUMP(l.C_Country_ID)) "
                + "WHERE C_BPartner_ID IS NOT NULL AND C_BPartner_Location_ID IS NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Found Location=" + no);

            //	Interest Area
            sql = new StringBuilder("UPDATE I_BPartner i "
                + "SET R_InterestArea_ID=(SELECT R_InterestArea_ID FROM R_InterestArea ia "
                    + "WHERE i.InterestAreaName=ia.Name AND ia.AD_Client_ID=i.AD_Client_ID) "
                + "WHERE R_InterestArea_ID IS NULL AND InterestAreaName IS NOT NULL"
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
                        + ", C_BPartner_ID=" + impBP.GetC_BPartner_ID()
                        + ", C_BPartner_Location_ID=" + impBP.GetC_BPartner_Location_ID()
                        + ", AD_User_ID=" + impBP.GetAD_User_ID());


                    //	****	Create/Update BPartner	****
                    MBPartner bp = null;
                    if (impBP.GetC_BPartner_ID() == 0)	//	Insert new BPartner
                    {
                        bp = new MBPartner(impBP);
                        if (bp.Save())
                        {
                            impBP.SetC_BPartner_ID(bp.GetC_BPartner_ID());
                            log.Finest("Insert BPartner - " + bp.GetC_BPartner_ID());
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
                        bp = new MBPartner(GetCtx(), impBP.GetC_BPartner_ID(), Get_TrxName());
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
                        if (impBP.GetC_BP_Group_ID() != 0)
                            bp.SetC_BP_Group_ID(impBP.GetC_BP_Group_ID());
                        if (impBP.GetDescription() != null)
                            bp.SetDescription(impBP.GetDescription());
                        //
                        if (bp.Save())
                        {
                            log.Finest("Update BPartner - " + bp.GetC_BPartner_ID());
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
                    MBPartnerLocation bpl = null;
                    if (impBP.GetC_BPartner_Location_ID() != 0)		//	Update Location
                    {
                        bpl = new MBPartnerLocation(GetCtx(), impBP.GetC_BPartner_Location_ID(), Get_TrxName());
                        MLocation location = new MLocation(GetCtx(), bpl.GetC_Location_ID(), Get_TrxName());
                        location.SetC_Country_ID(impBP.GetC_Country_ID());
                        location.SetC_Region_ID(impBP.GetC_Region_ID());
                        location.SetCity(impBP.GetCity());
                        location.SetAddress1(impBP.GetAddress1());
                        location.SetAddress2(impBP.GetAddress2());
                        location.SetPostal(impBP.GetPostal());
                        location.SetPostal_Add(impBP.GetPostal_Add());
                        location.SetRegionName(impBP.GetRegionName());
                        if (!location.Save())
                            log.Warning("Location not updated");
                        else
                            bpl.SetC_Location_ID(location.GetC_Location_ID());
                        if (impBP.GetPhone() != null)
                            bpl.SetPhone(impBP.GetPhone());
                        if (impBP.GetPhone2() != null)
                            bpl.SetPhone2(impBP.GetPhone2());
                        if (impBP.GetFax() != null)
                            bpl.SetFax(impBP.GetFax());
                        bpl.Save();
                    }
                    else 	//	New Location
                        if (impBP.GetC_Country_ID() != 0
                            && impBP.GetAddress1() != null
                            && impBP.GetCity() != null)
                        {
                            MLocation location = new MLocation(GetCtx(), impBP.GetC_Country_ID(),
                                impBP.GetC_Region_ID(), impBP.GetCity(), Get_TrxName());
                            location.SetAddress1(impBP.GetAddress1());
                            location.SetAddress2(impBP.GetAddress2());
                            location.SetPostal(impBP.GetPostal());
                            location.SetPostal_Add(impBP.GetPostal_Add());
                            location.SetRegionName(impBP.GetRegionName());
                            if (location.Save())
                                log.Finest("Insert Location - " + location.GetC_Location_ID());
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
                            bpl = new MBPartnerLocation(bp);
                            bpl.SetC_Location_ID(location.GetC_Location_ID());
                            bpl.SetPhone(impBP.GetPhone());
                            bpl.SetPhone2(impBP.GetPhone2());
                            bpl.SetFax(impBP.GetFax());
                            if (bpl.Save())
                            {
                                log.Finest("Insert BP Location - " + bpl.GetC_BPartner_Location_ID());
                                impBP.SetC_BPartner_Location_ID(bpl.GetC_BPartner_Location_ID());
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
                    MUser user = null;
                    if (impBP.GetAD_User_ID() != 0)
                    {
                        user = new MUser(GetCtx(), impBP.GetAD_User_ID(), Get_TrxName());
                        if (user.GetC_BPartner_ID() == 0)
                            user.SetC_BPartner_ID(bp.GetC_BPartner_ID());
                        else if (user.GetC_BPartner_ID() != bp.GetC_BPartner_ID())
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
                        if (impBP.GetC_Greeting_ID() != 0)
                            user.SetC_Greeting_ID(impBP.GetC_Greeting_ID());
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
                            user.SetC_BPartner_Location_ID(bpl.GetC_BPartner_Location_ID());
                        if (user.Save())
                        {
                            log.Finest("Update BP Contact - " + user.GetAD_User_ID());
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
                            user = new MUser(bp);
                            if (impBP.GetC_Greeting_ID() != 0)
                                user.SetC_Greeting_ID(impBP.GetC_Greeting_ID());
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
                                user.SetC_BPartner_Location_ID(bpl.GetC_BPartner_Location_ID());
                            if (user.Save())
                            {
                                log.Finest("Insert BP Contact - " + user.GetAD_User_ID());
                                impBP.SetAD_User_ID(user.GetAD_User_ID());
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
                        MContactInterest ci = MContactInterest.Get(GetCtx(),
                            impBP.GetR_InterestArea_ID(), user.GetAD_User_ID(),
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
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsert), "@C_BPartner_ID@: @Inserted@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noUpdate), "@C_BPartner_ID@: @Updated@");
            return "";
        }	//	doIt
    }

}

/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ImportContact
 * Purpose        : Import Contacts
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
    public class ImportContact : ProcessEngine.SvrProcess
    {
        /**	Client to be imported to		*/
        private int _AD_Client_ID = 0;
        /**	Delete old Imported				*/
        private bool _deleteOldImported = false;

        /// <summary>
        /// Prepare
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
        }	//	prepare

        /// <summary>
        /// process
        /// </summary>
        /// <returns>Info</returns>
        protected override String DoIt()
        {
            StringBuilder sql = null;
            int no = 0;
            String clientCheck = " AND AD_Client_ID=" + _AD_Client_ID;

            //	****	Prepare	****

            //	Delete Old Imported
            if (_deleteOldImported)
            {

                sql = new StringBuilder("DELETE FROM I_Contact "
                    + "WHERE I_IsImported='Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Delete Old Impored =" + no);
            }

            //	Set Client, Org, IsActive, Created/Updated
            sql = new StringBuilder("UPDATE I_Contact "
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

            //	Interest Area
            sql = new StringBuilder("UPDATE I_Contact i "
                + "SET R_InterestArea_ID=(SELECT R_InterestArea_ID FROM R_InterestArea ia "
                    + "WHERE i.InterestAreaName=ia.Name AND ia.AD_Client_ID=i.AD_Client_ID) "
                + "WHERE R_InterestArea_ID IS NULL AND InterestAreaName IS NOT NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Interest Area=" + no);


            int noProcessed = 0;
            String sql0 = "SELECT * FROM I_Contact "
                + "WHERE I_IsImported<>'Y' AND AD_Client_ID=@param ORDER BY I_Contact_ID";
            IDataReader idr = null;
            SqlParameter[] param = new SqlParameter[1];
            try
            {
                //pstmt = DataBase.prepareStatement (sql0, Get_TrxName());
                //pstmt.setInt (1, _AD_Client_ID);
                param[0] = new SqlParameter("@param", _AD_Client_ID);
                idr = DataBase.DB.ExecuteReader(sql0, param, Get_TrxName());
                while (idr.Read())
                {
                    if (this.Process(new X_I_Contact(GetCtx(), idr, Get_TrxName())))
                        noProcessed++;
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql0, e);
            }

            return "@Processed@ #" + noProcessed;
        }	//	doIt

        /// <summary>
        /// impoert process
        /// </summary>
        /// <param name="imp">import</param>
        /// <returns>true if processed</returns>
        private bool Process(X_I_Contact imp)
        {
            if (imp.GetEMail() == null || imp.GetEMail().Length == 0)
                return ProcessFail(imp, "No EMail");

            MUser user = MUser.Get(GetCtx(), imp.GetEMail(), Get_TrxName());
            //	New User
            if (user == null || user.GetAD_User_ID() == 0)
            {
                if (imp.IsEMailBounced())
                    return ProcessFail(imp, "No User found with email - cannou set Bounced flag");
                if (imp.GetContactName() == null || imp.GetContactName().Length == 0)
                    return ProcessFail(imp, "No Name for User/Contact");

                user = new MUser(GetCtx(), 0, Get_TrxName());
                user.SetName(imp.GetContactName());
                user.SetDescription(imp.GetContactDescription());
                user.SetEMail(imp.GetEMail());
            }
            //	Existing User
            else
            {
                if (imp.IsEMailBounced())
                {
                    user.SetIsEMailBounced(true);
                    user.SetBouncedInfo(imp.GetBouncedInfo());
                }
            }
            if (!user.Save())
                return ProcessFail(imp, "Cannot save User");

            //	Create BP
            if (imp.IsCreateBP())
            {
                if (user.GetC_BPartner_ID() == 0)
                {
                    MBPartner bp = new MBPartner(GetCtx(), 0, Get_TrxName());
                    bp.SetName(user.GetName());
                    if (!bp.Save())
                        return ProcessFail(imp, "Cannot create BPartner");
                    else
                    {
                        user.SetC_BPartner_ID(bp.GetC_BPartner_ID());
                        if (!user.Save())
                            return ProcessFail(imp, "Cannot update User");
                    }
                }
                imp.SetC_BPartner_ID(user.GetC_BPartner_ID());
            }

            //	Create Lead
            if (imp.IsCreateLead())
            {
                MLead lead = new MLead(GetCtx(), 0, Get_TrxName());
                lead.SetName(imp.GetContactName());
                lead.SetDescription(imp.GetContactDescription());
                lead.SetAD_User_ID(user.GetAD_User_ID());
                lead.SetC_BPartner_ID(user.GetC_BPartner_ID());
                lead.Save();
                imp.SetC_Lead_ID(lead.GetC_Lead_ID());
            }

            //	Interest Area
            if (imp.GetR_InterestArea_ID() != 0 && user != null)
            {
                MContactInterest ci = MContactInterest.Get(GetCtx(),
                    imp.GetR_InterestArea_ID(), user.GetAD_User_ID(),
                    true, Get_TrxName());
                ci.Save();		//	don't subscribe or re-activate
            }

            imp.SetAD_User_ID(user.GetAD_User_ID());
            imp.SetI_IsImported(true);
            imp.SetI_ErrorMsg(null);
            imp.Save();
            return true;
        }	//	process

        /// <summary>
        /// Process Fail
        /// </summary>
        /// <param name="imp">import</param>
        /// <param name="errorMsg">error message</param>
        /// <returns>false</returns>
        private bool ProcessFail(X_I_Contact imp, String errorMsg)
        {
            imp.SetI_IsImported(false);
            imp.SetI_ErrorMsg(errorMsg);
            imp.Save();

            return false;
        }	//	processFail

    }
}

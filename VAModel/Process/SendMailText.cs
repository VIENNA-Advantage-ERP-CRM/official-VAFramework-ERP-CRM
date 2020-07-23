/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : SendMailText
 * Purpose        : Send Mail to Interest Area Subscribers
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           03-Dec-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class SendMailText : ProcessEngine.SvrProcess
    {
        // What to send			
        private int _R_MailText_ID = -1;
        //	Mail Text				
        private MMailText _MailText = null;

        //	From (sender)			
        private int _AD_User_ID = -1;
        // Client Info				
        private MClient _client = null;
        //	From					
        private MUser _from = null;
        // Recipient List to prevent duplicate mails	
        //private ArrayList<Integer>	_list = new ArrayList<Integer>();
        private List<int> _list = new List<int>();

        private int _counter = 0;
        private int _errors = 0;
        //	To Subscribers 			
        private int _R_InterestArea_ID = -1;
        // Interest Area		
        private MInterestArea _ia = null;
        // To Customer Type		
        private int _C_BP_Group_ID = -1;
        // To Purchaser of Product
        //	comes here

        /// <summary>
        ///   Prepare - e.g., get Parameters.           
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
                else if (name.Equals("R_InterestArea_ID"))
                {
                    _R_InterestArea_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("R_MailText_ID"))
                {
                    _R_MailText_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_BP_Group_ID"))
                {
                    _C_BP_Group_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("AD_User_ID"))
                {
                    _AD_User_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }	//	prepare

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns> Message</returns>
        protected override String DoIt()
        {
            log.Info("R_MailText_ID=" + _R_MailText_ID);
            //	Mail Test
            _MailText = new MMailText(GetCtx(),_R_MailText_ID, Get_TrxName());
            if (_MailText.GetR_MailText_ID() == 0)
            {
                throw new Exception("Not found @R_MailText_ID@=" + _R_MailText_ID);
            }
            //	Client Info
            _client = MClient.Get(GetCtx());
            if (_client.GetAD_Client_ID() == 0)
            {
                throw new Exception("Not found @AD_Client_ID@");
            }
            if (_client.GetSmtpHost() == null || _client.GetSmtpHost().Length == 0)
            {
                throw new Exception("No SMTP Host found");
            }
            //
            if (_AD_User_ID > 0)
            {
                _from = new MUser(GetCtx(), _AD_User_ID, Get_TrxName());
                if (_from.GetAD_User_ID() == 0)
                {
                    throw new Exception("No found @AD_User_ID@=" + _AD_User_ID);
                }
            }
            log.Fine("From " + _from);
            //long start = System.currentTimeMillis();
            long start = CommonFunctions.CurrentTimeMillis();

            if (_R_InterestArea_ID > 0)
            {
                SendInterestArea();
            }
            if (_C_BP_Group_ID > 0)
            {
                SendBPGroup();
            }
            return "@Created@=" + _counter + ", @Errors@=" + _errors + " - "
                + (CommonFunctions.CurrentTimeMillis() - start) + "ms";
        }	//	doIt

        /// <summary>
        ///	Send to InterestArea
        /// </summary>
        private void SendInterestArea()
        {
            log.Info("R_InterestArea_ID=" + _R_InterestArea_ID);
            _ia = MInterestArea.Get(GetCtx(), _R_InterestArea_ID);
            String unsubscribe = null;
            if (_ia.IsSelfService())
            {
                unsubscribe = "\n\n---------.----------.----------.----------.----------.----------\n"
                    + Msg.GetElement(GetCtx(), "R_InterestArea_ID")
                    + ": " + _ia.GetName()
                    + "\n" + Msg.GetMsg(GetCtx(), "UnsubscribeInfo")
                    + "\n";
                MStore[] wstores = MStore.GetOfClient(_client);

                int index = 0;
                for (int i = 0; i < wstores.Length; i++)
                {
                    if (wstores[i].IsDefault())
                    {
                        index = i;
                        break;
                    }
                }
                if (wstores.Length > 0)
                    unsubscribe += wstores[index].GetWebContext(true);
            }

            //
            String sql = "SELECT u.Name, u.EMail, u.AD_User_ID "
                + "FROM R_ContactInterest ci"
                + " INNER JOIN AD_User u ON (ci.AD_User_ID=u.AD_User_ID) "
                + "WHERE ci.IsActive='Y' AND u.IsActive='Y'"
                + " AND ci.OptOutDate IS NULL"
                + " AND u.EMail IS NOT NULL"
                + " AND ci.R_InterestArea_ID=@param1";

            SqlParameter[] param = new SqlParameter[1];
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                param[0] = new SqlParameter("@param1", _R_InterestArea_ID);
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                foreach (DataRow dr in dt.Rows)
                {
                    Boolean ok = SendIndividualMail(Utility.Util.GetValueOfString(dr[0]),Utility.Util.GetValueOfInt( dr[2]), unsubscribe);
                    if (ok)
                    {
                        _counter++;
                    }
                    else
                    {
                        _errors++;
                    }
                }

            }
            
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                dt = null;
                idr.Close();
            }
        }	//	sendInterestArea


       /// <summary>
       /// 	Send to BPGroup
       /// </summary>
        private void SendBPGroup()
        {
            log.Info("C_BP_Group_ID=" + _C_BP_Group_ID);
            String sql = "SELECT u.Name, u.EMail, u.AD_User_ID "
                + "FROM AD_User u"
                + " INNER JOIN C_BPartner bp ON (u.C_BPartner_ID=bp.C_BPartner_ID) "
                + "WHERE u.IsActive='Y' AND bp.IsActive='Y'"
                + " AND u.EMail IS NOT NULL"
                + " AND bp.C_BP_Group_ID=@Param1";

            SqlParameter[] Param = new SqlParameter[1];
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                Param[0] = new SqlParameter("@Param1", _C_BP_Group_ID);
                idr = DataBase.DB.ExecuteReader(sql, Param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                foreach (DataRow dr in dt.Rows)
                {
                    Boolean ok = SendIndividualMail(Utility.Util.GetValueOfString(dr[0]), Utility.Util.GetValueOfInt(dr[2]), null);
                    if (ok == false)
                    {
                        ;
                    }
                    else if (ok == true)
                    {
                        _counter++;
                    }
                    else
                    {
                        _errors++;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                dt = null;
                idr.Close();
            }
            
        }	//	sendBPGroup

        /// <summary>
        ///	Send Individual Mail
        /// </summary>
        /// <param name="Name">user name</param>
        /// <param name="AD_User_ID">user</param>
        /// <param name="unsubscribe">unsubscribe message</param>
        /// <returns>true if mail has been sent</returns>
        private Boolean SendIndividualMail(String Name, int AD_User_ID, String unsubscribe)
        {
            //	Prevent two email
            int ii = AD_User_ID;
            if (_list.Contains(ii))
            {
                //return null;
                return false;
            }
            _list.Add(ii);
            //
            MUser to = new MUser(GetCtx(), AD_User_ID, null);
            if (to.IsEMailBounced())			//	ignore bounces
            {
                //return null;
                return false;
            }
            _MailText.SetUser(AD_User_ID);		//	parse context
            String message = _MailText.GetMailText(true);
            //	Unsubscribe
            if (unsubscribe != null)
            {
                message += unsubscribe;
            }
            //
            EMail email = _client.CreateEMail(_from, to, _MailText.GetMailHeader(), message);
            if (email == null)
            {
                //return Boolean.FALSE;
                return false;
            }
            if (_MailText.IsHtml())
            {
                email.SetMessageHTML(_MailText.GetMailHeader(), message);
            }
            else
            {
                email.SetSubject(_MailText.GetMailHeader());
                email.SetMessageText(message);
            }
            if (!email.IsValid() && !email.IsValid(true))
            {
                log.Warning("NOT VALID - " + email);
                to.SetIsActive(false);
                to.AddDescription("Invalid EMail");
                to.Save();
                //return Boolean.FALSE;
                return false;
            }
            Boolean OK = EMail.SENT_OK.Equals(email.Send());
            new MUserMail(_MailText, AD_User_ID, email).Save();
            if (OK)
            {
                log.Fine(to.GetEMail());
            }
            else
            {
                log.Warning("FAILURE - " + to.GetEMail());
            }
            AddLog(0, null, null, (OK ? "@OK@" : "@ERROR@") + " - " + to.GetEMail());
            return OK;
        }	

    }

}

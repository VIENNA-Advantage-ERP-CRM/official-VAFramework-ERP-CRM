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
//using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class SendMailText : ProcessEngine.SvrProcess
    {
        // What to send			
        private int _VAR_MailTemplate_ID = -1;
        //	Mail Text				
        private MMailText _MailText = null;

        //	From (sender)			
        private int _VAF_UserContact_ID = -1;
        // Client Info				
        private MVAFClient _client = null;
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
        private int _VAB_BPart_Category_ID = -1;
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
                else if (name.Equals("VAR_InterestArea_ID"))
                {
                    _R_InterestArea_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAR_MailTemplate_ID"))
                {
                    _VAR_MailTemplate_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAB_BPart_Category_ID"))
                {
                    _VAB_BPart_Category_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAF_UserContact_ID"))
                {
                    _VAF_UserContact_ID = para[i].GetParameterAsInt();
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
            log.Info("VAR_MailTemplate_ID=" + _VAR_MailTemplate_ID);
            //	Mail Test
            _MailText = new MMailText(GetCtx(),_VAR_MailTemplate_ID, Get_TrxName());
            if (_MailText.GetVAR_MailTemplate_ID() == 0)
            {
                throw new Exception("Not found @VAR_MailTemplate_ID@=" + _VAR_MailTemplate_ID);
            }
            //	Client Info
            _client = MVAFClient.Get(GetCtx());
            if (_client.GetVAF_Client_ID() == 0)
            {
                throw new Exception("Not found @VAF_Client_ID@");
            }
            if (_client.GetSmtpHost() == null || _client.GetSmtpHost().Length == 0)
            {
                throw new Exception("No SMTP Host found");
            }
            //
            if (_VAF_UserContact_ID > 0)
            {
                _from = new MUser(GetCtx(), _VAF_UserContact_ID, Get_TrxName());
                if (_from.GetVAF_UserContact_ID() == 0)
                {
                    throw new Exception("No found @VAF_UserContact_ID@=" + _VAF_UserContact_ID);
                }
            }
            log.Fine("From " + _from);
            //long start = System.currentTimeMillis();
            long start = CommonFunctions.CurrentTimeMillis();

            if (_R_InterestArea_ID > 0)
            {
                SendInterestArea();
            }
            if (_VAB_BPart_Category_ID > 0)
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
            log.Info("VAR_InterestArea_ID=" + _R_InterestArea_ID);
            _ia = MInterestArea.Get(GetCtx(), _R_InterestArea_ID);
            String unsubscribe = null;
            if (_ia.IsSelfService())
            {
                unsubscribe = "\n\n---------.----------.----------.----------.----------.----------\n"
                    + Msg.GetElement(GetCtx(), "VAR_InterestArea_ID")
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
            String sql = "SELECT u.Name, u.EMail, u.VAF_UserContact_ID "
                + "FROM VAR_InterestedUser ci"
                + " INNER JOIN VAF_UserContact u ON (ci.VAF_UserContact_ID=u.VAF_UserContact_ID) "
                + "WHERE ci.IsActive='Y' AND u.IsActive='Y'"
                + " AND ci.OptOutDate IS NULL"
                + " AND u.EMail IS NOT NULL"
                + " AND ci.VAR_InterestArea_ID=@param1";

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
            log.Info("VAB_BPart_Category_ID=" + _VAB_BPart_Category_ID);
            String sql = "SELECT u.Name, u.EMail, u.VAF_UserContact_ID "
                + "FROM VAF_UserContact u"
                + " INNER JOIN VAB_BusinessPartner bp ON (u.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID) "
                + "WHERE u.IsActive='Y' AND bp.IsActive='Y'"
                + " AND u.EMail IS NOT NULL"
                + " AND bp.VAB_BPart_Category_ID=@Param1";

            SqlParameter[] Param = new SqlParameter[1];
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                Param[0] = new SqlParameter("@Param1", _VAB_BPart_Category_ID);
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
        /// <param name="VAF_UserContact_ID">user</param>
        /// <param name="unsubscribe">unsubscribe message</param>
        /// <returns>true if mail has been sent</returns>
        private Boolean SendIndividualMail(String Name, int VAF_UserContact_ID, String unsubscribe)
        {
            //	Prevent two email
            int ii = VAF_UserContact_ID;
            if (_list.Contains(ii))
            {
                //return null;
                return false;
            }
            _list.Add(ii);
            //
            MUser to = new MUser(GetCtx(), VAF_UserContact_ID, null);
            if (to.IsEMailBounced())			//	ignore bounces
            {
                //return null;
                return false;
            }
            _MailText.SetUser(VAF_UserContact_ID);		//	parse context
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
            new MUserMail(_MailText, VAF_UserContact_ID, email).Save();
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

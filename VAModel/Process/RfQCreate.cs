/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : RfQCreate
 * Purpose        : Create RfQ Response from RfQ Topic
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     11-Aug.-2009
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
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class RfQCreate : ProcessEngine.SvrProcess
    {
        //Send RfQ				
        private bool _IsSendRfQ = false;
        //	RfQ						
        private int _C_RfQ_ID = 0;

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
                else if (name.Equals("IsSendRfQ"))
                {
                    _IsSendRfQ = "Y".Equals(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _C_RfQ_ID = GetRecord_ID();
        }

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message (translated text)</returns>
        protected override String DoIt()
        {
            MRfQ rfq = new MRfQ(GetCtx(), _C_RfQ_ID, Get_TrxName());
            log.Info("doIt - " + rfq + ", Send=" + _IsSendRfQ);
            ////ErrorLog.FillErrorLog("", "", "doIt - " + rfq + ", Send=" + _IsSendRfQ, VAdvantage.Framework.Message.MessageType.INFORMATION);
            String error = rfq.CheckQuoteTotalAmtOnly();
            if (error != null && error.Length > 0)
            {
                throw new Exception(error);
            }

            int counter = 0;
            int sent = 0;
            int notSent = 0;

            //	Get all existing responses
            MRfQResponse[] responses = rfq.GetResponses(false, false);

            //	Topic
            MRfQTopic topic = new MRfQTopic(GetCtx(), rfq.GetC_RfQ_Topic_ID(), Get_TrxName());
            MRfQTopicSubscriber[] subscribers = topic.GetSubscribers();
            for (int i = 0; i < subscribers.Length; i++)
            {
                MRfQTopicSubscriber subscriber = subscribers[i];
                bool skip = false;
                //	existing response
                for (int r = 0; r < responses.Length; r++)
                {
                    if (subscriber.GetC_BPartner_ID() == responses[r].GetC_BPartner_ID()
                        && subscriber.GetC_BPartner_Location_ID() == responses[r].GetC_BPartner_Location_ID())
                    {
                        skip = true;
                        break;
                    }
                }
                if (skip)
                {
                    continue;
                }

                //	Create Response
                MRfQResponse response = new MRfQResponse(rfq, subscriber);
                if (response.Get_ID() == 0)	//	no lines
                {
                    continue;
                }

                counter++;
                if (_IsSendRfQ)//send mail check
                {
                    if (response.SendRfQ())
                    {
                        sent++;
                    }
                    else
                    {
                        notSent++;
                    }
                }
            }	//	for all subscribers

            String retValue = "@Created@ " + counter;
            if (_IsSendRfQ)
            {
                retValue += " - @IsSendRfQ@=" + sent + " - @Error@=" + notSent;
            }
            return retValue;
        }
    }
}

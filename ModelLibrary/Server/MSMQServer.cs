using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Timers;
using System.Net;
using System.Net.Mail;
using System.Messaging;
using System.Web;
using System.IO;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Server
{
    public class MSMQServer
    {
        #region Private Variables

        Timer timer = null;//timer for queue
        string queuePath = @".\Private$\queue";
        private System.Messaging.MessageQueue queue = null;
        //System.Messaging.Message message = null;
        System.Net.Mail.MailMessage mailMsg = new System.Net.Mail.MailMessage();//send mail from queue

        string _mailServer = "";
        string _mailUser = "";
        string _mailPassword = "";
        string _from = "";

        //EMail mail = null;
        int _timeInterval = 10;//Intervat for 10 sec
        private static VLogger log = VLogger.GetVLogger(typeof(MSMQServer).FullName);

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MSMQServer(string mailServer, string mailUser, string mailPassword, string from)
        {
            try
            {
                _mailServer = mailServer;
                _mailUser = mailUser;
                _mailPassword = mailPassword;
                _from = from;

                timer = new Timer();
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                // Set the Interval to seconds.
                timer.Interval = (1000) * _timeInterval;
                timer.Enabled = true;
                queue = new MessageQueue(queuePath);
                queue.Formatter = new BinaryMessageFormatter();
                //Console.WriteLine("Press \'q\' to quit from MSMQ Server.");
                //while (Console.Read() != 'q') ;

            }
            catch
            {
            }
        }

        /// <summary>
        /// Timer Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (queue.GetAllMessages().Length > 0)
                {
                    timer.Enabled = false;
                    QueueMail();
                    timer.Enabled = true;//to start the time after completion of process
                }
            }
            catch (Exception ex)
            {
                log.SaveError(VAdvantage.Utility.Msg.GetMsg(Env.GetContext(), "QueueError", true), ex);
                timer.Enabled = false;
            }
        }

        /// <summary>
        /// Logic For recive and send Mail
        /// </summary>
        /// <summary>
        /// Logic For recive and send Mail
        /// </summary>
        private void QueueMail()
        {
            queue = new MessageQueue(queuePath);//initialize the queue path
            queue.Formatter = new BinaryMessageFormatter();//define queue formate
            //foreach (System.Messaging.Message msg in queue)//check all message in the message queue
            //{
            //    List<byte[]> listData = new List<byte[]>();//byte list of attachment
            //    List<string> listFile = new List<string>();//name list of attachment
            //    //serialised class
            //    VAdvantage.Utility.EmailMessageSerializable serializedMsg = new VAdvantage.Utility.EmailMessageSerializable();
            //    message = new System.Messaging.Message();//message initialization
            //    mailMsg = new System.Net.Mail.MailMessage();//mail class object
            //    message = queue.Receive();//recive message
            //    //new deserializ queue message
            //    serializedMsg = (VAdvantage.Utility.EmailMessageSerializable)message.Body;
            //    //set the content in mailmessage object 
            //    mailMsg.From = new System.Net.Mail.MailAddress(serializedMsg.From);//set from
            //    mailMsg.To.Add(serializedMsg.TO);//set to
            //    mailMsg.Subject = serializedMsg.Subject;//set Subject
            //    mailMsg.Body = serializedMsg.Message;//set text message

            //    listData = serializedMsg.Attachment;
            //    listFile = serializedMsg.FileName;

            //    mail = new VAdvantage.Utility.EMail(Utility.Env.GetCtx(), _mailServer, 25, true, mailMsg, listData, listFile);
            //    mail.CreateAuthenticator(_mailUser, _mailPassword);//set credentials

            //    for (int i = 0; i < serializedMsg.Bcc.Length; i++)
            //    {
            //        mail.AddBcc(serializedMsg.Bcc[i].ToString());//set Bcc for mail
            //    }
            //    for (int i = 0; i < serializedMsg.Cc.Length; i++)
            //    {
            //        mail.AddCc(serializedMsg.Cc[i].ToString());//Set Cc for mail
            //    }
            //    mail.SetMessageHTML(mailMsg.Body);
            //    //send mail
            //    if (!(mail.Send() == "OK"))
            //    {
            //        log.SaveError(Msg.GetMsg(Env.GetContext(), "NotSentMailTo", true), mail.GetTo().ToString());
            //    }
            //    else
            //    {
            //        log.Fine(Msg.GetMsg(Env.GetContext(), " SentEmailTo ", true).ToString() + mail.GetTo().ToString());
            //    }
            //}
        }
    }
}

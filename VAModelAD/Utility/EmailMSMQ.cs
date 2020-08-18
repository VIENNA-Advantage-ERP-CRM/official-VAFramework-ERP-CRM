using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using System.Net;
using System.Net.Mail;
using System.Xml;
using VAdvantage.DataBase;
using System.IO;
using VAdvantage.Utility;

namespace VAdvantage.Utility
{
    public class EmailMSMQ
    {
        #region PrivateVariables

        public MessageQueue queue;
        //byte[] data = null;
        System.Messaging.Message message = null;
        string queuePath = "FormatName:DIRECT=TCP:192.168.0.147\\private$\\queue";

        #endregion

        public EmailMSMQ()
        {
            //default constructor
        }

        /// <summary>
        /// Send Email from queue using xml format
        /// </summary>
        /// <param name="from">mail from</param>
        /// <param name="to">mail to</param>
        /// <param name="subject">Subject</param>
        /// <param name="message">text message</param>
        /// <param name="ip">Server IP Address</param>
        public EmailMSMQ(string from, string to, string subject, string textMessage, string ip)
        {
            queuePath = "FormatName:DIRECT=TCP:" + ip + "\\private$\\queue";
            string strXml;
            strXml = "<?xml version='1.0' encoding='UTF-8'?>";
            strXml += "<root>";
            strXml += "<From></From>";
            strXml += "<To></To>";
            strXml += "<Subject></Subject>";
            strXml += "<Message></Message>";
            // strXml += "<recipients></recipients>";
            strXml += "</root>";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(strXml);
            XmlNodeList nodeList = null;
            nodeList = xmlDoc.GetElementsByTagName("From");
            nodeList[0].InnerText = from;

            nodeList = xmlDoc.GetElementsByTagName("To");
            nodeList[0].InnerText = to;

            nodeList = xmlDoc.GetElementsByTagName("Subject");
            nodeList[0].InnerText = subject;

            nodeList = xmlDoc.GetElementsByTagName("Message");
            nodeList[0].InnerText = textMessage;

            MessageQueue.ClearConnectionCache();
            queue = new MessageQueue(queuePath);
            queue.Refresh();
            queue.Formatter = new XmlMessageFormatter(new string[] { "System.String" });// ActiveXMessageFormatter();
            queue.Send(xmlDoc.InnerXml);
        }

        /// <summary>
        /// Used to send message in the queue
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="textMessage"></param>
        /// <param name="ip"></param>
        /// <param name="attchmentList"></param>
        public EmailMSMQ(string from, string to, string subject, string textMessage, string ip, VAdvantage.Model.MAttachmentEntry[] attchmentList, string[] bccArray, string[] ccArray)
        {
            queuePath = "FormatName:DIRECT=TCP:" + ip + "\\private$\\queue";
            try
            {
                List<byte[]> listData = new List<byte[]>();
                List<string> listFile = new List<string>();
                for (int i = 0; i < attchmentList.Length; i++)
                {
                    //add attachment to list
                    listData.Add(attchmentList[i].GetData());
                    listFile.Add(attchmentList[i].GetName());
                }
                //call serializable class and set values
                EmailMessageSerializable mailMessage = new EmailMessageSerializable();
                mailMessage.TO = to;
                mailMessage.From = from;
                mailMessage.Subject = subject;
                mailMessage.Message = textMessage;
                mailMessage.Attachment = listData;
                mailMessage.FileName = listFile;
                mailMessage.Bcc = bccArray;
                mailMessage.Cc = ccArray;

                //message = new System.Messaging.Message(mailMessage);
                message = new System.Messaging.Message();
                message.Body = mailMessage;

                queue = new MessageQueue(queuePath);
                queue.Formatter = new BinaryMessageFormatter();//set message formate
                queue.DefaultPropertiesToSend.Priority = MessagePriority.High;
                queue.DefaultPropertiesToSend.Recoverable = true;//Recovrable message
                queue.Send(message.Body);
                queue.Close();
            }
            catch
            { }
        }
    }
}
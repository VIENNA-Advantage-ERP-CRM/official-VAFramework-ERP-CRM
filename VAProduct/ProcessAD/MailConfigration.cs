using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using VAdvantage.Logging;
//using InterIMAP;
using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class MailConfigration : ProcessEngine.SvrProcess 
    {
        // The Column				
        private int p_AD_UserMailConfigration_ID = 0;
        private string processMode = "";
//private IMAPConfig config = null;
        //private VLogger log = null;

        /// <summary>
        /// function to et parameters
        /// </summary>
        /// <returns>int</returns>
        /// 
        override protected void Prepare()
        {
            log=VLogger.GetVLogger(this.GetType().FullName);

            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameterName() == "ProcessMode")
                {
                    processMode = para[i].GetParameter().ToString();
                }
               else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            p_AD_UserMailConfigration_ID = GetRecord_ID();
        }//	prepare


        /// <summary>
        /// Alert table 
        /// </summary>
        /// <returns>int</returns>
        /// 
        override protected string DoIt()
        {
            

            log.Info("p_AD_UserMailConfigration_ID=" + p_AD_UserMailConfigration_ID);
            if (p_AD_UserMailConfigration_ID == 0)
            {
                //    return "";
                throw new Exception("@No@ @AD_Column_ID@");
            }
            //IDbTransaction trx = ExecuteQuery.GerServerTransaction();

            if (processMode == "D")
            {
                //return MailBox.Classes.MailConfigMethod.DeleteLocalUMConfig(GetCtx()().GetAD_User_ID());

            }

            if (processMode == "C")
            {
                MUserMailConfigration mailConfig = new MUserMailConfigration(GetCtx(), p_AD_UserMailConfigration_ID, Get_TrxName());
                if (mailConfig.Get_ID() == 0)
                {
                    
                    throw new Exception("@NotFound@ @AD_UserMailConfigration_ID@" + p_AD_UserMailConfigration_ID);
                }

                //Create new 

                return BindAndSaveIMAPConfigInfo(mailConfig);
            }
            return "";
        }
        //	doIt

        public string BindAndSaveIMAPConfigInfo(MUserMailConfigration mailConfig)
        {
            try
            {
                //string configFile = System.Windows.Forms.Application.StartupPath + "\\" + DataBase.GlobalVariable.IMAP_CONFIG_FILE + GetCtx()().GetAD_User_ID() + ".cfg";
                //config = new IMAPConfig();
                //config.Name = "";
                //config.Host = mailConfig.GetImapHost();
                //config.UserName = mailConfig.GetImapUsername();
                //config.Password = mailConfig.GetImapPassword();
                //config.UseSSL = mailConfig.IsImapIsSsl();
                //config.AutoLogon = mailConfig.IsAutoLogin();
                //config.SaveConfig(configFile);
                //return Msg.GetMsg(GetCtx(), "FileCreated");
                return "gg";
            }
            catch(Exception ex) 
           {
               return ex.Message;
            }

             //config.AutoGetMsgID;
             //config.DebugMode;
             //config.AutoSyncCache;
             //configChanged = false;
            

            //  rootFolderBox.Text = config.DefaultFolderName;
            //  localCacheBox.Text = config.CacheFile;
            //  logFileBox.Text = config.LogFile;

            //switch (config.DebugDetail)
            //{
            //    case IMAPConfig.DebugDetailLevel.All:
            //        {
            //            debugDetailBox.SelectedIndex = 1;
            //            break;
            //        }
            //    case IMAPConfig.DebugDetailLevel.ErrorsOnly:
            //        {
            //            debugDetailBox.SelectedIndex = 3;
            //            break;
            //        }
            //    case IMAPConfig.DebugDetailLevel.InterIMAPOnly:
            //        {
            //            debugDetailBox.SelectedIndex = 2;
            //            break;
            //        }
            //}

            //switch (config.Format)
            //{
            //    case CacheFormat.Binary:
            //        {
            //            binaryFormat.Checked = true;
            //            break;
            //        }
            //    case CacheFormat.XML:
            //        {
            //            xmlFormat.Checked = true;
            //            break;
            //        }
            //}


        }
    }
}

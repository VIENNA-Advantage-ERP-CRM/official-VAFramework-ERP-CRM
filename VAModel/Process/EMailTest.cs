/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : EMailTest
 * Purpose        : EMail Test process
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           03-feb-2010
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
    public class EMailTest : ProcessEngine.SvrProcess
    {
        /** Client Parameter			*/
        protected int _AD_Client_ID = 0;

        /// <summary>
        /// Get Parameters
        /// </summary>
        protected override void Prepare()
        {
            _AD_Client_ID = GetRecord_ID();
            if (_AD_Client_ID == 0)
            {
                
                _AD_Client_ID = GetCtx().GetAD_Client_ID();
            }
        }	//	prepare

        /// <summary>
        ///	Process - Test EMail
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            MClient client = MClient.Get(GetCtx(), _AD_Client_ID);
            log.Info(client.ToString());

            //	 Test Client Mail
            String clientTest = client.TestEMail();
            AddLog(0, null, null, client.GetName() + ": " + clientTest);

            //	Test Client DocumentDir
            if (!Ini.IsClient())
            {
                String documentDir = client.GetDocumentDir();
                if (documentDir == null || documentDir.Length == 0)
                    documentDir = ".";
                //File file = new File (documentDir);
                //System.IO.FileInfo  file= new System.IO.FileInfo(documentDir);
                System.IO.FileAttributes fil = new System.IO.FileAttributes();
                fil = System.IO.File.GetAttributes(documentDir);
                //if (file.Exists() && file.isDirectory())
                if ((fil & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory)
                {
                    AddLog(0, null, null, "Found Directory: " + client.GetDocumentDir());
                }
                else
                {
                    AddLog(0, null, null, "Not Found Directory: " + client.GetDocumentDir());
                }
            }

            MStore[] wstores = MStore.GetOfClient(client);
            for (int i = 0; i < wstores.Length; i++)
            {
                MStore store = wstores[i];
                String test = store.TestEMail();
                AddLog(0, null, null, store.GetName() + ": " + test);
            }

            return clientTest;
        }	//	doIt

    }	//	EMailTest
}

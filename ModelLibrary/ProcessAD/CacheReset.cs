/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Reset Cache
 * Purpose        : Reset Cache
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     21-Sep-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Logging;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;
using System.Threading;

namespace VAdvantage.Process
{
    public class CacheReset : ProcessEngine.SvrProcess
    {
        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
        }


        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message to be translated</returns>
        protected override String DoIt()
        {
            log.Info("");
           // Env.Reset(false);	// not final            

            Thread t1 = new Thread(thread1);
            t1.Start();

            Thread t2 = new Thread(thread2);
            t2.Start();

            Thread t3 = new Thread(thread3);
            t3.Start();

            Thread t4 = new Thread(thread4);
            t4.Start();

            Thread t5 = new Thread(thread5);
            t5.Start();


            return "";

        }

        private void thread1()
        {
            for (int i = 0; i < 10000; i++)
            {
                int AD_Process_ID = 1000366;  // HARDCODED    C_InvoiceCreate
                MPInstance instance = new MPInstance(GetCtx(), AD_Process_ID, 0);
                if (!instance.Save())
                {
                    //lblStatusInfo = Msg.GetMsg(GetCtx(), "ProcessNoInstance");
                   // return Msg.GetMsg(GetCtx(), "ProcessNoInstance");
                }

                ProcessInfo pi = new ProcessInfo("", AD_Process_ID);
                pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());

                pi.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                ProcessCtl worker = new ProcessCtl(GetCtx(), null, pi, Get_TrxName());
                worker.Run();
                //Thread.Sleep(10000);
            }
        }

        private void thread2()
        {
            for (int i = 0; i < 10000; i++)
            {
                int AD_Process_ID = 1000366;  // HARDCODED    C_InvoiceCreate
                MPInstance instance = new MPInstance(GetCtx(), AD_Process_ID, 0);
                if (!instance.Save())
                {
                    //lblStatusInfo = Msg.GetMsg(GetCtx(), "ProcessNoInstance");
                    // return Msg.GetMsg(GetCtx(), "ProcessNoInstance");
                }

                ProcessInfo pi = new ProcessInfo("", AD_Process_ID);
                pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());

                pi.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                ProcessCtl worker = new ProcessCtl(GetCtx(), null, pi, Get_TrxName());
                worker.Run();
                //Thread.Sleep(10000);
            }
        }

        private void thread3()
        {
            for (int i = 0; i < 10000; i++)
            {
                int AD_Process_ID = 1000366;  // HARDCODED    C_InvoiceCreate
                MPInstance instance = new MPInstance(GetCtx(), AD_Process_ID, 0);
                if (!instance.Save())
                {
                    //lblStatusInfo = Msg.GetMsg(GetCtx(), "ProcessNoInstance");
                    // return Msg.GetMsg(GetCtx(), "ProcessNoInstance");
                }

                ProcessInfo pi = new ProcessInfo("", AD_Process_ID);
                pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());

                pi.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                ProcessCtl worker = new ProcessCtl(GetCtx(), null, pi, Get_TrxName());
                worker.Run();
                //Thread.Sleep(5000);
            }
        }

        private void thread4()
        {
            for (int i = 0; i < 10000; i++)
            {
                int AD_Process_ID = 1000366;  // HARDCODED    C_InvoiceCreate
                MPInstance instance = new MPInstance(GetCtx(), AD_Process_ID, 0);
                if (!instance.Save())
                {
                    //lblStatusInfo = Msg.GetMsg(GetCtx(), "ProcessNoInstance");
                    // return Msg.GetMsg(GetCtx(), "ProcessNoInstance");
                }

                ProcessInfo pi = new ProcessInfo("", AD_Process_ID);
                pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());

                pi.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                ProcessCtl worker = new ProcessCtl(GetCtx(), null, pi, Get_TrxName());
                worker.Run();
                //Thread.Sleep(20000);
            }
        }

        private void thread5()
        {
            for (int i = 0; i < 10000; i++)
            {
                int AD_Process_ID = 1000366;  // HARDCODED    C_InvoiceCreate
                MPInstance instance = new MPInstance(GetCtx(), AD_Process_ID, 0);
                if (!instance.Save())
                {
                    //lblStatusInfo = Msg.GetMsg(GetCtx(), "ProcessNoInstance");
                    // return Msg.GetMsg(GetCtx(), "ProcessNoInstance");
                }

                ProcessInfo pi = new ProcessInfo("", AD_Process_ID);
                pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());

                pi.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                ProcessCtl worker = new ProcessCtl(GetCtx(), null, pi, Get_TrxName());
                worker.Run();
                //Thread.Sleep(10000);
            }
        }
    }
}

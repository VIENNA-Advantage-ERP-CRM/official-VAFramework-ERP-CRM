/********************************************************
 * Module Name    :     Report
 * Purpose        :     Generate Reports
 * Author         :     Jagmohan Bhatt
 * Date           :     13-July-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Model;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using System.IO;
using VAdvantage.Process;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;
namespace VAdvantage.Print
{
    /// <summary>
    /// Report Controller.
    /// </summary>
    public class ReportCtl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ReportCtl()
        {
        }	//	ReportCtrl

        /**	Static Logger	*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(ReportCtl).FullName);

        //get report from work flow
        volatile static IReportEngine report = null;

        public static IReportEngine Report
        {
            get
            {
                return report;
            }
            set
            {
                report = value;
            }
        }

        /// <summary>
        /// Create Report.
        /// Called from ProcessCtl.
        /// - Check special reports first, if not, create standard Report
        /// </summary>
        /// <param name="ctx">process info</param>
        /// <param name="pi">if true, prints directly - otherwise View</param>
        /// <param name="IsDirectPrint">true if created</param>
        /// <returns></returns>
        static public ReportEngine_N Start(Ctx ctx, ProcessInfo pi, bool IsDirectPrint)
        {
            s_log.Info("" + pi);
            /*****************Manfacturing*******************/
            int AD_ProcessWorkOrder_ID = Convert.ToInt32(DB.ExecuteScalar("select ad_process_id from ad_process where name='Work Order Print'"));
            int AD_ProcessWorkOrderTrxn_ID = Convert.ToInt32(DB.ExecuteScalar("select ad_process_id from ad_process where name ='Work Order Transaction Print'"));
            int AD_ProcessStandardopration_ID = Convert.ToInt32(DB.ExecuteScalar("select ad_process_id from ad_process where name='Standard Operation Print'"));
            int AD_ProcessRouting_ID = Convert.ToInt32(DB.ExecuteScalar("select ad_process_id from ad_process where name='Routing Print'"));
            /*****************Manfacturing*******************/


            /**
             *	Order Print
             */
            if (pi.GetAD_Process_ID() == 110)			//	C_Order
                return StartDocumentPrint(ctx, ReportEngine_N.ORDER, pi.GetRecord_ID(), IsDirectPrint);
            else if (pi.GetAD_Process_ID() == 116)		//	C_Invoice
                return StartDocumentPrint(ctx, ReportEngine_N.INVOICE, pi.GetRecord_ID(), IsDirectPrint);
            else if (pi.GetAD_Process_ID() == 117)		//	M_InOut
                return StartDocumentPrint(ctx, ReportEngine_N.SHIPMENT, pi.GetRecord_ID(), IsDirectPrint);
            else if (pi.GetAD_Process_ID() == 217)		//	C_Project
                return StartDocumentPrint(ctx, ReportEngine_N.PROJECT, pi.GetRecord_ID(), IsDirectPrint);
            else if (pi.GetAD_Process_ID() == 276)		//	C_RfQResponse
                return StartDocumentPrint(ctx, ReportEngine_N.RFQ, pi.GetRecord_ID(), IsDirectPrint);
            else if (pi.GetAD_Process_ID() == 313)		//	C_Payment
                return StartCheckPrint(ctx, pi.GetRecord_ID(), IsDirectPrint);
            else if (pi.GetAD_Process_ID() == 290)      // 	Movement
                return StartDocumentPrint(ctx, ReportEngine_N.MOVEMENT, pi.GetRecord_ID(), IsDirectPrint);
            else if (pi.GetAD_Process_ID() == 291)		//	Inventory
                return StartDocumentPrint(ctx, ReportEngine_N.INVENTORY, pi.GetRecord_ID(), IsDirectPrint);
            /*****************Manfacturing*******************/

            else if (pi.GetAD_Process_ID() == AD_ProcessWorkOrder_ID)		//	M_WorkOrder415
            {
                return StartDocumentPrint(ctx, ReportEngine_N.WORKORDER, pi.GetRecord_ID(), IsDirectPrint);
            }
            else if (pi.GetAD_Process_ID() == AD_ProcessWorkOrderTrxn_ID)		//	M_WorkOrderTransaction 1481
                return StartDocumentPrint(ctx, ReportEngine_N.WORKORDERTXN, pi.GetRecord_ID(), IsDirectPrint);
            else if (pi.GetAD_Process_ID() == AD_ProcessStandardopration_ID)		//	M_StandardOperation//1504
                return StartDocumentPrint(ctx, ReportEngine_N.STANDARDOPERATION, pi.GetRecord_ID(), IsDirectPrint);
            else if (pi.GetAD_Process_ID() == AD_ProcessRouting_ID)		//	M_Routing//1503
                return StartDocumentPrint(ctx, ReportEngine_N.ROUTING, pi.GetRecord_ID(), IsDirectPrint);
            /*****************Manfacturing*******************/
            /**
            else if (pi.AD_Process_ID == 9999999)	//	PaySelection
                return startDocumentPrint(CHECK, pi, IsDirectPrint);
            else if (pi.AD_Process_ID == 9999999)	//	PaySelection
                return startDocumentPrint(REMITTANCE, pi, IsDirectPrint);
            **/
            else if (pi.GetAD_Process_ID() == 159)		//	Dunning
                return StartDocumentPrint(ctx, ReportEngine_N.DUNNING, pi.GetRecord_ID(), IsDirectPrint);
            else if (pi.GetAD_Process_ID() == 202			//	Financial Report
                || pi.GetAD_Process_ID() == 204)			//	Financial Statement
                return StartFinReport(ctx, pi);
            /********************
             *	Standard Report
             *******************/
            return StartStandardReport(ctx, pi, IsDirectPrint);
        }	//	create

        /// <summary>
        /// Start Standard Report.
        /// - Get Table Info & submit
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="pi">Process Info</param>
        /// <param name="IsDirectPrint">if true, prints directly - otherwise View</param>
        /// <returns>true if OK</returns>
        static public ReportEngine_N StartStandardReport(Ctx ctx, ProcessInfo pi, bool IsDirectPrint)
        {
            ReportEngine_N re = ReportEngine_N.Get(ctx, pi);
            if (re == null)
            {
                pi.SetSummary("");//No ReportEngine_N");
                s_log.Log(Level.SEVERE, "No ReportEngine_N");
                return null;
            }
            if (IsDirectPrint)
            {
                re.Print();
            }
            return re;
        }	//	startStandardReport


        public static ReportEngine_N StartDocumentPrint(Ctx ctx, int type, int Record_ID, bool IsDirectPrint)
        {
            ReportEngine_N re = ReportEngine_N.Get(ctx, type, Record_ID);
            if (re == null)
            {
                //ShowMessage.Error("NoDocPrintFormat", true);
                return null;
            }
            if (IsDirectPrint)
            {
                re.Print();
                ReportEngine_N.PrintConfirm(type, Record_ID);
            }
            return re;
            //return StartDocumentPrint(ctx, type, Record_ID, IsDirectPrint, true);
        }	//	StartDocumentPrint


        static public ReportEngine_N StartFinReport(Ctx ctx, ProcessInfo pi)
        {
            int AD_Client_ID = ctx.GetAD_Client_ID();

            //  Create Query from Parameters
            String TableName = pi.GetAD_Process_ID() == 202 ? "T_Report" : "T_ReportStatement";
            Query query = Query.Get(ctx, pi.GetAD_PInstance_ID(), TableName);

            //	Get PrintFormat
            MPrintFormat format = (MPrintFormat)pi.GetTransientObject();
            if (format == null)
                format = (MPrintFormat)pi.GetSerializableObject();
            if (format == null)
            {
                s_log.Log(Level.SEVERE, "startFinReport - No PrintFormat");
                return null;
            }
            PrintInfo info = new PrintInfo(pi);

            ReportEngine_N re = new ReportEngine_N(ctx, format, query, info);
            return re;
        }	//	startFinReport

        /// <summary>
        /// Start Document Print for Type. Called also directly from ProcessDialog, VInOutGen, VInvoiceGen, VPayPrint
        /// </summary>
        /// <param name="ctx">document type in ReportEngine</param>
        /// <param name="type"></param>
        /// <param name="Record_ID"></param>
        /// <param name="IsDirectPrint">if true, prints directly - otherwise View</param>
        /// <param name="IsShowError"></param>
        /// <returns>true if success</returns>
        public static ReportEngine_N StartDocumentPrint(Ctx ctx, int type, int Record_ID, bool IsDirectPrint, bool IsShowError)
        {
            ReportEngine_N re = ReportEngine_N.Get(ctx, type, Record_ID);
            if (re == null)
            {
                //if (IsShowError)
                //    ShowMessage.Error("NoDocPrintFormat", true);
                return null;
            }
            if (IsDirectPrint)
            {
                re.Print();
                ReportEngine_N.PrintConfirm(type, Record_ID);
            }
            return re;
        }	//	StartDocumentPrint


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_Payment_ID"></param>
        /// <param name="IsDirectPrint"></param>
        /// <returns></returns>
        public static ReportEngine_N StartCheckPrint(Ctx ctx, int C_Payment_ID, bool IsDirectPrint)
        {
            int C_PaySelectionCheck_ID = 0;
            MPaySelectionCheck psc = MPaySelectionCheck.GetOfPayment(ctx, C_Payment_ID, null);
            if (psc != null)
            {
                C_PaySelectionCheck_ID = psc.GetC_PaySelectionCheck_ID();
            }
            else
            {
                psc = MPaySelectionCheck.CreateForPayment(ctx, C_Payment_ID, null);
                if (psc != null)
                    C_PaySelectionCheck_ID = psc.GetC_PaySelectionCheck_ID();
            }
            return StartDocumentPrint(ctx, ReportEngine_N.CHECK, C_PaySelectionCheck_ID, IsDirectPrint);
        }	//	startCheckPrint
    }
}

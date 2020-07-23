/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : RequestInvoice
 * Purpose        : Create Invoices for Requests
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           18-Nov-2009
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


using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class RequestInvoice : ProcessEngine.SvrProcess
    {
        // Request Type				
        private int _R_RequestType_ID = 0;
        //	Request Group (opt)		
        private int _R_Group_ID = 0;
        // Request Categpry (opt)		
        private int _R_Category_ID = 0;
        // Business Partner (opt)		
        private int _C_BPartner_ID = 0;
        // Default product				
        private int _M_Product_ID = 0;

        // The invoice					
        private MInvoice _m_invoice = null;
        //	Line Count				
        private int _m_linecount = 0;
        // Return Msg
        private string _msg = string.Empty;

        /// <summary>
        ///	Prepare
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
                else if (name.Equals("R_RequestType_ID"))
                {
                    _R_RequestType_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("R_Group_ID"))
                {
                    _R_Group_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("R_Category_ID"))
                {
                    _R_Category_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("M_Product_ID"))
                {
                    _M_Product_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }	//	prepare

        /// <summary>
        ///	Process
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {

            int lenth = 1;
            log.Info("R_RequestType_ID=" + _R_RequestType_ID + ", R_Group_ID=" + _R_Group_ID
                + ", R_Category_ID=" + _R_Category_ID + ", C_BPartner_ID=" + _C_BPartner_ID
                + ", _M_Product_ID=" + _M_Product_ID);

            MRequestType type = MRequestType.Get(GetCtx(), _R_RequestType_ID);
            if (type.Get_ID() == 0)
            {
                throw new Exception("@R_RequestType_ID@ @NotFound@ " + _R_RequestType_ID);
            }

            if (!type.IsInvoiced())
            {
                throw new Exception("@R_RequestType_ID@ <> @IsInvoiced@");
            }

            String sql = "SELECT * FROM R_Request r"
                + " INNER JOIN R_Status s ON (r.R_Status_ID=s.R_Status_ID) "
                + "WHERE s.IsClosed='Y'"
                + " AND r.C_Invoice_ID IS null"
                + " AND r.R_RequestType_ID=@Param1";
            if (_R_Group_ID != 0 && _R_Group_ID != -1)
            {
                sql += " AND r.R_Group_ID=@Param2";
                lenth = lenth + 1;

            }
            if (_R_Category_ID != 0 && _R_Category_ID != -1)
            {
                sql += " AND r.R_Category_ID=@Param3";
                lenth = lenth + 1;
            }
            if (_C_BPartner_ID != 0)
            {
                sql += " AND r.C_BPartner_ID=@Param4";
                lenth = lenth + 1;
            }
            sql += " AND r.IsInvoiced='Y' "
                + "ORDER BY C_BPartner_ID";
            SqlParameter[] Param = new SqlParameter[lenth];
            IDataReader idr = null;
            DataTable dt = null;

            try
            {
                int index = 0;
                Param[index] = new SqlParameter("@Param1", _R_RequestType_ID);
                if (_R_Group_ID != 0 && _R_Group_ID != -1)
                {
                    index++;
                    Param[index] = new SqlParameter("@Param2", _R_Group_ID);
                }
                if (_R_Category_ID != 0 && _R_Category_ID != -1)
                {
                    index++;
                    Param[index] = new SqlParameter("@Param3", _R_Category_ID);
                }
                if (_C_BPartner_ID != 0)
                {
                    index++;
                    Param[index] = new SqlParameter("@Param4", _C_BPartner_ID);
                }
                idr = DataBase.DB.ExecuteReader(sql, Param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                int oldC_BPartner_ID = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    MRequest request = new MRequest(GetCtx(), dr, Get_TrxName());
                    if (!request.IsInvoiced())
                    {
                        continue;
                    }
                    if (oldC_BPartner_ID != request.GetC_BPartner_ID())
                    {
                        InvoiceDone();
                    }
                    if (_m_invoice == null)
                    {
                        InvoiceNew(request);
                        oldC_BPartner_ID = request.GetC_BPartner_ID();
                    }
                    InvoiceLine(request);
                }
                InvoiceDone();
                //
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }
            if (string.IsNullOrEmpty(_msg))
            {
                _msg = "No Invoice Generated";
            }
            //	R_Category_ID
            return _msg;
        }	//	doIt

        /// <summary>
        ///	Done with Invoice
        /// </summary>
        private void InvoiceDone()
        {
            //	Close Old
            if (_m_invoice != null)
            {
                if (_m_linecount == 0)
                {
                    _m_invoice.Delete(false);
                }
                else
                {
                    //_m_invoice.ProcessIt(MInvoice.ACTION_Prepare);
                    _m_invoice.ProcessIt(DocActionVariables.ACTION_PREPARE);
                    _m_invoice.Save();
                    AddLog(0, null, _m_invoice.GetGrandTotal(), _m_invoice.GetDocumentNo());
                }
            }
            _m_invoice = null;
        }	//	invoiceDone

        /// <summary>
        ///	New Invoice
        /// </summary>
        /// <param name="request">request</param>
        private void InvoiceNew(MRequest request)
        {
            _m_invoice = new MInvoice(GetCtx(), 0, Get_TrxName());
            _m_invoice.SetC_DocTypeTarget_ID(MDocBaseType.DOCBASETYPE_ARINVOICE);
            MBPartner partner = new MBPartner(GetCtx(), request.GetC_BPartner_ID(), Get_TrxName());
            _m_invoice.SetBPartner(partner);
            _m_invoice.SetM_PriceList_ID(partner.GetM_PriceList_ID());
            int _CountVA009 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
            if (_CountVA009 > 0)
            {
                _m_invoice.SetVA009_PaymentMethod_ID(partner.GetVA009_PaymentMethod_ID());
            }
            _m_invoice.Save();
            request.SetC_Invoice_ID(_m_invoice.GetC_Invoice_ID());
            request.Save();
            if (string.IsNullOrEmpty(_msg))
            {
                _msg = "Invoice-[ID:" + _m_invoice.Get_ID() + "] created";
            }
            else
            {
                _msg += ",Invoice-[ID: " + _m_invoice.Get_ID() + "] created";
            }
            //_m_linecount = 0;
        }	//	invoiceNew

        /// <summary>
        /// Invoice Line
        /// </summary>
        /// <param name="request">request</param>
        private void InvoiceLine(MRequest request)
        {
            MRequestUpdate[] updates = request.GetUpdatedRecord(null);

            for (int i = 0; i < updates.Length; i++)
            {
                Decimal qty = updates[i].GetQtyInvoiced();
                if (Env.Signum(qty) == 0)
                {
                    continue;
                }
                MInvoiceLine il = new MInvoiceLine(_m_invoice);
                _m_linecount++;
                il.SetLine(_m_linecount * 10);
                //
                il.SetQty(qty);
                //	Product
                int M_Product_ID = updates[i].GetM_ProductSpent_ID();
                if (M_Product_ID == 0)
                {
                    M_Product_ID = _M_Product_ID;
                }
                il.SetM_Product_ID(M_Product_ID);
                //
                //il.SetPrice();
                il.SetPrice(true);
                if (!il.Save())
                {
                }
                //il.Save();
            }
        }	//	invoiceLine

    }	//	RequestInvoice

}
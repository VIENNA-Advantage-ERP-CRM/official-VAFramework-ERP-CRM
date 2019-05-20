/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRfQResponse
 * Purpose        : RfQ Response Model
 * Class Used     : X_C_RfQResponse
 * Chronological    Development
 * Raghunandan     10-Aug.-2009
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
using System.IO;
using VAdvantage.Logging;
namespace VAdvantage.Model
{
    public class MRfQResponse : X_C_RfQResponse
    {
        //	underlying RfQ				
        private MRfQ _rfq = null;
        // Lines						
        private MRfQResponseLine[] _lines = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_RfQResponse_ID"></param>
        /// <param name="trxName"></param>
        public MRfQResponse(Ctx ctx, int C_RfQResponse_ID, Trx trxName)
            : base(ctx, C_RfQResponse_ID, trxName)
        {
            if (C_RfQResponse_ID == 0)
            {
                SetIsComplete(false);
                SetIsSelectedWinner(false);
                SetIsSelfService(false);
                SetPrice(Env.ZERO);
                SetProcessed(false);
                SetProcessing(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MRfQResponse(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="rfq"></param>
        /// <param name="subscriber"></param>
        public MRfQResponse(MRfQ rfq, MRfQTopicSubscriber subscriber)
            : this(rfq, subscriber,
                subscriber.GetC_BPartner_ID(),
                subscriber.GetC_BPartner_Location_ID(),
                subscriber.GetAD_User_ID())
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="rfq">rfq</param>
        /// <param name="partner">web response</param>
        public MRfQResponse(MRfQ rfq, MBPartner partner)
            : this(rfq, null,
                partner.GetC_BPartner_ID(),
                partner.GetPrimaryC_BPartner_Location_ID(),
                partner.GetPrimaryAD_User_ID())
        {

        }

        /// <summary>
        /// Parent Constructor.
        /// Automatically saved if lines were created 
        /// Saved automatically 
        /// @param rfq 
        /// </summary>
        /// <param name="rfq">rfq</param>
        /// <param name="subscriber">optional subscriber</param>
        /// <param name="C_BPartner_ID">bpartner</param>
        /// <param name="C_BPartner_Location_ID">bpartner location</param>
        /// <param name="AD_User_ID">bpartner user</param>
        public MRfQResponse(MRfQ rfq, MRfQTopicSubscriber subscriber,
            int C_BPartner_ID, int C_BPartner_Location_ID, int AD_User_ID)
            : this(rfq.GetCtx(), 0, rfq.Get_TrxName())
        {

            SetClientOrg(rfq);
            SetC_RfQ_ID(rfq.GetC_RfQ_ID());
            SetC_Currency_ID(rfq.GetC_Currency_ID());
            SetName(rfq.GetName());
            _rfq = rfq;
            //	Subscriber info
            SetC_BPartner_ID(C_BPartner_ID);
            SetC_BPartner_Location_ID(C_BPartner_Location_ID);
            SetAD_User_ID(AD_User_ID);

            //	Create Lines
            MRfQLine[] lines = rfq.GetLines();
            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].IsActive())
                    continue;

                //	Product on "Only" list
                if (subscriber != null
                    && !subscriber.IsIncluded(lines[i].GetM_Product_ID()))
                {
                    continue;
                }
                //
                if (Get_ID() == 0)	//	save Response
                {
                    Save();
                }

                MRfQResponseLine line = new MRfQResponseLine(this, lines[i]);
                //	line is not saved (dumped) if there are no Qtys 
            }
        }

        /// <summary>
        /// Get Response Lines
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>array of Response Lines</returns>
        public MRfQResponseLine[] GetLines(bool requery)
        {
            if (_lines != null && !requery)
            {
                return _lines;
            }
            List<MRfQResponseLine> list = new List<MRfQResponseLine>();
            String sql = "SELECT * FROM C_RfQResponseLine "
                + "WHERE C_RfQResponse_ID=" + GetC_RfQResponse_ID() + " AND IsActive='Y'";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MRfQResponseLine(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "getLines", e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
                idr.Close();
            }

            _lines = new MRfQResponseLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /// <summary>
        /// Get Response Lines (no requery)
        /// </summary>
        /// <returns>array of Response Lines</returns>
        public MRfQResponseLine[] GetLines()
        {
            return GetLines(false);
        }

        /// <summary>
        /// 	Get RfQ
        /// </summary>
        /// <returns>rfq</returns>
        public MRfQ GetRfQ()
        {
            if (_rfq == null)
            {
                _rfq = MRfQ.Get(GetCtx(), GetC_RfQ_ID(), Get_TrxName());
            }
            return _rfq;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRfQResponse[");
            sb.Append(Get_ID())
                .Append(",Complete=").Append(IsComplete())
                .Append(",Winner=").Append(IsSelectedWinner())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// 	Send RfQ
        /// </summary>
        /// <returns>true if RfQ is sent per email.</returns>
        public bool SendRfQ()
        {
            try
            {
                MUser to = MUser.Get(GetCtx(), GetAD_User_ID());
                if (to.Get_ID() == 0 || to.GetEMail() == null || to.GetEMail().Length == 0)
                {
                    log.Log(Level.SEVERE, "No User or no EMail - " + to);
                    return false;
                }
                MClient client = MClient.Get(GetCtx());
                //
                String message = GetDescription();
                if (message == null || message.Length == 0)
                {
                    message = GetHelp();
                }
                else if (GetHelp() != null)
                {
                    message += "\n" + GetHelp();
                }
                if (message == null)
                {
                    message = GetName();
                }
                //
                EMail email = client.CreateEMail(to.GetEMail(), to.GetName(), "RfQ: " + GetName(), message);
                if (email == null)
                {
                    return false;
                }
                email.AddAttachment(CreatePDF());
                if (EMail.SENT_OK.Equals(email.Send()))
                {
                    //SetDateInvited(new Timestamp(System.currentTimeMillis()));
                    SetDateInvited(DateTime.Now);
                    Save();
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Severe(ex.ToString());
                //MessageBox.Show("error--" + ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// Create PDF file
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            return CreatePDF(null);
        }

        /// <summary>
        /// Create PDF file
        /// </summary>
        /// <param name="file">output file</param>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF(FileInfo file)
        {
            //ReportEngine re = ReportEngine.get(getCtx(), ReportEngine.RFQ, getC_RfQResponse_ID());
            //if (re == null)
            //   return null;
            //return re.getPDF(file);
            return file;
        }

        /// <summary>
        /// Check if Response is Complete
        /// </summary>
        /// <returns>null if complere - error message otherwise</returns>
        public String CheckComplete()
        {
            if (IsComplete())
            {
                SetIsComplete(false);
            }
            MRfQ rfq = GetRfQ();

            //	Is RfQ Total valid
            String error = rfq.CheckQuoteTotalAmtOnly();
            if (error != null && error.Length > 0)
            {
                return error;
            }

            //	Do we have Total Amount ?
            if (rfq.IsQuoteTotalAmt() || rfq.IsQuoteTotalAmtOnly())
            {
                Decimal amt = GetPrice();
                if (Env.ZERO.CompareTo(amt) >= 0)
                {
                    return "No Total Amount";
                }
            }

            //	Do we have an amount/qty for all lines
            if (rfq.IsQuoteAllLines())
            {
                MRfQResponseLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MRfQResponseLine line = lines[i];
                    if (!line.IsActive())
                        return "Line " + line.GetRfQLine().GetLine()
                            + ": Not Active";
                    bool validAmt = false;
                    MRfQResponseLineQty[] qtys = line.GetQtys(false);
                    for (int j = 0; j < qtys.Length; j++)
                    {
                        MRfQResponseLineQty qty = qtys[j];
                        if (!qty.IsActive())
                        {
                            continue;
                        }
                        Decimal? amt = qty.GetNetAmt();
                        if ( Env.ZERO.CompareTo(amt) < 0)
                        {
                            validAmt = true;
                            break;
                        }
                    }
                    if (!validAmt)
                    {
                        return "Line " + line.GetRfQLine().GetLine()
                            + ": No Amount";
                    }
                }
            }

            //	Do we have an amount for all line qtys
            if (rfq.IsQuoteAllQty())
            {
                MRfQResponseLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MRfQResponseLine line = lines[i];
                    MRfQResponseLineQty[] qtys = line.GetQtys(false);
                    for (int j = 0; j < qtys.Length; j++)
                    {
                        MRfQResponseLineQty qty = qtys[j];
                        if (!qty.IsActive())
                            return "Line " + line.GetRfQLine().GetLine()
                            + " Qty=" + qty.GetRfQLineQty().GetQty()
                            + ": Not Active";
                        Decimal? amt = qty.GetNetAmt();
                        if (amt == null || Env.ZERO.CompareTo(amt) >= 0)
                        {
                            return "Line " + line.GetRfQLine().GetLine()
                                 + " Qty=" + qty.GetRfQLineQty().GetQty()
                                 + ": No Amount";
                        }
                    }
                }
            }

            SetIsComplete(true);
            return null;
        }

        /// <summary>
        /// Is Quote Total Amt Only
        /// </summary>
        /// <returns>true if only Total</returns>
        public bool IsQuoteTotalAmtOnly()
        {
            return GetRfQ().IsQuoteTotalAmtOnly();
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Calculate Complete Date (also used to verify)
            if (GetDateWorkStart() != null && GetDeliveryDays() != 0)
            {
                SetDateWorkComplete(TimeUtil.AddDays(GetDateWorkStart(), GetDeliveryDays()));
            }
            //	Calculate Delivery Days
            else if (GetDateWorkStart() != null && GetDeliveryDays() == 0 && GetDateWorkComplete() != null)
            {
                SetDeliveryDays(TimeUtil.GetDaysBetween(GetDateWorkStart(), GetDateWorkComplete()));
            }
            //	Calculate Start Date
            else if (GetDateWorkStart() == null && GetDeliveryDays() != 0 && GetDateWorkComplete() != null)
            {
                SetDateWorkStart(TimeUtil.AddDays(GetDateWorkComplete(), GetDeliveryDays() * -1));
            }
            return true;
        }
    }
}

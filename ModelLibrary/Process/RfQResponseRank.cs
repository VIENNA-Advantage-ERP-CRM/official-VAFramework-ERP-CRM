/********************************************************
* Project Name   : VAdvantage
* Class Name     : RfQResponseRank
* Purpose        : Rank RfQ Responses	
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


using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class RfQResponseRank : ProcessEngine.SvrProcess
    {
        //RfQ 			
        private int _VAB_RFQ_ID = 0;
        //private Logging.VLogger s_log = Logging.VLogger.GetVLogger(typeof(MVABOrderLine).FullName);

        /// <summary>
        /// Prepare
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
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _VAB_RFQ_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process.
        /// <pre>
        /// - ignore 0 or invalid responses
        /// - rank among qty
        /// - for selected PO qty select winner
        /// - if all lines are winner - select that
        ///  </pre> 
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            MVABRfQ rfq = new MVABRfQ(GetCtx(), _VAB_RFQ_ID, Get_TrxName());
            if (rfq.Get_ID() == 0)
            {
                throw new ArgumentException("No RfQ found");
            }
            log.Info(rfq.ToString());
            String error = rfq.CheckQuoteTotalAmtOnly();
            if (error != null && error.Length > 0)
            {
                throw new Exception(error);
            }

            //	Get Completed, Active Responses
            MVABRFQReply[] responses = rfq.GetResponses(true, true);
            log.Fine("doIt - #Responses=" + responses.Length);
            if (responses.Length == 0)
            {
                throw new ArgumentException("No completed RfQ Responses found");
            }
            if (responses.Length == 1)
            {
                responses[0].SetIsSelectedWinner(true);
                responses[0].Save();
                throw new ArgumentException("Only one completed RfQ Response found");
            }

            //	Rank
            if (rfq.IsQuoteTotalAmtOnly())
            {
                RankResponses(rfq, responses);
            }
            else
            {
                RankLines(rfq, responses);
            }
            return "# " + responses.Length;
        }

        /// <summary>
        /// Rank Lines
        /// </summary>
        /// <param name="rfq">RfQ</param>
        /// <param name="responses">responses</param>
        /// @SuppressWarnings("unchecked")
        private void RankLines(MVABRfQ rfq, MVABRFQReply[] responses)
        {
            MVABRfQLine[] rfqLines = rfq.GetLines();
            if (rfqLines.Length == 0)
            {
                throw new ArgumentException("No RfQ Lines found");
            }

            //	 for all lines
            for (int i = 0; i < rfqLines.Length; i++)
            {
                //	RfQ Line
                MVABRfQLine rfqLine = rfqLines[i];
                if (!rfqLine.IsActive())
                {
                    continue;
                }
                log.Fine("rankLines - " + rfqLine);
                MVABRfQLineQty[] rfqQtys = rfqLine.GetQtys();
                for (int j = 0; j < rfqQtys.Length; j++)
                {
                    //	RfQ Line Qty
                    MVABRfQLineQty rfqQty = rfqQtys[j];
                    if (!rfqQty.IsActive() || !rfqQty.IsRfQQty())
                    {
                        continue;
                    }
                    log.Fine("rankLines Qty - " + rfqQty);
                    //genrate rank for product
                    MVABRFQReplyLineQty[] respQtys = rfqQty.GetResponseQtys(false);
                    for (int kk = 0; kk < respQtys.Length; kk++)
                    {
                        //	Response Line Qty
                        MVABRFQReplyLineQty respQty = respQtys[kk];
                        if (!respQty.IsActive() || !respQty.IsValidAmt())
                        {
                            respQty.SetRanking(999);
                            respQty.Save();
                            log.Fine("  - ignored: " + respQty);
                        }
                    }	//	for all respones line qtys

                    //	Rank RfQ Line Qtys
                    respQtys = rfqQty.GetResponseQtys(false);
                    if (respQtys.Length == 0)
                    {
                        log.Fine("  - No Qtys with valid Amounts");
                    }
                    else
                    {
                        try
                        {
                            Array.Sort(respQtys, respQtys[0]);
                            // Arrays.sort(respQtys, respQtys[0]);
                            //Array.Sort(respQtys, respQtys);
                        }
                        catch { }
                        int lastRank = 1;		//	multiple rank #1
                        Decimal? lastAmt = Env.ZERO;
                        for (int rank = 0; rank < respQtys.Length; rank++)
                        {
                            //get the quantity of first record in the sorded array
                            MVABRFQReplyLineQty qty = respQtys[rank];
                            if (!qty.IsActive() || qty.GetRanking() == 999)
                            {
                                continue;
                            }
                            Decimal? netAmt = qty.GetNetAmt();
                            if (netAmt == null)
                            {
                                qty.SetRanking(999);
                                log.Fine("  - Rank 999: " + qty);
                            }
                            else
                            {
                                // JID_1064  only Submitted responce will set rank
                                if (lastAmt.Value.Equals(Env.ZERO))
                                {
                                    lastRank = rank;
                                }
                                else if (lastAmt.Value.CompareTo(netAmt.Value) != 0)
                                {
                                    lastRank = rank + 1;
                                    lastAmt = qty.GetNetAmt();
                                }
                                qty.SetRanking(lastRank);
                                log.Fine("  - Rank " + lastRank + ": " + qty);
                            }
                            qty.Save();
                            //	
                            if (rank == 0)	//	Update RfQ
                            {
                                rfqQty.SetBestResponseAmt(qty.GetNetAmt());
                                rfqQty.Save();
                            }
                        }
                    }
                }	//	for all rfq line qtys
            }	//	 for all rfq lines

            //	Select Winner based on line ranking
            MVABRFQReply winner = null;
            for (int ii = 0; ii < responses.Length; ii++)
            {
                MVABRFQReply response = responses[ii];
                if (response.IsSelectedWinner())
                {
                    response.SetIsSelectedWinner(false);
                }
                int ranking = 0;
                MVABRFQReplyLine[] respLines = response.GetLines(false);
                for (int jj = 0; jj < respLines.Length; jj++)
                {
                    //	Response Line
                    MVABRFQReplyLine respLine = respLines[jj];
                    if (!respLine.IsActive())
                    {
                        continue;
                    }
                    if (respLine.IsSelectedWinner())
                        respLine.SetIsSelectedWinner(false);
                    MVABRFQReplyLineQty[] respQtys = respLine.GetQtys(false);
                    for (int kk = 0; kk < respQtys.Length; kk++)
                    {
                        //	Response Line Qty
                        MVABRFQReplyLineQty respQty = respQtys[kk];
                        if (!respQty.IsActive())
                            continue;
                        ranking += respQty.GetRanking();
                        if (respQty.GetRanking() == 1
                            && respQty.GetRfQLineQty().IsPurchaseQty())
                        {
                            respLine.SetIsSelectedWinner(true);
                            respLine.Save();
                            break;
                        }
                    }
                }
                response.SetRanking(ranking);
                response.Save();
                log.Fine("- Response Ranking " + ranking + ": " + response);
                if (!rfq.IsQuoteSelectedLines())	//	no total selected winner if not all lines
                {
                    if (winner == null && ranking > 0)
                    {
                        winner = response;
                    }
                    if (winner != null
                            && response.GetRanking() > 0
                            && response.GetRanking() < winner.GetRanking())
                    {
                        winner = response;
                    }
                }
            }
            if (winner != null)
            {
                winner.SetIsSelectedWinner(true);
                winner.Save();
                log.Fine("- Response Winner: " + winner);
            }
        }

        /// <summary>
        /// Rank Response based on Header
        /// </summary>
        /// <param name="rfq">RfQ</param>
        /// <param name="responses">responses</param>
        private void RankResponses(MVABRfQ rfq, MVABRFQReply[] responses)
        {
            int ranking = 1;
            //	Responses Ordered by Price
            for (int ii = 0; ii < responses.Length; ii++)
            {
                MVABRFQReply response = responses[ii];
                if (response.GetPrice().CompareTo(Env.ZERO) > 0)
                {
                    if (response.IsSelectedWinner() != (ranking == 1))
                        response.SetIsSelectedWinner(ranking == 1);
                    response.SetRanking(ranking);
                    //
                    ranking++;
                }
                else
                {
                    response.SetRanking(999);
                    if (response.IsSelectedWinner())
                        response.SetIsSelectedWinner(false);
                }
                response.Save();
                log.Fine("rankResponse - " + response);
            }
        }
    }
}

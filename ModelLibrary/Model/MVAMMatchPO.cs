/********************************************************
 * Class Name     : MVAMMatchPO
 * Purpose        : 
 * Class Used     : X_VAM_MatchPO
 * Chronological    Development
 * Raghunandan     08-Jun-2009
 * Veena Pandey    16-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    /// <summary>
    /// Match PO model.
    ///	- Created when processing Shipment or Order
    ///	- Updates Order (delivered, invoiced)
    ///	- Creates PPV acct
    /// </summary>
    public class MVAMMatchPO : X_VAM_MatchPO
    {

        /** Invoice Changed			*/
        private bool _isInvoiceLineChange = false;
        /** InOut Changed			*/
        private bool _isInOutLineChange = false;
        /** Order Line				*/
        private MVABOrderLine _oLine = null;
        /** Invoice Line			*/
        private MVABInvoiceLine _iLine = null;
        /**	Static Logger	*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MVAMMatchPO).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_MatchPO_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMMatchPO(Ctx ctx, int VAM_MatchPO_ID, Trx trxName)
            : base(ctx, VAM_MatchPO_ID, trxName)
        {
            if (VAM_MatchPO_ID == 0)
            {
                //	setVAB_OrderLine_ID (0);
                //	setDateTrx (new Timestamp(System.currentTimeMillis()));
                //	setVAM_Inv_InOutLine_ID (0);
                //	setVAM_Product_ID (0);
                SetVAM_PFeature_SetInstance_ID(0);
                //	setQty (Env.ZERO);
                SetPosted(false);
                SetProcessed(false);
                SetProcessing(false);
            }
        }

        /// <summary>
        /// Load Construor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAMMatchPO(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Shipment Line Constructor
        /// </summary>
        /// <param name="sLine">shipment line</param>
        /// <param name="dateTrx">optional date</param>
        /// <param name="qty">matched quantity</param>
        public MVAMMatchPO(MVAMInvInOutLine sLine, DateTime? dateTrx, Decimal qty)
            : this(sLine.GetCtx(), 0, sLine.Get_Trx())
        {
            SetClientOrg(sLine);
            SetVAM_Inv_InOutLine_ID(sLine.GetVAM_Inv_InOutLine_ID());
            SetVAB_OrderLine_ID(sLine.GetVAB_OrderLine_ID());
            if (dateTrx != null)
                SetDateTrx(dateTrx);
            SetVAM_Product_ID(sLine.GetVAM_Product_ID());
            SetVAM_PFeature_SetInstance_ID(sLine.GetVAM_PFeature_SetInstance_ID());
            SetQty(qty);
            SetProcessed(true);		//	auto
        }

        /// <summary>
        /// Invoice Line Constructor
        /// </summary>
        /// <param name="iLine">invoice line</param>
        /// <param name="dateTrx">optional date</param>
        /// <param name="qty">matched quantity</param>
        public MVAMMatchPO(MVABInvoiceLine iLine, DateTime? dateTrx, Decimal qty)
            : this(iLine.GetCtx(), 0, iLine.Get_Trx())
        {
            SetClientOrg(iLine);
            SetVAB_InvoiceLine_ID(iLine);
            if (iLine.GetVAB_OrderLine_ID() != 0)
                SetVAB_OrderLine_ID(iLine.GetVAB_OrderLine_ID());
            if (dateTrx != null)
                SetDateTrx(dateTrx);
            SetVAM_Product_ID(iLine.GetVAM_Product_ID());
            SetVAM_PFeature_SetInstance_ID(iLine.GetVAM_PFeature_SetInstance_ID());
            SetQty(qty);
            SetProcessed(true);		//	auto
        }

        /// <summary>
        /// Consolidate MPO entries.
        /// </summary>
        /// <param name="ctx">context</param>
        public static void Consolidate(Ctx ctx)
        {
            String sql = "SELECT * FROM VAM_MatchPO po "
                //jz + "WHERE EXISTS (SELECT * FROM VAM_MatchPO x "
                + "WHERE EXISTS (SELECT 1 FROM VAM_MatchPO x "
                    + "WHERE po.VAB_OrderLine_ID=x.VAB_OrderLine_ID AND po.Qty=x.Qty "
                    + "GROUP BY VAB_OrderLine_ID, Qty "
                    + "HAVING COUNT(*) = 2) "
                + " AND VAF_Client_ID=" + ctx.GetVAF_Client_ID()
                + "ORDER BY VAB_OrderLine_ID, VAM_Inv_InOutLine_ID";
            int success = 0;
            int errors = 0;
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    DataRow dr = null;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dr = ds.Tables[0].Rows[i];
                        MVAMMatchPO po1 = new MVAMMatchPO(ctx, dr, null);
                        i++;
                        //if (dr.next())
                        if (i < ds.Tables[0].Rows.Count)
                        {
                            dr = ds.Tables[0].Rows[i];
                            MVAMMatchPO po2 = new MVAMMatchPO(ctx, dr, null);
                            if (po1.GetVAM_Inv_InOutLine_ID() != 0 && po1.GetVAB_InvoiceLine_ID() == 0
                                && po2.GetVAM_Inv_InOutLine_ID() == 0 && po2.GetVAB_InvoiceLine_ID() != 0)
                            {
                                String s1 = "UPDATE VAM_MatchPO SET VAB_InvoiceLine_ID="
                                    + po2.GetVAB_InvoiceLine_ID()
                                    + " WHERE VAM_MatchPO_ID=" + po1.GetVAM_MatchPO_ID();
                                int no1 = ExecuteQuery.ExecuteNonQuery(s1);
                                if (no1 != 1)
                                {
                                    errors++;
                                    //s_log.warning("Not updated VAM_MatchPO_ID=" + po1.GetVAM_MatchPO_ID());
                                    continue;
                                }
                                //
                                String s2 = "DELETE FROM Actual_Acct_Detail WHERE VAF_TableView_ID=473 AND Record_ID=" + po2.GetVAM_MatchPO_ID();
                                int no2 = ExecuteQuery.ExecuteNonQuery(s2);
                                String s3 = "DELETE FROM VAM_MatchPO WHERE VAM_MatchPO_ID=" + po2.GetVAM_MatchPO_ID();
                                int no3 = ExecuteQuery.ExecuteNonQuery(s3);
                                if (no2 == 0 && no3 == 1)
                                    success++;
                                else
                                {
                                    //s_log.warning("VAM_MatchPO_ID=" + po2.GetVAM_MatchPO_ID()
                                    //    + " - Deleted=" + no2 + ", Acct=" + no3);
                                    errors++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);

            }

            if (errors == 0 && success == 0)
            {
                ;
            }
            else
                s_log.Info("Success #" + success + " - Error #" + errors);
        }

        /**
        * 	Find/Create PO(Inv) Match
        *	@param iLine invoice line
        *	@param sLine receipt line
        *	@param dateTrx date
        *	@param qty qty
        *	@return Match Record
        */
        public static MVAMMatchPO Create(MVABInvoiceLine iLine, MVAMInvInOutLine sLine, DateTime? dateTrx, Decimal qty)
        {
            Trx trxName = null;
            Ctx ctx = null;
            int VAB_OrderLine_ID = 0;
            if (iLine != null)
            {
                trxName = iLine.Get_Trx();
                ctx = iLine.GetCtx();
                VAB_OrderLine_ID = iLine.GetVAB_OrderLine_ID();
            }
            else if (sLine != null)
            {
                trxName = sLine.Get_Trx();
                ctx = sLine.GetCtx();
                VAB_OrderLine_ID = sLine.GetVAB_OrderLine_ID();
            }

            MVAMMatchPO retValue = null;
            String sql = "SELECT * FROM VAM_MatchPO WHERE VAB_OrderLine_ID=" + VAB_OrderLine_ID;
            //		ArrayList list = new ArrayList();
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MVAMMatchPO mpo = new MVAMMatchPO(ctx, dr, trxName);
                    if (qty.CompareTo(mpo.GetQty()) == 0)
                    {
                        if (iLine != null)
                        {
                            if (mpo.GetVAB_InvoiceLine_ID() == 0
                                || mpo.GetVAB_InvoiceLine_ID() == iLine.GetVAB_InvoiceLine_ID())
                            {
                                mpo.SetVAB_InvoiceLine_ID(iLine);
                                if (iLine.GetVAM_PFeature_SetInstance_ID() != 0)
                                {
                                    if (mpo.GetVAM_PFeature_SetInstance_ID() == 0)
                                        mpo.SetVAM_PFeature_SetInstance_ID(iLine.GetVAM_PFeature_SetInstance_ID());
                                    else if (mpo.GetVAM_PFeature_SetInstance_ID() != iLine.GetVAM_PFeature_SetInstance_ID())
                                        continue;
                                }
                            }
                            else
                                continue;
                        }
                        if (sLine != null)
                        {
                            if (mpo.GetVAM_Inv_InOutLine_ID() == 0
                                || mpo.GetVAM_Inv_InOutLine_ID() == sLine.GetVAM_Inv_InOutLine_ID())
                            {
                                mpo.SetVAM_Inv_InOutLine_ID(sLine.GetVAM_Inv_InOutLine_ID());
                                if (sLine.GetVAM_PFeature_SetInstance_ID() != 0)
                                {
                                    if (mpo.GetVAM_PFeature_SetInstance_ID() == 0)
                                        mpo.SetVAM_PFeature_SetInstance_ID(sLine.GetVAM_PFeature_SetInstance_ID());
                                    else if (mpo.GetVAM_PFeature_SetInstance_ID() != sLine.GetVAM_PFeature_SetInstance_ID())
                                        continue;
                                }
                            }
                            else
                                continue;
                        }
                        retValue = mpo;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }
            //	Create New
            if (retValue == null)
            {
                if (sLine != null)
                {
                    retValue = new MVAMMatchPO(sLine, dateTrx, qty);
                    if (iLine != null)
                        retValue.SetVAB_InvoiceLine_ID(iLine);
                }
                else if (iLine != null)
                {
                    retValue = new MVAMMatchPO(iLine, dateTrx, qty);
                }
            }
            return retValue;
        }

        /// <summary>
        /// Get PO Matches of receipt
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Inv_InOut_ID">receipt</param>
        /// <param name="trxName">transaction</param>
        /// <returns>array of matches</returns>
        public static MVAMMatchPO[] GetInOut(Ctx ctx, int VAM_Inv_InOut_ID, Trx trxName)
        {
            if (VAM_Inv_InOut_ID == 0)
                return new MVAMMatchPO[] { };
            //
            String sql = "SELECT * FROM VAM_MatchPO m"
                + " INNER JOIN VAM_Inv_InOutLine l ON (m.VAM_Inv_InOutLine_ID=l.VAM_Inv_InOutLine_ID) "
                + "WHERE l.VAM_Inv_InOut_ID=" + VAM_Inv_InOut_ID;
            List<MVAMMatchPO> list = new List<MVAMMatchPO>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MVAMMatchPO(ctx, dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);

            }

            MVAMMatchPO[] retValue = new MVAMMatchPO[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get PO Match with order and invoice
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_OrderLine_ID">order</param>
        /// <param name="VAB_InvoiceLine_ID">invoice</param>
        /// <param name="trxName">transaction</param>
        /// <returns>array of matches</returns>
        public static MVAMMatchPO[] Get(Ctx ctx, int VAB_OrderLine_ID, int VAB_InvoiceLine_ID, Trx trxName)
        {
            if (VAB_OrderLine_ID == 0 || VAB_InvoiceLine_ID == 0)
                return new MVAMMatchPO[] { };
            //
            String sql = "SELECT * FROM VAM_MatchPO WHERE VAB_OrderLine_ID=" + VAB_OrderLine_ID + " AND VAB_InvoiceLine_ID=" + VAB_InvoiceLine_ID;
            List<MVAMMatchPO> list = new List<MVAMMatchPO>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MVAMMatchPO(ctx, dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);

            }
            MVAMMatchPO[] retValue = new MVAMMatchPO[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get PO Match of Receipt Line
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Inv_InOutLine_ID">receipt</param>
        /// <param name="trxName">transaction</param>
        /// <returns>array of matches</returns>
        public static MVAMMatchPO[] Get(Ctx ctx, int VAM_Inv_InOutLine_ID, Trx trxName)
        {
            if (VAM_Inv_InOutLine_ID == 0)
                return new MVAMMatchPO[] { };
            //
            String sql = "SELECT * FROM VAM_MatchPO WHERE VAM_Inv_InOutLine_ID=" + VAM_Inv_InOutLine_ID;
            List<MVAMMatchPO> list = new List<MVAMMatchPO>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MVAMMatchPO(ctx, dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);

            }
            MVAMMatchPO[] retValue = new MVAMMatchPO[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get PO Matches of Invoice
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_Invoice_ID">invoice</param>
        /// <param name="trxName">transaction</param>
        /// <returns>array of matches</returns>
        public static MVAMMatchPO[] GetInvoice(Ctx ctx, int VAB_Invoice_ID, Trx trxName)
        {
            if (VAB_Invoice_ID == 0)
                return new MVAMMatchPO[] { };
            //
            String sql = "SELECT * FROM VAM_MatchPO mi"
                + " INNER JOIN VAB_InvoiceLine il ON (mi.VAB_InvoiceLine_ID=il.VAB_InvoiceLine_ID) "
                + "WHERE il.VAB_Invoice_ID=" + VAB_Invoice_ID;
            List<MVAMMatchPO> list = new List<MVAMMatchPO>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MVAMMatchPO(ctx, dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
                // Common.//ErrorLog.FillErrorLog("MVAMMatchPO.GetInvoice", DataBase.GlobalVariable.LAST_EXECUTED_QUERY, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }
            MVAMMatchPO[] retValue = new MVAMMatchPO[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// After Delete.
        /// Set Order Qty Delivered/Invoiced
        /// </summary>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterDelete(bool success)
        {
            //	Order Delivered/Invoiced
            //	(Reserved in VMatch and MVAMInvInOut.completeIt)
            if (success && GetVAB_OrderLine_ID() != 0)
            {
                MVABOrderLine orderLine = new MVABOrderLine(GetCtx(), GetVAB_OrderLine_ID(), Get_Trx());
                Boolean IsReturnTrx = orderLine.GetParent().IsReturnTrx();
                if (GetVAM_Inv_InOutLine_ID() != 0)
                {
                    orderLine.SetQtyDelivered(Decimal.Subtract(orderLine.GetQtyDelivered(), GetQty()));
                    if (IsReturnTrx)
                    {
                        orderLine.SetQtyReturned(Decimal.Subtract(orderLine.GetQtyReturned(), GetQty()));
                        MVABOrderLine origOrderLine = new MVABOrderLine(GetCtx(), orderLine.GetOrig_OrderLine_ID(), Get_Trx());
                        origOrderLine.SetQtyReturned(Decimal.Subtract(origOrderLine.GetQtyReturned(), GetQty()));
                        origOrderLine.Save();
                    }
                }
                if (GetVAB_InvoiceLine_ID() != 0)
                    orderLine.SetQtyInvoiced(Decimal.Subtract(orderLine.GetQtyInvoiced(), GetQty()));
                return orderLine.Save(Get_Trx());
            }
            return success;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new record</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //	Purchase Order Delivered/Invoiced
            //	(Reserved in VMatch and MVAMInvInOut.completeIt)
            if (success && GetVAB_OrderLine_ID() != 0)
            {
                MVABOrderLine orderLine = GetOrderLine();
                bool isReturnTrx = orderLine.GetParent().IsReturnTrx();
                //
                if (_isInOutLineChange)
                {

                    if (GetVAM_Inv_InOutLine_ID() != 0)							//	new delivery
                    {
                        orderLine.SetQtyDelivered(Decimal.Add(orderLine.GetQtyDelivered(), GetQty()));
                        if (isReturnTrx)
                        {
                            orderLine.SetQtyReturned(Decimal.Add(orderLine.GetQtyReturned(), GetQty()));
                            MVABOrderLine origOrderLine = new MVABOrderLine(GetCtx(), orderLine.GetOrig_OrderLine_ID(), Get_Trx());
                            origOrderLine.SetQtyReturned(Decimal.Add(origOrderLine.GetQtyReturned(), GetQty()));
                            origOrderLine.Save();
                        }
                    }
                    else //	if (getVAM_Inv_InOutLine_ID() == 0)					//	reset to 0
                    {
                        orderLine.SetQtyDelivered(Decimal.Subtract(orderLine.GetQtyDelivered(), GetQty()));
                        if (isReturnTrx)
                        {
                            orderLine.SetQtyReturned(Decimal.Subtract(orderLine.GetQtyReturned(), GetQty()));
                            MVABOrderLine origOrderLine = new MVABOrderLine(GetCtx(), orderLine.GetOrig_OrderLine_ID(), Get_Trx());
                            origOrderLine.SetQtyReturned(Decimal.Add(origOrderLine.GetQtyReturned(), GetQty()));
                            origOrderLine.Save();
                        }
                    }
                    orderLine.SetDateDelivered(GetDateTrx());	//	overwrite=last
                }
                if (_isInvoiceLineChange)
                {
                    if (GetVAB_InvoiceLine_ID() != 0)						//	first time
                        orderLine.SetQtyInvoiced(Decimal.Add(orderLine.GetQtyInvoiced(), GetQty()));
                    else //	if (getVAB_InvoiceLine_ID() == 0)				//	set to 0
                        orderLine.SetQtyInvoiced(Decimal.Subtract(orderLine.GetQtyInvoiced(), GetQty()));
                    orderLine.SetDateInvoiced(GetDateTrx());	//	overwrite=last
                }

                //	Update Order ASI if full match
                //if (orderLine.GetVAM_PFeature_SetInstance_ID() == 0
                //    && GetVAM_Inv_InOutLine_ID() != 0)
                //{
                //    MVAMInvInOutLine iol = new MVAMInvInOutLine(GetCtx(), GetVAM_Inv_InOutLine_ID(), Get_Trx());
                //    if (iol.GetMovementQty().CompareTo(orderLine.GetQtyOrdered()) == 0)
                //        orderLine.SetVAM_PFeature_SetInstance_ID(iol.GetVAM_PFeature_SetInstance_ID());
                //}
                orderLine.Save();
            }
            return success;
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true if acct was deleted</returns>
        protected override bool BeforeDelete()
        {
            if (IsPosted())
            {
                if (!MVABYearPeriod.IsOpen(GetCtx(), GetDateTrx(), MVABMasterDocType.DOCBASETYPE_MATCHPO, GetVAF_Org_ID()))
                    return false;
                SetPosted(false);
                return true;// MActualAcctDetail.Delete(Table_ID, Get_ID(), Get_Trx()) >= 0;
            }

            //JID_0162: System should allow to delete the Matched PO of PO and MR with complete status only.
            if (GetVAB_OrderLine_ID() != 0)
            {
                MVABOrderLine line = new MVABOrderLine(GetCtx(), GetVAB_OrderLine_ID(), Get_TrxName());
                MVABOrder ord = new MVABOrder(GetCtx(), line.GetVAB_Order_ID(), Get_TrxName());
                if (ord.GetDocStatus() != DocumentEngine.ACTION_COMPLETE)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "Order/ShipmentNotCompleted"));
                    return false;
                }
            }

            if (GetVAM_Inv_InOutLine_ID() != 0)
            {
                MVAMInvInOutLine line = new MVAMInvInOutLine(GetCtx(), GetVAM_Inv_InOutLine_ID(), Get_TrxName());
                MVAMInvInOut ino = new MVAMInvInOut(GetCtx(), line.GetVAM_Inv_InOut_ID(), Get_TrxName());
                if (ino.GetDocStatus() != DocumentEngine.ACTION_COMPLETE)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "Order/ShipmentNotCompleted"));
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Set Trx Date
            if (GetDateTrx() == null)
                SetDateTrx(DateTime.Now);
            //	Set Acct Date
            if (GetDateAcct() == null)
            {
                DateTime? ts = GetNewerDateAcct();
                if (ts == null)
                    ts = GetDateTrx();
                SetDateAcct((DateTime?)ts);
            }
            //	Set ASI from Receipt
            if (GetVAM_PFeature_SetInstance_ID() == 0 && GetVAM_Inv_InOutLine_ID() != 0)
            {
                MVAMInvInOutLine iol = new MVAMInvInOutLine(GetCtx(), GetVAM_Inv_InOutLine_ID(), Get_Trx());
                SetVAM_PFeature_SetInstance_ID(iol.GetVAM_PFeature_SetInstance_ID());
            }

            //	Find OrderLine
            if (GetVAB_OrderLine_ID() == 0)
            {
                MVABInvoiceLine il = null;
                if (GetVAB_InvoiceLine_ID() != 0)
                {
                    il = GetInvoiceLine();
                    if (il.GetVAB_OrderLine_ID() != 0)
                        SetVAB_OrderLine_ID(il.GetVAB_OrderLine_ID());
                }	//	get from invoice
                if (GetVAB_OrderLine_ID() == 0 && GetVAM_Inv_InOutLine_ID() != 0)
                {
                    MVAMInvInOutLine iol = new MVAMInvInOutLine(GetCtx(), GetVAM_Inv_InOutLine_ID(), Get_Trx());
                    if (iol.GetVAB_OrderLine_ID() != 0)
                    {
                        SetVAB_OrderLine_ID(iol.GetVAB_OrderLine_ID());
                        if (il != null)
                        {
                            il.SetVAB_OrderLine_ID(iol.GetVAB_OrderLine_ID());
                            il.Save();
                        }
                    }
                }	//	get from shipment
            }	//	find order line

            //	Price Match Approval
            if (GetVAB_OrderLine_ID() != 0
                && GetVAB_InvoiceLine_ID() != 0
                && (newRecord ||
                    Is_ValueChanged("VAB_OrderLine_ID") || Is_ValueChanged("VAB_InvoiceLine_ID")))
            {
                Decimal poPrice = GetOrderLine().GetPriceActual();
                Decimal invPrice = GetInvoiceLine().GetPriceActual();
                Decimal difference = Decimal.Subtract(poPrice, invPrice);
                if (Math.Sign(difference) != 0)
                {
                    difference = Decimal.Multiply(difference, GetQty());
                    SetPriceMatchDifference(difference);
                    //	Approval
                    //MBPGroup group = MBPGroup.getOfBPartner(GetCtx(), GetOrderLine().GetVAB_BusinessPartner_ID());
                    Decimal mt = 0; //group.getPriceMatchTolerance();
                    if (Math.Sign(mt) != 0)
                    {
                        Decimal poAmt = Decimal.Multiply(poPrice, GetQty());
                        Decimal maxTolerance = Decimal.Multiply(poAmt, mt);
                        //maxTolerance = Math.Abs(maxTolerance)
                        //    .divide(Env.ONEHUNDRED, 2, Decimal.ROUND_HALF_UP);
                        maxTolerance = Decimal.Divide(Math.Abs(maxTolerance), Env.ONEHUNDRED);
                        difference = Math.Abs(difference);
                        bool ok = difference.CompareTo(maxTolerance) <= 0;
                        //log.config("Difference=" + GetPriceMatchDifference()
                        //    + ", Max=" + maxTolerance + " => " + ok);
                        SetIsApproved(ok);
                    }
                }
                else
                {
                    SetPriceMatchDifference(difference);
                    SetIsApproved(true);
                }
            }

            return true;
        }

        /// <summary>
        /// Get Invoice Line
        /// </summary>
        /// <returns>invoice line or null</returns>
        public MVABInvoiceLine GetInvoiceLine()
        {
            if (_iLine == null && GetVAB_InvoiceLine_ID() != 0)
                _iLine = new MVABInvoiceLine(GetCtx(), GetVAB_InvoiceLine_ID(), Get_Trx());
            return _iLine;
        }

        /// <summary>
        /// Get the later Date Acct from invoice or shipment
        /// </summary>
        /// <returns>date or null</returns>
        private DateTime? GetNewerDateAcct()
        {
            //		Timestamp orderDate = null;
            DateTime? invoiceDate = null;
            DateTime? shipDate = null;

            String sql = "SELECT i.DateAcct "
                + "FROM VAB_InvoiceLine il"
                + " INNER JOIN VAB_Invoice i ON (i.VAB_Invoice_ID=il.VAB_Invoice_ID) "
                + "WHERE VAB_InvoiceLine_ID=" + GetVAB_InvoiceLine_ID();
            if (GetVAB_InvoiceLine_ID() != 0)
            {
                IDataReader dr = null;
                try
                {
                    dr = DataBase.DB.ExecuteReader(sql, null, null);
                    if (dr.Read())
                    {
                        if (dr[0] != null && dr[0].ToString() != "")
                        {
                            invoiceDate = Convert.ToDateTime(dr[0].ToString());
                        }
                    }
                    dr.Close();
                }
                catch (Exception e)
                {
                    if (dr != null)
                    {
                        dr.Close();
                    }
                    log.Log(Level.SEVERE, sql, e);

                }
            }
            //
            sql = "SELECT io.DateAcct "
                + "FROM VAM_Inv_InOutLine iol"
                + " INNER JOIN VAM_Inv_InOut io ON (io.VAM_Inv_InOut_ID=iol.VAM_Inv_InOut_ID) "
                + "WHERE iol.VAM_Inv_InOutLine_ID=" + GetVAM_Inv_InOutLine_ID();
            if (GetVAM_Inv_InOutLine_ID() != 0)
            {
                IDataReader dr = null;
                try
                {
                    dr = DataBase.DB.ExecuteReader(sql, null, null);
                    if (dr.Read())
                    {
                        if (dr[0] != null && dr[0].ToString() != "")
                        {
                            shipDate = Convert.ToDateTime(dr[0].ToString());
                        }
                    }
                    dr.Close();
                }
                catch (Exception e)
                {
                    if (dr != null)
                    {
                        dr.Close();
                    }
                    log.Log(Level.SEVERE, sql, e);

                }
            }
            //

            //	Assuming that order date is always earlier
            if (invoiceDate == null)
                return (DateTime?)shipDate;
            if (shipDate == null)
                return (DateTime?)invoiceDate;
            if (invoiceDate > shipDate)
                return (DateTime?)invoiceDate;
            return (DateTime?)shipDate;
        }

        /// <summary>
        /// Get Order Line
        /// </summary>
        /// <returns>order line or null</returns>
        public MVABOrderLine GetOrderLine()
        {
            if ((_oLine == null && GetVAB_OrderLine_ID() != 0)
                || GetVAB_OrderLine_ID() != _oLine.GetVAB_OrderLine_ID())
                _oLine = new MVABOrderLine(GetCtx(), GetVAB_OrderLine_ID(), Get_Trx());
            return _oLine;
        }

        /// <summary>
        /// Set VAB_InvoiceLine_ID
        /// </summary>
        /// <param name="VAB_InvoiceLine_ID">id</param>
        public new void SetVAB_InvoiceLine_ID(int VAB_InvoiceLine_ID)
        {
            int old = GetVAB_InvoiceLine_ID();
            if (old != VAB_InvoiceLine_ID)
            {
                base.SetVAB_InvoiceLine_ID(VAB_InvoiceLine_ID);
                _isInvoiceLineChange = true;
            }
        }

        /// <summary>
        /// Set VAB_InvoiceLine_ID
        /// </summary>
        /// <param name="line">line</param>
        public void SetVAB_InvoiceLine_ID(MVABInvoiceLine line)
        {
            _iLine = line;
            if (line == null)
                SetVAB_InvoiceLine_ID(0);
            else
                SetVAB_InvoiceLine_ID(line.GetVAB_InvoiceLine_ID());
        }

        /// <summary>
        /// Set VAB_OrderLine_ID
        /// </summary>
        /// <param name="line">line</param>
        public void SetVAB_OrderLine_ID(MVABOrderLine line)
        {
            _oLine = line;
            if (line == null)
                SetVAB_OrderLine_ID(0);
            else
                SetVAB_OrderLine_ID(line.GetVAB_OrderLine_ID());
        }

        /// <summary>
        /// Set VAB_OrderLine_ID
        /// </summary>
        /// <param name="VAM_Inv_InOutLine_ID">id</param>
        public new void SetVAM_Inv_InOutLine_ID(int VAM_Inv_InOutLine_ID)
        {
            int old = GetVAM_Inv_InOutLine_ID();
            if (old != VAM_Inv_InOutLine_ID)
            {
                base.SetVAM_Inv_InOutLine_ID(VAM_Inv_InOutLine_ID);
                _isInOutLineChange = true;
            }
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMMatchPO[");
            sb.Append(Get_ID())
                .Append(",Qty=").Append(GetQty())
                .Append(",VAB_OrderLine_ID=").Append(GetVAB_OrderLine_ID())
                .Append(",VAM_Inv_InOutLine_ID=").Append(GetVAM_Inv_InOutLine_ID())
                .Append(",VAB_InvoiceLine_ID=").Append(GetVAB_InvoiceLine_ID())
                .Append("]");
            return sb.ToString();
        }

    }
}

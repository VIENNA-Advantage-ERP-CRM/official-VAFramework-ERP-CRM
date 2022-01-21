/********************************************************
 * Class Name     : MMatchPO
 * Purpose        : 
 * Class Used     : X_M_MatchPO
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
using System.Windows.Forms;
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
    public class MMatchPO : X_M_MatchPO
    {

        /** Invoice Changed			*/
        private bool _isInvoiceLineChange = false;
        /** InOut Changed			*/
        private bool _isInOutLineChange = false;
        /** Order Line				*/
        private MOrderLine _oLine = null;
        /** Invoice Line			*/
        private MInvoiceLine _iLine = null;
        /**	Static Logger	*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MMatchPO).FullName);

        public String orderDocStatus = String.Empty;
        public String inOutDocStatus = String.Empty;
        public MInOutLine iol = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_MatchPO_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MMatchPO(Ctx ctx, int M_MatchPO_ID, Trx trxName)
            : base(ctx, M_MatchPO_ID, trxName)
        {
            if (M_MatchPO_ID == 0)
            {
                //	setC_OrderLine_ID (0);
                //	setDateTrx (new Timestamp(System.currentTimeMillis()));
                //	setM_InOutLine_ID (0);
                //	setM_Product_ID (0);
                SetM_AttributeSetInstance_ID(0);
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
        public MMatchPO(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Shipment Line Constructor
        /// </summary>
        /// <param name="sLine">shipment line</param>
        /// <param name="dateTrx">optional date</param>
        /// <param name="qty">matched quantity</param>
        public MMatchPO(MInOutLine sLine, DateTime? dateTrx, Decimal qty)
            : this(sLine.GetCtx(), 0, sLine.Get_Trx())
        {
            SetClientOrg(sLine);
            SetM_InOutLine_ID(sLine.GetM_InOutLine_ID());
            SetC_OrderLine_ID(sLine.GetC_OrderLine_ID());
            if (dateTrx != null)
                SetDateTrx(dateTrx);
            SetM_Product_ID(sLine.GetM_Product_ID());
            SetM_AttributeSetInstance_ID(sLine.GetM_AttributeSetInstance_ID());
            SetQty(qty);
            SetProcessed(true);		//	auto
        }

        /// <summary>
        /// Invoice Line Constructor
        /// </summary>
        /// <param name="iLine">invoice line</param>
        /// <param name="dateTrx">optional date</param>
        /// <param name="qty">matched quantity</param>
        public MMatchPO(MInvoiceLine iLine, DateTime? dateTrx, Decimal qty)
            : this(iLine.GetCtx(), 0, iLine.Get_Trx())
        {
            SetClientOrg(iLine);
            SetC_InvoiceLine_ID(iLine);
            if (iLine.GetC_OrderLine_ID() != 0)
                SetC_OrderLine_ID(iLine.GetC_OrderLine_ID());
            if (dateTrx != null)
                SetDateTrx(dateTrx);
            SetM_Product_ID(iLine.GetM_Product_ID());
            SetM_AttributeSetInstance_ID(iLine.GetM_AttributeSetInstance_ID());
            SetQty(qty);
            SetProcessed(true);		//	auto
        }

        /// <summary>
        /// Consolidate MPO entries.
        /// </summary>
        /// <param name="ctx">context</param>
        public static void Consolidate(Ctx ctx)
        {
            String sql = "SELECT * FROM M_MatchPO po "
                //jz + "WHERE EXISTS (SELECT * FROM M_MatchPO x "
                + "WHERE EXISTS (SELECT 1 FROM M_MatchPO x "
                    + "WHERE po.C_OrderLine_ID=x.C_OrderLine_ID AND po.Qty=x.Qty "
                    + "GROUP BY C_OrderLine_ID, Qty "
                    + "HAVING COUNT(*) = 2) "
                + " AND AD_Client_ID=" + ctx.GetAD_Client_ID()
                + "ORDER BY C_OrderLine_ID, M_InOutLine_ID";
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
                        MMatchPO po1 = new MMatchPO(ctx, dr, null);
                        i++;
                        //if (dr.next())
                        if (i < ds.Tables[0].Rows.Count)
                        {
                            dr = ds.Tables[0].Rows[i];
                            MMatchPO po2 = new MMatchPO(ctx, dr, null);
                            if (po1.GetM_InOutLine_ID() != 0 && po1.GetC_InvoiceLine_ID() == 0
                                && po2.GetM_InOutLine_ID() == 0 && po2.GetC_InvoiceLine_ID() != 0)
                            {
                                String s1 = "UPDATE M_MatchPO SET C_InvoiceLine_ID="
                                    + po2.GetC_InvoiceLine_ID()
                                    + " WHERE M_MatchPO_ID=" + po1.GetM_MatchPO_ID();
                                int no1 = ExecuteQuery.ExecuteNonQuery(s1);
                                if (no1 != 1)
                                {
                                    errors++;
                                    //s_log.warning("Not updated M_MatchPO_ID=" + po1.GetM_MatchPO_ID());
                                    continue;
                                }
                                //
                                String s2 = "DELETE FROM Fact_Acct WHERE AD_Table_ID=473 AND Record_ID=" + po2.GetM_MatchPO_ID();
                                int no2 = ExecuteQuery.ExecuteNonQuery(s2);
                                String s3 = "DELETE FROM M_MatchPO WHERE M_MatchPO_ID=" + po2.GetM_MatchPO_ID();
                                int no3 = ExecuteQuery.ExecuteNonQuery(s3);
                                if (no2 == 0 && no3 == 1)
                                    success++;
                                else
                                {
                                    //s_log.warning("M_MatchPO_ID=" + po2.GetM_MatchPO_ID()
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
        public static MMatchPO Create(MInvoiceLine iLine, MInOutLine sLine, DateTime? dateTrx, Decimal qty)
        {
            Trx trxName = null;
            Ctx ctx = null;
            int C_OrderLine_ID = 0;
            if (iLine != null)
            {
                trxName = iLine.Get_Trx();
                ctx = iLine.GetCtx();
                C_OrderLine_ID = iLine.GetC_OrderLine_ID();
            }
            else if (sLine != null)
            {
                trxName = sLine.Get_Trx();
                ctx = sLine.GetCtx();
                C_OrderLine_ID = sLine.GetC_OrderLine_ID();
            }

            MMatchPO retValue = null;
            String sql = "SELECT * FROM M_MatchPO WHERE C_OrderLine_ID=" + C_OrderLine_ID;
            //		ArrayList list = new ArrayList();
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MMatchPO mpo = new MMatchPO(ctx, dr, trxName);
                    if (qty.CompareTo(mpo.GetQty()) == 0)
                    {
                        if (iLine != null)
                        {
                            if (mpo.GetC_InvoiceLine_ID() == 0
                                || mpo.GetC_InvoiceLine_ID() == iLine.GetC_InvoiceLine_ID())
                            {
                                mpo.SetC_InvoiceLine_ID(iLine);
                                if (iLine.GetM_AttributeSetInstance_ID() != 0)
                                {
                                    if (mpo.GetM_AttributeSetInstance_ID() == 0)
                                        mpo.SetM_AttributeSetInstance_ID(iLine.GetM_AttributeSetInstance_ID());
                                    else if (mpo.GetM_AttributeSetInstance_ID() != iLine.GetM_AttributeSetInstance_ID())
                                        continue;
                                }
                            }
                            else
                                continue;
                        }
                        if (sLine != null)
                        {
                            if (mpo.GetM_InOutLine_ID() == 0
                                || mpo.GetM_InOutLine_ID() == sLine.GetM_InOutLine_ID())
                            {
                                mpo.SetM_InOutLine_ID(sLine.GetM_InOutLine_ID());
                                if (sLine.GetM_AttributeSetInstance_ID() != 0)
                                {
                                    if (mpo.GetM_AttributeSetInstance_ID() == 0)
                                        mpo.SetM_AttributeSetInstance_ID(sLine.GetM_AttributeSetInstance_ID());
                                    else if (mpo.GetM_AttributeSetInstance_ID() != sLine.GetM_AttributeSetInstance_ID())
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
                    retValue = new MMatchPO(sLine, dateTrx, qty);
                    if (iLine != null)
                        retValue.SetC_InvoiceLine_ID(iLine);
                }
                else if (iLine != null)
                {
                    retValue = new MMatchPO(iLine, dateTrx, qty);
                }
            }
            return retValue;
        }

        /// <summary>
        /// Get PO Matches of receipt
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_InOut_ID">receipt</param>
        /// <param name="trxName">transaction</param>
        /// <returns>array of matches</returns>
        public static MMatchPO[] GetInOut(Ctx ctx, int M_InOut_ID, Trx trxName)
        {
            if (M_InOut_ID == 0)
                return new MMatchPO[] { };
            //
            String sql = "SELECT * FROM M_MatchPO m"
                + " INNER JOIN M_InOutLine l ON (m.M_InOutLine_ID=l.M_InOutLine_ID) "
                + "WHERE l.M_InOut_ID=" + M_InOut_ID;
            List<MMatchPO> list = new List<MMatchPO>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MMatchPO(ctx, dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);

            }

            MMatchPO[] retValue = new MMatchPO[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get PO Match with order and invoice
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_OrderLine_ID">order</param>
        /// <param name="C_InvoiceLine_ID">invoice</param>
        /// <param name="trxName">transaction</param>
        /// <returns>array of matches</returns>
        public static MMatchPO[] Get(Ctx ctx, int C_OrderLine_ID, int C_InvoiceLine_ID, Trx trxName)
        {
            if (C_OrderLine_ID == 0 || C_InvoiceLine_ID == 0)
                return new MMatchPO[] { };
            //
            String sql = "SELECT * FROM M_MatchPO WHERE C_OrderLine_ID=" + C_OrderLine_ID + " AND C_InvoiceLine_ID=" + C_InvoiceLine_ID;
            List<MMatchPO> list = new List<MMatchPO>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MMatchPO(ctx, dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);

            }
            MMatchPO[] retValue = new MMatchPO[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get PO Match of Receipt Line
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_InOutLine_ID">receipt</param>
        /// <param name="trxName">transaction</param>
        /// <returns>array of matches</returns>
        public static MMatchPO[] Get(Ctx ctx, int M_InOutLine_ID, Trx trxName)
        {
            if (M_InOutLine_ID == 0)
                return new MMatchPO[] { };
            //
            String sql = "SELECT * FROM M_MatchPO WHERE M_InOutLine_ID=" + M_InOutLine_ID;
            List<MMatchPO> list = new List<MMatchPO>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MMatchPO(ctx, dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);

            }
            MMatchPO[] retValue = new MMatchPO[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get PO Matches of Invoice
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Invoice_ID">invoice</param>
        /// <param name="trxName">transaction</param>
        /// <returns>array of matches</returns>
        public static MMatchPO[] GetInvoice(Ctx ctx, int C_Invoice_ID, Trx trxName)
        {
            if (C_Invoice_ID == 0)
                return new MMatchPO[] { };
            //
            String sql = "SELECT * FROM M_MatchPO mi"
                + " INNER JOIN C_InvoiceLine il ON (mi.C_InvoiceLine_ID=il.C_InvoiceLine_ID) "
                + "WHERE il.C_Invoice_ID=" + C_Invoice_ID;
            List<MMatchPO> list = new List<MMatchPO>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MMatchPO(ctx, dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
                // Common.//ErrorLog.FillErrorLog("MMatchPO.GetInvoice", DataBase.GlobalVariable.LAST_EXECUTED_QUERY, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }
            MMatchPO[] retValue = new MMatchPO[list.Count];
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
            //	(Reserved in VMatch and MInOut.completeIt)
            if (success && GetC_OrderLine_ID() != 0)
            {
                MOrderLine orderLine = new MOrderLine(GetCtx(), GetC_OrderLine_ID(), Get_Trx());
                Boolean IsReturnTrx = orderLine.GetParent().IsReturnTrx();
                if (GetM_InOutLine_ID() != 0)
                {
                    orderLine.SetQtyDelivered(Decimal.Subtract(orderLine.GetQtyDelivered(), GetQty()));
                    if (IsReturnTrx)
                    {
                        orderLine.SetQtyReturned(Decimal.Subtract(orderLine.GetQtyReturned(), GetQty()));
                        MOrderLine origOrderLine = new MOrderLine(GetCtx(), orderLine.GetOrig_OrderLine_ID(), Get_Trx());
                        origOrderLine.SetQtyReturned(Decimal.Subtract(origOrderLine.GetQtyReturned(), GetQty()));
                        origOrderLine.Save();
                    }
                }
                if (GetC_InvoiceLine_ID() != 0)
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
            //	(Reserved in VMatch and MInOut.completeIt)
            if (success && GetC_OrderLine_ID() != 0)
            {
                MOrderLine orderLine = GetOrderLine();
                bool isReturnTrx = orderLine.GetParent().IsReturnTrx();
                //
                if (_isInOutLineChange)
                {

                    if (GetM_InOutLine_ID() != 0)							//	new delivery
                    {
                        orderLine.SetQtyDelivered(Decimal.Add(orderLine.GetQtyDelivered(), GetQty()));
                        if (isReturnTrx)
                        {
                            orderLine.SetQtyReturned(Decimal.Add(orderLine.GetQtyReturned(), GetQty()));
                            MOrderLine origOrderLine = new MOrderLine(GetCtx(), orderLine.GetOrig_OrderLine_ID(), Get_Trx());
                            origOrderLine.SetQtyReturned(Decimal.Add(origOrderLine.GetQtyReturned(), GetQty()));
                            origOrderLine.Save();
                        }
                    }
                    else //	if (getM_InOutLine_ID() == 0)					//	reset to 0
                    {
                        orderLine.SetQtyDelivered(Decimal.Subtract(orderLine.GetQtyDelivered(), GetQty()));
                        if (isReturnTrx)
                        {
                            orderLine.SetQtyReturned(Decimal.Subtract(orderLine.GetQtyReturned(), GetQty()));
                            MOrderLine origOrderLine = new MOrderLine(GetCtx(), orderLine.GetOrig_OrderLine_ID(), Get_Trx());
                            origOrderLine.SetQtyReturned(Decimal.Add(origOrderLine.GetQtyReturned(), GetQty()));
                            origOrderLine.Save();
                        }
                    }
                    orderLine.SetDateDelivered(GetDateTrx());	//	overwrite=last
                }
                if (_isInvoiceLineChange)
                {
                    if (GetC_InvoiceLine_ID() != 0)						//	first time
                        orderLine.SetQtyInvoiced(Decimal.Add(orderLine.GetQtyInvoiced(), GetQty()));
                    else //	if (getC_InvoiceLine_ID() == 0)				//	set to 0
                        orderLine.SetQtyInvoiced(Decimal.Subtract(orderLine.GetQtyInvoiced(), GetQty()));
                    orderLine.SetDateInvoiced(GetDateTrx());	//	overwrite=last
                }

                //	Update Order ASI if full match
                //if (orderLine.GetM_AttributeSetInstance_ID() == 0
                //    && GetM_InOutLine_ID() != 0)
                //{
                //    MInOutLine iol = new MInOutLine(GetCtx(), GetM_InOutLine_ID(), Get_Trx());
                //    if (iol.GetMovementQty().CompareTo(orderLine.GetQtyOrdered()) == 0)
                //        orderLine.SetM_AttributeSetInstance_ID(iol.GetM_AttributeSetInstance_ID());
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
                if (!MPeriod.IsOpen(GetCtx(), GetDateTrx(), MDocBaseType.DOCBASETYPE_MATCHPO, GetAD_Org_ID()))
                    return false;
                SetPosted(false);
                return true;// MFactAcct.Delete(Table_ID, Get_ID(), Get_Trx()) >= 0;
            }

            //JID_0162: System should allow to delete the Matched PO of PO and MR with complete status only.
            if (GetC_OrderLine_ID() != 0)
            {
                if (String.IsNullOrEmpty(orderDocStatus))
                {
                    //MOrderLine line = new MOrderLine(GetCtx(), GetC_OrderLine_ID(), Get_TrxName());
                    //MOrder ord = new MOrder(GetCtx(), line.GetC_Order_ID(), Get_TrxName());
                    orderDocStatus = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT DocStatus FROM C_Order WHERE C_Order_ID = 
                    (SELECT C_Order_ID from C_OrderLine WHERE C_OrderLine_ID = " + GetC_OrderLine_ID() + ") ", null, Get_Trx()));
                }
                if (orderDocStatus != DocumentEngine.ACTION_COMPLETE)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "Order/ShipmentNotCompleted"));
                    return false;
                }
            }

            if (GetM_InOutLine_ID() != 0)
            {
                if (String.IsNullOrEmpty(inOutDocStatus))
                {
                    //MInOutLine line = new MInOutLine(GetCtx(), GetM_InOutLine_ID(), Get_TrxName());
                    //MInOut ino = new MInOut(GetCtx(), line.GetM_InOut_ID(), Get_TrxName());
                    inOutDocStatus = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT DocStatus FROM M_InOut WHERE M_InOut_ID = 
                    (SELECT M_InOut_ID from M_InOutLine WHERE M_InOutLine_ID = " + GetM_InOutLine_ID() + ") ", null, Get_Trx()));
                }
                if (inOutDocStatus != DocumentEngine.ACTION_COMPLETE)
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
            if (GetM_AttributeSetInstance_ID() == 0 && GetM_InOutLine_ID() != 0)
            {
                MInOutLine iol = new MInOutLine(GetCtx(), GetM_InOutLine_ID(), Get_Trx());
                SetM_AttributeSetInstance_ID(iol.GetM_AttributeSetInstance_ID());
            }

            //	Find OrderLine
            if (GetC_OrderLine_ID() == 0)
            {
                MInvoiceLine il = null;
                if (GetC_InvoiceLine_ID() != 0)
                {
                    il = GetInvoiceLine();
                    if (il.GetC_OrderLine_ID() != 0)
                        SetC_OrderLine_ID(il.GetC_OrderLine_ID());
                }	//	get from invoice
                if (GetC_OrderLine_ID() == 0 && GetM_InOutLine_ID() != 0)
                {
                    MInOutLine iol = new MInOutLine(GetCtx(), GetM_InOutLine_ID(), Get_Trx());
                    if (iol.GetC_OrderLine_ID() != 0)
                    {
                        SetC_OrderLine_ID(iol.GetC_OrderLine_ID());
                        if (il != null)
                        {
                            il.SetC_OrderLine_ID(iol.GetC_OrderLine_ID());
                            il.Save();
                        }
                    }
                }	//	get from shipment
            }	//	find order line

            //	Price Match Approval
            if (GetC_OrderLine_ID() != 0
                && GetC_InvoiceLine_ID() != 0
                && (newRecord ||
                    Is_ValueChanged("C_OrderLine_ID") || Is_ValueChanged("C_InvoiceLine_ID")))
            {
                Decimal poPrice = GetOrderLine().GetPriceActual();
                Decimal invPrice = GetInvoiceLine().GetPriceActual();
                Decimal difference = Decimal.Subtract(poPrice, invPrice);
                if (Math.Sign(difference) != 0)
                {
                    difference = Decimal.Multiply(difference, GetQty());
                    SetPriceMatchDifference(difference);
                    //	Approval
                    //MBPGroup group = MBPGroup.getOfBPartner(GetCtx(), GetOrderLine().GetC_BPartner_ID());
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
        public MInvoiceLine GetInvoiceLine()
        {
            if (_iLine == null && GetC_InvoiceLine_ID() != 0)
                _iLine = new MInvoiceLine(GetCtx(), GetC_InvoiceLine_ID(), Get_Trx());
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
                + "FROM C_InvoiceLine il"
                + " INNER JOIN C_Invoice i ON (i.C_Invoice_ID=il.C_Invoice_ID) "
                + "WHERE C_InvoiceLine_ID=" + GetC_InvoiceLine_ID();
            if (GetC_InvoiceLine_ID() != 0)
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
                + "FROM M_InOutLine iol"
                + " INNER JOIN M_InOut io ON (io.M_InOut_ID=iol.M_InOut_ID) "
                + "WHERE iol.M_InOutLine_ID=" + GetM_InOutLine_ID();
            if (GetM_InOutLine_ID() != 0)
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
        public MOrderLine GetOrderLine()
        {
            if ((_oLine == null && GetC_OrderLine_ID() != 0)
                || GetC_OrderLine_ID() != _oLine.GetC_OrderLine_ID())
                _oLine = new MOrderLine(GetCtx(), GetC_OrderLine_ID(), Get_Trx());
            return _oLine;
        }

        /// <summary>
        /// Set C_InvoiceLine_ID
        /// </summary>
        /// <param name="C_InvoiceLine_ID">id</param>
        public new void SetC_InvoiceLine_ID(int C_InvoiceLine_ID)
        {
            int old = GetC_InvoiceLine_ID();
            if (old != C_InvoiceLine_ID)
            {
                base.SetC_InvoiceLine_ID(C_InvoiceLine_ID);
                _isInvoiceLineChange = true;
            }
        }

        /// <summary>
        /// Set C_InvoiceLine_ID
        /// </summary>
        /// <param name="line">line</param>
        public void SetC_InvoiceLine_ID(MInvoiceLine line)
        {
            _iLine = line;
            if (line == null)
                SetC_InvoiceLine_ID(0);
            else
                SetC_InvoiceLine_ID(line.GetC_InvoiceLine_ID());
        }

        /// <summary>
        /// Set C_OrderLine_ID
        /// </summary>
        /// <param name="line">line</param>
        public void SetC_OrderLine_ID(MOrderLine line)
        {
            _oLine = line;
            if (line == null)
                SetC_OrderLine_ID(0);
            else
                SetC_OrderLine_ID(line.GetC_OrderLine_ID());
        }

        /// <summary>
        /// Set C_OrderLine_ID
        /// </summary>
        /// <param name="M_InOutLine_ID">id</param>
        public new void SetM_InOutLine_ID(int M_InOutLine_ID)
        {
            int old = GetM_InOutLine_ID();
            if (old != M_InOutLine_ID)
            {
                base.SetM_InOutLine_ID(M_InOutLine_ID);
                _isInOutLineChange = true;
            }
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MMatchPO[");
            sb.Append(Get_ID())
                .Append(",Qty=").Append(GetQty())
                .Append(",C_OrderLine_ID=").Append(GetC_OrderLine_ID())
                .Append(",M_InOutLine_ID=").Append(GetM_InOutLine_ID())
                .Append(",C_InvoiceLine_ID=").Append(GetC_InvoiceLine_ID())
                .Append("]");
            return sb.ToString();
        }

    }
}

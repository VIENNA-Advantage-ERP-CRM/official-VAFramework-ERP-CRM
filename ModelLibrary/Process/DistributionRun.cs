/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DistributionRun
 * Purpose        :Create Distribution	
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     03-Nov-2009
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
//using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class DistributionRun : ProcessEngine.SvrProcess
    {
        #region Private variables
        //	The Run to execute		
        private int _VAM_DistributionRun_ID = 0;
        //	Date Promised		
        private DateTime? _DatePromised = null;
        //Dicument Type			
        private int _VAB_DocTypes_ID = 0;
        // Test Mode				
        private bool _IsTest = false;

        //	Distribution Run			
        private MVAMDistributionRun _run = null;
        //Distribution Run Lines		
        private MVAMDistributionRunLine[] _runLines = null;
        // Distribution Run Details	
        private MVATCirculationDetail[] _details = null;

        //	Date Ordered			
        private DateTime? _DateOrdered = null;
        //	Orders Created			
        private int _counter = 0;
        // Document Type			
        private MVABDocTypes _docType = null;
        #endregion

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                //	log.fine("prepare - " + para[i]);
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAB_DocTypes_ID"))
                {
                    _VAB_DocTypes_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("DatePromised"))
                {
                    _DatePromised = (DateTime?)para[i].GetParameter();
                }
                else if (name.Equals("IsTest"))
                {
                    _IsTest = "Y".Equals(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
            _VAM_DistributionRun_ID = GetRecord_ID();
        }

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message (text with variables)</returns>
        protected override String DoIt()
        {
            log.Info("VAM_DistributionRun_ID=" + _VAM_DistributionRun_ID
                + ", VAB_DocTypes_ID=" + _VAB_DocTypes_ID
                + ", DatePromised=" + _DatePromised
                + ", Test=" + _IsTest);
            //	Distribution Run
            if (_VAM_DistributionRun_ID == 0)
            {
                throw new ArgumentException("No Distribution Run ID");
            }
            _run = new MVAMDistributionRun(GetCtx(), _VAM_DistributionRun_ID, Get_TrxName());
            if (_run.Get_ID() == 0)
            {
                throw new Exception("Distribution Run not found -  VAM_DistributionRun_ID=" + _VAM_DistributionRun_ID);
            }
            _runLines = _run.GetLines(true);
            if (_runLines == null || _runLines.Length == 0)
            {
                throw new Exception("No active, non-zero Distribution Run Lines found");
            }

            //	Document Type
            if (_VAB_DocTypes_ID == 0)
            {
                throw new ArgumentException("No Document Type ID");
            }
            _docType = new MVABDocTypes(GetCtx(), _VAB_DocTypes_ID, null);	//	outside trx
            if (_docType.Get_ID() == 0)
            {
                throw new Exception("Document Type not found -  VAB_DocTypes_ID=" + _VAB_DocTypes_ID);
            }
            //
            //_DateOrdered = Utility.Util.GetValueOfDateTime(new DateTime(CommonFunctions.CurrentTimeMillis()));
            _DateOrdered = Utility.Util.GetValueOfDateTime(DateTime.Now.Date);
            if (_DatePromised == null)
            {
                _DatePromised = _DateOrdered;
            }

            //	Create Temp Lines
            if (InsertDetails() == 0)
            {
                throw new Exception("No Lines");
            }

            //	Order By Distribution Run Line
            _details = MVATCirculationDetail.Get(GetCtx(), _VAM_DistributionRun_ID, false, Get_TrxName());
            //	First Run -- Add & Round
            AddAllocations();

            //	Do Allocation
            int loops = 0;
            while (!IsAllocationEqTotal())
            {
                AdjustAllocation();
                AddAllocations();
                if (++loops > 10)
                {
                    throw new Exception("Loop detected - more than 10 Allocation attempts");
                }
            }

            //	Order By Business Partner
            _details = MVATCirculationDetail.Get(GetCtx(), _VAM_DistributionRun_ID, true, Get_TrxName());
            //	Create Orders
            CreateOrders();

            return "@Created@ #" + _counter;
        }	//	doIt


        /// <summary>
        /// Insert Details
        /// </summary>
        /// <returns>number of rows inserted</returns>
        private int InsertDetails()
        {
            //	Handle NULL
            String sql = "UPDATE VAM_DistributionRunLine SET MinQty = 0 WHERE MinQty IS NULL";
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            sql = "UPDATE VAM_DistributionListLine SET MinQty = 0 WHERE MinQty IS NULL";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            //	Total Ratio
            sql ="UPDATE VAM_DistributionList l "
                + "SET RatioTotal = (SELECT SUM(Ratio) FROM VAM_DistributionListLine ll "
                + " WHERE l.VAM_DistributionList_ID=ll.VAM_DistributionList_ID) "
            + "WHERE EXISTS (SELECT * FROM VAM_DistributionRunLine rl"
                + " WHERE l.VAM_DistributionList_ID=rl.VAM_DistributionList_ID"
                + " AND rl.VAM_DistributionRun_ID=" + _VAM_DistributionRun_ID + ")";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());

            //	Delete Old
            sql = "DELETE FROM VAT_CirculationDetail WHERE VAM_DistributionRun_ID="
                + _VAM_DistributionRun_ID;
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            log.Fine("insertDetails - deleted #" + no);
            //	Insert New
            sql = "INSERT INTO VAT_CirculationDetail "
                + "(VAM_DistributionRun_ID, VAM_DistributionRunLine_ID, VAM_DistributionList_ID, VAM_DistributionListLine_ID,"
                + "VAF_Client_ID,VAF_Org_ID, IsActive, Created,CreatedBy, Updated,UpdatedBy,"
                + "VAB_BusinessPartner_ID, VAB_BPart_Location_ID, VAM_Product_ID,"
                + "Ratio, MinQty, Qty) "
                //
                + "SELECT rl.VAM_DistributionRun_ID, rl.VAM_DistributionRunLine_ID,"
                + "ll.VAM_DistributionList_ID, ll.VAM_DistributionListLine_ID, "
                + "rl.VAF_Client_ID,rl.VAF_Org_ID, rl.IsActive, rl.Created,rl.CreatedBy, rl.Updated,rl.UpdatedBy,"
                + "ll.VAB_BusinessPartner_ID, ll.VAB_BPart_Location_ID, rl.VAM_Product_ID, "
                + "ll.Ratio, "
                + "CASE WHEN rl.MinQty > ll.MinQty THEN rl.MinQty ELSE ll.MinQty END, ";

            if (DatabaseType.IsOracle || DatabaseType.IsPostgre)
            {
                sql += "trunc((ll.Ratio/l.RatioTotal*rl.TotalQty),2)";
            }
            else if (DatabaseType.IsMSSql || DatabaseType.IsMySql)
            {
                sql += "Round((ll.Ratio/l.RatioTotal*rl.TotalQty),2)";
            }
            //else if (DatabaseType.IsMySql)
            //{

            //}

            sql += "FROM VAM_DistributionRunLine rl"
                + " INNER JOIN VAM_DistributionList l ON (rl.VAM_DistributionList_ID=l.VAM_DistributionList_ID)"
                + " INNER JOIN VAM_DistributionListLine ll ON (rl.VAM_DistributionList_ID=ll.VAM_DistributionList_ID) "
                + "WHERE rl.VAM_DistributionRun_ID=" + _VAM_DistributionRun_ID
                + " AND l.RatioTotal<>0 AND rl.IsActive='Y' AND ll.IsActive='Y'";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());

            log.Fine("inserted #" + no);
            return no;
        }


        /// <summary>
        /// Add up Allocations
        /// </summary>
        private void AddAllocations()
        {
            //	Reset
            for (int j = 0; j < _runLines.Length; j++)
            {
                MVAMDistributionRunLine runLine = _runLines[j];
                runLine.ResetCalculations();
            }
            //	Add Up
            for (int i = 0; i < _details.Length; i++)
            {
                MVATCirculationDetail detail = _details[i];
                for (int j = 0; j < _runLines.Length; j++)
                {
                    MVAMDistributionRunLine runLine = _runLines[j];
                    if (runLine.GetVAM_DistributionRunLine_ID() == detail.GetVAM_DistributionRunLine_ID())
                    {
                        //	Round
                        detail.Round(runLine.GetUOMPrecision());
                        //	Add
                        runLine.AddActualMin(detail.GetMinQty());
                        runLine.AddActualQty(detail.GetQty());
                        runLine.AddActualAllocation(detail.GetActualAllocation());
                        runLine.SetMaxAllocation(detail.GetActualAllocation(), false);
                        //
                        log.Fine("RunLine=" + runLine.GetLine()
                            + ": BP_ID=" + detail.GetVAB_BusinessPartner_ID()
                            + ", Min=" + detail.GetMinQty()
                            + ", Qty=" + detail.GetQty()
                            + ", Allocation=" + detail.GetActualAllocation());
                        continue;
                    }
                }
            }	//	for all detail lines

            //	Info
            for (int j = 0; j < _runLines.Length; j++)
            {
                MVAMDistributionRunLine runLine = _runLines[j];
                log.Fine("Run - " + runLine.GetInfo());
            }
        }


        /// <summary>
        /// Is Allocation Equals Total
        /// </summary>
        /// <returns>true if allocation eq total</returns>
        private bool IsAllocationEqTotal()
        {
            bool allocationEqTotal = true;
            //	Check total min qty & delta
            for (int j = 0; j < _runLines.Length; j++)
            {
                MVAMDistributionRunLine runLine = _runLines[j];
                if (runLine.IsActualMinGtTotal())
                    throw new Exception("Line " + runLine.GetLine()
                        + " Sum of Min Qty=" + runLine.GetActualMin()
                        + " is greater than Total Qty=" + runLine.GetTotalQty());
                if (allocationEqTotal && !runLine.IsActualAllocationEqTotal())
                {
                    allocationEqTotal = false;
                }
            }	//	for all run lines
            log.Info("=" + allocationEqTotal);
            return allocationEqTotal;
        }


        /// <summary>
        /// Adjust Allocation
        /// </summary>
        private void AdjustAllocation()
        {
            for (int j = 0; j < _runLines.Length; j++)
                AdjustAllocation(j);
        }

        /// <summary>
        /// Adjust Run Line Allocation
        /// </summary>
        /// <param name="index">run line index</param>
        private void AdjustAllocation(int index)
        {
            MVAMDistributionRunLine runLine = _runLines[index];
            Decimal difference = runLine.GetActualAllocationDiff();
            if (difference.CompareTo(Env.ZERO) == 0)
            {
                return;
            }
            //	Adjust when difference is -1->1 or last difference is the same 
            bool adjustBiggest = Math.Abs(difference).CompareTo(Env.ONE) <= 0
                || Math.Abs(difference).CompareTo(Math.Abs(runLine.GetLastDifference())) == 0;
            log.Fine("Line=" + runLine.GetLine()
                + ", Diff=" + difference + ", Adjust=" + adjustBiggest);
            //	Adjust Biggest Amount
            if (adjustBiggest)
            {
                for (int i = 0; i < _details.Length; i++)
                {
                    MVATCirculationDetail detail = _details[i];
                    if (runLine.GetVAM_DistributionRunLine_ID() == detail.GetVAM_DistributionRunLine_ID())
                    {
                        log.Fine("Biggest - DetailAllocation=" + detail.GetActualAllocation()
                            + ", MaxAllocation=" + runLine.GetMaxAllocation()
                            + ", Qty Difference=" + difference);
                        if (detail.GetActualAllocation().CompareTo(runLine.GetMaxAllocation()) == 0
                            && detail.IsCanAdjust())
                        {
                            detail.AdjustQty(difference);
                            detail.Save();
                            return;
                        }
                    }
                }	//	for all detail lines
                throw new Exception("Cannot adjust Difference = " + difference
                    + " - You need to change Total Qty or Min Qty");
            }
            else	//	Distibute
            {
                //	New Total Ratio
                Decimal ratioTotal = Env.ZERO;
                for (int i = 0; i < _details.Length; i++)
                {
                    MVATCirculationDetail detail = _details[i];
                    if (runLine.GetVAM_DistributionRunLine_ID() == detail.GetVAM_DistributionRunLine_ID())
                    {
                        if (detail.IsCanAdjust())
                        {
                            ratioTotal = Decimal.Add(ratioTotal, detail.GetRatio());
                        }
                    }
                }
                if (ratioTotal.CompareTo(Env.ZERO) == 0)
                {
                    throw new Exception("Cannot distribute Difference = " + difference

                        + " - You need to change Total Qty or Min Qty");
                }
                //	Distribute
                for (int i = 0; i < _details.Length; i++)
                {
                    MVATCirculationDetail detail = _details[i];
                    if (runLine.GetVAM_DistributionRunLine_ID() == detail.GetVAM_DistributionRunLine_ID())
                    {
                        if (detail.IsCanAdjust())
                        {
                            Decimal diffRatio = Decimal.Round(Decimal.Divide(Decimal.Multiply(detail.GetRatio(), difference),
                                ratioTotal), MidpointRounding.AwayFromZero);
                            log.Fine("Detail=" + detail.ToString()
                                + ", Allocation=" + detail.GetActualAllocation()
                                + ", DiffRatio=" + diffRatio);
                            detail.AdjustQty(diffRatio);
                            detail.Save();
                        }
                    }
                }
            }
            runLine.SetLastDifference(difference);
        }


        /// <summary>
        /// Create Orders
        /// </summary>
        /// <returns>true if created</returns>
        private bool CreateOrders()
        {
            //	Get Counter Org/BP
            int runVAF_Org_ID = _run.GetVAF_Org_ID();
            if (runVAF_Org_ID == 0)
            {
                runVAF_Org_ID = GetCtx().GetVAF_Org_ID();

            }
            MVAFOrg runOrg = MVAFOrg.Get(GetCtx(), runVAF_Org_ID);
            int runVAB_BusinessPartner_ID = runOrg.GetLinkedVAB_BusinessPartner_ID();
            bool counter = !_run.IsCreateSingleOrder()	//	no single Order 
                && runVAB_BusinessPartner_ID > 0						//	Org linked to BP
                && !_docType.IsSOTrx();					//	PO
            MVABBusinessPartner runBPartner = counter ? new MVABBusinessPartner(GetCtx(), runVAB_BusinessPartner_ID, Get_TrxName()) : null;
            if (!counter || runBPartner == null || runBPartner.Get_ID() != runVAB_BusinessPartner_ID)
            {
                counter = false;
            }
            if (counter)
            {
                log.Info("RunBP=" + runBPartner
                    + " - " + _docType);
            }
            log.Info("Single=" + _run.IsCreateSingleOrder()
                + " - " + _docType + ",SO=" + _docType.IsSOTrx());
            log.Fine("Counter=" + counter
                + ",VAB_BusinessPartner_ID=" + runVAB_BusinessPartner_ID + "," + runBPartner);
            //
            MVABBusinessPartner bp = null;
            MVABOrder singleOrder = null;
            MProduct product = null;
            //	Consolidated Order
            if (_run.IsCreateSingleOrder())
            {
                bp = new MVABBusinessPartner(GetCtx(), _run.GetVAB_BusinessPartner_ID(), Get_TrxName());
                if (bp.Get_ID() == 0)
                {
                    throw new ArgumentException("Business Partner not found - VAB_BusinessPartner_ID=" + _run.GetVAB_BusinessPartner_ID());
                }
                //
                if (!_IsTest)
                {
                    singleOrder = new MVABOrder(GetCtx(), 0, Get_TrxName());
                    singleOrder.SetVAB_DocTypesTarget_ID(_docType.GetVAB_DocTypes_ID());
                    singleOrder.SetVAB_DocTypes_ID(_docType.GetVAB_DocTypes_ID());
                    singleOrder.SetIsReturnTrx(_docType.IsReturnTrx());
                    singleOrder.SetIsSOTrx(_docType.IsSOTrx());
                    singleOrder.SetBPartner(bp);
                    if (_run.GetVAB_BPart_Location_ID() != 0)
                    {
                        singleOrder.SetVAB_BPart_Location_ID(_run.GetVAB_BPart_Location_ID());
                    }
                    singleOrder.SetDateOrdered(_DateOrdered);
                    singleOrder.SetDatePromised(_DatePromised);
                    if (!singleOrder.Save())
                    {
                        log.Log(Level.SEVERE, "Order not saved");
                        return false;
                    }
                    _counter++;
                }
            }

            int lastVAB_BusinessPartner_ID = 0;
            int lastVAB_BPart_Location_ID = 0;
            MVABOrder order = null;
            //	For all lines
            for (int i = 0; i < _details.Length; i++)
            {
                MVATCirculationDetail detail = _details[i];

                //	Create Order Header
                if (_run.IsCreateSingleOrder())
                {
                    order = singleOrder;
                }
                //	New Business Partner
                else if (lastVAB_BusinessPartner_ID != detail.GetVAB_BusinessPartner_ID()
                    || lastVAB_BPart_Location_ID != detail.GetVAB_BPart_Location_ID())
                {
                    //	finish order
                    order = null;
                }
                lastVAB_BusinessPartner_ID = detail.GetVAB_BusinessPartner_ID();
                lastVAB_BPart_Location_ID = detail.GetVAB_BPart_Location_ID();

                //	New Order
                if (order == null)
                {
                    bp = new MVABBusinessPartner(GetCtx(), detail.GetVAB_BusinessPartner_ID(), Get_TrxName());
                    if (!_IsTest)
                    {
                        order = new MVABOrder(GetCtx(), 0, Get_TrxName());
                        order.SetVAB_DocTypesTarget_ID(_docType.GetVAB_DocTypes_ID());
                        order.SetIsReturnTrx(_docType.IsReturnTrx());
                        order.SetVAB_DocTypes_ID(_docType.GetVAB_DocTypes_ID());
                        order.SetIsSOTrx(_docType.IsSOTrx());
                        //	Counter Doc
                        if (counter && bp.GetVAF_OrgBP_ID_Int() > 0)
                        {
                            log.Fine("Counter - From_BPOrg=" + bp.GetVAF_OrgBP_ID_Int()
                                + "-" + bp + ", To_BP=" + runBPartner);
                            order.SetVAF_Org_ID(bp.GetVAF_OrgBP_ID_Int());
                            MVAFOrgDetail oi = MVAFOrgDetail.Get(GetCtx(), bp.GetVAF_OrgBP_ID_Int(), null);
                            if (oi.GetVAM_Warehouse_ID() > 0)
                            {
                                order.SetVAM_Warehouse_ID(oi.GetVAM_Warehouse_ID());
                            }
                            order.SetBPartner(runBPartner);
                        }
                        else	//	normal
                        {
                            log.Fine("From_Org=" + runVAF_Org_ID
                                + ", To_BP=" + bp);
                            order.SetVAF_Org_ID(runVAF_Org_ID);
                            order.SetBPartner(bp);
                            if (detail.GetVAB_BPart_Location_ID() != 0)
                            {
                                order.SetVAB_BPart_Location_ID(detail.GetVAB_BPart_Location_ID());
                            }
                        }
                        order.SetDateOrdered(_DateOrdered);
                        order.SetDatePromised(_DatePromised);
                        if (!order.Save())
                        {
                            log.Log(Level.SEVERE, "Order not saved");
                            return false;
                        }
                    }
                }

                //	Line
                if (product == null || product.GetVAM_Product_ID() != detail.GetVAM_Product_ID())
                    product = MProduct.Get(GetCtx(), detail.GetVAM_Product_ID());
                if (_IsTest)
                {
                    AddLog(0, null, detail.GetActualAllocation(),
                        bp.GetName() + " - " + product.GetName());
                    continue;
                }

                //	Create Order Line
                MVABOrderLine line = new MVABOrderLine(order);
                if (counter && bp.GetVAF_OrgBP_ID_Int() > 0)
                {
                    ;	//	don't overwrite counter doc
                }
                else	//	normal - optionally overwrite
                {
                    line.SetVAB_BusinessPartner_ID(detail.GetVAB_BusinessPartner_ID());
                    if (detail.GetVAB_BPart_Location_ID() != 0)
                        line.SetVAB_BPart_Location_ID(detail.GetVAB_BPart_Location_ID());
                }
                //
                line.SetProduct(product);
                line.SetQty(detail.GetActualAllocation());
                line.SetPrice();
                if (!line.Save())
                {
                    log.Log(Level.SEVERE, "OrderLine not saved");
                    return false;
                }
                AddLog(0, null, detail.GetActualAllocation(), order.GetDocumentNo()
                    + ": " + bp.GetName() + " - " + product.GetName());
            }
            //	finish order
            order = null;


            return true;
        }
    }
}

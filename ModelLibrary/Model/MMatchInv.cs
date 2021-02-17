/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MMatchInv
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     08-Jun-2009
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
    public class MMatchInv : X_VAM_MatchInvoice
    {
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MMatchInv).FullName);
        /**
       * 	Get InOut-Invoice Matches
       *	@param ctx context
       *	@param VAM_Inv_InOutLine_ID shipment
       *	@param VAB_InvoiceLine_ID invoice
       *	@param trxName transaction
       *	@return array of matches
       */
        public static MMatchInv[] Get(Ctx ctx, int VAM_Inv_InOutLine_ID, int VAB_InvoiceLine_ID, Trx trxName)
        {
            if (VAM_Inv_InOutLine_ID == 0 || VAB_InvoiceLine_ID == 0)
                return new MMatchInv[] { };
            //
            String sql = "SELECT * FROM VAM_MatchInvoice WHERE VAM_Inv_InOutLine_ID=" + VAM_Inv_InOutLine_ID + " AND VAB_InvoiceLine_ID=" + VAB_InvoiceLine_ID;
            List<MMatchInv> list = new List<MMatchInv>();
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MMatchInv(ctx, dr, trxName));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            MMatchInv[] retValue = new MMatchInv[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /*	Get InOut Invoice Matches
        *	@param ctx context
        *	@param VAM_Inv_InOutLine_ID shipment
        *	@param trxName transaction
        *	@return array of matches
        */
        public static MMatchInv[] Get(Ctx ctx, int VAM_Inv_InOutLine_ID, Trx trxName)
        {
            if (VAM_Inv_InOutLine_ID == 0)
                return new MMatchInv[] { };
            //
            String sql = "SELECT * FROM VAM_MatchInvoice WHERE VAM_Inv_InOutLine_ID=" + VAM_Inv_InOutLine_ID;
            List<MMatchInv> list = new List<MMatchInv>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MMatchInv(ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            MMatchInv[] retValue = new MMatchInv[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
       * 	Get Inv Matches for InOut
       *	@param ctx context
       *	@param VAM_Inv_InOut_ID shipment
       *	@param trxName transaction
       *	@return array of matches
       */
        public static MMatchInv[] GetInOut(Ctx ctx, int VAM_Inv_InOut_ID, Trx trxName)
        {
            if (VAM_Inv_InOut_ID == 0)
                return new MMatchInv[] { };
            //
            String sql = "SELECT * FROM VAM_MatchInvoice m"
                + " INNER JOIN VAM_Inv_InOutLine l ON (m.VAM_Inv_InOutLine_ID=l.VAM_Inv_InOutLine_ID) "
                + "WHERE l.VAM_Inv_InOut_ID=" + VAM_Inv_InOut_ID;
            List<MMatchInv> list = new List<MMatchInv>();
            IDataReader idr = null;
            DataTable dt = null;
            try
            {

                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MMatchInv(ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            { dt = null; }

            MMatchInv[] retValue = new MMatchInv[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /* 	Get Inv Matches for Invoice
         *	@param ctx context
         *	@param VAB_Invoice_ID invoice
         *	@param trxName transaction
         *	@return array of matches
         */
        public static MMatchInv[] GetInvoice(Ctx ctx, int VAB_Invoice_ID, Trx trxName)
        {
            if (VAB_Invoice_ID == 0)
                return new MMatchInv[] { };
            //
            String sql = "SELECT * FROM VAM_MatchInvoice mi"
                + " INNER JOIN VAB_InvoiceLine il ON (mi.VAB_InvoiceLine_ID=il.VAB_InvoiceLine_ID) "
                + "WHERE il.VAB_Invoice_ID=" + VAB_Invoice_ID;
            List<MMatchInv> list = new List<MMatchInv>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MMatchInv(ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            { dt = null; }

            MMatchInv[] retValue = new MMatchInv[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAM_MatchInvoice_ID id
         *	@param trxName transaction
         */
        public MMatchInv(Ctx ctx, int VAM_MatchInvoice_ID, Trx trxName)
            : base(ctx, VAM_MatchInvoice_ID, trxName)
        {
            if (VAM_MatchInvoice_ID == 0)
            {
                //	setDateTrx (new DateTime(System.currentTimeMillis()));
                //	setVAB_InvoiceLine_ID (0);
                //	setVAM_Inv_InOutLine_ID (0);
                //	setVAM_Product_ID (0);
                SetVAM_PFeature_SetInstance_ID(0);
                //	setQty (Env.ZERO);
                SetPosted(false);
                SetProcessed(false);
                SetProcessing(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MMatchInv(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
        * 	Invoice Line Constructor
        *	@param iLine invoice line
        *	@param dateTrx optional date
        *	@param qty matched quantity
        */
        public MMatchInv(MVABInvoiceLine iLine, DateTime? dateTrx, Decimal qty)
            : this(iLine.GetCtx(), 0, iLine.Get_TrxName())
        {
            SetClientOrg(iLine);
            SetVAB_InvoiceLine_ID(iLine.GetVAB_InvoiceLine_ID());
            SetVAM_Inv_InOutLine_ID(iLine.GetVAM_Inv_InOutLine_ID());
            if (dateTrx != null)
                SetDateTrx(dateTrx);
            SetVAM_Product_ID(iLine.GetVAM_Product_ID());
            SetVAM_PFeature_SetInstance_ID(iLine.GetVAM_PFeature_SetInstance_ID());
            SetQty(qty);
            SetProcessed(true);		//	auto
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            //	Set Trx Date
            if (GetDateTrx() == null)
                SetDateTrx(new DateTime(CommonFunctions.CurrentTimeMillis()));
            //	Set Acct Date
            if (GetDateAcct() == null)
            {
                DateTime? ts = GetNewerDateAcct();
                if (ts == null)
                    ts = GetDateTrx();
                SetDateAcct(ts);
            }
            if (GetVAM_PFeature_SetInstance_ID() == 0 && GetVAM_Inv_InOutLine_ID() != 0)
            {
                MVAMInvInOutLine iol = new MVAMInvInOutLine(GetCtx(), GetVAM_Inv_InOutLine_ID(), Get_TrxName());
                SetVAM_PFeature_SetInstance_ID(iol.GetVAM_PFeature_SetInstance_ID());
            }
            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (IsCostCalculated() || IsCostImmediate())
            {
                X_VAM_MatchInvoiceoiceCostTrack costTrack = null;
                int VAM_MatchInvoiceoiceCostTrack_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_MatchInvoiceoiceCostTrack_ID FROM VAM_MatchInvoiceoiceCostTrack WHERE VAM_MatchInvoice_ID = " + GetVAM_MatchInvoice_ID()));
                if (VAM_MatchInvoiceoiceCostTrack_ID > 0)
                {
                    costTrack = new X_VAM_MatchInvoiceoiceCostTrack(GetCtx(), VAM_MatchInvoiceoiceCostTrack_ID, null);
                    costTrack.SetIsCostCalculated(IsCostCalculated());
                    costTrack.SetIsCostImmediate(IsCostImmediate());
                }
                else
                {
                     costTrack = new X_VAM_MatchInvoiceoiceCostTrack(GetCtx(), 0, null);
                     costTrack.SetVAM_MatchInvoice_ID(GetVAM_MatchInvoice_ID());
                     costTrack.SetVAM_Inv_InOutLine_ID(GetVAM_Inv_InOutLine_ID());
                     costTrack.SetVAB_InvoiceLine_ID(GetVAB_InvoiceLine_ID());
                     costTrack.SetQty(GetQty());
                     costTrack.SetVAM_Product_ID(GetVAM_Product_ID());
                     costTrack.SetIsCostCalculated(IsCostCalculated());
                     costTrack.SetIsCostImmediate(IsCostImmediate());
                }
                costTrack.Save();
            }
            return true;
        }

        /**
         * 	Get the later Date Acct from invoice or shipment
         *	@return date or null
         */
        private DateTime? GetNewerDateAcct()
        {
            DateTime? invoiceDate = null;
            DateTime? shipDate = null;

            String sql = "SELECT i.DateAcct "
                + "FROM VAB_InvoiceLine il"
                + " INNER JOIN VAB_Invoice i ON (i.VAB_Invoice_ID=il.VAB_Invoice_ID) "
                + "WHERE VAB_InvoiceLine_ID=" + GetVAB_InvoiceLine_ID();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    invoiceDate =Utility.Util.GetValueOfDateTime(dr[0]);//.getTimestamp(1);
                }
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
            }
            sql = "SELECT io.DateAcct "
                + "FROM VAM_Inv_InOutLine iol"
                + " INNER JOIN VAM_Inv_InOut io ON (io.VAM_Inv_InOut_ID=iol.VAM_Inv_InOut_ID) "
                + "WHERE iol.VAM_Inv_InOutLine_ID=" + GetVAM_Inv_InOutLine_ID();
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    shipDate = Utility.Util.GetValueOfDateTime(dr[0]);//.getTimestamp(1);
                }

            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }

            if (invoiceDate == null)
                return (DateTime?)shipDate;
            if (shipDate == null)
                return (DateTime?)invoiceDate;
            //if (invoiceDate.after(shipDate))
            if (invoiceDate > shipDate)
            {
                return (DateTime?)invoiceDate;
            }
            return (DateTime?)shipDate;
        }

        /**
         * 	Before Delete
         *	@return true if acct was deleted
         */
        protected override bool BeforeDelete()
        {
            if (IsPosted())
            {
                if (!MPeriod.IsOpen(GetCtx(), GetDateTrx(), MVABMasterDocType.DOCBASETYPE_MATCHINVOICE, GetVAF_Org_ID()))
                    return false;
                SetPosted(false);
                return MActualAcctDetail.Delete(Table_ID, Get_ID(), Get_TrxName()) >= 0;
            }
            return true;
        }

        /**
         * 	After Delete
         *	@param success success
         *	@return success
         */
        protected override bool AfterDelete(bool success)
        {
            if (success)
            {
                //	Get Order and decrease invoices
                MVABInvoiceLine iLine = new MVABInvoiceLine(GetCtx(), GetVAB_InvoiceLine_ID(), Get_TrxName());
                int VAB_OrderLine_ID = iLine.GetVAB_OrderLine_ID();
                if (VAB_OrderLine_ID == 0)
                {
                    MVAMInvInOutLine ioLine = new MVAMInvInOutLine(GetCtx(), GetVAM_Inv_InOutLine_ID(), Get_TrxName());
                    VAB_OrderLine_ID = ioLine.GetVAB_OrderLine_ID();
                }
                //	No Order Found
                if (VAB_OrderLine_ID == 0)
                    return success;
                //	Find MatchPO
                MMatchPO[] mPO = MMatchPO.Get(GetCtx(), VAB_OrderLine_ID, GetVAB_InvoiceLine_ID(), Get_TrxName());
                for (int i = 0; i < mPO.Length; i++)
                {
                    if (mPO[i].GetVAM_Inv_InOutLine_ID() == 0)
                        mPO[i].Delete(true);
                    else
                    {
                        mPO[i].SetVAB_InvoiceLine_ID(null);
                        mPO[i].Save();
                    }
                }
            }
            return success;
        }
    }
}

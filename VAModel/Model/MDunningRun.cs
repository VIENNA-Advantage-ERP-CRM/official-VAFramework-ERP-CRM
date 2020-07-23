/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDunningRun
 * Purpose        : Dunning Run Model
 * Class Used     : X_C_DunningRun
 * Chronological    Development
 * Deepak          10-Nov-2009
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
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MDunningRun : X_C_DunningRun
    {
        #region Private variable
        private MDunningLevel _level = null;
        private MDunningRunEntry[] _entries = null;
        #endregion

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_DunningRun_ID"></param>
        /// <param name="trxName"></param>
        public MDunningRun(Ctx ctx, int C_DunningRun_ID, Trx trxName)
            : base(ctx, C_DunningRun_ID, trxName)
        {
            if (C_DunningRun_ID == 0)
            {
                SetDunningDate(new DateTime(CommonFunctions.CurrentTimeMillis()));
                SetProcessed(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MDunningRun(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        ///  	Get Dunning Level
        /// </summary>
        /// <returns>level</returns>
        public MDunningLevel GetLevel()
        {
            if (_level == null)
            {
                _level = new MDunningLevel(GetCtx(), GetC_DunningLevel_ID(), Get_TrxName());
            }
            return _level;
        }

        /// <summary>
        /// Get Entries
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>entries</returns>
        public MDunningRunEntry[] GetEntries(bool requery)
        {
            return GetEntries(requery, false);
        }

        /// <summary>
        /// Get Entries
        /// </summary>
        /// <param name="requery">requery requery</param>
        /// <param name="onlyInvoices">only invoices</param>
        /// <returns>entries</returns>
        public MDunningRunEntry[] GetEntries(bool requery, bool onlyInvoices)
        {
            if (_entries != null && !requery)
            {
                return _entries;
            }

            String sql = "SELECT * FROM C_DunningRunEntry WHERE C_DunningRun_ID=" + GetC_DunningRun_ID();
            List<MDunningRunEntry> list = new List<MDunningRunEntry>();
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
                    MDunningRunEntry thisEntry = new MDunningRunEntry(GetCtx(), dr, Get_TrxName());
                    if (!(onlyInvoices && thisEntry.HasInvoices()))
                    {
                        list.Add(new MDunningRunEntry(GetCtx(), dr, Get_TrxName()));
                    }
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
                if (idr != null)
                {
                    idr.Close();
                }
            }

            _entries = new MDunningRunEntry[list.Count];
            _entries = list.ToArray();
            return _entries;
        }

        /// <summary>
        /// Delete all Entries
        /// </summary>
        /// <param name="force">delete also processed records</param>
        /// <returns>true if deleted</returns>
        public bool DeleteEntries(bool force)
        {
            GetEntries(true);
            for (int i = 0; i < _entries.Length; i++)
            {
                MDunningRunEntry entry = _entries[i];
                entry.Delete(force);
            }
            bool ok = GetEntries(true).Length == 0;
            if (ok)
            {
                _entries = null;
            }
            return ok;
        }

        /// <summary>
        /// Get/Create Entry for BPartner
        /// </summary>
        /// <param name="C_BPartner_ID">business partner</param>
        /// <param name="C_Currency_ID">currency</param>
        /// <param name="SalesRep_ID">sales rep</param>
        /// <returns>entry</returns>
        public MDunningRunEntry GetEntry(int C_BPartner_ID, int C_Currency_ID, int SalesRep_ID)
        {
            // TODO: Related BP
            int C_BPartnerRelated_ID = C_BPartner_ID;
            //
            GetEntries(false);
            for (int i = 0; i < _entries.Length; i++)
            {
                MDunningRunEntry entry = _entries[i];
                if (entry.GetC_BPartner_ID() == C_BPartnerRelated_ID)
                {
                    return entry;
                }
            }
            //	New Entry
            MDunningRunEntry entry1 = new MDunningRunEntry(this);
            MBPartner bp = new MBPartner(GetCtx(), C_BPartnerRelated_ID, Get_TrxName());
            entry1.SetBPartner(bp, true);	//	AR hardcoded
            //
            //if (entry1.GetSalesRep_ID() == 0)
            // Change done bu mohit to set sales rep selected.- 1 May 2019.
            if (SalesRep_ID > 0)
            {
                entry1.SetSalesRep_ID(SalesRep_ID);
            }
            entry1.SetC_Currency_ID(C_Currency_ID);
            //
            _entries = null;
            return entry1;
        }

    }

}

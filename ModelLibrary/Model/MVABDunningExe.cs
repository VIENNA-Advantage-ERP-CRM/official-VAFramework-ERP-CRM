/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVABDunningExe
 * Purpose        : Dunning Run Model
 * Class Used     : X_VAB_DunningExe
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
//////using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVABDunningExe : X_VAB_DunningExe
    {
        #region Private variable
        private MVABDunningStep _level = null;
        private MVABDunningExeEntry[] _entries = null;
        #endregion

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_DunningExe_ID"></param>
        /// <param name="trxName"></param>
        public MVABDunningExe(Ctx ctx, int VAB_DunningExe_ID, Trx trxName)
            : base(ctx, VAB_DunningExe_ID, trxName)
        {
            if (VAB_DunningExe_ID == 0)
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
        public MVABDunningExe(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        ///  	Get Dunning Level
        /// </summary>
        /// <returns>level</returns>
        public MVABDunningStep GetLevel()
        {
            if (_level == null)
            {
                _level = new MVABDunningStep(GetCtx(), GetVAB_DunningStep_ID(), Get_TrxName());
            }
            return _level;
        }

        /// <summary>
        /// Get Entries
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>entries</returns>
        public MVABDunningExeEntry[] GetEntries(bool requery)
        {
            return GetEntries(requery, false);
        }

        /// <summary>
        /// Get Entries
        /// </summary>
        /// <param name="requery">requery requery</param>
        /// <param name="onlyInvoices">only invoices</param>
        /// <returns>entries</returns>
        public MVABDunningExeEntry[] GetEntries(bool requery, bool onlyInvoices)
        {
            if (_entries != null && !requery)
            {
                return _entries;
            }

            String sql = "SELECT * FROM VAB_DunningExeEntry WHERE VAB_DunningExe_ID=" + GetVAB_DunningExe_ID();
            List<MVABDunningExeEntry> list = new List<MVABDunningExeEntry>();
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
                    MVABDunningExeEntry thisEntry = new MVABDunningExeEntry(GetCtx(), dr, Get_TrxName());
                    if (!(onlyInvoices && thisEntry.HasInvoices()))
                    {
                        list.Add(new MVABDunningExeEntry(GetCtx(), dr, Get_TrxName()));
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

            _entries = new MVABDunningExeEntry[list.Count];
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
                MVABDunningExeEntry entry = _entries[i];
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
        /// <param name="VAB_BusinessPartner_ID">business partner</param>
        /// <param name="VAB_Currency_ID">currency</param>
        /// <param name="SalesRep_ID">sales rep</param>
        /// <returns>entry</returns>
        public MVABDunningExeEntry GetEntry(int VAB_BusinessPartner_ID, int VAB_Currency_ID, int SalesRep_ID)
        {
            // TODO: Related BP
            int VAB_BusinessPartnerRelated_ID = VAB_BusinessPartner_ID;
            //
            GetEntries(false);
            for (int i = 0; i < _entries.Length; i++)
            {
                MVABDunningExeEntry entry = _entries[i];
                if (entry.GetVAB_BusinessPartner_ID() == VAB_BusinessPartnerRelated_ID)
                {
                    return entry;
                }
            }
            //	New Entry
            MVABDunningExeEntry entry1 = new MVABDunningExeEntry(this);
            MVABBusinessPartner bp = new MVABBusinessPartner(GetCtx(), VAB_BusinessPartnerRelated_ID, Get_TrxName());
            entry1.SetBPartner(bp, true);	//	AR hardcoded
            //
            //if (entry1.GetSalesRep_ID() == 0)
            // Change done bu mohit to set sales rep selected.- 1 May 2019.
            if (SalesRep_ID > 0)
            {
                entry1.SetSalesRep_ID(SalesRep_ID);
            }
            entry1.SetVAB_Currency_ID(VAB_Currency_ID);
            //
            _entries = null;
            return entry1;
        }

    }

}

/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDunningRunEntry
 * Purpose        : Dunning Run Entry Model
 * Class Used     : X_C_DunningRunEntry
 * Chronological    Development
 * Raghunandan     10-Nov-2009
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
    public class MDunningRunEntry : X_C_DunningRunEntry
    {
        //Logger							
        private static VLogger _log = VLogger.GetVLogger(typeof(MPayment).FullName);
        //Parent				
        private MDunningRun m_parent = null;


        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_DunningRunEntry_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MDunningRunEntry(Ctx ctx, int C_DunningRunEntry_ID, Trx trxName)
            : base(ctx, C_DunningRunEntry_ID, trxName)
        {

            if (C_DunningRunEntry_ID == 0)
            {
                SetAmt(Env.ZERO);
                SetQty(Env.ZERO);
                SetProcessed(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MDunningRunEntry(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        public MDunningRunEntry(MDunningRun parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetC_DunningRun_ID(parent.GetC_DunningRun_ID());
            m_parent = parent;
        }

        /// <summary>
        /// Set BPartner
        /// </summary>
        /// <param name="bp">partner</param>
        /// <param name="isSOTrx">SO</param>
        public void SetBPartner(MBPartner bp, bool isSOTrx)
        {
            SetC_BPartner_ID(bp.GetC_BPartner_ID());
            MBPartnerLocation[] locations = GetLocations();
            //	Location

            for (int i = 0; i < locations.Length; i++)
            {
                MBPartnerLocation location = locations[i];
                if (!location.IsActive())
                {
                    continue;
                }
                if ((location.IsPayFrom() && isSOTrx)
                    || (location.IsRemitTo() && !isSOTrx))
                {
                    SetC_BPartner_Location_ID(location.GetC_BPartner_Location_ID());
                    break;
                }
            }
            //}
            if (GetC_BPartner_Location_ID() == 0)
            {
                String msg = "@C_BPartner_ID@ " + bp.GetName();
                if (isSOTrx)
                {
                    msg += " @No@ @IsPayFrom@";
                }
                else
                {
                    msg += " @No@ @IsRemitTo@";
                }
                //throw new ArgumentException(msg);
                log.SaveInfo("", msg);
                return;
            }

            //	User with location
            // Change done by mohit to pick users sorted by date updated. 7 May 2019.
            MUser[] users = GetOfBPartner(GetCtx(), bp.GetC_BPartner_ID());
            if (users.Length == 1)
            {
                if (users[0].IsEmail() || users[0].GetNotificationType() == MUser.NOTIFICATIONTYPE_EMail
                        || users[0].GetNotificationType() == MUser.NOTIFICATIONTYPE_EMailPlusNotice || users[0].GetNotificationType() == MUser.NOTIFICATIONTYPE_EMailPlusFaxEMail)
                {
                    SetAD_User_ID(users[0].GetAD_User_ID());
                }
            }
            else
            {
                for (int i = 0; i < users.Length; i++)
                {
                    MUser user = users[i];
                    if (user.GetC_BPartner_Location_ID() == GetC_BPartner_Location_ID() && (user.IsEmail() || user.GetNotificationType() == MUser.NOTIFICATIONTYPE_EMail
                        || user.GetNotificationType() == MUser.NOTIFICATIONTYPE_EMailPlusNotice || user.GetNotificationType() == MUser.NOTIFICATIONTYPE_EMailPlusFaxEMail))
                    {
                        SetAD_User_ID(users[i].GetAD_User_ID());
                        break;
                    }
                }
            }
            //
            int SalesRep_ID = bp.GetSalesRep_ID();
            if (SalesRep_ID != 0)
            {
                SetSalesRep_ID(SalesRep_ID);
            }
        }



        /// <summary>
        /// Get active Users of BPartner sorted by date updated desc.
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_BPartner_ID">id</param>
        /// <returns>array of users</returns>
        /// Writer - Mohit , Date - 7 may 2019
        public static MUser[] GetOfBPartner(Ctx ctx, int C_BPartner_ID)
        {
            List<MUser> list = new List<MUser>();
            String sql = "SELECT * FROM AD_User WHERE C_BPartner_ID=" + C_BPartner_ID + " AND IsActive='Y' ORDER BY Updated DESC ";

            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MUser(ctx, dr, null));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MUser[] retValue = new MUser[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        /// <summary>
        /// Get All Locations of a business partner sorted by updated descending.
        /// </summary>
        /// <returns>locations</returns>
        /// Writer - Mohit, Date - * May 2019.
        public MBPartnerLocation[] GetLocations()
        {
            MBPartnerLocation[] _locations = null;

            List<MBPartnerLocation> list = new List<MBPartnerLocation>();
            String sql = "SELECT * FROM C_BPartner_Location WHERE C_BPartner_ID=" + GetC_BPartner_ID() + " AND IsActive='Y' ORDER BY Updated DESC ";
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MBPartnerLocation(GetCtx(), dr, Get_TrxName()));
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            _locations = new MBPartnerLocation[list.Count];
            _locations = list.ToArray();
            return _locations;
        }

        /// <summary>
        /// Get Lines
        /// </summary>
        /// <returns>Array of all lines for this Run</returns>
        public MDunningRunLine[] GetLines()
        {
            return GetLines(false);
        }

        /// <summary>
        /// Get Lines
        /// </summary>
        /// <param name="onlyInvoices">only with invoices </param>
        /// <returns>Array of all lines for this Run</returns>
        public MDunningRunLine[] GetLines(bool onlyInvoices)
        {
            List<MDunningRunLine> list = new List<MDunningRunLine>();
            String sql = "SELECT * FROM C_DunningRunLine WHERE C_DunningRunEntry_ID=" + Get_ID();
            if (onlyInvoices)
            {
                sql += " AND C_Invoice_ID IS NOT NULL";
            }
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
                    list.Add(new MDunningRunLine(GetCtx(), dr, Get_TrxName()));
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
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }

            //
            MDunningRunLine[] retValue = new MDunningRunLine[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Check whether has Invoices
        /// </summary>
        /// <returns>true if it has Invoices</returns>
        public bool HasInvoices()
        {
            bool retValue = false;
            String sql = "SELECT COUNT(*) FROM C_DunningRunLine WHERE C_DunningRunEntry_ID=" + Get_ID() + " AND C_Invoice_ID IS NOT NULL";
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                if (idr.Read())
                {
                    if (Utility.Util.GetValueOfInt(idr[0]) > 0) // dr.getInt(1)
                    {
                        retValue = true;
                    }
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }

            return retValue;
        }

        /// <summary>
        /// Get Parent
        /// </summary>
        /// <returns>Dunning Run</returns>
        private MDunningRun GetParent()
        {
            if (m_parent == null)
            {
                m_parent = new MDunningRun(GetCtx(), GetC_DunningRun_ID(), Get_TrxName());
            }
            return m_parent;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Set Processed
            if (IsProcessed() && Is_ValueChanged("Processed"))
            {
                MDunningRunLine[] theseLines = GetLines();
                for (int i = 0; i < theseLines.Length; i++)
                {
                    theseLines[i].SetProcessed(true);
                    theseLines[i].Save(Get_TrxName());
                }
            }
            return true;
        }
    }
}

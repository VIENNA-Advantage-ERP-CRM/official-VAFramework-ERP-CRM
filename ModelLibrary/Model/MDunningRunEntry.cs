/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDunningRunEntry
 * Purpose        : Dunning Run Entry Model
 * Class Used     : X_VAB_DunningExeEntry
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
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MDunningRunEntry : X_VAB_DunningExeEntry
    {
        //Logger							
        private static VLogger _log = VLogger.GetVLogger(typeof(MPayment).FullName);
        //Parent				
        private MDunningRun m_parent = null;


        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_DunningExeEntry_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MDunningRunEntry(Ctx ctx, int VAB_DunningExeEntry_ID, Trx trxName)
            : base(ctx, VAB_DunningExeEntry_ID, trxName)
        {

            if (VAB_DunningExeEntry_ID == 0)
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
            SetVAB_DunningExe_ID(parent.GetVAB_DunningExe_ID());
            m_parent = parent;
        }

        /// <summary>
        /// Set BPartner
        /// </summary>
        /// <param name="bp">partner</param>
        /// <param name="isSOTrx">SO</param>
        public void SetBPartner(MBPartner bp, bool isSOTrx)
        {
            SetVAB_BusinessPartner_ID(bp.GetVAB_BusinessPartner_ID());
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
                    SetVAB_BPart_Location_ID(location.GetVAB_BPart_Location_ID());
                    break;
                }
            }
            //}
            if (GetVAB_BPart_Location_ID() == 0)
            {
                String msg = "@VAB_BusinessPartner_ID@ " + bp.GetName();
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
            MUser[] users = GetOfBPartner(GetCtx(), bp.GetVAB_BusinessPartner_ID());
            if (users.Length == 1)
            {
                if (users[0].IsEmail() || users[0].GetNotificationType() == MUser.NOTIFICATIONTYPE_EMail
                        || users[0].GetNotificationType() == MUser.NOTIFICATIONTYPE_EMailPlusNotice || users[0].GetNotificationType() == MUser.NOTIFICATIONTYPE_EMailPlusFaxEMail)
                {
                    SetVAF_UserContact_ID(users[0].GetVAF_UserContact_ID());
                }
            }
            else
            {
                for (int i = 0; i < users.Length; i++)
                {
                    MUser user = users[i];
                    if (user.GetVAB_BPart_Location_ID() == GetVAB_BPart_Location_ID() && (user.IsEmail() || user.GetNotificationType() == MUser.NOTIFICATIONTYPE_EMail
                        || user.GetNotificationType() == MUser.NOTIFICATIONTYPE_EMailPlusNotice || user.GetNotificationType() == MUser.NOTIFICATIONTYPE_EMailPlusFaxEMail))
                    {
                        SetVAF_UserContact_ID(users[i].GetVAF_UserContact_ID());
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
        /// <param name="VAB_BusinessPartner_ID">id</param>
        /// <returns>array of users</returns>
        /// Writer - Mohit , Date - 7 may 2019
        public static MUser[] GetOfBPartner(Ctx ctx, int VAB_BusinessPartner_ID)
        {
            List<MUser> list = new List<MUser>();
            String sql = "SELECT * FROM VAF_UserContact WHERE VAB_BusinessPartner_ID=" + VAB_BusinessPartner_ID + " AND IsActive='Y' ORDER BY Updated DESC ";

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
            String sql = "SELECT * FROM VAB_BPart_Location WHERE VAB_BusinessPartner_ID=" + GetVAB_BusinessPartner_ID() + " AND IsActive='Y' ORDER BY Updated DESC ";
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
            String sql = "SELECT * FROM VAB_DunningExeLine WHERE VAB_DunningExeEntry_ID=" + Get_ID();
            if (onlyInvoices)
            {
                sql += " AND VAB_Invoice_ID IS NOT NULL";
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
            String sql = "SELECT COUNT(*) FROM VAB_DunningExeLine WHERE VAB_DunningExeEntry_ID=" + Get_ID() + " AND VAB_Invoice_ID IS NOT NULL";
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
                m_parent = new MDunningRun(GetCtx(), GetVAB_DunningExe_ID(), Get_TrxName());
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

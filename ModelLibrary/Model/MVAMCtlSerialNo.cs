/********************************************************
 * Module Name    : Workflow
 * Purpose        : 
 * Class Used     : X_VAM_CtlSerialNo
 * Chronological Development
 * Veena Pandey     16-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAMCtlSerialNo : X_VAM_CtlSerialNo
    {

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_CtlSerialNo_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMCtlSerialNo(Ctx ctx, int VAM_CtlSerialNo_ID, Trx trxName)
            : base(ctx, VAM_CtlSerialNo_ID, trxName)
        {
            if (VAM_CtlSerialNo_ID == 0)
            {
                //	setVAM_CtlSerialNo_ID (0);
                SetStartNo(1);
                SetCurrentNext(1);
                SetIncrementNo(1);
                //	setName (null);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAMCtlSerialNo(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Create new Lot.
        /// Increments Current Next and Commits
        /// </summary>
        /// <returns>saved Lot</returns>
        public String CreateSerNo()
        {
            StringBuilder name = new StringBuilder();
            if (GetPrefix() != null)
                name.Append(GetPrefix());
            int no = GetCurrentNext();
            name.Append(no);
            if (GetSuffix() != null)
                name.Append(GetSuffix());
            //
            no += GetIncrementNo();
            SetCurrentNext(no);
            Save();
            return name.ToString();
        }

        /// <summary>
        /// Create Organization level Sequence No from Serial No Control
        /// </summary>
        /// <param name="po"></param>
        /// <returns>Serial No</returns>
        public String CreateDefiniteSerNo(PO po)
        {
            Boolean isUseOrgLevel = Util.GetValueOfBool(Get_Value("IsOrgLevelSequence"));
            String orgColumn = Util.GetValueOfString(Get_Value("OrgColumn"));

            int startNo = GetStartNo();
            int incrementNo = GetIncrementNo();
            String prefix = GetPrefix();
            String suffix = GetSuffix();
            String selectSQL = null;

            // if Organization level check box is true, the Get Current next from Serila No tab.
            if (isUseOrgLevel)
            {
                selectSQL = "SELECT y.CurrentNext, s.CurrentNext, y.prefix, y.suffix "
                        + "FROM VAM_SerialNoCtl_No y, VAM_CtlSerialNo s "
                        + "WHERE y.VAM_CtlSerialNo_ID = s.VAM_CtlSerialNo_ID "
                        + "AND s.VAM_CtlSerialNo_ID = " + GetVAM_CtlSerialNo_ID()
                        + " AND y.VAF_Org_ID = @param1"
                        + " AND s.IsActive='Y' "
                        + "ORDER BY s.VAF_Client_ID DESC";
            }
            else
            {
                selectSQL = "SELECT s.CurrentNext "
                        + "FROM VAM_CtlSerialNo s "
                        + "WHERE s.VAM_CtlSerialNo_ID = " + GetVAM_CtlSerialNo_ID()
                        + " AND s.IsActive='Y' "
                        + "ORDER BY s.VAF_Client_ID DESC";
            }

            int docOrg_ID = 0;
            int next = -1;

            String updateSQL = "";
            DataSet rs;
            try
            {
                if (po != null && orgColumn != null && orgColumn.Length > 0)
                {
                    docOrg_ID = Util.GetValueOfInt(po.Get_Value(orgColumn));
                }

                if (isUseOrgLevel)
                {
                    selectSQL = selectSQL.Replace("@param1", docOrg_ID.ToString());
                    rs = DB.ExecuteDataset(selectSQL, null, null);

                    // Check organization level document sequence settings and if exist then override general settings.
                    if (rs != null && rs.Tables[0].Rows.Count > 0)
                    {
                        string _prefix = Convert.ToString(rs.Tables[0].Rows[0]["Prefix"]);
                        string _suffix = Convert.ToString(rs.Tables[0].Rows[0]["suffix"]);

                        // if any of these fields contains data, then override parent tab's data....
                        if (!string.IsNullOrEmpty(_prefix) || !string.IsNullOrEmpty(_suffix))
                        {
                            prefix = _prefix;
                            suffix = _suffix;
                        }
                    }
                }
                else
                {
                    rs = DB.ExecuteDataset(selectSQL, null, null);
                }

                if (rs != null && rs.Tables.Count > 0 && rs.Tables[0].Rows.Count > 0)
                {
                    if (log.IsLoggable(Level.FINE))
                        log.Fine("VAM_CtlSerialNo_ID=" + GetVAM_CtlSerialNo_ID());

                    // Update current next on Serial No Control.
                    if (isUseOrgLevel)
                        updateSQL = "UPDATE VAM_SerialNoCtl_No SET CurrentNext = CurrentNext + " + incrementNo + " WHERE VAM_CtlSerialNo_ID= " + GetVAM_CtlSerialNo_ID() + " AND VAF_Org_ID=" + docOrg_ID;
                    else
                        updateSQL = "UPDATE VAM_CtlSerialNo SET CurrentNext = CurrentNext + " + incrementNo + " WHERE VAM_CtlSerialNo_ID=" + GetVAM_CtlSerialNo_ID();

                    next = int.Parse(rs.Tables[0].Rows[0]["CurrentNext"].ToString());

                    if (DB.ExecuteQuery(updateSQL, null, null) < 0)
                    {
                        next = -2;
                    }
                }
                else
                { // did not find Serial no .
                    if (isUseOrgLevel)
                    {   // create Serial no (CurrentNo = StartNo + IncrementNo) for this Organization and return first number (=StartNo)
                        next = startNo;

                        X_VAM_SerialNoCtl_No seqno = new X_VAM_SerialNoCtl_No(po.GetCtx(), 0, null);
                        seqno.SetVAM_CtlSerialNo_ID(GetVAM_CtlSerialNo_ID());
                        seqno.SetVAF_Org_ID(docOrg_ID);
                        seqno.SetCurrentNext(startNo + incrementNo);
                        seqno.Save();
                    }
                    else    // standard
                    {
                        log.Warning("(Serial No)- no record found - " + GetName());
                        next = -2;
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "(Serial No)", e);
                next = -2;
            }

            //	Error
            if (next < 0)
                return null;

            //	create DocumentNo
            StringBuilder doc = new StringBuilder();
            if (prefix != null && prefix.Length > 0)
                doc.Append(prefix);

            doc.Append(next);
            if (suffix != null && suffix.Length > 0)
                doc.Append(suffix);
            String documentNo = doc.ToString();
            return documentNo;
        }
    }
}

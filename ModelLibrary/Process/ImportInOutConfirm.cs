/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ImportInOutConfirm
 * Purpose        : Import Confirmations
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           12-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class ImportInOutConfirm : ProcessEngine.SvrProcess
    {
        /**	Client to be imported to		*/
        private int _AD_Client_ID = 0;
        /**	Delete old Imported			*/
        private bool _DeleteOldImported = false;
        /**	Import						*/
        private int _I_InOutLineConfirm_ID = 0;

        /// <summary>
        /// Prepare - e.g., get Parameters.
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
                else if (name.Equals("AD_Client_ID"))
                    _AD_Client_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("DeleteOldImported"))
                    _DeleteOldImported = "Y".Equals(para[i].GetParameter());
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            _I_InOutLineConfirm_ID = GetRecord_ID();
        }	//	prepare

        /// <summary>
        /// doIt
        /// </summary>
        /// <returns>Info</returns>
        protected override String DoIt()
        {
            log.Info("I_InOutLineConfirm_ID=" + _I_InOutLineConfirm_ID);
            StringBuilder sql = null;
            int no = 0;
            String clientCheck = " AND AD_Client_ID=" + _AD_Client_ID;

            //	Delete Old Imported
            if (_DeleteOldImported)
            {
                sql = new StringBuilder("DELETE FROM I_InOutLineConfirm "
                      + "WHERE I_IsImported='Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Delete Old Impored =" + no);
            }

            //	Set IsActive, Created/Updated
            sql = new StringBuilder("UPDATE I_InOutLineConfirm "
                + "SET IsActive = COALESCE (IsActive, 'Y'),"
                + " Created = COALESCE (Created, SysDate),"
                + " CreatedBy = COALESCE (CreatedBy, 0),"
                + " Updated = COALESCE (Updated, SysDate),"
                + " UpdatedBy = COALESCE (UpdatedBy, 0),"
                + " I_ErrorMsg = NULL,"
                + " I_IsImported = 'N' "
                + "WHERE I_IsImported<>'Y' OR I_IsImported IS NULL");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Info("Reset=" + no);

            //	Set Client from Name
            sql = new StringBuilder("UPDATE I_InOutLineConfirm i "
                + "SET AD_Client_ID=COALESCE (AD_Client_ID,").Append(_AD_Client_ID).Append(") "
                + "WHERE (AD_Client_ID IS NULL OR AD_Client_ID=0)"
                + " AND I_IsImported<>'Y'");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Client from Value=" + no);

            //	Error Confirmation Line
            String ts = DataBase.DB.IsPostgreSQL() ? "COALESCE(I_ErrorMsg,'')" : "I_ErrorMsg";  //java bug, it could not be used directly
            sql = new StringBuilder("UPDATE I_InOutLineConfirm i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Confirmation Line, '"
                + "WHERE (M_InOutLineConfirm_ID IS NULL OR M_InOutLineConfirm_ID=0"
                + " OR NOT EXISTS (SELECT * FROM M_InOutLineConfirm c WHERE i.M_InOutLineConfirm_ID=c.M_InOutLineConfirm_ID))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid InOutLineConfirm=" + no);

            //	Error Confirmation No
            sql = new StringBuilder("UPDATE I_InOutLineConfirm i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Missing Confirmation No, '"
                + "WHERE (ConfirmationNo IS NULL OR ConfirmationNo='')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid ConfirmationNo=" + no);

            //	Qty
            sql = new StringBuilder("UPDATE I_InOutLineConfirm i "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Target<>(Confirmed+Difference+Scrapped), ' "
                + "WHERE EXISTS (SELECT * FROM M_InOutLineConfirm c "
                    + "WHERE i.M_InOutLineConfirm_ID=c.M_InOutLineConfirm_ID"
                    + " AND c.TargetQty<>(i.ConfirmedQty+i.ScrappedQty+i.DifferenceQty))"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            if (no != 0)
                log.Warning("Invalid Qty=" + no);

            Commit();

            /*********************************************************************/

            IDataReader idr = null;
            sql = new StringBuilder("SELECT * FROM I_InOutLineConfirm "
                + "WHERE I_IsImported='N'").Append(clientCheck)
                .Append(" ORDER BY I_InOutLineConfirm_ID");
            no = 0;
            try
            {
                //pstmt = DataBase.prepareStatement (sql.ToString(), Get_TrxName());
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                while (idr.Read())
                {
                    X_I_InOutLineConfirm importLine = new X_I_InOutLineConfirm(GetCtx(), idr, Get_TrxName());
                    MInOutLineConfirm confirmLine = new MInOutLineConfirm(GetCtx(),
                        importLine.GetM_InOutLineConfirm_ID(), Get_TrxName());
                    if (confirmLine.Get_ID() == 0
                        || confirmLine.Get_ID() != importLine.GetM_InOutLineConfirm_ID())
                    {
                        importLine.SetI_IsImported(X_I_InOutLineConfirm.I_ISIMPORTED_No);
                        importLine.SetI_ErrorMsg("ID Not Found");
                        importLine.Save();
                    }
                    else
                    {
                        confirmLine.SetConfirmationNo(importLine.GetConfirmationNo());
                        confirmLine.SetConfirmedQty(importLine.GetConfirmedQty());
                        confirmLine.SetDifferenceQty(importLine.GetDifferenceQty());
                        confirmLine.SetScrappedQty(importLine.GetScrappedQty());
                        confirmLine.SetDescription(importLine.GetDescription());
                        if (confirmLine.Save())
                        {
                            //	Import
                            importLine.SetI_IsImported(X_I_InOutLineConfirm.I_ISIMPORTED_Yes);
                            importLine.SetProcessed(true);
                            if (importLine.Save())
                                no++;
                        }
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
                log.Log(Level.SEVERE, sql.ToString(), e);
            }

            return "@Updated@ #" + no;
        }	//	doIt

    }

}

/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : FactAcctReset
 * Purpose        : Accounting Fact Reset
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           21-Nov-2009
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

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class FactAcctReset : ProcessEngine.SvrProcess
    {
        /**	Client Parameter		*/
        private int _VAF_Client_ID = 0;
        /** Table Parameter			*/
        private int _VAF_TableView_ID = 0;
        /**	Delete Parameter		*/
        private Boolean _DeletePosting = false;

        private int _countReset = 0;
        private int _countDelete = 0;
        private bool bypass = false;

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
                else if (name.Equals("VAF_Client_ID"))
                {

                    _VAF_Client_ID = Utility.Util.GetValueOfInt(Utility.Util.GetValueOfDecimal(para[i].GetParameter()));
                }
                else if (name.Equals("VAF_TableView_ID"))
                {
                    //_VAF_TableView_ID = ((BigDecimal)para[i].getParameter()).intValue();
                    _VAF_TableView_ID = Utility.Util.GetValueOfInt(Utility.Util.GetValueOfDecimal(para[i].GetParameter()));
                }
                else if (name.Equals("DeletePosting"))
                {
                    _DeletePosting = "Y".Equals(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }	//	prepare

        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>(clear text)</returns>
        protected override String DoIt()
        {
            log.Info("VAF_Client_ID=" + _VAF_Client_ID
                + ", VAF_TableView_ID=" + _VAF_TableView_ID + ", DeletePosting=" + _DeletePosting);
            //	List of Tables with Accounting Consequences
            String sql = "SELECT VAF_TableView_ID, TableName "
                + "FROM VAF_TableView t "
                + "WHERE t.IsView='N'";
            if (_VAF_TableView_ID > 0)
            {
                sql += " AND t.VAF_TableView_ID=" + _VAF_TableView_ID;
            }
            sql += " AND EXISTS (SELECT * FROM VAF_Column c "
                    + "WHERE t.VAF_TableView_ID=c.VAF_TableView_ID AND c.ColumnName='Posted' AND c.IsActive='Y')";
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                while (idr.Read())
                {
                    int VAF_TableView_ID = Utility.Util.GetValueOfInt(idr[0]);// rs.getInt(1);
                    String TableName = Utility.Util.GetValueOfString(idr[1]);// rs.getString(2);
                    if (_DeletePosting)
                    {
                        Delete(TableName, VAF_TableView_ID);
                    }
                    else
                    {
                        Reset(TableName);
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
                log.Log(Level.SEVERE, sql, e);
            }

            //	Balances
            if (_DeletePosting)
            {
                //  FinBalance.updateBalanceClient(GetCtx(), _VAF_Client_ID, true);		//	delete
            }
            //
            return "@Updated@ = " + _countReset + ", @Deleted@ = " + _countDelete;
        }	//	doIt
        /// <summary>
        /// Reset Accounting Table and update count
        /// </summary>
        /// <param name="TableName">table</param>
        private void Reset(String TableName)
        {
            String sql = "UPDATE " + TableName
                + " SET Processing='N' WHERE VAF_Client_ID=" + _VAF_Client_ID
                + " AND (Processing<>'N' OR Processing IS NULL)";
            int unlocked = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            sql = "UPDATE " + TableName
                + " SET Posted='N' WHERE VAF_Client_ID=" + _VAF_Client_ID
                + " AND (Posted NOT IN ('Y','N') OR Posted IS NULL) AND Processed='Y'";
            int invalid = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (unlocked + invalid != 0)
            {
                log.Fine(TableName + " - Unlocked=" + unlocked + " - Invalid=" + invalid);
            }
            _countReset += unlocked + invalid;
        }	//	reset

        /// <summary>
        /// Delete Accounting Table where period status is open and update count.
        /// </summary>
        /// <param name="TableName">table name</param>
        /// <param name="VAF_TableView_ID">table</param>
        private void Delete(String TableName, int VAF_TableView_ID)
        {
            Reset(TableName);
            _countReset = 0;
            bypass = false;
            //
            /// Change by Mohit, Check applied for GL Journal,Profit & Loss, Income Tax Reset accounting, and logic changes to pick table ID from
            /// MTable rather than static constructor of mclass. Askes By Mukesh Sir, Amit Date=04/08/2017

            String docBaseType = null;
            if (VAF_TableView_ID == MTable.Get_Table_ID("C_Invoice"))
            {
                docBaseType = "IN ('" + MDocBaseType.DOCBASETYPE_APINVOICE
                    + "','" + MDocBaseType.DOCBASETYPE_APCREDITMEMO
                    + "','" + MDocBaseType.DOCBASETYPE_ARINVOICE
                    + "','" + MDocBaseType.DOCBASETYPE_ARCREDITMEMO
                    + "','" + MDocBaseType.DOCBASETYPE_ARPROFORMAINVOICE + "')";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("M_InOut"))
            {
                docBaseType = "IN ('" + MDocBaseType.DOCBASETYPE_MATERIALDELIVERY
                    + "','" + MDocBaseType.DOCBASETYPE_MATERIALRECEIPT + "')";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("C_Payment"))
            {
                docBaseType = "IN ('" + MDocBaseType.DOCBASETYPE_APPAYMENT
                    + "','" + MDocBaseType.DOCBASETYPE_ARRECEIPT + "')";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("C_Order"))
            {
                docBaseType = "IN ('" + MDocBaseType.DOCBASETYPE_SALESORDER
                    + "','" + MDocBaseType.DOCBASETYPE_PURCHASEORDER + "')";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("C_ProjectIssue"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_PROJECTISSUE + "'";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("C_BankStatement"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_BANKSTATEMENT + "'";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("C_Cash"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_CASHJOURNAL + "'";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("C_AllocationHdr"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_PAYMENTALLOCATION + "'";
            }

            else if (VAF_TableView_ID == MTable.Get_Table_ID("GL_Journal"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_GLJOURNAL + "'";

            }
            //	else if (VAF_TableView_ID == M.Table_ID)
            // {
            //		docBaseType = "= '" + MDocBaseType.DOCBASETYPE_GLDocument + "'";
            //  }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("M_Movement"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_MATERIALMOVEMENT + "'";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("M_Requisition"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_PURCHASEREQUISITION + "'";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("M_Inventory"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_MATERIALPHYSICALINVENTORY + "'";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("M_Production"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_MATERIALPRODUCTION + "'";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("M_MatchInv"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_MATCHINVOICE + "'";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("M_MatchPO"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_MATCHPO + "'";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("C_IncomeTax"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_INCOMETAX + "'";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("C_ProfitLoss"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_PROFITLOSS + "'";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("VAFAM_AssetDepreciation"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_FIXASSET + "'";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("VAFAM_AssetImpairEnhance"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_FIXASSETIMPAIRMENT + "'";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("VA026_LCDetail"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_LETTEROFCREDIT + "'";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("VA024_ObsoleteInventory"))
            {
                docBaseType = "= '" + MDocBaseType.DOCBASETYPE_INVENTORYPOVISION + "'";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("VA027_PostDatedCheck"))
            {
                docBaseType = " IN ( '" + MDocBaseType.DOCBASETYPE_PDCPAYABLE + "' ,'" + MDocBaseType.DOCBASETYPE_PDCRECEIVABLE + "' )";
            }
            else if (VAF_TableView_ID == MTable.Get_Table_ID("VAMFG_M_WorkOrder") || VAF_TableView_ID == MTable.Get_Table_ID("VAMFG_M_WrkOdrTransaction"))
            {
                bypass = true;
                docBaseType = null;
            }


            //
            if (docBaseType == null && !bypass)
            {
                String s = TableName + ": Unknown DocBaseType";
                log.Severe(s);
                AddLog(s);
                docBaseType = "";
                return;
            }
            else if (!string.IsNullOrEmpty(docBaseType))
                docBaseType = " AND pc.DocBaseType " + docBaseType;

            //	Doc
            String sql1 = "UPDATE " + TableName + " doc"
                + " SET Posted='N', Processing='N' "
                + "WHERE VAF_Client_ID=" + _VAF_Client_ID
                + " AND (Posted<>'N' OR Posted IS NULL OR Processing<>'N' OR Processing IS NULL)"
                + " AND EXISTS (SELECT * FROM C_PeriodControl pc"
                    + " INNER JOIN Fact_Acct fact ON (fact.C_Period_ID=pc.C_Period_ID) "
                    + "WHERE pc.PeriodStatus = 'O'" + docBaseType
                    + " AND fact.VAF_TableView_ID=" + VAF_TableView_ID
                    + " AND fact.Record_ID=doc." + TableName + "_ID)";
            int reset = DataBase.DB.ExecuteQuery(sql1, null, Get_TrxName());
            //	Fact
            String sql2 = "DELETE FROM Fact_Acct fact "
                + "WHERE VAF_Client_ID=" + _VAF_Client_ID
                + " AND VAF_TableView_ID=" + VAF_TableView_ID
                + " AND EXISTS (SELECT * FROM C_PeriodControl pc "
                    + "WHERE pc.PeriodStatus = 'O'" + docBaseType
                    + " AND fact.C_Period_ID=pc.C_Period_ID)";
            int deleted = DataBase.DB.ExecuteQuery(sql2, null, Get_TrxName());
            //
            log.Info(TableName + "(" + VAF_TableView_ID + ") - Reset=" + reset + " - Deleted=" + deleted);
            String s1 = TableName + " - Reset=" + reset + " - Deleted=" + deleted;
            AddLog(s1);
            if (reset == 0)
            {
                log.Finest(sql1);
            }
            if (deleted == 0)
            {
                log.Finest(sql2);
            }
            //
            _countReset += reset;
            _countDelete += deleted;
        }	//	delete

    }	//	FactAcctReset

}

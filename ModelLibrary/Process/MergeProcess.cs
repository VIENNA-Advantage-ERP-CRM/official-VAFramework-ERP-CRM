/********************************************************
 * Project Name   : VAdvantage
 * Class Name     :  MergeProcess 
 * Purpose        :  Merge Process 
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           03-feb-2010
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
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Data.SqlClient;


using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class MergeProcess : ProcessEngine.SvrProcess
    {
        /**	Merge Parameter		*/
        private int from_ID = 0;
        private int to_ID = 0;

        static private String AD_ORG_ID = "AD_Org_ID";
        static private String C_BPARTNER_ID = "C_BPartner_ID";
        static private String AD_USER_ID = "AD_User_ID";
        static private String M_PRODUCT_ID = "M_Product_ID";

        private String columnName = null;

        /** Tables to delete (not update) for AD_Org	*/
        static private String[] s_delete_Org = new String[] { "AD_OrgInfo" };
        /** Tables to delete (not update) for AD_User	*/
        static private String[] s_delete_User = new String[] { "AD_User_Roles" };
        /** Tables to delete (not update) for C_BPartner	*/
        static private String[] s_delete_BPartner = new String[]
		{"C_BP_Employee_Acct", "C_BP_Vendor_Acct", "C_BP_Customer_Acct", 
		"T_Aging", "FRPT_BP_Customer_Acct"};                               // Added Table FRPT_BP_Customer_Acct by Bharat on 01 May 2019
        /** Tables to delete (not update) for M_Product		*/
        static private String[] s_delete_Product = new String[]
		{"M_Product_PO", "M_Replenish", "T_Replenish", 
		"M_ProductPrice", "M_Product_Costing",                          
		"M_Product_Trl", "M_Product_Acct", "FRPT_Product_Acct",             // Added Table FRPT_Product_Acct and M_Cost by Bharat on 19 April 2019
        "M_Cost"};		//	M_Storage
        /**	Total Count			*/
        private int m_totalCount = 0;
        /** Error Log			*/
        private StringBuilder m_errorLog = new StringBuilder();
        /**	Connection			*/
        //private Connection		m_con = null;
        /**	Logger			*/

        private String[] m_deleteTables = null;

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();

            for (int i = 0; (i < para.Length); i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("AD_Org_ID"))
                {

                    from_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();					
                }

                else if (name.Equals("AD_Org_To_ID"))
                {
                    to_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                    columnName = AD_ORG_ID;
                    m_deleteTables = s_delete_Org;
                }
                else if (name.Equals("AD_User_ID"))
                {
                    from_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();					
                }
                else if (name.Equals("AD_User_To_ID"))
                {
                    to_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                    m_deleteTables = s_delete_User;
                    columnName = AD_USER_ID;
                }
                else if (name.Equals("C_BPartner_ID"))
                {
                    from_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();					
                }
                else if (name.Equals("C_BPartner_To_ID"))
                {
                    to_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                    m_deleteTables = s_delete_BPartner;
                    columnName = C_BPARTNER_ID;
                }
                else if (name.Equals("M_Product_ID"))
                {
                    from_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();					
                }
                else if (name.Equals("M_Product_To_ID"))
                {
                    to_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                    m_deleteTables = s_delete_Product;
                    columnName = M_PRODUCT_ID;
                }
            }
        }	//	prepare

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns> Message (clear text)</returns>
        protected override String DoIt()
        {

            log.Info("doIt ");
            String fromValue = null;
            String toValue = null;


            if (from_ID < 0)
            {
                throw new ArgumentException("Invalid From " + columnName + ": " + from_ID);
            }
            if (to_ID < 0)
            {
                throw new ArgumentException("Invalid To " + columnName + ": " + to_ID);
            }

            // JID_1226: Merge From and To cannot be the same
            if(from_ID == to_ID)
            {
                return Msg.GetMsg(GetCtx(), "MergeSame");
            }

            // JID_1226: Both Users must have same business partner or no Business partner
            if (columnName == AD_USER_ID)
            {
                MUser fromUsr = new MUser(GetCtx(), from_ID, Get_TrxName());
                MUser toUsr = new MUser(GetCtx(), to_ID, Get_TrxName());
                if(fromUsr.GetC_BPartner_ID() != toUsr.GetC_BPartner_ID())
                {
                    return Msg.GetMsg(GetCtx(), "MergeUserError");
                }
            }

            String fromSql = " SELECT Name FROM " + columnName.Substring(0, columnName.Length - 3) + " WHERE " + columnName + " = @param ";
            SqlParameter[] param = null;
            IDataReader idr = null;

            try
            {

                //pstmt = DataBase.prepareStatement(fromSql, null);
                //pstmt.setString(1, String.valueOf(from_ID));
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@param", Utility.Util.GetValueOfString(from_ID));
                idr = DataBase.DB.ExecuteReader(fromSql, param, Get_TrxName());

                while (idr.Read())
                {
                    fromValue = Utility.Util.GetValueOfString(idr[0]);// rs.getString(1);

                }
                idr.Close();
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, columnName, ex);
            }
            String toSql = " SELECT Name FROM " + columnName.Substring(0, columnName.Length - 3) + " WHERE " + columnName + " = @param ";

            try
            {

                //pstmt = DataBase.prepareStatement(toSql, null);
                //pstmt.setString(1, String.valueOf(to_ID));
                param = new SqlParameter[1];
                //ResultSet rs = pstmt.executeQuery();
                param[0] = new SqlParameter("@param", Utility.Util.GetValueOfString(to_ID));
                idr = DataBase.DB.ExecuteReader(toSql, param, Get_TrxName());
                while (idr.Read())
                {
                    toValue = Utility.Util.GetValueOfString(idr[0]);// rs.getString(1);

                }
                idr.Close();
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, columnName, ex);
            }
            String msg = Msg.Translate(GetCtx(), "MergeFrom") + " = " + fromValue
            + "\n" + Msg.Translate(GetCtx(), "MergeTo") + " = " + toValue + "\n";
            bool success = Merge(columnName, from_ID, to_ID);
            PostMerge(columnName, to_ID);

            if (success)
            {
                return msg + " #" + m_totalCount;
            }
            else
            {
                throw new SystemException(" " + m_errorLog.ToString());
            }
        }	//	doIt

        /// <summary>
        /// Merge
        /// </summary>
        /// <param name="ColumnName">Column Name</param>
        /// <param name="from_ID">From</param>
        /// <param name="to_ID">To</param>
        /// <returns>bool, true if merged</returns>
        private bool Merge(String ColumnName, int from_ID, int to_ID)
        {
            String TableName = ColumnName.Substring(0, ColumnName.Length - 3);
            log.Config(ColumnName
                + " - From=" + from_ID + ",To=" + to_ID);

            bool success = true;
            m_totalCount = 0;
            m_errorLog = new StringBuilder();
            String sql = "SELECT t.TableName, c.ColumnName "
                + "FROM AD_Table t"
                + " INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID) "
                + "WHERE t.IsView='N'"
                    + " AND t.TableName NOT IN ('C_TaxDeclarationAcct')"
                    + " AND c.ColumnSQL is NULL AND ("              // No Virtual Column
                    + "(c.ColumnName=@param1 AND c.IsKey='N')"		//	#1 - direct
                + " OR "
                    + "c.AD_Reference_Value_ID IN "				//	Table Reference
                        + "(SELECT rt.AD_Reference_ID FROM AD_Ref_Table rt"
                        + " INNER JOIN AD_Column cc ON (rt.AD_Table_ID=cc.AD_Table_ID AND rt.Column_Key_ID=cc.AD_Column_ID) "
                        + "WHERE cc.IsKey='Y' AND cc.ColumnName=@param2)"	//	#2
                + ") "
                + "ORDER BY t.LoadSeq DESC";
            SqlParameter[] param = new SqlParameter[2];
            IDataReader idr = null;
            //Savepoint sp = null;
            try
            {
                //m_con = DataBase.createConnection(false, Connection.TRANSACTION_READ_COMMITTED);

                //sp = m_con.setSavepoint("merge");
                //
                //pstmt = DataBase.prepareStatement(sql, null);
                //pstmt.setString(1, ColumnName);
                param[0] = new SqlParameter("@param1", ColumnName);
                //pstmt.setString(2, ColumnName);
                param[1] = new SqlParameter("@param2", ColumnName);
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                while (idr.Read())
                {
                    String tName = Utility.Util.GetValueOfString(idr[0]);// rs.getString(1);
                    String cName = Utility.Util.GetValueOfString(idr[1]); //rs.getString(2);
                    if (!TableName.Equals(tName))	//	to be sure - sql should prevent it
                    {
                        int count = MergeTable(tName, cName, from_ID, to_ID);
                        if (count < 0)
                        {
                            success = false;
                        }
                        else
                        {
                            m_totalCount += count;
                        }
                    }
                }
                idr.Close();
                //
                log.Config("Success=" + success
                    + " - " + ColumnName + " - From=" + from_ID + ",To=" + to_ID);
                if (success)
                {
                    sql = "DELETE FROM " + TableName + " WHERE " + ColumnName + "=" + from_ID;
                    //Statement stmt = m_con.createStatement();
                    int count = 0;
                    try
                    {
                        //count = stmt.executeUpdate (sql);
                        count = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                        //count = SqlExec.ExecuteQuery.ExecuteNonQuery(sql, null);
                        if (count != 1)
                        {
                            m_errorLog.Append(Env.NL).Append("DELETE FROM ").Append(TableName)
                                .Append(" - Count=").Append(count);
                            success = false;
                        }

                    }
                    catch (Exception ex1)
                    {
                        m_errorLog.Append(Env.NL).Append("DELETE FROM ").Append(TableName)
                            .Append(" - ").Append(ex1.Message.ToString());
                        success = false;
                    }

                }
                //
                if (success)
                {
                    //_con.commit();
                    Commit();
                }
                else
                {
                    Rollback();
                    // m_con.rollback(sp);
                }
                // m_con.close();
                // m_con = null;

            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, ColumnName, ex);
            }
            //	Cleanup

            return success;
        }	//	merge

        /// <summary>
        /// Delete or Update Data in the Table
        /// </summary>
        /// <param name="TableName">Table Name</param>
        /// <param name="ColumnName">Column Name</param>
        /// <param name="from_ID">From</param>
        /// <param name="to_ID">TO</param>
        /// <returns>int, -1 for error or number of changes</returns>
        private int MergeTable(String TableName, String ColumnName, int from_ID, int to_ID)
        {
            log.Fine(TableName + "." + ColumnName + " - From=" + from_ID + ",To=" + to_ID);
            StringBuilder sql = new StringBuilder("UPDATE " + TableName
                + " SET " + ColumnName + "=" + to_ID
                + " WHERE " + ColumnName + "=" + from_ID);
            bool delete = false;
            for (int i = 0; i < m_deleteTables.Length; i++)
            {
                if (m_deleteTables[i].Equals(TableName))
                {
                    delete = true;
                    sql.Clear();
                    sql.Append("DELETE FROM " + TableName + " WHERE " + ColumnName + "=" + from_ID);
                }
            }

            int count = -1;

            try
            {
                //Statement stmt = m_con.createStatement ();
                try
                {
                    //count = stmt.executeUpdate (sql);
                    count = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                    if (count < 0)          // Added in the Message if not Deleted or Updated
                    {
                        m_errorLog.Append(Env.NL)
                       .Append(delete ? "DELETE FROM " : "UPDATE ")
                       .Append(TableName).Append(" - ").Append(sql.ToString());
                    }
                    log.Fine(count
                        + (delete ? " -Delete- " : " -Update- ") + TableName);
                }
                catch (Exception ex1)
                {
                    count = -1;
                    m_errorLog.Append(Env.NL)
                        .Append(delete ? "DELETE FROM " : "UPDATE ")
                        .Append(TableName).Append(" - ").Append(ex1.ToString())
                        .Append(" - ").Append(sql.ToString());
                }

            }
            catch (Exception ex)
            {
                count = -1;
                m_errorLog.Append(Env.NL)
                    .Append(delete ? "DELETE FROM " : "UPDATE ")
                    .Append(TableName).Append(" - ").Append(ex.ToString())
                    .Append(" - ").Append(sql.ToString());
            }
            return count;
        }	//	mergeTable

        /// <summary>
        /// Post Merge
        /// </summary>
        /// <param name="ColumnName">column name</param>
        /// <param name="to_ID">ID</param>
        private void PostMerge(String ColumnName, int to_ID)
        {
            if (ColumnName.Equals(AD_ORG_ID))
            {

            }
            else if (ColumnName.Equals(AD_USER_ID))
            {

            }
            else if (ColumnName.Equals(C_BPARTNER_ID))
            {
                MBPartner bp = new MBPartner(GetCtx(), to_ID, Get_TrxName());
                if (bp.Get_ID() != 0)
                {
                    MPayment[] payments = MPayment.GetOfBPartner(GetCtx(), bp.GetC_BPartner_ID(), Get_TrxName());
                    for (int i = 0; i < payments.Length; i++)
                    {
                        MPayment payment = payments[i];
                        if (payment.TestAllocation())
                            payment.Save();
                    }
                    MInvoice[] invoices = MInvoice.GetOfBPartner(GetCtx(), bp.GetC_BPartner_ID(), Get_TrxName());
                    for (int i = 0; i < invoices.Length; i++)
                    {
                        MInvoice invoice = invoices[i];
                        if (invoice.TestAllocation())
                            invoice.Save();
                    }
                    bp.SetTotalOpenBalance();
                    bp.SetActualLifeTimeValue();
                    bp.Save();
                }
            }
            else if (ColumnName.Equals(M_PRODUCT_ID))
            {

            }
        }	//	postMerge
    }	
}
/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ImportAccount
 * Purpose        : Import Accounts from I_ElementValue
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
    public class ImportAccount : ProcessEngine.SvrProcess
    {
        /**	Client to be imported to		*/
        private int _VAF_Client_ID = 0;
        /** Default Element					*/
        private int _VAB_Element_ID = 0;
        /**	Update Default Accounts			*/
        private bool _updateDefaultAccounts = false;
        /** Create New Combination			*/
        private bool _createNewCombination = true;

        /**	Delete old Imported				*/
        private bool _deleteOldImported = false;

        /** Effective						*/
        private DateTime? _DateValue = null;

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
                    _VAF_Client_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("VAB_Element_ID"))
                    _VAB_Element_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                else if (name.Equals("UpdateDefaultAccounts"))
                    _updateDefaultAccounts = "Y".Equals(para[i].GetParameter());
                else if (name.Equals("CreateNewCombination"))
                    _createNewCombination = "Y".Equals(para[i].GetParameter());
                else if (name.Equals("DeleteOldImported"))
                    _deleteOldImported = "Y".Equals(para[i].GetParameter());
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            if (_DateValue == null)
                _DateValue = Utility.Util.GetValueOfDateTime(DateTime.Now);// new Timestamp (System.currentTimeMillis());
        }	//	prepare


        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>Info</returns>
        protected override String DoIt()
        {
            StringBuilder sql = null;
            int no = 0;
            String clientCheck = " AND VAF_Client_ID=" + _VAF_Client_ID;

            //	****	Prepare	****

            //	Delete Old Imported
            if (_deleteOldImported)
            {
                sql = new StringBuilder("DELETE FROM I_ElementValue "
                    + "WHERE I_IsImported='Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Delete Old Impored =" + no);
            }

            //	Set Client, Org, IsActive, Created/Updated
            sql = new StringBuilder("UPDATE I_ElementValue "
                + "SET VAF_Client_ID = COALESCE (VAF_Client_ID, ").Append(_VAF_Client_ID).Append("),"
                + " VAF_Org_ID = COALESCE (VAF_Org_ID, 0),"
                + " IsActive = COALESCE (IsActive, 'Y'),"
                + " Created = COALESCE (Created, SysDate),"
                + " CreatedBy = COALESCE (CreatedBy, 0),"
                + " Updated = COALESCE (Updated, SysDate),"
                + " UpdatedBy = COALESCE (UpdatedBy, 0),"
                + " I_ErrorMsg = NULL,"
                + " I_IsImported = 'N' "
                + "WHERE I_IsImported<>'Y' OR I_IsImported IS NULL");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Reset=" + no);

            //	****	Prepare	****

            //	Set Element
            if (_VAB_Element_ID != 0)
            {
                sql = new StringBuilder("UPDATE I_ElementValue "
                    + "SET ElementName=(SELECT Name FROM VAB_Element WHERE VAB_Element_ID=")
                        .Append(_VAB_Element_ID).Append(") "
                    + "WHERE ElementName IS NULL AND VAB_Element_ID IS NULL"
                    + " AND I_IsImported<>'Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Set Element Default=" + no);
            }
            //
            sql = new StringBuilder("UPDATE I_ElementValue i "
                + "SET VAB_Element_ID = (SELECT VAB_Element_ID FROM VAB_Element e"
                + " WHERE i.ElementName=e.Name AND i.VAF_Client_ID=e.VAF_Client_ID)"
                + "WHERE VAB_Element_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Element=" + no);
            //
            sql = new StringBuilder("UPDATE I_ElementValue "
                + "SET I_IsImported='E', I_ErrorMsg='ERR=Invalid Element, ' "
                + "WHERE VAB_Element_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid Element=" + no);

            //	No Name, Value
            sql = new StringBuilder("UPDATE I_ElementValue "
                + "SET I_IsImported='E', I_ErrorMsg='ERR=No Name, ' "
                + "WHERE (Value IS NULL OR Name IS NULL)"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid Name=" + no);


            //	Set Column
            sql = new StringBuilder("UPDATE I_ElementValue i "
                + "SET VAF_Column_ID = (SELECT VAF_Column_ID FROM VAF_Column c"
                + " WHERE UPPER(i.Default_Account)=UPPER(c.ColumnName)"
                + " AND c.VAF_TableView_ID IN (315,266) AND VAF_Control_Ref_ID=25) "
                + "WHERE Default_Account IS NOT NULL AND VAF_Column_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set Column=" + no);
            //
            String ts = DataBase.DB.IsPostgreSQL() ? "COALESCE(I_ErrorMsg,'')"
                : "I_ErrorMsg";  //java bug, it could not be used directly
            sql = new StringBuilder("UPDATE I_ElementValue "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid Column, ' "
                + "WHERE VAF_Column_ID IS NULL AND Default_Account IS NOT NULL"
                + " AND UPPER(Default_Account)<>'DEFAULT_ACCT'"		//	ignore default account
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid Column=" + no);

            //	Set Post* Defaults (ignore errors)
            String[] yColumns = new String[] { "PostActual", "PostBudget", "PostStatistical", "PostEncumbrance" };
            for (int i = 0; i < yColumns.Length; i++)
            {
                sql = new StringBuilder("UPDATE I_ElementValue SET ")
                    .Append(yColumns[i]).Append("='Y' WHERE ")
                    .Append(yColumns[i]).Append(" IS NULL OR ")
                    .Append(yColumns[i]).Append(" NOT IN ('Y','N')"
                    + " AND I_IsImported<>'Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Set " + yColumns[i] + " Default=" + no);
            }
            //	Summary
            sql = new StringBuilder("UPDATE I_ElementValue "
                + "SET IsSummary='N' "
                + "WHERE IsSummary IS NULL OR IsSummary NOT IN ('Y','N')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set IsSummary Default=" + no);

            //	Doc Controlled
            sql = new StringBuilder("UPDATE I_ElementValue "
                + "SET IsDocControlled = CASE WHEN VAF_Column_ID IS NOT NULL THEN 'Y' ELSE 'N' END "
                + "WHERE IsDocControlled IS NULL OR IsDocControlled NOT IN ('Y','N')"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set IsDocumentControlled Default=" + no);

            //	Check Account Type A (E) L M O R
            sql = new StringBuilder("UPDATE I_ElementValue "
                + "SET AccountType='E' "
                + "WHERE AccountType IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set AccountType Default=" + no);
            //
            sql = new StringBuilder("UPDATE I_ElementValue "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid AccountType, ' "
                + "WHERE AccountType NOT IN ('A','E','L','M','O','R')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid AccountType=" + no);
            //	Check Account Sign (N) C B
            sql = new StringBuilder("UPDATE I_ElementValue "
                + "SET AccountSign='N' "
                + "WHERE AccountSign IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set AccountSign Default=" + no);
            //
            sql = new StringBuilder("UPDATE I_ElementValue "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid AccountSign, ' "
                + "WHERE AccountSign NOT IN ('N','C','D')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid AccountSign=" + no);

            //	No Value
            sql = new StringBuilder("UPDATE I_ElementValue "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=No Key, ' "
                + "WHERE (Value IS NULL OR Value='')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid Key=" + no);

            //	****	Update ElementValue from existing
            sql = new StringBuilder("UPDATE I_ElementValue i "
                + "SET VAB_Acct_Element_ID=(SELECT VAB_Acct_Element_ID FROM VAB_Acct_Element ev"
                + " INNER JOIN VAB_Element e ON (ev.VAB_Element_ID=e.VAB_Element_ID)"
                + " WHERE i.VAB_Element_ID=e.VAB_Element_ID AND i.VAF_Client_ID=e.VAF_Client_ID"
                + " AND i.Value=ev.Value) "
                + "WHERE VAB_Acct_Element_ID IS NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Found ElementValue=" + no);

            Commit();

            //	-------------------------------------------------------------------
            int noInsert = 0;
            int noUpdate = 0;

            //	Go through Records
            sql = new StringBuilder("SELECT * "
                + "FROM I_ElementValue "
                + "WHERE I_IsImported='N'").Append(clientCheck)
                .Append(" ORDER BY I_ElementValue_ID");
            IDataReader idr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql.ToString(), Get_TrxName());
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                while (idr.Read())
                {
                    X_VAI_Acct impEV = new X_VAI_Acct(GetCtx(), idr, Get_TrxName());
                    int VAB_Acct_Element_ID = impEV.GetVAB_Acct_Element_ID();
                    int I_ElementValue_ID = impEV.GetI_ElementValue_ID();

                    //	****	Create/Update ElementValue
                    if (VAB_Acct_Element_ID == 0)		//	New
                    {
                        MVABAcctElement ev = new MVABAcctElement(impEV);
                        if (ev.Save())
                        {
                            noInsert++;
                            impEV.SetVAB_Acct_Element_ID(ev.GetVAB_Acct_Element_ID());
                            impEV.SetI_IsImported(X_VAI_Acct.I_ISIMPORTED_Yes);
                            impEV.Save();
                        }
                        else
                        {
                            sql = new StringBuilder("UPDATE I_ElementValue i "
                                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||").Append(DataBase.DB.TO_STRING("Insert ElementValue "))
                                .Append("WHERE I_ElementValue_ID=").Append(I_ElementValue_ID);
                            DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                        }
                    }
                    else							//	Update existing
                    {
                        MVABAcctElement ev = new MVABAcctElement(GetCtx(), VAB_Acct_Element_ID, null);
                        if (ev.Get_ID() != VAB_Acct_Element_ID)
                        {
                        }
                        ev.Set(impEV);
                        if (ev.Save())
                        {
                            noUpdate++;
                            impEV.SetI_IsImported(X_VAI_Acct.I_ISIMPORTED_Yes);
                            impEV.Save();
                        }
                        else
                        {
                            sql = new StringBuilder("UPDATE I_ElementValue i "
                                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||").Append(DataBase.DB.TO_STRING("Update ElementValue"))
                                .Append("WHERE I_ElementValue_ID=").Append(I_ElementValue_ID);
                            DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                        }
                    }
                }	//	for all I_Product
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                throw new Exception("create", e);
            }

            //	Set Error to indicator to not imported
            sql = new StringBuilder("UPDATE I_ElementValue "
                + "SET I_IsImported='N', Updated=SysDate "
                + "WHERE I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, Utility.Util.GetValueOfDecimal(no), "@Errors@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsert), "@VAB_Acct_Element_ID@: @Inserted@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noUpdate), "@VAB_Acct_Element_ID@: @Updated@");

            Commit();

            //	*****	Set Parent
            sql = new StringBuilder("UPDATE I_ElementValue i "
                + "SET ParentElementValue_ID=(SELECT VAB_Acct_Element_ID"
                + " FROM VAB_Acct_Element ev WHERE i.VAB_Element_ID=ev.VAB_Element_ID"
                + " AND i.ParentValue=ev.Value AND i.VAF_Client_ID=ev.VAF_Client_ID) "
                + "WHERE ParentElementValue_ID IS NULL"
                + " AND I_IsImported='Y' AND Processed='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Found Parent ElementValue=" + no);
            //
            sql = new StringBuilder("UPDATE I_ElementValue "
                + "SET I_ErrorMsg=" + ts + "||'Info=ParentNotFound, ' "
                + "WHERE ParentElementValue_ID IS NULL AND ParentValue IS NOT NULL"
                + " AND I_IsImported='Y' AND Processed='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Not Found Patent ElementValue=" + no);
            //
            sql = new StringBuilder("SELECT i.ParentElementValue_ID, i.I_ElementValue_ID,"
                + " e.VAF_TreeInfo_ID, i.VAB_Acct_Element_ID, i.Value||'-'||i.Name AS Info "
                + "FROM I_ElementValue i"
                + " INNER JOIN VAB_Element e ON (i.VAB_Element_ID=e.VAB_Element_ID) "
                + "WHERE i.VAB_Acct_Element_ID IS NOT NULL AND e.VAF_TreeInfo_ID IS NOT NULL"
                + " AND i.ParentElementValue_ID IS NOT NULL"
                + " AND i.I_IsImported='Y' AND Processed='N' AND i.VAF_Client_ID=").Append(_VAF_Client_ID);
            int noParentUpdate = 0;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql.ToString(), Get_TrxName());
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                //
                String updateSQL = "UPDATE VAF_TreeInfoChild SET Parent_ID=@param1, SeqNo=@param2 "
                    + "WHERE VAF_TreeInfo_ID=@param3 AND Node_ID=@param4";
                //PreparedStatement updateStmt = DataBase.prepareStatement(updateSQL, Get_TrxName());
                SqlParameter[] param = new SqlParameter[4];
                //IDataReader idr=null;
                while (idr.Read())
                {
                    //updateStmt.setInt(1, rs.getInt(1));		//	Parent
                    param[0] = new SqlParameter("@param1", Utility.Util.GetValueOfInt(idr[0]));
                    //updateStmt.setInt(2, rs.getInt(2));		//	SeqNo (assume sequenec in import is the same)
                    param[1] = new SqlParameter("@param2", Utility.Util.GetValueOfInt(idr[1]));
                    //updateStmt.setInt(3, rs.getInt(3));		//	Tree
                    param[2] = new SqlParameter("@param3", Utility.Util.GetValueOfInt(idr[2]));
                    //updateStmt.setInt(4, rs.getInt(4));		//	Node
                    param[3] = new SqlParameter("@param4", Utility.Util.GetValueOfInt(idr[3]));
                    try
                    {
                        no = DataBase.DB.ExecuteQuery(updateSQL, param, Get_TrxName());
                        noParentUpdate += no;
                    }
                    catch (Exception ex)
                    {
                        log.Log(Level.SEVERE, "(ParentUpdate)", ex);
                        no = 0;
                    }
                    if (no == 0)
                    {
                        log.Info("Parent not found for " + Utility.Util.GetValueOfString(idr[4]));// rs.getString(5));
                    }
                }
                idr.Close();

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, "(ParentUpdateLoop) " + sql.ToString(), e);
            }
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noParentUpdate), "@ParentElementValue_ID@: @Updated@");

            //	Reset Processing Flag
            sql = new StringBuilder("UPDATE I_ElementValue "
                + "SET Processing='-'"
                + "WHERE I_IsImported='Y' AND Processed='N' AND Processing='Y'"
                + " AND VAB_Acct_Element_ID IS NOT NULL")
                .Append(clientCheck);
            if (_updateDefaultAccounts)
                sql.Append(" AND VAF_Column_ID IS NULL");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Reset Processing Flag=" + no);

            if (_updateDefaultAccounts)
                UpdateDefaults(clientCheck);

            //	Update Description
            sql = new StringBuilder("SELECT * FROM VAB_Acct_ValidParameter vc "
                + "WHERE EXISTS (SELECT * FROM I_ElementValue i "
                    + "WHERE vc.Account_ID=i.VAB_Acct_Element_ID)");

            //	Done
            sql = new StringBuilder("UPDATE I_ElementValue "
                + "SET Processing='N', Processed='Y'"
                + "WHERE I_IsImported='Y'")
                .Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Processed=" + no);

            return "";
        }	//	doIt


        /// <summary>
        ///	Update Default Accounts
        /// </summary>
        /// <param name="clientCheck">client where cluase</param>
        private void UpdateDefaults(String clientCheck)
        {
            log.Config("CreateNewCombination=" + _createNewCombination);

            //	****	Update Defaults
            StringBuilder sql = new StringBuilder("SELECT VAB_AccountBook_ID FROM VAB_AccountBook_Element "
                + "WHERE VAB_Element_ID=@param").Append(clientCheck);
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql.ToString(), Get_TrxName());

                //pstmt.setInt(1, _VAB_Element_ID);
                param[0] = new SqlParameter("@param", _VAB_Element_ID);
                idr = DataBase.DB.ExecuteReader(sql.ToString(), param, Get_TrxName());
                while (idr.Read())
                {
                    UpdateDefaultAccounts(Utility.Util.GetValueOfInt(idr[0]));// (rs.getInt(1));
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

            //	Default Account		DEFAULT_ACCT
            sql = new StringBuilder("UPDATE VAB_AccountBook_Element e "
                + "SET VAB_Acct_Element_ID=(SELECT VAB_Acct_Element_ID FROM I_ElementValue i"
                + " WHERE e.VAB_Element_ID=i.VAB_Element_ID AND i.VAB_Acct_Element_ID IS NOT NULL"
                + " AND UPPER(i.Default_Account)='DEFAULT_ACCT') "
                + "WHERE EXISTS (SELECT * FROM I_ElementValue i"
                + " WHERE e.VAB_Element_ID=i.VAB_Element_ID AND i.VAB_Acct_Element_ID IS NOT NULL"
                + " AND UPPER(i.Default_Account)='DEFAULT_ACCT' "
                + "	AND i.I_IsImported='Y' AND i.Processing='-')")
                .Append(clientCheck);
            int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, Utility.Util.GetValueOfDecimal(no), "@VAB_AccountBook_Element_ID@: @Updated@");
        }	//	updateDefaults

        /// <summary>
        ///	Update Default Accounts.
        // _Default.xxxx = VAB_Acct_ValidParameter_ID  =>  Account_ID=VAB_Acct_Element_ID
        /// </summary>
        /// <param name="VAB_AccountBook_ID">Accounting Schema</param>
        private void UpdateDefaultAccounts(int VAB_AccountBook_ID)
        {
            log.Config("VAB_AccountBook_ID=" + VAB_AccountBook_ID);

            MVABAccountBook aas = new MVABAccountBook(GetCtx(), VAB_AccountBook_ID, null);
            if (aas.GetAcctSchemaElement("AC").GetVAB_Element_ID() != _VAB_Element_ID)
            {
                log.Log(Level.SEVERE, "VAB_Element_ID=" + _VAB_Element_ID + " not in AcctSchema=" + aas);
                return;
            }

            int[] counts = new int[] { 0, 0, 0 };

            String sql = "SELECT i.VAB_Acct_Element_ID, t.TableName, c.ColumnName, i.I_ElementValue_ID "
                + "FROM I_ElementValue i"
                + " INNER JOIN VAF_Column c ON (i.VAF_Column_ID=c.VAF_Column_ID)"
                + " INNER JOIN VAF_TableView t ON (c.VAF_TableView_ID=t.VAF_TableView_ID) "
                + "WHERE i.I_IsImported='Y' AND Processing='-'"
                + " AND i.VAB_Acct_Element_ID IS NOT NULL AND VAB_Element_ID=@param";
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql, Get_TrxName());
                //pstmt.setInt(1, _VAB_Element_ID);
                param[0] = new SqlParameter("@param", _VAB_Element_ID);
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                while (idr.Read())
                {
                    int VAB_Acct_Element_ID = Utility.Util.GetValueOfInt(idr[0]);//  rs.getInt(1);
                    String TableName = Utility.Util.GetValueOfString(idr[1]);//  rs.getString(2);
                    String ColumnName = Utility.Util.GetValueOfString(idr[2]);
                    int I_ElementValue_ID = Utility.Util.GetValueOfInt(idr[3]);
                    //	Update it
                    int u = UpdateDefaultAccount(TableName, ColumnName, VAB_AccountBook_ID, VAB_Acct_Element_ID);
                    counts[u]++;
                    if (u != UPDATE_ERROR)
                    {
                        sql = "UPDATE I_ElementValue SET Processing='N' "
                            + "WHERE I_ElementValue_ID=" + I_ElementValue_ID;
                        int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                        if (no != 1)
                            log.Log(Level.SEVERE, "Updated=" + no);
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
                log.Log(Level.SEVERE, "", e);
            }
            AddLog(0, null, Utility.Util.GetValueOfDecimal(counts[UPDATE_ERROR]), aas.ToString() + ": @Errors@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(counts[UPDATE_YES]), aas.ToString() + ": @Updated@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(counts[UPDATE_SAME]), aas.ToString() + ": OK");

        }	//	createDefaultAccounts


        private static int UPDATE_ERROR = 0;
        private static int UPDATE_YES = 1;
        private static int UPDATE_SAME = 2;

        /// <summary>
        ///	Update Default Account.
        //This is the sql to delete unused accounts - with the import still in the table(!):
        //DELETE VAB_Acct_Element e
        //WHERE NOT EXISTS (SELECT * FROM Actual_Acct_Detail f WHERE f.Account_ID=e.VAB_Acct_Element_ID)
        // AND NOT EXISTS (SELECT * FROM VAB_Acct_ValidParameter vc WHERE vc.Account_ID=e.VAB_Acct_Element_ID)
        // AND NOT EXISTS (SELECT * FROM I_ElementValue i WHERE i.VAB_Acct_Element_ID=e.VAB_Acct_Element_ID);
        /// </summary>
        /// <param name="TableName">trx</param>
        /// <param name="ColumnName">table name</param>
        /// <param name="VAB_AccountBook_ID">column name</param>
        /// <param name="VAB_Acct_Element_ID">account schema</param>
        /// <returns>update * statues</returns>
        private int UpdateDefaultAccount(String TableName, String ColumnName, int VAB_AccountBook_ID, int VAB_Acct_Element_ID)
        {
            log.Fine(TableName + "." + ColumnName + " - " + VAB_Acct_Element_ID);
            int retValue = UPDATE_ERROR;
            StringBuilder sql = new StringBuilder("SELECT x.")
                .Append(ColumnName).Append(",Account_ID FROM ")
                .Append(TableName).Append(" x INNER JOIN VAB_Acct_ValidParameter vc ON (x.")
                .Append(ColumnName).Append("=vc.VAB_Acct_ValidParameter_ID) ")
                .Append("WHERE x.VAB_AccountBook_ID=").Append(VAB_AccountBook_ID);
            IDataReader idr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql.ToString(), Get_TrxName());
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                if (idr.Read())
                {
                    int VAB_Acct_ValidParameter_ID = Utility.Util.GetValueOfInt(idr[0]);// rs.getInt(1);
                    int Account_ID = Utility.Util.GetValueOfInt(idr[1]);// rs.getInt(2);
                    //	The current account value is the same
                    if (Account_ID == VAB_Acct_Element_ID)
                    {
                        retValue = UPDATE_SAME;
                        log.Fine("Account_ID same as new value");
                    }
                    //	We need to update the Account Value
                    else
                    {
                        if (_createNewCombination)
                        {
                            MVABAccount acct = MVABAccount.Get(GetCtx(), VAB_Acct_ValidParameter_ID);
                            acct.SetAccount_ID(VAB_Acct_Element_ID);
                            if (acct.Save())
                            {
                                int newVAB_Acct_ValidParameter_ID = acct.GetVAB_Acct_ValidParameter_ID();
                                if (VAB_Acct_ValidParameter_ID != newVAB_Acct_ValidParameter_ID)
                                {
                                    sql = new StringBuilder("UPDATE ").Append(TableName)
                                        .Append(" SET ").Append(ColumnName).Append("=").Append(newVAB_Acct_ValidParameter_ID)
                                        .Append(" WHERE VAB_AccountBook_ID=").Append(VAB_AccountBook_ID);
                                    int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                                    log.Fine("New #" + no + " - "
                                        + TableName + "." + ColumnName + " - " + VAB_Acct_Element_ID
                                        + " -- " + VAB_Acct_ValidParameter_ID + " -> " + newVAB_Acct_ValidParameter_ID);
                                    if (no == 1)
                                        retValue = UPDATE_YES;
                                }
                            }
                            else
                                log.Log(Level.SEVERE, "Account not saved - " + acct);
                        }
                        else	//	Replace Combination
                        {
                            //	Only Acct Combination directly
                            sql = new StringBuilder("UPDATE VAB_Acct_ValidParameter SET Account_ID=")
                                .Append(VAB_Acct_Element_ID).Append(" WHERE VAB_Acct_ValidParameter_ID=").Append(VAB_Acct_ValidParameter_ID);
                            int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                            log.Fine("Replace #" + no + " - "
                                    + "VAB_Acct_ValidParameter_ID=" + VAB_Acct_ValidParameter_ID + ", New Account_ID=" + VAB_Acct_Element_ID);
                            if (no == 1)
                            {
                                retValue = UPDATE_YES;
                                //	Where Acct was used
                                sql = new StringBuilder("UPDATE VAB_Acct_ValidParameter SET Account_ID=")
                                    .Append(VAB_Acct_Element_ID).Append(" WHERE Account_ID=").Append(Account_ID);
                                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                                log.Fine("ImportAccount.updateDefaultAccount - Replace VC #" + no + " - "
                                        + "Account_ID=" + Account_ID + ", New Account_ID=" + VAB_Acct_Element_ID);
                                sql = new StringBuilder("UPDATE Actual_Acct_Detail SET Account_ID=")
                                    .Append(VAB_Acct_Element_ID).Append(" WHERE Account_ID=").Append(Account_ID);
                                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                                log.Fine("ImportAccount.updateDefaultAccount - Replace Fact #" + no + " - "
                                        + "Account_ID=" + Account_ID + ", New Account_ID=" + VAB_Acct_Element_ID);
                            }
                        }	//	replace combination
                    }	//	need to update
                }	//	for all default accounts
                else
                    log.Log(Level.SEVERE, "Account not found " + sql);
                idr.Close();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql.ToString(), e);
            }

            return retValue;
        }	//	updateDefaultAccount

    }

}

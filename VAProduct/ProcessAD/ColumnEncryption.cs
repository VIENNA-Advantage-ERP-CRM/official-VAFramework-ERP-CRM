using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Model;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using System.Data;


namespace VAdvantage.Process
{
    public class ColumnEncryption : SvrProcess
    {
        /** Enable/Disable Encryption		*/
        private bool p_IsEncrypted = false;
        /** Change Encryption Settings		*/
        private bool p_ChangeSetting = false;
        /** Maximum Length					*/
        private int p_MaxLength = 0;
        /** Test Value						*/
        private String p_TestValue = null;
        /** The Column						*/
        private int p_AD_Column_ID = 0;

        /**
         *  Prepare - e.g., get Parameters.
         */
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
                else if (name.Equals("IsEncrypted"))
                    p_IsEncrypted = "Y".Equals(para[i].GetParameter());
                else if (name.Equals("ChangeSetting"))
                    p_ChangeSetting = "Y".Equals(para[i].GetParameter());
                else if (name.Equals("MaxLength"))
                    p_MaxLength = para[i].GetParameterAsInt();
                else if (name.Equals("TestValue"))
                    p_TestValue = (String)para[i].GetParameter();
                else
                    log.Log(VAdvantage.Logging.Level.SEVERE, "Unknown Parameter: " + name);
            }
            p_AD_Column_ID = GetRecord_ID();
        }	//	prepare

        /**
         * 	Process
         *	@return info
         *	@throws Exception
         */
        protected override String DoIt()// throws Exception
        {
            log.Info("AD_Column_ID=" + p_AD_Column_ID
                + ", IsEncrypted=" + p_IsEncrypted
                + ", ChangeSetting=" + p_ChangeSetting
                + ", MaxLength=" + p_MaxLength);
            MColumn column = new MColumn(GetCtx(), p_AD_Column_ID, Get_Trx());
            if (column.Get_ID() == 0 || column.Get_ID() != p_AD_Column_ID)
                throw new Exception("@NotFound@ @AD_Column_ID@ - " + p_AD_Column_ID);
            //
            String columnName = column.GetColumnName();
            int dt = column.GetAD_Reference_ID();

            //	Can it be enabled?
            if (column.IsKey()
                || column.IsParent()
                || column.IsStandardColumn()
                || column.IsVirtualColumn()
                || column.IsIdentifier()
                || column.IsTranslated()
                || DisplayType.IsLookup(dt)
                || DisplayType.IsLOB(dt)
                || "DocumentNo".Equals(column.GetColumnName(), StringComparison.OrdinalIgnoreCase)
                || "Value".Equals(column.GetColumnName(), StringComparison.OrdinalIgnoreCase)
                || "Name".Equals(column.GetColumnName(), StringComparison.OrdinalIgnoreCase))
            {
                if (column.IsEncrypted())
                {
                    column.SetIsEncrypted(false);
                    column.Save(Get_Trx());
                }
                return columnName + ": cannot be encrypted";
            }

            //	Start
            AddLog(0, null, null, "Encryption Class = " + SecureEngine.GetClassName());
            bool error = false;

            //	Test Value
            if (p_TestValue != null && p_TestValue.Length > 0)
            {
                String encString = SecureEngine.Encrypt(p_TestValue);
                AddLog(0, null, null, "Encrypted Test Value=" + encString);
                String clearString = SecureEngine.Decrypt(encString);
                if (p_TestValue.Equals(clearString))
                    AddLog(0, null, null, "Decrypted=" + clearString
                        + " (same as test value)");
                else
                {
                    AddLog(0, null, null, "Decrypted=" + clearString
                        + " (NOT the same as test value - check algorithm)");
                    error = true;
                }
                int encLength = encString.Length;
                AddLog(0, null, null, "Test Length=" + p_TestValue.Length + " -> " + encLength);
                if (encLength <= column.GetFieldLength())
                    AddLog(0, null, null, "Encrypted Length (" + encLength
                        + ") fits into field (" + column.GetFieldLength() + ")");
                else
                {
                    AddLog(0, null, null, "Encrypted Length (" + encLength
                        + ") does NOT fit into field (" + column.GetFieldLength() + ") - resize field");
                    error = true;
                }
            }

            //	Length Test
            if (p_MaxLength != 0)
            {
                String testClear = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                while (testClear.Length < p_MaxLength)
                    testClear += testClear;
                testClear = testClear.Substring(0, p_MaxLength);
                log.Config("Test=" + testClear + " (" + p_MaxLength + ")");
                //
                String encString = SecureEngine.Encrypt(testClear);
                int encLength = encString.Length;
                AddLog(0, null, null, "Test Max Length=" + testClear.Length + " -> " + encLength);
                if (encLength <= column.GetFieldLength())
                    AddLog(0, null, null, "Encrypted Max Length (" + encLength
                        + ") fits into field (" + column.GetFieldLength() + ")");
                else
                {
                    AddLog(0, null, null, "Encrypted Max Length (" + encLength
                        + ") does NOT fit into field (" + column.GetFieldLength() + ") - resize field");
                    error = true;
                }
            }

            if (p_IsEncrypted != column.IsEncrypted())
            {
                if (error || !p_ChangeSetting)
                    AddLog(0, null, null, "Encryption NOT changed - Encryption=" + column.IsEncrypted());
                else
                {
                    column.SetIsEncrypted(p_IsEncrypted);
                    if (column.Save(Get_Trx()))
                        AddLog(0, null, null, "Encryption CHANGED - Encryption=" + column.IsEncrypted());
                    else
                        AddLog(0, null, null, "Save Error");
                }
            }


            if (p_IsEncrypted == column.IsEncrypted() && !error)      // Done By Karan on 10-nov-2016, to encrypt/decrypt passwords according to settings.
            {
                //object colID = DB.ExecuteScalar("SELECT AD_Column_ID FROM AD_Column WHERE AD_Table_ID =(SELECT AD_Table_ID From AD_Table WHERE TableName='AD_User') AND ColumnName='Password'", null, Get_Trx());



                // if (colID != null && colID != DBNull.Value && Convert.ToInt32(colID) == column.GetAD_Column_ID())
                //{

                string tableName = MTable.GetTableName(GetCtx(), column.GetAD_Table_ID());

                DataSet ds = DB.ExecuteDataset("SELECT " + column.GetColumnName() + "," + tableName
                                                                + "_ID FROM " + tableName, null, Get_Trx());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    if (p_IsEncrypted)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (ds.Tables[0].Rows[i][column.GetColumnName()] != null && ds.Tables[0].Rows[i][column.GetColumnName()] != DBNull.Value
                                && !SecureEngine.IsEncrypted(ds.Tables[0].Rows[i][column.GetColumnName()].ToString()))
                            {
                                //MUser user = new MUser(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i][MTable.GetTableName(GetCtx(), column.GetAD_Table_ID()) + "_ID"]), Get_Trx());
                                //user.SetPassword(SecureEngine.Encrypt(ds.Tables[0].Rows[i][column.GetColumnName()].ToString()));

                                int encLength = SecureEngine.Encrypt(ds.Tables[0].Rows[i][column.GetColumnName()].ToString()).Length;

                                if (encLength <= column.GetFieldLength())
                                {
                                    //PO tab = MTable.GetPO(GetCtx(), tableName,
                                    //    Util.GetValueOfInt(ds.Tables[0].Rows[i][tableName + "_ID"]), Get_Trx());

                                    //tab.Set_Value(column.GetColumnName(), (SecureEngine.Encrypt(ds.Tables[0].Rows[i][column.GetColumnName()].ToString())));
                                    //if (!tab.Save(Get_Trx()))
                                    //{
                                    //    Rollback();
                                    //    return "Encryption=" + false;
                                    //}
                                    string p_NewPassword = SecureEngine.Encrypt(ds.Tables[0].Rows[i][column.GetColumnName()].ToString());
                                    String sql = "UPDATE " + tableName + " SET Updated=SYSDATE, UpdatedBy=" + GetAD_User_ID();
                                    if (!string.IsNullOrEmpty(p_NewPassword))
                                    {
                                        sql += ", " + column.GetColumnName() + "=" + GlobalVariable.TO_STRING(p_NewPassword);
                                    }
                                    sql += " WHERE " + tableName + "_ID=" + Util.GetValueOfInt(ds.Tables[0].Rows[i][tableName + "_ID"]);
                                    int iRes = DB.ExecuteQuery(sql, null, Get_Trx());
                                    if (iRes <= 0)
                                    {
                                        Rollback();
                                        return "Encryption=" + false;
                                    }
                                }
                                else
                                {
                                    Rollback();
                                    return "After Encryption some values may exceed the value of column length. Please exceed column Length.";
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (ds.Tables[0].Rows[i][column.GetColumnName()] != null && ds.Tables[0].Rows[i][column.GetColumnName()] != DBNull.Value
                                && SecureEngine.IsEncrypted(ds.Tables[0].Rows[i][column.GetColumnName()].ToString()))
                            {
                                // MUser user = new MUser(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i][MTable.GetTableName(GetCtx(), column.GetAD_Table_ID())+"_ID"]), Get_Trx());

                                //PO tab = MTable.GetPO(GetCtx(), tableName,
                                //   Util.GetValueOfInt(ds.Tables[0].Rows[i][tableName + "_ID"]), Get_Trx());

                                //tab.Set_Value(column.GetColumnName(), (SecureEngine.Decrypt(ds.Tables[0].Rows[i][column.GetColumnName()].ToString())));
                                //if (!tab.Save(Get_Trx()))
                                //{
                                //    Rollback();
                                //    return "Encryption=" + false;
                                //}

                                string p_NewPassword = SecureEngine.Decrypt(ds.Tables[0].Rows[i][column.GetColumnName()].ToString());
                                String sql = "UPDATE " + tableName + "  SET Updated=SYSDATE, UpdatedBy=" + GetAD_User_ID();
                                if (!string.IsNullOrEmpty(p_NewPassword))
                                {
                                    sql += ", " + column.GetColumnName() + "=" + GlobalVariable.TO_STRING(p_NewPassword);
                                }
                                sql += " WHERE " + tableName + "_ID  =" + Util.GetValueOfInt(ds.Tables[0].Rows[i][tableName + "_ID"]);
                                int iRes = DB.ExecuteQuery(sql, null, Get_Trx());
                                if (iRes <= 0)
                                {
                                    Rollback();
                                    return "Encryption=" + false;
                                }

                            }
                        }
                    }
                }
                //}

            }
            return "Encryption=" + column.IsEncrypted();
        }
    }
}

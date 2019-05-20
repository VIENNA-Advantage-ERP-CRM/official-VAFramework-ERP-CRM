using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViennaAdvantage.Process;
using VAdvantage.Classes;
using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;
//using Microsoft.Office;
using System.Data.OleDb;
//using Microsoft.Office.Interop;
using System.IO;
using VAdvantage.Model;
//using Excel;
using System.ComponentModel;
using System.Windows.Forms;
using VAdvantage.ImpExp;

namespace VAdvantage.Process
{
    class ImpotCOATemp:SvrProcess
    {
        DataTable dt = new DataTable();
        string filename = "";
        int C_Elememt_ID = 0;
        string sql = "";
        // int result = 0;
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                string name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("C_Element_ID"))
                {
                    C_Elememt_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("FileType"))
                {
                    filename = para[i].GetInfo();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        protected override string DoIt()
        {
            string msg = "";
            string extension = filename;
            int ind = filename.LastIndexOf(".");
            extension = filename.Substring(ind, filename.Length - ind);
            int client = Util.GetValueOfInt(GetAD_Client_ID());
            int user = GetAD_User_ID();
            if (extension.ToUpper() == ".XLS" || extension.ToUpper() == ".CSV" )
            {
               
                try
                {
                    /////////////////lakhwinder
                    ExcelReader reader = new ExcelReader(1, 1);
                    System.Data.DataTable dt = reader.ExtractDataTable("D:\\gh.xls", "Sheet1$");    //extract all the records form excel to DataTable

                    /////////////////
                    //excel = AutomationFactory.CreateObject("Excel.Application");
                    //excel.Visible = true;
                    ////dynamic objExcel = AutomationFactory.CreateObject("Excel.Application");

                    //Microsoft.Office.Interop.Excel.Application xlsApp1;
                    //Microsoft.Office.Interop.Excel.Workbook xlsWorkbook1;
                    //Microsoft.Office.Interop.Excel.Worksheets xlsWorksheet1;

                    //Microsoft.Office.Interop.Excel.Application xlsApp = new Microsoft.Office.Interop.Excel.Application();
                    //Microsoft.Office.Interop.Excel.Workbook xlsWrkBook = (Microsoft.Office.Interop.Excel.Workbook)xlsApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);//
                    //Microsoft.Office.Interop.Excel.Worksheet wrkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlsWrkBook.Worksheets.get_Item(1);

                    // Microsoft.Office.Interop.Excel.ApplicationClass xlsApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
                    //  Microsoft.Office.Interop.Excel.WorkbookClass xlsWrkBook = (Microsoft.Office.Interop.Excel.WorkbookClass)xlsApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);//
                    //Microsoft.Office.Interop.Excel.Worksheet wrkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlsWrkBook.Worksheets.get_Item(1);
                    //string name = wrkSheet.Name.ToString();
                    //OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filename + ";Extended Properties=Excel 8.0");
                    //OleDbDataAdapter da = new OleDbDataAdapter("select * from [" + name + "$]", con);
                    //da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        sql = "select ad_tree_id from c_element where c_element_id = " + C_Elememt_ID + " and ad_client_id = " + client;
                        int ad_tree_id = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string key = Util.GetValueOfString(dt.Rows[i]["(Account_Value)"]);
                            if (key != "")
                            {
                                sql = "select c_elementvalue_id from c_elementvalue where value = '" + key + "' and ad_client_id = " + client;
                                int C_ElementValue_ID1 = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                                if (C_ElementValue_ID1 == 0)
                                {
                                    int parent_ID = Util.GetValueOfInt(dt.Rows[i]["(Account_Parent)"]);
                                    sql = "select c_elementvalue_id from c_elementvalue where value = '" + parent_ID + "' and ad_client_id = " + Util.GetValueOfInt(GetAD_Client_ID());
                                    int C_ElementValue_ID_Parent = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                                    MElementValue eleValue = new MElementValue(GetCtx(), 0, null);
                                    int C_ElementValue_ID = DB.GetNextID(GetAD_Client_ID(), "C_ElementValue", null);
                                    string accSign = Util.GetValueOfString(dt.Rows[i]["(Account_Sign)"]);
                                    if (accSign == "")
                                    {
                                        eleValue.SetAccountSign("N");
                                    }
                                    else
                                    {
                                        eleValue.SetAccountSign(accSign);
                                    }
                                    eleValue.SetC_Element_ID(C_Elememt_ID);
                                    eleValue.SetC_ElementValue_ID(C_ElementValue_ID);
                                    eleValue.SetValue(Util.GetValueOfString(dt.Rows[i]["(Account_Value)"]));
                                    eleValue.SetName(Util.GetValueOfString(dt.Rows[i]["(Account_Name)"]));
                                    eleValue.SetDescription(Util.GetValueOfString(dt.Rows[i]["(Account_Description)"]));
                                    eleValue.SetIsActive(true);
                                    // For Summary
                                    if (dt.Rows[i]["(Account_Summary)"].ToString().ToUpper() == "YES")
                                    {
                                        eleValue.SetIsSummary(true);
                                    }
                                    else
                                    {
                                        eleValue.SetIsSummary(false);
                                    }
                                    // For Account Type                   
                                    if (Util.GetValueOfString(dt.Rows[i]["(Account_Type)"]).ToUpper() == "ASSET")
                                    {
                                        eleValue.SetAccountType("A");
                                    }
                                    else if (Util.GetValueOfString(dt.Rows[i]["(Account_Type)"]).ToUpper() == "LIABILITY")
                                    {
                                        eleValue.SetAccountType("L");
                                    }
                                    else if (Util.GetValueOfString(dt.Rows[i]["(Account_Type)"]).ToUpper() == "OWNER'S EQUITY")
                                    {
                                        eleValue.SetAccountType("O");
                                    }
                                    else if (Util.GetValueOfString(dt.Rows[i]["(Account_Type)"]).ToUpper() == "REVENUE")
                                    {
                                        eleValue.SetAccountType("R");
                                    }
                                    else if (Util.GetValueOfString(dt.Rows[i]["(Account_Type)"]).ToUpper() == "EXPENSE")
                                    {
                                        eleValue.SetAccountType("E");
                                    }
                                    else
                                    {
                                        eleValue.SetAccountType("M");
                                    }

                                    if (!eleValue.Save())
                                    {
                                        log.SaveError("NotSaved", "");
                                        return msg;
                                    }
                                    VAdvantage.Model.MTree obj = new VAdvantage.Model.MTree(GetCtx(), ad_tree_id, null);
                                    C_ElementValue_ID = C_ElementValue_ID + 1;
                                    VAdvantage.Model.MTreeNode mNode = new VAdvantage.Model.MTreeNode(obj, C_ElementValue_ID);
                                    mNode.SetParent_ID(C_ElementValue_ID_Parent);
                                    if (!mNode.Save())
                                    {
                                        log.SaveError("NodeNotSaved", "");
                                        return msg;
                                    }
                                }
                            }
                        }
                    }
                    msg = Msg.GetMsg(GetCtx(), "ImportedSuccessfully");
                    return msg;
                }
                catch 
                {
                    msg = Msg.GetMsg(GetCtx(), "ExcelSheetNotInProperFormat");
                    return msg;
                }
            }
            else
            {
                msg = Msg.GetMsg(GetCtx(), "UseDefaultExcelSheet");
                return msg;
            }

        }
    }
}

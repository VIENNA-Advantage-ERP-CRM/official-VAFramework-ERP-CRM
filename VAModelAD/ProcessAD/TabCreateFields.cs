/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : TabCreateFields
 * Purpose        : To create/copy the fields from the current table in the vaf_field table.
 * Class Used     : TabCreateFields inherits SvrProcess class
 * Chronological    Development
 * Raghunandan      30-March-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.SqlExec;
using System.Windows.Forms;

using VAdvantage.ProcessEngine;
using VAdvantage.Logging;
namespace VAdvantage.Process
{
    //Create Field from Table Column.
    //(which do not exist in the Tab yet)   
    public class TabCreateFields : ProcessEngine.SvrProcess
    {
        //Tab NUmber
        private int p_VAF_Tab_ID = 0;

        /// <summary>
        /// function to et parameters
        /// </summary>
        /// <returns>int</returns>
        override protected void Prepare()
        {

            //ProcessInfoParameter[] para = GetParameter();
            //for (int i = 0; i < para.Length; i++)
            //{
            //    String name = para[i].getParameterName();
            //    // if (para[i].getParameter() == null)
            //    {
            //    }
            //    // else
            //    {
            //        // log.log(Level.SEVERE, "Unknown Parameter: " + name);
            //    }
            //}
            p_VAF_Tab_ID = GetRecord_ID();
            
        }
        /// <summary>
        /// Process
        /// throws Exception
        /// </summary>
        /// <returns>info</returns>
        override protected string DoIt()
        {
            //IDbTransaction trx = ExecuteQuery.GerServerTransaction();
            MTab tab = new MTab(GetCtx(), p_VAF_Tab_ID, Get_Trx());
            //MTab tab = new MTab(GetCtx(), p_VAF_Tab_ID, trx.ToString());
            if (p_VAF_Tab_ID == 0 || tab == null || tab.Get_ID() == 0)
            {
                throw new Exception("@NotFound@: @VAF_Tab_ID@ " + p_VAF_Tab_ID);
            }
            //log.info(tab.toString());
            int count = 0;
            string sql = "SELECT * FROM VAF_Column c "
                + "WHERE NOT EXISTS (SELECT * FROM VAF_Field f "
                    + "WHERE c.VAF_Column_ID=f.VAF_Column_ID"
                    + " AND c.VAF_TableView_ID=@VAF_TableView_Id"	//	#1
                    + " AND f.VAF_Tab_ID=@VAF_Tab_Id)"		//	#2
                + " AND VAF_TableView_ID=@VAF_TableView_Id1"			//	#3
                + " AND NOT (Name LIKE 'Created%' OR Name LIKE 'Updated%')"
                + " AND IsActive='Y' "
                + "ORDER BY Name desc";

           
            try
            {

                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@VAF_TableView_Id", tab.GetVAF_TableView_ID());
                param[1] = new SqlParameter("@VAF_Tab_Id", tab.GetVAF_Tab_ID());
                param[2] = new SqlParameter("@VAF_TableView_Id1", tab.GetVAF_TableView_ID());
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param,Get_Trx());
                //DataSet ds1 = ExecuteQuery.ExecuteDataset(sql);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    //MColumn column = new MColumn (getCtx(),dr, Get_Trx());
                    //MColumn column = new MColumn(GetCtx(), dr, null);
                    MColumn column = new MColumn(GetCtx(), dr, Get_Trx());
                    //
                    MField field = new MField(tab);
                    field.SetColumn(column);
                    field.SetMRIsDisplayed("Y");
                    if (column.IsKey())
                    {
                        field.SetIsDisplayed(false);
                        field.SetMRIsDisplayed("N");
                    }
                    if (column.GetColumnName().ToString() == "VAF_Client_ID")
                    {
                        field.SetSeqNo(10);
                        field.SetMRSeqNo(10);
                    }
                    if (column.GetColumnName().ToString()=="VAF_Org_ID")
                    {
                        field.SetIsSameLine(true);
                        field.SetSeqNo(20);
                        field.SetMRSeqNo(20);
                    }
                    //Export_ID Check  [Hide Export Field]
                    if (column.GetColumnName().ToString() == "Export_ID")
                    {
                        field.SetIsDisplayed(false);
                        field.SetMRIsDisplayed("N");
                    }

                    if (field.Save())
                    {
                        AddLog(0, DateTime.MinValue, Decimal.Parse(count.ToString()), column.GetName());
                        //AddLog(0, DateTime.MinValue, null, );
                        count++;
                        
                    }
                }
              
                ds = null;
               
               
                
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
          
            return "@Created@ #" + count;
        }	

    }
}

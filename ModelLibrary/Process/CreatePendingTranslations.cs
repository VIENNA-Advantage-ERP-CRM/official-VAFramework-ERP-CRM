using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Logging;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    class CreatePendingTranslations:SvrProcess
    {
        int tlLanguageID = 0;
        protected override void Prepare()
        {
           // throw new NotImplementedException();
            //ProcessInfoParameter[] para = GetParameter();
            //for (int i = 0; i < para.Length; i++)
            //{
            //    String name = para[i].GetParameterName();
            //    //	log.fine("prepare - " + para[i]);
            //    if (para[i].GetParameter() == null)
            //    {
            //        ;
            //    }
            //    else if (name.Equals("I_TLLanguage_ID"))
            //    {
            //        tlLanguageID = para[i].GetParameterAsInt();
            //    }
            //    else
            //    {
            //        log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            //    }
            //}
        }

        protected override string DoIt()
        {
            tlLanguageID = GetRecord_ID();
            if (tlLanguageID == 0)
            {
                return "NoLanguageFound";
            }
             Trx trx = Trx.Get("TRLF");
            StringBuilder res = new StringBuilder();
            StringBuilder sql = new StringBuilder();

            ////////Translate System Element
            sql.Clear();
            sql.Append("SELECT * FROM I_TLELEMENT_trl WHERE IsContinueTranslation='Y'  AND  I_TLLanguage_ID=" + tlLanguageID);
            DataSet ds = DB.ExecuteDataset(sql.ToString());
            string lang = Util.GetValueOfString(DB.ExecuteScalar("select I_TLLanguage From I_TLLanguage WHERE I_TLLAnguage_ID=" + tlLanguageID));
            System.Data.SqlClient.SqlParameter[] param = null;
            string desc = "";
            string help = "";
            if (ds != null)
            {
                X_I_TLElement_Trl tlEle = null;



                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tlEle = new X_I_TLElement_Trl(GetCtx(), ds.Tables[0].Rows[i], null);

                    sql.Clear();
                    param = new System.Data.SqlClient.SqlParameter[8];
                    param[0] = new System.Data.SqlClient.SqlParameter("@cname", tlEle.GetName());
                    if (string.IsNullOrEmpty(tlEle.GetPrintName()))
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@PrintName", DBNull.Value);
                    }
                    else
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@PrintName", tlEle.GetPrintName());
                    }
                    if (string.IsNullOrEmpty(tlEle.GetDescription()))
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@description", DBNull.Value);
                    }
                    else
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@description", tlEle.GetDescription());
                    }
                    help = tlEle.GetHelp();
                    if (string.IsNullOrEmpty(help))
                    {
                        param[3] = new System.Data.SqlClient.SqlParameter("@help", DBNull.Value);
                    }
                    else
                    {
                        param[3] = new System.Data.SqlClient.SqlParameter("@help", help);
                    }
                    if (string.IsNullOrEmpty(tlEle.GetPO_Name()))
                    {
                        param[4] = new System.Data.SqlClient.SqlParameter("@POName", DBNull.Value);
                    }
                    else
                    {
                        param[4] = new System.Data.SqlClient.SqlParameter("@POName", tlEle.GetPO_Name());
                    }
                    if (string.IsNullOrEmpty(tlEle.GetPO_PrintName()))
                    {
                        param[5] = new System.Data.SqlClient.SqlParameter("@POPrintName", DBNull.Value);
                    }
                    else
                    {
                        param[5] = new System.Data.SqlClient.SqlParameter("@POPrintName", tlEle.GetPO_PrintName());
                    }
                    if (string.IsNullOrEmpty(tlEle.GetPO_Description()))
                    {
                        param[6] = new System.Data.SqlClient.SqlParameter("@POdesc", DBNull.Value);
                    }
                    else
                    {
                        param[6] = new System.Data.SqlClient.SqlParameter("@POdesc", tlEle.GetPO_Description());

                    }
                    if (string.IsNullOrEmpty(tlEle.GetPO_Help()))
                    {
                        param[7] = new System.Data.SqlClient.SqlParameter("@POhelp", DBNull.Value);
                    }
                    else
                    {
                        param[7] = new System.Data.SqlClient.SqlParameter("@POhelp", tlEle.GetPO_Help());
                    }
                    sql.Append(@"Update VAF_COLUMNDIC_TL 
                                                            SET Name=@cname,
                                                            IsTranslated='Y',
                                                            PrintName=@PrintName,
                                                            Description=@description,
                                                            Help=@help,
                                                            PO_Name=@POName,
                                                            PO_PrintName=@POPrintName,
                                                            PO_Description=@POdesc,
                                                            PO_Help=@POhelp
                                                         WHERE VAF_COLUMNDIC_ID=" + tlEle.GetVAF_ColumnDic_ID() + " AND VAF_LANGUAGE='" + lang + "'");
                    if (DB.ExecuteQuery(sql.ToString(), param, trx) == -1)
                    {
                        res.Append(tlEle.GetVAF_ColumnDic_ID() + " ElementNotTranslated.");
                        break;
                    }
                    tlEle.SetIsTranslated(true);
                    tlEle.Save(trx);
                    //}

                }
            }
            sql.Clear();
            sql.Append("SELECT * FROM I_TLField_trl WHERE IsContinueTranslation='Y'  AND  I_TLLanguage_ID=" + tlLanguageID);
            ds = DB.ExecuteDataset(sql.ToString());
          

            if (ds != null)
            {
                X_I_TLField_Trl tlField = null;
                
                char isCentrallyMaintained='N';
                for (int i = 0; i < ds.Tables[0].Rows.Count;i++ )
                {
                    tlField = new X_I_TLField_Trl(GetCtx(), ds.Tables[0].Rows[i], null);
                    isCentrallyMaintained = 'N';
                    if (tlField.IsTranslateSystemElement())
                    {
                        isCentrallyMaintained = 'Y';
                        //Translate System Element trl
                        sql.Clear();
                        param = new System.Data.SqlClient.SqlParameter[2];
                        param[0] = new System.Data.SqlClient.SqlParameter("@cname", tlField.GetName());
                        param[1] = new System.Data.SqlClient.SqlParameter("@PrintName", tlField.GetName());
                        sql.Append(@"Update VAF_COLUMNDIC_TL 
                                            SET Name=@cname,
                                                PRINTNAME=@PrintName,
                                                IsTranslated='Y'
                                            WHERE VAF_COLUMNDIC_ID=" + tlField.GetVAF_ColumnDic_ID()+" AND VAF_LANGUAGE='"+lang+"'");
                       if(DB.ExecuteQuery(sql.ToString(),param,trx)==-1)
                       {
                           res.Append(tlField.GetVAF_ColumnDic_ID() + " SystemElementNotTranslated.");
                           break;
                       }
                    }
                    //else
                    //{
                        // Tarnslate Field Trl
                    sql.Clear();
                    param = new System.Data.SqlClient.SqlParameter[3];
                    param[0] = new System.Data.SqlClient.SqlParameter("@cname", tlField.GetName());
                    desc=tlField.GetDescription();
                    if (string.IsNullOrEmpty(desc))
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", DBNull.Value);
                    }
                    else
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", desc);
                    }
                    help = tlField.GetHelp();
                    if (string.IsNullOrEmpty(help))
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", DBNull.Value);
                    }
                    else
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", help);
                    }
                    sql.Append(@"Update VAF_Field_TL 
                                        SET Name=@cname,
                                            IsTranslated='Y',
                                            Description=@description,
                                            help=@help
                                        WHERE VAF_Field_ID=" + tlField.GetVAF_Field_ID() + " AND VAF_LANGUAGE='" + lang + "'");
                    if (DB.ExecuteQuery(sql.ToString(),param,trx) == -1)
                    {
                        res.Append(tlField.GetVAF_Field_ID() + " FieldNotTranslated.");
                        break;
                    }
                    sql.Clear();
                    sql.Append("UPDATE VAF_Field SET IsCentrallyMaintained='" + isCentrallyMaintained + "' WHERE VAF_Field_ID=" + tlField.GetVAF_Field_ID());
                    DB.ExecuteQuery(sql.ToString(),null,trx);
                    tlField.SetIsTranslated(true);
                    tlField.Save(trx);
                    //}
                   
                }
            }
           
            /////Translate Message
            sql.Clear();
            sql.Append("SELECT * FROM I_TLMessage_trl WHERE IsContinueTranslation='Y'  AND  I_TLLanguage_ID=" + tlLanguageID);
            ds = DB.ExecuteDataset(sql.ToString());
            if (ds != null)
            {
                X_I_TLMessage_Trl tlMsg = null;


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tlMsg = new X_I_TLMessage_Trl(GetCtx(), ds.Tables[0].Rows[i], null);
                    param = new System.Data.SqlClient.SqlParameter[2];
                    param[0] = new System.Data.SqlClient.SqlParameter("@Msg", tlMsg.GetMsgText());
                    if (string.IsNullOrEmpty(tlMsg.GetMsgTip()))
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@MsgTip", DBNull.Value);
                    }
                    else
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@MsgTip", tlMsg.GetMsgTip());
                    }
                    
                    sql.Clear();
                    sql.Append(@"Update VAF_Msg_Lable_TL 
                                        SET MsgText=@Msg,
                                            IsTranslated='Y',
                                            MsgTip=@MsgTip
                                        WHERE VAF_Msg_Lable_ID=" + tlMsg.GetVAF_Msg_Lable_ID() + " AND VAF_LANGUAGE='" + lang + "'");
                    if (DB.ExecuteQuery(sql.ToString(), param, trx) == -1)
                    {
                        res.Append(tlMsg.GetVAF_Msg_Lable_ID() + " MessageNotTranslated.");
                        break;
                    }
                    tlMsg.SetIsTranslated(true);
                    tlMsg.Save(trx);
                    //}

                }
            }
            ///Translate WIndow
            sql.Clear();
            sql.Append("SELECT * FROM I_TLWindow_trl WHERE IsContinueTranslation='Y'  AND  I_TLLanguage_ID=" + tlLanguageID);
            ds = DB.ExecuteDataset(sql.ToString());
            if (ds != null)
            {
                X_I_TLWindow_Trl tlWin = null;


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tlWin = new X_I_TLWindow_Trl(GetCtx(), ds.Tables[0].Rows[i], null);
                    param = new System.Data.SqlClient.SqlParameter[3];
                    param[0] = new System.Data.SqlClient.SqlParameter("@cname", tlWin.GetName());
                    desc = tlWin.GetDescription();
                    if (string.IsNullOrEmpty(desc))
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", DBNull.Value);
                    }
                    else
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", desc);
                    }
                    help = tlWin.GetHelp();
                    if (string.IsNullOrEmpty(help))
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", DBNull.Value);
                    }
                    else
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", help);
                    }
                    sql.Clear();
                    sql.Append(@"Update VAF_Screen_TL 
                                        SET Name=@cname,
                                            IsTranslated='Y',
                                            Description=@description,
                                            Help=@help
                                        WHERE VAF_Screen_ID=" + tlWin.GetVAF_Screen_ID() + " AND VAF_LANGUAGE='" + lang + "'");
                    if (DB.ExecuteQuery(sql.ToString(), param, trx) == -1)
                    {
                        res.Append(tlWin.GetVAF_Screen_ID() + " WindowNotTranslated.");
                        break;
                    }
                    tlWin.SetIsTranslated(true);
                    tlWin.Save(trx);
                    //}

                }
            }

            ///Translate tab
            sql.Clear();
            sql.Append("SELECT * FROM I_TLtab_trl WHERE IsContinueTranslation='Y'  AND  I_TLLanguage_ID=" + tlLanguageID);
            ds = DB.ExecuteDataset(sql.ToString());
            if (ds != null)
            {
                X_I_TLTab_Trl tlTab = null;


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tlTab = new X_I_TLTab_Trl(GetCtx(), ds.Tables[0].Rows[i], null);
                    param = new System.Data.SqlClient.SqlParameter[4];
                    param[0] = new System.Data.SqlClient.SqlParameter("@cname", tlTab.GetName());
                    desc = tlTab.GetDescription();
                    if (string.IsNullOrEmpty(desc))
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", DBNull.Value);
                    }
                    else
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", desc);
                    }
                    help = tlTab.GetHelp();
                    if (string.IsNullOrEmpty(help))
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", DBNull.Value);
                    }
                    else
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", help);
                    }
                    if (string.IsNullOrEmpty(tlTab.GetCommitWarning()))
                    {
                        param[3] = new System.Data.SqlClient.SqlParameter("@cm", DBNull.Value);
                    }
                    else
                    {
                        param[3] = new System.Data.SqlClient.SqlParameter("@cm", tlTab.GetCommitWarning());
                    }
                    sql.Clear();
                    sql.Append(@"Update VAF_Tab_TL 
                                        SET Name=@cname,
                                            IsTranslated='Y',
                                            Description=@description,
                                            Help=@help,
                                            CommitWarning=@cm
                                        WHERE VAF_Tab_ID=" + tlTab.GetVAF_Tab_ID() + " AND VAF_LANGUAGE='" + lang + "'");
                    if (DB.ExecuteQuery(sql.ToString(), param, trx) == -1)
                    {
                        res.Append(tlTab.GetVAF_Tab_ID() + " TabNotTranslated.");
                        break;
                    }
                    tlTab.SetIsTranslated(true);
                    tlTab.Save(trx);
                    //}

                }
            }


            ///Translate FieldGroup
            sql.Clear();
            sql.Append("SELECT * FROM I_TLFieldGroup_trl WHERE IsContinueTranslation='Y'  AND  I_TLLanguage_ID=" + tlLanguageID);
            ds = DB.ExecuteDataset(sql.ToString());
            if (ds != null)
            {
                X_I_TLFieldGroup_Trl tlfg = null;


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tlfg = new X_I_TLFieldGroup_Trl(GetCtx(), ds.Tables[0].Rows[i], null);
                    param = new System.Data.SqlClient.SqlParameter[1];
                    param[0] = new System.Data.SqlClient.SqlParameter("@cname", tlfg.GetName());                   
                    sql.Clear();
                    sql.Append(@"Update VAF_FieldSection_Tl 
                                        SET Name=@cname,
                                            IsTranslated='Y'                                                                                     
                                        WHERE VAF_FieldSection_ID=" + tlfg.GetVAF_FieldSection_ID() + " AND VAF_LANGUAGE='" + lang + "'");
                    if (DB.ExecuteQuery(sql.ToString(), param, trx) == -1)
                    {
                        res.Append(tlfg.GetVAF_FieldSection_ID() + " FieldGroupNotTranslated.");
                        break;
                    }
                    tlfg.SetIsTranslated(true);
                    tlfg.Save(trx);
                    //}

                }
            }


            ///Translate process
            sql.Clear();
            sql.Append("SELECT * FROM I_TLProcess_trl WHERE IsContinueTranslation='Y'  AND  I_TLLanguage_ID=" + tlLanguageID);
            ds = DB.ExecuteDataset(sql.ToString());
            if (ds != null)
            {
                X_I_TLProcess_Trl tlps = null;


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tlps = new X_I_TLProcess_Trl(GetCtx(), ds.Tables[0].Rows[i], null);
                    param = new System.Data.SqlClient.SqlParameter[3];
                    param[0] = new System.Data.SqlClient.SqlParameter("@cname", tlps.GetName());
                    desc = tlps.GetDescription();
                    if (string.IsNullOrEmpty(desc))
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", DBNull.Value);
                    }
                    else
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", desc);
                    }
                    help = tlps.GetHelp();
                    if (string.IsNullOrEmpty(help))
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", DBNull.Value);
                    }
                    else
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", help);
                    }
                    sql.Clear();
                    sql.Append(@"Update VAF_Job_TL 
                                        SET Name=@cname,
                                            IsTranslated='Y',
                                            Description=@description,
                                            Help=@help                                           
                                        WHERE VAF_Job_ID=" + tlps.GetVAF_Job_ID() + " AND VAF_LANGUAGE='" + lang + "'");
                    if (DB.ExecuteQuery(sql.ToString(), param, trx) == -1)
                    {
                        res.Append(tlps.GetVAF_Job_ID() + " ProcessNotTranslated.");
                        break;
                    }
                    tlps.SetIsTranslated(true);
                    tlps.Save(trx);
                    //}

                }
            }



            ///Translate Form
            sql.Clear();
            sql.Append("SELECT * FROM I_TLForm_trl WHERE IsContinueTranslation='Y'  AND  I_TLLanguage_ID=" + tlLanguageID);
            ds = DB.ExecuteDataset(sql.ToString());
            if (ds != null)
            {
                X_I_TLForm_Trl tlps = null;


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tlps = new X_I_TLForm_Trl(GetCtx(), ds.Tables[0].Rows[i], null);
                    param = new System.Data.SqlClient.SqlParameter[3];
                    param[0] = new System.Data.SqlClient.SqlParameter("@cname", tlps.GetName());
                    desc = tlps.GetDescription();
                    if (string.IsNullOrEmpty(desc))
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", DBNull.Value);
                    }
                    else
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", desc);
                    }
                    help = tlps.GetHelp();
                    if (string.IsNullOrEmpty(help))
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", DBNull.Value);
                    }
                    else
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", help);
                    }
                    sql.Clear();
                    sql.Append(@"Update VAF_Page_TL 
                                        SET Name=@cname,
                                            IsTranslated='Y',
                                            Description=@description,
                                            Help=@help                                           
                                        WHERE VAF_Page_ID=" + tlps.GetVAF_Page_ID() + " AND VAF_LANGUAGE='" + lang + "'");
                    if (DB.ExecuteQuery(sql.ToString(), param, trx) == -1)
                    {
                        res.Append(tlps.GetVAF_Page_ID() + " FormNotTranslated.");
                        break;
                    }
                    tlps.SetIsTranslated(true);
                    tlps.Save(trx);
                    //}

                }
            }

            ///Translate task
            sql.Clear();
            sql.Append("SELECT * FROM I_TLTask_trl WHERE IsContinueTranslation='Y'  AND  I_TLLanguage_ID=" + tlLanguageID);
            ds = DB.ExecuteDataset(sql.ToString());
            if (ds != null)
            {
                X_I_TLTask_Trl tlps = null;


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tlps = new X_I_TLTask_Trl(GetCtx(), ds.Tables[0].Rows[i], null);
                    param = new System.Data.SqlClient.SqlParameter[3];
                    param[0] = new System.Data.SqlClient.SqlParameter("@cname", tlps.GetName());
                    desc = tlps.GetDescription();
                    if (string.IsNullOrEmpty(desc))
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", DBNull.Value);
                    }
                    else
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", desc);
                    }
                    help = tlps.GetHelp();
                    if (string.IsNullOrEmpty(help))
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", DBNull.Value);
                    }
                    else
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", help);
                    }
                    sql.Clear();
                    sql.Append(@"Update AD_Task_TRL 
                                        SET Name=@cname,
                                            IsTranslated='Y',
                                            Description=@description,
                                            Help=@help                                          
                                        WHERE AD_Task_ID=" + tlps.GetAD_Task_ID() + " AND VAF_LANGUAGE='" + lang + "'");
                    if (DB.ExecuteQuery(sql.ToString(), param, trx) == -1)
                    {
                        res.Append(tlps.GetAD_Task_ID() + " ProcessNotTranslated.");
                        break;
                    }
                    tlps.SetIsTranslated(true);
                    tlps.Save(trx);
                    //}

                }
            }

            ///Translate Workflow
            sql.Clear();
            sql.Append("SELECT * FROM I_TLWorkflow_trl WHERE IsContinueTranslation='Y'  AND  I_TLLanguage_ID=" + tlLanguageID);
            ds = DB.ExecuteDataset(sql.ToString());
            if (ds != null)
            {
                X_I_TLWorkflow_Trl tlps = null;


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tlps = new X_I_TLWorkflow_Trl(GetCtx(), ds.Tables[0].Rows[i], null);
                    param = new System.Data.SqlClient.SqlParameter[3];
                    param[0] = new System.Data.SqlClient.SqlParameter("@cname", tlps.GetName());
                    desc = tlps.GetDescription();
                    if (string.IsNullOrEmpty(desc))
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", DBNull.Value);
                    }
                    else
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", desc);
                    }
                    help = tlps.GetHelp();
                    if (string.IsNullOrEmpty(help))
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", DBNull.Value);
                    }
                    else
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", help);
                    }
                    sql.Clear();
                    sql.Append(@"Update VAF_Workflow_TL 
                                        SET Name=@cname,
                                            IsTranslated='Y',
                                            Description=@description,
                                            Help=@help                                           
                                        WHERE AD_Workflow_ID=" + tlps.GetAD_Workflow_ID() + " AND VAF_LANGUAGE='" + lang + "'");
                    if (DB.ExecuteQuery(sql.ToString(), param, trx) == -1)
                    {
                        res.Append(tlps.GetAD_Workflow_ID() + " WorkflowNotTranslated.");
                        break;
                    }
                    tlps.SetIsTranslated(true);
                    tlps.Save(trx);
                    //}

                }
            }
            ///Translate WF_Node
            sql.Clear();
            sql.Append("SELECT * FROM I_TLWF_Node_trl WHERE IsContinueTranslation='Y'  AND  I_TLLanguage_ID=" + tlLanguageID);
            ds = DB.ExecuteDataset(sql.ToString());
            if (ds != null)
            {
                X_I_TLWF_Node_Trl tlps = null;


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tlps = new X_I_TLWF_Node_Trl(GetCtx(), ds.Tables[0].Rows[i], null);
                    param = new System.Data.SqlClient.SqlParameter[3];
                    param[0] = new System.Data.SqlClient.SqlParameter("@cname", tlps.GetName());
                    desc = tlps.GetDescription();
                    if (string.IsNullOrEmpty(desc))
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", DBNull.Value);
                    }
                    else
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", desc);
                    }
                    help = tlps.GetHelp();
                    if (string.IsNullOrEmpty(help))
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", DBNull.Value);
                    }
                    else
                    {
                        param[2] = new System.Data.SqlClient.SqlParameter("@help", help);
                    }
                    sql.Clear();
                    sql.Append(@"Update VAF_WFlow_Node_TL 
                                        SET Name=@cname,
                                            IsTranslated='Y',
                                            Description=@description,
                                            Help=@help                                         
                                        WHERE VAF_WFlow_Node_ID=" + tlps.GetVAF_WFlow_Node_ID() + " AND VAF_LANGUAGE='" + lang + "'");
                    if (DB.ExecuteQuery(sql.ToString(), param, trx) == -1)
                    {
                        res.Append(tlps.GetVAF_WFlow_Node_ID() + " WF_NodeflowNotTranslated.");
                        break;
                    }
                    tlps.SetIsTranslated(true);
                    tlps.Save(trx);
                    //}

                }
            }

            ///Translate Menu
            sql.Clear();
            sql.Append("SELECT * FROM I_TLMenu_trl WHERE IsContinueTranslation='Y'  AND  I_TLLanguage_ID=" + tlLanguageID);
            ds = DB.ExecuteDataset(sql.ToString());
            if (ds != null)
            {
                X_I_TLMenu_Trl tlps = null;


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tlps = new X_I_TLMenu_Trl(GetCtx(), ds.Tables[0].Rows[i], null);
                    param = new System.Data.SqlClient.SqlParameter[2];
                    param[0] = new System.Data.SqlClient.SqlParameter("@cname", tlps.GetName());
                    desc = tlps.GetDescription();
                    if (string.IsNullOrEmpty(desc))
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", DBNull.Value);
                    }
                    else
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", desc);
                    }
                   
                 
                    sql.Clear();
                    sql.Append(@"Update VAF_MenuConfig_TL 
                                        SET Name=@cname,
                                            IsTranslated='Y',   
                                            Description=@description                                                                                  
                                        WHERE VAF_MenuConfig_ID=" + tlps.GetVAF_MenuConfig_ID() + " AND VAF_LANGUAGE='" + lang + "'");
                    if (DB.ExecuteQuery(sql.ToString(), param, trx) == -1)
                    {
                        res.Append(tlps.GetVAF_MenuConfig_ID() + " MenuNotTranslated.");
                        break;
                    }
                    tlps.SetIsTranslated(true);
                    tlps.Save(trx);
                    //}

                }
            }
            ///Translate Ref_List
            sql.Clear();
            sql.Append("SELECT * FROM I_TLRef_List_trl WHERE IsContinueTranslation='Y'  AND  I_TLLanguage_ID=" + tlLanguageID);
            ds = DB.ExecuteDataset(sql.ToString());
            if (ds != null)
            {
                X_I_TLRef_List_Trl tlps = null;


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tlps = new X_I_TLRef_List_Trl(GetCtx(), ds.Tables[0].Rows[i], null);
                    param = new System.Data.SqlClient.SqlParameter[2];
                    param[0] = new System.Data.SqlClient.SqlParameter("@cname", tlps.GetName());
                    desc = tlps.GetDescription();
                    if (string.IsNullOrEmpty(desc))
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", DBNull.Value);
                    }
                    else
                    {
                        param[1] = new System.Data.SqlClient.SqlParameter("@description", desc);
                    }
                   
                    sql.Clear();
                    sql.Append(@"Update VAF_CtrlRef_TL 
                                        SET Name=@cname,
                                            IsTranslated='Y',
                                            Description=@description                                                                                    
                                        WHERE VAF_CtrlRef_List_ID=" + tlps.GetVAF_CtrlRef_List_ID() + " AND VAF_LANGUAGE='" + lang + "'");
                    if (DB.ExecuteQuery(sql.ToString(), param, trx) == -1)
                    {
                        res.Append(tlps.GetVAF_CtrlRef_List_ID() + " Ref_ListNotTranslated.");
                        break;
                    }
                    tlps.SetIsTranslated(true);
                    tlps.Save(trx);
                    //}

                }
            }

            if (res.Length == 0)
            {

                res.Append("DONE");
                trx.Commit();
            }
            else
            {
                trx.Rollback();
            }
            trx.Close();
            return res.ToString();
            //throw new NotImplementedException();
        }
    }
}

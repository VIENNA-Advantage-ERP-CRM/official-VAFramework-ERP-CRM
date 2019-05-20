using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Process
{
    class CopyTranslationEntries:SvrProcess
    {
        string AD_Language = "";
        string Mode = "";
        protected override void Prepare()
        {
            //throw new NotImplementedException();
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                //	log.fine("prepare - " + para[i]);
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("AD_Language"))
                {
                    AD_Language = para[i].GetParameter().ToString();
                }
                else if (name.Equals("Mode"))
                {
                    Mode = para[i].GetParameter().ToString();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }
        StringBuilder res = new StringBuilder();
      
        DataSet dsTab = null;
        DataSet dsCol = null;
        StringBuilder sql = new StringBuilder();
        X_I_TLLanguage lang = null;
        X_I_TLElement_Trl element = null;
        X_I_TLMessage_Trl msg = null;
        X_I_TLWindow_Trl win = null;
        X_I_TLTab_Trl tab = null;
        X_I_TLField_Trl field = null;
        X_I_TLFieldGroup_Trl fgroup = null;
        X_I_TLProcess_Trl process = null;
        X_I_TLForm_Trl form = null;
        X_I_TLTask_Trl task = null;
        X_I_TLWorkflow_Trl wf = null;
        X_I_TLWF_Node_Trl wfNode = null;
        X_I_TLMenu_Trl menu = null;
        X_I_TLRef_List_Trl reflst = null;
        protected override string DoIt()
        {
            //throw new NotImplementedException();

            //Trx trx = Trx.Get(Trx.CreateTrxName("TRL"), true);



            
            try
            {
               
                sql.Append("select  ad_language, countrycode, name, languageiso,isactive from ad_language where AD_CLIENT_ID=0 and IsBaseLanguage='N' AND IsSystemLanguage='Y' AND AD_LANGUAGE='"+AD_Language+"'");
                DataSet ds = null;
                ds = DB.ExecuteDataset(sql.ToString());
                dsTab = null;
                dsCol = null;
             
                if (ds != null && ds.Tables[0].Rows.Count>0)
                {


                        int tlLanguageID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT I_TLLanguage_ID FROM I_TLLanguage WHERE I_TLLanguage='" + AD_Language + "'"));
                        if (Mode.Equals("D"))//Delete Translations
                        {
                            return DeleteTranslations(tlLanguageID);
                        }
                        lang = new X_I_TLLanguage(Env.GetCtx(), tlLanguageID, null);
                        if (ds.Tables[0].Rows[0]["ad_language"] != null && ds.Tables[0].Rows[0]["ad_language"] != DBNull.Value)
                        {
                            lang.SetI_TLLanguage(ds.Tables[0].Rows[0]["ad_language"].ToString());
                        }
                        if (ds.Tables[0].Rows[0]["countrycode"] != null && ds.Tables[0].Rows[0]["countrycode"] != DBNull.Value)
                        {
                            lang.SetCountryCode(ds.Tables[0].Rows[0]["countrycode"].ToString());
                        }
                        if (ds.Tables[0].Rows[0]["name"] != null && ds.Tables[0].Rows[0]["name"] != DBNull.Value)
                        {
                            lang.SetName(ds.Tables[0].Rows[0]["name"].ToString());
                        }
                        if (ds.Tables[0].Rows[0]["languageiso"] != null && ds.Tables[0].Rows[0]["languageiso"] != DBNull.Value)
                        {
                            lang.SetLanguageISO(ds.Tables[0].Rows[0]["languageiso"].ToString());
                        }
                        lang.SetAD_Client_ID(0);
                        lang.SetAD_Org_ID(0);
                        if (ds.Tables[0].Rows[0]["isactive"] != null && ds.Tables[0].Rows[0]["isactive"] != DBNull.Value)
                        {
                            if (ds.Tables[0].Rows[0]["isactive"].ToString().Equals("Y"))
                            {
                                lang.SetIsActive(true);
                            }
                            else
                            {
                                lang.SetIsActive(false);
                            }
                        }
                        else
                        {
                            lang.SetIsActive(false);
                        }
                        if (lang.Save())
                        {


                            if (Mode.Equals("A"))//Add Missing translations
                            {
                               return AddMissingTranslations(lang.GetI_TLLanguage_ID(), ds.Tables[0].Rows[0]["ad_language"].ToString());
                            }
                            else if (Mode.Equals("D"))//Delete Translations
                            {
                               return DeleteTranslations(lang.GetI_TLLanguage_ID());
                            }
                            else if (Mode.Equals("R"))//ReCreate
                            {
                                return ReCreateTranslations(lang.GetI_TLLanguage_ID(), ds.Tables[0].Rows[0]["ad_language"].ToString());
                            }

                        }
                        else
                        {
                            return "LanguageNotSaved";
                        }

                    }
                
                else
                {
                    return "NO DATA FOUND";
                }

            }
            catch (Exception ex)
            {
              
                return  ex.Message;
            }
                   
            
            return "Done";

        }

        string ReCreateTranslations( int I_TLLanguage_ID,string AD_Language)
        {
            /////////delete Old Data///
            DB.ExecuteQuery("DELETE I_TLElement_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLMessage_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLWindow_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLTab_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLField_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLFieldGroup_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLProcess_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLForm_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLTask_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLWorkflow_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLWF_Node_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLMenu_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLRef_List_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);



            //////////Element TRL
            sql.Clear();
            sql.Append(@" SELECT ad_element_id,
                                              istranslated       ,
                                              isactive           ,
                                              name               ,
                                              printname          ,
                                              description        ,
                                              help               ,
                                              po_name            ,
                                              po_printname       ,
                                              po_description     ,
                                              po_help
                                               FROM ad_element_trl 
                                        where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    element = new X_I_TLElement_Trl(GetCtx(), 0, null);
                    element.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    element.SetAD_Client_ID(0);
                    element.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_element_id"] != null && dsTab.Tables[0].Rows[j]["ad_element_id"] != DBNull.Value)
                    {
                        element.SetAD_Element_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_element_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            element.SetIsTranslated(true);
                        }
                        else
                        {
                            element.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        element.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            element.SetIsActive(true);
                        }
                        else
                        {
                            element.SetIsActive(false);
                        }
                    }
                    else
                    {
                        element.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        element.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["printname"] != null && dsTab.Tables[0].Rows[j]["printname"] != DBNull.Value)
                    {
                        element.SetPrintName(dsTab.Tables[0].Rows[j]["printname"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        element.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        element.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["po_name"] != null && dsTab.Tables[0].Rows[j]["po_name"] != DBNull.Value)
                    {
                        element.SetPO_Name(dsTab.Tables[0].Rows[j]["po_name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["po_printname"] != null && dsTab.Tables[0].Rows[j]["po_printname"] != DBNull.Value)
                    {
                        element.SetPO_PrintName(dsTab.Tables[0].Rows[j]["po_printname"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["po_description"] != null && dsTab.Tables[0].Rows[j]["po_description"] != DBNull.Value)
                    {
                        element.SetPO_Description(dsTab.Tables[0].Rows[j]["po_description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["po_help"] != null && dsTab.Tables[0].Rows[j]["po_help"] != DBNull.Value)
                    {
                        element.SetPO_Help(dsTab.Tables[0].Rows[j]["po_help"].ToString());
                    }
                    if (!element.Save())
                    {
                        res.Append(element.GetName() + " NOT SAVED");
                    }


                }
            }
            /////////Message Trl
            sql.Clear();
            sql.Append(@"SELECT ad_Message_id,
                                                      istranslated       ,
                                                      isactive           ,
                                                      msgtext            ,
                                                      msgtip
                                                       FROM ad_Message_trl
                                              where ad_language='"+ AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    msg = new X_I_TLMessage_Trl(GetCtx(), 0, null);
                    msg.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    msg.SetAD_Client_ID(0);
                    msg.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_Message_id"] != null && dsTab.Tables[0].Rows[j]["ad_Message_id"] != DBNull.Value)
                    {
                        msg.SetAD_Message_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_Message_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            msg.SetIsTranslated(true);
                        }
                        else
                        {
                            msg.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        msg.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            msg.SetIsActive(true);
                        }
                        else
                        {
                            msg.SetIsActive(false);
                        }
                    }
                    else
                    {
                        msg.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["msgtext"] != null && dsTab.Tables[0].Rows[j]["msgtext"] != DBNull.Value)
                    {
                        msg.SetMsgText(dsTab.Tables[0].Rows[j]["msgtext"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["msgtip"] != null && dsTab.Tables[0].Rows[j]["msgtip"] != DBNull.Value)
                    {
                        msg.SetMsgTip(dsTab.Tables[0].Rows[j]["msgtip"].ToString());
                    }
                    if (!msg.Save())
                    {
                        res.Append(msg.GetMsgText() + " NOT SAVED");
                    }

                }
            }

           
            /////////Window Trl
            sql.Clear();
            sql.Append(@"SELECT ad_window_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help
                                               FROM ad_window_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    win = new X_I_TLWindow_Trl(GetCtx(), 0, null);
                    win.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    win.SetAD_Client_ID(0);
                    win.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_window_id"] != null && dsTab.Tables[0].Rows[j]["ad_window_id"] != DBNull.Value)
                    {
                        win.SetAD_Window_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_window_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            win.SetIsTranslated(true);
                        }
                        else
                        {
                            win.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        win.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            win.SetIsActive(true);
                        }
                        else
                        {
                            win.SetIsActive(false);
                        }
                    }
                    else
                    {
                        win.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        win.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        win.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        win.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (!win.Save(null))
                    {
                        res.Append(win.GetName() + " NOT SAVED");
                    }
                }
            }

            /////////Tab Trl
            sql.Clear();
            sql.Append(@" SELECT ad_tab_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help, 
                                              commitwarning
                                               FROM ad_tab_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    tab = new X_I_TLTab_Trl(GetCtx(), 0, null);
                    tab.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    tab.SetAD_Client_ID(0);
                    tab.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_tab_id"] != null && dsTab.Tables[0].Rows[j]["ad_tab_id"] != DBNull.Value)
                    {
                        tab.SetAD_Tab_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_tab_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            tab.SetIsTranslated(true);
                        }
                        else
                        {
                            tab.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        tab.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            tab.SetIsActive(true);
                        }
                        else
                        {
                            tab.SetIsActive(false);
                        }
                    }
                    else
                    {
                        tab.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        tab.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        tab.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        tab.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["commitwarning"] != null && dsTab.Tables[0].Rows[j]["commitwarning"] != DBNull.Value)
                    {
                        tab.SetCommitWarning(dsTab.Tables[0].Rows[j]["commitwarning"].ToString());
                    }
                    if (!tab.Save())
                    {
                        res.Append(tab.GetName() + " NOT SAVED");
                    }
                }
            }

            /////////Field Trl
            sql.Clear();
            sql.Append(@" SELECT ad_Field_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help
                                               FROM ad_Field_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    field = new X_I_TLField_Trl(GetCtx(), 0, null);
                    field.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    field.SetAD_Client_ID(0);
                    field.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_Field_id"] != null && dsTab.Tables[0].Rows[j]["ad_Field_id"] != DBNull.Value)
                    {
                        field.SetAD_Field_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_Field_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            field.SetIsTranslated(true);
                        }
                        else
                        {
                            field.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        field.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            field.SetIsActive(true);
                        }
                        else
                        {
                            field.SetIsActive(false);
                        }
                    }
                    else
                    {
                        field.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        field.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        field.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        field.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    sql.Clear();
                    sql.Append(@" SELECT 
                                                 win.ad_window_id,
                                                 col.ad_element_id, 
                                                 tab.ad_tab_id
                                                   FROM ad_field fld
                                                INNER JOIN ad_tab tab
                                                     ON (tab.ad_tab_id= fld.ad_tab_id)
                                                INNER JOIN ad_window win
                                                     ON (tab.ad_window_id= win.ad_window_id)
                                                     inner join ad_column col on(col.ad_column_id= fld.ad_column_id)
                                                     where fld.ad_field_ID=" + dsTab.Tables[0].Rows[j]["ad_Field_id"] + "");
                    dsCol = DB.ExecuteDataset(sql.ToString());
                    if (dsCol != null && dsCol.Tables[0].Rows.Count > 0)
                    {
                        if (dsCol.Tables[0].Rows[0]["ad_window_id"] != null && dsCol.Tables[0].Rows[0]["ad_window_id"] != DBNull.Value)
                        {
                            field.SetAD_Window_ID(Util.GetValueOfInt(dsCol.Tables[0].Rows[0]["ad_window_id"]));
                        }
                        if (dsCol.Tables[0].Rows[0]["ad_element_id"] != null && dsCol.Tables[0].Rows[0]["ad_element_id"] != DBNull.Value)
                        {
                            field.SetAD_Element_ID(Util.GetValueOfInt(dsCol.Tables[0].Rows[0]["ad_element_id"]));
                        }
                        if (dsCol.Tables[0].Rows[0]["ad_tab_id"] != null && dsCol.Tables[0].Rows[0]["ad_tab_id"] != DBNull.Value)
                        {
                            field.SetAD_Tab_ID(Util.GetValueOfInt(dsCol.Tables[0].Rows[0]["ad_tab_id"]));
                        }
                    }

                    if (!field.Save())
                    {
                        res.Append(field.GetName() + " NOT SAVED");
                    }
                }
            }


            /////////FieldGROUP Trl
            sql.Clear();
            sql.Append(@"SELECT ad_FieldGroup_id,
                                                      istranslated      ,
                                                      isactive          ,
                                                      name 
                                                      FROM ad_Fieldgroup_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    fgroup = new X_I_TLFieldGroup_Trl(GetCtx(), 0, null);
                    fgroup.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    fgroup.SetAD_Client_ID(0);
                    fgroup.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_FieldGroup_id"] != null && dsTab.Tables[0].Rows[j]["ad_FieldGroup_id"] != DBNull.Value)
                    {
                        fgroup.SetAD_FieldGroup_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_FieldGroup_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            fgroup.SetIsTranslated(true);
                        }
                        else
                        {
                            fgroup.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        fgroup.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            fgroup.SetIsActive(true);
                        }
                        else
                        {
                            fgroup.SetIsActive(false);
                        }
                    }
                    else
                    {
                        fgroup.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        fgroup.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }

                    if (!fgroup.Save())
                    {
                        res.Append(fgroup.GetName() + " NOT SAVED");
                    }
                }
            }

            /////////PROCESS Trl
            sql.Clear();
            sql.Append(@" SELECT ad_process_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help
                                               FROM ad_process_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    process = new X_I_TLProcess_Trl(GetCtx(), 0, null);
                    process.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    process.SetAD_Client_ID(0);
                    process.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_process_id"] != null && dsTab.Tables[0].Rows[j]["ad_process_id"] != DBNull.Value)
                    {
                        process.SetAD_Process_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_process_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            process.SetIsTranslated(true);
                        }
                        else
                        {
                            process.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        process.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            process.SetIsActive(true);
                        }
                        else
                        {
                            process.SetIsActive(false);
                        }
                    }
                    else
                    {
                        process.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        process.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        process.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        process.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (!process.Save())
                    {
                        res.Append(process.GetName() + " NOT SAVED");
                    }
                }
            }
            /////////FROM Trl
            sql.Clear();
            sql.Append(@" SELECT ad_form_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help
                                               FROM ad_form_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    form = new X_I_TLForm_Trl(GetCtx(), 0, null);
                    form.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    form.SetAD_Client_ID(0);
                    form.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_form_id"] != null && dsTab.Tables[0].Rows[j]["ad_form_id"] != DBNull.Value)
                    {
                        form.SetAD_Form_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_form_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            form.SetIsTranslated(true);
                        }
                        else
                        {
                            form.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        form.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            form.SetIsActive(true);
                        }
                        else
                        {
                            form.SetIsActive(false);
                        }
                    }
                    else
                    {
                        form.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        form.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        form.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        form.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (!form.Save())
                    {
                        res.Append(form.GetName() + " NOT SAVED");
                    }
                }
            }


            /////////TASK Trl
            sql.Clear();
            sql.Append(@" SELECT ad_task_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help
                                               FROM ad_task_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    task = new X_I_TLTask_Trl(GetCtx(), 0, null);
                    task.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    task.SetAD_Client_ID(0);
                    task.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_task_id"] != null && dsTab.Tables[0].Rows[j]["ad_task_id"] != DBNull.Value)
                    {
                        task.SetAD_Task_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_task_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            task.SetIsTranslated(true);
                        }
                        else
                        {
                            task.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        task.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            task.SetIsActive(true);
                        }
                        else
                        {
                            task.SetIsActive(false);
                        }
                    }
                    else
                    {
                        task.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        task.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        task.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        task.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (!task.Save())
                    {
                        res.Append(task.GetName() + " NOT SAVED");
                    }
                }
            }


            /////////WF Trl
            sql.Clear();
            sql.Append(@" SELECT ad_workflow_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help
                                               FROM ad_workflow_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    wf = new X_I_TLWorkflow_Trl(GetCtx(), 0, null);
                    wf.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    wf.SetAD_Client_ID(0);
                    wf.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_workflow_id"] != null && dsTab.Tables[0].Rows[j]["ad_workflow_id"] != DBNull.Value)
                    {
                        wf.SetAD_Workflow_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_workflow_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            wf.SetIsTranslated(true);
                        }
                        else
                        {
                            wf.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        wf.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            wf.SetIsActive(true);
                        }
                        else
                        {
                            wf.SetIsActive(false);
                        }
                    }
                    else
                    {
                        wf.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        wf.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        wf.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        wf.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (!wf.Save())
                    {
                        res.Append(wf.GetName() + " NOT SAVED");
                    }
                }
            }
            /////////WFNode Trl
            sql.Clear();
            sql.Append(@" SELECT ad_wf_node_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help
                                               FROM ad_wf_node_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    wfNode = new X_I_TLWF_Node_Trl(GetCtx(), 0, null);
                    wfNode.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    wfNode.SetAD_Client_ID(0);
                    wfNode.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_wf_node_id"] != null && dsTab.Tables[0].Rows[j]["ad_wf_node_id"] != DBNull.Value)
                    {
                        wfNode.SetAD_WF_Node_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_wf_node_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            wfNode.SetIsTranslated(true);
                        }
                        else
                        {
                            wfNode.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        wfNode.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            wfNode.SetIsActive(true);
                        }
                        else
                        {
                            wfNode.SetIsActive(false);
                        }
                    }
                    else
                    {
                        wfNode.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        wfNode.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        wfNode.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        wfNode.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (!wfNode.Save())
                    {
                        res.Append(wfNode.GetName() + " NOT SAVED");
                    }
                }
            }


            /////////Menu Trl
            sql.Clear();
            sql.Append(@" SELECT ad_menu_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description   
                                               FROM ad_menu_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    menu = new X_I_TLMenu_Trl(GetCtx(), 0,null);
                    menu.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    menu.SetAD_Client_ID(0);
                    menu.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_menu_id"] != null && dsTab.Tables[0].Rows[j]["ad_menu_id"] != DBNull.Value)
                    {
                        menu.SetAD_Menu_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_menu_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            menu.SetIsTranslated(true);
                        }
                        else
                        {
                            menu.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        menu.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            menu.SetIsActive(true);
                        }
                        else
                        {
                            menu.SetIsActive(false);
                        }
                    }
                    else
                    {
                        menu.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        menu.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        menu.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (!menu.Save())
                    {
                        res.Append(menu.GetName() + " NOT SAVED");
                    }
                }
            }
            /////////REF List Trl
            sql.Clear();
            sql.Append(@"SELECT ad_ref_list_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description                                                
                                               FROM ad_ref_list_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    reflst = new X_I_TLRef_List_Trl(GetCtx(), 0, null);
                    reflst.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    reflst.SetAD_Client_ID(0);
                    reflst.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_ref_list_id"] != null && dsTab.Tables[0].Rows[j]["ad_ref_list_id"] != DBNull.Value)
                    {
                        reflst.SetAD_Ref_List_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_ref_list_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            reflst.SetIsTranslated(true);
                        }
                        else
                        {
                            reflst.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        reflst.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            reflst.SetIsActive(true);
                        }
                        else
                        {
                            reflst.SetIsActive(false);
                        }
                    }
                    else
                    {
                        reflst.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        reflst.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        reflst.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (!reflst.Save())
                    {
                        res.Append(reflst.GetName() + " NOT SAVED");
                    }
                }
            }


            //if (res.Length == 0)
            //{
            //    res.Append("Done");
            //}
            return "Done";
        }
        string DeleteTranslations(int I_TLLanguage_ID)
        {
            DB.ExecuteQuery("DELETE I_TLElement_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLMessage_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLWindow_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLTab_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLField_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLFieldGroup_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLProcess_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLForm_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLTask_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLWorkflow_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLWF_Node_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLMenu_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLRef_List_Trl WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);
            DB.ExecuteQuery("DELETE I_TLLanguage WHERE I_TLLanguage_ID=" + I_TLLanguage_ID);

            return "Done";
        }
        string AddMissingTranslations(int I_TLLanguage_ID,string AD_Language)
        {
            //////////Element TRL
            sql.Clear();
            sql.Append(@" SELECT ad_element_id,
                                              istranslated       ,
                                              isactive           ,
                                              name               ,
                                              printname          ,
                                              description        ,
                                              help               ,
                                              po_name            ,
                                              po_printname       ,
                                              po_description     ,
                                              po_help
                                               FROM ad_element_trl 
                                        where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            int count = 0;
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {

                    sql.Clear();
                    sql.Append("SELECT Count(*) FROM I_TLElement_Trl WHERE AD_ELEMENT_ID="+dsTab.Tables[0].Rows[j]["AD_ELEMENT_ID"] +" AND I_TLLanguage_ID ="+I_TLLanguage_ID);
                    count=Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                    if(count>0)
                    {
                        continue;
                    }
                    element = new X_I_TLElement_Trl(GetCtx(), 0, null);
                    element.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    element.SetAD_Client_ID(0);
                    element.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_element_id"] != null && dsTab.Tables[0].Rows[j]["ad_element_id"] != DBNull.Value)
                    {
                        element.SetAD_Element_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_element_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            element.SetIsTranslated(true);
                        }
                        else
                        {
                            element.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        element.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            element.SetIsActive(true);
                        }
                        else
                        {
                            element.SetIsActive(false);
                        }
                    }
                    else
                    {
                        element.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        element.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["printname"] != null && dsTab.Tables[0].Rows[j]["printname"] != DBNull.Value)
                    {
                        element.SetPrintName(dsTab.Tables[0].Rows[j]["printname"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        element.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        element.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["po_name"] != null && dsTab.Tables[0].Rows[j]["po_name"] != DBNull.Value)
                    {
                        element.SetPO_Name(dsTab.Tables[0].Rows[j]["po_name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["po_printname"] != null && dsTab.Tables[0].Rows[j]["po_printname"] != DBNull.Value)
                    {
                        element.SetPO_PrintName(dsTab.Tables[0].Rows[j]["po_printname"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["po_description"] != null && dsTab.Tables[0].Rows[j]["po_description"] != DBNull.Value)
                    {
                        element.SetPO_Description(dsTab.Tables[0].Rows[j]["po_description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["po_help"] != null && dsTab.Tables[0].Rows[j]["po_help"] != DBNull.Value)
                    {
                        element.SetPO_Help(dsTab.Tables[0].Rows[j]["po_help"].ToString());
                    }
                    if (!element.Save())
                    {
                        res.Append(element.GetName() + " NOT SAVED");
                    }


                }
            }
            /////////Message Trl
            sql.Clear();
            sql.Append(@"SELECT ad_Message_id,
                                                      istranslated       ,
                                                      isactive           ,
                                                      msgtext            ,
                                                      msgtip
                                                       FROM ad_Message_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    sql.Clear();
                    sql.Append("SELECT Count(*) FROM I_TLMessage_Trl WHERE AD_MESSAGE_ID=" + dsTab.Tables[0].Rows[j]["AD_Message_ID"] + " AND I_TLLanguage_ID =" + I_TLLanguage_ID);
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                    if (count > 0)
                    {
                        continue;
                    }
                    msg = new X_I_TLMessage_Trl(GetCtx(), 0, null);
                    msg.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    msg.SetAD_Client_ID(0);
                    msg.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_Message_id"] != null && dsTab.Tables[0].Rows[j]["ad_Message_id"] != DBNull.Value)
                    {
                        msg.SetAD_Message_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_Message_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            msg.SetIsTranslated(true);
                        }
                        else
                        {
                            msg.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        msg.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            msg.SetIsActive(true);
                        }
                        else
                        {
                            msg.SetIsActive(false);
                        }
                    }
                    else
                    {
                        msg.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["msgtext"] != null && dsTab.Tables[0].Rows[j]["msgtext"] != DBNull.Value)
                    {
                        msg.SetMsgText(dsTab.Tables[0].Rows[j]["msgtext"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["msgtip"] != null && dsTab.Tables[0].Rows[j]["msgtip"] != DBNull.Value)
                    {
                        msg.SetMsgTip(dsTab.Tables[0].Rows[j]["msgtip"].ToString());
                    }
                    if (!msg.Save())
                    {
                        res.Append(msg.GetMsgText() + " NOT SAVED");
                    }

                }
            }


            /////////Window Trl
            sql.Clear();
            sql.Append(@"SELECT ad_window_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help
                                               FROM ad_window_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    sql.Clear();
                    sql.Append("SELECT Count(*) FROM I_TLWindow_Trl WHERE AD_Window_ID=" + dsTab.Tables[0].Rows[j]["AD_Window_ID"] + " AND I_TLLanguage_ID =" + I_TLLanguage_ID);
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                    if (count > 0)
                    {
                        continue;
                    }
                    win = new X_I_TLWindow_Trl(GetCtx(), 0, null);
                    win.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    win.SetAD_Client_ID(0);
                    win.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_window_id"] != null && dsTab.Tables[0].Rows[j]["ad_window_id"] != DBNull.Value)
                    {
                        win.SetAD_Window_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_window_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            win.SetIsTranslated(true);
                        }
                        else
                        {
                            win.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        win.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            win.SetIsActive(true);
                        }
                        else
                        {
                            win.SetIsActive(false);
                        }
                    }
                    else
                    {
                        win.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        win.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        win.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        win.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (!win.Save(null))
                    {
                        res.Append(win.GetName() + " NOT SAVED");
                    }
                }
            }

            /////////Tab Trl
            sql.Clear();
            sql.Append(@" SELECT ad_tab_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help, 
                                              commitwarning
                                               FROM ad_tab_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    sql.Clear();
                    sql.Append("SELECT Count(*) FROM I_TLTAB_Trl WHERE AD_TAB_ID=" + dsTab.Tables[0].Rows[j]["AD_Tab_ID"] + " AND I_TLLanguage_ID =" + I_TLLanguage_ID);
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                    if (count > 0)
                    {
                        continue;
                    }
                    tab = new X_I_TLTab_Trl(GetCtx(), 0, null);
                    tab.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    tab.SetAD_Client_ID(0);
                    tab.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_tab_id"] != null && dsTab.Tables[0].Rows[j]["ad_tab_id"] != DBNull.Value)
                    {
                        tab.SetAD_Tab_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_tab_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            tab.SetIsTranslated(true);
                        }
                        else
                        {
                            tab.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        tab.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            tab.SetIsActive(true);
                        }
                        else
                        {
                            tab.SetIsActive(false);
                        }
                    }
                    else
                    {
                        tab.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        tab.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        tab.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        tab.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["commitwarning"] != null && dsTab.Tables[0].Rows[j]["commitwarning"] != DBNull.Value)
                    {
                        tab.SetCommitWarning(dsTab.Tables[0].Rows[j]["commitwarning"].ToString());
                    }
                    if (!tab.Save())
                    {
                        res.Append(tab.GetName() + " NOT SAVED");
                    }
                }
            }

            /////////Field Trl
            sql.Clear();
            sql.Append(@" SELECT ad_Field_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help
                                               FROM ad_Field_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    sql.Clear();
                    sql.Append("SELECT Count(*) FROM I_TLField_Trl WHERE AD_Field_ID=" + dsTab.Tables[0].Rows[j]["AD_Field_ID"] + " AND I_TLLanguage_ID =" + I_TLLanguage_ID);
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                    if (count > 0)
                    {
                        continue;
                    }
                    field = new X_I_TLField_Trl(GetCtx(), 0, null);
                    field.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    field.SetAD_Client_ID(0);
                    field.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_Field_id"] != null && dsTab.Tables[0].Rows[j]["ad_Field_id"] != DBNull.Value)
                    {
                        field.SetAD_Field_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_Field_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            field.SetIsTranslated(true);
                        }
                        else
                        {
                            field.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        field.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            field.SetIsActive(true);
                        }
                        else
                        {
                            field.SetIsActive(false);
                        }
                    }
                    else
                    {
                        field.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        field.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        field.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        field.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    sql.Clear();
                    sql.Append(@" SELECT 
                                                 win.ad_window_id,
                                                 col.ad_element_id, 
                                                 tab.ad_tab_id
                                                   FROM ad_field fld
                                                INNER JOIN ad_tab tab
                                                     ON (tab.ad_tab_id= fld.ad_tab_id)
                                                INNER JOIN ad_window win
                                                     ON (tab.ad_window_id= win.ad_window_id)
                                                     inner join ad_column col on(col.ad_column_id= fld.ad_column_id)
                                                     where fld.ad_field_ID=" + dsTab.Tables[0].Rows[j]["ad_Field_id"] + "");
                    dsCol = DB.ExecuteDataset(sql.ToString());
                    if (dsCol != null && dsCol.Tables[0].Rows.Count > 0)
                    {
                        if (dsCol.Tables[0].Rows[0]["ad_window_id"] != null && dsCol.Tables[0].Rows[0]["ad_window_id"] != DBNull.Value)
                        {
                            field.SetAD_Window_ID(Util.GetValueOfInt(dsCol.Tables[0].Rows[0]["ad_window_id"]));
                        }
                        if (dsCol.Tables[0].Rows[0]["ad_element_id"] != null && dsCol.Tables[0].Rows[0]["ad_element_id"] != DBNull.Value)
                        {
                            field.SetAD_Element_ID(Util.GetValueOfInt(dsCol.Tables[0].Rows[0]["ad_element_id"]));
                        }
                        if (dsCol.Tables[0].Rows[0]["ad_tab_id"] != null && dsCol.Tables[0].Rows[0]["ad_tab_id"] != DBNull.Value)
                        {
                            field.SetAD_Tab_ID(Util.GetValueOfInt(dsCol.Tables[0].Rows[0]["ad_tab_id"]));
                        }
                    }

                    if (!field.Save())
                    {
                        res.Append(field.GetName() + " NOT SAVED");
                    }
                }
            }


            /////////FieldGROUP Trl
            sql.Clear();
            sql.Append(@"SELECT ad_FieldGroup_id,
                                                      istranslated      ,
                                                      isactive          ,
                                                      name 
                                                      FROM ad_Fieldgroup_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    sql.Clear();
                    sql.Append("SELECT Count(*) FROM I_TLFieldGroup_Trl WHERE AD_FieldGroup_ID=" + dsTab.Tables[0].Rows[j]["AD_FieldGroup_ID"] + " AND I_TLLanguage_ID =" + I_TLLanguage_ID);
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                    if (count > 0)
                    {
                        continue;
                    }
                    fgroup = new X_I_TLFieldGroup_Trl(GetCtx(), 0, null);
                    fgroup.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    fgroup.SetAD_Client_ID(0);
                    fgroup.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_FieldGroup_id"] != null && dsTab.Tables[0].Rows[j]["ad_FieldGroup_id"] != DBNull.Value)
                    {
                        fgroup.SetAD_FieldGroup_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_FieldGroup_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            fgroup.SetIsTranslated(true);
                        }
                        else
                        {
                            fgroup.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        fgroup.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            fgroup.SetIsActive(true);
                        }
                        else
                        {
                            fgroup.SetIsActive(false);
                        }
                    }
                    else
                    {
                        fgroup.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        fgroup.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }

                    if (!fgroup.Save())
                    {
                        res.Append(fgroup.GetName() + " NOT SAVED");
                    }
                }
            }

            /////////PROCESS Trl
            sql.Clear();
            sql.Append(@" SELECT ad_process_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help
                                               FROM ad_process_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    sql.Clear();
                    sql.Append("SELECT Count(*) FROM I_TLPROCESS_Trl WHERE AD_PROCESS_ID=" + dsTab.Tables[0].Rows[j]["AD_Process_ID"] + " AND I_TLLanguage_ID =" + I_TLLanguage_ID);
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                    if (count > 0)
                    {
                        continue;
                    }
                    process = new X_I_TLProcess_Trl(GetCtx(), 0, null);
                    process.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    process.SetAD_Client_ID(0);
                    process.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_process_id"] != null && dsTab.Tables[0].Rows[j]["ad_process_id"] != DBNull.Value)
                    {
                        process.SetAD_Process_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_process_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            process.SetIsTranslated(true);
                        }
                        else
                        {
                            process.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        process.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            process.SetIsActive(true);
                        }
                        else
                        {
                            process.SetIsActive(false);
                        }
                    }
                    else
                    {
                        process.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        process.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        process.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        process.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (!process.Save())
                    {
                        res.Append(process.GetName() + " NOT SAVED");
                    }
                }
            }
            /////////FROM Trl
            sql.Clear();
            sql.Append(@" SELECT ad_form_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help
                                               FROM ad_form_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    sql.Clear();
                    sql.Append("SELECT Count(*) FROM I_TLFORM_Trl WHERE AD_FORM_ID=" + dsTab.Tables[0].Rows[j]["AD_Form_ID"] + " AND I_TLLanguage_ID =" + I_TLLanguage_ID);
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                    if (count > 0)
                    {
                        continue;
                    }
                    form = new X_I_TLForm_Trl(GetCtx(), 0, null);
                    form.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    form.SetAD_Client_ID(0);
                    form.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_form_id"] != null && dsTab.Tables[0].Rows[j]["ad_form_id"] != DBNull.Value)
                    {
                        form.SetAD_Form_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_form_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            form.SetIsTranslated(true);
                        }
                        else
                        {
                            form.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        form.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            form.SetIsActive(true);
                        }
                        else
                        {
                            form.SetIsActive(false);
                        }
                    }
                    else
                    {
                        form.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        form.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        form.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        form.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (!form.Save())
                    {
                        res.Append(form.GetName() + " NOT SAVED");
                    }
                }
            }


            /////////TASK Trl
            sql.Clear();
            sql.Append(@" SELECT ad_task_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help
                                               FROM ad_task_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    sql.Clear();
                    sql.Append("SELECT Count(*) FROM I_TLTASK_Trl WHERE AD_TASK_ID=" + dsTab.Tables[0].Rows[j]["AD_Task_ID"] + " AND I_TLLanguage_ID =" + I_TLLanguage_ID);
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                    if (count > 0)
                    {
                        continue;
                    }
                    task = new X_I_TLTask_Trl(GetCtx(), 0, null);
                    task.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    task.SetAD_Client_ID(0);
                    task.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_task_id"] != null && dsTab.Tables[0].Rows[j]["ad_task_id"] != DBNull.Value)
                    {
                        task.SetAD_Task_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_task_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            task.SetIsTranslated(true);
                        }
                        else
                        {
                            task.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        task.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            task.SetIsActive(true);
                        }
                        else
                        {
                            task.SetIsActive(false);
                        }
                    }
                    else
                    {
                        task.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        task.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        task.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        task.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (!task.Save())
                    {
                        res.Append(task.GetName() + " NOT SAVED");
                    }
                }
            }


            /////////WF Trl
            sql.Clear();
            sql.Append(@" SELECT ad_workflow_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help
                                               FROM ad_workflow_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    sql.Clear();
                    sql.Append("SELECT Count(*) FROM I_TLWORKFLOW_Trl WHERE AD_WORKFLOW_ID=" + dsTab.Tables[0].Rows[j]["AD_Workflow_ID"] + " AND I_TLLanguage_ID =" + I_TLLanguage_ID);
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                    if (count > 0)
                    {
                        continue;
                    }
                    wf = new X_I_TLWorkflow_Trl(GetCtx(), 0, null);
                    wf.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    wf.SetAD_Client_ID(0);
                    wf.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_workflow_id"] != null && dsTab.Tables[0].Rows[j]["ad_workflow_id"] != DBNull.Value)
                    {
                        wf.SetAD_Workflow_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_workflow_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            wf.SetIsTranslated(true);
                        }
                        else
                        {
                            wf.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        wf.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            wf.SetIsActive(true);
                        }
                        else
                        {
                            wf.SetIsActive(false);
                        }
                    }
                    else
                    {
                        wf.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        wf.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        wf.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        wf.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (!wf.Save())
                    {
                        res.Append(wf.GetName() + " NOT SAVED");
                    }
                }
            }
            /////////WFNode Trl
            sql.Clear();
            sql.Append(@" SELECT ad_wf_node_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description       ,
                                              help
                                               FROM ad_wf_node_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    sql.Clear();
                    sql.Append("SELECT Count(*) FROM I_TLWF_NODE_Trl WHERE AD_WF_NODE_ID=" + dsTab.Tables[0].Rows[j]["AD_wf_node_ID"] + " AND I_TLLanguage_ID =" + I_TLLanguage_ID);
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                    if (count > 0)
                    {
                        continue;
                    }
                    wfNode = new X_I_TLWF_Node_Trl(GetCtx(), 0, null);
                    wfNode.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    wfNode.SetAD_Client_ID(0);
                    wfNode.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_wf_node_id"] != null && dsTab.Tables[0].Rows[j]["ad_wf_node_id"] != DBNull.Value)
                    {
                        wfNode.SetAD_WF_Node_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_wf_node_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            wfNode.SetIsTranslated(true);
                        }
                        else
                        {
                            wfNode.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        wfNode.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            wfNode.SetIsActive(true);
                        }
                        else
                        {
                            wfNode.SetIsActive(false);
                        }
                    }
                    else
                    {
                        wfNode.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        wfNode.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        wfNode.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["help"] != null && dsTab.Tables[0].Rows[j]["help"] != DBNull.Value)
                    {
                        wfNode.SetHelp(dsTab.Tables[0].Rows[j]["help"].ToString());
                    }
                    if (!wfNode.Save())
                    {
                        res.Append(wfNode.GetName() + " NOT SAVED");
                    }
                }
            }


            /////////Menu Trl
            sql.Clear();
            sql.Append(@" SELECT ad_menu_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description   
                                               FROM ad_menu_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    sql.Clear();
                    sql.Append("SELECT Count(*) FROM I_TLMENU_Trl WHERE AD_MENU_ID=" + dsTab.Tables[0].Rows[j]["AD_menu_ID"] + " AND I_TLLanguage_ID =" + I_TLLanguage_ID);
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                    if (count > 0)
                    {
                        continue;
                    }
                    menu = new X_I_TLMenu_Trl(GetCtx(), 0, null);
                    menu.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    menu.SetAD_Client_ID(0);
                    menu.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_menu_id"] != null && dsTab.Tables[0].Rows[j]["ad_menu_id"] != DBNull.Value)
                    {
                        menu.SetAD_Menu_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_menu_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            menu.SetIsTranslated(true);
                        }
                        else
                        {
                            menu.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        menu.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            menu.SetIsActive(true);
                        }
                        else
                        {
                            menu.SetIsActive(false);
                        }
                    }
                    else
                    {
                        menu.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        menu.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        menu.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (!menu.Save())
                    {
                        res.Append(menu.GetName() + " NOT SAVED");
                    }
                }
            }
            /////////REF List Trl
            sql.Clear();
            sql.Append(@"SELECT ad_ref_list_id,
                                              istranslated      ,
                                              isactive          ,
                                              name              ,
                                              description                                                
                                               FROM ad_ref_list_trl
                                              where ad_language='" + AD_Language + "' and ad_client_id=0");
            dsTab = DB.ExecuteDataset(sql.ToString());
            if (dsTab != null)
            {
                for (int j = 0; j < dsTab.Tables[0].Rows.Count; j++)
                {
                    sql.Clear();
                    sql.Append("SELECT Count(*) FROM I_TLREF_LIST_Trl WHERE AD_REF_LIST_ID=" + dsTab.Tables[0].Rows[j]["AD_ref_list_ID"] + " AND I_TLLanguage_ID =" + I_TLLanguage_ID);
                    count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                    if (count > 0)
                    {
                        continue;
                    }
                    reflst = new X_I_TLRef_List_Trl(GetCtx(), 0, null);
                    reflst.SetI_TLLanguage_ID(lang.GetI_TLLanguage_ID());
                    reflst.SetAD_Client_ID(0);
                    reflst.SetAD_Org_ID(0);
                    if (dsTab.Tables[0].Rows[j]["ad_ref_list_id"] != null && dsTab.Tables[0].Rows[j]["ad_ref_list_id"] != DBNull.Value)
                    {
                        reflst.SetAD_Ref_List_ID(Util.GetValueOfInt(dsTab.Tables[0].Rows[j]["ad_ref_list_id"]));
                    }
                    if (dsTab.Tables[0].Rows[j]["istranslated"] != null && dsTab.Tables[0].Rows[j]["istranslated"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["istranslated"].ToString().Equals("Y"))
                        {
                            reflst.SetIsTranslated(true);
                        }
                        else
                        {
                            reflst.SetIsTranslated(false);
                        }
                    }
                    else
                    {
                        reflst.SetIsTranslated(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["isactive"] != null && dsTab.Tables[0].Rows[j]["isactive"] != DBNull.Value)
                    {
                        if (dsTab.Tables[0].Rows[j]["isactive"].ToString().Equals("Y"))
                        {
                            reflst.SetIsActive(true);
                        }
                        else
                        {
                            reflst.SetIsActive(false);
                        }
                    }
                    else
                    {
                        reflst.SetIsActive(false);
                    }
                    if (dsTab.Tables[0].Rows[j]["name"] != null && dsTab.Tables[0].Rows[j]["name"] != DBNull.Value)
                    {
                        reflst.SetName(dsTab.Tables[0].Rows[j]["name"].ToString());
                    }
                    if (dsTab.Tables[0].Rows[j]["description"] != null && dsTab.Tables[0].Rows[j]["description"] != DBNull.Value)
                    {
                        reflst.SetDescription(dsTab.Tables[0].Rows[j]["description"].ToString());
                    }
                    if (!reflst.Save())
                    {
                        res.Append(reflst.GetName() + " NOT SAVED");
                    }
                }
            }


            //if (res.Length == 0)
            //{
            //    res.Append("Done");
            //}
            return "Done";
        }
    }
}

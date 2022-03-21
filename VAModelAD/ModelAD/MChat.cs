/********************************************************
 * Project Name   : VAdvantage
 * Form Name      : Chat
 * Purpose        : To featch/insert(get/set) data in the CM_CHAT table.
 * Class Used     : MChat (inherits X_CM_Chat class) 
 * Chronological    Development
 * Raghunandan      13-March-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Logging;
using VAdvantage.Common;
//using VAdvantage.Grid;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MChat : X_CM_Chat
    {
        #region Private Variable
        private MChatEntry[] _chatEntries = null;//chat entries
        //private SimpleDateFormat _createdDate = null;
        private DateTime _createdDate;//get date from database according to chat entry
        private DataSet _ds = null;//data set for CM_ChatEntry table
        //public Ctx ctx = Utility.Env.GetCtx();//context
        private DateTime _format;
        //Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MChat).FullName);
        #endregion

        /// <summary>
        /// Get Chats Of Table - of client in context
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">table</param>
        /// <returns> array of chats</returns>
        public static MChat[] GetOfTable(Ctx ctx, int AD_Table_ID)
        {
            int AD_Client_ID = ctx.GetAD_Client_ID();
            List<MChat> list = new List<MChat>();
            //
            String sql = "SELECT * FROM CM_Chat "
                + "WHERE AD_Client_ID=" + AD_Client_ID + " AND AD_Table_ID=" + AD_Table_ID + " ORDER BY Record_ID";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)// while (rs.next())
                {
                    list.Add(new MChat(ctx, dr, null));
                }
            }
            catch (Exception e)
            {
                idr.Close();
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                idr.Close();
            }
            MChat[] retValue = new MChat[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        #region Full Constructor   
        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Table_ID">AD_Table_ID table</param>
        /// <param name="Record_ID">Record_ID record</param>
        /// <param name="Description">Description description</param>
        /// <param name="trxName">trxName transaction</param>
        public MChat(Ctx ctx, int AD_Table_ID, int Record_ID,
        String Description, Trx trxName)
            : this(ctx, 0, trxName)
        {
            //set tableID
            SetAD_Table_ID(AD_Table_ID);
            //set record id
            SetRecord_ID(Record_ID);
            //set discription
            SetDescription(Description);
        }
        #endregion

        #region Standard Constructor   
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">ctx context</param>
        /// <param name="CM_Chat_ID">CM_Chat_ID id</param>
        /// <param name="trxName">trxName transcation</param>
        public MChat(Ctx ctx, int CM_Chat_ID, Trx trxName)
            : base(ctx, CM_Chat_ID, trxName)
        {
            if (CM_Chat_ID == 0)
            {
                //set table id
                //  SetAD_Table_ID(0);
                // SetRecord_ID(0);
                SetConfidentialType(CONFIDENTIALTYPE_PublicInformation);//set confidential type 
                SetModerationType(MODERATIONTYPE_NotModerated);//set modrate
                // SetDescription(null);
            }
        }
        #endregion

        #region Load Constructor   
        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">ctx context</param>
        /// <param name="rs">rs result set</param>
        /// <param name="trxName">trxName transaction</param>
        public MChat(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            //super (ctx, rs, trxName);
        }
        #endregion

        #region GetEntries   
        /// <summary>
        ///Get Entries
        /// </summary>
        /// <param name="reload">Bool Type(reload data)</param>
        /// <returns>array of lines</returns>
        public MChatEntry[] GetEntries(Boolean reload)
        {
            //chat entries
            if (_chatEntries != null && !reload)
                return _chatEntries;//return chat
            //list for chatEntry records
            List<MChatEntry> list = new List<MChatEntry>();
            String sql = "SELECT * FROM CM_ChatEntry WHERE CM_Chat_ID=" + GetCM_Chat_ID() + " ORDER BY Created";

            try
            {
                //execute the Query get number of chat for a perticular CHAT_ID
                _ds = DataBase.DB.ExecuteDataset(sql, null, null);

                DataRow rs;
                for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                {
                    rs = _ds.Tables[0].Rows[i];
                    //list.Add(new MChatEntry(GetCtx, rs, Get_TrxName()));
                    //add chatentries into list from CM_ChatEntry table
                    list.Add(new MChatEntry(GetCtx(), rs, Get_TrxName()));
                }
                //_ds = null;

            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            //count number of records
            _chatEntries = new MChatEntry[list.Count];
            //add list into array
            _chatEntries = list.ToArray();
            //list.ToArray(_chatEntries);1..........................
            return _chatEntries;
        }
        #endregion

        /// <summary>
        /// Set Description
        /// </summary>
        /// <param name="Description">discription</param>
        public new void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 0)
            {
                base.SetDescription(Description);
            }
            else
            {
                base.SetDescription(GetAD_Table_ID() + "#" + GetRecord_ID());
            }
        }

        #region Chat History    
        /// <summary>
        /// Get History as text from data base using Html display formate
        /// </summary>
        /// <param name="ConfidentialType">confidential type</param>
        /// <returns>text from control</returns>
        public string GetHistory(String confidentialType)
        {
            GetEntries(true);//array list status
            Boolean first = true;
            //text to show in browser
            StringBuilder strbChatText = new StringBuilder("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>" +
"<html xmlns='http://www.w3.org/1999/xhtml'>" +
"<head>" +
"<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />" +
"<title>Untitled Document</title>" +
"<style type='text/css'>" +
"<!--" +
"body {" +
    "margin-left: 0px;" +
    "margin-top: 0px;" +
    "margin-right: 0px;" +
    "margin-bottom: 0px;" +
    "font-family:Arial;" +
    "font-size:12px;" +
"}" +
".grey" +
"{" +
"color:#969292;" +
"}" +
".grey-1" +
"{" +
"font-size:10px;" +
"color:#969292;" +
"}" +
"-->" +
"</style></head>" +
"<body>" +
"<table width='100%' style='background-color:#F6F6F6' border='0' align='left' cellpadding='4' cellspacing='0'>");
            //loop for each Chat record in Array
            for (int i = 0; i < _chatEntries.Length; i++)
            {
                MChatEntry entry = _chatEntries[i];
                //get the created date of a perticular chat from PO
                _createdDate = entry.GetCreated();
                _format = new DateTime(_createdDate.Year, _createdDate.Month, _createdDate.Day, _createdDate.Hour, _createdDate.Minute, _createdDate.Second);
                _createdDate = _format;
                //check for valid value
                if (!entry.IsActive() || !entry.IsConfidentialTypeValid(confidentialType))
                    continue;
                //status for first chat
                if (first)
                    first = false;
                //only show those records where Character data is not empty
                if (entry.GetCharacterData() != null)
                {
                    //set table
                    strbChatText.Append("<tr>" +
    "<td><table width='100%' border='0' align='left' cellpadding='0' cellspacing='0'>" +
      "<tr>" +
        "<td class='grey'></td>" +
        "<td>&nbsp;</td>" +
      "</tr>" +
      "<tr>" +
        "<td width='76px' class='grey'>" + GetCtx().GetAD_User_Name() + " :</td>" +//Show the Login User Name
        "<td width='100%' Style='text-align:left' ><hr /></td>" +
      "</tr>" +
      "<tr>" +
        "<td>&nbsp;</td>" +
        "<td align='right' class='grey-1'>" + _createdDate + "</td>" +//show the chat created date
      "</tr>" +
      "<tr>" +
        "<td colspan='2' width='100%'>" + entry.GetCharacterData() + "</td>" +//Show the chat
        "</tr>" +
      "<tr>" +
        "<td colspan='2'>&nbsp;</td>" +
      "</tr>" +
    "</table></td>" +
  "</tr>" +
  "<tr>" +
    "<td class='grey'>&nbsp;</td>" +
  "</tr>");
                    //if (i == 0)
                    //{

                    //    //_createdDate
                    //    data += ctx.GetAD_User_Name() + ":__________\n  " + _createdDate.Date+"\n\n" + entry.GetCharacterData();
                    //}
                    //else
                    //{
                    //    data1 += "\n\n" + ctx.GetAD_User_Name() + ":_____________________\n        " + _createdDate.Date + "\n\n" + entry.GetCharacterData();
                    //}
                }
            }
            strbChatText.Append("</table></body></html>");//close table
            //data += data1;
            //return data;
            //return string text with HTML
            return strbChatText.ToString();
        }
        #endregion
    }
}

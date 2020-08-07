/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MMailMsg
 * Purpose        : Wab Store Mail Message Model
 * Class Used     : X_W_MailMsg
 * Chronological    Development
 * Deepak           07-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
     public class MMailMsg:X_W_MailMsg
    {
       /// <summary>
       /// Standard Constructor
       /// </summary>
       /// <param name="ctx">context</param>
       /// <param name="W_MailMsg_ID">id</param>
       /// <param name="trxName"> trx</param>
        public MMailMsg(Ctx ctx, int W_MailMsg_ID, Trx trxName):base(ctx, W_MailMsg_ID, trxName)
        {
            
            if (W_MailMsg_ID == 0)
            {
                //	setW_Store_ID (0);
                //	setMailMsgType (null);
                //	setName (null);
                //	setSubject (null);
                //	setMessage (null);
            }
        }	//	MMailMsg

       /// <summary>
       ///  Load Constructor
       /// </summary>
       /// <param name="ctx">context</param>
       /// <param name="dr">datarow</param>
       /// <param name="trxName">trx</param>
         public MMailMsg(Ctx ctx, DataRow dr, Trx trxName):base(ctx, dr, trxName)
        { 
            
        }	//	MMailMsg

       /// <summary>
       ///  Full Constructor
       /// </summary>
       /// <param name="parent">store</param>
       /// <param name="MailMsgType">msg type</param>
       /// <param name="Name">name</param>
       /// <param name="Subject">Subject</param>
       /// <param name="Message">Message</param>
       /// <param name="Message2">Message</param>
       /// <param name="Message3">Message</param>
        public MMailMsg(MStore parent, String MailMsgType,
            String Name, String Subject, String Message, String Message2, String Message3):this(parent.GetCtx(), 0, parent.Get_TrxName())
         {
            
            SetClientOrg(parent);
            SetW_Store_ID(parent.GetW_Store_ID());
            SetMailMsgType(MailMsgType);
            SetName(Name);
            SetSubject(Subject);
            SetMessage(Message);
            SetMessage2(Message2);
            SetMessage3(Message3);
        }	//	MMailMsg

    }	//	MMailMsg


}

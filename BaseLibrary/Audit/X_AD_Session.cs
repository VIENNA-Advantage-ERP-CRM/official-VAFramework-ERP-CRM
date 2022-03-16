namespace VAdvantage.Model
{

    /** Generated Model - DO NOT CHANGE */
    using System;
    using System.Text;
    using VAdvantage.DataBase;
    
    using VAdvantage.Classes;
   
    using VAdvantage.Utility;
    using System.Data;
    /** Generated Model for AD_Session
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_Session : PO
    {
        public X_AD_Session(Context ctx, int AD_Session_ID, Trx trxName)
            : base(ctx, AD_Session_ID, trxName)
        {
            /** if (AD_Session_ID == 0)
            {
            SetAD_Session_ID (0);
            SetProcessed (false);	// N
            }
             */
        }
        public X_AD_Session(Ctx ctx, int AD_Session_ID, Trx trxName)
            : base(ctx, AD_Session_ID, trxName)
        {
            /** if (AD_Session_ID == 0)
            {
            SetAD_Session_ID (0);
            SetProcessed (false);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_Session(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_Session(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_Session(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_Session()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514364163L;
        /** Last Updated Timestamp 7/29/2010 1:07:27 PM */
        public static long updatedMS = 1280389047374L;
        /** AD_Table_ID=566 */
        public static int Table_ID;
        // =566;

        /** TableName=AD_Session */
        public static String Table_Name = "AD_Session";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(6);
        /** AccessLevel
        @return 6 - System - Client 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_AD_Session[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Role.
        @param AD_Role_ID Responsibility Role */
        public void SetAD_Role_ID(int AD_Role_ID)
        {
            if (AD_Role_ID <= 0) Set_Value("AD_Role_ID", null);
            else
                Set_Value("AD_Role_ID", AD_Role_ID);
        }
        /** Get Role.
        @return Responsibility Role */
        public int GetAD_Role_ID()
        {
            Object ii = Get_Value("AD_Role_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Session.
        @param AD_Session_ID User Session Online or Web */
        public void SetAD_Session_ID(int AD_Session_ID)
        {
            if (AD_Session_ID < 1) throw new ArgumentException("AD_Session_ID is mandatory.");
            Set_ValueNoCheck("AD_Session_ID", AD_Session_ID);
        }
        /** Get Session.
        @return User Session Online or Web */
        public int GetAD_Session_ID()
        {
            Object ii = Get_Value("AD_Session_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetAD_Session_ID().ToString());
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                Description = Description.Substring(0, 255);
            }
            Set_Value("Description", Description);
        }
        /** Get Description.
        @return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
        }
        /** Set Processed.
        @param Processed The document has been processed */
        public void SetProcessed(Boolean Processed)
        {
            Set_ValueNoCheck("Processed", Processed);
        }
        /** Get Processed.
        @return The document has been processed */
        public Boolean IsProcessed()
        {
            Object oo = Get_Value("Processed");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Remote Addr.
        @param Remote_Addr Remote Address */
        public void SetRemote_Addr(String Remote_Addr)
        {
            if (Remote_Addr != null && Remote_Addr.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Remote_Addr = Remote_Addr.Substring(0, 60);
            }
            Set_ValueNoCheck("Remote_Addr", Remote_Addr);
        }
        /** Get Remote Addr.
        @return Remote Address */
        public String GetRemote_Addr()
        {
            return (String)Get_Value("Remote_Addr");
        }
        /** Set Remote Host.
        @param Remote_Host Remote host Info */
        public void SetRemote_Host(String Remote_Host)
        {
            if (Remote_Host != null && Remote_Host.Length > 120)
            {
                log.Warning("Length > 120 - truncated");
                Remote_Host = Remote_Host.Substring(0, 120);
            }
            Set_ValueNoCheck("Remote_Host", Remote_Host);
        }
        /** Get Remote Host.
        @return Remote host Info */
        public String GetRemote_Host()
        {
            return (String)Get_Value("Remote_Host");
        }
        /** Set Web Session.
        @param WebSession Web Session ID */
        public void SetWebSession(String WebSession)
        {
            if (WebSession != null && WebSession.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                WebSession = WebSession.Substring(0, 40);
            }
            Set_ValueNoCheck("WebSession", WebSession);
        }

        /** Get Web Session.
        @return Web Session ID */
        public String GetWebSession()
        {
            return (String)Get_Value("WebSession");
        }

        /** Set Request Address.
        @param Request_Addr Request Address */
        public void SetRequest_Addr(String Request_Addr)
        {
            if (Request_Addr != null && Request_Addr.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Request_Addr = Request_Addr.Substring(0, 60);
            }
            Set_Value("Request_Addr", Request_Addr);
        }

        /** Get Request Address.
        @return Request Address */
        public String GetRequest_Addr()
        {
            return (String)Get_Value("Request_Addr");
        }
    }

}

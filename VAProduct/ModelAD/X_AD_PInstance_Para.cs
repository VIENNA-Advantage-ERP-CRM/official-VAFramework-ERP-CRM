namespace VAdvantage.Model
{

    /** Generated Model - DO NOT CHANGE */
    using System;
    using System.Text;
    using VAdvantage.DataBase;
    using VAdvantage.Common;
    using VAdvantage.Classes;
    using VAdvantage.Process;
    using VAdvantage.Model;
    using VAdvantage.Utility;
    using System.Data;
    /** Generated Model for AD_PInstance_Para
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_PInstance_Para : PO
    {
        public X_AD_PInstance_Para(Context ctx, int AD_PInstance_Para_ID, Trx trxName)
            : base(ctx, AD_PInstance_Para_ID, trxName)
        {
            /** if (AD_PInstance_Para_ID == 0)
            {
            SetAD_PInstance_ID (0);
            SetSeqNo (0);
            }
             */
        }
        public X_AD_PInstance_Para(Ctx ctx, int AD_PInstance_Para_ID, Trx trxName)
            : base(ctx, AD_PInstance_Para_ID, trxName)
        {
            /** if (AD_PInstance_Para_ID == 0)
            {
            SetAD_PInstance_ID (0);
            SetSeqNo (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_PInstance_Para(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_PInstance_Para(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AD_PInstance_Para(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AD_PInstance_Para()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID = 27562514362470L;
        /** Last Updated Timestamp 7/29/2010 1:07:25 PM */
        public static long updatedMS = 1280389045681L;
        /** AD_Table_ID=283 */
        public static int Table_ID;
        // =283;

        /** TableName=AD_PInstance_Para */
        public static String Table_Name = "AD_PInstance_Para";

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
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
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
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_AD_PInstance_Para[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Process Instance.
        @param AD_PInstance_ID Instance of the process */
        public void SetAD_PInstance_ID(int AD_PInstance_ID)
        {
            if (AD_PInstance_ID < 1) throw new ArgumentException("AD_PInstance_ID is mandatory.");
            Set_ValueNoCheck("AD_PInstance_ID", AD_PInstance_ID);
        }
        /** Get Process Instance.
        @return Instance of the process */
        public int GetAD_PInstance_ID()
        {
            Object ii = Get_Value("AD_PInstance_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetAD_PInstance_ID().ToString());
        }
        /** Set Info.
        @param Info Information */
        public void SetInfo(String Info)
        {
            if (Info != null && Info.Length > 1000)
            {
                log.Warning("Length > 1000 - truncated");
                Info = Info.Substring(0, 1000);
            }
            Set_Value("Info", Info);
        }
        /** Get Info.
        @return Information */
        public String GetInfo()
        {
            return (String)Get_Value("Info");
        }
        /** Set Info To.
        @param Info_To Info To */
        public void SetInfo_To(String Info_To)
        {
            if (Info_To != null && Info_To.Length > 1000)
            {
                log.Warning("Length > 1000 - truncated");
                Info_To = Info_To.Substring(0, 1000);
            }
            Set_Value("Info_To", Info_To);
        }
        /** Get Info To.
        @return Info To */
        public String GetInfo_To()
        {
            return (String)Get_Value("Info_To");
        }
        /** Set Process Date.
        @param P_Date Process Parameter */
        public void SetP_Date(DateTime? P_Date)
        {
            Set_Value("P_Date", (DateTime?)P_Date);
        }
        /** Get Process Date.
        @return Process Parameter */
        public DateTime? GetP_Date()
        {
            return (DateTime?)Get_Value("P_Date");
        }
        /** Set Process Date To.
        @param P_Date_To Process Parameter */
        public void SetP_Date_To(DateTime? P_Date_To)
        {
            Set_Value("P_Date_To", (DateTime?)P_Date_To);
        }
        /** Get Process Date To.
        @return Process Parameter */
        public DateTime? GetP_Date_To()
        {
            return (DateTime?)Get_Value("P_Date_To");
        }
        /** Set Process Number.
        @param P_Number Process Parameter */
        public void SetP_Number(Decimal? P_Number)
        {
            Set_Value("P_Number", (Decimal?)P_Number);
        }
        /** Get Process Number.
        @return Process Parameter */
        public Decimal GetP_Number()
        {
            Object bd = Get_Value("P_Number");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Process Number To.
        @param P_Number_To Process Parameter */
        public void SetP_Number_To(Decimal? P_Number_To)
        {
            Set_Value("P_Number_To", (Decimal?)P_Number_To);
        }
        /** Get Process Number To.
        @return Process Parameter */
        public Decimal GetP_Number_To()
        {
            Object bd = Get_Value("P_Number_To");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Process String.
        @param P_String Process Parameter */
        public void SetP_String(String P_String)
        {
            if (P_String != null && P_String.Length > 1000)
            {
                log.Warning("Length > 1000 - truncated");
                P_String = P_String.Substring(0, 1000);
            }
            Set_Value("P_String", P_String);
        }
        /** Get Process String.
        @return Process Parameter */
        public String GetP_String()
        {
            return (String)Get_Value("P_String");
        }
        /** Set Process String To.
        @param P_String_To Process Parameter */
        public void SetP_String_To(String P_String_To)
        {
            if (P_String_To != null && P_String_To.Length > 150)
            {
                log.Warning("Length > 150 - truncated");
                P_String_To = P_String_To.Substring(0, 150);
            }
            Set_Value("P_String_To", P_String_To);
        }
        /** Get Process String To.
        @return Process Parameter */
        public String GetP_String_To()
        {
            return (String)Get_Value("P_String_To");
        }
        /** Set Parameter Name.
        @param ParameterName Parameter Name */
        public void SetParameterName(String ParameterName)
        {
            if (ParameterName != null && ParameterName.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                ParameterName = ParameterName.Substring(0, 60);
            }
            Set_Value("ParameterName", ParameterName);
        }
        /** Get Parameter Name.
        @return Parameter Name */
        public String GetParameterName()
        {
            return (String)Get_Value("ParameterName");
        }
        /** Set Sequence.
        @param SeqNo Method of ordering elements;
         lowest number comes first */
        public void SetSeqNo(int SeqNo)
        {
            Set_ValueNoCheck("SeqNo", SeqNo);
        }
        /** Get Sequence.
        @return Method of ordering elements;
         lowest number comes first */
        public int GetSeqNo()
        {
            Object ii = Get_Value("SeqNo");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }


        /** Set Date Time.
@param P_Date_Time Date Time */
        public void SetP_Date_Time(DateTime? P_Date_Time) { Set_Value("P_Date_Time", (DateTime?)P_Date_Time); }/** Get Date Time.
@return Date Time */
        public DateTime? GetP_Date_Time() { return (DateTime?)Get_Value("P_Date_Time"); }/** Set Date Time To.
@param P_Date_Time_To Date Time To */
        public void SetP_Date_Time_To(DateTime? P_Date_Time_To) { Set_Value("P_Date_Time_To", (DateTime?)P_Date_Time_To); }/** Get Date Time To.
@return Date Time To */
        public DateTime? GetP_Date_Time_To() { return (DateTime?)Get_Value("P_Date_Time_To"); }


        /** Set Time.
@param P_Time Time */
public void SetP_Time (DateTime? P_Time){Set_Value ("P_Time", (DateTime?)P_Time);}/** Get Time.
@return Time */
public DateTime? GetP_Time() {return (DateTime?)Get_Value("P_Time");}/** Set Time To.
@param P_Time_To Time To */
public void SetP_Time_To (DateTime? P_Time_To){Set_Value ("P_Time_To", (DateTime?)P_Time_To);}/** Get Time To.
@return Time To */
public DateTime? GetP_Time_To() {return (DateTime?)Get_Value("P_Time_To");}


        /** Set Process Parameter.
@param AD_Process_Para_ID Process Parameter */
public void SetAD_Process_Para_ID (int AD_Process_Para_ID){if (AD_Process_Para_ID <= 0) Set_Value ("AD_Process_Para_ID", null);else
Set_Value ("AD_Process_Para_ID", AD_Process_Para_ID);}/** Get Process Parameter.
@return Process Parameter */
public int GetAD_Process_Para_ID() {Object ii = Get_Value("AD_Process_Para_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}



    }

}

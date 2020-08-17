using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.ProcessEngine
{
    [Serializable]
    public class ProcessInfoLog
    {
        public ProcessInfoLog(int P_ID, DateTime? P_Date, Decimal? P_Number, String P_Msg)
            : this(s_Log_ID++, P_ID, P_Date, P_Number, P_Msg)
        {
        }	//	ProcessInfoLog

        public ProcessInfoLog(int Log_ID, int? P_ID, DateTime? P_Date, Decimal? P_Number, String P_Msg)
        {
            SetLog_ID(Log_ID);
            SetP_ID(P_ID);
            SetP_Date(P_Date);
            SetP_Number(P_Number);
            SetP_Msg(P_Msg);
        }	//	ProcessInfoLog

        private static int s_Log_ID = 0;

        private int _Log_ID;
        private int? _P_ID;
        private DateTime? _P_Date;
        private Decimal? _P_Number;
        private String _P_Msg;

        public int GetLog_ID()
        {
            return _Log_ID;
        }

        public void SetLog_ID(int Log_ID)
        {
            _Log_ID = Log_ID;
        }

        public int? GetP_ID()
        {
            return _P_ID;
        }

        public void SetP_ID(int? P_ID)
        {
            _P_ID = P_ID;
        }

        public DateTime? GetP_Date()
        {
            return _P_Date;
        }

        public void SetP_Date(DateTime? P_Date)
        {
            _P_Date = P_Date;
        }

        public Decimal? GetP_Number()
        {
            return _P_Number;
        }

        public void SetP_Number(Decimal? P_Number)
        {
            _P_Number = P_Number;
        }

        public String GetP_Msg()
        {
            return _P_Msg;
        }

        public void SetP_Msg(String P_Msg)
        {
            _P_Msg = P_Msg;
        }
    }
}

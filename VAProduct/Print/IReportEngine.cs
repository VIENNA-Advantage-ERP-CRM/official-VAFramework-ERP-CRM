using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Print
{
    public interface IReportEngine
    {
        byte[] GetReportBytes();
        String GetReportString();
        string GetReportFilePath(bool fetchByteArr, out byte[] bytes);
        string GetCsvReportFilePath(string data);
        string GetRtfReportFilePath(string data);

        bool StartReport(Utility.Ctx ctx, ProcessEngine.ProcessInfo pi, DataBase.Trx trx);
    }

    public interface IReportView
    {
        View GetView();
        MPrintFormat GetPrintFormat();
        void SetPrintFormat(MPrintFormat pf);
    }
}

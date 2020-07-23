using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Print;
using System.Reflection;
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;

namespace VAdvantage.ReportFormat
{
    public class ReportFormatEngine
    {

        public static IReportEngine Get(Utility.Ctx p_ctx, ProcessEngine.ProcessInfo _pi, out int totalRecords, bool IsArabicReportFromOutside)
        {
            IReportEngine re = null;

            Type type = null;

            try
            {
                if (_pi.GetRecIds().Length > 0)
                {
                    Assembly asm = Assembly.Load("VARCOMSvc");
                    type = asm.GetType("ViennaAdvantage.Classes.ReportFromatWrapper");
                    ConstructorInfo cinfo = type.GetConstructor(new Type[] { typeof(Ctx), typeof(string), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(string) });
                    re = (IReportEngine)cinfo.Invoke(new object[] { p_ctx, _pi.GetTitle(), _pi.GetAD_Process_ID(), _pi.GetTable_ID(), _pi.GetRecord_ID(), 0, 0, _pi.GetAD_PInstance_ID(), _pi.GetRecIds() });

                    MethodInfo mInfo = type.GetMethod("Init");
                    totalRecords = Convert.ToInt32(mInfo.Invoke(re, new object[] { IsArabicReportFromOutside }));
                }
                else
                {


                    Assembly asm = Assembly.Load("VARCOMSvc");
                    type = asm.GetType("ViennaAdvantage.Classes.ReportFromatWrapper");
                    ConstructorInfo cinfo = type.GetConstructor(new Type[] { typeof(Ctx), typeof(string), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) });
                    re = (IReportEngine)cinfo.Invoke(new object[] { p_ctx, _pi.GetTitle(), _pi.GetAD_Process_ID(), _pi.GetTable_ID(), _pi.GetRecord_ID(), 0, 0, _pi.GetAD_PInstance_ID() });

                    MethodInfo mInfo = type.GetMethod("Init");
                    totalRecords = Convert.ToInt32(mInfo.Invoke(re, new object[] { IsArabicReportFromOutside }));
                }
            }
            catch
            {
                totalRecords = 0;
            }

            return re;

        }

        internal static IReportEngine Get(Utility.Ctx p_ctx, ProcessEngine.ProcessInfo _pi, bool IsArabicReportFromOutside)
        {
            int i = 0;
            return Get(p_ctx, _pi, out i, IsArabicReportFromOutside);
        }



    }
}

namespace VAdvanatge.Report
{



    public class ReportEngine
    {

        public static IReportEngine GetReportEngine(Ctx ctx, ProcessInfo pi, VAdvantage.DataBase.Trx trx, String assemblyName, String fqClassname)
        {
            IReportEngine re = null;

            //1 load assembly
            Type type = VAdvantage.Utility.ClassTypeContainer.GetClassType(fqClassname, assemblyName);
            if (type != null && type.IsClass)
            {
                re = (IReportEngine)Activator.CreateInstance(type);

            }
            else
            {
                pi.SetSummary("Could not find class " + fqClassname);
            }
            if (re != null)
            {
                //oClass.StartProcess(_ctx, _pi, _trx);
                if (!re.StartReport(ctx, pi, trx))
                    re = null;
            }
            return re;
        }
    }
}

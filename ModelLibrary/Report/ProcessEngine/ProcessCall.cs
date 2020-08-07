/********************************************************
 * Module Name    : Framework (Application Dictionary)
 * Purpose        : Difines the process function
 * Author         : Jagmohan Bhatt
 * Date           : 13-Jan-2009
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.ProcessEngine
{
    /// <summary>
    /// Interface to define function used in process
    /// </summary>
    public interface ProcessCall
    {
        bool StartProcess(Ctx ctx, ProcessInfo pi, Trx trx);
    }
}

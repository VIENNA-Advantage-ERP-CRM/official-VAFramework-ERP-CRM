/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ViennaProcessorLog
 * Purpose        : Processor Log Interface 
 * Class Used     : --
 * Chronological    Development
 * Raghunandan      04-May-2009 
  ******************************************************/
using System;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Data;
using VAdvantage.SqlExec;

namespace VAdvantage.Model
{
    public interface ViennaProcessorLog
    {
        /// <summary>
        /// Get Created Date
        /// </summary>
        /// <returns>created</returns>
        DateTime GetCreated();

        /// <summary>
        ///Get Summary. Textual summary of this request
        /// </summary>
        /// <returns>Summary</returns>
        String GetSummary();

        /// <summary>
        ///Get Description. Optional short description of the record
        /// </summary>
        /// <returns>description</returns>
        String GetDescription();

        /// <summary>
        ///Get Error. An Error occured in the execution
        /// </summary>
        /// <returns>true if error</returns>
        bool IsError();

        /// <summary>
        ///Get Reference. Reference for this record
        /// </summary>
        /// <returns>reference</returns>
        String GetReference();

        /// <summary>
        ///Get Text Message. Text Message
        /// </summary>
        /// <returns>text message</returns>
        String GetTextMsg();
    }
}

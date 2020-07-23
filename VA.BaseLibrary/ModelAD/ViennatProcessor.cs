/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ViennaProcessor
 * Purpose        : Processor interface 
 * Class Used     : --
 * Chronological    Development
 * Raghunandan      04-May-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.SqlExec;

namespace VAdvantage.Model
{
    public interface ViennaProcessor
    {
        /// <summary>
        ///Get Client
        /// </summary>
        /// <returns>AD_Client_ID</returns>
         int GetAD_Client_ID();

        /// <summary>
        ///	Get Name
        /// </summary>
        /// <returns>Name</returns>
         String GetName();

        /// <summary>
        ///Get Description
        /// </summary>
        /// <returns>Description</returns>
         String GetDescription();

        /// <summary>
        ///Get Context
        /// </summary>
        /// <returns>context</returns>
         Utility.Ctx GetCtx();

        /// <summary>
        ///Get the frequency type
        /// </summary>
        /// <returns>frequency type</returns>
         String GetFrequencyType();

        /// <summary>
        ///	Get the frequency
        /// </summary>
        /// <returns>frequency</returns>
         int GetFrequency();

        /// <summary>
        ///Get AD_Schedule_ID
        /// </summary>
        /// <returns>schedule</returns>
         int GetAD_Schedule_ID();

        /// <summary>
        ///Get Unique ID
        /// </summary>
        /// <returns>Unique ID</returns>
         String GetServerID();

        /// <summary>
        ///Get the date Next run
        /// </summary>
        /// <param name="requery">requery database</param>
        /// <returns>date next run</returns>
         DateTime? GetDateNextRun(bool requery);

        /// <summary>
        ///Set Date Next Run
        /// </summary>
        /// <param name="now">dateNextWork next work</param>
          void SetDateNextRun(DateTime? now);

        /// <summary>
        ///Get the date Last run
        /// </summary>
        /// <returns>date lext run</returns>
          DateTime? GetDateLastRun();

        /// <summary>
        ///Set Date Last Run
        /// </summary>
        /// <param name="dateLastRun">dateLastRun last run</param>
          void SetDateLastRun(DateTime? dateLastRun);

        /// <summary>
        ///Save
        /// </summary>
        /// <returns>true if saved</returns>
         bool Save();


        /// <summary>
        ///Get Processor Logs
        /// </summary>
        /// <returns>logs</returns>
         ViennaProcessorLog[] GetLogs();
	
    }
}

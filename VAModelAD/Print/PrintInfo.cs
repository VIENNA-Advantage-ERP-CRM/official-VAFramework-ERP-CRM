/********************************************************
 * Module Name    :     Report
 * Purpose        :     Generate Reports
 * Author         :     Jagmohan Bhatt
 * Date           :     25-June-2009
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.ProcessEngine;
namespace VAdvantage.Print
{
    /// <summary>
    /// Print Info
    /// </summary>
    public class PrintInfo
    {
        /// <summary>
        /// Process Archive Info
        /// </summary>
        /// <param name="pi">process info</param>
        public PrintInfo(ProcessInfo pi)
        {
            SetName(pi.GetTitle());
            SetVAF_Job_ID(pi.GetVAF_Job_ID());
            SetVAF_TableView_ID(pi.GetTable_ID());
            SetRecord_ID(pi.GetRecord_ID());
            SetVAF_JInstance_ID(pi.GetVAF_JInstance_ID());
        }	//	PrintInfo


        /// <summary>
        /// Document Archive Info
        /// </summary>
        /// <param name="Name">name</param>
        /// <param name="VAF_TableView_ID">table id</param>
        /// <param name="Record_ID">record id</param>
        /// <param name="VAB_BusinessPartner_ID">BPartner ID</param>
        public PrintInfo(String Name, int VAF_TableView_ID, int Record_ID, int VAB_BusinessPartner_ID)
        {
            SetName(Name);
            SetVAF_TableView_ID(VAF_TableView_ID);
            SetRecord_ID(Record_ID);
            SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
        }	//	ArchiveInfo


        /// <summary>
        /// Report Archive Info
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="VAF_TableView_ID"></param>
        /// <param name="Record_ID"></param>
        public PrintInfo(String Name, int VAF_TableView_ID, int Record_ID)
        {
            SetName(Name);
            SetVAF_TableView_ID(VAF_TableView_ID);
            SetRecord_ID(Record_ID);
        }	//	ArchiveInfo

        bool _withDialog = false;
        private int _copies = 1;
        private bool _isDocumentCopy = false;
        private String _printerName = null;
        //
        private String _Name = null;
        private String _Description = null;
        private String _Help = null;
        private int _VAF_Job_ID = 0;
        private int _VAF_TableView_ID = 0;
        private int _Record_ID = 0;
        private int _VAB_BusinessPartner_ID = 0;
        private int _VAF_JInstance_ID = 0;


        /// <summary>
        /// Is this a Report
        /// </summary>
        /// <returns>true if report</returns>
        public bool IsReport()
        {
            return _VAF_Job_ID != 0	//	Menu Report
                || _VAB_BusinessPartner_ID == 0;
        }	//	isReport

        /// <summary>
        /// Is this a Document
        /// </summary>
        /// <returns></returns>
        public bool IsDocument()
        {
            return _VAB_BusinessPartner_ID != 0;
        }	//	isDocument


        /// <summary>
        /// Get Copies
        /// </summary>
        /// <returns>Returns the copies.</returns>
        public int GetCopies()
        {
            return _copies;
        }

        /// <summary>
        /// The copies to set.
        /// </summary>
        /// <param name="copies">The copies to set.</param>
        public void SetCopies(int copies)
        {
            _copies = copies;
        }

        /// <summary>
        /// Get printer name
        /// </summary>
        /// <returns>the printerName.</returns>
        public String GetPrinterName()
        {
            return _printerName;
        }

        /// <summary>
        /// Sets the printer name
        /// </summary>
        /// <param name="printerName">printer name</param>
        public void SetPrinterName(String printerName)
        {
            _printerName = printerName;
        }

        /// <summary>
        /// Is withDialog
        /// </summary>
        /// <returns>return with dialog</returns>
        public bool IsWithDialog()
        {
            return _withDialog;
        }

        /// <summary>
        /// set withDialog
        /// </summary>
        /// <param name="withDialog">The withDialog to set.</param>
        public void SetWithDialog(bool withDialog)
        {
            _withDialog = withDialog;
        }


        /// <summary>
        /// Set Document copy
        /// </summary>
        /// <param name="isDocumentCopy">true if it is doucment copy</param>
        public void SetDocumentCopy(bool isDocumentCopy)
        {
            _isDocumentCopy = isDocumentCopy;
        }


        /// <summary>
        /// Check if document copy
        /// </summary>
        /// <returns>true or false</returns>
        public bool IsDocumentCopy()
        {
            return _isDocumentCopy;
        }	//	isDocument


        /// <summary>
        /// Gets the processID
        /// </summary>
        /// <returns>Process ID</returns>
        public int GetVAF_Job_ID()
        {
            return _VAF_Job_ID;
        }


        /// <summary>
        /// Set Process ID
        /// </summary>
        /// <param name="process_ID"></param>
        public void SetVAF_Job_ID(int process_ID)
        {
            _VAF_Job_ID = process_ID;
        }

        /// <summary>
        /// Get Table ID
        /// </summary>
        /// <returns>Table ID</returns>
        public int GetVAF_TableView_ID()
        {
            return _VAF_TableView_ID;
        }

        /// <summary>
        /// Set Table ID
        /// </summary>
        /// <param name="table_ID"></param>
        public void SetVAF_TableView_ID(int table_ID)
        {
            _VAF_TableView_ID = table_ID;
        }

        /// <summary>
        /// Get B Partner ID
        /// </summary>
        /// <returns></returns>
        public int GetVAB_BusinessPartner_ID()
        {
            return _VAB_BusinessPartner_ID;
        }

        /// <summary>
        /// Set VAB_BusinessPartner_ID
        /// </summary>
        /// <param name="partner_ID">partner ID</param>
        public void SetVAB_BusinessPartner_ID(int partner_ID)
        {
            _VAB_BusinessPartner_ID = partner_ID;
        }



        /// <summary>
        /// Get description
        /// </summary>
        /// <returns>description</returns>
        public String GetDescription()
        {
            return _Description;
        }


        /// <summary>
        /// Sets description
        /// </summary>
        /// <param name="description">description text</param>
        public void SetDescription(String description)
        {
            _Description = description;
        }

        /// <summary>
        /// Get Help
        /// </summary>
        /// <returns>help text</returns>
        public String GetHelp()
        {
            return _Help;
        }
        /**
         * @param help The help to set.
         */
        public void setHelp(String help)
        {
            _Help = help;
        }


        /// <summary>
        /// Get Name
        /// </summary>
        /// <returns>Name</returns>
        public String GetName()
        {
            if (_Name == null || _Name.Length == 0)
                return "Unknown";
            return _Name;
        }


        /// <summary>
        /// Set Name
        /// </summary>
        /// <param name="name">Name text</param>
        public void SetName(String name)
        {
            _Name = name;
        }

        /// <summary>
        /// Get Record ID
        /// </summary>
        /// <returns>Record ID</returns>
        public int GetRecord_ID()
        {
            return _Record_ID;
        }

        /// <summary>
        /// Set Record ID
        /// </summary>
        /// <param name="record_ID">Record ID</param>
        public void SetRecord_ID(int record_ID)
        {
            _Record_ID = record_ID;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("PrintInfo[");
            sb.Append(GetName());
            if (GetVAF_Job_ID() != 0)
                sb.Append(",VAF_Job_ID=").Append(GetVAF_Job_ID());
            if (GetVAF_TableView_ID() != 0)
                sb.Append(",VAF_TableView_ID=").Append(GetVAF_TableView_ID());
            if (GetRecord_ID() != 0)
                sb.Append(",Record_ID=").Append(GetRecord_ID());
            if (GetVAB_BusinessPartner_ID() != 0)
                sb.Append(",VAB_BusinessPartner_ID=").Append(GetVAB_BusinessPartner_ID());

            sb.Append("]");
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Get Record ID
        /// </summary>
        /// <returns>Record ID</returns>
        public int GetVAF_JInstance_ID()
        {
            return _VAF_JInstance_ID;
        }

        /// <summary>
        /// Set Record ID
        /// </summary>
        /// <param name="record_ID">Record ID</param>
        public void SetVAF_JInstance_ID(int VAF_JInstance_ID)
        {
            _VAF_JInstance_ID = VAF_JInstance_ID;
        }

    }
}

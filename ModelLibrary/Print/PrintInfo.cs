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
            SetAD_Process_ID(pi.GetAD_Process_ID());
            SetAD_Table_ID(pi.GetTable_ID());
            SetRecord_ID(pi.GetRecord_ID());
            SetAD_PInstance_ID(pi.GetAD_PInstance_ID());
        }	//	PrintInfo


        /// <summary>
        /// Document Archive Info
        /// </summary>
        /// <param name="Name">name</param>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="Record_ID">record id</param>
        /// <param name="C_BPartner_ID">BPartner ID</param>
        public PrintInfo(String Name, int AD_Table_ID, int Record_ID, int C_BPartner_ID)
        {
            SetName(Name);
            SetAD_Table_ID(AD_Table_ID);
            SetRecord_ID(Record_ID);
            SetC_BPartner_ID(C_BPartner_ID);
        }	//	ArchiveInfo


        /// <summary>
        /// Report Archive Info
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="Record_ID"></param>
        public PrintInfo(String Name, int AD_Table_ID, int Record_ID)
        {
            SetName(Name);
            SetAD_Table_ID(AD_Table_ID);
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
        private int _AD_Process_ID = 0;
        private int _AD_Table_ID = 0;
        private int _Record_ID = 0;
        private int _C_BPartner_ID = 0;
        private int _AD_PInstance_ID = 0;


        /// <summary>
        /// Is this a Report
        /// </summary>
        /// <returns>true if report</returns>
        public bool IsReport()
        {
            return _AD_Process_ID != 0	//	Menu Report
                || _C_BPartner_ID == 0;
        }	//	isReport

        /// <summary>
        /// Is this a Document
        /// </summary>
        /// <returns></returns>
        public bool IsDocument()
        {
            return _C_BPartner_ID != 0;
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
        public int GetAD_Process_ID()
        {
            return _AD_Process_ID;
        }


        /// <summary>
        /// Set Process ID
        /// </summary>
        /// <param name="process_ID"></param>
        public void SetAD_Process_ID(int process_ID)
        {
            _AD_Process_ID = process_ID;
        }

        /// <summary>
        /// Get Table ID
        /// </summary>
        /// <returns>Table ID</returns>
        public int GetAD_Table_ID()
        {
            return _AD_Table_ID;
        }

        /// <summary>
        /// Set Table ID
        /// </summary>
        /// <param name="table_ID"></param>
        public void SetAD_Table_ID(int table_ID)
        {
            _AD_Table_ID = table_ID;
        }

        /// <summary>
        /// Get B Partner ID
        /// </summary>
        /// <returns></returns>
        public int GetC_BPartner_ID()
        {
            return _C_BPartner_ID;
        }

        /// <summary>
        /// Set C_BPartner_ID
        /// </summary>
        /// <param name="partner_ID">partner ID</param>
        public void SetC_BPartner_ID(int partner_ID)
        {
            _C_BPartner_ID = partner_ID;
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
            if (GetAD_Process_ID() != 0)
                sb.Append(",AD_Process_ID=").Append(GetAD_Process_ID());
            if (GetAD_Table_ID() != 0)
                sb.Append(",AD_Table_ID=").Append(GetAD_Table_ID());
            if (GetRecord_ID() != 0)
                sb.Append(",Record_ID=").Append(GetRecord_ID());
            if (GetC_BPartner_ID() != 0)
                sb.Append(",C_BPartner_ID=").Append(GetC_BPartner_ID());

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
        public int GetAD_PInstance_ID()
        {
            return _AD_PInstance_ID;
        }

        /// <summary>
        /// Set Record ID
        /// </summary>
        /// <param name="record_ID">Record ID</param>
        public void SetAD_PInstance_ID(int AD_PInstance_ID)
        {
            _AD_PInstance_ID = AD_PInstance_ID;
        }

    }
}

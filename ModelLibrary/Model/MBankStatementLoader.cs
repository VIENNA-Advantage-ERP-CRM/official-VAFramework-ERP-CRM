/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MBankStatementLoader
 * Purpose        : MBankStatement Loader  Model
 * Class Used     : X_C_BankStatementLoader
 * Chronological    Development
 * Deepak           03-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
//using VAdvantage.ImpExp;
namespace VAdvantage.Model
{
    public class MBankStatementLoader : X_C_BankStatementLoader
    {
        /**	Number of statement lines imported			*/
        private int loadCount = 0;

        /**	Message will be handled by Vienna (e.g. translated)	*/
        private String errorMessage = "";

        /**	Additional error description				*/
        private String errorDescription = "";

        /**	Loader object to use 					*/
        private BankStatementLoaderInterface _loader = null;

        /**	File name from process parameter							*/
        private String localFileName = null;



        /// <summary>
        /// Create a Statement Loader Added for compatibility with new PO infrastructure (bug# 968136)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_BankStatementLoader_ID">loader to use</param>
        /// <param name="trxName">trx</param>
        public MBankStatementLoader(Ctx ctx, int C_BankStatementLoader_ID, Trx trxName)
            : base(ctx, C_BankStatementLoader_ID, trxName)
        {
            Init(null);
        }	//	MBankStatementLoader

        /// <summary>
        /// Create a Statement Loader
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_BankStatementLoader_ID">loader to use</param>
        /// <param name="trxName">trx</param>
        public MBankStatementLoader(Ctx ctx, int C_BankStatementLoader_ID, String fileName, Trx trxName)
            : base(ctx, C_BankStatementLoader_ID, trxName)
        {

            Init(fileName);

        }	//	MBankStatementLoader

        /// <summary>
        /// Create a Statement Loader
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MBankStatementLoader(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

            Init(null);
        }	//	MBankStatementLoader

        private void Init(String fileName)
        {
            localFileName = fileName;
            try
            {
                log.Info("MBankStatementLoader Class Name=" + GetStmtLoaderClass());
                //Class bsrClass = Class.forName(getStmtLoaderClass());
                Type bsrClass = Type.GetType(GetStmtLoaderClass());// Class.forName(getStmtLoaderClass());
                _loader = (BankStatementLoaderInterface)Activator.CreateInstance(bsrClass);// bsrClass.newInstance();
            }
            catch (Exception e)
            {
                errorMessage = "ClassNotLoaded";
                errorDescription = e.Message;
            }
        }

        /// <summary>
        /// Return Name
        /// </summary>
        /// <returns>Name</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MBankStatementLoader[")
                .Append(Get_ID()).Append("-").Append(GetName())
                .Append("]");
            return sb.ToString();
        }	//	toString

        /// <summary>
        /// Return Local File Name
        /// </summary>
        /// <returns>Name</returns>
        public String GetLocalFileName()
        {
            return localFileName;
        }	//	getLocalFileName

        /// <summary>
        /// Start loading Bankstatements
        /// </summary>
        /// <returns>true if loading completed succesfully</returns>
        public bool LoadLines()
        {
            bool result = false;
            log.Info("MBankStatementLoader.loadLines");
            if (_loader == null)
            {
                errorMessage = "ClassNotLoaded";
                return result;
            }
            //	Initialize the Loader 
            if (!_loader.Init(this))
            {
                errorMessage = _loader.GetLastErrorMessage();
                errorDescription = _loader.GetLastErrorDescription();
                return result;
            }
            //	Verify whether the data structure is valid
            if (!_loader.IsValid())
            {
                errorMessage = _loader.GetLastErrorMessage();
                errorDescription = _loader.GetLastErrorDescription();
                return result;
            }
            //	Load statement lines
            if (!_loader.LoadLines())
            {
                errorMessage = _loader.GetLastErrorMessage();
                errorDescription = _loader.GetLastErrorDescription();
                return result;
            }
            result = true;
            return result;

        }	//	loadLines

        /// <summary>
        /// Load a bank statement into the I_BankStatement table
        /// </summary>
        /// <returns>Statement line was loaded succesfully</returns>
        /// This method is called by the BankStatementLoadere whenever a complete 
        ///statement line has been read.

        public bool SaveLine()
        {
            log.Info("MBankStatementLoader.importLine");
            bool result = false;
            X_I_BankStatement imp = new X_I_BankStatement(GetCtx(), 0, Get_TrxName());
            if (_loader == null)
            {
                errorMessage = "LoadError";
                return result;
            }
            //	Bank Account fields
            log.Config("MBankStatementLoader.importLine Bank Account=" + _loader.GetBankAccountNo());
            imp.SetBankAccountNo(_loader.GetBankAccountNo());
            log.Config("MBankStatementLoader.importLine Routing No=" + _loader.GetRoutingNo());
            imp.SetRoutingNo(_loader.GetRoutingNo());

            //	Statement fields
            log.Config("MBankStatementLoader.importLine EFT Statement Reference No=" + _loader.GetStatementReference());
            imp.SetEftStatementReference(_loader.GetStatementReference());
            log.Config("MBankStatementLoader.importLine EFT Statement Date=" + _loader.GetStatementDate());
            imp.SetEftStatementDate(_loader.GetStatementDate());
            log.Config("MBankStatementLoader.importLine Statement Date=" + _loader.GetStatementDate());
            imp.SetStatementDate(_loader.GetStatementDate());

            //	Statement Line fields
            log.Config("MBankStatementLoader.importLine EFT Transaction ID=" + _loader.GetTrxID());
            imp.SetEftTrxID(_loader.GetTrxID());
            log.Config("MBankStatementLoader.importLine Statement Line Date=" + _loader.GetStatementLineDate());
            imp.SetStatementLineDate(_loader.GetStatementLineDate());
            imp.SetStatementLineDate(_loader.GetStatementLineDate());
            imp.SetEftStatementLineDate(_loader.GetStatementLineDate());
            log.Config("MBankStatementLoader.importLine Valuta Date=" + _loader.GetValutaDate());
            imp.SetValutaDate(_loader.GetValutaDate());
            imp.SetEftValutaDate(_loader.GetValutaDate());
            log.Config("MBankStatementLoader.importLine Statement Amount=" + _loader.GetStmtAmt());
            imp.SetStmtAmt(_loader.GetStmtAmt());
            imp.SetEftAmt(_loader.GetStmtAmt());
            log.Config("MBankStatementLoader.importLine Transaction Amount=" + _loader.GetTrxAmt());
            imp.SetTrxAmt(_loader.GetTrxAmt());
            log.Config("MBankStatementLoader.importLine Interest Amount=" + _loader.GetInterestAmt());
            imp.SetInterestAmt(_loader.GetInterestAmt());
            log.Config("MBankStatementLoader.importLine Reference No=" + _loader.GetReference());
            imp.SetReferenceNo(_loader.GetReference());
            imp.SetEftReference(_loader.GetReference());
            log.Config("MBankStatementLoader.importLine Check No=" + _loader.GetReference());
            imp.SetEftCheckNo(_loader.GetCheckNo());
            log.Config("MBankStatementLoader.importLine Memo=" + _loader.GetMemo());
            imp.SetMemo(_loader.GetMemo());
            imp.SetEftMemo(_loader.GetMemo());
            log.Config("MBankStatementLoader.importLine Payee Name=" + _loader.GetPayeeName());
            imp.SetEftPayee(_loader.GetPayeeName());
            log.Config("MBankStatementLoader.importLine Payee Account No=" + _loader.GetPayeeAccountNo());
            imp.SetEftPayeeAccount(_loader.GetPayeeAccountNo());
            log.Config("MBankStatementLoader.importLine EFT Transaction Type=" + _loader.GetTrxType());
            imp.SetEftTrxType(_loader.GetTrxType());
            log.Config("MBankStatementLoader.importLine Currency=" + _loader.GetCurrency());
            imp.SetEftCurrency(_loader.GetCurrency());
            imp.SetISO_Code(_loader.GetCurrency());
            log.Config("MBankStatementLoader.importLine Charge Name=" + _loader.GetChargeName());
            imp.SetChargeName(_loader.GetChargeName());
            log.Config("MBankStatementLoader.importLine Charge Amount=" + _loader.GetChargeAmt());
            imp.SetChargeAmt(_loader.GetChargeAmt());
            imp.SetProcessed(false);
            imp.SetI_IsImported(X_I_BankStatement.I_ISIMPORTED_No);

            result = imp.Save();
            if (result)
            {
                loadCount++;
            }
            else
            {
                errorMessage = "LoadError";
            }
            imp = null;
            return result;
        }	//	importLine

        /// <summary>
        /// Return the most recent error
        //	This error message will be handled as a Vienna message,
        //	(e.g. it can be translated)
        /// </summary>
        /// <returns>message</returns>
        public String GetErrorMessage()
        {
            return errorMessage;
        }	//	getErrorMessage

        /// <summary>
        /// Return the most recent error description	 
        //This is an additional error description, it can be used to provided
        //descriptive iformation, such as a file name or SQL error, that can not
        //be translated by the Vienna message system.
        /// </summary>
        /// <returns>Error discription</returns>
        public String GetErrorDescription()
        {
            return errorDescription;
        }	//	getErrorDescription

        /// <summary>
        /// The total number of statement lines loaded
        /// </summary>
        /// <returns>Number of imported statement lines</returns>
        public int GetLoadCount()
        {
            return loadCount;
        }	//	getLoadCount


    }	//MBankStatementLoader


 
}

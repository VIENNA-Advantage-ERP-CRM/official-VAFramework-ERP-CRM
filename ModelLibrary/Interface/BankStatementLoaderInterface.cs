/********************************************************
 * Project Name   : VAdvantage
 * Interface Name : BankStatementLoaderInterface
 * Purpose        : Interface to be implemented by bank statement loader classes
 * Class Used     :  ..
 * Chronological    Development
 * Deepak           03-Feb-2010
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;

namespace VAdvantage.Interface
{
    public interface BankStatementLoaderInterface
    {
        /// <summary>
        ///Initialize the loader
        /// </summary>
        /// <param name="controller">controller Reference to the MBankStatementLoader controller object</param>
        /// <returns>Initialized succesfully</returns>
        bool Init(MBankStatementLoader controller);
       
        /// <summary>
        /// Verify whether the data to be imported is valid      
        // If the actual loaders does not do any validity checks
        //it will just return true.      
        /// </summary>
        /// <returns>Data is valid</returns>
        bool IsValid();

        /// <summary>
        ///Start importing statement lines
        /// </summary>
        /// <returns>Statement lines imported succesfully</returns>
        bool LoadLines();

        /// <summary>
        ///Return the most recent error     
        //This error message will be handled as a Vienna message,
        //(e.g. it can be translated)
        /// </summary>
        /// <returns>Error message</returns>
        String GetLastErrorMessage();

        /// <summary>
        /// Return the most recent error description      
        // This is an additional error description, it can be used to provided
        // descriptive iformation, such as a file name or SQL error, that can not
        // be translated by the Vienna message system.
        /// </summary>
        /// <returns>Error discription</returns>
        String GetLastErrorDescription();

        /// <summary>
        ///The last time this loader aquired bank statement data.
        // For OFX this is the <DTEND> value. This is generally only available\
        // after loadLines() has been called. If a specific loader class 
        // does not provided this information it is allowed to return null
        /// </summary>
        /// <returns> Date last run</returns>
        DateTime? GetDateLastRun();

        /// <summary>
        ///The routing number of the bank account for the statement line.
        /// </summary>
        /// <returns>Routing number</returns>
        String GetRoutingNo();

        /// <summary>
        ///The account number of the bank account for the statement line.
        /// </summary>
        /// <returns>Bank account number</returns>
        String GetBankAccountNo();

        /// <summary>
        ///Additional reference information      
        // Statement level reference information. If a specific loader class
        // does not provided this, it is allowed to return null.     
        /// </summary>
        /// <returns>Error discription</returns>
        String GetStatementReference();

        /// <summary>
        ///Statement Date
        //Date of the bank statement. If a specific loader does not provide this, 
        // it is allowed to return null.
        /// </summary>
        /// <returns>Statement Date</returns>
        DateTime? GetStatementDate();

        /// <summary>
        ///Transaction ID assigned by the bank.
        // For OFX this is the <FITID>
        // If a specific loader does not provide this, it is allowed to return
        //null.        
        /// </summary>
        /// <returns>Transaction ID</returns>
        String GetTrxID();

        /// <summary>
        ///Additional reference information      
        //Statement line level reference information.
        //For OFX this is the <REFNUM> field.
        //If a specific loader does not provided this, it is allowed to return null.     
        /// </summary>
        /// <returns>Error discription</returns>
        String GetReference();

        /// <summary>
        ///Check number      
        // Check number, in case the transaction was initiated by a check.
        // For OFX this is the <CHECKNUM> field, for MS-Money (OFC) this is the
        // <CHKNUM> field.
        // If a specific loader does not provide this, it is allowed to return null.
        /// </summary>
        /// <returns>Transaction reference</returns>
        String GetCheckNo();

        /// <summary>
        ///Payee name 
        // Name information, for OFX this is the <NAME> or
        // <PAYEE><NAME> field	
        // If a specific loader class does not provide this, it is allowed
        // to return null.
        /// </summary>
        /// <returns>Payee name</returns>
        String GetPayeeName();

        /// <summary>
        ///Payee account 
        // Account information of "the other party"
        // If a specific loader class does not provide this, it is allowed
        // to return null.
        /// </summary>
        /// <returns>Payee bank account number</returns>
        String GetPayeeAccountNo();

        /// <summary>
        /// Statement line date
        /// This has to be provided by all loader Classes.
        /// </summary>
        /// <returns>Statement line date</returns>
        DateTime? GetStatementLineDate();

        /// <summary>
        ///Effective date
        // Date theat the funds became available.
        // If a specific loader does not provide this, it is allowed to return null.      
        /// </summary>
        /// <returns>Effective date</returns>
        DateTime? GetValutaDate();

        /// <summary>
        /// Transaction type
        // This returns the transaction type as used by the bank
        // Whether a transaction is credit or debit depends on the amount (i.e. negative),
        // this field is for reference only.
        // If a specific loader class does not provide this, it is allowed
        // to return null.
        /// </summary>
        /// <returns> Transaction type</returns>
        String GetTrxType();

        /// <summary>
        ///Indicates whether this transaction is a reversal
        /// </summary>
        /// <returns>true if this is a reversal</returns>
        bool GetIsReversal();

        /// <summary>
        /// Currency    
        // Return the currency, if included in the statement data.
        // It is returned as it appears in the import data, it should
        // not be processed by the loader in any way.
        // If a specific loader class does not provide this, it is allowed
        // to return null.
        /// </summary>
        /// <returns>Currency</returns>
        String GetCurrency();

        /// <summary>
        ///Statement line amount
        // This has to be provided by all loader Classes.
        /// </summary>
        /// <returns>Statement Line Amount</returns>
        Decimal GetStmtAmt();

        /// <summary>
        /// Transaction Amount
        /// </summary>
        /// <returns>Transaction Amount</returns>
        Decimal GetTrxAmt();

        /// <summary>
        /// Interest Amount
        /// </summary>
        /// <returns>Interest Amount</returns>
        Decimal GetInterestAmt();

        /// <summary>
        ///Transaction memo
        // Additional descriptive information.
        // For OFX this is the <MEMO> filed, for SWIFT MT940
        // this is the "86" line.
        // If a specific loader does not provide this, it is allowed to return null.
        /// </summary>
        /// <returns>Memo</returns>
        String GetMemo();

        /// <summary>
        /// Charge name
        // Name of the charge, in case this transaction is a bank charge.
        // If a specific loader class does not provide this, it is allowed
        // to return null.
        /// </summary>
        /// <returns>Charge name</returns>
        String GetChargeName();

        /// <summary>
        ///Charge amount
        // Name of the charge, in case this transaction is a bank charge.
        // If a specific loader class does not provide this, it is allowed
        // to return null.
        /// </summary>
        /// <returns>Charge amount</returns>
        Decimal GetChargeAmt();

    }	




}

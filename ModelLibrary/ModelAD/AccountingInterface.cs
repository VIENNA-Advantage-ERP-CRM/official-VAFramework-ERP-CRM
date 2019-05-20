/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : AccountingInterface
 * Purpose        : Accounting Interface for Base Document Types
 * Class Used     : --
 * Chronological    Development
 * Raghunandan      7-May-2009 
  ******************************************************/
//"Vienna/api/AccountingInterface.java"  shift into Acct floder
/******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace VAdvantage.Model
{
    public interface AccountingInterface
    {
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rs">accounting schemata</param>
        /// <param name="rs">trx</param>
        //	must implement - cannot enforce here
        	//public <init> (MAcctSchema[] ass, DataRow rs, string trxName);

        /// <summary>
        /// Load Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        String LoadDocumentDetails();

        /// <summary>
        ///Get Source Currency Balance - subtracts line (and tax) amounts from total - no rounding
        /// </summary>
        /// <returns>positive amount, if total header is bigger than lines</returns>
        Decimal GetBalance();

        /// <summary>
        /// Create Facts (the accounting logic)
        /// </summary>
        /// <param name="rs">as accounting schema</param>
        /// <returns>Facts</returns>
        //	made to comment as Fact is not a class in AD - must be implemented	
        //	public ArrayList<Fact> createFacts (MAcctSchema as);

    }
}

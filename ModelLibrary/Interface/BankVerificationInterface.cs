/********************************************************
 * Project Name   : VAdvantage
 * Inteface Name     : BankVerificationInterface
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     23-Jun-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace VAdvantage.Model
{
    public interface BankVerificationInterface
    {
        /*	Verify Routing No
        * 	@param VAB_Country_ID country
        *	@param RoutingNo Routing Number
        *	@return error message or null
        */
        String VerifyRoutingNo(int VAB_Country_ID, String RoutingNo);

        /**
         * 	Verify Swift Code or BIC
         *	@param SwiftCode Swift Code
         *	@return error message or null
         */
        String VerifySwiftCode(String SwiftCode);


        /**
         * 	Verify Basic Bank Account Number
         * 	@oaram bank the bank
         *	@param BBAN Basic Bank Account Number
         *	@return error message or null
         */
        String VerifyBBAN(MVABBank bank, String BBAN);

        /**
         * 	Verify International Bank Account Number
         * 	@oaram bank the bank
         *	@param IBAN International Bank Account Number
         *	@return error message or null
         */
        String VerifyIBAN(MVABBank bank, String IBAN);

        /**
         * 	Verify Account Number.
         * 	@oaram bank the bank
         *	@param AccountNo Bank Account Number
         *	@return error message or null
         */
        String VerifyAccountNo(MVABBank bank, String AccountNo);


        /**
         * 	Validate Credit Card Number.
         * 	(not used at the moment)
         *	@param creditCardNumber credit card number
         *	@param creditCardType credit card type X_VAB_Payment.CREDITCARDTYPE_
         *	@return error message or null
         */
        String ValidateCreditCardNumber(String creditCardNumber, String creditCardType);


    }
}

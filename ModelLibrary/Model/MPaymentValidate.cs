/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : 
 * Chronological Development
 * Veena Pandey     24-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    /// <summary>
    /// Payment Validion Routines
    /// </summary>
    public class MPaymentValidate
    {
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MPaymentValidate).FullName);

        /**
	     *  Is this a valid Credit Card Exp Date?
	     *	@param mmyy Exp in form of mmyy
	     *  @return "" or Error AD_Message
	     */
        public static String ValidateCreditCardExp(String mmyy)
        {
            String exp = CheckNumeric(mmyy);
            if (exp.Length != 4)
            {
                return "Credit Card Expiration Date Format must be MMYY";//Credit Card ExpFormat";
            }
            //
            String mmStr = exp.Substring(0, 2);
            String yyStr = exp.Substring(2, 2);
            //
            int mm = 0;
            int yy = 0;
            try
            {
                mm = int.Parse(mmStr);
                yy = int.Parse(yyStr);
            }
            catch 
            {
                return "Credit CardExp Format";
            }
            return ValidateCreditCardExp(mm, yy);
        }

        /**
         *  Return Month of Exp
         *  @param mmyy  Exp in form of mmyy
         *  @return month
         */
        public static int GetCreditCardExpMM(String mmyy)
        {
            String mmStr = mmyy.Substring(0, 2);
            int mm = 0;
            try
            {
                mm = int.Parse(mmStr);
            }
            catch 
            {
            }
            return mm;
        }

        /**
         *  Return Year of Exp
         *  @param mmyy  Exp in form of mmyy
         *  @return year
         */
        public static int GetCreditCardExpYY(String mmyy)
        {
            String yyStr = mmyy.Substring(2);
            int yy = 0;
            try
            {
                yy = int.Parse(yyStr);
            }
            catch 
            {
            }
            return yy;
        }

        /**
         *  Is this a valid Credit Card Exp Date?
         *  @param mm month
         *  @param yy year
         *  @return "" or Error AD_Message
         */
        public static String ValidateCreditCardExp(int mm, int yy)
        {
            if (mm < 1 || mm > 12)
                return "CreditCardExpMonth";
            //	if (yy < 0 || yy > EXP_YEAR)
            //		return "CreditCardExpYear";

            /*
            Calendar cal = Calendar.getInstance();
            int year = cal.get(Calendar.YEAR) - 2000;   //  two digits
            int month = cal.get(Calendar.MONTH) + 1;    //  zero based
            */

            //Today's date
            DateTime dtToday = DateTime.Today;
            int year = dtToday.Year - 2000;
            int month = dtToday.Month;
            //
            if (yy < year)
                return "CreditCardExpired";
            else if (yy == year && mm < month)
                return "CreditCardExpired";
            return "";
        }


        /**
         *  Validate Credit Card Number.
         *  - Based on LUHN formula
         *  @param creditCardNumber credit card number
         *  @return "" or Error AD_Message
         */
        public static String ValidateCreditCardNumber(String creditCardNumber)
        {
            if (creditCardNumber == null || creditCardNumber.Length == 0)
                return "Credit Card Number Error";

            /**
             *  1:  Double the value of alternate digits beginning with
             *      the	first right-hand digit (low order).
             *  2:  Add the individual digits comprising the products
             *      obtained in step 1 to each of the unaffected digits
             *      in the original number.
             *  3:  Subtract the total obtained in step 2 from the next higher
             *      number ending in 0 [this in the equivalent of calculating
             *      the "tens complement" of the low order digit (unit digit)
             *      of the total].
             *      If the total obtained in step 2 is a number ending in zero
             *      (30, 40 etc.), the check digit is 0.
             *  Example:
             *  Account number: 4992 73 9871 6
             *
             *  4  9  9  2  7  3  9  8  7  1  6
             *    x2    x2    x2    x2    x2
             *  -------------------------------
             *  4 18  9  4  7  6  9 16  7  2  6
             *
             *  4 + 1 + 8 + 9 + 4 + 7 + 6 + 9 + 1 + 6 + 7 + 2 + 6 = 70
             *  70 % 10 = 0
             */

            //  Clean up number
            String ccNumber1 = CheckNumeric(creditCardNumber);
            Char[] ccNumber1Ary = ccNumber1.ToCharArray();
            int ccLength = ccNumber1Ary.Length;

            //  Reverse string
            StringBuilder buf = new StringBuilder();
            for (int i = ccLength; i != 0; i--)
            {
                buf.Append(ccNumber1Ary[i - 1]);
            }
            String ccNumber = buf.ToString();
            Char[] ccNumberAry = ccNumber.ToCharArray();
            int sum = 0;
            for (int i = 0; i < ccLength; i++)
            {
                int digit = int.Parse(Char.GetNumericValue(ccNumberAry[i]).ToString());
                if (i % 2 == 1)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }
                sum += digit;
            }
            if (sum % 10 == 0)
                return "";

            _log.Fine("validateCreditCardNumber - " + creditCardNumber + " -> "
               + ccNumber + ", Luhn=" + sum);
            return "CreditCardNumberError";
        }

        /**
         *  Validate Credit Card Number.
         *  - Check Card Type and Length
         *  @param creditCardNumber CC Number
         *  @param creditCardType CC Type
         *  @return "" or Error AD_Message
         */
        public static String ValidateCreditCardNumber(String creditCardNumber, String creditCardType)
        {
            if (creditCardNumber == null || creditCardType == null)
                return "CreditCardNumberError";

            //  http://www.beachnet.com/~hstiles/cardtype.html
            //	http://staff.semel.fi/~kribe/document/luhn.htm

            String ccStartList = "";    //  comma separated list of starting numbers
            String ccLengthList = "";   //  comma separated list of lengths
            //
            if (creditCardType.Equals(X_C_Payment.CREDITCARDTYPE_MasterCard))
            {
                ccStartList = "51,52,53,54,55";
                ccLengthList = "16";
            }
            else if (creditCardType.Equals(X_C_Payment.CREDITCARDTYPE_Visa))
            {
                ccStartList = "4";
                ccLengthList = "13,16";
            }
            else if (creditCardType.Equals(X_C_Payment.CREDITCARDTYPE_Amex))
            {
                ccStartList = "34,37";
                ccLengthList = "15";
            }
            else if (creditCardType.Equals(X_C_Payment.CREDITCARDTYPE_Discover))
            {
                ccStartList = "6011";
                ccLengthList = "16";
            }
            else if (creditCardType.Equals(X_C_Payment.CREDITCARDTYPE_Diners))
            {
                ccStartList = "300,301,302,303,304,305,36,38";
                ccLengthList = "14";
            }
            else
            {
                //  enRouteCard
                ccStartList = "2014,2149";
                ccLengthList = "15";
                //  JCBCard
                ccStartList += ",3088,3096,3112,3158,3337,3528";
                ccLengthList += ",16";
                //  JCBCard
                ccStartList += ",2131,1800";
                ccLengthList += ",15";
            }

            //  Clean up number
            String ccNumber = CheckNumeric(creditCardNumber);

            /**
             *  Check Length
             */
            int ccLength = ccNumber.Length;
            Boolean ccLengthOK = false;
            StringTokenizer st = new StringTokenizer(ccLengthList, ",", false);
            while (st.HasMoreTokens() && !ccLengthOK)
            {
                int l = int.Parse(st.NextToken());
                if (ccLength == l)
                    ccLengthOK = true;
            }
            if (!ccLengthOK)
            {
                _log.Fine("validateCreditCardNumber Length=" + ccLength + " <> " + ccLengthList);
                return "CreditCardNumberError";//Credit Card Number not valid";//
            }

            /**
             *  Check Start Digits
             */
            Boolean ccIdentified = false;
            st = new StringTokenizer(ccStartList, ",", false);
            while (st.HasMoreTokens() && !ccIdentified)
            {
                if (ccNumber.StartsWith(st.NextToken()))
                    ccIdentified = true;
            }
            if (!ccIdentified)
            {
                _log.Fine("validateCreditCardNumber Type=" + creditCardType + " <> " + ccStartList);
            }

            //
            String check = ValidateCreditCardNumber(ccNumber);
            if (check.Length != 0)
                return check;
            if (!ccIdentified)
            {
                return "CreditCardNumberProblem?";//There seems to be a Credit Card Number problem.\n\n Continue?";//
            }
            return "";
        }


        /**
         *  Validate Validation Code
         *  @param creditCardVV CC Verification Code
         *  @return "" or Error AD_Message
         */
        public static String ValidateCreditCardVV(String creditCardVV)
        {
            if (creditCardVV == null)
                return "";
            int length = CheckNumeric(creditCardVV).Length;
            if (length == 3 || length == 4)
                return "";
            try
            {
                int.Parse(creditCardVV);
                return "";
            }
            catch (Exception ex)
            {
                _log.Fine("validateCreditCardVV - " + ex);
            }
            _log.Fine("ValidateCreditCardVV - length=" + length);
            return "CreditCardVVError";
        }

        /**
         *  Validate Validation Code
         *  @param creditCardVV CC Verification Code
         *  @param creditCardType CC Type see CC_
         *  @return "" or Error AD_Message
         */
        public static String ValidateCreditCardVV(String creditCardVV, String creditCardType)
        {
            //	no data
            if (creditCardVV == null || creditCardVV.Length == 0
                || creditCardType == null || creditCardType.Length == 0)
                return "";

            int length = CheckNumeric(creditCardVV).Length;

            //	Amex = 4 digits
            if (creditCardType.Equals(X_C_Payment.CREDITCARDTYPE_Amex))
            {
                if (length == 4)
                {
                    try
                    {
                        int.Parse(creditCardVV);
                        return "";
                    }
                    catch (Exception ex)
                    {
                        _log.Fine("validateCreditCardVV - " + ex);
                    }
                }
                _log.Fine("ValidateCreditCardVV(4) CC=" + creditCardType + ", length=" + length);
                return "CreditCardVVError";
            }
            //	Visa & MasterCard - 3 digits
            if (creditCardType.Equals(X_C_Payment.CREDITCARDTYPE_Visa)
                || creditCardType.Equals(X_C_Payment.CREDITCARDTYPE_MasterCard))
            {
                if (length == 3)
                {
                    try
                    {
                        int.Parse(creditCardVV);
                        return "";
                    }
                    catch (Exception ex)
                    {
                        _log.Fine("validateCreditCardVV - " + ex);
                    }
                }
                _log.Fine("ValidateCreditCardVV(3) CC=" + creditCardType + ", length=" + length);
                return "CreditCardVVError";
            }

            //	Other
            return "";
        }


        /**************************************************************************
         *  Validate Routing Number
         *  @param routingNo Routing No
         *  @return "" or Error AD_Message
         */
        public static String ValidateRoutingNo(String routingNo)
        {
            int length = CheckNumeric(routingNo).Length;
            //  US - length 9
            //  Germany - length 8
            //	Japan - 7
            //	CH - 5
            //	Issue: Bank account country
            if (length > 0)
                return "";
            return "PaymentBankRoutingNotValid";//Bank Routing Number is not valid";//
        }

        /**
         *  Validate Account No
         *  @param AccountNo AccountNo
         *  @return "" or Error AD_Message
         */
        public static String ValidateAccountNo(String accountNo)
        {
            int length = CheckNumeric(accountNo).Length;
            if (length > 0)
                return "";
            return "PaymentBankAccountNotValid";//Bank Account Number is not valid";//
        }

        /**
         *  Validate Check No
         *  @param CheckNo CheckNo
         *  @return "" or Error AD_Message
         */
        public static String ValidateCheckNo(String checkNo)
        {
            int length = CheckNumeric(checkNo).Length;
            if (length > 0)
                return "";
            return "PaymentBankCheckNotValid";//Bank Check Number is not valid";//
        }

        /**
         *  Check Numeric
         *  @param data input
         *  @return the digits of the data - ignore the rest
         */
        public static String CheckNumeric(String data)
        {
            if (data == null || data.Length == 0)
                return "";
            //  Remove all non Digits
            StringBuilder sb = new StringBuilder();
            Char[] chrArry = data.ToCharArray();
            for (int i = 0; i < chrArry.Length; i++)
            {
                if (Char.IsDigit(chrArry[i]))
                    sb.Append(chrArry[i].ToString());
            }
            return sb.ToString();
        }
    }
}
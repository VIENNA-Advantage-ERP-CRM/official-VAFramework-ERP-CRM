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
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public abstract class PaymentProcessor
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
	    public PaymentProcessor()
	    {
            if (log == null)
            {
                log = VLogger.GetVLogger(this.GetType().FullName);
            }
	    }

	    //Logger
        protected VLogger log = null;
	    //Payment Processor Logger		
        private static VLogger _log = VLogger.GetVLogger(typeof(PaymentProcessor).FullName);
	    /** Encoding (ISO-8859-1 - UTF-8) 		*/
	    public const String	ENCODING = "UTF-8";
	    /** Encode Parameters		*/
	    private Boolean _encoded = false;
	    /** Ampersand				*/
	    public const char AMP = '&'; 
	    /** Equals					*/
	    public const char EQ = '='; 

        protected MPaymentProcessor _mpp = null;
	    protected MPayment _mp = null;
	    //
	    private int _timeout = 30;

	    /**
	     *  Factory
	     * 	@param mpp payment processor model
	     * 	@param mp payment model
	     *  @return initialized PaymentProcessor or null
	     */
	    public static PaymentProcessor Create (MPaymentProcessor mpp, MPayment mp)
	    {
		    _log.Info("create for " + mpp);
		    String className = mpp.GetPayProcessorClass();
		    if (className == null || className.Length == 0)
		    {
			    _log.Log(Level.SEVERE, "No PaymentProcessor class name in " + mpp);
			    return null;
		    }
		    //
		    PaymentProcessor myProcessor = null;
		    try
		    {
                //Class ppClass = Class.forName(className);
                //if (ppClass != null)
                //    myProcessor = (PaymentProcessor)ppClass.newInstance();

                Type ppClass = Type.GetType(className);
                if (ppClass != null)
                {
                    myProcessor = (PaymentProcessor)Activator.CreateInstance(ppClass);
                }
		    }
            //catch (Error e1)    //  NoClassDefFound
            //{
            //    _log.Log(Level.SEVERE, className + " - Error=" + e1.Message);
            //    return null;
            //}
		    catch (Exception e2)
		    {
			    _log.Log(Level.SEVERE, className, e2);
			    return null;
		    }
		    if (myProcessor == null)
		    {
			    _log.Log(Level.SEVERE, "no class");
			    return null;
		    }

		    //  Initialize
		    myProcessor._mpp = mpp;
		    myProcessor._mp = mp;
		    //
		    return myProcessor;
	    }

	    /**
	     *  Process CreditCard (no date check)
	     *  @return true if processed successfully
	     */
        public abstract Boolean ProcessCC();

	    /**
	     *  Payment is procesed successfully
	     *  @return true if OK
	     */
	    public abstract Boolean IsProcessedOK();

    	
	    /**
	     * 	Set Timeout
	     * 	@param newTimeout timeout
	     */
	    public void SetTimeout(int newTimeout)
	    {
		    _timeout = newTimeout;
	    }
	    /**
	     * 	Get Timeout
	     *	@return timeout
	     */
	    public int GetTimeout()
	    {
		    return _timeout;
	    }

    	
	    /**************************************************************************
	     *  Check for delimiter fields &= and add length of not encoded
	     *  @param name name
	     *  @param value value
	     *  @param maxLength maximum length
	     *  @return name[5]=value or name=value
	     */
	    protected String CreatePair(String name, Decimal value, int maxLength)
	    {
            //if (value == null)
            //    return CreatePair (name, "0", maxLength);
		   // else
		    //{
                if (Env.Scale(value) < 2)
                    value = Decimal.Round(value, 2, MidpointRounding.AwayFromZero);
			    return CreatePair (name, value.ToString(), maxLength);
		   // }
	    }

	    /**
	     *  Check for delimiter fields &= and add length of not encoded
	     *  @param name name
	     *  @param value value
	     *  @param maxLength maximum length
	     *  @return name[5]=value or name=value
	     */
	    protected String CreatePair(String name, int value, int maxLength)
	    {
		    if (value == 0)
			    return "";
		    else
                return CreatePair(name, value.ToString(), maxLength);
	    }

	    /**
	     *  Check for delimiter fields &= and add length of not encoded
	     *  @param name name
	     *  @param value value
	     *  @param maxLength maximum length
	     *  @return name[5]=value or name=value 
	     */
	    protected String CreatePair(String name, String value, int maxLength)
	    {
		    //  Nothing to say
		    if (name == null || name.Length == 0
			    || value == null || value.Length == 0)
			    return "";
    		
		    if (value.Length > maxLength)
			    value = value.Substring(0, maxLength);
    		
		    StringBuilder retValue = new StringBuilder(name);
		    if (_encoded)
			    try
			    {
				   // value = URLEncoder.encode(value, ENCODING);
			    }
			    catch (Exception e)
			    {
				    log.Log(Level.SEVERE, value + " - " + e.ToString());
			    }
		    else if (value.IndexOf(AMP) != -1 || value.IndexOf(EQ) != -1)
			    retValue.Append("[").Append(value.Length).Append("]");
		    //
		    retValue.Append(EQ);
		    retValue.Append(value);
		    return retValue.ToString();
	    }
    	
	    /**
	     * 	Set Encoded
	     *	@param doEncode true if encode
	     */
	    public void SetEncoded (Boolean doEncode)
	    {
		    _encoded = doEncode;
	    }

	    /**
	     * 	Is Encoded
	     *	@return true if encoded
	     */
	    public Boolean IsEncoded()
	    {
		    return _encoded;
	    }
    	
	    /**
	     * 	Get Connect Post Properties
	     *	@param urlString POST url string
	     *	@param parameter parameter
	     *	@return result as properties
	     */
        //protected Properties GetConnectPostProperties (String urlString, String parameter)
        //{
        //    long start = CommonFunctions.CurrentTimeMillis();
        //    String result = ConnectPost(urlString, parameter);
        //    if (result == null)
        //        return null;
        //    Properties prop = new Properties();
        //    try
        //    {
        //        String info = URLDecoder.decode(result, ENCODING);
        //        StringTokenizer st = new StringTokenizer(info, "&");	//	AMP
        //        while (st.HasMoreTokens())
        //        {
        //            String token = st.NextToken();
        //            int index = token.IndexOf('=');
        //            if (index == -1)
        //                prop.put(token, "");
        //            else
        //            {
        //                String key = token.Substring(0, index);
        //                String value = token.Substring(index+1);
        //                prop.put(key, value);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Log(Level.SEVERE, result, e);
        //    }
        //    long ms = CommonFunctions.CurrentTimeMillis() - start;
        //    log.fine(ms + "ms - " + prop.toString());
        //    return prop;
        //}
    	
        ///**
        // * 	Connect via Post
        // *	@param urlString url destination (assuming https)
        // *	@param parameter parameter
        // *	@return response or null if failure
        // */
        //protected String ConnectPost (String urlString, String parameter)
        //{
        //    String response = null;
        //    try
        //    {
        //        // open secure connection
        //        URL url = new URL(urlString);
        //        HttpsURLConnection connection = (HttpsURLConnection) url.openConnection();
        //    //	URLConnection connection = url.openConnection();
        //        connection.SetDoOutput(true);
        //        connection.SetUseCaches(false);
        //        connection.SetRequestProperty("Content-Type","application/x-www-form-urlencoded");
        //        log.fine(connection.GetURL().toString());

        //        // POST the parameter
        //        DataOutputStream out = new DataOutputStream (connection.GetOutputStream());
        //        out.write(parameter.GetBytes());
        //        out.flush();
        //        out.close();

        //        // process and read the gateway response
        //        BufferedReader in = new BufferedReader(new InputStreamReader(connection.GetInputStream()));
        //        response = in.readLine();
        //        in.close();	                     // no more data
        //        log.finest(response);
        //    }
        //    catch (Exception e)
        //    {
        //        log.Log(Level.SEVERE, urlString, e);
        //    }
        //    //
        //    return response;
        //}
    }
}

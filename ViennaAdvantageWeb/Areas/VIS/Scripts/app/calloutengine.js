; VIS.Model = VIS.Model || {}; //Init Model NameSpace

; (function (VIS, $) {


    var calloutActive = false; //flag

    /**
     *  Callout Base class for Callout.
     *  Used in MTab and ImpFormatRow
     *
     */

    function CalloutEngine(clsName) {
        if (!clsName) {
            clsName = "CalloutEngine";
        }
        this.log = VIS.Logging.VLogger.getVLogger(clsName);//init log Class
    };

    /**
	 * 	Is Callout Active
	 *	@return true if active
	 */
    CalloutEngine.prototype.isCalloutActive = function () {
        return calloutActive;
    };

    /**
	 * 	Set Callout (in)active
	 *	@param active active
	 */
    CalloutEngine.prototype.setCalloutActive = function (active) {
        calloutActive = active;
    };

    /**
	 *	Start Callout.
	 *  <p>
	 *	Callout's are used for cross field validation and setting values in other fields
	 *	when returning a non empty (error message) string, an exception is raised
	 *  <p>
	 *	When invoked, the Tab model has the new value!
	 *
	 *  @param ctx      Context
	 *  @param methodName   Method name
	 *  @param WindowNo current Window No
	 *  @param mTab     Model Tab
	 *  @param mField   Model Field
	 *  @param value    The new value
	 *  @param oldValue The old value
	 *  @return Error message or ""
	 */
    CalloutEngine.prototype.start = function (ctx, methodName, windowNo, mTab, mField, value, oldValue) {

        if (methodName == null || methodName.length == 0)
            throw new Error("No Method Name");
        //
        var retValue = "";
        var msg = methodName + " - " + (mField.getColumnName())
			+ "=" + value + " (old=" + oldValue + ") {active=" + this.isCalloutActive() + "}";

        if (!this.isCalloutActive())
            this.log.info(msg.toString());

        //	Find Method
        var method = this[methodName];
        if (method == null) {
            //methodName = methodName.replace(methodName, methodName.charAt(0).toUpperCase() + methodName.substr(1));//Added By Sarab to Handle Case
            methodName = methodName.charAt(0).toUpperCase() + methodName.substr(1);

            method = this[methodName];
            if (!method) {
                throw new Error("Method not found: " + methodName);
            };
        };
        //  console.log(method);

        var argLength = method.length;
        if (!(argLength == 5 || argLength == 6))
            throw new Error("Method " + methodName
				+ " has invalid no of arguments: " + argLength);

        //	Call Method
        try {
            if (argLength == 6)
                retValue = this[methodName](ctx, windowNo, mTab, mField, value, oldValue);
            else
                retValue = this[methodName](ctx, windowNo, mTab, mField, value);
        }
        catch (e) {
            this.setCalloutActive(false);
            this.log.log(VIS.Logging.Level.SEVERE, "start: " + methodName, e);
            retValue = e.toString();
        }
        return retValue;
    };

    /// <summary>
    /// Set Account Date to the date of the calling column.
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="tab">tab</param>
    /// <param name="field">field</param>
    /// <param name="value">value</param>
    /// <returns>null or error message</returns>
    CalloutEngine.prototype.DateAcct = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive())		//	assuming it is resetting value
            return "";
        //	setCalloutActive(true);
        //if (value == null || !(value.getType() == typeof(DateTime)))
        if (!value)
            return "";
        mTab.setValue("DateAcct", value);
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Rate - set Multiply Rate from Divide Rate and vice versa
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="tab">tab</param>
    /// <param name="field">field</param>
    /// <param name="value">value</param>
    /// <returns>null or error message</returns>
    CalloutEngine.prototype.Rate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null)		//	assuming it is Conversion_Rate
        {
            return "";
        }
        this.setCalloutActive(true);

        var rate1 = value;
        var rate2 = VIS.Env.ZERO;
        var one = 1.0;

        if (VIS.Utility.Util.getValueOfDouble(rate1) != 0.0)	//	no divide by zero
        {
            //rate2 = Decimal.Round(Decimal.Divide(one, rate1), 12);// MidpointRounding.AwayFromZero);  //By Sarab
            rate2 = (one / rate1).toFixed(12);
        }
        //
        if (mField.getColumnName().toString() == "MultiplyRate") {
            mTab.setValue("DivideRate", rate2);
        }
        else {
            mTab.setValue("MultiplyRate", rate2);
        }
        this.log.info(mField.getColumnName() + "=" + rate1 + " => " + rate2);
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;

        return "";
    };

    VIS.Model.CalloutEngine = CalloutEngine;// global object assignment
    VIS.CalloutEngine = CalloutEngine;
})(VIS, jQuery);
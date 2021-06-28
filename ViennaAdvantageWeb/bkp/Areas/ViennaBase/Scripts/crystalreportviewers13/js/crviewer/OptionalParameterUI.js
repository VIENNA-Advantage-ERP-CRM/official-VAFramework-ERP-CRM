/**
 * OptionalParameterUI extends ParameterUI
 */

bobj.crv.params.newOptionalParameterUI = function(kwArgs) {
    kwArgs = MochiKit.Base.update( {
        noValueDisplayText :'',
        isEmptyStringNoValue: true,
        clearValuesCB : null
    }, kwArgs);

    var o = bobj.crv.params.newParameterUI(kwArgs);

    /*
     * The reason I'm using bobj.extendClass is that it would populate all functions defined in ParameterUI 
     * in this.superClass  so I can call any function on parent class
     */
    bobj.extendClass(o, bobj.crv.params.OptionalParameterUI, bobj.crv.params.ParameterUI);

    return o;
};

bobj.crv.params.OptionalParameterUI = {
    _getNewValueRowConstructor : function() {
        return bobj.crv.params.newOptionalParameterValueRow;
    },

    _getNewValueRowArgs : function(value) {
        var args = this.superClass._getNewValueRowArgs(value);
        args.noValueDisplayText = this.noValueDisplayText;
        args.isEmptyStringNoValue = this.isEmptyStringNoValue;
        args.clearValuesCB = this.clearValuesCB;
        return args;
    }
};

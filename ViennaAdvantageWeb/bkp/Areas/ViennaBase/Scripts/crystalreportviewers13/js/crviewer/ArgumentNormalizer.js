/* Copyright (c) Business Objects 2006. All rights reserved. */

/**
 * ArgumentNormalizer names and transforms function arguments based upon a
 * set of rules provided by the user.
 */
 
if (typeof bobj ==  'undefined') {
    bobj = {};
}
 
bobj.ArgumentNormalizer = function() {
    this._rules = [];    
};

bobj.ArgumentNormalizer.prototype = {
    
    /**
     * Add a rule for naming and transforming arguments.
     * When arguments are normalized each rule is applied until a match is 
     * found. A rule is a set of elements of the following form:
     *
     * {
     *   test:  [function, null],
     *   name:  [string],
     *   xform: [function - optional]
     * }
     *
     * There should be one element of the above form for each argument expected
     * by the rule. A rule is a match if and only if true is returned by every 
     * element's test function when passed its corresponding argument. A null  
     * test function is considered to return true. Rules are tested in the order
     * they were added until one matches or there are no more to test.
     *
     * When a rule matches, it applies the optional xform (transform) functions 
     * to it arguments and saves the results in a return object with properties
     * specified by the names in the rule elements.
     *
     * Example:
     *
     * n.addRule({test:isString, name:'description', xform:trim},
     *           {test:isNumber, name:'id'});
     * n.addRule({test:isString, name:'description', xform:trim},
     *           {test:isString, name:'id', xform:parseInt});
     *
     * n.normalize("  Blue car", 11); // First rule matches
     * -> {description: "Blue car", id: 11}
     *    
     * n.normalize("Green car ", "55"); // Second rule matches
     * -> {description: "Green car", id: 55}   
     *
     * Rule elements may be passed as arrays for brevity. The first rule
     * from the example above would be added as follows:
     *
     * n.addRule([isString, 'description', trim], [isNumber, 'id']);
     *
     * When an element simply names an argument (no test or transform is 
     * desired), it may be specified as a string. For example:
     *
     * n.addRule([isString, 'description', trim], 'id');
     */
    addRule: function() {
        this._rules.push(arguments);
    },
    
    /**
     * Normalize the arguments based upon the rules that have been added
     *
     * @param arguments   Arguments to be normalized
     * 
     * @return An object with a property for each transformed argument or null
     */
    normalize: function() {
        for (var rIdx = 0, nRules = this._rules.length; rIdx < nRules; ++rIdx) {
            var rule = this._rules[rIdx];
            
            if (rule.length == arguments.length) {
                var normalArgs = {};
                
                for (var aIdx = 0, nArgs = rule.length; aIdx < nArgs; ++aIdx) {
                    var argVal = arguments[aIdx];
                    var element = rule[aIdx];
                    
                    if (bobj.isString(element)) { // No test specified, just name the argument
                        var argTest = null;
                        var argName = element;
                        var argXform = null;
                    }
                    else if (bobj.isArray(element)) {
                        var argTest = element[0];
                        var argName = element[1];
                        var argXform = element[2];
                    }
                    else {
                        var argTest = element.test;
                        var argName = element.name;
                        var argXform = element.xform;
                    }
                
                    if (!argTest || argTest(argVal)) { 
                        normalArgs[argName] = argXform ? argXform(argVal) : argVal;
                        if (aIdx+1 == nArgs) { // if no more args to check
                            return normalArgs; // Rule matched, return normalized args    
                        }
                    }
                    else {
                        break; // Rule didn't match, try the next one    
                    }
                }
            }
        }
        return null;
    },
    
    /**
     * Applies an array of arguments to normalize()
     *
     * @param argsArray [Array]  Array of arguments to normalize
     *
     * @return Normalized arguments or null
     */
    normalizeArray: function(argsArray) {
        return this.normalize.apply(this, argsArray);    
    }
};


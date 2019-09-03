/**
 * Publish/Subscribe event system for anonymous message passing
 *
 */
 
if (typeof bobj == 'undefined') {
    bobj = {};
}

if (typeof bobj.event == 'undefined') {
    bobj.event = {};
    bobj.event._topicSubscriptions = {};
    bobj.event._globalSubscriptions = [];
}

/**
 * Publish an event for subscribers to receive
 * 
 * @param topic [String]  The event topic
 * @param arguments       Unnamed args will be passed to subscribers
 */
bobj.event.publish = function(topic) {
    var args = bobj.slice(arguments, 1);
    
    var topicSubs = bobj.event._topicSubscriptions[topic];
    if (topicSubs) {
        for (var i = 0; i < topicSubs.length; ++i) {
            topicSubs[i]._notify.apply(null, args);        
        }
    }
    
    var globalSubs = bobj.event._globalSubscriptions;
    for (var j = 0; j < globalSubs.length; ++j) {
        globalSubs[j]._notify.apply(null, args);        
    }
};

/**
 * Subscribe to a topic
 *
 * @param topic     [String]   The event topic
 * @param callback  [Function] Function to be call with topic's arguments
 * @param target    [Object]   Object to notify (requires methName)
 * @param methName  [String]   Name of method to call when target is notified
 *
 * Arguments may be passed in any of the following combinations:
 * (topic, target, methName)
 * (topic, callback)
 * (target, methName)
 * (callback)
 *
 * When a topic is not specified, the subscriber receives all topics
 *
 * @return Subscription object that can be used to unsubscribe
 */
bobj.event.subscribe = function() {
    var nmlr = bobj.event.subscribe._normalizer;
    if (!nmlr) {
        nmlr = bobj.event.subscribe._normalizer = new bobj.ArgumentNormalizer();
        
        // If 3 args: (topic, target, methName)
        nmlr.addRule('topic', 'target', 'methName');
        
        // If 2 args and first is a string: (topic, callback)
        nmlr.addRule([bobj.isString, 'topic'], 'callback');
        
        // If 2 args: (target, methName)
        nmlr.addRule('target', 'methName');
        
        // If 1 arg: (callback)
        nmlr.addRule('callback');
    }
    
    return bobj.event.kwSubscribe(nmlr.normalizeArray(arguments));
};

/**
 * Subscribe to a topic using keyword arguments
 *
 * @param kwArgs.topic     [String]   The event topic
 * @param kwArgs.callback  [Function] Function to be call with topic's arguments
 * @param kwArgs.target    [Object]   Object to notify (requires methName)
 * @param kwArgs.methName  [String]   Name of method to call when target is notified
 *
 * @see bobj.event.subscribe
 *
 * @return Subscription object that can be used to unsubscribe
 */
bobj.event.kwSubscribe = function(kwArgs) {
    var bind = MochiKit.Base.bind;
    var subscription = {};
    
    if (kwArgs.callback) {
        subscription._notify = kwArgs.callback;        
    }
    else {
        subscription._notify = bind(kwArgs.target[kwArgs.methName], kwArgs.target);
    }
    
    if (kwArgs.topic) {
        subscription.topic = kwArgs.topic;
        
        var subs = bobj.event._topicSubscriptions;
        if (!subs[kwArgs.topic]) {
            subs[kwArgs.topic] = [];    
        }
        subs[kwArgs.topic].push(subscription);
    }
    else {
        bobj.event._globalSubscriptions.push(subscription);
    }
    
    return subscription;
};

/**
 * Unsubscribe from a topic
 *
 * @param subscription [Subscription]  The object returned by subscribe
 */
bobj.event.unsubscribe = function(subscription) {
    var subsList = bobj.event._globalSubscriptions;
    
    if (subscription.topic) {
        subsList = bobj.event._topicSubscriptions[subscription.topic];
    }
    
    if (subsList) {
        var idx = MochiKit.Base.findIdentical(subsList, subscription);
        if (idx != -1) {
            subsList.splice(idx, 1);     // remove the subscription
            delete subscription._notify; // prevent leaks
        }
    }
};


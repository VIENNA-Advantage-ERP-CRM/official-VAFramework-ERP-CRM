/* Copyright (c) Business Objects 2006. All rights reserved. */

if (typeof(bobj.crv.Separator) == 'undefined') {
    bobj.crv.Separator = {};
}

/**
 * Separator Constructor. Simple horizontal separator.
 *
 */
bobj.crv.newSeparator = function(kwArgs) {
    var UPDATE = MochiKit.Base.update;
    kwArgs = UPDATE({
        id: bobj.uniqueId(),
        marginLeft: 4,
        marginRight: 4,
        marginTop: 0,
        marginBottom: 2
    }, kwArgs);
    var o = newWidget(kwArgs.id);

    bobj.fillIn(o, kwArgs);  
    o.widgetType = 'Separator';
    
    // Attach member functions 
    UPDATE(o, bobj.crv.Separator);
    
    return o;    
};

bobj.crv.Separator.getHTML = function() {
    var HTML = bobj.html;
    var htmlStr = '';
    if (bobj.isBorderBoxModel()) { 
        htmlStr = HTML.IMG({
            id: this.id,
            src: bobj.skinUri('sep.gif'),
            style: {
                'height': 2 + 'px',
                'width':'100%',
                'margin-left': this.marginLeft + 'px',
                'margin-right': this.marginRight + 'px',
                'margin-top': this.marginTop + 'px',
                'margin-bottom': this.marginBottom + 'px'
        }});
    }
    else {
        htmlStr = HTML.DIV({
            id : this.id,
            style: {
                'height': 2 + 'px',
                'margin-left': this.marginLeft + 'px',
                'margin-right': this.marginRight + 'px',
                'margin-top': this.marginTop + 'px',
                'margin-bottom': this.marginBottom + 'px',
                'background-image': 'url(' + bobj.skinUri('sep.gif') + ')',
                'background-repeat': 'repeat-x',
                'overflow': 'hidden'
    

        }});
    }
    return htmlStr + bobj.crv.getInitHTML(this.widx);
};

/**
 * @return Returns the outer height of the widget, including margins
 */
bobj.crv.Separator.getHeight = function() {
    return this.layer.offsetHeight + this.marginTop + this.marginBottom;
};
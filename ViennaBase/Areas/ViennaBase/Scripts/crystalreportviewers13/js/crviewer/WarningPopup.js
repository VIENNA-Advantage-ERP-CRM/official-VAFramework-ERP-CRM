if (typeof bobj === 'undefined') {
    bobj = {};
}
if (typeof bobj.crv === 'undefined') {
    bobj.crv = {};
}
if (typeof bobj.crv.WarningPopup === 'undefined') {
    bobj.crv.WarningPopup = {};
}

bobj.crv.WarningPopup.getInstance = function() {
    if (bobj.crv.WarningPopup.__instance === undefined)
        bobj.crv.WarningPopup.__instance = new bobj.crv.WarningPopup.Class();

    return bobj.crv.WarningPopup.__instance;
};

bobj.crv.WarningPopup.Class = function() {
    this.layer = null;
    this.id = bobj.uniqueId();
};

bobj.crv.WarningPopup.Class.prototype = {
    show : function(text, xPos, yPos) {
        if (!this.layer) {
            this.init();
        }

        this.layer.style.top = yPos + "px";
        this.layer.style.left = xPos + "px";
        this.txtLayer.innerHTML = text;
        this.layer.style.display = "block";
    },

    hide : function() {
        if (this.layer)
            this.layer.style.display = "none";
    },

    getHTML : function() {
        return bobj.html.DIV( {
            'class' : 'WarningPopup',
            id : this.id
        }, bobj.html.IMG( {
            id : this.id + "img",
            style : {
                position : 'absolute',
                left : '10px',
                top : '-19px'
            },
            src : bobj.crvUri('images/WarningPopupTriangle.gif')
        }), bobj.html.DIV( {
            id : this.id + "txt",
            style : {
                padding : "5px"
            }
        }));

    },
    init : function() {
        append2(document.body, this.getHTML());
        this.layer = getLayer(this.id);
        this.txtLayer = getLayer(this.id + "txt");
    }
};

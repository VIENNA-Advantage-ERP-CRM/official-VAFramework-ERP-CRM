; (function (VIS, $) {
/**
     *	Card view Container
     * 
     */

function VCardView() {

    this.cGroupInfo = {}; //group control
    this.cCols = [];  //Include Column
    this.cConditions = []; //Conditions
    this.cGroup = null;//Group Column
    this.mTab;
    this.AD_Window_ID;
    this.AD_Tab_ID;
    this.groupCtrls = [];
    this.fields = [];// card view fields
    this.grpCount = 0;
    this.grpColName = '';
    this.hasIncludedCols = false;
    // this.aPanel;
    this.onCardEdit = null;

    var root;
    var body = null;
    var headerdiv;
    //  var cardList;
    function init() {
        root = $("<div class='vis-cv-body vis-noselect'>");
        body = $("<div class='vis-cv-main'>");
        headerdiv = $("<div class='vis-cv-header'>");
        //   cardList = $("<img style='margin-left:10px;margin-top:4px;float:right' src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/defaultCView.png' >");
        root.append(body);
    }

    //function eventHandle() {
    //    var selfff = this;
    //    var $menu = $('<ul class="vis-apanel-rb-ul-card"></ul>');

    //    cardList.on("click", function () {

    //        $menu.empty();

    //        var selfee = this;
    //        var url = VIS.Application.contextUrl + "CardView/GetCardView";
    //        $.ajax({
    //            type: "GET",
    //            async: false,
    //            url: url,
    //            dataType: "json",
    //            contentType: 'application/json; charset=utf-8',
    //            data: { ad_Window_ID: self.mTab.getAD_Window_ID(), ad_Tab_ID: self.mTab.getAD_Tab_ID() },
    //            success: function (data) {
    //                data = JSON.parse(data);

    //                if (data && data.length > 0 && data[0].lstCardViewData && data[0].lstCardViewData.length > 0) {

    //                    for (var i = 0; i < data[0].lstCardViewData.length; i++) {
    //                        if (data[0].lstCardViewData[i].DefaultID) {
    //                            $menu.append($('<li style="background-color:white;overflow:auto;border:white;border-bottom:2px #EBEBEB solid;"  data-id="' + data[0].lstCardViewData[i].CardViewID + '" ><p style="margin:5px 5px 5px 0px;font-size:12px;float:left" data-id="' + data[0].lstCardViewData[i].CardViewID + '"  >' + data[0].lstCardViewData[i].CardViewName + '</p><span data-id="' + data[0].lstCardViewData[i].CardViewID + '" title="' + VIS.Msg.getMsg("DefaultSearch") + '" class="VIS-winSearch-defaultIcon"></span> </div></li>'));
    //                        }
    //                        else {
    //                            $menu.append($('<li style="background-color:white;overflow:auto;border:white;border-bottom:2px #EBEBEB solid;"  data-id="' + data[0].lstCardViewData[i].CardViewID + '" ><p style="margin:5px 5px 5px 0px;font-size:12px;float:left" data-id="' + data[0].lstCardViewData[i].CardViewID + '"  >' + data[0].lstCardViewData[i].CardViewName + '</p><span data-id="' + data[0].lstCardViewData[i].CardViewID + '" title="' + VIS.Msg.getMsg("MakeDefaultSearch") + '" class="VIS-winSearch-NonDefaultIcon"></span> </div></li>'));
    //                        }
    //                    }
    //                }

    //                $(selfee).w2overlay($menu.clone(true),
    //                    {
    //                        options: {
    //                            css: { width: 'auto' },
    //                            onShow: function (e) {
    //                                console.log(e);
    //                            }
    //                        }
    //                    }
    //               );
    //                //var item = [];
    //                //if (data && data.length > 0 && data[0].lstCardViewData && data[0].lstCardViewData.length > 0) {
    //                //    for (var i = 0; i < data[0].lstCardViewData.length; i++) {
    //                //        if (data[0].lstCardViewData[i].DefaultID) {
    //                //            item.push({ id: i, text: data[0].lstCardViewData[i].CardViewName, img: 'VIS-winSearch-defaultIcon' });
    //                //        }
    //                //        else {
    //                //            item.push({ id: i, text: data[0].lstCardViewData[i].CardViewName, img: 'VIS-winSearch-NonDefaultIcon' });
    //                //        }
    //                //    }
    //                //}

    //                //$(selfee).w2menu({
    //                //    items: item,
    //                //    onSelect: function (event) {
    //                //        console.log(event);
    //                //        var target = $(event.target);
    //                //        AD_CardView_ID = target.data("id");

    //                //    }
    //                //});


    //            }
    //        });
    //    });


    //    $menu.on("click", function (e) {
    //        var target = $(e.target);
    //        AD_CardView_ID = target.data("id");
    //        if (!AD_CardView_ID)
    //        {
    //            return;
    //        }
    //        if (target.is('span')) {
    //            $.ajax({
    //                type: "GET",
    //                url: VIS.Application.contextUrl + "CardView/SetDefaultView",
    //                dataType: "json",
    //                data: { AD_Tab_ID: self.mTab.getAD_Tab_ID(), cardView: AD_CardView_ID },
    //                success: function (data) {

    //                },
    //                error: function (err) {
    //                    console.log(err);
    //                }
    //            });
    //        }


    //        var url = VIS.Application.contextUrl + "CardView/GetCardViewColumns";
    //        $.ajax({
    //            type: "GET",
    //            async: false,
    //            url: url,
    //            dataType: "json",
    //            contentType: 'application/json; charset=utf-8',
    //            data: { ad_CardView_ID: AD_CardView_ID },
    //            success: function (data) {
    //                var dbResult = JSON.parse(data);
    //                var CVColumns = dbResult[0].lstCardViewData;
    //                var LstCardViewCondition = dbResult[0].lstCardViewConditonData;

    //                if (CVColumns != null && CVColumns.length > 0) {
    //                    var cardViewUserID = CVColumns[0].UserID;

    //                    var incColumns = [];
    //                    for (var i = 0; i < CVColumns.length; i++) {
    //                        if (CVColumns[i].AD_Field_ID == 0) {
    //                            continue;
    //                        }
    //                        incColumns.push(CVColumns[i].AD_Field_ID);
    //                    }


    //                    var retVal = {};
    //                    retVal.FieldGroupID = CVColumns[0].AD_GroupField_ID;
    //                    retVal.IncludedCols = incColumns;
    //                    retVal.Conditions = [];
    //                    retVal.AD_CardView_ID = AD_CardView_ID;
    //                    retVal.Conditions = dbResult[0].lstCardViewConditonData;
    //                    self.setCardViewData(retVal);


    //                    self.refreshUI(self.aPanel.curGC.getVCardPanel().width());

    //                }
    //            }, error: function (errorThrown) {
    //                VIS.ADialog.error(errorThrown.statusText);
    //            }
    //        })


    //    });


    //};
    var self = this;

    init();
    //eventHandle();

    this.getRoot = function () {
        return root;
    };
    this.getBody = function () {
        return body;
    };

    this.setHeader = function (txt) {
        headerdiv.text(txt);
    }
    this.getHeader = function () {
        return headerdiv;
    }

    //this.getCardList = function () {
    //    return cardList;
    //};


    this.sizeChanged = function (h, w) {
        root.height((h - 12) + 'px');
        this.calculateWidth(w);
    }

    this.calculateWidth = function (width) {
        //set width
        var grpCtrlC = this.groupCtrls.length;
        if (grpCtrlC < 1)
            return;
        if (width) {

            var tGrpW = 262 * grpCtrlC;
            if (tGrpW > width) {
                body.width(262 * grpCtrlC);
            }
            else {
                body.width(width);
                var newW = Math.ceil(width / grpCtrlC) - 24;
                while (grpCtrlC > 0) {
                    --grpCtrlC;

                    this.groupCtrls[grpCtrlC].setWidth(newW);
                }
            }
        }
        else
            body.width(262 * (grpCtrlC));
        this.navigate();
    };


    //body.on('click', 'span.vis-cv-card-edit', function (e) {
    //    if (self.onCardEdit) {
    //        var s = $(e.target).data('recid');
    //        if (s || s === 0) {
    //            self.onCardEdit({ 'recid': s })
    //        }
    //    }
    //    e.stopPropagation();
    //});

    body.on('click', 'div.vis-cv-card', function (e) {

        if (self.onCardEdit) {
            var d = $(e.target);
            var s;
            if (d[0].nodeName == 'SPAN' && d.hasClass('vis-cv-card-edit')) {
                s = d.data('recid');
                if (s || s === 0) {
                    self.onCardEdit({ 'recid': s })
                }
            }
            else {
                var i = 0;
                while (!d.hasClass('vis-cv-card')) {
                    if (i > 5)
                        break;
                    d = d.parent();
                    i++
                }
                s = d.data('recid');
                if (s || s === 0) {
                    self.onCardEdit({ 'recid': s }, true)
                    self.navigate(s, false, true);
                }
            }
        }
        e.stopPropagation();
    });

    this.getAD_CardView_ID = function () {
        return this.AD_CardView_ID;
    };

    this.getField_Group_ID = function () {
        if (this.cGroup)
            return this.cGroup.getAD_Field_ID();
        return 0;
    };

    var curCard = null;
    var crid = null;
    this.navigate = function (rid, oset, skipScroll) {
        if (rid)
            crid = rid;
        if (oset)
            return;

        if (curCard && curCard.length > 0)
            curCard.toggleClass("vis-cv-card-selected");     //.find('span.vis-cv-card-selected').css({ 'display': 'none' });

        curCard = body.find('div.vis-cv-card[name~=vc_' + crid + ']');
        if (curCard.length != 0) {
            //curCard.find('span.vis-cv-card-selected').css({ 'display': 'block' });
            curCard.toggleClass("vis-cv-card-selected");
            if (!skipScroll)
                curCard[0].scrollIntoView();
        }
    };

    this.dC = function () {

        body.off('click');
        this.onCardEdit = null;
        self = null;

        root.remove();
        body.remove();
        headerdiv.remove();

        root = body = headerdiv = null;

        this.getRoot = this.getBody = this.setHeader = this.getHeader = this.dC = null;
        curCard = null;
        this.cGroupInfo = {}; //group control
        this.cCols.length = 0;
        this.cConditions.length = 0;
        this.cGroup = null;//Group Column
        this.mTab = null;
        this.fields.length = 0;
        this.grpCount = null;

        while (this.groupCtrls.length > 0) {
            this.groupCtrls.pop().dispose();
        }
    };
};

VCardView.prototype.tableModelChanged = function (action, args, actionIndexOrId) {

    // this.blockSelect = true;
    var id = null;
    if (action === VIS.VTable.prototype.ROW_REFRESH) {
        if (args.recid)
            id = args.recid;
        else
            id = args;
    }

    else {

        if (action === VIS.VTable.prototype.ROW_UNDO) {
            //this.grid.unselect(args);
            this.getBody().find('div.vis-cv-card[name~=vc_' + args + ']').remove();
        }

        else if (action === VIS.VTable.prototype.ROW_DELETE) {
            var argsL = args.slice();
            while (argsL.length > 0) {
                var recid = argsL.pop();
                this.getBody().find('div.vis-cv-card[name~=vc_' + recid + ']').remove();
            }
            if (isNaN(actionIndexOrId)) //recid array In this case
            {
                id = actionIndexOrId[0];
            }
            else {
                //if (this.grid.records.length > 0)
                //   id = this.grid.records[(this.grid.records.length - 1) < actionIndexOrId ? (this.grid.records.length - 1) : actionIndexOrId].recid;
            }
        }
        else if (action === VIS.VTable.prototype.ROW_ADD) {
            id = args.recid; // ro
        }
    }
    if (id) {
        this.navigate(id);
    }
};

VCardView.prototype.setupCardView = function (aPanel, mTab, cContainer, vCardId) {
    this.mTab = mTab;
    // this.aPanel = aPanel;
    var self = this;
    VIS.dataContext.getCardViewInfo(mTab.getAD_Window_ID(), mTab.getAD_Tab_ID(), function (retData) {
        //init 
        //var retData = {};
        // retData.Group = "AccessLevel" //C_UOM_ID";
        // retData.InCol = ['Name', 'Description', 'Help', 'C_UOM_ID']
        // retData.Conditions = [];
        self.setCardViewData(retData);

    });

    //this.createGroups();
    //  cContainer.append(this.getCardList()).append(this.getHeader());
    cContainer.append(this.getHeader());
    cContainer.append(this.getRoot());
};

VCardView.prototype.setCardViewData = function (retData) {
    this.hasIncludedCols = false;
    this.fields = [];
    this.cGroup = null;
    this.cConditions = [];
    if (retData) {

        this.AD_CardView_ID = retData.AD_CardView_ID;

        var f = this.mTab.getFieldById(retData.FieldGroupID)
        if (f) {
            this.cGroup = f;
        }
        // self.cCols = retData.IncludedCols;
        this.cConditions = retData.Conditions;

        for (var i = 0; i < retData.IncludedCols.length; i++) {

            var f = this.mTab.getFieldById(retData.IncludedCols[i]);
            if (f)
                this.fields.push(f);

            this.hasIncludedCols = true;
        }
    }

    if (this.fields.length < 1) {

        //Check for Included column
        // var aFields = this.mTab.gridTable.gridFields.slice(); //copy of array
        //var f = null;
        //while (aFields.length > 0) {
        //    f = aFields.pop();
        //    if (f.getIsIncludedColumn()) {
        //        this.fields.unshift(f);
        //    }
        //}

        if (this.fields.length < 1) {
            f = this.mTab.getField('Name');
            if (f) {
                this.fields.push(f);
            }
            var f = this.mTab.getField('Description');
            if (f) {
                this.fields.push(f);
            }
            var f = this.mTab.getField('Help');
            if (f) {
                this.fields.push(f);
            }
        }
    }
    for (var p in this.cGroupInfo) {
        this.cGroupInfo[p].records.length = 0;
    }
    this.cGroupInfo = {};
    this.grpCount = 0;

    this.isProcessed = false;
};

VCardView.prototype.createGroups = function () {
    //1 get Grup field
    // var groupName = [];
    //  var groupValue = [];

    if (this.isProcessed) {
        for (var p in this.cGroupInfo) {
            this.cGroupInfo[p].records = [];
        }
        return;
    }

    if (this.cGroup) {

        var field = this.cGroup;
        if (field) {
            if (field.getDisplayType() == VIS.DisplayType.YesNo) {
                this.cGroupInfo['true'] = { 'name': 'Yes', 'records': [] };
                this.cGroupInfo['false'] = { 'name': 'No', 'records': [] };
                this.grpCount = 2;
            }
            else if (VIS.DisplayType.IsLookup(field.getDisplayType()) && field.getLookup()) { //TODO: check validated also

                //getlookup
                var lookup = field.getLookup();
                lookup.fillCombobox(true, true, true, false);

                var data = lookup.data;

                for (var i = 0; i < data.length; i++) {
                    this.cGroupInfo[data[i].Key] = { 'name': data[i].Name, 'records': [] };
                    this.grpCount += 1;
                    //if (i >= 4)
                    //    break;
                }
            }
            this.setHeader(field.getHeader());
        }
        else {
            this.setHeader('');
        }
    }
    if (this.grpCount < 1) {//add one group by de
        this.cGroupInfo['All'] = { 'name': VIS.Msg.getMsg('All'), 'records': [] };
        this.grpCount = 1;
    }
    this.isProcessed = true;
};

VCardView.prototype.refreshUI = function (width) {

    this.createGroups();

    var records = this.mTab.getTableModel().mSortList;


    var root = this.getBody();

    while (this.groupCtrls.length > 0) {
        this.groupCtrls.pop().dispose();
    }

    this.groupCtrls.length = 0;

    root.empty();

    var cardGroup = null;
    if (this.grpCount == 1) {

        var n = '';
        for (var p in this.cGroupInfo) {
            n = this.cGroupInfo[p].name;
            break;
        }

        cardGroup = new VCardGroup(true, records, n, this.fields, this.cConditions);
        this.groupCtrls.push(cardGroup);
        root.append(cardGroup.getRoot())
    }
    else {
        this.filterRecord(records);
        for (var p in this.cGroupInfo) {
            cardGroup = new VCardGroup(this.grpCount === 1, this.cGroupInfo[p].records, this.cGroupInfo[p].name, this.fields, this.cConditions);
            this.groupCtrls.push(cardGroup);
            root.append(cardGroup.getRoot())
        }
    }
    this.calculateWidth(width);
};

VCardView.prototype.filterRecord = function (records) {
    var len = records.length;

    var grpCol = this.cGroup.getColumnName().toLowerCase();
    var record = null;
    var colValue = null;

    for (var i = 0; i < len; i++) {
        record = records[i];

        colValue = record[grpCol];
        if (this.cGroupInfo[colValue])
            this.cGroupInfo[colValue].records.push(record);
        else {
            if (!this.cGroupInfo['Other__1']) {
                this.cGroupInfo['Other__1'] = { 'name': 'Others', 'records': [] };
                this.grpCount += 1;
            }
            this.cGroupInfo['Other__1'].records.push(record);
        }
    }

    var eCols = [];
    for (var p in this.cGroupInfo) {
        if (this.cGroupInfo[p].records.length < 1) {
            eCols.push(p);
        }
    }
    this.grpCount -= eCols.length;

    while (eCols.length > 0) {
        delete this.cGroupInfo[eCols.pop()];
    }
};

VCardView.prototype.dispose = function () {
    this.dC();
};

/* Group Control */
function VCardGroup(onlyOne, records, grpName, fields, conditions) {

    //conditions = [{ 'bgColor': '#80ff80', 'cValue': '@AD_User_ID@=1005324 & @C_DocTypeTarget_ID@=132' }];

    var root = null;
    var body;
    var cards = [];

    function init() {
        var str = "<div class='vis-cv-cg vis-pull-left'> <div class='vis-cv-head' >" + grpName
            + "</div><div class='vis-cv-grpbody'></div></div>";
        root = $(str);
        body = root.find('.vis-cv-grpbody');
        if (onlyOne) {
            root.css({ 'margin-right': '0px', 'width': '100%' });
            //root.width('100%');
        }
    };
    init();


    function createCards() {
        var card = null;
        for (var i = 0; i < records.length; i++) {
            card = new VCard(fields, records[i]);
            if (onlyOne) {
                card.getRoot().width("240px").css({ 'margin': '5px 12px 12px 5px', 'float': (VIS.Application.isRTL ? 'right' : 'left') });
            }
            cards.push(card);
            body.append(card.getRoot());
            card.evaluate(conditions)
        }
    };
    createCards();

    this.getRoot = function () {
        return root;
    };

    this.getBody = function () {
        return body;
    };

    this.setWidth = function (w) {
        root.width(w);
    };

    this.dC = function () {
        while (cards.length > 0) {
            cards.pop().dispose();
        }
        root.remove();
        root = null;
        body = null;
        this.getBody = null;
        this.getRoot = null;
    };

};

VCardGroup.prototype.dispose = function () {
    this.dC();
};

/* Card View Control */
function VCard(fields, record) {
    this.record = record;

    var root = $('<div class="vis-cv-card" data-recid=' + record.recid + ' name = vc_' + record.recid + ' ></div>');

    //root.append($("<i class='pin'></i>"));
    // root.append($('<span class="glyphicon glyphicon-hand-down vis-cv-card-selected"></span>'));
    var pencil = $('<span class="glyphicon glyphicon-pencil vis-cv-card-edit vis-pull-right" data-recid=' + record.recid + '></span>');
    root.append(pencil);

    var field = null;
    var dt;
    //createview
    for (var i = 0; i < fields.length; i++) {
        field = fields[i];
        var value = record[field.getColumnName().toLowerCase()];
        dt = field.getDisplayType();

        if (VIS.DisplayType.IsLookup(dt)) {
            value = field.getLookup().getDisplay(value);
        }

        else if (VIS.DisplayType.YesNo == dt) {
            if (value || value == 'Y')
                value = VIS.Msg.getMsg('Yes');
            else
                value = value = VIS.Msg.getMsg('No');
        }

        else if (VIS.DisplayType.IsDate(dt)) {
            if (value) {
                // JID_1826 Date is showing as per browser culture
                var d = new Date(value);
                if (dt == VIS.DisplayType.Date)
                    value = d.toLocaleDateString();
                //value = Globalize.format(new Date(value), 'd');
                else if (dt == VIS.DisplayType.DateTime)
                    value = d.toDateString();
                //value = Globalize.format(new Date(value), 'f');
                else
                    value = d.toLocaleTimeString();
                //value = Globalize.format(new Date(value), 't');
            }
            else value = null;
        }
        // JID_1826 Amount is showing as per browser culture
        else if (VIS.DisplayType.Amount == dt) {
            var val = VIS.Utility.Util.getValueOfDecimal(value);
            value = (val).toLocaleString();
        }
        // JID_1826 Quantity is showing as per browser culture
        else if (VIS.DisplayType.Quantity == dt) {
            var val = VIS.Utility.Util.getValueOfDecimal(value);
            value = (val).toLocaleString();
        }
        if (!value && value != 0)
            value = ' -- ';
        value = w2utils.encodeTags(value);

        if (field.getIsEncryptedField()) {
            value = value.replace(/\w|\W/g, "*");
        }
        if (field.getObscureType()) {
            value = VIS.Env.getObscureValue(field.getObscureType(), value);
        }

        var span = "";
        //if (i != 0)
        //    span = "<p>" + field.getHeader() + " : <strong>" + value + "<strong></p>";
        //else
        if (VIS.Application.isRTL)
            span = "<p><strong title='" + value + "'>" + value + "</strong> :" + field.getHeader() + "</p>";
        else
            span = "<p>" + field.getHeader() + ": <strong title='" + value + "'>" + value + "</strong></p>";

        root.append($(span));
    };

    this.setColor = function (bc, fc) {
        if (bc)
            root.css('background-color', bc);
        if (fc)
            root.css('color', bc);
    };





    pencil.on('touchstart', function () {
        $(this).css({ 'color': 'gray' });
    });

    this.getRoot = function () {
        return root;
    };

    this.setWidth = function (w) {
        root.width(w)
    };

    this.dC = function () {
        pencil.off('touchstart mouseover');
        root.remove();
        root = null;
        this.getRoot = null;
        this.dc = null;
    };
};

VCard.prototype.evaluate = function (cnds) {
    //if (!cnds)
    //{
    //    return;
    //}
    var c = null;
    for (var i = 0; i < cnds.length; i++) {
        c = cnds[i];
        if (!c.ConditionValue)
            continue;
        if (VIS.Evaluator.evaluateLogic(this, c.ConditionValue)) {
            this.setColor(c.Color, c.FColor);
        }
    }
};

VCard.prototype.getValueAsString = function (vName) {
    var val = this.record[vName.toLowerCase()];
    if (val) {
        if (val === true) {
            val = 'Y';
        }
        else if (val && val.toString().endsWith('.000Z')) {

            val = val.replace('.000Z', 'Z');
        }
        val = val.toString();
    }
    else if (val === false)
        val = 'N';
    else if (val === 0) {
        val = '0';
    }

    return val;
};

VCard.prototype.dispose = function () {
    this.dC();
    };

    VIS.VCardView = VCardView;

}(VIS, jQuery));
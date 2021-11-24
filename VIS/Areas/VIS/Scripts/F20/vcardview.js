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
        this.oldGrpCount = 0;
        this.grpColName = '';
        this.hasIncludedCols = false;
        // this.aPanel;
        this.onCardEdit = null;

        var root;
        var body = null;
        var headerdiv;
        var $cmbCards = null;
        var $lblGroup = null;
        var $btnClrSearch = null;
        var $imgdownSearch = null;
        var self = this;
        this.isAutoCompleteOpen = false;

        //  var cardList;
        function init() {
            root = $("<div class='vis-cv-body vis-noselect'>");
            body = $("<div class='vis-cv-main'>");
            headerdiv = $("<div class='vis-cv-header'>");
            $cmbCards = $('<input  class="vis-vs-card-autoComplete" style="display:inline">')
            $lblGroup = $('<p>');
            $imgdownSearch = $('<span class="vis-ad-w-p-tb-s-icon-down vis-cv-cardlist"><i class="fa fa-ellipsis-h"></i></span>');
            headerdiv.append($cmbCards).append($imgdownSearch).append($lblGroup);
            root.append(headerdiv).append(body);
            createCardautoComplete();
        }

        /**
         * create autocomplete box to show list of cards
         * */
        function createCardautoComplete() {
            $cmbCards.autocomplete({
                select: function (ev, ui) {
                    cardChanged(ui.item.id, ui.item.label);
                    var currentTarget = $(ev.currentTarget);

                    currentTarget.find('.vis-cv-card-selected-card').removeClass('vis-cv-card-selected-card');

                    currentTarget.find('[data-checkid="' + ui.item.id + '"]').addClass('vis-cv-card-selected-card');

                    ev.stopPropagation();
                },
                minLength: 0,
                open: function (ev, ui) {
                    self.isAutoCompleteOpen = true;
                },
                close: function (event, ui) {
                    //$imgdownSearch.css("transform", "rotate(360deg)");
                    window.setTimeout(function () {
                        self.isAutoCompleteOpen = false;
                    }, 600);
                },
                source: []
            });

            /*
             * Handled render event to show make default icon in menu
             */
            $cmbCards.autocomplete().data('ui-autocomplete')._renderItem = function (ul, item) {

                var span = null;
                var tickSpan = null;
                if (item.isDefault == 'Y') {
                    span = $("<span title='" + VIS.Msg.getMsg("DefaultCard") + "'  data-id='" + item.id + "' class='VIS-winSearch-defaultIcon'></span>");

                }
                else {
                    span = $("<span title='" + VIS.Msg.getMsg("MakeDefaultCard") + "' data-id='" + item.id + "' class='VIS-winSearch-NonDefaultIcon'></span>");
                }


                tickSpan = $("<span class='fa fa-check-circle vis-cv-card-selected-card-listitem'></span>");

                var li = null;
                if (self.AD_CardView_ID == item.id) {
                    li = $("<li>")
                        .append($("<a  data-checkid='" + item.id + "'  class='vis-cv-card-selected-card' style='display:block' title='" + item.title + "'></a>").append(tickSpan).append("<p>" + item.label + "</p>").append(span))
                        .prependTo(ul);
                }
                else {
                    li = $("<li>")
                        .append($("<a  data-checkid='" + item.id + "'  style='display:block' title='" + item.title + "'></a>").append(tickSpan).append("<p>" + item.label + "</p>").append(span))
                        .prependTo(ul);
                }

                // When user clicks on make default icon, then save details in DB.
                span.on("click", function (e) {
                    var cardID = $(this).data('id');
                    $.ajax({
                        url: VIS.Application.contextUrl + "JsonData/InsertUpdateDefaultCard",
                        dataType: "json",
                        data: { AD_Tab_ID: self.mTab.getAD_Tab_ID(), AD_Card_ID: cardID },
                        success: function (data) {

                        },
                        error: function (er) {
                            console.log(er);
                        }

                    });
                });

                return li;
            };
        };

        init();
        //eventHandle();

        this.getRoot = function () {
            return root;
        };

        this.getBody = function () {
            return body;
        };

        this.setHeader = function (txt) {
            $lblGroup.text(txt);
        }

        this.getHeader = function () {
            return headerdiv;
        }

        this.sizeChanged = function (h, w) {
            root.height((h - 12) + 'px');
            this.calculateWidth(w);
        }

        this.calculateWidth = function (width) {
            //set width
            //width = (width - 6);
            //var grpCtrlC = this.groupCtrls.length;
            //if (grpCtrlC < 1)
            //    return;
            //if (width) {

            //    var tGrpW = 300 * grpCtrlC;
            //    if (tGrpW > width) {
            //        body.width(300 * grpCtrlC);
            //    }
            //    else {
            //        body.width(width);
            //        var newW = Math.ceil(width / grpCtrlC) - 24;
            //        while (grpCtrlC > 0) {
            //            --grpCtrlC;

            //            this.groupCtrls[grpCtrlC].setWidth(newW);
            //        }
            //    }
            //}
            //else
            //    body.width(body.parent().width() * (grpCtrlC));
            this.navigate();
        };

        body.on('mousedown touchstart', 'div.vis-cv-card', function (e) {

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

        this.getCardCmb = function () {
            return $cmbCards;
        };

        var handleEvents = function () {
            $cmbCards.one("focus", loadCards);
            $cmbCards.on("change", cardChanged);
            $imgdownSearch.on("click", function () {
                //if (!self.isAutoCompleteOpen) {

                loadCards();
                //}
                // else {
                //$imgdownSearch.css("transform", "rotate(360deg)");
                // }
            });
        };

        /**
         * Load list of cards for current tab and current user
         * */
        var loadCards = function () {
            var res = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "JsonData/GetCardsInfo", { AD_Tab_ID: self.mTab.getAD_Tab_ID() });
            if (res) {
                self.fillCardViewList(res, true);
            }
        };

        /**
         * When user change caard from drowdown, then fetch details of card and show card
         * @param {any} cardID
         * @param {any} cardName
         */
        var cardChanged = function (cardID, cardName) {
            self.getCardViewData(self.mTab, cardID, cardName);
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
            $cmbCards.remove();
            $cmbCards = null;
            $imgdownSearch.remove();
            $imgdownSearch = null;

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

        /**
         * Display cards in auto complete dropdown
         * @param {any} cards
         */
        this.fillCardViewList = function (cards, showData) {
            $cmbCards.empty();
            var userQueries = [];
            //$imgdownSearch.css('visibility', 'visible');
            if (cards && cards.length > 0) {
                //headerdiv.show();
                for (var i = 0; i < cards.length; i++) {
                    // $cmbCards.append('<option value="' + cards[i].AD_CardView_ID + '">' + cards[i].Name + '</option>');
                    if (cards[i].IsDefault) {
                        userQueries.push({ 'title': cards[i].Name, 'label': cards[i].Name, 'value': cards[i].Name, 'id': cards[i].AD_CardView_ID, 'isDefault': 'Y' });
                    }
                    else {
                        userQueries.push({ 'title': cards[i].Name, 'label': cards[i].Name, 'value': cards[i].Name, 'id': cards[i].AD_CardView_ID, 'isDefault': 'N' });
                    }
                }
                $cmbCards.autocomplete('option', 'source', userQueries, "position", { my: "left top", at: "left bottom" });
                //$imgdownSearch.css("transform", "rotate(180deg)");
                if (showData) {
                    //window.setTimeout(function () {

                    //$cmbCards.trigger("focus");
                    $cmbCards.autocomplete("search", "");
                    //}, 400);

                }

                if (this.AD_CardView_ID) {
                    $cmbCards.val(this.cardName);
                }
            }
            else {
                //headerdiv.hide();
            }
        };

        handleEvents();
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
        this.aPanel = aPanel;

        this.fillCardViewList(mTab.vo.Cards);

        // this.getCardViewData(mTab, 0);
        var cardss = mTab.vo.Cards[0];

        this.setCardViewData(cardss);
        this.refreshUI(this.getBody().width());

        //cContainer.append(this.getHeader());
        cContainer.append(this.getRoot());
    };

    /**
     * When user change card from drowdown, then fetch details of card and show card
     * @param {any} mTab
     * @param {any} cardID
     * @param {any} cardName
     */
    VCardView.prototype.getCardViewData = function (mTab, cardID, cardName) {
        var self = this;
        var windowID = 0;
        var tabID = 0;
        if (mTab) {
            windowID = mTab.getAD_Window_ID();
            tabID = mTab.getAD_Tab_ID();
        }

        VIS.dataContext.getCardViewInfo(windowID, tabID, cardID, function (retData) {
            self.setCardViewData(retData);
            self.refreshUI(self.getBody().width());
            if (cardID) {
                self.getCardCmb().val(retData.Name);
            }

        });
    };

    VCardView.prototype.setCardViewData = function (retData) {
        this.hasIncludedCols = false;
        this.fields = [];
        this.cGroup = null;
        this.cConditions = [];
        this.headerItems = {};
        if (retData) {
            // this.getHeader().show();
            this.AD_CardView_ID = retData.AD_CardView_ID;
            //$cmbCards.autocomplete('option', 'source', userQueries);
            //$cmbCards.autocomplete("search", "");
            //$cmbCards.trigger("focus");
            this.cardName = retData.Name;
            this.getCardCmb().val(retData.Name);

            var f = this.mTab.getFieldById(retData.FieldGroupID)
            if (f) {
                this.cGroup = f;
            }
            // self.cCols = retData.IncludedCols;
            this.cConditions = retData.Conditions;

            this.headerItems = retData.HeaderItems;

            this.headerStyle = retData.Style;
            this.headerPadding = retData.Padding;

            for (var i = 0; i < retData.IncludedCols.length; i++) {
                var f = this.mTab.getFieldById(retData.IncludedCols[i].AD_Field_ID);

                //f.setCardIconHide(retData.IncludedCols[i].HideIcon);
                //f.setCardTextHide(retData.IncludedCols[i].HideText);
                if (f) {
                    f.setCardViewSeqNo(retData.IncludedCols[i].SeqNo);
                    f.setCardFieldStyle(retData.IncludedCols[i].HTMLStyle);
                    this.fields.push(f);
                    //if (!retData.headerItems || retdata.headerItems.length == 0) {
                    //    f.setCardIconHide(retData.IncludedCols[i].HideIcon);
                    //    f.setCardTextHide(retData.IncludedCols[i].HideText);
                    //}
                    this.hasIncludedCols = true;
                }

            }
        }

        if (this.fields.length < 1) {
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
            this.cGroupInfo = [];
            this.grpCount = 0;
            var field = this.cGroup;
            if (field) {
                if (field.getDisplayType() == VIS.DisplayType.YesNo) {

                    this.cGroupInfo['true'] = { 'name': 'Yes', 'records': [], 'key': true };
                    this.cGroupInfo['false'] = { 'name': 'No', 'records': [], 'key': false };

                    this.grpCount = 2;
                }
                else if (VIS.DisplayType.IsLookup(field.getDisplayType()) && field.getLookup()) { //TODO: check validated also

                    //getlookup
                    var lookup = field.getLookup();
                    lookup.fillCombobox(true, true, true, false);

                    var data = lookup.data;

                    for (var i = 0; i < data.length; i++) {
                        this.cGroupInfo[data[i].Key] = { 'name': data[i].Name, 'records': [], 'key': data[i].Key };
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
        else { this.setHeader(''); }
        if (this.grpCount < 1 || $.isEmptyObject(this.cGroupInfo)) {//add one group by de
            this.cGroupInfo['All'] = { 'name': VIS.Msg.getMsg('All'), 'records': [], 'key': null };
            this.grpCount = 1;
        }
        this.isProcessed = true;
    };

    VCardView.prototype.refreshUI = function (width) {

        var $this = this;
        window.setTimeout(function () {
            if (width == 0) {
                width = $this.getBody().width();
            }
            $this.isProcessed = false;
            $this.createGroups();

            var records = $this.mTab.getTableModel().mSortList;

            var root = $this.getBody();

            while ($this.groupCtrls.length > 0) {
                $this.groupCtrls.pop().dispose();
            }

            $this.groupCtrls.length = 0;

            root.empty();

            var cardGroup = null;
            if ($this.grpCount == 1) {

                var n = '';
                var key = null;
                for (var p in $this.cGroupInfo) {
                    n = VIS.Utility.Util.getIdentifierDisplayVal($this.cGroupInfo[p].name);
                    key = $this.cGroupInfo[p].key;
                    break;
                }

                cardGroup = new VCardGroup(true, records, n, $this.fields, $this.cConditions, $this.headerItems, $this.headerStyle, $this.headerPadding, key, $this.aPanel);
                $this.groupCtrls.push(cardGroup);
                root.append(cardGroup.getRoot())
            }
            else {
                $this.filterRecord(records);
                for (var p in $this.cGroupInfo) {
                    cardGroup = new VCardGroup($this.grpCount === 1, $this.cGroupInfo[p].records, VIS.Utility.Util.getIdentifierDisplayVal($this.cGroupInfo[p].name), $this.fields, $this.cConditions, $this.headerItems, $this.headerStyle, $this.headerPaddings, $this.cGroupInfo[p].key, $this.aPanel);
                    $this.groupCtrls.push(cardGroup);
                    root.append(cardGroup.getRoot());
                    var sortable = new vaSortable(cardGroup.getBody()[0], {
                        attr: 'data-recid',
                        selfSort: false,
                        ignore: ['.vis-cv-card-edit', '.vis-ev-col-wrap-button'],
                        onSelect: function (e, item) {
                            //$this.onCardEdit({ 'recid': item }, true);
                            var obj = {
                                grpValue: $(e).parent().attr('data-key'),
                                recordID: $this.mTab.getRecord_ID(),
                                columnName: $this.cGroup.getColumnName(),
                                tableName: $this.mTab.getTableName(),
                                dataType: $this.cGroup.getDisplayType()
                            }
                            $.ajax({
                                type: "POST",
                                url: VIS.Application.contextUrl + "CardView/UpdateCardByDragDrop",
                                dataType: "json",
                                data: obj,
                                success: function (data) {
                                    if (data < 1) {
                                        vaSortable.prototype.revertItem();
                                    } else {
                                        $this.mTab.dataRefresh();
                                        var rec = $.grep(records, function (element, index) {
                                            return element.recid == item;
                                        });
                                        var changeCard = new VCard($this.fields, rec[0], $this.headerItems, $this.headerStyle, $this.headerPadding, windowNo, {}, $this.aPanel)
                                        root.find("[name='vc_" + item + "']").replaceWith(changeCard.getRoot());
                                        changeCard.evaluate($this.cConditions)
                                    }
                                },
                                error: function (err) {
                                    vaSortable.prototype.revertItem();
                                }
                            });
                        }
                    });
                }
            }

            $this.calculateWidth(width);

        }, 10);

    };

    VCardView.prototype.filterRecord = function (records) {

        if (!records)
            return;

        var len = records.length;
        var grpCol = this.cGroup.getColumnName().toLowerCase();
        var record = null;
        var colValue = null;
        var isgrouprChanged = false;
        for (var i = 0; i < len; i++) {
            record = records[i];

            colValue = record[grpCol];
            if (this.cGroupInfo[colValue]) {
                this.cGroupInfo[colValue].records.push(record);
                isgrouprChanged = true;
            }
            else {
                if (!this.cGroupInfo['Other__1']) {
                    this.cGroupInfo['Other__1'] = { 'name': 'Others', 'records': [] };
                    this.grpCount += 1;
                    isgrouprChanged = true;
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

        if (this.oldGrpCount != this.grpCount || isgrouprChanged)
            this.isProcessed = false;

        this.oldGrpCount = this.grpCount;
    };

    VCardView.prototype.dispose = function () {
        this.dC();
    };

    /* Group Control */
    function VCardGroup(onlyOne, records, grpName, fields, conditions, headerItems, headerStyle, headerPadding, key, aPanel) {

        //conditions = [{ 'bgColor': '#80ff80', 'cValue': '@AD_User_ID@=1005324 & @C_DocTypeTarget_ID@=132' }];
        var root = null;
        var body;
        var cards = [];
        windowNo = VIS.Env.getWindowNo();
        function init() {
            var str = "<div class='vis-cv-cg vis-pull-left'> <div class='vis-cv-head' >" + grpName
                + "</div><div data-key='" + key + "'  class='vis-cv-grpbody'></div></div>";
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
            this.fieldStyles = {};
            if (!records)
                return;
            for (var i = 0; i < records.length; i++) {
                card = new VCard(fields, records[i], headerItems, headerStyle, headerPadding, windowNo, this.fieldStyles, aPanel);
                if (onlyOne) {
                    card.getRoot().width("240px").css({ 'margin': '5px 12px 12px 5px', 'float': (VIS.Application.isRTL ? 'right' : 'left') });
                }
                if (i == 0) {
                    card.addStyleToDom();
                }
                cards.push(card);
                body.append(card.getRoot());
                card.evaluate(conditions)
            }
        };


        this.getRoot = function () {
            return root;
        };

        this.getBody = function () {
            return body;
        };

        this.setWidth = function (w) {
            root.width(w);
        };

        createCards();

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
    function VCard(fields, record, headerItems, headerStyle, headerPadding, windowNo, fieldStyles, aPanel) {
        this.record = record;
        this.aPanel = aPanel;
        this.dynamicStyle = [];
        this.textAlignEnum = { "C": "Center", "R": "flex-end", "L": "flex-start" };
        this.alignItemEnum = { "C": "Center", "T": "flex-start", "B": "flex-end" };
        this.dynamicStyle = [];
        this.styleTag = document.createElement('style');
        this.windowNo = windowNo;
        this.fieldStyles = fieldStyles;


        var root = $('<div class="vis-cv-card va-dragdrop" data-recid=' + record.recid + ' name = vc_' + record.recid + ' ></div>');

        //root.append($("<i class='pin'></i>"));
        // root.append($('<span class="glyphicon glyphicon-hand-down vis-cv-card-selected"></span>'));
        var pencil = $('<span class="glyphicon glyphicon-pencil vis-cv-card-edit vis-pull-right" data-recid=' + record.recid + '></span>');
        root.append(pencil);

        var field = null;
        var dt;
        //createview

        var $root = $('<div class="vis-ad-w-p-card_root_common">');
        root.append($root);

        if (!this.fieldStyles["vis-ad-w-p-card-Custom_" + windowNo])
            this.fieldStyles["vis-ad-w-p-card-Custom_" + windowNo] = {};
        this.headerCustom = this.fieldStyles["vis-ad-w-p-card-Custom_" + windowNo]["headerParentCustomUISettings"];
        if (!this.headerCustom) {
            this.headerCustom = this.headerParentCustomUISettings(headerStyle);
            this.fieldStyles["vis-ad-w-p-card-Custom_" + windowNo]["headerParentCustomUISettings"] = this.headerCustom;
        }
        root.addClass(this.headerCustom);

        if (!this.fieldStyles["root_" + windowNo])
            this.fieldStyles["root_" + windowNo] = {};
        this.rootCustomStyle = this.fieldStyles["root_" + windowNo]['headerUISettings'];
        if (!this.rootCustomStyle) {
            this.rootCustomStyle = this.headerUISettings("", headerPadding);
            this.fieldStyles["root_" + windowNo]['headerUISettings'] = this.rootCustomStyle;
        }
        $root.addClass(this.rootCustomStyle);



        this.setHeaderItems = function (currentItem, $containerDiv, fields, record) {

            /*If controls are already loaded, then do not manipulate DOM.Only fetch there reference from DOM and Change Values.
             *Else create header panel items. 
             */
            if (!currentItem)
                return;

            //loop through header item
            var headergFields = null;
            for (var headerSeqNo in currentItem.HeaderItems) {

                var headerItem = currentItem.HeaderItems[headerSeqNo];

                var startCol = headerItem.StartColumn;
                var colSpan = headerItem.ColumnSpan;
                var startRow = headerItem.StartRow;
                var rowSpan = headerItem.RowSpan;
                var justyFy = headerItem.JustifyItems;
                var alignItem = headerItem.AlignItems;
                var fieldPadding = headerItem.Padding;
                var backgroundColor = headerItem.BackgroundColor;
                var hideFieldIcon = headerItem.HideFieldIcon;
                var hideFieldText = headerItem.HideFieldText;
                var fieldValueStyle = headerItem.FieldValueStyle;

                if (!backgroundColor) {
                    backgroundColor = '';
                }
                var FontColor = headerItem.FontColor;
                if (!FontColor) {
                    FontColor = '';
                }
                var fontSize = headerItem.FontSize;
                if (!fontSize) {
                    fontSize = '';
                }
                var $div = null;
                var $divIcon = null;
                //$divIconSpan = $('<span>');
                //$divIconImg = $('<img>');
                var $divLabel = null;
                var $label = null;
                var iControl = null;
                var gridLayout_ID = currentItem.AD_GridLayout_ID;
                if (!this.fieldStyles[startCol + '_' + colSpan + '_' + startRow + '_' + rowSpan + '_' + gridLayout_ID])
                    this.fieldStyles[startCol + '_' + colSpan + '_' + startRow + '_' + rowSpan + '_' + gridLayout_ID] = {};
                //Apply HTML Style
                this.dynamicClassName = this.fieldStyles[startCol + '_' + colSpan + '_' + startRow + '_' + rowSpan + '_' + gridLayout_ID]['applyCustomUISettings'];
                if (!this.dynamicClassName) {
                    this.dynamicClassName = this.applyCustomUISettings(headerSeqNo, startCol, colSpan, startRow, rowSpan, justyFy, alignItem,
                        backgroundColor, FontColor, fontSize, fieldPadding);
                    this.fieldStyles[startCol + '_' + colSpan + '_' + startRow + '_' + rowSpan + '_' + gridLayout_ID]['applyCustomUISettings'] = this.dynamicClassName;
                }

                // Find the div with dynamic class from container. Class will only be available in DOm if two fields are having same item seq. No.
                $div = $containerDiv.find('.' + this.dynamicClassName);

                //If div not found, then create new one.
                if ($div.length <= 0)
                    $div = $('<div class="vis-w-p-card-data-f ' + this.dynamicClassName + '">');




                // is dynamic 
                if (headerItem.ColSql.length > 0) {
                    var controls = {};
                    $divLabel = $('<div class="vis-w-p-header-Label-f"></div>');
                    iControl = new VIS.Controls.VKeyText(headerItem.ColSql, this.windowNo,
                        this.windowNo + "_" + headerSeqNo);

                    if (iControl == null) {
                        continue;
                    }

                    controls["control"] = iControl;
                    var objctrls = { "control": controls, "field": null };

                    $divLabel.append(iControl.getControl());
                    iControl.setValue();
                    fieldValueStyle = headerItem.FieldValueStyle;
                    if (fieldValueStyle)
                        $divLabel.attr('style', fieldValueStyle);
                    $div.append($divLabel);
                    // $div.append($divLabel);
                    $containerDiv.append($div);
                    //$self.controls.push(objctrls);
                }
                else {
                    if (!headergFields) {
                        headergFields = {};
                        for (var i = 0; i < fields.length; i++) {
                            var field = fields[i];
                            if (field.getCardViewSeqNo() in headergFields) {
                                headergFields[field.getCardViewSeqNo()].push(field);
                            }
                            else {
                                headergFields[field.getCardViewSeqNo()] = [field];
                            }
                            //}
                        }
                    }

                    var mFields = headergFields[headerSeqNo];
                    if (!mFields)
                        continue;
                    for (var x = 0; x < mFields.length; x++) {
                        var mField = mFields[x];
                        if (!mField)
                            continue;
                        if (mField.getCardFieldStyle())
                            fieldValueStyle = mField.getCardFieldStyle();

                        mField.setCardIconHide(hideFieldIcon);
                        mField.setCardTextHide(hideFieldText);

                        if (!this.fieldStyles[mField.getColumnName()])
                            this.fieldStyles[mField.getColumnName()] = {}
                        var controls = {};
                        $divIcon = $('<div class="vis-w-p-card-icon-f"></div>');

                        $divLabel = $('<div class="vis-w-p-card-Label-f"></div>');
                        // If Referenceof field is Image then added extra class to align image and Label in center.
                        if (mField.getDisplayType() == VIS.DisplayType.Image) {
                            $divLabel.addClass('vis-w-p-card-Label-center-f');

                            if (!this.fieldStyles[mField.getColumnName() + 'justifyAlignImageItems'])
                                this.fieldStyles[mField.getColumnName() + 'justifyAlignImageItems'] = {};

                            this.dynamicClassForImageJustyfy = this.fieldStyles[mField.getColumnName() + 'justifyAlignImageItems']['justifyAlignImageItems'];
                            if (!this.dynamicClassForImageJustyfy) {
                                this.dynamicClassForImageJustyfy = this.justifyAlignImageItems(headerSeqNo, justyFy, alignItem);
                                this.fieldStyles[mField.getColumnName() + 'justifyAlignImageItems'] = { 'justifyAlignImageItems': this.dynamicClassForImageJustyfy };
                            }
                            $divLabel.addClass(this.dynamicClassForImageJustyfy);
                        }

                        // Get Controls to be displayed in Header Panel
                        $label = VIS.VControlFactory.getHeaderLabel(mField, true);
                        iControl = VIS.VControlFactory.getReadOnlyControl(this.curTab, mField, false, false, false);

                        if (mField.getDisplayType() == VIS.DisplayType.Button) {
                            if (iControl != null)
                                iControl.addActionListner(this);
                        }

                        if (!this.fieldStyles[mField.getColumnName() + 'applyCustomUIForFieldValue'])
                            this.fieldStyles[mField.getColumnName() + 'applyCustomUIForFieldValue'] = {};

                        this.dynamicFieldValue = this.fieldStyles[mField.getColumnName() + 'applyCustomUIForFieldValue']['applyCustomUIForFieldValue'];
                        if (!this.dynamicFieldValue) {
                            this.dynamicFieldValue = this.applyCustomUIForFieldValue(headerSeqNo, startCol, startRow, mField, fieldValueStyle);
                            this.fieldStyles[mField.getColumnName() + 'applyCustomUIForFieldValue'] = { 'applyCustomUIForFieldValue': this.dynamicFieldValue };
                        }
                        iControl.getControl().addClass(this.dynamicFieldValue);

                        // Create object of controls and push object and Field in Array
                        // THis array is used when user navigate from one record to another.
                        controls["control"] = iControl;

                        var objctrls = { "control": controls, "field": mField };

                        var $spanIcon = $('<span></span>');
                        var icon = VIS.VControlFactory.getIcon(mField);
                        if (iControl == null) {
                            continue;
                        }

                        var $lblControl = null;
                        if ($label) {
                            $lblControl = $label.getControl().addClass('vis-w-p-card-data-label');
                        }

                        var colValue = getFieldValue(mField, record);




                        setFieldLayout(fieldValueStyle, $div, $divIcon, $divLabel);
                        var $image = $('<img>');
                        var $imageSpan = $('<span>');
                        objctrls["img"] = $image;


                        if (mField.lookup && mField.lookup.gethasImageIdentifier()) {


                            objctrls["imgspan"] = $imageSpan;

                            var img = null;
                            var imgSpan = null;
                            var styleArr = null;
                            if (VIS.DisplayType.List == mField.lookup.displayType) {

                                img = mField.lookup.getLOVIconElement(record[mField.getColumnName().toLower()], true);
                                if (!img && colValue) {
                                    imgSpan = colValue.substring(0, 1);
                                    img = imgSpan;
                                }
                            }
                            else {
                                colValue = VIS.Utility.Util.getIdentifierDisplayVal(colValue);
                                img = getIdentifierImage(mField, record);
                            }
                            if (img && !img.contains("Images/")) {
                                imgSpan = img;//img contains First charater of Name or Identifier text
                                $imageSpan.append(imgSpan);
                            }
                            else {
                                $image.attr('src', img);
                            }

                            $divIcon.append($imageSpan);
                            $divIcon.append($image);

                            /*Set what do you want to show? Icon OR Label OR Both OR None*/
                            setFieldVisibility(mField, imgSpan, $image, $imageSpan, $lblControl, $divLabel, $divIcon);

                            $divLabel.append(iControl.getControl());

                            $containerDiv.append($div);
                            setValue(colValue, iControl, mField);
                        }
                        else {
                            $spanIcon.addClass('vis-w-p-card-icon-fixed');
                            objctrls["imgspan"] = $spanIcon;
                            /*Set what do you want to show? Icon OR Label OR Both OR None*/
                            if (mField.getDisplayType() == VIS.DisplayType.Button) {
                                $divIcon.remove(); // button has image with field
                            }
                            else if (!mField.isCardIconHide() && !mField.isCardTextHide()) {
                                $divIcon.append($spanIcon.append(icon));
                                if ($lblControl && $lblControl.length > 0)
                                    $divLabel.append($lblControl);
                            }
                            else if (mField.isCardIconHide() && mField.isCardTextHide()) {
                                $divIcon.remove();
                            }
                            else if (!mField.isCardIconHide() && mField.isCardTextHide()) {
                                $divIcon.append($spanIcon.append(icon));
                                if ($lblControl && $lblControl.length > 0)
                                    $lblControl.hide();
                            }
                            else if (mField.isCardIconHide() && !mField.isCardTextHide()) {
                                if ($lblControl && $lblControl.length > 0) {
                                    $divLabel.append($lblControl);
                                }
                                $divIcon.remove();
                            }

                            setValue(colValue, iControl, mField);
                            /****END ******  Set what do you want to show? Icon OR Label OR Both OR None*/
                        }
                        $divLabel.append(iControl.getControl());
                        $containerDiv.append($div);
                        //$self.controls.push(objctrls);
                    }
                }
            }

        };

        /**
         * Create card according to template
         * */
        this.setHeader = function () {
            if (headerItems && headerItems.length > 0) {
                for (var j = 0; j < headerItems.length; j++) {

                    var currentItem = headerItems[j];

                    var rows = currentItem.HeaderTotalRow;
                    var columns = currentItem.HeaderTotalColumn;
                    var backColor = currentItem.HeaderBackColor;
                    var padding = currentItem.HeaderPadding;
                    var gid = currentItem.AD_GridLayout_ID;

                    if (!backColor) {
                        backColor = '';
                    }

                    if (!padding) {
                        padding = '';
                    }

                    if (!this.fieldStyles[columns + '_' + rows + '_' + backColor + '_' + padding + '_' + gid])
                        this.fieldStyles[columns + '_' + rows + '_' + backColor + '_' + padding + '_' + gid] = {};
                    //Apply HTML Style
                    this.dymcClass = this.fieldStyles[columns + '_' + rows + '_' + backColor + '_' + padding + '_' + gid]['fieldGroupContainerUISettings'];
                    if (!this.dymcClass) {
                        this.dymcClass = this.fieldGroupContainerUISettings(columns, rows, backColor, padding, gid);
                        this.fieldStyles[columns + '_' + rows + '_' + backColor + '_' + padding + '_' + gid]['fieldGroupContainerUISettings'] = this.dymcClass;
                    }

                    var $containerDiv = $('<div class="' + this.dymcClass + '">');
                    root.append($containerDiv);

                    this.setHeaderItems(currentItem, $containerDiv, fields, record)


                }
            }
            else {
                for (var i = 0; i < fields.length; i++) {
                    field = fields[i];
                    var value = record[field.getColumnName().toLowerCase()];
                    dt = field.getDisplayType();

                    var $label = VIS.VControlFactory.getHeaderLabel(field, true);
                    var iControl = VIS.VControlFactory.getReadOnlyControl(this.curTab, field, false, false, false);
                    var $lblControl = null;
                    if ($label) {
                        $lblControl = $label.getControl().addClass('vis-w-p-card-data-label');
                    }
                    if (field.getDisplayType() == VIS.DisplayType.Button) {
                        if (iControl != null)
                            iControl.addActionListner(this);
                    }



                    if (field.lookup && field.lookup.gethasImageIdentifier()) {
                        var $divIcon = $('<div class="vis-w-p-card-icon-f"></div>');
                        var $div = $('<div class="vis-w-p-card-data-f">')
                        var $divLabel = $('<div class="vis-w-p-card-Label-f"></div>');
                        var img = null;
                        var imgSpan = null;
                        var styleArr = null;
                        var $image = $('<img>');
                        var $imageSpan = $('<span>');
                        var colValue = getFieldValue(field, record);

                        setFieldLayout(field.cardFieldStyle, $div, $divIcon, $divLabel);

                        if (VIS.DisplayType.List == field.lookup.displayType) {
                            img = field.lookup.getLOVIconElement(record[field.getColumnName().toLower()], true);

                            if (!img && colValue) {
                                imgSpan = colValue.substring(0, 1);
                                img = imgSpan;
                            }
                        }
                        else {
                            colValue = VIS.Utility.Util.getIdentifierDisplayVal(colValue);
                            img = getIdentifierImage(field, record);
                        }
                        if (img && !img.contains("Images/")) {
                            imgSpan = img;//img contains First charater of Name or Identifier text
                            $imageSpan.append(imgSpan);
                        }
                        else {
                            $image.attr('src', img);
                        }

                        $divIcon.append($imageSpan);
                        $divIcon.append($image);

                        /*Set what do you want to show? Icon OR Label OR Both OR None*/
                        setFieldVisibility(field, imgSpan, $image, $imageSpan, $lblControl, $divLabel, $divIcon);

                        $divLabel.append(iControl.getControl());
                        setValue(colValue, iControl, field);
                        root.append($div);
                        continue;
                    }
                    else if (VIS.DisplayType.IsLookup(dt)) {
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
                    else if (VIS.DisplayType.Image == dt) {
                        setValue(value, iControl, field);
                        root.append($label.getControl()).append(iControl.getControl());
                        continue;
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

                    if (VIS.Application.isRTL)
                        span = "<p><strong title='" + value + "'>" + value + "</strong> :" + field.getHeader() + "</p>";
                    else
                        span = "<p>" + field.getHeader() + ": <strong title='" + value + "'>" + value + "</strong></p>";

                    root.append($(span));
                };
            }
        };

        this.setColor = function (bc, fc) {
            if (bc)
                root.css('background', bc);
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

            this.styleTag.remove();
            this.styleTag = null;
            root.remove();
            root = null;
            this.getRoot = null;
            this.dc = null;
        };

        var setValue = function (colValue, iControl, mField) {
            if (colValue) {
                if (colValue.startsWith && colValue.startsWith("<") && colValue.endsWith(">")) {
                    colValue = colValue.replace("<", "").replace(">", "");
                }

                if (mField.getDisplayType() == VIS.DisplayType.Image) {
                    var oldValue = iControl.getValue();
                    iControl.getControl().show();
                    iControl.setDimension(240, 320);
                    if (oldValue == colValue) {
                        iControl.refreshImage(colValue);
                    }
                }

                else if (iControl.format) {
                    colValue = iControl.format.GetFormatAmount(iControl.format.GetFormatedValue(colValue), "init", VIS.Env.isDecimalPoint());
                }


                iControl.setValue(w2utils.encodeTags(colValue), false);


            }
            else {
                if (mField.getDisplayType() == VIS.DisplayType.Image) {
                    iControl.getControl().hide();

                    iControl.setValue(null, false);
                }
                else if (mField.getDisplayType() == VIS.DisplayType.Button && mField.getAD_Reference_Value_ID() > 0) {
                    iControl.setText("- -");
                }
                else
                    iControl.setValue("- -", true);
            }
        };

        //Get value of field..
        var getFieldValue = function (mField, record) {
            var colValue = record[mField.getColumnName().toLowerCase()]; // mField.getValue();

            if (colValue) {
                var displayType = mField.getDisplayType();

                if (mField.lookup) {
                    colValue = mField.lookup.getDisplay(colValue, true, true);
                }
                //	Date
                else if (VIS.DisplayType.IsDate(displayType)) {
                    if (displayType == VIS.DisplayType.DateTime) {
                        colValue = new Date(colValue).toLocaleString();
                    }
                    else if (displayType == VIS.DisplayType.Date) {
                        colValue = new Date(colValue).toLocaleDateString();
                    }
                    else {
                        colValue = (new Date(colValue).toLocaleTimeString());
                    }
                }
                //	YesNo
                else if (displayType == VIS.DisplayType.YesNo) {
                    var str = colValue.toString();
                    if (mField.getIsEncryptedColumn())
                        str = VIS.secureEngine.decrypt(str);
                    colValue = str.equals("true");	//	Boolean
                }

                //	LOB 
                else
                    colValue = colValue.toString();//string

                //	Encrypted
                // If field is marked encrypted, then replace all text of field with *.
                if (mField.getIsEncryptedField()) {
                    if (colValue && colValue.length > 0) {
                        colValue = colValue.replace(/[a-zA-Z0-9-. ]/g, '*').replace(/[^a-zA-Z0-9-. ]/g, '*');
                    }
                }

                if (mField.getObscureType()) {
                    if (colValue && colValue.length > 0) {
                        colValue = VIS.Env.getObscureValue(mField.getObscureType(), colValue);
                    }
                }

            }
            else {
                colValue = null;
            }

            return colValue;
        }

        /**
         * Set layout of idenifier field
         * @param {any} mField
         * @param {any} record
         */
        var getIdentifierImage = function (mField, record) {
            var value = record[mField.getColumnName().toLowerCase()];
            value = mField.lookup.getDisplay(value, true, true);

            if (value != null && value && value.indexOf("Images/") > -1) {// Based on sequence of image in idenitifer, perform logic and display image with text

                var img = value.substring(value.indexOf("Images/") + 7, value.lastIndexOf("^^"));
                img = VIS.Application.contextUrl + "Images/Thumb32x32/" + img;

                if (c == 0 || img.indexOf("nothing.png") > -1) {

                    value = value.replace("^^" + value.substring(value.indexOf("Images/"), value.lastIndexOf("^^") + 2), "^^^")
                    if (value.indexOf("Images/") > -1)
                        value = value.replace(value.substring(value.indexOf("Images/"), value.lastIndexOf("^^") + 2), "^^^");

                    value = value.split("^^^");
                    var highlightChar = '';
                    for (var c = 0; c < value.length; c++) {
                        if (value[c].trim().length > 0) {
                            if (highlightChar.length == 0)
                                highlightChar = value[c].trim().substring(0, 1).toUpper();
                            return highlightChar;
                        }

                    }
                }
                else
                    return img;
            }

        };

        /**
         * Set Layout of field based on setting in Field Value Style field
         * @param {any} fieldValueStyle
         * @param {any} $div
         * @param {any} $divIcon
         * @param {any} $divLabel
         */
        var setFieldLayout = function (fieldValueStyle, $div, $divIcon, $divLabel) {
            var styleArr = fieldValueStyle;
            if (styleArr && styleArr.length > 0)
                styleArr = styleArr.split("|");

            if (styleArr && styleArr.length > 0) {
                for (var j = 0; j < styleArr.length; j++) {
                    if (styleArr[j].indexOf("@img::") > -1 || styleArr[j].indexOf("@span::") > -1) {
                        $div.append($divIcon);
                        var css = "";
                        if (styleArr[j].indexOf("@img::") > -1) {
                            css = styleArr[j].replace("@img::", "");
                        }
                        else if (styleArr[j].indexOf("@span::")) {
                            css = styleArr[j].replace("@span::", "");
                        }
                        $divIcon.attr('style', css);
                    }
                    else if (styleArr[j].indexOf("@value::") > -1) {
                        $div.append($divLabel);
                    }
                    else if (styleArr[j].indexOf("<br>") > -1) {
                        $div.css("flex-direction", "column");
                    }
                    else {
                        $div.append($divIcon);
                        $div.append($divLabel);
                    }
                }
            }
            else {
                $div.append($divIcon);
                $div.append($divLabel);

            }
        };

        /**
         * Set visibility of field, icon and label of field
         * @param {any} mField
         * @param {any} imgSpan
         * @param {any} $image
         * @param {any} $imageSpan
         * @param {any} $lblControl
         * @param {any} $divLabel
         * @param {any} $divIcon
         */
        var setFieldVisibility = function (mField, imgSpan, $image, $imageSpan, $lblControl, $divLabel, $divIcon) {
            if (!mField.isCardIconHide() && !mField.isCardTextHide()) {
                if (imgSpan != null)
                    $image.hide();
                else {
                    $imageSpan.hide();
                }

                if ($lblControl && $lblControl.length > 0)
                    $divLabel.append($lblControl);
            }
            else if (mField.isCardIconHide() && mField.isCardTextHide()) {
                //$divIcon.hide();
                $divIcon.remove();
            }
            else if (mField.isCardTextHide()) {
                if (imgSpan != null)
                    $image.hide();
                else
                    $imageSpan.hide();

                if ($lblControl && $lblControl.length > 0)
                    $lblControl.remove();
            }
            else if (mField.isCardIconHide()) {
                if ($lblControl && $lblControl.length > 0) {
                    $divLabel.append($lblControl);
                }
                $divIcon.remove();
            }
        };

        this.setHeader();

        //this.addStyleToDom();
    };


    /**
     * Add dynamically created style tags to HTML document
     * */
    VCard.prototype.addStyleToDom = function () {
        this.styleTag.type = 'text/css';
        this.styleTag.innerHTML = this.dynamicStyle.join(" ");
        $($('head')[0]).append(this.styleTag);
    };

    /**
     * Set field style
     * @param {any} headerSeqNo
     * @param {any} startCol
     * @param {any} startRow
     * @param {any} mField
     * @param {any} fieldValueStyle
     */
    VCard.prototype.applyCustomUIForFieldValue = function (headerSeqNo, startCol, startRow, mField, fieldValueStyle) {
        var style = fieldValueStyle;
        var dynamicClassName = "vis-hp-card-FieldValue_" + startRow + "_" + startCol + "_" + this.windowNo + "_" + headerSeqNo + "_" + mField.getAD_Column_ID();
        if (style && style.toLower().indexOf("@value::") > -1) {
            style = getStylefromCompositeValue(style, "@value::");
        }

        this.dynamicStyle.push("." + dynamicClassName + "  {" + style + "} ");
        return dynamicClassName;
    };

    /**
     * Set field style
     * @param {any} headerSeqNo
     * @param {any} justify
     * @param {any} alignItem
     */
    VCard.prototype.justifyAlignImageItems = function (headerSeqNo, justify, alignItem) {
        var dynamicClassName = "vis-w-p-header-label-center-justify_" + headerSeqNo + "_" + this.windowNo;
        this.dynamicStyle.push(" ." + dynamicClassName + " {justify-content:" + this.textAlignEnum[justify] + ";align-items:" + this.alignItemEnum[alignItem] + "}");
        return dynamicClassName;
    };

    /**
     * 
     * @param {any} style
     * @param {any} requiredtype
     */
    var getStylefromCompositeValue = function (style, requiredtype) {
        if (style && style.toLower().indexOf(requiredtype) > -1) {
            var styleArr = style.split("|");
            for (var i = 0; i < styleArr.length; i++) {
                if (styleArr[i].toLower().indexOf(requiredtype) > -1) {
                    return styleArr[i].toLower().replace(requiredtype, "").trim();
                }
            }
        }
    }

    /**
     * Get  Custom Style of Parent of field
     * @param {any} backColor
     */
    VCard.prototype.headerParentCustomUISettings = function (backColor) {
        var dynamicClassName = "vis-ad-w-p-card-Custom_" + this.windowNo;
        this.dynamicStyle.push(" ." + dynamicClassName + " {flex:1;");
        this.dynamicStyle.push(backColor);

        this.dynamicStyle.push("} ");
        return dynamicClassName;
    };

    /**
     * Get Header UI Setting Style
     * @param {any} backcolor
     * @param {any} padding
     */
    VCard.prototype.headerUISettings = function (backcolor, padding) {
        var dynamicClassName = "vis-ad-w-p-card_root_" + this.windowNo;
        this.dynamicStyle.push(" ." + dynamicClassName + " {display:flex;overflow:auto;");
        this.dynamicStyle.push("padding:" + padding + ";" + backcolor);
        this.dynamicStyle.push("} ");
        return dynamicClassName;
    };

    /**
     * Set Custom style of field Group
     * @param {any} columns
     * @param {any} rows
     * @param {any} backcolor
     * @param {any} padding
     * @param {any} itemNo
     */
    VCard.prototype.fieldGroupContainerUISettings = function (columns, rows, backcolor, padding, itemNo) {
        var dynamicClassName = "vis-ad-w-p-fg_card-container_" + rows + "_" + columns + "_" + this.windowNo + "_" + itemNo;
        this.dynamicStyle.push(" ." + dynamicClassName + " {display:grid;");
        this.dynamicStyle.push('grid-template-columns:repeat(' + columns + ', 1fr);grid-template-rows:repeat(' + rows + ', auto);padding:' + padding + ';' + backcolor);
        this.dynamicStyle.push("} ");
        return dynamicClassName;
    };

    /**
     * get Style of Field Group
     * @param {any} headerSeqNo
     * @param {any} startCol
     * @param {any} colSpan
     * @param {any} startRow
     * @param {any} rowSpan
     * @param {any} justify
     * @param {any} alignment
     * @param {any} backColor
     * @param {any} fontColor
     * @param {any} fontSize
     * @param {any} padding
     */
    VCard.prototype.applyCustomUISettings = function (headerSeqNo, startCol, colSpan, startRow, rowSpan, justify, alignment, backColor, fontColor, fontSize, padding) {
        var dynamicClassName = "vis-hp-card-FieldGroup_" + startRow + "_" + startCol + "_" + this.windowNo + "_" + headerSeqNo;
        this.dynamicStyle.push("." + dynamicClassName + "  {grid-column:" + startCol + " / span " + colSpan + "; grid-row: " + startRow + " / span " + rowSpan + ";");

        this.dynamicStyle.push("justify-content:" + this.textAlignEnum[justify] + ";align-items:" + this.alignItemEnum[alignment]);
        this.dynamicStyle.push(";font-size:" + fontSize + ";color:" + fontColor + ";padding:" + padding + ";");
        this.dynamicStyle.push(backColor);
        this.dynamicStyle.push("} ");
        return dynamicClassName;
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

    /**
     * handled Button click 
     * @param {any} action
     */
    VCard.prototype.actionPerformed = function (action) {
        //selfPan.actionButton(action.source);
        if (this.aPanel.curTab.needSave(true, false)) {
            this.aPanel.cmd_save(true);
            return;
        }

        this.curTab = this.aPanel.curTab;
        this.curGC = this.aPanel.curGC;

        this.aPanel.actionPerformed(action, this);
    };

    /**
     * Save unsaved changes before buton click
     * @param {any} manual
     * @param {any} callback
     */
    VCard.prototype.cmd_save = function (manual, callback) {
        return this.aPanel.cmd_save2(manual, this.curTab, this.curGC, this.aPanel, callback);
    }

    VCard.prototype.dispose = function () {
        this.dC();
        vaSortable.prototype.dispose();
    };

    VIS.VCardView = VCardView;

}(VIS, jQuery));
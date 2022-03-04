; (function (VIS, $) {

    function VCardPanel(root) {

        var divTopArrow = $('<div class=vis-cv-rd-top><i class="vis vis-arrow-up"></i></div>');
        var divDownArrow = $('<div class=vis-cv-rd-down><i class="vis vis-arrow-down"></i></div>');
        var divbody = $('<div class=vis-cv-rd-body>');
        root.append(divTopArrow).append(divbody).append(divDownArrow);

        this.addItem = function (name) {
            var $spn = $('<span class="vis-cv-rd-body-item">');
            $spn.text(name);
            divbody.append($spn);
            if (divbody[0].clientHeight < divbody[0].scrollHeight) {
                divDownArrow.css('opacity', '1');
            }
        };

        this.reset = function () {
            divbody.empty();
            divTopArrow.css('opacity', '.5');
            divDownArrow.css('opacity', '.5');
        }

        this.dispose = function () {
            divbody.empty();
            //clear events
            divTopArrow.off();
            divDownArrow.off();
        }

        

        function clkHandler(btn) {
           
            if (divbody[0].clientHeight + divbody[0].scrollTop >= divbody[0].scrollHeight) {
                divDownArrow.css('opacity', '.6');
                // return;
            }
            else {
                divDownArrow.css('opacity', '1');
            }
            if (divbody[0].scrollTop == 0) {
                divTopArrow.css('opacity', '.6');
                //return;
            }
            else {
                divTopArrow.css('opacity', '1');
            }
            //else if (divbody.scrollTop >= divbody.height()) {
            //    divDownArrow.css('opacity', '.6');
            //    return;
            //}
            //if (btn.css('opacity') < 1)
            //    return;

            if (btn == 'up') {
                if (divTopArrow.css('opacity') < 1)
                    return;
                divbody[0].scrollTop -= 60;
            }
            else {
                if (divDownArrow.css('opacity') < 1)
                    return;
                divbody[0].scrollTop += 60; 
            }

            };
        divTopArrow.on('click', function () { clkHandler('up') });
        divDownArrow.on('click', function () { clkHandler('down') });
    }

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
        this.GroupSequence = null;
        this.ExcludedGroup = null;
        // this.aPanel;
        this.onCardEdit = null;

        this.cardViewData = null;

        var root;
        var body = null;
        var headerdiv;
        var $cmbCards = null;
        var $lblGroup = null;
        var $btnClrSearch = null;
        var $imgdownSearch = null;
        var self = this;
        var groupHeader = null;
        this.isAutoCompleteOpen = false;
        var bsyDiv = null;
        var rightDiv = null;
        var leftDiv = null;
        this.cardID = 0;
        var records = null;
        this.editID = 0;

        this.VCardRightPanel = null;
        //  var cardList;
        function init() {
            var width = $('body').width()-65;
            root = $("<div class='vis-cv-body vis-noselect'>");
            leftDiv = $("<div class='vis-cv-ld'>");
            rightDiv = $("<div class='vis-cv-rd'>");
            //bsyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="fa fa-spinner fa-pulse fa-3x fa-fw"></i></div></div>');
            //root.append(bsyDiv);
            //bsyDiv.css("display", 'none');
            body = $("<div class='vis-cv-main' style='max-width:" + width+"px'>");
            headerdiv = $("<div class='vis-cv-header'>");
            $cmbCards = $('<input  class="vis-vs-card-autoComplete" style="display:inline">')
            $lblGroup = $('<p>');
            $imgdownSearch = $('<span class="vis-ad-w-p-tb-s-icon-down vis-cv-cardlist"><i class="fa fa-ellipsis-h"></i></span>');
            groupHeader = $("<div class='vis-cv-groupHeader'style='overflow:hidden; max-width:" + width + "px'>");
            headerdiv.append($cmbCards).append($imgdownSearch).append($lblGroup); 
            leftDiv.append(headerdiv).append(groupHeader).append(body);
            root.append(leftDiv).append(rightDiv);
            body.scroll(function () {
                SyncScroll();
            });
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
                    li = $("<li style='white-space:normal !important; max-width:210px !important'>")
                        .append($("<a  data-checkid='" + item.id + "'  class='vis-cv-card-selected-card' style='display:block' title='" + item.title + "'></a>").append(tickSpan).append("<p>" + item.label + "</p>").append(span))
                        .prependTo(ul);
                }
                else {
                    li = $("<li style='white-space:normal !important; max-width:210px !important'>")
                        .append($("<a  data-checkid='" + item.id + "'  style='display:block' title='" + item.title + "'></a>").append(tickSpan).append("<p>" + item.label + "</p>").append(span))
                        .prependTo(ul);
                }

                if (item.Created != VIS.context.getAD_User_ID()) {
                    li.find('p').text(item.label +" (S)");
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

        function SyncScroll() {
            groupHeader.scrollLeft(body.scrollLeft());
        }       

        this.SyncScroll = function () {
            SyncScroll();            
        }

        init();

        //set right pnae div for excluded group
        this.VCardRightPanel = new VCardPanel(rightDiv);
        //eventHandle();

        this.getRoot = function () {
            return root;
        };

        this.getRightDiv = function () {
            return rightDiv;
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
        this.getGroupHeader = function () {
            return groupHeader;
        }

        this.sizeChanged = function (h, w) {
            root.height((h - 12) + 'px');
            rightDiv.height((h - 12) + 'px');
            this.calculateWidth(w);
        }

        this.setBusy = function (isBusy) {
            this.aPanel.setBusy(false);
           // bsyDiv.css("display", isBusy ? 'block' : 'none');
        };

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
                        self.editID = s;
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
                        self.onCardEdit({ 'recid': s }, true);
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
            if (this.VCardRightPanel)
                this.VCardRightPanel.dispose();
            this.VCardRightPanel = null;
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
                for (var i = cards.length - 1; i >= 0; i--) {

                    cards[i].Name = VIS.Utility.decodeText(cards[i].Name);
                    // $cmbCards.append('<option value="' + cards[i].AD_CardView_ID + '">' + cards[i].Name + '</option>');
                    if (cards[i].IsDefault) {
                        userQueries.push({ 'title': cards[i].Name, 'label': cards[i].Name, 'value': cards[i].Name, 'id': cards[i].AD_CardView_ID, 'isDefault': 'Y', 'Created': cards[i].Created });
                    }
                    else {
                        userQueries.push({ 'title': cards[i].Name, 'label': cards[i].Name, 'value': cards[i].Name, 'id': cards[i].AD_CardView_ID, 'isDefault': 'N', 'Created': cards[i].Created });
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
            if (args) {
                this.replaceCard(args, id);
            }
            this.navigate(id, null, null);
        }
       
    };
    /**
     * Replace card after drag drop for apply new condition
     * @param {any} rec
     * @param {any} id
     */
    VCardView.prototype.replaceCard = function (rec,id) {
        rec.recid = id;

        var changeCard = new VCard(this.fields, rec, this.headerItems, this.headerStyle, this.headerPadding, this.mTab.getWindowNo(), {}, this.aPanel)
        if (this.grpCount==1) {
            changeCard.getRoot().width("240px").css({ 'margin': '5px 12px 12px 5px', 'float': (VIS.Application.isRTL ? 'right' : 'left') });
        }
        if ($('head:contains("' + changeCard.headerCustom + ' {")').length == 0) {
            changeCard.addStyleToDom();
        }
        
       this.getRoot().find("[name='vc_" + id + "']").replaceWith(changeCard.getRoot());
        changeCard.evaluate(this.cConditions);
    }

    /**
     * Set up card view fill cards list, set default card ID
     * @param {any} aPanel
     * @param {any} mTab
     * @param {any} cContainer
     * @param {any} vCardId
     */
    VCardView.prototype.setupCardView = function (aPanel, mTab, cContainer, vCardId) {
        this.mTab = mTab;
        this.aPanel = aPanel;        
        if (mTab.vo && mTab.vo.DefaultCardID) {
            this.cardID = (mTab.vo.DefaultCardID);
        } else {
            this.setCardViewData();
            //this.refreshUI(this.getBody().width());
        }
        cContainer.append(this.getRoot());
    };

    /**
     * When user change card from drowdown, then fetch details of card and show card
     * @param {any} mTab
     * @param {any} cardID
     * @param {any} cardName
     */
    VCardView.prototype.getCardViewData = function (mTab, cardID, cardName) {        
        this.cardID = cardID;
        this.mTab.getTableModel().setCardID(cardID);
        this.aPanel.curGC.query(this.mTab.getOnlyCurrentDays(), 0, false);
        this.getCardCmb().val(cardName);
    };
    /**
     * Rest card on view change
     * */
    VCardView.prototype.resetCard = function () {
        
        while (this.groupCtrls.length > 0) {
            this.groupCtrls.pop().dispose();
        }
        this.VCardRightPanel.reset();
        this.getGroupHeader().empty();
        this.groupCtrls.length = 0;
        this.editID = 0;
    }
       
    /**
     * Create Card's Schema, like fields included, groupby etc.
     * @param {any} retData
     */
    VCardView.prototype.setCardViewData = function (retData) {
        this.hasIncludedCols = false;
        this.fields = [];
        this.cGroup = null;
        this.cConditions = [];
        this.GroupCount = {};
        this.headerItems = {};
        if (retData) {
            // this.getHeader().show();
            this.AD_CardView_ID = retData.AD_CardView_ID;
            //$cmbCards.autocomplete('option', 'source', userQueries);
            //$cmbCards.autocomplete("search", "");
            //$cmbCards.trigger("focus");
            this.cardName = retData.Name;
            this.GroupSequence = retData.GroupSequence;
            this.ExcludedGroup = retData.ExcludedGroup;

            for (var i = 0; i < retData.GroupCount.length; i++) {
                this.GroupCount[retData.GroupCount[i].Group] = retData.GroupCount[i].Count;
            }
          
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

    /**
     * Get Group's details
     * */
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

                    this.GroupCount['true'] = this.GroupCount['Y'];
                    this.GroupCount['false'] = this.GroupCount['N'];

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
                    }


                }
                this.setHeader(field.getHeader());
            }
            else {
                this.setHeader(' ');
            }
        }
        else { this.setHeader(' '); }
        if (this.grpCount < 1 || $.isEmptyObject(this.cGroupInfo)) {//add one group by de
            this.cGroupInfo['All'] = { 'name': VIS.Msg.getMsg('All'), 'records': [], 'key': null };
            this.grpCount = 1;
        }
        this.isProcessed = true;
    };

    /**
     * Create cards and groups
     * @param {any} width
     */
    VCardView.prototype.refreshUI = function (width) {      
        //if (!this.cardViewData && this.cardID>0) {
        //    this.getCardViewData(this.mTab, this.cardID, "", true);
        //} else {            
        //    this.refresh(width);
        //}

        var temp= this.mTab.getTableModel().getCardTemplate();
        
        //Reset Variables
        //this.aPanel.setBusy(true);
        this.setCardViewData(temp);
        this.refresh(width);
        //this.getBody().empty();
        
    };

    VCardView.prototype.refresh = function (width) {
        var $this = this;
        window.setTimeout(function () {
            if (width == 0) {
                width = $this.getBody().width();
            }
            $this.isProcessed = false;
            $this.createGroups();

            records = $this.mTab.getTableModel().mSortList;

            var root = $this.getBody();

            //while ($this.groupCtrls.length > 0) {
            //    $this.groupCtrls.pop().dispose();
            //}

            $this.resetCard();

            //root.not('.vis-busyindicatorouterwrap').empty();
           // $this.getGroupHeader().html('');
            var cardGroup = null;
            if ($this.grpCount == 1) {

                var n = '';
                var key = null;
                for (var p in $this.cGroupInfo) {
                    n = VIS.Utility.Util.getIdentifierDisplayVal($this.cGroupInfo[p].name);
                    key = $this.cGroupInfo[p].key;
                    $this.getGroupHeader().append("<div class='vis-cv-head' >" + n + "</div>");
                    break;
                }

                cardGroup = new VCardGroup(true, records, n, $this.fields, $this.cConditions, $this.headerItems, $this.headerStyle, $this.headerPadding, key, $this.aPanel);
                $this.groupCtrls.push(cardGroup);
                root.append(cardGroup.getRoot())
                $this.getGroupHeader().find('.vis-cv-head').width(root.find('.vis-cv-grpbody').width() - 10);
                $this.getRightDiv().css('display', 'none');
            }
            else {
                $this.filterRecord(records);
                for (var p in $this.cGroupInfo) {
                    setCardGroup(p);
                    var gc = $this.cGroupInfo[p].key;
                    if ($this.GroupCount[gc]) {
                        gc = $this.GroupCount[gc];
                    } else {
                        gc = 0;
                    }

                    var grp = $("<div data-key='" + $this.cGroupInfo[p].key + "' class='vis-cv-cg-grp'></div>");
                    grp.append("<div class='vis-cv-head' title='" + VIS.Utility.Util.getIdentifierDisplayVal($this.cGroupInfo[p].name) + " (" + gc + ")' >" + VIS.Utility.Util.getIdentifierDisplayVal($this.cGroupInfo[p].name) + "<span> (" + gc + ")</span></div>");
                    $this.getGroupHeader().append(grp);
                }



                if ($this.cGroup.lookup && ($this.cGroup.lookup.displayType == VIS.DisplayType.List || $this.cGroup.lookup.displayType == VIS.DisplayType.TableDir || $this.cGroup.lookup.displayType == VIS.DisplayType.Table || $this.cGroup.lookup.displayType == VIS.DisplayType.Search) && $this.GroupSequence != null && $this.GroupSequence != "") {
                    var grpArr = $this.GroupSequence.split(",");
                    for (var j = 0; j < grpArr.length; j++) {
                        var item = root.find(".vis-cv-grpbody[data-key='" + grpArr[j] + "']").parent();
                        var itemG = $this.getGroupHeader().find(".vis-cv-cg-grp[data-key='" + grpArr[j] + "']");
                        var before = root.find(".vis-cv-cg").eq(j);
                        item.insertBefore(before);
                        itemG.insertBefore($this.getGroupHeader().find(".vis-cv-cg-grp").eq(j));
                    }
                }
                if ($this.editID == 0) {
                    $this.onCardEdit({ 'recid': root.find(".vis-cv-card:first").attr('data-recid') }, true);                   
                }
                $this.editID = 0;

                emptyCardSetup();
            }

            function setCardGroup(p) {
                cardGroup = new VCardGroup($this.grpCount === 1, $this.cGroupInfo[p].records, VIS.Utility.Util.getIdentifierDisplayVal($this.cGroupInfo[p].name), $this.fields, $this.cConditions, $this.headerItems, $this.headerStyle, $this.headerPaddings, $this.cGroupInfo[p].key, $this.aPanel);
                $this.groupCtrls.push(cardGroup);
                root.append(cardGroup.getRoot());
                var sortable = new vaSortable(cardGroup.getBody()[0], {
                    attr: 'data-recid',
                    selfSort: true,
                    force: false,
                    mainNode: '.vis-cv-main',
                    ignore: ['.vis-cv-card-edit', '.vis-ev-col-wrap-button', '.cardEmpty'],
                    onclick: function (e, item) {
                        var recID = $(item).attr("data-recid");
                        $this.onCardEdit({ 'recid': recID }, true);
                        var m_field = $this.mTab.getFieldById($this.getField_Group_ID());
                        if (m_field.getIsReadOnly() || m_field.getCallout() != '' || !m_field.getIsEditable(true,true)) {
                            vaSortable.prototype.setStopDrag(true);
                            $(item).css("cursor","not-allowed");
                        } else {
                            $(item).css("cursor", "default");
                            vaSortable.prototype.setStopDrag(false);
                        }

                    },
                    onSelect: function (e, item, fromItem) {
                        var toKey = $(e).parent().attr('data-key');
                        if (toKey == $(fromItem).attr('data-key')) {
                            return;
                        }
                        var obj = {
                            grpValue: (toKey == 'null' ? null : toKey),
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
                            beforeSend: function () {
                                $this.setBusy(true);
                            },
                            success: function (data) {
                                if (data != "1") {
                                    VIS.ADialog.error(data, true, "");
                                    vaSortable.prototype.revertItem();
                                } else {

                                    var fromkey = $(fromItem).attr('data-key');
                                    if (!$this.GroupCount[toKey]) {
                                        $this.GroupCount[toKey] = 0;
                                    }

                                    $this.GroupCount[toKey] = $this.GroupCount[toKey] + 1;
                                    $this.GroupCount[fromkey] = $this.GroupCount[fromkey] - 1;


                                    $this.getGroupHeader().find(".vis-cv-cg-grp[data-key='" + fromkey + "']").find('span').text(' (' + $this.GroupCount[fromkey] + ')');
                                    $this.getGroupHeader().find(".vis-cv-cg-grp[data-key='" + toKey + "']").removeClass('emptyGroup').find('span').text(' (' + $this.GroupCount[toKey] + ')');
                                    $this.getBody().find('.cardEmpty').closest('.emptyGroup').removeClass('emptyGroup');

                                    $this.getGroupHeader().find(".vis-cv-cg-grp[data-key='" + fromkey + "'] .vis-cv-head").attr("title", $this.getGroupHeader().find(".vis-cv-cg-grp[data-key='" + fromkey + "']").text());
                                    $this.getGroupHeader().find(".vis-cv-cg-grp[data-key='" + toKey + "'] .vis-cv-head").attr("title", $this.getGroupHeader().find(".vis-cv-cg-grp[data-key='" + toKey + "']").text());

                                    emptyCardSetup();
                                    $this.mTab.dataRefresh();
                                }
                            },
                            error: function (err) {
                                VIS.ADialog.error(err.responseText, true, "");
                                vaSortable.prototype.revertItem();
                                $this.setBusy(false);
                            },
                            complete: function () {
                                $this.setBusy(false);
                            }
                        });
                    }
                });
            }

            function emptyCardSetup() {
                $this.getRoot().removeClass('emptyGroup').removeAttr('style');
                root.find('.cardEmpty').remove();
                var excludeGrp = $this.ExcludedGroup;
                excludeGrp = excludeGrp.split(',');
                var hasItems = false;
                root.find('.vis-cv-grpbody').each(function (i, e) {
                    var evnt = $(e);
                    evnt.parent().removeAttr('style');
                    if (evnt.is(':empty')) {
                        evnt.append("<div class='va-dragdrop cardEmpty' style='height:" + root.height() + "px'>").parent().addClass('emptyGroup');
                        $this.getGroupHeader().find('.vis-cv-cg-grp').eq(i).addClass('emptyGroup').removeAttr('style');
                    } else {
                        $this.getGroupHeader().find('.vis-cv-cg-grp').eq(i).css("min-width", evnt.parent().width());
                        evnt.parent().css('min-width', evnt.parent().width());
                    }

                    if (excludeGrp.indexOf(evnt.attr('data-key')) != -1) {
                        //show item info in right panel
                        evnt.parent().hide();
                        var grpHeader = $this.getGroupHeader().find(".vis-cv-cg-grp[data-key='" + evnt.attr('data-key') + "']");
                        $this.VCardRightPanel.addItem(grpHeader.find('.vis-cv-head').text());
                        hasItems = true;
                        grpHeader.hide();
                    }

                });

                $this.getRightDiv().height($this.getRoot().height());
                root.find('.vis-cv-grpbody').height(maxHeight(root.find('.vis-cv-grpbody')));
                if (hasItems)
                    $this.getRightDiv().css('display', 'flex');
                else $this.getRightDiv().css('display', 'none');

            }
            function maxHeight(elems) {
                return Math.max.apply(null, elems.map(function () {
                    return $(this)[0].scrollHeight;
                }).get());
            }
            $this.calculateWidth(width);
            $this.SyncScroll();
            $this.aPanel.setBusy(false);
        }, 10);
    }

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
                    this.cGroupInfo['Other__1'] = { 'name': VIS.Msg.getMsg("No")+ ' ' + this.cGroup.getHeader() , 'records': [], 'key': null };
                    this.grpCount += 1;
                    isgrouprChanged = true;
                    isgrouprChanged = true;
                }
                this.cGroupInfo['Other__1'].records.push(record);

            }
        }

        //var eCols = [];
        //for (var p in this.cGroupInfo) {
        //    if (this.cGroupInfo[p].records.length < 1) {
        //        eCols.push(p);               
        //    }
        //}
        //this.grpCount -= eCols.length;

        //while (eCols.length > 0) {
        //    delete this.cGroupInfo[eCols.pop()];

        //}

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
        windowNo = aPanel.curTab.getWindowNo();//  VIS.Env.getWindowNo();
        function init() {            
            var str = "<div class='vis-cv-cg vis-pull-left'>"
                + "<div data-key='" + key + "'  class='vis-cv-grpbody'></div></div></div>";
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
        vaSortable.prototype.dispose();
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

                        if (mField.getOrginalDisplayType() == VIS.DisplayType.Button) {
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
                                if (VIS.DisplayType.List == mField.lookup.displayType) {
                                     $image.attr('src', $(img).attr('src'));
                                }
                                else {
                                    $image.attr('src', img);
                                }
                               
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
                            if (mField.getOrginalDisplayType() == VIS.DisplayType.Button) {
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
                    dt = field.getOrginalDisplayType();

                    var $label = VIS.VControlFactory.getHeaderLabel(field, true);
                    var iControl = VIS.VControlFactory.getReadOnlyControl(this.curTab, field, false, false, false);
                    var $lblControl = null;
                    if ($label) {
                        $lblControl = $label.getControl().addClass('vis-w-p-card-data-label');
                    }
                    if (field.getOrginalDisplayType() == VIS.DisplayType.Button) {
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
                            if (VIS.DisplayType.List == field.lookup.displayType) {
                                $image.attr('src', $(img).attr('src'));
                            }
                            else {
                                $image.attr('src', img);
                            }
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
                        if (field.getLookup()) {
                            value = field.getLookup().getDisplay(value);
                        }
                        value = value;
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
                    else if (VIS.DisplayType.Button == dt) {
                        setValue(value, iControl, field);
                        root.append(iControl.getControl());
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
                else if (mField.getOrginalDisplayType() == VIS.DisplayType.Button && mField.getAD_Reference_Value_ID() > 0) {
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
        //vaSortable.prototype.dispose();
    };

    VIS.VCardView = VCardView;

}(VIS, jQuery));
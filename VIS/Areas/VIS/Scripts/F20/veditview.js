; (function (VIS, $) {
    //****************************************************//
    //**             VPanel                            **//
    //**************************************************//
    VIS.VGridPanel = function () {

        var oldFieldGroup = null, columnIndex = -2, allControlCount = -1;;
        var allControls = [];
        var allLinkControls = [];

        var $table;

        var $td0, $td1, $td2, $td3;

        var _curParent = null;

        var col0 = { rSpan: 1, cSpan: 0, cSpace: 0, orgRSpan: 1 };
        var col1 = { rSpan: 1, cSpan: 0, cSpace: 0, orgRSpan: 1 };
        var col2 = { rSpan: 1, cSpan: 0, cSpace: 0, orgRSpan: 1 };
        var col3 = { rSpan: 1, cSpan: 0, cSpace: 0, orgRSpan: 1 };

        /** Map of group name to list of components in group. */
        //control = field array
        var compToFieldMap = {}

        /** Map of group name to list of components in group. */
        var groupToCompsMap = {};

        var fieldToCompParentMap = {};
        var colDescHelpList = {};

        var lastPopover = null;
        function initComponent() {
            $table = $("<div class='vis-ad-w-p-vc-ev-grid'>"); //   $("<table class='vis-gc-vpanel-table'>");
            $table.on("click", "span.vis-ev-ctrlinfowrap", onInfoClick);
        };

        function onInfoClick(e) {
            var curTgt = $(e.currentTarget);
            var colName = curTgt.data('colname');
            if (colName != '') {

                if (lastPopover) {
                    lastPopover.popover('dispose');
                    lastPopover = null;
                }

                curTgt.attr('data-content', colDescHelpList[colName].help);
                //attr('title', colDescHelpList[colName].desc);
                lastPopover = curTgt.popover('show');

            }
        }

        initComponent();

        function initCols(isCol0, isCol1, isCol2, isCol3) {

            if (isCol0)
                _curParent = $td0 = $("<div class='vis-ev-col'></div>");
            if (isCol1)
                _curParent = $td1 = $("<div class='vis-ev-col vis-ev-col-start2'></div>");
            if (isCol2)
                _curParent = $td2 = $("<div class='vis-ev-col vis-ev-col-start3'></div>");
            if (isCol3)
                _curParent = $td3 = $("<div class='vis-ev-col vis-ev-col-start4'></div>");
        };

        function reset(col) {
            if (!col) {
                col0 = { rSpan: 1, cSpan: 0, cSpace: 0, set: false };
                col1 = { rSpan: 1, cSpan: 0, cSpace: 0, set: false };
                col2 = { rSpan: 1, cSpan: 0, cSpace: 0, set: false };
                col3 = { rSpan: 1, cSpan: 0, cSpace: 0, set: false };
            }
            else if (col.rSpan <= 1) {
                col = { rSpan: 1, cSpan: 0, cSpace: 0, set: false };
            }
        };

        function adjustRowSpanForSameLine(colIndex) {
            // if(colIndex)

        }

        function adjustRowSpan(colIndex) {

            if (col0.rSpan > 1) { //skip column 
                if (col0.set && colIndex == 1 &&  col0.cSpan < 4) { //special case
                    col0.set = false;
                }
                else
                    --col0.rSpan;

                reset(col0);
            }
            if (col1.rSpan > 1) { //skip column 
                if (colIndex == 2 && col1.set &&  col1.cSpan < 3) { //special case
                    col1.set = false;
                }
                else
                    --col1.rSpan;
                reset(col1);
            }
            if (col2.rSpan > 1) { //skip column 
                if (colIndex == 3 && col2.set && col2.cSpan <2) { //special case
                    col2.set = false;
                }
                else
                    --col2.rSpan;
                reset(col2);
            }
            if (col3.rSpan > 1) { //skip column 
                --col3.rSpan;
                reset(col3);
            }
        };

        function adjustLayout(mField, isNewRow) {
            var rowSpan = mField.getFieldBreadth();
            var colSpan = mField.getFieldColSpan();
            var cellSpace = mField.getCellSpace();
            var isLongFiled = mField.getIsLongField();
            var isLineBreak = mField.getIsLineBreak();

            if (isLineBreak) {
                reset();
                isNewRow = true;
            }
            if (isNewRow) {
                adjustRowSpan(columnIndex);
                addRow();
                columnIndex = 0;
            }


            if (columnIndex == 0) {
                if (isLongFiled) {
                    addRow();//add last row;
                    reset();
                    initCols(true);
                    $td0.addClass("vis-ev-col-end4");
                    columnIndex = 4;
                }
                else {
                    // check for row span
                    if (col0.rSpan > 1) { //skip column 
                        columnIndex += col0.cSpan;
                        //--col0.rSpan;
                        //reset(col0);
                    }
                    else if (cellSpace > 0) {
                        if (cellSpace > 3)
                            cellSpace = 3;
                        columnIndex += cellSpace;
                        cellSpace = 0; //reset
                    }
                    else if ($td0) {
                        columnIndex += 1;
                    }
                    else {

                        initCols(true);
                        if (colSpan == 2) {
                            if (col1.rSpan <= 1) //if nor row span on on colujn 1
                                $td0.addClass("vis-ev-col-end2");
                        }

                        else if (colSpan == 3) {
                            if (col1.rSpan <= 1 && col2.rSpan <= 1)
                                $td0.addClass("vis-ev-col-end3");
                            else if (col1.cSpan < 1)
                                $td0.addClass("vis-ev-col-end2");
                        }
                        else if (colSpan > 3) {
                            if (col1.rSpan <= 1 && col2.rSpan <= 1 && col3.rSpan <= 1)
                                $td0.addClass("vis-ev-col-end4");
                            else if (col1.rSpan <= 1 && col2.rSpan <= 1)
                                $td0.addClass("vis-ev-col-end3");
                            else if (col1.rSpan <= 1)
                                $td0.addClass("vis-ev-col-end2");
                        }
                        columnIndex += colSpan - 1;

                        if (rowSpan > 1) {
                            col0.rSpan = rowSpan + 1; //extra to fill addnew minus
                            col0.set = true;
                            col0.cSpan = colSpan;
                            col0.cSpace = cellSpace;
                            $td0.css("grid-row", "span " + rowSpan);
                        }
                        return;
                    }
                }
            }

            if (columnIndex == 1) {

                // check for row span
                if (col1.rSpan > 1) { //skip column 
                    columnIndex += col1.cSpan;
                    //--col1.rSpan;
                    //reset(col1);
                }
                else if (cellSpace > 0) {
                    if (cellSpace > 2)
                        cellSpace = 2;
                    columnIndex += cellSpace;
                    cellSpace = 0;
                }
                else if ($td1) {
                    columnIndex += 1;
                }
                else {

                    initCols(false, true);
                    if (colSpan == 2) {
                        if (col2.rSpan <= 1) //if nor row span on on colujn 1
                            $td1.addClass("vis-ev-col-end3");
                    }

                    else if (colSpan >= 3) {
                        if (col2.rSpan <= 1 && col3.rSpan <= 1)
                            $td1.addClass("vis-ev-col-end4");
                        else if (col2.cSpan < 1)
                            $td1.addClass("vis-ev-col-end3");
                    }

                    columnIndex += colSpan - 1;
                    if (rowSpan > 1) {
                        col1.rSpan = rowSpan + 1;
                        col1.set = true;
                        col1.cSpan = colSpan;
                        col1.cSpace = cellSpace;
                        $td1.css("grid-row", "span " + rowSpan);
                    }
                    return;
                }
            }

            if (columnIndex == 2) {

                // check for row span
                if (col2.rSpan > 1) { //skip column 
                    columnIndex += col2.cSpan;
                    //--col2.rSpan;
                    //reset(col2);
                }
                else if (cellSpace > 0) {
                    if (cellSpace > 1)
                        cellSpace = 1;
                    columnIndex += cellSpace;
                    cellSpace = 0;
                }
                else if ($td2) {
                    columnIndex += 1;
                }
                else {

                    initCols(false, false, true);
                    if (colSpan >= 2) {
                        if (col3.rSpan <= 1) //if nor row span on on colujn 1
                            $td2.addClass("vis-ev-col-end4");
                    }

                    columnIndex += colSpan - 1;
                    if (rowSpan > 1) {
                        col2.rSpan = rowSpan + 1;
                        col2.set = true;
                        col2.cSpan = colSpan;
                        col2.cSpace = cellSpace;
                        $td2.css("grid-row", "span " + rowSpan);
                    }
                    return;
                }
            }

            if (columnIndex == 3) {
                // check for row span
                if (col3.rSpan > 1) { //skip column 
                    //--col3.rSpan;
                    //reset(col3);
                }
                else if ($td3) {
                    isNewRow = true;
                    //addRow();
                    //columnIndex = 0;
                }
                else {
                    initCols(false, false, false, true);
                    if (colSpan >= 2) {
                        $td3.addClass("vis-ev-col-end4");
                    }
                    if (rowSpan > 1) {
                        col3.rSpan = rowSpan + 1;
                        col3.set = true;
                        col3.cSpan = colSpan;
                        col3.cSpace = cellSpace;
                        $td3.css("grid-row", "span " + rowSpan);
                    }
                }
                return;
            }

            //if all col index are skipped
            if (!$td0 && !$td1 && !$td2 && !$td3) {
                //columnIndex = 0;
                adjustLayout(mField, isNewRow);
            }
            else if (!isLongFiled && columnIndex > 3) {
                adjustLayout(mField, true);
            }
        };

        function addRow() {

            if ($td0)
                $table.append($td0);
            if ($td1)
                $table.append($td1);
            if ($td2)
                $table.append($td2);
            if ($td3)
                $table.append($td3);

            $td0 = $td1 = $td2 = $td3 = $td4 = null;
            //if (td3RSpan < 0)
            //    $table.append($td3)
            //else if (td3RSpan > 100) {
            //    td3RSpan = td3RSpan - 100;
            //    $table.append($td3.css('grid-row', 'span ' + td3RSpan));
            //}
        };

       

        function onGroupClick(e) {
            e.stopPropagation();
            var divGroup = $(this);
            var target = $(e.target);
            var name = divGroup.data("name");
            var dis = divGroup.data("display");
            var viewMore = $(divGroup.find('.vis-ev-col-fg-more')[0]);
            //console.log(name);
            //console.log(dis);
            var show = false;
            var showGroupFieldDefault = false;


            if (target.is('span')) {
                if (dis !== "show") {// If group is vlosed and user click on show more then no processing.
                    return;
                }
                show = true;
                if (divGroup.data("showmore") == 'Y') {
                    showGroupFieldDefault = true;
                    divGroup.data("showmore", "N");
                    viewMore.text(VIS.Msg.getMsg("ShowLess"));
                }
                else {
                    divGroup.data("showmore", "Y");
                    viewMore.text(VIS.Msg.getMsg("ShowMore"));
                }
            }
            else {
                if (divGroup.data("showmore") == 'N') {
                    showGroupFieldDefault = true;
                }

                if (dis === "show") {
                    divGroup.data("display", "hide");
                    viewMore.hide();
                    $(divGroup.children()[2]).addClass("vis-ev-col-fg-rotate");
                } else {

                    divGroup.data("display", "show");
                    viewMore.show();
                    show = true;
                    $(divGroup.children()[2]).removeClass("vis-ev-col-fg-rotate");
                }
            }

            var list = groupToCompsMap[name];

            for (var i = 0; i < list.length; i++) {
                var field = list[i];
                var ctrls = compToFieldMap[field.getColumnName()];

                for (var j = 0; j < ctrls.length; j++) {
                    ctrls[j].tag = show;
                    ctrls[j].setVisible(show && field.getIsDisplayed(true));
                }
                if (show && field.getIsDisplayed(true) && (field.getIsFieldgroupDefault() || showGroupFieldDefault))
                    fieldToCompParentMap[field.getColumnName()].show();
                else
                    fieldToCompParentMap[field.getColumnName()].hide();
            }
        };

        function addGroup(fieldGroup) {
            if (oldFieldGroup == null) {
                //addTop();
                oldFieldGroup = "";
            }
            if (fieldGroup == null || fieldGroup.length == 0 || fieldGroup.equals(oldFieldGroup))
                return false;
            oldFieldGroup = fieldGroup;

            //setColumns(columnIndex);
            // clearRowSpan();
            addRow();
            reset();
            initCols(true);
            //<i class="fa fa-ellipsis-h"></i>
            var gDiv = $('<div class="vis-ev-col-fieldgroup" data-showmore="Y" data-name="' + fieldGroup + '" data-display="show">' +
                '<span class="vis-ev-col-fg-hdr"  >' + fieldGroup + ' </span> ' +
                '<span class="vis-ev-col-fg-more">' + VIS.Msg.getMsg("ShowMore") + '</span>' +
                '<i class= "fa fa-angle-up">' +
                '</span>' +
                '</div>');


            $td0.append(gDiv);
            $td0.addClass("vis-ev-col-end4");
            columnIndex = 0;

            //VLine fp = new VLine(fieldGroup);
            gDiv.on("click", onGroupClick);

            return true;
        };

        function addToCompList(comp) {

            if (oldFieldGroup != null && !oldFieldGroup.equals("")) {
                var compList = null;

                if (groupToCompsMap[oldFieldGroup]) {
                    compList = groupToCompsMap[oldFieldGroup];
                }

                if (compList == null) {
                    compList = [];
                    groupToCompsMap[oldFieldGroup] = compList;
                }
                compList.push(comp);
            }
        };

        function addCompToFieldList(name, comp) {

            if (compToFieldMap[name])
                compToFieldMap[name].push(comp);
            else {
                compToFieldMap[name] = [];
                compToFieldMap[name].push(comp);
            }
        }

        function addFieldToGroupList(mField) {

            if (oldFieldGroup != null && !oldFieldGroup.equals("")) {
                var fieldList = null;

                if (groupToCompsMap[oldFieldGroup]) {
                    fieldList = groupToCompsMap[oldFieldGroup];
                }

                if (fieldList == null) {
                    fieldList = [];
                    groupToCompsMap[oldFieldGroup] = fieldList;
                }
                fieldList.push(mField);
                if (!mField.getIsFieldgroupDefault()) {
                    fieldToCompParentMap[mField.getColumnName()].hide();
                }
            }
        };

        this.addField = function (editor, mField) {

            var insertRow = false;

            /* Dont Add in control panel */
            if (mField.getIsLink() && mField.getIsRightPaneLink()) {
                allControls[++allControlCount] = editor;
                //allControls.push(editor);
                allLinkControls.push(editor);
                return;
            }

            var label = VIS.VControlFactory.getLabel(mField);
            if (label == null && editor == null)
                return;
            var sameLine = mField.getIsSameLine();
            if (addGroup(mField.getFieldGroup(), columnIndex)) {
                sameLine = false;
            }



            if (sameLine) {
                ++columnIndex;
                if (columnIndex > 3) {
                    sameLine = false;
                    insertRow = true;
                    // columnIndex = 0;
                }
                else if (columnIndex < 0) {
                    //addRow();
                    insertRow = true;
                    //columnIndex = 0;
                }
            }
            else {
                //columnIndex = 0;
                insertRow = true;
                //addRow();
            }


            adjustLayout(mField, insertRow);


            if (label != null) {


                if (mField.getDescription().length > 0) {
                    //label.getControl().prop('title', mField.getDescription());
                }


                //addToCompList(label);
                //compToFieldMap[label.getName()] = mField;
                addCompToFieldList(mField.getColumnName(), label);
                allControls[++allControlCount] = label;
            }

            if (editor != null) {


                var fieldVFormat = mField.getVFormat();
                var formatErr = mField.getVFormatError();
                switch (fieldVFormat) {
                    case '': {
                        break;
                    }
                    default: {
                        editor.getControl().on("focusout", function (e) {
                            var patt = new RegExp(fieldVFormat);
                            if (VIS.DisplayType.IsString(mField.getDisplayType())) {
                                if ($(e.target).val() != null) {
                                    if ($(e.target).val().toString().trim().length > 0) {
                                        if (!patt.test($(e.target).val())) {
                                            if (!formatErr && formatErr.length > 0) {
                                                formatErr = VIS.Msg.getMsg('RegexFailed') + ":" + mField.getHeader()
                                            }
                                            //Work DOne to set focus in field whose value does not match with regular expression.
                                            VIS.ADialogUI.warn(formatErr, "", function () {
                                                $(e.target).focus();
                                            });

                                        }
                                    }
                                }
                            }
                        });
                    }
                }

                var count = editor.getBtnCount();


                //addToCompList(editor);
                // compToFieldMap[editor.getName()] = mField;
                addCompToFieldList(mField.getColumnName(), editor);
                allControls[++allControlCount] = editor;

            }


            //new design container
            if (label != null || editor != null) {

                var ctnr = _curParent;


                insertCWrapper(label, editor, ctnr, mField);


                fieldToCompParentMap[mField.getColumnName()] = ctnr;
                addFieldToGroupList(mField);
                colDescHelpList[mField.getColumnName()] = {
                    // 'desc': mField.getDescription(),
                    'help': mField.getHelp()
                };
            }
        };

        this.flushLayout = function () {
            addRow();
        };

        this.getRoot = function () {
            return $table;
        };

        this.getComponents = function () {
            return allControls;
        }

        this.getLinkComponents = function () {
            return allLinkControls;
        }

        this.setVisible = function (colName, show) {
            if (fieldToCompParentMap[colName])
                show ? fieldToCompParentMap[colName].show() : fieldToCompParentMap[colName].hide();
        };

        this.dispose = function () {
            $table.off("click", "span.vis-ev-ctrlinfowrap", onInfoClick);
            colDescHelpList = {};
            if (lastPopover) {
                lastPopover.popover('dispose');
            }
            lastPopover = null;

            allLinkControls.length = 0;
            allLinkControls = null;

            while (allControls.length > 0) {
                allControls.pop().dispose();
            };



            // console.log(compToFieldMap);
            for (var p in compToFieldMap) {
                compToFieldMap[p] = null;
                delete compToFieldMap[p];
            }
            compToFieldMap = null;
            fieldToCompParentMap = {};
            fieldToCompParentMap = null;

            // console.log(groupToCompsMap);
            for (var p1 in groupToCompsMap) {
                groupToCompsMap[p1].length = 0;
                groupToCompsMap[p1] = null;
                delete groupToCompsMap[p];
            }
            groupToCompsMap = null;

            allControlCount = null;
            allControls = null;
            $table.remove();
            $table = null;
            this.addField = null;
            addRow = null;
            addToCompList = null;
        };

    };

    /**
     * create wrapeer div and add conrols and label in parent div
     * @param {any} label label to add
     * @param {any} editor controls to add
     * @param {any} parent current row/column div
     * @param {any} mField model field 
     */
    function insertCWrapper(label, editor, parent, mField) {
        var customStyle = mField.getHtmlStyle();

        var wraper = '<div class="input-group vis-input-wrap">';
        //special case for textarea and image button strech height to 100%
        if (editor && (editor.getControl()[0].tagName == 'TEXTAREA' || editor.getControl().hasClass("vis-ev-col-img-ctrl"))) {
            wraper = '<div class="input-group vis-input-wrap vis-ev-full-h">';
        }

        var ctrl = $(wraper);

        if (!mField.getIsLink() && mField.getDisplayType() != VIS.DisplayType.Button) {
            if (mField.getShowIcon() && (mField.getFontClass() != '' || mField.getImageName() != '')) {

                var btns = ['<div class="input-group-prepend"><span class="input-group-text vis-color-primary">'];
                if (mField.getFontClass() != '')
                    btns.push('<i class="' + mField.getFontClass() + '"></i>');
                else
                    btns.push('<img src="' + VIS.Application.contextUrl + 'Images/Thumb16x16/' + mField.getImageName() + '"></img>');
                btns.push('</span></div>');
                ctrl.append(btns.join(' '));

            }
        }

        if (editor != null && customStyle != "") {
            editor.getControl().attr('style', customStyle);
        }

        var ctrlP = $("<div class='vis-control-wrap'>");

        if (editor && (editor.getControl()[0].tagName == 'INPUT' || editor.getControl()[0].tagName == "SELECT" ||
            editor.getControl()[0].tagName == 'TEXTAREA' || editor.getControl()[0].className == 'vis-progressCtrlWrap') && editor.getControl()[0].type != 'checkbox') {
            //editor.getControl().addClass("custom-select");
            ctrlP.append(editor.getControl().attr("placeholder", " ").attr("data-placeholder", ""));
            if (label != null) {
                ctrlP.append(label.getControl());
            }
        }
        else {
            if (label != null)
                ctrlP.append(label.getControl());
            if (editor)
                ctrlP.append(editor.getControl());
        }



        if (mField.getDisplayType() != VIS.DisplayType.Label && !mField.getIsLink()) { // exclude Label display type
            ctrlP.append("<span class='vis-ev-ctrlinfowrap' data-colname='" + mField.getColumnName() + "' title='" + mField.getDescription() + "'  tabindex='-1' data-toggle='popover' data-trigger='focus'>" +
                "<i class='vis vis-info' aria-hidden='true'></i></span'>");
        }

        //if (editor && mField.getDisplayType() == VIS.DisplayType.ProgressBar) {
        //    if (customStyle != "") {
        //        editor.getProgressOutput().attr('style', customStyle);
        //    }
        //    ctrlP.prepend(editor.getProgressOutput());
        //}

        ctrlP.append("<span class='vis-ev-col-msign'><i class='fa fa-exclamation' aria-hidden='true'></span'>");
        ctrl.append(ctrlP);
        if (editor) {
            var count = editor.getBtnCount();
            if (count > 0) {
                editor.getControl().attr("data-hasBtn", " ");
                var i = 0;
                while (i < count) {
                    var btn = editor.getBtn(i);
                    if (btn != null) {
                        ctrl.append($('<div class="input-group-append">').append(btn));
                    }
                    ++i;
                }
                count = -1;
                i = 0;
            }
        }
        parent.append(ctrl);
    }

}(VIS, jQuery));



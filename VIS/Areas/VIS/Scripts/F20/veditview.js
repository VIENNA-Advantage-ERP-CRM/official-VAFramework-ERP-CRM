; (function (VIS, $) {
    //****************************************************//
    //**             VPanel                            **//
    //**************************************************//
    VIS.VGridPanel = function () {

        var oldFieldGroup = null, columnIndex = -2, allControlCount = -1;;
        var allControls = [];
        var allLinkControls = [];

        var $table;
        var $row = null;
        var $td0, $td1, $td11, $td12, $td2, $td3, $td31, $td32, $td4;

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
            //<tr><td class='vis-gc-vpanel-table-td0'><td class='vis-gc-vpanel-table-td1'>" +
            //"<td  class='vis-gc-vpanel-table-td-auto'><td  class='vis-gc-vpanel-table-td-auto'><td class='vis-gc-vpanel-table-td2'>" +
            //"<td  class='vis-gc-vpanel-table-td3'><td  class='vis-gc-vpanel-table-td-auto'><td  class='vis-gc-vpanel-table-td-auto'>" +
            //"<td class='vis-gc-vpanel-table-td4'></tr></table>");
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
               
                curTgt.attr('data-content', colDescHelpList[colName].help).
                    attr('title', colDescHelpList[colName].desc);
                    lastPopover =  curTgt.popover('show');
                
            }
        }

        initComponent();

        function addRow() {
            $td0 = $td1 = $td11 = $td12 = $td2 = $td3 = $td31 = $td32 = $td4 = $row = null;
            // $td0 = $("<td  class='vis-gc-vpanel-table-td0'>");
            $td0 = $("<div class='vis-ev-col'></div>");
            //$td1 = $("<td colspan = '3' class='vis-gc-vpanel-table-td1'>");
            $td1 = $("<div class='vis-ev-col vis-ev-col-start2'></div>");
            //$td2 = $("<td class='vis-gc-vpanel-table-td2'>");
            $td2 = $("<div class='vis-ev-col vis-ev-col-start3' ></div>");
            //$td3 = $("<td colspan='3' class='vis-gc-vpanel-table-td3'>");
            $td3 = $("<div class='vis-ev-col vis-ev-col-start4'></div>");
            //$td4 = $("<td class='vis-gc-vpanel-table-td4'>");

            // $row = $("<tr>");
            // $table.append($row);
            $table.append($td0).append($td1).append($td2).append($td3);//.append($td4);
        }

        function onGroupClick(e) {
            e.stopPropagation();
            var o = $(this);
            var name = o.data("name");
            var dis = o.data("display");

            //console.log(name);
            //console.log(dis);
            var show = false;
            if (dis === "show") {
                o.data("display", "hide");
                $(o.children()[2]).addClass("vis-ev-col-fg-rotate");
            } else {
                o.data("display", "show");
                show = true;
                $(o.children()[2]).removeClass("vis-ev-col-fg-rotate");
            }

            var list = groupToCompsMap[name];
           
            for (var i = 0; i < list.length; i++) {
                var field = list[i];
                var ctrls = compToFieldMap[field.getColumnName()];

                for (var j = 0; j < ctrls.length; j++) {
                    ctrls[j].tag = show;
                    ctrls[j].setVisible(show && field.getIsDisplayed(true));
                }
                if (show && field.getIsDisplayed(true))
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

            setColumns(columnIndex);
            addRow();
            var gDiv = $('<div class="vis-ev-col-fieldgroup" data-name="' + fieldGroup + '" data-display="show">' +
                            '<span class="vis-ev-col-fg-hdr">' + fieldGroup + ' </span> ' +
                            '<span class="vis-ev-col-fg-more" style="display:none"><i class="fa fa-ellipsis-h"></i></span>'+
                                '<i class= "fa fa-angle-up">'+
                            '</span>' +
                          '</div>');


            $td0.append(gDiv);
            columnIndex = 8;

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
            }
        };


        function setColumns() {
            if ($td0 != null) {
                if (columnIndex < 2) {
                    $td0.addClass("vis-ev-col-end2");
                    $td1.removeClass("vis-ev-col-start2").addClass("vis-ev-col-start3").addClass("vis-ev-col-end4");
                    $td2.remove();
                    $td3.remove();
                }
                else if (columnIndex == 8) {
                    $td3.remove();
                    $td2.remove();
                    $td1.remove();
                    $td0.addClass("vis-ev-col-end4");
                }
            }
        }

        this.addField = function (editor, mField) {

            var insertRow = false;

            /* Dont Add in control panel */
            if (mField.getIsLink()) {
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

            if (insertRow) {
                setColumns();
                addRow();
                columnIndex = 0;
            }

            if (label != null) {
                //if (sameLine) {
                //    if (columnIndex == 1) {
                //        $td1.append(label.getControl());
                //    }
                //    else if (columnIndex == 2) {
                //        $td2.append(label.getControl());

                //    }
                //    else {
                //        $td3.append(label.getControl());
                //    }
                //} else {
                //    $td0.append(label.getControl());
                //}

                if (mField.getDescription().length > 0) {
                    //label.getControl().prop('title', mField.getDescription());
                }


                //addToCompList(label);
                //compToFieldMap[label.getName()] = mField;
                addCompToFieldList(mField.getColumnName(),label);
                allControls[++allControlCount] = label;
            }

            if (editor != null) {
                //if (sameLine) {
                //    if (columnIndex == 1) {
                //        $td1.append(editor.getControl());
                //    }
                //    else if (columnIndex == 2) {
                //        $td2.append(editor.getControl());
                //    }
                //    else {
                //        $td3.append(editor.getControl());
                //    }
                //} else {
                //    $td0.append(editor.getControl());
                //}

                var fieldVFormat = mField.getVFormat();
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
                                            //Work DOne to set focus in field whose value does not match with regular expression.
                                            VIS.ADialogUI.warn(VIS.Msg.getMsg('RegexFailed') + ":" + mField.getHeader(), "", function () {
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

                //// if (count > 0) {
                // if (0 > 0) {

                //     while (count > 0) {
                //         var ctrl = editor.getBtn(count - 1);

                //         if (ctrl != null) {
                //             if (sameLine) {
                //                 $("<td class='vis-gc-vpanel-table-td-fix30'>").append(ctrl).insertAfter($td3);
                //             }
                //             else {
                //                 $("<td class='vis-gc-vpanel-table-td-fix30'>").append(ctrl).insertAfter($td1);
                //             }
                //         }
                //         --count;
                //     }
                //     count = -1;
                //     ctrl = null;
                // }
                // else {

                // }

                count = editor.getBtnCount();


                //if (!sameLine && mField.getIsLongField()) {
                //    $td1.remove();
                //    $td2.remove();
                //    $td3.remove();
                //    $td0.addClass("vis-ev-col-end4");
                //    columnIndex = 4;
                //}
                //else {
                //    if (sameLine) {
                //        //if (columnIndex == 1)
                //        //    $td1.addClass("vis-ev-col-start2 vis-ev-col-end2");
                //        //else if (columnIndex == 2)
                //        //    $td2.addClass("vis-ev-col-start3 vis-ev-col-end3");
                //        //else 
                //        //    $td3.addClass("vis-ev-col-start4 vis-ev-col-end4");

                //       // columnIndex += 1;
                //    } else {
                //       // $td1.prop('colspan', 3 - count);
                //        //$td3.prop('colspan', 3);
                //    }
                //}
                count = 0;

                //addToCompList(editor);
               // compToFieldMap[editor.getName()] = mField;
                addCompToFieldList(mField.getColumnName(),editor);
                allControls[++allControlCount] = editor;

            }


            //new design container
            if (label != null || editor != null) {
                addFieldToGroupList(mField);
                var ctnr = null;
                if (sameLine) {
                    if (columnIndex == 1) {
                        //$td1.append(editor.getControl());
                        insertCWrapper(label, editor, $td1, mField);
                        ctnr = $td1;
                    }
                    else if (columnIndex == 2) {
                        insertCWrapper(label, editor, $td2, mField);
                        ctnr = $td2;
                    }
                    else {
                        insertCWrapper(label, editor, $td3, mField);
                        ctnr = $td3;
                    }
                } else {
                    insertCWrapper(label, editor, $td0, mField);
                    ctnr = $td0;
                }

                if (!sameLine && mField.getIsLongField()) {
                    $td1.remove();
                    $td2.remove();
                    $td3.remove();
                    $td0.addClass("vis-ev-col-end4");
                    columnIndex = 4;
                }

                fieldToCompParentMap[mField.getColumnName()] = ctnr;

                colDescHelpList[mField.getColumnName()] = {
                    'desc': mField.getDescription(),
                    'help': mField.getHelp()
                };
            }
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

    function insertCWrapper(label, editor, parent, mField) {
        var customStyle = mField.getHtmlStyle();

        var ctrl = $('<div class="input-group vis-input-wrap">');
        if (mField.getShowIcon() && (mField.getFontClass() != '' || mField.getImageName() != '')) {
            var btns = ['<div class="input-group-prepend"><span class="input-group-text vis-color-primary">'];
            if (mField.getFontClass() != '')
                btns.push('<i class="' + mField.getFontClass() + '"></i>');
            else
                btns.push('<img src="' + VIS.Application.contextUrl + 'Images/Thumb16x16/' + mFiled.getImageName() + '"></img>');
            btns.push('</span></div>');
            ctrl.append(btns.join(' '));
            
        }

        if (editor != null && customStyle != "") {
            editor.getControl().attr('style', customStyle);
        }

        var ctrlP = $("<div class='vis-control-wrap'>");

        if (editor && (editor.getControl()[0].tagName == 'INPUT' || editor.getControl()[0].tagName == "SELECT" ||
            editor.getControl()[0].tagName == 'TEXTAREA') && editor.getControl()[0].type != 'checkbox') {
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

        

        ctrlP.append("<span class='vis-ev-ctrlinfowrap' data-colname='" + mField.getColumnName()+"' title=''  tabindex='0' data-toggle='popover' data-trigger='focus'>"+
            "<i class='vis vis-info' aria-hidden='true'></i></span'>");

        ctrlP.append("<span class='vis-ev-col-msign'><i class='fa fa-exclamation' aria-hidden='true'></span'>");
        ctrl.append(ctrlP);
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
        parent.append(ctrl);
    }

}(VIS, jQuery));



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


    function initComponent() {
        $table = $("<div class='vis-ad-w-p-vc-editview'>"); //   $("<table class='vis-gc-vpanel-table'>");
        //<tr><td class='vis-gc-vpanel-table-td0'><td class='vis-gc-vpanel-table-td1'>" +
        //"<td  class='vis-gc-vpanel-table-td-auto'><td  class='vis-gc-vpanel-table-td-auto'><td class='vis-gc-vpanel-table-td2'>" +
        //"<td  class='vis-gc-vpanel-table-td3'><td  class='vis-gc-vpanel-table-td-auto'><td  class='vis-gc-vpanel-table-td-auto'>" +
        //"<td class='vis-gc-vpanel-table-td4'></tr></table>");
    };

    initComponent();

    function addRow() {
        $td0 = $td1 = $td11 = $td12 = $td2 = $td3 = $td31 = $td32 = $td4 = $row = null;
       // $td0 = $("<td  class='vis-gc-vpanel-table-td0'>");
        $td0 = $("<div class='vis-ev-col'></div>");
        //$td1 = $("<td colspan = '3' class='vis-gc-vpanel-table-td1'>");
        $td1 = $("<div class='vis-ev-col vis-ev-col-start2'></div>");
        //$td2 = $("<td class='vis-gc-vpanel-table-td2'>");
        $td2 = $("<div class='vis-ev-col vis-ev-col-start3'></div>");
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
            $(o.children()[0]).addClass("vis-gc-vpanel-fieldgroup-img-rotate");
        } else {
            o.data("display", "show");
            show = true;
            $(o.children()[0]).removeClass("vis-gc-vpanel-fieldgroup-img-rotate");
        }

        var list = groupToCompsMap[name];
        for (var i = 0; i < list.length; i++) {
            var field = compToFieldMap[list[i].getName()];
            list[i].tag = show;
            list[i].setVisible(show && field.getIsDisplayed(true));
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

        addRow();
        var gSpan = $("<span class='vis-gc-vpanel-fieldgroup-span'>" + fieldGroup + "</span>");
        var gImg = $("<img class='vis-gc-vpanel-fieldgroup-img' src= '" + VIS.Application.contextUrl + "Areas/VIS/Images/base/fieldgrpdown.png' >");
        var gDiv = $("<div class='vis-gc-vpanel-fieldgroup' data-name='" + fieldGroup + "' data-display='show' >").append(gImg).append(gSpan);
        //$td1.attr("colspan", 3);
        $td3.remove();
        $td2.remove();
        $td1.prop('colspan', 8);
        $td1.append(gDiv);

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

    this.addField = function (editor, mField) {

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
        if (addGroup(mField.getFieldGroup())) {
            sameLine = false;
        }

        if (sameLine) {
            ++columnIndex;
            if (columnIndex > 4) {
                sameLine = false;
                addRow();
                columnIndex = 0;
            }
            else if (columnIndex < 0) {
                addRow();
                columnIndex = 0;
            }

        }
        else {
            columnIndex = 0;
            addRow();
        }

        if (label != null) {
            if (sameLine) {
                if (columnIndex == 1)
                    $td1.append(label.getControl());
                else if (columnIndex == 2)
                    $td2.append(label.getControl());
                else
                    $td3.append(label.getControl());
            } else {
                $td0.append(label.getControl());
            }

            if (mField.getDescription().length > 0) {
                label.getControl().prop('title', mField.getDescription());
            }


            addToCompList(label);
            compToFieldMap[label.getName()] = mField;
            allControls[++allControlCount] = label;
        }
        if (editor != null) {
            if (sameLine) {
                if (columnIndex == 1)
                    $td1.append(editor.getControl());
                else if (columnIndex==2)
                    $td2.append(editor.getControl());
                else
                    $td3.append(editor.getControl());
            } else {
                $td0.append(editor.getControl());
            }
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

           // if (count > 0) {
            if (0 > 0) {

                while (count > 0) {
                    var ctrl = editor.getBtn(count - 1);

                    if (ctrl != null) {
                        if (sameLine) {
                            $("<td class='vis-gc-vpanel-table-td-fix30'>").append(ctrl).insertAfter($td3);
                        }
                        else {
                            $("<td class='vis-gc-vpanel-table-td-fix30'>").append(ctrl).insertAfter($td1);
                        }
                    }
                    --count;
                }
                count = -1;
                ctrl = null;
            }
            else {

            }

            count = editor.getBtnCount();


            if (!sameLine && mField.getIsLongField()) {
                $td1.remove();
                $td2.remove();
                $td3.remove();
                $td0.addClass("vis-ev-col-end4");
                columnIndex = 4;
            }
            else {
                if (sameLine) {
                    //if (columnIndex == 1)
                    //    $td1.addClass("vis-ev-col-start2 vis-ev-col-end2");
                    //else if (columnIndex == 2)
                    //    $td2.addClass("vis-ev-col-start3 vis-ev-col-end3");
                    //else 
                    //    $td3.addClass("vis-ev-col-start4 vis-ev-col-end4");

                    columnIndex += 1;
                } else {
                   // $td1.prop('colspan', 3 - count);
                    //$td3.prop('colspan', 3);
                }
            }
            count = 0;

            addToCompList(editor);
            compToFieldMap[editor.getName()] = mField;
            allControls[++allControlCount] = editor;
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
}(VIS, jQuery));
; (function (VIS, $) {

    VIS.FilterPanel.Controls = VIS.FilterPanel.Controls || {};
    VIS.FilterPanel.Controls.CheckBoxList = function (SelectedValues, columnName, AD_Column_ID, displayType, container, data, isSearch, infoWinID, colName, tableName, windowNo, isMultiSearch) {
        var datasource = [];
        var self = this;
        var values = [];
        if (data != null) {
            for (var i = 0; i < data.length; i++) {
                datasource.push({ 'label': data[i].Name, 'value': data[i].Name, 'ids': data[i].Key });
            }
        }

        this.dataa = datasource;

        var mainDiv = $('<div style="max-height:none" class="vadb-data">');

        this.div = null;
        var btn = null;

        if (isSearch == true) {
            this.div = $('<input style="width:185px" placeholder="' + VIS.Msg.getMsg("Search") + '" >')
            if (VIS.Application.isRTL) {
                btn = $('<img style="display:inline;margin-top:-3px" title=' + VIS.Msg.getMsg("Search") + ' src="' + VIS.Application.contextUrl + 'Areas/VADB/Images/search.png"></img>');
            }
            else {
                btn = $('<img style="display:inline;margin-top:-3px" title=' + VIS.Msg.getMsg("Search") + ' src="' + VIS.Application.contextUrl + 'Areas/VADB/Images/search.png"></img>');
            }
        }
        else {
            this.div = $('<input style="width:100%"  placeholder="' + VIS.Msg.getMsg("Search") + '" >');
            if (VIS.Application.isRTL) {
                btn = $('<img style="display:inline;margin-top:-3px;margin-right:-15px;cursor:pointer;position:relative" title=' + VIS.Msg.getMsg("Search") + ' src="' + VIS.Application.contextUrl + 'Areas/VADB/Images/open-arrow.png" ></img>');
            }
            else {
                btn = $('<img style="display:inline;margin-top:-3px;margin-left:-15px;cursor:pointer" title=' + VIS.Msg.getMsg("Search") + ' src="' + VIS.Application.contextUrl + 'Areas/VADB/Images/open-arrow.png" ></img>');
            }
        }
        var divCheckBox = $('<div class="vadb-checkboxlist" >');

        //this.div.autocomplete({
        //    minLength: 0,
        //    source: datasource,
        //    select: function (ev, ui) {
        //        divCheckBox.append('<input type="checkbox" checked value="' + ui.item.ids + '">' + ui.item.label + '</br>');
        //        this.value = "";
        //        return false;
        //    }
        //});


        this.div.autocomplete({
            minLength: 0,
            source: function (request, response) {
                if (request.term.trim().length == 0) {
                    return;
                }

                fillAutoCompleteonTextChange(request.term, response);
            },
            select: function (ev, ui) {
                divCheckBox.append('<input type="checkbox" checked value="' + ui.item.ids + '">' + ui.item.label + '</br>');
                this.value = "";
            }
        });


        function fillAutoCompleteonTextChange(text, response) {

            var url = ''
            if (displayType == VIS.DisplayType.TableDir || displayType == 30) {
                url = VIS.Application.contextUrl + "VADB/AdvancedSearch/LoadTableDirectValues";
            }
            else if (displayType == VIS.DisplayType.Table) {
                url = VIS.Application.contextUrl + "VADB/AdvancedSearch/LoadTableValues";
            }

            $.ajax({
                url: url,
                dataType: "json",
                data: {
                    AD_Column_ID: AD_Column_ID, ColumnName: columnName,
                    name_startsWith: text
                },
                success: function (data) {

                    var result = JSON.parse(data);


                    datasource = [];
                    //for (var i = 0; i < result.length; i++) {
                    //    datasource.push({ 'label': result[i].Name, 'value': result[i].Name, 'ids': result[i].Key });
                    //}
                    //$(self.div).autocomplete('option', 'source', datasource)
                    //$(self.div).autocomplete("search", "");
                    //$(self.div).trigger("focus");

                    response($.map(result, function (item) {
                        return {
                            label: item.Name,
                            value: item.Name,
                            ids: item.Key
                        }


                    }));
                    $(self.div).autocomplete("search", "");
                    $(self.div).trigger("focus");

                }
            });
        };


        this.getSelectedValues = function () {
            var checks = divCheckBox.children('input');
            if (checks.length > 0) {
                for (var i = 0; i < checks.length; i++) {
                    if (checks[i].prop('checked')) {
                        values.push($(checks[i]).data("ids"));
                    }
                }
            }
        };


        if (isSearch == true) {
            mainDiv.append(this.div).append(btn).append(divCheckBox);
            btn.on("click", btnClickforInfo);
        }
        else {
            mainDiv.append(this.div).append(btn).append(divCheckBox);
            btn.on("click", btnClick);
        }


        container.append(mainDiv);


        function btnClickforInfo() {
            var InfoWindow = null;
            if (infoWinID > 0) {
                InfoWindow = new VIS.InfoWindow(infoWinID, "", windowNo, "", isMultiSearch);
            }
            else {
                InfoWindow = new VIS.infoGeneral(true, windowNo, "",
                    tableName, colName, isMultiSearch, "");
            }


            InfoWindow.onClose = function () {
                //self.setValue(InfoWindow.getSelectedValues(), false, true);

                var objResult = InfoWindow.getSelectedValues();

                for (i = 0, j = objResult.length; i < j; i++) {

                    var elements = $.grep(self.dataa, function (ele, index) {
                        return ele.ids == objResult[i];
                    });
                    if (elements != null && elements != undefined && elements.length > 0) {
                        //var control = elements[0].label;
                        divCheckBox.append('<input type="checkbox" checked value="' + elements[0].ids + '">' + elements[0].label + '</br>')
                    }
                }

                //divCheckBox.append('<input type="checkbox" checked value="' + ui.item.ids + '">' + ui.item.label + '</br>');



            };

            InfoWindow.show();


        };


        function btnClick(e) {
            if (displayType == VIS.DisplayType.TableDir || displayType == 30) {
                updateAutoComplete(VIS.Application.contextUrl + "VADB/AdvancedSearch/LoadTableDirectValues");
            }
            else if (displayType == VIS.DisplayType.Table) {
                updateAutoComplete(VIS.Application.contextUrl + "VADB/AdvancedSearch/LoadTableValues");
            }
        };

        if (SelectedValues != null && SelectedValues != undefined) {
            btnClick();
        }


        function updateAutoComplete(url) {
            $.ajax({
                url: url,
                datatype: "json",
                type: "get",
                cache: false,
                data: { AD_Column_ID: AD_Column_ID, ColumnName: columnName },
                success: function (data) {
                    var result = JSON.parse(data);
                    datasource = [];

                    if (SelectedValues != null && SelectedValues != undefined) {
                        SelectedValues = SelectedValues.toString().replace(/'/g, "");
                        SelectedValues = SelectedValues.split(', ');
                    }

                    for (var i = 0; i < result.length; i++) {
                        datasource.push({ 'label': result[i].Name, 'value': result[i].Name, 'ids': result[i].Key });
                        if (SelectedValues != null && SelectedValues != undefined) {
                            if (SelectedValues.indexOf(result[i].Key) > -1) {
                                divCheckBox.append('<input type="checkbox" checked value="' + result[i].Key + '">' + result[i].Name + '</br>')
                            }
                        }

                    }
                    $(self.div).autocomplete('option', 'source', datasource)

                    if (SelectedValues == null || SelectedValues == undefined) {
                        $(self.div).autocomplete("search", "");
                        $(self.div).trigger("focus");
                    }
                    SelectedValues = null;
                }
            });
        };


    };
}
)(VIS, jQuery);



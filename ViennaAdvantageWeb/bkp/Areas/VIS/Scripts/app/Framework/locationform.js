
; (function (VIS, $) {

    //form declaration
    // added new parameter for Maintain Versions
    function LocationForm(locationId, maintainVersinos) {

        //call parent function on close
        this.onClose = null;

        var $C_Location_ID = locationId;
        var $self = this;
        var $root = $("<div style='position:relative;'>");
        var $busyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
        var windowNo = VIS.Env.getWindowNo();

        var searchlst = null;
        var country = null;
        var add1 = null;
        var add2 = null;
        var add3 = null;
        var add4 = null;

        var city = null;
        var state = null;
        var zip = null;

        var contryId = null;
        var stateId = null;
        var cityId = null;

        var Okbtn = null;
        var cancelbtn = null;
        var customAddList = null;

        var stringAddress = null;
        var change = false;

        var maintainVer = maintainVersinos;

        this.load = function () {
            // Parameter - AD_Language - Added to get country from location
            $root.load(VIS.Application.contextUrl + 'Location/Locations/?windowno=' + windowNo + '&locationId=' + $C_Location_ID + '&AD_Language=' + VIS.context.getContext("#AD_Language"), function (event) {
                $root.append($busyDiv);
                setBusy(true);
                $self.init();
                setBusy(false);
            });
        };

        this.init = function () {

            Okbtn = $root.find("#btnOk_" + windowNo);
            cancelbtn = $root.find("#btnCancel_" + windowNo);
            searchlst = $root.find("#lstLocation_" + windowNo);
            country = $root.find("#txtCountry_" + windowNo);
            add1 = $root.find("#txtAddress1_" + windowNo);
            add2 = $root.find("#txtAddress2_" + windowNo);
            add3 = $root.find("#txtAddress3_" + windowNo);
            add4 = $root.find("#txtAddress4_" + windowNo);

            //check Arebic Calture
            if (VIS.Application.isRTL) {
                Okbtn.removeClass("pull-left");
                Okbtn.addClass("pull-right");
                $root.find("Sup").css("float", "right");
                $($root.find(".cat2 a")[0]).removeClass("pull-right");
                $($root.find(".cat2 a")[0]).addClass("pull-left");
                $($root.find(".cat3 a")[0]).removeClass("pull-right");
                $($root.find(".cat3 a")[0]).addClass("pull-left");
                //$($root.find(".cat2 a")[0]).css("margin-left", "10px");
                //$($root.find(".cat3 a")[0]).css("margin-left", "10px");
            }


            if (add1.val()) {
                add1.val((add1.val()));
            }

            if (add2.val()) {
                add2.val((add2.val()));
            }

            if (add3.val()) {
                var obj = $root.find("#aCollection");
                var ctrl = $root.find('.cat' + $(obj).attr('data-prod-cat'));
                if (ctrl) {
                    if (ctrl.length > 0)
                        ctrl.toggle();
                }
                else {
                    $root.find('.cat' + $(obj).attr('data-prod-close')).hide()
                }
                add3.val((add3.val()));
            }

            if (add4.val()) {
                var obj = $root.find(".cat3").find("a")[0];
                var ctrl = $root.find('.cat' + $(obj).attr('data-prod-cat'));
                if (ctrl) {
                    if (ctrl.length > 0)
                        ctrl.toggle();
                }
                else {
                    $root.find('.cat' + $(obj).attr('data-prod-close')).hide()
                }
                add4.val((add4.val()));
            }

            city = $root.find("#txtCity_" + windowNo);
            if (city.val()) {
                city.val((city.val()));
            }

            state = $root.find("#txtState_" + windowNo);
            zip = $root.find("#txtZipCode_" + windowNo);
            if (zip.val()) {
                zip.val((zip.val()));
            }
            contryId = $root.find("#countryhdn_" + windowNo).val();
            stateId = $root.find("#Statehdn_" + windowNo).val();


            /*Country Fill*/
            country.autocomplete({
                source: function (request, response) {
                    if (request.term.trim().length == 0) {
                        return;
                    }
                    $.ajax({
                        url: VIS.Application.contextUrl + "Location/GetCountry",
                        dataType: "json",
                        data: {
                            //featureClass: "P",
                            style: "full",
                            //maxRows: 12,
                            name_startsWith: request.term
                        },
                        success: function (data) {
                            response($.map(data.result, function (item) {
                                return {
                                    label: item.Name,
                                    value: item.Name,
                                    id: item.Key
                                }
                            }));
                        }
                    });
                },
                minLength: 1,
                select: function (event, ui) {
                    contryId = ui.item.id;
                    //var contryId = null;
                    //var stateId = null;
                    //var cityId = null;
                },
                open: function () {
                    $root.find(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                },
                close: function () {
                    $root.find(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                    //state.val("");
                    //city.val("");
                    //stateId = 0;
                    //cityId = 0;
                    if (Number(contryId) > 0) {
                        change = true;
                    }
                }
            });

            /*State Fill*/
            state.autocomplete({
                source: function (request, response) {
                    if (request.term.trim().length == 0) {
                        return;
                    }
                    $.ajax({
                        url: VIS.Application.contextUrl + "Location/GetStates",
                        dataType: "json",
                        data: {
                            style: "full",
                            name_startsWith: request.term,
                            countryId: contryId
                        },
                        success: function (data) {
                            response($.map(data.result, function (item) {
                                return {
                                    label: item.Name,
                                    value: item.Name,
                                    id: item.Key
                                }
                            }));
                        }
                    });
                },
                minLength: 1,
                select: function (event, ui) {
                    stateId = ui.item.id;
                },
                open: function () {
                    $root.find(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                },
                close: function () {
                    $root.find(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                }
            });

            /*Address Fill*/
            searchlst.autocomplete({
                source: function (request, response) {
                    if (request.term.trim().length == 0) {
                        return;
                    }

                    $.ajax({
                        url: VIS.Application.contextUrl + "Location/GetAddresses",
                        dataType: "json",
                        data: {
                            style: "full",
                            name_startsWith: request.term
                        },
                        success: function (data) {
                            response($.map(data.result, function (item) {
                                return {
                                    label: item.ADDRESS,
                                    value: item.ADDRESS,
                                    C_COUNTRY_ID: item.C_COUNTRY_ID,
                                    C_LOCATION_ID: item.C_LOCATION_ID,
                                    C_REGION_ID: item.C_REGION_ID,
                                    COUNTRYNAME: item.COUNTRYNAME,
                                    ADDRESS1: item.ADDRESS1,
                                    ADDRESS2: item.ADDRESS2,
                                    ADDRESS3: item.ADDRESS3,
                                    ADDRESS4: item.ADDRESS4,
                                    CITYNAME: item.CITYNAME,
                                    STATENAME: item.STATENAME,
                                    ZIPCODE: item.ZIPCODE
                                }
                            }));
                        }
                    });
                },
                minLength: 1,
                select: function (event, ui) {
                    country.val(ui.item.COUNTRYNAME);
                    add1.val(ui.item.ADDRESS1);
                    add2.val(ui.item.ADDRESS2);
                    add3.val(ui.item.ADDRESS3);
                    add4.val(ui.item.ADDRESS4);
                    city.val(ui.item.CITYNAME);
                    state.val(ui.item.STATENAME);
                    zip.val(ui.item.ZIPCODE);
                    change = true;
                    stateId = ui.item.C_REGION_ID;
                    $C_Location_ID = 0;//ui.item.C_LOCATION_ID;    
                    cityId = 0;
                    contryId = ui.item.C_COUNTRY_ID;
                },
                open: function () {
                    $root.find(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                },
                close: function () {
                    $root.find(this).removeClass("ui-corner-top").addClass("ui-corner-all");

                }
            });


            Okbtn.on("click", function () {
                setBusy(true);
                if (Number(contryId) <= 0) {
                    VIS.ADialog.warn("SelectCountry", true, null);
                    setBusy(false);
                    return;
                }

                // Set C_Location_ID as 0, 
                // if maintain Version is marked on column and there is any change in value on Location Control
                if (maintainVer && change)
                    $C_Location_ID = 0;

                var objValue = {
                    countryName: country.val(),
                    addvalue1: add1.val(),
                    addvalue2: add2.val(),
                    addvalue3: add3.val(),
                    addvalue4: add4.val(),
                    cityValue: city.val(),
                    stateValue: state.val(),
                    zipValue: zip.val(),
                    clocationId: $C_Location_ID,
                    countryId: contryId,
                    stateId: stateId,
                    cityId: cityId
                };

                var callbackValue = saveLocation(objValue);
            });

            cancelbtn.on("click", function () {
                $root.dialog('close');
            });

            $root.find(".VIS-Location-toggler").click(function (e) {
                e.preventDefault();
                var ctrl = $root.find('.cat' + $root.find(this).attr('data-prod-cat'));
                if (ctrl && ctrl.length > 0) {
                        ctrl.toggle();
                }
                else {
                    $root.find('.cat' + $root.find(this).attr('data-prod-close')).hide()
                }
                // $('img.expand').toggleClass('collapse');
            });

            country.bind('change', function (e) {
                change = true;
            });
            add1.bind('change', function (e) {
                change = true;
            });
            add2.bind('change', function (e) {
                change = true;
            });
            add3.bind('change', function (e) {
                change = true;
            });
            add4.bind('change', function (e) {
                change = true;
            });

            city.bind('change', function (e) {
                change = true;
            });
            state.bind('change', function (e) {
                change = true;
            });
            zip.bind('change', function (e) {
                change = true;
            });

            //Save data in the database
            function saveLocation(data, callback) {
                var result = null;
                $.ajax({
                    url: VIS.Application.contextUrl + "Location/SaveLocation",
                    type: "POST",
                    datatype: "json",
                    contentType: "application/json; charset=utf-8",
                    async: false,
                    data: JSON.stringify({ pref: data })
                }).done(function (json) {
                    result = json;
                    $C_Location_ID = result.locationid;
                    stringAddress = result.locaddress;
                    this.location = $C_Location_ID;
                    setBusy(false);
                    if ($self.onClose)
                        $self.onClose($C_Location_ID, change);
                    $root.dialog('close');
                    change = null;
                })
            };
        };

        function setBusy(isBusy) {
            $busyDiv.css("display", isBusy ? 'block' : 'none');
        };

        this.showDialog = function () {
            $root.append($busyDiv);
            $root.dialog({
                modal: true,
                resizable: false,
                title: VIS.Msg.getMsg("Location"),
                closeText: VIS.Msg.getMsg("close"),
                // height: 440,
                width: 620,
                position: { at: "center top", of: window },
                close: function () {
                    $self.dispose();
                    $self = null;
                    $root.dialog("destroy");
                    $root = null;
                }
            });
        };

        this.disposeComponent = function () {
            $self = null;
            if (Okbtn)
                Okbtn.off("click");
            if (cancelbtn)
                cancelbtn.off("click");

            $C_Location_ID = 0;
            searchlst = null;
            country = null;
            add1 = null;
            add2 = null;
            add3 = null;
            add4 = null;

            city = null;
            state = null;
            zip = null;

            contryId = null;
            stateId = null;
            cityId = null;

            Okbtn = null;
            cancelbtn = null;
            customAddList = null;

            stringAddress = null;



            this.disposeComponent = null;
        };
    };

    //dispose call
    LocationForm.prototype.dispose = function () {

        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();
    };

    //Load form into VIS
    VIS.LocationForm = LocationForm;

})(VIS, jQuery);
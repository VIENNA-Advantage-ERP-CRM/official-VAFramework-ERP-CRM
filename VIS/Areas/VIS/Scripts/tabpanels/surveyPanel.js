VIS = window.VIS || {};
(function () {

    function surveyPanel() {
        this.record_ID = 0;
        this.windowNo = 0;
        this.curTab = null;
        this.selectedRow = null;
        this.panelWidth;
        this.extraInfo = null;
        var self = this;
        var $root;
        var questionSection = null;
        var responseSection = null;
        var _AD_Window_ID = 0;
        var _AD_Tab_ID = 0;
        var iFrame;
        var IsMandatoryAll = false;      
        var pageSize = 0;
        var Limit = 0;
        var AD_SurveyAssignment_ID = 0;
        var AD_SurveyResponse_ID = 0;
        var ResponseCount = 0;
        var isSelfShow = true;
       
        this.init = function () {
            $root = $('<div></div>');

            var tab = $('<div class="vis-surveyTab">'
                + '<div class="vis-tabPrimary">'
                + '<ul class="nav vis-primarySection nav-tabs mb-3" id="myTab" role="tablist">'
                + '<li class="nav-item vis-firstResLink  text-center">'
                + '  <a'
                + '    class="nav-link active"'
                + '    id="home-tab"'
                + '    data-toggle="tab"'
                + '    href="#ques_' + self.windowNo+'"'
                + '    >Questions</a>'
                + '</li>'
                + '<li class="nav-item text-center">'
                + '  <a'
                + '    class="nav-link"'
                + '    id="profile-tab"'
                + '    data-toggle="tab"'
                + '    href="#resp_' + self.windowNo +'"'
                + '    >Responses <span class="badge badge-light responseCount_' + self.windowNo+'"></span></a>'
                + '</li>'
                + '</ul>'
                + '<div class="tab-content" id="myTabContent">'
                + '<div class="tab-pane fade show active" style="height:57vh;width:100%;overflow:auto !important;" id="ques_' + self.windowNo +'">'                
                + '</div>'
                + '<div class="tab-pane fade" style="height:57vh;width:100%;overflow:auto !important;" id="resp_' + self.windowNo + '">'
                + '<div class="text-right mr-2">'
                +'<span class="d-inline-block mr-1">Select User</span>'
                + '<select></select>'
                + '</div > '
                + '<div class="response"></div>'
                + '</div > '
                + '</div>'
                + '</div>'
                + '</div>'
            );
            $root.append(tab);
            questionSection = $root.find('#ques_' + self.windowNo);
            responseSection = $root.find('#resp_' + self.windowNo);
           // panelDetails(this.curTab.vo.AD_Window_ID, this.curTab.vo.AD_Tab_ID, $root);
        };

        this.update = function (Record_ID) {
            questionSection.empty();
            responseSection.find('.response').empty();
            IsMandatoryAll = false;
            pageSize = 0;
            panelDetails(this.curTab.vo.AD_Window_ID, this.curTab.vo.AD_Tab_ID, $root);
        }
            
        var panelDetails = function (AD_window_ID, AD_Tab_ID, root) {
            _AD_Window_ID = AD_window_ID;
            _AD_Tab_ID = AD_Tab_ID;
            $.ajax({
                url: VIS.Application.contextUrl + "VIS/SurveyPanel/GetSurveyAssignments",
                //async: false,
                data: { AD_window_ID: AD_window_ID, AD_Tab_ID: AD_Tab_ID, AD_Table_ID: self.curTab.getAD_Table_ID(), AD_Record_ID: self.curTab.getRecord_ID()},
                success: function (data) {
                    var res = [];
                    res = JSON.parse(data);
                    if (res != null && res.length > 0) {
                        IsMandatoryAll = res[0].IsMandatory;
                        pageSize = res[0].QuestionsPerPage;
                        isSelfShow = res[0].IsSelfshow;
                        Limit = res[0].Limit;
                        ResponseCount = res[0].ResponseCount;
                        AD_SurveyAssignment_ID = res[0].SurveyAssignment_ID;
                        AD_SurveyResponse_ID = res[0].SurveyResponse_ID;
                        loadSurveyUI(res[0].Survey_ID, res[0].SurveyType, root);                        
                    };
                },
                error: function (e) {
                }
            });
        };

        this.getRoot = function () {
            return $root;
        };

        this.disposeComponent = function () {
            if (iFrame)
                iFrame.remove();
            $root.remove();
            iFrame = null;
            $root = null;
        };

        /**
         * Load Survey UI
         * @param {any} AD_Survey_ID
         * @param {any} SurveyType
         * @param {any} $root
         */
        function loadSurveyUI(AD_Survey_ID, SurveyType, root) {
            $.ajax({
                url: VIS.Application.contextUrl + "VIS/SurveyPanel/GetSurveyItems",
                //async: false,
                data: { AD_Survey_ID: AD_Survey_ID },
                success: function (data) {
                    var res = [];
                    res = JSON.parse(data);
                    if (res != null) {
                        loadSurveyDesign(res, SurveyType, root, AD_Survey_ID);
                        findControls();
                    };
                },
                error: function (e) {
                }
            });
        };

        /**
         * Create survey Question Answer
         * @param {any} SurveyData
         * @param {any} SurveyType
         * @param {any} $root
         */
        function loadSurveyDesign(SurveyData, SurveyType, root, AD_Survey_ID) {
            var $dsgn;
            var dsg = '<div class="VIS_SI_Main' + self.windowNo + '" style="width:100%; height:calc(100% - 38px);">' +
                '<div class="vis-tp-mainContainer"> ' +
                '<ol class="list-unstyled vis-tp-orderListWrap"> ';
            if (SurveyType == "CL") //if survey type is Checklist.
            {

                if (SurveyData.length > 0) {
                    for (var i = 0; i < SurveyData.length; i++) {
                        dsg += '<li class="VIS_SI_' + SurveyData[i].Item.AD_SurveyItem_ID + ' align-items-center d-flex mb-3 pb-3 vis-tp-listItem">' +
                            '<h6 class="mr-2 mb-0">' + (i + 1) + '.</h6><input class="VIS_Answ_' + self.windowNo + '" data-id="VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + '" type= "checkbox" ><p class="VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + '" data-qtype="' + SurveyType + '" data-mandatory="' + SurveyData[i].Item.IsMandatory + '">' + SurveyData[i].Item.Question;

                        if (IsMandatoryAll || SurveyData[i].Item.IsMandatory == 'Y') {
                            dsg += '<sub style="color:red;font-size: 100%;bottom: unset;">*</sub>';
                        }
                        dsg += '</p>' +
                            '</li>';
                    }
                }
                dsg += '</ol>';

                //$dsgn = $(dsg);
            }
            else // if survey type is Questionnarie.
            {
                if (SurveyData.length > 0) {
                    for (var i = 0; i < SurveyData.length; i++) {
                        if (SurveyData[i].Item.AnswerType == "CB") { //if answer type check box
                            dsg += '<li class="mb-3"> ' +
                                '<h6 data-qtype="' + SurveyData[i].Item.AnswerType + '" data-mandatory="' + SurveyData[i].Item.IsMandatory + '" class= "VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + ' mb-3 vis-tp-qus ml"> ' + (i + 1) + '. ' + SurveyData[i].Item.Question;
                            if (IsMandatoryAll || SurveyData[i].Item.IsMandatory == 'Y') {
                                dsg += '<sub style="color:red;font-size: 100%;bottom: unset;">*</sub>';
                            }
                            dsg += '</h6 > ' +
                                '<div class="vis-tp-listWrap"> ';
                            for (var j = 0; j < SurveyData[i].Values.length; j++) {
                                dsg += ' <div class="VIS_SI_' + SurveyData[i].Values[j].AD_SurveyValue_ID + ' align-items-center d-flex mb-3 vis-tp-listItem"> ' +
                                    ' <input class="VIS_Answ_' + SurveyData[i].Values[j].AD_SurveyValue_ID + '" data-id="VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + '"';
                                if (SurveyData[i].Item.AnswerSelection == 'SL') {
                                    dsg += ' class="group_' + SurveyData[i].Item.AD_SurveyItem_ID + '" ';
                                }
                                dsg += ' data-surveyitem=' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-surveyvalue=' + SurveyData[i].Values[j].AD_SurveyValue_ID + ' data-survey=' + SurveyData[i].Item.AD_Survey_ID + ' value="' + SurveyData[i].Values[j].Answer + '" type="checkbox">' +
                                    ' <p>' + SurveyData[i].Values[j].Answer + '</p>' +
                                    ' </div>';
                            }
                            dsg += '</div> ' +
                                '</li > ';
                        }
                        else if (SurveyData[i].Item.AnswerType == "OP") { // if answer type optional 
                            dsg += '<li class="mb-3"> ' +
                                '<h6 data-qtype=' + SurveyData[i].Item.AnswerType + ' data-mandatory="' + SurveyData[i].Item.IsMandatory + '" class="VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + ' mb-3 vis-tp-qus">' + (i + 1) + '. ' + SurveyData[i].Item.Question;
                            if (IsMandatoryAll || SurveyData[i].Item.IsMandatory == 'Y') {
                                dsg += '<sub style="color:red;font-size: 100%;bottom: unset;">*</sub>';
                            }
                            dsg += '</h6 > ' +
                                '<div class="vis-tp-listWrap"> ';
                            for (var j = 0; j < SurveyData[i].Values.length; j++) {
                                dsg += '<div  class=" VIS_SI_' + SurveyData[i].Values[j].AD_SurveyValue_ID + ' align-items-center d-flex mb-3 vis-tp-listItem"> ' +
                                    '<input type="radio" name=VIS_' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-id=VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID +
                                    ' data-surveyitem=' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-surveyvalue=' + SurveyData[i].Values[j].AD_SurveyValue_ID + ' data-survey=' + SurveyData[i].Item.AD_Survey_ID + ' class = "VIS_Answ_' + SurveyData[i].Values[j].AD_SurveyValue_ID + '" value="' + SurveyData[i].Values[j].Answer + '" > <p>' + SurveyData[i].Values[j].Answer + '</p>' +
                                    '</div>';
                            }
                            dsg += '</div>' +
                                '</li > ';
                        }
                        else if (SurveyData[i].Item.AnswerType == "TX") {// if answer type textbox
                            dsg += '<li class="mb-3"> ' +
                                '<h6 data-qtype=' + SurveyData[i].Item.AnswerType + ' data-mandatory="' + SurveyData[i].Item.IsMandatory + '" class="VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + ' mb-3 vis-tp-qus ml">' + (i + 1) + '. ' + SurveyData[i].Item.Question;
                            if (IsMandatoryAll || SurveyData[i].Item.IsMandatory == 'Y') {
                                dsg += '<sub style="color:red;font-size: 100%;bottom: unset;">*</sub>';
                            }
                            dsg += '</h6 > ' +
                                '<textarea class="VIS_Answ_' + SurveyData[i].Item.AD_SurveyItem_ID + '" data-id=VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-surveyitem=' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-surveyvalue="0" data-survey=' + SurveyData[i].Item.AD_Survey_ID + '  cols="30" rows="10" placeholder="Enter your text here"></textarea> ' +
                                '<small class="mb-3">Max 200 letters</small> ' +
                                '</li > ';
                        }
                    }
                }

                dsg += '</ol > ';
                // $dsgn = $(dsg);
            }
            dsg += '</div >' +
                '</div > ';

            $dsgn = $(dsg);
            responseSection.find('.response').append($dsgn.clone(true).removeAttr('style'));
            responseSection.find('input,textarea').attr('disabled', 'disabled');
            if (Limit > 0 && ResponseCount >= Limit) {

            } else {
                questionSection.append($dsgn);
                var main = questionSection.find('.VIS_SI_Main' + self.windowNo);
                var btns = '<div class="vis-survey">';
                var totalQues = main.find('[class^=VIS_Quest_]').length;

                //show pagging button according to page size and question length.
                if (pageSize > 0 && totalQues > pageSize) {
                    btns += '<div class="float-left"><a class="prev btn mr-2"><i class="fa fa-chevron-left" aria-hidden="true"></i> Prev</a></div>';
                }

                btns += '<div class="vis-tp-btnWrap float-right"> ' +
                    '<a class="next btn">Next <i class="fa fa-chevron-right" aria-hidden="true"></i></a>' +
                    '<a href="#" id="VIS_SI_BtnSubmit_' + self.windowNo + '" class="btn" >Submit</a> ' +
                    '</div >';

                btns += '<div>';
                questionSection.append(btns);

                //paging setup
                if (pageSize > 0 && totalQues > pageSize) {
                    questionSection.find('.vis-tp-orderListWrap li:gt(' + (pageSize - 1) + ')').addClass('hideQuestion');
                }
            }

           
            if (!isSelfShow) {
                responseSection.hide();
            } 

            loadAccessData(AD_Survey_ID);
           
        };

        function loadAccessData(AD_Survey_ID) {
            $.ajax({
                url: VIS.Application.contextUrl + "VIS/SurveyPanel/CheckResponseAccess",
                //async: false,
                data: {
                    AD_Survey_ID: AD_Survey_ID,
                    AD_SurveyAssignment_ID: AD_SurveyAssignment_ID,
                    AD_User_ID: VIS.context.getAD_User_ID(),
                    AD_Role_ID: VIS.context.getAD_Role_ID(),
                    Record_ID: self.record_ID,
                    AD_window_ID: _AD_Window_ID,
                    AD_Table_ID: self.curTab.getAD_Table_ID()
                },
                success: function (data) {
                    var res = [];
                    res = JSON.parse(data);
                    if (res != null && res.length>0) {
                        var count = 0;
                        //responseSection.show();
                        for (var i = 0; i < res.length; i++) {
                            if (res[i].Name == 'Self' && count == 0) {
                                count++;
                                AD_SurveyResponse_ID = res[i].AD_SurveyResponse_ID;
                                responseSection.find('select').append('<option selected value="' + res[i].User_ID + '" data-responseid="' + res[i].AD_SurveyResponse_ID + '">' + res[i].Name + '</option>');
                            } else {
                                responseSection.find('select').append('<option value="' + res[i].User_ID + '" data-responseid="' + res[i].AD_SurveyResponse_ID + '">' + res[i].Name + '</option>');
                            }
                        }
                    } else {
                        responseSection.find('select').append('<option selected value="' + VIS.context.getAD_User_ID() + '" data-responseid="' + AD_SurveyResponse_ID + '">Self</option>');
                    }

                    if (isSelfShow || count > 0) {
                        loadSurveyResponse(VIS.context.getAD_User_ID());
                    }
                },
                error: function (e) {
                }
            });

        }

        function loadSurveyResponse(userID) {
            $.ajax({
                url: VIS.Application.contextUrl + "VIS/SurveyPanel/GetResponseList",
                //async: false,
                data: {
                    AD_window_ID: _AD_Window_ID,
                    AD_Table_ID: self.curTab.getAD_Table_ID(),
                    Record_ID: self.record_ID,
                    AD_User_ID: userID,
                    AD_SurveyResponse_ID: AD_SurveyResponse_ID
                },
                success: function (data) {
                    var res = [];
                    res = JSON.parse(data);
                    if (res != null) {
                        for (var i = 0; i < res.length; i++) {
                            if (res[i].AnswerType == 'TX') {
                                responseSection.find('[data-surveyitem="' + res[i].AD_SurveyItem_ID + '"][data-surveyvalue="' + res[i].AD_SurveyValue_ID + '"]').val(res[i].Answer);
                            } else {
                                responseSection.find('[data-surveyitem="' + res[i].AD_SurveyItem_ID + '"][data-surveyvalue="' + res[i].AD_SurveyValue_ID + '"]').prop("checked", true);
                            }
                        }
                    };
                },
                error: function (e) {
                }
            });
        }

        //control setup
        function findControls() {           
            $mainDiv = questionSection.find('.VIS_SI_Main' + self.windowNo);
            $btnSubmit = questionSection.find('#VIS_SI_BtnSubmit_' + self.windowNo);
            eventHandling(); 
            showHideSubmit();
        };

        //Submit button show hide on the behalf of pagging
        function showHideSubmit() {
           
            if (questionSection.find('.vis-tp-orderListWrap li:last').is(':hidden')) {
                $btnSubmit.hide();
                questionSection.find('.next').show();
            } else {
                $btnSubmit.show();
                questionSection.find('.next').hide();
            }

            //if first item  is hidden
            if (questionSection.find('.vis-tp-orderListWrap li:first').is(':hidden')) {
                questionSection.find('.prev').show();
            } else {
                questionSection.find('.prev').hide();
            }
        }

        function eventHandling() {
            // Save response
            $btnSubmit.on("click", function (e) {
                var main = questionSection.find('.VIS_SI_Main' + self.windowNo);
                var asnwers = main.find('[class^=VIS_Answ_]'); //get all answer start VIS_Quest_
                var questions = main.find('[class^=VIS_Quest_]'); // get all question start VIS_Quest_
                var Final_Data = {};
                Final_Data.Questions = [];
                var AD_Survey_ID = 0;
                for (var i = 0; i < questions.length; i++) {
                    var required = true;
                    var lisItem = questionSection.find('[data-id="' + questions[i].classList[0] + '"]');
                    AD_Survey_ID= lisItem[0].dataset.survey;
                    if (questions[i].dataset.qtype == 'CB' || questions[i].dataset.qtype == 'OP' || questions[i].dataset.qtype == 'CL') {
                        if (lisItem.length > 0) {
                            for (var j = 0; j < lisItem.length; j++) {
                                if ($(lisItem[j]).is(":checked")) {
                                    required = false;
                                    Final_Data.Questions.push({
                                        "Question": questions[i].textContent,
                                        "Answer": $(lisItem[j]).val(),
                                        "AD_SurveyItem_ID": lisItem[j].dataset.surveyitem,
                                        "AD_SurveyValue_ID": lisItem[j].dataset.surveyvalue
                                    });
                                }
                            }
                            if ((IsMandatoryAll || questions[i].dataset.mandatory=='Y') && required) {                                
                                VIS.ADialog.error("FillMandatory", true, "Ques-" + (i + 1));
                                return;
                            }
                        }
                    }
                    else if (questions[i].dataset.qtype == 'TX') {
                        if ($(lisItem[0]).val().length > 0) {
                            required = false;
                        };
                        Final_Data.Questions.push({
                            "Question": questions[i].textContent,
                            "Answer": $(lisItem[0]).val(),                           
                            "AD_SurveyItem_ID": lisItem[0].dataset.surveyitem,
                            "AD_SurveyValue_ID": lisItem[0].dataset.surveyvalue                           
                        });

                        if ((IsMandatoryAll || questions[i].dataset.mandatory == 'Y') && required) {
                            VIS.ADialog.error("FillMandatory", true, "Ques-" + (i + 1));
                            return;
                        }
                    }
                }
                
                $.ajax({
                    type: "POST",
                    url: VIS.Application.contextUrl + "VIS/SurveyPanel/SaveSurveyResponse",
                    dataType: "json",
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({
                        "surveyResponseValue": Final_Data.Questions,
                        "AD_Window_ID": _AD_Window_ID,
                        "AD_Survey_ID": AD_Survey_ID,
                        "AD_Tab_ID": _AD_Tab_ID,
                        "Record_ID": self.record_ID,
                        "AD_Table_ID": self.curTab.getAD_Table_ID()
                    }),
                    success: function (data) {
                        toastr.success(VIS.Msg.getMsg("CheckListSaved"), '', { timeOut: 3000, "positionClass": "toast-top-right", "closeButton": true, });
                    },
                    error: function (e) {
                    }
                });
            });

            questionSection.find('input[type="checkbox"]').click(function () {
                if ($(this).attr('class')) {
                    singleChkBoxSelection(this, $(this).attr('class'));
                }

            });

            //Next Page
            questionSection.find('.next').click(function () {
                var last = questionSection.find('.vis-tp-orderListWrap').children('li:visible:last');
                last.nextAll(':lt(' + pageSize + ')').removeClass('hideQuestion');
                last.next().prevAll().addClass('hideQuestion');
                showHideSubmit();
            });
            // Previous Page
            questionSection.find('.prev').click(function () {
                var first = questionSection.find('.vis-tp-orderListWrap').children('li:visible:first');
                first.prevAll(':lt(' + pageSize + ')').removeClass('hideQuestion');
                first.prev().nextAll().addClass('hideQuestion');
                showHideSubmit();
            });

            responseSection.find('select').change(function () {
                responseSection.find('input').prop('checked', false);
                responseSection.find('textarea').val('');
                AD_SurveyResponse_ID = responseSection.find('select option:selected').data('responseid');
                loadSurveyResponse(responseSection.find('select option:selected').val());
            })

        };

        //Single selection of checkbox
        function singleChkBoxSelection(which, theClass) {
            var checkbox = questionSection[0].getElementsByClassName(theClass);
            for (var i = 0; i < checkbox.length; i++) {
                if (checkbox[i] == which) {

                } else {
                    checkbox[i].checked = false;
                }
            }
        }
    };

    /**
    *	Invoked when user click on panel icon
    */
    surveyPanel.prototype.startPanel = function (windowNo, curTab, extraInfo) {
        this.windowNo = windowNo;
        this.curTab = curTab;
        this.extraInfo = extraInfo;
        this.init();
    };

    /**
         *	This function will execute when user navigate  or refresh a record
         */
    surveyPanel.prototype.refreshPanelData = function (recordID, selectedRow) {
        
        this.record_ID = recordID;
        this.selectedRow = selectedRow;
        this.update();
    };

    /**
     *	Fired When Size of panel Changed
     */
    surveyPanel.prototype.sizeChanged = function (width) {
        this.panelWidth = width;
    };

    /**
     *	Dispose Component
     */
    surveyPanel.prototype.dispose = function () {
        this.disposeComponent();
    };

    /*
        * Fully qualified class name
        */
    VIS.SurveyPanel = surveyPanel;

})();
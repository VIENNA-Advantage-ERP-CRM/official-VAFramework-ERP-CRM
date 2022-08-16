VIS = window.VIS || {};
(function () {

    function surveyPanel() {
        this.record_ID = 0;
        this.windowNo = 0;
        this.curTab = null;
        this.selectedRow = null;
        this.panelWidth;
        this.extraInfo = null;
        var $root;
        var _AD_Window_ID = 0;
        var _AD_Tab_ID = 0;
        var iFrame;

        this.init = function () {
            $root = $('<div style="height:100%;width:100%;overflow:auto !important;"></div>');
            panelDetails(this.curTab.vo.AD_Window_ID, this.curTab.vo.AD_Tab_ID, $root);
        };

        var panelDetails = function (AD_window_ID, AD_Tab_ID, $root) {
            _AD_Window_ID = AD_window_ID;
            _AD_Tab_ID = AD_Tab_ID;
            $.ajax({
                url: VIS.Application.contextUrl + "VIS/SurveyPanel/GetSurveyAssignments",
                //async: false,
                data: { AD_window_ID: AD_window_ID, AD_Tab_ID: AD_Tab_ID },
                success: function (data) {
                    var res = [];
                    res = JSON.parse(data);
                    if (res != null) {
                        loadSurveyUI(res[0].Survey_ID, res[0].SurveyType, $root);
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

        function loadSurveyUI(AD_Survey_ID, SurveyType, $root) {
            $.ajax({
                url: VIS.Application.contextUrl + "VIS/SurveyPanel/GetSurveyItems",
                //async: false,
                data: { AD_Survey_ID: AD_Survey_ID },
                success: function (data) {
                    var res = [];
                    console.log("SurveyType-" + SurveyType);
                    res = JSON.parse(data);
                    if (res != null) {
                        loadSurveyDesign(res, SurveyType, $root);
                        findControls();
                    };
                },
                error: function (e) {
                }
            });
        };

        function loadSurveyDesign(SurveyData, SurveyType, $root) {
            var $dsgn;
            if (SurveyType == "CL") //if survey type is Checklist.
            {
                var dsg = '<div id=VIS_SI_Main' + this.windowNo + ' style="width:100%; height:100%;">' +
                    '<div class="vis-tp-mainContainer"> ' +
                    '<div class="vis-tp-listWrap"> ';
                if (SurveyData.length > 0) {
                    for (var i = 0; i < SurveyData.length; i++) {
                        dsg += '<div id= VIS_SI_' + SurveyData[i].Item.AD_SurveyItem_ID + ' class="align-items-center d-flex mb-3 pb-3 vis-tp-listItem">' +
                            '<input id=VIS_Answ_' + this.windowNo + ' data-id=VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + ' type= "checkbox" ><p id=VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + '>' + SurveyData[i].Item.Question + '</p>' +
                            '</div >';
                    }
                }
                dsg += '</div >' +
                    '<div class="vis-tp-btnWrap text-right mr-3"> ' +
                    '<a href="#" id=VIS_SI_BtnSubmit_' + this.windowNo + ' class="btn"">' + VIS.Msg.getMsg("Submit") + '</a> ' +
                    '</div >' +
                    '</div >' +
                    '</div > ';
                $dsgn = $(dsg);
            }
            else //(SurveyType == "QN") // if survey type is Questionnarie.
            {
                var dsg = '<div id=VIS_SI_Main' + this.windowNo + 'style="width:100%; height:100%;">' +
                    '<div class="vis-tp-mainContainer"> ' +
                    '<ol class="vis-tp-orderListWrap"> ';
                if (SurveyData.length > 0) {
                    for (var i = 0; i < SurveyData.length; i++) {
                        if (SurveyData[i].Item.AnswerType == "CB") {
                            dsg += '<li class="mb-3"> ' +
                                '<h6 id=VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-qtype=' + SurveyData[i].Item.AnswerType + ' class= "mb-3 vis-tp-qus"> ' + SurveyData[i].Item.Question + '</h6 > ' +
                                '<div class="vis-tp-listWrap"> ';
                            for (var j = 0; j < SurveyData[i].Values.length; j++) {
                                dsg += ' <div id=VIS_SI_' + SurveyData[i].Values[j].AD_SurveyValue_ID + ' class="align-items-center d-flex mb-3 vis-tp-listItem"> ' +
                                    ' <input id=VIS_Answ_' + SurveyData[i].Values[j].AD_SurveyValue_ID + ' data-id=VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID +
                                    ' data-surveyitem=' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-surveyvalue=' + SurveyData[i].Values[j].AD_SurveyValue_ID + ' data-survey=' + SurveyData[i].Item.AD_Survey_ID +  ' type="checkbox">' +
                                    ' <p>' + SurveyData[i].Values[j].Answer + '</p>' +
                                    ' </div>';
                            }
                            dsg += '</div> ' +
                                '</li > ';
                        }
                        else if (SurveyData[i].Item.AnswerType == "OP") {
                            dsg += '<li class="mb-3"> ' +
                                '<h6 id=VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-qtype=' + SurveyData[i].Item.AnswerType + ' class="mb-3 vis-tp-qus">' + SurveyData[i].Item.Question + '</h6> ' +
                                '<div class="vis-tp-listWrap"> ';
                            for (var j = 0; j < SurveyData[i].Values.length; j++) {
                                dsg += '<div id= VIS_SI_' + SurveyData[i].Values[j].AD_SurveyValue_ID + ' class="align-items-center d-flex mb-3 vis-tp-listItem"> ' +
                                    '<input type="radio" name=VIS_' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-id=VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID +
                                    ' data-surveyitem=' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-surveyvalue=' + SurveyData[i].Values[j].AD_SurveyValue_ID + ' data-survey=' + SurveyData[i].Item.AD_Survey_ID +  ' id = VIS_Answ_' + SurveyData[i].Values[j].AD_SurveyValue_ID + ' > <p>' + SurveyData[i].Values[j].Answer + '</p>' +
                                    '</div>';
                            }
                            dsg += '</div>' +
                                '</li > ';
                        }
                        else if (SurveyData[i].Item.AnswerType == "TX") {
                            dsg += '<li class="mb-3"> ' +
                                '<h6 id=VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-qtype=' + SurveyData[i].Item.AnswerType + ' class="mb-3 vis-tp-qus">' + SurveyData[i].Item.Question + '</h6> ' +
                                '<textarea id=VIS_Answ_' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-id=VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-surveyitem=' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-surveyvalue=0 data-survey=' + SurveyData[i].Item.AD_Survey_ID +  ' cols="30" rows="10" placeholder="Enter your text here"></textarea> ' +
                                '<small class="mb-3">Max 200 letters</small> ' +
                                '</li > ';
                        }
                    }
                }

                dsg += '</ol > ' +
                    '<div class="vis-tp-btnWrap text-right mr-3"> ' +
                    '<a href="#" id=VIS_SI_BtnSubmit_' + this.windowNo + ' class="btn" >Submit</a> ' +
                    '</div >' +
                    '</div >' +
                    '</div > ';
                $dsgn = $(dsg);
            }
            $root.append($dsgn);
        };

        function findControls() {
            $mainDiv = $root.find('#VIS_SI_Main' + this.windowNo);
            $btnSubmit = $root.find('#VIS_SI_BtnSubmit_' + this.windowNo);
            eventHandling();
        };
        function eventHandling() {
            $btnSubmit.click(function (e) {
                var asnwers = $root.find('[id^=VIS_Answ_]');
                var questions = $root.find('[id^=VIS_Quest_]');
                var Final_Data = {};
                Final_Data.Questions = [];
                for (var i = 0; i < questions.length; i++) {
                    var lisItem = $root.find('[data-id="' + questions[i].id + '"]');

                    if (questions[i].dataset.qtype == 'CB' || questions[i].dataset.qtype == 'OP') {
                        if (lisItem.length > 0) {
                            for (var j = 0; j < lisItem.length; j++) {
                                if ($(lisItem[j]).is(":checked"))
                                    Final_Data.Questions.push([{
                                        "Question": questions[i].textContent,
                                        "Asnwer": $(lisItem[j]).is(":checked"),
                                        "AD_Survey_ID": lisItem[j].dataset.survey,
                                        "AD_SurveyItem_ID": lisItem[j].dataset.surveyitem,
                                        "AD_SurveyValue_ID": lisItem[j].dataset.surveyvalue,
                                        "AD_Window_ID": _AD_Window_ID,
                                        "AD_Tab_ID": _AD_Tab_ID
                                    }]);
                            }
                        }
                    }
                    else if (questions[i].dataset.qtype == 'TX') {
                        Final_Data.Questions.push([{
                            "Question": questions[i].textContent,
                            "Asnwer": $(lisItem[0]).val(),
                            "AD_Survey_ID": lisItem[0].dataset.survey,
                            "AD_SurveyItem_ID": lisItem[0].dataset.surveyitem,
                            "AD_SurveyValue_ID": lisItem[0].dataset.surveyvalue,
                            "AD_Window_ID": _AD_Window_ID,
                            "AD_Tab_ID": _AD_Tab_ID
                        }]);
                    }
                }
                console.log(Final_Data);
            });
        };
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
    VIS.surveyPanel = surveyPanel;

})();
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
        var _AD_Window_ID = 0;
        var _AD_Tab_ID = 0;
        var iFrame;
        var IsMandatoryAll = false;      
        var pageSize = 0;
       
        this.init = function () {
            $root = $('<div style="height:65vh;width:100%;overflow:auto !important;"></div>');
           // panelDetails(this.curTab.vo.AD_Window_ID, this.curTab.vo.AD_Tab_ID, $root);
        };

        this.update = function (Record_ID) {
            $root.empty();
            IsMandatoryAll = false;
            pageSize = 0;
            panelDetails(this.curTab.vo.AD_Window_ID, this.curTab.vo.AD_Tab_ID, $root);
        }
            
        var panelDetails = function (AD_window_ID, AD_Tab_ID, $root) {
            _AD_Window_ID = AD_window_ID;
            _AD_Tab_ID = AD_Tab_ID;
            $.ajax({
                url: VIS.Application.contextUrl + "VIS/SurveyPanel/GetSurveyAssignments",
                //async: false,
                data: { AD_window_ID: AD_window_ID, AD_Tab_ID: AD_Tab_ID, AD_Table_ID: self.curTab.getAD_Table_ID(), AD_Record_ID: self.curTab.getRecord_ID()},
                success: function (data) {
                    var res = [];
                    res = JSON.parse(data);
                    if (res != null && res.length>0) {
                        IsMandatoryAll = res[0].IsMandatory;
                        pageSize = res[0].QuestionsPerPage;
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

        /**
         * Load Survey UI
         * @param {any} AD_Survey_ID
         * @param {any} SurveyType
         * @param {any} $root
         */
        function loadSurveyUI(AD_Survey_ID, SurveyType, $root) {
            $.ajax({
                url: VIS.Application.contextUrl + "VIS/SurveyPanel/GetSurveyItems",
                //async: false,
                data: { AD_Survey_ID: AD_Survey_ID },
                success: function (data) {
                    var res = [];
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

        /**
         * Create survey Question Answer
         * @param {any} SurveyData
         * @param {any} SurveyType
         * @param {any} $root
         */
        function loadSurveyDesign(SurveyData, SurveyType, $root) {
            var $dsgn;
            var dsg = '<div id="VIS_SI_Main' + self.windowNo + '" style="width:100%; height:calc(100% - 38px);">' +
                '<div class="vis-tp-mainContainer"> ' +
                '<ol class="list-unstyled vis-tp-orderListWrap"> ';
            if (SurveyType == "CL") //if survey type is Checklist.
            {
               
                if (SurveyData.length > 0) {
                    for (var i = 0; i < SurveyData.length; i++) {
                        dsg += '<li id="VIS_SI_' + SurveyData[i].Item.AD_SurveyItem_ID + '" class="align-items-center d-flex mb-3 pb-3 vis-tp-listItem">' +
                            '<h6 class="mr-2 mb-0">' + (i + 1) + '.</h6><input id="VIS_Answ_' + self.windowNo + '" data-id="VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + '" type= "checkbox" ><p id="VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + '" data-qtype="' + SurveyType + '" data-mandatory="' + SurveyData[i].Item.IsMandatory + '">' + SurveyData[i].Item.Question;

                        if (IsMandatoryAll || SurveyData[i].Item.IsMandatory == 'Y') {
                            dsg += '<sub style="color:red;font-size: 100%;bottom: unset;">*</sub>';
                        }
                        dsg += '</p>'+
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
                                '<h6 id="VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + '" data-qtype="' + SurveyData[i].Item.AnswerType + '" data-mandatory="' + SurveyData[i].Item.IsMandatory +'" class= "mb-3 vis-tp-qus ml"> ' +(i+1)+'. ' + SurveyData[i].Item.Question;
                            if (IsMandatoryAll || SurveyData[i].Item.IsMandatory == 'Y') {
                                dsg += '<sub style="color:red;font-size: 100%;bottom: unset;">*</sub>';
                            }
                            dsg += '</h6 > ' +
                                '<div class="vis-tp-listWrap"> ';
                            for (var j = 0; j < SurveyData[i].Values.length; j++) {
                                dsg += ' <div id="VIS_SI_' + SurveyData[i].Values[j].AD_SurveyValue_ID + '" class="align-items-center d-flex mb-3 vis-tp-listItem"> ' +
                                    ' <input id="VIS_Answ_' + SurveyData[i].Values[j].AD_SurveyValue_ID + '" data-id="VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID+'"';
                                if (SurveyData[i].Item.AnswerSelection == 'SL') {
                                    dsg += ' class="group_' + SurveyData[i].Item.AD_SurveyItem_ID + '" ';
                                }
                                    dsg +=  ' data-surveyitem=' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-surveyvalue=' + SurveyData[i].Values[j].AD_SurveyValue_ID + ' data-survey=' + SurveyData[i].Item.AD_Survey_ID + ' value="' + SurveyData[i].Values[j].Answer+'" type="checkbox">' +
                                    ' <p>' + SurveyData[i].Values[j].Answer + '</p>' +
                                    ' </div>';
                            }
                            dsg += '</div> ' +
                                '</li > '; 
                        }
                        else if (SurveyData[i].Item.AnswerType == "OP") { // if answer type optional 
                            dsg += '<li class="mb-3"> ' +
                                '<h6 id=VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-qtype=' + SurveyData[i].Item.AnswerType + ' data-mandatory="' + SurveyData[i].Item.IsMandatory + '" class="mb-3 vis-tp-qus">' +(i+1)+'. ' + SurveyData[i].Item.Question;
                            if (IsMandatoryAll || SurveyData[i].Item.IsMandatory=='Y') {
                                dsg += '<sub style="color:red;font-size: 100%;bottom: unset;">*</sub>';
                            }
                            dsg += '</h6 > ' +
                                '<div class="vis-tp-listWrap"> ';
                            for (var j = 0; j < SurveyData[i].Values.length; j++) {
                                dsg += '<div id= VIS_SI_' + SurveyData[i].Values[j].AD_SurveyValue_ID + ' class="align-items-center d-flex mb-3 vis-tp-listItem"> ' +
                                    '<input type="radio" name=VIS_' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-id=VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID +
                                    ' data-surveyitem=' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-surveyvalue=' + SurveyData[i].Values[j].AD_SurveyValue_ID + ' data-survey=' + SurveyData[i].Item.AD_Survey_ID + ' id = VIS_Answ_' + SurveyData[i].Values[j].AD_SurveyValue_ID + ' value="' + SurveyData[i].Values[j].Answer +'" > <p>' + SurveyData[i].Values[j].Answer + '</p>' +
                                    '</div>';
                            }
                            dsg += '</div>' +
                                '</li > ';
                        }
                        else if (SurveyData[i].Item.AnswerType == "TX") {// if answer type textbox
                            dsg += '<li class="mb-3"> ' +
                                '<h6 id="VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + '" data-qtype=' + SurveyData[i].Item.AnswerType + ' data-mandatory="' + SurveyData[i].Item.IsMandatory + '" class="mb-3 vis-tp-qus ml">' + (i + 1) + '. ' + SurveyData[i].Item.Question;
                            if (IsMandatoryAll || SurveyData[i].Item.IsMandatory == 'Y') {
                                dsg += '<sub style="color:red;font-size: 100%;bottom: unset;">*</sub>';
                            }
                            dsg += '</h6 > ' +
                                '<textarea id="VIS_Answ_' + SurveyData[i].Item.AD_SurveyItem_ID + '" data-id=VIS_Quest_' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-surveyitem=' + SurveyData[i].Item.AD_SurveyItem_ID + ' data-surveyvalue="0" data-survey=' + SurveyData[i].Item.AD_Survey_ID + '  cols="30" rows="10" placeholder="Enter your text here"></textarea> ' +
                                '<small class="mb-3">Max 200 letters</small> ' +
                                '</li > ';
                        }
                    }
                }

                dsg += '</ol > ' ;
               // $dsgn = $(dsg);
            }
            dsg+= '</div >' +
                '</div > ';

            $dsgn = $(dsg);
            $root.append($dsgn);
            var main = $root.find('#VIS_SI_Main' + self.windowNo);
            var btns = '<div class="vis-survey">';
            var totalQues = main.find('[id^=VIS_Quest_]').length;

            //show pagging button according to page size and question length.
            if (pageSize > 0 && totalQues > pageSize) {
                btns += '<div class="float-left"><a class="prev btn mr-2"><i class="fa fa-chevron-left" aria-hidden="true"></i> Prev</a></div>';
            }

            btns += '<div class="vis-tp-btnWrap float-right"> ' +
                '<a class="next btn">Next <i class="fa fa-chevron-right" aria-hidden="true"></i></a>'+
                '<a href="#" id="VIS_SI_BtnSubmit_' + self.windowNo + '" class="btn" >Submit</a> ' +
                '</div >';
           
            btns += '<div>';
            $root.append(btns);

            //paging setup
            if (pageSize > 0 && totalQues > pageSize) {
                $root.find('.vis-tp-orderListWrap li:gt(' + (pageSize - 1) + ')').addClass('hideQuestion');
            }
        };

        //control setup
        function findControls() {
            $mainDiv = $root.find('#VIS_SI_Main' + self.windowNo);
            $btnSubmit = $root.find('#VIS_SI_BtnSubmit_' + self.windowNo);
            eventHandling(); 
            showHideSubmit();
        };

        //Submit button show hide on the behalf of pagging
        function showHideSubmit() {
            if ($root.find('.vis-tp-orderListWrap li:last').is(':hidden')) {
                $btnSubmit.hide();
                $root.find('.next').show();
            } else {
                $btnSubmit.show();
                $root.find('.next').hide();
            }

            //if first item  is hidden
            if ($root.find('.vis-tp-orderListWrap li:first').is(':hidden')) {
                $root.find('.prev').show();
            } else {
                $root.find('.prev').hide();
            }
        }

        function eventHandling() {
            // Save response
            $btnSubmit.on("click", function (e) {
                var main = $root.find('#VIS_SI_Main' + self.windowNo);
                var asnwers = main.find('[id^=VIS_Answ_]'); //get all answer start VIS_Quest_
                var questions = main.find('[id^=VIS_Quest_]'); // get all question start VIS_Quest_
                var Final_Data = {};
                Final_Data.Questions = [];
                var AD_Survey_ID = 0;
                for (var i = 0; i < questions.length; i++) {
                    var required = true;
                    var lisItem = $root.find('[data-id="' + questions[i].id + '"]');
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

            $root.find('input[type="checkbox"]').click(function () {
                if ($(this).attr('class')) {
                    singleChkBoxSelection(this, $(this).attr('class'));
                }

            });

            //Next Page
            $root.find('.next').click(function () {
                var last = $root.find('.vis-tp-orderListWrap').children('li:visible:last');
                last.nextAll(':lt(' + pageSize + ')').removeClass('hideQuestion');
                last.next().prevAll().addClass('hideQuestion');
                showHideSubmit();
            });
            // Previous Page
            $root.find('.prev').click(function () {
                var first = $root.find('.vis-tp-orderListWrap').children('li:visible:first');
                first.prevAll(':lt(' + pageSize + ')').removeClass('hideQuestion');
                first.prev().nextAll().addClass('hideQuestion');
                showHideSubmit();
            });

           

        };

        //Single selection of checkbox
        function singleChkBoxSelection(which, theClass) {
            var checkbox = $root[0].getElementsByClassName(theClass);
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
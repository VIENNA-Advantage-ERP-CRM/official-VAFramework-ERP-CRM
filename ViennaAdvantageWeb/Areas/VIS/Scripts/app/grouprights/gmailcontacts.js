//(function (VIS, $) {


//    VIS.gmailcontacts = function (username, password, role, isUpdateExistingRecord, win) {

//        var self = this;
//        var windowNo = win;
//        var settingsDialog = null;
//        var gmailcontactdialog = null;
//        var grdCols = [];
//        var dData = [];
//        var originalData = [];
//        var $mainTable = null;
//        var $tr0 = null;
//        var $td01 = null;
//        var $lblUsername = null;
//        var $td02 = null;
//        var $txtUsername = null;
//        var $tr1 = null;
//        var $td11 = null;
//        var $lblPassword = null;
//        var $td12 = null;
//        var $txtPassword = null;
//        var $tr2 = null;
//        var $td21 = null;
//        var $lblRole = null;
//        var $td22 = null;
//        var $cmbRole = null;
//        var $tr3 = null;
//        var $td31 = null;
//        var $lblchkExisting;
//        var $chkbxUpdateExisting = null;
//        var $lblchkShowLinkedContacts = null;
//        var $chkbxShowLinkedContacts = null;
//        var chkShowLinkedContacts = false;
//        var $contactsetting = $('<div></div>');
//        var $dGrid = null;
//        var gContactHtml = "";
//        var bpAddress = [];
//        var bpGroupSource = [];
//        var bpgroup = "";
//        var org = "";
//        var $btnImport;
//        var $btnVerify;
//        var $btnRequery;
//        var uName = username;
//        var pWord = password;
//        var role = role;
//        var userName;
//        var $isBusy = null;
//        var $isBusyImg = null;
//        var password;
//        var role;
//        var bpgroup = "";
//        var chkUpdateExistingRecords = isUpdateExistingRecord;
//        var windowWidth = $(window).width();
//        var windowHeight = $(window).height();
//        var gmailcontacts = $("<div style='height:100%;width:100%;margin-left:5px;margin:right:5px;'></div>");
//        var gmailcontactsbtnrow = $("<div style='height:" + (parseInt(windowHeight) - 210) + "px;width:100%;margin-left:3px;'></div>");
//        var $root = $('<div style="background-color:white !important;"></div>');
//        var bsyDiv = $("<div class='vis-apanel-busy'>");
//        bsyDiv.css("width", "100%");
//        bsyDiv.css("height", "95%");
//        bsyDiv.css('text-align', 'center');
//        bsyDiv.css("position", "absolute");

//        //******************************
//        //On Resize of window
//        //******************************
//        $(window).on("resize", function () {
//            var h = $(window).height();
//            if ($root != null) {
//                // $root.css("height", h - 100);
//                $root.css("height", $(window).height());
//                //if ($(window).height() > $(window).width()) {

//                //    gmailcontactsbtnrow.css("height", (parseInt(windowHeight) - 225));
//                //}
//                //else {
//                //    gmailcontactsbtnrow.css("height", (parseInt(windowHeight) - 225));
//                //}
//                gmailcontactsbtnrow.css("height", (parseInt(windowHeight) - 210));
//            }
//        });

//        function initialize() {
//            $mainTable = $('<table></table>');
//            $tr0 = $('<tr></tr>');
//            $td01 = $('<td></td>');
//            $lblUsername = $("<label name='lblUsername'>" + VIS.Msg.getMsg("GmailUsername") + "</label>");
//            $td01.append($lblUsername);
//            $tr0.append($td01);
//            $td02 = $('<td></td>');
//            $txtUsername = $("<input type='text' name='txtUsername' id='txtUsername_" + windowNo + "' style='margin-bottom:10px'></input>");
//            $td02.append($txtUsername);
//            $tr0.append($td02);
//            $mainTable.append($tr0);
//            $tr1 = $('<tr></tr>');
//            $td11 = $('<td></td>');
//            $lblPassword = $("<label name='lblPassword'>" + VIS.Msg.getMsg("GmailPassword") + "</label>");
//            $td11.append($lblPassword);
//            $tr1.append($td11);
//            $td12 = $('<td></td>');
//            $txtPassword = $("<input type='password' name='txtPassword' id='txtPassword_" + windowNo + "' style='margin-bottom:10px'></input>");
//            $td12.append($txtPassword);
//            $tr1.append($td12);
//            $mainTable.append($tr1);
//            load();

//        }

//        //***************************
//        //Load Contacts from Gmail according to the credientials entered in Contact Setting Dialog or credientials saved in database
//        //***************************
//        this.loadSavedData = function () {
//            $isBusy = $("#busy_1");
//            $isBusyImg = $isBusy.children();
//            $isBusy.hide();
//            Addbuttons();

//            var url = VIS.Application.contextUrl + "WSP/ContactSettings/GetContacts?username=" + uName + "&password=" + pWord + "&role_ID=" + parseInt(role) + "&isUpdateExisting=" + chkUpdateExistingRecords;
//            $.ajax({
//                type: "POST",
//                async: false,
//                url: url,
//                dataType: "json",

//                success: function (dynData) {
//                    debugger;
//                    dData = JSON.parse(dynData);
//                    try {
//                        Object.keys(dData[0]);
//                    }
//                    catch (err) {
//                        alert(dData);
//                        self.dispose();
//                        VIS.contactSettings(this);
//                    }
//                    if (dData == null || dData == "" || dData.length == 0 || Object.keys(dData[0]).length <= 1) {
//                        if (dData.length > 0) {
//                            alert(dData.toString());
//                        }
//                        self.dispose();
//                        return;
//                    }
//                    openMyGmailContact(dynData);
//                },
//                error: function (e) {
//                    debugger;
//                    self.dispose();
//                    return;
//                }
//            });
//        }



//        //********************
//        //Add buttons on Customed Header 
//        //********************
//        function Addbuttons() {
//            var $toolbarDiv = $('<div class="vis-awindow-header vis-menuTitle">');
//            $root.height($(window).height() - 70);

//            var $btnclose = $('<a href="javascript:void(0)"  class="vis-mainMenuIcons vis-icon-menuclose"></a>');
//            var $title = $('<p>' + VIS.Msg.getMsg("MyGmailContacts") + ' </p>');
//            $btnclose.click(function () {

//                self.dispose();
//            });
//            $root.append($toolbarDiv.append($btnclose).append($title));

//            $btnRequery = $('<img  style="margin-top:10px;margin-right:20px;cursor: pointer;float:right" title="' + VIS.Msg.getMsg("Requery").replace('&', '') + '" src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/Refresh24.png"> </img>');
//            $toolbarDiv.append($btnRequery);

//        };




//        var openMyGmailContact = function (dynData) {
//            getorg();
//            getbpgroup();
//            $root.append(gmailcontactsbtnrow);
//            $root.css("height", $(window).height());
//            loadGridData(dynData);
//            gmailcontactevents();
//        };


//        //**************************
//        //Create grid and load data on page load
//        //param1=dyncData(Contacts detail we get from server side)
//        //**************************
//        var count = 0;
//        function loadGridData(dynData) {
//            var grdRows = [];
//            var columns = [];

//            dData = JSON.parse(dynData);

//            originalData = dData;
//            var cols = Object.keys(dData[0]);

//            for (var c = 0; c < cols.length; c++)   //Fill Columns
//            {
//                if (cols[c] == "IsSelected") {

//                    columns.push({
//                        field: cols[c], caption: '<div><input type="checkbox" id="chkSelectAll" style="width:13px;height:13px;" ></input></div>', size: '30px', resizable: true, style: 'text-align: left', render: function (record) {
//                            //var html = '<div><input  type="checkbox" id="chk_' + count + '" style="width:13px;height:13px;" ></input></div>';
//                            var html = '<div><input class="chkRow" type="checkbox" id="chk_' + count + '" style="width:13px;height:13px;" ></input></div>';
//                            count++;
//                            return html;
//                        }
//                    });
//                }
//                if (cols[c] == "ContactName") {
//                    columns.push({ field: cols[c], caption: VIS.Msg.getMsg("ContactName"), size: '100px', resizable: true, sortable: true, editable: { type: 'text' } });
//                }
//                else if (cols[c] == "BpartnerName") {
//                    columns.push({ field: cols[c], caption: VIS.Msg.getMsg("WSP_BpartnerName"), size: '100px', resizable: true, sortable: true, editable: { type: 'text' } });
//                }
//                else if (cols[c] == "IsCustomer") {

//                    columns.push({
//                        field: cols[c], caption: VIS.Msg.getMsg("IsCustomer"), size: '100px', resizable: true, sortable: true,
//                        editable: { type: 'checkbox' }, style: 'height:25px !Important;width:25px !Important;'
//                    });

//                }
//                else if (cols[c] == "IsVendor") {
//                    columns.push({
//                        field: cols[c], caption: VIS.Msg.getMsg("IsVendor"), size: '100px', resizable: true, sortable: true,
//                        editable: { type: 'checkbox' }
//                    });

//                }
//                else if (cols[c] == "IsEmployee") {
//                    columns.push({
//                        field: cols[c], caption: VIS.Msg.getMsg("IsEmployee"), size: '100px', resizable: true, sortable: true, editable: { type: 'checkbox' }
//                    });

//                }
//                else if (cols[c] == "BpGroup") {

//                    columns.push({
//                        field: cols[c], caption: VIS.Msg.getMsg("BPGroup"), size: '100px', resizable: true, sortable: true,
//                        editable: { type: 'select', items: bpGroupSource },
//                        render: function (record, index, col_index) {
//                            var html = '';
//                            for (var p in bpGroupSource) {
//                                if (bpGroupSource[p].id == this.getCellValue(index, col_index)) html = bpGroupSource[p].text;
//                            }
//                            return html;
//                        }
//                    });
//                }
//                else if (cols[c] == "Title") {
//                    columns.push({
//                        field: cols[c], caption: VIS.Msg.getMsg("Title"), size: '100px', resizable: true, sortable: true, editable: { type: 'text' }
//                    });


//                }
//                else if (cols[c] == "EmailAddress") {
//                    columns.push({
//                        field: cols[c], caption: VIS.Msg.getMsg("EmailAddress"), size: '100px', resizable: true, sortable: true, editable: { type: 'text' }
//                    });
//                }

//                else if (cols[c] == "Mobile") {
//                    columns.push({
//                        field: cols[c], caption: VIS.Msg.getMsg("Mobile"), size: '100px', resizable: true, sortable: true, editable: { type: 'text' }
//                    });

//                }
//                else if (cols[c] == "PhoneNumber") {
//                    columns.push({
//                        field: cols[c], caption: VIS.Msg.getMsg("PhoneNumber"), size: '100px', resizable: true, sortable: true, editable: { type: 'text' }
//                    });

//                }
//                else if (cols[c] == "PhoneNumber2") {
//                    columns.push({
//                        field: cols[c], caption: VIS.Msg.getMsg("PhoneNumber2"), size: '100px', resizable: true, sortable: true, editable: { type: 'text' }
//                    });

//                }
//                else if (cols[c] == "BPAddress") {
//                    columns.push({
//                        field: cols[c], caption: VIS.Msg.getMsg("WSP_BpAddress"), size: '100px', resizable: true, sortable: true,
//                        render: function (record) {
//                            var resString = '<div style="width:95px;"><select style="width:90px;">';
//                            if (record.BPAddress == null || record.BPAddress.length == 0) {
//                                resString += '</select></div>';
//                                return resString;
//                            }
//                            for (var itm in record.BPAddress) {
//                                resString += '<option>' + record.BPAddress[itm] + '</option>';
//                            }
//                            resString += '</select></div>';
//                            return resString;
//                        }
//                    });
//                }
//                else if (cols[c] == "BirthDay") {
//                    columns.push({
//                        field: cols[c], caption: VIS.Msg.getMsg("BirthDay"), size: '150px', sortable: true, resizable: true, render: 'date', style: 'text-align: right',
//                        editable: { type: 'date' }
//                    });
//                }
//            }





//            //var bpAddress=[];
//            if (dData != null && dData != "" && dData.length > 0)   //Fill Rows
//            {
//                var recID = 0;
//                for (var j = 0; j < dData.length; j++) {


//                    var row = {};
//                    if (chkShowLinkedContacts == false) {
//                        if (dData[j].IsLinked == true) {
//                            continue;
//                        }
//                    }
//                    row['recid'] = dData[j].ID;
//                    for (var c = 0; c < cols.length; c++) {

//                        if (colna == "ContactName" || colna == "BpartnerName" || colna == "Title" || colna == "EmailAddress" || colna == "Mobile" || colna == "PhoneNumber" || colna == "PhoneNumber2") {
//                            dData[j][colna] = VIS.Utility.encodeText(dData[j][colna]);
//                        }
//                        var colna = cols[c];

//                        if (colna == "BpGroup") {
//                            row[colna] = dData[j]["SelectedVal"];
//                            continue;
//                        }
//                        else if (colna == "BPAddress") {
//                            row[colna] = dData[j]["BPAddress"];
//                        }
//                        else if (colna == "BirthDay") {
//                            //if (dData[j]["BirthDay"] != null)
//                            //{
//                            //    var bDate = new Date(dData[j]["BirthDay"]);
//                            //    row[colna] = (bDate.getMonth()+1) + "/" + bDate.getDate() + "/" + bDate.getFullYear();
//                            //}
//                        }
//                        else {
//                            row[colna] = dData[j][colna];
//                        }
//                    }
//                    if (dData[j].IsLinked == true) {
//                        row['style'] = 'background-color:LightGray';
//                        if (!chkUpdateExistingRecords) {
//                            row['editable'] = false;
//                        }
//                    }
//                    else if (dData[j].IsColor == true) {
//                        row['style'] = 'background-color:#D99694';

//                    }
//                    grdRows[recID] = row;
//                    recID += 1;

//                }








//                $dGrid = gmailcontacts.w2grid({
//                    name: 'gridInfodata_' + windowNo,
//                    recordHeight: 40,
//                    autoLoad: false,
//                    show: {

//                        //toolbar: true,  // indicates if toolbar is v isible
//                        //columnHeaders: true,   // indicates if columns is visible
//                        lineNumbers: true,  // indicates if line numbers column is visible
//                        //selectType: 'row',
//                        // selectColumn: true,  // indicates if select column is visible

//                        //toolbarReload: false,   // indicates if toolbar reload button is visible
//                        //toolbarColumns: true,   // indicates if toolbar columns button is visible
//                        //toolbarSearch: false,   // indicates if toolbar search controls are visible
//                        //toolbarAdd: false,   // indicates if toolbar add new button is visible
//                        //toolbarDelete: false,   // indicates if toolbar delete button is visible
//                        //toolbarSave: false,   // indicates if toolbar save button is visible
//                        //selectionBorder: false,	 // display border arround selection (for selectType = 'cell')
//                        //recordTitles: false	 // indicates if to define titles for records
//                    },
//                    columns: columns,
//                    records: grdRows
//                })


//            }
//        };


//        var chkSelect = [];
//        //**************************
//        //Events on controls of Gmail Contact Form
//        //**************************
//        function gmailcontactevents() {

//            $btnImport.on("click", function () {
//                var msg = "";
//                bsyDiv[0].style.visibility = 'visible';
//                //$isBusy.show();
//                //var h = $(window).height() * (40 / 100);
//                //var w = $(window).width() * (50 / 100);
//                //$isBusyImg.css("margin-top", h);
//                //$isBusyImg.css("margin-left", w);

//                window.setTimeout(function () {
//                    var getSelectedRows = [];
//                    //getSelectedRows = $dGrid.getSelection();
//                    getSelectedRows = chkSelect;
//                    var cols = [];
//                    if (getSelectedRows.length > 0) {


//                        var changes = [];
//                        var selectedChanges = [];
//                        var selectedChangeItem = {};
//                        var index = 0;
//                        changes = w2ui['gridInfodata_' + windowNo].getChanges();
//                        for (var k = 0; k < getSelectedRows.length; k++) {

//                            selectedChangeItem = $.grep(changes, function (p) { return p.recid == getSelectedRows[k]; })
//                            .map(function (p) { return p; });
//                            if (selectedChangeItem.length > 0) {
//                                selectedChanges[index] = selectedChangeItem;
//                                index++;
//                            }


//                        }
//                        for (var i = 0; i < selectedChanges.length; i++) {
//                            cols = Object.keys(selectedChanges[i][0]);


//                            for (var j = 0; j < cols.length; j++) {
//                                var colna = cols[j];
//                                if (colna == "recid")
//                                    continue;
//                                updateRecord(selectedChanges[i][0].recid, colna, selectedChanges[i][0][colna]);
//                            }


//                        }
//                        for (var k = 0; k < getSelectedRows.length; k++) {
//                            var recid = $.grep(dData, function (p) { return p.ID == getSelectedRows[k]; })
//                                            .map(function (p) { return p.Index; });
//                            if (dData[recid].BpartnerName == "" || dData[recid].BpartnerName == null) {
//                                bsyDiv[0].style.visibility = 'hidden';
//                                alert(VIS.Msg.getMsg("BPNotNUll") + (parseInt($dGrid.get(getSelectedRows[k], true)) + 1));
//                                //$isBusy.hide();
//                                return;
//                            }
//                            else if (dData[recid].ContactName == "" || dData[recid].ContactName == null) {
//                                bsyDiv[0].style.visibility = 'hidden';
//                                alert(VIS.Msg.getMsg("ContactnotNUll") + (parseInt($dGrid.get(getSelectedRows[k], true)) + 1));
//                                //$isBusy.hide();
//                                return;
//                            }
//                            else if (dData[recid].IsCustomer == false && dData[recid].IsVendor == false && dData[recid].IsEmployee == false) {
//                                bsyDiv[0].style.visibility = 'hidden';
//                                msg = VIS.Msg.getMsg("BPTypeNotNull") + (parseInt($dGrid.get(getSelectedRows[k], true)) + 1);
//                                // $isBusy.hide();

//                            }
//                        }
//                    }
//                    else {
//                        bsyDiv[0].style.visibility = 'hidden';
//                        alert(VIS.Msg.getMsg("NoRecordSelected"));

//                        // $isBusy.hide();
//                        return;
//                    }
//                    // bsyDiv[0].style.visibility = 'visible';
//                    var fields = JSON.stringify(dData);
//                    var url = VIS.Application.contextUrl + "WSP/ContactSettings/Import";
//                    $.ajax({
//                        type: "POST",
//                        async: false,
//                        url: url,
//                        dataType: "json",
//                        data: {
//                            fields: fields,
//                            orgID: JSON.stringify($("#ddlOrg_" + windowNo).val())
//                        },
//                        success: function (dynData) {
//                            //  bsyDiv[0].style.visibility = 'hidden';
//                            //  $isBusy.hide();
//                            if (!JSON.parse(dynData).contains("inserted")) {
//                                bsyDiv[0].style.visibility = 'hidden';
//                                if (msg.length > 0) {
//                                    alert(msg);
//                                }
//                                else {
//                                    alert(JSON.parse(dynData));
//                                }
//                                return;
//                            }
//                            requery();
//                            alert(JSON.parse(dynData));



//                        }
//                    });
//                }, 500);



//            });
//            $btnVerify.on("click", function () {

//                var msg = "";
//                bsyDiv[0].style.visibility = 'visible';
//                //$isBusy.show();
//                //var h = $(window).height() * (40 / 100);
//                //var w = $(window).width() * (50 / 100);
//                //$isBusyImg.css("margin-top", h);
//                //$isBusyImg.css("margin-left", w);

//                window.setTimeout(function () {
//                    debugger;
//                    var getSelectedRows = [];
//                    //getSelectedRows = $dGrid.getSelection();
//                    getSelectedRows = chkSelect;
//                    var cols = [];

//                    if (getSelectedRows.length > 0) {


//                        var changes = [];
//                        var selectedChanges = [];
//                        var selectedChangeItem = {};
//                        var index = 0;
//                        changes = w2ui['gridInfodata_' + windowNo].getChanges();
//                        for (var k = 0; k < getSelectedRows.length; k++) {

//                            selectedChangeItem = $.grep(changes, function (p) { return p.recid == getSelectedRows[k]; })
//                            .map(function (p) { return p; });
//                            if (selectedChangeItem.length > 0) {
//                                selectedChanges[index] = selectedChangeItem;
//                                index++;
//                            }


//                        }
//                        for (var i = 0; i < selectedChanges.length; i++) {
//                            cols = Object.keys(selectedChanges[i][0]);


//                            for (var j = 0; j < cols.length; j++) {
//                                var colna = cols[j];
//                                if (colna == "recid")
//                                    continue;
//                                updateRecord(selectedChanges[i][0].recid, colna, selectedChanges[i][0][colna]);
//                            }


//                        }
//                        for (var k = 0; k < getSelectedRows.length; k++) {
//                            var recid = $.grep(dData, function (p) { return p.ID == getSelectedRows[k]; })
//                                            .map(function (p) { return p.Index; });
//                            if (dData[recid].BpartnerName == "" || dData[recid].BpartnerName == null) {
//                                bsyDiv[0].style.visibility = 'hidden';
//                                alert(VIS.Msg.getMsg("BPNotNUll") + (parseInt($dGrid.get(getSelectedRows[k], true)) + 1));
//                                //  $isBusy.hide();

//                                return;
//                            }
//                            else if (dData[recid].ContactName == "" || dData[recid].ContactName == null) {
//                                bsyDiv[0].style.visibility = 'hidden';
//                                alert(VIS.Msg.getMsg("ContactnotNUll") + (parseInt($dGrid.get(getSelectedRows[k], true)) + 1));
//                                //  $isBusy.hide();
//                                //bsyDiv[0].style.visibility = 'hidden';
//                                return;
//                            }
//                            else if (dData[recid].IsCustomer == false && dData[recid].IsVendor == false && dData[recid].IsEmployee == false) {
//                                bsyDiv[0].style.visibility = 'hidden';
//                                msg = VIS.Msg.getMsg("BPTypeNotNull") + (parseInt($dGrid.get(getSelectedRows[k], true)) + 1);
//                            }
//                        }
//                    }
//                    else {
//                        bsyDiv[0].style.visibility = 'hidden';
//                        alert(VIS.Msg.getMsg("NoRecordSelected"));
//                        // $isBusy.hide();

//                        return;
//                    }




//                    var fields = JSON.stringify(dData);
//                    var value = w2ui['gridInfodata_' + windowNo].get(w2ui['gridInfodata_' + windowNo].getSelection());
//                    var url = VIS.Application.contextUrl + "WSP/ContactSettings/VerifyData";
//                    $.ajax({
//                        type: "POST",
//                        async: false,
//                        url: url,
//                        dataType: "json",
//                        data: {
//                            fields: fields
//                        },
//                        success: function (dynData) {
//                            if (JSON.parse(dynData).contains("type")) {
//                                bsyDiv[0].style.visibility = 'hidden';
//                                if (msg.length > 0) {
//                                    alert(msg);
//                                }
//                                else {
//                                    alert(JSON.parse(dynData));
//                                }
//                                return;
//                            }
//                            bsyDiv[0].style.visibility = 'hidden';
//                            alert(JSON.parse(dynData));
//                            //disposeComponent();


//                        }
//                    });
//                }, 5000);
//            });
//            $btnRequery.on("click", function () {
//                requery();
//            });


//            function requery() {

//                bsyDiv[0].style.visibility = 'visible';
//                //$isBusy.show();
//                //var h = $(window).height() * (40 / 100);
//                //var w = $(window).width() * (50 / 100);
//                //$isBusyImg.css("margin-top", h);
//                //$isBusyImg.css("margin-left", w);

//                window.setTimeout(function () {
//                    updateIsSelected(0, false, true);
//                    var url = VIS.Application.contextUrl + "WSP/ContactSettings/GetContacts?username=" + uName + "&password=" + pWord + "&role_ID=" + parseInt(role) + "&isUpdateExisting=" + chkUpdateExistingRecords;
//                    $.ajax({
//                        type: "POST",
//                        async: false,
//                        url: url,
//                        dataType: "json",

//                        success: function (dynData) {
//                            try {
//                                $dGrid.lock();
//                                $dGrid.clear();
//                                fillGrid(JSON.parse(dynData));
//                                dData = JSON.parse(dynData);
//                                originalData = dData;
//                                $dGrid.unlock();
//                                // $isBusy.hide();
//                                bsyDiv[0].style.visibility = 'hidden';
//                            }
//                            catch (err) {
//                                // $isBusy.hide();
//                                bsyDiv[0].style.visibility = 'hidden';
//                            }
//                            //disposeComponent();


//                        }
//                    });
//                }, 500);
//            };
//            $chkbxShowLinkedContacts.on("click", function (event) {
//                bsyDiv[0].style.visibility = 'visible';
//                //$isBusy.show();
//                //var h = $(window).height() * (40 / 100);
//                //var w = $(window).width() * (50 / 100);
//                //$isBusyImg.css("margin-top", h);
//                //$isBusyImg.css("margin-left", w);

//                window.setTimeout(function () {
//                    try {
//                        chkShowLinkedContacts = event.originalEvent.target.checked;

//                        // $dGrid.destroy();               
//                        //$dGrid.remove();
//                        $dGrid.clear();
//                        fillGrid(dData);
//                        // $isBusy.hide();
//                        bsyDiv[0].style.visibility = 'hidden';
//                    }
//                    catch (err) {
//                        //  $isBusy.hide();
//                        bsyDiv[0].style.visibility = 'hidden';
//                    }
//                }, 500);
//                //}
//            });

//            var chkrecord = {};

//            $(".chkRow").on("change", function (event) {
//                debugger;


//                $("#chkSelectAll").prop("checked", false);
//                var index = chkSelect.length;
//                // alert(event.originalEvent.target.checked);
//                if (event.originalEvent.target.checked) {
//                    //chkrecord = {};
//                    //chkrecord['id'] =index;
//                    //chkrecord['text'] = event.recid;
//                    //chkSelect[index] = chkrecord;
//                    chkSelect[index] = $($($($(this).parent()).parent()).parent()).attr("recid");
//                }
//                else {
//                    //  var i = $.grep(chkSelect, function (p) { return p.text == event.recid; })
//                    //.map(function (p) { return p.id; });
//                    //  chkSelect.pop(i);

//                    for (var i = 0; i < chkSelect.length; i++) {
//                        if (chkSelect[i] == $($($($(this).parent()).parent()).parent()).attr("recid")) {
//                            chkSelect.splice(i, 1);
//                            i--;
//                        }
//                    }
//                    chkSelect = jQuery.grep(chkSelect, function (value) {
//                        return value != $($($($(this).parent()).parent()).parent()).attr("recid");
//                    });
//                }
//                updateData($($($($(this).parent()).parent()).parent()).attr("recid"), 0, event.originalEvent.target.checked);
//                // alert(chkSelect);
//            });

//            $dGrid.on('change', function (event) {


//                //alert("chnge");

//                var index = chkSelect.length;
//                if (event.column == 0) {
//                    if (event.value_new) {
//                        //chkrecord = {};
//                        //chkrecord['id'] =index;
//                        //chkrecord['text'] = event.recid;
//                        //chkSelect[index] = chkrecord;
//                        chkSelect[index] = event.recid;
//                    }
//                    else {
//                        chkSelect = jQuery.grep(chkSelect, function (value) {
//                            return value != event.recid;
//                        });
//                        //  var i = $.grep(chkSelect, function (p) { return p.text == event.recid; })
//                        //.map(function (p) { return p.id; });
//                    }
//                    updateData(event.recid, event.column, event.value_new);
//                }
//            });

//            //$("#chkSelectAll").on("click", function (event) {
//            //    
//            //    if (!event.originalEvent.target.checked) {
//            //        chkSelect = [];
//            //    }
//            //    for (var i = 0; i < $dGrid.records.length; i++) {

//            //        $("#chk_" + i).prop("checked", event.originalEvent.target.checked);
//            //        if (event.originalEvent.target.checked) {
//            //            chkSelect[i] = $dGrid.records[i].recid;
//            //        }
//            //    }

//            //});
//            $dGrid.on('columnClick', function (event) {
//                debugger;
//                // alert("columnClick");
//                if (event.field == "IsSelected") {
//                    if (!event.originalEvent.target.checked) {
//                        chkSelect = [];
//                    }
//                    for (var i = 0; i < $dGrid.records.length; i++) {

//                        $("#chk_" + i).prop("checked", event.originalEvent.target.checked);
//                        if (event.originalEvent.target.checked) {
//                            chkSelect[i] = $dGrid.records[i].recid;
//                        }
//                        updateData($dGrid.records[i].recid, 0, event.originalEvent.target.checked);
//                    }
//                }
//                // fillGrid(dData);
//            });
//            //$dGrid.on('click', function (event) {    //21-11-2014
//            //    debugger;
//            //    //alert(event.column);
//            //    if (event.column == 0) {
//            //        $("#chkSelectAll").prop("checked", false);
//            //        var index = chkSelect.length;
//            //       // alert(event.originalEvent.target.checked);
//            //        if (event.originalEvent.target.checked)
//            //            {
//            //                //chkrecord = {};
//            //                //chkrecord['id'] =index;
//            //                //chkrecord['text'] = event.recid;
//            //            //chkSelect[index] = chkrecord;
//            //            chkSelect[index] = event.recid;
//            //            }
//            //            else
//            //            {
//            //              //  var i = $.grep(chkSelect, function (p) { return p.text == event.recid; })
//            //              //.map(function (p) { return p.id; });
//            //            //  chkSelect.pop(i);
//            //            chkSelect = jQuery.grep(chkSelect, function (value) {
//            //                return value != event.recid;
//            //            });
//            //            }
//            //        updateData(event.recid, event.column, event.originalEvent.target.checked);
//            //    }

//            //});
//            //$dGrid.on('select', function (event) {
//            //    
//            //    if (event.all == true) {
//            //        updateIsSelected(0, true, true);
//            //    }
//            //    else {
//            //        updateIsSelected(event.recid, true, false);
//            //    }
//            //});
//            //$dGrid.on('unselect', function (event) {
//            //    
//            //    if (event.all == true) {
//            //        updateIsSelected(0, false, true);
//            //    }
//            //    else {
//            //        updateIsSelected(event.recid, false, false);
//            //    }
//            //});
//        };


//        //***************************
//        //Update value of IsSelected field
//        //param1=RecordID
//        //param2=value(value of field)
//        //param3=isAll(isAll true for all records and false for partcular record)
//        //***************************
//        function updateIsSelected(recid, value, isall) {

//            recid = $.grep(dData, function (p) { return p.ID == recid; })
//           .map(function (p) { return p.Index; });

//            if (isall) {
//                for (var i = 0; i < dData.length; i++) {
//                    dData[i]["IsSelected"] = value;
//                }
//            }
//            else {
//                dData[recid]["IsSelected"] = value;
//            }
//        }

//        //***************************
//        //Update data in ContactList
//        //param1=RecordID
//        //param2=Column(Field whose value you want to update)
//        //param3=value(value of field)
//        //***************************
//        function updateRecord(recid, column, value) {

//            recid = $.grep(dData, function (p) { return p.ID == recid; })
//            .map(function (p) { return p.Index; });
//            if (column == "BpGroup") {
//                dData[recid]["SelectedVal"] = value;
//            }
//            dData[recid]["IsSelected"] = true;
//            dData[recid][column] = value;
//        }


//        //***************************
//        //Update data in ContactList
//        //param1=RecordID
//        //param2=Column(Field whose value you want to update)
//        //param3=value(value of field)
//        //***************************
//        function updateData(recid, column, value) {

//            recid = $.grep(dData, function (p) { return p.ID == recid; })
//            .map(function (p) { return p.Index; });
//            if (column == 0) {
//                dData[recid].IsSelected = value;
//            }
//            else if (column == 1) {
//                dData[recid].ContactName = value;
//            }
//            else if (column == 2) {
//                dData[recid].BpartnerName = value;
//            }
//            else if (column == 3) {
//                dData[recid].IsCustomer = value;
//            }
//            else if (column == 4) {
//                dData[recid].IsVendor = value;
//            }
//            else if (column == 5) {
//                dData[recid].IsEmployee = value;
//            }
//            else if (column == 6) {
//                dData[recid].bpgroup = value;
//            }
//            else if (column == 7) {
//                dData[recid].Title = value;
//            }
//            else if (column == 8) {
//                dData[recid].EmailAddress = value;
//            }
//            else if (column == 9) {
//                dData[recid].Mobile = value;
//            }
//            else if (column == 10) {
//                dData[recid].PhoneNumber = value;
//            }
//            else if (column == 11) {
//                dData[recid].PhoneNumber2 = value;
//            }
//            else if (column == 12) {
//                dData[recid].BPAddress = value;
//            }
//            else if (column == 13) {
//                dData[recid].BirthDay = value;
//            }
//        };


//        //****************************
//        //Reload Grid in case of click on Refresh button or on click of Show Linked Contacts checkbox
//        //param1=dData(contacts and there detail get from database and gmail)
//        //****************************

//        function fillGrid(dData) {
//            //updateIsSelected(0, false, true);
//            count = 0;
//            chkSelect = [];

//            var cols = Object.keys(dData[0]);
//            var grdRows = [];
//            if (dData != null && dData != "" && dData.length > 0)   //Fill Rows
//            {
//                // var recID = 0;
//                for (var j = 0; j < dData.length; j++) {
//                    var row = {};
//                    if (chkShowLinkedContacts == false) {
//                        if (dData[j].IsLinked == true) {
//                            continue;
//                        }
//                    }
//                    row['recid'] = dData[j].ID;
//                    for (var c = 0; c < cols.length; c++) {
//                        if (colna == "ContactName" || colna == "BpartnerName" || colna == "Title" || colna == "EmailAddress" || colna == "Mobile" || colna == "PhoneNumber" || colna == "PhoneNumber2") {
//                            dData[j][colna] = VIS.Utility.encodeText(dData[j][colna]);
//                        }
//                        var colna = cols[c];
//                        if (colna == "BpGroup") {
//                            row[colna] = dData[j]["SelectedVal"];
//                            continue;
//                        }
//                        else if (colna == "BPAddress") {

//                            //var lookup = VIS.MLookupFactory.get(VIS.context.ctx, 0, 0, VIS.DisplayType.TableDir, "C_Location_ID", 0, false, null);
//                            //row[colna] = new VIS.Controls.VComboBox("C_Location_ID", false, false, true, lookup, 50)

//                            row[colna] = dData[j]["BPAddress"];
//                        }
//                            //else if (colna == "BirthDay") {
//                            //    if (dData[j]["BirthDay"] != null) {
//                            //        var bDate = new Date(dData[j]["BirthDay"]);
//                            //        row[colna] = (bDate.getMonth()+1) + "/" + bDate.getDate() + "/" + bDate.getFullYear();
//                            //    }
//                            //}
//                        else if (colna == "IsSelected") {
//                            row[colna] = false;
//                            dData[j][colna] = false;
//                        }
//                        else {
//                            row[colna] = dData[j][colna];
//                        }
//                    }
//                    if (dData[j].IsLinked == true) {
//                        row['style'] = 'background-color:LightGray';
//                        if (!chkUpdateExistingRecords) {
//                            row['editable'] = false;
//                        }
//                    }
//                    else if (dData[j].IsColor == true) {
//                        row['style'] = 'background-color:#D99694';
//                    }
//                    grdRows[dData[j].ID] = row;
//                    // recID += 1;

//                }
//            }
//            $dGrid.add(grdRows);


//            $(".chkRow").on("change", function (event) {
//                debugger;


//                $("#chkSelectAll").prop("checked", false);
//                var index = chkSelect.length;
//                // alert(event.originalEvent.target.checked);
//                if (event.originalEvent.target.checked) {
//                    //chkrecord = {};
//                    //chkrecord['id'] =index;
//                    //chkrecord['text'] = event.recid;
//                    //chkSelect[index] = chkrecord;
//                    chkSelect[index] = $($($($(this).parent()).parent()).parent()).attr("recid");
//                }
//                else {
//                    //  var i = $.grep(chkSelect, function (p) { return p.text == event.recid; })
//                    //.map(function (p) { return p.id; });
//                    //  chkSelect.pop(i);

//                    for (var i = 0; i < chkSelect.length; i++) {
//                        if (chkSelect[i] == $($($($(this).parent()).parent()).parent()).attr("recid")) {
//                            chkSelect.splice(i, 1);
//                            i--;
//                        }
//                    }
//                    chkSelect = jQuery.grep(chkSelect, function (value) {
//                        return value != $($($($(this).parent()).parent()).parent()).attr("recid");
//                    });
//                }
//                updateData($($($($(this).parent()).parent()).parent()).attr("recid"), 0, event.originalEvent.target.checked);
//                // alert(chkSelect);
//            });
//        };

//        //*****************************
//        //Get Organizations 
//        //*****************************
//        function getorg() {
//            var url = VIS.Application.contextUrl + "WSP/ContactSettings/GetOrg";
//            $.ajax({
//                type: "GET",
//                async: false,
//                url: url,
//                dataType: "json",

//                success: function (data) {

//                    var dd = JSON.parse(data);
//                    for (var i = 0; i < dd.length; i++) {
//                        org += "<option value='" + dd[i]["ID"] + "'>" + dd[i]["Name"] + "</option>";
//                    }
//                    appendGmailContactSettingForm();


//                }
//            });
//        };


//        //*****************************
//        //Append HTML of form in this function
//        //****************************

//        var appendGmailContactSettingForm = function () {

//            var divOrg = $('<div style="float:left;display:inline;margin-top:25px;"></div>');
//            var table2 = $('<div style="float:right;display:inline;margin-top:25px;"></div>');
//            var divlabelOrg = $('<div style="float:left;display:inline;margin-left:5px;"></div>');
//            var table2lbl = $(" <label style='font-size:small !Important;font-weight:normal;float:left;' name='lblRole'>" + VIS.Msg.getMsg("ImportUnderOrganization") + "</label>");
//            divlabelOrg.append(table2lbl);

//            var divComboOrg = $('<div style="float:left;display:inline;margin-left:5px;"></div>');
//            var table2combo = $("<select id='ddlOrg_" + windowNo + "' style='margin-left:2%;float:left;width:200px;'>" + org + "</select>");
//            divComboOrg.append(table2combo);

//            var divbtnVerify = $('<div style="float:left;display:inline;margin-left:5px;"></div>');
//            $btnVerify = $('<input style="margin-left:2%;background-color:#616364;color: white;width:150px;font-weight: 200;font-family: helvetica;margin-bottom: 20px;font-size: 14px;padding: 10px 15px;float:left;" type="submit" value="' + VIS.Msg.getMsg("VerifyData") + '" ></input>');
//            divbtnVerify.append($btnVerify);

//            var divbtnImport = $('<div style="float:left;display:inline;margin-left:5px;"></div>');
//            $btnImport = $('<input  style="margin-left:2%;background-color:#616364;color: white;width:150px;margin-right:12px;font-weight: 200;font-family: helvetica;margin-bottom: 20px;font-size: 14px;padding: 10px 15px;float:left;" type="submit" value="' + VIS.Msg.getMsg("Import") + '"></input>');
//            divbtnImport.append($btnImport);
//            divOrg.append(divlabelOrg);
//            divOrg.append(divComboOrg);
//            table2.append(divbtnVerify);
//            table2.append(divbtnImport);

//            var table2tr2 = $('<div style="height:95%;width:99%;"></div>');
//            table2tr2.append(gmailcontacts);

//            var divTop = $('<div style="margin-top:1%;margin-right:10px;height:5%;"></div>');
//            var $div1 = $(' <div style="float:right;margin-left:5px;"></div>');
//            var $div2 = $('<div style="border:6px solid;color:lightgray;width:5px;height:5px;display:inline;float:left;margin-right: 5px;margin-top:3px;"></div>');
//            var $p1 = $('<p style="float:right;margin-top:0px;font-size:12px;">' + VIS.Msg.getMsg("LinkedContact") + '</p>');
//            $div1.append($div2);
//            $div1.append($p1);

//            var $div3 = $('<div style="float:right;margin-left:5px;"></div>');
//            var $div4 = $('<div style="border:6px solid;color:#D99694;width:5px;display:inline;height:5px;float:left;margin-top: 3px;margin-right: 5px;"></div>');
//            var $p2 = $('<p style="float:left;margin-top:0px;font-size:12px;">' + VIS.Msg.getMsg("UnLinkedContact") + '</p>');
//            $div3.append($div4);
//            $div3.append($p2);

//            var $div5 = $('<div style="float:right;margin-left:5px;"></div>');
//            $chkbxShowLinkedContacts = $('<input style="margin-bottom:1px;display:inline;float:left;margin-top:3px;margin-right:5px;" type="checkbox" id="chkShowLinkedContact_"></input>');
//            var $p3 = $('<p style="float:left;font-size:12px;">' + VIS.Msg.getMsg("ShowLinkedContacts") + '</p>');
//            $div5.append($chkbxShowLinkedContacts);
//            $div5.append($p3);
//            divTop.append($div5);
//            divTop.append($div3);
//            divTop.append($div1);




//            gmailcontactsbtnrow.append(divTop);
//            gmailcontactsbtnrow.append(table2tr2);
//            gmailcontactsbtnrow.append(divOrg);
//            gmailcontactsbtnrow.append(table2);
//            gmailcontactsbtnrow.append(bsyDiv);

//            bsyDiv[0].style.visibility = 'hidden';
//            divOrg = null; table2 = null; divlabelOrg = null; table2lbl = null; divComboOrg = null; table2combo = null; divbtnVerify = null;
//            divbtnImport = null; table2tr2 = null; divTop = null; $div1 = null; $div2 = null; $p1 = null; $div3 = null; $div4 = null; $p2 = null;
//            $div5 = null; $p3 = null;
//        };


//        //********************
//        //Get Percentage 
//        //param1=value whose percentage to get
//        //param2=how many percentage of param1 you want 
//        //e.g if you want 50% of 500 then call as getpercentage(500,50)
//        //********************
//        function getpercentage(value, percentile) {
//            return (parseInt(value) * parseInt(percentile)) / 100;
//        }


//        //********************
//        //Dispose the variables
//        //*********************
//        this.disposeComponent = function () {
//            debugger;
//            if ($dGrid != null) {
//                for (var i = 0; i < $dGrid.records.length; i++) {

//                    $("#chk_" + i).remove();
//                }
//            }
//            if ($root != null) {
//                $root.empty();
//            }
//            $root = null;
//            $("#chkSelectAll").remove();
//            self = this;
//            count = null;
//            chkSelect = null;
//            windowNo = null;
//            settingsDialog = null;
//            gmailcontactdialog = null;
//            grdCols = null;
//            dData = null;
//            originalData = null;
//            $mainTable = null;
//            $tr0 = null;
//            $td01 = null;
//            $lblUsername = null;
//            $td02 = null;
//            $txtUsername = null;
//            $tr1 = null;
//            $td11 = null;
//            $lblPassword = null;
//            $td12 = null;
//            $txtPassword = null;
//            $tr2 = null;
//            $td21 = null;
//            $lblRole = null;
//            $td22 = null;
//            $cmbRole = null;
//            $tr3 = null;
//            $td31 = null;
//            $lblchkExisting = null;
//            $chkbxUpdateExisting = null;
//            $lblchkShowLinkedContacts = null;
//            $chkbxShowLinkedContacts = null;
//            chkShowLinkedContacts = null;
//            $contactsetting = null;
//            $dGrid = null;
//            gContactHtml = null;
//            bpAddress = null;
//            bpGroupSource = null;
//            bpgroup = null;
//            org = null;
//            $btnImport = null;
//            $btnVerify = null;
//            $btnRequery = null;
//            uName = null;
//            pWord = null;
//            role = null;
//            userName = null;
//            $isBusy = null;
//            $isBusyImg = null;
//            password = null;
//            role = null;
//            bpgroup = null;
//            chkUpdateExistingRecords = null;
//            windowWidth = null;
//            windowHeight = null;
//            gmailcontacts = null;
//            gmailcontactsbtnrow = null;
//            gmailcontactsbtnrow = null;
//            $root = null;
//            bsyDiv = null;

//        };




//        //**************************
//        //Get Business Partner Group to fill the combobox BPGroup in grid
//        //**************************
//        function getbpgroup() {

//            var url = VIS.Application.contextUrl + "WSP/ContactSettings/GetBPGroup";
//            $.ajax({
//                type: "GET",
//                async: false,
//                url: url,
//                dataType: "json",

//                success: function (data) {
//                    var record = {};
//                    var dd = JSON.parse(data);
//                    for (var i = 0; i < dd.length; i++) {
//                        bpgroup += "<option value='" + dd[i]["ID"] + "'>" + dd[i]["Name"] + "</option>";
//                        record = {};
//                        record['id'] = dd[i]["ID"];
//                        record['text'] = dd[i]["Name"];
//                        bpGroupSource[i] = record;
//                    }
//                }
//            });
//        };


//        //Get Root frame
//        this.getRoot = function () {
//            return $root;
//        };

//        this.getdataGrid = function () {
//            return gmailcontactsbtnrow;
//        };


//    };

//    VIS.gmailcontacts.prototype.refresh = function () {
//        //var h = $(window).height();
//        //var $root = this.getRoot();
//        //if ($root != null) {
//        //    $root.css("height", $(window).height());
//        //    //gmailcontactsbtnrow.css("height", (parseInt(windowHeight) - 210));
//        //}

//        $(window).trigger("resize");
//    };

//    VIS.gmailcontacts.prototype.init = function (windowNo, frame) {

//        //Assign to this Varable
//        debugger;
//        this.frame = frame;
//        windowNo = windowNo;
//        this.windowNo = windowNo;
//        frame.hideHeader(true);
//        this.frame.getContentGrid().append(this.getRoot());
//        this.height = this.frame.getContentGrid().height();
//        //to load scheduler
//        this.loadSavedData();

//    };

//    //Must implement dispose
//    VIS.gmailcontacts.prototype.dispose = function () {
//        /*CleanUp Code */
//        //dispose this component
//        debugger;
//        this.disposeComponent();

//        //call frame dispose function
//        if (this.frame)
//            this.frame.dispose();
//        this.frame = null;
//    };

//})(VIS, jQuery);
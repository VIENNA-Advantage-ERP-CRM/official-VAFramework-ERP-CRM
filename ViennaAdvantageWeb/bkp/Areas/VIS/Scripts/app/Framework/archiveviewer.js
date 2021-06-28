; (function (VIS, $) {
    VIS.Apps = VIS.Apps || {};
    VIS.Apps.AForms = VIS.Apps.AForms || {}

    function ArchiveViewer() {
        this.log = VIS.Logging.VLogger.getVLogger("VIS.ArchiveViewer");
        this.frame;
        this.windowNo;
        /** Archive Index		*/
        var index = 0;
        /** Table direct		*/
        var gAD_Table_ID = 0;
        /** Record direct		*/
        var gRecord_ID = 0;


        var $self = this;

        var toggle = false;
        var toggleGen = false;
        var toggleside = false;

        var $root = $("<div class='vis-forms-container' style='width: 100%; height: 100%; background-color: white;'>");
        var $busyDiv = $('<div class="vis-busyindicatorouterwrap" style="visibility: hidden"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');

        var arrListColumns = [];
        var dGrid = null;
        var archiveId = 0;

        //Query
        var chkReportQ = null;
        var lblBPartnerQ = null;
        var lblProcessQ = null;
        var lblTableQ = null;
        var lblNameQ = null;
        var lblDescriptionQ = null;
        var lblCommentQ = null;
        var lblCreatedByQ = null;
        var lblCreatedOnQ = null;
        var vSearchBPartnerQ = null;
        var cmbTableQ = null;
        var txtNameQ = null;
        var txtDescriptionQ = null;
        var txtCommentQ = null;
        var cmbCreatedByQ = null;
        var cmbProcess = null;
        var dtpCreatedFromQ = null;
        var dtpCreatedToQ = null;

        //view

        var lblCreatedBy = null;
        var lblCreatedOn = null;
        var lblName = null;
        var lblDescription = null;
        var lblComment = null;
        var txtName = null;
        var txtDescription = null;
        var txtComment = null;
        var txtCreatedBy = null;
        var dtpCreatedOn = null;

        var lblRecordsCount = null;

        var btnOk = null;
        var btnSave = null;
        var btnNext = null;
        var btnPre = null;
        this.btnToggal = null;


        var topLeftDiv = null;
        var topRightDiv = null;
        var topleftparaDiv = null;
        var topleftInnerParaDiv = null;
        var leftBottomDiv = null;

        var rightTopDiv = null;
        var rightBottomDiv = null;
        var rightBottomSaveDiv = null;

        var leftDivWidth = 260;
        var minSideWidth = 50;

        //window with-(sidediv with_margin from left+ space)
        var selectLeftDivWidth = $(window).width() - (leftDivWidth);
        var selectDivFullWidth = $(window).width() - (minSideWidth);
        var sideDivHeight = $(window).height() - 193;
        var gridHeight = $(window).height() - 382;
        var rightparaHeight = $(window).height() - (gridHeight + 55 + 118);

        function initializeComponent() {

            chkReportQ = $("<label id='" + "lblReportQ_" + $self.windowNo + "' class='vis-ec-col-lblchkbox'><input id='" + "chkReportQ_" + $self.windowNo + "' type='checkbox' style='margin-left: 0px;' class='VIS_Pref_automatic'>" + VIS.Msg.translate(VIS.Env.getCtx(), "IsReport") +
           "</label>");

            lblBPartnerQ = new VIS.Controls.VLabel();
            lblProcessQ = new VIS.Controls.VLabel();
            lblTableQ = new VIS.Controls.VLabel();
            lblNameQ = new VIS.Controls.VLabel();
            lblDescriptionQ = new VIS.Controls.VLabel();
            lblCommentQ = new VIS.Controls.VLabel();
            lblCreatedByQ = new VIS.Controls.VLabel();
            lblCreatedOnQ = new VIS.Controls.VLabel();

            var lookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), $self.windowNo, 0, VIS.DisplayType.Search, "C_BPartner_ID", 0, false, null);
            vSearchBPartnerQ = new VIS.Controls.VTextBoxButton("C_BPartner_ID", false, false, true, VIS.DisplayType.Search, lookup);

            cmbTableQ = new VIS.Controls.VComboBox('', false, false, true);
            cmbProcess = new VIS.Controls.VComboBox('', false, false, true);

            txtNameQ = new VIS.Controls.VTextBox("NameQ", false, false, true, 50, 50, "NameQ");
            txtDescriptionQ = new VIS.Controls.VTextBox("DescriptionQ", false, false, true, 50, 50, "DescriptionQ");
            txtCommentQ = new VIS.Controls.VTextBox("CommentQ", false, false, true, 50, 50, "CommentQ");

            txtNameQ.getControl().val("%%");
            txtDescriptionQ.getControl().val("%%");
            txtCommentQ.getControl().val("%%");

            cmbCreatedByQ = new VIS.Controls.VComboBox('', false, false, true);
            dtpCreatedFromQ = $("<input id='" + "dtpCreatedFromQ_" + $self.windowNo + "' type='date'>")
            dtpCreatedToQ = $("<input id='" + "dtpCreatedToQ_" + $self.windowNo + "' type='date'>");

            lblCreatedBy = new VIS.Controls.VLabel();
            lblCreatedOn = new VIS.Controls.VLabel();
            lblName = new VIS.Controls.VLabel();
            lblDescription = new VIS.Controls.VLabel();
            lblComment = new VIS.Controls.VLabel();

            txtCreatedBy = new VIS.Controls.VTextBox("CreatedBy", false, true, true, 50, 50, "CreatedBy");
            txtName = new VIS.Controls.VTextBox("Name", false, false, true, 50, 50, "Name");
            txtDescription = new VIS.Controls.VTextBox("Description", false, false, true, 50, 50, "Description");
            txtComment = new VIS.Controls.VTextArea("Comment", false, false, true, 200, 200);
            dtpCreatedOn = $("<input id='" + "dtpCreatedOn_" + $self.windowNo + "' readonly class='vis-gc-vpanel-table-readOnly'>");

            lblRecordsCount = new VIS.Controls.VLabel();

            btnOk = $("<input id='" + "btnOk_" + $self.windowNo + "' class='VIS_Pref_btn-2 vis-frm-button' style=' margin-top: 0px; margin-right: 0px;' type='button' value='Ok'>");
            btnSave = $("<input id='" + "btnSave_" + $self.windowNo + "' class='VIS_Pref_btn-2' style='margin-top: 0px;float: right;' type='button' value='Save'>");
            btnNext = $("<input id='" + "btnNext_" + $self.windowNo + "' class='VIS_Pref_btn-2' style='margin: 10px; display: none;'  type='button' value='>>'>");
            btnPre = $("<input id='" + "btnPre_" + $self.windowNo + "' class='VIS_Pref_btn-2' style='margin: 10px; display: none;' type='button' value='<<'>");

            btnSave.val(VIS.Msg.getMsg("Save"));
            btnOk.val(VIS.Msg.getMsg("OK"));

            //Left Div Design settings 
            var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/arrow-left.png";
            topLeftDiv = $("<div class='vis-archive-left-sidebar' id='" + "topLeftDiv_" + $self.windowNo + "'>" +
                "<div id='" + "topToggalDiv_" + $self.windowNo + "' class='vis-archive-l-s-head'>" +
                       "<button id='" + "btnToggal_" + $self.windowNo + "' class='vis-archive-sb-t-button'><i class='vis vis-arrow-left'></i></button></div></div>");

            this.btnToggal = topLeftDiv.find("#btnToggal_" + $self.windowNo);

            topleftparaDiv = $("<div class='vis-archive-l-s-content vis-leftsidebarouterwrap' id='" + "topleftparaDiv_" + $self.windowNo + "'>");
            //topleftparaDiv.css("height", sideDivHeight);

            topleftInnerParaDiv = $("<div  class='vis-archive-l-s-content-inner' id='" + "topleftInnerParaDiv_" + $self.windowNo + "'>");
            topleftparaDiv.append(topleftInnerParaDiv);
            topLeftDiv.append(topleftparaDiv);
            topLeftDiv.css("width", leftDivWidth);

            var tble = $("<table style='width: 100%' class='vis-formouterwrpdiv'>");

            var tr = $("<tr>");
            var td = $("<td style='padding: 0px 10px 0px;'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');

            topleftInnerParaDiv.append(tble);
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(chkReportQ);

            //tr = $("<tr>");
            //td = $("<td style='padding: 0px 10px 0px;'>");
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblProcessQ.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font")).append(lblBPartnerQ.getControl().addClass("VIS-Search-hide").css("display", "inline-block").addClass("VIS_Pref_Label_Font"));
            //lblProcessQ.getControl().hide();

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px;'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            var Leftformfieldbtnwrap = $('<div class="input-group-append">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldwrp.append(Leftformfieldbtnwrap);
            Leftformfieldctrlwrp.append(vSearchBPartnerQ.getControl().attr('data-placeholder', '').attr('placeholder', ' ').attr('data-hasbtn', ' ').addClass("VIS-Search-hide"));
            Leftformfieldbtnwrap.append(vSearchBPartnerQ.getBtn(0).addClass("VIS-Search-hide"));
            Leftformfieldctrlwrp.append(lblBPartnerQ.getControl().addClass("VIS-Search-hide").addClass("VIS_Pref_Label_Font"));
            Leftformfieldctrlwrp.append(cmbProcess.getControl());
            Leftformfieldctrlwrp.append(lblProcessQ.getControl().addClass("VIS_Pref_Label_Font"));
            lblProcessQ.getControl().hide();
            cmbProcess.getControl().hide();

            //tr = $("<tr>");
            //td = $("<td style='padding: 0px 10px 0px;'>");
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblTableQ.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px;'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(cmbTableQ.getControl());
            Leftformfieldctrlwrp.append(lblTableQ.getControl().addClass("VIS_Pref_Label_Font"));


            //tr = $("<tr>");
            //td = $("<td style='padding: 0px 10px 2px;'>");
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblNameQ.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px;'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(txtNameQ.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
            Leftformfieldctrlwrp.append(lblNameQ.getControl().addClass("VIS_Pref_Label_Font"));

            //tr = $("<tr>");
            //td = $("<td style='padding: 0px 10px 0px;'>");
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblDescriptionQ.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px;'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(txtDescriptionQ.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
            Leftformfieldctrlwrp.append(lblDescriptionQ.getControl().addClass("VIS_Pref_Label_Font"));

            //tr = $("<tr>");
            //td = $("<td style='padding: 0px 10px 0px;'>");
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblCommentQ.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px;'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(txtCommentQ.getControl().attr('data-placeholder', '').attr('placeholder', ' '));
            Leftformfieldctrlwrp.append(lblCommentQ.getControl().addClass("VIS_Pref_Label_Font"));

            //tr = $("<tr>");
            //td = $("<td style='padding: 0px 10px 0px;'>");
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblCreatedByQ.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px;'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(cmbCreatedByQ.getControl());
            Leftformfieldctrlwrp.append(lblCreatedByQ.getControl().addClass("VIS_Pref_Label_Font"));

            //tr = $("<tr>");
            //td = $("<td style='padding: 0px 10px 2px;'>");
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblCreatedOnQ.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px;'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(dtpCreatedFromQ);
            Leftformfieldctrlwrp.append(lblCreatedOnQ.getControl().addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td style='padding: 0px 10px 0px;'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(dtpCreatedToQ);

            //Bottom Ok Button for search
            leftBottomDiv = $("<div class='vis-frm-bot-btn-wrp' id='" + "leftBottomDiv_" + $self.windowNo + "'>");
            topLeftDiv.append(leftBottomDiv);
            leftBottomDiv.append(btnOk);

            //Right Div Setting
            topRightDiv = $("<div id='" + "topRightDiv_" + $self.windowNo + "' class='vis-frm-grid-wrap'>");
            //topRightDiv.css("width", selectLeftDivWidth);
            //topRightDiv.css("height", sideDivHeight);

            //check Arebic Calture
            //if (VIS.Application.isRTL) {
            //    topRightDiv.css("float", "left");
            //    btnSave.css("float", "left");
            //    chkReportQ.css("margin-left", "5px");
            //}



            rightTopDiv = $("<div id='" + "rightTopDiv_" + $self.windowNo + "' style='float: left; width: 100%; height: 55%;'>");
            rightBottomDiv = $("<div id='" + "rightBottomDiv_" + $self.windowNo + "' style='float: left; width: 100%;margin-top: 5px; margin-bottom: 10px; '>");
            //rightTopDiv.css("height", gridHeight);
            rightTopDiv.css("height", "calc(100% - 274px)");
            rightBottomDiv.css("height", rightparaHeight);


            tble = $("<table style='width: 100%; ' class='vis-formouterwrpdiv'>");

            tr = $("<tr>");
            td = $("<td colspan='4' style='text-align: right; height: 10px;'>");//55px
            rightBottomDiv.append(tble);
            tble.append(tr);
            tr.append(td);
            td.append(btnPre).append(lblRecordsCount.getControl().css("display", "inline-block").css("margin-top", "20px").css("margin-right", "10px").css("margin-left", "10px").addClass("VIS_Pref_Label_Font")).append(btnNext);
            lblRecordsCount.getControl().css("display", "none");

            //tr = $("<tr>");
            //td = $("<td colspan='2' class='VIS-archive-table-padding' >");
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblCreatedBy.getControl().addClass("VIS_Pref_Label_Font"));

            //td = $("<td colspan='2' class='VIS-archive-table-padding'>");
            //tr.append(td);
            //td.append(lblCreatedOn.getControl().addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td colspan='2' class='VIS-archive-table-padding'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(txtCreatedBy.getControl().attr('data-placeholder', '').attr('placeholder', ' ').addClass("VIS_Pref_Label_Font"));
            Leftformfieldctrlwrp.append(lblCreatedBy.getControl().addClass("VIS_Pref_Label_Font"));

            td = $("<td colspan='2' class='VIS-archive-table-padding'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(dtpCreatedOn.addClass("VIS_Pref_Label_Font"));
            Leftformfieldctrlwrp.append(lblCreatedOn.getControl().addClass("VIS_Pref_Label_Font"));


            //tr = $("<tr>");
            //td = $("<td colspan='2' class='VIS-archive-table-padding' >");
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblName.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            //td = $("<td colspan='2' class='VIS-archive-table-padding'>");
            //tr.append(td);
            //td.append(lblDescription.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td colspan='2' class='VIS-archive-table-padding'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(txtName.getControl().attr('data-placeholder', '').attr('placeholder', ' ').addClass("VIS_Pref_Label_Font"));
            Leftformfieldctrlwrp.append(lblName.getControl().addClass("VIS_Pref_Label_Font"));
            
            td = $("<td colspan='2' class='VIS-archive-table-padding'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(txtDescription.getControl().attr('data-placeholder', '').attr('placeholder', ' ').addClass("VIS_Pref_Label_Font"));
            Leftformfieldctrlwrp.append(lblDescription.getControl().addClass("VIS_Pref_Label_Font"));


            //tr = $("<tr>");
            //td = $("<td colspan='2' class='VIS-archive-table-padding'>");
            //tble.append(tr);
            //tr.append(td);
            //td.append(lblComment.getControl().css("display", "inline-block").addClass("VIS_Pref_Label_Font"));

            tr = $("<tr>");
            td = $("<td colspan='4' class='VIS-archive-table-padding'>");
            var Leftformfieldwrp = $('<div class="input-group vis-input-wrap">');
            var Leftformfieldctrlwrp = $('<div class="vis-control-wrap">');
            tble.append(tr);
            tr.append(td);
            td.append(Leftformfieldwrp);
            Leftformfieldwrp.append(Leftformfieldctrlwrp);
            Leftformfieldctrlwrp.append(txtComment.getControl().attr('data-placeholder', '').attr('placeholder', ' ').addClass("VIS_Pref_Label_Font"));
            Leftformfieldctrlwrp.append(lblComment.getControl().addClass("VIS_Pref_Label_Font"));

            //topRightDiv.append(rightTopDiv).append(rightBottomDiv);

            //Bottom save Button for search
            rightBottomSaveDiv = $("<div style='float: left; width: 100%;height: 40px;position: relative;' id='" + "rightBottomSaveDiv_" + $self.windowNo + "'>");
            rightBottomSaveDiv.append(btnSave);

            topRightDiv.append(rightTopDiv).append(rightBottomDiv).append(rightBottomSaveDiv);

            //tr = $("<tr>");
            //td = $("<td colspan='4'>");
            //tble.append(tr);
            //tr.append(td);
            //td.append(btnSave);

            //Add to root
            $root.append($busyDiv);
            $root.append(topLeftDiv).append(topRightDiv);
        }

        function jbInit() {

            //chkReportQ.find("label").text(VIS.Msg.translate(VIS.Env.getCtx(), "IsReport"));
            lblProcessQ.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "AD_Process_ID"));
            lblTableQ.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "AD_Table_ID"));
            lblBPartnerQ.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "C_BPartner_ID"));
            lblNameQ.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "Name"));
            lblDescriptionQ.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "Description"));
            lblCommentQ.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "Help"));
            lblCreatedByQ.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "CreatedBy"));
            lblCreatedOnQ.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "Created"));

            lblCreatedBy.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "CreatedBy"));
            lblCreatedOn.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "Created"));
            lblName.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "Name"));
            lblDescription.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "Description"));
            lblComment.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "Help"));
            lblRecordsCount.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "NoRecordFound"));

        }

        function dynInit() {
            var AD_Role_ID = VIS.context.getAD_Role_ID();
            var defaultItem = true;
            //	Processes
            var sql = "SELECT DISTINCT p.AD_Process_ID, p.Name "
                    + "FROM AD_Process p INNER JOIN AD_Process_Access pa ON (p.AD_Process_ID=pa.AD_Process_ID) "
                    + "WHERE pa.AD_Role_ID=" + AD_Role_ID
                    + " AND p.IsReport='Y' AND p.IsActive='Y' AND pa.IsActive='Y' "
                    + "ORDER BY 2";

            var dr = VIS.DB.executeReader(sql.toString(), null);
            var key, value;
            while (dr.read()) {
                key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
                value = VIS.Utility.encodeText(dr.getString(1));
                if (defaultItem) {
                    cmbProcess.getControl().append(" <option></option>");
                    defaultItem = false;
                }
                cmbProcess.getControl().append(" <option value=" + key + ">" + value + "</option>");
            }
            dr.close();
            defaultItem = true;
            cmbProcess.getControl().prop('selectedIndex', 0);

            //	Tables
            sql = "SELECT DISTINCT t.AD_Table_ID, t.Name "
                + "FROM AD_Table t INNER JOIN AD_Tab tab ON (tab.AD_Table_ID=t.AD_Table_ID)"
                + " INNER JOIN AD_Window_Access wa ON (tab.AD_Window_ID=wa.AD_Window_ID) "
                + "WHERE wa.AD_Role_ID=" + AD_Role_ID
                + " AND t.IsActive='Y' AND tab.IsActive='Y' "
                + "ORDER BY 2";

            dr = VIS.DB.executeReader(sql.toString(), null);

            while (dr.read()) {
                key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
                value = VIS.Utility.encodeText(dr.getString(1));
                if (defaultItem) {
                    cmbTableQ.getControl().append(" <option></option>");
                    defaultItem = false;
                }
                cmbTableQ.getControl().append(" <option value=" + key + ">" + value + "</option>");
            }
            dr.close();
            defaultItem = true;
            cmbTableQ.getControl().prop('selectedIndex', 0);

            //	Internal Users
            sql = "SELECT AD_User_ID, Name "
                + "FROM AD_User u WHERE EXISTS "
                    + "(SELECT * FROM AD_User_Roles ur WHERE u.AD_User_ID=ur.AD_User_ID) "
                + "ORDER BY 2";

            sql = VIS.MRole.getDefault().addAccessSQL(sql,		//	Own First
                "AD_User", VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RW);

            dr = VIS.DB.executeReader(sql.toString(), null);
            while (dr.read()) {
                key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
                value =VIS.Utility.encodeText( dr.getString(1));
                if (defaultItem) {
                    cmbCreatedByQ.getControl().append(" <option></option>");
                    defaultItem = false;
                }
                cmbCreatedByQ.getControl().append(" <option value=" + key + ">" + value + "</option>");
            }
            dr.close();
            defaultItem = true;
            cmbCreatedByQ.getControl().prop('selectedIndex', 0);

            if (gAD_Table_ID > 0) {
                //reportField.IsChecked = true;
                chkReportQ.find('input').prop("checked", true);
                cmdQuery();
            }
        }

        function query(isReport, AD_Table_ID, Record_ID) {
            $self.log.config("Report=" + isReport + ", AD_Table_ID=" + AD_Table_ID + ",Record_ID=" + Record_ID);
            chkReportQ.find('input').prop("checked", isReport);
            gAD_Table_ID = AD_Table_ID;
            gRecord_ID = Record_ID;
            cmdQuery();

        }

        function updateVDisplay(next) {
            //var  m_archives=dGrid.records.length;
            //var m_index = Number(dGrid.getSelection().toString());

            //$self.log.info("Index=" + m_index + ", Length=" + m_archives);
            //if (m_archives == 0)
            //{
            //    lblRecordsCount.getControl().val("No Record Found");
            //    txtName.getControl().val("");
            //    txtDescription.getControl().val("");
            //    txtComment.getControl().val("");
            //    txtCreatedBy.getControl().val("");
            //    if (dGrid.get(m_index).CREATED != null) {
            //        dtpCreatedOn.val(null)
            //    }
            //    return;
            //}

            //plblRecordsCount.getControl().val(m_index + 1 + " of " + m_archives);
            //MArchive ar = m_archives[m_index];
            //createdByField.Text = ar.MCreatedByName;
            //createdField.Text = ar.GetCreated().ToString();
            //nameField.Text = ar.GetName() == null ? "" : ar.GetName();
            //descriptionField.Text = ar.GetDescription() == null ? "" : ar.GetDescription();
            //helpField.Text = ar.GetHelp() == null ? "" : ar.GetHelp();

            //try
            //{
            //    MemoryStream ms = ar.GetInputStream();
            //    if (c1ReportViewer1 != null)
            //    {
            //        c1ReportViewer1.LoadDocument(ms);
            //    }
            //}
            //catch ( e)
            //{
            //    log.Log(Level.SEVERE, "pdf", e);
            //    PageStatus.IsBusy = false;
            //    pdfViewer.clearDocument();
            //}

        }

        function cmdQuery() {
            var sql = "";
            var reports = chkReportQ.find('input').prop("checked");
            var role = null;

            role = VIS.MRole.getDefault();
            try {
                if (!role.getIsCanReport()) {
                    $self.log.warning("User/Role cannot Report AD_User_ID=" + VIS.context.getAD_User_ID());
                    return;
                }
                sql = sql.concat(" AND IsReport=").concat(reports ? "'Y'" : "'N'");
                //	Process
                if (reports) {
                    var nn = cmbProcess.getControl().find('option:selected').val();

                    if (nn != "" && nn != null)
                        sql = sql.concat(" AND AD_Process_ID=").concat(nn);
                }

                //	Table
                if (gAD_Table_ID > 0) {
                    sql = sql.concat(" AND ((AD_Table_ID=").concat(gAD_Table_ID);

                    if (gRecord_ID > 0)
                        sql = sql.concat(" AND Record_ID=").concat(gRecord_ID);

                    sql = sql.concat(")");

                    if (gAD_Table_ID == X_C_BPartner.Table_ID && gRecord_ID > 0)
                        sql = sql.concat(" OR C_BPartner_ID=").concat(gRecord_ID);

                    sql = sql.concat(")");
                    //	Reset for query
                    gAD_Table_ID = 0;
                    gRecord_ID = 0;
                }
                else {
                    var nn = cmbTableQ.getControl().find('option:selected').val();

                    if (nn != "" && nn != null)
                        sql = sql.concat(" AND AD_Table_ID=").concat(nn);
                }

                //	Business Partner
                if (!reports) {
                    var ii = vSearchBPartnerQ.getValue();
                    if (ii != null)
                        sql = sql.concat(" AND C_BPartner_ID=").concat(ii);
                    else
                        sql = sql.concat(" AND C_BPartner_ID IS NOT NULL");
                }

                //	Name
                var ss = txtNameQ.getControl().val();
                if (ss != null && ss != "%%" && ss.length > 0) {
                    if (ss.indexOf("%") != -1 || ss.indexOf("_") != -1)
                        sql = sql.concat(" AND LOWER(Name) LIKE LOWER( ").concat("'").concat(ss).concat("')");
                    else
                        sql = sql.concat(" AND LOWER(Name)= LOWER(").concat("'").concat(ss).concat("')");
                }

                //	Description
                ss = txtDescriptionQ.getControl().val();
                if (ss != null && ss != "%%" && ss.length > 0) {
                    if (ss.indexOf("%") != -1 || ss.indexOf("_") != -1)
                        sql = sql.concat(" AND LOWER(Description) LIKE LOWER( ").concat("'").concat(ss).concat("')");
                    else
                        sql = sql.concat(" AND LOWER(Description)= LOWER(").concat("'").concat(ss).concat("')");
                }

                //	Help
                ss = txtCommentQ.getControl().val();
                if (ss != null && ss != "%%" && ss.length > 0) {
                    if (ss.indexOf("%") != -1 || ss.indexOf("_") != -1)
                        sql = sql.concat(" AND LOWER(Help) LIKE LOWER( ").concat("'").concat(ss).concat("')");
                    else
                        sql = sql.concat(" AND LOWER(Help)= LOWER(").concat("'").concat(ss).concat("')");
                }

                //	CreatedBy
                var nnn = cmbCreatedByQ.getControl().find('option:selected').val();
                if (nnn != null && nnn > 0)
                    sql = sql.concat(" AND CreatedBy=").concat(nnn);

                //	Created
                var tt = dtpCreatedFromQ.val();
                //JID_1725 getting the Data between fromdate and todate
                if (tt != "")
                    sql = sql.concat(" AND ").concat("TRUNC(").concat("Created,'DD') >= ").concat(VIS.DB.to_date(tt));
                var tt = dtpCreatedToQ.val();
                if (tt != "")
                    sql = sql.concat(" AND ").concat("TRUNC(").concat("Created,'DD') <= ").concat(VIS.DB.to_date(tt));               

                $self.log.fine(sql.toString());

                //	Process Access
                sql = sql.concat(" AND (AD_Process_ID IS NULL OR AD_Process_ID IN "
        + "(SELECT AD_Process_ID FROM AD_Process_Access WHERE AD_Role_ID=")
        .concat(VIS.context.getAD_Role_ID()).concat("))");
                //	Table Access
                sql = sql.concat(" AND (AD_Table_ID IS NULL "
        + "OR (AD_Table_ID IS NOT NULL AND AD_Process_ID IS NOT NULL) "	//	Menu Reports 
        + "OR AD_Table_ID IN "
        + "(SELECT t.AD_Table_ID FROM AD_Tab t"
        + " INNER JOIN AD_Window_Access wa ON (t.AD_Window_ID=wa.AD_Window_ID) "
        + "WHERE wa.AD_Role_ID=").concat(VIS.context.getAD_Role_ID()).concat("))");
                $self.log.finest(sql.toString());
            }
            catch (e) {
            }

            var whereClause = sql;

            var sqlMain = "SELECT AD_ARCHIVE_ID,AD_CLIENT_ID,AD_ORG_ID,AD_PROCESS_ID,AD_TABLE_ID,C_BPARTNER_ID,CREATED,CREATEDBY,DESCRIPTION,HELP," +
            " ISACTIVE,ISREPORT,NAME,RECORD_ID,UPDATED,UPDATEDBY,EXPORT_ID FROM AD_Archive WHERE AD_Client_ID=" + VIS.Env.getCtx().getAD_Client_ID();
            if (whereClause != null && whereClause.length > 0)
                sqlMain += whereClause;
            sqlMain += " ORDER BY Created desc";

            getData(sqlMain);
        }

        function isSame(s1, s2) {
            if (s1 == null)
                return s2 == null;
            else if (s2 == null)
                return false;
            else
                return s1.equals(s2);
        }

        function cmdUpdateArchive() {
            if (dGrid.getSelection().length < 0) {
                return;
            }

            var name = txtName.getControl().val();
            var des = txtDescription.getControl().val();
            var help = txtComment.getControl().val();

            var recid = Number(dGrid.getSelection().toString());
            if (dGrid.get(recid) == null) {
                return;
            }

            var update = false;
            if (!isSame(name, dGrid.get(recid).NAME)) {
                if (name != null && name.length > 0) {
                    update = true;
                }
            }
            if (!isSame(des, dGrid.get(recid).DESCRIPTION)) {
                update = true;
            }
            if (!isSame(help, dGrid.get(recid).HELP)) {
                update = true;
            }

            archiveId = dGrid.get(recid).AD_ARCHIVE_ID;

            //Aject Request
            if (update) {
                $.ajax({
                    url: VIS.Application.contextUrl + "Common/UpdateArchive",
                    dataType: "json",
                    data: {
                        name: name,
                        des: des,
                        help: help,
                        archiveId: archiveId
                    },
                    success: function (data) {
                        if (data.result) {
                            //refresh grid
                            cmdQuery();
                        }
                    }
                });
            }
        }

        function initGrid(data) {

            if (dGrid != null) {
                dGrid.destroy();
                dGrid = null;
            }
            if (arrListColumns.length == 0) {

                arrListColumns.push({ field: "NAME", caption: VIS.Msg.translate(VIS.Env.getCtx(), "NAME"), sortable: true, size: '16%', min: 150, hidden: false });
                arrListColumns.push({ field: "DESCRIPTION", caption: VIS.Msg.translate(VIS.Env.getCtx(), "DESCRIPTION"), sortable: true, size: '16%', min: 150, hidden: false });
                arrListColumns.push({ field: "HELP", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "HELP"), sortable: true, size: '16%', min: 150, hidden: false });
                arrListColumns.push({ field: "CREATEDBY", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "CREATEDBY"), sortable: true, size: '16%', min: 150, hidden: false });
                arrListColumns.push({ field: "CREATED", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "CREATED"), sortable: true, size: '16%', min: 150, hidden: false });
                arrListColumns.push({
                    field: "View", caption: VIS.Msg.translate(VIS.Env.getCtx(), "View"), sortable: true, size: '80px', min: 150, hidden: false,
                    render: function () { return '<div><i class="vis vis-download" title="View record" style="opacity: 1; font-size: 1rem"></i></div>'; }
                });

                arrListColumns.push({ field: "AD_ARCHIVE_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "AD_ARCHIVE_ID"), hidden: true });
                //arrListColumns.push({ field: "AD_PROCESS_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "AD_PROCESS_ID"), hidden: true });
                //arrListColumns.push({ field: "AD_TABLE_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "AD_TABLE_ID"), hidden: true });
                //arrListColumns.push({ field: "C_BPARTNER_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "C_BPARTNER_ID"), hidden: true });
                //arrListColumns.push({ field: "RECORD_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "RECORD_ID"), hidden: true });
                //arrListColumns.push({ field: "EXPORT_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "EXPORT_ID"), hidden: true });
            }
            setTimeout(10);
            w2utils.encodeTags(data);
            dGrid = $(rightTopDiv).w2grid({
                name: "gridArchive" + $self.windowNo,
                recordHeight: 40,
                columns: arrListColumns,
                records: data,
                onClick: function (event) {
                    if (event.column == 5 && dGrid.records.length > 0) {
                        downloadPdf(event.recid);
                    }
                    fillControls(event.recid);
                }
            });
        }

        function downloadPdf(recid) {
            archiveId = dGrid.get(recid).AD_ARCHIVE_ID;

            $.ajax({
                url: VIS.Application.contextUrl + "Common/DownloadPdf",
                dataType: "json",
                data: {
                    archiveId: archiveId
                },
                success: function (data) {
                    if (data.result != null) {
                        if (data.result.indexOf("TempDownload") > -1) {
                            window.open(VIS.Application.contextUrl + data.result);
                        }
                        else {
                            window.open("data:application/pdf;base64, " + data.result);
                        }
                    }
                }
            });
        }

        function fillControls(recid) {
            txtName.getControl().val(dGrid.get(recid).NAME);
            txtDescription.getControl().val(dGrid.get(recid).DESCRIPTION);
            txtComment.getControl().val(dGrid.get(recid).HELP);
            txtCreatedBy.getControl().val(dGrid.get(recid).CREATEDBY);

            if (dGrid.get(recid).CREATED != null) {
                //dtpCreatedOn.val((dGrid.get(recid).CREATED).split('T')[0])
                dtpCreatedOn.val(dGrid.get(recid).CREATED)
            }

            archiveId = dGrid.get(recid).AD_ARCHIVE_ID;
        }

        function getCreatedByName(createdby) {
            var name = "";
            var sql = "SELECT Name FROM AD_User WHERE AD_User_ID=" + createdby;
            try {
                var dr = VIS.DB.executeReader(sql.toString(), null, null);
                if (dr.read()) {
                    name = dr.getString('Name');
                }
                dr.close();
            }
            catch (e) {
                //log.Log(Level.SEVERE, sql, e);
            }
            return name;
        }

        function getData(sql) {
            var data = [];
            try {
                var dr = VIS.DB.executeReader(sql.toString(), null, null);
                var count = 1;
                while (dr.read()) {


                    var line = {};
                    line['NAME'] = dr.getString('NAME');
                    line['DESCRIPTION'] = dr.getString('DESCRIPTION');
                    line['HELP'] = dr.getString('HELP');
                    line['CREATEDBY'] = getCreatedByName(dr.getString('CREATEDBY'));
                    //JID_1825 Date Showing as per browser culture
                    var da = (dr.getString('CREATED'));
                    var d = new Date(da);
                    CreatedOn = d.toLocaleDateString();

                    line['CREATED'] = CreatedOn;
                    line['AD_ARCHIVE_ID'] = dr.getInt('AD_ARCHIVE_ID');
                    //line['AD_PROCESS_ID'] = dr.getInt('AD_PROCESS_ID');
                    //line['AD_TABLE_ID'] = dr.getInt('AD_TABLE_ID');
                    //line['C_BPARTNER_ID'] = dr.getInt('C_BPARTNER_ID');
                    //line['RECORD_ID'] = dr.getInt('RECORD_ID');
                    //line['EXPORT_ID'] = dr.getInt('EXPORT_ID');
                    line['recid'] = count;
                    count++;
                    data.push(line);
                }
                dr.close();
            }
            catch (e) {
            }
            initGrid(data);
            return data;
        }

        function clear() {
            txtName.getControl().val("");
            txtDescription.getControl().val("");
            txtComment.getControl().val("");
            txtCreatedBy.getControl().val("");
            dtpCreatedOn.val(null);
        }

        function events() {

            if (btnOk != null)
                btnOk.on(VIS.Events.onTouchStartOrClick, function () {
                    $busyDiv[0].style.visibility = 'visible';
                    cmdQuery();
                    clear();
                    $busyDiv[0].style.visibility = 'hidden';
                });

            if (btnSave != null)
                btnSave.on(VIS.Events.onTouchStartOrClick, function () {
                    $busyDiv[0].style.visibility = 'visible';
                    cmdUpdateArchive();
                    $busyDiv[0].style.visibility = 'hidden';
                });

            if (btnNext != null)
                btnNext.on(VIS.Events.onTouchStartOrClick, function () {
                    alert('btnNext');
                });

            if (btnPre != null)
                btnPre.on(VIS.Events.onTouchStartOrClick, function () {
                    alert('btnPre');
                });

            if (this.btnToggal != null)
                var borderspace = 0;
                this.btnToggal.on(VIS.Events.onTouchStartOrClick, function () {
                    if (toggleside) {
                        if (VIS.Application.isRTL) {
                            borderspace = 180;
                        }
                        else {
                            borderspace = 0;

                        }
                        btnToggal.animate({ borderSpacing: borderspace }, {
                            step: function (now, fx) {
                                $(this).css('-webkit-transform', 'rotate(' + now + 'deg)');
                                $(this).css('-moz-transform', 'rotate(' + now + 'deg)');
                                $(this).css('transform', 'rotate(' + now + 'deg)');
                            },
                            duration: 'slow'
                        }, 'linear');

                        toggleside = false;
                        topLeftDiv.animate({ width: leftDivWidth }, "slow");
                        topleftparaDiv.animate({ width: leftDivWidth }, "slow");
                        topleftparaDiv.find("table").css("display", "block");
                        //topleftparaDiv.css("background-color", "transparent");
                        btnOk.css("display", "block");
                        topRightDiv.animate({ width: selectLeftDivWidth }, "slow", null, function () {
                            dGrid.resize();
                        });
                    }
                    else {
                        if (VIS.Application.isRTL) {
                            borderspace = 0;
                        }
                        else {
                            borderspace = 180;

                        }
                        btnToggal.animate({ borderSpacing: borderspace }, {
                            step: function (now, fx) {
                                $(this).css('-webkit-transform', 'rotate(' + now + 'deg)');
                                $(this).css('-moz-transform', 'rotate(' + now + 'deg)');
                                $(this).css('transform', 'rotate(' + now + 'deg)');
                            },
                            duration: 'slow'
                        }, 'linear');

                        toggleside = true;
                        topLeftDiv.animate({ width: minSideWidth }, "slow");
                        topleftparaDiv.animate({ width: minSideWidth }, "slow");
                        topleftparaDiv.find("table").css("display", "none");
                        //topleftparaDiv.css("background-color", "#F1F1F1");
                        btnOk.css("display", "none");
                        topRightDiv.animate({ width: selectDivFullWidth }, "slow", null, function () {
                            dGrid.resize();
                        });
                    }
                });

            chkReportQ.change(function () {
                cmbProcess.getControl().hide();
                lblProcessQ.getControl().hide();
                $(".VIS-Search-hide").show();
                if (chkReportQ.find('input').prop("checked")) {
                    cmbProcess.getControl().show();
                    lblProcessQ.getControl().show();
                    $(".VIS-Search-hide").hide();
                }
            });
        }

        //size chnage 
        this.sizeChanged = function (h, w) {
            selectLeftDivWidth = w - (leftDivWidth + 1);
            selectDivFullWidth = w - (minSideWidth + 1);
            if (toggleside == true) {
                topLeftDiv.animate({ width: minSideWidth }, "slow");
                topleftparaDiv.animate({ width: minSideWidth }, "slow");
                topleftparaDiv.find("table").css("display", "none");
                //topleftparaDiv.css("background-color", "#F1F1F1");
                btnOk.css("display", "none");
                topRightDiv.animate({ width: selectDivFullWidth }, "slow", null, function () {
                    dGrid.resize();
                });
            }
            else {
                topLeftDiv.animate({ width: leftDivWidth }, "slow");
                topleftparaDiv.animate({ width: leftDivWidth }, "slow");
                topleftparaDiv.find("table").css("display", "block");
                //topleftparaDiv.css("background-color", "transparent");
                btnOk.css("display", "block");
                topRightDiv.animate({ width: selectLeftDivWidth }, "slow", null, function () {
                    dGrid.resize();
                });
            }
        }

        this.display = function () {
            initGrid();
        }

        this.load = function () {
            initializeComponent();
            jbInit();
            dynInit();
            events();
        }

        this.getRoot = function () {
            return $root;
        };

        this.disposeComponent = function () {
            $root = null;
        };

    };

    //Must Implement with same parameter
    ArchiveViewer.prototype.sizeChanged = function (height, width) {
        //this.sizeChanged(height, width);
    };


    //Must Implement with same parameter
    ArchiveViewer.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        this.windowNo = windowNo;
        this.load();
        this.frame.getContentGrid().append(this.getRoot());
        this.display();

    };

    //Must implement dispose
    ArchiveViewer.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };

    VIS.Apps.AForms.ArchiveViewer = ArchiveViewer;

})(VIS, jQuery);
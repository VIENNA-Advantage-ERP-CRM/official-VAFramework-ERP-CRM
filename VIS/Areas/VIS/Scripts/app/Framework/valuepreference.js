; (function (VIS, $) {
    //form declaretion
    function ValuePreference(mField, val, dispValue) {
        if (mField == null) {
            return null;
        }

        var $self = this;
        var $root = $("<div class='vis-forms-container' style='position:relative;'>");
        var $busyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');

        this.Okbtn = null;
        this.cancelbtn = null;
        this.deletebtn = null;

        this.txtAttribute = null;
        this.lblAttribute = null;
        this.txtSearchKey = null;
        this.lblValue = null;
        this.lblAttribute = null;
        this.lblValueText = null;
        this.lblLevelText = null;
        this.chkTenant = null;
        this.chkOrg = null;
        this.chkUser = null;
        this.chkWindow = null;
        this.lblMsg = null;


        this.userchk = null;
        this.tenantchk = null;
        this.windowchk = null;
        this.orgchk = null;


        this.aValue = null;
        this.aDisplayValue = null;
        // if value exists
        if (val != null) {
            this.aValue = val;
            if (typeof val == Boolean) {
                this.aValue = val ? "Y" : "N";
            }
            // set display value
            this.aDisplayValue = (dispValue == null) ? this.aValue : dispValue;
        }

        this.AD_Window_ID = mField.getAD_Window_ID();
        this.aAttribute = mField.getColumnName();
        this.aDisplayAttribute = mField.getHeader();
        this.aDisplayType = mField.getDisplayType();
        this.AD_Reference_ID = 0;
        this.windowNum = mField.getWindowNo();

        if ("Value".equals(this.aAttribute) || "DocumentNo".equals(this.aAttribute)) {
            // No Preference for "Value" or "DocumentNo" attributes
            return null;
        }

        this.AD_Client_ID = VIS.context.getAD_Client_ID();
        this.AD_Org_ID = VIS.context.getWindowContext(this.windowNum, "AD_Org_ID", true);
        this.AD_User_ID = VIS.context.getAD_User_ID();
        this.role = VIS.MRole.getDefault();

        function setBusy(isBusy) {
            $busyDiv.css("display", isBusy ? 'block' : 'none');
        };
        /*  
        Design Removed from server side and created on client Side.
        */
        function createDesign($self) {
            var html = '<div class="vis-valpreferencedatawrp" id="fisrt_' + $self.windowNum + '">' +
                '<div class="input-group vis-input-wrap" style="flex: 1;">' + 
                '<div class="vis-control-wrap">' +
    '<input class="VIS_Pref_pass vis-gc-vpanel-table-readOnly" data-placeholder="" placeholder=" " style="margin-bottom: 0;" type="text" readonly="true" id="vtxtAttribute_' + $self.windowNum + '" />' +
    '<label class=" VIS_Pref_Label_Font" id="vlblAttributeText_' + $self.windowNum + '">test</label>' +
     '</div>' +
     '</div>' +
    '<label class="VIS_Pref_Label_Font" id="vlblAttribute_' + $self.windowNum + '">test</label>' +
'</div><div class="vis-valpreferencedatawrp" id="second_' + $self.windowNum + '">' +

                '<div class="input-group vis-input-wrap" style="flex: 1;">' +
                '<div class="vis-control-wrap">' +
    '<input class="VIS_Pref_pass vis-gc-vpanel-table-readOnly" data-placeholder="" placeholder=" " style="margin-bottom: 0;" type="text" readonly="true" id="vtxtSearchKey_' + $self.windowNum + '" />' +
    '<label class="VIS_Pref_Label_Font" id="vlblValueText_' + $self.windowNum + '">test</label>' +
                '</div>' +
                '</div>' +
                '<label class="VIS_Pref_Label_Font" id="vlblValue_' + $self.windowNum + '">test</label>' +
        '</div>' +
     '</div>' +
                '</div><div style="width: 100%; float: left">    ' +
                //'<label style="float: left; width:15%;text-align:left" class=" VIS_Pref_Label_Font" id="vlblLevelText_' + $self.windowNum + '">test</label>' +
    '<div> <div class="VIS_Pref_col-50"><input  type="checkbox" id="vchkTenant_' + $self.windowNum + '" ><label class="VIS_Pref_Label_Font">test</label>' +
        '</div>   <div class="VIS_Pref_col-50"> <input  type="checkbox" id="vchkOrg_' + $self.windowNum + '" /><label class="VIS_Pref_Label_Font">test</label>' +
        '</div> <div class="VIS_Pref_col-50">' +
            '<input  type="checkbox" id="vchkUser_' + $self.windowNum + '" /><label class="VIS_Pref_Label_Font">test</label>' +
        '</div>  <div class="VIS_Pref_col-50"> <input  type="checkbox" id="vchkWindow_' + $self.windowNum + '" /><label class="VIS_Pref_Label_Font">test</label>' +
        '</div> </div><div id="divMsg_' + $self.windowNum + '" style="width: 100%; float: left; margin-bottom: 5px; margin-top: 5px; text-align: left">' +
    '<label class="VIS_Pref_Label_Font" id="vlblMsg_' + $self.windowNum + '">test</label>' +
'</div><div style="height: 10%; width: 100%; float: left"> <button type="button" id="btnDelete_' + $self.windowNum + '"  class="VIS_Pref_btn-2"  style="float: left" value="Ok" role="button" aria-disabled="false">' +
        '<i class="vis vis-delete"></i>' +
    '</button> <input id= "btnCancel_' + $self.windowNum + '" class="VIS_Pref_btn-2" style="float:right" type="button" value="Cancel" />' +
    '<input id="btnOK_' + $self.windowNum + '"  class="VIS_Pref_btn-2" style="float:right; margin-right:10px" type="button" value="Ok" />' +
    '</div>';

            $root.append(html);
        };

        this.load = function () {
            //$root.load(VIS.Application.contextUrl + 'ValuePreference/Index/?windowno=' + this.windowNum, function (event) {

            createDesign($self);
                $root.append($busyDiv);
                setBusy(true);
                $self.init($root);
                setBusy(false);
            //});
        };

        this.init = function (root) {
            this.lblAttributeText = $root.find("#vlblAttributeText_" + this.windowNum);
            this.lblValueText = $root.find("#vlblValueText_" + this.windowNum);
            this.lblLevelText = $root.find("#vlblLevelText_" + this.windowNum);
            this.deletebtn = $root.find("#btnDelete_" + this.windowNum);
            this.txtAttribute = $root.find("#vtxtAttribute_" + this.windowNum);
            this.txtSearchKey = $root.find("#vtxtSearchKey_" + this.windowNum);
            this.chkTenant = $root.find("#vchkTenant_" + this.windowNum);
            this.chkOrg = $root.find("#vchkOrg_" + this.windowNum);
            this.chkUser = $root.find("#vchkUser_" + this.windowNum);
            this.chkWindow = $root.find("#vchkWindow_" + this.windowNum);
            this.lblMsg = $root.find("#vlblMsg_" + this.windowNum);
            this.lblAttribute = $root.find("#vlblAttribute_" + this.windowNum);
            this.lblValue = $root.find("#vlblValue_" + this.windowNum);
            this.Okbtn = $root.find("#btnOK_" + this.windowNum);
            this.cancelbtn = $root.find("#btnCancel_" + this.windowNum);

            if (VIS.Application.isRTL) {
                this.Okbtn.css("margin-right", "-128px");
                this.cancelbtn.css("margin-right", "55px");
                this.lblAttributeText.css("text-align", "right");
                this.lblValueText.css("text-align", "right");
                this.lblLevelText.css("text-align", "right");
                this.chkOrg.css("margin-right", "0px");
                this.chkTenant.css("margin-right", "37px");
                //this.chkUser.css("margin-right", "72px");
                //this.chkWindow.css("margin-right", "72px");
                $root.find("#divMsg_" + this.windowNum).css("text-align", "right");
            }
            else {
                //this.chkUser.css("margin-left", "83px");
                //this.chkWindow.css("margin-left", "45px");
            }

            // display values in label and textbox
            this.txtAttribute.val(this.aDisplayAttribute == null ? "" : this.aDisplayAttribute);
            this.lblAttribute.text(this.aAttribute == null ? "" : this.aAttribute);
            this.txtSearchKey.val(this.aDisplayValue == null ? "" : VIS.Utility.decodeText(this.aDisplayValue));
            this.lblValue.text(this.aValue);

            this.lblAttributeText.text(VIS.Msg.getMsg("Attribute"));
            this.lblValueText.text(VIS.Msg.getMsg("SearchKey"));
            this.lblLevelText.text(VIS.Msg.getMsg("ValuePreferenceSetFor"));
            this.chkTenant.next("label").text(VIS.Msg.getMsg("Tenant"));
            this.chkOrg.next("label").text(VIS.Msg.getMsg("Org"));
            this.chkUser.next("label").text(VIS.Msg.getMsg("UserContact"));
            this.chkWindow.next("label").text(VIS.Msg.getMsg("Window"));

            this.chkTenant.prop("disabled", true);
            this.chkTenant.prop("checked", true);
            this.chkUser.prop("checked", true);
            this.chkWindow.prop("checked", true);


            //	Can Change Org
            if (!VIS.MRole.PREFERENCETYPE_Client.equals(VIS.MRole.getPreferenceType())) {
                this.chkOrg.prop("disabled", true);
                this.chkOrg.prop("checked", true);
            }

            //	Can Change User
            if (!(VIS.MRole.PREFERENCETYPE_Client.equals(VIS.MRole.getPreferenceType())
                || VIS.MRole.PREFERENCETYPE_Organization.equals(VIS.MRole.getPreferenceType()))) {
                this.chkUser.prop("disabled", true);
                this.chkUser.prop("checked", true);
            }

            this.setExplanation();


            this.Okbtn.on("click", function () {
                setBusy(true);
                $self.save();
                setBusy(false);
                $root.dialog('close');
            });

            this.cancelbtn.on("click", function () {
                $root.dialog('close');
            });

            this.deletebtn.on("click", function () {
                $self.delete();
                $root.dialog('close');
            });

            $('[id^=vch]').click(function () {
                $self.setExplanation();
            });

        };

        this.showDialog = function () {
            $root.append($busyDiv);
            setBusy(true);
            setTimeout(
            this.load(), 5);
            $root.dialog({
                modal: true,
                title: VIS.Msg.getMsg("ValuePreference"),
                //height: 280,
                width: 530,
                position: { at: "center top", of: window },
                close: function () {
                    $self.dispose();
                    $root.dialog("destroy");
                    $root = null;
                }
            });
        };

        this.disposeComponent = function () {
            if (this.Okbtn)
                this.Okbtn.off("click");
            if (this.cancelbtn)
                this.cancelbtn.off("click");
            if (this.deletebtn)
                this.deletebtn.off("click");
            if ($('[id^=vch]'))
                $('[id^=vch]').off("click");

            this.Okbtn = null;
            this.cancelbtn = null;
            this.deletebtn = null;
            $busyDiv = null;
            $self = null;
            this.disposeComponent = null;
        };
    };

    ValuePreference.prototype.setExplanation = function () {
        var expl = "For ";
        if (this.chkTenant.prop("checked") && this.chkOrg.prop("checked")) {
            expl += "this Client and Organization";
        }
        else if (this.chkTenant.prop("checked") && !this.chkOrg.prop("checked")) {
            expl += "all Organizations of this Client";
        }
        else if (!this.chkTenant.prop("checked") && this.chkOrg.prop("checked")) {
            this.chkOrg.prop("checked", false);
            expl += "entire System";
        }
        else {
            expl += "entire System";
        }

        // set text according to user
        if (this.chkUser.prop("checked")) {
            expl += ", this User";
        }
        else {
            expl += ", all Users";
        }

        // set text according to window
        if (this.chkWindow.prop("checked")) {
            expl += " and this Window";
        }
        else {
            expl += " and all Windows";
        }

        if (VIS.Env.isBaseLanguage()) {
            this.lblMsg.text(expl.toString());
        }

        this.orgchk = this.chkOrg.prop("checked");
        this.userchk = this.chkUser.prop("checked");
        this.windowchk = this.chkWindow.prop("checked");
        this.tenantchk = this.chkTenant.prop("checked");
    };

    ValuePreference.prototype.getADPreferenceID = function () {
        // make sql query to get preference id
        var sql = "SELECT AD_Preference_ID FROM AD_Preference WHERE ";
        var valuetem = this.tenantchk ? this.AD_Client_ID : 0;
        //	Client
        sql += "AD_Client_ID=" + valuetem;
        //	Org
        valuetem = this.orgchk ? this.AD_Org_ID : 0;
        sql += " AND AD_Org_ID=" + valuetem;
        //	Optional User
        if (this.userchk) {
            sql += " AND AD_User_ID=" + this.AD_User_ID;
        }
        else {
            sql += " AND AD_User_ID IS NULL";
        }
        //	Optional Window
        if (this.windowchk) {
            sql += " AND AD_Window_ID=" + this.AD_Window_ID;
        }
        else {
            sql += " AND AD_Window_ID IS NULL";
        }
        //	Attribute (Key)
        sql += " AND Attribute='" + this.aAttribute + "'";

        var preferenceId = 0;

       
        var ds = VIS.DB.executeDataSet(sql.toString());

        if (ds != null && ds.tables[0].rows.length > 0) {
            // get id
            preferenceId = ds.tables[0].rows[0].getCell("ad_preference_id");
        }
        if (preferenceId < 0) {
            preferenceId = 0;
        }
        return preferenceId;
    };

    ValuePreference.prototype.delete = function () {
        var returnValue = false;
        // get preference id
        var AD_Preference_ID = this.getADPreferenceID();
        var localObj = this;
        if (AD_Preference_ID > 0) {
            $.ajax({
                url: VIS.Application.contextUrl + "ValuePreference/Delete",
                dataType: "json",
                data: {
                    preferenceId: AD_Preference_ID
                },
                success: function (data) {
                    returnValue = data.result;
                    if (returnValue) {
                        VIS.Env.getCtx().setContext(localObj.getContextKey(), null);
                        VIS.ADialog.info("ValuePreferenceDeleted", true, "", null);
                    }
                }
            });
        }
        else {
            VIS.ADialog.warn("ValuePreferenceNotFound", true, "", null);
        }
        return returnValue;
    };

    ValuePreference.prototype.save = function () {
        //	Handle NULL
        if (this.aValue == null) {
            if (VIS.DisplayType.IsLookup(this.displayType)) {
                this.aValue = "-1";	//	 -1 may cause problems (BPartner - M_DiscountSchema
            }
            else if (VIS.DisplayType.IsDate(this.displayType)) {
                this.aValue = " ";
            }
            else {
                VIS.ADialog.warn("ValuePreferenceNotInserted", true, "", null);
                return false;
            }
        }
            //	No Variables (SQL)
        else if (this.aValue.toString().indexOf("@") != -1) {
            ////ShowMessage.Warn("ValuePreferenceNotInserted", true, "", null);
            return false;
        }

        var returnValue = false;

        this.orgchk = this.chkOrg.prop("checked");
        this.userchk = this.chkUser.prop("checked");
        this.windowchk = this.chkWindow.prop("checked");
        this.tenantchk = this.chkTenant.prop("checked");

        var localObj = this;

        var AD_Preference_ID = this.getADPreferenceID();

       
        $.ajax({
            url: VIS.Application.contextUrl + "ValuePreference/Save",
            dataType: "json",
            data: {
                preferenceId: AD_Preference_ID,
                clientId: this.tenantchk ? this.AD_Client_ID : 0,
                orgId: this.orgchk ? this.AD_Org_ID : 0,
                chkWindow: this.windowchk,
                AD_Window_ID: this.AD_Window_ID,
                chkUser: this.userchk,
                attribute: this.aAttribute,
                userId: this.AD_User_ID,
                value: this.aValue
            },
            success: function (data) {
                returnValue = data.result;
                if (returnValue) {
                    VIS.Env.getCtx().setContext(localObj.getContextKey(), localObj.aValue);
                    VIS.ADialog.info("ValuePreferenceInserted", true, "", null);
                }
                else {
                    VIS.ADialog.warn("ValuePreferenceNotInserted", true, "", null);
                }
            }
        });
        return returnValue;
    };

    ValuePreference.prototype.getContextKey = function () {
        if (this.windowchk) {
            return "P" + this.AD_Window_ID + "|" + this.aAttribute;
        }
        else {
            return "P|" + this.aAttribute;
        }
    };

    ValuePreference.prototype.dispose = function () {
        this.disposeComponent();
    };


    //Load form into VIS
    VIS.ValuePreference = ValuePreference;

})(VIS, jQuery);
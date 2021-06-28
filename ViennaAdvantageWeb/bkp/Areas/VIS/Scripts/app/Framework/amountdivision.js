; (function (VIS, $) {
	function AmountDivision(C_DimAmt_ID, AD_Org_ID, defaultVal, isReadOnly) {
		//  AD_TableId = 0;
		//  AD_RecordID = 0;
		// C_DimAmt_ID = 0;
		//  AD_Org_ID = 0;
		if (C_DimAmt_ID && C_DimAmt_ID.toString().indexOf("<") > -1) {
			C_DimAmt_ID = 0;
		}
		this.onClose = null;
		this.onClosing = null;
		var root = $("<div style='width:100%;height:100%;position:relative;display: flex;'>");
		var contentDiv = $("<div class='VIS-AMTD-MainContent vis-formouterwrpdiv'>");
		var bsyDiv = $('<div class="vis-busyindicatorouterwrap"><div class="vis-busyindicatorinnerwrap"><i class="vis-busyindicatordiv"></i></div></div>');
		var tblDesing = $("<table>");
		var cmbAcctSchema, cmbDimensionType, txtTotalAmount, cmbOrg, txtAmount, btnAdd, btnOk, btnNew, grdDimTypeLine, divDesign, lblAcctSchema, lblDimensionType, lblTotalAmount, lblOrg, lblAmount, txtTotal, lblTotal;
		var lblActivity, cmbActivity, lblCampaign, cmbCampaign, lblSalesRegion, cmbSalesRegion, lblUserElement, cmbUserElement, lblBPartner, cmbBPartner, lblAddress, locAddress, lblProject, txtProject, lblProduct, txtProduct;
		var lblElement, lblAccountElement, cmbElement, txtAccountElement, lblAcctElementValue, txtAcctElementValue;
		var PAGE_SIZE = null;
		var pageNoOrder = null;
		var grdname = null;
		var dGrid = null;
		var datasec = null;
		var generateControl = null;
		var storeDimensionData = null;
		var windowNo = VIS.Env.getWindowNo();
		var recid = null;
		var checkValUpdate = null;
		var updateRecordId = null;
		var indexValue = null;
		var checkDelete = null;
		var txtb = null;
		var txtLoc = null;
		var txtProj = null;
		var txtProd = "";
		var modalTxtb = null;
		var modalTxtLoc = null;
		var modalTxtProj = null;
		var modalTxtProd = null;
		var arrAcctSchemaID = null;
		var arrDimensionEType = null;
		var ch = new VIS.ChildDialog();
		//AD_Org_ID = 0;
		var ctrlDiv = null;
		var divOrg = null;
		var divAccountElement = null;
		var divAccountElementVal = null;
		var divActivity = null;
		var divBPartner = null;
		var divLocation = null;
		var divCampaign = null;
		var divProduct = null;
		var divProject = null;
		var divSales = null;
		var divUserElement = null;
		var divAmount = null;
		var gridBtnAdd = null;
		var gridBtnDelete = null;
		var demoCount = null;
		var allAcctSchemaID = null;
		var self = this;
		var divbutton = null;
		var modalDivButton = null;
		var modalDiv = null;
		var modalDivConent = null;
		var modalSpanClose = null;
		var divGrid = null;
		var modalDivOrg = null;
		var modalDivAccountElement = null;
		var modalDivAccountElementVal = null;
		var modalDivActivity = null;
		var modalDivBPartner = null;
		var modalDivLocation = null;
		var modalDivCampaign = null;
		var modalDivProduct = null;
		var modalDivProject = null;
		var modalDivSales = null;
		var modalDivUserElement = null;
		var modalDivAmount = null;
		var modalGenerateControl = null;
		var modalCmbOrg, modalTxtAmount, modalBtnAdd, modalBtnNew, modalLblOrg, modalLblAmount;
		var modalLblActivity, modalCmbActivity, modalLblCampaign, modalCmbCampaign, modalLblSalesRegion, modalCmbSalesRegion, modalLblUserElement, modalCmbUserElement, modalLblBPartner, modalCmbBPartner, modalLblAddress, modalLocAddress, modalLblProject, modalTxtProject, modalLblProduct, modalTxtProduct;
		var modalLblElement, modalLblAccountElement, modalCmbElement, modalTxtAccountElement, modalLblAcctElementValue, modalTxtAcctElementValue;
		var divStatusContainer = null;
		var ulStatusdimension = null;
		var liStatusFirst = null;
		var liStatusPrev = null;
		var cmbStausRecordCount = null;
		var liStatusNext = null;
		var liStatusLast = null;
		var refreshCmbDimension = null;
		var LineGridName = null;
		var dimensionLineID = null;
		var oldAmount = null;
		var lblMaxAmount = null;
		var divMaxAmount = null;
		var format = null;
		this.dotFormatter = VIS.Env.isDecimalPoint();
		var Precision = VIS.Env.getCtx().getContextAsInt("#StdPrecision");
		var Amount = null;
		var cmbValue = null;
		var acctValue = null;
		var oldDimensionNameValue = null;
		var totalRecords = 0;
		var C_Element_ID = 0;
		var IsElementOk = true;
		var oldBPartnerID = 0;
		var $bsyDiv = null;

		var fileUpldIconDim = null;
		var count = 0;


		function initializeComponent() {
			lblAcctSchema = $("<label>");
			lblDimensionType = $("<label>");
			lblTotalAmount = $("<label>");
			lblAmount = $("<label>");
			modalLblAmount = $("<label>");
			lblTotal = $("<label>");
			lblMaxAmount = $("<label>");
			cmbAcctSchema = $("<select tabindex='2'>");
			cmbDimensionType = $("<select tabindex='3'>");
			cmbElement = $("<select>");

			txtAmount = new VIS.Controls.VAmountTextBox("Amount", false, false, true, 50, 100, VIS.DisplayType.Amount, VIS.Msg.getMsg("Amount")); //$("<input type='number' min='0' tabindex='6'>");
			modalTxtAmount = new VIS.Controls.VAmountTextBox("ModalAmount", false, false, true, 50, 100, VIS.DisplayType.Amount, VIS.Msg.getMsg("Amount")); //$("<input type='number' min='0' tabindex='11'>");

			//if (isReadOnly) {
			//    txtTotalAmount = $("<input type='number' min='0' tabindex='1' readonly>");
			//}
			//else {
			txtTotalAmount = new VIS.Controls.VAmountTextBox("TotalAmount", false, true, true, 50, 100, VIS.DisplayType.Amount, VIS.Msg.getMsg("Amount"));  //$("<input type='number' min='0' tabindex='1' readonly>");
			//}

			txtTotal = new VIS.Controls.VAmountTextBox("Total", false, true, true, 50, 100, VIS.DisplayType.Amount, VIS.Msg.getMsg("Amount"));     //$("<input type='text' readonly='true' tabindex='15'>");
			btnAdd = $("<a style='cursor:pointer;' tabindex='7'>");
			btnNew = $("<a>");
			modalBtnAdd = $("<a style='cursor:pointer;' tabindex='12'>");
			modalBtnNew = $("<a style='cursor:pointer;' tabindex='13'>");
			divGrid = $("<div class='VIS-AMTD-Grid'>");
			datasec = $("<div class='VIS-AMTD-SubGrid'>");
			generateControl = $("<div class='VIS-AMTD-formDynmicInput'>");
			modalGenerateControl = $("<div class='VIS-AMTD-formDynmicInput'>");
			gridBtnAdd = $("<button>");
			gridBtnDelete = $("<button>");
			modalDiv = $("<div class='VIS-AMTD-Modal' id='updateModal'>");
			modalDivConent = $("<div class='VIS-AMTD-Modal-Content'>");
			modalSpanClose = $("<span class='VIS-AMTD-Close'>");
			statusSpanShowRecordNo = $("<label>");
			statusSpanShowRecordNo.addClass("vis-statusbar-pageMsg");
			modalSpanClose.append("X");
			ulStatusdimension = $('<ul class="vis-statusbar-ul"></ul>');
			divStatusContainer = $('<div class="VIS-AMTD-StatusContainer"></div>');

			liStatusFirst = $('<li  style="opacity: 0.6;"><div><i class="vis vis-shiftleft" style="opacity: 1;" action="first" title="First Page" ></i></div></li>');
			ulStatusdimension.append(liStatusFirst);

			liStatusPrev = $('<li style="opacity: 0.6;"><div><i class="vis vis-pageup" style="opacity: 1;" action="prev" title="Page Down" ></i></div></li>');
			ulStatusdimension.append(liStatusPrev);

			cmbStausRecordCount = $('<select class="vis-statusbar-combo" style="width:50px;"></select>');
			var li = $('<li></li>');
			li.append(cmbStausRecordCount);
			ulStatusdimension.append(li);

			liStatusNext = $('<li style="opacity: 1;"><div><i class="vis vis-pagedown" style="opacity: 1;" action="next" title="Page Up" alt="Page Down" src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/PageDown16.png"></div></li>');
			ulStatusdimension.append(liStatusNext);

			liStatusLast = $('<li style="opacity: 1;"><div><i class="vis vis-shiftright" style="opacity: 1;" action="last" title="Last Page" ></i></div></li>');
			ulStatusdimension.append(liStatusLast);

			divStatusContainer.append(ulStatusdimension);
			storeDimensionData = new Array();
			recid = 0;
			checkValUpdate = false;
			updateRecordId = 0;
			indexValue = 0;
			checkDelete = "";
			txtb = "";
			txtLoc = "";
			txtProj = "";
			txtProd = "";
			modalTxtb = "";
			modalTxtLoc = "";
			modalTxtProj = "";
			modalTxtProd = "";
			allAcctSchemaID = "";
			LineGridName = "";
			demoCount = 0;
			allAcctSchemaID = [];
			arrAcctSchemaID = [];
			oldAmount = 0;
			refreshCmbDimension = true;
			dimensionLineID = 0;
			PAGE_SIZE = 30;
			pageNoOrder = 1;
			Amount = 0;
			cmbValue = 0;
			acctValue = -1;
			oldDimensionNameValue = 0;
			format = VIS.DisplayType.GetNumberFormat(VIS.DisplayType.Amount);
			ctrlDiv = $("<div class='VIS-AMTD-formWrap'>");
			//C_DimAmt_ID = 0;//  1000015;//1000002;   //Initialize Record ID for Testing Record.........................
			//Amount = 5000;
			btnOk = $("<input type='button' class='VIS_Pref_btn-2 pull-left' tabindex='16' >");
			btnAdd.append(VIS.Msg.getMsg("Add"));
			//gridBtnAdd.append(addTxt);
			btnNew.append(VIS.Msg.getMsg("Cancel"));
			modalBtnAdd.append(VIS.Msg.getMsg("Update"));
			modalBtnNew.append(VIS.Msg.getMsg("Cancel"));
			btnOk.val(VIS.Msg.getMsg("OK"));


			lblAcctSchema.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_AcctSchema_ID"));

			lblDimensionType.append(VIS.Msg.translate(VIS.Env.getCtx(), "ElementType"));

			lblTotalAmount.append(VIS.Msg.getMsg("TotalAmount"));

			lblAmount.append(VIS.Msg.getMsg("Amount"));
			modalLblAmount.append(VIS.Msg.getMsg("Amount"));
			btnNew.css("display", "none");
			lblTotal.append(VIS.Msg.getMsg("Total"));
			var divAcctSchema = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			var divAcctSchemaCtrlWrp = $("<div class='vis-control-wrap'>");
			divAcctSchema.append(divAcctSchemaCtrlWrp);
			divAcctSchemaCtrlWrp.append(cmbAcctSchema).append(lblAcctSchema);
			var divDimType = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			var divDimTypeCtrlWrp = $("<div class='vis-control-wrap'>");
			var divAmountDimExcel = $('<div class="vis-excelimportbtn VIS-AMTD-formBtns" style=" display: block; "><a tabindex="" class="vis vis-doc-excel"></a></div>');
			divDimType.append(divDimTypeCtrlWrp);
			divDimType.append(divAmountDimExcel);
			divDimTypeCtrlWrp.append(cmbDimensionType).append(lblDimensionType);

			/* Create the element of input File */
			var divExcelDim = $('<div style="float: right;margin-right: 5px;display:none" ><input id="VIS-AMTD-file-input" style="display:none" type="file"  accept=".xls,.xlsx"><label title="'
				+ VIS.Msg.getMsg("VIS_ExcelImport") + '"  for="VIS-AMTD-file-input" ></label></div>');
			divDimType.append(divExcelDim);

			fileUpldIconDim = divDimType.find("#VIS-AMTD-file-input");

			var divTotalAmount = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			var divTotalAmountCtrlWrp = $("<div class='vis-control-wrap'>");
			divTotalAmount.append(divTotalAmountCtrlWrp);
			divTotalAmountCtrlWrp.append(txtTotalAmount.getControl().attr('placeholder', ' ').attr('data-placeholder', '')).append(lblTotalAmount);
			divAmount = $("<div class='VIS-AMTD-formData VIS-AMTD-amountTxt input-group vis-input-wrap'>");
			var divAmountCtrlWrp = $("<div class='vis-control-wrap'>");
			divAmount.append(divAmountCtrlWrp);
			divAmountCtrlWrp.append(txtAmount.getControl().attr('placeholder', ' ').attr('data-placeholder', '')).append(lblAmount);
			modalDivAmount = $("<div class='VIS-AMTD-formData VIS-AMTD-amountTxt input-group vis-input-wrap'>");
			var modalDivAmountCtrlWrp = $("<div class='vis-control-wrap'>");
			modalDivAmount.append(modalDivAmountCtrlWrp);
			modalDivAmount.append(modalTxtAmount.getControl().attr('placeholder', ' ').attr('data-placeholder', '')).append(modalLblAmount);
			divbutton = $("<div class='VIS-AMTD-formBtns'>");
			var divButton1 = $("<div>");
			divButton1.append(btnAdd);//.append(btnNew);
			divbutton.append(divButton1);
			modalDivButton = $("<div class='VIS-AMTD-ModalformBtns'>");
			var modalButton1 = $("<div>");
			modalButton1.append(modalBtnAdd).append(modalBtnNew);
			modalDivButton.append(modalButton1);
			var divTotal = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			var divTotalCtrlWrp = $("<div class='vis-control-wrap'>");
			modalDivConent.append(modalGenerateControl).append(modalDivAmount).append(modalDivButton);
			modalDiv.append(modalDivConent);
			divGrid.append(datasec).append(modalDiv).append(divStatusContainer);
			//datasec.append(modalDiv);
			divTotal.append(divTotalCtrlWrp);
			divTotalCtrlWrp.append(txtTotal.getControl().attr('placeholder', ' ').attr('data-placeholder', '')).append(lblTotal);
			divMaxAmount = $("<div style='color:green;font-size:small'>");
			divMaxAmount.append(lblMaxAmount);
			ctrlDiv.append(divMaxAmount).append(divTotalAmount).append(divAcctSchema).append(divDimType).append(generateControl).append(divAmount).append(divbutton);
			contentDiv.append(ctrlDiv).append(divGrid).append(divTotal).append(btnOk);
			root.append(contentDiv).append(bsyDiv);

			if (isReadOnly) {
				contentDiv.addClass('VIS-AMTD-ReadOnly');
			}

			//Accounting Schema Change function...............
			cmbAcctSchema.on("change", function () {
				busyDiv("visible");
				var tempCheck = true;
				if (cmbAcctSchema.val() == 0) {
					tempCheck = false;
					//This Check Work if Accounting Shema is "All" in this ..
					//Remove All Dimension against different Accounting Schema if exists.

					getDimensionLine(allAcctSchemaID, null, function (acctSchemaData) {

						if (acctSchemaData.length != 0) {
							VIS.ADialog.confirm("ConfirmSelectAllAcctSchema", true, "", "Confirm", function (result) {
								if (!result) {
									cmbAcctSchema.val(acctValue);
									cmbDimensionType.val(cmbValue);
									loadSchemaValue(false);
									//busyDiv("hidden");
								}
								else {
									busyDiv("visible");
									for (var j = 0; j < allAcctSchemaID.length; j++) {
										VIS.DB.executeQuery("delete from c_dimamtaccttype where c_acctschema_id=" + allAcctSchemaID[j] + " and c_dimamt_id=" + C_DimAmt_ID + "");
									}
									getMaxDimensionAmount();

									//loadData(getDimensionLine(allAcctSchemaID));

									getDimensionLine(allAcctSchemaID, null, function (vall) {
										busyDiv("visible");
										loadData(vall);



										acctValue = cmbAcctSchema.find("option:selected").val();
										recid = 0;
										txtAmount.setValue(0);
										checkValUpdate = false;
										btnNew.css("display", "none");
										//divbutton.css("width", "6%");
										if (cmbDimensionType.val() == "AC" || cmbDimensionType.val() == "U1" || cmbDimensionType.val() == "U2") {
											divAmount.css("width", "26%");
										}
										else { divAmount.css("width", "33.3%"); }
										btnAdd.empty().append(VIS.Msg.getMsg("Add"));
										txtTotal.setValue(0);
										loadSchemaValue(false);
										// busyDiv("hidden");
									});

								}

							});
						}

					});


				}
				getDimensionLine(allAcctSchemaID, null, function (acctSchemaData) {
					loadSchemaValue(tempCheck);
					busyDiv("hidden");
				});
				Precision = cmbAcctSchema.find('option:selected').attr('precision');
			});

			function loadSchemaValue(tempCheck) {
				if (tempCheck) {
					acctValue = cmbAcctSchema.find("option:selected").val();
				}
				pageNoOrder = 1;
				arrAcctSchemaID = [];
				if (cmbAcctSchema.val() != 0) {

					arrAcctSchemaID[0] = cmbAcctSchema.val();
				}
				else {

					arrAcctSchemaID = allAcctSchemaID;
				}
				getDimensionLine(arrAcctSchemaID, null, function (acctSchemaData) {
					loadData(acctSchemaData);
					var getAcctSchemaId = -1;
					getDiminsionType(cmbAcctSchema.val(), function () {
						if (acctSchemaData.length != 0) {
							cmbDimensionType.val(acctSchemaData[0].DimensionTypeVal);
							getControl(cmbDimensionType.val());

							txtAmount.setValue(0);
							checkValUpdate = false;
							btnAdd.empty().append(VIS.Msg.getMsg("Add"));
							if ((cmbAcctSchema.val() != -1) && (cmbDimensionType.val() != 0)) {
								divAmount.css("display", "block");
								btnAdd.css("display", "block");
								if (lblMaxAmount.html() == "") {
									ch.getRoot().parent().css("height", "448px");
								}
								else {
									ch.getRoot().parent().css("height", "469px");
								}
							}
							else {


								divAmount.css("display", "none");
								btnAdd.css("display", "none");
								if (lblMaxAmount.html() == "") {
									ch.getRoot().parent().css("height", "390px");
								}
								else {
									ch.getRoot().parent().css("height", "410px");
								}
							}
							//calculateGrossAmount(acctSchemaData[0].TotalLineAmount);
						}
						else {
							getControl(cmbDimensionType.val());
							if ((cmbAcctSchema.val() != -1) && (cmbDimensionType.val() != 0)) {
								divAmount.css("display", "block");
								btnAdd.css("display", "block");
								if (lblMaxAmount.html() == "") {
									ch.getRoot().parent().css("height", "448px");
								}
								else {
									ch.getRoot().parent().css("height", "469px");
								}
							}
							else {


								divAmount.css("display", "none");
								btnAdd.css("display", "none");
								if (lblMaxAmount.html() == "") {
									ch.getRoot().parent().css("height", "390px");
								}
								else {
									ch.getRoot().parent().css("height", "410px");
								}
							}
						}
					});
					//if (acctSchemaData.length != 0) {
					//    cmbDimensionType.val(acctSchemaData[0].DimensionTypeVal);
					//    //calculateGrossAmount(acctSchemaData[0].TotalLineAmount);
					//}

				});//Filter data against Accounting Schema.........


			};
			//DimensionType Change Funcion..............
			cmbDimensionType.on("change", function (e) {
				busyDiv("visible");
				getDimensionLine(arrAcctSchemaID, null, function (acctSchemaData) {
					if (acctSchemaData.length != 0) {

						VIS.ADialog.confirm("ConfirmChangeDimensionType", true, "", "Confirm", function (result) {
							if (!result) {
								cmbDimensionType.val(cmbValue);
								hideShowDimensionValue();
								busyDiv("hidden");
								return false;
							}
							else {
								busyDiv("visible");
								//Remove Dimension against old Dimension Type ............
								getControl(cmbDimensionType.val());
								cmbValue = cmbDimensionType.find("option:selected").val();
								//recid = 0;
								txtAmount.setValue(0);
								checkValUpdate = false;
								btnNew.css("display", "none");
								//divbutton.css("width", "6%");
								if (cmbDimensionType.val() == "AC" || cmbDimensionType.val() == "U1" || cmbDimensionType.val() == "U2") {
									divAmount.css("width", "26%");
								}
								else { divAmount.css("width", "33.3%"); }
								btnAdd.empty().append(VIS.Msg.getMsg("Add"));

								for (var j = 0; j < arrAcctSchemaID.length; j++) {
									VIS.DB.executeQuery("delete from c_dimamtaccttype where c_acctschema_id=" + arrAcctSchemaID[j] + " and c_dimamt_id=" + C_DimAmt_ID + "");
								}
								getDimensionLine(arrAcctSchemaID, null, function (temp) {

									loadData(temp);
									getMaxDimensionAmount();
									txtTotal.setValue(0);
								});

								busyDiv("hidden");
							}
							hideShowDimensionValue();
						});

					}
					else {
						getControl(cmbDimensionType.val());
						if (!IsElementOk) {
							busyDiv("hidden");
							VIS.ADialog.warn("VIS_ElementDifferent");
							return false;
						}
						cmbValue = cmbDimensionType.find("option:selected").val();
						if (cmbDimensionType.val() == "AC") {
							dGrid.showColumn("C_BPartner");
						}
						else {
							dGrid.hideColumn("C_BPartner");
						}
					}
					hideShowDimensionValue();
					cmbDimensionType.focus();
					busyDiv("hidden");
				});

			});
			btnAdd.on("keydown", function (e) {
				if (e.keyCode == 13) {
					busyDiv("visible");
					//Validate Control ...................
					if (validateControl()) {

						if (!checkValUpdate) { recid++; }
						var dimTypeVal = cmbDimensionType.find("option:selected").val();
						//Add Data against particular Dimension Type Value......................
						if (dimTypeVal == "AC") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), txtAccountElement.val(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), txtAcctElementValue.getValue(), C_Element_ID, 0, txtb.getValue(), cmbBPartner.val());
							cmbElement.val("-1");
							txtAcctElementValue.setValue("-1");
						}//Account
						else if (dimTypeVal == "AY") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), cmbActivity.find("option:selected").text(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), cmbActivity.find("option:selected").val(), -1, 0, 0, "");
							cmbActivity.val(-1);
						}//Activity
						else if (dimTypeVal == "BP") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), cmbBPartner.val(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), txtb.getValue(), -1, 0, 0, "");
							cmbBPartner.val("");
							txtb.setValue("-1");
						}//BPartner
						else if (dimTypeVal == "LF" || dimTypeVal == "LT") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), locAddress.val(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), txtLoc.getValue(), -1, 0, 0, "");
							locAddress.val("");
							txtLoc.setValue("-1");
						}//Location From//Location To
						else if (dimTypeVal == "MC") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), cmbCampaign.find("option:selected").text(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), cmbCampaign.find("option:selected").val(), -1, 0, 0, "");
							cmbCampaign.val(-1);
						}//Campaign
						else if (dimTypeVal == "OO" || dimTypeVal == "OT") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), cmbOrg.find("option:selected").text(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), cmbOrg.find("option:selected").val(), -1, 0, 0, "");
							cmbOrg.val("-1");
						}//Organization//Org Trx
						else if (dimTypeVal == "PJ") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), txtProject.val(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), txtProj.getValue(), -1, 0, 0, "");
							txtProject.val("");
							txtProj.setValue("-1");
						}//Project
						else if (dimTypeVal == "PR") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), txtProduct.val(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), txtProd.getValue(), -1, 0, 0, "");
							txtProduct.val("");
							txtProd.setValue("-1");
						}//Product
						else if (dimTypeVal == "SA") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), "", txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), "", -1, 0, 0, "");
						}//Sub Account
						else if (dimTypeVal == "SR") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), cmbSalesRegion.find("option:selected").text(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), cmbSalesRegion.find("option:selected").val(), -1, 0, 0, "");
							cmbSalesRegion.val("-1");
						}//Sales Region
						else if (dimTypeVal == "U1" || dimTypeVal == "U2") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), txtAccountElement.val(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), txtAcctElementValue.getValue(), C_Element_ID, 0, txtb.getValue(), cmbBPartner.val());
							cmbElement.val("-1");
							txtAccountElement.val("");
							txtAcctElementValue.setValue("-1");
						}//User List 1//User List 2
						else if (dimTypeVal == "X1" || dimTypeVal == "X2" || dimTypeVal == "X3" || dimTypeVal == "X4" || dimTypeVal == "X5" || dimTypeVal == "X6" ||
							dimTypeVal == "X7" || dimTypeVal == "X8" || dimTypeVal == "X9") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), cmbUserElement.find("option:selected").text(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), cmbUserElement.find("option:selected").val(), -1, 0, 0, "");
							cmbUserElement.val(-1);
						}//User Element 1 to User Element 9

					}

				}
			});
			//Add and Update Button Click........
			btnAdd.on("click", function () {
				busyDiv("visible");
				//Validate Control ...................
				if (validateControl()) {

					if (!checkValUpdate) { recid++; }
					var dimTypeVal = cmbDimensionType.find("option:selected").val();
					//Add Data against particular Dimension Type Value......................
					if (dimTypeVal == "AC") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), txtAccountElement.val(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), txtAcctElementValue.getValue(), C_Element_ID, 0, txtb.getValue(), cmbBPartner.val());
						cmbElement.val("-1");
						txtAcctElementValue.setValue("-1");
						cmbBPartner.val("");
						txtb.setValue("-1");
					}//Account
					else if (dimTypeVal == "AY") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), cmbActivity.find("option:selected").text(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), cmbActivity.find("option:selected").val(), -1, 0, 0, "");
						cmbActivity.val(-1);
					}//Activity
					else if (dimTypeVal == "BP") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), cmbBPartner.val(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), txtb.getValue(), -1, 0, 0, "");
						cmbBPartner.val("");
						txtb.setValue("-1");
					}//BPartner
					else if (dimTypeVal == "LF" || dimTypeVal == "LT") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), locAddress.val(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), txtLoc.getValue(), -1, 0, 0, "");
						locAddress.val("");
						txtLoc.setValue("-1");
					}//Location From//Location To
					else if (dimTypeVal == "MC") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), cmbCampaign.find("option:selected").text(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), cmbCampaign.find("option:selected").val(), -1, 0, 0, "");
						cmbCampaign.val(-1);
					}//Campaign
					else if (dimTypeVal == "OO" || dimTypeVal == "OT") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), cmbOrg.find("option:selected").text(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), cmbOrg.find("option:selected").val(), -1, 0, 0, "");
						cmbOrg.val("-1");
					}//Organization//Org Trx
					else if (dimTypeVal == "PJ") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), txtProject.val(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), txtProj.getValue(), -1, 0, 0, "");
						txtProject.val("");
						txtProj.setValue("-1");
					}//Project
					else if (dimTypeVal == "PR") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), txtProduct.val(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), txtProd.getValue(), -1, 0, 0, "");
						txtProduct.val("");
						txtProd.setValue("-1");
					}//Product
					else if (dimTypeVal == "SA") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), "", txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), "", -1, 0, 0, "");
					}//Sub Account
					else if (dimTypeVal == "SR") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), cmbSalesRegion.find("option:selected").text(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), cmbSalesRegion.find("option:selected").val(), -1, 0, 0, "");
						cmbSalesRegion.val("-1");
					}//Sales Region
					else if (dimTypeVal == "U1" || dimTypeVal == "U2") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), txtAccountElement.val(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), txtAcctElementValue.getValue(), C_Element_ID, 0, txtb.getValue(), cmbBPartner.val());
						cmbElement.val("-1");
						txtAccountElement.val("");
						txtAcctElementValue.setValue("-1");
					}//User List 1//User List 2
					else if (dimTypeVal == "X1" || dimTypeVal == "X2" || dimTypeVal == "X3" || dimTypeVal == "X4" || dimTypeVal == "X5" || dimTypeVal == "X6" ||
						dimTypeVal == "X7" || dimTypeVal == "X8" || dimTypeVal == "X9") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), cmbUserElement.find("option:selected").text(), txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), cmbUserElement.find("option:selected").val(), -1, 0, 0, "");
						cmbUserElement.val(-1);
					}//User Element 1 to User Element 9

				}

				// busyDiv("hidden");


			});



			/* Click event of the Input file*/
			divAmountDimExcel.on('click', function () {
				$("#VIS-AMTD-file-input").val("");
				if (cmbDimensionType.val() == "0") {
					VIS.ADialog.error("VIS_DimensionType", true, "", "");
					return false;
				}
				fileUpldIconDim.trigger("click");
			});

			/* Change event of the Excel file*/

			fileUpldIconDim.on("change", function (e) {
				var file = this;
				var validExts = new Array(".xlsx", ".xls");
				var fileExt = $("#VIS-AMTD-file-input").val();
				fileExt = fileExt.substring(fileExt.lastIndexOf('.'));
				if (validExts.indexOf(fileExt) < 0) {
					VIS.ADialog.error("VIS_InValidFile", true, "", "");
					return false;
				}
				window.setTimeout(function () {
					busyDiv("visible");
					_result = $.parseJSON(VIS.UploadAmountDimension(file, null));
					// upload excel work on dialog
					UploadFileReadAndImport(_result);
				}, 400);
			});
			//Cancel Button Click.................
			btnNew.on("click", function () {
				busyDiv("visible");
				getControl(cmbDimensionType.val());
				txtAmount.setValue(0);
				checkValUpdate = false;
				btnNew.css("display", "none");
				//divbutton.css("width", "6%");
				if (cmbDimensionType.val() == "AC" || cmbDimensionType.val() == "U1" || cmbDimensionType.val() == "U2") {
					divAmount.css("width", "26%");
				}
				else { divAmount.css("width", "33.3%"); }
				btnAdd.empty().append(VIS.Msg.getMsg("Add"));
				busyDiv("hidden");
			});

			//Update Click on Update Model Panel............
			modalBtnAdd.on("keydown", function (e) {
				if (e.keyCode == 13) {
					busyDiv("visible");
					//Validate Control ...................
					if (validateControl()) {


						var dimTypeVal = cmbDimensionType.find("option:selected").val();
						//Add Data against particular Dimension Type Value......................
						if (dimTypeVal == "AC") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalTxtAccountElement.val(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalTxtAcctElementValue.getValue(), C_Element_ID, dimensionLineID, modalTxtb.getValue(), modalCmbBPartner.val());
							modalCmbElement.val("-1");
							modalTxtAcctElementValue.setValue("-1");
							modalCmbBPartner.val("");
							modalTxtb.setValue("-1");
						}//Account
						else if (dimTypeVal == "AY") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalCmbActivity.find("option:selected").text(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalCmbActivity.find("option:selected").val(), -1, dimensionLineID, 0, "");
							modalCmbActivity.val(-1);
						}//Activity
						else if (dimTypeVal == "BP") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalCmbBPartner.val(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalTxtb.getValue(), -1, dimensionLineID, 0, "");
							modalCmbBPartner.val("");
							modalTxtb.setValue("-1");
						}//BPartner
						else if (dimTypeVal == "LF" || dimTypeVal == "LT") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalLocAddress.val(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalTxtLoc.getValue(), -1, dimensionLineID, 0, "");
							modalLocAddress.val("");
							modalTxtLoc.setValue("-1");
						}//Location From//Location To
						else if (dimTypeVal == "MC") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalCmbCampaign.find("option:selected").text(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalCmbCampaign.find("option:selected").val(), -1, dimensionLineID, 0, "");
							modalCmbCampaign.val(-1);
						}//Campaign
						else if (dimTypeVal == "OO" || dimTypeVal == "OT") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalCmbOrg.find("option:selected").text(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalCmbOrg.find("option:selected").val(), -1, dimensionLineID, 0, "");
							modalCmbOrg.val("-1");
						}//Organization//Org Trx
						else if (dimTypeVal == "PJ") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalTxtProject.val(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalTxtProj.getValue(), -1, dimensionLineID, 0, "");
							modalTxtProject.val("");
							modalTxtProj.setValue("-1");
						}//Project
						else if (dimTypeVal == "PR") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalTxtProduct.val(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalTxtProd.getValue(), -1, dimensionLineID, 0, "");
							modalTxtProduct.val("");
							modalTxtProd.setValue("-1");
						}//Product
						else if (dimTypeVal == "SA") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), "", txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), "", -1, dimensionLineID, 0, "");
						}//Sub Account
						else if (dimTypeVal == "SR") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalCmbSalesRegion.find("option:selected").text(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalCmbSalesRegion.find("option:selected").val(), -1, dimensionLineID, 0, "");
							modalCmbSalesRegion.val("-1");
						}//Sales Region
						else if (dimTypeVal == "U1" || dimTypeVal == "U2") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalTxtAccountElement.val(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalTxtAcctElementValue.getValue(), C_Element_ID, dimensionLineID, 0, "");
							modalCmbElement.val("-1");
							modalTxtAccountElement.val("");
							modalTxtAcctElementValue.setValue("-1");
						}//User List 1//User List 2
						else if (dimTypeVal == "X1" || dimTypeVal == "X2" || dimTypeVal == "X3" || dimTypeVal == "X4" || dimTypeVal == "X5" || dimTypeVal == "X6" ||
							dimTypeVal == "X7" || dimTypeVal == "X8" || dimTypeVal == "X9") {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalCmbUserElement.find("option:selected").text(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalCmbUserElement.find("option:selected").val(), -1, dimensionLineID, 0, "");
							modalCmbUserElement.val(-1);
						}//User Element 1 to User Element 9

					}
				}
				//busyDiv("hidden");
			});
			modalBtnAdd.on("click", function () {
				busyDiv("visible");
				//Validate Control ...................
				if (validateControl()) {


					var dimTypeVal = cmbDimensionType.find("option:selected").val();
					//Add Data against particular Dimension Type Value......................
					if (dimTypeVal == "AC") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalTxtAccountElement.val(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalTxtAcctElementValue.getValue(), C_Element_ID, dimensionLineID, modalTxtb.getValue(), modalCmbBPartner.val());
						modalCmbElement.val("-1");
						modalTxtAcctElementValue.setValue("-1");
						modalCmbBPartner.val("");
						modalTxtb.setValue("-1");
					}//Account
					else if (dimTypeVal == "AY") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalCmbActivity.find("option:selected").text(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalCmbActivity.find("option:selected").val(), -1, dimensionLineID, 0, "");
						modalCmbActivity.val(-1);
					}//Activity
					else if (dimTypeVal == "BP") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalCmbBPartner.val(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalTxtb.getValue(), -1, dimensionLineID, 0, "");
						modalCmbBPartner.val("");
						modalTxtb.setValue("-1");
					}//BPartner
					else if (dimTypeVal == "LF" || dimTypeVal == "LT") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalLocAddress.val(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalTxtLoc.getValue(), -1, dimensionLineID, 0, "");
						modalLocAddress.val("");
						modalTxtLoc.setValue("-1");
					}//Location From//Location To
					else if (dimTypeVal == "MC") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalCmbCampaign.find("option:selected").text(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalCmbCampaign.find("option:selected").val(), -1, dimensionLineID, 0, "");
						modalCmbCampaign.val(-1);
					}//Campaign
					else if (dimTypeVal == "OO" || dimTypeVal == "OT") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalCmbOrg.find("option:selected").text(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalCmbOrg.find("option:selected").val(), -1, dimensionLineID, 0, "");
						modalCmbOrg.val("-1");
					}//Organization//Org Trx
					else if (dimTypeVal == "PJ") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalTxtProject.val(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalTxtProj.getValue(), -1, dimensionLineID, 0, "");
						modalTxtProject.val("");
						modalTxtProj.setValue("-1");
					}//Project
					else if (dimTypeVal == "PR") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalTxtProduct.val(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalTxtProd.getValue(), -1, dimensionLineID, 0, "");
						modalTxtProduct.val("");
						modalTxtProd.setValue("-1");
					}//Product
					else if (dimTypeVal == "SA") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), "", txtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), "", -1, dimensionLineID, 0, "");
					}//Sub Account
					else if (dimTypeVal == "SR") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalCmbSalesRegion.find("option:selected").text(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalCmbSalesRegion.find("option:selected").val(), -1, dimensionLineID, 0, "");
						modalCmbSalesRegion.val("-1");
					}//Sales Region
					else if (dimTypeVal == "U1" || dimTypeVal == "U2") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalTxtAccountElement.val(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalTxtAcctElementValue.getValue(), C_Element_ID, dimensionLineID, 0, "");
						modalCmbElement.val("-1");
						modalTxtAccountElement.val("");
						modalTxtAcctElementValue.setValue("-1");
					}//User List 1//User List 2
					else if (dimTypeVal == "X1" || dimTypeVal == "X2" || dimTypeVal == "X3" || dimTypeVal == "X4" || dimTypeVal == "X5" || dimTypeVal == "X6" ||
						dimTypeVal == "X7" || dimTypeVal == "X8" || dimTypeVal == "X9") {
						addDimensionAmount(cmbDimensionType.find("option:selected").text(), modalCmbUserElement.find("option:selected").text(), modalTxtAmount.getValue(), recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), modalCmbUserElement.find("option:selected").val(), -1, dimensionLineID, 0, "");
						modalCmbUserElement.val(-1);
					}//User Element 1 to User Element 9

				}
				//busyDiv("hidden");


			});

			//Validate Numeric Content on KeyPress..............
			//txtTotalAmount.getControl().on("keypress", function (e) {
			//    if ((e.keyCode >= 48 && e.keyCode <= 57) || e.keyCode == 45 || e.keyCode == 46) {
			//        return true;
			//    }
			//    else { return false; }

			//});

			// changes done to allow negative value also
			//txtAmount.getControl().on("keypress", function (e) {
			//    if ((e.keyCode >= 48 && e.keyCode <= 57) || e.keyCode == 45 || e.keyCode == 46) {
			//        return true;
			//    }
			//    else { return false; }

			//});

			// changes done to allow negative value also
			//modalTxtAmount.getControl().on("keypress", function (e) {
			//    if ((e.keyCode >= 48 && e.keyCode <= 57) || e.keyCode == 45 || e.keyCode == 46) {
			//        return true;
			//    }
			//    else { return false; }

			//});
			modalBtnNew.on("click", function () {
				checkValUpdate = false;
				modalDiv.css("display", "none");
			});
			modalBtnNew.on("keydown", function (e) {
				if (e.keyCode == 13) {
					checkValUpdate = false;
					modalDiv.css("display", "none");
				}
			});
			//Grid Paging Click.............................
			ulStatusdimension.on(VIS.Events.onTouchStartOrClick, "LI", function (e) {
				busyDiv("visible");
				if ($(e.target).attr("action") == "first") {
					if (pageNoOrder != 1) {
						pageNoOrder = 1;
						getDimensionLine(arrAcctSchemaID, null, function (temp) {
							loadData(temp);
							cmbStausRecordCount.val(pageNoOrder);
						});



					}
				}
				else if ($(e.target).attr("action") == "prev") {
					if (pageNoOrder != 1) {
						pageNoOrder--;
						getDimensionLine(arrAcctSchemaID, null, function (temp) {
							loadData(temp);
							cmbStausRecordCount.val(pageNoOrder);
						});
						//loadData(getDimensionLine(arrAcctSchemaID));
						//cmbStausRecordCount.val(pageNoOrder);

					}
				}
				else if ($(e.target).attr("action") == "next") {

					var totRec = calculateNoofPages();
					if (pageNoOrder >= totRec) {
						busyDiv("hidden");
						return;
					}
					pageNoOrder++;
					getDimensionLine(arrAcctSchemaID, null, function (temp) {
						loadData(temp);
						cmbStausRecordCount.val(pageNoOrder);
					});
					//loadData(getDimensionLine(arrAcctSchemaID));
					//cmbStausRecordCount.val(pageNoOrder);

				}
				else if ($(e.target).attr("action") == "last") {
					var totRec = calculateNoofPages();
					if (pageNoOrder >= totRec) {
						busyDiv("hidden");
						return;
					}
					pageNoOrder = totRec;
					getDimensionLine(arrAcctSchemaID, null, function (temp) {
						loadData(temp);
						cmbStausRecordCount.val(pageNoOrder);
					});
					//loadData(getDimensionLine(arrAcctSchemaID));
					//cmbStausRecordCount.val(pageNoOrder);

				}
				busyDiv("hidden");
			});
			cmbStausRecordCount.on("change", function () {
				busyDiv("visible");
				if (pageNoOrder != cmbStausRecordCount.val()) {
					pageNoOrder = cmbStausRecordCount.val();
					getDimensionLine(arrAcctSchemaID, null, function (temp) {
						loadData(temp);
						cmbStausRecordCount.val(pageNoOrder);
					});
					////loadData(getDimensionLine(arrAcctSchemaID));
					////cmbStausRecordCount.val(pageNoOrder);
				}
				busyDiv("hidden");
			});
			btnOk.on("click", function () {
				busyDiv("visible");
				// handle the cases of negative amount.
				if (Math.abs(parseFloat(txtTotal.getValue())) > Math.abs(parseFloat(txtTotalAmount.getValue()))) {
					VIS.ADialog.warn("LineTotalNotGrater");
					txtTotalAmount.getControl().focus();
					busyDiv("hidden");
				}
				else {
					if (txtTotalAmount.getValue() != "" && txtTotalAmount.getValue() != 0) {
						arrAcctSchemaID = [];
						if (cmbAcctSchema.val() == 0) {
							arrAcctSchemaID = allAcctSchemaID;
						}
						else {
							arrAcctSchemaID[0] = cmbAcctSchema.val();
						}
						insertDimensionAmountLine(C_DimAmt_ID, txtTotalAmount.getValue(), -1, arrAcctSchemaID, "", "", "", -1, -1, -1, 0, "", 0, function () {
							self.onClosing(C_DimAmt_ID);
							ch.close();
						});

					}
					else {
						self.onClosing(C_DimAmt_ID);
						ch.close();
					}
				}

			});

		}

		/* Send the File details to Controller*/
		function UploadFileReadAndImport(_result) {

			if (_result != null) {
				if (_result._filename == null || _result._filename == "" || _result._path == null || _result._path == "") {
					return;
				}
				else if (_result._error != null && _result._error != "") {
					VIS.ADialog.info(_result._error, null, "", "");
					return;
				}
				else {

					var _path = _result._path;
					var _filename = _result._filename;
					var dimType = cmbDimensionType.find("option:selected").val();
					var acctschema = cmbAcctSchema.find("Option:selected").val();
					 var docAmount= parseFloat(txtTotalAmount.getValue())
					var param = [];
					param.push(_path);
					param.push(dimType)
					param.push(acctschema)
					param.push(docAmount)

					$.ajax({
						url: VIS.Application.contextUrl + "AmountDivision/GetDataFromExcelOrText",
						type: "POST",
						datatype: "json",
						contentType: "application/json; charset=utf-8",
						async: false,
						data: JSON.stringify(param),
						success: function (result) {
							var flag = true;
							flag = callbackReadFileData(result);
							if (count == 0 && flag) {
								VIS.ADialog.warn("VIS_InCorrectData", null, "", "");
							}
						},
						error: function () {
							$bsyDiv[0].style.visibility = "hidden";
							VIS.ADialog.info(VIS.Msg.getMsg("error"), null, "", "");
						}
					})
					//$bsyDiv[0].style.visibility = "hidden";
				}
			}
		};

		/* bind the Excel records into grid */
		function callbackReadFileData(data) {
			var amount = parseFloat(txtTotalAmount.getValue());

			data = JSON.parse(data);
			var amountExcel = parseFloat(data.result[0]["TotalAmount"])
			if ((amount > 0 && amountExcel > amount) || (amount < 0 && Math.abs(amountExcel) > Math.abs(amount))) {
				VIS.ADialog.warn("LineTotalNotGrater");
				busyDiv("hidden");
				return false;
			}
			else {
				count = 0;
				for (var i = 0; i < data.result.length; i++) {
					var dimTypeVal = cmbDimensionType.find("option:selected").val();
					if (dimTypeVal == "AY" || dimTypeVal == "BP" || dimTypeVal == "MC" || dimTypeVal == "OO" || dimTypeVal == "OT" || dimTypeVal == "PJ" || dimTypeVal == "PR" || dimTypeVal == "SR") {
						if (parseInt(data.result[i]["Record_ID"]) > 0) {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), data.result[i]["Search Key/Name"], data.result[i]["Amount"], recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), data.result[i]["Record_ID"], -1, dimensionLineID, 0, "");
							count++;
						}
					}
					else if (dimTypeVal == "U1" || dimTypeVal == "U2" || dimTypeVal == "AC") {
						if (data.result[i]["AccoutId"] != 0 && data.result[i]["BPartnerId"] != 0) {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), data.result[i]["AccountValue"], data.result[i]["Amount"], recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), data.result[i]["AccoutId"], data.result[i]["C_Element_ID"], dimensionLineID, data.result[i]["BPartnerId"], data.result[i]["Search Key/Name"]);
							modalCmbElement.val("-1");
							modalTxtAccountElement.val("");
							modalTxtAcctElementValue.setValue("-1");
							count++;
						}
					}//User List 1//User List 2
					else if (dimTypeVal == "X1" || dimTypeVal == "X2" || dimTypeVal == "X3" || dimTypeVal == "X4" || dimTypeVal == "X5" || dimTypeVal == "X6" ||
						dimTypeVal == "X7" || dimTypeVal == "X8" || dimTypeVal == "X9") {
						if (parseInt(data.result[i]["Record_ID"]) != 0) {
							addDimensionAmount(cmbDimensionType.find("option:selected").text(), data.result[i]["Search Key/Name"], data.result[i]["Amount"], recid, cmbAcctSchema.find("Option:selected").val(), cmbDimensionType.find("option:selected").val(), data.result[i]["Record_ID"], -1, dimensionLineID, 0, "");
							modalCmbUserElement.val(-1);
							count++;
						}
					}
				}
				busyDiv("hidden");
			}
		};
		function hideShowDimensionValue() {
			if (cmbDimensionType.val() == "" || cmbDimensionType.val() == 0) {
				divAmount.css("display", "none");
				btnAdd.css("display", "none");
				if (lblMaxAmount.html() == "") {
					ch.getRoot().parent().css("height", "390px");
				}
				else {
					ch.getRoot().parent().css("height", "410px");
				}
			}
			else {
				divAmount.css("display", "block");
				btnAdd.css("display", "block");
				if (lblMaxAmount.html() == "") {
					ch.getRoot().parent().css("height", "448px");
				}
				else {
					ch.getRoot().parent().css("height", "469px");
				}
			}
		}
		//This Function Filter Data in Array against selected Accounting Schema............
		function filterStoreDimensionData(storeDimensionData, acctSchema) {
			if (storeDimensionData.length > 0) {
				chkDuplicate = $.grep(storeDimensionData, function (e) { return (e.AcctSchema == acctSchema); });
				return chkDuplicate;
			}
			else { return storeDimensionData; }
		};

		//Validate Control For valid Input............
		function onValidateOk() {
			if ((cmbAcctSchema.val() == "" || cmbAcctSchema.val() == -1) && (cmbDimensionType.val() == null || cmbDimensionType.val() == 0)) {
				if (txtTotalAmount.getValue() == "" && txtTotalAmount.getValue().trim().length == 0) {
					VIS.ADialog.warn("ValidateTotalAmount");
					txtTotalAmount.getControl().focus();
					busyDiv("hidden");
					return false;
				}
				else { return true; }
			}

		};

		function getMaxDimensionAmount() {
			// busyDiv("visible");
			var tempMax = false;
			var tempacctName = "";
			var tempacctAmt = "";
			lblMaxAmount.empty();
			lblMaxAmount.append("");
			//var sql = "select * from (select Max(ct.totaldimlineamout) as amount,ac.name from c_dimamtaccttype ct " +
			//           " inner join c_dimamtline cl on ct.c_dimamt_id=cl.c_dimamt_id and ct.c_dimamtaccttype_id=cl.c_dimamtaccttype_id" +
			//           " inner join c_acctschema ac on ac.c_acctschema_id=ct.c_acctschema_id " +
			//            " group by ac.name,ct.c_dimamt_id having ct.c_dimamt_id=" + C_DimAmt_ID + " order by Amount desc ) " +
			//             " main where rownum=1";
			var sql = " select distinct ct.totaldimlineamout as amount,ac.name from c_dimamtaccttype ct " +
				" inner join c_dimamtline cl on ct.c_dimamt_id=cl.c_dimamt_id and ct.c_dimamtaccttype_id=cl.c_dimamtaccttype_id " +
				" inner join c_acctschema ac on ac.c_acctschema_id=ct.c_acctschema_id " +
				"  where ct.totaldimlineamout in (Select max(totaldimlineamout) from c_Dimamtaccttype " +
				"   where c_dimamt_id=" + C_DimAmt_ID + ") and ct.c_dimamt_id=" + C_DimAmt_ID + "";
			var maxDimension = VIS.DB.executeReader(sql);
			while (maxDimension.read()) {
				if (maxDimension.getInt(0) != null) {
					divMaxAmount.css("display", "block");
					tempMax = true;
					tempacctAmt = format.GetFormatedValue(maxDimension.getString(0));
					if (tempacctName == "") {
						tempacctName += maxDimension.getString(1);
					}
					else {
						tempacctName += "," + maxDimension.getString(1);
					}
				}
				else {
					lblMaxAmount.append("");
					//ch.getRoot().parent().css("height", "448px");
					tempMax = false;
					window.setTimeout(function () {
						ch.getRoot().parent().css("height", "448px");
					}, 200);
				}
			}
			if (tempMax) {
				if (tempacctName.length > 55) {
					tempacctName = tempacctName.substring(0, 55) + "...";
				}
				lblMaxAmount.append(VIS.Msg.getMsg("MaxDimensionAmount") + " : " + tempacctAmt + " " + VIS.Msg.getMsg("ForAcctSchema") + " : " + tempacctName);
				window.setTimeout(function () {
					ch.getRoot().parent().css("height", "470px");
				}, 200);
			}
			if (C_DimAmt_ID != 0) {
				var maxAmount = VIS.DB.executeScalar("select amount from c_dimAmt where c_dimamt_id=" + C_DimAmt_ID + "");

				// handle the cases of negative amount.
				if (Math.abs(parseFloat(defaultVal)) > 0) {
					txtTotalAmount.setValue(defaultVal);
				}
				else {
					txtTotalAmount.setValue(maxAmount);
				}
			}
			else {
				if (defaultVal) {
					txtTotalAmount.setValue(defaultVal);
				}
				else {
					txtTotalAmount.setValue(0);
				}
			}
			if (lblMaxAmount.html() == "") {
				divMaxAmount.css("display", "none");
				ch.getRoot().parent().css("height", "448px");
			}
			// busyDiv("hidden");
		}

		function validateControl() {

			if (txtTotalAmount.getValue() == 0 || (txtTotalAmount.getValue() == "" && txtTotalAmount.getValue().trim().length == 0)) {
				VIS.ADialog.warn("ValidateTotalAmount");
				txtTotalAmount.getControl().focus();
				busyDiv("hidden");
				return false;
			}
			else if (cmbAcctSchema.val() == "" || cmbAcctSchema.val() == -1) {
				VIS.ADialog.warn("ValidateAcctSchema");
				cmbAcctSchema.focus();
				busyDiv("hidden");
				return false;
			}
			else if (cmbDimensionType.val() == "" || cmbDimensionType.val() == 0) {
				VIS.ADialog.warn("ValidateDimType");
				cmbDimensionType.focus();
				busyDiv("hidden");
				return false;
			}
			else if (txtTotalAmount.getValue() == 0 || (txtTotalAmount.getValue() == "" && txtTotalAmount.getValue().trim().length == 0)) {
				VIS.ADialog.warn("ValidateTotalAmount");
				txtTotalAmount.getControl().focus();
				busyDiv("hidden");
				return false;
			}
			if (!checkValUpdate) {
				if (txtAmount.getValue() == 0 || (txtAmount.getValue() == "" && txtAmount.getValue().trim().length == 0)) {
					VIS.ADialog.warn("ValidateAmount");
					txtAmount.getControl().focus();
					busyDiv("hidden");
					return false;
				}   // handle the cases of negative amount.
				else if (Math.abs(parseFloat(txtAmount.getValue())) > Math.abs(parseFloat(txtTotalAmount.getValue()))) {
					VIS.ADialog.warn("LineTotalNotGrater");
					txtAmount.getControl().focus();
					busyDiv("hidden");
					return false;
				}
				// Handle the cases of Negative and Positive amount on Amount dimension control
				else if (parseFloat(txtAmount.getValue()) > 0 && parseFloat(txtTotalAmount.getValue()) < 0) {
					VIS.ADialog.warn("VIS_NegativeAmount");
					txtAmount.getControl().focus();
					busyDiv("hidden");
					return false;
				}
				else if (parseFloat(txtAmount.getValue()) < 0 && parseFloat(txtTotalAmount.getValue()) > 0) {
					VIS.ADialog.warn("VIS_PositiveAmount");
					txtAmount.getControl().focus();
					busyDiv("hidden");
					return false;
				}
				else if (!validateDynamicControl()) {
					busyDiv("hidden");
					return false;
				}
				else { busyDiv("hidden"); return true; }

			}

			else if (checkValUpdate) {
				if (modalTxtAmount.getValue() == 0 || (modalTxtAmount.getValue() == "" && modalTxtAmount.getValue().trim().length == 0)) {
					VIS.ADialog.warn("ValidateAmount");
					modalTxtAmount.getControl().focus();
					busyDiv("hidden");
					return false;
				}   // handle the cases of negative amount.
				else if (Math.abs(parseFloat(modalTxtAmount.getValue())) > Math.abs(parseFloat(txtTotalAmount.getValue()))) {
					VIS.ADialog.warn("LineTotalNotGrater");
					modalTxtAmount.getControl().focus();
					busyDiv("hidden");
					return false;
				}
				// Handle the cases of Negative and Positive amount on Amount dimension control
				else if (parseFloat(modalTxtAmount.getValue()) > 0 && parseFloat(txtTotalAmount.getValue()) < 0) {
					VIS.ADialog.warn("VIS_NegativeAmount");
					txtAmount.getControl().focus();
					busyDiv("hidden");
					return false;
				}
				else if (parseFloat(modalTxtAmount.getValue()) < 0 && parseFloat(txtTotalAmount.getValue()) > 0) {
					VIS.ADialog.warn("VIS_PositiveAmount");
					txtAmount.getControl().focus();
					busyDiv("hidden");
					return false;
				}
				else if (!validateDynamicControl()) {
					busyDiv("hidden");
					return false;
				}
				else { busyDiv("hidden"); return true; }
			}

			else { busyDiv("hidden"); return true; }
		};
		function validateDynamicControl() {
			if (cmbDimensionType.val() != 0) {
				if (!checkValUpdate) {
					if (cmbDimensionType.val() == "AC") {
						//if (cmbElement.val() == -1 || cmbElement.val() == null) {
						//    VIS.ADialog.warn("ValidateElement");
						//    cmbElement.focus();
						//    busyDiv("hidden");
						//    return false;
						//}
						//else
						if (txtAccountElement.val() == "") {
							VIS.ADialog.warn("ValidateAcctElement");
							txtAccountElement.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//Account
					else if (cmbDimensionType.val() == "AY") {
						if (cmbActivity.val() == -1 || cmbActivity.val() == null) {
							VIS.ADialog.warn("ValidateActivity");
							cmbActivity.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//Activity
					else if (cmbDimensionType.val() == "BP") {
						if (cmbBPartner.val() == "") {
							VIS.ADialog.warn("ValidateBPartner");
							cmbBPartner.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//BPartner
					else if (cmbDimensionType.val() == "LF" || cmbDimensionType.val() == "LT") {
						if (locAddress.val() == "") {
							VIS.ADialog.warn("ValidateLocation");
							locAddress.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }

					}//Location From//Location To
					else if (cmbDimensionType.val() == "MC") {
						if (cmbCampaign.val() == -1 || cmbCampaign.val() == null) {
							VIS.ADialog.warn("ValidateCamp");
							cmbCampaign.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//Campaign
					else if (cmbDimensionType.val() == "OO" || cmbDimensionType.val() == "OT") {
						if (cmbOrg.val() == -1 || cmbOrg.val() == null) {
							VIS.ADialog.warn("ValidateOrg");
							cmbOrg.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//Organization//Org Trx
					else if (cmbDimensionType.val() == "PJ") {
						if (txtProject.val() == "") {
							VIS.ADialog.warn("ValidateProject");
							txtProject.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//Project
					else if (cmbDimensionType.val() == "PR") {
						if (txtProduct.val() == "") {
							VIS.ADialog.warn("ValidateProduct");
							txtProduct.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//Product
					else if (cmbDimensionType.val() == "SA") { return true; }//Sub Account
					else if (cmbDimensionType.val() == "SR") {
						if (cmbSalesRegion.val() == -1 || cmbSalesRegion.val() == null) {
							VIS.ADialog.warn("ValidateSales");
							cmbSalesRegion.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//Sales Region
					else if (cmbDimensionType.val() == "U1" || cmbDimensionType.val() == "U2") {
						if (txtAccountElement.val() == -1 || txtAccountElement.val() == null) {
							VIS.ADialog.warn("ValidateElement");
							txtAccountElement.focus();
							busyDiv("hidden");
							return false;
						}
						else if (txtAccountElement.val() == "") {
							VIS.ADialog.warn("ValidateAcctElement");
							txtAccountElement.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }

					}//User List 1//User List 2
					else if (cmbDimensionType.val() == "X1" || cmbDimensionType.val() == "X2" || cmbDimensionType.val() == "X3" || cmbDimensionType.val() == "X4" || cmbDimensionType.val() == "X5" || cmbDimensionType.val() == "X6" ||
						cmbDimensionType.val() == "X7" || cmbDimensionType.val() == "X8" || cmbDimensionType.val() == "X9") {
						if (cmbUserElement.val() == -1 || cmbUserElement.val() == null) {
							VIS.ADialog.warn("ValidateUElement");
							cmbUserElement.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//User Element 1 to User Element 9
				}
				else {

					if (cmbDimensionType.val() == "AC") {
						//if (modalCmbElement.val() == -1 || modalCmbElement.val() == null) {
						//    VIS.ADialog.warn("ValidateElement");
						//    modalCmbElement.focus();
						//    busyDiv("hidden");
						//    return false;
						//}
						//else
						if (modalTxtAccountElement.val() == "") {
							VIS.ADialog.warn("ValidateAcctElement");
							modalTxtAccountElement.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//Account
					else if (cmbDimensionType.val() == "AY") {
						if (modalCmbActivity.val() == -1 || modalCmbActivity.val() == null) {
							VIS.ADialog.warn("ValidateActivity");
							modalCmbActivity.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//Activity
					else if (cmbDimensionType.val() == "BP") {
						if (modalCmbBPartner.val() == "") {
							VIS.ADialog.warn("ValidateBPartner");
							modalCmbBPartner.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//BPartner
					else if (cmbDimensionType.val() == "LF" || cmbDimensionType.val() == "LT") {
						if (modalLocAddress.val() == "") {
							VIS.ADialog.warn("ValidateLocation");
							modalLocAddress.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }

					}//Location From//Location To
					else if (cmbDimensionType.val() == "MC") {
						if (modalCmbCampaign.val() == -1 || modalCmbCampaign.val() == null) {
							VIS.ADialog.warn("ValidateCamp");
							cmbCampaign.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//Campaign
					else if (cmbDimensionType.val() == "OO" || cmbDimensionType.val() == "OT") {
						if (modalCmbOrg.val() == -1 || modalCmbOrg.val() == null) {
							VIS.ADialog.warn("ValidateOrg");
							modalCmbOrg.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//Organization//Org Trx
					else if (cmbDimensionType.val() == "PJ") {
						if (modalTxtProject.val() == "") {
							VIS.ADialog.warn("ValidateProject");
							modalTxtProject.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//Project
					else if (cmbDimensionType.val() == "PR") {
						if (modalTxtProduct.val() == "") {
							VIS.ADialog.warn("ValidateProduct");
							modalTxtProduct.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//Product
					else if (cmbDimensionType.val() == "SA") { return true; }//Sub Account
					else if (cmbDimensionType.val() == "SR") {
						if (modalCmbSalesRegion.val() == -1 || modalCmbSalesRegion.val() == null) {
							VIS.ADialog.warn("ValidateSales");
							modalCmbSalesRegion.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//Sales Region
					else if (cmbDimensionType.val() == "U1" || cmbDimensionType.val() == "U2") {
						if (modalCmbElement.val() == -1 || modalCmbElement.val() == null) {
							VIS.ADialog.warn("ValidateElement");
							modalCmbElement.focus();
							busyDiv("hidden");
							return false;
						}
						else if (modalTxtAccountElement.val() == "") {
							VIS.ADialog.warn("ValidateAcctElement");
							modalTxtAccountElement.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }

					}//User List 1//User List 2
					else if (cmbDimensionType.val() == "X1" || cmbDimensionType.val() == "X2" || cmbDimensionType.val() == "X3" || cmbDimensionType.val() == "X4" || cmbDimensionType.val() == "X5" || cmbDimensionType.val() == "X6" ||
						cmbDimensionType.val() == "X7" || cmbDimensionType.val() == "X8" || cmbDimensionType.val() == "X9") {
						if (modalCmbUserElement.val() == -1 || modalCmbUserElement.val() == null) {
							VIS.ADialog.warn("ValidateUElement");
							modalCmbUserElement.focus();
							busyDiv("hidden");
							return false;
						}
						else { busyDiv("hidden"); return true; }
					}//User Element 1 to User Element 9
				}
			}
		};
		//This Function finally Save Data From Array to database on Ok Button Click................
		//function insertDimensionAmount() {
		//    arrDimensionEType = [];
		//    //Fetch Distinct Accounting Schma from Array................
		//    var acct = storeDimensionData.map(function (e) { return e.AcctSchema; });
		//    arrAcctSchemaID = acct.filter(function (itm, i, acct) {
		//        return i == acct.indexOf(itm);
		//    });
		//    //Fetch Distinct Dimension Type from Array against Accounting Schema............
		//    var tempvar;
		//    for (var j = 0; j < arrAcctSchemaID.length; j++) {
		//        var element = filterStoreDimensionData(storeDimensionData, arrAcctSchemaID[j]).map(function (e) { return e.DimensionTypeVal; })
		//        tempvar = element.filter(function (item, i, element) {
		//            return i == element.indexOf(item);
		//        });
		//        arrDimensionEType[j] = tempvar[0];
		//    }
		//    if (storeDimensionData.length == 0) {
		//        //if Array is Empty but user selected Accounting Schema and dimensionType........
		//        //Then Override the value of arrAcctSchemaID and arrDimensionEType array......
		//        if (cmbAcctSchema.val() != "" && cmbAcctSchema.val() != -1) {
		//            if (cmbAcctSchema.val() == 0) {
		//                arrAcctSchemaID = allAcctSchemaID;
		//                for (var i = 0; i < arrAcctSchemaID.length; i++) {
		//                    arrDimensionEType[i] = cmbDimensionType.val();
		//                }
		//            }
		//            else {
		//                arrAcctSchemaID = [cmbAcctSchema.val()];
		//                arrDimensionEType = [cmbDimensionType.val()];
		//            }

		//        }
		//    }
		//    else {
		//        //This Case Work is Accounting Schema is All..........
		//        //Insert All Accounting Schema ID in arrAcctSchemaID array and Dimension Type Value in arrDimensionEType array.
		//        if (arrAcctSchemaID[0] == 0) {
		//            arrAcctSchemaID = allAcctSchemaID;
		//            for (var i = 0; i < arrAcctSchemaID.length; i++) {
		//                arrDimensionEType[i] = cmbDimensionType.val();
		//            }
		//        }

		//    }

		//    var loading = true;
		//    var retval = false;
		//    $.ajax({
		//        dataType: "json",
		//        type: "POST",
		//        async: false,
		//        url: VIS.Application.contextUrl + "AmountDivision/InsertDimensionAmount",
		//        data: { "acctSchema": JSON.stringify(arrAcctSchemaID), "elementType": JSON.stringify(arrDimensionEType), "amount": txtTotalAmount.val(), "dimensionLine": JSON.stringify(storeDimensionData), "DimAmtId": C_DimAmt_ID },
		//        success: function (result) {
		//            if (result != -1) {
		//                alert("Record saved successfully");
		//                C_DimAmt_ID = result;
		//                self.onClosing(C_DimAmt_ID);
		//                retval = true;
		//                loading = false;
		//            }
		//            else {
		//                alert("Error while saving Dimension.Try again later..");
		//                retval = false;
		//                loading = false;
		//            }
		//        },
		//        error: function () {
		//            alert(VIS.Msg.getMsg('ErrorWhileSavingData'));
		//            bsyDiv[0].style.visibility = "hidden";
		//            retval = false;
		//            loading = false;
		//        }


		//    });



		//    return retval;
		//};

		//This function Fetch All Dimension Line From DataBase...........
		function getDimensionLine(acountingSchemaArr, onDataLoad, callback) {
			var storeData = [];
			//busyDiv("visible");
			// window.setTimeout(function () {
			//   window.setTimeout(function () {
			$.ajax({
				dataType: "json",
				type: "POST",
				url: VIS.Application.contextUrl + "AmountDivision/GetDiminsionLine",
				data: { "accountingSchema": JSON.stringify(acountingSchemaArr), "dimensionID": C_DimAmt_ID, "DimensionLineID": 0, "pageNo": pageNoOrder, "pSize": PAGE_SIZE },
				//     async: false,
				success: function (result) {
					if (result) {

						result = JSON.parse(result);
						refreshCmbDimension = true;
						if (result.length > 0) {
							for (var j = 0; j < result.length; j++) {

								if (j == 0 && refreshCmbDimension) {
									totalRecords = result[j].TotalRecord;
									var countPG = calculateNoofPages();
									cmbStausRecordCount.empty();
									for (b = 1; b < countPG + 1; b++) {

										cmbStausRecordCount.empty().append('<option value=' + b + '>' + b + '</option>');
									}
									cmbStausRecordCount.val(1);
									refreshCmbDimension = false;
								}
							}
							if (onDataLoad) {
								cmbAcctSchema.val(result[0].AcctSchema);
								getDiminsionType(cmbAcctSchema.val(), function () {
									cmbDimensionType.val(result[0].DimensionTypeVal);
									getControl(cmbDimensionType.val());
									cmbValue = result[0].DimensionTypeVal;
									acctValue = result[0].AcctSchema;
									txtTotal.setValue(format.GetFormatedValue(result[0].TotalLineAmount));
								});
							}
							if (!onDataLoad) {
								cmbValue = result[0].DimensionTypeVal;
								acctValue = result[0].AcctSchema;
								txtTotal.setValue(format.GetFormatedValue(result[0].TotalLineAmount));
							}
						}
						else {
							cmbStausRecordCount.empty().append('<option value="1">1</option>');
							txtTotal.setValue(format.GetFormatedValue(0));
						}
						//Show Default Value against first occuring Accounting Schema..............

						recid = result.length;
						txtAmount.setValue(0);
						checkValUpdate = false;
						btnAdd.empty().append(VIS.Msg.getMsg("Add"));
						storeData = result;

						if (callback) {
							callback(result);
						}

						// busyDiv("hidden");
					}
					else {
						VIS.ADialog.error("ErrorGettingData");
						busyDiv("hidden");
					}
				},

				error: function () {
					busyDiv("hidden");
					VIS.ADialog.error('ErrorGettingData');
					return;
				}

			});

			//busyDiv("hidden");
			//}, 1000);
			return storeData;
			// }, 3000);
		};
		function busyDiv(Value) {
			bsyDiv[0].style.visibility = Value;
		}
		function calculateNoofPages() {
			var noofPages = 1;
			var rem = totalRecords % PAGE_SIZE;
			if (rem != 0) {
				noofPages = parseInt(totalRecords / PAGE_SIZE) + 1;
			}
			else {
				noofPages = parseInt(totalRecords / PAGE_SIZE);
			}
			return noofPages;
		};
		function insertDimensionAmountLine(recordId, totalAmount, lineAmount, acctSchemaID, elementType, elementTypeID, dimensionName, dimensionValue, elementID, oldDimensionNameValue, C_BPartner_ID, bpartnerName, oldBPartnerID, callback) {
			busyDiv("visible");
			totalAmount = format.GetFormatedValue(totalAmount);
			lineAmount = format.GetFormatedValue(lineAmount);
			$.ajax({
				dataType: "json",
				type: "POST",
				url: VIS.Application.contextUrl + "AmountDivision/InsertDimensionLine",
				data: { "recordId": recordId, "totalAmount": totalAmount, "lineAmount": lineAmount, "acctSchemaID": JSON.stringify(acctSchemaID), "elementTypeID": elementTypeID, "dimensionValue": dimensionValue, "elementID": elementID, "oldDimensionName": oldDimensionNameValue, "bpartner_ID": VIS.Utility.Util.getValueOfInt(C_BPartner_ID), "oldBPartner_ID": oldBPartnerID },
				success: function (result) {
					result = JSON.parse(result)
					if (result[0] != "") {
						C_DimAmt_ID = result[0];
						getMaxDimensionAmount();
						if (!checkValUpdate) {
							// alert("Record saved successfully");
							var g = w2ui[LineGridName].records.length;
							if (elementTypeID == "AC" || elementTypeID == "U1" || elementTypeID == "U2") {
								// format.GetFormatAmount(lineAmount, "init", this.dotFormatter)
								w2ui[LineGridName].add({ recid: g + 1, DimensionType: elementType, DimensionName: dimensionName, C_BPartner: bpartnerName, DimensionValueAmount: lineAmount, lineAmountID: result[1], CalculateDimValAmt: lineAmount, C_BPartner_ID: C_BPartner_ID });
							}
							else {
								w2ui[LineGridName].add({ recid: g + 1, DimensionType: elementType, DimensionName: dimensionName, DimensionValueAmount: lineAmount, lineAmountID: result[1], CalculateDimValAmt: lineAmount });
							}
							var temp = format.GetFormatedValue((parseFloat(txtTotal.getValue()) + parseFloat(lineAmount)).toFixed(Precision));
							txtTotal.setValue(temp);
						}
						else {
							VIS.ADialog.info("UpdatedDimLine");
							w2ui[LineGridName].set(recid, { DimensionName: dimensionName });
							w2ui[LineGridName].set(recid, { DimensionValueAmount: lineAmount });
							w2ui[LineGridName].set(recid, { CalculateDimValAmt: lineAmount });
							if (elementTypeID == "AC" || elementTypeID == "U1" || elementTypeID == "U2") {
								w2ui[LineGridName].set(recid, { C_BPartner: bpartnerName });
								w2ui[LineGridName].set(recid, { C_BPartner_ID: C_BPartner_ID });
							}
							var temp = format.GetFormatedValue((parseFloat(parseFloat(txtTotal.getValue()) - parseFloat(oldAmount)) + parseFloat(lineAmount)).toFixed(Precision));
							txtTotal.setValue(temp);
						}
						busyDiv("hidden");
					}
					else {
						VIS.ADialog.error("ErrorGettingData");
						busyDiv("hidden");
					}

					if (callback) {
						callback();
					}
				},
				error: function () {
					busyDiv("hidden");
					VIS.ADialog.error("ErrorGettingData");
					retval = false;
					loading = false;
				}


			});
		}
		//This Function Store Dimension Data in storeDimensionData Array.........................
		function addDimensionAmount(DimensionType, DimensionName, Amount, recid, AccountSchemaVal, DimensionTypeVal, DimensionNameVal, ElementID, DimensionLineID, C_BPartner_ID, BpartnerName) {
			Amount = format.GetFormatedValue(Amount.toFixed(Precision));
			var chkDuplicate = "";
			arrAcctSchemaID = [];
			if (AccountSchemaVal == 0) {

				arrAcctSchemaID = allAcctSchemaID;

			}
			else {

				arrAcctSchemaID[0] = AccountSchemaVal;
			}
			if (checkValUpdate) {
				var sql = "select nvl(cline.c_dimamtline_id,0) as DimLineID from c_dimamt cd inner join c_dimamtaccttype cact on cd.c_dimamt_id=cact.c_dimamt_id " +
					" inner join c_dimamtline cline on cd.c_Dimamt_id=cline.c_dimamt_id and cact.c_dimamtaccttype_id=cline.c_dimamtaccttype_id " +
					" where cd.c_dimamt_id=" + C_DimAmt_ID + " and cact.elementtype='" + DimensionTypeVal + "' and cact.c_acctschema_id in(" + arrAcctSchemaID.toString() + ") and cline.c_dimamtline_id not in (" + DimensionLineID + ") ";
				if (DimensionTypeVal == "AC") {
					sql += " and C_ElementValue_ID=" + DimensionNameVal + " AND NVL(C_BPartner_ID,0)=" + C_BPartner_ID;
				}//Account
				else if (DimensionTypeVal == "AY") { sql += " and C_Activity_ID =" + DimensionNameVal }//Activity
				else if (DimensionTypeVal == "BP") { sql += " and C_BPartner_ID=" + DimensionNameVal }//BPartner
				else if (DimensionTypeVal == "LF" || DimensionTypeVal == "LT") { sql += " and C_Location_ID=" + DimensionNameVal }//Location From//Location To
				else if (DimensionTypeVal == "MC") { sql += " and C_Campaign_ID=" + DimensionNameVal }//Campaign
				else if (DimensionTypeVal == "OO" || DimensionTypeVal == "OT") { sql += " and Org_ID=" + DimensionNameVal }//Organization//Org Trx
				else if (DimensionTypeVal == "PJ") { sql += " and C_Project_ID=" + DimensionNameVal }//Project
				else if (DimensionTypeVal == "PR") { sql += " and M_Product_ID=" + DimensionNameVal }//Product
				else if (DimensionTypeVal == "SA") { }//Sub Account
				else if (DimensionTypeVal == "SR") { sql += " and C_SalesRegion_ID=" + DimensionNameVal }//Sales Region
				else if (DimensionTypeVal == "U1" || DimensionTypeVal == "U2") {
					sql += " and C_ElementValue_ID=" + DimensionNameVal;
					if (C_BPartner_ID > 0) {
						sql += " AND NVL(C_BPartner_ID,0)=" + C_BPartner_ID;
					}
				}//User List 1//User List 2
				else if (DimensionTypeVal == "X1" || DimensionTypeVal == "X2" || DimensionTypeVal == "X3" || DimensionTypeVal == "X4" || DimensionTypeVal == "X5" || DimensionTypeVal == "X6" ||
					DimensionTypeVal == "X7" || DimensionTypeVal == "X8" || DimensionTypeVal == "X9") { sql += " and AD_Column_ID=" + DimensionNameVal }//User Element 1 to User Element 9
				chkDuplicate = VIS.DB.executeScalar(sql);
				if (chkDuplicate == null) {
					var paramStr = DimensionLineID.toString();
					var tempLineAmount = VIS.dataContext.getJSONRecord("AmountDivision/GetTempDimLineAmount", paramStr);
					//var tempLineAmount = VIS.DB.executeScalar("select amount from c_dimamtline where c_dimamtline_id in (" + DimensionLineID + ") and rownum=1");

					// handle the cases of negative amount.
					if (Math.abs((parseFloat(txtTotal.getValue()) + parseFloat(Amount)) - parseFloat(tempLineAmount)) <= Math.abs(parseFloat(txtTotalAmount.getValue()))) {//Dimension Line Sum Must Equal to Total Amout..............
						insertDimensionAmountLine(C_DimAmt_ID, txtTotalAmount.getValue(), Amount, arrAcctSchemaID, DimensionType, DimensionTypeVal, DimensionName, DimensionNameVal, ElementID, oldDimensionNameValue, C_BPartner_ID, BpartnerName, oldBPartnerID, function () {
							afterSave();
						});
					}
					else { VIS.ADialog.warn("LineTotalNotGrater"); modalTxtAmount.setValue(0); modalTxtAmount.getControl().focus(); afterSave(); }


				}
				else {
					VIS.ADialog.warn("NameExists");
					modalTxtAmount.setValue(0);
					busyDiv("hidden");
					return false;
				}



			}
			else {
				//This Function Work When Inserting Dimension Value in Array......................  // handle the cases of negative amount.
				if (Math.abs(parseFloat(txtTotal.getValue()) + parseFloat(Amount)) <= Math.abs(parseFloat(txtTotalAmount.getValue()))) {//Dimension Line Sum Must Equal to Total Amout..............

					var sql = "select nvl(cline.c_dimamtline_id,0) as DimLineID from c_dimamt cd inner join c_dimamtaccttype cact on cd.c_dimamt_id=cact.c_dimamt_id " +
						" inner join c_dimamtline cline on cd.c_Dimamt_id=cline.c_dimamt_id and cact.c_dimamtaccttype_id=cline.c_dimamtaccttype_id " +
						" where cd.c_dimamt_id=" + C_DimAmt_ID + " and cact.elementtype='" + DimensionTypeVal + "' and cact.c_acctschema_id in(" + arrAcctSchemaID.toString() + ")";
					if (DimensionTypeVal == "AC") {
						sql += " and C_ElementValue_ID=" + DimensionNameVal + " AND C_BPartner_ID=" + C_BPartner_ID;
					}//Account
					else if (DimensionTypeVal == "AY") { sql += " and C_Activity_ID =" + DimensionNameVal }//Activity
					else if (DimensionTypeVal == "BP") { sql += " and C_BPartner_ID=" + DimensionNameVal }//BPartner
					else if (DimensionTypeVal == "LF" || DimensionTypeVal == "LT") { sql += " and C_Location_ID=" + DimensionNameVal }//Location From//Location To
					else if (DimensionTypeVal == "MC") { sql += " and C_Campaign_ID=" + DimensionNameVal }//Campaign
					else if (DimensionTypeVal == "OO" || DimensionTypeVal == "OT") { sql += " and Org_ID=" + DimensionNameVal }//Organization//Org Trx
					else if (DimensionTypeVal == "PJ") { sql += " and C_Project_ID=" + DimensionNameVal }//Project
					else if (DimensionTypeVal == "PR") { sql += " and M_Product_ID=" + DimensionNameVal }//Product
					else if (DimensionTypeVal == "SA") { }//Sub Account
					else if (DimensionTypeVal == "SR") { sql += " and C_SalesRegion_ID=" + DimensionNameVal }//Sales Region
					else if (DimensionTypeVal == "U1" || DimensionTypeVal == "U2") {
						sql += " and C_ElementValue_ID=" + DimensionNameVal;
						if (C_BPartner_ID > 0) {
							sql += " AND C_BPartner_ID=" + C_BPartner_ID;
						}
					}//User List 1//User List 2
					else if (DimensionTypeVal == "X1" || DimensionTypeVal == "X2" || DimensionTypeVal == "X3" || DimensionTypeVal == "X4" || DimensionTypeVal == "X5" || DimensionTypeVal == "X6" ||
						DimensionTypeVal == "X7" || DimensionTypeVal == "X8" || DimensionTypeVal == "X9") { sql += " and AD_Column_ID=" + DimensionNameVal }//User Element 1 to User Element 9

					chkDuplicate = VIS.DB.executeScalar(sql);

					if (chkDuplicate == null) {
						insertDimensionAmountLine(C_DimAmt_ID, txtTotalAmount.getValue(), Amount, arrAcctSchemaID, DimensionType, DimensionTypeVal, DimensionName, DimensionNameVal, ElementID, 0, C_BPartner_ID, BpartnerName, 0, function () {
							afterSave();
						});
					}
					else {
						VIS.ADialog.warn("NameExists");
						recid = recid - 1;
						txtAmount.setValue(0);
						busyDiv("hidden");
						return false;
					}

				}
				else { VIS.ADialog.warn("LineTotalNotGrater"); txtAmount.setValue(0); txtAmount.getControl().focus(); recid = recid - 1; afterSave(); }
			}




		};
		function afterSave() {
			txtAmount.setValue(0);
			checkValUpdate = false;
			btnNew.css("display", "none");
			modalDiv.css("display", "none");
			//divbutton.css("width", "6%");
			if (cmbDimensionType.val() == "AC" || cmbDimensionType.val() == "U1" || cmbDimensionType.val() == "U2") {
				divAmount.css("width", "26%");
			}
			else { divAmount.css("width", "33.3%"); }
			btnAdd.empty().append(VIS.Msg.getMsg("Add"));
		}

		this.vetoablechange = function (evt) {
			if (evt.propertyName == "Amount") {
				txtAmount.setValue(evt.newValue);
			}
			else if (evt.propertyName == "ModalAmount") {
				modalTxtAmount.setValue(evt.newValue);
			}
		};
		//Calculate Dimension Line Amount....................
		function calculateGrossAmount(data) {
			var grossAmount = 0;
			if (data != null) {
				for (var i = 0; i < data.length; i++) {
					grossAmount += parseFloat((data[i])["CalculateDimValAmt"]);
				}
			}
			txtTotal.setValue(grossAmount);
		};

		// Unbind Dynamic generated Control...............
		function unBindControl() {
			lblOrg = null; cmbOrg = null; lblActivity = null; cmbActivity = null; lblCampaign = null; cmbCampaign = null; lblSalesRegion = null; cmbSalesRegion = null;
			lblUserElement = null; cmbUserElement = null; lblBPartner = null; cmbBPartner = null; lblProject = null; txtProject = null; lblProduct = null; txtProduct = null;
			lblElement = null; lblAccountElement = null; cmbElement = null;
			modalLblOrg = null; modalCmbOrg = null; modalLblActivity = null; modalCmbActivity = null; modalLblCampaign = null; modalCmbCampaign = null; modalLblSalesRegion = null; modalCmbSalesRegion = null;
			modalLblUserElement = null; modalCmbUserElement = null; modalLblBPartner = null; modalCmbBPartner = null; modalLblProject = null; modalTxtProject = null; modalLblProduct = null; modalTxtProduct = null;
			modalLblElement = null; modalLblAccountElement = null; modalCmbElement = null;
			generateControl.empty();
			modalGenerateControl.empty();
			C_Element_ID = 0;
			IsElementOk = true;
		};

		//Generate Dynamic Control against DimensionType.................
		function getControl(dimensionValue, datagrid) {
			unBindControl();
			if (dimensionValue == "AC") { getAccountElement(); }//Account
			else if (dimensionValue == "AY") { getActivity(); }//Activity
			else if (dimensionValue == "BP") { getBPartner(); }//BPartner
			else if (dimensionValue == "LF") { getAddress(); }//Location From
			else if (dimensionValue == "LT") { getAddress(); }//Location To
			else if (dimensionValue == "MC") { getCampaign(); }//Campaign
			else if (dimensionValue == "OO") { getOrg(dimensionValue); }//Organization
			else if (dimensionValue == "OT") { getOrg(dimensionValue); }//Org Trx
			else if (dimensionValue == "PJ") { getProject(); }//Project
			else if (dimensionValue == "PR") { getProduct(); }//Product
			else if (dimensionValue == "SA") { }//Sub Account
			else if (dimensionValue == "SR") { getSalesRegion(); }//Sales Region
			else if (dimensionValue == "U1" || dimensionValue == "U2") { getAccountElement(); }//User List 1//User List 2
			else if (dimensionValue == "X1" || dimensionValue == "X2" || dimensionValue == "X3" || dimensionValue == "X4" || dimensionValue == "X5" || dimensionValue == "X6" ||
				dimensionValue == "X7" || dimensionValue == "X8" || dimensionValue == "X9") { getUserElement(); }//User Element 1 to User Element 9


		};
		//Fetch Accounting Schema against Records Org ID......................
		function getAccountingSchema(orgId, callback) {
			//    busyDiv("visible");
			$.ajax({
				url: VIS.Application.contextUrl + "AmountDivision/GetAccountingSchema",
				dataType: "json",
				data: { "OrgID": orgId },
				type: "POST",
				error: function () {
					busyDiv("hidden");
					alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
					return;
				},
				success: function (data) {
					var Sql = "";
					var DefaultValue = VIS.DB.executeScalar("select c_acctschema1_id from ad_clientinfo where ad_client_ID=" + VIS.Env.getCtx().getAD_Client_ID() + "");
					var defaultCheck = false;
					var res = JSON.parse(data);
					if (res.Error) {
						VIS.ADialog.error(res.Error);
						busyDiv("hidden");
						return;
					}
					cmbAcctSchema.empty().append('<option selected="selected" value="-1">Please select</option>');
					for (var i = 0; i < res.length; i++) {
						if (i == 0) {
							cmbAcctSchema.append('<option precision="' + Precision + '" value="0">ALL</option>');
						}
						if (DefaultValue == res[i]['Key']) {
							defaultCheck = true;
						}

						cmbAcctSchema.append($('<option precision="' + res[i]['Precision'] + '"></option>').val(res[i]['Key']).html(res[i]['Value']));
						allAcctSchemaID.push(res[i]['Key']);
					}

					if (defaultCheck) {

						var dr = VIS.DB.executeScalar("select cd.c_acctschema_id from c_dimamtaccttype cd inner join c_dimamtline cl on cd.c_dimamtaccttype_id=cl.c_dimamtaccttype_id where cd.c_dimamt_id=" + C_DimAmt_ID + "");
						if (dr == "" || dr == null) {
							cmbAcctSchema.val(DefaultValue);
							getDiminsionType(DefaultValue);
						}

					}

					if (callback) {
						busyDiv("visible");
						callback();
						busyDiv("hidden");
					}
					//busyDiv("hidden");
				}

			});

		};

		// Funcion to Generate dynamic control
		var getOrg = function (orgType) {
			generateControl.css({ "width": "33.3%" });
			modalGenerateControl.css({ "width": "33.3%" });
			divAmount.css("width", "33.3%");
			//divbutton.css("width", "6%");
			//modalDivAmount.css("width", "33.3%");
			lblOrg = $("<label>");
			modalLblOrg = $("<label>");
			var orgWhere = "";
			// Get Organizations based on element type selected
			if (orgType == "OO") {
				orgWhere = " AND IsSummary='N' AND IsCostCenter='N' AND IsProfitCenter='N'";
			}
			else {
				orgWhere = " AND IsSummary='N' AND (IsCostCenter='Y' OR IsProfitCenter='Y')";
			}
			var lookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "AD_Org_ID", 0, false, "AD_Org_ID<>0" + orgWhere);
			var modalLookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "AD_Org_ID", 0, false, "AD_Org_ID<>0" + orgWhere);
			var cmb = new VIS.Controls.VComboBox("Org_ID", false, false, true, lookup, 50);
			var modalCmb = new VIS.Controls.VComboBox("Org_ID", false, false, true, modalLookup, 50);
			cmbOrg = cmb.getControl().attr('placeholder', ' ').attr('data-placeholder', '');
			cmbOrg.find('option[value=0]').remove();
			cmbOrg.attr("tabindex", "4");
			modalCmbOrg = modalCmb.getControl().attr('placeholder', ' ').attr('data-placeholder', '');
			modalCmbOrg.attr("tabindex", "9");
			lblOrg.append(VIS.Msg.translate(VIS.Env.getCtx(), "Org_ID"));
			modalLblOrg.append(VIS.Msg.translate(VIS.Env.getCtx(), "Org_ID"));
			divOrg = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			divOrg.css("width", "100%");
			var divOrgCtrlWrap = $('<div class="vis-control-wrap">');
			modalDivOrg = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			modalDivOrg.css("width", "100%");
			var modalDivOrgCtrlWrap = $('<div class="vis-control-wrap">');
			divOrg.append(divOrgCtrlWrap);
			divOrgCtrlWrap.append(cmbOrg).append(lblOrg);
			generateControl.append(divOrg);
			modalDivOrg.append(modalDivOrg);
			modalDivOrg.append(modalCmbOrg).append(modalLblOrg);
			modalGenerateControl.append(modalDivOrg);
			cmbOrg.focus(); modalCmbOrg.focus();


		};
		var getActivity = function () {
			generateControl.css({ "width": "33.3%" });
			modalGenerateControl.css({ "width": "33.3%" });
			divAmount.css("width", "33.3%");
			//divbutton.css("width", "6%");
			//modalDivAmount.css("width", "33.3%");
			lblActivity = $("<label>");
			modalLblActivity = $("<label>");
			var lookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_Activity_ID", 0, false, null);
			var modalLookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_Activity_ID", 0, false, null);
			var cmb = new VIS.Controls.VComboBox("C_Activity_ID", false, false, true, lookup, 50);
			var modalCmb = new VIS.Controls.VComboBox("C_Activity_ID", false, false, true, modalLookup, 50);
			cmbActivity = cmb.getControl().attr('placeholder', ' ').attr('data-placeholder', '');
			cmbActivity.attr("tabindex", "4");
			modalCmbActivity = modalCmb.getControl().attr('placeholder', ' ').attr('data-placeholder', '');
			modalCmbActivity.attr("tabindex", "9");
			lblActivity.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_Activity_ID"));
			divActivity = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			divActivity.css("width", "100%");
			var divActivityCtrlWrap = $("<div class='vis-control-wrap'>");
			divActivity.append(divActivityCtrlWrap);
			divActivityCtrlWrap.append(cmbActivity).append(lblActivity);
			generateControl.append(divActivity);
			cmbActivity.focus();
			modalLblActivity.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_Activity_ID"));
			modalDivActivity = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			modalDivActivity.css("width", "100%");
			var modalDivActivityCtrlWrap = $("<div class='vis-control-wrap'>");
			modalDivActivity.append(modalDivActivityCtrlWrap);
			modalDivActivityCtrlWrap.append(modalCmbActivity).append(modalLblActivity);
			modalGenerateControl.append(modalDivActivity);
			modalCmbActivity.focus();
		};
		var getCampaign = function () {
			generateControl.css({ "width": "33.3%" });
			modalGenerateControl.css({ "width": "33.3%" });
			divAmount.css("width", "33.3%");
			//divbutton.css("width", "6%");
			//modalDivAmount.css("width", "33.3%");
			lblCampaign = $("<label>");
			modalLblCampaign = $("<label>");
			var lookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_Campaign_ID", 0, false, null);
			var modalLookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_Campaign_ID", 0, false, null);
			var cmb = new VIS.Controls.VComboBox("C_Campaign_ID", false, false, true, lookup, 50);
			var modalCmb = new VIS.Controls.VComboBox("C_Campaign_ID", false, false, true, modalLookup, 50);
			cmbCampaign = cmb.getControl().attr('placeholder', ' ').attr('data-placeholder', '');
			cmbCampaign.attr("tabindex", "4");
			modalCmbCampaign = modalCmb.getControl().attr('placeholder', ' ').attr('data-placeholder', '');
			modalCmbCampaign.attr("tabindex", "9");
			lblCampaign.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_Campaign_ID"));
			divCampaign = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			divCampaign.css("width", "100%");
			var divCampaignCtrlWrap = $("<div class='vis-control-wrap'>");
			divCampaign.append(divCampaignCtrlWrap);
			divCampaignCtrlWrap.append(cmbCampaign).append(lblCampaign);
			generateControl.append(divCampaign);
			cmbCampaign.focus();
			modalLblCampaign.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_Campaign_ID"));
			modalDivCampaign = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			modalDivCampaign.css("width", "100%");
			var modalDivCampaignCtrlWrap = $("<div class='vis-control-wrap'>");
			modalDivCampaign.append(modalDivCampaignCtrlWrap);
			modalDivCampaignCtrlWrap.append(modalCmbCampaign).append(modalLblCampaign);
			modalGenerateControl.append(modalDivCampaign);
			modalCmbCampaign.focus();
		};
		var getSalesRegion = function () {
			generateControl.css({ "width": "33.3%" });
			modalGenerateControl.css({ "width": "33.3%" });
			divAmount.css("width", "33.3%");
			//divbutton.css("width", "6%");
			//modalDivAmount.css("width", "33.3%");
			lblSalesRegion = $("<label>");
			modalLblSalesRegion = $("<label>");
			var lookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_SalesRegion_ID", 0, false, null);
			var cmb = new VIS.Controls.VComboBox("C_SalesRegion_ID", false, false, true, lookup, 50);
			cmbSalesRegion = cmb.getControl().attr('placeholder', ' ').attr('data-placeholder', '');
			cmbSalesRegion.attr("tabindex", "4");
			lblSalesRegion.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_SalesRegion_ID"));
			divSales = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			divSales.css("width", "100%");
			var divSalesCtrlWrap = $("<div class='vis-control-wrap'>");
			divSales.append(divSalesCtrlWrap);
			divSalesCtrlWrap.append(cmbSalesRegion).append(lblSalesRegion);
			generateControl.append(divSales);
			cmbSalesRegion.focus();
			var modalLookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_SalesRegion_ID", 0, false, null);
			var modalCmb = new VIS.Controls.VComboBox("C_SalesRegion_ID", false, false, true, modalLookup, 50);
			modalCmbSalesRegion = modalCmb.getControl().attr('placeholder', ' ').attr('data-placeholder', '');
			modalCmbSalesRegion.attr("tabindex", "9");
			modalLblSalesRegion.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_SalesRegion_ID"));
			modalDivSales = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			modalDivSales.css("width", "100%");
			var modalDivSalesCtrlWrap = $("<div class='vis-control-wrap'>");
			modalDivSales.append(modalDivSalesCtrlWrap);
			modalDivSalesCtrlWrap.append(modalCmbSalesRegion).append(modalLblSalesRegion);
			modalGenerateControl.append(modalDivSales);
			modalCmbSalesRegion.focus();
		};
		var getUserElement = function () {
			cmbUserElement = $("<select>");
			modalCmbUserElement = $("<select>");
			var sql = "select adt.ad_column_id,adt.columnname,adtab.TableName from c_acctschema_element ac inner join ad_column ad on ac.ad_column_id=ad.ad_column_id " +
				" inner join ad_column adt on ad.ad_table_ID=adt.ad_table_ID and adt.isactive='Y' " +
				"  inner join ad_table adtab on adtab.ad_table_id=ad.ad_table_ID " +
				" where ac.c_acctschema_id=" + arrAcctSchemaID[0] + " and ac.elementtype='" + cmbDimensionType.find("option:selected").val() + "' and adt.isidentifier='Y' order by adt.ad_column_ID";
			var dr = VIS.DB.executeReader(sql);
			var tblName = "";
			var colName = "";
			while (dr.read()) {
				tblName = dr.getString(2);
				if (colName == "") {
					colName += dr.getString(1);

				}
				else {

					colName += " ||'_'|| " + dr.getString(1);
				}
			}
			generateControl.css({ "width": "33.3%" });
			modalGenerateControl.css({ "width": "33.3%" });
			divAmount.css("width", "33.3%");
			//divbutton.css("width", "6%");
			//modalDivAmount.css("width", "33.3%");
			lblUserElement = $("<label>");
			cmbUserElement.empty().append('<option value=-1></option>');
			modalCmbUserElement.empty().append('<option value=-1></option>');
			if (tblName != "") {
				var sql = "SELECT " + tblName + "_ID ,(" + colName + ") as Name FROM " + tblName + " WHERE isactive='Y' ORDER BY " + colName;   // Order by Identifier
				sql = VIS.MRole.addAccessSQL(sql, tblName, VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RW);
				var drTbl = VIS.DB.executeReader(sql);
				while (drTbl.read()) {
					cmbUserElement.append('<option value=' + drTbl.getInt(0) + '>' + drTbl.getString(1) + '</option>');
					modalCmbUserElement.append('<option value=' + drTbl.getInt(0) + '>' + drTbl.getString(1) + '</option>');
				}
			}
			//var lookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "AD_Column_ID", 0, false, "AD_Column.IsKey='Y' AND AD_Column.IsActive='Y'");
			//var cmb = new VIS.Controls.VComboBox("AD_Column_ID", false, false, true, lookup, 50);
			//cmbUserElement = cmb.getControl();
			cmbUserElement.attr("tabindex", "4");
			lblUserElement.append(VIS.Msg.translate(VIS.Env.getCtx(), "AD_Column_ID"));
			divUserElement = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			divUserElement.css("width", "100%");
			var divUserElementCtrlWrap = $("<div class='vis-control-wrap'>");
			divUserElement.append(divUserElementCtrlWrap);
			divUserElementCtrlWrap.append(cmbUserElement).append(lblUserElement);
			generateControl.append(divUserElement);
			cmbUserElement.focus();
			modalLblUserElement = $("<label>");

			//var modalLookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "AD_Column_ID", 0, false, "AD_Column.IsKey='Y' AND AD_Column.IsActive='Y'");
			//var modalCmb = new VIS.Controls.VComboBox("AD_Column_ID", false, false, true, modalLookup, 50);
			//modalCmbUserElement = modalCmb.getControl();
			modalCmbUserElement.attr("tabindex", "9");
			modalLblUserElement.append(VIS.Msg.translate(VIS.Env.getCtx(), "AD_Column_ID"));
			modalDivUserElement = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			modalDivUserElement.css("width", "100%");
			var modalDivUserElementCtrlWrap = $("<div class='vis-control-wrap'>");
			modalDivUserElement.append(modalDivUserElementCtrlWrap);
			modalDivUserElementCtrlWrap.append(modalCmbUserElement).append(modalLblUserElement);
			modalGenerateControl.append(modalDivUserElement);
			modalCmbUserElement.focus();
		};
		var getBPartner = function () {
			generateControl.css({ "width": "33.3%" });
			modalGenerateControl.css({ "width": "33.3%" });
			divAmount.css("width", "33.3%");
			//divbutton.css("width", "6%");
			//modalDivAmount.css("width", "33.3%");
			lblBPartner = $("<label>");
			var lookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_BPartner_ID", 0, false, null);
			txtb = new VIS.Controls.VTextBoxButton("C_BPartner_ID", false, false, true, VIS.DisplayType.Search, lookup);
			cmbBPartner = txtb.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ');
			cmbBPartner.attr("tabindex", "4");
			lblBPartner.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_BPartner_ID"));
			divBPartner = $("<div class='VIS-AMTD-formData VIS-AMTD-InputBtns input-group vis-input-wrap'>");
			var divBPartnerCtrlWrap = $('<div class="vis-control-wrap">');
			var divBPartnerBtnWrap = $('<div class="input-group-append">');
			divBPartner.css("width", "100%");
			divBPartner.append(divBPartnerCtrlWrap);
			divBPartner.append(divBPartnerBtnWrap);
			divBPartnerCtrlWrap.append(cmbBPartner).append(lblBPartner);
			divBPartnerBtnWrap.append(txtb.getBtn(0));//.append(txtb.getBtn(1));
			generateControl.append(divBPartner);
			modalLblBPartner = $("<label>");
			var modalLookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_BPartner_ID", 0, false, null);
			modalTxtb = new VIS.Controls.VTextBoxButton("C_BPartner_ID", false, false, true, VIS.DisplayType.Search, modalLookup);
			modalCmbBPartner = modalTxtb.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ');
			modalCmbBPartner.attr("tabindex", "9");
			modalLblBPartner.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_BPartner_ID"));
			modalDivBPartner = $("<div class='VIS-AMTD-formData VIS-AMTD-InputBtns input-group vis-input-wrap'>");
			var modalDivBPartnerCtrlWrap = $('<div class="vis-control-wrap">');
			var modalDivBPartnerBtnWrap = $('<div class="input-group-append">');
			modalDivBPartner.css("width", "100%");
			modalDivBPartner.append(modalDivBPartnerCtrlWrap);
			modalDivBPartner.append(modalDivBPartnerBtnWrap);
			modalDivBPartnerCtrlWrap.append(modalCmbBPartner).append(modalLblBPartner);
			modalDivBPartnerBtnWrap.append(modalTxtb.getBtn(0));//.append(modalTxtb.getBtn(1));
			modalGenerateControl.append(modalDivBPartner);

		};
		var getAddress = function () {
			generateControl.css({ "width": "33.3%" });
			modalGenerateControl.css({ "width": "33.3%" });
			divAmount.css("width", "33.3%");
			//divbutton.css("width", "6%");
			//modalDivAmount.css("width", "33.3%");
			lblAddress = $("<label>");
			var lookups = new VIS.MLocationLookup(VIS.Env.getCtx(), windowNo);
			txtLoc = new VIS.Controls.VLocation("C_Location_ID", false, false, true, VIS.DisplayType.Location, lookups);
			locAddress = txtLoc.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ');
			locAddress.attr("tabindex", "4");
			lblAddress.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_Location_ID"));
			divLocation = $("<div class='VIS-AMTD-formData VIS-AMTD-InputBtns input-group vis-input-wrap'>");
			var divLocationCtrlWrap = $('<div class="vis-control-wrap">');
			var divLocationBtnWrap = $('<div class="input-group-append">');
			divLocation.css("width", "100%");
			divLocation.append(divLocationCtrlWrap);
			divLocation.append(divLocationBtnWrap);
			divLocationCtrlWrap.append(locAddress).append(lblAddress);
			divLocationBtnWrap.append(txtLoc.getBtn(1));
			generateControl.append(divLocation);
			modalLblAddress = $("<label>");
			var modelLookups = new VIS.MLocationLookup(VIS.Env.getCtx(), windowNo);
			modalTxtLoc = new VIS.Controls.VLocation("C_Location_ID", false, false, true, VIS.DisplayType.Location, modelLookups);
			modalLocAddress = modalTxtLoc.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ');
			modalLocAddress.attr("tabindex", "9");
			modalLblAddress.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_Location_ID"));
			modalDivLocation = $("<div class='VIS-AMTD-formData VIS-AMTD-InputBtns input-group vis-input-wrap'>");
			var modalDivLocationCtrlWrap = $('<div class="vis-control-wrap">');
			var modalDivLocationBtnWrap = $('<div class="input-group-append">');
			modalDivLocation.css("width", "100%");
			modalDivLocation.append(modalDivLocationCtrlWrap);
			modalDivLocation.append(modalDivLocationBtnWrap);
			modalDivLocationCtrlWrap.append(modalLocAddress).append(modalLblAddress);
			modalDivLocationBtnWrap.append(modalTxtLoc.getBtn(1));
			modalGenerateControl.append(modalDivLocation);
		};
		var getProject = function () {
			generateControl.css({ "width": "33.3%" });
			modalGenerateControl.css({ "width": "33.3%" });
			divAmount.css("width", "33.3%");
			//divbutton.css("width", "6%");
			//modalDivAmount.css("width", "33.3%");
			lblProject = $("<label>");
			var lookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_Project_ID", 0, false, null);
			txtProj = new VIS.Controls.VTextBoxButton("C_Project_ID", false, false, true, VIS.DisplayType.Search, lookup);
			txtProject = txtProj.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ');
			txtProject.attr("tabindex", "4");
			lblProject.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_Project_ID"));
			divProject = $("<div class='VIS-AMTD-formData VIS-AMTD-InputBtns input-group vis-input-wrap'>");
			var divProjectCtrlWrap = $('<div class="vis-control-wrap">');
			var divProjectBtnWrap = $('<div class="input-group-append">');
			divProject.css("width", "100%");
			divProject.append(divProjectCtrlWrap);
			divProject.append(divProjectBtnWrap);
			divProjectCtrlWrap.append(txtProject).append(lblProject);
			divProjectBtnWrap.append(txtProj.getBtn(0));//.append(txtProj.getBtn(1));
			generateControl.append(divProject);
			modalLblProject = $("<label>");
			var modalLookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_Project_ID", 0, false, null);
			modalTxtProj = new VIS.Controls.VTextBoxButton("C_Project_ID", false, false, true, VIS.DisplayType.Search, modalLookup);
			modalTxtProject = modalTxtProj.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ');
			modalTxtProject.attr("tabindex", "9");
			modalLblProject.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_Project_ID"));
			modalDivProject = $("<div class='VIS-AMTD-formData VIS-AMTD-InputBtns input-group vis-input-wrap'>");
			var modalDivProjectCtrlWrap = $('<div class="vis-control-wrap">');
			var modalDivProjectBtnWrap = $('<div class="input-group-append">');
			modalDivProject.css("width", "100%");
			modalDivProject.append(modalDivProjectCtrlWrap);
			modalDivProject.append(modalDivProjectBtnWrap);
			modalDivProjectCtrlWrap.append(modalTxtProject).append(modalLblProject);
			modalDivProjectBtnWrap.append(modalTxtProj.getBtn(0));//.append(modalTxtProj.getBtn(1));
			modalGenerateControl.append(modalDivProject);

		};
		var getProduct = function () {
			generateControl.css({ "width": "33.3%" });
			modalGenerateControl.css({ "width": "33.3%" });
			divAmount.css("width", "33.3%");
			////divbutton.css("width", "6%");
			//modalDivAmount.css("width", "33.3%");
			lblProduct = $("<label>");
			var lookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "M_Product_ID", 0, false, null);
			txtProd = new VIS.Controls.VTextBoxButton("M_Product_ID", false, false, true, VIS.DisplayType.Search, lookup);
			txtProduct = txtProd.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ');
			txtProduct.attr("tabindex", "4");
			lblProduct.append(VIS.Msg.translate(VIS.Env.getCtx(), "M_Product_ID"));
			divProduct = $("<div class='VIS-AMTD-formData VIS-AMTD-InputBtns input-group vis-input-wrap'>");
			var divProductCtrlWrap = $('<div class="vis-control-wrap">');
			var divProductBtnWrap = $('<div class="input-group-append">');
			divProduct.css("width", "100%");
			divProduct.append(divProductCtrlWrap);
			divProduct.append(divProductBtnWrap);
			divProductCtrlWrap.append(txtProduct).append(lblProduct);
			divProductBtnWrap.append(txtProd.getBtn(0));//.append(txtProd.getBtn(1));
			generateControl.append(divProduct);
			modalLblProduct = $("<label>");
			var modalLookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "M_Product_ID", 0, false, null);
			modalTxtProd = new VIS.Controls.VTextBoxButton("M_Product_ID", false, false, true, VIS.DisplayType.Search, modalLookup);
			modalTxtProduct = modalTxtProd.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ');
			modalTxtProduct.attr("tabindex", "9");
			modalLblProduct.append(VIS.Msg.translate(VIS.Env.getCtx(), "M_Product_ID"));
			modalDivProduct = $("<div class='VIS-AMTD-formData VIS-AMTD-InputBtns input-group vis-input-wrap'>");
			var modalDivProductCtrlWrap = $('<div class="vis-control-wrap">');
			var modalDivProductBtnWrap = $('<div class="input-group-append">');
			modalDivProduct.css("width", "100%");
			modalDivProduct.append(modalDivProductCtrlWrap);
			modalDivProduct.append(modalDivProductBtnWrap);
			modalDivProductCtrlWrap.append(modalTxtProduct).append(modalLblProduct);
			modalDivProductBtnWrap.append(modalTxtProd.getBtn(0));//.append(modalTxtProd.getBtn(1));
			modalGenerateControl.append(modalDivProduct);

		};
		var getAccountElement = function () {
			C_Element_ID = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "AmountDivision/GetElementID", { "accountingSchema": JSON.stringify(arrAcctSchemaID) }, null);
			if (C_Element_ID == 0) {
				IsElementOk = false;
				return false;
			}

			generateControl.css({ "width": "66.6%" });
			modalGenerateControl.css({ "width": "66.6%", "margin-right": "9px" });
			divAmount.css("width", "26.5%");
			//divbutton.css("width", "6%");
			modalDivAmount.css("width", "26.5%");
			lblElement = $("<label>");
			lblAccountElement = $("<label>");
			lblElement.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_Element_ID"));
			lblAccountElement.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_ElementValue_ID"));
			var lookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_Element_ID", 0, false, null);
			var cmb = new VIS.Controls.VComboBox("C_Element_ID", false, false, true, lookup, 50);
			cmbElement = cmb.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ');
			cmbElement.attr("tabindex", "4");
			divAccountElement = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			var divAccountElementCtrlWrap = $('<div class="vis-control-wrap">');
			divAccountElement.css("width", "50%");
			divAccountElement.append(divAccountElementCtrlWrap);
			divAccountElementCtrlWrap.append(cmbElement).append(lblElement);
			generateControl.append(divAccountElement);
			divAccountElement.hide();
			//cmbElement.focus();

			var look = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_ElementValue_ID", 0, false, "C_Element_ID=" + C_Element_ID);
			txtAcctElementValue = new VIS.Controls.VTextBoxButton("C_ElementValue_ID", false, false, true, VIS.DisplayType.Search, look);
			txtAccountElement = txtAcctElementValue.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ');
			txtAccountElement.attr("tabindex", "4");
			divAccountElementVal = $("<div class='VIS-AMTD-formData VIS-AMTD-InputBtns input-group vis-input-wrap'>");
			var divAccountElementValCtrlWrap = $('<div class="vis-control-wrap">');
			var divAccountElementValBtnWrap = $('<div class="input-group-append">');
			divAccountElementVal.css("width", "50%");
			divAccountElementVal.append(divAccountElementValCtrlWrap);
			divAccountElementVal.append(divAccountElementValBtnWrap);
			divAccountElementValCtrlWrap.append(txtAccountElement).append(lblAccountElement);
			divAccountElementValBtnWrap.append(txtAcctElementValue.getBtn(0));//.append(txtAcctElementValue.getBtn(1))

			generateControl.append(divAccountElementVal);
			cmbElement.on("change", function () {
				var look = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_ElementValue_ID", 0, false, "C_Element_ID=" + cmbElement.val());
				txtAccountElement.lookup = look;
			});
			txtAccountElement.focus();

			lblBPartner = $("<label>");
			var lookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_BPartner_ID", 0, false, null);
			txtb = new VIS.Controls.VTextBoxButton("C_BPartner_ID", false, false, true, VIS.DisplayType.Search, lookup);
			cmbBPartner = txtb.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ');
			cmbBPartner.attr("tabindex", "5");
			lblBPartner.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_BPartner_ID"));
			divBPartner = $("<div class='VIS-AMTD-formData VIS-AMTD-InputBtns input-group vis-input-wrap'>");
			var divBPartnerCtrlWrap = $('<div class="vis-control-wrap">');
			var divBPartnerBtnWrap = $('<div class="input-group-append">');
			divBPartner.css("width", "50%");
			divBPartner.append(divBPartnerCtrlWrap);
			divBPartner.append(divBPartnerBtnWrap);
			divBPartnerCtrlWrap.append(cmbBPartner).append(lblBPartner);
			divBPartnerBtnWrap.append(txtb.getBtn(0));//.append(txtb.getBtn(1));
			generateControl.append(divBPartner);

			if (cmbDimensionType.val() == "AC" || cmbDimensionType.val() == "U1" || cmbDimensionType.val() == "U2") {
				dGrid.showColumn("C_BPartner");
				divAccountElementVal.css("width", "49%");
				divBPartner.css("width", "49%").css("margin-left", '9px');
				divAmount.css("width", "26%");
				modalDivAmount.css("width", "26%");
			}
			else {
				divAccountElementVal.css("width", "51%");
				divBPartner.css("width", "49%").css("margin-left", '');
				divAmount.css("width", "26.5%");
				modalDivAmount.css("width", "26.5%");
			}

			modalLblElement = $("<label>");
			modalLblAccountElement = $("<label>");
			modalLblElement.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_Element_ID"));
			modalLblAccountElement.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_ElementValue_ID"));
			var modalLookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_Element_ID", 0, false, null);
			var modalCmb = new VIS.Controls.VComboBox("C_Element_ID", false, false, true, modalLookup, 50);
			modalCmbElement = modalCmb.getControl().attr('placeholder', ' ').attr('data-placeholder', '');
			modalCmbElement.attr("tabindex", "9");
			modalDivAccountElement = $("<div class='VIS-AMTD-formData input-group vis-input-wrap'>");
			var modalDivAccountElementCtrlWrap = $('<div class="vis-control-wrap">');
			modalDivAccountElement.css("width", "50%");
			modalDivAccountElement.append(modalDivAccountElementCtrlWrap);
			modalDivAccountElementCtrlWrap.append(modalCmbElement).append(modalLblElement);
			modalGenerateControl.append(modalDivAccountElement);
			modalDivAccountElement.hide();
			//modalCmbElement.focus();

			var modalLook = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_ElementValue_ID", 0, false, "C_Element_ID=" + C_Element_ID);
			modalTxtAcctElementValue = new VIS.Controls.VTextBoxButton("C_ElementValue_ID", false, false, true, VIS.DisplayType.Search, modalLook);
			modalTxtAccountElement = modalTxtAcctElementValue.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ');
			modalTxtAccountElement.attr("tabindex", "9");
			modalDivAccountElementVal = $("<div class='VIS-AMTD-formData VIS-AMTD-InputBtns input-group vis-input-wrap'>");
			var modalDivAccEleValCtrlWrap = $('<div class="vis-control-wrap">');
			var modalDivAccEleValBtnWrap = $('<div class="input-group-append">');
			modalDivAccountElementVal.css("width", "50%");
			modalDivAccountElementVal.append(modalDivAccEleValCtrlWrap);
			modalDivAccountElementVal.append(modalDivAccEleValBtnWrap);
			modalDivAccEleValCtrlWrap.append(modalTxtAccountElement).append(modalLblAccountElement);
			modalDivAccEleValBtnWrap.append(modalTxtAcctElementValue.getBtn(0));//.append(modalTxtAcctElementValue.getBtn(1))

			modalGenerateControl.append(modalDivAccountElementVal);
			modalCmbElement.on("change", function () {
				var modalLook = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_ElementValue_ID", 0, false, "C_Element_ID=" + modalCmbElement.val());
				modalTxtAccountElement.lookup = modalLook;
			});
			modalTxtAccountElement.focus();

			modalLblBPartner = $("<label>");
			var modalLookup = VIS.MLookupFactory.get(VIS.Env.getCtx(), windowNo, 0, VIS.DisplayType.TableDir, "C_BPartner_ID", 0, false, null);
			modalTxtb = new VIS.Controls.VTextBoxButton("C_BPartner_ID", false, false, true, VIS.DisplayType.Search, modalLookup);
			modalCmbBPartner = modalTxtb.getControl().attr('placeholder', ' ').attr('data-placeholder', '').attr('data-hasbtn', ' ');
			modalCmbBPartner.attr("tabindex", "10");
			modalLblBPartner.append(VIS.Msg.translate(VIS.Env.getCtx(), "C_BPartner_ID"));
			modalDivBPartner = $("<div class='VIS-AMTD-formData VIS-AMTD-InputBtns input-group vis-input-wrap'>");
			var modalDivBPartnerCtrlWrap = $('<div class="vis-control-wrap">');
			var modalDivBPartnerBtnWrap = $('<div class="input-group-append">');
			modalDivBPartner.css("width", "49%");
			modalDivBPartner.append(modalDivBPartnerCtrlWrap);
			modalDivBPartner.append(modalDivBPartnerBtnWrap);
			if (cmbDimensionType.val() == "AC" || cmbDimensionType.val() == "U1" || cmbDimensionType.val() == "U2") {
				modalDivAccountElementVal.css("width", "49%");
				modalDivBPartner.css("width", "48%").css("margin-left", '9px');
				modalDivAmount.css("width", "26%");
			}
			else {
				modalDivAccountElementVal.css("width", "51%");
				modalDivBPartner.css("width", "49%");
				modalDivAmount.css("width", "26.5%");
			}
			modalDivBPartnerCtrlWrap.append(modalCmbBPartner).append(modalLblBPartner);
			modalDivBPartnerBtnWrap.append(modalTxtb.getBtn(0));//.append(modalTxtb.getBtn(1));
			modalGenerateControl.append(modalDivBPartner);
		};
		//End of Generate dynamic control

		function getElements() {
			var ds = VIS.dataContext.getJSONData(VIS.Application.contextUrl + "AmountDivision/GetElementID", { "accountingSchema": JSON.stringify(arrAcctSchemaID) }, null);
		}

		//Fetch Dimension Type against Selected Accounting Schema.................
		function getDiminsionType(acctSchemaID, callback) {
			busyDiv("visible");
			$.ajax({
				url: VIS.Application.contextUrl + "AmountDivision/GetDiminsionType",
				dataType: "json",
				data: "{}",
				type: "POST",
				error: function () {
					alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
					busyDiv("hidden");
					return;
				},
				success: function (data) {
					var Sql = "";
					var res = JSON.parse(data);
					if (res.Error) {
						VIS.ADialog.error(res.Error);
						busyDiv("hidden");
						return;
					}
					if (acctSchemaID == 0) {
						for (var i = 0; i < allAcctSchemaID.length; i++) {
							if (i == 0) {
								Sql = "SELECT Distinct ElementType, Name FROM C_AcctSchema_Element WHERE  c_acctschema_id =" + allAcctSchemaID[i] + " AND ElementType NOT IN('SA','X1','X2','X3','X4','X5','X6','X7','X8','X9') ";
							}
							else {
								Sql += " AND ElementType IN(SELECT ElementType FROM C_AcctSchema_Element WHERE  c_acctschema_id =" + allAcctSchemaID[i] + " AND ElementType NOT IN('SA','X1','X2','X3','X4','X5','X6','X7','X8','X9')) ";
							}
						}
						Sql += " ORDER BY ElementType";

					}
					else {
						Sql = "SELECT Distinct ElementType, Name FROM C_AcctSchema_Element WHERE  c_acctschema_id =" + acctSchemaID + " AND ElementType<>'SA' ORDER BY ElementType";
					}
					Sql = VIS.MRole.addAccessSQL(Sql, "C_AcctSchema_Element", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RW);
					var dr = VIS.DB.executeReader(Sql);
					cmbDimensionType.empty().append('<option selected="selected" value="0">Please select</option>');
					while (dr.read()) {
						for (var i = 0; i < res.length; i++) {
							if (res[i]["Key"] == dr.getString(0)) {
								cmbDimensionType.append($("<option></option>").val(res[i]['Key']).html(dr.getString(1)));       // Displayed name from Accounting Schema elements
								break;
							}
						}
					}

					if (callback) {
						callback();
						//calculateGrossAmount(acctSchemaData[0].TotalLineAmount);
					}

					//$.each(dimensionType, function () {
					//    cmbDimensionType.append($("<option></option>").val(this['Key']).html(this['Value']));
					//});
					busyDiv("hidden");
				}

			});
		};

		//Show Data in Grid.............
		var loadData = function (data) {
			// busyDiv("visible");
			var grdCols = [];
			var grdRows = [];
			var rander = null;
			var displayType = null;
			grdCols.push({ field: "DimensionType", caption: "Dimension Type", size: "30%", min: 217 });
			grdCols.push({ field: "DimensionName", caption: "Dimension Name", size: "30%", min: 217 });
			grdCols.push({ field: "C_BPartner", caption: "Business Partner", size: "30%", min: 217, hidden: true });
			grdCols.push({
				field: "DimensionValueAmount", caption: "Dimension Value Amount", size: "30%", min: 216,
				render: function (record, index, col_index) {
					var val = VIS.Utility.Util.getValueOfDecimal(record["DimensionValueAmount"]);
					return (val).toLocaleString();
				}
			});
			grdCols.push({
				field: "Edit", caption: "", size: "5%", resizable: false,
				render: function () { return '<a  tabindex="8"><div><img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/Edit_sub16.png" alt="Edit record" title="Edit record" style="opacity: 1;cursor:pointer"></div></a>'; }
			});
			grdCols.push({
				field: "Delete", caption: "", size: "5%", resizable: false,
				render: function () { return '<a id="a1" tabindex="14"><div><img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/delete10.png" alt="Delete record" title="Delete record" style="opacity: 1;cursor:pointer"></div></a>'; }
			});
			grdCols.push({ field: "lineAmountID", caption: "lineAmountID", hidden: true, resizable: false, min: 0, max: 0 });
			grdCols.push({ field: "CalculateDimValAmt", caption: "CalculateDimValAmt", hidden: true, resizable: false, min: 0, max: 0 });
			grdCols.push({ field: "C_BPartner_ID", caption: "C_BPartner", hidden: true, resizable: false, min: 0, max: 0 });
			grdname = 'gridAmountDivision' + Math.random();
			grdname = grdname.replace('.', '');
			LineGridName = grdname;
			w2utils.encodeTags(grdRows);
			dGrid = $(datasec).w2grid({
				name: grdname,
				recordHeight: 20,
				show: {

					toolbar: false,  // indicates if toolbar is visible
					columnHeaders: true,   // indicates if columns is visible
					lineNumbers: false,  // indicates if line numbers column is visible
					selectColumn: false,  // indicates if select column is visible
					toolbarReload: false,   // indicates if toolbar reload button is visible
					toolbarColumns: true,   // indicates if toolbar columns button is visible
					toolbarSearch: false,   // indicates if toolbar search controls are visible
					toolbarAdd: false,   // indicates if toolbar add new button is visible
					toolbarDelete: false,   // indicates if toolbar delete button is visible
					toolbarSave: false,   // indicates if toolbar save button is visible
					selectionBorder: false,	 // display border arround selection (for selectType = 'cell')
					recordTitles: false	 // indicates if to define titles for records

				},
				columns: grdCols,
				records: data,
				onClick: function (event) {
					if (event.column == 4 && dGrid.records.length > 0) {
						checkDelete = event.recid;
						var templineID = dGrid.get(event.recid).lineAmountID;
						var arrlineId = [];
						if (templineID.search(',') == -1) {
							arrlineId[0] = templineID;
						}
						else {
							arrlineId = templineID.split(",");
						}
						editRecord(arrlineId, dGrid.get(event.recid).CalculateDimValAmt, event.recid);
					}
					if (event.column == 5 && dGrid.records.length > 0) {
						if (btnAdd.html() == VIS.Msg.getMsg("Update") && checkDelete == event.recid) {
							VIS.ADialog.warn("PleaseFirstUpdateThisRecord");
						}
						else {

							deleteRecord(dGrid.get(event.recid).lineAmountID, dGrid.get(event.recid).CalculateDimValAmt, event.recid);
						}

					}

				}
			});

			if (cmbDimensionType.val() == "AC" || cmbDimensionType.val() == "U1" || cmbDimensionType.val() == "U2") {
				dGrid.showColumn("C_BPartner");
			}
			// busyDiv("hidden");
		};

		//Delete Dimension Line From Array.................
		function deleteRecord(dimensionLineID, Amount, gridRecordID) {
			VIS.ADialog.confirm("DeleteConfirm", true, "", "Confirm", function (result) {
				if (result) {
					busyDiv("visible");
					VIS.DB.executeQuery("delete from c_dimamtline where c_dimamtline_id in(" + dimensionLineID + ")");
					var sql = "select nvl((sum(cd.amount)),0) as Amount from c_dimamtline cd inner join c_dimamtaccttype ct on cd.c_dimamt_id=ct.c_dimamt_id " +
						" and cd.c_dimamtaccttype_id=ct.c_dimamtaccttype_id " +
						" where cd.c_dimamt_id=" + C_DimAmt_ID + " and ct.c_acctSchema_id=" + arrAcctSchemaID[0] + "";
					var amount = VIS.DB.executeScalar(sql);
					VIS.DB.executeQuery("update c_dimamtaccttype set totaldimlineamout=" + amount + " where c_dimamt_id=" + C_DimAmt_ID + " and c_acctSchema_id=" + arrAcctSchemaID[0] + "");
					w2ui[LineGridName].select(gridRecordID);
					console.log("selected Rows : " + w2ui[LineGridName].getSelection().length);
					w2ui[LineGridName].delete(true);
					var temp = parseFloat((txtTotal.getValue() - parseFloat(Amount)).toFixed(Precision));
					txtTotal.setValue(temp);
					getMaxDimensionAmount();
					busyDiv("hidden");
				}
			});

		};
		//Edit Dimension Line........................
		function editRecord(LineID, Amount, gridRecordID) {
			busyDiv("visible");
			var tempData = [];
			modalDiv.css("display", "table");
			//divbutton.css("width", "15.5%");
			if (cmbDimensionType.val() == "AC" || cmbDimensionType.val() == "U1" || cmbDimensionType.val() == "U2") {
				// divAmount.css("width", "17.5%");
				modalDivAmount.css({ "width": "11%", "padding-right": "0" });
			}
			else {
				divAmount.css("width", "33.3%");
				if (modalDivAmount != undefined && modalDivAmount != null) {
					modalDivAmount.css("width", "33.3%");
				}
			}
			// btnAdd.empty().append(VIS.Msg.getMsg("Update"));
			checkValUpdate = true;
			arrAcctSchemaID = [];
			if (cmbAcctSchema.val() == 0) {
				arrAcctSchemaID = allAcctSchemaID;
			}
			else { arrAcctSchemaID[0] = cmbAcctSchema.val(); }
			$.ajax({
				dataType: "json",
				type: "POST",
				url: VIS.Application.contextUrl + "AmountDivision/GetDiminsionLine",
				data: { "accountingSchema": JSON.stringify(arrAcctSchemaID), "dimensionID": C_DimAmt_ID, "DimensionLineID": LineID[0], "pageNo": pageNoOrder, "pSize": PAGE_SIZE },
				success: function (result) {
					if (result) {
						tempData = JSON.parse(result);
						recid = gridRecordID;
						dimensionLineID = LineID.toString();
						var dimensionValue = cmbDimensionType.val();
						DimensionNameVal = tempData[0].DimensionNameVal;
						oldDimensionNameValue = tempData[0].DimensionNameVal;
						modalTxtAmount.setValue(tempData[0].CalculateDimValAmt);
						oldAmount = tempData[0].CalculateDimValAmt;
						oldBPartnerID = tempData[0].C_BPartner_ID;
						if (cmbDimensionType.val() == "AC") {
							//modalCmbElement.val(tempData[0].ElementID);
							C_Element_ID = tempData[0].ElementID;
							modalTxtAcctElementValue.setValue(DimensionNameVal);
							modalTxtb.setValue((tempData[0].C_BPartner_ID > 0 ? tempData[0].C_BPartner_ID : -1));
						}//Account
						else if (dimensionValue == "AY") { modalCmbActivity.focus(); modalCmbActivity.val(DimensionNameVal); }//Activity
						else if (dimensionValue == "BP") {
							modalTxtb.getControl(0).val(modalTxtb.getDisplay());
							modalTxtb.setValue(DimensionNameVal);
						}//BPartner
						else if (dimensionValue == "LF" || dimensionValue == "LT") {
							modalTxtLoc.getControl(0).val(modalTxtLoc.getDisplay());
							modalTxtLoc.setValue(DimensionNameVal);
						}//Location From//Location To
						else if (dimensionValue == "MC") { modalCmbCampaign.focus(); modalCmbCampaign.val(DimensionNameVal); }//Campaign
						else if (dimensionValue == "OO" || dimensionValue == "OT") { modalCmbOrg.focus(); modalCmbOrg.val(DimensionNameVal); }//Organization}//Org Trx
						else if (dimensionValue == "PJ") {
							modalTxtProj.getControl(0).val(modalTxtProj.getDisplay());
							modalTxtProj.setValue(DimensionNameVal);
						}//Project
						else if (dimensionValue == "PR") {
							modalTxtProd.getControl(0).val(modalTxtProd.getDisplay());
							modalTxtProd.setValue(DimensionNameVal);
						}//Product
						else if (dimensionValue == "SA") { }//Sub Account
						else if (dimensionValue == "SR") { modalCmbSalesRegion.focus(); modalCmbSalesRegion.val(DimensionNameVal); }//Sales Region
						else if (dimensionValue == "U1" || dimensionValue == "U2") {
							//modalCmbElement.focus();
							//modalCmbElement.val(editArrayData[0]["ElementID"]);
							//modalTxtAcctElementValue.getControl(0).val(modalTxtAcctElementValue.getDisplay());
							//modalTxtAcctElementValue.setValue(DimensionNameVal);
							C_Element_ID = tempData[0].ElementID;
							modalTxtAcctElementValue.setValue(DimensionNameVal);
							modalTxtb.setValue((tempData[0].C_BPartner_ID > 0 ? tempData[0].C_BPartner_ID : -1));
						}//User List 1//User List 2
						else if (dimensionValue == "X1" || dimensionValue == "X2" || dimensionValue == "X3" || dimensionValue == "X4" || dimensionValue == "X5" || dimensionValue == "X6" ||
							dimensionValue == "X7" || dimensionValue == "X8" || dimensionValue == "X9") { modalCmbUserElement.focus(); modalCmbUserElement.val(DimensionNameVal); }//User Element 1 to User Element 9
						busyDiv("hidden");
					}
				}
			});

		};

		function disposeComponent() {
			contentDiv = bsyDiv = cmbAcctSchema = cmbDimensionType = cmbOrg = txtTotalAmount = txtAmount = btnAdd = btnNew = btnOk = divDesign = null; checkDelete = null;
			lblAcctSchema = lblDimensionType = lblTotalAmount = lblOrg = lblAmount = lblTotal = txtTotal = storeDimensionData = null; recid = null; checkValUpdate = null;
			updateRecordId = null; indexValue = null; txtb = null; arrAcctSchemaID = null; arrDimensionEType = null; ctrlDiv = null;
			txtLoc = null;
			txtProj = null;
			txtProd = null;
			grdname = null;
			loadData = null;
			datasec = null;
			divOrg = null;
			divAccountElement = null;
			divAccountElementVal = null;
			divActivity = null;
			divBPartner = null;
			divLocation = null;
			divCampaign = null;
			divProduct = null;
			divProject = null;
			divSales = null;
			divUserElement = null;
			divAmount = null;
			gridBtnAdd = null;
			gridBtnDelete = null;
			demoCount = null;
			allAcctSchemaID = null;
			self = this;
			divbutton = null;
			modalDivButton = null;
			modalDiv = null;
			modalDivConent = null;
			modalSpanClose = null;
			divGrid = null;
			modalDivOrg = null;
			modalDivAccountElement = null;
			modalDivAccountElementVal = null;
			modalDivActivity = null;
			modalDivBPartner = null;
			modalDivLocation = null;
			modalDivCampaign = null;
			modalDivProduct = null;
			modalDivProject = null;
			modalDivSales = null;
			modalDivUserElement = null;
			modalDivAmount = null;
			modalGenerateControl = null;
			divStatusContainer = null;
			ulStatusdimension = null;
			liStatusFirst = null;
			liStatusPrev = null;
			cmbStausRecordCount = null;
			liStatusNext = null;
			liStatusLast = null;
			refreshCmbDimension = null;
			LineGridName = null;
			dimensionLineID = null;
			oldAmount = null;
			C_DimAmt_ID = null;
			PAGE_SIZE = null;
			pageNoOrder = null;
			lblMaxAmount = null;
			divMaxAmount = null;
			format = null;
			cmbValue = null;
			acctValue = null;
			unBindControl();
			if (dGrid != null) {
				dGrid.destroy();
			}
			dGrid = null;
			if (root != null) {
				root.dialog('destroy');
				root.remove();
			}
			root = null;
			this.disposeComponent = null;
		};
		//function onClosing() {
		//    disposeComponent();
		//};
		this.show = function () {
			displayDialog();
			ch.getRoot();
			busyDiv("visible");
			txtAmount.addVetoableChangeListener(this);
			modalTxtAmount.addVetoableChangeListener(this);
			getAccountingSchema(AD_Org_ID, function () {

				getDimensionLine(allAcctSchemaID, true, function (tempData) {

					loadData(tempData);

					arrAcctSchemaID[0] = cmbAcctSchema.val();
					getMaxDimensionAmount();
					if (C_DimAmt_ID == 0 || tempData.length == 0) {
						divAmount.css("display", "none");
						btnAdd.css("display", "none");
						ch.getRoot().parent().css("height", "390px");
					}
					else {
						divAmount.css("display", "block");
						btnAdd.css("display", "block");
						if (lblMaxAmount.html() == "") {
							ch.getRoot().parent().css("height", "448px");
						}
						else {
							ch.getRoot().parent().css("height", "469px");
						}
					}
					busyDiv("hidden");

				});


			});  //Id 0 is passed For testing Purpose......................
			//  window.setTimeout(function () {
			// var tempData = [];




			///}, 3000);

		};
		initializeComponent();

		var displayDialog = function () {
			ch.setContent(root);
			//ch.setHeight(445);
			ch.setWidth(750);
			ch.setTitle(VIS.Msg.getMsg("AmountDimension"));
			ch.setModal(true);
			ch.setEnableResize(false);
			ch.show();
			ch.hideButtons();
			//Disposing Everything on Close
			ch.onClose = function () {
				self.onClosing(C_DimAmt_ID);
				if (self.onClose) self.onClose();

			};

			//ch.onOkClick = function (e) {
			//    var acct = storeDimensionData.map(function (e) { return e.AcctSchema; });
			//    var acctSchema = acct.filter(function (itm, i, acct) {
			//        return i == acct.indexOf(itm);
			//    });
			//    var acctAmount = 0;
			//    var totalAmountCheck = true;
			//    var msg = "";
			//    for (var i = 0; i < acctSchema.length; i++) {
			//        var acctSchemaData = filterStoreDimensionData(storeDimensionData, acctSchema[i]);
			//        for (var j = 0; j < acctSchemaData.length; j++) {
			//            acctAmount += parseFloat(acctSchemaData[j]["DimensionValueAmount"]);
			//        }
			//        if (parseFloat(acctAmount) > parseFloat(txtTotalAmount.val())) {
			//            totalAmountCheck = false;
			//            var sql = "select Name from c_acctschema where c_acctschema_id=" + acctSchema[i];
			//            var dr = VIS.DB.executeReader(sql);
			//            var errorAcctSchema = "";
			//            while (dr.read()) {
			//                errorAcctSchema = dr.getString(0);
			//            }
			//            msg = "Dimension Amount is greater than Total Amount in '" + errorAcctSchema + "' Accounting Schema.";

			//            break;
			//        }

			//        acctAmount = 0;
			//    }
			//    if (totalAmountCheck) {

			//        if (onValidateOk()) {

			//            //insertDimensionAmount();
			//            if (!insertDimensionAmount()) {
			//                return false;
			//            }
			//        }
			//        else { return false; }
			//    }
			//    else {
			//        alert(msg);
			//        return false;
			//    }
			//};

			//ch.onCancelClick = function () {

			//};

		};

	};

	VIS.AmountDivision = AmountDivision;
})(VIS, jQuery);
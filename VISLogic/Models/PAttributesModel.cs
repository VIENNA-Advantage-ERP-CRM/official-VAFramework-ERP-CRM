using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class AttributesObjects
    {
        public bool IsReturnNull { get; set; }
        public string Error { get; set; }
        public string tableStucture = "";
        public string ControlList { get; set; }
        public int MAttributeSetID { get; set; }
        public bool IsCanCreate { get; set; }
        public bool IsCanEdit { get; set; }
    }

    public class AttributeInstance
    {
        public AttributeInstance()
        {
        }
        public int M_AttributeSetInstance_ID { get; set; }
        public string M_AttributeSetInstanceName { get; set; }
        public string attrCode = "";
        //property for genral attribute
        public string Description { get; set; }
        public string GenSetInstance { get; set; }
        public string Error { get; set; }
    }


    public class PAttributesModel
    {
        private VLogger log = VLogger.GetVLogger(typeof(PAttributesModel).FullName);

        //Dictionary<MAttribute, KeyValuePair<MAttributeInstance, MAttributeValue[]>> attributesList = new Dictionary<MAttribute, KeyValuePair<MAttributeInstance, MAttributeValue[]>>(4);

        public AttributesObjects LoadInit(int _M_AttributeSetInstance_ID, int _M_Product_ID, bool _productWindow, int windowNo, Ctx ctx, int AD_Column_ID, int window_ID, bool IsSOTrx, string IsInternalUse)
        {

            AttributesObjects obj = new AttributesObjects();

            MAttributeSet aset = null;
            MAttribute[] attributes = null;
            //	Get Model
            MAttributeSetInstance _masi = MAttributeSetInstance.Get(ctx, _M_AttributeSetInstance_ID, _M_Product_ID);
            MProduct _prd = new MProduct(ctx, _M_Product_ID, null);
            if (_masi == null)
            {
                obj.IsReturnNull = true;
                obj.Error = "No Model for M_AttributeSetInstance_ID=" + _M_AttributeSetInstance_ID + ", M_Product_ID=" + _M_Product_ID;
                return obj;
            }

            //	Get Attribute Set
            aset = _masi.GetMAttributeSet();
            //	Product has no Attribute Set
            if (aset == null)
            {
                obj.IsReturnNull = true;
                obj.Error = "PAttributeNoAttributeSet";
                return obj;
            }

            obj.MAttributeSetID = aset.Get_ID();

            //	Product has no Instance Attributes
            if (!_productWindow && !aset.IsInstanceAttribute())
            {
                obj.Error = "PAttributeNoInstanceAttribute";            // JID_0647: System is giving error [NPAttributeNoInstanceAttribute=] if no instance attribute
                return obj;
            }

            if (_productWindow)
            {
                attributes = aset.GetMAttributes(false);
            }
            else
            {
                attributes = aset.GetMAttributes(true);
            }

            // Check the User Role for Edit or Create access.
            obj.IsCanCreate = MRole.GetDefault(ctx).IsCanCreateAttribute();
            obj.IsCanEdit = MRole.GetDefault(ctx).IsCanEditAttribute();

            //Row 0
            obj.tableStucture = "<table class='vis-formouterwrpdiv' style='width: 100%;'><tr>";
            if (_productWindow)
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    obj.tableStucture = AddAttributeLine(attributes[i], _M_AttributeSetInstance_ID, true, false, windowNo, obj, i);
                }
            }
            else
            {
                var newEditContent = VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(ctx, "NewRecord"));
                // JID_1070: Enabled Create new checkbox on Attribute set Instance
                //if (_M_AttributeSetInstance_ID > 0)
                //{
                //    newEditContent = VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(ctx, "EditRecord"));
                //}
                //column 1
                //obj.tableStucture += "<td>";
                //obj.tableStucture += "<input type='checkbox' id='chkNewEdit_" + windowNo + "' ><label  class='VIS_Pref_Label_Font'>" + newEditContent + "</label>";
                //obj.tableStucture += "</td>";

                ////column 1
                //obj.tableStucture += "<td>";
                //obj.tableStucture += "<input type='checkbox' id='chkEdit_" + windowNo + "' ><label  class='VIS_Pref_Label_Font'>" + Msg.GetMsg(ctx, "EditRecord") + "</label>";
                //obj.tableStucture += "</td>";

                obj.tableStucture += "<td>";
                obj.tableStucture += "<div style='display: flex'>";
                obj.tableStucture += "<div id=cmdNew_" + windowNo + " class='input-group vis-input-wrap' style='width: 50%; float: left;'>";
                obj.tableStucture += "<div class='vis-control-wrap'>";
                obj.tableStucture += "<label class='vis-ec-col-lblchkbox'><input type='checkbox' style='float:left;' id=chkNewEdit_" + windowNo
                    + ">" + newEditContent + "</label></div></div>";
                obj.tableStucture += "<div class='input-group vis-input-wrap' style='width: 50%; float: left;'>";
                obj.tableStucture += "<div class='vis-control-wrap'>";
                obj.tableStucture += "<label id=lblEdit_" + windowNo + " class='vis-ec-col-lblchkbox'><input type='checkbox' id=chkEdit_" + windowNo
                    + ">" + Msg.GetMsg(ctx, "EditRecord") + "</label></div></div>";
                //obj.tableStucture += "<input type='checkbox' style='height: 31px;' id=chkEdit_" + windowNo + " >";
                obj.tableStucture += "</div>";
                obj.tableStucture += "</td>";
                obj.tableStucture += "</tr>";

                //column 2
                obj.tableStucture += "<tr'>";
                //obj.tableStucture += "<td></td>";

                //column 2
                obj.tableStucture += "<td>";
                obj.tableStucture += "<div class='input-group vis-input-wrap'>";
                obj.tableStucture += "<div class='vis-control-wrap'>";
                obj.tableStucture += "<button type='button' id='btnSelect_" + windowNo + "' role='button' aria-disabled='false'>"
                    + "<i class='vis vis-locator' style='padding: 0 6px'></i><span>"
                    + VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(ctx, "SelectExisting")) + "</span></button></div></div>";
                obj.tableStucture += "</td>";
                obj.tableStucture += "</tr>";

                //Change 20-May-2015 Bharat
                var label = Msg.Translate(ctx, "AttrCode");
                obj.tableStucture += "<tr>";
                obj.tableStucture += "<td>";
                obj.tableStucture += "<div class='input-group vis-input-wrap'>";
                obj.tableStucture += "<div class='vis-control-wrap'>";
                obj.tableStucture += "<input id='txtAttrCode_" + windowNo + "' value='' type='text'  placeholder=' ' data-placeholder=''>";
                obj.tableStucture += "<label id=lot_" + windowNo + "' class='VIS_Pref_Label_Font'>" + label + "</label></div></div>";
                //obj.tableStucture += "</td>";
                ////column 2
                //obj.tableStucture += "<td>";
                obj.tableStucture += "</td>";

                obj.tableStucture += "</tr>";

                //Row 1
                obj.tableStucture += "<tr>";
                //	All Attributes
                for (int i = 0; i < attributes.Length; i++)
                {
                    obj.tableStucture = AddAttributeLine(attributes[i], _M_AttributeSetInstance_ID, true, false, windowNo, obj, i);
                }
            }

            //	Lot
            if (!_productWindow && aset.IsLot())
            {
                //column 1
                var label = Msg.Translate(ctx, "Lot");
                obj.tableStucture += "<td>";
                obj.tableStucture += "<div class='input-group vis-input-wrap'>";
                obj.tableStucture += "<div class='vis-control-wrap'>";
                //obj.tableStucture += "</td>";
                ////column 2
                //obj.tableStucture += "<td>";
                obj.tableStucture += "<input id='txtLotString_" + windowNo + "' value='" + _masi.GetLot() + "' type='text' placeholder=' ' data-placeholder=''>";
                obj.tableStucture += "<label id=lot_" + windowNo + "' class='VIS_Pref_Label_Font'>" + label + "</label></div></div>";
                obj.tableStucture += "</td>";

                obj.tableStucture += "</tr>";

                //Row 1
                if (!IsSOTrx || IsInternalUse == "N" || window_ID == 191 || window_ID == 140)
                {
                    obj.tableStucture += "<tr>";
                    //column 1
                    label = Msg.Translate(ctx, "M_Lot_ID");
                    obj.tableStucture += "<td>";
                    obj.tableStucture += "<div class='input-group vis-input-wrap'>";
                    obj.tableStucture += "<div class='vis-control-wrap'>";
                    //obj.tableStucture += "</td>";


                    String sql = "SELECT M_Lot_ID, Name "
                        + "FROM M_Lot l "
                        + "WHERE EXISTS (SELECT M_Product_ID FROM M_Product p "
                            + "WHERE p.M_AttributeSet_ID=" + _masi.GetM_AttributeSet_ID()
                            + " AND p.M_Product_ID=l.M_Product_ID)";

                    KeyNamePair[] data = DB.GetKeyNamePairs(sql, true);
                    //column 2
                    //obj.tableStucture += "<td>";
                    obj.tableStucture += "<select id='cmbLot_" + windowNo + "'>";
                    obj.tableStucture += " <option selected value='" + 0 + "'></option>";
                    for (int i = 1; i < data.Length; i++)
                    {
                        if (Convert.ToInt32(data[i].Key) == _masi.GetM_Lot_ID())
                        {
                            obj.tableStucture += " <option selected value='" + data[i].Key + "' >" + data[i].Name + "</option>";
                        }
                        else
                        {
                            obj.tableStucture += " <option value='" + data[i].Key + "' >" + data[i].Name + "</option>";
                        }
                    }

                    obj.tableStucture += "</select>";
                    obj.tableStucture += "<label id=M_Lot_ID_" + windowNo + "' class='VIS_Pref_Label_Font'>" + label + "</label></div></div>";
                    obj.tableStucture += "</td>";
                    obj.tableStucture += "</tr>";


                    //Row 2
                    obj.tableStucture += "<tr>";

                    //	New Lot Button
                    if (_masi.GetMAttributeSet().GetM_LotCtl_ID() != 0)
                    {
                        if (MRole.GetDefault(ctx).IsTableAccess(MLot.Table_ID, false) && MRole.GetDefault(ctx).IsTableAccess(MLotCtl.Table_ID, false))
                        {
                            if (!_masi.IsExcludeLot(AD_Column_ID, IsSOTrx))//_windowNoParent
                            {
                                //column 1
                                //obj.tableStucture += "<td></td>";
                                //column 2
                                obj.tableStucture += "<td>";
                                obj.tableStucture += "<div class='input-group vis-input-wrap'>";
                                obj.tableStucture += "<div class='vis-control-wrap'>";
                                obj.tableStucture += "<button type='button' id='btnLot_" + windowNo + "' role='button' aria-disabled='false'><span >" + VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(ctx, "New")) + "</span></button>";
                                obj.tableStucture += "</div></div></td>";

                                obj.tableStucture += "</tr>";
                                //Row 3
                                obj.tableStucture += "<tr>";
                            }
                        }
                    }
                }
                //mZoom = new System.Windows.Forms.ToolStripMenuItem(Msg.GetMsg(Env.GetContext(), "Zoom"), Env.GetImageIcon("Zoom16.gif"));
                //mZoom.Click += new EventHandler(mZoom_Click);
                //ctxStrip.Items.Add(mZoom);
            }

            //	SerNo
            if (!_productWindow && aset.IsSerNo())
            {
                //column 1
                var label = Msg.Translate(ctx, "SerNo");
                obj.tableStucture += "<td>";
                obj.tableStucture += "<div class='input-group vis-input-wrap'>";
                obj.tableStucture += "<div class='vis-control-wrap'>";
                //obj.tableStucture += "</td>";

                //column 2
                // txtSerNo.Text = _masi.GetSerNo();
                //obj.tableStucture += "<td>";
                obj.tableStucture += "<input id='txtSerNo_" + windowNo + "' value='" + _masi.GetSerNo() + "' type='text' placeholder=' ' data-placeholder=''>";
                obj.tableStucture += "<label id=SerNo_" + windowNo + "' class='VIS_Pref_Label_Font'>" + label + "</label></div></div>";
                obj.tableStucture += "</td>";

                obj.tableStucture += "</tr>";

                //Row 1
                obj.tableStucture += "<tr>";

                //	New SerNo Button
                if (_masi.GetMAttributeSet().GetM_SerNoCtl_ID() != 0)
                {
                    if (MRole.GetDefault(ctx).IsTableAccess(MSerNoCtl.Table_ID, false))
                    {
                        if (!_masi.IsExcludeSerNo(AD_Column_ID, IsSOTrx))//_windowNoParent
                        {
                            //column 1
                            //obj.tableStucture += "<td></td>";
                            obj.tableStucture += "<td>";
                            obj.tableStucture += "<div class='input-group vis-input-wrap'>";
                            obj.tableStucture += "<div class='vis-control-wrap'>";
                            obj.tableStucture += "<button type='button' id='btnSerNo_" + windowNo + "' role='button' aria-disabled='false'><span >" + VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(ctx, "New")) + "</span></button>";
                            obj.tableStucture += "</div></div></td>";
                        }

                        obj.tableStucture += "</tr>";
                        //Row 2
                        obj.tableStucture += "<tr>";
                    }
                }
            }	//	SerNo

            ////	GuaranteeDate
            if (!_productWindow && aset.IsGuaranteeDate())
            {
                DateTime? dtpicGuaranteeDate = TimeUtil.AddDays(DateTime.Now, _prd.GetGuaranteeDays());
                if (_M_AttributeSetInstance_ID > 0)
                {
                    dtpicGuaranteeDate = _masi.GetGuaranteeDate();
                }
                // JID_1811: Need to rename lable from ""Gurantee Date" to "Expiration date"
                var label = Msg.Translate(ctx, "ExpirationDate");
                //Column 1
                obj.tableStucture += "<td>";
                obj.tableStucture += "<div class='input-group vis-input-wrap'>";
                obj.tableStucture += "<div class='vis-control-wrap'>";
                //obj.tableStucture += "</td>";
                //Column 2
                //obj.tableStucture += "<td>";
                //obj.tableStucture += "<input style='width: 100%;' value='" + String.Format("{0:yyyy-MM-dd}", dtpicGuaranteeDate) + "' type='date'  id='dtpicGuaranteeDate_" + windowNo + "' class='VIS_Pref_pass'/>";
                obj.tableStucture += "<input value='" + String.Format("{0:yyyy-MM-dd}", dtpicGuaranteeDate) + "' type='date'  id='dtpicGuaranteeDate_" + windowNo + "' >";
                obj.tableStucture += "<label id='guaranteeDate_" + windowNo + "' class='VIS_Pref_Label_Font'>" + label + "</label>";
                obj.tableStucture += "</div></div></td>";

                obj.tableStucture += "</tr>";
                //Row 2
                obj.tableStucture += "<tr>";
            }

            //string[] sep = new string[1];
            //sep[0] = "<tr>";
            //sep = obj.tableStucture.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            //if (sep.Length <= 3)
            //{
            //    obj.Error = "PAttributeNoInfo";
            //    obj.IsReturnNull = true;
            //    return null;
            //}

            //	New/Edit Window
            if (!_productWindow)
            {
                //chkNewEdit.IsChecked = _M_AttributeSetInstance_ID == 0;
            }

            //	Attrribute Set Instance Description
            //Column 1
            var label1 = Msg.Translate(ctx, "Description");
            //obj.tableStucture += "<td>";
            //obj.tableStucture += "<label style='padding-bottom: 10px; padding-right: 5px;' id='description_" + windowNo + "' class='VIS_Pref_Label_Font'>" + label1 + "</label>";
            //obj.tableStucture += "</td>";
            //Column 2
            obj.tableStucture += "<td>";
            obj.tableStucture += "<div class='input-group vis-input-wrap'>";
            obj.tableStucture += "<div class='vis-control-wrap'>";
            obj.tableStucture += "<input readonly  id='txtDescription_" + windowNo + "' value='" + _masi.GetDescription() + "' class='vis-ev-col-readonly' type='text'>";
            obj.tableStucture += "<label id='description_" + windowNo + "' class='VIS_Pref_Label_Font'>" + label1 + "</label>";
            obj.tableStucture += "</div>";
            obj.tableStucture += "</div>";
            obj.tableStucture += "</td>";

            obj.tableStucture += "</tr>";


            //Add Ok and Cancel button 
            //Last row
            obj.tableStucture += "<tr>";

            obj.tableStucture += "<td style='text-align:right'>";
            obj.tableStucture += "<button style='margin-bottom:0px;margin-top:0px; float:right' type='button' class='VIS_Pref_btn-2' style='float: right;'  id='btnCancel_" + windowNo + "' role='button' aria-disabled='false'>" + VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(ctx, "Cancel")) + "</button>";
            obj.tableStucture += "<button style='margin-bottom:0px;margin-top:0px; float:right; margin-right: 10px;' type='button' class='VIS_Pref_btn-2' style='float: right; margin-right: 10px;' id='btnOk_" + windowNo + "' role='button' aria-disabled='false'>" + VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(ctx, "OK")) + "</button>";
            obj.tableStucture += "</td>";
            obj.tableStucture += "</tr>";

            obj.tableStucture += "</table>";
            if (obj.ControlList != null)
            {
                if (obj.ControlList.Length > 1)
                    obj.ControlList = obj.ControlList.Substring(0, obj.ControlList.Length - 1); ;
            }
            return obj;
        }

        /// <summary>
        /// Get Attributes from Instance Atrributes
        /// </summary>
        /// <param name="_M_AttributeSetInstance_ID">Attribute Set Instance</param>
        /// <param name="_M_Product_ID"></param>
        /// <param name="_productWindow"></param>
        /// <param name="windowNo"></param>
        /// <param name="ctx"></param>
        /// <param name="AD_Column_ID"></param>
        /// <param name="attrcode"></param>
        /// <returns></returns>
        public Dictionary<String, String> GetAttribute(int _M_AttributeSetInstance_ID, int _M_Product_ID, bool _productWindow, int windowNo, Ctx ctx, int AD_Column_ID, string attrcode)
        {
            Dictionary<String, String> attrValues = new Dictionary<String, String>();

            StringBuilder sql = new StringBuilder();
            MAttributeSet aset = null;
            MAttribute[] attributes = null;
            string attrsetQry = "";
            int attributeSet = 0;
            MAttributeSetInstance _masi = MAttributeSetInstance.Get(ctx, _M_AttributeSetInstance_ID, _M_Product_ID);
            //	Get Attribute Set
            aset = _masi.GetMAttributeSet();
            //	Product has no Attribute Set
            if (aset == null)
            {
                Msg.GetMsg("PAttributeNoAttributeSet", null);
                return null; ;
            }
            string attrCodeQry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID = 559 AND ColumnName = 'Value'";
            int codeCount = Util.GetValueOfInt(DB.ExecuteScalar(attrCodeQry));
            bool hasValue = codeCount > 0 ? true : false;

            // JID_1388: On ASI Control if we select attribute form existing instance same is not showing on control.
            if (hasValue)
            {
                attrsetQry = @"SELECT M_AttributeSet_ID FROM M_AttributeSetInstance WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND Value='" + attrcode + "'";

            }
            else
            {
                attrsetQry = @"SELECT ats.M_AttributeSet_ID FROM M_ProductAttributes patr LEFT JOIN  M_AttributeSetInstance ats 
                        ON (patr.M_AttributeSetInstance_ID=ats.M_AttributeSetInstance_ID) where patr.AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND patr.UPC='" + attrcode + "'";
            }
            attributeSet = Util.GetValueOfInt(DB.ExecuteScalar(attrsetQry));
            if (attributeSet != aset.Get_ID())
            {
                return null;
            }

            ////	Product has no Instance Attributes
            //if (!_productWindow && !aset.IsInstanceAttribute())
            //{
            //    Dispatcher.BeginInvoke(() => Classes.ShowMessage.Error("PAttributeNoInstanceAttribute", null));
            //    //ADialog.error(m_WindowNo, this, "PAttributeNoInstanceAttribute");
            //    return;
            //}
            if (_productWindow)
            {
                attributes = aset.GetMAttributes(false);
                log.Fine("Product Attributes=" + attributes.Length);

            }
            else
            {
                attributes = aset.GetMAttributes(true);
            }

            if (attributes.Length > 0)
            {
                string attrQry = "";
                if (hasValue)
                {
                    attrQry = @"SELECT ats.M_Attribute_ID,ats.M_AttributeValue_ID,ats.Value,att.attributevaluetype FROM M_AttributeSetInstance ast INNER JOIN M_AttributeInstance ats 
                    ON (ast.M_AttributeSetInstance_ID=ats.M_AttributeSetInstance_ID) INNER JOIN M_Attribute att ON ats.M_Attribute_ID=att.M_Attribute_ID
                    WHERE ast.AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND ast.Value='" + attrcode + "' AND ast.M_AttributeSet_ID = " + _masi.GetM_AttributeSet_ID() + " Order By ats.M_Attribute_ID";
                }
                else
                {
                    attrQry = @"SELECT ats.M_Attribute_ID,ats.M_AttributeValue_ID,ats.Value,att.attributevaluetype FROM M_ProductAttributes patr INNER JOIN M_AttributeInstance ats 
                    ON (patr.M_AttributeSetInstance_ID=ats.M_AttributeSetInstance_ID) INNER JOIN M_attributesetinstance ast ON (patr.M_AttributeSetInstance_ID=ast.M_AttributeSetInstance_ID)
                    INNER JOIN M_Attribute att ON ats.M_Attribute_ID=att.M_Attribute_ID
                    WHERE patr.AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND patr.UPC='" + attrcode + "' AND ast.M_AttributeSet_ID = " + _masi.GetM_AttributeSet_ID() + " Order By ats.M_Attribute_ID";
                }
                DataSet ds = null;
                try
                {
                    ds = DB.ExecuteDataset(attrQry, null, null);
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                if (Util.GetValueOfString(ds.Tables[0].Rows[i]["AttributeValueType"]) == "L")
                                {
                                    attrValues[Util.GetValueOfString(ds.Tables[0].Rows[i]["M_Attribute_ID"])] = (Util.GetValueOfString(ds.Tables[0].Rows[i]["M_AttributeValue_ID"]));
                                }
                                else
                                {
                                    attrValues[Util.GetValueOfString(ds.Tables[0].Rows[i]["M_Attribute_ID"])] = (Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]));
                                }
                            }
                            ds.Dispose();
                        }
                        else
                        {
                            ds.Dispose();
                        }
                    }
                    else
                    {
                        ds.Dispose();
                    }
                }
                catch
                {

                }
                finally
                {
                    ds.Dispose();
                }
            }
            //if (attrValues.Count == 0)
            //{
            //    attrValues.Add("");
            //}
            return attrValues;
        }

        /// <summary>
        /// Get Attribute Set Instance Data
        /// </summary>
        /// <param name="_M_AttributeSetInstance_ID">Attribute Set Instance</param>
        /// <param name="_M_Product_ID">Product</param>
        /// <param name="_productWindow">IS Opened from Product Window</param>
        /// <param name="windowNo">Window No</param>
        /// <param name="ctx">Context</param>
        /// <param name="AD_Column_ID">Column ID</param>
        /// <param name="attrcode">Attribute Code</param>
        /// <returns>List of String Data</returns>
        public List<String> GetAttributeInstance(int _M_AttributeSetInstance_ID, int _M_Product_ID, bool _productWindow, int windowNo, Ctx ctx, int AD_Column_ID, string attrcode)
        {
            List<String> attrValues = new List<String>();

            StringBuilder sql = new StringBuilder();
            MAttributeSet aset = null;
            MAttribute[] attributes = null;
            string attrsetQry = "";
            int attributeSet = 0;
            MAttributeSetInstance _masi = MAttributeSetInstance.Get(ctx, _M_AttributeSetInstance_ID, _M_Product_ID);
            //	Get Attribute Set
            aset = _masi.GetMAttributeSet();
            //	Product has no Attribute Set
            if (aset == null)
            {
                Msg.GetMsg("PAttributeNoAttributeSet", null);
                return null; ;
            }
            string attrCodeQry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID = 559 AND ColumnName = 'Value'";
            int codeCount = Util.GetValueOfInt(DB.ExecuteScalar(attrCodeQry));
            bool hasValue = codeCount > 0 ? true : false;
            if (hasValue)
            {
                attrsetQry = @"SELECT M_AttributeSet_ID FROM M_AttributeSetInstance WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND Value='" + attrcode + "'";
            }
            else
            {
                attrsetQry = @"SELECT ats.M_AttributeSet_ID FROM M_ProductAttributes patr LEFT JOIN  M_AttributeSetInstance ats 
                        ON (patr.M_AttributeSetInstance_ID=ats.M_AttributeSetInstance_ID) where patr.UPC='" + attrcode + "'";
            }
            attributeSet = Util.GetValueOfInt(DB.ExecuteScalar(attrsetQry));
            if (attributeSet != aset.Get_ID())
            {
                return null;
            }

            // JID_1201: Sytem is not picking the serial no. , lot no. when record is selected from Select Exiting record button ASI control.
            if (!_productWindow && (aset.IsLot() || aset.IsSerNo() || aset.IsGuaranteeDate()))
            {
                if (hasValue)
                {
                    sql.Append("SELECT Lot,SerNo,GuaranteeDate ");
                }
                else
                {
                    sql.Append("SELECT ats.Lot,ats.SerNo,ats.GuaranteeDate ");
                }
            }	//	Lot
            //if (!_productWindow && aset.IsSerNo())
            //{
            //    sql.Append(" SELECT ats.SerNo");
            //}
            //if (!_productWindow && aset.IsGuaranteeDate())
            //{
            //    if (sql.Length > 0)
            //    {
            //        sql.Append(",ats.GuaranteeDate");
            //    }
            //    else
            //    {
            //        sql.Append("SELECT ats.GuaranteeDate");
            //    }
            //}	//	GuaranteeDate
            if (sql.Length > 0)
            {
                if (hasValue)
                {
                    sql.Append(@" FROM M_AttributeSetInstance WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND Value='" + attrcode + "'");
                }
                else
                {
                    sql.Append(@" FROM M_ProductAttributes patr INNER JOIN M_AttributeSetInstance ats ON (patr.m_attributesetinstance_id=ats.m_attributesetinstance_id) 
                                    WHERE patr.AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND patr.UPC='" + attrcode + "'");
                }
                DataSet ds1 = null;
                try
                {
                    ds1 = DB.ExecuteDataset(sql.ToString(), null, null);

                    if (ds1 != null)
                    {
                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            if (!_productWindow && aset.IsLot())
                            {
                                attrValues.Add(Util.GetValueOfString(ds1.Tables[0].Rows[0]["Lot"]));
                            }	//	Lot
                            else
                            {
                                attrValues.Add("");
                            }
                            if (!_productWindow && aset.IsSerNo())
                            {
                                attrValues.Add(Util.GetValueOfString(ds1.Tables[0].Rows[0]["SerNo"]));
                            }
                            else
                            {
                                attrValues.Add("");
                            }
                            if (!_productWindow && aset.IsGuaranteeDate())
                            {
                                attrValues.Add(Util.GetValueOfString(ds1.Tables[0].Rows[0]["GuaranteeDate"]));
                            }	//	GuaranteeDate
                            else
                            {
                                attrValues.Add("");
                            }
                            ds1.Dispose();
                        }
                        else
                        {
                            ds1.Dispose();
                        }
                    }
                    else
                    {
                        ds1.Dispose();
                    }
                }
                catch
                {
                    attrValues.Clear();
                }
                finally
                {
                    ds1.Dispose();
                }
            }
            if (attrValues.Count == 0)
            {
                attrValues.Add("");
                attrValues.Add("");
                attrValues.Add("");
            }
            return attrValues;
        }

        /// <summary>
        /// Table line structure
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="product"></param>
        /// <param name="readOnly"></param>
        private string AddAttributeLine(MAttribute attribute, int M_AttributeSetInstance_ID, bool product, bool readOnly, int windowNo, AttributesObjects obj, int count)
        {
            log.Fine(attribute + ", Product=" + product + ", R/O=" + readOnly);
            //Column 1
            obj.tableStucture += "<td>";
            obj.tableStucture += "<div class='input-group vis-input-wrap'>";
            obj.tableStucture += "<div class='vis-control-wrap'>";

            string lbl = "";

            if (product)
            {
                lbl += "<label id=" + attribute.GetName().Replace(" ", "") + "_" + windowNo + ">" + attribute.GetName() + "</label>";
            }
            else
            {
                lbl += "<label id=" + attribute.GetName().Replace(" ", "") + "_" + windowNo + "  >" + attribute.GetName() + "</label>";
            }
            //obj.tableStucture += "</td>";

            MAttributeInstance instance = attribute.GetMAttributeInstance(M_AttributeSetInstance_ID);

            if (MAttribute.ATTRIBUTEVALUETYPE_List.Equals(attribute.GetAttributeValueType()))
            {
                MAttributeValue[] values = attribute.GetMAttributeValues();
                //Column 2
                //obj.tableStucture += "<td>";
                if (readOnly)
                {
                    obj.tableStucture += "<select readonly id='cmb_" + count + "_" + windowNo + "' attribute_id = " + attribute.Get_ID() + ">";
                }
                else
                {
                    obj.tableStucture += "<select id='cmb_" + count + "_" + windowNo + "' attribute_id = " + attribute.Get_ID() + ">";
                }
                obj.ControlList += "cmb_" + count + "_" + windowNo + ",";
                bool found = false;

                for (int i = 0; i < values.Length; i++)
                {
                    //Set first value default empty
                    if (values[i] == null && i == 0)
                    {
                        obj.tableStucture += " <option value='0' > </option>";
                    }
                    else if (values[i] != null)
                    {
                        if (instance != null)
                        {
                            if (values[i].GetM_AttributeValue_ID() == instance.GetM_AttributeValue_ID())
                            {
                                obj.tableStucture += " <option selected value='" + values[i].GetM_AttributeValue_ID() + "' >" + values[i].GetName() + "</option>";
                            }
                            else
                            {
                                obj.tableStucture += " <option value='" + values[i].GetM_AttributeValue_ID() + "' >" + values[i].GetName() + "</option>";
                            }
                        }
                        else
                        {
                            obj.tableStucture += " <option value='" + values[i].GetM_AttributeValue_ID() + "' >" + values[i].GetName() + "</option>";
                        }
                    }
                }

                obj.tableStucture += "</select>";
                obj.tableStucture += lbl;
                obj.tableStucture += "</div></div></td>";

                if (found)
                {
                    log.Fine("Attribute=" + attribute.GetName() + " #" + values.Length + " - found: " + instance);
                }
                else
                {
                    log.Warning("Attribute=" + attribute.GetName() + " #" + values.Length + " - NOT found: " + instance);
                }

                if (instance != null)
                {
                }
                else
                {
                    log.Fine("Attribute=" + attribute.GetName() + " #" + values.Length + " no instance");
                }
            }
            else if (MAttribute.ATTRIBUTEVALUETYPE_Number.Equals(attribute.GetAttributeValueType()))
            {
                string value = null;
                if (instance != null)
                {
                    value = instance.GetValue();
                }
                //Column 2
                //obj.tableStucture += "<td>";
                //obj.tableStucture += "<div class='input-group vis-input-wrap'>";
                //obj.tableStucture += "<div class='vis-control-wrap'>";
                if (readOnly)
                {
                    obj.tableStucture += "<input placeholder=' ' data-placeholder='' class='' readonly id='txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo
                        + "' value='" + value + "' type='number' attribute_id = " + attribute.Get_ID() + ">";
                }
                else
                {
                    string addclass = "";
                    if (attribute.IsMandatory())
                    {
                        addclass += " vis-ev-col-mandatory ";
                    }

                    obj.tableStucture += "<input placeholder=' ' data-placeholder='' maxlength='40' class='" + addclass + "' id='txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo
                        + "' value='" + value + "' class='' type='number' attribute_id = " + attribute.Get_ID() + ">";
                }
                obj.ControlList += "txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo + ",";
                obj.tableStucture += lbl;
                obj.tableStucture += "</div></div></td>";
            }
            else	//	Text Field
            {
                string value = null;
                if (instance != null)
                {
                    value = instance.GetValue();
                }

                //Column 2
                //obj.tableStucture += "<td>";
                //obj.tableStucture += "<div class='input-group vis-input-wrap'>";
                //obj.tableStucture += "<div class='vis-control-wrap'>";
                if (readOnly)
                {
                    obj.tableStucture += "<input placeholder=' ' data-placeholder='' class='' readonly id='txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo
                        + "' value='" + value + "' type='text' attribute_id = " + attribute.Get_ID() + ">";
                }
                else
                {
                    string addclass = "";
                    if (attribute.IsMandatory())
                    {
                        addclass += " vis-ev-col-mandatory ";
                    }

                    obj.tableStucture += "<input placeholder=' ' data-placeholder='' maxlength='40' class='" + addclass + "' id='txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo
                        + "' value='" + value + "' class='' type='text' attribute_id = " + attribute.Get_ID() + ">";
                }
                obj.ControlList += "txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo + ",";
                obj.tableStucture += lbl;
                obj.tableStucture += "</div></div></td>";
            }

            obj.tableStucture += "</tr>";
            //Row Add
            obj.tableStucture += "<tr>";
            return obj.tableStucture;
        }

        public bool GetExcludeEntry(int productId, int adColumn, int windowNo, Ctx ctx)
        {
            bool exclude = true;
            VAdvantage.Model.MProduct product = VAdvantage.Model.MProduct.Get(ctx, productId);
            int M_AttributeSet_ID = product.GetM_AttributeSet_ID();
            if (M_AttributeSet_ID != 0)
            {
                VAdvantage.Model.MAttributeSet mas = VAdvantage.Model.MAttributeSet.Get(ctx, M_AttributeSet_ID);
                exclude = mas.ExcludeEntry(adColumn, ctx.IsSOTrx(windowNo));
            }
            return exclude;
        }

        /// <summary>
        /// Save or Update Attribute Set Instance
        /// </summary>
        /// <param name="windowNoParent">Window no of Parent Window on which control is opening</param>
        /// <param name="strLotStringC">Lot Number</param>
        /// <param name="strSerNoC">Serial Number</param>
        /// <param name="dtGuaranteeDateC">Guarantee Date</param>
        /// <param name="strAttrCodeC">Attribute Code</param>
        /// <param name="productWindow">The parent Window is Product Window or Not</param>
        /// <param name="mAttributeSetInstanceId">Attribute Set Instance ID</param>
        /// <param name="mProductId">Product ID</param>
        /// <param name="windowNo">Window No for Control</param>
        /// <param name="description">Description</param>
        /// <param name="isEdited">Attribute set Instance is Edited or Not</param>
        /// <param name="values">Values selected on the Controls</param>
        /// <param name="ctx">Context</param>
        /// <returns>M_AttributeSetInstance_ID, Attribute Description, Error Message</returns>
        public AttributeInstance SaveAttribute(int windowNoParent, string strLotStringC, string strSerNoC, string dtGuaranteeDateC, string strAttrCodeC,
           bool productWindow, int mAttributeSetInstanceId, int mProductId, int windowNo, string description, bool isEdited, List<KeyNamePair> values, Ctx ctx)
        {
            var editors = values;
            AttributeInstance obj = new AttributeInstance();
            String strLotString = "", strSerNo = "", strAttrCode = "", attrValue = "";
            int attributeID = 0, prdAttributes = 0, pAttribute_ID = 0, product_id = 0, attrCount = 0, m_attributeSetInstance_ID = 0;
            StringBuilder sql = new StringBuilder();
            StringBuilder qry = new StringBuilder();
            StringBuilder qryAttr = null;
            DataSet ds = null;
            DateTime? dtGuaranteeDate = null;
            bool _changed = false;
            Trx trx = null;
            try
            {
                trx = Trx.GetTrx("VPAttribute" + DateTime.Now.Ticks);

                if (!productWindow && !String.IsNullOrEmpty(strLotStringC))
                {
                    strLotString = strLotStringC;
                }	//	L

                if (!productWindow && !String.IsNullOrEmpty(strSerNoC))
                {
                    log.Fine("SerNo=" + strSerNoC);
                    strSerNo = strSerNoC;
                }

                if (!productWindow && !String.IsNullOrEmpty(dtGuaranteeDateC))
                {
                    dtGuaranteeDate = Util.GetValueOfDateTime(dtGuaranteeDateC);
                }	//	Gua

                if (!productWindow && !String.IsNullOrEmpty(strAttrCodeC))
                {
                    strAttrCode = strAttrCodeC;
                }
                if (String.IsNullOrEmpty(strAttrCode))
                {
                    ctx.SetContext(windowNoParent, "AttrCode", "");
                }
                else
                {
                    ctx.SetContext(windowNoParent, "AttrCode", strAttrCode);
                }

                MAttributeSet aset = null;
                MAttribute[] attributes = null;
                String mandatory = "";

                MProduct product = MProduct.Get(ctx, mProductId);

                if (isEdited)
                {
                    m_attributeSetInstance_ID = mAttributeSetInstanceId;
                }
                // JID_1070 : work done for Edit Attribute Set Inastance
                MAttributeSetInstance _masi = new MAttributeSetInstance(ctx, m_attributeSetInstance_ID, product.GetM_AttributeSet_ID(), trx);

                string attrCodeQry = "SELECT Count(AD_Column_ID) FROM AD_Column WHERE AD_Table_ID = 559 AND ColumnName = 'Value'";
                int codeCount = Util.GetValueOfInt(DB.ExecuteScalar(attrCodeQry));
                bool hasValue = codeCount > 0 ? true : false;
                aset = _masi.GetMAttributeSet();
                if (aset == null)
                {
                    trx.Rollback();
                    return null;
                }

                if (!productWindow && strAttrCode != "")
                {
                    qryAttr = new StringBuilder();
                    if (hasValue)
                    {
                        qryAttr.Append(@"SELECT Count(M_AttributeSetInstance_ID) FROM M_AttributeSetInstance WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND Value = '" + strAttrCode + "'");
                        prdAttributes = Util.GetValueOfInt(DB.ExecuteScalar(qryAttr.ToString()));
                        if (prdAttributes != 0)
                        {
                            qryAttr.Clear();
                            qryAttr.Append("SELECT M_AttributeSetInstance_ID FROM M_AttributeSetInstance WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND Value = '" + strAttrCode + "'");
                            attributeID = Util.GetValueOfInt(DB.ExecuteScalar(qryAttr.ToString()));
                        }
                    }
                    else
                    {
                        qryAttr.Append(@"SELECT Count(M_Product_ID) FROM M_Product prd LEFT JOIN M_ProductAttributes patr on (prd.M_Product_ID=patr.M_Product_ID) " +
                        " LEFT JOIN M_Manufacturer muf on (prd.M_Product_ID=muf.M_Product_ID) WHERE prd.AD_Client_ID = " + ctx.GetAD_Client_ID()
                        + " AND (patr.UPC = '" + strAttrCode + "' OR prd.UPC = '" + strAttrCode + "' OR muf.UPC = '" + strAttrCode + "')");
                        prdAttributes = Util.GetValueOfInt(DB.ExecuteScalar(qryAttr.ToString()));
                        if (prdAttributes != 0)
                        {
                            qryAttr.Clear();
                            qryAttr.Append("SELECT M_ProductAttributes_ID FROM M_ProductAttributes WHERE AD_Client_ID = " + ctx.GetAD_Client_ID() + " AND UPC = '" + strAttrCode + "'");
                            pAttribute_ID = Util.GetValueOfInt(DB.ExecuteScalar(qryAttr.ToString()));
                            if (pAttribute_ID != 0)
                            {
                                MProductAttributes patr = new MProductAttributes(ctx, pAttribute_ID, trx);
                                attributeID = patr.GetM_AttributeSetInstance_ID();
                                product_id = patr.GetM_Product_ID();
                            }
                        }
                    }
                    //"AND (patr.M_Product_ID = " + _M_Product_ID + " OR prd.M_Product_ID = " + _M_Product_ID + " OR muf.M_Product_ID = " + _M_Product_ID + ")";

                    _changed = true;
                }	//	Attribute Code

                if (!productWindow && aset.IsLot())
                {
                    log.Fine("Lot=" + strLotString);
                    String text = strLotString;
                    _masi.SetLot(text);
                    if (text == null || text.Length == 0)
                    {
                        sql.Append("ats.Lot is NULL");
                    }
                    else
                    {
                        sql.Append("UPPER(ats.Lot) = '" + text.ToUpper() + "'");
                    }
                    if (aset.IsLotMandatory() && (text == null || text.Length == 0))
                        mandatory += " - " + Msg.Translate(ctx, "Lot");
                    _changed = true;
                }	//	Lot
                if (!productWindow && aset.IsSerNo())
                {
                    log.Fine("SerNo=" + strSerNo);
                    String text = strSerNo;
                    string serQry = "";
                    _masi.SetSerNo(text);
                    _masi.SetSerNo(text);
                    if (text == null || text.Length == 0)
                    {
                        serQry = " ats.SerNo is NULL";
                    }
                    else
                    {
                        serQry = " UPPER(ats.SerNo) = '" + text.ToUpper() + "'";
                    }
                    if (sql.Length > 0)
                    {
                        sql.Append(" AND " + serQry);
                    }
                    else
                    {
                        sql.Append(serQry);
                    }
                    if (aset.IsSerNoMandatory() && (text == null || text.Length == 0))
                        mandatory += " - " + Msg.Translate(ctx, "SerNo");
                    _changed = true;
                }
                if (!productWindow && aset.IsGuaranteeDate())
                {
                    log.Fine("GuaranteeDate=" + dtGuaranteeDate);
                    DateTime? ts = dtGuaranteeDate;
                    _masi.SetGuaranteeDate(ts);
                    if (sql.Length > 0)
                    {
                        sql.Append(" AND ats.GuaranteeDate = " + GlobalVariable.TO_DATE(dtGuaranteeDate, true));
                    }
                    else
                    {
                        sql.Append(" ats.GuaranteeDate = " + GlobalVariable.TO_DATE(dtGuaranteeDate, true));
                    }
                    if (aset.IsGuaranteeDateMandatory() && ts == null)
                        mandatory += " - " + Msg.Translate(ctx, "GuaranteeDate");
                    _changed = true;
                }	//	GuaranteeDate

                attributes = aset.GetMAttributes(!productWindow);

                //if (sql.Length > 0)
                //{
                //    if (attributes.Length > 0)
                //    {
                //        sql.Insert(0, " AND ");
                //    }
                //    else
                //    {
                //        sql.Insert(0, " WHERE ");
                //    }
                //    //qry += sql;
                //}
                //sql.Append(" ORDER BY ats.M_AttributeSetInstance_ID");

                Dictionary<MAttribute, object> lst = new Dictionary<MAttribute, object>();
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (MAttribute.ATTRIBUTEVALUETYPE_List.Equals(attributes[i].GetAttributeValueType()))
                    {
                        object editor = editors[i];
                        MAttributeValue value = null;
                        if (Convert.ToInt32(editors[i].Key) > 0)
                        {
                            value = new MAttributeValue(ctx, Convert.ToInt32(editors[i].Key), trx);
                            value.SetName(editors[i].Name);
                        }
                        log.Fine(attributes[i].GetName() + "=" + value);
                        if (attributes[i].IsMandatory() && value == null)
                        {
                            mandatory += " - " + attributes[i].GetName();
                        }
                        lst[attributes[i]] = value;
                        //attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, value);
                    }
                    else if (MAttribute.ATTRIBUTEVALUETYPE_Number.Equals(attributes[i].GetAttributeValueType()))
                    {
                        object editor = editors[i].Name;
                        string value = Util.GetValueOfString(editor);
                        log.Fine(attributes[i].GetName() + "=" + value);
                        if (attributes[i].IsMandatory() && (value == null || value.Length == 0))
                        {
                            mandatory += " - " + attributes[i].GetName();
                        }
                        lst[attributes[i]] = value;
                        //attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, value);
                    }
                    else
                    {
                        object editor = editors[i].Name;
                        String value = Convert.ToString(editor);
                        log.Fine(attributes[i].GetName() + "=" + value);
                        if (attributes[i].IsMandatory() && (value == null || value.Length == 0))
                        {
                            mandatory += " - " + attributes[i].GetName();
                        }
                        lst[attributes[i]] = value;
                        //attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, value);
                    }
                    _changed = true;
                }

                // JID_1070 : work done for Edit Attribute Set Inastance
                //if (!isEdited)
                //{
                if (attributes.Length > 0)
                {
                    qry.Append(@"SELECT M_AttributeSetInstance_ID FROM (SELECT ats.M_AttributeSetInstance_ID, av.M_AttributeValue_ID,ats.M_AttributeSet_ID,au.Value,att.AttributeValueType FROM 
                        M_AttributeSetInstance ats INNER JOIN M_AttributeInstance au ON ats.M_AttributeSetInstance_ID=au.M_AttributeSetInstance_ID LEFT JOIN M_Attribute att 
                        ON au.M_Attribute_ID=att.M_Attribute_ID LEFT JOIN M_AttributeValue av ON au.M_AttributeValue_ID=av.M_AttributeValue_ID WHERE ats.M_AttributeSet_ID = "
                        + aset.GetM_AttributeSet_ID());     // + " AND ");
                    // Change done by mohit to consider M_Attribute_ID also with value in querry to restrict the duplicacy and updation of existing attribute set instance ids- asked by Mukesh sir.- 8 April 2019.
                    bool hasAttr = false;
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(Util.GetValueOfString(lst[attributes[i]])))
                        {
                            if (attrCount == 0)
                            {
                                qry.Append(" AND");
                            }

                            if (hasAttr && i < attributes.Length)
                            {
                                qry.Append(" OR");
                            }
                            attrCount++;
                            qry.Append("( au.Value = '" + Util.GetValueOfString(lst[attributes[i]]) + "' AND au.M_Attribute_ID=" + attributes[i].GetM_Attribute_ID() + " )");
                            hasAttr = true;
                        }
                    }

                    // JID_0929: On Create line form, Product having attribute with (Color + Garentee Date) IF we insert only with Garentee Date System do not add any value
                    if (attrCount == 0)
                    {
                        qry.Clear();
                        qry.Append(@"SELECT ats.M_AttributeSetInstance_ID FROM M_AttributeSetInstance ats WHERE ats.M_AttributeSet_ID = " + aset.GetM_AttributeSet_ID());
                    }
                }
                else
                {
                    qry.Append(@"SELECT ats.M_AttributeSetInstance_ID FROM M_AttributeSetInstance ats WHERE ats.M_AttributeSet_ID = " + aset.GetM_AttributeSet_ID());
                }

                // if Lot, Serial or Guarantee Date id selected
                if (sql.Length > 0)
                {
                    //if (attributes.Length > 0 && attrCount > 0)
                    //{
                    //    sql.Insert(0, " AND ");
                    //}
                    //else
                    //{
                    //    sql.Insert(0, " WHERE ");
                    //}

                    sql.Insert(0, " AND ");
                }

                sql.Append(" ORDER BY ats.M_AttributeSetInstance_ID");
                qry.Append(sql.ToString());

                if (attributes.Length > 0 && attrCount > 0)
                {
                    qry.Append(",au.M_Attribute_ID) t GROUP BY M_AttributeSetInstance_ID HAVING Count(M_AttributeSetInstance_ID) = " + attrCount);
                }

                ds = DB.ExecuteDataset(qry.ToString(), null, trx);
                //}

                if (_changed)
                {
                    if (mandatory.Length > 0)
                    {
                        obj.Error = Msg.GetMsg(ctx, "FillMandatory") + mandatory;
                        trx.Rollback();
                        return obj;
                    }

                    // JID_1070 : work done for Edit Attribute Set Inastance
                    //if (isEdited)
                    //{
                    //    int no = DB.ExecuteQuery("DELETE FROM M_AttributeInstance WHERE M_AttributeSetInstance_ID = " + mAttributeSetInstanceId, null, trx);
                    //}
                    //else
                    //{
                    mAttributeSetInstanceId = 0;

                    if (attributes.Length > 0)
                    {
                        if (ds != null)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                int attCount = 0;
                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    qry.Clear();
                                    qry.Append("SELECT Count(M_AttributeSetInstance_ID) FROM M_AttributeInstance WHERE M_AttributeSetInstance_ID = "
                                        + Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]));
                                    attCount = Util.GetValueOfInt(DB.ExecuteScalar(qry.ToString(), null, trx));
                                    if (attCount == attrCount)
                                    {
                                        mAttributeSetInstanceId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                mAttributeSetInstanceId = 0;
                            }
                            ds.Dispose();
                        }
                        else
                        {
                            mAttributeSetInstanceId = 0;
                        }
                    }
                    else
                    {
                        if (ds != null)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    mAttributeSetInstanceId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                    break;
                                }
                            }
                            else
                            {
                                mAttributeSetInstanceId = 0;
                            }
                            ds.Dispose();
                        }
                        else
                        {
                            mAttributeSetInstanceId = 0;
                        }
                    }
                    //}

                    if (mAttributeSetInstanceId == 0)
                    {
                        if (isEdited)
                        {
                            int no = DB.ExecuteQuery("DELETE FROM M_AttributeInstance WHERE M_AttributeSetInstance_ID = " + m_attributeSetInstance_ID, null, trx);
                        }

                        if (hasValue && strAttrCode != "" && attributeID == 0)
                        {
                            _masi.Set_Value("Value", strAttrCode);
                        }

                        // Create new Attribute Set Instance in * Organization
                        _masi.SetAD_Org_ID(0);
                        if (!_masi.Save())
                        {
                            obj.Error = Msg.GetMsg(ctx, "NotSaved") + " - " + MAttributeSetInstance.Table_Name;
                            trx.Rollback();
                        }
                        mAttributeSetInstanceId = _masi.GetM_AttributeSetInstance_ID();
                        obj.M_AttributeSetInstance_ID = _masi.GetM_AttributeSetInstance_ID();
                        obj.M_AttributeSetInstanceName = _masi.GetDescription();
                    }

                    else
                    {
                        _masi = new MAttributeSetInstance(ctx, mAttributeSetInstanceId, trx);
                    }

                    for (int i = 0; i < attributes.Length; i++)
                    {
                        if (MAttribute.ATTRIBUTEVALUETYPE_List.Equals(attributes[i].GetAttributeValueType()))
                        {
                            MAttributeValue value = lst[attributes[i]] != null ? lst[attributes[i]] as MAttributeValue : null;
                            if (value == null)
                            {
                                continue;
                            }
                            attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, value);
                        }
                        else if (MAttribute.ATTRIBUTEVALUETYPE_Number.Equals(attributes[i].GetAttributeValueType()))
                        {
                            if (Convert.ToDecimal(lst[attributes[i]]) == 0)
                            {
                                continue;
                            }
                            attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, Convert.ToDecimal(lst[attributes[i]]));
                        }
                        else
                        {
                            if ((String)lst[attributes[i]] == "")
                            {
                                continue;
                            }
                            attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, (String)lst[attributes[i]]);
                        }
                    }

                    if (hasValue)
                    {
                        sql.Clear();
                        sql.Append("SELECT M_ProductAttributes_ID FROM M_ProductAttributes WHERE M_AttributeSetInstance_ID = " + mAttributeSetInstanceId + " AND M_Product_ID = "
                            + mProductId + " AND UPC IS NULL");
                        pAttribute_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));

                        if (pAttribute_ID == 0)
                        {
                            MProductAttributes pAttr = new MProductAttributes(ctx, 0, trx);
                            pAttr.SetAD_Org_ID(product.GetAD_Org_ID());
                            pAttr.SetUPC("");
                            pAttr.SetM_Product_ID(mProductId);
                            pAttr.SetM_AttributeSetInstance_ID(mAttributeSetInstanceId);
                            if (!pAttr.Save())
                            {
                                obj.Error = Msg.GetMsg(ctx, "NotSaved") + " - " + MProductAttributes.Table_Name;
                                trx.Rollback();
                            }

                        }

                        sql.Clear();
                        sql.Append("SELECT Value FROM M_AttributeSetInstance WHERE M_AttributeSetInstance_ID = " + mAttributeSetInstanceId);
                        attrValue = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString()));
                        //
                        if (attributeID != 0 && attributeID != mAttributeSetInstanceId)
                        {
                            obj.Error = Msg.GetMsg(ctx, "AttributeCodeExists");
                        }
                        else if (strAttrCode != "" && attrValue == "")
                        {
                            _masi.Set_Value("Value", strAttrCode);
                        }
                        _masi.SetDescription();
                        if (!_masi.Save())
                        {
                            obj.Error = Msg.GetMsg(ctx, "NotSaved") + " - " + MAttributeSetInstance.Table_Name;
                            trx.Rollback();
                        }

                        obj.M_AttributeSetInstance_ID = _masi.GetM_AttributeSetInstance_ID();
                        obj.M_AttributeSetInstanceName = _masi.GetDescription();
                        obj.attrCode = strAttrCode;
                    }
                    else
                    {
                        if (attributeID == 0 && strAttrCode != "")
                        {
                            MProductAttributes pAttr = new MProductAttributes(ctx, 0, trx);
                            pAttr.SetAD_Org_ID(product.GetAD_Org_ID());
                            pAttr.SetUPC(strAttrCode);
                            pAttr.SetM_Product_ID(mProductId);
                            pAttr.SetM_AttributeSetInstance_ID(mAttributeSetInstanceId);
                            if (!pAttr.Save())
                            {
                                obj.Error = Msg.GetMsg(ctx, "NotSaved") + " - " + MProductAttributes.Table_Name;
                                trx.Rollback();
                            }
                        }
                        _masi.SetDescription();
                        if (!_masi.Save())
                        {
                            obj.Error = Msg.GetMsg(ctx, "NotSaved") + " - " + MAttributeSetInstance.Table_Name;
                            trx.Rollback();
                        }
                        mAttributeSetInstanceId = _masi.GetM_AttributeSetInstance_ID();
                        obj.M_AttributeSetInstance_ID = _masi.GetM_AttributeSetInstance_ID();
                        obj.M_AttributeSetInstanceName = _masi.GetDescription();
                        obj.attrCode = strAttrCode;
                        //
                        if (attributeID != 0 && (attributeID != mAttributeSetInstanceId || product_id != mProductId))
                        {
                            obj.Error = Msg.GetMsg(ctx, "AttributeCodeExists");
                        }
                    }
                }
                else
                {
                    obj.M_AttributeSetInstance_ID = _masi.GetM_AttributeSetInstance_ID();
                    obj.M_AttributeSetInstanceName = _masi.GetDescription();
                    obj.attrCode = strAttrCode;
                }
                trx.Commit();
            }
            catch (Exception e)
            {
                if (trx != null)
                {
                    trx.Rollback();
                }
            }
            finally
            {
                if (trx != null)
                {
                    trx.Close();
                }
            }
            return obj;
        }

        // Change GSI Barcode
        public AttributeInstance SaveAttributeMR(int windowNoParent, string strLotStringC, string strSerNoC, string dtGuaranteeDateC, string strAttrCodeC,
          bool productWindow, int mAttributeSetInstanceId, int mProductId, int windowNo, List<KeyNamePair> values, Ctx ctx)
        {
            var editors = values;
            AttributeInstance obj = new AttributeInstance();
            String strLotString = "", strSerNo = "", strAttrCode = "", attrValue = "";
            int attributeID = 0, prdAttributes = 0, pAttribute_ID = 0, product_id = 0, attrCount = 0;
            StringBuilder sql = new StringBuilder();
            string qry = "";
            StringBuilder qryAttr = null;
            DataSet ds = null;
            DateTime? dtGuaranteeDate = null;
            bool _changed = false;

            if (!productWindow && strLotStringC != null)
            {
                strLotString = strLotStringC;
            }	//	L

            if (!productWindow && strSerNoC != null)
            {
                log.Fine("SerNo=" + strSerNoC);
                strSerNo = strSerNoC;
            }

            if (!productWindow && dtGuaranteeDateC != null)
            {
                if (dtGuaranteeDateC != "")
                {
                    dtGuaranteeDate = Convert.ToDateTime(dtGuaranteeDateC);
                }
            }	//	Gua

            if (!productWindow && strAttrCodeC != null)
            {
                strAttrCode = strAttrCodeC;
            }
            if (String.IsNullOrEmpty(strAttrCode))
            {
                ctx.SetContext(windowNoParent, "AttrCode", "");
            }
            else
            {
                ctx.SetContext(windowNoParent, "AttrCode", strAttrCode);
            }

            MAttributeSet aset = null;
            MAttribute[] attributes = null;
            String mandatory = "";
            var _masi = MAttributeSetInstance.Get(ctx, 0, mProductId);
            MProduct product = MProduct.Get(ctx, mProductId);
            string attrCodeQry = "SELECT Count(*) FROM AD_Column WHERE AD_Table_ID = 559 AND ColumnName = 'Value'";
            int codeCount = Util.GetValueOfInt(DB.ExecuteScalar(attrCodeQry));
            bool hasValue = codeCount > 0 ? true : false;
            aset = _masi.GetMAttributeSet();
            if (aset == null)
            {
                return null;
            }

            if (!productWindow && strAttrCode != "")
            {
                qryAttr = new StringBuilder();
                if (hasValue)
                {
                    qryAttr.Append(@"SELECT count(*) FROM M_AttributeSetInstance WHERE Value = '" + strAttrCode + "'");
                    prdAttributes = Util.GetValueOfInt(DB.ExecuteScalar(qryAttr.ToString()));
                    if (prdAttributes != 0)
                    {
                        qryAttr.Clear();
                        qryAttr.Append("SELECT M_AttributeSetInstance_ID FROM M_AttributeSetInstance WHERE Value = '" + strAttrCode + "'");
                        attributeID = Util.GetValueOfInt(DB.ExecuteScalar(qryAttr.ToString()));
                    }
                }
                else
                {
                    qryAttr.Append(@"SELECT count(*) FROM M_Product prd LEFT JOIN M_ProductAttributes patr on (prd.M_Product_ID=patr.M_Product_ID) " +
                    " LEFT JOIN M_Manufacturer muf on (prd.M_Product_ID=muf.M_Product_ID) WHERE (patr.UPC = '" + strAttrCode + "' OR prd.UPC = '" + strAttrCode + "' OR muf.UPC = '" + strAttrCode + "')");
                    prdAttributes = Util.GetValueOfInt(DB.ExecuteScalar(qryAttr.ToString()));
                    if (prdAttributes != 0)
                    {
                        qryAttr.Clear();
                        qryAttr.Append("SELECT M_ProductAttributes_ID FROM M_ProductAttributes WHERE UPC = '" + strAttrCode + "'");
                        pAttribute_ID = Util.GetValueOfInt(DB.ExecuteScalar(qryAttr.ToString()));
                        if (pAttribute_ID != 0)
                        {
                            MProductAttributes patr = new MProductAttributes(ctx, pAttribute_ID, null);
                            attributeID = patr.GetM_AttributeSetInstance_ID();
                            product_id = patr.GetM_Product_ID();
                        }
                    }
                }
                //"AND (patr.M_Product_ID = " + _M_Product_ID + " OR prd.M_Product_ID = " + _M_Product_ID + " OR muf.M_Product_ID = " + _M_Product_ID + ")";

                _changed = true;
            }	//	Attribute Code

            if (!productWindow && aset.IsLot())
            {
                log.Fine("Lot=" + strLotString);
                String text = strLotString;
                _masi.SetLot(text);
                if (text == null || text.Length == 0)
                {
                    sql.Append("ats.Lot is NULL");
                }
                else
                {
                    sql.Append("UPPER(ats.Lot) = '" + text.ToUpper() + "'");
                }
                if (aset.IsLotMandatory() && (text == null || text.Length == 0))
                    mandatory += " - " + Msg.Translate(ctx, "Lot");
                _changed = true;
            }	//	Lot
            if (!productWindow && aset.IsSerNo())
            {
                log.Fine("SerNo=" + strSerNo);
                String text = strSerNo;
                string serQry = "";
                _masi.SetSerNo(text);
                _masi.SetSerNo(text);
                if (text == null || text.Length == 0)
                {
                    serQry = " ats.SerNo is NULL";
                }
                else
                {
                    serQry = " UPPER(ats.SerNo) = '" + text.ToUpper() + "'";
                }
                if (sql.Length > 0)
                {
                    sql.Append(" and " + serQry);
                }
                else
                {
                    sql.Append(serQry);
                }
                if (aset.IsSerNoMandatory() && (text == null || text.Length == 0))
                    mandatory += " - " + Msg.Translate(ctx, "SerNo");
                _changed = true;
            }
            if (!productWindow && aset.IsGuaranteeDate())
            {
                log.Fine("GuaranteeDate=" + dtGuaranteeDate);
                DateTime? ts = dtGuaranteeDate;
                _masi.SetGuaranteeDate(ts);
                if (sql.Length > 0)
                {
                    sql.Append(" AND ats.GuaranteeDate = " + GlobalVariable.TO_DATE(dtGuaranteeDate, true));
                }
                else
                {
                    sql.Append(" ats.GuaranteeDate = " + GlobalVariable.TO_DATE(dtGuaranteeDate, true));
                }
                if (aset.IsGuaranteeDateMandatory() && ts == null)
                    mandatory += " - " + Msg.Translate(ctx, "GuaranteeDate");
                _changed = true;
            }	//	GuaranteeDate

            attributes = aset.GetMAttributes(!productWindow);

            if (sql.Length > 0)
            {
                if (attributes.Length > 0)
                {
                    sql.Insert(0, " AND ");
                }
                else
                {
                    sql.Insert(0, " WHERE ");
                }
                qry += sql;
            }
            sql.Append(" ORDER BY ats.m_attributesetinstance_id");

            Dictionary<MAttribute, object> lst = new Dictionary<MAttribute, object>();
            for (int i = 0; i < attributes.Length; i++)
            {
                if (MAttribute.ATTRIBUTEVALUETYPE_List.Equals(attributes[i].GetAttributeValueType()))
                {
                    object editor = editors[i];
                    MAttributeValue value = null;
                    if (Convert.ToInt32(editors[i].Key) > 0)
                    {
                        value = new MAttributeValue(ctx, Convert.ToInt32(editors[i].Key), null);
                        value.SetName(editors[i].Name);
                    }
                    log.Fine(attributes[i].GetName() + "=" + value);
                    if (attributes[i].IsMandatory() && value == null)
                    {
                        mandatory += " - " + attributes[i].GetName();
                    }
                    lst[attributes[i]] = value;
                    //attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, value);
                }
                else if (MAttribute.ATTRIBUTEVALUETYPE_Number.Equals(attributes[i].GetAttributeValueType()))
                {
                    object editor = editors[i].Name;
                    decimal value = Convert.ToDecimal(editor);
                    log.Fine(attributes[i].GetName() + "=" + value);
                    if (attributes[i].IsMandatory() && value == null)
                    {
                        mandatory += " - " + attributes[i].GetName();
                    }
                    lst[attributes[i]] = value.ToString("0.##########");
                    //attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, value);
                }
                else
                {
                    object editor = editors[i].Name;
                    String value = Convert.ToString(editor);
                    log.Fine(attributes[i].GetName() + "=" + value);
                    if (attributes[i].IsMandatory() && (value == null || value.Length == 0))
                    {
                        mandatory += " - " + attributes[i].GetName();
                    }
                    lst[attributes[i]] = value;
                    //attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, value);
                }
                _changed = true;
            }
            if (attributes.Length > 0)
            {
                qry = @"SELECT M_AttributeSetInstance_ID FROM (SELECT ats.M_AttributeSetInstance_ID, av.M_AttributeValue_ID,ats.M_AttributeSet_ID,au.Value,att.AttributeValueType FROM M_AttributeSetInstance ats 
                        INNER JOIN M_AttributeInstance au ON ats.M_AttributeSetInstance_ID=au.M_AttributeSetInstance_ID LEFT JOIN M_Attribute att 
                        ON au.M_Attribute_ID=att.M_Attribute_ID LEFT JOIN M_AttributeValue av ON au.M_AttributeValue_ID=av.M_AttributeValue_ID WHERE ats.M_AttributeSet_ID = " + aset.GetM_AttributeSet_ID() + " AND (";
                bool hasAttr = false;
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (!String.IsNullOrEmpty(Util.GetValueOfString(lst[attributes[i]])))
                    {
                        if (hasAttr && i < attributes.Length)
                        {
                            qry += " OR";
                        }
                        attrCount++;
                        qry += " au.value = '" + Util.GetValueOfString(lst[attributes[i]]) + "'";
                        hasAttr = true;
                    }
                }
                qry += ")";
            }
            else
            {
                qry = @"SELECT ats.M_AttributeSetInstance_ID FROM M_AttributeSetInstance ats ";
            }

            if (sql.Length > 0)
            {
                qry += sql;
            }
            if (attributes.Length > 0)
            {
                qry += ",au.M_Attribute_ID) GROUP BY M_AttributeSetInstance_ID HAVING Count(M_AttributeSetInstance_ID) = " + attrCount;
            }

            ds = DB.ExecuteDataset(qry, null, null);

            if (_changed)
            {
                if (mandatory.Length > 0)
                {
                    obj.Error = Msg.GetMsg(ctx, "FillMandatory") + mandatory;
                    return obj;
                }
                if (attributes.Length > 0)
                {
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            int attCount = 0;
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                qry = "SELECT Count(*) FROM M_AttributeInstance WHERE M_AttributeSetInstance_ID = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                attCount = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                                if (attCount == attrCount)
                                {
                                    mAttributeSetInstanceId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                    break;
                                }

                                //    //int attSetID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSet_ID"]);
                                //    string valueType = Util.GetValueOfString(ds.Tables[0].Rows[i]["AttributeValueType"]);
                                //    int attributesetinstance_iD = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                //    if (i > 0 && attributesetinstance_iD != Util.GetValueOfInt(ds.Tables[0].Rows[i - 1]["M_AttributeSetInstance_ID"]))
                                //    {
                                //        attCount = 0;
                                //    }
                                //    for (int j = 0; j < attributes.Length; j++)
                                //    {
                                //        if (MAttribute.ATTRIBUTEVALUETYPE_List.Equals(attributes[j].GetAttributeValueType()) && MAttribute.ATTRIBUTEVALUETYPE_List.Equals(valueType))
                                //        {
                                //            int attID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeValue_ID"]);
                                //            MAttributeValue atr = new MAttributeValue(ctx, attID, null);

                                //            if (Util.GetValueOfString(atr.GetName()) == Util.GetValueOfString(lst[attributes[j]]) /*&& attSetID == aset.GetM_AttributeSet_ID()*/)
                                //            {
                                //                attCount += 1;
                                //            }
                                //            else
                                //            {
                                //                continue;
                                //            }
                                //        }
                                //        else if (MAttribute.ATTRIBUTEVALUETYPE_Number.Equals(attributes[j].GetAttributeValueType()) && MAttribute.ATTRIBUTEVALUETYPE_Number.Equals(valueType))
                                //        {
                                //            decimal? attVal = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Value"]);
                                //            if (attVal == Util.GetValueOfDecimal(lst[attributes[j]]) /*&& attSetID == aset.GetM_AttributeSet_ID()*/)
                                //            {
                                //                attCount += 1;
                                //            }
                                //            else
                                //            {
                                //                continue;
                                //            }
                                //        }
                                //        else if (MAttribute.ATTRIBUTEVALUETYPE_StringMax40.Equals(attributes[j].GetAttributeValueType()) && MAttribute.ATTRIBUTEVALUETYPE_StringMax40.Equals(valueType))
                                //        {
                                //            string attVal = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                                //            if (attVal == Util.GetValueOfString(lst[attributes[j]]) /*&& attSetID == aset.GetM_AttributeSet_ID()*/)
                                //            {
                                //                attCount += 1;
                                //            }
                                //            else
                                //            {
                                //                continue;
                                //            }
                                //        }
                                //    }

                                //    if (attCount == attributes.Length)
                                //    {
                                //        mAttributeSetInstanceId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                //        break;
                                //    }
                            }
                            //if (attCount != attributes.Length)
                            //{
                            //    mAttributeSetInstanceId = 0;
                            //}
                        }
                        else
                        {
                            mAttributeSetInstanceId = 0;
                        }
                    }
                    else
                    {
                        mAttributeSetInstanceId = 0;
                    }
                    ds.Dispose();
                }
                else
                {
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                mAttributeSetInstanceId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                break;
                            }
                        }
                        else
                        {
                            mAttributeSetInstanceId = 0;
                        }
                    }
                    else
                    {
                        mAttributeSetInstanceId = 0;
                    }
                    ds.Dispose();
                }

                if (mAttributeSetInstanceId == 0)
                {
                    if (hasValue && strAttrCode != "" && attributeID == 0)
                    {
                        _masi.Set_Value("Value", strAttrCode);
                    }
                    _masi.Save();
                    mAttributeSetInstanceId = _masi.GetM_AttributeSetInstance_ID();
                    obj.M_AttributeSetInstance_ID = _masi.GetM_AttributeSetInstance_ID();
                    obj.M_AttributeSetInstanceName = _masi.GetDescription();
                }

                else
                {
                    _masi = new MAttributeSetInstance(ctx, mAttributeSetInstanceId, null);
                }

                for (int i = 0; i < attributes.Length; i++)
                {
                    if (MAttribute.ATTRIBUTEVALUETYPE_List.Equals(attributes[i].GetAttributeValueType()))
                    {
                        MAttributeValue value = lst[attributes[i]] != null ? lst[attributes[i]] as MAttributeValue : null;

                        //Commented because that part is already handled in SetMAttributeInstance Function if Value is null then it'll set id to 0 and value to null
                        //if (value == null)
                        //{
                        //    continue;
                        //}
                        attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, value);
                    }
                    else if (MAttribute.ATTRIBUTEVALUETYPE_Number.Equals(attributes[i].GetAttributeValueType()))
                    {
                        if (Convert.ToDecimal(lst[attributes[i]]) == 0)
                        {
                            continue;
                        }
                        attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, Convert.ToDecimal(lst[attributes[i]]));
                    }
                    else
                    {
                        if ((String)lst[attributes[i]] == "")
                        {
                            continue;
                        }
                        attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, (String)lst[attributes[i]]);
                    }
                }
                if (hasValue)
                {
                    sql.Clear();
                    sql.Append("SELECT M_ProductAttributes_ID FROM M_ProductAttributes WHERE M_AttributeSetInstance_ID = " + mAttributeSetInstanceId + " AND M_Product_ID = " + mProductId + " AND UPC IS NULL");
                    pAttribute_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));

                    if (pAttribute_ID == 0)
                    {
                        MProductAttributes pAttr = new MProductAttributes(ctx, 0, null);
                        pAttr.SetAD_Org_ID(product.GetAD_Org_ID());
                        if (strAttrCodeC != "")
                        {
                            pAttr.SetUPC(strAttrCodeC);
                        }
                        else
                        {
                            pAttr.SetUPC("");
                        }
                        pAttr.SetM_Product_ID(mProductId);
                        pAttr.SetM_AttributeSetInstance_ID(mAttributeSetInstanceId);
                        pAttr.Save();
                    }

                    sql.Clear();
                    sql.Append("SELECT Value FROM M_AttributeSetInstance WHERE M_AttributeSetInstance_ID = " + mAttributeSetInstanceId);
                    attrValue = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString()));
                    //
                    if (attributeID != 0 && attributeID != mAttributeSetInstanceId)
                    {
                        obj.Error = Msg.GetMsg(ctx, "AttributeCodeExists");
                    }
                    else if (strAttrCode != "" && attrValue == "")
                    {
                        _masi.Set_Value("Value", strAttrCode);
                    }
                    _masi.SetDescription();
                    _masi.Save();

                    obj.M_AttributeSetInstance_ID = _masi.GetM_AttributeSetInstance_ID();
                    obj.M_AttributeSetInstanceName = _masi.GetDescription();
                    obj.attrCode = strAttrCode;
                }
                else
                {
                    if (attributeID == 0 && strAttrCode != "")
                    {
                        MProductAttributes pAttr = new MProductAttributes(ctx, 0, null);
                        pAttr.SetAD_Org_ID(product.GetAD_Org_ID());
                        pAttr.SetUPC(strAttrCode);
                        pAttr.SetM_Product_ID(mProductId);
                        pAttr.SetM_AttributeSetInstance_ID(mAttributeSetInstanceId);
                        pAttr.Save();
                    }
                    _masi.SetDescription();
                    _masi.Save();

                    mAttributeSetInstanceId = _masi.GetM_AttributeSetInstance_ID();
                    obj.M_AttributeSetInstance_ID = _masi.GetM_AttributeSetInstance_ID();
                    obj.M_AttributeSetInstanceName = _masi.GetDescription();
                    obj.attrCode = strAttrCode;
                    //
                    if (attributeID != 0 && (attributeID != mAttributeSetInstanceId || product_id != mProductId))
                    {
                        obj.Error = Msg.GetMsg(ctx, "AttributeCodeExists");
                    }
                }
            }
            else
            {
                obj.M_AttributeSetInstance_ID = _masi.GetM_AttributeSetInstance_ID();
                obj.M_AttributeSetInstanceName = _masi.GetDescription();
                obj.attrCode = strAttrCode;
            }
            return obj;
        }

        public string GetSerNo(Ctx ctx, int M_AttributeSetInstance_ID, int M_Product_ID)
        {
            var masi = MAttributeSetInstance.Get(ctx, M_AttributeSetInstance_ID, M_Product_ID);
            return masi.GetSerNo(true);
        }

        public KeyNamePair CreateLot(Ctx ctx, int M_AttributeSetInstance_ID, int M_Product_ID)
        {
            var masi = MAttributeSetInstance.Get(ctx, M_AttributeSetInstance_ID, M_Product_ID);
            return masi.CreateLot(M_Product_ID);
        }

        // Added by Bharat on 01 June 2017
        public Dictionary<string, object> GetBPData(int mProductID, int C_BPartner_ID, Ctx ctx)
        {
            Dictionary<string, object> retBP = null;
            string sql = "SELECT bp.ShelfLifeMinPct, bpp.ShelfLifeMinPct AS PCT, bpp.ShelfLifeMinDays "
                    + "FROM C_BPartner bp "
                    + " LEFT OUTER JOIN C_BPartner_Product bpp"
                    + " ON (bp.C_BPartner_ID=bpp.C_BPartner_ID AND bpp.M_Product_ID=" + mProductID + ") "
                    + "WHERE bp.C_BPartner_ID=" + C_BPartner_ID;

            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retBP = new Dictionary<string, object>();
                retBP["ShelfLifeMinPct"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["ShelfLifeMinPct"]);
                retBP["PCT"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["PCT"]);
                retBP["ShelfLifeMinDays"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["ShelfLifeMinDays"]);
            }
            return retBP;
        }

        // Added by Bharat on 01 June 2017
        public List<Dictionary<string, object>> GetAttributeData(string sql, int product_ID, Ctx ctx)
        {
            List<Dictionary<string, object>> retAttr = null;
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@M_Product_ID", product_ID);
            DataSet ds = DB.ExecuteDataset(sql, param, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retAttr = new List<Dictionary<string, object>>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, object> obj = new Dictionary<string, object>();
                    obj["M_AttributeSetInstance_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                    obj["Description"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Description"]);
                    obj["Lot"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Lot"]);
                    obj["SerNo"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["SerNo"]);
                    obj["GuaranteeDate"] = Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["GuaranteeDate"]);
                    obj["Value"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                    obj["QtyReserved"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["QtyReserved"]);
                    obj["QtyOrdered"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["QtyOrdered"]);
                    obj["QtyOnHand"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["QtyOnHand"]);
                    obj["GoodForDays"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["GoodForDays"]);
                    obj["ShelfLifeDays"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["ShelfLifeDays"]);
                    obj["ShelfLifeRemainingPct"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["ShelfLifeRemainingPct"]);
                    obj["M_Locator_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]);
                    obj["AttrCode"] = Util.GetValueOfString(ds.Tables[0].Rows[i]["AttrCode"]);
                    retAttr.Add(obj);
                }
            }
            return retAttr;
        }

        // Added by Bharat on 01 June 2017
        public int CheckUniqueLot(Ctx ctx)
        {
            string sql = "SELECT Count(AD_Column_ID) FROM AD_Column WHERE ColumnName = 'UniqueLot' AND AD_Table_ID = (SELECT AD_Table_ID FROM AD_Table WHERE TableName = 'M_AttributeSet')";
            int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            return count;
        }

        // Added by Bharat on 01 June 2017
        public int CheckAttribute(int windowNoParent, int mProductId, string lotString, Ctx ctx)
        {
            int Count = 0;
            string sql = "SELECT COUNT(M_Product_ID) FROM M_Storage s INNER JOIN M_Locator l ON (l.M_Locator_ID = s.M_Locator_ID) "
                       + " INNER JOIN M_warehouse w ON (w.M_warehouse_ID = l.M_Warehouse_ID) WHERE AD_Client_ID = " + ctx.GetAD_Client_ID();

            StringBuilder sqlWhere = new StringBuilder();
            var AD_Org_ID = Env.GetCtx().GetContextAsInt(windowNoParent, "AD_Org_ID");
            var sqlChk = "SELECT IsOrganization, IsProduct, IsWarehouse FROM M_AttributeSet aSet INNER JOIN M_Product mp on mp.M_AttributeSet_ID = aset.M_AttributeSet_ID"
                + " WHERE mp.M_Product_ID = " + mProductId;

            DataSet ds = DB.ExecuteDataset(sqlChk, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                if (Util.GetValueOfString(ds.Tables[0].Rows[0][0]) == "Y")
                {
                    sqlWhere.Append(" OR s.AD_Org_ID = " + AD_Org_ID);
                }
                if (Util.GetValueOfString(ds.Tables[0].Rows[0][1]) == "Y")
                {
                    sqlWhere = sqlWhere.Append(" OR s.M_Product_ID = " + mProductId);
                }
                if (Util.GetValueOfString(ds.Tables[0].Rows[0][2]) == "Y")
                {
                    int M_Warehouse_ID = 0;
                    string sqlMovement = "SELECT TableName FROM AD_Table WHERE AD_Table_ID = " + Env.GetCtx().GetContextAsInt(windowNoParent, "BaseTable_ID");
                    string tableName = Util.GetValueOfString(DB.ExecuteScalar(sqlMovement, null, null));
                    if (tableName != "")
                    {
                        if (tableName.ToUpper() == "M_MOVEMENT")
                        {
                            string sqlWarehouse = "SELECT wh.M_Warehouse_ID FROM M_Warehouse wh INNER JOIN M_Locator l ON (wh.M_Warehouse_ID = l.M_Warehouse_ID) "
                            + " WHERE l.M_Locator_ID = " + Env.GetCtx().GetContextAsInt(windowNoParent, "M_LocatorTo_ID");
                            M_Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(sqlWarehouse, null, null));
                        }
                    }
                    else
                    {
                        M_Warehouse_ID = Env.GetCtx().GetContextAsInt(windowNoParent, "M_Warehouse_ID"); ;
                    }

                    sqlWhere = sqlWhere.Append(" OR w.M_Warehouse_ID = " + M_Warehouse_ID);
                }
                if (sqlWhere.Length > 0)
                {
                    sqlWhere = sqlWhere.Remove(0, 3);
                    sql = sql + " AND (" + sqlWhere.ToString();
                    sql = sql + ") AND s.M_AttributeSetInstance_ID IN (SELECT M_AttributeSetInstance_ID FROM M_AttributeSetInstance WHERE Lot = '" + lotString + "')";
                }
                Count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            }
            return Count;
        }

        // Added by Bharat on 01 June 2017
        public string GetTitle(int Warehouse_ID, int Product_ID, Ctx ctx)
        {
            string sql = "SELECT p.Name || ' - ' || w.Name AS Name FROM M_Product p, M_Warehouse w "
                + "WHERE p.M_Product_ID=" + Product_ID + " AND w.M_Warehouse_ID=" + Warehouse_ID;
            string title = Util.GetValueOfString(DB.ExecuteScalar(sql, null, null));
            return title;
        }
    }
}
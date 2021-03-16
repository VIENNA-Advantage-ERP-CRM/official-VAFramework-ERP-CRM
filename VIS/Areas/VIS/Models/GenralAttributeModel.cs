using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{

    public class GenralAttributeModel
    {
        private VLogger log = VLogger.GetVLogger(typeof(GenralAttributeModel).FullName);

        Dictionary<MGenAttribute, KeyValuePair<MGenAttributeInstance, MGenAttributeValue[]>> attributesList = new Dictionary<MGenAttribute, KeyValuePair<MGenAttributeInstance, MGenAttributeValue[]>>(4);

        /// <summary>
        /// Load genral attribute
        /// </summary>
        /// <param name="mAttributeSetInstanceId"></param>
        /// <param name="vadms_AttributeSet_ID"></param>
        /// <param name="windowNo"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public AttributesObjects LoadInit(int mAttributeSetInstanceId, int vadms_AttributeSet_ID, int windowNo, Ctx ctx, HttpServerUtilityBase objBase)
        {
            AttributesObjects obj = new AttributesObjects();

            MGenAttributeSet aset = null;
            MGenAttribute[] attributes = null;

            //	Get Model
            var _masi = new MGenAttributeSetInstance(ctx, mAttributeSetInstanceId, null);
            _masi.SetC_GenAttributeSet_ID(vadms_AttributeSet_ID);
            if (_masi == null)
            {
                //  log.Severe("No Model for M_AttributeSetInstance_ID=" + _M_AttributeSetInstance_ID + ", M_Product_ID=" + _M_Product_ID);
                return null;
            }
            /* set context to client side */
            ctx.SetContext(windowNo, "C_GenAttributeSet_ID", _masi.GetC_GenAttributeSet_ID());
            //	Get Attribute Set
            aset = _masi.GetMGenAttributeSet();
            //	Product has no Attribute Set
            if (aset == null)
            {
                obj.IsReturnNull = true;
                obj.Error = "GenAttributeNoAttributeSet";
                return obj;
            }

            attributes = aset.GetCGenAttributes(false);

            for (int i = 0; i < attributes.Length; i++)
            {
                MGenAttribute a = attributes[i];
                MGenAttributeInstance ins = a.GetCGenAttributeInstance(mAttributeSetInstanceId);
                MGenAttributeValue[] v = null;

                if (MGenAttribute.ATTRIBUTEVALUETYPE_List.Equals(a.GetAttributeValueType()))
                {
                    v = a.GetMAttributeValues();
                }
                attributesList[a] = new KeyValuePair<MGenAttributeInstance, MGenAttributeValue[]>(ins, v);
            }


            //Row 0
            obj.tableStucture = "<table style='width: 100%;'><tr>";
            for (int i = 0; i < attributes.Length; i++)
            {
                obj.tableStucture = AddAttributeLine(attributes[i], false, windowNo, obj, i);
            }

            //	Attrribute Set Instance Description
            //Column 1
            var label1 = Msg.Translate(ctx, "Description");
            obj.tableStucture += "<td>";
            obj.tableStucture += "<div class='input-group vis-input-wrap'>";
            obj.tableStucture += "<div class='vis-control-wrap'>";
            obj.tableStucture += "<input readonly data-placeholder='' placeholder=' '  id='txtDescription_" + windowNo + "' value='" + (_masi.GetDescription()) + "' class='VIS_Pref_pass' type='text'>";
            obj.tableStucture += "<label id='description_" + windowNo + "' class='VIS_Pref_Label_Font'>" + label1 + "</label>";
            obj.tableStucture += "</div>";
            obj.tableStucture += "</div>";
            obj.tableStucture += "</td>";
            //Column 2
            //obj.tableStucture += "<td>";
            //obj.tableStucture += "</td>";

            obj.tableStucture += "</tr>";


            //Add Ok and Cancel button 
            //Last row
            obj.tableStucture += "<tr>";

            obj.tableStucture += "<td style='text-align:right'  colspan='2'>";
            obj.tableStucture += "<button style='margin-bottom:0px;margin-top:0px; float:right' type='button' class='VIS_Pref_btn-2' style='float: right;'  id='btnCancel_" + windowNo + "' role='button' aria-disabled='false'>" + Msg.GetMsg(ctx, "Cancel") + "</button>";
            obj.tableStucture += "<button style='margin-bottom:0px;margin-top:0px; float:right; margin-right: 10px;' type='button' class='VIS_Pref_btn-2' style='float: right; margin-right: 10px;'   id='btnOk_" + windowNo + "' role='button' aria-disabled='false'>" + Msg.GetMsg(ctx, "ok") + "</button>";
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
        /// Table line structure
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="product"></param>
        /// <param name="readOnly"></param>
        private string AddAttributeLine(MGenAttribute attribute, bool readOnly, int windowNo, AttributesObjects obj, int count)
        {
            //Column 1
            obj.tableStucture += "<td>";
            obj.tableStucture += "<label style='padding-bottom: 10px; padding-right: 5px;'  class='VIS_Pref_Label_Font' id=" + attribute.GetName().Replace(" ", "") + "_" + windowNo + "  >" + attribute.GetName() + "</label>";
            obj.tableStucture += "</td>";

            MGenAttributeInstance instance = attributesList[attribute].Key;

            if (MGenAttribute.ATTRIBUTEVALUETYPE_List.Equals(attribute.GetAttributeValueType()))
            {
                MGenAttributeValue[] values = attributesList[attribute].Value;

                //Column 2
                obj.tableStucture += "<td>";
                if (readOnly)
                {
                    obj.tableStucture += "<select style='width: 100%;'  class='VIS_Pref_pass'  readonly id='cmb_" + count + "_" + windowNo + "'>";
                }
                else
                {
                    obj.tableStucture += "<select style='width: 100%;'  class='VIS_Pref_pass' id='cmb_" + count + "_" + windowNo + "'>";
                }

                obj.ControlList += "cmb_" + count + "_" + windowNo + ",";
                bool found = false;
                if (instance != null)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i] == null && i == 0)
                        {
                            obj.tableStucture += " <option value='0' > </option>";
                        }
                        else if (values[i] != null)
                        {
                            if (values[i].GetC_GenAttributeValue_ID() == instance.GetC_GenAttributeValue_ID())
                            {
                                obj.tableStucture += " <option selected value='" + values[i].GetC_GenAttributeValue_ID() + "' >" + values[i].GetName() + "</option>";
                            }
                            else
                            {
                                obj.tableStucture += " <option value='" + values[i].GetC_GenAttributeValue_ID() + "' >" + values[i].GetName() + "</option>";
                            }
                        }
                    }


                    if (found)
                    {
                        log.Fine("Attribute=" + attribute.GetName() + " #" + values.Length + " - found: " + instance);
                    }
                    else
                    {
                        log.Warning("Attribute=" + attribute.GetName() + " #" + values.Length + " - NOT found: " + instance);
                    }
                }
                else
                {
                    //if instance value is null
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i] == null && i == 0)
                        {
                            obj.tableStucture += " <option value='0' > </option>";
                        }
                        else if (values[i] != null)
                        {
                            obj.tableStucture += " <option value='" + values[i].GetC_GenAttributeValue_ID() + "' >" + values[i].GetName() + "</option>";
                        }
                    }
                    log.Fine("Attribute=" + attribute.GetName() + " #" + values.Length + " no instance");
                }
                obj.tableStucture += "</select>";
                obj.tableStucture += "</td>";
            }
            else if (MGenAttribute.ATTRIBUTEVALUETYPE_Number.Equals(attribute.GetAttributeValueType()))
            {
                //Column 2
                obj.tableStucture += "<td>";
                var instanceValue = 0.0M;
                if (instance != null)
                {
                    instanceValue = instance.GetValueNumber();
                }

                if (readOnly)
                {
                    obj.tableStucture += "<input style='width: 100%;' class='VIS_Pref_pass' readonly id='txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo + "' value='" + instanceValue + "' class='' type='number'>";
                }
                else
                {
                    obj.tableStucture += "<input style='width: 100%;' class='VIS_Pref_pass'  id='txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo + "' value='" + instanceValue + "' class='' type='number'>";
                }
                obj.ControlList += "txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo + ",";
                obj.tableStucture += "</td>";
            }
            else	//	Text Field
            {
                //Column 2
                obj.tableStucture += "<td>";

                var val = "";

                if (instance != null)
                {
                    val = instance.GetValue();
                }

                if (readOnly)
                {
                    obj.tableStucture += "<input style='width: 100%;' class='VIS_Pref_pass' readonly id='txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo + "' value='" + val + "' class='' type='text'>";
                }
                else
                {
                    obj.tableStucture += "<input style='width: 100%;' class='VIS_Pref_pass'  id='txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo + "' value='" + val + "' class='' type='text'>";
                }

                obj.ControlList += "txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo + ",";
                obj.tableStucture += "</td>";
            }

            obj.tableStucture += "</tr>";
            //Row Add
            obj.tableStucture += "<tr>";
            return obj.tableStucture;
        }

        /// <summary>
        /// Save Genral attribute
        /// </summary>
        /// <param name="windowNoParent"></param>
        /// <param name="mAttributeSetInstanceId"></param>
        /// <param name="windowNo"></param>
        /// <param name="values"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public AttributeInstance SaveGenAttribute(int windowNoParent, int mAttributeSetInstanceId, int vadms_AttributeSet_ID, int windowNo, List<KeyNamePair> values, Ctx ctx)
        {
            var editors = values;
            AttributeInstance obj = new AttributeInstance();
            bool _changed = false;


            MGenAttributeSet aset = null;
            MGenAttribute[] attributes = null;
            String mandatory = "";
            var _masi = new MGenAttributeSetInstance(ctx, mAttributeSetInstanceId, null);

            //if there is different attribute set then delete old instance
            if (mAttributeSetInstanceId != 0 && (vadms_AttributeSet_ID != _masi.GetC_GenAttributeSet_ID()))
            {
                DB.ExecuteQuery("DELETE FROM C_GenAttributeInstance WHERE C_GenAttributeSetInstance_ID=" + mAttributeSetInstanceId);
            }

            _masi.SetC_GenAttributeSet_ID(vadms_AttributeSet_ID);
            aset = _masi.GetMGenAttributeSet();
            if (aset == null)
            {
                return null;
            }

            //	***	Save Attributes ***
            //	New Instance
            if (_changed || _masi.GetC_GenAttributeSetInstance_ID() == 0)
            {
                _masi.Save();
                obj.M_AttributeSetInstance_ID = _masi.GetC_GenAttributeSetInstance_ID();
                mAttributeSetInstanceId = _masi.GetC_GenAttributeSetInstance_ID();
                obj.M_AttributeSetInstanceName = _masi.GetDescription();
            }
            else
            {
                obj.M_AttributeSetInstance_ID = _masi.GetC_GenAttributeSetInstance_ID();
                mAttributeSetInstanceId = _masi.GetC_GenAttributeSetInstance_ID();
                obj.M_AttributeSetInstanceName = _masi.GetDescription();
            }
            //	Save Instance Attributes
            attributes = aset.GetCGenAttributes(false);

            Dictionary<MGenAttribute, object> lst = new Dictionary<MGenAttribute, object>();

            for (int i = 0; i < attributes.Length; i++)
            {
                if (MGenAttribute.ATTRIBUTEVALUETYPE_List.Equals(attributes[i].GetAttributeValueType()))
                {
                    object editor = editors[i];
                    //MGenAttributeValue value = (MGenAttributeValue)editor;
                    MGenAttributeValue value = null;
                    if (Convert.ToInt32(editors[i].Key) > 0)
                    {
                        value = new MGenAttributeValue(ctx, Convert.ToInt32(editors[i].Key), null);
                        value.SetName(editors[i].Name);
                    }
                    //log.fine(attributes[i].GetName() + "=" + value);
                    if (attributes[i].IsMandatory() && value == null)
                        mandatory += " - " + attributes[i].GetName();
                    lst[attributes[i]] = value;
                }
                else if (MGenAttribute.ATTRIBUTEVALUETYPE_Number.Equals(attributes[i].GetAttributeValueType()))
                {
                    object editor = editors[i].Name;
                    decimal value = Convert.ToDecimal(editor);
                    // log.fine(attributes[i].GetName() + "=" + value);
                    if (attributes[i].IsMandatory())
                        mandatory += " - " + attributes[i].GetName();
                    lst[attributes[i]] = value;
                }
                else
                {
                    object editor = editors[i].Name;
                    String value = Convert.ToString(editor);
                    //  log.Fine(attributes[i].GetName() + "=" + value);
                    if (attributes[i].IsMandatory() && (value == null || value.Length == 0))
                        mandatory += " - " + attributes[i].GetName();
                    lst[attributes[i]] = value;
                }
                _changed = true;
            }

            //	Save Model
            if (_changed)
            {

                for (int i = 0; i < attributes.Length; i++)
                {
                    if (MGenAttribute.ATTRIBUTEVALUETYPE_List.Equals(attributes[i].GetAttributeValueType()))
                    {
                        var editor = editors[i];

                        MGenAttributeValue value = lst[attributes[i]] != null ? lst[attributes[i]] as MGenAttributeValue : null;
                        // Done by Bharat on 14 Sep 2017 to handle the cases of null attributes
                        if (value == null)
                        {
                            continue;
                        }
                        attributes[i].SetCGenAttributeInstance(mAttributeSetInstanceId, value);
                    }
                    else if (MGenAttribute.ATTRIBUTEVALUETYPE_Number.Equals(attributes[i].GetAttributeValueType()))
                    {
                        if (Convert.ToDecimal(lst[attributes[i]]) == 0)
                        {
                            continue;
                        }
                        attributes[i].SetCGenAttributeInstance(mAttributeSetInstanceId, (decimal?)lst[attributes[i]]);
                    }
                    else
                    {
                        if ((String)lst[attributes[i]] == "")
                        {
                            continue;
                        }
                        attributes[i].SetCGenAttributeInstance(mAttributeSetInstanceId, (String)lst[attributes[i]]);
                    }
                }
                _masi.SetDescription();
                _masi.Save();

                obj.M_AttributeSetInstance_ID = _masi.GetC_GenAttributeSetInstance_ID();
                obj.M_AttributeSetInstanceName = _masi.GetDescription();
            }
            else
            {
                obj.M_AttributeSetInstance_ID = _masi.GetC_GenAttributeSetInstance_ID();
                obj.M_AttributeSetInstanceName = _masi.GetDescription();
            }
            return obj;
        }

        public AttributeInstance SearchValuesAttribute(int windowNoParent, int mAttributeSetInstanceId, int vadms_AttributeSet_ID, int windowNo, List<KeyNamePair> values, Ctx ctx)
        {
            AttributeInstance obj = new AttributeInstance();
            try
            {
                var editors = values;
                MGenAttributeSet aset = null;
                MGenAttribute[] attributes = null;
                String mandatory = "";

                var _masi = new MGenAttributeSetInstance(ctx, mAttributeSetInstanceId, null);
                _masi.SetC_GenAttributeSet_ID(vadms_AttributeSet_ID);
                aset = _masi.GetMGenAttributeSet();
                if (aset == null)
                {
                    return obj;
                }

                if (_masi.GetC_GenAttributeSetInstance_ID() == 0)
                {
                    // _masi.Save();
                    obj.M_AttributeSetInstance_ID = _masi.GetC_GenAttributeSetInstance_ID();
                    obj.M_AttributeSetInstanceName = _masi.GetDescription();
                }
                else
                {
                    var _masi1 = new MGenAttributeSetInstance(ctx, 0, null);
                    _masi.CopyTo(_masi1);
                    // _masi1.Save();

                    _masi = null;
                    _masi = _masi1;

                    obj.M_AttributeSetInstance_ID = _masi.GetC_GenAttributeSetInstance_ID();
                    obj.M_AttributeSetInstanceName = _masi.GetDescription();
                }

                //	Save Instance Attributes
                attributes = aset.GetCGenAttributes(false);

                Dictionary<MGenAttribute, object> lst = new Dictionary<MGenAttribute, object>();

                StringBuilder sql = new StringBuilder();
                StringBuilder where = new StringBuilder();
                StringBuilder description = new StringBuilder();

                for (int i = 0; i < attributes.Length; i++)
                {
                    if (MGenAttribute.ATTRIBUTEVALUETYPE_List.Equals(attributes[i].GetAttributeValueType()))
                    {
                        object editor = editors[i];
                        MGenAttributeValue value = null;
                        if (Convert.ToInt32(editors[i].Key) > 0)
                        {
                            value = new MGenAttributeValue(ctx, Convert.ToInt32(editors[i].Key), null);
                            value.SetName(editors[i].Name);
                        }

                        log.Fine(attributes[i].GetName() + "=" + value);
                        if (value != null)
                        {
                            if (attributes[i].IsMandatory() && value == null)
                                mandatory += " - " + attributes[i].GetName();
                            // attributes[i].SetMAttributeInstance(_M_AttributeSetInstance_ID, value);
                            lst[attributes[i]] = value;
                            description.Append(value + "_");

                            if (i == 0 || sql.Length == 0)
                            {
                                sql.Append("SELECT DISTINCT cg.C_GenAttributeSetInstance_ID FROM C_GenAttributeInstance cg");
                                where.Append(" WHERE cg.IsActive='Y'");
                                if (value != null)
                                {
                                    where.Append(" AND (cg.C_GenattributeValue_ID=").Append(value.GetC_GenAttributeValue_ID()).Append(" AND cg.C_GenAttribute_ID=").Append(attributes[i].GetC_GenAttribute_ID() + ")");
                                }
                                else
                                {
                                    where.Append(" AND (cg.C_GenattributeValue_ID IS NULL ").Append(" AND cg.C_GenAttribute_ID=").Append(attributes[i].GetC_GenAttribute_ID() + ")");
                                }
                            }
                            else if (i > 0 && sql.Length > 0)
                            {
                                sql.Append(" JOIN C_GenAttributeInstance cg" + i + " ON cg" + i + ".C_GenAttributeSetInstance_ID = cg.C_GenAttributeSetInstance_ID AND cg" + i + ".IsActive='Y'");
                                if (value != null)
                                {
                                    sql.Append(" AND (cg" + i + ".C_GenattributeValue_ID=").Append(value.GetC_GenAttributeValue_ID()).Append(" AND cg" + i + ".C_GenAttribute_ID=").Append(attributes[i].GetC_GenAttribute_ID() + ")");
                                }
                                else
                                {
                                    sql.Append(" AND (cg" + i + ".C_GenattributeValue_ID IS NULL ").Append(" AND cg" + i + ".C_GenAttribute_ID=").Append(attributes[i].GetC_GenAttribute_ID() + ")");
                                }
                            }
                        }
                    }
                    else if (MGenAttribute.ATTRIBUTEVALUETYPE_Number.Equals(attributes[i].GetAttributeValueType()))
                    {
                        var editor = editors[i];
                        var value = Convert.ToDecimal(editor.Name);

                        if (value > 0)
                        {
                            log.Fine(attributes[i].GetName() + "=" + value);

                            if (attributes[i].IsMandatory())
                                mandatory += " - " + attributes[i].GetName();

                            //attributes[i].SetMAttributeInstance(_M_AttributeSetInstance_ID, value);
                            lst[attributes[i]] = value;

                            if (i == 0 || sql.Length == 0)
                            {
                                sql.Append("SELECT DISTINCT cg.C_GenAttributeSetInstance_ID FROM C_GenAttributeInstance cg");
                                where.Append(" WHERE cg.IsActive='Y' AND (cg.VALUENUMBER=").Append(value).Append(" AND cg.C_GenAttribute_ID=").Append(attributes[i].GetC_GenAttribute_ID() + ")");
                            }
                            else if (i > 0 && sql.Length > 0)
                            {
                                sql.Append(" JOIN C_GenAttributeInstance cg" + i + " ON cg" + i + ".C_GenAttributeSetInstance_ID = cg.C_GenAttributeSetInstance_ID AND cg" + i + ".IsActive='Y'");
                                sql.Append(" AND (cg" + i + ".VALUENUMBER=").Append(value).Append(" AND cg" + i + ".C_GenAttribute_ID=").Append(attributes[i].GetC_GenAttribute_ID() + ")");                                
                            }

                            description.Append(value + "_");
                        }
                    }
                    else
                    {
                        var editor = editors[i];
                        var value = editor.Name;

                        if (value != null && value != "")
                        {
                            log.Fine(attributes[i].GetName() + "=" + value);
                            if (attributes[i].IsMandatory() && (value == null || value.ToString().Length == 0))
                                mandatory += " - " + attributes[i].GetName();

                            lst[attributes[i]] = value;

                            if (i == 0 || sql.Length == 0)
                            {
                                sql.Append("SELECT DISTINCT cg.C_GenAttributeSetInstance_ID FROM C_GenAttributeInstance cg");
                                where.Append(" WHERE cg.IsActive='Y' AND (upper(cg.VALUE)=upper('").Append(value).Append("') AND cg.C_GenAttribute_ID=").Append(attributes[i].GetC_GenAttribute_ID() + ")");
                            }
                            else if (i > 0 && sql.Length > 0)
                            {
                                sql.Append(" JOIN C_GenAttributeInstance cg" + i + " ON cg" + i + ".C_GenAttributeSetInstance_ID = cg.C_GenAttributeSetInstance_ID AND cg" + i + ".IsActive='Y'");
                                sql.Append(" AND (upper(cg" + i + ".VALUE)=upper('").Append(value).Append("') AND cg" + i + ".C_GenAttribute_ID=").Append(attributes[i].GetC_GenAttribute_ID() + ")");
                            }

                            description.Append(value + "_");
                        }
                    }
                }
                if (description.Length > 0)
                {
                    description.Remove(description.Length - 1, 1);
                    sql.Append(where.ToString());
                    DataSet ds = DB.ExecuteDataset(sql.ToString(), null);

                    StringBuilder ids = new StringBuilder();
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int c = 0; c < ds.Tables[0].Rows.Count; c++)
                        {
                            ids.Append(Util.GetValueOfString(ds.Tables[0].Rows[c][0])).Append(",");
                        }
                    }

                    if (ids.ToString().Contains(","))
                    {
                        ids.Remove(ids.Length - 1, 1);
                    }

                    obj.Description = description.ToString();

                    if (ids.Length > 0)
                    {
                        obj.GenSetInstance = ids.ToString();
                    }
                    else
                    {
                        obj.GenSetInstance = "0";
                    }
                }
                else
                {
                    obj.Description = description.ToString();
                }
            }
            catch (Exception ex)
            {
                log.Severe(ex.Message);
            }
            return obj;
        }
    }
}
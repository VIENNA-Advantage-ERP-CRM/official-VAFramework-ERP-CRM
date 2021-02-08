using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;
using ViennaAdvantage.Model;
namespace VIS.Models
{
    public class AmountDivisionModel
    {
        protected VLogger log = VLogger.GetVLogger(typeof(AmountDivisionModel).FullName);
        public int recid { get; set; }
        public string DimensionType { get; set; }
        public string DimensionName { get; set; }
        public string DimensionValueAmount { get; set; }
        public decimal CalculateDimValAmt { get; set; }
        public int AcctSchema { get; set; }
        public string DimensionTypeVal { get; set; }
        public int DimensionNameVal { get; set; }
        public int ElementID { get; set; }
        public int VAB_BusinessPartner_ID { get; set; }
        public String VAB_BusinessPartner { get; set; }
        public String lineAmountID { get; set; }
        public int TotalRecord { get; set; }
        public decimal TotalLineAmount { get; set; }

        /// <summary>
        /// Get Dimension Type
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <returns>List of Dimension Type</returns>
        public List<InfoRefList> GetDimensionType(Ctx ctx)
        {
            List<InfoRefList> list = new List<InfoRefList>();
            int Reference_ID = Convert.ToInt32(VAdvantage.DataBase.DB.ExecuteScalar("SELECT VAF_Control_Ref_Value_ID FROM VAF_Column WHERE Export_ID='VIS_2663'"));
            ValueNamePair[] refList = MVAFCtrlRefList.GetList(Reference_ID, true, ctx);
            InfoRefList itm = null;// new InfoRefList();
            // itm.Key = "";
            // itm.Value = "";
            // list.Add(itm);
            for (int i = 0; i < refList.Length; i++)
            {
                if (refList[i].GetKeyID().ToString() != "")
                {
                    itm = new InfoRefList();
                    itm.Key = refList[i].GetKeyID().ToString();//["Value"].ToString();
                    itm.Value = refList[i].GetName();// ds.Tables[0].Rows[i]["Name"].ToString();

                    list.Add(itm);
                }
            }
            refList = null;
            return list;
        }
        public class ListAccountingSchema
        {
            public int Key { get; set; }
            public string Value { get; set; }

            public int Precision { get; set; }
        }
        public List<ListAccountingSchema> GetAccountingSchema(Ctx ctx, int OrgID)
        {
            string Sql = "";
            List<ListAccountingSchema> listAcctSchema = new List<ListAccountingSchema>();
            //Sql = "SELECT object_name FROM all_objects WHERE object_type IN ('TABLE','VIEW') AND (object_name)  = UPPER('FRPT_ASSIGNEDORG') AND OWNER LIKE '" + DB.GetSchema() + "'";
            Sql = DBFunctionCollection.CheckTableExistence(DB.GetSchema(), "FRPT_ASSIGNEDORG");
            string ObjectName = Convert.ToString(DB.ExecuteScalar(Sql));
            if (ObjectName != "")
            {
                Sql = "SELECT DISTINCT VAB_AccountBook.Name,VAB_AccountBook.VAB_AccountBook_id, c.StdPrecision FROM VAB_AccountBook VAB_AccountBook INNER JOIN FRPT_AssignedOrg FRPT_AssignedOrg ON FRPT_AssignedOrg.vaf_client_ID=VAB_AccountBook.vaf_client_ID " +
                    " AND FRPT_AssignedOrg.VAB_AccountBook_ID=VAB_AccountBook.VAB_AccountBook_id INNER JOIN VAB_Currency c ON VAB_AccountBook.VAB_Currency_ID = c.VAB_Currency_ID " +
                    " WHERE  VAB_AccountBook.isactive='Y' AND VAB_AccountBook.costing='N' AND";
                if (OrgID != 0)
                {
                    Sql += "(FRPT_AssignedOrg.VAF_Org_ID=" + OrgID + " OR FRPT_AssignedOrg.VAF_Org_ID=0)";
                }
                else
                {
                    Sql += "1=1";
                }
            }
            else
            {
                Sql = "SELECT distinct VAB_AccountBook.Name, VAB_AccountBook.VAB_AccountBook_id, c.StdPrecision FROM VAB_AccountBook INNER JOIN VAB_Currency c ON VAB_AccountBook.VAB_Currency_ID = c.VAB_Currency_ID WHERE VAB_AccountBook.isactive='Y' AND VAB_AccountBook.Costing='N'";
            }

            Sql = MVAFRole.GetDefault(ctx).AddAccessSQL(Sql, "VAB_AccountBook", MVAFRole.SQL_FULLYQUALIFIED, MVAFRole.SQL_RW);

            DataSet dsAcctSchema = DB.ExecuteDataset(Sql);
            if (dsAcctSchema != null && dsAcctSchema.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsAcctSchema.Tables[0].Rows.Count; i++)
                {
                    ListAccountingSchema objAcctSchema = new ListAccountingSchema();
                    objAcctSchema.Key = Convert.ToInt32(dsAcctSchema.Tables[0].Rows[i][1]);
                    objAcctSchema.Value = Convert.ToString(dsAcctSchema.Tables[0].Rows[i][0]);
                    objAcctSchema.Precision = Util.GetValueOfInt(dsAcctSchema.Tables[0].Rows[i][2]);
                    listAcctSchema.Add(objAcctSchema);
                }
            }
            return listAcctSchema;
        }
        private List<AmountDivisionModel> SplitAllAccountSchema(int[] acctSchema, List<AmountDivisionModel> dimensionLine)
        {
            List<AmountDivisionModel> splitDimensionLine = new List<AmountDivisionModel>();
            if (dimensionLine.Exists(x => x.AcctSchema == 0))
            {
                for (int i = 0; i < acctSchema.Length; i++)
                {
                    foreach (AmountDivisionModel amt in dimensionLine)
                    {
                        AmountDivisionModel obj = new AmountDivisionModel();
                        obj.AcctSchema = acctSchema[i];
                        obj.DimensionType = amt.DimensionType;
                        obj.DimensionName = amt.DimensionName;
                        obj.DimensionValueAmount = amt.DimensionValueAmount;
                        obj.DimensionTypeVal = amt.DimensionTypeVal;
                        obj.DimensionNameVal = amt.DimensionNameVal;
                        obj.ElementID = amt.ElementID;
                        obj.lineAmountID = amt.lineAmountID;
                        splitDimensionLine.Add(obj);
                    }

                }
                return splitDimensionLine;
            }
            else
            {
                return dimensionLine;
            }
        }
        private bool CheckUpdateMaxAmount(int RecordID, decimal TotalAmount, int[] acctSchema, decimal lineAmount)
        {
            bool temChkAmount = true;
            if (RecordID == 0)
            {
                return true;
            }
            else
            {
                decimal chkAmount = Convert.ToDecimal(DB.ExecuteScalar("select Amount from VAB_DimAmtline where VAB_DimAmt_ID=" + RecordID + " AND ROWNUM=1"));
                if (chkAmount == 0)
                {
                    return true;
                }
                else
                {

                    DataSet dsAmount = DB.ExecuteDataset("select totaldimlineamout,VAB_AccountBook_ID from VAB_DimAmtaccttype where VAB_DimAmt_id=" + RecordID + "");
                    if (dsAmount != null && dsAmount.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < dsAmount.Tables[0].Rows.Count; j++)
                        {
                            if (Convert.ToInt32(dsAmount.Tables[0].Rows[j]["VAB_AccountBook_ID"]) == acctSchema[0])
                            {
                                if (TotalAmount < (Convert.ToDecimal(dsAmount.Tables[0].Rows[j]["totaldimlineamout"]) + lineAmount))
                                {
                                    temChkAmount = false;
                                    break;
                                }
                            }
                            else
                            {
                                if (TotalAmount < (Convert.ToDecimal(dsAmount.Tables[0].Rows[j]["totaldimlineamout"])))
                                {
                                    temChkAmount = false;
                                    break;
                                }
                            }

                        }
                    }

                    if (temChkAmount)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        /// <summary>
        /// Insert Line on Amount Dimension
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="recordId">Amount Dimension ID</param>
        /// <param name="totalAmount">Total Amount</param>
        /// <param name="lineAmount">Line Amount</param>
        /// <param name="acctSchemaID">selected Accounting Schema</param>
        /// <param name="elementTypeID">Element Type</param>
        /// <param name="dimensionValue">Dimension Value</param>
        /// <param name="elementID">Element ID</param>
        /// <param name="oldDimensionName">Old Dimension Value</param>
        /// <param name="bpartner_ID">Business Partner</param>
        /// <param name="oldBPartner_ID"Old Business Partner></param>
        /// <returns>Amount Dimension Line</returns>
        public string[] InsertDimensionLine(Ctx ctx, int RecordId, decimal TotalAmount, decimal LineAmount, int[] acctSchemaID, string elementTypeID, int dimensionValue, int elementID, int oldDimensionName, int bpartner_ID, int oldBPartner_ID)
        {
            string Sql = "";
            int DimAcctTypeId;
            int dimAmtLineId;
            string[] LineAmountID = new string[2];
            string lineID = "";
            bool error = false;
            Trx trx = Trx.Get("trxDim" + DateTime.Now.Millisecond);
            try
            {
                foreach (int acct in acctSchemaID)
                {
                    X_VAB_DimAmt objDimAmt = new X_VAB_DimAmt(ctx, RecordId, trx);
                    //  if (objDimAmt.GetAmount() <= TotalAmount)
                    // {
                    if (CheckUpdateMaxAmount(RecordId, TotalAmount, acctSchemaID, LineAmount))
                    {
                        objDimAmt.SetAmount(TotalAmount);
                        //objDimAmt.SetVAF_TableView_ID(VAF_TableViewId);
                        // objDimAmt.SetRecord_ID(AD_RecordID);
                        if (!objDimAmt.Save(trx))
                        {
                            error = true;
                            goto ErrorCheck;
                        }
                    }
                    // }
                    LineAmountID[0] = Convert.ToString(objDimAmt.GetVAB_DimAmt_ID());
                    RecordId = objDimAmt.GetVAB_DimAmt_ID();
                    if (acct != -1)
                    {

                        // RecordId = objDimAmt.GetVAB_DimAmt_ID();

                        Sql = "select nvl(VAB_DimAmtaccttype_ID,0) from VAB_DimAmtaccttype where VAB_DimAmt_id=" + RecordId + " and VAB_AccountBook_ID=" + acct + "";
                        DimAcctTypeId = Convert.ToInt32(DB.ExecuteScalar(Sql));
                        Sql = "select nvl((sum(cd.amount)),0) as Amount from VAB_DimAmtline cd inner join VAB_DimAmtaccttype ct on cd.VAB_DimAmt_id=ct.VAB_DimAmt_id " +
                            " and cd.VAB_DimAmtaccttype_id=ct.VAB_DimAmtaccttype_id " +
                            " where cd.VAB_DimAmt_id=" + RecordId + " and ct.VAB_DimAmtaccttype_id=" + DimAcctTypeId + "";
                        decimal TotoalDimAmount = Convert.ToDecimal(DB.ExecuteScalar(Sql));
                        if (LineAmount != -1)
                        {
                            TotoalDimAmount += LineAmount;
                        }
                        X_VAB_DimAmtAcctType objDimAcctType = new X_VAB_DimAmtAcctType(ctx, DimAcctTypeId, trx);
                        objDimAcctType.SetVAB_DimAmt_ID(objDimAmt.GetVAB_DimAmt_ID());
                        if (LineAmount != -1)
                        {
                            objDimAcctType.SetVAB_AccountBook_ID(acct);
                            objDimAcctType.SetElementType(elementTypeID);
                        }
                        if (LineAmount != -1 || TotoalDimAmount != 0)
                        {
                            objDimAcctType.SetTotalDimLineAmout(TotoalDimAmount);
                            if (!objDimAcctType.Save(trx))
                            {
                                error = true;
                                goto ErrorCheck;
                            }
                        }
                        if (LineAmount != -1)
                        {
                            Sql = "select nvl(VAB_DimAmtline_id,0) from VAB_DimAmtline where VAB_DimAmt_ID=" + RecordId + " and VAB_DimAmtaccttype_id=" + objDimAcctType.GetVAB_DimAmtAcctType_ID() + "";
                            if (elementTypeID == "AC")
                            {
                                Sql += " and VAB_Acct_Element_id=" + oldDimensionName + " AND NVL(VAB_BusinessPartner_ID, 0)=" + oldBPartner_ID;
                            }//Account
                            else if (elementTypeID == "AY") { Sql += " and VAB_BillingCode_id=" + oldDimensionName; }//Activity
                            else if (elementTypeID == "BP") { Sql += " and VAB_BusinessPartner_ID=" + oldDimensionName; }//BPartner
                            else if (elementTypeID == "LF" || elementTypeID == "LT") { Sql += " and VAB_Address_ID=" + oldDimensionName; }//Location From//Location To
                            else if (elementTypeID == "MC") { Sql += " and VAB_Promotion_ID=" + oldDimensionName; }//Campaign
                            else if (elementTypeID == "OO" || elementTypeID == "OT") { Sql += " and Org_ID=" + oldDimensionName; }//Organization//Org Trx
                            else if (elementTypeID == "PJ") { Sql += " and VAB_Project_ID=" + oldDimensionName; }//Project
                            else if (elementTypeID == "PR") { Sql += " and VAM_Product_Id=" + oldDimensionName; }//Product
                            else if (elementTypeID == "SR") { Sql += " and VAB_SalesRegionState_Id=" + oldDimensionName; }//Sales Region
                            else if (elementTypeID == "U1" || elementTypeID == "U2")
                            {
                                Sql += " and VAB_Acct_Element_id=" + oldDimensionName;
                                if (oldBPartner_ID > 0)
                                {
                                    Sql += " AND NVL(VAB_BusinessPartner_ID, 0)= " + oldBPartner_ID;
                                }
                            }//User List 1//User List 2
                            else if (elementTypeID == "X1" || elementTypeID == "X2" || elementTypeID == "X3" || elementTypeID == "X4" || elementTypeID == "X5" || elementTypeID == "X6" ||
                                     elementTypeID == "X7" || elementTypeID == "X8" || elementTypeID == "X9") { Sql += " and VAF_Column_ID=" + oldDimensionName; }//User Element 1 to User Element 9

                            dimAmtLineId = Convert.ToInt32(DB.ExecuteScalar(Sql));

                            X_VAB_DimAmtLine objDimAmtLine = new X_VAB_DimAmtLine(ctx, dimAmtLineId, trx);
                            if (dimAmtLineId != 0)
                            {
                                objDimAcctType.SetTotalDimLineAmout(objDimAcctType.GetTotalDimLineAmout() - objDimAmtLine.GetAmount());
                                if (!objDimAcctType.Save(trx))
                                {
                                    error = true;
                                    goto ErrorCheck;
                                }
                            }
                            objDimAmtLine.SetVAB_DimAmt_ID(objDimAmt.GetVAB_DimAmt_ID());
                            objDimAmtLine.SetVAB_DimAmtAcctType_ID(objDimAcctType.GetVAB_DimAmtAcctType_ID());
                            objDimAmtLine.SetAmount(LineAmount);

                            if (elementTypeID == "AC")
                            {
                                objDimAmtLine.SetVAB_Element_ID(elementID);
                                objDimAmtLine.SetVAB_Acct_Element_ID(dimensionValue);
                                objDimAmtLine.SetVAB_BusinessPartner_ID(bpartner_ID);
                            }//Account
                            else if (elementTypeID == "AY") { objDimAmtLine.SetVAB_BillingCode_ID(dimensionValue); }//Activity
                            else if (elementTypeID == "BP") { objDimAmtLine.SetVAB_BusinessPartner_ID(dimensionValue); }//BPartner
                            else if (elementTypeID == "LF" || elementTypeID == "LT") { objDimAmtLine.SetVAB_Address_ID(dimensionValue); }//Location From//Location To
                            else if (elementTypeID == "MC") { objDimAmtLine.SetVAB_Promotion_ID(dimensionValue); }//Campaign
                            else if (elementTypeID == "OO" || elementTypeID == "OT") { objDimAmtLine.SetOrg_ID(dimensionValue); }//Organization//Org Trx
                            else if (elementTypeID == "PJ") { objDimAmtLine.SetVAB_Project_ID(dimensionValue); }//Project
                            else if (elementTypeID == "PR") { objDimAmtLine.SetVAM_Product_ID(dimensionValue); }//Product
                            else if (elementTypeID == "SA") { }//Sub Account
                            else if (elementTypeID == "SR") { objDimAmtLine.SetVAB_SalesRegionState_ID(dimensionValue); }//Sales Region
                            else if (elementTypeID == "U1" || elementTypeID == "U2")
                            {
                                objDimAmtLine.SetVAB_Element_ID(elementID);
                                objDimAmtLine.SetVAB_Acct_Element_ID(dimensionValue);
                                objDimAmtLine.SetVAB_BusinessPartner_ID(bpartner_ID);
                            }//User List 1//User List 2
                            else if (elementTypeID == "X1" || elementTypeID == "X2" || elementTypeID == "X3" || elementTypeID == "X4" || elementTypeID == "X5" || elementTypeID == "X6" ||
                                     elementTypeID == "X7" || elementTypeID == "X8" || elementTypeID == "X9") { objDimAmtLine.SetVAF_Column_ID(dimensionValue); }//User Element 1 to User Element 9
                            if (!objDimAmtLine.Save(trx))
                            {
                                error = true;
                                goto ErrorCheck;
                            }
                            if (lineID == "")
                            {
                                lineID += Convert.ToString(objDimAmtLine.GetVAB_DimAmtLine_ID());
                            }
                            else
                            {
                                lineID += "," + Convert.ToString(objDimAmtLine.GetVAB_DimAmtLine_ID());
                            }
                        }


                    }
                    else { break; }
                ErrorCheck:
                    if (error) { break; }
                }
            }
            catch (Exception e)
            {
                error = true;
            }
            finally
            {

                if (error)
                {
                    LineAmountID[0] = ""; LineAmountID[1] = "";
                    trx.Rollback();
                    log.Warning("Some error occured while saving Dimension");
                }
                else
                {
                    LineAmountID[1] = lineID;
                    trx.Commit();
                }
            }
            return LineAmountID;
        }
        //public int InsertDimensionAmount(int[] acctSchema, string[] elementType, decimal amount, List<AmountDivisionModel> dimensionLine, Ctx ctx, int DimAmtId)
        //{
        //    bool checkTran = true;
        //    int DimAcctTypeId = 0;
        //    int RecordID = -1;
        //    string Sql = "";
        //    Trx trx = Trx.Get("trxDim" + DateTime.Now.Millisecond);
        //    try
        //    {
        //        dimensionLine = splitAllAccountSchema(acctSchema, dimensionLine);
        //        X_VAB_DimAmt objDimAmt = new X_VAB_DimAmt(ctx, DimAmtId, trx);
        //        objDimAmt.SetAmount(amount);
        //        if (!objDimAmt.Save(trx))
        //        {
        //            checkTran = false;
        //            RecordID = -1;
        //            return RecordID;
        //        }
        //        RecordID = objDimAmt.GetVAB_DimAmt_ID();
        //        List<AmountDivisionModel> oldDimensionLine = GetDimensionLine(DimAmtId);

        //        //Check For Value in oldDimensionLine List against DimensionType and Accounting Schema...............
        //        foreach (AmountDivisionModel obj in oldDimensionLine)
        //        {
        //            var abc = dimensionLine.Where(x => x.DimensionTypeVal == obj.DimensionTypeVal && x.AcctSchema == obj.AcctSchema);
        //            if (abc == null)
        //            {
        //                //if Value does not exist in dimension Line than delete from VAB_DimAmtAcctType Table against DimensionType and AccountSchema
        //                //and corresponding Dimension Line Will be deleted.................(cascade).
        //                Sql = "delete from VAB_DimAmtaccttype where VAB_DimAmt_id=" + DimAmtId + " and elementtype='" + obj.DimensionTypeVal + "' and VAB_AccountBook_id=" + obj.AcctSchema;
        //                DB.ExecuteQuery(Sql, null, trx);
        //                oldDimensionLine.RemoveAll(x => x.DimensionTypeVal == obj.DimensionTypeVal && x.AcctSchema == obj.AcctSchema);
        //            }
        //        }
        //        //string sql1 = "delete from VAB_DimAmtline where VAB_DimAmt_id=" + objDimAmt.GetVAB_DimAmt_ID();
        //        //DB.ExecuteQuery(sql1, null, trx);
        //        for (int i = 0; i < acctSchema.Length; i++)
        //        {
        //            if (DimAmtId != 0)
        //            {
        //                //if User Change Dimension Type Value from Accounting Schema Element against Accounting Schema............
        //                //When user update Record against that Value than delete old Value From VAB_DimAmtAcctType............
        //                Sql = "select nvl(VAB_DimAmtaccttype_ID,0) from VAB_DimAmtaccttype where VAB_DimAmt_id=" + objDimAmt.GetVAB_DimAmt_ID() + " and VAB_AccountBook_ID=" + acctSchema[i] + "";
        //                DimAcctTypeId = Convert.ToInt32(DB.ExecuteScalar(Sql));
        //                Sql = "select elementType from VAB_DimAmtaccttype where VAB_DimAmtAcctType_id=" + DimAcctTypeId;
        //                string oldElement = Convert.ToString(DB.ExecuteScalar(Sql));
        //                if (oldElement != elementType[i])
        //                {
        //                    Sql = "delete from VAB_DimAmtaccttype where VAB_DimAmtAcctType_id=" + DimAcctTypeId;
        //                    DB.ExecuteQuery(Sql, null, trx);
        //                    DimAcctTypeId = 0;
        //                }
        //            }

        //            X_VAB_DimAmtAcctType objDimAcctType = new X_VAB_DimAmtAcctType(ctx, DimAcctTypeId, trx);
        //            objDimAcctType.SetVAB_DimAmt_ID(objDimAmt.GetVAB_DimAmt_ID());
        //            objDimAcctType.SetVAB_AccountBook_ID(acctSchema[i]);
        //            objDimAcctType.SetElementType(elementType[i]);
        //            if (DimAcctTypeId == 0)
        //            {
        //                if (!objDimAcctType.Save(trx))
        //                {
        //                    checkTran = false;
        //                    RecordID = -1;
        //                    return RecordID;
        //                }
        //            }
        //            List<AmountDivisionModel> accountDimensionLine = dimensionLine.FindAll(x => x.AcctSchema == acctSchema[i]);//Find Value against Accounting Schema in New List..........
        //            List<AmountDivisionModel> oldAccountDimensionLine = oldDimensionLine.FindAll(x => x.AcctSchema == acctSchema[i]);//Find Value against Accounting Schema in Old List...............

        //            foreach (AmountDivisionModel mod in accountDimensionLine)
        //            {
        //                List<AmountDivisionModel> oldTempDim = new List<AmountDivisionModel>();
        //                //Find New List Value in old List............
        //                //if All Value Matches than Do Nothing..........
        //                //if Amount Changes in New List than update against that Line ID.....
        //                if (oldAccountDimensionLine.Count > 0)
        //                {
        //                    oldTempDim = oldAccountDimensionLine.FindAll(x => x.DimensionNameVal == mod.DimensionNameVal && x.DimensionTypeVal == mod.DimensionTypeVal && x.DiemensionValueAmount == mod.DiemensionValueAmount);
        //                }
        //                if (oldTempDim.Count == 0)
        //                {
        //                    int dimAmtLineId = 0;
        //                    oldTempDim = oldAccountDimensionLine.FindAll(x => x.DimensionNameVal == mod.DimensionNameVal && x.DimensionTypeVal == mod.DimensionTypeVal);
        //                    if (oldTempDim != null)
        //                    {
        //                        Sql = "select VAB_DimAmtline_id from VAB_DimAmtline cl inner join VAB_DimAmtacctType ct on cl.VAB_DimAmt_id=ct.VAB_DimAmt_id and ct.VAB_DimAmtaccttype_id = cl.VAB_DimAmtaccttype_id " +
        //                               " where cl.VAB_DimAmt_id=" + objDimAmt.GetVAB_DimAmt_ID() + " and cl.VAB_DimAmtacctType_Id=" + DimAcctTypeId;
        //                        if (mod.DimensionTypeVal == "AC")
        //                        {
        //                            Sql += " and cl.VAB_Acct_Element_id=" + mod.DimensionNameVal;
        //                        }//Account
        //                        else if (mod.DimensionTypeVal == "AY") { Sql += " and cl.VAB_BillingCode_id=" + mod.DimensionNameVal; }//Activity
        //                        else if (mod.DimensionTypeVal == "BP") { Sql += " and cl.VAB_BusinessPartner_ID=" + mod.DimensionNameVal; }//BPartner
        //                        else if (mod.DimensionTypeVal == "LF" || mod.DimensionTypeVal == "LT") { Sql += " and cl.VAB_Address_ID=" + mod.DimensionNameVal; }//Location From//Location To
        //                        else if (mod.DimensionTypeVal == "MC") { Sql += " and cl.VAB_Promotion_ID=" + mod.DimensionNameVal; }//Campaign
        //                        else if (mod.DimensionTypeVal == "OO" || mod.DimensionTypeVal == "OT") { Sql += " and cl.Org_ID=" + mod.DimensionNameVal; }//Organization//Org Trx
        //                        else if (mod.DimensionTypeVal == "PJ") { Sql += " and cl.VAB_Project_ID=" + mod.DimensionNameVal; }//Project
        //                        else if (mod.DimensionTypeVal == "PR") { Sql += " and cl.VAM_Product_Id=" + mod.DimensionNameVal; }//Product
        //                        else if (mod.DimensionTypeVal == "SR") { Sql += " and cl.VAB_SalesRegionState_Id=" + mod.DimensionNameVal; }//Sales Region
        //                        else if (mod.DimensionTypeVal == "U1" || mod.DimensionTypeVal == "U2")
        //                        {
        //                            Sql += " and cl.VAB_Acct_Element_id=" + mod.DimensionNameVal;
        //                        }//User List 1//User List 2
        //                        else if (mod.DimensionTypeVal == "X1" || mod.DimensionTypeVal == "X2" || mod.DimensionTypeVal == "X3" || mod.DimensionTypeVal == "X4" || mod.DimensionTypeVal == "X5" || mod.DimensionTypeVal == "X6" ||
        //                                 mod.DimensionTypeVal == "X7" || mod.DimensionTypeVal == "X8" || mod.DimensionTypeVal == "X9") { Sql += " and cl.VAF_Column_ID=" + mod.DimensionNameVal; }//User Element 1 to User Element 9

        //                        dimAmtLineId = Convert.ToInt32(DB.ExecuteScalar(Sql));
        //                    }
        //                    else
        //                    {
        //                        dimAmtLineId = 0;
        //                    }
        //                    X_VAB_DimAmtLine objDimAmtLine = new X_VAB_DimAmtLine(ctx, dimAmtLineId, trx);
        //                    objDimAmtLine.SetVAB_DimAmt_ID(objDimAmt.GetVAB_DimAmt_ID());
        //                    objDimAmtLine.SetVAB_DimAmtAcctType_ID(objDimAcctType.GetVAB_DimAmtAcctType_ID());
        //                    objDimAmtLine.SetAmount(mod.DiemensionValueAmount);
        //                    if (elementType[i] == "AC")
        //                    {
        //                        objDimAmtLine.SetVAB_Element_ID(mod.ElementID);
        //                        objDimAmtLine.SetVAB_Acct_Element_ID(mod.DimensionNameVal);
        //                    }//Account
        //                    else if (elementType[i] == "AY") { objDimAmtLine.SetVAB_BillingCode_ID(mod.DimensionNameVal); }//Activity
        //                    else if (elementType[i] == "BP") { objDimAmtLine.SetVAB_BusinessPartner_ID(mod.DimensionNameVal); }//BPartner
        //                    else if (elementType[i] == "LF" || elementType[i] == "LT") { objDimAmtLine.SetVAB_Address_ID(mod.DimensionNameVal); }//Location From//Location To
        //                    else if (elementType[i] == "MC") { objDimAmtLine.SetVAB_Promotion_ID(mod.DimensionNameVal); }//Campaign
        //                    else if (elementType[i] == "OO" || elementType[i] == "OT") { objDimAmtLine.SetOrg_ID(mod.DimensionNameVal); }//Organization//Org Trx
        //                    else if (elementType[i] == "PJ") { objDimAmtLine.SetVAB_Project_ID(mod.DimensionNameVal); }//Project
        //                    else if (elementType[i] == "PR") { objDimAmtLine.SetVAM_Product_ID(mod.DimensionNameVal); }//Product
        //                    else if (elementType[i] == "SA") { }//Sub Account
        //                    else if (elementType[i] == "SR") { objDimAmtLine.SetVAB_SalesRegionState_ID(mod.DimensionNameVal); }//Sales Region
        //                    else if (elementType[i] == "U1" || elementType[i] == "U2")
        //                    {
        //                        objDimAmtLine.SetVAB_Element_ID(mod.ElementID);
        //                        objDimAmtLine.SetVAB_Acct_Element_ID(mod.DimensionNameVal);
        //                    }//User List 1//User List 2
        //                    else if (elementType[i] == "X1" || elementType[i] == "X2" || elementType[i] == "X3" || elementType[i] == "X4" || elementType[i] == "X5" || elementType[i] == "X6" ||
        //                             elementType[i] == "X7" || elementType[i] == "X8" || elementType[i] == "X9") { objDimAmtLine.SetVAF_Column_ID(mod.DimensionNameVal); }//User Element 1 to User Element 9
        //                    if (!objDimAmtLine.Save(trx))
        //                    {
        //                        checkTran = false;
        //                        RecordID = -1;
        //                        //return RecordID;
        //                        break;
        //                    }
        //                    //Remove Value From old List After that value is passed or Updated............
        //                    oldAccountDimensionLine.Remove(oldAccountDimensionLine.Find(a => a.AcctSchema == mod.AcctSchema && a.DimensionTypeVal == mod.DimensionTypeVal && a.DimensionNameVal == mod.DimensionNameVal));
        //                }
        //                else
        //                {
        //                    oldAccountDimensionLine.Remove(oldAccountDimensionLine.Find(a => a.AcctSchema == mod.AcctSchema && a.DimensionTypeVal == mod.DimensionTypeVal && a.DimensionNameVal == mod.DimensionNameVal));
        //                }
        //            }
        //            foreach (AmountDivisionModel oldMod in oldAccountDimensionLine)
        //            {
        //                //Delete Remaing Value Which are in old List and from VAB_DimAmtLine Table....................
        //                Sql = "delete from VAB_DimAmtline where VAB_DimAmtline_id=" + oldMod.AmtLineID;
        //                DB.ExecuteQuery(Sql, null, trx);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        checkTran = false;
        //        RecordID = -1;
        //    }
        //    finally
        //    {
        //        if (!checkTran)
        //        {
        //            trx.Rollback();
        //            log.Warning("Some error occured while saving Dimension");
        //        }
        //        else
        //        {
        //            trx.Commit();
        //        }
        //    }
        //    return RecordID;
        //}
        public List<AmountDivisionModel> GetDimensionLine(Ctx ctx, int[] accountingSchema, int dimensionID, int DimensionLineID = 0, int pageNo = 0, int pSize = 0)
        {
            int tempRecid = pSize * (pageNo - 1);
            List<AmountDivisionModel> objAmtdimModel = new List<AmountDivisionModel>();
            foreach (int acctId in accountingSchema)
            {
                string uElementTable = "";
                string uElementColumn = "";
                if (objAmtdimModel.Count != 0)
                {
                    break;
                }

                int tempDimensionID = Convert.ToInt32(DB.ExecuteScalar("select nvl(VAB_DimAmt_id,0) as DimID from VAB_DimAmtline where VAB_DimAmt_id=" + dimensionID + ""));
                if (tempDimensionID != 0)
                {
                    string uQuery = "select main.TableName," + DBFunctionCollection.ListAggregationAmountDimesnionLine("listagg(TO_CHAR(('ac.'||main.Colname)),'||''_''||') WITHIN GROUP(order by main.ColId)") + " AS ColName from " +
                         "(select distinct tab.tableName as TableName,col2.columnName as Colname,col2.vaf_column_id as ColId from VAB_DimAmtaccttype ct " +
                         "inner join VAB_DimAmtline cl on cl.VAB_DimAmt_id=ct.VAB_DimAmt_id and cl.VAB_DimAmtaccttype_id=ct.VAB_DimAmtaccttype_id " +
                         "inner join VAB_AccountBook_element se on se.VAB_AccountBook_id=ct.VAB_AccountBook_id and se.elementtype=ct.elementtype " +
                         "inner join vaf_column col1 on col1.vaf_column_id=se.vaf_column_id " +
                         "inner join vaf_tableview tab on tab.vaf_tableview_id=col1.vaf_tableview_id " +
                         "inner join vaf_column col2 on col2.vaf_tableview_id=tab.vaf_tableview_id and col2.isidentifier='Y' " +
                         "where ct.VAB_DimAmt_id=" + dimensionID + " and ct.VAB_AccountBook_id=" + acctId + " order by col2.vaf_column_id)  main " +
                         "group by main.TableName";
                    DataSet dsUelement = DB.ExecuteDataset(uQuery);
                    if (dsUelement != null && dsUelement.Tables[0].Rows.Count > 0)
                    {
                        uElementTable = Convert.ToString(dsUelement.Tables[0].Rows[0][0]);
                        uElementColumn = Convert.ToString(dsUelement.Tables[0].Rows[0][1]);
                    }

                    string sql = "SELECT distinct COALESCE(cl.vaf_column_id,cl.VAB_ACCT_ELEMENT_ID,cl.VAB_BillingCode_id,cl.VAB_BUSINESSPARTNER_ID,cl.VAB_PROMOTION_ID,cl.VAB_ADDRESS_ID,cl.VAB_PROJECT_ID ,cl.VAB_SALESREGIONSTATE_ID,cl.VAM_Product_ID,cl.ORG_ID) AS DimensionValue," +
                                  " cl.amount,ct.VAB_AccountBook_id,ct.elementtype,rl.Name as DimensionType, " +
                                   " COALESCE(o.name ";
                    if (uElementColumn != "")
                    {
                        sql += " ,(" + uElementColumn + ") ";
                    }
                    sql += " ,cel.Name,act.Name,cb.Name,cc.Name,cloc.address1,cpr.Name,cs.Name,mp.NAME) AS DimensionName ,nvl(cl.VAB_ELEMENT_ID,0) as ElementID,cl.VAB_DimAmtline_id as LineID, cl.VAB_BusinessPartner_ID, cb.Name AS BPartnerName " +
                        " from VAB_DimAmt cdm ";

                    //{
                    //    sql += " left join VAB_DimAmtaccttype ct on cdm.VAB_DimAmt_id=ct.VAB_DimAmt_id " +
                    //       "  Left join  VAB_DimAmtline cl ON cl.VAB_DimAmt_ID=ct.VAB_DimAmt_ID and cl.VAB_DimAmtaccttype_id=ct.VAB_DimAmtaccttype_id" +
                    //       " left JOIN VAB_AccountBook_element rl ON ct.elementtype     =rl.elementtype AND ct.VAB_AccountBook_id=rl.VAB_AccountBook_ID " +
                    //       " left join VAF_CtrlRef_List adref on adref.value=ct.elementtype " +
                    //       " left join vaf_column adc on adc.VAF_Control_Ref_Value_ID=adref.VAF_Control_Ref_ID and adc.export_id='VIS_2663'";
                    //}
                    //else
                    //{
                    sql += " inner join VAB_DimAmtaccttype ct on cdm.VAB_DimAmt_id=ct.VAB_DimAmt_id " +
                       "  inner join  VAB_DimAmtline cl ON cl.VAB_DimAmt_ID=ct.VAB_DimAmt_ID and cl.VAB_DimAmtaccttype_id=ct.VAB_DimAmtaccttype_id" +
                       " inner JOIN VAB_AccountBook_element rl ON ct.elementtype     =rl.elementtype AND ct.VAB_AccountBook_id=rl.VAB_AccountBook_ID " +
                       " inner join VAF_CtrlRef_List adref on adref.value=ct.elementtype " +
                       " inner join vaf_column adc on adc.VAF_Control_Ref_Value_ID=adref.VAF_Control_Ref_ID and adc.export_id='VIS_2663'";

                    // }
                    if (uElementTable != "")
                    {
                        sql += " LEFT JOIN " + uElementTable + " ac ON cl.vaf_column_ID=ac." + uElementTable + "_ID ";
                    }

                    sql +=
                       " LEFT JOIN VAB_BillingCode act ON cl.VAB_BillingCode_id=act.VAB_BillingCode_id " +
                       " LEFT JOIN VAB_BUSINESSPARTNER cb ON cl.VAB_BUSINESSPARTNER_ID=cb.VAB_BUSINESSPARTNER_ID " +
                       " LEFT JOIN VAB_PROMOTION cc ON cl.VAB_PROMOTION_ID=cc.VAB_PROMOTION_ID " +
                       " LEFT JOIN VAB_ACCT_ELEMENT cel ON cl.VAB_ACCT_ELEMENT_ID=cel.VAB_ACCT_ELEMENT_ID " +
                       " LEFT JOIN VAB_ELEMENT el ON cl.VAB_ELEMENT_ID=el.VAB_ELEMENT_ID " +
                       " LEFT JOIN VAB_ADDRESS cloc ON cl.VAB_ADDRESS_ID=cloc.VAB_ADDRESS_ID " +
                       " LEFT JOIN VAB_PROJECT cpr ON cl.VAB_PROJECT_ID=cpr.VAB_PROJECT_ID " +
                       " LEFT JOIN VAB_SALESREGIONSTATE cs ON cl.VAB_SALESREGIONSTATE_ID=cs.VAB_SALESREGIONSTATE_ID " +
                       " LEFT JOIN VAM_Product mp ON cl.VAM_Product_ID=mp.VAM_Product_ID " +
                       " LEFT JOIN vaf_org o ON cl.org_id=o.vaf_org_id " +
                       " WHERE cdm.VAB_DimAmt_ID=" + dimensionID + " and ct.VAB_AccountBook_id=" + acctId;
                    if (DimensionLineID != 0)
                    {
                        sql += " and cl.VAB_DimAmtline_id=" + DimensionLineID;
                    }

                    sql += " order by  ct.VAB_AccountBook_id";
                    DataSet ds = new DataSet();
                    if (pSize == 0 || DimensionLineID != 0)
                    {
                        ds = DB.ExecuteDataset(sql);
                    }
                    else
                    {
                        ds = VIS.DBase.DB.ExecuteDatasetPaging(sql, pageNo, pSize);
                    }

                    string sqlcount = "select count(*),sum(lineTotalAmount) as lineTotalAmount  from ( " +
                                      " SELECT distinct (cl.Amount) AS LineTotalAmount,cdm.VAB_DimAmt_id,ct.elementtype,cl.VAB_DimAmtline_id from VAB_DimAmt cdm ";
                    if (tempDimensionID == 0)
                    {
                        sqlcount += " left join VAB_DimAmtaccttype ct on cdm.VAB_DimAmt_id=ct.VAB_DimAmt_id " +
                           "  Left join  VAB_DimAmtline cl ON cl.VAB_DimAmt_ID=ct.VAB_DimAmt_ID and cl.VAB_DimAmtaccttype_id=ct.VAB_DimAmtaccttype_id" +
                           " left JOIN VAB_AccountBook_element rl ON ct.elementtype     =rl.elementtype AND ct.VAB_AccountBook_id=rl.VAB_AccountBook_ID " +
                           " left join VAF_CtrlRef_List adref on adref.value=ct.elementtype " +
                           " left join vaf_column adc on adc.VAF_Control_Ref_Value_ID=adref.VAF_Control_Ref_ID and adc.export_id='VIS_2663'";
                    }
                    else
                    {
                        sqlcount += " inner join VAB_DimAmtaccttype ct on cdm.VAB_DimAmt_id=ct.VAB_DimAmt_id " +
                           "  inner join  VAB_DimAmtline cl ON cl.VAB_DimAmt_ID=ct.VAB_DimAmt_ID and cl.VAB_DimAmtaccttype_id=ct.VAB_DimAmtaccttype_id" +
                           " inner JOIN VAB_AccountBook_element rl ON ct.elementtype     =rl.elementtype AND ct.VAB_AccountBook_id=rl.VAB_AccountBook_ID " +
                           " inner join VAF_CtrlRef_List adref on adref.value=ct.elementtype " +
                           " inner join vaf_column adc on adc.VAF_Control_Ref_Value_ID=adref.VAF_Control_Ref_ID and adc.export_id='VIS_2663'";

                    }
                    sqlcount += " LEFT JOIN vaf_column ac ON cl.vaf_column_ID=ac.vaf_column_ID " +
                       " LEFT JOIN VAB_BillingCode act ON cl.VAB_BillingCode_id=act.VAB_BillingCode_id " +
                       " LEFT JOIN VAB_BUSINESSPARTNER cb ON cl.VAB_BUSINESSPARTNER_ID=cb.VAB_BUSINESSPARTNER_ID " +
                       " LEFT JOIN VAB_PROMOTION cc ON cl.VAB_PROMOTION_ID=cc.VAB_PROMOTION_ID " +
                       " LEFT JOIN VAB_ACCT_ELEMENT cel ON cl.VAB_ACCT_ELEMENT_ID=cel.VAB_ACCT_ELEMENT_ID " +
                       " LEFT JOIN VAB_ELEMENT el ON cl.VAB_ELEMENT_ID=el.VAB_ELEMENT_ID " +
                       " LEFT JOIN VAB_ADDRESS cloc ON cl.VAB_ADDRESS_ID=cloc.VAB_ADDRESS_ID " +
                       " LEFT JOIN VAB_PROJECT cpr ON cl.VAB_PROJECT_ID=cpr.VAB_PROJECT_ID " +
                       " LEFT JOIN VAB_SALESREGIONSTATE cs ON cl.VAB_SALESREGIONSTATE_ID=cs.VAB_SALESREGIONSTATE_ID " +
                       " LEFT JOIN VAM_Product mp ON cl.VAM_Product_ID=mp.VAM_Product_ID " +
                       " LEFT JOIN vaf_org o ON cl.org_id=o.vaf_org_id " +
                       " WHERE cdm.VAB_DimAmt_ID=" + dimensionID + " and ct.VAB_AccountBook_id=" + acctId + "  ) main";//order by  ct.VAB_AccountBook_id

                    DataSet Record = DB.ExecuteDataset(sqlcount);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            AmountDivisionModel obj = new AmountDivisionModel();
                            obj.recid = tempRecid + (i + 1);
                            obj.AcctSchema = Convert.ToInt32(ds.Tables[0].Rows[i]["VAB_AccountBook_id"]);
                            if (ds.Tables[0].Rows[i]["amount"] != DBNull.Value)
                            {

                                //obj.DimensionValueAmount = DisplayType.GetNumberFormat(DisplayType.Amount).GetFormatedValue(Convert.ToDecimal(ds.Tables[0].Rows[i]["amount"]));
                                obj.DimensionValueAmount = Util.GetValueOfString(ds.Tables[0].Rows[i]["amount"]);
                                obj.CalculateDimValAmt = Convert.ToDecimal(ds.Tables[0].Rows[i]["amount"]);
                            }
                            else
                            {
                                //obj.DimensionValueAmount = DisplayType.GetNumberFormat(DisplayType.Amount).GetFormatedValue(0);
                                obj.DimensionValueAmount = Util.GetValueOfString(0);
                                obj.CalculateDimValAmt = 0;
                            }
                            obj.DimensionName = Convert.ToString(ds.Tables[0].Rows[i]["DimensionName"]);
                            if (ds.Tables[0].Rows[i]["DimensionValue"] != DBNull.Value)
                            {
                                obj.DimensionNameVal = Convert.ToInt32(ds.Tables[0].Rows[i]["DimensionValue"]);
                            }
                            else
                            {
                                obj.DimensionNameVal = 0;
                            }
                            obj.VAB_BusinessPartner_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_BusinessPartner_ID"]);
                            obj.VAB_BusinessPartner = Convert.ToString(ds.Tables[0].Rows[i]["BPartnerName"]);

                            obj.DimensionType = Convert.ToString(ds.Tables[0].Rows[i]["DimensionType"]);
                            obj.DimensionTypeVal = Convert.ToString(ds.Tables[0].Rows[i]["elementtype"]);
                            if (obj.DimensionTypeVal == "LF" || obj.DimensionTypeVal == "LT")
                            {
                                MVABAddress objMLocation = new MVABAddress(ctx, obj.DimensionNameVal, null);
                                obj.DimensionName = objMLocation.ToString();
                            }
                            obj.ElementID = Convert.ToInt32(ds.Tables[0].Rows[i]["ElementID"]);
                            if (ds.Tables[0].Rows[i]["LineID"] != DBNull.Value)
                            {
                                obj.lineAmountID = Convert.ToString(ds.Tables[0].Rows[i]["LineID"]);
                            }
                            else
                            {
                                obj.lineAmountID = "0";
                            }
                            if (Record != null && Record.Tables[0].Rows.Count > 0)
                            {
                                if (Record.Tables[0].Rows[0][0] != DBNull.Value)
                                {
                                    obj.TotalRecord = Convert.ToInt32(Record.Tables[0].Rows[0][0]);
                                }
                                else
                                {
                                    obj.TotalRecord = 0;
                                }
                                if (Record.Tables[0].Rows[0][1] != DBNull.Value)
                                {
                                    obj.TotalLineAmount = Convert.ToDecimal(Record.Tables[0].Rows[0][1]);
                                }
                                else
                                {
                                    obj.TotalLineAmount = 0;
                                }
                            }
                            else
                            {
                                obj.TotalRecord = 0;
                                obj.TotalLineAmount = 0;
                            }
                            objAmtdimModel.Add(obj);
                        }
                    }
                }

            }
            return objAmtdimModel;
        }

        /// <summary>
        /// Get Element linked with Account on Accounting Schema Element
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="accountingSchema">selected Accounting Schema</param>
        /// <returns>Element ID</returns>
        public int GetElementID(Ctx ctx, int[] accountingSchema)
        {
            int VAB_Element_id = 0;
            List<int> elementID = new List<int>();
            for (int i = 0; i < accountingSchema.Length; i++)
            {
                string qry = "SELECT VAB_Element_ID FROM VAB_AccountBook_Element WHERE ElementType = 'AC' AND IsActive='Y' AND VAB_AccountBook_ID = " + accountingSchema[i];
                VAB_Element_id = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, null));
                elementID.Add(VAB_Element_id);
                if (i > 0)
                {
                    if (elementID[i - 1] != elementID[i])
                    {
                        VAB_Element_id = 0;
                        break;
                    }
                }
            }

            return VAB_Element_id;
        }

        /// <summary>
        /// get the Id of the table
        /// </summary>
        /// <param name="columName">value of the columnname</param>
        /// <param name="tableName">name of the table</param>
        /// <param name="value">value</param>
        /// <returns>Id of the table</returns>
        public int DimnesionValue(string columName, string tableName, string value)
        {
            string qry = null;
            int DimensionId = 0;
            try
            {
                qry = "SELECT " + columName + " from " + tableName + "  WHERE  value ='" + value + "' OR Name='" + value + "'";
                DimensionId = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, null));
            }
            catch
            {
                qry = "SELECT " + columName + " from " + tableName + "  WHERE  Name ='" + value + "'";
                DimensionId = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, null));
            }
            return DimensionId;
        }
        /// <summary>
        /// get the Id of the table
        /// </summary>
        /// <param name="columName">value of the columnname</param>
        /// <param name="tableName">name of the table</param>
        /// <param name="value">value</param>
        /// <returns>Id of the table</returns>
        public int UserElementDimnesionValue(string columName, string tableName, string value)
        {
            string qry = null;
            int DimensionId = 0;
            qry = "SELECT " + tableName + "_ID from " + tableName + "  WHERE  " + columName + " ='" + value + "'";
            DimensionId = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, null));
            return DimensionId;
        }

        /// <summary>
        /// get the details of Element
        /// </summary>
        /// <param name="acctschemaid"> value of the acctschemaid</param>
        /// <param name="type">value of the</param>
        /// <param name="value">value</param>
        /// <param name="rowNo">rowno of the table</param>
        /// <param name="dt">data of the table</param>
        /// <returns>Object of the datatable</returns>
        public DataTable GetAcountIdByValue(int acctschemaid, string type, string value, int rowNo, DataTable dt)
        {
            string qry = null;
            //DataSet ds1 = ds;
            qry = "select cv.VAB_Acct_Element_ID, ac.VAB_Element_ID, cv.value || '_' || cv.name AS ElementName from VAB_AccountBook_element ac "
                  + " inner join VAB_Element ce on ce.VAB_Element_ID = ac.VAB_Element_ID"
                  + " inner JOIN VAB_Acct_Element cv on cv.VAB_Element_ID = ce.VAB_Element_ID where ac.VAB_AccountBook_id =" + acctschemaid + " AND  "
                  + " ac.ElementType = '" + type + "'  and cv.Value = '" + value + "'";
            DataSet dsAccountElement = (DB.ExecuteDataset(qry, null, null));
            if (dsAccountElement != null && dsAccountElement.Tables.Count > 0 && dsAccountElement.Tables[0].Rows.Count > 0)
            {
                dt.Rows[rowNo]["AccoutId"] = dsAccountElement.Tables[0].Rows[0]["VAB_Acct_Element_ID"];
                dt.Rows[rowNo]["AccountValue"] = dsAccountElement.Tables[0].Rows[0]["ElementName"];
                dt.Rows[rowNo]["VAB_Element_ID"] = dsAccountElement.Tables[0].Rows[0]["VAB_Element_ID"];
            }
            return dt;
        }


    }

    public class UploadResponse
    {
        public string _path { get; set; }
        public string _filename { get; set; }
        public string _error { get; set; }
        public string _statementID { get; set; }
        public string _orgfilename { get; set; }
    }
}
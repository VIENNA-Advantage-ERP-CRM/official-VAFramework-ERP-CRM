using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.DataBase;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class CalculateCrossCurrRate : SvrProcess
    {
        StringBuilder retmsg = new StringBuilder();
        StringBuilder query = new StringBuilder();
        string name = string.Empty;
        int Client_ID = 0;
        int Org_ID = 0;
        int CurrType = 0;
        int CommonCurr = 0;
        int FromCurr = 0;
        int ToCurr = 0;
        int FromCurr1 = 0;
        DateTime ValidFrom1 = new DateTime();
        DateTime ValidFrom2 = new DateTime();
        DateTime ValidTo1 = new DateTime();
        DateTime ValidTo2 = new DateTime();
        Decimal newMulRate = 0;
        Decimal newDivideRate = 0;
        Decimal MulRate1, MulRate2 = 0;
        DateTime newValidFrom = new DateTime();
        DateTime newValidTo = new DateTime();

        protected override string DoIt()
        {
            //string sysDate = System.DateTime.Now.Date;
             string sysDate = "trunc(sysdate)";
            if (DB.IsPostgreSQL())
            {
                sysDate = "current_timestamp";
            }
            // Getting records from cross rate setting
            query.Append("SELECT * FROM C_CurrCrossRate WHERE IsActive='Y'");
            DataSet dsobj = DB.ExecuteDataset(query.ToString());
            query.Clear();
            if (dsobj != null && dsobj.Tables.Count > 0 && dsobj.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsobj.Tables[0].Rows.Count; i++)
                {
                    Org_ID = Util.GetValueOfInt(dsobj.Tables[0].Rows[i]["AD_Org_ID"]);
                    name= Util.GetValueOfString(dsobj.Tables[0].Rows[i]["Name"]);
                    CurrType = Util.GetValueOfInt(dsobj.Tables[0].Rows[i]["C_ConversionType_ID"]);
                    CommonCurr = Util.GetValueOfInt(dsobj.Tables[0].Rows[i]["C_Currency_ID"]);
                    FromCurr = Util.GetValueOfInt(dsobj.Tables[0].Rows[i]["C_Currency_From_ID"]);
                    ToCurr = Util.GetValueOfInt(dsobj.Tables[0].Rows[i]["C_Currency_To_ID"]);
                    // Getting records from currency rate based on conditions
                    query.Append("SELECT AD_Client_ID,AD_Org_ID,C_Currency_ID,ValidFrom,ValidTo,MultiplyRate FROM C_Conversion_Rate WHERE " +  sysDate + " BETWEEN ValidFrom AND ValidTo AND IsActive='Y' AND AD_Org_ID=" + Org_ID + " AND C_ConversionType_ID=" + CurrType + " AND C_Currency_To_ID=" + CommonCurr + " AND C_Currency_ID IN ('" + FromCurr + "','" + ToCurr + "')");
                    DataSet ds = DB.ExecuteDataset(query.ToString());
                    query.Clear();
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count == 2)
                    {
                        Client_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Client_ID"]);
                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                        {
                            //Org_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j]["AD_Org_ID"]);
                            FromCurr1 = Util.GetValueOfInt(ds.Tables[0].Rows[j]["C_Currency_ID"]);
                            // Getting multiply rate from both records
                            if (FromCurr == FromCurr1)
                            {
                                ValidFrom1 = Convert.ToDateTime(ds.Tables[0].Rows[j]["ValidFrom"]);
                                ValidTo1 = Convert.ToDateTime(ds.Tables[0].Rows[j]["ValidTo"]);
                                MulRate1 = Util.GetValueOfDecimal(ds.Tables[0].Rows[j]["MultiplyRate"]);
                            }
                            else if (ToCurr == FromCurr1)
                            {
                                ValidFrom2 = Convert.ToDateTime(ds.Tables[0].Rows[j]["ValidFrom"]);
                                ValidTo2 = Convert.ToDateTime(ds.Tables[0].Rows[j]["ValidTo"]);
                                MulRate2 = Util.GetValueOfDecimal(ds.Tables[0].Rows[j]["MultiplyRate"]);
                            }
                            else
                            {
                                retmsg.Append("Error: " + " Currency not matched \n");
                            }
                        }
                        // Calculating new multiply rate & divide rate
                        if (MulRate2 > 0)
                        {
                            newMulRate = MulRate1 / MulRate2;
                            newDivideRate = 1 / newMulRate;

                        }
                        else
                        {
                            retmsg.Append("Error: " + " Multiply rate must be greater than 0");
                        }

                        // Geting Valid from and valid to dates
                        if (ValidFrom1 == ValidFrom2 && ValidTo1 == ValidTo2)
                        {
                            newValidFrom = ValidFrom1;
                            newValidTo = ValidTo1;
                        }
                        else if ((ValidFrom1 < ValidFrom2 && ValidTo1 == ValidTo2) || (ValidFrom1 < ValidFrom2 && ValidTo1 < ValidTo2))
                        {
                            newValidFrom = ValidFrom2;
                            newValidTo = ValidTo1;
                        }
                        else if ((ValidFrom1 > ValidFrom2 && ValidTo1 == ValidTo2) || (ValidFrom1 > ValidFrom2 && ValidTo1 > ValidTo2))
                        {
                            newValidFrom = ValidFrom1;
                            newValidTo = ValidTo2;
                        }
                        else if (ValidFrom1 == ValidFrom2 && ValidTo1 < ValidTo2)
                        {
                            newValidFrom = ValidFrom1;
                            newValidTo = ValidTo1;
                        }
                        else if (ValidFrom1 == ValidFrom2 && ValidTo1 > ValidTo2)
                        {
                            newValidFrom = ValidFrom1;
                            newValidTo = ValidTo2;
                        }
                        else
                        {
                            retmsg.Append("\n Error: " + Msg.GetMsg("Not Valid Dates", ""));
                        }
                        query.Clear();
                        //int ID = DB.GetNextID(0, "C_Conversion_Rate", null);
                        //query.Append("INSERT INTO C_Conversion_Rate (AD_Client_ID,AD_Org_ID,C_Conversion_Rate_ID,C_Currency_ID,C_Currency_To_ID,C_ConversionType_ID,ValidFrom,ValidTo,MultiplyRate,DivideRate,CreatedBy,UpdatedBy) "
                        //    + "Values (" + Client_ID + ", " + Org_ID + ","+ ID +", " + FromCurr + ", " + ToCurr + ", " + CurrType + ", TO_DATE('" + newValidFrom.ToShortDateString() + "','MM-DD-YYYY'), TO_DATE('" + newValidTo.ToShortDateString() + "','MM-DD-YYYY') , " + newMulRate + ", " + newDivideRate + ", " + Client_ID + ", "+ Client_ID + ") ");
                        //int _updated = DB.ExecuteQuery(query.ToString(), null);
                        //if (_updated > 0)
                        //{
                        //    Msg.GetMsg("Record Inserted","");
                        //}
                        // Inserting new conversion in the currency rate
                        MConversionRate conobj = new MConversionRate(GetCtx(), 0, Get_Trx());
                        conobj.SetAD_Client_ID(Client_ID);
                        conobj.SetAD_Org_ID(Org_ID);
                        conobj.SetC_Currency_ID(FromCurr);
                        conobj.SetC_Currency_To_ID(ToCurr);
                        conobj.SetC_ConversionType_ID(CurrType);
                        conobj.SetValidFrom(newValidFrom);
                        conobj.SetValidTo(newValidTo);
                        conobj.SetMultiplyRate(newMulRate);
                        conobj.SetDivideRate(newDivideRate);
                        if (!conobj.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                retmsg.Append("Cross rate not calculated " + ":- " + pp.GetName() + name + ", ");
                            }
                            else
                            {
                                retmsg.Append(Msg.GetMsg("Cross rate not calculated", ""));
                            }
                        }
                        else
                        {
                            retmsg.Append("Cross rate calculated for" + ":- " + name + ", ");
                        }
                    }
                    else
                    {
                        retmsg.Append(" Cross rate not calculated for :" + name + ", ");
                    }
                }
            }
            else
            {
                retmsg.Append(Msg.GetMsg(GetCtx(),"NoRecords"));
            }
            return retmsg.ToString();
        }

        protected override void Prepare()
        {
        }

    }
}

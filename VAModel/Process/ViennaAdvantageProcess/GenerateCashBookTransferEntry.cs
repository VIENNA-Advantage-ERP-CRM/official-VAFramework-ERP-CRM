using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using ViennaAdvantage.Model;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.Model;

namespace ViennaAdvantage.Process
{
    public class GenerateCashBookTransferEntry : SvrProcess
    {
        private int C_Cash_ID = 0;
        MCashLine cashline = null;
        MCash cash = null;
        //   bool completed = false;
        string sql = "";
        Decimal convertedamount = 0;
        private int Currency_ID = 0;
        private Decimal _Curencyrate = 0;
        int cashtype = 0;
        List<int> _cashIds = new List<int>();
        int i;
        protected override string DoIt()
        {

            MCash cashheader = new MCash(GetCtx(), GetRecord_ID(), Get_Trx());
            sql = "select C_Currency_ID from  C_CashBook where c_Cashbook_id=" + cashheader.GetC_CashBook_ID();
            int C_Currencyheader_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
            sql = "select * from C_Cashline WHERE C_Cash_ID=" + GetRecord_ID();
            DataSet dscashline = DB.ExecuteDataset(sql, null, Get_Trx());
            if (dscashline != null)
            {
                if (dscashline.Tables[0].Rows.Count > 0)
                {

                    for (i = 0; i < dscashline.Tables[0].Rows.Count; i++)
                    {
                        cashline = new MCashLine(GetCtx(), Util.GetValueOfInt(dscashline.Tables[0].Rows[i]["C_CashLine_ID"]), Get_Trx());
                        if (cashline.GetCashType().ToString() == "A" || cashline.GetCashType().ToString() == "F")
                        {
                            sql = "Select * from C_Cash where C_CashBook_Id=" + cashline.GetC_CashBook_ID() + " and docstatus='DR' and  DateAcct=" + GlobalVariable.TO_DATE(DateTime.Now, true);
                            DataSet dscashbook = DB.ExecuteDataset(sql, null, Get_Trx());
                            if (dscashbook != null)
                            {
                                if (dscashbook.Tables[0].Rows.Count > 0)
                                {
                                    int j;
                                    for (j = 0; j < dscashbook.Tables[0].Rows.Count; j++)
                                    {

                                        cash = new MCash(GetCtx(), Util.GetValueOfInt(dscashbook.Tables[0].Rows[j]["C_Cash_ID"]), Get_Trx());
                                        if (!_cashIds.Contains(cash.GetC_Cash_ID()))
                                        {
                                            _cashIds.Add(cash.GetC_Cash_ID());
                                        }
                                        //ViennaAdvantage.Model.MCashLine cashline1 = new ViennaAdvantage.Model.MCashLine(GetCtx(), 0, Get_Trx());
                                        MCashLine cashline1 = new MCashLine(GetCtx(), 0, Get_Trx());
                                        cashline1.SetC_Cash_ID(cash.GetC_Cash_ID());
                                        cashline1.SetAD_Client_ID(cash.GetAD_Client_ID());
                                        cashline1.SetAD_Org_ID(cash.GetAD_Org_ID());
                                        if (cashline.GetCashType().ToString() == "A")
                                        {
                                            cashline1.SetCashType("F");
                                        }
                                        if (cashline.GetCashType().ToString() == "F")
                                        {
                                            cashline1.SetCashType("A");
                                        }
                                        // Added by Bharat as discussed with Ravikant on 22 March 2017
                                        if (cashline1.Get_ColumnIndex("C_ConversionType_ID") > 0)
                                        {
                                            cashline1.SetC_ConversionType_ID(cashline.GetC_ConversionType_ID());
                                        }
                                        cashline1.SetC_CashBook_ID(cashheader.GetC_CashBook_ID());
                                        cashline1.SetC_BPartner_ID(cashline.GetC_BPartner_ID());
                                        sql = "select C_Currency_ID from  C_CashBook where c_Cashbook_id=" + cash.GetC_CashBook_ID();
                                        Currency_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                                        if (Currency_ID == C_Currencyheader_ID)
                                        {
                                            cashline1.SetC_Currency_ID(C_Currencyheader_ID);
                                            cashline1.SetAmount(Decimal.Negate(cashline.GetAmount()));
                                            cashline1.SetConvertedAmt(Util.GetValueOfString(Decimal.Negate(cashline.GetAmount())));
                                        }
                                        else
                                        {
                                            //cashline1.SetC_Currency_ID(Currency_ID);
                                            // Change by Bharat as discussed with Ravikant on 22 March 2017
                                            cashline1.SetC_Currency_ID(C_Currencyheader_ID);
                                            if (cashline1.Get_ColumnIndex("C_ConversionType_ID") > 0)
                                            {
                                                //_Curencyrate = MConversionRate.GetRate(C_Currencyheader_ID, Currency_ID, cash.GetDateAcct(), cashline.GetC_ConversionType_ID(),
                                                //    cash.GetAD_Client_ID(), cash.GetAD_Org_ID());

                                                convertedamount = Decimal.Negate(Util.GetValueOfDecimal(cashline.GetConvertedAmt()));

                                                // if converted amount not found then get amount based on currency conversion avaliable
                                                if (cashline.GetAmount() != 0 && convertedamount == 0)
                                                {
                                                    convertedamount = MConversionRate.Convert(GetCtx(), Decimal.Negate(cashline.GetAmount()), Currency_ID, C_Currencyheader_ID,
                                                        cash.GetDateAcct(), cashline.GetC_ConversionType_ID(), cash.GetAD_Client_ID(), cash.GetAD_Org_ID());
                                                    cashline.SetConvertedAmt(Util.GetValueOfString(Decimal.Negate(convertedamount)));
                                                }

                                                if (cashline.GetAmount() != 0 && convertedamount == 0)
                                                {
                                                    return Msg.GetMsg(GetCtx(), "NoCurrencyRateDefined");
                                                }
                                                else if (cashline.GetAmount() == 0)
                                                {
                                                    return "Amount Should be greater than zero";
                                                }
                                                cashline1.SetAmount(convertedamount);
                                                cashline1.SetConvertedAmt(Util.GetValueOfString(Decimal.Negate(cashline.GetAmount())));
                                            }
                                            else
                                            {
                                                sql = "select multiplyrate from c_conversion_rate where isactive='Y' and c_currency_id=" + C_Currencyheader_ID + " and c_currency_to_id=" + Currency_ID;
                                                _Curencyrate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_Trx()));
                                                if (_Curencyrate == 0)
                                                {
                                                    return Msg.GetMsg(GetCtx(), "NoCurrencyRateDefined");
                                                }
                                                else
                                                {
                                                    convertedamount = Decimal.Round(Decimal.Multiply(_Curencyrate, Decimal.Negate(cashline.GetAmount())), 2);
                                                }
                                                cashline1.SetConvertedAmt(Util.GetValueOfString(convertedamount));
                                                cashline1.SetAmount(convertedamount);
                                            }
                                        }

                                        cashline1.SetC_Charge_ID(cashline.GetC_Charge_ID());
                                        if (cashline.GetCashType().ToString() == "A")
                                        {
                                            cashline1.SetVSS_PAYMENTTYPE("R");
                                        }
                                        if (cashline.GetCashType().ToString() == "F")
                                        {
                                            cashline1.SetVSS_PAYMENTTYPE("P");

                                        }
                                        cashline1.SetC_CashLine_ID_1(cashline.GetC_CashLine_ID());
                                        if (!cashline1.Save())
                                        {
                                            Rollback();
                                            log.Severe("NotSaved");
                                            return " NotSaved Cashline1 [ Header Found ]  ";

                                        }
                                        if (cashline.GetCashType().ToString() == "F")
                                        {
                                            cashline.SetC_CashLine_ID_1(cashline1.GetC_CashLine_ID());
                                        }
                                        if (!cashline.Save())
                                        {
                                            Rollback();
                                            log.Severe("Not Saved");
                                            return " NotSaved Cashline [ Header Found ]  ";
                                        }
                                        //change by Amit 1-june-2016 after discussion with ravikant

                                        //int BeginBal = Util.GetValueOfInt(DB.ExecuteScalar("select completedbalance from c_cashbook where c_cashbook_id=" + cash.GetC_CashBook_ID(), null, Get_Trx()));
                                        //cash.SetBeginningBalance(BeginBal);
                                        //if (!cash.Save(Get_Trx()))
                                        //{
                                        //    Rollback();
                                        //    log.Severe("Not Saved");
                                        //    return " NotSaved Cash [ Header Found ]  ";

                                        //}

                                        //end

                                        break;
                                    }


                                }
                                else
                                {
                                    dscashbook.Dispose();

                                    MCash cash1 = new MCash(GetCtx(), 0, Get_Trx());
                                    cash1.SetAD_Client_ID(cashheader.GetAD_Client_ID());
                                    cash1.SetAD_Org_ID(cashheader.GetAD_Org_ID());
                                    cash1.SetC_DocType_ID(cashheader.GetC_DocType_ID());
                                    cash1.SetName(Util.GetValueOfString(System.DateTime.Today.ToShortDateString()));
                                    cash1.SetC_CashBook_ID(cashline.GetC_CashBook_ID());
                                    cash1.SetDateAcct(DateTime.Now);
                                    cash1.SetStatementDate(DateTime.Now);
                                    //change by Amit 1-june-2016 after discussion with ravikant
                                    int BeginBal = Util.GetValueOfInt(DB.ExecuteScalar("select SUM(completedbalance + runningbalance) from c_cashbook where c_cashbook_id=" + cashline.GetC_CashBook_ID(), null, Get_Trx()));
                                    cash1.SetBeginningBalance(BeginBal);
                                    //end
                                    //Change by Bharat as discussed with Ravikant on 22 March 2017
                                    sql = "select C_Currency_ID from  C_CashBook where c_Cashbook_id=" + cash1.GetC_CashBook_ID();
                                    Currency_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                                    if (cash1.Get_ColumnIndex("C_Currency_ID") > 0)
                                    {
                                        cash1.SetC_Currency_ID(Currency_ID);
                                    }
                                    //End
                                    if (!cash1.Save())
                                    {
                                        Rollback();

                                        VAdvantage.Model.ValueNamePair ppE = VLogger.RetrieveError();
                                        if (ppE != null)
                                        {
                                            return ppE.GetName() + "<--->" + ppE.GetValue();
                                        }

                                        return "cash1 not saved [header not found->]";
                                    }
                                    // return cash1.GetC_Cash_ID() + "suc";
                                    if (!_cashIds.Contains(cash1.GetC_Cash_ID()))
                                    {
                                        _cashIds.Add(cash1.GetC_Cash_ID());
                                    }
                                    MCashLine cashline1 = new MCashLine(GetCtx(), 0, Get_Trx());
                                    cashline1.SetC_Cash_ID(cash1.GetC_Cash_ID());
                                    cashline1.SetAD_Client_ID(cash1.GetAD_Client_ID());
                                    cashline1.SetAD_Org_ID(cash1.GetAD_Org_ID());
                                    if (cashline.GetCashType().ToString() == "A")
                                    {
                                        cashline1.SetCashType("F");
                                    }
                                    if (cashline.GetCashType().ToString() == "F")
                                    {
                                        cashline1.SetCashType("A");
                                    }
                                    if (cashline1.Get_ColumnIndex("C_ConversionType_ID") > 0)
                                    {
                                        cashline1.SetC_ConversionType_ID(cashline.GetC_ConversionType_ID());
                                    }
                                    cashline1.SetC_CashBook_ID(cashheader.GetC_CashBook_ID());
                                    cashline1.SetC_BPartner_ID(cashline.GetC_BPartner_ID());

                                    //Change by Bharat as discussed with Ravikant on 22 March 2017
                                    //sql = "select C_Currency_ID from  C_CashBook where c_Cashbook_id=" + cash1.GetC_CashBook_ID();
                                    //Currency_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                                    if (Currency_ID == C_Currencyheader_ID)
                                    {
                                        cashline1.SetAmount(Decimal.Negate(cashline.GetAmount()));
                                        cashline1.SetC_Currency_ID(C_Currencyheader_ID);
                                        cashline1.SetConvertedAmt(Util.GetValueOfString(Decimal.Negate(cashline.GetAmount())));
                                    }
                                    else
                                    {
                                        //Change by Bharat as discussed with Ravikant on 22 March 2017
                                        cashline1.SetC_Currency_ID(C_Currencyheader_ID);
                                        if (cashline1.Get_ColumnIndex("C_ConversionType_ID") > 0)
                                        {
                                            //_Curencyrate = MConversionRate.GetRate(C_Currencyheader_ID, Currency_ID, cashheader.GetDateAcct(), cashline.GetC_ConversionType_ID(),
                                            //    cashheader.GetAD_Client_ID(), cashheader.GetAD_Org_ID());
                                            convertedamount = Decimal.Negate(Util.GetValueOfDecimal(cashline.GetConvertedAmt()));

                                            // if converted amount not found then get amount based on currency conversion avaliable
                                            if (cashline.GetAmount() != 0 && convertedamount == 0)
                                            {
                                                convertedamount = MConversionRate.Convert(GetCtx(), Decimal.Negate(cashline.GetAmount()), Currency_ID, C_Currencyheader_ID,
                                                    cash1.GetDateAcct(), cashline.GetC_ConversionType_ID(), cash1.GetAD_Client_ID(), cash1.GetAD_Org_ID());
                                                cashline.SetConvertedAmt(Util.GetValueOfString(Decimal.Negate(convertedamount)));
                                            }

                                            if (cashline.GetAmount() != 0 && convertedamount == 0)
                                            {
                                                return Msg.GetMsg(GetCtx(), "NoCurrencyRateDefined");
                                            }
                                            else if (cashline.GetAmount() == 0)
                                            {
                                                return "Amount Should be greater than zero";
                                            }
                                            cashline1.SetAmount(convertedamount);
                                            cashline1.SetConvertedAmt(Util.GetValueOfString(Decimal.Negate(cashline.GetAmount())));
                                        }
                                        else
                                        {
                                            sql = "select multiplyrate from c_conversion_rate where isactive='Y' and c_currency_id=" + C_Currencyheader_ID + " and c_currency_to_id=" + Currency_ID;
                                            _Curencyrate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_Trx()));

                                            if (_Curencyrate == 0)
                                            {
                                                return Msg.GetMsg(GetCtx(), "NoCurrencyRateDefined");
                                            }
                                            else
                                            {
                                                convertedamount = Decimal.Round(Decimal.Multiply(_Curencyrate, Decimal.Negate(cashline.GetAmount())), 2);
                                            }
                                            cashline1.SetConvertedAmt(Util.GetValueOfString(convertedamount));
                                            cashline1.SetAmount(convertedamount);
                                        }

                                    }

                                    cashline1.SetC_Charge_ID(cashline.GetC_Charge_ID());
                                    if (cashline.GetCashType().ToString() == "A")
                                    {
                                        cashline1.SetVSS_PAYMENTTYPE("R");
                                    }
                                    if (cashline.GetCashType().ToString() == "F")
                                    {
                                        cashline1.SetVSS_PAYMENTTYPE("P");
                                    }
                                    cashline1.SetC_CashLine_ID_1(cashline.GetC_CashLine_ID());
                                    if (!cashline1.Save(Get_Trx()))
                                    {
                                        Rollback();
                                        log.Severe("NotSaved");
                                        return " NotSaved Cashline1 [ Header not Found ]  ";
                                    }
                                    if (cashline.GetCashType().ToString() == "F")
                                    {
                                        cashline.SetC_CashLine_ID_1(cashline1.GetC_CashLine_ID());
                                    }
                                    if (!cashline.Save(Get_Trx()))
                                    {
                                        Rollback();
                                        log.Severe("NotSaved");
                                        return " NotSaved Cashline [ Header not Found ]  ";
                                    }
                                    //change by Amit 1-june-2016 after discussion with ravikant

                                    //int BeginBal = Util.GetValueOfInt(DB.ExecuteScalar("select completedbalance from c_cashbook where c_cashbook_id=" + cash1.GetC_CashBook_ID(), null, Get_Trx()));
                                    //cash1.SetBeginningBalance(BeginBal);
                                    //if (!cash1.Save())
                                    //{
                                    //    Rollback();
                                    //    log.Severe("NotSaved");
                                    //    return " NotSaved Cash1 [ Header not Found(bb) ]  ";
                                    //}

                                    //end
                                }

                            }
                            else
                            {
                                //dscashbook.Dispose();
                            }
                        }
                        else
                        {
                            cashtype++;



                        }
                    }
                    // Discussed with Ravikant it is re updating the Header so commented on 23-March-2017 by Bharat
                    //decimal OpenBal = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT completedbalance FROM  C_CashBook WHERE c_cashbook_id=" + cashheader.GetC_CashBook_ID() + "", null, Get_Trx()));
                    //cashheader.SetBeginningBalance(OpenBal);
                    //if (!cashheader.Save())
                    //{
                    //    Rollback();
                    //    log.Severe("Not Saved");
                    //}
                }
                else
                {
                    dscashline.Dispose();
                    cashheader.SetGenerateCashBookTransfer("Y");
                    if (!cashheader.Save())
                    {
                        Rollback();
                        log.Severe("NotSaved");
                    }
                    return Msg.GetMsg(GetCtx(), "NoCashLineExist");
                }
                if (_cashIds.Count > 0)
                {
                    for (int k = 0; k < _cashIds.Count; k++)
                    {
                        cash = new MCash(GetCtx(), _cashIds[k], Get_Trx());
                        cash.SetDocStatus(cash.CompleteIt());
                        if (!cash.Save())
                        {
                            Rollback();
                            log.Severe("NotSaved");
                        }
                    }

                }
            }

            cashheader.SetGenerateCashBookTransfer("Y");
            if (!cashheader.Save())
            {
                log.Severe("NotSaved");
            }
            if (cashtype == i)
            {
                return Msg.GetMsg(GetCtx(), "NoCashLineExist");

            }
            return "Completed";

        }

        protected override void Prepare()
        {
            C_Cash_ID = GetRecord_ID();

        }
    }
}

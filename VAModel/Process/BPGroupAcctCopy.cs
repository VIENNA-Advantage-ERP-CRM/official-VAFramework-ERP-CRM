/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : BPGroupAcctCopy
 * Purpose        : Copy BP Group default Accounts
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           19-Dec-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class BPGroupAcctCopy:ProcessEngine.SvrProcess
    {
           //BP Group					
            private int _C_BP_Group_ID = 0;
          //Acct Schema					
            private int _C_AcctSchema_ID = 0;

            /// <summary>
            /// Prepare - e.g., get Parameters.
            /// </summary>
            protected override void Prepare()
            {
                ProcessInfoParameter[] para = GetParameter();
                for (int i = 0; i < para.Length; i++)
                {
                    String name = para[i].GetParameterName();
                    if (para[i].GetParameter() == null)
                    {
                        ;
                    }
                    else if (name.Equals("C_BP_Group_ID"))
                    {
                        _C_BP_Group_ID = para[i].GetParameterAsInt();
                    }
                    else if (name.Equals("C_AcctSchema_ID"))
                    {
                        _C_AcctSchema_ID = para[i].GetParameterAsInt();
                    }
                    else
                    {
                        log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                    }
                }
            }	//	prepare


            /// <summary>
            /// Process
            /// </summary>
            /// <returns>message</returns>
            protected override String DoIt()
            {
                log.Info("C_AcctSchema_ID=" + _C_AcctSchema_ID);
                if (_C_AcctSchema_ID == 0)
                {
                    throw new SystemException("C_AcctSchema_ID=0");
                }
                MAcctSchema ass = MAcctSchema.Get(GetCtx(), _C_AcctSchema_ID);
                if (ass.Get_ID() == 0)
                {
                    throw new SystemException("Not Found - C_AcctSchema_ID=" + _C_AcctSchema_ID);
                }
                //
                String sql = null;
                int _updated = 0;
                int _created = 0;
                int _updatedTotal = 0;
                int _createdTotal = 0;

                //	Update existing Customers
                sql = "UPDATE C_BP_Customer_Acct ca "
                    + "SET (C_Receivable_Acct,C_Receivable_Services_Acct,C_PrePayment_Acct)="
                     + " (SELECT C_Receivable_Acct,C_Receivable_Services_Acct,C_PrePayment_Acct "
                     + " FROM C_BP_Group_Acct"
                     + " WHERE C_BP_Group_ID=" + _C_BP_Group_ID
                     + " AND C_AcctSchema_ID=" + _C_AcctSchema_ID
                    + "), Updated=SysDate, UpdatedBy=0 "
                    + "WHERE ca.C_AcctSchema_ID=" + _C_AcctSchema_ID
                    + " AND EXISTS (SELECT * FROM C_BPartner p "
                        + "WHERE p.C_BPartner_ID=ca.C_BPartner_ID"
                        + " AND p.C_BP_Group_ID=" + _C_BP_Group_ID + ")";
                _updated = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
                AddLog(0, null, Utility.Util.GetValueOfDecimal(_updated), "@Updated@ @C_BPartner_ID@ @IsCustomer@");
                _updatedTotal += _updated;

                //	Insert new Customer
                sql = "INSERT INTO C_BP_Customer_Acct "
                    + "(C_BPartner_ID, C_AcctSchema_ID,"
                    + " AD_Client_ID, AD_Org_ID, IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                    + " C_Receivable_Acct, C_Receivable_Services_Acct, C_PrePayment_Acct) "
                    + "SELECT p.C_BPartner_ID, acct.C_AcctSchema_ID,"
                    + " p.AD_Client_ID, p.AD_Org_ID, 'Y', SysDate, 0, SysDate, 0,"
                    + " acct.C_Receivable_Acct, acct.C_Receivable_Services_Acct, acct.C_PrePayment_Acct "
                    + "FROM C_BPartner p"
                    + " INNER JOIN C_BP_Group_Acct acct ON (acct.C_BP_Group_ID=p.C_BP_Group_ID)"
                    + "WHERE acct.C_AcctSchema_ID=" + _C_AcctSchema_ID			//	#
                    + " AND p.C_BP_Group_ID=" + _C_BP_Group_ID
                    + " AND NOT EXISTS (SELECT * FROM C_BP_Customer_Acct ca "
                        + "WHERE ca.C_BPartner_ID=p.C_BPartner_ID"
                        + " AND ca.C_AcctSchema_ID=acct.C_AcctSchema_ID)";
                _created = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
                AddLog(0, null, Utility.Util.GetValueOfDecimal(_created), "@Created@ @C_BPartner_ID@ @IsCustomer@");
                _createdTotal += _created;


                //	Update existing Vendors
                sql = "UPDATE C_BP_Vendor_Acct va "
                    + "SET (V_Liability_Acct,V_Liability_Services_Acct,V_PrePayment_Acct)="
                     + " (SELECT V_Liability_Acct,V_Liability_Services_Acct,V_PrePayment_Acct "
                     + " FROM C_BP_Group_Acct"
                     + " WHERE C_BP_Group_ID=" + _C_BP_Group_ID
                     + " AND C_AcctSchema_ID=" + _C_AcctSchema_ID
                    + "), Updated=SysDate, UpdatedBy=0 "
                    + "WHERE va.C_AcctSchema_ID=" + _C_AcctSchema_ID
                    + " AND EXISTS (SELECT * FROM C_BPartner p "
                        + "WHERE p.C_BPartner_ID=va.C_BPartner_ID"
                        + " AND p.C_BP_Group_ID=" + _C_BP_Group_ID + ")";
                _updated = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
                AddLog(0, null, Utility.Util.GetValueOfDecimal(_updated), "@Updated@ @C_BPartner_ID@ @IsVendor@");
                _updatedTotal += _updated;

                //	Insert new Vendors
                sql = "INSERT INTO C_BP_Vendor_Acct "
                    + "(C_BPartner_ID, C_AcctSchema_ID,"
                    + " AD_Client_ID, AD_Org_ID, IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                    + " V_Liability_Acct, V_Liability_Services_Acct, V_PrePayment_Acct) "
                    + "SELECT p.C_BPartner_ID, acct.C_AcctSchema_ID,"
                    + " p.AD_Client_ID, p.AD_Org_ID, 'Y', SysDate, 0, SysDate, 0,"
                    + " acct.V_Liability_Acct, acct.V_Liability_Services_Acct, acct.V_PrePayment_Acct "
                    + "FROM C_BPartner p"
                    + " INNER JOIN C_BP_Group_Acct acct ON (acct.C_BP_Group_ID=p.C_BP_Group_ID)"
                    + "WHERE acct.C_AcctSchema_ID=" + _C_AcctSchema_ID			//	#
                    + " AND p.C_BP_Group_ID=" + _C_BP_Group_ID
                    + " AND NOT EXISTS (SELECT * FROM C_BP_Vendor_Acct va "
                        + "WHERE va.C_BPartner_ID=p.C_BPartner_ID AND va.C_AcctSchema_ID=acct.C_AcctSchema_ID)";
                _created = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
                AddLog(0, null, Utility.Util.GetValueOfDecimal(_created), "@Created@ @C_BPartner_ID@ @IsVendor@");
                _createdTotal += _created;

                return "@Created@=" + _createdTotal + ", @Updated@=" + _updatedTotal;
            }	//	doIt

        }	
}

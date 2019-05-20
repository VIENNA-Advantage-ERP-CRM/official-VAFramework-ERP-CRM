using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;
using System.Data;
using VAdvantage.Model;
using VAdvantage.Classes;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class DocumentTypeVerify : ProcessEngine.SvrProcess
    {
	    /**	Static Logger	*/
	    private static VLogger	_log	= VLogger.GetVLogger(typeof(DocumentTypeVerify).FullName);

        protected override void Prepare()
        {
        }	//	prepare

        protected override string DoIt()
        {
            CreateDocumentTypes(GetCtx(), GetAD_Client_ID(), this, Get_TrxName());
            CreatePeriodControls(GetCtx(), GetAD_Client_ID(), this, Get_TrxName());
            return "OK";            
        }

        public static void CreateDocumentTypes(Ctx ctx, int AD_Client_ID, SvrProcess sp, Trx trxName)
        {
            _log.Info("AD_Client_ID=" + AD_Client_ID);
            String sql = "SELECT rl.Value, rl.Name "
                + "FROM AD_Ref_List rl "
                + "WHERE rl.AD_Reference_ID=183"
                + " AND rl.IsActive='Y' AND NOT EXISTS "
                + " (SELECT * FROM C_DocType dt WHERE dt.AD_Client_ID='" + AD_Client_ID + "' AND rl.Value=dt.DocBaseType)";
            IDataReader idr=null;
            try
            {
               idr=DataBase.DB.ExecuteReader(sql, null);
                while(idr.Read())
                {
                    String name =idr[1].ToString();
                    String value = idr[0].ToString();
                    _log.Config(name + "=" + value);
                    MDocType dt = new MDocType(ctx, value, name, trxName);
                    if (dt.Save())
                    {
                        if (sp != null)
                        {
                            sp.AddLog(0, null, null, name);
                        }
                        else
                        {
                            _log.Fine(name);
                        }
                    }
                    else
                    {
                        if (sp != null)
                        {
                            sp.AddLog(0, null, null, "Not created: " + name);
                        }
                        else
                        {
                            _log.Warning("Not created: " + name);
                        }
                    }
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
        }	//	createDocumentTypes


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Client_ID"></param>
        /// <param name="sp"></param>
        /// <param name="trxName"></param>
        public static void CreatePeriodControls(Ctx ctx, int AD_Client_ID, SvrProcess sp, Trx trxName)
        {
            _log.Info("AD_Client_ID=" + AD_Client_ID);

            //	Delete Duplicates
            //jz remove correlation ID  String sql = "DELETE FROM C_PeriodControl pc1 "
            String sql = "DELETE FROM C_PeriodControl "
                + "WHERE (C_Period_ID, DocBaseType) IN "
                    + "(SELECT C_Period_ID, DocBaseType "
                    + "FROM C_PeriodControl pc2 "
                    + "GROUP BY C_Period_ID, DocBaseType "
                    + "HAVING COUNT(*) > 1)"
                + " AND C_PeriodControl_ID NOT IN "
                    + "(SELECT MIN(C_PeriodControl_ID) "
                    + "FROM C_PeriodControl pc3 "
                    + "GROUP BY C_Period_ID, DocBaseType)";
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            _log.Info("Duplicates deleted #" + no);

            //	Insert Missing
            sql = "SELECT DISTINCT p.AD_Client_ID, p.C_Period_ID, dt.DocBaseType "
                + "FROM C_Period p"
                + " FULL JOIN C_DocType dt ON (p.AD_Client_ID=dt.AD_Client_ID) "
                + "WHERE p.AD_Client_ID='" + AD_Client_ID + "'"
                + " AND NOT EXISTS"
                + " (SELECT * FROM C_PeriodControl pc "
                    + "WHERE pc.C_Period_ID=p.C_Period_ID AND pc.DocBaseType=dt.DocBaseType)";
            IDataReader idr = null;
            int counter = 0;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                while (idr.Read())
                {
                    int Client_ID = Utility.Util.GetValueOfInt(idr[0].ToString());
                    int C_Period_ID = Utility.Util.GetValueOfInt(idr[1].ToString());
                    String DocBaseType = idr[2].ToString();
                   _log.Config("AD_Client_ID=" + Client_ID
                        + ", C_Period_ID=" + C_Period_ID + ", DocBaseType=" + DocBaseType);
                    //
                    MPeriodControl pc = new MPeriodControl(ctx, Client_ID, C_Period_ID, DocBaseType, trxName);
                    if (pc.Save())
                    {
                        counter++;
                        _log.Fine(pc.ToString());
                    }
                    else
                        _log.Warning("Not saved: " + pc);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }

            if (sp != null)
                sp.AddLog(0, null, (Decimal)counter, "@C_PeriodControl_ID@ @Created@");
            _log.Info("Inserted #" + counter);
        }	//	createPeriodControls

    }
}

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
            CreateDocumentTypes(GetCtx(), GetVAF_Client_ID(), this, Get_TrxName());
            CreatePeriodControls(GetCtx(), GetVAF_Client_ID(), this, Get_TrxName());
            return "OK";            
        }

        public static void CreateDocumentTypes(Ctx ctx, int VAF_Client_ID, SvrProcess sp, Trx trxName)
        {
            _log.Info("VAF_Client_ID=" + VAF_Client_ID);
            String sql = "SELECT rl.Value, rl.Name "
                + "FROM VAF_CtrlRef_List rl "
                + "WHERE rl.VAF_Control_Ref_ID=183"
                + " AND rl.IsActive='Y' AND NOT EXISTS "
                + " (SELECT * FROM VAB_DocTypes dt WHERE dt.VAF_Client_ID='" + VAF_Client_ID + "' AND rl.Value=dt.DocBaseType)";
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
        /// <param name="VAF_Client_ID"></param>
        /// <param name="sp"></param>
        /// <param name="trxName"></param>
        public static void CreatePeriodControls(Ctx ctx, int VAF_Client_ID, SvrProcess sp, Trx trxName)
        {
            _log.Info("VAF_Client_ID=" + VAF_Client_ID);

            //	Delete Duplicates
            //jz remove correlation ID  String sql = "DELETE FROM VAB_YearPeriodControl pc1 "
            String sql = "DELETE FROM VAB_YearPeriodControl "
                + "WHERE (VAB_YearPeriod_ID, DocBaseType) IN "
                    + "(SELECT VAB_YearPeriod_ID, DocBaseType "
                    + "FROM VAB_YearPeriodControl pc2 "
                    + "GROUP BY VAB_YearPeriod_ID, DocBaseType "
                    + "HAVING COUNT(*) > 1)"
                + " AND VAB_YearPeriodControl_ID NOT IN "
                    + "(SELECT MIN(VAB_YearPeriodControl_ID) "
                    + "FROM VAB_YearPeriodControl pc3 "
                    + "GROUP BY VAB_YearPeriod_ID, DocBaseType)";
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            _log.Info("Duplicates deleted #" + no);

            //	Insert Missing
            sql = "SELECT DISTINCT p.VAF_Client_ID, p.VAF_Org_ID, p.VAB_YearPeriod_ID, dt.DocBaseType "
                + "FROM VAB_YearPeriod p"
                + " FULL JOIN VAB_DocTypes dt ON (p.VAF_Client_ID=dt.VAF_Client_ID) "
                + "WHERE p.VAF_Client_ID='" + VAF_Client_ID + "'"
                + " AND NOT EXISTS"
                + " (SELECT * FROM VAB_YearPeriodControl pc "
                    + "WHERE pc.VAB_YearPeriod_ID=p.VAB_YearPeriod_ID AND pc.DocBaseType=dt.DocBaseType)";
            IDataReader idr = null;
            int counter = 0;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                while (idr.Read())
                {
                    int Client_ID = Utility.Util.GetValueOfInt(idr[0].ToString());
                    int Org_ID = Utility.Util.GetValueOfInt(idr[1].ToString());
                    int VAB_YearPeriod_ID = Utility.Util.GetValueOfInt(idr[2].ToString());
                    String DocBaseType = idr[3].ToString();
                   _log.Config("VAF_Client_ID=" + Client_ID
                        + ", VAB_YearPeriod_ID=" + VAB_YearPeriod_ID + ", DocBaseType=" + DocBaseType);
                    //
                    MPeriodControl pc = new MPeriodControl(ctx, Client_ID, VAB_YearPeriod_ID, DocBaseType, trxName);
                    pc.SetVAF_Org_ID(Org_ID);                // Set Organization of Period, on Period Control.
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
                sp.AddLog(0, null, (Decimal)counter, "@VAB_YearPeriodControl_ID@ @Created@");
            _log.Info("Inserted #" + counter);
        }	//	createPeriodControls

    }
}

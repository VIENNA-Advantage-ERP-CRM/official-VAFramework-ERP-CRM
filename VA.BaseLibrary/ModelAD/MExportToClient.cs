using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MExportToClient : X_AD_ExportData
    {
        public MExportToClient(Ctx ctx, int AD_ExportData_ID, Trx trxName)
            : base(ctx, AD_ExportData_ID, trxName)
        {
            if (AD_ExportData_ID == 0)
            {
            }
        }

        public static int Get(int Record_ID_1, int Record_ID_2, int Table_ID)
        {
            String sql = "Select AD_ExportData_ID from AD_ExportData where Record_ID = " + Record_ID_1 + " AND AD_COLONE_ID = " + Record_ID_2 + " AND AD_Table_ID = " + Table_ID;

            int id = 0;
            int imex = 0;
            IDataReader dr = DataBase.DB.ExecuteReader(sql);
            bool blReturn = false;
            while (dr.Read())
            {
                id = Convert.ToInt32(dr[0].ToString());
            }
            dr.Close();
            if (id > 0)
            {
                sql = "delete from AD_ExportData where Record_ID = " + Record_ID_1 + " AND AD_COLONE_ID = " + Record_ID_2 + " AND AD_Table_ID = " + Table_ID;
                int res = DataBase.DB.ExecuteQuery(sql);
                if (res == 1)
                    imex = 1;
            }
            else
            {
                MExportToClient mex = new MExportToClient(Env.GetCtx(), id, null);
                mex.SetRecord_ID(Record_ID_1);
                mex.SetAD_ColOne_ID(Record_ID_2);
                mex.SetAD_Table_ID(Table_ID);
                blReturn = mex.Save();

                if (blReturn)
                    imex = 0;
            }

            return imex;            
        }

        public static int Get(int Record_ID, int Table_ID)
        {
            String sql = "Select AD_ExportData_ID from AD_ExportData where Record_ID = " + Record_ID + " AND AD_Table_ID = " + Table_ID;

            int id = 0;
            int imex = 0;
            IDataReader dr = DataBase.DB.ExecuteReader(sql);
            bool blReturn = false;
            while (dr.Read())
            {
                id = Convert.ToInt32(dr[0].ToString());
            }
            dr.Close();
            if (id > 0)
            {
                sql = "delete from AD_ExportData where Record_ID = " + Record_ID + " AND AD_Table_ID = " + Table_ID;
                int res = DataBase.DB.ExecuteQuery(sql);
                if (res == 1)
                    imex = 1;
            }
            else
            {
                MExportToClient mex = new MExportToClient(Env.GetCtx(), id, null);
                mex.SetRecord_ID(Record_ID);
                mex.SetAD_Table_ID(Table_ID);
                blReturn = mex.Save();

                if (blReturn)
                    imex = 0;
            }

            return imex;
        }
    }


}

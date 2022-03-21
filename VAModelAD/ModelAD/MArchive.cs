using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;
using System.Data.SqlClient;
using VAdvantage.Logging;
using System.IO;
using VAdvantage.Print;

namespace VAdvantage.Model
{
    public class MArchive : X_AD_Archive
    {
        public static MArchive[] Get(Ctx ctx, String whereClause)
        {
            List<MArchive> list = new List<MArchive>();
            String sql = "SELECT * FROM AD_Archive WHERE AD_Client_ID=@param1";
            if (whereClause != null && whereClause.Length > 0)
                sql += whereClause;
            sql += " ORDER BY Created";

            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@param1", ctx.GetAD_Client_ID());
                DataSet ds = DB.ExecuteDataset(sql, param);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MArchive(ctx, dr, null));
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }
            if (list.Count() == 0)
                s_log.Fine(sql);
            else
                s_log.Finer(sql);
            //
            MArchive[] retValue = new MArchive[list.Count()];
            retValue = list.ToArray();
            return retValue;
        }	//	get

        /**	Logger			*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MArchive).FullName);

        private Int64 m_inflated = 0;
        private Int64 m_deflated = 0;

        public MArchive(Ctx ctx, DataRow rs, Trx trx)
            : base(ctx, rs, trx)
        {

        }	//	MArchive

        public MArchive(Ctx ctx, int AD_Archive_ID, Trx trx)
            : base(ctx, AD_Archive_ID, trx)
        {

        }	//	MArchive

        public MArchive(Ctx ctx, PrintInfo info, Trx trx)
            : this(ctx, 0, trx)
        {
            SetName(info.GetName());
            SetIsReport(info.IsReport());
            //
            SetAD_Process_ID(info.GetAD_Process_ID());
            SetAD_Table_ID(info.GetAD_Table_ID());
            SetRecord_ID(info.GetRecord_ID());
            SetC_BPartner_ID(info.GetC_BPartner_ID());
        }	//	MArchive

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MArchive[");
            sb.Append(Get_ID()).Append(",Name=").Append(GetName());
            //if (m_inflated != null)
                sb.Append(",Inflated=" + m_inflated);
           // if (m_deflated != null)
                sb.Append(",Deflated=" + m_deflated);
            sb.Append("]");
            return sb.ToString();
        }


        public String GetCreatedByName()
        {
            String name = "?";
            String sql = "SELECT Name FROM AD_User WHERE AD_User_ID=@param";
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@param1", GetCreatedBy());
                dr = DB.ExecuteReader(sql);
                while (dr.Read())
                {
                    name = dr[0].ToString();
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }

            return name;
        }	//	getCreatedByName

        protected override bool BeforeSave(bool newRecord)
        {
            //	Binary Data is Mandatory
            byte[] data = base.GetBinaryData();
            if (data == null || data.Length == 0)
                return false;
            //
            log.Fine(ToString());
            return true;
        }

        public Stream GetInputStream()
        {
            byte[] inflatedData = GetBinaryData();
            if (inflatedData == null)
                return null;

            MemoryStream memStream = new MemoryStream();
            memStream.Write(inflatedData, 0, inflatedData.Length);

            return memStream;
        }	//	getInputStream
    }
}

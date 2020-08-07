/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MMedia
 * Purpose        : Web Media Model
 * Class Used     : X_CM_Media
 * Chronological    Development
 * Deepak           12-Feb-2010
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
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MMedia : X_CM_Media
    {
        /// <summary>
        /// Get Media AD_ MPrintForm
        /// </summary>
        /// <param name="project">project</param>
        /// <returns>list of server</returns>
        public static MMedia[] GetMedia(MWebProject project)
        {
            List<MMedia> list = new List<MMedia>();
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            String sql = "SELECT * FROM CM_Media WHERE CM_WebProject_ID=@param ORDER BY CM_Media_ID";
            try
            {
                //pstmt = DataBase.prepareStatement (sql, project.get_TrxName());
                //pstmt.setInt (1, project.getCM_WebProject_ID());
                param[0] = new SqlParameter("@param", project.GetCM_WebProject_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, project.Get_TrxName());
                while (idr.Read())
                {
                    list.Add(new MMedia(project.GetCtx(), idr, project.Get_TrxName()));
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
            MMedia[] retValue = new MMedia[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	//	getMedia

        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MMedia).FullName);//.class);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="CM_Media_ID">id</param>
        /// <param name="trxName">trx</param>
        public MMedia(Ctx ctx, int CM_Media_ID, Trx trxName)
            : base(ctx, CM_Media_ID, trxName)
        {

        }	//	MMedia

        /// <summary>
        /// 	Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">trx</param>
        public MMedia(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }	//	MMedia
        public MMedia(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        { }
        /** Web Project			*/
        private MWebProject _project = null;

        /// <summary>
        /// /**
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="CM_Media_Server_ID">server id</param>
        /// <param name="trxName">id</param>
        /// <returns>server list of media items to deploy</returns>
        public static MMedia[] GetMediaToDeploy(Ctx ctx, int CM_Media_Server_ID, Trx trxName)
        {
            List<MMedia> list = new List<MMedia>();
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            String sql = "SELECT CMM.* FROM CM_Media CMM, CM_MediaDeploy CMMD WHERE " +
                    "CMM.CM_Media_ID = CMMD.CM_Media_ID AND " +
                    "CMMD.CM_Media_Server_ID = @param AND " +
                    "CMMD.IsDeployed='N' " +
                    "ORDER BY CMM.CM_Media_ID";
            try
            {
                //pstmt = DataBase.prepareStatement (sql, trxName);
                //pstmt.setInt (1, CM_Media_Server_ID);
                param[0] = new SqlParameter("@param", CM_Media_Server_ID);
                idr = DataBase.DB.ExecuteReader(sql, param, trxName);
                while (idr.Read())
                {
                    list.Add(new MMedia(ctx, idr, trxName));
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

            MMedia[] retValue = new MMedia[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	//	getMedia



        /// <summary>
        /// /**
        /// </summary>
        /// get container by name
        /// <param name="ctx">context</param>
        /// <param name="Name">name</param>
        /// <param name="CM_WebProject_Id">id</param>
        /// <param name="trxName">trx</param>
        /// <returns>container or null if nt found</returns>
        public static MMedia GetByName(Ctx ctx, String Name, int CM_WebProject_Id, Trx trxName)
        {
            MMedia thisMedia = null;
            String sql = "SELECT * FROM CM_Media WHERE (LOWER(Name) LIKE @param1) AND CM_WebProject_ID=@param2";
            SqlParameter[] param = new SqlParameter[2];
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql, trxName);
                //pstmt.setString (1,Name.toLowerCase ());
                //pstmt.setInt(2, CM_WebProject_Id);
                param[0] = new SqlParameter("@param1", Name.ToLower());
                param[1] = new SqlParameter("@param2", CM_WebProject_Id);
                idr = DataBase.DB.ExecuteReader(sql, param, trxName);
                if (idr.Read())
                {
                    thisMedia = (new MMedia(ctx, idr, trxName));
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

            return thisMedia;
        }

        /// <summary>
        /// Get Web Project
        /// </summary>
        /// <returns> Web Project</returns>
        public MWebProject GetWebProject()
        {
            if (_project == null)
            {
                _project = MWebProject.Get(GetCtx(), GetCM_WebProject_ID());
            }
            return _project;
        }	//	getWebProject

        /// <summary>
        /// Get AD_Tree_ID
        /// </summary>
        /// <returns>tree</returns>
        public int GetAD_Tree_ID()
        {
            return GetWebProject().GetAD_TreeCMM_ID();
        }	//	getAD_Tree_ID;


        /// <summary>
        /// 	Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (IsSummary())
            {
                SetMediaType(null);
                SetAD_Image_ID(0);
            }
            return true;
        }	//	beforeSave

        /// <summary>
        /// after save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>true if saved</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;
            if (newRecord)
            {
                StringBuilder sb = new StringBuilder("INSERT INTO AD_TreeNodeCMM "
                    + "(AD_Client_ID,AD_Org_ID, IsActive,Created,CreatedBy,Updated,UpdatedBy, "
                    + "AD_Tree_ID, Node_ID, Parent_ID, SeqNo) "
                    + "VALUES (")
                    .Append(GetAD_Client_ID()).Append(",0, 'Y', SysDate, 0, SysDate, 0,")
                    .Append(GetAD_Tree_ID()).Append(",").Append(Get_ID())
                    .Append(", 0, 999)");
                int no = DataBase.DB.ExecuteQuery(sb.ToString(), null, Get_TrxName());
                if (no > 0)
                {
                    log.Fine("#" + no + " - TreeType=CMM");
                }
                else
                {
                    log.Warning("#" + no + " - TreeType=CMM");
                }
                return no > 0;
            }
            // Construct / Update Deployment Procedure
            MMediaServer[] theseServers = MMediaServer.GetMediaServer(_project);
            if (theseServers != null && theseServers.Length > 0)
                for (int i = 0; i < theseServers.Length; i++)
                {
                    MMediaDeploy thisDeploy = MMediaDeploy.GetByMedia(GetCtx(), Get_ID(), theseServers[i].Get_ID(), true, Get_TrxName());
                    if (thisDeploy.IsDeployed())
                    {
                        thisDeploy.SetIsDeployed(false);
                        thisDeploy.Save();
                    }
                }
            return success;
        }	//	afterSave

        /// <summary>
        /// Reperstation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MMedia[ID=")
            .Append(Get_ID())
            .Append(",FileName=").Append(Get_ID() + GetExtension())
            .Append("]");
            return sb.ToString();
        }

        protected override bool BeforeDelete()
        {
            // Delete from Deployment Server
            MMediaDeploy[] theseDeployers = MMediaDeploy.GetByMediaAndProject(GetCtx(), Get_IDOld(), _project, false, Get_TrxName());
            if (theseDeployers != null && theseDeployers.Length > 0)
                for (int i = 0; i < theseDeployers.Length; i++)
                    if (!theseDeployers[i].GetServer().DeleteMediaItem(this))
                        log.Warning("Could not delete file + " + this.ToString() + " from Server: " + theseDeployers[i].GetServer());
            // Delete From MMediaDeploy
            StringBuilder sb = new StringBuilder("DELETE FROM CM_MediaDeploy ")
                .Append(" WHERE CM_Media_ID=").Append(Get_IDOld());
            int no = DataBase.DB.ExecuteQuery(sb.ToString(), null, Get_TrxName());
            if (no > 0)
                log.Fine("#" + no + " - CM_MediaDeploy");
            else
                log.Warning("#" + no + " - CM_MediaDeploy");

            return true;
        }

        /// <summary>
        /// After Delete
        /// </summary>
        /// <param name="success">success</param>
        /// <returns>delete</returns>
        protected override bool AfterDelete(bool success)
        {
            if (!success)
                return success;
            // Delete from tree
            StringBuilder sb = new StringBuilder("DELETE FROM AD_TreeNodeCMM ")
                .Append(" WHERE Node_ID=").Append(Get_IDOld())
                .Append(" AND AD_Tree_ID=").Append(GetAD_Tree_ID());
            int no = DataBase.DB.ExecuteQuery(sb.ToString(), null, Get_TrxName());
            if (no > 0)
                log.Fine("#" + no + " - TreeType=CMM");
            else
                log.Warning("#" + no + " - TreeType=CMM");
            return no > 0;
        }	//	afterDelete

        /// <summary>
        /// get file name
        /// </summary>
        /// <returns>file name return ID</returns>
        public String GetFileName()
        {
            return Get_ID() + GetExtension();
        }	//	getFileName

        /// <summary>
        /// Get Extension with .
        /// </summary>
        /// <returns>extension</returns>
        public String GetExtension()
        {
            String mt = GetMediaType();
            if (MEDIATYPE_ApplicationPdf.Equals(mt))
                return ".pdf";
            if (MEDIATYPE_ImageGif.Equals(mt))
                return ".gif";
            if (MEDIATYPE_ImageJpeg.Equals(mt))
                return ".jpg";
            if (MEDIATYPE_ImagePng.Equals(mt))
                return ".png";
            if (MEDIATYPE_TextCss.Equals(mt))
                return ".css";
            if (MEDIATYPE_TextJs.Equals(mt))
                return ".js";
            //	Unknown
            return ".dat";
        }	//	getExtension

        /// <summary>
        /// Get Image
        /// </summary>
        /// <returns>image or null</returns>
        public MImage GetImage()
        {
            if (GetAD_Image_ID() != 0)
                return MImage.Get(GetCtx(), GetAD_Image_ID());
            return null;
        }	//	getImage

        /// <summary>
        ///	Get Data as byte array
        /// </summary>
        /// <returns>data or null</returns>
        public byte[] GetData()
        {
            MImage image = GetImage();
            if (image != null)
            {
                byte[] data = image.GetData();
                if (data == null || data.Length == 0)
                    log.Config("No Image Data");
            }

            //	Attachment
            MAttachment att = GetAttachment();
            if (att == null || att.GetEntryCount() == 0)
            {
                log.Config("No Attachment");
                return null;
            }
            if (att.GetEntryCount() > 1)
                log.Warning(GetName() + " - more then one attachment - " + att.GetEntryCount());
            //
            MAttachmentEntry entry = att.GetEntry(0);
            if (entry == null)
            {
                log.Config("No Attachment Entry");
                return null;
            }
            byte[] buffer = entry.GetData();
            if (buffer == null || buffer.Length == 0)
            {
                log.Config("No Attachment Entry Data");
                return null;
            }
            return buffer;
        }	//	getData

        /// <summary>
        /// Get Input Stream
        /// </summary>
        /// <returns>input stream or null</returns>
        public System.IO.Stream GetInputStream()
        {
            byte[] buffer = GetData();
            //ByteArrayInputStream is = new ByteArrayInputStream(buffer);
            //System.IO.BufferedStream iss=new System.IO.BufferedStream(buffer);
            System.IO.Stream iss = new System.IO.MemoryStream(buffer);
            return iss;
        }	//	getInputStream

        /// <summary>
        /// Get Updated timestamp of Attachment
        /// </summary>
        /// <returns>update or null if no attachment</returns>
        public DateTime? GetAttachmentUpdated()
        {
            MAttachment att = GetAttachment();
            if (att == null)
                return null;
            return att.GetUpdated();
        }	//	getAttachmentUpdated

    }	//	MMedia

}

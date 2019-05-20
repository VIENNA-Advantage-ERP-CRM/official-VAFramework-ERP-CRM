/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MMediaServer
 * Purpose        : Media Server Model
 * Class Used     : X_CM_Media_Server
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
    public class MMediaServer : X_CM_Media_Server
    {
        /// <summary>
        /// Get Media Server
        /// </summary>
        /// <param name="project">proeject</param>
        /// <returns>server list</returns>
        public static MMediaServer[] GetMediaServer(MWebProject project)
        {
            List<MMediaServer> list = new List<MMediaServer>();
            IDataReader idr = null;
            SqlParameter[] param = new SqlParameter[1];
            String sql = "SELECT * FROM CM_Media_Server WHERE CM_WebProject_ID=@param ORDER BY CM_Media_Server_ID";
            try
            {
                //pstmt = DataBase.prepareStatement (sql, project.get_TrxName());
                //pstmt.setInt (1, project.getCM_WebProject_ID());
                param[0] = new SqlParameter("@param", project.GetCM_WebProject_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, project.Get_TrxName());
                //DataTable dt = new DataTable();
                //dt.Load(idr);
                while (idr.Read())
                {
                    list.Add(new MMediaServer(project.GetCtx(), idr, project.Get_TrxName()));
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
            MMediaServer[] retValue = new MMediaServer[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	//	getMediaServer

        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MMediaServer).FullName);//.class);


        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="CM_Media_Server_ID">id</param>
        /// <param name="trxName">trx</param>
        public MMediaServer(Ctx ctx, int CM_Media_Server_ID, Trx trxName)
            : base(ctx, CM_Media_Server_ID, trxName)
        {

        }	//	MMediaServer

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">trx</param>
        public MMediaServer(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }	//	MMediaServer
        public MMediaServer(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        { }

        /**  MWebProject WebProject (parent) */
        public MWebProject _project = null;

        /// <summary>
        /// getWebProject
        /// </summary>
        /// <returns>web project</returns>
        public MWebProject GetWebProject()
        {
            if (_project == null)
            {
                _project = new MWebProject(GetCtx(), GetCM_WebProject_ID(), Get_TrxName());
            }
            return _project;
        }

        /// <summary>
        /// reDeployAll set all media items to redeploy
        /// </summary>
        public void ReDeployAll()
        {
            MMedia[] media = MMedia.GetMedia(GetWebProject());
            if (media != null && media.Length > 0)
            {
                for (int i = 0; i < media.Length; i++)
                {
                    MMediaDeploy thisDeploy = MMediaDeploy.GetByMedia(GetCtx(), media[i].Get_ID(), Get_ID(), true, null);
                    if (thisDeploy.IsDeployed())
                    {
                        log.Log(Level.FINE, "Reset Deployed Flag on MediaItem" + media[i].Get_ID());
                        thisDeploy.SetIsDeployed(false);
                        thisDeploy.Save();
                    }
                }
            }
        }

        /// <summary>
        /// (Re-)Deploy all media
        /// </summary>
        /// <returns>true if deployed</returns>
        public bool Deploy()
        {
            MMedia[] media = MMedia.GetMediaToDeploy(GetCtx(), this.Get_ID(), Get_TrxName());

            // Check whether the host is our example localhost, we will not deploy locally, but this is no error
            if (this.GetIP_Address().Equals("127.0.0.1") || this.GetName().Equals("localhost"))
            {
                log.Warning("You have not defined your own server, we will not really deploy to localhost!");
                return true;
            }

            //FTPClient ftp = new FTPClient();          

            System.Net.FtpWebRequest ftp;
            try
            {
                //ftp.connect (getIP_Address());
                ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(GetIP_Address());
                ftp.Credentials = new System.Net.NetworkCredential(GetUserName(), GetPassword());
                if (ftp.Credentials != null)
                {
                    //if (ftp.login (getUserName(), getPassword()))
                    log.Info("Connected to " + GetIP_Address() + " as " + GetUserName());
                }
                else
                {
                    log.Warning("Could NOT connect to " + GetIP_Address() + " as " + GetUserName());
                    return false;
                }
            }
            catch (Exception e)
            {
                log.Log(Level.WARNING, "Could NOT connect to " + GetIP_Address()
                    + " as " + GetUserName(), e);
                return false;
            }

            bool success = true;
            String cmd = null;
            //	List the files in the directory
            try
            {
                cmd = "cwd";
                //ftp.changeWorkingDirectory (getFolder());
                ftp.Method = GetFolder();
                //
                cmd = "list";
                ftp.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;
                System.IO.StreamReader sr = new System.IO.StreamReader(ftp.GetResponse().GetResponseStream());
                List<String> listnames = new List<string>();
                String str = sr.ReadLine();
                while (str != null)
                {
                    listnames.Add(str);
                    str = sr.ReadLine();
                }
                String[] fileNames = listnames.ToArray();
                log.Log(Level.FINE, "Number of files in " + GetFolder() + ": " + fileNames.Length);

                /*
                FTPFile[] files = ftp.listFiles();
                log.config("Number of files in " + getFolder() + ": " + files.length);
                for (int i = 0; i < files.length; i++)
                    log.fine(files[i].getTimestamp() + " \t" + files[i].getName());*/
                //
                cmd = "bin";
                //ftp.setFileType(FTP.BINARY_FILE_TYPE);
                ftp.UseBinary = true;

                for (int i = 0; i < media.Length; i++)
                {
                    MMediaDeploy thisDeployment = MMediaDeploy.GetByMedia(GetCtx(), media[i].Get_ID(), this.Get_ID(), false, Get_TrxName());
                    if (!media[i].IsSummary() && media[i].GetMediaType() != null)
                    {
                        log.Log(Level.INFO, " Deploying Media Item: " + media[i].ToString());
                        MImage thisImage = media[i].GetImage();

                        // Open the file and output streams
                        byte[] buffer = thisImage.GetData();
                        //ByteArrayInputStream is = new ByteArrayInputStream(buffer);
                        //System.IO.BufferedStream iss=new System.IO.BufferedStream(buffer);
                        String fileName = media[i].Get_ID() + media[i].GetExtension();
                        cmd = "put " + fileName;
                        //ftp.storeFile(fileName, is);
                        System.IO.FileStream streamObj = System.IO.File.OpenRead(fileName);
                        streamObj.Read(buffer, 0, buffer.Length);
                        streamObj.Close();
                        streamObj = null;
                        ftp.GetRequestStream().Write(buffer, 0, buffer.Length);
                        thisDeployment.SetIsDeployed(true);
                        thisDeployment.Save();
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.WARNING, cmd, e);
                success = false;
            }
            //	Logout from the FTP Server and disconnect
            try
            {
                cmd = "logout";
                //ftp.logout();
                ftp.UsePassive = false;
                log.Log(Level.FINE, " FTP logged out");
                cmd = "disconnect";
                //ftp.disconnect();
                ftp.KeepAlive = false;
            }
            catch (Exception e)
            {
                log.Log(Level.WARNING, cmd, e);
            }
            ftp = null;
            return success;
        }	//	deploy

        /// <summary>
        /// (Re-)Deploy all media
        /// </summary>
        /// <param name="t_media">true if deployed</param>
        /// <returns></returns>
        public bool DeleteMediaItem(MMedia t_media)
        {
            // Check whether the host is our example localhost, we will not deploy locally, but this is no error
            if (this.GetIP_Address().Equals("127.0.0.1") || this.GetName().Equals("localhost"))
            {
                log.Warning("You have not defined your own server, we will not really deploy to localhost!");
                return true;
            }

            //FTPClient ftp = new FTPClient();
            System.Net.FtpWebRequest ftp;
            try
            {
                //ftp.connect (getIP_Address());
                ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(GetIP_Address());
                ftp.Credentials = new System.Net.NetworkCredential(GetUserName(), GetPassword());
                //if (ftp.login (getUserName(), getPassword()))
                if (ftp.Credentials != null)
                {
                    log.Info("Connected to " + GetIP_Address() + " as " + GetUserName());
                }
                else
                {
                    log.Warning("Could NOT connect to " + GetIP_Address() + " as " + GetUserName());
                    return false;
                }
            }
            catch (Exception e)
            {
                log.Log(Level.WARNING, "Could NOT connect to " + GetIP_Address()
                    + " as " + GetUserName(), e);
                return false;
            }

            bool success = true;
            String cmd = null;
            //	List the files in the directory
            try
            {
                cmd = "cwd";
                //ftp.changeWorkingDirectory (getFolder());
                ftp.Method = GetFolder();
                //
                if (!t_media.IsSummary())
                {
                    log.Log(Level.INFO, " Deleting Media Item:" + t_media.Get_ID() + t_media.GetExtension());
                    //ftp.dele (t_media.Get_ID() + t_media.GetExtension());
                    ftp = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(t_media.Get_ID() + t_media.GetExtension());
                    // ftp.Method = t_media.Get_ID() + t_media.GetExtension();
                    ftp.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;

                }
            }
            catch (Exception e)
            {
                log.Log(Level.WARNING, cmd, e);
                success = false;
            }
            //	Logout from the FTP Server and disconnect
            try
            {
                cmd = "logout";
                //ftp.logout();
                ftp.UsePassive = false;
                cmd = "disconnect";
                //ftp.disconnect();
                ftp.KeepAlive = false;
            }
            catch (Exception e)
            {
                log.Log(Level.WARNING, cmd, e);
            }
            ftp = null;
            return success;
        }	//	deploy

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MMediaServer[")
            .Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }
    }
}

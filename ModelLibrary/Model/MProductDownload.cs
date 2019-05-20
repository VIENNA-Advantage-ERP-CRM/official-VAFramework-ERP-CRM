/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MProductDownload
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     11-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using VAdvantage.Logging;
using System.Security.Policy;

namespace VAdvantage.Model
{
    public class MProductDownload : X_M_ProductDownload
    {
        /**	Cache						*/
        private static CCache<int, MProductDownload> s_cache
            = new CCache<int, MProductDownload>("M_ProductDownload", 20);

        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MProductDownload).FullName);

        /**
         * 	Migrate Download URLs (2.5.2c)
         *	@param ctx context
         */
        public static void MigrateDownloads(Ctx ctx)
        {
            String sql = "SELECT COUNT(*) FROM M_ProductDownload";
            int no = DataBase.DB.GetSQLValue(null, sql);
            if (no > 0)
                return;
            //
            int count = 0;
            sql = "SELECT AD_Client_ID, AD_Org_ID, M_Product_ID, Name, DownloadURL "
                + "FROM M_Product "
                + "WHERE DownloadURL IS NOT NULL";
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    int AD_Client_ID = Utility.Util.GetValueOfInt(idr[0].ToString());
                    int AD_Org_ID = Utility.Util.GetValueOfInt(idr[1].ToString());
                    int M_Product_ID = Utility.Util.GetValueOfInt(idr[2].ToString());
                    String Name = idr[3].ToString();
                    String DownloadURL = idr[4].ToString();
                    //
                    MProductDownload pdl = new MProductDownload(ctx, 0, null);
                    pdl.SetClientOrg(AD_Client_ID, AD_Org_ID);
                    pdl.SetM_Product_ID(M_Product_ID);
                    pdl.SetName(Name);
                    pdl.SetDownloadURL(DownloadURL);
                    if (pdl.Save())
                    {
                        count++;
                        String sqlUpdate = "UPDATE M_Product SET DownloadURL = NULL WHERE M_Product_ID=" + M_Product_ID;
                        int updated = DataBase.DB.ExecuteQuery(sqlUpdate, null,null);
                        if (updated != 1)
                        {
                            _log.Warning("Product not updated");
                        }
                    }
                    else
                    {
                        _log.Warning("Product Download not created M_Product_ID=" + M_Product_ID);
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
           _log.Info("#" + count);
        }

        /**
         * 	Get Product Download from Cache
         *	@param ctx context
         *	@param M_ProductDownload_ID id
         *	@return MProductDownload
         */
        public static MProductDownload Get(Ctx ctx, int M_ProductDownload_ID)
        {
            int key = M_ProductDownload_ID;
            MProductDownload retValue = (MProductDownload)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MProductDownload(ctx, M_ProductDownload_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }


        /**************************************************************************
         * 	Standard Constructor
         *	@param ctx context
         *	@param M_ProductDownload_ID id
         *	@param trxName trx
         */
        public MProductDownload(Ctx ctx, int M_ProductDownload_ID,
            Trx trxName)
            : base(ctx, M_ProductDownload_ID, trxName)
        {
            if (M_ProductDownload_ID == 0)
            {
                //	setM_Product_ID (0);
                //	setName (null);
                //	setDownloadURL (null);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName trx
         */
        public MProductDownload(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MProductDownload[")
                .Append(Get_ID())
                .Append(",M_Product_ID=").Append(GetM_Product_ID())
                .Append(",").Append(GetDownloadURL())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Get Download Name
         *	@return download name (last part of name)
         */
        public String GetDownloadName()
        {
            String url = GetDownloadURL();
            if (url == null || !IsActive())
                return null;
            int pos = Math.Max(url.LastIndexOf('/'), url.LastIndexOf('\\'));
            if (pos != -1)
                return url.Substring(pos + 1);
            return url;
        }

        /**
         * 	Get Download URL
         * 	@param directory optional directory
         *	@return url
         */
        //public URL GetDownloadURL (String directory)
        //{
        //    String dl_url = GetDownloadURL();
        //    if (dl_url == null || !IsActive())
        //        return null;

        //    URL url = null;
        //    try
        //    {
        //        if (dl_url.IndexOf ("://") != -1)
        //            url = new URL (dl_url);
        //        else
        //        {
        //            File f = GetDownloadFile (directory);
        //            if (f != null)
        //                url = f.toURL ();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Log(Level.SEVERE, dl_url, ex);
        //        return null;
        //    }
        //    return url;
        //}

        /**
         * 	Find download url
         *	@param directory optional directory
         *	@return file or null
         */
        public FileInfo GetDownloadFile(String directory)
        {
            FileInfo file = new FileInfo(GetDownloadURL());	//	absolute file
            if (File.Exists(file.FullName))// || file.Exists)
            {
                return file;
            }
            //File file = new File(GetDownloadURL());	//	absolute file
            //if (file.exists())
            //    return file;
            if (directory == null || directory.Length == 0)
            {
                log.Log(Level.SEVERE, "Not found " + GetDownloadURL());
                return null;
            }
            String downloadURL2 = directory;
            if (!downloadURL2.EndsWith(Path.DirectorySeparatorChar.ToString()))
                downloadURL2 += Path.DirectorySeparatorChar.ToString();
            downloadURL2 += GetDownloadURL();
            file = new FileInfo(downloadURL2);
            if (File.Exists(file.FullName))// || file.Exists)
                return file;

            log.Log(Level.SEVERE, "Not found " + GetDownloadURL() + " + " + downloadURL2);
            return null;
        }

        /**
         * 	Get Download Stream
         * 	@param directory optional directory
         *	@return input stream
         */
        //public InputStream GetDownloadStream(String directory)
        //{
        //    String dl_url = GetDownloadURL();
        //    if (dl_url == null || !IsActive())
        //        return null;

        //    InputStream in1 = null;
        //    try
        //    {
        //        if (dl_url.IndexOf("://") != -1)
        //        {
        //            URL url = new URL(dl_url);
        //            in1 = url.openStream();
        //        }
        //        else //	file
        //        {
        //            File file = GetDownloadFile(directory);
        //            if (file == null)
        //                return null;
        //            in1 = new FileInputStream(file);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Log(Level.SEVERE, dl_url, ex);
        //        return null;
        //    }
        //    return in1;
        //}
        public Url GetDownloadURL(String directory)
        {
            String dl_url = GetDownloadURL();
            if (dl_url == null || !IsActive())
                return null;

            Url url = null;
            try
            {
                if (dl_url.IndexOf("://") != -1)
                    url = new Url(dl_url);// URL(dl_url);
                else
                {
                    FileInfo f = GetDownloadFile(directory);
                    if (f != null)
                        url =new Url(f.Name);// f.toURL();
                }
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, dl_url, ex);
                return null;
            }
            return url;
        }
    }
}
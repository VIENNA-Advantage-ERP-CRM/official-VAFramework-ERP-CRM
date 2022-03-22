using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Drawing;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using System.IO;
using System.Web.Hosting;
using System.Web;
//using System.Web.Helpers;



namespace VAdvantage.Model
{
    public class MImage : X_AD_Image
    {
        /// <summary>
        ///Get MImage from Cache
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Image_ID"></param>
        /// <returns></returns>
        private byte[] byteArray;
        public byte[] ByteArray
        {
            get { return byteArray; }
            set { byteArray = value; }
        }
        private string imageFormat;


        private List<string> imageExtensions = new List<string>() { ".png", ".jpg", ".ico", ".webp", ".svg", ".jpeg" };

        public string ImageFormat
        {
            get { 
                return imageFormat; }
            set { imageFormat = value;
                SetImageExtension(imageFormat);
            }
        }
        public static MImage Get(Ctx ctx, int AD_Image_ID)
        {
            if (AD_Image_ID == 0)
                return new MImage(ctx, AD_Image_ID, null);
            //
            int key = AD_Image_ID;
            MImage retValue = s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MImage(ctx, AD_Image_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache[key] = retValue;
            return retValue;
        }

        /**	Cache						*/
        private static CCache<int, MImage> s_cache = new CCache<int, MImage>("AD_Image", 20);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Image_ID"></param>
        /// <param name="trxName"></param>
        public MImage(Ctx ctx, int AD_Image_ID, Trx trxName)
            : base(ctx, AD_Image_ID, trxName)
        {
            if (AD_Image_ID < 1)
                SetName("-");
        }

        /// <summary>
        ///	Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MImage(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }	//	MImage


        /** The Image                   */
        private Image _image = null;
        /** The Icon                   */
        private Icon _icon = null;

        /// <summary>
        ///	Get Image
        /// </summary>
        /// <returns></returns>
        public Image GetImage()
        {
            if (_image != null)
                return _image;
            //	Via byte array
            byte[] data = GetBinaryData();
            if (data != null && data.Length > 0)
            {
                try
                {
                    //Toolkit tk = Toolkit.getDefaultToolkit();
                    //m_image = tk.createImage(data);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream(data);
                    _image = Image.FromStream(ms);
                    return _image;
                }
                catch (Exception e)
                {
                    log.Log(Level.WARNING, "(byteArray)", e);
                    return null;
                }
            }
            //	Via URL
            //URL url = getURL();
            string url = GetURL();




            if (url == null)
                return null;
            try
            {
                //    Toolkit tk = Toolkit.getDefaultToolkit();
                _image = null;// Utility.Env.GetImageIcon(url);


                //    m_image = tk.getImage(url);
                return _image;
            }
            catch (Exception e)
            {
                log.Log(Level.WARNING, "(URL)", e);
            }
            return null;
        }   //  getImage


        /**
	 * 	Get URL
	 *	@return url or null
	 */
        //private URL getURL()
        //{
        //    String str = getImageURL();
        //    if (str == null || str.length() == 0)
        //        return null;

        //    URL url = null;
        //    try
        //    {
        //        //	Try URL directly
        //        if (str.indexOf("://") != -1)
        //            url = new URL(str);
        //        else	//	Try Resource
        //            url = getClass().getResource(str);
        //        //
        //        if (url == null)
        //            log.warning("Not found: " + str);
        //    }
        //    catch (Exception e)
        //    {
        //        log.warning("Not found: " + str + " - " + e.getMessage());
        //    }
        //    return url;
        //}	

        //Added By Sarab
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (byteArray != null)
            {
                String imageExt = GetImageExtension();
                String imageUrl = GetImageURL();
                //if (GetImageURL() != null)
                //{
                //    if (!Directory.Exists(GetImageURL()))
                //    {
                //        return false;
                //    }
                //}
                if (!string.IsNullOrEmpty(imageExt))
                {
                    //imageUrl = imageUrl.Insert(imageUrl.Length, "/" + GetAD_Image_ID() + ImageFormat);
                    imageUrl = "Images/" + GetAD_Image_ID() + imageFormat;
                    int count = DB.ExecuteQuery("UPDATE AD_IMAGE SET IMAGEURL='" + imageUrl + "' WHERE AD_IMAGE_ID=" + GetAD_Image_ID());
                }

                ConvertByteArrayToThumbnail(byteArray, GetAD_Image_ID().ToString() + imageExt);
            }
            else if (GetBinaryData() != null)
            {
                String imageExt = GetImageExtension();
                String imageUrl = GetImageURL();
                if (!string.IsNullOrEmpty(imageExt) && imageExt.Contains("."))
                {
                   // imageFormat = imageUrl.Substring(imageUrl.LastIndexOf("."));
                    imageUrl = "Images/" + GetAD_Image_ID() + imageExt;
                    int count = DB.ExecuteQuery("UPDATE AD_IMAGE SET IMAGEURL='" + imageUrl + "' WHERE AD_IMAGE_ID=" + GetAD_Image_ID());
                }
                ConvertByteArrayToThumbnail(GetBinaryData(), GetAD_Image_ID().ToString() + imageExt);
            }
            return success;
        }

        /// <summary>
        /// After delete is used to delete images from all folders
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        protected override bool AfterDelete(bool success)
        {
            //string[,] thumbnails = new string[,] { { "0", "0" }, { "16", "16" }, { "32", "32" }, { "46", "46" }, { "140", "120" }, { "320", "185" }, { "320", "240" } };
            string imageUrl = GetImageURL();
            string imageName = imageUrl.Substring(imageUrl.LastIndexOf('/') + 1);
            DeleteThumbnail("0", "0", imageName);
            DeleteThumbnail("16", "16", imageName);
            DeleteThumbnail("32", "32", imageName);
            DeleteThumbnail("46", "46", imageName);
            DeleteThumbnail("140", "120", imageName);
            DeleteThumbnail("320", "185", imageName);
            DeleteThumbnail("320", "240", imageName);
            DeleteThumbnail("500", "375", imageName);
            return success;
        }

        #region Create thumbnail of image
        /// <summary>
        /// This function is used to create thumbnail of Image
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <returns></returns>
        /// 
        public void ConvertByteArrayToThumbnail(byte[] imageBytes, string imageName)
        {

            try
            {
                //********** Save Original Image ********************
                CreateThumbnail(0, 0, imageBytes, imageName);

                //*********Create Thumbnail of 500/375 *******************
                CreateThumbnail(500, 375, imageBytes, imageName);

                //*********Create Thumbnail of 320/240 *******************
                CreateThumbnail(320, 240, imageBytes, imageName);

                //*********Create Thumbnail of 320/185 *******************
                CreateThumbnail(320, 185, imageBytes, imageName);

                //*********Create Thumbnail of 140/120 *******************
                CreateThumbnail(140, 120, imageBytes, imageName);

                //*********Create Thumbnail of 46/46 *******************
                CreateThumbnail(46, 46, imageBytes, imageName);

                //*********Create Thumbnail of 32/32 *******************
                CreateThumbnail(32, 32, imageBytes, imageName);

                //*********Create Thumbnail of 16/16 *******************
                CreateThumbnail(16, 16, imageBytes, imageName);
            }
            catch
            {
            }
            finally
            {
            }


        }
        public bool ThumbnailCallback()
        {
            return true;
        }
        #endregion




        //private void CreateThumbnail(int height, int width,byte[] byteArray,string imageName)
        //{
        //    MemoryStream ms = null;          
        //    try
        //    {
        //        System.Drawing.Image imThumbnailImage;
        //        System.Drawing.Image OriginalImage;
        //        ms = new MemoryStream();
        //        // Stream / Write Image to Memory Stream from the Byte Array.
        //        ms.Write(byteArray, 0, byteArray.Length);

        //        OriginalImage = System.Drawing.Image.FromStream(ms);
        //        if (height == 0 && width == 0)
        //        {
        //            OriginalImage.Save(Path.Combine(HostingEnvironment.MapPath(@"~/Images"), imageName));
        //            return;                    
        //        }
        //        if (!Directory.Exists((Path.Combine(HostingEnvironment.MapPath(@"~/Images"), "Thumb"+height.ToString() + "x" + width.ToString()))))
        //        {
        //            Directory.CreateDirectory(Path.Combine(HostingEnvironment.MapPath(@"~/Images"), "Thumb" + height.ToString() + "x" + width.ToString()));       //Create Thumbnail Folder if doesnot exists
        //        }
        //        // Shrink the Original Image to a thumbnail size.
        //        string filepath = Path.Combine(HostingEnvironment.MapPath(@"~/Images"), "Thumb" + height.ToString() + "x" + width.ToString() + "/" + imageName);
        //        if (!(OriginalImage.Height <= height || OriginalImage.Width <= width))
        //        {
        //            imThumbnailImage = OriginalImage.GetThumbnailImage(height, width, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
        //            imThumbnailImage.Save(filepath);
        //        }
        //        else
        //        {
        //            OriginalImage.Save(filepath);
        //        }                


        //    }
        //    catch
        //    {
        //    }
        //    finally
        //    {
        //        if (ms != null)
        //        {
        //            ms.Close();
        //        }                
        //    }
        //    //return myMS.ToArray();
        //}

        private int GetPercentage(int value, int total)
        {
            return (value * 100) / total;
        }
        private void CreateThumbnail(int width, int height, byte[] byteArray, string imageName)
        {
            int dimWidth = width;
            int dimHeight = height;
            MemoryStream ms = null;
            try
            {
                // System.Drawing.Image imThumbnailImage;
                Bitmap imThumbnailImage;
                // System.Drawing.Image OriginalImage;
                ms = new MemoryStream();
                // Stream / Write Image to Memory Stream from the Byte Array.
                ms.Write(byteArray, 0, byteArray.Length);
                Bitmap OriginalImage = (Bitmap)Image.FromStream(ms);
                //OriginalImage = System.Drawing.Image.FromStream(ms);
                if (height == 0 && width == 0)
                {
                    OriginalImage.Save(Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Images", imageName));
                    return;
                }
                if (!Directory.Exists((Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Images", "Thumb" + width.ToString() + "x" + height.ToString()))))
                {
                    Directory.CreateDirectory(Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Images", "Thumb" + width.ToString() + "x" + height.ToString()));       //Create Thumbnail Folder if doesnot exists
                }
                // Shrink the Original Image to a thumbnail size.
                int percenetage = 0;
                string filepath = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Images", "Thumb" + width.ToString() + "x" + height.ToString() + "/" + imageName);
                if (!(OriginalImage.Height < height && OriginalImage.Width < width))
                {
                    if (OriginalImage.Height > OriginalImage.Width)
                    {
                        percenetage = GetPercentage(OriginalImage.Width, OriginalImage.Height);
                        // height = width*100 / percenetage;

                        width = (height * percenetage) / 100;
                        if (width > dimWidth)
                        {
                            width = dimWidth;
                            height = (width * 100) / percenetage;
                        }
                        // height =height+( width*GetPercentage(OriginalImage.Width, OriginalImage.Height))/100;
                        // width = (width * GetPercentage(OriginalImage.Width, OriginalImage.Height)) / 100;
                    }
                    else if (OriginalImage.Height == OriginalImage.Width)
                    {
                        width = height;
                    }
                    else
                    {
                        percenetage = GetPercentage(OriginalImage.Height, OriginalImage.Width);
                        //  width = height*100 / percenetage;

                        height = (width * percenetage) / 100;
                        if (height > dimHeight)
                        {
                            height = dimHeight;
                            width = (dimHeight * 100) / percenetage;
                        }
                        //width =width+(height*GetPercentage(OriginalImage.Height, OriginalImage.Width))/100;
                        //height = (height * GetPercentage(OriginalImage.Height, OriginalImage.Width)) / 100;
                    }
                    // imThumbnailImage = OriginalImage.GetThumbnailImage(width, height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                    imThumbnailImage = new Bitmap(OriginalImage, new Size(width, height));
                    imThumbnailImage.Save(filepath);
                }
                else
                {
                    OriginalImage.Save(filepath);
                }


            }
            catch
            {
            }
            finally
            {
                if (ms != null)
                {
                    ms.Close();
                }
            }
            //return myMS.ToArray();
        }

        /// <summary>
        /// Delete all thumbnails images as well as main image
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="imageName"></param>
        private void DeleteThumbnail(string x, string y, string imageName)
        {
            try
            {
                string filepath = "";
                if (x == "0")
                    filepath = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Images", imageName);
                else
                    filepath = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Images", "Thumb" + x + "x" + y + "/" + imageName);

                FileInfo file = new FileInfo(filepath);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            catch
            { }
        }


        public string GetThumbnailURL(int height, int width)
        {
            if (GetImageURL() != null)
            {
                string imageName = GetImageURL().Substring(GetImageURL().LastIndexOf('/') + 1);
                string url = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Images", "Thumb" + height.ToString() + "x" + width.ToString() + "\\" + imageName);
                if (File.Exists(url))
                {
                    //  url = HostingEnvironment.ApplicationPhysicalPath.ToString().Substring(0, HostingEnvironment.ApplicationPhysicalPath.LastIndexOf("\\"));
                    // url = url.Substring(url.LastIndexOf("\\")) + "\\Images\\Thumb" + height.ToString() + "x" + width.ToString() + "\\" + imageName;
                    url = "Images\\Thumb" + height.ToString() + "x" + width.ToString() + "\\" + imageName;
                    return url;
                }
                else
                {
                    return "FileDoesn'tExist";
                }
            }
            return "NoRecordFound";
        }
        public byte[] GetThumbnailByte(int height, int width)
        {
            if (GetImageURL() != null)
            {
                string imageName = GetImageURL().Substring(GetImageURL().LastIndexOf('/') + 1);
                string url = string.Empty;
                if (height == 0 && width == 0)
                {
                    url = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Images", imageName);
                }
                else
                {
                    url = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Images", "Thumb" + height.ToString() + "x" + width.ToString() + "\\" + imageName);
                }
                if (File.Exists(url))
                {
                    FileStream stream = null;
                    try
                    {
                        //string filepath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, dsData.Tables[0].Rows[0]["imageurl"].ToString());
                        if (File.Exists(url))
                        {
                            stream = File.OpenRead(url);
                            byte[] fileBytes = new byte[stream.Length];
                            stream.Read(fileBytes, 0, fileBytes.Length);
                            stream.Close();
                            return fileBytes;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                    finally
                    {
                        if (stream != null)
                            stream.Close();
                    }
                }
                else
                {
                    return null;
                }
            }

            return GetBinaryData();

        }


        public dynamic GetThumbnail(int height, int width, out bool isUrl)
        {
            if (GetImageURL() != null)
            {
                string imageName = GetImageURL().Substring(GetImageURL().LastIndexOf('/') + 1);
                string url = string.Empty;
                if (height == 0 && width == 0)
                {
                    url = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Images", imageName);
                }
                else
                {
                    url = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Images", "Thumb" + height.ToString() + "x" + width.ToString() + "\\" + imageName);
                }
                if (File.Exists(url))
                {
                    isUrl = true;
                    FileStream stream = null;
                    try
                    {
                        //string filepath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, dsData.Tables[0].Rows[0]["imageurl"].ToString());
                        if (File.Exists(url))
                        {
                            return imageName;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                    finally
                    {
                        if (stream != null)
                            stream.Close();
                    }
                }
                else
                {
                    isUrl = false;
                    return null;
                }
            }
            isUrl = false;
            return GetBinaryData();


        }

        private string GetURL()
        {

            String str = GetImageURL();
            if (str == null || str.Length == 0)
                return null;

            string url = null;
            try
            {
                //	Try URL directly
                //if (str.IndexOf("://") != -1)
                //    url = new URL(str);
                //else	//	Try Resource
                //    url = getClass().getResource(str);
                ////
                //if (url == null)
                //    log.warning("Not found: " + str);
                url = str.Substring(str.LastIndexOf("/") + 1);
                return url;

            }
            catch (Exception e)
            {
                log.Warning("Not found: " + str + " - " + e.Message);
            }
            return url;
        }


        /**
         * 	Get Icon
         *	@return icon or null
         */
        public Icon GetIcon()
        {
            if (_icon != null)
                return _icon;
            //	Via byte array
            byte[] data = GetBinaryData();
            if (data != null && data.Length > 0)
            {
                try
                {

                    _icon = null;// new  Icon (ImageIcon(data, getName());
                    return _icon;
                }
                catch (Exception e)
                {
                    log.Log(Level.WARNING, "(byteArray)", e);
                    return null;
                }
            }
            ////	Via URL
            //URL url = getURL();
            //if (url == null)
            //    return null;
            //try
            //{
            //    m_icon = new ImageIcon(url, getName());
            //    return m_icon;
            //}
            //catch (Exception e)
            //{
            //    log.log(Level.WARNING, "(URL)", e);
            //}
            return null;
        }   //  getIcon

        /// <summary>
        ///Get URL
        /// </summary>
        /// <returns></returns>
        //private URL getURL()
        //{
        //    String str = getImageURL();
        //    if (str == null || str.length() == 0)
        //        return null;

        //    URL url = null;
        //    try
        //    {
        //        //	Try URL directly
        //        if (str.indexOf("://") != -1)
        //            url = new URL(str);
        //        else	//	Try Resource
        //            url = getClass().getResource(str);
        //        //
        //        if (url == null)
        //            log.warning("Not found: " + str);
        //    }
        //    catch (Exception e)
        //    {
        //        log.warning("Not found: " + str + " - " + e.getMessage());
        //    }
        //    return url;
        //}	//	getURL

        /// <summary>
        /// Set Image URL
        /// </summary>
        /// <param name="imageURL"></param>
        public new void SetImageURL(String imageURL)
        {
            _image = null;
            _icon = null;
            base.SetImageURL(imageURL);
        }	//	setImageURL

        /// <summary>
        /// Set Binary Data
        /// </summary>
        /// <param name="binaryData"></param>
        public new void SetBinaryData(byte[] binaryData)
        {
            _image = null;
            _icon = null;
            base.SetBinaryData(binaryData);
        }	//	setBinaryData

        /// <summary>
        /// Get Data 
        /// </summary>
        /// <returns></returns>
        public byte[] GetData()
        {
            byte[] data = base.GetBinaryData();
            if (data != null)
                return data;
            //	From URL
            String str = GetImageURL();
            if (str == null || str.Length == 0)
            {
                log.Config("No Image URL");
                return null;
            }
            //	Get from URL
            //URL url = getURL();
            //if (url == null)
            //{
            //    log.config("No URL");
            //    return null;
            //}
            //try
            //{
            //    URLConnection conn = url.openConnection();
            //    conn.setUseCaches(false);
            //    InputStream is = conn.getInputStream();
            //    byte[] buffer = new byte[1024*8];   //  8kB
            //    ByteArrayOutputStream os = new ByteArrayOutputStream();
            //    int length = -1;
            //    while ((length = is.read(buffer)) != -1)
            //        os.write(buffer, 0, length);
            //    is.close();
            //    data = os.toByteArray();
            //    os.close();

            //}
            //catch (Exception e)
            //{
            //    log.config (e.toString());
            //}
            return data;
        }	//	getData

        /// <summary>
        ///String Representation
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return "MImage[ID=" + Get_ID() + ",Name=" + GetName() + "]";
        }


        /// <summary>
        ///Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetAD_Org_ID() != 0)
                SetAD_Org_ID(0);
            string imageUrl = GetImageURL();
            if (String.IsNullOrEmpty(imageUrl) && String.IsNullOrEmpty(GetFontName()) && GetBinaryData() ==null)
            {
                log.SaveError("EnterFontNameOrUrl", "");
                return false;
            }

            if (!String.IsNullOrEmpty(imageUrl))
            {
                imageUrl = imageUrl.ToLower();
                if (!imageUrl.Contains(".") || imageExtensions.IndexOf(imageUrl.Substring(imageUrl.LastIndexOf("."))) < 0)
                {
                    log.SaveError("AddExtension", "");
                    return false;
                }
            }

            
            if (GetBinaryData() != null )
            {
                String imgExt = GetImageExtension();
                if (String.IsNullOrEmpty(imgExt) || !imgExt.Contains(".") || imageExtensions.IndexOf(imgExt.Substring(imgExt.LastIndexOf("."))) < 0)
                {
                    log.SaveError("AddExtension", "");
                    return false;
                }
            }


            return true;
        }
    }   //  MImage
}

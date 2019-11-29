using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Web.Helpers;
using System.Web.Hosting;
using VIS.Classes;

namespace VIS.Models
{
    public class VImageModel
    {
        public string UsrImage { get; set; }
        public bool Isdatabase { get; set; }

        public ImagePathInfo GetImage(Ctx ctx, int ad_image_id, int height, int width, string URI)
        {
            MImage mimg = new MImage(ctx, ad_image_id, null);
            bool url = false;
            var value = mimg.GetThumbnail(height, width, out url);
            if (url)
            {
                UsrImage = URI + "/Images/" + value;
                return new ImagePathInfo() { Path = value, IsUrl = true };
            }
            else
            {
                UsrImage = "data:image/jpg;base64," + value;
                return new ImagePathInfo() { Bytes = value, IsUrl = false };
            }
        }


        //public ImagePathInfo GetImageForWindowControl(Ctx ctx, int ad_image_id, int height, int width)
        //{
        //    MImage mimg = new MImage(ctx, ad_image_id, null);
        //    if (!string.IsNullOrEmpty(mimg.GetFontName()))
        //    {
        //        return new ImagePathInfo() { ClassName = mimg.GetFontName() };
        //    }
        //    var value = mimg.GetThumbnailByte(height, width);
        //    if (value != null)
        //    {
        //        UsrImage = Convert.ToBase64String(value);
        //        //obj.UsrImage = Convert.ToBase64String(mimg.GetBinaryData());
        //        if (mimg.GetBinaryData() != null)
        //        {
        //            Isdatabase = true;
        //        }
        //        return new ImagePathInfo() { Path = UsrImage };
        //    }
        //    return null;
        //}

        /// <summary>
        /// Save images
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="serverPath"></param>
        /// <param name="file"></param>
        /// <param name="imageID"></param>
        /// <param name="isDatabaseSave"></param>
        /// <returns></returns>
        public int SaveImage(Ctx ctx, string serverPath, HttpPostedFileBase file, int imageID, bool isDatabaseSave)
        {
            // check if its new insert or upadate, true in case if upload file has value
            MemoryStream ms = null;
            try
            {
                if (file != null)
                {
                    HttpPostedFileBase hpf = file as HttpPostedFileBase;

                    string savedFileName = Path.Combine(serverPath, Path.GetFileName(hpf.FileName));
                    hpf.SaveAs(savedFileName);
                    ms = new MemoryStream();
                    hpf.InputStream.CopyTo(ms);
                    byte[] byteArray = ms.ToArray();
                    FileInfo file1 = new FileInfo(savedFileName);
                    if (file1.Exists)
                    {
                        file1.Delete(); //Delete Temporary file             
                    }
                    ms.Dispose();
                    ms = null;
                    string imgByte = Convert.ToBase64String(byteArray);
                    var id = CommonFunctions.SaveImage(ctx, byteArray, imageID, hpf.FileName.Substring(hpf.FileName.LastIndexOf('.')), isDatabaseSave);
                    return id;
                }
                else
                {
                    // update database with image blob, this occurs when save in db check box changed
                    if (imageID > 0)
                    {
                        string imageUrl = new MImage(ctx, imageID, null).GetImageURL();
                        if (imageUrl != null)
                        {
                            string imageName = imageUrl.Substring(imageUrl.LastIndexOf('/') + 1);
                            System.Drawing.Image img = System.Drawing.Image.FromFile(Path.Combine(HostingEnvironment.MapPath(@"~/Images"), imageName));
                            ms = new MemoryStream();
                            img.Save(ms, img.RawFormat);
                            var id = CommonFunctions.SaveImage(ctx, ms.ToArray(), imageID, imageName, isDatabaseSave);
                            ms.Dispose();
                            ms = null;
                            return id;
                        }
                        else
                            return 0;
                    }
                    else
                        return 0;
                }

            }
            finally
            {
                if (ms != null)
                {
                    ms.Dispose();
                    ms = null;
                }
            }
        }

        /// <summary>
        /// Delete image 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="ad_image_id"></param>
        /// <returns></returns>
        public int DeleteImage(Ctx ctx, int ad_image_id)
        {
            var ret = CommonFunctions.DeleteImage(ctx, ad_image_id);
            return 0;
        }

        public object GetArrayFromFile(string serverPath, HttpPostedFileBase file)
        {
            MemoryStream ms = null;
            string imgByte = "";
            HttpPostedFileBase hpf = null;
            try
            {
                hpf = file as HttpPostedFileBase;
                string savedFileName = Path.Combine(serverPath, Path.GetFileName(hpf.FileName));
                hpf.SaveAs(savedFileName);
                ms = new MemoryStream();
                hpf.InputStream.CopyTo(ms);
                byte[] byteArray = ms.ToArray();
                FileInfo file1 = new FileInfo(savedFileName);
                if (file1.Exists)
                {
                    file1.Delete(); //Delete Temporary file             
                }

                imgByte = Convert.ToBase64String(byteArray);
            }
            finally
            {
                if (ms != null)
                {
                    ms.Dispose();
                    ms = null;
                }

                hpf = null;
            }
            return imgByte;
        }

    }

    public class ImagePathInfo
    {
        public string Path { get; set; }

        public byte[] Bytes { get; set; }

        public bool IsUrl = false;
    }

}
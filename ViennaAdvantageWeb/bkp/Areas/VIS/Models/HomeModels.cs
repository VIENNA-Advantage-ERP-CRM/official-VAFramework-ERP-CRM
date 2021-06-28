using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Utility;
using VAdvantage.Model;
using System.Web.Hosting;
using System.Text;
using System.IO;

namespace VIS.Models
{
    # region Count of Home Page
    public class HomeModels
    {

        public int FollowUpCnt { get; set; }
        public int AppointmentCnt { get; set; }
        public int ToDoCnt { get; set; }
        public int NotesCnt { get; set; }
        public int IncommingRequestCnt { get; set; }

        public int RequestCnt { get; set; }
        public int NoticeCnt { get; set; }
        public int WorkFlowCnt { get; set; }

        public int TaskAssignByMeCnt { get; set; }
        public int MyTaskCnt { get; set; }
        public int KPICnt { get; set; }

        public string UsrName { get; set; }
        public string UsrEmail { get; set; }
        public string UsrImage { get; set; }
        public string UsrStatus { get; set; }
        public string Greeting { get; set; }
        public HomeFolloUpsInfo HomeFolloUpsInfo;
        //Save User Image
        public int SaveUserImage(Ctx ctx, byte[] buffer, string imageName, bool isSaveInDB)
        {

            MUser user = new MUser(ctx, ctx.GetAD_User_ID(), null);
            int imageID = Util.GetValueOfInt(user.GetAD_Image_ID());

            MImage mimg = new MImage(ctx, imageID, null);
            mimg.ByteArray = buffer;
            mimg.ImageFormat = imageName.Substring(imageName.LastIndexOf('.'));
            mimg.SetName(imageName);
            if (isSaveInDB)
            {
                mimg.SetBinaryData(buffer);
                mimg.SetImageURL(string.Empty);
            }
            else
            {
                //mimg.SetImageURL(HostingEnvironment.MapPath(@"~/Images/100by100"));//Image Saved in File System so instead of byteArray image Url will be set
                mimg.SetImageURL(mimg.ImageFormat);//Image Saved in File System so instead of byteArray image Url will be set
                mimg.SetBinaryData(new byte[0]);


            }
            if (!mimg.Save())
            {
                return 0;
            }
            //mimg = new MImage(ctx, imageID, null);
            user.SetAD_Image_ID(mimg.GetAD_Image_ID());
            if (!user.Save())
            {
                return 0;
            }


            return mimg.GetAD_Image_ID();
        }


    }
    #endregion

    #region Follups
    //follups data
    public class HomeFolloUps
    {

        public int ChatID { get; set; }
        public int ChatEntryID { get; set; }
        public int EntryID { get; set; }
        public int TableID { get; set; }
        public string CName { get; set; }
        public int RecordID { get; set; }
        public int WinID { get; set; }
        public int SubscriberID { get; set; }

        public string Name { get; set; }
        public string ChatData { get; set; }
        public string WinName { get; set; }
        public string WinImage { get; set; }
        //public object UsrImage { get; set; }
        public int AD_Image_ID { get; set; }
        public DateTime Cdate { get; set; }
        public string TableName { get; set; }
        public int AD_User_ID { get; set; }
        public string Identifier { get; set; }
    }
    //follups usrimage
    public class FllUsrImages
    {
        public object UserImg { get; set; }
        public int AD_Image_ID { get; set; }
    }
    //follups list
    public class HomeFolloUpsInfo
    {
        public int FllCnt { get; set; }
        public List<FllUsrImages> lstUserImg { get; set; }
        public List<HomeFolloUps> lstFollowups { get; set; }
    }


    #endregion

    #region Notice
    public class HomeNotice
    {
        public int AD_Note_ID { get; set; }
        public int AD_Table_ID { get; set; }
        public int AD_Window_ID { get; set; }
        public int Record_ID { get; set; }
        public string MsgType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TableName { get; set; }
        public DateTime CDate { get; set; }
        public bool SpecialTable { get; set; }
        public string ProcessTableName { get; set; }
        public int ProcessWindowID { get; set; }
    }

    #endregion

    #region Request
    public class HomeRequest
    {
        public int R_Request_ID { get; set; }
        public int AD_Window_ID { get; set; }
        public string DocumentNo { get; set; }
        public string TableName { get; set; }
        public string Name { get; set; }
        public string CaseType { get; set; }
        public string Summary { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime? NextActionDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    #endregion


}
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Classes;

namespace VIS.Models
{
    public class BPartnerModel
    {
        Ctx ctx = null;
        private const string CUSTOMER = "Customer";
        private const string VENDOR = "Vendor";
        private const string EMPLOYEE = "Employee";
        private const string PROSPECT = "Prospect";
        private const string CUSTOMERMASTER = "Customer Master";
        private const string VENDORMASTER = "Vendor Master";
        private const string EMPLOYEEMASTER = "Employee Master";
        private const string PROSPECTMASTER = "Prospects";
        public string whereClause = string.Empty;

        //private int _windowNo;
        private bool _readOnly = false;

        // private int _line;
        public List<BPInfo> _greeting;

        /*****************************************/
        public List<BPInfo> _bpGroup;
        public List<BPInfo> _bpRelation;
        public List<BPInfo> _bpLocation;
        /*****************************************/
        /**	Logger			*/

        //

        public string searchKey = string.Empty;
        public string name = string.Empty;
        public string name2 = string.Empty;
        public string phoneNo = string.Empty;
        public string userImage = string.Empty;
        public string phoneNo2 = string.Empty;
        public string mobile = string.Empty;
        public string fax = string.Empty;
        public string contact = string.Empty;
        public string title = string.Empty;
        public string email = string.Empty;
        public string greeting = string.Empty;
        public string greeting1 = string.Empty;
        public int location = 0;
        public int bpGroupID = 0;
        public string bpRelationID = string.Empty;
        public string WebUrl = string.Empty;
        //public string bpRelationID = string.Empty;
        public string bpLocationID = string.Empty;
        public bool isCustomer = false;
        public bool isVendor = false;
        public bool isEmployee = false;
        public int tableID = 0;
        int C_BPartner_ID = 0;

        //private string BPtype = null;
        /// <summary>
        /// constructor with no parameter 
        /// </summary>
        public BPartnerModel()
        {
        }

        /// <summary>
        /// Constructor with parameter call when open business Partner 
        /// </summary>
        /// <param name="WinNo"></param>
        /// <param name="bPartnerID"></param>
        /// <param name="bpType"></param>
        public BPartnerModel(int WinNo, int bPartnerID, string bpType, Ctx context)
        {
            ctx = context;
            InitBPartner(WinNo, bPartnerID, bpType);

        }
        private void InitBPartner(int WinNo, int bPartnerID, string bpType)
        {

            C_BPartner_ID = bPartnerID;
            bool ro = false;
            DataSet ds = null;


            log.Config("C_BPartner_ID=" + bPartnerID);
            //  New bpartner
            if (bPartnerID == 0)
            {
                _partner = null;
                _pLocation = null;
                _user = null;
                _bprelation = null;
                _bpLocation = null;
                _bpGroup = null;
                //return true;
            }

            _partner = new MBPartner(Env.GetCtx(), bPartnerID, null);
            if (_partner.Get_ID() != 0)
            {
                //	Contact - Load values
                _pLocation = _partner.GetLocation(
                    Env.GetCtx().GetContextAsInt(WinNo, "C_BPartner_Location_ID"));
                _user = _partner.GetContact(
                   Env.GetCtx().GetContextAsInt(WinNo, "AD_User_ID"));
            }

            isCustomer = _partner.IsCustomer();
            isVendor = _partner.IsVendor();
            isEmployee = _partner.IsEmployee();
            _readOnly = !MRole.GetDefault(Env.GetCtx()).CanUpdate(
                Env.GetCtx().GetAD_Client_ID(), Env.GetCtx().GetAD_Org_ID(),
                MBPartner.Table_ID, 0, false);
            log.Info("R/O=" + _readOnly);

            //	Get Data
            _greeting = FillGreeting();

            /************************************/
            _bpGroup = FillBPGroup();
            // VIS0060
            //_bpRelation = FillBPRelation();
            //_bpLocation = FillBPLocation(0, ctx);
            /************************************/
            ro = _readOnly;
            if (!ro)
                ro = !MRole.GetDefault(Env.GetCtx()).CanUpdate(
                    Env.GetCtx().GetAD_Client_ID(), Env.GetCtx().GetAD_Org_ID(),
                    MBPartnerLocation.Table_ID, 0, false);
            if (!ro)
                ro = !MRole.GetDefault(Env.GetCtx()).CanUpdate(
                    Env.GetCtx().GetAD_Client_ID(), Env.GetCtx().GetAD_Org_ID(),
                    MLocation.Table_ID, 0, false);

            ds = DB.ExecuteDataset("Select C_BPartnerRelation_ID, c_bpartnerrelation_location_id from C_BP_Relation where c_bpartner_id=" + _partner.GetC_BPartner_ID());

            LoadBPartner(C_BPartner_ID, ds);

        }

        public bool LoadBPartner(int C_BPartner_ID, DataSet ds)
        {
            log.Config("C_BPartner_ID=" + C_BPartner_ID);
            ////  New bpartner
            if (C_BPartner_ID == 0)
            {
                _partner = null;
                _pLocation = null;
                _user = null;
                _bprelation = null;
                _bpLocation = null;
                _bpGroup = null;
                return true;
            }

            //_partner = new MBPartner(Env.GetCtx(), C_BPartner_ID, null);
            if (_partner.Get_ID() == 0)
            {
                //Classes.ShowMessage.Error("BPartnerNotFound", null);

            }

            //	BPartner - Load values
            searchKey = _partner.GetValue() ?? "";
            greeting = GetGreeting(_partner.GetC_Greeting_ID());
            name = _partner.GetName() ?? "";
            name2 = _partner.GetName2() ?? "";
            WebUrl = _partner.GetURL();

            if (_pLocation != null)
            {
                location = _pLocation.GetC_Location_ID();

                //
                phoneNo = _pLocation.GetPhone() ?? "";
                phoneNo2 = _pLocation.GetPhone2() ?? "";
                fax = _pLocation.GetFax() ?? "";
            }
            //	User - Load values

            if (_user != null)
            {
                greeting1 = GetGreeting(_user.GetC_Greeting_ID());
                contact = _user.GetName() ?? "";
                title = _user.GetTitle() ?? "";
                email = _user.GetEMail() ?? "";
                //
                phoneNo = _user.GetPhone() ?? "";
                phoneNo2 = _user.GetPhone2() ?? "";
                mobile = _user.GetMobile() ?? "";
                fax = _user.GetFax() ?? "";
                userImage = GetUserImage(_user.GetAD_Image_ID());

            }
            bpGroupID = _partner.GetC_BP_Group_ID();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    bpRelationID = Convert.ToString(ds.Tables[0].Rows[i]["C_BPartnerRelation_ID"]);
                    bpLocationID = Convert.ToString(ds.Tables[0].Rows[i]["c_bpartnerrelation_location_id"]);
                }
            }


            return true;
        }


        private string GetUserImage(int imageID)
        {
            string image = string.Empty;
            if (imageID > 0)
            {
                MImage objImage = new MImage(ctx, imageID, null);
                // byte[] imageByte = objImage.GetThumbnailByte(320, 185);
                byte[] imageByte = objImage.GetThumbnailByte(320, 240);
                if (imageByte != null)
                {
                    image = "data:image/jpg;base64," + Convert.ToBase64String(imageByte);
                }
                else
                {
                    image = "Areas/VIS/Images/home/User.png";
                }
            }
            else
            { image = "Areas/VIS/Images/home/User.png"; }
            return image;
        }

        private string GetGreeting(int key)
        {
            for (int i = 0; i < _greeting.Count; i++)
            {
                if (_greeting[i].ID == key)
                    return Convert.ToString(_greeting[i].ID);
            }
            return "";
        }

        /// <summary>
        /// Get BPartner ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public int GetBPartnerID(int userID)
        {
            int c_BPartner_ID = 0;
            string sqlQuery = "select c_bpartner_id from ad_user where ad_user_id =" + userID;
            c_BPartner_ID = Util.GetValueOfInt(DB.ExecuteScalar(sqlQuery, null, null));
            return c_BPartner_ID;
        }
        /// <summary>
        /// Get C_order_id
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public int GetCOrderID(int userID)
        {
            int c_Order_ID = 0;
            string sqlQuery = "select C_ORDER_ID from C_ORDER where ad_user_id =" + userID;
            c_Order_ID = Util.GetValueOfInt(DB.ExecuteScalar(sqlQuery, null, null));
            return c_Order_ID;
        }
        public List<BPInfo> lstBPGroup = null;
        public List<BPInfo> FillBPGroup()
        {
            lstBPGroup = new List<BPInfo>();
            lstBPGroup.Add(new BPInfo()
            {
                ID = 0,
                Name = ""
            });
            DataSet ds = new DataSet();
            String sql = "select c_bp_group_id, Name  from c_bp_group WHERE IsActive='Y' ";
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "c_bp_group", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);
            sql += "ORDER BY 2";
            ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstBPGroup.Add(new BPInfo()
                    {
                        ID = Convert.ToInt32(ds.Tables[0].Rows[i]["C_BP_GROUP_ID"]),
                        Name = Convert.ToString(ds.Tables[0].Rows[i]["NAME"])
                    });
                }
            }
            return lstBPGroup;
        }
        /// <summary>
        ///Fill Greeting
        /// </summary>
        /// <returns>Array of Greetings</returns>
        public List<BPInfo> lstBPRelation = null;
        /// <summary>
        /// Fill BPRelation
        /// </summary>
        /// <returns></returns>
        public List<BPInfo> FillBPRelation()
        {
            lstBPRelation = new List<BPInfo>();
            lstBPRelation.Add(new BPInfo()
            {
                ID = 0,
                Name = ""
            });
            DataSet ds = new DataSet();
            String sql = "select c_bpartner_id, Name  from c_bpartner WHERE IsActive='Y' ";
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "c_bpartner", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);
            sql += "ORDER BY 2";
            ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstBPRelation.Add(new BPInfo()
                    {
                        ID = Convert.ToInt32(ds.Tables[0].Rows[i]["C_BPartner_ID"]),
                        Name = Convert.ToString(ds.Tables[0].Rows[i]["NAME"])
                    });
                }
            }
            return lstBPRelation;
        }

        /// <summary>
        /// Fill BPLocation
        /// </summary>
        public List<BPInfo> lstBPLocation = null;
        public List<BPInfo> FillBPLocation(int c_bpartner_id, Ctx ctx)
        {
            lstBPLocation = new List<BPInfo>();
            lstBPLocation.Add(new BPInfo()
            {
                ID = 0,
                Name = ""
            });
            DataSet ds = new DataSet();
            String sql = "select c_bpartner_location_id, Name  from c_bpartner_location WHERE IsActive='Y' and c_bpartner_id=" + c_bpartner_id;
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "c_bpartner_location", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);
            sql += "ORDER BY 2";
            ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstBPLocation.Add(new BPInfo()
                    {
                        ID = Convert.ToInt32(ds.Tables[0].Rows[i]["C_BPartner_Location_ID"]),
                        Name = Convert.ToString(ds.Tables[0].Rows[i]["NAME"])
                    });
                }
            }
            return lstBPLocation;
        }
        public List<BPInfo> lstGreeting = null;
        /// <summary>
        ///Fill Greeting
        /// </summary>
        /// <returns>Array of Greetings</returns>
        public List<BPInfo> FillGreeting()
        {
            lstGreeting = new List<BPInfo>();
            lstGreeting.Add(new BPInfo()
            {
                ID = 0,
                Name = ""
            });
            DataSet ds = new DataSet();
            String sql = "SELECT C_Greeting_ID, Name FROM C_Greeting WHERE IsActive='Y' ";
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "C_Greeting", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);
            sql += "ORDER BY 2";
            ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstGreeting.Add(new BPInfo()
                    {
                        ID = Convert.ToInt32(ds.Tables[0].Rows[i]["C_GREETING_ID"]),
                        Name = Convert.ToString(ds.Tables[0].Rows[i]["NAME"])
                    });
                }
            }
            return lstGreeting;
        }



        private MBPartnerLocation _pLocation = null;
        private MBPartner _partner = null;

        private MUser _user = null;
        X_C_BP_Relation _bprelation = null;
        private static VLogger log = VLogger.GetVLogger(typeof(BPartnerModel).FullName);
        /// <summary>
        /// Add Or Update Business Partner
        /// </summary>
        /// <param name="searchKey"></param>
        /// <param name="name"></param>
        /// <param name="name2"></param>
        /// <param name="greeting"></param>
        /// <param name="bpGroup"></param>
        /// <param name="bpRelation"></param>
        /// <param name="bpLocation"></param>
        /// <param name="contact"></param>
        /// <param name="greeting1"></param>
        /// <param name="title"></param>
        /// <param name="email"></param>
        /// <param name="address"></param>
        /// <param name="phoneNo"></param>
        /// <param name="phoneNo2"></param>
        /// <param name="fax"></param>
        /// <param name="ctx"></param>
        /// <param name="_windowNo"></param>
        /// <param name="BPtype"></param>
        /// <param name="C_BPartner_ID"></param>
        /// <returns></returns>

        public string AddBPartner(string searchKey, string name, string name2, string greeting, string bpGroup, string bpRelation, string bpLocation, string contact, string greeting1, string title, string email, string address, string phoneNo, string phoneNo2, string fax, Ctx ctx, int _windowNo, string BPtype, int C_BPartner_ID, bool isCustomer, bool isVendor, bool isProspect, string fileUrl, string umobile, string webUrl, bool isEmployee)
        {
            StringBuilder strError = new StringBuilder();
            int AD_Client_ID = ctx.GetAD_Client_ID();
            if (C_BPartner_ID > 0)
            {
                _partner = new MBPartner(ctx, C_BPartner_ID, null);
            }
            else
            {
                _partner = MBPartner.GetTemplate(ctx, AD_Client_ID);
            }
            bool isSOTrx = ctx.IsSOTrx(_windowNo);
            _partner.SetIsCustomer(isSOTrx);
            _partner.SetIsVendor(!isSOTrx);
            // JID_1197 IN Business partner  updating Createdby,Updatedby,Created,Updated fields as per changed date
            _partner.Set_ValueNoCheck("CreatedBy", ctx.GetAD_User_ID());
            _partner.Set_ValueNoCheck("Created", DateTime.Now);
            _partner.Set_ValueNoCheck("Updated", DateTime.Now);
            _partner.Set_ValueNoCheck("UpdatedBy", ctx.GetAD_User_ID());
            if (BPtype != null && (!isCustomer && !isVendor))
            {
                if (BPtype.Contains("Customer"))
                {
                    _partner.SetIsCustomer(true);
                }
                if (BPtype.Contains("Employee"))
                {
                    _partner.SetIsEmployee(true);
                }
                if (BPtype.Contains("Vendor"))
                {
                    _partner.SetIsVendor(true);
                }
                if (BPtype.Contains("Prospect"))
                {
                    _partner.SetIsProspect(true);
                }
                /*
                if (BPtype == "Customer")
                {
                    _partner.SetIsCustomer(true);
                }
                else if (BPtype == "Employee")
                {
                    _partner.SetIsEmployee(true);
                }
                else if (BPtype == "Vendor")
                {
                    _partner.SetIsVendor(true);
                }
                else if (BPtype == "Prospect")
                {
                    _partner.SetIsProspect(true);
                }*/
            }
            if (isCustomer)
            {
                _partner.SetIsCustomer(true);
            }
            else
            {
                _partner.SetIsCustomer(false);
            }
            if (isVendor)
            {
                _partner.SetIsVendor(true);
            }
            else
            {
                _partner.SetIsVendor(false);
            }
            if (isProspect)
            {
                _partner.SetIsProspect(true);
            }
            else
            {
                _partner.SetIsProspect(false);
            }

            if (isEmployee)
            {
                _partner.SetIsEmployee(true);
            }
            else
            {
                _partner.SetIsEmployee(false);
            }

            if (searchKey == null || searchKey.Length == 0)
            {
                //	get Table Documet No
                searchKey = MSequence.GetDocumentNo(ctx.GetAD_Client_ID(), "C_BPartner", null, ctx);
                //Dispatcher.BeginInvoke(() => { txtValue.Text = value; });
            }
            _partner.SetValue(searchKey);
            //
            _partner.SetName(name);
            _partner.SetURL(webUrl);
            //  _partner.SetName2(name2);
            //KeyNamePair p = (KeyNamePair)cmbGreetingBP.SelectedItem;
            //if (greeting >0)
            //{
            //    _partner.SetC_Greeting_ID(greeting);
            //}
            //else
            //{
            //    _partner.SetC_Greeting_ID(0);
            //}
            if (greeting != string.Empty)
            {
                _partner.SetC_Greeting_ID(Convert.ToInt32(greeting));
            }
            else
            {
                _partner.SetC_Greeting_ID(0);
            }
            /***************************************************/
            _partner.SetC_BP_Group_ID(Util.GetValueOfInt(bpGroup));
            /***************************************************/

            if (_partner.Save())
            {
                log.Fine("C_BPartner_ID=" + _partner.GetC_BPartner_ID());
            }
            else
            {
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null && !string.IsNullOrEmpty(pp.GetName()))
                {
                    strError.Append("BPSaveError -- " + pp.GetName());
                }
                // Classes.ShowMessage.Error("SearchKeyExist", null);
                //strError.Append("SearchKeyExist");
                //this.Cursor = Cursors.Arrow;
                return strError.ToString();
            }

            //	***** Business Partner - Location *****
            if (_pLocation == null)
                if (C_BPartner_ID > 0)
                {
                    _pLocation = new MBPartnerLocation(ctx, GetBPartnerLocationID(_partner.Get_ID()), null);
                    if (_pLocation.Get_ID() <= 0)
                    {
                        _pLocation = new MBPartnerLocation(_partner);
                    }
                }
                else
                {
                    _pLocation = new MBPartnerLocation(_partner);
                }
            if (address != string.Empty)
            {
                _pLocation.SetC_Location_ID(Convert.ToInt32(address));
            }

            //
            _pLocation.SetPhone(phoneNo);
            // _pLocation.SetPhone2(phoneNo2);
            _pLocation.SetFax(fax);
            if (_pLocation.Save())
            {
                log.Fine("C_BPartner_Location_ID=" + _pLocation.GetC_BPartner_Location_ID());
            }
            else
            {
                //   ADialog.error(m_WindowNo, this, "BPartnerNotSaved", Msg.translate(Env.getCtx(), "C_BPartner_Location_ID"));
                // Classes.ShowMessage.Error("BPartnerNotSaved", null);
                //this.Cursor = Cursors.Arrow;
                strError.Append("BPartnerNotSaved");
                return strError.ToString();
            }

            //	***** Business Partner - User *****
            //String contact = txtContact.Text;
            //String email = txtEMail.Text;
            if (_user == null && (contact.Length > 0 || email.Length > 0))
                if (C_BPartner_ID > 0)
                {
                    _user = new MUser(ctx, GetUserID(_partner.Get_ID()), null);
                }
                else
                {
                    _user = new MUser(_partner);
                }
            if (_user != null)
            {
                if (contact.Length == 0)
                    contact = name;
                _user.SetName(contact);
                _user.SetEMail(email);
                _user.SetTitle(title);
                _user.SetC_Location_ID(Convert.ToInt32(address));

                // = (KeyNamePair)cmbGreetingC.SelectedItem;

                //if (greeting1 >0)
                //    _user.SetC_Greeting_ID(greeting1);
                if (greeting1 != string.Empty)
                    _user.SetC_Greeting_ID(Convert.ToInt32(greeting1));
                else
                    _user.SetC_Greeting_ID(0);
                //
                _user.SetPhone(phoneNo);
                // _user.SetPhone2(phoneNo2);
                _user.SetMobile(umobile);
                _user.SetFax(fax);
                _user.SetC_BPartner_Location_ID(_pLocation.GetC_BPartner_Location_ID());
                if (_user.Save())
                {
                    if (fileUrl != null && fileUrl != string.Empty)
                    {
                        _user.SetAD_Image_ID(SaveUserImage(ctx, fileUrl, _user.GetAD_User_ID()));
                    }
                    if (_user.Save())
                    {
                        log.Fine("AD_User_ID(AD_Image_ID)=" + _user.GetAD_User_ID() + "(" + _user.GetAD_Image_ID() + ")");
                    }
                    log.Fine("AD_User_ID=" + _user.GetAD_User_ID());
                }
                else
                {
                    //Classes.ShowMessage.Error("BPartnerNotSaved", null);
                    //this.Cursor = Cursors.Arrow;
                    strError.Append("BPartnerNotSaved");
                    return strError.ToString();
                }

                /*************************************************/
                if ((bpRelation != null && bpLocation != null) && (bpRelation != string.Empty && bpLocation != string.Empty))
                {
                    if (bpRelation.ToString().Trim() == "" || bpLocation.ToString().Trim() == "")
                    {
                        int dele = DB.ExecuteQuery("DELETE from C_BP_Relation where c_bpartner_id=" + _partner.GetC_BPartner_ID(), null, null);
                        if (dele == -1)
                        {
                            log.SaveError("C_BP_RelationNotDeleted", "c_bpartner_id=" + _partner.GetC_BPartner_ID());
                        }
                    }
                    else
                    {
                        //Business Partner Relation 
                        if (C_BPartner_ID > 0)
                        {
                            _bprelation = new X_C_BP_Relation(ctx, GetBPRelationID(_partner.Get_ID()), null);
                        }
                        else
                        {
                            _bprelation = new X_C_BP_Relation(ctx, 0, null);
                        }
                        _bprelation.SetAD_Client_ID(_partner.GetAD_Client_ID());
                        _bprelation.SetAD_Org_ID(_partner.GetAD_Org_ID());
                        _bprelation.SetName(_partner.GetName());
                        _bprelation.SetDescription(_partner.GetDescription());
                        _bprelation.SetC_BPartner_ID(_partner.GetC_BPartner_ID());
                        _bprelation.SetC_BPartner_Location_ID(_pLocation.GetC_BPartner_Location_ID());
                        _bprelation.SetC_BP_Relation_ID(Util.GetValueOfInt(bpRelation));
                        _bprelation.SetC_BPartnerRelation_ID(Util.GetValueOfInt(bpRelation));
                        _bprelation.SetC_BPartnerRelation_Location_ID(Util.GetValueOfInt(bpLocation));
                        _bprelation.SetIsBillTo(true);
                        if (_bprelation.Save())
                        {
                            log.Fine("C_BP_Relation_ID=" + _bprelation.GetC_BP_Relation_ID());
                        }
                        else
                        {
                            //Classes.ShowMessage.Error("BPRelationNotSaved", null);

                            //this.Cursor = Cursors.Arrow;
                            strError.Append("BPRelationNotSaved");
                            return strError.ToString();
                        }
                    }
                }
                /*************************************************/

            }
            return strError.ToString();

        }


        private int SaveUserImage(Ctx ctx, string fileUrl, int userID)
        {
            int imageID = 0;
            if (File.Exists(fileUrl))
            {
                byte[] byteArray = File.ReadAllBytes(fileUrl);
                string fileName = Path.GetFileName(fileUrl);
                File.Delete(fileUrl); //Delete Temporary file             
                imageID = CommonFunctions.SaveUserImage(ctx, byteArray, fileName, false, userID);
            }
            return imageID;
        }

        /// <summary>
        /// Get window ID
        /// </summary>
        /// <param name="windowName"></param>
        /// <returns></returns>
        public int GetWindowID(string windowName)
        {
            return Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_WINDOW_ID FROM AD_WINDOW WHERE NAME='" + windowName + "' AND ISACTIVE='Y'", null, null));

        }
        /// <summary>
        /// Get user ID
        /// </summary>
        /// <param name="C_BPartner_ID"></param>
        /// <returns></returns>
        public int GetUserID(int C_BPartner_ID)
        {
            return Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_USER_ID FROM AD_USER WHERE C_BPARTNER_ID='" + C_BPartner_ID + "' AND ISACTIVE='Y'", null, null));

        }

        /// <summary>
        /// Get BPartnerLocation Id
        /// </summary>
        /// <param name="C_BPartner_ID"></param>
        /// <returns></returns>
        public int GetBPartnerLocationID(int C_BPartner_ID)
        {
            return Util.GetValueOfInt(DB.ExecuteScalar("SELECT c_bpartner_location_ID FROM c_bpartner_location WHERE C_BPARTNER_ID='" + C_BPartner_ID + "' AND ISACTIVE='Y'", null, null));

        }
        /// <summary>
        /// Get BPRelation ID
        /// </summary>
        /// <param name="C_BPartner_ID"></param>
        /// <returns></returns>
        public int GetBPRelationID(int C_BPartner_ID)
        {
            return Util.GetValueOfInt(DB.ExecuteScalar("SELECT c_bp_relation_ID FROM c_bp_relation WHERE C_BPARTNER_ID='" + C_BPartner_ID + "' AND ISACTIVE='Y'", null, null));

        }

        /// <summary>
        /// Get table id
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public int GetTableID(string TableName)
        {
            return Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Table_ID FROM AD_Table WHERE TABLENAME='" + TableName + "' AND ISACTIVE='Y'", null, null));

        }
    }
    public class BPInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

}
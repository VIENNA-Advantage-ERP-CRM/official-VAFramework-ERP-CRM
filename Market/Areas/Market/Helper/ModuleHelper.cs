using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Utility;

namespace Market.Helper
{
    public class ModuleHelper
    {
        internal static object GetModuleInfo(MarketSvc.MService.ModuleContainer md, string modType, Ctx ctx)
        {

            if (md == null)
                return md;

            StringBuilder _depInfo = null;
            bool isUpdateMod = false;
            Int16 updateIndex = -1;

            int len = md.lstModuleInfo.Count;

            for (int i = 0; i < len; i++)
            {
                var mInfo = md.lstModuleInfo[i];

                _depInfo = new StringBuilder();
                //Lakhwinder
                if (!(mInfo.ModuleDependency == null || mInfo.ModuleDependency.Count == 0))
                {
                    mInfo.DependentModInstalled = new List<bool>();
                    for (int j = 0; j < mInfo.ModuleDependency.Count; j++)
                    {
                        var info = mInfo.ModuleDependency[j];



                        if (info.Lable == 1)
                        {
                            _depInfo.Append(info.Name + " " + info.VersionNo + "\t\n");
                        }

                        bool? isInstalled = Env.IsModuleAlreadyInstalled(mInfo.ModuleDependency[j].Prefix, mInfo.ModuleDependency[j].VersionNo, mInfo.ModuleDependency[j].Name);

                        if (!(isInstalled != null && isInstalled.Value))
                        {
                            mInfo.DependentModInstalled.Add(false);
                        }
                        else
                        {
                            mInfo.DependentModInstalled.Add(true);
                        }
                    }
                }
                if (_depInfo.Length > 0)
                {
                    if (mInfo.ColNameValues.ContainsKey("DEPENDENCYINFO"))
                    {
                        mInfo.ColNameValues["DEPENDENCYINFO"] = _depInfo.ToString();
                    }
                }

                mInfo.ModMedia = new List<MarketSvc.MService.MMedia>();

                mInfo.ModMedia.Add(new MarketSvc.MService.MMedia() { data = mInfo.Image, isVideo = false });

                for (int img = 0; img < mInfo.Images.Count; img++)
                {
                    mInfo.ModMedia.Add(new MarketSvc.MService.MMedia() { data = mInfo.Images[img] });
                }

                for (int v = 0; v < mInfo.Videos.Count; v++)
                {
                    if (mInfo.Videos[v] != null && mInfo.Videos[v].ToString() != "")
                    {
                        mInfo.ModMedia.Add(new MarketSvc.MService.MMedia() { data = "http://i.ytimg.com/vi/" + mInfo.Videos[v] + "/default.jpg", isVideo = true, id = mInfo.Videos[v].ToString() });
                    }
                }

                 string installedVersion = "";
                if (modType != "PLAN")
                {

                    bool? installed = Env.IsModuleAlreadyInstalled(mInfo.Prefix, mInfo.VersionNo, mInfo.Name);
                    Tuple<string, string, string> assemblyName = null;
                   
                    if (Env.HasModulePrefix(mInfo.Prefix, out assemblyName))
                    {
                        installedVersion =  Msg.GetMsg(ctx,"Market_InstalledVersion") + " " + assemblyName.Item3.Substring(assemblyName.Item3.IndexOf("=") + 1)  ;
                    }


                    else if (mInfo.Prefix == "VIS_")
                    {
                       
                          installedVersion =  Env.GetVISInstalledVersion(mInfo.Name);
                          if (!String.IsNullOrEmpty(installedVersion))
                          {
                              installedVersion = Msg.GetMsg(ctx, "Market_InstalledVersion") + " " + installedVersion;
                          }
                    }


                    //m.IsEnabled = true;
                    if (installed == null)
                    {
                        mInfo.ButtonText = Msg.GetMsg(ctx, "Update");
                        mInfo.IsEnabled = true;
                        mInfo.ItemText = Msg.GetMsg(ctx, "Update");
                        // m.InstallIcon = GetImageSource(null, "InstallIcon.png");
                        //m.LableForeground = new SolidColorBrush(Colors.Green);
                        mInfo.InstalledVersion = installedVersion;
                        isUpdateMod = true;
                        updateIndex++;
                    }
                    else if (installed.Value)
                    {
                        mInfo.ButtonText = Msg.GetMsg(ctx, "Market_REINSTALL");
                        mInfo.IsEnabled = true;
                        mInfo.ItemText =  Msg.GetMsg(ctx, "Market_Installed");
                        // m.InstallIcon = GetImageSource(null, "InstallIcon.png");
                        mInfo.InstalledVersion = installedVersion;
                    }
                    else
                    {
                        if (modType == "PAID" && !(mInfo.IsPaymentDone))
                        {

                            mInfo.ButtonText = Msg.GetMsg(ctx, "Buy");
                            mInfo.ItemText = Msg.GetMsg(ctx, "Buy");
                            // m.LableForeground = new SolidColorBrush(Colors.Black);
                            mInfo.IsEnabled = true;
                        }
                        else
                        {
                            mInfo.ButtonText = Msg.GetMsg(ctx, "Install");
                            //m.IsEnabled = true;
                            mInfo.ItemText = Msg.GetMsg(ctx, "Install");
                            // m.LableForeground = new SolidColorBrush(Colors.Black);
                            mInfo.InstalledVersion = installedVersion;
                        }
                    }
                }

                if (isUpdateMod && (modType == "MY" || modType == "SPL"))
                {
                    md.lstModuleInfo.Remove(mInfo);
                    md.lstModuleInfo.Insert(updateIndex, mInfo);
                    isUpdateMod = false;
                }
            }
            return md;
        }



        internal static object GetModuleInfo(MarketSvc.MService.MarketModuleInfo mInfo)
        {

            if (mInfo == null)
                return mInfo;

            StringBuilder _depInfo = null;
            

            //int len = md.lstModuleInfo.Count;

            //for (int i = 0; i < len; i++)
            //{
            //    var mInfo = md.lstModuleInfo[i];

                _depInfo = new StringBuilder();
                //Lakhwinder
                if (!(mInfo.ModuleDependency == null || mInfo.ModuleDependency.Count == 0))
                {
                    mInfo.DependentModInstalled = new List<bool>();
                    for (int j = 0; j < mInfo.ModuleDependency.Count; j++)
                    {
                        var info = mInfo.ModuleDependency[j];



                        if (info.Lable == 1)
                        {
                            _depInfo.Append(info.Name + " " + info.VersionNo + "\t\n");
                        }

                        bool? isInstalled = Env.IsModuleAlreadyInstalled(mInfo.ModuleDependency[j].Prefix, mInfo.ModuleDependency[j].VersionNo, mInfo.ModuleDependency[j].Name);

                        if (!(isInstalled != null && isInstalled.Value))
                        {
                            mInfo.DependentModInstalled.Add(false);
                        }
                        else
                        {
                            mInfo.DependentModInstalled.Add(true);
                        }
                    }
                }
               
            //}
                return mInfo;
        }
    }
}
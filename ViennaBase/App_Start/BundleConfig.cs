using System.Web;
using System.Web.Optimization;

namespace ViennaBase
{
    public class BundleConfig
    {

        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725

        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                      "~/Areas/ViennaBase/Scripts/jquery-2.1.0.min.js",
                      "~/Areas/ViennaBase/Scripts/handlebars.min.js",
                      "~/Areas/ViennaBase/Scripts/bootstrap.min.js"
                    ));

            bundles.Add(new StyleBundle("~/bundles/boot").Include(
                "~/Areas/ViennaBase/Content/bootstrap-theme.min.css",
                   "~/Areas/ViennaBase/Content/bootstrap.min.css"
                ));


            bundles.Add(new ScriptBundle("~/Areas/ViennaBase/Scripts/modernizr").Include(
                      "~/Areas/ViennaBase/Scripts/modernizr-2.6.2.js"));
            

            //Old jquery
            bundles.Add(new ScriptBundle("~/Areas/ViennaBase/bundles/jquery_v2").Include(
                          "~/Areas/ViennaBase/Scripts/jquery-2.1.0.min.js",
                          "~/Areas/ViennaBase/Scripts/bootstrap.min.js"
                        ));
            //New jquery
            bundles.Add(new ScriptBundle("~/Areas/ViennaBase/Scripts/jquery_v3").Include(
                         "~/Areas/ViennaBase/Scripts/migration/jquery-3.4.1.min.js",
                           "~/Areas/ViennaBase/Scripts/migration/jquery-migrate-3.1.0.min.js",
                        
                         "~/Areas/ViennaBase/Scripts/migration/popper-utils.min.js",
                         "~/Areas/ViennaBase/Scripts/migration/popper.min.js",
                          "~/Areas/ViennaBase/Scripts/migration/bootstrap.min.js"
                       ));

            
            //Old boot
            bundles.Add(new StyleBundle("~/Areas/ViennaBase/bundles/boot_v2").Include(
                "~/Areas/ViennaBase/Content/bootstrap-theme.min.css",
                   "~/Areas/ViennaBase/Content/bootstrap.min.css"
                ));
            //New boot
            bundles.Add(new StyleBundle("~/Areas/ViennaBase/Content/boot_v3").Include(
                   "~/Areas/ViennaBase/Content/migration/bootstrap.min.css",
                      "~/Areas/ViennaBase/Content/migration/bootstrap-migration-4.css",
                         "~/Areas/ViennaBase/Content/migration/ad-cust.css"
                ));

            
            //old UI
            bundles.Add(new StyleBundle("~/Areas/ViennaBase/Content/themes/base/minified/jquicss_v2").Include(
                 "~/Areas/ViennaBase/Content/themes/base/minified/jquery-ui.min.css"
               ));
            bundles.Add(new ScriptBundle("~/Areas/ViennaBase/Scripts/jqueryui_v2").Include(
                       "~/Areas/ViennaBase/Scripts/jquery-ui-1.10.4.min.js"
                        ));

            
            //new UI
            bundles.Add(new ScriptBundle("~/Areas/ViennaBase/Scripts/migration/jqueryui_v3").Include(
                       "~/Areas/ViennaBase/Scripts/migration/jquery-ui-1.12.1.min.js"
                        ));
            bundles.Add(new StyleBundle("~/Areas/ViennaBase/Content/migration/themes/base/jquicss_v3").Include(
                 "~/Areas/ViennaBase/Content/migration/themes/base/jquery-ui.min.css"
               ));



            bundles.Add(new StyleBundle("~/Areas/ViennaBase/Content/externalcss").Include(
                "~/Areas/ViennaBase/Content/w2ui/w2ui-1.4.3.min.css",
                "~/Areas/ViennaBase/Content/spectrum.css"
              ));

            bundles.Add(new StyleBundle("~/Areas/ViennaBase/Content/custom/customcss").Include(
               "~/Areas/ViennaBase/Content/custom/custom.css"
             ));


            bundles.Add(new StyleBundle("~/Areas/ViennaBase/Content/fontlib/fontcss").Include(
                "~/Areas/ViennaBase/Content/fontlib/font-awesome.min.css",
                "~/Areas/ViennaBase/Content/fontlib/glyphicons.css",
                "~/Areas/ViennaBase/Content/fontlib/vis-fonts.css"
              ));

            bundles.Add(new ScriptBundle("~/Areas/ViennaBase/Scripts/externaljs").Include(
                        "~/Areas/ViennaBase/Scripts/handlebars.min.js",
                        "~/Areas/ViennaBase/Scripts/w2ui/w2ui-1.4.3.min.js",
                        "~/Areas/ViennaBase/Scripts/spectrum.js",
                        "~/Areas/ViennaBase/Scripts/custom/custom.js",
                        "~/Areas/ViennaBase/Scripts/custom/vaautocomplete.js",
                        "~/Areas/ViennaBase/Scripts/custom/vadragdrop.js",
                        "~/Areas/ViennaBase/Scripts/FileSaver.js", 
                        "~/Areas/ViennaBase/Scripts/jszip.js", 
                        "~/Areas/ViennaBase/Scripts/jszip-utils.js", 
                        "~/Areas/ViennaBase/Scripts/downloader.js"
                ));

            bundles.Add(new ScriptBundle("~/Areas/ViennaBase/Scripts/toastrjs").Include(
                "~/Areas/ViennaBase/Scripts/toastr/toastr.min.js"
                ));

            bundles.Add(new StyleBundle("~/Areas/ViennaBase/Content/toastrcss").Include(
                "~/Areas/ViennaBase/Content/toastr/toastr.min.css"
                ));

            // For image select work
            bundles.Add(new ScriptBundle("~/Areas/ViennaBase/bundles/imgareaselecterjs").Include(
                "~/Areas/ViennaBase/Scripts/jquery.imgareaselect.js"
                ));




            //bundles.Add(new ScriptBundle("~/bundles/CRV").Include(
            //         "~/Areas/ViennaBase/Scripts/crystalreportviewers13/js/crviewer/crv.js"
            // ));

            //bundles.Add(new ScriptBundle("~/bundles/viennabase/css").Include("~/Areas/ViennaBase/Content/viennabase.min.css"));
            //bundles.Add(new ScriptBundle("~/bundles/viennabase/js").Include("~/Areas/ViennaBase/Scripts/viennabase.min.js"));
        }
    }
}
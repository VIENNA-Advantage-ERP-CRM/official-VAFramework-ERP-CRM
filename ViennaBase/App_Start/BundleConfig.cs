using System.Web;
using System.Web.Optimization;

namespace ViennaBase
{
    public class BundleConfig
    {

        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                      "~/Areas/ViennaBase/Scripts/modernizr-2.6.2.js"));


            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                          "~/Areas/ViennaBase/Scripts/jquery-2.1.0.min.js",
                          "~/Areas/ViennaBase/Scripts/handlebars.min.js",
                          "~/Areas/ViennaBase/Scripts/bootstrap.min.js"
                        ));

            bundles.Add(new StyleBundle("~/bundles/boot").Include(
                "~/Areas/ViennaBase/Content/bootstrap-theme.min.css",
                   "~/Areas/ViennaBase/Content/bootstrap.min.css"
                ));


            bundles.Add(new StyleBundle("~/Areas/ViennaBase/Content/themes/base/minified/jquicss").Include(
                 "~/Areas/ViennaBase/Content/themes/base/minified/jquery-ui.min.css",
                 "~/Areas/ViennaBase/Content/w2ui/w2ui-1.4.3.min.css",
                 "~/Areas/ViennaBase/Content/spectrum.css",
                 "~/Areas/ViennaBase/Content/custom/custom.css"
               ));


            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                       "~/Areas/ViennaBase/Scripts/jquery-ui-1.10.4.min.js",
                        "~/Areas/ViennaBase/Scripts/w2ui/w2ui-1.4.3.min.js",
                        "~/Areas/ViennaBase/Scripts/spectrum.js",
                        "~/Areas/ViennaBase/Scripts/custom/custom.js"

                        ));
            bundles.Add(new ScriptBundle("~/bundles/external").Include( 
                        "~/Areas/ViennaBase/Scripts/FileSaver.js", 
                        "~/Areas/ViennaBase/Scripts/jszip.js", 
                        "~/Areas/ViennaBase/Scripts/jszip-utils.js", 
                        "~/Areas/ViennaBase/Scripts/downloader.js"
                ));

            //bundles.Add(new ScriptBundle("~/bundles/viennabase/css").Include("~/Areas/ViennaBase/Content/viennabase.min.css"));
            //bundles.Add(new ScriptBundle("~/bundles/viennabase/js").Include("~/Areas/ViennaBase/Scripts/viennabase.min.js"));



        }
    }
}
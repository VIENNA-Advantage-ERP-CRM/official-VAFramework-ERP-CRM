using System.Web;
using System.Web.Optimization;

namespace ViennaAdvantageWeb
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            ViennaBase.BundleConfig.RegisterBundles(bundles);
            BundleTable.EnableOptimizations = true;
        }
    }
}
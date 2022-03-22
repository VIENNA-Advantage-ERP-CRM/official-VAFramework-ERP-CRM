using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Optimization;

namespace VAdvantage
{
    public class ModuleBundles
    {

        public static List<ScriptBundle> GetScriptBundles()
        {
            return VAModelAD.ModuleBundles.GetScriptBundles();
        }

        public static List<StyleBundle> GetStyleBundles()
        {
            return VAModelAD.ModuleBundles.GetStyleBundles();
        }

        public static List<StyleBundle> GetRTLStyleBundles()
        {
            return VAModelAD.ModuleBundles.GetRTLStyleBundles();
        }

        /// <summary>
        /// Regsiter module scripts bundle in app bundle list
        /// -- order matter
        /// -- module might be dependetdent on other module
        /// -- so parent module scripts regiter prior than child module 
        /// </summary>
        /// <param name="modScript">script bundle</param>
        /// <param name="prefix">module prefix</param>
        /// <param name="order">higher value late loading </param>
        public static void RegisterScriptBundle(ScriptBundle modScript, string prefix, int order = 1)
        {
            VAModelAD.ModuleBundles.RegisterScriptBundle(modScript, prefix, order);
        }

        /// <summary>
        /// Regsiter module style bundle in app bundle list
        /// -- order matter
        /// -- module might be dependetdent on other module
        /// -- so parent module scripts regiter prior than child module 
        /// </summary>
        /// <param name="styleBundle">style bundle</param>
        /// <param name="prefix">module prefix</param>
        /// <param name="order">higher value late loading </param>
        public static void RegisterStyleBundle(StyleBundle styleBundle, string prefix, int order)
        {
            VAModelAD.ModuleBundles.RegisterStyleBundle(styleBundle, prefix, order);
        }

        public static void RegisterRTLStyleBundle(StyleBundle styleBundle, string prefix, int order)
        {
            VAModelAD.ModuleBundles.RegisterRTLStyleBundle(styleBundle, prefix, order);
        }
    }
}

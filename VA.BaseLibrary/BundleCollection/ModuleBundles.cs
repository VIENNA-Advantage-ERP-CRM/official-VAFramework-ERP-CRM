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
        //static ScriptBundle modScript = null;
        //static StyleBundle modStyle = null;

        static SortedDictionary<int, Dictionary<string, ScriptBundle>> modScriptList = new SortedDictionary<int, Dictionary<string, ScriptBundle>>();
        static SortedDictionary<int, Dictionary<string, StyleBundle>> modStyleList = new SortedDictionary<int, Dictionary<string, StyleBundle>>();
        static SortedDictionary<int, Dictionary<string, StyleBundle>> modRTLStyleList = new SortedDictionary<int, Dictionary<string, StyleBundle>>();

        public static List<ScriptBundle> GetScriptBundles()
        {
            var lst = new List<ScriptBundle>();

            foreach (var o in modScriptList)
            {
                lst.AddRange(o.Value.Values);
            }
            return lst;
        }

        public static List<StyleBundle> GetStyleBundles()
        {
            var lst = new List<StyleBundle>();

            foreach (var o in modStyleList)
            {
                lst.AddRange(o.Value.Values);
            }
            return lst;
        }

        public static List<StyleBundle> GetRTLStyleBundles()
        {
            var lst = new List<StyleBundle>();

            foreach (var o in modRTLStyleList)
            {
                lst.AddRange(o.Value.Values);
            }
            return lst;
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
            if (order == int.MaxValue)
            {
                order--;
            }

            if (order < 0 && !prefix.Equals("VIS"))
            {
                order = 0;
            }

            if (prefix.Equals("ViennaAdvantage"))
            {
                order = int.MaxValue;
            }


            if (modScriptList.ContainsKey(order))
            {
                var lst = modScriptList[order];
                lst[prefix] = modScript;
            }
            else
            {
                var lst = new Dictionary<string, ScriptBundle>();
                lst[prefix] = modScript;
                modScriptList[order] = lst;

            }
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
            if (order == int.MaxValue)
            {
                order--;
            }
            if (order < 0 && !prefix.Equals("VIS"))
            {
                order = 0;
            }

            if (prefix.Equals("ViennaAdvantage"))
            {
                order = int.MaxValue;
            }
            if (modStyleList.ContainsKey(order))
            {
                var lst = modStyleList[order];
                lst[prefix] = styleBundle;
            }
            else
            {
                var lst = new Dictionary<string, StyleBundle>();
                lst[prefix] = styleBundle;
                modStyleList[order] = lst;
            }
        }

        public static void RegisterRTLStyleBundle(StyleBundle styleBundle, string prefix, int order)
        {
            if (order == int.MaxValue)
            {
                order--;
            }
            if (order < 0 && !prefix.Equals("VIS"))
            {
                order = 0;
            }

            if (prefix.Equals("ViennaAdvantage"))
            {
                order = int.MaxValue;
            }
            if (modRTLStyleList.ContainsKey(order))
            {
                var lst = modRTLStyleList[order];
                lst[prefix] = styleBundle;
            }
            else
            {
                var lst = new Dictionary<string, StyleBundle>();
                lst[prefix] = styleBundle;
                modRTLStyleList[order] = lst;
            }
        }
    }

    public class CustomBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Classes
{

    public class AttachmentHelper
    {

        private static Dictionary<string, List<byte[]>> FileCache = null;

        private static AttachmentHelper attHelper = null;

        private AttachmentHelper()
        {
            FileCache = new Dictionary<string, List<byte[]>>();
        }

        public static AttachmentHelper Get()
        {
            if (attHelper == null)
            {
                attHelper = new AttachmentHelper();
            }
            return attHelper;
        }


        public List<byte[]> GetValue(string key)
        {
            if (!FileCache.ContainsKey(key))
            {
                return null;
            }

            return FileCache[key];

        }

        public bool Set(string key, List<byte[]> value)
        {
            if (FileCache.ContainsKey(key))
            {
                return false;
            }
            FileCache.Add(key, value);
            return true;
        }

        public bool Add(string key, byte[] value)
        {

            if (!FileCache.ContainsKey(key))
            {
                FileCache.Add(key, new List<byte[]>());
            }
            FileCache[key].Add(value);
            return true;
        }

        public bool Remove(string key)
        {
            if (string.IsNullOrEmpty(key) || !FileCache.ContainsKey(key))
            {
                return true;
            }
            return FileCache.Remove(key);
        }

        public void Dispose()
        {
            FileCache.Clear();
            FileCache = null;
            attHelper = null;
        }
    }
}

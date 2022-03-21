using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using VAdvantage.DataBase;

namespace VAdvantage.Classes
{
    public class CleanUp : IDisposable
    {
        private static CleanUp _cleanUp = null;
        // private static bool isThreadRunning = false;
        Thread t = null;

        public static CleanUp Get()
        {
            if (_cleanUp == null)
            {
                _cleanUp = new CleanUp();
            }
            return _cleanUp;

        }

        private CleanUp()
        {
            initThread();

        }

        private void initThread()
        {
            t = new Thread(new ThreadStart(CleanUpFiles));
            t.IsBackground = true;
            t.Name = "CleanUpThread";
            t.Start();
        }


        public void Start()
        {
            if (t == null || !t.IsAlive)
            {
                initThread();
            }
        }

        private void CleanUpFiles()
        {
            // isThreadRunning = true;
            try
            {
                string tempPath = Path.Combine(GlobalVariable.PhysicalPath, "TempDownload");
                string[] files = Directory.GetFiles(tempPath);

                //delete files which are Older than 12 hours 
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastAccessTime < DateTime.Now.AddHours(-24))
                        fi.Delete();
                }
                files = null;
                //delete directories which are Older than 12 hours 
                string[] dirs = Directory.GetDirectories(tempPath);
                foreach (string dir in dirs)
                {
                    files = Directory.GetFiles(dir);
                    if (files != null && files.Length == 0)
                    {
                        Directory.Delete(dir, true);
                    }
                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(file);
                        if (fi.LastAccessTime < DateTime.Now.AddHours(-24))
                            fi.Delete();

                        files = null;
                        files = Directory.GetFiles(dir);
                        if (files != null && files.Length == 0)
                        {
                            Directory.Delete(dir, true);
                        }
                    }

                }
                Thread.Sleep(1000 * 60 * 60 * 1);
            }
            catch
            {

            }


            ///isThreadRunning = false;
        }

        public void Dispose()
        {
            if (t != null)
            {
                try
                {
                    t.Interrupt();
                    t = null;
                }
                catch
                {
                }
            }
        }
    }
}

/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Task
 * Purpose        : Execute OS Task
 * Class Used     : Thread
 * Chronological    Development
 * Deepak           29-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Logging;
using System.Threading;
using System.IO;
using System.Diagnostics;
namespace VAdvantage.Utility
{
    public class Task
    {
        /**
          *  Create Process with cmd
          *  @param cmd o/s command
          */
        public Task(String cmd)
        {
            _cmd = cmd;
        }   //  Task
      
        private String _cmd;
        private System.Diagnostics.Process _child = null;

        StringBuilder _out = new StringBuilder();
        StringBuilder _err = new StringBuilder();

        /** The Output Stream of process        */
        //InputStream     _outStream;
        StreamWriter _outStream;
        /** The Error Output Stream of process  */
        //InputStream     _errStream;
        StreamReader _errStream;
        /** The Input Stream of process         */
        StreamReader  _inStream;
        /**	Logger			*/
        static VLogger log = VLogger.GetVLogger(typeof(Task).FullName);

        /** Read Out                            */
        //private Thread _outReader = new Thread()
        //{
        //    public void Run()

        //       { 
        //           log.Fine("outReader");
        //           try
        //           { 
        //               int c;
        //               while ((c = _outStream.Read()) != -1 && !isInterrupted())
        //               {
        //                //System.out.print((char)c);
        //                   _out.Append((char)c);
        //               }
        //               _outStream.Close();
        //           }
        //           catch (IOException ioe)
        //           {
        //               log.Log(Level.SEVERE, "outReader", ioe);
        //           }
        //           log.Fine("outReader - done");
        //       }  
        //};

        //public void Run1()
        //{
        //    log.Fine("outReader");
        //    try
        //    { 
        //        int c;
        //        while ((c = _outStream.Read()) != -1 && ! _outReader.IsAlive)
        //        { 
                 
        //            //System.out.print((char)c);
        //            System.Console.WriteLine((char)c);
        //            _out.Append((char)c);
        //        }
        //        _outStream.Close();
        //    }
        //    catch (IOException ioe)
        //    {
        //        log.Log(Level.SEVERE, "outReader", ioe);
        //    }
        //    log.Fine("outReader - done");
        //}

        //public void Run2()
        //{
        //    log.Fine("outReader");
        //    try
        //    {
        //        int c;
        //        while ((c = _outStream .Read()) != -1 && !_outReader.IsAlive)
        //        {
        //            //System.out.print((char)c);
        //            System.Console.WriteLine((char)c);
        //            _out.Append((char)c);
        //        }
        //        _outStream.Close();
        //    }
        //    catch (IOException ioe)
        //    {
        //        log.Log(Level.SEVERE, "outReader", ioe);
        //    }
        //    log.Fine("outReader - done");
        //}

        /** Read Out                            */
        //private Thread m_errReader = new Thread()
        //{
        //    public void run()
        //    {
        //        log.Fine("errReader");
        //        try
        //        { 
        //            int c;
        //            while ((c = _errStream.read()) != -1 && !isInterrupted())
        //            {
        //        //		System.err.print((char)c);
        //                _err.append((char)c);
        //            }
        //            _errStream.close();
        //        }
        //        catch (IOException ioe)
        //        {
        //            log.log(Level.SEVERE, "errReader", ioe);
        //        }
        //        log.fine("errReader - done");
        //    }  
        //};
        private Thread _outReader = null;
        private Thread _errReader = null;
        private Thread _main = null;

        public void Start()
        {
            _main = new Thread(new ThreadStart(Run));
            _main.Start();
           

        }


        /**
         *  Execute it
         */
        public void Run()
        {
            log.Info(_cmd);
            try
            {
                _child =  System.Diagnostics.Process.Start(_cmd);

                _outStream = _child.StandardInput;
                 _inStream = _child.StandardOutput;
                
                
                ////_errStream = _child.getErrorStream();
                _errStream = _child.StandardError;
                ////_inStream = _child.getOutputStream();
                ////_inStream = _child.Threads;
                ////
                if (CheckInterrupted())
                {
                    return;
                }
                //_outReader.Start(new ThreadStart(Run1));
                //_errReader.Start(new ThreadStart(Run2));
                //
                try
                {
                    if (CheckInterrupted())
                    {
                        return;
                    }
                    _errReader.Join();
                    if (CheckInterrupted())
                    {
                        return;
                    }
                    _outReader.Join();
                    if (CheckInterrupted())
                    {
                        return;
                    }
                    // _child.waitFor();
                    _child.WaitForExit();
                }
                catch (ThreadInterruptedException ie)
                {
                    log.Log(Level.INFO, "(ie) - " + ie);
                }
                 
                try
                {
                    if (_child != null)
                    {
                        log.Fine("run - ExitValue=" + _child.ExitCode);//  .exitValue());
                    }
                }
                catch  { }
                log.Config("done");
            }
            catch (IOException ioe)
            {
                log.Log(Level.SEVERE, "(ioe)", ioe);
            }
        }   //  run

        /**
         *  Check if interrupted
         *  @return true if interrupted
         */
        private bool CheckInterrupted()
        {
            if (_outReader.IsAlive)// isInterrupted())
            {
                log.Config("interrupted");
                //  interrupt child processes
                if (_child != null)
                {
                    _child.Kill();// .destroy();
                }
                _child = null;
                if (_outReader != null && _outReader.IsAlive)
                {
                    _outReader.Interrupt();
                }
                _outReader = null;
                if (_errReader != null && _errReader.IsAlive)
                {
                    _errReader.Interrupt();
                }
                _errReader = null;
                //  close Streams
                if (_inStream != null)
                    try
                    {
                        _inStream.Close();
                    }
                    catch  { }
                _inStream = null;
                if (_outStream != null)
                    try
                    {
                        _outStream.Close(); 
                    }
                    catch  { }
                _outStream = null;
                if (_errStream != null)
                    try
                    {
                        _errStream.Close(); 
                    }
                    catch { }
                _errStream = null;
                //
                return true;
            }
            return false;
        }   //  checkInterrupted

        /**
         *  Get Out Info
         *  @return StringBuilder
         */
        public StringBuilder GetOut()
        {
            return _out;
        }   //  getOut

        /**
         *  Get Err Info
         *  @return StringBuilder
         */
        public StringBuilder GetErr()
        {
            return _err;
        }   //  getErr

        /**
         *  Get The process input stream - i.e. we output to it
         *  @return OutputStream
         */
        public StreamReader GetInStream()
        {
            return _inStream;
        }   //  getInStream
        public bool IsAlive()
        {
            return _outReader.IsAlive;
        }
    }   //  Task

}

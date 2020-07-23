/********************************************************
 * Module Name    : Workflow
 * Purpose        : 
 * Class Used     : ---
 * Chronological Development
 * Veena Pandey     02-May-2009
 ******************************************************/

using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;

using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
namespace VAdvantage.Process
{
    public class StateEngine
    {
        // To get context for culture message work
        private Ctx _ctx;
        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
        }

        /** Open - Not Started 	*/
	    public const String	STATE_NOTSTARTED = "ON";
	    /** Open - Running		*/
	    public const String	STATE_RUNNING 	= "OR";
	    /** Open - Suspended	*/
	    public const String	STATE_SUSPENDED = "OS";
	    /** Closed - Completed - normal exit	*/
	    public const String	STATE_COMPLETED = "CC";
	    /** Closed - Aborted - Environment/Setup Error	*/
	    public const String	STATE_ABORTED = "CA";
	    /** Closed - Teminated - Execution Error		*/
	    public const String	STATE_TERMINATED = "CT";

        /** Background - will run at background	*/
        public const String STATE_BACKGROUND = "BK";

	    /** Suspend			*/
	    public const String	ACTION_SUSPEND = "Suspend";
	    /** Start			*/
	    public const String	ACTION_START = "Start";
	    /** Resume			*/
	    public const String	ACTION_RESUME = "Resume";
	    /** Complete		*/
	    public const String	ACTION_COMPLETE = "Complete";
	    /** Abort			*/
	    public const String	ACTION_ABORT = "Abort";
	    /** Terminate		*/
	    public const String	ACTION_TERMINATE = "Terminate";

        /** Background		*/
        public const String ACTION_BACKGROUND = "Background";
    	
	    /**	Internal State				*/
	    private String _state = STATE_NOTSTARTED;

	    /** If true throw exceptions	*/
	    private bool _throwException	= false;
    	
	    /**	Logger			*/
	    //protected CLogger	log = null;

        /// <summary>
        /// Default Constructor (not started)
        /// </summary>
        public StateEngine()
            : this(STATE_NOTSTARTED)
        {
            //log = CLogger.getCLogger(getClass());
        }

        /// <summary>
        /// Initialized Constructor
        /// </summary>
        /// <param name="startState">start state</param>
        public StateEngine(String startState)
        {
            if (startState != null)
                _state = startState;
        }

        /// <summary>
        /// Are Exception Thrown
        /// </summary>
        /// <returns>trie if exceptions thrown</returns>
        public bool IsThrowException()
        {
            return _throwException;
        }

        /// <summary>
        /// Set if Exceptions are Thrown
        /// </summary>
        /// <param name="throwException"></param>
        public void SetThrowException(bool throwException)
        {
            _throwException = throwException;
        }

        /// <summary>
        /// Get State
        /// </summary>
        /// <returns>state</returns>
        public String GetState()
        {
            return _state;
        }

        /// <summary>
        /// Get clear text State Info.
        /// </summary>
        /// <returns>state info</returns>
        public String GetStateInfo()
        {
            String state = GetState();	//	is overwritten to update
            /**
            int AD_Reference_ID = 305;
            MRefList.getList(AD_Reference_ID, false);
            **/
            if (_ctx != null)
                return GetStateInfoCultured(state);
            if (STATE_RUNNING.Equals(state))
                return "Running";
            else if (STATE_NOTSTARTED.Equals(state))
                return "Not Started";
            else if (STATE_SUSPENDED.Equals(state))
                return "Suspended";
            else if (STATE_COMPLETED.Equals(state))
                return "Completed";
            else if (STATE_ABORTED.Equals(state))
                return "Aborted";
            else if (STATE_TERMINATED.Equals(state))
                return "Terminated";
            else if (STATE_BACKGROUND.Equals(state))
                return "Background";
            return state;
        }

        /// <summary>
        /// Get cultured text State Info.
        /// </summary>
        /// <returns>state info</returns>
        public String GetStateInfoCultured(string state)
        {
            if (STATE_RUNNING.Equals(state))
                return Msg.GetMsg(_ctx, "Running");
            else if (STATE_NOTSTARTED.Equals(state))
                return Msg.GetMsg(_ctx, "NotStarted");
            else if (STATE_SUSPENDED.Equals(state))
                return Msg.GetMsg(_ctx, "Suspended");
            else if (STATE_COMPLETED.Equals(state))
                return Msg.GetMsg(_ctx, "Completed");
            else if (STATE_ABORTED.Equals(state))
                return Msg.GetMsg(_ctx, "Aborted");
            else if (STATE_TERMINATED.Equals(state))
                return Msg.GetMsg(_ctx, "Terminated");
            else if (STATE_BACKGROUND.Equals(state))
                return Msg.GetMsg(_ctx, "Background");
            return state;
        }

        /// <summary>
        /// State is Open
        /// </summary>
        /// <returns>true if open (running, not started, suspended)</returns>
        public bool IsOpen()
        {
            return STATE_RUNNING.Equals(_state)
                || STATE_NOTSTARTED.Equals(_state)
                || STATE_SUSPENDED.Equals(_state);
        }

        /// <summary>
        /// State is Not Running
        /// </summary>
        /// <returns>true if not running (not started, suspended)</returns>
        public bool IsNotRunning()
        {
            return STATE_NOTSTARTED.Equals(_state)
                || STATE_SUSPENDED.Equals(_state);
        }

        /// <summary>
        /// State is Closed
        /// </summary>
        /// <returns>true if closed (completed, aborted, terminated)</returns>
        public bool IsClosed()
        {
            return STATE_COMPLETED.Equals(_state)
                || STATE_ABORTED.Equals(_state)
                || STATE_TERMINATED.Equals(_state);
        }

        /// <summary>
        /// State is Not Started
        /// </summary>
        /// <returns>true if Not Started</returns>
        public bool IsNotStarted()
        {
            return STATE_NOTSTARTED.Equals(_state);
        }

        /// <summary>
        /// State is Running
        /// </summary>
        /// <returns>true if Running</returns>
        public bool IsRunning()
        {
            return STATE_RUNNING.Equals(_state);
        }

        /// <summary>
        /// State is Suspended
        /// </summary>
        /// <returns>true if Suspended</returns>
        public bool IsSuspended()
        {
            return STATE_SUSPENDED.Equals(_state);
        }

        /// <summary>
        /// State is Completed
        /// </summary>
        /// <returns>true if Completed</returns>
        public bool IsCompleted()
        {
            return STATE_COMPLETED.Equals(_state);
        }

        /// <summary>
        /// State is Aborted (Environment/Setup issue)
        /// </summary>
        /// <returns>true if Aborted</returns>
        public bool IsAborted()
        {
            return STATE_ABORTED.Equals(_state);
        }

        /// <summary>
        /// State is Terminated (Execution issue)
        /// </summary>
        /// <returns>true if Terminated</returns>
        public bool IsTerminated()
        {
            return STATE_TERMINATED.Equals(_state);
        }

        /// <summary>
        /// State is Background
        /// </summary>
        /// <returns>true if Backgrounded</returns>
        public bool IsBackground()
        {
            return STATE_BACKGROUND.Equals(_state);
        }

        /// <summary>
        /// Start: not started -> running
        /// </summary>
        /// <returns>true if set to running</returns>
        public bool Start()
        {
            //if (log == null)
            //    log = CLogger.getCLogger(getClass());
            if (IsNotStarted())
            {
                _state = STATE_RUNNING;
                //log.info("starting ...");
                return true;
            }
            String msg = "start failed: Not Not Started (" + GetState() + ")";
            if (_throwException)
            {
                //throw new IllegalStateException(msg);
                throw new Exception(msg);
            }
            //log.warning(msg);
            return false;
        }

        /// <summary>
        /// Resume: suspended -> running
        /// </summary>
        /// <returns>true if set to sunning</returns>
        public bool Resume()	//	raises CannotResume, NotRunning, NotSuspended
        {
            //if (log == null)
            //    log = CLogger.getCLogger(getClass());
            if (IsSuspended())
            {
                _state = STATE_RUNNING;
                //log.info("resuming ...");
                return true;
            }
            String msg = "resume failed: Not Suspended (" + GetState() + ")";
            if (_throwException)
            {
                //throw new IllegalStateException(msg);
                throw new Exception(msg);
            }
            //log.warning(msg);
            return false;
        }

        /// <summary>
        /// Suspend: running -> suspended
        /// </summary>
        /// <returns>true if suspended</returns>
        public bool Suspend()	//	raises CannotSuspend, NotRunning, AlreadySuspended
        {
            //if (log == null)
            //    log = CLogger.getCLogger(getClass());
            if (IsRunning())
            {
                _state = STATE_SUSPENDED;
                //log.info("suspending ...");
                return true;
            }
            String msg = "suspend failed: Not Running (" + GetState() + ")";
            if (_throwException)
            {
                //throw new IllegalStateException(msg);
                throw new Exception(msg);
            }
            //log.warning(msg);
            return false;
        }

        /// <summary>
        /// Complete: running -> completed
        /// </summary>
        /// <returns>true if set to completed</returns>
        public bool Complete()
        {
            //if (log == null)
            //    log = CLogger.getCLogger(getClass());
            if (IsRunning())
            {
                _state = STATE_COMPLETED;
                //log.info("completing ...");
                return true;
            }
            String msg = "complete failed: Not Running (" + GetState() + ")";
            if (_throwException)
            {
                //throw new IllegalStateException(msg);
                throw new Exception(msg);
            }
            //log.warning(msg);
            return false;
        }

        /// <summary>
        /// Abort: open -> aborted
        /// </summary>
        /// <returns>true if set to aborted</returns>
        public bool Abort()	//	raises CannotStop, NotRunning
        {
            //if (log == null)
            //    log = CLogger.getCLogger(getClass());
            if (IsOpen())
            {
                _state = STATE_ABORTED;
                //log.info("aborting ...");
                return true;
            }
            String msg = "abort failed: Not Open (" + GetState() + ")";
            if (_throwException)
            {
                //throw new IllegalStateException(msg);
                throw new Exception(msg);
            }
            //log.warning(msg);
            return false;
        }

        /// <summary>
        /// Terminate: open -> terminated
        /// </summary>
        /// <returns>true if set to terminated</returns>
        public bool Terminate()	//	raises CannotStop, NotRunning
        {
            //if (log == null)
            //    log = CLogger.getCLogger(getClass());
            if (IsOpen())
            {
                _state = STATE_TERMINATED;
                //log.info("terminating ...");
                return true;
            }
            String msg = "terminate failed: Not Open (" + GetState() + ")";
            if (_throwException)
            {
                //throw new IllegalStateException(msg);
                throw new Exception(msg);
            }
            //log.warning(msg);
            return false;
        }

        /// <summary>
        /// Back: open -> background
        /// </summary>
        /// <returns>true if set to background</returns>
        public bool Background()	//	raises run background
        {
            if (IsOpen())
            {
                _state = STATE_BACKGROUND;
                return true;
            }
            String msg = "background failed: Not background (" + GetState() + ")";
            if (_throwException)
            {
                throw new Exception(msg);
            }
            return false;
        }

        /// <summary>
        /// Get New State Options based on current State
        /// </summary>
        /// <returns>array of new states</returns>
        public String[] GetNewStateOptions()
        {
            if (IsNotStarted())
                return new String[] { STATE_RUNNING, STATE_ABORTED, STATE_TERMINATED };
            if (IsRunning())
                return new String[] { STATE_SUSPENDED, STATE_COMPLETED, STATE_ABORTED, STATE_TERMINATED };
            if (IsSuspended())
                return new String[] { STATE_RUNNING, STATE_ABORTED, STATE_TERMINATED };
            if (IsBackground())
                return new String[] { STATE_BACKGROUND };
            //
            return new String[] { };
        }

        /// <summary>
        /// Is the new State valid based on current state
        /// </summary>
        /// <param name="newState">new state</param>
        /// <returns>true valid new state</returns>
        public bool IsValidNewState(String newState)
        {
            String[] options = GetNewStateOptions();
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i].Equals(newState))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Set State to new State
        /// </summary>
        /// <param name="newState">new state</param>
        /// <returns>true if set to new state</returns>
        public bool SetState(String newState)	//	raises InvalidState, TransitionNotAllowed
        {
            if (STATE_RUNNING.Equals(newState))
            {
                if (IsNotStarted())
                    return Start();
                else
                    return Resume();
            }
            else if (STATE_SUSPENDED.Equals(newState))
                return Suspend();
            else if (STATE_COMPLETED.Equals(newState))
                return Complete();
            else if (STATE_ABORTED.Equals(newState))
                return Abort();
            else if (STATE_TERMINATED.Equals(newState))
                return Terminate();
            else if (STATE_BACKGROUND.Equals(newState))
                return Background();
            return false;
        }

        /// <summary>
        /// Get Action Options based on current State
        /// </summary>
        /// <returns>array of actions</returns>
        public String[] GetActionOptions()
        {
            if (IsNotStarted())
                return new String[] { ACTION_START, ACTION_ABORT, ACTION_TERMINATE };
            if (IsRunning())
                return new String[] { ACTION_SUSPEND, ACTION_COMPLETE, ACTION_ABORT, ACTION_TERMINATE };
            if (IsSuspended())
                return new String[] { ACTION_RESUME, ACTION_ABORT, ACTION_TERMINATE };
            if (IsBackground())
                return new String[] { ACTION_BACKGROUND };
            //
            return new String[] { };
        }

        /// <summary>
        /// Is The Action Valid based on current state
        /// </summary>
        /// <param name="action">action</param>
        /// <returns>true if valid</returns>
        public bool IsValidAction(String action)
        {
            String[] options = GetActionOptions();
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i].Equals(action))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="action">action</param>
        /// <returns>true if set to new state</returns>
        public bool Process(String action)	//	raises InvalidState, TransitionNotAllowed
        {
            if (ACTION_START.Equals(action))
                return Start();
            else if (ACTION_COMPLETE.Equals(action))
                return Complete();
            else if (ACTION_SUSPEND.Equals(action))
                return Suspend();
            else if (ACTION_RESUME.Equals(action))
                return Resume();
            else if (ACTION_ABORT.Equals(action))
                return Abort();
            else if (ACTION_TERMINATE.Equals(action))
                return Terminate();
            else if (ACTION_BACKGROUND.Equals(action))
                return Background();
            return false;
        }

        /// <summary>
        /// Get New State If Action performed
        /// </summary>
        /// <param name="action">action</param>
        /// <returns>potential new state</returns>
        public String GetNewStateIfAction(String action)
        {
            if (IsValidAction(action))
            {
                if (ACTION_START.Equals(action))
                    return STATE_RUNNING;
                else if (ACTION_COMPLETE.Equals(action))
                    return STATE_COMPLETED;
                else if (ACTION_SUSPEND.Equals(action))
                    return STATE_SUSPENDED;
                else if (ACTION_RESUME.Equals(action))
                    return STATE_RUNNING;
                else if (ACTION_ABORT.Equals(action))
                    return STATE_ABORTED;
                else if (ACTION_TERMINATE.Equals(action))
                    return STATE_TERMINATED;
                else if (ACTION_BACKGROUND.Equals(action))
                    return STATE_BACKGROUND;
            }
            //	Unchanged
            return GetState();
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            return GetStateInfo();
        }

    }
}

/********************************************************
 * Module Name    : VAdvantage
 * Purpose        : Background thread initiated at login
 * Class Used     : Interface IThreadObservable
 * Chronological Development
 * VIS0008   20-Dec-2021
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Interface;
using VAdvantage.Logging;

namespace VAdvantage.Classes
{
    public class PeriodicJob : IThreadObservable
    {
        #region Private Variables
        private static List<IThreadObserver> _observersList = new List<IThreadObserver>();
        private static PeriodicJob _periodJob = null;
        #endregion Private Variables

        /// <summary>
        /// Function to notify all registered observers
        /// </summary>
        public void Notify()
        {
            foreach (IThreadObserver o in _observersList)
            {
                o.Update(this);
            }
        }

        /// <summary>
        /// Function to Register observers to the list
        /// </summary>
        /// <param name="itho">Thread Observer to be registered for notification</param>
        public void Register(IThreadObserver itho)
        {
            VLogger.Get().Log(Level.INFO, "Registered Thread : " + itho.GetName());
            _observersList.Add(itho);
        }

        /// <summary>
        /// Function to UnRegister observers to the list
        /// </summary>
        /// <param name="itho">Thread Observer to be unregistered from notification</param>
        public void UnRegister(IThreadObserver itho)
        {
            _observersList.Remove(itho);
        }

        /// <summary>
        /// Singleton object of Periodic Job
        /// </summary>
        /// <returns>Object of Periodic Job</returns>
        public static PeriodicJob Get()
        {
            if (_periodJob == null)
                _periodJob = new PeriodicJob();
            return _periodJob;
        }
    }
}

/********************************************************
 * Project Name   : VAdvantage
 * Interface Name : ThreadInterface
 * Purpose        : Interface to be implemented by classes which will be called after specified time
 * Class Used     :  ..
 * Chronological    Development
 * VIS0008   20-Dec-2021
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdvantage.Interface
{
    /// <summary>
    /// Observable interface to register/unregister/notify observers
    /// </summary>
    public interface IThreadObservable
    {
        void Register(IThreadObserver itho);
        void UnRegister(IThreadObserver itho);
        void Notify();
    }

    /// <summary>
    /// Observer interface for observers
    /// </summary>
    public interface IThreadObserver
    {
        void Update(IThreadObservable obsvl);

        string GetName();
    }

}

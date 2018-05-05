using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.ViewModel
{
    public abstract class BaseViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        protected Logger Log { get; private set; }

        public BaseViewModel()
        {
            Log = LogManager.GetLogger(this.GetType().ToString());
        }

    }
}

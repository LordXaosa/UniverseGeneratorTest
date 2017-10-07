using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniverseGeneratorTestWpf.ViewModels
{
    public class UserWaitableViewModel: NotifyPropertyChanged
    {
        private bool _isInProgress;
        public bool IsInProgress
        {
            get { return _isInProgress; }
            set
            {
                _isInProgress = value;
                RaisePropertyChanged();
            }
        }
    }
}

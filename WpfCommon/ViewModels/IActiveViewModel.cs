using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCommon.ViewModels
{
    public interface IActiveViewModel
    {
        void OnProgress(bool isBusy);
    }
}

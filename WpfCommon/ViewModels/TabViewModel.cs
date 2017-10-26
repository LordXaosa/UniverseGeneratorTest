using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCommon.ViewModels
{
    public class TabViewModel : ActiveViewModel, IActiveViewModel
    {
        public TabViewModel(IActiveViewModel parentViewModel):base(parentViewModel)
        {

        }
    }
}

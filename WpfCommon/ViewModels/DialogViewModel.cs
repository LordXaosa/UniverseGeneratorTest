using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfCommon.Helpers;

namespace WpfCommon.ViewModels
{
    public class DialogViewModel : NotifyPropertyChanged
    {
        private bool? dialogResult;
        public bool? DialogResult
        {
            get
            {
                return dialogResult;
            }
            set
            {
                dialogResult = value;
                RaisePropertyChanged();
            }
        }

        public ICommand DialogResultOKCommand { get; set; }
        public ICommand DialogResultCancelCommand { get; set; }

        public DialogViewModel()
        {
            DialogResultOKCommand = new Command(DialogResultOK);
            DialogResultCancelCommand = new Command(DialogResultCancel);
        }

        protected void DialogResultOK()
        {
            DialogResult = true;
        }

        protected void DialogResultCancel()
        {
            DialogResult = false;
        }
    }
}

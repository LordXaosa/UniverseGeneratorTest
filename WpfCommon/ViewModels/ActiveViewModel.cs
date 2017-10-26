using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCommon.ViewModels
{
    public class ActiveViewModel : DialogViewModel, IActiveViewModel
    {
        private bool isEnabled;
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
                RaisePropertyChanged();
            }
        }
        private string status;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                RaisePropertyChanged();
            }
        }
        private bool isBusy;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                isBusy = value;
                RaisePropertyChanged();
            }
        }

        public IActiveViewModel parentModel;
        public IActiveViewModel ParentModel
        {
            get
            {
                return parentModel;
            }
            set
            {
                parentModel = value;
                RaisePropertyChanged();
            }
        }



        public ActiveViewModel() : this(null) { }

        public ActiveViewModel(IActiveViewModel parentModel)
        {
            this.ParentModel = parentModel;
        }

        public void OnProgress(bool isBusy)
        {
            if (ParentModel != null)
            {
                ParentModel.OnProgress(isBusy);
            }
            else
            {
                IsBusy = isBusy;
            }
        }
        public async Task OnProgress(Task act)
        {
            OnProgress(true);
            await act;
            OnProgress(false);
        }
    }
}

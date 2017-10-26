using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCommon.ViewModels;

namespace Editor.ViewModels
{
    public class MainViewModel:ActiveViewModel
    {
        public ItemsEditorViewModel ItemsViewModel { get; set; }
        public MainViewModel()
        {
            ItemsViewModel = new ItemsEditorViewModel(this);
        }
    }
}

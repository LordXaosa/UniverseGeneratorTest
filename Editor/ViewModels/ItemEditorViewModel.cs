using Common.Models;
using Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfCommon.Helpers;
using WpfCommon.ViewModels;

namespace Editor.ViewModels
{
    public class ItemEditorViewModel : ActiveViewModel
    {
        private ItemModel _item;
        public ItemModel Item
        {
            get => _item;
            set
            {
                _item = value;
                RaisePropertyChanged();
            }
        }

        private ItemModel _itemToSelect;
        public ItemModel ItemToSelect
        {
            get => _itemToSelect;
            set
            {
                _itemToSelect = value;
                RaisePropertyChanged();
            }
        }

        private DictWrapper<ItemModel, decimal> _itemToUnselect;
        public DictWrapper<ItemModel, decimal> ItemToUnselect
        {
            get => _itemToUnselect;
            set
            {
                _itemToUnselect = value;
                RaisePropertyChanged();
            }
        }
        private List<ItemModel> _unselectedItems;
        public List<ItemModel> UnselectedItems
        {
            get => _unselectedItems;
            set
            {
                _unselectedItems = value;
                RaisePropertyChanged();
            }
        }

        private List<DictWrapper<ItemModel, decimal>> _selectedItems;
        public List<DictWrapper<ItemModel, decimal>> SelectedItems
        {
            get => _selectedItems;
            set
            {
                _selectedItems = value;
                RaisePropertyChanged();
            }
        }

        public ICommand SelectItem { get; set; }
        public ICommand UnselectItem { get; set; }
        private ItemEditorViewModel(List<ItemModel> items)
        {
            SelectItem = new Command(SelectItemCmd);
            UnselectItem = new Command(UnselectItemCmd);
            UnselectedItems = new List<ItemModel>(items);
        }
        public ItemEditorViewModel(List<ItemModel> items, int newId) : this(items)
        {
            Item = new ItemModel();
            Item.Id = newId;
            SelectedItems = new List<DictWrapper<ItemModel, decimal>>();
        }
        public ItemEditorViewModel(List<ItemModel> items, ItemModel item) : this(items)
        {
            Item = item;
            SelectedItems = new List<DictWrapper<ItemModel, decimal>>();
            foreach (var i in item.Resources)
            {
                SelectedItems.Add(new DictWrapper<ItemModel, decimal>(i.Key, i.Value));
            }
        }

        private void SelectItemCmd()
        {
            if (ItemToSelect != null)
            {
                if (!SelectedItems.Any(p=>p.Key==ItemToSelect))
                {
                    SelectedItems.Add(new DictWrapper<ItemModel, decimal>(ItemToSelect, 0));
                    SelectedItems = new List<DictWrapper<ItemModel, decimal>>(SelectedItems);
                }
            }
        }

        private void UnselectItemCmd()
        {
            if (ItemToUnselect != null)
            {
                SelectedItems.Remove(ItemToUnselect);
                SelectedItems = new List<DictWrapper<ItemModel, decimal>>(SelectedItems);
            }
        }
    }
}

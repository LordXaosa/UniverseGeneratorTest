using Common.Models;
using Editor.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfCommon.Helpers;
using WpfCommon.ViewModels;

namespace Editor.ViewModels
{
    public class ItemsEditorViewModel : TabViewModel
    {
        public ICommand LoadData { get; set; }
        public ICommand SaveData { get; set; }
        public ICommand AddItem { get; set; }
        public ICommand DeleteItem { get; set; }
        public ICommand EditItem { get; set; }

        public int NextId { get
            {
                if(ItemsList!=null && ItemsList.Count>0)
                    return ItemsList.Max(p => p.Id) + 1;

                return 1;
            }
        }

        private ItemModel _selectedItem;
        public ItemModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                RaisePropertyChanged();
            }
        }

        private List<ItemModel> _itemsList;
        public List<ItemModel> ItemsList
        {
            get => _itemsList;
            set
            {
                _itemsList = value;
                RaisePropertyChanged();
            }
        }
        private Dictionary<int, ItemModel> itemsDict;
        public ItemsEditorViewModel(IActiveViewModel parent) : base(parent)
        {
            ItemsList = new List<ItemModel>();
            LoadData = new Command(LoadDataCmd);
            SaveData = new Command(SaveDataCmd);
            AddItem = new Command(AddItemCmd);
            DeleteItem = new Command(DeleteItemCmd);
            EditItem = new Command(EditItemCmd);
        }
        private async void LoadDataCmd()
        {
            await OnProgress(Task.Factory.StartNew(() =>
                {
                    using (BinaryReader br = new BinaryReader(File.Open("Items.dat", FileMode.Open)))
                    {
                        itemsDict = new Dictionary<int, ItemModel>();
                        List<ItemModel> items = new List<ItemModel>();
                        int count = br.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            ItemModel item = new ItemModel(br);
                            items.Add(item);
                            itemsDict.Add(item.Id, item);
                        }
                        foreach (var item in items)
                        {
                            item.SetLinks(itemsDict);
                        }
                        ItemsList = items;
                    }

                })
            );
        }

        private async void SaveDataCmd()
        {
            await OnProgress(Task.Factory.StartNew(() =>
            {
                using (BinaryWriter bw = new BinaryWriter(File.Open("Items.dat", FileMode.Create)))
                {
                    bw.Write(ItemsList.Count);
                    foreach (var item in ItemsList)
                    {
                        item.WriteBinary(bw);
                    }
                }
            })
            );
        }

        private void AddItemCmd()
        {
            ItemEditorViewModel vm = new ItemEditorViewModel(ItemsList, NextId);
            ItemEditorWindow window = new ItemEditorWindow();
            window.DataContext = vm;
            if (window.ShowDialog() == true)
            {
                SetResources(vm);
                ItemsList.Add(vm.Item);
                ItemsList = new List<ItemModel>(ItemsList);
            }
        }
        private void DeleteItemCmd()
        {
            if(SelectedItem!=null)
            {
                ItemsList.Remove(SelectedItem);
                ItemsList = new List<ItemModel>(ItemsList);
            }
        }
        private void EditItemCmd()
        {
            ItemEditorViewModel vm = new ItemEditorViewModel(ItemsList, SelectedItem);
            ItemEditorWindow window = new ItemEditorWindow();
            window.DataContext = vm;
            window.ShowDialog();
            SetResources(vm);
            ItemsList = new List<ItemModel>(ItemsList);
        }

        void SetResources(ItemEditorViewModel vm)
        {
            vm.Item.Resources = new Dictionary<ItemModel, decimal>();
            foreach(var item in vm.SelectedItems)
            {
                vm.Item.Resources.Add(item.Key, item.Value);
            }
        }
    }
}

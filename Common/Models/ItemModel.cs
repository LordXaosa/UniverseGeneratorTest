using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class ItemModel : ISerializableModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal TotalCost { get; set; }
        public decimal CountCost
        {
            get
            {
                decimal temp = 0;
                foreach (var item in Resources)
                {
                    if (item.Key.TotalCost == 0)
                    {
                        temp += item.Key.CountCost * item.Value;
                    }
                    else
                    {
                        temp += item.Key.TotalCost * item.Value;
                    }
                }
                return temp;
            }
        }
        public Dictionary<ItemModel, decimal> Resources { get; set; }
        private Dictionary<int, decimal> _resourcesById;

        public ItemModel()
        {
            Resources = new Dictionary<ItemModel, decimal>();
        }
        public ItemModel(BinaryReader br)
        {
            ReadBinary(br);
        }

        public void WriteBinary(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(Name);
            bw.Write(TotalCost);
            bw.Write(Resources.Count);
            foreach (var item in Resources)
            {
                bw.Write(item.Key.Id);
                bw.Write(item.Value);
            }
        }

        public void ReadBinary(BinaryReader br)
        {
            Id = br.ReadInt32();
            Name = br.ReadString();
            TotalCost = br.ReadDecimal();
            int count = br.ReadInt32();
            _resourcesById = new Dictionary<int, decimal>();
            for (int i = 0; i < count; i++)
            {
                _resourcesById.Add(br.ReadInt32(), br.ReadDecimal());
            }
        }

        public void SetLinks(Dictionary<int, ItemModel> items)
        {
            Resources = new Dictionary<ItemModel, decimal>();
            foreach (var item in _resourcesById)
            {
                Resources.Add(items[item.Key], item.Value);
            }
        }
    }
}

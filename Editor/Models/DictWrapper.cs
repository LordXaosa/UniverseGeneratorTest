using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Models
{
    public class DictWrapper<TKey, TValue> : NotifyPropertyChanged
    {
        private TKey _key;
        public TKey Key
        {
            get => _key;
            set
            {
                _key = value;
                RaisePropertyChanged();
            }
        }
        private TValue _value;
        public TValue Value
        {
            get => _value;
            set
            {
                _value = value;
                RaisePropertyChanged();
            }
        }
        public DictWrapper(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}

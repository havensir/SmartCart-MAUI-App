using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartCart.Models
{
    public class GroceryItem : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _category;
        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        public int ListId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}

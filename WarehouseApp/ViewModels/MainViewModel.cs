using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WarehouseApp.Models; 

namespace WarehouseApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _nextId = 1;

        public ObservableCollection<Product> Products { get; set; }

        private Product? _selectedProduct;
        public Product? SelectedProduct
        {
            get { return _selectedProduct; }
            set { 
                _selectedProduct = value;
                OnPropertyChanged();

                if (_selectedProduct != null)
                {
                    NewProductName = _selectedProduct.Name;
                    NewProductPrice = _selectedProduct.Price.ToString();
                    NewProductQuantity = _selectedProduct.Quantity.ToString();
                }
            }
        }

        private string _newProductName = String.Empty;
        public string NewProductName
        {
            get { return _newProductName; }
            set { _newProductName = value; OnPropertyChanged(); }
        }

        private string _newProductPrice = String.Empty;
        public string NewProductPrice
        {
            get { return _newProductPrice; }
            set { _newProductPrice = value; OnPropertyChanged(); }
        }

        private string _newProductQuantity = "1";
        public string NewProductQuantity
        {
            get { return _newProductQuantity; }
            set { _newProductQuantity = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand ClearCommand { get; set; } 

        public MainViewModel()
        {
            Products = new ObservableCollection<Product>();

            AddCommand = new RelayCommand(AddProduct);
            DeleteCommand = new RelayCommand(DeleteProduct, CanDelete);
            UpdateCommand = new RelayCommand(UpdateProduct, CanUpdate); 
            ClearCommand = new RelayCommand(ClearInputs, CanClear);
        }

        private void AddProduct(object? obj)
        {
            if (string.IsNullOrWhiteSpace(NewProductName))
            {
                MessageBox.Show("Ошибка: Введите название товара!", "Валидация");
                return;
            }

            string priceString = NewProductPrice.Replace('.', ',');
            if (!decimal.TryParse(priceString, out decimal parsedPrice))
            {
                MessageBox.Show("Ошибка: Введите корректную цену (например, 10,50)!", "Валидация");
                return;
            }

            if (!int.TryParse(NewProductQuantity, out int parsedQuantity) || parsedQuantity <= 0)
            {
                parsedQuantity = 1;
            }

            var newProduct = new Product
            {
                Id = _nextId++,
                Name = NewProductName,
                Price = parsedPrice,
                Quantity = parsedQuantity 
            };

            Products.Add(newProduct);

            NewProductName = "";
            NewProductPrice = "";
            NewProductQuantity = "1";
        }

        private void DeleteProduct(object? obj)
        {
            Products.Remove(SelectedProduct!);
        }

        private bool CanDelete(object? obj)
        {
            return SelectedProduct != null;
        }

        private void UpdateProduct(object? obj)
        {
            if (string.IsNullOrWhiteSpace(NewProductName))
            {
                MessageBox.Show("Ошибка: Введите название товара!", "Валидация");
                return;
            }

            string priceString = NewProductPrice.Replace('.', ',');
            if (!decimal.TryParse(priceString, out decimal parsedPrice))
            {
                MessageBox.Show("Ошибка: Введите корректную цену!", "Валидация");
                return;
            }

            if (!int.TryParse(NewProductQuantity, out int parsedQuantity) || parsedQuantity < 0)
            {
                parsedQuantity = 0;
            }

            SelectedProduct!.Name = NewProductName;
            SelectedProduct.Price = parsedPrice;
            SelectedProduct.Quantity = parsedQuantity;

            ClearInputs(null);
        }

        private bool CanUpdate(object? obj)
        {
            return SelectedProduct != null;
        }

        private void ClearInputs(object? obj)
        {
            NewProductName = "";
            NewProductPrice = "";
            NewProductQuantity = "1"; 
            SelectedProduct = null;  
        }

        private bool CanClear(object? obj)
        {
            return !string.IsNullOrWhiteSpace(NewProductName)
                || !string.IsNullOrWhiteSpace(NewProductPrice)
                || SelectedProduct != null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
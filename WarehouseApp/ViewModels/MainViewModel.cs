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
        private const decimal MaxPrice = 1_000_000m;
        private const int MaxQuantity = 1_000_000;

        private int _nextId = 1;

        public ObservableCollection<Product> Products { get; set; }

        private Product? _selectedProduct;
        public Product? SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
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
        public ICommand HelpCommand { get; set; }

        public MainViewModel()
        {
            Products = new ObservableCollection<Product>();

            AddCommand = new RelayCommand(AddProduct);
            DeleteCommand = new RelayCommand(DeleteProduct, CanDelete);
            UpdateCommand = new RelayCommand(UpdateProduct, CanUpdate);
            ClearCommand = new RelayCommand(ClearInputs);
            HelpCommand = new RelayCommand(ShowHelp);
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

            if (parsedPrice <= 0 || parsedPrice > MaxPrice)
            {
                MessageBox.Show($"Цена должна быть от 0 до {MaxPrice:N0} руб.", "Валидация");
                return;
            }

            if (!int.TryParse(NewProductQuantity, out int parsedQuantity))
            {
                MessageBox.Show("Ошибка: Введите корректное количество!", "Валидация");
                return;
            }

            if (parsedQuantity <= 0 || parsedQuantity > MaxQuantity)
            {
                MessageBox.Show($"Количество должно быть от 1 до {MaxQuantity:N0} шт.", "Валидация");
                return;
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
            if (SelectedProduct != null)
            {
                Products.Remove(SelectedProduct);
            }
        }

        private bool CanDelete(object? obj)
        {
            return SelectedProduct != null;
        }

        private void UpdateProduct(object? obj)
        {
            if (SelectedProduct == null)
            {
                return;
            }

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

            if (parsedPrice <= 0 || parsedPrice > MaxPrice)
            {
                MessageBox.Show($"Цена должна быть от 0 до {MaxPrice:N0} руб.", "Валидация");
                return;
            }

            if (!int.TryParse(NewProductQuantity, out int parsedQuantity))
            {
                MessageBox.Show("Ошибка: Введите корректное количество!", "Валидация");
                return;
            }

            if (parsedQuantity < 0 || parsedQuantity > MaxQuantity)
            {
                MessageBox.Show($"Количество должно быть от 0 до {MaxQuantity:N0} шт.", "Валидация");
                return;
            }

            SelectedProduct.Name = NewProductName;
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

        private void ShowHelp(object? obj)
        {
            MessageBox.Show(
                "Правила ввода данных:\n\n" +
                "• Название: не пустое\n" +
                $"• Цена: от 0 до {MaxPrice:N0} руб.\n" +
                $"• Количество: от 0 до {MaxQuantity:N0} шт.\n\n" +
                "Эти ограничения защищают программу от переполнения при подсчёте суммы.",
                "Справка");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
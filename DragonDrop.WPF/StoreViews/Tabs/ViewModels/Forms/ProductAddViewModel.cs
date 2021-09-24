using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using DragonDrop.Infrastructure;
using DragonDrop.Infrastructure.Helpers;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class ProductAddViewModel : BindableBase
    {
        private IRelayReloadAndRemoteCloseControl _callingView;
        private bool _asyncCalls = bool.Parse(ConfigurationManager.AppSettings["UseAsyncCalls"]);

        #region Fields

        private IProductDataService _service;

        private string _defError = "This should not be visible";
        
        #region Name

        private string _nameEntryText;
        private string _nameErrorText;
        private Visibility _nameErrorVisibility;

        #endregion

        #region Price

        private string _priceEntryText;
        private string _priceErrorText;
        private Visibility _priceErrorVisibility;

        #endregion

        #region Stock

        private string _stockEntryText;
        private string _stockErrorText;
        private Visibility _stockErrorVisibility;

        #endregion

        #region BarCode

        private string _codeEntryText;
        private string _codeErrorText;
        private Visibility _codeErrorVisibility;

        #endregion

        #region Manufacturer

        private string _manufacturerText;

        #endregion

        #region Description

        private string _descEntryText;
        private string _descErrorText;
        private Visibility _descErrorVisibility;

        private int _remainingDesc;
        private Brush _descCounterFg;

        #endregion

        #endregion

        public ProductAddViewModel(IProductDataService service, ICategoryRepository catRepo,
            IRelayReloadAndRemoteCloseControl callingView=null)
        {
            _callingView = callingView;
            _service = service;

            #region Commands

            SubmitCommand = _asyncCalls ? new DelegateCommand(SubmitCommandExecuteAsync)
                : new DelegateCommand(SubmitCommandExecute);

            #region Name

            NameBoxGotFocusCommand = new DelegateCommand(NameBoxGotFocusCommandExecute);
            NameBoxLostFocusCommand = new DelegateCommand(NameBoxLostFocusCommandExecute);
            NameBoxTextChangedCommand = new DelegateCommand(NameBoxTextChangedCommandExecute);

            #endregion

            #region Price

            PriceBoxGotFocusCommand = new DelegateCommand(PriceBoxGotFocusCommandExecute);
            PriceBoxLostFocusCommand = new DelegateCommand(PriceBoxLostFocusCommandExecute);
            PriceBoxTextChangedCommand = new DelegateCommand(PriceBoxTextChangedCommandExecute);

            #endregion

            #region Stock

            StockBoxGotFocusCommand = new DelegateCommand(StockBoxGotFocusCommandExecute);
            StockBoxLostFocusCommand = new DelegateCommand(StockBoxLostFocusCommandExecute);
            StockBoxTextChangedCommand = new DelegateCommand(StockBoxTextChangedCommandExecute);

            #endregion

            #region BarCode

            CodeBoxGotFocusCommand = new DelegateCommand(CodeBoxGotFocusCommandExecute);
            CodeBoxLostFocusCommand = new DelegateCommand(CodeBoxLostFocusCommandExecute);
            CodeBoxTextChangedCommand = 
                new DelegateCommand<TextChangedEventArgs>(CodeBoxTextChangedCommandExecute);

            #endregion

            #region Description

            DescBoxGotFocusCommand = new DelegateCommand(DescBoxGotFocusCommandExecute);
            DescBoxLostFocusCommand = new DelegateCommand(DescBoxLostFocusCommandExecute);
            DescBoxTextChangedCommand = new DelegateCommand(DescBoxTextChangedCommandExecute);

            #endregion

            #endregion

            #region Init

            Categories = new ObservableCollection<Category>(catRepo.GetAll());

            #region Name

            NameEntryText = "Name";
            NameErrorText = _defError;
            NameErrorVisibility = Visibility.Collapsed;

            #endregion

            #region Price

            PriceEntryText = "Price";
            PriceErrorText = _defError;
            PriceErrorVisibility = Visibility.Collapsed;

            #endregion

            #region Stock

            StockEntryText = "Stock";
            StockErrorText = _defError;
            StockErrorVisibility = Visibility.Collapsed;

            #endregion

            #region BarCode

            CodeEntryText = "Barcode";
            CodeErrorText = _defError;
            CodeErrorVisibility = Visibility.Collapsed;

            #endregion

            _manufacturerText = "Manufacturer";

            #region Description

            DescEntryText = "Description";
            DescErrorText = _defError;
            DescErrorVisibility = Visibility.Collapsed;

            RemainingDesc = 0;
            DescCounterFg = Brushes.LightGray;

            #endregion

            #endregion
        }

        #region Properties

        #region ViewModelProps

        public ObservableCollection<Category> Categories { get; }

        #region NameEntry

        public string NameEntryText
        {
            get => _nameEntryText;
            set => SetProperty(ref _nameEntryText, value);
        }

        public string NameErrorText
        {
            get => _nameErrorText;
            set => SetProperty(ref _nameErrorText, value);
        }

        public Visibility NameErrorVisibility
        {
            get => _nameErrorVisibility;
            set => SetProperty(ref _nameErrorVisibility, value);
        }

        #endregion

        public Category SelectedCategory { get; set; }

        #region PriceEntry

        public string PriceEntryText
        {
            get => _priceEntryText;
            set => SetProperty(ref _priceEntryText, value);
        }

        public string PriceErrorText
        {
            get => _priceErrorText;
            set => SetProperty(ref _priceErrorText, value);
        }

        public Visibility PriceErrorVisibility
        {
            get => _priceErrorVisibility;
            set => SetProperty(ref _priceErrorVisibility, value);
        }

        #endregion

        #region StockEntry

        public string StockEntryText
        {
            get => _stockEntryText;
            set => SetProperty(ref _stockEntryText, value);
        }

        public string StockErrorText
        {
            get => _stockErrorText;
            set => SetProperty(ref _stockErrorText, value);
        }

        public Visibility StockErrorVisibility
        {
            get => _stockErrorVisibility;
            set => SetProperty(ref _stockErrorVisibility, value);
        }

        #endregion

        #region BarCodeEntry

        public string CodeEntryText
        {
            get => _codeEntryText;
            set => SetProperty(ref _codeEntryText, value);
        }

        public string CodeErrorText
        {
            get => _codeErrorText;
            set => SetProperty(ref _codeErrorText, value);
        }

        public Visibility CodeErrorVisibility
        {
            get => _codeErrorVisibility;
            set => SetProperty(ref _codeErrorVisibility, value);
        }

        #endregion

        public string ManufacturerText
        {
            get => _manufacturerText;
            set => SetProperty(ref _manufacturerText, value);
        }

        #region DescriptionEntry

        public int RemainingDesc
        {
            get => _remainingDesc;
            set => SetProperty(ref _remainingDesc, value);
        }

        public Brush DescCounterFg
        {
            get => _descCounterFg;
            set => SetProperty(ref _descCounterFg, value);
        }

        public string DescEntryText
        {
            get => _descEntryText;
            set => SetProperty(ref _descEntryText, value);
        }

        public string DescErrorText
        {
            get => _descErrorText;
            set => SetProperty(ref _descErrorText, value);
        }

        public Visibility DescErrorVisibility
        {
            get => _descErrorVisibility;
            set => SetProperty(ref _descErrorVisibility, value);
        }

        #endregion

        #endregion

        #region Commands

        public DelegateCommand SubmitCommand { get; }

        #region Name

        public DelegateCommand NameBoxGotFocusCommand { get; }
        public DelegateCommand NameBoxLostFocusCommand { get; }
        public DelegateCommand NameBoxTextChangedCommand { get; }

        #endregion

        #region Price

        public DelegateCommand PriceBoxGotFocusCommand { get; }
        public DelegateCommand PriceBoxLostFocusCommand { get; }
        public DelegateCommand PriceBoxTextChangedCommand { get; }

        #endregion

        #region Stock

        public DelegateCommand StockBoxGotFocusCommand { get; }
        public DelegateCommand StockBoxLostFocusCommand { get; }
        public DelegateCommand StockBoxTextChangedCommand { get; }

        #endregion

        #region BarCode

        public DelegateCommand CodeBoxGotFocusCommand { get; }
        public DelegateCommand CodeBoxLostFocusCommand { get; }
        public DelegateCommand<TextChangedEventArgs> CodeBoxTextChangedCommand { get; }

        #endregion

        #region Description

        public DelegateCommand DescBoxGotFocusCommand { get; }
        public DelegateCommand DescBoxLostFocusCommand { get; }
        public DelegateCommand DescBoxTextChangedCommand { get; }

        #endregion

        #endregion

        #endregion

        #region Commands

        private async void SubmitCommandExecuteAsync()
        {
            #region Converting Field Entry Data
            if (SelectedCategory == null) SelectedCategory = Categories.FirstOrDefault();

            var price = 0m;
            var priceParses = !string.IsNullOrEmpty(PriceEntryText) &&
                decimal.TryParse(PriceEntryText.Replace("$", string.Empty), out price);
            if (!priceParses) price = 0m;

            var stock = -1;
            var stockParses = int.TryParse(StockEntryText, out stock);
            if (!stockParses) stock = -1;
            #endregion

            var prod = new Product
            {
                Name = NameEntryText,
                CategoryId = SelectedCategory.CategoryId,
                UnitPrice = price,
                Stock = stock,
                BarCode = CodeEntryText.Replace(" ", string.Empty),
                Description = DescEntryText
            };

            var val = await _service.ValidateProductAsync(prod);

            if (val.isValid)
            {
                StaticLogger.LogInfo(GetType(), "Admin requesting Product Registration from WPF form.");
                await _service.CreateAsync(prod);
                if (_callingView == null) return;
                _callingView.RefreshParent();
                _callingView.Stop();
            }
            else
            {
                var bulkList = val.errorList.Trim();
                var valErrors = new StringBuilder();
                while (!string.IsNullOrEmpty(bulkList))
                {
                    var line = bulkList.GetUntilOrEmpty(".");
                    valErrors.AppendLine(line);
                    bulkList = bulkList.Replace(line, string.Empty);
                }
                MessageBox.Show(valErrors.ToString(), "Failed to Register Product", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void SubmitCommandExecute()
        {
            #region Converting Field Entry Data
            if (SelectedCategory == null) SelectedCategory = Categories.FirstOrDefault();

            var price = 0m;
            var priceParses = !string.IsNullOrEmpty(PriceEntryText)&&
                decimal.TryParse(PriceEntryText.Replace("$", string.Empty), out price);
            if (!priceParses) price = 0m;

            var stock = -1;
            var stockParses = int.TryParse(StockEntryText, out stock);
            if (!stockParses) stock = -1;
            #endregion

            var prod = new Product
            {
                Name = NameEntryText,
                CategoryId = SelectedCategory.CategoryId,
                UnitPrice = price,
                Stock = stock,
                BarCode = CodeEntryText.Replace(" ",string.Empty),
                Description = DescEntryText
            };

            var val = _service.ValidateProduct(prod);

            if (val.isValid)
            {
                StaticLogger.LogInfo(GetType(), "Admin requesting Product Registration from WPF form.");
                _service.Create(prod);
                if (_callingView == null) return;
                _callingView.RefreshParent();
                _callingView.Stop();
            }
            else
            {
                var bulkList = val.errorList.Trim();
                var valErrors = new StringBuilder();
                while (!string.IsNullOrEmpty(bulkList))
                {
                    var line = bulkList.GetUntilOrEmpty(".");
                    valErrors.AppendLine(line);
                    bulkList = bulkList.Replace(line, string.Empty);
                }
                MessageBox.Show(valErrors.ToString(), "Failed to Register Product", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        #region Name

        private void NameBoxGotFocusCommandExecute()
        {
            if (NameEntryText == "Name") NameEntryText = string.Empty;
        }

        private void NameBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(NameEntryText)) NameEntryText = "Name";
        }

        private void NameBoxTextChangedCommandExecute()
        {
            var validation = _asyncCalls ? FetchNameValidationAsync(NameEntryText).Result : 
                _service.ValidateName(NameEntryText);
            if (!validation.isValid)
            {
                NameErrorText = validation.errorList.GetUntilOrEmpty(".");
                NameErrorVisibility = Visibility.Visible;
            }
            else
            {
                NameErrorText = _defError;
                NameErrorVisibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Price

        private void PriceBoxGotFocusCommandExecute()
        {
            if (PriceEntryText == "Price") PriceEntryText = string.Empty;
        }

        private void PriceBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(PriceEntryText)) PriceEntryText = "Price";
            var val = 0m;
            var parses = decimal.TryParse(PriceEntryText, out val);
            if (parses) PriceEntryText = string.Format("{0:C}", val);
        }

        private void PriceBoxTextChangedCommandExecute()
        {
            decimal val;
            var parses = decimal.TryParse(PriceEntryText.Replace("$",string.Empty), out val);
            var checkVal = !parses ? (decimal?)null : val;

            var validation = _asyncCalls ? FetchUnitPriceValidationAsync(checkVal.GetValueOrDefault()).Result :
                _service.ValidateUnitPrice(checkVal);
            if (!validation.isValid)
            {
                PriceErrorText = validation.errorList.GetUntilOrEmpty(".");
                PriceErrorVisibility = Visibility.Visible;
            }
            else
            {
                PriceErrorText = _defError;
                PriceErrorVisibility = Visibility.Collapsed;
            }

        }

        #endregion

        #region Stock

        private void StockBoxGotFocusCommandExecute()
        {
            if (StockEntryText == "Stock") StockEntryText = string.Empty;
        }

        private void StockBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(StockEntryText)) StockEntryText = "Stock";
        }

        private void StockBoxTextChangedCommandExecute()
        {
            if (StockEntryText.Length > 9)
            {
                StockEntryText = StockEntryText.Substring(0, 9);
                return;
            }

            var junk = 0;
            var isValid = int.TryParse(StockEntryText, out junk);

            if (!isValid)
            {
                StockErrorText = "Stock must be a whole number.";
                StockErrorVisibility = Visibility.Visible;
            }
            else
            {
                StockErrorText = _defError;
                StockErrorVisibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region BarCode

        private void CodeBoxGotFocusCommandExecute()
        {
            if (CodeEntryText == "Barcode") CodeEntryText = string.Empty;
        }

        private void CodeBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(CodeEntryText)) CodeEntryText = "Barcode";
        }

        private void CodeBoxTextChangedCommandExecute(TextChangedEventArgs e)
        {
            if (CodeEntryText == "Barcode") return;

            //Restrict Format and Add Separators for Readability
            //Handle Input
            var tempMan = string.Empty;
            ControlHelpers.HandleBarCodeInput(e, ref tempMan);
            ManufacturerText = tempMan;

            //Validate and Display Errors
            var checkTxt = CodeEntryText.Replace(" ", string.Empty);
            var validation = _asyncCalls ? FetchBarCodeValidationAsync(checkTxt).Result :
                _service.ValidateBarCode(checkTxt, 0);
            if (!validation.isValid)
            {
                CodeErrorText = validation.errorList.GetUntilOrEmpty(".");
                CodeErrorVisibility = Visibility.Visible;
            }
            else
            {
                CodeErrorText = _defError;
                CodeErrorVisibility = Visibility.Collapsed;
                //ManufacturerText = CodeEntryText.Replace(" ", string.Empty).Substring(0, 6);
            }
        }

        #endregion
        
        #region Description

        private void DescBoxGotFocusCommandExecute()
        {
            if (DescEntryText == "Description") DescEntryText = string.Empty;
        }

        private void DescBoxLostFocusCommandExecute()
        {
            if (string.IsNullOrEmpty(DescEntryText)) DescEntryText = "Description";
        }

        private void DescBoxTextChangedCommandExecute()
        {
            if(DescEntryText.Length>360)
            {
                DescEntryText = DescEntryText.Substring(0, 360);
                return;
            }
            
            RemainingDesc = DescEntryText.Length;
            DescCounterFg = _remainingDesc < 360 ? Brushes.LightGray : Brushes.Crimson;

            var validation = _asyncCalls ? FetchDescriptionValidationAsync(DescEntryText).Result :
                _service.ValidateDescription(DescEntryText);
            if (!validation.isValid)
            {
                DescErrorText = validation.errorList.GetUntilOrEmpty(".");
                DescErrorVisibility = Visibility.Visible;
            }
            else
            {
                DescErrorText = _defError;
                DescErrorVisibility = Visibility.Collapsed;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        public async Task<(bool isValid, string errorList)> FetchBarCodeValidationAsync(string checkTxt) 
            => await _service.ValidateBarCodeAsync(checkTxt, 0);

        public async Task<(bool isValid, string errorList)> FetchUnitPriceValidationAsync(decimal checkVal)
            => await _service.ValidateUnitPriceAsync(checkVal);

        public async Task<(bool isValid, string errorList)> FetchNameValidationAsync(string name)
            => await _service.ValidateNameAsync(name);

        public async Task<(bool isValid, string errorList)> FetchDescriptionValidationAsync(string desc)
                    => await _service.ValidateDescriptionAsync(desc);
        
        #endregion
    }
}

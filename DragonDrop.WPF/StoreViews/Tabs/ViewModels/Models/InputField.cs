using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.Infrastructure.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels.Models
{
    public class InputField : BindableBase
    {
        #region Fields

        private ICustomerDataService _service;
        private string _ogText;

        private static readonly string _imgFormat = ".png";
        private static readonly string _imgRoot = "pack://application:,,,/Art/";
        private string _title;
        private string _image;
        private string _text;
        private string _pencilImg;
        private bool _isEditMode;
        private Brush _entryTxtColour;
        private FontStyle _entryTxtStyle;
        private Visibility _errorVisibility;
        private string _errorText;

        #endregion


        public InputField(ICustomerDataService service, string title, string image, string text)
        {
            _service = service;
            _title = title;
            _image = image;
            _ogText = text;
            _text = _ogText;

            #region Commands

            EntryTextChangedCommand = new DelegateCommand<TextChangedEventArgs>(EntryTextChangedCommandExecute);
            PencilBtnClickCommand = new DelegateCommand(PencilBtnClickCommandExecute);
            EntryLostFocusCommand = new DelegateCommand(EntryLostFocusCommandExecute);
            KeyUpCommand = new DelegateCommand<KeyEventArgs>(KeyUpCommandExecute);

            #endregion

            #region Init

            _pencilImg = "pencil";
            _isEditMode = false;
            _entryTxtColour = Brushes.Gray;
            _entryTxtStyle = FontStyles.Italic;
            _errorVisibility = Visibility.Collapsed;
            _errorText = string.Empty;

            #endregion
        }

        #region Properties

        #region Commands

        public DelegateCommand<TextChangedEventArgs> EntryTextChangedCommand { get; }
        public DelegateCommand PencilBtnClickCommand { get; }
        public DelegateCommand EntryLostFocusCommand { get; }
        public DelegateCommand<KeyEventArgs> KeyUpCommand { get; }

        #endregion

        #region ViewModel Props

        public string Title
            {
                get=> _title+":";
                set { }
            }

        public string PencilPath
        {
            get => GetIconPath(_pencilImg);
            set => SetProperty(ref _pencilImg, value);
        }

        public string IconPath
        {
            get => GetIconPath(_image);
            set { }
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public string ToolTip
        {
            get
            {
                switch (_title)
                {
                    case "Name":
                        return "Up to 100 characters. Cannot be blank.";
                    case "E-mail":
                        return "Must be in a valid E-mail format, up to 100 characters.";
                    case "Phone":
                        return "Must be in a valid Phone Format(###-###-####)";
                    case "Address":
                        return "Up to 200 characters. Cannot be empty";
                    case "City":
                        return "Up to 100 characters. Cannot be empty";
                    case "State":
                        return "Up to 50 characters. Please use abbreviations where necessary.";
                    default:
                        return "Something went wrong in rendering the page.";
                }
            }
            set { }
        }

        public Brush EntryTxtColour
        {
            get => _entryTxtColour;
            set => SetProperty(ref _entryTxtColour, value);
        }

        public FontStyle EntryTxtStyle
        {
            get => _entryTxtStyle;
            set => SetProperty(ref _entryTxtStyle, value);
        }

        public Visibility ErrorVisibility
        {
            get => _errorVisibility;
            set => SetProperty(ref _errorVisibility, value);
        }

        public string ErrorText
        {
            get => _errorText;
            set => SetProperty(ref _errorText, value);
        }
        
        public string OgText
        {
            get => _ogText;
            set => SetProperty(ref _ogText, value);
        }

        #endregion

        #endregion

        #region Handlers

        private void EntryTextChangedCommandExecute(TextChangedEventArgs e)
        {
            (bool isValid, string errorList) validation;

            if(_title == "Phone")
            {
                if (Text.Length != 0)
                {
                    if (Text.Length > 12) Text = Text.Substring(0, 12);
                    if (Text.Length > 3 && Text[3] != '-') Text = Text.Insert(3, "-");
                    if (Text.Length > 7 && Text[7] != '-') Text = Text.Insert(7, "-");

                    if (e.Changes.FirstOrDefault().RemovedLength == 1
                        && Text.ToCharArray().LastOrDefault() == '-')
                        Text = Text.Substring(0, Text.Length - 1);

                    (e.Source as TextBox).CaretIndex = Text.Length;
                }
                validation = _service.ValidatePhone(Text);
            }

            else switch (_title)
            {
                case "Name":
                    validation = _service.ValidateName(Text);
                    break;
                case "E-mail":
                    validation = _service.ValidateEmail(Text);
                    break;
                case "Address":
                    validation = _service.ValidateAddress(Text);
                    break;
                case "City":
                    validation = _service.ValidateCity(Text);
                    break;
                case "State":
                    validation = _service.ValidateState(Text);
                    break;
                default:
                    validation.isValid = false;
                    validation.errorList = string.Empty;
                    break;
            }

            if (!validation.isValid)
            {
                PencilPath = "close";
                ErrorVisibility = Visibility.Visible;
                ErrorText = validation.errorList.GetUntilOrEmpty(".");
                return;
            }

            PencilPath = "checkCircle";
            ErrorVisibility = Visibility.Collapsed;
            ErrorText = string.Empty;
        }

        private void PencilBtnClickCommandExecute()
        {
            if (!IsEditMode)
            {
                PencilPath = "checkCircle";
                IsEditMode = true;
                EntryTxtColour = Brushes.Black;
                EntryTxtStyle = FontStyles.Normal;
                return;
            }

            if (_pencilImg == "close") Text = _ogText;

            PencilPath = "pencil";
            IsEditMode = false;
            EntryTxtColour = Brushes.Gray;
            EntryTxtStyle = FontStyles.Italic;
        }

        private void EntryLostFocusCommandExecute()
        {
            if (_pencilImg == "pencil") return;
            PencilBtnClickCommandExecute();
        }

        private void KeyUpCommandExecute(KeyEventArgs e)
        {
            if (e.Key == Key.Enter) EntryLostFocusCommandExecute();
        }

        #endregion

        #region Private/Not-exposed Methods

        private string GetIconPath(string iconName) => _imgRoot + iconName + _imgFormat;

        #endregion

    }
}

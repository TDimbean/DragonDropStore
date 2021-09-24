using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DragonDrop.WPF
{
    public class MainWindowViewModel : BindableBase
    {
        #region Fields

        private ICustomerDataService _service;
        private List<Customer> _allCusts;

        private string _adrBoxText;
        private string _passBoxText;
        private Visibility _adrApprovedVisibility;
        private Visibility _passApprovedVisibility;
        private Visibility _adrErrorVisibility;
        private Visibility _passErrorVisibility;

        #endregion

        public MainWindowViewModel(ICustomerDataService service)
        {
            _service = service;
            _allCusts = _service.GetAll().ToList();

            #region Commands

            AdrBoxTextChangedCommand = new DelegateCommand(AdrTextChangedCommandExecute);
            PassBoxTextChangedCommand = new DelegateCommand(PassTextChangedCommandExecute);

            #endregion

            #region Init

            AdrApprovedVisibility = Visibility.Hidden;
            PassApprovedVisibility = Visibility.Hidden;
            AdrErrorVisibility = Visibility.Hidden;
            PassErrorVisibility = Visibility.Hidden;
            AdrBoxText = string.Empty;
            PassBoxText = string.Empty;

            #endregion
        }

        #region Properties

        #region ViewModel Props

        public string AdrBoxText
        {
            get => _adrBoxText;
            set => SetProperty(ref _adrBoxText, value);
        }

        public string PassBoxText
        {
            get => _passBoxText;
            set => SetProperty(ref _passBoxText, value);
        }

        public Visibility AdrApprovedVisibility
        {
            get => _adrApprovedVisibility;
            set => SetProperty(ref _adrApprovedVisibility, value);
        }

        public Visibility PassApprovedVisibility
        {
            get => _passApprovedVisibility;
            set => SetProperty(ref _passApprovedVisibility, value);
        }

        public Visibility AdrErrorVisibility
        {
            get => _adrErrorVisibility;
            set => SetProperty(ref _adrErrorVisibility, value);
        }

        public Visibility PassErrorVisibility
        {
            get => _passErrorVisibility;
            set => SetProperty(ref _passErrorVisibility, value);
        }

        #endregion

        #region Commands

        public DelegateCommand AdrBoxTextChangedCommand { get; }
        public DelegateCommand PassBoxTextChangedCommand { get; }

        #endregion

        #endregion

        #region Private/Not Exposed Methods

        #region Handlers

        private void AdrTextChangedCommandExecute() => InputUpdate();
        private void PassTextChangedCommandExecute() => InputUpdate();

        #endregion

        #region Methods

        private void InputUpdate()
        {
            if (string.IsNullOrEmpty(AdrBoxText))
            {
                PassApprovedVisibility = Visibility.Hidden;
                PassErrorVisibility = Visibility.Hidden;
                AdrApprovedVisibility = Visibility.Hidden;
                AdrErrorVisibility = Visibility.Hidden;
                return;
            }

            if (_service.OneCustWithEmailExists(AdrBoxText))
            {
                AdrApprovedVisibility = Visibility.Visible;
                AdrErrorVisibility = Visibility.Hidden;

                if (_service.PassMatchesEmail(AdrBoxText, PassBoxText))
                {
                    PassApprovedVisibility = Visibility.Visible;
                    PassErrorVisibility = Visibility.Hidden;
                }
                else
                {
                    PassApprovedVisibility = Visibility.Hidden;
                    PassErrorVisibility = Visibility.Visible;
                }
            }
            else
            {
                AdrApprovedVisibility = Visibility.Hidden;
                AdrErrorVisibility = Visibility.Visible;
            }


        }

        #endregion

        #endregion
    }
}

using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using Prism.Commands;
using Prism.Mvvm;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class ValueCellAddViewModel : BindableBase
    {
        private int _repoType;
        private IRelayReloadAndRemoteCloseControl _callingWindow;

        private string _valText;

        public ValueCellAddViewModel(IRelayReloadAndRemoteCloseControl callingWindow, int repoType)
        {
            _callingWindow = callingWindow;
            _repoType = repoType;

            SubmitCommand = new DelegateCommand(SubmitCommandExecute);
            ValText = "New Value";
        }

        public string ValText
        {
            get => _valText;
            set => SetProperty(ref _valText, value);
        }

        public DelegateCommand SubmitCommand { get; }

        private void SubmitCommandExecute()
        {
            ResRepoHelpers.GetRepo(_repoType).Add(ValText);
            _callingWindow.RefreshParent();
            _callingWindow.Stop();
        }
    }
}

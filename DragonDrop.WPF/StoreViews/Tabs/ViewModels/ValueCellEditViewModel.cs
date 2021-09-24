using DragonDrop.Infrastructure.DTOs;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using Prism.Commands;
using Prism.Mvvm;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class ValueCellEditViewModel : BindableBase
    {
        private DefaultValueDto _ogItem;
        private int _repoType;
        private IRelayReloadAndRemoteCloseControl _callingWindow;

        private string _valText;

        public ValueCellEditViewModel(IRelayReloadAndRemoteCloseControl callingWindow, int repoType, DefaultValueDto ogItem)
        {
            _callingWindow = callingWindow;
            _repoType = repoType;
            _ogItem = ogItem;

            SubmitCommand = new DelegateCommand(SubmitCommandExecute);
            ValText = ogItem.Name;
        }

        public string ValText
        {
            get => _valText;
            set => SetProperty(ref _valText, value);
        }

        public DelegateCommand SubmitCommand { get; }

        private void SubmitCommandExecute()
        {
            ResRepoHelpers.GetRepo(_repoType).Update(new DefaultValueDto { ID = _ogItem.ID, Name = ValText });
            _callingWindow.RefreshParent();
            _callingWindow.Stop();
        }
    }
}

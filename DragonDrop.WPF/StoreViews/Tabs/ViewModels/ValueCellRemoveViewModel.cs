using DragonDrop.Infrastructure.DTOs;
using DragonDrop.WPF.Helpers;
using DragonDrop.WPF.Interfaces;
using Prism.Commands;
using Prism.Mvvm;

namespace DragonDrop.WPF.StoreViews.Tabs.ViewModels
{
    public class ValueCellRemoveViewModel : BindableBase
    {
        private IRelayReloadAndRemoteCloseControl _callingWindow;
        private int _repoType;
        private int _id;

        private string _valueId;
        private string _valueName;

        public ValueCellRemoveViewModel(IRelayReloadAndRemoteCloseControl callingWindow, DefaultValueDto ogItem, int repoType)
        {
            _callingWindow = callingWindow;
            _id = ogItem.ID;
            _repoType = repoType;

            ValueId = ogItem.ID.ToString();
            ValueName = ogItem.Name;

            YesCommand = new DelegateCommand(YesCommandExecute);
        }

        public string ValueId
        {
            get => _valueId;
            set => SetProperty(ref _valueId, value);
        }

        public string ValueName
        {
            get => _valueName;
            set => SetProperty(ref _valueName, value);
        }

        public DelegateCommand YesCommand { get; }

        private void YesCommandExecute()
        {
            ResRepoHelpers.GetRepo(_repoType).Remove(_id);
            _callingWindow.RefreshParent();
            _callingWindow.Stop();
        }
    }
}

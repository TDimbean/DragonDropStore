using System.Windows.Controls;

namespace DragonDrop.WPF.StoreViews.ViewModels
{
    public class Tab
    {
        private string _imgSourceFolder;
        private string _imgFormat;
        private string _imgName;

        public Tab(string title, string imgName, UserControl tabContent)
        {
            _imgSourceFolder = "../Art/";
            _imgFormat = ".png";
            _imgName = imgName;
            Title = title;
            TabContent = tabContent;

        }

        public UserControl TabContent { get; set; }
        public string Title { get; set; }
        public string Icon
        {
            get => _imgSourceFolder + _imgName + _imgFormat;
            set { }
        }
    }
}

using System.Windows;
using System.Windows.Controls;

namespace DragonDrop.WPF.Helpers
{
    public static class ButtonHelpers
    {
        public static void PerformClick(this Button btn)
        {
            btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }
}

using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace DragonDrop.WPF.Helpers
{
    public static class ControlHelpers
    {
        public static void ToggleBtnText(RoutedEventArgs e, string opt1, string opt2)
        {
            var btn = e.Source as Button;
            btn.Content = (string)btn.Content == opt1 ? opt2 : opt1;
        }

        public static void LimitText(TextCompositionEventArgs e, string reg)
            => e.Handled = new Regex(reg).IsMatch(e.Text);

        public static void SortDataGridByCustom(ref bool isAsc, string sortCrit, DataGrid custDG)
        {
            var sortDir = isAsc ? ListSortDirection.Ascending : ListSortDirection.Descending;
            isAsc = !isAsc;

            var dataView = CollectionViewSource.GetDefaultView(custDG.ItemsSource);
            dataView.SortDescriptions.Clear();
            dataView.SortDescriptions.Add(new SortDescription(sortCrit, sortDir));
            dataView.Refresh();
        }

        public static void UpdatePaging(RoutedEventArgs e, ref Label lbl)
        {
            var box = e.Source as TextBox;

            var size = 0;
            var parses = int.TryParse(box.Text, out size);
            if (!parses || size == 0)
            {
                box.Text = "All";
                lbl.Content = " results";
                return;
            }
            size = Math.Abs(size);
            box.Text = size.ToString();
            lbl.Content = size > 1 ? " results per page" : " result per page";
        }

        public static void PopComboBoxPlaceholder(ComboBox combo)
        {
            //var combo = e.Source as ComboBox;
            if (!combo.IsEditable) return;
            combo.IsEditable = false;
            combo.Focus();
        }

        public static void HandlePhoneNumberInput(TextChangedEventArgs e)
        {
            var box = e.Source as TextBox;
            if (box.Text.Length != 0)
            {
                if (box.Text.Length > 12) box.Text = box.Text.Substring(0, 12);
                if (box.Text.Length > 3 && box.Text[3] != '-') box.Text = box.Text.Insert(3, "-");
                if (box.Text.Length > 7 && box.Text[7] != '-') box.Text = box.Text.Insert(7, "-");

                if (e.Changes.FirstOrDefault().RemovedLength == 1
                    && box.Text.ToCharArray().LastOrDefault() == '-')
                    box.Text = box.Text.Substring(0, box.Text.Length - 1);

                box.CaretIndex = box.Text.Length;
            }
        }

        public static void HandleBarCodeInput(TextChangedEventArgs e, ref string manText)
        {
            var box = e.Source as TextBox;
            if (box.Text.Length != 0)
            {
                if (box.Text.Length > 15) box.Text = box.Text.Substring(0, 15);
                if (box.Text.Length > 1 && box.Text[1] != ' ') box.Text = box.Text.Insert(1, " ");
                if (box.Text.Length > 7 && box.Text[7] != ' ') box.Text = box.Text.Insert(7, " ");
                if (box.Text.Length > 13 && box.Text[13] != ' ') box.Text = box.Text.Insert(13, " ");

                if (e.Changes.FirstOrDefault().RemovedLength == 1
                    && box.Text.ToCharArray().LastOrDefault() == ' ')
                    box.Text = box.Text.Substring(0, box.Text.Length - 1);

                box.CaretIndex = box.Text.Length;
            }

            //Update Manufacturer Label
            manText = box.Text.Length > 6 ? box.Text.Substring(0, 7).Replace(" ", string.Empty) : "Manufacturer";

        }

        public static void HandlePortBoxInput(TextChangedEventArgs e)
        {
            var box = e.Source as TextBox;
            if (box.Text.Length != 0)
            {
                if (box.Text.Replace(" ", string.Empty).Length < 5)
                {
                    box.Text = box.Text.Replace(" ", string.Empty);

                }
                else
                {
                    if (box.Text.Replace(" ", string.Empty).Length > 5) box.Text = box.Text.Substring(0, 6);
                    if (box.Text.Length > 4 && box.Text[2] != ' ') box.Text = box.Text.Insert(2, " ");

                    if (e.Changes.FirstOrDefault().RemovedLength == 1
                        && box.Text.ToCharArray().LastOrDefault() == ' ')
                        box.Text = box.Text.Substring(0, box.Text.Length - 1);
                }
            }
                    box.CaretIndex = box.Text.Length;

            
        }
    }

}


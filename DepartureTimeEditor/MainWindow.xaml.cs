using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DepartureTimeEditor
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<DepartTime> _Departures { set; get; }
        private string _RegexDepartTime = @"[0-9]{4}";

        public MainWindow()
        {
            InitializeComponent();
            _Departures = new ObservableCollection<DepartTime>
            {
                new DepartTime
                {
                    Time = "9999"
                }
            };
            DataContext = _Departures;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // まず初期化
            _Departures.Clear();

            var matched = Regex.Matches(dptText.Text, _RegexDepartTime);

            foreach (Match m in matched)
            {
                _Departures.Add(new DepartTime
                {
                    Time = m.Value
                });
            }
        }

        private void Move_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(span.Text)) return;

            foreach (var item in DeparturesList.SelectedItems)
            {
                var departTime = item as DepartTime;
                DateTime beforeTime = new DateTime(2000, 1, 1, Convert.ToInt32(departTime.Time.Substring(0, 2)), Convert.ToInt32(departTime.Time.Substring(2, 2)), 0);
                DateTime afterTime = beforeTime.AddMinutes(Double.Parse(span.Text));
                
                //日付が変わったとしても時間の計算がずれるわけでもないので無視することにする 

                departTime.Time = afterTime.ToString("HHmm");
            }
            _Departures = new ObservableCollection<DepartTime>(_Departures.OrderBy(item => item.Time));
            DataContext = _Departures;
            DeparturesList.UpdateLayout();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(span.Text)) return;

            foreach (var item in DeparturesList.SelectedItems)
            {
                var departTime = item as DepartTime;
                DateTime beforeTime = new DateTime(2000, 1, 1, Convert.ToInt32(departTime.Time.Substring(0, 2)), Convert.ToInt32(departTime.Time.Substring(2, 2)), 0);
                DateTime afterTime = beforeTime.AddMinutes(Double.Parse(span.Text));

                //日付が変わったとしても時間の計算がずれるわけでもないので無視することにする 

                _Departures.Add(
                    new DepartTime
                    {
                        Time = afterTime.ToString("HHmm")
                    }
                 );
            }
            _Departures = new ObservableCollection<DepartTime>(_Departures.OrderBy(item => item.Time));
            DataContext = _Departures;
            DeparturesList.UpdateLayout();
        }

        private void ToString_Click(object sender, RoutedEventArgs e)
        {
            string _ret = string.Empty;
            foreach(var item in _Departures)
            {
                _ret += string.Format("            <Departure>{0}</Departure>\r\n", item.Time);
            }
            dptText.Text = _ret;
        }
    }

    public class DepartTime : INotifyPropertyChanged
    {
        private string _time;
        public string Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

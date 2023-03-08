using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Xml.Serialization;
using TrainScheduler.TimeTable;

namespace TimetableEditor
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _RegexDepartTime = @"[0-9]{4}";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログのインスタンスを生成
            var dialog = new OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "全てのファイル (*.*)|*.*";

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                // 選択されたファイル名 (ファイルパス) をメッセージボックスに表示
                Lbl_FirstStep.Content = dialog.FileName;
                TimeTableManager.LoadTimeTable(dialog.FileName);
                TimeTable = ParseTimeTable(TimeTableManager.TimeTable);
                this.DataContext = TimeTable;
                //LblTest.Content = TimeTableManager.TimeTable[0].Name;

                foreach(var tt in TimeTable)
                {
                    foreach(var s in tt.Stops)
                    {
                        string dptText = string.Empty;
                        foreach (var d in s.Departures)
                        {
                            if (string.IsNullOrEmpty(dptText))
                            {
                                dptText += string.Format("{0} ", d);
                            }
                            else
                            {
                                dptText += string.Format(", {0} ", d);
                            }
                        }
                        s.DeptText = dptText;
                    }
                }
            }
        }

        public ObservableCollection<LineRecord> ParseTimeTable(List<LineRecord> records)
        {
            ObservableCollection<LineRecord> ret = new ObservableCollection<LineRecord>();
            foreach(var line in records)
            {
                ret.Add(line);
            }
            return ret;
        }

        public TimeTableRecord ParseTimeTable(ObservableCollection<LineRecord> records)
        {
            var ret = new TimeTableRecord() { Lines = new List<LineRecord>() };
            foreach (var line in records)
            {
                ret.Lines.Add(line);
            }
            return ret;
        }

        public ObservableCollection<LineRecord> TimeTable { get; set; }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var tt in TimeTable)
            {
                foreach (var s in tt.Stops)
                {
                    var matched = Regex.Matches(s.DeptText, _RegexDepartTime);
                    s.Departures = new List<string>();

                    foreach (Match m in matched)
                    {
                        s.Departures.Add(m.Value);
                    }

                    s.DeptText = String.Empty;
                }
            }

            // 既存のファイルは日付をつけてバックアップ
            if (File.Exists(Lbl_FirstStep.Content.ToString()))
            {
                File.Copy(Lbl_FirstStep.Content.ToString(), Lbl_FirstStep.Content.ToString() +"." + DateTime.Now.ToString("yyyyMMddHHmmss"));
            }

            TimeTableManager.SaveXml(Lbl_FirstStep.Content.ToString(), ParseTimeTable(TimeTable));


            // 選択されたファイル名 (ファイルパス) をメッセージボックスに表示
            TimeTableManager.LoadTimeTable(Lbl_FirstStep.Content.ToString());
            TimeTable = ParseTimeTable(TimeTableManager.TimeTable);
            this.DataContext = TimeTable;

            foreach (var tt in TimeTable)
            {
                foreach (var s in tt.Stops)
                {
                    string dptText = string.Empty;
                    foreach (var d in s.Departures)
                    {
                        if (string.IsNullOrEmpty(dptText))
                        {
                            dptText += string.Format("{0} ", d);
                        }
                        else
                        {
                            dptText += string.Format(", {0} ", d);
                        }
                    }
                    s.DeptText = dptText;
                }
            }
        }

    }
}

﻿using FzLib;
using Mapster;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAPICodePack.FzExtension;
using SimpleFFmpegGUI.Manager;
using SimpleFFmpegGUI.Model;
using SimpleFFmpegGUI.WPF.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleFFmpegGUI.WPF.Panels
{
    public class FileIOPanelViewModel : INotifyPropertyChanged
    {
        public FileIOPanelViewModel()
        {
            for (int i = 0; i < MinInputsCount; i++)
            {
                Inputs.Add(new InputArgumentsDetail() { Index = i + 1, });
            }
            Inputs.CollectionChanged += Inputs_CollectionChanged;
        }

        private int minInputsCount = 1;

        public int MinInputsCount
        {
            get => minInputsCount;
            set
            {
                this.SetValueAndNotify(ref minInputsCount, value, nameof(MinInputsCount));
                while (value > Inputs.Count)
                {
                    Inputs.Add(new InputArgumentsDetail());
                }
                while (value < Inputs.Count)
                {
                    Inputs.RemoveAt(Inputs.Count - 1);
                }
            }
        }

        private bool showTimeClip;

        public bool ShowTimeClip
        {
            get => showTimeClip;
            set => this.SetValueAndNotify(ref showTimeClip, value, nameof(ShowTimeClip));
        }

        private void Inputs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            for (int i = 0; i < Inputs.Count; i++)
            {
                Inputs[i].Index = i + 1;
                Inputs[i].CanDelete = Inputs.Count > MinInputsCount;
            }
        }

        public ObservableCollection<InputArgumentsDetail> Inputs { get; } = new ObservableCollection<InputArgumentsDetail>();
        private string output;

        public string Output
        {
            get => output;
            set => this.SetValueAndNotify(ref output, value, nameof(Output));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool canChangeInputsCount;

        public bool CanChangeInputsCount
        {
            get => canChangeInputsCount;
            set => this.SetValueAndNotify(ref canChangeInputsCount, value, nameof(CanChangeInputsCount));
        }

        public void Reset()
        {
            Inputs.Clear();
            while (Inputs.Count < MinInputsCount)
            {
                Inputs.Add(new InputArgumentsDetail());
            }
        }

        public void Update(TaskType type)
        {
            CanChangeInputsCount = type is TaskType.Code or TaskType.Combine or TaskType.Concat;
            MinInputsCount = type switch
            {
                TaskType.Code => 1,
                TaskType.Combine or TaskType.Concat or TaskType.Compare => 2,
                _ => 0
            };
            ShowTimeClip = type switch
            {
                TaskType.Code => true,
                _ => false
            };
        }

        internal void Update(TaskType type, List<InputArguments> inputs, string output)
        {
            Update(type);
            Inputs.Clear();
            foreach (var input in inputs)
            {
                var newInput = input.Adapt<InputArgumentsDetail>();
                newInput.Update();
                Inputs.Add(newInput);
            }
            Output = output;
        }
    }

    public partial class FileIOPanel : UserControl
    {
        public FileIOPanel()
        {
            DataContext = ViewModel;
            InitializeComponent();
        }

        public FileIOPanelViewModel ViewModel { get; } = App.ServiceProvider.GetService<FileIOPanelViewModel>();

        public List<InputArguments> GetInputs()
        {
            foreach (var input in ViewModel.Inputs)
            {
                input.Apply();
            }
            var inputs = ViewModel.Inputs.Where(p => !string.IsNullOrEmpty(p.FilePath));
            if (inputs.Count() < ViewModel.MinInputsCount)
            {
                throw new Exception("输入文件少于需要的文件数量");
            }
            return inputs.Cast<InputArguments>().ToList();
        }

        public string GetOutput()
        {
            return ViewModel.Output;
        }

        public void Reset()
        {
            ViewModel.Reset();
        }

        private void BrowseFileButton_Click(object sender, RoutedEventArgs e)
        {
            var input = (sender as FrameworkElement).Tag as InputArgumentsDetail;
            string path = new FileFilterCollection().AddAll().CreateOpenFileDialog().GetFilePath();
            if (path != null)
            {
                input.FilePath = path;
            }
        }

        private void DeleteFileButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Inputs.Remove((sender as FrameworkElement).Tag as InputArgumentsDetail);
        }

        public void Update(TaskType type, List<InputArguments> inputs, string output)
        {
            ViewModel.Update(type, inputs, output);
        }

        public void Update(TaskType type)
        {
            ViewModel.Update(type);
        }

        public void AddInput()
        {
            ViewModel.Inputs.Add(new InputArgumentsDetail());
        }

        public void BrowseAndAddInput()
        {
            string path = new FileFilterCollection().AddAll().CreateOpenFileDialog().GetFilePath();
            if (path != null)
            {
                var input = new InputArgumentsDetail();
                input.FilePath = path;
                ViewModel.Inputs.Add(input);
            }
        }

        private void BrowseOutputFileButton_Click(object sender, RoutedEventArgs e)
        {
            string path = new FileFilterCollection().AddAll().CreateSaveFileDialog().GetFilePath();
            if (path != null)
            {
                path = path.RemoveEnd(".*");
                ViewModel.Output = path;
            }
        }
    }

    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            if (value is TimeSpan ts)
            {
                return ts.ToString("hh\\:mm\\:ss");
            }
            throw new Exception("绑定源必须为TimeSpan");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            if (value is string str)
            {
                str = str.Trim().ToLower();
                if (str.Any(p => p is ';' or '：' or '：'))
                {
                    str = str
                        .Replace(";", ":")
                        .Replace("；", ":")
                        .Replace('：', ':');
                }
                if (str.Count(p => p == ':') == 1)
                {
                    str = "0:" + str;
                }
                if (TimeSpan.TryParse(str, out TimeSpan t))
                {
                    return t;
                }
                if (double.TryParse(str.TrimEnd('s'), out double s))
                {
                    return TimeSpan.FromSeconds(s);
                }
                if (str.EndsWith('m') && double.TryParse(str.TrimEnd('m'), out double m))
                {
                    return TimeSpan.FromMinutes(m);
                }
                if (str.EndsWith('h') && double.TryParse(str.TrimEnd('h'), out double h))
                {
                    return TimeSpan.FromHours(h);
                }
                throw new Exception("转换失败");
            }
            throw new Exception("绑定目标必须为String");
        }
    }
}
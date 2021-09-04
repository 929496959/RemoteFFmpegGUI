﻿using Enterwell.Clients.Wpf.Notifications;
using FzLib;
using Mapster;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAPICodePack.FzExtension;
using ModernWpf.FzExtension.CommonDialog;
using Notifications.Wpf.Core;
using SimpleFFmpegGUI.Manager;
using SimpleFFmpegGUI.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleFFmpegGUI.WPF.Panels
{
    public class PresetsPanelViewModel : INotifyPropertyChanged
    {
        public PresetsPanelViewModel()
        {
        }

        private ObservableCollection<CodePreset> presets;

        public ObservableCollection<CodePreset> Presets
        {
            get => presets;
            set => this.SetValueAndNotify(ref presets, value, nameof(Presets));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public INotificationMessageManager Manager { get; } = new NotificationMessageManager();
    }

    public partial class PresetsPanel : UserControl
    {
        public PresetsPanel()
        {
            DataContext = ViewModel;
            InitializeComponent();
        }

        public PresetsPanelViewModel ViewModel { get; } = App.ServiceProvider.GetService<PresetsPanelViewModel>();
        private TaskType type;

        public void Update(TaskType type)
        {
            this.type = type;
            ViewModel.Presets = new ObservableCollection<CodePreset>(PresetManager.GetPresets().Where(p => p.Type == type));
        }

        public CodeArgumentsPanelViewModel CodeArgumentsViewModel { get; set; }

        public async Task SaveToPresetAsync()
        {
            Debug.Assert(CodeArgumentsViewModel != null);
            var name = await CommonDialog.ShowInputDialogAsync("请输入新预设的名称");
            if (name == null)
            {
                return;
            }
            try
            {
                var preset = PresetManager.AddPreset(name, type, CodeArgumentsViewModel.GetArguments());
                ViewModel.Presets.Add(preset);
            }
            catch (Exception ex)
            {
                this.CreateMessage().QueueError("新增预设失败", ex);
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(CodeArgumentsViewModel != null);
            var preset = (sender as FrameworkElement).Tag as CodePreset;
            CodeArgumentsViewModel.Update(type, preset.Arguments);
            this.CreateMessage().QueueSuccess($"已加载预设“{preset.Name}”");
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(CodeArgumentsViewModel != null);
            var preset = (sender as FrameworkElement).Tag as CodePreset;
            try
            {
                preset.Arguments = CodeArgumentsViewModel.GetArguments();
                this.CreateMessage().QueueSuccess($"预设“{preset.Name}”更新成功");
            }
            catch (Exception ex)
            {
                this.CreateMessage().QueueError("更新预设失败", ex);
            }
        }
    }
}
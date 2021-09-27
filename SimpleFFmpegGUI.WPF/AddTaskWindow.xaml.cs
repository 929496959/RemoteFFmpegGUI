﻿using Enterwell.Clients.Wpf.Notifications;
using FzLib;
using Microsoft.Extensions.DependencyInjection;
using SimpleFFmpegGUI.Manager;
using SimpleFFmpegGUI.Model;
using SimpleFFmpegGUI.WPF.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SimpleFFmpegGUI.WPF
{
    public class AddTaskWindowViewModel : INotifyPropertyChanged
    {
        public AddTaskWindowViewModel(QueueManager queue)
        {
            Queue = queue;
        }

        public IEnumerable TaskTypes => Enum.GetValues(typeof(TaskType));
        private TaskType type;

        public event PropertyChangedEventHandler PropertyChanged;

        public TaskType Type
        {
            get => type;
            set
            {
                this.SetValueAndNotify(ref type, value, nameof(Type));
                CanAddFile = value is TaskType.Code or TaskType.Concat;
            }
        }

        public QueueManager Queue { get; }

        private bool allowChangeType = true;

        public bool AllowChangeType
        {
            get => allowChangeType;
            set => this.SetValueAndNotify(ref allowChangeType, value, nameof(AllowChangeType));
        }

        private bool canAddFile;
        public bool CanAddFile
        {
            get => canAddFile;
            set => this.SetValueAndNotify(ref canAddFile, value, nameof(CanAddFile));
        }

    }

    /// <summary>
    /// Interaction logic for AddTaskWindow.xaml
    /// </summary>
    public partial class AddTaskWindow : Window
    {
        public AddTaskWindowViewModel ViewModel { get; set; }

        public AddTaskWindow(AddTaskWindowViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            DataContext = ViewModel;
            InitializeComponent();
            ViewModel.Type = TaskType.Code;
            presetsPanel.CodeArgumentsViewModel = argumentsPanel.ViewModel;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.Type):
                    fileIOPanel.Update(ViewModel.Type);
                    argumentsPanel.Update(ViewModel.Type);
                    presetsPanel.Update(ViewModel.Type);
                    break;

                default:
                    break;
            }
        }

        private void AddToQueueAndStartButton_Click(object sender, RoutedEventArgs e)
        {
            AddToQueue(true);
        }

        private void AddToQueueButton_Click(object sender, RoutedEventArgs e)
        {
            AddToQueue(false);
        }

        public void SetAsClone(TaskInfo task)
        {
            ViewModel.AllowChangeType = false;
            ViewModel.Type = task.Type;
            fileIOPanel.Update(task.Type, task.Inputs, task.Output);
            argumentsPanel.Update(task);
        }

        public void SetFiles(IEnumerable<string> files)
        {
            fileIOPanel.Update(TaskType.Code, files.Select(p => new InputArguments() { FilePath = p }).ToList(), null);
        }

        private void AddToQueue(bool start)
        {
            try
            {
                List<InputArguments> inputs = fileIOPanel.GetInputs();
                string output = fileIOPanel.GetOutput();
                OutputArguments args = argumentsPanel.GetOutputArguments();
                switch (ViewModel.Type)
                {
                    case TaskType.Code:
                        foreach (var input in inputs)
                        {
                            var task = TaskManager.AddTask(TaskType.Code, new List<InputArguments>() { input }, output, args);
                            App.ServiceProvider.GetService<TasksAndStatuses>().Tasks.Insert(0, UITaskInfo.FromTask(task));
                        }
                        this.CreateMessage().QueueSuccess($"已加入{inputs.Count}个任务队列");
                        break;

                    default:
                        {
                            var task = TaskManager.AddTask(ViewModel.Type, inputs, output, args);
                            App.ServiceProvider.GetService<TasksAndStatuses>().Tasks.Insert(0, UITaskInfo.FromTask(task));
                            this.CreateMessage().QueueSuccess("已加入队列");
                        }
                        break;
                }

                fileIOPanel.Reset();
            }
            catch (Exception ex)
            {
                this.CreateMessage().QueueError("加入队列失败", ex);
            }
            if (start)
            {
                ViewModel.Queue.StartQueue();
            }
        }

        private async void SaveToPresetButton_Click(object sender, RoutedEventArgs e)
        {
            await presetsPanel.SaveToPresetAsync();
        }

        private void BrowseAndAddInputButton_Click(object sender, RoutedEventArgs e)
        {
            fileIOPanel.BrowseAndAddInput();
        }

        private void AddInputButton_Click(object sender, RoutedEventArgs e)
        {
            fileIOPanel.AddInput();
        }

        private void CommandBar_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as FrameworkElement).Focus();
        }
    }
}
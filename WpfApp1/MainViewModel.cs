using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using XSharedMemoryPubSub.Models;
using XSharedMemoryPubSub.Services;

namespace WpfApp1
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ISharedMemoryPubSub _sharedMemoryPubSub;
        public MainViewModel()
        {
            _sharedMemoryPubSub = new SharedMemoryPubSub("demo");
        }
        [ObservableProperty] private string _send;
        [ObservableProperty] private string _message;

        [RelayCommand]
        private void Subscribe()
        {
            _sharedMemoryPubSub.Subscribe(MessageTopics.DATA_SYNC, OnThemeChanged);
        }

        private void OnThemeChanged((int MessageId, int TopicId, byte[] Data) obj)
        {
            var ms = Encoding.UTF8.GetString(obj.Data).TrimEnd('\0');
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                Message = ms;
            });
        }

        [RelayCommand]
        private void Unsubscribe()
        {
            _sharedMemoryPubSub.Unsubscribe(MessageTopics.DATA_SYNC);
        }

        [RelayCommand]
        private void PublishMessage()
        {
            _sharedMemoryPubSub.Publish(
                MessageTopics.DATA_SYNC,
                Encoding.UTF8.GetBytes(Send)
            );
        }

    }
}

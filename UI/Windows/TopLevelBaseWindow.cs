using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using AvaloniaToolbox.ViewModel;
using AvaloniaToolbox.Functions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AvaloniaToolbox.UI.Windows
{
    public class TopLevelBaseWindow : ReactiveWindow<TopLevelBaseViewModel>, INotifyPropertyChanged
    {
        protected bool _isReleasable = true;
        public TopLevelBaseWindow()
        {
            WindowStartupLocation   = WindowStartupLocation.CenterScreen;
            SizeToContent           = SizeToContent.WidthAndHeight;
            Opened                  += Window_Opened;
            if (CustomDebug.IsDebug)
            {
                #if DEBUG
                this.AttachDevTools();
                #endif
            }
        }

        private void Window_Opened(object? sender, EventArgs e)
        {
            MinWidth    = Width;
            MinHeight   = Height;
            if (CustomDebug.IsRelease && _isReleasable == false)
            {
                Content = null;
                Extensions.RunOnUIThread(async () =>
                {
                    bool dismiss = await MessageBox.Show(
                        this,
                        CustomIconType.Maintenance,
                        "This feature is still in development",
                        "This window is under development and is marked as so. This feature will soon be available.",
                        "OK",
                        Colors.Red
                    );

                    Close();
                });
            }
        }

         public event PropertyChangedEventHandler? PropertyChangedHandler;

        event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                PropertyChangedHandler += value;
            }

            remove
            {
                PropertyChangedHandler -= value;
            }
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName]string? propertyName = "")
        {
            PropertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                OnPropertyChanged(propertyName);
                return true;
            }

            return false;
        }

        protected bool SetProperty<T>(ref T field, T newValue, Action onChanged, [CallerMemberName] string? propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                OnPropertyChanged(propertyName);
                onChanged();
                return true;
            }

            return false;
        }
    }
}


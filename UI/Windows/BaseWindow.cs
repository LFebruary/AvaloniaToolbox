using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using AvaloniaToolbox.ViewModel;
using AvaloniaToolbox.Functions;
using AvaloniaToolbox.UI.CustomIcons;

namespace AvaloniaToolbox.UI.Windows
{
    public class BaseWindow : ReactiveWindow<BaseViewModel>
    {
        protected bool _isReleasable = true;
        public BaseWindow()
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
    }
}


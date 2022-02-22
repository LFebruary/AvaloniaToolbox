using Avalonia.Threading;
using AvaloniaToolbox.Functions;
using AvaloniaToolbox.UI;
using AvaloniaToolbox.UI.Windows;
using ReactiveUI;
using System.Runtime.CompilerServices;
using static AvaloniaToolbox.UI.MessageBox;

namespace AvaloniaToolbox.ViewModel
{
    public class TopLevelBaseViewModel : ReactiveObject
    {
        #region Constructor
        public TopLevelBaseViewModel(TopLevelBaseWindow parentView)
        {
            _parentView = parentView;

            _parentView.Opened          += OnOpened;
            _parentView.Closed          += OnClosed;
            _parentView.Activated       += OnActivated;
            _parentView.Deactivated     += OnDeactivated;
        }

        private string EventString(BaseWindowEvent @event)
        {
            string eventString = @event switch
            {
                BaseWindowEvent.Closed      => "Closed",
                BaseWindowEvent.Opened      => "Opened",
                BaseWindowEvent.Closing     => "Closing",
                BaseWindowEvent.Activated   => "Activated",
                BaseWindowEvent.Deactivated => "Deactivated",
                _ => "UndefinedEvent",
            };

            return $"{_parentView.Title}Window::Event::{eventString}";
        }

        protected enum BaseWindowEvent
        {
            Opened,
            Opening,
            Closed,
            Closing,
            Activated,
            Deactivated
        }

        protected virtual void OnDeactivated(object? sender, EventArgs e) => CustomDebug.WriteLine(EventString(BaseWindowEvent.Deactivated));

        protected virtual void OnActivated(object? sender, EventArgs e) => CustomDebug.WriteLine(EventString(BaseWindowEvent.Activated));

        protected virtual void OnClosed(object? sender, EventArgs e) => CustomDebug.WriteLine(EventString(BaseWindowEvent.Closed));

        protected virtual void OnOpened(object? sender, EventArgs e) => CustomDebug.WriteLine(EventString(BaseWindowEvent.Opened));
        #endregion

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value, () => _parentView.Title = value);
        }

        #region Fields
        public TopLevelBaseWindow _parentView;
        #endregion

        #region Property changed methods
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null, DispatcherPriority? priority = null) => Extensions.RunOnUIThread(() => this.RaisePropertyChanged(propertyName), priority);

        protected void SetProperty<T>(ref T storage, T value, DispatcherPriority? priority = null, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            Extensions.RunOnUIThread(() => this.RaisePropertyChanged(propertyName), priority);
        }

        protected void SetProperty<T>(ref T storage, T value, Action onChange, DispatcherPriority? priority = null, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            Extensions.RunOnUIThread(() => this.RaisePropertyChanged(propertyName), priority);
            onChange?.Invoke();
        }

        protected void SetProperty<T>(ref T storage, T value, Action<T> onChange, DispatcherPriority? priority = null, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            Extensions.RunOnUIThread(() => this.RaisePropertyChanged(propertyName), priority);
            onChange?.Invoke(value);
        }
        #endregion

        #region Dialogs
        protected async Task<bool> ShowDialog(DialogType messageBoxType, string message, string positive = "OK", string negative = "Cancel", string overrideTitle = "")
        {
            string title = string.IsNullOrWhiteSpace(overrideTitle)
                ? messageBoxType.ToString()
                : overrideTitle;

            return await Extensions.RunOnUIThread(() => Show(
                parent: _parentView,
                title: title, 
                message: message, 
                positive: positive, 
                negative: negative)
            );
        }

        protected async Task<bool> ShowDialog(DialogType messageBoxType, string message, string positive, string negative, Action onDismiss, string overrideTitle = "")
        {
            string title = string.IsNullOrWhiteSpace(overrideTitle)
                ? messageBoxType.ToString()
                : overrideTitle;

            return await Extensions.RunOnUIThread(() => Show(
                parent: _parentView, 
                title: title,
                message: message, 
                positive: positive, 
                negative: negative, 
                onDismiss: onDismiss)
            );
        }

        protected async Task<bool?> ShowDialog(DialogType messageBoxType, string message, string positive, string negative, string neutral, string overrideTitle = "")
        {
            string title = string.IsNullOrWhiteSpace(overrideTitle)
                ? messageBoxType.ToString()
                : overrideTitle;

            return await Extensions.RunOnUIThread(() => Show(
                parent: _parentView, 
                message: message, 
                title: title, 
                positive: positive, 
                negative: negative, 
                neutral: neutral)
            );
        }

        protected async Task<bool?> ShowDialog(DialogType messageBoxType, string message, string positive, string negative, Action onDismiss, string neutral, string overrideTitle = "")
        {
            string title = string.IsNullOrWhiteSpace(overrideTitle)
                ? messageBoxType.ToString()
                : overrideTitle;

            return await Extensions.RunOnUIThread(() => Show(
                parent: _parentView,
                title: title,
                message: message, 
                positive: positive, 
                negative: negative, 
                neutral: neutral, 
                onDismiss: onDismiss)
            );
        }

        protected async Task ShowDialog(Exception e, string? buttonText = null)
        {
            _ = await Extensions.RunOnUIThread(() => Show(_parentView, e, buttonText));
        }

        protected async Task ShowDialog(Avalonia.Media.Imaging.Bitmap bitmap)
        {
            await Extensions.InvokeOnUiThread(Show(_parentView, bitmap));
        }

        protected async Task ShowDialog(Task runningTask, string title, string prompt)
        {
            await Extensions.InvokeOnUiThread(ShowProgressDialog(_parentView, title, prompt, runningTask));
        }

        protected async Task ShowDialog(Action runningAction, string title, string prompt)
        {
            await Extensions.InvokeOnUiThread(ShowProgressDialog(_parentView, title, prompt, runningAction));
        }

        protected async Task<string?> InputDialog(string title, string prompt, string? placeholder = null)
        {
            return await Extensions.RunOnUIThread(() => MessageBox.InputDialog(_parentView, title, prompt, "OK", "Cancel", placeholder: placeholder));
        }

        protected async Task<T?> ItemPickerDialog<T>(string title, string prompt, string label, List<T> items, T? selectedItem, string? placeholder = null)
        {
            (T? item, int? _, string? _) = await Extensions.RunOnUIThread(() => PickerDialog(_parentView, title, prompt, label, items, selectedItem, placeholder));
            return item;
        }

        protected async Task<int> IndexPickerDialog<T>(string title, string prompt, string label, List<T> items, T? selectedItem = default, string? placeholder = null, string? selectButton = null, string? cancelButton = null)
        {
            (T? _, int? index, string? _) = await Extensions.RunOnUIThread(() => PickerDialog(_parentView, title, prompt, label, items, selectedItem, placeholder, selectButton, cancelButton));

            CustomDebug.WriteLine(index);

            return index ?? -1;
        }
        #endregion
    }
}

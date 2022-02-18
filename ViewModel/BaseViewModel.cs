using AvaloniaToolbox.Functions;
using AvaloniaToolbox.UI;
using AvaloniaToolbox.UI.Windows;
using ReactiveUI;
using System.Runtime.CompilerServices;
using static AvaloniaToolbox.UI.MessageBox;

namespace AvaloniaToolbox.ViewModel
{
    public class BaseViewModel : ReactiveObject
    {
        #region Constructor
        public BaseViewModel(BaseWindow parentView)
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
        public BaseWindow _parentView;
        #endregion

        #region Property changed methods
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => Extensions.RunOnUIThread(() => this.RaisePropertyChanged(propertyName));

        protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            Extensions.RunOnUIThread(() => this.RaisePropertyChanged(propertyName));
        }

        protected void SetProperty<T>(ref T storage, T value, Action onChange, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            Extensions.RunOnUIThread(() => this.RaisePropertyChanged(propertyName));
            onChange?.Invoke();
        }

        protected void SetProperty<T>(ref T storage, T value, Action<T> onChange, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            Extensions.RunOnUIThread(() => this.RaisePropertyChanged(propertyName));
            onChange?.Invoke(value);
        }
        #endregion

        #region Dialogs
        internal async Task<bool> ShowDialog(DialogType messageBoxType, string message, string positive = "OK", string negative = "Cancel", string overrideTitle = "")
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

        internal async Task<bool> ShowDialog(DialogType messageBoxType, string message, string positive, string negative, Action onDismiss, string overrideTitle = "")
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

        internal async Task<bool?> ShowDialog(DialogType messageBoxType, string message, string positive, string negative, string neutral, string overrideTitle = "")
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

        internal async Task<bool?> ShowDialog(DialogType messageBoxType, string message, string positive, string negative, Action onDismiss, string neutral, string overrideTitle = "")
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

        internal async Task ShowDialog(Exception e, string? buttonText = null)
        {
            _ = await Extensions.RunOnUIThread(() => Show(_parentView, e, buttonText));
        }

        internal async Task ShowDialog(Avalonia.Media.Imaging.Bitmap bitmap)
        {
            await Extensions.InvokeOnUiThread(Show(_parentView, bitmap));
        }

        internal async Task ShowDialog(Task runningTask, string title, string prompt)
        {
            await Extensions.InvokeOnUiThread(ShowProgressDialog(_parentView, title, prompt, runningTask));
        }

        internal async Task ShowDialog(Action runningAction, string title, string prompt)
        {
            await Extensions.InvokeOnUiThread(ShowProgressDialog(_parentView, title, prompt, runningAction));
        }

        internal async Task<string?> InputDialog(string title, string prompt, string? placeholder = null)
        {
            return await Extensions.RunOnUIThread(() => MessageBox.InputDialog(_parentView, title, prompt, "OK", "Cancel", placeholder: placeholder));
        }

        internal async Task<T?> ItemPickerDialog<T>(string title, string prompt, string label, List<T> items, T? selectedItem, string? placeholder = null)
        {
            (T? item, int? _, string? _) = await Extensions.RunOnUIThread(() => PickerDialog(_parentView, title, prompt, label, items, selectedItem, placeholder));
            return item;
        }

        internal async Task<int> IndexPickerDialog<T>(string title, string prompt, string label, List<T> items, T? selectedItem = default, string? placeholder = null, string? selectButton = null, string? cancelButton = null)
        {
            (T? _, int? index, string? _) = await Extensions.RunOnUIThread(() => PickerDialog(_parentView, title, prompt, label, items, selectedItem, placeholder, selectButton, cancelButton));

            CustomDebug.WriteLine(index);

            return index ?? -1;
        }
        #endregion
    }
}

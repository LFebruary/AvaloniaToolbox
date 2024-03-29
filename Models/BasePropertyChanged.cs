﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AvaloniaToolbox.Models
{
    public abstract class BasePropertyChanged : INotifyPropertyChanged
    {
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
        
        protected virtual void OnPropertyChanged([CallerMemberName]string? propertyName = "", bool runOnUiThread = false)
        {
            if (runOnUiThread)
            {
                Functions.Extensions.RunOnUIThread(() =>
                {
                    PropertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }, Avalonia.Threading.DispatcherPriority.DataBind);
            }
            else
            {
                PropertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null, bool runOnUiThread = false)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                OnPropertyChanged(propertyName, runOnUiThread);
                return true;
            }

            return false;
        }

        protected bool SetProperty<T>(ref T field, T newValue, Action onChanged, [CallerMemberName] string? propertyName = null, bool runOnUiThread = false)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                OnPropertyChanged(propertyName, runOnUiThread);
                onChanged();
                return true;
            }

            return false;
        }
    }
}

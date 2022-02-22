using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaToolbox.Functions;
using AvaloniaToolbox.UI.Windows;
using Color = Avalonia.Media.Color;
using Image = Avalonia.Controls.Image;

namespace AvaloniaToolbox.UI
{
    public partial class MessageBox : TopLevelBaseWindow
    {
        public MessageBox()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public bool?    FinalBoolResult         { get; set; } = false;
        public string?  FinalStringResult       { get; set; }
        public object?  FinalObjectResult       { get; set; }

        public CancellationTokenSource _cancellationTokenSource = new();

        public static async Task ShowProgressDialog(Window? parent, string title, string message, Action action) => await ShowProgressDialog(parent, title, message, Task.Run(action));
        public static async Task ShowProgressDialog(Window? parent, string title, string message, Task task)
        {
            MessageBox messageBox = new()
            {
                Title = title
            };

            messageBox.FindControl<TextBlock>("Text").Text = message;
            messageBox.FindControl<ProgressBar>("ProgressBar").IsVisible = true;

            Button? btn = new()
            {
                Content = "Cancel",
                MinWidth = 75
            };

            btn.Click += (_, __) =>
            {
                messageBox._cancellationTokenSource.Cancel();
                messageBox.Close();
            };

            StackPanel? buttonPanel = messageBox.FindControl<StackPanel>("Buttons");
            buttonPanel.Children.Add(btn);
            

            if (parent is not null) 
            {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                _ = messageBox.ShowDialog(parent);
            }
            else 
            {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                messageBox.Show();
            }

            if (await Task.WhenAny(
                    Task.Run(() => s_dialogMinimumTime.Wait(),   messageBox._cancellationTokenSource.Token),
                    Task.Run(() => task.Wait(),                 messageBox._cancellationTokenSource.Token)
                ) == s_dialogMinimumTime)
            {
                task.GetAwaiter().OnCompleted(() => messageBox.Close());
            }
            else
            {
                s_dialogMinimumTime.GetAwaiter().OnCompleted(() => messageBox.Close());
            }
        }

        private static readonly Task s_dialogMinimumTime = Task.Delay(1000);
        /// <summary>
        /// This is a MessageBox.Show invocation that displays a QR-code
        /// </summary>
        /// <param name="parent">Parent of the dialog</param>
        /// <param name="bitmap">QRCode bitmap to display</param>
        public static Task Show(Window? parent, Avalonia.Media.Imaging.Bitmap bitmap)
        {
            MessageBox? messageBox = new()
            {
                Title = "Scan code"
            };

            TextBlock? textBlock = messageBox.FindControl<TextBlock>("Text");
            textBlock.IsVisible = false;

            Image? image = messageBox.FindControl<Image>("DialogImage");

            image.IsVisible = true;
            image.Width     = 400;
            image.Height    = 400;
            image.Source    = bitmap;
            
            StackPanel? buttonPanel = messageBox.FindControl<StackPanel>("Buttons");

            void AddButton(string caption)
            {
                Button? btn = new()
                {
                    Content = caption,
                    MinWidth = 75
                };

                btn.Click += (_, __) => messageBox.Close();
                buttonPanel.Children.Add(btn);
            }

            AddButton("Close");

            TaskCompletionSource<bool?>? tcs = new();
            messageBox.Closed += delegate { _ = tcs.TrySetResult(true); };
            if (parent is not null) 
            {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                _ = messageBox.ShowDialog(parent);
            }
            else 
            {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                messageBox.Show();
            }

            return tcs.Task;
        }

        /// <summary>
        /// This is a MessageBox.Show invocation that displays a dialog with positive, negative and neutral buttons
        /// </summary>
        /// <param name="parent">Parent of the dialog</param>
        /// <param name="message">Text to show in dialog</param>
        /// <param name="title">Title of dialog</param>
        /// <param name="positive">Text on positive button of dialog</param>
        /// <param name="negative">Text on negative button of dialog</param>
        /// <param name="neutral">Text on neutral button of dialog</param>
        /// <returns>Returns true when positive button is clicked, false when negative button is clicked and null when neutral button is clicked</returns>
        public static Task<bool?> Show(Window? parent, string title, string message, string positive, string negative, string neutral)
        {
            (TaskCompletionSource<bool?> taskCompletionSource, MessageBox messageBox) = CreateNullableBoolMessageBox(parent, title, message);

            messageBox.AddButton(positive, true);
            messageBox.AddButton(negative, false);
            messageBox.AddButton(neutral, boolButtonResult: null, true);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// This is a MessageBox.Show invocation that displays a dialog with positive and negative buttons
        /// </summary>
        /// <param name="parent">Parent of the dialog</param>
        /// <param name="message">Text to show in dialog</param>
        /// <param name="title">Title of dialog</param>
        /// <param name="positive">Text on positive button of dialog</param>
        /// <param name="negative">Text on negative button of dialog</param>
        /// <returns>Returns true when positive button is clicked and false when negative button is clicked</returns>
        public static Task<bool> Show(Window? parent, string title, string message, string positive, string negative)
        {
            (TaskCompletionSource<bool> taskCompletionSource, MessageBox messageBox) = CreateBoolMessageBox(parent, title, message);

            messageBox.AddButton(positive, true);
            messageBox.AddButton(negative, false, true);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// This is a MessageBox.Show invocation that displays a dialog with only one button
        /// </summary>
        /// <param name="parent">Parent of the dialog</param>
        /// <param name="title">Title of dialog</param>
        /// <param name="message">Text to show in dialog</param>
        /// <param name="button">Text on button of dialog</param>
        /// <returns>True when button is clicked</returns>
        public static Task<bool> Show(Window? parent, string title, string message, string button = "Cancel")
        {
            (TaskCompletionSource<bool> taskCompletionSource, MessageBox messageBox) = CreateBoolMessageBox(parent, title, message);

            messageBox.AddButton(button, true, true);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// This is a MessageBox.Show invocation that displays a dialog with only one button
        /// </summary>
        /// <param name="parent">Parent of the dialog</param>
        /// <param name="title">Title of dialog</param>
        /// <param name="message">Text to show in dialog</param>
        /// <param name="button">Text on button of dialog</param>
        /// <returns>True when button is clicked</returns>
        public static Task<bool> Show(Window? parent, CustomIconType icontype, string title, string message, string button = "Cancel", Color? color = null)
        {
            (TaskCompletionSource<bool> taskCompletionSource, MessageBox messageBox) = CreateBoolMessageBox(parent, title, message);

            messageBox.FindControl<Image>("AlertIcon").Source       = CustomIcon.GenerateIcon(icontype, color ?? Colors.White);
            messageBox.FindControl<Image>("AlertIcon").IsVisible    = true;
            messageBox.AddButton(button, true, true);

            return taskCompletionSource.Task;
        }

        public static async Task<bool?> Show(Window? parent, string title, string message, string positive, string negative, string neutral, Action onDismiss)
        {
            bool? result = await Show(parent, title, message, positive, negative, neutral);
            onDismiss.Invoke();
            return result;
        }

        public enum DialogType
        {
            Success,
            Warning,
            Error
        }

        private static void ConfigureIconAndSound(string title, MessageBox messageBox)
        {
            if (title.ToUpper().Trim().Contains("SUCCESS"))
            {
                SetIcon(DialogType.Success, messageBox);
            }
            else if (title.ToUpper().Trim().Contains("ERROR"))
            {
                SetIcon(DialogType.Error, messageBox);
            }
            else if (title.ToUpper().Trim().Contains("WARNING"))
            {
                SetIcon(DialogType.Warning, messageBox);
            }
        }

        private static readonly DrawingImage s_error    = CustomIcon.GenerateIcon(CustomIconType.ErrorCircular, Colors.Red);
        private static readonly DrawingImage s_warning  = CustomIcon.GenerateIcon(CustomIconType.WarningTriangular, Colors.Orange);
        private static readonly DrawingImage s_success  = CustomIcon.GenerateIcon(CustomIconType.CheckCircular, Colors.Lime);

        public static async Task<bool> Show(Window? owner, Exception error, string? buttonText = null) => await Show(owner, "Error", error.Message, buttonText ?? "Exit");

        private static void SetIcon(DialogType type, MessageBox messageBox)
        {
            switch (type)
            {
                case DialogType.Success:
                    messageBox.FindControl<Image>("AlertIcon").Source = s_success;
                    break;
                case DialogType.Warning:
                    messageBox.FindControl<Image>("AlertIcon").Source = s_warning;
                    break;
                case DialogType.Error:
                    messageBox.FindControl<Image>("AlertIcon").Source = s_error;
                    break;
            }

            messageBox.FindControl<Image>("AlertIcon").IsVisible = true;
        }

        public static Task<(T? item, int? index, string? displayText)> PickerDialog<T>(Window? parent, string title, string prompt, string label, List<T> items, T? selectedItem, string? placeholder = null, string? selectButton = null, string? cancelButton = null)
        {
            (TaskCompletionSource<(T? item, int? index, string? displayText)> taskCompletionSource, MessageBox messageBox)
                = CreatePickerMessageBox(parent, title, prompt, items, selectedItem, label: label, placeholder: placeholder);

            messageBox.AddButton(selectButton ?? "Select", objectButtonResult: messageBox.FinalObjectResult);

            Button? button = new() { Content = cancelButton ?? "Cancel", MinWidth = 75 };
            StackPanel? buttonPanel = messageBox.FindControl<StackPanel>("Buttons");
            button.Click += (_, __) => {
                GetComboBox(messageBox).SelectedItem    = null;
                GetComboBox(messageBox).SelectedIndex   = -1;

                messageBox.Close();
            };
            buttonPanel.Children.Add(button);
            messageBox.FinalObjectResult = null;

            return taskCompletionSource.Task;
        }

        public static Task<string?> InputDialog(Window? parent, string title, string prompt, string positive, string negative, string? placeholder = null)
        {
            (TaskCompletionSource<string?> input, MessageBox messageBox) = CreateInputMessageBox(parent, title, prompt, placeholder);

            messageBox.AddButton(positive, messageBox.FinalStringResult);
            messageBox.AddButton(negative, string.Empty);

            return input.Task;
        }


        public static async Task<bool> Show(Window? parent, string title, string message, string positive, string negative, Action onDismiss)
        {
            bool result = await Show(parent, title, message, positive, negative);
            onDismiss.Invoke();
            return result;
        }

        private static ComboBox GetComboBox(MessageBox messageBox) => messageBox.FindControl<Grid>("ComboBoxGrid").FindControl<ComboBox>("ComboBox");

        private static (TaskCompletionSource<(T? item, int? index, string? displayText)> taskCompletionSource, MessageBox messageBox) CreatePickerMessageBox<T>(
            Window? parent,
            string title,
            string prompt,
            List<T> items,
            T? selectedItem,
            int? selectedItemIndex = null,
            string? label = null,
            string? placeholder = null)
        {
            MessageBox messageBox = new()
            {
                Title = title
            };

            messageBox.FindControl<TextBlock>("Text").Text = prompt;
            messageBox.FinalObjectResult = selectedItem;

            Grid comboBoxGrid = messageBox.FindControl<Grid>("ComboBoxGrid");
            comboBoxGrid.IsVisible = true;

            comboBoxGrid.FindControl<TextBlock>("ComboBoxLabel").IsVisible = string.IsNullOrWhiteSpace(label) == false;
            comboBoxGrid.FindControl<TextBlock>("ComboBoxLabel").Text      = label;

            GetComboBox(messageBox).IsVisible = true;
            GetComboBox(messageBox).Items = items;
            GetComboBox(messageBox).PlaceholderText = placeholder;

            if (selectedItem != null && items.Contains(selectedItem))
            {
                GetComboBox(messageBox).SelectedItem = selectedItem;
            }
            else if (selectedItemIndex is not null && selectedItemIndex < items.Count)
            {
                GetComboBox(messageBox).SelectedIndex = (int)selectedItemIndex;
            }


            TaskCompletionSource<(T? item, int? index, string? displayText)>? taskCompletionSource = new();

            messageBox.Closed += delegate
            {
                bool notSelected = GetComboBox(messageBox).SelectedIndex == -1;
                T? item = notSelected ? default : items[GetComboBox(messageBox).SelectedIndex];

                _ = taskCompletionSource.TrySetResult((
                    item,
                    GetComboBox(messageBox).SelectedIndex,
                    item?.ToString()
                ));
            };

            messageBox.KeyDown += delegate(object? _, Avalonia.Input.KeyEventArgs e)
            {
                if (e.Key == Avalonia.Input.Key.Enter)
                {
                    messageBox.Close();
                }
            };

            ConfigureIconAndSound(title, messageBox);

            if (parent is not null)
            {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                _ = messageBox.ShowDialog(parent);
            }
            else
            {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                messageBox.Show();
            }

            return (taskCompletionSource, messageBox);
        }

        private static (TaskCompletionSource<string?> taskCompletionSource, MessageBox messageBox) CreateInputMessageBox(Window? parent, string title, string prompt, string? placeholder = null)
        {
            MessageBox messageBox = new()
            {
                Title = title
            };

            messageBox.FindControl<TextBlock>("Text").Text = prompt;
            messageBox.FinalStringResult = null;

            messageBox.FindControl<TextBox>("TextEntry").IsVisible  = true;
            messageBox.FindControl<TextBox>("TextEntry").Watermark  = placeholder;
            messageBox.FindControl<TextBox>("TextEntry").KeyDown    += delegate(object? _, Avalonia.Input.KeyEventArgs e)
            {
                if (e.Key == Avalonia.Input.Key.Enter)
                {
                    messageBox.Close();
                }
            };

            TaskCompletionSource<string?>? taskCompletionSource = new();

            messageBox.Closed += delegate
            {
                _ = taskCompletionSource.TrySetResult(messageBox.FinalStringResult);
            };

            messageBox.KeyDown += delegate(object? _, Avalonia.Input.KeyEventArgs e)
            {
                if (e.Key == Avalonia.Input.Key.Enter)
                {
                    messageBox.Close();
                }
            };

            ConfigureIconAndSound(title, messageBox);

            if (parent is not null)
            {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                _ = messageBox.ShowDialog(parent);
            }
            else
            {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                messageBox.Show();
            }

            return (taskCompletionSource, messageBox);
        }

        private static (TaskCompletionSource<bool?> taskCompletionSource, MessageBox messageBox) CreateNullableBoolMessageBox(Window? parent, string title, string message)
        {
            MessageBox messageBox = new()
            {
                Title = title
            };

            messageBox.FindControl<TextBlock>("Text").Text = message;
            messageBox.FinalBoolResult = false;

            TaskCompletionSource<bool?>? taskCompletionSource = new();

            messageBox.Closed += delegate
            {
                _ = taskCompletionSource.TrySetResult(messageBox.FinalBoolResult);
            };

            ConfigureIconAndSound(title, messageBox);

            if (parent is not null)
            {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                _ = messageBox.ShowDialog(parent);
            }
            else
            {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                messageBox.Show();
            }

            return (taskCompletionSource, messageBox);
        }

        private static (TaskCompletionSource<bool> taskCompletionSource, MessageBox messageBox) CreateBoolMessageBox(Window? parent, string title, string message)
        {
            MessageBox messageBox = new()
            {
                Title = title
            };

            messageBox.FindControl<TextBlock>("Text").Text = message;
            messageBox.FinalBoolResult = false;

            TaskCompletionSource<bool>? taskCompletionSource = new();

            messageBox.Closed += delegate
            {
                _ = taskCompletionSource.TrySetResult(messageBox.FinalBoolResult ?? false);
            };

            ConfigureIconAndSound(title, messageBox);

            if (parent is not null)
            {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                _ = messageBox.ShowDialog(parent);
            }
            else
            {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                messageBox.Show();
            }

            return (taskCompletionSource, messageBox);
        }
    }
}

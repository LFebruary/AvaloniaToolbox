using Avalonia.Controls;
using Avalonia.Threading;
using AvaloniaToolbox.UI;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AvaloniaToolbox.Functions
{
    public static partial class Extensions
    {
        public static List<T> EnumValues<T>() where T: Enum
        {
            return new((T[])Enum.GetValues(typeof(T)));
        }

        public static Dictionary<string, object?> GetPropertiesDictionary<T>(this T @object) where T: notnull
        {
            Dictionary<string, object?> dict = new();
            foreach(System.Reflection.PropertyInfo property in @object.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                dict.Add(property.Name, property.GetValue(@object));
            }
            return dict;
        }

        public static Dictionary<TKey, TValue?> Join<TKey, TValue>(this IDictionary<TKey, TValue?> dictionary, params IDictionary<TKey, TValue?>[] others) where TKey: notnull
        {
            Dictionary<TKey, TValue?> newMap = new();
            foreach (IDictionary<TKey,TValue?> src in
                new List<IDictionary<TKey, TValue?>> { dictionary }.Concat(others)) {
                foreach (KeyValuePair<TKey, TValue?> p in src) {
                    newMap[p.Key] = p.Value;
                }
            }
            return newMap;
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool None<TSource>(this IEnumerable<TSource>? source) => source == null || source.Any() == false;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool None<TSource>(this IEnumerable<TSource>? source, Func<TSource, bool> predicate) => source == null || source.Any(predicate) == false;

        /// <summary>
        /// Method that takes <paramref name="url"/> and tries to open it in the default browser for the platform
        /// </summary>
        /// <param name="url">Link to open in the browser</param>
        /// <exception cref="ArgumentException"><paramref name="url"/> is in an invalid format</exception>
        public static void OpenLink(string url) => _ = OpenLinkAndReturnProcess(url);

        public static Process? OpenLinkAndReturnProcess(string url)
        {

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                try
                {
                    return Process.Start(url);
                }
                catch
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        url = url.Replace("&", "^&");
                        return Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        return Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        return Process.Start("open", url);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Provided URL is in an invalid format");
            }
        }

        /// <summary>
        /// Dispatcher abstraction method that posts an action to the UI-thread
        /// </summary>
        /// <param name="action">Action to run on the UI-thread</param>
        /// <param name="dispatcherPriority">Priority of job on the UI-thread's dispatcher</param>
        public static void RunOnUIThread(Action action, DispatcherPriority? dispatcherPriority = null)
            => Dispatcher.UIThread.Post(action, dispatcherPriority ?? DispatcherPriority.Normal);

        /// <summary>
        /// Dispatcher abstraction method that posts a task to the UI-thread
        /// </summary>
        /// <param name="taskToRun">Task to run on the UI-thread</param>
        /// <param name="dispatcherPriority">Priority of job on the UI-thread's dispatcher</param>
        public static void RunOnUIThread(Func<Task> taskToRun, DispatcherPriority? dispatcherPriority = null)
            => Dispatcher.UIThread.Post(async () => await taskToRun(), dispatcherPriority ?? DispatcherPriority.Normal);

        /// <summary>
        /// Dispatcher abstraction method that invokes a Task on the UI-thread
        /// </summary>
        /// <typeparam name="T">Type that is expected to return from task</typeparam>
        /// <param name="taskToRun">Task to run on the UI-thread</param>
        /// <param name="dispatcherPriority">Priority of job on the UI-thread's dispatcher</param>
        /// <returns>A task that represents a proxy for the task returned by <paramref name="taskToRun"/> </returns>
        public static async Task<T> RunOnUIThread<T>(Func<Task<T>> taskToRun, DispatcherPriority? dispatcherPriority = null)
            => await Dispatcher.UIThread.InvokeAsync(taskToRun, dispatcherPriority ?? DispatcherPriority.Normal);

        /// <summary>
        /// Dispatcher abstraction method that invokes a Task on the UI-thread
        /// </summary>
        /// <param name="taskToRun"></param>
        /// <param name="dispatcherPriority"></param>
        /// <returns>A task that represents a proxy for the task returned by <paramref name="taskToRun"/> </returns>
        public static async Task InvokeOnUiThread(Task taskToRun, DispatcherPriority? dispatcherPriority = null)
            => await Dispatcher.UIThread.InvokeAsync(async () => await taskToRun, dispatcherPriority ?? DispatcherPriority.Normal);

        public static async Task InvokeOnUiThread(Func<Task> taskToRun, DispatcherPriority? dispatcherPriority = null)
            => await Dispatcher.UIThread.InvokeAsync(async () => await taskToRun(), dispatcherPriority ?? DispatcherPriority.Normal);

        /// <summary>
        /// A custom FirstOrDefault implementation that provides an out parameter for the matching predicate
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">An IEnumerable to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <param name="result">First element that matches predicate on source</param>
        /// <returns>True if matching element found, false if no matching element found</returns>
        public static bool FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out TSource? result)
        {
            if (source.Any(predicate))
            {
                result = source.First(predicate);
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// This method runs a delay and task alongside each other and checks which task finishes first as a custom timeout
        /// </summary>
        /// <typeparam name="T">The type of task</typeparam>
        /// <param name="task">The task which should throw a timeout after specified timespan</param>
        /// <param name="timeoutAction">Action to be invoked if task times out</param>
        /// <param name="timeSpan">Timespan in which task should finish</param>
        /// <returns>True when task times out and false if task finishes in time</returns>
        private static async Task<bool> OnTimeout<T>(this T task, Action<T> timeoutAction, TimeSpan timeSpan) where T : Task
        {
            Task? timeout = Task.Delay(timeSpan);

            if (await Task.WhenAny(task, timeout) == timeout)
            {
                timeoutAction(task);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method runs a delay and task alongside each other and checks which task finishes first as a custom timeout <br/>
        /// This method runs the task without returning a result
        /// </summary>
        /// <typeparam name="T">The type of task</typeparam>
        /// <param name="task">The task which should throw a timeout after specified timespan</param>
        /// <param name="timeoutAction">Action to be invoked if task times out</param>
        /// <param name="timeSpan">Timespan in which task should finish</param>
        //public static async Task OnTimeoutWithoutResult<T>(this T task, Action<T> timeoutAction, TimeSpan timeSpan) where T : Task
        //{
        //    Task? timeout = Task.Delay(timeSpan);

        //    if (await Task.WhenAny(task, timeout) == timeout)
        //    {
        //        timeoutAction(task);
        //    }
        //}

        /// <summary>
        /// This method runs a delay and task alongside each other and checks which task finishes first as a custom timeout
        /// </summary>
        /// <typeparam name="T">The type of task</typeparam>
        /// <param name="task">The task which should throw a timeout after specified timespan</param>
        /// <param name="timeSpan">Timespan in which task should finish</param>
        /// <exception cref="TimeoutException">If task does not finish in time, a timeout exception is thrown</exception>
        /// <returns>True when task times out and false if task finishes in time</returns>
        public static async Task<bool> OnTimeout<T>(this T task, TimeSpan timeSpan) where T : Task
        {
            return await task.OnTimeout(_ => throw new TimeoutException("OnTimeout provided task did not complete in specified time span"), timeSpan);
        }

        /// <summary>
        /// This method is used to add button on <paramref name="messageBox"/> and <paramref name="boolButtonResult"/> associated with button
        /// </summary>
        /// <param name="messageBox">MessageBox to add button to</param>
        /// <param name="buttonText">Text to appear on button</param>
        /// <param name="boolButtonResult">Result returned by clicking button</param>
        /// <param name="isDefault">Whether this button's result should return by default</param>
        public static void AddButton(this MessageBox messageBox, string buttonText, bool? boolButtonResult, bool isDefault = false)
        {
            Button? button = new() { Content = buttonText, MinWidth = 75 };
            StackPanel? buttonPanel = messageBox.FindControl<StackPanel>("Buttons");

            button.Click += (_, __) =>
            {
                messageBox.FinalBoolResult = boolButtonResult;
                messageBox.Close();
            };

            buttonPanel.Children.Add(button);

            if (isDefault)
            {
                messageBox.FinalBoolResult = boolButtonResult;
            }
        }

        /// <summary>
        /// This method is used to add button on <paramref name="messageBox"/> and <paramref name="stringButtonResult"/> associated with button
        /// </summary>
        /// <param name="messageBox">MessageBox to add button to</param>
        /// <param name="buttonText">Text to appear on button</param>
        /// <param name="stringButtonResult">Result returned by clicking button</param>
        /// <param name="isDefault">Whether this button's result should return by default</param>
        internal static void AddButton(this MessageBox messageBox, string buttonText, string? stringButtonResult, bool isDefault = false)
        {
            Button? button = new() { Content = buttonText, MinWidth = 75 };
            StackPanel? buttonPanel = messageBox.FindControl<StackPanel>("Buttons");

            button.Click += (_, __) =>
            {
                messageBox.FinalStringResult = stringButtonResult;
                messageBox.Close();
            };

            buttonPanel.Children.Add(button);

            if (isDefault)
            {
                messageBox.FinalStringResult = stringButtonResult;
            }
        }


        /// <summary>
        /// This method is used to add button on <paramref name="messageBox"/> and <paramref name="objectButtonResult"/> associated with button
        /// </summary>
        /// <param name="messageBox">MessageBox to add button to</param>
        /// <param name="buttonText">Text to appear on button</param>
        /// <param name="objectButtonResult">Result returned by clicking button</param>
        /// <param name="isDefault">Whether this button's result should return by default</param>
        internal static void AddButton(this MessageBox messageBox, string buttonText, object? objectButtonResult, bool isDefault = false)
        {
            Button? button = new() { Content = buttonText, MinWidth = 75 };
            StackPanel? buttonPanel = messageBox.FindControl<StackPanel>("Buttons");

            button.Click += (_, __) =>
            {
                messageBox.FinalObjectResult = objectButtonResult;
                messageBox.Close();
            };

            buttonPanel.Children.Add(button);

            if (isDefault)
            {
                messageBox.FinalObjectResult = objectButtonResult;
            }
        }

        internal static Task FadeIn<T>(this T view, TimeSpan timespan, double startingOpacity = 0.0, double finalOpacity = 1.0, Action? actionBeforeFadeIn = null, Func<bool?>? interrupt = null) where T: IControl
        {
            Debug.Assert(startingOpacity >= 0.0);
            Debug.Assert(finalOpacity > 0.0);
            Debug.Assert(finalOpacity > startingOpacity);
            Debug.Assert(timespan.TotalMilliseconds > 0);

            TimeSpan wait   = TimeSpan.FromMilliseconds(timespan.TotalMilliseconds / 100);
            double step     = (finalOpacity - startingOpacity) / 100;

            return InvokeOnUiThread(async () =>
            {
                view.Opacity = startingOpacity;

                if (actionBeforeFadeIn is not null)
                {
                    actionBeforeFadeIn();
                }

                if (interrupt is not null)
                {
                    do
                    {
                        await Task.Delay(wait);
                        view.Opacity += step;
                    }
                    while (view.Opacity < finalOpacity && interrupt() == false);
                }
                else
                {
                    do
                    {
                        await Task.Delay(wait);
                        view.Opacity += step;
                    }
                    while (view.Opacity < finalOpacity);
                }

                view.Opacity = finalOpacity;
            });
        }

        internal static Task FadeOut<T>(this T view, TimeSpan timespan, double startingOpacity = 1.0, double finalOpacity = 0.0, Action<T>? actionBeforeFadeOut = null, Func<bool?>? interrupt = null) where T: IControl
        {
            Debug.Assert(startingOpacity > 0.0);
            Debug.Assert(finalOpacity >= 0.0);
            Debug.Assert(startingOpacity > finalOpacity);
            Debug.Assert(timespan.TotalMilliseconds > 0);

            TimeSpan wait   = TimeSpan.FromMilliseconds(timespan.TotalMilliseconds / 100);
            double step     = (startingOpacity - finalOpacity) / 100;

            return InvokeOnUiThread(async () =>
            {
                view.Opacity = startingOpacity;
                if (actionBeforeFadeOut is not null)
                {
                    actionBeforeFadeOut.Invoke(view);
                }

                if (interrupt is not null)
                {
                    do
                    {
                        await Task.Delay(wait);
                        view.Opacity -= step;
                    }
                    while (view.Opacity > finalOpacity && interrupt() == false);
                }
                else
                {
                    do
                    {
                        await Task.Delay(wait);
                        view.Opacity -= step;
                    }
                    while (view.Opacity > finalOpacity);
                }

                view.Opacity = finalOpacity;
            });
        }
    }
}

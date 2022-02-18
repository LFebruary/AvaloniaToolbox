using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using Avalonia.Controls.ApplicationLifetimes;

namespace AvaloniaToolbox.Functions
{
    public static partial class ViewFunctions
    {
        #region Resource accessors
        /// <summary>
        /// Method that takes <paramref name="customColor"/> + <paramref name="isDarkMode"/> and gets matching color brush
        /// </summary>
        /// <param name="customColor">Custom defined color</param>
        /// <param name="isDarkMode">Whether the dark mode or light mode variant of the color should be used</param>
        /// <returns>Brush instance created from app resource color</returns>
        internal static SolidColorBrush GetColorBrush(string key) => GetResource<SolidColorBrush>(key);



        /// <summary>
        /// Method that takes <paramref name="customColor"/> + <paramref name="isDarkMode"/> and gets matching color type
        /// </summary>
        /// <param name="customColor">Custom defined color</param>
        /// <param name="isDarkMode">Whether the dark mode or light mode variant of the color should be used</param>
        /// <returns>Color instance determined from defined app resource colors</returns>
        /// <exception cref="NullReferenceException">No matching resource found in application resources</exception>
        public static Color GetColor(string key)
        {
            if (Application.Current?.TryFindResource(key, out object? resource) == true)
            {
                if (resource is not null and object castedRes)
                {
                    return (Color)castedRes;
                }
            }

            throw new NullReferenceException("GetColor returned null");
        }

        /// <summary>
        /// Method that takes <paramref name="key"/> + <typeparamref name="TResource"/> and finds matching resource from defined application resources
        /// </summary>
        /// <typeparam name="TResource">Type to cast application resource to</typeparam>
        /// <param name="key">Key of resource in application resources</param>
        /// <returns>Resource matching provided key and type</returns>
        /// <exception cref="NullReferenceException">No matching resource found in application resources</exception>
        private static TResource GetResource<TResource>(string key) where TResource : class
        {
            if (Application.Current?.TryFindResource(key, out object? resource) == true)
            {
                if (resource is not null and object castedRes)
                {
                    return (TResource)castedRes;
                }
            }

            throw new NullReferenceException("GetResource returned null");
        }
        #endregion


        #region Imaging

        /// <summary>
        /// Converts provided byte array to Bitmap
        /// </summary>
        /// <param name="buffer">Buffer to convert</param>
        /// <returns>Bitmap created from <paramref name="buffer"/> </returns>
        internal static Bitmap GetImageBitmap(byte[] buffer)
        {
            using MemoryStream memoryStream = new(buffer);
            return new(memoryStream);
        }

        #endregion

        #region Windows
        private static IClassicDesktopStyleApplicationLifetime ClassicDesktopStyleApplicationLifetime => (IClassicDesktopStyleApplicationLifetime?)Application.Current?.ApplicationLifetime
            ?? throw new NullReferenceException($"{nameof(ClassicDesktopStyleApplicationLifetime)} can not be null.");

        /// <summary>
        /// Finds window in windows stack in App that matches <typeparamref name="TWindow"/> and returns it
        /// </summary>
        /// <typeparam name="TWindow">Type of window to find in windows stack</typeparam>
        /// <returns>Window that matches <typeparamref name="TWindow"/>, null if no match found</returns>
        public static Window? FindWindow<TWindow>() where TWindow : Window, new() => Windows.OfType<TWindow>().SingleOrDefault();

        public static IEnumerable<Window> Windows => ClassicDesktopStyleApplicationLifetime.Windows;

        /// <summary>
        /// This method takes <typeparamref name="TWindow"/> and opens new instance of window if there is not any existing instance of <typeparamref name="TWindow"/> in the window stack <br/>
        /// Otherwise it just activates the existing instance of the matching window in the stack
        /// </summary>
        /// <typeparam name="TWindow">Type of window to open or restore</typeparam>
        /// <param name="ownerWindow">Owner window in case window opens as dialog</param>
        public static void OpenNewOrRestoreWindow<TWindow>(Window? ownerWindow = null) where TWindow : Window, new()
        {
            Window? window = FindWindow<TWindow>();

            if (window is not null)
            {
                window.Activate();
            }
            else
            {
                TWindow newwindow = new();

                if (ownerWindow is not null)
                {
                    newwindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    _ = newwindow.ShowDialog(ownerWindow);
                }
                else
                {
                    newwindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    newwindow.Show();
                }
            }
        }

        #endregion
    }
}

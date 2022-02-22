using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace AvaloniaToolbox.UI
{
    /// <summary>
    /// Custom Image class with added CustomIconType and Color properties for use adjusting image sources to matching icons.
    /// </summary>
    public partial class CustomIcon : Image
    {
        #region Geometry constants
        private const string AlertCircleOutlineGeometry         = "M11,15H13V17H11V15M11,7H13V13H11V7M12,2C6.47,2 2,6.5 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,20A8,8 0 0,1 4,12A8,8 0 0,1 12,4A8,8 0 0,1 20,12A8,8 0 0,1 12,20Z";
        private const string AlertOutlineGeometry               = "M12,2L1,21H23M12,6L19.53,19H4.47M11,10V14H13V10M11,16V18H13V16";
        private const string ArrowLeftCircleOutlineGeometry     = "M18,11V13H10L13.5,16.5L12.08,17.92L6.16,12L12.08,6.08L13.5,7.5L10,11H18M2,12A10,10 0 0,1 12,2A10,10 0 0,1 22,12A10,10 0 0,1 12,22A10,10 0 0,1 2,12M4,12A8,8 0 0,0 12,20A8,8 0 0,0 20,12A8,8 0 0,0 12,4A8,8 0 0,0 4,12Z";
        private const string ArrowRightCircleOutlineGeometry    = "M6,13V11H14L10.5,7.5L11.92,6.08L17.84,12L11.92,17.92L10.5,16.5L14,13H6M22,12A10,10 0 0,1 12,22A10,10 0 0,1 2,12A10,10 0 0,1 12,2A10,10 0 0,1 22,12M20,12A8,8 0 0,0 12,4A8,8 0 0,0 4,12A8,8 0 0,0 12,20A8,8 0 0,0 20,12Z";
        private const string BackspaceOutlineGeometry           = "M19,15.59L17.59,17L14,13.41L10.41,17L9,15.59L12.59,12L9,8.41L10.41,7L14,10.59L17.59,7L19,8.41L15.41,12L19,15.59M22,3A2,2 0 0,1 24,5V19A2,2 0 0,1 22,21H7C6.31,21 5.77,20.64 5.41,20.11L0,12L5.41,3.88C5.77,3.35 6.31,3 7,3H22M22,5H7L2.28,12L7,19H22V5Z";
        private const string BroadcastGeometry                  = "M12 10C10.9 10 10 10.9 10 12S10.9 14 12 14 14 13.1 14 12 13.1 10 12 10M18 12C18 8.7 15.3 6 12 6S6 8.7 6 12C6 14.2 7.2 16.1 9 17.2L10 15.5C8.8 14.8 8 13.5 8 12.1C8 9.9 9.8 8.1 12 8.1S16 9.9 16 12.1C16 13.6 15.2 14.9 14 15.5L15 17.2C16.8 16.2 18 14.2 18 12M12 2C6.5 2 2 6.5 2 12C2 15.7 4 18.9 7 20.6L8 18.9C5.6 17.5 4 14.9 4 12C4 7.6 7.6 4 12 4S20 7.6 20 12C20 15 18.4 17.5 16 18.9L17 20.6C20 18.9 22 15.7 22 12C22 6.5 17.5 2 12 2Z";
        private const string BroadcastOffGeometry               = "M17.6 14.2C17.9 13.5 18 12.8 18 12C18 8.7 15.3 6 12 6C11.2 6 10.4 6.2 9.8 6.4L11.4 8H12C14.2 8 16 9.8 16 12C16 12.2 16 12.4 15.9 12.6L17.6 14.2M12 4C16.4 4 20 7.6 20 12C20 13.4 19.6 14.6 19 15.7L20.5 17.2C21.4 15.7 22 13.9 22 12C22 6.5 17.5 2 12 2C10.1 2 8.3 2.5 6.8 3.5L8.3 5C9.4 4.3 10.6 4 12 4M3.3 2.5L2 3.8L4.1 5.9C2.8 7.6 2 9.7 2 12C2 15.7 4 18.9 7 20.6L8 18.9C5.6 17.5 4 14.9 4 12C4 10.2 4.6 8.6 5.5 7.3L7 8.8C6.4 9.7 6 10.8 6 12C6 14.2 7.2 16.1 9 17.2L10 15.5C8.8 14.8 8 13.5 8 12.1C8 11.5 8.2 10.9 8.4 10.3L10 11.9V12.1C10 13.2 10.9 14.1 12 14.1H12.2L19.7 21.6L21 20.3L4.3 3.5L3.3 2.5Z";
        private const string CheckCircleOutlineGeometry         = "M12 2C6.5 2 2 6.5 2 12S6.5 22 12 22 22 17.5 22 12 17.5 2 12 2M12 20C7.59 20 4 16.41 4 12S7.59 4 12 4 20 7.59 20 12 16.41 20 12 20M16.59 7.58L10 14.17L7.41 11.59L6 13L10 17L18 9L16.59 7.58Z";
        private const string ContentSaveSettingsGeometry        = "M15,8V4H5V8H15M12,18A3,3 0 0,0 15,15A3,3 0 0,0 12,12A3,3 0 0,0 9,15A3,3 0 0,0 12,18M17,2L21,6V18A2,2 0 0,1 19,20H5C3.89,20 3,19.1 3,18V4A2,2 0 0,1 5,2H17M11,22H13V24H11V22M7,22H9V24H7V22M15,22H17V24H15V22Z";
        private const string DeleteOutlineGeometry              = "M6,19A2,2 0 0,0 8,21H16A2,2 0 0,0 18,19V7H6V19M8,9H16V19H8V9M15.5,4L14.5,3H9.5L8.5,4H5V6H19V4H15.5Z";
        private const string FilterGeometry                     = "M14,12V19.88C14.04,20.18 13.94,20.5 13.71,20.71C13.32,21.1 12.69,21.1 12.3,20.71L10.29,18.7C10.06,18.47 9.96,18.16 10,17.87V12H9.97L4.21,4.62C3.87,4.19 3.95,3.56 4.38,3.22C4.57,3.08 4.78,3 5,3V3H19V3C19.22,3 19.43,3.08 19.62,3.22C20.05,3.56 20.13,4.19 19.79,4.62L14.03,12H14Z";
        private const string HamburgerMenuGeometry              = "M3,6H21V8H3V6M3,11H21V13H3V11M3,16H21V18H3V16Z";
        private const string HistoryGeometry                    = "M13.5,8H12V13L16.28,15.54L17,14.33L13.5,12.25V8M13,3A9,9 0 0,0 4,12H1L4.96,16.03L9,12H6A7,7 0 0,1 13,5A7,7 0 0,1 20,12A7,7 0 0,1 13,19C11.07,19 9.32,18.21 8.06,16.94L6.64,18.36C8.27,20 10.5,21 13,21A9,9 0 0,0 22,12A9,9 0 0,0 13,3";
        private const string InformationGeometry                = "M13,9H11V7H13M13,17H11V11H13M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2Z";
        private const string LinkedInGeometry                   = "M19 3A2 2 0 0 1 21 5V19A2 2 0 0 1 19 21H5A2 2 0 0 1 3 19V5A2 2 0 0 1 5 3H19M18.5 18.5V13.2A3.26 3.26 0 0 0 15.24 9.94C14.39 9.94 13.4 10.46 12.92 11.24V10.13H10.13V18.5H12.92V13.57C12.92 12.8 13.54 12.17 14.31 12.17A1.4 1.4 0 0 1 15.71 13.57V18.5H18.5M6.88 8.56A1.68 1.68 0 0 0 8.56 6.88C8.56 5.95 7.81 5.19 6.88 5.19A1.69 1.69 0 0 0 5.19 6.88C5.19 7.81 5.95 8.56 6.88 8.56M8.27 18.5V10.13H5.5V18.5H8.27Z";
        private const string LockGeometry                       = "M12,17A2,2 0 0,0 14,15C14,13.89 13.1,13 12,13A2,2 0 0,0 10,15A2,2 0 0,0 12,17M18,8A2,2 0 0,1 20,10V20A2,2 0 0,1 18,22H6A2,2 0 0,1 4,20V10C4,8.89 4.9,8 6,8H7V6A5,5 0 0,1 12,1A5,5 0 0,1 17,6V8H18M12,3A3,3 0 0,0 9,6V8H15V6A3,3 0 0,0 12,3Z";
        private const string LockOpenGeometry                   = "M18,8A2,2 0 0,1 20,10V20A2,2 0 0,1 18,22H6C4.89,22 4,21.1 4,20V10A2,2 0 0,1 6,8H15V6A3,3 0 0,0 12,3A3,3 0 0,0 9,6H7A5,5 0 0,1 12,1A5,5 0 0,1 17,6V8H18M12,17A2,2 0 0,0 14,15A2,2 0 0,0 12,13A2,2 0 0,0 10,15A2,2 0 0,0 12,17Z";
        private const string MaintenanceGeometry                = "M10 6.2C10 4.3 8.8 2.6 7 2V5.7H4V2C2.2 2.6 1 4.3 1 6.2C1 8.1 2.2 9.8 4 10.4V21.4C4 21.8 4.2 22 4.5 22H6.5C6.8 22 7 21.8 7 21.5V10.5C8.8 9.9 10 8.2 10 6.2M16 8C16 8 15.9 8 16 8C12.1 8.1 9 11.2 9 15C9 18.9 12.1 22 16 22S23 18.9 23 15 19.9 8 16 8M16 20C13.2 20 11 17.8 11 15S13.2 10 16 10 21 12.2 21 15 18.8 20 16 20M15 11V16L18.6 18.2L19.4 17L16.5 15.3V11H15Z";
        private const string PlusCircleOutlineGeometry          = "M12,20C7.59,20 4,16.41 4,12C4,7.59 7.59,4 12,4C16.41,4 20,7.59 20,12C20,16.41 16.41,20 12,20M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M13,7H11V11H7V13H11V17H13V13H17V11H13V7Z";
        private const string QrCodeGeometry                     = "M3,11H5V13H3V11M11,5H13V9H11V5M9,11H13V15H11V13H9V11M15,11H17V13H19V11H21V13H19V15H21V19H19V21H17V19H13V21H11V17H15V15H17V13H15V11M19,19V15H17V19H19M15,3H21V9H15V3M17,5V7H19V5H17M3,3H9V9H3V3M5,5V7H7V5H5M3,15H9V21H3V15M5,17V19H7V17H5Z";
        private const string RestoreGeometry                    = "M13,3A9,9 0 0,0 4,12H1L4.89,15.89L4.96,16.03L9,12H6A7,7 0 0,1 13,5A7,7 0 0,1 20,12A7,7 0 0,1 13,19C11.07,19 9.32,18.21 8.06,16.94L6.64,18.36C8.27,20 10.5,21 13,21A9,9 0 0,0 22,12A9,9 0 0,0 13,3Z";
        private const string SerialPortGeometry                 = "M7,3H17V5H19V8H16V14H8V8H5V5H7V3M17,9H19V14H17V9M11,15H13V22H11V15M5,9H7V14H5V9Z";
        private const string SettingsCogGeometry                = "M12,15.5A3.5,3.5 0 0,1 8.5,12A3.5,3.5 0 0,1 12,8.5A3.5,3.5 0 0,1 15.5,12A3.5,3.5 0 0,1 12,15.5M19.43,12.97C19.47,12.65 19.5,12.33 19.5,12C19.5,11.67 19.47,11.34 19.43,11L21.54,9.37C21.73,9.22 21.78,8.95 21.66,8.73L19.66,5.27C19.54,5.05 19.27,4.96 19.05,5.05L16.56,6.05C16.04,5.66 15.5,5.32 14.87,5.07L14.5,2.42C14.46,2.18 14.25,2 14,2H10C9.75,2 9.54,2.18 9.5,2.42L9.13,5.07C8.5,5.32 7.96,5.66 7.44,6.05L4.95,5.05C4.73,4.96 4.46,5.05 4.34,5.27L2.34,8.73C2.21,8.95 2.27,9.22 2.46,9.37L4.57,11C4.53,11.34 4.5,11.67 4.5,12C4.5,12.33 4.53,12.65 4.57,12.97L2.46,14.63C2.27,14.78 2.21,15.05 2.34,15.27L4.34,18.73C4.46,18.95 4.73,19.03 4.95,18.95L7.44,17.94C7.96,18.34 8.5,18.68 9.13,18.93L9.5,21.58C9.54,21.82 9.75,22 10,22H14C14.25,22 14.46,21.82 14.5,21.58L14.87,18.93C15.5,18.67 16.04,18.34 16.56,17.94L19.05,18.95C19.27,19.03 19.54,18.95 19.66,18.73L21.66,15.27C21.78,15.05 21.73,14.78 21.54,14.63L19.43,12.97Z";
        private const string SupportFaceGeometry                = "M18.72,14.76C19.07,13.91 19.26,13 19.26,12C19.26,11.28 19.15,10.59 18.96,9.95C18.31,10.1 17.63,10.18 16.92,10.18C13.86,10.18 11.15,8.67 9.5,6.34C8.61,8.5 6.91,10.26 4.77,11.22C4.73,11.47 4.73,11.74 4.73,12A7.27,7.27 0 0,0 12,19.27C13.05,19.27 14.06,19.04 14.97,18.63C15.54,19.72 15.8,20.26 15.78,20.26C14.14,20.81 12.87,21.08 12,21.08C9.58,21.08 7.27,20.13 5.57,18.42C4.53,17.38 3.76,16.11 3.33,14.73H2V10.18H3.09C3.93,6.04 7.6,2.92 12,2.92C14.4,2.92 16.71,3.87 18.42,5.58C19.69,6.84 20.54,8.45 20.89,10.18H22V14.67H22V14.69L22,14.73H21.94L18.38,18L13.08,17.4V15.73H17.91L18.72,14.76M9.27,11.77C9.57,11.77 9.86,11.89 10.07,12.11C10.28,12.32 10.4,12.61 10.4,12.91C10.4,13.21 10.28,13.5 10.07,13.71C9.86,13.92 9.57,14.04 9.27,14.04C8.64,14.04 8.13,13.54 8.13,12.91C8.13,12.28 8.64,11.77 9.27,11.77M14.72,11.77C15.35,11.77 15.85,12.28 15.85,12.91C15.85,13.54 15.35,14.04 14.72,14.04C14.09,14.04 13.58,13.54 13.58,12.91A1.14,1.14 0 0,1 14.72,11.77Z";
        private const string WebGeometry                        = "M16.36,14C16.44,13.34 16.5,12.68 16.5,12C16.5,11.32 16.44,10.66 16.36,10H19.74C19.9,10.64 20,11.31 20,12C20,12.69 19.9,13.36 19.74,14M14.59,19.56C15.19,18.45 15.65,17.25 15.97,16H18.92C17.96,17.65 16.43,18.93 14.59,19.56M14.34,14H9.66C9.56,13.34 9.5,12.68 9.5,12C9.5,11.32 9.56,10.65 9.66,10H14.34C14.43,10.65 14.5,11.32 14.5,12C14.5,12.68 14.43,13.34 14.34,14M12,19.96C11.17,18.76 10.5,17.43 10.09,16H13.91C13.5,17.43 12.83,18.76 12,19.96M8,8H5.08C6.03,6.34 7.57,5.06 9.4,4.44C8.8,5.55 8.35,6.75 8,8M5.08,16H8C8.35,17.25 8.8,18.45 9.4,19.56C7.57,18.93 6.03,17.65 5.08,16M4.26,14C4.1,13.36 4,12.69 4,12C4,11.31 4.1,10.64 4.26,10H7.64C7.56,10.66 7.5,11.32 7.5,12C7.5,12.68 7.56,13.34 7.64,14M12,4.03C12.83,5.23 13.5,6.57 13.91,8H10.09C10.5,6.57 11.17,5.23 12,4.03M18.92,8H15.97C15.65,6.75 15.19,5.55 14.59,4.44C16.43,5.07 17.96,6.34 18.92,8M12,2C6.47,2 2,6.5 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2Z";
        #endregion

        #region Methods
        /// <summary>
        /// Takes <paramref name="customIconType"/> and <paramref name="color"/> to instantiate icon with appropriate geometry and color
        /// </summary>
        /// <param name="customIconType"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DrawingImage GenerateIcon(CustomIconType? customIconType, Color color, bool flipHorizontally = false) => customIconType switch
        {
            CustomIconType.ArrowLeft            => Icon.From(ArrowLeftCircleOutlineGeometry, color, flipHorizontally),
            CustomIconType.ArrowRight           => Icon.From(ArrowRightCircleOutlineGeometry, color, flipHorizontally),
            CustomIconType.Backspace            => Icon.From(BackspaceOutlineGeometry, color, flipHorizontally),
            CustomIconType.Broadcast            => Icon.From(BroadcastGeometry, color, flipHorizontally),
            CustomIconType.BroadcastOff         => Icon.From(BroadcastOffGeometry, color, flipHorizontally),
            CustomIconType.CheckCircular        => Icon.From(CheckCircleOutlineGeometry, color, flipHorizontally),
            CustomIconType.Delete               => Icon.From(DeleteOutlineGeometry, color, flipHorizontally),
            CustomIconType.ErrorCircular        => Icon.From(AlertCircleOutlineGeometry, color, flipHorizontally),
            CustomIconType.Filter               => Icon.From(FilterGeometry, color, flipHorizontally),
            CustomIconType.History              => Icon.From(HistoryGeometry, color, flipHorizontally),
            CustomIconType.Info                 => Icon.From(InformationGeometry, color, flipHorizontally),
            CustomIconType.LinkedIn             => Icon.From(LinkedInGeometry, color, flipHorizontally),
            CustomIconType.Lock                 => Icon.From(LockGeometry, color, flipHorizontally),
            CustomIconType.LockOpen             => Icon.From(LockOpenGeometry, color, flipHorizontally),
            CustomIconType.Maintenance          => Icon.From(MaintenanceGeometry, color, flipHorizontally),
            CustomIconType.Menu                 => Icon.From(HamburgerMenuGeometry, color, flipHorizontally),
            CustomIconType.PlusCircular         => Icon.From(PlusCircleOutlineGeometry, color, flipHorizontally),
            CustomIconType.QRCode               => Icon.From(QrCodeGeometry, color, flipHorizontally),
            CustomIconType.Settings             => Icon.From(SettingsCogGeometry, color, flipHorizontally),
            CustomIconType.SettingsSave         => Icon.From(ContentSaveSettingsGeometry, color, flipHorizontally),
            CustomIconType.SupportAgent         => Icon.From(SupportFaceGeometry, color, flipHorizontally),
            CustomIconType.WarningTriangular    => Icon.From(AlertOutlineGeometry, color, flipHorizontally),
            CustomIconType.Web                  => Icon.From(WebGeometry, color, flipHorizontally),
            CustomIconType.Restore              => Icon.From(RestoreGeometry, color, flipHorizontally),
            CustomIconType.SerialPort           => Icon.From(SerialPortGeometry, color, flipHorizontally),
            _ => new DrawingImage(),
        };

        /// <summary>
        /// Takes <paramref name="customGeometry"/> and <paramref name="color"/> to instantiate icon with provided geometry and color
        /// </summary>
        /// <param name="customGeometry"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DrawingImage GenerateIcon(string? customGeometry, Color color, bool flipHorizontally = false) => customGeometry == null ? throw new NullReferenceException(nameof(customGeometry)) : Icon.From(customGeometry, color, flipHorizontally);
        #endregion

        private class Icon : DrawingImage
        {
            /// <summary>
            /// Private constructor that takes geometry and color to parse into a drawing
            /// </summary>
            /// <param name="geometry">String geometry to draw</param>
            /// <param name="brushColor">Color to draw icon with</param>
            private Icon(string geometry, Color brushColor, bool flipHorizontally, bool flipVertically = false)
            {
                Geometry parsedGeometry = Geometry.Parse(geometry);

                if (flipHorizontally || flipVertically)
                {
                    Functions.CustomDebug.WriteLine("Entered transform if-statement");
                    var transform = new ScaleTransform();
                    if (flipHorizontally)
                    {
                        Functions.CustomDebug.WriteLine("Flip horizontally");
                        transform.ScaleX = -1;
                    }

                    if (flipVertically)
                    {
                        Functions.CustomDebug.WriteLine("Flip vertically");
                        transform.ScaleY = -1;
                    }

                    Functions.CustomDebug.WriteLine("Apply transform");
                    parsedGeometry.Transform = transform;
                }

                Drawing = new GeometryDrawing()
                {
                    Geometry    = parsedGeometry,
                    Brush       = new SolidColorBrush(brushColor),
                };
            }

            /// <summary>
            /// Static Icon generator that takes a geometry and color to instantiate a new Icon
            /// </summary>
            /// <param name="geometry">String geometry to draw</param>
            /// <param name="brushColor">Color of icon</param>
            /// <returns>Icon instantiated from provided <paramref name="geometry"/> and <paramref name="brushColor"/></returns>
            internal static Icon From(string geometry, Color brushColor, bool flipHorizontally = false) => new(geometry, brushColor, flipHorizontally);
        }


        public static readonly AttachedProperty<bool> AnimateDrawProperty =
    AvaloniaProperty.RegisterAttached<CustomIcon, Interactive, bool>(
        nameof(AnimateDraw),
        false,
        false,
        Avalonia.Data.BindingMode.OneWay,
        null);

        private bool _animateDraw;
        public bool AnimateDraw
        {
            get => _animateDraw;
            set => SetAndRaise(AnimateDrawProperty, ref _animateDraw, value);
        }

        public static readonly AttachedProperty<bool> ReactToPointerProperty =
    AvaloniaProperty.RegisterAttached<CustomIcon, Interactive, bool>(
        nameof(ReactToPointer),
        false,
        false,
        Avalonia.Data.BindingMode.OneWay,
        null);

        private bool _reactToPointer;
        public bool ReactToPointer
        {
            get => _reactToPointer;
            set => SetAndRaise(ReactToPointerProperty, ref _reactToPointer, value);
        }


        public static readonly AttachedProperty<CustomIconType?> IconKindProperty =
    AvaloniaProperty.RegisterAttached<CustomIcon, Interactive, CustomIconType?>(
        nameof(IconKind),
        null,
        false,
        Avalonia.Data.BindingMode.OneWay);

        private CustomIconType? _iconKind;
        public CustomIconType? IconKind
        {
            get => _iconKind;
            set => SetAndRaise(IconKindProperty, ref _iconKind, value);
        }

        public static readonly AttachedProperty<string?> IconGeometryProperty =
    AvaloniaProperty.RegisterAttached<CustomIcon, Interactive, string?>(
        nameof(IconGeometry),
        null,
        false,
        Avalonia.Data.BindingMode.OneWay);

        private string? _iconGeometry;
        public string? IconGeometry
        {
            get => _iconGeometry;
            set => SetAndRaise(IconGeometryProperty, ref _iconGeometry, value);
        }

        /// <summary>
        /// This takes current instance's color and icon type and regenerates icon
        /// </summary>
        private void RedrawIcon() => Dispatcher.UIThread.Post(() =>
        {
            if (IconGeometry is not null)
            {
                Source = GenerateIcon(IconGeometry, Color, FlipHorizontally);
            }
            else if (IconKind is not null)
            {
                Source = GenerateIcon(IconKind, Color, FlipHorizontally);
            }
            else
            {
                Source = null;
            }
        }, DispatcherPriority.Render);


        public static readonly AttachedProperty<Color> ColorProperty =
    AvaloniaProperty.RegisterAttached<CustomIcon, Interactive, Color>(
        nameof(Color),
        Colors.White,
        false,
        Avalonia.Data.BindingMode.OneWay,
        null);

        private Color _color;
        public Color Color
        {
            get => _color;
            set => SetAndRaise(ColorProperty, ref _color, value);
        }

        public static readonly AttachedProperty<bool> FlipHorizontallyProperty =
    AvaloniaProperty.RegisterAttached<CustomIcon, Interactive, bool>(
        nameof(FlipHorizontally),
        false,
        false,
        Avalonia.Data.BindingMode.OneWay,
        null);

        private bool _flipHorizontally = false;
        public bool FlipHorizontally
        {
            get => _flipHorizontally;
            set => SetAndRaise(FlipHorizontallyProperty, ref _flipHorizontally, value);
        }

        public CustomIcon()
        {
            MinHeight = 18;

            PointerEnter += CustomIcon_PointerEnter;
            PointerLeave += CustomIcon_PointerLeave;
            _ = IconGeometryProperty.Changed.Subscribe(x => HandleIconGeometryChanged(x.Sender, x.NewValue.GetValueOrDefault<string>()));
            _ = FlipHorizontallyProperty.Changed.Subscribe(x => HandleFlipPropertiesChanged(x.Sender, x.NewValue.GetValueOrDefault<bool>(), true));
            _ = ColorProperty.Changed.Subscribe(x => HandleColorChanged(x.Sender, x.NewValue.GetValueOrDefault<Color>));
            _ = IconKindProperty.Changed.Subscribe(x => HandleIconKindChanged(x.Sender, x.NewValue.GetValueOrDefault<CustomIconType>));
        }

        private static void HandleFlipPropertiesChanged(IAvaloniaObject sender, bool flip, bool horizontal)
        {
            if (sender is CustomIcon icon && flip && icon.IconGeometry is not null)
            {
                Functions.CustomDebug.WriteLine("About to redraw flipped icon");
                icon.FlipHorizontally = flip;
                icon.RedrawIcon();
            }
        }

        private static void HandleIconGeometryChanged(IAvaloniaObject sender, string? geo)
        {
            if (sender is CustomIcon icon && geo is not null)
            {
                icon.IconGeometry = geo;
                icon.RedrawIcon();
            }
        }

        private static void HandleColorChanged(IAvaloniaObject sender, Func<Color> getValueOrDefault)
        {
            if (sender is CustomIcon icon)
            {
                icon.Color = getValueOrDefault();
                icon.RedrawIcon();
            }
        }

        private static void HandleIconKindChanged(IAvaloniaObject sender, Func<CustomIconType> getValueOrDefault)
        {
            if (sender is CustomIcon icon)
            {
                icon.IconKind = getValueOrDefault();
                icon.RedrawIcon();
            }
        }

        private void CustomIcon_PointerLeave(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            if (sender is CustomIcon icon && icon.ReactToPointer && icon.IsPointerOver == false)
            {
                icon.Height -= 8;
            }
        }

        private void CustomIcon_PointerEnter(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            if (sender is CustomIcon icon && icon.ReactToPointer && icon.IsPointerOver== true)
            {
                icon.Height += 8;
            }
        }
    }
}

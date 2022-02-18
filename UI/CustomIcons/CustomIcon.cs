using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace AvaloniaToolbox.UI.CustomIcons
{
    /// <summary>
    /// Custom Image class with added CustomIconType and Color properties for use adjusting image sources to matching icons.
    /// </summary>
    public partial class CustomIcon : Image
    {
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


        public static readonly AttachedProperty<CustomIconType> IconKindProperty =
    AvaloniaProperty.RegisterAttached<CustomIcon, Interactive, CustomIconType>(
        nameof(IconKind),
        CustomIconType.None,
        false,
        Avalonia.Data.BindingMode.OneWay);

        private CustomIconType _iconKind;
        public CustomIconType IconKind
        {
            get => _iconKind;
            set => SetAndRaise(IconKindProperty, ref _iconKind, value);
        }
        
        /// <summary>
        /// This takes current instance's color and icon type and regenerates icon
        /// </summary>
        private void RedrawIcon()
        {
            Dispatcher.UIThread.Post(() => Source = Icons.GenerateIcon(IconKind, Color), DispatcherPriority.Render);
        }

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

        public CustomIcon()
        {
            MinHeight = 18;

            PointerEnter += CustomIcon_PointerEnter;
            PointerLeave += CustomIcon_PointerLeave;
            _ = ColorProperty.Changed.Subscribe(x => HandleColorChanged(x.Sender, x.NewValue.GetValueOrDefault<Color>));
            _ = IconKindProperty.Changed.Subscribe(x => HandleIconKindChanged(x.Sender, x.NewValue.GetValueOrDefault<CustomIconType>));
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
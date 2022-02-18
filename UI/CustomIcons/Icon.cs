using Avalonia.Media;

namespace AvaloniaToolbox.UI.CustomIcons
{
    public static partial class Icons
    {
        private class Icon : DrawingImage
        {
            /// <summary>
            /// Private constructor that takes geometry and color to parse into a drawing
            /// </summary>
            /// <param name="geometry">String geometry to draw</param>
            /// <param name="brushColor">Color to draw icon with</param>
            private Icon(string geometry, Color brushColor, bool flipHorizontally)
            {
                Geometry parsedGeometry = Geometry.Parse(geometry);

                //We do this here because FarFulcrum's icon's geometry is reversed
                if (flipHorizontally)
                {
                    parsedGeometry.Transform = new ScaleTransform
                    {
                        ScaleX = -1
                    };
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
            public static Icon From(string geometry, Color brushColor, bool flipHorizontally = false) => new(geometry, brushColor, flipHorizontally);
        }
    }
}
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace PlasmaLibrary
{
    public class PlasmaSpriteVisual : PlasmaRectangleGeometry
    {
        private readonly Dictionary<int, SpriteVisual> _sprites = new Dictionary<int, SpriteVisual>();

        public PlasmaSpriteVisual(int width, int height, int rectangleWidth, UIElement rootElement) : base(width, height,
            rectangleWidth, rootElement)
        {
        }

        public override void Render()
        {
            var c = Window.Current.Compositor;

            // Need this so we can add multiple shapes to a sprite
            var container = c.CreateContainerVisual();

            for (var y = 0; y < _width; y++)
            for (var x = 0; x < _height; x++)
            {
                var rectangle = c.CreateSpriteVisual();
                rectangle.Size = new Vector2(_rectangleWidth + 1);
                rectangle.Brush = _colorBrushes[PlasmaColors[y][x]];
                rectangle.Offset = new Vector3(_rectangleWidth * y, _rectangleWidth * x, 0);

                _sprites.Add(GenerateXYIndex(x, y), rectangle);
                container.Children.InsertAtTop(rectangle);
            }


            // This is what we can add as a child to an element

            ElementCompositionPreview.SetElementChildVisual(_rootElement, container);
        }

        public override async Task Animate()
        {
            var c = Window.Current.Compositor;
            PaletteOffset = 0;

            while (true)
            {
                await Task.Delay(1);

                PaletteOffset++;

                for (var y = 0; y < _width; y++)
                for (var x = 0; x < _height; x++)
                {
                    var sprite = _sprites[GenerateXYIndex(x, y)];

                    var colorBrush = _colorBrushes[(PlasmaColors[y][x] + PaletteOffset) % 360];
                    sprite.Brush = colorBrush;
                }
            }
        }

    }
}
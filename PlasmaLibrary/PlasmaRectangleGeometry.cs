using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace PlasmaLibrary
{
    // Credit to http://lodev.org/cgtutor/plasma.html
    // For the PlasmaRectangleGeometry algorithms
    public class PlasmaRectangleGeometry : PlasmaBase
    {
        #region Fields

        protected readonly CompositionColorBrush[] _colorBrushes = new CompositionColorBrush[360];


        private readonly Dictionary<int, CompositionSpriteShape> _spriteShapes =
            new Dictionary<int, CompositionSpriteShape>();

        protected UIElement _rootElement;

        #endregion

        public PlasmaRectangleGeometry(int width, int height, int rectangleWidth, UIElement rootElement) : base(width, height,
            rectangleWidth)
        {
            _rootElement = rootElement;

            var c = Window.Current.Compositor;

            for (var index = 0; index < Palette.Length; index++)
                _colorBrushes[index] = c.CreateColorBrush(Palette[index]);
        }

        public override async Task Animate()
        {
            var c = Window.Current.Compositor;
            PaletteOffset = 0;

            while (true)
            {
                await Task.Delay(10);

                PaletteOffset++;

                for (var y = 0; y < _width; y++)
                for (var x = 0; x < _height; x++)
                {
                    var spriteShape = _spriteShapes[GenerateXYIndex(x, y)];

                    var colorBrush = _colorBrushes[(PlasmaColors[y][x] + PaletteOffset) % 360];
                    spriteShape.StrokeBrush = colorBrush;
                    spriteShape.FillBrush = colorBrush;
                }
            }
        }

        public override void Render()
        {
            var c = Window.Current.Compositor;

            // Need this so we can add multiple shapes to a sprite
            var shapeContainer = c.CreateContainerShape();

            for (var y = 0; y < _width; y++)
            for (var x = 0; x < _height; x++)
            {
                var rectangle = c.CreateRectangleGeometry();
                rectangle.Size = new Vector2(_rectangleWidth + 1);


                // Need to create a sprite shape from the rounded rect
                var spriteShape = c.CreateSpriteShape(rectangle);
                var colorBrush = _colorBrushes[PlasmaColors[y][x]];
                spriteShape.FillBrush = colorBrush;
                spriteShape.StrokeThickness = 0;
                spriteShape.Offset = new Vector2(_rectangleWidth * y, _rectangleWidth * x);

                _spriteShapes.Add(GenerateXYIndex(x, y), spriteShape);

                shapeContainer.Shapes.Add(spriteShape);
            }


            // This is what we can add as a child to an element
            var shapeVisual = c.CreateShapeVisual();
            shapeVisual.Shapes.Add(shapeContainer);

            shapeVisual.Size = new Vector2(1000, 1000);

            ElementCompositionPreview.SetElementChildVisual(_rootElement, shapeVisual);
        }

        public override void RenderPalette()
        {
            var c = Window.Current.Compositor;
            // Need this so we can add multiple shapes to a sprite
            var shapeContainer = c.CreateContainerShape();

            for (var i = 0; i < 256; i++)
            {
                var rectangle = c.CreateRectangleGeometry();
                rectangle.Size = new Vector2(5, 100);

                // Need to create a sprite shape from the rounded rect
                var spriteShape = c.CreateSpriteShape(rectangle);
                var colorBrush = _colorBrushes[i];
                spriteShape.StrokeBrush = colorBrush;
                spriteShape.FillBrush = colorBrush;
                spriteShape.StrokeThickness = 1;
                spriteShape.Offset = new Vector2(5 * i, 0);

                shapeContainer.Shapes.Add(spriteShape);
            }

            // This is what we can add as a child to an element
            var shapeVisual = c.CreateShapeVisual();
            shapeVisual.Shapes.Add(shapeContainer);

            shapeVisual.Size = new Vector2(1000, 1000);

            ElementCompositionPreview.SetElementChildVisual(_rootElement, shapeVisual);
        }

    }
}
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.Web.Http.Headers;
using Microsoft.Toolkit.Uwp.Helpers;

namespace UWPCompositionPlasma
{
    // Credit to http://lodev.org/cgtutor/plasma.html
    // For the Plasma algorithms
    public class Plasma
    {
        #region Fields

        private readonly int _height;
        private readonly int _rectangleWidth;
        private readonly int _width;

        private readonly Dictionary<int, CompositionSpriteShape> _spriteShapes = new Dictionary<int, CompositionSpriteShape>();
        private readonly Dictionary<int, SpriteVisual> _sprites = new Dictionary<int, SpriteVisual>();

        #endregion

        #region Properties

        public CompositionColorBrush[] Palette { get; }

        public int[][] PlasmaColors { get; private set; }

        #endregion

        public Plasma(int width, int height, int rectangleWidth)
        {
            Palette = GeneratePalette();

            _width = width;
            _height = height;
            _rectangleWidth = rectangleWidth;

            InitializePlasmaArray(width, height);
            GenerateSimplePlasma();
        }

        public void RenderPalette(UIElement rootElement)
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
                var colorBrush = Palette[i];
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

            ElementCompositionPreview.SetElementChildVisual(rootElement, shapeVisual);

        }

        public void RenderRectangleGeometry(UIElement rootElement)
        {
            var c = Window.Current.Compositor;

            // Need this so we can add multiple shapes to a sprite
            var shapeContainer = c.CreateContainerShape();

            for (var y = 0; y < _width; y++)
            {
                for (var x = 0; x < _height; x++)
                {
                    var rectangle = c.CreateRectangleGeometry();
                    rectangle.Size = new Vector2(_rectangleWidth+1);


                    // Need to create a sprite shape from the rounded rect
                    var spriteShape = c.CreateSpriteShape(rectangle);
                    var colorBrush = Palette[PlasmaColors[y][x]];
                    //spriteShape.StrokeBrush = colorBrush;
                    spriteShape.FillBrush = colorBrush;
                    spriteShape.StrokeThickness = 0;
                    spriteShape.Offset = new Vector2(_rectangleWidth * y, _rectangleWidth * x);

                    _spriteShapes.Add(GenerateXYIndex(x, y), spriteShape);

                    shapeContainer.Shapes.Add(spriteShape);
                }
            }


            // This is what we can add as a child to an element
            var shapeVisual = c.CreateShapeVisual();
            shapeVisual.Shapes.Add(shapeContainer);

            shapeVisual.Size = new Vector2(1000, 1000);

            ElementCompositionPreview.SetElementChildVisual(rootElement, shapeVisual);
        }

        public void RenderSolidSpriteVisual(UIElement rootElement)
        {
            var c = Window.Current.Compositor;

            // Need this so we can add multiple shapes to a sprite
            var container = c.CreateContainerVisual();

            for (var y = 0; y < _width; y++)
            {
                for (var x = 0; x < _height; x++)
                {
                    var rectangle = c.CreateSpriteVisual();
                    rectangle.Size = new Vector2(_rectangleWidth+1);
                    rectangle.Brush = Palette[PlasmaColors[y][x]];
                    rectangle.Offset = new Vector3(_rectangleWidth * y, _rectangleWidth * x, 0);

                    _sprites.Add(GenerateXYIndex(x, y), rectangle);


                    container.Children.InsertAtTop(rectangle);
                }
            }


            // This is what we can add as a child to an element

            ElementCompositionPreview.SetElementChildVisual(rootElement, container);
        }

        public async Task AnimateSpriteVisual()
        {
            var c = Window.Current.Compositor;
            var paletteShift = 0;

            while (true)
            {
                await Task.Delay(1);

                paletteShift++;

                for (var y = 0; y < _width; y++)
                {
                    for (var x = 0; x < _height; x++)
                    {
                        var sprite = _sprites[GenerateXYIndex(x, y)];

                        var colorBrush = Palette[(PlasmaColors[y][x] + paletteShift) % 360];
                        sprite.Brush = colorBrush;
                    }
                }
            }
        }


        public async Task AnimateRectangleGeometry()
        {
            var c = Window.Current.Compositor;
            var paletteShift = 0;

            while (true)
            {
                await Task.Delay(1);

                paletteShift++;

                for (var y = 0; y < _width; y++)
                {
                    for (var x = 0; x < _height; x++)
                    {
                        var spriteShape = _spriteShapes[GenerateXYIndex(x, y)];

                        var colorBrush = Palette[(PlasmaColors[y][x] + paletteShift) % 360];
                        spriteShape.StrokeBrush = colorBrush;
                        spriteShape.FillBrush = colorBrush;
                    }
                }
            }
        }

        private int GenerateXYIndex(int x, int y)
        {
            return (x * 1024 + y);
        }

        private static CompositionColorBrush[] GeneratePalette()
        {
            var c = Window.Current.Compositor;
            var colors = new CompositionColorBrush[360];

            for (var x = 0; x < 360; x++)
                colors[x] = c.CreateColorBrush(Microsoft.Toolkit.Uwp.Helpers.ColorHelper.FromHsv(x, 1, 1));

            return colors;
        }

        private void GenerateSimplePlasma()
        {
            //generate the plasma once
            for (var y = 0; y < _height; y++)
                for (var x = 0; x < _width; x++)
                {
                    //the plasma buffer is a sum of sines
                    int color = (int)
                                (
                                    128.0 + (128.0 * Math.Sin(x / 16.0))
                                    + 128.0 + (128.0 * Math.Sin(y / 8.0))
                                    + 128.0 + (128.0 * Math.Sin((x + y) / 16.0))
                                    + 128.0 + (128.0 * Math.Sin(Math.Sqrt(x * x + y * y) / 8.0))
                                ) / 4;
                    PlasmaColors[x][y] = color;
                }
        }

        private void InitializePlasmaArray(int width, int height)
        {
            PlasmaColors = new int[width][];
            for (var i = 0; i < width; i++)
                PlasmaColors[i] = new int[height];
        }
    }
}
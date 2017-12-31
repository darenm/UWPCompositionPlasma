using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using ColorHelper = Microsoft.Toolkit.Uwp.Helpers.ColorHelper;

namespace PlasmaLibrary
{
    public abstract class PlasmaBase
    {
        #region Fields

        protected int _height;
        protected int _rectangleWidth;
        protected int _width;

        #endregion

        #region Properties

        public bool IsReady { get; }

        public long PaletteOffset { get; set; }
        public Color[] Palette { get; protected set; }
        public int[][] PlasmaColors { get; private set; }

        #endregion

        protected PlasmaBase(int width, int height, int rectangleWidth)
        {
            _width = width;
            _height = height;
            _rectangleWidth = rectangleWidth;

            Palette = GeneratePalette();
            InitializePlasmaArray(width, height);
            GenerateSimplePlasma();
            IsReady = true;
        }

        public abstract Task Animate();

        public abstract void Render();

        public abstract void RenderPalette();

        protected static Color[] GeneratePalette()
        {
            var c = Window.Current.Compositor;
            var colors = new Color[360];

            for (var x = 0; x < 360; x++)
                colors[x] = ColorHelper.FromHsv(x, 1, 1);

            return colors;
        }

        protected void GenerateSimplePlasma()
        {
            //generate the plasma once
            for (var y = 0; y < _height; y++)
            for (var x = 0; x < _width; x++)
            {
                //the plasma buffer is a sum of sines
                var color = (int)
                            (
                                128.0 + 128.0 * Math.Sin(x / 16.0)
                                + 128.0 + 128.0 * Math.Sin(y / 8.0)
                                + 128.0 + 128.0 * Math.Sin((x + y) / 16.0)
                                + 128.0 + 128.0 * Math.Sin(Math.Sqrt(x * x + y * y) / 8.0)
                            ) / 4;
                PlasmaColors[x][y] = color;
            }
        }

        protected int GenerateXYIndex(int x, int y)
        {
            return x * 1024 + y;
        }

        protected void InitializePlasmaArray(int width, int height)
        {
            PlasmaColors = new int[width][];
            for (var i = 0; i < width; i++)
                PlasmaColors[i] = new int[height];
        }
    }
}
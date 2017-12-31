using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using PlasmaLibrary;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Win2DPlasma
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int RectangleWidth = 5;

        #region Fields

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private int _gridHeight = 300;
        private int _gridWidth = 300;
        private PlasmaWin2D _plasma;
        private int _localFramecount;

        #endregion

        public MainPage()
        {
            InitializeComponent();
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += TimerOnTick;

            Loaded += (sender, args) =>
            {
                _stopwatch.Start();
                _timer.Start();
            };

            SizeChanged += MainPage_SizeChanged;
        }

        private void AnimatedControl_OnCreateResources(CanvasAnimatedControl sender,
            CanvasCreateResourcesEventArgs args)
        {
        }

        private void AnimatedControl_OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            if (_plasma == null || !_plasma.IsReady)
            {
                return;
            }
            try
            {
                for (var y = 0; y < _gridHeight; y++)
                for (var x = 0; x < _gridWidth; x++)
                {
                    var r = new Rect(RectangleWidth * x, RectangleWidth * y, RectangleWidth + 1, RectangleWidth + 1);
                    args.DrawingSession.FillRectangle(r,
                        _plasma.Palette[(_plasma.PlasmaColors[x][y] + _plasma.PaletteOffset) % 360]);
                }

                _localFramecount++;
            }
            catch (IndexOutOfRangeException)
            {
                // catch transient error during resizing
            }
        }

        private void AnimatedControl_OnUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            _plasma.PaletteOffset++;
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _gridHeight = (int) ActualHeight / RectangleWidth;
            _gridWidth = (int) ActualWidth / RectangleWidth;
            _plasma = new PlasmaWin2D(_gridWidth, _gridHeight, RectangleWidth);
        }

        private void TimerOnTick(object sender, object e)
        {
            double v = _stopwatch.ElapsedMilliseconds;
            _stopwatch.Reset();
            _stopwatch.Start();
            var fps = _localFramecount / (v / 1000d);
            _localFramecount = 0;
            FrameRate.Text = $"{fps} fps";
        }
    }
}
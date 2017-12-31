using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PlasmaLibrary;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPCompositionPlasma
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly PlasmaBase _plasma;
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public MainPage()
        {
            this.InitializeComponent();
            //_plasma = new PlasmaRectangleGeometry(150, 150, 5, RootGrid);
            _plasma = new PlasmaSpriteVisual(150, 150, 5, RootGrid);
            //_plasma = new PlasmaRectangleGeometry(150, 150, 5, RootGrid);
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += TimerOnTick;
            Loaded += OnLoaded;
        }

        private void TimerOnTick(object sender, object o)
        {
            double v = _stopwatch.ElapsedMilliseconds;
            double fps = _plasma.PaletteOffset / (v / 1000d);
            FrameRate.Text = $"{fps} fps";
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            //_plasma.RenderPalette(RootGrid);

            // This won't render at 200x200
            _plasma.Render();
            _stopwatch.Start();
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () => _plasma.Animate());

            _timer.Start();

        }

    }
}

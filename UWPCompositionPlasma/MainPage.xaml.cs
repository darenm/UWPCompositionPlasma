using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPCompositionPlasma
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly Plasma _plasma;

        public MainPage()
        {
            this.InitializeComponent();
            _plasma = new Plasma(180, 180, 5);
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            //_plasma.RenderPalette(RootGrid);

            // This won't render at 200x200
            _plasma.RenderRectangleGeometry(RootGrid);
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () => _plasma.AnimateRectangleGeometry());

            //_plasma.RenderSolidSpriteVisual(RootGrid);
            //Dispatcher.RunAsync(CoreDispatcherPriority.High, () => _plasma.AnimateSpriteVisual());

        }

    }
}

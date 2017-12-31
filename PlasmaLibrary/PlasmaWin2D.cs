using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasmaLibrary
{
    public class PlasmaWin2D : PlasmaBase
    {
        public PlasmaWin2D(int width, int height, int rectangleWidth) : base(width, height, rectangleWidth)
        {
        }

        public override void RenderPalette()
        {
            throw new NotImplementedException();
        }

        public override void Render()
        {
            throw new NotImplementedException();
        }

        public override Task Animate()
        {
            throw new NotImplementedException();
        }
    }
}

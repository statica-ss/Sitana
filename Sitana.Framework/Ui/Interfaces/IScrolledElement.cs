using System;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Views;

namespace Sitana.Framework.Ui.Interfaces
{
    public interface IScrolledElement
    {
        Rectangle ScreenBounds { get; }

        int MaxScrollX { get; }
        int MaxScrollY { get; }

        ScrollingService ScrollingService {get;}
    }
}


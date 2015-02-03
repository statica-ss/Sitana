using System;
using Microsoft.Xna.Framework.Input;

namespace Sitana.Framework.Input
{
    public interface IFocusable
    {
        void Unfocus();

        void OnKey(Keys key);
        void OnCharacter(char character);

        void SetText(string text);

        int Bottom { get; }
    }
}


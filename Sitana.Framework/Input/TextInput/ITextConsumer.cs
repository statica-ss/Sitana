using System;

namespace Sitana.Framework.Input
{
    public interface ITextConsumer
    {
        string OnTextChanged(string newText);

        void OnLostFocus();

        int SelectionStart { set;}
        int SelectionEnd { set;}

        void Cancel();
        void Apply();
    }
}


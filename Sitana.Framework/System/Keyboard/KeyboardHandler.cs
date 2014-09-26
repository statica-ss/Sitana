using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Xna.Framework.Input
{
    public class KeyboardHandler: MessageHook
    {
        public Action<Char> OnCharacter;

        public KeyboardHandler( IntPtr window )
            : base( window )
        {
            
        }

        protected override void Hook( ref Message m )
        {
            switch ( m.msg )
            {
            case Wm.KeyDown:
                _TranslateMessage( ref m );
                break;

            case Wm.Char:
                char c = (char)m.wparam;
                if ( c < (char)0x20 
                    && c != '\n'
                    && c != '\r'
                    //&& c != '\t'//tab //uncomment to accept tab
                    && c != '\b' )//backspace
                    break;

                if (OnCharacter != null)
                {
                    OnCharacter.Invoke(c);
                }
                break;
            }
        }
    }
}

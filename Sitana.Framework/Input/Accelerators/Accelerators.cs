using System;
using Sitana.Framework.Cs;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Sitana.Framework.Input
{
    public class Accelerators: Singleton<Accelerators>
    {
        private KeyboardState _oldKeyState = new KeyboardState();

        struct Accelerator
        {
            public KeyModifiers Modifiers;
            public Keys Key;

            public char Character;

            public EmptyArgsVoidDelegate Action;
        }

        List<Accelerator> _accelerators = new List<Accelerator>();

        public void Register( KeyModifiers modifiers, Keys key, EmptyArgsVoidDelegate action )
        {
            _accelerators.Add(new Accelerator() 
            {
                Modifiers = modifiers,
                Key = key,
                Action = action,
                Character = '\0'
            });
        }

        public void Register( char character, EmptyArgsVoidDelegate action )
        {
            _accelerators.Add(new Accelerator() 
            {
                Modifiers = KeyModifiers.None,
                Key = Keys.None,
                Action = action,
                Character = character
            });
        }

        KeyModifiers Modifiers(ref KeyboardState state)
        {
            KeyModifiers modifier = KeyModifiers.None;

            if(state.IsKeyDown(Keys.LeftControl) || state.IsKeyDown(Keys.RightControl))
            {
                modifier |= KeyModifiers.Ctrl;
            }

            if(state.IsKeyDown(Keys.LeftWindows) || state.IsKeyDown(Keys.RightWindows))
            {
                modifier |= KeyModifiers.Ctrl;
            }

            if(state.IsKeyDown(Keys.LeftAlt) || state.IsKeyDown(Keys.RightAlt))
            {
                modifier |= KeyModifiers.Alt;
            }

            if(state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift))
            {
                modifier |= KeyModifiers.Shift;
            }
            return modifier;
        }

        internal void Process(char character)
        {
            KeyboardState state = _oldKeyState;
            KeyModifiers modifier = Modifiers(ref state);

            for (int idx = 0; idx < _accelerators.Count; ++idx)
            {

                if(_accelerators[idx].Character == character)
                {
                    _accelerators[idx].Action();
                    break;
                }
            }
        }

//        internal void Process(bool analyze)
//        {
//            KeyboardState state = Keyboard.GetState();
//
//            if (analyze)
//            {
//                KeyModifiers modifier = Modifiers(ref state);
//
//                for (int idx = 0; idx < _accelerators.Count; ++idx)
//                {
//                    KeyModifiers mod = _accelerators[idx].Modifiers;
//
//                    if (mod == modifier)
//                    {
//                        Keys key = _accelerators[idx].Key;
//
//                        if (state.IsKeyDown(key) && _oldKeyState.IsKeyUp(key))
//                        {
//                            _accelerators[idx].Action();
//                            break;
//                        }
//                    }
//                }
//            }
//
//            _oldKeyState = state;
//        }
    }
}


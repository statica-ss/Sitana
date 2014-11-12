using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Sitana.Framework.Cs;
using System;
using System.Collections.Generic;
using System.Text;

using XnaGamePad = Microsoft.Xna.Framework.Input.GamePad;

namespace Sitana.Framework.Input.GamePad
{
    public class GamePad
    {
        PlayerIndex _playerIndex;

        Dictionary<Buttons, GamePadButtonState> _states = new Dictionary<Buttons, GamePadButtonState>();
        Buttons[] _keys;

        internal GamePad(PlayerIndex playerIndex)
        {
            _playerIndex = playerIndex;

            Array keys = Enum.GetValues(typeof(Buttons));

            _keys = new Buttons[keys.Length];

            for(int idx = 0; idx < keys.Length; ++idx)
            {
                _states.Add((Buttons)(keys.GetValue(idx)), GamePadButtonState.Released);
                _keys[idx] = (Buttons)(keys.GetValue(idx));
            }
        }

        public bool IsConnected
        {
            get
            {
                return XnaGamePad.GetCapabilities(_playerIndex).IsConnected;
            }
        }

        internal void Update()
        {
            GamePadState state = XnaGamePad.GetState(_playerIndex);

            foreach (var en in _keys)
            {
                bool isPressed = state.IsButtonDown(en);
                bool wasPressed = _states[en] != GamePadButtonState.Released;
                 
                if ( isPressed )
                {
                    if (wasPressed)
                    {
                        _states[en] = GamePadButtonState.Hold;
                    }
                    else
                    {
                        _states[en] = GamePadButtonState.Pressed;
                    }
                }
                else
                {
                    _states[en] = GamePadButtonState.Released;
                }
            }
        }

        public bool IsButtonDown(Buttons button)
        {
            return _states[button] != GamePadButtonState.Released;
        }

        public bool IsButtonUp(Buttons button)
        {
            return _states[button] == GamePadButtonState.Released;
        }

        public GamePadButtonState ButtonState(Buttons button)
        {
            return _states[button];
        }
    }
}

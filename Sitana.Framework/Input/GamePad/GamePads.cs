using Microsoft.Xna.Framework;
using Sitana.Framework.Cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.Input.GamePad
{
    public class GamePads: Singleton<GamePads>
    {
#if WINDOWS_PHONE_APP
        GamePad[] _gamePads = new GamePad[1];
#else
        GamePad[] _gamePads = new GamePad[4];
#endif

        public GamePads()
        {
            _gamePads[0] = new GamePad(PlayerIndex.One);

#if !WINDOWS_PHONE_APP
            _gamePads[1] = new GamePad(PlayerIndex.Two);
            _gamePads[2] = new GamePad(PlayerIndex.Three);
            _gamePads[3] = new GamePad(PlayerIndex.Four);
#endif
        }

        public GamePad this[int index]
        {
            get
            {
                return _gamePads[index];
            }
        }

        internal void Update()
        {
            foreach(var gp in _gamePads)
            {
                gp.Update();
            }
        }
    }
}

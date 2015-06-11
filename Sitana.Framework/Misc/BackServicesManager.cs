using Microsoft.Xna.Framework.Input;
using Sitana.Framework.Cs;
using Sitana.Framework.Input.GamePad;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Ui.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.Misc
{
    public class BackServicesManager : Singleton<BackServicesManager>, IUpdatable
    {
        List<IBackable> _backables = new List<IBackable>();

        public void Add(IBackable backable)
        {
            if(backable == null)
            {
                throw new ArgumentNullException("Backable can't be null");
            }
            
            if(_backables.Find(item => item == backable) != null)
            {
                throw new ArgumentException("Can't add more then one backable");
            }

            _backables.Add(backable);
        }

        public void Remove(IBackable obj)
        {
            _backables.Remove(obj);
        }

        public void Update(float time)
        {
            if(_backables.Count > 0 && GamePads.Instance[0].ButtonState(Buttons.Back) == GamePadButtonState.Pressed)
            {
                for(int index = _backables.Count - 1; index >= 0; --index)
                {
                    if(_backables[index].OnBack())
                    {
                        return;
                    }
                }
            }
        }
    }
}

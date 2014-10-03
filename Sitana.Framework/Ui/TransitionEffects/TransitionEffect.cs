using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Xml;

namespace Sitana.Framework.Ui.Views
{
    public abstract class TransitionEffect: IDefinitionClass
    {
        internal bool HideTransition {get; private set;}

        public abstract void Get(double transition, Rectangle containerRect, Rectangle elementRect, out Matrix transform, out float opacity);

        public abstract TransitionEffect Reverse();

        void IDefinitionClass.Init(UiController controller, object binding, DefinitionFile file)
        {
            Init(controller, binding, file);
        }

        protected virtual void Init(UiController controller, object binding, DefinitionFile definition)
        {
            
        }
    }
}

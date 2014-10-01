using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Ui.Controllers;

namespace Sitana.Framework.Ui.Views
{
    public abstract class TransitionEffect: IDefinitionClass
    {
        public abstract void Get(double transition, out Matrix transform, out float opacity);

        void IDefinitionClass.Init(UiController controller, object binding, DefinitionFile file)
        {
            Init(controller, binding, file);
        }

        protected abstract void Init(UiController controller, object binding, DefinitionFile definition);
    }
}

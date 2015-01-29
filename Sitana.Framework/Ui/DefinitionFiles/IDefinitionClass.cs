// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Sitana.Framework.Ui.Controllers;

namespace Sitana.Framework.Ui.DefinitionFiles
{
    public interface IDefinitionClass
    {
        bool Init(UiController controller, object binding, DefinitionFile file);
    }
}

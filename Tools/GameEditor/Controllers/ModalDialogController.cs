using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Input;
using Microsoft.Xna.Framework.Input;
using Sitana.Framework.Ui.Core;

namespace GameEditor
{
    public class ModalDialogController : UiNavigationController
    {
        public static ModalDialogController Current { get; private set; }

        public ModalDialogController()
        {
            Current = this;
        }

        public void Show(string path)
        {
            Find("DialogModalDialog").Visible = true;
            NavigateTo(path);
        }
    }

    static class ModalDialog
    {
        public static void Show(string path)
        {
            ModalDialogController.Current.Show(path);
        }
    }
}

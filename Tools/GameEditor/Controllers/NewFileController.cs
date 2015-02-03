using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Input;
using Microsoft.Xna.Framework.Input;
using Sitana.Framework.Ui.Binding;
using Sitana.Framework.Ui.Core;
using Sitana.Framework;

namespace GameEditor
{
    public class NewFileController : UiNavigationController, IFocusable
    {
        public IItemsProvider Templates { get { return RegisteredTemplates.Instance.Templates; } }

        void IFocusable.Unfocus() { }

        int IFocusable.Bottom { get { return 0; } }

        void IFocusable.OnKey(Keys key)
        {
            if (key == Keys.Escape)
            {
                Close();
            }
        }

        void IFocusable.OnCharacter(char character)
        {
            if (character == 27)
            {
                Close();
            }
        }

        void IFocusable.SetText(string text) { }

        public NewFileController()
        {
            AppMain.Current.SetFocus(this);
        }

        public void Close()
        {
            AppMain.Current.SetFocus(null);
            Find("DialogModalDialog").Visible.Value = false;
            NavigateTo(null);
        }

        public void NewFromTemplate(RegisteredTemplates.Template template)
        {
            FileMenuController.Current.OnNew(template.Path);
            Close();
        }

        public void Register()
        {
            string path = Platform.OpenFileDialog("Open template", "Zip files (*.zip)|*.zip|All files|*.*");

            if (path != null)
            {
                try
                {
                    RegisteredTemplates.Instance.Register(path);
					RegisteredTemplates.Instance.SaveTemplatesList();
                }
                catch(Exception ex)
                { 
                    Console.WriteLine("Unable to register: {0}", ex.ToString());
                }
            }
        }
    }
}

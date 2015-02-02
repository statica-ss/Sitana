using System;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Cs;
using System.IO;
using Sitana.Framework.Content;

namespace GameEditor
{
    public class FileMenuController: UiController
    {
        static readonly string SaveChangesQuestionFormat = "Do you want to save changes in\n{0}?";

        public static FileMenuController Current {get; private set;}

        public bool CanClose
        {
            get
            {
                return !Document.Current.IsModified;
            }
        }

        string SaveChangesQuestion
        {
            get
            {
                return String.Format(SaveChangesQuestionFormat, Document.Current.FileName);
            }
        }

        public FileMenuController()
        {
            Current = this;
        }

        public void New()
        {
            ModalDialog.Show("Views/NewFile");
            HideElement("FileMenu");
        }

        public void OnNew(string template)
        {
            if (!Document.Current.IsModified)
            {
                ChangeTemplate(template);
                Document.Instance.New();
                HideElement("FileMenu");
                return;
            }

            MessageBox.YesNoCancel(SaveChangesQuestion,
                () =>
            {
                UiTask.BeginInvoke(()=>
                {
                    if(SaveInternal())
                    {
                        ChangeTemplate(template);
                        Document.Instance.New();
                    }
                });
            }, 
                () =>
            {
                ChangeTemplate(template);
                Document.Instance.New();
            },
                ()=>
            {
                HideElement("FileMenu");
            });
        }

        public void Open()
        {
            UiTask.BeginInvoke(()=>
            {
                string path = Platform.OpenFileDialog("Open file", "Map file (*.smf)|*.smf|All files|*.*");

                if (path != null)
                {
                    if ( !Document.Current.IsModified )
                    {
                        Open(path);
                        HideElement("FileMenu");
                        return;
                    }

                    MessageBox.YesNoCancel(SaveChangesQuestion, 
                        () =>
                    {
                        UiTask.BeginInvoke(()=>
                        {
                            if(SaveInternal())
                            {
                                Open(path);
                            }
                        });

                    }, 
                        () =>
                    {
                        Open(path);
                    },
                        ()=>
                    {

                        HideElement("FileMenu");
                    });
                }
                else
                {
                    HideElement("FileMenu");
                }
            });
        }

        private bool SaveInternal()
        {
            if (Document.Instance.FilePath == null)
            {
                string path = Platform.SaveFileDialog("Save file", "Map file (*.smf)|*.smf|All files|*.*", ".smf");

                if (path != null)
                {
                    Document.Instance.Save(path);
                    return true;
                }
                return false;
            }
            else
            {
                Document.Instance.Save();
                return true;
            }
        }

        public void Save()
        {
            if (Document.Instance.FilePath == null)
            {
                SaveAs();
            }
            else
            {
                HideElement("FileMenu");
                Document.Instance.Save();
            }
        }

        public void SaveAs()
        {
            UiTask.BeginInvoke(()=>
            {
                string path = Platform.SaveFileDialog("Save file", "Map file (*.smf)|*.smf|All files|*.*", ".smf");

                if (path != null)
                {
                    Document.Instance.Save(path);
                }

                HideElement("FileMenu");
            });
        }

        public void Exit()
        {
            if ( !Document.Current.IsModified )
            {
                AppMain.Current.Exit();
                return;
            }

            MessageBox.YesNoCancel(SaveChangesQuestion, () =>
            {
                if ( SaveInternal() )
                {
                    AppMain.Current.Exit();
                }
            }, 
                ()=>
            {
                Document.Current.CancelModified();
                AppMain.Current.CloseApp();
            },
                ()=>
            {
                HideElement("FileMenu");
            });
        }

        void Open(string path)
        {

        }

        bool ChangeTemplate(string template)
        {
            if (template == null)
            {
                using (Stream stream = ContentLoader.Current.Open("Templates/SampleTemplate.zip"))
                {
                    CurrentTemplate.Instance.Load(stream);
                }
            }
            else
            {
                try
                {
                    using (Stream stream = new FileStream(template, FileMode.Open))
                    {
                        CurrentTemplate.Instance.Load(stream);
                    }
                }
                catch
                {
                    RegisteredTemplates.Instance.InvalidateTemplate(template);

                    MessageBox.Info(string.Format("Failed to load template {0}.", template));
                    
                    return false;
                }
            }

            return true;
        }
    }
}


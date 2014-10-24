using System;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Cs;

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
            if (!Document.Current.IsModified)
            {
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
                        Document.Instance.New();
                    }
                });
            }, 
                () =>
            {
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
                string path = SystemWrapper.OpenFileDialog("Open file");

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
                string path = SystemWrapper.SaveFileDialog("Save file");

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
                string path = SystemWrapper.SaveFileDialog("Save file");

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
                AppMain.Current.Exit();
            },
                ()=>
            {
                HideElement("FileMenu");
            });
        }

        void Open(string path)
        {

        }
    }
}


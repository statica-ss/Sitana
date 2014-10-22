using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Content;
using Sitana.Framework.Ui.Views;
using Sitana.Framework;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui.Binding;

namespace GameEditor
{
    public class MainController: UiController
    {
        static readonly string SaveChangesQuestionFormat = "Do you want to save changes in\n{0}?";

        public static MainController Current { get; private set; }

        public SharedString MessageBoxText { get; private set; }
        public SharedString FileName { get; private set; }

        public SharedValue<bool> ShowAllLayers
        { 
            get 
            { 
                return EditorSettings.Instance.ShowAllLayersShared;
            }
        }

        public bool CanClose
        { 
            get
            {
                return !Document.Current.IsModified;
            }
        }

        public IItemsProvider Layers
        {
            get
            {
                return Document.Instance.Layers;
            }
        }

        EmptyArgsVoidDelegate _onMessageBoxYes;
        EmptyArgsVoidDelegate _onMessageBoxNo;
        EmptyArgsVoidDelegate _onMessageBoxClose;

        string SaveChangesQuestion
        {
            get
            {
                return String.Format(SaveChangesQuestionFormat, Document.Current.FileName);
            }
        }

        public static void OnLoadContent(AppMain main)
        {
            FontManager.Instance.AddSpriteFont("Font", "Font", 8);
        }

        public MainController()
        {
            if (Current != null)
            {
                throw new Exception("There can be only one MainController!");
            }

            Current = this;

            MessageBoxText = new SharedString();
            FileName = Document.Instance.FileName;
        }

        public void OpenLink(UiButton sender)
        {
            SystemWrapper.OpenWebsite(sender.Text.StringValue);
        }

        public void New()
        {
            if (!Document.Current.IsModified)
            {
                Document.Instance.New();
                HideElement("FileMenu");
                return;
            }

            MessageBoxYesNoCancel(SaveChangesQuestion,
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

                    MessageBoxYesNoCancel(SaveChangesQuestion, 
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

            MessageBoxYesNoCancel(SaveChangesQuestion, () =>
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

        public void MessageBox(string text)
        {
            MessageBoxText.StringValue = text;
            ShowElement("MessageBox");
            HideElement("MessageBoxNo");
            HideElement("MessageBoxYes");
            HideElement("MessageBoxCancel");

            HideElement("MessageBoxNo2");
            HideElement("MessageBoxYes2");

            ShowElement("MessageBoxOk");
        }

        public void MessageBoxYesNo(string text, EmptyArgsVoidDelegate onYes, EmptyArgsVoidDelegate onNo = null, EmptyArgsVoidDelegate alwaysCall = null)
        {
            _onMessageBoxYes = onYes;
            _onMessageBoxNo = onNo;
            _onMessageBoxClose = alwaysCall;

            MessageBoxText.StringValue = text;
            ShowElement("MessageBox");
            ShowElement("MessageBoxNo2");
            ShowElement("MessageBoxYes2");

            HideElement("MessageBoxCancel");
            HideElement("MessageBoxNo");
            HideElement("MessageBoxYes");

            HideElement("MessageBoxOk");
        }

        public void MessageBoxYesNoCancel(string text, EmptyArgsVoidDelegate onYes, EmptyArgsVoidDelegate onNo = null, EmptyArgsVoidDelegate alwaysCall = null)
        {
            _onMessageBoxYes = onYes;
            _onMessageBoxNo = onNo;
            _onMessageBoxClose = alwaysCall;

            MessageBoxText.StringValue = text;
            ShowElement("MessageBox");
            ShowElement("MessageBoxNo");
            ShowElement("MessageBoxYes");
            ShowElement("MessageBoxCancel");
            HideElement("MessageBoxOk");

            HideElement("MessageBoxNo2");
            HideElement("MessageBoxYes2");
        }

        public void AddTiledLayer()
        {
            Document.Current.AddTilesetLayer();
        }

        public void AddVectorLayer()
        {
            Document.Current.AddVectorLayer();
        }

        public void RemoveLayer()
        {
            if ( Document.Current.Layers.Count == 1 )
            {
                MessageBox("Cannot remove last layer!");
                return;
            }

            MessageBoxYesNo(String.Format("Do you want to delete layer \n{0}?", Document.Current.SelectedLayer.Name ), () =>
            {
                Document.Current.RemoveSelectedLayer();
            });
        }

        public void SelectLayer(DocLayer layer)
        {
            Document.Current.Select(layer);
        }

        public void OnMessageBoxCancel()
        {
            CloseMessageBox();
        }

        public void OnMessageBoxYes()
        {
            if (_onMessageBoxYes != null)
            {
                _onMessageBoxYes();
            }

            CloseMessageBox();
        }

        public void OnMessageBoxNo()
        {
            if (_onMessageBoxNo != null)
            {
                _onMessageBoxNo();
            }
            CloseMessageBox();
        }

        void CloseMessageBox()
        {
            HideElement("MessageBox");

            if ( _onMessageBoxClose != null )
            {
                _onMessageBoxClose();
                _onMessageBoxClose = null;
            }
        }

        void Open(string path)
        {

        }
    }
}

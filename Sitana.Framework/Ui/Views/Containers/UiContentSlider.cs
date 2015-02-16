using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.DefinitionFiles;
using Sitana.Framework.Xml;
using Sitana.Framework.Diagnostics;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Views.Parameters;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui.Interfaces;

namespace Sitana.Framework.Ui.Views
{
    public class UiContentSlider : UiContainer, IIndexedElement
    {
        public new static void Parse(XNode node, DefinitionFile file)
        {
            UiContainer.Parse(node, file);

            DefinitionParser parser = new DefinitionParser(node);

            file["SelectedIndex"] = parser.ParseInt("SelectedIndex");
            file["TransitionTime"] = parser.ParseDouble("TransitionTime");
            file["Cycle"] = parser.ParseBoolean("Cycle");

            foreach (var cn in node.Nodes)
            {
                switch (cn.Tag)
                {
                    case "UiContentSlider.ShowTransitionEffect":
                    case "UiContentSlider.HideTransitionEffect":
                    case "UiContentSlider.ShowTransitionEffectNext":
                    case "UiContentSlider.HideTransitionEffectNext":
                    case "UiContentSlider.ShowTransitionEffectPrev":
                    case "UiContentSlider.HideTransitionEffectPrev":
                        ParseTransitionEffect(cn, file);
                        break;
                }
            }
        }

        static void ParseTransitionEffect(XNode cn, DefinitionFile file)
        {
            string id = cn.Tag.Split('.')[1];

            if (cn.Nodes.Count != 1)
            {
                string error = cn.NodeError(String.Format("{0} must have exactly 1 child.", cn.Tag));
                if (DefinitionParser.EnableCheckMode)
                {
                    ConsoleEx.WriteLine(error);
                }
                else
                {
                    throw new Exception(error);
                }
            }

            file[id] = DefinitionFile.LoadFile(cn.Nodes[0]);
        }

        UiView _current;
        UiView _previous;

        TransitionEffect _transitionEffectShowNext;
        TransitionEffect _transitionEffectShowPrev;
        TransitionEffect _transitionEffectHideNext;
        TransitionEffect _transitionEffectHidePrev;

        bool _cycle = false;

        int _selectedIndex = 0;
        float _transition = 0;
        private float _transitionSpeed = 1;

        bool _next = false;

        public override void Remove(UiView view)
        {
            base.Remove(view);

            if (view == _current)
            {
                if (_children.Count > 0)
                {
                    _current = _children[Math.Min(_children.Count - 1, _selectedIndex)];
                }
                else
                {
                    _current = null;
                }
            }
        }

        protected override void Update(float time)
        {
            base.Update(time);

            if (_transition > 0 )
            {
                _transition -= _transitionSpeed * time;
                if (_transition <= 0)
                {
                    _transition = 0;
                    _previous = null;
                }
            }
        }

        protected override void Draw(ref Parameters.UiViewDrawParameters parameters)
        {
            float opacity = parameters.Opacity;

            if (opacity == 0)
            {
                return;
            }

            DrawBackground(ref parameters);

            if (_clipChildren)
            {
                parameters.DrawBatch.PushClip(ScreenBounds);
            }

            for (int idx = 0; idx < 2; ++idx)
            {
                UiView view = idx == 0 ? _previous : _current;
                float transition = _transition;

                if (idx == 0)
                {
                    transition = 1 - transition;
                }

                if (view != null)
                {
                    UiViewDrawParameters drawParams = parameters;

                    drawParams.Opacity = opacity;

                    TransitionEffect transitionEffect = FindTransition(idx == 0);

                    if (transitionEffect != null)
                    {
                        float opacity2;
                        Matrix transform;

                        drawParams.Transition = transition;

                        transitionEffect.Get(transition, ScreenBounds, view.ScreenBounds, out transform, out opacity2);

                        drawParams.Opacity *= opacity2;

                        drawParams.DrawBatch.PushTransform(transform);

                        view.ViewDraw(ref drawParams);

                        drawParams.DrawBatch.PopTransform();
                    }
                    else
                    {
                        view.ViewDraw(ref drawParams);
                    }
                }
            }

            if (_clipChildren)
            {
                parameters.DrawBatch.PopClip();
            }
        }

        TransitionEffect FindTransition(bool hide)
        {
            bool prev = !_next;

            if (hide)
            {
                // prev.
                if (prev)
                {
                    if (_transitionEffectHidePrev != null)
                    {
                        return _transitionEffectHidePrev;
                    }
                }
                // next
                else
                {
                    if (_transitionEffectHideNext != null)
                    {
                        return _transitionEffectHideNext;
                    }
                }
            }
            else
            {
                // prev.
                if (prev)
                {
                    if (_transitionEffectShowPrev != null)
                    {
                        return _transitionEffectShowPrev;
                    }
                }
                // next
                else
                {
                    if (_transitionEffectShowNext != null)
                    {
                        return _transitionEffectShowNext;
                    }
                }
            }

            return null;
        }

        protected override bool Init(object controller, object binding, DefinitionFile definition)
        {
            if (!base.Init(controller, binding, definition))
            {
                return false;
            }

            InitChildren(Controller, Binding, definition);

            DefinitionFileWithStyle file = new DefinitionFileWithStyle(definition, typeof(UiNavigationView));

            _selectedIndex = DefinitionResolver.Get<int>(Controller, Binding, file["SelectedIndex"], 0);
            _cycle = DefinitionResolver.Get<bool>(Controller, Binding, file["Cycle"], false);

            double speed = DefinitionResolver.Get<double>(Controller, Binding, file["TransitionTime"], 500) / 1000.0;
            _transitionSpeed = (float)(speed > 0 ? 1 / speed : 10000000);

            DefinitionFile transitionEffectFile = file["ShowTransitionEffectNext"] as DefinitionFile;

            if (transitionEffectFile != null)
            {
                _transitionEffectShowNext = transitionEffectFile.CreateInstance(Controller, Binding) as TransitionEffect;
            }

            transitionEffectFile = file["HideTransitionEffectNext"] as DefinitionFile;

            if (transitionEffectFile != null)
            {
                _transitionEffectHideNext = transitionEffectFile.CreateInstance(Controller, Binding) as TransitionEffect;
            }

            transitionEffectFile = file["ShowTransitionEffectPrev"] as DefinitionFile;

            if (transitionEffectFile != null)
            {
                _transitionEffectShowPrev = transitionEffectFile.CreateInstance(Controller, Binding) as TransitionEffect;
            }

            transitionEffectFile = file["HideTransitionEffectPrev"] as DefinitionFile;

            if (transitionEffectFile != null)
            {
                _transitionEffectHidePrev = transitionEffectFile.CreateInstance(Controller, Binding) as TransitionEffect;
            }

            transitionEffectFile = file["ShowTransitionEffect"] as DefinitionFile;

            if (transitionEffectFile != null)
            {
                if ( _transitionEffectShowPrev == null )
                {
                    _transitionEffectShowPrev = transitionEffectFile.CreateInstance(Controller, Binding) as TransitionEffect;
                }

                if ( _transitionEffectShowNext == null )
                {
                    _transitionEffectShowNext = transitionEffectFile.CreateInstance(Controller, Binding) as TransitionEffect;
                }
            }

            transitionEffectFile = file["HideTransitionEffect"] as DefinitionFile;

            if (transitionEffectFile != null)
            {
                if ( _transitionEffectHidePrev == null )
                {
                    _transitionEffectHidePrev = transitionEffectFile.CreateInstance(Controller, Binding) as TransitionEffect;
                }

                if ( _transitionEffectHideNext == null )
                {
                    _transitionEffectHideNext = transitionEffectFile.CreateInstance(Controller, Binding) as TransitionEffect;
                }
            }

            if (_transitionEffectHideNext == null)
            {
                if (_transitionEffectShowNext != null)
                {
                    _transitionEffectHideNext = _transitionEffectShowNext.Reverse();
                }
            }

            if (_transitionEffectHidePrev == null)
            {
                if (_transitionEffectShowPrev != null)
                {
                    _transitionEffectHidePrev = _transitionEffectShowPrev.Reverse();
                }
            }

            if (_children.Count > 0)
            {
                _current = _children[_selectedIndex];
            }

            _previous = null;

            return true;
        }

        public void ShowNext()
        {
            int prev = _selectedIndex;

            if (_cycle)
            {
                _selectedIndex = (_selectedIndex +  1) % _children.Count;
            }
            else
            {
                _selectedIndex = Math.Min(_selectedIndex + 1, _children.Count-1);
            }

            if (prev != _selectedIndex)
            {
                _transition = 1;
                _next = true;
                _previous = _children[prev];
                _current = _children[_selectedIndex];
            }
        }

        public void ShowPrev()
        {
            int prev = _selectedIndex;

            if (_cycle)
            {
                _selectedIndex = (_selectedIndex + _children.Count - 1) % _children.Count;
            }
            else
            {
                _selectedIndex = Math.Max(_selectedIndex - 1, 0);
            }

            if (prev != _selectedIndex)
            {
                _transition = 1;
                _next = false;
                _previous = _children[prev];
                _current = _children[_selectedIndex];
            }
        }

        public void Switch(int index)
        {
            _previous = null;
            _transition = 0;
            _next = false;

            _current = _children[index];
            _selectedIndex = index;
        }

        int IIndexedElement.Count
        {
            get
            {
                return _children.Count;
            }
        }

        public UiView SelectedChild
        {
            get
            {
                return _children[_selectedIndex];
            }

            set
            {
                for (int idx = 0; idx < _children.Count; ++idx)
                {
                    if (_children[idx] == value)
                    {
                        SelectedIndex = idx;
                        break;
                    }
                }
            }
        }

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }

            set
            {
                if (value != _selectedIndex)
                {
                    if (_selectedIndex < _children.Count)
                    {
                        _previous = _children[_selectedIndex];
                        _transition = 1;
                    }
                    else
                    {
                        _previous = null;
                        _transition = 0;
                    }

                    _next = value > _selectedIndex;
                    _selectedIndex = value;
                }

                _current = _children[value];
            }
        }

        void IIndexedElement.GetText(SharedString text, int index)
        {
            var child = _children[index];

            if (child.Tag != null)
            {
                lock(child.Tag)
                {
                    text.Copy(child.Tag.StringBuilder);
                }
            }
            else
            {
                text.Format("{0}", index + 1);
            }
        }
    }
}

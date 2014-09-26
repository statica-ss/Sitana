/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
/// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
/// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
///
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
/// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
/// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// contact@ebatianos.com
/// www.ebatianos.com/essentials-library
/// 
///---------------------------------------------------------------------------

using Sitana.Framework.Content;
using Sitana.Framework;
using Sitana.Framework.Cs;
using Sitana.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;

namespace Sitana.Framework.Gui
{
    public class Accordion : GuiElement
    {
        private enum AccordionMode
        {
            Single,
            Multi,
            Always
        }

        private List<AccordionGroup> _groups = new List<AccordionGroup>();
        private AccordionMode _mode = AccordionMode.Single;

        private Texture2D _localSeparator;
        private Texture2D _globalSeparator;
        private Texture2D _groupArrow;

        private Color _groupArrowColor;

        private Boolean _redraw = true;
        private AccordionGroup _selectedGroup = null;

        private Texture2D _footer;
        private Vector2 _footerPosition;
        private Single _footerMargin;

        private VerticalScroller _verticalScroller;

        /// <summary>
        /// Draws button.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch object used to render textures and texts.</param>
        /// <param name="color">Color to multiply all contents by.</param>
        /// <param name="offset">Offset to move bnutton by.</param>
        public override void Draw(Int32 level, SpriteBatch spriteBatch, Vector2 topLeft, Single transition)
        {
            if (!DrawLevel(level))
            {
                return;
            }

            if (Opacity <= 0)
            {
                return;
            }

            Vector2 position = new Vector2(ElementRectangle.X, ElementRectangle.Y) + topLeft;
            position.Y -= _verticalScroller.Position;

            Int32 lastVisibleGroup = 0;
            for (Int32 idx = 0; idx < _groups.Count; ++idx)
            {
                if ( _groups[idx].IsVisible )
                {
                    lastVisibleGroup = idx;
                }
            }

            AccordionGroup groupPrev = null;

            for ( Int32 idx = 0; idx < _groups.Count; ++idx )
            {
                var group = _groups[idx];

                if ( !group.IsVisible )
                {
                    continue;
                }

                if (position.Y + group.DrawHeight >= ElementRectangle.Top)
                {
                    if (idx > 0)
                    {
                        if (group.IsExpanded)
                        {
                            Vector2 separatorScale = new Vector2((Single)ElementRectangle.Width / (Single)_globalSeparator.Width, 1);

                            spriteBatch.Draw(_globalSeparator, position, null, Color.White, 0, Vector2.Zero, separatorScale, SpriteEffects.None, 0);
                        }
                        else if (groupPrev == null || !groupPrev.IsExpanded)
                        {
                            Vector2 separatorScale = new Vector2((Single)ElementRectangle.Width / (Single)_localSeparator.Width, 1);
                            spriteBatch.Draw(_localSeparator, position, null, Color.White, 0, Vector2.Zero, separatorScale, SpriteEffects.None, 0);
                        }
                    }

                    group.Draw(level, spriteBatch, position, transition, _localSeparator, _groupArrow, _groupArrowColor);
                }
                
                position.Y += group.DrawHeight;

                if ( group.IsExpanded )
                {
                    Vector2 separatorScale = new Vector2((Single)ElementRectangle.Width / (Single)_globalSeparator.Width, 1);

                    spriteBatch.Draw(_globalSeparator, position, null, Color.White, 0, Vector2.Zero, separatorScale, SpriteEffects.None, 0);
                }
                else if (idx == lastVisibleGroup)
                {
                    Vector2 separatorScale = new Vector2((Single)ElementRectangle.Width / (Single)_localSeparator.Width, 1);
                    spriteBatch.Draw(_localSeparator, position, null, Color.White, 0, Vector2.Zero, separatorScale, SpriteEffects.None, 0);
                }

                if (position.Y > ElementRectangle.Bottom)
                {
                    break;
                }

                groupPrev = group;
            }

            if (_footer != null && Owner.State != Screen.ScreenState.TransitionOut )
            {
                Vector2 footerPosition = new Vector2(_footerPosition.X, Math.Max(position.Y + _footerMargin, _footerPosition.Y + ElementRectangle.Y));
                spriteBatch.Draw(_footer, footerPosition, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            }
        }

        public override bool Update(TimeSpan gameTime, Screen.ScreenState screenState)
        {
            Boolean redraw = _redraw || base.Update(gameTime, screenState);
            _redraw = false;

            if ( screenState != Screen.ScreenState.Visible )
            {
                return redraw;
            }

            Vector2 topLeft = new Vector2(ElementRectangle.X, ElementRectangle.Y);

            Int32 height = 0;
            Int32 lastHeight = 0;

            for (Int32 idx = 0; idx < _groups.Count; ++idx)
            {
                var group = _groups[idx];

                if (group.IsVisible)
                {
                    redraw |= group.Update(gameTime, topLeft - new Vector2(0, _verticalScroller.Position), screenState);
                    lastHeight = group.DrawHeight;

                    topLeft.Y += lastHeight;
                    height += lastHeight;
                }
            }

            height += lastHeight / 4;

            if ( _footer != null )
            {
                height += (Int32)(_footer.Height * Scale.Y + _footerMargin);
            }

            Single time = (Single)gameTime.TotalSeconds;

            redraw |= _verticalScroller.Update(time, height);

            return redraw;
        }

        private AccordionGroup FromPoint(Point point, Boolean withContent)
        {
            if (ElementRectangle.Contains(point))
            {
                Vector2 topLeft = new Vector2(ElementRectangle.X, ElementRectangle.Y - _verticalScroller.Position);

                for (Int32 idx = 0; idx < _groups.Count; ++idx)
                {
                    var group = _groups[idx];

                    if (group.IsVisible)
                    {
                        Int32 pos = point.Y - (Int32)topLeft.Y;

                        Int32 height = withContent ? group.DrawHeight : group.HeaderHeight;

                        if (pos >= 0 && pos < height)
                        {
                            return group;
                        }

                        topLeft.Y += group.DrawHeight + _localSeparator.Height;
                    }
                }
            }

            return null;
        }

        private void HandleTapGesture(Object sender, GestureEventArgs args)
        {
            if ( Owner.State != Screen.ScreenState.Visible )
            {
                return;
            }

            if (_selectedGroup != null)
            {
                _selectedGroup.Selected = false;
                _selectedGroup = null;
            }
                
            Point point = GraphicsHelper.PointFromVector2(args.Sample.Position);

            AccordionGroup group = FromPoint(point, false);

            if (group != null)
            {
                if (group.HasContent)
                {
                    Boolean expand = !group.IsExpanded;

                    if (_mode == AccordionMode.Single)
                    {
                        for (Int32 idx2 = 0; idx2 < _groups.Count; ++idx2)
                        {
                            _groups[idx2].IsExpanded = false;
                        }
                    }

                    if (_mode != AccordionMode.Always)
                    {
                        group.IsExpanded = expand;
                    }
                }
                else
                {
                    group.DoAction();
                }
            }

            _verticalScroller.HandleClickGesture(sender, args);
        }

        private void HandleTouchDownGesture(Object sender, GestureEventArgs args)
        {
            if ( Owner.State != Screen.ScreenState.Visible )
            {
                return;
            }

            _verticalScroller.HandleTouchDownGesture(sender, args);

            Point pos = GraphicsHelper.PointFromVector2(args.Sample.Position);

            if ( _selectedGroup != null )
            {
                _selectedGroup.Selected = false;
                _redraw = true;
            }

            var group = FromPoint(pos, false);

            if ( group != null )
            {
                group.Selected = true;
                _selectedGroup = group;
                _redraw = true;
            }

            InputHandler.Current.PointerInvalidated += HandlePointerInvalidated;
        }

        private void HandlePointerInvalidated(Object sender, GestureEventArgs args)
        {
            if (_selectedGroup != null)
            {
                _selectedGroup.Selected = false;
                _selectedGroup = null;
            }

            _redraw = true;
            InputHandler.Current.PointerInvalidated -= HandleTouchUpGesture;
        }

        private void HandleTouchUpGesture(Object sender, GestureEventArgs args)
        {
            _verticalScroller.HandleTouchUpGesture(sender, args);

            if (_selectedGroup != null)
            {
                _selectedGroup.Selected = false;
                _selectedGroup = null;
            }

            _redraw = true;
        }

        private void HandleHorizontalDragGesture(Object sender, GestureEventArgs args)
        {
            _verticalScroller.HandleHorizontalDragGesture(sender, args);

            if (!_verticalScroller.CanDoGesture)
            {
                return;
            }

            if (_selectedGroup != null)
            {
                _selectedGroup.Selected = false;
                _selectedGroup = null;
            }

            args.Handled = true;
        }

        private void HandleVerticalDragGesture(Object sender, GestureEventArgs args)
        {
            if ( Owner.State != Screen.ScreenState.Visible )
            {
                return;
            }

            if (_selectedGroup != null)
            {
                _selectedGroup.Selected = false;
                _selectedGroup = null;
            }

            _verticalScroller.HandleVerticalDragGesture(sender, args);
        }

        private void HandleFlickGesture(Object sender, GestureEventArgs args)
        {
            if ( Owner.State != Screen.ScreenState.Visible )
            {
                return;
            }

            _verticalScroller.HandleFlickGesture(sender, args);

            if (_selectedGroup != null)
            {
                _selectedGroup.Selected = false;
                _selectedGroup = null;
            }
        }

        /// <summary>
        /// Initializes accordion from parameters.
        /// </summary>
        /// <param name="node">XML node entity.</param>
        /// <param name="parameters">Parameters from xml node.</param>
        /// <param name="scale">Current screen scale.</param>
        /// <param name="areaSize">Size of the area.</param>
        /// <param name="owner">Owner screen.</param>
        /// <returns>True when succeeded.</returns>
        protected override Boolean Initialize(InitializeParams initParams)
        {
            XmlFileNode node = initParams.Node;
            Dictionary<String, String> namespaceAliases = initParams.NamespaceAliases;
            ParametersCollection parameters = initParams.Parameters;
            String directory = initParams.Directory;
            Vector2 scale = initParams.Scale;
            Vector2 areaSize = initParams.AreaSize;
            Vector2 offset = initParams.Offset;

            // First unserialize base parameters.
            if (!base.Initialize(initParams))
            {
                return false;
            }

            Point position1 = ParsePosition(parameters, "X1", "Y1", GraphicsHelper.PointFromVector2(areaSize), scale);
            Point position2 = ParsePosition(parameters, "X2", "Y2", GraphicsHelper.PointFromVector2(areaSize), scale);

            ElementRectangle = new Rectangle(position1.X + (Int32)offset.X, position1.Y + (Int32)offset.Y, position2.X-position1.X, position2.Y-position1.Y);

            String font = parameters.AsString("HeaderFont");

            IFontPresenter fontObj = FontLoader.Load(font);

            Int32 height = parameters.AsInt32("HeaderHeight");

            Int32 headerTextMargin = parameters.AsInt32("HeaderTextMargin");
            Point headerMediaMargin = parameters.ParsePoint("HeaderMargin");

            Color[] colors = new Color[(Int32)AccordionGroup.Colors.Count];

            colors[(Int32)AccordionGroup.Colors.ContentBgColor] = parameters.AsColor("ContentBgColor");
            colors[(Int32)AccordionGroup.Colors.HeaderBgColor] = parameters.AsColor("HeaderBgColor");
            colors[(Int32)AccordionGroup.Colors.HeaderTextColor] = parameters.AsColor("HeaderTextColor");
            colors[(Int32)AccordionGroup.Colors.HeaderExpandedIconColor] = parameters.AsColor("HeaderExpandedIconColor");
            colors[(Int32)AccordionGroup.Colors.HeaderCollapsedIconColor] = parameters.AsColor("HeaderCollapsedIconColor");

            _mode = parameters.AsEnum<AccordionMode>("Mode", AccordionMode.Single);

            Int32 contentHeight = parameters.AsInt32("ContentHeight");

            _globalSeparator = ContentLoader.Current.Load<Texture2D>(parameters.AsString("GlobalSeparator"));
            _localSeparator = ContentLoader.Current.Load<Texture2D>(parameters.AsString("LocalSeparator"));

            try
            {
                _groupArrow = ContentLoader.Current.Load<Texture2D>(parameters.AsString("GroupArrow"));
                _groupArrowColor = parameters.AsColor("GroupArrowColor");
            }
            catch
            {

            }

            for (Int32 idx = 0; idx < node.Nodes.Count; ++idx )
            {
                if (node.Nodes[idx].Tag == "Footer")
                {
                    AddFooter(node.Nodes[idx], directory, offset);
                }
                else
                {
                    AddGroup(node.Nodes[idx], namespaceAliases, directory, fontObj, scale, height, contentHeight, headerTextMargin, headerMediaMargin, colors);
                }
            }

            if ( _mode == AccordionMode.Always )
            {
                for (Int32 idx = 0; idx < _groups.Count; ++idx)
                {
                    var group = _groups[idx];
                    group.IsExpanded = true;
                    group.Update(TimeSpan.FromSeconds(1), Vector2.Zero, Screen.ScreenState.TransitionIn);
                }
            }

            InstallGestureHandler(GestureAdditionalType.Native, GestureType.VerticalDrag, HandleVerticalDragGesture);
            InstallGestureHandler(GestureAdditionalType.Native, GestureType.HorizontalDrag, HandleHorizontalDragGesture);
            InstallGestureHandler(GestureAdditionalType.Native, GestureType.Tap, HandleTapGesture);

            InstallGestureHandler(GestureAdditionalType.Native, GestureType.Flick, HandleFlickGesture);

            InstallGestureHandler(GestureAdditionalType.TouchDown, GestureType.None, HandleTouchDownGesture);
            InstallGestureHandler(GestureAdditionalType.TouchUp, GestureType.None, HandleTouchUpGesture);

            ClipToElement = true;

            _verticalScroller = new VerticalScroller(ElementRectangle, Scale.Y * 100);
            return true;
        }

        private void AddFooter(XmlFileNode node, String directory, Vector2 offset)
        {
            ParametersCollection parameters = node.Attributes;

            _footer = ContentLoader.Current.Load<Texture2D>(parameters.AsString("Image"));

            Point pos = ParsePosition(parameters, "X", "Y", new Point(ElementRectangle.Width, ElementRectangle.Height), Scale);
            Point size = GraphicsHelper.PointFromVector2(new Vector2(_footer.Width, _footer.Height) * Scale);

            Align align = parameters.AsAlign("Align", "Valign");
            Rectangle rect = GuiElement.RectangleFromAlignAndSize(pos, size, align, offset);

            _footerMargin = parameters.AsSingle("Margin") * Scale.Y;

            _footerPosition = new Vector2(rect.X, rect.Y);
        }

        private void AddGroup(XmlFileNode node, Dictionary<String, String> namespaceAliases, String directory, IFontPresenter fontObj, Vector2 scale, Int32 height, Int32 contentHeight, Int32 headerTextMargin, Point headerMediaMargin, Color[] colors)
        {
            if ( node.Tag != "Group")
            {
                throw new Exception("Invalid xml node. Expected: Group.");
            }

            Boolean visible = true;

            if ( node.Attributes.HasKey("IsVisible") && !node.Attributes.AsBoolean("IsVisible") )
            {
                visible = false;
            }

            String text = node.Attributes.AsString("Text");

            if (node.Attributes.HasKey("Font"))
            {
                String font = node.Attributes.AsString("Font");
                fontObj = FontLoader.Load(font);
            }

            Texture2D iconObj = null;

            if (node.Attributes.HasKey("Icon"))
            {
                String icon = node.Attributes.AsString("Icon");
                iconObj = ContentLoader.Current.Load<Texture2D>(icon);
            }

            String id = node.Attributes.AsString("Id");

            Action action = node.Attributes.AsAction("OnClick", this);

            var group = new AccordionGroup(id, fontObj, iconObj, text, scale, height, headerTextMargin, headerMediaMargin, colors, ElementRectangle.Width, action);
            _groups.Add(group);

            for ( Int32 idx = 0; idx < node.Nodes.Count; ++idx )
            {
                Vector2 areaSize = new Vector2((Single)ElementRectangle.Width / scale.X, (Int32)((Single)contentHeight * scale.Y + 0.95f));
                AddContent(group, node.Nodes[idx], namespaceAliases, directory, scale, areaSize);
            }

            group.IsVisible = visible;
        }

        public void UpdateGroup(String id, String text, Boolean? expanded, Boolean? visible)
        {
            for ( Int32 idx = 0; idx < _groups.Count; ++idx )
            {
                if ( _groups[idx].Id == id )
                {
                    var group = _groups[idx];

                    if ( text != null )
                    {
                        group.UpdateText(text);
                    }

                    if ( expanded.HasValue )
                    {
                        group.IsExpanded = expanded.Value;
                    }

                    if (visible.HasValue)
                    {
                        group.IsVisible = visible.Value;
                    }
                }
            }

            _redraw = true;
        }

        private void AddContent(AccordionGroup group, XmlFileNode node, Dictionary<String, String> namespaceAliases, String directory, Vector2 scale, Vector2 areaSize )
        {
            switch(node.Tag)
            {
                case "Control":
                    {
                        if (node.Attributes.AsBoolean("IsVisible", true))
                        {
                            group.AddElement(node.Nodes[0], namespaceAliases, directory, scale, areaSize, Owner);
                        }
                    }
                    return;

                case "ControlsBuilder":
                    {
                        Int32 count = node.Attributes.AsInt32("Count");

                        for (Int32 idx = 0; idx < count; ++idx )
                        {
                            node.Nodes[0].Attributes.MethodCallIndexArgument = idx;
                            group.AddElement(node.Nodes[0], namespaceAliases, directory, scale, areaSize, Owner);
                        }
                    }

                    return;
            }

            throw new Exception("Invalid xml node: " + node.Tag + ".");
        }

        public void DisableScroll()
        {
            _verticalScroller.Enable = false;
        }
    }
}

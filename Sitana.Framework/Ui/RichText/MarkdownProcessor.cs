using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonMark.Syntax;
using System.IO;
using CommonMark;

namespace Sitana.Framework.Ui.RichText
{
    public class MarkdownProcessor: IRichProcessor
    {
        private List<Line> _lines = new List<Line>();

        List<Line> IRichProcessor.Lines
        {
            get
            {
                return _lines;
            }
        }

        void IRichProcessor.Process(string text)
        {
            _lines.Clear();

            var reader = new StringReader(text);
            Block document = CommonMarkConverter.ProcessStage1(reader);
            CommonMarkConverter.ProcessStage2(document);

            TagProperties props = TagProperties.Default;

            Line line = new Line();
            Process(document, ref props, ref line);

            Console.WriteLine();
        }

        private void Process(Block block, ref TagProperties tagProperties, ref Line currentLine)
        {
            while (block != null)
            {
                switch (block.Tag)
                {
                    case BlockTag.Document:
                        Process(block.FirstChild, ref tagProperties, ref currentLine);
                        break;

                    case BlockTag.Paragraph:

                        if (!tagProperties.IsTight)
                        {
                            currentLine = AddLine();
                        }

                        ProcessInlines(block.InlineContent, ref tagProperties, ref currentLine);
                        break;

                    case BlockTag.BlockQuote:
                        {
                            TagProperties newProps = tagProperties;
                            newProps.IsTight = false;
                            ProcessInlines(block.InlineContent, ref newProps, ref currentLine);
                        }
                        break;

                    case BlockTag.ListItem:
                        {
                            currentLine = AddLine();
                            currentLine.Add(new Entity(tagProperties.ListIndex > 0 ? EntityType.ListNumber : EntityType.ListBullet, ref tagProperties, null));

                            {
                                TagProperties newProps = tagProperties;
                                newProps.IsTight = true;
                                Process(block.FirstChild, ref newProps, ref currentLine);
                            }

                            if (tagProperties.ListIndex > 0)
                            {
                                tagProperties.ListIndex++;
                            }
                        }
                        break;

                    case BlockTag.List:
                        {
                            TagProperties newProps = tagProperties;
                            newProps.IsTight = block.ListData.IsTight;
                            newProps.ListIndex = block.ListData.ListType == ListType.Ordered ? 1 : -1;
                            Process(block.FirstChild, ref newProps, ref currentLine);
                        }
                        break;

                    case BlockTag.AtxHeader:
                    case BlockTag.SETextHeader:
                        {
                            currentLine = AddLine();
                            TagProperties newProps = tagProperties;

                            newProps.FontSize = (SizeType)block.HeaderLevel;
                            ProcessInlines(block.InlineContent, ref newProps, ref currentLine);
                        }
                        break;

                    case BlockTag.IndentedCode:
                        Console.WriteLine(block.StringContent);
                        break;

                    case BlockTag.FencedCode:
                        ProcessInlines(block.InlineContent, ref tagProperties, ref currentLine);
                        break;

                    case BlockTag.HtmlBlock:
                        ProcessInlines(block.InlineContent, ref tagProperties, ref currentLine);
                        break;

                    case BlockTag.HorizontalRuler:

                        currentLine = AddLine();
                        currentLine.Add(new Entity(EntityType.HorizontalLine, ref tagProperties, null));
                        currentLine = AddLine();

                        break;

                    case BlockTag.ReferenceDefinition:
                        break;

                    default:
                        throw new CommonMarkException("Block type " + block.Tag + " is not supported.", block);
                }

                block = block.NextSibling;
            }
        }

        private void ProcessInlines(Inline inline, ref TagProperties tagProperties, ref Line currentLine)
        {
            while (inline != null)
            {
                switch (inline.Tag)
                {
                    case InlineTag.String:

                        if (currentLine.Entities.Count > 0)
                        {
                            Entity? merged = Entity.MergeString(currentLine.Entities.Last(), ref tagProperties, inline.LiteralContent);

                            if (merged.HasValue)
                            {
                                currentLine.Entities.RemoveAt(currentLine.Entities.Count - 1);
                                currentLine.Add(merged.Value);
                                break;
                            }
                        }
                        

                        currentLine.Add(new Entity(EntityType.String, ref tagProperties, inline.LiteralContent));
                        break;

                    case InlineTag.LineBreak:
                        currentLine = AddLine();
                        break;

                    case InlineTag.SoftBreak:
                        currentLine = AddLine();

                        if (tagProperties.ListIndex != 0)
                        {
                            currentLine.Add(new Entity(EntityType.ListIndent, ref tagProperties, null));
                        }

                        break;

                    case InlineTag.Code:
                        Console.WriteLine(inline.LiteralContent);
                        break;

                    case InlineTag.RawHtml:
                        Console.WriteLine(inline.LiteralContent);
                        break;

                    case InlineTag.Link:
                        {
                            TagProperties newProps = tagProperties;
                            newProps.Url = inline.TargetUrl;

                            ProcessInlines(inline.FirstChild, ref newProps, ref currentLine);
                        }
                        break;

                    case InlineTag.Image:
                        {
                            currentLine.Add(new Entity(EntityType.Image, ref tagProperties, inline.TargetUrl));
                        }
                        break;

                    case InlineTag.Strong:
                        {
                            TagProperties newProps = tagProperties;
                            switch (newProps.FontType)
                            {
                                case FontType.i:
                                    newProps.FontType = FontType.bi;
                                    break;

                                default:
                                    newProps.FontType = FontType.b;
                                    break;
                            }

                            ProcessInlines(inline.FirstChild, ref newProps, ref currentLine);
                        }
                        break;

                    case InlineTag.Emphasis:
                        {
                            TagProperties newProps = tagProperties;
                            switch (newProps.FontType)
                            {
                                case FontType.b:
                                    newProps.FontType = FontType.bi;
                                    break;

                                default:
                                    newProps.FontType = FontType.i;
                                    break;
                            }

                            ProcessInlines(inline.FirstChild, ref newProps, ref currentLine);
                        }
                        break;

                    case InlineTag.Strikethrough:
                        break;

                    default:
                        throw new CommonMarkException("Inline type " + inline.Tag + " is not supported.", inline);
                }

                inline = inline.NextSibling;
            }
        }

        Line AddLine()
        {
            var line = new Line() { Entities = new List<Entity>() };
            _lines.Add(line);
            return line;
        }
    }
}

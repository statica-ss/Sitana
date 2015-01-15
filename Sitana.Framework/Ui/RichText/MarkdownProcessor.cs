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
        private List<Line> _lines;

        void IRichProcessor.Process(List<Line> lines, string text)
        {
            if (_lines != null)
            {
                throw new Exception("Processing already in progress...");
            }

            _lines = lines;
            _lines.Clear();

            var reader = new StringReader(text);
            Block document = CommonMarkConverter.ProcessStage1(reader);
            CommonMarkConverter.ProcessStage2(document);

            TagProperties props = TagProperties.Default;

            Line line = new Line();
            Process(document, ref props, ref line);

            Console.WriteLine();
            _lines = null;
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

                        currentLine = AddLine();
                        ProcessInlines(block.InlineContent, ref tagProperties, ref currentLine);

                        break;

                    case BlockTag.BlockQuote:
                        ProcessInlines(block.InlineContent, ref tagProperties, ref currentLine);
                        break;

                    case BlockTag.ListItem:
                        {
                            TagProperties newProps = tagProperties;
                            Process(block.FirstChild, ref tagProperties, ref currentLine);
                            tagProperties.ListIndex++;
                        }
                        break;

                    case BlockTag.List:
                        {
                            TagProperties newProps = tagProperties;
                            newProps.ListIndex = 1;
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
                        currentLine.AddString(ref tagProperties, inline.LiteralContent);
                        break;

                    case InlineTag.LineBreak:
                        currentLine = AddLine();
                        break;

                    case InlineTag.SoftBreak:
                        currentLine = AddLine();
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

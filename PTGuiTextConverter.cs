//
// Author:
//   Michael Göricke
//
// Copyright (c) 2024
//
// This file is part of PTGui Language Editor.
//
// The PTGui Language Editor is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see<http://www.gnu.org/licenses/>.

using System;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;

namespace PTGui_Language_Editor
{
    internal static class PTGuiTextConverter
    {
        static FlowDocument myFlowDoc = null!;
        static Paragraph paragraph = null!;
        static StringBuilder sb = null!;
        static bool inCommand;
        static bool inEscape;
        static bool isBold;
        static bool inReplacement;
        static bool isRed;
        static bool isMac;
        static bool isWindows;
        static bool isPro;
        static bool isError;
        static bool isHyperlink;
        static bool isUnderlineNextChar;

        public static FlowDocument ConvertToFlowDocument(string? text, bool isHtml, Func<string, string?> replaceId)
        {
            if (text == null)
            {
                return new FlowDocument();
            }

            try
            {
                inCommand = false;
                inEscape = false;
                isBold = false;
                inReplacement = false;
                isRed = false;
                isMac = false;
                isWindows = false;
                isPro = false;
                isError = false;
                isHyperlink = false;
                isUnderlineNextChar = false;

                myFlowDoc = new FlowDocument();
                myFlowDoc.TextAlignment = System.Windows.TextAlignment.Left;
                paragraph = new Paragraph();
                ConvertToFlowDocument2(text, isHtml, replaceId);
                myFlowDoc.Blocks.Add(paragraph);
            }
            catch
            {
                AddText();
                var run = new Bold(new Run("Error"));
                run.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                run.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                paragraph.Inlines.Add(run);
                myFlowDoc.Blocks.Add(paragraph);
            }

            return myFlowDoc;
        }

        static void AddText()
        {
            Inline run;

            if (sb.Length == 0)
            {
                return;
            }

            run = new Run(sb.ToString());

            if (isBold)
            {
                run = new Bold(run);
            }

            if (isUnderlineNextChar)
            {
                run = new Underline(run);
            }

            if (isHyperlink)
            {
                run = new Underline(run);
                run.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            }

            if (isRed)
            {
                run.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }

            if (isPro)
            {
                run.Background = new SolidColorBrush(Color.FromRgb(255, 240, 240));
            }

            if (isMac)
            {
                run.Background = new SolidColorBrush(Color.FromRgb(200, 255, 200));
            }
            else
            if (isWindows)
            {
                run.Background = new SolidColorBrush(Color.FromRgb(200, 255, 255));
            }

            if (isError)
            {
                run.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }

            paragraph.Inlines.Add(run);
            sb = new StringBuilder();
        }

        static void ConvertToFlowDocument2(string text, bool isHtml, Func<string, string?> replaceId)
        {
            int index = 0;
            bool firstToUpper = false;
            sb = new StringBuilder();
            StringBuilder command = null!;

            while (index < text.Length)
            {
                if (!isHtml && text[index] == '&')
                {
                    AddText();
                    isUnderlineNextChar = true;
                    index++;
                    command = new();
                }
                else if (isUnderlineNextChar)
                {
                    sb.Append(text[index]);
                    AddText();
                    isUnderlineNextChar = false;
                    index++;
                }
                else if (isHtml && text[index] == '&')
                {
                    AddText();
                    inEscape = true;
                    index++;
                    command = new();
                }
                else if (inEscape)
                {
                    if (text[index] == ';')
                    {
                        index++;
                        inEscape = false;
                        var cmd = command.ToString();

                        if (cmd == "amp")
                        {
                            sb.Append('&');
                            AddText();
                        }
                        else
                        if (cmd == "quot")
                        {
                            sb.Append('"');
                            AddText();
                        }
                        else
                        if (cmd == "apos")
                        {
                            sb.Append('\'');
                            AddText();
                        }
                        else
                        {
                            sb.Append('&');
                            sb.Append(cmd);
                            sb.Append(';');
                            AddText();
                        }
                    }
                    else
                    {
                        command.Append(text[index]);
                        index++;
                    }
                }
                else
                if (text[index] == '<')
                {
                    AddText();
                    inCommand = true;
                    index++;
                    command = new();
                }
                else if (inCommand)
                {
                    if (text[index] == '>')
                    {
                        AddText();
                        inCommand = false;
                        index++;

                        var cmd = command.ToString();

                        if (isHtml && cmd == "br")
                        {
                            myFlowDoc.Blocks.Add(paragraph);
                            paragraph = new Paragraph();
                        }
                        else if (isHtml && cmd == "b")
                        {
                            isBold = true;
                        }
                        else if (isHtml && cmd == "/b")
                        {
                            isBold = false;
                        }
                        else if (isHtml && cmd == "red")
                        {
                            isRed = true;
                        }
                        else if (isHtml && cmd == "/red")
                        {
                            isRed = false;
                        }
                        else if (isHtml && cmd.StartsWith("a "))
                        {
                            isHyperlink = true;
                        }
                        else if (isHtml && cmd == "/a")
                        {
                            isHyperlink = false;
                        }
                        else if (cmd.StartsWith("optional "))
                        {
                            if (cmd.Contains(" platform='mac'"))
                            {
                                isMac = true;
                            }
                            else if (cmd.Contains(" platform='windows'"))
                            {
                                isWindows = true;
                            }
                            else if (cmd.Contains(" product='pro'"))
                            {
                                isPro = true;
                            }
                            else
                            {
                                isError = true;
                            }
                        }
                        else if (cmd == "/optional")
                        {
                            isMac = false;
                            isWindows = false;
                            isPro = false;
                            isError = false;
                        }
                        else
                        {
                            sb.Append('<');
                            sb.Append(cmd);
                            sb.Append('>');
                            AddText();
                            isError = false;
                        }
                    }
                    else
                    {
                        command.Append(text[index]);
                        index++;
                    }
                }
                else if (text[index] == '@')
                {
                    if (!inReplacement)
                    {
                        AddText();
                        inReplacement = true;
                        index++;

                        if (text[index] == '+')
                        {
                            firstToUpper = true;
                            index++;
                        }
                        else
                        {
                            firstToUpper = false;
                        }
                    }
                    else
                    {
                        index++;
                        inReplacement = false;
                        var id = sb.ToString();
                        var ids = replaceId(id);

                        if (ids != null)
                        {
                            if (firstToUpper)
                            {
                                var first = ids[0].ToString().ToUpper();
                                ids = first + ids.Substring(1);
                            }

                            ConvertToFlowDocument2(ids, isHtml, replaceId);
                        }
                        else
                        {
                            var run = new Run("@" + (firstToUpper ? "+" : string.Empty) + sb.ToString() + "@");
                            run.Background = new SolidColorBrush(Color.FromRgb(255, 140, 140));
                            paragraph.Inlines.Add(run);
                            sb = new StringBuilder();
                        }
                    }
                }
                else if (text[index] == '\n')
                {
                    AddText();
                    index++;
                    myFlowDoc.Blocks.Add(paragraph);
                    paragraph = new Paragraph();
                }
                else
                {
                    sb.Append(text[index]);
                    index++;
                }
            }

            AddText();
        }

        public static string? ConvertToHtml(string? str, bool isHtml)
        {
            if (!string.IsNullOrEmpty(str) && isHtml)
            {
                return str?.Replace("\n", "<br>");
            }

            return str;
        }

        public static string? ConvertFromHtml(string? str, bool isHtml)
        {
            if (!string.IsNullOrEmpty(str) && isHtml)
            {
                return str?.Replace("<br>", "\n");
            }

            return str;
        }
    }
}

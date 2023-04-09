using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        static bool isBold;
        static bool inReplacement;
        static bool isRed;
        static bool isMac;
        static bool isWindows;
        static bool isPro;
        static bool isError;

        public static FlowDocument ConvertToFlowDocument(string? text, Func<string, string?> replaceId)
        {
            if (text == null)
            {
                return new FlowDocument();
            }

            try
            {
                myFlowDoc = new FlowDocument();
                myFlowDoc.TextAlignment = System.Windows.TextAlignment.Left;
                paragraph = new Paragraph();
                ConvertToFlowDocument2(text, replaceId);
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

            if (!isBold)
            {
                run = new Run(sb.ToString());
            }
            else
            {
                run = new Bold(new Run(sb.ToString()));
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

        static void ConvertToFlowDocument2(string text, Func<string, string?> replaceId)
        {
            int index = 0;
            bool firstToUpper = false;
            sb = new StringBuilder();
            StringBuilder command = null!;

            while (index < text.Length)
            {
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

                        if (cmd == "br")
                        {
                            myFlowDoc.Blocks.Add(paragraph);
                            paragraph = new Paragraph();
                        }
                        else if (cmd == "b")
                        {
                            isBold = true;
                        }
                        else if (cmd == "/b")
                        {
                            isBold = false;
                        }
                        else if (cmd == "red")
                        {
                            isRed = true;
                        }
                        else if (cmd == "/red")
                        {
                            isRed = false;
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
                        else if (cmd.StartsWith("a ") || cmd == "/a")
                        {
                            sb.Append('<');
                            sb.Append(cmd);
                            sb.Append('>');
                            AddText();
                            isError = false;
                        }
                        else
                        {
                            isError = true;
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

                            ConvertToFlowDocument2(ids, replaceId);
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
    }
}

using HtmlAgilityPack;
using Markdig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarkdownEditor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string markdown = textBoxMarkdown.Text;
            string html = Markdown.ToHtml(markdown);
            //webBrowser1.DocumentText = html;

            richTextBox1.Rtf = ConvertHtmlToRtf(html);
        }


        private string ConvertHtmlToRtf(string html)
        {
            // Load HTML using Html Agility Pack
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);

            StringBuilder rtfBuilder = new StringBuilder();
            rtfBuilder.Append(@"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang1033");

            foreach (var node in htmlDoc.DocumentNode.ChildNodes)
            {
                ProcessNode(node, rtfBuilder);
            }

            rtfBuilder.Append("}");
            return rtfBuilder.ToString();
        }

        private void ProcessNode(HtmlNode node, StringBuilder rtfBuilder)
        {
            switch (node.Name.ToLower())
            {
                case "h1":
                    rtfBuilder.Append(@"{\b\fs32 " + node.InnerText + @"\par}");
                    break;
                case "h2":
                    rtfBuilder.Append(@"{\b\fs24 " + node.InnerText + @"\par}");
                    break;
                case "h3":
                    rtfBuilder.Append(@"{\b\fs20 " + node.InnerText + @"\par}");
                    break;
                case "p":
                    rtfBuilder.Append(node.InnerText + @"\par");
                    break;
                case "ul":
                    foreach (var li in node.ChildNodes)
                    {
                        if (li.Name == "li")
                        {
                            rtfBuilder.Append(@"\bullet " + li.InnerText + @"\par");
                        }
                    }
                    break;
                case "strong":
                    rtfBuilder.Append(@"{\b " + node.InnerText + @"}");
                    break;
                case "em":
                    rtfBuilder.Append(@"{\i " + node.InnerText + @"}");
                    break;
                case "a":
                    // For links, convert to clickable format in RTF
                    rtfBuilder.Append(@"{\ul " + node.InnerText + @"\footnote {\*\footnote {\fldrslt {\*\fldinst HYPERLINK """ + node.GetAttributeValue("href", "") + @"""}}}}");
                    break;
                case "img":
                    // RTF does not support embedding images directly, but you can reference the image
                    string imgSrc = node.GetAttributeValue("src", "");
                    rtfBuilder.Append(@"{\pict\wmetafile8\picw" + imgSrc + @"\pich" + imgSrc + @"\picwgoal" + imgSrc + @"\pichgoal" + imgSrc + @"}"); // Placeholder for images
                    break;
                case "blockquote":
                    rtfBuilder.Append(@"{\pard\itap1 " + node.InnerText + @"\par}"); // Indent block quotes
                    break;
                // Add more cases as needed
                default:
                    foreach (var child in node.ChildNodes)
                    {
                        ProcessNode(child, rtfBuilder); // Recursively process child nodes
                    }
                    break;
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}

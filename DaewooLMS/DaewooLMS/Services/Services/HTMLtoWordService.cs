using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Xml;

//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;
//using HtmlAgilityPack;
//using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using LMSProject.Services.Interface;
//using HtmlToOpenXml;


namespace LMSProject.Services.Services
{
    public class HTMLtoWordService : IHTMLtoWord
    {
        public byte[] ConvertHtmlToWord(string htmlContent)
        {
         using (MemoryStream mem = new MemoryStream())
        {
            // Create a Wordprocessing document.
            //using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(mem, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
            //{
            //    // Add a main document part.
            //    MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();

            //    // Create the document structure and add some text.
            //    mainPart.Document = new Document();
            //    Body body = new Body();
            //    mainPart.Document.Append(body);

            //    // Convert the HTML into OpenXML and insert it.
            //    HtmlConverter converter = new HtmlConverter(mainPart);
            //    var paragraphs = converter.Parse(htmlContent);
            //    foreach (var paragraph in paragraphs)
            //    {
            //        body.Append(paragraph);
            //    }

            //    // Save changes to the main document part.
            //    mainPart.Document.Save();
            //}

            return mem.ToArray();
        }
        }

        //private Paragraph ConvertHtmlNodeToWordParagraph(HtmlNode node)
        //{
        //    Paragraph paragraph = new Paragraph();
        //    Run run = new Run();

        //    run.Append(new Text(node.InnerText));
        //    paragraph.Append(run);

        //    return paragraph;
        //}
    }
}

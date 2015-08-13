using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(@"c:\test\1.htm");
            //foreach(HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            //foreach (HtmlNode node in doc.DocumentNode.SelectNodes("descendant::div[attribute::id='caption']"))
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("descendant::div[attribute::class='primary_photo']"))
            {
                Console.WriteLine(node.Name);
                HtmlNode imgNode = node.SelectSingleNode("descendant::img");
                if (imgNode != null)
                {
                    HtmlAttribute att = imgNode.Attributes["src"];
                    if (att != null)
                        Console.WriteLine(att.Value);
                }
                //att.Value = FixLink(att);
            }
            Console.ReadKey();
            //doc.Save("file.htm");
        }
    }
}

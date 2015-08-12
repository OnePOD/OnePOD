using HtmlAgilityPack;
using OnePOD.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace OnePOD
{
    public class PodModel
    {
        internal Picture CurrentPicture { get; set; }
        private Picture previousPicture { get; set; }

        // remote
        internal string DefaultPortalUrl = "";

        // local
        private string podTempFolder = "";
        private string podPagePath = "";
        //private string podPicOnePath = "";
        //internal string PodPicPath = "";

        public PodModel()
        {
            PodUtil.GenerateBuild();
            DefaultPortalUrl = @"http://photography.nationalgeographic.com/photography/photo-of-the-day/";

            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string podFolder = docFolder + @"\OnePOD";
            PodUtil.CreateDirIfNotAvailable(podFolder);
            podTempFolder = podFolder + @"\temp";
            PodUtil.CreateDirIfNotAvailable(podTempFolder);
            podPagePath = podTempFolder + @"\current.pod";
            CurrentPicture = new Picture();
            AcquireInfoFromLocalPage();

            //podPicOnePath = podTempFolder + @"\one.png";
            //PodUtil.CreateEmptyFileIfNotAvailable(podPicOnePath);
            //PodUtil.DownloadPicIfNotAvailable(PodPicOnePath);
            
            //PodPicPath = Properties.Settings.Default.LastPodPicPath;

            // clean temp folder
            DirectoryInfo tempDir = new DirectoryInfo(podTempFolder);
            List<FileInfo> filesToBeDeleted = new List<FileInfo>();
            foreach (FileInfo f in tempDir.EnumerateFiles())
            {
                //if (f.FullName == PodPicPath)
                //    continue;
                string s = Path.GetFileNameWithoutExtension(f.FullName);
                try
                {
                    Guid guid = new Guid(s);
                    // if conversion is successful, then delete the temp file
                    filesToBeDeleted.Add(f);
                }
                catch (Exception e)
                { 
                    // if conversion is NOT successful, then do nothing 
                }
            }
            foreach (FileInfo f in filesToBeDeleted)
                f.Delete();
        }

        internal string GetPodPicPath()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CurrentPicture.SourceDomain+"-");
            sb.Append(CurrentPicture.DateShort+"-");
            sb.Append(CurrentPicture.FileName);
            string s = podTempFolder + "\\" + sb.ToString();
            CurrentPicture.FilePath = s;
            return s;
        }

        //internal string GenerateNewPodPicPath()
        //{
        //    //string s = Guid.NewGuid().ToString();
        //    //PodPicPath = podTempFolder + "\\" + s + ".jpg";
        //    //Properties.Settings.Default.LastPodPicPath = PodPicPath;
        //    //Properties.Settings.Default.Save();
        //    //return PodPicPath;
        //}

        internal void GetPortalPage()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(DefaultPortalUrl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = null;
            FileStream fileStream = null;
            try
            {
                responseStream = response.GetResponseStream();
                long fileSize = response.ContentLength;
                fileStream = new FileStream(podPagePath, FileMode.OpenOrCreate, FileAccess.Write);
                int length = 1024;
                byte[] buffer = new byte[1025];
                int bytesRead = 0;
                while ((bytesRead = responseStream.Read(buffer, 0, length)) > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                }
            }
            catch (Exception e)
            {
                PodUtil.Error(e.Message);
            }
            finally
            {
                if (responseStream != null)
                    responseStream.Close();
                if (fileStream != null)
                    fileStream.Close();
            }
            previousPicture = CurrentPicture;
            CurrentPicture = new Picture();
            AcquireInfoFromLocalPage();
        }

        private bool _infoAcquired = false;
        internal void AcquireInfoFromLocalPage()
        {
            if (!File.Exists(podPagePath))
            {   // just show empty pic
                _infoAcquired = true;
                return;
            }
            HtmlDocument doc = new HtmlDocument();
            doc.Load(podPagePath);
            HtmlNode node = doc.DocumentNode.SelectSingleNode("descendant::div[attribute::id='caption']");
            if (node != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (HtmlNode n in node.ChildNodes)
                {
                    Console.WriteLine(n.InnerText); // debug only
                    if(PodUtil.IsAllWhiteSpace(n.InnerText))
                        continue;
                    if (n.OriginalName == "br") // n.OriginalName == "#text" || 
                        continue;
                    if (n.InnerText.Contains("&")) // ???
                        continue;
                    if(n.OriginalName == "h2") // title
                    {
                        CurrentPicture.Title = n.InnerText;
                        continue;
                    }
                    bool hit = false;
                    foreach (HtmlAttribute a in n.Attributes)
                    {
                        if (a.Value == "publication_time")
                        {
                            CurrentPicture.Date = n.InnerText;
                            hit = true;
                            break;
                        }
                        if (a.Value == "credit")
                        {
                            CurrentPicture.Credit = PodUtil.ProcessGarbledChar(n.InnerText);
                            hit = true;
                            break;
                        }
                    }
                    if (hit)
                        continue;
                    sb.Append("\n" + n.InnerText + "\n");
                }
                CurrentPicture.Detail = PodUtil.RemoveConsecutiveWhiteSpaces(PodUtil.ProcessGarbledChar(sb.ToString()));

                //string captionString = node.InnerText;
                //string tempString = PodUtil.RemoveConsecutiveWhiteSpaces(captionString);

                //HtmlNode dateNode = node.SelectSingleNode("descendant::p[attribute::class='publication_time']");
                //if (dateNode != null)
                //{
                //    _date = dateNode.InnerText;
                //    tempString = PodUtil.RemoveSubstring(tempString, _date);
                //}
                //HtmlNode titleNode = node.SelectSingleNode("descendant::h2");
                //if(titleNode != null)
                //{
                //    _title = titleNode.InnerText;
                //    tempString = PodUtil.RemoveSubstring(tempString, _title);
                //}
                //tempString = PodUtil.RemoveConsecutiveWhiteSpaces(tempString);
                //_detail = PodUtil.ProcessGarbledChar(tempString);

                // the following is not working because the <p> not being properly closed
                //HtmlNode detailNode = node.SelectSingleNode("descendant::p[3]");
                //if(detailNode != null)
                //{
                //    _detail = detailNode.InnerText;
                //}
            }
            GetPicUrl(false);
            _infoAcquired = true;
        }

        internal string GetPicUrl(bool forceError)
        {
            if (string.IsNullOrEmpty(CurrentPicture.Uri))
            {
                // analyse the portal page file
                string tempUrl = "";
                HtmlDocument doc = new HtmlDocument();
                doc.Load(podPagePath);
                HtmlNode node = doc.DocumentNode.SelectSingleNode("descendant::div[attribute::class='primary_photo']");
                if (node != null)
                {
                    HtmlNode imgNode = node.SelectSingleNode("descendant::img");
                    if (imgNode != null)
                    {
                        HtmlAttribute att = imgNode.Attributes["src"];
                        if (att != null)
                        {
                            tempUrl = att.Value;
                        }
                    }
                }
                if (string.IsNullOrEmpty(tempUrl))
                {
                    if(forceError)
                         PodUtil.Error("Not able to get picture url.");
                    CurrentPicture.Uri = "";
                }
                else
                    CurrentPicture.Uri = "http:" + tempUrl;
            }
            return CurrentPicture.Uri;
        }
    }
}

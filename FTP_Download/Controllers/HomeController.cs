using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FTP_Download.Controllers
{
    public class HomeController : Controller
    {

        string host = "";
        string username = "";
        string password = "";
        public ActionResult Index()
        {
           
            return View();
        }

        [HttpGet]
        public ActionResult Index2(string folder) {

            
            try
            {
                List<string> filenames = listdir(folder); //Returning all the files in the directory
                using (var zip = new ZipFile())
                {
                    var outputStream = new MemoryStream();
                    foreach (var item in filenames)
                    {
                        if (item != (folder + "/.") && item != (folder + "/.."))
                        {

                            var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + item);
                            ftpRequest.Credentials = new NetworkCredential(username, password);
                            ftpRequest.UseBinary = true;
                            ftpRequest.UsePassive = true;
                            ftpRequest.KeepAlive = true;
                            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                            var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                            var ftpStream = ftpResponse.GetResponseStream();
                            var fname = item.Split('/').ToList();
                            zip.AddEntry(fname[1], ftpStream);


                        }
                    }
                    zip.Save(outputStream);




                    outputStream.Position = 0;
                    return File(outputStream, "application/zip", folder + ".zip");


                }

                
            }
            catch (Exception ex)
            {
                throw;
            }

            
        }

        private List<string> listdir(string folder) { 

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(host + folder);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                request.Credentials = new NetworkCredential(username, password);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string names = reader.ReadToEnd();

                reader.Close();
                response.Close();

                List<string> lst =  names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
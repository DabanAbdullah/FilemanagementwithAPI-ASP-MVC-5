using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using System.Xml;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using project.Models;


namespace project.Controllers
{
    public class HomeController : Controller
    {
      public static  string Baseurl = "http://localhost:52813/";
        projectdbEntities dbb = new projectdbEntities();
      


        public void deleteinactiv()
        {
            var baseAddress = new Uri(Baseurl+ "api/FileAPI/DeleteInactive");


            httpresult p = new httpresult();


            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress);
            myHttpWebRequest.MaximumAutomaticRedirections = 5;
            myHttpWebRequest.AllowAutoRedirect = false;
            myHttpWebRequest.Method = "DELETE";
           
      


            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            
        }

        public async Task<ActionResult> Index()
        {


            Baseurl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~");
            deleteinactiv();

            return View();
        }




      




        public ActionResult Error()
        {
            ViewBag.Message = "Something went wrong";
            try { ViewBag.error = Session["error"].ToString(); }
            catch { }
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
           
            return View();
        }


        public static string GetAccessToken(string Email, string Password)
        {
            string AccessToken = "";
            string responseFromServer = "";
            WebRequest request = WebRequest.Create(Baseurl+"token"); //your project url
            request.Method = "POST";
            string postData = "username=" + Email + "&password=" + Password + "&grant_type=password";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            System.IO.Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            using (dataStream = response.GetResponseStream())
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
            }
            TokenInfo myDeserializedClass = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenInfo>(responseFromServer);
            AccessToken = myDeserializedClass.access_token;

            response.Close();
            return AccessToken;
        }



        [Authorize]
        public async Task<ActionResult> Upload()
        {
            Baseurl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~");
            try
            {
                deleteinactiv();
            }
            catch { }


            IEnumerable<filestbl> files = new List<filestbl>() ;
            try
            {
                var id = User.Identity.GetUserId();
             

                using (var client = new HttpClient())
                {

                    //string Email = "daban.csi@gmail.com";
                    //string Password = "gnt573@CS";


                


                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();


                    //    HttpResponseMessage Res = await client.GetAsync("api/FileAPI/GetUserFile?id=" + id);

                    //if (Res.IsSuccessStatusCode)
                    //    {
                    //        var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                    //    files = JsonConvert.DeserializeObject<List<filestbl>>(EmpResponse);
                    //    }


                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Baseurl+"api/FileAPI/GetUserFile"); //Your project Local host api url
                    string AccessToken = Request.Cookies["token"].Value.ToString();
                    request.AutomaticDecompression = DecompressionMethods.GZip;
                    request.Method = "GET";
                   request.Headers.Add("Authorization", "Bearer " + AccessToken);
                    using (System.Net.WebResponse GetResponse = request.GetResponse())
                    {
                        using (System.IO.StreamReader streamReader = new System.IO.StreamReader(GetResponse.GetResponseStream()))
                        {
                            dynamic jsonResponseText = streamReader.ReadToEnd();
                            files = JsonConvert.DeserializeObject<List<filestbl>>(jsonResponseText);
                        }
                    }



                    //  ViewBag.result = request.StatusCode;
                    return View(files);
                }
            }
            catch
            {
                return View(files);
            }
           
        }

        [HttpPost]
        public ActionResult SetViewBag(string value)
        {
            ViewBag.result = value;
            return new EmptyResult();
        }







        public async Task<ActionResult> detail(string id)
        {
            Baseurl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~");
            deleteinactiv();
            filestbl file = null;

            try
            {
              

                using (var client = new HttpClient())
                {

                    //  string AccessToken = Request.Cookies["token"].Value.ToString();


                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Baseurl+"api/FileAPI/getfilebyid?id=" + id); //Your project Local host api url
                    request.AutomaticDecompression = DecompressionMethods.GZip;
                    request.Method = "GET";
                    //request.Headers.Add("Authorization", "Bearer " + AccessToken);
                    using (System.Net.WebResponse GetResponse = request.GetResponse())
                    {
                        using (System.IO.StreamReader streamReader = new System.IO.StreamReader(GetResponse.GetResponseStream()))
                        {
                            dynamic jsonResponseText = streamReader.ReadToEnd();
                            file = JsonConvert.DeserializeObject<filestbl>(jsonResponseText);


                            var rec = dbb.filestbls.Where(x => x.fileid == file.fileid).FirstOrDefault();


                            var baseAddress = new Uri("https://www.tu-chemnitz.de/informatik/DVS/blocklist/" + rec.filehash);


                            httpresult p = new httpresult();


                            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress);
                            myHttpWebRequest.MaximumAutomaticRedirections = 5;
                            myHttpWebRequest.AllowAutoRedirect = false;
                            myHttpWebRequest.Method = "GET";

                            //   myHttpWebRequest.UseDefaultCredentials = true;

                            CookieContainer cookieContainer2 = new CookieContainer();
                            Cookie authco = new Cookie(ConfigurationManager.AppSettings["cookiename"], ConfigurationManager.AppSettings["cookieval"]);
                            Cookie nameco = new Cookie("WTC_AUTHENTICATED", ConfigurationManager.AppSettings["name"]);
                            cookieContainer2.Add(new Uri("https://www.tu-chemnitz.de"), authco);
                            cookieContainer2.Add(new Uri("https://www.tu-chemnitz.de"), nameco);
                            myHttpWebRequest.CookieContainer = cookieContainer2;
                            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                            using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream()))
                            {
                                p.content = myHttpWebResponse.StatusCode.ToString();
                             
                            }
                            if (p.content == "OK")
                            {
                                rec.blocked = 0;
                            }
                            else
                            {
                                rec.blocked = 1;
                            }
                            file.blocked = rec.blocked ;

                            }
                        dbb.SaveChanges();
                    }


                }




                

                if (Request.Cookies[System.Web.HttpContext.Current.Request.UserHostAddress] != null)
                {
                    DateTime end = DateTime.Parse(Request.Cookies[System.Web.HttpContext.Current.Request.UserHostAddress].Value.ToString());
                    DateTime now = DateTime.Now;
                    TimeSpan ts = end - now;
                    ViewBag.nextdownload = end.ToString("yyyy-MM-ddTHH:mm:ss");
                    ViewBag.result = "time left for next download for ip " + System.Web.HttpContext.Current.Request.UserHostAddress + "  is " + ts.TotalMinutes + " minutes";

                }


                

            }
            catch (Exception ex){ Session["error"] = ex.Message; return RedirectToAction("Error", "Home"); }
            return View(file);
        }


        

        [HttpPost]
        public async Task<ActionResult> detail(filestbl model)
        {


            filestbl file = model;

            try
            {


                var baseAddress = new Uri("https://www.tu-chemnitz.de/informatik/DVS/blocklist/" + model.filehash);


                httpresult p = new httpresult();


                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress);
                myHttpWebRequest.MaximumAutomaticRedirections = 5;
                myHttpWebRequest.AllowAutoRedirect = false;
                myHttpWebRequest.Method = "GET";

                //   myHttpWebRequest.UseDefaultCredentials = true;

                CookieContainer cookieContainer2 = new CookieContainer();
                Cookie authco = new Cookie(ConfigurationManager.AppSettings["cookiename"], ConfigurationManager.AppSettings["cookieval"]);
                Cookie nameco = new Cookie("WTC_AUTHENTICATED", ConfigurationManager.AppSettings["name"]);
                cookieContainer2.Add(new Uri("https://www.tu-chemnitz.de"), authco);
                cookieContainer2.Add(new Uri("https://www.tu-chemnitz.de"), nameco);
                myHttpWebRequest.CookieContainer = cookieContainer2;
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream()))
                {

                    p.content = myHttpWebResponse.StatusCode.ToString();
                    //    p.cookie = myHttpWebResponse.Headers[HttpResponseHeader.Location];

                }


                if (p.content == "OK")
                {








                    if (Request.Form["download"] != null)
                    {

                        if (User.Identity.IsAuthenticated)
                        {
                            var cookieContainer = new CookieContainer();
                            using (var handler = new HttpClientHandler()
                            {
                                CookieContainer = cookieContainer,


                            })
                            using (var client = new HttpClient(handler) { BaseAddress = new Uri(Baseurl) })
                            {

                                string AccessToken = Request.Cookies["token"].Value.ToString();
                                var userid = User.Identity.GetUserId();
                                //  cookieContainer.Add(new Uri(Baseurl), new Cookie("userid", userid));
                                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                                HttpResponseMessage Res = await client.GetAsync("api/FileAPI/download?id=" + model.fileid);
                                Res.EnsureSuccessStatusCode();
                                if (Res.IsSuccessStatusCode)
                                {
                                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                                    file = JsonConvert.DeserializeObject<filestbl>(EmpResponse);
                                }


                            }


                            if (file != null)
                            {
                                lastdownload(file.fileid);
                                return File(file.filedata, "attachment", file.filename);
                            }
                            else
                            {
                                ViewBag.result = "This File is Blocked ";
                                return View(model);

                            }

                        }
                        else
                        {
                            file = model;
                            if (dbb.filestbls.Where(x => x.sharedwith == "public" && x.fileid == model.fileid && x.blocked == 0).FirstOrDefault() != null)
                            {

                                if (Request.Cookies[GetIp()] != null)
                                //if (dbb.limittbls.Where(x => x.ip == System.Web.HttpContext.Current.Request.UserHostAddress && x.nextdownload > DateTime.Now).FirstOrDefault() != null)
                                {
                                    //   var recc = dbb.limittbls.Where(x => x.ip == System.Web.HttpContext.Current.Request.UserHostAddress && x.nextdownload > DateTime.Now).FirstOrDefault();
                                    // DateTime end = recc.nextdownload;
                                    DateTime end = DateTime.Parse(Request.Cookies[GetIp()].Value.ToString());
                                    DateTime now = DateTime.Now;
                                    TimeSpan ts = end - now;
                                    ViewBag.nextdownload = end.ToString("yyyy-MM-ddTHH:mm:ss");
                                    ViewBag.result = "Time Left for next download for ip " + GetIp() + "  is " + ts.TotalMinutes + " minutes";
                                    return View(file);
                                }
                                else
                                {




                                    HttpCookie cookie = new HttpCookie("");

                                    cookie[GetIp()] = DateTime.Now.AddMinutes(10).ToString();


                                    cookie.Expires = DateTime.Now.AddMinutes(10);
                                    Response.Cookies.Add(cookie);
                                    int downloadSpeed = 1024 * int.Parse("10");
                                    DownloadFileWithLimitedSpeed(file.filedata, file.filename, downloadSpeed, file.fileid);
                                    //   return File( file.filedata, System.Net.Mime.MediaTypeNames.Application.Octet, file.filename);

                                    lastdownload(file.fileid);
                                    return RedirectToAction("detail", new { model = file });
                                }
                            }
                            else
                            {
                                ViewBag.result = "This file is not shared with you";
                                return View(model);
                            }
                        }

                    }

                }
                else
                {
                    ViewBag.result = "This File is Bloked";
                }
            }
            catch { }




            return View(file);




        }



  
            public string GetIp()
            {
                string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip))
                {
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                return ip;
            }
      


        public void DownloadFileWithLimitedSpeed(byte[]file, string filename,long downloadSpeed,Guid fileid)
        {

            if (!System.IO.File.Exists(Server.MapPath("~/Content/download/") + filename))
                System.IO.File.WriteAllBytes(Server.MapPath("~/Content/download/") + filename, file);


            if (!System.IO.File.Exists(Server.MapPath("~/Content/download/") + filename))
            {
                throw new Exception("Err: There is no such a file to download.");
            }
            using (FileStream fs = new FileStream(Server.MapPath("~/Content/download/") + filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    Response.Buffer = false;
                    long fileLength = fs.Length;
                    int pack = 1024;
                    int sleep = (int)Math.Ceiling(1000.0 * pack / downloadSpeed);
                    Response.AddHeader("Content-Length", fileLength.ToString());
                    Response.ContentType = "application/octet-stream";

                    string utf8EncodingFileName = HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8);
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + utf8EncodingFileName);
                    int maxCount = (int)Math.Ceiling(Convert.ToDouble(fileLength) / pack);
                    for (int i = 0; i < maxCount; i++)
                    {
                        if (Response.IsClientConnected)
                        {
                            Response.BinaryWrite(br.ReadBytes(pack));
                            Thread.Sleep(sleep);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }


            System.IO.File.Delete(Server.MapPath("~/Content/download/") + filename);
        }
           



        public void lastdownload(Guid fileid)
        {
            var rec = dbb.filestbls.Where(x => x.fileid == fileid).FirstOrDefault();
            if (rec != null)
            {
                rec.lastdownloaddate = DateTime.Now;
            }
            dbb.SaveChanges();
        }



        public ActionResult uploadnoaccount()
        {



            return View();
        }









        [HttpGet]
        public string nextdownload()
        {
            if (Request.Cookies[GetIp()] != null)
           // if (dbb.limittbls.Where(x => x.ip == System.Web.HttpContext.Current.Request.UserHostAddress && x.nextdownload > DateTime.Now).FirstOrDefault() != null)
            {
                //   var rec = dbb.limittbls.Where(x => x.ip == System.Web.HttpContext.Current.Request.UserHostAddress && x.nextdownload > DateTime.Now).FirstOrDefault();
                // DateTime end = rec.nextdownload;
              
            
                DateTime end = DateTime.Parse(Request.Cookies[GetIp()].Value.ToString());
                DateTime now = DateTime.Now;
                TimeSpan ts = end - now;
                return ("time left for next download for ip " + GetIp()+ "  is " + ts.TotalMinutes + " minutes");

            }
            else
                return ("you can download now");


          
        }




        public async Task<ActionResult> API()
        {

           



            return View();
        }



    }
}
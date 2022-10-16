using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using project.Models;
namespace project.Controllers
{
    public class FileAPIController : ApiController
    {


        projectdbEntities dbb = new projectdbEntities();

        [Route("api/FileAPI/file")]
        [HttpPost]
        [Authorize]
       // allow usres to upload multiple files when you are Authenticated which means has account  
        public HttpResponseMessage UploadFiles()
        {
            string result = "";

            var httpContext = HttpContext.Current;
            var identity = (ClaimsIdentity)User.Identity;
            var id = dbb.AspNetUsers.Where(x => x.Email == identity.Name).FirstOrDefault().Id;
            // Check for any uploaded file  
            if (httpContext.Request.Files.Count > 0)
            {
                //Loop through uploaded files  
                for (int i = 0; i < httpContext.Request.Files.Count; i++)
                {
                    HttpPostedFile httpPostedFile = httpContext.Request.Files[i];
                    if (httpPostedFile != null)
                    {
                        //Fetch the File.
                        //HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];

                        //Fetch the File Name.
                        string fileName = Path.GetFileName(httpPostedFile.FileName);
                        // HttpContext.Current.Request.Form["fileName"]  +
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(httpPostedFile.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(httpPostedFile.ContentLength);
                        }
                        string filesize = Utilityclass.GetSizeInMemory(fileData.Length);
                        string sha256 = Utilityclass.ComputeStringToSha256Hash(fileData);


                        int MaxContentLength = 1024 * 1024 * 10; //Size = 10 MB
                        if (fileData.Length > MaxContentLength)
                        {
                            // return Request.CreateResponse(HttpStatusCode.OK, "your file was not uploadded because  size was more  than 10 mgb it was=" + filesize);
                            result = result + Environment.NewLine + "your file was not uploadded because  size was more  than 10 mgb it was=" + filesize;

                        }
                        else
                        {
                            string userId = User.Identity.GetUserId();
                            //  postedFile.SaveAs(path + fileName);
                            System.Guid guid = System.Guid.NewGuid();
                            filestbl rec = new filestbl();
                            rec.fileid = guid;
                            rec.filedata = fileData;
                            rec.filehash = sha256;
                            rec.filename = fileName;
                            rec.filesize = filesize;
                            rec.uid = id;
                            rec.blocked = 0;
                            rec.uploaddate = DateTime.Now;
                            rec.sharedwith = "public";
                            rec.lastdownloaddate = DateTime.Now;
                            dbb.filestbls.Add(rec);
                            dbb.SaveChanges();
                            //   File.WriteAllBytes(path + fileName, fileData);

                            result = result + Environment.NewLine + fileName + " file size is " + filesize + "  <a href='detail/" + guid + "' target='_blank'>download </a>";

                            //Send OK Response to Client.
                            //  return Request.CreateResponse(HttpStatusCode.OK, fileName + " file size is " + filesize + " and it has been uploadded sha256=" + sha256 + "<br/> <a href=" + HttpContext.Current.Request + "/" + guid + ">download <a/>");
                        }

                    }
                }

            }


          
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        // allow non registered usres to upload single file when you are not Authenticated which means has no account

        [Route("api/FileAPI/uploadnoauth")]
        [HttpPost]
  
        public HttpResponseMessage UploadFilesnoauth()
        {
            try
            {

                //Create the Directory.
                //string path = HttpContext.Current.Server.MapPath("~/Content/uploads/");
                //if (!Directory.Exists(path))
                //{
                //    Directory.CreateDirectory(path);
                //}

                //Fetch the File.
                HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];

                //Fetch the File Name.
                string fileName = Path.GetFileName(postedFile.FileName);
                // HttpContext.Current.Request.Form["fileName"]  +
                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(postedFile.InputStream))
                {
                    fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                }
                string filesize = Utilityclass.GetSizeInMemory(fileData.Length);
                string sha256 = Utilityclass.ComputeStringToSha256Hash(fileData);


                int MaxContentLength = 1024 * 1024 * 10; //Size = 10 MB
                if (fileData.Length > MaxContentLength)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "your file was not uploadded because  size was more  than 10 mgb it was=" + filesize);

                }
                else
                {
                    string userId = User.Identity.GetUserId();
                    //  postedFile.SaveAs(path + fileName);
                    System.Guid guid = System.Guid.NewGuid();
                    filestbl rec = new filestbl();
                    rec.fileid = guid;
                    rec.filedata = fileData;
                    rec.filehash = sha256;
                    rec.filename = fileName;
                    rec.filesize = filesize;
                    rec.uid = "no user";
                    rec.blocked = 0;
                    rec.sharedwith = "public";
                    rec.uploaddate = DateTime.Now;
                    rec.lastdownloaddate = DateTime.Now;
                    dbb.filestbls.Add(rec);
                    dbb.SaveChanges();
                //   File.WriteAllBytes(path + fileName, fileData);

                

                    //Send OK Response to Client.
                return Request.CreateResponse(HttpStatusCode.OK, fileName + " file size is " + filesize + "  <a href='detail/" + guid+"' target='_blank'>download </a>");
                }
            }
           catch(Exception ex)
           {
               return Request.CreateResponse(HttpStatusCode.BadRequest,ex.Message);
           }

        }





          // to view uploaded files who is authenticated  
        [Route("api/FileAPI/GetUserFile")]
        [HttpGet]
       [Authorize]
        public IHttpActionResult GetUserFile()
        {
            IList<filestbl> files = null;
            var identity = (ClaimsIdentity)User.Identity;
            var id = dbb.AspNetUsers.Where(x => x.Email == identity.Name).FirstOrDefault().Id;
            using (var ctx = new projectdbEntities())
            {
                files = ctx.filestbls.Where(x => x.uid == id).ToList();
            }

            if (files.Count == 0)
            {
                return NotFound();
            }

            return Ok(files);
        }


        //  gives a file detail by ID  
        [Route("api/FileAPI/getfilebyid")]
        [HttpGet]
        public IHttpActionResult GetFileById(string id)
        {
           filestbl file = null;

            using (var ctx = new projectdbEntities())
            {
                file = ctx.filestbls.Where(x => x.fileid.ToString() == id).FirstOrDefault();
                
            }

            if (file==null)
            {
                return NotFound();
            }

            return Ok(file);
        }



        //[Authorize]
        //[Route("api/FileAPI/directdownload")]
        //public HttpResponseMessage downloadfiledirectly(string id,string username,string password)
        //{

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        byte[] filedata = null; string filename = "";
        //        //Create HTTP Response.
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
        //        var rec = dbb.filestbls.Where(x => x.fileid.ToString() == id).FirstOrDefault();
        //        if (rec != null)
        //        {
        //            filename = rec.filename;
        //            filedata = rec.filedata;

        //        }
        //        else
        //        {
        //            response.StatusCode = HttpStatusCode.NotFound;
        //            response.ReasonPhrase = string.Format("File not found");
        //            throw new HttpResponseException(response);
        //        }

        //        //    return rec;


        //        response.Content = new ByteArrayContent(filedata);

        //        //Set the Response Content Length.
        //        response.Content.Headers.ContentLength = filedata.LongLength;

        //        //Set the Content Disposition Header Value and FileName.
        //        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //        response.Content.Headers.ContentDisposition.FileName = filename;

        //        //Set the File Content Type.
        //        response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(filename));
        //        return response;
        //    }
        //    else {
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
        //        response.StatusCode = HttpStatusCode.NotFound;
        //        response.ReasonPhrase = string.Format("you are not authenticated");
              
        //        return response;
        //    }
        //}


      
       // download files it will check for the bklocked and if it is shared with specidic person
       
        [HttpGet]
        [Route("api/FileAPI/download")]
        [Authorize]
        public IHttpActionResult downloadfile(string id)
        {
            filestbl data = null;
            string sharedwith = "";

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            var rec = dbb.filestbls.Where(x => x.fileid.ToString() == id).FirstOrDefault();
            if (rec != null)
            {
                if (rec.blocked == 1) {
                    response.StatusCode = HttpStatusCode.OK;
                    response.ReasonPhrase = string.Format("file is blocked and can't be download at this time");
                    throw new HttpResponseException(response);
                }
                else
                {


                    sharedwith = rec.sharedwith;
                    data = rec;
                    if (sharedwith == "public") { return Ok(data); }
                    else
                    {

                        //CookieHeaderValue cookie = Request.Headers.GetCookies("userid").FirstOrDefault();
                        //var userid = cookie["userid"].Value;
                        var identity = (ClaimsIdentity)User.Identity;
                        var userid = dbb.AspNetUsers.Where(x => x.Email == identity.Name).FirstOrDefault().Id;
                        try
                        {
                            if (sharedwith == dbb.AspNetUsers.Where(x => x.Id.ToString() == userid).FirstOrDefault().Email || dbb.filestbls.Where(x => x.uid == userid && x.fileid.ToString() == id).FirstOrDefault() != null) { return Ok(data); }


                        }
                        catch
                        {

                        }
                    }
                }


            }
                       
               

            response.StatusCode = HttpStatusCode.OK;
            response.ReasonPhrase = string.Format("file not found or it is private");
            throw new HttpResponseException(response);

        }





        // delete uploaded files you have to own the file
        [Route("api/FileAPI/file")]
        [HttpDelete]
        [Authorize]
        public HttpResponseMessage Delete()
        {
            string result = "";
            try
            {

                var identity = (ClaimsIdentity)User.Identity;
                var userid = dbb.AspNetUsers.Where(x => x.Email == identity.Name).FirstOrDefault().Id;

                //var userid = HttpContext.Current.Request.Form["uid"].ToString();
                var Id = HttpContext.Current.Request.Form["Id"].ToString();
                var rec = dbb.filestbls.Where(x => x.uid == userid && x.fileid.ToString() == Id).FirstOrDefault();
                if (rec != null)
                {

                    dbb.filestbls.Remove(rec);
                    dbb.Requesttbls.RemoveRange(dbb.Requesttbls.Where(x => x.fileid ==rec.fileid.ToString()));
                    dbb.SaveChanges();

                    result = "this file was deleted ";
                }
                else { result = "this file was not deleted because you don't own it"; }
            }
            catch { return Request.CreateResponse(HttpStatusCode.OK, "something went worng please check your parameter"); }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        //delete files that are not been downloaded for more than 14 days
        [Route("api/FileAPI/DeleteInactive")]
        [HttpDelete]
      
        public HttpResponseMessage DeleteInactive()
        {
            string result = "";
            try
            {
                var oldDate = DateTime.Now.AddDays(-14);
                dbb.filestbls.RemoveRange(dbb.filestbls.Where(x => x.lastdownloaddate<oldDate));
                dbb.SaveChanges();
                
                 result = "any inactive file older than 14 days  was deleted";
               
               
            }
            catch { return Request.CreateResponse(HttpStatusCode.OK, "something went worng please check your parameter"); }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }



        // authenticated users can share file with one person
        [Route("api/FileAPI/Share")]
        [Authorize]
        [HttpPut]

        public HttpResponseMessage ShareFile()
        {
            string result = "";
            try
            {

                var identity = (ClaimsIdentity)User.Identity;
                var userid = dbb.AspNetUsers.Where(x => x.Email == identity.Name).FirstOrDefault().Id;
                var fileId = HttpContext.Current.Request.Form["fileid"].ToString();
                var email = HttpContext.Current.Request.Form["email"].ToString();
                var rec = dbb.filestbls.Where(x => x.uid == userid && x.fileid.ToString() == fileId).FirstOrDefault();
                if (rec != null)
                {

                    rec.sharedwith = email;
                    dbb.SaveChanges();

                    result = "this file was shared with "+email;
                }
                else { result = "this file was not shared because you don't own it"; }
            }
            catch { return Request.CreateResponse(HttpStatusCode.OK, "something went worng please check your parameter"); }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }




        //anyone can request a file to be blocked or unblocked providing a reason

        [Route("api/FileAPI/MakeRequest")]
        [HttpPost]
       
        public HttpResponseMessage MakeRequest()
        {
            string result = "";
            try
            {
                //if blocklist api returened result

                var fileId = HttpContext.Current.Request.Form["fileid"].ToString();
                var cause = HttpContext.Current.Request.Form["cause"].ToString();
                var type = HttpContext.Current.Request.Form["type"].ToString();
                var rec = dbb.filestbls.Where(x =>  x.fileid.ToString() == fileId).FirstOrDefault();
                if (rec != null)
                {

                    var rec2 = dbb.Requesttbls.Where(x => x.fileid.ToString() == fileId && x.response == null && x.requesttyepe==type).FirstOrDefault();
                    if (rec2 != null)
                    {
                        result = type + " request to this  file was already made";
                    }
                    else
                    {
                        Requesttbl req = new Requesttbl();
                        System.Guid guid = System.Guid.NewGuid();
                        req.id = guid;
                        req.fileid = fileId;
                        req.causeofrequest = cause;
                        req.requesttyepe = type;
                        dbb.Requesttbls.Add(req);
                        dbb.SaveChanges();

                        result = type + " request to this  file was sent to admin";
                    }
                }
                else { result = "this file was not found"; }
            }
            catch { return Request.CreateResponse(HttpStatusCode.OK, "something went worng please check your parameter"); }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        //only admin can respond to a request deciding to block or unblock file providing a reason.

        [Route("api/FileAPI/RespondtoRequest")]
        [HttpPut]
       [Authorize]
        public HttpResponseMessage RespondtoRequest()
        {
            string result = "";
           
            try
            {

                //if blocklist api returened result
                var identity = (ClaimsIdentity)User.Identity;
                var userid = dbb.AspNetUsers.Where(x => x.Email == identity.Name).FirstOrDefault().Id;


                var role = dbb.userroles.Where(x => x.Id ==userid).FirstOrDefault().Name;

                if (role == "Admin")
                {

                    var requestid = HttpContext.Current.Request.Form["id"].ToString();
                    var cause = HttpContext.Current.Request.Form["cause"].ToString();
                    var type = HttpContext.Current.Request.Form["type"].ToString();


                    httpresult p = new httpresult();


                    var rec2 = dbb.Requesttbls.Where(x => x.id.ToString() == requestid).FirstOrDefault();
                    if (rec2 != null)
                    {
                        if (rec2.response != null)
                        {
                            result = " Response to this  request was already made";
                        }
                        else
                        {
                            // if (type == "accept") rec2.response = "accept";
                            // else rec2.response = "reject";
                            // rec2.causeofresponse = cause;
                            // 
                            //result = "this  reqeuest was responded";

                            if (type != "accept")
                            {
                                rec2.causeofresponse = cause;
                                rec2.response = "reject";
                                result = "this  reqeuest was responded";

                            }

                            var f = dbb.filestbls.Where(x => x.fileid.ToString() == rec2.fileid).FirstOrDefault();
                            if (f != null)
                            {
                                var baseAddress = new Uri("https://www.tu-chemnitz.de/informatik/DVS/blocklist/" + f.filehash);
                                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress);
                                myHttpWebRequest.MaximumAutomaticRedirections = 5;
                                myHttpWebRequest.AllowAutoRedirect = false;
                                CookieContainer cookieContainer2 = new CookieContainer();
                                Cookie authco = new Cookie(ConfigurationManager.AppSettings["cookiename"], ConfigurationManager.AppSettings["cookieval"]);
                                Cookie nameco = new Cookie("WTC_AUTHENTICATED", ConfigurationManager.AppSettings["name"]);
                                cookieContainer2.Add(new Uri("https://www.tu-chemnitz.de"), authco);
                                cookieContainer2.Add(new Uri("https://www.tu-chemnitz.de"), nameco);
                                myHttpWebRequest.CookieContainer = cookieContainer2;


                                if (rec2.requesttyepe == "block" && type == "accept")
                                {



                                    myHttpWebRequest.Method = "PUT";

                                    //   myHttpWebRequest.UseDefaultCredentials = true;


                                    HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                                    using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream()))
                                    {

                                        p.content = myHttpWebResponse.StatusCode.ToString();
                                        //    p.cookie = myHttpWebResponse.Headers[HttpResponseHeader.Location];

                                    }


                                    if (p.content == "Created")
                                    {
                                        rec2.causeofresponse = cause;
                                        result = "this  reqeuest was responded";
                                        rec2.response = "accept";
                                        f.blocked = 1;
                                        f.cause = "this file was blocked because " + cause;
                                    }
                                }
                                else if (rec2.requesttyepe == "unblock" && type == "accept")
                                {

                                    myHttpWebRequest.Method = "DELETE";

                                    //   myHttpWebRequest.UseDefaultCredentials = true;


                                    HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                                    using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream()))
                                    {

                                        p.content = myHttpWebResponse.StatusCode.ToString();
                                        //    p.cookie = myHttpWebResponse.Headers[HttpResponseHeader.Location];

                                    }


                                    if (p.content == "NoContent")
                                    {
                                        rec2.causeofresponse = cause;
                                        result = "this  reqeuest was responded";
                                        rec2.response = "accept";
                                        f.blocked = 0;
                                        f.cause = "this file was Unblocked because " + cause;
                                    }
                                }

                            }

                            dbb.SaveChanges();
                        }

                    }
                    else
                    {
                        result = "there is no such request";
                    }
                }
                else
                {
                    result = "you are not admin";
                }
               
            }
            catch { return Request.CreateResponse(HttpStatusCode.OK, "something went worng please check your parameter"); }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }








        [Authorize]
        [HttpGet]
        [Route("api/FileAPI/GetForAuthenticate")]
        public IHttpActionResult GetForAuthenticate()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = dbb.AspNetUsers.Where(x => x.Email == identity.Name).FirstOrDefault().Id;
            return Ok("Hello " + identity.Name);
        }





    }
}
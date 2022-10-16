using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace project.Controllers
{
    public class WTCController : Controller
    {
        HttpWebRequest myHttpWebRequest;


        public async Task<JsonResult> wtc1Async()
        {
          

            var baseAddress = new Uri("https://www.tu-chemnitz.de/informatik/DVS/blocklist/e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
            ////  var baseAddress = new Uri(url);

            httpresult p = new httpresult();






            myHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress);
            myHttpWebRequest.MaximumAutomaticRedirections = 3;
            myHttpWebRequest.AllowAutoRedirect = true;
            myHttpWebRequest.Method = "GET";
            myHttpWebRequest.Credentials = CredentialCache.DefaultNetworkCredentials;
            //myHttpWebRequest.CookieContainer = new CookieContainer();
            //myHttpWebRequest.CookieContainer.Add(new Cookie("_saml_sp", cookie, "wtc.tu-chemnitz.de"));






            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream()))
            {
                p.content = reader.ReadToEnd(); // do something fun..

                p.cookie = myHttpWebResponse.Headers[HttpResponseHeader.SetCookie];

            }

            return Json(p, JsonRequestBehavior.AllowGet);


        }




        public async Task<JsonResult> Getusername1(string url, string cookie)
        {

            cookie = cookie.Replace("_saml_sp=", "");
            cookie = cookie.Replace(";", "");

            var baseAddress = new Uri(url);
            ////  var baseAddress = new Uri(url);

            httpresult p = new httpresult();







            myHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress);
            //  myHttpWebRequest.MaximumAutomaticRedirections = 2;
            myHttpWebRequest.AllowAutoRedirect = false;
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.Credentials = CredentialCache.DefaultNetworkCredentials;
            //myHttpWebRequest.CookieContainer = new CookieContainer();
            //myHttpWebRequest.CookieContainer.Add(new Cookie("_saml_sp", cookie, "wtc.tu-chemnitz.de"));
            CookieContainer cookieContainer = new CookieContainer();
            Cookie userNameCookie = new Cookie("_saml_sp", cookie);

            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), userNameCookie);
            //  cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), tucldap);
            myHttpWebRequest.CookieContainer = cookieContainer;
            string postData = "session=true&user_idp=https://wtc.tu-chemnitz.de/shibboleth&Select=";
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(postData);
            //  myHttpWebRequest.Headers.Add(HttpRequestHeader.Cookie, cookie + " _redirection_state=checked");
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";

            myHttpWebRequest.ContentLength = byte1.Length;
            Stream newStream = myHttpWebRequest.GetRequestStream();

            newStream.Write(byte1, 0, byte1.Length);


            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream()))
            {
                p.content = reader.ReadToEnd(); // do something fun...

                p.cookie = myHttpWebResponse.Headers[HttpResponseHeader.SetCookie];

                p.cookie2 = myHttpWebResponse.Headers[HttpResponseHeader.Location];

            }
            //foreach (Cookie cookiee in cookieContainer.GetCookies(new Uri("https://wtc.tu-chemnitz.de")))
            //{

            //    p.cookie2 = p.cookie2 + cookiee.Name + "=" + cookiee.Value + ";";
            //}
            return Json(p, JsonRequestBehavior.AllowGet);
        }


        Cookie finalcookie;
        public async Task<JsonResult> Shibbolethsso(string url)
        {



            var baseAddress = new Uri(url);
            ////  var baseAddress = new Uri(url);

            httpresult p = new httpresult();







            myHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress);
            //  myHttpWebRequest.MaximumAutomaticRedirections = 2;
            myHttpWebRequest.AllowAutoRedirect = false;
            myHttpWebRequest.Method = "GET";
            myHttpWebRequest.UseDefaultCredentials = true;
            //myHttpWebRequest.CookieContainer = new CookieContainer();
            //myHttpWebRequest.CookieContainer.Add(new Cookie("_saml_sp", cookie, "wtc.tu-chemnitz.de"));
            CookieContainer cookieContainer = new CookieContainer();


            myHttpWebRequest.CookieContainer = cookieContainer;



            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            foreach (Cookie cookie in cookieContainer.GetCookies(new Uri("https://wtc.tu-chemnitz.de")))
            {
                // Console.WriteLine("Name = {0} ; Value = {1} ; Domain = {2}",
                // cookie.Name, cookie.Value, cookie.Domain);
                p.cookie2 = p.cookie2 + cookie.Name + "=" + cookie.Value + ";";
            }
            using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream()))
            {
                p.content = reader.ReadToEnd(); // do something fun...

                p.cookie = myHttpWebResponse.Headers[HttpResponseHeader.SetCookie];

                finalcookie = new Cookie(p.cookie.Split('=')[0], "");

            }

            return Json(p, JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> SSOService(string url, string sp, string idp, string userdp)
        {


            httpresult p = new httpresult();
            var baseAddress = new Uri(url);

            userdp = userdp.Trim();
            sp = sp.Trim();
            myHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress);
            myHttpWebRequest.MaximumAutomaticRedirections = 5;

            myHttpWebRequest.AllowAutoRedirect = false;
            myHttpWebRequest.Method = "GET";
            myHttpWebRequest.UseDefaultCredentials = true;
            //myHttpWebRequest.CookieContainer = new CookieContainer();
            //myHttpWebRequest.CookieContainer.Add(new Cookie("_saml_sp", cookie, "wtc.tu-chemnitz.de"));
            CookieContainer cookieContainer = new CookieContainer();
            Cookie _saml_sp = new Cookie("_saml_sp", sp);
            Cookie _saml_idp = new Cookie("_saml_idp", idp);
            Cookie _redirect_user_idp = new Cookie("_redirect_user_idp", userdp);
            Cookie _redirection_stateco = new Cookie("_redirection_state", "checked");
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _saml_idp);
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _saml_sp);
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _redirection_stateco);
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _redirect_user_idp);


            myHttpWebRequest.CookieContainer = cookieContainer;





            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            foreach (Cookie cookie in cookieContainer.GetCookies(new Uri("https://wtc.tu-chemnitz.de")))
            {
                // Console.WriteLine("Name = {0} ; Value = {1} ; Domain = {2}",
                // cookie.Name, cookie.Value, cookie.Domain);
                p.cookie2 = p.cookie2 + cookie.Name + "=" + cookie.Value + ";";
            }
            using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream()))
            {
                p.content = reader.ReadToEnd(); // do something fun...

                p.cookie = myHttpWebResponse.Headers[HttpResponseHeader.SetCookie];
                p.cookie = myHttpWebResponse.Headers[HttpResponseHeader.SetCookie];


            }

            return Json(p, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> Usernanephp(string url, string sp, string idp, string userdp)
        {

            httpresult p = new httpresult();
            var baseAddress = new Uri(url);


            userdp = userdp.Trim();
            sp = sp.Trim();

            myHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress);
            myHttpWebRequest.MaximumAutomaticRedirections = 5;
            myHttpWebRequest.AllowAutoRedirect = false;
            myHttpWebRequest.Method = "GET";
            myHttpWebRequest.UseDefaultCredentials = true;
            //myHttpWebRequest.CookieContainer = new CookieContainer();
            //myHttpWebRequest.CookieContainer.Add(new Cookie("_saml_sp", cookie, "wtc.tu-chemnitz.de"));
            CookieContainer cookieContainer = new CookieContainer();
            Cookie _saml_sp = new Cookie("_saml_sp", sp);
            Cookie _saml_idp = new Cookie("_saml_idp", idp);
            Cookie _redirect_user_idp = new Cookie("_redirect_user_idp", userdp);
            Cookie _redirection_state = new Cookie("_redirection_state", "checked");
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _saml_idp);
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _saml_sp);
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _redirection_state);
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _redirect_user_idp);


            myHttpWebRequest.CookieContainer = cookieContainer;





            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream()))
            {
                p.content = reader.ReadToEnd(); // do something fun...

                p.cookie = myHttpWebResponse.Headers[HttpResponseHeader.SetCookie];

                var ShibSessionID = p.cookie.Substring(p.cookie.IndexOf('=') + 1, p.cookie.IndexOf(';') - 14);

                myHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress);
                myHttpWebRequest.MaximumAutomaticRedirections = 5;
                myHttpWebRequest.AllowAutoRedirect = true;
                myHttpWebRequest.Method = "GET";
                myHttpWebRequest.UseDefaultCredentials = true;
                //myHttpWebRequest.CookieContainer = new CookieContainer();
                //myHttpWebRequest.CookieContainer.Add(new Cookie("_saml_sp", cookie, "wtc.tu-chemnitz.de"));
                cookieContainer = new CookieContainer();
                _saml_sp = new Cookie("_saml_sp", sp);
                _saml_idp = new Cookie("_saml_idp", idp);
                _redirect_user_idp = new Cookie("_redirect_user_idp", userdp);
                _redirection_state = new Cookie("_redirection_state", "checked");
                Cookie ShibSessionIDco = new Cookie("ShibSessionID", ShibSessionID);
                cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), ShibSessionIDco);
                cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _saml_idp);
                cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _saml_sp);
                cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _redirection_state);
                cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _redirect_user_idp);


                myHttpWebRequest.CookieContainer = cookieContainer;

                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                using (var reader2 = new StreamReader(myHttpWebResponse.GetResponseStream()))
                {
                    p.content = reader2.ReadToEnd(); // do something fun...

                    p.cookie = myHttpWebResponse.Headers[HttpResponseHeader.SetCookie];
                }
                foreach (Cookie cookie in cookieContainer.GetCookies(new Uri("https://wtc.tu-chemnitz.de")))
                {
                    // Console.WriteLine("Name = {0} ; Value = {1} ; Domain = {2}",
                    // cookie.Name, cookie.Value, cookie.Domain);
                    p.cookie2 = p.cookie2 + cookie.Name + "=" + cookie.Value + ";";
                }




            }

            return Json(p, JsonRequestBehavior.AllowGet);
        }





        public async Task<JsonResult> Getusername(string url, string _saml_sp)
        {

            var baseAddress = new Uri(url);


            httpresult p = new httpresult();


            myHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress);
            myHttpWebRequest.MaximumAutomaticRedirections = 5;
            myHttpWebRequest.AllowAutoRedirect = true;
            myHttpWebRequest.Method = "POST";

            myHttpWebRequest.UseDefaultCredentials = false;

            CookieContainer cookieContainer = new CookieContainer();
            Cookie _saml_spco = new Cookie("_saml_sp", _saml_sp);
            //Cookie _redirect_user_idpco = new Cookie("_redirection_state", "checked");
            //Cookie _saml_idpco = new Cookie("_saml_idp", _saml_idp);

            //cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _redirect_user_idpco);

            //cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _saml_spco);
            //cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _redirect_user_idpco);

            myHttpWebRequest.CookieContainer = cookieContainer;
            string postData = "session=true&user_idp=https://wtc.tu-chemnitz.de/shibboleth&Select=";
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(postData);

            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";


            myHttpWebRequest.ContentLength = byte1.Length;
            Stream newStream = myHttpWebRequest.GetRequestStream();

            newStream.Write(byte1, 0, byte1.Length);


            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();







            using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream()))
            {

                p.content = reader.ReadToEnd();
                p.cookie = myHttpWebResponse.Headers[HttpResponseHeader.Location];

            }




            return Json(p, JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> Postusername(string link, string _saml_sp, string _saml_idp, string _redirect_user_idp, string AuthState, string ShibSessionID)
        {

            var baseAddress = new Uri("https://wtc.tu-chemnitz.de/krb/module.php/TUC/username.php?AuthState=" + AuthState);
            // var baseAddress = new Uri("https://wtc.tu-chemnitz.de/krb/module.php/TUC/username.php?");


            httpresult p = new httpresult();


            myHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress);
            myHttpWebRequest.MaximumAutomaticRedirections = 7;
            myHttpWebRequest.AllowAutoRedirect = true;
            myHttpWebRequest.Method = "POST";

            myHttpWebRequest.UseDefaultCredentials = true;

            CookieContainer cookieContainer = new CookieContainer();
            Cookie _saml_spco = new Cookie("_saml_sp", _saml_sp);
            Cookie _redirect_user_stateco = new Cookie("_redirection_state", "checked");
            Cookie _saml_idpco = new Cookie("_saml_idp", _saml_idp);
            Cookie _redirect_user_idpco = new Cookie("_redirect_user_idp", "https://wtc.tu-chemnitz.de/shibboleth");
            Cookie ShibSessionIDco = new Cookie("ShibSessionID", ShibSessionID);


            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), ShibSessionIDco);
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _saml_spco);
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _saml_idpco);
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _redirect_user_stateco);
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _redirect_user_idpco);
            myHttpWebRequest.CookieContainer = cookieContainer;
            string postData = "username="+ ConfigurationManager.AppSettings["name"] + "&AuthState=" + AuthState;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(postData);

            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";



            myHttpWebRequest.ContentLength = byte1.Length;
            Stream newStream = myHttpWebRequest.GetRequestStream();

            newStream.Write(byte1, 0, byte1.Length);


            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream()))
            {
                p.content = reader.ReadToEnd();
                p.cookie = myHttpWebResponse.Headers[HttpResponseHeader.SetCookie];
                p.cookie2 = myHttpWebResponse.ResponseUri.ToString();

            }




            return Json(p, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> Postpass(string link, string _saml_sp, string _saml_idp, string _redirect_user_idp, string AuthState, string ShibSessionID)
        {

            var baseAddress = new Uri("https://wtc.tu-chemnitz.de/krb/module.php/core/loginuserpass.php?AuthState=" + AuthState);


            httpresult p = new httpresult();


            myHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress);
            myHttpWebRequest.MaximumAutomaticRedirections = 7;
            myHttpWebRequest.AllowAutoRedirect = true;
            myHttpWebRequest.Method = "POST";

            myHttpWebRequest.UseDefaultCredentials = true;

            CookieContainer cookieContainer = new CookieContainer();
            Cookie _saml_spco = new Cookie("_saml_sp", _saml_sp);
            Cookie _redirect_user_stateco = new Cookie("_redirection_state", "checked");
            Cookie _saml_idpco = new Cookie("_saml_idp", _saml_idp);
            Cookie _redirect_user_idpco = new Cookie("_redirect_user_idp", "https://wtc.tu-chemnitz.de/shibboleth");
            Cookie ShibSessionIDco = new Cookie("ShibSessionID", ShibSessionID);
            Cookie ldapusername = new Cookie("tuc-ldap-username", "daban");

            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), ShibSessionIDco);
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), ldapusername);
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _saml_spco);
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _saml_idpco);
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _redirect_user_stateco);
            cookieContainer.Add(new Uri("https://wtc.tu-chemnitz.de"), _redirect_user_idpco);
            myHttpWebRequest.CookieContainer = cookieContainer;
            string postData = "password="+ ConfigurationManager.AppSettings["pass"] + "&AuthState=" + AuthState;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(postData);

            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";



            myHttpWebRequest.ContentLength = byte1.Length;
            Stream newStream = myHttpWebRequest.GetRequestStream();

            newStream.Write(byte1, 0, byte1.Length);


            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream()))
            {
                p.content = reader.ReadToEnd();
                p.cookie = myHttpWebResponse.Headers[HttpResponseHeader.SetCookie];
                foreach (Cookie cookiee in cookieContainer.GetCookies(new Uri("https://www.tu-chemnitz.de")))
                {

                    p.cookie2 = p.cookie2 + cookiee.Name + "=" + cookiee.Value + ";";
                }

            }




            return Json(p, JsonRequestBehavior.AllowGet);
        }





        public async Task<JsonResult> Saml2post(string SAMLResponse, string RelayState, string namee, string vall)
        {





            var baseAddress = new Uri("https://www.tu-chemnitz.de/Shibboleth.sso/SAML2/POST");

            httpresult p = new httpresult();
            CookieContainer cookieContainer = new CookieContainer();
            try
            {


                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress);

                myHttpWebRequest.Method = "POST";
                myHttpWebRequest.AllowAutoRedirect = false;
                myHttpWebRequest.Timeout = 1000 * 30;
                myHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.63 Safari/537.36";
                myHttpWebRequest.PreAuthenticate = true;
                myHttpWebRequest.Credentials = CredentialCache.DefaultCredentials;
                Cookie wtc = new Cookie("WTC_AUTHENTICATED", "daban");
                Cookie req = new Cookie(namee, vall);

                cookieContainer.Add(new Uri("https://www.tu-chemnitz.de"), req);
                cookieContainer.Add(new Uri("https://www.tu-chemnitz.de"), wtc);


                myHttpWebRequest.CookieContainer = cookieContainer;
                string postData = "SAMLResponse=" + SAMLResponse + "&RelayState=" + RelayState;
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] byte1 = encoding.GetBytes(postData);

                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";


                myHttpWebRequest.ContentLength = byte1.Length;
                Stream newStream = myHttpWebRequest.GetRequestStream();

                newStream.Write(byte1, 0, byte1.Length);
                //   p.content=  myHttpWebRequest.CookieContainer.GetCookieHeader(new Uri("https://www.tu-chemnitz.de"));

                try
                {
                    HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                    using (var reader = new StreamReader(myHttpWebResponse.GetResponseStream()))
                    {
                        p.content = reader.ReadToEnd();
                        p.cookie = myHttpWebResponse.Headers[HttpResponseHeader.SetCookie];

                        HttpCookie WTC_AUTHENTICATED = new HttpCookie("WTC_AUTHENTICATED");
                        WTC_AUTHENTICATED.Value = ConfigurationManager.AppSettings["name"];
                        WTC_AUTHENTICATED.Expires = DateTime.Now.AddHours(6);
          
                        Response.SetCookie(WTC_AUTHENTICATED);




                        HttpCookie shib = new HttpCookie(p.cookie.Split('=')[0]);
                        shib.Value = p.cookie.Split('=')[1].ToString().Replace("; path", "");

                        Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

                        //Modifying the AppKey from AppValue to AppValue1
                        webConfigApp.AppSettings.Settings["cookiename"].Value = p.cookie.Split('=')[0];
                        webConfigApp.AppSettings.Settings["cookieval"].Value = p.cookie.Split('=')[1].ToString().Replace("; path", "");
                        //Save the Modified settings of AppSettings.
                        webConfigApp.Save();

                        shib.Expires = DateTime.Now.AddHours(6);
                        Response.SetCookie(shib);

                    }







                }
                catch (WebException wex)
                {
                    p.content = new StreamReader(wex.Response.GetResponseStream())
                                          .ReadToEnd();
                }



                //return Json(p, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                p.content = ex.Message + ex.StackTrace;
            }

            return Json(p, JsonRequestBehavior.AllowGet);
        }



    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
namespace project.Controllers
{
    class HttpMethods
    {


        public static string Get(string url, string referer, string posteddata, ref CookieContainer cookies)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.CookieContainer = cookies;
            req.UserAgent = "";
            req.Referer = referer;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(posteddata);
            var base64 = System.Convert.ToBase64String(plainTextBytes);
            req.Headers.Add("Authorization", "Basic " + base64);
            req.Headers.Add("Accept-Encoding", "gzip, deflate");


            try
            {

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();


                string pageSrc;
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    pageSrc = sr.ReadToEnd();
                }
                return pageSrc;
            }
            catch
            {
                return "no";
            }
        }

        public static string Post(string url, string postData, string referer, CookieContainer cookies)
        {
            string key = "Login failed";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.CookieContainer = cookies;
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36";
            req.UseDefaultCredentials = true;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes("hello:123");
            var base64 = System.Convert.ToBase64String(plainTextBytes);
            req.Headers.Add("Authorization", "Basic " + base64);
            req.Headers.Add("Accept-Encoding", "gzip, deflate");
            req.Headers.Add("Accept-Encoding", "gzip, deflate");

            req.ContentType = "application/x-www-form-urlencoded";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";

            Stream postStream = req.GetRequestStream();
            // byte[] postBytes = Encoding.ASCII.GetBytes("Username=" + "hello" + "&Password=" + postData.ToString());
            // postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Dispose();

            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                //cookies.Add(resp.Cookies);

                StreamReader sr = new StreamReader(resp.GetResponseStream());
                string pageSrc = sr.ReadToEnd();
                sr.Dispose();
                Console.WriteLine(pageSrc);
                // return (!pageSrc.Contains(key));
                return (pageSrc.ToString());
            }
            catch (Exception ex) { return "no" + ex.Message; }
        }
    }
    }

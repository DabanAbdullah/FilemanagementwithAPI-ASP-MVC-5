using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using project.Models;

namespace project.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        string Baseurl = "http://localhost:52813/";
        projectdbEntities dbb = new projectdbEntities();


        
        public ActionResult Index()
        {
            var model = dbb.Requesttbls.Where(x=>x.response==null).ToList();

            return View(model);

        }
    }
}
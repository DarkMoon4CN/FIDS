using CATC.FIDS.Factory;
using CATC.FIDS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CATC.FIDS.Controllers
{
    public class DeviceManagerController :BaseController
    {
        // GET: DeviceManager
        public ActionResult DisplayInfo()
        {
            return View();
        }
    }
}
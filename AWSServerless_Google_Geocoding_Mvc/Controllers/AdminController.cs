using AWSServerless_Google_Geocoding_Mvc.Models;
using OfficeOpenXml;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.Data.Entity.SqlServer;
using System.Text;
using Amazon.S3;
using Amazon;
using System.Configuration;
using Amazon.S3.Transfer;
using System.Net.Http;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data;
using ClosedXML.Excel;
using System.Data.SqlClient;
using System.Net;
using DocumentFormat.OpenXml.Bibliography;

namespace AWSServerless_Google_Geocoding_Mvc.Controllers
{
    [ControlLogin]
    public class AdminController : Controller
    {
        // GET: Admin
        LocationDBEntities _dbContext = new LocationDBEntities();
        public ActionResult Index()
        {
            ViewBag.Name = Session["FirstName"];
            //var oncekii = _dbContext.Map.OrderByDescending(o => o.FileName).Take(1).Select(o => o.FileName).FirstOrDefault();
            //ViewBag.onceki = oncekii;
            //var onceki = _dbContext.Map.OrderBy(o => o.FileName).Take(1).Select(o => o.FileName).FirstOrDefault();
            //ViewBag.oncekii = onceki;
            //var lat = _dbContext.Map.OrderBy(o => o.FileName).Take(1).Select(o => o.Latitude).FirstOrDefault();
            //ViewBag.lat = lat;
            //var lng = _dbContext.Map.OrderBy(o => o.FileName).Take(1).Select(o => o.Longitude).FirstOrDefault();
            //ViewBag.lng = lng;
            //var address = _dbContext.Map.OrderByDescending(o => o.FileName).Select(o => o.Address).ToList();
            //ViewBag.address = address;
            int uid = Convert.ToInt32(Session["UserID"]);
            var user = _dbContext.User.Where(x => x.UserId == uid).FirstOrDefault();
            ViewBag.User = uid;

            //ViewBag.LastFileName = Request.Cookies["sonYuklenenDosya"].Value;
            //ViewBag.PrevFileName = Request.Cookies["oncekiYuklenenDosya"].Value;
            //ViewBag.BeforeFileName = Request.Cookies["dahaOncekiYuklenenDosya"].Value;
            var maps = from m in _dbContext.Map where m.UserID == uid select m;
            return View(maps);
        }

        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            LocationDBEntities db = new LocationDBEntities();
            List<Map> mapList = new List<Map>();
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["FileUpload"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileNamee = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var path = Path.Combine(Server.MapPath("~/Content/FileUpload"), file.FileName);
                    file.SaveAs(path);
                    //Save(MapList,fileName,len);
                        //using (var package = new ExcelPackage(file.InputStream))
                        //{
                        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        //    var currentSheet = package.Workbook.Worksheets;
                        //    var workSheet = currentSheet.First();
                        //    var noOfCol = workSheet.Dimension.End.Column;
                        //    var noOfRow = workSheet.Dimension.End.Row;
                        //    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        //    {

                        //        var map = new Map();
                        //        map.Address = workSheet.Cells[rowIterator, 3].Value.ToString();
                        //        //map.UserID = Convert.ToInt32(Session["UserID"]);

                        //        //map.Address = workSheet.Cells[rowIterator, 3].Value.ToString();
                        //        //map.LastFileName = Request.Cookies["sonYuklenenDosya"].Value;
                        //        //map.PrevFileName = Request.Cookies["oncekiYuklenenDosya"].Value;
                        //        //map.BeforeFileName = Request.Cookies["dahaOncekiYuklenenDosya"].Value;
                        //        //map.ModifyUserId = Convert.ToInt32(Session["UserID"]);
                        //        //map.ModifyUser = Session["FirstName"].ToString();
                        //        //map.ModifyDate = DateTime.Now;
                        //        //map.CreateUserId = Convert.ToInt32(Session["UserID"]);
                        //        //map.CreateHost = Session["FirstName"].ToString();
                        //        //map.CreateDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                        //        mapList.Add(map);
                        //        //map.FileCount = mapList.Count;
                        //    }
                        //    //ViewBag.ExcelPath = path;
                        //    //ViewBag.Path = path;
                        //}
                }
            }
            //foreach (var item in mapList)
            //{
            //    db.Map.Add(item);
            //}
            //db.SaveChanges();
            //ViewBag.Count = mapList.Count();
            return View(mapList);


            //HttpPostedFileBase file = Request.Files["FileUpload"];

            //var json = JsonConvert.SerializeObject(
            //    new
            //    {
            //        files = file,
            //        Passed = true,
            //        Mesaj = "item added"
            //    },
            //    new HttpPostedFileConverter());


            //var stringContent = new StringContent(json, Encoding.UTF8, "multipart/form-data");

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        //var userid = Session["UserID"];
            //        client.BaseAddress = new Uri("https://cp7u0uz8r9.execute-api.eu-west-2.amazonaws.com/Prod/");
            //        client.DefaultRequestHeaders.Accept.Clear();
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
            //        //HttpResponseMessage response = await client.PostAsJsonAsync("api/address/postmap?mapList=" + mapList).Result;
            //        HttpResponseMessage response = client.PostAsync("api/address/save", stringContent).Result;

            //        if (response.IsSuccessStatusCode)
            //        {
            //            return RedirectToAction("Index", "Admin");
            //        }
            //        return null;
            //    }
            //}
            //catch (Exception e)
            //{
            //    ViewBag.Hata = e.Message;
            //}
            //return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public ActionResult Save(List<Map> MapList, string fileName, int len)
        {
            getArray(MapList, fileName, len);
            return View();
        }

        [HttpPost]
        [ActionName("ArrayToController")]
        public ActionResult getArray(List<Map> MapList, string fileName, int len)
        {

            LocationDBEntities db = new LocationDBEntities();
            List<Map> mapList = new List<Map>();
            //int uid = Convert.ToInt32(Session["UserID"]);
            //var isAlreadyExists = db.Map.Where(x => x.UserID == uid && x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)).Any();
            //if (isAlreadyExists)
            //{
            //    TempData["message"] = "Aynı isimde dosya daha önceden kaydedilmiş...";
            //    return Content(@"<script language='javascript' type='text/javascript'>
            //             alert('Aynı isimde dosya daha önceden kaydedilmiş...');
            //             </script>
            //          ");
            //}
            //else
            //{
                //loop the MapList
                foreach (Map m in MapList)
                {
                    var map = new Map();
                    map.UserID = Convert.ToInt32(Session["UserID"]);
                    map.Latitude = m.Latitude;
                    map.Longitude = m.Longitude;
                    map.Address = m.Address;
                    //map.ExcelPath = path;
                    map.FileName = m.FileName;
                    //map.LastFileName = Request.Cookies["sonYuklenenDosya"].Value;
                    //map.PrevFileName = Request.Cookies["oncekiYuklenenDosya"].Value;
                    //map.BeforeFileName = Request.Cookies["dahaOncekiYuklenenDosya"].Value;
                    map.ModifyUserId = Convert.ToInt32(Session["UserID"]);
                    map.ModifyUser = Session["FirstName"].ToString();
                    map.ModifyDate = DateTime.Now;
                    map.CreateUserId = Convert.ToInt32(Session["UserID"]);
                    map.CreateHost = Session["FirstName"].ToString();
                    map.CreateDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    mapList.Add(map);
                    map.FileCount = m.FileCount;
                    foreach (var item in mapList)
                    {
                        db.Map.Add(item);
                    }
                }
            //}
                db.SaveChanges();
                return Json(mapList,JsonRequestBehavior.AllowGet);
            //}
            //LocationDBEntities db = new LocationDBEntities();
            //List<Map> mapList = new List<Map>();
            //var isAlreadyExists = db.Map.Where(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)).Any();
            //if (isAlreadyExists)
            //{
            //    TempData["message"] = "Aynı isimde dosya daha önceden kaydedilmiş...";
            //    return Content("<script language='javascript' type='text/javascript'>alert('Aynı isimde dosya daha önceden kaydedilmiş...');</script>");
            //}
            //else
            //{
            //    //loop the MapList
            //    foreach (Map m in MapList)
            //    {
            //        var map = new Map();
            //        map.UserID = Convert.ToInt32(Session["UserID"]);
            //        map.Latitude = m.Latitude;
            //        map.Longitude = m.Longitude;
            //        map.Address = m.Address;
            //        //map.ExcelPath = path;
            //        map.FileName = m.FileName;
            //        map.LastFileName = Request.Cookies["sonYuklenenDosya"].Value;
            //        map.PrevFileName = Request.Cookies["oncekiYuklenenDosya"].Value;
            //        map.BeforeFileName = Request.Cookies["dahaOncekiYuklenenDosya"].Value;
            //        map.ModifyUserId = Convert.ToInt32(Session["UserID"]);
            //        map.ModifyUser = Session["FirstName"].ToString();
            //        map.ModifyDate = DateTime.Now;
            //        map.CreateUserId = Convert.ToInt32(Session["UserID"]);
            //        map.CreateHost = Session["FirstName"].ToString();
            //        map.CreateDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            //        mapList.Add(map);
            //        map.FileCount = m.FileCount;
            //    }
            //    foreach (var item in mapList)
            //    {
            //        db.Map.Add(item);
            //    }
            //    db.SaveChanges();
            //    return Json(mapList);
            //}

        }

        //LocationDBEntities db = new LocationDBEntities();
        //List<Map> mapList = new List<Map>();
        //var isAlreadyExists = db.Map.Where(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)).Any();
        //if (isAlreadyExists)
        //{
        //    TempData["message"] = "Aynı isimde dosya daha önceden kaydedilmiş...";
        //    return Content("<script language='javascript' type='text/javascript'>alert('Aynı isimde dosya daha önceden kaydedilmiş...');</script>");
        //}
        //else
        //{
        //    //loop the MapList
        //    foreach (Map m in MapList)
        //    {
        //        var map = new Map();
        //        map.UserID = Convert.ToInt32(Session["UserID"]);
        //        map.Latitude = m.Latitude;
        //        map.Longitude = m.Longitude;
        //        map.Address = m.Address;
        //        //map.ExcelPath = path;
        //        map.FileName = m.FileName;
        //        map.LastFileName = Request.Cookies["sonYuklenenDosya"].Value;
        //        map.PrevFileName = Request.Cookies["oncekiYuklenenDosya"].Value;
        //        map.BeforeFileName = Request.Cookies["dahaOncekiYuklenenDosya"].Value;
        //        map.ModifyUserId = Convert.ToInt32(Session["UserID"]);
        //        map.ModifyUser = Session["FirstName"].ToString();
        //        map.ModifyDate = DateTime.Now;
        //        map.CreateUserId = Convert.ToInt32(Session["UserID"]);
        //        map.CreateHost = Session["FirstName"].ToString();
        //        map.CreateDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
        //        mapList.Add(map);
        //        map.FileCount = m.FileCount;
        //    }
        //    foreach (var item in mapList)
        //    {
        //        db.Map.Add(item);
        //    }
        //    db.SaveChanges();
        //    return Json(mapList);
        //}
        //public async Task<ActionResult> OnPostUploadAsync(List<IFormFile> files)
        //{
        //    long size = files.Sum(f => f.Length);

        //    //foreach (var formFile in files)
        //    //{
        //    //    if (formFile.Length > 0)
        //    //    {
        //    //        var filePath = Path.GetTempFileName();

        //    //        using (var stream = System.IO.File.Create(filePath))
        //    //        {
        //    //            await formFile.CopyToAsync(stream);
        //    //        }
        //    //    }
        //    //}

        //    var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //    var uid = Convert.ToInt32(userId);
        //    //var s3Client = new AmazonS3Client(accesskey, secretkey, bucketRegion);

        //    //var fileTransferUtility = new TransferUtility(s3Client);
        //    //if (files[0].Length == 0)
        //    //{
        //    //    return BadRequest("please provide valid file");
        //    //}
        //    var fileName = ContentDispositionHeaderValue
        //        .Parse(files[0].ContentDisposition)
        //        .FileName
        //        .TrimStart().ToString();
        //    string folderName = null;
        //    bool status;
        //    using (var fileStream = files[0].OpenReadStream())

        //    using (var ms = new MemoryStream())
        //    {
        //        await fileStream.CopyToAsync(ms);
        //        status = await _awsS3Service.UploadFileAsync(ms, fileName, folderName);
        //    }

        //    // Process uploaded files
        //    // Don't rely on or trust the FileName property without validation.

        //    return Ok(new { count = files.Count, size });
        //}


        public ActionResult AddMap(Map Map)
        {
            Map map = new Map();
            map.UserID = Convert.ToInt32(Request.Form["User"]);
            map.Address = Map.Address;
            map.Latitude = Map.Latitude;
            map.Longitude = Map.Longitude;
            map.ExcelPath = null;
            map.FileName = null;
            //map.LastFileName = Request.Form["LastFileName"];
            //map.PrevFileName = Request.Form["PrevFileName"];
            //map.BeforeFileName = Request.Form["BeforeFileName"];
            map.ModifyUserId = Convert.ToInt32(Request.Form["User"]);
            map.ModifyUser = Session["FirstName"].ToString();
            map.ModifyDate = DateTime.Now;
            map.CreateUserId = Convert.ToInt32(Request.Form["User"]);
            map.CreateHost = Session["FirstName"].ToString();
            map.CreateDate = DateTime.Now;

            var json = JsonConvert.SerializeObject(map);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://cp7u0uz8r9.execute-api.eu-west-2.amazonaws.com/Prod/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsync("api/address/savemap", stringContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Admin");
                }

                return RedirectToAction("Index", "Admin");

            }
        }

        public ActionResult Liste()
        {
            ViewBag.Name = Session["FirstName"];
            return View();
        }

        public ActionResult Log()
        {
            ViewBag.Name = Session["FirstName"];
            int uid = Convert.ToInt32(Session["UserID"]);
            var maps = from m in _dbContext.Map where m.UserID == uid select m;
            var pathh = from p in _dbContext.Map where p.UserID == uid select p.ExcelPath;
            var fileNamee = from p in _dbContext.Map where p.UserID == uid select p.FileName;
            ViewBag.Pathh = pathh;
            ViewBag.FileNamee = fileNamee;
            return View(maps);
        }


        public JsonResult GetMapss()
        {
            var userId = Session["UserID"];
            var uid = Convert.ToInt32(userId);

            //var maps = _dbContext.Map.ToList();

            //var query = from m in _dbContext.Map
            //            where m.FileName == fileName
            //            select new { Latitude = m.Latitude, Longitude = m.Longitude, Address = m.Address, FileName = m.FileName, LastFileName = m.LastFileName, CreateHost = m.CreateHost, CreateDate = m.CreateDate, FileCount = m.FileCount };
            var query = _dbContext.Map
                                .Where(x => x.UserID == uid)
                                .Select(m => new
                                {
                                    UserID = uid,
                                    Latitude = m.Latitude,
                                    Longitude = m.Longitude,
                                    Address = m.Address,
                                    FileName = m.FileName,
                                    //LastFileName = m.LastFileName,
                                    CreateHost = m.CreateHost,
                                    CreateDate = m.CreateDate,
                                    FileCount = m.FileCount
                                });
            //var maps = query.ToList();
            var maps = query.ToList().Select(r => new Map
            {
                UserID = uid,
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                Address = r.Address,
                FileName = r.FileName,
                //LastFileName = r.LastFileName,
                CreateHost = r.CreateHost,
                CreateDate = Convert.ToDateTime(r.CreateDate.Value.ToShortDateString()),
                FileCount = r.FileCount
            }).ToList();


            //var query =
            //               (from m in _dbContext.Map
            //                where m.FileName == fileName
            //                select new { Latitude = m.Latitude, Longitude = m.Longitude, Address = m.Address, FileName = m.FileName, LastFileName = m.LastFileName, CreateHost = m.CreateHost, CreateDate = m.CreateDate, FileCount = m.FileCount });
            //    var maps = query.ToList().Select(r => new Map
            //    {
            //        Latitude = r.Latitude,
            //        Longitude = r.Longitude,
            //        Address = r.Address,
            //        FileName = r.FileName,
            //        LastFileName = r.LastFileName,
            //        CreateHost = r.CreateHost,
            //        FileCount = r.FileCount,
            //        CreateDate = Convert.ToDateTime(r.CreateDate.Value.ToShortDateString())
            //    }).ToList();


            return Json(maps, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetMaps(string fileName)
        {
            var userId = Session["UserID"];
            var uid = Convert.ToInt32(userId);

            //var maps = _dbContext.Map.ToList();

            //var query = from m in _dbContext.Map
            //            where m.FileName == fileName
            //            select new { Latitude = m.Latitude, Longitude = m.Longitude, Address = m.Address, FileName = m.FileName, LastFileName = m.LastFileName, CreateHost = m.CreateHost, CreateDate = m.CreateDate, FileCount = m.FileCount };
            var query = _dbContext.Map
                                .Where(x => x.UserID == uid && x.FileName == fileName)
                                .Select(m => new
                                {
                                    Latitude = m.Latitude,
                                    Longitude = m.Longitude,
                                    Address = m.Address,
                                    FileName = m.FileName,
                                    //LastFileName = m.LastFileName,
                                    CreateHost = m.CreateHost,
                                    CreateDate = m.CreateDate,
                                    FileCount = m.FileCount
                                });
            //var maps = query.ToList();
            var maps = query.ToList().Select(r => new Map
            {
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                Address = r.Address,
                FileName = r.FileName,
                //LastFileName = r.LastFileName,
                CreateHost = r.CreateHost,
                CreateDate = Convert.ToDateTime(r.CreateDate.Value.ToShortDateString()),
                FileCount = r.FileCount
            }).ToList();


            //var query =
            //               (from m in _dbContext.Map
            //                where m.FileName == fileName
            //                select new { Latitude = m.Latitude, Longitude = m.Longitude, Address = m.Address, FileName = m.FileName, LastFileName = m.LastFileName, CreateHost = m.CreateHost, CreateDate = m.CreateDate, FileCount = m.FileCount });
            //    var maps = query.ToList().Select(r => new Map
            //    {
            //        Latitude = r.Latitude,
            //        Longitude = r.Longitude,
            //        Address = r.Address,
            //        FileName = r.FileName,
            //        LastFileName = r.LastFileName,
            //        CreateHost = r.CreateHost,
            //        FileCount = r.FileCount,
            //        CreateDate = Convert.ToDateTime(r.CreateDate.Value.ToShortDateString())
            //    }).ToList();


            return Json(maps, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FileList(string basTarih, string bitTarih)
        {
            int userid = Convert.ToInt32(Session["UserID"]);
            DateTime start = DateTime.MinValue;
            DateTime end = DateTime.MaxValue;
            var sDs = basTarih;
            var eDs = bitTarih;
            DateTime.TryParse(sDs, out start);
            DateTime.TryParse(eDs, out end);
            var userId = Session["UserID"];
            var uid = Convert.ToInt32(userId);
            //var mapsData = from m in dbContext.Map where m.UserId == uid select m;
            List<Map> mapList = new List<Map>();
            var query = (from m in _dbContext.Map
                         where m.UserID == uid
                         select new { Latitude = m.Latitude, Longitude = m.Longitude, Address = m.Address, FileName = m.FileName, CreateHost = m.CreateHost, CreateDate = m.CreateDate, FileCount = m.FileCount });
            mapList = query.ToList().Select(r => new Map
            {
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                Address = r.Address,
                FileName = r.FileName,
                //LastFileName = r.LastFileName,
                CreateHost = r.CreateHost,
                CreateDate = Convert.ToDateTime(r.CreateDate.Value.ToShortDateString()),
                FileCount = r.FileCount,
            }).ToList();


            if (start != DateTime.MinValue && end != DateTime.MinValue)
            {
                mapList = mapList.Where(x => Convert.ToDateTime(x.CreateDate) >= start && Convert.ToDateTime(x.CreateDate) <= end).ToList();
            }

            return Json(mapList, JsonRequestBehavior.AllowGet);
            //using (var client = new HttpClient())
            //{
            //    userid = Convert.ToInt32(Session["UserID"]);
            //    client.BaseAddress = new Uri("https://cp7u0uz8r9.execute-api.eu-west-2.amazonaws.com/Prod/");
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Authorization =
            //        new AuthenticationHeaderValue("Bearer", token);
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    HttpResponseMessage response = client.GetAsync("api/address/getmaplist").Result;

            //    List<Map> model = null;

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var jsonString = await response.Content.ReadAsStringAsync();
            //        model = JsonConvert.DeserializeObject<List<Map>>(jsonString);

            //        if (start != DateTime.MinValue && end != DateTime.MinValue)
            //        {
            //            model = model.Where(x => x.CreateDate >= start.Date && x.CreateDate <= end.Date).ToList();
            //            return Json(model, JsonRequestBehavior.AllowGet);
            //        }
            //    }

            //    return Json(model, JsonRequestBehavior.AllowGet);

            //}
        }

        public void ExportToExcel()
        {
            LocationDBEntities _dbContext = new LocationDBEntities();
            int uid = Convert.ToInt32(Session["UserID"]);
            System.Data.DataTable dtMaps = new System.Data.DataTable();
            dtMaps.Columns.AddRange(new DataColumn[4] {
                      new DataColumn("Enlem"),
                      new DataColumn("Boylam"),
                      new DataColumn("Adres"),
                      new DataColumn("Tarih"),

          });

            List<Map> mapList = new List<Map>();
            var query = (from m in _dbContext.Map
                         where m.UserID == uid
                         select new { Latitude = m.Latitude, Longitude = m.Longitude, Address = m.Address, FileName = m.FileName, CreateHost = m.CreateHost, CreateDate = m.CreateDate, FileCount = m.FileCount });
            mapList = query.ToList().Select(r => new Map
            {
                Latitude = r.Latitude.ToString(),
                Longitude = r.Longitude.ToString(),
                Address = r.Address,
                FileName = r.FileName,
                //LastFileName = r.LastFileName,
                CreateHost = r.CreateHost,
                CreateDate = Convert.ToDateTime(r.CreateDate.Value.ToShortDateString()),
                FileCount = r.FileCount
            }).OrderByDescending(x => x.CreateDate).Take(50000).ToList();

            //var maps = _dbContext.Map.Where(x => x.UserID == uid).OrderByDescending(x => x.CreateDate).Take(50000).ToList();

            foreach (var map in mapList)
            {
                //double d = Convert.ToDouble(map.Latitude);
                //string s = d.ToString("0:0,0");
                //double dd = Convert.ToDouble(map.Longitude);
                //string ss = dd.ToString("0:0,0");
                dtMaps.Rows.Add(map.Latitude.ToString().Replace('.', ','), map.Longitude.ToString().Replace('.', ','), map.Address, map.CreateDate.Value.ToShortDateString());
            }


            string filename = DateTime.Now.ToShortDateString() + " Lokasyon Listesi";
            var grid = new GridView();
            grid.DataSource = dtMaps;
            grid.DataBind();

            Response.ClearContent();
            Response.Charset = "utf-8";
            Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xls");

            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());
            Response.End();
        }

        public void ExportToExcell()
        {
            LocationDBEntities _dbContext = new LocationDBEntities();
            int uid = Convert.ToInt32(Session["UserID"]);
            System.Data.DataTable dtMaps = new System.Data.DataTable();
            dtMaps.Columns.AddRange(new DataColumn[3] {
                      new DataColumn("Enlem"),
                      new DataColumn("Boylam"),
                      new DataColumn("Adres"),

            });

            Map map = new Map();

            dtMaps.Rows.Add(map.Latitude, map.Longitude, map.Address);

            string filename = DateTime.Now.ToShortDateString() + " Lokasyon Listesi";
            var grid = new GridView();
            grid.DataSource = dtMaps;
            grid.DataBind();

            Response.ClearContent();
            Response.Charset = "utf-8";
            Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xls");

            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());
            Response.End();
        }

        public void SonExport(string fileName)
        {
            LocationDBEntities _dbContext = new LocationDBEntities();
            int uid = Convert.ToInt32(Session["UserID"]);
            var lastFile = fileName;
            System.Data.DataTable dtMaps = new System.Data.DataTable();
            dtMaps.Columns.AddRange(new DataColumn[4] {
                      new DataColumn("Enlem"),
                      new DataColumn("Boylam"),
                      new DataColumn("Adres"),
                      new DataColumn("Tarih"),

          });

            List<Map> mapList = new List<Map>();
            var query = (from m in _dbContext.Map
                         where m.UserID == uid && m.FileName == lastFile
                         select new { Latitude = m.Latitude, Longitude = m.Longitude, Address = m.Address, FileName = m.FileName, CreateHost = m.CreateHost, CreateDate = m.CreateDate, FileCount = m.FileCount });
            mapList = query.ToList().Select(r => new Map
            {
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                Address = r.Address,
                FileName = r.FileName,
                //LastFileName = r.LastFileName,
                CreateHost = r.CreateHost,
                CreateDate = Convert.ToDateTime(r.CreateDate.Value.ToShortDateString()),
                FileCount = r.FileCount
            }).ToList();


            var isAlreadyExists = _dbContext.Map.Where(x => x.FileName.Equals(lastFile, StringComparison.InvariantCultureIgnoreCase)).Any();

            if (isAlreadyExists)
            {
                mapList = query.ToList().Select(r => new Map
                {
                    Latitude = r.Latitude,
                    Longitude = r.Longitude,
                    Address = r.Address,
                    FileName = r.FileName,
                    //LastFileName = r.LastFileName,
                    CreateHost = r.CreateHost,
                    CreateDate = Convert.ToDateTime(r.CreateDate.Value.ToShortDateString()),
                    FileCount = r.FileCount
                }).ToList();

                foreach (var map in mapList)
                {
                    dtMaps.Rows.Add(map.Latitude.ToString().Replace('.', ','), map.Longitude.ToString().Replace('.', ','), map.Address, map.CreateDate.Value.ToShortDateString());
                }
            }
            else
            {
                var maps = _dbContext.Map.Where(x => x.UserID == uid && x.FileName == lastFile).OrderByDescending(x => x.CreateDate).Take(50000).ToList();
                foreach (var map in maps)
                {
                    dtMaps.Rows.Add(map.Latitude.ToString().Replace('.', ','), map.Longitude.ToString().Replace('.', ','), map.Address, map.CreateDate.Value.ToShortDateString());
                }
            }


            string filename = DateTime.Now.ToShortDateString() + " Lokasyon Listesi";
            var grid = new GridView();
            grid.DataSource = dtMaps;
            grid.DataBind();

            Response.ClearContent();
            Response.Charset = "utf-8";
            Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xls");

            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());
            Response.End();
        }



        //public ActionResult Export()
        //{
        //    LocationDBEntities _dbContext = new LocationDBEntities();
        //    int uid = Convert.ToInt32(Session["UserID"]);
        //    //var maps = from m in _dbContext.Map where m.UserID == uid orderby m.CreateDate select m;
        //    //var maps = _dbContext.Map.Where(x => x.UserID == uid).OrderByDescending(x => x.CreateDate).Take(50000).ToList();
        //    var maps = _dbContext.Map.Where(x => x.UserID == uid).OrderByDescending(x => x.CreateDate).Take(50000).ToList();
        //    //ExcelPackage Ep = new ExcelPackage();
        //    //ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
        //    //Sheet.Cells["A1"].Value = "Enlem";
        //    //Sheet.Cells["B1"].Value = "Boylam";
        //    //Sheet.Cells["C1"].Value = "Adres";
        //    //Sheet.Cells["D1"].Value = "Tarih";
        //    //var i = 2;
        //    //foreach (var item in maps)
        //    //{
        //    //    Sheet.Cells[string.Format("A{0}", item)].Value = item.Latitude;
        //    //    Sheet.Cells[string.Format("B{0}", item)].Value = item.Longitude;
        //    //    Sheet.Cells[string.Format("C{0}", item)].Value = item.Address;
        //    //    Sheet.Cells[string.Format("D{0}", item)].Value = item.CreateDate.Value.ToShortDateString();
        //    //    i++;
        //    //}

        //    //Sheet.Cells["A:AZ"].AutoFitColumns();
        //    //Response.Clear();
        //    //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    //Response.AddHeader("content-disposition","attachment: filename="+"ExcelReport.xlsx");
        //    //Response.BinaryWrite(Ep.GetAsByteArray());
        //    //Response.End();
        //}

        //public ActionResult Export()
        //{
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri("https://cp7u0uz8r9.execute-api.eu-west-2.amazonaws.com/Prod/");
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        HttpResponseMessage response = client.GetAsync("api/address/export").Result;
        //        if (response.IsSuccessStatusCode)
        //        {
        //            return RedirectToAction("Index", "Admin");
        //        }

        //        return RedirectToAction("Index", "Admin");

        //    }
        //}

        //public ActionResult SonExport(string lastFileName)
        //{
        //    LocationDBEntities _dbContext = new LocationDBEntities();
        //    int uid = Convert.ToInt32(Session["UserID"]);
        //    var lastFile = lastFileName;
        //    var maps = _dbContext.Map.Where(x => x.UserID == uid && x.LastFileName == lastFile).OrderByDescending(x => x.CreateDate).Take(50000).ToList();
        //    Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
        //    Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
        //    Worksheet ws = (Worksheet)xla.ActiveSheet;
        //    ws.Name = DateTime.Now.ToShortDateString() + "" + lastFile;
        //    xla.Visible = true;
        //    ws.Cells[1, 1] = "Enlem";
        //    ws.Cells[1, 2] = "Boylam";
        //    ws.Cells[1, 3] = "Adres";
        //    ws.Cells[1, 4] = "Tarih";
        //    var i = 2;
        //    foreach (var item in maps)
        //    {
        //        ws.Cells[i, 1] = item.Latitude;
        //        ws.Cells[i, 2] = item.Longitude;
        //        ws.Cells[i, 3] = item.Address;
        //        ws.Cells[i, 4] = item.CreateDate.Value.ToShortDateString();
        //        i++;
        //    }

        //    return View("Index");
        //}

        //public ActionResult SonExport(string lastFileName)
        //{
        //    var lastFile = lastFileName;

        //    var json = JsonConvert.SerializeObject(lastFile);
        //    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri("https://cp7u0uz8r9.execute-api.eu-west-2.amazonaws.com/Prod/");
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        HttpResponseMessage response = client.PostAsync("api/address/lastexport", stringContent).Result;
        //        if (response.IsSuccessStatusCode)
        //        {
        //            return RedirectToAction("Index", "Admin");
        //        }

        //        return RedirectToAction("Index", "Admin");

        //    }
        //}
    }
}
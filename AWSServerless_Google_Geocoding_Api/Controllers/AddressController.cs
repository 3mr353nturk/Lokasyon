using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using AWSServerless_Google_Geocoding_Api.Domain;
using AWSServerless_Google_Geocoding_Api.Domain.Helpers;
using AWSServerless_Google_Geocoding_Api.Domain.Model;
using AWSServerless_Google_Geocoding_Api.Domain.Responses;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;

namespace AWSServerless_Google_Geocoding_Api.Controllers
{
    [ApiController]
    public class AddressController : Controller
    {

        private readonly LocationDBContext dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAwsS3HelperService _awsS3Service;

        public AddressController(LocationDBContext _dbContext, IHttpContextAccessor httpContextAccessor, IAwsS3HelperService awsS3Service)
        {
            dbContext = _dbContext;
            _httpContextAccessor = httpContextAccessor;
            _awsS3Service = awsS3Service;
        }

        [Authorize]
        [HttpGet]
        [Route("api/address/getmaplist")]
        public IQueryable<Map> GetMapList()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);
            //var mapsData = from m in dbContext.Map where m.UserId == uid select m;
            var maps = dbContext.Map.Select(x => new Map()
            {
                UserId = x.UserId,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Address = x.Address,
                ExcelPath = x.ExcelPath,
                FileName = x.FileName,
                LastFileName = x.LastFileName,
                PrevFileName = x.PrevFileName,
                BeforeFileName = x.BeforeFileName,
                FileCount = x.FileCount,
                ModifyUserId = x.ModifyUserId,
                ModifyUser = x.ModifyUser,
                ModifyDate = x.ModifyDate,
                CreateUserId = x.CreateUserId,
                CreateHost = x.CreateHost,
                CreateDate = x.CreateDate
            });
            return maps;
        }

        [HttpPost]
        [Route("api/address/savemap")]
        public IActionResult SaveMap([FromBody] Map map)
        {
            dbContext.Map.Add(new Map()
            {
                UserId = map.UserId,
                Address = map.Address,
                Latitude = map.Latitude,
                Longitude = map.Longitude,
                ExcelPath = map.ExcelPath,
                FileName = map.FileName,
                LastFileName = map.LastFileName,
                PrevFileName = map.PrevFileName,
                BeforeFileName = map.BeforeFileName,
                ModifyUserId = map.ModifyUserId,
                ModifyUser = map.ModifyUser,
                ModifyDate = map.ModifyDate,
                CreateUserId = map.CreateUserId,
                CreateHost = map.CreateHost,
                CreateDate = map.CreateDate
            });
            dbContext.SaveChanges();
            return Ok("Kayıt Başarıyla eklendi"); ;
        }


        [HttpPost]
        [Route("api/address/save")]
        public async Task<IActionResult> Save(IFormFile formFile)
        {
            if (formFile.Length == 0)
            {
                return BadRequest("please provide valid file");
            }
            var fileName = ContentDispositionHeaderValue
                .Parse(formFile.ContentDisposition)
                .FileName
                .TrimStart().ToString();
            var folderName = Request.Form.ContainsKey("folder") ? Request.Form["folder"].ToString() : null;
            bool status;
            using (var fileStream = formFile.OpenReadStream())

            using (var ms = new MemoryStream())
            {
                await fileStream.CopyToAsync(ms);
                status = await _awsS3Service.UploadFileAsync(ms, fileName, folderName);

                //db.User.Add(new User()
                //{
                //    FirstName = user.FirstName,
                //    LastName = user.LastName,
                //    Password = user.Password,
                //    Company = user.Company,
                //    Email = user.Email,
                //    UserName = user.UserName,
                //    CompanyLogo = "https://takimomrubucketyeni.s3.eu-west-2.amazonaws.com/Images/" + fileName
                //});
                //db.SaveChanges();
                //List<Map> mapList = new List<Map>();
                //string fileContentType = FileUpload.ContentType;
                //byte[] fileBytes = new byte[FileUpload.Length];
                //var path = "https://geocodingapi.s3.eu-west-2.amazonaws.com/" + FileUpload.FileName;
                ////FileUpload.SaveAs(path);
                //var inputStream = new FileStream(path, FileMode.Create);
                //var data = inputStream.Read(fileBytes, 0, Convert.ToInt32(FileUpload.Length));
                //using (var package = new ExcelPackage(inputStream))
                //{
                //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //    var currentSheet = package.Workbook.Worksheets;
                //    var workSheet = currentSheet.First();
                //    var noOfCol = workSheet.Dimension.End.Column;
                //    var noOfRow = workSheet.Dimension.End.Row;
                //    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                //    {

                //        var map = new Map();
                //        //map.UserId = uid;
                //        map.Latitude = workSheet.Cells[rowIterator, 1].Value.ToString();
                //        map.Longitude = workSheet.Cells[rowIterator, 2].Value.ToString();
                //        map.Address = workSheet.Cells[rowIterator, 3].Value.ToString();
                //        map.ExcelPath = path;
                //        map.FileName = fileName;
                //        //map.LastFileName = _httpContextAccessor.HttpContext.Request.Cookies["sonYuklenenDosya"];
                //        //map.PrevFileName = _httpContextAccessor.HttpContext.Request.Cookies["oncekiYuklenenDosya"];
                //        //map.BeforeFileName = _httpContextAccessor.HttpContext.Request.Cookies["dahaOncekiYuklenenDosya"];
                //        //map.ModifyUserId = uid;
                //        //map.ModifyUser = userId;
                //        map.ModifyDate = DateTime.Now;
                //        //map.CreateUserId = uid;
                //        //map.CreateHost = userId;
                //        map.CreateDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                //        mapList.Add(map);
                //        map.FileCount = mapList.Count;
                //    }
                //}
                //foreach (var item in mapList)
                //{
                //    dbContext.Map.Add(item);
                //}
                //dbContext.SaveChanges();
            }

            return status ? Ok("success")
                         : StatusCode((int)HttpStatusCode.InternalServerError, $"error uploading {fileName}");
        }

        [HttpPost]
        [Route("api/address/import")]
        public IActionResult Import(IFormFile formFile)
        {
            List<Map> mapList = new List<Map>();
            if ((formFile != null) && (formFile.Length > 0) && !string.IsNullOrEmpty(formFile.FileName))
            {
                string fileName = formFile.FileName;
                string fileContentType = formFile.ContentType;
                byte[] fileBytes = new byte[formFile.Length];
                var path = "https://geocodingapi.s3.eu-west-2.amazonaws.com/" + formFile.FileName;
                var data = formFile.OpenReadStream().Read(fileBytes, 0, Convert.ToInt32(formFile.Length));
                using (var package = new ExcelPackage(formFile.OpenReadStream()))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    //Map maps = new Map();
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {

                        var map = new Map();
                        //map.UserId = Convert.ToInt32(Session["UserID"]);
                        map.Latitude = workSheet.Cells[rowIterator, 1].Value.ToString();
                        map.Longitude = workSheet.Cells[rowIterator, 2].Value.ToString();
                        map.Address = workSheet.Cells[rowIterator, 3].Value.ToString();
                        map.ExcelPath = path;
                        map.FileName = fileName;
                        //map.LastFileName = Request.Cookies["sonYuklenenDosya"].Value;
                        //map.PrevFileName = Request.Cookies["oncekiYuklenenDosya"].Value;
                        //map.BeforeFileName = Request.Cookies["dahaOncekiYuklenenDosya"].Value;
                        //map.ModifyUserId = Convert.ToInt32(Session["UserID"]);
                        //map.ModifyUser = Session["FirstName"].ToString();
                        map.ModifyDate = DateTime.Now;
                        //map.CreateUserId = Convert.ToInt32(Session["UserID"]);
                        //map.CreateHost = Session["FirstName"].ToString();
                        map.CreateDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                        mapList.Add(map);
                        map.FileCount = mapList.Count;
                    }
                }
                foreach (var item in mapList)
                {
                    dbContext.Map.Add(item);
                }
                dbContext.SaveChanges();
            }

            return Ok();

        }

        [HttpGet]
        [Route("api/address/export")]
        public IActionResult Excel()
        {
            using (var workbook = new XLWorkbook())
            {
                var maps = dbContext.Map.OrderByDescending(x => x.CreateDate).Take(50000).ToList();
                var worksheet = workbook.Worksheets.Add("Maps");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Enlem";
                worksheet.Cell(currentRow, 2).Value = "Boylam";
                worksheet.Cell(currentRow, 3).Value = "Address";
                worksheet.Cell(currentRow, 4).Value = "Tarih";
                foreach (var item in maps)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Latitude;
                    worksheet.Cell(currentRow, 2).Value = item.Longitude;
                    worksheet.Cell(currentRow, 3).Value = item.Longitude;
                    worksheet.Cell(currentRow, 4).Value = item.Longitude;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        DateTime.Now.ToShortDateString() + ".xlsx");
                }
            }
        }


        //var maps = dbContext.Map.OrderByDescending(x => x.CreateDate).Take(50000).ToList();
        //Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
        //Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
        //Worksheet ws = (Worksheet)xla.ActiveSheet;
        //ws.Name = DateTime.Now.ToShortDateString();
        //xla.Visible = true;
        //ws.Cells[1, 1] = "Enlem";
        //ws.Cells[1, 2] = "Boylam";
        //ws.Cells[1, 3] = "Adres";
        //ws.Cells[1, 4] = "Tarih";
        //var i = 2;
        //foreach (var item in maps)
        //{
        //    ws.Cells[i, 1] = item.Latitude;
        //    ws.Cells[i, 2] = item.Longitude;
        //    ws.Cells[i, 3] = item.Address;
        //    ws.Cells[i, 4] = item.CreateDate.Value.ToShortDateString();
        //    i++;
        //}
        //using (var stream = new MemoryStream())
        //{
        //    wb.SaveAs(stream);
        //    var content = stream.ToArray();

        //        return File(
        //            content,
        //            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //            DateTime.Now.ToShortDateString() + ".xlsx");
        //}
        //public IActionResult Export()
        //{
        //    //int uid = Convert.ToInt32(Session["UserID"]);
        //    //var maps = from m in _dbContext.Map where m.UserID == uid orderby m.CreateDate select m;
        //    //var maps = _dbContext.Map.Where(x => x.UserID == uid).OrderByDescending(x => x.CreateDate).Take(50000).ToList();
        //    var maps = dbContext.Map.OrderByDescending(x => x.CreateDate).Take(50000).ToList();
        //    Microsoft.Office.Interop.Excel.Application xla = new Microsoft.Office.Interop.Excel.Application();
        //    Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
        //    Worksheet ws = (Worksheet)xla.ActiveSheet;
        //    ws.Name = DateTime.Now.ToShortDateString();
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

        //    return Ok("Kayıt excel'e aktarıldı.");
        //}


        [HttpPost]
        [Route("api/address/lastexport")]
        public IActionResult SonExport(string lastFileName)
        {
            using (var workbook = new XLWorkbook())
            {
                //var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                //var uid = Convert.ToInt32(userId);
                var lastFile = lastFileName;
                var maps = dbContext.Map.Where(x => x.FileName == lastFile).OrderByDescending(x => x.CreateDate).Take(50000).ToList();
                var worksheet = workbook.Worksheets.Add("Maps");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Enlem";
                worksheet.Cell(currentRow, 2).Value = "Boylam";
                worksheet.Cell(currentRow, 3).Value = "Address";
                worksheet.Cell(currentRow, 4).Value = "Tarih";
                foreach (var item in maps)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Latitude;
                    worksheet.Cell(currentRow, 2).Value = item.Longitude;
                    worksheet.Cell(currentRow, 3).Value = item.Longitude;
                    worksheet.Cell(currentRow, 4).Value = item.Longitude;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        DateTime.Now.ToShortDateString() + ".xlsx");
                }
            }
        }

        //public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
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
        //    if (files[0].Length == 0)
        //    {
        //        return BadRequest("please provide valid file");
        //    }
        //    var fileName = ContentDispositionHeaderValue
        //        .Parse(files[0].ContentDisposition)
        //        .FileName
        //        .TrimStart().ToString();
        //    var folderName = Request.Form.ContainsKey("folder") ? Request.Form["folder"].ToString() : null;
        //    bool status;
        //    using (var fileStream = files[0].OpenReadStream())

        //    using (var ms = new MemoryStream())
        //    {
        //        await fileStream.CopyToAsync(ms);
        //        status = await _awsS3Service.UploadFileAsync(ms, fileName, folderName);
        //    }

        //        // Process uploaded files
        //        // Don't rely on or trust the FileName property without validation.

        //   return Ok(new { count = files.Count, size });
        //}

        //[Authorize]
        //[HttpPost]
        //[Route("api/address/save")]
        //public async Task<IActionResult> Save(IFormFile FileUpload)
        //{
        //    //var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //    //var uid = Convert.ToInt32(userId);
        //    //var s3Client = new AmazonS3Client(accesskey, secretkey, bucketRegion);

        //    //var fileTransferUtility = new TransferUtility(s3Client);
        //    if (FileUpload.Length == 0)
        //    {
        //        return BadRequest("please provide valid file");
        //    }
        //    var fileName = ContentDispositionHeaderValue
        //        .Parse(FileUpload.ContentDisposition)
        //        .FileName
        //        .TrimStart().ToString();
        //    var folderName = Request.Form.ContainsKey("folder") ? Request.Form["folder"].ToString() : null;
        //    bool status;
        //    using (var fileStream = FileUpload.OpenReadStream())

        //    using (var ms = new MemoryStream())
        //    {
        //        await fileStream.CopyToAsync(ms);
        //        status = await _awsS3Service.UploadFileAsync(ms, fileName, folderName);
        //        //db.User.Add(new User()
        //        //{
        //        //    FirstName = user.FirstName,
        //        //    LastName = user.LastName,
        //        //    Password = user.Password,
        //        //    Company = user.Company,
        //        //    Email = user.Email,
        //        //    UserName = user.UserName,
        //        //    CompanyLogo = "https://takimomrubucketyeni.s3.eu-west-2.amazonaws.com/Images/" + fileName
        //        //});
        //        //db.SaveChanges();
        //        string fileContentType = FileUpload.ContentType;
        //        byte[] fileBytes = new byte[FileUpload.Length];
        //        var path = "https://geocodingapi.s3.eu-west-2.amazonaws.com/" + FileUpload.FileName;
        //        //FileUpload.SaveAs(path);
        //        var inputStream = new FileStream(path, FileMode.Create);
        //        var data = inputStream.Read(fileBytes, 0, Convert.ToInt32(FileUpload.Length));
        //        using (var package = new ExcelPackage(inputStream))
        //        {
        //            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //            var currentSheet = package.Workbook.Worksheets;
        //            var workSheet = currentSheet.First();
        //            var noOfCol = workSheet.Dimension.End.Column;
        //            var noOfRow = workSheet.Dimension.End.Row;
        //            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
        //            {

        //                var map = new Map();
        //                //map.UserId = uid;
        //                map.Latitude = workSheet.Cells[rowIterator, 1].Value.ToString();
        //                map.Longitude = workSheet.Cells[rowIterator, 2].Value.ToString();
        //                map.Address = workSheet.Cells[rowIterator, 3].Value.ToString();
        //                map.ExcelPath = path;
        //                map.FileName = fileName;
        //                //map.LastFileName = _httpContextAccessor.HttpContext.Request.Cookies["sonYuklenenDosya"];
        //                //map.PrevFileName = _httpContextAccessor.HttpContext.Request.Cookies["oncekiYuklenenDosya"];
        //                //map.BeforeFileName = _httpContextAccessor.HttpContext.Request.Cookies["dahaOncekiYuklenenDosya"];
        //                //map.ModifyUserId = uid;
        //                //map.ModifyUser = userId;
        //                map.ModifyDate = DateTime.Now;
        //                //map.CreateUserId = uid;
        //                //map.CreateHost = userId;
        //                map.CreateDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
        //                mapList.Add(map);
        //                map.FileCount = mapList.Count;
        //            }
        //        }
        //        foreach (var item in mapList)
        //        {
        //            dbContext.Map.Add(item);
        //        }
        //        dbContext.SaveChanges();
        //    }
        //    return status ? Ok("success")
        //                 : StatusCode((int)HttpStatusCode.InternalServerError, $"error uploading {fileName}");
        //}



        [Authorize]
        [HttpGet]
        [Route("api/address/getmaplistbyid/{id}")]
        // for get Map data by UserID  
        public ResponseModel GetMapListById(int UserID)
        {
            ResponseModel _modelObj = new ResponseModel();
            List<Map> maps = GetMapDetails(UserID);
            if (maps.Count > 0)
            {
                _modelObj.Data = maps;
                _modelObj.Status = true;
                _modelObj.Message = "Data listelendi";
            }
            else
            {
                _modelObj.Data = maps;
                _modelObj.Status = false;
                _modelObj.Message = "Data bulunamadı";
            }
            return _modelObj;
        }
        private List<Map> GetMapDetails(int UserID)
        {
            List<Map> maps = null;
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var uid = Convert.ToInt32(userId);
            try
            {
                maps = (from ds in dbContext.Map
                        where ds.UserId == uid
                        select new Map
                        {
                            Id = ds.Id,
                            Latitude = ds.Latitude,
                            Longitude = ds.Longitude,
                            Address = ds.Address,
                            UserId = uid
                        }).ToList();
                return maps;
            }
            catch
            {
                maps = null;

            }
            return maps;
        }
    }
}
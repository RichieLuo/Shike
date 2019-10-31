using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Net.Http.Headers;

namespace ShiKe.Web.Controllers.Upload
{
    /// <summary>
    /// 用于处理上传文件和相应的存储处理的控制器
    /// </summary>
    public class UploadFileController : Controller
    {
        private IHostingEnvironment _HostingEnv;

        public UploadFileController(IHostingEnvironment hostingEnv)
        {
            this._HostingEnv = hostingEnv;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ForSimple(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            return Ok(new { count = files.Count, size, filePath });
        }

        public IActionResult FromFormFiles(List<IFormFile> files)
        {
            long size = 0;
            foreach (var file in files)
            {
                //var fileName = file.FileName;
                var fileName = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName
                                .Trim('"')
                                .Substring(file.FileName.LastIndexOf("\\") + 1);
                fileName = _HostingEnv.WebRootPath + $@"\uploadFiles\{fileName}";
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(fileName))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            ViewBag.Message = $"{files.Count}个文件 /{size}字节上传成功!";
            return View();
        }

        public IActionResult FromAjaxFiles()
        {
            return View();
        }

        public IActionResult SaveFromAjaxFiles()
        {
            long size = 0;
            var files = Request.Form.Files;
            foreach (var file in files)
            {
                //var fileName = file.FileName;
                var fileName = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName
                                .Trim('"')
                                .Substring(file.FileName.LastIndexOf("\\") + 1);
                fileName = _HostingEnv.WebRootPath + $@"\uploadFiles\{fileName}";
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(fileName))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            return Json(new { isOK = true, fileCount = files.Count, size = size });
        }
    }
}
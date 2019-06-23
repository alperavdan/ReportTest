using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jsreport.AspNetCore;
using jsreport.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JSReportTestApp.Controllers
{
    [Route("api/[controller]")]
    public class JSReportController : Controller
    {
        private IJsReportMVCService _jsreportService;
        private IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;
        public JSReportController(IJsReportMVCService jsreportService,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _jsreportService = jsreportService;
        }
        /// <summary>
        /// pdf rapor görüntülemek için browser'a url'i giriniz
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            var report = _jsreportService.RenderAsync(new RenderRequest()
            {
                Template = new Template
                {
                    Content = "<h1>Hello world -> {{name}}</h1>",
                    Engine = Engine.Handlebars,
                    Recipe = Recipe.ChromePdf
                },
                Data = new
                {
                    name = "Cemre"
                }
            }).Result;

            return File(report.Content, report.Meta.ContentType);
        }
        [HttpPost]
        [Route("upload")]
        public async Task<string> Upload(IFormFile uploadFile)
        {
            var directoryName = _configuration.GetSection("FileSystem").GetSection("DirectoryName").Value;
            var fileDirectory = _configuration.GetSection("FileSystem").GetSection("FileDirectory").Value;
            string combinedDirectoryPath = System.IO.Path.Combine(directoryName, fileDirectory);

            string directoryPath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, combinedDirectoryPath);
            System.IO.Directory.CreateDirectory(directoryPath);

            string fullPath = System.IO.Path.Combine(directoryPath, uploadFile.FileName);
            bool doesExist = System.IO.File.Exists(fullPath);
            if (uploadFile.Length > 0 && !doesExist)  // Needs re-work.. Should save file but check db if exists and replace. Furthermore, same file can be used in many answers
            {
                var imgVirtualPath = System.IO.Path.Combine(combinedDirectoryPath, uploadFile.FileName);
                var uriBuilder = new UriBuilder
                {
                    Host = Request.Host.Host,
                    Scheme = Request.Scheme,
                    Path = imgVirtualPath
                };
                if (Request.Host.Port.HasValue)
                    uriBuilder.Port = Request.Host.Port.Value;
                using (var stream = new System.IO.FileStream(fullPath, System.IO.FileMode.Create))
                {
                    await uploadFile.CopyToAsync(stream);
                    return uriBuilder.Path;
                }
            }

            return "File Not Uploaded";
        }
    }
}

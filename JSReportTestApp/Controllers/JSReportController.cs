using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jsreport.AspNetCore;
using jsreport.Types;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JSReportTestApp.Controllers
{
    [Route("api/[controller]")]
    public class JSReportController : Controller
    {
        private IJsReportMVCService _jsreportService;
        public JSReportController(IJsReportMVCService jsreportService)
        {
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
    }
}

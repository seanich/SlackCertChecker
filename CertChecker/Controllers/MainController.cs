using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CertChecker.Controllers
{
    [Route("")]
    public class MainController : Controller
    {
        private string expectedToken { get; }
        private readonly ILogger<MainController> _logger;
        private readonly ICertificateReader _certificateReader;
        private readonly IConfiguration _configuration;

        public MainController(ILogger<MainController> logger, ICertificateReader certificateReader, IConfiguration configuration)
        {
            _logger = logger;
            _certificateReader = certificateReader;
            expectedToken = configuration.GetValue("SLACK_TOKEN", "faketoken");
        }
        
        // POST / 
        [HttpPost]
        public IActionResult Post(IFormCollection data)
        {
            var clientToken = data["token"];
            if (clientToken != expectedToken) {
                _logger.LogWarning($"Client tried to call service with unrecognized token {clientToken}"); 
                return Unauthorized();
            }

            var commandParts = data["text"].ToString().Split(new []{' ','\t'}, StringSplitOptions.RemoveEmptyEntries);
            var baseCommand = commandParts.FirstOrDefault();

            switch (baseCommand)
            {
                case "add":
                    return Ok("Add a cert check");
                case "remove":
                    return Ok("Remove a cert check");
                case "check":
                    var cert = _certificateReader.GetCertificate(commandParts.Skip(1).FirstOrDefault()).Result;
                    return Ok(cert.ToString());
                case "details":
                    return Ok("Get details about a cert");
                default:
                    return Ok("Whoops");
            }
            
        }
    }
}
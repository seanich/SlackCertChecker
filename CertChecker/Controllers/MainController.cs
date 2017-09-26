using System;
using System.Collections.Generic;
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
        private readonly ILogger<MainController> _logger;
        private readonly ICertificateReader _certificateReader;
        private readonly string _expectedToken;
        private readonly Dictionary<string, Command> commands;

        public MainController(ILogger<MainController> logger, ICertificateReader certificateReader,
            IConfiguration configuration)
        {
            _logger = logger;
            _certificateReader = certificateReader;
            _expectedToken = configuration.GetValue("SLACK_TOKEN", "invalid_token");
            commands = new Dictionary<string, Command>();

            registerCommand("add", "Add a new domain to your monitoring list.", commandAdd, new[] {"domain"});
            registerCommand("remove", "Remove a domain from your monitoring list.", commandRemove, new[] {"domain"});
            registerCommand("check", "Check the certificate expiry date for a domain.", commandCheck, new[] {"domain"});
            registerCommand("details", "Get certificate details for a domain.", commandDetails, new[] {"domain"});
            registerCommand("help", "Display this help message.", commandHelp);
        }

        // POST / 
        [HttpPost]
        public IActionResult Post(IFormCollection data)
        {
            var clientToken = data["token"];

            if (clientToken != _expectedToken)
            {
                _logger.LogWarning($"Client tried to call service with unrecognized token {clientToken}");
                return Unauthorized();
            }

            return handleCommand(new SlackParams(data));
        }

        private void registerCommand(string commandName, string commandDescription,
            Func<SlackParams, IActionResult> commandAction, string[] commandArguments = null)
        {
            commands.Add(commandName, new Command
            {
                Name = commandName,
                Description = commandDescription,
                CommandAction = commandAction,
                Arguments = commandArguments
            });
        }

        private IActionResult handleCommand(SlackParams slackParams)
        {
            return commands.TryGetValue(slackParams.SubCommand, out var command)
                ? command.CommandAction(slackParams)
                : commandHelp(slackParams);
        }

        private IActionResult commandCheck(SlackParams slackParams)
        {
            var host = slackParams.SubCommandParams.FirstOrDefault();
            if (string.IsNullOrEmpty(host))
            {
                return Ok($"Usage: {slackParams.Command} check [domain]");
            }
            var cert = _certificateReader.GetCertificate(host).Result;
            var expiry = DateTime.Parse(cert.GetExpirationDateString());
            var timeUntilExpired = expiry - DateTime.Now;
            return Ok(
                $"Certificate expires in {timeUntilExpired.Days} days {timeUntilExpired.Hours} hours. ({SlackHelper.FormatDate(expiry)})");
        }

        private IActionResult commandDetails(SlackParams slackParams)
        {
            return Ok("This is details!");
        }

        private IActionResult commandAdd(SlackParams slackParams)
        {
            return Ok("This is add!");
        }

        private IActionResult commandRemove(SlackParams slackParams)
        {
            return Ok("This is remove!");
        }

        private IActionResult commandHelp(SlackParams slackParams)
        {
            return Ok(new SlackMessage
            {
                Text = "Usage:",
                Attachments = new List<SlackAttachment>(1)
                {
                    new SlackAttachment
                    {
                        Fallback = "Commands listing.",
                        Fields = commands.Select(command => command.Value)
                            .Select(command =>
                            {
                                var args = command.Arguments != null && command.Arguments.Length > 0
                                    ? string.Join(' ', command.Arguments.Select(arg => $"[{arg}]"))
                                    : "";
                                return new SlackField
                                {
                                    Title = $"{slackParams.Command} {command.Name} {args}",
                                    Value = command.Description,
                                    Short = false
                                };
                            })
                            .ToList()
                    }
                }
            });
        }
    }
}
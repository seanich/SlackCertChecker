using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace CertChecker
{
    public class SlackParams
    {
        public readonly string Token;
        
        public readonly string Command;
        public readonly string SubCommand;
        public readonly string[] SubCommandParams;
        public readonly string SubCommandParamsString;
        public readonly string ResponseUrl;

        public SlackParams(IFormCollection formData)
        {
            var text = formData["text"].ToString();
            var commandTextParts = text.Split(new []{' ','\t'}, StringSplitOptions.RemoveEmptyEntries);
            
            Token = formData["token"];
            Command = formData["command"];
            SubCommand = commandTextParts.Length == 0 ? "" : commandTextParts.FirstOrDefault();
            SubCommandParams = commandTextParts.Skip(1).ToArray();
            SubCommandParamsString = commandTextParts.Length > 1 ? text.Substring(SubCommand.Length).Trim() : "";
            ResponseUrl = formData["reponse_url"];
        }
    }
}
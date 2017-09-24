using System;
using Microsoft.AspNetCore.Mvc;

namespace CertChecker
{
    public struct Command
    {
        public string Name;
        public string Description;
        public Func<SlackParams, IActionResult> CommandAction;
    }
}
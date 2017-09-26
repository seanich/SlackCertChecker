using System.Collections.Generic;

namespace CertChecker
{
    public struct SlackMessage
    {
        public string Text;
        public List<SlackAttachment> Attachments;
    }
}
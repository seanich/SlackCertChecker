using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CertChecker
{
    [DataContract]
    public struct SlackAttachment
    {
        [DataMember]
        public string Fallback;
        [DataMember]
        public string Color;
        [DataMember]
        public string Pretext;
        [DataMember]
        public string AuthorName;
        [DataMember]
        public string AuthorLink;
        [DataMember]
        public string AuthorIcon;
        [DataMember]
        public string Title;
        [DataMember]
        public string TitleLink;
        [DataMember]
        public string Text;
        [DataMember]
        public string ImageUrl;
        [DataMember]
        public string ThumbUrl;
        [DataMember]
        public string Footer;
        [DataMember]
        public string FooterIcon;
        [DataMember(Name="ts", EmitDefaultValue = false)]
        public long Timestamp;
        [DataMember]
        public List<SlackField> Fields;
    }
}
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HIU.Models.Slack
{
    [DataContract(Name = "payload")]
    public class SlackPayload
    {
        [DataMember(Name = "attachments")]
        public List<Attachment> Attachments { get; set; }

        public SlackPayload()
        {
            Attachments = new List<Attachment>();
        }

        public Attachment CreateAttachment(string text, string title)
        {
            return new Attachment(text, title);
        }
    }

    [DataContract]
    public class Attachment
    {
        public Attachment(string text, string title)
        {
            Title = title;
            Text = text;
            Color = "#36a64f";

            Fields = new List<Field>();
        }

        [DataMember(Name = "fallback")]
        public string Fallback { get; set; }

        [DataMember(Name = "color")]
        public string Color { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "fields")]
        public List<Field> Fields { get; set; }

        public void AddField(string title, string text, bool inline)
        {
            Fields.Add(new Field
            {
                Text = text,
                Title = title,
                Inline = inline
            });
        }
    }

    [DataContract]
    public class Field
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "value")]
        public string Text { get; set; }

        [DataMember(Name = "short")]
        public bool Inline { get; set; }
    }

}

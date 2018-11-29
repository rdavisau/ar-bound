using System;

namespace ARKitMeetup.Models
{
    public class DisplayInMenuAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public string DisplayDescription { get; set; }
    }
}

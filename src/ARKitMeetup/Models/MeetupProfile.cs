using System;

namespace ARKitMeetup.Models
{
    public class MeetupProfile
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Bio { get; set; }
        public long Joined { get; set; }
        public DateTimeOffset JoinedOn => DateTimeOffset.FromUnixTimeMilliseconds(Joined);
        public string Status { get; set; }
        public int TopicCount { get; set; }
        public int TotalRsvps { get; set; }
        public int YesRsvps { get; set; }
        public int NoRsvps { get; set; }
        public MeetupPhotoUrl Photo { get; set; }
        public bool IsGhost { get; internal set; }
    }

    public class MeetupPhotoUrl
    {
        public string detect_link { get; set; }
        public string display_link { get; set; }
    }
}

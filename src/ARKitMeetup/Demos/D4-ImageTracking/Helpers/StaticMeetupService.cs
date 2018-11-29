using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ARKitMeetup.Helpers;
using ARKitMeetup.Models;
using Newtonsoft.Json;

namespace ARKitMeetup.Demos.Helpers
{
    public static class StaticMeetupService
    {
        private static HttpClient _httpClient = new HttpClient();

        // for the demo, this used the meetup api to get profiles of the rsvps
        // but i've replaced that with hard coded entries (profiles.json)
        // this also required having different images for detection and display which has 
        // made the code a bit uglier because i hacked that in quickly
        public static async Task<List<ImageDetectionReferenceItem<MeetupProfile>>> GetRsvps()
        {
            var profileJson = File.ReadAllText("profiles.json");
            var profiles = JsonConvert.DeserializeObject<List<MeetupProfile>>(profileJson);
            var refs = await profiles.SelectToListAsync(x => GetItemOrNull(x));

            return refs;
        }

        private static async Task<ImageDetectionReferenceItem<MeetupProfile>> GetItemOrNull(MeetupProfile profile)
        {
            var detectUrl = profile.Photo.detect_link;
            var displayUrl = profile.Photo.display_link ?? detectUrl;
            
            if (detectUrl == null)
                return null;

            byte[] imgData = null;
            byte[] displayData = null;
            
            try 
            { 
                imgData = await _httpClient.GetByteArrayAsync(detectUrl);
                displayData = await _httpClient.GetByteArrayAsync(displayUrl);
            }
            catch (Exception ex) { Debug.WriteLine(ex); return null; }

            return new ImageDetectionReferenceItem<MeetupProfile>
            {
                ItemData = profile,
                ImageData = imgData,
                DisplayData = displayData,
                RealWorldSizeCms = 6.6f
            };
        }
    }
}

namespace ARKitMeetup.Demos.Helpers
{
    public class ImageDetectionReferenceItem<T>
    {
        public T ItemData { get; set; }
        public byte[] ImageData { get; set; } 
        public byte[] DisplayData { get; set; }
        public float RealWorldSizeCms { get; set; }
    }
}

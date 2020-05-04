namespace Aspose.Cloud.Marketplace.Report.Model { 
    public class Location
    {
        public double? LLX { get; set; }
        public double? LLY { get; set; }
        public double? URX { get; set; }
        public double? URY { get; set; }

        public double? Top { get; set; }
        public double? Left { get; set; }
        public double? Bottom { get; set; }
        public double? Right { get; set; }

        public bool EmptyLow()
        {
            return LLX == null || LLY == null || URX == null || URY == null;
        }
        public bool EmptyTop()
        {
            return Top == null || Left == null || Bottom == null || Right == null;
        }

        public bool Empty()
        {
            return EmptyLow() && EmptyTop();
        }
    }
}

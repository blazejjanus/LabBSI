namespace CodingComparison {
    internal class Result {
        public string File {  get; set; }
        public string Algorithm { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public int MemoryUsed { get; set; }
        public int InputSize { get; set; }
        public int OutputSize { get; set; }
        public double CompressionRatio => InputSize / OutputSize;
    }
}

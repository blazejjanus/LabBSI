namespace CodingComparison {
    internal class CharStats {
        public char Character { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }

        public CharStats(char c) {
            Character = c;
            Count = 1;
            Percentage = 0;
        }

        public void CalcPercentage(int totalCount) {
            Percentage = Math.Round((double)Count / totalCount, 5);
        }

        public override string ToString() {
            return $"\'{Character}\': {Count} ({Percentage}%)";
        }
    }
}

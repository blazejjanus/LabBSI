namespace CodingComparison {
    internal static class FileStats { 
        public static List<CharStats> GetCharStats(string content) {
            var result = new List<CharStats>();
            foreach (var c in content) {
                if(result.Any(x => x.Character == c)) {
                    result.First(x => x.Character == c).Count++;
                } else {
                    result.Add(new CharStats(c));
                }
            }
            foreach(var c in result) {
                c.CalcPercentage(content.Length);
            }
            return result.OrderByDescending(x => x.Count).ToList();
        }
    }
}

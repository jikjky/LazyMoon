namespace LazyMoon.Model
{
    public class User
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int? TTSId { get; set; }
        public TTS? TTS { get; set; }
        public int? ValorantRankId { get; set; }
        public ValorantRank? ValorantRank { get; set; }
    }
}

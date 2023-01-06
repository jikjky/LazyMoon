namespace LazyMoon.Model
{
    public class User
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public int? TTSId { get; set; }
        public TTS TTS { get; set; }
        public int? ValorantRankId { get; set; }
        public ValorantRank ValorantRank { get; set; }
    }
}

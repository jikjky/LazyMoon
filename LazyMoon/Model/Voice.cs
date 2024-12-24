namespace LazyMoon.Model
{
    public class Voice
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public EVoice VoiceMode { get; set; } = EVoice.A;
        public double Pitch { get; set; } = 0;
        public bool Use { get; set; } = true;
    }
    public enum EVoice
    {
        A = 1,
        B = 2,
        C = 3,
        D = 4,
    }
}

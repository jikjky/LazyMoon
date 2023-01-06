using System.Collections.Generic;

namespace LazyMoon.Model
{
    public class TTS
    {
        public int Id { get; set; }
        public double Rate { get; set; } = 1;
        public double Volume { get; set; } = 0;
        public bool TTSEnable { get; set; } = true;
        public ICollection<Voice> Voices { get; set; }
    }
}

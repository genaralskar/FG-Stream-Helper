namespace FG_Stream_Helper
{
    public class SmashGGInfoModel
    {
        public Set set { get; set; }
    }

    public class Set
    {
        public string id { get; set; }
        public SetSlot[] slots { get; set; }
        public string fullRoundText { get; set; }
    }

    public class SetSlot
    {
        public Entrant entrant { get; set; }
        public Standing standing;
    }

    public class Standing
    {
        public StandingStats stats;
    }

    public class StandingStats
    {
        public Score score;
    }

    public class Score
    {
        public float value;
    }

    public class Entrant
    {
        public string name { get; set; }
    }
}

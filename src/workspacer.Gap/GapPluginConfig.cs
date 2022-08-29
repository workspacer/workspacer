namespace workspacer.Gap
{
    public class GapPluginConfig
    {
        public int InnerGap { get; set; } = 0;
        public int OuterGap { get; set; } = 0;
        public int Delta { get; set; } = 0;

        public GapPluginConfig()
        {

        }

        public GapPluginConfig(int innerGap, int outerGap, int delta)
        {
            InnerGap = innerGap;
            OuterGap = outerGap;
            Delta = delta;
        }
    }
}
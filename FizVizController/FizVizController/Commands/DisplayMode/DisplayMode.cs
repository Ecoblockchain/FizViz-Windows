namespace FizVizController.Commands
{
    internal abstract class DisplayMode : FizVizCommand
    {
        public enum DisplayModeValue
        {
            HotNeedle = 0,
            BlockNeedle = 1,
            BackgroundRotate = 2,
            MinMax = 3
        };


        /****************************************************************
         *                  Public Properties                           *
         ****************************************************************/
        public byte Mode => mode;

        /****************************************************************
         *                  Data Access Methods                         *
         ****************************************************************/

        public override byte Command => 0x40;

        /****************************************************************
         *                  Data                                        *
         ****************************************************************/
        protected byte mode;
    }
}

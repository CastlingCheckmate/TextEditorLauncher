namespace WPF.UI.Converters
{

    public static class ConvertersHost
    {

        static ConvertersHost()
        {
            BoolToInvertedBoolConverter = new BoolToInvertedBoolConverter();
        }

        public static BoolToInvertedBoolConverter BoolToInvertedBoolConverter
        {
            get;

            private set;
        }

    }

}
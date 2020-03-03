namespace WPF.UI.Converters
{

    // класс, содержащий ссылки на используемые в XAML конвертеры
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
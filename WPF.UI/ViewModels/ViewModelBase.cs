using System.ComponentModel;

namespace WPF.UI.ViewModels
{

    // базовый класс для вьюмоделей
    // реализуем INotifyPropertyChanged и все вьюмодели наследуем от этого класса
    public class ViewModelBase : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        // проходим по набору переданных строк с именами свойств и для каждой дёргаем событие PropertyChanged, сообщающее вьюхе об изменениях в DataContext (коим и является текущая вьюмодель)
        protected void NotifyPropertyChanged(params string[] propertiesNames)
        {
            foreach (var propertyName in propertiesNames)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

}
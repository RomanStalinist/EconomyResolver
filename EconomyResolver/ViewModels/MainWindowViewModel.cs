using EconomyResolver.BusinessLogic;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Linq;

namespace EconomyResolver.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        public ObservableCollection<KeyValuePair<string, Показатель>> Данные { get; set; }
        public ObservableCollection<KeyValuePair<string, Показатель>> Результаты { get; set; }

        public ICommand ВыполнитьРасчетКоманда { get; }
        public ICommand СброситьРезультатыКоманда { get; }
        public ICommand ИмпортироватьДанныеКоманда { get; }
        public ICommand ЭкспортироватьДанныеКоманда { get; }
        public ICommand ЭкспортироватьРезультатыКоманда { get; }

        public MainWindowViewModel()
        {
            Данные = new ObservableCollection<KeyValuePair<string, Показатель>>(Показатель.Показатели.ПолучитьСловарь());
            Результаты = new ObservableCollection<KeyValuePair<string, Показатель>>();
            ВыполнитьРасчетКоманда = new RelayCommand(ВыполнитьРасчет);
            СброситьРезультатыКоманда = new RelayCommand(СброситьРезультаты);
            ИмпортироватьДанныеКоманда = new RelayCommand(ИмпортироватьДанные);
            ЭкспортироватьДанныеКоманда = new RelayCommand(ЭкспортироватьДанные);
            ЭкспортироватьРезультатыКоманда = new RelayCommand(ЭкспортироватьРезультаты);
        }

        private void ВыполнитьРасчет()
        {
            try
            {
                var результаты = Калькулятор.ВыполнитьРасчет(НаборПоказателей.Новый(Данные));
                Результаты.Clear();
                foreach (var результат in результаты.ПолучитьСловарь())
                    Результаты.Add(результат);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void СброситьРезультаты() => Результаты.Clear();

        private void ИмпортироватьДанные()
        {
            try
            {
                var результаты = МенеджерИмпорта.Импортировать();
                if (результаты == null)
                    return;

                Данные.Clear();
                foreach (var результат in результаты.ToDictionary(показатель => показатель.Наименование, показатель => показатель))
                    Данные.Add(результат);

                MessageBox.Show($"Импорт успешен!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ЭкспортироватьДанные()
        {
            try
            {
                if (!Данные.Any())
                    throw new Exception("Данные отсутствуют.");

                var менеджерЭкспорта = new МенеджерЭкспорта(Данные.Select(пара => пара.Value));

                if (менеджерЭкспорта.Экспортировать())
                    MessageBox.Show("Экспорт успешен!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ЭкспортироватьРезультаты()
        {
            try
            {
                if (!Результаты.Any())
                    throw new Exception("Результаты отсутствуют.");

                var менеджерЭкспорта = new МенеджерЭкспорта(Результаты.Select(пара => пара.Value));

                if (менеджерЭкспорта.Экспортировать())
                    MessageBox.Show("Экспорт успешен!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
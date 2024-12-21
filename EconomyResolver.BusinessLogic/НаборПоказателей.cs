using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace EconomyResolver.BusinessLogic
{
    public class НаборПоказателей
    {
        private readonly Dictionary<string, Показатель> _показатели = new Dictionary<string, Показатель>();

        public static НаборПоказателей Новый() => new НаборПоказателей();
        public static НаборПоказателей Новый(ICollection<KeyValuePair<string, Показатель>> Коллекция) => new НаборПоказателей(Коллекция);

        public НаборПоказателей() { }
        public НаборПоказателей(ICollection<KeyValuePair<string, Показатель>> Коллекция) => _показатели = Коллекция.ToDictionary(пара => пара.Key, пара => пара.Value);

        // Регулярное выражение для проверки ключей
        private static readonly Regex Кириллица =
            new Regex(@"^[А-Яа-яЁё]+$", RegexOptions.Compiled);

        public void Добавить(string ключ, Показатель показатель)
        {
            if (string.IsNullOrWhiteSpace(ключ))
                throw new ArgumentException("Ключ не может быть пустым или содержать пробелы.");

            if (!Кириллица.IsMatch(ключ))
                throw new ArgumentException("Ключ должен содержать только буквы кириллицы.");

            if (_показатели.ContainsKey(ключ))
                throw new ArgumentException($"Показатель с ключом '{ключ}' уже существует.");

            _показатели[ключ] = показатель;
        }

        public Показатель Получить(string ключ)
        {
            if (_показатели.TryGetValue(ключ, out var показатель))
                return показатель;

            throw new KeyNotFoundException($"Показатель с ключом '{ключ}' не найден.");
        }

        public bool Содержит(string ключ) => _показатели.ContainsKey(ключ);

        public IEnumerable<KeyValuePair<string, Показатель>> ВсеПары() => _показатели;

        public IEnumerable<string> ВсеКлючи() => _показатели.Keys;

        public IEnumerable<Показатель> ВсеПоказатели() => _показатели.Values;

        public Dictionary<string, Показатель> ПолучитьСловарь() => _показатели;

        public void Удалить(string ключ)
        {
            if (!_показатели.Remove(ключ))
                throw new KeyNotFoundException($"Показатель с ключом '{ключ}' не найден.");
        }

        public int Количество => _показатели.Count;

        public Показатель this[int индекс]
        {
            get
            {
                if (индекс < 0 || индекс >= Количество)
                    throw new IndexOutOfRangeException("Индекс вне диапазона.");

                return _показатели.ElementAt(индекс).Value;
            }
            set
            {
                if (индекс < 0 || индекс >= Количество)
                    throw new IndexOutOfRangeException("Индекс вне диапазона.");

                var ключ = _показатели.ElementAt(индекс).Key;
                _показатели[ключ] = value;
            }
        }

        public Показатель this[string ключ]
        {
            get => _показатели[ключ];
            set => _показатели[ключ] = value;
        }

        public override string ToString() => string.Join(Environment.NewLine, ВсеПары()
            .Select(пара => $"{пара.Key} = {пара.Value.ToShortString()}"));

        public override int GetHashCode()
        {
            var hash = 17;
            foreach (var kvp in _показатели)
            {
                hash = hash * 31 + (kvp.Key?.GetHashCode() ?? 0);
                hash = hash * 31 + (kvp.Value?.GetHashCode() ?? 0);
            }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is НаборПоказателей другойНабор)
            {
                // Сравниваем количество показателей
                if (Количество != другойНабор.Количество)
                    return false;

                // Сравниваем каждую пару ключ-значение
                return !ВсеПары().Except(другойНабор.ВсеПары()).Any();
            }
            return false;
        }
    }
}

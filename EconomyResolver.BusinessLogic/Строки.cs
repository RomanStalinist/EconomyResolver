using System.Linq;

namespace EconomyResolver.BusinessLogic
{
    public static class Строки
    {
        /// <summary>
        /// Преобразует строку "Пример строки" в "ПримерСтроки".
        /// </summary>
        /// <param name="input">Входная строка.</param>
        /// <returns>Преобразованная строка.</returns>
        public static string ПривестиКСлитномуФормату(this string input)
        {
            input = input.Trim();
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Убираем пробелы и делаем каждое слово с заглавной буквы
            var words = input.Split(' ');
            return string.Concat(words.Select(word => char.ToUpper(word[0]) + word.Substring(1).Trim()));
        }

        /// <summary>
        /// Преобразует строку "ПримерСтроки" в "Пример строки".
        /// </summary>
        /// <param name="input">Входная строка.</param>
        /// <returns>Преобразованная строка.</returns>
        public static string ПривестиКРазделённомуФормату(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var result = string.Empty;
            foreach (var c in input)
                result +=  $"{(char.IsUpper(c) && result.Length > 0 ? " " : string.Empty)}{c}";

            return result;
        }
    }
}

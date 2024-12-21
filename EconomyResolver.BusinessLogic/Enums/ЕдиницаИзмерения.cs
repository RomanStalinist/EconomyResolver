using System.ComponentModel;

namespace EconomyResolver.BusinessLogic.Enums
{
    public enum ЕдиницаИзмерения
    {
        [Description("шт.")]
        Штуки,
        
        [Description("руб.")]
        Рубли,
        
        [Description("руб./ч.")]
        РублиВЧас,
        
        [Description("%")]
        Проценты,
        
        [Description("чел.")]
        Люди,
        
        [Description("мин.")]
        Минуты,
        
        [Description("ч.")]
        Часы,
        
        [Description("дн.")]
        Дни,
        
        [Description("нед.")]
        Недели,
        
        [Description("мес.")]
        Месяцы,
        
        [Description("г.")]
        Года
    }

    public static class UnitExtension
    {
        public static string ПолучитьОписание(this ЕдиницаИзмерения unit)
        {
            // Получаем тип перечисления
            var fieldInfo = unit.GetType().GetField(unit.ToString());
            
            // Извлекаем атрибут Description
            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            
            // Возвращаем значение атрибута, если он существует
            return attributes.Length > 0 ? attributes[0].Description : unit.ToString();
        }
    }
}
using OfficeOpenXml;

namespace EconomyResolver.BusinessLogic
{
    public class МенеджерЛицензий
    {
        public static void УстановитьЛицензию(LicenseContext context = LicenseContext.NonCommercial)
            => ExcelPackage.LicenseContext = context;
    }
}

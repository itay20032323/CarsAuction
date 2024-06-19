using System.Reflection;

namespace AuctionService.Helpers;

public class ConsoleHelper
{
    public static void WriteLinesObject(object obj)
    {
        Type type = obj.GetType();
        PropertyInfo[] properties = type.GetProperties();

        foreach (PropertyInfo property in properties)
        {
            string propertyName = property.Name;
            object propertyValue = property.GetValue(obj);

            Console.WriteLine($"{propertyName}: {propertyValue}");
        }
    }
}

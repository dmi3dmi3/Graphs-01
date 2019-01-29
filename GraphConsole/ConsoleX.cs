using System;

namespace GraphConsole
{
    public class ConsoleX
    {
        public static void Write(string data, ConsoleColor color)
        {
            var backColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(data);
            Console.ForegroundColor = backColor;
        }

        public static void WriteLine(string data, ConsoleColor color)
        {
            Write(data + Environment.NewLine, color);
        }

        public static void PrintValue(string name, object item)
        {
            Console.Write($"{name}:");
            switch (item)
            {
                case bool boolValue:
                    WriteLine(boolValue.ToString(), boolValue ? ConsoleColor.Green : ConsoleColor.Red);
                    break;
                case string stringValue:
                    WriteLine(stringValue, ConsoleColor.Cyan);
                    break;
                case int intValue:
                    WriteLine(intValue.ToString(), ConsoleColor.Yellow);
                    break;
                case float intValue:
                    WriteLine(intValue.ToString(), ConsoleColor.Yellow);
                    break;
                case decimal intValue:
                    WriteLine(intValue.ToString(), ConsoleColor.Yellow);
                    break;
                case long intValue:
                    WriteLine(intValue.ToString(), ConsoleColor.Yellow);
                    break;
                case Enum enumValue:
                    WriteLine(enumValue.ToString(), ConsoleColor.Magenta);
                    break;
                case null:
                    WriteLine("NULL", ConsoleColor.Red);
                    break;
                default:
                    WriteLine(item.ToString(), ConsoleColor.White);
                    break;
            }
        }

        public static void PrintBytes(string name, long bytes)
        {
            var kbytes = bytes / 1024D;

            var mbytes = bytes / (1024D * 1024D);


            Console.Write($"{name}:");
            Write(bytes.ToString(), ConsoleColor.Cyan);
            Write("b | ", ConsoleColor.Gray);
            Write(kbytes.ToString("F"), ConsoleColor.Cyan);
            Write("kb | ", ConsoleColor.Gray);
            Write(mbytes.ToString("F"), ConsoleColor.Cyan);
            WriteLine("mb | ", ConsoleColor.Gray);
        }

        public static TItem GetValue<TItem>(string name)
        {
            while (true)
            {
                Console.Write($"{name}: ");
                var stringItem = Console.ReadLine();
                var itemType = typeof(TItem);

                if (itemType == typeof(int))
                    if (int.TryParse(stringItem, out var intResult))
                        return (TItem) Convert.ChangeType(intResult, typeof(TItem));

                if (itemType == typeof(long))
                    if (long.TryParse(stringItem, out var intResult))
                        return (TItem) Convert.ChangeType(intResult, typeof(TItem));
                if (itemType == typeof(float))
                    if (float.TryParse(stringItem, out var intResult))
                        return (TItem) Convert.ChangeType(intResult, typeof(TItem));
                if (itemType == typeof(double))
                    if (double.TryParse(stringItem, out var intResult))
                        return (TItem) Convert.ChangeType(intResult, typeof(TItem));
                if (itemType == typeof(string))
                    return (TItem) Convert.ChangeType(stringItem, typeof(TItem));

                Console.Write("Invalid data, try again");
                WriteLine($"({typeof(TItem)}).", ConsoleColor.DarkGray);
            }
        }

        public static TItem GetValue<TItem>(string name, TItem defaultValue)
        {
            while (true)
            {
                Console.Write($"{name}[{defaultValue}]: ");
                var stringItem = Console.ReadLine();
                if (string.IsNullOrEmpty(stringItem))
                    return defaultValue;

                var itemType = typeof(TItem);

                if (itemType == typeof(int))
                    if (int.TryParse(stringItem, out var intResult))
                        return (TItem) Convert.ChangeType(intResult, typeof(TItem));

                if (itemType == typeof(long))
                    if (long.TryParse(stringItem, out var intResult))
                        return (TItem) Convert.ChangeType(intResult, typeof(TItem));

                if (itemType == typeof(float))
                    if (float.TryParse(stringItem, out var intResult))
                        return (TItem) Convert.ChangeType(intResult, typeof(TItem));

                if (itemType == typeof(double))
                    if (double.TryParse(stringItem, out var intResult))
                        return (TItem) Convert.ChangeType(intResult, typeof(TItem));

                if (itemType == typeof(DateTime))
                    if (DateTime.TryParse(stringItem, out var intResult))
                        return (TItem) Convert.ChangeType(intResult, typeof(TItem));

                if (itemType == typeof(Enum))
                    try
                    {
                        var enumValue = Enum.Parse(typeof(TItem), stringItem, true);
                        return (TItem) Convert.ChangeType(enumValue, typeof(TItem));
                    }
                    catch (Exception e)
                    {
                    }


                if (itemType == typeof(string))
                    return (TItem) Convert.ChangeType(stringItem, typeof(TItem));
            }
        }
    }
}
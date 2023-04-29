using System;

namespace TelegramBot
{
    partial class Program
    {
        static void Main(string[] args)
        {
            // Подготовка базы данных
            Database.CreateDatabase();

            // Запуск телеграм-бота
            TelegramBot.StartBot();

            Console.ReadLine();
        }
    }
}
using System;
using System.Text.RegularExpressions;
using Google.Cloud.Translation.V2;

namespace TelegramBot
{
    // Автоматическое определение языка сообщения и динамический перевод сообщения с использованием Cloud Translation API
    class CloudTranslationAPI
    {
        // Ключ Cloud Translation API
        private static string apiKey = "AIzaSyD0rD6Py4t4lzU3AYXV-JR99TECMTPZJow";

        // Клиент Translation с использованием ключа API
        static TranslationClient translationClient = TranslationClient.CreateFromApiKey(apiKey);

        // Перевод сообщения
        public static string Translation(string message)
        {
            string detectedLanguage = DetectionLanguage(message);
            string translatedText;

            if (Regex.IsMatch(message, @"[\u0400-\u04FF]"))
            {
                Console.WriteLine("Текст сообщения содержит кириллицу");
                translatedText = translationClient.TranslateText(message, LanguageCodes.English, LanguageCodes.Russian).TranslatedText;
            }
            else
            {
                Console.WriteLine("Текст сообщения не содержит кириллицу");
                translatedText = translationClient.TranslateText(message, LanguageCodes.Russian, LanguageCodes.English).TranslatedText;
            }

            Console.WriteLine($"Перевод сообщения: {translatedText}");
            Console.WriteLine();
            return translatedText;
        }

        // Определение языка сообщения
        private static string DetectionLanguage(string message)
        {
            var detected = translationClient.DetectLanguage(message);
            Console.WriteLine($"Язык сообщения: {detected.Language}");
            return detected.Language;
        }
    }
}
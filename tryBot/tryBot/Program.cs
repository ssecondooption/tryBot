using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

internal class Program
{
    private static async Task Main(string[] args)
    {
        {
            var botClient = new TelegramBotClient("6199273547:AAF3Hz48FDz4ZtYvDa15TJl6NCFvXnVpBz0");

            using CancellationTokenSource cts = new();
            var me = await botClient.GetMeAsync();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            Console.ReadLine();

            cts.Cancel();
        }

        async static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            if (update.Message is not { } message)
            {
                return;
            }
            if (message.Text is not { } messageText)
            {
                return;
            }
            var chatId = message.Chat.Id;

            Console.WriteLine($"{message.Chat.FirstName} = {messageText}");

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Помощь", "Найти фильм" },
                new KeyboardButton[] { "Рандомный фильм", "Жанры"},
            })
            {
                ResizeKeyboard = true
            };

            if (messageText.ToLower().Contains("привет") || messageText.ToLower().Contains("/start"))
            {
                Message sendMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Привет!",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: token);

                return;

            }
            if (messageText.ToLower().Contains("помощь"))
            {
                Message sendMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Привет, я бот, который поможет найти тебе любой фильм!",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: token);

                return;
            }
            if (messageText.ToLower().Contains("рандомный фильм"))
            {
                Message sendMessage = await botClient.SendPhotoAsync(
                    chatId: chatId,
                    photo: InputFile.FromUri("https://static.hdrezka.ac/i/2020/10/27/hf91d7db92363zu25r74p.png"),
                    caption: "<b>Волейбол</b>. <a href=\"https://jut.su/haaikyu/\">Смотреть</a>",
                    parseMode: ParseMode.Html,
                    cancellationToken: token);

                return;
            }
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

    }
}
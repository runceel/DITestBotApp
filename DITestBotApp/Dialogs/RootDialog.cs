using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using DITestBotApp.Services;

namespace DITestBotApp.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private IGreetService GreetService { get; }

        // DIする
        public RootDialog(IGreetService greetService)
        {
            this.GreetService = greetService;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.GreetAsync);

            return Task.CompletedTask;
        }

        private async Task GreetAsync(IDialogContext context, IAwaitable<object> result)
        {
            // DIしたやつを使う
            await context.PostAsync(this.GreetService.GetMessage());
            await this.MessageReceivedAsync(context, result);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            context.Wait(MessageReceivedAsync);
        }
    }
}
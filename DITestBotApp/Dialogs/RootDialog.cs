using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using DITestBotApp.Services;
using Autofac;
using DITestBotApp.Factories;

namespace DITestBotApp.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private IGreetService GreetService { get; }
        private IDialogFactory Factory { get; }

        // DIする
        public RootDialog(IDialogFactory factory, IGreetService greetService)
        {
            this.Factory = factory;
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

            if (activity.Text == "change")
            {
                context.Call(this.Factory.Create<SimpleDialog>(), this.ResumeSimpleDialogAsync);
            }
            else
            {
                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                // return our reply to the user
                await context.PostAsync($"You sent {activity.Text} which was {length} characters");

                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ResumeSimpleDialogAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("returned");
            context.Wait(this.MessageReceivedAsync);
        }
    }
}
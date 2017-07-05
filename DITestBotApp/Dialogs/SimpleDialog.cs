using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace DITestBotApp.Dialogs
{
    [Serializable]
    public class SimpleDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("SimpleDialog started");
            context.Wait(this.HelloWorldAsync);
        }

        private async Task HelloWorldAsync(IDialogContext context, IAwaitable<object> result)
        {
            var input = await result as Activity;
            await context.PostAsync($"Hello world!! {input.Text}");
            context.Done<object>(null);
        }
    }
}
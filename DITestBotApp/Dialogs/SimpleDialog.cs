using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace DITestBotApp.Dialogs
{
    public interface ISimpleDialog : IDialog<object>
    {
        // こいつもテストするならHelloWorldAsyncとか定義することになるかな
    }

    [Serializable]
    public class SimpleDialog : ISimpleDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("SimpleDialog started");
            PromptDialog.Choice(
                context,
                this.HelloWorldAsync,
                new[]
                {
                    "A", "B", "C",
                },
                "あなたの好きなのは？？");
        }

        public async Task HelloWorldAsync(IDialogContext context, IAwaitable<string> result)
        {
            var input = await result;
            await context.PostAsync($"Hello world!! {input}");
            context.Done<object>(null);
        }
    }
}
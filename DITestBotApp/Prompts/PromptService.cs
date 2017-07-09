using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;

namespace DITestBotApp.Prompts
{
    public interface IPromptService
    {
        void Choice<T>(IDialogContext context, ResumeAfter<T> resume, IEnumerable<T> options, string prompt, string retry = null, int attempts = 3, PromptStyle promptStyle = PromptStyle.Auto, IEnumerable<string> descriptions = null);
    }

    public class PromptService : IPromptService
    {
        public void Choice<T>(IDialogContext context, ResumeAfter<T> resume, IEnumerable<T> options, string prompt, string retry = null, int attempts = 3, PromptStyle promptStyle = PromptStyle.Auto, IEnumerable<string> descriptions = null)
        {
            PromptDialog.Choice<T>(context, resume, options, prompt, retry, attempts, promptStyle, descriptions);
        }
    }
}
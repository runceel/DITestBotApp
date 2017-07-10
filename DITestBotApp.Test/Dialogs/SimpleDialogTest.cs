using DITestBotApp.Dialogs;
using DITestBotApp.Prompts;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DITestBotApp.Test.Dialogs
{
    [TestClass]
    public class SimpleDialogTest
    {
        [TestMethod]
        public async Task StartAsyncTest()
        {
            var promptServiceMock = new Mock<IPromptService>();
            promptServiceMock
                .Setup(x => x.Choice(It.IsNotNull<IDialogContext>(),
                    It.Is<ResumeAfter<string>>(y => y.Method.Name == nameof(SimpleDialog.HelloWorldAsync)),
                    It.Is<IEnumerable<string>>(y => y.ElementAtOrDefault(0) == "A" &&
                        y.ElementAtOrDefault(1) == "B" &&
                        y.ElementAtOrDefault(2) == "C"),
                    "あなたの好きなのは？？",
                    null,
                    3,
                    PromptStyle.Auto,
                    null))
                    .Verifiable();

            var dialogContextMock = new Mock<IDialogContext>();
            dialogContextMock
                .Setup(x => x.MakeMessage())
                .Returns(Activity.CreateMessageActivity());

            dialogContextMock
                .Setup(x => x.PostAsync(It.Is<IMessageActivity>(y => y.Text == "SimpleDialog started"), default(CancellationToken)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var target = new SimpleDialog(promptServiceMock.Object);
            await target.StartAsync(dialogContextMock.Object);

            promptServiceMock.Verify();
            dialogContextMock.Verify();
        }
    }
}

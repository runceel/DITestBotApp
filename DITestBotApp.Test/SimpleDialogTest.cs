﻿using DITestBotApp.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Fakes;
using Microsoft.Bot.Connector;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DITestBotApp.Test
{
    [TestClass]
    public class SimpleDialogTest
    {
        [TestMethod]
        public async Task StartAsyncTest_Fakes()
        {
            using (ShimsContext.Create())
            {
                var choiceCalled = false;
                ShimPromptDialog.ChoiceOf1IDialogContextResumeAfterOfM0IEnumerableOfM0StringStringInt32PromptStyleIEnumerableOfString<string>(
                    (context, resumeAfter, chioce, primpt, _, __, ___, ____) =>
                    {
                        Assert.AreEqual("A", chioce.ElementAtOrDefault(0));
                        Assert.AreEqual("B", chioce.ElementAtOrDefault(1));
                        Assert.AreEqual("C", chioce.ElementAtOrDefault(2));
                        Assert.AreEqual("あなたの好きなのは？？", primpt);
                        Assert.AreEqual(nameof(SimpleDialog.HelloWorldAsync), resumeAfter.Method.Name);
                        choiceCalled = true;
                    });

                var dialogContextMock = new Mock<IDialogContext>();
                dialogContextMock.Setup(x => x.PostAsync(It.Is<IMessageActivity>(y => y.Text == "SimpleDialog started"), default(CancellationToken)))
                    .Returns(Task.CompletedTask)
                    .Verifiable();
                dialogContextMock.Setup(x => x.MakeMessage()).Returns(Activity.CreateMessageActivity());
                var target = new SimpleDialog();
                await target.StartAsync(dialogContextMock.Object);

                Assert.IsTrue(choiceCalled);
            }
        }
    }
}

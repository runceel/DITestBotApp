using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DITestBotApp.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Moq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using DITestBotApp.Services;
using DITestBotApp.Factories;
using System.Threading;

namespace DITestBotApp.Test
{
    [TestClass]
    public class RootDialogTest
    {
        [TestMethod]
        public async Task StartAsyncTest()
        {
            var target = new RootDialog(null, null);

            var dialogContextMock = new Mock<IDialogContext>();
            dialogContextMock.Setup(x => x.Wait(It.Is<ResumeAfter<IMessageActivity>>(y => y.Method.Name == nameof(RootDialog.GreetInteractionAsync))))
                .Verifiable();

            await target.StartAsync(dialogContextMock.Object);
            dialogContextMock.Verify();
        }

        [TestMethod]
        public async Task GreetInteractionAsyncTest()
        {
            var dialogFactoryMock = new Mock<IDialogFactory>();
            var greetServiceMock = new Mock<IGreetService>();
            greetServiceMock.Setup(x => x.GetMessage())
                .Returns("Hello world")
                .Verifiable();

            var dialogContextMock = new Mock<IDialogContext>();
            dialogContextMock.Setup(x => x.MakeMessage())
                .Returns(Activity.CreateMessageActivity());
            dialogContextMock.Setup(x => x.PostAsync(It.Is<IMessageActivity>(y => y.Text == "Hello world"), default(CancellationToken)))
                .Returns(Task.CompletedTask)
                .Verifiable();
            dialogContextMock.Setup(x => x.PostAsync(It.Is<IMessageActivity>(y => y.Text == "You sent okazuki which was 7 characters"), default(CancellationToken)))
                .Returns(Task.CompletedTask)
                .Verifiable();
            var resultMock = Awaitable.FromItem<object>(new Activity { Text = "okazuki" });

            var target = new RootDialog(dialogFactoryMock.Object, greetServiceMock.Object);
            await target.GreetInteractionAsync(dialogContextMock.Object, resultMock);

            greetServiceMock.Verify();
            dialogContextMock.Verify();
        }

        [TestMethod]
        public async Task MainInteractionAsyncChangeCase()
        {
            var dialogFactoryMock = new Mock<IDialogFactory>();
            var simpleDialogMock = new Mock<ISimpleDialog>();
            dialogFactoryMock.Setup(x => x.Create<ISimpleDialog>())
                .Returns(simpleDialogMock.Object)
                .Verifiable();

            var dialogContextMock = new Mock<IDialogContext>();
            dialogContextMock.Setup(x => x.Call(It.Is<ISimpleDialog>(y => y == simpleDialogMock.Object),
                It.Is<ResumeAfter<object>>(y => y.Method.Name == nameof(RootDialog.ReturnFromSimpleDialogInteractionAsync))))
                .Verifiable();

            var resultMock = Awaitable.FromItem<object>(new Activity { Text = "change" });

            var target = new RootDialog(dialogFactoryMock.Object, null);
            await target.MainInteractionAsync(dialogContextMock.Object, resultMock);

            dialogFactoryMock.Verify();
            dialogContextMock.Verify();
        }

        [TestMethod]
        public async Task ReturnFromSimpleDialogInteractionAsync()
        {
            var dialogContextMock = new Mock<IDialogContext>();
            dialogContextMock.Setup(x => x.PostAsync(It.Is<IMessageActivity>(y => y.Text == "returned"), default(CancellationToken)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            dialogContextMock.Setup(x => x.MakeMessage())
                .Returns(Activity.CreateMessageActivity());
            dialogContextMock.Setup(x => x.Wait(It.Is<ResumeAfter<IMessageActivity>>(y => y.Method.Name == nameof(RootDialog.MainInteractionAsync))))
                .Verifiable();

            var target = new RootDialog(null, null);
            await target.ReturnFromSimpleDialogInteractionAsync(dialogContextMock.Object, null);

            dialogContextMock.Verify();
        }
    }
}

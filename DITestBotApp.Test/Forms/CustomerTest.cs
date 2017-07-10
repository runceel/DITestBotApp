using DITestBotApp.Forms;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DITestBotApp.Test.Forms
{
    [TestClass]
    public class CustomerTest
    {
        [TestMethod]
        public async Task BuildFormTest()
        {
            var dialogContextMock = new Mock<IDialogContext>();
            // 内部で呼ばれるメソッドの下準備
            dialogContextMock.Setup(x => x.MakeMessage())
                .Returns(Activity.CreateMessageActivity());
            dialogContextMock.Setup(x => x.Wait(It.IsAny<ResumeAfter<IMessageActivity>>()));

            // テスト対象のダイアログを作成
            var target = FormDialog.FromForm(Customer.BuildForm) as FormDialog<Customer>;

            // 初回メッセージを投げたときの応答
            dialogContextMock.Setup(x => x.PostAsync(It.Is<IMessageActivity>(y => y.Text == "カスタマーの情報を入れてね"), default(CancellationToken)))
                .Returns(Task.CompletedTask)
                .Callback<IMessageActivity, CancellationToken>((x, y) => Debug.WriteLine(x.Text))
                .Verifiable();
            dialogContextMock.Setup(x => x.PostAsync(It.Is<IMessageActivity>(y => y.Text == "名前を入力してください"), default(CancellationToken)))
                .Returns(Task.CompletedTask)
                .Callback<IMessageActivity, CancellationToken>((x, y) => Debug.WriteLine(x.Text))
                .Verifiable();

            // 初回メッセージを投げ込んで意図した内容が渡ってきたか確認
            await target.MessageReceived(dialogContextMock.Object, Awaitable.FromItem<IMessageActivity>(new Activity { Text = "hi" }));
            dialogContextMock.Verify();

            // 名前を入力したら次は年齢の入力が来るのでセットアップ
            dialogContextMock.Setup(x => x.PostAsync(It.Is<IMessageActivity>(y => y.Text == "年齢を入力してください"), default(CancellationToken)))
                .Returns(Task.CompletedTask)
                .Verifiable();
            // 名前を入力して確認
            await target.MessageReceived(dialogContextMock.Object, Awaitable.FromItem<IMessageActivity>(new Activity { Text = "tanaka" }));
            dialogContextMock.Verify();

            // 年齢を入力したら次はタイプの入力が来るのでセットアップ
            dialogContextMock.Setup(x => x.PostAsync(It.Is<IMessageActivity>(y => y.Text == "タイプを選択してください"), default(CancellationToken)))
                .Returns(Task.CompletedTask)
                .Verifiable();
            // 年齢を入力
            await target.MessageReceived(dialogContextMock.Object, Awaitable.FromItem<IMessageActivity>(new Activity { Text = "20" }));
            dialogContextMock.Verify();

            // タイプを入力すると最終確認が来るのでセットアップ
            dialogContextMock.Setup(x => x.PostAsync(It.Is<IMessageActivity>(y => y.Text == "上記内容でよろしいですか"), default(CancellationToken)))
                .Returns(Task.CompletedTask)
                .Verifiable();
            // タイプを入力
            await target.MessageReceived(dialogContextMock.Object, Awaitable.FromItem<IMessageActivity>(new Activity { Text = "Normal" }));
            dialogContextMock.Verify();

            // 最終確認ではいを入力すると入力した結果が渡ってくるのでセットアップ
            dialogContextMock.Setup(x => x.Done(It.Is<Customer>(y =>
                y.Name == "tanaka" &&
                y.Age == 20 &&
                y.Type == CustomerType.Normal)))
                .Verifiable();
            // はいを入力
            await target.MessageReceived(dialogContextMock.Object, Awaitable.FromItem<IMessageActivity>(new Activity { Text = "はい" }));
            dialogContextMock.Verify();
        }
    }
}

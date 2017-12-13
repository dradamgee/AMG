using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;


namespace AMG.Physics.Test {

    public class MyWindow
    {
        private bool isBusy = false;

        public string Text;

        private async Task<string> LongRunning()
        {
            await Task.Delay(10);
            //throw new ApplicationException("Bang");
            return "Bang";
        }

        public void Click()
        {
            if (isBusy)
                return ;

            var asd = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(10);
                throw new ApplicationException("Bang");
            });

            asd.ContinueWith(
                t => { Text = t.Exception.InnerExceptions[0].Message;
                       isBusy = false; }
                , TaskContinuationOptions.NotOnRanToCompletion
            );
        }

        public async Task ClickAsync()
        {
            if (isBusy)
                return;

            try
            {
                isBusy = false;
                Text = await LongRunning();
            }
            catch (AggregateException ae)
            {
                isBusy = false;
                Text = ae.InnerExceptions[0].Message;
            }
        }

    }
    
    [TestFixture]
    public class UnitTest1 {
        [Test]
        public async Task TestTasks() {
            var myWindow = new MyWindow();
            await myWindow.ClickAsync();
            Assert.AreEqual("Bang", myWindow.Text);
        }
    }
}

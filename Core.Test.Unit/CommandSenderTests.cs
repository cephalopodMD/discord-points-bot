using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PointsBot.Core.Commands;

namespace Core.Test.Unit
{
    [TestClass]
    public class CommandSenderTests
    {
        [TestMethod]
        public async Task SendCommand_CallsSendAsync_WithMessage()
        {
            const string testMessage = "test";
            var testMessageBytes = Encoding.UTF8.GetBytes(testMessage);

            var mockQueueClient = new Mock<IQueueClient>();
            mockQueueClient.Setup(queue => queue.SendAsync(It.IsAny<Message>()));

            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(command => command.Serialize()).Returns(testMessage);

            var structureUnderTest = new CommandSender(mockQueueClient.Object);
            await structureUnderTest.SendCommand(mockCommand.Object);

            mockQueueClient.Verify(client => client.SendAsync(It.Is<Message>(message => message.Body.Length == testMessageBytes.Length)));
        }
    }
}

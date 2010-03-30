using Moq;
using NUnit.Framework;

namespace AppBus.Tests
{
    [TestFixture]
    public class MessageHandlerTests
    {
        [Test]
        public void Can_handle_when_type_is_the_same_as_T()
        {
            var handler = new Mock<MessageHandler<string>>().Object;
            var result = handler.CanHandle(typeof (string));
            Assert.IsTrue(result);
        }

        [Test]
        public void Cannot_handle_when_type_is_not_the_same_as_T()
        {
            var handler = new Mock<MessageHandler<string>>().Object;
            var result = handler.CanHandle(typeof (int));
            Assert.IsFalse(result);
        }

        [Test]
        public void Passing_object_to_handle_method_cast_to_T_and_passed_to_T_handle_method()
        {
            var mock = new Mock<MessageHandler<string>>();
            var handler = mock.Object;
            
            var value = string.Empty;
            handler.Handle((object)value);

            mock.Verify(x=>x.Handle(value), Times.Once());
        }
    }
}
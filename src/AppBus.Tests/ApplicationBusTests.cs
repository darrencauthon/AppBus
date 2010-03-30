using System;
using System.Linq;
using AutoMoq;
using Moq;
using NUnit.Framework;

namespace AppBus.Tests
{
    [TestFixture]
    public class ApplicationBusTests
    {
        private AutoMoqer mocker;

        [TestFixtureSetUp]
        public void Setup()
        {
            mocker = new AutoMoqer();
        }

        [Test]
        public void Can_Get_Handlers_That_Can_Handle_Type()
        {
            var type = typeof (string);

            var expectedResult = CreateHandler(type);

            var bus = mocker.Resolve<ApplicationBus>();
            bus.Add(expectedResult.GetType());

            var handlers = bus.GetHandlersForType(type);
            Assert.AreEqual(1, handlers.Count());
            Assert.AreSame(expectedResult, handlers.First());
        }

        [Test]
        public void Does_Not_Return_Handlers_That_Cannot_Handle_Type()
        {
            var type = typeof (string);

            var handler = CreateHandler(type);

            var bus = mocker.Resolve<ApplicationBus>();
            bus.Add(handler.GetType());

            var handlers = bus.GetHandlersForType(typeof (object));
            Assert.AreEqual(0, handlers.Count());
        }

        [Test]
        public void Calls_Handle_On_Handler_For_Type()
        {
            var message = new TestMessage();
            var type = message.GetType();

            var handler = CreateHandlerFake(type);

            var bus = mocker.Resolve<ApplicationBus>();
            bus.Add(handler.Object.GetType());

            bus.Send(message);

            handler.Verify(x => x.Handle(message), Times.Once());
        }

        [Test]
        public void Does_Not_Call_Handle_On_Handlers_For_Other_Types()
        {
            var message = new TestMessage();

            var handler = CreateHandlerFake(typeof (string));

            var bus = mocker.Resolve<ApplicationBus>();
            bus.Add(handler.Object.GetType());

            bus.Send(message);

            handler.Verify(x => x.Handle(message), Times.Never());
        }

        [Test]
        public void Calls_Handle_On_All_Handlers_For_This_Type()
        {
            var message = new TestMessage();
            var type = message.GetType();

            var firstHandler = new HandlerFake1();
            var secondHandler = new HandlerFake2();
            var thirdHandler = new HandlerFake3();

            ReturnThisHandlerWhenCalledForThisType(firstHandler, type);
            ReturnThisHandlerWhenCalledForThisType(secondHandler, type);
            ReturnThisHandlerWhenCalledForThisType(thirdHandler, type);

            var bus = mocker.Resolve<ApplicationBus>();
            bus.Add(firstHandler.GetType());
            bus.Add(secondHandler.GetType());
            bus.Add(thirdHandler.GetType());

            bus.Send(message);

            Assert.AreEqual(message, firstHandler.Message);
            Assert.AreEqual(message, secondHandler.Message);
            Assert.AreEqual(message, thirdHandler.Message);
        }

        private IMessageHandler CreateHandler(Type type)
        {
            return CreateHandlerFake(type).Object;
        }

        private Mock<IMessageHandler> CreateHandlerFake(Type type)
        {
            var fake = CreateFakeMessageHandlerThatCanHandleThis(type);
            mocker.GetMock<IMessageHandlerFactory>()
                .Setup(x => x.Create(fake.Object.GetType()))
                .Returns(fake.Object);
            return fake;
        }

        private void ReturnThisHandlerWhenCalledForThisType(IMessageHandler handler, Type type)
        {
            mocker.GetMock<IMessageHandlerFactory>()
                .Setup(x => x.Create(handler.GetType()))
                .Returns(handler);
        }

        private static Mock<IMessageHandler> CreateFakeMessageHandlerThatCanHandleThis(Type type)
        {
            var mock = new Mock<IMessageHandler>();
            mock.Setup(x => x.CanHandle(type))
                .Returns(true);
            return mock;
        }

        public class TestMessage
        {
        }

        public class HandlerFake1 : HandlerFake
        {
        }

        public class HandlerFake2 : HandlerFake
        {
        }

        public class HandlerFake3 : HandlerFake
        {
        }

        public class HandlerFake : IMessageHandler
        {
            public TestMessage Message { get; set; }

            public bool CanHandle(Type type)
            {
                return type == typeof (TestMessage);
            }

            public void Handle(object message)
            {
                Message = (TestMessage)message;
            }
        }
    }
}
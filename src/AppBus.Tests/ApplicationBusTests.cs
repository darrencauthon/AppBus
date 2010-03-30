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
            bus.Add(type);

            var handlers = bus.GetHandlersForType(type);
            Assert.AreEqual(1, handlers.Count());
            Assert.AreSame(expectedResult, handlers.First());
        }

        [Test]
        public void Does_Not_Return_Handlers_That_Cannot_Handle_Type()
        {
            var type = typeof (string);

            var bus = mocker.Resolve<ApplicationBus>();
            bus.Add(type);

            var handlers = bus.GetHandlersForType(typeof (object));
            Assert.AreEqual(0, handlers.Count());
        }

        private static Mock<IMessageHandler> CreateFakeMessageHandlerThatCanHandleThis(Type type)
        {
            var mock = new Mock<IMessageHandler>();
            mock.Setup(x => x.CanHandle(type))
                .Returns(true);
            return mock;
        }

        private IMessageHandler CreateHandler(Type type)
        {
            var expectedFake = CreateFakeMessageHandlerThatCanHandleThis(type);
            mocker.GetMock<IMessageHandlerFactory>()
                .Setup(x => x.Create(type))
                .Returns(expectedFake.Object);
            return expectedFake.Object;
        }
    }
}
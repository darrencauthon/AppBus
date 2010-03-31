using System;
using System.Linq;
using AutoMoq;
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
        public void Add_message_handler_type_adds_an_application_bus_registration()
        {
            var applicationBus = mocker.Resolve<ApplicationBus>();
            applicationBus.Add<Message>(typeof (MessageHandler));

            Assert.AreEqual(1, applicationBus.Count);
        }

        [Test]
        public void Message_handler_type_on_new_application_bus_registration_is_type_of_handler()
        {
            var applicationBus = mocker.Resolve<ApplicationBus>();
            applicationBus.Add<Message>(typeof (MessageHandler));

            var registration = applicationBus.First();
            Assert.AreEqual(typeof (MessageHandler), registration.MessageHandlerType);
        }

        [Test]
        public void Message_type_on_new_application_bus_registration_Is_T()
        {
            var applicationBus = mocker.Resolve<ApplicationBus>();
            applicationBus.Add<Message>(typeof (MessageHandler));

            var registration = applicationBus.First();
            Assert.AreEqual(typeof (Message), registration.MessageType);
        }

        [Test]
        public void Send_called_on_message_handler_that_matches_type_of_message()
        {
            var message = new Message();
            var messageHandler = new MessageHandler();

            var messageHandlerFactory = new MessageHandlerFactory();
            messageHandlerFactory.SetMessageHandlerToReturn(messageHandler);

            var applicationBus = new ApplicationBus(messageHandlerFactory);
            applicationBus.Add<Message>(typeof (MessageHandler));

            applicationBus.Send(message);

            Assert.AreSame(message, messageHandler.SentMessage);
        }
    }

    public class Message : IEventMessage
    {
    }

    public class MessageHandler : IMessageHandler<Message>
    {
        public Message SentMessage { get; set; }

        public void Handle(Message message)
        {
            SentMessage = message;
        }
    }

    public class MessageHandlerFactory : IMessageHandlerFactory
    {
        private MessageHandler messageHandlerToReturn;

        public void SetMessageHandlerToReturn(MessageHandler messageHandler)
        {
            messageHandlerToReturn = messageHandler;
        }

        public IMessageHandler<T> Create<T>(Type type)
        {
            if (type == typeof (MessageHandler))
                return messageHandlerToReturn as IMessageHandler<T>;
            return null;
        }
    }
}
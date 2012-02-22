AppBus is a small application bus that I borrowed from MvcContrib's Portable Areas.  I liked the idea of a simple, syncronous bus for decoupling, I took the ideas and wrote my own implementation.

The main difference between MvcContrib and this is that I filter the handlers according to type.  In other words, message handlers can handle only one type of message, and only the handlers that can handle a specific message are instantiated when a message is passed to the bus. 

```c#
public class SomethingHappened : IEventMessage {}

public class DoSomethingWhenSomethingHappens : IMessageHandler<SomethingHappened>{
	public void Handle(SomethingHappened message){
		// do something
	}
}

public class DoSomethingElseWhenSomethingHappens : IMessageHandler<SomethingHappened>{
	public void Handle(SomethingHappened message){
		// do something else
	}
}


applicationBus.Add(typeof (DoSomethingWhenSomethingHappens));
applicationBus.Add(typeof (DoSomethingElseWhenSomethingHappens));

// this message will be passed to both handlers
applicationBus.Send(new SomethingHappened());
````

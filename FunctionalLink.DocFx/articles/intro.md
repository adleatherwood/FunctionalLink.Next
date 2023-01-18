# FunctionalLink.Next

## Coming from FunctionalLink

This library is a reincarnation of a previous library [FunctionalLink](https://gitlab.com/adleatherwood/FunctionalLink).  I have written a significant amount of code with that library 
but felt that some of the choices I had made were limiting.  There were several 
issues I had with the library: 

* There was some overreaching functionality that was never really used.
* Some naming that was hard for people to understand (e.g. Bind).
* Some fancy asynchronous work (that was nice) but misplaced. 
* A dozen ad-hoc extension methods that were not focused enough to fit the library.

The library's purpose was to bring a few functional programming concepts to C# 
in an idiomatic way.  At the time, I thought the choices I had made were idiomatic.  
But after using it for awhile, I've found that things can be simplified.  That's 
what this library is.  A reduction of the first.  Down to it's most important parts.

## Language Used

While the language of the original library is more concise and accurate, the new 
library aims to use language that is more familiar and intuitive.  Originally, 
there were three main compositional functions: Bind, Map, & Void.  Used 
hypothetically like so:

```cs
Result.Success(value)
    .Bind(Validate) // i.e. SelectMany or FlatMap in other languages
    .Map(ToString)  // i.e. Select in Linq
    .Void(Console.WriteLine)
```

While the distinctions between those functions are interesting to note, they 
only served to be distracting to read for most people.  All anyone really wants 
to know is what's happening, and what's next.  While functional languages don't 
generally have function overloads, C# does.  So I've adopted the languages of 
Promises and bundled those functions under `Then`.

```cs
Result.Success(value)
    .Then(Validate)
    .Then(ToString)
    .Then(Console.WriteLine)
```

## Bare Minimum

There are actually only three main types in this library:

* Either&lt;T, TOther&gt;
* Option&lt;T&gt;
* Result&lt;T&gt;

Anything else that exists is purely to support those two types and make the use 
of them more intuitive and seamless.  In general, you should not be aware that 
any other types are being used.

## Declaration & Workflow over Inference & Exceptions

Developer optimism is a very common problem.  Take this function signature below:

```cs
User FindUser(string userId) { ... }
```

At face value, it would appear that this function will always return a User.  
Except, as experienced developers, we know that can't be true.  So we **infer** 
that FindUser must return null when it can't find the requested entity.  Maybe 
we'll even dig through it's implementation to find out if we are correct.  A 
more declarative (and informative) way to write this signature could be like so:

```cs
Option<User> FindUser(string userId) { ... }
```

Now we're being told quite explicitly that a result may not be found.  And we 
are given a return type that allows us to write "Happy Path" code when consuming 
that type:

```cs
FindUser("123")
    .Then(user => $"{user.FirstName} {user.LastName}")
    .Then(name => Console.WriteLine(name))
```

But this call also **implies** I/O.  Which is typically an behavior that throws 
exceptions.  So far, we know we might not get a user back from our call.  But 
the signature doesn't express anything about something going wrong.  And since 
it has the potential for an exception, we now have to decide if we should 
try/catch this call or let the exception fly.  Let's avoid all of that ambiguity 
and adjust the signature to tell the consumer something can go wrong without an 
exception being thrown.

```cs
Result<User, Exception> FindUser(string userId) { ... }
```

Now the caller can choose what to do with the Exception rather than just 
throwing it up the chain and hoping it's caught and handled in some way.  This 
is really the goal of this library.  To encourage developers to be explicit and 
code for all possibilities in any given context.  The benefits of functional 
composition run deeper than this, but this kind of thinking is the entry point 
for what's possible. 

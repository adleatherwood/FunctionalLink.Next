# The Result

The Result type is structured very similarly to the Option type.  It's recommend that
you read through the Option type first because all of the functions are the same.

The main different of the Result type is that instead of getting nothing (or None) when
there is no result, you get a Failure.  So the Option's narrative is, "there may or may not
be a value".  And the Result's narrative is, "This may or may not work".

Here's one of the Option examples refactored as a Result to show their similarities:
```csharp
public Result<User> FindUser(int userId)
{
   var found = ReadDb(userId);
   return found != null
        ? Success(found)
        : Failure("User not found"); 
}
```
A better context for a Result is validation:
```csharp
Result<User> ValidateUsername(User user)
{
    if (user.Username == null)
        return Failure("Username cannot be null");

    if (string.IsNullOrWhiteSpace(user.Username))
        return Failure("Username cannot be empty");

    return Success(user);
}
```
With a handful of validation functions like above, you can validate an entire entity like so:
```csharp
public Result<User> ValidateUser(User user)
{
    return ValidateUsername(user)
        .Then(ValidateEmail)
        .Then(ValidatePhone)
        .Then(ValidateAddress);
}
```
I/O is another easy fit for the Result type:
```csharp
public Result<string> ReadFile(string filename)
{
    if (!File.Exists(filename))
        return Failure("File does not exist");
        
    var contents = File.ReadAllText(filename);
    
    return Success(contents);
}
```
Or more succinctly:
```csharp
public Result<string> ReadFile(string filename) => 
    File.Exists(filename)
        ? Success(File.ReadAllText(filename))
        : Failure("File does not exist");        
```

The goal is to encourage writing workflows for negative paths rather than throwing exceptions.

For convenience, there are two Result Types:

* Result&lt;T&gt;
* Result&lt;T, TFailure&gt;

`Result<T>` inherits from `Result<T,string>`.  Because the failure state of a Result is often
a `string`, this reduces the number of generic parameters you have to deal with for most
use cases.

# The Option

The Option type is intended as a general replacement for null.  It's data 
structure is the same as Nullable<T>, but it can be used with either references 
or structs. So:
```csharp
{
    bool HasValue;
    T Value;
}
```
Here's an example of how you could use an Option:
```csharp
public Option<User> FindUser(int userId)
{
   var found = ReadDb(userId);
   return found != null
        ? Option.Some(found)
        : Option.None(); 
}
```
The database you are using may deal in nulls, but your code can prevent that 
from proliferating through your code.  The function signature above gives us a 
clear understanding what will be returned.  It also gives us a few other things 
as well.

## Deconstruction
You can inspect the Option for what value it has, if any:
```csharp
var user = FindUser(1); // returns Option<User>

if (user.HasValue(out var found))
{
    GreetUser(found)
}
```
You could also take a more functional approach:
```csharp
var user = FindUser(1)
    .Then(found => GreetUser(found));    
```
Or, in this case, simply:
```csharp
var user = FindUser(1)
    .Then(GreetUser);    
```
All of these arrangements provide the same functionality.  Just expressed 
differently.  It just depends on your taste as to how you prefer to express your 
logic.  This library is geared towards the last version.  It's hard to get more 
readable than: `Find user then greet user`.

## Substitution
You can also, make substitutions for when values are not present:
```csharp
var highScore = FindHighScore(userId: 1) // returns Option<int>
    .ValueOr(0);
    
Console.WriteLine($"Your Personal Best: {highScore}");  
```
Or:
```csharp
var highScore = FindHighScore(userId: 1) 
    .Then(hs => $"Your Personal Best: {hs}")
    .ValueOr("No scores on record yet"); 
    
Console.WriteLine(highScore);
```
Or:
```csharp
var highScore = FindHighScore(userId: 1) 
    .Match(
        some => $"Your Personal Best: {some}",
        none => "No scores on record yet");
        
Console.WriteLine(highScore); 
```
Or ideally:
```csharp
var highScore = FindHighScore(userId: 1) 
    .Then(hs => $"Your Personal Best: {hs}")
    .Else("No scores on record yet")
    .Then(Console.WriteLine); 
```
Each of the previous methods has it's time and place.  It's just a matter of 
what seems most appropriate at any given moment.

## Logical Composition
In addition to the basics above.  You can also logically chain Options together 
with Ands and Ors.
```csharp
var user = FindUser(userId: 1);           
var highScore = FindHighScore(userId: 1); 

var message = user.And(highScore,               // if both are successful 
        (u, hs) => (u.Username, HighScore: hs)) // map them together
    .Then(t => $"{t.Username}'s Personal Best: {t.HighScore}")
    .ValueOr("No scores on record yet");
    
Console.WriteLine(message);        
```
Or:
```csharp
var user = FindUser(userId: 1)                  
    .Or(() => CreateGuestUser(userId: 1)); // in order, the first successful value is used           

var highScore = FindHighScore(userId: 1); 

var message = user.And(highScore, 
        (u, hs) => (u.Username, HighScore: hs))
    .Then(t => $"{t.Username}'s Personal Best: {t.HighScore}")
    .ValueOr("No scores on record yet");
    
Console.WriteLine(message);        
```
The underlying goal of adopting these kinds of workflows is to ensure that all 
logical paths are managed and the code isn't written with casual developer 
optimism.  Where negative paths aren't managed by anything but exceptions.

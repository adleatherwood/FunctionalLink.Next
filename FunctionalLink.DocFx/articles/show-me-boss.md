# Just Show Me (Boss Edition)

```csharp
using FunctionalLink.Next;

namespace MyApp_Boss
{
    public class User
    {
        public string Id;
        public string Username;
    }

    public interface IUserRepository
    {        
        Task<Result<User>> LoadUser(string userId);
        Task<Result<int>> LoadHighScore(string userId);
    }

    public interface ITerminal
    {        
        Result<string> PromptForUserId();
        void GreetUser(User user);
        void DisplayHighScore(int highScore);
        void Alert(string message);
    }

    public static class App
    {         
        // this version does everything the "winded" version does.
        // but in less space than the optimistic version.
        // do this version.
        public static async void TheDiligentDeclarativeCoder(IUserRepository users, ITerminal terminal)
        {
            var result = await terminal.PromptForUserId()                    
                .Then(userId => users.LoadUser(userId))
                .Then(user => terminal.GreetUser(user))
                .Then(user => users.LoadHighScore(user.Id))
                .Then(score => terminal.DisplayHighScore(score));

            if (result.HasFailure(out var failure))
            {
                terminal.Alert(failure.Message);
            }
        }
    }
}
```
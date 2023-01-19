# Just Show Me (Winded Edition)

```csharp
using FunctionalLink.Next;

namespace MyApp_Winded
{
    public class User
    {
        public string Id;
        public string Username;
    }

    public interface IUserRepository
    {
        // now we know it might not succeed and the negative result is not "thrown".
        // we are given a type that allows us to deal with that possibility logically.
        Task<Result<User>> LoadUser(string userId);
        Task<Result<int>> LoadHighScore(string userId);
    }

    public interface ITerminal
    {
        // maybe some validation is added to the prompt to see if the input is any good or not
        Result<string> PromptForUserId();
        void GreetUser(User user);
        void DisplayHighScore(int highScore);
        void Alert(string message);
    }

    public static class App
    {                
        // this is the imperative logic that is conditioned for all possibilities.
        // this is crazy.  don't do this.  do the version after this.
        public static async void Main(IUserRepository users, ITerminal terminal)
        {
            var userId = terminal.PromptForUserId();

            // notice the repetition
            if (userId.HasSuccess(out var enteredId))
            {
                var user = await users.LoadUser(enteredId);

                // notice the repetition
                if (user.HasSuccess(out var foundUser))
                {
                    terminal.GreetUser(foundUser);

                    var highScore = await users.LoadHighScore(foundUser.Id);

                    // notice the repetition
                    if (highScore.HasSuccess(out var foundScore))
                    {
                        terminal.DisplayHighScore(foundScore);
                    }
                    
                    // notice the repetition
                    if (highScore.HasFailure(out var failure1))
                    {
                        terminal.Alert(failure1.Message);
                    }
                }

                // notice the repetition
                if (user.HasFailure(out var failure2))
                {
                    terminal.Alert(failure2.Message);
                }
            }
            
            // notice the repetition
            if (userId.HasFailure(out var failure3))
            {
                terminal.Alert(failure3.Message);
            }
        }
    }
}
```
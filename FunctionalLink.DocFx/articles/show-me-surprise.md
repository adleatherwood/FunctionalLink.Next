# Just Show Me (Surprise Edition)

Words.  What do they even mean?  Here's code instead.

```csharp
using FunctionalLink.Next;

namespace MyApp_Surprise
{
    public class User
    {
        public string Id;
        public string Username;
    }

    public interface IUserRepository
    {
        // we know these can't possibly always return a result.
        // they may return a null or throw an exception. we just have to assume.
        Task<User> LoadUser(string userId);
        Task<int> LoadHighScore(string userId);
    }

    public interface ITerminal
    {
        string PromptForUserId();
        void GreetUser(User user);
        void DisplayHighScore(int highScore);
        void Alert(string message);
    }

    public static class App
    {            
        // at a high level, this looks pretty reasonable.
        // exceptions are not declared. you have to know or suspect they will be thrown!
        // exceptions jump to the catch and eliminate the possibility for alternate workflows!
        // all calls to these functions must make the same assumptions/precautions.
        public static async void Main(IUserRepository users, ITerminal terminal)
        {        
            try
            {        
                var userId = terminal.PromptForUserId();        
                var user = await users.LoadUser(userId);

                terminal.GreetUser(user);

                var highScore = await users.LoadHighScore(user.Id);
                
                terminal.DisplayHighScore(highScore);
            }        
            catch (System.Exception e)
            {            
                terminal.Alert(e.Message);            
            }
        }
    }
}
```
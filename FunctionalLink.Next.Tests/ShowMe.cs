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
        public static async void TheOptimisticCoder(IUserRepository users, ITerminal terminal)
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
        public static async void TheDiligentImperativeCoder(IUserRepository users, ITerminal terminal)
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



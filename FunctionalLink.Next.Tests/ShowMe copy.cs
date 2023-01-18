using FunctionalLink.Next;

namespace MyApp1
{
    public class User
    {
        public string Id;
        public string Username;
    }

    namespace AnOptimisticImplementation
    {        
        public interface IUserRepository
        {
            // we know these can't possibly always return a result.
            // it must return a null or throw an exception, but we don't know which.
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
            // exceptions are the only negative path!
            // exceptions are not declared. you have to know or suspect they will be thrown!
            // exceptions jump straight to the catch and eliminate the possibility for alternate workflows!
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

    namespace ADiligentImplementation
    {
        public interface IUserRepository
        {
            // now we know it might not succeed 
            // we are given a type that allows us to deal with that possibility logically
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
            // this is the imperative logic that checks everything and is conditioned for all possibilities.
            // this is crazy.  don't do this.  do the version below this.
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

            // this version does everything the diligent version does, but in the space of the optimistic version.
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
}


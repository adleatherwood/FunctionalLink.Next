using System.Diagnostics.CodeAnalysis;

// ReSharper disable UnusedParameter.Local
// ReSharper disable ConvertClosureToMethodGroup

namespace FunctionalLink.Next;

/// <summary>
/// All behavior for the Result types are defined here.
/// </summary>
public static class ResultExtensions
{
    #region match / void -----------------------------------------------------------------------------------------------
    
    /// <summary>
	/// Executes one of the two given actions based on the state of the Result.  If the Result
	/// was successful, the onSuccess Action will be executed.  If not, the onFailure Action
	/// will be executed
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="onSuccess">The action to execute if the Result was successful</param>
	/// <param name="onFailure">The Action to execute if the Result was not successful</param>
	public static void Match<T,TFailure>(this Result<T,TFailure> result, Action<T> onSuccess, Action<TFailure> onFailure)
	{
		if (result.HasValue(out var value))
			onSuccess(value);
		else if (result.HasFailure(out var failure))
			onFailure(failure);
	}

    /// <summary>
	/// Executes one of the two given actions based on the state of the Result.  If the Result
	/// was successful, the onSuccess Action will be executed.  If not, the onFailure Action
	/// will be executed
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="onSuccess">The action to execute if the Result was successful</param>
	/// <param name="onFailure">The Action to execute if the Result was not successful</param>
    // async -> sync
    public static async Task Match<T, TFailure>(this Task<Result<T, TFailure>> result, Action<T> onSuccess,
        Action<TFailure> onFailure)
    {
        (await result).Match(onSuccess, onFailure);
    }

    #endregion
    #region match / func -----------------------------------------------------------------------------------------------

    /// <summary>
	/// Executes one of the two given functions based on the state of the Result and returns a value.  If
	/// the Result was successful, the onSuccess function will be executed.  If not, the onFailure function
	/// will be executed
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="onSuccess">The function to execute if the Result was successful</param>
	/// <param name="onFailure">The function to execute if the Result was not successful</param>
	/// <returns>Returns the result of onSuccess if the Result was successful.  Otherwise, the result of onFailure</returns>
	public static TResult Match<T, TFailure, TResult>(this Result<T,TFailure> result, Func<T, TResult> onSuccess, Func<TFailure, TResult> onFailure) =>
		result.HasValue(out var value)
			? onSuccess(value)
			: result.HasFailure(out var failure) 
                ? onFailure(failure)
                : throw new InvalidOperationException("Invalid result state");

    /// <summary>
	/// Executes one of the two given functions based on the state of the Result and returns a value.  If
	/// the Result was successful, the onSuccess function will be executed.  If not, the onFailure function
	/// will be executed
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="onSuccess">The function to execute if the Result was successful</param>
	/// <param name="onFailure">The function to execute if the Result was not successful</param>
	/// <returns>Returns the result of onSuccess if the Result was successful.  Otherwise, the result of onFailure</returns>
    // async -> sync
    public static async Task<TResult> Match<T, TFailure, TResult>(this Task<Result<T, TFailure>> result,
        Func<T, TResult> onSuccess, Func<TFailure, TResult> onFailure) =>
        (await result).Match(onSuccess, onFailure);


    #endregion
    #region then / bind ------------------------------------------------------------------------------------------------
    
    /// <summary>
	/// Executes the given function if the given Result was successful.  This overload is the compositional equivalent
	/// of `Bind`, `FlatMap`, or `SelectMany` style functions available in other contexts
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns another result with either the return value of the given function or the current failure</returns>
    public static Result<TResult, TFailure> Then<T, TFailure, TResult>(this Result<T,TFailure> result, Func<T, Result<TResult, TFailure>> f) =>
        result.Match(f, Result<TResult, TFailure>.Failure);
            
    /// <summary>
	/// Executes the given function if the given Result was successful.  This overload is the compositional equivalent
	/// of `Bind`, `FlatMap`, or `SelectMany` style functions available in other contexts
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns another result with either the return value of the given function or the current failure</returns>
    // sync -> async
    public static async Task<Result<TResult, TFailure>> Then<T, TFailure, TResult>(this Result<T, TFailure> result,
        Func<T, Task<Result<TResult, TFailure>>> f) =>
        await result.Match(f, TaskFailure<TResult,TFailure>);
    
    /// <summary>
	/// Executes the given function if the given Result was successful.  This overload is the compositional equivalent
	/// of `Bind`, `FlatMap`, or `SelectMany` style functions available in other contexts
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns another result with either the return value of the given function or the current failure</returns>
    // async -> async
    public static async Task<Result<TResult, TFailure>> Then<T, TFailure, TResult>(this Task<Result<T, TFailure>> result,
        Func<T, Task<Result<TResult, TFailure>>> f) =>
        await (await result).Then(f);
    
    /// <summary>
	/// Executes the given function if the given Result was successful.  This overload is the compositional equivalent
	/// of `Bind`, `FlatMap`, or `SelectMany` style functions available in other contexts
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns another result with either the return value of the given function or the current failure</returns>
    // async -> sync
    public static async Task<Result<TResult, TFailure>> Then<T, TFailure, TResult>(this Task<Result<T, TFailure>> result,
        Func<T, Result<TResult, TFailure>> f) =>
        (await result).Then(f);

    #endregion
    #region then / map -------------------------------------------------------------------------------------------------
    
    /// <summary>
	/// Executes the given function if the given Result was successful.  This overload is the compositional equivalent
	/// of `Map` or `Select` style functions available in other contexts
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns another result with either the return value of the given function or the current failure</returns>
    public static Result<TResult, TFailure> Then<T, TFailure, TResult>(this Result<T,TFailure> result, Func<T, TResult> f) =>
        result.Match(
            success => Result<TResult, TFailure>.Success(f(success)),
            Result<TResult, TFailure>.Failure);
        
    /// <summary>
	/// Executes the given function if the given Result was successful.  This overload is the compositional equivalent
	/// of `Map` or `Select` style functions available in other contexts
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns another result with either the return value of the given function or the current failure</returns>
    // sync -> async
    public static async Task<Result<TResult, TFailure>> Then<T, TFailure, TResult>(this Result<T, TFailure> result,
        Func<T, Task<TResult>> f) =>
        await result.Match(
            async success => Result<TResult, TFailure>.Success(await f(success)),
            TaskFailure<TResult, TFailure>);
        
    /// <summary>
	/// Executes the given function if the given Result was successful.  This overload is the compositional equivalent
	/// of `Map` or `Select` style functions available in other contexts
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns another result with either the return value of the given function or the current failure</returns>
    // async -> async
    public static async Task<Result<TResult, TFailure>> Then<T, TFailure, TResult>(this Task<Result<T, TFailure>> result,
        Func<T, Task<TResult>> f) =>
        await (await result).Then(f);
    
    /// <summary>
	/// Executes the given function if the given Result was successful.  This overload is the compositional equivalent
	/// of `Map` or `Select` style functions available in other contexts
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns another result with either the return value of the given function or the current failure</returns>
    // async -> sync
    public static async Task<Result<TResult, TFailure>> Then<T, TFailure, TResult>(this Task<Result<T, TFailure>> result,
        Func<T, TResult> f) =>
        (await result).Then(f);
    
    #endregion
    #region then / void ------------------------------------------------------------------------------------------------
    
    /// <summary>
	/// Executes the given action if the given Result was successful.  This overload is used to accommodate methods that
	/// have a void return signature
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns the given Result after the action is executed</returns>
    public static Result<T, TFailure> Then<T,TFailure>(this Result<T,TFailure> result, Action<T> f)
    {
        if (result.HasValue(out var value))
            f(value);

        return result;
    }
    
    /// <summary>
	/// Executes the given action if the given Result was successful.  This overload is used to accommodate methods that
	/// have a void return signature
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns the given Result after the action is executed</returns>
    // sync -> async
    public static async Task<Result<T, TFailure>> Then<T, TFailure>(this Result<T, TFailure> result,
        Func<T, Task> f)
    {
        if (result.HasValue(out var value))
            await f(value);
        
        return result;
    }

    /// <summary>
	/// Executes the given action if the given Result was successful.  This overload is used to accommodate methods that
	/// have a void return signature
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns the given Result after the action is executed</returns>
    // async -> async
    public static async Task<Result<T, TFailure>> Then<T, TFailure>(this Task<Result<T, TFailure>> result,
        Func<T, Task> f) =>
        await (await result).Then(f);
    
    /// <summary>
	/// Executes the given action if the given Result was successful.  This overload is used to accommodate methods that
	/// have a void return signature
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns the given Result after the action is executed</returns>
    // async -> sync
    public static async Task<Result<T, TFailure>> Then<T, TFailure>(this Task<Result<T, TFailure>> result,
        Action<T> f) =>
        (await result).Then(f);
    
    #endregion
    #region then / lift Result<T> --------------------------------------------------------------------------------------
    
    /// <summary>
	/// Executes the given function if the given Result was successful.  This overload is the compositional equivalent
	/// of `Bind`, `FlatMap`, or `SelectMany` style functions available in other contexts.
	///
	/// This is a special case for mitigating interop between Result&lt;T&gt; and Result&lt;T, TFailure&gt;.  Without
	/// this handler, the return type is: Result&lt;Result&lt;T&gt;, TFailure&gt;
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns another result with either the return value of the given function or the current failure</returns>
	public static Result<TResult, string> Then<T, TResult, TFailure>(this Result<T, TFailure> result, Func<T, Result<TResult>> f) =>
        result.Match<T, TFailure, Result<TResult,string>>(
            success => f(success),
            failure => Result<TResult, string>.Failure(failure?.ToString()));
		
    /// <summary>
	/// Executes the given function if the given Result was successful.  This overload is the compositional equivalent
	/// of `Bind`, `FlatMap`, or `SelectMany` style functions available in other contexts.
	///
	/// This is a special case for mitigating interop between Result&lt;T&gt; and Result&lt;T, TFailure&gt;.  Without
	/// this handler, the return type is: Result&lt;Result&lt;T&gt;, TFailure&gt;
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns another result with either the return value of the given function or the current failure</returns>
    // sync -> async
    public static async Task<Result<TResult, string>> Then<T, TFailure, TResult>(this Result<T, TFailure> result,
        Func<T, Task<Result<TResult>>> f) =>
        await result.Match<T,TFailure,Task<Result<TResult,string>>>(
            async success => await f(success),
            failure => TaskFailure<TResult, string>(failure?.ToString()));        
    
    /// <summary>
	/// Executes the given function if the given Result was successful.  This overload is the compositional equivalent
	/// of `Bind`, `FlatMap`, or `SelectMany` style functions available in other contexts.
	///
	/// This is a special case for mitigating interop between Result&lt;T&gt; and Result&lt;T, TFailure&gt;.  Without
	/// this handler, the return type is: Result&lt;Result&lt;T&gt;, TFailure&gt;
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns another result with either the return value of the given function or the current failure</returns>
    // async -> async
    public static async Task<Result<TResult, string>> Then<T, TFailure, TResult>(this Task<Result<T, TFailure>> result,
        Func<T, Task<Result<TResult>>> f) =>
        await (await result).Then(f);
    
    /// <summary>
	/// Executes the given function if the given Result was successful.  This overload is the compositional equivalent
	/// of `Bind`, `FlatMap`, or `SelectMany` style functions available in other contexts.
	///
	/// This is a special case for mitigating interop between Result&lt;T&gt; and Result&lt;T, TFailure&gt;.  Without
	/// this handler, the return type is: Result&lt;Result&lt;T&gt;, TFailure&gt;
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns another result with either the return value of the given function or the current failure</returns>
    // async -> sync
    public static async Task<Result<TResult, string>> Then<T, TFailure, TResult>(this Task<Result<T, TFailure>> result,
        Func<T, Result<TResult>> f) =>
        (await result).Then(f);
    
    #endregion    
    #region value or ---------------------------------------------------------------------------------------------------


    /// <summary>
	/// Used to deconstruct a Result to either it's successful value or a given alternative
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="alt">The alternative value to return if the Result was not successful</param>
	/// <returns>If the Result is successful, the Result value is returned.  Otherwise, the alternate value is returned</returns>
	public static T ValueOr<T, TFailure>(this Result<T, TFailure> result, T alt) =>
        result.Match(
            success => success,
            failure => alt);
		
    /// <summary>
	/// Used to deconstruct a Result to either it's successful value or a given alternative
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="alt">The alternative value to return if the Result was not successful</param>
	/// <returns>If the Result is successful, the Result value is returned.  Otherwise, the alternate value is returned</returns>
    // async -> sync
    public static async Task<T> ValueOr<T, TFailure>(this Task<Result<T, TFailure>> result, T alt) =>
        (await result).ValueOr(alt);
    
    #endregion
    #region or / map / static ------------------------------------------------------------------------------------------
    
    /// <summary>
	/// Used to transform a Failure state into a Success alternate value.
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="alt">The function to provide an alternate value</param>
	/// <returns>If the current Result is a Failure, then the alternate value is applied</returns>
	public static Result<T, TFailure> Or<T,TFailure>(this Result<T,TFailure> result, T alt) =>
        result.Match(
            success => Result<T,TFailure>.Success(success),
            failure => Result<T,TFailure>.Success(alt));
		
    /// <summary>
	/// Used to transform a Failure state into a Success alternate value.
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="alt">The function to provide an alternate value</param>
	/// <returns>If the current Result is a Failure, then the alternate value is applied</returns>
    // async -> sync
    public static async Task<Result<T, TFailure>> Or<T, TFailure>(this Task<Result<T, TFailure>> result, T alt) =>
        (await result).Or(alt);
    
    #endregion
    #region or / map / lazy --------------------------------------------------------------------------------------------

    /// <summary>
	/// Used to transform a Failure state into a Success alternate value.
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="alt">The function to provide an alternate value</param>
	/// <returns>If the current Result is a Failure, then the alternate value is applied</returns>
	public static Result<T, TFailure> Or<T, TFailure>(this Result<T, TFailure> result, Func<T> alt) =>
		result.Match(
            success => Result<T,TFailure>.Success(success),
            failure => Result<T,TFailure>.Success(alt()));

    /// <summary>
	/// Used to transform a Failure state into a Success alternate value.
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="alt">The function to provide an alternate value</param>
	/// <returns>If the current Result is a Failure, then the alternate value is applied</returns>
    // async -> sync
    public static async Task<Result<T, TFailure>> Or<T, TFailure>(this Task<Result<T, TFailure>> result, Func<T> alt) =>
        (await result).Or(alt);
    
    #endregion
    #region or / bind / static -----------------------------------------------------------------------------------------
    
    /// <summary>
	/// Used to trade alternative Result values if the given Result is a failure
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="alt">The function to return an alternate Result</param>
	/// <returns>If the given Result is successful, it will be returned.  Otherwise, the alternate Result is returned</returns>
	public static Result<T, TFailure> Or<T, TFailure>(this Result<T, TFailure> result, Result<T, TFailure> alt) =>
        result.Match(
            success => Result<T, TFailure>.Success(success),
            failure => alt);

    /// <summary>
	/// Used to trade alternative Result values if the given Result is a failure
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="alt">The function to return an alternate Result</param>
	/// <returns>If the given Result is successful, it will be returned.  Otherwise, the alternate Result is returned</returns>
    // async -> sync
    public static async Task<Result<T, TFailure>> Or<T, TFailure>(this Task<Result<T, TFailure>> result, Result<T, TFailure> alt) =>
        (await result).Or(alt);
    
    #endregion
    #region or / bind / lazy -------------------------------------------------------------------------------------------
    
    /// <summary>
	/// Used to trade alternative Result values if the given Result is a failure
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="alt">The function to return an alternate Result</param>
	/// <returns>If the given Result is successful, it will be returned.  Otherwise, the alternate Result is returned</returns>
	public static Result<T, TFailure> Or<T, TFailure>(this Result<T, TFailure> result, Func<Result<T, TFailure>> alt) =>
        result.Match(
            success => Result<T, TFailure>.Success(success),
            failure => alt());
		
    /// <summary>
	/// Used to trade alternative Result values if the given Result is a failure
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="alt">The function to return an alternate Result</param>
	/// <returns>If the given Result is successful, it will be returned.  Otherwise, the alternate Result is returned</returns>
    // sync -> async
    public static async Task<Result<T, TFailure>> Or<T, TFailure>(this Result<T, TFailure> result,
        Func<Task<Result<T, TFailure>>> alt) =>
        result.HasValue(out _)
            ? result
            : await alt();
        
    /// <summary>
	/// Used to trade alternative Result values if the given Result is a failure
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="alt">The function to return an alternate Result</param>
	/// <returns>If the given Result is successful, it will be returned.  Otherwise, the alternate Result is returned</returns>
    // async -> async
    public static async Task<Result<T, TFailure>> Or<T, TFailure>(this Task<Result<T, TFailure>> result,
        Func<Task<Result<T, TFailure>>> alt) =>
        await (await result).Or(alt);
    
    /// <summary>
	/// Used to trade alternative Result values if the given Result is a failure
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="alt">The function to return an alternate Result</param>
	/// <returns>If the given Result is successful, it will be returned.  Otherwise, the alternate Result is returned</returns>
    // async -> sync
    public static async Task<Result<T, TFailure>> Or<T, TFailure>(this Task<Result<T, TFailure>> result,
        Func<Result<T, TFailure>> alt) =>
        (await result).Or(alt);
    
    #endregion
    #region and / map / static -----------------------------------------------------------------------------------------

    /// <summary>
	/// Used to combine two Results if they are both successful
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="other">The second result to combine</param>
	/// <param name="selector">The function used to combine the values from ResultA and B</param>	
	/// <returns>If the Result was successful, the combined value of ResultA and B is returned.  Otherwise, the failure from either ResultA or B will be returned</returns>
	public static Result<TResult, TFailure> And<T, TFailure, TOther, TResult>(this Result<T,TFailure> result, TOther other, Func<T, TOther, TResult> selector) =>
        result.Match(
            success => Result<TResult, TFailure>.Success(selector(success, other)),
            failure => Result<TResult, TFailure>.Failure(failure));

    /// <summary>
	/// Used to combine two Results if they are both successful
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="other">The second result to combine</param>
	/// <param name="selector">The function used to combine the values from ResultA and B</param>	
	/// <returns>If the Result was successful, the combined value of ResultA and B is returned.  Otherwise, the failure from either ResultA or B will be returned</returns>
    // async -> sync
    public static async Task<Result<TResult, TFailure>> And<T, TFailure, TOther, TResult>(this Task<Result<T, TFailure>> result, TOther other, Func<T, TOther, TResult> selector) =>
        (await result).And(other, selector);

    #endregion
    #region and / map / lazy -------------------------------------------------------------------------------------------
 
    /// <summary>
	/// Used to combine two Results if they are both successful
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="other">The second result to combine</param>
	/// <param name="selector">The function used to combine the values from ResultA and B</param>	
	/// <returns>If the Result was successful, the combined value of ResultA and B is returned.  Otherwise, the failure from either ResultA or B will be returned</returns>
	public static Result<TResult, TFailure> And<T, TFailure, TOther, TResult>(this Result<T,TFailure> result, Func<TOther> other, Func<T, TOther, TResult> selector) =>
        result.Match(
            success => Result<TResult, TFailure>.Success(selector(success, other())),
            failure => Result<TResult, TFailure>.Failure(failure));
	
    /// <summary>
	/// Used to combine two Results if they are both successful
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="other">The second result to combine</param>
	/// <param name="selector">The function used to combine the values from ResultA and B</param>	
	/// <returns>If the Result was successful, the combined value of ResultA and B is returned.  Otherwise, the failure from either ResultA or B will be returned</returns>
    // async -> sync
    public static async Task<Result<TResult, TFailure>> And<T, TFailure, TOther, TResult>(this Task<Result<T, TFailure>> result, Func<TOther> other, Func<T, TOther, TResult> selector) =>
        (await result).And(other, selector);

    #endregion
    #region and / bind / static ----------------------------------------------------------------------------------------

    // moved to class to remove ambiguity with "and / map / static"
    // /// <summary>
	// /// Used to combine two Results if they are both successful
	// /// </summary>
    // /// <param name="result">The Result to examine</param>
	// /// <param name="other">The second result to combine</param>
	// /// <param name="selector">The function used to combine the values from ResultA and B</param>	
	// /// <returns>If the Result was successful, the combined value of ResultA and B is returned.  Otherwise, the failure from either ResultA or B will be returned</returns>
	// public static Result<TResult, TFailure> And<T, TFailure, TOther, TResult>(this Result<T,TFailure> result, Result<TOther, TFailure> other, Func<T, TOther, TResult> selector) =>
    //     result.Match(
    //         success1 => 
    //             other.Match(
    //                 success2 => Result<TResult, TFailure>.Success(selector(success1, success2)),
    //                 failure2 => Result<TResult, TFailure>.Failure(failure2)),
    //         failure1 => Result<TResult, TFailure>.Failure(failure1));
	
    /// <summary>
	/// Used to combine two Results if they are both successful
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="other">The second result to combine</param>
	/// <param name="selector">The function used to combine the values from ResultA and B</param>	
	/// <returns>If the Result was successful, the combined value of ResultA and B is returned.  Otherwise, the failure from either ResultA or B will be returned</returns>
    // async -> sync
    public static async Task<Result<TResult, TFailure>> And<T, TFailure, TOther, TResult>(this Task<Result<T, TFailure>> result, Result<TOther, TFailure> other, Func<T, TOther, TResult> selector) =>
        (await result).And(other, selector);
    
    #endregion
    #region and / bind / lazy ------------------------------------------------------------------------------------------
    
    // moved to class to remove ambiguity with "and / map / lazy"
    // /// <summary>
	// /// Used to combine two Results if they are both successful
	// /// </summary>
    // /// <param name="result">The Result to examine</param>
	// /// <param name="other">The second result to combine</param>
	// /// <param name="selector">The function used to combine the values from ResultA and B</param>	
	// /// <returns>If the Result was successful, the combined value of ResultA and B is returned.  Otherwise, the failure from either ResultA or B will be returned</returns>
	// public static Result<TResult, TFailure> And<T, TFailure, TOther, TResult>(this Result<T, TFailure> result, Func<Result<TOther, TFailure>> other, Func<T, TOther, TResult> selector) =>
    //     result.Match(
    //         success => 
    //             other().Match(
    //                 success2 => Result<TResult, TFailure>.Success(selector(success, success2)),
    //                 failure2 => Result<TResult, TFailure>.Failure(failure2)),
    //         failure => Result<TResult, TFailure>.Failure(failure));

    /// <summary>
	/// Used to combine two Results if they are both successful
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="other">The second result to combine</param>
	/// <param name="selector">The function used to combine the values from ResultA and B</param>	
	/// <returns>If the Result was successful, the combined value of ResultA and B is returned.  Otherwise, the failure from either ResultA or B will be returned</returns>
    // async -> sync
    public static async Task<Result<TResult, TFailure>> And<T, TFailure, TOther, TResult>(this Task<Result<T, TFailure>> result, Func<Result<TOther, TFailure>> other, Func<T, TOther, TResult> selector) =>
        (await result).And(other, selector);

    /// <summary>
	/// Used to combine two Results if they are both successful
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="other">The second result to combine</param>
	/// <param name="selector">The function used to combine the values from ResultA and B</param>	
	/// <returns>If the Result was successful, the combined value of ResultA and B is returned.  Otherwise, the failure from either ResultA or B will be returned</returns>
    // sync -> async
    public static async Task<Result<TResult, TFailure>> And<T, TFailure, TOther, TResult>(this Result<T, TFailure> result, Func<Task<Result<TOther, TFailure>>> other, Func<T, TOther, TResult> selector) =>
        result.And(await other(), selector);
    
    /// <summary>
	/// Used to combine two Results if they are both successful
	/// </summary>
    /// <param name="result">The Result to examine</param>
	/// <param name="other">The second result to combine</param>
	/// <param name="selector">The function used to combine the values from ResultA and B</param>	
	/// <returns>If the Result was successful, the combined value of ResultA and B is returned.  Otherwise, the failure from either ResultA or B will be returned</returns>
    // async -> async
    public static async Task<Result<TResult, TFailure>> And<T, TFailure, TOther, TResult>(this Task<Result<T, TFailure>> result, Func<Task<Result<TOther, TFailure>>> other, Func<T, TOther, TResult> selector) =>
        await (await result).And(other, selector);
    
    #endregion   
    // private ---------------------------------------------------------------------------------------------------------
    
    [ExcludeFromCodeCoverage]
    private static Task<Result<T, TFailure>> TaskFailure<T, TFailure>(TFailure failure) =>
        Task.FromResult(Result<T, TFailure>.Failure(failure));
}
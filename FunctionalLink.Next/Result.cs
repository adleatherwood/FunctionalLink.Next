namespace FunctionalLink.Next;

/// <summary>
/// This version of the Result type assumes the failure state will be a string.  This is often
/// the case and reduced the amount of type information required on function signatures
/// </summary>
/// <typeparam name="T">The type of the internal value if successful</typeparam>
public class Result<T>
	: Result<T, string>
{
	internal Result(bool success, T value, string failure = "")
		: base(success, value, failure)
	{
	}
	
	/// <summary>
	/// An implicit conversion operator for reducing the amount of syntax required for creating
	/// successful Results.
	/// </summary>
	/// <param name="success">The Success instance to convert</param>
	/// <returns>A Result of T</returns>
	/// <remarks>
	/// Typically, this operator is used like so:
	/// <code>
	/// public static Result&lt;int&gt; Divide(int a, int b) =>
	///     b != 0
	///         ? Success(a / b)  // this Success is implicitly converted to Result&lt;int&gt;
	///         : Failure("Cannot divide by zero"); 
	/// </code>
	/// To avoid the implicit conversion, use the 'Failure' Option constructor directly:
	/// <code>
	/// public static Result&lt;int&gt; Divide(int a, int b) =>
	///     b != 0
	///         ? Result&lt;int&gt;.Success(a / b)  // no implicit conversion is done here
	///         : Result&lt;int&gt;.Failure("Cannot divide by zero");   
	/// </code>
	/// </remarks>
	public static implicit operator Result<T>(Success<T> success) =>
		new (true, success.Value);

	/// <summary>
	/// An implicit conversion operator for reducing the amount of syntax required for creating
	/// failed Results.
	/// </summary>
	/// <param name="failure">The failure message</param>
	/// <returns>A Result of T</returns>
	/// <remarks>
	/// Typically, this operator is used like so:
	/// <code>
	/// public static Result&lt;int&gt; Divide(int a, int b) =>
	///     b != 0
	///         ? Success(a / b)
	///         : Failure("Cannot divide by zero"); // this Failure is implicitly converted to Result&lt;int&gt;
	/// </code>
	/// To avoid the implicit conversion, use the 'Failure' Option constructor directly:
	/// <code>
	/// public static Result&lt;int&gt; Divide(int a, int b) =>
	///     b != 0
	///         ? Result&lt;int&gt;.Success(a / b)
	///         : Result&lt;int&gt;.Failure("Cannot divide by zero"); // no implicit conversion is done here  
	/// </code>
	/// </remarks>
	public static implicit operator Result<T>(Failure<string> failure) =>
		new(false, default!, failure.Value);
}

/// <summary>
/// The result type is used to eliminate the need for throwing exceptions.  The idea is that a function
/// that returns a result will either return a successful value or an error value.  The methods of the
/// Result type help you compose and deconstruct this information.
/// </summary>
/// <typeparam name="T">The internal value type, if successful</typeparam>
/// <typeparam name="TFailure">The internal failure type, if failed</typeparam>
public class Result<T, TFailure>
{
	internal Result(bool hasValue, T value, TFailure failure = default!) =>
		(this._hasValue, _value, _failure) = (hasValue, value, failure);

	private readonly bool _hasValue;
	private readonly T _value;
	private readonly TFailure _failure;

	/// <summary>
	/// The constructor for a successful Result.
	/// </summary>
	/// <param name="value">The successful value of the Result</param>
	/// <returns>A Result in the 'Success' state with the given value</returns>
	public static Result<T, TFailure> Success(T value) =>
		new (true, value);

	/// <summary>
	/// The constructor for a failed Result.
	/// </summary>
	/// <param name="failure">The failure value of the Result</param>
	/// <returns>A Result in the 'Failed' state with the given value</returns>
	public static Result<T, TFailure> Failure(TFailure failure) =>
		new (false, default!, failure);

	/// <summary>
	/// An implicit conversion operator for reducing the amount of syntax required for creating
	/// successful Results.
	/// </summary>
	/// <param name="success">The Success instance to convert</param>
	/// <returns>A Result of T</returns>
	/// <remarks>
	/// Typically, this operator is used like so:
	/// <code>
	/// public static Result&lt;int,string&gt; Divide(int a, int b) =>
	///     b != 0
	///         ? Success(a / b)  // this Success is implicitly converted to Result&lt;int,string&gt;
	///         : Failure("Cannot divide by zero"); 
	/// </code>
	/// To avoid the implicit conversion, use the 'Failure' Result constructor directly:
	/// <code>
	/// public static Result&lt;int&gt; Divide(int a, int b) =>
	///     b != 0
	///         ? Result&lt;int,string&gt;;.Success(a / b)  // no implicit conversion is done here
	///         : Result&lt;int,string&gt;;.Failure("Cannot divide by zero");   
	/// </code>
	/// </remarks>
	public static implicit operator Result<T, TFailure>(Success<T> success) =>
		new (true, success.Value);

	/// <summary>
	/// An implicit conversion operator for reducing the amount of syntax required for creating
	/// failed Results.
	/// </summary>
	/// <param name="failure">The failure message</param>
	/// <returns>A Result of T</returns>
	/// <remarks>
	/// Typically, this operator is used like so:
	/// <code>
	/// public static Result&lt;int,string&gt; Divide(int a, int b) =>
	///     b != 0
	///         ? Success(a / b)
	///         : Failure("Cannot divide by zero"); // this Failure is implicitly converted to Result&lt;int&gt;
	/// </code>
	/// To avoid the implicit conversion, use the 'Failure' Result constructor directly:
	/// <code>
	/// public static Result&lt;int,string&gt; Divide(int a, int b) =>
	///     b != 0
	///         ? Result&lt;int,string&gt;.Success(a / b)
	///         : Result&lt;int,string&gt;.Failure("Cannot divide by zero"); // no implicit conversion is done here  
	/// </code>
	/// </remarks>
	public static implicit operator Result<T, TFailure>(Failure<TFailure> failure) =>
		new (false, default!, failure.Value);

	/// <summary>
	/// A conversion operator for turning Result&lt;T,TFailure&gt; to Result&lt;T&gt; 
	/// </summary>
	/// <param name="result">The result to convert</param>
	/// <returns>A Result&lt;T&gt;</returns>
	public static implicit operator Result<T>(Result<T, TFailure> result) =>
		/* NOTE: this conversion is unfortunate, but it seems the only way to get to a Result<T> implicitly
		 *		 from a Result<T,string>. it cannot be done from the subclass, the compiler won't allow it.
		 *		 which is sort of tragic, because that's easily the most sensible place for it to be.
		 */
		new (result._hasValue, result._value, result._failure?.ToString());
	
	/// <summary>
	/// Determines if a Result was successful and returns the value
	/// </summary>
	/// <param name="value">The value contained within the Result</param>
	/// <returns>True if the Result was successful, False if not</returns>
	public bool HasValue(out T value)
	{
		value = _hasValue ? _value : default!;
		return _hasValue;
	}
	
	/// <summary>
	/// Determines if a Result was not successful and returns the failure
	/// </summary>
	/// <param name="value">The value contained within the Result</param>
	/// <returns>True if the Result was not success, False if it was</returns>
	public bool HasFailure(out TFailure value)
	{
		value = _failure;
		return !_hasValue;
	}
	
	/// <summary>
	/// Executes one of the two given actions based on the state of the Result.  If the Result
	/// was successful, the onSuccess Action will be executed.  If not, the onFailure Action
	/// will be executed
	/// </summary>
	/// <param name="onSuccess">The action to execute if the Result was successful</param>
	/// <param name="onFailure">The Action to execute if the Result was not successful</param>
	public void Match(Action<T> onSuccess,
		Action<TFailure> onFailure)
	{
		if (HasValue(out var value))
			onSuccess(value);
		else
			onFailure(_failure);
	}
	
	/// <summary>
	/// Executes one of the two given functions based on the state of the Result and returns a value.  If
	/// the Result was successful, the onSuccess function will be executed.  If not, the onFailure function
	/// will be executed
	/// </summary>
	/// <param name="onSuccess">The function to execute if the Result was successful</param>
	/// <param name="onFailure">The function to execute if the Result was not successful</param>
	/// <returns>Returns the result of onSuccess if the Result was successful.  Otherwise, the result of onFailure</returns>
	public TResult Match<TResult>(Func<T, TResult> onSuccess,
		Func<TFailure, TResult> onFailure) =>
		HasValue(out var value)
			? onSuccess(value)
			: onFailure(_failure);
	
	/// <summary>
	/// Executes the given function if the given Result was successful.  This overload is the compositional equivalent
	/// of `Bind`, `FlatMap`, or `SelectMany` style functions available in other contexts
	/// </summary>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns another result with either the return value of the given function or the current failure</returns>
	public Result<TResult, TFailure> Then<TResult>(Func<T, Result<TResult, TFailure>> f) =>
		HasValue(out var value)
			? f(value)
			: Result<TResult, TFailure>.Failure(_failure);
	
	
	/// <summary>
	/// Executes the given function if the given Result was successful.  This overload is the compositional equivalent
	/// of `Bind`, `FlatMap`, or `SelectMany` style functions available in other contexts.
	///
	/// This is a special case for mitigating interop between Result&lt;T&gt; and Result&lt;T, TFailure&gt;.  Without
	/// this handler, the return type is: Result&lt;Result&lt;T&gt;, TFailure&gt;
	/// </summary>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns another result with either the return value of the given function or the current failure</returns>
	public Result<TResult, string> Then<TResult>(Func<T, Result<TResult>> f) =>
		HasValue(out var value)
			? f(value)
			: Result<TResult, TFailure>.Failure(_failure);
	
	/// <summary>
	/// Executes the given function if the given Result was successful.  This overload is the compositional equivalent
	/// of `Map` or `Select` style functions available in other contexts
	/// </summary>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns another result with either the return value of the given function or the current failure</returns>
	public Result<TResult, TFailure> Then<TResult>(Func<T, TResult> f) =>
		HasValue(out var value)
			? Result<TResult, TFailure>.Success(f(value))
			: Result<TResult, TFailure>.Failure(_failure);
	
	/// <summary>
	/// Executes the given action if the given Result was successful.  This overload is used to accomodate methods that
	/// have a void return signature
	/// </summary>
	/// <param name="f">The function to execute if the Result was successful</param>
	/// <returns>Returns the given Result after the action is executed</returns>
	public Result<T, TFailure> Then(Action<T> f)
	{
		if (HasValue(out var value))
			f(value);

		return this;
	}

	/// <summary>
	/// Used to transform a Failure state into a Success alternate value.
	/// </summary>
	/// <param name="alt">The function to provide an alternate value</param>
	/// <returns>If the current Result is a Failure, then the alternate value is applied</returns>
	public Result<T, TFailure> Else(T alt) =>
		_hasValue
			? this
			: Success(alt);
	
	/// <summary>
	/// Used to transform a Failure state into a Success alternate value.
	/// </summary>
	/// <param name="alt">The function to provide an alternate value</param>
	/// <returns>If the current Result is a Failure, then the alternate value is applied</returns>
	public Result<T, TFailure> Else(Func<T> alt) =>
		_hasValue
			? this
			: Success(alt());
	
	/// <summary>
	/// Used to deconstruct a Result to either it's successful value or a given alternative
	/// </summary>
	/// <param name="alt">The alternative value to return if the Result was not successful</param>
	/// <returns>If the Result is successful, the Result value is returned.  Otherwise, the alternate value is returned</returns>
	public T ValueOr(T alt) =>
		HasValue(out var value)
			? value
			: alt;
	
	/// <summary>
	/// Used to trade alternative Result values if the given Result is a failure
	/// </summary>
	/// <param name="alt">The function to return an alternate Result</param>
	/// <returns>If the given Result is successful, it will be returned.  Otherwise, the alternate Result is returned</returns>
	public Result<T, TFailure> Or(Result<T, TFailure> alt) =>
		_hasValue
			? this
			: alt;
	
	/// <summary>
	/// Used to trade alternative Result values if the given Result is a failure
	/// </summary>
	/// <param name="alt">The function to return an alternate Result</param>
	/// <returns>If the given Result is successful, it will be returned.  Otherwise, the alternate Result is returned</returns>
	public Result<T, TFailure> Or(Func<Result<T, TFailure>> alt) =>
		_hasValue
			? this
			: alt();
	
	/// <summary>
	/// Used to combine two Results if they are both successful
	/// </summary>
	/// <param name="other">The second result to combine</param>
	/// <param name="selector">The function used to combine the values from ResultA and B</param>
	/// <typeparam name="TOther">The type of the other Result</typeparam>
	/// <typeparam name="TResult">The type of the new Result</typeparam>
	/// <returns>If the Result was successful, the combined value of ResultA and B is returned.  Otherwise, the failure from either ResultA or B will be returned</returns>
	public Result<TResult, TFailure> And<TOther, TResult>(Result<TOther, TFailure> other, Func<T, TOther, TResult> selector)
	{
		if (HasFailure(out var failure))
			return Result<TResult, TFailure>.Failure(failure);

		return other.HasValue(out var value)
			? Result<TResult, TFailure>.Success(selector(_value, value))
			: Result<TResult, TFailure>.Failure(other._failure);
	}
	
	/// <summary>
	/// Used to combine two Results if they are both successful
	/// </summary>
	/// <param name="other">The second result to combine</param>
	/// <param name="selector">The function used to combine the values from ResultA and B</param>
	/// <typeparam name="TOther">The type of the other Result</typeparam>
	/// <typeparam name="TResult">The type of the new Result</typeparam>
	/// <returns>If the Result was successful, the combined value of ResultA and B is returned.  Otherwise, the failure from either ResultA or B will be returned</returns>
	public Result<TResult, TFailure> And<TOther, TResult>(Func<Result<TOther, TFailure>> other, Func<T, TOther, TResult> selector)
	{
		if (HasFailure(out var failure))
			return Result<TResult, TFailure>.Failure(failure);

		var bb = other();

		return bb.HasValue(out var value)
			? Result<TResult, TFailure>.Success(selector(_value, value))
			: Result<TResult, TFailure>.Failure(bb._failure);
	}
}

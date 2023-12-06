# CHANGE LOG

## 3.1

Added new `HasSuccess(out var value, out var error)` signature for the Result<T>.
This allows for cleaner decomposition when using imperative conditional logic.

Added new `Result` type with no generic value to specify no return type.  Underneath,
it is a `Result<None>` type.  This will make signature declarations cleaner.

## 3.0

Removed the `Func` overloads for the `ValueOr` function.  Calling `ValueOr(null)`
would be a very likely request for a developer to make and the compiler cannot
figure out which overload to call.

The solution is to relegate `ValueOr` to a primitives/object-only situation and
to use `Match` when functions or lazy evaluation is needed.  This reduces the 
number of overloads that exist without removing the ability to handle the 
scenario.

## v2.0

In general the library should look and feel very similar to what it's first 
iteration was.  The largest change is the renaming of the `Result&lt;T,F&gt;`
to `Either` in order to reserve the type name of `Result` for a the situation 
where you have a value or an error.  Having a more generic A or B situation has
now been delegated to the `Either` type.

### Result, Result2, & Either Types

* Inheritance between Result types is limiting and does not work entirely as
  hoped.  Binding method signatures are ambiguous and implicit conversions
  a kludge trying to force the hand of the type system.  Inheritance has been
  removed.
* Result&lt;T&gt; still exists and is the language owner of `Success` and 
  `Failure`.  It's right side has been change from a `string` to a new `Error`
  type that can be used for exceptions or simple failure messages.
* Result&lt;T,TFailure&gt; has been renamed as `Either` and uses the constructors
  `Value` or `Other`. 

### General Language

* `Then` (as it has been) is strictly used for `left` map/bind functions.
* `Else` is strictly used for `right` map/bind` functions.
* All other language is the same as before.

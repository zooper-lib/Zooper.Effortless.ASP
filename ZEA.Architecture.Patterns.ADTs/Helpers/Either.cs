// ReSharper disable MemberCanBePrivate.Global

namespace ZEA.Architecture.Patterns.ADTs.Helpers;

/// <summary>
/// Represents a disjoint union of two types, where an instance can hold a value of either the left or the right type.
/// This can be useful for representing a value that can have two possible types, similar to a Result or Option type.
/// </summary>
/// <typeparam name="TLeft">The type of the Left value.</typeparam>
/// <typeparam name="TRight">The type of the Right value.</typeparam>
public class Either<TLeft, TRight>
{
	/// <summary>
	/// Gets the Left value if it exists; otherwise, returns null.
	/// </summary>
	public TLeft? Left { get; }

	/// <summary>
	/// Gets the Right value if it exists; otherwise, returns null.
	/// </summary>
	public TRight? Right { get; }

	/// <summary>
	/// Indicates whether the instance holds a Left value.
	/// </summary>
	public bool IsLeft => Left != null;

	/// <summary>
	/// Indicates whether the instance holds a Right value.
	/// </summary>
	public bool IsRight => Right != null;

	/// <summary>
	/// Initializes a new instance of the <see cref="Either{TLeft, TRight}"/> class with a Left value.
	/// </summary>
	/// <param name="left">The Left value to initialize.</param>
	protected Either(TLeft left)
	{
		Left = left;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Either{TLeft, TRight}"/> class with a Right value.
	/// </summary>
	/// <param name="right">The Right value to initialize.</param>
	protected Either(TRight right)
	{
		Right = right;
	}

	/// <summary>
	/// Creates a new <see cref="Either{TLeft, TRight}"/> instance from a Left value.
	/// </summary>
	/// <param name="left">The Left value to wrap.</param>
	/// <returns>An <see cref="Either{TLeft, TRight}"/> instance containing the Left value.</returns>
	public static Either<TLeft, TRight> FromLeft(TLeft left) => new(left);

	/// <summary>
	/// Creates a new <see cref="Either{TLeft, TRight}"/> instance from a Right value.
	/// </summary>
	/// <param name="right">The Right value to wrap.</param>
	/// <returns>An <see cref="Either{TLeft, TRight}"/> instance containing the Right value.</returns>
	public static Either<TLeft, TRight> FromRight(TRight right) => new(right);

	/// <summary>
	/// Matches the current value to one of two possible functions and returns the result of the matched function.
	/// </summary>
	/// <typeparam name="T">The return type of the matching functions.</typeparam>
	/// <param name="leftFunc">The function to invoke if the instance holds a Left value.</param>
	/// <param name="rightFunc">The function to invoke if the instance holds a Right value.</param>
	/// <returns>The result of the invoked function.</returns>
	/// <exception cref="InvalidOperationException">Thrown if both Left and Right values are null.</exception>
	public T Match<T>(
		Func<TLeft, T> leftFunc,
		Func<TRight, T> rightFunc)
	{
		if (IsLeft)
		{
			return leftFunc(Left!);
		}

		if (IsRight)
		{
			return rightFunc(Right!);
		}

		throw new InvalidOperationException("Both Left and Right cannot be null.");
	}

	/// <summary>
	/// Matches the current value to one of two possible actions and invokes the matched action.
	/// </summary>
	/// <param name="leftAction">The action to invoke if the instance holds a Left value.</param>
	/// <param name="rightAction">The action to invoke if the instance holds a Right value.</param>
	/// <exception cref="InvalidOperationException">Thrown if both Left and Right values are null.</exception>
	public void Match(
		Action<TLeft> leftAction,
		Action<TRight> rightAction)
	{
		if (IsLeft)
		{
			leftAction(Left!);
		}

		if (IsRight)
		{
			rightAction(Right!);
		}

		throw new InvalidOperationException("Both Left and Right cannot be null.");
	}
}
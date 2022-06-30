/*
 *  Written by Abdullah Beyaztaş.
 *  Utility tools for applying functional coding style in c# language.
 */
namespace FP;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
///     The error
/// </summary>
[Serializable]
public sealed class Error
{
    #region Public Properties
    /// <summary>
    ///     Gets or sets the message.
    /// </summary>
    public string Message { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    ///     Performs an implicit conversion from <see cref="Exception" /> to <see cref="Error" />.
    /// </summary>
    public static implicit operator Error(Exception exception)
    {
        return new Error
        {
            Message = exception.ToString()
        };
    }

    /// <summary>
    ///     Performs an implicit conversion from <see cref="System.String" /> to <see cref="Error" />.
    /// </summary>
    public static implicit operator Error(string errorMessage)
    {
        return new Error
        {
            Message = errorMessage
        };
    }

    public override string ToString()
    {
        return Message;
    }
    #endregion
}

/// <summary>
///     The response
/// </summary>
[Serializable]
public class Response
{
    #region Static Fields
    /// <summary>
    ///     The success
    /// </summary>
    public static readonly Response Success = new();
    #endregion

    #region Fields
    /// <summary>
    ///     The errors
    /// </summary>
    protected readonly List<Error> errors = new();
    #endregion

    #region Public Properties
    /// <summary>
    ///     Gets the results.
    /// </summary>
    public IReadOnlyList<Error> Errors => errors;

    /// <summary>
    ///     Gets the fail message.
    /// </summary>
    public string FailMessage => string.Join(Environment.NewLine, errors.Select(e => e.Message));

    /// <summary>
    ///     Gets a value indicating whether this instance is fail.
    /// </summary>
    public bool IsFail => errors.Count > 0;

    /// <summary>
    ///     Gets a value indicating whether this instance is success.
    /// </summary>
    public bool IsSuccess => errors.Count == 0;
    #endregion

    #region Public Methods
    /// <summary>
    ///     Fails the specified error message.
    /// </summary>
    public static Response Fail(string errorMessage)
    {
        return new Response
        {
            errors = { errorMessage }
        };
    }

    public static Response operator +(Response responseX, Response responseY)
    {
        var response = new Response();

        response.errors.AddRange(responseX.Errors);
        response.errors.AddRange(responseY.Errors);

        return response;
    }

    /// <summary>
    ///     Performs an implicit conversion from <see cref="Exception" /> to <see cref="Response" />.
    /// </summary>
    public static implicit operator Response(Exception exception)
    {
        var response = new Response();

        response.errors.Add(exception);

        return response;
    }

    /// <summary>
    ///     Performs an implicit conversion from <see cref="Error" /> to <see cref="Response" />.
    /// </summary>
    public static implicit operator Response(Error error)
    {
        var response = new Response();

        response.errors.Add(error);

        return response;
    }
    #endregion
}

/// <summary>
///     The response
/// </summary>
[Serializable]
public sealed class Response<TValue> : Response
{
    #region Public Properties
    /// <summary>
    ///     Gets or sets the value.
    /// </summary>
    public TValue Value { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    ///     Performs an implicit conversion from <see cref="Exception" /> to <see cref="Response{TValue}" />.
    /// </summary>
    public static implicit operator Response<TValue>(Exception exception)
    {
        var response = new Response<TValue>();

        response.errors.Add(exception);

        return response;
    }

    /// <summary>
    ///     Performs an implicit conversion from <see cref="Error" /> to <see cref="Response{TValue}" />.
    /// </summary>
    public static implicit operator Response<TValue>(Error error)
    {
        var response = new Response<TValue>();

        response.errors.Add(error);

        return response;
    }

    public static implicit operator Response<TValue>(string error)
    {
        var response = new Response<TValue>();

        response.errors.Add(error);

        return response;
    }

    /// <summary>
    ///     Performs an implicit conversion from <see cref="Error" /> to <see cref="Response{TValue}" />.
    /// </summary>
    public static implicit operator Response<TValue>(Error[] errors)
    {
        var response = new Response<TValue>();

        response.errors.AddRange(errors);

        return response;
    }

    /// <summary>
    ///     Performs an implicit conversion from <see cref="TValue" /> to <see cref="Response{TValue}" />.
    /// </summary>
    public static implicit operator Response<TValue>(TValue value)
    {
        return new Response<TValue> { Value = value };
    }
    #endregion
}

public static class FpExtensions
{
    #region Public Methods
    public static Func<Response<C>> Compose<A, B, C>(Func<Response<A>> funcA, Func<A, Response<B>> funcB, Func<B, Response<C>> funcC)
    {
        return () =>
        {
            var responseA = funcA();
            if (responseA.IsFail)
            {
                return responseA.FailAs<C>();
            }

            var responseB = funcB(responseA.Value);
            if (responseB.IsFail)
            {
                return responseB.FailAs<C>();
            }

            return funcC(responseB.Value);
        };
    }

    public static Func<Response<D>> Compose<A, B, C, D>(Func<Response<A>> funcA, Func<A, Response<B>> funcB, Func<B, Response<C>> funcC, Func<C, Response<D>> funcD)
    {
        return () =>
        {
            var responseA = funcA();
            if (responseA.IsFail)
            {
                return responseA.FailAs<D>();
            }

            var responseB = funcB(responseA.Value);
            if (responseB.IsFail)
            {
                return responseB.FailAs<D>();
            }

            var responseC = funcC(responseB.Value);
            if (responseC.IsFail)
            {
                return responseC.FailAs<D>();
            }

            return funcD(responseC.Value);
        };
    }

    public static Func<Response<E>> Compose<A, B, C, D, E>(Func<Response<A>> funcA, Func<A, Response<B>> funcB, Func<B, Response<C>> funcC, Func<C, Response<D>> funcD, Func<D, Response<E>> funcE)
    {
        return () =>
        {
            var responseA = funcA();
            if (responseA.IsFail)
            {
                return responseA.FailAs<E>();
            }

            var responseB = funcB(responseA.Value);
            if (responseB.IsFail)
            {
                return responseB.FailAs<E>();
            }

            var responseC = funcC(responseB.Value);
            if (responseC.IsFail)
            {
                return responseC.FailAs<E>();
            }

            var responseD = funcD(responseC.Value);
            if (responseD.IsFail)
            {
                return responseD.FailAs<E>();
            }

            return funcE(responseD.Value);
        };
    }

    public static Func<Response<F>> Compose<A, B, C, D, E, F>(Func<Response<A>> funcA, Func<A, Response<B>> funcB, Func<B, Response<C>> funcC, Func<C, Response<D>> funcD, Func<D, Response<E>> funcE, Func<E, Response<F>> funcF)
    {
        return () =>
        {
            var responseA = funcA();
            if (responseA.IsFail)
            {
                return responseA.FailAs<F>();
            }

            var responseB = funcB(responseA.Value);
            if (responseB.IsFail)
            {
                return responseB.FailAs<F>();
            }

            var responseC = funcC(responseB.Value);
            if (responseC.IsFail)
            {
                return responseC.FailAs<F>();
            }

            var responseD = funcD(responseC.Value);
            if (responseD.IsFail)
            {
                return responseD.FailAs<F>();
            }

            var responseE = funcE(responseD.Value);
            if (responseE.IsFail)
            {
                return responseE.FailAs<F>();
            }

            return funcF(responseE.Value);
        };
    }

    public static Response<C> Pipe<TValue, A, B, C>(TValue value, Func<TValue, Response<A>> funcA, Func<A, Response<B>> funcB, Func<B, Response<C>> funcC)
    {
        return Compose(() => funcA(value), funcB, funcC)();
    }

    public static Response<D> Pipe<TValue, A, B, C, D>(TValue value, Func<TValue, Response<A>> funcA, Func<A, Response<B>> funcB, Func<B, Response<C>> funcC, Func<C, Response<D>> funcD)
    {
        return Compose(() => funcA(value), funcB, funcC, funcD)();
    }

    public static Response<E> Pipe<TValue, A, B, C, D, E>(TValue value, Func<TValue, Response<A>> funcA, Func<A, Response<B>> funcB, Func<B, Response<C>> funcC, Func<C, Response<D>> funcD, Func<D, Response<E>> funcE)
    {
        return Compose(() => funcA(value), funcB, funcC, funcD, funcE)();
    }

    public static Response<F> Pipe<TValue, A, B, C, D, E, F>(TValue value, Func<TValue, Response<A>> funcA, Func<A, Response<B>> funcB, Func<B, Response<C>> funcC, Func<C, Response<D>> funcD, Func<D, Response<E>> funcE, Func<E, Response<F>> funcF)
    {
        return Compose(() => funcA(value), funcB, funcC, funcD, funcE, funcF)();
    }

    public static Response<B> Then<A, B>(this Response<A> responseA, Func<A, Response<B>> onSuccess)
    {
        if (responseA.IsFail)
        {
            return responseA.FailAs<B>();
        }

        return onSuccess(responseA.Value);
    }

    public static T Unwrap<T>(this Response<T> response, Func<string, Exception> createException)
    {
        if (response.IsSuccess)
        {
            return response.Value;
        }

        throw createException(response.FailMessage);
    }

    public static T Unwrap<T>(this Response<T> response, Func<IReadOnlyList<Error>, Exception> createException)
    {
        if (response.IsSuccess)
        {
            return response.Value;
        }

        throw createException(response.Errors);
    }
    #endregion

    #region Methods
    static Response<T> FailAs<T>(this Response responseA)
    {
        return responseA.Errors.ToArray();
    }
    #endregion
}
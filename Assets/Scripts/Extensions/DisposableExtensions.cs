using System;
using R3;

namespace Runtime.Extensions
{
    public static class DisposableExtensions
    {
        /// <summary>
        /// Assigns this <see cref="IDisposable"/> to the specified <see cref="SerialDisposable"/>.
        /// </summary>
        /// <param name="disposable">This <see cref="IDisposable"/>.</param>
        /// <param name="serialDisposable">The <see cref="SerialDisposable"/> to assign this <see cref="IDisposable"/> to.</param>
        /// <returns>This <see cref="IDisposable"/></returns>
        public static IDisposable AssignTo(this IDisposable disposable, SerialDisposable serialDisposable)
        {
            serialDisposable.Disposable = disposable;
            return disposable;
        }
    }
}
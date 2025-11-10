using System;
using R3;

namespace Runtime.Extensions
{
    public static class ObservableExtensions
    {
        /// <summary>
        /// An overload to Subscribe that invokes a parameter-less action. 
        /// </summary>
        /// <param name="observable"></param>
        /// <param name="onNext"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IDisposable Subscribe<T>(this Observable<T> observable, Action onNext) => 
            observable.Subscribe(_ => onNext());

        public static void OnNext(this Subject<Unit> subject)
        {
            subject.OnNext(Unit.Default);
        }
    }
}
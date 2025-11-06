using System;
using R3;

namespace Runtime.Extensions
{
    /// <summary>
    ///  I fucking love RORPing
    /// </summary>
    public class RORP<T>
    {
        private RORP() => rp = new ReactiveProperty<T>(default);
        private RORP(T initial) => rp = new ReactiveProperty<T>(initial);
        private readonly ReactiveProperty<T> rp = new();
        public ReadOnlyReactiveProperty<T> RP => rp;
        public T NewValue
        {
            set => rp.Value = value;
        }
        public void Set(T newValue) => rp.Value = newValue;
        private T Get => rp.CurrentValue;
        public static implicit operator T(RORP<T> rorp) => rorp.Get;
        public static implicit operator ReadOnlyReactiveProperty<T>(RORP<T> rorp) => rorp.RP;
        public static implicit operator RORP<T>(T value) => new(value);
        /// <summary>
        /// SUBSCRIBE TO MY FUCKING RORP
        /// </summary>
        public IDisposable Subscribe(Action<T> onNext) => RP.Subscribe(onNext);
    }
}
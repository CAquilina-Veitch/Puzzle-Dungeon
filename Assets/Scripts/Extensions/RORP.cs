using System;
using R3;

namespace Runtime.Extensions
{
    /// <summary>
    ///  I fucking love RORPing
    /// </summary>
    public class RORP<T>
    {
        public RORP() => rp = new ReactiveProperty<T>(default);
        public RORP(T initial) => rp = new ReactiveProperty<T>(initial);
        private readonly ReactiveProperty<T> rp = new();
        public ReadOnlyReactiveProperty<T> RP => rp;
        public T Get => NewValue;
        public void Set(T newValue) => NewValue = newValue;
        public T NewValue
        {
            get => rp.CurrentValue;
            set
            {
                if (!Equals(value, rp.CurrentValue)) 
                    lastValue = rp.CurrentValue;
                rp.Value = value; 
            }
        }

        public static implicit operator T(RORP<T> rorp) => rorp.Get;
        public static implicit operator ReadOnlyReactiveProperty<T>(RORP<T> rorp) => rorp.RP;
        public static implicit operator RORP<T>(T value) => new(value);
        /// <summary>
        /// SUBSCRIBE TO MY FUCKING RORP
        /// </summary>
        public IDisposable Subscribe(Action<T> onNext) => RP.Subscribe(onNext);

        private T lastValue;
        public T LastValue => lastValue;
    }
}
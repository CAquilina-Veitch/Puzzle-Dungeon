using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Extensions
{
    [Serializable] public class EnumPair<TEnum, T> where TEnum : Enum
    {
        public TEnum enumKey;
        public T Value;

        public EnumPair(TEnum enumKey, T value)
        {
            this.enumKey = enumKey;
            Value = value;
        }
    }
}
using UnityEngine;

namespace Scripts.UI.Buttons
{
    public class ValuedUIButton<T> : UIButton
    {
        [SerializeField] private T value;
        public T Value => value;
    }
}
using System.Linq;
using R3;
using UnityEngine;

namespace Runtime.Extensions
{
    public class InputAsObservable
    {
        public InputAsObservable(KeyCode key)
        {
            Observable.EveryUpdate()
                .Subscribe(_ => {
                    if (Input.GetKeyDown(key)) isPressed.Value = true;
                    else if (Input.GetKeyUp(key)) isPressed.Value = false;
                })
                .AddTo(disposable);
        }
    
        public InputAsObservable(KeyCode[] keys)
        {
            if (keys == null || keys.Length == 0) return;
        
            Observable.EveryUpdate()
                .Subscribe(_ => {
                    if (keys.Any(Input.GetKeyDown))
                        isPressed.Value = true;
                
                    else if (keys.Any(Input.GetKeyUp) && !keys.Any(Input.GetKey))
                        isPressed.Value = false;
                })
                .AddTo(disposable);
        }

        ~InputAsObservable() => disposable.Clear();
        
        public ReadOnlyReactiveProperty<bool> IsPressed => isPressed;
        private readonly ReactiveProperty<bool> isPressed = new();
        private readonly CompositeDisposable disposable = new();
    
    }
}
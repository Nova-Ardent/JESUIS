using System.Collections;
using UnityEngine;

namespace JESUIS.Runtime.Screen
{
    public class BaseScreenController
    {
        public ScreenStackContainer ScreenStack;

        public virtual void OnLoad() { }
        public virtual IEnumerator OnLoadAsync() { yield break; }

        public virtual void OnReturn() { }
        public virtual IEnumerator OnReturnAsync() { yield break; }

        public virtual void OnUpdate() { }

        public virtual void OnUnload() { }
        public virtual IEnumerator OnUnloadAsync() { yield break; }
    }
}

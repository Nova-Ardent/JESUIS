using JESUIS.Runtime.Screen;
using UnityEngine;
using System;
using System.Collections;

namespace JESUIS.Runtime
{
    public class UIRunner : MonoBehaviour
    {
        [SerializeField] GameObject screenContainer;
        [SerializeField] GameObject persistentScreenContainer;

        public ScreenStackContainer ScreenStack { get; private set; }
        public static UIRunner Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                throw new Exception("There can only be one UI Runner Instance");
            }

            ScreenStack = new ScreenStackContainer(screenContainer, persistentScreenContainer);
            ScreenStack.Initialize();

            DontDestroyOnLoad(this.gameObject);
            StartCoroutine(AnsyncFlow());
        }

        private void OnDestroy()
        {
            
        }

        private void Update()
        {
            ScreenStack.Update();
        }

        IEnumerator AnsyncFlow()
        {
            while (true)
            {
                yield return ScreenStack.AsyncUpdate();
                yield return null;
            }
        }
    }
}

using JESUIS.Runtime.Screen.Layout;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Collections;

namespace JESUIS.Runtime.Screen
{
    public class ScreenStackContainer
    {
        LayoutBuilder layoutBuilder = new LayoutBuilder();

        GameObject screenContainer;
        GameObject persistentScreenContainer;

        Queue<IEnumerator> QueuedAsyncEvent = new Queue<IEnumerator>();
        public BaseScreenController CurrentScreen { get; private set; }
        Stack<BaseScreenController> screenStack = new Stack<BaseScreenController>();

        public ScreenStackContainer(GameObject screenContainer, GameObject persistentScreenContainer)
        {
            this.screenContainer = screenContainer;
            this.persistentScreenContainer = persistentScreenContainer;
        }

        public void Update()
        {
            if (CurrentScreen != null)
            {
                CurrentScreen.OnUpdate();
            }
        }

        public IEnumerator AsyncUpdate()
        {
            if (QueuedAsyncEvent.Count > 0)
            {
                yield return QueuedAsyncEvent.Dequeue();
            }
        }

        public void Initialize()
        {
            layoutBuilder.Initialize(screenContainer);
        }

        public void LoadScreen(BaseScreenController screen, bool clearStack = false)
        {
            layoutBuilder.ClearLayout();
            if (clearStack)
            {
                foreach (BaseScreenController screenController in screenStack)
                {
                    UnloadScreen(screenController);
                }
                screenStack.Clear();

                if (screen != null)
                {
                    screenStack.Push(screen);
                    CurrentScreen = screen;

                    BuildScreen(screen);
                }
            }
            else
            {
                if (screen == null)
                {
                    UnloadScreen(screenStack.Pop());
                    if (screenStack.Count > 0)
                    {
                        CurrentScreen = screenStack.Peek();
                        OnScreenPoppedTo(CurrentScreen);
                    }
                }
                else
                {
                    if (screenStack.Any(x => x.GetType() == screen.GetType()))
                    {
                        do
                        {
                            BaseScreenController stackScreen = screenStack.Pop();
                            UnloadScreen(stackScreen);

                            if (screenStack.GetType() == screen.GetType())
                            {
                                screenStack.Push(screen);
                                BuildScreen(screen);
                                break;
                            }

                            if (screenStack.Count == 0)
                            {
                                throw new System.Exception("Error finding screen in screen stack. Something wrong had happened.");
                            }

                        } while (true);
                    }
                    else
                    {
                        screenStack.Push(screen);
                        CurrentScreen = screen;
                        BuildScreen(screen);
                    }
                }
            }
        }

        void OnScreenPoppedTo(BaseScreenController screen)
        {
            if (screen == null)
                return;

            screen.OnReturn();
            QueuedAsyncEvent.Enqueue(screen.OnReturnAsync());
        }

        void BuildScreen(BaseScreenController screen)
        {
            ScreenLayoutAttribute screenLayoutAttribute = screen.GetType().GetCustomAttribute<ScreenLayoutAttribute>();
            if (screenLayoutAttribute == null)
            {
                throw new System.Exception("Screen " + screen.GetType().Name + " does not have a ScreenLayout attribute.");
            }

            screen.ScreenStack = this;
            screen.OnLoad();
            QueuedAsyncEvent.Enqueue(screen.OnLoadAsync());

            layoutBuilder.BuildLayout(screenLayoutAttribute.Guid);
        }

        void UnloadScreen(BaseScreenController screen)
        {
            if (screen == null) 
                return;

            screen.ScreenStack = null;
            screen.OnUnload();
            QueuedAsyncEvent.Enqueue(screen.OnUnloadAsync());
        }
    }
}

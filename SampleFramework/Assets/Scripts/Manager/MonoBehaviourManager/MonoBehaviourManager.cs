using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public delegate void MonoUpdateMethod();

    public enum UpdateMethodType
    {
        Update,
        FixUpdate,
        LateUpdate,
        OnGUI
    }

    public class MonoBehaviourManager : MonoSingleton<MonoBehaviourManager>
    {
        private List<MonoUpdateMethod> updateMethods;
        private List<MonoUpdateMethod> fixedUpdateMethods;
        private List<MonoUpdateMethod> lateUpdateMethods;
        private List<MonoUpdateMethod> onGUIMethods;

        private void Update()
        {
            if (updateMethods != null)
            {
                for (int i = 0; i < updateMethods.Count; i++)
                {
                    updateMethods[i].Invoke();
                }
            }
        }

        private void FixedUpdate()
        {
            if (fixedUpdateMethods != null)
            {
                for (int i = 0; i < fixedUpdateMethods.Count; i++)
                {
                    fixedUpdateMethods[i].Invoke();
                }
            }
        }

        private void LateUpdate()
        {
            if (lateUpdateMethods != null)
            {
                for (int i = 0; i < lateUpdateMethods.Count; i++)
                {
                    lateUpdateMethods[i].Invoke();
                }
            }
        }

        private void OnGUI()
        {
            if (onGUIMethods != null)
            {
                for (int i = 0; i < onGUIMethods.Count; i++)
                {
                    onGUIMethods[i].Invoke();
                }
            }
        }

        public override void Init()
        {
            base.Init();
            updateMethods = new List<MonoUpdateMethod>();
            fixedUpdateMethods = new List<MonoUpdateMethod>();
            lateUpdateMethods = new List<MonoUpdateMethod>();
            onGUIMethods = new List<MonoUpdateMethod>();
        }

        public override void Release()
        {
            updateMethods = null;
            fixedUpdateMethods = null;
            lateUpdateMethods = null;
            onGUIMethods = null;

            base.Release();
        }

        public void Register(MonoUpdateMethod method, UpdateMethodType methodType)
        {
            switch (methodType)
            {
                case UpdateMethodType.Update:
                    AddMethod(updateMethods, method);
                    break;
                case UpdateMethodType.FixUpdate:
                    AddMethod(fixedUpdateMethods, method);
                    break;
                case UpdateMethodType.LateUpdate:
                    AddMethod(lateUpdateMethods, method);
                    break;
                case UpdateMethodType.OnGUI:
                    AddMethod(onGUIMethods, method);
                    break;
            }
        }

        public void UnRegister(MonoUpdateMethod method, UpdateMethodType methodType)
        {
            switch (methodType)
            {
                case UpdateMethodType.Update:
                    RemoveMethod(updateMethods, method);
                    break;
                case UpdateMethodType.FixUpdate:
                    RemoveMethod(fixedUpdateMethods, method);
                    break;
                case UpdateMethodType.LateUpdate:
                    RemoveMethod(lateUpdateMethods, method);
                    break;
                case UpdateMethodType.OnGUI:
                    RemoveMethod(onGUIMethods, method);
                    break;
            }
        }

        public static IEnumerator DelayAction(Action action, float time = 0)
        {
            if (time <= 0)
            {
                yield return new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForSeconds(time);
            }

            action?.Invoke();
        }

        private void AddMethod(List<MonoUpdateMethod> method, MonoUpdateMethod target)
        {
            if (method != null && !method.Contains(target))
            {
                method.Add(target);
            }
        }

        private void RemoveMethod(List<MonoUpdateMethod> method, MonoUpdateMethod target)
        {
            if (method != null && method.Contains(target))
            {
                method.Remove(target);
            }
        }
    }

}
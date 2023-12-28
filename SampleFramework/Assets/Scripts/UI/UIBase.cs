using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public class UIBase : BaseMonoBehaviour
    {
        public string UIName => uiName;

        private string guid;

        public string Guid => guid;

        private string uiName;

        public Dictionary<string, System.Object> Param = new Dictionary<string, System.Object>();

        private bool isOpened;

        public bool IsOpened => isOpened;

        public virtual void Register()
        {

        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
            isOpened = true;
        }

        public virtual void UnRegister()
        {

        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
            isOpened = false;
        }

        public void InitUI(string uiName)
        {
            guid = System.Guid.NewGuid().ToString("N");
            this.uiName = uiName;
        }
    }

}
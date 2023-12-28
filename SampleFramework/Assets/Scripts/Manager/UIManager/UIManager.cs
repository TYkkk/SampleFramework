using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public class UIManager : Singleton<UIManager>
    {
        public UIRoot UIRoot => uiRoot;

        private UIRoot uiRoot;

        private const string uiRootPath = "UI/UIRoot";

        private UIRoot uiRootTemplate;

        private static Dictionary<string, UIBase> loadedUIDict;

        public override void Init()
        {
            base.Init();

            loadedUIDict = new Dictionary<string, UIBase>();

            InitUIRoot();
        }

        public override void Release()
        {
            base.Release();

            loadedUIDict = null;

            ClearUIRoot();
        }

        private void InitUIRoot()
        {
            if (uiRootTemplate == null)
            {
                uiRootTemplate = Resources.Load<UIRoot>(uiRootPath);
            }

            if (uiRootTemplate == null)
            {
                MDebug.LogError("uiRootTemplate Load Error");
                return;
            }

            ClearUIRoot();

            GameObject UIRootObj = Object.Instantiate(uiRootTemplate.gameObject);
            uiRoot = UIRootObj.GetComponent<UIRoot>();
        }

        private void ClearUIRoot()
        {
            if (uiRoot != null)
            {
                Object.Destroy(uiRoot.gameObject);
            }
        }

        public UIBase OpenUI(string uiName, Dictionary<string, System.Object> param = null)
        {
            UIBase uiBase = GetOrCreateUI(uiName);

            if (uiBase == null)
            {
                return null;
            }

            if (uiBase.IsOpened)
            {
                return uiBase;
            }

            if (param != null)
            {
                foreach (var child in param.Keys)
                {
                    if (!uiBase.Param.ContainsKey(child))
                    {
                        uiBase.Param.Add(child, param[child]);
                    }
                    else
                    {
                        MDebug.LogWarning($"{uiBase.name} paramKey:{child} Replace");
                        uiBase.Param[child] = param[child];
                    }
                }
            }

            uiBase.transform.SetParent(UIRoot.LayerRoot.GetChild((int)UIDataSetting.UIDataSettingDict[uiName].Layer), false);

            uiBase.Register();

            uiBase.Open();

            return uiBase;
        }

        private UIBase GetOrCreateUI(string uiName)
        {
            if (loadedUIDict.ContainsKey(uiName))
            {
                if (loadedUIDict[uiName] != null)
                {
                    return loadedUIDict[uiName];
                }
            }

            GameObject createUI = Object.Instantiate(Resources.Load<GameObject>(UIDataSetting.UIDataSettingDict[uiName].UIPath));

            UIBase uiBase = createUI.GetComponent<UIBase>();

            uiBase.InitUI(uiName);

            if (!loadedUIDict.ContainsKey(uiName))
            {
                loadedUIDict.Add(uiName, uiBase);
            }

            return uiBase;
        }

        public void HideUI(string uiName)
        {
            if (!loadedUIDict.ContainsKey(uiName))
            {
                return;
            }

            HideUI(loadedUIDict[uiName]);
        }

        public void HideUI(UIBase uiBase)
        {
            uiBase.UnRegister();

            uiBase.Close();
        }

        public void CloseUI(string uiName)
        {
            if (!loadedUIDict.ContainsKey(uiName))
            {
                return;
            }

            CloseUI(loadedUIDict[uiName]);
        }

        public void CloseUI(UIBase uiBase)
        {
            uiBase.UnRegister();

            uiBase.Close();

            loadedUIDict.Remove(uiBase.UIName);

            Object.Destroy(uiBase.gameObject);
        }
    }


    public class UIData
    {
        public string UIName;
        public string UIPath;
        public UILayer Layer;

        public UIData(string uiName, string uiPath, UILayer layer, bool multi = false)
        {
            UIName = uiName;
            UIPath = uiPath;
            this.Layer = layer;
        }
    }

    public enum UILayer
    {
        Bottom,
        Layer01,
        Layer02,
        Layer03,
        Pop,
        Top,
        System,
    }
}
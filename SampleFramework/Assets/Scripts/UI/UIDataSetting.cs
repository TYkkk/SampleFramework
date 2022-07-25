using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public static class UIDataSetting
    {
        public static Dictionary<string, UIData> UIDataSettingDict = new Dictionary<string, UIData>()
        {

        };

        public static UIData GetUIData(string uiName)
        {
            if (UIDataSettingDict.ContainsKey(uiName))
            {
                return UIDataSettingDict[uiName];
            }

            return null;
        }
    }
}

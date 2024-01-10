using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableCellTextTemplate : TableCellTemplate
{
    public Text ContentText;
    public Image BgImg;

    public override void SetData(TableCellData data, Dictionary<string, object> param)
    {
        base.SetData(data, param);
        ContentText.text = data.Content;
        if (param != null)
        {
            if (param.ContainsKey(TableRowTemplate.BgColorParam))
            {
                BgImg.color = (Color)param[TableRowTemplate.BgColorParam];
            }

            if (param.ContainsKey(TableRowTemplate.TextColorParam))
            {
                ContentText.color = (Color)param[TableRowTemplate.TextColorParam];
            }
        }
    }
}

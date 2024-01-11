using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableCellTextTemplate : TableCellTemplate
{
    public Text ContentText;
    public Image BgImg;

    public override void SetData(TableRowData rowData, TableCellData cellData, Dictionary<string, object> param)
    {
        base.SetData(rowData, cellData, param);

        ContentText.text = cellData.Content;
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TableCellCustom01Template : TableCellTemplate
{
    public delegate void CustomBtnEvent(TableRowData rowData);

    public const string BtnClickedEvents = "BtnClickedEvents";

    public Button[] Btns;

    public override void SetData(TableRowData rowData, TableCellData cellData, Dictionary<string, object> param)
    {
        base.SetData(rowData, cellData, param);

        if (param != null && param.ContainsKey(BtnClickedEvents))
        {
            List<CustomBtnEvent> actions = param[BtnClickedEvents] as List<CustomBtnEvent>;
            for (int i = 0; i < actions.Count; i++)
            {
                int index = i;
                if (index < Btns.Length)
                {
                    Btns[index].onClick.RemoveAllListeners();
                    Btns[index].onClick.AddListener(() =>
                    {
                        actions[index]?.Invoke(rowData);
                    });
                }
            }
        }
    }
}

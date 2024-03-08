using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TableRowTemplate : BaseFramework.BaseMonoBehaviour
{
    public RectTransform MainRect;
    public Button SelectBtn;

    public TableCellTextTemplate CellTextTemplate;
    public TableCellCustom01Template CellCustom01Template;
    public TableCellImageTemplate CellImageTemplate;

    public const string BgColorParam = "BgColor";
    public const string TextColorParam = "TextColor";

    public delegate void SetCellMethod(TableRowData tableRowData, TableCellTemplate cellTemplate, TableCellData cellData);
    public SetCellMethod CustomSetCellMethod;

    public Transform CellRoot;
    public GameObject SelectFalg;

    TableRowData rowData;
    public TableRowData RowData => rowData;

    TableCellTemplate[] loadedCellTemplates;

    public void SetData(TableRowData data, Action<TableRowTemplate> selectAction)
    {
        rowData = data;

        MainRect.anchoredPosition = new Vector2(0, -rowData.RowIndex * rowData.Height);
        MainRect.sizeDelta = new Vector2(rowData.Width, rowData.Height);

        SelectBtn.onClick.RemoveAllListeners();
        SelectBtn.onClick.AddListener(() => { selectAction?.Invoke(this); });

        DrawCell();
    }

    private void DrawCell()
    {
        if (loadedCellTemplates != null && loadedCellTemplates.Length > 0)
        {
            for (int i = 0; i < loadedCellTemplates.Length; i++)
            {
                Destroy(loadedCellTemplates[i].gameObject);
            }
            loadedCellTemplates = null;
        }

        loadedCellTemplates = new TableCellTemplate[rowData.CellDatas.Length];

        float totalWidth = 0;
        for (int i = 0; i < rowData.CellDatas.Length; i++)
        {
            var CellTemplate = CreateCellTemplate(rowData.CellDatas[i].Type);
            if (CellTemplate == null)
            {
                continue;
            }

            var rectTransform = CellTemplate.gameObject.GetComponent<RectTransform>();
            rectTransform.SetParent(CellRoot, false);
            rectTransform.anchoredPosition = new Vector2(totalWidth, 0);
            rectTransform.sizeDelta = new Vector2(rowData.CellDatas[i].Width, rowData.CellDatas[i].Height);
            totalWidth += rowData.CellDatas[i].Width;
            CustomSetCellMethod?.Invoke(rowData, CellTemplate, rowData.CellDatas[i]);
            loadedCellTemplates[i] = CellTemplate;
        }
    }

    private TableCellTemplate CreateCellTemplate(int type)
    {
        switch (type)
        {
            case 1:
                return Instantiate(CellTextTemplate);
            case 2:
                return Instantiate(CellCustom01Template);
            case 3:
                return Instantiate(CellTextTemplate);
            case 4:
                return Instantiate(CellImageTemplate);
            default:
                return null;
        }
    }

    public void SetSelectFlag(bool isShow)
    {
        SelectFalg.SetActive(isShow);
    }
}

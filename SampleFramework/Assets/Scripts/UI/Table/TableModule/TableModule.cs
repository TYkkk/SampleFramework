using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableModule : BaseFramework.BaseMonoBehaviour
{
    public int RowCount;
    public int ColumnCount;

    public Color BgColor = Color.white;
    public Color LineColor = Color.black;

    public bool IsCustomColumnWidth = false;
    public float[] CustomColumnWidths;

    public bool IsCustomRowHeight = false;
    public float CustomRowHeight;

    public RectTransform TableRoot;
    public bool ResetRootHeight = false;

    public TableRowTemplate RowTemplate;
    public TableRowTemplate.SetCellMethod CustomSetCellMethod;

    private TableRowData[] rowDats;
    private List<TableRowTemplate> loadedTableRows;

    private RectTransform tableLineRoot;
    private RectTransform tableTemplateRoot;

    private float maxHeight;

    private Action<TableRowTemplate> rowTemplateSelectAction;

    public void DrawTable()
    {
        ClearTable();
        Draw();
    }

    public void ClearTable()
    {
        ClearLoadedRowTemplate();
        if (tableLineRoot != null)
            Destroy(tableLineRoot.gameObject);
        if (tableTemplateRoot != null)
            Destroy(tableTemplateRoot.gameObject);
    }

    public void SetDataAndMethod(TableRowData[] rowDats, TableRowTemplate.SetCellMethod method, Action<TableRowTemplate> selectAction = null)
    {
        this.rowDats = rowDats;
        CustomSetCellMethod = method;
        rowTemplateSelectAction = selectAction;
    }

    private void Draw()
    {
        if (TableRoot == null)
        {
            TableRoot = CreateObjectRoot("TableRoot", transform, BgColor);
        }

        tableTemplateRoot = CreateObjectRoot("TableTemplateRoot", TableRoot, Color.clear);

        maxHeight = TableRoot.rect.height;
        if (IsCustomRowHeight)
        {
            maxHeight = CustomRowHeight * RowCount;
        }

        if (ResetRootHeight)
        {
            TableRoot.sizeDelta = new Vector2(TableRoot.sizeDelta.x, maxHeight);
        }

        DrawLine();
        DrawTableTemplate();
    }

    private RectTransform CreateObjectRoot(string name, Transform parent, Color BgColor)
    {
        var root = new GameObject(name);
        root.transform.SetParent(parent, false);
        var rectTransform = root.AddComponent<RectTransform>();
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.sizeDelta = Vector2.zero;
        if (BgColor.a != 0)
            rectTransform.gameObject.AddComponent<RawImage>().color = BgColor;
        return rectTransform;
    }

    private void DrawLine()
    {
        tableLineRoot = CreateObjectRoot("TableLineRoot", TableRoot, Color.clear);
        CreateLine(TableRoot.rect.width, 0, 0);
        CreateLine(maxHeight, 0, 0, false);
        CreateLine(TableRoot.rect.width, 0, -maxHeight);
        CreateLine(maxHeight, TableRoot.rect.width, 0, false);

        if (RowCount > 0)
        {
            for (int i = 1; i < RowCount; i++)
            {
                CreateLine(TableRoot.rect.width, 0, -i * maxHeight / RowCount);
            }
        }

        if (ColumnCount > 0)
        {
            float tempTotalWidth = 0;
            for (int i = 0; i < ColumnCount - 1; i++)
            {
                if (IsCustomColumnWidth)
                {
                    if (i < CustomColumnWidths.Length)
                    {
                        CreateLine(maxHeight, tempTotalWidth + CustomColumnWidths[i], 0, false);
                        tempTotalWidth += CustomColumnWidths[i];
                    }
                    else
                    {
                        var next = (TableRoot.rect.width - tempTotalWidth) / (ColumnCount - i);
                        CreateLine(maxHeight, tempTotalWidth + next, 0, false);
                        tempTotalWidth += next;
                    }
                }
                else
                {
                    CreateLine(maxHeight, (i + 1) * TableRoot.rect.width / ColumnCount, 0, false);
                }
            }
        }
    }

    private void DrawTableTemplate()
    {
        ClearLoadedRowTemplate();

        float templateHeight = maxHeight / RowCount;
        float[] templateWidths = new float[ColumnCount];
        float tempTotal = 0;
        for (int i = 0; i < templateWidths.Length; i++)
        {
            if (IsCustomColumnWidth)
            {
                if (i < templateWidths.Length - 1 && i < CustomColumnWidths.Length)
                {
                    templateWidths[i] = CustomColumnWidths[i];
                    tempTotal += templateWidths[i];
                }
                else
                {
                    templateWidths[i] = (TableRoot.rect.width - tempTotal) / (ColumnCount - i);
                    tempTotal += templateWidths[i];
                }
            }
            else
            {
                templateWidths[i] = TableRoot.rect.width / ColumnCount;
            }
        }

        CreateTableRowTemplate(SetRowData(templateWidths, templateHeight));
    }

    private TableRowData[] SetRowData(float[] widths, float height)
    {
        if (rowDats == null)
        {
            return null;
        }

        for (int i = 0; i < rowDats.Length; i++)
        {
            if (rowDats[i] == null)
            {
                continue;
            }
            rowDats[i].Width = tableTemplateRoot.rect.width;
            rowDats[i].Height = maxHeight / RowCount;
            for (int j = 0; j < rowDats[i].CellDatas.Length; j++)
            {
                rowDats[i].CellDatas[j].Width = widths[j];
                rowDats[i].CellDatas[j].Height = height;
            }
        }

        return rowDats;
    }

    private void CreateTableRowTemplate(TableRowData[] datas)
    {
        if (datas == null)
        {
            return;
        }

        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i] == null)
            {
                continue;
            }

            TableRowTemplate tableRowTemplate = Instantiate(RowTemplate, tableTemplateRoot);
            tableRowTemplate.CustomSetCellMethod = CustomSetCellMethod;
            tableRowTemplate.SetData(datas[i], rowTemplateSelectAction);
            if (loadedTableRows == null)
            {
                loadedTableRows = new List<TableRowTemplate>();
            }

            loadedTableRows.Add(tableRowTemplate);
        }
    }

    private void ClearLoadedRowTemplate()
    {
        if (loadedTableRows != null && loadedTableRows.Count > 0)
        {
            for (int i = 0; i < loadedTableRows.Count; i++)
            {
                Destroy(loadedTableRows[i].gameObject);
            }

            loadedTableRows.Clear();
        }
    }

    private RectTransform CreateLine(float length, float posX, float posY, bool isRow = true)
    {
        float lineWidth = 2;
        var lineObject = new GameObject("line");
        lineObject.transform.SetParent(tableLineRoot, false);
        var lineRect = lineObject.AddComponent<RectTransform>();
        lineRect.anchorMax = new Vector2(0, 1);
        lineRect.anchorMin = new Vector2(0, 1);
        if (isRow)
        {
            lineRect.pivot = new Vector2(0, 0.5f);
            lineRect.sizeDelta = new Vector2(length, lineWidth);
        }
        else
        {
            lineRect.pivot = new Vector2(0.5f, 1);
            lineRect.sizeDelta = new Vector2(lineWidth, length);
        }
        lineRect.anchoredPosition = new Vector2(posX, posY);
        lineObject.AddComponent<Image>().color = LineColor;
        return lineRect;
    }
}

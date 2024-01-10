using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableRowData
{
    public int RowIndex;
    public float Width;
    public float Height;
    public TableCellData[] CellDatas;
    public object Data;

    public TableRowData(int index, TableCellData[] data)
    {
        RowIndex = index;
        CellDatas = data;
    }
}

public class TableCellData
{
    public float Width;
    public float Height;
    /// <summary>
    /// 1:常规文本 2:自定义按钮 3:标题文本 4:图像
    /// </summary>
    public int Type;
    public string Content;
}
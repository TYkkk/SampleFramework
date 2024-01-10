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
    /// 1:�����ı� 2:�Զ��尴ť 3:�����ı� 4:ͼ��
    /// </summary>
    public int Type;
    public string Content;
}
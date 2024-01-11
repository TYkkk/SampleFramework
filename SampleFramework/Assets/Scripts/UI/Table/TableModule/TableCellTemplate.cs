using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCellTemplate : BaseFramework.BaseMonoBehaviour
{
    public TableRowData RowData;
    public TableCellData CellData;

    public virtual void SetData(TableRowData rowData, TableCellData cellData, Dictionary<string, object> param)
    {
        RowData = rowData;
        CellData = cellData;
    }
}

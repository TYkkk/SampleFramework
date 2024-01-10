using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCellTemplate : BaseFramework.BaseMonoBehaviour
{
    public TableCellData CellData;

    public virtual void SetData(TableCellData data, Dictionary<string, object> param)
    {
        CellData = data;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GoundBackItem : MonoBehaviour
{
    /// <summary>
    /// �ýڵ�����������
    /// </summary>
    public Vector2Int ItemPosition;
    public List<Surface> SurfacesList = new List<Surface>();

    /// <summary>
    /// ��
    /// </summary>
    /// <param name="surfacesList"></param>
    /// <param name="isOn"></param>
    public void EntrIntoWarehouse(List<Surface> surfacesList,bool isOn=false) {
        for (int i = 0; i < surfacesList.Count; i++)
        {
            
        }
    }
}

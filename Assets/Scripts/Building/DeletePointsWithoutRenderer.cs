using UnityEngine;

namespace Building
{
    [ExecuteInEditMode]
    public class DeletePointsWithoutRenderer : MonoBehaviour
    { 
        void Awake()
        {
            foreach (Transform child in transform)
            {
                GridPoint gridPoint = child.GetComponent<GridPoint>();
                if (gridPoint.GetSpriteRenderer() == null)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}

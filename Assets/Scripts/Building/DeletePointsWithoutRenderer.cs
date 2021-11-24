using UnityEngine;

namespace Building
{
    [ExecuteInEditMode]
    public class DeletePointsWithoutRenderer : MonoBehaviour
    {
        // Start is called before the first frame update
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

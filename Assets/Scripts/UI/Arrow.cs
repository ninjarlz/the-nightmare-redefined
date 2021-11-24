using GameLogic;
using UnityEngine;

namespace UI
{
    public class Arrow : MonoBehaviour
    {
        private bool isSet = false;
        private GameObject target;
        void Update()
        {
            if (isSet)
            {
                Vector3 targetPosition = target.transform.position;
                targetPosition.y = transform.position.y;
                transform.LookAt(targetPosition);
            }
        }

        public void setTarget()
        {
            target = GameManager.Instance.currentChest;
            isSet = true;
        }
    }
}

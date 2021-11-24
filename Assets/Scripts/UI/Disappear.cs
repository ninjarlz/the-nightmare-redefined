using System.Collections;
using UnityEngine;

namespace UI
{
    public class Disappear : MonoBehaviour
    {
        public float time;
        void Start()
        {
            StartCoroutine(Fade());
        }

        private IEnumerator Fade()
        {
            yield return new WaitForSeconds(time);
            Destroy(gameObject);
        }
    }
}

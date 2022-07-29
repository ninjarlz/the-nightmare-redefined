using UnityEngine;

namespace Enemy
{
    public class DissolveEnemyBody : MonoBehaviour
    {
        private const string DissolveAmountShaderProp = "_DissolveAmount";
        private static readonly int DissolveAmountID = Shader.PropertyToID(DissolveAmountShaderProp);

        private Material _material;
        private EnemyControllerServer _enemyControllerServer;
        private float _dissolveAmount;
        public bool Enabled { get; set; }
     

        private void Init()
        {
            _material = GetComponent<Renderer>().material;
            _enemyControllerServer = GetComponentInParent<EnemyControllerServer>();
        }

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            if (Enabled)
            {
                Dissolve();
            }
        }

        private void Dissolve()
        {
            _dissolveAmount += Time.deltaTime / 3;
            _material.SetFloat(DissolveAmountID, _dissolveAmount);
            if (_dissolveAmount >= 1)
            {
                Destroy(_enemyControllerServer.gameObject);
            }
        }
    }
}
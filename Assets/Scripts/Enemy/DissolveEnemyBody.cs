using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEnemyBody : MonoBehaviour
{

    private const string DISSOLVE_AMOUNT_SHADER_PROP = "_DissolveAmount";
    private Material material;
    private EnemyControllerServer _enemyControllerServer;
    private float _dissolveAmount = 0f;
    public bool Enabled { get; set; }
    public float DissolveAmount { get => _dissolveAmount; }


    private void Init()
    {
        material = GetComponent<Renderer>().material;
        Enabled = false;
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
        material.SetFloat(DISSOLVE_AMOUNT_SHADER_PROP, DissolveAmount);
        if (_dissolveAmount >= 1)
        {
            Destroy(_enemyControllerServer.gameObject);
        }
    }
}
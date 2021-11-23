using UnityEngine;
using Mirror;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyControllerClient : NetworkBehaviour
{

    public NavMeshAgent Agent { get; set; }
    public Transform Dest { get; set; }

    private AudioSource _source;
    public bool IsWalking { get; set; }
    private Animator _animator;
    public List<AudioClip> clips = new List<AudioClip>();


    private void InitClientController()
    {
        _source = GetComponent<AudioSource>();
        Agent = GetComponent<NavMeshAgent>();
        IsWalking = true;
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (isServer)
        {
            enabled = false;
        }
        else
        {
            InitClientController();
        }
    }
    private void Update()
    {
        bool isDestSet = Dest != null && Dest.gameObject.activeSelf;
        if (isDestSet && IsWalking)
        {
            Agent.SetDestination(Dest.position);
        }
    }
    public void SetAnim(string animName, bool isOn)
    {
        _animator.SetBool(animName, isOn);
    }

    public void Scream(int random)
    {
        _source.PlayOneShot(_source.clip);
        _source.clip = clips[random];
        _source.PlayOneShot(_source.clip);

    }

    public void SetAgentSpeed(float speed)
    {
        Agent.speed = speed;
    }

}

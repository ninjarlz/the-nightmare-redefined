using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

namespace Enemy
{
    public class EnemyControllerClient : NetworkBehaviour
    {

        public NavMeshAgent Agent { get; set; }
        public Transform Dest { get; set; }
        [SerializeField] private List<AudioClip> _clips = new List<AudioClip>();
        
        private AudioSource _source;
        public bool IsWalking { get; set; }
        
        private Animator _animator;
        public Animator Animator => _animator;

        private void InitClientController()
        {
            _source = GetComponent<AudioSource>();
            Agent = GetComponent<NavMeshAgent>();
            IsWalking = true;
            _animator = GetComponentInChildren<Animator>();
        }

        private bool IsDestinationSet()
        {
            return Dest != null && Dest.gameObject.activeSelf;
        }

        private bool ShouldWalk()
        {
            return IsDestinationSet() && IsWalking;
        }

        private void Start()
        {
            if (isServer)
            {
                enabled = false;
                return;
            }
            InitClientController();
        }
        private void Update()
        {
            if (!ShouldWalk())
            {
                return;
            }
            Agent.SetDestination(Dest.position);
        }
        public void Scream(int random)
        {
            _source.PlayOneShot(_source.clip);
            _source.clip = _clips[random];
            _source.PlayOneShot(_source.clip);
        }
    }
}

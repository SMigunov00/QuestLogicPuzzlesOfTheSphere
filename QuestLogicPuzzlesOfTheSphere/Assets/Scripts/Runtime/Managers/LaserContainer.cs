using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Managers
{
    public sealed class LaserContainer : MonoBehaviour
    {
        private static LaserContainer _instance;
        [SerializeField] private Laser _laserPrefab;
        [SerializeField] private Transform _pool;
        [SerializeField] private int _amountToPool = 20;

        private readonly List<Laser> _pooledLasers = new List<Laser>();

        private void Awake()
        {
            _instance = this;
            InitPool();
        }

        public static Laser CreateLaser(Color color, float energy)
        {
            Laser laser = _instance.GetPooledLaser();
            
            if (laser == null)
                laser = _instance.CreateLaser();

            laser.SetParameters(color, energy);
            laser.gameObject.SetActive(true);
            return laser;
        }

        private void InitPool()
        {
            for (int i = 0; i < _amountToPool; i++)
                CreateLaser();
        }

        private Laser GetPooledLaser()
        {
            for (int i = 0; i < _pooledLasers.Count; i++)
            {
                if (!_pooledLasers[i].gameObject.activeInHierarchy)
                    return _pooledLasers[i];
            }
            return null;
        }

        private Laser CreateLaser()
        {
            Laser laser = Instantiate(_instance._laserPrefab, _instance._pool);
            laser.gameObject.SetActive(false);
            _pooledLasers.Add(laser);
            return laser;
        }
    }
}
using EditorExtensions.Range;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.LaserActivators
{
    public class Gun : Activator
    {
        [SerializeField] private Transform _startPoint;

        //[RangeEx(100, 1000, 50)]
        [Tooltip("Мощность лазера в процентах")]
        [SerializeField] private float _laserPower;

        [Tooltip("Скорость потери энергии в секунду за 100% мощности лазера")]
        [SerializeField] private float _drainRate = 10;

        private Laser _selfLaser;

        private void Start()
        {
            if (GetEnergy() > 0)
            {
                Activate();
            }
        }

        private void Activate()
        {
            _selfLaser = LaserContainer.CreateLaser(GetColor(), _laserPower);
        }

        private void Update()
        {
            if (_selfLaser != null)
            {
                if (GetEnergy() > 0)
                {
                    _selfLaser.transform.position = _startPoint.position;
                    _selfLaser.transform.rotation = _startPoint.rotation;
                    _selfLaser.SetParameters(GetColor(), _laserPower);

                    _selfLaser.Process(); // Удобно, сразу после обновления позиции делается перерасчет лазера.

                    //float drainValue = _drainRate * (_laserPower / 100.0f) * Time.deltaTime;
                    //SetEnergy(GetEnergy() - drainValue);
                }
                //else
                //{
                //    SetEnergy(0);
                //    _selfLaser.Stop();
                //    _selfLaser = null;
                //}
            }
        }
    }
}
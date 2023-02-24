using UnityEngine;

namespace Scrips.Towers
{
    public class BellowTower : TowerBase
    {
        protected override void Start()
        {
            attackRadius = 3f;
            base.Start();
        }

        public override void UpgradeTower(Vector3 upgrade)
        {
            throw new System.NotImplementedException();
        }

        protected override void Attack()
        {
            throw new System.NotImplementedException();
        }

        protected override void VisualChange()
        {
            throw new System.NotImplementedException();
        }
    }
}

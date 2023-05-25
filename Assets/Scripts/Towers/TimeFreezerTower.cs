using UnityEngine;

namespace Towers
{
    public class TimeFreezerTower : TowerBase
    {
        [SerializeField] private Color[] upgradeColors = new Color[5];
        [SerializeField] private ParticleSystem _particleSystem;
        
        private float _stopDuration = 2f,_attackDelay = 6f;
        protected override void Start()
        {
            attackRadius = 1.5f;
            base.Start();
        }
        public override void UpgradeTower(Vector3 upgrade)
        {
            upgradeLevel += upgrade;
            attackRadius += 0.15f * upgrade.x;
            _stopDuration += 0.2f * upgrade.y;
            _attackDelay -= 0.4f * upgrade.z;

            VisualChange();
            indicator.gameObject.transform.localScale = new Vector3(attackRadius*2, attackRadius*2, 1);
        }

        protected override void Attack()
        {
            if (Time.time >= timeForNextAttack && Physics2D.OverlapCircle(transform.position, attackRadius, enemyLayer))
            {
                Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, attackRadius, enemyLayer);
                foreach (Collider2D target in targets)
                {
                    Enemy enemy = target.gameObject.GetComponent<Enemy>();
                    if (enemy)
                    {
                        enemy.TriggerStopEnemy(_stopDuration);
                    }
                }
                timeForNextAttack = Time.time + _attackDelay;
                _particleSystem.Play();
            }
        }

        protected override void VisualChange()
        {
            MainBodySpriteRenderer.color = upgradeColors[(int) upgradeLevel.y];
        }
    }
}

using System.Collections;
using Towers;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scrips.Towers
{
    public class ArtilleryTower : TowerBase
    {
        [SerializeField] private GameObject[] barrels;
        [SerializeField] private GameObject explosion; 

        private bool _changingTargetPosition;
        private int _attackDamage = 1,  _barrelNumber = 1;
        private float _attackDelay = 6;
        private Vector3 _oldBombTargetPosition;
        protected override void Start()
        {
            attackRadius = 1f;
            ToggleBarrels();
            base.Start();
        }
        private void LateUpdate()
        {
            if (_changingTargetPosition)
            {
                Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                indicator.transform.position = mousePosition;
                indicator.enabled = true;
                if (Input.GetMouseButtonDown(0))
                {
                    _changingTargetPosition = false; _oldBombTargetPosition = indicator.transform.position;
                    indicator.enabled = false;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    _changingTargetPosition = false;
                    indicator.enabled = false;
                    indicator.transform.position = _oldBombTargetPosition;
                }
            }
        }

        public override void UpgradeTower(Vector3 upgrade)
        {
            upgradeLevel += upgrade;
            _barrelNumber += 1 * (int) upgrade.x ;
            _attackDamage += 1 * (int) upgrade.y;
            _attackDelay -= 0.3f * (int) upgrade.z;

            VisualChange(); ToggleBarrels();
            indicator.gameObject.transform.localScale = new Vector3(attackRadius*2, attackRadius*2, 1);
        }

        protected override void Attack()
        {
            if (Time.time >= timeForNextAttack )
            {
                Collider2D[] targets = Physics2D.OverlapCircleAll(indicator.transform.position, attackRadius, enemyLayer);
                foreach (Collider2D target in targets)
                {
                    target.GetComponent<Enemy>().TakeDamage(_attackDamage);
                }
                Instantiate(explosion, indicator.transform.position+ new Vector3(Random.Range(-0.2f,0.2f),Random.Range(-0.2f,0.2f),0), quaternion.identity,indicator.gameObject.transform);
                timeForNextAttack = Time.time + _attackDelay/_barrelNumber;
                Debug.Log("shoot");
            }
        }

        protected override void VisualChange()
        {
            MainBodySpriteRenderer.color = ColorSequence(_attackDamage);
        }

        private void ToggleBarrels()
        {
            for (int i = 0; i < barrels.Length; i++)
            {
                barrels[i].SetActive(i <= _barrelNumber -1);
            }
        }

        private IEnumerator DelayChangingTargetPosition()
        {
            yield return new WaitForEndOfFrame();
            _changingTargetPosition = true;
        }

        public void ChangingTargetPosition()
        {
            StartCoroutine(DelayChangingTargetPosition());
            _oldBombTargetPosition = indicator.transform.position;
        }
    }
}

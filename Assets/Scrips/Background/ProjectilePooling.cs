using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scrips.Background
{
    public class ProjectilePooling : MonoBehaviour
    {
        public static ProjectilePooling Instance;
        
        [SerializeField] private GameObject _standardProjectile;
        private List<GameObject> _standardProjectilePool;
        [SerializeField] private int _totalStandardProjectileCount;
        void Start()
        {
            if (!Instance)
            {
                Instance = this;
            }
            _standardProjectilePool = new List<GameObject>() ;
        }

        public void AddStandardProjectileToPool(GameObject projectile)
        {
            if (_standardProjectilePool.Contains(projectile))
            {
                return;
            }
            projectile.transform.SetParent(transform);
            _standardProjectilePool.Add(projectile);
            UpdateName();
            projectile.SetActive(false);
        }

        public GameObject GetStandardProjectileFromPool()
        {
            GameObject returnProjectile = null;
            if (_standardProjectilePool.Count < 1)
            {
                returnProjectile = Instantiate(_standardProjectile, transform.position, Quaternion.identity);
                returnProjectile.gameObject.name = $"StandardProjectile({_totalStandardProjectileCount})";
                _totalStandardProjectileCount++;
            }
            else
            {
                returnProjectile = _standardProjectilePool.First();
                returnProjectile.SetActive(true);
                _standardProjectilePool.Remove(returnProjectile);
                returnProjectile.transform.SetParent(null);
            }
            
            UpdateName();
            return returnProjectile;
        }

        private void UpdateName()
        {
            gameObject.name = $"ProjectilePool ({transform.childCount})";
        }
    }
}

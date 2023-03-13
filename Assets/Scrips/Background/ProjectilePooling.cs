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
            projectile.transform.SetParent(transform);
            projectile.SetActive(false);
            _standardProjectilePool.Add(projectile);
            UpdateName();
        }

        public GameObject GetStandardProjectileFromPool()
        {
            GameObject returnProjectile = null;
            if (_standardProjectilePool.Count < 1)
            {
                returnProjectile = Instantiate(_standardProjectile, transform.position, Quaternion.identity);
            }
            else
            {
                returnProjectile = _standardProjectilePool.First();
                returnProjectile.SetActive(true);
                _standardProjectilePool.Remove(returnProjectile);
            }
            returnProjectile.transform.SetParent(null);
            UpdateName();
            return returnProjectile;
        }

        private void UpdateName()
        {
            gameObject.name = $"ProjectilePool ({transform.childCount})";
        }
    }
}

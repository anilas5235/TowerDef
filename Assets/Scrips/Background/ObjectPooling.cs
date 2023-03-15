using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scrips.Background
{
    public abstract class ObjectPooling : MonoBehaviour
    {
        [SerializeField] protected GameObject objectToPool;
        protected List<GameObject> _objectPool;
        protected int _totalObjectCount;
        protected virtual void Start()
        {
            _objectPool = new List<GameObject>() ;
        }

        public void AddObjectToPool(GameObject projectile)
        {
            if (_objectPool.Contains(projectile))
            {
                return;
            }
            projectile.transform.SetParent(transform);
            _objectPool.Add(projectile);
            UpdateName();
            projectile.SetActive(false);
        }

        public GameObject GetObjectFromPool()
        {
            GameObject returnProjectile = null;
            if (_objectPool.Count < 1)
            {
                returnProjectile = Instantiate(objectToPool, transform.position, Quaternion.identity);
                returnProjectile.gameObject.name = $"{objectToPool.name}({_totalObjectCount})";
                SetPoolingReference(returnProjectile);
                _totalObjectCount++;
            }
            else
            {
                returnProjectile = _objectPool.First();
                returnProjectile.SetActive(true);
                _objectPool.Remove(returnProjectile);
                returnProjectile.transform.SetParent(null);
            }
            
            UpdateName();
            return returnProjectile;
        }

        protected abstract void UpdateName();

        protected abstract void SetPoolingReference(GameObject objectRef);
    }
}

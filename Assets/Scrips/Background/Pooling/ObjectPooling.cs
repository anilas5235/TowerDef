using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scrips.Background.Pooling
{
    public abstract class ObjectPooling : MonoBehaviour
    {
        [SerializeField] protected GameObject objectToPool;
        private List<GameObject> _objectPool;
        private int _totalObjectCount;
        protected virtual void Start()
        {
            _objectPool = new List<GameObject>() ;
        }

        public void AddObjectToPool(GameObject objectToPool)
        {
            if (_objectPool.Contains(objectToPool))
            {
                return;
            }
            objectToPool.transform.SetParent(transform);
            _objectPool.Add(objectToPool);
            UpdateName();
            objectToPool.transform.position = transform.position;
            objectToPool.SetActive(false);
        }

        public GameObject GetObjectFromPool()
        {
            GameObject returnProjectile = null;
            if (_objectPool.Count < 1)
            {
                returnProjectile = Instantiate(objectToPool, transform.position, Quaternion.identity);
                returnProjectile.gameObject.name = $"{objectToPool.name}({_totalObjectCount})";
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

        private void UpdateName()
        {
            gameObject.name = $"{objectToPool.name}Pool({transform.childCount})";
        }
    }
}

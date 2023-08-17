using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools
{
    using Object = UnityEngine.Object;

    public static class Pool
    {
        private static Dictionary<Type, List<GameObject>> pools = new Dictionary<Type, List<GameObject>>();

        private static bool IsExist<T>()
        {
            return pools.ContainsKey(typeof(T));
        }

        private static T AddToPool<T>(T component) where T : Component
        {
            pools[typeof(T)].Add(component.gameObject);
            return component;
        }

        private static T TryGetFromPool<T>(Func<T> onFailedGet) where T : Component
        {
            CheckPool<T>();
            return pools[typeof(T)].FirstOrDefault(x => !x.activeSelf)?.GetComponent<T>() ?? onFailedGet?.Invoke();
        }

        private static T New<T>() where T : Component
        {
            GameObject g = new GameObject("CreatedFromPool" + typeof(T));
            T result = g.AddComponent<T>();
            AddToPool(result);
            return result;
        }

        private static void CheckPool<T>()
        {
            if (!IsExist<T>())
            {
                pools.Add(typeof(T), new List<GameObject>());
            }
        }

        #region static component

        public static T Instantiate<T>() where T : Component
        {
            T result = TryGetFromPool(New<T>);
            result.gameObject.SetActive(true);
            return result;
        }

        public static T Instantiate<T>(T original) where T : Component
        {
            T result = TryGetFromPool(() => AddToPool(Object.Instantiate(original)));
            result.gameObject.SetActive(true);
            return result;
        }

        public static T Instantiate<T>(T original, Transform parent) where T : Component
        {
            T result = TryGetFromPool(() => AddToPool(Object.Instantiate(original, parent)));
            result.transform.SetParent(parent);
            result.gameObject.SetActive(true);
            return result;
        }

        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Component
        {
            T result = TryGetFromPool(() => AddToPool(Object.Instantiate(original, position, rotation)));
            result.transform.SetPosRot(position, rotation);
            result.gameObject.SetActive(true);
            return result;
        }

        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            T result = TryGetFromPool(() => AddToPool(Object.Instantiate(original, position, rotation, parent)));
            result.transform.SetPosRot(position, rotation);
            result.transform.SetParent(parent);
            result.gameObject.SetActive(true);
            return result;
        }

        #endregion

        #region extended component

        public static T InstantiateUsePool<T>(this Object obj) where T : Component
        {
            return Instantiate<T>();
        }

        public static T InstantiateUsePool<T>(this Object obj, T original) where T : Component
        {
            return Instantiate(original);
        }

        public static T InstantiateUsePool<T>(this Object obj, T original, Transform parent) where T : Component
        {
            return Instantiate(original, parent);
        }

        public static T InstantiateUsePool<T>(this Object obj, T original, Vector3 position, Quaternion rotation) where T : Component
        {
            return Instantiate(original, position, rotation);
        }

        public static T InstantiateUsePool<T>(this Object obj, T original, Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            return Instantiate(original, position, rotation, parent);
        }

        #endregion

        #region destroy static

        public static void Destroy<T>(T obj) where T : Component
        {
            obj.gameObject.SetActive(false);
        }

        #endregion

        #region destroy extention

        public static void DestroyWithPool<T>(this Object o, T obj) where T : Component
        {
            obj.gameObject.SetActive(false);
        }

        #endregion
        
        #region sugar

        public static T FirstOrDefaultFromPool<T>(this Object o, Func<T, bool> predicate) where T : Component
        {
            if (predicate != null)
            {
                CheckPool<T>();
                return pools[typeof(T)].FirstOrDefault(delegate(GameObject x)
                {
                    if (x.activeSelf && x.GetComponent<T>() != null)
                    {
                        return predicate.Invoke(x.GetComponent<T>());
                    }
                
                    return false;
                })?.GetComponent<T>();
            }

            throw new Exception("Predicate is null");
        }
        
        #endregion
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A system for storing multiple copies of the same
/// object that need to be disabled before use.
/// </summary>
/// <typeparam name="T">Type of the pooled object</typeparam>
public class Pool<T>
    where T : Component
{
    // The initial size of the pool.
    private int _poolSize;

    // The prefab from which all objects in the pool are instantiated.
    private T _objectPrefab;

    // When the pool runs out of objects, should the pool grow or just
    // return null.
    private bool _shouldGrow;

    // The list containing all the objects in this pool.
    private List<T> _pool;

    // The pooled objects' initialization method
    private Action<T> _initMethod;

    /// <summary>
    /// A pool for multiple copies of the same object.
    /// </summary>
    /// <param name="objectPrefab">The pooled object</param>
    /// <param name="poolSize">How many copies are there of the object
    /// </param>
    /// <param name="shouldGrow">Is a new copy created when attempting
    /// to get one from the pool but they are all in use</param>
    public Pool(T objectPrefab, int poolSize, bool shouldGrow)
    {
        _objectPrefab = objectPrefab;
        _poolSize = poolSize;
        _shouldGrow = shouldGrow;

        // Initialize the pool by adding '_poolSize' amount of objects to the pool.
        _pool = new List<T>(_poolSize);

        for (int i = 0; i < _poolSize; ++i)
        {
            AddObject();
        }
    }

    /// <summary>
    /// A pool for multiple copies of the same object.
    /// </summary>
    /// <param name="objectPrefab">The pooled object</param>
    /// <param name="poolSize">How many copies are there of the object
    /// </param>
    /// <param name="shouldGrow">Is a new copy created when attempting
    /// to get one from the pool but they are all in use</param>
    /// <param name="initMethod">An initialization method for the object
    /// </param>
    public Pool(T objectPrefab, int poolSize, bool shouldGrow, Action<T> initMethod)
        : this(objectPrefab, poolSize, shouldGrow)
    {
        _initMethod = initMethod;

        foreach (var item in _pool)
        {
            _initMethod(item);
        }
    }

    /// <summary>
    /// Adds an object to the pool.
    /// </summary>
    /// <param name="isActive">Should the object be active when it is added to the pool or not.</param>
    /// <returns>The object added to the pool.</returns>
    private T AddObject(bool isActive = false)
    {
        // Instantiate pooled objects.
        T component = UnityEngine.Object.Instantiate(_objectPrefab);

        if (isActive)
        {
            Activate(component);
        }
        else
        {
            Deactivate(component);
        }

        _pool.Add(component);

        return component;
    }

    /// <summary>
    /// Called when the object is fetched from the pool. Activates the object.
    /// </summary>
    /// <param name="component">Object to activate</param>
    protected virtual void Activate(T component)
    {
        component.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called when the object is returned to the pool. Deactivates the object.
    /// </summary>
    /// <param name="component">Object to deactivate</param>
    protected virtual void Deactivate(T component)
    {
        component.gameObject.SetActive(false);
    }

    /// <summary>
    /// Fetches an object form the pool.
    /// </summary>
    /// <param name="activate">is the object returned active</param>
    /// <returns>An object from the pool or if all objects are
    /// already in use and pool cannot grow, returns null</returns>
    public T GetPooledObject(bool activate)
    {
        T result = null;
        for (int i = 0; i < _pool.Count; i++)
        {
            if (_pool[i].gameObject.activeSelf == false)
            {
                result = _pool[i];
                break;
            }
        }

        // If there were no inactive GameObjects in the pool and the pool should
        // grow, then let's add a new object to the pool.
        if (result == null && _shouldGrow)
        {
            result = AddObject();
        }

        // If we found an inactive object, let's activate it.
        if (result != null)
        {
            if (activate)
            {
                Activate(result);
            }
        }

        return result;
    }

    /// <summary>
    /// Returns an object back to the pool.
    /// </summary>
    /// <param name="component">The object which should be returned to the pool.</param>
    /// <returns>Could the object be returned back to the pool.</returns>
    public bool ReturnObject(T component)
    {
        bool result = false;

        foreach (var pooledObject in _pool)
        {
            if (pooledObject == component)
            {
                Deactivate(component);
                result = true;
                break;
            }
        }

        if (!result)
        {
            Debug.LogError("Tried to return an object which doesn't belong to this pool!");
        }

        return result;
    }
}

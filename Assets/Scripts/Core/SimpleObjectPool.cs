using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 泛型对象池，用于复用频繁创建/销毁的 Component。
/// 适用于 TrackSegment、Obstacle、Collectible、VFX 等。
/// </summary>
/// <typeparam name="T">池中对象的 Component 类型</typeparam>
public sealed class SimpleObjectPool<T> where T : Component
{
    private readonly T prefab;
    private readonly Transform parent;
    private readonly Queue<T> idleObjects;
    private readonly int defaultCapacity;

    /// <summary>当前空闲（可用）的对象数量</summary>
    public int IdleCount => idleObjects.Count;

    /// <summary>当前活跃（已被取出）的对象数量</summary>
    public int ActiveCount { get; private set; }

    /// <summary>池中总对象数（空闲 + 活跃）</summary>
    public int TotalCount => IdleCount + ActiveCount;

    /// <summary>
    /// 创建对象池。
    /// </summary>
    /// <param name="prefab">要复用的预制体</param>
    /// <param name="initialCapacity">预创建数量（默认 10）</param>
    /// <param name="parent">池中对象的父 Transform（可选）</param>
    public SimpleObjectPool(T prefab, int initialCapacity = 10, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.defaultCapacity = Mathf.Max(1, initialCapacity);
        idleObjects = new Queue<T>(this.defaultCapacity);

        // 预创建：游戏开始时就建好对象，避免运行时 Instantiate 卡顿
        for (int i = 0; i < this.defaultCapacity; i++)
        {
            CreateOne(idle: true);
        }
    }

    // ────────────────────────── 核心 API ──────────────────────────

    /// <summary>
    /// 从池中取出一个对象（设置位置和旋转），自动激活。
    /// 池为空时自动扩容。
    /// </summary>
    public T Get(Vector3 position, Quaternion rotation)
    {
        T obj = idleObjects.Count > 0 ? idleObjects.Dequeue() : CreateOne(idle: false);

        Transform t = obj.transform;
        t.SetPositionAndRotation(position, rotation);
        obj.gameObject.SetActive(true);

        if (obj is IPoolable poolable)
        {
            poolable.OnSpawn();
        }

        ActiveCount++;
        return obj;
    }

    /// <summary>
    /// 从池中取出一个对象（仅设置位置，旋转为 Quaternion.identity）。
    /// </summary>
    public T Get(Vector3 position)
    {
        return Get(position, Quaternion.identity);
    }

    /// <summary>
    /// 将对象放回池中。对象会被隐藏并重新入队，等待下次取出。
    /// 对象为 null 时静默忽略。
    /// </summary>
    public void Release(T obj)
    {
        if (obj == null)
        {
            return;
        }

        if (obj is IPoolable poolable)
        {
            poolable.OnDespawn();
        }

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(parent);
        idleObjects.Enqueue(obj);
        ActiveCount--;
    }

    // ──────────────────────── 内部方法 ──────────────────────────

    /// <summary>
    /// 自动扩容：当池子空了且有新请求时，创建一个额外对象。
    /// </summary>
    private T CreateOne(bool idle)
    {
        T obj = Object.Instantiate(prefab, parent);
        obj.gameObject.SetActive(false);

        if (idle)
        {
            idleObjects.Enqueue(obj);
        }

        return obj;
    }
}

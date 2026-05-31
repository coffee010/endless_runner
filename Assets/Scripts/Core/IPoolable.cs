/// <summary>
/// 对象池元素接口。实现此接口的组件在被取出/放回池时会收到回调，
/// 用于重置状态、取消订阅事件等。
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// 从池中取出时调用。在此重置所有可变状态（血量、标记、速度等）。
    /// </summary>
    void OnSpawn();

    /// <summary>
    /// 放回池中时调用。在此清理引用、取消订阅、停止协程等。
    /// </summary>
    void OnDespawn();
}

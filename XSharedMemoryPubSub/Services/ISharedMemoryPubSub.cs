using System;
using System.Collections.Generic;
using System.Text;

namespace XSharedMemoryPubSub.Services
{
    /// <summary>
    /// 共享内存发布订阅接口
    /// </summary>
    public interface ISharedMemoryPubSub : IDisposable
    {
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="topicId">主题ID</param>
        /// <param name="data">消息数据（最大1024字节）</param>
        void Publish(int topicId, byte[] data);

        /// <summary>
        /// 取消指定主题的订阅
        /// </summary>
        void Unsubscribe(int topicId);

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="topicId">主题ID</param>
        /// <param name="handler">消息处理回调</param>
        /// <returns>订阅句柄（用于取消订阅）</returns>
        IDisposable Subscribe(int topicId, Action<(int MessageId, int TopicId, byte[] Data)> handler);
    }
}

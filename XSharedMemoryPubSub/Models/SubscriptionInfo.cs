using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace XSharedMemoryPubSub.Models
{
    /// <summary>
    /// 订阅信息类
    /// </summary>
    public class SubscriptionInfo
    {
        public Guid Id { get; set; } // 订阅ID
        public Action<(int MessageId, int TopicId, byte[] Data)> Handler { get; set; } // 消息处理函数
        public CancellationTokenSource Cts { get; set; } // 取消令牌
    }
}

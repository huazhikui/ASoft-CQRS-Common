using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft
{
    public interface ICacheable
    {
        void Add<T>(string key, T item);

        void Add<T>(string key, T item, TimeSpan? timeToLive);


        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataRetriever">缓存中没有时，调用该委托返回数据并添加到缓存</param>
        /// <returns></returns>
        T Get<T>(string key, Func<T> dataRetriever) where T : class;

        T Get<T>(string key, Func<T> dataRetriever, TimeSpan? timeToLive) where T : class;

        object Get(string key, Type type, Func<object> dataRetriever);

        object Get(string key, Type type, Func<object> dataRetriever, TimeSpan? timeToLive);

        Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever) where T : class;

        Task<T> GetAsync<T>(string key, Func<Task<T>> dataRetriever, TimeSpan? timeToLive) where T : class;

        Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever);

        Task<object> GetAsync(string key, Type type, Func<Task<object>> dataRetriever, TimeSpan? timeToLive);

        void Remove(string key);
    }


}

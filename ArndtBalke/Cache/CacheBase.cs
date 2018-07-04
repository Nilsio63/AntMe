using System.Collections;
using System.Collections.Generic;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal abstract class CacheBase<T> : IEnumerable<T>
    {
        private readonly List<T> _list = new List<T>();

        public void Add(T obj)
        {
            if (!_list.Contains(obj))
                _list.Add(obj);
        }

        public void Cleanup()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (IsDeprecated(_list[i]))
                    _list.RemoveAt(i);
            }
        }

        protected abstract bool IsDeprecated(T obj);

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

    }
}

using System.Collections;
using System.Collections.Generic;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal abstract class CacheBase
    {
        public abstract void Cleanup();

    }

    internal abstract class CacheBase<T> : CacheBase, IEnumerable<T>
        where T : class
    {
        protected readonly List<T> _list = new List<T>();

        public virtual void Add(T obj)
        {
            if (!Contains(obj))
                _list.Add(obj);
        }

        public virtual bool Contains(T obj)
        {
            return _list.Contains(obj);
        }

        public override void Cleanup()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (IsDeprecated(_list[i]))
                    _list.RemoveAt(i--);
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

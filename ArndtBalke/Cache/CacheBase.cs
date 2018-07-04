using AntMe.Player.ArndtBalke.Behavior;
using System.Collections;
using System.Collections.Generic;

namespace AntMe.Player.ArndtBalke.Cache
{
    internal abstract class CacheBase
    {
        protected readonly BaseBehavior _ant;

        protected CacheBase(BaseBehavior ant)
        {
            _ant = ant;
        }

        public abstract void Cleanup();

    }

    internal abstract class CacheBase<T> : CacheBase, IEnumerable<T>
        where T : class
    {
        protected readonly List<T> _list = new List<T>();

        protected CacheBase(BaseBehavior ant)
            : base(ant)
        { }

        public virtual void Add(T obj)
        {
            if (!Contains(obj))
                _list.Add(obj);
        }

        public T GetNearest()
        {
            T nearest = null;

            foreach (T item in _list)
            {
                if (IsValid(item) && (nearest == null || GetDistanceToAnt(item) < GetDistanceToAnt(nearest)))
                    nearest = item;
            }

            return nearest;
        }

        protected virtual bool IsValid(T obj)
        {
            return true;
        }

        protected abstract int GetDistanceToAnt(T obj);

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

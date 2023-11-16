using System;
using System.Collections.Generic;

namespace Game.Utils;

internal class ObjectPool<T> {
    private readonly Stack<T> m_PoolItems;
    private readonly Func<T> m_CreateObjectFunc;
    private readonly Action<T> m_DeleteObjectFunc;
    private readonly int m_MaxPoolSize;

    private int m_ObjectsCreated;

    public ObjectPool(Func<T> createObjectFunc, Action<T> deleteObjectFunc, int startPoolSize = 10, int maxPoolSize = 30) {
        m_CreateObjectFunc = createObjectFunc;
        m_DeleteObjectFunc = deleteObjectFunc;
        m_MaxPoolSize = maxPoolSize;
        m_ObjectsCreated = startPoolSize;

        m_PoolItems = new Stack<T>(startPoolSize);
        for(int i=0; i<startPoolSize; ++i)
            m_PoolItems.Push(Create());
    }

    public T Get() {
        if(!m_PoolItems.TryPop(out T obj))
            obj = Create();

        return obj;
    }

    public void Release(T obj) {
        if(m_PoolItems.Count >= m_MaxPoolSize) {
            m_DeleteObjectFunc(obj);
            return;
        }

        m_PoolItems.Push(obj);
    }

    private T Create() {
        T obj = m_CreateObjectFunc();
        m_ObjectsCreated++;
        return obj;
    }
}
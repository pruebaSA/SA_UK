namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading;

    public abstract class BufferManager
    {
        protected BufferManager()
        {
        }

        public abstract void Clear();
        public static BufferManager CreateBufferManager(long maxBufferPoolSize, int maxBufferSize)
        {
            if (maxBufferPoolSize == 0L)
            {
                return GCBufferManager.Value;
            }
            return new PooledBufferManager(maxBufferPoolSize, maxBufferSize);
        }

        public abstract void ReturnBuffer(byte[] buffer);
        public abstract byte[] TakeBuffer(int bufferSize);

        private class GCBufferManager : BufferManager
        {
            private static BufferManager.GCBufferManager value = new BufferManager.GCBufferManager();

            private GCBufferManager()
            {
            }

            public override void Clear()
            {
            }

            public override void ReturnBuffer(byte[] buffer)
            {
                if (buffer == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
                }
            }

            public override byte[] TakeBuffer(int bufferSize)
            {
                if (bufferSize < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("bufferSize", bufferSize, System.ServiceModel.SR.GetString("ValueMustBeNonNegative")));
                }
                return DiagnosticUtility.Utility.AllocateByteArray(bufferSize);
            }

            public static BufferManager.GCBufferManager Value =>
                value;
        }

        private class PooledBufferManager : BufferManager
        {
            private bool areQuotasBeingTuned;
            private BufferPool[] bufferPools;
            private int[] bufferSizes;
            private const int initialBufferCount = 1;
            private const int maxMissesBeforeTuning = 8;
            private long memoryLimit;
            private const int minBufferSize = 0x80;
            private long remainingMemory;
            private int totalMisses;
            private object tuningLock;

            public PooledBufferManager(long maxMemoryToPool, int maxBufferSize)
            {
                long num2;
                this.tuningLock = new object();
                if (maxMemoryToPool < 0L)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("maxMemoryToPool", maxMemoryToPool, System.ServiceModel.SR.GetString("ValueMustBeNonNegative")));
                }
                if (maxBufferSize < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("maxBufferSize", maxBufferSize, System.ServiceModel.SR.GetString("ValueMustBeNonNegative")));
                }
                this.memoryLimit = maxMemoryToPool;
                this.remainingMemory = maxMemoryToPool;
                List<BufferPool> list = new List<BufferPool>();
                int bufferSize = 0x80;
            Label_007E:
                num2 = this.remainingMemory / ((long) bufferSize);
                int limit = (num2 > 0x7fffffffL) ? 0x7fffffff : ((int) num2);
                if (limit > 1)
                {
                    limit = 1;
                }
                list.Add(new BufferPool(bufferSize, limit));
                this.remainingMemory -= limit * bufferSize;
                if (bufferSize < maxBufferSize)
                {
                    long num4 = bufferSize * 2L;
                    if (num4 > maxBufferSize)
                    {
                        bufferSize = maxBufferSize;
                    }
                    else
                    {
                        bufferSize = (int) num4;
                    }
                    goto Label_007E;
                }
                this.bufferPools = list.ToArray();
                this.bufferSizes = new int[this.bufferPools.Length];
                for (int i = 0; i < this.bufferPools.Length; i++)
                {
                    this.bufferSizes[i] = this.bufferPools[i].BufferSize;
                }
            }

            private byte[] AllocNewBuffer(int bufferSize) => 
                DiagnosticUtility.Utility.AllocateByteArray(bufferSize);

            private void ChangeQuota(ref BufferPool bufferPool, int delta)
            {
                BufferPool pool = bufferPool;
                int limit = pool.Limit + delta;
                BufferPool pool2 = new BufferPool(pool.BufferSize, limit);
                for (int i = 0; i < limit; i++)
                {
                    byte[] buffer = pool.Take();
                    if (buffer == null)
                    {
                        break;
                    }
                    pool2.Return(buffer);
                    pool2.IncrementCount();
                }
                this.remainingMemory -= pool.BufferSize * delta;
                bufferPool = pool2;
            }

            public override void Clear()
            {
                for (int i = 0; i < this.bufferPools.Length; i++)
                {
                    this.bufferPools[i].Clear();
                }
            }

            private void DecreaseQuota(ref BufferPool bufferPool)
            {
                this.ChangeQuota(ref bufferPool, -1);
            }

            private int FindMostExcessivePool()
            {
                long num = 0L;
                int num2 = -1;
                for (int i = 0; i < this.bufferPools.Length; i++)
                {
                    BufferPool pool = this.bufferPools[i];
                    if (pool.Peak < pool.Limit)
                    {
                        long num4 = (pool.Limit - pool.Peak) * pool.BufferSize;
                        if (num4 > num)
                        {
                            num2 = i;
                            num = num4;
                        }
                    }
                }
                return num2;
            }

            private int FindMostStarvedPool()
            {
                long num = 0L;
                int num2 = -1;
                for (int i = 0; i < this.bufferPools.Length; i++)
                {
                    BufferPool pool = this.bufferPools[i];
                    if (pool.Peak == pool.Limit)
                    {
                        long num4 = pool.Misses * pool.BufferSize;
                        if (num4 > num)
                        {
                            num2 = i;
                            num = num4;
                        }
                    }
                }
                return num2;
            }

            private BufferPool FindPool(int desiredBufferSize)
            {
                for (int i = 0; i < this.bufferSizes.Length; i++)
                {
                    if (desiredBufferSize <= this.bufferSizes[i])
                    {
                        return this.bufferPools[i];
                    }
                }
                return null;
            }

            private void IncreaseQuota(ref BufferPool bufferPool)
            {
                this.ChangeQuota(ref bufferPool, 1);
            }

            public override void ReturnBuffer(byte[] buffer)
            {
                if (buffer == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
                }
                BufferPool pool = this.FindPool(buffer.Length);
                if (pool != null)
                {
                    if (buffer.Length != pool.BufferSize)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.ServiceModel.SR.GetString("BufferIsNotRightSizeForBufferManager"), "buffer"));
                    }
                    if (pool.Return(buffer))
                    {
                        pool.IncrementCount();
                    }
                }
            }

            public override byte[] TakeBuffer(int bufferSize)
            {
                if (bufferSize < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("bufferSize", bufferSize, System.ServiceModel.SR.GetString("ValueMustBeNonNegative")));
                }
                BufferPool pool = this.FindPool(bufferSize);
                if (pool == null)
                {
                    return this.AllocNewBuffer(bufferSize);
                }
                byte[] buffer = pool.Take();
                if (buffer != null)
                {
                    pool.DecrementCount();
                    return buffer;
                }
                if (pool.Peak == pool.Limit)
                {
                    pool.Misses++;
                    if (++this.totalMisses >= 8)
                    {
                        this.TuneQuotas();
                    }
                }
                return this.AllocNewBuffer(pool.BufferSize);
            }

            private void TuneQuotas()
            {
                if (!this.areQuotasBeingTuned)
                {
                    bool flag = false;
                    try
                    {
                        try
                        {
                        }
                        finally
                        {
                            flag = Monitor.TryEnter(this.tuningLock);
                        }
                        if (!flag || this.areQuotasBeingTuned)
                        {
                            return;
                        }
                        this.areQuotasBeingTuned = true;
                    }
                    finally
                    {
                        if (flag)
                        {
                            Monitor.Exit(this.tuningLock);
                        }
                    }
                    int index = this.FindMostStarvedPool();
                    if (index >= 0)
                    {
                        BufferPool pool = this.bufferPools[index];
                        if (this.remainingMemory < pool.BufferSize)
                        {
                            int num2 = this.FindMostExcessivePool();
                            if (num2 >= 0)
                            {
                                this.DecreaseQuota(ref this.bufferPools[num2]);
                            }
                        }
                        if (this.remainingMemory >= pool.BufferSize)
                        {
                            this.IncreaseQuota(ref this.bufferPools[index]);
                        }
                    }
                    for (int i = 0; i < this.bufferPools.Length; i++)
                    {
                        BufferPool pool2 = this.bufferPools[i];
                        pool2.Misses = 0;
                    }
                    this.totalMisses = 0;
                    this.areQuotasBeingTuned = false;
                }
            }

            private class BufferPool
            {
                private int bufferSize;
                private int count;
                private int limit;
                private int misses;
                private int peak;
                private SynchronizedPool<byte[]> pool;

                public BufferPool(int bufferSize, int limit)
                {
                    this.pool = new SynchronizedPool<byte[]>(limit);
                    this.bufferSize = bufferSize;
                    this.limit = limit;
                }

                public void Clear()
                {
                    this.pool.Clear();
                    this.count = 0;
                }

                public void DecrementCount()
                {
                    int num = this.count - 1;
                    if (num >= 0)
                    {
                        this.count = num;
                    }
                }

                public void IncrementCount()
                {
                    int num = this.count + 1;
                    if (num <= this.limit)
                    {
                        this.count = num;
                        if (num > this.peak)
                        {
                            this.peak = num;
                        }
                    }
                }

                public bool Return(byte[] buffer) => 
                    this.pool.Return(buffer);

                public byte[] Take() => 
                    this.pool.Take();

                public int BufferSize =>
                    this.bufferSize;

                public int Limit =>
                    this.limit;

                public int Misses
                {
                    get => 
                        this.misses;
                    set
                    {
                        this.misses = value;
                    }
                }

                public int Peak =>
                    this.peak;
            }
        }
    }
}


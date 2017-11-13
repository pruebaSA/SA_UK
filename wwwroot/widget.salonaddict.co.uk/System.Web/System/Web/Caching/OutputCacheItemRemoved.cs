namespace System.Web.Caching
{
    using System;
    using System.Threading;
    using System.Web;
    using System.Web.Hosting;

    internal class OutputCacheItemRemoved
    {
        internal OutputCacheItemRemoved()
        {
        }

        internal void CacheItemRemovedCallback(string key, object value, CacheItemRemovedReason reason)
        {
            Interlocked.Decrement(ref OutputCacheModule.s_cEntries);
            PerfCounters.DecrementCounter(AppPerfCounter.OUTPUT_CACHE_ENTRIES);
            PerfCounters.IncrementCounter(AppPerfCounter.OUTPUT_CACHE_TURNOVER_RATE);
            CachedRawResponse response = value as CachedRawResponse;
            if (response != null)
            {
                string cacheKey = response._kernelCacheUrl;
                if ((cacheKey != null) && (HttpRuntime.CacheInternal[key] == null))
                {
                    if (HttpRuntime.UseIntegratedPipeline)
                    {
                        UnsafeIISMethods.MgdFlushKernelCache(cacheKey);
                    }
                    else
                    {
                        UnsafeNativeMethods.InvalidateKernelCache(cacheKey);
                    }
                }
            }
        }
    }
}


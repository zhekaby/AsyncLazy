using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace S4B.LazyAsyncNET
{
    public class LazyAsync<T>
    {
        class Data
        {
            public long RenewAt, ExpiresAt;
            public Task<T> Current, Pending;
        }

        private Data _data = new Data();

        public Task<T> Task
        {
            get
            {
                var now = DateTime.UtcNow.Ticks;
                var data = _data;

                for (; ; )
                {
                    if (data.ExpiresAt >= now)
                    {
                        if (data.RenewAt < now && data.Pending == data.Current)
                        {
                            var tcs = new TaskCompletionSource<T>();
                            var pending = new Data
                            {
                                RenewAt = data.RenewAt,
                                ExpiresAt = data.ExpiresAt,
                                Current = data.Current,
                                Pending = tcs.Task
                            };

                            var xchgResult = Interlocked.CompareExchange(ref _data, pending, data);

                            if (xchgResult == data)
                                Evaluate(tcs);
                        }

                        return data.Current;
                    }

                    if (data.Pending != data.Current)
                    {
                        return data.Pending;
                    }
                    else
                    {
                        var tcs = new TaskCompletionSource<T>();

                        var pending = new Data
                        {
                            RenewAt = data.RenewAt,
                            ExpiresAt = data.ExpiresAt,
                            Current = data.Current,
                            Pending = tcs.Task
                        };

                        var xchgResult = Interlocked.CompareExchange(ref _data, pending, data);

                        if (xchgResult != data)
                        {
                            data = xchgResult;
                            continue;
                        }

                        Evaluate(tcs, true);

                        return tcs.Task;
                    }
                }
            }
        }

        private readonly Func<Task<(T value, DateTime renewAt, DateTime expiresAt)>> eval;

        public LazyAsync(
           Func<Task<(T value, DateTime renewAt, DateTime expiresAt)>> eval,
           Task<(T value, DateTime renewAt, DateTime expiresAt)> initial = null, string name = null)
        {
            if (initial != null)
            {
                this.eval = () => initial;
                var tcs = new TaskCompletionSource<T>();

                _data = new Data
                {
                    RenewAt = 0,
                    ExpiresAt = 0,
                    Current = null,
                    Pending = tcs.Task
                };

                Evaluate(tcs);
            }

            this.eval = eval;
        }

        async void Evaluate(TaskCompletionSource<T> tcs, bool blocking = false)
        {
            try
            {
                var result = await eval().ConfigureAwait(false);

                var pending = new Data
                {
                    RenewAt = result.renewAt.Ticks,
                    ExpiresAt = result.expiresAt.Ticks,
                    Current = tcs.Task,
                    Pending = tcs.Task
                };

                var xchgResult = _data;
                Data data;

                do
                {
                    data = xchgResult;

                    if (data.Pending != tcs.Task)
                        break;

                    xchgResult = Interlocked.CompareExchange(ref _data, pending, data);

                } while (data != xchgResult);

                tcs.SetResult(result.value);
            }
            catch (Exception ex)
            {
                var data = _data;

                if (data.Current == tcs.Task)
                {
                    Interlocked.CompareExchange(ref _data, new Data(), data);
                }
                else if (data.Pending == tcs.Task)
                {
                    Interlocked.CompareExchange(ref _data, new Data
                    {
                        RenewAt = data.RenewAt,
                        ExpiresAt = data.ExpiresAt,
                        Current = data.Current,
                        Pending = data.Current
                    }, data);
                }

                tcs.SetException(ex);
            }
        }

        public TaskAwaiter<T> GetAwaiter() => Task.GetAwaiter();

        public void Invalidate() => _data = new Data();


    }
}

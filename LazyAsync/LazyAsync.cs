using System;
using System.Threading.Tasks;

namespace LazyAsync
{
    public class LazyAsync
    {
        public static LazyAsync<T> Create<T>(
            Func<Task<(T value, DateTime renewAt, DateTime expiresAt)>> eval,
            Task<(T value, DateTime renewAt, DateTime expiresAt)> initial = null)
        {
            return new LazyAsync<T>(eval, initial);
        }

        public static LazyAsync<T> Create<T>(
            TimeSpan renewAge,
            TimeSpan expiresAge,
            Func<Task<T>> eval)
        {
            return new LazyAsync<T>(() => eval().ContinueWith(t => (t.Result, DateTime.UtcNow + renewAge, DateTime.UtcNow + expiresAge), TaskContinuationOptions.ExecuteSynchronously), null, eval.Method.ToString());
        }

        public static LazyAsync<T> Create<T>(
            TimeSpan renewAge,
            Func<Task<T>> eval)
        {
            return new LazyAsync<T>(() => eval().ContinueWith(t => (t.Result, DateTime.UtcNow + renewAge, DateTime.MaxValue), TaskContinuationOptions.ExecuteSynchronously), null, eval.Method.ToString());
        }
    }

    
}

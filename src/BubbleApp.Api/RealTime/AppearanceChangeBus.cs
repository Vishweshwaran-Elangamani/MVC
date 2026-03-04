using System.Collections.Concurrent;

namespace BubbleApp.Api.RealTime
{
    /// <summary>
    /// Minimal "notify-and-wait" bus for appearance changes. No DI, no external deps.
    /// Controllers call Bump(workspaceId) after saving, and widgets use WaitForChangeAsync(...) to wait.
    /// </summary>
    internal static class AppearanceChangeBus
    {
        // last known version per workspace
        private static readonly ConcurrentDictionary<string, long> Versions =
            new(StringComparer.Ordinal);

        // waiters per workspace (long-poll clients)
        private static readonly ConcurrentDictionary<string, ConcurrentBag<TaskCompletionSource<long>>> Waiters =
            new(StringComparer.Ordinal);

        /// <summary>Signal change for a workspace and wake all waiters.</summary>
        public static long Bump(string workspaceId)
        {
            var v = DateTime.UtcNow.Ticks; // monotonic-ish version
            Versions[workspaceId] = v;

            if (Waiters.TryRemove(workspaceId, out var bag))
            {
                foreach (var tcs in bag)
                    tcs.TrySetResult(v);
            }
            return v;
        }

        /// <summary>
        /// If the version is newer than 'since', returns immediately; otherwise waits up to 'timeout'.
        /// Returns the current version (same as 'since' on timeout).
        /// </summary>
        public static async Task<long> WaitForChangeAsync(
            string workspaceId,
            long since,
            TimeSpan timeout,
            CancellationToken ct)
        {
            if (Versions.TryGetValue(workspaceId, out var current) && current > since)
                return current;

            var tcs = new TaskCompletionSource<long>(TaskCreationOptions.RunContinuationsAsynchronously);
            var bag = Waiters.GetOrAdd(workspaceId, _ => new());
            bag.Add(tcs);

            using var timeoutCts = new CancellationTokenSource(timeout);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

            try
            {
                return await tcs.Task.WaitAsync(linked.Token);
            }
            catch (OperationCanceledException)
            {
                // timeout or request aborted: return the old version
                return since;
            }
        }

        /// <summary>Get current version (0 if none yet).</summary>
        public static long Current(string workspaceId) =>
            Versions.TryGetValue(workspaceId, out var v) ? v : 0L;
    }
}
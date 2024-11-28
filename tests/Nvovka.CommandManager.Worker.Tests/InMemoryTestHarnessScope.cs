using MassTransit.Testing;

namespace Nvovka.CommandManager.Worker.Tests
{
    public sealed class InMemoryTestHarnessScope : IAsyncDisposable, IDisposable
    {
        private InMemoryTestHarnessScope(InMemoryTestHarness harness)
        {
            Harness = harness;
        }

        public InMemoryTestHarness Harness { get; }

        public static async Task<InMemoryTestHarnessScope> StartAsync(InMemoryTestHarness harness)
        {
            var scope = new InMemoryTestHarnessScope(harness);

            await scope.Harness.Start();

            return scope;
        }

        public async ValueTask DisposeAsync()
        {
            if (Harness is not null)
            {
                await Harness.Stop();
                Harness.Dispose();
            }
        }

        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }
    }
}

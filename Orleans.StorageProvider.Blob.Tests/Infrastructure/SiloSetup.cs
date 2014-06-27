using System;
using System.Diagnostics;

namespace Orleans.StorageProvider.Blob.Tests.Infrastructure
{
    public class SiloSetup : IDisposable
    {
        private static readonly UnitTestSiloOptions SiloOptions = new UnitTestSiloOptions
        {
            StartFreshOrleans = true,
            StartSecondary = false,
        };

        private static readonly UnitTestClientOptions ClientOptions = new UnitTestClientOptions
        {
            ResponseTimeout = TimeSpan.FromSeconds(30)
        };

        private readonly UnitTestSiloHost unitTestSiloHost;

        public SiloSetup()
        {
            this.unitTestSiloHost = new UnitTestSiloHost(SiloOptions, ClientOptions);
        }

        public void Dispose()
        {
            if (this.unitTestSiloHost == null)
            {
                return;
            }

            try
            {
                this.unitTestSiloHost.StopDefaultSilos();
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }
        }
    }
}
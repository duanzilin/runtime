// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using Xunit;

namespace System.Data.Common.Tests
{
    public class DbConnectionTests
    {
        private static volatile bool _wasFinalized;

        private class FinalizingConnection : DbConnection
        {
            public static void CreateAndRelease()
            {
                new FinalizingConnection();
            }

            protected override void Dispose(bool disposing)
            {
                if (!disposing)
                    _wasFinalized = true;
                base.Dispose(disposing);
            }

            public override string ConnectionString
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public override string Database
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override string DataSource
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override string ServerVersion
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override ConnectionState State
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override void ChangeDatabase(string databaseName)
            {
                throw new NotImplementedException();
            }

            public override void Close()
            {
                throw new NotImplementedException();
            }

            public override void Open()
            {
                throw new NotImplementedException();
            }

            protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
            {
                throw new NotImplementedException();
            }

            protected override DbCommand CreateDbCommand()
            {
                throw new NotImplementedException();
            }
        }

        private class DbProviderFactoryConnection : DbConnection
        {
            protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
            {
                throw new NotImplementedException();
            }

            public override void ChangeDatabase(string databaseName)
            {
                throw new NotImplementedException();
            }

            public override void Close()
            {
                throw new NotImplementedException();
            }

            public override void Open()
            {
                throw new NotImplementedException();
            }

            public override string ConnectionString { get; set; }
            public override string Database { get; }
            public override ConnectionState State { get; }
            public override string DataSource { get; }
            public override string ServerVersion { get; }

            protected override DbCommand CreateDbCommand()
            {
                throw new NotImplementedException();
            }

            protected override DbProviderFactory DbProviderFactory => TestDbProviderFactory.Instance;
        }

        private class TestDbProviderFactory : DbProviderFactory
        {
            public static DbProviderFactory Instance = new TestDbProviderFactory();
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsPreciseGcSupported))]
        public void CanBeFinalized()
        {
            FinalizingConnection.CreateAndRelease();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.True(_wasFinalized);
        }

        [Fact]
        [ActiveIssue("https://github.com/mono/mono/issues/15180", TestRuntimes.Mono)]
        public void ProviderFactoryTest()
        {
            DbProviderFactoryConnection con = new DbProviderFactoryConnection();
            PropertyInfo providerFactoryProperty = con.GetType().GetProperty("ProviderFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(providerFactoryProperty);
            DbProviderFactory factory = providerFactoryProperty.GetValue(con) as DbProviderFactory;
            Assert.NotNull(factory);
            Assert.Same(typeof(TestDbProviderFactory), factory.GetType());
            Assert.Same(TestDbProviderFactory.Instance, factory);
        }
    }
}

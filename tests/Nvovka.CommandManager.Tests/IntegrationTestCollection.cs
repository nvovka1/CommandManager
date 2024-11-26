using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nvovka.CommandManager.Tests;

[CollectionDefinition(nameof(TestsFixture))]
public class IntegrationTestCollection : ICollectionFixture<TestsFixture>;

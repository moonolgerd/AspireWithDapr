using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Immutable;

namespace Dapr.Analyzers.Tests;

[TestClass]
public class DaprActorAnalyzerTests
{
    private static async Task<Diagnostic[]> GetDiagnosticsAsync(string code)
    {
        var tree = CSharpSyntaxTree.ParseText(code);

        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Actors.IActor).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Actors.Runtime.Actor).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.Serialization.DataContractAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Text.Json.Serialization.JsonPropertyNameAttribute).Assembly.Location),
        };

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { tree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzer = new DaprActorAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));

        var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
        return diagnostics.Where(d => d.Id.StartsWith("DAPR")).ToArray();
    }

    [TestMethod]
    public async Task ActorInterface_WithoutIActor_ShouldReportDAPR001()
    {
        var code = @"
using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace Test
{
    public interface ITestActor
    {
        Task<string> GetDataAsync();
    }

    public class TestActor : Actor, ITestActor
    {
        public TestActor(ActorHost host) : base(host) { }
        
        public Task<string> GetDataAsync() => Task.FromResult(""test"");
    }
}";

        var diagnostics = await GetDiagnosticsAsync(code);
        var dapr001 = diagnostics.FirstOrDefault(d => d.Id == "DAPR001");

        Assert.IsNotNull(dapr001, "Expected DAPR001 diagnostic for interface missing IActor inheritance");
        Assert.IsTrue(dapr001.GetMessage().Contains("ITestActor"));
    }

    [TestMethod]
    public async Task EnumWithoutEnumMember_ShouldReportDAPR002()
    {
        var code = @"
namespace Test
{
    public enum TestEnum
    {
        Value1,
        Value2
    }
}";

        var diagnostics = await GetDiagnosticsAsync(code);
        var dapr002Diagnostics = diagnostics.Where(d => d.Id == "DAPR002").ToArray();

        Assert.AreEqual(2, dapr002Diagnostics.Length, "Expected DAPR002 diagnostics for enum members without EnumMember attribute");
        Assert.IsTrue(dapr002Diagnostics.Any(d => d.GetMessage().Contains("Value1")));
        Assert.IsTrue(dapr002Diagnostics.Any(d => d.GetMessage().Contains("Value2")));
    }

    [TestMethod]
    public async Task ActorMethodWithComplexParameter_ShouldReportDAPR005()
    {
        var code = @"
using Dapr.Actors;
using Dapr.Actors.Runtime;
using System.Threading.Tasks;

namespace Test
{
    public class ComplexType
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public interface ITestActor : IActor
    {
        Task ProcessDataAsync(ComplexType data);
    }

    public class TestActor : Actor, ITestActor
    {
        public TestActor(ActorHost host) : base(host) { }
        
        public Task ProcessDataAsync(ComplexType data) => Task.CompletedTask;
    }
}";

        var diagnostics = await GetDiagnosticsAsync(code);
        var dapr005 = diagnostics.FirstOrDefault(d => d.Id == "DAPR005");

        Assert.IsNotNull(dapr005, "Expected DAPR005 diagnostic for method parameter without serialization attributes");
        Assert.IsTrue(dapr005.GetMessage().Contains("ComplexType"));
    }

    [TestMethod]
    public async Task ActorMethodWithComplexReturnType_ShouldReportDAPR006()
    {
        var code = @"
using Dapr.Actors;
using Dapr.Actors.Runtime;
using System.Threading.Tasks;

namespace Test
{
    public class ComplexResult
    {
        public string Data { get; set; }
        public bool Success { get; set; }
    }

    public interface ITestActor : IActor
    {
        Task<ComplexResult> GetResultAsync();
    }

    public class TestActor : Actor, ITestActor
    {
        public TestActor(ActorHost host) : base(host) { }
        
        public Task<ComplexResult> GetResultAsync() => Task.FromResult(new ComplexResult());
    }
}";

        var diagnostics = await GetDiagnosticsAsync(code);
        var dapr006 = diagnostics.FirstOrDefault(d => d.Id == "DAPR006");

        Assert.IsNotNull(dapr006, "Expected DAPR006 diagnostic for method return type without serialization attributes");
        Assert.IsTrue(dapr006.GetMessage().Contains("ComplexResult"));
    }

    [TestMethod]
    public async Task ActorMethodWithCollectionOfComplexType_ShouldReportDAPR007()
    {
        var code = @"
using Dapr.Actors;
using Dapr.Actors.Runtime;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test
{
    public class Item
    {
        public string Name { get; set; }
    }

    public interface ITestActor : IActor
    {
        Task<List<Item>> GetItemsAsync();
    }

    public class TestActor : Actor, ITestActor
    {
        public TestActor(ActorHost host) : base(host) { }
        
        public Task<List<Item>> GetItemsAsync() => Task.FromResult(new List<Item>());
    }
}";

        var diagnostics = await GetDiagnosticsAsync(code);
        var dapr007 = diagnostics.FirstOrDefault(d => d.Id == "DAPR007");

        Assert.IsNotNull(dapr007, "Expected DAPR007 diagnostic for collection with complex element type");
        Assert.IsTrue(dapr007.GetMessage().Contains("List"));
        Assert.IsTrue(dapr007.GetMessage().Contains("Item"));
    }

    [TestMethod]
    public async Task ValidActorWithDataContract_ShouldNotReportDiagnostics()
    {
        var code = @"
using Dapr.Actors;
using Dapr.Actors.Runtime;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Test
{
    [DataContract]
    public class ValidComplexType
    {
        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public int Value { get; set; }
    }

    public interface ITestActor : IActor
    {
        Task<ValidComplexType> GetDataAsync(ValidComplexType input);
    }

    public class TestActor : Actor, ITestActor
    {
        public TestActor(ActorHost host) : base(host) { }
        
        public Task<ValidComplexType> GetDataAsync(ValidComplexType input) => Task.FromResult(input);
    }
}";

        var diagnostics = await GetDiagnosticsAsync(code);
        var complexTypeDiagnostics = diagnostics.Where(d => d.Id == "DAPR005" || d.Id == "DAPR006").ToArray();

        Assert.AreEqual(0, complexTypeDiagnostics.Length, "Should not report diagnostics for types with DataContract attribute");
    }

    [TestMethod]
    public async Task ActorMethodWithPrimitiveParameters_ShouldNotReportWarning()
    {
        var code = @"
using Dapr.Actors;
using Dapr.Actors.Runtime;
using System;
using System.Threading.Tasks;

namespace Test
{
    public interface ITestActor : IActor
    {
        Task<string> ProcessAsync(string input, int count, DateTime timestamp);
    }

    public class TestActor : Actor, ITestActor
    {
        public TestActor(ActorHost host) : base(host) { }
        
        public Task<string> ProcessAsync(string input, int count, DateTime timestamp) 
            => Task.FromResult($""{input}_{count}_{timestamp}"");
    }
}";

        var diagnostics = await GetDiagnosticsAsync(code);
        var parameterDiagnostics = diagnostics.Where(d => d.Id == "DAPR005" || d.Id == "DAPR006").ToArray();

        Assert.AreEqual(0, parameterDiagnostics.Length, "Should not report diagnostics for primitive types");
    }

    [TestMethod]
    public async Task EnumWithEnumMemberAttributes_ShouldNotReportWarning()
    {
        var code = @"
using System.Runtime.Serialization;

namespace Test
{
    public enum Season
    {
        [EnumMember]
        Spring,
        [EnumMember]
        Summer,
        [EnumMember]
        Fall,
        [EnumMember]
        Winter
    }
}";

        var diagnostics = await GetDiagnosticsAsync(code);
        var enumDiagnostics = diagnostics.Where(d => d.Id == "DAPR002").ToArray();

        Assert.AreEqual(0, enumDiagnostics.Length, "Should not report diagnostics for enum members with EnumMember attribute");
    }

    [TestMethod]
    public async Task RecordWithoutDataContract_ShouldReportDAPR008()
    {
        var code = @"
using System;

namespace Test
{
    public record Doodad(Guid Id, string Name, int Count);
}";

        var diagnostics = await GetDiagnosticsAsync(code);
        var dapr008 = diagnostics.FirstOrDefault(d => d.Id == "DAPR008");

        Assert.IsNotNull(dapr008, "Expected DAPR008 diagnostic for record without DataContract attribute");
        Assert.IsTrue(dapr008.GetMessage().Contains("Doodad"));
    }

    [TestMethod]
    public async Task RecordWithDataContractButMissingDataMember_ShouldReportDAPR008()
    {
        var code = @"
using System;
using System.Runtime.Serialization;

namespace Test
{
    [DataContract]
    public record Doodad(Guid Id, string Name, int Count);
}";
        var diagnostics = await GetDiagnosticsAsync(code);
        var dapr008Diagnostics = diagnostics.Where(d => d.Id == "DAPR008").ToArray();

        Assert.IsTrue(dapr008Diagnostics.Length > 0, "Expected DAPR008 diagnostics for record parameters without DataMember attributes");
    }

    [TestMethod]
    public async Task RecordWithProperDataContractAndDataMember_ShouldNotReportDAPR008()
    {
        var code = @"
using System;
using System.Runtime.Serialization;

namespace Test
{
    [DataContract]
    public record Doodad(
        [property: DataMember] Guid Id,
        [property: DataMember] string Name,
        [property: DataMember] int Count);
}";
        var diagnostics = await GetDiagnosticsAsync(code);
        var dapr008Diagnostics = diagnostics.Where(d => d.Id == "DAPR008").ToArray();

        Assert.AreEqual(0, dapr008Diagnostics.Length, "Should not report diagnostics for record with proper DataContract and DataMember attributes");
    }

    [TestMethod]
    public async Task RecordUsedInActorMethod_ShouldReportDAPR008WhenMissingAttributes()
    {
        var code = @"
using System;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using System.Threading.Tasks;

namespace Test
{
    public record UserData(string Name, int Age, DateTime CreatedAt);

    public interface ITestActor : IActor
    {
        Task<UserData> GetUserDataAsync(UserData input);
    }

    public class TestActor : Actor, ITestActor
    {
        public TestActor(ActorHost host) : base(host) { }
        
        public Task<UserData> GetUserDataAsync(UserData input) => Task.FromResult(input);
    }
}";
        var diagnostics = await GetDiagnosticsAsync(code);
        var dapr008 = diagnostics.FirstOrDefault(d => d.Id == "DAPR008");

        Assert.IsNotNull(dapr008, "Expected DAPR008 diagnostic for record used in Actor method without proper attributes");
        Assert.IsTrue(dapr008.GetMessage().Contains("UserData"));
    }

    [TestMethod]
    public async Task ActorClass_WithoutIActorInterface_ShouldReportDAPR009()
    {
        var code = @"
using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace Test
{
    public class TestActor : Actor
    {
        public TestActor(ActorHost host) : base(host) { }
        
        public void DoSomething() { }
    }
}";
        var diagnostics = await GetDiagnosticsAsync(code);
        var dapr009 = diagnostics.FirstOrDefault(d => d.Id == "DAPR009");

        Assert.IsNotNull(dapr009, "Expected DAPR009 diagnostic for Actor class without IActor interface");
        Assert.IsTrue(dapr009.GetMessage().Contains("TestActor"));
    }

    [TestMethod]
    public async Task ActorClass_WithIActorInterface_ShouldNotReportDAPR009()
    {
        var code = @"
using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace Test
{
    public interface ITestActor : IActor
    {
        void DoSomething();
    }

    public class TestActor : Actor, ITestActor
    {
        public TestActor(ActorHost host) : base(host) { }
        
        public void DoSomething() { }
    }
}";
        var diagnostics = await GetDiagnosticsAsync(code);
        var dapr009 = diagnostics.FirstOrDefault(d => d.Id == "DAPR009");

        Assert.IsNull(dapr009, "Should not report DAPR009 for Actor class with proper IActor interface");
    }
}

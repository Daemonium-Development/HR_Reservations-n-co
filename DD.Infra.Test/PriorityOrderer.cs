using Xunit.Abstractions;
using Xunit.Sdk;

namespace DD.Infra.Test;

public class PriorityOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(
        IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
    {
        var sorted = new SortedDictionary<int, List<TTestCase>>();
        foreach (var testCase in testCases)
        {
            var priority = testCase.TestMethod.Method
                .GetCustomAttributes(typeof(TestPriorityAttribute).AssemblyQualifiedName)
                .FirstOrDefault()
                ?.GetNamedArgument<int>("Priority") ?? 0;

            if (!sorted.ContainsKey(priority))
                sorted[priority] = [];
            sorted[priority].Add(testCase);
        }

        foreach (var group in sorted.Values)
            foreach (var testCase in group)
                yield return testCase;
    }
}
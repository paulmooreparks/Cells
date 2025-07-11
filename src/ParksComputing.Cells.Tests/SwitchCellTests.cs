using FluentAssertions;

using ParksComputing.Cells.Testing;

namespace ParksComputing.Cells.Tests;

public class SwitchCellTests {
    [Fact]
    public async Task Routes_ByPredicate_IntoCorrectBranch() {
        var tapA = new CaptureTapCell<int>();
        var tapB = new CaptureTapCell<int>();

        var mesh = Pathway
            .Start<int>()
            .Switch(x => x % 2 == 0)
                .Case(true, tapA)   // even → A
                .Case(false, tapB)   // odd  → B
            .EndSwitch()
            .Finally();

        var h = new TestHarness<int, int>(mesh);

        await h.RunAsync(4);
        await h.RunAsync(5);

        tapA.Captured.Should().ContainSingle(v => v == 4);
        tapB.Captured.Should().ContainSingle(v => v == 5);
    }
}

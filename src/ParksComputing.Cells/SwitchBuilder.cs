using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParksComputing.Cells;
internal sealed class SwitchBuilder<TStart, TCurrent>
    : ISwitchBuilder<TStart, TCurrent> {
    private readonly PathwayBuilder<TStart, TCurrent> _parent;
    private readonly Func<TCurrent, bool> _predicate;
    private readonly Dictionary<bool, ICell<TCurrent, TCurrent>> _branches = new();

    public SwitchBuilder(PathwayBuilder<TStart, TCurrent> parent,
                         Func<TCurrent, bool> predicate)
        => (_parent, _predicate) = (parent, predicate);

    public ISwitchBuilder<TStart, TCurrent> Case(
        bool branchValue,
        ICell<TCurrent, TCurrent> branch) {
        _branches[branchValue] = branch;
        return this;
    }

    public PathwayBuilder<TStart, TCurrent> EndSwitch() {
        var cell = new SwitchCell<TCurrent>(_predicate, _branches);
        return _parent.Then(cell);
    }
}

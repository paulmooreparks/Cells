namespace ParksComputing.Cells;

public interface ISwitchBuilder<TStart, TCurrent> {
    ISwitchBuilder<TStart, TCurrent> Case(
        bool branchValue,
        ICell<TCurrent, TCurrent> branch);

    PathwayBuilder<TStart, TCurrent> EndSwitch();
}

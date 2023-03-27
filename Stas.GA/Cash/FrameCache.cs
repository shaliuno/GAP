namespace Stas.GA;

public class FrameCache<T> :CachedValue<T> {
    private uint _frame;

    public FrameCache(Func<T> func) : base(func) {
        _frame = uint.MaxValue;
    }

    protected override bool Update(bool force) {
        if(_frame != ui.curr_frame || force) {
            _frame = ui.curr_frame;
            return true;
        }

        return false;
    }
}

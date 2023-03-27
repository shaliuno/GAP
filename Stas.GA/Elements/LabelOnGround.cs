namespace Stas.GA;

public class LabelOnGround : ObjectBase {
    //private readonly Lazy<string> debug;
    private readonly string debug;
    private readonly Lazy<long> labelInfo;

    public LabelOnGround(nint ptr, string name = "LabelOnGround") :base(ptr, name) {
        labelInfo = new Lazy<long>( 
             Label != null ? 
                Label.Address != default ? ui.m.Read<long>(Label.Address + 0x398) : 0   : 0
        );
      
        //debug = new Lazy<string>(() => ItemOnGround.HasComp<WorldItem>()
        //   ? ItemOnGround.GetComp<WorldItem>().ItemEntity?.GetComp<Base>()?.Name
        //   : ItemOnGround.Path);
    }

    public bool IsVisible => Label?.IsVisible ?? false;

    public Entity ItemOnGround {
        get {
            var readObjectAt = new Entity(Address+ 0x18);
            return readObjectAt.Address == default ? null : readObjectAt;
        }
    }

    public Element Label {
        get {
            var readObjectAt = new Element(Address+0x10);
            return readObjectAt.Address == default ? null : readObjectAt;
        }
    }

    //Temp solution for pick it, need test PickTest and PickTest2
    public bool CanPickUp {
        get {
            var label = Label;

            if(label != null)
                return ui.m.Read<long>(label.Address + 0x420) == 1;

            return true;
        }
    }

    public TimeSpan TimeLeft {
        get {
            if(CanPickUp) return new TimeSpan();
            if(labelInfo.Value == 0) 
                return MaxTimeForPickUp;
            var futureTime = ui.m.Read<int>(labelInfo.Value + 0x38);
            return TimeSpan.FromMilliseconds(futureTime - Environment.TickCount);
        }
    }

    //Temp solution for pick it
    public TimeSpan MaxTimeForPickUp =>
        TimeSpan.Zero; // !CanPickUp ? TimeSpan.FromMilliseconds(M.Read<int>(labelInfo.Value + 0x34)) : new TimeSpan();

    private long GetLabelInfo() {
        return Label != null ? Label.Address != default ? ui.m.Read<long>(Label.Address + 0x398) : 0 : 0;
    }

    public override string ToString() {
        if (ItemOnGround == null)
            return "ItemOnGround==null";
        else {
            if (ItemOnGround.GetComp<WorldItem>(out var wi)) {
                var ie = wi.ItemEntity;
                if (ie.GetComp<Base>(out var b)) {
                    return b.ItemBaseName + " [" + ItemOnGround.GetKey + "]"; ;
                }
            }
            return "WorldItem==null";
        }
    }
}
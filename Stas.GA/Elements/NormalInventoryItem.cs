namespace Stas.GA;

public class NormalInventoryItem :Element {
    private Entity _item;
    private readonly Lazy<NormalInventoryItemOffsets> cachedValue;

    public NormalInventoryItem(nint ptr, string name= "NormalInventoryItem") :base (ptr, name) {
        cachedValue = new Lazy<NormalInventoryItemOffsets>(() => ui.m.Read<NormalInventoryItemOffsets>(Address));
    }

    public virtual int InventPosX => cachedValue.Value.InventPosX; 
    public virtual int InventPosY => cachedValue.Value.InventPosY; 
    public virtual int ItemWidth => cachedValue.Value.Width; 
    public virtual int ItemHeight => cachedValue.Value.Height; 

    public Entity Item {
        get {
            if(_item == null) 
                _item = new Entity(cachedValue.Value.Item);
            return _item;
        }
    }

    public ToolTipType toolTipType => ToolTipType.InventoryItem;

    public Element ToolTip => new Element(Address + 0xB20);

    //0xB40 0xB48 some inf about image DDS
    public override string ToString() {
        Item.GetComp<Mods>(out var mods );
        return "x="+InventPosX+" y="+InventPosY+" w="+ItemWidth+" h="+ ItemHeight;
    }

    public enum ToolTipType {
        None,
        InventoryItem,
        ItemOnGround,
        ItemInChat
    }
}

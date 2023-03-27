namespace Stas.GA;

public class QuestRewardWindow : Element {
    public QuestRewardWindow(nint ptr, string name= "QuestRewardWindow") : base(ptr, name) {
    }
    private Element PossibleRewardsWrapper => GetChildFromIndices(5, 0, 0);
    public IList<Element> PossibleRewards => PossibleRewardsWrapper?.children;
    public Element CancelButton => GetChildAtIndex(3);
    public Element SelectOneRewardString => GetChildAtIndex(0);
}

namespace Stas.GA;

public class FilesContainer {
    public FilesFromMemory FilesFromMemory;

    BaseItemTypes _BaseItemTypes;
    UniversalFileWrapper<AtlasNode> _AtlasNodes;
    UniversalFileWrapper<BetrayalChoiceAction> _BetrayalChoiceActions;
    UniversalFileWrapper<BetrayalChoice> _BetrayalChoices;
    UniversalFileWrapper<BetrayalDialogue> _BetrayalDialogue;
    UniversalFileWrapper<BetrayalJob> _BetrayalJobs;
    UniversalFileWrapper<BetrayalRank> _BetrayalRanks;
    UniversalFileWrapper<BetrayalReward> _BetrayalRewards;
    UniversalFileWrapper<BetrayalTarget> _BetrayalTargets;
    UniversalFileWrapper<HeistJobRecord> _HeistJobs;
    UniversalFileWrapper<HeistChestRewardTypeRecord> _HeistChestRewardTypes;
    UniversalFileWrapper<HeistNpcRecord> _HeistNpcs;
    LabyrinthTrials _LabyrinthTrials;
    ModsDat _Mods;
    MonsterVarieties _MonsterVarieties;
    PassiveSkills _PassiveSkills;
    PropheciesDat _Prophecies;
    Quests _Quests;
    QuestStates _QuestStates;
    StatsDat _Stats;
    TagsDat _Tags;
    WorldAreas _WorldAreas;

    public FilesContainer() {
        ItemClasses = new ItemClasses();
        FilesFromMemory = new FilesFromMemory();

        ReloadFiles();
    }

    #region Misc

    public ItemClasses ItemClasses { get; }

    public long ActiveSkillsPtr => FindFile("Data/ActiveSkills.dat");
    public BaseItemTypes BaseItemTypes =>
        _BaseItemTypes ??= new BaseItemTypes(_Memory, () => FindFile("Data/BaseItemTypes.dat"));

    public ModsDat Mods => _Mods ??= new ModsDat(_Memory, () => FindFile("Data/Mods.dat"), Stats, Tags);
    public StatsDat Stats => _Stats ??= new StatsDat(_Memory, () => FindFile("Data/Stats.dat"));
    public TagsDat Tags => _Tags ??= new TagsDat(_Memory, () => FindFile("Data/Tags.dat"));
    public WorldAreas WorldAreas => _WorldAreas ??= new WorldAreas(_Memory, () => FindFile("Data/WorldAreas.dat"));
   

    public PassiveSkills PassiveSkills =>
        _PassiveSkills ??= new PassiveSkills(_Memory, () => FindFile("Data/PassiveSkills.dat"));

    public LabyrinthTrials LabyrinthTrials =>
        _LabyrinthTrials ??= new LabyrinthTrials(_Memory, () => FindFile("Data/LabyrinthTrials.dat"));

    public Quests Quests => _Quests ??= new Quests(_Memory, () => FindFile("Data/Quest.dat"));

    public QuestStates QuestStates =>
        _QuestStates ??= new QuestStates(_Memory, () => FindFile("Data/QuestStates.dat"));

    public MonsterVarieties MonsterVarieties =>
        _MonsterVarieties ??= new MonsterVarieties(_Memory, () => FindFile("Data/MonsterVarieties.dat"));

    public PropheciesDat Prophecies =>
        _Prophecies ??= new PropheciesDat(_Memory, () => FindFile("Data/Prophecies.dat"));

    public UniversalFileWrapper<AtlasNode> AtlasNodes =>
        _AtlasNodes ??= new AtlasNodes(_Memory, () => FindFile("Data/AtlasNode.dat"));

    #endregion Misc

    #region Betrayal

    public UniversalFileWrapper<BetrayalTarget> BetrayalTargets =>
        _BetrayalTargets ??=
            new UniversalFileWrapper<BetrayalTarget>(_Memory, () => FindFile("Data/BetrayalTargets.dat"));

    public UniversalFileWrapper<BetrayalJob> BetrayalJobs =>
        _BetrayalJobs ??= new UniversalFileWrapper<BetrayalJob>(_Memory, () => FindFile("Data/BetrayalJobs.dat"));

    public UniversalFileWrapper<BetrayalRank> BetrayalRanks =>
        _BetrayalRanks ??=
            new UniversalFileWrapper<BetrayalRank>(_Memory, () => FindFile("Data/BetrayalRanks.dat"));

    public UniversalFileWrapper<BetrayalReward> BetrayalRewards =>
        _BetrayalRewards ??=
            new UniversalFileWrapper<BetrayalReward>(_Memory, () => FindFile("Data/BetrayalTraitorRewards.dat"));

    public UniversalFileWrapper<BetrayalChoice> BetrayalChoises =>
        _BetrayalChoices ??=
            new UniversalFileWrapper<BetrayalChoice>(_Memory, () => FindFile("Data/BetrayalChoices.dat"));

    public UniversalFileWrapper<BetrayalChoiceAction> BetrayalChoiceActions =>
        _BetrayalChoiceActions ??=
            new UniversalFileWrapper<BetrayalChoiceAction>(_Memory,
                () => FindFile("Data/BetrayalChoiceActions.dat"));

    public UniversalFileWrapper<BetrayalDialogue> BetrayalDialogue =>
        _BetrayalDialogue ??=
            new UniversalFileWrapper<BetrayalDialogue>(_Memory, () => FindFile("Data/BetrayalDialogue.dat"));

    #endregion

    #region Heist

    public UniversalFileWrapper<HeistJobRecord> HeistJobs =>
        _HeistJobs ??= new UniversalFileWrapper<HeistJobRecord>(_Memory, () => FindFile("Data/HeistJobs.dat"));

    public UniversalFileWrapper<HeistChestRewardTypeRecord> HeistChestRewardType =>
        _HeistChestRewardTypes ??=
            new UniversalFileWrapper<HeistChestRewardTypeRecord>(_Memory,
                () => FindFile("Data/HeistChestRewardTypes.dat"));
    public UniversalFileWrapper<HeistNpcRecord> HeistNpcs => _HeistNpcs ??=
        new UniversalFileWrapper<HeistNpcRecord>(_Memory, () => FindFile("Data/HeistNPCs.dat"));

    #endregion

    #region Metamorph

    private UniversalFileWrapper<MetamorphMetaSkill> _MetamorphMetaSkills;
    private UniversalFileWrapper<MetamorphMetaSkillType> _MetamorphMetaSkillTypes;
    private UniversalFileWrapper<MetamorphMetaMonster> _MetamorphMetaMonsters;
    private UniversalFileWrapper<MetamorphRewardType> _MetamorphRewardTypes;
    private UniversalFileWrapper<MetamorphRewardTypeItemsClient> _MetamorphRewardTypeItemsClient;

    public UniversalFileWrapper<MetamorphMetaSkill> MetamorphMetaSkills =>
        _MetamorphMetaSkills ??=
            new UniversalFileWrapper<MetamorphMetaSkill>(_Memory,
                () => FindFile("Data/MetamorphosisMetaSkills.dat"));

    public UniversalFileWrapper<MetamorphMetaSkillType> MetamorphMetaSkillTypes =>
        _MetamorphMetaSkillTypes ??=
            new UniversalFileWrapper<MetamorphMetaSkillType>(_Memory,
                () => FindFile("Data/MetamorphosisMetaSkillTypes.dat"));

    public UniversalFileWrapper<MetamorphMetaMonster> MetamorphMetaMonsters =>
        _MetamorphMetaMonsters ??=
            new UniversalFileWrapper<MetamorphMetaMonster>(_Memory,
                () => FindFile("Data/MetamorphosisMetaMonsters.dat"));

    public UniversalFileWrapper<MetamorphRewardType> MetamorphRewardTypes =>
        _MetamorphRewardTypes ??=
            new UniversalFileWrapper<MetamorphRewardType>(_Memory,
                () => FindFile("Data/MetamorphosisRewardTypes.dat"));

    public UniversalFileWrapper<MetamorphRewardTypeItemsClient> MetamorphRewardTypeItemsClient =>
        _MetamorphRewardTypeItemsClient ??=
            new UniversalFileWrapper<MetamorphRewardTypeItemsClient>(_Memory,
                () => FindFile("Data/MetamorphosisRewardTypeItemsClient.dat"));

    #endregion

    #region Bestiary

    private BestiaryCapturableMonsters _BestiaryCapturableMonsters;

    public BestiaryCapturableMonsters BestiaryCapturableMonsters =>
        _BestiaryCapturableMonsters ??=
            new BestiaryCapturableMonsters(_Memory, () => FindFile("Data/BestiaryCapturableMonsters.dat"));

    private UniversalFileWrapper<BestiaryRecipe> _BestiaryRecipes;

    public UniversalFileWrapper<BestiaryRecipe> BestiaryRecipes =>
        _BestiaryRecipes ??=
            new UniversalFileWrapper<BestiaryRecipe>(_Memory, () => FindFile("Data/BestiaryRecipes.dat"));

    private UniversalFileWrapper<BestiaryRecipeComponent> _BestiaryRecipeComponents;

    public UniversalFileWrapper<BestiaryRecipeComponent> BestiaryRecipeComponents =>
        _BestiaryRecipeComponents ??=
            new UniversalFileWrapper<BestiaryRecipeComponent>(_Memory,
                () => FindFile("Data/BestiaryRecipeComponent.dat"));

    private UniversalFileWrapper<BestiaryGroup> _BestiaryGroups;

    public UniversalFileWrapper<BestiaryGroup> BestiaryGroups =>
        _BestiaryGroups ??=
            new UniversalFileWrapper<BestiaryGroup>(_Memory, () => FindFile("Data/BestiaryGroups.dat"));

    private UniversalFileWrapper<BestiaryFamily> _BestiaryFamilies;

    public UniversalFileWrapper<BestiaryFamily> BestiaryFamilies =>
        _BestiaryFamilies ??=
            new UniversalFileWrapper<BestiaryFamily>(_Memory, () => FindFile("Data/BestiaryFamilies.dat"));

    private UniversalFileWrapper<BestiaryGenus> _BestiaryGenuses;

    public UniversalFileWrapper<BestiaryGenus> BestiaryGenuses =>
        _BestiaryGenuses ??=
            new UniversalFileWrapper<BestiaryGenus>(_Memory, () => FindFile("Data/BestiaryGenus.dat"));

    #endregion

    #region NewAtlas

    private AtlasRegions _AtlasRegions;

    public AtlasRegions AtlasRegions =>
        _AtlasRegions ??= new AtlasRegions(_Memory, () => FindFile("Data/AtlasRegions.dat"));

    #endregion

    public Dictionary<string, FileInfo> AllFiles { get; private set; }
    public Dictionary<string, FileInfo> Metadata { get; } = new Dictionary<string, FileInfo>();

    public Dictionary<string, FileInfo> Data { get; private set; } =
        new Dictionary<string, FileInfo>();

    public Dictionary<string, FileInfo> OtherFiles { get; } = new Dictionary<string, FileInfo>();

    public Dictionary<string, FileInfo> LoadedInThisArea { get; private set; } =
        new Dictionary<string, FileInfo>(1024);


    public event EventHandler<Dictionary<string, FileInfo>> LoadedFiles;

    public void ReloadFiles() {
        using (new PerformanceTimer("Load files from memory")) {
            AllFiles = FilesFromMemory.GetAllFiles();
        }
    }

    public void ParseFiles(Dictionary<string, FileInfo> files) {
        foreach (var file in files) {
            if (file.Key[0] == 'M' && file.Key[8] == '/')
                Metadata[file.Key] = file.Value;
            else if (file.Key[0] == 'D' && file.Key[4] == '/' && file.Key.EndsWith(".dat"))
                Data[file.Key] = file.Value;
            else
                OtherFiles[file.Key] = file.Value;
        }

        Data = Data.OrderBy(x => x.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public void ParseFiles(int gameAreaChangeCount) {
        if (AllFiles != null) {
            LoadedInThisArea = new Dictionary<string, FileInfo>(1024);

            foreach (var file in AllFiles) {
                if (file.Value.ChangeCount == gameAreaChangeCount) LoadedInThisArea[file.Key] = file.Value;

                if (file.Key[0] == 'M' && file.Key[8] == '/')
                    Metadata[file.Key] = file.Value;
                else if (file.Key[0] == 'D' && file.Key[4] == '/' && file.Key.EndsWith(".dat"))
                    Data[file.Key] = file.Value;
                else
                    OtherFiles[file.Key] = file.Value;
            }

            LoadedFiles?.Invoke(this, LoadedInThisArea);
        }
    }

    public long FindFile(string name) {
        try {
            if (AllFiles.TryGetValue(name, out var result))
                return result.Ptr;
        } catch (KeyNotFoundException) {
            const string messageFormat = "Couldn't find the file in memory: {0}\nTry to restart the game.";
            ui.AddToLog(messageFormat, MessType.Error);
            Environment.Exit(1);
        }

        return 0;
    }
}
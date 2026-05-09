using STS2RitsuLib.Content;
using STS2RitsuLib.Keywords;

namespace Fighter;

public static class FighterKeywords
{
    public const string ThrowId = "FIGHTER_KEYWORD_THROW";
    public const string StarterId = "FIGHTER_KEYWORD_STARTER";
    public const string ComboId = "FIGHTER_KEYWORD_COMBO";
    public const string CancelId = "FIGHTER_KEYWORD_CANCEL";
    public const string SpecialId = "FIGHTER_KEYWORD_SPECIAL";
    public const string SuperId = "FIGHTER_KEYWORD_SUPER";
    public const string TipsyId = "FIGHTER_KEYWORD_TIPSY";

    public static ModKeywordDefinition? Throw { get; private set; }
    public static ModKeywordDefinition? Starter { get; private set; }
    public static ModKeywordDefinition? Combo { get; private set; }
    public static ModKeywordDefinition? Cancel { get; private set; }
    public static ModKeywordDefinition? Special { get; private set; }
    public static ModKeywordDefinition? Super { get; private set; }
    public static ModKeywordDefinition? Tipsy { get; private set; }

    public static void Register(ModKeywordRegistry registry)
    {
        Throw = registry.RegisterCardKeywordOwnedByLocNamespace("THROW");
        Starter = registry.RegisterCardKeywordOwnedByLocNamespace("STARTER");
        Combo = registry.RegisterCardKeywordOwnedByLocNamespace("COMBO");
        Cancel = registry.RegisterCardKeywordOwnedByLocNamespace("CANCEL");
        Special = registry.RegisterCardKeywordOwnedByLocNamespace("SPECIAL");
        Super = registry.RegisterCardKeywordOwnedByLocNamespace("SUPER");
        Tipsy = registry.RegisterCardKeywordOwnedByLocNamespace("TIPSY");
    }
}

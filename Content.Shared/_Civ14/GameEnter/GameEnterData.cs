using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Civ14.GameEnter;

[Serializable, NetSerializable]
public class SendSpawnTextToPlayer(string text, float updateTime, float existTime, Color color) : EntityEventArgs
{
    public string Text => text;
    public float UpdateTime => updateTime;
    public float ExistTime => existTime;
    public Color Color => color;
}

[Prototype]
public partial class TextPresetsPrototype : IPrototype
{
    [IdDataField]
    public required string ID { get; set; }

    [DataField]
    public string MapName = "Dev";

    [DataField]
    public Dictionary<string, string> TextPresets = new();

    [DataField]
    public float UpdateTime = 10f;

    [DataField]
    public float ExistTime = 10f;

    [DataField]
    public string Color = "#FFFFFF";
}

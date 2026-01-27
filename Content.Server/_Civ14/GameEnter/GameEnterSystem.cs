using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.Maps;
using Content.Shared._Civ14.GameEnter;
using Content.Shared.GameTicking;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Server._Civ14.GameEnter;

public sealed class GameEnterSystem : EntitySystem
{
    [Dependency] private readonly GameTicker _gameTicker = null!;
    [Dependency] private readonly IGameMapManager _gameMapManager = null!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = null!;

    private readonly Dictionary<string, TextPresets> _textPresets = [];

    public override void Initialize()
    {
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawn);
        SubscribeLocalEvent<PrototypesReloadedEventArgs>(ReloadPresets);
        LoadPresets();
    }

    private void LoadPresets()
    {
        _textPresets.Clear();
        var presetsPrototypes = _prototypeManager.EnumeratePrototypes<TextPresetsPrototype>();
        foreach (var prototype in presetsPrototypes)
        {
            _textPresets.Add(prototype.MapName, new TextPresets(prototype.TextPresets, prototype.UpdateTime, prototype.ExistTime, prototype.Color));
        }
    }

    private void ReloadPresets(PrototypesReloadedEventArgs ev)
    {
        if (!ev.Modified.Contains(typeof(TextPresetsPrototype)))
            return;

        _textPresets.Clear();
        var presetsPrototypes = _prototypeManager.EnumeratePrototypes<TextPresetsPrototype>();
        foreach (var prototype in presetsPrototypes)
        {
            _textPresets.Add(prototype.MapName, new TextPresets(prototype.TextPresets, prototype.UpdateTime, prototype.ExistTime, prototype.Color));
        }
    }

    private void OnPlayerSpawn(PlayerSpawnCompleteEvent ev)
    {
        var map = _gameMapManager.GetSelectedMap();
        if (map is null)
            return;

        if (!_textPresets.TryGetValue(map.MapName, out var preset))
            return;

        if (_gameTicker.CurrentPreset?.ID is null)
            return;

        if (!preset.Presets.TryGetValue(_gameTicker.CurrentPreset.ID, out var text))
            return;

        var textEv = new SendSpawnTextToPlayer(text, preset.UpdateTime, preset.ExistTime, Color.FromHex(preset.Color));
        RaiseNetworkEvent(textEv, ev.Player);
    }
}

public readonly struct TextPresets(Dictionary<string, string> textPresets, float updateTime, float existTime, string color)
{
    public Dictionary<string, string> Presets => textPresets;
    public float UpdateTime => updateTime;
    public float ExistTime => existTime;
    public string Color => color;
}


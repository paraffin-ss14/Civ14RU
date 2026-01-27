using Content.Client.Stylesheets;
using Content.Shared._Civ14.GameEnter;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;

namespace Content.Client._Civ14.GameEntry;

public sealed class GameEntryClientSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = null!;
    [Dependency] private readonly IUserInterfaceManager _uiManager = null!;

    private Label? _text;
    private BoxContainer _boxContainer = null!;
    private string _textAppear = string.Empty;
    private TimeSpan _exist;
    private TimeSpan _updateTime;
    private TimeSpan _actualTime;
    public override void Initialize()
    {
        SubscribeNetworkEvent<SendSpawnTextToPlayer>(OnPlayerSpawned);
        _boxContainer = new BoxContainer
        {
            HorizontalExpand = true,
            VerticalExpand = true,
        };
        LayoutContainer.SetAnchorPreset(_boxContainer, LayoutContainer.LayoutPreset.Wide);
        _uiManager.WindowRoot.AddChild(_boxContainer);
    }

    private void OnPlayerSpawned(SendSpawnTextToPlayer ev)
    {
        if (_text != null)
        {
            _text.Dispose();
            _text = null;
        }

        _textAppear = ev.Text;
        _text = new Label
        {
            Text = ev.Text[..0],
            VerticalExpand = true,
            HorizontalExpand = true,
            StyleClasses = { StyleClass.LabelSignWoodHeading },
            VerticalAlignment = Control.VAlignment.Top,
            HorizontalAlignment = Control.HAlignment.Center,
            Align = Label.AlignMode.Center,
            FontColorOverride = ev.Color,
            Margin = new Thickness(0, 200, 0, 0),
        };
        _updateTime = TimeSpan.FromSeconds(ev.UpdateTime);
        _exist = _gameTiming.CurTime + TimeSpan.FromSeconds(ev.ExistTime);

        _boxContainer.AddChild(_text);
    }

    public override void Update(float deltaTime)
    {
        if (_text == null)
            return;

        if (_text.Text == null)
        {
            _text.Dispose();
            _text = null;
            return;
        }

        if (_updateTime <= TimeSpan.Zero)
            _text.Text = _textAppear;

        TryAddNextChar();
        TryDelete();
    }

    private void TryDelete()
    {
        if (_exist > _gameTiming.CurTime)
            return;

        _text!.Dispose();
        _text = null;
    }

    private void TryAddNextChar()
    {
        if (_actualTime > _gameTiming.CurTime)
            return;

        _actualTime = _gameTiming.CurTime + _updateTime;
        var charPos = _text!.Text!.Length + 1;

        if (_textAppear.Length >= charPos)
            _text.Text = _textAppear[..charPos];
    }
}

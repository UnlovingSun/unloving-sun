using System.Linq;
using System.Numerics;
using Content.Shared.Ghost;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.UserInterface.Systems.Ghost.Controls
{
    [GenerateTypedNameReferences]
    public sealed partial class GhostTargetWindow : DefaultWindow
    {
        private List<(string, NetEntity)> _warps = new();

        public event Action<NetEntity>? WarpClicked;

        public GhostTargetWindow()
        {
            RobustXamlLoader.Load(this);
        }

        public void UpdateWarps(IEnumerable<GhostWarp> warps)
        {
            // Server COULD send these sorted but how about we just use the client to do it instead
            _warps = warps
                .OrderBy(w => w.IsWarpPoint)
                .ThenBy(w => w.DisplayName, Comparer<string>.Create(
                    (x, y) => string.Compare(x, y, StringComparison.Ordinal)))
                .Select(w =>
                {
                    var name = w.IsWarpPoint
                        ? Loc.GetString("ghost-target-window-current-button", ("name", w.DisplayName))
                        : w.DisplayName;

                    return (name, w.Entity);
                })
                .ToList();
        }

        public void Populate()
        {
            ButtonContainer.DisposeAllChildren();
            AddButtons();
        }

        private void AddButtons()
        {
            foreach (var (name, warpTarget) in _warps)
            {
                var currentButtonRef = new Button
                {
                    Text = name,
                    TextAlign = Label.AlignMode.Right,
                    HorizontalAlignment = HAlignment.Center,
                    VerticalAlignment = VAlignment.Center,
                    SizeFlagsStretchRatio = 1,
                    MinSize = new Vector2(340, 20),
                    ClipText = true,
                };

                currentButtonRef.OnPressed += _ => WarpClicked?.Invoke(warpTarget);

                ButtonContainer.AddChild(currentButtonRef);
            }
        }
    }
}

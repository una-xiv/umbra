using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.Sheets;

namespace Umbra.Markers.Library;

[Service]
internal class GatheringNodeMarkerFactory(
    IDataManager dataManager,
    IObjectTable objectTable,
    IPlayer      player,
    IZoneManager zoneManager
) : WorldMarkerFactory
{
    public override string Id          { get; } = "GatheringNodeMarkers";
    public override string Name        { get; } = I18N.Translate("Markers.GatheringNodes.Name");
    public override string Description { get; } = I18N.Translate("Markers.GatheringNodes.Description");

    private          int                 _displayIndex;
    private readonly List<GatheringNode> _gatheringNodes = [];
    private readonly List<GatheringNode> _farAwayNodes   = [];

    public override List<IMarkerConfigVariable> GetConfigVariables()
    {
        return [
            ..DefaultStateConfigVariables,
            new BooleanMarkerConfigVariable(
                "ShowContents",
                I18N.Translate("Markers.GatheringNodes.ShowContents.Name"),
                I18N.Translate("Markers.GatheringNodes.ShowContents.Description"),
                true
            ),
            ..DefaultFadeConfigVariables
        ];
    }

    protected override void OnZoneChanged(IZone zone)
    {
        RemoveAllMarkers();
    }

    [OnTick(interval: 1000)]
    private void OnUpdate()
    {
        if (!GetConfigValue<bool>("Enabled") || !zoneManager.HasCurrentZone) {
            RemoveAllMarkers();
            return;
        }

        UpdateGatheringNodes();

        List<string> activeIds = [];

        var fadeDist       = GetConfigValue<int>("FadeDistance");
        var fadeAttn       = GetConfigValue<int>("FadeAttenuation");
        var showCtns       = GetConfigValue<bool>("ShowContents");
        var maxVisDistance = GetConfigValue<int>("MaxVisibleDistance");

        foreach (GatheringNode node in _gatheringNodes) {
            activeIds.Add(node.Key);

            SetMarker(
                new() {
                    MapId              = zoneManager.CurrentZone.Id,
                    Key                = node.Key,
                    Position           = node.Position,
                    IconId             = node.IconId,
                    Label              = node.Label,
                    SubLabel           = showCtns ? node.SubLabel : null,
                    ShowOnCompass      = node.ShowDirection && GetConfigValue<bool>("ShowOnCompass"),
                    FadeDistance       = new(fadeDist, fadeDist + fadeAttn),
                    MaxVisibleDistance = maxVisDistance,
                }
            );
        }

        RemoveMarkersExcept(activeIds);
    }

    private void UpdateGatheringNodes()
    {
        _gatheringNodes.Clear();

        List<string> activeKeys = [];

        foreach (var obj in objectTable) {
            if (!obj.IsTargetable
                || obj.ObjectKind != ObjectKind.GatheringPoint)
                continue;

            var node = CreateNodeFromObject(obj);
            if (node == null) continue;

            _gatheringNodes.Add(node.Value);
            activeKeys.Add(CreateLocationKey(node.Value.Position));
        }

        if (!zoneManager.HasCurrentZone) return;

        zoneManager.CurrentZone.DynamicMarkers.Where(t => t.Type == ZoneMarkerType.GatheringNode).ToList().ForEach(
            obj => {
                // Don't add these types of markers if we already got one from the object table.
                if (activeKeys.Contains(CreateLocationKey(obj.WorldPosition))) return;

                _gatheringNodes.Add(new() {
                    Key           = $"GN_{obj.WorldPosition.X:N0}_{obj.WorldPosition.Y:N0}_{obj.WorldPosition.Z:N0}",
                    Position      = obj.WorldPosition,
                    IconId        = obj.IconId,
                    Label         = $"{obj.Name}",
                    SubLabel      = null,
                    ShowDirection = true,
                });
            }
        );
    }

    private static string CreateLocationKey(Vector3 position)
    {
        var x = (int)(position.X / 5) * 5;
        var z = (int)(position.Z / 5) * 5;

        return $"{x}:{z}";
    }

    private GatheringNode? CreateNodeFromObject(IGameObject obj)
    {
        var point = dataManager.GetExcelSheet<GatheringPoint>().FindRow(obj.DataId);
        if (point == null) return null;

        List<string> items = point.Value.GatheringPointBase.Value!
            .Item.Select(
                i => {
                    if (i.RowId == 0) return null;

                    var gItem = dataManager.GetExcelSheet<GatheringItem>().FindRow(i.RowId);

                    return gItem == null
                        ? null
                        : dataManager.GetExcelSheet<Item>().FindRow(gItem.Value.Item.RowId)?.Name.ToString();
                }
            )
            .Where(i => i != null)
            .ToList()!;

        return new GatheringNode {
            Key           = $"GN_{obj.Position.X:N0}_{obj.Position.Y:N0}_{obj.Position.Z:N0}",
            Position      = obj.Position,
            IconId        = (uint)(point.Value.GatheringPointBase.ValueNullable?.GatheringType.ValueNullable?.IconMain ?? 0),
            Label         = $"Lv.{point.Value.GatheringPointBase.Value.GatheringLevel} {obj.Name}",
            SubLabel      = items.Count > 0 ? $"{point.Value.Count}x {items[_displayIndex % items.Count]}" : null,
            ShowDirection = !(!player.IsDiving && point.Value.GatheringPointBase.ValueNullable?.GatheringType.RowId == 5),
        };
    }

    [OnTick(interval: 2000)]
    internal void IncreaseDisplayIndex()
    {
        _displayIndex++;

        if (_displayIndex > 1000) _displayIndex = 0;
    }

    private struct GatheringNode
    {
        public string  Key;
        public Vector3 Position;
        public uint    IconId;
        public string  Label;
        public string? SubLabel;
        public bool    ShowDirection;
    }
}

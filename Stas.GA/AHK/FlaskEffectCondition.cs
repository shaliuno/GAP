﻿using ImGuiNET;


namespace Stas.GA;
/// <summary>
///     For triggering an action when flask effect is not active on player.
///     NOTE: will not trigger the action if flask isn't available on the slot.
/// </summary>
public class FlaskEffectCondition : ICondition {
    private static readonly FlaskEffectCondition ConfigurationInstance = new(1);

    [JsonInclude] private int flaskSlot;
    [JsonInclude] private IComponent component;
    private IntPtr flaskAddressCache = IntPtr.Zero;
    private List<string> flaskBuffsCache = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="FlaskEffectCondition" /> class.
    /// </summary>
    /// <param name="flaskSlot">flask number whos effect to use in the condition.</param>
    public FlaskEffectCondition(int flaskSlot) {
        this.flaskSlot = flaskSlot;
        this.component = null;
    }

    /// <summary>
    ///     Draws the ImGui widget for adding the condition.
    /// </summary>
    /// <returns>
    ///     <see cref="ICondition" /> if user wants to add the condition, otherwise null.
    /// </returns>
    public static FlaskEffectCondition Add() {
        ConfigurationInstance.ToImGui();
        ImGui.SameLine();
        if (ImGui.Button("Add##FlaskEffect")) {
            return new FlaskEffectCondition(ConfigurationInstance.flaskSlot);
        }

        return null;
    }

    /// <inheritdoc />
    public void Display(bool expand) {
        this.ToImGui(expand);
        this.component?.Display(expand);
    }

    /// <inheritdoc/>
    public void Add(IComponent component) {
        this.component = component;
    }

    /// <inheritdoc />
    public bool Evaluate() {
        var flask = ui.flasks[0, this.flaskSlot - 1];
        if (flask.Address == IntPtr.Zero) {
            return this.component != null && this.component.execute(false);
        }

        if (flask.Address != this.flaskAddressCache) {
            if (flask.GetComp<Base>(out var baseComponent)) {
                if (AHK.FlaskNameToBuffGroups.TryGetValue(baseComponent.ItemBaseName, out var buffNames)) {
                    this.flaskBuffsCache = buffNames;
                    this.flaskAddressCache = flask.Address;
                }
                else {
                    var allItems = ui.flasks.Items.Values;
                    var data = string.Empty;
                    foreach (var item in allItems) {
                        data += item.Path;
                        data += " ";
                    }

                    throw new Exception($"New (IsValid={flask.IsValid}) flask base found " +
                        $"{baseComponent.ItemBaseName}. Please let the developer know. All items in list are {data}");
                }
            }
        }

        if (ui.me.GetComp<Buffs>(out var buffs)) {
            //var have = "";
            //foreach (var b in buffs.StatusEffects)
            //    have += b.Key+" ";
            if (!this.flaskBuffsCache.Any(buffName => buffs.StatusEffects.ContainsKey(buffName))) {
                //ui.AddToLog("must use " + flaskSlot + " have="+ have + " need=" + flaskBuffsCache[0]);
                return this.component == null || this.component.execute(true);
            }
        }
        return this.component != null && this.component.execute(false);
    }

    private void ToImGui(bool expand = true) {
        if (expand) {
            ImGui.Text("I does't have effect of flask");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(40);
            ImGui.DragInt("##FlaskEffectFlaskSlot", ref this.flaskSlot, 0.05f, 1, 5);
        }
        else {
            ImGui.Text("Flask");
            ImGui.SameLine();
            ImGui.TextColored(new System.Numerics.Vector4(255, 255, 0, 255), $"{this.flaskSlot}");
            ImGui.SameLine();
            ImGui.Text("is not activated");
        }

    }
}
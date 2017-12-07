using UnityEngine;

/// <summary>
/// Allows a player to move a bardmage.
/// </summary>
public class PlayerControl : BaseControl {

    /// <summary>
    /// Gets the directional input to move the bardmage with.
    /// </summary>
    /// <returns>The directional input to move the bardmage with.</returns>
    protected override Vector2 GetDirectionInput() {
        return new Vector2(ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickX, player),
            ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickY, player));
    }

    /// <summary>
    /// Checks if the bardmage turns gradually.
    /// </summary>
    /// <returns>Whether the bardmage turns gradually.</returns>
    protected override bool GetGradualTurn() {
        return ControllerManager.instance.PlayerControlType(player) == ControllerManager.ControlType.Keyboard;
    }
}

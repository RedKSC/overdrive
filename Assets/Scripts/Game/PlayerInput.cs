using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerInput : MonoBehaviour {
    //Whether or not to use GetAxisRaw or just GetAxis for Axis
    public static bool RawAxisInput = true;

    static PlayerInputState inputState;
    static InputBuffer[] InputBuffers = new InputBuffer[(int)PlayerInputBuffer.COUNT];

    public struct InputBuffer {
        public PlayerInputKey inputKey;
        public PlayerInputType inputType;
        public float time;
    }

    private void Awake() {
        inputState = new PlayerInputState() {
            Player = ReInput.players.GetPlayer(0),
            Buttons = new float[(int)PlayerInputKey.COUNT],
            ButtonsPrev = new float[(int)PlayerInputKey.COUNT],
        };

        CreateButtonBuffer(PlayerInputKey.Shoot, PlayerInputBuffer.ShootPressed, PlayerInputType.Press);
    }
    
    private void Update() {
        for (int i = 0; i < (int)PlayerInputKey.COUNT; i++) {
            inputState.SetButton(i, RawAxisInput ?
                inputState.Player.GetAxisRaw(i) :
                inputState.Player.GetAxis(i));
            inputState.SetButtonPrev(i, inputState.Player.GetAxisRawPrev(i));

            if (inputState.Player.GetAxisRaw(i) == 1) {
                for (int q = 0; q < InputBuffers.Length; q++) {
                    if ((int)InputBuffers[q].inputKey == i && InputBuffers[q].inputType == PlayerInputType.Hold) {
                        InputBuffers[q].time = Time.time;
                    }
                }
            }
            if (inputState.Player.GetAxisRaw(i) == 1 && inputState.Player.GetAxisRawPrev(i) == 0) {
                for (int q = 0; q < InputBuffers.Length; q++) {
                    if ((int)InputBuffers[q].inputKey == i && InputBuffers[q].inputType == PlayerInputType.Press) {
                        InputBuffers[q].time = Time.time;
                    }
                }
            }
        }
    }

    public static bool GetButton(PlayerInputKey key) => inputState.GetButton((int)key);
    public static bool GetButtonPrev(PlayerInputKey key) => inputState.GetButtonPrev((int)key);
    public static bool GetButtonDown(PlayerInputKey key) => !inputState.GetButtonPrev((int)key) && inputState.GetButton((int)key);
    public static bool GetButtonUp(PlayerInputKey key) => inputState.GetButtonPrev((int)key) && !inputState.GetButton((int)key);
    public static bool GetRevButtonDown(PlayerInputKey key) => !inputState.GetRevButtonPrev((int)key) && inputState.GetRevButton((int)key);
    public static float GetAxis(PlayerInputKey key) => inputState.GetAxis((int)key);
    public static float GetAxisPrev(PlayerInputKey key) => inputState.GetAxisPrev((int)key);
    public static float GetAxisDelta(PlayerInputKey key) => inputState.GetAxis((int)key) - inputState.GetAxisPrev((int)key);

    public static void CreateButtonBuffer(PlayerInputKey key, PlayerInputBuffer buffer, PlayerInputType type) {
        InputBuffers[(int)buffer] = new InputBuffer() {
            inputKey = key,
            inputType = type,
            time = Time.time,
        };
    }
    public static float GetButtonBuffer(PlayerInputBuffer buffer) {
        return Time.time - InputBuffers[(int)buffer].time;
    }
    public static float NullifyButtonBuffer(PlayerInputBuffer buffer) {
        return InputBuffers[(int)buffer].time = 0;
    }
}

public struct PlayerInputState {
    public Player Player;
    public float[] Buttons;
    public float[] ButtonsPrev;

    public void SetButton(int buttonID, float value) => Buttons[buttonID] = value;
    public void SetButtonPrev(int buttonID, float value) => ButtonsPrev[buttonID] = value;
    public float GetAxis(int buttonID) => Buttons[buttonID];
    public float GetAxisPrev(int buttonID) => ButtonsPrev[buttonID];
    public bool GetButton(int buttonID) => Buttons[buttonID] > 0f;
    public bool GetButtonPrev(int buttonID) => ButtonsPrev[buttonID] > 0f;
    public bool GetRevButton(int buttonID) => Buttons[buttonID] < 0f;
    public bool GetRevButtonPrev(int buttonID) => ButtonsPrev[buttonID] < 0f;


}

public enum PlayerInputKey {
    Horizontal,
    Vertical,
    Shoot,
    Map,
    Dash,
    ExWpn1,
    ExWpn2,
    ExWpn3,
    Roll,
    COUNT
}
public enum PlayerInputType {
    Press,
    Hold,
    Release
}

public enum PlayerInputBuffer {
    ShootPressed,
    DashPressed,
    StunPressed,
    COUNT
}
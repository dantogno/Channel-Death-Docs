//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Input/Controls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Controls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""159fb91d-d821-4576-b1ef-d62475549daf"",
            ""actions"": [
                {
                    ""name"": ""ChannelUp"",
                    ""type"": ""Button"",
                    ""id"": ""d76e8e05-9a3d-44bf-a2ad-ebbab959e790"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ChannelDown"",
                    ""type"": ""Button"",
                    ""id"": ""a0e196d8-efaf-4610-9324-8704bebd0365"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Input2"",
                    ""type"": ""Button"",
                    ""id"": ""85c213f6-9e1e-4d70-9a1b-59a446ba43ab"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Input4"",
                    ""type"": ""Button"",
                    ""id"": ""0e507b4f-2b4f-4fea-ae73-32b6985a7bb8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Input6"",
                    ""type"": ""Button"",
                    ""id"": ""c4739fed-ec73-4942-9690-52eb182ba1d5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Input8"",
                    ""type"": ""Button"",
                    ""id"": ""bc5dae79-434b-41ba-84e5-ccd7097f0a4e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""VolumeDown"",
                    ""type"": ""Button"",
                    ""id"": ""36c8f162-9657-4a1d-857b-8141a0f9126d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""VolumeUp"",
                    ""type"": ""Button"",
                    ""id"": ""3cdcfc94-d129-4f04-882d-e8a50d42711b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Mute"",
                    ""type"": ""Button"",
                    ""id"": ""3a820c2f-f6ef-4aa6-b9bd-29e5dd6b1812"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""UpPlusR"",
                    ""type"": ""Button"",
                    ""id"": ""2ef7471a-1250-47c4-b1a8-ab2e24823e37"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DownMinusL"",
                    ""type"": ""Button"",
                    ""id"": ""1450ef26-f43f-433d-90a5-f46a8a77d41c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""4c4febfe-a338-493b-b431-6d1d15e35aa0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9e28f94d-0e18-40b5-ac55-0059abedeb36"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChannelUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dcc16e60-88ac-4320-8c59-f9f8eec85269"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChannelDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b6560f04-8d86-423b-84e1-c0b095a1fcd2"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Input2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f83c1bd6-24c0-4129-90f5-23beb6e44a4c"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Input2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""81b383b7-8fad-40a5-9749-74f4ae43d2fe"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Input4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3ce6b96e-b0ba-44d9-849a-880ac733101d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Input4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0118218d-6531-42e6-8e94-f5165132ece2"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Input6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6b3b7915-ac41-439e-9301-e62560f1aeec"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Input6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""457c1aba-c6e7-4c06-bee3-839129ee355d"",
                    ""path"": ""<Keyboard>/8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Input8"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""88c8c8ca-9a0a-48ae-9692-3516e958b7c2"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Input8"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a548703b-caf3-4440-a3b9-3f8c6f12c133"",
                    ""path"": ""<Keyboard>/minus"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VolumeDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c9790b51-3061-496f-a9e6-685d53becae5"",
                    ""path"": ""<Keyboard>/equals"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VolumeUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""075ddd41-d25a-420a-8369-e1448f086f80"",
                    ""path"": ""<Keyboard>/backquote"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mute"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""06c2d17d-b956-4b2c-b72e-5f9e998358e1"",
                    ""path"": ""<Keyboard>/rightBracket"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UpPlusR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cb293304-3a40-4f92-a2c2-537e18344d0b"",
                    ""path"": ""<Keyboard>/leftBracket"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DownMinusL"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1e387554-16d6-4b26-a0a7-2cd87abd3fc1"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_ChannelUp = m_Gameplay.FindAction("ChannelUp", throwIfNotFound: true);
        m_Gameplay_ChannelDown = m_Gameplay.FindAction("ChannelDown", throwIfNotFound: true);
        m_Gameplay_Input2 = m_Gameplay.FindAction("Input2", throwIfNotFound: true);
        m_Gameplay_Input4 = m_Gameplay.FindAction("Input4", throwIfNotFound: true);
        m_Gameplay_Input6 = m_Gameplay.FindAction("Input6", throwIfNotFound: true);
        m_Gameplay_Input8 = m_Gameplay.FindAction("Input8", throwIfNotFound: true);
        m_Gameplay_VolumeDown = m_Gameplay.FindAction("VolumeDown", throwIfNotFound: true);
        m_Gameplay_VolumeUp = m_Gameplay.FindAction("VolumeUp", throwIfNotFound: true);
        m_Gameplay_Mute = m_Gameplay.FindAction("Mute", throwIfNotFound: true);
        m_Gameplay_UpPlusR = m_Gameplay.FindAction("UpPlusR", throwIfNotFound: true);
        m_Gameplay_DownMinusL = m_Gameplay.FindAction("DownMinusL", throwIfNotFound: true);
        m_Gameplay_Jump = m_Gameplay.FindAction("Jump", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private List<IGameplayActions> m_GameplayActionsCallbackInterfaces = new List<IGameplayActions>();
    private readonly InputAction m_Gameplay_ChannelUp;
    private readonly InputAction m_Gameplay_ChannelDown;
    private readonly InputAction m_Gameplay_Input2;
    private readonly InputAction m_Gameplay_Input4;
    private readonly InputAction m_Gameplay_Input6;
    private readonly InputAction m_Gameplay_Input8;
    private readonly InputAction m_Gameplay_VolumeDown;
    private readonly InputAction m_Gameplay_VolumeUp;
    private readonly InputAction m_Gameplay_Mute;
    private readonly InputAction m_Gameplay_UpPlusR;
    private readonly InputAction m_Gameplay_DownMinusL;
    private readonly InputAction m_Gameplay_Jump;
    public struct GameplayActions
    {
        private @Controls m_Wrapper;
        public GameplayActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @ChannelUp => m_Wrapper.m_Gameplay_ChannelUp;
        public InputAction @ChannelDown => m_Wrapper.m_Gameplay_ChannelDown;
        public InputAction @Input2 => m_Wrapper.m_Gameplay_Input2;
        public InputAction @Input4 => m_Wrapper.m_Gameplay_Input4;
        public InputAction @Input6 => m_Wrapper.m_Gameplay_Input6;
        public InputAction @Input8 => m_Wrapper.m_Gameplay_Input8;
        public InputAction @VolumeDown => m_Wrapper.m_Gameplay_VolumeDown;
        public InputAction @VolumeUp => m_Wrapper.m_Gameplay_VolumeUp;
        public InputAction @Mute => m_Wrapper.m_Gameplay_Mute;
        public InputAction @UpPlusR => m_Wrapper.m_Gameplay_UpPlusR;
        public InputAction @DownMinusL => m_Wrapper.m_Gameplay_DownMinusL;
        public InputAction @Jump => m_Wrapper.m_Gameplay_Jump;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void AddCallbacks(IGameplayActions instance)
        {
            if (instance == null || m_Wrapper.m_GameplayActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Add(instance);
            @ChannelUp.started += instance.OnChannelUp;
            @ChannelUp.performed += instance.OnChannelUp;
            @ChannelUp.canceled += instance.OnChannelUp;
            @ChannelDown.started += instance.OnChannelDown;
            @ChannelDown.performed += instance.OnChannelDown;
            @ChannelDown.canceled += instance.OnChannelDown;
            @Input2.started += instance.OnInput2;
            @Input2.performed += instance.OnInput2;
            @Input2.canceled += instance.OnInput2;
            @Input4.started += instance.OnInput4;
            @Input4.performed += instance.OnInput4;
            @Input4.canceled += instance.OnInput4;
            @Input6.started += instance.OnInput6;
            @Input6.performed += instance.OnInput6;
            @Input6.canceled += instance.OnInput6;
            @Input8.started += instance.OnInput8;
            @Input8.performed += instance.OnInput8;
            @Input8.canceled += instance.OnInput8;
            @VolumeDown.started += instance.OnVolumeDown;
            @VolumeDown.performed += instance.OnVolumeDown;
            @VolumeDown.canceled += instance.OnVolumeDown;
            @VolumeUp.started += instance.OnVolumeUp;
            @VolumeUp.performed += instance.OnVolumeUp;
            @VolumeUp.canceled += instance.OnVolumeUp;
            @Mute.started += instance.OnMute;
            @Mute.performed += instance.OnMute;
            @Mute.canceled += instance.OnMute;
            @UpPlusR.started += instance.OnUpPlusR;
            @UpPlusR.performed += instance.OnUpPlusR;
            @UpPlusR.canceled += instance.OnUpPlusR;
            @DownMinusL.started += instance.OnDownMinusL;
            @DownMinusL.performed += instance.OnDownMinusL;
            @DownMinusL.canceled += instance.OnDownMinusL;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
        }

        private void UnregisterCallbacks(IGameplayActions instance)
        {
            @ChannelUp.started -= instance.OnChannelUp;
            @ChannelUp.performed -= instance.OnChannelUp;
            @ChannelUp.canceled -= instance.OnChannelUp;
            @ChannelDown.started -= instance.OnChannelDown;
            @ChannelDown.performed -= instance.OnChannelDown;
            @ChannelDown.canceled -= instance.OnChannelDown;
            @Input2.started -= instance.OnInput2;
            @Input2.performed -= instance.OnInput2;
            @Input2.canceled -= instance.OnInput2;
            @Input4.started -= instance.OnInput4;
            @Input4.performed -= instance.OnInput4;
            @Input4.canceled -= instance.OnInput4;
            @Input6.started -= instance.OnInput6;
            @Input6.performed -= instance.OnInput6;
            @Input6.canceled -= instance.OnInput6;
            @Input8.started -= instance.OnInput8;
            @Input8.performed -= instance.OnInput8;
            @Input8.canceled -= instance.OnInput8;
            @VolumeDown.started -= instance.OnVolumeDown;
            @VolumeDown.performed -= instance.OnVolumeDown;
            @VolumeDown.canceled -= instance.OnVolumeDown;
            @VolumeUp.started -= instance.OnVolumeUp;
            @VolumeUp.performed -= instance.OnVolumeUp;
            @VolumeUp.canceled -= instance.OnVolumeUp;
            @Mute.started -= instance.OnMute;
            @Mute.performed -= instance.OnMute;
            @Mute.canceled -= instance.OnMute;
            @UpPlusR.started -= instance.OnUpPlusR;
            @UpPlusR.performed -= instance.OnUpPlusR;
            @UpPlusR.canceled -= instance.OnUpPlusR;
            @DownMinusL.started -= instance.OnDownMinusL;
            @DownMinusL.performed -= instance.OnDownMinusL;
            @DownMinusL.canceled -= instance.OnDownMinusL;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
        }

        public void RemoveCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGameplayActions instance)
        {
            foreach (var item in m_Wrapper.m_GameplayActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnChannelUp(InputAction.CallbackContext context);
        void OnChannelDown(InputAction.CallbackContext context);
        void OnInput2(InputAction.CallbackContext context);
        void OnInput4(InputAction.CallbackContext context);
        void OnInput6(InputAction.CallbackContext context);
        void OnInput8(InputAction.CallbackContext context);
        void OnVolumeDown(InputAction.CallbackContext context);
        void OnVolumeUp(InputAction.CallbackContext context);
        void OnMute(InputAction.CallbackContext context);
        void OnUpPlusR(InputAction.CallbackContext context);
        void OnDownMinusL(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
    }
}

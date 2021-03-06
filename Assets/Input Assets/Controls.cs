// GENERATED AUTOMATICALLY FROM 'Assets/Input Assets/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""ab249fb3-b5c1-4516-a3d9-6c6f3ea5ec95"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""101f2cf0-b6d8-45f0-96b6-db45898787f0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""cbacc269-7ac5-453b-b665-e8f3a0f2800e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""a1963189-0a40-46b4-b064-b7f1e245fde8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""9f8e6edc-5e70-4def-934d-2fb7bf2c1c05"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""57e7474a-5f31-4641-aa8b-15c5a44396c5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RangedAttack"",
                    ""type"": ""Button"",
                    ""id"": ""b029ba26-1e5d-46d1-ab62-c502c4a6546f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dodge"",
                    ""type"": ""Button"",
                    ""id"": ""8f17d74e-4099-4322-b560-bed6cc1eb38e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""4943f965-1c96-4148-8762-0de3e9549957"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""UserInterface"",
                    ""type"": ""Button"",
                    ""id"": ""ac1368cc-7c9f-4c39-9005-5875a9dab03a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Movement"",
                    ""id"": ""a345af7f-327c-4730-9716-84e8a527b2c3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""2135b2b3-5774-4ea0-9e0f-1a8d40394c1b"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""M&K"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""85d082a0-f768-4160-a17e-2f44f98f2310"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""M&K"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0a2fa5ab-5ae6-4110-8102-0884ff2ab77d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""M&K"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""8c2f16c1-a15e-45ef-af08-110d8d293056"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""M&K"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""9c9c8845-9f68-4ce9-808a-997bb219e96f"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""49a61875-84cf-40b3-b6f0-73074534cdfc"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""M&K"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""129c27e4-5ce6-4635-aed7-bd329a97ff18"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""M&K"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2c44d746-22de-4010-bc65-abc7f1c5cab9"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d04d0210-81d0-4696-8c37-4b9f4a88c856"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""M&K"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7e8140a6-b449-4ebd-9ad7-43f1bc6bbd86"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""abc833f5-fe84-4f93-a589-5a04b1a35a48"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""M&K"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d9b42175-2e41-44a7-926b-7e9eb5831bfd"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""M&K"",
                    ""action"": ""RangedAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d52b8904-8a77-4611-b244-dae45a4bd2b8"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""M&K"",
                    ""action"": ""RangedAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""debd5772-815e-4929-90b3-5e4c388eae7c"",
                    ""path"": ""<Keyboard>/alt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""M&K"",
                    ""action"": ""Dodge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""52d17c8b-2df1-46b1-8d71-386027c5f3e3"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""M&K"",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a40a1291-0fd5-4e07-b73c-9cc02a2be764"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""M&K"",
                    ""action"": ""UserInterface"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UserInterface"",
            ""id"": ""434bef3d-4f66-4b33-83de-3d935e092847"",
            ""actions"": [
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""c7e645bb-3b9c-4729-afab-8c40b9a54e16"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""UserInterface"",
                    ""type"": ""Button"",
                    ""id"": ""35e1b39e-b8d7-4bd0-8c09-8bd7af153b85"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2fd23aa4-53d6-4396-b0ff-45df627053fb"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""M&K"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a7b951b0-ad6a-44d6-8ba4-25ea9cfa5ea4"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""M&K"",
                    ""action"": ""UserInterface"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""M&K"",
            ""bindingGroup"": ""M&K"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Attack = m_Player.FindAction("Attack", throwIfNotFound: true);
        m_Player_Interact = m_Player.FindAction("Interact", throwIfNotFound: true);
        m_Player_RangedAttack = m_Player.FindAction("RangedAttack", throwIfNotFound: true);
        m_Player_Dodge = m_Player.FindAction("Dodge", throwIfNotFound: true);
        m_Player_Sprint = m_Player.FindAction("Sprint", throwIfNotFound: true);
        m_Player_UserInterface = m_Player.FindAction("UserInterface", throwIfNotFound: true);
        // UserInterface
        m_UserInterface = asset.FindActionMap("UserInterface", throwIfNotFound: true);
        m_UserInterface_Interact = m_UserInterface.FindAction("Interact", throwIfNotFound: true);
        m_UserInterface_UserInterface = m_UserInterface.FindAction("UserInterface", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Look;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Attack;
    private readonly InputAction m_Player_Interact;
    private readonly InputAction m_Player_RangedAttack;
    private readonly InputAction m_Player_Dodge;
    private readonly InputAction m_Player_Sprint;
    private readonly InputAction m_Player_UserInterface;
    public struct PlayerActions
    {
        private @Controls m_Wrapper;
        public PlayerActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Look => m_Wrapper.m_Player_Look;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Attack => m_Wrapper.m_Player_Attack;
        public InputAction @Interact => m_Wrapper.m_Player_Interact;
        public InputAction @RangedAttack => m_Wrapper.m_Player_RangedAttack;
        public InputAction @Dodge => m_Wrapper.m_Player_Dodge;
        public InputAction @Sprint => m_Wrapper.m_Player_Sprint;
        public InputAction @UserInterface => m_Wrapper.m_Player_UserInterface;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Look.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Attack.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                @Interact.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                @RangedAttack.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRangedAttack;
                @RangedAttack.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRangedAttack;
                @RangedAttack.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRangedAttack;
                @Dodge.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDodge;
                @Dodge.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDodge;
                @Dodge.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDodge;
                @Sprint.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSprint;
                @UserInterface.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUserInterface;
                @UserInterface.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUserInterface;
                @UserInterface.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUserInterface;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @RangedAttack.started += instance.OnRangedAttack;
                @RangedAttack.performed += instance.OnRangedAttack;
                @RangedAttack.canceled += instance.OnRangedAttack;
                @Dodge.started += instance.OnDodge;
                @Dodge.performed += instance.OnDodge;
                @Dodge.canceled += instance.OnDodge;
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
                @UserInterface.started += instance.OnUserInterface;
                @UserInterface.performed += instance.OnUserInterface;
                @UserInterface.canceled += instance.OnUserInterface;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // UserInterface
    private readonly InputActionMap m_UserInterface;
    private IUserInterfaceActions m_UserInterfaceActionsCallbackInterface;
    private readonly InputAction m_UserInterface_Interact;
    private readonly InputAction m_UserInterface_UserInterface;
    public struct UserInterfaceActions
    {
        private @Controls m_Wrapper;
        public UserInterfaceActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Interact => m_Wrapper.m_UserInterface_Interact;
        public InputAction @UserInterface => m_Wrapper.m_UserInterface_UserInterface;
        public InputActionMap Get() { return m_Wrapper.m_UserInterface; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UserInterfaceActions set) { return set.Get(); }
        public void SetCallbacks(IUserInterfaceActions instance)
        {
            if (m_Wrapper.m_UserInterfaceActionsCallbackInterface != null)
            {
                @Interact.started -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnInteract;
                @UserInterface.started -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnUserInterface;
                @UserInterface.performed -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnUserInterface;
                @UserInterface.canceled -= m_Wrapper.m_UserInterfaceActionsCallbackInterface.OnUserInterface;
            }
            m_Wrapper.m_UserInterfaceActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @UserInterface.started += instance.OnUserInterface;
                @UserInterface.performed += instance.OnUserInterface;
                @UserInterface.canceled += instance.OnUserInterface;
            }
        }
    }
    public UserInterfaceActions @UserInterface => new UserInterfaceActions(this);
    private int m_MKSchemeIndex = -1;
    public InputControlScheme MKScheme
    {
        get
        {
            if (m_MKSchemeIndex == -1) m_MKSchemeIndex = asset.FindControlSchemeIndex("M&K");
            return asset.controlSchemes[m_MKSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnRangedAttack(InputAction.CallbackContext context);
        void OnDodge(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnUserInterface(InputAction.CallbackContext context);
    }
    public interface IUserInterfaceActions
    {
        void OnInteract(InputAction.CallbackContext context);
        void OnUserInterface(InputAction.CallbackContext context);
    }
}

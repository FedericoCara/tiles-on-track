using System;
using System.Collections.Generic;

using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


namespace Mimic {

    [Serializable]
    public class InputAdapter {

        #if ENABLE_INPUT_SYSTEM
        private static InputActionAsset input;
        private static InputActionAsset Input {
            get {
                if(input == null) {
                    input = Resources.Load<InputActionAsset>("Input");
                }
                return input;
            }
        }

        private static List<InputAction> actions;
        private static List<InputAction> Actions {
            get {
                if(actions == null) {
                    actions = new List<InputAction>(Input.FindActionMap("Main").actions);
                }
                return actions;
            }
        }
        #endif

        public static float GetAxis(string axis) {
            #if ENABLE_INPUT_SYSTEM
            return Input.FindActionMap("Main").FindAction(axis).ReadValue<float>();           
            #else
            return Input.GetAxis(axis);
            #endif
        }

        public static bool GetButton(string button) {
            #if ENABLE_INPUT_SYSTEM
            return Input.FindActionMap("Main").FindAction(button).ReadValue<bool>();           
            #else
            return Input.GetButton(button);
            #endif
        }

        public static bool GetButtonDown(string button) {
            #if ENABLE_INPUT_SYSTEM
            return Input.FindActionMap("Main").FindAction(button).triggered;       
            #else
            return Input.GetButtonDown(button);
            #endif
        }

        public static bool GetButtonUp(string button) {
            #if ENABLE_INPUT_SYSTEM
            return Input.FindActionMap("Main").FindAction(button).triggered;       
            #else
            return Input.GetButtonUp(button);
            #endif
        }

        public static bool GetKey(KeyCode key) {
            #if ENABLE_INPUT_SYSTEM
            InputAction action = GetActionWithKeyBinding(key);
            return action == null ? false : action.ReadValue<bool>();           
            #else
            return Input.GetKey(key);
            #endif
        }

        public static bool GetKeyDown(KeyCode key) {
            #if ENABLE_INPUT_SYSTEM        
            InputAction action = GetActionWithKeyBinding(key);
            return action == null ? false : action.triggered;  
            #else
            return Input.GetKeyDown(key);
            #endif
        }

        #if ENABLE_INPUT_SYSTEM
        public static InputAction GetActionWithKeyBinding(KeyCode key) {
            return Actions.Find(action => HasBinding(action, key));
        }

        public static bool HasBinding(InputAction action, KeyCode key) {
            foreach(InputBinding binding in action.bindings) {
                if(binding.path == "<Keyboard>/"+key) {
                    return true;
                }
            }
            return false;
        }
        #endif


        public static bool GetMouseButton(int button) {
            #if ENABLE_INPUT_SYSTEM
            return Input.FindActionMap("Main").FindAction("Mouse "+button).ReadValue<bool>();           
            #else
            return Input.GetMouseButton(button);
            #endif
        }

    }

}
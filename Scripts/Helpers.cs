using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;

namespace Pandora.Helpers {
    public enum ButtonOptions {
        Left, 
        Right,
        Forwards, 
        Backwards, 
        Submit,
        Jump, 
        Run, 
        Close, 
        Action, 
        Inventory, 
        InventorySlot1,
        InventorySlot2,
        InventorySlot3,
        InventorySlot4,
        InventorySlot5,
        Objectives
    }
    public static class Helpers {
        
        public static string ModifiedText(ButtonOptions ReplaceActionWith, string textToReplace) {
            string button = "";
            switch (ReplaceActionWith) {
                case ButtonOptions.Backwards:
                    button = InputManager.GetInputConfiguration (PlayerID.One).axes [1].negative.ToString ().Replace ("Alpha", "");
                    break;
                case ButtonOptions.Forwards:
                    button = InputManager.GetInputConfiguration (PlayerID.One).axes [1].positive.ToString ().Replace ("Alpha", "");
                    break;
                case ButtonOptions.Left:
                    button = InputManager.GetInputConfiguration (PlayerID.One).axes [0].negative.ToString ().Replace ("Alpha", "");
                    break;
                case ButtonOptions.Right:
                    button = InputManager.GetInputConfiguration (PlayerID.One).axes [0].positive.ToString ().Replace ("Alpha", "");
                    break;
                case ButtonOptions.Submit:
                    button = InputManager.GetInputConfiguration (PlayerID.One).axes [7].positive.ToString ().Replace ("Alpha", "");
                    break;
                case ButtonOptions.Jump:
                    button = InputManager.GetInputConfiguration (PlayerID.One).axes [6].positive.ToString ().Replace ("Alpha", "");
                    break;
                case ButtonOptions.Run:
                    button = InputManager.GetInputConfiguration (PlayerID.One).axes [9].positive.ToString ().Replace ("Alpha", "");
                    break;
                case ButtonOptions.Close:
                    button = InputManager.GetInputConfiguration (PlayerID.One).axes [17].positive.ToString ().Replace ("Alpha", "");
                    break;
                case ButtonOptions.Action:
                    button = InputManager.GetInputConfiguration (PlayerID.One).axes [10].positive.ToString ().Replace ("Alpha", "");
                    break;
                case ButtonOptions.Inventory:
                    button = InputManager.GetInputConfiguration (PlayerID.One).axes [11].positive.ToString ().Replace ("Alpha", "");
                    break;
                case ButtonOptions.InventorySlot1:
                    button = InputManager.GetInputConfiguration (PlayerID.One).axes [12].positive.ToString ().Replace ("Alpha", "");
                    break;
                case ButtonOptions.InventorySlot2:
                    button = InputManager.GetInputConfiguration (PlayerID.One).axes [13].positive.ToString ().Replace ("Alpha", "");
                    break;
                case ButtonOptions.InventorySlot3:
                    button = InputManager.GetInputConfiguration (PlayerID.One).axes [14].positive.ToString ().Replace ("Alpha", "");
                    break;
                case ButtonOptions.InventorySlot4:
                    button = InputManager.GetInputConfiguration (PlayerID.One).axes [15].positive.ToString ().Replace ("Alpha", "");
                    break;
                case ButtonOptions.InventorySlot5:
                    button = InputManager.GetInputConfiguration (PlayerID.One).axes [16].positive.ToString ().Replace ("Alpha", "");
                    break;
            }

            return textToReplace.Replace ("<ACTION>", button);
        }
    }
}
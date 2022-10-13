using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public enum UserDeviceType
    {
        HTC,
        PC
    }
    public class UserDeviceManager : MonoBehaviour
    {
        public static UserDeviceType GetDeviceUsed()
        {
            // Server execution

            string deviceUsed = "PC"; //AppConfig.Instance.DeviceUsed.ToLower(); //TODO setup APPConfig

            switch (deviceUsed)
            {
                case "htc":
                case "auto":
                    // Si l'app config demande du HTC mais que le casque n'est pas branché
                    Debug.LogWarning("AppConfig asked for HTC, but not active, so use PC version");
                    return UnityEngine.XR.XRSettings.isDeviceActive ? UserDeviceType.HTC : UserDeviceType.PC;

                case "pc":
                    return UserDeviceType.PC;

                default: // "auto" and others
                    return UnityEngine.XR.XRSettings.isDeviceActive ? UserDeviceType.HTC : UserDeviceType.PC;
            }
        }

        public static GameObject GetPrefabToSpawnWithDeviceUsed(GameObject pcPrefab, GameObject HTCPrefab)
        {
            UserDeviceType userDeviceType = GetDeviceUsed();
            switch (userDeviceType)
            {
                case UserDeviceType.HTC:
                    return HTCPrefab;
                case UserDeviceType.PC:
                default:
                    return pcPrefab;
            }
        }
    }

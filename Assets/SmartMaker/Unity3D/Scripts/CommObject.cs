using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;


namespace SmartMaker
{
    [Serializable]
    public class CommDevice
    {
        public string name;
        public string address;
        public List<string> args = new List<string>();

        public CommDevice()
        {

        }

        public CommDevice(CommDevice device)
        {
            name = device.name;
            address = device.address;
            for (int i = 0; i < device.args.Count; i++)
                args.Add(device.args[i]);
        }

        public bool Equals(CommDevice device)
        {
            if (device == null)
                return false;

            if (!name.Equals(device.name))
                return false;

            if (!address.Equals(device.address))
                return false;

            if (args.Count != device.args.Count)
                return false;

            for (int i = 0; i < args.Count; i++)
            {
                if (!args[i].Equals(device.args[i]))
                    return false;
            }

            return true;
        }
    }

    [AddComponentMenu("SmartMaker/Unity3D/Internal/CommObject")]
    public class CommObject : MonoBehaviour
    {
        [SerializeField]
        public List<CommDevice> foundDevices = new List<CommDevice>();
        [SerializeField]
        public CommDevice device;

        public UnityEvent OnOpen;
        public UnityEvent OnClose;
        public UnityEvent OnOpenFailed;
        public UnityEvent OnErrorClosed;
        public UnityEvent OnStartSearch;
        public UnityEvent OnStopSearch;
        public UnityEvent OnFoundDevice;

        protected bool platformSupport = false;

        public virtual void Open()
        {
        }

        public virtual void Close()
        {
        }

        protected virtual void ErrorClose()
        {

        }

        public virtual void StartSearch()
        {
        }

        public virtual void StopSearch()
        {
        }

        public virtual void Write(byte[] data)
        {
        }

        public virtual byte[] Read()
        {
            return null;
        }

        public virtual bool IsOpen
        {
            get
            {
                return false;
            }
        }

        public bool IsPlatformSupport
        {
            get
            {
                return platformSupport;
            }
        }
    }
}


namespace System.EnterpriseServices
{
    using Microsoft.Win32;
    using System;

    internal class BaseSwitch
    {
        private string _name;
        private int _value;

        internal BaseSwitch(string name)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(Path);
            this._name = name;
            if (key == null)
            {
                this._value = 0;
            }
            else
            {
                object obj2 = key.GetValue(name);
                if (obj2 != null)
                {
                    this._value = (int) obj2;
                }
            }
        }

        internal string Name =>
            this._name;

        internal static string Path =>
            @"SOFTWARE\Microsoft\COM3\System.EnterpriseServices";

        protected int Value =>
            this._value;
    }
}


using Godot;
using System;

namespace Haldric.Wdk
{
    public partial class Schedule : Node
    {
        public int _current = 0;

        public void Next()
        {
            _current = (_current + 1) % GetChildCount();
        }

        public Daytime GetCurrentDaytime()
        {
            return GetChild<Daytime>(_current);
        }
    }
}

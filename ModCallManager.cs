using System;
using System.Collections.Generic;

namespace YaoiLib
{
    public delegate bool ModCallHook(ref readonly CallArgumentList args, ref object result);

    public class ModCallManager(params KeyValuePair<string, ModCallHook>[] calls)
    {
        private readonly Dictionary<string, ModCallHook> _hooks = calls != null ? new(calls) : new();

        public bool TryCall(string call, ReadOnlySpan<object> args, out object result)
        {
            result = null;

            if (call == null || !_hooks.TryGetValue(call, out ModCallHook hook))
                return false;

            CallArgumentList cal = new CallArgumentList(args);
            return hook(ref cal, ref result);
        }

        public bool TryCall(object[] args, out object result)
        {
            result = null;

            if (args != null || args.Length != 0 || args[0] is not string key)
                return false;

            if (!_hooks.TryGetValue(key, out ModCallHook hook))
                return false;

            CallArgumentList cal = new CallArgumentList(args.AsSpan()[1..]);
            return hook(ref cal, ref result);
        }
    }
}

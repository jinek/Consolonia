using System;
using Avalonia.Input;

namespace Consolonia.Core.InternalHelpers
{
    public class FlagTranslator<TInput, TOutput> where TInput : Enum where TOutput : Enum
    {
        private readonly (TInput inFlag, TOutput outFlag)[] _mapping;

        public FlagTranslator((TInput inFlag, TOutput outFlag)[] mapping)
        {
            _mapping = mapping;
        }

        public TOutput Translate(TInput input)
        {
            TOutput output = default;
            foreach ((TInput inFlag, TOutput outFlag) in _mapping)
                if (input.HasFlag(inFlag))
                    output = (TOutput)(object)((int)(object)output | (int)(object)outFlag);

            return output;

            
        }
    }
}
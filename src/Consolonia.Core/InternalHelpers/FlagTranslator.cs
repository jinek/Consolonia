using System;

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
            return Translate(input, false);
        }

        public TOutput Translate(TInput input, bool singleValueOnly)
        {
            TOutput output = default;
            foreach ((TInput inFlag, TOutput outFlag) in _mapping)
                if (singleValueOnly)
                {
                    if (input.Equals(inFlag))
                        return outFlag;
                }
                else if (input.HasFlag(inFlag))
                {
                    output = (TOutput)(object)((int)(output as object)! | (int)(object)outFlag);
                }

            return output;
        }
    }
}
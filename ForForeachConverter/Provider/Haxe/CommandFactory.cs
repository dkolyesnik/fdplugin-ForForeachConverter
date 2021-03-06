﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.Collections.Generic;
using CodeRefactor.Commands;
using ForForeachConverter.Commands.Haxe;
using PluginCore.FRService;
using ScintillaNet;

namespace ForForeachConverter.Provider.Haxe
{
    using Command = RefactorCommand<IDictionary<string, List<SearchMatch>>>;

    class CommandFactory : ICommandFactory
    {
        public bool IsValidForConvertForeachToFor(ScintillaControl sci) => ConvertForeachToForCommand.IsValidForConvert(sci);
        public bool IsValidForConvertForeachToKeyValueIterator(ScintillaControl sci) => ConvertForeachToKeyValueIteratorCommand.IsValidForConvert(sci);
        public Command CreateConvertForeachToForCommand() => new ConvertForeachToForCommand();
        public Command CreateConvertForeachToKeyValueIteratorCommand() => new ConvertForeachToKeyValueIteratorCommand();
    }
}

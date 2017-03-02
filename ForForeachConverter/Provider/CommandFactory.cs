﻿using System.Collections.Generic;
using CodeRefactor.Commands;
using ForForeachConverter.Commands;
using PluginCore.FRService;

namespace ForForeachConverter.Provider
{
    using Command = RefactorCommand<IDictionary<string, List<SearchMatch>>>;

    class CommandFactory : ICommandFactory
    {
        public Command CreateForeachToForCommand() => new ForeachToForCommand();
    }
}

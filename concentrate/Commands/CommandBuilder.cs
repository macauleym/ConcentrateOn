using System.CommandLine;
using ConcentrateOn.Core.Enums;

namespace concentrate.Commands;

public class CommandBuilder(Command root)
{
    Command command = root;

    public Dictionary<string, Type> Arguments           = new();
    public Dictionary<string, (string[], Type)> Options = new();

    public CommandBuilder WithArgument<T>(string name)
    {
        var arg = new Argument<T>(name);
        command.Add(arg);
        Arguments[name] = typeof(T);

        return this;
    }

    public CommandBuilder ManyArgumentsOf(Dictionary<string, Type> arguments)
    {
        foreach (var arg in arguments)
            switch (arg.Value.Name)
            {
                case "String":
                    WithArgument<string>(arg.Key);
                    break;
                case "Int32":
                    WithArgument<int>(arg.Key);
                    break;
            }

        return this;
    }
    
    public CommandBuilder WithOption<T>(string name, params string[] aliases)
    {
        var op = new Option<T>(name, aliases);
        command.Add(op);
        Options[name] = (aliases, typeof(T));

        return this;
    }

    public CommandBuilder ManyOptionsOf(Dictionary<string, (string[], Type)> options)
    {
        foreach (var opt in options)
            switch (opt.Value.Item2.Name)
            {
                case "String":
                    WithOption<string>(opt.Key, opt.Value.Item1);
                    break;
                case "Int32":
                    WithOption<int>(opt.Key, opt.Value.Item1);
                    break;
                case "During":
                    WithOption<During>(opt.Key, opt.Value.Item1);
                    break;
            }

        return this;
    }

    public CommandBuilder WithSubCommand(Command sub)
    {
        command.Add(sub);

        return this;
    }

    public CommandBuilder WithAsyncHandler(Func<ParseResult, Task> handler)
    {
        command.SetAction(handler);

        return this;
    }

    public Command Build() =>
        command;
}

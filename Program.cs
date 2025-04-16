using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

// Abstract Product - Markdown Element
public abstract class MarkdownElement
{
    public abstract string Render();
    public abstract MarkdownElement Clone(); // Prototype Pattern
    public bool Processed { get; set; } = false; // for Iterator pattern
}

// Factory products
public class BoldText : MarkdownElement
{
    public override string Render() => "**Bold Text**";
    public override MarkdownElement Clone() => new BoldText();
}

public class ItalicText : MarkdownElement
{
    public override string Render() => "*Italic Text*";
    public override MarkdownElement Clone() => new ItalicText();
}

// Factory Method
public abstract class MarkdownElementFactory
{
    public abstract MarkdownElement CreateElement();
}

public class BoldTextFactory : MarkdownElementFactory
{
    public override MarkdownElement CreateElement() => new BoldText();
}

public class ItalicTextFactory : MarkdownElementFactory
{
    public override MarkdownElement CreateElement() => new ItalicText();
}

// Abstract Factory
public abstract class MarkdownProcessorFactory
{
    public abstract MarkdownElementFactory GetBoldFactory();
    public abstract MarkdownElementFactory GetItalicFactory();
}

// Factory itself that does the thing
public class SimpleMarkdownProcessorFactory : MarkdownProcessorFactory
{
    public override MarkdownElementFactory GetBoldFactory() => new BoldTextFactory();
    public override MarkdownElementFactory GetItalicFactory() => new ItalicTextFactory();
}

// Bob the Builder
public class MarkdownBuilder
{
    private List<MarkdownElement> elements = new List<MarkdownElement>();

    public MarkdownBuilder AddBold()
    {
        elements.Add(new BoldText());
        return this;
    }

    public MarkdownBuilder AddItalic()
    {
        elements.Add(new ItalicText());
        return this;
    }

    public List<MarkdownElement> Build()
    {
        return elements;
    }
}

// Strategy Pattern
public interface IRenderStrategy
{
    void Render(List<MarkdownElement> elements);
}

public class ConsoleRenderStrategy : IRenderStrategy
{
    public void Render(List<MarkdownElement> elements)
    {
        foreach (var element in elements)
        {
            Console.WriteLine(element.Render());
        }
    }
}

// Observer Pattern - Plugin Watcher (The Watcher) ((Scug))
public interface IObserver
{
    void Update(string message);
}

public class PluginObserver : IObserver
{
    public void Update(string message)
    {
        Console.WriteLine($"Plugin Watcher: {message}");
    }
}

public class PluginWatcher
{
    private readonly List<IObserver> observers = new List<IObserver>();
    private readonly string pluginDirectory;
    private FileSystemWatcher fileWatcher;

    public PluginWatcher(string pluginDirectory)
    {
        this.pluginDirectory = pluginDirectory;
    }

    public void Attach(IObserver observer)
    {
        observers.Add(observer);
    }

    public void Notify(string message)
    {
        foreach (var observer in observers)
        {
            observer.Update(message);
        }
    }

    public void StartWatching()
    {
        if (!Directory.Exists(pluginDirectory))
        {
            Directory.CreateDirectory(pluginDirectory);
        }

        fileWatcher = new FileSystemWatcher(pluginDirectory)
        {
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
        };

        fileWatcher.Created += (sender, args) => Notify($"New plugin detected: {args.Name}");
        fileWatcher.Changed += (sender, args) => Notify($"Plugin updated: {args.Name}");
        fileWatcher.Deleted += (sender, args) => Notify($"Plugin removed: {args.Name}");
    }
}

// Command Pattern
public interface ICommand
{
    void Execute();
}

public class RenderCommand : ICommand
{
    private readonly IRenderStrategy renderStrategy;
    private readonly List<MarkdownElement> elements;

    public RenderCommand(IRenderStrategy renderStrategy, List<MarkdownElement> elements)
    {
        this.renderStrategy = renderStrategy;
        this.elements = elements;
    }

    public void Execute()
    {
        renderStrategy.Render(elements);
    }
}

// Macro Command Pattern
public class MacroCommand : ICommand
{
    private readonly List<ICommand> commands = new List<ICommand>();

    public void AddCommand(ICommand command)
    {
        commands.Add(command);
    }

    public void Execute()
    {
        foreach (var command in commands)
        {
            command.Execute();
        }
    }
}

// Template Method Pattern
public abstract class MarkdownProcessor
{
    protected List<MarkdownElement> elements;

    public void Process()
    {
        elements = LoadMarkdown();
        ParseMarkdown(elements);
        RenderMarkdown(elements);
    }

    protected abstract List<MarkdownElement> LoadMarkdown();
    protected abstract void ParseMarkdown(List<MarkdownElement> elements);
    protected abstract void RenderMarkdown(List<MarkdownElement> elements);
}

public class SimpleMarkdownProcessor : MarkdownProcessor
{
    protected override List<MarkdownElement> LoadMarkdown()
    {
        Console.WriteLine("Loading Markdown...");
        return new List<MarkdownElement> { new BoldText(), new ItalicText() };
    }

    protected override void ParseMarkdown(List<MarkdownElement> elements)
    {
        Console.WriteLine("Parsing Markdown...");
    }

    protected override void RenderMarkdown(List<MarkdownElement> elements)
    {
        Console.WriteLine("Rendering Markdown...");
        foreach (var element in elements)
        {
            Console.WriteLine(element.Render());
        }
    }
}

// Iterator Pattern
public class MarkdownIterator : IEnumerator<MarkdownElement>
{
    private readonly List<MarkdownElement> _elements;
    private int _position = -1;

    public MarkdownIterator(List<MarkdownElement> elements)
    {
        _elements = elements;
    }

    public MarkdownElement Current => _elements[_position];
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        while (++_position < _elements.Count)
        {
            if (!_elements[_position].Processed)
                return true;
        }
        return false;
    }

    public void Reset() => _position = -1;
    public void Dispose() { }
}

// State Pattern
public interface IEditorState
{
    void Handle();
}

public class ReadOnlyState : IEditorState
{
    public void Handle() => Console.WriteLine("Editor is in read-only mode. Editing is disabled.");
}

public class EditableState : IEditorState
{
    public void Handle() => Console.WriteLine("Editor is editable. Changes are allowed.");
}

public class EditorContext
{
    private IEditorState state;

    public void SetState(IEditorState state)
    {
        this.state = state;
    }

    public void ApplyState()
    {
        state.Handle();
    }
}

// Chain of Responsibility Pattern
public abstract class TextHandler
{
    protected TextHandler next;

    public void SetNext(TextHandler next)
    {
        this.next = next;
    }

    public virtual void Handle(ref string text)
    {
        next?.Handle(ref text);
    }
}

public class TrimHandler : TextHandler
{
    public override void Handle(ref string text)
    {
        text = text.Trim();
        base.Handle(ref text);
    }
}

public class ReplaceTabsHandler : TextHandler
{
    public override void Handle(ref string text)
    {
        text = text.Replace("\t", "    ");
        base.Handle(ref text);
    }
}

// Interpreter Pattern
public interface IMarkdownExpression
{
    string Interpret();
}

public class MathExpression : IMarkdownExpression
{
    private string _expression;
    public MathExpression(string expression)
    {
        _expression = expression;
    }

    public string Interpret()
    {
        if (_expression == "2+2") return "4";
        if (_expression == "10*3") return "30";
        return "(math)" + _expression;
    }
}

public class InterpretCommand : ICommand
{
    private string _input;

    public InterpretCommand(string input)
    {
        _input = input;
    }

    public void Execute()
    {
        if (_input.StartsWith("::math "))
        {
            string expr = _input.Substring(7);
            var math = new MathExpression(expr);
            Console.WriteLine("Interpreted: " + math.Interpret());
        }
        else
        {
            Console.WriteLine("Unrecognized command: " + _input);
        }
    }
}

// Mediator Pattern
public interface IMediator
{
    void Notify(object sender, string ev);
}

public class EditorMediator : IMediator
{
    public void Notify(object sender, string ev)
    {
        Console.WriteLine($"Mediator received notification: {ev} from {sender.GetType().Name}");
    }
}

public class PluginComponent
{
    private readonly IMediator _mediator;
    public PluginComponent(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void Register()
    {
        _mediator.Notify(this, "Plugin Registered");
    }
}

// Client
class Program
{
    static void Main()
    {
        string pluginDir = "./plugins";
        PluginWatcher pluginWatcher = new PluginWatcher(pluginDir);
        PluginObserver observer = new PluginObserver();
        pluginWatcher.Attach(observer);
        pluginWatcher.StartWatching();
        Console.WriteLine("Watching for plugin changes in: " + pluginDir);

        List<MarkdownElement> elements = new List<MarkdownElement> { new BoldText(), new ItalicText() };
        MarkdownIterator iterator = new MarkdownIterator(elements);
        while (iterator.MoveNext())
        {
            var element = iterator.Current;
            Console.WriteLine("Processing: " + element.Render());
            element.Processed = true;
        }

        EditorContext editor = new EditorContext();
        editor.SetState(new ReadOnlyState());
        editor.ApplyState();
        editor.SetState(new EditableState());
        editor.ApplyState();

        string rawText = "\t\t Some raw text with tabs.  ";
        TextHandler trimHandler = new TrimHandler();
        TextHandler tabHandler = new ReplaceTabsHandler();
        trimHandler.SetNext(tabHandler);
        trimHandler.Handle(ref rawText);
        Console.WriteLine("Processed Text: " + rawText);

        // Macro command + Interpreter usage
        MacroCommand macroCommand = new MacroCommand();
        macroCommand.AddCommand(new RenderCommand(new ConsoleRenderStrategy(), elements));
        macroCommand.AddCommand(new InterpretCommand("::math 2+2"));
        macroCommand.AddCommand(new InterpretCommand("::math 10*3"));
        macroCommand.AddCommand(new InterpretCommand("::unknown test"));
        macroCommand.Execute();

        MarkdownProcessor processor = new SimpleMarkdownProcessor();
        processor.Process();

        // Mediator usage
        EditorMediator mediator = new EditorMediator();
        PluginComponent plugin = new PluginComponent(mediator);
        plugin.Register();

        while (true) Thread.Sleep(1000);
    }
}

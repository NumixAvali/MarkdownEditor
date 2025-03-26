using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

// Abstract Product - Markdown Element
public abstract class MarkdownElement
{
    public abstract string Render();
    public abstract MarkdownElement Clone(); // Prototype Pattern
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

// Observer Pattern - Plugin Watcher
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

// Client
class Program
{
    static void Main()
    {
        MarkdownProcessorFactory factory = new SimpleMarkdownProcessorFactory();
        List<MarkdownElement> elements = new List<MarkdownElement>();

        // Create elements using Factory Method
        elements.Add(factory.GetBoldFactory().CreateElement());
        elements.Add(factory.GetItalicFactory().CreateElement());
        
        // Clone stuff via Prototype
        MarkdownElement clonedElement = elements[0].Clone();
        elements.Add(clonedElement);

        // Bob the Builder
        MarkdownBuilder builder = new MarkdownBuilder();
        List<MarkdownElement> builtElements = builder
            .AddBold()
            .AddItalic()
            .Build();
        elements.AddRange(builtElements);

        // Observer Pattern - Plugin Watcher
        string pluginDir = "./plugins";
        PluginWatcher pluginWatcher = new PluginWatcher(pluginDir);
        PluginObserver observer = new PluginObserver();
        pluginWatcher.Attach(observer);
        pluginWatcher.StartWatching();

        Console.WriteLine("Watching for plugin changes in: " + pluginDir);

        // Strategy Pattern and Command Pattern Usage
        IRenderStrategy renderStrategy = new ConsoleRenderStrategy();
        ICommand renderCommand = new RenderCommand(renderStrategy, elements);

        // Macro Command Usage
        MacroCommand macroCommand = new MacroCommand();
        macroCommand.AddCommand(renderCommand);
        macroCommand.Execute();

        // Template Method Usage
        MarkdownProcessor processor = new SimpleMarkdownProcessor();
        processor.Process();

        // Keep the application running to observe plugin changes
        while (true) Thread.Sleep(1000);
    }
}

using System;
using System.Collections.Generic;

// Abstract Product - Markdown Element
public abstract class MarkdownElement
{
    public abstract string Render();
    public abstract MarkdownElement Clone(); // Prototype
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

// Observer Pattern
public interface IObserver
{
    void Update();
}

public class MarkdownObserver : IObserver
{
    public void Update()
    {
        Console.WriteLine("Markdown document has changed.");
    }
}

public class MarkdownSubject
{
    private List<IObserver> observers = new List<IObserver>();
    
    public void Attach(IObserver observer)
    {
        observers.Add(observer);
    }
    
    public void Notify()
    {
        foreach (var observer in observers)
        {
            observer.Update();
        }
    }
}

// Command Pattern
public interface ICommand
{
    void Execute();
}

public class RenderCommand : ICommand
{
    private IRenderStrategy renderStrategy;
    private List<MarkdownElement> elements;

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

        // Observer
        MarkdownSubject subject = new MarkdownSubject();
        MarkdownObserver observer = new MarkdownObserver();
        subject.Attach(observer);
        subject.Notify();

        // Strategy and Command pattern Usage
        IRenderStrategy renderStrategy = new ConsoleRenderStrategy();
        ICommand renderCommand = new RenderCommand(renderStrategy, elements);
        renderCommand.Execute();
    }
}

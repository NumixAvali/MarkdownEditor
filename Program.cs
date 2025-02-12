using System;
using System.Collections.Generic;

// Abstract Product - Markdown Element
public abstract class MarkdownElement
{
    public abstract string Render();
}

// Things that are being used in the factory? I guess??????
public class BoldText : MarkdownElement
{
    public override string Render() => "**Text, but BOLD**";
}

public class ItalicText : MarkdownElement
{
    public override string Render() => "*Text, but in Italy*";
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

// Abstract Factory for Markdown Processor
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

class Program
{
    static void Main()
    {
        MarkdownProcessorFactory factory = new SimpleMarkdownProcessorFactory();
        List<MarkdownElement> elements = new List<MarkdownElement>();

        // Create elements using Factory Method
        elements.Add(factory.GetBoldFactory().CreateElement());
        elements.Add(factory.GetItalicFactory().CreateElement());

        // Render elements
        foreach (var element in elements)
        {
            Console.WriteLine(element.Render());
        }
    }
}
